using McpNetwork.Charli.Server.Models.Interfaces.Managers;
using MQTTnet;
using MQTTnet.Packets;
using MQTTnet.Protocol;
using MQTTnet.Server;
using System.Diagnostics;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace McpNetwork.Charli.Server.Managers.MessageQueue.Server
{
    internal class CharliMqttServer(IMessageQueueManager owner, ILoggerManager? loggerManager, string rootPath, string sessionUid) : IDisposable
    {

        private MqttServer? mqttServer;
        private readonly string rootPath = rootPath;
        private readonly MqttFactory mqttFactory = new();
        private readonly ILoggerManager? loggerManager = loggerManager;
        private readonly IPluginManager? pluginManager = owner.PluginManager;

        private bool disposed;
        private readonly string sessionUid = sessionUid;
        private readonly SortedList<string, string> connectedClientIds = [];


        public void Stop()
        {
            if (mqttServer == null)
            {
                return;
            }
            mqttServer.StopAsync().Wait();
            mqttServer.Dispose();

        }

        public async Task Start()
        {
            var certificatePath = Path.Combine(rootPath, "..", "Certificates", "charli.pfx");
            var certificate = new X509Certificate2(certificatePath, "GetANew1", X509KeyStorageFlags.Exportable);

            var optionsBuilder = new MqttServerOptionsBuilder()
                .WithoutDefaultEndpoint() // This call disables the default unencrypted endpoint on port 1883
                .WithEncryptedEndpoint()
                .WithEncryptedEndpointPort(8883)
                //.WithDefaultEndpointBoundIPAddress(new System.Net.IPAddress(new List<byte> { 192, 168, 1, 215 }.ToArray()))
                .WithEncryptionCertificate(certificate.Export(X509ContentType.Pfx))
                .WithEncryptionSslProtocol(SslProtocols.Tls12)
                .WithRemoteCertificateValidationCallback(Callback)
                .Build();

            mqttServer = mqttFactory.CreateMqttServer(optionsBuilder);
            mqttServer.ValidatingConnectionAsync += OnValidatingConnectionAsync;
            mqttServer.ClientDisconnectedAsync += MqttServer_ClientDisconnectedAsync;
            mqttServer.ClientSubscribedTopicAsync += MqttServer_ClientSubscribedTopicAsync;

            mqttServer.InterceptingPublishAsync += MqttServer_InterceptingPublishAsync;
            mqttServer.InterceptingSubscriptionAsync += MqttServer_InterceptingSubscriptionAsync;

            await mqttServer.StartAsync();
        }


        private Task OnValidatingConnectionAsync(ValidatingConnectionEventArgs args)
        {

            if (connectedClientIds.ContainsKey(args.ClientId))
            {
                args.ReasonCode = MqttConnectReasonCode.ClientIdentifierNotValid;
                return Task.CompletedTask;
            }

            var isAllowed = false;
            switch (args.UserName)
            {
                case "CharliManager":
                    if (GetUserProperty(args.UserProperties, "SessionUid") != sessionUid)
                    {
                        args.ReasonCode = MqttConnectReasonCode.NotAuthorized;
                    }
                    if (args.Password != "CharliPluginPassword")
                    {
                        args.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                    }
                    break;
                case "PluginRegistration":
                    if (args.Password != "CharliPluginPassword")
                    {
                        args.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                    }
                    var pluginName = GetUserProperty(args.UserProperties, "PluginName");
                    if (Version.TryParse(GetUserProperty(args.UserProperties, "PluginVersion"), out var pluginVersion))
                    {
                        isAllowed = pluginManager?.IsPluginAllowed(pluginName, pluginVersion) ?? false;
                        if (isAllowed)
                        {
                            pluginManager?.SetPluginStarted(args.ClientId, pluginName, pluginVersion);
                            connectedClientIds.Add(args.ClientId, pluginName);
                        }
                        else
                        {
                            pluginManager?.RegisterPlugin(pluginName, pluginVersion);
                            args.ReasonCode = MqttConnectReasonCode.NotAuthorized;
                        }
                    }
                    else
                    {
                        args.ReasonCode = MqttConnectReasonCode.BadAuthenticationMethod;
                    }
                    break;
                default:
                    args.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                    break;
            }

            return Task.CompletedTask;
        }

        private async Task MqttServer_ClientDisconnectedAsync(ClientDisconnectedEventArgs args)
        {
            pluginManager?.SetPluginStopped(args.ClientId);
            connectedClientIds.Remove(args.ClientId);
            await Task.Delay(100);
        }


        private Task MqttServer_InterceptingSubscriptionAsync(InterceptingSubscriptionEventArgs args)
        {
            var plugin = connectedClientIds.FirstOrDefault(x => x.Key == args.ClientId);

            var logMessage = string.Format("Enabled subscription on topic [{0}] for plugin [{1}]", args.TopicFilter.Topic, plugin.Value);
            if (!IsValidTopic(args.TopicFilter.Topic, plugin.Value))
            {
                logMessage = string.Format("Refused subscription on topic [{0}] for plugin [{1}]", args.TopicFilter.Topic, plugin.Value);
                args.ReasonString = "Topic is not allowed";
                args.Response.ReasonString = "Topic is not allowed";
                args.Response.ReasonCode = MqttSubscribeReasonCode.NotAuthorized;
                args.ProcessSubscription = false;
            }
            loggerManager?.LogInformation("CharliMqttServer", logMessage);
            return Task.CompletedTask;

        }

        private Task MqttServer_ClientSubscribedTopicAsync(ClientSubscribedTopicEventArgs args)
        {
            Debug.WriteLine(string.Format("Received subscription to [{0}] from [{1}]", args.TopicFilter.Topic, args.ClientId));
            //return Task.FromException(new NotImplementedException());
            return Task.CompletedTask;
        }

        private Task MqttServer_InterceptingPublishAsync(InterceptingPublishEventArgs arg)
        {
            var plugin = connectedClientIds.FirstOrDefault(x => x.Key == arg.ClientId);
            if (!IsValidTopic(arg.ApplicationMessage.Topic, plugin.Value))
            {
                arg.Response.ReasonString = "Topic is not allowed";
                arg.Response.ReasonCode = MqttPubAckReasonCode.NotAuthorized;
                arg.ProcessPublish = false;
            }

            loggerManager?.LogDebug("CharliMqttServer", string.Format("Received message on topic [{0}] from [{1}]", arg.ApplicationMessage.Topic, plugin.Value));
            return Task.CompletedTask;
        }


        private bool Callback(object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors)
        {
            // TODO --> Validate certificate...
            return true;
        }

        private bool IsValidTopic(string topic, string pluginName)
        {
            var result = true;
            switch (topic)
            {
                case "McpNetwork/Charli/Plugins/General":
                    break;
                default:
                    var allowedTopic = string.Format("McpNetwork/Charli/Plugins/{0}", pluginName);
                    result = topic.StartsWith(allowedTopic);
                    break;
            }
            return result;
        }

        private string GetUserProperty(List<MqttUserProperty> userProperties, string propertyName)
        {
            return userProperties.FirstOrDefault(x => x.Name == propertyName)?.Value ?? "INVALID";
        }

        #region IDisposable pattern
        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }
        protected async virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                // TODO: dispose managed state (managed objects).
                if (mqttServer?.IsStarted ?? false)
                {
                    await mqttServer.StopAsync();
                }
                mqttServer?.Dispose();
            }

            disposed = true;
        }

        #endregion

    }
}
