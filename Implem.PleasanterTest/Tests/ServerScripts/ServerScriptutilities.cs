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
using System;

//下記サーバースクリプトのテストを行います。
/*
try{
    model.Title =  utilities.ConvertToBase64String("プリザンター");
    model.DateA = utilities.Today();
    if (utilities.InRange(model.DateA)) {
        context.Log('OK');
    }
}catch{
    context.Log(ex.stack);
}
*/

namespace Implem.PleasanterTest.Tests.ServerScript
{
    public class ServerScriptutilities
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
        }

        public static IEnumerable<object[]> GetData()
        {
            string encodeBase64 = "44OX44Oq44K244Oz44K/44O8";
            string today = DateTime.Today.ToString("yyyy/MM/dd HH:mm:ss");
            string newLineEscaped = System.Environment.NewLine == "\r\n" ? "\\r\\n" : "\\n";
            string logValue = $"{{\"Log\":\"OK{newLineEscaped}\"}}";
            var hasMessages = BaseData.Tests(
                HtmlData.Attribute(name: "value", selector: "#Results_Title", value: encodeBase64),
                HtmlData.Attribute(name: "value", selector: "#Results_DateA", value: today),
                HtmlData.Attribute(name: "value", selector: "#Log", value: logValue));

            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "サーバスクリプト-utilities",
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
