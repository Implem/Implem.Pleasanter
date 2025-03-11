using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Tests.Settings
{
    [Collection(nameof(SiteSettingsNewEditView))]
    public class SiteSettingsNewEditView
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
                    title: "サイト設定 - Views",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "NewView")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#ViewDialog"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged"),
                        JsonData.TextContains(
                            method: "Html",
                            target: "#ViewDialog",
                            value: @"button id=""AddView""")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - Views",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "EditView"),
                        new KeyValue("Views", @"[""1""]")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#ViewDialog"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged"),
                        JsonData.TextContains(
                            method: "Html",
                            target: "#ViewDialog",
                            value: "テスト用ビュー1")),
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
            return SiteUtilities.SetSiteSettings(
                context: context,
                siteId: siteId);
        }
    }
}
