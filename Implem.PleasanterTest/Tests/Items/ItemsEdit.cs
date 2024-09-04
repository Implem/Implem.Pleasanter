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
    public class ItemsEdit
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
                routeData: RouteData.ItemsEdit(id: id));
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
                new TestPart(title: "サーバのテスト"),
                new TestPart(title: "ネットワーク構成が決まっていない"),
                new TestPart(title: "ディスク容量の要件に誤り"),
                new TestPart(title: "株式会社プリザンター"),
                new TestPart(title: "業務改善コンサルティング"),
                new TestPart(title: "R社システム開発"),
                new TestPart(title: "Wiki1")
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                title: testPart.Title,
                userModel: testPart.UserModel,
                baseTests: BaseData.Tests(HtmlData.ExistsOne(selector: "#Editor")));
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
            return itemModel.Editor(context: context);
        }
    }
}
