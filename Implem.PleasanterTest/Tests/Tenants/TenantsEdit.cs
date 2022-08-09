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
    public class TenantsEdit
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            UserModel userModel,
            List<HtmlTest> htmlTests)
        {
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.TenantsEdit());
            var results = Results(context: context);
            Assert.True(Compare.Html(
                context: context,
                results: results,
                htmlTests: htmlTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var validHtmlTests = HtmlData.ExistsOne(selector: "#Editor").ToSingleList();
            var notFoundMessage = HtmlData.NotFoundMessage().ToSingleList();
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    userType: UserData.UserTypes.TenantManager1,
                    htmlTests: validHtmlTests),
                new TestPart(
                    userType: UserData.UserTypes.Privileged,
                    htmlTests: validHtmlTests),
                new TestPart(
                    userType: UserData.UserTypes.General1,
                    htmlTests: notFoundMessage)
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    userModel: testPart.UserModel,
                    htmlTests: testPart.HtmlTests);
            }
        }

        private static object[] TestData(
            UserModel userModel,
            List<HtmlTest> htmlTests)
        {
            return new object[]
            {
                userModel,
                htmlTests
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
