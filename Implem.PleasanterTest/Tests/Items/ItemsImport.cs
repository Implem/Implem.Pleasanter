using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Tests.Items
{
    public class ItemsImport
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            Forms forms,
            string fileName,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            var id = Initializer.Titles.Get(title);
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.ItemsImport(id: id),
                httpMethod: "POST",
                forms: forms,
                fileName: fileName,
                contentType: "text/csv");
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
                    target: "#CopyToClipboards"),
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
            var testParts = new List<MyTestPart>()
            {
                new MyTestPart(
                    title: "WBS",
                    fileName: "WBS.csv",
                    encoding: "Shift-JIS",
                    updatableImport: false,
                    key: "IssueId",
                    userType: UserData.UserTypes.TenantManager1)
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    forms: FormsUtilities.Get(
                        new KeyValue($"Encoding", testPart.Encoding),
                        new KeyValue($"UpdatableImport", testPart.UpdatableImport.ToString().ToLower()),
                        new KeyValue("Key", testPart.Key)),
                    fileName: testPart.FileName,
                    userModel: testPart.UserModel,
                    baseTests: validJsonTests);
            }
        }

        private static object[] TestData(
            string title,
            Forms forms,
            string fileName,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            return new object[]
            {
                title,
                forms,
                fileName,
                userModel,
                baseTests
            };
        }

        private static string Results(Context context)
        {
            var itemModel = Initializer.ItemIds.Get(context.Id);
            return itemModel.Import(context: context);
        }

        private class MyTestPart : TestPart
        {
            public string Encoding { get; }
            public bool UpdatableImport { get; }
            public string Key { get; }

            public MyTestPart(
                string title,
                string fileName,
                string encoding,
                bool updatableImport,
                string key,
                UserData.UserTypes userType = UserData.UserTypes.General1)
            {
                Title = title;
                FileName = fileName;
                UpdatableImport = updatableImport;
                Encoding = encoding;
                Key = key;
                UserModel = UserData.Get(userType: userType);
            }
        }
    }
}
