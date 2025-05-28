using System.Xml.Serialization;

namespace McpNetwork.Charli.Server.Models.Setting
{
    public class XMLSettingGroup
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("setting")]
        public List<XMLSetting> Settings { get; set; }
    }
}
