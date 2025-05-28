using McpNetwork.Charli.Server.Models.Enums;

namespace McpNetwork.Charli.Server.Models.Schedules
{
    /// <summary>
    /// Schedule Model
    /// </summary>
    public class ScheduleModel
    {
        /// <summary>
        /// Start date of the schedule
        /// </summary>
        public DateTime ScheduledFrom { get; set; }

        /// <summary>
        /// End date of the schedule
        /// </summary>
        public DateTime ScheduledTo { get; set; }

        /// <summary>
        /// Frequency 
        /// </summary>
        public EScheduleFrequency Freqency { get; set; }

        /// <summary>
        /// Repeat schedule each every
        /// </summary>
        /// <remarks>Not supported for Weekly and Monthly schedules</remarks>
        public int EveryFrequency { get; set; }

        /// <summary>
        /// Daily schedule more data
        /// </summary>
        public DailyScheduleModel DailySchedule { get; set; }

        /// <summary>
        /// Weekly schedule more data
        /// </summary>
        public WeeklyScheduleModel WeeklySchedule { get; set; }

        /// <summary>
        /// Monthly schedule more data
        /// </summary>
        public MonthlyScheduleModel MonthlySchedule { get; set; }

        /// <summary>
        /// User parameters sent back to the caller
        /// </summary>
        public object UserParameters { get; set; }

    }

}
