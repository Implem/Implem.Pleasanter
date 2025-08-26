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
    [Collection(nameof(TenantsBGServerScript))]
    public class TenantsBGServerScript
    {
        static string TenantSettingsJson = """
            {
              "BackgroundServerScripts": {
                "Scripts": [
                  {
                    "backgoundSchedules": [
                      {
                        "Name": "スケジュール1",
                        "ScheduleType": "hourly",
                        "ScheduleTimeZoneId": "{TimeZone}",
                        "ScheduleHourlyTime": "05",
                        "ScheduleDailyTime": "00:00",
                        "ScheduleWeeklyWeek": "[\"2\",\"3\",\"4\"]",
                        "ScheduleWeeklyTime": "00:00",
                        "ScheduleMonthlyMonth": "[]",
                        "ScheduleMonthlyDay": "[]",
                        "ScheduleMonthlyTime": "00:00",
                        "ScheduleOnlyOnceTime": "2025/06/03 13:31",
                        "Id": 1
                      },
                      {
                        "Name": "スケジュール2",
                        "ScheduleType": "hourly",
                        "ScheduleTimeZoneId": "{TimeZone}",
                        "ScheduleHourlyTime": "00",
                        "ScheduleDailyTime": "00:00",
                        "ScheduleWeeklyWeek": "[]",
                        "ScheduleWeeklyTime": "00:00",
                        "ScheduleMonthlyMonth": "[]",
                        "ScheduleMonthlyDay": "[]",
                        "ScheduleMonthlyTime": "00:00",
                        "ScheduleOnlyOnceTime": "",
                        "Id": 2
                      }
                    ],
                    "UserId": "{Test_UserId}",
                    "Title": "BGServerScript:Test1",
                    "Name": "xUnitTest",
                    "WhenloadingSiteSettings": false,
                    "WhenViewProcessing": false,
                    "WhenloadingRecord": false,
                    "BeforeFormula": false,
                    "AfterFormula": false,
                    "BeforeCreate": false,
                    "AfterCreate": false,
                    "BeforeUpdate": false,
                    "AfterUpdate": false,
                    "BeforeDelete": false,
                    "BeforeBulkDelete": false,
                    "AfterDelete": false,
                    "AfterBulkDelete": false,
                    "BeforeOpeningPage": false,
                    "BeforeOpeningRow": false,
                    "Shared": false,
                    "Body": "context.Log(\"BGServerScript:Test1\");",
                    "Functionalize": false,
                    "TryCatch": false,
                    "Disabled": false,
                    "TimeOut": 0,
                    "Background": true,
                    "Id": 1
                  },
                  {
                    "backgoundSchedules": [],
                    "UserId": "{Test_UserId}",
                    "Title": "BGServerScript:Test2",
                    "Name": "",
                    "WhenloadingSiteSettings": false,
                    "WhenViewProcessing": false,
                    "WhenloadingRecord": false,
                    "BeforeFormula": false,
                    "AfterFormula": false,
                    "BeforeCreate": false,
                    "AfterCreate": false,
                    "BeforeUpdate": false,
                    "AfterUpdate": false,
                    "BeforeDelete": false,
                    "BeforeBulkDelete": false,
                    "AfterDelete": false,
                    "AfterBulkDelete": false,
                    "BeforeOpeningPage": false,
                    "BeforeOpeningRow": false,
                    "Shared": false,
                    "Body": "context.Log(\"BGServerScript:Test2\")",
                    "Functionalize": false,
                    "TryCatch": false,
                    "Disabled": false,
                    "TimeOut": 0,
                    "Background": true,
                    "Id": 2
                  }
                ],
                "TenantId": "{Test_TenantId}"
              }
            }
            """;

        private static string TimeZone = "Asia/Tokyo";

        static TenantsBGServerScript()
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
            context.SessionData.Add("TenantSettings", TenantSettingsJson);
            var results = Results(context: context);
            Initializer.SaveResults(results);
            Assert.True(Tester.Test(
                context: context,
                results: results,
                baseTests: baseTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var userModel = UserData.Get(UserData.UserTypes.Privileged);
            TenantSettingsJson = TenantSettingsJson
                .Replace("{Test_UserId}", userModel.UserId.ToString())
                .Replace("{Test_TenantId}", userModel.TenantId.ToString())
                .Replace("{TimeZone}", TimeZone);
            var validHtmlTests = BaseData.Tests(HtmlData.ExistsOne(selector: "#Editor"));
            var notFoundMessage = BaseData.Tests(HtmlData.NotFoundMessage());
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "NewServerScript")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#ServerScriptDialog"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "AddServerScript"),
                        new KeyValue("ServerScriptBody", "context.Log(\"AddServerScript\");"),
                        new KeyValue("ServerScriptUser", $"{userModel.UserId}"),
                        new KeyValue("ServerScriptTitle", "AddServerScript-xUnit"),
                        new KeyValue("ServerScriptId", "0")),
                    baseTests: BaseData.Tests(
                        JsonData.HtmlTextContains(
                            method: "ReplaceAll",
                            target: "#EditServerScriptWrap",
                            selector: "#EditServerScriptWrap",
                            value: "<td>3</td><td>AddServerScript-xUnit</td>"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "UpdateServerScript"),
                        new KeyValue("ServerScriptUser", $"{userModel.UserId}"),
                        new KeyValue("ServerScriptTitle", "BGServerScript:Test2-update"),
                        new KeyValue("ServerScriptId", "2"),
                        new KeyValue("EditServerScript", "[\"2\"]")),
                    baseTests: BaseData.Tests(
                        JsonData.HtmlTextContains(
                            method: "ReplaceAll",
                            target: "#EditServerScriptWrap",
                            selector: "#EditServerScriptWrap",
                            value: "<td>2</td><td>BGServerScript:Test2-update</td>"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "MoveUpServerScripts"),
                        new KeyValue("ServerScriptId", "1"),
                        new KeyValue("EditServerScript", "[\"2\"]")),
                    baseTests: BaseData.Tests(
                        JsonData.TextCheckOrder(
                            method: "ReplaceAll",
                            target: "#EditServerScriptWrap",
                            wordArray: new string[]
                            {
                                "<td>BGServerScript:Test2</td>",
                                "<td>BGServerScript:Test1</td>"
                            }),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "MoveDownServerScripts"),
                        new KeyValue("ServerScriptId", "1"),
                        new KeyValue("EditServerScript", "[\"1\"]")),
                    baseTests: BaseData.Tests(
                        JsonData.TextCheckOrder(
                            method: "ReplaceAll",
                            target: "#EditServerScriptWrap",
                            wordArray: new string[]
                            {
                                "<td>BGServerScript:Test2</td>",
                                "<td>BGServerScript:Test1</td>"
                            }),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "CopyServerScripts"),
                        new KeyValue("ServerScriptId", "1"),
                        new KeyValue("EditServerScript", "[\"2\"]")),
                    baseTests: BaseData.Tests(
                        JsonData.TextCountOf(
                            method: "ReplaceAll",
                            target: "#EditServerScriptWrap",
                            value: "<td>BGServerScript:Test2</td>",
                            estimate: 2),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "DeleteServerScripts"),
                        new KeyValue("ServerScriptId", "1"),
                        new KeyValue("EditServerScript", "[\"2\"]")),
                    baseTests: BaseData.Tests(
                        JsonData.TextCountOf(
                            method: "ReplaceAll",
                            target: "#EditServerScriptWrap",
                            value: "<td>BGServerScript:Test2</td>",
                            estimate: 0),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "EditServerScript"),
                        new KeyValue("ServerScriptId", "1")),
                    baseTests: BaseData.Tests(
                        JsonData.HtmlTests(
                            method: "Html",
                            target: "#ServerScriptDialog",
                            selector: "#ServerScriptForm"),
                        JsonData.HtmlTextContains(
                            method: "Html",
                            target: "#ServerScriptDialog",
                            selector: "#ServerScriptForm",
                            value: "<input id=\"ServerScriptTitle\" name=\"ServerScriptTitle\" class=\"control-textbox always-send\" type=\"text\" value=\"BGServerScript:Test1\" data-validate-required=\"1\" autofocus=\"autofocus\">"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                 new TestPart(
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "ExecServerScript"),
                        new KeyValue("ServerScriptUser", $"{userModel.UserId}"),
                        new KeyValue("ServerScriptId", "1"),
                        new KeyValue("ServerScriptName", "xUnitTest")),
                    baseTests: BaseData.Tests(
                        JsonData.Value(
                            method: "Message",
                            value: "{\"Text\":\"\\\" xUnitTest \\\" を実行しました。\",\"Css\":\"alert-success\"}"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged)
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
