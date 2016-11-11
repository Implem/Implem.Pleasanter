using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
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
            aggregations.AggregationCollection
                .Where(o => o.Data.Count > 0)
                .ForEach(aggregation =>
                {
                    var html = string.Empty;
                    var groupByColumn = ss.GetColumn(aggregation.GroupBy);
                    var targetColumn = ss.GetColumn(aggregation.Target);
                    if (aggregation.Data.Count > 0)
                    hb.GroupBy(
                        groupByColumn: groupByColumn,
                        targetColumn: targetColumn,
                        aggregation: aggregation);
                    aggregation.Data.OrderByDescending(o => o.Value).ForEach(data =>
                    {
                        hb.LabelValue(
                            label: groupByColumn != null
                                ? Label(
                                    groupByColumn: groupByColumn,
                                    selectedValue: data.Key)
                                : string.Empty,
                            value: (targetColumn != null 
                                ? targetColumn.Display(data.Value) 
                                : data.Value.ToString()) +
                                    (aggregation.Type != Aggregation.Types.Count
                                        ? targetColumn?.Unit
                                        : string.Empty),
                            attributes: new HtmlAttributes()
                                .Attributes(ss, aggregation, groupByColumn, data.Key));
                    });
                });
            return hb;
        }

        private static HtmlBuilder GroupBy(
            this HtmlBuilder hb,
            Column groupByColumn,
            Column targetColumn,
            Aggregation aggregation)
        {
            var text = groupByColumn != null
                ? groupByColumn.GridLabelText + ": "
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

        private static string Label(Column groupByColumn, string selectedValue)
        {
            if (groupByColumn.UserColumn)
            {
                return SiteInfo.UserFullName(selectedValue.ToInt());
            }
            else if (groupByColumn.HasChoices())
            {
                var label = groupByColumn.Choice(selectedValue).TextMini;
                if (groupByColumn.TypeName.CsTypeSummary() == Types.CsNumeric && label == "0")
                {
                    return Displays.NotSet();
                }
                return label != string.Empty
                    ? label
                    : Displays.NotSet();
            }
            else
            {
                return selectedValue;
            }
        }

        private static string Selector(SiteSettings ss, string columnName)
        {
            return "#DataViewFilters_" + ss.ReferenceType + "_" + columnName;
        }

        private static HtmlAttributes Attributes(
            this HtmlAttributes attributes,
            SiteSettings ss,
            Aggregation aggregation,
            Column groupByColumn, string key)
        {
            return groupByColumn != null
                ? attributes
                    .Class("data" + (groupByColumn != null
                        ? " link"
                        : string.Empty))
                    .DataSelector(Selector(ss, aggregation.GroupBy))
                    .DataValue(DataValue(groupByColumn, key))
                : attributes
                    .Class("data");
        }

        private static string DataValue(Column groupByColumn, string key)
        {
            if (groupByColumn.UserColumn)
            {
                if (User.UserTypes.Anonymous.ToInt().ToString() == key)
                {
                    return "\t";
                }
            }
            return key != string.Empty ? key : "\t";
        }
    }
}