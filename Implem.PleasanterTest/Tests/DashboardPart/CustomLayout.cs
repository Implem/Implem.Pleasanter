using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Web.Mvc;
using Xunit;

namespace Implem.PleasanterTest.Tests.DashboardPart
{
    [Collection(nameof(QuickAccessLayout))]
    public class CustomLayout
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            Dictionary<string, long> siteIdCollection,
            Forms forms,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
             var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.ItemsIndex(id: siteIdCollection.Get("参照元(ダッシュボード)")),
                forms: forms);
             var results = Results(context: context, siteId: siteIdCollection.Get("参照元(ダッシュボード)"));
             Initializer.SaveResults(results);
             Assert.True(Tester.Test(
                context: context,
                results: results,
                baseTests: baseTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            // 各サイトIDを取得
            Dictionary<string, long> siteIdCollection = new Dictionary<string, long>()
            {
                {
                    "参照元(ダッシュボード)", Initializer.Sites.Get("参照元(ダッシュボード)").SiteId
                }
            };
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    baseTests: BaseData.Tests(
                        // ダッシュボードパーツの確認
                        HtmlData.TextContains(
                            selector: "#DashboardPartLayouts",
                            value: "DashboardPart_6"),
                        // タイトルが表示されていることの確認
                        HtmlData.TextContains(
                            selector: "#DashboardPartLayouts",
                            value: "QuickAccessLayout(「タイトルを表示する：true」)"))),
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    siteIdCollection: siteIdCollection,
                    forms: testPart.Forms,
                    userModel: testPart.UserModel,
                    baseTests: testPart.BaseTests);
            }
        }

        private static object[] TestData(
            Dictionary<string, long> siteIdCollection,
            Forms forms,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            return new object[]
            {
                siteIdCollection,
                forms,
                userModel,
                baseTests
            };
        }

        private static string Results(Context context, long siteId)
        {
            var itemModel = Initializer.ItemIds.Get(siteId);
            itemModel.SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true);
             var ss = itemModel.Site.SiteSettings;
            return DashboardUtilities.Index(context: context, ss: ss);
        }
    }
}
