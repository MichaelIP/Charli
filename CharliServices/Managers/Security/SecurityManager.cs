using McpNetwork.Charli.Server.Constants;
using McpNetwork.Charli.Server.Exceptions;
using McpNetwork.Charli.Server.Helpers;
using McpNetwork.Charli.Server.Models.Enums;
using McpNetwork.Charli.Server.Models.Interfaces.Managers;
using McpNetwork.Charli.Server.Models.Security;
using McpNetwork.Charli.Server.Models.WebServices;
using McpNetwork.Charli.Service.Environment.Interfaces.Managers.Master;
using System.Collections.ObjectModel;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;

namespace McpNetwork.Charli.Server.Managers.Security
{
    public class SecurityManager : ACharliManager, ISecurityManager, IDisposable
    {

        private const string LOCALE_PATH = "SecurityManager/Messages";
        private const string LOCALE_INITIALIZING = "Initializing";
        private const string LOCALE_INITIALIZED = "Initialized";

        private readonly Task tokenWatcher;
        private readonly ManualResetEvent managerShutDown = new ManualResetEvent(false);

        private int tokenLifetime;
        private int removeTokenDelay;
        private bool isInitialized = false;
        private readonly List<SecurityTokenModel> tokens;

        private IDatabaseManager databaseManager;
        private ILoggerManager loggerManager;
        private ILocalizationManager localizationManager;

        public ReadOnlyCollection<SecurityTokenModel> SecurityTokens => GetTokens();
        public ReadOnlyCollection<AccessControlModel> AccessControl { get; private set; }

        #region Constructor
        public SecurityManager()
        {
            tokens = new List<SecurityTokenModel>();

            //this.cancellationTokenSource = new CancellationTokenSource();
            //this.cancellationToken = this.cancellationTokenSource.Token;
            tokenWatcher = Task.Factory.StartNew(WatchToken);
        }
        #endregion

        #region Abstracted properties
        public override string Name => "Security Manager";

        //public override bool IsInitialized => this.isInitialized;

        public override string Information => string.Format("Connexions: {0}", tokens.Count);

        public override EManagerStatus Status => EManagerStatus.Running;

        public override EManagersType Code => EManagersType.SecurityManager;

        public override Version Version => typeof(SecurityManager).Assembly.GetName().Version;

        public override string MobileEndPoint => "Security/endPoints";

        public override string MobileIcon => ImageHelpers.ImageToBase64(ImageHelpers.GetResource("McpNetwork.Charli.Managers.SecurityManager.Resources.icon.png"), ImageFormat.Png);

        #endregion

        #region Abstracted implementation

        public override bool Initialize(IServiceProvider serviceProvider)
        {
            if (!isInitialized)
            {
                var settingManager = (ISettingManager)serviceProvider.GetService(typeof(ISettingManager));
                var wsManager = (IWebServicesManager)serviceProvider.GetService(typeof(IWebServicesManager));
                databaseManager = (IDatabaseManager)serviceProvider.GetService(typeof(IDatabaseManager));
                loggerManager = (ILoggerManager)serviceProvider.GetService(typeof(ILoggerManager));
                localizationManager = (ILocalizationManager)serviceProvider.GetService(typeof(ILocalizationManager));

                LogInfo(GetTranslation(LOCALE_INITIALIZING));

                tokenLifetime = settingManager.GetSetting<int>("SecurityManager", "TokenLifeTime");
                removeTokenDelay = settingManager.GetSetting<int>("SecurityManager", "KeepExpiredTokenDelay", 5);

                //wsManager.AddServices(Assembly.GetExecutingAssembly());

                LoadAccessControlMatrix();

                LogInfo(GetTranslation(LOCALE_INITIALIZED));
                isInitialized = true;

            }
            return isInitialized;
        }

        #endregion

        #region Interface implementation

        public LoginResponseModel Login(string user, string password)
        {
            var result = new LoginResponseModel();

            try
            {
                var repo = databaseManager.UserDal;
                var userdb = repo.Login(user, password);

                if (userdb != null)
                {
                    var token = new SecurityTokenModel
                    {
                        DateCreated = DateTime.Now,
                        DateExpired = DateTime.Now.AddMinutes(tokenLifetime),
                        Token = Guid.NewGuid().ToString(),
                        UserId = userdb.UserId,
                        UserName = userdb.UserName,
                        UserRights = new List<long> { userdb.Right1, userdb.Right2, userdb.Right3, userdb.Right4, userdb.Right5 }
                    };

                    result = new LoginResponseModel
                    {
                        LastConnectionDate = userdb.LastConnectionDate,
                        Token = token.Token,
                        TokenExpirationDate = token.DateExpired,
                        UserId = userdb.UserId,
                        UserName = userdb.UserName
                    };
                    tokens.Add(token);
                }
            }
            catch (Exception ex)
            {
                loggerManager.LogError("SecurityManager", ex);
                throw new CharliException(ex.Message);
            }
            return result;
        }

        public void Logout(string securityToken)
        {
            var token = tokens.FirstOrDefault(t => t.Token == securityToken);
            if (token != null)
            {
                tokens.Remove(token);
            }

        }

        public SecurityTokenModel GetToken(string token)
        {
            var result = SecurityTokens.FirstOrDefault(st => st.Token == token);
            return result;
        }

        public RenewTokenResponseModel RenewToken(string token)
        {
            var result = new RenewTokenResponseModel();
            try
            {
                if (token != null)
                {
                    var securityToken = GetToken(token);
                    if (securityToken != null)
                    {
                        securityToken.DateExpired = DateTime.Now.AddMinutes(tokenLifetime);
                        result.TokenExpirationDate = securityToken.DateExpired;
                    }
                }
            }
            catch (Exception ex)
            {
                loggerManager.LogError("WebServicesController", ex);
                throw new CharliException(ex.Message);
            }
            return result;
        }

        public ECheckTokenResult IsAuthorized(string rightName, string token)
        {
            var result = ECheckTokenResult.Disabled;
            var userInfo = GetToken(token);
            if (userInfo != null)
            {
                if (userInfo.DateExpired <= DateTime.Now)
                {
                    result = ECheckTokenResult.Disabled;
                }
                else
                {
                    var right = AccessControl.FirstOrDefault(r => r.Name == rightName);
                    if (right != null)
                    {
                        var userRight = userInfo.UserRights[right.SetNb];
                        var requiredRight = (int)Math.Pow(2, right.BitNb);
                        result = (requiredRight & userRight) == requiredRight ? ECheckTokenResult.Allowed : result;
                    }
                }
            }
            return result;
        }

        #endregion

        #region private methods 
        internal ReadOnlyCollection<SecurityTokenModel> GetTokens()
        {
            return new ReadOnlyCollection<SecurityTokenModel>(tokens);
        }

        private void LoadAccessControlMatrix()
        {
            var repo = databaseManager.AccessControlDal;
            var accessRights = repo.LoadMatrix();

            List<AccessControlModel> loadedList = new List<AccessControlModel>();
            foreach (var accessRight in accessRights)
            {
                loadedList.Add(new AccessControlModel { Name = accessRight.Name, SetNb = accessRight.SetNb, BitNb = accessRight.BitNb });
            }
            AccessControl = new ReadOnlyCollection<AccessControlModel>(loadedList);
        }
        #endregion

        #region Logs
        private void LogInfo(string message, [CallerMemberName] string caller = null)
        {
            loggerManager.LogInformation(EManagersType.SecurityManager.ToString(), message, null, caller);
        }
        #endregion

        #region Translations
        private string GetTranslation(string localeName)
        {
            return GetTranslation(localeName, LOCALE_PATH);
        }

        private string GetTranslation(string localeName, string path)
        {
            return localizationManager.GetLocalization(LocalizationConstants.DefaultLocaleName, path, localeName);
        }
        #endregion

        #region Token watch
        private void WatchToken()
        {
            while (!managerShutDown.WaitOne(50))
            {
                Thread.Sleep(1000);
                var deletedTokens = new List<string>();
                foreach (var token in tokens)
                {
                    if (token.DateExpired.AddMinutes(removeTokenDelay) <= DateTime.Now)
                    {
                        deletedTokens.Add(token.Token);
                    }
                }
                tokens.RemoveAll(a => deletedTokens.Contains(a.Token));
            }
        }

        #endregion

        #region Internal 
        internal void KillConnection(string token)
        {
            var killedToken = tokens.FirstOrDefault(t => t.Token == token);
            tokens.Remove(killedToken);
        }
        #endregion

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                //this.cancellationTokenSource.Cancel();
                tokenWatcher.Wait();
            }
        }



    }
}
