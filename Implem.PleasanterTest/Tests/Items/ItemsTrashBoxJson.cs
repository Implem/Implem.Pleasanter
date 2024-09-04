using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Tests.Items
{
    public class ItemsTrashBoxJson
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            var siteId = title == "TopTraxhBox"
                ? 0
                : Initializer.Sites.Get(title).SiteId;
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.ItemsTrashBox(id: siteId));
            var results = Results(context: context);
            Initializer.SaveResults(results);
			Assert.True(Tester.Test(
                context: context,
                results: results,
                baseTests: baseTests));
        }

        public static IEnumerable<object[]> GetData()
        {
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
                        : BaseData.Tests(
                            JsonData.Html(
                                target: "#ViewModeContainer",
                                selector: "#Grid")));
                yield return TestData(
                    title: testPart.Title,
                    userModel: UserData.Get(userType: UserData.UserTypes.General1),
                    baseTests: BaseData.Tests(JsonData.Message(message: "NotFound")));
                yield return TestData(
                    title: testPart.Title,
                    userModel: UserData.Get(userType: UserData.UserTypes.Privileged),
                    baseTests: BaseData.Tests(
                        JsonData.Html(
                            target: "#ViewModeContainer",
                            selector: "#Grid")));
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
            var itemModel = context.Id == 0
                ? new ItemModel()
                : Initializer.ItemIds.Get(context.Id);
            return itemModel.TrashBoxJson(context: context);
        }
    }
}
