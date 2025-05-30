using McpNetwork.Charli.Plugins.VideoMonitoring;
using McpNetwork.WinNuxService;
using System.Diagnostics;
using System.Runtime.Versioning;

namespace VideoMonitoring
{
    internal class Program
    {
        [SupportedOSPlatform("linux")]
        [SupportedOSPlatform("windows")]
        static void Main(string[] args)
        {
            var serviceManager = new WinNuxService<VideoMonitoringService>();
            var isService = !(Debugger.IsAttached || args.Contains("--console"));
            if (isService)
            {
                serviceManager.RunAsService();
            }
            else
            {
                serviceManager.RunAsConsole();
            }
        }
    }
}
