using McpNetwork.WinNuxService.Abstracts;

namespace McpNetwork.Charli.Plugins.VideoMonitoring
{
    internal class VideoMonitoringService : AWinNuxService
    {

        private readonly VideoMonitoring videoMonitoring = new();
        public VideoMonitoringService()
        {
            DebugStopKey = ConsoleKey.Z;
        }

        public override void OnStart()
        {
            videoMonitoring.Start();
        }

        public override void OnStop()
        {
            videoMonitoring.Stop();
        }
    }

}
