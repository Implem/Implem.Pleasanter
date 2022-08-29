﻿using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Tests.Publishes
{
    public class PublishesGridRows
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            List<BaseTest> baseTests)
        {
            var id = Initializer.Sites.Get(title).SiteId;
            var context = ContextData.Get(
                userType: UserData.UserTypes.Anonymous,
                routeData: RouteData.PublishesGridRows(id: id));
            var results = Results(context: context);
            Assert.True(Tester.Test(
                context: context,
                results: results,
                baseTests: baseTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var validJsonTests = new List<BaseTest>()
            {
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
                    target: "#Grid"),
                JsonData.ExistsOne(method: "Log")
            };
            var testParts = new List<TestPart>()
            {
                new TestPart(title: "WBS")
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    baseTests: validJsonTests);
            }
        }

        private static object[] TestData(
            string title,
            List<BaseTest> baseTests)
        {
            return new object[]
            {
                title,
                baseTests
            };
        }

        private static string Results(Context context)
        {
            var itemModel = Initializer.ItemIds.Get(context.Id);
            return itemModel.GridRows(context: context);
        }
    }
}
