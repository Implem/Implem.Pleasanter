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
    var site = items.GetSiteByTitle('TargetResult')[0];
    const resultA = items.NewResult();
    resultA.Title = 'Result_A';
    const resultB = items.NewResult();
    resultB.Title = 'Result_B';
    const resultC = items.NewResult();
    resultC.Title = 'Result_C';

    if (!resultA.Create(site.SiteId)) {
        context.AddMessage('faild Create ResultA', 'alert-error');
    } else {
        context.AddMessage(resultA.Title, 'alert-information');
    }
    if (!resultB.Create(site.SiteId)) {
        context.AddMessage('faild Create ResultB', 'alert-error');
    } else {
        context.AddMessage(resultB.Title, 'alert-information');
    }
    if (!resultC.Create(site.SiteId)) {
        context.AddMessage('faild Create ResultC', 'alert-error');
    } else {
        context.AddMessage(resultC.Title, 'alert-information');
    }

    resultB.Body = 'Updated ResultB!!';
    if (!resultB.Update()) {
        context.AddMessage('faild Update ResultB', 'alert-error');
    } else {
        context.AddMessage('Updated', 'alert-information');
    }

    if (!resultC.Delete()) {
        context.AddMessage('faild Delete ResultC', 'alert-error');
    } else {
        context.AddMessage('Deleted', 'alert-information');
    }

} catch (e) {
    context.AddMessage(e.stack, 'alert-error');
}
*/

namespace Implem.PleasanterTest.Tests.ServerScript
{
    [Collection(nameof(ServerScriptResultApiModel))]
    public class ServerScriptResultApiModel
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

            //サイト名TargetResultのレコード一覧を取得
            var rows = RdsUtilities.SelectResultRows(
                context: context,
                siteTitle: "TargetResult");
            //タイトル"Result_A","Result_B"のレコードが存在する
            Assert.Contains(rows, row => row["Title"].ToStr() == "Result_A");
            Assert.Contains(rows, row => row["Title"].ToStr() == "Result_B");
            //タイトル"Result_C"のレコードが存在しない(削除済み)
            Assert.DoesNotContain(rows, row => row["Title"].ToStr() == "Result_C");
            //タイトル"Result_B"のBodyが設定した値になっている
            Assert.Equal("Updated ResultB!!", rows.First(row => row["Title"].ToStr() == "Result_B")["Body"].ToStr());
        }

        public static IEnumerable<object[]> GetData()
        {
            var hasMessages = BaseData.Tests(
                HtmlData.HasInformationMessage(message: "Result_A"),
                HtmlData.HasInformationMessage(message: "Result_B"),
                HtmlData.HasInformationMessage(message: "Result_C"),
                HtmlData.HasInformationMessage(message: "Updated"),
                HtmlData.HasInformationMessage(message: "Deleted"));

            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "サーバスクリプト-ResultApiModel",
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
