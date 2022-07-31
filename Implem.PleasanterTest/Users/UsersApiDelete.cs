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

namespace Implem.PleasanterTest.Users
{
    public class UsersApiDelete
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
                routeData: RouteData.UsersApiDelete(id: id));
            var results = GetResults(context: context);
            Assert.True(Compare.ApiResults(
                context: context,
                results: results,
                apiJsonTests: apiJsonTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            yield return TestData(
                title: "ユーザ22（API削除用）",
                userModel: UserData.Get(userType: UserData.UserTypes.TenantManager1),
                apiJsonTests: ApiJsonData.StatusCode(statusCode: 200).ToSingleList());
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
            return UserUtilities.DeleteByApi(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context),
                userId: context.Id.ToInt());
        }
    }
}
