using System.Xml.Serialization;

namespace McpNetwork.Charli.Server.Models.Setting
{
    public class ConnectionStrings
    {
        [XmlElement("connectionString")]
        public List<ConnectionString> ConnectionString { get; set; }

        public ConnectionStrings()
        {
            ConnectionString = new List<ConnectionString>();
        }
    }
}
