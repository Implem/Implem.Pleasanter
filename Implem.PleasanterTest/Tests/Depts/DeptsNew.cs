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
    public class DeptsNew
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            UserModel userModel,
            List<HtmlTest> htmlTests)
        {
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.DeptsNew());
            var html = Results(context: context);
            Assert.True(Compare.Html(
                context: context,
                html: html,
                htmlTests: htmlTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            yield return TestData(
                userModel: UserData.Get(userType: UserData.UserTypes.TenantManager1),
                htmlTests: HtmlData.ExistsOne(selector: "#Editor").ToSingleList());
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
            return DeptUtilities.EditorNew(
                context: context,
                ss: SiteSettingsUtilities.DeptsSiteSettings(context: context));
        }
    }
}
