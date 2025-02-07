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
                        //new KeyValue("BulkUpdateColumnTitle", "test")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#RelatingColumnDialog"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                        //JsonData.HtmlTextContains(
                        //    method : "ReplaceAll",
                        //    target : "#EditBulkUpdateColumns",
                        //    selector : "#EditBulkUpdateColumns",
                        //    value : "test")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - RelatingColumns",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "UpdateRelatingColumn"),
                        new KeyValue("RelatingColumnTitle", "RelatingColumn-cahnge"),
                        new KeyValue("RelatingColumnId",  "1")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#EditRelatingColumns"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                        //JsonData.HtmlTextContains(
                        //    method : "Html",
                        //    target : "#EditBulkUpdateColumns",
                        //    selector : "#EditBulkUpdateColumns",
                        //    value : "bulkupdatesetting-chage")),
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
                            target: "formChanged")),
                        //JsonData.HtmlTextContains(
                        //    method : "ReplaceAll",
                        //    target : "#EditBulkUpdateColumns",
                        //    selector : "#EditBulkUpdateColumns",
                        //    value : "bulkupdatesetting")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - RelatingColumns",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "DeleteRelatingColumns"),
                        new KeyValue("RelatingColumnId", "1"),
                        new KeyValue("EditRelatingColumns", "[\"1\"]")),
                        //new KeyValue("BulkUpdateColumnDetailValidateRequired", "false")),EditRelatingColumns, ["1"]
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
                            value: "<div id=\"EditRelatingColumnsWrap\" class=\"grid-wrap\"><table data-name=\"RelatingColumnId\" data-func=\"openRelatingColumnDialog\" data-action=\"SetSiteSettings\" data-method=\"post\" id=\"EditRelatingColumns\" class=\"grid\"><thead><tr class=\"ui-widget-header\"><th><input class=\"select-all\" type=\"checkbox\" /><label></label></th><th>タイトル</th><th>リンク</th></tr></thead><tbody></tbody></table></div>")),
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
