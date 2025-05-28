namespace McpNetwork.Charli.Server.Models.Enums
{
    public enum EPluginStatus
    {
        NotStarted = 0x00,
        Running = 0x11,
        Paused = 0x21,
        Stopped = 0x31,
        Faulted = 0x100
    }
}
