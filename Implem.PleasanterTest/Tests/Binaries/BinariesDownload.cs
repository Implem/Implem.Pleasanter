using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Xunit;

namespace Implem.PleasanterTest.Tests.Binaries
{
    public class BinariesDownload
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            string guid,
            UserModel userModel)
        {
            var id = Initializer.Titles.Get(title);
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.BinariesDownload(id: id));
            var results = Results(
                context: context,
                guid: guid);
			Assert.True(results);
        }

        public static IEnumerable<object[]> GetData()
        {
            var siteId = Initializer.Titles.Get("WBS");
            var id = Initializer.Titles.Get("サーバ要件の確認");
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
            BinaryUtilities.UploadFile(
                context: ContextData.Get(
                    userId: UserData.Get(userType: UserData.UserTypes.General1).UserId,
                    routeData: RouteData.BinariesUpload(id: id),
                    httpMethod: "POST",
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
                    contentType: "text/json"),
                id: id,
                contentRange: null);
            Initializer.ItemIds.Get(id).Update(context: ContextData.Get(
                userId: UserData.Get(userType: UserData.UserTypes.General1).UserId,
                routeData: RouteData.BinariesUpload(id: id),
                httpMethod: "POST",
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
                contentType: "text/json"));
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "サーバ要件の確認",
                    guid: uuid)
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    guid: testPart.Guid,
                    userModel: testPart.UserModel);
            }
        }

        private static object[] TestData(
            string title,
            string guid,
            UserModel userModel)
        {
            return new object[]
            {
                title,
                guid,
                userModel
            };
        }

        private static bool Results(Context context, string guid)
        {
            var file = BinaryUtilities.Donwload(
                context: context,
                guid: guid)
                    ?.FileStream();
            var result = file != null
                ? new FileContentResult(file.FileContents, file.ContentType)
                : null;
            return result != null;
        }
    }
}
