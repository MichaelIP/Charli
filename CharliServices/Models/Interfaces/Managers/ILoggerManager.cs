using System.Runtime.CompilerServices;

namespace McpNetwork.Charli.Server.Models.Interfaces.Managers
{
    internal interface ILoggerManager : ICharliManager
    {
        string LogConfigurationFile { get; set; }

        void LogDebug(string sender, string message, [CallerMemberName] string caller = null);
        void LogDebug(string sender, string message, object parameters, [CallerMemberName] string caller = null);
        void LogDebug(string sender, int? userid, string message, [CallerMemberName] string caller = null);
        void LogDebug(string sender, int? userId, string message, object parameters, [CallerMemberName] string caller = null);

        void LogInformation(string sender, string message, [CallerMemberName] string caller = null);
        void LogInformation(string sender, string message, object parameters, [CallerMemberName] string caller = null);
        void LogInformation(string sender, int? userid, string message, [CallerMemberName] string caller = null);
        void LogInformation(string sender, int? userId, string message, object parameters, [CallerMemberName] string caller = null);

        void LogWarning(string sender, string message, [CallerMemberName] string caller = null);
        void LogWarning(string sender, string message, object parameters, [CallerMemberName] string caller = null);
        void LogWarning(string sender, int? userid, string message, [CallerMemberName] string caller = null);
        void LogWarning(string sender, int? userId, string message, object parameters, [CallerMemberName] string caller = null);

        void LogError(string sender, string message, [CallerMemberName] string caller = null);
        void LogError(string sender, string message, object parameters, [CallerMemberName] string caller = null);
        void LogError(string sender, int? userid, string message, [CallerMemberName] string caller = null);
        void LogError(string sender, int? userId, string message, object parameters, [CallerMemberName] string caller = null);
        void LogError(string sender, Exception exception, [CallerMemberName] string caller = null);
        void LogError(string sender, Exception exception, object parameters, [CallerMemberName] string caller = null);
        void LogError(string sender, int? userid, Exception exception, [CallerMemberName] string caller = null);
        void LogError(string sender, int? userId, Exception exception, object parameters, [CallerMemberName] string caller = null);

        void LogCritical(string sender, string message, [CallerMemberName] string caller = null);
        void LogCritical(string sender, string message, object parameters, [CallerMemberName] string caller = null);
        void LogCritical(string sender, int? userid, string message, [CallerMemberName] string caller = null);
        void LogCritical(string sender, int? userId, string message, object parameters, [CallerMemberName] string caller = null);
        void LogCritical(string sender, Exception exception, [CallerMemberName] string caller = null);
        void LogCritical(string sender, Exception exception, object parameters, [CallerMemberName] string caller = null);
        void LogCritical(string sender, int? userid, Exception exception, [CallerMemberName] string caller = null);
        void LogCritical(string sender, int? userId, Exception exception, object parameters, [CallerMemberName] string caller = null);

    }
}
