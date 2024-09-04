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
    public class ItemsBulkMove
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
                routeData: RouteData.ItemsBulkMove(id: id),
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
                new KeyValue("MoveTargets", Initializer.Titles.Get("WBS").ToString()),
                new KeyValue("GridCheckedItems", Initializer.Titles
                    .Where(o => o.Key == "RecordToMove2"
                        || o.Key == "RecordToMove3")
                    .Select(o => o.Value)
                    .Join()));
            var baseTests = BaseData.Tests(
                JsonData.ExistsOne(method: "Log"),
                JsonData.ExistsOne(
                    method: "Remove",
                    target: ".grid tr"),
                JsonData.ExistsOne(
                    method: "ClearFormData",
                    target: "GridOffset"),
                JsonData.ExistsOne(
                    method: "ClearFormData",
                    target: "GridCheckAll"),
                JsonData.ExistsOne(
                    method: "ClearFormData",
                    target: "GridUnCheckedItems"),
                JsonData.ExistsOne(
                    method: "ClearFormData",
                    target: "GridCheckedItems"),
                JsonData.ExistsOne(method: "CloseDialog"),
                JsonData.ExistsOne(
                    method: "ReplaceAll",
                    target: "#CopyToClipboards"),
                JsonData.ExistsOne(
                    method: "ReplaceAll",
                    target: "#Aggregations"),
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
                JsonData.Value(
                    method: "Message",
                    value: "{\"Id\":\"BulkMoved\",\"Text\":\"2 件 移動しました。\",\"Css\":\"alert-success\"}"));
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "課題管理",
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
            return itemModel.BulkMove(context: context);
        }
    }
}
