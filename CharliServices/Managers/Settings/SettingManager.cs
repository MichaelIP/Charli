using McpNetwork.Charli.Environment.Helpers;
using McpNetwork.Charli.Server.Helpers;
using McpNetwork.Charli.Server.Models.Enums;
using McpNetwork.Charli.Server.Models.Interfaces.Managers;
using McpNetwork.Charli.Server.Models.Setting;
using McpNetwork.Charli.Service.Environment.Interfaces.Managers.Master;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;

namespace McpNetwork.Charli.Server.Managers.Settings
{
    public class SettingManager : ACharliManager, ISettingManager
    {

        private XmlSettings settings;
        private bool isInitialized = false;
        private EManagerStatus status = EManagerStatus.Stopped;

        private ILoggerManager loggerManager;

        #region Abstracted properties
        public override string Name => "Setting Manager";

        public override string Information => "TO BE DONE";

        public override EManagerStatus Status => status;

        public override EManagersType Code => EManagersType.SettingManager;

        public override Version Version => typeof(SettingManager).Assembly.GetName().Version;

        public override string MobileEndPoint => "Setting/endPoints";

        public override string MobileIcon => ImageHelpers.ImageToBase64(ImageHelpers.GetResource("McpNetwork.Charli.Managers.SettingManager.Resources.icon.png"), ImageFormat.Png);
        #endregion

        #region Interface properties
        public string SettingsFileName { get; set; }
        public string DefaultCulture => settings.Globals.Locale;
        #endregion

        public override bool Initialize(IServiceProvider serviceProvider)
        {
            if (!isInitialized)
            {

                loggerManager = (ILoggerManager)serviceProvider.GetService(typeof(ILoggerManager));
                var wsManager = (IWebServicesManager)serviceProvider.GetService(typeof(IWebServicesManager));

                var fi = new FileInfo(SettingsFileName);
                LogInfo("SettingManager is initializing");
                LogInfo(string.Format("Using setting file [{0}]", SettingsFileName));
                isInitialized = XmlValidationHelper.ValidateDocument(SettingsFileName);

                if (!isInitialized)
                {
                    status = EManagerStatus.Faulted;
                    LogError(string.Format("Settings file has not been validated agains xsd ! Cannot load it !"));
                }
                else
                {
                    status = EManagerStatus.Running;
                    settings = XmlValidationHelper.LoadDocument<XmlSettings>(SettingsFileName);
                }

                //wsManager.AddServices(Assembly.GetExecutingAssembly());

                LogInfo("SettingManager initialized");
            }
            return isInitialized;
        }

        #region Getters

        #region ConnectionString
        public string ConnectionString(string connectionName)
        {
            string result = string.Empty;
            var conn = settings.ConnectionStrings.ConnectionString.FirstOrDefault(c => c.Name == connectionName);
            if (conn != null)
            {
                result = conn.Value;
            }
            return result;
        }
        #endregion

        #region Settings

        public bool GetSetting(string settingGroup, string settingName, out int value)
        {
            bool result = true;
            value = default;
            try
            {
                value = int.Parse(GetSetting(settingGroup, settingName));
            }
            catch (Exception e)
            {
                LogError(e);
                result = false;
            }
            return result;
        }

        public bool GetSetting(string settingGroup, string settingName, out DateTime value)
        {
            bool result = true;
            value = default;
            try
            {
                value = DateTime.Parse(GetSetting(settingGroup, settingName));
            }
            catch (Exception e)
            {
                LogError(e);
                result = false;
            }
            return result;
        }

        public bool GetSetting(string settingGroup, string settingName, out bool value)
        {
            bool result = true;
            value = default;
            try
            {
                value = bool.Parse(GetSetting(settingGroup, settingName));
            }
            catch (Exception e)
            {
                LogError(e);
                result = false;
            }
            return result;
        }

        public bool GetSetting(string settingGroup, string settingName, out double value)
        {
            bool result = true;
            value = default;
            try
            {
                value = double.Parse(GetSetting(settingGroup, settingName));
            }
            catch (Exception e)
            {
                LogError(e);
                result = false;
            }
            return result;
        }

        public bool GetSetting(string settingGroup, string settingName, out string value)
        {
            bool result = true;
            value = null;
            try
            {
                value = GetSetting(settingGroup, settingName);
            }
            catch (Exception e)
            {
                LogError(e);
                result = false;
            }
            return result;
        }

        public T GetSetting<T>(string settingGroup, string settingName)
        {
            //var t = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

            T result = default;

            var t = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
            object value = GetSetting(settingGroup, settingName);
            switch (t.BaseType.Name)
            {
                case "Enum":
                    result = (T)Enum.Parse(typeof(T), value.ToString());
                    break;
                default:
                    result = value == null ? result : (T)Convert.ChangeType(value, t);
                    break;
            }
            return result;

            //var result = (T)Convert.ChangeType(this.GetSetting(settingGroup, settingName), t);
            //return result;
        }

        public T GetSetting<T>(string settingGroup, string settingName, object defaultValue)
        {
            T result = GetSetting<T>(settingGroup, settingName);
            T valueNotSet = default;
            if (result == null || result.Equals(valueNotSet))
            {
                result = (T)defaultValue;
            }
            return result;
        }

        private string GetSetting(string settingGroup, string settingName)
        {
            string result = null;
            XMLSettingGroup group = settings.SettingGroups.FirstOrDefault(g => g.Name == settingGroup);
            if (group == null)
            {
                LogWarning(string.Format("Setting group [{0}] not found", settingGroup));
            }
            else
            {
                XMLSetting setting = group.Settings.FirstOrDefault(s => s.Name == settingName);
                if (setting == null)
                {
                    LogWarning(string.Format("Setting [{0}] in group [{1}] not found", settingName, settingGroup));
                }
                else
                {
                    if (setting.Value != null)
                    {
                        result = SettingHelper.ReplacePlaceHolders(this, setting.Value);
                        result = result.Trim();
                    }
                }
            }

            return result;
        }

        #endregion


        #region Global

        public string GetPath(ESystemPath requiredPath)
        {
            string result = string.Empty;
            switch (requiredPath)
            {
                case ESystemPath.ApplicationData:
                    result = settings.Globals.Folders.ApplicationData;
                    break;
                case ESystemPath.Logs:
                    result = settings.Globals.Folders.Logs;
                    break;
                case ESystemPath.Locales:
                    result = settings.Globals.Folders.Locales;
                    break;
                //case ESystemPath.Managers:
                //    result = this.settings.Globals.Folders.Managers;
                //    break;
                case ESystemPath.Plugins:
                    result = settings.Globals.Folders.Plugins;
                    break;
                case ESystemPath.Grammars:
                    result = settings.Globals.Folders.Grammars;
                    break;
            }
            result = SettingHelper.ReplacePlaceHolders(this, result);
            return result;
        }
        #endregion

        #endregion

        #region Logs
        private void LogInvalidSetting(string settingName, ESettingDataType settingDataType, string settingValue)
        {
            LogError(string.Format("Setting {0} of type {1} has an invalid value of {2}. Setting not added", settingName, settingDataType, settingValue));
        }

        private void LogInfo(string message, [CallerMemberName] string caller = null)
        {
            if (loggerManager != null)
            {
                loggerManager.LogInformation(EManagersType.SettingManager.ToString(), message, null, caller);
            }
            else
            {
                Console.WriteLine("[INFO]: " + message);
            }
        }

        private void LogError(string error)
        {
            if (loggerManager != null)
            {
                loggerManager.LogError(EManagersType.SettingManager.ToString(), error);
            }
            else
            {
                Console.WriteLine("[ERROR]: " + error);
            }
        }

        private void LogError(Exception e)
        {
            if (loggerManager != null)
            {
                loggerManager.LogError(EManagersType.SettingManager.ToString(), e);
            }
            else
            {
                Console.WriteLine("[ERROR]: " + e.Message);
            }
        }

        private void LogWarning(string warning)
        {
            if (loggerManager != null)
            {
                loggerManager.LogWarning(EManagersType.SettingManager.ToString(), warning);
            }
            else
            {
                Console.WriteLine("[WARN]: " + warning);
            }
        }

        #endregion

    }
}
