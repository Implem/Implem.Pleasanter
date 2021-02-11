using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptModelDeptModel
    {
        private readonly Context Context;
        private readonly int TenantId;
        public readonly int DeptId;
        public readonly string DeptCode;
        public readonly string DeptName;

        public ServerScriptModelDeptModel(
            Context context,
            int tenantId,
            int deptId,
            string deptCode,
            string deptName)
        {
            Context = context;
            TenantId = tenantId;
            DeptId = deptId;
            DeptCode = deptCode;
            DeptName = deptName;
        }

        public List<ServerScriptModelUserModel> GetMembers()
        {
            var users = new List<ServerScriptModelUserModel>();
            DeptUtilities.Users(
                context: Context,
                deptId: DeptId)
                    .ForEach(dataRow =>
                        users.Add(new ServerScriptModelUserModel(
                            context: Context,
                            tenantId: dataRow.Int("DeptId"),
                            userId: dataRow.Int("UserId"),
                            deptId: dataRow.Int("DeptId"),
                            loginId: dataRow.String("LoginId"),
                            name: dataRow.String("Name"),
                            userCode: dataRow.String("UserCode"),
                            tenantManager: dataRow.Bool("TenantManager"),
                            serviceManager: dataRow.Bool("ServiceManager"),
                            disabled: dataRow.Bool("Disabled"))));
            return users;
        }
    }
}