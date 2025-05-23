using McpNetwork.Charli.Server.Models.Entities;

namespace McpNetwork.Charli.Server.Models.Interfaces.DataAccessLayers
{
    public interface IScheduleDal
    {
        IEnumerable<Schedule> GetSchedules();
    }
}
