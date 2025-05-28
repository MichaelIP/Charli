using McpNetwork.Charli.Environment.Helpers;
using McpNetwork.Charli.Server.Constants;
using McpNetwork.Charli.Server.Helpers;
using McpNetwork.Charli.Server.Models.Enums;
using McpNetwork.Charli.Server.Models.Interfaces.Managers;
using McpNetwork.Charli.Server.Models.Localization;
using McpNetwork.Charli.Service.Environment.Interfaces.Managers.Master;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;

namespace McpNetwork.Charli.Server.Managers.Localization
{
    public class LocalizationManager : ACharliManager, ILocalizationManager
    {

        private bool isInitialized;
        private readonly Dictionary<string, XmlLocalizations> locales;

        private ISettingManager settingManager;
        private ILoggerManager loggerManager;

        #region constructor
        public LocalizationManager()
        {
            locales = new Dictionary<string, XmlLocalizations>();
        }
        #endregion

        #region Abstract properties

        public override string Name => "Localization Manager"; // TODO --> Use localization
        public override EManagersType Code => EManagersType.LocalizationManager;

        public override string Information => string.Format("Loaded locales: {0}", locales.Count);

        public override EManagerStatus Status => EManagerStatus.Running;

        public override Version Version => typeof(LocalizationManager).Assembly.GetName().Version;

        public override string MobileEndPoint => "Localization/endPoints";

        public override string MobileIcon => ImageHelpers.ImageToBase64(ImageHelpers.GetResource("McpNetwork.Charli.Managers.LocalizationManager.Resources.icon.png"), ImageFormat.Png);


        #endregion

        #region Abstract methods

        public override bool Initialize(IServiceProvider serviceProvider)
        {
            if (!isInitialized)
            {

                isInitialized = true;
                settingManager = (ISettingManager)serviceProvider.GetService(typeof(ISettingManager));
                loggerManager = (ILoggerManager)serviceProvider.GetService(typeof(ILoggerManager));
                var wsManager = (IWebServicesManager)serviceProvider.GetService(typeof(IWebServicesManager));

                //this.LogInfo(this.GetLocalization(LocalizationConstants.DefaultLocaleName, "LocalizationManager/Messages/Initialization", "Initializing"));

                var localesFolderName = settingManager.GetPath(ESystemPath.Locales);
                DirectoryInfo localesFolder = new DirectoryInfo(localesFolderName);
                if (localesFolder.Exists)
                {
                    foreach (FileInfo localeFile in localesFolder.GetFiles("*.xml"))
                    {
                        if (XmlValidationHelper.ValidateDocument(localeFile.FullName))
                        {
                            LogInfo(string.Format("Loading file {0}", localeFile.Name));
                            XmlLocalizations local = XmlValidationHelper.LoadDocument<XmlLocalizations>(localeFile.FullName);
                            if (locales.ContainsKey(local.Name))
                            {
                                loggerManager.LogWarning(EManagersType.LocalizationManager.ToString(), string.Format("Locale with name {0} already exists. Not superseded", local.Name));
                            }
                            else
                            {
                                locales.Add(local.Name, local);
                            }
                        }
                        else
                        {
                            LogError(string.Format("Localization file {0} is invalid. not loaded", localeFile.FullName));
                        }
                    }
                }
                else
                {
                    LogError(string.Format("Settings global/folders/locales is missing or invalid. Path: {0}", localesFolder.FullName));
                }

                //wsManager.AddServices(Assembly.GetExecutingAssembly());

                LogInfo(this.GetLocalization(LocalizationConstants.DefaultLocaleName, "LocalizationManager/Messages/Initialization", "Initialized"));
            }
            return isInitialized;

        }

        #endregion

        #region Interface implementation
        public string GetLocalization(string localeName, string path, string key)
        {
            return GetLocalization(localeName, path, key, false);
        }
        #endregion

        #region Get localization

        private string GetLocalization(string localeName, string path, string key, bool failOnError)
        {
            string fctResult = string.Empty;
            try
            {
                var locales = GetLocales(localeName);
                ILocaleGroupOwner groupOwner = locales;
                List<string> groups = path.Split(LocalizationConstants.PathSeparator).ToList();
                foreach (var group in groups)
                {
                    if (!string.IsNullOrEmpty(group.Replace(LocalizationConstants.PathSeparator.ToString(), string.Empty)))
                    {
                        groupOwner = GetGroup(groupOwner, group);

                    }
                }
                var localization = GetLocale(groupOwner, key);
                var translation = this.GetTranslation(localization, settingManager.DefaultCulture, locales.DefaultCulture);
                fctResult = translation.Value;
            }
            catch
            {
                try
                {
                    if (!failOnError)
                    {
                        fctResult = this.GetLocalization(LocalizationConstants.DefaultLocaleName, "LocalizationManager/Messages", "LocaleNotfound", true);
                    }
                    else
                    {
                        string message = string.Format("Default locales not found ! Please check you locale file is not corrupt or missing.");
                        LogInfo(message);
                        fctResult = message;
                    }
                }
                finally
                {
                    string message = string.Format("Locale not found: {0} - {1} - {2} ", localeName, path, key);
                    LogError(message);
                }
            }

            return fctResult;

        }

        private XmlLocalizations GetLocales(string locale)
        {
            return locales[locale];
        }

        private XmlGroups GetGroup(ILocaleGroupOwner owner, string group)
        {
            return owner.Groups.FirstOrDefault(o => o.Name == group);
        }

        private XmlLocalization GetLocale(ILocaleGroupOwner owner, string key)
        {
            return owner.Localizations.FirstOrDefault(l => l.LocalizationKey == key);
        }

        private XmlTranslation GetTranslation(XmlLocalization localization, string culture, string defaultCulture)
        {
            XmlTranslation fctResult = localization.Translations.FirstOrDefault(t => t.Culture == culture);
            if (fctResult == null)
            {
                fctResult = localization.Translations.FirstOrDefault(t => t.Culture == defaultCulture);
            }
            return fctResult;
        }

        #endregion

        #region Logs
        private void LogError(string message, [CallerMemberName] string caller = null)
        {
            loggerManager.LogError(EManagersType.LocalizationManager.ToString(), message, caller);
        }
        private void LogError(Exception e, [CallerMemberName] string caller = null)
        {
            loggerManager.LogError(EManagersType.LocalizationManager.ToString(), e, caller);
        }

        private void LogInfo(string message, [CallerMemberName] string caller = null)
        {
            loggerManager.LogInformation(EManagersType.LocalizationManager.ToString(), message, null, caller);
        }
        private void LogInfo(string message, object parameters, [CallerMemberName] string caller = null)
        {
            loggerManager.LogInformation(EManagersType.LocalizationManager.ToString(), message, parameters, caller);
        }
        #endregion

    }
}
