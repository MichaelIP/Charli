using FcmSharp;
using FcmSharp.Requests;
using FcmSharp.Settings;
using McpNetwork.Charli.Server.Models;

namespace McpNetwork.Charli.Server.Managers.PushNotification
{
    internal class FcmNotifier
    {

        private string projectId;
        private string projectCredential;
        private FcmClientSettings settings;

        public FcmNotifier(string projectId, string projectCredentials)
        {
            this.projectId = projectId;
            projectCredential = projectCredentials;
            settings = FileBasedFcmClientSettings.CreateFromFile(this.projectId, projectCredential);
        }

        public bool Notify(string notificationToken, PushNotificationPayloadModel payload)
        {
            var result = false;

            using (var client = new FcmClient(settings))
            {
                var messageData = new Dictionary<string, string>()
                {
                    { "NotificationId", payload.NotificationId },
                    { "Sender", payload.Sender },
                    { "Message", payload.Message },
                    { "Title", payload.Title },
                    { "TimeStamp", payload.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss") },
                };

                var message = new FcmMessage
                {
                    ValidateOnly = false,
                    Message = new Message
                    {
                        Data = messageData,
                        Token = notificationToken
                    }
                };

                try
                {
                    var cts = new CancellationTokenSource();
                    var sendStatus = client.SendAsync(message, cts.Token).GetAwaiter().GetResult();

                    result = true;

                }
                catch (Exception e)
                {

                }


            }

            return result;

        }

    }
}
