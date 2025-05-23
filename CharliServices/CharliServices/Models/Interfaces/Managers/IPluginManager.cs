namespace McpNetwork.Charli.Server.Models.Interfaces.Managers
{
    internal interface IPluginManager : ICharliManager
    {
        void SetPluginStopped(string uid);
        void SetPluginStarted(string uid, string pluginName, Version version);
        void RegisterPlugin(string pluginName, Version version);
        bool IsPluginAllowed(string pluginName, Version version);
    }
}
