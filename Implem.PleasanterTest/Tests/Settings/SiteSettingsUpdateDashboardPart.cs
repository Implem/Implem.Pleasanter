using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;
using static Implem.PleasanterTest.Utilities.UserData;

namespace Implem.PleasanterTest.Tests.Settings
{
    [Collection(nameof(SiteSettingsUpdateDashboardPart))]
    public class SiteSettingsUpdateDashboardPart
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
            var siteId = Initializer.Sites.Get("課題管理").SiteId;
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "サイト設定 - Dashboard",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "UpdateDashboardPartLayouts"),
                        new KeyValue("DashboardPartLayouts","[]")),
                    baseTests: BaseData.Tests(
                        JsonData.Value(
                            method: "Message",
                            value: "{\"Id\":\"Updated\",\"Text\":\"\\\" サイト設定 - Dashboard \\\" を更新しました。\",\"Css\":\"alert-success\"}"),
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
            var itemModel = Initializer.ItemIds.Get(context.Id);
            return itemModel.Update(context: context);
        }
    }
}
