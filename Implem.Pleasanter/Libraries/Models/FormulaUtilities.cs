using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Models;
namespace Implem.Pleasanter.Libraries.Models
{
    public static class FormulaUtilities
    {
        public static void Synchronize(SiteModel siteModel)
        {
            Update(siteModel, 0);
        }

        public static void Update(long id)
        {
            Update(new SiteModel(new ItemModel(id).SiteId), id);
        }

        private static void Update(SiteModel siteModel, long id)
        {
            var hasFormula = siteModel.SiteSettings.FormulaHash?.Count > 0;
            switch (siteModel.ReferenceType)
            {
                case "Issues": UpdateIssues(siteModel, id, hasFormula: hasFormula); break;
                case "Results": UpdateResults(siteModel, id, hasFormula: hasFormula); break;
                case "Wikis": UpdateWikis(siteModel, id, hasFormula: hasFormula); break;
                default: break;
            }
        }

        private static void UpdateIssues(SiteModel siteModel, long id, bool hasFormula = false)
        {
            new IssueCollection(
                siteSettings: siteModel.SiteSettings,
                permissionType: siteModel.PermissionType,
                where: Rds.IssuesWhere()
                    .SiteId(siteModel.SiteId)
                    .IssueId(id, _using: id != 0))
                        .ForEach(issueModel =>
                        {
                            if (hasFormula) issueModel.UpdateFormulaColumns();
                            issueModel.UpdateRelatedRecords(
                                addUpdatedTimeParam: false, addUpdatorParam: false);
                        });
        }

        private static void UpdateResults(SiteModel siteModel, long id, bool hasFormula = false)
        {
            new ResultCollection(
                siteSettings: siteModel.SiteSettings,
                permissionType: siteModel.PermissionType,
                where: Rds.ResultsWhere()
                    .SiteId(siteModel.SiteId)
                    .ResultId(id, _using: id != 0))
                        .ForEach(resultModel =>
                        {
                            if (hasFormula) resultModel.UpdateFormulaColumns();
                            resultModel.UpdateRelatedRecords(
                                addUpdatedTimeParam: false, addUpdatorParam: false);
                        });
        }

        private static void UpdateWikis(SiteModel siteModel, long id, bool hasFormula = false)
        {
            new WikiCollection(
                siteSettings: siteModel.SiteSettings,
                permissionType: siteModel.PermissionType,
                where: Rds.WikisWhere()
                    .SiteId(siteModel.SiteId)
                    .WikiId(id, _using: id != 0))
                        .ForEach(wikiModel =>
                        {
                            if (hasFormula) wikiModel.UpdateFormulaColumns();
                            wikiModel.UpdateRelatedRecords(
                                addUpdatedTimeParam: false, addUpdatorParam: false);
                        });
        }
    }
}
