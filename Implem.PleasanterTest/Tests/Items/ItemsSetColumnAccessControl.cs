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
    public class ItemsSetColumnAccessControl
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
                routeData: RouteData.ItemsSetColumnAccessControl(id: id),
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
            var siteModel = Initializer.Sites.Get("WBS");
            var forms = FormsUtilities.Get(
                new KeyValue("ColumnAccessControlType", "Read"),
                new KeyValue("ColumnAccessControl", "[\"{\\\"ColumnName\\\":\\\"Title\\\",\\\"Type\\\":0}\"]"),
                new KeyValue("ColumnAccessControlAll", siteModel
                    .SiteSettings
                    .ColumnAccessControlOptions(
                        context: Initializer.Context,
                        type: "Read")
                    .Select(o => o.Key)
                    .ToJson()));
            var jsonTests = JsonData.Tests(
                JsonData.ExistsOne(method: "CloseDialog"),
                JsonData.ExistsOne(
                    method: "Html",
                    target: "#ReadColumnAccessControl"),
                JsonData.ExistsOne(
                    method: "SetData",
                    target: "#ReadColumnAccessControl"),
                JsonData.ExistsOne(
                    method: "SetMemory",
                    target: "formChanged"));
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "WBS",
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
            return PermissionUtilities.SetColumnAccessControl(
                context: context,
                referenceId: context.Id);
        }
    }
}
