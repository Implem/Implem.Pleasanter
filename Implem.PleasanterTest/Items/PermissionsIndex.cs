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

namespace Implem.PleasanterTest.Items
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
            List<HtmlTest> htmlTests)
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
                routeData: RouteData.ItemsIndex(siteId: siteId));
            var html = GetHtml(context: context);
            Assert.True(Compare.Html(
                context: context,
                html: html,
                htmlTests: htmlTests));
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
                                htmlTests: GetHtmlTests(
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

        private static List<HtmlTest> GetHtmlTests(
            string title,
            UserData.UserTypes userType,
            ItemData.ReferenceTypes referenceType,
            PermissionData.PermissionIdTypes permissionIdType,
            PermissionData.PermissionInitTypes permissionInitType)
        {
            switch (referenceType)
            {
                case ItemData.ReferenceTypes.Wikis:
                    return HtmlTestNotFoundMessage(title: title);
                default:
                    switch (permissionInitType)
                    {
                        case PermissionData.PermissionInitTypes.Nothing:
                        case PermissionData.PermissionInitTypes.RecordAllow:
                            return userType == UserData.UserTypes.Privileged
                                ? HtmlTestExistsOne(title: title)
                                : HtmlTestNotFoundMessage(title: title);
                        case PermissionData.PermissionInitTypes.SiteEntry:
                            switch (userType)
                            {
                                case UserData.UserTypes.DisabledDept:
                                    switch (permissionIdType)
                                    {
                                        case PermissionData.PermissionIdTypes.Dept:
                                        case PermissionData.PermissionIdTypes.DeptGroup:
                                            return HtmlTestNotFoundMessage(title: title);
                                        default:
                                            return HtmlTestEntryAndNoRow(title: title);
                                    }
                                case UserData.UserTypes.DisabledGroup:
                                    return permissionIdType == PermissionData.PermissionIdTypes.Group
                                        ? HtmlTestNotFoundMessage(title: title)
                                        : HtmlTestEntryAndNoRow(title: title);
                                case UserData.UserTypes.Disabled:
                                case UserData.UserTypes.Lockout:
                                    return HtmlTestNotFoundMessage(title: title);
                                case UserData.UserTypes.Privileged:
                                    return HtmlTestExistsOne(title: title);
                                default:
                                    return HtmlTestEntryAndNoRow(title: title);
                            }
                        default:
                            switch (userType)
                            {
                                case UserData.UserTypes.DisabledDept:
                                    switch (permissionIdType)
                                    {
                                        case PermissionData.PermissionIdTypes.Dept:
                                        case PermissionData.PermissionIdTypes.DeptGroup:
                                            return HtmlTestNotFoundMessage(title: title);
                                        default:
                                            return HtmlTestExistsOne(title: title);
                                    }
                                case UserData.UserTypes.DisabledGroup:
                                    return permissionIdType == PermissionData.PermissionIdTypes.Group
                                        ? HtmlTestNotFoundMessage(title: title)
                                        : HtmlTestExistsOne(title: title);
                                case UserData.UserTypes.Disabled:
                                case UserData.UserTypes.Lockout:
                                    return HtmlTestNotFoundMessage(title: title);
                                case UserData.UserTypes.Privileged:
                                    return HtmlTestExistsOne(title: title);
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
            List<HtmlTest> htmlTests)
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
                htmlTests
            };
        }

        private static string GetHtml(Context context)
        {
            var itemModel = Initializer.ItemIds.Get(context.Id);
            return itemModel.Index(context: context);
        }

        private static List<HtmlTest> HtmlTestExistsOne(string title)
        {
            var id = Initializer.Titles.Get(title);
            var htmlTest = new List<HtmlTest>()
            {
                new HtmlTest()
                {
                    Type = HtmlTest.Types.ExistsOne,
                    Selector = SelectorUtilities.GridRow(id)
                }
            };
            return htmlTest;
        }

        private static List<HtmlTest> HtmlTestNotFoundMessage(string title)
        {
            var id = Initializer.Titles.Get(title);
            var htmlTest = new List<HtmlTest>()
            {
                new HtmlTest()
                {
                    Type = HtmlTest.Types.NotFoundMessage
                }
            };
            return htmlTest;
        }

        private static List<HtmlTest> HtmlTestEntryAndNoRow(string title)
        {
            var id = Initializer.Titles.Get(title);
            var htmlTest = new List<HtmlTest>()
            {
                new HtmlTest()
                {
                    Type = HtmlTest.Types.ExistsOne,
                    Selector = "#Grid"
                },
                new HtmlTest()
                {
                    Type = HtmlTest.Types.NotExists,
                    Selector = SelectorUtilities.GridRow(id)
                }
            };
            return htmlTest;
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
