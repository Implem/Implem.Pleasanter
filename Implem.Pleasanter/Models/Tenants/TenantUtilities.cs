using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.Interfaces;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Resources;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
namespace Implem.Pleasanter.Models
{
    public static class TenantUtilities
    {
        public static HtmlBuilder TdValue(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Column column,
            TenantModel tenantModel,
            int? tabIndex = null,
            ServerScriptModelColumn serverScriptModelColumn = null)
        {
            if (serverScriptModelColumn?.Hide == true)
            {
                return hb.Td();
            }
            if (serverScriptModelColumn?.RawText.IsNullOrEmpty() == false)
            {
                return hb.Td(
                    context: context,
                    column: column,
                    action: () => hb.Raw(serverScriptModelColumn?.RawText),
                    tabIndex: tabIndex,
                    serverScriptModelColumn: serverScriptModelColumn);
            }
            else if (!column.GridDesign.IsNullOrEmpty())
            {
                return hb.TdCustomValue(
                    context: context,
                    ss: ss,
                    gridDesign: column.GridDesign,
                    css: column.CellCss(serverScriptModelColumn?.ExtendedCellCss),
                    tenantModel: tenantModel);
            }
            else
            {
                var mine = tenantModel.Mine(context: context);
                switch (column.Name)
                {
                    case "Ver":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: tenantModel.Ver,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "Comments":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: tenantModel.Comments,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "Creator":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: tenantModel.Creator,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "Updator":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: tenantModel.Updator,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "CreatedTime":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: tenantModel.CreatedTime,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "UpdatedTime":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: tenantModel.UpdatedTime,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    default:
                        switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                        {
                            case "Class":
                                return ss.ReadColumnAccessControls.Allowed(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    mine: mine)
                                        ? hb.Td(
                                            context: context,
                                            column: column,
                                            value: tenantModel.GetClass(columnName: column.Name),
                                            tabIndex: tabIndex,
                                            serverScriptModelColumn: serverScriptModelColumn)
                                        : hb.Td(
                                            context: context,
                                            column: column,
                                            value: string.Empty,
                                            tabIndex: tabIndex,
                                            serverScriptModelColumn: serverScriptModelColumn);
                            case "Num":
                                return ss.ReadColumnAccessControls.Allowed(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    mine: mine)
                                        ? hb.Td(
                                            context: context,
                                            column: column,
                                            value: tenantModel.GetNum(columnName: column.Name),
                                            tabIndex: tabIndex,
                                            serverScriptModelColumn: serverScriptModelColumn)
                                        : hb.Td(
                                            context: context,
                                            column: column,
                                            value: string.Empty,
                                            tabIndex: tabIndex,
                                            serverScriptModelColumn: serverScriptModelColumn);
                            case "Date":
                                return ss.ReadColumnAccessControls.Allowed(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    mine: mine)
                                        ? hb.Td(
                                            context: context,
                                            column: column,
                                            value: tenantModel.GetDate(columnName: column.Name),
                                            tabIndex: tabIndex,
                                            serverScriptModelColumn: serverScriptModelColumn)
                                        : hb.Td(
                                            context: context,
                                            column: column,
                                            value: string.Empty,
                                            tabIndex: tabIndex,
                                            serverScriptModelColumn: serverScriptModelColumn);
                            case "Description":
                                return ss.ReadColumnAccessControls.Allowed(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    mine: mine)
                                        ? hb.Td(
                                            context: context,
                                            column: column,
                                            value: tenantModel.GetDescription(columnName: column.Name),
                                            tabIndex: tabIndex,
                                            serverScriptModelColumn: serverScriptModelColumn)
                                        : hb.Td(
                                            context: context,
                                            column: column,
                                            value: string.Empty,
                                            tabIndex: tabIndex,
                                            serverScriptModelColumn: serverScriptModelColumn);
                            case "Check":
                                return ss.ReadColumnAccessControls.Allowed(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    mine: mine)
                                        ? hb.Td(
                                            context: context,
                                            column: column,
                                            value: tenantModel.GetCheck(columnName: column.Name),
                                            tabIndex: tabIndex,
                                            serverScriptModelColumn: serverScriptModelColumn)
                                        : hb.Td(
                                            context: context,
                                            column: column,
                                            value: string.Empty,
                                            tabIndex: tabIndex,
                                            serverScriptModelColumn: serverScriptModelColumn);
                            case "Attachments":
                                return ss.ReadColumnAccessControls.Allowed(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    mine: mine)
                                        ? hb.Td(
                                            context: context,
                                            column: column,
                                            value: tenantModel.GetAttachments(columnName: column.Name),
                                            tabIndex: tabIndex,
                                            serverScriptModelColumn: serverScriptModelColumn)
                                        : hb.Td(
                                            context: context,
                                            column: column,
                                            value: string.Empty,
                                            tabIndex: tabIndex,
                                            serverScriptModelColumn: serverScriptModelColumn);
                            default:
                                return hb;
                        }
                }
            }
        }

        private static HtmlBuilder TdCustomValue(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            string gridDesign,
            string css,
            TenantModel tenantModel)
        {
            ss.IncludedColumns(gridDesign).ForEach(column =>
            {
                var value = string.Empty;
                switch (column.Name)
                {
                    case "Ver": value = tenantModel.Ver.GridText(
                        context: context,
                        column: column); break;
                    case "Comments": value = tenantModel.Comments.GridText(
                        context: context,
                        column: column); break;
                    case "Creator": value = tenantModel.Creator.GridText(
                        context: context,
                        column: column); break;
                    case "Updator": value = tenantModel.Updator.GridText(
                        context: context,
                        column: column); break;
                    case "CreatedTime": value = tenantModel.CreatedTime.GridText(
                        context: context,
                        column: column); break;
                    case "UpdatedTime": value = tenantModel.UpdatedTime.GridText(
                        context: context,
                        column: column); break;
                    default:
                        switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                        {
                            case "Class":
                                value = tenantModel.GetClass(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Num":
                                value = tenantModel.GetNum(columnName: column.Name)?.Value?.GridText(
                                    context: context,
                                    column: column) ?? string.Empty;
                                break;
                            case "Date":
                                value = tenantModel.GetDate(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Description":
                                value = tenantModel.GetDescription(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Check":
                                value = tenantModel.GetCheck(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Attachments":
                                value = tenantModel.GetAttachments(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                        }
                        break;
                }
                gridDesign = gridDesign.Replace("[" + column.ColumnName + "]", value);
            });
            return hb.Td(
                css: css,
                action: () => hb
                    .Div(
                        css: "markup",
                        action: () => hb
                            .Text(text: gridDesign)));
        }

        public static string EditorNew(Context context, SiteSettings ss)
        {
            return Editor(context: context, ss: ss, tenantModel: new TenantModel(
                context: context,
                ss: SiteSettingsUtilities.TenantsSiteSettings(context: context),
                methodType: BaseModel.MethodTypes.New));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Editor(
            Context context, SiteSettings ss, int tenantId, bool clearSessions)
        {
            var tenantModel = new TenantModel(
                context: context,
                ss: SiteSettingsUtilities.TenantsSiteSettings(context: context),
                tenantId: tenantId,
                clearSessions: clearSessions,
                methodType: BaseModel.MethodTypes.Edit);
            if (tenantModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                Repository.ExecuteNonQuery(
                    context: context,
                    connectionString: Parameters.Rds.OwnerConnectionString,
                    statements: new[] {
                        Rds.IdentityInsertTenants(factory: context, on: true),
                        Rds.InsertTenants(
                            param: Rds.TenantsParam()
                                .TenantId(tenantId)
                                .TenantName("DefaultTenant")),
                        Rds.IdentityInsertTenants(factory: context, on: false)
                    });
                tenantModel.Get(context, ss);
            }
            tenantModel.SwitchTargets = GetSwitchTargets(
                context: context,
                ss: SiteSettingsUtilities.TenantsSiteSettings(context: context),
                tenantId: tenantId);
            return Editor(context: context, ss: ss, tenantModel: tenantModel);
        }

        public static string Editor(
            Context context, SiteSettings ss, TenantModel tenantModel)
        {
            var invalid = TenantValidators.OnEditing(
                context: context,
                ss: ss,
                tenantModel: tenantModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return HtmlTemplates.Error(
                    context: context,
                    errorData: invalid);
            }
            var hb = new HtmlBuilder();
            return hb.Template(
                context: context,
                ss: ss,
                view: null,
                referenceType: "Tenants",
                title: tenantModel.MethodType == BaseModel.MethodTypes.New
                    ? Displays.Tenants(context: context) + " - " + Displays.New(context: context)
                    : tenantModel.Title.MessageDisplay(context: context),
                action: () => hb
                    .Editor(
                        context: context,
                        ss: ss,
                        tenantModel: tenantModel)).ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Editor(
            this HtmlBuilder hb, Context context, SiteSettings ss, TenantModel tenantModel)
        {
            var commentsColumn = ss.GetColumn(context: context, columnName: "Comments");
            var commentsColumnPermissionType = Permissions.ColumnPermissionType(
                context: context,
                ss: ss,
                column: commentsColumn,
                baseModel: tenantModel);
            var showComments = false;
            var tabsCss = showComments ? null : "max";
            return hb.Div(id: "Editor", action: () => hb
                .Form(
                    attributes: new HtmlAttributes()
                        .Id("TenantForm")
                        .Class("main-form confirm-unload")
                        .Action(Locations.Action(
                            context: context,
                            controller: "Tenants",
                            id: tenantModel.TenantId)),
                    action: () => hb
                        .RecordHeader(
                            context: context,
                            ss: ss,
                            baseModel: tenantModel,
                            tableName: "Tenants")
                        .Div(
                            id: "EditorComments", action: () => hb
                                .Comments(
                                    context: context,
                                    ss: ss,
                                    comments: tenantModel.Comments,
                                    column: commentsColumn,
                                    verType: tenantModel.VerType,
                                    columnPermissionType: commentsColumnPermissionType),
                            _using: showComments)
                        .Div(
                            id: "EditorTabsContainer",
                            css: "tab-container " + tabsCss,
                            action: () => hb
                                .EditorTabs(
                                    context: context,
                                    tenantModel: tenantModel)
                                .FieldSetGeneral(context: context, ss: ss, tenantModel: tenantModel)
                                .FieldSet(
                                    attributes: new HtmlAttributes()
                                        .Id("FieldSetHistories")
                                        .DataAction("Histories")
                                        .DataMethod("post"),
                                    _using: tenantModel.MethodType != BaseModel.MethodTypes.New)
                                .MainCommands(
                                    context: context,
                                    ss: ss,
                                    verType: tenantModel.VerType,
                                    updateButton: true,
                                    mailButton: true,
                                    deleteButton: true,
                                    extensions: () => hb
                                        .MainCommandExtensions(
                                            context: context,
                                            tenantModel: tenantModel,
                                            ss: ss)))
                        .Hidden(
                            controlId: "BaseUrl",
                            value: Locations.BaseUrl(context: context))
                        .Hidden(
                            controlId: "Ver",
                            value: tenantModel.Ver.ToString())
                        .Hidden(
                            controlId: "MethodType",
                            value: tenantModel.MethodType.ToString().ToLower())
                        .Hidden(
                            controlId: "Tenants_Timestamp",
                            css: "always-send",
                            value: tenantModel.Timestamp)
                        .Hidden(
                            controlId: "SwitchTargets",
                            css: "always-send",
                            value: tenantModel.SwitchTargets?.Join(),
                            _using: !context.Ajax))
                .OutgoingMailsForm(
                    context: context,
                    ss: ss,
                    referenceType: "Tenants",
                    referenceId: tenantModel.TenantId,
                    referenceVer: tenantModel.Ver)
                .CopyDialog(
                    context: context,
                    ss: ss)
                .OutgoingMailDialog()
                .EditorExtensions(
                    context: context,
                    tenantModel: tenantModel,
                    ss: ss));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditorTabs(
            this HtmlBuilder hb, Context context, TenantModel tenantModel)
        {
            return hb.Ul(id: "EditorTabs", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetGeneral",
                        text: Displays.General(context: context))));
        }

        private static HtmlBuilder FieldSetGeneral(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            TenantModel tenantModel)
        {
            return hb.FieldSet(id: "FieldSetGeneral", action: () => hb
                .FieldSetGeneralColumns(
                    context: context, ss: ss, tenantModel: tenantModel));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder FieldSetGeneralColumns(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            TenantModel tenantModel,
            bool preview = false)
        {
            var titleColumn = ss.GetColumn(context, "Title");
            var logoTypeColumn = ss.GetColumn(context, "LogoType");
            var htmlTitleTopColumn = ss.GetColumn(context, "HtmlTitleTop");
            var htmlTitleSiteColumn = ss.GetColumn(context, "HtmlTitleSite");
            var htmlTitleRecordColumn = ss.GetColumn(context, "HtmlTitleRecord");
            var topStyleColumn = ss.GetColumn(context, "TopStyle");
            var topScriptColumn = ss.GetColumn(context, "TopScript");
            var UsedStorageCapacity = ((BinaryUtilities.UsedTenantStorageSize(context: context) / 1024) / 1024) / 1024;
            return hb.FieldSet(id: "FieldSetGeneralColumns", action: () => hb
                .Field(
                    context: context,
                    ss: ss,
                    column: titleColumn,
                    value: tenantModel.Title
                        .ToControl(context: context, ss: ss, column: titleColumn),
                    columnPermissionType: Permissions.ColumnPermissionType(
                        context: context,
                        ss: ss,
                        column: titleColumn,
                        baseModel: tenantModel))
                .FieldDropDown(
                    context: context,
                    controlId: "Tenants_LogoType",
                    controlCss: " always-send",
                    labelText: Displays.TenantImageType(context),
                    optionCollection: new Dictionary<string, string>()
                    {
                        [TenantModel.LogoTypes.ImageOnly.ToInt().ToString()] = Displays.ImageOnly(context),
                        [TenantModel.LogoTypes.ImageAndTitle.ToInt().ToString()] = Displays.ImageAndText(context)
                    }
                    , selectedValue: tenantModel.LogoType.ToInt().ToString())
                .FieldDropDown(
                    context: context,
                    controlId: "Tenants_TopDashboards",
                    controlCss: " always-send",
                    labelText: Displays.Dashboards(context: context),
                    optionCollection: GetDashboardSelectOptions(context: context),
                    selectedValue: tenantModel.TopDashboards
                        ?.Deserialize<long?[]>()
                        ?.FirstOrDefault()
                        ?.ToString())
                .FieldSet(
                    id: "PermissionsField",
                    css: " enclosed",
                    legendText: Displays.Permissions(context: context),
                    action: () => hb
                        .FieldCheckBox(
                            controlId: "Tenants_DisableAllUsersPermission",
                            fieldCss: "field-auto-thin",
                            _checked: tenantModel.DisableAllUsersPermission,
                            labelText: Displays.Tenants_DisableAllUsersPermission(context: context))
                        .FieldCheckBox(
                            controlId: "Tenants_DisableApi",
                            fieldCss: "field-auto-thin",
                            _checked: tenantModel.DisableApi,
                            labelText: Displays.Tenants_DisableApi(context: context),
                            _using: context.ContractSettings.Api != false
                                && Parameters.User.DisableApi != true)
                        .FieldCheckBox(
                            controlId: "Tenants_DisableStartGuide",
                            fieldCss: "field-auto-thin",
                            _checked: tenantModel.DisableStartGuide,
                            labelText: Displays.Tenants_DisableStartGuide(context: context),
                            _using: Parameters.Service.ShowStartGuide == true))
                .TenantImageSettingsEditor(context, tenantModel)
                .FieldSet(
                    id: "HtmlTitleSettingsField",
                    css: " enclosed",
                    legendText: Displays.HtmlTitle(context),
                    action: () => hb
                        .Field(
                            context: context,
                            ss: ss,
                            column: htmlTitleTopColumn,
                            value: tenantModel.HtmlTitleTop.ToControl(
                                context: context,
                                ss: ss,
                                column: titleColumn),
                            columnPermissionType: Permissions.ColumnPermissionType(
                                context: context,
                                ss: ss,
                                column: htmlTitleTopColumn,
                                baseModel: tenantModel))
                        .Field(
                            context: context,
                            ss: ss,
                            column: htmlTitleSiteColumn,
                            value: tenantModel.HtmlTitleSite.ToControl(
                                context: context,
                                ss: ss,
                                column: titleColumn),
                            columnPermissionType: Permissions.ColumnPermissionType(
                                context: context,
                                ss: ss,
                                column: htmlTitleSiteColumn,
                                baseModel: tenantModel))
                        .Field(
                            context: context,
                            ss: ss,
                            column: htmlTitleRecordColumn,
                            value: tenantModel.HtmlTitleRecord.ToControl(
                                context: context,
                                ss: ss,
                                column: titleColumn),
                            columnPermissionType: Permissions.ColumnPermissionType(
                                context: context,
                                ss: ss,
                                column: htmlTitleRecordColumn,
                                baseModel: tenantModel)))
                .FieldSet(
                    id: "StorageCheckField",
                    css: " enclosed",
                    legendText: Displays.Storage(context),
                    action: () => hb
                        .FieldText(
                            fieldId: "StorageCapacity",
                            labelText: Displays.StorageCapacity(context: context),
                            text: context.ContractSettings.StorageSize.ToString() + "GB")
                        .FieldText(
                            fieldId: "UsedStorageCapacity",
                            labelText: Displays.UsedStorageCapacity(context: context),
                            text: UsedStorageCapacity.ToString("F4") + "GB")
                        .FieldText(
                            fieldId: "UseRateStorageCapacity",
                            labelText: Displays.UsedRateStorageCapacity(context: context),
                            text: ((UsedStorageCapacity.ToDecimal()
                                / context.ContractSettings.StorageSize.ToDecimal()) * 100).ToString("F4") + "%"),
                    _using: context.ContractSettings.StorageSize.ToDecimal() > 0)
                .FieldSet(
                    id: "StyleField",
                    css: " enclosed",
                    legendText: Displays.Style(context),
                    action: () => hb
                        .Field(
                            context: context,
                            ss: ss,
                            column: topStyleColumn,
                            value: tenantModel.TopStyle.ToControl(
                                context: context,
                                ss: ss,
                                column: topStyleColumn),
                            columnPermissionType: Permissions.ColumnPermissionType(
                                context: context,
                                ss: ss,
                                column: topStyleColumn,
                                baseModel: tenantModel)))
                .FieldSet(
                    id: "ScriptField",
                    css: " enclosed",
                    legendText: Displays.Script(context),
                    action: () => hb
                        .Field(
                            context: context,
                            ss: ss,
                            column: topScriptColumn,
                            value: tenantModel.TopScript.ToControl(
                                context: context,
                                ss: ss,
                                column: topScriptColumn),
                            columnPermissionType: Permissions.ColumnPermissionType(
                                context: context,
                                ss: ss,
                                column: topScriptColumn,
                                baseModel: tenantModel)))
                .FieldSet(
                    id: "MaintenanceField",
                    css: " enclosed",
                    legendText: Displays.Maintenance(context),
                    action: () => hb
                        .Button(
                            controlId: "TenantSyncByLdap",
                            controlCss: "button-icon",
                            text: Displays.SyncByLdap(context: context),
                            onClick: "$p.send($(this));",
                            icon: "ui-icon-disk",
                            action: "SyncByLdap",
                            method: "post",
                            _using: Parameters.BackgroundService.SyncByLdap),
                    _using: Parameters.BackgroundService.SyncByLdap));
        }

        public static HtmlBuilder Field(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            TenantModel tenantModel,
            Column column,
            bool controlOnly = false,
            bool alwaysSend = false,
            string idSuffix = null,
            bool preview = false,
            bool disableSection = false)
        {
            var value = tenantModel.ControlValue(
                context: context,
                ss: ss,
                column: column);
            if (value != null)
            {
                hb.Field(
                    context: context,
                    ss: ss,
                    column: column,
                    value: value,
                    columnPermissionType: Permissions.ColumnPermissionType(
                        context: context,
                        ss: ss,
                        column: column,
                        baseModel: tenantModel),
                    controlOnly: controlOnly,
                    alwaysSend: alwaysSend,
                    idSuffix: idSuffix,
                    preview: preview,
                    disableSection: disableSection);
            }
            return hb;
        }

        public static string ControlValue(
            this TenantModel tenantModel,
            Context context,
            SiteSettings ss,
            Column column)
        {
            switch (column.Name)
            {
                case "Ver":
                    return tenantModel.Ver
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                default:
                    switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                    {
                        case "Class":
                            return tenantModel.GetClass(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Num":
                            return tenantModel.GetNum(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Date":
                            return tenantModel.GetDate(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Description":
                            return tenantModel.GetDescription(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Check":
                            return tenantModel.GetCheck(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Attachments":
                            return tenantModel.GetAttachments(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        default: return null;
                    }
            }
        }

        private static HtmlBuilder MainCommandExtensions(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            TenantModel tenantModel)
        {
            return hb;
        }

        private static HtmlBuilder EditorExtensions(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            TenantModel tenantModel)
        {
            return hb;
        }

        public static string EditorJson(Context context, SiteSettings ss, int tenantId)
        {
            return EditorResponse(context, ss, new TenantModel(
                context, ss, tenantId,
                formData: context.Forms)).ToJson();
        }

        private static ResponseCollection EditorResponse(
            Context context,
            SiteSettings ss,
            TenantModel tenantModel,
            Message message = null,
            string switchTargets = null)
        {
            tenantModel.MethodType = tenantModel.TenantId == 0
                ? BaseModel.MethodTypes.New
                : BaseModel.MethodTypes.Edit;
            return new TenantsResponseCollection(
                context: context,
                tenantModel: tenantModel)
                    .Invoke("clearDialogs")
                    .ReplaceAll("#MainContainer", Editor(context, ss, tenantModel))
                    .Val("#SwitchTargets", switchTargets, _using: switchTargets != null)
                    .SetMemory("formChanged", false)
                    .Invoke("setCurrentIndex")
                    .Message(message)
                    .Messages(context.Messages)
                    .ClearFormData(_using: !context.QueryStrings.Bool("control-auto-postback"));
        }

        private static List<int> GetSwitchTargets(Context context, SiteSettings ss, int tenantId)
        {
            var view = Views.GetBySession(
                context: context,
                ss: ss,
                setSession: false);
            var where = view.Where(context: context, ss: ss, where: Rds.TenantsWhere().TenantId(context.TenantId));
            var param = view.Param(
                context: context,
                ss: ss);
            var orderBy = view.OrderBy(
                context: context,
                ss: ss)
                    .Tenants_UpdatedTime(SqlOrderBy.Types.desc);
            var join = ss.Join(
                context: context,
                join: new IJoin[]
                {
                    where,
                    orderBy
                });
            var switchTargets = new List<int>();
            if (Parameters.General.SwitchTargetsLimit > 0)
            {
                if (Repository.ExecuteScalar_int(
                    context: context,
                    statements: Rds.SelectTenants(
                        column: Rds.TenantsColumn().TenantsCount(),
                        join: join,
                        where: where,
                        param: param)) <= Parameters.General.SwitchTargetsLimit)
                {
                    switchTargets = Repository.ExecuteTable(
                        context: context,
                        statements: Rds.SelectTenants(
                            column: Rds.TenantsColumn().TenantId(),
                            join: join,
                            where: where,
                            param: param,
                            orderBy: orderBy))
                                .AsEnumerable()
                                .Select(o => o["TenantId"].ToInt())
                                .ToList();
                }
            }
            if (!switchTargets.Contains(tenantId))
            {
                switchTargets.Add(tenantId);
            }
            return switchTargets;
        }

        public static ResponseCollection FieldResponse(
            this ResponseCollection res,
            Context context,
            SiteSettings ss,
            TenantModel tenantModel,
            string idSuffix = null)
        {
            var replaceFieldColumns = ss.ReplaceFieldColumns(
                context: context,
                serverScriptModelRow: tenantModel.ServerScriptModelRow);
            res.Val(
                target: "#ReplaceFieldColumns",
                value: replaceFieldColumns?.ToJson());
            res.LookupClearFormData(
                context: context,
                ss: ss);
            var columnNames = ss.GetEditorColumnNames(context.QueryStrings.Bool("control-auto-postback")
                ? ss.GetColumn(
                    context: context,
                    columnName: context.Forms.ControlId().Split_2nd('_'))
                : null);
            columnNames
                .Select(columnName => ss.GetColumn(
                    context: context,
                    columnName: columnName))
                .Where(column => column != null)
                .ForEach(column =>
                {
                    var serverScriptModelColumn = tenantModel
                        ?.ServerScriptModelRow
                        ?.Columns.Get(column.ColumnName);
                    if (replaceFieldColumns?.Contains(column.ColumnName) == true)
                    {
                        res.ReplaceAll(
                            target: $"#Tenants_{column.Name}Field" + idSuffix,
                            value: new HtmlBuilder().Field(
                                context: context,
                                ss: ss,
                                tenantModel: tenantModel,
                                column: column,
                                idSuffix: idSuffix));
                    }
                    else
                    {
                        switch (column.Name)
                        {
                            case "TenantName":
                                res.Val(
                                    target: "#Tenants_TenantName" + idSuffix,
                                    value: tenantModel.TenantName.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "Title":
                                res.Val(
                                    target: "#Tenants_Title" + idSuffix,
                                    value: tenantModel.Title.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "Body":
                                res.Val(
                                    target: "#Tenants_Body" + idSuffix,
                                    value: tenantModel.Body.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "ContractDeadline":
                                res.Val(
                                    target: "#Tenants_ContractDeadline" + idSuffix,
                                    value: tenantModel.ContractDeadline.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "DisableAllUsersPermission":
                                res.Val(
                                    target: "#Tenants_DisableAllUsersPermission" + idSuffix,
                                    value: tenantModel.DisableAllUsersPermission,
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "DisableApi":
                                res.Val(
                                    target: "#Tenants_DisableApi" + idSuffix,
                                    value: tenantModel.DisableApi,
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "DisableStartGuide":
                                res.Val(
                                    target: "#Tenants_DisableStartGuide" + idSuffix,
                                    value: tenantModel.DisableStartGuide,
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "LogoType":
                                res.Val(
                                    target: "#Tenants_LogoType" + idSuffix,
                                    value: tenantModel.LogoType.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "HtmlTitleTop":
                                res.Val(
                                    target: "#Tenants_HtmlTitleTop" + idSuffix,
                                    value: tenantModel.HtmlTitleTop.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "HtmlTitleSite":
                                res.Val(
                                    target: "#Tenants_HtmlTitleSite" + idSuffix,
                                    value: tenantModel.HtmlTitleSite.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "HtmlTitleRecord":
                                res.Val(
                                    target: "#Tenants_HtmlTitleRecord" + idSuffix,
                                    value: tenantModel.HtmlTitleRecord.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "TopStyle":
                                res.Val(
                                    target: "#Tenants_TopStyle" + idSuffix,
                                    value: tenantModel.TopStyle.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "TopScript":
                                res.Val(
                                    target: "#Tenants_TopScript" + idSuffix,
                                    value: tenantModel.TopScript.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "TopDashboards":
                                res.Val(
                                    target: "#Tenants_TopDashboards" + idSuffix,
                                    value: tenantModel.TopDashboards.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            default:
                                switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                                {
                                    case "Class":
                                        res.Val(
                                            target: $"#Tenants_{column.Name}{idSuffix}",
                                            value: tenantModel.GetClass(columnName: column.Name).ToResponse(
                                                context: context,
                                                ss: ss,
                                                column: column),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Num":
                                        res.Val(
                                            target: $"#Tenants_{column.Name}{idSuffix}",
                                            value: tenantModel.GetNum(columnName: column.Name).ToResponse(
                                                context: context,
                                                ss: ss,
                                                column: column),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Date":
                                        res.Val(
                                            target: $"#Tenants_{column.Name}{idSuffix}",
                                            value: tenantModel.GetDate(columnName: column.Name).ToResponse(
                                                context: context,
                                                ss: ss,
                                                column: column),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Description":
                                        res.Val(
                                            target: $"#Tenants_{column.Name}{idSuffix}",
                                            value: tenantModel.GetDescription(columnName: column.Name).ToResponse(
                                                context: context,
                                                ss: ss,
                                                column: column),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Check":
                                        res.Val(
                                            target: $"#Tenants_{column.Name}{idSuffix}",
                                            value: tenantModel.GetCheck(columnName: column.Name),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Attachments":
                                        res.ReplaceAll(
                                            target: $"#Tenants_{column.Name}Field",
                                            value: new HtmlBuilder()
                                                .FieldAttachments(
                                                    context: context,
                                                    fieldId: $"Tenants_{column.Name}Field",
                                                    controlId: $"Tenants_{column.Name}",
                                                    columnName: column.ColumnName,
                                                    fieldCss: column.FieldCss
                                                        + (column.TextAlign == SiteSettings.TextAlignTypes.Right
                                                            ? " right-align"
                                                            : column.TextAlign == SiteSettings.TextAlignTypes.Center
                                                                ? " center-align"
                                                                : string.Empty),
                                                    fieldDescription: column.Description,
                                                    labelText: column.LabelText,
                                                    value: tenantModel.GetAttachments(columnName: column.Name).ToJson(),
                                                    readOnly: Permissions.ColumnPermissionType(
                                                        context: context,
                                                        ss: ss,
                                                        column: column,
                                                        baseModel: tenantModel)
                                                            != Permissions.ColumnPermissionTypes.Update,
                                                    allowDelete: column.AllowDeleteAttachments != false,
                                                    validateRequired: column.ValidateRequired != false),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                }
                                break;
                        }
                    }
                });
            return res;
        }

        public static string Create(Context context, SiteSettings ss)
        {
            ﻿//Issues、Results以外は参照コピーを使用しないためcopyFromを0にする
            var copyFrom = 0;
            var tenantModel = new TenantModel(
                context: context,
                ss: ss,
                tenantId: copyFrom,
                formData: context.Forms);
            tenantModel.TenantId = 0;
            tenantModel.Ver = 1;
            var invalid = TenantValidators.OnCreating(
                context: context,
                ss: ss,
                tenantModel: tenantModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            List<Process> processes = null;
            var errorData = tenantModel.Create(context: context, ss: ss);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    SessionUtilities.Set(
                        context: context,
                        message: CreatedMessage(
                            context: context,
                            ss: ss,
                            tenantModel: tenantModel,
                            process: processes?.FirstOrDefault(o => o.MatchConditions)));
                    return new ResponseCollection(
                        context: context,
                        id: tenantModel.TenantId)
                            .SetMemory("formChanged", false)
                            .Messages(context.Messages)
                            .Href(Locations.Edit(
                                context: context,
                                controller: context.Controller,
                                id: ss.Columns.Any(o => o.Linking)
                                    ? context.Forms.Long("LinkId")
                                    : tenantModel.TenantId)
                                        + "?new=1"
                                        + (ss.Columns.Any(o => o.Linking)
                                            && context.Forms.Long("FromTabIndex") > 0
                                                ? $"&TabIndex={context.Forms.Long("FromTabIndex")}"
                                                : string.Empty))
                            .ToJson();
                default:
                    return errorData.MessageJson(context: context);
            }
        }

        private static Message CreatedMessage(
            Context context,
            SiteSettings ss,
            TenantModel tenantModel,
            Process process)
        {
            if (process == null)
            {
                return Messages.Created(
                    context: context,
                    data: tenantModel.Title.Value);
            }
            else
            {
                var message = process.GetSuccessMessage(context: context);
                message.Text = tenantModel.ReplacedDisplayValues(
                    context: context,
                    ss: ss,
                    value: message.Text);
                return message;
            }
        }

        public static string Update(Context context, SiteSettings ss, int tenantId)
        {
            var tenantModel = new TenantModel(
                context: context,
                ss: ss,
                tenantId: tenantId,
                formData: context.Forms);
            var invalid = TenantValidators.OnUpdating(
                context: context,
                ss: ss,
                tenantModel: tenantModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            if (tenantModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts(context: context).ToJson();
            }
            List<Process> processes = null;
            var errorData = tenantModel.Update(context: context, ss: ss);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    var res = new TenantsResponseCollection(
                        context: context,
                        tenantModel: tenantModel);
                    return ResponseByUpdate(res, context, ss, tenantModel, processes)
                        .PrependComment(
                            context: context,
                            ss: ss,
                            column: ss.GetColumn(context: context, columnName: "Comments"),
                            comments: tenantModel.Comments,
                            verType: tenantModel.VerType)
                        .ToJson();
                case Error.Types.UpdateConflicts:
                    return Messages.ResponseUpdateConflicts(
                        context: context,
                        data: tenantModel.Updator.Name)
                            .ToJson();
                default:
                    return errorData.MessageJson(context: context);
            }
        }

        private static ResponseCollection ResponseByUpdate(
            TenantsResponseCollection res,
            Context context,
            SiteSettings ss,
            TenantModel tenantModel,
            List<Process> processes)
        {
            ss.ClearColumnAccessControlCaches(baseModel: tenantModel);
            if (context.Forms.Bool("IsDialogEditorForm"))
            {
                var view = Views.GetBySession(
                    context: context,
                    ss: ss,
                    setSession: false);
                var gridData = new GridData(
                    context: context,
                    ss: ss,
                    view: view,
                    tableType: Sqls.TableTypes.Normal,
                    where: Rds.TenantsWhere().TenantId(tenantModel.TenantId));
                var columns = ss.GetGridColumns(
                    context: context,
                    view: view,
                    checkPermission: true);
                return res
                    .ReplaceAll(
                        $"[data-id=\"{tenantModel.TenantId}\"][data-latest]",
                        new HtmlBuilder().GridRows(
                            context: context,
                            ss: ss,
                            view: view,
                            dataRows: gridData.DataRows,
                            columns: columns))
                    .CloseDialog()
                    .Message(message: UpdatedMessage(
                        context: context,
                        ss: ss,
                        tenantModel: tenantModel,
                        processes: processes))
                    .Messages(context.Messages);
            }
            else
            {
                var verUp = Versions.VerUp(
                    context: context,
                    ss: ss,
                    verUp: false);
                return res
                    .Ver(context: context, ss: ss)
                    .Timestamp(context: context, ss: ss)
                    .FieldResponse(context: context, ss: ss, tenantModel: tenantModel)
                    .Val("#VerUp", verUp)
                    .Val("#Ver", tenantModel.Ver)
                    .Disabled("#VerUp", verUp)
                .ReplaceAll("#Breadcrumb", new HtmlBuilder().TenantsBreadcrumb(
                    context: context,
                    ss:ss))
                    .Html("#HeaderTitle", HttpUtility.HtmlEncode(tenantModel.Title.Value))
                    .Html("#RecordInfo", new HtmlBuilder().RecordInfo(
                        context: context,
                        baseModel: tenantModel,
                        tableName: "Tenants"))
                    .SetMemory("formChanged", false)
                    .Message(Messages.Updated(
                        context: context,
                        data: tenantModel.Title.Value))
                    .Messages(context.Messages)
                    .Comment(
                        context: context,
                        ss: ss,
                        column: ss.GetColumn(context: context, columnName: "Comments"),
                        comments: tenantModel.Comments,
                        deleteCommentId: tenantModel.DeleteCommentId)
                    .ClearFormData();
            }
        }

        private static Message UpdatedMessage(
            Context context,
            SiteSettings ss,
            TenantModel tenantModel,
            List<Process> processes)
        {
            var process = processes?.FirstOrDefault(o => !o.SuccessMessage.IsNullOrEmpty()
                && o.MatchConditions);
            if (process == null)
            {
                return Messages.Updated(
                    context: context,
                    data: tenantModel.Title.MessageDisplay(context: context));
            }
            else
            {
                var message = process.GetSuccessMessage(context: context);
                message.Text = tenantModel.ReplacedDisplayValues(
                    context: context,
                    ss: ss,
                    value: message.Text);
                return message;
            }
        }

        public static string Delete(Context context, SiteSettings ss, int tenantId)
        {
            var tenantModel = new TenantModel(context, ss, tenantId);
            var invalid = TenantValidators.OnDeleting(
                context: context,
                ss: ss,
                tenantModel: tenantModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var errorData = tenantModel.Delete(context: context, ss: ss);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    SessionUtilities.Set(
                        context: context,
                        message: Messages.Deleted(
                            context: context,
                            data: tenantModel.Title.MessageDisplay(context: context)));
                    var res = new TenantsResponseCollection(
                        context: context,
                        tenantModel: tenantModel);
                    res
                        .SetMemory("formChanged", false)
                        .Invoke("back");
                    return res.ToJson();
                default:
                    return errorData.MessageJson(context: context);
            }
        }

        public static string Histories(
            Context context, SiteSettings ss, int tenantId, Message message = null)
        {
            var tenantModel = new TenantModel(context: context, ss: ss, tenantId: tenantId);
            var columns = ss.GetHistoryColumns(context: context, checkPermission: true);
            if (!context.CanRead(ss: ss))
            {
                return Error.Types.HasNotPermission.MessageJson(context: context);
            }
            var hb = new HtmlBuilder();
            hb
                .HistoryCommands(context: context, ss: ss)
                .Table(
                    attributes: new HtmlAttributes().Class("grid history"),
                    action: () => hb
                        .THead(action: () => hb
                            .GridHeader(
                                context: context,
                                ss: ss,
                                columns: columns,
                                sort: false,
                                checkRow: true))
                        .TBody(action: () => hb
                            .HistoriesTableBody(
                                context: context,
                                ss: ss,
                                columns: columns,
                                tenantModel: tenantModel)));
            return new TenantsResponseCollection(
                context: context,
                tenantModel: tenantModel)
                    .Html("#FieldSetHistories", hb)
                    .Message(message)
                    .Messages(context.Messages)
                    .ToJson();
        }

        private static void HistoriesTableBody(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            List<Column> columns,
            TenantModel tenantModel)
        {
            new TenantCollection(
                context: context,
                ss: ss,
                column: HistoryColumn(columns),
                where: Rds.TenantsWhere().TenantId(tenantModel.TenantId),
                orderBy: Rds.TenantsOrderBy().Ver(SqlOrderBy.Types.desc),
                tableType: Sqls.TableTypes.NormalAndHistory)
                    .ForEach(tenantModelHistory => hb
                        .Tr(
                            attributes: new HtmlAttributes()
                                .Class("grid-row")
                                .DataAction("History")
                                .DataMethod("post")
                                .DataVer(tenantModelHistory.Ver)
                                .DataLatest(
                                    value: 1,
                                    _using: tenantModelHistory.Ver == tenantModel.Ver),
                            action: () =>
                            {
                                hb.Td(
                                    css: "grid-check-td",
                                    action: () => hb
                                        .CheckBox(
                                            controlCss: "grid-check",
                                            _checked: false,
                                            dataId: tenantModelHistory.Ver.ToString(),
                                            _using: tenantModelHistory.Ver < tenantModel.Ver));
                                columns.ForEach(column => hb
                                    .TdValue(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        tenantModel: tenantModelHistory));
                            }));
        }

        private static SqlColumnCollection HistoryColumn(List<Column> columns)
        {
            var sqlColumn = new Rds.TenantsColumnCollection()
                .TenantId()
                .Ver();
            columns.ForEach(column =>
                sqlColumn.TenantsColumn(columnName: column.ColumnName));
            return sqlColumn;
        }

        public static string History(Context context, SiteSettings ss, int tenantId)
        {
            var tenantModel = new TenantModel(context: context, ss: ss, tenantId: tenantId);
            tenantModel.Get(
                context: context,
                ss: ss,
                where: Rds.TenantsWhere()
                    .TenantId(tenantModel.TenantId)
                    .Ver(context.Forms.Int("Ver")),
                tableType: Sqls.TableTypes.NormalAndHistory);
            tenantModel.VerType = context.Forms.Bool("Latest")
                ? Versions.VerTypes.Latest
                : Versions.VerTypes.History;
            return EditorResponse(context, ss, tenantModel)
                .PushState("History", Locations.Get(
                    context: context,
                    parts: new string[]
                    {
                        "Items",
                        tenantId.ToString() 
                            + (tenantModel.VerType == Versions.VerTypes.History
                                ? "?ver=" + context.Forms.Int("Ver") 
                                : string.Empty)
                    }))
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Dictionary<string,string> GetDashboardSelectOptions(Context context)
        {
            var dashboards = new SiteCollection(
                context: context,
                column: Rds.SitesColumn()
                    .SiteId()
                    .Title(),
            where: Rds.SitesWhere()
                    .TenantId(context.TenantId)
                    .ReferenceType("Dashboards")
                    .Add(
            raw: Def.Sql.HasPermission,
                _using: !context.HasPrivilege));
            var options = new Dictionary<string, string>()
            {
                {"",""}
            };
            options.AddRange(dashboards.ToDictionary(
                o => o.SiteId.ToString(),
                o => $"[{o.SiteId}] {o.Title.DisplayValue}"));
            return options;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string SyncByLdap(Context context, SiteSettings ss)
        {
            var invalid = TenantValidators.OnSyncByLdap(
                context: context,
                ss: ss);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            //SyncByLdap()に時間がかかるのでTask呼び出しするが、呼び出し側がasyncでないのでawait無し。
            System.Threading.Tasks.Task.Run(() => UserUtilities.SyncByLdap(context: context));
            return Messages.ResponseSyncByLdapStarted(context: context).ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ResponseCollection FieldResponse(
            this TenantsResponseCollection res,
            Context context,
            SiteSettings ss,
            TenantModel tenantModel)
        {
            var title = ss.GetColumn(context, "Title");
            if (title != null)
            {
                res.Val(
                    "#Tenants_Title",
                    tenantModel.Title.ToResponse(context: context, ss: ss, column: title));
            }
            var logoType = ss.GetColumn(context, "LogoType");
            if (logoType != null)
            {
                res.Val(
                    "#Tenants_LogoType",
                    tenantModel.LogoType.ToInt().ToResponse(context: context, ss: ss, column: logoType));
            }
            var htmlTitleTop = ss.GetColumn(context, "HtmlTitleTop");
            if (htmlTitleTop != null)
            {
                res.Val(
                    "#Tenants_HtmlTitleTop",
                    tenantModel.HtmlTitleTop.ToResponse(context: context, ss: ss, column: htmlTitleTop));
            }
            var htmlTitleSite = ss.GetColumn(context, "HtmlTitleSite");
            if (htmlTitleSite != null)
            {
                res.Val(
                    "#Tenants_HtmlTitleSite",
                    tenantModel.HtmlTitleSite.ToResponse(context: context, ss: ss, column: htmlTitleSite));
            }
            var htmlTitleRecord = ss.GetColumn(context, "HtmlTitleRecord");
            if (htmlTitleRecord != null)
            {
                res.Val(
                    "#Tenants_HtmlTitleRecord",
                    tenantModel.HtmlTitleRecord.ToResponse(context: context, ss: ss, column: htmlTitleRecord));
            }
            return res;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder TenantImageSettingsEditor(this HtmlBuilder hb, Context context,TenantModel tenantModel)
        {
             return hb.FieldSet(
                    id: "TenantImageSettingsEditor",
                    css: " enclosed",
                    legendText: Displays.LogoImage(context: context),
                    action: () => hb
                        .FieldTextBox(
                            textType: HtmlTypes.TextTypes.File,
                            controlId: "TenantImage",
                            fieldCss: "field-auto-thin",
                            controlCss: " w400",
                            labelText: Displays.File(context: context))
                        .Button(
                            controlId: "SetTenantImage",
                            controlCss: "button-icon",
                            text: Displays.Upload(context: context),
                            onClick: "$p.uploadTenantImage($(this));",
                            icon: "ui-icon-disk",
                            action: "binaries/updatetenantimage",
                            method: "post")
                        .Button(
                            controlCss: "button-icon",
                            text: Displays.Delete(context: context),
                            onClick: "$p.send($(this));",
                            icon: "ui-icon-trash",
                            action: "binaries/deletetenantimage",
                            method: "delete",
                            confirm: "ConfirmDelete",
                            _using: BinaryUtilities.ExistsTenantImage(
                                context: context, 
                                ss: SiteSettingsUtilities.TenantsSiteSettings(context),
                                referenceId: tenantModel.TenantId,
                                sizeType: Libraries.Images.ImageData.SizeTypes.Logo)));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ContractSettings GetContractSettings(Context context, int tenantId)
        {
            var dataRow = Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectTenants(
                    column: Rds.TenantsColumn()
                        .ContractSettings()
                        .ContractDeadline(),
                    where: Rds.TenantsWhere().TenantId(tenantId)))
                        .AsEnumerable()
                        .FirstOrDefault();
            var contractSettings = dataRow?.String("ContractSettings").Deserialize<ContractSettings>()
                ?? new ContractSettings();
            contractSettings.Deadline = dataRow?.DateTime("ContractDeadline");
            return contractSettings;
        }
    }
}
