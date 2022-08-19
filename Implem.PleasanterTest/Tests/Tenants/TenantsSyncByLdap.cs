using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Tests.Tenants
{
    public class TenantsSyncByLdap
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            UserModel userModel,
            List<JsonTest> jsonTests)
        {
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.TenantsSyncByLdap());
            var results = Results(context: context);
            Assert.True(Compare.Json(
                context: context,
                results: results,
                jsonTests: jsonTests));
        }
        public static IEnumerable<object[]> GetData()
        {
            var jsonTestsSuccess = JsonData.Tests(
                JsonData.Value(
                    method: "Message",
                    value: "{\"Id\":\"SyncByLdapExecuted\",\"Text\":\"LDAP同期を開始しました。\",\"Css\":\"alert-success\"}"));
            var jsonTestsNotPermitted = JsonData.Tests(
                JsonData.Value(
                    method: "Message",
                    value: "{\"Id\":\"HasNotPermission\",\"Text\":\"この操作を行うための権限がありません。\",\"Css\":\"alert-error\"}"));
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    userType: UserData.UserTypes.TenantManager1,
                    jsonTests: jsonTestsSuccess),
                new TestPart(
                    userType: UserData.UserTypes.General1,
                    jsonTests: jsonTestsNotPermitted)
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    userModel: testPart.UserModel,
                    jsonTests: testPart.JsonTests);
            }
        }

        private static object[] TestData(
            UserModel userModel,
            List<JsonTest> jsonTests)
        {
            return new object[]
            {
                userModel,
                jsonTests
            };
        }

        private static string Results(Context context)
        {
            return TenantUtilities.SyncByLdap(
                context: context,
                ss: SiteSettingsUtilities.TenantsSiteSettings(context: context));
        }
    }
}
