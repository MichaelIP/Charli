using McpNetwork.Charli.Server.Constants;
using System.Xml.Serialization;

namespace McpNetwork.Charli.Server.Models.Localization
{
    [XmlRoot("localizations", Namespace = LocalizationConstants.XsdNameSpace)]
    public class XmlLocalizations : ILocaleGroupOwner
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("default-culture")]
        public string DefaultCulture { get; set; }

        [XmlElement("group")]
        public List<XmlGroups> Groups { get; set; }

        [XmlElement("localization")]
        public List<XmlLocalization> Localizations { get; set; }

    }
}
