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
    public static class ParameterUtilities
    {
        public static HtmlBuilder TdValue(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Column column,
            ParameterModel parameterModel,
            int? tabIndex = null,
            ServerScriptModelColumn serverScriptModelColumn = null)
        {
            if (serverScriptModelColumn?.HideChanged == true && serverScriptModelColumn?.Hide == true)
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
                    parameterModel: parameterModel);
            }
            else
            {
                var mine = parameterModel.Mine(context: context);
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
                                    value: parameterModel.Ver,
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
                                    value: parameterModel.Comments,
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
                                    value: parameterModel.Creator,
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
                                    value: parameterModel.Updator,
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
                                    value: parameterModel.CreatedTime,
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
                                    value: parameterModel.UpdatedTime,
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
                                            value: parameterModel.GetClass(columnName: column.Name),
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
                                            value: parameterModel.GetNum(columnName: column.Name),
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
                                            value: parameterModel.GetDate(columnName: column.Name),
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
                                            value: parameterModel.GetDescription(columnName: column.Name),
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
                                            value: parameterModel.GetCheck(columnName: column.Name),
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
                                            value: parameterModel.GetAttachments(columnName: column.Name),
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
            ParameterModel parameterModel)
        {
            ss.IncludedColumns(gridDesign).ForEach(column =>
            {
                var value = string.Empty;
                switch (column.Name)
                {
                    case "Ver": value = parameterModel.Ver.GridText(
                        context: context,
                        column: column); break;
                    case "Comments": value = parameterModel.Comments.GridText(
                        context: context,
                        column: column); break;
                    case "Creator": value = parameterModel.Creator.GridText(
                        context: context,
                        column: column); break;
                    case "Updator": value = parameterModel.Updator.GridText(
                        context: context,
                        column: column); break;
                    case "CreatedTime": value = parameterModel.CreatedTime.GridText(
                        context: context,
                        column: column); break;
                    case "UpdatedTime": value = parameterModel.UpdatedTime.GridText(
                        context: context,
                        column: column); break;
                    default:
                        switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                        {
                            case "Class":
                                value = parameterModel.GetClass(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Num":
                                value = parameterModel.GetNum(columnName: column.Name)?.Value?.GridText(
                                    context: context,
                                    column: column) ?? string.Empty;
                                break;
                            case "Date":
                                value = parameterModel.GetDate(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Description":
                                value = parameterModel.GetDescription(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Check":
                                value = parameterModel.GetCheck(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Attachments":
                                value = parameterModel.GetAttachments(columnName: column.Name).GridText(
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
                    .MarkDown(
                        context: context,
                        ss: ss,
                        disabled: true,
                        text: gridDesign));
        }

        public static string EditorNew(Context context, SiteSettings ss)
        {
            return Editor(context: context, ss: ss, parameterModel: new ParameterModel(
                context: context,
                ss: SiteSettingsUtilities.ParametersSiteSettings(context: context),
                methodType: BaseModel.MethodTypes.New));
        }

        public static string Editor(
            Context context, SiteSettings ss, int parameterId, bool clearSessions)
        {
            var parameterModel = new ParameterModel(
                context: context,
                ss: SiteSettingsUtilities.ParametersSiteSettings(context: context),
                parameterId: parameterId,
                clearSessions: clearSessions,
                methodType: BaseModel.MethodTypes.Edit);
            parameterModel.SwitchTargets = GetSwitchTargets(
                context: context,
                ss: SiteSettingsUtilities.ParametersSiteSettings(context: context),
                parameterId: parameterModel.ParameterId);
            return Editor(context: context, ss: ss, parameterModel: parameterModel);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Editor(
            Context context, SiteSettings ss, ParameterModel parameterModel)
        {
            var hb = new HtmlBuilder();
            return hb.Template(
                context: context,
                ss: ss,
                view: null,
                referenceType: "Parameters",
                title: Displays.Parameters(context: context),
                action: () => hb
                    .Editor(
                        context: context,
                        ss: ss,
                        parameterModel: parameterModel)).ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Editor(
            this HtmlBuilder hb, Context context, SiteSettings ss, ParameterModel parameterModel)
        {
            var commentsColumn = ss.GetColumn(context: context, columnName: "Comments");
            var commentsColumnPermissionType =  Permissions.ColumnPermissionType(
                context: context,
                ss: ss,
                column: commentsColumn,
                baseModel: parameterModel);
            var showComments = ss.ShowComments(commentsColumnPermissionType);
            var tabsCss = showComments ? null : "max";
            return hb.Div(id: "Editor", action: () => hb
                .Form(
                    attributes: new HtmlAttributes()
                        .Id("MainForm")
                        .Class("main-form confirm-unload")
                        .Action(parameterModel.ParameterId != 0
                            ? Locations.Action(
                                context: context,
                                controller: "Parameters",
                                id: parameterModel.ParameterId)
                            : Locations.Action(
                                context: context,
                                controller: "Parameters")),
                    action: () => hb
                        .RecordHeader(
                            context: context,
                            ss: ss,
                            baseModel: parameterModel,
                            tableName: "Parameters")
                        .Div(
                            id: "EditorComments", action: () => hb
                                .Comments(
                                    context: context,
                                    ss: ss,
                                    comments: parameterModel.Comments,
                                    column: commentsColumn,
                                    verType: parameterModel.VerType,
                                    columnPermissionType: commentsColumnPermissionType),
                            _using: showComments)
                        .Div(
                            id: "EditorTabsContainer",
                            css: "tab-container " + tabsCss,
                            action: () => hb
                                .EditorTabs(
                                    context: context,
                                    parameterModel: parameterModel)
                                .FieldSetGeneral(context: context, ss: ss, parameterModel: parameterModel)
                                .FieldSet(
                                    attributes: new HtmlAttributes()
                                        .Id("FieldSetHistories")
                                        .DataAction("Histories")
                                        .DataMethod("post"),
                                    _using: parameterModel.MethodType != BaseModel.MethodTypes.New
                                        && !context.Publish)
                                .MainCommands(
                                    context: context,
                                    ss: ss,
                                    verType: parameterModel.VerType,
                                    updateButton: true,
                                    mailButton: true,
                                    deleteButton: true,
                                    extensions: () => hb
                                        .MainCommandExtensions(
                                            context: context,
                                            parameterModel: parameterModel,
                                            ss: ss)))
                        .Hidden(
                            controlId: "BaseUrl",
                            value: Locations.BaseUrl(context: context))
                        .Hidden(
                            controlId: "Ver",
                            value: parameterModel.Ver.ToString())
                        .Hidden(
                            controlId: "MethodType",
                            value: parameterModel.MethodType.ToString().ToLower())
                        .Hidden(
                            controlId: "Parameters_Timestamp",
                            css: "always-send",
                            value: parameterModel.Timestamp)
                        .Hidden(
                            controlId: "SwitchTargets",
                            css: "always-send",
                            value: parameterModel.SwitchTargets?.Join(),
                            _using: !context.Ajax))
                .OutgoingMailsForm(
                    context: context,
                    ss: ss,
                    referenceType: "Parameters",
                    referenceId: parameterModel.ParameterId,
                    referenceVer: parameterModel.Ver)
                .DropDownSearchDialog(
                    context: context,
                    id: ss.SiteId)
                .CopyDialog(
                    context: context,
                    ss: ss)
                .OutgoingMailDialog(context: context)
                .EditorExtensions(
                    context: context,
                    parameterModel: parameterModel,
                    ss: ss));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditorTabs(
            this HtmlBuilder hb, Context context, ParameterModel parameterModel)
        {
            return hb.Ul(id: "EditorTabs", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetGeneral",
                        text: Displays.General(context: context))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder FieldSetGeneral(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            ParameterModel parameterModel)
        {
            return hb.TabsPanelField(
                id: "FieldSetGeneral",
                action: () => hb
                    .FieldDropDown(
                        context: context,
                        controlId: "Parameters_Title",
                        fieldCss: "field-wide",
                        controlCss: " auto-postback always-send",
                        labelText: Displays.Parameters_Title(context),
                        optionCollection: Parameters.ParameterHash
                            .OrderBy(o => o.Key)
                            .ToDictionary(o => o.Key, o => o.Key + ".json"),
                        selectedValue: context.Forms.Data("Parameters_Title"),
                        action: "edit",
                        method: "post")
                    .FieldSetGeneralColumns(
                        context: context,
                        ss: ss,
                        parameterModel: parameterModel));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder FieldSetGeneralColumns(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            ParameterModel parameterModel,
            bool preview = false)
        {
            ss.GetEditorColumns(context: context).ForEach(column =>
            {
                if (column.ColumnName == "Body")
                {
                    hb.FieldCodeEditor(
                        context: context,
                        controlId: column.Id,
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        dataLang: "json",
                        labelText: column.LabelText,
                        text: parameterModel.ControlValue(
                            context: context,
                            ss: ss,
                            column: column));
                }
                else
                {
                    hb.Field(
                        context: context,
                        ss: ss,
                        parameterModel: parameterModel,
                        column: column,
                        preview: preview);
                }
            });
            if (!preview)
            {
                hb.VerUpCheckBox(
                    context: context,
                    ss: ss,
                    baseModel: parameterModel);
            }
            return hb;
        }

        public static HtmlBuilder Field(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            ParameterModel parameterModel,
            Column column,
            bool controlOnly = false,
            bool alwaysSend = false,
            string idSuffix = null,
            bool isResponse = false,
            bool preview = false,
            bool disableSection = false)
        {
            var value = parameterModel.ControlValue(
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
                        baseModel: parameterModel),
                    controlOnly: controlOnly,
                    alwaysSend: alwaysSend,
                    idSuffix: idSuffix,
                    isResponse: isResponse,
                    preview: preview,
                    disableSection: disableSection);
            }
            return hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string ControlValue(
            this ParameterModel parameterModel,
            Context context,
            SiteSettings ss,
            Column column)
        {
            switch (column.Name)
            {
                case "ParameterId":
                    return parameterModel.ParameterId
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Ver":
                    return parameterModel.Ver
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Body":
                    var body = PatchedBody(parameterModel);
                    return body;
                default:
                    switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                    {
                        case "Class":
                            return parameterModel.GetClass(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Num":
                            return parameterModel.GetNum(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Date":
                            return parameterModel.GetDate(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Description":
                            return parameterModel.GetDescription(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Check":
                            return parameterModel.GetCheck(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Attachments":
                            return parameterModel.GetAttachments(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        default: return null;
                    }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder MainCommandExtensions(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            ParameterModel parameterModel)
        {
            if (context.CanRestart())
            {
                hb.Button(
                    text: Displays.Restart(context: context),
                    controlCss: "button-icon button-negative",
                    onClick: "$p.send($(this));",
                    icon: "ui-icon-refresh",
                    action: "Restart",
                    method: "post",
                    confirm: Displays.ConfirmRestart(context: context));
            }
            return hb;
        }

        private static HtmlBuilder EditorExtensions(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            ParameterModel parameterModel)
        {
            return hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string EditorJson(Context context, SiteSettings ss, int parameterId)
        {
            if (!context.CanRead(ss: ss))
            {
                return Error.Types.NotFound.MessageJson(context: context);
            }
            var parameterModel = new ParameterModel(
                context: context,
                ss: ss,
                title: context.Forms.Data("Parameters_Title"));
            return EditorResponse(
                context: context,
                ss: ss,
                parameterModel: parameterModel).ToJson();
        }

        private static ResponseCollection EditorResponse(
            Context context,
            SiteSettings ss,
            ParameterModel parameterModel,
            Message message = null,
            string switchTargets = null)
        {
            parameterModel.MethodType = BaseModel.MethodTypes.Edit;
            return new ParametersResponseCollection(
                context: context,
                parameterModel: parameterModel)
                    .Invoke("clearDialogs")
                    .ReplaceAll("#MainContainer", Editor(context, ss, parameterModel))
                    .Val("#SwitchTargets", switchTargets, _using: switchTargets != null)
                    .SetMemory("formChanged", false)
                    .Invoke("setCurrentIndex")
                    .Message(message)
                    .Messages(context.Messages)
                    .ClearFormData(_using: !context.QueryStrings.Bool("control-auto-postback"));
        }

        private static List<int> GetSwitchTargets(Context context, SiteSettings ss, int parameterId)
        {
            var view = Views.GetBySession(
                context: context,
                ss: ss,
                setSession: false);
            var where = view.Where(context: context, ss: ss);
            var param = view.Param(
                context: context,
                ss: ss);
            var orderBy = view.OrderBy(
                context: context,
                ss: ss)
                    .Parameters_UpdatedTime(SqlOrderBy.Types.desc);
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
                    statements: Rds.SelectParameters(
                        column: Rds.ParametersColumn().ParametersCount(),
                        join: join,
                        where: where,
                        param: param)) <= Parameters.General.SwitchTargetsLimit)
                {
                    switchTargets = Repository.ExecuteTable(
                        context: context,
                        statements: Rds.SelectParameters(
                            column: Rds.ParametersColumn().ParameterId(),
                            join: join,
                            where: where,
                            param: param,
                            orderBy: orderBy))
                                .AsEnumerable()
                                .Select(o => o["ParameterId"].ToInt())
                                .ToList();
                }
            }
            if (!switchTargets.Contains(parameterId))
            {
                switchTargets.Add(parameterId);
            }
            return switchTargets;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ResponseCollection FieldResponse(
            this ResponseCollection res,
            Context context,
            SiteSettings ss,
            ParameterModel parameterModel,
            string idSuffix = null)
        {
            var replaceFieldColumns = ss.ReplaceFieldColumns(
                context: context,
                serverScriptModelRow: parameterModel.ServerScriptModelRow);
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
                    var serverScriptModelColumn = parameterModel
                        ?.ServerScriptModelRow
                        ?.Columns.Get(column.ColumnName);
                    if (replaceFieldColumns?.Contains(column.ColumnName) == true)
                    {
                        res.ReplaceAll(
                            target: $"#Parameters_{column.Name}Field" + idSuffix,
                            value: new HtmlBuilder().Field(
                                context: context,
                                ss: ss,
                                parameterModel: parameterModel,
                                column: column,
                                idSuffix: idSuffix,
                                isResponse: true));
                    }
                    else
                    {
                        switch (column.Name)
                        {
                            case "Title":
                                res.Val(
                                    target: "#Parameters_Title" + idSuffix,
                                    value: parameterModel.Title.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "Body":
                                var body = PatchedBody(parameterModel);
                                res.Val(
                                    target: "#Parameters_Body" + idSuffix,
                                    value: body,
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            default:
                                switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                                {
                                    case "Class":
                                        res.Val(
                                            target: $"#Parameters_{column.Name}{idSuffix}",
                                            value: parameterModel.GetClass(columnName: column.Name).ToResponse(
                                                context: context,
                                                ss: ss,
                                                column: column),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Num":
                                        res.Val(
                                            target: $"#Parameters_{column.Name}{idSuffix}",
                                            value: parameterModel.GetNum(columnName: column.Name).ToResponse(
                                                context: context,
                                                ss: ss,
                                                column: column),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Date":
                                        res.Val(
                                            target: $"#Parameters_{column.Name}{idSuffix}",
                                            value: parameterModel.GetDate(columnName: column.Name).ToResponse(
                                                context: context,
                                                ss: ss,
                                                column: column),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Description":
                                        res.Val(
                                            target: $"#Parameters_{column.Name}{idSuffix}",
                                            value: parameterModel.GetDescription(columnName: column.Name).ToResponse(
                                                context: context,
                                                ss: ss,
                                                column: column),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Check":
                                        res.Val(
                                            target: $"#Parameters_{column.Name}{idSuffix}",
                                            value: parameterModel.GetCheck(columnName: column.Name),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Attachments":
                                        res.ReplaceAll(
                                            target: $"#Parameters_{column.Name}Field",
                                            value: new HtmlBuilder()
                                                .FieldAttachments(
                                                    context: context,
                                                    fieldId: $"Parameters_{column.Name}Field",
                                                    controlId: $"Parameters_{column.Name}",
                                                    columnName: column.ColumnName,
                                                    fieldCss: column.FieldCss
                                                        + (
                                                            column.TextAlign switch
                                                            {
                                                                SiteSettings.TextAlignTypes.Right => " right-align",
                                                                SiteSettings.TextAlignTypes.Center => " center-align",
                                                                _ => string.Empty
                                                            }),
                                                    fieldDescription: column.Description,
                                                    labelText: column.LabelText,
                                                    value: parameterModel.GetAttachments(columnName: column.Name).ToJson(),
                                                    readOnly: Permissions.ColumnPermissionType(
                                                        context: context,
                                                        ss: ss,
                                                        column: column,
                                                        baseModel: parameterModel)
                                                            != Permissions.ColumnPermissionTypes.Update,
                                                    allowDelete: column.AllowDeleteAttachments != false,
                                                    validateRequired: column.ValidateRequired != false,
                                                    inputGuide: column.InputGuide),
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
            var copyFrom = 0;
            var parameterModel = new ParameterModel(
                context: context,
                ss: ss,
                parameterId: copyFrom,
                formData: context.Forms);
            parameterModel.ParameterId = 0;
            parameterModel.Ver = 1;
            var invalid = ParameterValidators.OnCreating(
                context: context,
                ss: ss,
                parameterModel: parameterModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var processes = (List<Process>)null;
            var errorData = parameterModel.Create(context: context, ss: ss);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    SessionUtilities.Set(
                        context: context,
                        message: CreatedMessage(
                            context: context,
                            ss: ss,
                            parameterModel: parameterModel,
                            processes: processes));
                    return new ResponseCollection(
                        context: context,
                        id: parameterModel.ParameterId)
                            .SetMemory("formChanged", false)
                            .Messages(context.Messages)
                            .AfterCreate(
                                ss: ss,
                                id: parameterModel.ParameterId)
                            .ToJson();
                default:
                    return errorData.MessageJson(context: context);
            }
        }

        private static Message CreatedMessage(
            Context context,
            SiteSettings ss,
            ParameterModel parameterModel,
            List<Process> processes)
        {
            var process = processes?.FirstOrDefault(o => !o.SuccessMessage.IsNullOrEmpty()
                && o.MatchConditions);
            if (process == null)
            {
                return Messages.Created(
                    context: context,
                    data: parameterModel.Title.MessageDisplay(context: context));
            }
            else
            {
                var message = process.GetSuccessMessage(context: context);
                message.Text = parameterModel.ReplacedDisplayValues(
                    context: context,
                    ss: ss,
                    value: message.Text);
                return message;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Update(Context context, SiteSettings ss, int parameterId)
        {
            if (!context.CanUpdate(ss: ss))
            {
                return Error.Types.NotFound.MessageJson(context: context);
            }
            var title = context.Forms.Data("Parameters_Title");
            ParameterModel parameterModel;
            try
            {
                parameterModel = new ParameterModel(
                    context: context,
                    ss: ss,
                    title: title,
                    setByForm: true);
            }
            catch (Newtonsoft.Json.JsonException)
            {
                return Error.Types.JsonParseError.MessageJson(context: context);
            }
            catch (Exception)
            {
                throw;
            }
            var invalid = ParameterValidators.OnUpdating(
                context: context,
                ss: ss,
                parameterModel: parameterModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var processes = (List<Process>)null;
            var errorData = parameterModel.AccessStatus == Databases.AccessStatuses.NotFound
                ? parameterModel.Create(
                    context: context,
                    ss: ss)
                : parameterModel.Update(
                    context: context,
                    ss: ss);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    var res = new ParametersResponseCollection(
                        context: context,
                        parameterModel: parameterModel);
                    return ResponseByUpdate(res, context, ss, parameterModel, processes)
                        .PrependComment(
                            context: context,
                            ss: ss,
                            column: ss.GetColumn(context: context, columnName: "Comments"),
                            comments: parameterModel.Comments,
                            verType: parameterModel.VerType)
                        .ToJson();
                case Error.Types.UpdateConflicts:
                    return Messages.ResponseUpdateConflicts(
                        context: context,
                        data: parameterModel.Updator.Name)
                            .ToJson();
                default:
                    return errorData.MessageJson(context: context);
            }
        }

        private static ResponseCollection ResponseByUpdate(
            ParametersResponseCollection res,
            Context context,
            SiteSettings ss,
            ParameterModel parameterModel,
            List<Process> processes)
        {
            ss.ClearColumnAccessControlCaches(baseModel: parameterModel);
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
                    where: Rds.ParametersWhere().ParameterId(parameterModel.ParameterId));
                var columns = ss.GetGridColumns(
                    context: context,
                    view: view,
                    checkPermission: true);
                return res
                    .ReplaceAll(
                        $"[data-id=\"{parameterModel.ParameterId}\"][data-latest]",
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
                        parameterModel: parameterModel,
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
                    .FieldResponse(context: context, ss: ss, parameterModel: parameterModel)
                    .Val("#VerUp", verUp)
                    .Val("#Ver", parameterModel.Ver)
                    .Disabled("#VerUp", verUp)
                    .Html("#HeaderTitle", HttpUtility.HtmlEncode(parameterModel.Title.Value))
                    .Html("#RecordInfo", new HtmlBuilder().RecordInfo(
                        context: context,
                        baseModel: parameterModel,
                        tableName: "Parameters"))
                    .SetMemory("formChanged", false)
                    .Message(Messages.Updated(
                        context: context,
                        data: parameterModel.Title.Value))
                    .Messages(context.Messages)
                    .Comment(
                        context: context,
                        ss: ss,
                        column: ss.GetColumn(context: context, columnName: "Comments"),
                        comments: parameterModel.Comments,
                        deleteCommentId: parameterModel.DeleteCommentId)
                    .ClearFormData();
            }
        }

        private static Message UpdatedMessage(
            Context context,
            SiteSettings ss,
            ParameterModel parameterModel,
            List<Process> processes)
        {
            var process = processes?.FirstOrDefault(o => !o.SuccessMessage.IsNullOrEmpty()
                && o.MatchConditions);
            if (process == null)
            {
                return Messages.Updated(
                    context: context,
                    data: parameterModel.Title.MessageDisplay(context: context));
            }
            else
            {
                var message = process.GetSuccessMessage(context: context);
                message.Text = parameterModel.ReplacedDisplayValues(
                    context: context,
                    ss: ss,
                    value: message.Text);
                return message;
            }
        }

        public static string Delete(Context context, SiteSettings ss, int parameterId)
        {
            var parameterModel = new ParameterModel(context, ss, parameterId);
            var invalid = ParameterValidators.OnDeleting(
                context: context,
                ss: ss,
                parameterModel: parameterModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var errorData = parameterModel.Delete(context: context, ss: ss);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    SessionUtilities.Set(
                        context: context,
                        message: Messages.Deleted(
                            context: context,
                            data: parameterModel.Title.MessageDisplay(context: context)));
                    var res = new ParametersResponseCollection(
                        context: context,
                        parameterModel: parameterModel);
                    res
                        .SetMemory("formChanged", false)
                        .Invoke("back");
                    return res.ToJson();
                default:
                    return errorData.MessageJson(context: context);
            }
        }

        public static string Histories(
            Context context, SiteSettings ss, int parameterId, Message message = null)
        {
            var parameterModel = new ParameterModel(context: context, ss: ss, parameterId: parameterId);
            var columns = ss.GetHistoryColumns(context: context, checkPermission: true);
            if (!context.CanRead(ss: ss))
            {
                return Error.Types.HasNotPermission.MessageJson(context: context);
            }
            var hb = new HtmlBuilder();
            hb.Div(
                css: "tabs-panel-inner",
                action: () => hb
                    .HistoryCommands(context: context, ss: ss)
                    .GridTable(
                        context: context,
                        css: "history",
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
                                    parameterModel: parameterModel))));
            return new ParametersResponseCollection(
                context: context,
                parameterModel: parameterModel)
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
            ParameterModel parameterModel)
        {
            if (ss.ColumnHash.ContainsKey("TitleBody") && ss.ColumnHash.ContainsKey("Body"))
            {
                ss.ColumnHash["TitleBody"].ControlType = ss.ColumnHash["Body"].FieldCss == "field-rte" ? "RTEditor" : "MarkDown";
            }
            new ParameterCollection(
                context: context,
                ss: ss,
                column: HistoryColumn(columns),
                where: Rds.ParametersWhere().ParameterId(parameterModel.ParameterId),
                orderBy: Rds.ParametersOrderBy().Ver(SqlOrderBy.Types.desc),
                tableType: Sqls.TableTypes.NormalAndHistory)
                    .ForEach(parameterModelHistory => hb
                        .Tr(
                            attributes: new HtmlAttributes()
                                .Class("grid-row")
                                .DataAction("History")
                                .DataMethod("post")
                                .DataVer(parameterModelHistory.Ver)
                                .DataLatest(
                                    value: 1,
                                    _using: parameterModelHistory.Ver == parameterModel.Ver),
                            action: () =>
                            {
                                hb.Td(
                                    css: "grid-check-td",
                                    action: () => hb
                                        .CheckBox(
                                            controlCss: "grid-check",
                                            _checked: false,
                                            dataId: parameterModelHistory.Ver.ToString(),
                                            _using: parameterModelHistory.Ver < parameterModel.Ver));
                                columns.ForEach(column => hb
                                    .TdValue(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        parameterModel: parameterModelHistory));
                            }));
        }

        private static SqlColumnCollection HistoryColumn(List<Column> columns)
        {
            var sqlColumn = new Rds.ParametersColumnCollection()
                .ParameterId()
                .Ver();
            columns.ForEach(column =>
                sqlColumn.ParametersColumn(columnName: column.ColumnName));
            return sqlColumn;
        }

        public static string History(Context context, SiteSettings ss, int parameterId)
        {
            var parameterModel = new ParameterModel(context: context, ss: ss, parameterId: parameterId);
            parameterModel.Get(
                context: context,
                ss: ss,
                where: Rds.ParametersWhere()
                    .ParameterId(parameterModel.ParameterId)
                    .Ver(context.Forms.Int("Ver")),
                tableType: Sqls.TableTypes.NormalAndHistory);
            parameterModel.VerType = context.Forms.Bool("Latest")
                ? Versions.VerTypes.Latest
                : Versions.VerTypes.History;
            return EditorResponse(context, ss, parameterModel)
                .PushState("History", Locations.Get(
                    context: context,
                    parts: new string[]
                    {
                        context.Controller,
                        parameterId.ToString() 
                            + (parameterModel.VerType == Versions.VerTypes.History
                                ? "?ver=" + context.Forms.Int("Ver") 
                                : string.Empty)
                    }))
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string PatchedBody(ParameterModel parameterModel)
        {
            var typeName = parameterModel.Title.Value;
            var json = Parameters.ParameterHash.Get(typeName);
            var body = parameterModel.Body.IsNullOrEmpty()
                ? json
                : Jsons.ApplyPatch(
                    original: json,
                    patch: parameterModel.Body,
                    typeName: typeName);
            if (typeName == "Security")
            {
                body = Jsons.MergeArrayUnion(
                    original: json,
                    target: body,
                    propertyName: "PrivilegedUsers");
            }
            return body;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Editor(Context context, SiteSettings ss)
        {
            if (!context.CanRead(ss: ss))
            {
                return HtmlTemplates.Error(
                    context: context,
                    errorData: new ErrorData(type: Error.Types.NotFound));
            }
            var title = context.Forms.Data("Parameters_Title");
            if (!Parameters.ParameterHash.ContainsKey(title))
            {
                title = Parameters.ParameterHash.Keys.FirstOrDefault();
            }
            var parameterModel = new ParameterModel(
                context: context,
                ss: ss,
                title: title);
            parameterModel.SwitchTargets = GetSwitchTargets(
                context: context,
                ss: SiteSettingsUtilities.ParametersSiteSettings(context: context),
                parameterId: parameterModel.ParameterId);
            return Editor(context: context, ss: ss, parameterModel: parameterModel);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static void RequestTenantRestart(Context context)
        {
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.UpdateTenants(
                    param: Rds.TenantsParam()
                        .RestartScheduledTime(value: DateTime.UtcNow),
                    where: Rds.TenantsWhere()
                        .TenantId(context.TenantId)));
        }
    }
}
