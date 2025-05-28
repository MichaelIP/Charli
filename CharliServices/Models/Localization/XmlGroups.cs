using System.Xml.Serialization;

namespace McpNetwork.Charli.Server.Models.Localization
{
    public class XmlGroups : ILocaleGroupOwner
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("group")]
        public List<XmlGroups> Groups { get; set; }

        [XmlElement("localization")]
        public List<XmlLocalization> Localizations { get; set; }
    }
}
