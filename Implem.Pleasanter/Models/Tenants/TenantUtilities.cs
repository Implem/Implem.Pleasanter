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
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Models
{
    public static class TenantUtilities
    {
        public static HtmlBuilder TdValue(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Column column,
            TenantModel tenantModel)
        {
            if (!column.GridDesign.IsNullOrEmpty())
            {
                return hb.TdCustomValue(
                    context: context,
                    ss: ss,
                    gridDesign: column.GridDesign,
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
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: tenantModel.Ver)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "Comments":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: tenantModel.Comments)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "Creator":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: tenantModel.Creator)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "Updator":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: tenantModel.Updator)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CreatedTime":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: tenantModel.CreatedTime)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "UpdatedTime":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: tenantModel.UpdatedTime)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    default:
                        switch (Def.ExtendedColumnTypes.Get(column.Name))
                        {
                            case "Class":
                                return ss.ReadColumnAccessControls.Allowed(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    type: ss.PermissionType,
                                    mine: mine)
                                        ? hb.Td(
                                            context: context,
                                            column: column,
                                            value: tenantModel.Class(columnName: column.Name))
                                        : hb.Td(
                                            context: context,
                                            column: column,
                                            value: string.Empty);
                            case "Num":
                                return ss.ReadColumnAccessControls.Allowed(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    type: ss.PermissionType,
                                    mine: mine)
                                        ? hb.Td(
                                            context: context,
                                            column: column,
                                            value: tenantModel.Num(columnName: column.Name))
                                        : hb.Td(
                                            context: context,
                                            column: column,
                                            value: string.Empty);
                            case "Date":
                                return ss.ReadColumnAccessControls.Allowed(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    type: ss.PermissionType,
                                    mine: mine)
                                        ? hb.Td(
                                            context: context,
                                            column: column,
                                            value: tenantModel.Date(columnName: column.Name))
                                        : hb.Td(
                                            context: context,
                                            column: column,
                                            value: string.Empty);
                            case "Description":
                                return ss.ReadColumnAccessControls.Allowed(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    type: ss.PermissionType,
                                    mine: mine)
                                        ? hb.Td(
                                            context: context,
                                            column: column,
                                            value: tenantModel.Description(columnName: column.Name))
                                        : hb.Td(
                                            context: context,
                                            column: column,
                                            value: string.Empty);
                            case "Check":
                                return ss.ReadColumnAccessControls.Allowed(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    type: ss.PermissionType,
                                    mine: mine)
                                        ? hb.Td(
                                            context: context,
                                            column: column,
                                            value: tenantModel.Check(columnName: column.Name))
                                        : hb.Td(
                                            context: context,
                                            column: column,
                                            value: string.Empty);
                            case "Attachments":
                                return ss.ReadColumnAccessControls.Allowed(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    type: ss.PermissionType,
                                    mine: mine)
                                        ? hb.Td(
                                            context: context,
                                            column: column,
                                            value: tenantModel.Attachments(columnName: column.Name))
                                        : hb.Td(
                                            context: context,
                                            column: column,
                                            value: string.Empty);
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
                        switch (Def.ExtendedColumnTypes.Get(column.Name))
                        {
                            case "Class":
                                value = tenantModel.Class(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Num":
                                value = tenantModel.Num(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Date":
                                value = tenantModel.Date(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Description":
                                value = tenantModel.Description(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Check":
                                value = tenantModel.Check(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Attachments":
                                value = tenantModel.Attachments(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                        }
                        break;
                }
                gridDesign = gridDesign.Replace("[" + column.ColumnName + "]", value);
            });
            return hb.Td(action: () => hb
                .Div(css: "markup", action: () => hb
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
            ss.SetColumnAccessControls(
                context: context,
                mine: tenantModel.Mine(context: context));
            return hb.Template(
                context: context,
                ss: ss,
                view: null,
                verType: tenantModel.VerType,
                methodType: tenantModel.MethodType,
                referenceType: "Tenants",
                title: tenantModel.MethodType == BaseModel.MethodTypes.New
                    ? Displays.Tenants(context: context) + " - " + Displays.New(context: context)
                    : tenantModel.Title.Value,
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
            var commentsColumnPermissionType = commentsColumn
                .ColumnPermissionType(context: context);
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
                        .Div(id: "EditorTabsContainer", css: tabsCss, action: () => hb
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
                    referenceType: "Tenants",
                    id: tenantModel.TenantId)
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
            var mine = tenantModel.Mine(context: context);
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
            var title = ss.GetColumn(context, "Title");
            var logoType = ss.GetColumn(context, "LogoType");
            var htmlTitleTop = ss.GetColumn(context, "HtmlTitleTop");
            var htmlTitleSite = ss.GetColumn(context, "HtmlTitleSite");
            var htmlTitleRecord = ss.GetColumn(context, "HtmlTitleRecord");
            var UsedStorageCapacity = ((BinaryUtilities.UsedTenantStorageSize(context: context) / 1024) / 1024) / 1024;
            return hb.FieldSet(id: "FieldSetGeneral", action: () => hb
                .Field(
                    context: context,
                    ss: ss,
                    column: title,
                    methodType: tenantModel.MethodType,
                    value: tenantModel.Title
                        .ToControl(context: context, ss: ss, column: title),
                            columnPermissionType: title.ColumnPermissionType(context: context))
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
                            column: htmlTitleTop,
                            methodType: tenantModel.MethodType,
                            value: tenantModel.HtmlTitleTop.ToControl(context: context, ss: ss, column: title),
                            columnPermissionType: htmlTitleTop.ColumnPermissionType(context: context))
                        .Field(
                            context: context,
                            ss: ss,
                            column: htmlTitleSite,
                            methodType: tenantModel.MethodType,
                            value: tenantModel.HtmlTitleSite.ToControl(context: context, ss: ss, column: title),
                            columnPermissionType: htmlTitleSite.ColumnPermissionType(context: context))
                        .Field(
                            context: context,
                            ss: ss,
                            column: htmlTitleRecord,
                            methodType: tenantModel.MethodType,
                            value: tenantModel.HtmlTitleRecord.ToControl(context: context, ss: ss, column: title),
                            columnPermissionType: htmlTitleRecord.ColumnPermissionType(context: context)))
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
                    _using: context.ContractSettings.StorageSize.ToDecimal() > 0));
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
                    methodType: tenantModel.MethodType,
                    value: value,
                    columnPermissionType: column.ColumnPermissionType(context: context),
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
                    switch (Def.ExtendedColumnTypes.Get(column.Name))
                    {
                        case "Class":
                            return tenantModel.Class(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Num":
                            return tenantModel.Num(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Date":
                            return tenantModel.Date(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Description":
                            return tenantModel.Description(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Check":
                            return tenantModel.Check(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Attachments":
                            return tenantModel.Attachments(columnName: column.Name)
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
                context, ss, tenantId)).ToJson();
        }

        private static ResponseCollection EditorResponse(
            Context context,
            SiteSettings ss,
            TenantModel tenantModel,
            Message message = null,
            string switchTargets = null)
        {
            tenantModel.MethodType = BaseModel.MethodTypes.Edit;
            return new TenantsResponseCollection(tenantModel)
                .Invoke("clearDialogs")
                .ReplaceAll("#MainContainer", Editor(context, ss, tenantModel))
                .Val("#SwitchTargets", switchTargets, _using: switchTargets != null)
                .SetMemory("formChanged", false)
                .Invoke("setCurrentIndex")
                .Message(message)
                .ClearFormData();
        }

        private static List<int> GetSwitchTargets(Context context, SiteSettings ss, int tenantId)
        {
            var view = Views.GetBySession(
                context: context,
                ss: ss,
                setSession: false);
            var where = view.Where(context: context, ss: ss, where: Rds.TenantsWhere().TenantId(context.TenantId));
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
            var switchTargets = Repository.ExecuteScalar_int(
                context: context,
                statements: Rds.SelectTenants(
                    column: Rds.TenantsColumn().TenantsCount(),
                    join: join,
                    where: where)) <= Parameters.General.SwitchTargetsLimit
                        ? Repository.ExecuteTable(
                            context: context,
                            statements: Rds.SelectTenants(
                                column: Rds.TenantsColumn().TenantId(),
                                join: join,
                                where: where,
                                orderBy: orderBy))
                                    .AsEnumerable()
                                    .Select(o => o["TenantId"].ToInt())
                                    .ToList()
                        : new List<int>();
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
            var mine = tenantModel.Mine(context: context);
            ss.EditorColumns
                .Select(columnName => ss.GetColumn(
                    context: context,
                    columnName: columnName))
                .Where(column => column != null)
                .ForEach(column =>
                {
                    switch (column.Name)
                    {
                        case "TenantName":
                            res.Val(
                                "#Tenants_TenantName" + idSuffix,
                                tenantModel.TenantName.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "Title":
                            res.Val(
                                "#Tenants_Title" + idSuffix,
                                tenantModel.Title.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "Body":
                            res.Val(
                                "#Tenants_Body" + idSuffix,
                                tenantModel.Body.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ContractDeadline":
                            res.Val(
                                "#Tenants_ContractDeadline" + idSuffix,
                                tenantModel.ContractDeadline.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DisableAllUsersPermission":
                            res.Val(
                                "#Tenants_DisableAllUsersPermission" + idSuffix,
                                tenantModel.DisableAllUsersPermission);
                            break;
                        case "LogoType":
                            res.Val(
                                "#Tenants_LogoType" + idSuffix,
                                tenantModel.LogoType.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "HtmlTitleTop":
                            res.Val(
                                "#Tenants_HtmlTitleTop" + idSuffix,
                                tenantModel.HtmlTitleTop.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "HtmlTitleSite":
                            res.Val(
                                "#Tenants_HtmlTitleSite" + idSuffix,
                                tenantModel.HtmlTitleSite.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "HtmlTitleRecord":
                            res.Val(
                                "#Tenants_HtmlTitleRecord" + idSuffix,
                                tenantModel.HtmlTitleRecord.ToResponse(context: context, ss: ss, column: column));
                            break;
                        default:
                            switch (Def.ExtendedColumnTypes.Get(column.Name))
                            {
                                case "Class":
                                    res.Val(
                                        $"#Tenants_{column.Name}{idSuffix}",
                                        tenantModel.Class(columnName: column.Name).ToResponse(
                                            context: context,
                                            ss: ss,
                                            column: column));
                                    break;
                                case "Num":
                                    res.Val(
                                        $"#Tenants_{column.Name}{idSuffix}",
                                        tenantModel.Num(columnName: column.Name).ToResponse(
                                            context: context,
                                            ss: ss,
                                            column: column));
                                    break;
                                case "Date":
                                    res.Val(
                                        $"#Tenants_{column.Name}{idSuffix}",
                                        tenantModel.Date(columnName: column.Name).ToResponse(
                                            context: context,
                                            ss: ss,
                                            column: column));
                                    break;
                                case "Description":
                                    res.Val(
                                        $"#Tenants_{column.Name}{idSuffix}",
                                        tenantModel.Description(columnName: column.Name).ToResponse(
                                            context: context,
                                            ss: ss,
                                            column: column));
                                    break;
                                case "Check":
                                    res.Val(
                                        $"#Tenants_{column.Name}{idSuffix}",
                                        tenantModel.Check(columnName: column.Name));
                                    break;
                                case "Attachments":
                                    res.ReplaceAll(
                                        $"#Tenants_{column.Name}Field",
                                        new HtmlBuilder()
                                            .FieldAttachments(
                                                context: context,
                                                fieldId: $"Tenants_{column.Name}Field",
                                                controlId: $"Tenants_{column.Name}",
                                                columnName: column.ColumnName,
                                                fieldCss: column.FieldCss,
                                                fieldDescription: column.Description,
                                                controlCss: column.ControlCss,
                                                labelText: column.LabelText,
                                                value: tenantModel.Attachments(columnName: column.Name).ToJson(),
                                                placeholder: column.LabelText,
                                                readOnly: column.ColumnPermissionType(context: context)
                                                    != Permissions.ColumnPermissionTypes.Update));
                                    break;
                            }
                            break;
                    }
                });
            return res;
        }

        public static string Create(Context context, SiteSettings ss)
        {
            var tenantModel = new TenantModel(
                context: context,
                ss: ss,
                tenantId: 0,
                formData: context.Forms);
            var invalid = TenantValidators.OnCreating(
                context: context,
                ss: ss,
                tenantModel: tenantModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.Type.MessageJson(context: context);
            }
            var errorData = tenantModel.Create(context: context, ss: ss);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    SessionUtilities.Set(
                        context: context,
                        message: Messages.Created(
                            context: context,
                            data: tenantModel.Title.Value));
                    return new ResponseCollection()
                        .Response("id", tenantModel.TenantId.ToString())
                        .SetMemory("formChanged", false)
                        .Href(Locations.Edit(
                            context: context,
                            controller: context.Controller,
                            id: ss.Columns.Any(o => o.Linking)
                                ? context.Forms.Long("LinkId")
                                : tenantModel.TenantId))
                        .ToJson();
                default:
                    return errorData.Type.MessageJson(context: context);
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
                default: return invalid.Type.MessageJson(context: context);
            }
            if (tenantModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts(context: context).ToJson();
            }
            var errorData = tenantModel.Update(context: context, ss: ss);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    var res = new TenantsResponseCollection(tenantModel);
                    return ResponseByUpdate(res, context, ss, tenantModel)
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
                    return errorData.Type.MessageJson(context: context);
            }
        }

        private static ResponseCollection ResponseByUpdate(
            TenantsResponseCollection res,
            Context context,
            SiteSettings ss,
            TenantModel tenantModel)
        {
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
                            dataRows: gridData.DataRows,
                            columns: columns,
                            gridSelector: null))
                    .CloseDialog()
                    .Message(Messages.Updated(
                        context: context,
                        data: tenantModel.Title.DisplayValue));
            }
            else
            {
                return res
                    .Ver(context: context, ss: ss)
                    .Timestamp(context: context, ss: ss)
                    .FieldResponse(context: context, ss: ss, tenantModel: tenantModel)
                    .Val("#VerUp", false)
                    .Disabled("#VerUp", false)
                    .Html("#HeaderTitle", tenantModel.Title.Value)
                    .Html("#RecordInfo", new HtmlBuilder().RecordInfo(
                        context: context,
                        baseModel: tenantModel,
                        tableName: "Tenants"))
                    .SetMemory("formChanged", false)
                    .Message(Messages.Updated(
                        context: context,
                        data: tenantModel.Title.Value))
                    .Comment(
                        context: context,
                        ss: ss,
                        column: ss.GetColumn(context: context, columnName: "Comments"),
                        comments: tenantModel.Comments,
                        deleteCommentId: tenantModel.DeleteCommentId)
                    .ClearFormData();
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
                default: return invalid.Type.MessageJson(context: context);
            }
            var errorData = tenantModel.Delete(context: context, ss: ss);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    SessionUtilities.Set(
                        context: context,
                        message: Messages.Deleted(
                            context: context,
                            data: tenantModel.Title.Value));
                    var res = new TenantsResponseCollection(tenantModel);
                    res
                        .SetMemory("formChanged", false)
                        .Invoke("back");
                    return res.ToJson();
                default:
                    return errorData.Type.MessageJson(context: context);
            }
        }

        public static string Histories(
            Context context, SiteSettings ss, int tenantId, Message message = null)
        {
            var tenantModel = new TenantModel(context: context, ss: ss, tenantId: tenantId);
            ss.SetColumnAccessControls(
                context: context,
                mine: tenantModel.Mine(context: context));
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
            return new TenantsResponseCollection(tenantModel)
                .Html("#FieldSetHistories", hb)
                .Message(message)
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
                                .DataLatest(1, _using:
                                    tenantModelHistory.Ver == tenantModel.Ver),
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
                                columns
                                    .ForEach(column => hb
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
            ss.SetColumnAccessControls(
                context: context,
                mine: tenantModel.Mine(context: context));
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
