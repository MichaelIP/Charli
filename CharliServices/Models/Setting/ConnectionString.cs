using System.Xml.Serialization;

namespace McpNetwork.Charli.Server.Models.Setting
{
    public class ConnectionString
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlText]
        public string Value { get; set; }
    }
}
