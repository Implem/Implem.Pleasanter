using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
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

namespace Implem.PleasanterTest.Tests.Users
{
    [Collection(nameof(UsersNewValue))]
    public class UsersNewValue
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            UserModel userModel,
            List<BaseTest> baseTests,
            Dictionary<string, string> extendedParams)
        {
            try
            {
                Parameters.Service.DefaultLanguage = "en";
                TenantOperations.Update(
                    userType: UserData.UserTypes.TenantManager1,
                    forms: new Forms()
                    {
                        ["Tenants_Language"] = extendedParams["Tenants_Language"],
                        ["Tenants_TimeZone"] = extendedParams["Tenants_TimeZone"]
                    });
                var context = ContextData.Get(
                    userId: userModel.UserId,
                    routeData: RouteData.UsersNew());
                var results = Results(context: context);
                Utilities.Initializer.SaveResults(results);
                Assert.True(Tester.Test(
                        context: context,
                        results: results,
                        baseTests: baseTests));
            }
            finally
            {
                TenantOperations.Update(
                    userType: UserData.UserTypes.TenantManager1,
                    forms: new Forms()
                    {
                        ["Tenants_Language"] = "",
                        ["Tenants_TimeZone"] = ""
                    });
                Parameters.Service.DefaultLanguage = "ja";
            }
        }

        public static IEnumerable<object[]> GetData()
        {
            var tenantsInitializeHtmlTests = BaseData.Tests(
                HtmlData.ExistsOne(selector: "#Editor"),
                HtmlData.SelectedOption(selector: "#Users_Language", value: "vn"),
                HtmlData.SelectedOption(selector: "#Users_TimeZone", value: "SE Asia Standard Time"));
            var serviceInitializeHtmlTests = BaseData.Tests(
                HtmlData.ExistsOne(selector: "#Editor"),
                HtmlData.SelectedOption(selector: "#Users_Language", value: "en"),
                HtmlData.SelectedOption(selector: "#Users_TimeZone", value: "UTC"));
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    baseTests: tenantsInitializeHtmlTests,
                    userType: UserData.UserTypes.TenantManager1,
                    extendedParams: new Dictionary<string, string>()
                    {
                        { "Tenants_Language", "vn" },
                        { "Tenants_TimeZone", "SE Asia Standard Time"},
                    }),
                new TestPart(
                    baseTests: serviceInitializeHtmlTests,
                    userType: UserData.UserTypes.TenantManager1,
                    extendedParams: new Dictionary<string, string>()
                    {
                        { "Tenants_Language", "" },
                        { "Tenants_TimeZone", ""},
                    })
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    userModel: testPart.UserModel,
                    baseTests: testPart.BaseTests,
                    extendedParams: testPart.ExtendedParams);
            }
        }

        private static object[] TestData(
            UserModel userModel,
            List<BaseTest> baseTests,
            Dictionary<string, string> extendedParams)
        {
            return new object[]
            {
                userModel,
                baseTests,
                extendedParams
            };
        }

        private static string Results(Context context)
        {
            return UserUtilities.EditorNew(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context));
        }
    }
}
