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
    [Collection(nameof(UsersEdit))]
    public class UsersEdit
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            var id = Initializer.Users.Values.FirstOrDefault(o => o.Name == title).UserId;
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.UsersEdit(id: id));
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
                    title: "佐藤 由香",
                    userType: UserData.UserTypes.TenantManager1,
                    baseTests: validHtmlTests),
                new TestPart(
                    title: "佐藤 由香",
                    userType: UserData.UserTypes.Privileged,
                    baseTests: validHtmlTests),
                new TestPart(
                    title: "佐藤 由香",
                    userType: UserData.UserTypes.General1,
                    baseTests: validHtmlTests),
                new TestPart(
                    title: "高橋 一郎",
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
            return UserUtilities.Editor(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context),
                userId: id,
                clearSessions: true);
        }
    }
}
