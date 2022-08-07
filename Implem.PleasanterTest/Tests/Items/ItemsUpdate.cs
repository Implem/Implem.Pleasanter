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
    public class ItemsUpdate
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            UserModel userModel,
            List<JsonTest> jsonTests)
        {
            var id = Initializer.Titles.Get(title);
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.ItemsUpdate(id: id));
            var json = Results(context: context);
            Assert.True(Compare.Json(
                context: context,
                json: json,
                jsonTests: jsonTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var siteJsonTests = new List<JsonTest>()
            {
                JsonData.ExistsOne(
                    method: "ReplaceAll",
                    target: "#Breadcrumb"),
                JsonData.ExistsOne(
                    method: "ReplaceAll",
                    target: "#Warnings"),
                JsonData.ExistsOne(
                    method: "Html",
                    target: "#HeaderTitle"),
                JsonData.ExistsOne(
                    method: "Html",
                    target: "#RecordInfo"),
                JsonData.ExistsOne(
                    method: "SetMemory",
                    target: "formChanged"),
                JsonData.ExistsOne(method: "Message"),
                JsonData.ExistsOne(method: "ClearFormData")
            };
            var tableJsonTests = new List<JsonTest>()
            {
                JsonData.ExistsOne(method: "Log"),
                JsonData.ExistsOne(
                    method: "Response",
                    target: "id"),
                JsonData.ExistsOne(
                    method: "Invoke",
                    target: "clearDialogs"),
                JsonData.ExistsOne(
                    method: "ReplaceAll",
                    target: "#MainContainer"),
                JsonData.ExistsOne(
                    method: "SetMemory",
                    target: "formChanged"),
                JsonData.ExistsOne(
                    method: "Invoke",
                    target: "setCurrentIndex"),
                JsonData.ExistsOne(
                    method: "Invoke",
                    target: "initRelatingColumnEditor"),
                JsonData.ExistsOne(method: "Message"),
                JsonData.ExistsOne(method: "ClearFormData"),
                JsonData.ExistsOne(
                    method: "Events",
                    target: "on_editor_load")
            };
            var wikiJsonTests = new List<JsonTest>()
            {
                JsonData.ExistsOne(
                    method: "ReplaceAll",
                    target: "#Breadcrumb"),
                JsonData.ExistsOne(
                    method: "Html",
                    target: "#HeaderTitle"),
                JsonData.ExistsOne(
                    method: "Html",
                    target: "#RecordInfo"),
                JsonData.ExistsOne(
                    method: "SetMemory",
                    target: "formChanged"),
                JsonData.ExistsOne(method: "Message"),
                JsonData.ExistsOne(method: "ClearFormData")
            };
            var titles = new Dictionary<string, List<JsonTest>>()
            {
                { "プロジェクト管理の例", siteJsonTests },
                { "WBS", siteJsonTests },
                { "課題管理", siteJsonTests },
                { "レビュー記録", siteJsonTests },
                { "商談管理の例", siteJsonTests },
                { "顧客マスタ", siteJsonTests },
                { "商談", siteJsonTests },
                { "仕入", siteJsonTests },
                { "サーバのテスト", tableJsonTests },
                { "ネットワーク構成が決まっていない", tableJsonTests },
                { "ディスク容量の要件に誤り", tableJsonTests },
                { "株式会社プリザンター", tableJsonTests },
                { "業務改善コンサルティング", tableJsonTests },
                { "R社システム開発", tableJsonTests },
                { "Wiki1", wikiJsonTests }
            };
            foreach (var data in titles)
            {
                yield return TestData(
                    title: data.Key,
                    userModel: UserData.Get(userType: UserData.UserTypes.TenantManager1),
                    jsonTests: data.Value);
            }
        }

        private static object[] TestData(
            string title,
            UserModel userModel,
            List<JsonTest> jsonTests)
        {
            return new object[]
            {
                title,
                userModel,
                jsonTests
            };
        }

        private static string Results(Context context)
        {
            var itemModel = Initializer.ItemIds.Get(context.Id);
            return itemModel.Update(context: context);
        }
    }
}
