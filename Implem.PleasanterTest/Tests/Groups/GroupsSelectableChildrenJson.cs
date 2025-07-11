using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Implem.PleasanterTest.Tests.Groups
{
    [Collection(nameof(GroupsSelectableChildrenJson))]
    public class GroupsSelectableChildrenJson
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            Forms form,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            var id = Initializer.Groups.Values.FirstOrDefault(o => o.GroupName == "グループ1").GroupId;
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.GroupsEdit(id: id),
                forms: form);
            var results = Results(context: context);
            Initializer.SaveResults(results);
            Assert.True(Tester.Test(
                context: context,
                results: results,
                baseTests: baseTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var id = Initializer.Groups.Values.FirstOrDefault(o => o.GroupName == "グループ3").GroupId;
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    forms: FormsUtilities.Get(
                        new KeyValue("SearchChildText", "グループ3"),
                        new KeyValue("ControlId", "SearchChildText"),
                        new KeyValue("VerUp", "false"),
                        new KeyValue("AddedGroupMembers", "[]"),
                        new KeyValue("DeletedGroupMembers", "[]"),
                        new KeyValue("ModifiedGroupMembers", "[]"),
                        new KeyValue("CurrentMembersOffset", "0"),
                        new KeyValue("SearchMemberText", ""),
                        new KeyValue("SelectableMembers", "[]"),
                        new KeyValue("SelectableMembersAll", "[]"),
                        new KeyValue("SelectableMembersOffset", "0"),
                        new KeyValue("CurrentChildren", "[]"),
                        new KeyValue("CurrentChildrenAll", "[]"),
                        new KeyValue("AddedGroupChildren", "[]"),
                        new KeyValue("DeletedGroupChildren", "[]"),
                        new KeyValue("ModifiedGroupChildren", "[]"),
                        new KeyValue("CurrentChildrenOffset", "0"),
                        new KeyValue("SelectableChildren", "[]"),
                        new KeyValue("SelectableChildrenAll", "[]"),
                        new KeyValue("SelectableChildrenOffset", "0")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#SelectableChildren"),
                        JsonData.Value(
                            method: "Html",
                            target: "#SelectableChildren",
                            value: $"<li class=\"ui-widget-content\" data-value=\"Group,{id},\">[グループ {id}] グループ3</li>")),
                    userType: UserData.UserTypes.TenantManager1)
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    form: testPart.Forms,
                    userModel: testPart.UserModel,
                    baseTests: testPart.BaseTests);
            }
        }

        private static object[] TestData(
            Forms form,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            return new object[]
            {
                form,
                userModel,
                baseTests
            };
        }

        private static string Results(Context context)
        {
            return GroupUtilities.SelectableChildrenJson(context: context);
        }
    }
}
