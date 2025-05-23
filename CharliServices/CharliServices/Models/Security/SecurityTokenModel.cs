namespace McpNetwork.Charli.Server.Models.Security
{
    public class SecurityTokenModel
    {
        public string Token { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateExpired { get; set; }
        public string UserName { get; set; }
        public int UserId { get; set; }
        public List<long> UserRights { get; set; }

        public SecurityTokenModel()
        {
            UserRights = new List<long>();
        }
    }
}
