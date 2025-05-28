using McpNetwork.Charli.Server.Models.Enums;

namespace McpNetwork.Charli.Service.Environment.Interfaces.Managers.Master
{
    public abstract class ACharliManager : ICharliManager
    {
        /// <summary>
        /// Name of Manager
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Global information
        /// </summary>
        public abstract string Information { get; }

        /// <summary>
        /// Execution status of the Manager
        /// </summary>
        public abstract EManagerStatus Status { get; }

        /// <summary>
        /// Manager type
        /// </summary>
        public abstract EManagersType Code { get; }

        /// <summary>
        /// Version
        /// </summary>
        public abstract Version Version { get; }

        /// <summary>
        /// End-Point for the mobile application
        /// </summary>
        public abstract string MobileEndPoint { get; }

        /// <summary>
        /// Icon used by the manager (Base64 encoded)
        /// </summary>
        public abstract string MobileIcon { get; }

        /// <summary>
        /// Initialize the manager
        /// </summary>
        /// <returns></returns>
        public abstract bool Initialize(IServiceProvider serviceProvider);

        /// <summary>
        /// Start the manager
        /// </summary>
        public virtual void Start() { }

        /// <summary>
        /// Stop the manager
        /// </summary>
        public virtual void Stop() { }
    }
}
