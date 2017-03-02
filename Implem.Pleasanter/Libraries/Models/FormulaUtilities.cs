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

        public static void Update(long id)
        {
            Update(new SiteModel(new ItemModel(id).SiteId), id);
        }

        private static void Update(SiteModel siteModel, long id, IEnumerable<int> selected = null)
        {
            var hasFormula = siteModel.SiteSettings.Formulas?.Any() ?? false;
            switch (siteModel.ReferenceType)
            {
                case "Issues":
                    UpdateIssues(siteModel, id, selected, hasFormula: hasFormula);
                    break;
                case "Results":
                    UpdateResults(siteModel, id, selected, hasFormula: hasFormula);
                    break;
                case "Wikis":
                    UpdateWikis(siteModel, id, selected, hasFormula: hasFormula);
                    break;
                default: break;
            }
        }

        private static void UpdateIssues(
            SiteModel siteModel,
            long id,
            IEnumerable<int> selected = null,
            bool hasFormula = false)
        {
            new IssueCollection(
                ss: siteModel.SiteSettings,
                where: Rds.IssuesWhere()
                    .SiteId(siteModel.SiteId)
                    .IssueId(id, _using: id != 0))
                        .ForEach(issueModel =>
                        {
                            var ss = SiteSettingsUtilities.Get(siteModel);
                            if (hasFormula) issueModel.UpdateFormulaColumns(ss, selected);
                            issueModel.UpdateRelatedRecords(
                                ss: ss,
                                addUpdatedTimeParam: false,
                                addUpdatorParam: false,
                                updateItems: false);
                        });
        }

        private static void UpdateResults(
            SiteModel siteModel,
            long id,
            IEnumerable<int> selected = null,
            bool hasFormula = false)
        {
            new ResultCollection(
                ss: siteModel.SiteSettings,
                where: Rds.ResultsWhere()
                    .SiteId(siteModel.SiteId)
                    .ResultId(id, _using: id != 0))
                        .ForEach(resultModel =>
                        {
                            var ss = SiteSettingsUtilities.Get(siteModel);
                            if (hasFormula) resultModel.UpdateFormulaColumns(ss, selected);
                            resultModel.UpdateRelatedRecords(
                                ss: ss,
                                addUpdatedTimeParam: false,
                                addUpdatorParam: false,
                                updateItems: false);
                        });
        }

        private static void UpdateWikis(
            SiteModel siteModel,
            long id,
            IEnumerable<int> selected = null,
            bool hasFormula = false)
        {
            new WikiCollection(
                ss: siteModel.SiteSettings,
                where: Rds.WikisWhere()
                    .SiteId(siteModel.SiteId)
                    .WikiId(id, _using: id != 0))
                        .ForEach(wikiModel =>
                        {
                            var ss = SiteSettingsUtilities.Get(siteModel);
                            if (hasFormula) wikiModel.UpdateFormulaColumns(ss, selected);
                            wikiModel.UpdateRelatedRecords(
                                ss: ss,
                                addUpdatedTimeParam: false,
                                addUpdatorParam: false,
                                updateItems: false);
                        });
        }
    }
}
