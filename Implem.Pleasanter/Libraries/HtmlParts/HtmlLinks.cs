using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Converts;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.HtmlParts
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
                join: LinkUtilities.JoinByDestination(),
                where: Rds.LinksWhere().SourceId(linkId));
        }

        private static LinkCollection Sources(long linkId)
        {
            return new LinkCollection(where: Rds.LinksWhere().DestinationId(linkId));
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
                        .SiteId_In(linkCollection.Select(o => o.SiteId).Distinct()));
                siteCollection.ForEach(siteModel => hb
                    .Table(css: "grid", action: () =>
                    {
                        var siteLinkCollection = linkCollection
                            .Where(o => o.SiteId == siteModel.SiteId);
                        switch (siteModel.SiteSettings.ReferenceType)
                        {
                            case "Issues":
                                hb
                                    .Caption(caption: "{0} : {1} {2} - {3} {4}".Params(
                                        caption,
                                        siteModel.SiteId,
                                        siteModel.Title.Value,
                                        Displays.Quantity(),
                                        siteLinkCollection.Count()))
                                    .IssuesLinkHeader(siteModel: siteModel, type: type)
                                    .IssuesRows(
                                        linkCollection: siteLinkCollection,
                                        siteSettings: siteModel.IssuesSiteSettings(),
                                        type: type);
                                break;
                            case "Results":
                                hb
                                    .Caption(caption: "{0} : {1} {2} - {3} {4}".Params(
                                        caption,
                                        siteModel.SiteId,
                                        siteModel.Title.Value,
                                        Displays.Quantity(),
                                        siteLinkCollection.Count()))
                                    .ResultsLinkHeader(siteModel: siteModel, type: type)
                                    .ResultsRows(
                                        linkCollection: siteLinkCollection,
                                        siteSettings: siteModel.ResultsSiteSettings(),
                                        type: type);
                                break;
                            case "Wikis":
                                hb
                                    .Caption(caption: "{0} : {1} {2} - {3} {4}".Params(
                                        caption,
                                        siteModel.SiteId,
                                        siteModel.Title.Value,
                                        Displays.Quantity(),
                                        siteLinkCollection.Count()))
                                    .WikisLinkHeader(siteModel: siteModel, type: type)
                                    .WikisRows(
                                        linkCollection: siteLinkCollection,
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
                siteSettings
                    .LinkColumnCollection()
                    .Where(o => o.LinkVisible.ToBool())
                    .ForEach(column => hb
                        .Th(action: () => hb
                            .Text(text: column.LabelText)));
            });
        }

        private static HtmlBuilder ResultsLinkHeader(
            this HtmlBuilder hb, SiteModel siteModel, Types type)
        {
            var siteSettings = siteModel.ResultsSiteSettings();
            return hb.Tr(css: "ui-widget-header", action: () =>
            {
                siteSettings
                    .LinkColumnCollection()
                    .Where(o => o.LinkVisible.ToBool())
                    .ForEach(column => hb
                        .Th(action: () => hb
                            .Text(text: column.LabelText)));
            });
        }

        private static HtmlBuilder WikisLinkHeader(
            this HtmlBuilder hb, SiteModel siteModel, Types type)
        {
            var siteSettings = siteModel.WikisSiteSettings();
            return hb.Tr(css: "ui-widget-header", action: () =>
            {
                siteSettings
                    .LinkColumnCollection()
                    .Where(o => o.LinkVisible.ToBool())
                    .ForEach(column => hb
                        .Th(action: () => hb
                            .Text(text: column.LabelText)));
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
                        attributes: new HtmlAttributes()
                            .Class("grid-row")
                            .DataId(id),
                        action: () =>
                        {
                            siteSettings
                                .LinkColumnCollection()
                                .Where(o => o.LinkVisible.ToBool())
                                .ForEach(column =>
                                {
                                    switch (column.ColumnName)
                                    {
                                        case "SiteId":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("SiteId"),
                                        	    value: issueSubset.SiteId);
                                        	break;
                                        case "UpdatedTime":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("UpdatedTime"),
                                        	    value: issueSubset.UpdatedTime);
                                        	break;
                                        case "IssueId":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("IssueId"),
                                        	    value: issueSubset.IssueId);
                                        	break;
                                        case "Ver":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("Ver"),
                                        	    value: issueSubset.Ver);
                                        	break;
                                        case "Title":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("Title"),
                                        	    value: issueSubset.Title);
                                        	break;
                                        case "Body":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("Body"),
                                        	    value: issueSubset.Body);
                                        	break;
                                        case "TitleBody":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("TitleBody"),
                                        	    value: issueSubset.TitleBody);
                                        	break;
                                        case "StartTime":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("StartTime"),
                                        	    value: issueSubset.StartTime);
                                        	break;
                                        case "CompletionTime":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CompletionTime"),
                                        	    value: issueSubset.CompletionTime);
                                        	break;
                                        case "WorkValue":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("WorkValue"),
                                        	    value: issueSubset.WorkValue);
                                        	break;
                                        case "ProgressRate":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ProgressRate"),
                                        	    value: issueSubset.ProgressRate);
                                        	break;
                                        case "RemainingWorkValue":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("RemainingWorkValue"),
                                        	    value: issueSubset.RemainingWorkValue);
                                        	break;
                                        case "Status":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("Status"),
                                        	    value: issueSubset.Status);
                                        	break;
                                        case "Manager":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("Manager"),
                                        	    value: issueSubset.Manager);
                                        	break;
                                        case "Owner":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("Owner"),
                                        	    value: issueSubset.Owner);
                                        	break;
                                        case "ClassA":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassA"),
                                        	    value: issueSubset.ClassA);
                                        	break;
                                        case "ClassB":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassB"),
                                        	    value: issueSubset.ClassB);
                                        	break;
                                        case "ClassC":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassC"),
                                        	    value: issueSubset.ClassC);
                                        	break;
                                        case "ClassD":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassD"),
                                        	    value: issueSubset.ClassD);
                                        	break;
                                        case "ClassE":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassE"),
                                        	    value: issueSubset.ClassE);
                                        	break;
                                        case "ClassF":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassF"),
                                        	    value: issueSubset.ClassF);
                                        	break;
                                        case "ClassG":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassG"),
                                        	    value: issueSubset.ClassG);
                                        	break;
                                        case "ClassH":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassH"),
                                        	    value: issueSubset.ClassH);
                                        	break;
                                        case "ClassI":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassI"),
                                        	    value: issueSubset.ClassI);
                                        	break;
                                        case "ClassJ":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassJ"),
                                        	    value: issueSubset.ClassJ);
                                        	break;
                                        case "ClassK":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassK"),
                                        	    value: issueSubset.ClassK);
                                        	break;
                                        case "ClassL":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassL"),
                                        	    value: issueSubset.ClassL);
                                        	break;
                                        case "ClassM":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassM"),
                                        	    value: issueSubset.ClassM);
                                        	break;
                                        case "ClassN":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassN"),
                                        	    value: issueSubset.ClassN);
                                        	break;
                                        case "ClassO":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassO"),
                                        	    value: issueSubset.ClassO);
                                        	break;
                                        case "ClassP":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassP"),
                                        	    value: issueSubset.ClassP);
                                        	break;
                                        case "ClassQ":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassQ"),
                                        	    value: issueSubset.ClassQ);
                                        	break;
                                        case "ClassR":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassR"),
                                        	    value: issueSubset.ClassR);
                                        	break;
                                        case "ClassS":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassS"),
                                        	    value: issueSubset.ClassS);
                                        	break;
                                        case "ClassT":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassT"),
                                        	    value: issueSubset.ClassT);
                                        	break;
                                        case "ClassU":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassU"),
                                        	    value: issueSubset.ClassU);
                                        	break;
                                        case "ClassV":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassV"),
                                        	    value: issueSubset.ClassV);
                                        	break;
                                        case "ClassW":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassW"),
                                        	    value: issueSubset.ClassW);
                                        	break;
                                        case "ClassX":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassX"),
                                        	    value: issueSubset.ClassX);
                                        	break;
                                        case "ClassY":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassY"),
                                        	    value: issueSubset.ClassY);
                                        	break;
                                        case "ClassZ":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassZ"),
                                        	    value: issueSubset.ClassZ);
                                        	break;
                                        case "NumA":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumA"),
                                        	    value: issueSubset.NumA);
                                        	break;
                                        case "NumB":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumB"),
                                        	    value: issueSubset.NumB);
                                        	break;
                                        case "NumC":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumC"),
                                        	    value: issueSubset.NumC);
                                        	break;
                                        case "NumD":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumD"),
                                        	    value: issueSubset.NumD);
                                        	break;
                                        case "NumE":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumE"),
                                        	    value: issueSubset.NumE);
                                        	break;
                                        case "NumF":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumF"),
                                        	    value: issueSubset.NumF);
                                        	break;
                                        case "NumG":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumG"),
                                        	    value: issueSubset.NumG);
                                        	break;
                                        case "NumH":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumH"),
                                        	    value: issueSubset.NumH);
                                        	break;
                                        case "NumI":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumI"),
                                        	    value: issueSubset.NumI);
                                        	break;
                                        case "NumJ":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumJ"),
                                        	    value: issueSubset.NumJ);
                                        	break;
                                        case "NumK":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumK"),
                                        	    value: issueSubset.NumK);
                                        	break;
                                        case "NumL":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumL"),
                                        	    value: issueSubset.NumL);
                                        	break;
                                        case "NumM":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumM"),
                                        	    value: issueSubset.NumM);
                                        	break;
                                        case "NumN":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumN"),
                                        	    value: issueSubset.NumN);
                                        	break;
                                        case "NumO":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumO"),
                                        	    value: issueSubset.NumO);
                                        	break;
                                        case "NumP":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumP"),
                                        	    value: issueSubset.NumP);
                                        	break;
                                        case "NumQ":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumQ"),
                                        	    value: issueSubset.NumQ);
                                        	break;
                                        case "NumR":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumR"),
                                        	    value: issueSubset.NumR);
                                        	break;
                                        case "NumS":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumS"),
                                        	    value: issueSubset.NumS);
                                        	break;
                                        case "NumT":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumT"),
                                        	    value: issueSubset.NumT);
                                        	break;
                                        case "NumU":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumU"),
                                        	    value: issueSubset.NumU);
                                        	break;
                                        case "NumV":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumV"),
                                        	    value: issueSubset.NumV);
                                        	break;
                                        case "NumW":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumW"),
                                        	    value: issueSubset.NumW);
                                        	break;
                                        case "NumX":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumX"),
                                        	    value: issueSubset.NumX);
                                        	break;
                                        case "NumY":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumY"),
                                        	    value: issueSubset.NumY);
                                        	break;
                                        case "NumZ":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumZ"),
                                        	    value: issueSubset.NumZ);
                                        	break;
                                        case "DateA":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateA"),
                                        	    value: issueSubset.DateA);
                                        	break;
                                        case "DateB":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateB"),
                                        	    value: issueSubset.DateB);
                                        	break;
                                        case "DateC":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateC"),
                                        	    value: issueSubset.DateC);
                                        	break;
                                        case "DateD":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateD"),
                                        	    value: issueSubset.DateD);
                                        	break;
                                        case "DateE":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateE"),
                                        	    value: issueSubset.DateE);
                                        	break;
                                        case "DateF":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateF"),
                                        	    value: issueSubset.DateF);
                                        	break;
                                        case "DateG":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateG"),
                                        	    value: issueSubset.DateG);
                                        	break;
                                        case "DateH":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateH"),
                                        	    value: issueSubset.DateH);
                                        	break;
                                        case "DateI":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateI"),
                                        	    value: issueSubset.DateI);
                                        	break;
                                        case "DateJ":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateJ"),
                                        	    value: issueSubset.DateJ);
                                        	break;
                                        case "DateK":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateK"),
                                        	    value: issueSubset.DateK);
                                        	break;
                                        case "DateL":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateL"),
                                        	    value: issueSubset.DateL);
                                        	break;
                                        case "DateM":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateM"),
                                        	    value: issueSubset.DateM);
                                        	break;
                                        case "DateN":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateN"),
                                        	    value: issueSubset.DateN);
                                        	break;
                                        case "DateO":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateO"),
                                        	    value: issueSubset.DateO);
                                        	break;
                                        case "DateP":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateP"),
                                        	    value: issueSubset.DateP);
                                        	break;
                                        case "DateQ":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateQ"),
                                        	    value: issueSubset.DateQ);
                                        	break;
                                        case "DateR":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateR"),
                                        	    value: issueSubset.DateR);
                                        	break;
                                        case "DateS":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateS"),
                                        	    value: issueSubset.DateS);
                                        	break;
                                        case "DateT":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateT"),
                                        	    value: issueSubset.DateT);
                                        	break;
                                        case "DateU":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateU"),
                                        	    value: issueSubset.DateU);
                                        	break;
                                        case "DateV":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateV"),
                                        	    value: issueSubset.DateV);
                                        	break;
                                        case "DateW":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateW"),
                                        	    value: issueSubset.DateW);
                                        	break;
                                        case "DateX":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateX"),
                                        	    value: issueSubset.DateX);
                                        	break;
                                        case "DateY":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateY"),
                                        	    value: issueSubset.DateY);
                                        	break;
                                        case "DateZ":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateZ"),
                                        	    value: issueSubset.DateZ);
                                        	break;
                                        case "DescriptionA":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionA"),
                                        	    value: issueSubset.DescriptionA);
                                        	break;
                                        case "DescriptionB":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionB"),
                                        	    value: issueSubset.DescriptionB);
                                        	break;
                                        case "DescriptionC":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionC"),
                                        	    value: issueSubset.DescriptionC);
                                        	break;
                                        case "DescriptionD":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionD"),
                                        	    value: issueSubset.DescriptionD);
                                        	break;
                                        case "DescriptionE":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionE"),
                                        	    value: issueSubset.DescriptionE);
                                        	break;
                                        case "DescriptionF":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionF"),
                                        	    value: issueSubset.DescriptionF);
                                        	break;
                                        case "DescriptionG":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionG"),
                                        	    value: issueSubset.DescriptionG);
                                        	break;
                                        case "DescriptionH":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionH"),
                                        	    value: issueSubset.DescriptionH);
                                        	break;
                                        case "DescriptionI":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionI"),
                                        	    value: issueSubset.DescriptionI);
                                        	break;
                                        case "DescriptionJ":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionJ"),
                                        	    value: issueSubset.DescriptionJ);
                                        	break;
                                        case "DescriptionK":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionK"),
                                        	    value: issueSubset.DescriptionK);
                                        	break;
                                        case "DescriptionL":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionL"),
                                        	    value: issueSubset.DescriptionL);
                                        	break;
                                        case "DescriptionM":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionM"),
                                        	    value: issueSubset.DescriptionM);
                                        	break;
                                        case "DescriptionN":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionN"),
                                        	    value: issueSubset.DescriptionN);
                                        	break;
                                        case "DescriptionO":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionO"),
                                        	    value: issueSubset.DescriptionO);
                                        	break;
                                        case "DescriptionP":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionP"),
                                        	    value: issueSubset.DescriptionP);
                                        	break;
                                        case "DescriptionQ":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionQ"),
                                        	    value: issueSubset.DescriptionQ);
                                        	break;
                                        case "DescriptionR":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionR"),
                                        	    value: issueSubset.DescriptionR);
                                        	break;
                                        case "DescriptionS":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionS"),
                                        	    value: issueSubset.DescriptionS);
                                        	break;
                                        case "DescriptionT":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionT"),
                                        	    value: issueSubset.DescriptionT);
                                        	break;
                                        case "DescriptionU":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionU"),
                                        	    value: issueSubset.DescriptionU);
                                        	break;
                                        case "DescriptionV":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionV"),
                                        	    value: issueSubset.DescriptionV);
                                        	break;
                                        case "DescriptionW":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionW"),
                                        	    value: issueSubset.DescriptionW);
                                        	break;
                                        case "DescriptionX":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionX"),
                                        	    value: issueSubset.DescriptionX);
                                        	break;
                                        case "DescriptionY":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionY"),
                                        	    value: issueSubset.DescriptionY);
                                        	break;
                                        case "DescriptionZ":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionZ"),
                                        	    value: issueSubset.DescriptionZ);
                                        	break;
                                        case "CheckA":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckA"),
                                        	    value: issueSubset.CheckA);
                                        	break;
                                        case "CheckB":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckB"),
                                        	    value: issueSubset.CheckB);
                                        	break;
                                        case "CheckC":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckC"),
                                        	    value: issueSubset.CheckC);
                                        	break;
                                        case "CheckD":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckD"),
                                        	    value: issueSubset.CheckD);
                                        	break;
                                        case "CheckE":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckE"),
                                        	    value: issueSubset.CheckE);
                                        	break;
                                        case "CheckF":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckF"),
                                        	    value: issueSubset.CheckF);
                                        	break;
                                        case "CheckG":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckG"),
                                        	    value: issueSubset.CheckG);
                                        	break;
                                        case "CheckH":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckH"),
                                        	    value: issueSubset.CheckH);
                                        	break;
                                        case "CheckI":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckI"),
                                        	    value: issueSubset.CheckI);
                                        	break;
                                        case "CheckJ":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckJ"),
                                        	    value: issueSubset.CheckJ);
                                        	break;
                                        case "CheckK":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckK"),
                                        	    value: issueSubset.CheckK);
                                        	break;
                                        case "CheckL":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckL"),
                                        	    value: issueSubset.CheckL);
                                        	break;
                                        case "CheckM":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckM"),
                                        	    value: issueSubset.CheckM);
                                        	break;
                                        case "CheckN":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckN"),
                                        	    value: issueSubset.CheckN);
                                        	break;
                                        case "CheckO":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckO"),
                                        	    value: issueSubset.CheckO);
                                        	break;
                                        case "CheckP":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckP"),
                                        	    value: issueSubset.CheckP);
                                        	break;
                                        case "CheckQ":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckQ"),
                                        	    value: issueSubset.CheckQ);
                                        	break;
                                        case "CheckR":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckR"),
                                        	    value: issueSubset.CheckR);
                                        	break;
                                        case "CheckS":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckS"),
                                        	    value: issueSubset.CheckS);
                                        	break;
                                        case "CheckT":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckT"),
                                        	    value: issueSubset.CheckT);
                                        	break;
                                        case "CheckU":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckU"),
                                        	    value: issueSubset.CheckU);
                                        	break;
                                        case "CheckV":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckV"),
                                        	    value: issueSubset.CheckV);
                                        	break;
                                        case "CheckW":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckW"),
                                        	    value: issueSubset.CheckW);
                                        	break;
                                        case "CheckX":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckX"),
                                        	    value: issueSubset.CheckX);
                                        	break;
                                        case "CheckY":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckY"),
                                        	    value: issueSubset.CheckY);
                                        	break;
                                        case "CheckZ":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckZ"),
                                        	    value: issueSubset.CheckZ);
                                        	break;
                                        case "Comments":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("Comments"),
                                        	    value: issueSubset.Comments);
                                        	break;
                                        case "Creator":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("Creator"),
                                        	    value: issueSubset.Creator);
                                        	break;
                                        case "Updator":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("Updator"),
                                        	    value: issueSubset.Updator);
                                        	break;
                                        case "CreatedTime":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CreatedTime"),
                                        	    value: issueSubset.CreatedTime);
                                        	break;
                                        case "VerUp":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("VerUp"),
                                        	    value: issueSubset.VerUp);
                                        	break;
                                        case "Timestamp":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("Timestamp"),
                                        	    value: issueSubset.Timestamp);
                                        	break;
                                    }
                                });
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
                        attributes: new HtmlAttributes()
                            .Class("grid-row")
                            .DataId(id),
                        action: () =>
                        {
                            siteSettings
                                .LinkColumnCollection()
                                .Where(o => o.LinkVisible.ToBool())
                                .ForEach(column =>
                                {
                                    switch (column.ColumnName)
                                    {
                                        case "SiteId":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("SiteId"),
                                        	    value: resultSubset.SiteId);
                                        	break;
                                        case "UpdatedTime":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("UpdatedTime"),
                                        	    value: resultSubset.UpdatedTime);
                                        	break;
                                        case "ResultId":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ResultId"),
                                        	    value: resultSubset.ResultId);
                                        	break;
                                        case "Ver":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("Ver"),
                                        	    value: resultSubset.Ver);
                                        	break;
                                        case "Title":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("Title"),
                                        	    value: resultSubset.Title);
                                        	break;
                                        case "Body":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("Body"),
                                        	    value: resultSubset.Body);
                                        	break;
                                        case "TitleBody":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("TitleBody"),
                                        	    value: resultSubset.TitleBody);
                                        	break;
                                        case "Status":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("Status"),
                                        	    value: resultSubset.Status);
                                        	break;
                                        case "Manager":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("Manager"),
                                        	    value: resultSubset.Manager);
                                        	break;
                                        case "Owner":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("Owner"),
                                        	    value: resultSubset.Owner);
                                        	break;
                                        case "ClassA":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassA"),
                                        	    value: resultSubset.ClassA);
                                        	break;
                                        case "ClassB":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassB"),
                                        	    value: resultSubset.ClassB);
                                        	break;
                                        case "ClassC":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassC"),
                                        	    value: resultSubset.ClassC);
                                        	break;
                                        case "ClassD":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassD"),
                                        	    value: resultSubset.ClassD);
                                        	break;
                                        case "ClassE":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassE"),
                                        	    value: resultSubset.ClassE);
                                        	break;
                                        case "ClassF":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassF"),
                                        	    value: resultSubset.ClassF);
                                        	break;
                                        case "ClassG":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassG"),
                                        	    value: resultSubset.ClassG);
                                        	break;
                                        case "ClassH":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassH"),
                                        	    value: resultSubset.ClassH);
                                        	break;
                                        case "ClassI":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassI"),
                                        	    value: resultSubset.ClassI);
                                        	break;
                                        case "ClassJ":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassJ"),
                                        	    value: resultSubset.ClassJ);
                                        	break;
                                        case "ClassK":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassK"),
                                        	    value: resultSubset.ClassK);
                                        	break;
                                        case "ClassL":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassL"),
                                        	    value: resultSubset.ClassL);
                                        	break;
                                        case "ClassM":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassM"),
                                        	    value: resultSubset.ClassM);
                                        	break;
                                        case "ClassN":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassN"),
                                        	    value: resultSubset.ClassN);
                                        	break;
                                        case "ClassO":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassO"),
                                        	    value: resultSubset.ClassO);
                                        	break;
                                        case "ClassP":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassP"),
                                        	    value: resultSubset.ClassP);
                                        	break;
                                        case "ClassQ":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassQ"),
                                        	    value: resultSubset.ClassQ);
                                        	break;
                                        case "ClassR":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassR"),
                                        	    value: resultSubset.ClassR);
                                        	break;
                                        case "ClassS":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassS"),
                                        	    value: resultSubset.ClassS);
                                        	break;
                                        case "ClassT":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassT"),
                                        	    value: resultSubset.ClassT);
                                        	break;
                                        case "ClassU":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassU"),
                                        	    value: resultSubset.ClassU);
                                        	break;
                                        case "ClassV":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassV"),
                                        	    value: resultSubset.ClassV);
                                        	break;
                                        case "ClassW":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassW"),
                                        	    value: resultSubset.ClassW);
                                        	break;
                                        case "ClassX":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassX"),
                                        	    value: resultSubset.ClassX);
                                        	break;
                                        case "ClassY":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassY"),
                                        	    value: resultSubset.ClassY);
                                        	break;
                                        case "ClassZ":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("ClassZ"),
                                        	    value: resultSubset.ClassZ);
                                        	break;
                                        case "NumA":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumA"),
                                        	    value: resultSubset.NumA);
                                        	break;
                                        case "NumB":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumB"),
                                        	    value: resultSubset.NumB);
                                        	break;
                                        case "NumC":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumC"),
                                        	    value: resultSubset.NumC);
                                        	break;
                                        case "NumD":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumD"),
                                        	    value: resultSubset.NumD);
                                        	break;
                                        case "NumE":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumE"),
                                        	    value: resultSubset.NumE);
                                        	break;
                                        case "NumF":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumF"),
                                        	    value: resultSubset.NumF);
                                        	break;
                                        case "NumG":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumG"),
                                        	    value: resultSubset.NumG);
                                        	break;
                                        case "NumH":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumH"),
                                        	    value: resultSubset.NumH);
                                        	break;
                                        case "NumI":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumI"),
                                        	    value: resultSubset.NumI);
                                        	break;
                                        case "NumJ":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumJ"),
                                        	    value: resultSubset.NumJ);
                                        	break;
                                        case "NumK":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumK"),
                                        	    value: resultSubset.NumK);
                                        	break;
                                        case "NumL":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumL"),
                                        	    value: resultSubset.NumL);
                                        	break;
                                        case "NumM":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumM"),
                                        	    value: resultSubset.NumM);
                                        	break;
                                        case "NumN":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumN"),
                                        	    value: resultSubset.NumN);
                                        	break;
                                        case "NumO":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumO"),
                                        	    value: resultSubset.NumO);
                                        	break;
                                        case "NumP":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumP"),
                                        	    value: resultSubset.NumP);
                                        	break;
                                        case "NumQ":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumQ"),
                                        	    value: resultSubset.NumQ);
                                        	break;
                                        case "NumR":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumR"),
                                        	    value: resultSubset.NumR);
                                        	break;
                                        case "NumS":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumS"),
                                        	    value: resultSubset.NumS);
                                        	break;
                                        case "NumT":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumT"),
                                        	    value: resultSubset.NumT);
                                        	break;
                                        case "NumU":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumU"),
                                        	    value: resultSubset.NumU);
                                        	break;
                                        case "NumV":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumV"),
                                        	    value: resultSubset.NumV);
                                        	break;
                                        case "NumW":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumW"),
                                        	    value: resultSubset.NumW);
                                        	break;
                                        case "NumX":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumX"),
                                        	    value: resultSubset.NumX);
                                        	break;
                                        case "NumY":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumY"),
                                        	    value: resultSubset.NumY);
                                        	break;
                                        case "NumZ":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("NumZ"),
                                        	    value: resultSubset.NumZ);
                                        	break;
                                        case "DateA":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateA"),
                                        	    value: resultSubset.DateA);
                                        	break;
                                        case "DateB":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateB"),
                                        	    value: resultSubset.DateB);
                                        	break;
                                        case "DateC":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateC"),
                                        	    value: resultSubset.DateC);
                                        	break;
                                        case "DateD":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateD"),
                                        	    value: resultSubset.DateD);
                                        	break;
                                        case "DateE":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateE"),
                                        	    value: resultSubset.DateE);
                                        	break;
                                        case "DateF":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateF"),
                                        	    value: resultSubset.DateF);
                                        	break;
                                        case "DateG":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateG"),
                                        	    value: resultSubset.DateG);
                                        	break;
                                        case "DateH":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateH"),
                                        	    value: resultSubset.DateH);
                                        	break;
                                        case "DateI":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateI"),
                                        	    value: resultSubset.DateI);
                                        	break;
                                        case "DateJ":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateJ"),
                                        	    value: resultSubset.DateJ);
                                        	break;
                                        case "DateK":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateK"),
                                        	    value: resultSubset.DateK);
                                        	break;
                                        case "DateL":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateL"),
                                        	    value: resultSubset.DateL);
                                        	break;
                                        case "DateM":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateM"),
                                        	    value: resultSubset.DateM);
                                        	break;
                                        case "DateN":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateN"),
                                        	    value: resultSubset.DateN);
                                        	break;
                                        case "DateO":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateO"),
                                        	    value: resultSubset.DateO);
                                        	break;
                                        case "DateP":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateP"),
                                        	    value: resultSubset.DateP);
                                        	break;
                                        case "DateQ":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateQ"),
                                        	    value: resultSubset.DateQ);
                                        	break;
                                        case "DateR":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateR"),
                                        	    value: resultSubset.DateR);
                                        	break;
                                        case "DateS":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateS"),
                                        	    value: resultSubset.DateS);
                                        	break;
                                        case "DateT":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateT"),
                                        	    value: resultSubset.DateT);
                                        	break;
                                        case "DateU":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateU"),
                                        	    value: resultSubset.DateU);
                                        	break;
                                        case "DateV":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateV"),
                                        	    value: resultSubset.DateV);
                                        	break;
                                        case "DateW":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateW"),
                                        	    value: resultSubset.DateW);
                                        	break;
                                        case "DateX":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateX"),
                                        	    value: resultSubset.DateX);
                                        	break;
                                        case "DateY":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateY"),
                                        	    value: resultSubset.DateY);
                                        	break;
                                        case "DateZ":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DateZ"),
                                        	    value: resultSubset.DateZ);
                                        	break;
                                        case "DescriptionA":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionA"),
                                        	    value: resultSubset.DescriptionA);
                                        	break;
                                        case "DescriptionB":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionB"),
                                        	    value: resultSubset.DescriptionB);
                                        	break;
                                        case "DescriptionC":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionC"),
                                        	    value: resultSubset.DescriptionC);
                                        	break;
                                        case "DescriptionD":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionD"),
                                        	    value: resultSubset.DescriptionD);
                                        	break;
                                        case "DescriptionE":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionE"),
                                        	    value: resultSubset.DescriptionE);
                                        	break;
                                        case "DescriptionF":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionF"),
                                        	    value: resultSubset.DescriptionF);
                                        	break;
                                        case "DescriptionG":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionG"),
                                        	    value: resultSubset.DescriptionG);
                                        	break;
                                        case "DescriptionH":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionH"),
                                        	    value: resultSubset.DescriptionH);
                                        	break;
                                        case "DescriptionI":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionI"),
                                        	    value: resultSubset.DescriptionI);
                                        	break;
                                        case "DescriptionJ":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionJ"),
                                        	    value: resultSubset.DescriptionJ);
                                        	break;
                                        case "DescriptionK":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionK"),
                                        	    value: resultSubset.DescriptionK);
                                        	break;
                                        case "DescriptionL":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionL"),
                                        	    value: resultSubset.DescriptionL);
                                        	break;
                                        case "DescriptionM":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionM"),
                                        	    value: resultSubset.DescriptionM);
                                        	break;
                                        case "DescriptionN":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionN"),
                                        	    value: resultSubset.DescriptionN);
                                        	break;
                                        case "DescriptionO":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionO"),
                                        	    value: resultSubset.DescriptionO);
                                        	break;
                                        case "DescriptionP":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionP"),
                                        	    value: resultSubset.DescriptionP);
                                        	break;
                                        case "DescriptionQ":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionQ"),
                                        	    value: resultSubset.DescriptionQ);
                                        	break;
                                        case "DescriptionR":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionR"),
                                        	    value: resultSubset.DescriptionR);
                                        	break;
                                        case "DescriptionS":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionS"),
                                        	    value: resultSubset.DescriptionS);
                                        	break;
                                        case "DescriptionT":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionT"),
                                        	    value: resultSubset.DescriptionT);
                                        	break;
                                        case "DescriptionU":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionU"),
                                        	    value: resultSubset.DescriptionU);
                                        	break;
                                        case "DescriptionV":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionV"),
                                        	    value: resultSubset.DescriptionV);
                                        	break;
                                        case "DescriptionW":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionW"),
                                        	    value: resultSubset.DescriptionW);
                                        	break;
                                        case "DescriptionX":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionX"),
                                        	    value: resultSubset.DescriptionX);
                                        	break;
                                        case "DescriptionY":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionY"),
                                        	    value: resultSubset.DescriptionY);
                                        	break;
                                        case "DescriptionZ":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("DescriptionZ"),
                                        	    value: resultSubset.DescriptionZ);
                                        	break;
                                        case "CheckA":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckA"),
                                        	    value: resultSubset.CheckA);
                                        	break;
                                        case "CheckB":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckB"),
                                        	    value: resultSubset.CheckB);
                                        	break;
                                        case "CheckC":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckC"),
                                        	    value: resultSubset.CheckC);
                                        	break;
                                        case "CheckD":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckD"),
                                        	    value: resultSubset.CheckD);
                                        	break;
                                        case "CheckE":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckE"),
                                        	    value: resultSubset.CheckE);
                                        	break;
                                        case "CheckF":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckF"),
                                        	    value: resultSubset.CheckF);
                                        	break;
                                        case "CheckG":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckG"),
                                        	    value: resultSubset.CheckG);
                                        	break;
                                        case "CheckH":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckH"),
                                        	    value: resultSubset.CheckH);
                                        	break;
                                        case "CheckI":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckI"),
                                        	    value: resultSubset.CheckI);
                                        	break;
                                        case "CheckJ":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckJ"),
                                        	    value: resultSubset.CheckJ);
                                        	break;
                                        case "CheckK":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckK"),
                                        	    value: resultSubset.CheckK);
                                        	break;
                                        case "CheckL":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckL"),
                                        	    value: resultSubset.CheckL);
                                        	break;
                                        case "CheckM":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckM"),
                                        	    value: resultSubset.CheckM);
                                        	break;
                                        case "CheckN":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckN"),
                                        	    value: resultSubset.CheckN);
                                        	break;
                                        case "CheckO":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckO"),
                                        	    value: resultSubset.CheckO);
                                        	break;
                                        case "CheckP":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckP"),
                                        	    value: resultSubset.CheckP);
                                        	break;
                                        case "CheckQ":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckQ"),
                                        	    value: resultSubset.CheckQ);
                                        	break;
                                        case "CheckR":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckR"),
                                        	    value: resultSubset.CheckR);
                                        	break;
                                        case "CheckS":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckS"),
                                        	    value: resultSubset.CheckS);
                                        	break;
                                        case "CheckT":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckT"),
                                        	    value: resultSubset.CheckT);
                                        	break;
                                        case "CheckU":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckU"),
                                        	    value: resultSubset.CheckU);
                                        	break;
                                        case "CheckV":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckV"),
                                        	    value: resultSubset.CheckV);
                                        	break;
                                        case "CheckW":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckW"),
                                        	    value: resultSubset.CheckW);
                                        	break;
                                        case "CheckX":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckX"),
                                        	    value: resultSubset.CheckX);
                                        	break;
                                        case "CheckY":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckY"),
                                        	    value: resultSubset.CheckY);
                                        	break;
                                        case "CheckZ":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CheckZ"),
                                        	    value: resultSubset.CheckZ);
                                        	break;
                                        case "Comments":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("Comments"),
                                        	    value: resultSubset.Comments);
                                        	break;
                                        case "Creator":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("Creator"),
                                        	    value: resultSubset.Creator);
                                        	break;
                                        case "Updator":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("Updator"),
                                        	    value: resultSubset.Updator);
                                        	break;
                                        case "CreatedTime":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CreatedTime"),
                                        	    value: resultSubset.CreatedTime);
                                        	break;
                                        case "VerUp":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("VerUp"),
                                        	    value: resultSubset.VerUp);
                                        	break;
                                        case "Timestamp":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("Timestamp"),
                                        	    value: resultSubset.Timestamp);
                                        	break;
                                    }
                                });
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
                        attributes: new HtmlAttributes()
                            .Class("grid-row")
                            .DataId(id),
                        action: () =>
                        {
                            siteSettings
                                .LinkColumnCollection()
                                .Where(o => o.LinkVisible.ToBool())
                                .ForEach(column =>
                                {
                                    switch (column.ColumnName)
                                    {
                                        case "SiteId":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("SiteId"),
                                        	    value: wikiSubset.SiteId);
                                        	break;
                                        case "UpdatedTime":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("UpdatedTime"),
                                        	    value: wikiSubset.UpdatedTime);
                                        	break;
                                        case "WikiId":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("WikiId"),
                                        	    value: wikiSubset.WikiId);
                                        	break;
                                        case "Ver":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("Ver"),
                                        	    value: wikiSubset.Ver);
                                        	break;
                                        case "Title":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("Title"),
                                        	    value: wikiSubset.Title);
                                        	break;
                                        case "Body":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("Body"),
                                        	    value: wikiSubset.Body);
                                        	break;
                                        case "TitleBody":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("TitleBody"),
                                        	    value: wikiSubset.TitleBody);
                                        	break;
                                        case "Comments":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("Comments"),
                                        	    value: wikiSubset.Comments);
                                        	break;
                                        case "Creator":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("Creator"),
                                        	    value: wikiSubset.Creator);
                                        	break;
                                        case "Updator":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("Updator"),
                                        	    value: wikiSubset.Updator);
                                        	break;
                                        case "CreatedTime":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("CreatedTime"),
                                        	    value: wikiSubset.CreatedTime);
                                        	break;
                                        case "VerUp":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("VerUp"),
                                        	    value: wikiSubset.VerUp);
                                        	break;
                                        case "Timestamp":
                                        	hb.Td(
                                        	    column: siteSettings.LinkColumn("Timestamp"),
                                        	    value: wikiSubset.Timestamp);
                                        	break;
                                    }
                                });
                        });
                }
            });
            return hb;
        }
    }
}
