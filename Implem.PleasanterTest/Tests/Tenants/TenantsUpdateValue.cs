using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Operations;
using Implem.PleasanterTest.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Implem.PleasanterTest.Tests.Tenants
{
    public class TenantsUpdateValue : IDisposable
    {
        private static string timeZone = "Asia/Ho_Chi_Minh";
        private static string language = "vn";

        static TenantsUpdateValue()
        {
            if (!TimeZoneInfo.GetSystemTimeZones().Any(o => o.Id == timeZone))
            {
                timeZone = "SE Asia Standard Time";
            }
        }

        public TenantsUpdateValue()
        {
            TenantOperations.Update(
                userType: UserData.UserTypes.TenantManager1,
                forms: new Forms()
                {
                    ["Tenants_Language"] = language,
                    ["Tenants_TimeZone"] = timeZone
                });
        }

        public void Dispose()
        {
            TenantOperations.Update(
                userType: UserData.UserTypes.TenantManager1,
                forms: new Forms()
                {
                    ["Tenants_Language"] = "",
                    ["Tenants_TimeZone"] = ""
                });
        }

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
            Assert.True(Tester.Test(
                context: context,
                results: results,
                baseTests: baseTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var validHtmlTests = BaseData.Tests(
                HtmlData.ExistsOne(selector: "#Editor"),
                HtmlData.SelectedOption(selector: "#Tenants_Language", value: language),
                HtmlData.SelectedOption(selector: "#Tenants_TimeZone", value: timeZone));
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    userType: UserData.UserTypes.TenantManager1,
                    baseTests: validHtmlTests),
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
