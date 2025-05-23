using McpNetwork.Charli.Server.Models.Entities;

namespace McpNetwork.Charli.Server.Models.Interfaces.DataAccessLayers
{
    public interface IAccessControlDal
    {
        /// <summary>
        /// Load the security matrix
        /// </summary>
        /// <returns></returns>
        IEnumerable<AccessControl> LoadMatrix();

        /// <summary>
        /// return right set & bit for a role
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        (byte SetNb, byte BitNb) GetRoleSetAndBit(string role);
    }
}
