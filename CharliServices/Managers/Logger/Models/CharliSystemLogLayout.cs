using log4net.Layout;
using log4net.Util;

namespace McpNetwork.Charli.Server.Managers.Logger.Models
{
    public class CharliSystemLogLayout : PatternLayout
    {
        public CharliSystemLogLayout()
        {
            AddConverter(new ConverterInfo { Name = "SystemInfo", Type = typeof(CharliLogConverter) });
        }
    }

}
