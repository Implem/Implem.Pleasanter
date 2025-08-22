using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
namespace Implem.Pleasanter.Libraries.DataTypes
{
    [Serializable]
    public class User : IConvertable
    {
        public int TenantId;
        public int Id;
        public int DeptId;
        public Dept Dept;
        public string LoginId;
        public string Name;
        public string UserCode;
        public string Body;
        public bool TenantManager;
        public bool ServiceManager;
        public bool AllowCreationAtTopSite;
        public bool AllowGroupAdministration;
        public bool AllowGroupCreation;
        public bool AllowApi;
        public bool AllowMovingFromTopSite;
        public bool Disabled;

        public User()
        {
        }

        public User(Context context, int userId)
        {
            if (userId != 0 && userId != SiteInfo.AnonymousId)
            {
                var dataTable = Repository.ExecuteTable(
                    context: context,
                    statements: Rds.SelectUsers(
                        column: Rds.UsersColumn()
                            .TenantId()
                            .UserId()
                            .DeptId()
                            .LoginId()
                            .Name()
                            .UserCode()
                            .Body()
                            .UserSettings()
                            .TenantManager()
                            .ServiceManager()
                            .AllowCreationAtTopSite()
                            .AllowGroupAdministration()
                            .AllowGroupCreation()
                            .AllowApi()
                            .AllowMovingFromTopSite()
                            .Disabled(),
                        where: Rds.UsersWhere()
                            .UserId(userId)));
                if (dataTable.Rows.Count == 1)
                {
                    Set(dataRow: dataTable.Rows[0]);
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
            Set(dataRow: dataRow);
        }

        private void Set(DataRow dataRow)
        {
            TenantId = dataRow.Int("TenantId");
            Id = dataRow.Int("UserId");
            DeptId = dataRow.Int("DeptId");
            Dept = SiteInfo.Dept(
                tenantId: TenantId,
                deptId: DeptId);
            LoginId = dataRow.String("LoginId");
            Name = Strings.CoalesceEmpty(dataRow.String("Name"), LoginId);
            UserCode = dataRow.String("UserCode");
            Body = dataRow.String("Body");
            TenantManager = dataRow.Bool("TenantManager")
                || Permissions.PrivilegedUsers(loginId: dataRow.String("LoginId"));
            ServiceManager = dataRow.Bool("ServiceManager");
            AllowCreationAtTopSite = dataRow.Bool("AllowCreationAtTopSite");
            AllowGroupAdministration = dataRow.Bool("AllowGroupAdministration");
            AllowGroupCreation = dataRow.Bool("AllowGroupCreation");
            AllowApi = dataRow.Bool("AllowApi");
            AllowMovingFromTopSite = dataRow.Bool("AllowMovingFromTopSite");
            Disabled = dataRow.Bool("Disabled");
        }

        private void SetAnonymous()
        {
            TenantId = 0;
            Id = 0;
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
            return !column.GetEditorReadOnly()
                ? Id.ToString()
                : SiteInfo.UserName(
                    context: context,
                    userId: Id);
        }

        public string ToDisplay(Context context, SiteSettings ss, Column column)
        {
            return SiteInfo.UserName(
                context: context,
                userId: Id);
        }

        public string ToLookup(Context context, SiteSettings ss, Column column, Lookup.Types? type)
        {
            switch (type)
            {
                case Lookup.Types.DisplayName:
                    return ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                default:
                    return Id.ToString();
            }
        }

        public HtmlBuilder Td(
            HtmlBuilder hb,
            Context context,
            Column column,
            int? tabIndex,
            ServerScriptModelColumn serverScriptModelColumn)
        {
            return !Anonymous()
                ? hb.Td(
                    css: column.CellCss(serverScriptModelColumn?.ExtendedCellCss),
                    attributes: new HtmlAttributes()
                        .DataCellSticky(column.CellSticky)
                        .DataCellWidth(column.CellWidth),
                    action: () => hb
                        .HtmlUser(
                            context: context,
                            text: column.ChoiceHash.Get(Id.ToString())?.Text
                                ?? SiteInfo.UserName(
                                    context: context,
                                    userId: Id)))
                : hb.Td(
                    css: column.CellCss(serverScriptModelColumn?.ExtendedCellCss),
                    attributes: new HtmlAttributes()
                        .DataCellSticky(column.CellSticky)
                        .DataCellWidth(column.CellWidth),
                    action: () => { });
        }

        public string GridText(Context context, Column column)
        {
            return !Anonymous()
                ? SiteInfo.UserName(
                    context: context,
                    userId: Id)
                : string.Empty;
        }

        public string SelectableText(Context context, string format)
        {
            var value = format;
            foreach (Match match in format.RegexMatches("\\[[A-Za-z]+?\\]"))
            {
                switch (match.Value)
                {
                    case "[User]":
                        value = value.Replace(match.Value, Displays.Users(context: context));
                        break;
                    case "[UserId]":
                        value = value.Replace(match.Value, Id > 0 && !Anonymous()
                            ? Id.ToString()
                            : string.Empty);
                        break;
                    case "[DeptId]":
                        value = value.Replace(match.Value, DeptId > 0
                            ? DeptId.ToString()
                            : string.Empty);
                        break;
                    case "[Dept]":
                    case "[DeptName]":
                        value = value.Replace(match.Value, Dept?.Name);
                        break;
                    case "[DeptCode]":
                        value = value.Replace(match.Value, Dept?.Code);
                        break;
                    case "[LoginId]":
                        value = value.Replace(match.Value, LoginId);
                        break;
                    case "[Name]":
                        value = value.Replace(match.Value, Name);
                        break;
                    case "[UserCode]":
                        value = value.Replace(match.Value, UserCode);
                        break;
                    case "[Body]":
                        value = value.Replace(match.Value, Body);
                        break;
                }
            }
            return value;
        }

        public string Tooltip(Context context)
        {
            var mailAddress = Parameters.User.IsMailAddressSelectorToolTip()
                ? MailAddressUtilities.Get(
                    context: context,
                    userId: Id)
                : string.Empty;
            var list = new List<string>()
            {
                Strings.CoalesceEmpty(mailAddress, LoginId),
                UserCode,
                Body
            };
            return list.Where(o => !o.IsNullOrEmpty()).Join(" ");
        }

        public object ToApiDisplayValue(Context context, SiteSettings ss, Column column)
        {
            var name = SiteInfo.Name(
                context: context,
                id: Id,
                type: Column.Types.User);
            return !name.IsNullOrEmpty()
                ? name
                : null;
        }

        public object ToApiValue(Context context, SiteSettings ss, Column column)
        {
            return Id;
        }

        public string ToExport(Context context, Column column, ExportColumn exportColumn = null)
        {
            return !Anonymous()
                ? column.ChoiceParts(
                    context: context,
                    selectedValues: Id.ToString(),
                    type: exportColumn?.Type ?? ExportColumn.Types.Text)
                        .FirstOrDefault()
                : string.Empty;
        }

        public string ToNotice(
            Context context,
            int saved,
            Column column,
            NotificationColumnFormat notificationColumnFormat,
            bool updated,
            bool update)
        {
            return notificationColumnFormat.DisplayText(
                self: Name,
                saved: SiteInfo.User(
                    context: context,
                    userId: saved).Name,
                column: column,
                updated: updated,
                update: update);
        }

        public bool InitialValue(Context context)
        {
            return Id == 0;
        }

        public bool Anonymous()
        {
            return Id == 0
                || Id == SiteInfo.AnonymousId;
        }
    }
}