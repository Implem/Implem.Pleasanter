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
    [Collection(nameof(DashboardPartLayout))]
    public class DashboardPartLayout
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            Dictionary<string, long> siteIdCollection,
            Forms forms,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            // セッションに「ViewMode」を設定
            var sessionData = new Dictionary<string, string>() { { "ViewMode", "{\"18826\":\"index\",\"18827\":\"index\"}" } };
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.ItemsIndex(id: siteIdCollection.Get("参照元(ダッシュボード)")),
                forms: forms,
                sessionData: sessionData);
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
                },
                {
                    "参照先(Wiki)", Initializer.Sites.Get("参照先(Wiki)").SiteId
                },
                {
                    "参照先(ダッシュボード)", Initializer.Sites.Get("参照先(ダッシュボード)").SiteId
                },
                {
                    "参照先(記録テーブル)", Initializer.Sites.Get("参照先(記録テーブル)").SiteId
                }
            };
            var testParts = new List<TestPart>()
            {
                // DashboardPartLayoutのテスト項目 1：操作＝id:1を拡大する。（Widthが4→10、Heightが8→14）
                // DashboardPartLayoutのテスト項目 2：操作＝1d:2とid:3を入れ替える。
                // 戻りのJSONに検査可能な内容は少ない。このため拡大や入れ替えが画面上反映されているかどうか返却された文字列から確認することはできない。
                new TestPart(
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "DashboardPartLayout"),
                        new KeyValue("Sites_Timestamp", "2025/4/1 10:00:00.000"),
                        new KeyValue("DashboardPartLayout","[{\"id\":16,\"x\":0,\"y\":0,\"w\":2,\"h\":10,\"content\":\"\"},{\"id\":15,\"x\":0,\"y\":10,\"w\":2,\"h\":10,\"content\":\"\"},{\"id\":14,\"x\":0,\"y\":20,\"w\":2,\"h\":10,\"content\":\"\"},{\"id\":13,\"x\":0,\"y\":30,\"w\":2,\"h\":10,\"content\":\"\"},{\"id\":12,\"x\":0,\"y\":40,\"w\":2,\"h\":10,\"content\":\"\"},{\"id\":11,\"x\":0,\"y\":50,\"w\":2,\"h\":10,\"content\":\"\"},{\"id\":10,\"x\":0,\"y\":60,\"w\":2,\"h\":10,\"content\":\"\"},{\"id\":9,\"x\":0,\"y\":70,\"w\":2,\"h\":10,\"content\":\"\"},{\"id\":8,\"x\":0,\"y\":80,\"w\":2,\"h\":10,\"content\":\"\"},{\"id\":7,\"x\":0,\"y\":90,\"w\":2,\"h\":10,\"content\":\"\"},{\"id\":6,\"x\":0,\"y\":100,\"w\":2,\"h\":10,\"content\":\"\"},{\"id\":5,\"x\":0,\"y\":110,\"w\":2,\"h\":10,\"content\":\"\"},{\"id\":4,\"x\":0,\"y\":120,\"w\":2,\"h\":10,\"content\":\"\"},{\"id\":3,\"x\":0,\"y\":130,\"w\":2,\"h\":10,\"content\":\"\"},{\"id\":2,\"x\":0,\"y\":140,\"w\":5,\"h\":10,\"content\":\"\"},{\"id\":1,\"x\":0,\"y\":150,\"w\":10,\"h\":14,\"content\":\"\"}]")),
                    baseTests: BaseData.Tests(JsonData.Value(
                        method: "Log",
                        value: ""))),
                new TestPart(
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "DashboardPartLayout"),
                        new KeyValue("Sites_Timestamp", "2025/4/1 10:00:00.000"),
                        new KeyValue("DashboardPartLayout","[{\"id\":16,\"x\":0,\"y\":0,\"w\":2,\"h\":10,\"content\":\"\"},{\"id\":15,\"x\":0,\"y\":10,\"w\":2,\"h\":10,\"content\":\"\"},{\"id\":14,\"x\":0,\"y\":20,\"w\":2,\"h\":10,\"content\":\"\"},{\"id\":13,\"x\":0,\"y\":30,\"w\":2,\"h\":10,\"content\":\"\"},{\"id\":12,\"x\":0,\"y\":40,\"w\":2,\"h\":10,\"content\":\"\"},{\"id\":11,\"x\":0,\"y\":50,\"w\":2,\"h\":10,\"content\":\"\"},{\"id\":10,\"x\":0,\"y\":60,\"w\":2,\"h\":10,\"content\":\"\"},{\"id\":9,\"x\":0,\"y\":70,\"w\":2,\"h\":10,\"content\":\"\"},{\"id\":8,\"x\":0,\"y\":80,\"w\":2,\"h\":10,\"content\":\"\"},{\"id\":7,\"x\":0,\"y\":90,\"w\":2,\"h\":10,\"content\":\"\"},{\"id\":6,\"x\":0,\"y\":100,\"w\":2,\"h\":10,\"content\":\"\"},{\"id\":5,\"x\":0,\"y\":110,\"w\":2,\"h\":10,\"content\":\"\"},{\"id\":4,\"x\":0,\"y\":120,\"w\":2,\"h\":10,\"content\":\"\"},{\"id\":2,\"x\":0,\"y\":140,\"w\":5,\"h\":10,\"content\":\"\"},{\"id\":3,\"x\":0,\"y\":130,\"w\":2,\"h\":10,\"content\":\"\"},{\"id\":1,\"x\":0,\"y\":150,\"w\":10,\"h\":14,\"content\":\"\"}]")),
                    baseTests: BaseData.Tests(JsonData.Value(
                        method: "Log",
                        value: "")))
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
            return DashboardUtilities.DashboardPartLayout(context: context, ss: ss);
        }
    }
}
