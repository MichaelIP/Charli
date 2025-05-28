using log4net;
using log4net.Config;
using McpNetwork.Charli.Server.Helpers;
using McpNetwork.Charli.Server.Managers.Logger.Models;
using McpNetwork.Charli.Server.Models.Enums;
using McpNetwork.Charli.Server.Models.Interfaces.Managers;
using McpNetwork.Charli.Service.Environment.Interfaces.Managers.Master;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace McpNetwork.Charli.Server.Managers.Logger
{
    public class LoggerManager : ACharliManager, ILoggerManager
    {

        private ILog _systemLoggerToBeChanged = null;
        private ILog SystemLogger
        {
            get
            {
                if (_systemLoggerToBeChanged == null)
                {
                    _systemLoggerToBeChanged = log4net.LogManager.GetLogger(typeof(LoggerManager));
                    var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
                    XmlConfigurator.Configure(logRepository, new FileInfo(LogConfigurationFile));
                }
                return _systemLoggerToBeChanged;
            }
        }

        private bool isInitialized = false;
        private EManagerStatus status = EManagerStatus.Stopped;

        #region Abstract properties

        public override string Name => "Logger Manager";

        public override string Information => "TO BE DONE !";

        public override EManagerStatus Status => status;

        public override EManagersType Code => EManagersType.LoggerManager;

        public override Version Version => typeof(LoggerManager).Assembly.GetName().Version;

        public override string MobileEndPoint => "Logger/endPoints";

        public override string MobileIcon => ImageHelpers.ImageToBase64(ImageHelpers.GetResource("McpNetwork.Charli.Managers.LoggerManager.Resources.icon.png"), ImageFormat.Png);

        #endregion

        public override bool Initialize(IServiceProvider serviceProvider)
        {
            if (!isInitialized)
            {
                status = EManagerStatus.Running;
                //var wsManager = serviceProvider.GetService<IWebServicesManager>();
                //wsManager.AddServices(Assembly.GetExecutingAssembly());
            }
            return true;
        }

        public string LogConfigurationFile { get; set; }

        #region DEBUG logs
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public void LogDebug(string plugin, string message, [CallerMemberName] string caller = null)
        {
            LogDebug(plugin, null, message, null, caller);
        }
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public void LogDebug(string plugin, string message, object parameters, [CallerMemberName] string caller = null)
        {
            LogDebug(plugin, null, message, parameters, caller);
        }
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public void LogDebug(string plugin, int? userid, string message, [CallerMemberName] string caller = null)
        {
            LogDebug(plugin, null, message, null, caller);
        }
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public void LogDebug(string plugin, int? userId, string message, object parameters, [CallerMemberName] string caller = null)
        {
            var stackTrace = new StackTrace(0);
            var log = new CharliSystemLogModel(userId, plugin, message, null, parameters, stackTrace, caller);
            SystemLogger.Debug(log);
        }
        #endregion

        #region INFO logs
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public void LogInformation(string plugin, string message, [CallerMemberName] string caller = null)
        {
            LogInformation(plugin, null, message, null, caller);
        }
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public void LogInformation(string plugin, string message, object parameters, [CallerMemberName] string caller = null)
        {
            LogInformation(plugin, null, message, parameters, caller);
        }
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public void LogInformation(string plugin, int? userid, string message, [CallerMemberName] string caller = null)
        {
            LogInformation(plugin, null, message, null, caller);
        }
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public void LogInformation(string plugin, int? userId, string message, object parameters, [CallerMemberName] string caller = null)
        {
            var stackTrace = new StackTrace(0);
            var log = new CharliSystemLogModel(userId, plugin, message, null, parameters, stackTrace, caller);
            SystemLogger.Info(log);
        }
        #endregion

        #region WARNING logs
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public void LogWarning(string plugin, string message, [CallerMemberName] string caller = null)
        {
            LogWarning(plugin, null, message, null, caller);
        }
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public void LogWarning(string plugin, string message, object parameters, [CallerMemberName] string caller = null)
        {
            LogWarning(plugin, null, message, parameters, caller);
        }
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public void LogWarning(string plugin, int? userid, string message, [CallerMemberName] string caller = null)
        {
            LogWarning(plugin, null, message, null, caller);
        }
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public void LogWarning(string plugin, int? userId, string message, object parameters, [CallerMemberName] string caller = null)
        {
            var stackTrace = new StackTrace(0);
            var log = new CharliSystemLogModel(userId, plugin, message, null, parameters, stackTrace, caller);
            SystemLogger.Warn(log);
        }
        #endregion

        #region ERROR logs
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public void LogError(string plugin, string message, [CallerMemberName] string caller = null)
        {
            LogError(plugin, null, message, null, caller);
        }
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public void LogError(string plugin, string message, object parameters, [CallerMemberName] string caller = null)
        {
            LogError(plugin, null, message, parameters, caller);
        }
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public void LogError(string plugin, int? userid, string message, [CallerMemberName] string caller = null)
        {
            LogError(plugin, null, message, null, caller);
        }
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public void LogError(string plugin, int? userId, string message, object parameters, [CallerMemberName] string caller = null)
        {
            var stackTrace = new StackTrace(0);
            var log = new CharliSystemLogModel(userId, plugin, message, null, parameters, stackTrace, caller);
            SystemLogger.Error(log);
        }


        public void LogError(string plugin, string message, Exception exception, [CallerMemberName] string caller = null)
        {
            LogError(plugin, message, exception, null, caller);
        }
        public void LogError(string plugin, string message, Exception exception, object parameters, [CallerMemberName] string caller = null)
        {
            LogError(plugin, null, message, exception, parameters, caller);
        }
        public void LogError(string plugin, int? userId, string message, Exception exception, [CallerMemberName] string caller = null)
        {
            LogError(plugin, userId, message, exception, null, caller);
        }
        public void LogError(string plugin, int? userId, string message, Exception exception, object parameters, [CallerMemberName] string caller = null)
        {
            var stackTrace = new StackTrace(0);
            var log = new CharliSystemLogModel(userId, plugin, message, GetJSonException(exception), parameters, stackTrace, caller);
            SystemLogger.Error(log);
        }

        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public void LogError(string plugin, Exception exception, [CallerMemberName] string caller = null)
        {
            LogError(plugin, (int?)null, exception, null, caller);
        }
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public void LogError(string plugin, Exception exception, object parameters, [CallerMemberName] string caller = null)
        {
            LogError(plugin, (int?)null, exception, parameters, caller);
        }
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public void LogError(string plugin, int? userid, Exception exception, [CallerMemberName] string caller = null)
        {
            LogError(plugin, userid, exception, null, caller);
        }
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public void LogError(string plugin, int? userId, Exception exception, object parameters, [CallerMemberName] string caller = null)
        {
            var stackTrace = new StackTrace(0);
            var log = new CharliSystemLogModel(userId, plugin, GetExceptionMessage(exception), GetJSonException(exception), parameters, stackTrace, caller);
            SystemLogger.Error(log);
        }
        #endregion

        #region FATAL logs
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public void LogCritical(string plugin, string message, [CallerMemberName] string caller = null)
        {
            LogCritical(plugin, null, message, null, caller);
        }
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public void LogCritical(string plugin, string message, object parameters, [CallerMemberName] string caller = null)
        {
            LogCritical(plugin, null, message, parameters, caller);
        }
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public void LogCritical(string plugin, int? userid, string message, [CallerMemberName] string caller = null)
        {
            LogCritical(plugin, null, message, null, caller);
        }
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public void LogCritical(string plugin, int? userId, string message, object parameters, [CallerMemberName] string caller = null)
        {
            var stackTrace = new StackTrace(0);
            var log = new CharliSystemLogModel(userId, plugin, message, null, parameters, stackTrace, caller);
            SystemLogger.Fatal(log);
        }

        public void LogCritical(string plugin, string message, Exception exception, [CallerMemberName] string caller = null)
        {
            LogCritical(plugin, message, exception, null, caller);
        }
        public void LogCritical(string plugin, string message, Exception exception, object parameters, [CallerMemberName] string caller = null)
        {
            LogCritical(plugin, null, message, exception, parameters, caller);
        }
        public void LogCritical(string plugin, int? userId, string message, Exception exception, [CallerMemberName] string caller = null)
        {
            LogCritical(plugin, userId, message, exception, null, caller);
        }
        public void LogCritical(string plugin, int? userId, string message, Exception exception, object parameters, [CallerMemberName] string caller = null)
        {
            var stackTrace = new StackTrace(0);
            var log = new CharliSystemLogModel(userId, plugin, message, GetJSonException(exception), parameters, stackTrace, caller);
            SystemLogger.Fatal(log);
        }

        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public void LogCritical(string plugin, Exception exception, [CallerMemberName] string caller = null)
        {
            LogCritical(plugin, (int?)null, exception, null, caller);
        }
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public void LogCritical(string plugin, Exception exception, object parameters, [CallerMemberName] string caller = null)
        {
            LogCritical(plugin, (int?)null, exception, parameters, caller);
        }
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public void LogCritical(string plugin, int? userid, Exception exception, [CallerMemberName] string caller = null)
        {
            LogCritical(plugin, (int?)null, exception, null, caller);
        }
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public void LogCritical(string plugin, int? userId, Exception exception, object parameters, [CallerMemberName] string caller = null)
        {
            var stackTrace = new StackTrace(0);
            var log = new CharliSystemLogModel(userId, plugin, GetExceptionMessage(exception), GetJSonException(exception), parameters, stackTrace, caller);
            SystemLogger.Fatal(log);
        }
        #endregion

        private string GetExceptionMessage(Exception exception)
        {
            var result = exception.Message;
            if (exception.InnerException != null)
            {
                result += GetInnerExceptionMessage(exception.InnerException);
            }
            result = result.Replace(System.Environment.NewLine, string.Empty);
            return result;
        }

        private string GetInnerExceptionMessage(Exception exception)
        {
            var result = string.Format(" [InnerException: {0}", exception.Message);
            if (exception.InnerException != null)
            {
                result += GetInnerExceptionMessage(exception.InnerException);
            }
            result += "]";
            return result;
        }

        private string GetJSonException(Exception exception)
        {
            var result = string.Empty;
            try
            {
                result = JsonConvert.SerializeObject(exception);
            }
            catch (Exception)
            {
                result = JsonConvert.SerializeObject(new { Exception = exception.ToString() });
            }
            return result;
        }

    }

}
