﻿using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Tests.Items
{
    public class ItemsGridRows
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            UserModel userModel,
            List<JsonTest> jsonTests)
        {
            var siteId = Initializer.Sites.Get(title).SiteId;
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.ItemsGridRows(id: siteId));
            var json = Results(context: context);
            Assert.True(Compare.Json(
                context: context,
                json: json,
                jsonTests: jsonTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var validJsonTests = new List<JsonTest>()
            {
                JsonData.ExistsOne(method: "Log"),
                JsonData.ExistsOne(
                    method: "Remove",
                    target: ".grid tr"),
                JsonData.ExistsOne(
                    method: "ClearFormData",
                    target: "GridOffset"),
                JsonData.ExistsOne(method: "CloseDialog"),
                JsonData.ExistsOne(
                    method: "ReplaceAll",
                    target: "#CopyDirectUrlToClipboard"),
                JsonData.ExistsOne(
                    method: "ReplaceAll",
                    target: "#Aggregations"),
                JsonData.ExistsOne(
                    method: "Append",
                    target: "#Grid"),
                JsonData.ExistsOne(
                    method: "SetValue",
                    target: "#GridOffset"),
                JsonData.ExistsOne(
                    method: "SetValue",
                    target: "#GridRowIds"),
                JsonData.ExistsOne(
                    method: "SetValue",
                    target: "#GridColumns"),
                JsonData.ExistsOne(
                    method: "Paging",
                    target: "#Grid")
            };
            var testParts = new List<TestPart>()
            {
                new TestPart(title: "WBS"),
                new TestPart(title: "課題管理"),
                new TestPart(title: "レビュー記録"),
                new TestPart(title: "顧客マスタ"),
                new TestPart(title: "商談"),
                new TestPart(title: "仕入")
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    userModel: testPart.UserModel,
                    jsonTests: validJsonTests);
            }
        }

        private static object[] TestData(
            string title,
            UserModel userModel,
            List<JsonTest> jsonTests)
        {
            return new object[]
            {
                title,
                userModel,
                jsonTests
            };
        }

        private static string Results(Context context)
        {
            var itemModel = Initializer.ItemIds.Get(context.Id);
            return itemModel.GridRows(context: context);
        }
    }
}
