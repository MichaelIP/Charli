using McpNetwork.WinNuxService.Abstracts;

namespace McpNetwork.Charli.Server.Service
{
    internal class CharliService : AWinNuxService
    {

        private readonly CharliServer charliServer = new();

        public CharliService()
        {
            this.DebugStopKey = ConsoleKey.Z;
        }

        public override void OnStart()
        {
            this.charliServer.Start(this.StartArguments);
        }

        public override void OnStop()
        {
            this.charliServer.Stop();
        }
    }

}
