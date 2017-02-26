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
        public static HtmlBuilder Links(this HtmlBuilder hb, SiteSettings ss, long id)
        {
            var targets = Targets(id);
            var dataSet = DataSet(targets);
            return Contains(dataSet)
                ? hb.FieldSet(
                    css: " enclosed",
                    legendText: Displays.Links(),
                    action: () => hb
                        .Links(
                            targets: targets,
                            ss: ss,
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
                        .Add("'Source' as [Direction]"),
                    where: Rds.LinksWhere().DestinationId(linkId)),
                Rds.SelectLinks(
                    unionType: Sqls.UnionTypes.UnionAll,
                    column: Rds.LinksColumn()
                        .DestinationId(_as: "Id")
                        .Add("'Destination' as [Direction]"),
                    where: Rds.LinksWhere().SourceId(linkId))
            }).AsEnumerable();
        }

        private static DataSet DataSet(EnumerableRowCollection<DataRow> targets)
        {
            return Rds.ExecuteDataSet(statements: new SqlStatement[]
            {
                SelectIssues(targets, "Destination"), SelectIssues(targets, "Source"),
                SelectResults(targets, "Destination"), SelectResults(targets, "Source")
            });
        }

        private static SqlStatement SelectIssues(
            EnumerableRowCollection<DataRow> targets, string direction)
        {
            return Rds.SelectIssues(
                dataTableName: "Issues" + "_" + direction,
                column: Rds
                    .IssuesDefaultColumns()
                    .Add("'" + direction + "' as Direction"),
                join: Rds.IssuesJoinDefault(),
                where: Rds.IssuesWhere().IssueId_In(targets
                    .Where(o => o["Direction"].ToString() == direction)
                    .Select(o => o["Id"].ToLong())));
        }

        private static SqlStatement SelectResults(
            EnumerableRowCollection<DataRow> targets, string direction)
        {
            return Rds.SelectResults(
                dataTableName: "Results" + "_" + direction,
                column: Rds
                    .ResultsDefaultColumns()
                    .Add("'" + direction + "' as Direction"),
                join: Rds.ResultsJoinDefault(),
                where: Rds.ResultsWhere().ResultId_In(targets
                    .Where(o => o["Direction"].ToString() == direction)
                    .Select(o => o["Id"].ToLong())));
        }

        private static bool Contains(DataSet dataSet)
        {
            for (var i = 0; i < dataSet.Tables.Count; i++)
            {
                if (dataSet.Tables[i].Rows.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }

        private static HtmlBuilder Links(
            this HtmlBuilder hb,
            EnumerableRowCollection<DataRow> targets,
            SiteSettings ss,
            DataSet dataSet)
        {
            return hb.Div(action: () => hb
                .LinkTables(
                    ssList: ss.Destinations,
                    dataSet: dataSet,
                    direction: "Destination",
                    caption: Displays.LinkDestinations())
                .LinkTables(
                    ssList: ss.Sources,
                    dataSet: dataSet,
                    direction: "Source",
                    caption: Displays.LinkSources()));
        }

        private static HtmlBuilder LinkTables(
            this HtmlBuilder hb,
            IEnumerable<SiteSettings> ssList,
            DataSet dataSet,
            string direction,
            string caption)
        {
            ssList.ForEach(ss => hb.Table(css: "grid", action: () =>
            {
                var dataRows = dataSet.Tables[ss.ReferenceType + "_" + direction]?
                    .AsEnumerable()
                    .Where(o => o["SiteId"].ToLong() == ss.SiteId);
                if (dataRows != null && dataRows.Any())
                {
                    switch (ss.ReferenceType)
                    {
                        case "Issues":
                            hb
                                .Caption(caption: "{0} : {1} - {2} {3}".Params(
                                    caption,
                                    SiteInfo.SiteMenu.Breadcrumb(ss.SiteId)
                                        .Select(o => o.Title)
                                        .Join(" > "),
                                    Displays.Quantity(),
                                    dataRows.Count()))
                                .THead(action: () => hb
                                    .IssuesHeader(ss: ss))
                                .TBody(action: () => hb
                                    .Issues(ss: ss, dataRows: dataRows));
                            break;
                        case "Results":
                            hb
                                .Caption(caption: "{0} : {1} - {2} {3}".Params(
                                    caption,
                                    SiteInfo.SiteMenu.Breadcrumb(ss.SiteId)
                                        .Select(o => o.Title)
                                        .Join(" > "),
                                    Displays.Quantity(),
                                    dataRows.Count()))
                                .THead(action: () => hb
                                    .ResultsHeader(ss: ss))
                                .TBody(action: () => hb
                                    .Results(ss: ss, dataRows: dataRows));
                            break;
                    }
                }
            }));
            return hb;
        }

        private static HtmlBuilder IssuesHeader(this HtmlBuilder hb, SiteSettings ss)
        {
            return hb.Tr(css: "ui-widget-header", action: () => ss
                .GetLinkColumns()
                .ForEach(column => hb
                    .Th(action: () => hb
                        .Text(text: column.GridLabelText))));
        }

        private static HtmlBuilder ResultsHeader(this HtmlBuilder hb, SiteSettings ss)
        {
            return hb.Tr(css: "ui-widget-header", action: () => ss
                .GetLinkColumns()
                .ForEach(column => hb
                    .Th(action: () => hb
                        .Text(text: column.GridLabelText))));
        }

        private static HtmlBuilder Issues(
            this HtmlBuilder hb,
            SiteSettings ss,
            EnumerableRowCollection<DataRow> dataRows)
        {
            dataRows.ForEach(dataRow =>
            {
                var issueModel = new IssueModel(ss, dataRow);
                hb.Tr(
                    attributes: new HtmlAttributes()
                        .Class("grid-row")
                        .DataId(issueModel.IssueId.ToString()),
                    action: () => ss
                        .GetLinkColumns()
                        .ForEach(column =>
                        {
                            switch (column.ColumnName)
                            {
                                case "SiteId":
                                	hb.Td(
                                	    column: ss.LinkColumn("SiteId"),
                                	    value: issueModel.SiteId);
                                	break;
                                case "UpdatedTime":
                                	hb.Td(
                                	    column: ss.LinkColumn("UpdatedTime"),
                                	    value: issueModel.UpdatedTime);
                                	break;
                                case "IssueId":
                                	hb.Td(
                                	    column: ss.LinkColumn("IssueId"),
                                	    value: issueModel.IssueId);
                                	break;
                                case "Ver":
                                	hb.Td(
                                	    column: ss.LinkColumn("Ver"),
                                	    value: issueModel.Ver);
                                	break;
                                case "Title":
                                	hb.Td(
                                	    column: ss.LinkColumn("Title"),
                                	    value: issueModel.Title);
                                	break;
                                case "Body":
                                	hb.Td(
                                	    column: ss.LinkColumn("Body"),
                                	    value: issueModel.Body);
                                	break;
                                case "TitleBody":
                                	hb.Td(
                                	    column: ss.LinkColumn("TitleBody"),
                                	    value: issueModel.TitleBody);
                                	break;
                                case "StartTime":
                                	hb.Td(
                                	    column: ss.LinkColumn("StartTime"),
                                	    value: issueModel.StartTime);
                                	break;
                                case "CompletionTime":
                                	hb.Td(
                                	    column: ss.LinkColumn("CompletionTime"),
                                	    value: issueModel.CompletionTime);
                                	break;
                                case "WorkValue":
                                	hb.Td(
                                	    column: ss.LinkColumn("WorkValue"),
                                	    value: issueModel.WorkValue);
                                	break;
                                case "ProgressRate":
                                	hb.Td(
                                	    column: ss.LinkColumn("ProgressRate"),
                                	    value: issueModel.ProgressRate);
                                	break;
                                case "RemainingWorkValue":
                                	hb.Td(
                                	    column: ss.LinkColumn("RemainingWorkValue"),
                                	    value: issueModel.RemainingWorkValue);
                                	break;
                                case "Status":
                                	hb.Td(
                                	    column: ss.LinkColumn("Status"),
                                	    value: issueModel.Status);
                                	break;
                                case "Manager":
                                	hb.Td(
                                	    column: ss.LinkColumn("Manager"),
                                	    value: issueModel.Manager);
                                	break;
                                case "Owner":
                                	hb.Td(
                                	    column: ss.LinkColumn("Owner"),
                                	    value: issueModel.Owner);
                                	break;
                                case "ClassA":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassA"),
                                	    value: issueModel.ClassA);
                                	break;
                                case "ClassB":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassB"),
                                	    value: issueModel.ClassB);
                                	break;
                                case "ClassC":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassC"),
                                	    value: issueModel.ClassC);
                                	break;
                                case "ClassD":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassD"),
                                	    value: issueModel.ClassD);
                                	break;
                                case "ClassE":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassE"),
                                	    value: issueModel.ClassE);
                                	break;
                                case "ClassF":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassF"),
                                	    value: issueModel.ClassF);
                                	break;
                                case "ClassG":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassG"),
                                	    value: issueModel.ClassG);
                                	break;
                                case "ClassH":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassH"),
                                	    value: issueModel.ClassH);
                                	break;
                                case "ClassI":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassI"),
                                	    value: issueModel.ClassI);
                                	break;
                                case "ClassJ":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassJ"),
                                	    value: issueModel.ClassJ);
                                	break;
                                case "ClassK":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassK"),
                                	    value: issueModel.ClassK);
                                	break;
                                case "ClassL":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassL"),
                                	    value: issueModel.ClassL);
                                	break;
                                case "ClassM":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassM"),
                                	    value: issueModel.ClassM);
                                	break;
                                case "ClassN":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassN"),
                                	    value: issueModel.ClassN);
                                	break;
                                case "ClassO":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassO"),
                                	    value: issueModel.ClassO);
                                	break;
                                case "ClassP":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassP"),
                                	    value: issueModel.ClassP);
                                	break;
                                case "ClassQ":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassQ"),
                                	    value: issueModel.ClassQ);
                                	break;
                                case "ClassR":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassR"),
                                	    value: issueModel.ClassR);
                                	break;
                                case "ClassS":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassS"),
                                	    value: issueModel.ClassS);
                                	break;
                                case "ClassT":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassT"),
                                	    value: issueModel.ClassT);
                                	break;
                                case "ClassU":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassU"),
                                	    value: issueModel.ClassU);
                                	break;
                                case "ClassV":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassV"),
                                	    value: issueModel.ClassV);
                                	break;
                                case "ClassW":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassW"),
                                	    value: issueModel.ClassW);
                                	break;
                                case "ClassX":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassX"),
                                	    value: issueModel.ClassX);
                                	break;
                                case "ClassY":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassY"),
                                	    value: issueModel.ClassY);
                                	break;
                                case "ClassZ":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassZ"),
                                	    value: issueModel.ClassZ);
                                	break;
                                case "NumA":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumA"),
                                	    value: issueModel.NumA);
                                	break;
                                case "NumB":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumB"),
                                	    value: issueModel.NumB);
                                	break;
                                case "NumC":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumC"),
                                	    value: issueModel.NumC);
                                	break;
                                case "NumD":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumD"),
                                	    value: issueModel.NumD);
                                	break;
                                case "NumE":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumE"),
                                	    value: issueModel.NumE);
                                	break;
                                case "NumF":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumF"),
                                	    value: issueModel.NumF);
                                	break;
                                case "NumG":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumG"),
                                	    value: issueModel.NumG);
                                	break;
                                case "NumH":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumH"),
                                	    value: issueModel.NumH);
                                	break;
                                case "NumI":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumI"),
                                	    value: issueModel.NumI);
                                	break;
                                case "NumJ":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumJ"),
                                	    value: issueModel.NumJ);
                                	break;
                                case "NumK":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumK"),
                                	    value: issueModel.NumK);
                                	break;
                                case "NumL":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumL"),
                                	    value: issueModel.NumL);
                                	break;
                                case "NumM":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumM"),
                                	    value: issueModel.NumM);
                                	break;
                                case "NumN":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumN"),
                                	    value: issueModel.NumN);
                                	break;
                                case "NumO":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumO"),
                                	    value: issueModel.NumO);
                                	break;
                                case "NumP":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumP"),
                                	    value: issueModel.NumP);
                                	break;
                                case "NumQ":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumQ"),
                                	    value: issueModel.NumQ);
                                	break;
                                case "NumR":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumR"),
                                	    value: issueModel.NumR);
                                	break;
                                case "NumS":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumS"),
                                	    value: issueModel.NumS);
                                	break;
                                case "NumT":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumT"),
                                	    value: issueModel.NumT);
                                	break;
                                case "NumU":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumU"),
                                	    value: issueModel.NumU);
                                	break;
                                case "NumV":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumV"),
                                	    value: issueModel.NumV);
                                	break;
                                case "NumW":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumW"),
                                	    value: issueModel.NumW);
                                	break;
                                case "NumX":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumX"),
                                	    value: issueModel.NumX);
                                	break;
                                case "NumY":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumY"),
                                	    value: issueModel.NumY);
                                	break;
                                case "NumZ":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumZ"),
                                	    value: issueModel.NumZ);
                                	break;
                                case "DateA":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateA"),
                                	    value: issueModel.DateA);
                                	break;
                                case "DateB":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateB"),
                                	    value: issueModel.DateB);
                                	break;
                                case "DateC":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateC"),
                                	    value: issueModel.DateC);
                                	break;
                                case "DateD":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateD"),
                                	    value: issueModel.DateD);
                                	break;
                                case "DateE":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateE"),
                                	    value: issueModel.DateE);
                                	break;
                                case "DateF":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateF"),
                                	    value: issueModel.DateF);
                                	break;
                                case "DateG":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateG"),
                                	    value: issueModel.DateG);
                                	break;
                                case "DateH":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateH"),
                                	    value: issueModel.DateH);
                                	break;
                                case "DateI":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateI"),
                                	    value: issueModel.DateI);
                                	break;
                                case "DateJ":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateJ"),
                                	    value: issueModel.DateJ);
                                	break;
                                case "DateK":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateK"),
                                	    value: issueModel.DateK);
                                	break;
                                case "DateL":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateL"),
                                	    value: issueModel.DateL);
                                	break;
                                case "DateM":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateM"),
                                	    value: issueModel.DateM);
                                	break;
                                case "DateN":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateN"),
                                	    value: issueModel.DateN);
                                	break;
                                case "DateO":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateO"),
                                	    value: issueModel.DateO);
                                	break;
                                case "DateP":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateP"),
                                	    value: issueModel.DateP);
                                	break;
                                case "DateQ":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateQ"),
                                	    value: issueModel.DateQ);
                                	break;
                                case "DateR":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateR"),
                                	    value: issueModel.DateR);
                                	break;
                                case "DateS":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateS"),
                                	    value: issueModel.DateS);
                                	break;
                                case "DateT":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateT"),
                                	    value: issueModel.DateT);
                                	break;
                                case "DateU":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateU"),
                                	    value: issueModel.DateU);
                                	break;
                                case "DateV":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateV"),
                                	    value: issueModel.DateV);
                                	break;
                                case "DateW":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateW"),
                                	    value: issueModel.DateW);
                                	break;
                                case "DateX":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateX"),
                                	    value: issueModel.DateX);
                                	break;
                                case "DateY":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateY"),
                                	    value: issueModel.DateY);
                                	break;
                                case "DateZ":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateZ"),
                                	    value: issueModel.DateZ);
                                	break;
                                case "DescriptionA":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionA"),
                                	    value: issueModel.DescriptionA);
                                	break;
                                case "DescriptionB":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionB"),
                                	    value: issueModel.DescriptionB);
                                	break;
                                case "DescriptionC":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionC"),
                                	    value: issueModel.DescriptionC);
                                	break;
                                case "DescriptionD":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionD"),
                                	    value: issueModel.DescriptionD);
                                	break;
                                case "DescriptionE":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionE"),
                                	    value: issueModel.DescriptionE);
                                	break;
                                case "DescriptionF":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionF"),
                                	    value: issueModel.DescriptionF);
                                	break;
                                case "DescriptionG":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionG"),
                                	    value: issueModel.DescriptionG);
                                	break;
                                case "DescriptionH":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionH"),
                                	    value: issueModel.DescriptionH);
                                	break;
                                case "DescriptionI":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionI"),
                                	    value: issueModel.DescriptionI);
                                	break;
                                case "DescriptionJ":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionJ"),
                                	    value: issueModel.DescriptionJ);
                                	break;
                                case "DescriptionK":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionK"),
                                	    value: issueModel.DescriptionK);
                                	break;
                                case "DescriptionL":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionL"),
                                	    value: issueModel.DescriptionL);
                                	break;
                                case "DescriptionM":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionM"),
                                	    value: issueModel.DescriptionM);
                                	break;
                                case "DescriptionN":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionN"),
                                	    value: issueModel.DescriptionN);
                                	break;
                                case "DescriptionO":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionO"),
                                	    value: issueModel.DescriptionO);
                                	break;
                                case "DescriptionP":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionP"),
                                	    value: issueModel.DescriptionP);
                                	break;
                                case "DescriptionQ":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionQ"),
                                	    value: issueModel.DescriptionQ);
                                	break;
                                case "DescriptionR":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionR"),
                                	    value: issueModel.DescriptionR);
                                	break;
                                case "DescriptionS":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionS"),
                                	    value: issueModel.DescriptionS);
                                	break;
                                case "DescriptionT":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionT"),
                                	    value: issueModel.DescriptionT);
                                	break;
                                case "DescriptionU":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionU"),
                                	    value: issueModel.DescriptionU);
                                	break;
                                case "DescriptionV":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionV"),
                                	    value: issueModel.DescriptionV);
                                	break;
                                case "DescriptionW":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionW"),
                                	    value: issueModel.DescriptionW);
                                	break;
                                case "DescriptionX":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionX"),
                                	    value: issueModel.DescriptionX);
                                	break;
                                case "DescriptionY":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionY"),
                                	    value: issueModel.DescriptionY);
                                	break;
                                case "DescriptionZ":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionZ"),
                                	    value: issueModel.DescriptionZ);
                                	break;
                                case "CheckA":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckA"),
                                	    value: issueModel.CheckA);
                                	break;
                                case "CheckB":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckB"),
                                	    value: issueModel.CheckB);
                                	break;
                                case "CheckC":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckC"),
                                	    value: issueModel.CheckC);
                                	break;
                                case "CheckD":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckD"),
                                	    value: issueModel.CheckD);
                                	break;
                                case "CheckE":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckE"),
                                	    value: issueModel.CheckE);
                                	break;
                                case "CheckF":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckF"),
                                	    value: issueModel.CheckF);
                                	break;
                                case "CheckG":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckG"),
                                	    value: issueModel.CheckG);
                                	break;
                                case "CheckH":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckH"),
                                	    value: issueModel.CheckH);
                                	break;
                                case "CheckI":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckI"),
                                	    value: issueModel.CheckI);
                                	break;
                                case "CheckJ":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckJ"),
                                	    value: issueModel.CheckJ);
                                	break;
                                case "CheckK":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckK"),
                                	    value: issueModel.CheckK);
                                	break;
                                case "CheckL":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckL"),
                                	    value: issueModel.CheckL);
                                	break;
                                case "CheckM":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckM"),
                                	    value: issueModel.CheckM);
                                	break;
                                case "CheckN":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckN"),
                                	    value: issueModel.CheckN);
                                	break;
                                case "CheckO":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckO"),
                                	    value: issueModel.CheckO);
                                	break;
                                case "CheckP":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckP"),
                                	    value: issueModel.CheckP);
                                	break;
                                case "CheckQ":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckQ"),
                                	    value: issueModel.CheckQ);
                                	break;
                                case "CheckR":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckR"),
                                	    value: issueModel.CheckR);
                                	break;
                                case "CheckS":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckS"),
                                	    value: issueModel.CheckS);
                                	break;
                                case "CheckT":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckT"),
                                	    value: issueModel.CheckT);
                                	break;
                                case "CheckU":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckU"),
                                	    value: issueModel.CheckU);
                                	break;
                                case "CheckV":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckV"),
                                	    value: issueModel.CheckV);
                                	break;
                                case "CheckW":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckW"),
                                	    value: issueModel.CheckW);
                                	break;
                                case "CheckX":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckX"),
                                	    value: issueModel.CheckX);
                                	break;
                                case "CheckY":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckY"),
                                	    value: issueModel.CheckY);
                                	break;
                                case "CheckZ":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckZ"),
                                	    value: issueModel.CheckZ);
                                	break;
                                case "Comments":
                                	hb.Td(
                                	    column: ss.LinkColumn("Comments"),
                                	    value: issueModel.Comments);
                                	break;
                                case "Creator":
                                	hb.Td(
                                	    column: ss.LinkColumn("Creator"),
                                	    value: issueModel.Creator);
                                	break;
                                case "Updator":
                                	hb.Td(
                                	    column: ss.LinkColumn("Updator"),
                                	    value: issueModel.Updator);
                                	break;
                                case "CreatedTime":
                                	hb.Td(
                                	    column: ss.LinkColumn("CreatedTime"),
                                	    value: issueModel.CreatedTime);
                                	break;
                                case "VerUp":
                                	hb.Td(
                                	    column: ss.LinkColumn("VerUp"),
                                	    value: issueModel.VerUp);
                                	break;
                                case "Timestamp":
                                	hb.Td(
                                	    column: ss.LinkColumn("Timestamp"),
                                	    value: issueModel.Timestamp);
                                	break;
                            }
                        }));
            });
            return hb;
        }

        private static HtmlBuilder Results(
            this HtmlBuilder hb,
            SiteSettings ss,
            EnumerableRowCollection<DataRow> dataRows)
        {
            dataRows.ForEach(dataRow =>
            {
                var resultModel = new ResultModel(ss, dataRow);
                hb.Tr(
                    attributes: new HtmlAttributes()
                        .Class("grid-row")
                        .DataId(resultModel.ResultId.ToString()),
                    action: () => ss
                        .GetLinkColumns()
                        .ForEach(column =>
                        {
                            switch (column.ColumnName)
                            {
                                case "SiteId":
                                	hb.Td(
                                	    column: ss.LinkColumn("SiteId"),
                                	    value: resultModel.SiteId);
                                	break;
                                case "UpdatedTime":
                                	hb.Td(
                                	    column: ss.LinkColumn("UpdatedTime"),
                                	    value: resultModel.UpdatedTime);
                                	break;
                                case "ResultId":
                                	hb.Td(
                                	    column: ss.LinkColumn("ResultId"),
                                	    value: resultModel.ResultId);
                                	break;
                                case "Ver":
                                	hb.Td(
                                	    column: ss.LinkColumn("Ver"),
                                	    value: resultModel.Ver);
                                	break;
                                case "Title":
                                	hb.Td(
                                	    column: ss.LinkColumn("Title"),
                                	    value: resultModel.Title);
                                	break;
                                case "Body":
                                	hb.Td(
                                	    column: ss.LinkColumn("Body"),
                                	    value: resultModel.Body);
                                	break;
                                case "TitleBody":
                                	hb.Td(
                                	    column: ss.LinkColumn("TitleBody"),
                                	    value: resultModel.TitleBody);
                                	break;
                                case "Status":
                                	hb.Td(
                                	    column: ss.LinkColumn("Status"),
                                	    value: resultModel.Status);
                                	break;
                                case "Manager":
                                	hb.Td(
                                	    column: ss.LinkColumn("Manager"),
                                	    value: resultModel.Manager);
                                	break;
                                case "Owner":
                                	hb.Td(
                                	    column: ss.LinkColumn("Owner"),
                                	    value: resultModel.Owner);
                                	break;
                                case "ClassA":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassA"),
                                	    value: resultModel.ClassA);
                                	break;
                                case "ClassB":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassB"),
                                	    value: resultModel.ClassB);
                                	break;
                                case "ClassC":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassC"),
                                	    value: resultModel.ClassC);
                                	break;
                                case "ClassD":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassD"),
                                	    value: resultModel.ClassD);
                                	break;
                                case "ClassE":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassE"),
                                	    value: resultModel.ClassE);
                                	break;
                                case "ClassF":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassF"),
                                	    value: resultModel.ClassF);
                                	break;
                                case "ClassG":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassG"),
                                	    value: resultModel.ClassG);
                                	break;
                                case "ClassH":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassH"),
                                	    value: resultModel.ClassH);
                                	break;
                                case "ClassI":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassI"),
                                	    value: resultModel.ClassI);
                                	break;
                                case "ClassJ":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassJ"),
                                	    value: resultModel.ClassJ);
                                	break;
                                case "ClassK":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassK"),
                                	    value: resultModel.ClassK);
                                	break;
                                case "ClassL":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassL"),
                                	    value: resultModel.ClassL);
                                	break;
                                case "ClassM":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassM"),
                                	    value: resultModel.ClassM);
                                	break;
                                case "ClassN":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassN"),
                                	    value: resultModel.ClassN);
                                	break;
                                case "ClassO":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassO"),
                                	    value: resultModel.ClassO);
                                	break;
                                case "ClassP":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassP"),
                                	    value: resultModel.ClassP);
                                	break;
                                case "ClassQ":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassQ"),
                                	    value: resultModel.ClassQ);
                                	break;
                                case "ClassR":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassR"),
                                	    value: resultModel.ClassR);
                                	break;
                                case "ClassS":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassS"),
                                	    value: resultModel.ClassS);
                                	break;
                                case "ClassT":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassT"),
                                	    value: resultModel.ClassT);
                                	break;
                                case "ClassU":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassU"),
                                	    value: resultModel.ClassU);
                                	break;
                                case "ClassV":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassV"),
                                	    value: resultModel.ClassV);
                                	break;
                                case "ClassW":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassW"),
                                	    value: resultModel.ClassW);
                                	break;
                                case "ClassX":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassX"),
                                	    value: resultModel.ClassX);
                                	break;
                                case "ClassY":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassY"),
                                	    value: resultModel.ClassY);
                                	break;
                                case "ClassZ":
                                	hb.Td(
                                	    column: ss.LinkColumn("ClassZ"),
                                	    value: resultModel.ClassZ);
                                	break;
                                case "NumA":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumA"),
                                	    value: resultModel.NumA);
                                	break;
                                case "NumB":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumB"),
                                	    value: resultModel.NumB);
                                	break;
                                case "NumC":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumC"),
                                	    value: resultModel.NumC);
                                	break;
                                case "NumD":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumD"),
                                	    value: resultModel.NumD);
                                	break;
                                case "NumE":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumE"),
                                	    value: resultModel.NumE);
                                	break;
                                case "NumF":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumF"),
                                	    value: resultModel.NumF);
                                	break;
                                case "NumG":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumG"),
                                	    value: resultModel.NumG);
                                	break;
                                case "NumH":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumH"),
                                	    value: resultModel.NumH);
                                	break;
                                case "NumI":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumI"),
                                	    value: resultModel.NumI);
                                	break;
                                case "NumJ":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumJ"),
                                	    value: resultModel.NumJ);
                                	break;
                                case "NumK":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumK"),
                                	    value: resultModel.NumK);
                                	break;
                                case "NumL":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumL"),
                                	    value: resultModel.NumL);
                                	break;
                                case "NumM":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumM"),
                                	    value: resultModel.NumM);
                                	break;
                                case "NumN":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumN"),
                                	    value: resultModel.NumN);
                                	break;
                                case "NumO":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumO"),
                                	    value: resultModel.NumO);
                                	break;
                                case "NumP":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumP"),
                                	    value: resultModel.NumP);
                                	break;
                                case "NumQ":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumQ"),
                                	    value: resultModel.NumQ);
                                	break;
                                case "NumR":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumR"),
                                	    value: resultModel.NumR);
                                	break;
                                case "NumS":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumS"),
                                	    value: resultModel.NumS);
                                	break;
                                case "NumT":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumT"),
                                	    value: resultModel.NumT);
                                	break;
                                case "NumU":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumU"),
                                	    value: resultModel.NumU);
                                	break;
                                case "NumV":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumV"),
                                	    value: resultModel.NumV);
                                	break;
                                case "NumW":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumW"),
                                	    value: resultModel.NumW);
                                	break;
                                case "NumX":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumX"),
                                	    value: resultModel.NumX);
                                	break;
                                case "NumY":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumY"),
                                	    value: resultModel.NumY);
                                	break;
                                case "NumZ":
                                	hb.Td(
                                	    column: ss.LinkColumn("NumZ"),
                                	    value: resultModel.NumZ);
                                	break;
                                case "DateA":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateA"),
                                	    value: resultModel.DateA);
                                	break;
                                case "DateB":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateB"),
                                	    value: resultModel.DateB);
                                	break;
                                case "DateC":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateC"),
                                	    value: resultModel.DateC);
                                	break;
                                case "DateD":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateD"),
                                	    value: resultModel.DateD);
                                	break;
                                case "DateE":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateE"),
                                	    value: resultModel.DateE);
                                	break;
                                case "DateF":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateF"),
                                	    value: resultModel.DateF);
                                	break;
                                case "DateG":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateG"),
                                	    value: resultModel.DateG);
                                	break;
                                case "DateH":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateH"),
                                	    value: resultModel.DateH);
                                	break;
                                case "DateI":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateI"),
                                	    value: resultModel.DateI);
                                	break;
                                case "DateJ":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateJ"),
                                	    value: resultModel.DateJ);
                                	break;
                                case "DateK":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateK"),
                                	    value: resultModel.DateK);
                                	break;
                                case "DateL":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateL"),
                                	    value: resultModel.DateL);
                                	break;
                                case "DateM":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateM"),
                                	    value: resultModel.DateM);
                                	break;
                                case "DateN":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateN"),
                                	    value: resultModel.DateN);
                                	break;
                                case "DateO":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateO"),
                                	    value: resultModel.DateO);
                                	break;
                                case "DateP":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateP"),
                                	    value: resultModel.DateP);
                                	break;
                                case "DateQ":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateQ"),
                                	    value: resultModel.DateQ);
                                	break;
                                case "DateR":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateR"),
                                	    value: resultModel.DateR);
                                	break;
                                case "DateS":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateS"),
                                	    value: resultModel.DateS);
                                	break;
                                case "DateT":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateT"),
                                	    value: resultModel.DateT);
                                	break;
                                case "DateU":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateU"),
                                	    value: resultModel.DateU);
                                	break;
                                case "DateV":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateV"),
                                	    value: resultModel.DateV);
                                	break;
                                case "DateW":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateW"),
                                	    value: resultModel.DateW);
                                	break;
                                case "DateX":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateX"),
                                	    value: resultModel.DateX);
                                	break;
                                case "DateY":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateY"),
                                	    value: resultModel.DateY);
                                	break;
                                case "DateZ":
                                	hb.Td(
                                	    column: ss.LinkColumn("DateZ"),
                                	    value: resultModel.DateZ);
                                	break;
                                case "DescriptionA":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionA"),
                                	    value: resultModel.DescriptionA);
                                	break;
                                case "DescriptionB":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionB"),
                                	    value: resultModel.DescriptionB);
                                	break;
                                case "DescriptionC":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionC"),
                                	    value: resultModel.DescriptionC);
                                	break;
                                case "DescriptionD":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionD"),
                                	    value: resultModel.DescriptionD);
                                	break;
                                case "DescriptionE":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionE"),
                                	    value: resultModel.DescriptionE);
                                	break;
                                case "DescriptionF":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionF"),
                                	    value: resultModel.DescriptionF);
                                	break;
                                case "DescriptionG":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionG"),
                                	    value: resultModel.DescriptionG);
                                	break;
                                case "DescriptionH":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionH"),
                                	    value: resultModel.DescriptionH);
                                	break;
                                case "DescriptionI":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionI"),
                                	    value: resultModel.DescriptionI);
                                	break;
                                case "DescriptionJ":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionJ"),
                                	    value: resultModel.DescriptionJ);
                                	break;
                                case "DescriptionK":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionK"),
                                	    value: resultModel.DescriptionK);
                                	break;
                                case "DescriptionL":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionL"),
                                	    value: resultModel.DescriptionL);
                                	break;
                                case "DescriptionM":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionM"),
                                	    value: resultModel.DescriptionM);
                                	break;
                                case "DescriptionN":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionN"),
                                	    value: resultModel.DescriptionN);
                                	break;
                                case "DescriptionO":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionO"),
                                	    value: resultModel.DescriptionO);
                                	break;
                                case "DescriptionP":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionP"),
                                	    value: resultModel.DescriptionP);
                                	break;
                                case "DescriptionQ":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionQ"),
                                	    value: resultModel.DescriptionQ);
                                	break;
                                case "DescriptionR":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionR"),
                                	    value: resultModel.DescriptionR);
                                	break;
                                case "DescriptionS":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionS"),
                                	    value: resultModel.DescriptionS);
                                	break;
                                case "DescriptionT":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionT"),
                                	    value: resultModel.DescriptionT);
                                	break;
                                case "DescriptionU":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionU"),
                                	    value: resultModel.DescriptionU);
                                	break;
                                case "DescriptionV":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionV"),
                                	    value: resultModel.DescriptionV);
                                	break;
                                case "DescriptionW":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionW"),
                                	    value: resultModel.DescriptionW);
                                	break;
                                case "DescriptionX":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionX"),
                                	    value: resultModel.DescriptionX);
                                	break;
                                case "DescriptionY":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionY"),
                                	    value: resultModel.DescriptionY);
                                	break;
                                case "DescriptionZ":
                                	hb.Td(
                                	    column: ss.LinkColumn("DescriptionZ"),
                                	    value: resultModel.DescriptionZ);
                                	break;
                                case "CheckA":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckA"),
                                	    value: resultModel.CheckA);
                                	break;
                                case "CheckB":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckB"),
                                	    value: resultModel.CheckB);
                                	break;
                                case "CheckC":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckC"),
                                	    value: resultModel.CheckC);
                                	break;
                                case "CheckD":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckD"),
                                	    value: resultModel.CheckD);
                                	break;
                                case "CheckE":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckE"),
                                	    value: resultModel.CheckE);
                                	break;
                                case "CheckF":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckF"),
                                	    value: resultModel.CheckF);
                                	break;
                                case "CheckG":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckG"),
                                	    value: resultModel.CheckG);
                                	break;
                                case "CheckH":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckH"),
                                	    value: resultModel.CheckH);
                                	break;
                                case "CheckI":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckI"),
                                	    value: resultModel.CheckI);
                                	break;
                                case "CheckJ":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckJ"),
                                	    value: resultModel.CheckJ);
                                	break;
                                case "CheckK":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckK"),
                                	    value: resultModel.CheckK);
                                	break;
                                case "CheckL":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckL"),
                                	    value: resultModel.CheckL);
                                	break;
                                case "CheckM":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckM"),
                                	    value: resultModel.CheckM);
                                	break;
                                case "CheckN":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckN"),
                                	    value: resultModel.CheckN);
                                	break;
                                case "CheckO":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckO"),
                                	    value: resultModel.CheckO);
                                	break;
                                case "CheckP":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckP"),
                                	    value: resultModel.CheckP);
                                	break;
                                case "CheckQ":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckQ"),
                                	    value: resultModel.CheckQ);
                                	break;
                                case "CheckR":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckR"),
                                	    value: resultModel.CheckR);
                                	break;
                                case "CheckS":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckS"),
                                	    value: resultModel.CheckS);
                                	break;
                                case "CheckT":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckT"),
                                	    value: resultModel.CheckT);
                                	break;
                                case "CheckU":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckU"),
                                	    value: resultModel.CheckU);
                                	break;
                                case "CheckV":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckV"),
                                	    value: resultModel.CheckV);
                                	break;
                                case "CheckW":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckW"),
                                	    value: resultModel.CheckW);
                                	break;
                                case "CheckX":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckX"),
                                	    value: resultModel.CheckX);
                                	break;
                                case "CheckY":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckY"),
                                	    value: resultModel.CheckY);
                                	break;
                                case "CheckZ":
                                	hb.Td(
                                	    column: ss.LinkColumn("CheckZ"),
                                	    value: resultModel.CheckZ);
                                	break;
                                case "Comments":
                                	hb.Td(
                                	    column: ss.LinkColumn("Comments"),
                                	    value: resultModel.Comments);
                                	break;
                                case "Creator":
                                	hb.Td(
                                	    column: ss.LinkColumn("Creator"),
                                	    value: resultModel.Creator);
                                	break;
                                case "Updator":
                                	hb.Td(
                                	    column: ss.LinkColumn("Updator"),
                                	    value: resultModel.Updator);
                                	break;
                                case "CreatedTime":
                                	hb.Td(
                                	    column: ss.LinkColumn("CreatedTime"),
                                	    value: resultModel.CreatedTime);
                                	break;
                                case "VerUp":
                                	hb.Td(
                                	    column: ss.LinkColumn("VerUp"),
                                	    value: resultModel.VerUp);
                                	break;
                                case "Timestamp":
                                	hb.Td(
                                	    column: ss.LinkColumn("Timestamp"),
                                	    value: resultModel.Timestamp);
                                	break;
                            }
                        }));
            });
            return hb;
        }
    }
}
