using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using System;
using System.Linq;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlEditorTabs
    {
        public static HtmlAttributes TabActive(
            this HtmlAttributes attributes,
            Context context,
            string editorTabs)
        {
            var tabName = context.QueryStrings.Data("TabName");
            if (tabName.IsNullOrEmpty())
            {
                return attributes;
            }
            var index = TabIndex(
                editorTabs: editorTabs,
                tabName: tabName);
            return attributes.Add(
                name: "tab-active",
                value: index.ToString(),
                _using: index > 0);
        }

        private static int TabIndex(string editorTabs, string tabName)
        {
            var index = 0;
            foreach (var tab in editorTabs
                .Split(new string[] { "<li" }, StringSplitOptions.None)
                .Skip(1))
            {
                if (tab.Split('>').FirstOrDefault()?.Contains("ignore-tab") == true)
                {
                    continue;
                }
                if (tab.Contains($"href=\"#{tabName}\""))
                {
                    return index;
                }
                index++;
            }
            return 0;
        }
    }
}
