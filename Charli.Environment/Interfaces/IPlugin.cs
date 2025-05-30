using McpNetwork.Charli.Environment.Enums;

namespace McpNetwork.Charli.Environment.Interfaces;

public interface IPlugin
{
    string Name { get; }

    string Description { get; }

    Version Version { get; }

    EPluginStatus Status();
}
