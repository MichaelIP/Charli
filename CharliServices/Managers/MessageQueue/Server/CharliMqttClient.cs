using McpNetwork.Charli.Server.Models.Interfaces.Managers;
using MQTTnet;
using MQTTnet.Client;
using System.Diagnostics;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace McpNetwork.Charli.Server.Managers.MessageQueue.Server
{
    internal class CharliMqttClient(IMessageQueueManager owner, ILoggerManager? loggerManager, string rootPath, string sessionUid) : IDisposable
    {

        private const string charliPlugins = "McpNetwork/Charli/Plugins/#";

        private bool disposed;
        private readonly string sessionUid = sessionUid;

        private IMqttClient? mqttClient;
        private readonly IMessageQueueManager messageQueueManager = owner;
        private readonly MqttFactory mqttFactory = new();
        private readonly ILoggerManager? loggerManager = loggerManager;

        public void Stop()
        {
            mqttClient?.DisconnectAsync().Wait();
            mqttClient?.Dispose();
        }

        public async Task Start()
        {
            var clientTlsOptions = new MqttClientTlsOptions
            {
                UseTls = true,
                AllowUntrustedCertificates = true,
                SslProtocol = SslProtocols.Tls12,
                CertificateValidationHandler = Callback
            };

            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithProtocolVersion(MQTTnet.Formatter.MqttProtocolVersion.V500)
                .WithTcpServer("127.0.0.1", 8883)
                .WithTlsOptions(clientTlsOptions)
                .WithCredentials("CharliManager", "BadPassword")
                .WithClientId(Guid.NewGuid().ToString())
                .WithCleanSession()
                .WithKeepAlivePeriod(TimeSpan.FromMinutes(10))
            .Build();

            try
            {
                mqttClient = mqttFactory.CreateMqttClient();
                mqttClient.DisconnectedAsync += MqttClient_DisconnectedAsync;
                mqttClient.ApplicationMessageReceivedAsync += MqttClient_ApplicationMessageReceivedAsync;

                await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
            }
            catch (Exception ex)
            {
                loggerManager?.LogError(ex.Message, ex);
            }

            try
            {
                var topicFilterBuilder = new MqttTopicFilterBuilder()
                    .WithTopic(charliPlugins)
                    .Build();
                var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
                    .WithTopicFilter(topicFilterBuilder)
                    .Build();
                await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
            }
            catch (Exception ex)
            {
                loggerManager?.LogError(ex.Message, ex);
            }

        }

        private bool Callback(MqttClientCertificateValidationEventArgs args)
        {
            return ValidateCertificate(this, args.Certificate, args.Chain, args.SslPolicyErrors);
        }
        private bool ValidateCertificate(object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors)
        {
            // TODO --> Validate certificate...
            return true;
        }


        private Task MqttClient_DisconnectedAsync(MqttClientDisconnectedEventArgs arg)
        {
            loggerManager?.LogError("Disconnected ! Reason: {0}", arg.Reason.ToString());
            return Task.CompletedTask;
        }
        private Task MqttClient_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
        {
            Debug.WriteLine(string.Format("Received message from client {0}", arg.ClientId));
            Debug.WriteLine(string.Format("Topic : {0}", arg.ApplicationMessage.Topic));
            var payload = arg.ApplicationMessage.ConvertPayloadToString();
            Debug.WriteLine(string.Format("Payload: {0}", payload));
            return Task.CompletedTask;
        }

        #region IDisposable pattern
        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }
        protected void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                // TODO: dispose managed state (managed objects).

            }

            if (mqttClient?.IsConnected ?? false)
            {
                mqttClient?.DisconnectAsync();
            }
            mqttClient?.Dispose();

            disposed = true;
        }

        #endregion
    }
}
