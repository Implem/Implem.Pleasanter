using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Search;
using Implem.Pleasanter.Models;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers
{
    public class ItemsController
    {
        public string Index(Context context, long id = 0)
        {
            if (!context.Ajax)
            {
                var log = new SysLogModel(context: context);
                var html = new ItemModel(context: context, referenceId: id).Index(context: context);
                log.Finish(context: context, responseSize: html.Length);
                return html;
            }
            else
            {
                var log = new SysLogModel(context: context);
                var json = new ItemModel(context: context, referenceId: id).IndexJson(context: context);
                log.Finish(context: context, responseSize: json.Length);
                return json;
            }
        }

        public string TrashBox(Context context, long id = 0)
        {
            if (!context.Ajax)
            {
                var log = new SysLogModel(context: context);
                var html = new ItemModel(context: context, referenceId: id).TrashBox(context: context);
                log.Finish(context: context, responseSize: html.Length);
                return html;
            }
            else
            {
                var log = new SysLogModel(context: context);
                var json = new ItemModel(context: context, referenceId: id).TrashBoxJson(context: context);
                log.Finish(context: context, responseSize: json.Length);
                return json;
            }
        }

        public string Calendar(Context context, long id = 0)
        {
            if (!context.Ajax)
            {
                var log = new SysLogModel(context: context);
                var html = new ItemModel(context: context, referenceId: id).Calendar(context: context);
                log.Finish(context: context, responseSize: html.Length);
                return html;
            }
            else
            {
                var log = new SysLogModel(context: context);
                var json = new ItemModel(context: context, referenceId: id).CalendarJson(context: context);
                log.Finish(context: context, responseSize: json.Length);
                return json;
            }
        }

        public string Crosstab(Context context, long id = 0)
        {
            if (!context.Ajax)
            {
                var log = new SysLogModel(context: context);
                var html = new ItemModel(context: context, referenceId: id).Crosstab(context: context);
                log.Finish(context: context, responseSize: html.Length);
                return html;
            }
            else
            {
                var log = new SysLogModel(context: context);
                var json = new ItemModel(context: context, referenceId: id).CrosstabJson(context: context);
                log.Finish(context: context, responseSize: json.Length);
                return json;
            }
        }

        public string Gantt(Context context, long id = 0)
        {
            if (!context.Ajax)
            {
                var log = new SysLogModel(context: context);
                var html = new ItemModel(context: context, referenceId: id).Gantt(context: context);
                log.Finish(context: context, responseSize: html.Length);
                return html;
            }
            else
            {
                var log = new SysLogModel(context: context);
                var json = new ItemModel(context: context, referenceId: id).GanttJson(context: context);
                log.Finish(context: context, responseSize: json.Length);
                return json;
            }
        }

        public string BurnDown(Context context, long id = 0)
        {
            if (!context.Ajax)
            {
                var log = new SysLogModel(context: context);
                var html = new ItemModel(context: context, referenceId: id).BurnDown(context: context);
                log.Finish(context: context, responseSize: html.Length);
                return html;
            }
            else
            {
                var log = new SysLogModel(context: context);
                var json = new ItemModel(context: context, referenceId: id).BurnDownJson(context: context);
                log.Finish(context: context, responseSize: json.Length);
                return json;
            }
        }

        public string TimeSeries(Context context, long id = 0)
        {
            if (!context.Ajax)
            {
                var log = new SysLogModel(context: context);
                var html = new ItemModel(context: context, referenceId: id).TimeSeries(context: context);
                log.Finish(context: context, responseSize: html.Length);
                return html;
            }
            else
            {
                var log = new SysLogModel(context: context);
                var json = new ItemModel(context: context, referenceId: id).TimeSeriesJson(context: context);
                log.Finish(context: context, responseSize: json.Length);
                return json;
            }
        }

        public string Kamban(Context context, long id = 0)
        {
            if (!context.Ajax)
            {
                var log = new SysLogModel(context: context);
                var html = new ItemModel(context: context, referenceId: id).Kamban(context: context);
                log.Finish(context: context, responseSize: html.Length);
                return html;
            }
            else
            {
                var log = new SysLogModel(context: context);
                var json = new ItemModel(context: context, referenceId: id).KambanJson(context: context);
                log.Finish(context: context, responseSize: json.Length);
                return json;
            }
        }

        public string ImageLib(Context context, long id = 0)
        {
            if (!context.Ajax)
            {
                var log = new SysLogModel(context: context);
                var html = new ItemModel(context: context, referenceId: id).ImageLib(context: context);
                log.Finish(context: context, responseSize: html.Length);
                return html;
            }
            else
            {
                var log = new SysLogModel(context: context);
                var json = new ItemModel(context: context, referenceId: id).ImageLibJson(context: context);
                log.Finish(context: context, responseSize: json.Length);
                return json;
            }
        }

        public string New(Context context, long id = 0)
        {
            if (!context.Ajax)
            {
                var log = new SysLogModel(context: context);
                var html = new ItemModel(context: context, referenceId: id).New(context: context);
                log.Finish(context: context, responseSize: html.Length);
                return html;
            }
            else
            {
                var log = new SysLogModel(context: context);
                var json = new ItemModel(context: context, referenceId: id).NewJson(context: context);
                log.Finish(context: context, responseSize: json.Length);
                return json;
            }
        }

        public string NewOnGrid(Context context, long id = 0)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(
                context: context,
                referenceId: id)
                    .NewOnGrid(context: context);
            log.Finish(
                context: context,
                responseSize: json.Length);
            return json;
        }

        public string CancelNewRow(Context context, long id = 0)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(
                context: context,
                referenceId: id)
                    .CancelNewRow(context: context);
            log.Finish(
                context: context,
                responseSize: json.Length);
            return json;
        }

        public string Edit(Context context, long id)
        {
            if (!context.Ajax)
            {
                var log = new SysLogModel(context: context);
                var html = new ItemModel(context: context, referenceId: id).Editor(context: context);
                log.Finish(context: context, responseSize: html.Length);
                return html;
            }
            else
            {
                var log = new SysLogModel(context: context);
                var json = new ItemModel(context: context, referenceId: id).EditorJson(context: context);
                log.Finish(context: context, responseSize: json.Length);
                return json;
            }
        }

        public string LinkTable(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(context: context, referenceId: id).LinkTable(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string Import(Context context, long id, IHttpPostedFile[] file)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(context: context, referenceId: id).Import(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string OpenExportSelectorDialog(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(context: context, referenceId: id).OpenExportSelectorDialog(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string OpenBulkUpdateSelectorDialog(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(context: context, referenceId: id).OpenBulkUpdateSelectorDialog(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string BulkUpdateSelectChanged(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(context: context, referenceId: id).BulkUpdateSelectChanged(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string OpenSetNumericRangeDialog(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(context: context, referenceId: id).OpenSetNumericRangeDialog(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string OpenSetDateRangeDialog(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(context: context, referenceId: id).OpenSetDateRangeDialog(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public FileContentResult Export(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var responseFile = new ItemModel(context: context, referenceId: id).Export(context: context);
            if (responseFile != null)
            {
                log.Finish(context: context, responseSize: responseFile.Length);
                return responseFile.ToFile();
            }
            else
            {
                log.Finish(context: context);
                return null;
            }
        }

        public string ExportAsync(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(context: context, referenceId: id).ExportAsync(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public FileContentResult ExportCrosstab(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var responseFile = new ItemModel(context: context, referenceId: id).ExportCrosstab(context: context);
            if (responseFile != null)
            {
                log.Finish(context: context, responseSize: responseFile.Length);
                return responseFile.ToFile();
            }
            else
            {
                log.Finish(context: context);
                return null;
            }
        }

        public string Search(Context context)
        {
            if (!context.Ajax)
            {
                var log = new SysLogModel(context: context);
                var html = Indexes.Search(context: context);
                log.Finish(context: context, responseSize: html.Length);
                return html;
            }
            else
            {
                var log = new SysLogModel(context: context);
                var json = Indexes.SearchJson(context: context);
                log.Finish(context: context, responseSize: json.Length);
                return json;
            }
        }

        public string SearchDropDown(Context context, long id = 0)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(context: context, referenceId: id).SearchDropDown(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string RelatingDropDown(Context context, long id = 0)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(context: context, referenceId: id).RelatingDropDown(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string SelectSearchDropDown(Context context, long id = 0)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(context: context, referenceId: id).SelectSearchDropDown(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string GridRows(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(
                context: context,
                referenceId: id)
                    .GridRows(context: context);
            log.Finish(
                context: context,
                responseSize: json.Length);
            return json;
        }

        public string EditOnGrid(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(
                context: context,
                referenceId: id)
                    .GridRows(context: context);
            log.Finish(
                context: context,
                responseSize: json.Length);
            return json;
        }

        [HttpPost]
        public string ReloadRow(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(
                context: context,
                referenceId: id)
                    .ReloadRow(context: context);
            log.Finish(
                context: context,
                responseSize: json.Length);
            return json;
        }

        public string CopyRow(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(
                context: context,
                referenceId: id)
                    .CopyRow(context: context);
            log.Finish(
                context: context,
                responseSize: json.Length);
            return json;
        }

        public string TrashBoxGridRows(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(context: context, referenceId: id).TrashBoxGridRows(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string ImageLibNext(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(context: context, referenceId: id).ImageLibNext(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string Create(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(context: context, referenceId: id).Create(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string PreviewTemplate(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = SiteUtilities.PreviewTemplate(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string Templates(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(context: context, referenceId: id).Templates(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string CreateByTemplate(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(context: context, referenceId: id).CreateByTemplate(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string SiteMenu(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(context: context, referenceId: id).SiteMenu(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string Update(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(
                context: context,
                referenceId: id)
                    .Update(context: context);
            log.Finish(
                context: context,
                responseSize: json.Length);
            return json;
        }

        public string BulkUpdate(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(
                context: context,
                referenceId: id)
                    .BulkUpdate(context: context);
            log.Finish(
                context: context,
                responseSize: json.Length);
            return json;
        }

        public string UpdateByGrid(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(
                context: context,
                referenceId: id)
                    .UpdateByGrid(context: context);
            log.Finish(
                context: context,
                responseSize: json.Length);
            return json;
        }

        public string Copy(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(context: context, referenceId: id).Copy(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string MoveTargets(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(context: context, referenceId: id).MoveTargets(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string Move(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(context: context, referenceId: id).Move(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string MoveSiteMenu(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = SiteUtilities.MoveSiteMenu(context: context, id: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string CreateLink(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = SiteUtilities.CreateLink(context: context, id: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string SortSiteMenu(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = SiteUtilities.SortSiteMenu(context: context, siteId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string BulkMove(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(context: context, referenceId: id).BulkMove(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string Delete(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(context: context, referenceId: id).Delete(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string BulkDelete(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(context: context, referenceId: id).BulkDelete(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string DeleteComment(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(context: context, referenceId: id).DeleteComment(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string DeleteHistory(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(context: context, referenceId: id).DeleteHistory(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string PhysicalDelete(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(context: context, referenceId: id).PhysicalDelete(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string Restore(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(context: context, referenceId: id).Restore(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string RestoreFromHistory(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(context: context, referenceId: id).RestoreFromHistory(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string EditSeparateSettings(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(context: context, referenceId: id).EditSeparateSettings(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string Separate(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(context: context, referenceId: id).Separate(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string SetSiteSettings(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new SiteModel(context: context, siteId: id)
                .SetSiteSettings(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpPost]
        public string OpenImportSitePackageDialog(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(
                context: context,
                referenceId: id)
                    .OpenImportSitePackageDialog(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpPost]
        public string ImportSitePackage(Context context, long id, IHttpPostedFile[] file)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(
                context: context,
                referenceId: id)
                    .ImportSitePackage(context: context);
            log.Finish(
                context: context,
                responseSize: json.Length);
            return json;
        }

        [HttpPost]
        public string OpenExportSitePackageDialog(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(
                context: context,
                referenceId: id)
                    .OpenExportSitePackageDialog(context: context);
            log.Finish(
                context: context,
                responseSize: json.Length);
            return json;
        }

        [HttpGet]
        public FileContentResult ExportSitePackage(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var responseFile = new ItemModel(
                context: context,
                referenceId: id)
                    .ExportSitePackage(context: context);
            if (responseFile != null)
            {
                log.Finish(
                    context: context,
                    responseSize: responseFile.Length);
                return responseFile.ToFile();
            }
            else
            {
                log.Finish(context: context);
                return null;
            }
        }

        public string RebuildSearchIndexes(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = Indexes.RebuildSearchIndexes(
                context: context,
                siteModel: new SiteModel(
                    context: context,
                    siteId: id));
            log.Finish(
                context: context,
                responseSize: json.Length);
            return json;
        }

        public string Histories(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(context: context, referenceId: id).Histories(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string History(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(context: context, referenceId: id).History(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string Permissions(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = PermissionUtilities.Permission(context: context, referenceId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string SearchPermissionElements(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = PermissionUtilities.SearchPermissionElements(
                context: context, referenceId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string SetPermissions(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = PermissionUtilities.SetPermissions(context: context, referenceId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string OpenPermissionsDialog(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = PermissionUtilities.OpenPermissionsDialog(
                context: context, referenceId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string PermissionForCreating(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = PermissionUtilities.PermissionForCreating(
                context: context, referenceId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string SetPermissionForCreating(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = PermissionUtilities.SetPermissionForCreating(
                context: context, referenceId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string OpenPermissionForCreatingDialog(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = PermissionUtilities.OpenPermissionForCreatingDialog(
                context: context, referenceId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string ColumnAccessControl(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = PermissionUtilities.ColumnAccessControl(context: context, referenceId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string SetColumnAccessControl(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = PermissionUtilities.SetColumnAccessControl(
                context: context, referenceId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string OpenColumnAccessControlDialog(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = PermissionUtilities.OpenColumnAccessControlDialog(
                context: context, referenceId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string SearchColumnAccessControl(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = PermissionUtilities.SearchColumnAccessControl(
                context: context, referenceId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string BurnDownRecordDetails(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(context: context, referenceId: id).BurnDownRecordDetailsJson(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string UpdateByCalendar(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(context: context, referenceId: id).UpdateByCalendar(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string UpdateByKamban(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(context: context, referenceId: id).UpdateByKamban(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string SynchronizeTitles(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(context: context, referenceId: id).SynchronizeTitles(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string SynchronizeSummaries(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(context: context, referenceId: id).SynchronizeSummaries(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string SynchronizeFormulas(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(context: context, referenceId: id).SynchronizeFormulas(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string LockTable(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(
                context: context,
                referenceId: id)
                    .LockTable(context: context);
            log.Finish(
                context: context,
                responseSize: json.Length);
            return json;
        }

        public string UnlockTable(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(
                context: context,
                referenceId: id)
                    .UnlockTable(context: context);
            log.Finish(
                context: context,
                responseSize: json.Length);
            return json;
        }

        public string ForceUnlockTable(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(
                context: context,
                referenceId: id)
                    .ForceUnlockTable(context: context);
            log.Finish(
                context: context,
                responseSize: json.Length);
            return json;
        }

        public ContentResult Get(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var result = new ItemModel(context: context, referenceId: id)
                .GetByApi(
                    context: context,
                    internalRequest: true);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result;
        }
    }
}
