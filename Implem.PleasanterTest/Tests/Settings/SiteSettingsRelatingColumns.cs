using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using NLog.Targets;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Tests.Settings
{
    [Collection(nameof(SiteSettingsRelatingColumns))]
    public class SiteSettingsRelatingColumns
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
                    title: "サイト設定 - RelatingColumns",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "NewRelatingColumn")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#RelatingColumnDialog"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - RelatingColumns",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "EditRelatingColumns"),
                        new KeyValue("RelatingColumnId", "1")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#RelatingColumnDialog"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - RelatingColumns",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "UpdateRelatingColumn"),
                        new KeyValue("RelatingColumnId", "1"),
                        new KeyValue("RelatingColumnTitle", "RelatingColumn-cahnge")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#EditRelatingColumns"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged"),
                        JsonData.HtmlTextContains(
                            method : "Html",
                            target : "#EditRelatingColumns",
                            selector : "#EditRelatingColumns",
                            value : "RelatingColumn-cahnge")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - RelatingColumns",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "AddRelatingColumn"),
                        new KeyValue("RelatingColumnTitle", "test")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "ReplaceAll",
                            target: "#EditRelatingColumns"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged"),
                        JsonData.HtmlTextContains(
                            method : "ReplaceAll",
                            target : "#EditRelatingColumns",
                            selector : "#EditRelatingColumns",
                            value : "test")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - RelatingColumns",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "DeleteRelatingColumns"),
                        new KeyValue("RelatingColumnId", "1"),
                        new KeyValue("EditRelatingColumns", "[\"1\"]")),                       
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "ReplaceAll",
                            target: "#EditRelatingColumns"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged"),
                        JsonData.Value(
                            method: "ReplaceAll",
                            target:"#EditRelatingColumns",
                            value: "<grid-container data-scrollable=\"1\" id=\"EditRelatingColumnsWrap\" class=\"grid-wrap\"><table data-name=\"RelatingColumnId\" data-func=\"openRelatingColumnDialog\" data-action=\"SetSiteSettings\" data-method=\"post\" id=\"EditRelatingColumns\" class=\"grid\"><thead><tr class=\"ui-widget-header\"><th><label class=\"check-option\"><span class=\"check-icon\"><input class=\"select-all\" type=\"checkbox\" /></span><span class=\"check-text\"></span></label></th><th>タイトル</th><th>リンク</th></tr></thead><tbody></tbody></table></grid-container>")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - RelatingColumns",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "MoveUpRelatingColumns"),
                        new KeyValue("EditRelatingColumns", "[\"1\"]")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#EditRelatingColumns"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - RelatingColumns",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "MoveDownRelatingColumns"),
                        new KeyValue("EditRelatingColumns", "[\"1\"]")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#EditRelatingColumns"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
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
