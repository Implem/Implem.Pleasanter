using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Responses;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlImageLib
    {
        public static HtmlBuilder ImageLib(this HtmlBuilder hb,
            EnumerableRowCollection<DataRow> dataRows)
        {
            return hb.Div(id: "ImageLib", css: "both", action: () =>
                hb.ImageLibBody(dataRows: dataRows));
        }

        public static HtmlBuilder ImageLibBody(this HtmlBuilder hb,
            EnumerableRowCollection<DataRow> dataRows)
        {
            return hb.Div(
                attributes: new HtmlAttributes().Id("ImageLibBody"),
                action: () => dataRows
                    .ForEach(dataRow => hb
                        .ImageLibItem(dataRow)));
        }

        private static HtmlBuilder ImageLibItem(this HtmlBuilder hb, DataRow dataRow)
        {
            var href = Locations.ShowFile(dataRow.String("Guid"));
            return hb.Div(
                css: "item",
                action: () => hb
                    .Div(
                        css: "title",
                        action: () => hb
                            .A(
                                href: Locations.ItemEdit(dataRow.Long("Id")),
                                action: () => hb
                                    .Text(text: dataRow.String("ItemTitle"))))
                    .Div(
                        css: "image",
                        action: () => hb
                            .A(
                                href: href,
                                action: () => hb
                                    .Img(src: href))));
        }
    }
}