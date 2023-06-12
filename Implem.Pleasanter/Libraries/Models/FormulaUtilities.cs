using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Models
{
    public static class FormulaUtilities
    {
        public static void Synchronize(
            Context context, SiteModel siteModel, IEnumerable<int> selected = null)
        {
            Update(
                context: context,
                siteModel: siteModel,
                id: 0,
                selected: selected);
        }

        private static void Update(
            Context context, SiteModel siteModel, long id, IEnumerable<int> selected = null)
        {
            var hasFormula = siteModel.SiteSettings.Formulas?.Any() ?? false;
            var ss = SiteSettingsUtilities.Get(
                context: context, siteModel: siteModel, referenceId: id);
            switch (siteModel.ReferenceType)
            {
                case "Dashboards":
                    UpdateDashboards(
                        context: context,
                        ss: ss,
                        siteId: siteModel.SiteId,
                        id: id,
                        selected: selected,
                        hasFormula: hasFormula);
                    break;
                case "Issues":
                    UpdateIssues(
                        context: context,
                        ss: ss,
                        siteId: siteModel.SiteId,
                        id: id,
                        selected: selected,
                        hasFormula: hasFormula);
                    break;
                case "Results":
                    UpdateResults(
                        context: context,
                        ss: ss,
                        siteId: siteModel.SiteId,
                        id: id,
                        selected: selected,
                        hasFormula: hasFormula);
                    break;
                case "Wikis":
                    UpdateWikis(
                        context: context,
                        ss: ss,
                        siteId: siteModel.SiteId,
                        id: id,
                        selected: selected,
                        hasFormula: hasFormula);
                    break;
                default: break;
            }
        }

        private static void UpdateDashboards(
            Context context,
            SiteSettings ss,
            long siteId,
            long id,
            IEnumerable<int> selected = null,
            bool hasFormula = false)
        {
            new DashboardCollection(
                context: context,
                ss: ss,
                where: Rds.DashboardsWhere()
                    .SiteId(siteId)
                    .DashboardId(id, _using: id != 0))
                        .ForEach(dashboardModel =>
                        {
                            if (hasFormula) dashboardModel.UpdateFormulaColumns(
                                context: context, ss: ss, selected: selected);
                            dashboardModel.UpdateRelatedRecords(
                                context: context,
                                ss: ss,
                                extendedSqls: true,
                                addUpdatedTimeParam: false,
                                addUpdatorParam: false,
                                updateItems: false);
                        });
        }

        private static void UpdateIssues(
            Context context,
            SiteSettings ss,
            long siteId,
            long id,
            IEnumerable<int> selected = null,
            bool hasFormula = false)
        {
            Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectIssues(
                    column: Rds.IssuesColumn().IssueId(),
                    where: Rds.IssuesWhere()
                        .SiteId(siteId)
                        .IssueId(id, _using: id != 0)))
                            .AsEnumerable()
                            .Select(dataRow => dataRow.Long("IssueId"))
                            .ForEach(issueId =>
                            {
                                var issueModel = new IssueModel(
                                    context: context,
                                    ss: ss,
                                    issueId: issueId,
                                    column: Rds.IssuesDefaultColumns());
                                if (hasFormula) issueModel.UpdateFormulaColumns(
                                    context: context, ss: ss, selected: selected);
                                issueModel.UpdateRelatedRecords(
                                    context: context,
                                    ss: ss,
                                    extendedSqls: true,
                                    addUpdatedTimeParam: false,
                                    addUpdatorParam: false,
                                    updateItems: false);
                            });
        }

        private static void UpdateResults(
            Context context,
            SiteSettings ss,
            long siteId,
            long id,
            IEnumerable<int> selected = null,
            bool hasFormula = false)
        {
            Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectResults(
                    column: Rds.ResultsColumn().ResultId(),
                    where: Rds.ResultsWhere()
                        .SiteId(siteId)
                        .ResultId(id, _using: id != 0)))
                            .AsEnumerable()
                            .Select(dataRow => dataRow.Long("ResultId"))
                            .ForEach(resultId =>
                            {
                                var resultModel = new ResultModel(
                                    context: context,
                                    ss: ss,
                                    resultId: resultId,
                                    column: Rds.ResultsDefaultColumns());
                                if (hasFormula) resultModel.UpdateFormulaColumns(
                                    context: context, ss: ss, selected: selected);
                                resultModel.UpdateRelatedRecords(
                                    context: context,
                                    ss: ss,
                                    extendedSqls: true,
                                    addUpdatedTimeParam: false,
                                    addUpdatorParam: false,
                                    updateItems: false);
                            });
        }

        private static void UpdateWikis(
            Context context,
            SiteSettings ss,
            long siteId,
            long id,
            IEnumerable<int> selected = null,
            bool hasFormula = false)
        {
            Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectWikis(
                    column: Rds.WikisColumn().WikiId(),
                    where: Rds.WikisWhere()
                        .SiteId(siteId)
                        .WikiId(id, _using: id != 0)))
                            .AsEnumerable()
                            .Select(dataRow => dataRow.Long("WikiId"))
                            .ForEach(wikiId =>
                            {
                                var wikiModel = new WikiModel(
                                    context: context,
                                    ss: ss,
                                    wikiId: wikiId,
                                    column: Rds.WikisDefaultColumns());
                                if (hasFormula) wikiModel.UpdateFormulaColumns(
                                    context: context, ss: ss, selected: selected);
                                wikiModel.UpdateRelatedRecords(
                                    context: context,
                                    ss: ss,
                                    extendedSqls: true,
                                    addUpdatedTimeParam: false,
                                    addUpdatorParam: false,
                                    updateItems: false);
                            });
        }
    }
}
