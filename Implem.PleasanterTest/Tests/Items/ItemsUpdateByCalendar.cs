using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Tests.Items
{
    public class ItemsUpdateByCalendar
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            Forms forms,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            var id = Initializer.Titles.Get(title);
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.ItemsUpdateByCalendar(id: id),
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
            var forms = FormsUtilities.Get(
                new KeyValue("ControlId", "CalendarBody"),
                new KeyValue("Id", Initializer.Titles.Get("ネットワークの構築").ToString()),
                new KeyValue("Issues_StartTime", DateTime.Now.AddDays(0).Date.ToString("yyyy/MM/dd 0:0:0")),
                new KeyValue("Issues_CompletionTime", DateTime.Now.AddDays(3).Date.ToString("yyyy/MM/dd 0:0:0")));
            var baseTests = BaseData.Tests(
                JsonData.ExistsOne(
                    method: "Html",
                    target: "#FullCalendarBody"),
                JsonData.ExistsOne(
                    method: "ClearFormData",
                    target: "View"),
                JsonData.ExistsOne(
                    method: "Invoke",
                    target: "initRelatingColumnWhenViewChanged"),
                JsonData.ExistsOne(
                    method: "ReplaceAll",
                    target: "title"),
                JsonData.ExistsOne(
                    method: "ReplaceAll",
                    target: "#Breadcrumb"),
                JsonData.ExistsOne(
                    method: "ReplaceAll",
                    target: "#Guide"),
                JsonData.ExistsOne(
                    method: "ReplaceAll",
                    target: "#CopyToClipboards"),
                JsonData.ExistsOne(
                    method: "ReplaceAll",
                    target: "#Aggregations"),
                JsonData.ExistsOne(
                    method: "ReplaceAll",
                    target: "#MainCommandsContainer"),
                JsonData.ExistsOne(
                    method: "SetValue",
                    target: "#EditOnGrid"),
                JsonData.ExistsOne(
                    method: "SetMemory",
                    target: "formChanged"),
                JsonData.ExistsOne(
                    method: "Invoke",
                    target: "setCalendar"),
                JsonData.ExistsOne(method: "Message"),
                JsonData.ExistsOne(method: "LoadScroll"),
                JsonData.ExistsOne(method: "ClearFormData"),
                JsonData.ExistsOne(method: "Log"),
                JsonData.ExistsOne(
                    method: "Events",
                    target: "on_calendar_load"));
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "WBS",
                    forms: forms,
                    baseTests: baseTests)
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    forms: testPart.Forms,
                    userModel: testPart.UserModel,
                    baseTests: testPart.BaseTests);
            }
        }

        private static object[] TestData(
            string title,
            Forms forms,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            return new object[]
            {
                title,
                forms,
                userModel,
                baseTests
            };
        }

        private static string Results(Context context)
        {
            var itemModel = Initializer.ItemIds.Get(context.Id) ?? new ItemModel();
            return itemModel.UpdateByCalendar(context: context);
        }
    }
}
