using Implem.Libraries.DataSources.SqlServer;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.SitePackages
{
    public class PackagePermissionModel
    {
        public long SiteId = 0;
        public List<PermissionShortModel> Permissions = new List<PermissionShortModel>();

        internal PackagePermissionModel()
        {
        }

        internal PackagePermissionModel(Context context, SiteModel siteModel, View view)
        {
            SiteId = siteModel.SiteId;
            var dataTable = Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectPermissions(
                column: Rds.PermissionsColumn()
                    .ReferenceId()
                    .DeptId()
                    .GroupId()
                    .UserId()
                    .PermissionType(),
                where: Rds.PermissionsWhere().Or(or:
                    Rds.PermissionsWhere()
                        .ReferenceId(SiteId)
                        .ReferenceId_In(sub: Rds.Select(
                            tableName: siteModel.ReferenceType,
                            column: new SqlColumnCollection()
                            {
                                Rds.IdColumn(siteModel.ReferenceType)
                            },
                            where: view.Where(
                                context: context,
                                ss: siteModel.SiteSettings))))));
            foreach (DataRow dataRow in dataTable.Rows)
            {
                Permissions.Add(new PermissionShortModel(dataRow));
            }
        }
    }
}