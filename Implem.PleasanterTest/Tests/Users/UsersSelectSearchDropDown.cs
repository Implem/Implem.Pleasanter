using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using Microsoft.AspNetCore.Mvc;
using NLog.Targets;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Implem.PleasanterTest.Tests.Users
{
    [Collection(nameof(UsersSelectSearchDropDown))]
    public class UsersSelectSearchDropDown
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            UserModel userModel,
            List<BaseTest> baseTests,
            Forms forms)
        {
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.UsersEdit(userModel.UserId),
                forms: forms);
            var results = Results(context: context);
            Initializer.SaveResults(results);
            Assert.True(Tester.Test(
                context: context,
                results: results,
                baseTests: baseTests));
        }
        public static IEnumerable<object[]> GetData()
        {
            var userModel = UserData.Get(UserData.UserTypes.Privileged);
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    forms: FormsUtilities.Get(
                        new KeyValue("DropDownSearchText", ""),
                        new KeyValue("DropDownSearchReferenceId", "1"),
                        new KeyValue("DropDownSearchSelectedValues", $"\"{userModel.UserId}\""),
                        new KeyValue("DropDownSearchTarget", "Users_Manager"),
                        new KeyValue("DropDownSearchMultiple", "false"),
                        new KeyValue("DropDownSearchResultsOffset", "-1"),
                        new KeyValue("DropDownSearchParentClass", ""),
                        new KeyValue("DropDownSearchParentDataId", ""),
                        new KeyValue("Users_Manager", $"{userModel.UserId}"),
                        new KeyValue("DropDownSearchResults", $"[\"{userModel.UserId}\"]")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "[id=\"Users_Manager\"]"),
                        JsonData.Value(
                            method: "Html",
                            target: "[id=\"Users_Manager\"]",
                            value: $"<option value=\"\"></option><option value=\"{userModel.UserId}\" selected=\"selected\">村上 佳奈（特権ユーザ）</option>")),
                    userType: UserData.UserTypes.TenantManager1),
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    userModel: testPart.UserModel,
                    baseTests: testPart.BaseTests,
                    forms: testPart.Forms);
            }
        }

        private static object[] TestData(
            UserModel userModel,
            List<BaseTest> baseTests,
            Forms forms)
        {
            return new object[]
            {
                userModel,
                baseTests,
                forms,
            };
        }

        private static string Results(Context context)
        {
            return Implem.Pleasanter.Libraries.Models.DropDowns.SelectSearchDropDown(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context));
        }
    }
}
