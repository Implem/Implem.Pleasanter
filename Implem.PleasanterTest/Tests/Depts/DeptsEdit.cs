using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Implem.PleasanterTest.Tests.Depts
{
    public class DeptsEdit
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            UserModel userModel,
            List<HtmlTest> htmlTests)
        {
            var id = Initializer.Depts.Values.FirstOrDefault(o => o.DeptName == title).DeptId;
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.DeptsEdit(id: id));
            var results = Results(
                context: context,
                id: id);
            Assert.True(Compare.Html(
                context: context,
                results: results,
                htmlTests: htmlTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var validHtmlTests = HtmlData.ExistsOne(selector: "#Editor").ToSingleList();
            var notFoundHtmlTests = HtmlData.NotFoundMessage().ToSingleList();
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "会社",
                    htmlTests: validHtmlTests,
                    userType: UserData.UserTypes.TenantManager1),
                new TestPart(
                    title: "会社",
                    htmlTests: validHtmlTests,
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "会社",
                    htmlTests: notFoundHtmlTests,
                    userType: UserData.UserTypes.General1)
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    userModel: testPart.UserModel,
                    htmlTests: testPart.HtmlTests);
            }
        }

        private static object[] TestData(
            string title,
            UserModel userModel,
            List<HtmlTest> htmlTests)
        {
            return new object[]
            {
                title,
                userModel,
                htmlTests
            };
        }

        private static string Results(
            Context context,
            int id)
        {
            return DeptUtilities.Editor(
                context: context,
                ss: SiteSettingsUtilities.DeptsSiteSettings(context: context),
                deptId: id,
                clearSessions: true);
        }
    }
}
