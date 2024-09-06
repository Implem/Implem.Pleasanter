using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;

//下記サーバースクリプトのテストを行います。
/* script1
try {
    const newResults = items.NewSite('Results');
    newResults.Title = 'Result_001';
    if (!items.Create(0, newResults)) {
        context.AddMessage('failed CreateSite Result_001', 'alert-error');
    } else {
        context.AddMessage('Result_001', 'alert-information');
        let resultItem = items.NewResult();
        resultItem.Title = 'NewResult';
        if (!items.Create(newResults.SiteId, resultItem)) {
            context.AddMessage('failed CreateResult', 'alert-error');
        } else {
            context.AddMessage('NewResult', 'alert-information');
        }
    }
} catch (e) {
    context.AddMessage(e.stack, 'alert-error');
}
*/

/* script2
try {
    const newIssues = items.NewSite('Issues');
    newIssues.Title = 'Issue_001';
    if (!items.Create(0, newIssues)) {
        context.AddMessage('failed CreateSite Issue_001', 'alert-error');
    } else {
        context.AddMessage('Issue_001', 'alert-information');
        let issueItem = items.NewIssue();
        issueItem.Title = 'NewIssue';
        if (!items.Create(newIssues.SiteId, issueItem)) {
            context.AddMessage('failed CreateIssue', 'alert-error');
            return;
        } else {
            context.AddMessage('NewIssue', 'alert-information');
        }
    }

} catch (e) {
    context.AddMessage(e.stack, 'alert-error');
}
*/

namespace Implem.PleasanterTest.Tests.ServerScript
{
    [Collection(nameof(ServerScriptItemsNew))]
    public class ServerScriptItemsNew
    {
        private static readonly string IssuesSiteTitle = "Issue_001";
        private static readonly string ResultsSiteTitle = "Result_001";
        private static readonly string IssueTitle = "NewIssue";
        private static readonly string ResultTitle = "NewResult";


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
            context.BackgroundServerScript = true;
            var results = Results(context: context);
            Initializer.SaveResults(results);
            Assert.True(Tester.Test(
                context: context,
                results: results,
                baseTests: baseTests));
            //サーバスクリプトで作成したサイト、レコードが作成されていることを確認する。
            Assert.Equal(1, RdsUtilities.SitesCount(context, IssuesSiteTitle));
            Assert.Equal(1, RdsUtilities.SitesCount(context, ResultsSiteTitle));
            Assert.Equal(1, RdsUtilities.IssuesCount(context, IssueTitle));
            Assert.Equal(1, RdsUtilities.ResultsCount(context, ResultTitle));

        }

        public static IEnumerable<object[]> GetData()
        {
            var hasMessages = BaseData.Tests(
                HtmlData.HasInformationMessage(message: IssuesSiteTitle),
                HtmlData.HasInformationMessage(message: ResultsSiteTitle),
                HtmlData.HasInformationMessage(message: IssueTitle),
                HtmlData.HasInformationMessage(message: ResultTitle));
            var testParts = new List<TestPart>()
            {
                new TestPart(title: "サーバスクリプト-Items.New", baseTests: hasMessages,userType: UserData.UserTypes.Privileged),

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
