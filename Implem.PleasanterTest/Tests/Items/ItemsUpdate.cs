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
            List<BaseTest> baseTests)
        {
            var id = Initializer.Titles.Get(title);
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.ItemsUpdate(id: id));
            var results = Results(context: context);
            Initializer.SaveResults(results);
            Assert.True(Tester.Test(
                context: context,
                results: results,
                baseTests: baseTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var siteJsonTests = new List<BaseTest>()
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
            var tableJsonTests = new List<BaseTest>()
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
            var wikiJsonTests = new List<BaseTest>()
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
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "プロジェクト管理の例",
                    baseTests: siteJsonTests,
                    userType: UserData.UserTypes.TenantManager1),
                new TestPart(
                    title: "WBS",
                    baseTests: siteJsonTests,
                    userType: UserData.UserTypes.TenantManager1),
                new TestPart(
                    title: "課題管理",
                    baseTests: siteJsonTests,
                    userType: UserData.UserTypes.TenantManager1),
                new TestPart(
                    title: "レビュー記録",
                    baseTests: siteJsonTests,
                    userType: UserData.UserTypes.TenantManager1),
                new TestPart(
                    title: "商談管理の例",
                    baseTests: siteJsonTests,
                    userType: UserData.UserTypes.TenantManager1),
                new TestPart(
                    title: "顧客マスタ",
                    baseTests: siteJsonTests,
                    userType: UserData.UserTypes.TenantManager1),
                new TestPart(
                    title: "商談",
                    baseTests: siteJsonTests,
                    userType: UserData.UserTypes.TenantManager1),
                new TestPart(
                    title: "仕入",
                    baseTests: siteJsonTests,
                    userType: UserData.UserTypes.TenantManager1),
                new TestPart(
                    title: "サーバのテスト",
                    baseTests: tableJsonTests,
                    userType: UserData.UserTypes.TenantManager1),
                new TestPart(
                    title: "ネットワーク構成が決まっていない",
                    baseTests: tableJsonTests,
                    userType: UserData.UserTypes.TenantManager1),
                new TestPart(
                    title: "ディスク容量の要件に誤り",
                    baseTests: tableJsonTests,
                    userType: UserData.UserTypes.TenantManager1),
                new TestPart(
                    title: "株式会社プリザンター",
                    baseTests: tableJsonTests,
                    userType: UserData.UserTypes.TenantManager1),
                new TestPart(
                    title: "業務改善コンサルティング",
                    baseTests: tableJsonTests,
                    userType: UserData.UserTypes.TenantManager1),
                new TestPart(
                    title: "R社システム開発",
                    baseTests: tableJsonTests,
                    userType: UserData.UserTypes.TenantManager1),
                new TestPart(
                    title: "Wiki1",
                    baseTests: wikiJsonTests,
                    userType: UserData.UserTypes.TenantManager1),
                new TestPart(title: "ダッシュボード1",
                    baseTests: siteJsonTests,
                    userType: UserData.UserTypes.TenantManager1)
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
            return itemModel.Update(context: context);
        }
    }
}
