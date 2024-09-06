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
    [Collection(nameof(PublishesIndexJson))]
    public class PublishesIndexJson
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            List<BaseTest> baseTests)
        {
            var siteId = Initializer.Sites.Get(title).SiteId;
            var context = ContextData.Get(
                userType: UserData.UserTypes.Anonymous,
                httpMethod: "POST",
                routeData: RouteData.PublishesIndex(id: siteId));
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
                    baseTests: BaseData.Tests(
                        JsonData.Html(
                            target: "#ViewModeContainer",
                            selector: "#Grid")));
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
            return itemModel.IndexJson(context: context);
        }
    }
}
