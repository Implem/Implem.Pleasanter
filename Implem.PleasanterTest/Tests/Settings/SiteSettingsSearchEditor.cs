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
    [Collection(nameof(SiteSettingsSearchEditor))]
    public class SiteSettingsSearchEditor
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
                    title: "課題管理",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "OpenSearchEditorColumnDialog"),
                        new KeyValue("EditorSourceColumnsType", "Columns")),
                    baseTests: BaseData.Tests(
                        JsonData.HtmlTextContains(
                            method: "Html",
                            target: "#SearchEditorColumnDialog",
                            selector: "#SearchEditorColumnForm",
                            value: "<button id=\"ShowTargetColumnBasic\" class=\"button button-icon w150\" type=\"button\" onclick=\"$p.selectSearchEditorColumn('Basic');\">基本</button><button id=\"ShowTargetColumnClass\" class=\"button button-icon w150\" type=\"button\" onclick=\"$p.selectSearchEditorColumn('Class');\">分類</button><button id=\"ShowTargetColumnNum\" class=\"button button-icon w150\" type=\"button\" onclick=\"$p.selectSearchEditorColumn('Num');\">数値</button></div><div class=\"command-left\"><button id=\"ShowTargetColumnDate\" class=\"button button-icon w150\" type=\"button\" onclick=\"$p.selectSearchEditorColumn('Date');\">日付</button><button id=\"ShowTargetColumnDescription\" class=\"button button-icon w150\" type=\"button\" onclick=\"$p.selectSearchEditorColumn('Description');\">説明</button><button id=\"ShowTargetColumnCheck\" class=\"button button-icon w150\" type=\"button\" onclick=\"$p.selectSearchEditorColumn('Check');\">チェック</button></div><div class=\"command-left\"><button id=\"ShowTargetColumnAttachments\" class=\"button button-icon w150\" type=\"button\" onclick=\"$p.selectSearchEditorColumn('Attachments');\">添付ファイル</button></div></fieldset><div class=\"command-center\"><button id=\"ShowTargetColumnDefault\" class=\"button button-icon\" type=\"button\" onclick=\"$p.selectSearchEditorColumn('');\" data-icon=\"ui-icon-gear\">リセット</button><button class=\"button button-icon button-neutral\" type=\"button\" onclick=\"$p.closeDialog($(this));\" data-icon=\"ui-icon-cancel\">キャンセル</button>"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "課題管理",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "SearchEditorColumnDialogInput"),
                        new KeyValue("SearchEditorColumnDialogInput","{\"selection\":\"Basic\",\"keyWord\":\"\"}")),
                    baseTests: BaseData.Tests(
                        JsonData.Value(
                            method: "Html",
                            target: "#EditorSourceColumns",
                            value: "<li class=\"ui-widget-content\" title=\"ロック\" data-order=\"12\" data-value=\"Locked\">[課題管理] ロック</li>"),
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
            return SiteUtilities.SetSiteSettings(
                context: context,
                siteId: siteId);
        }
    }
}
