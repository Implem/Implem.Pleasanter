using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
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

        internal PackagePermissionModel(Context context, SiteModel siteModel, List<long> recordIdList)
        {
            SiteId = siteModel.SiteId;
            recordIdList.Add(SiteId);
            var dataTable = Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectPermissions(
                    column: Rds.PermissionsColumn()
                        .ReferenceId()
                        .DeptId()
                        .GroupId()
                        .UserId()
                        .PermissionType(),
                    where: Rds.PermissionsWhere()
                        .ReferenceId_In(
                            value: recordIdList.Where(o => o != 0),
                            _using: recordIdList.Count() > 0)));
            foreach (DataRow dataRow in dataTable.Rows)
            {
                Permissions.Add(new PermissionShortModel(dataRow));
            }
        }
    }
}