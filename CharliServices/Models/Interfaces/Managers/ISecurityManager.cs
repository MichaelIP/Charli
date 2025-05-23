using McpNetwork.Charli.Server.Models.Security;
using McpNetwork.Charli.Server.Models.WebServices;
using System.Collections.ObjectModel;

namespace McpNetwork.Charli.Server.Models.Interfaces.Managers
{
    internal interface ISecurityManager : ICharliManager
    {
        ReadOnlyCollection<SecurityTokenModel> SecurityTokens { get; }

        ReadOnlyCollection<AccessControlModel> AccessControl { get; }

        void Logout(string securityToken);

        LoginResponseModel Login(string user, string password);

        RenewTokenResponseModel RenewToken(string token);

        SecurityTokenModel GetToken(string token);

        bool IsAuthorized(string right, string token);
    }
}
