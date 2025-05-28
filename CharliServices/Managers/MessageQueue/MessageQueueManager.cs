using McpNetwork.Charli.Server.Managers.MessageQueue.Server;
using McpNetwork.Charli.Server.Models.Enums;
using McpNetwork.Charli.Server.Models.Interfaces;
using McpNetwork.Charli.Server.Models.Interfaces.Managers;
using McpNetwork.Charli.Service.Environment.Interfaces.Managers.Master;
using Microsoft.Extensions.DependencyInjection;

namespace McpNetwork.Charli.Server.Managers.MessageQueue
{
    internal class MessageQueueManager : ACharliManager, IMessageQueueManager
    {

        #region interface implementation

        public override string Name => "Mqtt Manager";
        public override string Information => "Provides MQTT server facilities";

        public override EManagerStatus Status => status;

        public override EManagersType Code => EManagersType.MessageQueueManager;

        public override Version Version => typeof(MessageQueueManager).Assembly.GetName().Version ?? new Version("0.0.0");

        public override string MobileEndPoint => "Not implemented";

        public override string MobileIcon => "Not implemented";

        #endregion

        private bool initialized;
        private EManagerStatus status = EManagerStatus.Stopped;

        private ILoggerManager? loggerManager;
        public IPluginManager? PluginManager { get; private set; }
        public ISettingManager? SettingManager { get;private set; }

        private string rootPath = string.Empty;
        private bool disposed = false;

        private CharliMqttClient? client;
        private CharliMqttServer? server;

        private readonly string sessionUid = Guid.NewGuid().ToString("N");

        public override bool Initialize(IServiceProvider serviceProvider)
        {
            if (!initialized)
            {
                initialized = true;

                PluginManager = serviceProvider.GetService<IPluginManager>();
                SettingManager = serviceProvider.GetService<ISettingManager>();
                loggerManager = serviceProvider.GetService<ILoggerManager>();

                loggerManager?.LogInformation(EManagersType.MessageQueueManager.ToString(), $"Initializing {Name} with UID {sessionUid}");
                rootPath = SettingManager?.GetPath(ESystemPath.ApplicationData) ?? string.Empty;

                client = new CharliMqttClient(this, loggerManager, rootPath, sessionUid);
                server = new CharliMqttServer(this, loggerManager, rootPath, sessionUid);

                status = EManagerStatus.Paused;
            }

            return true;
        }

        public override void Start()
        {
            server?.Start().Wait();
            client?.Start().Wait();
            status = EManagerStatus.Running;

        }

        public override void Stop()
        {
            client?.Stop();
            server?.Stop();
            status = EManagerStatus.Stopped;
        }

        
        #region IDisposable pattern
        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }
        protected void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                // TODO: dispose managed state (managed objects).
                client?.Dispose();
                server?.Dispose();
            }


            disposed = true;
        }
        #endregion
    }
}
