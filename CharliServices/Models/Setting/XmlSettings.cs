using McpNetwork.Charli.Server.Constants;
using System.Xml.Serialization;

namespace McpNetwork.Charli.Server.Models.Setting
{
    [XmlRoot("settings", Namespace = SettingConstants.XsdNameSpace)]
    public class XmlSettings
    {
        [XmlElement("connectionStrings")]
        public ConnectionStrings ConnectionStrings { get; set; }

        [XmlElement("settingGroup")]
        public List<XMLSettingGroup> SettingGroups { get; set; }

        [XmlElement("global")]
        public Global Globals { get; set; }

        public XmlSettings()
        {
            ConnectionStrings = new ConnectionStrings();
            SettingGroups = new List<XMLSettingGroup>();
            Globals = new Global();
        }
    }
}
