using McpNetwork.Charli.Managers.PluginManager;
using McpNetwork.Charli.Server.Constants;
using McpNetwork.Charli.Server.Helpers;
using McpNetwork.Charli.Server.Models.Entities;
using McpNetwork.Charli.Server.Models.Enums;
using McpNetwork.Charli.Server.Models.Interfaces.Managers;
using McpNetwork.Charli.Service.Environment.Interfaces.Managers.Master;
using Microsoft.Extensions.DependencyInjection;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;

namespace McpNetwork.Charli.Server.Managers.Plugins
{
    public class PluginManager : ACharliManager, IPluginManager, IDisposable
    {

        private const string LOCALE_PATH = "PluginManager/Messages";

        private const string LOCALE_INITIALIZING = "Initializing";
        private const string LOCALE_INITIALIZED = "Initialized";
        private const string LOCALE_INVALID_SETTING = "InvalidSetting";
        private const string LOCALE_PLUGIN_NOT_ACTIVATED = "PluginNotActivated";
        private const string LOCALE_STARTING_PLUGIN = "StartingPlugin";
        private const string LOCALE_STOPPING_PLUGIN = "StoppingPlugin";
        private const string LOCALE_PLUGIN_NOT_STOPPED = "PluginNotStopped";
        private const string LOCALE_PAUSING_PLUGIN = "PausingPlugin";
        private const string LOCALE_RESUMING_PLUGIN = "ResumingPlugin";
        private const string LOCALE_PLUGIN_NOT_REGISTERED = "PluginNotRegistered";
        private const string LOCALE_ERROR_LOADING_DLL = "ErrorLoadingDll";
        private const string LOCALE_ERROR_LOADING_DLL_MESSAGE = "ErrorLoadingDllMessage";
        private const string LOCALE_PLUGIN_NOT_RESPONDING = "PluginNotResponding";
        
        private bool isInitialized;

        internal readonly List<PluginInfo> LoadedPlugins;

        private ILoggerManager? loggerManager;
        private IDatabaseManager? databaseManager;
        private ILocalizationManager? localizationManager;

        #region constructor
        public PluginManager()
        {
            LoadedPlugins = [];
        }
        #endregion

        #region abtracted properties

        public override string Name => "Plugin Manager";

        public override string Information => string.Format("Loaded Plugins: {0}", LoadedPlugins.Count);

        public override EManagerStatus Status => EManagerStatus.Running;

        public override EManagersType Code => EManagersType.PluginManager;

        public override Version Version => typeof(PluginManager).Assembly.GetName().Version;

        public override string MobileEndPoint => "Plugin/endPoints";

        public override string MobileIcon => ImageHelpers.ImageToBase64(ImageHelpers.GetResource("McpNetwork.Charli.Managers.PluginManager.Resources.icon.png"), ImageFormat.Png);


        public override bool Initialize(IServiceProvider serviceProvider)
        {
            if (!isInitialized)
            {
                databaseManager = serviceProvider.GetService<IDatabaseManager>() ?? throw new InvalidOperationException("IDatabaseManager service is not registered.");
                loggerManager = serviceProvider.GetService<ILoggerManager>() ?? throw new InvalidOperationException("ILoggerManager service is not registered.");
                localizationManager = serviceProvider.GetService<ILocalizationManager>() ?? throw new InvalidOperationException("ILocalizationManager service is not registered.");

                Initialize();

                isInitialized = true;
            }

            return isInitialized;
        }

        public void Pause()
        {
            foreach (var pluginInfo in LoadedPlugins)
            {
                //this.PausePlugin(pluginInfo.Guid);
            }
        }

        public void Resume()
        {
            foreach (var pluginInfo in LoadedPlugins)
            {
                //this.ResumePlugin(pluginInfo.Guid);
            }
        }

        public override void Start()
        {
            foreach (var pluginInfo in LoadedPlugins)
            {
                //this.StartPlugin(pluginInfo.Guid);
            }
        }

        public override void Stop()
        {
            //this.managerShutDown.Set();
            foreach (var pluginInfo in LoadedPlugins)
            {
                //this.StopPlugin(pluginInfo.Guid);
            }
        }

        #endregion

        #region INTERFACES
        public bool IsPluginAllowed(string pluginName, Version version)
        {
            if (databaseManager == null) return false;

            LogDebug(string.Format("Checking if plugin is allowed: {0}-{1}", pluginName, version));
            var repo = databaseManager.PluginDal;
            return repo.PluginIsActive(pluginName, version);
        }

        public void RegisterPlugin(string pluginName, Version version)
        {
            if (databaseManager == null) return;

            LogDebug(string.Format("Registering plugin : {0}-{1}", pluginName, version));
            var repo = databaseManager.PluginDal;
            repo.RegisterPlugin(new Plugin
            {
                Name = pluginName,
                Version = version.ToString(4),
                CreationDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                Active = false,
            });
        }

        public void SetPluginStarted(string uid, string pluginName, Version version)
        {

            LogDebug(string.Format("Declaring plugin started : {0}-{1}", pluginName, version));
            var loadedPlugin = LoadedPlugins
                                .Where(p => p.Name == pluginName)
                                .FirstOrDefault(p => p.Version.ToString(4) == version.ToString(4));
            
            if (loadedPlugin == null)
            {
                loadedPlugin = new PluginInfo(uid, pluginName, version);
                LoadedPlugins.Add(loadedPlugin);
            }
            loadedPlugin.StartTime = DateTime.Now;
            loadedPlugin.PluginStatus = EPluginStatus.Running;
        }

        public void SetPluginStopped(string uid)
        {
            var loadedPlugin = LoadedPlugins.Find(p => p.MqttClientId == uid);
            if (loadedPlugin != null)
            {
                LogDebug(string.Format("Declaring plugin stopped : {0}-{1}", loadedPlugin.Name, loadedPlugin.Version));
                loadedPlugin.StopTime = DateTime.Now;
                loadedPlugin.PluginStatus = EPluginStatus.Stopped;
            }
        }

        #endregion

        #region Initialization
        private void Initialize()
        {
            LogInfo(GetTranslation(LOCALE_INITIALIZING));

            LogInfo(GetTranslation(LOCALE_INITIALIZED));
        }

        #endregion

        #region Logs

        private void LogDebug(string message, [CallerMemberName] string caller = null)
        {
            loggerManager?.LogDebug(EManagersType.PluginManager.ToString(), message, caller);
        }

        private void LogWarning(string message, [CallerMemberName] string caller = null)
        {
            loggerManager?.LogWarning(EManagersType.PluginManager.ToString(), message, caller);
        }

        private void LogError(Exception e, [CallerMemberName] string caller = null)
        {
            loggerManager?.LogError(EManagersType.PluginManager.ToString(), e, caller);
        }
        private void LogError(string message, [CallerMemberName] string caller = null)
        {
            loggerManager?.LogError(EManagersType.PluginManager.ToString(), message, caller);
        }

        private void LogInfo(string message, [CallerMemberName] string caller = null)
        {
            loggerManager?.LogInformation(EManagersType.PluginManager.ToString(), message, caller);
        }

        #endregion

        #region Translation
        private string GetTranslation(string localeName)
        {
            return GetTranslation(localeName, LOCALE_PATH);
        }

        private string GetTranslation(string localeName, string path)
        {
            return localizationManager?.GetLocalization(LocalizationConstants.DefaultLocaleName, path, localeName) ?? localeName;
        }
        #endregion

        #region IDisposable pattern
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected void Dispose(bool isDisposing)
        {
            //this.pluginWatchDog.Dispose();

        }
        #endregion


    }
}
