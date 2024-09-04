using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
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

namespace Implem.PleasanterTest.Tests.Binaries
{
    public class BinariesUpload
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            Dictionary<string, string> routeData,
            Forms forms,
            string fileName,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: routeData,
                httpMethod: "POST",
                forms: forms,
                fileName: fileName,
                contentType: "text/json");
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
            var id = Initializer.Titles.Get("ネットワークの構築");
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
            var uuid = Strings.NewGuid();
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    routeData: RouteData.BinariesUpload(id: id),
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "Issues_AttachmentsA"),
                        new KeyValue("ColumnName", "AttachmentsA"),
                        new KeyValue("AttachmentsData", "[]"),
                        new KeyValue("fileNames", "[\"Attachments.txt\"]"),
                        new KeyValue("fileSizes", "[15]"),
                        new KeyValue("fileTypes", "[\"text/plain\"]"),
                        new KeyValue("FileHash", "242988169a3d92a28807ff02153d5e3a"),
                        new KeyValue("uuid", uuid),
                        new KeyValue("Uuids", uuid)),
                    fileName: "Attachments.txt",
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "ReplaceAll",
                            target: "#Issues_AttachmentsAField"),
                        JsonData.ExistsOne(
                            method: "SetData",
                            target: "#Issues_AttachmentsA"))),
                new TestPart(
                    routeData: RouteData.ItemsUpdate(id: id),
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "UpdateCommand"),
                        new KeyValue("Issues_AttachmentsA", new Attachment()
                        {
                            Guid = uuid,
                            Name = "Attachments.txt",
                            Size = 15,
                            Extension = "txt",
                            ContentType = "text/plain",
                            Added = true,
                            Deleted = false
                        }
                            .ToSingleList()
                            .ToJson()),
                        new KeyValue($"Issues_Timestamp", timestamp)),
                    fileName: "Attachments.txt",
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Response",
                            target: "id"),
                        JsonData.ExistsOne(
                            method: "Invoke",
                            target: "clearDialogs"),
                        JsonData.ExistsOne(
                            method: "ReplaceAll",
                            target: "#MainContainer"),
                        JsonData.ExistsOne(
                            method: "SetValue",
                            target: "#Id"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged"),
                        JsonData.ExistsOne(
                            method: "Invoke",
                            target: "setCurrentIndex"),
                        JsonData.ExistsOne(
                            method: "Invoke",
                            target: "initRelatingColumnEditor"),
                        JsonData.ExistsOne(method: "Message"),
                        JsonData.ExistsOne(method: "ClearFormData"),
                        JsonData.ExistsOne(
                            method: "Events",
                            target: "on_editor_load"),
                        JsonData.ExistsOne(method: "Log")))
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    routeData: testPart.RouteData,
                    forms: testPart.Forms,
                    fileName: testPart.FileName,
                    userModel: testPart.UserModel,
                    baseTests: testPart.BaseTests);
            }
        }

        private static object[] TestData(
            Dictionary<string, string> routeData,
            Forms forms,
            string fileName,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            return new object[]
            {
                routeData,
                forms,
                fileName,
                userModel,
                baseTests
            };
        }

        private static string Results(Context context)
        {
            switch (context.RouteData.Get("action"))
            {
                case "upload":
                    return BinaryUtilities.UploadFile(
                        context: context,
                        id: context.Id,
                        contentRange: null)
                            .Deserialize<FileResponse>()
                            .ResponseJson;
                case "update":
                    var itemModel = Initializer.ItemIds.Get(context.Id);
                    return itemModel.Update(context: context);
                default:
                    throw new ArgumentException($"action:{context.RouteData.Get("action")}");
            }
        }
    }
}
