using McpNetwork.Charli.Environment.Interfaces;
using McpNetwork.Charli.Environment.Models.MessageQueue;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace McpNetwork.Charli.Plugins.DemoPlugin
{
    internal class PluginMqttClient
    {

        private const string allTopics = "McpNetwork/Charli/Plugins/#";
        private const string invalidTopic = "McpNetwork/Charli/Plugins/OtherPlugin";
        private const string charliPluginsTopic = "McpNetwork/Charli/Plugins/General";
        private const string demoPluginTopic = "McpNetwork/Charli/Plugins/DemoPlugin";

        private IMqttClient? mqttClient;
        private readonly IPlugin plugin;
        private readonly MqttFactory mqttFactory = new();

        public bool IsConnected { get; private set; }

        public PluginMqttClient(IPlugin plugin)
        {
            IsConnected = false;
            this.plugin = plugin;
        }

        public void Start()
        {
            Task.Factory.StartNew(() => ConnectCharliMqtt());

        }

        private void ConnectCharliMqtt()
        {
            while (!IsConnected)
            {
                var startTask = Task.Factory.StartNew(() => CreateAndStartClient());
                startTask.Wait();
                if (!IsConnected)
                {
                    Console.WriteLine("Unable to connect MQTT... ");
                    Task.Delay(5000).Wait();
                }
            }
            Task.Delay(250).Wait();
            Task.Factory.StartNew(() => SubscribeTopics(allTopics));
            Task.Factory.StartNew(() => SubscribeTopics(invalidTopic));
            Task.Factory.StartNew(() => SubscribeTopics(demoPluginTopic));
            Task.Factory.StartNew(() => SubscribeTopics(charliPluginsTopic));

        }

        private void CreateAndStartClient()
        {
            Console.WriteLine("Connecting to MQTT server.");
            var clientTlsOptions = new MqttClientTlsOptions
            {
                UseTls = true,
                AllowUntrustedCertificates = true,
                SslProtocol = SslProtocols.Tls12,
                CertificateValidationHandler = Callback2
            };

            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithProtocolVersion(MQTTnet.Formatter.MqttProtocolVersion.V500)
                .WithTcpServer("127.0.0.1", 8883)
                .WithTlsOptions(clientTlsOptions)
                .WithCredentials("PluginRegistration", "CharliPluginPassword")
                .WithUserProperty("PluginName", "DemoPlugin")
                .WithUserProperty("PluginVersion", plugin.Version.ToString(3))
                .WithClientId(Guid.NewGuid().ToString())
                .WithCleanSession()
                .WithKeepAlivePeriod(TimeSpan.FromMinutes(10))
                .Build();

            try
            {
                mqttClient = mqttFactory.CreateMqttClient();
                mqttClient.DisconnectedAsync += MqttClient_DisconnectedAsync;
                mqttClient.ApplicationMessageReceivedAsync += MqttClient_ApplicationMessageReceivedAsync;

                mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None).Wait();
                Console.WriteLine("Connection successfull.");
                IsConnected = true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Connection failed. Errro: {0}", ex.Message));
            }
        }

        private async Task SubscribeTopics(string topic)
        {
            if (mqttClient == null)
            {
                Console.WriteLine("MQTT client is null, cannot subscribe to topic.");
                return;
            }

            try
            {
                var topicFilterBuilder = new MqttTopicFilterBuilder()
                    .WithTopic(topic)
                    .Build();
                var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
                    .WithTopicFilter(topicFilterBuilder)
                    .Build();
                var result = await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

                Console.WriteLine(string.Format("Subscribed to topic [{0}] : {1}", topic, result?.Items.First().ResultCode != MqttClientSubscribeResultCode.NotAuthorized ? "SUCCESS" : "FAILURE"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error subscribing topic: {0}", ex.Message));
            }
        }

        private Task MqttClient_DisconnectedAsync(MqttClientDisconnectedEventArgs arg)
        {
            //loggerManager.LogError("Disconnected ! Reason: {0}", arg.Reason.ToString());
            return Task.CompletedTask;
        }
        private Task MqttClient_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
        {
            var payload = arg.ApplicationMessage.ConvertPayloadToString();
            Debug.WriteLine(string.Format("Received message from [{1}] on topic [{0}] :", arg.ApplicationMessage.Topic, arg.ClientId));
            Debug.WriteLine(string.Format("Message [{0}] :", payload));
            return Task.CompletedTask;
        }

        public void SendMessage(MessagePayload message)
        {
            var jsonPayload = JsonConvert.SerializeObject(message);
            var jsonBytes = Encoding.UTF8.GetBytes(jsonPayload);
            mqttClient?.PublishAsync(new MqttApplicationMessage
            {
                Topic = demoPluginTopic,
                PayloadSegment = new ArraySegment<byte>(jsonBytes),
                PayloadFormatIndicator = MQTTnet.Protocol.MqttPayloadFormatIndicator.CharacterData
            });
            mqttClient?.PublishAsync(new MqttApplicationMessage
            {
                Topic = invalidTopic,
                PayloadSegment = new ArraySegment<byte>(jsonBytes),
                PayloadFormatIndicator = MQTTnet.Protocol.MqttPayloadFormatIndicator.CharacterData
            });
        }

        #region Certificate validation

        private bool Callback(object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        private bool Callback2(MqttClientCertificateValidationEventArgs args)
        {
            return Callback(this, args.Certificate, args.Chain, args.SslPolicyErrors);
        }

        #endregion

    }
}
