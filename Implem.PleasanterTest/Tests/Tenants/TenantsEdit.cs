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
    [Collection(nameof(TenantsEdit))]
    public class TenantsEdit
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.TenantsEdit());
            var results = Results(context: context);
            Initializer.SaveResults(results);
            Assert.True(Tester.Test(
                context: context,
                results: results,
                baseTests: baseTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var validHtmlTests = BaseData.Tests(HtmlData.ExistsOne(selector: "#Editor"));
            var notFoundMessage = BaseData.Tests(HtmlData.NotFoundMessage());
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    userType: UserData.UserTypes.TenantManager1,
                    baseTests: validHtmlTests),
                new TestPart(
                    userType: UserData.UserTypes.Privileged,
                    baseTests: validHtmlTests),
                new TestPart(
                    userType: UserData.UserTypes.General1,
                    baseTests: notFoundMessage)
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    userModel: testPart.UserModel,
                    baseTests: testPart.BaseTests);
            }
        }

        private static object[] TestData(
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            return new object[]
            {
                userModel,
                baseTests
            };
        }

        private static string Results(Context context)
        {
            return TenantUtilities.Editor(
                context: context,
                ss: SiteSettingsUtilities.TenantsSiteSettings(context: context),
                tenantId: context.TenantId,
                clearSessions: true);
        }
    }
}
