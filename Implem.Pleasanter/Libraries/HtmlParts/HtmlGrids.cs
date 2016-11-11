using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
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
            FormData formData = null,
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
                                        .Id("GridSorters_" + column.Id)
                                        .Add("data-order-type", GridSorters
                                            .TypeString(formData, "GridSorters_" + column.Id))
                                        .DataAction("GridRows")
                                        .DataMethod("post"),
                                    action: () => hb
                                        .Span(action: () => hb
                                            .Text(text: Displays.Get(column.GridLabelText)))
                                        .SortIcon(
                                            formData: formData,
                                            key: "GridSorters_" + column.Id)));
                        }
                        else
                        {
                            hb.Th(action: () => hb
                                .Text(text: Displays.Get(column.GridLabelText)));
                        }
                    });
                });
        }
    }
}