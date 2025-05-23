using McpNetwork.Charli.Server.Service;
using McpNetwork.WinNuxService;
using System.Diagnostics;
using System.Runtime.Versioning;

namespace McpNetwork.Charli.Server
{
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("windows")]
    class Program
    {
        static void Main(string[] args)
        {
            var isService = !(Debugger.IsAttached || args.Contains("--console"));

            var serviceManager = new WinNuxService<CharliService>
            {
                StartArguments = args.Where(a => a != "--console").ToArray()
            };

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
