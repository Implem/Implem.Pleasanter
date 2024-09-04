using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Tests.Depts
{
    public class DeptsIndex
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.DeptsIndex());
            var results = Results(context: context);
            Initializer.SaveResults(results);
			Assert.True(Tester.Test(
                context: context,
                results: results,
                baseTests: baseTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var validHtmlTests = BaseData.Tests(HtmlData.ExistsOne(selector: "#Grid"));
            var hasNotPermissionMessageHtmlTests = BaseData.Tests(HtmlData.HasNotPermissionMessage());
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    baseTests: validHtmlTests,
                    userType: UserData.UserTypes.TenantManager1),
                new TestPart(
                    baseTests: validHtmlTests,
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    baseTests: hasNotPermissionMessageHtmlTests,
                    userType: UserData.UserTypes.General1)
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
            return DeptUtilities.Index(
                context: context,
                ss: SiteSettingsUtilities.DeptsSiteSettings(context: context));
        }
    }
}
