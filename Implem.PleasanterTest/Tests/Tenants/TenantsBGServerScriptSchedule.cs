using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Operations;
using Implem.PleasanterTest.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static Implem.PleasanterTest.Utilities.UserData;

namespace Implem.PleasanterTest.Tests.Tenants
{
    [Collection(nameof(TenantsBGServerScriptSchedule))]
    public class TenantsBGServerScriptSchedule
    {
        private static string TimeZone = "Asia/Tokyo";

        static TenantsBGServerScriptSchedule()
        {
            if (!TimeZoneInfo.GetSystemTimeZones().Any(o => o.Id == TimeZone))
            {
                TimeZone = "Tokyo Standard Time";
            }
        }

        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            Forms forms,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.TenantsBGServerScript(),
                httpMethod: "POST",
                forms: forms);
            var results = Results(context: context);
            Initializer.SaveResults(results);
            Assert.True(Tester.Test(
                context: context,
                results: results,
                baseTests: baseTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var scheduleList = "[{\"Name\":\"スケジュール1\",\"ScheduleType\":\"hourly\",\"ScheduleTimeZoneId\":\"{TimeZone}\",\"ScheduleHourlyTime\":\"05\",\"ScheduleDailyTime\":\"00:00\",\"ScheduleWeeklyWeek\":\"[\\\"2\\\",\\\"3\\\",\\\"4\\\"]\",\"ScheduleWeeklyTime\":\"00:00\",\"ScheduleMonthlyMonth\":\"[]\",\"ScheduleMonthlyDay\":\"[]\",\"ScheduleMonthlyTime\":\"00:00\",\"ScheduleOnlyOnceTime\":\"2025/06/03 13:31\",\"Id\":1},{\"Name\":\"スケジュール2\",\"ScheduleType\":\"hourly\",\"ScheduleTimeZoneId\":\"{TimeZone}\",\"ScheduleHourlyTime\":\"00\",\"ScheduleDailyTime\":\"00:00\",\"ScheduleWeeklyWeek\":\"[]\",\"ScheduleWeeklyTime\":\"00:00\",\"ScheduleMonthlyMonth\":\"[]\",\"ScheduleMonthlyDay\":\"[]\",\"ScheduleMonthlyTime\":\"00:00\",\"ScheduleOnlyOnceTime\":\"\",\"Id\":2}]";
            scheduleList.Replace("{TimeZone}", TimeZone);
            var validHtmlTests = BaseData.Tests(HtmlData.ExistsOne(selector: "#Editor"));
            var notFoundMessage = BaseData.Tests(HtmlData.NotFoundMessage());
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "AddServerScriptSchedules"),
                        new KeyValue("ServerScriptScheduleName", "スケジュール3"),
                        new KeyValue("ServerScriptScheduleType", "hourly"),
                        new KeyValue("ServerScriptScheduleTimeZoneId", $"{TimeZone}"),
                        new KeyValue("ServerScriptScheduleHourlyTime", "00"),
                        new KeyValue("ServerScriptScheduleDailyTime", "00:00"),
                        new KeyValue("ServerScriptScheduleWeeklyWeek", "[]"),
                        new KeyValue("ServerScriptScheduleWeeklyTime", "00:00"),
                        new KeyValue("ServerScriptScheduleMonthlyMonth", "[]"),
                        new KeyValue("ServerScriptScheduleMonthlyDay", "[]"),
                        new KeyValue("ServerScriptScheduleMonthlyTime", "00:00"),
                        new KeyValue("ServerScriptScheduleOnlyOnceTime", ""),
                        new KeyValue("EditServerScriptScheduleList", scheduleList)),
                    baseTests: BaseData.Tests(
                        JsonData.TextContains(
                            method: "SetValue",
                            target: "#BackgroundSchedule",
                            value: $"{{\"Name\":\"スケジュール3\",\"ScheduleType\":\"hourly\",\"ScheduleTimeZoneId\":\"{TimeZone}\",\"ScheduleHourlyTime\":\"00\",\"ScheduleDailyTime\":\"00:00\",\"ScheduleWeeklyWeek\":\"[]\",\"ScheduleWeeklyTime\":\"00:00\",\"ScheduleMonthlyMonth\":\"[]\",\"ScheduleMonthlyDay\":\"[]\",\"ScheduleMonthlyTime\":\"00:00\",\"ScheduleOnlyOnceTime\":\"\",\"Id\":3}}"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                 new TestPart(
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "MoveUpServerScriptSchedules"),
                        new KeyValue("EditServerScriptSchedules", "[\"2\"]"),
                        new KeyValue("ServerScriptId", "1"),
                        new KeyValue("BackgroundSchedule", scheduleList)),
                    baseTests: BaseData.Tests(
                        JsonData.TextCheckOrder(
                            method: "ReplaceAll",
                            target: "#EditServerScriptSchedules",
                            wordArray: new string[] {
                                "<td>スケジュール2</td>",
                                "<td>スケジュール1</td>" }), 
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                 new TestPart(
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "MoveDownServerScriptSchedules"),
                        new KeyValue("EditServerScriptSchedules", "[\"1\"]"),
                        new KeyValue("ServerScriptId", "1"),
                        new KeyValue("BackgroundSchedule", scheduleList)),
                    baseTests: BaseData.Tests(
                        JsonData.TextCheckOrder(
                            method: "ReplaceAll",
                            target: "#EditServerScriptSchedules",
                            wordArray: new string[] {
                                "<td>スケジュール2</td>",
                                "<td>スケジュール1</td>" }),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                 new TestPart(
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "NewServerScriptSchedules"),
                        new KeyValue("ServerScriptId", "1"),
                        new KeyValue("BackgroundSchedule", scheduleList)),
                    baseTests: BaseData.Tests(
                        JsonData.HtmlTests(
                            method: "Html",
                            target: "#ServerScriptScheduleDialog",
                            selector: "#ServerScriptScheduleForm"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                 new TestPart(
                        forms: FormsUtilities.Get(
                            new KeyValue("ControlId", "CopyServerScriptSchedules"),
                            new KeyValue("ServerScriptScheduleId", "1"),
                            new KeyValue("EditServerScriptSchedules","[\"2\"]"),
                            new KeyValue("BackgroundSchedule", scheduleList)),
                        baseTests: BaseData.Tests(
                            JsonData.HtmlTextContains(
                                method:"ReplaceAll",
                                target: "#EditServerScriptSchedules",
                                selector: "#EditServerScriptSchedulesWrap",
                                value: "</td><td>3</td><td>スケジュール2</td>"),
                            JsonData.ExistsOne(
                                method: "SetMemory",
                                target: "formChanged")),
                        userType: UserData.UserTypes.Privileged),
                 new TestPart(
                        forms: FormsUtilities.Get(
                            new KeyValue("ControlId", "DeleteServerScriptSchedules"),
                            new KeyValue("ServerScriptScheduleId", "1"),
                            new KeyValue("EditServerScriptSchedules","[\"1\"]"),
                            new KeyValue("BackgroundSchedule", scheduleList)),
                        baseTests: BaseData.Tests(
                            JsonData.TextCountOf(
                                method:"ReplaceAll",
                                target: "#EditServerScriptSchedules",
                                value: "<td>スケジュール1</td>",
                                estimate: 0),
                            JsonData.ExistsOne(
                                method: "SetMemory",
                                target: "formChanged")),
                        userType: UserData.UserTypes.Privileged),
                 new TestPart(
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "EditServerScriptSchedules"),
                        new KeyValue("ServerScriptId", "1"),
                        new KeyValue("ServerScriptScheduleId", "2"),
                        new KeyValue("BackgroundSchedule", scheduleList)),
                    baseTests: BaseData.Tests(
                        JsonData.HtmlTests(
                            method: "Html",
                            target: "#ServerScriptScheduleDialog",
                            selector: "#ServerScriptScheduleForm"),
                        JsonData.HtmlTextContains(
                            method: "Html",
                            target: "#ServerScriptScheduleDialog",
                            selector: "#ServerScriptScheduleForm",
                            value: "<input id=\"ServerScriptScheduleName\" name=\"ServerScriptScheduleName\" class=\"control-textbox always-send\" type=\"text\" value=\"スケジュール2\" data-validate-required=\"1\">"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                    new TestPart(
                        forms: FormsUtilities.Get(
                            new KeyValue("ControlId", "UpdateServerScriptSchedules"),
                            new KeyValue("ServerScriptScheduleId", "1"),
                            new KeyValue("ServerScriptScheduleName","スケジュール1-update"),
                            new KeyValue("ServerScriptScheduleType", "hourly"),
                            new KeyValue("ServerScriptScheduleHourlyTime", "00"),
                            new KeyValue("ServerScriptScheduleDailyTime", "00:00"),
                            new KeyValue("ServerScriptScheduleWeeklyWeek", "[]"),
                            new KeyValue("ServerScriptScheduleWeeklyTime", "00:00"),
                            new KeyValue("ServerScriptScheduleMonthlyMonth", "[]"),
                            new KeyValue("ServerScriptScheduleMonthlyDay", "[]"),
                            new KeyValue("ServerScriptScheduleMonthlyTime", "00:00"),
                            new KeyValue("ServerScriptScheduleOnlyOnceTime", ""),
                            new KeyValue("EditServerScriptScheduleList", scheduleList)),
                        baseTests: BaseData.Tests(
                            JsonData.HtmlTextContains(
                                method: "Html",
                                target: "#EditServerScriptSchedules",
                                selector: "#EditServerScriptSchedulesWrap",
                                value: "<td>スケジュール1-update</td>"),
                            JsonData.ExistsOne(
                                method: "SetMemory",
                                target: "formChanged")),
                        userType: UserData.UserTypes.Privileged),
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    forms : testPart.Forms,
                    userModel: testPart.UserModel,
                    baseTests: testPart.BaseTests);
            }
        }

        private static object[] TestData(
            Forms forms,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            return new object[]
            {
                forms,
                userModel,
                baseTests
            };
        }

        private static string Results(Context context)
        {
            return TenantUtilities.SetBGServerScript(
                context: context,
                ss: SiteSettingsUtilities.TenantsSiteSettings(context: context),
                tenantId: context.TenantId);
        }
    }
}
