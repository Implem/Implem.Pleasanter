using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Migrators.Statuses
{
    public static class Version00_037_010
    {
        public static void Migrate()
        {
            Rds.ExecuteTable(statements: Rds.SelectUsers(
                column: Rds.UsersColumn()
                    .UserId()
                    .Name()
                    .FirstName()
                    .LastName()
                    .FirstAndLastNameOrder()))
                        .AsEnumerable()
                        .Select(o => new
                        {
                            UserId = o["UserId"].ToInt(),
                            Name = o["Name"].ToString(),
                            FirstName = o["FirstName"].ToString(),
                            LastName = o["LastName"].ToString(),
                            FirstAndLastNameOrder = o["FirstAndLastNameOrder"].ToInt()
                        })
                        .Where(o => o.Name == string.Empty)
                        .ForEach(user =>
                            Rds.ExecuteNonQuery(statements:
                                Rds.UpdateUsers(
                                    where: Rds.UsersWhere().UserId(user.UserId),
                                    param: Rds.UsersParam().Name(
                                        user.FirstAndLastNameOrder == 1
                                            ? (user.FirstName + " " + user.LastName).Trim()
                                            : (user.LastName + " " + user.FirstName).Trim()))));
        }
    }
}