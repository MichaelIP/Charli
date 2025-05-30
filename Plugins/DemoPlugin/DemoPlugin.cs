using McpNetwork.Charli.Environment.Enums;
using McpNetwork.Charli.Environment.Interfaces;
using McpNetwork.Charli.Environment.Models.MessageQueue;

namespace McpNetwork.Charli.Plugins.DemoPlugin
{
    internal class DemoPlugin : IPlugin
    {

        private EPluginStatus status;

        public string Name => "DemoPlugin";

        public string Description => "This plugin demonstrates how a Charli plugin works";

        public Version Version => typeof(DemoPlugin).Assembly.GetName().Version;

        private PluginMqttClient? mqttClient;

        public void Start()
        {
            mqttClient = new PluginMqttClient(this);
            mqttClient.Start();
            status = EPluginStatus.Running;

            Task.Factory.StartNew(() => SendMessages());
        }

        public void Stop()
        {
            status = EPluginStatus.Stopped;
        }

        public EPluginStatus Status() => status;

        private void SendMessages()
        {
            var messageNb = 1;
            while (status == EPluginStatus.Running)
            {

                if (mqttClient?.IsConnected ?? false)
                {
                    var message = new MessagePayload
                    {
                        Sender = "DemoPlugin",
                        TimeStamp = DateTime.Now.ToString(),
                        Message = string.Format("Message #{0} from DemoPlugin", messageNb++)
                    };
                    Console.WriteLine("Sending message to CharliServer");
                    mqttClient.SendMessage(message);
                }
                Task.Delay(5000).Wait();
            }
        }

    }

}
