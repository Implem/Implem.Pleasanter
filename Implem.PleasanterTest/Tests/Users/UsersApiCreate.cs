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
            List<BaseTest> baseTests)
        {
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.UsersApiCreate(),
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
                    apiRequestBody: new
                    {
                        LoginId = Strings.NewGuid(),
                        Name = Strings.NewGuid(),
                        Password = Strings.NewGuid()
                    },
                    baseTests: BaseData.Tests(ApiJsonData.StatusCode(statusCode: 200)),
                    userType: UserData.UserTypes.TenantManager1),
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    apiRequestBody: testPart.ApiRequestBody,
                    userModel: testPart.UserModel,
                    baseTests: testPart.BaseTests);
            }
        }

        private static object[] TestData(
            string apiRequestBody,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            return new object[]
            {
                apiRequestBody,
                userModel,
                baseTests
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
