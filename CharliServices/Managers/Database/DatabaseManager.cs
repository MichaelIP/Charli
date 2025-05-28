using McpNetwork.Charli.Managers.DatabaseManager.DataAccessLayers;
using McpNetwork.Charli.Server.Helpers;
using McpNetwork.Charli.Server.Managers.Database.DataAccessLayers;
using McpNetwork.Charli.Server.Models.Enums;
using McpNetwork.Charli.Server.Models.Interfaces.DataAccessLayers;
using McpNetwork.Charli.Server.Models.Interfaces.Managers;
using McpNetwork.Charli.Service.Environment.Interfaces.Managers.Master;
using System.Drawing.Imaging;
using System.Reflection;

namespace McpNetwork.Charli.Server.Managers.Database
{
    public class DatabaseManager : ACharliManager, IDatabaseManager
    {

        private const string CONNECTION_STRING = "CharliConnection";

        private ISettingManager settingManager;

        public override EManagerStatus Status => EManagerStatus.Running;
        public override string Information => "Database manager";
        public override string Name => "Database Manager";
        public override EManagersType Code => EManagersType.DatabaseManager;
        public override Version Version => new Version(2, 0, 0);
        public override string MobileEndPoint => "Database/EndPoints";

        public override string MobileIcon => ImageHelpers.ImageToBase64(ImageHelpers.GetResource("McpNetwork.Charli.Managers.DatabaseManager.Resources.icon.png"), ImageFormat.Png);

        #region Manager abstract implementation

        public override bool Initialize(IServiceProvider serviceProvider)
        {
            settingManager = (ISettingManager)serviceProvider.GetService(typeof(ISettingManager));
            var wsManager = (IWebServicesManager)serviceProvider.GetService(typeof(IWebServicesManager));
            //wsManager.AddServices(Assembly.GetExecutingAssembly());
            return true;
        }

        #endregion

        private IAccessControlDal accessControlDal;
        private IDeviceDal deviceDal;
        private IPluginDal pluginDal;
        private IScheduleDal scheduleDal;
        private IUserDal userDal;

        public IAccessControlDal AccessControlDal => GetDal<AccessControlDal, IAccessControlDal>(typeof(AccessControlDal), accessControlDal);
        public IDeviceDal DeviceDal => GetDal<DeviceDal, IDeviceDal>(typeof(DeviceDal), deviceDal);
        public IPluginDal PluginDal => GetDal<PluginDal, IPluginDal>(typeof(PluginDal), pluginDal);
        public IScheduleDal ScheduleDal => GetDal<ScheduleDal, IScheduleDal>(typeof(ScheduleDal), scheduleDal);
        public IUserDal UserDal => GetDal<UserDal, IUserDal>(typeof(UserDal), userDal);

        private T2 GetDal<T1, T2>(Type type, T2 value)
        {
            if (value == null)
            {
                value = (T2)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { settingManager.ConnectionString("CharliConnection") }, null);
            }
            return value;
        }

    }
}
