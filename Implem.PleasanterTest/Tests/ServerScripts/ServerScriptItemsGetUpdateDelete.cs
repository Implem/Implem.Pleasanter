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
    // 対象サーバスクリプト
    // ・items.Update
    // ・items.Upsert
    // ・items.Get
    // ・items.BulkDelete
    // ・items.Delete

    // 処理概要
    // 1. 「items.GetClosestSite()」で既存サイトのサイトの情報を取得
    // 2. 「items.Upsert()」でサイト情報のサイトIDをもとにレコードを新規作成
    // 3. 「items.Get()」でサイトIDをもとに一括作成したレコードの情報を取得
    // 4. 「items.Update()」でレコード情報のうち1レコードを更新
    // 5. 「items.Upsert()」でサイトIDをもとに全レコードを更新
    // 6. 「items.Delete()」でレコード情報のうち1レコードを削除
    // 7. 「items.BulkDelete()」でサイトIDをもとに全レコードを削除

    // 変数宣言箇所
    // 操作用サイト名
    const siteName = 'サイト名：items更新系 - 操作用サイト';
    // 操作用レコードのタイトル
    const recordTitle = ['items.Update(), items.Delete()で使用するレコード', 'items.Upsert(), items.BulkDelete()で使用するレコード1', 'items.Upsert(), items.BulkDelete()で使用するレコード2'];
    // 操作用レコードのレコードID
    var recordIds = [];
    // 各関数呼び出し時のデータ
    var data = {};
    // 新規作成時の状況
    const initStatus = 100;
    // 更新後の状況
    const updatedStatus = 200;
    // 対象サーバスクリプト名
    var targetFunction;
    // 成功メッセージ
    const successMessage = '：成功';
    // 失敗メッセージ
    const failureMessage = '：失敗';
    // 成功CSS
    const successCSS = 'alert-information';
    // 失敗CSS
    const failureCSS = 'alert-error';

    // ロジック箇所
    // 1. 「items.GetClosestSite()」で既存サイトのサイトの情報を取得
    const siteInfo = items.GetClosestSite(siteName);
    const targetSiteId = siteInfo.SiteId;

    // 2. 「items.Upsert()」でサイト情報のサイトIDをもとにレコードを新規作成
    targetFunction = 'items.Upsert() - Create';
    var upsertCreateResults = [];
    var upsertCreateResult = true;
    for (var Title of recordTitle) {
        data = {
            Keys: ['Title'],
            Title: Title,
            Status: initStatus
        }
        var tmpResult = items.Upsert(targetSiteId, JSON.stringify(data));
        upsertCreateResults.push(tmpResult);
    }
    for (var Result of upsertCreateResults) {
        if (Result === false) {
            upsertCreateResult = false;
            break;
        }
    }
    if (upsertCreateResult === true) {
        context.AddMessage(targetFunction + successMessage, successCSS);
    } else {
        context.AddMessage(targetFunction + failureMessage, failureCSS);
    }

    // 3. 「items.Get()」でサイトIDをもとに一括作成したレコードの情報を取得
    targetFunction = 'items.Get()';
    const getResults = items.Get(targetSiteId);
    if (getResults.Length === 3) {
        for (var getResult of getResults) recordIds.push({ResultId: getResult.Model.Title.Id, Title: getResult.Model.Title.Value});
        context.AddMessage(targetFunction + successMessage, successCSS);
    } else {
        context.AddMessage(targetFunction + failureMessage, failureCSS);
    }

    // 4. 「items.Update()」でレコード情報のうち1レコードを更新
    targetFunction = 'items.Update()';
    const targetRecordId = recordIds[2];
    const updateDeleteRecordId = targetRecordId['ResultId'];
    data = {
        Status: updatedStatus
    }
    if (items.Update(updateDeleteRecordId, JSON.stringify(data)) === true) {
        context.AddMessage(targetFunction + successMessage, successCSS);
    } else {
        context.AddMessage(targetFunction + failureMessage, failureCSS);
    }

    // 5. 「items.Upsert()」でサイトIDをもとに全レコードを更新
    targetFunction = 'items.Upsert() - Update';
    var upsertUpdateResults = [];
    var upsertUpdateResult = true;
    for (var Title of recordTitle) {
        data = {
            Keys: ['Title'],
            Title: Title,
            Status: updatedStatus
        }
        var tmpResult = items.Upsert(targetSiteId, JSON.stringify(data));
        upsertUpdateResults.push(tmpResult);
    }
    for (var Result of upsertUpdateResults) {
        if (Result === false) {
            upsertUpdateResult = false;
            break;
        }
    }
    if (upsertUpdateResult === true) {
        context.AddMessage(targetFunction + successMessage, successCSS);
    } else {
        context.AddMessage(targetFunction + failureMessage, failureCSS);
    }

    // 6. 「items.Delete()」でレコード情報のうち1レコードを削除
    targetFunction = 'items.Delete()';
    if (items.Delete(updateDeleteRecordId) === true) {
        context.AddMessage(targetFunction + successMessage, successCSS);
    } else {
        context.AddMessage(targetFunction + failureMessage, failureCSS);
    }

    // 7. 「items.BulkDelete()」でサイトIDをもとに全レコードを削除
    targetFunction = 'items.BulkDelete()';
    data = {
        View: {
            ColumnFilterHash: {
                Status: 200
            }
        }
    }
    if (items.BulkDelete(targetSiteId, JSON.stringify(data)) === 2) {
        context.AddMessage(targetFunction + successMessage, successCSS);
    } else {
        context.AddMessage(targetFunction + failureMessage, failureCSS);
    }
} catch (ex) {
    context.Log(ex.stack);
}
*/

namespace Implem.PleasanterTest.Tests.ServerScript
{
    [Collection(nameof(ServerScriptItemsGetUpdateDelete))]
    public class ServerScriptItemsGetUpdateDelete
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
                HtmlData.HasInformationMessage(message: "items.Upsert() - Create：成功"),
                HtmlData.HasInformationMessage(message: "items.Get()：成功"),
                HtmlData.HasInformationMessage(message: "items.Update()：成功"),
                HtmlData.HasInformationMessage(message: "items.Upsert() - Update：成功"),
                HtmlData.HasInformationMessage(message: "items.Delete()：成功"),
                HtmlData.HasInformationMessage(message: "items.BulkDelete()：成功"));
            var testParts = new List<TestPart>()
            {
                new TestPart(title: "items更新系 - 判定サイト", baseTests: hasMessages,userType: UserData.UserTypes.Privileged),

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
