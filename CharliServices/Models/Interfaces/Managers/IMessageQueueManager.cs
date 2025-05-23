namespace McpNetwork.Charli.Server.Models.Interfaces.Managers
{
    internal interface IMessageQueueManager : ICharliManager, IDisposable
    {
        IPluginManager? PluginManager { get; }
        ISettingManager? SettingManager { get; } 

    }
}
