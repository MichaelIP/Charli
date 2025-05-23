using McpNetwork.Charli.Server.Models.Entities;

namespace McpNetwork.Charli.Server.Models.Interfaces.DataAccessLayers
{
    public interface IDeviceDal
    {
        IEnumerable<Device> FindDeviceForUser(int userId);
        bool DeleteDeviceByNotificationToken(string notificationToken);
        bool UpdateNotificationToken(string oldNotificationToken, string newNotificationToken);
        bool RegisterDevice(Device device);
        IEnumerable<Device> GetUserDevices(int userId);
    }
}
