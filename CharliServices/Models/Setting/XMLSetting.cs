using McpNetwork.Charli.Server.Models.Enums;
using System.Xml.Serialization;

namespace McpNetwork.Charli.Server.Models.Setting
{
    public class XMLSetting
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("data-type")]
        public ESettingDataType DataType { get; set; }

        [XmlText]
        public string Value { get; set; }

    }
}
