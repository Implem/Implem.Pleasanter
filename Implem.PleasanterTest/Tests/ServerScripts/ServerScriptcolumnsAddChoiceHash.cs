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
try{
    for (let i = 1; i <= 3; i++) {
        columns.ClassA.AddChoiceHash(i, 'TEST' + i);
    }
    columns.ClassB.ClearChoiceHash();
}catch{
    context.Log(ex.stack);
}
*/

namespace Implem.PleasanterTest.Tests.ServerScript
{
    public class ServerScriptcolumnsAddChoiceHash
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
            Assert.True(Tester.Test(
                context: context,
                results: results,
                baseTests: baseTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            string htmlClassA = "<option value=\"\" selected=\"selected\">&nbsp;</option><option value=\"1\">TEST1</option><option value=\"2\">TEST2</option><option value=\"3\">TEST3</option>";
            string htmlClassB = "<option value=\"\" selected=\"selected\">&nbsp;</option>";
            var hasMessages = BaseData.Tests(
                HtmlData.InnerHtml(selector: "#Results_ClassA",value: htmlClassA),
                HtmlData.InnerHtml(selector: "#Results_ClassB", value: htmlClassB));

            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "サーバスクリプト-columnsAddChoiceHash",
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
            return itemModel.New(context: context);
        }
    }
}
