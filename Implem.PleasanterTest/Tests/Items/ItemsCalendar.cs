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
    public class ItemsCalendar
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
                routeData: RouteData.ItemsCalendar(siteId: siteId));
            var html = Results(context: context);
            Assert.True(Compare.Html(
                context: context,
                html: html,
                htmlTests: htmlTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var titles = new List<string>()
            {
                "WBS",
                "課題管理",
                "レビュー記録",
                "顧客マスタ",
                "商談",
                "仕入"
            };
            foreach (var title in titles)
            {
                yield return TestData(
                    title: title,
                    userModel: UserData.Get(userType: UserData.UserTypes.General1),
                    htmlTests: new List<HtmlTest>
                    {
                        new HtmlTest()
                        {
                                Type = HtmlTest.Types.ExistsOne,
                                Selector = "#Calendar"
                        }
                    });
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
            return itemModel.Calendar(context: context);
        }
    }
}