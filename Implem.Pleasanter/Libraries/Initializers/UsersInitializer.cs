using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Requests;
using System;
namespace Implem.Pleasanter.Libraries.Initializers
{
    public static class UsersInitializer
    {
        public static void Initialize(Context context)
        {
            if (Repository.ExecuteScalar_int(
                context: context,
                statements: Rds.SelectUsers(
                    column: Rds.UsersColumn().UsersCount())) == 0)
            {
                Create(
                    context: context,
                    tenantId: 1,
                    loginId: "Administrator",
                    name: "Administrator",
                    password: Parameters.Service.DefaultPassword.Sha512Cng(),
                    passwordExpirationTime: new Time(
                        context: context,
                        value: DateTime.Now),
                    tenantManager: true);
                Create(
                    context: context,
                    tenantId: 0,
                    loginId: "Anonymous",
                    name: string.Empty,
                    disabled: true);
            }
        }

        private static void Create(
            Context context, 
            int tenantId,
            string loginId,
            string name,
            bool disabled = false,
            string password = "",
            Time passwordExpirationTime = null,
            bool tenantManager = false)
        {
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.InsertUsers(
                    param: Rds.UsersParam()
                        .TenantId(tenantId)
                        .LoginId(loginId)
                        .Disabled(disabled)
                        .Password(password)
                        .Name(name)
                        .DeptId(0)
                        .FirstAndLastNameOrder(1)
                        .PasswordExpirationTime(
                        context.Sqls.DateTimeValue(
                            value: passwordExpirationTime?.ToString()),
                            _using: passwordExpirationTime != null)
                        .TenantManager(tenantManager)));
        }
    }
}