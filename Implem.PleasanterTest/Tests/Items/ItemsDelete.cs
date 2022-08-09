using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Tests.Items
{
    public class ItemsDelete
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            UserModel userModel,
            List<JsonTest> jsonTests)
        {
            var id = Initializer.Titles.Get(title);
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.ItemsDelete(id: id));
            var results = Results(context: context);
            Assert.True(Compare.Json(
                context: context,
                results: results,
                jsonTests: jsonTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var jsonTests = new List<JsonTest>()
            {
                JsonData.ExistsOne(
                    method: "SetMemory",
                    target: "formChanged"),
                JsonData.ExistsOne(
                    method: "Invoke",
                    target: "back")
            };
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "RecordToDeleteSite2",
                    jsonTests: jsonTests),
                new TestPart(
                    title: "RecordToDeleteSite3",
                    jsonTests: jsonTests),
                new TestPart(
                    title: "RecordToDeleteSite4",
                    jsonTests: jsonTests),
                new TestPart(
                    title: "RecordToDeleteSite6",
                    jsonTests: jsonTests),
                new TestPart(
                    title: "RecordToDeleteSite7",
                    jsonTests: jsonTests),
                new TestPart(
                    title: "RecordToDeleteSite8",
                    jsonTests: jsonTests)
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    userModel: testPart.UserModel,
                    jsonTests: testPart.JsonTests);
            }
        }

        private static object[] TestData(
            string title,
            UserModel userModel,
            List<JsonTest> jsonTests)
        {
            return new object[]
            {
                title,
                userModel,
                jsonTests
            };
        }

        private static string Results(Context context)
        {
            var itemModel = Initializer.ItemIds.Get(context.Id);
            return itemModel.Delete(context: context);
        }
    }
}
