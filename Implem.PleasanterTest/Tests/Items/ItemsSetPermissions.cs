﻿using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Implem.PleasanterTest.Tests.Items
{
    public class ItemsSetPermissions
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            Forms forms,
            UserModel userModel,
            List<JsonTest> jsonTests)
        {
            var id = Initializer.Titles.Get(title);
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.ItemsSetPermissions(id: id),
                httpMethod: "POST",
                forms: forms);
            var results = Results(context: context);
            Assert.True(Compare.Json(
                context: context,
                results: results,
                jsonTests: jsonTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var forms = FormsUtilities.Get(
                new KeyValue("ControlId", "AddPermissions"),
                new KeyValue("SourcePermissions", $"[\"User,{Initializer.Users.Values.FirstOrDefault(o => o.Name == "高橋 一郎").UserId},0\"]"));
            var jsonTests = JsonData.Tests(
                JsonData.ExistsOne(
                    method: "ScrollTop",
                    target: "#SourcePermissionsWrapper"),
                JsonData.ExistsOne(
                    method: "Html",
                    target: "#CurrentPermissions"),
                JsonData.ExistsOne(
                    method: "Html",
                    target: "#SourcePermissions"),
                JsonData.ExistsOne(
                    method: "SetValue",
                    target: "#SourcePermissionsOffset"),
                JsonData.ExistsOne(
                    method: "SetData",
                    target: "#CurrentPermissions"),
                JsonData.ExistsOne(
                    method: "SetData",
                    target: "#SourcePermissions"),
                JsonData.ExistsOne(
                    method: "SetMemory",
                    target: "formChanged"));
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "ネットワークの構築",
                    forms: forms,
                    jsonTests: jsonTests,
                    userType: UserData.UserTypes.TenantManager1)
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    forms: testPart.Forms,
                    userModel: testPart.UserModel,
                    jsonTests: testPart.JsonTests);
            }
        }

        private static object[] TestData(
            string title,
            Forms forms,
            UserModel userModel,
            List<JsonTest> jsonTests)
        {
            return new object[]
            {
                title,
                forms,
                userModel,
                jsonTests
            };
        }

        private static string Results(Context context)
        {
            return PermissionUtilities.SetPermissions(
                context: context,
                referenceId: context.Id);
        }
    }
}
