using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Items
{
    public class ItemsTrashBoxJson
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            UserModel userModel,
            List<JsonTest> jsonTests)
        {
            var siteId = title == "TopTraxhBox"
                ? 0
                : Initializer.Sites.Get(title).SiteId;
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.ItemsTrashBox(siteId: siteId));
            var json = GetJson(context: context);
            Assert.True(Compare.Json(
                context: context,
                json: json,
                jsonTests: jsonTests));
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
                    userModel: UserData.Get(userType: UserData.UserTypes.TenantManager),
                    jsonTests: title == "TopTraxhBox"
                        ? new List<JsonTest>
                        {
                            new JsonTest()
                            {
                                Type = JsonTest.Types.Message,
                                Method = "Message",
                                Value = "NotFound"
                            }
                        }
                        : new List<JsonTest>
                        {
                            new JsonTest()
                            {
                                Type = JsonTest.Types.Html,
                                Method = "Html",
                                Target = "#ViewModeContainer",
                                HtmlTests = new List<HtmlTest>()
                                {
                                    new HtmlTest()
                                    {
                                        Type = HtmlTest.Types.ExistsOne,
                                        Selector = "#Grid"
                                    }
                                }
                            }
                        });
                yield return TestData(
                    title: title,
                    userModel: UserData.Get(userType: UserData.UserTypes.General1),
                    jsonTests: new List<JsonTest>
                    {
                        new JsonTest()
                        {
                            Type = JsonTest.Types.Message,
                            Method = "Message",
                            Value = "NotFound"
                        }
                    });
                yield return TestData(
                    title: title,
                    userModel: UserData.Get(userType: UserData.UserTypes.Privileged),
                    jsonTests: new List<JsonTest>
                    {
                        new JsonTest()
                        {
                            Type = JsonTest.Types.Html,
                            Method = "Html",
                            Target = "#ViewModeContainer",
                            HtmlTests = new List<HtmlTest>()
                            {
                                new HtmlTest()
                                {
                                    Type = HtmlTest.Types.ExistsOne,
                                    Selector = "#Grid"
                                }
                            }
                        }
                    });
            }
        }

        private static object[] TestData(
            string title,
            UserModel userModel,
            List<JsonTest> jsonTests)
        {
            return new object[]
            {
                title,
                userModel,
                jsonTests
            };
        }

        private static string GetJson(Context context)
        {
            var itemModel = context.Id == 0
                ? new ItemModel()
                : Initializer.ItemIds.Get(context.Id);
            return itemModel.TrashBoxJson(context: context);
        }
    }
}
