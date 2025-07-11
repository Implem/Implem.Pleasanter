using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace Implem.PleasanterTest.Tests.Users
{
    [Collection(nameof(UsersImport))]
    public class UsersImport : IDisposable
    {
        private static string CopyFileName = "Users_Import_xUnit_Copy.csv";
        public UsersImport()
        {
            string guid = Guid.NewGuid().ToString();
            string csvContent =
                "ユーザID,ログインID,名前,言語,タイムゾーン,メールアドレス,パスワード\n"
                 + $",ImportTestUser_{guid},TestUser_Import,Japanese,協定世界時,{guid}@example.com,";
            var filePath = Path.Combine(
                        Directories.PleasanterTest(),
                        "BinaryData",
                         "Users_Import_tmp.csv");
            var destFilePath = Path.Combine(
                Directories.PleasanterTest(),
                "BinaryData",
                CopyFileName);
            File.Copy(filePath, destFilePath, true);
            File.WriteAllText(destFilePath, csvContent, Encoding.UTF8);
        }

        public void Dispose()
        {
            var deleteFilePath = Path.Combine(
                Directories.PleasanterTest(),
                "BinaryData",
                CopyFileName);
            if (File.Exists(deleteFilePath))
            {
                File.Delete(deleteFilePath);
            }
        }
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            Forms forms,
            string fileName,
            UserModel userModel,
            List<BaseTest> baseTests)
        {          
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.UsersImport(userModel.UserId),
                httpMethod: "POST",
                forms: forms,
                fileName: fileName,
                contentType: "text/csv");
            var results = GetResults(context: context);
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
                JsonData.Value(
                    method: "Message",
                    value: "{\"Id\":\"Imported\",\"Text\":\": 1 件追加し、0 件更新しました。\",\"Css\":\"alert-success\"}")
            };
            var testParts = new List<MyTestPart>()
            {
                new MyTestPart(
                    fileName: CopyFileName,
                    encoding: "UTF-8",
                    updatableImport: false,
                    key: null,
                    userType: UserData.UserTypes.TenantManager1)
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    forms: FormsUtilities.Get(
                        new KeyValue($"Encoding", testPart.Encoding),
                        new KeyValue($"UpdatableImport", null),
                        new KeyValue("Key", null),
                        new KeyValue("ReplaceAllGroupMembers", null),
                        new KeyValue("MigrationMode", null)),
                    fileName: testPart.FileName,
                    userModel: testPart.UserModel,
                    baseTests: validJsonTests);
            }
        }

        private static object[] TestData(
            Forms forms,
            string fileName,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            return new object[]
            {
                forms,
                fileName,
                userModel,
                baseTests
            };
        }

        private static string GetResults(Context context)
        {
            return UserUtilities.Import(context: context);
        }

        private class MyTestPart : TestPart
        {
            public string Encoding { get; }
            public bool UpdatableImport { get; }
            public string Key { get; }

            public MyTestPart(
                string fileName,
                string encoding,
                bool updatableImport,
                string key,
                UserData.UserTypes userType = UserData.UserTypes.General1)
            {
                FileName = fileName;
                UpdatableImport = updatableImport;
                Encoding = encoding;
                Key = key;
                UserModel = UserData.Get(userType: userType);
            }
        }
    }
}
