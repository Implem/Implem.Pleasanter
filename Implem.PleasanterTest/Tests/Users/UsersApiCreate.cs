using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Tests.Users
{
    public class UsersApiCreate
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string apiRequestBody,
            UserModel userModel,
            List<ApiJsonTest> apiJsonTests)
        {
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.UsersApiCreate(),
                apiRequestBody: apiRequestBody);
            var results = GetResults(context: context);
            Assert.True(Compare.ApiResults(
                context: context,
                results: results,
                apiJsonTests: apiJsonTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            yield return TestData(
                apiRequestBody: new
                {
                    LoginId = Strings.NewGuid(),
                    Name = Strings.NewGuid(),
                    Password = Strings.NewGuid()
                }
                    .ToJson(),
                userModel: UserData.Get(userType: UserData.UserTypes.TenantManager1),
                apiJsonTests: ApiJsonData.StatusCode(statusCode: 200).ToSingleList());
        }

        private static object[] TestData(
            string apiRequestBody,
            UserModel userModel,
            List<ApiJsonTest> apiJsonTests)
        {
            return new object[]
            {
                apiRequestBody,
                userModel,
                apiJsonTests
            };
        }

        private static ContentResultInheritance GetResults(Context context)
        {
            return UserUtilities.CreateByApi(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context));
        }
    }
}
