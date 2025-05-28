using McpNetwork.Charli.Server.Constants;
using McpNetwork.Charli.Server.Helpers;
using McpNetwork.Charli.Server.Managers.Scheduler.Scheduler;
using McpNetwork.Charli.Server.Models.Enums;
using McpNetwork.Charli.Server.Models.Interfaces.Managers;
using McpNetwork.Charli.Server.Models.Schedules;
using McpNetwork.Charli.Service.Environment.Interfaces.Managers.Master;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;

namespace McpNetwork.Charli.Server.Managers.Scheduler
{
    public class SchedulerManager : ACharliManager, ISchedulerManager
    {

        private const string LOCALE_PATH = "SchedulerManager/Messages";
        private const string LOCALE_INITIALIZING = "Initializing";
        private const string LOCALE_INITIALIZED = "Initialized";

        private TimerRunner scheduler;
        private bool isInitialized;

        private ILoggerManager loggerManager;
        private ILocalizationManager localizationManager;


        #region Abstracted properties

        public override string Name => "Scheduler Manager";

        public override string Information => "TO BE DONE !";

        public override EManagerStatus Status => EManagerStatus.Running;

        public override EManagersType Code => EManagersType.SchedulerManager;

        public override Version Version => typeof(SchedulerManager).Assembly.GetName().Version;

        public override string MobileEndPoint => "Scheduler/endPoints";

        public override string MobileIcon => ImageHelpers.ImageToBase64(ImageHelpers.GetResource("McpNetwork.Charli.Managers.SchedulerManager.Resources.icon.png"), ImageFormat.Png);

        #endregion

        #region Abstracted methods

        public override bool Initialize(IServiceProvider serviceProvider)
        {
            if (!isInitialized)
            {

                loggerManager = (ILoggerManager)serviceProvider.GetService(typeof(ILoggerManager));
                localizationManager = (ILocalizationManager)serviceProvider.GetService(typeof(ILocalizationManager));
                var wsManager = (IWebServicesManager)serviceProvider.GetService(typeof(IWebServicesManager));

                LogInfo(GetTranslation(LOCALE_INITIALIZING));

                //wsManager.AddServices(Assembly.GetExecutingAssembly());

                isInitialized = true;
                LogInfo(GetTranslation(LOCALE_INITIALIZED));
            }
            return isInitialized;

        }

        #endregion

        #region Interface implementation
        public Guid AddSchedule(ScheduleModel schedule, Action<Guid, ScheduleModel> action)
        {
            return scheduler.AddSchedule(schedule, action);
        }

        public void RemoveSchedule(Guid schedule)
        {
            scheduler.RemoveSchedule(schedule);
        }

        public void Pause()
        {
            Stop();
        }

        public void Resume()
        {
            Start();
        }

        public void Start()
        {
            StartScheduler();
        }

        public void Stop()
        {
            scheduler = null;
        }

        #endregion


        private void StartScheduler()
        {
            scheduler = new TimerRunner(loggerManager);

        }


        #region Log
        private void LogInfo(string message, [CallerMemberName] string caller = null)
        {
            loggerManager.LogInformation(EManagersType.PushNotificationManager.ToString(), message, null, caller);
        }

        private void LogError(string message, [CallerMemberName] string caller = null)
        {
            loggerManager.LogError(EManagersType.PushNotificationManager.ToString(), message, null, caller);
        }
        #endregion

        #region Translations
        private string GetTranslation(string localeName)
        {
            return GetTranslation(localeName, LOCALE_PATH);
        }

        private string GetTranslation(string localeName, string path)
        {
            return localizationManager.GetLocalization(LocalizationConstants.DefaultLocaleName, path, localeName);
        }
        #endregion

    }
}
