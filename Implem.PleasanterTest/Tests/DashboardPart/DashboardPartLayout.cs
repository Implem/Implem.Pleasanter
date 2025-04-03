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
            // 更新対象サイト設定
            var siteSettings = "{\"Version\":1.017,\"ReferenceType\":\"Dashboards\",\"DashboardParts\":[{\"Id\":1,\"Title\":\"QuickAccessLayout(「タイトルを表示する：true」)\",\"ShowTitle\":true,\"DisableAsynchronousLoading\":false,\"Type\":0,\"X\":0,\"Y\":10,\"Width\":4,\"Height\":8,\"QuickAccessSites\":\"[\\r\\n  {\\r\\n    \\\"Url\\\": \\\"https://pleasanter.org/ja/manual\\\",\\r\\n    \\\"Title\\\": \\\"ユーザマニュアル\\\",\\r\\n    \\\"OpenInNewTab\\\": false\\r\\n  }\\r\\n]\",\"TimeLineItemCount\":0,\"CalendarShowStatus\":false,\"KambanAggregationView\":false,\"KambanShowStatus\":false,\"SiteId\":0,\"ExtendedCss\":\"\"},{\"Id\":2,\"Title\":\"QuickAccessLayout(「Urlが指定されていない」「サイトID：0」「参照先がサイト/ダッシュボード/Wiki/テーブル」)\",\"DisableAsynchronousLoading\":false,\"Type\":0,\"X\":0,\"Y\":0,\"Width\":5,\"Height\":10,\"QuickAccessSites\":\"[\\n  {\\n    \\\"Id\\\": \\\"0\\\",\\n    \\\"Title\\\": \\\"トップ\\\",\\n    \\\"OpenInNewTab\\\": false\\n  },\\n  {\\\"Id\\\": \\\"" + siteIdCollection.Get("参照先(ダッシュボード)") + "\\\", \\\"Title\\\": \\\"ダッシュボード\\\", \\\"OpenInNewTab\\\": false},{\\\"Id\\\": \\\"" + siteIdCollection.Get("参照先(Wiki)") + "\\\", \\\"Title\\\": \\\"Wiki\\\", \\\"OpenInNewTab\\\": false},{\\\"Id\\\": \\\"" + siteIdCollection.Get("参照先(記録テーブル)") + "\\\", \\\"Title\\\": \\\"テーブル(記録テーブル)\\\", \\\"OpenInNewTab\\\": false}\\n]\",\"TimeLineItemCount\":0,\"CalendarShowStatus\":false,\"KambanAggregationView\":false,\"KambanShowStatus\":false,\"SiteId\":0,\"ExtendedCss\":\"\"},{\"Id\":3,\"Title\":\"TimeLineLayout(「タイトルを表示する：true」「参照先が期限付きテーブル」)\",\"ShowTitle\":true,\"DisableAsynchronousLoading\":false,\"Type\":1,\"X\":0,\"Y\":0,\"Width\":2,\"Height\":10,\"TimeLineSites\":\"18831\",\"View\":{\"Id\":0,\"GridColumns\":[\"TitleBody\",\"Comments\",\"Updator\",\"UpdatedTime\"],\"ApiColumnKeyDisplayType\":0,\"ApiColumnValueDisplayType\":0,\"CalendarSiteId\":0,\"CalendarTimePeriod\":\"Monthly\",\"CalendarFromTo\":\"StartTime-CompletionTime\",\"KambanGroupByX\":\"Status\",\"KambanGroupByY\":\"Owner\",\"KambanAggregateType\":\"Total\",\"KambanValue\":\"NumA\",\"ApiDataType\":0},\"TimeLineTitle\":\"[Title]\",\"TimeLineBody\":\"[Body]\",\"TimeLineItemCount\":20,\"CalendarShowStatus\":false,\"KambanAggregationView\":false,\"KambanShowStatus\":false,\"SiteId\":18831,\"ExtendedCss\":\"\"},{\"Id\":4,\"Title\":\"TimeLineLayout(「参照先が記録テーブル」)\",\"DisableAsynchronousLoading\":false,\"Type\":1,\"X\":0,\"Y\":0,\"Width\":2,\"Height\":10,\"TimeLineSites\":\"18828\",\"View\":{\"Id\":0,\"GridColumns\":[\"TitleBody\",\"Comments\",\"Updator\",\"UpdatedTime\"],\"ApiColumnKeyDisplayType\":0,\"ApiColumnValueDisplayType\":0,\"CalendarSiteId\":0,\"CalendarTimePeriod\":\"Monthly\",\"CalendarFromTo\":\"StartTime-CompletionTime\",\"KambanGroupByX\":\"Status\",\"KambanGroupByY\":\"Owner\",\"KambanAggregateType\":\"Total\",\"KambanValue\":\"NumA\",\"ApiDataType\":0},\"TimeLineTitle\":\"[Title]\",\"TimeLineBody\":\"[Body]\",\"TimeLineItemCount\":20,\"CalendarShowStatus\":false,\"KambanAggregationView\":false,\"KambanShowStatus\":false,\"SiteId\":18828,\"ExtendedCss\":\"\"},{\"Id\":5,\"Title\":\"TimeLineLayout(「参照先がそれ以外(未入力)」)\",\"DisableAsynchronousLoading\":false,\"Type\":1,\"X\":0,\"Y\":0,\"Width\":2,\"Height\":10,\"TimeLineSites\":\"\",\"View\":{\"Id\":0,\"ApiColumnKeyDisplayType\":0,\"ApiColumnValueDisplayType\":0,\"CalendarSiteId\":0,\"ApiDataType\":0},\"TimeLineTitle\":\"[Title]\",\"TimeLineBody\":\"[Body]\",\"TimeLineItemCount\":20,\"CalendarShowStatus\":false,\"KambanAggregationView\":false,\"KambanShowStatus\":false,\"SiteId\":0,\"ExtendedCss\":\"\"},{\"Id\":6,\"Title\":\"CustomLayout(「タイトルを表示する：true」)\",\"ShowTitle\":true,\"DisableAsynchronousLoading\":false,\"Type\":2,\"X\":0,\"Y\":0,\"Width\":2,\"Height\":10,\"TimeLineItemCount\":0,\"Content\":\"\",\"CalendarShowStatus\":false,\"KambanAggregationView\":false,\"KambanShowStatus\":false,\"SiteId\":0,\"ExtendedCss\":\"\"},{\"Id\":7,\"Title\":\"CustomHtmlLayout(「タイトルを表示する：true」)\",\"ShowTitle\":true,\"DisableAsynchronousLoading\":false,\"Type\":3,\"X\":0,\"Y\":0,\"Width\":2,\"Height\":10,\"TimeLineItemCount\":0,\"HtmlContent\":\"\",\"CalendarShowStatus\":false,\"KambanAggregationView\":false,\"KambanShowStatus\":false,\"SiteId\":0,\"ExtendedCss\":\"\"},{\"Id\":8,\"Title\":\"CalendarLayout(「タイトルを表示する：true」「参照先が期限付きテーブル」)\",\"ShowTitle\":true,\"DisableAsynchronousLoading\":false,\"Type\":4,\"X\":0,\"Y\":0,\"Width\":2,\"Height\":10,\"View\":{\"Id\":0,\"GridColumns\":[\"TitleBody\",\"Comments\",\"Updator\",\"UpdatedTime\"],\"ApiColumnKeyDisplayType\":0,\"ApiColumnValueDisplayType\":0,\"CalendarSiteId\":0,\"CalendarTimePeriod\":\"Monthly\",\"CalendarFromTo\":\"StartTime-CompletionTime\",\"KambanGroupByX\":\"Status\",\"KambanGroupByY\":\"Owner\",\"KambanAggregateType\":\"Total\",\"KambanValue\":\"NumA\",\"ApiDataType\":0},\"TimeLineItemCount\":0,\"CalendarType\":2,\"CalendarSites\":\"18831\",\"CalendarSitesData\":[\"18831\"],\"CalendarGroupBy\":\"\",\"CalendarTimePeriod\":\"Monthly\",\"CalendarFromTo\":\"StartTime-CompletionTime\",\"CalendarShowStatus\":false,\"KambanAggregationView\":false,\"KambanShowStatus\":false,\"SiteId\":18831,\"ExtendedCss\":\"\"},{\"Id\":9,\"Title\":\"CalendarLayout(「参照先が記録テーブル」)\",\"DisableAsynchronousLoading\":false,\"Type\":4,\"X\":0,\"Y\":0,\"Width\":2,\"Height\":10,\"View\":{\"Id\":0,\"GridColumns\":[\"TitleBody\",\"Comments\",\"Updator\",\"UpdatedTime\"],\"ApiColumnKeyDisplayType\":0,\"ApiColumnValueDisplayType\":0,\"CalendarSiteId\":0,\"CalendarTimePeriod\":\"Monthly\",\"CalendarFromTo\":\"UpdatedTime\",\"KambanGroupByX\":\"Status\",\"KambanGroupByY\":\"Owner\",\"KambanAggregateType\":\"Total\",\"KambanValue\":\"NumA\",\"ApiDataType\":0},\"TimeLineItemCount\":0,\"CalendarType\":2,\"CalendarSites\":\"18828\",\"CalendarSitesData\":[\"18828\"],\"CalendarGroupBy\":\"\",\"CalendarTimePeriod\":\"Monthly\",\"CalendarFromTo\":\"UpdatedTime\",\"CalendarShowStatus\":false,\"KambanAggregationView\":false,\"KambanShowStatus\":false,\"SiteId\":18828,\"ExtendedCss\":\"\"},{\"Id\":10,\"Title\":\"CalendarLayout(「参照先がそれ以外(未入力)」)\",\"DisableAsynchronousLoading\":false,\"Type\":4,\"X\":0,\"Y\":0,\"Width\":2,\"Height\":10,\"View\":{\"Id\":0,\"ApiColumnKeyDisplayType\":0,\"ApiColumnValueDisplayType\":0,\"CalendarSiteId\":0,\"ApiDataType\":0},\"TimeLineItemCount\":0,\"CalendarType\":2,\"CalendarSites\":\"\",\"CalendarSitesData\":[],\"CalendarGroupBy\":\"\",\"CalendarTimePeriod\":\"Monthly\",\"CalendarFromTo\":\"StartTime-CompletionTime\",\"CalendarShowStatus\":false,\"KambanAggregationView\":false,\"KambanShowStatus\":false,\"SiteId\":0,\"ExtendedCss\":\"\"},{\"Id\":11,\"Title\":\"KambamLayout(「タイトルを表示する：true」「参照先が期限付きテーブル」)\",\"ShowTitle\":true,\"DisableAsynchronousLoading\":false,\"Type\":5,\"X\":0,\"Y\":0,\"Width\":2,\"Height\":10,\"View\":{\"Id\":0,\"GridColumns\":[\"TitleBody\",\"Comments\",\"Updator\",\"UpdatedTime\"],\"ApiColumnKeyDisplayType\":0,\"ApiColumnValueDisplayType\":0,\"CalendarSiteId\":0,\"CalendarTimePeriod\":\"Monthly\",\"CalendarFromTo\":\"StartTime-CompletionTime\",\"KambanGroupByX\":\"Status\",\"KambanGroupByY\":\"Owner\",\"KambanAggregateType\":\"Total\",\"KambanValue\":\"RemainingWorkValue\",\"ApiDataType\":0},\"TimeLineItemCount\":0,\"CalendarShowStatus\":false,\"KambanSites\":\"18831\",\"KambanSitesData\":[\"18831\"],\"KambanGroupByX\":\"Status\",\"KambanGroupByY\":\"Owner\",\"KambanAggregateType\":\"Total\",\"KambanValue\":\"RemainingWorkValue\",\"KambanColumns\":\"10\",\"KambanAggregationView\":false,\"KambanShowStatus\":false,\"SiteId\":18831,\"ExtendedCss\":\"\"},{\"Id\":12,\"Title\":\"KambamLayout(「参照先が記録テーブル」)\",\"DisableAsynchronousLoading\":false,\"Type\":5,\"X\":0,\"Y\":0,\"Width\":2,\"Height\":10,\"View\":{\"Id\":0,\"GridColumns\":[\"TitleBody\",\"Comments\",\"Updator\",\"UpdatedTime\"],\"ApiColumnKeyDisplayType\":0,\"ApiColumnValueDisplayType\":0,\"CalendarSiteId\":0,\"CalendarTimePeriod\":\"Monthly\",\"CalendarFromTo\":\"StartTime-CompletionTime\",\"KambanGroupByX\":\"Status\",\"KambanGroupByY\":\"Owner\",\"KambanAggregateType\":\"Total\",\"KambanValue\":\"NumA\",\"ApiDataType\":0},\"TimeLineItemCount\":0,\"CalendarShowStatus\":false,\"KambanSites\":\"18828\",\"KambanSitesData\":[\"18828\"],\"KambanGroupByX\":\"Status\",\"KambanGroupByY\":\"Owner\",\"KambanAggregateType\":\"Total\",\"KambanValue\":\"NumA\",\"KambanColumns\":\"10\",\"KambanAggregationView\":false,\"KambanShowStatus\":false,\"SiteId\":18828,\"ExtendedCss\":\"\"},{\"Id\":13,\"Title\":\"KambamLayout(「参照先がそれ以外(未入力)」)\",\"DisableAsynchronousLoading\":false,\"Type\":5,\"X\":0,\"Y\":0,\"Width\":2,\"Height\":10,\"View\":{\"Id\":0,\"ApiColumnKeyDisplayType\":0,\"ApiColumnValueDisplayType\":0,\"CalendarSiteId\":0,\"ApiDataType\":0},\"TimeLineItemCount\":0,\"CalendarShowStatus\":false,\"KambanSites\":\"\",\"KambanSitesData\":[],\"KambanGroupByX\":\"Status\",\"KambanGroupByY\":\"Owner\",\"KambanAggregateType\":\"Total\",\"KambanValue\":\"NumA\",\"KambanColumns\":\"10\",\"KambanAggregationView\":false,\"KambanShowStatus\":false,\"SiteId\":0,\"ExtendedCss\":\"\"},{\"Id\":14,\"Title\":\"IndexLayout(「タイトルを表示する：true」「参照先が期限付きテーブル」)\",\"ShowTitle\":true,\"DisableAsynchronousLoading\":false,\"Type\":6,\"X\":0,\"Y\":0,\"Width\":2,\"Height\":10,\"View\":{\"Id\":0,\"ApiColumnKeyDisplayType\":0,\"ApiColumnValueDisplayType\":0,\"CalendarSiteId\":0,\"CalendarTimePeriod\":\"Monthly\",\"CalendarFromTo\":\"StartTime-CompletionTime\",\"KambanGroupByX\":\"Status\",\"KambanGroupByY\":\"Owner\",\"KambanAggregateType\":\"Total\",\"KambanValue\":\"NumA\",\"ApiDataType\":0},\"TimeLineItemCount\":0,\"CalendarShowStatus\":false,\"IndexSites\":\"18831\",\"IndexSitesData\":[\"18831\"],\"KambanAggregationView\":false,\"KambanShowStatus\":false,\"SiteId\":18831,\"ExtendedCss\":\"\"},{\"Id\":15,\"Title\":\"IndexLayout(「参照先が記録テーブル」)\",\"DisableAsynchronousLoading\":false,\"Type\":6,\"X\":0,\"Y\":0,\"Width\":2,\"Height\":10,\"View\":{\"Id\":0,\"ApiColumnKeyDisplayType\":0,\"ApiColumnValueDisplayType\":0,\"CalendarSiteId\":0,\"CalendarTimePeriod\":\"Monthly\",\"CalendarFromTo\":\"StartTime-CompletionTime\",\"KambanGroupByX\":\"Status\",\"KambanGroupByY\":\"Owner\",\"KambanAggregateType\":\"Total\",\"KambanValue\":\"NumA\",\"ApiDataType\":0},\"TimeLineItemCount\":0,\"CalendarShowStatus\":false,\"IndexSites\":\"18828\",\"IndexSitesData\":[\"18828\"],\"KambanAggregationView\":false,\"KambanShowStatus\":false,\"SiteId\":18828,\"ExtendedCss\":\"\"},{\"Id\":16,\"Title\":\"IndexLayout(「参照先がそれ以外(未入力)」)\",\"DisableAsynchronousLoading\":false,\"Type\":6,\"X\":0,\"Y\":0,\"Width\":2,\"Height\":10,\"View\":{\"Id\":0,\"ApiColumnKeyDisplayType\":0,\"ApiColumnValueDisplayType\":0,\"CalendarSiteId\":0,\"ApiDataType\":0},\"TimeLineItemCount\":0,\"CalendarShowStatus\":false,\"IndexSites\":\"\",\"IndexSitesData\":[],\"KambanAggregationView\":false,\"KambanShowStatus\":false,\"SiteId\":0,\"ExtendedCss\":\"\"}],\"NoDisplayIfReadOnly\":false,\"NotInheritPermissionsWhenCreatingSite\":false}";
            // セッションに更新対象のサイト設定と「ViewMode」を設定
            var sessionData = new Dictionary<string, string>() { { "SiteSettings", siteSettings }, { "ViewMode", "{\"18826\":\"index\",\"18827\":\"index\"}" } };
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.ItemsIndex(id: siteIdCollection.Get("参照元(ダッシュボード)")),
                forms: forms,
                sessionData: sessionData);
            switch (context.Forms.Get("ControlId"))
            {
                // サイト設定更新
                case "UpdateCommand":
                    new ItemModel(
                        context: context,
                        referenceId: siteIdCollection.Get("参照元(ダッシュボード)"))
                    .Update(context: context);
                    break;
                // 今回 xUnit
                default:
                    var results = Results(context: context, siteId: siteIdCollection.Get("参照元(ダッシュボード)"));
                    Initializer.SaveResults(results);
                    Assert.True(Tester.Test(
                        context: context,
                        results: results,
                        baseTests: baseTests));
                    break;
            }
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
                // 事前準備
                // 各ダッシュボードパーツのサイトIDを上書きするための TestPart
                new TestPart(
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "UpdateCommand"),
                        new KeyValue("DashboardPartId","2")),
                    userType: UserData.UserTypes.Privileged),
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
