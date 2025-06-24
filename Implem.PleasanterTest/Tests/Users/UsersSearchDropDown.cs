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
    [Collection(nameof(UsersSearchDropDown))]
    public class UsersSearchDropDown
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
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    forms: FormsUtilities.Get(
                        new KeyValue("DropDownSearchText", ""),
                        new KeyValue("ControlId", "DropDownSearchTarget"),
                        new KeyValue("DropDownSearchReferenceId", "1"),
                        new KeyValue("DropDownSearchSelectedValues", "\"\""),
                        new KeyValue("DropDownSearchTarget", "Users_Manager"),
                        new KeyValue("DropDownSearchMultiple", "false"),
                        new KeyValue("DropDownSearchResultsOffset", "-1"),
                        new KeyValue("DropDownSearchParentClass", ""),
                        new KeyValue("DropDownSearchParentDataId", "")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#DropDownSearchDialogBody")),
                    userType: UserData.UserTypes.TenantManager1),
                new TestPart(
                    forms: FormsUtilities.Get(
                        new KeyValue("DropDownSearchText", "佐藤"),
                        new KeyValue("ControlId", "DropDownSearchText"),
                        new KeyValue("DropDownSearchReferenceId", "1"),
                        new KeyValue("DropDownSearchSelectedValues", "\"\""),
                        new KeyValue("DropDownSearchTarget", "Users_Manager"),
                        new KeyValue("DropDownSearchMultiple", "false"),
                        new KeyValue("DropDownSearchResultsOffset", "-1"),
                        new KeyValue("DropDownSearchParentClass", ""),
                        new KeyValue("DropDownSearchParentDataId", "")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#DropDownSearchResults"),
                        JsonData.TextCheckOrder(
                            method: "Html",
                            target: "#DropDownSearchResults",
                            wordArray: new string[]
                            {
                                "未設定",
                                "佐藤 由香"
                            })),
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
            return Implem.Pleasanter.Libraries.Models.DropDowns.SearchDropDown(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context));
        }
    }
}
