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
    public class ItemsTrashBox
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            UserModel userModel,
            List<HtmlTest> htmlTests)
        {
            var siteId = title == "TopTraxhBox"
                ? 0
                : Initializer.Sites.Get(title).SiteId;
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.ItemsTrashBox(siteId: siteId));
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
                "TopTraxhBox",
                "プロジェクト管理の例",
                "WBS",
                "課題管理",
                "レビュー記録",
                "商談管理の例",
                "顧客マスタ",
                "商談",
                "仕入"
            };
            foreach (var title in titles)
            {
                yield return TestData(
                    title: title,
                    userModel: UserData.Get(userType: UserData.UserTypes.TenantManager1),
                    htmlTests: title == "TopTraxhBox"
                        ? new List<HtmlTest>
                        {
                            new HtmlTest()
                            {
                                Type = HtmlTest.Types.NotFoundMessage
                            }
                        }
                        : new List<HtmlTest>
                        {
                            new HtmlTest()
                            {
                                Type = HtmlTest.Types.ExistsOne,
                                Selector = "#Grid"
                            }
                        });
                yield return TestData(
                    title: title,
                    userModel: UserData.Get(userType: UserData.UserTypes.General1),
                    htmlTests: new List<HtmlTest>
                    {
                        new HtmlTest()
                        {
                            Type = HtmlTest.Types.NotFoundMessage,
                        }
                    });
                yield return TestData(
                    title: title,
                    userModel: UserData.Get(userType: UserData.UserTypes.Privileged),
                    htmlTests: new List<HtmlTest>
                    {
                        new HtmlTest()
                        {
                                Type = HtmlTest.Types.ExistsOne,
                                Selector = "#Grid"
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
            var itemModel = context.Id == 0
                ? new ItemModel()
                : Initializer.ItemIds.Get(context.Id);
            return itemModel.TrashBox(context: context);
        }
    }
}