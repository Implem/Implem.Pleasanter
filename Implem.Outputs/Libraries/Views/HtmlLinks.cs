using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Converts;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Views
{
    public static class HtmlLinks
    {
        private enum Types
        {
            Destination,
            Source
        }

        public static HtmlBuilder Links(this HtmlBuilder hb, long linkId)
        {
            var destinations = Destinations(linkId);
            var sources = Sources(linkId);
            return destinations.Count > 0 || sources.Count > 0
                ? hb.FieldSet(
                    css: " enclosed",
                    legendText: Displays.Links(),
                    action: () => hb
                        .Links(
                            linkId: linkId,
                            destinations: destinations,
                            sources: sources))
                : hb;
        }

        private static HtmlBuilder Links(
            this HtmlBuilder hb,
            long linkId,
            LinkCollection destinations,
            LinkCollection sources)
        {
            return hb.Div(action: () => hb
                .LinkDestinations(
                    linkId: linkId,
                    linkCollection: destinations)
                .LinkSources(linkId: linkId, sources: sources));
        }

        private static LinkCollection Destinations(long linkId)
        {
            return new LinkCollection(
                join: LinksUtility.JoinByDestination(),
                where: Rds.LinksWhere().SourceId(linkId));
        }

        private static LinkCollection Sources(long linkId)
        {
            return new LinkCollection(where: Rds.LinksWhere()
                .DestinationId(linkId)
                .ReferenceType("Issues"));
        }

        public static HtmlBuilder LinkDestinations(
            this HtmlBuilder hb, long linkId, LinkCollection linkCollection)
        {
            return hb.LinkTable(
                linkCollection: linkCollection,
                caption: Displays.LinkDestinations(),
                type: Types.Source);
        }

        public static HtmlBuilder LinkSources(
            this HtmlBuilder hb, long linkId, LinkCollection sources)
        {
            return hb.LinkTable(
                linkCollection: sources,
                caption: Displays.LinkSources(),
                type: Types.Destination);
        }

        private static HtmlBuilder LinkTable(
            this HtmlBuilder hb,
            LinkCollection linkCollection,
            string caption,
            Types type)
        {
            if (linkCollection.Count > 0)
            {
                var siteCollection = new SiteCollection(
                    where: Rds.SitesWhere()
                        .SiteId_In( linkCollection.Select(o => o.SiteId).Distinct()));
                siteCollection.ForEach(siteModel => hb
                    .Table(css: "grid", action: () =>
                    {
                        switch (siteModel.SiteSettings.ReferenceType)
                        {
                            case "Issues":
                                hb
                                    .Caption(caption: caption + " : " + siteModel.Title.Value)
                                    .IssuesLinkHeader(siteModel: siteModel, type: type)
                                    .IssuesRows(
                                        linkCollection: linkCollection.Where(o => o.SiteId == siteModel.SiteId),
                                        siteSettings: siteModel.IssuesSiteSettings(),
                                        type: type);
                                break;
                            case "Results":
                                hb
                                    .Caption(caption: caption + " : " + siteModel.Title.Value)
                                    .ResultsLinkHeader(siteModel: siteModel, type: type)
                                    .ResultsRows(
                                        linkCollection: linkCollection.Where(o => o.SiteId == siteModel.SiteId),
                                        siteSettings: siteModel.ResultsSiteSettings(),
                                        type: type);
                                break;
                            case "Wikis":
                                hb
                                    .Caption(caption: caption + " : " + siteModel.Title.Value)
                                    .WikisLinkHeader(siteModel: siteModel, type: type)
                                    .WikisRows(
                                        linkCollection: linkCollection.Where(o => o.SiteId == siteModel.SiteId),
                                        siteSettings: siteModel.WikisSiteSettings(),
                                        type: type);
                                break;
                        }
                    }));
            }
            return hb;
        }

        private static HtmlBuilder LinkHeader(
            this HtmlBuilder hb, SiteModel siteModel, Types type)
        {
            switch (siteModel.ReferenceType)
            {
                case "Issues": return hb.IssuesLinkHeader(siteModel: siteModel, type: type);
                case "Results": return hb.ResultsLinkHeader(siteModel: siteModel, type: type);
                case "Wikis": return hb.WikisLinkHeader(siteModel: siteModel, type: type);
                default: return hb;
            }
        }

        private static HtmlBuilder IssuesLinkHeader(
            this HtmlBuilder hb, SiteModel siteModel, Types type)
        {
            var siteSettings = siteModel.IssuesSiteSettings();
            return hb.Tr(css: "ui-widget-header", action: () =>
            {
                hb.Th(action: () => hb
                    .Text(text: Displays.Get(type == Types.Source
                        ? Def.ColumnTable.Links_DestinationId.ColumnLabel
                        : Def.ColumnTable.Links_SourceId.ColumnLabel)));
                hb.Th(action: () => hb
                    .Text(text: siteSettings.EditorColumn("IssueId").LabelText));
                hb.Th(action: () => hb
                    .Text(text: siteSettings.EditorColumn("Title").LabelText));
                hb.Th(action: () => hb
                    .Text(text: siteSettings.EditorColumn("StartTime").LabelText));
                hb.Th(action: () => hb
                    .Text(text: siteSettings.EditorColumn("CompletionTime").LabelText));
                hb.Th(action: () => hb
                    .Text(text: siteSettings.EditorColumn("WorkValue").LabelText));
                hb.Th(action: () => hb
                    .Text(text: siteSettings.EditorColumn("ProgressRate").LabelText));
                hb.Th(action: () => hb
                    .Text(text: siteSettings.EditorColumn("RemainingWorkValue").LabelText));
                hb.Th(action: () => hb
                    .Text(text: siteSettings.EditorColumn("Status").LabelText));
                hb.Th(action: () => hb
                    .Text(text: siteSettings.EditorColumn("Owner").LabelText));
            });
        }

        private static HtmlBuilder ResultsLinkHeader(
            this HtmlBuilder hb, SiteModel siteModel, Types type)
        {
            var siteSettings = siteModel.ResultsSiteSettings();
            return hb.Tr(css: "ui-widget-header", action: () =>
            {
                hb.Th(action: () => hb
                    .Text(text: Displays.Get(type == Types.Source
                        ? Def.ColumnTable.Links_DestinationId.ColumnLabel
                        : Def.ColumnTable.Links_SourceId.ColumnLabel)));
                hb.Th(action: () => hb
                    .Text(text: siteSettings.EditorColumn("ResultId").LabelText));
                hb.Th(action: () => hb
                    .Text(text: siteSettings.EditorColumn("Title").LabelText));
                hb.Th(action: () => hb
                    .Text(text: siteSettings.EditorColumn("Owner").LabelText));
            });
        }

        private static HtmlBuilder WikisLinkHeader(
            this HtmlBuilder hb, SiteModel siteModel, Types type)
        {
            var siteSettings = siteModel.WikisSiteSettings();
            return hb.Tr(css: "ui-widget-header", action: () =>
            {
                hb.Th(action: () => hb
                    .Text(text: Displays.Get(type == Types.Source
                        ? Def.ColumnTable.Links_DestinationId.ColumnLabel
                        : Def.ColumnTable.Links_SourceId.ColumnLabel)));
                hb.Th(action: () => hb
                    .Text(text: siteSettings.EditorColumn("WikiId").LabelText));
                hb.Th(action: () => hb
                    .Text(text: siteSettings.EditorColumn("Title").LabelText));
            });
        }

        private static HtmlBuilder IssuesRows(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            IEnumerable<LinkModel> linkCollection,
            Types type)
        {
            linkCollection.ForEach(linkModel =>
            {
                var issueSubset = linkModel.Subset.Deserialize<IssueSubset>();
                if (issueSubset != null)
                {
                    var id = type == Types.Source
                        ? linkModel.DestinationId.ToString()
                        : linkModel.SourceId.ToString();
                    hb.Tr(
                        attributes: Html.Attributes()
                            .Class("grid-row")
                            .DataId(id),
                        action: () =>
                        {
                            hb.Td(action: () => hb
                                .Text(text: id));
                            hb.Td(
                                column: siteSettings.EditorColumn("IssueId"),
                                value: issueSubset.IssueId);
                            hb.Td(
                                column: siteSettings.EditorColumn("Title"),
                                value: linkModel.Title);
                            hb.Td(
                                column: siteSettings.EditorColumn("StartTime"),
                                value: issueSubset.StartTime);
                            hb.Td(
                                column: siteSettings.EditorColumn("CompletionTime"),
                                value: issueSubset.CompletionTime);
                            hb.Td(
                                column: siteSettings.EditorColumn("WorkValue"),
                                value: issueSubset.WorkValue);
                            hb.Td(
                                column: siteSettings.EditorColumn("ProgressRate"),
                                value: issueSubset.ProgressRate);
                            hb.Td(
                                column: siteSettings.EditorColumn("RemainingWorkValue"),
                                value: issueSubset.RemainingWorkValue);
                            hb.Td(
                                column: siteSettings.EditorColumn("Status"),
                                value: issueSubset.Status);
                            hb.Td(
                                column: siteSettings.EditorColumn("Owner"),
                                value: issueSubset.Owner);
                        });
                }
            });
            return hb;
        }

        private static HtmlBuilder ResultsRows(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            IEnumerable<LinkModel> linkCollection,
            Types type)
        {
            linkCollection.ForEach(linkModel =>
            {
                var resultSubset = linkModel.Subset.Deserialize<ResultSubset>();
                if (resultSubset != null)
                {
                    var id = type == Types.Source
                        ? linkModel.DestinationId.ToString()
                        : linkModel.SourceId.ToString();
                    hb.Tr(
                        attributes: Html.Attributes()
                            .Class("grid-row")
                            .DataId(id),
                        action: () =>
                        {
                            hb.Td(action: () => hb
                                .Text(text: id));
                            hb.Td(
                                column: siteSettings.EditorColumn("ResultId"),
                                value: resultSubset.ResultId);
                            hb.Td(
                                column: siteSettings.EditorColumn("Title"),
                                value: linkModel.Title);
                            hb.Td(
                                column: siteSettings.EditorColumn("Owner"),
                                value: resultSubset.Owner);
                        });
                }
            });
            return hb;
        }

        private static HtmlBuilder WikisRows(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            IEnumerable<LinkModel> linkCollection,
            Types type)
        {
            linkCollection.ForEach(linkModel =>
            {
                var wikiSubset = linkModel.Subset.Deserialize<WikiSubset>();
                if (wikiSubset != null)
                {
                    var id = type == Types.Source
                        ? linkModel.DestinationId.ToString()
                        : linkModel.SourceId.ToString();
                    hb.Tr(
                        attributes: Html.Attributes()
                            .Class("grid-row")
                            .DataId(id),
                        action: () =>
                        {
                            hb.Td(action: () => hb
                                .Text(text: id));
                            hb.Td(
                                column: siteSettings.EditorColumn("WikiId"),
                                value: wikiSubset.WikiId);
                            hb.Td(
                                column: siteSettings.EditorColumn("Title"),
                                value: linkModel.Title);
                        });
                }
            });
            return hb;
        }
    }
}
