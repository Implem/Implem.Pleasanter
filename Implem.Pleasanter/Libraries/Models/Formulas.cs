using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Models;
namespace Implem.Pleasanter.Libraries.Models
{
    public static class Formulas
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
            if (siteModel.SiteSettings.FormulaHash?.Count > 0)
            {
                switch (siteModel.ReferenceType)
                {
                    case "Issues": UpdateIssues(siteModel, id); break;
                    case "Results": UpdateResults(siteModel, id); break;
                    case "Wikis": UpdateWikis(siteModel, id); break;
                    default: break;
                }
            }
        }

        private static void UpdateIssues(SiteModel siteModel, long id)
        {
            new IssueCollection(
                siteSettings: siteModel.SiteSettings,
                permissionType: siteModel.PermissionType,
                where: Rds.IssuesWhere()
                    .SiteId(siteModel.SiteId)
                    .IssueId(id, _using: id != 0))
                        .ForEach(issueModel =>
                            issueModel.UpdateFormulaColumns());
        }

        private static void UpdateResults(SiteModel siteModel, long id)
        {
            new ResultCollection(
                siteSettings: siteModel.SiteSettings,
                permissionType: siteModel.PermissionType,
                where: Rds.ResultsWhere()
                    .SiteId(siteModel.SiteId)
                    .ResultId(id, _using: id != 0))
                        .ForEach(resultModel =>
                            resultModel.UpdateFormulaColumns());
        }

        private static void UpdateWikis(SiteModel siteModel, long id)
        {
            new WikiCollection(
                siteSettings: siteModel.SiteSettings,
                permissionType: siteModel.PermissionType,
                where: Rds.WikisWhere()
                    .SiteId(siteModel.SiteId)
                    .WikiId(id, _using: id != 0))
                        .ForEach(wikiModel =>
                            wikiModel.UpdateFormulaColumns());
        }
    }
}
