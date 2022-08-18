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
    public class ItemsIndex
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            UserModel userModel,
            List<HtmlTest> htmlTests)
        {
            var siteId = Initializer.Sites.Get(title).SiteId;
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.ItemsIndex(id: siteId));
            var results = Results(context: context);
            Assert.True(Compare.Html(
                context: context,
                results: results,
                htmlTests: htmlTests));
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
                new TestPart(title: "仕入")
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    userModel: testPart.UserModel,
                    htmlTests: HtmlData.Tests(HtmlData.ExistsOne(selector: "#Grid")));
            }
        }

        private static object[] TestData(
            string title,
            UserModel userModel,
            List<HtmlTest> htmlTests)
        {
            return new object[]
            {
                title,
                userModel,
                htmlTests
            };
        }

        private static string Results(Context context)
        {
            var itemModel = Initializer.ItemIds.Get(context.Id);
            return itemModel.Index(context: context);
        }
    }
}
