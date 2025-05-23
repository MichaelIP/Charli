using System.Reflection;

namespace McpNetwork.Charli.Server.Models.Interfaces.Managers
{
    internal interface IWebServicesManager : ICharliManager
    {
        /// <summary>
        /// Add a Web service
        /// </summary>
        /// <param name="assembly"></param>
        void AddServices(Assembly assembly);
    }
}
