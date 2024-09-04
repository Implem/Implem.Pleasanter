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
    public class PublishesIndex
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            List<BaseTest> baseTests)
        {
            var id = Initializer.Sites.Get(title).SiteId;
            var context = ContextData.Get(
                userType: UserData.UserTypes.Anonymous,
                routeData: RouteData.PublishesIndex(id: id));
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
                new TestPart(title: "WBS")
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    baseTests: BaseData.Tests(HtmlData.ExistsOne(selector: "#Grid")));
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
            return itemModel.Index(context: context);
        }
    }
}
