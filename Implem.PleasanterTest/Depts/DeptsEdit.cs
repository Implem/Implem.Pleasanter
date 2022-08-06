using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Implem.PleasanterTest.Depts
{
    public class DeptsEdit
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            int deptId,
            UserModel userModel,
            List<HtmlTest> htmlTests)
        {
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.DeptsEdit(deptId: deptId));
            var html = GetHtml(
                context: context,
                deptId: deptId);
            Assert.True(Compare.Html(
                context: context,
                html: html,
                htmlTests: htmlTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            yield return TestData(
                deptId: Initializer.Depts.Values.FirstOrDefault().DeptId,
                userModel: UserData.Get(userType: UserData.UserTypes.TenantManager1),
                htmlTests: new List<HtmlTest>
                {
                    new HtmlTest()
                    {
                        Type = HtmlTest.Types.ExistsOne,
                        Selector = "#Editor"
                    }
                });
            yield return TestData(
                deptId: Initializer.Depts.Values.FirstOrDefault().DeptId,
                userModel: UserData.Get(userType: UserData.UserTypes.Privileged),
                htmlTests: new List<HtmlTest>
                {
                    new HtmlTest()
                    {
                        Type = HtmlTest.Types.ExistsOne,
                        Selector = "#Editor"
                    }
                });
            yield return TestData(
                deptId: Initializer.Depts.Values.FirstOrDefault().DeptId,
                userModel: UserData.Get(userType: UserData.UserTypes.General1),
                htmlTests: new List<HtmlTest>
                {
                    new HtmlTest()
                    {
                        Type = HtmlTest.Types.NotFoundMessage,
                    }
                });
        }

        private static object[] TestData(
            int deptId,
            UserModel userModel,
            List<HtmlTest> htmlTests)
        {
            return new object[]
            {
                deptId,
                userModel,
                htmlTests
            };
        }

        private static string GetHtml(
            Context context,
            int deptId)
        {
            return DeptUtilities.Editor(
                context: context,
                ss: SiteSettingsUtilities.DeptsSiteSettings(context: context),
                deptId: deptId,
                clearSessions: true);
        }
    }
}
