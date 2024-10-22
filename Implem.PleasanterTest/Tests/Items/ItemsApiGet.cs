using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Tests.Items
{
    [Collection(nameof(ItemsApiGet))]
    public class ItemsApiGet
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
                routeData: RouteData.ItemsApiGet(id: id));
            var results = GetResults(context: context);
            Initializer.SaveResults(results.Content);
            Assert.True(Tester.Test(
                context: context,
                results: results,
                baseTests: baseTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var testParts = new List<TestPart>()
            {
                new TestPart(title: "WBS"),
                new TestPart(title: "課題管理"),
                new TestPart(title: "レビュー記録"),
                new TestPart(title: "顧客マスタ"),
                new TestPart(title: "商談"),
                new TestPart(title: "仕入"),
                new TestPart(title: "サーバのテスト"),
                new TestPart(title: "ネットワーク構成が決まっていない"),
                new TestPart(title: "ディスク容量の要件に誤り"),
                new TestPart(title: "株式会社プリザンター"),
                new TestPart(title: "業務改善コンサルティング"),
                new TestPart(title: "R社システム開発"),
                new TestPart(title: "ApiGetWiki")
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    userModel: testPart.UserModel,
                    baseTests: BaseData.Tests(ApiJsonData.StatusCode(statusCode: 200)));
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

        private static ContentResultInheritance GetResults(Context context)
        {
            var itemModel = Initializer.ItemIds.Get(context.Id);
            return itemModel.GetByApi(context: context);
        }
    }
}
