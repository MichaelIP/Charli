using System.Xml.Serialization;

namespace McpNetwork.Charli.Server.Models.Localization
{
    public class XmlLocalization
    {
        [XmlAttribute("key")]
        public string LocalizationKey { get; set; }
        [XmlElement("translation")]
        public List<XmlTranslation> Translations { get; set; }
    }
}
