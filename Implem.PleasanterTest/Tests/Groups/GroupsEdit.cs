using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Implem.PleasanterTest.Tests.Groups
{
    public class GroupsEdit
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            UserModel userModel,
            List<HtmlTest> htmlTests)
        {
            var id = Initializer.Groups.Values.FirstOrDefault(o => o.GroupName == title).GroupId;
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.GroupsEdit(id: id));
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
            var notFoundMessage = HtmlData.NotFoundMessage().ToSingleList();
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "グループ2",
                    userType: UserData.UserTypes.TenantManager1,
                    htmlTests: validHtmlTests),
                new TestPart(
                    title: "グループ2",
                    userType: UserData.UserTypes.Privileged,
                    htmlTests: validHtmlTests),
                new TestPart(
                    title: "グループ2",
                    userType: UserData.UserTypes.General1,
                    htmlTests: validHtmlTests),
                new TestPart(
                    title: "グループ1",
                    userType: UserData.UserTypes.General1,
                    htmlTests: notFoundMessage)
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
            return GroupUtilities.Editor(
                context: context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(context: context),
                groupId: id,
                clearSessions: true);
        }
    }
}
