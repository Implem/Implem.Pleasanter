using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Tests.Users
{
    public class UsersEdit
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            int userId,
            UserModel userModel,
            List<HtmlTest> htmlTests)
        {
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.UsersEdit(userId: userId));
            var html = Results(
                context: context,
                userId: userId);
            Assert.True(Compare.Html(
                context: context,
                html: html,
                htmlTests: htmlTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            yield return TestData(
                userId: UserData.Get(userType: UserData.UserTypes.General1).UserId,
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
                userId: UserData.Get(userType: UserData.UserTypes.General1).UserId,
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
                userId: UserData.Get(userType: UserData.UserTypes.General1).UserId,
                userModel: UserData.Get(userType: UserData.UserTypes.General1),
                htmlTests: new List<HtmlTest>
                {
                    new HtmlTest()
                    {
                        Type = HtmlTest.Types.ExistsOne,
                        Selector = "#Editor"
                    }
                });
            yield return TestData(
                userId: UserData.Get(userType: UserData.UserTypes.General2).UserId,
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
            int userId,
            UserModel userModel,
            List<HtmlTest> htmlTests)
        {
            return new object[]
            {
                userId,
                userModel,
                htmlTests
            };
        }

        private static string Results(
            Context context,
            int userId)
        {
            return UserUtilities.Editor(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context),
                userId: userId,
                clearSessions: true);
        }
    }
}
