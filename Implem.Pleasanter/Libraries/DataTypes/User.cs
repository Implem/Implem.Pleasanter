using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using System.Data;
namespace Implem.Pleasanter.Libraries.DataTypes
{
    public class User : IConvertable
    {
        public int TenantId;
        public int Id;
        public int DeptId;
        public string FullName;
        public bool TenantAdmin;
        public bool ServiceAdmin;

        public enum UserTypes : int
        {
            System = 1,
            Anonymous = 2
        }

        public User()
        {
        }

        public User(int userId)
        {
            var dataTable = Rds.ExecuteTable(statements:
                Rds.SelectUsers(
                    column: Rds.UsersColumn()
                        .TenantId()
                        .UserId()
                        .DeptId()
                        .FirstName()
                        .LastName()
                        .FirstAndLastNameOrder()
                        .TenantAdmin()
                        .ServiceAdmin(),
                    where: Rds.UsersWhere()
                        .UserId(userId)));
            if (dataTable.Rows.Count == 1)
            {
                Set(dataTable.Rows[0]);
            }
            else
            {
                SetAnonymouse();
            }
        }

        public User(DataRow dataRow)
        {
            Set(dataRow);
        }

        private void Set(DataRow dataRow)
        {
            TenantId = dataRow.Int("TenantId");
            Id = dataRow.Int("UserId");
            DeptId = dataRow.Int("DeptId");
            FullName = Names.FullName(
                (Names.FirstAndLastNameOrders)dataRow["FirstAndLastNameOrder"],
                dataRow.String("FirstName") + " " + dataRow.String("LastName"),
                dataRow.String("LastName") + " " + dataRow.String("FirstName"));
            TenantAdmin = dataRow.Bool("TenantAdmin");
            ServiceAdmin = dataRow.Bool("ServiceAdmin");
        }

        private void SetAnonymouse()
        {
            TenantId = 0;
            Id = UserTypes.Anonymous.ToInt();
            DeptId = 0;
            FullName = UserTypes.Anonymous.ToString();
            TenantAdmin = false;
            ServiceAdmin = false;
        }

        public string ToControl(Column column, Permissions.Types permissionType)
        {
            return Id.ToString();
        }

        public string ToResponse()
        {
            return Id.ToString();
        }

        public HtmlBuilder Td(HtmlBuilder hb, Column column)
        {
            return Id != UserTypes.Anonymous.ToInt()
                ? hb.Td(action: () => hb
                    .HtmlUser(Id))
                : hb.Td(action: () => { });
        }

        public string ToExport(Column column)
        {
            return FullName.ToString();
        }
    }
}