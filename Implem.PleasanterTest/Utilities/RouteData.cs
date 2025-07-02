using System.Collections.Generic;

namespace Implem.PleasanterTest.Utilities
{
    public static class RouteData
    {
        public static Dictionary<string, string> TenantsEdit()
        {
            return new Dictionary<string, string>()
            {
                { "controller", "tenants" },
                { "action", "edit" },
                { "id", "0" }
            };
        }

        public static Dictionary<string, string> TenantsUpdate()
        {
            return new Dictionary<string, string>()
            {
                { "controller", "tenants" },
                { "action", "update" },
                { "id", "0" }
            };
        }

        public static Dictionary<string, string> TenantsSyncByLdap()
        {
            return new Dictionary<string, string>()
            {
                { "controller", "tenants" },
                { "action", "syncbyldap" },
                { "id", "0" }
            };
        }

        public static Dictionary<string, string> TenantsBGServerScript()
        {
            return new Dictionary<string, string>()
            {
                { "controller", "tenants" },
                { "action", "setbgserverscript" },
                { "id", "0" }
            };
        }

        public static Dictionary<string, string> DeptsIndex()
        {
            return new Dictionary<string, string>()
            {
                { "controller", "depts" },
                { "action", "index" },
                { "id", "0" }
            };
        }
        public static Dictionary<string, string> DeptsNew()
        {
            return new Dictionary<string, string>()
            {
                { "controller", "depts" },
                { "action", "new" },
                { "id", "0" }
            };
        }

        public static Dictionary<string, string> DeptsEdit(int id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "depts" },
                { "action", "edit" },
                { "id", id.ToString() }
            };
        }


        public static Dictionary<string, string> DeptsCreate()
        {
            return new Dictionary<string, string>()
            {
                { "controller", "depts" },
                { "action", "create" },
                { "id", "0" }
            };
        }

        public static Dictionary<string, string> DeptsUpdate(int id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "depts" },
                { "action", "update" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> DeptsDeleteComment(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "depts" },
                { "action", "deletecomment" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> DeptsDelete(int id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "depts" },
                { "action", "delete" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> DeptsHistories(int id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "depts" },
                { "action", "histories" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> DeptsApiGet(int id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "depts" },
                { "action", "get" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> GroupsIndex()
        {
            return new Dictionary<string, string>()
            {
                { "controller", "groups" },
                { "action", "index" },
                { "id", "0" }
            };
        }

        public static Dictionary<string, string> GroupsNew()
        {
            return new Dictionary<string, string>()
            {
                { "controller", "groups" },
                { "action", "new" },
                { "id", "0" }
            };
        }

        public static Dictionary<string, string> GroupsEdit(int id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "groups" },
                { "action", "edit" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> GroupsCreate()
        {
            return new Dictionary<string, string>()
            {
                { "controller", "groups" },
                { "action", "create" },
                { "id", "0" }
            };
        }

        public static Dictionary<string, string> GroupsUpdate(int id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "groups" },
                { "action", "update" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> GroupsDeleteComment(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "groups" },
                { "action", "deletecomment" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> GroupsDelete(int id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "groups" },
                { "action", "delete" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> GroupsBulkDelete(int id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "groups" },
                { "action", "bulkdelete" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> GroupsHistories(int id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "groups" },
                { "action", "histories" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> GroupsImport()
        {
            return new Dictionary<string, string>()
            {
                { "controller", "groups" },
                { "action", "import" },
                { "id", "0" }
            };
        }

        public static Dictionary<string, string> GroupsExport()
        {
            return new Dictionary<string, string>()
            {
                { "controller", "groups" },
                { "action", "histories" },
                { "id", "0" }
            };
        }

        public static Dictionary<string, string> GroupsTrashBox()
        {
            return new Dictionary<string, string>()
            {
                { "controller", "groups" },
                { "action", "trashbox" },
                { "id", "0" }
            };
        }

        public static Dictionary<string, string> GroupsApiGet(int id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "groups" },
                { "action", "get" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> GroupsApiCreate()
        {
            return new Dictionary<string, string>()
            {
                { "controller", "groups" },
                { "action", "create" },
                { "id", "0" }
            };
        }

        public static Dictionary<string, string> GroupsApiUpdate(int id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "groups" },
                { "action", "update" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> GroupsApiDelete(int id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "groups" },
                { "action", "delete" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> UsersIndex()
        {
            return new Dictionary<string, string>()
            {
                { "controller", "users" },
                { "action", "index" },
                { "id", "0" }
            };
        }

        public static Dictionary<string, string> UsersNew()
        {
            return new Dictionary<string, string>()
            {
                { "controller", "users" },
                { "action", "new" },
                { "id", "0" }
            };
        }

        public static Dictionary<string, string> UsersEdit(int id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "users" },
                { "action", "edit" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> UsersCreate()
        {
            return new Dictionary<string, string>()
            {
                { "controller", "users" },
                { "action", "create" },
                { "id", "0" }
            };
        }

        public static Dictionary<string, string> UsersUpdate(int id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "users" },
                { "action", "update" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> UsersDeleteComment(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "users" },
                { "action", "deletecomment" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> UsersDelete(int id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "users" },
                { "action", "delete" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> UsersHistories(int id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "users" },
                { "action", "histories" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> UsersResetPassword(int id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "users" },
                { "action", "resetpassword" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> UsersApiGet(int id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "users" },
                { "action", "get" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> UsersApiCreate()
        {
            return new Dictionary<string, string>()
            {
                { "controller", "users" },
                { "action", "create" },
                { "id", "0" }
            };
        }

        public static Dictionary<string, string> UsersApiUpdate(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "users" },
                { "action", "update" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> UsersApiDelete(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "users" },
                { "action", "delete" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> BinariesSiteImageThumbnail(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "binaries" },
                { "action", "siteimagethumbnail" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> BinariesSiteImageIcon(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "binaries" },
                { "action", "siteimageicon" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> BinariesTenantImageLogo(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "binaries" },
                { "action", "tenantimagelogo" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> BinariesUpdateSiteImage(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "binaries" },
                { "action", "updatesiteimage" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> BinariesUpdateTenantImage(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "binaries" },
                { "action", "updatetenantimage" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> BinariesDeleteSiteImage(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "binaries" },
                { "action", "deletesiteimage" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> BinariesDeleteTenantImage(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "binaries" },
                { "action", "deletetenantimage" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> BinariesUploadImage(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "binaries" },
                { "action", "uploadimage" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> BinariesDeleteImage(string guid)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "binaries" },
                { "action", "deleteimage" },
                { "id", guid }
            };
        }

        public static Dictionary<string, string> BinariesDownload(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "binaries" },
                { "action", "download" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> BinariesDownloadTemp(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "binaries" },
                { "action", "downloadtemp" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> BinariesShow(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "binaries" },
                { "action", "show" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> BinariesShowTemp(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "binaries" },
                { "action", "showtemp" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> BinariesDeleteTemp(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "binaries" },
                { "action", "deletetemp" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> BinariesUpload(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "binaries" },
                { "action", "upload" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsIndex(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "index" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsTrashBox(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "trashbox" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsCalendar(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "calendar" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsCrosstab(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "crosstab" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsGantt(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "gantt" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsBurnDown(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "burndown" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsTimeSeries(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "timeseries" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsKamban(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "kamban" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsImageLib(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "imagelib" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsNew(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "new" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsNewOnGrid(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "newongrid" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsCancelNewRow(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "cancelnewrow" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsEdit(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "edit" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsSelectedIds(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "selectedids" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsLinkTable(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "linktable" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsImport(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "import" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsOpenExportSelectorDialog(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "openexportselectordialog" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsOpenBulkUpdateSelectorDialog(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "openbulkupdateselectordialog" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsBulkUpdateSelectChanged(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "bulkupdateselectchanged" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsOpenSetNumericRangeDialog(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "opensetnumericrangedialog" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsOpenSetDateRangeDialog(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "opensetdaterangedialog" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsExport(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "export" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsExportAndMailNotify(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "exportandmailnotify" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsExportCrosstab(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "exportcrosstab" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsSearch()
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "search" },
                { "id", "0" }
            };
        }

        public static Dictionary<string, string> ItemsSearchDropDown(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "searchdropdown" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsRelatingDropDown(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "relatingdropdown" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsSelectSearchDropDown(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "selectsearchdropdown" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsGridRows(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "gridrows" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsReloadRow(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "reloadrow" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsCopyRow(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "copyrow" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsTrashBoxGridRows(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "trashboxgridrows" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsImageLibNext(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "imagelibnext" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsCreate(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "create" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsPreviewTemplate(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "previewtemplate" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsTemplates(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "templates" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsCreateByTemplate(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "createbytemplate" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsSiteMenu(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "sitemenu" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsUpdate(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "update" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsBulkUpdate(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "bulkupdate" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsUpdateByGrid(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "updatebygrid" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsCopy(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "copy" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsMoveTargets(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "movetargets" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsMove(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "move" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsMoveSiteMenu(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "movesitemenu" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsCreateLink(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "createlink" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsSortSiteMenu(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "sortsitemenu" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsBulkMove(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "bulkmove" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsDelete(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "delete" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsBulkDelete(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "bulkdelete" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsDeleteComment(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "deletecomment" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsDeleteHistory(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "deletehistory" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsPhysicalDelete(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "physicaldelete" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsRestore(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "restore" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsRestoreFromHistory(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "restorefromhistory" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsEditSeparateSettings(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "editseparatesettings" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsSeparate(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "separate" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsSetSiteSettings(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "setsitesettings" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsOpenImportSitePackageDialog(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "openimportsitepackagedialog" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsImportSitePackage(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "importsitepackage" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsOpenExportSitePackageDialog(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "openexportsitepackagedialog" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsExportSitePackage(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "exportsitepackage" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsRebuildSearchIndexes(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "rebuildsearchindexes" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsHistories(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "histories" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsHistory(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "history" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsPermissions(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "permissions" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsSearchPermissionElements(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "searchpermissionelements" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsSetPermissions(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "setpermissions" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsOpenPermissionsDialog(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "openpermissionsdialog" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsPermissionForRecord(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "permissionforrecord" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsSetPermissionForCreating(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "setpermissionforcreating" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsOpenPermissionForCreatingDialog(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "openpermissionforcreatingdialog" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsSetPermissionForUpdating(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "setpermissionforupdating" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsOpenPermissionForUpdatingDialog(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "openpermissionforupdatingdialog" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsColumnAccessControl(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "columnaccesscontrol" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsSetColumnAccessControl(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "setcolumnaccesscontrol" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsOpenColumnAccessControlDialog(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "opencolumnaccesscontroldialog" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsSearchColumnAccessControl(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "searchcolumnaccesscontrol" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsBurnDownRecordDetails(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "burndownrecorddetails" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsUpdateByCalendar(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "updatebycalendar" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsUpdateByKamban(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "updatebykamban" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsSynchronizeTitles(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "synchronizetitles" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> itemsSynchronizeSummaries(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "synchronizesummaries" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsSynchronizeFormulas(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "synchronizeformulas" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsLockTable(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "locktable" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsUnlockTable(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "unlocktable" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsForceUnlockTable(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "forceunlocktable" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsUnlockRecord(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "unlockrecord" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsApiGet(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "get" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsApiUpdate(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "update" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsApiCreate(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "create" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> ItemsApiCreateSite(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "createsite" },
                { "id", id.ToString() }
            };
        }


        public static Dictionary<string, string> ItemsApiDelete(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "delete" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> VersionsIndex()
        {
            return new Dictionary<string, string>()
            {
                { "controller", "versions" },
                { "action", "index" },
                { "id", "0" }
            };
        }

        public static Dictionary<string, string> PublishesIndex(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "publishes" },
                { "action", "index" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> PublishesOpenSetNumericRangeDialog(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "publishes" },
                { "action", "opensetnumericrangedialog" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> PublishesOpenSetDateRangeDialog(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "publishes" },
                { "action", "opensetdaterangedialog" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> PublishesSearchDropDown(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "publishes" },
                { "action", "searchdropdown" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> PublishesSelectSearchDropDown(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "publishes" },
                { "action", "selectsearchdropdown" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> PublishesGridRows(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "publishes" },
                { "action", "gridrows" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> PublishesEdit(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "publishes" },
                { "action", "edit" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> PublishBinariesSiteImageThumbnail(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "publishbinaries" },
                { "action", "siteimagethumbnail" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> PublishBinariesSiteImageIcon(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "publishbinaries" },
                { "action", "siteimageicon" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> PublishBinariesDownload(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "publishbinaries" },
                { "action", "download" },
                { "id", id.ToString() }
            };
        }

        public static Dictionary<string, string> PublishBinariesShow(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "publishbinaries" },
                { "action", "show" },
                { "id", id.ToString() }
            };
        }
    }
}
