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
            HtmlTest.Types htmlTestType)
        {
            InitPermissions(
                permissionInitType: permissionInitType,
                deptId: deptId,
                groupId: groupId,
                userId: userId,
                permissionType: permissionType);
            var id = Initializer.Titles.Get(title);
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: new Dictionary<string, string>()
                {
                    { "controller", "items" },
                    { "action", "index" },
                    { "id", id.ToString() },
                });
            var html = GetHtml(
                context: context,
                id: id);
            var htmlTests = GetHtmlTest(
                title: title,
                htmlTestType: htmlTestType);
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
                                userId: userModel.UserId,
                                type: GetTestType(
                                    userType: userType,
                                    referenceType: referenceType,
                                    allow: permissionInitType != PermissionData.PermissionInitTypes.Nothing));
                        }
                    }
                }
            }
        }

        private static HtmlTest.Types GetTestType(
            UserData.UserTypes userType,
            ItemData.ReferenceTypes referenceType,
            bool allow)
        {
            switch (referenceType)
            {
                case ItemData.ReferenceTypes.Wikis:
                    return HtmlTest.Types.NotFoundMessage;
                default:
                    switch (userType)
                    {
                        case UserData.UserTypes.PrivilegedUser:
                            return HtmlTest.Types.ExistsOne;
                        case UserData.UserTypes.Lockout:
                        case UserData.UserTypes.Disabled:
                            return HtmlTest.Types.NotFoundMessage;
                        default:
                            return allow
                                ? HtmlTest.Types.ExistsOne
                                : HtmlTest.Types.NotFoundMessage;
                    }
            }
        }

        private static object[] TestData(
            string title,
            UserModel userModel,
            PermissionData.PermissionInitTypes permissionInitType,
            int deptId = 0,
            int groupId = 0,
            int userId = 0,
            Permissions.Types permissionType = (Permissions.Types)31,
            HtmlTest.Types type = HtmlTest.Types.ExistsOne)
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
                type
            };
        }

        private static string GetHtml(
            Context context,
            long id)
        {
            var itemModel = Initializer.ItemIds.Get(Initializer.ItemIds.Get(id).SiteId);
            return itemModel.Index(context: context);
        }

        private static List<HtmlTest> GetHtmlTest(
            string title,
            HtmlTest.Types htmlTestType)
        {
            var id = Initializer.Titles.Get(title);
            var htmlTest = new List<HtmlTest>()
            {
                new HtmlTest()
                {
                    Type = htmlTestType,
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
