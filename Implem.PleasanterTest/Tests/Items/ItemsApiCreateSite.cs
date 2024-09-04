using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;
using static Implem.PleasanterTest.Utilities.ItemData;

namespace Implem.PleasanterTest.Tests.Items
{
    public class ItemsApiCreateSite
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            UserModel userModel,
            string apiRequestBody,
            List<BaseTest> baseTests)
        {
            var id = Initializer.Titles.Get(title);
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.ItemsApiCreateSite(id: id),
                apiRequestBody: apiRequestBody);
            var results = GetResults(context: context);
            Initializer.SaveResults(results.Content);
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
                    title: "ApiCreateSite",
                    apiRequestBody: new
                    {
                        ReferenceType="Wikis",
                        Title="CreatedWikiByApiCreateSite",
                        SiteSettings=new
                        {
                            Version=1.017,
                            ReferenceType="Wikis"
                        }
                    },
                    userType: UserData.UserTypes.TenantManager1)
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    userModel: testPart.UserModel,
                    apiRequestBody: testPart.ApiRequestBody,
                    baseTests: BaseData.Tests(ApiJsonData.StatusCode(statusCode: 200)));
            }
        }

        private static object[] TestData(
            string title,
            UserModel userModel,
            string apiRequestBody,
            List<BaseTest> baseTests)
        {
            return new object[]
            {
                title,
                userModel,
                apiRequestBody,
                baseTests
            };
        }

        private static ContentResultInheritance GetResults(Context context)
        {
            var itemModel = Initializer.ItemIds.Get(context.Id) ?? new ItemModel();
            return itemModel.CreateByApi(context: context);
        }
    }
}
