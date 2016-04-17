using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Utilities;
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
                    "Anonymouse",
                    "Anonymouse",
                    disabled: true);
                Create(
                    "Administrator",
                    "System Admin",
                    password: Securities.DefaultAdminPassword().Sha512Cng());
            }
        }

        private static void Create(
            string loginId, string name, bool disabled = false, string password = "")
        {
            Rds.ExecuteNonQuery(statements:
                Rds.InsertUsers(
                    param: Rds.UsersParam()
                        .TenantId(1)
                        .LoginId(loginId)
                        .Disabled(disabled)
                        .Password(password)
                        .LastName(string.Empty)
                        .FirstName(name)
                        .Language("en")
                        .DeptId(0)
                        .FirstAndLastNameOrder(1)));
        }
    }
}