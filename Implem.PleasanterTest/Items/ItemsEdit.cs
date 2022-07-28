using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Items
{
    public class ItemsEdit
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            UserModel userModel,
            List<HtmlTest> htmlTests)
        {
            var id = Initializer.Titles.Get(title);
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.ItemsEdit(id: id));
            var html = GetHtml(context: context);
            Assert.True(Compare.Html(
                context: context,
                html: html,
                htmlTests: htmlTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var titles = new List<string>()
            {
                "サーバのテスト",
                "ネットワーク構成が決まっていない",
                "ディスク容量の要件に誤り",
                "株式会社プリザンター",
                "業務改善コンサルティング",
                "R社システム開発",
                "Wiki"
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
                            Selector = "#Editor"
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

        private static string GetHtml(Context context)
        {
            var itemModel = Initializer.ItemIds.Get(context.Id);
            return itemModel.Editor(context: context);
        }
    }
}
