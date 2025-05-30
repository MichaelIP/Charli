using McpNetwork.Charli.Environment.Enums;
using McpNetwork.Charli.Environment.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McpNetwork.Charli.Plugins.VideoMonitoring
{
    internal class VideoMonitoring : IPlugin
    {
        private EPluginStatus status;

        public string Name => "VideoMonitoring";

        public string Description => "This plugin ensure video monitoring";

        public Version Version => typeof(VideoMonitoring).Assembly.GetName().Version ?? new(0, 0, 0);

        private PluginMqttClient? mqttClient;

        public void Start()
        {
            mqttClient = new PluginMqttClient(this);
            mqttClient.Start();
            status = EPluginStatus.Running;

            //Task.Factory.StartNew(() => SendMessages());
        }

        public void Stop()
        {
            status = EPluginStatus.Stopped;
            mqttClient?.Stop();
        }

        public EPluginStatus Status() => status;
    }
}
