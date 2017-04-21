using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using System;
namespace Implem.Pleasanter.Libraries.Initializers
{
    public static class UsersInitializer
    {
        public static void Initialize()
        {
            if (Rds.ExecuteScalar_int(statements:
                Rds.SelectUsers(column: Rds.UsersColumn().UsersCount())) == 0)
            {
                Create(
                    1,
                    "Administrator",
                    "Administrator",
                    password: Parameters.Service.DefaultPassword.Sha512Cng(),
                    passwordExpirationTime: new Time(DateTime.Now),
                    tenantManager: true);
                Create(
                    0,
                    "Anonymouse",
                    string.Empty,
                    disabled: true);
            }
        }

        private static void Create(
            int tenantId,
            string loginId,
            string name,
            bool disabled = false,
            string password = "",
            Time passwordExpirationTime = null,
            bool tenantManager = false)
        {
            Rds.ExecuteNonQuery(statements:
                Rds.InsertUsers(
                    param: Rds.UsersParam()
                        .TenantId(tenantId)
                        .LoginId(loginId)
                        .Disabled(disabled)
                        .Password(password)
                        .Name(name)
                        .DeptId(0)
                        .FirstAndLastNameOrder(1)
                        .PasswordExpirationTime(
                            passwordExpirationTime?.ToString(),
                            _using: passwordExpirationTime != null)
                        .TenantManager(tenantManager)));
        }
    }
}