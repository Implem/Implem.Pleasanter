using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Models;
namespace Implem.Pleasanter.Libraries.Models
{
    public static class FormulaUtilities
    {
        public static void Synchronize(SiteModel siteModel)
        {
            Update(siteModel, 0, updateRelatedRecords: true);
        }

        public static void Update(long id)
        {
            Update(new SiteModel(new ItemModel(id).SiteId), id);
        }

        private static void Update(SiteModel siteModel, long id, bool updateRelatedRecords = false)
        {
            if (siteModel.SiteSettings.FormulaHash?.Count > 0)
            {
                switch (siteModel.ReferenceType)
                {
                    case "Issues":
                        UpdateIssues(
                            siteModel,
                            id,
                            updateFormula: true,
                            updateRelatedRecords: updateRelatedRecords);
                        break;
                    case "Results":
                        UpdateResults(
                            siteModel,
                            id,
                            updateFormula: true,
                            updateRelatedRecords: updateRelatedRecords);
                        break;
                    case "Wikis":
                        UpdateWikis(
                            siteModel,
                            id,
                            updateFormula: true,
                            updateRelatedRecords: updateRelatedRecords);
                        break;
                    default: break;
                }
            }
            else if (updateRelatedRecords)
            {
                switch (siteModel.ReferenceType)
                {
                    case "Issues":
                        UpdateIssues(siteModel, id, updateRelatedRecords: true);
                        break;
                    case "Results":
                        UpdateResults(siteModel, id, updateRelatedRecords: true);
                        break;
                    case "Wikis":
                        UpdateWikis(siteModel, id, updateRelatedRecords: true);
                        break;
                    default: break;
                }            
            }
        }

        private static void UpdateIssues(
            SiteModel siteModel,
            long id,
            bool updateFormula = false,
            bool updateRelatedRecords = false)
        {
            new IssueCollection(
                siteSettings: siteModel.SiteSettings,
                permissionType: siteModel.PermissionType,
                where: Rds.IssuesWhere()
                    .SiteId(siteModel.SiteId)
                    .IssueId(id, _using: id != 0))
                        .ForEach(issueModel =>
                        {
                            if (updateFormula)
                            {
                                issueModel.UpdateFormulaColumns();
                            }
                            if (updateRelatedRecords)
                            {
                                issueModel.UpdateRelatedRecords(
                                    addUpdatedTimeParam: false, addUpdatorParam: false);
                            }
                        });
        }

        private static void UpdateResults(
            SiteModel siteModel,
            long id,
            bool updateFormula = false,
            bool updateRelatedRecords = false)
        {
            new ResultCollection(
                siteSettings: siteModel.SiteSettings,
                permissionType: siteModel.PermissionType,
                where: Rds.ResultsWhere()
                    .SiteId(siteModel.SiteId)
                    .ResultId(id, _using: id != 0))
                        .ForEach(resultModel =>
                        {
                            if (updateFormula)
                            {
                                resultModel.UpdateFormulaColumns();
                            }
                            if (updateRelatedRecords)
                            {
                                resultModel.UpdateRelatedRecords(
                                    addUpdatedTimeParam: false, addUpdatorParam: false);
                            }
                        });
        }

        private static void UpdateWikis(
            SiteModel siteModel,
            long id,
            bool updateFormula = false,
            bool updateRelatedRecords = false)
        {
            new WikiCollection(
                siteSettings: siteModel.SiteSettings,
                permissionType: siteModel.PermissionType,
                where: Rds.WikisWhere()
                    .SiteId(siteModel.SiteId)
                    .WikiId(id, _using: id != 0))
                        .ForEach(wikiModel =>
                        {
                            if (updateFormula)
                            {
                                wikiModel.UpdateFormulaColumns();
                            }
                            if (updateRelatedRecords)
                            {
                                wikiModel.UpdateRelatedRecords(
                                    addUpdatedTimeParam: false, addUpdatorParam: false);
                            }
                        });
        }
    }
}
