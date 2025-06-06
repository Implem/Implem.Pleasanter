using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Operations;
using Implem.PleasanterTest.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static Implem.PleasanterTest.Utilities.UserData;

namespace Implem.PleasanterTest.Tests.Tenants
{
    [Collection(nameof(TenantsBGServerScriptSearchDropDown))]
    public class TenantsBGServerScriptSearchDropDown
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            Forms forms,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.TenantsBGServerScript(),
                httpMethod: "POST",
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
            var validHtmlTests = BaseData.Tests(HtmlData.ExistsOne(selector: "#Editor"));
            var notFoundMessage = BaseData.Tests(HtmlData.NotFoundMessage());
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "DropDownSearchTarget"),
                        new KeyValue("DropDownSearchReferenceId", "0"),
                        new KeyValue("DropDownSearchText","村上 佳奈（特権ユーザ）"),
                        new KeyValue("DropDownSearchTarget", "ServerScriptUser")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#DropDownSearchDialogBody"),
                        JsonData.HtmlTextContains(
                            method: "Html",
                            target: "#DropDownSearchDialogBody",
                            selector: "#DropDownSearchResultsWrapper",
                            value: $"<li class=\"ui-widget-content\" data-value=\"\t\">(未設定)</li><li class=\"ui-widget-content\" data-value=\"{userModel.UserId}\">村上 佳奈（特権ユーザ）</li>")),
                    userType: UserData.UserTypes.Privileged),
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    forms : testPart.Forms,
                    userModel: testPart.UserModel,
                    baseTests: testPart.BaseTests);
            }
        }

        private static object[] TestData(
            Forms forms,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            return new object[]
            {
                forms,
                userModel,
                baseTests
            };
        }

        private static string Results(Context context)
        {
            return TenantUtilities.SearchDropDown(context: context);
        }
    }
}
