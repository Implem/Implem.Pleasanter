using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Tests.Users
{
    public class UsersCreate
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            Forms forms,
            UserModel userModel,
            List<JsonTest> jsonTests)
        {
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.UsersCreate(),
                forms: forms);
            var json = Results(context: context);
            Assert.True(Compare.Json(
                context: context,
                json: json,
                jsonTests: jsonTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var jsonTests = new List<JsonTest>()
            {
                JsonData.ExistsOne(
                    method: "Response",
                    target: "id"),
                JsonData.ExistsOne(
                    method: "SetMemory",
                    target: "formChanged"),
                JsonData.ExistsOne(method: "Href")
            };
            yield return TestData(
                forms: FormsUtilities.Get(
                    new KeyValue("Users_LoginId", Strings.NewGuid()),
                    new KeyValue("Users_Password", "password")),
                userModel: UserData.Get(userType: UserData.UserTypes.TenantManager2),
                jsonTests: jsonTests);
        }

        private static object[] TestData(
            Forms forms,
            UserModel userModel,
            List<JsonTest> jsonTests)
        {
            return new object[]
            {
                forms,
                userModel,
                jsonTests
            };
        }

        private static string Results(Context context)
        {
            return UserUtilities.Create(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context));
        }
    }
}
