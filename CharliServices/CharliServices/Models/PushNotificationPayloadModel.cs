namespace McpNetwork.Charli.Server.Models
{
    public class PushNotificationPayloadModel
    {
        public string NotificationId { get; set; }
        public string Sender { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime TimeStamp { get; private set; }

        public PushNotificationPayloadModel()
        {
            TimeStamp = DateTime.Now;
        }
    }
}
