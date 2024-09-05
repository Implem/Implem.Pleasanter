using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;
using System.Linq;

//下記サーバースクリプトのテストを行います。
/*
try {
    var site = items.GetSiteByTitle('TargetIssue')[0];
    const issueA = items.NewIssue();
    issueA.Title = 'Issue_A';
    const issueB = items.NewIssue();
    issueB.Title = 'Issue_B';
    const issueC = items.NewIssue();
    issueC.Title = 'Issue_C';

    if (!issueA.Create(site.SiteId)) {
        context.AddMessage('faild Create IssueA', 'alert-error');
    } else {
        context.AddMessage(issueA.Title, 'alert-information');
    }
    if (!issueB.Create(site.SiteId)) {
        context.AddMessage('faild Create IssueB', 'alert-error');
    } else {
        context.AddMessage(issueB.Title, 'alert-information');
    }
    if (!issueC.Create(site.SiteId)) {
        context.AddMessage('faild Create IssueC', 'alert-error');
    } else {
        context.AddMessage(issueC.Title, 'alert-information');
    }

    issueB.Body = 'Updated IssueB!!';
    if (!issueB.Update()) {
        context.AddMessage('faild Update IssueB', 'alert-error');
    } else {
        context.AddMessage('Updated', 'alert-information');
    }

    if (!issueC.Delete()) {
        context.AddMessage('faild Delete IssueC', 'alert-error');
    } else {
        context.AddMessage('Deleted', 'alert-information');
    }

} catch (e) {
    context.AddMessage(e.stack, 'alert-error');
}
*/

namespace Implem.PleasanterTest.Tests.ServerScript
{
    public class ServerScriptIssueApiModel
    {

        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            var siteId = Initializer.Sites.Get(title).SiteId;
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.ItemsIndex(id: siteId));

            context.BackgroundServerScript = true; //サーバースクリプトのテスト実施時は必須

            var results = Results(context: context);
            Initializer.SaveResults(results);
            Assert.True(Tester.Test(
                context: context,
                results: results,
                baseTests: baseTests));

            //サイト名TargetIssueのレコード一覧を取得
            var rows = RdsUtilities.SelectIssueRows(
                context: context,
                siteTitle: "TargetIssue");
            //タイトル"Issue_A","Issue_B"のレコードが存在する
            Assert.Contains(rows, row => row["Title"].ToStr() == "Issue_A");
            Assert.Contains(rows, row => row["Title"].ToStr() == "Issue_B");
            //タイトル"Issue_C"のレコードが存在しない(削除済み)
            Assert.DoesNotContain(rows, row => row["Title"].ToStr() == "Issue_C");
            //タイトル"Issue_B"のBodyが設定した値になっている
            Assert.Equal("Updated IssueB!!", rows.First(row => row["Title"].ToStr() == "Issue_B")["Body"].ToStr());
        }

        public static IEnumerable<object[]> GetData()
        {
            var hasMessages = BaseData.Tests(
                HtmlData.HasInformationMessage(message: "Issue_A"),
                HtmlData.HasInformationMessage(message: "Issue_B"),
                HtmlData.HasInformationMessage(message: "Issue_C"),
                HtmlData.HasInformationMessage(message: "Updated"),
                HtmlData.HasInformationMessage(message: "Deleted"));

            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "サーバスクリプト-IssueApiModel",
                    baseTests: hasMessages,
                    userType: UserData.UserTypes.Privileged),

            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    userModel: testPart.UserModel,
                    baseTests: testPart.BaseTests);
            }
        }

        private static object[] TestData(
            string title,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            return new object[]
            {
                title,
                userModel,
                baseTests
            };
        }

        private static string Results(Context context)
        {
            var itemModel = Initializer.ItemIds.Get(context.Id);
            return itemModel.Index(context: context);
        }
    }
}
