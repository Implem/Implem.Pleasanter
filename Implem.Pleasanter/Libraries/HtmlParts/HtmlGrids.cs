using Implem.Libraries.DataSources.SqlServer;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlGrids
    {
        public static HtmlBuilder GridHeader(
            this HtmlBuilder hb,
            IEnumerable<Column> columnCollection, 
            DataView dataView = null,
            bool sort = true,
            bool checkAll = false,
            bool checkRow = true)
        {
            return hb.Tr(
                css: "ui-widget-header",
                action: () => 
                {
                    if (checkRow)
                    {
                        hb.Th(action: () => hb
                            .CheckBox(
                                controlId: "GridCheckAll",
                                _checked: checkAll));
                    }
                    columnCollection.ForEach(column =>
                    {
                        if (sort)
                        {
                            hb.Th(css: "sortable", action: () => hb
                                .Div(
                                    attributes: new HtmlAttributes()
                                        .Id("DataViewSorters_" + column.Id)
                                        .Add("data-order-type", OrderBy(
                                            dataView, column.ColumnName))
                                        .DataAction("GridRows")
                                        .DataMethod("post"),
                                    action: () => hb
                                        .Span(action: () => hb
                                            .Text(text: Displays.Get(column.GridLabelText)))
                                        .SortIcon(
                                            dataView: dataView,
                                            key: column.ColumnName)));
                        }
                        else
                        {
                            hb.Th(action: () => hb
                                .Text(text: Displays.Get(column.GridLabelText)));
                        }
                    });
                });
        }

        private static string OrderBy(DataView dataView, string key)
        {
            switch (dataView.ColumnSorter(key))
            {
                case SqlOrderBy.Types.asc: return "Desc";
                case SqlOrderBy.Types.desc: return string.Empty;
                default: return "Asc";
            }
        }

        public static HtmlBuilder SortIcon(this HtmlBuilder hb, DataView dataView, string key)
        {
            switch (dataView.ColumnSorter(key))
            {
                case SqlOrderBy.Types.asc: return hb.Icon(iconCss: "ui-icon-triangle-1-n");
                case SqlOrderBy.Types.desc: return hb.Icon(iconCss: "ui-icon-triangle-1-s");
                default: return hb;
            }
        }
    }
}