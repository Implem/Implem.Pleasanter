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
using Newtonsoft.Json.Linq;
using Implem.Pleasanter.Libraries.ServerScripts;
using System.ComponentModel.DataAnnotations;
using Implem.Pleasanter.Libraries.Resources;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.VisualBasic;
using static System.Net.Mime.MediaTypeNames;
using System;

//下記サーバースクリプトのテストを行います。
/*
サーバスクリプト①：UserDataBefore
context.UserData.MyData = 'HAYATO'; 

サーバスクリプト②：UserDataAfter
if (context.UserData.MyData === 'HAYATO') {
    context.AddMessage('OK','alert-information');
} else {
    context.AddMessage('NG','alert-information');
}
*/

namespace Implem.PleasanterTest.Tests.ServerScript
{
    [Collection(nameof(ServerScriptContextUserData))]
    public class ServerScriptContextUserData
    {

        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            var resultId = Initializer.Results.Get(title).Get("ResultsA").ResultId;
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.ItemsUpdate(id: resultId));

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
            var baseTests = new List<BaseTest>()
            {
                JsonData.Value(
                    method: "Log",
                    value: "OK\r\n")
            };
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "xUnit_contextUserData",
                    baseTests: baseTests,
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
            return itemModel.Update(context: context);
        }
    }
}
