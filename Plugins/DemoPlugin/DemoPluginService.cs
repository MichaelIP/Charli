using McpNetwork.WinNuxService.Abstracts;

namespace McpNetwork.Charli.Plugins.DemoPlugin
{
    internal class DemoPluginService : AWinNuxService
    {

        private readonly DemoPlugin demoPlugin = new();

        public DemoPluginService()
        {
            DebugStopKey = ConsoleKey.Z;
        }

        public override void OnStart()
        {
            demoPlugin.Start();
        }

        public override void OnStop()
        {
            demoPlugin.Stop();
        }
    }

}
