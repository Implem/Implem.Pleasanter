using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Tests.DashboardPart
{
    [Collection(nameof(QuickAccessLayout))]
    public class QuickAccessLayout
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
            var testParts = new List<TestPart>()
            {
                // No. 1：「タイトルを表示する：true」
                new TestPart(
                    title: "参照元(ダッシュボード)",
                    baseTests: BaseData.Tests(
                        // ダッシュボードパーツの確認
                        HtmlData.TextContains(selector: "#DashboardPartLayouts", value: "DashboardPart_1"),
                        // タイトルが表示されていることの確認
                        HtmlData.TextContains(selector: "#DashboardPartLayouts", value: "QuickAccessLayout(「タイトルを表示する：true」)"))),
                // No. 2：「Urlが指定されている」
                new TestPart(
                    title: "参照元(ダッシュボード)",
                    baseTests: BaseData.Tests(
                        // ダッシュボードパーツの確認
                        HtmlData.TextContains(selector: "#DashboardPartLayouts", value: "DashboardPart_1"),
                        // Urlが指定されていることの確認
                        HtmlData.TextContains(selector: "#DashboardPartLayouts", value: "https://pleasanter.org/ja/manual"))),
                // No. 3：「Urlが指定されていない」「参照先がサイト」「参照先がダッシュボード」「参照先がWiki」「参照先がテーブル」
                new TestPart(
                    title: "参照元(ダッシュボード)",
                    baseTests: BaseData.Tests(
                        // ダッシュボードパーツの確認
                        HtmlData.TextContains(selector: "#DashboardPartLayouts", value: "DashboardPart_2"),
                        // Urlが指定されていないことの確認
                        HtmlData.TextContains(selector: "#DashboardPartLayouts", value: "/items/0/index"),
                        // 参照先がサイトであることの確認
                        HtmlData.TextContains(selector: "#DashboardPartLayouts", value: "dashboard-part-nav-item dashboard-part-nav-folder")))
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
            itemModel.SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true);
            var ss = itemModel.Site.SiteSettings;
            return DashboardUtilities.Index(context: context, ss: ss);
        }
    }
}
