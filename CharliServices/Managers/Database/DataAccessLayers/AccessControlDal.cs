using McpNetwork.Charli.Managers.DatabaseManager.DataModel;
using McpNetwork.Charli.Server.Exceptions;
using McpNetwork.Charli.Server.Models.Entities;
using McpNetwork.Charli.Server.Models.Interfaces.DataAccessLayers;

namespace McpNetwork.Charli.Server.Managers.Database.DataAccessLayers
{
    public class AccessControlDal : IAccessControlDal
    {
        private readonly string dbPath;

        internal AccessControlDal(string dbPath)
        {
            this.dbPath = dbPath;
        }

        public IEnumerable<AccessControl> LoadMatrix()
        {
            var result = new List<AccessControl>();
            try
            {
                using (var dbContext = new CharliEntities(dbPath))
                {
                    result = dbContext.AccessControls.ToList();
                }
            }
            catch (Exception e)
            {
                throw new DalException("DAL Error", e);
            }
            return result;
        }

        public (byte SetNb, byte BitNb) GetRoleSetAndBit(string role)
        {
            using (var dbContext = new CharliEntities(dbPath))
            {
                var accessControl = dbContext.AccessControls.FirstOrDefault(ac => ac.Name == role);
                return (accessControl.SetNb, accessControl.BitNb);
            }
        }
    }
}
