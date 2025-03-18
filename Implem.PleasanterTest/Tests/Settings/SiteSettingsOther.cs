using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Tests.Settings
{
    [Collection(nameof(SiteSettingsOther))]
    public class SiteSettingsOther
    {

        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            Forms forms,
            List<BaseTest> baseTests,
            UserModel userModel)
        {
            var siteId = Initializer.Sites.Get(title).SiteId;
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.ItemsSetSiteSettings(id: siteId),
                httpMethod: "POST",
                forms: forms);
            var results = Results(context: context, siteId: siteId);
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
                    title: "サイト設定 - 更新",
                    forms: FormsUtilities.Get(
                    new KeyValue("ControlId", "AddPermissions"),
                    new KeyValue("SourcePermissionsOffset","100"),
                    new KeyValue("CurrentPermissionsAll","[\"User,1,511\"]"),
                    new KeyValue("SourcePermissions","[\"User,-1,0\"]")),
                baseTests: BaseData.Tests(
                    JsonData.TextContains(
                        method: "Html",
                        target: "#CurrentPermissions",
                        value: "<li class=\"ui-widget-content ui-selected\" data-value=\"User,-1,31\">[ユーザ ] 全てのユーザ - [書き込み]</li>"),
                    JsonData.ExistsOne(
                        method: "SetMemory",
                        target: "formChanged")),
    userType: UserData.UserTypes.Privileged)
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    forms: testPart.Forms,
                    baseTests: testPart.BaseTests,
                    userModel: testPart.UserModel);
            }
        }

        private static object[] TestData(
            string title,
            Forms forms,
            List<BaseTest> baseTests,
            UserModel userModel)
        {
            return new object[]
            {
                title,
                forms,
                baseTests,
                userModel
            };
        }

        private static string Results(Context context, long siteId)
        {
            return PermissionUtilities.SetPermissions(
                context: context,
                referenceId: siteId);
        }
    }
}
