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
            int groupId,
            UserModel userModel,
            List<HtmlTest> htmlTests)
        {
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.GroupsEdit(groupId: groupId));
            var html = Results(
                context: context,
                groupId: groupId);
            Assert.True(Compare.Html(
                context: context,
                html: html,
                htmlTests: htmlTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            yield return TestData(
                groupId: Initializer.Groups.Values.FirstOrDefault(o => o.GroupName == "グループ2").GroupId,
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
                groupId: Initializer.Groups.Values.FirstOrDefault(o => o.GroupName == "グループ2").GroupId,
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
                groupId: Initializer.Groups.Values.FirstOrDefault(o => o.GroupName == "グループ2").GroupId,
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
                groupId: Initializer.Groups.Values.FirstOrDefault(o => o.GroupName == "グループ1").GroupId,
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
            int groupId,
            UserModel userModel,
            List<HtmlTest> htmlTests)
        {
            return new object[]
            {
                groupId,
                userModel,
                htmlTests
            };
        }

        private static string Results(
            Context context,
            int groupId)
        {
            return GroupUtilities.Editor(
                context: context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(context: context),
                groupId: groupId,
                clearSessions: true);
        }
    }
}
