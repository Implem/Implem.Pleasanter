using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Depts
{
    public class DeptsIndex
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            UserModel userModel,
            List<HtmlTest> htmlTests)
        {
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.DeptsIndex());
            var html = GetHtml(context: context);
            Assert.True(Compare.Html(
                context: context,
                html: html,
                htmlTests: htmlTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            yield return TestData(
                userModel: UserData.Get(userType: UserData.UserTypes.TenantManager),
                htmlTests: new List<HtmlTest>
                {
                    new HtmlTest()
                    {
                        Type = HtmlTest.Types.Exists,
                        Selector = "#Grid"
                    }
                });
            yield return TestData(
                userModel: UserData.Get(userType: UserData.UserTypes.Privileged),
                htmlTests: new List<HtmlTest>
                {
                    new HtmlTest()
                    {
                        Type = HtmlTest.Types.Exists,
                        Selector = "#Grid"
                    }
                });
            yield return TestData(
                userModel: UserData.Get(userType: UserData.UserTypes.General1),
                htmlTests: new List<HtmlTest>
                {
                    new HtmlTest()
                    {
                        Type = HtmlTest.Types.HasNotPermissionMessage,
                    }
                });
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

        private static string GetHtml(Context context)
        {
            return DeptUtilities.Index(
                context: context,
                ss: SiteSettingsUtilities.DeptsSiteSettings(context: context));
        }
    }
}
