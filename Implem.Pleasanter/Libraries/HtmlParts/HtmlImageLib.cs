using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlImageLib
    {
        public static HtmlBuilder ImageLib(
            this HtmlBuilder hb, SiteSettings ss, ImageLibData imageLibData)
        {
            return hb.Div(
                attributes: new HtmlAttributes()
                    .Id("ImageLib")
                    .Class("both")
                    .DataAction("ImageLibNext")
                    .DataMethod("post"),
                action: () =>
                    hb
                        .Hidden(
                            controlId: "ImageLibOffset",
                            value: ss.ImageLibNextOffset(
                                0,
                                imageLibData.DataRows.Count(),
                                imageLibData.TotalCount)
                                    .ToString())
                        .ImageLibBody(
                            ss: ss,
                            imageLibData: imageLibData));
        }

        public static HtmlBuilder ImageLibBody(
            this HtmlBuilder hb, SiteSettings ss, ImageLibData imageLibData)
        {
            return hb.Div(
                attributes: new HtmlAttributes().Id("ImageLibBody"),
                action: () => imageLibData.DataRows
                    .ForEach(dataRow => hb
                        .ImageLibItem(
                            ss: ss,
                            dataRow: dataRow)));
        }

        public static HtmlBuilder ImageLibItem(
            this HtmlBuilder hb, SiteSettings ss, DataRow dataRow)
        {
            var guid = dataRow.String("Guid");
            var href = Locations.ShowFile(guid);
            return hb.Div(
                attributes: new HtmlAttributes()
                    .Class("item")
                    .DataId(guid),
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
                                    .Img(src: href)))
                    .Button(
                        controlCss: "button-icon delete-image",
                        onClick: $"$p.deleteImage($(this));",
                        dataId: guid,
                        icon: "ui-icon-trash",
                        action: Locations.DeleteImage(guid),
                        method: "delete",
                        confirm: "ConfirmDelete",
                        _using: ss.CanUpdate()));
        }
    }
}