using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
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
    public static class WikiUtilities
    {
        public static HtmlBuilder TdValue(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Column column,
            WikiModel wikiModel)
        {
            if (!column.GridDesign.IsNullOrEmpty())
            {
                return hb.TdCustomValue(
                    context: context,
                    ss: ss,
                    gridDesign: column.GridDesign,
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
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: wikiModel.SiteId)
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
                                    value: wikiModel.UpdatedTime)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "WikiId":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: wikiModel.WikiId)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
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
                                    value: wikiModel.Ver)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "Title":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: wikiModel.Title)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "Body":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: wikiModel.Body)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "TitleBody":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: wikiModel.TitleBody)
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
                                    value: wikiModel.Comments)
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
                                    value: wikiModel.Creator)
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
                                    value: wikiModel.Updator)
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
                                    value: wikiModel.CreatedTime)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    default: return hb;
                }
            }
        }

        private static HtmlBuilder TdCustomValue(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            string gridDesign,
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
                }
                gridDesign = gridDesign.Replace("[" + column.ColumnName + "]", value);
            });
            return hb.Td(action: () => hb
                .Div(css: "markup", action: () => hb
                    .Text(text: gridDesign)));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string EditorNew(Context context, SiteSettings ss)
        {
            var wikiId = Rds.ExecuteScalar_long(
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
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return HtmlTemplates.Error(context, invalid);
            }
            var hb = new HtmlBuilder();
            ss.SetColumnAccessControls(
                context: context,
                mine: wikiModel.Mine(context: context));
            return editInDialog
                ? hb.DialogEditorForm(
                    siteId: wikiModel.SiteId,
                    referenceId: wikiModel.WikiId,
                    action: () => hb
                        .FieldSetGeneral(
                            context: context,
                            ss: ss,
                            wikiModel: wikiModel,
                            editInDialog: editInDialog))
                                .ToString()
                : hb.Template(
                    context: context,
                    ss: ss,
                    verType: wikiModel.VerType,
                    methodType: wikiModel.MethodType,
                    siteId: wikiModel.SiteId,
                    parentId: ss.ParentId,
                    referenceType: "Wikis",
                    title: wikiModel.MethodType == BaseModel.MethodTypes.New
                        ? Displays.New()
                        : wikiModel.Title.DisplayValue,
                    useTitle: ss.TitleColumns?.Any(o => ss.EditorColumns.Contains(o)) == true,
                    userScript: ss.EditorScripts(
                        context: context, methodType: wikiModel.MethodType),
                    userStyle: ss.EditorStyles(
                        context: context, methodType: wikiModel.MethodType),
                    action: () => hb
                        .Editor(
                            context: context,
                            ss: ss,
                            wikiModel: wikiModel)
                        .Hidden(controlId: "TableName", value: "Wikis")
                        .Hidden(controlId: "Id", value: wikiModel.WikiId.ToString())
                        .Hidden(controlId: "TriggerRelatingColumns", value: Jsons.ToJson(ss.RelatingColumns))
                        .Hidden(controlId: "DropDownSearchPageSize", value: Parameters.General.DropDownSearchPageSize.ToString()))
                            .ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Editor(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            WikiModel wikiModel)
        {
            var commentsColumn = ss.GetColumn(
                context: context,
                columnName: "Comments");
            var commentsColumnPermissionType = commentsColumn
                .ColumnPermissionType(context: context);
            var showComments = ss.ShowComments(commentsColumnPermissionType);
            var tabsCss = showComments ? null : "max";
            return hb.Div(id: "Editor", action: () => hb
                .Form(
                    attributes: new HtmlAttributes()
                        .Id("WikiForm")
                        .Class("main-form")
                        .Action(Locations.ItemAction(wikiModel.WikiId != 0
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
                        .Div(id: "EditorTabsContainer", css: tabsCss, action: () => hb
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
                                siteId: wikiModel.SiteId,
                                verType: wikiModel.VerType,
                                referenceId: wikiModel.WikiId,
                                updateButton: true,
                                copyButton: false,
                                moveButton: false,
                                mailButton: true,
                                deleteButton: true))
                        .Hidden(controlId: "BaseUrl", value: Locations.BaseUrl())
                        .Hidden(controlId: "MethodType", value: "edit")
                        .Hidden(
                            controlId: "Wikis_Timestamp",
                            css: "always-send",
                            value: wikiModel.Timestamp)
                        .Hidden(
                            controlId: "SwitchTargets",
                            css: "always-send",
                            value: wikiModel.WikiId.ToString(),
                            _using: !Request.IsAjax() || context.Action == "create"))
                .OutgoingMailsForm(
                    context: context,
                    referenceType: "Wikis",
                    referenceId: wikiModel.WikiId,
                    referenceVer: wikiModel.Ver)
                .CopyDialog("items", wikiModel.WikiId)
                .MoveDialog(context: context)
                .OutgoingMailDialog());
        }

        private static HtmlBuilder EditorTabs(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            WikiModel wikiModel)
        {
            return hb.Ul(id: "EditorTabs", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetGeneral",
                        text: Displays.General()))
                .Li(_using: wikiModel.MethodType != BaseModel.MethodTypes.New,
                    action: () => hb
                        .A(
                            href: "#FieldSetHistories",
                            text: Displays.ChangeHistoryList()))
                .Li(_using: context.CanManagePermission(ss: ss) &&
                        wikiModel.MethodType != BaseModel.MethodTypes.New,
                    action: () => hb
                        .A(
                            href: "#FieldSetRecordAccessControl",
                            text: Displays.RecordAccessControl())));
        }

        private static HtmlBuilder FieldSetGeneral(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            WikiModel wikiModel,
            bool editInDialog = false)
        {
            var mine = wikiModel.Mine(context: context);
            return hb.FieldSet(id: "FieldSetGeneral", action: () => hb
                .FieldSetGeneralColumns(
                    context: context,
                    ss: ss,
                    wikiModel: wikiModel,
                    editInDialog: editInDialog));
        }

        public static HtmlBuilder FieldSetGeneralColumns(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            WikiModel wikiModel,
            bool preview = false,
            bool editInDialog = false)
        {
            ss.GetEditorColumns(context: context).ForEach(column =>
            {
                switch (column.Name)
                {
                    case "WikiId":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: wikiModel.MethodType,
                            value: wikiModel.WikiId
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "Ver":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: wikiModel.MethodType,
                            value: wikiModel.Ver
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "Title":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: wikiModel.MethodType,
                            value: wikiModel.Title
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "Body":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: wikiModel.MethodType,
                            value: wikiModel.Body
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                }
            });
            if (!preview)
            {
                hb.VerUpCheckBox(
                    context: context,
                    ss: ss,
                    baseModel: wikiModel);
            }
            return hb;
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
                context, ss, wikiId)).ToJson();
        }

        private static ResponseCollection EditorResponse(
            Context context,
            SiteSettings ss,
            WikiModel wikiModel,
            Message message = null,
            string switchTargets = null)
        {
            wikiModel.MethodType = BaseModel.MethodTypes.Edit;
            return new WikisResponseCollection(wikiModel)
                .Invoke("clearDialogs")
                .ReplaceAll("#MainContainer", Editor(context, ss, wikiModel))
                .Val("#SwitchTargets", switchTargets, _using: switchTargets != null)
                .SetMemory("formChanged", false)
                .Invoke("setCurrentIndex")
                .Message(message)
                .ClearFormData();
        }

        public static ResponseCollection FieldResponse(
            this ResponseCollection res,
            Context context,
            SiteSettings ss,
            WikiModel wikiModel)
        {
            var mine = wikiModel.Mine(context: context);
            ss.EditorColumns
                .Select(columnName => ss.GetColumn(context: context, columnName: columnName))
                .Where(column => column != null)
                .ForEach(column =>
                {
                    switch (column.Name)
                    {
                        case "SiteId":
                            res.Val(
                                "#Wikis_SiteId",
                                wikiModel.SiteId.ToControl(context: context, ss: ss, column: column));
                            break;
                        case "UpdatedTime":
                            res.Val(
                                "#Wikis_UpdatedTime",
                                wikiModel.UpdatedTime.ToControl(context: context, ss: ss, column: column));
                            break;
                        case "WikiId":
                            res.Val(
                                "#Wikis_WikiId",
                                wikiModel.WikiId.ToControl(context: context, ss: ss, column: column));
                            break;
                        case "Ver":
                            res.Val(
                                "#Wikis_Ver",
                                wikiModel.Ver.ToControl(context: context, ss: ss, column: column));
                            break;
                        case "Title":
                            res.Val(
                                "#Wikis_Title",
                                wikiModel.Title.ToControl(context: context, ss: ss, column: column));
                            break;
                        case "Body":
                            res.Val(
                                "#Wikis_Body",
                                wikiModel.Body.ToControl(context: context, ss: ss, column: column));
                            break;
                        case "Comments":
                            res.Val(
                                "#Wikis_Comments",
                                wikiModel.Comments.ToControl(context: context, ss: ss, column: column));
                            break;
                        case "Creator":
                            res.Val(
                                "#Wikis_Creator",
                                wikiModel.Creator.ToControl(context: context, ss: ss, column: column));
                            break;
                        case "Updator":
                            res.Val(
                                "#Wikis_Updator",
                                wikiModel.Updator.ToControl(context: context, ss: ss, column: column));
                            break;
                        case "CreatedTime":
                            res.Val(
                                "#Wikis_CreatedTime",
                                wikiModel.CreatedTime.ToControl(context: context, ss: ss, column: column));
                            break;
                        default: break;
                    }
                });
            return res;
        }

        public static string Update(Context context, SiteSettings ss, long wikiId)
        {
            var wikiModel = new WikiModel(
                context: context, ss: ss, wikiId: wikiId, setByForm: true);
            var invalid = WikiValidators.OnUpdating(
                context: context, ss: ss, wikiModel: wikiModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            if (wikiModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts().ToJson();
            }
            var error = wikiModel.Update(
                context: context,
                ss: ss,
                notice: true,
                permissions: Forms.List("CurrentPermissionsAll"),
                permissionChanged: Forms.Exists("CurrentPermissionsAll"));
            switch (error)
            {
                case Error.Types.None:
                    var res = new WikisResponseCollection(wikiModel);
                    res.ReplaceAll("#Breadcrumb", new HtmlBuilder()
                        .Breadcrumb(context: context, ss: ss));
                    return ResponseByUpdate(res, context, ss, wikiModel)
                        .PrependComment(
                            context: context,
                            ss: ss,
                            column: ss.GetColumn(context: context, columnName: "Comments"),
                            comments: wikiModel.Comments,
                            verType: wikiModel.VerType)
                        .ToJson();
                case Error.Types.UpdateConflicts:
                    return Messages.ResponseUpdateConflicts(
                        wikiModel.Updator.Name)
                            .ToJson();
                default:
                    return error.MessageJson();
            }
        }

        private static ResponseCollection ResponseByUpdate(
            WikisResponseCollection res,
            Context context,
            SiteSettings ss,
            WikiModel wikiModel)
        {
            if (Forms.Bool("IsDialogEditorForm"))
            {
                var view = Views.GetBySession(
                    context: context,
                    ss: ss);
                var gridData = new GridData(
                    context: context,
                    ss: ss,
                    view: view,
                    where: Rds.WikisWhere().WikiId(wikiModel.WikiId));
                var columns = ss.GetGridColumns(
                    context: context,
                    checkPermission: true);
                return res
                    .ReplaceAll(
                        $"[data-id=\"{wikiModel.WikiId}\"]",
                        gridData.TBody(
                            hb: new HtmlBuilder(),
                            context: context,
                            ss: ss,
                            columns: columns,
                            checkAll: false))
                    .CloseDialog()
                    .Message(Messages.Updated(wikiModel.Title.DisplayValue));
            }
            else
            {
                return res
                    .Ver(context: context)
                    .Timestamp(context: context)
                    .Val("#VerUp", false)
                    .FieldResponse(context: context, ss: ss, wikiModel: wikiModel)
                    .Disabled("#VerUp", false)
                    .Html("#HeaderTitle", wikiModel.Title.DisplayValue)
                    .Html("#RecordInfo", new HtmlBuilder().RecordInfo(
                        context: context,
                        baseModel: wikiModel,
                        tableName: "Wikis"))
                    .Html("#Links", new HtmlBuilder().Links(
                        context: context,
                        ss: ss,
                        id: wikiModel.WikiId))
                    .SetMemory("formChanged", false)
                    .Message(Messages.Updated(wikiModel.Title.DisplayValue))
                    .Comment(
                        context: context,
                        ss: ss,
                        column: ss.GetColumn(context: context, columnName: "Comments"),
                        comments: wikiModel.Comments,
                        deleteCommentId: wikiModel.DeleteCommentId)
                    .ClearFormData();
            }
        }

        public static string Delete(Context context, SiteSettings ss, long wikiId)
        {
            var wikiModel = new WikiModel(context, ss, wikiId);
            var invalid = WikiValidators.OnDeleting(
                context: context, ss: ss, wikiModel: wikiModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var error = wikiModel.Delete(context: context, ss: ss, notice: true);
            switch (error)
            {
                case Error.Types.None:
                    Sessions.Set("Message", Messages.Deleted(wikiModel.Title.Value));
                    var res = new WikisResponseCollection(wikiModel);
                res
                    .SetMemory("formChanged", false)
                    .Href(Locations.ItemIndex(Rds.ExecuteScalar_long(
                        context: context,
                        statements: Rds.SelectSites(
                            tableType: Sqls.TableTypes.Deleted,
                            column: Rds.SitesColumn().ParentId(),
                            where: Rds.SitesWhere()
                                .TenantId(context.TenantId)
                                .SiteId(wikiModel.SiteId)))));
                    return res.ToJson();
                default:
                    return error.MessageJson();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Restore(Context context, SiteSettings ss)
        {
            if (context.CanManageSite(ss: ss))
            {
                var selector = new GridSelector();
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
                        return Messages.ResponseSelectTargets().ToJson();
                    }
                }
                Summaries.Synchronize(context: context, ss: ss);
                return "";
            }
            else
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
        }

        public static int Restore(
            Context context, SiteSettings ss, List<long> selected, bool negative = false)
        {
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
                        where: Views.GetBySession(context: context, ss: ss).Where(context: context, ss: ss)));
            return Rds.ExecuteScalar_response(
                context: context,
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreItems(where: Rds.ItemsWhere().ReferenceId_In(sub:
                        Rds.SelectWikis(
                            tableType: Sqls.TableTypes.Deleted,
                            _as: "Wikis_Deleted",
                            column: Rds.WikisColumn()
                                .WikiId(tableName: "Wikis_Deleted"),
                            where: where))),
                    Rds.RestoreWikis(where: where, countRecord: true)
                }).Count.ToInt();
        }

        public static string RestoreFromHistory(
            Context context, SiteSettings ss, long wikiId)
        {
            if (!Parameters.History.Restore)
            {
                return Error.Types.InvalidRequest.MessageJson();
            }
            var wikiModel = new WikiModel(context, ss, wikiId);
            var invalid = WikiValidators.OnUpdating(
                context: context, ss: ss, wikiModel: wikiModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var ver = Forms.Data("GridCheckedItems")
                .Split(',')
                .Where(o => !o.IsNullOrEmpty())
                .ToList();
            if (ver.Count() != 1)
            {
                return Error.Types.SelectOne.MessageJson();
            }
            wikiModel.SetByModel(new WikiModel().Get(
                context: context,
                ss: ss,
                tableType: Sqls.TableTypes.History,
                where: Rds.WikisWhere()
                    .SiteId(ss.SiteId)
                    .WikiId(wikiId)
                    .Ver(ver.First())));
            wikiModel.VerUp = true;
            var error = wikiModel.Update(
                context: context,
                ss: ss,
                otherInitValue: true);
            switch (error)
            {
                case Error.Types.None:
                    Sessions.Set("Message", Messages.RestoredFromHistory(ver.First().ToString()));
                    return  new ResponseCollection()
                        .SetMemory("formChanged", false)
                        .Href(Locations.ItemEdit(wikiId))
                        .ToJson();
                default:
                    return error.MessageJson();
            }
        }

        public static string Histories(
            Context context, SiteSettings ss, long wikiId, Message message = null)
        {
            var wikiModel = new WikiModel(context: context, ss: ss, wikiId: wikiId);
            ss.SetColumnAccessControls(
                context: context,
                mine: wikiModel.Mine(context: context));
            var columns = ss.GetHistoryColumns(context: context, checkPermission: true);
            if (!context.CanRead(ss: ss))
            {
                return Error.Types.HasNotPermission.MessageJson();
            }
            var hb = new HtmlBuilder();
            hb
                .HistoryCommands(context: context, ss: ss)
                .Table(
                    attributes: new HtmlAttributes().Class("grid history"),
                    action: () => hb
                        .THead(action: () => hb
                            .GridHeader(
                                columns: columns,
                                sort: false,
                                checkRow: true))
                        .TBody(action: () => hb
                            .HistoriesTableBody(
                                context: context,
                                ss: ss,
                                columns: columns,
                                wikiModel: wikiModel)));
            return new WikisResponseCollection(wikiModel)
                .Html("#FieldSetHistories", hb)
                .Message(message)
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
                                .DataLatest(1, _using:
                                    wikiModelHistory.Ver == wikiModel.Ver),
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
                                columns
                                    .ForEach(column => hb
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
            columns.ForEach(column => sqlColumn.WikisColumn(column.ColumnName));
            return sqlColumn;
        }

        public static string History(Context context, SiteSettings ss, long wikiId)
        {
            var wikiModel = new WikiModel(context: context, ss: ss, wikiId: wikiId);
            ss.SetColumnAccessControls(
                context: context,
                mine: wikiModel.Mine(context: context));
            wikiModel.Get(
                context: context,
                ss: ss,
                where: Rds.WikisWhere()
                    .WikiId(wikiModel.WikiId)
                    .Ver(Forms.Int("Ver")),
                tableType: Sqls.TableTypes.NormalAndHistory);
            wikiModel.VerType = Forms.Bool("Latest")
                ? Versions.VerTypes.Latest
                : Versions.VerTypes.History;
            return EditorResponse(context, ss, wikiModel).ToJson();
        }

        public static string DeleteHistory(Context context, SiteSettings ss, long wikiId)
        {
            if (!Parameters.History.PhysicalDelete)
            {
                return Error.Types.InvalidRequest.MessageJson();
            }
            if (context.CanManageSite(ss: ss))
            {
                var selector = new GridSelector();
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
                        return Messages.ResponseSelectTargets().ToJson();
                    }
                }
                return Histories(
                    context: context,
                    ss: ss,
                    wikiId: wikiId,
                    message: Messages.HistoryDeleted(count.ToString()));
            }
            else
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
        }

        private static int DeleteHistory(
            Context context,
            SiteSettings ss,
            long wikiId,
            List<int> selected,
            bool negative = false)
        {
            return Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: Rds.PhysicalDeleteWikis(
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
                                where: Views.GetBySession(context: context, ss: ss).Where(
                                    context: context, ss: ss))),
                    countRecord: true)).Count.ToInt();
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
                    .Text(text: Displays.SamplesDisplayed()))
                .Div(css: "template-tab-container", action: () => hb
                    .Ul(action: () => hb
                        .Li(action: () => hb
                            .A(
                                href: "#" + name + "Editor",
                                text: Displays.Editor())))
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
