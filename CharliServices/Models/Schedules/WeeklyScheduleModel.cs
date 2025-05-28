namespace McpNetwork.Charli.Server.Models.Schedules
{
    /// <summary>
    /// Weekly schedule
    /// </summary>
    public class WeeklyScheduleModel
    {
        /// <summary>
        /// Start which days
        /// </summary>
        public List<DayOfWeek> Days { get; set; }

        /// <summary>
        /// Start at which hour
        /// </summary>
        public TimeSpan StartTime { get; set; }
    }
}
