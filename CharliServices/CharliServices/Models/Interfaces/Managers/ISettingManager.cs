using McpNetwork.Charli.Server.Models.Enums;

namespace McpNetwork.Charli.Server.Models.Interfaces.Managers
{
    internal interface ISettingManager : ICharliManager
    {
        /// <summary>
        /// Default culture
        /// </summary>
        string DefaultCulture { get; }

        /// <summary>
        /// Settings full file name
        /// </summary>
        string SettingsFileName { get; set; }

        #region TODO --> To be kept ? 
        /// <summary>
        /// Get a setting
        /// </summary>
        /// <param name="settingGroup"></param>
        /// <param name="settingName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool GetSetting(string settingGroup, string settingName, out string value);
        /// <summary>
        /// Get a setting
        /// </summary>
        /// <param name="settingGroup"></param>
        /// <param name="settingName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool GetSetting(string settingGroup, string settingName, out int value);
        /// <summary>
        /// Get a setting
        /// </summary>
        /// <param name="settingGroup"></param>
        /// <param name="settingName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool GetSetting(string settingGroup, string settingName, out double value);
        /// <summary>
        /// Get a setting
        /// </summary>
        /// <param name="settingGroup"></param>
        /// <param name="settingName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool GetSetting(string settingGroup, string settingName, out DateTime value);
        /// <summary>
        /// Get a setting
        /// </summary>
        /// <param name="settingGroup"></param>
        /// <param name="settingName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool GetSetting(string settingGroup, string settingName, out bool value);
        /// <summary>
        /// Get a setting
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="settingGroup"></param>
        /// <param name="settingName"></param>
        /// <returns></returns>
        T GetSetting<T>(string settingGroup, string settingName);
        /// <summary>
        /// Get a setting
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="settingGroup"></param>
        /// <param name="settingName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        T GetSetting<T>(string settingGroup, string settingName, object defaultValue);

        /// <summary>
        /// Get a system path
        /// </summary>
        /// <param name="requiredPath"></param>
        /// <returns></returns>
        string GetPath(ESystemPath requiredPath);

        /// <summary>
        /// Get a connection string
        /// </summary>
        /// <param name="connectionName"></param>
        /// <returns></returns>
        string ConnectionString(string connectionName);
        #endregion
    }
}
