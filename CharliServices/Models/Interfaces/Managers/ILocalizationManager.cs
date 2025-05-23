namespace McpNetwork.Charli.Server.Models.Interfaces.Managers
{
    internal interface ILocalizationManager : ICharliManager
    {

        /// <summary>
        /// Get a localisation
        /// </summary>
        /// <param name="localeName"></param>
        /// <param name="path"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetLocalization(string localeName, string path, string key);

    }
}
