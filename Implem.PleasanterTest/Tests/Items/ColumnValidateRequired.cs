using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Tests.Items
{
    public class ColumnValidateRequired
    {
        private Context Context;
        private SiteSettings SiteSettings;
        private IssueModel IssueModel;
        private string Html;

        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(List<BaseTest> baseTests)
        {
            Init();
            Assert.True(Tester.Test(
                context: Context,
                results: Html,
                baseTests: baseTests));
        }

        public static IEnumerable<object[]> GetData => new List<object[]>()
        {
            TestParts.ExistsOne(selector: "span#Issues_IssueId"),
            TestParts.ExistsOne(selector: "span#Issues_Ver"),
            TestParts.ValidateRequired(selector: "input#Issues_Title"),
            TestParts.ValidateRequired(selector: "textarea#Issues_Body"),
            TestParts.ExistsOne(selector: "div[id=\"Issues_Body.editor\"]"),
            TestParts.ValidateRequired(selector: "input#Issues_StartTime"),
            TestParts.ValidateRequired(selector: "input#Issues_CompletionTime"),
            TestParts.ValidateRequired(selector: "input#Issues_WorkValue"),
            TestParts.ValidateRequired(selector: "input#Issues_ProgressRate"),
            TestParts.ExistsOne(selector: "span#Issues_RemainingWorkValue"),
            TestParts.ValidateRequired(selector: "select#Issues_Status"),
            TestParts.ValidateRequired(selector: "select#Issues_Manager"),
            TestParts.ValidateRequired(selector: "select#Issues_Owner"),
            TestParts.ValidateRequired(selector: "select#Issues_ClassA"),
            TestParts.ValidateRequired(selector: "select#Issues_ClassB"),
            TestParts.ValidateRequired(selector: "select#Issues_ClassC"),
            TestParts.ValidateRequired(selector: "input#Issues_ClassD"),
            TestParts.ValidateRequired(selector: "input#Issues_NumA"),
            TestParts.ValidateRequired(selector: "input#Issues_NumB"),
            TestParts.ValidateRequired(selector: "input#Issues_NumC"),
            TestParts.ValidateRequired(selector: "input#Issues_NumD"),
            TestParts.ValidateRequired(selector: "input#Issues_DateA"),
            TestParts.ValidateRequired(selector: "input#Issues_CheckA"),
            TestParts.ValidateRequired(selector: "textarea#Issues_DescriptionA"),
            TestParts.ExistsOne(selector: "div[id=\"Issues_DescriptionA.editor\"]"),
            TestParts.ValidateAttachmentsRequired(selector: "input#Issues_AttachmentsA"),
            TestParts.ExistsOne(selector: "div[id=\"AttachmentsA.upload\"]"),
            TestParts.ExistsOne(selector: "textarea#Comments")
        };

        private void Init()
        {
            if (Context == null)
            {
                IssueModel = Initializer.Issues.Get("商談").Get("営業支援ツールの追加機能開発");
                Context = ContextData.Get(
                    userType: UserData.UserTypes.General1,
                    routeData: new Dictionary<string, string>()
                    {
                        { "controller", "items" },
                        { "action", "edit" },
                        { "id", IssueModel.IssueId.ToString() },
                    });
                InitSiteSettings(
                    "Title",
                    "Body",
                    "StartTime",
                    "CompletionTime",
                    "WorkValue",
                    "ProgressRate",
                    "Status",
                    "Manager",
                    "Owner",
                    "ClassA",
                    "ClassB",
                    "ClassC",
                    "ClassD",
                    "NumA",
                    "NumB",
                    "NumC",
                    "NumD",
                    "DateA",
                    "CheckA",
                    "DescriptionA",
                    "AttachmentsA");
                SiteSettings = SiteSettingsUtilities.Get(
                    context: Context,
                    siteId: IssueModel.SiteId);
                Html = IssueUtilities.Editor(
                    context: Context,
                    ss: SiteSettings,
                    issueId: IssueModel.IssueId,
                    clearSessions: true);
            }
        }

        private void InitSiteSettings(params string[] columnNames)
        {
            var ss = SiteData.GetSiteSettings(
                context: Context,
                title: "商談");
            columnNames.ForEach(columnName =>
            {
                var column = ss.GetColumn(
                    context: Context,
                    columnName: columnName);
                column.ValidateRequired = true;
            });
            SiteData.UpdateSiteSettings(
                context: Context,
                siteId: IssueModel.SiteId,
                ss: ss);
        }
    }
}
