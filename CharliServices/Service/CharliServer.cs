using McpNetwork.Charli.Server.Constants;
using McpNetwork.Charli.Server.Helpers;
using McpNetwork.Charli.Server.Models;
using McpNetwork.Charli.Server.Models.Enums;
using McpNetwork.Charli.Server.Models.Interfaces;
using McpNetwork.Charli.Server.Models.Interfaces.Managers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Globalization;
using System.Reflection;

namespace McpNetwork.Charli.Server.Service
{
    internal class CharliServer
    {

        private const string MODULE_NAME = "CharliServer";

        #region locale constants

        private const string LOCALE_PATH_ERROR_MESSAGES = LOCALE_PATH_CHARLI + "/ErrorMessages";

        private const string LOCALE_ACTION_STARTING = "Starting";
        private const string LOCALE_ACTION_STOPPING = "Stopping";

        private const string LOCALE_PATH_CHARLI = "CharliServer";
        private const string LOCALE_PATH_ACTIONS = LOCALE_PATH_CHARLI + "/Actions";
        private const string LOCALE_PATH_ACTIONS_TITLE = LOCALE_PATH_CHARLI + "/Titles";

        private const string LOCALE_ACTION_CHARLI_STARTED = "CharliStarted";
        private const string LOCALE_ACTION_CHARLI_STOPPING = "CharliStopping";

        private const string LOCALE_MANAGER_START = "StartManager";
        private const string LOCALE_MANAGER_STOP = "StopManager";

        #endregion

        private ServiceProvider serviceProvider;
        private readonly ServiceCollection services = new();

        #region Interface implementation

        private ISettingManager SettingManager => this.serviceProvider.GetService<ISettingManager>();

        private ILocalizationManager LocalizationManager => serviceProvider.GetService<ILocalizationManager>();

        private IPushNotificationManager PushNotificationManager => serviceProvider.GetService<IPushNotificationManager>();

        private ILoggerManager LoggerManager => serviceProvider.GetService<ILoggerManager>();

        private IVoiceToolsManager VoiceToolsManager => serviceProvider.GetService<IVoiceToolsManager>();

        #endregion

        public void Start(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();

            var rootPath = config.GetValue<string>("root");
            if (rootPath == null)
            {
                throw new InvalidExpressionException(nameof(rootPath));
            }

            if (InitializeSystems(rootPath))
            {
                this.LoggerManager.LogInformation(MODULE_NAME, this.LocalizationManager.GetLocalization(LocalizationConstants.DefaultLocaleName, LOCALE_PATH_ACTIONS, LOCALE_ACTION_STARTING));
                this.StartSystems();
                this.PushNotificationManager.Notify("Administrator", new PushNotificationPayloadModel
                {
                    Message = this.LocalizationManager.GetLocalization(LocalizationConstants.DefaultLocaleName, LOCALE_PATH_ACTIONS, LOCALE_ACTION_CHARLI_STARTED),
                    NotificationId = "CHARLI-01",
                    Sender = MODULE_NAME,
                    Title = this.LocalizationManager.GetLocalization(LocalizationConstants.DefaultLocaleName, LOCALE_PATH_ACTIONS_TITLE, LOCALE_ACTION_CHARLI_STARTED),
                });

                this.VoiceToolsManager?.SetLanguage(this.SettingManager.DefaultCulture);
                this.VoiceToolsManager?.Speak("Charli is now listening");
            }

        }

        public void Stop()
        {
            this.LoggerManager.LogInformation(MODULE_NAME, this.LocalizationManager.GetLocalization(LocalizationConstants.DefaultLocaleName, LOCALE_PATH_ACTIONS, LOCALE_ACTION_STOPPING));
            this.PushNotificationManager.Notify("Administrator", new PushNotificationPayloadModel
            {
                Message = this.LocalizationManager.GetLocalization(LocalizationConstants.DefaultLocaleName, LOCALE_PATH_ACTIONS, LOCALE_ACTION_CHARLI_STOPPING),
                NotificationId = "CHARLI-02",
                Sender = MODULE_NAME,
                Title = this.LocalizationManager.GetLocalization(LocalizationConstants.DefaultLocaleName, LOCALE_PATH_ACTIONS_TITLE, LOCALE_ACTION_CHARLI_STOPPING),
            });
            this.StopSystems();
        }


        private void StartSystems()
        {
            foreach (var manager in this.GetManagers())
            {
                try
                {
                    Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Starting manager {0}", manager));
                    manager.Value.Start();
                }
                catch (Exception e)
                {
                    this.LoggerManager.LogCritical(MODULE_NAME, string.Format(CultureInfo.InvariantCulture, this.LocalizationManager.GetLocalization(LocalizationConstants.DefaultLocaleName, LOCALE_PATH_ERROR_MESSAGES, LOCALE_MANAGER_START), manager), e);
                }
            }
        }

        private void StopSystems()
        {
            foreach (var manager in this.GetManagers().OrderByDescending(m => m.Key))
            {
                try
                {
                    Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Stopping manager {0}", manager));
                    manager.Value.Stop();
                }
                catch (Exception e)
                {
                    this.LoggerManager?.LogCritical(MODULE_NAME, string.Format(CultureInfo.InvariantCulture, this.LocalizationManager.GetLocalization(LocalizationConstants.DefaultLocaleName, LOCALE_PATH_ERROR_MESSAGES, LOCALE_MANAGER_STOP), manager), e);
                }
            }
        }

        #region Managers
        private bool InitializeSystems(string rootPath)
        {

            var result = true;
            var settingsFile = Path.Combine(rootPath, "Settings.xml");

            //this.services.AddSingleton(typeof(IMessageQueueManager), new MessageQueueManager());
            //this.services.AddSingleton(typeof(IDatabaseManager), new DatabaseManager());
            //this.services.AddSingleton(typeof(ILocalizationManager), new LocalizationManager());
            //this.services.AddSingleton(typeof(ILoggerManager), new LoggerManager());
            //this.services.AddSingleton(typeof(IPluginManager), new PluginManager());
            //this.services.AddSingleton(typeof(IPushNotificationManager), new PushNotificationManager());
            //this.services.AddSingleton(typeof(ISchedulerManager), new SchedulerManager());
            //this.services.AddSingleton(typeof(ISecurityManager), new SecurityManager());
            //this.services.AddSingleton(typeof(ISettingManager), new SettingManager());
            //this.services.AddSingleton(typeof(IVoiceToolsManager), new VoiceToolsManager());
            //this.services.AddSingleton(typeof(IWebServicesManager), new WebServicesManager());

            this.serviceProvider = this.services.BuildServiceProvider();

            var settingManager = this.serviceProvider.GetService<ISettingManager>();

            // Manager initialization
            foreach (var manager in this.GetManagers())
            {
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Initializing manager {0}", manager));
                if (manager.Value != null)
                {
                    switch (manager.Value.Code)
                    {
                        case EManagersType.SettingManager:
                            settingManager.SettingsFileName = settingsFile;
                            break;
                        case EManagersType.LoggerManager:
                            var fi = new FileInfo(settingsFile);
                            this.LoggerManager.LogConfigurationFile = Path.Combine(fi.FullName.Replace(fi.Name, string.Empty), "log4net.config");
                            this.LoggerManager.LogInformation("Charli", "/--------------------------------------------------\\");
                            this.LoggerManager.LogInformation("Charli", "|     C H A R L I                                  |");
                            this.LoggerManager.LogInformation("Charli", "|                                      is starting |");
                            this.LoggerManager.LogInformation("Charli", "\\--------------------------------------------------/");
                            break;
                    }
                    manager.Value.Initialize(this.serviceProvider);
                }
            }

            if (result)
            {
                var webServicesManager = this.serviceProvider.GetService<IWebServicesManager>();
                webServicesManager?.AddServices(Assembly.GetExecutingAssembly());
            }

            return result;
        }

        private SortedList<byte, ICharliManager> GetManagers()
        {
            var result = new SortedList<byte, ICharliManager>();
            var counter = (byte)0;
            foreach (var managerName in Enum.GetValues(typeof(EManagersType)))
            {

                var managerType = ((EManagersType)managerName).GetManagerImplementedType();
                var manager = this.serviceProvider.GetService(managerType) as ICharliManager;
                result.Add(++counter, manager);
            }

            return result;
        }

        #endregion


    }

}
