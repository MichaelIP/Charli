namespace McpNetwork.Charli.Server.Models.WebServices
{
    public class LoginResponseModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public DateTime? LastConnectionDate { get; set; }
        public string Token { get; set; }
        public DateTime TokenExpirationDate { get; set; }
    }
}
