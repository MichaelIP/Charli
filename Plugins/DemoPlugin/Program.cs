using McpNetwork.Charli.Plugins.DemoPlugin;
using McpNetwork.WinNuxService;
using System.Diagnostics;
using System.Runtime.Versioning;

namespace DemoPlugin
{
    internal class Program
    {
        [SupportedOSPlatform("linux")]
        [SupportedOSPlatform("windows")]
        static void Main(string[] args)
        {
            var serviceManager = new WinNuxService<DemoPluginService>();
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
