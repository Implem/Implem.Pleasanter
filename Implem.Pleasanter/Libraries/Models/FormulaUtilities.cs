using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Models
{
    public static class FormulaUtilities
    {
        public static void Synchronize(SiteModel siteModel, IEnumerable<int> selected = null)
        {
            Update(siteModel, 0, selected);
        }

        private static void Update(SiteModel siteModel, long id, IEnumerable<int> selected = null)
        {
            var hasFormula = siteModel.SiteSettings.Formulas?.Any() ?? false;
            var ss = SiteSettingsUtilities.Get(siteModel, id);
            switch (siteModel.ReferenceType)
            {
                case "Issues":
                    UpdateIssues(ss, siteModel.SiteId, id, selected, hasFormula: hasFormula);
                    break;
                case "Results":
                    UpdateResults(ss, siteModel.SiteId, id, selected, hasFormula: hasFormula);
                    break;
                case "Wikis":
                    UpdateWikis(ss, siteModel.SiteId, id, selected, hasFormula: hasFormula);
                    break;
                default: break;
            }
        }

        private static void UpdateIssues(
            SiteSettings ss,
            long siteId,
            long id,
            IEnumerable<int> selected = null,
            bool hasFormula = false)
        {
            new IssueCollection(
                ss: ss,
                where: Rds.IssuesWhere()
                    .SiteId(siteId)
                    .IssueId(id, _using: id != 0))
                        .ForEach(issueModel =>
                        {
                            if (hasFormula) issueModel.UpdateFormulaColumns(ss, selected);
                            issueModel.UpdateRelatedRecords(
                                ss: ss,
                                extendedSqls: true,
                                addUpdatedTimeParam: false,
                                addUpdatorParam: false,
                                updateItems: false);
                        });
        }

        private static void UpdateResults(
            SiteSettings ss,
            long siteId,
            long id,
            IEnumerable<int> selected = null,
            bool hasFormula = false)
        {
            new ResultCollection(
                ss: ss,
                where: Rds.ResultsWhere()
                    .SiteId(siteId)
                    .ResultId(id, _using: id != 0))
                        .ForEach(resultModel =>
                        {
                            if (hasFormula) resultModel.UpdateFormulaColumns(ss, selected);
                            resultModel.UpdateRelatedRecords(
                                ss: ss,
                                extendedSqls: true,
                                addUpdatedTimeParam: false,
                                addUpdatorParam: false,
                                updateItems: false);
                        });
        }

        private static void UpdateWikis(
            SiteSettings ss,
            long siteId,
            long id,
            IEnumerable<int> selected = null,
            bool hasFormula = false)
        {
            new WikiCollection(
                ss: ss,
                where: Rds.WikisWhere()
                    .SiteId(siteId)
                    .WikiId(id, _using: id != 0))
                        .ForEach(wikiModel =>
                        {
                            if (hasFormula) wikiModel.UpdateFormulaColumns(ss, selected);
                            wikiModel.UpdateRelatedRecords(
                                ss: ss,
                                extendedSqls: true,
                                addUpdatedTimeParam: false,
                                addUpdatorParam: false,
                                updateItems: false);
                        });
        }
    }
}
