using System.Xml.Serialization;

namespace McpNetwork.Charli.Server.Models.Setting
{
    public class Global
    {
        [XmlElement("locale")]
        public string Locale { get; set; }
        [XmlElement("folders")]
        public Folders Folders { get; set; }
    }
}
