using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Implem.PleasanterTest.Tests.Users
{
    public class UsersEdit
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            UserModel userModel,
            List<HtmlTest> htmlTests)
        {
            var id = Initializer.Users.Values.FirstOrDefault(o => o.Name == title).UserId;
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.UsersEdit(id: id));
            var html = Results(
                context: context,
                id: id);
            Assert.True(Compare.Html(
                context: context,
                html: html,
                htmlTests: htmlTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var validHtmlTests = HtmlData.ExistsOne(selector: "#Editor").ToSingleList();
            var notFoundMessage = HtmlData.NotFoundMessage().ToSingleList();
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "佐藤 由香",
                    userType: UserData.UserTypes.TenantManager1,
                    htmlTests: validHtmlTests),
                new TestPart(
                    title: "佐藤 由香",
                    userType: UserData.UserTypes.Privileged,
                    htmlTests: validHtmlTests),
                new TestPart(
                    title: "佐藤 由香",
                    userType: UserData.UserTypes.General1,
                    htmlTests: validHtmlTests),
                new TestPart(
                    title: "高橋 一郎",
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
            return UserUtilities.Editor(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context),
                userId: id,
                clearSessions: true);
        }
    }
}
