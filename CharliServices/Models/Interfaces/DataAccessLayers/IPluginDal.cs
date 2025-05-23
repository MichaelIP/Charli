using McpNetwork.Charli.Server.Models.Entities;

namespace McpNetwork.Charli.Server.Models.Interfaces.DataAccessLayers
{
    public interface IPluginDal
    {
        IEnumerable<Plugin> GetPlugins();
        bool RegisterPlugin(Plugin plugin);
        bool PluginIsActive(string pluginName, Version version);
    }
}
