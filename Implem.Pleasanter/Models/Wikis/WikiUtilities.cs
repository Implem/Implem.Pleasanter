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
            this HtmlBuilder hb, SiteSettings ss, Column column, WikiModel wikiModel)
        {
            if (!column.GridDesign.IsNullOrEmpty())
            {
                return hb.TdCustomValue(
                    ss: ss,
                    gridDesign: column.GridDesign,
                    wikiModel: wikiModel);
            }
            else
            {
                var mine = wikiModel.Mine();
                switch (column.Name)
                {
                    case "SiteId":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: wikiModel.SiteId)
                            : hb.Td(column: column, value: string.Empty);
                    case "UpdatedTime":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: wikiModel.UpdatedTime)
                            : hb.Td(column: column, value: string.Empty);
                    case "WikiId":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: wikiModel.WikiId)
                            : hb.Td(column: column, value: string.Empty);
                    case "Ver":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: wikiModel.Ver)
                            : hb.Td(column: column, value: string.Empty);
                    case "Title":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: wikiModel.Title)
                            : hb.Td(column: column, value: string.Empty);
                    case "Body":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: wikiModel.Body)
                            : hb.Td(column: column, value: string.Empty);
                    case "TitleBody":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: wikiModel.TitleBody)
                            : hb.Td(column: column, value: string.Empty);
                    case "Comments":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: wikiModel.Comments)
                            : hb.Td(column: column, value: string.Empty);
                    case "Creator":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: wikiModel.Creator)
                            : hb.Td(column: column, value: string.Empty);
                    case "Updator":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: wikiModel.Updator)
                            : hb.Td(column: column, value: string.Empty);
                    case "CreatedTime":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: wikiModel.CreatedTime)
                            : hb.Td(column: column, value: string.Empty);
                    default: return hb;
                }
            }
        }

        private static HtmlBuilder TdCustomValue(
            this HtmlBuilder hb, SiteSettings ss, string gridDesign, WikiModel wikiModel)
        {
            ss.IncludedColumns(gridDesign).ForEach(column =>
            {
                var value = string.Empty;
                switch (column.Name)
                {
                    case "SiteId": value = wikiModel.SiteId.GridText(column: column); break;
                    case "UpdatedTime": value = wikiModel.UpdatedTime.GridText(column: column); break;
                    case "WikiId": value = wikiModel.WikiId.GridText(column: column); break;
                    case "Ver": value = wikiModel.Ver.GridText(column: column); break;
                    case "Title": value = wikiModel.Title.GridText(column: column); break;
                    case "Body": value = wikiModel.Body.GridText(column: column); break;
                    case "TitleBody": value = wikiModel.TitleBody.GridText(column: column); break;
                    case "Comments": value = wikiModel.Comments.GridText(column: column); break;
                    case "Creator": value = wikiModel.Creator.GridText(column: column); break;
                    case "Updator": value = wikiModel.Updator.GridText(column: column); break;
                    case "CreatedTime": value = wikiModel.CreatedTime.GridText(column: column); break;
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
        public static string EditorNew(SiteSettings ss)
        {
            var wikiId = Rds.ExecuteScalar_long(statements: Rds.SelectWikis(
                column: Rds.WikisColumn().WikiId(),
                where: Rds.WikisWhere().SiteId(ss.SiteId)));
            return Editor(ss, new WikiModel(ss, methodType: BaseModel.MethodTypes.New));
        }

        public static string Editor(SiteSettings ss, long wikiId, bool clearSessions)
        {
            var wikiModel = new WikiModel(
                ss: ss,
                wikiId: wikiId,
                clearSessions: clearSessions,
                methodType: BaseModel.MethodTypes.Edit);
            return Editor(ss, wikiModel);
        }

        public static string Editor(SiteSettings ss, WikiModel wikiModel)
        {
            var invalid = WikiValidators.OnEditing(ss, wikiModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return HtmlTemplates.Error(invalid);
            }
            var hb = new HtmlBuilder();
            ss.SetColumnAccessControls(wikiModel.Mine());
            return hb.Template(
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
                userScript: ss.EditorScripts(wikiModel.MethodType),
                userStyle: ss.EditorStyles(wikiModel.MethodType),
                action: () => hb
                    .Editor(
                        ss: ss,
                        wikiModel: wikiModel)
                    .Hidden(controlId: "TableName", value: "Wikis")
                    .Hidden(controlId: "Id", value: wikiModel.WikiId.ToString()))
                        .ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Editor(
            this HtmlBuilder hb,
            SiteSettings ss,
            WikiModel wikiModel)
        {
            var commentsColumn = ss.GetColumn("Comments");
            var commentsColumnPermissionType = commentsColumn.ColumnPermissionType();
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
                            ss: ss,
                            baseModel: wikiModel,
                            tableName: "Wikis")
                        .Div(
                            id: "EditorComments", action: () => hb
                                .Comments(
                                    comments: wikiModel.Comments,
                                    column: commentsColumn,
                                    verType: wikiModel.VerType,
                                    columnPermissionType: commentsColumnPermissionType),
                            _using: showComments)
                        .Div(id: "EditorTabsContainer", css: tabsCss, action: () => hb
                            .EditorTabs(wikiModel: wikiModel, ss: ss)
                            .FieldSetGeneral(
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
                                _using: ss.CanManagePermission())
                            .MainCommands(
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
                            _using: !Request.IsAjax() || Routes.Action() == "create"))
                .OutgoingMailsForm("Wikis", wikiModel.WikiId, wikiModel.Ver)
                .CopyDialog("items", wikiModel.WikiId)
                .MoveDialog()
                .OutgoingMailDialog());
        }

        private static HtmlBuilder EditorTabs(
            this HtmlBuilder hb, SiteSettings ss, WikiModel wikiModel)
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
                .Li(_using: ss.CanManagePermission() &&
                        wikiModel.MethodType != BaseModel.MethodTypes.New,
                    action: () => hb
                        .A(
                            href: "#FieldSetRecordAccessControl",
                            text: Displays.RecordAccessControl())));
        }

        private static HtmlBuilder FieldSetGeneral(
            this HtmlBuilder hb,
            SiteSettings ss,
            WikiModel wikiModel)
        {
            var mine = wikiModel.Mine();
            return hb.FieldSet(id: "FieldSetGeneral", action: () => hb
                .FieldSetGeneralColumns(
                    ss: ss, wikiModel: wikiModel));
        }

        public static HtmlBuilder FieldSetGeneralColumns(
            this HtmlBuilder hb,
            SiteSettings ss,
            WikiModel wikiModel,
            bool preview = false)
        {
            ss.GetEditorColumns().ForEach(column =>
            {
                switch (column.Name)
                {
                    case "WikiId":
                        hb.Field(
                            ss,
                            column,
                            wikiModel.MethodType,
                            wikiModel.WikiId.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "Ver":
                        hb.Field(
                            ss,
                            column,
                            wikiModel.MethodType,
                            wikiModel.Ver.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "Title":
                        hb.Field(
                            ss,
                            column,
                            wikiModel.MethodType,
                            wikiModel.Title.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "Body":
                        hb.Field(
                            ss,
                            column,
                            wikiModel.MethodType,
                            wikiModel.Body.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                }
            });
            if (!preview)
            {
                hb.VerUpCheckBox(wikiModel);
            }
            return hb;
        }

        private static HtmlBuilder MainCommandExtensions(
            this HtmlBuilder hb, SiteSettings ss, WikiModel wikiModel)
        {
            return hb;
        }

        private static HtmlBuilder EditorExtensions(
            this HtmlBuilder hb, SiteSettings ss, WikiModel wikiModel)
        {
            return hb;
        }

        public static string EditorJson(SiteSettings ss, long wikiId)
        {
            return EditorResponse(ss, new WikiModel(ss, wikiId)).ToJson();
        }

        private static ResponseCollection EditorResponse(
            SiteSettings ss,
            WikiModel wikiModel,
            Message message = null,
            string switchTargets = null)
        {
            wikiModel.MethodType = BaseModel.MethodTypes.Edit;
            return new WikisResponseCollection(wikiModel)
                .Invoke("clearDialogs")
                .ReplaceAll("#MainContainer", Editor(ss, wikiModel))
                .Val("#SwitchTargets", switchTargets, _using: switchTargets != null)
                .SetMemory("formChanged", false)
                .Invoke("setCurrentIndex")
                .Message(message)
                .ClearFormData();
        }

        public static ResponseCollection FieldResponse(
            this ResponseCollection res,
            SiteSettings ss,
            WikiModel wikiModel)
        {
            var mine = wikiModel.Mine();
            ss.EditorColumns
                .Select(o => ss.GetColumn(o))
                .Where(o => o != null)
                .ForEach(column =>
                {
                    switch (column.Name)
                    {
                        case "SiteId":
                            res.Val(
                                "#Wikis_SiteId",
                                wikiModel.SiteId.ToControl(ss, column));
                            break;
                        case "UpdatedTime":
                            res.Val(
                                "#Wikis_UpdatedTime",
                                wikiModel.UpdatedTime.ToControl(ss, column));
                            break;
                        case "WikiId":
                            res.Val(
                                "#Wikis_WikiId",
                                wikiModel.WikiId.ToControl(ss, column));
                            break;
                        case "Ver":
                            res.Val(
                                "#Wikis_Ver",
                                wikiModel.Ver.ToControl(ss, column));
                            break;
                        case "Title":
                            res.Val(
                                "#Wikis_Title",
                                wikiModel.Title.ToControl(ss, column));
                            break;
                        case "Body":
                            res.Val(
                                "#Wikis_Body",
                                wikiModel.Body.ToControl(ss, column));
                            break;
                        case "Comments":
                            res.Val(
                                "#Wikis_Comments",
                                wikiModel.Comments.ToControl(ss, column));
                            break;
                        case "Creator":
                            res.Val(
                                "#Wikis_Creator",
                                wikiModel.Creator.ToControl(ss, column));
                            break;
                        case "Updator":
                            res.Val(
                                "#Wikis_Updator",
                                wikiModel.Updator.ToControl(ss, column));
                            break;
                        case "CreatedTime":
                            res.Val(
                                "#Wikis_CreatedTime",
                                wikiModel.CreatedTime.ToControl(ss, column));
                            break;
                        default: break;
                    }
                });
            return res;
        }

        public static string Update(SiteSettings ss, long wikiId)
        {
            var wikiModel = new WikiModel(ss, wikiId, setByForm: true);
            var invalid = WikiValidators.OnUpdating(ss, wikiModel);
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
                ss: ss,
                notice: true,
                permissions: Forms.List("CurrentPermissionsAll"),
                permissionChanged: Forms.Exists("CurrentPermissionsAll"));
            if (error.Has())
            {
                return error == Error.Types.UpdateConflicts
                    ? Messages.ResponseUpdateConflicts(wikiModel.Updator.Name).ToJson()
                    : new ResponseCollection().Message(error.Message()).ToJson();
            }
            else
            {
                var res = new WikisResponseCollection(wikiModel);
                res.ReplaceAll("#Breadcrumb", new HtmlBuilder().Breadcrumb(ss.SiteId));
                return ResponseByUpdate(res, ss, wikiModel)
                    .PrependComment(
                        ss,
                        ss.GetColumn("Comments"),
                        wikiModel.Comments,
                        wikiModel.VerType)
                    .ToJson();
            }
        }

        private static ResponseCollection ResponseByUpdate(
            WikisResponseCollection res,
            SiteSettings ss,
            WikiModel wikiModel)
        {
            return res
                .Ver()
                .Timestamp()
                .Val("#VerUp", false)
                .FieldResponse(ss, wikiModel)
                .Disabled("#VerUp", false)
                .Html("#HeaderTitle", wikiModel.Title.DisplayValue)
                .Html("#RecordInfo", new HtmlBuilder().RecordInfo(
                    baseModel: wikiModel, tableName: "Wikis"))
                .Html("#Links", new HtmlBuilder().Links(
                    ss: ss, id: wikiModel.WikiId))
                .SetMemory("formChanged", false)
                .Message(Messages.Updated(wikiModel.Title.DisplayValue))
                .Comment(
                    ss,
                    ss.GetColumn("Comments"),
                    wikiModel.Comments,
                    wikiModel.DeleteCommentId)
                .ClearFormData();
        }

        public static string Delete(SiteSettings ss, long wikiId)
        {
            var wikiModel = new WikiModel(ss, wikiId);
            var invalid = WikiValidators.OnDeleting(ss, wikiModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var error = wikiModel.Delete(ss, notice: true);
            if (error.Has())
            {
                return error.MessageJson();
            }
            else
            {
                Sessions.Set("Message", Messages.Deleted(wikiModel.Title.Value));
                var res = new WikisResponseCollection(wikiModel);
                res
                    .SetMemory("formChanged", false)
                    .Href(Locations.ItemIndex(Rds.ExecuteScalar_long(statements:
                        Rds.SelectSites(
                            tableType: Sqls.TableTypes.Deleted,
                            column: Rds.SitesColumn().ParentId(),
                            where: Rds.SitesWhere()
                                .TenantId(Sessions.TenantId())
                                .SiteId(wikiModel.SiteId)))));
                return res.ToJson();
            }
        }

        public static string Restore(SiteSettings ss, long wikiId)
        {
            var wikiModel = new WikiModel();
            var invalid = WikiValidators.OnRestoring();
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var error = wikiModel.Restore(ss, wikiId);
            if (error.Has())
            {
                return error.MessageJson();
            }
            else
            {
                var res = new WikisResponseCollection(wikiModel);
                return res.ToJson();
            }
        }

        public static string Histories(SiteSettings ss, long wikiId)
        {
            var wikiModel = new WikiModel(ss, wikiId);
            ss.SetColumnAccessControls(wikiModel.Mine());
            var columns = ss.GetHistoryColumns(checkPermission: true);
            if (!ss.CanRead())
            {
                return Error.Types.HasNotPermission.MessageJson();
            }
            var hb = new HtmlBuilder();
            hb.Table(
                attributes: new HtmlAttributes().Class("grid"),
                action: () => hb
                    .THead(action: () => hb
                        .GridHeader(
                            columns: columns,
                            sort: false,
                            checkRow: false))
                    .TBody(action: () =>
                        new WikiCollection(
                            ss: ss,
                            column: HistoryColumn(columns),
                            where: Rds.WikisWhere().WikiId(wikiModel.WikiId),
                            orderBy: Rds.WikisOrderBy().Ver(SqlOrderBy.Types.desc),
                            tableType: Sqls.TableTypes.NormalAndHistory)
                                .ForEach(wikiModelHistory => hb
                                    .Tr(
                                        attributes: new HtmlAttributes()
                                            .Class("grid-row history not-link")
                                            .DataAction("History")
                                            .DataMethod("post")
                                            .DataVer(wikiModelHistory.Ver)
                                            .DataLatest(1, _using:
                                                wikiModelHistory.Ver == wikiModel.Ver),
                                        action: () => columns
                                            .ForEach(column => hb
                                                .TdValue(
                                                    ss: ss,
                                                    column: column,
                                                    wikiModel: wikiModelHistory))))));
            return new WikisResponseCollection(wikiModel)
                .Html("#FieldSetHistories", hb).ToJson();
        }

        private static SqlColumnCollection HistoryColumn(List<Column> columns)
        {
            var sqlColumn = new Rds.WikisColumnCollection()
                .WikiId()
                .Ver();
            columns.ForEach(column => sqlColumn.WikisColumn(column.ColumnName));
            return sqlColumn;
        }

        public static string History(SiteSettings ss, long wikiId)
        {
            var wikiModel = new WikiModel(ss, wikiId);
            ss.SetColumnAccessControls(wikiModel.Mine());
            wikiModel.Get(
                ss, 
                where: Rds.WikisWhere()
                    .WikiId(wikiModel.WikiId)
                    .Ver(Forms.Int("Ver")),
                tableType: Sqls.TableTypes.NormalAndHistory);
            wikiModel.VerType = Forms.Bool("Latest")
                ? Versions.VerTypes.Latest
                : Versions.VerTypes.History;
            return EditorResponse(ss, wikiModel).ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string PreviewTemplate(SiteSettings ss, string body)
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
                                ss: ss,
                                wikiModel: new WikiModel() { Body = body },
                                preview: true)))
                                    .ToString();
        }
    }
}
