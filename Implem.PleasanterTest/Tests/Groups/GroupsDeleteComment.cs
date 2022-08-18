﻿using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Implem.PleasanterTest.Tests.Groups
{
    public class GroupsDeleteComment
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            Forms forms,
            UserModel userModel,
            List<JsonTest> jsonTests)
        {
            var id = Initializer.Groups.Values.FirstOrDefault(o => o.GroupName == title).GroupId;
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.GroupsDeleteComment(id: id),
                forms: forms);
            var results = Results(context: context);
            Assert.True(Compare.Json(
                context: context,
                results: results,
                jsonTests: jsonTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var testParts = new List<MyTestPart>()
            {
                new MyTestPart(
                    title: "グループ9",
                    commentId: 1,
                    updateResponseType: 1,
                    userType: UserData.UserTypes.TenantManager1)
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", $"DeleteComment,{testPart.CommentId}")),
                    userModel: testPart.UserModel,
                    jsonTests: testPart.UpdateResponseType == 0
                        ? JsonData.ReplaceAll(
                            target: "#MainContainer",
                            selector: "#Editor").ToSingleList()
                        : new List<JsonTest>()
                        {
                            JsonData.ExistsOne(
                                method: "Html",
                                target: "#HeaderTitle"),
                            JsonData.ExistsOne(
                                method: "Html",
                                target: "#RecordInfo"),
                            JsonData.ExistsOne(
                                method: "SetMemory",
                                target: "formChanged"),
                            JsonData.ExistsOne(
                                method: "Remove",
                                target: $"[id=\"Comment{testPart.CommentId}.wrapper\"]"),
                        });
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
            return GroupUtilities.Update(
                context: context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(context: context),
                groupId: context.Id.ToInt());
        }

        private class MyTestPart : TestPart
        {
            public int CommentId { get; set; }
            public int UpdateResponseType { get; set; }

            public MyTestPart(
                string title,
                int commentId,
                int updateResponseType,
                UserData.UserTypes userType = UserData.UserTypes.General1)
            {
                Title = title;
                CommentId = commentId;
                UpdateResponseType = updateResponseType;
                UserModel = UserData.Get(userType: userType);
            }
        }
    }
}
