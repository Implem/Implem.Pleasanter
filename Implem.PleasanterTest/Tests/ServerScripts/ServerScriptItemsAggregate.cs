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
    var site = items.GetSiteByTitle('ItemsTargetIssue')[0];
    var aggregateSite = items.GetSiteByTitle('サーバスクリプト-Items.Aggregate')[0];
    const issueA = items.NewIssue();
    issueA.Title = 'Issue_A';
    const issueB = items.NewIssue();
    issueB.Title = 'Issue_B';
    const issueC = items.NewIssue();
    issueC.Title = 'Issue_C';
    const issueD = items.NewIssue();
    issueD.Title = 'Issue_D';
    const issueE = items.NewIssue();
    issueE.Title = 'Issue_E';

    if (items.Count(site.SiteId) != 3) {
        context.AddMessage('Count is not 3', 'alert-error');
    } else {
        context.AddMessage(String(items.Count(site.SiteId)), 'alert-information');
        issueA.NumA = items.Count(site.SiteId);
        issueA.Create(aggregateSite.SiteId);
    }
    if (items.Sum(site.SiteId,'NumA') != 600) {
        context.AddMessage('Total is not 600', 'alert-error');
    } else {
        context.AddMessage(String(items.Sum(site.SiteId,'NumA')), 'alert-information');
        issueB.NumA = items.Sum(site.SiteId,'NumA');
        issueB.Create(aggregateSite.SiteId);
    }
    if (items.Max(site.SiteId,'NumA') != 300) {
        context.AddMessage('Maximum value is not 300', 'alert-error');
    } else {
        context.AddMessage(String(items.Max(site.SiteId,'NumA')), 'alert-information');
        issueC.NumA = items.Max(site.SiteId,'NumA');
        issueC.Create(aggregateSite.SiteId);
    }
    if (items.Min(site.SiteId,'NumA') != 100) {
        context.AddMessage('Manimum value is not 100', 'alert-error');
    } else {
        context.AddMessage(String(items.Min(site.SiteId,'NumA')), 'alert-information');
        issueD.NumA = items.Min(site.SiteId,'NumA');
        issueD.Create(aggregateSite.SiteId);
    }
    if (items.Average(site.SiteId,'NumA') != 200) {
        context.AddMessage('Average is not 200', 'alert-error');
    } else {
        context.AddMessage(String(items.Average(site.SiteId,'NumA')), 'alert-information');
        issueE.NumA = items.Average(site.SiteId,'NumA');
        issueE.Create(aggregateSite.SiteId);
    }
} catch (e) {
    context.AddMessage(e.stack, 'alert-error');
}
*/

namespace Implem.PleasanterTest.Tests.ServerScript
{
    [Collection(nameof(ServerScriptItemsAggregate))]
    public class ServerScriptItemsAggregate
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
            //サイト名サーバスクリプト-Items.Aggregateのレコード一覧を取得
            var rows = RdsUtilities.SelectIssueRows(
                context: context,
                siteTitle: "サーバスクリプト-Items.Aggregate");
            //サーバスクリプトで作成したレコードのNumAが設定した値になっている
            Assert.Equal(3, rows.First(row => row["Title"].ToStr() == "Issue_A")["NumA"].ToDecimal());
            Assert.Equal(600, rows.First(row => row["Title"].ToStr() == "Issue_B")["NumA"].ToDecimal());
            Assert.Equal(300, rows.First(row => row["Title"].ToStr() == "Issue_C")["NumA"].ToDecimal());
            Assert.Equal(100, rows.First(row => row["Title"].ToStr() == "Issue_D")["NumA"].ToDecimal());
            Assert.Equal(200, rows.First(row => row["Title"].ToStr() == "Issue_E")["NumA"].ToDecimal());
        }

        public static IEnumerable<object[]> GetData()
        {
            var hasMessages = BaseData.Tests(
                HtmlData.HasInformationMessage(message: "3"),
                HtmlData.HasInformationMessage(message: "600"),
                HtmlData.HasInformationMessage(message: "300"),
                HtmlData.HasInformationMessage(message: "100"),
                HtmlData.HasInformationMessage(message: "200"));

            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "サーバスクリプト-Items.Aggregate",
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
