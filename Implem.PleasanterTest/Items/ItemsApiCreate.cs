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
    public class ItemsApiCreate
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            UserModel userModel,
            List<ApiJsonTest> apiJsonTests)
        {
            var id = Initializer.Titles.Get(title);
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.ItemsApiCreate(id: id));
            var results = GetResults(context: context);
            Assert.True(Compare.ApiResults(
                context: context,
                results: results,
                apiJsonTests: apiJsonTests));
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
                    apiJsonTests: ApiJsonData.StatusCode(statusCode: 200).ToSingleList());
            }
        }

        private static object[] TestData(
            string title,
            UserModel userModel,
            List<ApiJsonTest> apiJsonTests)
        {
            return new object[]
            {
                title,
                userModel,
                apiJsonTests
            };
        }

        private static ContentResultInheritance GetResults(Context context)
        {
            var itemModel = Initializer.ItemIds.Get(context.Id);
            return itemModel.CreateByApi(context: context);
        }
    }
}
