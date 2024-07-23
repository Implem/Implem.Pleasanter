using AngleSharp.Html.Dom;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.PleasanterTest.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Implem.PleasanterTest.Utilities
{
    /// <summary>
    /// HTMLまたはJSONの返却値をテストデータによって正しいことをチェック。
    /// </summary>
    public static class Tester
    {
        public static bool Test(
            Context context,
            object results,
            List<BaseTest> baseTests)
        {
            foreach (var baseTest in baseTests)
            {
                switch (baseTest)
                {
                    case HtmlTest htmlTest:
                        if (!Html(
                            context: context,
                            results: (string)results,
                            htmlTests: htmlTest.ToSingleList()))
                        {
                            return false;
                        };
                        break;
                    case JsonTest jsonTest:
                        if (!Json(
                            context: context,
                            results: (string)results,
                            jsonTest: jsonTest))
                        {
                            return false;
                        };
                        break;
                    case ApiJsonTest apiJsonTest:
                        if (!ApiResults(
                            context: context,
                            results: (ContentResultInheritance)results,
                            apiJsonTests: apiJsonTest.ToSingleList()))
                        {
                            return false;
                        };
                        break;
                    case TextTest textTest:
                        if (!Text(
                            context: context,
                            results: (string)results,
                            textTest: textTest))
                        {
                            return false;
                        };
                        break;
                    case FileTest fileTest:
                        if (!File(
                            context: context,
                            results: (ResponseFile)results,
                            fileTest: fileTest))
                        {
                            return false;
                        };
                        break;
                    default:
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// HTMLの返却値をテストデータによって正しいことをチェック。
        /// HTMLはAngleSharpを使用してParse。
        /// </summary>
        private static bool Html(
            Context context,
            string results,
            List<HtmlTest> htmlTests)
        {
            var parser = new AngleSharp.Html.Parser.HtmlParser();
            var doc = parser.ParseDocument(results);
            foreach (var htmlTest in htmlTests)
            {
                var nodes = Nodes(
                    doc: doc,
                    htmlTest: htmlTest);
                switch (htmlTest.Type)
                {
                    case HtmlTest.Types.TextContent:
                        if (nodes.Count() != 1)
                        {
                            return false;
                        }
                        else if (nodes[0].TextContent != htmlTest.Value)
                        {
                            return false;
                        }
                        break;
                    case HtmlTest.Types.InnerHtml:
                        if (nodes.Count() != 1)
                        {
                            return false;
                        }
                        else if (nodes[0].InnerHtml != htmlTest.Value)
                        {
                            return false;
                        }
                        break;
                    case HtmlTest.Types.Exists:
                        if (!nodes.Any())
                        {
                            return false;
                        }
                        break;
                    case HtmlTest.Types.ExistsOne:
                        if (nodes.Count() != 1)
                        {
                            return false;
                        }
                        break;
                    case HtmlTest.Types.NotExists:
                        if (nodes.Any())
                        {
                            return false;
                        }
                        break;
                    case HtmlTest.Types.Disabled:
                        if (nodes.Count() != 1)
                        {
                            return false;
                        }
                        else if (nodes[0].GetAttribute("disabled") != "disabled")
                        {
                            return false;
                        }
                        break;
                    case HtmlTest.Types.HasClass:
                        if (nodes.Count() != 1)
                        {
                            return false;
                        }
                        else if (!nodes[0].ClassList.Contains(htmlTest.Value))
                        {
                            return false;
                        }
                        break;
                    case HtmlTest.Types.Attribute:
                        if (nodes.Count() != 1)
                        {
                            return false;
                        }
                        else if (nodes[0].GetAttribute(htmlTest.Name) != htmlTest.Value)
                        {
                            return false;
                        }
                        break;
                    case HtmlTest.Types.NotFoundMessage:
                        if (nodes.Count() != 1)
                        {
                            return false;
                        }
                        else if (nodes[0].GetAttribute("value") != Messages.NotFound(context: context).ToSingleList().ToJson())
                        {
                            return false;
                        }
                        break;
                    case HtmlTest.Types.HasNotPermissionMessage:
                        if (nodes.Count() != 1)
                        {
                            return false;
                        }
                        else if (nodes[0].GetAttribute("value") != Messages.HasNotPermission(context: context).ToSingleList().ToJson())
                        {
                            return false;
                        }
                        break;
                    case HtmlTest.Types.HasInformationMessage:
                        if (nodes.Count() != 1)
                        {
                            return false;
                        }
                        else
                        {
                            var valueText = nodes[0].GetAttribute("value");
                            var messages = JsonConvert.DeserializeAnonymousType(valueText, new[] { new { Text = "", Css = "" } });
                            return messages?.Any(m => m.Css == "alert-information" && m.Text == htmlTest.Value) ?? false;
                        }
                    case HtmlTest.Types.Text:
                        if (nodes.Count() != 1)
                        {
                            return false;
                        }
                        else if (!Text(
                            context: context,
                            results: nodes[0].OuterHtml,
                            textTest: htmlTest.TextTest))
                        {
                            return false;
                        }
                        break;
                    case HtmlTest.Types.SelectedOption:
                        if (nodes.Count() != 1)
                        {
                            return false;
                        }
                        var value = (nodes[0] as AngleSharp.Html.Dom.IHtmlSelectElement).SelectedOptions[0].Value;
                        if (value != htmlTest.Value)
                        {
                            return false;
                        }

                        break;
                    default:
                        return false;
                }
            }
            return true;
        }

        private static AngleSharp.Dom.IHtmlCollection<AngleSharp.Dom.IElement> Nodes(
            AngleSharp.Html.Dom.IHtmlDocument doc,
            HtmlTest htmlTest)
        {
            switch (htmlTest.Type)
            {
                case HtmlTest.Types.NotFoundMessage:
                case HtmlTest.Types.HasNotPermissionMessage:
                case HtmlTest.Types.HasInformationMessage:
                    return doc.QuerySelectorAll("#MessageData");
                default:
                    return doc.QuerySelectorAll(htmlTest.Selector);
            }
        }

        /// <summary>
        /// ResponseCollectionの返却値をテストデータによって正しいことをチェック。
        /// ResponseCollection内のHTMLはHTMLメソッドを呼び出してチェック。
        /// </summary>
        private static bool Json(
            Context context,
            string results,
            JsonTest jsonTest)
        {
            var responseCollection = results.Deserialize<ResponseCollection>();
            var nodes = responseCollection
                .Where(o => o.Method == jsonTest.Method)
                .Where(o => o.Target == jsonTest.Target)
                .ToArray();
            switch (jsonTest.Type)
            {
                case JsonTest.Types.Value:
                    if (nodes[0].Value.ToString() != jsonTest.Value.ToString())
                    {
                        return false;
                    }
                    if (nodes[0].Options != jsonTest.Options)
                    {
                        return false;
                    }
                    break;
                case JsonTest.Types.Exists:
                    if (!nodes.Any())
                    {
                        return false;
                    }
                    break;
                case JsonTest.Types.ExistsOne:
                    if (nodes.Count() != 1)
                    {
                        return false;
                    }
                    break;
                case JsonTest.Types.Html:
                    if (nodes.Count() != 1)
                    {
                        return false;
                    }
                    else if (!Html(
                        context: context,
                        results: nodes[0].Value.ToString(),
                        htmlTests: jsonTest.HtmlTests))
                    {
                        return false;
                    }
                    break;
                case JsonTest.Types.Message:
                    if (nodes.Count() != 1)
                    {
                        return false;
                    }
                    else
                    {
                        switch (jsonTest.Value)
                        {
                            case "NotFound":
                                if (nodes[0].Value.ToString() != Messages.NotFound(context: context).ToJson())
                                {
                                    return false;
                                }
                                break;
                            default:
                                return false;
                        }
                    }
                    break;
                default:
                    return false;
            }
            return true;
        }

        /// <summary>
        /// APIの返却値をテストデータによって正しいことをチェック。
        /// </summary>
        private static bool ApiResults(
            Context context,
            ContentResultInheritance results,
            List<ApiJsonTest> apiJsonTests)
        {
            dynamic content = JsonConvert.DeserializeObject<ExpandoObject>(results.Content);
            var data = (IDictionary<string, object>)content;
            foreach (var apiJsonTest in apiJsonTests)
            {
                switch (apiJsonTest.Type)
                {
                    case ApiJsonTest.Types.StatusCode:
                        if (!data.ContainsKey("StatusCode"))
                        {
                            return false;
                        }
                        else if (data["StatusCode"].ToInt() != apiJsonTest.Value.ToInt())
                        {
                            return false;
                        }
                        break;
                    default:
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 文字列の返却値をテストデータによって正しいことをチェック。
        /// </summary>
        private static bool Text(
            Context context,
            string results,
            TextTest textTest)
        {
            switch (textTest.Type)
            {
                case TextTest.Types.Equals:
                    if (results != textTest.Value?.ToString())
                    {
                        return false;
                    }
                    break;
                case TextTest.Types.ListEquals:
                    if (!textTest.ListEquals(text: results))
                    {
                        return false;
                    }
                    break;
                case TextTest.Types.Contains:
                    if (!results.Contains(textTest.Value?.ToString()))
                    {
                        return false;
                    }
                    break;
                default:
                    return false;
            }
            return true;
        }

        /// <summary>
        /// ResponseFileの返却値をテストデータによって正しいことをチェック。
        /// </summary>
        private static bool File(
            Context context,
            ResponseFile results,
            FileTest fileTest)
        {
            switch (fileTest.Type)
            {
                case FileTest.Types.Exists:
                    if (results == null)
                    {
                        return false;
                    }
                    if (results.Length == 0)
                    {
                        return false;
                    }
                    break;
            }
            return true;
        }
    }
}
