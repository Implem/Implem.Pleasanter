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
    public class ColumnNormal
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
            TestParts.ExistsOne(selector: "input#Issues_Title"),
            TestParts.ExistsOne(selector: "textarea#Issues_Body"),
            TestParts.ExistsOne(selector: "div[id=\"Issues_Body.editor\"]"),
            TestParts.ExistsOne(selector: "input#Issues_StartTime"),
            TestParts.ExistsOne(selector: "input#Issues_CompletionTime"),
            TestParts.ExistsOne(selector: "input#Issues_WorkValue"),
            TestParts.ExistsOne(selector: "input#Issues_ProgressRate"),
            TestParts.ExistsOne(selector: "span#Issues_RemainingWorkValue"),
            TestParts.ExistsOne(selector: "select#Issues_Status"),
            TestParts.ExistsOne(selector: "select#Issues_Manager"),
            TestParts.ExistsOne(selector: "select#Issues_Owner"),
            TestParts.ExistsOne(selector: "select#Issues_ClassA"),
            TestParts.ExistsOne(selector: "select#Issues_ClassB"),
            TestParts.ExistsOne(selector: "select#Issues_ClassC"),
            TestParts.ExistsOne(selector: "input#Issues_ClassD"),
            TestParts.ExistsOne(selector: "input#Issues_NumA"),
            TestParts.ExistsOne(selector: "input#Issues_NumB"),
            TestParts.ExistsOne(selector: "input#Issues_NumC"),
            TestParts.ExistsOne(selector: "input#Issues_NumD"),
            TestParts.ExistsOne(selector: "input#Issues_DateA"),
            TestParts.ExistsOne(selector: "input#Issues_CheckA"),
            TestParts.ExistsOne(selector: "textarea#Issues_DescriptionA"),
            TestParts.ExistsOne(selector: "div[id=\"Issues_DescriptionA.editor\"]"),
            TestParts.ExistsOne(selector: "input#Issues_AttachmentsA"),
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
                InitSiteSettings();
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

        private void InitSiteSettings()
        {
            var ss = SiteData.GetSiteSettings(
                context: Context,
                title: "商談");
            SiteData.UpdateSiteSettings(
                context: Context,
                siteId: IssueModel.SiteId,
                ss: ss);
        }
    }
}
