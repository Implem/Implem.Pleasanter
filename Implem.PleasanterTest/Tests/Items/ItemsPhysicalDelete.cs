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
    public class ItemsPhysicalDelete
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
                routeData: RouteData.ItemsPhysicalDelete(id: id),
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
                new KeyValue("GridCheckedItems", Initializer.Titles
                    .Where(o => o.Key == "RecordToPhysicalDelete1"
                        || o.Key == "RecordToPhysicalDelete2")
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
                    value: "{\"Id\":\"PhysicalBulkDeletedFromRecycleBin\",\"Text\":\"ごみ箱から 2 件 削除しました。\",\"Css\":\"alert-success\"}"));
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "WBS",
                    forms: forms,
                    baseTests: baseTests,
                    userType: UserData.UserTypes.TenantManager1)
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
            DeleteItems(context);
            var itemModel = Initializer.ItemIds.Get(context.Id) ?? new ItemModel();
            return itemModel.PhysicalBulkDelete(context: context);
        }

        private static void DeleteItems(Context context)
        {
            // ごみ箱に移動するために削除を実施。
            foreach (var id in context.Forms.Data("GridCheckedItems").Split(','))
            {
                var itemModel = new ItemModel(
                    context: context,
                    referenceId: id.ToLong());
                itemModel.Delete(context: context);
            }
        }
    }
}
