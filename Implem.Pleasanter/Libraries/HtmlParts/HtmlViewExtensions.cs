using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlViewExtensions
    {
        public static HtmlBuilder ViewExtensions(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view)
        {
            var columns = context.ExtendedFieldColumns(
                ss: ss,
                extendedFieldType: "ViewExtensions");
            return columns.Count > 0
                && ss.ReferenceType != "Sites"
                    ? hb.Div(
                        id: "ViewExtensions",
                        action: () => hb.ViewExtensionColumns(
                            context: context,
                            ss: ss,
                            view: view,
                            columns: columns))
                    : hb;
        }

        private static HtmlBuilder ViewExtensionColumns(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view,
            IList<Column> columns)
        {
            foreach (var column in columns)
            {
                hb.ViewExtensionField(
                    context: context,
                    ss: ss,
                    column: column,
                    view: view);
            }
            return hb;
        }
    }
}