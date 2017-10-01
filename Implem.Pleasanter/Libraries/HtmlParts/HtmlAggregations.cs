using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System.Linq;
using System.Web;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlAggregations
    {
        public static HtmlBuilder Aggregations(
            this HtmlBuilder hb, SiteSettings ss, Aggregations aggregations)
        {
            return !Reduced(ss.SiteId)
                ? hb.Div(
                    id: "Aggregations",
                    action: () => hb
                        .DisplayControl(
                            id: "ReduceAggregations",
                            icon: "ui-icon-close")
                        .Contents(ss, aggregations))
                : hb.Div(
                    id: "Aggregations",
                    css: "reduced",
                    action: () => hb
                        .DisplayControl(
                            id: "ExpandAggregations",
                            icon: "ui-icon-folder-open"));
        }

        private static bool Reduced(long siteId)
        {
            var key = "ReduceAggregations_" + (siteId == 0
                ? Pages.Key()
                : siteId.ToString());
            if (Forms.ControlId() == "ReduceAggregations")
            {
                HttpContext.Current.Session[key] = true;
            }
            else if (Forms.ControlId() == "ExpandAggregations")
            {
                HttpContext.Current.Session.Remove(key);
            }
            return HttpContext.Current.Session[key].ToBool();
        }

        private static HtmlBuilder DisplayControl(this HtmlBuilder hb, string id, string icon)
        {
            return hb.Div(
                attributes: new HtmlAttributes()
                    .Id(id)
                    .Class("display-control")
                    .OnClick("$p.send($(this));")
                    .DataMethod("post"),
                action: () => hb
                    .Span(css: "ui-icon " + icon)
                    .Text(text: Displays.Aggregations() + ":"));
        }

        private static HtmlBuilder Contents(
            this HtmlBuilder hb, SiteSettings ss, Aggregations aggregations)
        {
            return aggregations.TotalCount != 0
                ? hb
                    .Total(aggregations)
                    .Overdue(aggregations)
                    .Parts(ss, aggregations)
                : hb.Span(css: "label", action: () => hb
                    .Text(text: Displays.NoData()));
        }

        private static HtmlBuilder Total(this HtmlBuilder hb, Aggregations aggregations)
        {
            return hb
                .Span(css: "label", action: () => hb
                    .Text(text: Displays.Quantity()))
                .Span(css: "data", action: () => hb
                    .Text(text: aggregations.TotalCount.ToString()));
        }

        private static HtmlBuilder Overdue(this HtmlBuilder hb, Aggregations aggregations)
        {
            return aggregations.OverdueCount > 0
                ? hb
                    .Span(css: "label overdue", action: () => hb
                        .Text(text: Displays.Overdue()))
                    .Span(css: "data overdue", action: () => hb
                        .Text(text: aggregations.OverdueCount.ToString()))
                : hb;
        }

        private static HtmlBuilder Parts(
            this HtmlBuilder hb, SiteSettings ss, Aggregations aggregations)
        {
            var allowedColumns = Permissions.AllowedColumns(ss);
            aggregations.AggregationCollection
                .Where(o => o.Target.IsNullOrEmpty() || allowedColumns.Contains(o.Target))
                .Where(o => o.GroupBy == "[NotGroupBy]" || allowedColumns.Contains(o.GroupBy))
                .Where(o => o.Data.Count > 0)
                .ForEach(aggregation =>
                {
                    var html = string.Empty;
                    var groupBy = ss.GetColumn(aggregation.GroupBy);
                    var targetColumn = ss.GetColumn(aggregation.Target);
                    if (aggregation.Data.Count > 0)
                        hb.GroupBy(
                            groupBy: groupBy,
                            targetColumn: targetColumn,
                            aggregation: aggregation);
                    aggregation.Data.OrderByDescending(o => o.Value).ForEach(data =>
                        hb.LabelValue(
                            label: groupBy != null
                                ? Label(
                                    groupBy: groupBy,
                                    selectedValue: data.Key)
                                : string.Empty,
                            value: (targetColumn != null
                                ? targetColumn.Display(data.Value)
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
                    text += Displays.Get(aggregation.Type.ToString());
                    break;
                default:
                    text += targetColumn.GridLabelText + " " +
                        Displays.Get(aggregation.Type.ToString());
                    break;
            }
            return hb.Span(css: "label", action: () => hb
                .Text(text: text));
        }

        private static string Label(Column groupBy, string selectedValue)
        {
            if (groupBy.UserColumn)
            {
                return SiteInfo.UserName(selectedValue.ToInt());
            }
            else if (groupBy.HasChoices())
            {
                var label = groupBy.Choice(selectedValue).TextMini;
                return label.IsNullOrEmpty()
                    ? NumericZero(groupBy, selectedValue)
                        ? Displays.NotSet()
                        : StringEmpty(groupBy, selectedValue)
                            ? Displays.NotSet()
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