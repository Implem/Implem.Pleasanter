using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Data;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
namespace Implem.Pleasanter.Libraries.DataTypes
{
    [Serializable]
    public class User : IConvertable
    {
        public int TenantId;
        public int Id;
        public int DeptId;
        public string LoginId;
        public string Name;
        public bool TenantManager;
        public bool ServiceManager;
        public bool Disabled;

        public enum UserTypes : int
        {
            System = 1,
            Anonymous = 2
        }

        public User()
        {
        }

        public User(Context context, int userId)
        {
            if (userId != 0 && userId != 2)
            {
                var dataTable = Rds.ExecuteTable(
                    context: context,
                    statements: Rds.SelectUsers(
                        column: Rds.UsersColumn()
                            .TenantId()
                            .UserId()
                            .DeptId()
                            .LoginId()
                            .Name()
                            .TenantManager()
                            .ServiceManager()
                            .Disabled(),
                        where: Rds.UsersWhere()
                            .UserId(userId)));
                if (dataTable.Rows.Count == 1)
                {
                    Set(
                        context: context,
                        dataRow: dataTable.Rows[0]);
                }
                else
                {
                    SetAnonymous();
                }
            }
            else
            {
                SetAnonymous();
            }
        }

        public User(Context context, DataRow dataRow)
        {
            Set(
                context: context,
                dataRow: dataRow);
        }

        private void Set(Context context, DataRow dataRow)
        {
            TenantId = dataRow.Int("TenantId");
            Id = dataRow.Int("UserId");
            DeptId = dataRow.Int("DeptId");
            LoginId = dataRow.String("LoginId");
            Name = dataRow.String("Name");
            TenantManager = dataRow.Bool("TenantManager")
                || Permissions.PrivilegedUsers(loginId: dataRow.String("LoginId"));
            ServiceManager = dataRow.Bool("ServiceManager");
            Disabled = dataRow.Bool("Disabled");
        }

        private void SetAnonymous()
        {
            TenantId = 0;
            Id = UserTypes.Anonymous.ToInt();
            DeptId = 0;
            LoginId = "Anonymous";
            TenantManager = false;
            ServiceManager = false;
        }

        public string ToControl(Context context, SiteSettings ss, Column column)
        {
            return Id.ToString();
        }

        public string ToResponse(Context context, SiteSettings ss, Column column)
        {
            return column.EditorReadOnly != true
                ? Id.ToString()
                : SiteInfo.UserName(
                    context: context,
                    userId: Id);
        }

        public HtmlBuilder Td(HtmlBuilder hb, Context context, Column column)
        {
            return Id != UserTypes.Anonymous.ToInt()
                ? hb.Td(action: () => hb
                    .HtmlUser(
                        context: context,
                        id: Id))
                : hb.Td(action: () => { });
        }

        public string GridText(Context context, Column column)
        {
            return Id != UserTypes.Anonymous.ToInt()
                ? SiteInfo.UserName(
                    context: context,
                    userId: Id)
                : string.Empty;
        }

        public string ToExport(Context context, Column column, ExportColumn exportColumn = null)
        {
            return !Anonymous()
                ? column.ChoicePart(
                    context: context,
                    selectedValue: Id.ToString(),
                    type: exportColumn?.Type ?? ExportColumn.Types.Text)
                : string.Empty;
        }

        public string ToNotice(
            Context context,
            int saved,
            Column column,
            bool updated,
            bool update)
        {
            return Name.ToNoticeLine(
                context: context,
                saved: SiteInfo.User(
                    context: context,
                    userId: saved).Name,
                column: column,
                updated: updated,
                update: update);
        }

        public bool Anonymous()
        {
            return Id == UserTypes.Anonymous.ToInt();
        }

        public bool InitialValue(Context context)
        {
            return Id == 0;
        }
    }
}