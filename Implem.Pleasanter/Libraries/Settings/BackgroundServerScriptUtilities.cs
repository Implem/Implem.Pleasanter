using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.BackgroundServices;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using Quartz;
using Quartz.Impl.Matchers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using static Implem.Pleasanter.Libraries.DataSources.Rds;

namespace Implem.Pleasanter.Libraries.Settings
{
    public static class BackgroundServerScriptUtilities
    {
        public static void InitSchedule()
        {
            Task.Run(async () =>
            {
                await InitScheduleAsync();
            });
        }

        public static void Reschedule(BackgroundServerScripts backgroundServerScripts)
        {
            Task.Run(async () =>
            {
                await RescheduleAsync(backgroundServerScripts);
            });
        }

        private static async Task InitScheduleAsync()
        {
            if (Parameters.Script.ServerScript != true || Parameters.Script.BackgroundServerScript != true) return;
            var context = GetContext();
            var ss = GetSitemSettings();
            var dataTable = Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectTenants(
                    tableType: Sqls.TableTypes.Normal,
                    column: TenantsColumn().TenantSettings()));
            foreach (var dataRow in dataTable.AsEnumerable())
            {
                var tenant = new TenantModel(context: context, ss: ss, dataRow: dataRow);
                await RescheduleAsync(tenant.TenantSettings.BackgroundServerScripts);
            }
        }

        private static async Task RescheduleAsync(BackgroundServerScripts backgroundServerScripts)
        {
            if (Parameters.Script.ServerScript != true || Parameters.Script.BackgroundServerScript != true) return;
            var scheduler = CustomQuartzHostedService.Scheduler;
            string groupKey = $"BGServerScript_TenantId_{backgroundServerScripts.TenantId}";
            await scheduler.DeleteJobs(await scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupContains(groupKey)));
            backgroundServerScripts.Scripts
                .Where(v => v.Disabled != true && v.Background == true && v.Shared == false)
                .Where(v => v.backgoundSchedules.Count > 0)
                .ForEach(script =>
                {
                    var jobKey = new JobKey($"s_{script.Id}", groupKey);
                    var job = JobBuilder.Create(typeof(BackgroundServerScriptJob))
                        .WithIdentity(jobKey)
                        .UsingJobData("tenantId", backgroundServerScripts.TenantId)
                        .UsingJobData("scriptId", script.Id)
                        .UsingJobData("userId", script.UserId.ToString())
                        .UsingJobData("scripts", null)
                        .Build();
                    scheduler.AddJob(job, replace: true, storeNonDurableWhileAwaitingScheduling: true).Wait();
                    foreach (var schedule in script.backgoundSchedules)
                    {
                        var trigger = GetTrigger(schedule)?
                            .UsingJobData("scheduleId", schedule.Id)
                            .ForJob(job)
                            .Build();
                        if (trigger?.GetFireTimeAfter(DateTimeOffset.UtcNow) != null) scheduler.ScheduleJob(trigger).Wait();
                    }
                });
        }

        internal static async Task ExecuteNow(BackgroundServerScripts backgroundServerScripts, int scriptId)
        {
            if (Parameters.Script.ServerScript != true || Parameters.Script.BackgroundServerScript != true) return;
            var scheduler = CustomQuartzHostedService.Scheduler;
            string groupKey = $"BGServerScript_ExeNow_TenantId_{backgroundServerScripts.TenantId}";
            await scheduler.DeleteJobs(await scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupContains(groupKey)));
            var script = backgroundServerScripts.Scripts.FirstOrDefault(s => s.Id == scriptId);
            if (script == null) return;
            var jobKey = new JobKey($"s_{script.Id}", groupKey);
            var job = JobBuilder.Create(typeof(BackgroundServerScriptJob))
                .WithIdentity(jobKey)
                .UsingJobData("tenantId", backgroundServerScripts.TenantId)
                .UsingJobData("scriptId", script.Id)
                .UsingJobData("userId", script.UserId.ToString())
                .UsingJobData("scripts", backgroundServerScripts.ToJson())
                .Build();
            await scheduler.AddJob(job, replace: true, storeNonDurableWhileAwaitingScheduling: true);
            var trigger = TriggerBuilder.Create().StartNow().ForJob(job).Build();
            await scheduler.ScheduleJob(trigger);
        }

        private static TriggerBuilder GetTrigger(
            BackgroundSchedule schedule)
        {
            string cronStr = null;
            switch (schedule.ScheduleType)
            {
                case "hourly":
                    {
                        if(int.TryParse(schedule.ScheduleHourlyTime, out int mm)
                            && mm >= 0 && mm < 60)
                        {
                            cronStr = $"0 {mm} * * * ? *";
                        }
                        break;
                    }
                case "daily":
                    {
                        var (isOk, hh, mm) = ParseHhMm(schedule.ScheduleDailyTime);
                        if (isOk)
                        {
                            cronStr = $"0 {mm} {hh} * * ? *";
                        }
                        break;
                    }
                case "weekly":
                    {
                        var (isOkHhMm, hh, mm) = ParseHhMm(schedule.ScheduleWeeklyTime);
                        var (isOkWeek, week) = ParseIntList(schedule.ScheduleWeeklyWeek);
                        if (isOkHhMm && isOkWeek)
                        {
                            cronStr = $"0 {mm} {hh} ? * {week} *";
                        }
                        break;
                    }
                case "monthly":
                    {
                        var (isOkHhMm, hh, mm) = ParseHhMm(schedule.ScheduleMonthlyTime);
                        var (isOkDay, days) = ParseDays(schedule.ScheduleMonthlyDay);
                        var (isOkMonth, months) = ParseIntList(schedule.ScheduleMonthlyMonth);
                        if (isOkHhMm && isOkDay && isOkMonth)
                        {
                            cronStr = $"0 {mm} {hh} {days} {months} ? *";
                        }
                        break;
                    }
                case "onlyonce":
                    {
                        if (DateTime.TryParse(schedule.ScheduleOnlyOnceTime, out var d))
                        {
                            cronStr = $"0 {d.Minute} {d.Hour} {d.Day} {d.Month} ? {d.Year}";
                        }
                        break;
                    }
                default:
                    throw new ArgumentException(nameof(schedule.ScheduleType));
            }
            if (cronStr.IsNullOrEmpty()) return null;
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(
                !schedule.ScheduleTimeZoneId.IsNullOrEmpty()
                    ? schedule.ScheduleTimeZoneId
                    : !Parameters.Service.TimeZoneDefault.IsNullOrEmpty()
                        ? Parameters.Service.TimeZoneDefault
                        : TimeZoneInfo.Utc.Id);
            return TriggerBuilder.Create()
                .WithCronSchedule(
                    cronStr,
                    (s) => s.InTimeZone(timeZone));
        }

        private static (bool isOk, int hh, int mm) ParseHhMm(string hhmmStr)
        {
            return DateTime.TryParse($"2020-01-01T{hhmmStr}:00.00", out var date)
                ? (true, date.Hour, date.Minute)
                : (false, -1, -1);
        }

        private static (bool isOk, string list) ParseIntList(string str)
        {
            var list = str.Deserialize<List<int>>()?
                .Select(v => v.ToString())
                .Join(",");
            return (!list.IsNullOrEmpty(), list);
        }

        private static (bool isOk, string days) ParseDays(string str)
        {
            var list = str.Deserialize<List<int>>()?
                .Select(v => v == 32 ? "L" : v.ToString())
                .Join(",");
            return (!list.IsNullOrEmpty(), list);
        }

        private static Context GetContext()
        {
            return new Context(tenantId: 0, request: false);
        }

        private static SiteSettings GetSitemSettings()
        {
            return new SiteSettings();
        }
    }
}
