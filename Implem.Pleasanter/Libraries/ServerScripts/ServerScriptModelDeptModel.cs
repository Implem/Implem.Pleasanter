using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptModelDeptModel
    {
        private readonly Context Context;
        private readonly int TenantId;
        public readonly int DeptId;
        public readonly string DeptCode;
        public readonly string DeptName;
        public readonly string Body;
        public readonly bool Disabled;
        private readonly Dictionary<string, object> Extras;

        public ServerScriptModelDeptModel(
            Context context,
            int tenantId,
            int deptId,
            string deptCode,
            string deptName,
            string body,
            bool disabled,
            Dictionary<string, object> extras)
        {
            Context = context;
            TenantId = tenantId;
            DeptId = deptId;
            DeptCode = deptCode;
            DeptName = deptName;
            Body = body;
            Disabled = disabled;
            Extras = extras;
        }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(ToJson());
        }

        public object ToJson()
        {
            dynamic d = new ExpandoObject();
            var dict = (IDictionary<string, object>)d;
            dict["TenantId"] = TenantId;
            dict["DeptId"] = DeptId;
            dict["DeptCode"] = DeptCode;
            dict["DeptName"] = DeptName;
            dict["Body"] = Body;
            dict["Disabled"] = Disabled;
            ServerScriptUtilities.MergeExtras(dict, Extras);
            return d;
        }

        public List<ServerScriptModelUserModel> GetMembers()
        {
            var dataTable = Repository.ExecuteTable(
                context: Context,
                statements: Rds.SelectUsers(
                    column: User.QueryColumnWithExtras(),
                    where: Rds.UsersWhere()
                        .TenantId(Context.TenantId)
                        .DeptId(DeptId)));
            return dataTable.AsEnumerable()
                .Select(dataRow => new User(context: Context, dataRow: dataRow))
                .Select(u => new ServerScriptModelUserModel(
                    context: Context,
                    tenantId: u.TenantId,
                    userId: u.Id,
                    deptId: u.DeptId,
                    loginId: u.LoginId,
                    name: u.Name,
                    userCode: u.UserCode,
                    tenantManager: u.TenantManager,
                    serviceManager: u.ServiceManager,
                    disabled: u.Disabled,
                    extras: u.Extras))
                .ToList();
        }
    }
}