using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;
using Org.BouncyCastle.Asn1.X509;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Xunit.Sdk;

//下記サーバースクリプトのテストを行います。
/*
try {
    // 判定により表示されるメッセージのcssを設定
    const successScc = 'alert-information';
    const errorScc = 'alert-error';

    // 判定により表示されるメッセージ内容を設定
    const successMessage = '：テスト成功';
    const errorMessage = '：テスト失敗';

    var condition;
    var title;
    var data;

    //1. items.GetClosestSite
    title = 'items.GetClosestSite()';
    condition = 'items.GetClosestSite';
    data = items.GetClosestSite('GetSiteBySiteName');
    judgeCondition(data.Title, condition);

    //2. items.GetSite
    title = 'items.NewSite()';
    condition = title;
    var item = items.NewSite("Results");
    item.Title = title;
    items.Create(0, item);
    judgeCondition(item.Title, condition);
    title = 'items.GetSite()';
    data = items.GetSite(item.SiteId)[0].Model;
    judgeCondition(data.SavedTitle, condition);

    //3. items.GetSiteByGroupName
    condition = 'items.GetSiteByGroupName';
    title = 'items.GetSiteByGroupName()';
    data = items.GetSiteByGroupName('items.GetSiteByGroupName')[0].Model;
    judgeCondition(data.SavedTitle, condition);

    //4. items.GetSiteByName
    condition = 2;
    title = 'items.GetSiteByName()';
    data = items.GetSiteByName('GetSiteBySiteName');
    judgeCondition(data.Length, condition);

    function judgeCondition(formula, condition) {
        if (formula === condition) {
            context.AddMessage(title + successMessage, successScc);
        } else {
            context.AddMessage(title + errorMessage, errorScc);
        }
    }
} catch (ex) {
    context.Log(ex.stack);
}


*/

namespace Implem.PleasanterTest.Tests.ServerScript
{
    public class ServerScriptItemsGetSite
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
            context.BackgroundServerScript = true;
            var results = Results(context: context);
            Initializer.SaveResults(results);
            Assert.True(Tester.Test(
                context: context,
                results: results,
                baseTests: baseTests));

        }

        public static IEnumerable<object[]> GetData()
        {
            var hasMessages = BaseData.Tests(
                HtmlData.HasInformationMessage(message: "items.GetClosestSite()：テスト成功"),
                HtmlData.HasInformationMessage(message: "items.NewSite()：テスト成功"),
                HtmlData.HasInformationMessage(message: "items.GetSite()：テスト成功"),
                HtmlData.HasInformationMessage(message: "items.GetSiteByGroupName()：テスト成功"),
                HtmlData.HasInformationMessage(message: "items.GetSiteByName()：テスト成功"));
            var testParts = new List<TestPart>()
            {
                new TestPart(title: "サイト取得", baseTests: hasMessages,userType: UserData.UserTypes.Privileged),

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
