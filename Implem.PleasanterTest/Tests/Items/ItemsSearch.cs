using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Search;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Tests.Items
{
    [Collection(nameof(ItemsSearch))]
    public class ItemsSearch
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            QueryStrings queryStrings,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            var context = ContextData.Get(
                queryStrings: queryStrings,
                userId: userModel.UserId,
                routeData: RouteData.ItemsSearch());
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
                    queryStrings: QueryStringsUtilities.Get(new KeyValue("text", "WBS")),
                    baseTests: BaseData.Tests(HtmlData.ExistsOne(selector: "#SearchResults"))),
                new TestPart(
                    queryStrings: QueryStringsUtilities.Get(new KeyValue("text", "サーバ テスト")),
                    baseTests: BaseData.Tests(HtmlData.ExistsOne(selector: "#SearchResults"))),
                new TestPart(
                    queryStrings: QueryStringsUtilities.Get(new KeyValue("text", "ABCDEFG")),
                    baseTests: BaseData.Tests(HtmlData.ExistsOne(selector: "#SearchResults")))
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    queryStrings: testPart.QueryStrings,
                    userModel: testPart.UserModel,
                    baseTests: testPart.BaseTests);
            }
        }

        private static object[] TestData(
            QueryStrings queryStrings,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            return new object[]
            {
                queryStrings,
                userModel,
                baseTests
            };
        }

        private static string Results(Context context)
        {
            return Indexes.Search(context: context);
        }
    }
}