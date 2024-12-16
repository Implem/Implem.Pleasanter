using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Tests.Tenants
{
    [Collection(nameof(TenantsUpdate))]
    public class TenantsUpdate
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.TenantsUpdate());
            var results = Results(context: context);
            Initializer.SaveResults(results);
            Assert.True(Tester.Test(
                context: context,
                results: results,
                baseTests: baseTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var baseTests = new List<BaseTest>()
            {
                JsonData.ExistsOne(
                    method: "Html",
                    target: "#HeaderTitle"),
                JsonData.ExistsOne(
                    method: "Html",
                    target: "#RecordInfo"),
                JsonData.ExistsOne(
                    method: "SetMemory",
                    target: "formChanged"),
                JsonData.ExistsOne(method: "Message"),
                JsonData.ExistsOne(method: "ClearFormData")
            };
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    baseTests: baseTests,
                    userType: UserData.UserTypes.TenantManager1)
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
            return TenantUtilities.Update(
                context: context,
                ss: SiteSettingsUtilities.TenantsSiteSettings(context: context),
                tenantId: context.TenantId);
        }
    }
}
