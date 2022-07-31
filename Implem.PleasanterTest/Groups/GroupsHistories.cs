using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Implem.PleasanterTest.Groups
{
    public class GroupsHistories
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            UserModel userModel,
            List<JsonTest> jsonTests)
        {
            var id = Initializer.Groups.Values.FirstOrDefault(o => o.GroupName == title).GroupId;
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.GroupsHistories(id: id));
            var json = GetJson(context: context);
            Assert.True(Compare.Json(
                context: context,
                json: json,
                jsonTests: jsonTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            yield return TestData(
                title: "グループ9",
                userModel: UserData.Get(userType: UserData.UserTypes.TenantManager1),
                jsonTests: JsonData.ExistsOne(
                    method: "Html",
                    target: "#FieldSetHistories").ToSingleList());
        }

        private static object[] TestData(
            string title,
            UserModel userModel,
            List<JsonTest> jsonTests)
        {
            return new object[]
            {
                title,
                userModel,
                jsonTests
            };
        }

        private static string GetJson(Context context)
        {
            return GroupUtilities.Histories(
                context: context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(context: context),
                groupId: context.Id.ToInt());
        }
    }
}
