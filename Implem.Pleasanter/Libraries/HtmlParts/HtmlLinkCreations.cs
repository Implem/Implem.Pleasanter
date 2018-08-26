using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Linq;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlLinkCreations
    {
        public static HtmlBuilder LinkCreations(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            long linkId,
            BaseModel.MethodTypes methodType)
        {
            var linkCollection = LinkCollection(context: context, ss: ss);
            return
                methodType != BaseModel.MethodTypes.New &&
                linkCollection.Any()
                    ? hb.FieldSet(
                        css: " enclosed link-creations",
                        legendText: Displays.LinkCreations(),
                        action: () => hb
                            .LinkCreations(
                                context: context,
                                ss: ss,
                                linkCollection: linkCollection,
                                linkId: linkId))
                    : hb;
        }

        private static LinkCollection LinkCollection(Context context, SiteSettings ss)
        {
            return new LinkCollection(
                context: context,
                column: Rds.LinksColumn()
                    .SourceId()
                    .SiteTitle(),
                where: Rds.LinksWhere()
                    .DestinationId(ss.SiteId)
                    .SiteId_In(ss.Sources?
                        .Where(currentSs => context.CanCreate(ss: currentSs))
                        .Select(currentSs => currentSs.SiteId)));
        }

        private static HtmlBuilder LinkCreations(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            LinkCollection linkCollection,
            long linkId)
        {
            return hb.Div(action: () => linkCollection.ForEach(linkModel => hb
                .LinkCreationButton(
                    context: context,
                    ss: ss,
                    linkId: linkId,
                    sourceId: linkModel.SourceId,
                    text: linkModel.SiteTitle)));
        }

        private static HtmlBuilder LinkCreationButton(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            long linkId,
            long sourceId,
            string text)
        {
            return hb.Button(
                attributes: new HtmlAttributes()
                    .Class("button button-icon confirm-reload")
                    .OnClick("$p.new($(this));")
                    .Title(SiteInfo.TenantCaches.Get(context.TenantId)?
                        .SiteMenu
                        .Breadcrumb(context: context, siteId: sourceId)
                        .Select(o => o.Title).Join(" > "))
                    .DataId(linkId.ToString())
                    .DataIcon("ui-icon-plus")
                    .Add("data-from-site-id", ss.SiteId.ToString())
                    .Add("data-to-site-id", sourceId.ToString()),
                action: () => hb
                    .Text(text: text));
        }
    }
}