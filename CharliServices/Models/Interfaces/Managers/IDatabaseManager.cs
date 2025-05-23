using McpNetwork.Charli.Server.Models.Interfaces.DataAccessLayers;

namespace McpNetwork.Charli.Server.Models.Interfaces.Managers
{
    internal interface IDatabaseManager : ICharliManager
    {
        IAccessControlDal AccessControlDal { get; }
        IDeviceDal DeviceDal { get; }
        IPluginDal PluginDal { get; }
        IScheduleDal ScheduleDal { get; }
        IUserDal UserDal { get; }

    }
}
