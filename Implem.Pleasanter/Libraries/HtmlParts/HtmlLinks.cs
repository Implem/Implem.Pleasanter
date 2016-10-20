using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Converts;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlLinks
    {
        public static HtmlBuilder Links(this HtmlBuilder hb, long linkId)
        {
            var targets = Targets(linkId);
            var siteCollection = new SiteCollection(where: Rds.SitesWhere()
                .SiteId_In(targets.Select(o => o["SiteId"].ToLong()).Distinct()));
            var dataSet = DataSet(targets.Select(o => o["Id"].ToLong()));
            return Contains(dataSet)
                ? hb.FieldSet(
                    css: " enclosed",
                    legendText: Displays.Links(),
                    action: () => hb
                        .Links(
                            targets: targets,
                            siteCollection: siteCollection,
                            dataSet: dataSet))
                : hb;
        }

        private static EnumerableRowCollection<DataRow> Targets(long linkId)
        {
            return Rds.ExecuteTable(statements: new SqlStatement[]
            {
                Rds.SelectLinks(
                    column: Rds.LinksColumn()
                        .SourceId(_as: "Id")
                        .SiteId()
                        .Add("0 as [Destination]"),
                    join: Rds.LinksJoinDefault(),
                    where: Rds.LinksWhere().DestinationId(linkId),
                    unionType: Sqls.UnionTypes.UnionAll),
                Rds.SelectLinks(
                    column: Rds.LinksColumn()
                        .DestinationId(_as: "Id")
                        .SiteId()
                        .Add("1 as [Destination]"),
                    join: LinkUtilities.JoinByDestination(),
                    where: Rds.LinksWhere().SourceId(linkId))
            }).AsEnumerable();
        }

        private static DataSet DataSet(IEnumerable<long> targets)
        {
            return Rds.ExecuteDataSet(statements: new SqlStatement[]
            {
                SelectIssues(targets),
                SelectResults(targets)
            });
        }

        private static SqlStatement SelectIssues(IEnumerable<long> targets)
        {
            return Rds.SelectIssues(
                dataTableName: "Issues",
                column: Rds.IssuesDefaultColumns(),
                join: Rds.IssuesJoinDefault(),
                where: Rds.IssuesWhere().IssueId_In(targets));
        }

        private static SqlStatement SelectResults(IEnumerable<long> targets)
        {
            return Rds.SelectResults(
                dataTableName: "Results",
                column: Rds.ResultsDefaultColumns(),
                join: Rds.ResultsJoinDefault(),
                where: Rds.ResultsWhere().ResultId_In(targets));
        }

        private static bool Contains(DataSet dataSet)
        {
            return
                dataSet.Tables["Issues"].Rows.Count > 0 ||
                dataSet.Tables["Results"].Rows.Count > 0;
        }

        private static HtmlBuilder Links(
            this HtmlBuilder hb,
            EnumerableRowCollection<DataRow> targets,
            IEnumerable<SiteModel> siteCollection,
            DataSet dataSet)
        {
            return hb.Div(action: () => hb
                .LinkTables(
                    siteCollection: siteCollection.Where(o =>
                        targets.Any(p =>
                            p["Destination"].ToBool() &&
                            p["SiteId"].ToLong() == o.SiteId)),
                    dataSet: dataSet,
                    caption: Displays.LinkDestinations())
                .LinkTables(
                    siteCollection: siteCollection.Where(o =>
                        targets.Any(p =>
                            !p["Destination"].ToBool() &&
                            p["SiteId"].ToLong() == o.SiteId)),
                    dataSet: dataSet,
                    caption: Displays.LinkSources()));
        }

        private static HtmlBuilder LinkTables(
            this HtmlBuilder hb,
            IEnumerable<SiteModel> siteCollection,
            DataSet dataSet,
            string caption)
        {
            siteCollection.ForEach(siteModel => hb.Table(css: "grid", action: () =>
            {
                var dataRows = dataSet.Tables[siteModel.ReferenceType]
                    .AsEnumerable()
                    .Where(o => o["SiteId"].ToLong() == siteModel.SiteId);
                if (dataRows.Any())
                {
                    switch (siteModel.ReferenceType)
                    {
                        case "Issues":
                            var issuesSiteSettings = siteModel.IssuesSiteSettings();
                            hb
                                .Caption(caption: "{0} : {1} - {2} {3}".Params(
                                    caption,
                                    SiteInfo.SiteMenu.Breadcrumb(siteModel.SiteId)
                                        .Select(o => o.Title)
                                        .Join(" > "),
                                    Displays.Quantity(),
                                    dataRows.Count()))
                                .THead(action: () => hb
                                    .IssuesHeader(siteSettings: issuesSiteSettings))
                                .TBody(action: () => hb
                                    .Issues(
                                        siteSettings: issuesSiteSettings,
                                        permissionType: siteModel.PermissionType,
                                        dataRows: dataRows));
                            break;
                        case "Results":
                            var resultsSiteSettings = siteModel.ResultsSiteSettings();
                            hb
                                .Caption(caption: "{0} : {1} - {2} {3}".Params(
                                    caption,
                                    SiteInfo.SiteMenu.Breadcrumb(siteModel.SiteId)
                                        .Select(o => o.Title)
                                        .Join(" > "),
                                    Displays.Quantity(),
                                    dataRows.Count()))
                                .THead(action: () => hb
                                    .ResultsHeader(siteSettings: resultsSiteSettings))
                                .TBody(action: () => hb
                                    .Results(
                                        siteSettings: resultsSiteSettings,
                                        permissionType: siteModel.PermissionType,
                                        dataRows: dataRows));
                            break;
                        case "Wikis":
                            var wikisSiteSettings = siteModel.WikisSiteSettings();
                            hb
                                .Caption(caption: "{0} : {1} - {2} {3}".Params(
                                    caption,
                                    SiteInfo.SiteMenu.Breadcrumb(siteModel.SiteId)
                                        .Select(o => o.Title)
                                        .Join(" > "),
                                    Displays.Quantity(),
                                    dataRows.Count()))
                                .THead(action: () => hb
                                    .WikisHeader(siteSettings: wikisSiteSettings))
                                .TBody(action: () => hb
                                    .Wikis(
                                        siteSettings: wikisSiteSettings,
                                        permissionType: siteModel.PermissionType,
                                        dataRows: dataRows));
                            break;
                    }
                }
            }));
            return hb;
        }

        private static HtmlBuilder IssuesHeader(this HtmlBuilder hb, SiteSettings siteSettings)
        {
            return hb.Tr(css: "ui-widget-header", action: () =>
            {
                siteSettings
                    .LinkColumnCollection()
                    .ForEach(column => hb
                        .Th(action: () => hb
                            .Text(text: column.LabelText)));
            });
        }

        private static HtmlBuilder ResultsHeader(this HtmlBuilder hb, SiteSettings siteSettings)
        {
            return hb.Tr(css: "ui-widget-header", action: () =>
            {
                siteSettings
                    .LinkColumnCollection()
                    .ForEach(column => hb
                        .Th(action: () => hb
                            .Text(text: column.LabelText)));
            });
        }

        private static HtmlBuilder WikisHeader(this HtmlBuilder hb, SiteSettings siteSettings)
        {
            return hb.Tr(css: "ui-widget-header", action: () =>
            {
                siteSettings
                    .LinkColumnCollection()
                    .ForEach(column => hb
                        .Th(action: () => hb
                            .Text(text: column.LabelText)));
            });
        }

        private static HtmlBuilder Issues(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            EnumerableRowCollection<DataRow> dataRows)
        {
            dataRows.ForEach(dataRow =>
            {
                var issueModel = new IssueModel(siteSettings, permissionType, dataRow);
                hb.Tr(
                    attributes: new HtmlAttributes()
                        .Class("grid-row")
                        .DataId(issueModel.IssueId.ToString()),
                    action: () =>
                    {
                        siteSettings
                            .LinkColumnCollection()
                            .ForEach(column =>
                            {
                                switch (column.ColumnName)
                                {
                                    case "SiteId":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("SiteId"),
                                    	    value: issueModel.SiteId);
                                    	break;
                                    case "UpdatedTime":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("UpdatedTime"),
                                    	    value: issueModel.UpdatedTime);
                                    	break;
                                    case "IssueId":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("IssueId"),
                                    	    value: issueModel.IssueId);
                                    	break;
                                    case "Ver":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("Ver"),
                                    	    value: issueModel.Ver);
                                    	break;
                                    case "Title":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("Title"),
                                    	    value: issueModel.Title);
                                    	break;
                                    case "Body":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("Body"),
                                    	    value: issueModel.Body);
                                    	break;
                                    case "TitleBody":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("TitleBody"),
                                    	    value: issueModel.TitleBody);
                                    	break;
                                    case "StartTime":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("StartTime"),
                                    	    value: issueModel.StartTime);
                                    	break;
                                    case "CompletionTime":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CompletionTime"),
                                    	    value: issueModel.CompletionTime);
                                    	break;
                                    case "WorkValue":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("WorkValue"),
                                    	    value: issueModel.WorkValue);
                                    	break;
                                    case "ProgressRate":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ProgressRate"),
                                    	    value: issueModel.ProgressRate);
                                    	break;
                                    case "RemainingWorkValue":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("RemainingWorkValue"),
                                    	    value: issueModel.RemainingWorkValue);
                                    	break;
                                    case "Status":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("Status"),
                                    	    value: issueModel.Status);
                                    	break;
                                    case "Manager":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("Manager"),
                                    	    value: issueModel.Manager);
                                    	break;
                                    case "Owner":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("Owner"),
                                    	    value: issueModel.Owner);
                                    	break;
                                    case "ClassA":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassA"),
                                    	    value: issueModel.ClassA);
                                    	break;
                                    case "ClassB":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassB"),
                                    	    value: issueModel.ClassB);
                                    	break;
                                    case "ClassC":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassC"),
                                    	    value: issueModel.ClassC);
                                    	break;
                                    case "ClassD":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassD"),
                                    	    value: issueModel.ClassD);
                                    	break;
                                    case "ClassE":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassE"),
                                    	    value: issueModel.ClassE);
                                    	break;
                                    case "ClassF":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassF"),
                                    	    value: issueModel.ClassF);
                                    	break;
                                    case "ClassG":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassG"),
                                    	    value: issueModel.ClassG);
                                    	break;
                                    case "ClassH":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassH"),
                                    	    value: issueModel.ClassH);
                                    	break;
                                    case "ClassI":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassI"),
                                    	    value: issueModel.ClassI);
                                    	break;
                                    case "ClassJ":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassJ"),
                                    	    value: issueModel.ClassJ);
                                    	break;
                                    case "ClassK":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassK"),
                                    	    value: issueModel.ClassK);
                                    	break;
                                    case "ClassL":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassL"),
                                    	    value: issueModel.ClassL);
                                    	break;
                                    case "ClassM":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassM"),
                                    	    value: issueModel.ClassM);
                                    	break;
                                    case "ClassN":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassN"),
                                    	    value: issueModel.ClassN);
                                    	break;
                                    case "ClassO":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassO"),
                                    	    value: issueModel.ClassO);
                                    	break;
                                    case "ClassP":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassP"),
                                    	    value: issueModel.ClassP);
                                    	break;
                                    case "ClassQ":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassQ"),
                                    	    value: issueModel.ClassQ);
                                    	break;
                                    case "ClassR":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassR"),
                                    	    value: issueModel.ClassR);
                                    	break;
                                    case "ClassS":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassS"),
                                    	    value: issueModel.ClassS);
                                    	break;
                                    case "ClassT":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassT"),
                                    	    value: issueModel.ClassT);
                                    	break;
                                    case "ClassU":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassU"),
                                    	    value: issueModel.ClassU);
                                    	break;
                                    case "ClassV":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassV"),
                                    	    value: issueModel.ClassV);
                                    	break;
                                    case "ClassW":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassW"),
                                    	    value: issueModel.ClassW);
                                    	break;
                                    case "ClassX":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassX"),
                                    	    value: issueModel.ClassX);
                                    	break;
                                    case "ClassY":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassY"),
                                    	    value: issueModel.ClassY);
                                    	break;
                                    case "ClassZ":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassZ"),
                                    	    value: issueModel.ClassZ);
                                    	break;
                                    case "NumA":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumA"),
                                    	    value: issueModel.NumA);
                                    	break;
                                    case "NumB":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumB"),
                                    	    value: issueModel.NumB);
                                    	break;
                                    case "NumC":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumC"),
                                    	    value: issueModel.NumC);
                                    	break;
                                    case "NumD":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumD"),
                                    	    value: issueModel.NumD);
                                    	break;
                                    case "NumE":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumE"),
                                    	    value: issueModel.NumE);
                                    	break;
                                    case "NumF":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumF"),
                                    	    value: issueModel.NumF);
                                    	break;
                                    case "NumG":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumG"),
                                    	    value: issueModel.NumG);
                                    	break;
                                    case "NumH":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumH"),
                                    	    value: issueModel.NumH);
                                    	break;
                                    case "NumI":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumI"),
                                    	    value: issueModel.NumI);
                                    	break;
                                    case "NumJ":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumJ"),
                                    	    value: issueModel.NumJ);
                                    	break;
                                    case "NumK":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumK"),
                                    	    value: issueModel.NumK);
                                    	break;
                                    case "NumL":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumL"),
                                    	    value: issueModel.NumL);
                                    	break;
                                    case "NumM":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumM"),
                                    	    value: issueModel.NumM);
                                    	break;
                                    case "NumN":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumN"),
                                    	    value: issueModel.NumN);
                                    	break;
                                    case "NumO":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumO"),
                                    	    value: issueModel.NumO);
                                    	break;
                                    case "NumP":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumP"),
                                    	    value: issueModel.NumP);
                                    	break;
                                    case "NumQ":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumQ"),
                                    	    value: issueModel.NumQ);
                                    	break;
                                    case "NumR":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumR"),
                                    	    value: issueModel.NumR);
                                    	break;
                                    case "NumS":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumS"),
                                    	    value: issueModel.NumS);
                                    	break;
                                    case "NumT":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumT"),
                                    	    value: issueModel.NumT);
                                    	break;
                                    case "NumU":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumU"),
                                    	    value: issueModel.NumU);
                                    	break;
                                    case "NumV":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumV"),
                                    	    value: issueModel.NumV);
                                    	break;
                                    case "NumW":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumW"),
                                    	    value: issueModel.NumW);
                                    	break;
                                    case "NumX":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumX"),
                                    	    value: issueModel.NumX);
                                    	break;
                                    case "NumY":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumY"),
                                    	    value: issueModel.NumY);
                                    	break;
                                    case "NumZ":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumZ"),
                                    	    value: issueModel.NumZ);
                                    	break;
                                    case "DateA":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateA"),
                                    	    value: issueModel.DateA);
                                    	break;
                                    case "DateB":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateB"),
                                    	    value: issueModel.DateB);
                                    	break;
                                    case "DateC":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateC"),
                                    	    value: issueModel.DateC);
                                    	break;
                                    case "DateD":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateD"),
                                    	    value: issueModel.DateD);
                                    	break;
                                    case "DateE":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateE"),
                                    	    value: issueModel.DateE);
                                    	break;
                                    case "DateF":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateF"),
                                    	    value: issueModel.DateF);
                                    	break;
                                    case "DateG":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateG"),
                                    	    value: issueModel.DateG);
                                    	break;
                                    case "DateH":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateH"),
                                    	    value: issueModel.DateH);
                                    	break;
                                    case "DateI":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateI"),
                                    	    value: issueModel.DateI);
                                    	break;
                                    case "DateJ":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateJ"),
                                    	    value: issueModel.DateJ);
                                    	break;
                                    case "DateK":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateK"),
                                    	    value: issueModel.DateK);
                                    	break;
                                    case "DateL":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateL"),
                                    	    value: issueModel.DateL);
                                    	break;
                                    case "DateM":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateM"),
                                    	    value: issueModel.DateM);
                                    	break;
                                    case "DateN":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateN"),
                                    	    value: issueModel.DateN);
                                    	break;
                                    case "DateO":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateO"),
                                    	    value: issueModel.DateO);
                                    	break;
                                    case "DateP":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateP"),
                                    	    value: issueModel.DateP);
                                    	break;
                                    case "DateQ":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateQ"),
                                    	    value: issueModel.DateQ);
                                    	break;
                                    case "DateR":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateR"),
                                    	    value: issueModel.DateR);
                                    	break;
                                    case "DateS":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateS"),
                                    	    value: issueModel.DateS);
                                    	break;
                                    case "DateT":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateT"),
                                    	    value: issueModel.DateT);
                                    	break;
                                    case "DateU":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateU"),
                                    	    value: issueModel.DateU);
                                    	break;
                                    case "DateV":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateV"),
                                    	    value: issueModel.DateV);
                                    	break;
                                    case "DateW":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateW"),
                                    	    value: issueModel.DateW);
                                    	break;
                                    case "DateX":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateX"),
                                    	    value: issueModel.DateX);
                                    	break;
                                    case "DateY":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateY"),
                                    	    value: issueModel.DateY);
                                    	break;
                                    case "DateZ":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateZ"),
                                    	    value: issueModel.DateZ);
                                    	break;
                                    case "DescriptionA":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionA"),
                                    	    value: issueModel.DescriptionA);
                                    	break;
                                    case "DescriptionB":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionB"),
                                    	    value: issueModel.DescriptionB);
                                    	break;
                                    case "DescriptionC":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionC"),
                                    	    value: issueModel.DescriptionC);
                                    	break;
                                    case "DescriptionD":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionD"),
                                    	    value: issueModel.DescriptionD);
                                    	break;
                                    case "DescriptionE":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionE"),
                                    	    value: issueModel.DescriptionE);
                                    	break;
                                    case "DescriptionF":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionF"),
                                    	    value: issueModel.DescriptionF);
                                    	break;
                                    case "DescriptionG":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionG"),
                                    	    value: issueModel.DescriptionG);
                                    	break;
                                    case "DescriptionH":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionH"),
                                    	    value: issueModel.DescriptionH);
                                    	break;
                                    case "DescriptionI":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionI"),
                                    	    value: issueModel.DescriptionI);
                                    	break;
                                    case "DescriptionJ":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionJ"),
                                    	    value: issueModel.DescriptionJ);
                                    	break;
                                    case "DescriptionK":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionK"),
                                    	    value: issueModel.DescriptionK);
                                    	break;
                                    case "DescriptionL":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionL"),
                                    	    value: issueModel.DescriptionL);
                                    	break;
                                    case "DescriptionM":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionM"),
                                    	    value: issueModel.DescriptionM);
                                    	break;
                                    case "DescriptionN":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionN"),
                                    	    value: issueModel.DescriptionN);
                                    	break;
                                    case "DescriptionO":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionO"),
                                    	    value: issueModel.DescriptionO);
                                    	break;
                                    case "DescriptionP":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionP"),
                                    	    value: issueModel.DescriptionP);
                                    	break;
                                    case "DescriptionQ":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionQ"),
                                    	    value: issueModel.DescriptionQ);
                                    	break;
                                    case "DescriptionR":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionR"),
                                    	    value: issueModel.DescriptionR);
                                    	break;
                                    case "DescriptionS":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionS"),
                                    	    value: issueModel.DescriptionS);
                                    	break;
                                    case "DescriptionT":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionT"),
                                    	    value: issueModel.DescriptionT);
                                    	break;
                                    case "DescriptionU":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionU"),
                                    	    value: issueModel.DescriptionU);
                                    	break;
                                    case "DescriptionV":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionV"),
                                    	    value: issueModel.DescriptionV);
                                    	break;
                                    case "DescriptionW":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionW"),
                                    	    value: issueModel.DescriptionW);
                                    	break;
                                    case "DescriptionX":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionX"),
                                    	    value: issueModel.DescriptionX);
                                    	break;
                                    case "DescriptionY":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionY"),
                                    	    value: issueModel.DescriptionY);
                                    	break;
                                    case "DescriptionZ":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionZ"),
                                    	    value: issueModel.DescriptionZ);
                                    	break;
                                    case "CheckA":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckA"),
                                    	    value: issueModel.CheckA);
                                    	break;
                                    case "CheckB":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckB"),
                                    	    value: issueModel.CheckB);
                                    	break;
                                    case "CheckC":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckC"),
                                    	    value: issueModel.CheckC);
                                    	break;
                                    case "CheckD":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckD"),
                                    	    value: issueModel.CheckD);
                                    	break;
                                    case "CheckE":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckE"),
                                    	    value: issueModel.CheckE);
                                    	break;
                                    case "CheckF":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckF"),
                                    	    value: issueModel.CheckF);
                                    	break;
                                    case "CheckG":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckG"),
                                    	    value: issueModel.CheckG);
                                    	break;
                                    case "CheckH":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckH"),
                                    	    value: issueModel.CheckH);
                                    	break;
                                    case "CheckI":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckI"),
                                    	    value: issueModel.CheckI);
                                    	break;
                                    case "CheckJ":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckJ"),
                                    	    value: issueModel.CheckJ);
                                    	break;
                                    case "CheckK":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckK"),
                                    	    value: issueModel.CheckK);
                                    	break;
                                    case "CheckL":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckL"),
                                    	    value: issueModel.CheckL);
                                    	break;
                                    case "CheckM":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckM"),
                                    	    value: issueModel.CheckM);
                                    	break;
                                    case "CheckN":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckN"),
                                    	    value: issueModel.CheckN);
                                    	break;
                                    case "CheckO":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckO"),
                                    	    value: issueModel.CheckO);
                                    	break;
                                    case "CheckP":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckP"),
                                    	    value: issueModel.CheckP);
                                    	break;
                                    case "CheckQ":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckQ"),
                                    	    value: issueModel.CheckQ);
                                    	break;
                                    case "CheckR":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckR"),
                                    	    value: issueModel.CheckR);
                                    	break;
                                    case "CheckS":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckS"),
                                    	    value: issueModel.CheckS);
                                    	break;
                                    case "CheckT":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckT"),
                                    	    value: issueModel.CheckT);
                                    	break;
                                    case "CheckU":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckU"),
                                    	    value: issueModel.CheckU);
                                    	break;
                                    case "CheckV":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckV"),
                                    	    value: issueModel.CheckV);
                                    	break;
                                    case "CheckW":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckW"),
                                    	    value: issueModel.CheckW);
                                    	break;
                                    case "CheckX":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckX"),
                                    	    value: issueModel.CheckX);
                                    	break;
                                    case "CheckY":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckY"),
                                    	    value: issueModel.CheckY);
                                    	break;
                                    case "CheckZ":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckZ"),
                                    	    value: issueModel.CheckZ);
                                    	break;
                                    case "Comments":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("Comments"),
                                    	    value: issueModel.Comments);
                                    	break;
                                    case "Creator":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("Creator"),
                                    	    value: issueModel.Creator);
                                    	break;
                                    case "Updator":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("Updator"),
                                    	    value: issueModel.Updator);
                                    	break;
                                    case "CreatedTime":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CreatedTime"),
                                    	    value: issueModel.CreatedTime);
                                    	break;
                                    case "VerUp":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("VerUp"),
                                    	    value: issueModel.VerUp);
                                    	break;
                                    case "Timestamp":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("Timestamp"),
                                    	    value: issueModel.Timestamp);
                                    	break;
                                }
                            });
                    });
            });
            return hb;
        }

        private static HtmlBuilder Results(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            EnumerableRowCollection<DataRow> dataRows)
        {
            dataRows.ForEach(dataRow =>
            {
                var resultModel = new ResultModel(siteSettings, permissionType, dataRow);
                hb.Tr(
                    attributes: new HtmlAttributes()
                        .Class("grid-row")
                        .DataId(resultModel.ResultId.ToString()),
                    action: () =>
                    {
                        siteSettings
                            .LinkColumnCollection()
                            .ForEach(column =>
                            {
                                switch (column.ColumnName)
                                {
                                    case "SiteId":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("SiteId"),
                                    	    value: resultModel.SiteId);
                                    	break;
                                    case "UpdatedTime":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("UpdatedTime"),
                                    	    value: resultModel.UpdatedTime);
                                    	break;
                                    case "ResultId":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ResultId"),
                                    	    value: resultModel.ResultId);
                                    	break;
                                    case "Ver":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("Ver"),
                                    	    value: resultModel.Ver);
                                    	break;
                                    case "Title":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("Title"),
                                    	    value: resultModel.Title);
                                    	break;
                                    case "Body":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("Body"),
                                    	    value: resultModel.Body);
                                    	break;
                                    case "TitleBody":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("TitleBody"),
                                    	    value: resultModel.TitleBody);
                                    	break;
                                    case "Status":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("Status"),
                                    	    value: resultModel.Status);
                                    	break;
                                    case "Manager":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("Manager"),
                                    	    value: resultModel.Manager);
                                    	break;
                                    case "Owner":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("Owner"),
                                    	    value: resultModel.Owner);
                                    	break;
                                    case "ClassA":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassA"),
                                    	    value: resultModel.ClassA);
                                    	break;
                                    case "ClassB":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassB"),
                                    	    value: resultModel.ClassB);
                                    	break;
                                    case "ClassC":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassC"),
                                    	    value: resultModel.ClassC);
                                    	break;
                                    case "ClassD":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassD"),
                                    	    value: resultModel.ClassD);
                                    	break;
                                    case "ClassE":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassE"),
                                    	    value: resultModel.ClassE);
                                    	break;
                                    case "ClassF":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassF"),
                                    	    value: resultModel.ClassF);
                                    	break;
                                    case "ClassG":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassG"),
                                    	    value: resultModel.ClassG);
                                    	break;
                                    case "ClassH":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassH"),
                                    	    value: resultModel.ClassH);
                                    	break;
                                    case "ClassI":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassI"),
                                    	    value: resultModel.ClassI);
                                    	break;
                                    case "ClassJ":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassJ"),
                                    	    value: resultModel.ClassJ);
                                    	break;
                                    case "ClassK":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassK"),
                                    	    value: resultModel.ClassK);
                                    	break;
                                    case "ClassL":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassL"),
                                    	    value: resultModel.ClassL);
                                    	break;
                                    case "ClassM":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassM"),
                                    	    value: resultModel.ClassM);
                                    	break;
                                    case "ClassN":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassN"),
                                    	    value: resultModel.ClassN);
                                    	break;
                                    case "ClassO":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassO"),
                                    	    value: resultModel.ClassO);
                                    	break;
                                    case "ClassP":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassP"),
                                    	    value: resultModel.ClassP);
                                    	break;
                                    case "ClassQ":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassQ"),
                                    	    value: resultModel.ClassQ);
                                    	break;
                                    case "ClassR":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassR"),
                                    	    value: resultModel.ClassR);
                                    	break;
                                    case "ClassS":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassS"),
                                    	    value: resultModel.ClassS);
                                    	break;
                                    case "ClassT":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassT"),
                                    	    value: resultModel.ClassT);
                                    	break;
                                    case "ClassU":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassU"),
                                    	    value: resultModel.ClassU);
                                    	break;
                                    case "ClassV":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassV"),
                                    	    value: resultModel.ClassV);
                                    	break;
                                    case "ClassW":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassW"),
                                    	    value: resultModel.ClassW);
                                    	break;
                                    case "ClassX":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassX"),
                                    	    value: resultModel.ClassX);
                                    	break;
                                    case "ClassY":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassY"),
                                    	    value: resultModel.ClassY);
                                    	break;
                                    case "ClassZ":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("ClassZ"),
                                    	    value: resultModel.ClassZ);
                                    	break;
                                    case "NumA":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumA"),
                                    	    value: resultModel.NumA);
                                    	break;
                                    case "NumB":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumB"),
                                    	    value: resultModel.NumB);
                                    	break;
                                    case "NumC":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumC"),
                                    	    value: resultModel.NumC);
                                    	break;
                                    case "NumD":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumD"),
                                    	    value: resultModel.NumD);
                                    	break;
                                    case "NumE":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumE"),
                                    	    value: resultModel.NumE);
                                    	break;
                                    case "NumF":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumF"),
                                    	    value: resultModel.NumF);
                                    	break;
                                    case "NumG":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumG"),
                                    	    value: resultModel.NumG);
                                    	break;
                                    case "NumH":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumH"),
                                    	    value: resultModel.NumH);
                                    	break;
                                    case "NumI":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumI"),
                                    	    value: resultModel.NumI);
                                    	break;
                                    case "NumJ":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumJ"),
                                    	    value: resultModel.NumJ);
                                    	break;
                                    case "NumK":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumK"),
                                    	    value: resultModel.NumK);
                                    	break;
                                    case "NumL":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumL"),
                                    	    value: resultModel.NumL);
                                    	break;
                                    case "NumM":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumM"),
                                    	    value: resultModel.NumM);
                                    	break;
                                    case "NumN":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumN"),
                                    	    value: resultModel.NumN);
                                    	break;
                                    case "NumO":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumO"),
                                    	    value: resultModel.NumO);
                                    	break;
                                    case "NumP":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumP"),
                                    	    value: resultModel.NumP);
                                    	break;
                                    case "NumQ":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumQ"),
                                    	    value: resultModel.NumQ);
                                    	break;
                                    case "NumR":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumR"),
                                    	    value: resultModel.NumR);
                                    	break;
                                    case "NumS":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumS"),
                                    	    value: resultModel.NumS);
                                    	break;
                                    case "NumT":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumT"),
                                    	    value: resultModel.NumT);
                                    	break;
                                    case "NumU":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumU"),
                                    	    value: resultModel.NumU);
                                    	break;
                                    case "NumV":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumV"),
                                    	    value: resultModel.NumV);
                                    	break;
                                    case "NumW":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumW"),
                                    	    value: resultModel.NumW);
                                    	break;
                                    case "NumX":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumX"),
                                    	    value: resultModel.NumX);
                                    	break;
                                    case "NumY":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumY"),
                                    	    value: resultModel.NumY);
                                    	break;
                                    case "NumZ":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("NumZ"),
                                    	    value: resultModel.NumZ);
                                    	break;
                                    case "DateA":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateA"),
                                    	    value: resultModel.DateA);
                                    	break;
                                    case "DateB":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateB"),
                                    	    value: resultModel.DateB);
                                    	break;
                                    case "DateC":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateC"),
                                    	    value: resultModel.DateC);
                                    	break;
                                    case "DateD":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateD"),
                                    	    value: resultModel.DateD);
                                    	break;
                                    case "DateE":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateE"),
                                    	    value: resultModel.DateE);
                                    	break;
                                    case "DateF":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateF"),
                                    	    value: resultModel.DateF);
                                    	break;
                                    case "DateG":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateG"),
                                    	    value: resultModel.DateG);
                                    	break;
                                    case "DateH":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateH"),
                                    	    value: resultModel.DateH);
                                    	break;
                                    case "DateI":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateI"),
                                    	    value: resultModel.DateI);
                                    	break;
                                    case "DateJ":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateJ"),
                                    	    value: resultModel.DateJ);
                                    	break;
                                    case "DateK":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateK"),
                                    	    value: resultModel.DateK);
                                    	break;
                                    case "DateL":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateL"),
                                    	    value: resultModel.DateL);
                                    	break;
                                    case "DateM":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateM"),
                                    	    value: resultModel.DateM);
                                    	break;
                                    case "DateN":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateN"),
                                    	    value: resultModel.DateN);
                                    	break;
                                    case "DateO":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateO"),
                                    	    value: resultModel.DateO);
                                    	break;
                                    case "DateP":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateP"),
                                    	    value: resultModel.DateP);
                                    	break;
                                    case "DateQ":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateQ"),
                                    	    value: resultModel.DateQ);
                                    	break;
                                    case "DateR":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateR"),
                                    	    value: resultModel.DateR);
                                    	break;
                                    case "DateS":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateS"),
                                    	    value: resultModel.DateS);
                                    	break;
                                    case "DateT":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateT"),
                                    	    value: resultModel.DateT);
                                    	break;
                                    case "DateU":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateU"),
                                    	    value: resultModel.DateU);
                                    	break;
                                    case "DateV":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateV"),
                                    	    value: resultModel.DateV);
                                    	break;
                                    case "DateW":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateW"),
                                    	    value: resultModel.DateW);
                                    	break;
                                    case "DateX":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateX"),
                                    	    value: resultModel.DateX);
                                    	break;
                                    case "DateY":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateY"),
                                    	    value: resultModel.DateY);
                                    	break;
                                    case "DateZ":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DateZ"),
                                    	    value: resultModel.DateZ);
                                    	break;
                                    case "DescriptionA":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionA"),
                                    	    value: resultModel.DescriptionA);
                                    	break;
                                    case "DescriptionB":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionB"),
                                    	    value: resultModel.DescriptionB);
                                    	break;
                                    case "DescriptionC":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionC"),
                                    	    value: resultModel.DescriptionC);
                                    	break;
                                    case "DescriptionD":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionD"),
                                    	    value: resultModel.DescriptionD);
                                    	break;
                                    case "DescriptionE":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionE"),
                                    	    value: resultModel.DescriptionE);
                                    	break;
                                    case "DescriptionF":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionF"),
                                    	    value: resultModel.DescriptionF);
                                    	break;
                                    case "DescriptionG":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionG"),
                                    	    value: resultModel.DescriptionG);
                                    	break;
                                    case "DescriptionH":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionH"),
                                    	    value: resultModel.DescriptionH);
                                    	break;
                                    case "DescriptionI":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionI"),
                                    	    value: resultModel.DescriptionI);
                                    	break;
                                    case "DescriptionJ":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionJ"),
                                    	    value: resultModel.DescriptionJ);
                                    	break;
                                    case "DescriptionK":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionK"),
                                    	    value: resultModel.DescriptionK);
                                    	break;
                                    case "DescriptionL":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionL"),
                                    	    value: resultModel.DescriptionL);
                                    	break;
                                    case "DescriptionM":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionM"),
                                    	    value: resultModel.DescriptionM);
                                    	break;
                                    case "DescriptionN":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionN"),
                                    	    value: resultModel.DescriptionN);
                                    	break;
                                    case "DescriptionO":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionO"),
                                    	    value: resultModel.DescriptionO);
                                    	break;
                                    case "DescriptionP":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionP"),
                                    	    value: resultModel.DescriptionP);
                                    	break;
                                    case "DescriptionQ":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionQ"),
                                    	    value: resultModel.DescriptionQ);
                                    	break;
                                    case "DescriptionR":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionR"),
                                    	    value: resultModel.DescriptionR);
                                    	break;
                                    case "DescriptionS":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionS"),
                                    	    value: resultModel.DescriptionS);
                                    	break;
                                    case "DescriptionT":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionT"),
                                    	    value: resultModel.DescriptionT);
                                    	break;
                                    case "DescriptionU":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionU"),
                                    	    value: resultModel.DescriptionU);
                                    	break;
                                    case "DescriptionV":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionV"),
                                    	    value: resultModel.DescriptionV);
                                    	break;
                                    case "DescriptionW":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionW"),
                                    	    value: resultModel.DescriptionW);
                                    	break;
                                    case "DescriptionX":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionX"),
                                    	    value: resultModel.DescriptionX);
                                    	break;
                                    case "DescriptionY":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionY"),
                                    	    value: resultModel.DescriptionY);
                                    	break;
                                    case "DescriptionZ":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("DescriptionZ"),
                                    	    value: resultModel.DescriptionZ);
                                    	break;
                                    case "CheckA":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckA"),
                                    	    value: resultModel.CheckA);
                                    	break;
                                    case "CheckB":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckB"),
                                    	    value: resultModel.CheckB);
                                    	break;
                                    case "CheckC":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckC"),
                                    	    value: resultModel.CheckC);
                                    	break;
                                    case "CheckD":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckD"),
                                    	    value: resultModel.CheckD);
                                    	break;
                                    case "CheckE":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckE"),
                                    	    value: resultModel.CheckE);
                                    	break;
                                    case "CheckF":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckF"),
                                    	    value: resultModel.CheckF);
                                    	break;
                                    case "CheckG":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckG"),
                                    	    value: resultModel.CheckG);
                                    	break;
                                    case "CheckH":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckH"),
                                    	    value: resultModel.CheckH);
                                    	break;
                                    case "CheckI":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckI"),
                                    	    value: resultModel.CheckI);
                                    	break;
                                    case "CheckJ":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckJ"),
                                    	    value: resultModel.CheckJ);
                                    	break;
                                    case "CheckK":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckK"),
                                    	    value: resultModel.CheckK);
                                    	break;
                                    case "CheckL":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckL"),
                                    	    value: resultModel.CheckL);
                                    	break;
                                    case "CheckM":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckM"),
                                    	    value: resultModel.CheckM);
                                    	break;
                                    case "CheckN":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckN"),
                                    	    value: resultModel.CheckN);
                                    	break;
                                    case "CheckO":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckO"),
                                    	    value: resultModel.CheckO);
                                    	break;
                                    case "CheckP":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckP"),
                                    	    value: resultModel.CheckP);
                                    	break;
                                    case "CheckQ":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckQ"),
                                    	    value: resultModel.CheckQ);
                                    	break;
                                    case "CheckR":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckR"),
                                    	    value: resultModel.CheckR);
                                    	break;
                                    case "CheckS":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckS"),
                                    	    value: resultModel.CheckS);
                                    	break;
                                    case "CheckT":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckT"),
                                    	    value: resultModel.CheckT);
                                    	break;
                                    case "CheckU":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckU"),
                                    	    value: resultModel.CheckU);
                                    	break;
                                    case "CheckV":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckV"),
                                    	    value: resultModel.CheckV);
                                    	break;
                                    case "CheckW":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckW"),
                                    	    value: resultModel.CheckW);
                                    	break;
                                    case "CheckX":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckX"),
                                    	    value: resultModel.CheckX);
                                    	break;
                                    case "CheckY":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckY"),
                                    	    value: resultModel.CheckY);
                                    	break;
                                    case "CheckZ":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CheckZ"),
                                    	    value: resultModel.CheckZ);
                                    	break;
                                    case "Comments":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("Comments"),
                                    	    value: resultModel.Comments);
                                    	break;
                                    case "Creator":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("Creator"),
                                    	    value: resultModel.Creator);
                                    	break;
                                    case "Updator":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("Updator"),
                                    	    value: resultModel.Updator);
                                    	break;
                                    case "CreatedTime":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CreatedTime"),
                                    	    value: resultModel.CreatedTime);
                                    	break;
                                    case "VerUp":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("VerUp"),
                                    	    value: resultModel.VerUp);
                                    	break;
                                    case "Timestamp":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("Timestamp"),
                                    	    value: resultModel.Timestamp);
                                    	break;
                                }
                            });
                    });
            });
            return hb;
        }

        private static HtmlBuilder Wikis(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            EnumerableRowCollection<DataRow> dataRows)
        {
            dataRows.ForEach(dataRow =>
            {
                var wikiModel = new WikiModel(siteSettings, permissionType, dataRow);
                hb.Tr(
                    attributes: new HtmlAttributes()
                        .Class("grid-row")
                        .DataId(wikiModel.WikiId.ToString()),
                    action: () =>
                    {
                        siteSettings
                            .LinkColumnCollection()
                            .ForEach(column =>
                            {
                                switch (column.ColumnName)
                                {
                                    case "SiteId":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("SiteId"),
                                    	    value: wikiModel.SiteId);
                                    	break;
                                    case "UpdatedTime":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("UpdatedTime"),
                                    	    value: wikiModel.UpdatedTime);
                                    	break;
                                    case "WikiId":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("WikiId"),
                                    	    value: wikiModel.WikiId);
                                    	break;
                                    case "Ver":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("Ver"),
                                    	    value: wikiModel.Ver);
                                    	break;
                                    case "Title":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("Title"),
                                    	    value: wikiModel.Title);
                                    	break;
                                    case "Body":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("Body"),
                                    	    value: wikiModel.Body);
                                    	break;
                                    case "TitleBody":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("TitleBody"),
                                    	    value: wikiModel.TitleBody);
                                    	break;
                                    case "Comments":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("Comments"),
                                    	    value: wikiModel.Comments);
                                    	break;
                                    case "Creator":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("Creator"),
                                    	    value: wikiModel.Creator);
                                    	break;
                                    case "Updator":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("Updator"),
                                    	    value: wikiModel.Updator);
                                    	break;
                                    case "CreatedTime":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("CreatedTime"),
                                    	    value: wikiModel.CreatedTime);
                                    	break;
                                    case "VerUp":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("VerUp"),
                                    	    value: wikiModel.VerUp);
                                    	break;
                                    case "Timestamp":
                                    	hb.Td(
                                    	    column: siteSettings.LinkColumn("Timestamp"),
                                    	    value: wikiModel.Timestamp);
                                    	break;
                                }
                            });
                    });
            });
            return hb;
        }
    }
}
