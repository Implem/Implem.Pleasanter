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
    public static class WikiUtilities
    {
        public static HtmlBuilder TdValue(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Column column,
            WikiModel wikiModel,
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
                    wikiModel: wikiModel);
            }
            else
            {
                var mine = wikiModel.Mine(context: context);
                switch (column.Name)
                {
                    case "SiteId":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: wikiModel.SiteId,
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
                                    value: wikiModel.UpdatedTime,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "WikiId":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: wikiModel.WikiId,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "Ver":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: wikiModel.Ver,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "Title":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: wikiModel.Title,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "Body":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: wikiModel.Body,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "TitleBody":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: wikiModel.TitleBody,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "Locked":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: wikiModel.Locked,
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
                                    value: wikiModel.Comments,
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
                                    value: wikiModel.Creator,
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
                                    value: wikiModel.Updator,
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
                                    value: wikiModel.CreatedTime,
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
                                            value: wikiModel.GetClass(columnName: column.Name),
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
                                            value: wikiModel.GetNum(columnName: column.Name),
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
                                            value: wikiModel.GetDate(columnName: column.Name),
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
                                            value: wikiModel.GetDescription(columnName: column.Name),
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
                                            value: wikiModel.GetCheck(columnName: column.Name),
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
                                            value: wikiModel.GetAttachments(columnName: column.Name),
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
            WikiModel wikiModel)
        {
            ss.IncludedColumns(gridDesign).ForEach(column =>
            {
                var value = string.Empty;
                switch (column.Name)
                {
                    case "SiteId": value = wikiModel.SiteId.GridText(
                        context: context,
                        column: column); break;
                    case "UpdatedTime": value = wikiModel.UpdatedTime.GridText(
                        context: context,
                        column: column); break;
                    case "WikiId": value = wikiModel.WikiId.GridText(
                        context: context,
                        column: column); break;
                    case "Ver": value = wikiModel.Ver.GridText(
                        context: context,
                        column: column); break;
                    case "Title": value = wikiModel.Title.GridText(
                        context: context,
                        column: column); break;
                    case "Body": value = wikiModel.Body.GridText(
                        context: context,
                        column: column); break;
                    case "TitleBody": value = wikiModel.TitleBody.GridText(
                        context: context,
                        column: column); break;
                    case "Locked": value = wikiModel.Locked.GridText(
                        context: context,
                        column: column); break;
                    case "Comments": value = wikiModel.Comments.GridText(
                        context: context,
                        column: column); break;
                    case "Creator": value = wikiModel.Creator.GridText(
                        context: context,
                        column: column); break;
                    case "Updator": value = wikiModel.Updator.GridText(
                        context: context,
                        column: column); break;
                    case "CreatedTime": value = wikiModel.CreatedTime.GridText(
                        context: context,
                        column: column); break;
                    default:
                        switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                        {
                            case "Class":
                                value = wikiModel.GetClass(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Num":
                                value = wikiModel.GetNum(columnName: column.Name)?.Value?.GridText(
                                    context: context,
                                    column: column) ?? string.Empty;
                                break;
                            case "Date":
                                value = wikiModel.GetDate(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Description":
                                value = wikiModel.GetDescription(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Check":
                                value = wikiModel.GetCheck(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Attachments":
                                value = wikiModel.GetAttachments(columnName: column.Name).GridText(
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

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string EditorNew(Context context, SiteSettings ss)
        {
            var wikiId = Repository.ExecuteScalar_long(
                context: context,
                statements: Rds.SelectWikis(
                    column: Rds.WikisColumn().WikiId(),
                    where: Rds.WikisWhere().SiteId(ss.SiteId)));
            return Editor(
                context: context,
                ss: ss,
                wikiModel: new WikiModel(
                    context: context,
                    ss: ss,
                    methodType: BaseModel.MethodTypes.New));
        }

        public static string Editor(
            Context context, SiteSettings ss, long wikiId, bool clearSessions)
        {
            var wikiModel = new WikiModel(
                context: context,
                ss: ss,
                wikiId: wikiId,
                clearSessions: clearSessions,
                methodType: BaseModel.MethodTypes.Edit);
            return Editor(
                context: context,
                ss: ss,
                wikiModel: wikiModel);
        }

        public static string Editor(
            Context context,
            SiteSettings ss,
            WikiModel wikiModel,
            bool editInDialog = false)
        {
            var invalid = WikiValidators.OnEditing(
                context: context,
                ss: ss,
                wikiModel: wikiModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return HtmlTemplates.Error(
                    context: context,
                    errorData: invalid);
            }
            var hb = new HtmlBuilder();
            var serverScriptModelRow = ss.GetServerScriptModelRow(
                context: context,
                itemModel: wikiModel);
            return editInDialog
                ? hb.DialogEditorForm(
                    context: context,
                    ss: ss,
                    siteId: wikiModel.SiteId,
                    referenceId: wikiModel.WikiId,
                    isHistory: wikiModel.VerType == Versions.VerTypes.History,
                    action: () => hb.EditorInDialog(
                        context: context,
                        ss: ss,
                        wikiModel: wikiModel,
                        editInDialog: editInDialog))
                            .ToString()
                : hb.Template(
                    context: context,
                    ss: ss,
                    view: Views.GetBySession(
                        context: context,
                        ss: ss),
                    siteId: wikiModel.SiteId,
                    parentId: ss.ParentId,
                    referenceType: "Wikis",
                    title: wikiModel.MethodType == BaseModel.MethodTypes.New
                        ? Displays.New(context: context)
                        : wikiModel.Title.MessageDisplay(context: context),
                    body: wikiModel.Body,
                    useTitle: ss.TitleColumns?.Any(o => ss
                        .GetEditorColumnNames()
                        .Contains(o)) == true,
                    userScript: ss.EditorScripts(
                        context: context, methodType: wikiModel.MethodType),
                    userStyle: ss.EditorStyles(
                        context: context, methodType: wikiModel.MethodType),
                    methodType: wikiModel.MethodType,
                    serverScriptModelRow: serverScriptModelRow,
                    action: () => hb
                        .Editor(
                            context: context,
                            ss: ss,
                            wikiModel: wikiModel,
                            serverScriptModelRow: serverScriptModelRow)
                        .Hidden(controlId: "DropDownSearchPageSize", value: Parameters.General.DropDownSearchPageSize.ToString()))
                            .ToString();
        }

        private static HtmlBuilder EditorInDialog(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            WikiModel wikiModel,
            bool editInDialog)
        {
            if (ss.Tabs?.Any() != true)
            {
                hb.FieldSetGeneral(
                    context: context,
                    ss: ss,
                    wikiModel: wikiModel,
                    editInDialog: editInDialog);
            }
            else
            {
                hb.Div(
                    id: "EditorTabsContainer",
                    css: "tab-container max",
                    attributes: new HtmlAttributes().TabActive(context: context),
                    action: () => hb
                        .EditorTabs(
                            context: context,
                            ss: ss,
                            wikiModel: wikiModel,
                            editInDialog: editInDialog)
                        .FieldSetGeneral(
                            context: context,
                            ss: ss,
                            wikiModel: wikiModel,
                            editInDialog: editInDialog)
                        .FieldSetTabs(
                            context: context,
                            ss: ss,
                            id: wikiModel.WikiId,
                            wikiModel: wikiModel,
                            editInDialog: editInDialog));
            }
            return hb.Hidden(
                controlId: "EditorInDialogRecordId",
                value: context.Id.ToString());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Editor(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            WikiModel wikiModel,
            ServerScriptModelRow serverScriptModelRow)
        {
            var commentsColumn = ss.GetColumn(
                context: context,
                columnName: "Comments");
            var commentsColumnPermissionType = Permissions.ColumnPermissionType(
                context: context,
                ss: ss,
                column: commentsColumn,
                baseModel: wikiModel);
            var showComments = ss.ShowComments(commentsColumnPermissionType);
            var tabsCss = showComments ? null : "max";
            return hb.Div(id: "Editor", action: () => hb
                .Form(
                    attributes: new HtmlAttributes()
                        .Id("MainForm")
                        .Class("main-form")
                        .Action(Locations.ItemAction(
                            context: context,
                            id: wikiModel.WikiId != 0
                                ? wikiModel.WikiId
                                : wikiModel.SiteId)),
                    action: () => hb
                        .RecordHeader(
                            context: context,
                            ss: ss,
                            baseModel: wikiModel,
                            tableName: "Wikis")
                        .Div(
                            id: "EditorComments", action: () => hb
                                .Comments(
                                    context: context,
                                    ss: ss,
                                    comments: wikiModel.Comments,
                                    column: commentsColumn,
                                    verType: wikiModel.VerType,
                                    columnPermissionType: commentsColumnPermissionType),
                            _using: showComments)
                        .Div(
                            id: "EditorTabsContainer",
                            css: "tab-container " + tabsCss,
                            action: () => hb
                                .EditorTabs(
                                    context: context,
                                    ss: ss,
                                    wikiModel: wikiModel)
                                .FieldSetGeneral(
                                    context: context,
                                    ss: ss,
                                    wikiModel: wikiModel)
                                .FieldSet(
                                    attributes: new HtmlAttributes()
                                        .Id("FieldSetHistories")
                                        .DataAction("Histories")
                                        .DataMethod("post"),
                                    _using: wikiModel.MethodType != BaseModel.MethodTypes.New)
                                .FieldSet(
                                    attributes: new HtmlAttributes()
                                        .Id("FieldSetRecordAccessControl")
                                        .DataAction("Permissions")
                                        .DataMethod("post"),
                                    _using: context.CanManagePermission(ss: ss))
                                .MainCommands(
                                    context: context,
                                    ss: ss,
                                    verType: wikiModel.VerType,
                                    updateButton: true,
                                    copyButton: false,
                                    moveButton: false,
                                    mailButton: true,
                                    deleteButton: true,
                                    serverScriptModelRow: serverScriptModelRow))
                        .Hidden(
                            controlId: "BaseUrl",
                            value: Locations.BaseUrl(context: context))
                        .Hidden(
                            controlId: "Ver",
                            value: wikiModel.Ver.ToString())
                        .Hidden(
                            controlId: "MethodType",
                            value: "edit")
                        .Hidden(
                            controlId: "Wikis_Timestamp",
                            css: "always-send",
                            value: wikiModel.Timestamp)
                        .Hidden(
                            controlId: "SwitchTargets",
                            css: "always-send",
                            value: wikiModel.WikiId.ToString(),
                            _using: !context.Ajax || context.Action == "create")
                        .Hidden(
                            controlId: "TriggerRelatingColumns_Editor", 
                            value: Jsons.ToJson(ss.RelatingColumns))
                        .PostInitHiddenData(context: context))
                .OutgoingMailsForm(
                    context: context,
                    ss: ss,
                    referenceType: "Wikis",
                    referenceId: wikiModel.WikiId,
                    referenceVer: wikiModel.Ver)
                .CopyDialog(
                    context: context,
                    ss: ss)
                .MoveDialog(context: context)
                .OutgoingMailDialog());
        }

        private static HtmlBuilder PostInitHiddenData(
            this HtmlBuilder hb,
            Context context)
        {
            var postInitData = context.Forms.Where(o => o.Key.StartsWith("PostInit_"));
            postInitData.ForEach(data =>
            {
                hb.Hidden(
                    controlId: data.Key,
                    value: data.Value,
                    css: "always-send");
            });
            return hb;
        }

        private static HtmlBuilder EditorTabs(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            WikiModel wikiModel,
            bool editInDialog = false)
        {
            return hb.Ul(id: "EditorTabs", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetGeneral",
                        text: ss.GeneralTabLabelText))
                .Tabs(
                    context: context,
                    ss: ss)
                .Li(
                    _using: wikiModel.MethodType != BaseModel.MethodTypes.New
                        && !context.Publish
                        && !editInDialog,
                    action: () => hb
                        .A(
                            href: "#FieldSetHistories",
                            text: Displays.ChangeHistoryList(context: context)))
                .Li(
                    _using: context.CanManagePermission(ss: ss)
                        && !ss.Locked()
                        && wikiModel.MethodType != BaseModel.MethodTypes.New
                        && !editInDialog
                        && ss.ReferenceType != "Wikis",
                    action: () => hb
                        .A(
                            href: "#FieldSetRecordAccessControl",
                            text: Displays.RecordAccessControl(context: context))));
        }

        private static HtmlBuilder FieldSetGeneral(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            WikiModel wikiModel,
            DataSet dataSet = null,
            List<Link> links = null,
            bool editInDialog = false)
        {
            var mine = wikiModel.Mine(context: context);
            return hb.FieldSet(id: "FieldSetGeneral", action: () => hb
                .FieldSetGeneralColumns(
                    context: context,
                    ss: ss,
                    wikiModel: wikiModel,
                    dataSet: dataSet,
                    links: links,
                    editInDialog: editInDialog));
        }

        public static HtmlBuilder FieldSetGeneralColumns(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            WikiModel wikiModel,
            DataSet dataSet = null,
            List<Link> links = null,
            bool preview = false,
            bool editInDialog = false)
        {
            hb.Fields(
                context: context,
                ss: ss,
                id: wikiModel.WikiId,
                wikiModel: wikiModel,
                dataSet: dataSet,
                links: links,
                preview: preview,
                editInDialog: editInDialog);
            if (!preview)
            {
                hb.VerUpCheckBox(
                    context: context,
                    ss: ss,
                    baseModel: wikiModel);
            }
            return hb;
        }

        public static HtmlBuilder Field(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            WikiModel wikiModel,
            Column column,
            bool controlOnly = false,
            bool alwaysSend = false,
            bool disableAutoPostBack = false,
            string idSuffix = null,
            bool preview = false,
            bool disableSection = false)
        {
            var value = wikiModel.ControlValue(
                context: context,
                ss: ss,
                column: column);
            if (value != null)
            {
                //数値項目の場合、「単位」を値に連結する
                value += wikiModel.NumUnit(
                    context: context,
                    ss: ss,
                    column: column);
                SetChoiceHashByFilterExpressions(
                    context: context,
                    ss: ss,
                    column: column,
                    wikiModel: wikiModel);
                hb.Field(
                    context: context,
                    ss: ss,
                    column: column,
                    serverScriptModelColumn: wikiModel
                        ?.ServerScriptModelRow
                        ?.Columns.Get(column.ColumnName),
                    value: value,
                    columnPermissionType: Permissions.ColumnPermissionType(
                        context: context,
                        ss: ss,
                        column: column,
                        baseModel: wikiModel),
                    controlOnly: controlOnly,
                    alwaysSend: alwaysSend,
                    disableAutoPostBack: disableAutoPostBack,
                    idSuffix: idSuffix,
                    preview: preview,
                    disableSection: disableSection);
            }
            return hb;
        }

        private static HtmlBuilder Tabs(this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            ss.Tabs?.ForEach(tab => hb.Li(action: () => hb.A(
                href: $"#FieldSetTab{tab.Id}",
                action: () => hb.Label(action: () => hb.Text(tab.LabelText)))));
            return hb;
        }

        private static HtmlBuilder FieldSetTabs(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            long id,
            WikiModel wikiModel,
            DataSet dataSet = null,
            List<Link> links = null,
            bool preview = false,
            bool editInDialog = false)
        {
            dataSet = dataSet ?? HtmlLinks.DataSet(
                context: context,
                ss: ss,
                id: id);
            links = links ?? HtmlLinkCreations.Links(
                context: context,
                ss: ss);
            ss.Tabs?.Select((tab, index) => new { tab = tab, index = index + 1 })?.ForEach(data =>
            {
                hb.FieldSet(
                    id: $"FieldSetTab{data.tab.Id}",
                    css: " fieldset cf ui-tabs-panel ui-corner-bottom ui-widget-content ",
                    action: () => hb.Fields(
                        context: context,
                        ss: ss,
                        id: id,
                        tab: data.tab,
                        dataSet: dataSet,
                        links: links,
                        preview: preview,
                        editInDialog: editInDialog,
                        wikiModel: wikiModel,
                        tabIndex: data.index));
            });
            return hb;
        }

        private static HtmlBuilder Fields(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            long id,
            WikiModel wikiModel,
            DataSet dataSet = null,
            List<Link> links = null,
            bool preview = false,
            bool editInDialog = false)
        {
            return hb.Fields(
                context: context,
                ss: ss,
                id: id,
                tab: new Tab { Id = 0 },
                dataSet: !preview
                    ? dataSet ?? HtmlLinks.DataSet(
                        context: context,
                        ss: ss,
                        id: id)
                    : null,
                links: links ?? HtmlLinkCreations.Links(
                    context: context,
                    ss: ss),
                wikiModel: wikiModel,
                preview: preview,
                editInDialog: editInDialog);
        }

        private static HtmlBuilder Fields(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            long id,
            Tab tab,
            DataSet dataSet,
            List<Link> links,
            WikiModel wikiModel,
            bool preview = false,
            bool editInDialog = false,
            int tabIndex = 0)
        {
            ss
                .GetEditorColumns(
                    context: context,
                    tab: tab,
                    columnOnly: false)
                ?.Aggregate(new List<KeyValuePair<Section, List<string>>>(), (columns, column) =>
                {
                    var sectionId = ss.SectionId(column.ColumnName);
                    var section = ss
                        .Sections
                        ?.FirstOrDefault(o => o.Id == sectionId);
                    if (section != null)
                    {
                        columns.Add(new KeyValuePair<Section, List<string>>(
                            new Section
                            {
                                Id = section.Id,
                                LabelText = section.LabelText,
                                AllowExpand = section.AllowExpand,
                                Expand = section.Expand,
                                Hide = section.Hide
                            },
                            new List<string>()));
                    }
                    else
                    {
                        if (!columns.Any())
                        {
                            columns.Add(new KeyValuePair<Section, List<string>>(
                                null,
                                new List<string>()));
                        }
                        columns.Last().Value.Add(column.ColumnName);
                    }
                    return columns;
                }).ForEach(section =>
                {
                    if (section.Key == null)
                    {
                        hb.Fields(
                            context: context,
                            ss: ss,
                            id: id,
                            columnNames: section.Value,
                            dataSet: dataSet,
                            links: links,
                            wikiModel: wikiModel,
                            preview: preview,
                            editInDialog: editInDialog,
                            tabIndex: tabIndex);
                    }
                    else if (section.Key.Hide != true)
                    {
                        hb
                            .Div(
                                id: $"SectionFields{section.Key.Id}Container",
                                css: "section-fields-container",
                                action: () => hb
                                    .Div(action: () => hb.Label(
                                        css: "field-section" + (section.Key.AllowExpand == true
                                            ? " expand"
                                            : string.Empty),
                                        attributes: new HtmlAttributes()
                                            .For($"SectionFields{section.Key.Id}"),
                                        action: () => hb
                                            .Span(css: section.Key.AllowExpand == true
                                                ? section.Key.Expand == true
                                                    ? "ui-icon ui-icon-triangle-1-s"
                                                    : "ui-icon ui-icon-triangle-1-e"
                                                : string.Empty)
                                            .Text(text: section.Key.LabelText)))
                                    .Div(
                                        id: $"SectionFields{section.Key.Id}",
                                        css: section.Key.AllowExpand == true && section.Key.Expand != true
                                            ? "section-fields hidden"
                                            : "section-fields",
                                        action: () => hb.Fields(
                                            context: context,
                                            ss: ss,
                                            id: id,
                                            columnNames: section.Value,
                                            dataSet: dataSet,
                                            links: links,
                                            wikiModel: wikiModel,
                                            preview: preview,
                                            editInDialog: editInDialog,
                                            tabIndex: tabIndex)));
                    }
                });
            return hb;
        }

        private static HtmlBuilder Fields(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            long id,
            List<string> columnNames,
            DataSet dataSet,
            List<Link> links,
            WikiModel wikiModel,
            bool preview = false,
            bool editInDialog = false,
            int tabIndex = 0)
        {
            columnNames.ForEach(columnName => hb.Field(
                context: context,
                ss: ss,
                id: id,
                columnName: columnName,
                dataSet: dataSet,
                links: links,
                wikiModel: wikiModel,
                preview: preview,
                editInDialog: editInDialog,
                tabIndex: tabIndex));
            return hb;
        }

        private static HtmlBuilder Field(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            long id,
            string columnName,
            DataSet dataSet,
            List<Link> links,
            WikiModel wikiModel,
            bool preview = false,
            bool editInDialog = false,
            int tabIndex = 0)
        {
            var column = ss.GetColumn(
                context: context,
                columnName: columnName);
            var linkId = !preview && !editInDialog ? ss.LinkId(columnName) : 0;
            if (column != null)
            {
                hb.Field(
                    context: context,
                    ss: ss,
                    wikiModel: wikiModel,
                    column: column,
                    preview: preview);
            }
            else if (!editInDialog && linkId != 0)
            {
                hb.LinkField(
                    context: context,
                    ss: ss,
                    id: wikiModel.WikiId,
                    linkId: linkId,
                    links: links,
                    dataSet: dataSet,
                    methodType: wikiModel?.MethodType,
                    tabIndex: tabIndex);
            }
            return hb;
        }

        private static HtmlAttributes TabActive(
            this HtmlAttributes attributes,
            Context context)
        {
            var tabIndex = context.QueryStrings.Get("TabIndex").ToInt();
            return attributes.Add(
                name: "tab-active",
                value: tabIndex.ToString(),
                _using: tabIndex > 0);
        }

        public static string NumUnit(
            this WikiModel wikiModel,
            Context context,
            SiteSettings ss,
            Column column)
        {
            if (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty) != "Num"
                || column.ControlType == "Spinner")
            {
                return string.Empty;
            }
            return (column.GetEditorReadOnly()
                || Permissions.ColumnPermissionType(
                    context: context,
                    ss: ss,
                    column: column,
                    baseModel: wikiModel) != Permissions.ColumnPermissionTypes.Update
                        ? column.Unit
                        : string.Empty);
        }

        public static string ControlValue(
            this WikiModel wikiModel,
            Context context,
            SiteSettings ss,
            Column column)
        {
            switch (column.Name)
            {
                case "WikiId":
                    return wikiModel.WikiId
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Ver":
                    return wikiModel.Ver
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Title":
                    return wikiModel.Title
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Body":
                    return wikiModel.Body
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Locked":
                    return wikiModel.Locked
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                default:
                    switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                    {
                        case "Class":
                            return wikiModel.GetClass(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Num":
                            return wikiModel.GetNum(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Date":
                            return wikiModel.GetDate(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Description":
                            return wikiModel.GetDescription(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Check":
                            return wikiModel.GetCheck(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Attachments":
                            return wikiModel.GetAttachments(columnName: column.Name)
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
            WikiModel wikiModel)
        {
            return hb;
        }

        private static HtmlBuilder EditorExtensions(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            WikiModel wikiModel)
        {
            return hb;
        }

        public static string EditorJson(Context context, SiteSettings ss, long wikiId)
        {
            return EditorResponse(context, ss, new WikiModel(
                context, ss, wikiId,
                formData: context.Forms)).ToJson();
        }

        private static ResponseCollection EditorResponse(
            Context context,
            SiteSettings ss,
            WikiModel wikiModel,
            Message message = null,
            string switchTargets = null)
        {
            wikiModel.MethodType = wikiModel.WikiId == 0
                ? BaseModel.MethodTypes.New
                : BaseModel.MethodTypes.Edit;
            return new WikisResponseCollection(
                context: context,
                wikiModel: wikiModel)
                    .Invoke("clearDialogs")
                    .ReplaceAll("#MainContainer", Editor(context, ss, wikiModel))
                    .Val("#SwitchTargets", switchTargets, _using: switchTargets != null)
                    .SetMemory("formChanged", false)
                    .Invoke("setCurrentIndex")
                    .Message(message)
                    .Messages(context.Messages)
                    .ClearFormData(_using: !context.QueryStrings.Bool("control-auto-postback"));
        }

        public static ResponseCollection FieldResponse(
            this ResponseCollection res,
            Context context,
            SiteSettings ss,
            WikiModel wikiModel,
            string idSuffix = null)
        {
            var replaceFieldColumns = ss.ReplaceFieldColumns(
                context: context,
                serverScriptModelRow: wikiModel.ServerScriptModelRow);
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
                    var serverScriptModelColumn = wikiModel
                        ?.ServerScriptModelRow
                        ?.Columns.Get(column.ColumnName);
                    if (replaceFieldColumns?.Contains(column.ColumnName) == true)
                    {
                        res.ReplaceAll(
                            target: $"#Wikis_{column.Name}Field" + idSuffix,
                            value: new HtmlBuilder().Field(
                                context: context,
                                ss: ss,
                                wikiModel: wikiModel,
                                column: column,
                                idSuffix: idSuffix));
                    }
                    else
                    {
                        switch (column.Name)
                        {
                            case "WikiId":
                                res.Val(
                                    target: "#Wikis_WikiId" + idSuffix,
                                    value: wikiModel.WikiId.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "Title":
                                res.Val(
                                    target: "#Wikis_Title" + idSuffix,
                                    value: wikiModel.Title.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "Body":
                                res.Val(
                                    target: "#Wikis_Body" + idSuffix,
                                    value: wikiModel.Body.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "Locked":
                                res.Val(
                                    target: "#Wikis_Locked" + idSuffix,
                                    value: wikiModel.Locked,
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            default:
                                switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                                {
                                    case "Class":
                                        res.Val(
                                            target: $"#Wikis_{column.Name}{idSuffix}",
                                            value: wikiModel.GetClass(columnName: column.Name).ToResponse(
                                                context: context,
                                                ss: ss,
                                                column: column),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Num":
                                        res.Val(
                                            target: $"#Wikis_{column.Name}{idSuffix}",
                                            value: wikiModel.GetNum(columnName: column.Name).ToResponse(
                                                context: context,
                                                ss: ss,
                                                column: column),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Date":
                                        res.Val(
                                            target: $"#Wikis_{column.Name}{idSuffix}",
                                            value: wikiModel.GetDate(columnName: column.Name).ToResponse(
                                                context: context,
                                                ss: ss,
                                                column: column),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Description":
                                        res.Val(
                                            target: $"#Wikis_{column.Name}{idSuffix}",
                                            value: wikiModel.GetDescription(columnName: column.Name).ToResponse(
                                                context: context,
                                                ss: ss,
                                                column: column),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Check":
                                        res.Val(
                                            target: $"#Wikis_{column.Name}{idSuffix}",
                                            value: wikiModel.GetCheck(columnName: column.Name),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Attachments":
                                        res.ReplaceAll(
                                            target: $"#Wikis_{column.Name}Field",
                                            value: new HtmlBuilder()
                                                .FieldAttachments(
                                                    context: context,
                                                    fieldId: $"Wikis_{column.Name}Field",
                                                    controlId: $"Wikis_{column.Name}",
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
                                                    value: wikiModel.GetAttachments(columnName: column.Name).ToJson(),
                                                    readOnly: Permissions.ColumnPermissionType(
                                                        context: context,
                                                        ss: ss,
                                                        column: column,
                                                        baseModel: wikiModel)
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

        public static void SetChoiceHashByFilterExpressions(
            Context context,
            SiteSettings ss,
            Column column,
            WikiModel wikiModel,
            string searchText = null,
            int offset = 0,
            bool search = false,
            bool searchFormat = false)
        {
            var link = ss.ColumnFilterExpressionsLink(
                context: context,
                column: column);
            if (link != null)
            {
                var view = link.View;
                var targetSs = ss.GetLinkedSiteSettings(
                    context: context,
                    link: link);
                if (targetSs != null)
                {
                    if (view.ColumnFilterHash == null)
                    {
                        view.ColumnFilterHash = new Dictionary<string, string>();
                    }
                    view.ColumnFilterExpressions.ForEach(data =>
                    {
                        var columnName = data.Key;
                        var targetColumn = targetSs?.GetFilterExpressionsColumn(
                            context: context,
                            link: link,
                            columnName: columnName);
                        if (targetColumn != null)
                        {
                            var expression = data.Value;
                            var raw = expression.StartsWith("=");
                            var hash = new Dictionary<string, Column>();
                            ss.IncludedColumns(value: data.Value).ForEach(includedColumn =>
                            {
                                var guid = Strings.NewGuid();
                                expression = expression.Replace(
                                    $"[{includedColumn.ExpressionColumnName()}]",
                                    guid);
                                hash.Add(guid, includedColumn);
                            });
                            hash.ForEach(hashData =>
                            {
                                var guid = hashData.Key;
                                var includedColumn = hashData.Value;
                                expression = expression.Replace(
                                    guid,
                                    includedColumn.OutputType == Column.OutputTypes.DisplayValue
                                        ? wikiModel.ToDisplay(
                                            context: context,
                                            ss: ss,
                                            column: includedColumn,
                                            mine: wikiModel.Mine(context: context))
                                        : wikiModel.ToValue(
                                            context: context,
                                            ss: ss,
                                            column: includedColumn,
                                            mine: wikiModel.Mine(context: context)));
                            });
                            view.SetColumnFilterHashByExpression(
                                ss: targetSs,
                                targetColumn: targetColumn,
                                columnName: columnName,
                                expression: expression,
                                raw: raw);
                        }
                    });
                    column.SetChoiceHash(
                        context: context,
                        ss: ss,
                        link: link,
                        searchText: searchText,
                        offset: offset,
                        search: search,
                        searchFormat: searchFormat,
                        setChoices: true);
                }
            }
        }

        public static ContentResultInheritance GetByApi(
            Context context,
            SiteSettings ss,
            long wikiId,
            bool internalRequest)
        {
            if (!Mime.ValidateOnApi(contentType: context.ContentType))
            {
                return ApiResults.BadRequest(context: context);
            }
            var invalid = WikiValidators.OnEntry(
                context: context,
                ss: ss,
                api: !internalRequest);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return ApiResults.Error(
                    context: context,
                    errorData: invalid);
            }
            var api = context.RequestDataString.Deserialize<Api>();
            if (api == null && !context.RequestDataString.IsNullOrEmpty())
            {
                return ApiResults.Error(
                    context: context,
                    errorData: new ErrorData(type: Error.Types.InvalidJsonData));
            }
            var view = api?.View ?? new View();
            var pageSize = Parameters.Api.PageSize;
            var tableType = (api?.TableType) ?? Sqls.TableTypes.Normal;
            if (wikiId > 0)
            {
                if (view.ColumnFilterHash == null)
                {
                    view.ColumnFilterHash = new Dictionary<string, string>();
                }
                view.ColumnFilterHash.Add("WikiId", wikiId.ToString());
            }
            view.MergeSession(sessionView: Views.GetBySession(
                context: context,
                ss: ss));
            switch (view.ApiDataType)
            {
                case View.ApiDataTypes.KeyValues:
                    var gridData = new GridData(
                        context: context,
                        ss: ss,
                        view: view,
                        tableType: tableType,
                        offset: api?.Offset ?? 0,
                        pageSize: pageSize);
                    return ApiResults.Get(
                        statusCode: 200,
                        limitPerDate: context.ContractSettings.ApiLimit(),
                        limitRemaining: context.ContractSettings.ApiLimit() - ss.ApiCount,
                        response: new
                        {
                            Offset = api?.Offset ?? 0,
                            PageSize = pageSize,
                            TotalCount = gridData.TotalCount,
                            Data = gridData.KeyValues(
                                context: context,
                                ss: ss,
                                view: view)
                        });
                default:
                    var wikiCollection = new WikiCollection(
                        context: context,
                        ss: ss,
                        join: Rds.ItemsJoin().Add(new SqlJoin(
                            tableBracket: "\"Items\"",
                            joinType: SqlJoin.JoinTypes.Inner,
                            joinExpression: "\"Wikis\".\"WikiId\"=\"Wikis_Items\".\"ReferenceId\"",
                            _as: "Wikis_Items")),
                        where: view.Where(
                            context: context,
                            ss: ss),
                        orderBy: view.OrderBy(
                            context: context,
                            ss: ss),
                        offset: api?.Offset ?? 0,
                        pageSize: pageSize,
                        tableType: tableType);
                    return ApiResults.Get(
                        statusCode: 200,
                        limitPerDate: context.ContractSettings.ApiLimit(),
                        limitRemaining: context.ContractSettings.ApiLimit() - ss.ApiCount,
                        response: new
                        {
                            Offset = api?.Offset ?? 0,
                            PageSize = pageSize,
                            wikiCollection.TotalCount,
                            Data = wikiCollection.Select(o => o.GetByApi(
                                context: context,
                                ss: ss))
                        });
            }
        }

        public static WikiModel[] GetByServerScript(
            Context context,
            SiteSettings ss)
        {
            var invalid = WikiValidators.OnEntry(
                context: context,
                ss: ss,
                api: true,
                serverScript: true);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    return null;
            }
            var api = context.RequestDataString.Deserialize<Api>();
            var view = api?.View ?? new View();
            var where = view.Where(
                context: context,
                ss: ss);
            var orderBy = view.OrderBy(
                context: context,
                ss: ss);
            var join = ss.Join(
                context: context,
                join: new IJoin[]
                {
                    where,
                    orderBy
                });
            var pageSize = Parameters.Api.PageSize;
            var tableType = (api?.TableType) ?? Sqls.TableTypes.Normal;
            var wikiCollection = new WikiCollection(
                context: context,
                ss: ss,
                join: join,
                where: where,
                orderBy: orderBy,
                offset: api?.Offset ?? 0,
                pageSize: pageSize,
                tableType: tableType);
            return wikiCollection.ToArray();
        }

        public static WikiModel GetByServerScript(
            Context context,
            SiteSettings ss,
            long wikiId)
        {
            var wikiModel = new WikiModel(
                context: context,
                ss: ss,
                wikiId: wikiId,
                methodType: BaseModel.MethodTypes.Edit);
            if (wikiModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return null;
            }
            var invalid = WikiValidators.OnEditing(
                context: context,
                ss: ss,
                wikiModel: wikiModel,
                api: true,
                serverScript: true);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    return null;
            }
            return wikiModel;
        }

        private static Message CreatedMessage(
            Context context,
            SiteSettings ss,
            WikiModel wikiModel,
            Process process)
        {
            if (process == null)
            {
                return Messages.Created(
                    context: context,
                    data: wikiModel.Title.DisplayValue);
            }
            else
            {
                var message = process.GetSuccessMessage(context: context);
                message.Text = wikiModel.ReplacedDisplayValues(
                    context: context,
                    ss: ss,
                    value: message.Text);
                return message;
            }
        }

        public static ContentResultInheritance CreateByApi(Context context, SiteSettings ss)
        {
            if (!Mime.ValidateOnApi(contentType: context.ContentType))
            {
                return ApiResults.BadRequest(context: context);
            }
            if (context.ContractSettings.ItemsLimit(context: context, siteId: ss.SiteId))
            {
                return ApiResults.Error(
                    context: context,
                    errorData: new ErrorData(type: Error.Types.ItemsLimit));
            }
            var wikiApiModel = context.RequestDataString.Deserialize<WikiApiModel>();
            if (wikiApiModel == null)
            {
                context.InvalidJsonData = !context.RequestDataString.IsNullOrEmpty();
            }
            var wikiModel = new WikiModel(
                context: context,
                ss: ss,
                wikiId: 0,
                wikiApiModel: wikiApiModel);
            var invalid = WikiValidators.OnCreating(
                context: context,
                ss: ss,
                wikiModel: wikiModel,
                api: true);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return ApiResults.Error(
                    context: context,
                    errorData: invalid);
            }
            wikiModel.SiteId = ss.SiteId;
            wikiModel.SetTitle(
                context: context,
                ss: ss);
            var errorData = wikiModel.Create(
                context: context,
                ss: ss,
                notice: true);
            BinaryUtilities.UploadImage(
                context: context,
                ss: ss,
                id: wikiModel.WikiId,
                postedFileHash: wikiModel.PostedImageHash);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    return ApiResults.Success(
                        id: wikiModel.WikiId,
                        limitPerDate: context.ContractSettings.ApiLimit(),
                        limitRemaining: context.ContractSettings.ApiLimit() - ss.ApiCount,
                        message: Displays.Created(
                            context: context,
                            data: wikiModel.Title.MessageDisplay(context: context)));
                default:
                    return ApiResults.Error(
                        context: context,
                        errorData: errorData);
            }
        }

        public static bool CreateByServerScript(Context context, SiteSettings ss, object model)
        {
            if (context.ContractSettings.ItemsLimit(context: context, siteId: ss.SiteId))
            {
                return false;
            }
            var wikiApiModel = context.RequestDataString.Deserialize<WikiApiModel>();
            if (wikiApiModel == null)
            {
                context.InvalidJsonData = !context.RequestDataString.IsNullOrEmpty();
            }
            var wikiModel = new WikiModel(
                context: context,
                ss: ss,
                wikiId: 0,
                wikiApiModel: wikiApiModel);
            var invalid = WikiValidators.OnCreating(
                context: context,
                ss: ss,
                wikiModel: wikiModel,
                api: true,
                serverScript: true);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    return false;
            }
            wikiModel.SiteId = ss.SiteId;
            wikiModel.SetTitle(context: context, ss: ss);
            var errorData = wikiModel.Create(
                context: context,
                ss: ss,
                notice: true);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    if (model is Libraries.ServerScripts.ServerScriptModelApiModel serverScriptModelApiModel)
                    {
                        if (serverScriptModelApiModel.Model is WikiModel data)
                        {
                            data.WikiId = wikiModel.WikiId;
                            data.SetByModel(wikiModel: wikiModel);
                        }
                    }
                    return true;
                case Error.Types.Duplicated:
                    return false;
                default:
                    return false;
            }
        }

        public static string Update(Context context, SiteSettings ss, long wikiId, string previousTitle)
        {
            var wikiModel = new WikiModel(
                context: context,
                ss: ss,
                wikiId: wikiId,
                formData: context.Forms);
            var invalid = WikiValidators.OnUpdating(
                context: context,
                ss: ss,
                wikiModel: wikiModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            if (wikiModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts(context: context).ToJson();
            }
            List<Process> processes = null;
            var errorData = wikiModel.Update(
                context: context,
                ss: ss,
                processes: processes,
                notice: true,
                previousTitle: previousTitle);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    var res = new WikisResponseCollection(
                        context: context,
                        wikiModel: wikiModel);
                    res.ReplaceAll("#Breadcrumb", new HtmlBuilder()
                        .Breadcrumb(context: context, ss: ss));
                    return ResponseByUpdate(res, context, ss, wikiModel, processes)
                        .PrependComment(
                            context: context,
                            ss: ss,
                            column: ss.GetColumn(context: context, columnName: "Comments"),
                            comments: wikiModel.Comments,
                            verType: wikiModel.VerType)
                        .ToJson();
                case Error.Types.UpdateConflicts:
                    return Messages.ResponseUpdateConflicts(
                        context: context,
                        data: wikiModel.Updator.Name)
                            .ToJson();
                default:
                    return errorData.MessageJson(context: context);
            }
        }

        private static ResponseCollection ResponseByUpdate(
            WikisResponseCollection res,
            Context context,
            SiteSettings ss,
            WikiModel wikiModel,
            List<Process> processes)
        {
            ss.ClearColumnAccessControlCaches(baseModel: wikiModel);
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
                    where: Rds.WikisWhere().WikiId(wikiModel.WikiId));
                var columns = ss.GetGridColumns(
                    context: context,
                    view: view,
                    checkPermission: true);
                return res
                    .ReplaceAll(
                        $"[data-id=\"{wikiModel.WikiId}\"][data-latest]",
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
                        wikiModel: wikiModel,
                        processes: processes))
                    .Messages(context.Messages);
            }
            else if (wikiModel.Locked)
            {
                ss.SetLockedRecord(
                    context: context,
                    time: wikiModel.UpdatedTime,
                    user: wikiModel.Updator);
                return EditorResponse(
                    context: context,
                    ss: ss,
                    wikiModel: wikiModel)
                        .SetMemory("formChanged", false)
                        .Message(message: UpdatedMessage(
                            context: context,
                            ss: ss,
                            wikiModel: wikiModel,
                            processes: processes))
                        .Messages(context.Messages)
                        .ClearFormData();
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
                    .FieldResponse(context: context, ss: ss, wikiModel: wikiModel)
                    .Val("#VerUp", verUp)
                    .Val("#Ver", wikiModel.Ver)
                    .Disabled("#VerUp", verUp)
                    .Html("#HeaderTitle", HttpUtility.HtmlEncode(wikiModel.Title.DisplayValue))
                    .Html("#RecordInfo", new HtmlBuilder().RecordInfo(
                        context: context,
                        baseModel: wikiModel,
                        tableName: "Wikis"))
                    .Html("#Links", new HtmlBuilder().Links(
                        context: context,
                        ss: ss,
                        id: wikiModel.WikiId))
                    .Links(
                        context: context,
                        ss: ss,
                        id: wikiModel.WikiId,
                        methodType: wikiModel.MethodType)
                    .SetMemory("formChanged", false)
                    .Message(Messages.Updated(
                        context: context,
                        data: wikiModel.Title.DisplayValue))
                    .Messages(context.Messages)
                    .Comment(
                        context: context,
                        ss: ss,
                        column: ss.GetColumn(context: context, columnName: "Comments"),
                        comments: wikiModel.Comments,
                        deleteCommentId: wikiModel.DeleteCommentId)
                    .ClearFormData();
            }
        }

        private static Message UpdatedMessage(
            Context context,
            SiteSettings ss,
            WikiModel wikiModel,
            List<Process> processes)
        {
            var process = processes?.FirstOrDefault(o => !o.SuccessMessage.IsNullOrEmpty()
                && o.MatchConditions);
            if (process == null)
            {
                return Messages.Updated(
                    context: context,
                    data: wikiModel.Title.MessageDisplay(context: context));
            }
            else
            {
                var message = process.GetSuccessMessage(context: context);
                message.Text = wikiModel.ReplacedDisplayValues(
                    context: context,
                    ss: ss,
                    value: message.Text);
                return message;
            }
        }

        public static ContentResultInheritance UpdateByApi(
            Context context,
            SiteSettings ss,
            long wikiId,
            string previousTitle)
        {
            if (!Mime.ValidateOnApi(contentType: context.ContentType))
            {
                return ApiResults.BadRequest(context: context);
            }
            var wikiApiModel = context.RequestDataString.Deserialize<WikiApiModel>();
            if (wikiApiModel == null)
            {
                context.InvalidJsonData = !context.RequestDataString.IsNullOrEmpty();
            }
            var wikiModel = new WikiModel(
                context: context,
                ss: ss,
                wikiId: wikiId,
                wikiApiModel: wikiApiModel);
            if (wikiModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return ApiResults.Get(ApiResponses.NotFound(context: context));
            }
            var invalid = WikiValidators.OnUpdating(
                context: context,
                ss: ss,
                wikiModel: wikiModel,
                api: true);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return ApiResults.Error(
                    context: context,
                    errorData: invalid);
            }
            wikiModel.SiteId = ss.SiteId;
            wikiModel.SetTitle(
                context: context,
                ss: ss);
            wikiModel.VerUp = Versions.MustVerUp(
                context: context,
                ss: ss,
                baseModel: wikiModel);
            var errorData = wikiModel.Update(
                context: context,
                ss: ss,
                notice: true,
                previousTitle: previousTitle);
            BinaryUtilities.UploadImage(
                context: context,
                ss: ss,
                id: wikiModel.WikiId,
                postedFileHash: wikiModel.PostedImageHash);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    return ApiResults.Success(
                        wikiModel.WikiId,
                        limitPerDate: context.ContractSettings.ApiLimit(),
                        limitRemaining: context.ContractSettings.ApiLimit() - ss.ApiCount,
                        message: Displays.Updated(
                            context: context,
                            data: wikiModel.Title.MessageDisplay(context: context)));
                default:
                    return ApiResults.Error(
                        context: context,
                        errorData: errorData);
            }
        }

        public static bool UpdateByServerScript(
            Context context,
            SiteSettings ss,
            long wikiId,
            string previousTitle,
            object model)
        {
            var wikiApiModel = context.RequestDataString.Deserialize<WikiApiModel>();
            if (wikiApiModel == null)
            {
                context.InvalidJsonData = !context.RequestDataString.IsNullOrEmpty();
            }
            var wikiModel = new WikiModel(
                context: context,
                ss: ss,
                wikiId: wikiId,
                wikiApiModel: wikiApiModel);
            if (wikiModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return false;
            }
            var invalid = WikiValidators.OnUpdating(
                context: context,
                ss: ss,
                wikiModel: wikiModel,
                api: true,
                serverScript: true);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    return false;
            }
            wikiModel.SiteId = ss.SiteId;
            wikiModel.SetTitle(
                context: context,
                ss: ss);
            wikiModel.VerUp = Versions.MustVerUp(
                context: context,
                ss: ss,
                baseModel: wikiModel);
            var errorData = wikiModel.Update(
                context: context,
                ss: ss,
                notice: true,
                previousTitle: previousTitle);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    if (model is Libraries.ServerScripts.ServerScriptModelApiModel serverScriptModelApiModel)
                    {
                        if (serverScriptModelApiModel.Model is WikiModel data)
                        {
                            data.SetByModel(wikiModel: wikiModel);
                        }
                    }
                    return true;
                case Error.Types.Duplicated:
                    return false;
                default:
                    return false;
            }
        }

        public static string Delete(Context context, SiteSettings ss, long wikiId)
        {
            var wikiModel = new WikiModel(context, ss, wikiId);
            var invalid = WikiValidators.OnDeleting(
                context: context,
                ss: ss,
                wikiModel: wikiModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var errorData = wikiModel.Delete(context: context, ss: ss, notice: true);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    SessionUtilities.Set(
                        context: context,
                        message: Messages.Deleted(
                            context: context,
                            data: wikiModel.Title.MessageDisplay(context: context)));
                    var res = new WikisResponseCollection(
                        context: context,
                        wikiModel: wikiModel);
                    res
                        .SetMemory("formChanged", false)
                        .Invoke("back");
                    return res.ToJson();
                default:
                    return errorData.MessageJson(context: context);
            }
        }

        public static ContentResultInheritance DeleteByApi(
            Context context, SiteSettings ss, long wikiId)
        {
            if (!Mime.ValidateOnApi(contentType: context.ContentType))
            {
                return ApiResults.BadRequest(context: context);
            }
            var wikiModel = new WikiModel(
                context: context,
                ss: ss,
                wikiId: wikiId,
                methodType: BaseModel.MethodTypes.Edit);
            if (wikiModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return ApiResults.Get(ApiResponses.NotFound(context: context));
            }
            var invalid = WikiValidators.OnDeleting(
                context: context,
                ss: ss,
                wikiModel: wikiModel,
                api: true);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return ApiResults.Error(
                    context: context,
                    errorData: invalid);
            }
            wikiModel.SiteId = ss.SiteId;
            wikiModel.SetTitle(context: context, ss: ss);
            var errorData = wikiModel.Delete(
                context: context,
                ss: ss,
                notice: true);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    return ApiResults.Success(
                        id: wikiModel.WikiId,
                        limitPerDate: context.ContractSettings.ApiLimit(),
                        limitRemaining: context.ContractSettings.ApiLimit() - ss.ApiCount,
                        message: Displays.Deleted(
                            context: context,
                            data: wikiModel.Title.MessageDisplay(context: context)));
                default:
                    return ApiResults.Error(
                        context: context,
                        errorData: errorData);
            }
        }

        public static bool DeleteByServerScript(
            Context context,
            SiteSettings ss,
            long wikiId)
        {
            var wikiModel = new WikiModel(
                context: context,
                ss: ss,
                wikiId: wikiId,
                methodType: BaseModel.MethodTypes.Edit);
            if (wikiModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return false;
            }
            var invalid = WikiValidators.OnDeleting(
                context: context,
                ss: ss,
                wikiModel: wikiModel,
                api: true,
                serverScript: true);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    return false;
            }
            wikiModel.SiteId = ss.SiteId;
            wikiModel.SetTitle(context: context, ss: ss);
            var errorData = wikiModel.Delete(
                context: context,
                ss: ss,
                notice: true);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Restore(Context context, SiteSettings ss)
        {
            if (context.CanManageSite(ss: ss))
            {
                var selector = new RecordSelector(context: context);
                var count = 0;
                if (selector.All)
                {
                    count = Restore(
                        context: context,
                        ss: ss,
                        selected: selector.Selected,
                        negative: true);
                }
                else
                {
                    if (selector.Selected.Any())
                    {
                        count = Restore(
                            context: context,
                            ss: ss,
                            selected: selector.Selected);
                    }
                    else
                    {
                        return Messages.ResponseSelectTargets(context: context).ToJson();
                    }
                }
                Summaries.Synchronize(context: context, ss: ss);
                return "";
            }
            else
            {
                return Messages.ResponseHasNotPermission(context: context).ToJson();
            }
        }

        public static int Restore(
            Context context, SiteSettings ss, List<long> selected, bool negative = false)
        {
            var subWhere = Views.GetBySession(
                context: context,
                ss: ss)
                    .Where(
                        context: context,
                        ss: ss,
                        itemJoin: false);
            var where = Rds.WikisWhere()
                .SiteId(
                    value: ss.SiteId,
                    tableName: "Wikis_Deleted")
                .WikiId_In(
                    value: selected,
                    tableName: "Wikis_Deleted",
                    negative: negative,
                    _using: selected.Any())
                .WikiId_In(
                    tableName: "Wikis_Deleted",
                    sub: Rds.SelectWikis(
                        tableType: Sqls.TableTypes.Deleted,
                        column: Rds.WikisColumn().WikiId(),
                        join: ss.Join(
                            context: context,
                            join: new IJoin[]
                            {
                                subWhere
                            }),
                        where: subWhere));
            var sub = Rds.SelectWikis(
                tableType: Sqls.TableTypes.Deleted,
                _as: "Wikis_Deleted",
                column: Rds.WikisColumn()
                    .WikiId(tableName: "Wikis_Deleted"),
                where: where);
            var column = new Rds.WikisColumnCollection();
                column.WikiId();
                ss.Columns
                    .Where(o => o.TypeCs == "Attachments")
                    .Select(o => o.ColumnName)
                    .ForEach(columnName =>
                        column.Add($"\"{columnName}\""));
            var attachments = Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectWikis(
                    tableType: Sqls.TableTypes.Deleted,
                    column: column,
                    where: Rds.WikisWhere()
                        .SiteId(ss.SiteId)
                        .WikiId_In(sub: sub)))
                .AsEnumerable()
                .Select(dataRow => new
                {
                    wikiId = dataRow.Long("WikiId"),
                    attachments = dataRow
                        .Columns()
                        .Where(columnName => columnName.StartsWith("Attachments"))
                        .SelectMany(columnName => 
                            Jsons.Deserialize<IEnumerable<Attachment>>(dataRow.String(columnName)) 
                                ?? Enumerable.Empty<Attachment>())
                        .Where(o => o != null)
                        .Select(o => o.Guid)
                        .Concat(GetNotDeleteHistoryGuids(
                            context: context,
                            ss: ss,
                            wikiId: dataRow.Long("WikiId")))
                        .Distinct()
                        .ToArray()
                })
                .Where(o => o.attachments.Length > 0);
            var guid = Strings.NewGuid();
            var itemsSub = Rds.SelectItems(
                tableType: Sqls.TableTypes.Deleted,
                column: Rds.ItemsColumn().ReferenceId(),
                where: Rds.ItemsWhere().ReferenceType(guid));
            var count = Repository.ExecuteScalar_response(
                context: context,
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.UpdateItems(
                        tableType: Sqls.TableTypes.Deleted,
                        where: Rds.ItemsWhere()
                            .SiteId(ss.SiteId)
                            .ReferenceId_In(sub: sub),
                        param: Rds.ItemsParam()
                            .ReferenceType(guid)),
                    Rds.RestoreWikis(
                        factory: context,
                        where: Rds.WikisWhere()
                            .WikiId_In(sub: itemsSub)),
                    Rds.RowCount(),
                    Rds.RestoreBinaries(
                        factory: context,
                        where: Rds.BinariesWhere()
                            .ReferenceId_In(sub: itemsSub)
                            .BinaryType("Images")),
                    Rds.RestoreItems(
                        factory: context,
                        where: Rds.ItemsWhere()
                            .SiteId(ss.SiteId)
                            .ReferenceType(guid)),
                    Rds.UpdateItems(
                        where: Rds.ItemsWhere()
                            .SiteId(ss.SiteId)
                            .ReferenceType(guid),
                        param: Rds.ItemsParam()
                            .ReferenceType(ss.ReferenceType))
                }).Count.ToInt();
            attachments.ForEach(o =>
            {
                RestoreAttachments(context, o.wikiId, o.attachments);
            });    
            return count;
        }

        private static void RestoreAttachments(Context context, long wikiId, IList<string> attachments)
        {
            var raw = $" ({string.Join(", ", attachments.Select(o => $"'{o}'"))}) ";
            Repository.ExecuteNonQuery(
                context: context,
                connectionString: Parameters.Rds.OwnerConnectionString,
                statements: new SqlStatement[] {
                    Rds.DeleteBinaries(
                        factory: context,
                        where: Rds.BinariesWhere()
                            .ReferenceId(wikiId)
                            .BinaryType("Attachments")
                            .Binaries_Guid(
                                _operator:" not in ",
                                raw: raw,
                                _using: attachments.Any())),
                    Rds.RestoreBinaries(
                        factory: context,
                        where: Rds.BinariesWhere()
                            .ReferenceId(wikiId)
                            .BinaryType("Attachments")
                            .Binaries_Guid(
                                _operator: $" in ",
                                raw: raw),
                        _using: attachments.Any())
            }, transactional: true);
        }

        public static string RestoreFromHistory(
            Context context, SiteSettings ss, long wikiId)
        {
            if (!Parameters.History.Restore
                || ss.AllowRestoreHistories == false)
            {
                return Error.Types.InvalidRequest.MessageJson(context: context);
            }
            var wikiModel = new WikiModel(context, ss, wikiId);
            var invalid = WikiValidators.OnUpdating(
                context: context,
                ss: ss,
                wikiModel: wikiModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var ver = context.Forms.Data("GridCheckedItems")
                .Split(',')
                .Where(o => !o.IsNullOrEmpty())
                .ToList();
            if (ver.Count() != 1)
            {
                return Error.Types.SelectOne.MessageJson(context: context);
            }
            wikiModel.SetByModel(new WikiModel().Get(
                context: context,
                ss: ss,
                tableType: Sqls.TableTypes.History,
                where: Rds.WikisWhere()
                    .SiteId(ss.SiteId)
                    .WikiId(wikiId)
                    .Ver(ver.First().ToInt())));
            wikiModel.VerUp = true;
            var errorData = wikiModel.Update(
                context: context,
                ss: ss,
                otherInitValue: true);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    RestoreAttachments(
                        context: context,
                        wikiId: wikiModel.WikiId,
                        attachments: wikiModel
                            .AttachmentsHash
                            .Values
                            .SelectMany(o => o.AsEnumerable())
                            .Select(o => o.Guid)
                            .Concat(GetNotDeleteHistoryGuids(
                                context: context,
                                ss: ss,
                                wikiId: wikiModel.WikiId))
                            .Distinct()
                            .ToArray());
                    SessionUtilities.Set(
                        context: context,
                        message: Messages.RestoredFromHistory(
                            context: context,
                            data: ver.First().ToString()));
                    return new ResponseCollection(context: context)
                        .SetMemory("formChanged", false)
                        .Href(Locations.ItemEdit(
                            context: context,
                            id: wikiId))
                        .ToJson();
                default:
                    return errorData.MessageJson(context: context);
            }
        }

        private static IEnumerable<string> GetNotDeleteHistoryGuids(
            Context context,
            SiteSettings ss,
            long wikiId)
        {
            var ret = new List<string>();
            var sqlColumn = new SqlColumnCollection();
            ss.Columns
                ?.Where(column => column.ControlType == "Attachments")
                .Where(column => column?.NotDeleteExistHistory == true)
                .ForEach(column => sqlColumn.Add(column: column));
            if (sqlColumn.Any())
            {
                var dataRows = Rds.ExecuteTable(
                    context: context,
                    statements: Rds.SelectWikis(
                        tableType: Sqls.TableTypes.History,
                        column: sqlColumn,
                        where: Rds.WikisWhere()
                            .SiteId(ss.SiteId)
                            .WikiId(wikiId),
                        distinct: true))
                            .AsEnumerable();
                foreach (var dataRow in dataRows)
                {
                    foreach (DataColumn dataColumn in dataRow.Table.Columns)
                    {
                        var column = new ColumnNameInfo(dataColumn.ColumnName);
                        if (dataRow[column.ColumnName] != DBNull.Value)
                        {
                            dataRow[column.ColumnName].ToString().Deserialize<Attachments>()
                                ?.ForEach(attachment =>
                                    ret.Add(attachment.Guid));
                        }
                    }
                }
            }
            return ret.Distinct();
        }

        public static string Histories(
            Context context, SiteSettings ss, long wikiId, Message message = null)
        {
            var wikiModel = new WikiModel(context: context, ss: ss, wikiId: wikiId);
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
                                wikiModel: wikiModel)));
            return new WikisResponseCollection(
                context: context,
                wikiModel: wikiModel)
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
            WikiModel wikiModel)
        {
            new WikiCollection(
                context: context,
                ss: ss,
                column: HistoryColumn(columns),
                where: Rds.WikisWhere().WikiId(wikiModel.WikiId),
                orderBy: Rds.WikisOrderBy().Ver(SqlOrderBy.Types.desc),
                tableType: Sqls.TableTypes.NormalAndHistory)
                    .ForEach(wikiModelHistory => hb
                        .Tr(
                            attributes: new HtmlAttributes()
                                .Class("grid-row")
                                .DataAction("History")
                                .DataMethod("post")
                                .DataVer(wikiModelHistory.Ver)
                                .DataLatest(
                                    value: 1,
                                    _using: wikiModelHistory.Ver == wikiModel.Ver),
                            action: () =>
                            {
                                hb.Td(
                                    css: "grid-check-td",
                                    action: () => hb
                                        .CheckBox(
                                            controlCss: "grid-check",
                                            _checked: false,
                                            dataId: wikiModelHistory.Ver.ToString(),
                                            _using: wikiModelHistory.Ver < wikiModel.Ver));
                                columns.ForEach(column => hb
                                    .TdValue(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        wikiModel: wikiModelHistory));
                            }));
        }

        private static SqlColumnCollection HistoryColumn(List<Column> columns)
        {
            var sqlColumn = new Rds.WikisColumnCollection()
                .WikiId()
                .Ver();
            columns.ForEach(column =>
                sqlColumn.WikisColumn(columnName: column.ColumnName));
            return sqlColumn;
        }

        public static string History(Context context, SiteSettings ss, long wikiId)
        {
            var wikiModel = new WikiModel(context: context, ss: ss, wikiId: wikiId);
            wikiModel.Get(
                context: context,
                ss: ss,
                where: Rds.WikisWhere()
                    .WikiId(wikiModel.WikiId)
                    .Ver(context.Forms.Int("Ver")),
                tableType: Sqls.TableTypes.NormalAndHistory);
            wikiModel.VerType = context.Forms.Bool("Latest")
                ? Versions.VerTypes.Latest
                : Versions.VerTypes.History;
            return EditorResponse(context, ss, wikiModel)
                .PushState("History", Locations.Get(
                    context: context,
                    parts: new string[]
                    {
                        "Items",
                        wikiId.ToString() 
                            + (wikiModel.VerType == Versions.VerTypes.History
                                ? "?ver=" + context.Forms.Int("Ver") 
                                : string.Empty)
                    }))
                .ToJson();
        }

        public static string DeleteHistory(Context context, SiteSettings ss, long wikiId)
        {
            var wikiModel = new WikiModel(
                context: context,
                ss: ss,
                wikiId: wikiId);
            var invalid = WikiValidators.OnDeleteHistory(
                context: context,
                ss: ss,
                wikiModel: wikiModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var selector = new RecordSelector(context: context);
            var selected = selector
                .Selected
                .Select(o => o.ToInt())
                .ToList();
            var count = 0;
            if (selector.All)
            {
                count = DeleteHistory(
                    context: context,
                    ss: ss,
                    wikiId: wikiId,
                    selected: selected,
                    negative: true);
            }
            else
            {
                if (selector.Selected.Any())
                {
                    count = DeleteHistory(
                        context: context,
                        ss: ss,
                        wikiId: wikiId,
                        selected: selected);
                }
                else
                {
                    return Messages.ResponseSelectTargets(context: context).ToJson();
                }
            }
            return Histories(
                context: context,
                ss: ss,
                wikiId: wikiId,
                message: Messages.HistoryDeleted(
                    context: context,
                    data: count.ToString()));
        }

        private static int DeleteHistory(
            Context context,
            SiteSettings ss,
            long wikiId,
            List<int> selected,
            bool negative = false)
        {
            return Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.PhysicalDeleteWikis(
                        tableType: Sqls.TableTypes.History,
                        where: Rds.WikisWhere()
                            .SiteId(
                                value: ss.SiteId,
                                tableName: "Wikis_History")
                            .WikiId(
                                value: wikiId,
                                tableName: "Wikis_History")
                            .Ver_In(
                                value: selected,
                                tableName: "Wikis_History",
                                negative: negative,
                                _using: selected.Any())
                            .WikiId_In(
                                tableName: "Wikis_History",
                                sub: Rds.SelectWikis(
                                    tableType: Sqls.TableTypes.History,
                                    column: Rds.WikisColumn().WikiId(),
                                    where: new View()
                                        .Where(
                                            context: context,
                                            ss: ss)))),
                    Rds.RowCount()
                }).Count.ToInt();
        }

        public static string UnlockRecord(
            Context context, SiteSettings ss, long wikiId)
        {
            var wikiModel = new WikiModel(
                context: context,
                ss: ss,
                wikiId: wikiId,
                formData: context.Forms);
            var invalid = WikiValidators.OnUnlockRecord(
                context: context,
                ss: ss,
                wikiModel: wikiModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            wikiModel.Timestamp = context.Forms.Get("Timestamp");
            wikiModel.Locked = false;
            var errorData = wikiModel.Update(
                context: context,
                ss: ss,
                notice: true);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    ss.LockedRecordTime = null;
                    ss.LockedRecordUser = null;
                    return EditorResponse(
                        context: context,
                        ss: ss,
                        wikiModel: wikiModel)
                            .Message(Messages.UnlockedRecord(context: context))
                            .Messages(context.Messages)
                            .ToJson();
                case Error.Types.UpdateConflicts:
                    return Messages.ResponseUpdateConflicts(
                        context: context,
                        data: wikiModel.Updator.Name)
                            .ToJson();
                default:
                    return errorData.MessageJson(context: context);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string PreviewTemplate(Context context, SiteSettings ss, string body)
        {
            var hb = new HtmlBuilder();
            var name = Strings.NewGuid();
            return hb
                .Div(css: "samples-displayed", action: () => hb
                    .Text(text: Displays.SamplesDisplayed(context: context)))
                .Div(css: "template-tab-container", action: () => hb
                    .Ul(action: () => hb
                        .Li(action: () => hb
                            .A(
                                href: "#" + name + "Editor",
                                text: Displays.Editor(context: context))))
                    .FieldSet(
                        id: name + "Editor",
                        action: () => hb
                            .FieldSetGeneralColumns(
                                context: context,
                                ss: ss,
                                wikiModel: new WikiModel() { Body = body },
                                preview: true)))
                                    .ToString();
        }
    }
}
