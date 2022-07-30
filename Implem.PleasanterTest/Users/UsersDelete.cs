using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Implem.PleasanterTest.Users
{
    public class UsersDelete
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            UserModel userModel,
            List<JsonTest> jsonTests)
        {
            var id = Initializer.Users.Values.FirstOrDefault(o => o.Name == title).UserId;
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.UsersDelete(id: id));
            var json = GetJson(context: context);
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
                    method: "SetMemory",
                    target: "formChanged"),
                JsonData.ExistsOne(
                    method: "Invoke",
                    target: "back")
            };
            yield return TestData(
                title: "ユーザ21（削除用）",
                userModel: UserData.Get(userType: UserData.UserTypes.TenantManager1),
                jsonTests: jsonTests);
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
            return UserUtilities.Delete(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context),
                userId: context.Id.ToInt());
        }
    }
}
