using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Implem.PleasanterTest.Tests.Items
{
    public class PermissionsIndex
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            UserModel userModel,
            PermissionData.PermissionInitTypes permissionInitType,
            int deptId,
            int groupId,
            int userId,
            Permissions.Types permissionType,
            List<BaseTest> baseTests)
        {
            InitPermissions(
                permissionInitType: permissionInitType,
                deptId: deptId,
                groupId: groupId,
                userId: userId,
                permissionType: permissionType);
            var id = Initializer.Titles.Get(title);
            var siteId = Initializer.ItemIds.Get(id).SiteId;
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.ItemsIndex(id: siteId));
            var results = Results(context: context);
            Initializer.SaveResults(results);
            Assert.True(Tester.Test(
                context: context,
                results: results,
                baseTests: baseTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            foreach (var referenceType in ItemData.GetReferenceTypes().Where(o => o != ItemData.ReferenceTypes.Sites))
            {
                var title = $"PermissionsTest{referenceType}1";
                foreach (PermissionData.PermissionIdTypes permissionIdType in Enum.GetValues(typeof(PermissionData.PermissionIdTypes)))
                {
                    foreach (var userType in UserData.GetUserTypePatterns())
                    {
                        var userModel = UserData.Get(userType: userType);
                        var ids = PermissionData.PermissionIds(
                            permissionIdType: permissionIdType,
                            userId: userModel.UserId);
                        foreach (var permissionInitType in PermissionData.GetPermissionInitTypes())
                        {
                            yield return TestData(
                                title: title,
                                userModel: userModel,
                                permissionInitType: permissionInitType,
                                deptId: ids.DeptId,
                                groupId: ids.GroupId,
                                userId: ids.UserId,
                                permissionType: (Permissions.Types)31,
                                baseTests: GetHtmlTests(
                                    title: title,
                                    userType: userType,
                                    referenceType: referenceType,
                                    permissionIdType: permissionIdType,
                                    permissionInitType: permissionInitType));
                        }
                    }
                }
            }
        }

        private static List<BaseTest> GetHtmlTests(
            string title,
            UserData.UserTypes userType,
            ItemData.ReferenceTypes referenceType,
            PermissionData.PermissionIdTypes permissionIdType,
            PermissionData.PermissionInitTypes permissionInitType)
        {
            // テーブルの種類でスイッチ
            switch (referenceType)
            {
                // Wikiのレコードのindexは存在しないためNotFound
                case ItemData.ReferenceTypes.Wikis:
                    return HtmlTestNotFoundMessage(title: title);
                default:
                    // 権限の種類でスイッチ
                    switch (permissionInitType)
                    {
                        // アクセス権無しの場合にはNotFound
                        // レコードのアクセス権のみの場合にはサイトにアクセスできないためNotFound
                        // 特権ユーザはアクセス権に関係なくアクセスできるためExists
                        case PermissionData.PermissionInitTypes.Nothing:
                        case PermissionData.PermissionInitTypes.RecordAllow:
                            return userType == UserData.UserTypes.Privileged
                                ? HtmlTestExistsOne(title: title)
                                : HtmlTestNotFoundMessage(title: title);
                        // サイトへのエントリー権限(PermissionType:0)が存在する場合
                        case PermissionData.PermissionInitTypes.SiteEntry:
                            // ユーザの種類でスイッチ
                            switch (userType)
                            {
                                // 無効化された組織のユーザの場合
                                case UserData.UserTypes.DisabledDept:
                                    // アクセス付与対象でスイッチ
                                    switch (permissionIdType)
                                    {
                                        // 無効化された組織またはそれを含むグループではアクセスできないためNotFound
                                        case PermissionData.PermissionIdTypes.Dept:
                                        case PermissionData.PermissionIdTypes.DeptGroup:
                                            return HtmlTestNotFoundMessage(title: title);
                                        // ユーザのアクセス権またはグループのアクセス権では一覧は表示できるが対象の行が表示されないためEntryAndNoRow
                                        default:
                                            return HtmlTestEntryAndNoRow(title: title);
                                    }
                                // 無効化されたグループのユーザの場合アクセス付与対象がグループの場合はNotFound
                                // 上記グループ以外の場合、一覧は表示できるが対象の行が表示されないためEntryAndNoRow
                                case UserData.UserTypes.DisabledGroup:
                                    return permissionIdType == PermissionData.PermissionIdTypes.Group
                                        ? HtmlTestNotFoundMessage(title: title)
                                        : HtmlTestEntryAndNoRow(title: title);
                                // 無効化またはロックされたユーザの場合はアクセスできないためNotFound
                                case UserData.UserTypes.Disabled:
                                case UserData.UserTypes.Lockout:
                                    return HtmlTestNotFoundMessage(title: title);
                                // 特権ユーザはアクセス権に関係なくアクセスできるためExists
                                case UserData.UserTypes.Privileged:
                                    return HtmlTestExistsOne(title: title);
                                // 一般ユーザでは一覧は表示できるが対象の行が表示されないためEntryAndNoRow
                                default:
                                    return HtmlTestEntryAndNoRow(title: title);
                            }
                        // サイトのアクセス権または継承されたサイトのアクセス権がある場合
                        // またはサイトのエントリー権限とレコードのアクセス権がある場合
                        default:
                            // ユーザの種類でスイッチ
                            switch (userType)
                            {
                                // 無効化された組織のユーザの場合
                                case UserData.UserTypes.DisabledDept:
                                    // アクセス付与対象でスイッチ
                                    switch (permissionIdType)
                                    {
                                        // 無効化された組織またはそれを含むグループではアクセスできないためNotFound
                                        case PermissionData.PermissionIdTypes.Dept:
                                        case PermissionData.PermissionIdTypes.DeptGroup:
                                            return HtmlTestNotFoundMessage(title: title);
                                        // ユーザのアクセス権またはグループのアクセス権ではアクセスできるためExists
                                        default:
                                            return HtmlTestExistsOne(title: title);
                                    }
                                // 無効化されたグループのユーザの場合アクセス付与対象がグループの場合はNotFound
                                // 上記グループ以外の場合アクセスできるためExists
                                case UserData.UserTypes.DisabledGroup:
                                    return permissionIdType == PermissionData.PermissionIdTypes.Group
                                        ? HtmlTestNotFoundMessage(title: title)
                                        : HtmlTestExistsOne(title: title);
                                // 無効化またはロックされたユーザの場合はアクセスできないためNotFound
                                case UserData.UserTypes.Disabled:
                                case UserData.UserTypes.Lockout:
                                    return HtmlTestNotFoundMessage(title: title);
                                // 特権ユーザはアクセス権に関係なくアクセスできるためExists
                                case UserData.UserTypes.Privileged:
                                    return HtmlTestExistsOne(title: title);
                                // 一般ユーザはアクセスできるためExists
                                default:
                                    return HtmlTestExistsOne(title: title);
                            }
                    }
            }
        }

        private static object[] TestData(
            string title,
            UserModel userModel,
            PermissionData.PermissionInitTypes permissionInitType,
            int deptId,
            int groupId,
            int userId,
            Permissions.Types permissionType,
            List<BaseTest> baseTests)
        {
            return new object[]
            {
                title,
                userModel,
                permissionInitType,
                deptId,
                groupId,
                userId,
                permissionType,
                baseTests
            };
        }

        private static string Results(Context context)
        {
            var itemModel = Initializer.ItemIds.Get(context.Id);
            return itemModel.Index(context: context);
        }

        private static List<BaseTest> HtmlTestExistsOne(string title)
        {
            var id = Initializer.Titles.Get(title);
            var baseTests = BaseData.Tests(HtmlData.ExistsOne(selector: SelectorUtilities.GridRow(id)));
            return baseTests;
        }

        private static List<BaseTest> HtmlTestNotFoundMessage(string title)
        {
            var id = Initializer.Titles.Get(title);
            var baseTests = BaseData.Tests(HtmlData.NotFoundMessage());
            return baseTests;
        }

        private static List<BaseTest> HtmlTestEntryAndNoRow(string title)
        {
            var id = Initializer.Titles.Get(title);
            var baseTests = BaseData.Tests(
                HtmlData.ExistsOne(selector: "#Grid"),
                HtmlData.NotExists(selector: SelectorUtilities.GridRow(id)));
            return baseTests;
        }

        private static void InitPermissions(
            PermissionData.PermissionInitTypes permissionInitType,
            int deptId = 0,
            int groupId = 0,
            int userId = 0,
            Permissions.Types permissionType = (Permissions.Types)31)
        {
            PermissionData.Init(
                title: "アクセス権のテスト",
                permissionInitType: permissionInitType,
                deptId: deptId,
                groupId: groupId,
                userId: userId,
                permissionType: permissionType);
        }
    }
}
