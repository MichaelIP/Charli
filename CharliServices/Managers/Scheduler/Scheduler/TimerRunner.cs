using McpNetwork.Charli.Server.Models.Enums;
using McpNetwork.Charli.Server.Models.Interfaces.Managers;
using McpNetwork.Charli.Server.Models.Schedules;
using System.Runtime.CompilerServices;

namespace McpNetwork.Charli.Server.Managers.Scheduler.Scheduler
{
    internal class TimerRunner : IDisposable
    {

        private class ScheduledEvents
        {
            public Guid Key { get; set; }
            public Action<Guid, ScheduleModel> Action { get; set; }
            public ScheduleModel Schedule { get; set; }

            public DateTime NextSchedule { get; set; }
        }

        private ILoggerManager logger;

        private Timer timer;
        private readonly AutoResetEvent resetEvent = new AutoResetEvent(false);

        private readonly List<ScheduledEvents> schedules;

        public TimerRunner(ILoggerManager loggerManager)
        {
            logger = loggerManager;
            var startDelay = new TimeSpan(0, 0, 10);
            var interval = new TimeSpan(0, 0, 10);
            timer = new Timer(TimerInterval, resetEvent, startDelay, interval);
            schedules = new List<ScheduledEvents>();
        }

        private void TimerInterval(object stateInfo)
        {
            var selectedSchedules = schedules
                .Where(s => s.Schedule.ScheduledFrom <= DateTime.Now)
                .Where(s => s.Schedule.ScheduledTo >= DateTime.Now)
                .Where(s => s.NextSchedule <= DateTime.Now)
                .ToList();

            // Update next schedule time
            selectedSchedules.ForEach(s => s.NextSchedule = GetNextSchedule(s.Schedule));

            try
            {
                foreach (var schedule in selectedSchedules)
                {
                    schedule.Action(schedule.Key, schedule.Schedule);
                }
            }
            catch (Exception e)
            {
                LogError(e.Message, e);
            }

        }

        public void Dispose()
        {
            resetEvent.Set();
            timer.Dispose();
        }

        public Guid AddSchedule(ScheduleModel schedule, Action<Guid, ScheduleModel> action)
        {
            var guid = Guid.NewGuid();

            var addedSchedule = new ScheduledEvents
            {
                Key = guid,
                Schedule = schedule,
                Action = action,
                NextSchedule = GetNextSchedule(schedule),
            };
            schedules.Add(addedSchedule);
            var message = string.Format("Added schedule [{0}]. Will starts at {1}", addedSchedule.Key, addedSchedule.NextSchedule);
            LogInfo(message);

            return guid;
        }

        public void RemoveSchedule(Guid schedule)
        {
            var item = schedules.FirstOrDefault(s => s.Key == schedule);
            if (item != null)
            {
                schedules.Remove(item);
            }
        }

        private DateTime GetNextSchedule(ScheduleModel schedule)
        {
            var result = DateTime.Now;
            switch (schedule.Freqency)
            {
                case EScheduleFrequency.Minutes:
                    result = result.AddMinutes(schedule.EveryFrequency);
                    break;
                case EScheduleFrequency.Hours:
                    result = result.AddHours(schedule.EveryFrequency);
                    break;
                case EScheduleFrequency.Days:
                    result = this.GetNextSchedule(result, schedule.DailySchedule.StartTime);
                    break;
                case EScheduleFrequency.Weeks:
                    result = this.GetNextSchedule(result, schedule.WeeklySchedule.StartTime);
                    while (!schedule.WeeklySchedule.Days.Contains(result.DayOfWeek))
                    {
                        result = result.AddDays(1);
                    }
                    break;
                case EScheduleFrequency.Months:
                    result = this.GetNextSchedule(result, schedule.MonthlySchedule.StartTime);
                    while (result.Day != schedule.MonthlySchedule.Day)
                    {
                        result = result.AddDays(1);
                    }
                    break;
            }
            return result;
        }

        private DateTime GetNextSchedule(DateTime start, TimeSpan time)
        {
            var result = new DateTime(start.Year, start.Month, start.Day, time.Hours, time.Minutes, time.Seconds);
            if (result < DateTime.Now)
            {
                result = result.AddDays(1);
            }
            return result;
        }


        #region Log
        private void LogInfo(string message, [CallerMemberName] string caller = null)
        {
            logger.LogInformation(EManagersType.SchedulerManager.ToString(), message, null, caller);
        }

        private void LogError(string message, [CallerMemberName] string caller = null)
        {
            logger.LogError(EManagersType.SchedulerManager.ToString(), message, null, caller);
        }
        private void LogError(string message, Exception exception, [CallerMemberName] string caller = null)
        {
            logger.LogError(EManagersType.SchedulerManager.ToString(), exception, null, caller);
        }

        #endregion
    }
}
