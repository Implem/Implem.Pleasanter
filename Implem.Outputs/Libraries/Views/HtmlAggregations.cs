using Implem.Libraries.Classes;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Analysis;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.ServerData;
using Implem.Pleasanter.Libraries.Settings;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Views
{
    public static class HtmlAggregations
    {
        public static HtmlBuilder Aggregations(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            Aggregations aggregations,
            bool container = true)
        {
            return container
                ? hb.Div(
                    id: "Aggregations",
                    css: "aggregations",
                    action: () => hb
                        .Content(siteSettings, aggregations))
                : hb.Content(siteSettings, aggregations);
        }

        private static HtmlBuilder Content(
            this HtmlBuilder hb, SiteSettings siteSettings, Aggregations aggregations)
        {
            return aggregations.TotalCount != 0
                ? hb
                    .Total(aggregations)
                    .Aggregations(siteSettings, aggregations)
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

        private static HtmlBuilder Aggregations(
           this HtmlBuilder hb, SiteSettings siteSettings, Aggregations aggregations)
        {
            aggregations.AggregationCollection
                .Where(o => o.Data.Count > 0)
                .ForEach(aggregation =>
                {
                    var html = string.Empty;
                    var groupByColumn = siteSettings.AllColumn(aggregation.GroupBy);
                    var targetColumn = siteSettings.AllColumn(aggregation.Target);
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
                                ? targetColumn.Format(data.Value) 
                                : data.Value.ToString()) +
                                    (aggregation.Type != Settings.Aggregation.Types.Count
                                        ? targetColumn?.Unit
                                        : string.Empty),
                            attributes: new HtmlAttributes()
                                .Class("data" + (groupByColumn != null
                                    ? " link"
                                    : string.Empty))
                                .DataSelector(Selector(siteSettings, aggregation.GroupBy))
                                .DataValue(data.Key));
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
                ? groupByColumn.LabelText + ": "
                : string.Empty;
            switch (aggregation.Type)
            {
                case Settings.Aggregation.Types.Count:
                    text += Displays.Get(aggregation.Type.ToString());
                    break;
                default:
                    text += targetColumn.LabelText + " " +
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
                return selectedValue.ToInt() != DbUser.UserTypes.Anonymous.ToInt()
                    ? SiteInfo.UserFullName(selectedValue.ToInt())
                    : Displays.NotSet();
            }
            else if (groupByColumn.HasChoices())
            {
                var label = groupByColumn.Choice(selectedValue).TextMini();
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

        private static string Selector(SiteSettings siteSettings, string columnName)
        {
            return "#DataViewFilters_" + siteSettings.ReferenceType + "_" + columnName;
        }
    }
}