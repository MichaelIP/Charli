namespace McpNetwork.Charli.Server.Models.Schedules
{
    /// <summary>
    /// Monthly schedule
    /// </summary>
    public class MonthlyScheduleModel
    {
        /// <summary>
        /// Start which day
        /// </summary>
        public int Day { get; set; }

        /// <summary>
        /// Start at which hour
        /// </summary>
        public TimeSpan StartTime { get; set; }
    }
}
