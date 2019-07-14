using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Linq;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlAggregations
    {
        public static HtmlBuilder Aggregations(
            this HtmlBuilder hb, Context context, SiteSettings ss, View view)
        {
            return !Reduced(context: context, siteId: ss.SiteId)
                ? hb.Div(
                    id: "Aggregations",
                    action: () => hb
                        .DisplayControl(
                            context: context,
                            id: "ReduceAggregations",
                            icon: "ui-icon-close")
                        .Contents(
                            context: context,
                            ss: ss,
                            view: view))
                : hb.Div(
                    id: "Aggregations",
                    css: "reduced",
                    action: () => hb
                        .DisplayControl(
                            context: context,
                            id: "ExpandAggregations",
                            icon: "ui-icon-folder-open"));
        }

        private static bool Reduced(Context context, long siteId)
        {
            var key = "ReduceAggregations";
            if (context.Forms.ControlId() == key)
            {
                SessionUtilities.Set(
                    context: context,
                    key: key,
                    value: "1",
                    page: true);
            }
            else if (context.Forms.ControlId() == "ExpandAggregations")
            {
                SessionUtilities.Remove(
                    context: context,
                    key: key,
                    page: true);
            }
            return SessionUtilities.Bool(
                context: context,
                key: key);
        }

        private static HtmlBuilder DisplayControl(
            this HtmlBuilder hb, Context context, string id, string icon)
        {
            return hb.Div(
                attributes: new HtmlAttributes()
                    .Id(id)
                    .Class("display-control")
                    .OnClick("$p.send($(this));")
                    .DataMethod("post"),
                action: () => hb
                    .Span(css: "ui-icon " + icon)
                    .Text(text: Displays.Aggregations(context: context) + ":"));
        }

        private static HtmlBuilder Contents(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view)
        {
            if (view.RequestSearchCondition(ss: ss))
            {
                return hb.Span(
                    css: "label",
                    action: () => hb
                        .Text(text: Displays.NoDataSearchCondition(context: context)));
            }
            else
            {
                var aggregations = new Aggregations(
                    context: context,
                    ss: ss,
                    view: view);
                return aggregations?.TotalCount != 0
                    ? hb
                        .Total(
                            context: context,
                            aggregations: aggregations)
                        .Overdue(
                            context: context,
                            aggregations: aggregations)
                        .Parts(
                            context: context,
                            ss: ss,
                            aggregations: aggregations)
                    : hb.Span(css: "label", action: () => hb
                        .Text(text: Displays.NoData(context: context)));
            }
        }

        private static HtmlBuilder Total(
            this HtmlBuilder hb, Context context, Aggregations aggregations)
        {
            return hb
                .Span(css: "label", action: () => hb
                    .Text(text: Displays.Quantity(context: context)))
                .Span(css: "data", action: () => hb
                    .Text(text: aggregations.TotalCount.ToString()));
        }

        private static HtmlBuilder Overdue(
            this HtmlBuilder hb, Context context, Aggregations aggregations)
        {
            return aggregations.OverdueCount > 0
                ? hb
                    .Span(css: "label overdue", action: () => hb
                        .Text(text: Displays.Overdue(context: context)))
                    .Span(css: "data overdue", action: () => hb
                        .Text(text: aggregations.OverdueCount.ToString()))
                : hb;
        }

        private static HtmlBuilder Parts(
            this HtmlBuilder hb, Context context, SiteSettings ss, Aggregations aggregations)
        {
            var allowedColumns = Permissions.AllowedColumns(ss);
            aggregations.AggregationCollection
                .Where(o => o.Target.IsNullOrEmpty() || allowedColumns.Contains(o.Target))
                .Where(o => o.GroupBy == "[NotGroupBy]" || allowedColumns.Contains(o.GroupBy))
                .Where(o => o.Data.Count > 0)
                .ForEach(aggregation =>
                {
                    var html = string.Empty;
                    var groupBy = ss.GetColumn(
                        context: context, columnName: aggregation.GroupBy);
                    var targetColumn = ss.GetColumn(
                        context: context, columnName: aggregation.Target);
                    if (aggregation.Data.Count > 0)
                        hb.GroupBy(
                            context: context,
                            groupBy: groupBy,
                            targetColumn: targetColumn,
                            aggregation: aggregation);
                    aggregation.Data.OrderByDescending(o => o.Value).ForEach(data =>
                        hb.LabelValue(
                            label: groupBy != null
                                ? Label(
                                    context: context,
                                    groupBy: groupBy,
                                    selectedValue: data.Key)
                                : string.Empty,
                            value: (targetColumn != null
                                ? targetColumn.Display(
                                    context: context,
                                    value: data.Value)
                                : data.Value.ToString()) +
                                    (aggregation.Type != Aggregation.Types.Count
                                        ? targetColumn?.Unit
                                        : string.Empty),
                            bold: Bold(groupBy, data.Key),
                            attributes: new HtmlAttributes()
                                .Attributes(ss, aggregation, groupBy, data.Key)));
                });
            return hb;
        }

        private static HtmlBuilder GroupBy(
            this HtmlBuilder hb,
            Context context,
            Column groupBy,
            Column targetColumn,
            Aggregation aggregation)
        {
            var text = groupBy != null
                ? groupBy.GridLabelText + ": "
                : string.Empty;
            switch (aggregation.Type)
            {
                case Aggregation.Types.Count:
                    text += Displays.Get(
                        context: context,
                        id: aggregation.Type.ToString());
                    break;
                default:
                    text += targetColumn.GridLabelText + " " +
                        Displays.Get(
                            context: context,
                            id: aggregation.Type.ToString());
                    break;
            }
            return hb.Span(css: "label", action: () => hb
                .Text(text: text));
        }

        private static string Label(Context context, Column groupBy, string selectedValue)
        {
            if (groupBy.UserColumn)
            {
                return SiteInfo.UserName(
                    context: context,
                    userId: selectedValue.ToInt());
            }
            else if (groupBy.HasChoices())
            {
                var label = groupBy.Choice(selectedValue).TextMini;
                return label.IsNullOrEmpty()
                    ? NumericZero(groupBy, selectedValue)
                        ? Displays.NotSet(context: context)
                        : StringEmpty(groupBy, selectedValue)
                            ? Displays.NotSet(context: context)
                            : "? " + selectedValue
                    : label;
            }
            else
            {
                return selectedValue;
            }
        }

        private static string Selector(SiteSettings ss, string columnName)
        {
            return "#ViewFilters__" + columnName;
        }

        private static HtmlAttributes Attributes(
            this HtmlAttributes attributes,
            SiteSettings ss,
            Aggregation aggregation,
            Column groupBy,
            string key)
        {
            return groupBy != null
                ? attributes
                    .Class("data" + (groupBy != null
                        ? " link"
                        : string.Empty))
                    .DataSelector(Selector(ss, aggregation.GroupBy))
                    .DataValue(DataValue(groupBy, key))
                : attributes
                    .Class("data");
        }

        private static string DataValue(Column groupBy, string key)
        {
            switch (groupBy.ColumnName)
            {
                case "Status":
                    if (key == "0")
                    {
                        return "\t";
                    }
                    break;
                default:
                    if (groupBy.UserColumn)
                    {
                        if (User.UserTypes.Anonymous.ToInt().ToString() == key)
                        {
                            return "\t";
                        }
                    }
                    break;
            }
            return key != string.Empty ? key : "\t";
        }

        private static HtmlBuilder LabelValue(
            this HtmlBuilder hb,
            string label,
            string value,
            bool bold,
            HtmlAttributes attributes)
        {
            return hb.Span(attributes: attributes, action: () =>
            {
                if (label != string.Empty)
                {
                    hb.Span(
                        css: bold ? "bold" : null,
                        action: () => hb
                            .Text(label));
                }
                hb.Text(value);
            });
        }

        private static bool Bold(Column groupBy, string value)
        {
            return groupBy?.HasChoices() == true
                ? groupBy.ChoiceHash.Get(value) != null ||
                    UserNotSet(groupBy, value) ||
                    NumericZero(groupBy, value) ||
                    StringEmpty(groupBy, value)
                : true;
        }

        private static bool UserNotSet(Column column, string value)
        {
            return column.UserColumn && (value == "0" || value == "2");
        }

        private static bool NumericZero(Column column, string value)
        {
            return column.TypeName.CsTypeSummary() == Types.CsNumeric && value == "0";
        }

        private static bool StringEmpty(Column column, string value)
        {
            return column.TypeName.CsTypeSummary() == Types.CsString && value == string.Empty;
        }
    }
}