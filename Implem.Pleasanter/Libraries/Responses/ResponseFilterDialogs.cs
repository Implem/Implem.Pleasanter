using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;

namespace Implem.Pleasanter.Libraries.Responses
{
    public static class ResponseFilterDialogs
    {
        public static string OpenSetNumericRangeDialog(
            Context context,
            SiteSettings ss)
        {
            if (context.HasPermission(ss: ss))
            {
                var controlId = context.Forms.ControlId();
                var columnName = controlId
                    .Substring(controlId.IndexOf("__") + 2)
                    .Replace("_NumericRange", string.Empty);
                var column = ss.GetColumn(
                    context: context,
                    columnName: columnName);
                return new ResponseCollection(context: context)
                    .Html(
                        "#SetNumericRangeDialog",
                        new HtmlBuilder().SetNumericRangeDialog(
                            context: context,
                            ss: ss,
                            column: column,
                            itemfilter: true))
                    .ToJson();
            }
            else
            {
                return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public static string OpenSetDateRangeDialog(
            Context context,
            SiteSettings ss)
        {
            if (context.HasPermission(ss: ss))
            {
                var controlId = context.Forms.ControlId();
                var columnName = controlId
                    .Substring(controlId.IndexOf("__") + 2)
                    .Replace("_DateRange", string.Empty);
                var column = ss.GetColumn(
                    context: context,
                    columnName: columnName);
                return new ResponseCollection(context: context)
                    .Html(
                        "#SetDateRangeDialog",
                        new HtmlBuilder().SetDateRangeDialog(
                            context: context,
                            ss: ss,
                            column: column,
                            itemfilter: true))
                    .ToJson();
            }
            else
            {
                return Messages.ResponseNotFound(context: context).ToJson();
            }
        }
    }
}
