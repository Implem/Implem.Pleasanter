using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Converts;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System.Data;
namespace Implem.Pleasanter.Libraries.DataTypes
{
    public class User : IConvertable
    {
        public int TenantId;
        public int Id;
        public int DeptId;
        public string Name;
        public bool TenantManager;
        public bool ServiceManager;

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
            if (userId != 0 && userId != 2)
            {
                var dataTable = Rds.ExecuteTable(statements:
                    Rds.SelectUsers(
                        column: Rds.UsersColumn()
                            .TenantId()
                            .UserId()
                            .DeptId()
                            .Name()
                            .TenantManager()
                            .ServiceManager(),
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
            Name = dataRow.String("Name");
            TenantManager = dataRow.Bool("TenantManager");
            ServiceManager = dataRow.Bool("ServiceManager");
        }

        private void SetAnonymouse()
        {
            TenantId = 0;
            Id = UserTypes.Anonymous.ToInt();
            DeptId = 0;
            TenantManager = false;
            ServiceManager = false;
        }

        public string ToControl(SiteSettings ss, Column column)
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

        public string GridText(Column column)
        {
            return Id != UserTypes.Anonymous.ToInt()
                ? SiteInfo.UserName(Id)
                : string.Empty;
        }

        public string ToExport(Column column)
        {
            return Name.ToStr();
        }

        public string ToNotice(
            int saved,
            Column column,
            bool updated,
            bool update)
        {
            return Name.ToNoticeLine(
                SiteInfo.User(saved).Name,
                column,
                updated,
                update);
        }

        public bool Anonymous()
        {
            return Id == UserTypes.Anonymous.ToInt();
        }
    }
}