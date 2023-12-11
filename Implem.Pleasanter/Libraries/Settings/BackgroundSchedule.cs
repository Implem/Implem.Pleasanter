using Implem.Pleasanter.Controllers;
using Implem.Pleasanter.Interfaces;
using Quartz;
using System;

namespace Implem.Pleasanter.Libraries.Settings
{
    [Serializable()]
    public class BackgroundSchedule : ISettingListItem
    {
        /*
         * ScheduleTypeは下記の値
            hourly,
            daily,
            weekly,
            monthly,
            onlyonce,
         */

        public int Id { get; set; }
        public string Name;
        public string ScheduleType;
        public string ScheduleTimeZoneId;
        public string ScheduleHourlyTime;
        public string ScheduleDailyTime = "00:00";
        public string ScheduleWeeklyWeek;
        public string ScheduleWeeklyTime = "00:00";
        public string ScheduleMonthlyMonth;
        public string ScheduleMonthlyDay;
        public string ScheduleMonthlyTime = "00:00";
        public string ScheduleOnlyOnceTime;

        public BackgroundSchedule(
            int id = 0,
            string name = null,
            string type = null,
            string timeZoneId = null,
            string dailyTime = null,
            string hourlyTime = null,
            string weeklyWeek = null,
            string weeklyTime = null,
            string monthlyMonth = null,
            string monthlyDay = null,
            string monthlyTime = null,
            string onlyOnceTime = null)
        {
            Id = id;
            if (name != null) Name = name;
            if (type != null) ScheduleType = type;
            if (timeZoneId != null) ScheduleTimeZoneId = timeZoneId;
            if (dailyTime != null) ScheduleHourlyTime = hourlyTime;
            if (hourlyTime != null) ScheduleDailyTime = dailyTime;
            if (weeklyWeek != null) ScheduleWeeklyWeek = weeklyWeek;
            if (weeklyTime != null) ScheduleWeeklyTime = weeklyTime;
            if (monthlyMonth != null) ScheduleMonthlyMonth = monthlyMonth;
            if (monthlyDay != null) ScheduleMonthlyDay = monthlyDay;
            if (monthlyTime != null) ScheduleMonthlyTime = monthlyTime;
            if (onlyOnceTime != null) ScheduleOnlyOnceTime = onlyOnceTime;
        }

        public void UpdateFromRecode(
            BackgroundSchedule schedule)
        {
            Name = schedule.Name;
            ScheduleType = schedule.ScheduleType;
            ScheduleTimeZoneId = schedule.ScheduleTimeZoneId;
            ScheduleHourlyTime = schedule.ScheduleHourlyTime;
            ScheduleDailyTime = schedule.ScheduleDailyTime;
            ScheduleWeeklyWeek = schedule.ScheduleWeeklyWeek;
            ScheduleWeeklyTime = schedule.ScheduleWeeklyTime;
            ScheduleMonthlyMonth = schedule.ScheduleMonthlyMonth;
            ScheduleMonthlyDay = schedule.ScheduleMonthlyDay;
            ScheduleMonthlyTime = schedule.ScheduleMonthlyTime;
            ScheduleOnlyOnceTime = schedule.ScheduleOnlyOnceTime;
        }
    }
}