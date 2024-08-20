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
if (context.Action === 'index') {
    try {
        context.Log('【DEBUG】xUnit事前処理：開始');
        // データ
        const data = [
            { No: 1, Title: '野原家の中', Status: 900, NumA: 1 },
            { No: 2, Title: '部屋の中の野原', Status: 200, NumA: 2 },
            { No: 3, Title: 'インプリム：中野', Status: 200, NumA: 3 },
            { No: 4, Title: '東京都中野区', Status: 200, NumA: 4 },
            { No: 5, Title: '中野区', Status: 100, NumA: 5 },
            { No: 6, Title: '中野', Status: 100, NumA: 6 },
            { No: 7, Title: '中 野', Status: 100, NumA: 7 },
        ];
        createRecord(context.SiteId, data);
        context.Log('【DEBUG】xUnit事前処理：終了');
        context.Log('【DEBUG】xUnit処理：開始');
        // 対象機能
        // 1. view.Id
        // 2. view.Sorters
        // 3. view.Filters
        // 4. view.SearchTypes
        // 5. view.FilterNegative
        // 6. view.ClearFilters
        // 7. view.FiltersCleared

        // 処理概要
        // 1-1.「view.Id」で選択しているビューIDを確認
        // 2-1.「view.Sorters()」で「数値A(NumA)：昇順」をもとにデータを並び替え
        // 2-2.「items.Get()」で並び替えしたデータを確認
        // 3-1.「view.Filters」で「タイトル(Title)：中野」をもとにデータを絞り込み
        // 3-2.「items.Get()」で絞り込みしたデータを確認
        // 4-1.「view.SearchTypes()」で「タイトル(Title)：完全一致」をもとにデータを検索
        // 4-2.「items.Get()」で検索したデータを確認
        // 5-1.「view.FilterNegative()」で「タイトル(Title)：中野」をもとにデータの絞り込み
        // 5-2.「items.Get()」で絞り込みしたデータを確認
        // 6-1.「view.ClearFilters()」でデータの絞り込みを解除
        // 6-2.「items.Get()」で絞り込みを解除したデータを確認
        // 7-1.「view.FiltersCleared()」で「view.ClearFilters()」が実行されていることを確認

        // サーバスクリプト(ID：1)：ServerScriptWhenViewProcessing
        // ロジック箇所
        // 1. view.Id
        // 1-1.「view.Id」で選択しているビューIDを確認
        judgeResult('view.Id === 0', 'view.Id');

        // 2. view.Sorters
        // 2-1.「view.Sorters()」で「数値A(NumA)：昇順」をもとにデータを並び替え
        view.Sorters.NumA = 'desc'; // 7件
        // 2-2.「items.Get()」で並び替えしたデータを確認
        judgeResult('items.Get(2981).Length === 7', 'view.Sorters');

        // 3. view.Filters
        // 3-1.「view.Filters」で「タイトル(Title)：中野」をもとにデータを絞り込み
        view.Filters.Title = '中野'; // 4件
        // 3-2.「items.Get()」で絞り込みしたデータを確認
        judgeResult('items.Get(2981).Length === 4', 'view.Filters');

        // 4. view.SearchTypes
        // 4-1.「view.SearchTypes()」で「タイトル(Title)：完全一致」をもとにデータを検索
        view.SearchTypes.Title = 'ExactMatch'; // 1件
        // 4-2.「items.Get()」で検索したデータを確認
        judgeResult('items.Get(2981).Length === 1', 'view.SearchTypes');

        // 5. view.FilterNegative
        // 5-1.「view.FilterNegative()」で「タイトル(Title)：中野」をもとにデータの絞り込み
        view.FilterNegative('Title'); // 6件
        // 5-2.「items.Get()」で絞り込みしたデータを確認
        judgeResult('items.Get(2981).Length === 6', 'view.FilterNegative');

        // 6. view.ClearFilters
        // 6-1.「view.ClearFilters()」でデータの絞り込みを解除
        view.ClearFilters(); // 7件
        // 6-2.「items.Get()」で絞り込みを解除したデータを確認
        judgeResult('items.Get(2981).Length === 7', 'view.ClearFilters');

        // 7. view.FiltersCleared
        // 7-1.「view.FiltersCleared()」で「view.ClearFilters()」が実行されていることを確認
        judgeResult('view.FiltersCleared === true', 'view.ClearFilters');

        // 2. view.Sorters
    } catch (ex) {
        context.Log('【DEBUG】xUnit処理：実装エラー');
        context.Log(ex.stack);
    } finally {
        context.Log('【DEBUG】xUnit処理：終了');
    }

    function createRecord(siteId, data) {
        context.Log('【DEBUG】データ作成処理：開始');
        try {
            for (const datum of data) {
                const apiModel = items.NewResult();
                apiModel.Title = datum['Title'];
                apiModel.Status = datum['Status'];
                apiModel.NumA = datum['NumA'];
                apiModel.Create(siteId);
            }
        } catch (ex) {
            context.Log('【DEBUG】データ作成処理：実装エラー');
            context.Log(ex.stack);
        } finally {
            context.Log('【DEBUG】データ作成処理：終了');
        }
    }

    // 成功 or 失敗を判定
    //  - 引数 -
    // condition：判定条件
    // targetFunction：対象機能名
    function judgeResult(condition, targetFunction) {
        context.Log('【DEBUG】' + targetFunction + '判定処理：開始');
        try {
            // 成功メッセージ
            const successMessage = '：成功';
            // 失敗メッセージ
            const failureMessage = '：失敗';
            // 成功CSS
            const successCSS = 'alert-information';
            // 失敗CSS
            const failureCSS = 'alert-error';
            context.Log('【INFO 】条件：' + condition);
            if (condition) {
                context.Log('【DEBUG】判定結果：成功');
                context.AddMessage(targetFunction + successMessage, successCSS);
            } else {
                context.Log('【DEBUG】判定結果：失敗');
                context.AddMessage(targetFunction + failureMessage, failureCSS);
            }
        } catch (ex) {
            context.Log('【DEBUG】' + targetFunction + '判定処理：実装エラー');
            context.Log(ex.stack);
        } finally {
            context.Log('【DEBUG】' + targetFunction + '判定処理：終了');
        }
    }
}
*/

namespace Implem.PleasanterTest.Tests.ServerScript
{
    public class ServerScriptView
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
            Assert.True(Tester.Test(
                context: context,
                results: results,
                baseTests: baseTests));

        }

        public static IEnumerable<object[]> GetData()
        {
            var hasMessages = BaseData.Tests(
                HtmlData.HasInformationMessage(message: "view.Id：成功"),
                HtmlData.HasInformationMessage(message: "view.Sorters：成功"),
                HtmlData.HasInformationMessage(message: "view.Filters：成功"),
                HtmlData.HasInformationMessage(message: "view.SearchTypes：成功"),
                HtmlData.HasInformationMessage(message: "view.FilterNegative：成功"),
                HtmlData.HasInformationMessage(message: "view.ClearFilters：成功"),
                HtmlData.HasInformationMessage(message: "view.ClearFilters：成功"));
            var testParts = new List<TestPart>()
            {
                new TestPart(title: "ServerScript - View", baseTests: hasMessages,userType: UserData.UserTypes.Privileged),

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

