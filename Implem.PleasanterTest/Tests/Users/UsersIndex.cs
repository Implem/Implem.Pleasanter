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
    public class UsersIndex
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            UserModel userModel,
            List<HtmlTest> htmlTests)
        {
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.UsersIndex());
            var results = Results(context: context);
            Assert.True(Compare.Html(
                context: context,
                results: results,
                htmlTests: htmlTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var validHtmlTests = HtmlData.ExistsOne(selector: "#Grid").ToSingleList();
            var hasNotPermissionMessageHtmlTests = HtmlData.HasNotPermissionMessage().ToSingleList();
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    htmlTests: validHtmlTests,
                    userType: UserData.UserTypes.TenantManager1),
                new TestPart(
                    htmlTests: validHtmlTests,
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    htmlTests: hasNotPermissionMessageHtmlTests,
                    userType: UserData.UserTypes.General1)
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
            return UserUtilities.Index(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context));
        }
    }
}
