using McpNetwork.Charli.Server.Models.Attributes;
using McpNetwork.Charli.Server.Models.Interfaces.Managers;

namespace McpNetwork.Charli.Server.Models.Enums
{
    public enum EManagersType
    {
        [ManagerInterface(typeof(ILoggerManager))]
        LoggerManager = 0x010,
        [ManagerInterface(typeof(ISettingManager))]
        SettingManager = 0x020,
        [ManagerInterface(typeof(ILocalizationManager))]
        LocalizationManager = 0x030,
        [ManagerInterface(typeof(IDatabaseManager))]
        DatabaseManager = 0x040,
        [ManagerInterface(typeof(IMessageQueueManager))]
        MessageQueueManager = 0x50,
        [ManagerInterface(typeof(IPushNotificationManager))]
        PushNotificationManager = 0x100,
        [ManagerInterface(typeof(IVoiceToolsManager))]
        VoiceToolsManager = 0x200,
        [ManagerInterface(typeof(ISecurityManager))]
        SecurityManager = 0x300,
        [ManagerInterface(typeof(ISchedulerManager))]
        SchedulerManager = 0x400,
        [ManagerInterface(typeof(IPluginManager))]
        PluginManager = 0x500,
        [ManagerInterface(typeof(IWebServicesManager))]
        WebServicesManager = 0x600,
    }
}
