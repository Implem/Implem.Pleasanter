using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Tests.Items
{
    public class ItemsHistory
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            Forms forms,
            UserModel userModel,
            List<JsonTest> jsonTests)
        {
            var id = Initializer.Titles.Get(title);
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.ItemsHistory(id: id),
                forms: forms);
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
                "プロジェクト管理の例",
                "サーバのテスト",
                "ネットワーク構成が決まっていない",
                "ディスク容量の要件に誤り",
                "商談管理の例",
                "株式会社プリザンター",
                "業務改善コンサルティング",
                "R社システム開発",
                "Wikis",
                "Wiki1"
            };
            foreach (var title in titles)
            {
                yield return TestData(
                    title: title,
                    forms: new Forms()
                    {
                        {
                            "Ver",
                            "1"
                        },
                        {
                            "Latest",
                            Latest(title: title).ToOneOrZeroString()
                        }
                    },
                    userModel: UserData.Get(userType: UserData.UserTypes.TenantManager1),
                    jsonTests: JsonData.ReplaceAll(
                        target: "#MainContainer",
                        selector: "#Editor").ToSingleList());
            }
        }

        private static object[] TestData(
            string title,
            Forms forms,
            UserModel userModel,
            List<JsonTest> jsonTests)
        {
            return new object[]
            {
                title,
                forms,
                userModel,
                jsonTests
            };
        }

        private static string Results(Context context)
        {
            var itemModel = Initializer.ItemIds.Get(context.Id);
            return itemModel.History(context: context);
        }

        private static bool Latest(string title)
        {
            var id = Initializer.Titles.Get(title);
            var ver = Rds.ExecuteScalar_int(
                context: Initializer.Context,
                statements: Rds.SelectItems(
                    column: Rds.ItemsColumn().Ver(),
                    where: Rds.ItemsWhere().ReferenceId(id)));
            return ver == 1;
        }
    }
}
