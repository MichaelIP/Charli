using McpNetwork.Charli.Server.Models.Entities;

namespace McpNetwork.Charli.Server.Models.Interfaces.DataAccessLayers
{
    public interface IUserDal
    {
        /// <summary>
        /// Login
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        User Login(string user, string password);

        /// <summary>
        /// Returns user information
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        User GetUserInfo(int userId);

        /// <summary>
        /// Save user information
        /// </summary>
        /// <param name="updates"></param>
        /// <returns></returns>
        bool SaveUserInfo(User updates);

        /// <summary>
        /// returns list of users belonging to a role
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        IEnumerable<User> GetUsersByRole(string role);
    }
}
