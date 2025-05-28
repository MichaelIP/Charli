using McpNetwork.Charli.Server.Models.Enums;
using McpNetwork.Charli.Server.Models.Interfaces.Managers;
using System.Text.RegularExpressions;

namespace McpNetwork.Charli.Server.Helpers
{
    internal class SettingHelper
    {
        public static string ReplacePlaceHolders(ISettingManager settingManager, string source)
        {
            string fctResult = source;

            fctResult = fctResult.Replace("{dd}", DateTime.Today.ToString("dd"));
            fctResult = fctResult.Replace("{mm}", DateTime.Today.ToString("MM"));
            fctResult = fctResult.Replace("{yy}", DateTime.Today.ToString("yy"));
            fctResult = fctResult.Replace("{yyyy}", DateTime.Today.ToString("yyyy"));
            fctResult = fctResult.Replace("{PathSeparator}", Path.DirectorySeparatorChar.ToString());

            Regex regex = new Regex("%(?<setting>.+?)%");
            MatchCollection matches = regex.Matches(fctResult);
            foreach (Match match in matches)
            {
                string replacedSettingValue;
                string replacedSettingName = match.Groups["setting"].ToString();
                string replacedSettingPlaceHolder = string.Format("%{0}%", replacedSettingName);
                replacedSettingValue = settingManager.GetPath((ESystemPath)Enum.Parse(typeof(ESystemPath), replacedSettingName));
                fctResult = fctResult.ToString().Replace(replacedSettingPlaceHolder, replacedSettingValue);
            }

            return fctResult;

        }
    }
}
