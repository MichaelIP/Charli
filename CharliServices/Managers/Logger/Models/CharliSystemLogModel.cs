using Newtonsoft.Json;
using System.Diagnostics;

namespace McpNetwork.Charli.Server.Managers.Logger.Models
{
    internal class CharliSystemLogModel
    {
        private const string ANONYMOUS = "<anonymous>";

        private readonly string callingClass;
        private readonly string callingMethod;
        private readonly string callingParameters;
        private readonly string plugin;

        public int? UserId { get; set; }
        public string User { get; set; }

        public string Plugin { get { return plugin; } }

        public string Message { get; set; }
        public string Exception { get; set; }

        public string CallingClass { get { return callingClass; } }
        public string CallingMethod { get { return callingMethod; } }
        public string CallingParameters { get { return callingParameters; } }

        public CharliSystemLogModel(int? userId, string plugin, string message, string exception, object parameters, StackTrace trace, string caller)
        {
            UserId = userId;
            User = userId == null ? ANONYMOUS : userId.ToString();
            Message = message;
            Exception = exception;

            this.plugin = plugin;

            if (trace != null && trace.FrameCount != 0)
            {
                var stackFrame = trace.GetFrames().FirstOrDefault(st => st.GetMethod().Name == caller);
                if (stackFrame != null)
                {
                    var method = stackFrame.GetMethod();
                    callingClass = method.DeclaringType.Name;
                    callingMethod = method.Name;
                }
            }

            if (parameters != null)
            {
                try
                {
                    callingParameters = JsonConvert.SerializeObject(parameters);
                }
                catch (Exception)
                {
                    callingParameters = "Unable to serialize parameters";
                }
            }
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

}
