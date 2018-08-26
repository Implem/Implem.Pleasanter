using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
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

        private static void UpdateIssues(
            Context context,
            SiteSettings ss,
            long siteId,
            long id,
            IEnumerable<int> selected = null,
            bool hasFormula = false)
        {
            new IssueCollection(
                context: context,
                ss: ss,
                where: Rds.IssuesWhere()
                    .SiteId(siteId)
                    .IssueId(id, _using: id != 0))
                        .ForEach(issueModel =>
                        {
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
            new ResultCollection(
                context: context,
                ss: ss,
                where: Rds.ResultsWhere()
                    .SiteId(siteId)
                    .ResultId(id, _using: id != 0))
                        .ForEach(resultModel =>
                        {
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
            new WikiCollection(
                context: context,
                ss: ss,
                where: Rds.WikisWhere()
                    .SiteId(siteId)
                    .WikiId(id, _using: id != 0))
                        .ForEach(wikiModel =>
                        {
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
