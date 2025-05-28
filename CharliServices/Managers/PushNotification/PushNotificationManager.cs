using McpNetwork.Charli.Server.Constants;
using McpNetwork.Charli.Server.Helpers;
using McpNetwork.Charli.Server.Models;
using McpNetwork.Charli.Server.Models.Enums;
using McpNetwork.Charli.Server.Models.Interfaces.Managers;
using McpNetwork.Charli.Service.Environment.Interfaces.Managers.Master;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;

namespace McpNetwork.Charli.Server.Managers.PushNotification
{
    public class PushNotificationManager : ACharliManager, IPushNotificationManager
    {

        private const string LOCALE_PATH = "PushNotificationManager/Messages";
        private const string LOCALE_INITIALIZING = "Initializing";
        private const string LOCALE_INITIALIZED = "Initialized";

        private bool isInitialized;

        private string fcmProjectName;
        private string fcmCredentialFile;

        private ILoggerManager loggerManager;
        private IDatabaseManager databaseManager;
        private ILocalizationManager localizationManager;

        #region Abstracted properties

        public override string Name => "Push Notification Manager";

        public override string Information => string.Empty;

        public override EManagerStatus Status => EManagerStatus.Running;

        public override EManagersType Code => EManagersType.PushNotificationManager;

        public override Version Version => typeof(PushNotificationManager).Assembly.GetName().Version;

        public override string MobileEndPoint => "PushNotification/endPoints";

        public override string MobileIcon => ImageHelpers.ImageToBase64(ImageHelpers.GetResource("McpNetwork.Charli.Managers.PushNotificationManager.Resources.icon.png"), ImageFormat.Png);

        #endregion

        #region Abstracted methods

        public override bool Initialize(IServiceProvider serviceProvider)
        {
            if (!isInitialized)
            {

                loggerManager = (ILoggerManager)serviceProvider.GetService(typeof(ILoggerManager));
                databaseManager = (IDatabaseManager)serviceProvider.GetService(typeof(IDatabaseManager));
                localizationManager = (ILocalizationManager)serviceProvider.GetService(typeof(ILocalizationManager));
                var settingsManager = (ISettingManager)serviceProvider.GetService(typeof(ISettingManager));
                var wsManager = (IWebServicesManager)serviceProvider.GetService(typeof(IWebServicesManager));

                //wsManager.AddServices(Assembly.GetExecutingAssembly());

                LogInfo(GetTranslation(LOCALE_INITIALIZING));

                fcmProjectName = settingsManager.GetSetting<string>("PushNotificationsManager", "PushNotification.Project");
                fcmCredentialFile = settingsManager.GetSetting<string>("PushNotificationsManager", "PushNotification.CredentialFile");

                isInitialized = true;
                LogInfo(GetTranslation(LOCALE_INITIALIZED));
            }
            return isInitialized;
        }

        #endregion

        #region Interface implementation
        public bool Notify(int userId, PushNotificationPayloadModel payload)
        {
            var result = false;
            try
            {
                var deviceDal = databaseManager.DeviceDal;
                var devices = deviceDal.GetUserDevices(userId);
                if (devices.Any())
                {
                    var notifier = new FcmNotifier(fcmProjectName, fcmCredentialFile);
                    foreach (var device in devices)
                    {
                        result = result || notifier.Notify(device.NotificationToken, payload);
                    }
                    if (result)
                    {
                        LogInfo(string.Format("Sent notification [{0}] to user {1}", payload.NotificationId, userId));
                    }
                }
                else
                {
                    LogError(string.Format("No device found for user with Id {0} ! No notification sent.", userId));
                }
            }
            catch (Exception e)
            {
                LogError(e.Message);
            }

            return result;

        }

        public bool Notify(List<int> userIds, PushNotificationPayloadModel payload)
        {
            var result = true;
            foreach (var userId in userIds)
            {
                result &= Notify(userId, payload);
            }
            return result;
        }

        public bool Notify(string role, PushNotificationPayloadModel payload)
        {
            var result = true;
            var users = databaseManager.UserDal.GetUsersByRole(role);
            foreach (var user in users)
            {
                result &= Notify(user.UserId, payload);
            }
            return result;
        }
        #endregion

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

        private string GetTranslation(string localeName)
        {
            return GetTranslation(localeName, LOCALE_PATH);
        }

        private string GetTranslation(string localeName, string path)
        {
            return localizationManager.GetLocalization(LocalizationConstants.DefaultLocaleName, path, localeName);
        }

    }
}
