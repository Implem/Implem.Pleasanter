using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Xunit;

namespace Implem.PleasanterTest.Tests.Items
{
    [Collection(nameof(ItemsUpdateByGrid))]
    public class ItemsUpdateByGrid
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            Forms forms,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            var id = Initializer.Sites.Get(title).SiteId;
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.ItemsUpdateByGrid(id: id),
                httpMethod: "POST",
                forms: forms);
            var results = Results(context: context);
            Initializer.SaveResults(results);
            Assert.True(Tester.Test(
                context: context,
                results: results,
                baseTests: baseTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var siteId = Initializer.Titles.Get("WBS");
            var id = Initializer.Titles.Get("運用者向けマニュアルを作成する");
            var timestamp = Rds.ExecuteTable(
                context: Initializer.Context,
                statements: Rds.SelectIssues(
                    column: Rds.IssuesColumn().UpdatedTime(),
                    where: Rds.IssuesWhere()
                        .SiteId(siteId)
                        .IssueId(id)))
                            .AsEnumerable()
                            .FirstOrDefault()
                            .Field<DateTime>("UpdatedTime").ToString("yyyy/M/d H:m:s.fff");
            var forms = FormsUtilities.Get(
                new KeyValue("ControlId", "UpdateByGridCommand"),
                new KeyValue("EditOnGrid", "1"),
                new KeyValue($"Issues_Status_{siteId}_{id}", "900"),
                new KeyValue($"Issues_Timestamp_{siteId}_{id}", timestamp));
            var baseTests = BaseData.Tests(
                JsonData.ExistsOne(
                    method: "ReplaceAll",
                    target: $"[data-id=\"{id}\"][data-latest]"),
                JsonData.ExistsOne(
                    method: "ClearFormData",
                    target: $"Issues_Status_{siteId}_{id}"),
                JsonData.ExistsOne(
                    method: "ClearFormData",
                    target: $"Issues_Timestamp_{siteId}_{id}"),
                JsonData.ExistsOne(
                    method: "SetMemory",
                    target: "formChanged"),
                JsonData.Value(
                    method: "Message",
                    value: "{\"Id\":\"UpdatedByGrid\",\"Text\":\"1 件 更新しました。\",\"Css\":\"alert-success\"}"),
                JsonData.ExistsOne(method: "Log"));
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "WBS",
                    forms: forms,
                    baseTests: baseTests)
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    forms: testPart.Forms,
                    userModel: testPart.UserModel,
                    baseTests: testPart.BaseTests);
            }
        }

        private static object[] TestData(
            string title,
            Forms forms,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            return new object[]
            {
                title,
                forms,
                userModel,
                baseTests
            };
        }

        private static string Results(Context context)
        {
            var itemModel = Initializer.ItemIds.Get(context.Id);
            return itemModel.UpdateByGrid(context: context);
        }
    }
}
