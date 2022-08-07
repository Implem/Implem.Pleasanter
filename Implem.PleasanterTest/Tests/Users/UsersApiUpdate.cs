using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Implem.PleasanterTest.Tests.Users
{
    public class UsersApiUpdate
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            UserModel userModel,
            List<ApiJsonTest> apiJsonTests)
        {
            var id = Initializer.Users.Values.FirstOrDefault(o => o.Name == title).UserId;
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.UsersApiUpdate(id: id));
            var results = GetResults(context: context);
            Assert.True(Compare.ApiResults(
                context: context,
                results: results,
                apiJsonTests: apiJsonTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "加藤 誠",
                    apiJsonTests: ApiJsonData.StatusCode(statusCode: 200).ToSingleList(),
                    userType: UserData.UserTypes.TenantManager1),
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    userModel: testPart.UserModel,
                    apiJsonTests: testPart.ApiJsonTests);
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
            return UserUtilities.UpdateByApi(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context),
                userId: context.Id.ToInt());
        }
    }
}
