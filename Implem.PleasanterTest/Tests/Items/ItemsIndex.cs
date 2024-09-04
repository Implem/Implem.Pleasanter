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
    public class ItemsIndex
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            var siteId = Initializer.Sites.Get(title).SiteId;
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.ItemsIndex(id: siteId));
            var results = Results(context: context);
            Initializer.SaveResults(results);
			Assert.True(Tester.Test(
                context: context,
                results: results,
                baseTests: baseTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var existsGridTest = BaseData.Tests(HtmlData.ExistsOne(selector: "#Grid"));
            var testParts = new List<TestPart>()
            {
                new TestPart(title: "WBS", baseTests: existsGridTest),
                new TestPart(title: "課題管理", baseTests: existsGridTest),
                new TestPart(title: "レビュー記録", baseTests: existsGridTest),
                new TestPart(title: "顧客マスタ", baseTests: existsGridTest),
                new TestPart(title: "商談", baseTests: existsGridTest),
                new TestPart(title: "仕入", baseTests: existsGridTest),
                new TestPart(
                    title: "ダッシュボード1",
                    baseTests: BaseData.Tests(
                        HtmlData.TextContains(selector: "#DashboardPartLayouts", value: "DashboardPart_1"),
                        HtmlData.TextContains(selector: "#DashboardPartLayouts", value: "DashboardPart_2"),
                        HtmlData.TextContains(selector: "#DashboardPartLayouts", value: "DashboardPart_3"),
                        HtmlData.TextContains(selector: "#DashboardPartLayouts", value: "DashboardPart_4")))
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    userModel: testPart.UserModel,
                    baseTests: testPart.BaseTests);
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
            var itemModel = Initializer.ItemIds.Get(context.Id);
            return itemModel.Index(context: context);
        }
    }
}
