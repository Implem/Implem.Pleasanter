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
// 事前準備
// 下記の情報が登録されていること
// ログインユーザ：1ユーザ
// 組織：1組織
// グループ：2グループ(親グループ・子グループ)
// ログインユーザは組織、親グループに属していること
// 組織にはログインユーザのみ所属していること
// グループには親グループの子グループに子グループを登録していること
// グループには組織が含まれていること

// 対象機能
// 1. users.Get
// 2. depts.Get
// 3. dept.GetMembers
// 4. groups.Get
// 5. group.GetChildren
// 6. group.GetMembers
// 7. group.ContainsChild
// 8. group.ContainsDept
// 9. group.ContainsUser
// 10. groups.Update

// 処理概要
// 1-1. 「users.Get()」でログインユーザの情報を取得
// 1-2. 取得したユーザ情報の「Name」を確認
// 2-1. 「depts.Get()」でログインユーザの所属組織情報を取得
// 2-2. 取得した組織情報の「DeptName」を確認
// 3-1. 取得した組織情報をもとに「dept.GetMembers()」でメンバーを取得
// 3-2. 取得したメンバーの「Name」を確認
// 4-1. 「groups.Get()」でログインユーザが所属しているグループ情報を取得
// 4-2. 取得したグループ情報の「GroupName」を確認
// 5-1. 取得したグループ情報をもとに「group.GetChildren()」で子グループを取得
// 5-2. 取得した子グループの「GroupName」を確認
// 6-1. 取得したグループ情報をもとに「group.GetMembers()」で所属しているメンバーを取得
// 6-2. 取得したメンバーの「UserId」を確認
// 7-1. 取得したグループ情報をもとに「group.ContainsChild()」で子グループの存在を確認
// 8-1. 取得したグループ情報をもとに「group.ContainsDept()」で組織の存在を確認
// 9-1. 取得したグループ情報をもとに「group.ContainsUser()」でユーザの存在を確認
// 10-1. 取得したグループ情報をもとに「groups.Update()」でグループのメンバーを更新
// 10-2. グループの更新可否を確認

// 変数宣言箇所
const logClass = { DEBUG: '【DEBUG】', INFO: '【INFO】' };
var targetFunction;
var result;
const loginUserId = context.UserId;
const loginUserBelongDeptId = context.DeptId;
const loginUserBelongGroupIds = context.Groups;
const userName = 'xUnitテスト用ユーザ';
const deptName = 'xUnitテスト用組織';
const groupName = 'xUnitテスト用グループ';
const groupChildName = 'xUnitテスト用グループの子グループ';

try {
    // 実装箇所
    context.Log(logClass['DEBUG'] + 'xUnit処理：開始');

    // 1. users.Get
    targetFunction = 'users.Get()';
    // 1-1. 「users.Get()」でログインユーザの情報を取得
    const loginUserInfo = users.Get(loginUserId);
    // 1-2. 取得したユーザ情報の「Name」を確認
    result = loginUserInfo.Name === userName;
    judgeResult(result, targetFunction);

    // 2. depts.Get
    targetFunction = 'depts.Get()';
    // 2-1. 「depts.Get()」でログインユーザの所属組織情報を取得
    const loginUserBelongDeptInfo = depts.Get(loginUserBelongDeptId);
    // 2-2. 取得した組織情報の「DeptName」を確認
    result = loginUserBelongDeptInfo.DeptName === deptName;
    judgeResult(result, targetFunction);

    // 3. dept.GetMembers
    targetFunction = 'dept.GetMembers()';
    // 3-1. 取得した組織情報をもとに「dept.GetMembers()」でメンバーを取得
    const loginUserBelongDeptMembers = loginUserBelongDeptInfo.GetMembers();
    for (const loginUserBelongDeptMember of loginUserBelongDeptMembers) {
        // 3-2. 取得したメンバーの「Name」を確認
        result = loginUserBelongDeptMember.Name === userName;
        judgeResult(result, targetFunction);
    }

    // 4. groups.Get
    targetFunction = 'groups.Get()';
    // 4-1. 「groups.Get()」でログインユーザが所属しているグループ情報を取得
    const loginUserBelongGroupInfo = groups.Get(loginUserBelongGroupIds[0]);
    // 4-2. 取得したグループ情報の「GroupName」を確認
    result = loginUserBelongGroupInfo.GroupName === groupName;
    judgeResult(result, targetFunction);

    // 5. group.GetChildren
    targetFunction = 'group.GetChildren()';
    // 5-1. 取得したグループ情報をもとに「group.GetChildren()」で子グループを取得
    const loginUserBelongGroupChildrenData = loginUserBelongGroupInfo.GetChildren();
    for (const loginUserBelongGroupChildrenDatum of loginUserBelongGroupChildrenData) {
        // 5-2. 取得した子グループの「GroupName」を確認
        result = loginUserBelongGroupChildrenDatum.GroupName === groupChildName;
        judgeResult(result, targetFunction);
    }

    // 6. group.GetMembers
    targetFunction = 'group.GetMembers()';
    // 6-1. 取得したグループ情報をもとに「group.GetMembers()」で所属しているメンバーを取得
    const loginUserBelongGroupMemberData = loginUserBelongGroupInfo.GetMembers();
    for (const loginUserBelongGroupMemberDatum of loginUserBelongGroupMemberData) {
        // 6-2. 取得したメンバーの「UserId」を確認
        if (loginUserBelongGroupMemberDatum.UserId === loginUserId ||loginUserBelongGroupMemberDatum.UserId === loginUserBelongDeptId) result = true;
        judgeResult(result, targetFunction);
    }

    // 7. group.ContainsChild
    targetFunction = 'group.ContainsChild()';
    for (const loginUserBelongGroupChildrenDatum of loginUserBelongGroupChildrenData) {
        // 7-1. 取得したグループ情報をもとに「group.ContainsChild()」で子グループの存在を確認
        result = loginUserBelongGroupInfo.ContainsChild(loginUserBelongGroupChildrenDatum.GroupId) === true;
        judgeResult(result, targetFunction);
    }

    // 8. group.ContainsDept
    targetFunction = 'group.ContainsDept()';
    // 8-1. 取得したグループ情報をもとに「group.ContainsDept()」で組織の存在を確認
    result = loginUserBelongGroupInfo.ContainsDept(loginUserBelongDeptId) === true;
    judgeResult(result, targetFunction);

    // 9. group.ContainsUser
    targetFunction = 'group.ContainsUser()';
    // 9-1. 取得したグループ情報をもとに「group.ContainsUser()」でユーザの存在を確認
    result = loginUserBelongGroupInfo.ContainsUser(loginUserId) === true;
    judgeResult(result, targetFunction);

    // 10. groups.Update
    targetFunction = 'groups.Update()';
    const groupMembers = [
        'User,' + loginUserId + ',true',
        'Dept,' + loginUserBelongDeptId + ',true'
]
    const data = {
        GroupMembers: groupMembers
    };
    // 10-1. 取得したグループ情報をもとに「groups.Update()」でグループのメンバーを更新
    // 10-2. グループの更新可否を確認
    result = groups.Update(loginUserBelongGroupIds[0], JSON.stringify(data));
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
    [Collection(nameof(ServerScriptUsersDeptsGroups))]
    public class ServerScriptUsersDeptsGroups
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
                HtmlData.HasInformationMessage(message: "users.Get()：成功"),
                HtmlData.HasInformationMessage(message: "depts.Get()：成功"),
                HtmlData.HasInformationMessage(message: "dept.GetMembers()：成功"),
                HtmlData.HasInformationMessage(message: "groups.Get()：成功"),
                HtmlData.HasInformationMessage(message: "group.GetChildren()：成功"),
                HtmlData.HasInformationMessage(message: "group.GetMembers()：成功"),
                HtmlData.HasInformationMessage(message: "group.GetMembers()：成功"),
                HtmlData.HasInformationMessage(message: "group.ContainsChild()：成功"),
                HtmlData.HasInformationMessage(message: "group.ContainsDept()：成功"),
                HtmlData.HasInformationMessage(message: "group.ContainsUser()：成功"),
                HtmlData.HasInformationMessage(message: "groups.Update()：成功"));
            var testParts = new List<TestPart>()
            {
                new TestPart(title: "サーバスクリプト-UsersDeptsGroups", baseTests: hasMessages,userType: UserData.UserTypes.UserDeptsGroups),

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
