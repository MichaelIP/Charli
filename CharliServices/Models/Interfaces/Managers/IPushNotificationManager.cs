namespace McpNetwork.Charli.Server.Models.Interfaces.Managers
{
    internal interface IPushNotificationManager : ICharliManager
    {

        /// <summary>
        /// notify a user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        bool Notify(int userId, PushNotificationPayloadModel payload);


        /// <summary>
        /// Notify a list of users
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        bool Notify(List<int> userIds, PushNotificationPayloadModel payload);

        /// <summary>
        /// Notify users who belong to indicated role
        /// </summary>
        /// <param name="role"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        bool Notify(string role, PushNotificationPayloadModel payload);

    }
}
