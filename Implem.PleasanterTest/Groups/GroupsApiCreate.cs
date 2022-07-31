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

namespace Implem.PleasanterTest.Groups
{
    public class GroupsApiCreate
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            UserModel userModel,
            List<ApiJsonTest> apiJsonTests)
        {
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.GroupsApiCreate());
            var results = GetResults(context: context);
            Assert.True(Compare.ApiResults(
                context: context,
                results: results,
                apiJsonTests: apiJsonTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            yield return TestData(
                userModel: UserData.Get(userType: UserData.UserTypes.TenantManager1),
                apiJsonTests: ApiJsonData.StatusCode(statusCode: 200).ToSingleList());
        }

        private static object[] TestData(
            UserModel userModel,
            List<ApiJsonTest> apiJsonTests)
        {
            return new object[]
            {
                userModel,
                apiJsonTests
            };
        }

        private static ContentResultInheritance GetResults(Context context)
        {
            return GroupUtilities.CreateByApi(
                context: context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(context: context));
        }
    }
}
