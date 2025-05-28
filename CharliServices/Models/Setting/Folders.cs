using System.Xml.Serialization;

namespace McpNetwork.Charli.Server.Models.Setting
{
    public class Folders
    {
        [XmlElement("application-data")]
        public string ApplicationData { get; set; }
        [XmlElement("logs")]
        public string Logs { get; set; }
        [XmlElement("plugins")]
        public string Plugins { get; set; }
        [XmlElement("managers")]
        public string Managers { get; set; }
        [XmlElement("locales")]
        public string Locales { get; set; }
        [XmlElement("grammars")]
        public string Grammars { get; set; }
    }
}
