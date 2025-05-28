using McpNetwork.Charli.Server.Constants;
using McpNetwork.Charli.Server.Helpers;
using McpNetwork.Charli.Server.Managers.WebService.WebServer;
using McpNetwork.Charli.Server.Models.Enums;
using McpNetwork.Charli.Server.Models.Interfaces.Managers;
using McpNetwork.Charli.Service.Environment.Interfaces.Managers.Master;
using McpNetwork.MicroServiceCore.Models;
using McpNetwork.MicroServiceCore.WebHost;
using Microsoft.Extensions.DependencyInjection;
using System.Drawing.Imaging;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace McpNetwork.Charli.Server.Managers.WebService
{
    public class WebServicesManager : AWebHost, IWebServicesManager
    {

        private const string LOCALE_PATH = "WebServicesManager/Messages";

        private const string LOCALE_INITIALIZING = "Initializing";
        private const string LOCALE_INITIALIZED = "Initialized";
        private const string LOCALE_LISTENING = "Listening";
        private const string LOCALE_STOPPED = "Stopped";

        private bool isInitialized;

        private int loadedAssembliesCount;

        private ILoggerManager loggerManager;
        private ISettingManager settingManager;
        private ILocalizationManager localizationManager;
        private ISecurityManager securityManager;

        #region ICharliManager interface properties
        public string Name => "Web Services Manager";

        public string Information => string.Format(CultureInfo.InvariantCulture, "Loaded Assemblies: {0}", loadedAssembliesCount);

        public EManagerStatus Status => EManagerStatus.Running;

        public EManagersType Code => EManagersType.WebServicesManager;

        public Version Version => typeof(WebServicesManager).Assembly.GetName().Version;

        public string MobileEndPoint => "WebServices/endPoints";

        public string MobileIcon => ImageHelpers.ImageToBase64(ImageHelpers.GetResource("McpNetwork.Charli.Managers.WebServiceManager.Resources.icon.png"), ImageFormat.Png);

        #endregion

        #region AWebHost abstract properties
        public override string ServiceName => "Charli WebService manager";
        public override string ServiceDescription => "Charli WebService manager";
        public override string GetConnectionString(string connectionStringName)
        {
            return settingManager.ConnectionString(connectionStringName);
        }
        public override T GetSetting<T>(string key)
        {
            return settingManager.GetSetting<T>("WebServicesManager", key);
        }
        public override T GetSetting<T>(string key, T defaultValue)
        {
            return settingManager.GetSetting<T>("WebServicesManager", key, defaultValue);
        }
        #endregion

        public WebServicesManager()
        {
            this.ValidateToken = CheckTokenValidity;
        }

        public bool Initialize(IServiceProvider serviceProvider)
        {
            if (!isInitialized)
            {
                isInitialized = true;
                loggerManager = serviceProvider.GetService<ILoggerManager>();
                settingManager = serviceProvider.GetService<ISettingManager>();
                localizationManager = serviceProvider.GetService<ILocalizationManager>();
                securityManager = serviceProvider.GetService<ISecurityManager>();

                LogInfo(GetTranslation(LOCALE_INITIALIZING));

                this.ApplicationModelConvention = new CharliModelConvention();

                AddServices(Assembly.GetExecutingAssembly());


                //var dependencies = new List<DependencyModel>();
                //foreach (var service in Enum.GetValues(typeof(EManagersType)))
                //{
                //    var managerType = ((EManagersType)service).GetManagerImplementedType();
                //    var manager = serviceProvider.GetService(managerType) as ICharliManager;
                //    dependencies.Add(new DependencyModel(ServiceLifetime.Singleton, managerType, manager));
                //}

                //this.AddDependencies(dependencies);

                LogInfo(GetTranslation(LOCALE_INITIALIZED));
            }
            return isInitialized;

        }


        #region Interface implementation
        public void AddServices(Assembly assembly)
        {
            loadedAssembliesCount++;
            this.AddAssemblyControllers(assembly);
        }

        public void Resume()
        {
            this.Start();
        }

        public void Pause()
        {
            this.Stop();
        }

        public void ShutDown() { }

        #endregion

        #region Logs

        protected override void LoggerDebugFired(object sender, ServiceLogModel arg)
        {
            loggerManager.LogDebug(arg.Service, arg.Message);
        }
        protected override void LoggerErrorFired(object sender, ServiceLogModel arg)
        {
            if (arg.Exception == null)
            {
                loggerManager.LogError(arg.Service, arg.Message);
            }
            else
            {
                loggerManager.LogError(arg.Service, arg.Exception);
            }
        }
        protected override void LoggerFatalFired(object sender, ServiceLogModel arg)
        {
            loggerManager.LogError(arg.Service, arg.Exception);
        }
        protected override void LoggerInfoFired(object sender, ServiceLogModel arg)
        {
            loggerManager.LogInformation(arg.Service, arg.Message);
        }
        protected override void LoggerWarningFired(object sender, ServiceLogModel arg)
        {
            loggerManager.LogWarning(arg.Service, arg.Message);
        }


        private void AddErrorLog(string message, [CallerMemberName] string caller = null)
        {
            loggerManager.LogError(EManagersType.WebServicesManager.ToString(), message, caller);
        }

        private void AddErrorLog(Exception e, [CallerMemberName] string caller = null)
        {
            loggerManager.LogError(EManagersType.WebServicesManager.ToString(), e, caller);
        }

        private void LogInfo(string message, [CallerMemberName] string caller = null)
        {
            loggerManager.LogInformation(EManagersType.WebServicesManager.ToString(), message, null, caller);
        }
        private void LogInfo(string message, object parameters, [CallerMemberName] string caller = null)
        {
            loggerManager.LogInformation(EManagersType.WebServicesManager.ToString(), message, parameters, caller);
        }

        private void LogWarning(string message, [CallerMemberName] string caller = null)
        {
            loggerManager.LogWarning(EManagersType.WebServicesManager.ToString(), message, null, caller);
        }

        #endregion

        #region Localization
        private string GetTranslation(string localeName)
        {
            return GetTranslation(localeName, LOCALE_PATH);
        }

        private string GetTranslation(string localeName, string path)
        {
            return localizationManager.GetLocalization(LocalizationConstants.DefaultLocaleName, path, localeName);
        }
        #endregion


        #region Security

        private McpNetwork.MicroServiceCore.Models.Enums.ECheckTokenResult CheckTokenValidity(string token, string authorizationRole)
        {
            var result = ECheckTokenResult.Disabled;
            if (token != null)
            {
                var trueToken = GetAuthorizationToken(token);
                result = securityManager.IsAuthorized(string.IsNullOrEmpty(authorizationRole) ? "Authenticated" : authorizationRole, trueToken);
            }

            return Enum.Parse< McpNetwork.MicroServiceCore.Models.Enums.ECheckTokenResult>(result.ToString());

        }


        private string GetAuthorizationToken(string token)
        {
            string result = null;

            var regex = new Regex(@"^Bearer (.+)$");
            var match = regex.Match(token);
            if (match.Success)
            {
                result = match.Groups[1].Value;
            }
            return result;
        }

        #endregion

    }
}
