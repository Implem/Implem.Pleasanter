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
// 本xUnitは異なるサイト(計：5テーブル)を用意し、それぞれに異なる条件のサーバスクリプト(2~3つ)を実装

// サイト1
// 対象機能
// 1. view.Sorters
// 1. view.AlwaysGetColumns
// 1. view.Id

// 処理概要
// 1-1.「条件：ビュー処理時」で「view.Sorters」により「数値A(NumA)：昇順」をもとにデータを並び替え
// 1-2.「条件：ビュー処理時」で「view.AlwaysGetColumns()」により一覧に表示されていない「分類A：ClassA」を追加
// 1-3.「条件：行表示の前」で「model.ClassA」に値が設定されいているか確認
// 1-4.「条件：画面表示の前」で「items.Get()」により並び替えしたデータを確認
// 1-5.「条件：画面表示の前」で「view.Id」により選択しているビューIDを確認

try {
    context.Log('【DEBUG】xUnit処理：開始');
    var result;
    const siteId = context.SiteId;
    var tmp;
    var data = '';
    // サーバスクリプト(ID：1)：ServerScriptWhenViewProcessing
    // 1-1.「条件：ビュー処理時」で「view.Sorters」により「数値A(NumA)：昇順」をもとにデータを並び替え
    view.Sorters.NumA = 'asc';
    // 1-2.「条件：ビュー処理時」で「view.AlwaysGetColumns()」により一覧に表示されていない「分類A：ClassA」を追加
    view.AlwaysGetColumns.Add('ClassA');
    // サーバスクリプト(ID：2)：ServerScriptBeforeOpeningRow
    // 1-3.「条件：行表示の前」で「model.ClassA」に値が設定されいているか確認
    targetFunction = 'view.AlwaysGetColumns()';
    result = (typeof model.ClassA !== 'undefined');
    judgeResult(result, targetFunction);
    // サーバスクリプト(ID：3)：ServerScriptBeforeOpeningPage
    // 1-4.「条件：画面表示の前」で「items.Get()」により並び替えしたデータを確認
    targetFunction = 'view.Sorters';
    tmp = items.Get(siteId);
    for (const datum of tmp) data += datum.NumA;
    result = data === '1234567';
    judgeResult(result, targetFunction);
    // 1-5.「条件：画面表示の前」で「view.Id」により選択しているビューIDを確認
    targetFunction = 'view.Id';
    result = view.Id === 0;
    judgeResult(result, targetFunction);

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
} catch (ex) {
    context.Log('【DEBUG】xUnit処理：実装エラー');
    context.Log(ex.stack);
} finally {
    context.Log('【DEBUG】xUnit処理：終了');
}

// サイト2
// 対象機能
// 2. view.Filters

// 処理概要
// 2-1.「条件：ビュー処理時」で「view.Filters」により「タイトル(Title)：中野」をもとにデータを絞り込み
// 2-2.「条件：画面表示の前」で「items.Get()」により絞り込みしたデータを確認

try {
    context.Log('【DEBUG】xUnit処理：開始');
    var result;
    const siteId = context.SiteId;
    var tmp;
    var data = '';
    // サーバスクリプト(ID：1)：ServerScriptWhenViewProcessing
    // 2-1.「条件：ビュー処理時」で「view.Filters」により「タイトル(Title)：中野」をもとにデータを絞り込み
    view.Filters.Title = '中野';
    // サーバスクリプト(ID：2)：ServerScriptBeforeOpeningPage
    // 2-2.「条件：画面表示の前」で「items.Get()」により絞り込みしたデータを確認
    targetFunction = 'view.Filters';
    result = items.Get(siteId).Length === 4;
    judgeResult(result, targetFunction);

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
} catch (ex) {
    context.Log('【DEBUG】xUnit処理：実装エラー');
    context.Log(ex.stack);
} finally {
    context.Log('【DEBUG】xUnit処理：終了');
}

// サイト3
// 対象機能
// 3. view.SearchTypes

// 処理概要
// 3-1.「条件：ビュー処理時」で「view.SearchTypes」により「タイトル(Title)：完全一致」をもとにデータを検索
// 3-2.「条件：画面表示の前」で「items.Get()」により検索したデータを確認

try {
    context.Log('【DEBUG】xUnit処理：開始');
    var result;
    const siteId = context.SiteId;
    var tmp;
    var data = '';
    // サーバスクリプト(ID：1)：ServerScriptWhenViewProcessing
    // 3-1.「条件：ビュー処理時」で「view.SearchTypes」により「タイトル(Title)：完全一致」をもとにデータを検索
    view.Filters.Title = '中野';
    view.SearchTypes.Title = 'ExactMatch';
    // サーバスクリプト(ID：2)：ServerScriptBeforeOpeningPage
    // 2-2.「条件：画面表示の前」で「items.Get()」により絞り込みしたデータを確認
    targetFunction = 'view.SearchTypes';
    result = items.Get(siteId).Length === 1;
    judgeResult(result, targetFunction);

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
} catch (ex) {
    context.Log('【DEBUG】xUnit処理：実装エラー');
    context.Log(ex.stack);
} finally {
    context.Log('【DEBUG】xUnit処理：終了');
}

// サイト4
// 対象機能
// 4. view.FilterNegative

// 処理概要
// 4-1.「条件：ビュー処理時」で「view.FilterNegative()」により「タイトル(Title)：中野」をもとにデータの絞り込み
// 4-2.「条件；画面表示の前」で「items.Get()」により絞り込みしたデータを確認

try {
    context.Log('【DEBUG】xUnit処理：開始');
    var result;
    const siteId = context.SiteId;
    var tmp;
    var data = '';
    // サーバスクリプト(ID：1)：ServerScriptWhenViewProcessing
    // 4-1.「条件：ビュー処理時」で「view.FilterNegative()」により「タイトル(Title)：中野」をもとにデータの絞り込み
    view.Filters.Title = '中野';
    view.SearchTypes.Title = 'ExactMatch';
    view.FilterNegative('Title');
    // サーバスクリプト(ID：2)：ServerScriptBeforeOpeningPage
    // 4-2.「条件；画面表示の前」で「items.Get()」により絞り込みしたデータを確認
    targetFunction = 'view.FilterNegative()';
    result = items.Get(siteId).Length === 6;
    judgeResult(result, targetFunction);

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
} catch (ex) {
    context.Log('【DEBUG】xUnit処理：実装エラー');
    context.Log(ex.stack);
} finally {
    context.Log('【DEBUG】xUnit処理：終了');
}

// サイト5
// 対象機能
// 5. view.ClearFilters
// 5. view.FiltersCleared

// 処理概要
// 5-1.「条件：ビュー処理時」で「view.ClearFilters()」によりデータの絞り込みを解除
// 5-2.「条件：ビュー処理時」で「view.FiltersCleared()」により「view.ClearFilters()」が実行されていることを確認
// 5-3.「条件：画面表示の前」で「items.Get()」により絞り込みを解除したデータを確認

try {
    context.Log('【DEBUG】xUnit処理：開始');
    var result;
    const siteId = context.SiteId;
    var tmp;
    var data = '';
    // サーバスクリプト(ID：1)：ServerScriptWhenViewProcessing
    // 5-1.「条件：ビュー処理時」で「view.ClearFilters()」によりデータの絞り込みを解除
    view.ClearFilters();
    // 5-2.「条件：ビュー処理時」で「view.FiltersCleared」により「view.ClearFilters()」が実行されていることを確認
    targetFunction = 'view.FiltersCleared';
    result = view.FiltersCleared === true;
    judgeResult(result, targetFunction);
    // サーバスクリプト(ID：2)：ServerScriptBeforeOpeningPage
    // 4-2.「条件；画面表示の前」で「items.Get()」により絞り込みしたデータを確認
    targetFunction = 'view.ClearFilters()';
    result = items.Get(siteId).Length === 7;
    judgeResult(result, targetFunction);

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
} catch (ex) {
    context.Log('【DEBUG】xUnit処理：実装エラー');
    context.Log(ex.stack);
} finally {
    context.Log('【DEBUG】xUnit処理：終了');
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
            Initializer.SaveResults(results);
			Assert.True(Tester.Test(
                context: context,
                results: results,
                baseTests: baseTests));

        }

        public static IEnumerable<object[]> GetData()
        {
            // 「Id：Site_SScript_View_Sorters_AlwaysGetColumns_Id」を対象としたメッセージの定義
            var messageListSortersAlwaysGetColumnsId = new List<string> {
                "view.Sorters：成功",
                "view.Id：成功",
                "view.AlwaysGetColumns()：成功",
                "view.AlwaysGetColumns()：成功",
                "view.AlwaysGetColumns()：成功",
                "view.AlwaysGetColumns()：成功",
                "view.AlwaysGetColumns()：成功",
                "view.AlwaysGetColumns()：成功",
                "view.AlwaysGetColumns()：成功"
            };
            // 「Id：Site_SScript_View_Sorters_AlwaysGetColumns_Id」のテスト確認用メッセージの設定
            var hasMessagesSortersAlwaysGetColumnsId = new List<BaseTest> { };
            foreach (var message in messageListSortersAlwaysGetColumnsId)
            {
                hasMessagesSortersAlwaysGetColumnsId = BaseData.Tests(
                    HtmlData.HasInformationMessage(message: message));
            }
            // 「Id：Site_SScript_View_Filters」を対象としたメッセージの設定
            var hasMessagesFilters = BaseData.Tests(
                HtmlData.HasInformationMessage(message: "view.Filters：成功"));
            // 「Id：Site_SScript_View_SearchTypes」を対象としたメッセージの設定
            var hasMessagesSearchTypes = BaseData.Tests(
                HtmlData.HasInformationMessage(message: "view.SearchTypes：成功"));
            // 「Id：Site_SScript_View_FilterNegative」を対象としたメッセージの設定
            var hasMessagesFilterNegative = BaseData.Tests(
                HtmlData.HasInformationMessage(message: "view.FilterNegative()：成功"));
            // 「Id：Site_SScript_View_ClearFilters_FiltersCleared」を対象としたメッセージの定義
            var messageListClearFiltersFiltersCleared = new List<string> {
                "view.FiltersCleared：成功",
                "view.FiltersCleared：成功",
                "view.ClearFilters()：成功",
            };
            // 「Id：Site_SScript_View_ClearFilters_FiltersCleared」のテスト確認用メッセージの設定
            var hasMessagesClearFiltersFiltersCleared = new List<BaseTest> { };
            foreach (var message in messageListClearFiltersFiltersCleared)
            {
                hasMessagesClearFiltersFiltersCleared = BaseData.Tests(
                    HtmlData.HasInformationMessage(message: message));
            }
            var testParts = new List<TestPart>()
            {
                new TestPart(title: "View_Sorters_AlwaysGetColumns_Id", baseTests: hasMessagesSortersAlwaysGetColumnsId,userType: UserData.UserTypes.Privileged),
                new TestPart(title: "View_Filters", baseTests: hasMessagesFilters,userType: UserData.UserTypes.Privileged),
                new TestPart(title: "View_SearchTypes", baseTests: hasMessagesSearchTypes,userType: UserData.UserTypes.Privileged),
                new TestPart(title: "View_FilterNegative", baseTests: hasMessagesFilterNegative,userType: UserData.UserTypes.Privileged),
                new TestPart(title: "View_ClearFilters_FiltersCleared", baseTests: hasMessagesClearFiltersFiltersCleared,userType: UserData.UserTypes.Privileged)

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
            var data = new[] {
                "{\"Title\":\"野原家の中\",\"Status\":900,\"NumHash\":{\"NumA\":1},\"ClassHash\":{\"ClassA\":\"あいうえお\"}}",
                "{\"Title\":\"部屋の中の野原\",\"Status\":200,\"NumHash\":{\"NumA\":2},\"ClassHash\":{\"ClassA\":\"かきくけこ\"}}",
                "{\"Title\":\"インプリム：中野\",\"Status\":200,\"NumHash\":{\"NumA\":3},\"ClassHash\":{\"ClassA\":\"さしすせそ\"}}",
                "{\"Title\":\"東京都中野区\",\"Status\":200,\"NumHash\":{\"NumA\":4},\"ClassHash\":{\"ClassA\":\"たちつてと\"}}",
                "{\"Title\":\"中野区\",\"Status\":200,\"NumHash\":{\"NumA\":5},\"ClassHash\":{\"ClassA\":\"なにぬねの\"}}",
                "{\"Title\":\"中野\",\"Status\":100,\"NumHash\":{\"NumA\":6},\"ClassHash\":{\"ClassA\":\"はひふへほ\"}}",
                "{\"Title\":\"中 野\",\"Status\":100,\"NumHash\":{\"NumA\":7},\"ClassHash\":{\"ClassA\":\"まみむめも\"}}"
            };
            var serverScriptModelApiItems = new ServerScriptModelApiItems(context, false);
            foreach (string element in data)
            {
                serverScriptModelApiItems.Create(context.Id, element);
            }
            var itemModel = Initializer.ItemIds.Get(context.Id);
            return itemModel.Index(context: context);
        }
    }
}

