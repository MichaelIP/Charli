using System.Xml.Serialization;

namespace McpNetwork.Charli.Server.Models.Localization
{
    public class XmlTranslation
    {
        [XmlAttribute("culture")]
        public string Culture { get; set; }
        [XmlText]
        public string Value { get; set; }
    }
}
