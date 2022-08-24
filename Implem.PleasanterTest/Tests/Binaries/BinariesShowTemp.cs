﻿using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Tests.Binaries
{
    public class BinariesShowTemp
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
                routeData: RouteData.BinariesShowTemp(id: id));
            var results = Results(
                context: context,
                guid: guid);
            Assert.True(results);
        }

        public static IEnumerable<object[]> GetData()
        {
            var id = Initializer.Titles.Get("運用管理システムの構築");
            var guid = Strings.NewGuid();
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
                        new KeyValue("uuid", guid),
                        new KeyValue("Uuids", guid)),
                    fileName: "Attachments.txt",
                    contentType: "text/json"),
                id: id,
                contentRange: null);
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "運用管理システムの構築",
                    guid: guid)
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
            var file = BinaryUtilities.DownloadTemp(
                context: context,
                guid: guid);
            var result = file?.FileStream();
            return result != null;
        }
    }
}
