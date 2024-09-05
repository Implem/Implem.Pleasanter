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
            List<BaseTest> baseTests)
        {
            var id = Initializer.Titles.Get(title);
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.ItemsHistory(id: id),
                forms: forms);
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
                new TestPart(
                    title: "プロジェクト管理の例",
                    userType: UserData.UserTypes.TenantManager1),
                new TestPart(
                    title: "サーバのテスト",
                    userType: UserData.UserTypes.TenantManager1),
                new TestPart(
                    title: "ネットワーク構成が決まっていない",
                    userType: UserData.UserTypes.TenantManager1),
                new TestPart(
                    title: "ディスク容量の要件に誤り",
                    userType: UserData.UserTypes.TenantManager1),
                new TestPart(
                    title: "商談管理の例",
                    userType: UserData.UserTypes.TenantManager1),
                new TestPart(
                    title: "株式会社プリザンター",
                    userType: UserData.UserTypes.TenantManager1),
                new TestPart(
                    title: "業務改善コンサルティング",
                    userType: UserData.UserTypes.TenantManager1),
                new TestPart(
                    title: "R社システム開発",
                    userType: UserData.UserTypes.TenantManager1),
                new TestPart(
                    title: "Wikis",
                    userType: UserData.UserTypes.TenantManager1),
                new TestPart(
                    title: "Wiki1",
                    userType: UserData.UserTypes.TenantManager1)
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    forms: FormsUtilities.Get(
                        new KeyValue("Ver", "1"),
                        new KeyValue("Latest", Latest(title: testPart.Title).ToOneOrZeroString())),
                    userModel: testPart.UserModel,
                    baseTests: BaseData.Tests(
                        JsonData.ReplaceAll(
                            target: "#MainContainer",
                            selector: "#Editor")));
            }
        }

        private static object[] TestData(
            string title,
            Forms forms,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            return new object[]
            {
                title,
                forms,
                userModel,
                baseTests
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
