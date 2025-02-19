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
    [Collection(nameof(SiteSettingsAddBulkUpdateColumn))]
    public class SiteSettingsAddBulkUpdateColumn
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
                    title: "顧客マスタ",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "AddBulkUpdateColumn"),
                        new KeyValue("BulkUpdateColumnTitle", "test")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "ReplaceAll",
                            target: "#EditBulkUpdateColumns"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged"),
                        JsonData.HtmlTextContains(
                            method : "ReplaceAll",
                            target : "#EditBulkUpdateColumns",
                            selector : "#EditBulkUpdateColumns",
                            value : "test")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - BulkUpdate",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "UpdateBulkUpdateColumn"),
                        new KeyValue("BulkUpdateColumnTitle", "bulkupdatesetting-chage"),
                        new KeyValue("BulkUpdateColumnId",  "1")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#EditBulkUpdateColumns"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged"),
                        JsonData.HtmlTextContains(
                            method : "Html",
                            target : "#EditBulkUpdateColumns",
                            selector : "#EditBulkUpdateColumns",
                            value : "bulkupdatesetting-chage")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - BulkUpdate",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "CopyBulkUpdateColumns"),
                        new KeyValue("EditBulkUpdateColumns", "[\"1\"]")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "ReplaceAll",
                            target: "#EditBulkUpdateColumns"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged"),
                        JsonData.HtmlTextContains(
                            method : "ReplaceAll",
                            target : "#EditBulkUpdateColumns",
                            selector : "#EditBulkUpdateColumns",
                            value : "bulkupdatesetting")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - BulkUpdate",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "UpdateBulkUpdateColumnDetail"),
                        new KeyValue("BulkUpdateColumnDetailColumnName", "Title"),
                        new KeyValue("BulkUpdateColumnDetailValidateRequired", "false")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "SetValue",
                            target: "#BulkUpdateColumnDetails"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged"),
                        JsonData.Value(
                            method: "SetValue",
                            target:"#BulkUpdateColumnDetails",
                            value: "{\"Title\":{\"ValidateRequired\":false,\"EditorReadOnly\":false,\"DefaultInput\":\"\"}}")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - BulkUpdate",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "DeleteBulkUpdateColumns"),
                        new KeyValue("EditBulkUpdateColumns", "[\"1\"]")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "ReplaceAll",
                            target: "#EditBulkUpdateColumns"),
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
