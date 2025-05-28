using McpNetwork.Charli.Server.Models.Enums;

namespace McpNetwork.Charli.Managers.PluginManager
{
    internal class PluginInfo
    {
        public string MqttClientId { get; set; }
        public string Name { get; set; }
        public Version Version { get; set; }

        public EPluginStatus PluginStatus { get; set; } = EPluginStatus.NotStarted;
        public DateTime? StartTime { get; set; }
        public DateTime? StopTime { get; set; }

        public PluginInfo(string clientId, string name, Version version)
        {
            Version = version ?? throw new ArgumentNullException(nameof(version));
            MqttClientId = clientId ?? throw new ArgumentNullException(nameof(clientId));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }

}
