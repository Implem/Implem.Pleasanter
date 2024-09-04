using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Tests.Publishes
{
    public class PublishesEdit
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            List<BaseTest> baseTests)
        {
            var id = Initializer.Titles.Get(title);
            var context = ContextData.Get(
                userType: UserData.UserTypes.Anonymous,
                routeData: RouteData.PublishesEdit(id: id));
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
                new TestPart(title: "サーバのテスト")
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    baseTests: BaseData.Tests(HtmlData.ExistsOne(selector: "#Editor")));
            }
        }

        private static object[] TestData(
            string title,
            List<BaseTest> baseTests)
        {
            return new object[]
            {
                title,
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
