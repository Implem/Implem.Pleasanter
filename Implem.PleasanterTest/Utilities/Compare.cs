using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.PleasanterTest.Models;
using System.Collections.Generic;
using System.Linq;

namespace Implem.PleasanterTest.Utilities
{
    /// <summary>
    /// HTMLまたはJSONの返却値をテストデータによって正しいことをチェックします。
    /// </summary>
    public static class Compare
    {
        /// <summary>
        /// HTMLの返却値をテストデータによって正しいことをチェックします。
        /// HTMLはAngleSharpを使用してParseします。
        /// </summary>
        public static bool Html(
            Context context,
            string html,
            List<HtmlTest> htmlTests)
        {
            var parser = new AngleSharp.Html.Parser.HtmlParser();
            var doc = parser.ParseDocument(html);
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
                    return doc.QuerySelectorAll("#MessageData");
                default:
                    return doc.QuerySelectorAll(htmlTest.Selector);
            }
        }

        /// <summary>
        /// JSONの返却値をテストデータによって正しいことをチェックします。
        /// JSON内のHTMLはHTMLメソッドを呼び出してチェックします。
        /// </summary>
        public static bool Json(
            Context context,
            string json,
            List<JsonTest> jsonTests)
        {
            var responseCollection = json.Deserialize<ResponseCollection>();
            foreach (var jsonTest in jsonTests)
            {
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
                            html: nodes[0].Value.ToString(),
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
            }
            return true;
        }
    }
}
