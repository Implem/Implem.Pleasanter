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
    public class ItemsTrashBoxGridRows
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            var siteId = Initializer.Sites.Get(title)?.SiteId ?? 0;
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.ItemsTrashBoxGridRows(id: siteId));
            var results = Results(context: context);
            Initializer.SaveResults(results);
            Assert.True(Tester.Test(
                context: context,
                results: results,
                baseTests: baseTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var validJsonTests = new List<BaseTest>()
            {
                JsonData.ExistsOne(method: "Log"),
                JsonData.ExistsOne(
                    method: "Remove",
                    target: ".grid tr"),
                JsonData.ExistsOne(
                    method: "ClearFormData",
                    target: "GridOffset"),
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
                    target: "#Grid")
            };
            var testParts = new List<TestPart>()
            {
                new TestPart(title: "TopTraxhBox"),
                new TestPart(title: "プロジェクト管理の例"),
                new TestPart(title: "WBS"),
                new TestPart(title: "課題管理"),
                new TestPart(title: "レビュー記録"),
                new TestPart(title: "商談管理の例"),
                new TestPart(title: "顧客マスタ"),
                new TestPart(title: "商談"),
                new TestPart(title: "仕入")
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    userModel: UserData.Get(userType: UserData.UserTypes.TenantManager1),
                    baseTests: testPart.Title == "TopTraxhBox"
                        ? BaseData.Tests(JsonData.Message(message: "NotFound"))
                        : validJsonTests);
                yield return TestData(
                    title: testPart.Title,
                    userModel: UserData.Get(userType: UserData.UserTypes.General1),
                    baseTests: BaseData.Tests(JsonData.Message(message: "NotFound")));
                yield return TestData(
                    title: testPart.Title,
                    userModel: UserData.Get(userType: UserData.UserTypes.Privileged),
                    baseTests: validJsonTests);
            }
        }

        private static object[] TestData(
            string title,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            return new object[]
            {
                title,
                userModel,
                baseTests
            };
        }

        private static string Results(Context context)
        {
            var itemModel = Initializer.ItemIds.Get(context.Id)
                ?? new ItemModel();
            return itemModel.TrashBoxGridRows(context: context);
        }
    }
}
