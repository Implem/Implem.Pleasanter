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
            List<BaseTest> baseTests)
        {
            var id = Initializer.Groups.Values.FirstOrDefault(o => o.GroupName == title).GroupId;
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.GroupsEdit(id: id));
            var results = Results(
                context: context,
                id: id);
            Initializer.SaveResults(results);
            Assert.True(Tester.Test(
                context: context,
                results: results,
                baseTests: baseTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var validHtmlTests = BaseData.Tests(HtmlData.ExistsOne(selector: "#Editor"));
            var notFoundMessage = BaseData.Tests(HtmlData.NotFoundMessage());
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "グループ2",
                    userType: UserData.UserTypes.TenantManager1,
                    baseTests: validHtmlTests),
                new TestPart(
                    title: "グループ2",
                    userType: UserData.UserTypes.Privileged,
                    baseTests: validHtmlTests),
                new TestPart(
                    title: "グループ2",
                    userType: UserData.UserTypes.General1,
                    baseTests: validHtmlTests),
                new TestPart(
                    title: "グループ1",
                    userType: UserData.UserTypes.General1,
                    baseTests: notFoundMessage)
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    userModel: testPart.UserModel,
                    baseTests: testPart.BaseTests);
            }
        }

        private static object[] TestData(
            string title,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            return new object[]
            {
                title,
                userModel,
                baseTests
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
