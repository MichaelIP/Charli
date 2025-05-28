using McpNetwork.Charli.Server.Models.Enums;

namespace McpNetwork.Charli.Service.Environment.Interfaces.Managers.Master
{
    public interface ICharliManager
    {
        /// <summary>
        /// Code for the manager
        /// </summary>
        EManagersType Code { get; }

        /// <summary>
        ///  Name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Execution status
        /// </summary>
        EManagerStatus Status { get; }

        /// <summary>
        /// Version of the manager
        /// </summary>
        Version Version { get; }

        /// <summary>
        /// string representing the overall status
        /// </summary>
        string Information { get; }

        /// <summary>
        /// url of the master endpoint for mobiles application
        /// </summary>
        string MobileEndPoint { get; }

        /// <summary>
        /// Icon to be used in mobile application
        /// </summary>
        string MobileIcon { get; }

        /// <summary>
        /// Initialize the manager
        /// </summary>
        /// <returns></returns>
        bool Initialize(IServiceProvider serviceProvider);

        /// <summary>
        /// Start the manager
        /// </summary>
        void Start();

        /// <summary>
        /// Stop the manager
        /// </summary>
        void Stop();
    }
}
