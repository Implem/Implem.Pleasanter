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
    public class ItemsEditJson
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
                routeData: RouteData.ItemsEdit(id: id));
            var json = Results(context: context);
            Assert.True(Compare.Json(
                context: context,
                json: json,
                jsonTests: jsonTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var titles = new List<string>()
            {
                "サーバのテスト",
                "ネットワーク構成が決まっていない",
                "ディスク容量の要件に誤り",
                "株式会社プリザンター",
                "業務改善コンサルティング",
                "R社システム開発",
                "Wiki1"
            };
            foreach (var title in titles)
            {
                yield return TestData(
                    title: title,
                    userModel: UserData.Get(userType: UserData.UserTypes.General1),
                    jsonTests: JsonData.ReplaceAll(
                        target: "#MainContainer",
                        selector: "#Editor").ToSingleList());
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
            return itemModel.EditorJson(context: context);
        }
    }
}
