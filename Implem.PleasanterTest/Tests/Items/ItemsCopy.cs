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
    public class ItemsCopy
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
                routeData: RouteData.ItemsCopy(id: id),
                httpMethod: "POST",
                forms: forms);
            var json = Results(context: context);
            Assert.True(Compare.Json(
                context: context,
                json: json,
                jsonTests: jsonTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var forms = FormsUtilities.Get(
                new KeyValue("ControlId", "CopyCommand"),
                new KeyValue("CopyWithComments", "true"));
            var jsonTests = JsonData.Tests(
                JsonData.ExistsOne(method: "Log"),
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
                    method: "SetValue",
                    target: "#SwitchTargets"),
                JsonData.ExistsOne(
                    method: "SetMemory",
                    target: "formChanged"),
                JsonData.ExistsOne(
                    method: "Invoke",
                    target: "setCurrentIndex"),
                JsonData.ExistsOne(
                    method: "Invoke",
                    target: "initRelatingColumnEditor"),
                JsonData.Value(
                    method: "Message",
                    value: "{\"Id\":\"Copied\",\"Text\":\"コピーしました。\",\"Css\":\"alert-success\"}"),
                JsonData.ExistsOne(method: "ClearFormData"),
                JsonData.ExistsOne(
                    method: "Events",
                    target: "on_editor_load"));
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "ネットワーク要件の確認",
                    forms: forms,
                    jsonTests: jsonTests)
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
            var itemModel = Initializer.ItemIds.Get(context.Id) ?? new ItemModel();
            return itemModel.Copy(context: context);
        }
    }
}
