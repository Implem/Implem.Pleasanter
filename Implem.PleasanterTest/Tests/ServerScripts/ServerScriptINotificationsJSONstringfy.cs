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
using Implem.Pleasanter.Libraries.ServerScripts;
using System.IO;
using System.Collections;
using CsvHelper.Configuration;
using Implem.Pleasanter.Libraries.DataTypes;
using System;

//下記サーバースクリプトのテストを行います。
/*
// 事前準備
//

// 対象機能
// 1. notifications.Get
// 2. notifications.New
// 3. notification.Send
// 4. $p.JSON.stringify

// 処理概要
// 1-1. 「notifications.Get()」で設定されている通知情報を取得
// 1-2. 設定されている「タイトル」を確認
// 2-1. 「notifications.New()」で通知オブジェクトを生成
// 2-2. オブジェクトが設定されているか確認
// 3-1. 「2-1.」で設定した通知オブジェクトをもとに「notification.Send()」で通知
// 3-2. 返却された値(2024/09/09時点：true)を確認
// 4-1. 「$p.JSON.stringify()」で引数の値を文字列にシリアライズ
// 4-2. 「typeof演算子」で値が「文字列(string)」であるか確認

// 変数宣言箇所
const logClass = { DEBUG: '【DEBUG】', INFO: '【INFO】' };
var targetFunction;
var result;

try {
    // 実装箇所
    context.Log(logClass['DEBUG'] + 'xUnit処理：開始');

    // 1. notifications.Get
    targetFunction = 'notifications.Get()';
    // 1-1. 「notifications.Get()」で設定されている通知情報を取得
    const response = notifications.Get(1);
    // 1-2. 設定されている「タイトル」を確認
    result = response.Title === 'notifications.Get()';
    judgeResult(result, targetFunction);

    // 2. notifications.New
    targetFunction = 'notifications.New()';
    // 2-1. 「notifications.New()」で通知オブジェクトを生成
    var notificationObject = notifications.New();
    // 2-2. オブジェクトが設定されているか確認
    judgeResult(notificationObject, targetFunction);

    // 3. notification.Send
    targetFunction = 'notifications.Send()';
    // 3-1. 「2-1.」で設定した通知オブジェクトをもとに「notification.Send()」で通知
    notificationObject.Address = 'urata@implem.co.jp';
    notificationObject.Title = 'notifications.Send()';
    // 3-2. 返却された値(2024/09/09時点：true)を確認
    var result = notificationObject.Send();
    judgeResult(result, targetFunction);

    // 4. $p.JSON.stringify
    targetFunction = '$p.JSON.stringify()';
    // 4-1. 「$p.JSON.stringify()」で引数の値を文字列にシリアライズ
    var stringSerializedValue = $p.JSON.stringify(123);
    // 4-2. 「typeof演算子」で値が「文字列(string)」であるか確認
    result = typeof stringSerializedValue === 'string';
    judgeResult(result, targetFunction);
} catch (ex) {
    context.Log(logClass['DEBUG'] + targetFunction + 'xUnit処理：エラー');
    context.Log(ex.stack);
} finally {
    context.Log(logClass['DEBUG'] + 'xUnit処理：終了');
}

// 成功 or 失敗を判定
//  - 引数 -
// condition：判定条件
// targetFunction：対象機能名
// - 戻値 -
// true：成功, false：失敗
function judgeResult(condition, targetFunction) {
    context.Log(logClass['DEBUG'] + targetFunction + '判定処理：開始');
    try {
        // 成功メッセージ
        const successMessage = '：成功';
        // 失敗メッセージ
        const failureMessage = '：失敗';
        // 成功CSS
        const successCSS = 'alert-information';
        // 失敗CSS
        const failureCSS = 'alert-error';
        context.Log(logClass['INFO'] + condition);
        if (condition) {
            context.Log(logClass['DEBUG'] + '判定結果：' + successMessage);
            context.AddMessage(targetFunction + successMessage, successCSS);
        } else {
            context.Log(logClass['DEBUG'] + '判定結果：' + failureMessage);
            context.AddMessage(targetFunction + failureMessage, failureCSS);
        }
    } catch (ex) {
        context.Log(logClass['DEBUG'] + targetFunction + '判定処理：エラー');
        context.Log(ex.stack);
    } finally {
        context.Log(logClass['DEBUG'] + targetFunction + '判定処理：終了');
    }
}

*/

namespace Implem.PleasanterTest.Tests.ServerScript
{
    [Collection(nameof(ServerScriptView))]
    public class ServerScriptINotificationsJSONstringfy
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
                var xUnitMessage = BaseData.Tests(
                    HtmlData.HasInformationMessage(message: "notifications.Get()：成功"),
                    HtmlData.HasInformationMessage(message: "notifications.New()：成功"),
                    HtmlData.HasInformationMessage(message: "notifications.Send()：成功"),
                    HtmlData.HasInformationMessage(message: "$p.JSON.stringify()：成功")
                    );
            var testParts = new List<TestPart>()
            {
                new TestPart(title: "notificationsと$p.JSON.stringify", baseTests: xUnitMessage,userType: UserData.UserTypes.Privileged)

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

