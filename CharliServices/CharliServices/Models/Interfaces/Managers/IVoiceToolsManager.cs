namespace McpNetwork.Charli.Server.Models.Interfaces.Managers
{
    internal interface IVoiceToolsManager : ICharliManager
    {

        /// <summary>
        /// Speak
        /// </summary>
        /// <param name="phrase"></param>
        void Speak(string sentence);
        void SetLanguage(string locale);
    }
}
