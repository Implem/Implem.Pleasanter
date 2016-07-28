using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Linq;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlLinkCreations
    {
        public static HtmlBuilder LinkCreations(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            long linkId,
            BaseModel.MethodTypes methodType)
        {
            var linkCollection = LinkCollection(siteSettings);
            return
                methodType != BaseModel.MethodTypes.New &&
                linkCollection.Any()
                    ? hb.FieldSet(
                        css: " enclosed link-creations",
                        legendText: Displays.LinkCreations(),
                        action: () => hb
                            .LinkCreations(
                                linkCollection: linkCollection,
                                siteId: siteSettings.SiteId,
                                linkId: linkId))
                    : hb;
        }

        private static LinkCollection LinkCollection(SiteSettings siteSettings)
        {
            return new LinkCollection(
                column: Rds.LinksColumn()
                    .SourceId()
                    .SiteTitle(),
                where: Rds.LinksWhere().DestinationId(siteSettings.SiteId));
        }

        private static HtmlBuilder LinkCreations(
            this HtmlBuilder hb, LinkCollection linkCollection, long siteId, long linkId)
        {
            return hb.Div(action: () => linkCollection.ForEach(linkModel => hb
                .LinkCreationButton(
                    siteId: siteId,
                    linkId: linkId,
                    sourceId: linkModel.SourceId,
                    text: linkModel.SiteTitle)));
        }

        private static HtmlBuilder LinkCreationButton(
            this HtmlBuilder hb, long siteId,long linkId, long sourceId, string text)
        {
            return hb.Button(
                attributes: new HtmlAttributes()
                    .Class("button button-create")
                    .OnClick(Def.JavaScript.NewByLink)
                    .DataId(linkId.ToString())
                    .Add("data-from-site-id", siteId.ToString())
                    .Add("data-to-site-id", sourceId.ToString()),
                action: () => hb
                    .Text(text: text));
        }
    }
}