using McpNetwork.Charli.Managers.DatabaseManager.DataModel;
using McpNetwork.Charli.Server.Exceptions;
using McpNetwork.Charli.Server.Helpers;
using McpNetwork.Charli.Server.Models.Entities;
using McpNetwork.Charli.Server.Models.Interfaces.DataAccessLayers;

namespace McpNetwork.Charli.Managers.DatabaseManager.DataAccessLayers
{
    public partial class UserDal : IUserDal
    {
        private readonly string dbPath;

        internal UserDal(string dbPath)
        {
            this.dbPath = dbPath;
        }


        public User Login(string user, string password)
        {
            User result = null;
            try
            {
                using (var dbContext = new CharliEntities(this.dbPath))
                {

                    var userDb = dbContext.Users.FirstOrDefault(u => u.UserName == user);
                    if (userDb != null)
                    {
                        if (SecurityHelper.VerifyHashedPassword(userDb.Password, password))
                        {
                            userDb.LastConnectionDate = DateTime.Now;
                            dbContext.SaveChanges();
                            result = userDb;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new DalException("DAL Error", e);
            }
            return result;
        }

        public User GetUserInfo(int userId)
        {
            using (var dbContext = new CharliEntities(this.dbPath))
            {
                return dbContext.Users.FirstOrDefault(u => u.UserId == userId);
            }
        }

        public bool SaveUserInfo(User updates)
        {
            var result = true;
            try
            {
                using (var dbContext = new CharliEntities(this.dbPath))
                {

                    var user = dbContext.Users.FirstOrDefault(u => u.UserId == updates.UserId);
                    if (user != null)
                    {
                        user.Active = updates.Active;
                        user.Email = updates.Email;
                        user.FirstName = updates.FirstName;
                        user.LastName = updates.LastName;
                        if (!string.IsNullOrEmpty(updates.Password))
                        {
                            var hashedPassword = SecurityHelper.HashPassword(updates.Password, out string securityCode);
                            user.Password = hashedPassword;
                            user.SecurityCode = securityCode;
                        }
                        user.UpdateDate = DateTime.Now;
                    }
                    dbContext.SaveChanges();
                }
            }
            catch (Exception e)
            {
                result = false;
            }

            return result;
        }

        public IEnumerable<User> GetUsersByRole(string role)
        {
            using (var dbContext = new CharliEntities(this.dbPath))
            {
                var access = dbContext.AccessControls.FirstOrDefault(ac => ac.Name == role);
                if (access == null)
                {
                    return new List<User>();
                }

                var rights = new List<int> { 0, 0, 0, 0, 0 };
                rights[(int)access.SetNb] = (int)Math.Pow(2, access.BitNb);
                var users = dbContext.Users
                    .Where(u => (u.Right1 & rights[0]) == rights[0])
                    .Where(u => (u.Right2 & rights[1]) == rights[1])
                    .Where(u => (u.Right3 & rights[2]) == rights[2])
                    .Where(u => (u.Right4 & rights[3]) == rights[3])
                    .Where(u => (u.Right5 & rights[4]) == rights[4])
                    .ToList();
                return users;
            }
        }

    }
}
