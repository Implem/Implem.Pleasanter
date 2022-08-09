using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Implem.PleasanterTest.Tests.Items
{
    public class ItemsBulkUpdate
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            Forms forms,
            UserModel userModel,
            List<JsonTest> jsonTests)
        {
            var id = Initializer.Titles.Get(title);
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.ItemsBulkUpdate(id: id),
                httpMethod: "POST",
                forms: forms);
            var json = Results(context: context);
            Assert.True(Compare.Json(
                context: context,
                json: json,
                jsonTests: jsonTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var forms = FormsUtilities.Get(
                new KeyValue("ControlId", "BulkUpdate"),
                new KeyValue("BulkUpdateColumnName", "ClassA"),
                new KeyValue("Issues_ClassA", "設計"),
                new KeyValue("GridCheckedItems", Initializer.Titles
                    .Where(o => o.Key == "サーバ要件の確認"
                        || o.Key == "ネットワーク要件の確認")
                    .Select(o => o.Value)
                    .Join()));
            var jsonTests = JsonData.Tests(
                JsonData.ExistsOne(method: "Log"),
                JsonData.ExistsOne(
                    method: "Append",
                    target: "#Grid"),
                JsonData.ExistsOne(
                    method: "SetValue",
                    target: "#GridOffset"),
                JsonData.ExistsOne(
                    method: "SetValue",
                    target: "#GridRowIds"),
                JsonData.ExistsOne(
                    method: "SetValue",
                    target: "#GridColumns"),
                JsonData.ExistsOne(
                    method: "Paging",
                    target: "#Grid"),
                JsonData.ExistsOne(method: "Message"));
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "WBS",
                    forms: forms,
                    jsonTests: jsonTests)
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    forms: testPart.Forms,
                    userModel: testPart.UserModel,
                    jsonTests: testPart.JsonTests);
            }
        }

        private static object[] TestData(
            string title,
            Forms forms,
            UserModel userModel,
            List<JsonTest> jsonTests)
        {
            return new object[]
            {
                title,
                forms,
                userModel,
                jsonTests
            };
        }

        private static string Results(Context context)
        {
            var itemModel = Initializer.ItemIds.Get(context.Id) ?? new ItemModel();
            return itemModel.BulkUpdate(context: context);
        }
    }
}
