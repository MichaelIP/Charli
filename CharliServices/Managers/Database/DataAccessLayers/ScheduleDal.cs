using McpNetwork.Charli.Managers.DatabaseManager.DataModel;
using McpNetwork.Charli.Server.Exceptions;
using McpNetwork.Charli.Server.Models.Entities;
using McpNetwork.Charli.Server.Models.Interfaces.DataAccessLayers;

namespace McpNetwork.Charli.Server.Managers.Database.DataAccessLayers
{
    public class ScheduleDal : IScheduleDal
    {

        private readonly string dbPath;

        internal ScheduleDal(string dbPath)
        {
            this.dbPath = dbPath;
        }

        public IEnumerable<Schedule> GetSchedules()
        {
            var result = new List<Schedule>();
            try
            {
                using (var dbContext = new CharliEntities(dbPath))
                {
                    result = dbContext.Schedules.ToList();
                }
            }
            catch (Exception e)
            {
                throw new DalException("DAL Error", e);
            }
            return result;
        }
    }
}
