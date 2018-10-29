﻿using Implem.DefinitionAccessor;
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
    public static class SiteUtilities
    {
        public static string Index(Context context, SiteSettings ss)
        {
            var hb = new HtmlBuilder();
            var view = Views.GetBySession(context: context, ss: ss);
            var gridData = GetGridData(context: context, ss: ss, view: view);
            var viewMode = ViewModes.GetSessionData(
                context: context,
                siteId: ss.SiteId);
            return hb.ViewModeTemplate(
                context: context,
                ss: ss,
                gridData: gridData,
                view: view,
                viewMode: viewMode,
                viewModeBody: () => hb.Grid(
                   context: context,
                   gridData: gridData,
                   ss: ss,
                   view: view));
        }

        private static string ViewModeTemplate(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            GridData gridData,
            View view,
            string viewMode,
            Action viewModeBody)
        {
            var invalid = SiteValidators.OnEntry(
                context: context,
                ss: ss);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return HtmlTemplates.Error(context, invalid);
            }
            return hb.Template(
                context: context,
                ss: ss,
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.Index,
                siteId: ss.SiteId,
                parentId: ss.ParentId,
                referenceType: "Sites",
                script: JavaScripts.ViewMode(viewMode),
                userScript: ss.ViewModeScripts(context: context),
                userStyle: ss.ViewModeStyles(context: context),
                action: () => hb
                    .Form(
                        attributes: new HtmlAttributes()
                            .Id("SitesForm")
                            .Class("main-form")
                            .Action(Locations.ItemAction(ss.SiteId)),
                        action: () => hb
                            .ViewSelector(context: context, ss: ss, view: view)
                            .ViewFilters(context: context, ss: ss, view: view)
                            .Aggregations(
                                context: context,
                                ss: ss,
                                aggregations: gridData.Aggregations)
                            .Div(id: "ViewModeContainer", action: () => viewModeBody())
                            .MainCommands(
                                context: context,
                                ss: ss,
                                siteId: ss.SiteId,
                                verType: Versions.VerTypes.Latest)
                            .Div(css: "margin-bottom")
                            .Hidden(controlId: "TableName", value: "Sites")
                            .Hidden(controlId: "BaseUrl", value: Locations.BaseUrl()))
                    .EditorDialog(context: context, ss: ss)
                    .DropDownSearchDialog(
                        context: context,
                        controller: "items",
                        id: ss.SiteId)
                    .MoveDialog(context: context, bulk: true)
                    .Div(attributes: new HtmlAttributes()
                        .Id("ExportSelectorDialog")
                        .Class("dialog")
                        .Title(Displays.Export(context: context))))
                    .ToString();
        }

        public static string IndexJson(Context context, SiteSettings ss)
        {
            var view = Views.GetBySession(context: context, ss: ss);
            var gridData = GetGridData(context: context, ss: ss, view: view);
            return new ResponseCollection()
                .ViewMode(
                    context: context,
                    ss: ss,
                    view: view,
                    gridData: gridData,
                    invoke: "setGrid",
                    body: new HtmlBuilder()
                        .Grid(
                            context: context,
                            ss: ss,
                            gridData: gridData,
                            view: view))
                .ToJson();
        }

        private static GridData GetGridData(
            Context context, SiteSettings ss, View view, int offset = 0)
        {
            ss.SetColumnAccessControls(context: context);
            return new GridData(
                context: context,
                ss: ss,
                view: view,
                where: Rds.SitesWhere().TenantId(context.TenantId),
                offset: offset,
                pageSize: ss.GridPageSize.ToInt(),
                countRecord: true,
                aggregations: ss.Aggregations);
        }

        private static HtmlBuilder Grid(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            GridData gridData,
            View view,
            string action = "GridRows")
        {
            return hb
                .Table(
                    attributes: new HtmlAttributes()
                        .Id("Grid")
                        .Class(ss.GridCss())
                        .DataValue("back", _using: ss?.IntegratedSites?.Any() == true)
                        .DataAction(action)
                        .DataMethod("post"),
                    action: () => hb
                        .GridRows(
                            context: context,
                            ss: ss,
                            gridData: gridData,
                            view: view,
                            action: action))
                .Hidden(
                    controlId: "GridOffset",
                    value: ss.GridNextOffset(
                        0,
                        gridData.DataRows.Count(),
                        gridData.Aggregations.TotalCount)
                            .ToString())
                .Button(
                    controlId: "ViewSorter",
                    controlCss: "hidden",
                    action: action,
                    method: "post")
                .Button(
                    controlId: "ViewSorters_Reset",
                    controlCss: "hidden",
                    action: action,
                    method: "post");
        }

        public static string GridRows(
            Context context,
            SiteSettings ss,
            ResponseCollection res = null,
            int offset = 0,
            bool clearCheck = false,
            string action = "GridRows",
            Message message = null)
        {
            var view = Views.GetBySession(context: context, ss: ss);
            var gridData = GetGridData(
                context: context,
                ss: ss,
                view: view,
                offset: offset);
            return (res ?? new ResponseCollection())
                .Remove(".grid tr", _using: offset == 0)
                .ClearFormData("GridCheckAll", _using: clearCheck)
                .ClearFormData("GridUnCheckedItems", _using: clearCheck)
                .ClearFormData("GridCheckedItems", _using: clearCheck)
                .CloseDialog()
                .ReplaceAll("#CopyDirectUrlToClipboard", new HtmlBuilder()
                    .CopyDirectUrlToClipboard(
                        context: context,
                        ss: ss))
                .ReplaceAll("#Aggregations", new HtmlBuilder().Aggregations(
                    context: context,
                    ss: ss,
                    aggregations: gridData.Aggregations),
                    _using: offset == 0)
                .Append("#Grid", new HtmlBuilder().GridRows(
                    context: context,
                    ss: ss,
                    gridData: gridData,
                    view: view,
                    addHeader: offset == 0,
                    clearCheck: clearCheck,
                    action: action))
                .Val("#GridOffset", ss.GridNextOffset(
                    offset,
                    gridData.DataRows.Count(),
                    gridData.Aggregations.TotalCount))
                .Paging("#Grid")
                .Message(message)
                .ToJson();
        }

        private static HtmlBuilder GridRows(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            GridData gridData,
            View view,
            bool addHeader = true,
            bool clearCheck = false,
            string action = "GridRows")
        {
            var checkAll = clearCheck ? false : Forms.Bool("GridCheckAll");
            var columns = ss.GetGridColumns(
                context: context,
                view: view,
                checkPermission: true);
            return hb
                .THead(
                    _using: addHeader,
                    action: () => hb
                        .GridHeader(
                            context: context,
                            columns: columns, 
                            view: view,
                            sort: false,
                            checkAll: checkAll,
                            action: action))
                .TBody(action: () => gridData.TBody(
                    hb: hb,
                    context: context,
                    ss: ss,
                    columns: columns,
                    checkAll: checkAll));
        }

        private static SqlColumnCollection GridSqlColumnCollection(
            Context context, SiteSettings ss)
        {
            var sqlColumnCollection = Rds.SitesColumn();
            new List<string> { "SiteId", "SiteId", "Creator", "Updator" }
                .Concat(ss.GridColumns)
                .Concat(ss.IncludedColumns())
                .Concat(ss.GetUseSearchLinks(context: context).Select(o => o.ColumnName))
                .Concat(ss.TitleColumns)
                    .Distinct().ForEach(column =>
                        sqlColumnCollection.SitesColumn(column));
            return sqlColumnCollection;
        }

        private static SqlColumnCollection DefaultSqlColumns(
            Context context, SiteSettings ss)
        {
            var sqlColumnCollection = Rds.SitesColumn();
            new List<string> { "SiteId", "SiteId", "Creator", "Updator" }
                .Concat(ss.IncludedColumns())
                .Concat(ss.GetUseSearchLinks(context: context).Select(o => o.ColumnName))
                .Concat(ss.TitleColumns)
                    .Distinct().ForEach(column =>
                        sqlColumnCollection.SitesColumn(column));
            return sqlColumnCollection;
        }

        public static string TrashBox(Context context, SiteSettings ss)
        {
            var hb = new HtmlBuilder();
            var view = Views.GetBySession(context: context, ss: ss);
            var gridData = GetGridData(context: context, ss: ss, view: view);
            var viewMode = ViewModes.GetSessionData(
                context: context,
                siteId: ss.SiteId);
            return hb.ViewModeTemplate(
                context: context,
                ss: ss,
                gridData: gridData,
                view: view,
                viewMode: viewMode,
                viewModeBody: () => hb
                    .TrashBoxCommands(context: context, ss: ss)
                    .Grid(
                        context: context,
                        ss: ss,
                        gridData: gridData,
                        view: view,
                        action: "TrashBoxGridRows"));
        }

        public static string TrashBoxJson(Context context, SiteSettings ss)
        {
            var view = Views.GetBySession(context: context, ss: ss);
            var gridData = GetGridData(context: context, ss: ss, view: view);
            return new ResponseCollection()
                .ViewMode(
                    context: context,
                    ss: ss,
                    view: view,
                    gridData: gridData,
                    invoke: "setGrid",
                    body: new HtmlBuilder()
                        .TrashBoxCommands(context: context, ss: ss)
                        .Grid(
                            context: context,
                            ss: ss,
                            gridData: gridData,
                            view: view,
                            action: "TrashBoxGridRows"))
                .ToJson();
        }

        public static HtmlBuilder TdValue(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Column column,
            SiteModel siteModel)
        {
            if (!column.GridDesign.IsNullOrEmpty())
            {
                return hb.TdCustomValue(
                    context: context,
                    ss: ss,
                    gridDesign: column.GridDesign,
                    siteModel: siteModel);
            }
            else
            {
                var mine = siteModel.Mine(context: context);
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
                                    value: siteModel.SiteId)
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
                                    value: siteModel.UpdatedTime)
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
                                    value: siteModel.Ver)
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
                                    value: siteModel.Title)
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
                                    value: siteModel.Body)
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
                                    value: siteModel.TitleBody)
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
                                    value: siteModel.Comments)
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
                                    value: siteModel.Creator)
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
                                    value: siteModel.Updator)
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
                                    value: siteModel.CreatedTime)
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
            SiteModel siteModel)
        {
            ss.IncludedColumns(gridDesign).ForEach(column =>
            {
                var value = string.Empty;
                switch (column.Name)
                {
                    case "SiteId": value = siteModel.SiteId.GridText(
                        context: context,
                        column: column); break;
                    case "UpdatedTime": value = siteModel.UpdatedTime.GridText(
                        context: context,
                        column: column); break;
                    case "Ver": value = siteModel.Ver.GridText(
                        context: context,
                        column: column); break;
                    case "Title": value = siteModel.Title.GridText(
                        context: context,
                        column: column); break;
                    case "Body": value = siteModel.Body.GridText(
                        context: context,
                        column: column); break;
                    case "TitleBody": value = siteModel.TitleBody.GridText(
                        context: context,
                        column: column); break;
                    case "Comments": value = siteModel.Comments.GridText(
                        context: context,
                        column: column); break;
                    case "Creator": value = siteModel.Creator.GridText(
                        context: context,
                        column: column); break;
                    case "Updator": value = siteModel.Updator.GridText(
                        context: context,
                        column: column); break;
                    case "CreatedTime": value = siteModel.CreatedTime.GridText(
                        context: context,
                        column: column); break;
                }
                gridDesign = gridDesign.Replace("[" + column.ColumnName + "]", value);
            });
            return hb.Td(action: () => hb
                .Div(css: "markup", action: () => hb
                    .Text(text: gridDesign)));
        }

        public static string EditorJson(Context context, SiteModel siteModel)
        {
            siteModel.ClearSessions(context: context);
            return EditorResponse(context: context, siteModel: siteModel).ToJson();
        }

        private static ResponseCollection EditorResponse(
            Context context,
            SiteModel siteModel,
            Message message = null,
            string switchTargets = null)
        {
            siteModel.MethodType = BaseModel.MethodTypes.Edit;
            return new SitesResponseCollection(siteModel)
                .Invoke("clearDialogs")
                .ReplaceAll("#MainContainer", Editor(context, siteModel))
                .Val("#SwitchTargets", switchTargets, _using: switchTargets != null)
                .SetMemory("formChanged", false)
                .Invoke("setCurrentIndex")
                .Message(message)
                .ClearFormData();
        }

        private static HtmlBuilder ReferenceType(
            this HtmlBuilder hb,
            Context context,
            string referenceType,
            BaseModel.MethodTypes methodType)
        {
            return methodType == BaseModel.MethodTypes.New
                ? hb.Select(
                    attributes: new HtmlAttributes()
                        .Id("Sites_ReferenceType")
                        .Class("control-dropdown"),
                    action: () => hb
                        .OptionCollection(
                            context: context,
                            optionCollection: new Dictionary<string, ControlData>
                            {
                                {
                                    "Sites",
                                    new ControlData(ReferenceTypeDisplayName(
                                        context: context,
                                        referenceType: "Sites"))
                                },
                                {
                                    "Issues",
                                    new ControlData(ReferenceTypeDisplayName(
                                        context: context,
                                        referenceType: "Issues"))
                                }
,
                                {
                                    "Results",
                                    new ControlData(ReferenceTypeDisplayName(
                                        context: context,
                                        referenceType: "Results"))
                                }
,
                                {
                                    "Wikis",
                                    new ControlData(ReferenceTypeDisplayName(
                                        context: context,
                                        referenceType: "Wikis"))
                                }
                            },
                        selectedValue: referenceType))
                : hb.Span(css: "control-text", action: () => hb
                    .Text(text: ReferenceTypeDisplayName(
                        context: context,
                        referenceType: referenceType)));
        }

        private static string ReferenceTypeDisplayName(Context context, string referenceType)
        {
            switch (referenceType)
            {
                case "Sites": return Displays.Folder(context: context);
                case "Issues": return Displays.Get(context: context, id: "Issues");
                case "Results": return Displays.Get(context: context, id: "Results");
                case "Wikis": return Displays.Get(context: context, id: "Wikis");
                default: return null;
            }
        }

        public static string Create(Context context, long parentId, long inheritPermission)
        {
            var siteModel = new SiteModel(
                context: context,
                parentId: parentId,
                inheritPermission: inheritPermission,
                setByForm: true);
            var ss = siteModel.SitesSiteSettings(context: context, referenceId: parentId);
            if (context.ContractSettings.SitesLimit(context: context))
            {
                return Error.Types.SitesLimit.MessageJson(context: context);
            }
            if (parentId == 0)
            {
                ss.PermissionType = SiteTopPermission(context: context);
            }
            var invalid = SiteValidators.OnCreating(
                context: context,
                ss: ss,
                siteModel: siteModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var error = siteModel.Create(context: context);
            switch (error)
            {
                case Error.Types.None:
                    SessionUtilities.Set(
                        context: context,
                        message: Messages.Created(
                            context: context,
                            data: siteModel.Title.Value));
                    return new ResponseCollection()
                        .SetMemory("formChanged", false)
                        .Href(Locations.Edit(
                            controller: context.Controller,
                            id: siteModel.ReferenceType == "Wikis"
                                ? Rds.ExecuteScalar_long(
                                    context: context,
                                    statements: Rds.SelectWikis(
                                        column: Rds.WikisColumn().WikiId(),
                                        where: Rds.WikisWhere().SiteId(siteModel.SiteId)))
                                : siteModel.SiteId))
                        .ToJson();
                default:
                    return error.MessageJson(context: context);
            }
        }

        public static string Update(Context context, SiteModel siteModel, long siteId)
        {
            siteModel.SetByForm(context: context);
            siteModel.SiteSettings = SiteSettingsUtilities.Get(
                context: context, siteModel: siteModel, referenceId: siteId);
            var ss = siteModel.SiteSettings.SiteSettingsOnUpdate(context: context);
            var invalid = SiteValidators.OnUpdating(
                context: context,
                ss: ss,
                siteModel: siteModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            if (siteModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts(context: context).ToJson();
            }
            if (Forms.Exists("InheritPermission"))
            {
                siteModel.InheritPermission = Forms.Long("InheritPermission");
                ss.InheritPermission = siteModel.InheritPermission;
            }
            var error = siteModel.Update(
                context: context,
                ss: ss,
                permissions: Forms.List("CurrentPermissionsAll"),
                permissionChanged:
                    Forms.Exists("InheritPermission") ||
                    Forms.Exists("CurrentPermissionsAll"));
            switch (error)
            {
                case Error.Types.None:
                    var res = new SitesResponseCollection(siteModel);
                    res.ReplaceAll("#Breadcrumb", new HtmlBuilder()
                        .Breadcrumb(context: context, ss: ss));
                    return ResponseByUpdate(res, context, siteModel)
                        .PrependComment(
                            context: context,
                            ss: ss,
                            column: ss.GetColumn(context: context, columnName: "Comments"),
                            comments: siteModel.Comments,
                            verType: siteModel.VerType)
                        .ToJson();
                case Error.Types.UpdateConflicts:
                    return Messages.ResponseUpdateConflicts(
                        context: context,
                        data: siteModel.Updator.Name)
                            .ToJson();
                default:
                    return error.MessageJson(context: context);
            }
        }

        private static ResponseCollection ResponseByUpdate(
            SitesResponseCollection res,
            Context context,
            SiteModel siteModel)
        {
            var ss = siteModel.SiteSettings;
            if (Forms.Bool("IsDialogEditorForm"))
            {
                var view = Views.GetBySession(
                    context: context,
                    ss: ss);
                var gridData = new GridData(
                    context: context,
                    ss: ss,
                    view: view,
                    where: Rds.SitesWhere().SiteId(siteModel.SiteId));
                var columns = ss.GetGridColumns(
                    context: context,
                    checkPermission: true);
                return res
                    .ReplaceAll(
                        $"[data-id=\"{siteModel.SiteId}\"]",
                        gridData.TBody(
                            hb: new HtmlBuilder(),
                            context: context,
                            ss: ss,
                            columns: columns,
                            checkAll: false))
                    .CloseDialog()
                    .Message(Messages.Updated(
                        context: context,
                        data: siteModel.Title.DisplayValue));
            }
            else
            {
                return res
                    .Ver(context: context, ss: ss)
                    .Timestamp(context: context, ss: ss)
                    .Val("#VerUp", false)
                    .Disabled("#VerUp", false)
                    .Html("#HeaderTitle", siteModel.Title.Value)
                    .Html("#RecordInfo", new HtmlBuilder().RecordInfo(
                        context: context,
                        baseModel: siteModel,
                        tableName: "Sites"))
                    .SetMemory("formChanged", false)
                    .Message(Messages.Updated(
                        context: context,
                        data: siteModel.Title.Value))
                    .Comment(
                        context: context,
                        ss: ss,
                        column: ss.GetColumn(context: context, columnName: "Comments"),
                        comments: siteModel.Comments,
                        deleteCommentId: siteModel.DeleteCommentId)
                    .ClearFormData();
            }
        }

        public static string Copy(Context context, SiteModel siteModel)
        {
            var ss = siteModel.SiteSettings;
            if (context.ContractSettings.SitesLimit(context: context))
            {
                return Error.Types.SitesLimit.MessageJson(context: context);
            }
            siteModel.Title.Value += Displays.SuffixCopy(context: context);
            if (!Forms.Bool("CopyWithComments"))
            {
                siteModel.Comments.Clear();
            }
            var error = siteModel.Create(context: context, otherInitValue: true);
            return error.Has()
                ? error.MessageJson(context: context)
                : EditorResponse(
                    context: context,
                    siteModel: siteModel,
                    message: Messages.Copied(context: context)).ToJson();
        }

        public static string Delete(Context context, SiteSettings ss, long siteId)
        {
            var siteModel = new SiteModel(context, siteId);
            var invalid = SiteValidators.OnDeleting(
                context: context,
                ss: ss,
                siteModel: siteModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var error = siteModel.Delete(context: context, ss: ss);
            switch (error)
            {
                case Error.Types.None:
                    SessionUtilities.Set(
                        context: context,
                        message: Messages.Deleted(
                            context: context,
                            data: siteModel.Title.Value));
                    var res = new SitesResponseCollection(siteModel);
                res
                    .SetMemory("formChanged", false)
                    .Href(Locations.ItemIndex(siteModel.ParentId));
                    return res.ToJson();
                default:
                    return error.MessageJson(context: context);
            }
        }

        public static string Restore(Context context, SiteSettings ss)
        {
            if (!Parameters.Deleted.Restore)
            {
                return Error.Types.InvalidRequest.MessageJson(context: context);
            }
            else if (context.CanManageSite(ss: ss))
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
                        return Messages.ResponseSelectTargets(context: context).ToJson();
                    }
                }
                return GridRows(
                    context: context,
                    ss: ss,
                    clearCheck: true,
                    message: Messages.BulkRestored(
                        context: context,
                        data: count.ToString()));
            }
            else
            {
                return Messages.ResponseHasNotPermission(context: context).ToJson();
            }
        }

        public static int Restore(Context context, SiteSettings ss, List<long> selected, bool negative = false)
        {
            var where = Rds.SitesWhere()
                .SiteId_In(
                    value: selected,
                    tableName: "Sites_Deleted",
                    negative: negative,
                    _using: selected.Any());
            var sub = Rds.SelectSites(
                tableType: Sqls.TableTypes.Deleted,
                _as: "Sites_Deleted",
                column: Rds.SitesColumn()
                    .SiteId(tableName: "Sites_Deleted"),
                where: where);
            return Rds.ExecuteScalar_response(
                context: context,
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreItems(where: Rds.ItemsWhere()
                        .ReferenceId_In(sub:
                            Rds.SelectWikis(
                                tableType: Sqls.TableTypes.Deleted,
                                column: Rds.WikisColumn().WikiId(),
                                where: Rds.WikisWhere().SiteId_In(sub: sub)))
                        .ReferenceType("Wikis")),
                    Rds.RestoreWikis(where: Rds.WikisWhere().SiteId_In(sub: sub)),
                    Rds.RestoreItems(where: Rds.ItemsWhere().ReferenceId_In(sub: sub)),
                    Rds.RestoreSites(where: where, countRecord: true)
                }).Count.ToInt();
        }

        public static string RestoreFromHistory(Context context, SiteSettings ss, long siteId)
        {
            if (!Parameters.History.Restore)
            {
                return Error.Types.InvalidRequest.MessageJson(context: context);
            }
            var siteModel = new SiteModel(
                context: context,
                siteId: siteId);
            var invalid = SiteValidators.OnUpdating(
                context: context,
                ss: ss,
                siteModel: siteModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var ver = Forms.Data("GridCheckedItems")
                .Split(',')
                .Where(o => !o.IsNullOrEmpty())
                .ToList();
            if (ver.Count() != 1)
            {
                return Error.Types.SelectOne.MessageJson(context: context);
            }
            siteModel.SetByModel(new SiteModel().Get(
                context: context,
                tableType: Sqls.TableTypes.History,
                where: Rds.SitesWhere()
                    .SiteId(ss.SiteId)
                    .SiteId(siteId)
                    .Ver(ver.First())));
            siteModel.VerUp = true;
            var error = siteModel.Update(
                context: context, ss: ss, setBySession: false, otherInitValue: true);
            switch (error)
            {
                case Error.Types.None:
                    SessionUtilities.Set(
                        context: context,
                        message: Messages.RestoredFromHistory(
                            context: context,
                            data: ver.First().ToString()));
                    return  new ResponseCollection()
                        .SetMemory("formChanged", false)
                        .Href(Locations.ItemEdit(siteId))
                        .ToJson();
                default:
                    return error.MessageJson(context: context);
            }
        }

        public static string Histories(Context context, SiteModel siteModel, Message message = null)
        {
            var ss = siteModel.SiteSettings;
            var columns = new SiteSettings(context: context, referenceType: "Sites")
                .GetHistoryColumns(context: context);
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
                                columns: columns,
                                sort: false,
                                checkRow: true))
                        .TBody(action: () => hb
                            .HistoriesTableBody(
                                context: context,
                                ss: ss,
                                columns: columns,
                                siteModel: siteModel)));
            return new SitesResponseCollection(siteModel)
                .Html("#FieldSetHistories", hb)
                .Message(message)
                .ToJson();
        }

        private static void HistoriesTableBody(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            List<Column> columns,
            SiteModel siteModel)
        {
            new SiteCollection(
                context: context,
                column: HistoryColumn(columns),
                where: Rds.SitesWhere().SiteId(siteModel.SiteId),
                orderBy: Rds.SitesOrderBy().Ver(SqlOrderBy.Types.desc),
                tableType: Sqls.TableTypes.NormalAndHistory)
                    .ForEach(siteModelHistory => hb
                        .Tr(
                            attributes: new HtmlAttributes()
                                .Class("grid-row")
                                .DataAction("History")
                                .DataMethod("post")
                                .DataVer(siteModelHistory.Ver)
                                .DataLatest(1, _using:
                                    siteModelHistory.Ver == siteModel.Ver),
                            action: () =>
                            {
                                hb.Td(
                                    css: "grid-check-td",
                                    action: () => hb
                                        .CheckBox(
                                            controlCss: "grid-check",
                                            _checked: false,
                                            dataId: siteModelHistory.Ver.ToString(),
                                            _using: siteModelHistory.Ver < siteModel.Ver));
                                columns
                                    .ForEach(column => hb
                                        .TdValue(
                                            context: context,
                                            ss: ss,
                                            column: column,
                                            siteModel: siteModelHistory));
                            }));
        }

        private static SqlColumnCollection HistoryColumn(List<Column> columns)
        {
            var sqlColumn = new Rds.SitesColumnCollection()
                .SiteId()
                .Ver();
            columns.ForEach(column => sqlColumn.SitesColumn(column.ColumnName));
            return sqlColumn;
        }

        public static string History(Context context, SiteModel siteModel)
        {
            return EditorResponse(context: context, siteModel: siteModel).ToJson();
        }

        public static string DeleteHistory(Context context, SiteSettings ss, long siteId)
        {
            if (!Parameters.History.PhysicalDelete)
            {
                return Error.Types.InvalidRequest.MessageJson(context: context);
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
                        siteId: siteId,
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
                            siteId: siteId,
                            selected: selected);
                    }
                    else
                    {
                        return Messages.ResponseSelectTargets(context: context).ToJson();
                    }
                }
                var siteModel = new SiteModel(context: context, siteId: siteId);
                siteModel.SiteSettings = SiteSettingsUtilities.Get(
                    context: context,
                    siteModel: siteModel,
                    referenceId: siteId,
                    tableType: ss.TableType);
                return Histories(
                    context: context,
                    siteModel: siteModel,
                    message: Messages.HistoryDeleted(
                        context: context,
                        data: count.ToString()));
            }
            else
            {
                return Messages.ResponseHasNotPermission(context: context).ToJson();
            }
        }

        private static int DeleteHistory(
            Context context,
            SiteSettings ss,
            long siteId,
            List<int> selected,
            bool negative = false)
        {
            return Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: Rds.PhysicalDeleteSites(
                    tableType: Sqls.TableTypes.History,
                    where: Rds.SitesWhere()
                        .TenantId(
                            value: context.TenantId,
                            tableName: "Sites_History")
                        .SiteId(
                            value: ss.SiteId,
                            tableName: "Sites_History")
                        .Ver_In(
                            value: selected,
                            tableName: "Sites_History",
                            negative: negative,
                            _using: selected.Any()),
                    countRecord: true)).Count.ToInt();
        }

        public static string PhysicalDelete(Context context, SiteSettings ss)
        {
            if (!Parameters.Deleted.PhysicalDelete)
            {
                return Error.Types.InvalidRequest.MessageJson(context: context);
            }
            if (context.CanManageSite(ss: ss))
            {
                var selector = new GridSelector();
                var count = 0;
                if (selector.All)
                {
                    count = PhysicalDelete(
                        context: context,
                        ss: ss,
                        selected: selector.Selected,
                        negative: true);
                }
                else
                {
                    if (selector.Selected.Any())
                    {
                        count = PhysicalDelete(
                            context: context,
                            ss: ss,
                            selected: selector.Selected);
                    }
                    else
                    {
                        return Messages.ResponseSelectTargets(context: context).ToJson();
                    }
                }
                return GridRows(
                    context: context,
                    ss: ss,
                    clearCheck: true,
                    message: Messages.PhysicalDeleted(
                        context: context,
                        data: count.ToString()));
            }
            else
            {
                return Messages.ResponseHasNotPermission(context: context).ToJson();
            }
        }

        private static int PhysicalDelete(
            Context context, SiteSettings ss, List<long> selected, bool negative = false)
        {
            var where = Rds.SitesWhere()
                .TenantId(
                    value: context.TenantId,
                    tableName: "Sites_Deleted")
                .ParentId(
                    value: ss.SiteId,
                    tableName: "Sites_Deleted")
                .SiteId_In(
                    value: selected,
                    tableName: "Sites_Deleted",
                    negative: negative,
                    _using: selected.Any());
            var sub = Rds.SelectSites(
                tableType: Sqls.TableTypes.Deleted,
                _as: "Sites_Deleted",
                column: Rds.SitesColumn()
                    .SiteId(tableName: "Sites_Deleted"),
                where: where);
            return Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.PhysicalDeleteItems(
                        tableType: Sqls.TableTypes.Deleted,
                        where: Rds.ItemsWhere()
                            .ReferenceId_In(sub:
                                Rds.SelectWikis(
                                    tableType: Sqls.TableTypes.Deleted,
                                    column: Rds.WikisColumn().WikiId(),
                                    where: Rds.WikisWhere().SiteId_In(sub: sub)))
                            .ReferenceType("Wikis")),
                    Rds.PhysicalDeleteWikis(
                        tableType: Sqls.TableTypes.Deleted,
                        where: Rds.WikisWhere().SiteId_In(sub: sub)),
                    Rds.PhysicalDeleteItems(
                        tableType: Sqls.TableTypes.Deleted,
                        where: Rds.ItemsWhere().ReferenceId_In(sub: sub)),
                    Rds.PhysicalDeleteSites(
                        tableType: Sqls.TableTypes.Deleted,
                        where: where,
                        countRecord: true)
                }).Count.ToInt();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Templates(Context context, long parentId, long inheritPermission)
        {
            var siteModel = new SiteModel(
                context: context,
                parentId: parentId,
                inheritPermission: inheritPermission);
            var ss = siteModel.SitesSiteSettings(context: context, referenceId: parentId);
            if (context.ContractSettings.SitesLimit(context: context))
            {
                return Error.Types.SitesLimit.MessageJson(context: context);
            }
            if (parentId == 0)
            {
                ss.PermissionType = SiteTopPermission(context: context);
            }
            var invalid = SiteValidators.OnCreating(
                context: context, ss: ss, siteModel: siteModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var hb = new HtmlBuilder();
            return new ResponseCollection()
                .Html("#SiteMenu", new HtmlBuilder().TemplateTabsContainer(
                    context: context,
                    ss: ss))
                .ReplaceAll("#MainCommandsContainer", hb
                    .MainCommands(
                        context: context,
                        ss: ss,
                        siteId: ss.SiteId,
                        verType: Versions.VerTypes.Latest,
                        backButton: false,
                        extensions: () => hb
                            .Button(
                                text: Displays.GoBack(context: context),
                                controlCss: "button-icon",
                                accessKey: "q",
                                onClick: "$p.send($(this),'SitesForm');",
                                icon: "ui-icon-disk",
                                action: "SiteMenu",
                                method: "post")
                            .Button(
                                controlId: "OpenSiteTitleDialog",
                                text: Displays.Create(context: context),
                                controlCss: "button-icon hidden",
                                onClick: "$p.openSiteTitleDialog($(this));",
                                icon: "ui-icon-disk")))
                .Invoke("setTemplate")
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder TemplateTabsContainer(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return hb
                .Div(id: "TemplateTabsContainer", css: "max", action: () => hb
                    .Ul(id: "EditorTabs", action: () => hb
                        .Li(
                            action: () => hb
                                .A(
                                    href: "#FieldSetStandard",
                                    text: Displays.Standard(context: context)),
                            _using: Def.TemplateDefinitionCollection
                                .Any(o => o.Standard > 0))
                        .Li(
                            action: () => hb
                                .A(
                                    href: "#FieldSetProject",
                                    text: Displays.Project(context: context)),
                            _using: Def.TemplateDefinitionCollection
                                .Any(o => o.Project > 0))
                        .Li(
                            action: () => hb
                                .A(
                                    href: "#FieldSetBusinessImprovement",
                                    text: Displays.BusinessImprovement(context: context)),
                            _using: Def.TemplateDefinitionCollection
                                .Any(o => o.BusinessImprovement > 0))
                        .Li(
                            action: () => hb
                                .A(
                                    href: "#FieldSetSales",
                                    text: Displays.Sales(context: context)),
                            _using: Def.TemplateDefinitionCollection
                                .Any(o => o.Sales > 0))
                        .Li(
                            action: () => hb
                                .A(
                                    href: "#FieldSetCustomer",
                                    text: Displays.Customer(context: context)),
                            _using: Def.TemplateDefinitionCollection
                                .Any(o => o.Customer > 0))
                        .Li(
                            action: () => hb
                                .A(
                                    href: "#FieldSetStore",
                                    text: Displays.Store(context: context)),
                            _using: Def.TemplateDefinitionCollection
                                .Any(o => o.Store > 0))
                        .Li(
                            action: () => hb
                                .A(
                                    href: "#FieldSetResearchAndDevelopment",
                                    text: Displays.ResearchAndDevelopment(context: context)),
                            _using: Def.TemplateDefinitionCollection
                                .Any(o => o.ResearchAndDevelopment > 0))
                        .Li(
                            action: () => hb
                                .A(
                                    href: "#FieldSetMarketing",
                                    text: Displays.Marketing(context: context)),
                            _using: Def.TemplateDefinitionCollection
                                .Any(o => o.Marketing > 0))
                        .Li(
                            action: () => hb
                                .A(
                                    href: "#FieldSetManufacture",
                                    text: Displays.Manufacture(context: context)),
                            _using: Def.TemplateDefinitionCollection
                                .Any(o => o.Manufacture > 0))
                        .Li(
                            action: () => hb
                                .A(
                                    href: "#FieldSetInformationSystem",
                                    text: Displays.InformationSystem(context: context)),
                            _using: Def.TemplateDefinitionCollection
                                .Any(o => o.InformationSystem > 0))
                        .Li(
                            action: () => hb
                                .A(
                                    href: "#FieldSetCorporatePlanning",
                                    text: Displays.CorporatePlanning(context: context)),
                            _using: Def.TemplateDefinitionCollection
                                .Any(o => o.CorporatePlanning > 0))
                        .Li(
                            action: () => hb
                                .A(
                                    href: "#FieldSetHumanResourcesAndGeneralAffairs",
                                    text: Displays.HumanResourcesAndGeneralAffairs(context: context)),
                            _using: Def.TemplateDefinitionCollection
                                .Any(o => o.HumanResourcesAndGeneralAffairs > 0))
                        .Li(
                            action: () => hb
                                .A(
                                    href: "#FieldSetEducation",
                                    text: Displays.Education(context: context)),
                            _using: Def.TemplateDefinitionCollection
                                .Any(o => o.Education > 0))
                        .Li(
                            action: () => hb
                                .A(
                                    href: "#FieldSetPurchase",
                                    text: Displays.Purchase(context: context)),
                            _using: Def.TemplateDefinitionCollection
                                .Any(o => o.Purchase > 0))
                        .Li(
                            action: () => hb
                                .A(
                                    href: "#FieldSetLogistics",
                                    text: Displays.Logistics(context: context)),
                            _using: Def.TemplateDefinitionCollection
                                .Any(o => o.Logistics > 0))
                        .Li(
                            action: () => hb
                                .A(
                                    href: "#FieldSetLegalAffairs",
                                    text: Displays.LegalAffairs(context: context)),
                            _using: Def.TemplateDefinitionCollection
                                .Any(o => o.LegalAffairs > 0))
                        .Li(
                            action: () => hb
                                .A(
                                    href: "#FieldSetProductList",
                                    text: Displays.ProductList(context: context)),
                            _using: Def.TemplateDefinitionCollection
                                .Any(o => o.ProductList > 0))
                        .Li(
                            action: () => hb
                                .A(
                                    href: "#FieldSetClassification",
                                    text: Displays.Classification(context: context)),
                            _using: Def.TemplateDefinitionCollection
                                .Any(o => o.Classification > 0)))
                    .TemplateTab(
                        context: context,
                        name: "Standard",
                        templates: Def.TemplateDefinitionCollection
                            .Where(o => o.Standard > 0)
                            .OrderBy(o => o.Standard))
                    .TemplateTab(
                        context: context,
                        name: "Project",
                        templates: Def.TemplateDefinitionCollection
                            .Where(o => o.Project > 0)
                            .OrderBy(o => o.Project))
                    .TemplateTab(
                        context: context,
                        name: "BusinessImprovement",
                        templates: Def.TemplateDefinitionCollection
                            .Where(o => o.BusinessImprovement > 0)
                            .OrderBy(o => o.BusinessImprovement))
                    .TemplateTab(
                        context: context,
                        name: "Sales",
                        templates: Def.TemplateDefinitionCollection
                            .Where(o => o.Sales > 0)
                            .OrderBy(o => o.Sales))
                    .TemplateTab(
                        context: context,
                        name: "Customer",
                        templates: Def.TemplateDefinitionCollection
                            .Where(o => o.Customer > 0)
                            .OrderBy(o => o.Customer))
                    .TemplateTab(
                        context: context,
                        name: "Store",
                        templates: Def.TemplateDefinitionCollection
                            .Where(o => o.Store > 0)
                            .OrderBy(o => o.Store))
                    .TemplateTab(
                        context: context,
                        name: "ResearchAndDevelopment",
                        templates: Def.TemplateDefinitionCollection
                            .Where(o => o.ResearchAndDevelopment > 0)
                            .OrderBy(o => o.ResearchAndDevelopment))
                    .TemplateTab(
                        context: context,
                        name: "Marketing",
                        templates: Def.TemplateDefinitionCollection
                            .Where(o => o.Marketing > 0)
                            .OrderBy(o => o.Marketing))
                    .TemplateTab(
                        context: context,
                        name: "Manufacture",
                        templates: Def.TemplateDefinitionCollection
                            .Where(o => o.Manufacture > 0)
                            .OrderBy(o => o.Manufacture))
                    .TemplateTab(
                        context: context,
                        name: "InformationSystem",
                        templates: Def.TemplateDefinitionCollection
                            .Where(o => o.InformationSystem > 0)
                            .OrderBy(o => o.InformationSystem))
                    .TemplateTab(
                        context: context,
                        name: "CorporatePlanning",
                        templates: Def.TemplateDefinitionCollection
                            .Where(o => o.CorporatePlanning > 0)
                            .OrderBy(o => o.CorporatePlanning))
                    .TemplateTab(
                        context: context,
                        name: "HumanResourcesAndGeneralAffairs",
                        templates: Def.TemplateDefinitionCollection
                            .Where(o => o.HumanResourcesAndGeneralAffairs > 0)
                            .OrderBy(o => o.HumanResourcesAndGeneralAffairs))
                    .TemplateTab(
                        context: context,
                        name: "Education",
                        templates: Def.TemplateDefinitionCollection
                            .Where(o => o.Education > 0)
                            .OrderBy(o => o.Education))
                    .TemplateTab(
                        context: context,
                        name: "Purchase",
                        templates: Def.TemplateDefinitionCollection
                            .Where(o => o.Purchase > 0)
                            .OrderBy(o => o.Purchase))
                    .TemplateTab(
                        context: context,
                        name: "Logistics",
                        templates: Def.TemplateDefinitionCollection
                            .Where(o => o.Logistics > 0)
                            .OrderBy(o => o.Logistics))
                    .TemplateTab(
                        context: context,
                        name: "LegalAffairs",
                        templates: Def.TemplateDefinitionCollection
                            .Where(o => o.LegalAffairs > 0)
                            .OrderBy(o => o.LegalAffairs))
                    .TemplateTab(
                        context: context,
                        name: "ProductList",
                        templates: Def.TemplateDefinitionCollection
                            .Where(o => o.ProductList > 0)
                            .OrderBy(o => o.ProductList))
                    .TemplateTab(
                        context: context,
                        name: "Classification",
                        templates: Def.TemplateDefinitionCollection
                            .Where(o => o.Classification > 0)
                            .OrderBy(o => o.Classification)));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder TemplateTab(
            this HtmlBuilder hb,
            Context context,
            string name,
            IEnumerable<TemplateDefinition> templates)
        {
            return templates.Any()
                ? hb.FieldSet(id: "FieldSet" + name, css: "template", action: () => hb
                    .Div(
                        id: name + "TemplatesViewer",
                        css: "template-viewer-container",
                        action: () => hb
                            .Div(css: "template-viewer", action: () => hb
                                .P(css: "description", action: () => hb
                                    .Text(text: Displays.SelectTemplate(context: context)))
                                .Div(css: "viewer hidden")))
                    .Div(css: "template-selectable", action: () => hb
                        .FieldSelectable(
                            controlId: name + "Templates",
                            fieldCss: "field-vertical",
                            controlContainerCss: "container-selectable",
                            controlWrapperCss: " h350",
                            controlCss: " single applied",
                            listItemCollection: templates.ToDictionary(
                                o => o.Id, o => new ControlData(o.Title)),
                            action: "PreviewTemplate",
                            method: "post")))
                : hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string CreateByTemplate(Context context, long parentId, long inheritPermission)
        {
            var siteModel = new SiteModel(
                context: context,
                parentId: parentId,
                inheritPermission: inheritPermission);
            var ss = siteModel.SitesSiteSettings(context: context, referenceId: parentId);
            if (context.ContractSettings.SitesLimit(context: context))
            {
                return Error.Types.SitesLimit.MessageJson(context: context);
            }
            if (parentId == 0)
            {
                ss.PermissionType = SiteTopPermission(context: context);
            }
            var invalid = SiteValidators.OnCreating(
                context: context, ss: ss, siteModel: siteModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var id = Forms.Data("TemplateId");
            if (id.IsNullOrEmpty())
            {
                return Error.Types.SelectTargets.MessageJson(context: context);
            }
            var templateDefinition = Def.TemplateDefinitionCollection
                .FirstOrDefault(o => o.Id == id);
            if (templateDefinition == null)
            {
                return Error.Types.NotFound.MessageJson(context: context);
            }
            var templateSs = templateDefinition.SiteSettingsTemplate
                .Deserialize<SiteSettings>();
            if (templateSs == null)
            {
                return Error.Types.NotFound.MessageJson(context: context);
            }
            siteModel.ReferenceType = templateSs.ReferenceType;
            siteModel.Title = new Title(Forms.Data("SiteTitle"));
            siteModel.Body = templateDefinition.Body;
            siteModel.SiteSettings = templateSs;
            siteModel.Create(context: context, otherInitValue: true);
            return SiteMenuResponse(
                context: context,
                siteModel: new SiteModel(context: context, siteId: parentId));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string SiteMenuJson(Context context, SiteModel siteModel)
        {
            var ss = siteModel.SitesSiteSettings(
                context: context, referenceId: siteModel.ParentId);
            if (siteModel.ParentId == 0)
            {
                ss.PermissionType = SiteTopPermission(context: context);
            }
            var invalid = SiteValidators.OnCreating(
                context: context, ss: ss, siteModel: siteModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            return SiteMenuResponse(context: context, siteModel: siteModel);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string SiteMenuResponse(Context context, SiteModel siteModel)
        {
            return new ResponseCollection()
                .CloseDialog()
                .ReplaceAll("#SiteMenu", new HtmlBuilder().SiteMenu(
                    context: context,
                    siteModel: siteModel.SiteId != 0 ? siteModel : null,
                    siteConditions: SiteInfo.TenantCaches.Get(context.TenantId)?
                        .SiteMenu
                        .SiteConditions(context: context, ss: siteModel.SiteSettings)))
                .ReplaceAll("#MainCommandsContainer", new HtmlBuilder().MainCommands(
                    context: context,
                    ss: siteModel.SiteSettings,
                    siteId: siteModel.SiteId,
                    verType: siteModel.VerType,
                    backButton: siteModel.SiteId != 0))
                .Invoke("setSiteMenu")
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string MoveSiteMenu(Context context, long id)
        {
            var siteModel = new SiteModel(context: context, siteId: id);
            siteModel.SiteSettings.PermissionType = id == 0
                ? SiteTopPermission(context: context)
                : Permissions.Get(context: context, siteId: id);
            var sourceSiteModel = new SiteModel(
                context: context, siteId: Forms.Long("SiteId"));
            var destinationSiteModel = new SiteModel(
                context: context, siteId: Forms.Long("DestinationId"));
            if (siteModel.NotFound() ||
                sourceSiteModel.NotFound() ||
                destinationSiteModel.NotFound())
            {
                return SiteMenuError(
                    context: context,
                    id: id,
                    siteModel: siteModel,
                    invalid: Error.Types.NotFound);
            }
            if (destinationSiteModel.ReferenceType != "Sites")
            {
                switch (sourceSiteModel.ReferenceType)
                {
                    case "Sites":
                    case "Wikis":
                        return SiteMenuError(
                            context: context,
                            id: id,
                            siteModel: siteModel,
                            invalid: Error.Types.CanNotPerformed);
                    default:
                        return LinkDialog(
                            context: context,
                            id: id,
                            siteModel: siteModel,
                            sourceSiteModel: sourceSiteModel,
                            destinationSiteModel: destinationSiteModel);
                }
            }
            var toParent = id != 0 &&
                SiteInfo.TenantCaches.Get(context.TenantId)
                    .SiteMenu.Get(id).ParentId == destinationSiteModel.SiteId;
            var invalid = SiteValidators.OnMoving(
                context: context,
                currentId: id,
                destinationId: destinationSiteModel.SiteId,
                current: SiteSettingsUtilities.Get(
                    context: context,
                    siteModel: siteModel,
                    referenceId: id),
                source: SiteSettingsUtilities.Get(
                    context: context,
                    siteId: sourceSiteModel.SiteId,
                    referenceId: sourceSiteModel.SiteId),
                destination: SiteSettingsUtilities.Get(
                    context: context,
                    siteId: destinationSiteModel.SiteId,
                    referenceId: destinationSiteModel.SiteId));
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return SiteMenuError(
                    context: context,
                    id: id,
                    siteModel: siteModel,
                    invalid: invalid);
            }
            MoveSiteMenu(
                context: context,
                ss: siteModel.SiteSettings,
                sourceId: sourceSiteModel.SiteId,
                destinationId: destinationSiteModel.SiteId);
            return toParent
                ? "[]"
                : new ResponseCollection()
                    .ReplaceAll(
                        "[data-value=\"" + destinationSiteModel.SiteId + "\"]",
                        siteModel.ReplaceSiteMenu(
                            context: context,
                            sourceId: sourceSiteModel.SiteId,
                            destinationId: destinationSiteModel.SiteId))
                    .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string LinkDialog(
            Context context,
            long id,
            SiteModel siteModel,
            SiteModel sourceSiteModel,
            SiteModel destinationSiteModel)
        {
            if (sourceSiteModel.SiteSettings.Links?.Any(o =>
                    o.SiteId == destinationSiteModel.SiteId) == true ||
                destinationSiteModel.SiteSettings.Links?.Any(o =>
                    o.SiteId == sourceSiteModel.SiteId) == true)
            {
                return SiteMenuError(
                    context: context,
                    id: id,
                    siteModel: siteModel,
                    invalid: Error.Types.AlreadyLinked);
            }
            var invalid = SiteValidators.OnLinking(
                context: context,
                sourceInheritSiteId: sourceSiteModel.InheritPermission,
                destinationInheritSiteId: destinationSiteModel.InheritPermission);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return SiteMenuError(
                    context: context,
                    id: id,
                    siteModel: siteModel,
                    invalid: invalid);
            }
            var columns = sourceSiteModel.SiteSettings.Columns
                .Where(o => o.ColumnName.StartsWith("Class"));
            var hb = new HtmlBuilder();
            return new ResponseCollection()
                .Html("#LinkDialog", hb.Div(action: () => hb
                    .FieldSet(
                        css: "fieldset",
                        action: () => hb
                            .FieldText(
                                labelText: Displays.LinkDestinations(context: context),
                                text: destinationSiteModel.Title.Value)
                            .FieldText(
                                labelText: Displays.LinkSources(context: context),
                                text: sourceSiteModel.Title.Value)
                            .FieldDropDown(
                                context: context,
                                controlId: "LinkColumn",
                                labelText: Displays.LinkColumn(context: context),
                                controlCss: " always-send",
                                optionCollection: columns.ToDictionary(o =>
                                    o.ColumnName, o => new ControlData(o.LabelText)),
                                selectedValue: columns.Where(o => !sourceSiteModel
                                    .SiteSettings.EditorColumns.Contains(o.ColumnName))
                                    .FirstOrDefault()?
                                    .ColumnName)
                            .FieldTextBox(
                                controlId: "LinkColumnLabelText",
                                labelText: Displays.DisplayName(context: context),
                                controlCss: " always-send",
                                text: destinationSiteModel.Title.Value,
                                validateRequired: true)
                            .Hidden(
                                controlId: "DestinationId",
                                value: destinationSiteModel.SiteId.ToString())
                            .Hidden(
                                controlId: "SiteId",
                                value: sourceSiteModel.SiteId.ToString())
                            .P(css: "message-dialog")
                            .Div(css: "command-center", action: () => hb
                                .Button(
                                    text: Displays.Create(context: context),
                                    controlCss: "button-icon validate",
                                    onClick: "$p.send($(this));",
                                    icon: "ui-icon-disk",
                                    action: "CreateLink",
                                    method: "post",
                                    confirm: "ConfirmCreateLink")
                                .Button(
                                    text: Displays.Cancel(context: context),
                                    controlCss: "button-icon",
                                    onClick: "$p.closeDialog($(this));",
                                    icon: "ui-icon-cancel")))))
                .ReplaceAll("#SiteMenu", new HtmlBuilder().SiteMenu(
                    context: context,
                    siteModel: id != 0 ? siteModel : null,
                    siteConditions: SiteInfo.TenantCaches
                        .Get(context.TenantId)?
                        .SiteMenu
                        .SiteConditions(context: context, ss: siteModel.SiteSettings)))
                .Invoke("setSiteMenu")
                .Invoke("openLinkDialog").ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void MoveSiteMenu(
            Context context, SiteSettings ss, long sourceId, long destinationId)
        {
            Rds.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.UpdateSites(
                        where: Rds.SitesWhere()
                            .TenantId(context.TenantId)
                            .SiteId(sourceId),
                        param: Rds.SitesParam().ParentId(destinationId)),
                    StatusUtilities.UpdateStatus(
                        tenantId: context.TenantId,
                        type: StatusUtilities.Types.SitesUpdated)
                });
            SiteInfo.Reflesh(context: context);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string CreateLink(Context context, long id)
        {
            var siteModel = new SiteModel(context: context, siteId: id);
            siteModel.SiteSettings = SiteSettingsUtilities.Get(
                context: context, siteModel: siteModel, referenceId: id);
            var sourceSiteModel = new SiteModel(
                context: context, siteId: Forms.Long("SiteId"));
            var destinationSiteModel = new SiteModel(
                context: context, siteId: Forms.Long("DestinationId"));
            if (siteModel.NotFound() ||
                sourceSiteModel.NotFound() ||
                destinationSiteModel.NotFound())
            {
                return SiteMenuError(
                    context: context,
                    id: id,
                    siteModel: siteModel,
                    invalid: Error.Types.NotFound);
            }
            var invalid = SiteValidators.OnLinking(
                context: context,
                sourceInheritSiteId: sourceSiteModel.InheritPermission,
                destinationInheritSiteId: destinationSiteModel.InheritPermission);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return SiteMenuError(
                    context: context,
                    id: id,
                    siteModel: siteModel,
                    invalid: invalid);
            }
            switch (sourceSiteModel.ReferenceType)
            {
                case "Sites":
                case "Wikis":
                    return SiteMenuError(
                        context: context,
                        id: id,
                        siteModel: siteModel,
                        invalid: Error.Types.CanNotPerformed);
            }
            if (sourceSiteModel.SiteSettings.Links?.Any(o =>
                    o.SiteId == destinationSiteModel.SiteId) == true ||
                destinationSiteModel.SiteSettings.Links?.Any(o =>
                    o.SiteId == sourceSiteModel.SiteId) == true)
            {
                return SiteMenuError(
                    context: context,
                    id: id,
                    siteModel: siteModel,
                    invalid: Error.Types.AlreadyLinked);
            }
            var columns = sourceSiteModel.SiteSettings.Columns
                .Where(o => o.ColumnName.StartsWith("Class"));
            if (!columns.Any())
            {
                return SiteMenuError(
                    context: context,
                    id: id,
                    siteModel: siteModel,
                    invalid: Error.Types.CanNotLink);
            }
            var column = sourceSiteModel.SiteSettings.ColumnHash.Get(Forms.Data("LinkColumn"));
            if (column == null)
            {
                return SiteMenuError(
                    context: context,
                    id: id,
                    siteModel: siteModel,
                    invalid: Error.Types.InvalidRequest);
            } 
            var labelText = Forms.Data("LinkColumnLabelText");
            column.LabelText = labelText;
            column.GridLabelText = labelText;
            column.ChoicesText = $"[[{destinationSiteModel.SiteId}]]";
            sourceSiteModel.SiteSettings.SetLinks(context: context, column: column);
            if (!sourceSiteModel.SiteSettings.EditorColumns.Contains(column.ColumnName))
            {
                sourceSiteModel.SiteSettings.EditorColumns.Add(column.ColumnName);
            }
            Rds.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.UpdateSites(
                        param: Rds.SitesParam().SiteSettings(
                            sourceSiteModel.SiteSettings.RecordingJson(context: context)),
                        where: Rds.SitesWhere()
                            .TenantId(context.TenantId)
                            .SiteId(sourceSiteModel.SiteId)),
                    StatusUtilities.UpdateStatus(
                        tenantId: context.TenantId,
                        type: StatusUtilities.Types.SitesUpdated),
                    Rds.PhysicalDeleteLinks(
                        where: Rds.LinksWhere().SourceId(sourceSiteModel.SiteId)),
                    LinkUtilities.Insert(sourceSiteModel.SiteSettings.Links
                        .Select(o => o.SiteId)
                        .Distinct()
                        .ToDictionary(o => o, o => sourceSiteModel.SiteId))
                });
            return new ResponseCollection()
                .CloseDialog()
                .ReplaceAll("#SiteMenu", new HtmlBuilder().SiteMenu(
                    context: context,
                    siteModel: id != 0 ? siteModel : null,
                    siteConditions: SiteInfo.TenantCaches
                        .Get(context.TenantId)?
                        .SiteMenu
                        .SiteConditions(context: context, ss: siteModel.SiteSettings)))
                .Invoke("setSiteMenu")
                .Message(Messages.LinkCreated(context: context))
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string SortSiteMenu(Context context, long siteId)
        {
            var siteModel = new SiteModel(context: context, siteId: siteId);
            var invalid = SiteValidators.OnSorting(
                context: context, ss: SiteSettingsUtilities.Get(
                    context: context, siteModel: siteModel, referenceId: siteId));
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return SiteMenuError(
                    context: context,
                    id: siteId,
                    siteModel: siteModel,
                    invalid: invalid);
            }
            var ownerId = siteModel.SiteId == 0
                ? context.UserId
                : 0;
            SortSiteMenu(
                context: context,
                siteModel: siteModel,
                ownerId: ownerId);
            return new ResponseCollection().ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void SortSiteMenu(Context context, SiteModel siteModel, int ownerId)
        {
            new OrderModel()
            {
                ReferenceId = siteModel.SiteId,
                ReferenceType = "Sites",
                OwnerId = ownerId,
                Data = Forms.LongList("Data")
            }.UpdateOrCreate(context: context);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string SiteMenuError(
            Context context, long id, SiteModel siteModel, Error.Types invalid)
        {
            return new ResponseCollection()
                .ReplaceAll("#SiteMenu", new HtmlBuilder().SiteMenu(
                    context: context,
                    siteModel: id != 0 ? siteModel : null,
                    siteConditions: SiteInfo.TenantCaches
                        .Get(context.TenantId)?
                        .SiteMenu
                        .SiteConditions(context: context, ss: siteModel.SiteSettings)))
                .Invoke("setSiteMenu")
                .Message(invalid.Message(context: context))
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Editor(Context context, long siteId, bool clearSessions)
        {
            var siteModel = new SiteModel(
                context: context,
                siteId: siteId,
                clearSessions: clearSessions,
                methodType: BaseModel.MethodTypes.Edit);
            siteModel.SiteSettings = SiteSettingsUtilities.Get(
                context: context, siteModel: siteModel, referenceId: siteId);
            return Editor(context: context, siteModel: siteModel);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditorTabs(this HtmlBuilder hb, Context context, SiteModel siteModel)
        {
            var ss = siteModel.SiteSettings;
            return hb.Ul(id: "EditorTabs", action: () =>
            {
                hb.Li(action: () => hb
                    .A(
                        href: "#FieldSetGeneral",
                        text: Displays.General(context: context)));
                if (siteModel.MethodType != BaseModel.MethodTypes.New)
                {
                    hb.Li(action: () => hb
                        .A(
                            href: "#SiteImageSettingsEditor",
                            text: Displays.SiteImageSettingsEditor(context: context)));
                    switch (siteModel.ReferenceType)
                    {
                        case "Sites":
                            break;
                        case "Wikis":
                            hb
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#NotificationsSettingsEditor",
                                            text: Displays.Notifications(context: context)),
                                    _using: context.ContractSettings.Notice != false
                                        && NotificationUtilities.Types(context: context).Any())
                                .Li(action: () => hb
                                    .A(
                                        href: "#MailSettingsEditor",
                                        text: Displays.Mail(context: context)))
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#StylesSettingsEditor",
                                            text: Displays.Styles(context: context)),
                                    _using: context.ContractSettings.Style != false)
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#ScriptsSettingsEditor",
                                            text: Displays.Scripts(context: context)),
                                    _using: context.ContractSettings.Script != false);
                            break;
                        default:
                            hb
                                .Li(action: () => hb
                                    .A(
                                        href: "#GridSettingsEditor",
                                        text: Displays.Grid(context: context)))
                                .Li(action: () => hb
                                    .A(
                                        href: "#FiltersSettingsEditor",
                                        text: Displays.Filters(context: context)))
                                .Li(action: () => hb
                                    .A(
                                        href: "#AggregationsSettingsEditor",
                                        text: Displays.Aggregations(context: context)))
                                .Li(action: () => hb
                                    .A(
                                        href: "#EditorSettingsEditor",
                                        text: Displays.Editor(context: context)))
                                .Li(action: () => hb
                                    .A(
                                        href: "#LinksSettingsEditor",
                                        text: Displays.Links(context: context)))
                                .Li(action: () => hb
                                    .A(
                                        href: "#HistoriesSettingsEditor",
                                        text: Displays.Histories(context: context)))
                                .Li(action: () => hb
                                    .A(
                                        href: "#SummariesSettingsEditor",
                                        text: Displays.Summaries(context: context)))
                                .Li(action: () => hb
                                    .A(
                                        href: "#FormulasSettingsEditor",
                                        text: Displays.Formulas(context: context)))
                                .Li(action: () => hb
                                    .A(
                                        href: "#ViewsSettingsEditor",
                                        text: Displays.DataView(context: context)))
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#NotificationsSettingsEditor",
                                            text: Displays.Notifications(context: context)),
                                    _using: context.ContractSettings.Notice != false)
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#RemindersSettingsEditor",
                                            text: Displays.Reminders(context: context)),
                                    _using: context.ContractSettings.Remind != false)
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#ExportsSettingsEditor",
                                            text: Displays.Export(context: context)),
                                    _using: context.ContractSettings.Export != false)
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#CalendarSettingsEditor",
                                            text: Displays.Calendar(context: context)),
                                    _using: Def.ViewModeDefinitionCollection
                                        .Where(o => o.Name == "Calendar")
                                        .Any(o => o.ReferenceType == siteModel.ReferenceType))
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#CrosstabSettingsEditor",
                                            text: Displays.Crosstab(context: context)),
                                    _using: Def.ViewModeDefinitionCollection
                                        .Where(o => o.Name == "Crosstab")
                                        .Any(o => o.ReferenceType == siteModel.ReferenceType))
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#GanttSettingsEditor",
                                            text: Displays.Gantt(context: context)),
                                    _using: Def.ViewModeDefinitionCollection
                                        .Where(o => o.Name == "Gantt")
                                        .Any(o => o.ReferenceType == siteModel.ReferenceType))
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#BurnDownSettingsEditor",
                                            text: Displays.BurnDown(context: context)),
                                    _using: Def.ViewModeDefinitionCollection
                                        .Where(o => o.Name == "BurnDown")
                                        .Any(o => o.ReferenceType == siteModel.ReferenceType))
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#TimeSeriesSettingsEditor",
                                            text: Displays.TimeSeries(context: context)),
                                    _using: Def.ViewModeDefinitionCollection
                                        .Where(o => o.Name == "TimeSeries")
                                        .Any(o => o.ReferenceType == siteModel.ReferenceType))
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#KambanSettingsEditor",
                                            text: Displays.Kamban(context: context)),
                                    _using: Def.ViewModeDefinitionCollection
                                        .Where(o => o.Name == "Kamban")
                                        .Any(o => o.ReferenceType == siteModel.ReferenceType))
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#ImageLibSettingsEditor",
                                            text: Displays.ImageLib(context: context)),
                                    _using:
                                        context.ContractSettings.Images() &&
                                        Def.ViewModeDefinitionCollection
                                            .Where(o => o.Name == "ImageLib")
                                            .Any(o => o.ReferenceType == siteModel.ReferenceType))
                                .Li(action: () => hb
                                    .A(
                                        href: "#SearchSettingsEditor",
                                        text: Displays.Search(context: context)))
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#MailSettingsEditor",
                                            text: Displays.Mail(context: context)),
                                    _using: context.ContractSettings.Notice != false)
                                .Li(action: () => hb
                                    .A(
                                        href: "#SiteIntegrationEditor",
                                        text: Displays.SiteIntegration(context: context)))
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#StylesSettingsEditor",
                                            text: Displays.Styles(context: context)),
                                    _using: context.ContractSettings.Style != false)
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#ScriptsSettingsEditor",
                                            text: Displays.Scripts(context: context)),
                                    _using: context.ContractSettings.Script != false);
                            break;
                    }
                    hb
                        .Li(action: () => hb
                            .A(
                                href: "#FieldSetSiteAccessControl",
                                text: Displays.SiteAccessControl(context: context),
                                _using: context.CanManagePermission(ss: ss)))
                        .Li(
                            action: () => hb
                                .A(
                                    href: "#FieldSetRecordAccessControl",
                                    text: Displays.RecordAccessControl(context: context)),
                            _using: EnableAdvancedPermissions(
                                context: context, siteModel: siteModel))
                        .Li(
                            action: () => hb
                                .A(
                                    href: "#FieldSetColumnAccessControl",
                                    text: Displays.ColumnAccessControl(context: context)),
                            _using: EnableAdvancedPermissions(
                                context: context, siteModel: siteModel))
                        .Li(action: () => hb
                            .A(
                                href: "#FieldSetHistories",
                                text: Displays.ChangeHistoryList(context: context)));
                }
                hb.Hidden(controlId: "TableName", value: "Sites");
            });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string SiteTop(Context context)
        {
            var hb = new HtmlBuilder();
            var ss = new SiteSettings();
            ss.ReferenceType = "Sites";
            ss.PermissionType = SiteTopPermission(context: context);
            var verType = Versions.VerTypes.Latest;
            var siteConditions = SiteInfo.TenantCaches
                .Get(context.TenantId)?
                .SiteMenu
                .SiteConditions(context: context, ss: ss);
            return hb.Template(
                context: context,
                ss: ss,
                verType: verType,
                methodType: BaseModel.MethodTypes.Index,
                referenceType: "Sites",
                script: "$p.setSiteMenu();",
                action: () =>
                {
                    hb
                        .Form(
                            attributes: new HtmlAttributes()
                                .Id("SitesForm")
                                .Class("main-form")
                                .Action(Locations.ItemAction(0)),
                            action: () => hb
                                .SiteMenu(
                                    context: context,
                                    siteModel: null,
                                    siteConditions: siteConditions)
                                .SiteMenuData()
                                .LinkDialog(context: context))
                        .SiteTitleDialog(
                            context: context,
                            ss: ss)
                        .MainCommands(
                            context: context,
                            ss: ss,
                            siteId: 0,
                            verType: verType,
                            backButton: false);
                }).ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string SiteMenu(Context context, SiteModel siteModel)
        {
            var invalid = SiteValidators.OnShowingMenu(
                context: context,
                siteModel: siteModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return HtmlTemplates.Error(context, invalid);
            }
            var hb = new HtmlBuilder();
            var ss = siteModel.SiteSettings;
            var siteConditions = SiteInfo.TenantCaches
                .Get(context.TenantId)?
                .SiteMenu
                .SiteConditions(context: context, ss: ss);
            return hb.Template(
                context: context,
                ss: ss,
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.Index,
                siteId: siteModel.SiteId,
                parentId: siteModel.ParentId,
                referenceType: "Sites",
                script: "$p.setSiteMenu();",
                action: () =>
                {
                    hb
                        .Form(
                            attributes: new HtmlAttributes()
                                .Id("SitesForm")
                                .Class("main-form")
                                .Action(Locations.ItemAction(ss.SiteId)),
                            action: () => hb
                                .SiteMenu(
                                    context: context,
                                    siteModel: siteModel,
                                    siteConditions: siteConditions)
                                .SiteMenuData()
                                .LinkDialog(context: context))
                        .SiteTitleDialog(
                            context: context,
                            ss: siteModel.SiteSettings);
                    if (ss.SiteId != 0)
                    {
                        hb.MainCommands(
                            context: context,
                            ss: ss,
                            siteId: siteModel.SiteId,
                            verType: Versions.VerTypes.Latest);
                    }
                }).ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SiteMenu(
            this HtmlBuilder hb,
            Context context,
            SiteModel siteModel,
            IEnumerable<SiteCondition> siteConditions)
        {
            var ss = siteModel != null
                ? siteModel.SiteSettings
                : SiteSettingsUtilities.SitesSiteSettings(context: context, siteId: 0);
            ss.PermissionType = siteModel != null
                ? siteModel.SiteSettings.PermissionType
                : SiteTopPermission(context: context);
            return hb.Div(id: "SiteMenu", action: () => hb
                .Nav(css: "cf", _using: siteModel != null, action: () => hb
                    .Ul(css: "nav-sites", action: () => hb
                        .ToParent(context: context, siteModel: siteModel)))
                .Nav(css: "cf", action: () => hb
                    .Ul(css: "nav-sites sortable", action: () =>
                        Menu(context: context, ss: ss).ForEach(siteModelChild => hb
                            .SiteMenu(
                                context: context,
                                ss: ss,
                                siteId: siteModelChild.SiteId,
                                referenceType: siteModelChild.ReferenceType,
                                title: siteModelChild.Title.Value,
                                siteConditions: siteConditions))))
                .SiteMenuData());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ToParent(
            this HtmlBuilder hb, Context context, SiteModel siteModel)
        {
            return siteModel.SiteId != 0
                ? hb.SiteMenu(
                    context: context,
                    ss: siteModel.SiteSettings,
                    siteId: siteModel.ParentId,
                    referenceType: "Sites",
                    title: Displays.ToParent(context: context),
                    toParent: true)
                : hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder SiteMenu(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            long siteId,
            string referenceType,
            string title,
            bool toParent = false,
            IEnumerable<SiteCondition> siteConditions = null)
        {
            var hasImage = BinaryUtilities.ExistsSiteImage(
                context: context,
                ss: ss,
                referenceId: siteId,
                sizeType: Libraries.Images.ImageData.SizeTypes.Thumbnail);
            var siteImagePrefix = BinaryUtilities.SiteImagePrefix(
                context: context,
                ss: ss,
                referenceId: siteId,
                sizeType: Libraries.Images.ImageData.SizeTypes.Thumbnail);
            return hb.Li(
                attributes: new HtmlAttributes()
                    .Class(Css.Class("nav-site " + referenceType.ToLower() +
                        (hasImage
                            ? " has-image"
                            : string.Empty),
                         toParent
                            ? " to-parent"
                            : string.Empty))
                    .DataValue(siteId.ToString())
                    .DataType(referenceType),
                action: () => hb
                    .A(
                        attributes: new HtmlAttributes()
                            .Href(SiteHref(
                                context: context,
                                ss: ss,
                                siteId: siteId,
                                referenceType: referenceType)),
                        action: () => hb
                            .SiteMenuInnerElements(
                                context: context,
                                siteId: siteId,
                                referenceType: referenceType,
                                title: title,
                                toParent: toParent,
                                hasImage: hasImage,
                                siteImagePrefix: siteImagePrefix,
                                siteConditions: siteConditions)));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string SiteHref(
            Context context, SiteSettings ss, long siteId, string referenceType)
        {
            switch (referenceType)
            {
                case "Wikis":
                    return Locations.ItemEdit(Rds.ExecuteScalar_long(
                        context: context,
                        statements: Rds.SelectWikis(
                            column: Rds.WikisColumn().WikiId(),
                            where: Rds.WikisWhere().SiteId(siteId))));
                default:
                    var viewMode = ViewModes.GetSessionData(
                        context: context,
                        siteId: siteId);
                    switch (viewMode.ToLower())
                    {
                        case "trashbox":
                            viewMode = "index";
                            break;
                    }
                    return Locations.Get("Items", siteId.ToString(), viewMode);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SiteMenuInnerElements(
            this HtmlBuilder hb,
            Context context,
            long siteId,
            string referenceType,
            string title,
            bool toParent,
            bool hasImage,
            string siteImagePrefix,
            IEnumerable<SiteCondition> siteConditions)
        {
            if (toParent)
            {
                hb.SiteMenuParent(
                    siteId: siteId,
                    title: title,
                    hasImage: hasImage,
                    siteImagePrefix: siteImagePrefix);
            }
            else
            {
                hb.SiteMenuChild(
                    siteId: siteId,
                    title: title,
                    hasImage: hasImage,
                    siteImagePrefix: siteImagePrefix);
            }
            return hb
                .SiteMenuStyle(referenceType: referenceType)
                .SiteMenuConditions(
                    context: context,
                    siteId: siteId,
                    siteConditions: siteConditions);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SiteMenuParent(
            this HtmlBuilder hb,
            long siteId,
            string title,
            bool hasImage,
            string siteImagePrefix)
        {
            if (hasImage)
            {
                return hb
                    .Img(
                        src: Locations.Get(
                            "Items",
                            siteId.ToString(),
                            "Binaries",
                            "SiteImageIcon",
                            siteImagePrefix),
                        css: "site-image-icon")
                    .Span(css: "title", action: () => hb
                        .Text(title));
            }
            else
            {
                return hb.Icon(
                    iconCss: "ui-icon-circle-arrow-n",
                    cssText: "title",
                    text: title);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SiteMenuChild(
            this HtmlBuilder hb,
            long siteId,
            string title,
            bool hasImage,
            string siteImagePrefix)
        {
            if (hasImage)
            {
                hb.Img(
                    src: Locations.Get(
                        "Items",
                        siteId.ToString(),
                        "Binaries",
                        "SiteImageThumbnail",
                        siteImagePrefix),
                    css: "site-image-thumbnail");
            }
            return hb.Span(css: "title", action: () => hb
                .Text(title));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SiteMenuStyle(
            this HtmlBuilder hb,
            string referenceType)
        {
            if (referenceType == "Sites")
            {
                return hb.Div(css: "heading");
            }
            else
            {
                switch (referenceType)
                {
                    case "Wikis": return hb;
                    default: return hb.StackStyles();
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SiteMenuConditions(
            this HtmlBuilder hb,
            Context context,
            long siteId,
            IEnumerable<SiteCondition> siteConditions)
        {
            if (siteConditions != null &&
                siteConditions.Any(o => o.SiteId == siteId))
            {
                var condition = siteConditions
                    .FirstOrDefault(o => o.SiteId == siteId);
                hb.Div(
                    css: "conditions",
                    _using: condition.ItemCount > 0,
                    action: () => hb
                        .ElapsedTime(
                            context: context,
                            value: condition.UpdatedTime.ToLocal(context: context))
                        .Span(
                            attributes: new HtmlAttributes()
                                .Class("count")
                                .Title(Displays.Quantity(context: context)),
                            action: () => hb
                                .Text(condition.ItemCount.ToString()))
                        .Span(
                            attributes: new HtmlAttributes()
                                .Class("overdue")
                                .Title(Displays.Overdue(context: context)),
                            _using: condition.OverdueCount > 0,
                            action: () => hb
                                .Text($"({condition.OverdueCount})")));
            }
            return hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static IEnumerable<SiteModel> Menu(Context context, SiteSettings ss)
        {
            var siteDataRows = new SiteCollection(
                context: context,
                column: Rds.SitesColumn()
                    .SiteId()
                    .Title()
                    .ReferenceType(),
                where: Rds.SitesWhere()
                    .TenantId(context.TenantId)
                    .ParentId(ss.SiteId)
                    .Add(
                        raw: Def.Sql.HasPermission,
                        _using: !context.HasPrivilege));
            var orderModel = new OrderModel(
                context: context,
                ss: ss,
                referenceId: ss.SiteId,
                referenceType: "Sites");
            siteDataRows.ForEach(siteModel =>
            {
                var index = orderModel.Data.IndexOf(siteModel.SiteId);
                siteModel.SiteMenu = (index != -1 ? index : int.MaxValue);
            });
            return siteDataRows.OrderBy(o => o.SiteMenu);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SiteMenuData(this HtmlBuilder hb)
        {
            return hb
                .Hidden(attributes: new HtmlAttributes()
                    .Id("MoveSiteMenu")
                    .DataAction("MoveSiteMenu")
                    .DataMethod("post"))
                .Hidden(attributes: new HtmlAttributes()
                    .Id("SortSiteMenu")
                    .DataAction("SortSiteMenu")
                    .DataMethod("put"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SiteTitleDialog(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return hb.Div(
                attributes: new HtmlAttributes()
                    .Id("SiteTitleDialog")
                    .Class("dialog")
                    .Title(Displays.EnterTitle(context: context)),
                action: () => hb
                    .Form(
                        attributes: new HtmlAttributes()
                            .Id("SiteTitleForm")
                            .Action(Locations.ItemAction(ss.SiteId)),
                        action: () => hb
                            .FieldTextBox(
                                controlId: "SiteTitle",
                                controlCss: " focus always-send",
                                labelText: Displays.Title(context: context),
                                validateRequired: true)
                            .Hidden(
                                controlId: "TemplateId",
                                css: " always-send")
                            .P(css: "message-dialog")
                            .Div(css: "command-center", action: () => hb
                                .Button(
                                    controlId: "CreateByTemplate",
                                    text: Displays.Create(context: context),
                                    controlCss: "button-icon validate",
                                    onClick: "$p.send($(this));",
                                    icon: "ui-icon-gear",
                                    action: "CreateByTemplate",
                                    method: "post")
                                .Button(
                                    text: Displays.Cancel(context: context),
                                    controlCss: "button-icon",
                                    onClick: "$p.closeDialog($(this));",
                                    icon: "ui-icon-cancel"))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string PreviewTemplate(Context context)
        {
            var controlId = Forms.ControlId();
            var template = Def.TemplateDefinitionCollection
                .FirstOrDefault(o => o.Id == Forms.List(controlId).FirstOrDefault());
            return template != null
                ? PreviewTemplate(context: context, template: template, controlId: controlId)
                : new ResponseCollection()
                    .Html(
                        "#" + controlId + "Viewer .description",
                        Displays.SelectTemplate(context: context))
                    .Toggle("#" + controlId + "Viewer .viewer", false)
                    .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string PreviewTemplate(
            Context context, TemplateDefinition template, string controlId)
        {
            var hb = new HtmlBuilder();
            var ss = template.SiteSettingsTemplate.Deserialize<SiteSettings>();
            ss.Init(context: context);
            ss.SetChoiceHash(context: context, withLink: false);
            var html = string.Empty;
            switch (ss.ReferenceType)
            {
                case "Sites":
                    html = PreviewTemplate(
                        context: context,
                        ss: ss,
                        title: template.Title).ToString();
                    break;
                case "Issues":
                    html = IssueUtilities.PreviewTemplate(
                        context: context, ss: ss).ToString();
                    break;
                case "Results":
                    html = ResultUtilities.PreviewTemplate(
                        context: context, ss: ss).ToString();
                    break;
                case "Wikis":
                    html = WikiUtilities.PreviewTemplate(
                        context: context,
                        ss: ss,
                        body: template.Body).ToString();
                    break;
            }
            return new ResponseCollection()
                .Html(
                    "#" + controlId + "Viewer .description",
                    hb.Text(text: Strings.CoalesceEmpty(
                        template.Description, template.Title)))
                .Html("#" + controlId + "Viewer .viewer", html)
                .Invoke("setTemplateViewer")
                .Toggle("#" + controlId + "Viewer .viewer", true)
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string PreviewTemplate(Context context, SiteSettings ss, string title)
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
                                text: Displays.Menu(context: context))))
                    .FieldSet(
                        id: name + "Editor",
                        action: () => hb
                            .Div(css: "nav-site sites", action: () => hb
                                .Span(css: "title", action: () => hb.Text(title))
                                .Div(css: "heading"))))
                                    .ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Editor(Context context, SiteModel siteModel)
        {
            var invalid = SiteValidators.OnEditing(
                context: context, ss: siteModel.SiteSettings, siteModel: siteModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return HtmlTemplates.Error(context, invalid);
            }
            var hb = new HtmlBuilder();
            return hb.Template(
                context: context,
                ss: siteModel.SiteSettings,
                verType: siteModel.VerType,
                methodType: siteModel.MethodType,
                siteId: siteModel.SiteId,
                parentId: siteModel.ParentId,
                referenceType: "Sites",
                siteReferenceType: siteModel.ReferenceType,
                title: siteModel.MethodType == BaseModel.MethodTypes.New
                    ? Displays.Sites(context: context) + " - " + Displays.New(context: context)
                    : siteModel.Title + " - " + Displays.Manage(context: context),
                action: () => hb
                    .Editor(context: context, siteModel: siteModel)
                    .Hidden(controlId: "BaseUrl", value: Locations.BaseUrl())
                    .Hidden(controlId: "ReferenceType", value: "Sites")
                    .Hidden(
                        controlId: "SwitchTargets",
                        css: "always-send",
                        value: siteModel.SiteId.ToString(),
                        _using: !Request.IsAjax())).ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Editor(
            this HtmlBuilder hb, Context context, SiteModel siteModel)
        {
            var ss = siteModel.SiteSettings;
            var commentsColumn = ss.GetColumn(
                context: context,
                columnName: "Comments");
            var commentsColumnPermissionType = commentsColumn
                .ColumnPermissionType(context: context);
            var showComments = ss.EditorColumns?.Contains("Comments") == true &&
                commentsColumnPermissionType != Permissions.ColumnPermissionTypes.Deny;
            var tabsCss = showComments ? null : "max";
            return hb.Div(id: "Editor", action: () => hb
                .Form(
                    attributes: new HtmlAttributes()
                        .Id("SiteForm")
                        .Class("main-form confirm-reload")
                        .Action(Locations.ItemAction(siteModel.SiteId)),
                    action: () => hb
                        .RecordHeader(
                            context: context,
                            ss: ss,
                            baseModel: siteModel,
                            tableName: "Sites",
                            switcher: false)
                        .Div(
                            id: "EditorComments", action: () => hb
                                .Comments(
                                    context: context,
                                    ss: ss,
                                    comments: siteModel.Comments,
                                    column: commentsColumn,
                                    verType: siteModel.VerType,
                                    columnPermissionType: commentsColumnPermissionType),
                            _using: showComments)
                        .Div(id: "EditorTabsContainer", css: tabsCss, action: () => hb
                            .EditorTabs(context: context, siteModel: siteModel)
                            .FieldSetGeneral(context: context, siteModel: siteModel)
                            .FieldSet(
                                attributes: new HtmlAttributes()
                                    .Id("FieldSetHistories")
                                    .DataAction("Histories")
                                    .DataMethod("post"),
                                _using: siteModel.MethodType != BaseModel.MethodTypes.New)
                            .FieldSet(
                                attributes: new HtmlAttributes()
                                    .Id("FieldSetSiteAccessControl")
                                    .DataAction("Permissions")
                                    .DataMethod("post"),
                                _using: context.CanManagePermission(ss: ss))
                            .FieldSet(
                                attributes: new HtmlAttributes()
                                    .Id("FieldSetRecordAccessControl")
                                    .DataAction("PermissionForCreating")
                                    .DataMethod("post"),
                                _using: EnableAdvancedPermissions(
                                    context: context, siteModel: siteModel))
                            .FieldSet(
                                attributes: new HtmlAttributes()
                                    .Id("FieldSetColumnAccessControl")
                                    .DataAction("ColumnAccessControl")
                                    .DataMethod("post"),
                                _using: EnableAdvancedPermissions(
                                    context: context, siteModel: siteModel))
                            .MainCommands(
                                context: context,
                                ss: siteModel.SiteSettings,
                                siteId: siteModel.SiteId,
                                verType: siteModel.VerType,
                                referenceId: siteModel.SiteId,
                                updateButton: true,
                                copyButton: true,
                                mailButton: true,
                                deleteButton: true))
                        .Hidden(
                            controlId: "MethodType",
                            value: siteModel.MethodType.ToString().ToLower())
                        .Hidden(
                            controlId: "Sites_Timestamp",
                            css: "control-hidden always-send",
                            value: siteModel.Timestamp)
                        .Hidden(controlId: "Id", value: siteModel.SiteId.ToString()))
                .OutgoingMailsForm(
                    context: context,
                    referenceType: "items",
                    referenceId: siteModel.SiteId,
                    referenceVer: siteModel.Ver)
                .CopyDialog(
                    context: context,
                    referenceType: "items",
                    id: siteModel.SiteId)
                .OutgoingMailDialog()
                .DeleteSiteDialog(context: context)
                .Div(attributes: new HtmlAttributes()
                    .Id("GridColumnDialog")
                    .Class("dialog")
                    .Title(Displays.AdvancedSetting(context: context)))
                .Div(attributes: new HtmlAttributes()
                    .Id("FilterColumnDialog")
                    .Class("dialog")
                    .Title(Displays.AdvancedSetting(context: context)))
                .Div(attributes: new HtmlAttributes()
                    .Id("EditorColumnDialog")
                    .Class("dialog")
                    .Title(Displays.AdvancedSetting(context: context)))
                .Div(attributes: new HtmlAttributes()
                    .Id("SummaryDialog")
                    .Class("dialog")
                    .Title(Displays.AdvancedSetting(context: context)))
                .Div(attributes: new HtmlAttributes()
                    .Id("FormulaDialog")
                    .Class("dialog")
                    .Title(Displays.AdvancedSetting(context: context)))
                .Div(attributes: new HtmlAttributes()
                    .Id("ViewDialog")
                    .Class("dialog")
                    .Title(Displays.DataView(context: context)))
                .Div(
                    attributes: new HtmlAttributes()
                        .Id("NotificationDialog")
                        .Class("dialog")
                        .Title(Displays.Notifications(context: context)),
                    _using: context.ContractSettings.Notice != false)
                .Div(
                    attributes: new HtmlAttributes()
                        .Id("ReminderDialog")
                        .Class("dialog")
                        .Title(Displays.Reminders(context: context)),
                    _using: context.ContractSettings.Remind != false)
                .Div(
                    attributes: new HtmlAttributes()
                        .Id("ExportDialog")
                        .Class("dialog")
                        .Title(Displays.Export(context: context)),
                    _using: context.ContractSettings.Export != false)
                .Div(
                    attributes: new HtmlAttributes()
                        .Id("ExportColumnsDialog")
                        .Class("dialog")
                        .Title(Displays.AdvancedSetting(context: context)),
                    _using: context.ContractSettings.Export != false)
                .Div(
                    attributes: new HtmlAttributes()
                        .Id("StyleDialog")
                        .Class("dialog")
                        .Title(Displays.Style(context: context)),
                    _using: context.ContractSettings.Style != false)
                .Div(
                    attributes: new HtmlAttributes()
                        .Id("ScriptDialog")
                        .Class("dialog")
                        .Title(Displays.Script(context: context)),
                    _using: context.ContractSettings.Script != false)
                .Div(
                    attributes: new HtmlAttributes()
                        .Id("RelatingColumnDialog")
                        .Class("dialog")
                        .Title(Displays.RelatingColumn(context: context)))
                .PermissionsDialog(context: context)
                .PermissionForCreatingDialog(context: context)
                .ColumnAccessControlDialog(context: context));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static bool EnableAdvancedPermissions(Context context, SiteModel siteModel)
        {
            switch (siteModel.ReferenceType)
            {
                case "Issues":
                case "Results":
                    return context.CanManagePermission(ss: siteModel.SiteSettings);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string EditorBackUrl(Context context, SiteModel siteModel)
        {
            switch (siteModel.ReferenceType)
            {
                case "Wikis":
                    var wikiId = Rds.ExecuteScalar_long(
                        context: context,
                        statements: Rds.SelectWikis(
                            top: 1,
                            column: Rds.WikisColumn().WikiId(),
                            where: Rds.WikisWhere().SiteId(siteModel.SiteId)));
                    return wikiId != 0
                        ? Locations.ItemEdit(wikiId)
                        : Locations.ItemIndex(siteModel.ParentId);
                default:
                    return Locations.ItemIndex(siteModel.SiteId);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder FieldSetGeneral(
            this HtmlBuilder hb, Context context, SiteModel siteModel)
        {
            var ss = siteModel.SiteSettings;
            var titleColumn = siteModel.SiteSettings.GetColumn(
                context: context,
                columnName: "Title");
            hb.FieldSet(id: "FieldSetGeneral", action: () =>
            {
                hb
                    .FieldText(
                        controlId: "Sites_SiteId",
                        labelText: Displays.Sites_SiteId(context: context),
                        text: siteModel.SiteId.ToString())
                    .FieldText(
                        controlId: "Sites_Ver",
                        controlCss: siteModel.SiteSettings?.GetColumn(
                            context: context,
                            columnName: "Ver").ControlCss,
                        labelText: Displays.Sites_Ver(context: context),
                        text: siteModel.Ver.ToString())
                    .FieldTextBox(
                        controlId: "Sites_Title",
                        fieldCss: "field-wide",
                        controlCss: " focus",
                        labelText: Displays.Sites_Title(context: context),
                        text: siteModel.Title.Value.ToString(),
                        validateRequired: titleColumn.ValidateRequired ?? false,
                        validateMaxLength: titleColumn.ValidateMaxLength ?? 0,
                        _using: siteModel.ReferenceType != "Wikis")
                    .FieldMarkDown(
                        context: context,
                        ss: ss,
                        controlId: "Sites_Body",
                        fieldCss: "field-wide",
                        labelText: Displays.Sites_Body(context: context),
                        text: siteModel.Body,
                        mobile: siteModel.SiteSettings.Mobile,
                        _using: siteModel.ReferenceType != "Wikis")
                    .Field(
                        controlId: "Sites_ReferenceType",
                        labelText: Displays.Sites_ReferenceType(context: context),
                        controlAction: () => hb
                            .ReferenceType(
                                context: context,
                                referenceType: siteModel.ReferenceType,
                                methodType: siteModel.MethodType))
                    .VerUpCheckBox(
                        context: context,
                        ss: ss,
                        baseModel: siteModel);
            });
            if (siteModel.MethodType != BaseModel.MethodTypes.New)
            {
                hb.SiteImageSettingsEditor(
                    context: context,
                    ss: siteModel.SiteSettings);
                switch (siteModel.ReferenceType)
                {
                    case "Sites":
                        break;
                    case "Wikis":
                        hb
                            .NotificationsSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .MailSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .StylesSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .ScriptsSettingsEditor(context: context, ss: siteModel.SiteSettings);
                        break;
                    default:
                        hb
                            .GridSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .FiltersSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .AggregationsSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .EditorSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .LinksSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .HistoriesSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .SummariesSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .FormulasSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .ViewsSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .NotificationsSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .RemindersSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .ExportsSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .CalendarSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .CrosstabSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .GanttSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .BurnDownSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .TimeSeriesSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .KambanSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .ImageLibSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .SearchSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .MailSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .SiteIntegrationEditor(context: context, ss: siteModel.SiteSettings)
                            .StylesSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .ScriptsSettingsEditor(context: context, ss: siteModel.SiteSettings);
                        break;
                }
            }
            return hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder SiteImageSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return hb.FieldSet(id: "SiteImageSettingsEditor", action: () => hb
                .FieldSet(
                    css: " enclosed",
                    legendText: Displays.Icon(context: context),
                    action: () => hb
                        .FieldTextBox(
                            textType: HtmlTypes.TextTypes.File,
                            controlId: "SiteImage",
                            fieldCss: "field-auto-thin",
                            controlCss: " w400",
                            labelText: Displays.File(context: context))
                        .Button(
                            controlId: "SetSiteImage",
                            controlCss: "button-icon",
                            text: Displays.Upload(context: context),
                            onClick: "$p.uploadSiteImage($(this));",
                            icon: "ui-icon-disk",
                            action: "binaries/updatesiteimage",
                            method: "post")
                        .Button(
                            controlCss: "button-icon",
                            text: Displays.Delete(context: context),
                            onClick: "$p.send($(this));",
                            icon: "ui-icon-trash",
                            action: "binaries/deletesiteimage",
                            method: "delete",
                            confirm: "ConfirmDelete",
                            _using: BinaryUtilities.ExistsSiteImage(
                                context: context,
                                ss: ss,
                                referenceId: ss.SiteId,
                                sizeType: Libraries.Images.ImageData.SizeTypes.Thumbnail))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder GridSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return hb.FieldSet(id: "GridSettingsEditor", action: () => hb
                .GridColumns(context: context, ss: ss)
                .FieldSpinner(
                    controlId: "GridPageSize",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.NumberPerPage(context: context),
                    value: ss.GridPageSize.ToDecimal(),
                    min: Parameters.General.GridPageSizeMin,
                    max: Parameters.General.GridPageSizeMax,
                    step: 1,
                    width: 25)
                .FieldDropDown(
                    context: context,
                    controlId: "GridView",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.DefaultView(context: context),
                    optionCollection: ss.ViewSelectableOptions(),
                    selectedValue: ss.GridView?.ToString(),
                    insertBlank: true,
                    _using: ss.Views?.Any() == true)
                .FieldCheckBox(
                    controlId: "EditInDialog",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.EditInDialog(context: context),
                    _checked: ss.EditInDialog == true)
                .AggregationDetailsDialog(context: context, ss: ss));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder GridColumns(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return hb.FieldSet(
                css: " enclosed-thin",
                legendText: Displays.ListSettings(context: context),
                action: () => hb
                    .FieldSelectable(
                        controlId: "GridColumns",
                        fieldCss: "field-vertical",
                        controlContainerCss: "container-selectable",
                        controlWrapperCss: " h350",
                        controlCss: " always-send send-all",
                        labelText: Displays.CurrentSettings(context: context),
                        listItemCollection: ss.GridSelectableOptions(context: context),
                        selectedValueCollection: new List<string>(),
                        commandOptionPositionIsTop: true,
                        commandOptionAction: () => hb
                            .Div(css: "command-center", action: () => hb
                                .Button(
                                    controlId: "MoveUpGridColumns",
                                    controlCss: "button-icon",
                                    text: Displays.MoveUp(context: context),
                                    onClick: "$p.moveColumns($(this),'Grid',false,true);",
                                    icon: "ui-icon-circle-triangle-n")
                                .Button(
                                    controlId: "MoveDownGridColumns",
                                    controlCss: "button-icon",
                                    text: Displays.MoveDown(context: context),
                                    onClick: "$p.moveColumns($(this),'Grid',false,true);",
                                    icon: "ui-icon-circle-triangle-s")
                                .Button(
                                    controlId: "OpenGridColumnDialog",
                                    text: Displays.AdvancedSetting(context: context),
                                    controlCss: "button-icon",
                                    onClick: "$p.openGridColumnDialog($(this));",
                                    icon: "ui-icon-gear",
                                    action: "SetSiteSettings",
                                    method: "put")
                                .Button(
                                    controlId: "ToDisableGridColumns",
                                    controlCss: "button-icon",
                                    text: Displays.ToDisable(context: context),
                                    onClick: "$p.moveColumns($(this),'Grid',false,true);",
                                    icon: "ui-icon-circle-triangle-e")))
                    .FieldSelectable(
                        controlId: "GridSourceColumns",
                        fieldCss: "field-vertical",
                        controlContainerCss: "container-selectable",
                        controlWrapperCss: " h350",
                        labelText: Displays.OptionList(context: context),
                        listItemCollection: ss.GridSelectableOptions(
                            context: context, enabled: false),
                        commandOptionPositionIsTop: true,
                        commandOptionAction: () => hb
                            .Div(css: "command-left", action: () => hb
                                .Button(
                                    controlId: "ToEnableGridColumns",
                                    text: Displays.ToEnable(context: context),
                                    controlCss: "button-icon",
                                    onClick: "$p.moveColumns($(this),'Grid',false,true);",
                                    icon: "ui-icon-circle-triangle-w")
                                .FieldDropDown(
                                    context: context,
                                    controlId: "GridJoin",
                                    fieldCss: "w150",
                                    controlCss: " auto-postback always-send",
                                    optionCollection: ss.JoinOptionHash,
                                    addSelectedValue: false,
                                    action: "SetSiteSettings",
                                    method: "post"))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder GridColumnDialog(Context context, SiteSettings ss, Column column)
        {
            var hb = new HtmlBuilder();
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("GridColumnForm")
                    .Action(Locations.ItemAction(ss.SiteId)),
                action: () => hb
                    .GridColumnDialog(
                        context: context,
                        ss: ss,
                        column: column));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder GridColumnDialog(
            this HtmlBuilder hb, Context context, SiteSettings ss, Column column)
        {
            hb.FieldSet(
                css: " enclosed",
                legendText: column.LabelTextDefault,
                action: () =>
                {
                    hb.FieldTextBox(
                        controlId: "GridLabelText",
                        labelText: Displays.DisplayName(context: context),
                        text: column.GridLabelText,
                        validateRequired: true);
                    if (column.TypeName == "datetime")
                    {
                        hb
                            .FieldDropDown(
                                context: context,
                                controlId: "GridFormat",
                                labelText: Displays.GridFormat(context: context),
                                optionCollection: DateTimeOptions(context: context),
                                selectedValue: column.GridFormat);
                    }
                    hb
                        .FieldCheckBox(
                            controlId: "UseGridDesign",
                            labelText: Displays.UseCustomDesign(context: context),
                            _checked: !column.GridDesign.IsNullOrEmpty())
                        .FieldMarkDown(
                            context: context,
                            ss: ss,
                            fieldId: "GridDesignField",
                            controlId: "GridDesign",
                            fieldCss: "field-wide" + (!column.GridDesign.IsNullOrEmpty()
                                ? string.Empty
                                : " hidden"),
                            labelText: Displays.CustomDesign(context: context),
                            placeholder: Displays.CustomDesign(context: context),
                            text: ss.GridDesignEditorText(column),
                            allowImage: column.AllowImage == true,
                            mobile: ss.Mobile);
                });
            return hb
                .Hidden(
                    controlId: "GridColumnName",
                    css: "always-send",
                    value: column.ColumnName)
                .P(css: "message-dialog")
                .Div(css: "command-center", action: () => hb
                    .Button(
                        controlId: "SetGridColumn",
                        text: Displays.Change(context: context),
                        controlCss: "button-icon validate",
                        onClick: "$p.setGridColumn($(this));",
                        icon: "ui-icon-gear",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        text: Displays.Cancel(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.closeDialog($(this));",
                        icon: "ui-icon-cancel"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder FiltersSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return hb.FieldSet(id: "FiltersSettingsEditor", action: () => hb
                .FieldSet(
                    css: " enclosed-thin",
                    legendText: Displays.FilterSettings(context: context),
                    action: () => hb
                        .FieldSelectable(
                            controlId: "FilterColumns",
                            fieldCss: "field-vertical",
                            controlContainerCss: "container-selectable",
                            controlWrapperCss: " h350",
                            controlCss: " always-send send-all",
                            labelText: Displays.CurrentSettings(context: context),
                            listItemCollection: ss.FilterSelectableOptions(context: context),
                            selectedValueCollection: new List<string>(),
                            commandOptionPositionIsTop: true,
                            commandOptionAction: () => hb
                                .Div(css: "command-center", action: () => hb
                                    .Button(
                                        controlId: "MoveUpFilterColumns",
                                        controlCss: "button-icon",
                                        text: Displays.MoveUp(context: context),
                                        onClick: "$p.moveColumns($(this),'Filter',false,true);",
                                        icon: "ui-icon-circle-triangle-n")
                                    .Button(
                                        controlId: "MoveDownFilterColumns",
                                        controlCss: "button-icon",
                                        text: Displays.MoveDown(context: context),
                                        onClick: "$p.moveColumns($(this),'Filter',false,true);",
                                        icon: "ui-icon-circle-triangle-s")
                                    .Button(
                                        controlId: "OpenFilterColumnDialog",
                                        text: Displays.AdvancedSetting(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.openFilterColumnDialog($(this));",
                                        icon: "ui-icon-gear",
                                        action: "SetSiteSettings",
                                        method: "put")
                                    .Button(
                                        controlId: "ToDisableFilterColumns",
                                        controlCss: "button-icon",
                                        text: Displays.ToDisable(context: context),
                                        onClick: "$p.moveColumns($(this),'Filter',false,true);",
                                        icon: "ui-icon-circle-triangle-e")))
                        .FieldSelectable(
                            controlId: "FilterSourceColumns",
                            fieldCss: "field-vertical",
                            controlContainerCss: "container-selectable",
                            controlWrapperCss: " h350",
                            labelText: Displays.OptionList(context: context),
                            listItemCollection: ss.FilterSelectableOptions(
                                context: context, enabled: false),
                            commandOptionPositionIsTop: true,
                            commandOptionAction: () => hb
                                .Div(css: "command-left", action: () => hb
                                    .Button(
                                        controlId: "ToEnableFilterColumns",
                                        text: Displays.ToEnable(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.moveColumns($(this),'Filter',false,true);",
                                        icon: "ui-icon-circle-triangle-w")
                                    .FieldDropDown(
                                        context: context,
                                        controlId: "FilterJoin",
                                        fieldCss: "w150",
                                        controlCss: " auto-postback always-send",
                                        optionCollection: ss.JoinOptionHash,
                                        addSelectedValue: false,
                                        action: "SetSiteSettings",
                                        method: "post"))))
                .FieldSpinner(
                    controlId: "NearCompletionTimeAfterDays",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.NearCompletionTimeAfterDays(context: context),
                    value: ss.NearCompletionTimeAfterDays.ToDecimal(),
                    min: Parameters.General.NearCompletionTimeAfterDaysMin,
                    max: Parameters.General.NearCompletionTimeAfterDaysMax,
                    step: 1,
                    width: 25)
                .FieldSpinner(
                    controlId: "NearCompletionTimeBeforeDays",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.NearCompletionTimeBeforeDays(context: context),
                    value: ss.NearCompletionTimeBeforeDays.ToDecimal(),
                    min: Parameters.General.NearCompletionTimeBeforeDaysMin,
                    max: Parameters.General.NearCompletionTimeBeforeDaysMax,
                    step: 1,
                    width: 25));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder FilterColumnDialog(
            Context context, SiteSettings ss, Column column)
        {
            var hb = new HtmlBuilder();
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("FilterColumnForm")
                    .Action(Locations.ItemAction(ss.SiteId)),
                action: () => hb
                    .FilterColumnDialog(
                        context: context,
                        ss: ss,
                        column: column));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder FilterColumnDialog(
            this HtmlBuilder hb, Context context, SiteSettings ss, Column column)
        {
            hb.FieldSet(
                css: " enclosed",
                legendText: column.LabelText,
                action: () =>
                {
                    switch (column.TypeName.CsTypeSummary())
                    {
                        case Types.CsBool:
                            hb.FieldDropDown(
                                context: context,
                                controlId: "CheckFilterControlType",
                                fieldCss: "field-auto-thin",
                                labelText: Displays.ControlType(context: context),
                                optionCollection: ColumnUtilities
                                    .CheckFilterControlTypeOptions(context: context),
                                selectedValue: column.CheckFilterControlType.ToInt().ToString());
                            break;
                        case Types.CsNumeric:
                            hb
                                .FieldTextBox(
                                    controlId: "NumFilterMin",
                                    fieldCss: "field-auto-thin",
                                    labelText: Displays.Min(context: context),
                                    text: column.NumFilterMin.TrimEndZero(),
                                    validateRequired: true,
                                    validateNumber: true)
                                .FieldTextBox(
                                    controlId: "NumFilterMax",
                                    fieldCss: "field-auto-thin",
                                    labelText: Displays.Max(context: context),
                                    text: column.NumFilterMax.TrimEndZero(),
                                    validateRequired: true,
                                    validateNumber: true)
                                .FieldTextBox(
                                    controlId: "NumFilterStep",
                                    fieldCss: "field-auto-thin",
                                    labelText: Displays.Step(context: context),
                                    text: column.NumFilterStep.TrimEndZero(),
                                    validateRequired: true,
                                    validateNumber: true);
                            break;
                        case Types.CsDateTime:
                            hb
                                .FieldTextBox(
                                    controlId: "DateFilterMinSpan",
                                    fieldCss: "field-auto-thin",
                                    labelText: Displays.Min(context: context),
                                    text: column.DateFilterMinSpan.ToString(),
                                    validateRequired: true,
                                    validateNumber: true)
                                .FieldTextBox(
                                    controlId: "DateFilterMaxSpan",
                                    fieldCss: "field-auto-thin",
                                    labelText: Displays.Max(context: context),
                                    text: column.DateFilterMaxSpan.ToString(),
                                    validateRequired: true,
                                    validateNumber: true)
                                .FieldCheckBox(
                                    controlId: "DateFilterFy",
                                    fieldCss: "field-auto-thin",
                                    labelText: Displays.UseFy(context: context),
                                    _checked: column.DateFilterFy == true)
                                .FieldCheckBox(
                                    controlId: "DateFilterHalf",
                                    fieldCss: "field-auto-thin",
                                    labelText: Displays.UseHalf(context: context),
                                    _checked: column.DateFilterHalf == true)
                                .FieldCheckBox(
                                    controlId: "DateFilterQuarter",
                                    fieldCss: "field-auto-thin",
                                    labelText: Displays.UseQuarter(context: context),
                                    _checked: column.DateFilterQuarter == true)
                                .FieldCheckBox(
                                    controlId: "DateFilterMonth",
                                    fieldCss: "field-auto-thin",
                                    labelText: Displays.UseMonth(context: context),
                                    _checked: column.DateFilterMonth == true);
                            break;
                    }
                });
            return hb
                .Hidden(
                    controlId: "FilterColumnName",
                    css: "always-send",
                    value: column.ColumnName)
                .P(css: "message-dialog")
                .Div(css: "command-center", action: () => hb
                    .Button(
                        controlId: "SetFilterColumn",
                        text: Displays.Change(context: context),
                        controlCss: "button-icon validate",
                        onClick: "$p.send($(this));",
                        icon: "ui-icon-gear",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        text: Displays.Cancel(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.closeDialog($(this));",
                        icon: "ui-icon-cancel"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder AggregationsSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return hb.FieldSet(id: "AggregationsSettingsEditor", action: () => hb
                .FieldSet(
                    css: " enclosed-thin",
                    legendText: Displays.AggregationSettings(context: context),
                    action: () => hb
                        .FieldSelectable(
                            controlId: "AggregationDestination",
                            fieldCss: "field-vertical both",
                            controlContainerCss: "container-selectable",
                            controlWrapperCss: " h350",
                            controlCss: " always-send send-all",
                            labelText: Displays.CurrentSettings(context: context),
                            listItemCollection: ss.AggregationDestination(context: context),
                            selectedValueCollection: new List<string>(),
                            commandOptionPositionIsTop: true,
                            commandOptionAction: () => hb
                                .Div(css: "command-center", action: () => hb
                                    .Button(
                                        controlId: "MoveUpAggregations",
                                        controlCss: "button-icon",
                                        text: Displays.MoveUp(context: context),
                                        onClick: "$p.moveColumnsById($(this),'AggregationDestination','',false,true);",
                                        icon: "ui-icon-circle-triangle-n")
                                    .Button(
                                        controlId: "MoveDownAggregations",
                                        controlCss: "button-icon",
                                        text: Displays.MoveDown(context: context),
                                        onClick: "$p.moveColumnsById($(this),'AggregationDestination','',false,true);",
                                        icon: "ui-icon-circle-triangle-n")
                                    .Button(
                                        text: Displays.AdvancedSetting(context: context),
                                        controlCss: "button-icon open-dialog",
                                        onClick: "$p.openDialog($(this), '.main-form');",
                                        icon: "ui-icon-gear",
                                        selector: "#AggregationDetailsDialog")
                                    .Button(
                                        controlId: "DeleteAggregations",
                                        controlCss: "button-icon",
                                        text: Displays.Delete(context: context),
                                        onClick: "$p.send($(this));",
                                        icon: "ui-icon-circle-triangle-e",
                                        action: "SetSiteSettings",
                                        method: "put")))
                        .FieldSelectable(
                            controlId: "AggregationSource",
                            fieldCss: "field-vertical",
                            controlContainerCss: "container-selectable",
                            controlWrapperCss: " h350",
                            labelText: Displays.OptionList(context: context),
                            listItemCollection: ss.AggregationSource(context: context),
                            selectedValueCollection: new List<string>(),
                            commandOptionPositionIsTop: true,
                            commandOptionAction: () => hb
                                .Div(css: "command-center", action: () => hb
                                    .Button(
                                        controlId: "AddAggregations",
                                        controlCss: "button-icon",
                                        text: Displays.Add(context: context),
                                        onClick: "$p.send($(this));",
                                        icon: "ui-icon-circle-triangle-w",
                                        action: "SetSiteSettings",
                                        method: "post")))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder AggregationDetailsDialog(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return hb.Div(
                attributes: new HtmlAttributes()
                    .Id("AggregationDetailsDialog")
                    .Class("dialog")
                    .Title(Displays.AggregationDetails(context: context)),
                action: () => hb
                    .FieldDropDown(
                        context: context,
                        controlId: "AggregationType",
                        labelText: Displays.AggregationType(context: context),
                        optionCollection: new Dictionary<string, string>
                        {
                            { "Count", Displays.Count(context: context) },
                            { "Total", Displays.Total(context: context) },
                            { "Average", Displays.Average(context: context) }
                        })
                    .FieldDropDown(
                        context: context,
                        controlId: "AggregationTarget",
                        fieldCss: " hidden togglable",
                        labelText: Displays.AggregationTarget(context: context),
                        optionCollection: Def.ColumnDefinitionCollection
                            .Where(o => o.TableName == ss.ReferenceType)
                            .Where(o => o.Computable)
                            .Where(o => o.TypeName != "datetime")
                            .ToDictionary(
                                o => o.ColumnName,
                                o => ss.GetColumn(
                                    context: context,
                                    columnName: o.ColumnName).LabelText))
                    .P(css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "SetAggregationDetails",
                            text: Displays.Change(context: context),
                            controlCss: "button-icon",
                            onClick: "$p.setAggregationDetails($(this));",
                            icon: "ui-icon-gear",
                            action: "SetSiteSettings",
                            method: "post")
                        .Button(
                            text: Displays.Cancel(context: context),
                            controlCss: "button-icon",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditorSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return hb.FieldSet(id: "EditorSettingsEditor", action: () => hb
                .FieldSet(
                    css: " enclosed",
                    legendText: Displays.EditorSettings(context: context),
                    action: () => hb
                        .FieldSelectable(
                            controlId: "EditorColumns",
                            fieldCss: "field-vertical",
                            controlContainerCss: "container-selectable",
                            controlWrapperCss: " h250",
                            controlCss: " always-send send-all",
                            labelText: Displays.CurrentSettings(context: context),
                            listItemCollection: ss.EditorSelectableOptions(context: context),
                            commandOptionPositionIsTop: true,
                            commandOptionAction: () => hb
                                .Div(css: "command-center", action: () => hb
                                    .Hidden(controlId: "EditorColumnsNessesaryMessage",
                                        value: Messages.CanNotDisabled(
                                            context: context,
                                            data: "COLUMNNAME").Text)
                                    .Hidden(
                                        controlId: "EditorColumnsNessesaryColumns",
                                        value: Jsons.ToJson(
                                            ss.EditorColumns?
                                            .Where(o => ss.EditorColumn(o).Required)
                                            .Select(o => o)))
                                    .Button(
                                        controlId: "MoveUpEditorColumns",
                                        text: Displays.MoveUp(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.moveColumns($(this),'Editor');",
                                        icon: "ui-icon-circle-triangle-n")
                                    .Button(
                                        controlId: "MoveDownEditorColumns",
                                        text: Displays.MoveDown(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.moveColumns($(this),'Editor');",
                                        icon: "ui-icon-circle-triangle-s")
                                    .Button(
                                        controlId: "OpenEditorColumnDialog",
                                        text: Displays.AdvancedSetting(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.openEditorColumnDialog($(this));",
                                        icon: "ui-icon-gear",
                                        action: "SetSiteSettings",
                                        method: "put")
                                    .Button(
                                        controlId: "ToDisableEditorColumns",
                                        text: Displays.ToDisable(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.moveColumns($(this),'Editor');",
                                        icon: "ui-icon-circle-triangle-e")))
                        .FieldSelectable(
                            controlId: "EditorSourceColumns",
                            fieldCss: "field-vertical",
                            controlContainerCss: "container-selectable",
                            controlWrapperCss: " h250",
                            labelText: Displays.OptionList(context: context),
                            listItemCollection: ss.EditorSelectableOptions(
                                context: context, enabled: false),
                            commandOptionPositionIsTop: true,
                            commandOptionAction: () => hb
                                .Div(css: "command-center", action: () => hb
                                    .Button(
                                        controlId: "ToEnableEditorColumns",
                                        text: Displays.ToEnable(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.moveColumns($(this),'Editor');",
                                        icon: "ui-icon-circle-triangle-w"))))
                    .FieldCheckBox(
                        controlId: "AllowEditingComments",
                        fieldCss: "field-auto-thin both",
                        labelText: Displays.AllowEditingComments(context: context),
                        _checked: ss.AllowEditingComments == true)
                .FieldSet(id: "RelatingColumnsSettingsEditor",
                    css: " enclosed",
                    legendText: Displays.RelatingColumnSettings(context: context),
                    action: () => hb
                    .Div(css: "command-left", action: () => hb
                        .Button(
                            controlId: "MoveUpRelatingColumns",
                            controlCss: "button-icon",
                            text: Displays.MoveUp(context: context),
                            onClick: "$p.setAndSend('#EditRelatingColumns', $(this));",
                            icon: "ui-icon-circle-triangle-n",
                            action: "SetSiteSettings",
                            method: "post")
                        .Button(
                            controlId: "MoveDownRelatingColumns",
                            controlCss: "button-icon",
                            text: Displays.MoveDown(context: context),
                            onClick: "$p.setAndSend('#EditRelatingColumns', $(this));",
                            icon: "ui-icon-circle-triangle-s",
                            action: "SetSiteSettings",
                            method: "post")
                        .Button(
                            controlId: "NewRelatingColumn",
                            text: Displays.New(context: context),
                            controlCss: "button-icon",
                            onClick: "$p.openRelatingColumnDialog($(this));",
                            icon: "ui-icon-gear",
                            action: "SetSiteSettings",
                            method: "put")
                        .Button(
                            controlId: "DeleteRelatingColumns",
                            text: Displays.Delete(context: context),
                            controlCss: "button-icon",
                            onClick: "$p.setAndSend('#EditRelatingColumns', $(this));",
                            icon: "ui-icon-trash",
                            action: "SetSiteSettings",
                            method: "delete",
                            confirm: Displays.ConfirmDelete(context: context)))
                    .EditRelatingColumns(
                        context: context,
                        ss: ss)));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditorColumnDialog(
            Context context, SiteSettings ss, Column column, IEnumerable<string> titleColumns)
        {
            var hb = new HtmlBuilder();
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("EditorColumnForm")
                    .Action(Locations.ItemAction(ss.SiteId)),
                action: () => hb
                    .EditorColumnDialog(
                        context: context, ss: ss, column: column, titleColumns: titleColumns));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditorColumnDialog(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Column column,
            IEnumerable<string> titleColumns)
        {
            var type = column.TypeName.CsTypeSummary();
            hb.FieldSet(
                css: " enclosed",
                legendText: column.LabelTextDefault,
                action: () =>
                {
                    hb.FieldTextBox(
                        controlId: "LabelText",
                        labelText: Displays.DisplayName(context: context),
                        text: column.LabelText,
                        validateRequired: true);
                    if (column.ColumnName != "Comments" &&
                        column.TypeName != "bit" &&
                        column.ControlType != "Attachments")
                    {
                        var optionCollection = FieldCssOptions(
                            context: context,
                            column: column);
                        hb
                            .FieldDropDown(
                                context: context,
                                controlId: "FieldCss",
                                labelText: Displays.Style(context: context),
                                optionCollection: optionCollection,
                                selectedValue: column.FieldCss,
                                _using: optionCollection?.Any() == true)
                            .FieldCheckBox(
                                controlId: "ValidateRequired",
                                labelText: Displays.Required(context: context),
                                _checked: column.ValidateRequired ?? false,
                                disabled: column.Required,
                                _using: !column.Id_Ver);
                    }
                    switch (type)
                    {
                        case Types.CsNumeric:
                        case Types.CsDateTime:
                        case Types.CsString:
                            hb.FieldCheckBox(
                                controlId: "NoDuplication",
                                labelText: Displays.NoDuplication(context: context),
                                _checked: column.NoDuplication == true,
                                _using:
                                    !column.Id_Ver &&
                                    !column.NotUpdate &&
                                    column.ControlType != "Attachments" &&
                                    column.ColumnName != "Comments");
                            break;
                    }
                    if (!column.Required)
                    {
                        hb
                            .FieldCheckBox(
                                controlId: "CopyByDefault",
                                labelText: Displays.CopyByDefault(context: context),
                                _checked: column.CopyByDefault == true)
                            .FieldCheckBox(
                                controlId: "EditorReadOnly",
                                labelText: Displays.ReadOnly(context: context),
                                _checked: column.EditorReadOnly == true);
                    }
                    if (column.TypeName == "datetime")
                    {
                        hb
                            .FieldDropDown(
                                context: context,
                                controlId: "EditorFormat",
                                labelText: Displays.EditorFormat(context: context),
                                optionCollection: DateTimeOptions(
                                    context: context,
                                    editorFormat: true),
                                selectedValue: column.EditorFormat);
                    }
                    switch (type)
                    {
                        case Types.CsBool:
                            hb.FieldCheckBox(
                                controlId: "DefaultInput",
                                labelText: Displays.DefaultInput(context: context),
                                _checked: column.DefaultInput.ToBool());
                            break;
                        case Types.CsNumeric:
                            if (column.ControlType == "ChoicesText")
                            {
                                hb.FieldTextBox(
                                    controlId: "DefaultInput",
                                    labelText: Displays.DefaultInput(context: context),
                                    text: column.DefaultInput,
                                    _using: !column.Id_Ver);
                            }
                            else
                            {
                                var maxDecimalPlaces = MaxDecimalPlaces(column);
                                hb
                                    .FieldTextBox(
                                        controlId: "DefaultInput",
                                        labelText: Displays.DefaultInput(context: context),
                                        text: column.DefaultInput.ToLong().ToString(),
                                        validateNumber: true,
                                        _using: !column.Id_Ver)
                                    .EditorColumnFormatProperties(
                                        context: context,
                                        column: column)
                                    .FieldTextBox(
                                        controlId: "Unit",
                                        controlCss: " w50",
                                        labelText: Displays.Unit(context: context),
                                        text: column.Unit,
                                        _using: !column.Id_Ver)
                                    .FieldSpinner(
                                        controlId: "DecimalPlaces",
                                        labelText: Displays.DecimalPlaces(context: context),
                                        value: column.DecimalPlaces.ToDecimal(),
                                        min: 0,
                                        max: maxDecimalPlaces,
                                        step: 1,
                                        _using: maxDecimalPlaces > 0);
                                if (!column.NotUpdate && !column.Id_Ver)
                                {
                                    var hidden = column.ControlType != "Spinner"
                                        ? " hidden"
                                        : string.Empty;
                                    hb
                                        .FieldDropDown(
                                            context: context,
                                            controlId: "ControlType",
                                            labelText: Displays.ControlType(context: context),
                                            optionCollection: new Dictionary<string, string>
                                            {
                                                { "Normal", Displays.Normal(context: context) },
                                                { "Spinner", Displays.Spinner(context: context) }
                                            },
                                            selectedValue: column.ControlType)
                                        .FieldTextBox(
                                            fieldId: "MinField",
                                            controlId: "Min",
                                            fieldCss: " both" + hidden,
                                            labelText: Displays.Min(context: context),
                                            text: column.Min.ToString())
                                        .FieldTextBox(
                                            fieldId: "MaxField",
                                            controlId: "Max",
                                            fieldCss: hidden,
                                            labelText: Displays.Max(context: context),
                                            text: column.Max.ToString())
                                        .FieldTextBox(
                                            fieldId: "StepField",
                                            controlId: "Step",
                                            fieldCss: hidden,
                                            labelText: Displays.Step(context: context),
                                            text: column.Step.ToString());
                                }
                            }
                            break;
                        case Types.CsDateTime:
                            hb.FieldSpinner(
                                controlId: "DefaultInput",
                                controlCss: " allow-blank",
                                labelText: Displays.DefaultInput(context: context),
                                value: column.DefaultInput != string.Empty
                                    ? column.DefaultInput.ToDecimal()
                                    : (decimal?)null,
                                min: column.Min.ToInt(),
                                max: column.Max.ToInt(),
                                step: column.Step.ToInt(),
                                width: column.Width);
                            break;
                        case Types.CsString:
                            switch (column.ControlType)
                            {
                                case "Attachments":
                                    hb
                                        .FieldSpinner(
                                            controlId: "LimitQuantity",
                                            labelText: Displays.LimitQuantity(context: context),
                                            value: column.LimitQuantity,
                                            min: Parameters.BinaryStorage.MinQuantity,
                                            max: Parameters.BinaryStorage.MaxQuantity,
                                            step: column.Step.ToInt(),
                                            width: 50)
                                        .FieldSpinner(
                                            controlId: "LimitSize",
                                            labelText: Displays.LimitSize(context: context),
                                            value: column.LimitSize,
                                            min: Parameters.BinaryStorage.MinSize,
                                            max: Parameters.BinaryStorage.MaxSize,
                                            step: column.Step.ToInt(),
                                            width: 50)
                                        .FieldSpinner(
                                            controlId: "LimitTotalSize",
                                            labelText: Displays.LimitTotalSize(context: context),
                                            value: column.TotalLimitSize,
                                            min: Parameters.BinaryStorage.TotalMinSize,
                                            max: Parameters.BinaryStorage.TotalMaxSize,
                                            step: column.Step.ToInt(),
                                            width: 50);
                                    break;
                                default:
                                    hb
                                        .FieldCheckBox(
                                            controlId: "AllowImage",
                                            labelText: Displays.AllowImage(context: context),
                                            _checked: column.AllowImage == true,
                                            _using:
                                                context.ContractSettings.Images()
                                                && (column.ControlType == "MarkDown"
                                                || column.ColumnName == "Comments"))
                                        .FieldTextBox(
                                            textType: column.ControlType == "MarkDown"
                                                ? HtmlTypes.TextTypes.MultiLine
                                                : HtmlTypes.TextTypes.Normal,
                                            controlId: "DefaultInput",
                                            fieldCss: "field-wide",
                                            labelText: Displays.DefaultInput(context: context),
                                            text: column.DefaultInput,
                                            _using: column.ColumnName != "Comments");
                                    break;
                            }
                            break;
                    }
                    hb.FieldTextBox(
                        controlId: "Description",
                        fieldCss: "field-wide",
                        labelText: Displays.Description(context: context),
                        text: column.Description);
                    switch (column.ControlType)
                    {
                        case "ChoicesText":
                            hb
                                .FieldTextBox(
                                    textType: HtmlTypes.TextTypes.MultiLine,
                                    controlId: "ChoicesText",
                                    fieldCss: "field-wide",
                                    labelText: Displays.OptionList(context: context),
                                    text: column.ChoicesText)
                                .FieldCheckBox(
                                    controlId: "UseSearch",
                                    labelText: Displays.UseSearch(context: context),
                                    _checked: column.UseSearch == true);
                            break;
                        default:
                            break;
                    }
                    if (column.ColumnName == "Title")
                    {
                        hb.EditorColumnTitleProperties(
                            context: context,
                            ss: ss,
                            titleColumns: titleColumns);
                    }
                    if (column.ColumnName != "Comments")
                    {
                        hb
                            .FieldCheckBox(
                                controlId: "NoWrap",
                                labelText: Displays.NoWrap(context: context),
                                _checked: column.NoWrap == true)
                            .FieldTextBox(
                                controlId: "Section",
                                labelText: Displays.Section(context: context),
                                text: column.Section);
                    }
                });
            return hb
                .Hidden(
                    controlId: "EditorColumnName",
                    css: "always-send",
                    value: column.ColumnName)
                .P(css: "message-dialog")
                .Div(css: "command-center", action: () => hb
                    .Button(
                        controlId: "SetEditorColumn",
                        text: Displays.Change(context: context),
                        controlCss: "button-icon validate",
                        onClick: "$p.send($(this));",
                        icon: "ui-icon-gear",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "ResetEditorColumn",
                        text: Displays.Reset(context: context),
                        controlCss: "button-icon validate",
                        onClick: "$p.resetEditorColumn($(this));",
                        icon: "ui-icon-gear",
                        action: "SetSiteSettings",
                        method: "post",
                        confirm: "ConfirmReset")
                    .Button(
                        text: Displays.Cancel(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.closeDialog($(this));",
                        icon: "ui-icon-cancel"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static Dictionary<string, string> FieldCssOptions(Context context, Column column)
        {
            switch (column.ControlType)
            {
                case "MarkDown":
                    return new Dictionary<string, string>
                    {
                        { "field-normal", Displays.Normal(context: context) },
                        { "field-wide", Displays.Wide(context: context) },
                        { "field-markdown", Displays.MarkDown(context: context) }
                    };
                case "Attachment":
                    return null;
                default:
                    return new Dictionary<string, string>
                    {
                        { "field-normal", Displays.Normal(context: context) },
                        { "field-wide", Displays.Wide(context: context) }
                    };
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditorColumnFormatProperties(
            this HtmlBuilder hb, Context context, Column column)
        {
            var formats = Parameters.Formats
                .Where(o => (o.Type & ParameterAccessor.Parts.Format.Types.NumColumn) > 0);
            var format = formats.FirstOrDefault(o => o.String == column.Format);
            var other = !column.Format.IsNullOrEmpty() && format == null;
            return hb
                .FieldDropDown(
                    context: context,
                    controlId: "FormatSelector",
                    controlCss: " not-send",
                    labelText: Displays.Format(context: context),
                    optionCollection: formats
                        .ToDictionary(o => o.String, o => Displays.Get(
                            context: context,
                            id: o.Name)),
                    selectedValue: format != null
                        ? format.String
                        : other
                            ? "\t"
                            : string.Empty,
                    _using: !column.Id_Ver)
                .FieldTextBox(
                    fieldId: "FormatField",
                    controlId: "Format",
                    fieldCss: other ? string.Empty : " hidden",
                    labelText: Displays.Custom(context: context),
                    text: other
                        ? column.Format
                        : string.Empty);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditorColumnTitleProperties(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            IEnumerable<string> titleColumns)
        {
            return hb
                .FieldSelectable(
                    controlId: "TitleColumns",
                    fieldCss: "field-vertical",
                    controlContainerCss: "container-selectable",
                    controlWrapperCss: " h200",
                    controlCss: " always-send send-all",
                    labelText: Displays.CurrentSettings(context: context),
                    listItemCollection: ss
                        .TitleSelectableOptions(
                            context: context,
                            titleColumns: titleColumns),
                    commandOptionPositionIsTop: true,
                    commandOptionAction: () => hb
                        .Div(css: "command-center", action: () => hb
                            .Button(
                                controlId: "MoveUpTitleColumns",
                                text: Displays.MoveUp(context: context),
                                controlCss: "button-icon",
                                onClick: "$p.moveColumns($(this),'Title');",
                                icon: "ui-icon-circle-triangle-n")
                            .Button(
                                controlId: "MoveDownTitleColumns",
                                text: Displays.MoveDown(context: context),
                                controlCss: "button-icon",
                                onClick: "$p.moveColumns($(this),'Title');",
                                icon: "ui-icon-circle-triangle-s")
                            .Button(
                                controlId: "ToDisableTitleColumns",
                                text: Displays.ToDisable(context: context),
                                controlCss: "button-icon",
                                onClick: "$p.moveColumns($(this),'Title');",
                                icon: "ui-icon-circle-triangle-e")
                            .Button(
                                controlCss: "button-icon",
                                text: Displays.Synchronize(context: context),
                                onClick: "$p.send($(this));",
                                icon: "ui-icon-refresh",
                                action: "SynchronizeTitles",
                                method: "put",
                                confirm: Displays.ConfirmSynchronize(context: context))))
                .FieldSelectable(
                    controlId: "TitleSourceColumns",
                    fieldCss: "field-vertical",
                    controlContainerCss: "container-selectable",
                    controlWrapperCss: " h200",
                    labelText: Displays.OptionList(context: context),
                    listItemCollection: ss
                        .TitleSelectableOptions(
                            context: context,
                            titleColumns: titleColumns,
                            enabled: false),
                    commandOptionPositionIsTop: true,
                    commandOptionAction: () => hb
                        .Div(css: "command-center", action: () => hb
                            .Button(
                                controlId: "ToEnableTitleColumns",
                                text: Displays.ToEnable(context: context),
                                controlCss: "button-icon",
                                onClick: "$p.moveColumns($(this),'Title');",
                                icon: "ui-icon-circle-triangle-w")))
                .FieldTextBox(
                    controlId: "TitleSeparator",
                    fieldCss: " both",
                    labelText: Displays.TitleSeparator(context: context),
                    text: ss.TitleSeparator);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static Dictionary<string, string> DateTimeOptions(
            Context context, bool editorFormat = false)
        {
            return editorFormat
                ? DisplayAccessor.Displays.DisplayHash
                    .Where(o => new string[] { "Ymd", "Ymdhm" }.Contains(o.Key))
                    .ToDictionary(o => o.Key, o => Displays.Get(
                        context: context,
                        id: o.Key))
                : DisplayAccessor.Displays.DisplayHash
                    .Where(o => o.Value.Type == DisplayAccessor.Displays.Types.Date)
                    .ToDictionary(o => o.Key, o => Displays.Get(
                        context: context,
                        id: o.Key));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static int MaxDecimalPlaces(Column column)
        {
            return column.Size.Split_2nd().ToInt();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder LinksSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return hb.FieldSet(id: "LinksSettingsEditor", action: () => hb
                .FieldSet(
                    css: " enclosed",
                    legendText: Displays.ListSettings(context: context),
                    action: () => hb
                        .FieldSelectable(
                            controlId: "LinkColumns",
                            fieldCss: "field-vertical",
                            controlContainerCss: "container-selectable",
                            controlWrapperCss: " h350",
                            controlCss: " always-send send-all",
                            labelText: Displays.CurrentSettings(context: context),
                            listItemCollection: ss.LinkSelectableOptions(context: context),
                            commandOptionPositionIsTop: true,
                            commandOptionAction: () => hb
                                .Div(css: "command-center", action: () => hb
                                    .Button(
                                        controlId: "MoveUpLinkColumns",
                                        text: Displays.MoveUp(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.moveColumns($(this),'Link');",
                                        icon: "ui-icon-circle-triangle-n")
                                    .Button(
                                        controlId: "MoveDownLinkColumns",
                                        text: Displays.MoveDown(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.moveColumns($(this),'Link');",
                                        icon: "ui-icon-circle-triangle-s")
                                    .Button(
                                        controlId: "ToDisableLinkColumns",
                                        text: Displays.ToDisable(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.moveColumns($(this),'Link');",
                                        icon: "ui-icon-circle-triangle-e")))
                        .FieldSelectable(
                            controlId: "LinkSourceColumns",
                            fieldCss: "field-vertical",
                            controlContainerCss: "container-selectable",
                            controlWrapperCss: " h350",
                            labelText: Displays.OptionList(context: context),
                            listItemCollection: ss.LinkSelectableOptions(
                                context: context, enabled: false),
                            commandOptionPositionIsTop: true,
                            commandOptionAction: () => hb
                                .Div(css: "command-center", action: () => hb
                                    .Button(
                                        controlId: "ToEnableLinkColumns",
                                        text: Displays.ToEnable(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.moveColumns($(this),'Link');",
                                        icon: "ui-icon-circle-triangle-w")))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder HistoriesSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return hb.FieldSet(id: "HistoriesSettingsEditor", action: () => hb
                .FieldSet(
                    css: " enclosed",
                    legendText: Displays.ListSettings(context: context),
                    action: () => hb
                        .FieldSelectable(
                            controlId: "HistoryColumns",
                            fieldCss: "field-vertical",
                            controlContainerCss: "container-selectable",
                            controlWrapperCss: " h350",
                            controlCss: " always-send send-all",
                            labelText: Displays.CurrentSettings(context: context),
                            listItemCollection: ss.HistorySelectableOptions(context: context),
                            commandOptionPositionIsTop: true,
                            commandOptionAction: () => hb
                                .Div(css: "command-center", action: () => hb
                                    .Button(
                                        controlId: "MoveUpHistoryColumns",
                                        text: Displays.MoveUp(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.moveColumns($(this),'History');",
                                        icon: "ui-icon-circle-triangle-n")
                                    .Button(
                                        controlId: "MoveDownHistoryColumns",
                                        text: Displays.MoveDown(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.moveColumns($(this),'History');",
                                        icon: "ui-icon-circle-triangle-s")
                                    .Button(
                                        controlId: "ToDisableHistoryColumns",
                                        text: Displays.ToDisable(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.moveColumns($(this),'History');",
                                        icon: "ui-icon-circle-triangle-e")))
                        .FieldSelectable(
                            controlId: "HistorySourceColumns",
                            fieldCss: "field-vertical",
                            controlContainerCss: "container-selectable",
                            controlWrapperCss: " h350",
                            labelText: Displays.OptionList(context: context),
                            listItemCollection: ss.HistorySelectableOptions(
                                context: context, enabled: false),
                            commandOptionPositionIsTop: true,
                            commandOptionAction: () => hb
                                .Div(css: "command-center", action: () => hb
                                    .Button(
                                        controlId: "ToEnableHistoryColumns",
                                        text: Displays.ToEnable(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.moveColumns($(this),'History');",
                                        icon: "ui-icon-circle-triangle-w")))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SummariesSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return hb.FieldSet(id: "SummariesSettingsEditor", action: () => hb
                .Div(css: "command-left", action: () => hb
                    .Button(
                        controlId: "MoveUpSummaries",
                        controlCss: "button-icon",
                        text: Displays.MoveUp(context: context),
                        onClick: "$p.setAndSend('#EditSummary', $(this));",
                        icon: "ui-icon-circle-triangle-n",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "MoveDownSummaries",
                        controlCss: "button-icon",
                        text: Displays.MoveDown(context: context),
                        onClick: "$p.setAndSend('#EditSummary', $(this));",
                        icon: "ui-icon-circle-triangle-s",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "NewSummary",
                        text: Displays.New(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.openSummaryDialog($(this));",
                        icon: "ui-icon-gear",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "DeleteSummaries",
                        controlCss: "button-icon",
                        text: Displays.Delete(context: context),
                        onClick: "$p.setAndSend('#EditSummary', $(this));",
                        icon: "ui-icon-trash",
                        action: "SetSiteSettings",
                        method: "post",
                        confirm: Displays.ConfirmDelete(context: context))
                    .Button(
                        controlId: "SynchronizeSummaries",
                        controlCss: "button-icon",
                        text: Displays.Synchronize(context: context),
                        onClick: "$p.setAndSend('#EditSummary', $(this));",
                        icon: "ui-icon-refresh",
                        action: "SynchronizeSummaries",
                        method: "put",
                        confirm: Displays.ConfirmSynchronize(context: context)))
                .EditSummary(context: context, ss: ss));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditSummary(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            var selected = Forms.Data("EditSummary").Deserialize<IEnumerable<int>>();
            return hb
                .Table(
                    id: "EditSummary",
                    css: "grid",
                    attributes: new HtmlAttributes()
                        .DataName("SummaryId")
                        .DataFunc("openSummaryDialog")
                        .DataAction("SetSiteSettings")
                        .DataMethod("post"),
                    action: () => hb
                        .SummariesHeader(
                            context: context,
                            ss: ss,
                            selected: selected)
                        .SummariesBody(
                            context: context,
                            ss: ss,
                            selected: selected));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SummariesHeader(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            IEnumerable<int> selected)
        {
            return hb.THead(action: () => hb
                .Tr(css: "ui-widget-header", action: () => hb
                    .Th(attributes: new HtmlAttributes()
                            .Rowspan(2),
                        action: () => hb
                            .CheckBox(
                                controlCss: "select-all",
                                _checked: ss.Summaries?.All(o =>
                                    selected?.Contains(o.Id) == true) == true))
                    .Th(attributes: new HtmlAttributes()
                            .Rowspan(2),
                        action: () => hb
                            .Text(text: Displays.Id(context: context)))
                    .Th(attributes: new HtmlAttributes()
                            .Colspan(3),
                        action: () => hb
                            .Text(text: Displays.DataStorageDestination(context: context)))
                    .Th(attributes: new HtmlAttributes()
                            .Colspan(4),
                        action: () => hb
                            .Text(text: ss.Title)))
                .Tr(css: "ui-widget-header", action: () => hb
                    .Th(action: () => hb
                        .Text(text: Displays.Sites(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Column(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Condition(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.SummaryLinkColumn(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.SummaryType(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.SummarySourceColumn(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Condition(context: context)))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SummariesBody(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            IEnumerable<int> selected)
        {
            if (ss.Summaries?.Any() == true)
            {
                var dataRows = Rds.ExecuteTable(
                    context: context,
                    statements: Rds.SelectSites(
                        column: Rds.SitesColumn()
                            .SiteId()
                            .ReferenceType()
                            .Title()
                            .SiteSettings(),
                        where: Rds.SitesWhere()
                            .TenantId(context.TenantId)
                            .SiteId_In(ss.Summaries?
                                .Select(o => o.SiteId)))).AsEnumerable();
                hb.TBody(action: () =>
                {
                    ss.Summaries?.ForEach(summary =>
                    {
                        var dataRow = dataRows.FirstOrDefault(o =>
                            o["SiteId"].ToLong() == summary.SiteId);
                        var destinationSs = SiteSettingsUtilities.Get(
                            context: context, dataRow: dataRow);
                        if (destinationSs != null)
                        {
                            hb.Tr(
                                css: "grid-row",
                                attributes: new HtmlAttributes()
                                    .DataId(summary.Id.ToString()),
                                action: () => hb
                                    .Td(action: () => hb
                                        .CheckBox(
                                            controlCss: "select",
                                            _checked: selected?.Contains(summary.Id) == true))
                                    .Td(action: () => hb
                                        .Text(text: summary.Id.ToString()))
                                    .Td(action: () => hb
                                        .Text(text: dataRow["Title"].ToString()))
                                    .Td(action: () => hb
                                        .Text(text: destinationSs.GetColumn(
                                            context: context,
                                            columnName: summary.DestinationColumn)?.LabelText))
                                    .Td(action: () => hb
                                        .Text(text: destinationSs.Views?.Get(
                                            summary.DestinationCondition)?.Name))
                                    .Td(action: () => hb
                                        .Text(text: ss.GetColumn(
                                            context: context,
                                            columnName: summary.LinkColumn)?.LabelText))
                                    .Td(action: () => hb
                                        .Text(text: SummaryType(
                                            context: context,
                                            type: summary.Type)))
                                    .Td(action: () => hb
                                        .Text(text: ss.GetColumn(
                                            context: context,
                                            columnName: summary.SourceColumn)?.LabelText))
                                    .Td(action: () => hb
                                        .Text(text: ss.Views?.Get(
                                            summary.SourceCondition)?.Name)));
                        }
                    });
                });
            }
            return hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder SummaryDialog(
            Context context, SiteSettings ss, string controlId, Summary summary)
        {
            var hb = new HtmlBuilder();
            var destinationSiteHash = ss.Destinations?
                .ToDictionary(o => o.SiteId.ToString(), o => o.Title);
            var destinationSs = ss.Destinations?.Get(summary.SiteId);
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("SummaryForm")
                    .Action(Locations.ItemAction(ss.SiteId)),
                action: () => hb
                    .FieldText(
                        controlId: "SummaryId",
                        controlCss: " always-send",
                        labelText: Displays.Id(context: context),
                        text: summary.Id.ToString(),
                        _using: controlId == "EditSummary")
                    .FieldSet(
                        css: "fieldset enclosed-half h250 both",
                        legendText: Displays.DataStorageDestination(context: context),
                        action: () => hb
                            .FieldDropDown(
                                context: context,
                                controlId: "SummarySiteId",
                                controlCss: " auto-postback always-send",
                                labelText: Displays.Sites(context: context),
                                optionCollection: destinationSiteHash,
                                action: "SetSiteSettings",
                                method: "post")
                            .SummaryDestinationColumn(
                                context: context,
                                destinationSs: destinationSs,
                                destinationColumn: summary.DestinationColumn)
                            .FieldDropDown(
                                context: context,
                                controlId: "SummaryDestinationCondition",
                                controlCss: " always-send",
                                labelText: Displays.Condition(context: context),
                                optionCollection: destinationSs?.ViewSelectableOptions(),
                                selectedValue: summary.DestinationCondition.ToString(),
                                insertBlank: true,
                                _using: destinationSs?.Views?.Any() == true)
                            .FieldCheckBox(
                                fieldId: "SummarySetZeroWhenOutOfConditionField",
                                controlId: "SummarySetZeroWhenOutOfCondition",
                                fieldCss: "field-auto-thin right" +
                                    (destinationSs?.Views?.Any(o =>
                                        o.Id == summary.DestinationCondition) == true
                                            ? null
                                            : " hidden"),
                                controlCss: " always-send",
                                labelText: Displays.SetZeroWhenOutOfCondition(context: context),
                                _checked: summary.SetZeroWhenOutOfCondition == true))
                    .FieldSet(
                        css: "fieldset enclosed-half h250",
                        legendText: ss.Title,
                        action: () => hb
                            .SummaryLinkColumn(
                                context: context,
                                ss: ss,
                                siteId: summary.SiteId,
                                linkColumn: summary.LinkColumn)
                            .FieldDropDown(
                                context: context,
                                controlId: "SummaryType",
                                controlCss: " auto-postback always-send",
                                labelText: Displays.SummaryType(context: context),
                                optionCollection: SummaryTypeCollection(context: context),
                                selectedValue: summary.Type,
                                action: "SetSiteSettings",
                                method: "post")
                            .SummarySourceColumn(
                                context: context,
                                ss: ss,
                                type: summary.Type,
                                sourceColumn: summary.SourceColumn)
                            .FieldDropDown(
                                context: context,
                                controlId: "SummarySourceCondition",
                                controlCss: " always-send",
                                labelText: Displays.Condition(context: context),
                                optionCollection: ss.ViewSelectableOptions(),
                                selectedValue: summary.SourceCondition.ToString(),
                                insertBlank: true,
                                _using: ss.Views?.Any() == true))
                    .P(css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "AddSummary",
                            text: Displays.Add(context: context),
                            controlCss: "button-icon validate",
                            onClick: "$p.setSummary($(this));",
                            icon: "ui-icon-disk",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "NewSummary")
                        .Button(
                            controlId: "UpdateSummary",
                            text: Displays.Change(context: context),
                            controlCss: "button-icon validate",
                            onClick: "$p.setSummary($(this));",
                            icon: "ui-icon-disk",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "EditSummary")
                        .Button(
                            text: Displays.Cancel(context: context),
                            controlCss: "button-icon",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder SummaryDestinationColumn(
            this HtmlBuilder hb,
            Context context,
            SiteSettings destinationSs,
            string destinationColumn = null)
        {
            return hb.FieldDropDown(
                context: context,
                fieldId: "SummaryDestinationColumnField",
                controlId: "SummaryDestinationColumn",
                controlCss: " always-send",
                labelText: Displays.Column(context: context),
                optionCollection: destinationSs?.Columns?
                    .Where(o => o.Computable)
                    .Where(o => o.TypeName != "datetime")
                    .Where(o => !o.NotUpdate)
                    .OrderBy(o => o.No)
                    .ToDictionary(
                        o => o.ColumnName,
                        o => o.LabelText),
                selectedValue: destinationColumn,
                action: "SetSiteSettings",
                method: "post");
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static Dictionary<string, string> SummaryTypeCollection(Context context)
        {
            return new Dictionary<string, string>
            {
                { "Count", Displays.Count(context: context) },
                { "Total", Displays.Total(context: context) },
                { "Average", Displays.Average(context: context) },
                { "Min", Displays.Min(context: context) },
                { "Max", Displays.Max(context: context) }
            };
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder SummaryLinkColumn(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            long siteId,
            string linkColumn = null)
        {
            return hb.FieldDropDown(
                context: context,
                fieldId: "SummaryLinkColumnField",
                controlId: "SummaryLinkColumn",
                controlCss: " always-send",
                labelText: Displays.SummaryLinkColumn(context: context),
                optionCollection: ss.Links
                    .Where(o => o.SiteId == siteId)
                    .ToDictionary(
                        o => o.ColumnName,
                        o => ss.GetColumn(
                            context: context,
                            columnName: o.ColumnName).LabelText),
                selectedValue: linkColumn,
                action: "SetSiteSettings",
                method: "post");
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder SummarySourceColumn(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            string type = "Count",
            string sourceColumn = null)
        {
            switch (type)
            {
                case "Total":
                case "Average":
                case "Max":
                case "Min":
                    return hb.FieldDropDown(
                        context: context,
                        fieldId: "SummarySourceColumnField",
                        controlId: "SummarySourceColumn",
                        controlCss: " always-send",
                        labelText: Displays.SummarySourceColumn(context: context),
                        optionCollection: ss.Columns
                            .Where(o => o.Computable)
                            .Where(o => o.TypeName != "datetime")
                            .ToDictionary(o => o.ColumnName, o => o.LabelText),
                        selectedValue: sourceColumn,
                        action: "SetSiteSettings",
                        method: "post");
                default:
                    return hb.FieldContainer(
                        fieldId: "SummarySourceColumnField",
                        fieldCss: " hidden");
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string SummaryType(Context context, string type)
        {
            switch (type)
            {
                case "Count": return Displays.Count(context: context);
                case "Total": return Displays.Total(context: context);
                case "Average": return Displays.Average(context: context);
                case "Min": return Displays.Min(context: context);
                case "Max": return Displays.Max(context: context);
                default: return string.Empty;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder FormulasSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return hb.FieldSet(id: "FormulasSettingsEditor", action: () => hb
                .Div(css: "command-left", action: () => hb
                    .Button(
                        controlId: "MoveUpFormulas",
                        controlCss: "button-icon",
                        text: Displays.MoveUp(context: context),
                        onClick: "$p.setAndSend('#EditFormula', $(this));",
                        icon: "ui-icon-circle-triangle-n",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "MoveDownFormulas",
                        controlCss: "button-icon",
                        text: Displays.MoveDown(context: context),
                        onClick: "$p.setAndSend('#EditFormula', $(this));",
                        icon: "ui-icon-circle-triangle-s",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "NewFormula",
                        text: Displays.New(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.openFormulaDialog($(this));",
                        icon: "ui-icon-gear",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "DeleteFormulas",
                        controlCss: "button-icon",
                        text: Displays.Delete(context: context),
                        onClick: "$p.setAndSend('#EditFormula', $(this));",
                        icon: "ui-icon-trash",
                        action: "SetSiteSettings",
                        method: "post",
                        confirm: Displays.ConfirmDelete(context: context))
                    .Button(
                        controlId: "SynchronizeFormulas",
                        controlCss: "button-icon",
                        text: Displays.Synchronize(context: context),
                        onClick: "$p.setAndSend('#EditFormula', $(this));",
                        icon: "ui-icon-refresh",
                        action: "SynchronizeFormulas",
                        method: "put",
                        confirm: Displays.ConfirmSynchronize(context: context)))
                .EditFormula(context: context, ss: ss));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditFormula(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            var selected = Forms.Data("EditFormula").Deserialize<IEnumerable<int>>();
            return hb
                .Table(
                    id: "EditFormula",
                    css: "grid",
                    attributes: new HtmlAttributes()
                        .DataName("FormulaId")
                        .DataFunc("openFormulaDialog")
                        .DataAction("SetSiteSettings")
                        .DataMethod("post"),
                    action: () => hb
                        .FormulasHeader(
                            context: context,
                            ss: ss,
                            selected: selected)
                        .FormulasBody(
                            context: context,
                            ss: ss,
                            selected: selected));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder FormulasHeader(
            this HtmlBuilder hb, Context context, SiteSettings ss, IEnumerable<int> selected)
        {
            return hb.THead(action: () => hb
                .Tr(css: "ui-widget-header", action: () => hb
                    .Th(action: () => hb
                        .CheckBox(
                            controlCss: "select-all",
                            _checked: ss.Formulas?.All(o =>
                                selected?.Contains(o.Id) == true) == true))
                    .Th(action: () => hb
                            .Text(text: Displays.Id(context: context)))
                    .Th(action: () => hb
                            .Text(text: Displays.Target(context: context)))
                    .Th(action: () => hb
                            .Text(text: Displays.Formulas(context: context)))
                    .Th(action: () => hb
                            .Text(text: Displays.Condition(context: context)))
                    .Th(action: () => hb
                            .Text(text: Displays.OutOfCondition(context: context)))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder FormulasBody(
            this HtmlBuilder hb, Context context, SiteSettings ss, IEnumerable<int> selected)
        {
            if (ss.Formulas?.Any() == true)
            {
                hb.TBody(action: () =>
                {
                    ss.Formulas?.ForEach(formulaSet =>
                    {
                        hb.Tr(
                            css: "grid-row",
                            attributes: new HtmlAttributes()
                                .DataId(formulaSet.Id.ToString()),
                            action: () => hb
                                .Td(action: () => hb
                                    .CheckBox(
                                        controlCss: "select",
                                        _checked: selected?.Contains(formulaSet.Id) == true))
                                .Td(action: () => hb
                                    .Text(text: formulaSet.Id.ToString()))
                                .Td(action: () => hb
                                    .Text(text: ss.GetColumn(
                                        context: context,
                                        columnName: formulaSet.Target)?.LabelText))
                                .Td(action: () => hb
                                    .Text(text: formulaSet.Formula?.ToString(ss)))
                                .Td(action: () => hb
                                    .Text(text: ss.Views?.Get(formulaSet.Condition)?.Name))
                                .Td(action: () => hb
                                    .Text(text: formulaSet.OutOfCondition?.ToString(ss))));
                    });
                });
            }
            return hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder FormulaDialog(
            Context context, SiteSettings ss, string controlId, FormulaSet formulaSet)
        {
            var hb = new HtmlBuilder();
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("FormulaForm")
                    .Action(Locations.ItemAction(ss.SiteId)),
                action: () => hb
                    .FieldText(
                        controlId: "FormulaId",
                        controlCss: " always-send",
                        labelText: Displays.Id(context: context),
                        text: formulaSet.Id.ToString(),
                        _using: controlId == "EditFormula")
                    .FieldDropDown(
                        context: context,
                        controlId: "FormulaTarget",
                        controlCss: " always-send",
                        labelText: Displays.Target(context: context),
                        optionCollection: ss.FormulaTargetSelectableOptions(),
                        selectedValue: formulaSet.Target?.ToString())
                    .FieldTextBox(
                        controlId: "Formula",
                        controlCss: " always-send",
                        fieldCss: "field-wide",
                        labelText: Displays.Formulas(context: context),
                        text: formulaSet.Formula?.ToString(ss),
                        validateRequired: true)
                    .FieldDropDown(
                        context: context,
                        controlId: "FormulaCondition",
                        controlCss: " always-send",
                        labelText: Displays.Condition(context: context),
                        optionCollection: ss.ViewSelectableOptions(),
                        selectedValue: formulaSet.Condition?.ToString(),
                        insertBlank: true,
                        _using: ss.Views?.Any() == true)
                    .FieldTextBox(
                        fieldId: "FormulaOutOfConditionField",
                        controlId: "FormulaOutOfCondition",
                        controlCss: " always-send",
                        fieldCss: "field-wide" + (ss.Views?
                            .Any(o => o.Id == formulaSet.Condition) == true
                                ? string.Empty
                                : " hidden"),
                        labelText: Displays.OutOfCondition(context: context),
                        text: formulaSet.OutOfCondition?.ToString(ss))
                    .P(css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "AddFormula",
                            text: Displays.Add(context: context),
                            controlCss: "button-icon validate",
                            onClick: "$p.send($(this));",
                            icon: "ui-icon-disk",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "NewFormula")
                        .Button(
                            controlId: "UpdateFormula",
                            text: Displays.Change(context: context),
                            controlCss: "button-icon validate",
                            onClick: "$p.send($(this));",
                            icon: "ui-icon-disk",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "EditFormula")
                        .Button(
                            text: Displays.Cancel(context: context),
                            controlCss: "button-icon",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ViewsSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return hb.FieldSet(id: "ViewsSettingsEditor", action: () => hb
                .FieldSelectable(
                    controlId: "Views",
                    fieldCss: "field-vertical w400",
                    controlContainerCss: "container-selectable",
                    controlWrapperCss: " h350",
                    controlCss: " always-send send-all",
                    listItemCollection: ss.ViewSelectableOptions(),
                    commandOptionPositionIsTop: true,
                    commandOptionAction: () => hb
                        .Div(css: "command-center", action: () => hb
                            .Button(
                                controlId: "MoveUpViews",
                                text: Displays.MoveUp(context: context),
                                controlCss: "button-icon",
                                onClick: "$p.moveColumnsById($(this),'Views', '');",
                                icon: "ui-icon-circle-triangle-n")
                            .Button(
                                controlId: "MoveDownViews",
                                text: Displays.MoveDown(context: context),
                                controlCss: "button-icon",
                                onClick: "$p.moveColumnsById($(this),'Views', '');",
                                icon: "ui-icon-circle-triangle-s")
                            .Button(
                                controlId: "NewView",
                                text: Displays.New(context: context),
                                controlCss: "button-icon",
                                onClick: "$p.openViewDialog($(this));",
                                icon: "ui-icon-gear",
                                action: "SetSiteSettings",
                                method: "put")
                            .Button(
                                controlId: "EditView",
                                text: Displays.AdvancedSetting(context: context),
                                controlCss: "button-icon",
                                onClick: "$p.openViewDialog($(this));",
                                icon: "ui-icon-gear",
                                action: "SetSiteSettings",
                                method: "put")
                            .Button(
                                controlId: "DeleteViews",
                                text: Displays.Delete(context: context),
                                controlCss: "button-icon",
                                onClick: "$p.send($(this));",
                                icon: "ui-icon-trash",
                                action: "SetSiteSettings",
                                method: "put"))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder ViewDialog(
            Context context, SiteSettings ss, string controlId, View view)
        {
            var hb = new HtmlBuilder();
            var hasCalendar = Def.ViewModeDefinitionCollection
                .Any(o => o.Name == "Calendar" && o.ReferenceType == ss.ReferenceType);
            var hasCrosstab = Def.ViewModeDefinitionCollection
                .Any(o => o.Name == "Crosstab" && o.ReferenceType == ss.ReferenceType);
            var hasGantt = Def.ViewModeDefinitionCollection
                .Any(o => o.Name == "Gantt" && o.ReferenceType == ss.ReferenceType);
            var hasTimeSeries = Def.ViewModeDefinitionCollection
                .Any(o => o.Name == "TimeSeries" && o.ReferenceType == ss.ReferenceType);
            var hasKamban = Def.ViewModeDefinitionCollection
                .Any(o => o.Name == "Kamban" && o.ReferenceType == ss.ReferenceType);
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("ViewForm")
                    .Action(Locations.ItemAction(ss.SiteId)),
                action: () => hb
                    .FieldText(
                        controlId: "ViewId",
                        controlCss: " always-send",
                        labelText: Displays.Id(context: context),
                        text: view.Id.ToString())
                    .FieldTextBox(
                        controlId: "ViewName",
                        labelText: Displays.Name(context: context),
                        text: view.Name,
                        validateRequired: true)
                    .Div(id: "ViewTabsContainer", action: () => hb
                        .Ul(id: "ViewTabs", action: () => hb
                            .Li(action: () => hb
                                .A(
                                    href: "#ViewGridTab",
                                    text: Displays.Grid(context: context)))
                            .Li(action: () => hb
                                .A(
                                    href: "#ViewFiltersTab",
                                    text: Displays.Filters(context: context)))
                            .Li(action: () => hb
                                .A(
                                    href: "#ViewSortersTab",
                                    text: Displays.Sorters(context: context)))
                            .Li(
                                action: () => hb
                                    .A(
                                        href: "#ViewCalendarTab",
                                        text: Displays.Calendar(context: context)),
                                _using: hasCalendar)
                            .Li(
                                action: () => hb
                                    .A(
                                        href: "#ViewCrosstabTab",
                                        text: Displays.Crosstab(context: context)),
                                _using: hasCrosstab)
                            .Li(
                                action: () => hb
                                    .A(
                                        href: "#ViewGanttTab",
                                        text: Displays.Gantt(context: context)),
                                _using: hasGantt)
                            .Li(
                                action: () => hb
                                    .A(
                                        href: "#ViewTimeSeriesTab",
                                        text: Displays.TimeSeries(context: context)),
                                _using: hasTimeSeries)
                            .Li(
                                action: () => hb
                                    .A(
                                        href: "#ViewKambanTab",
                                        text: Displays.Kamban(context: context)),
                                _using: hasKamban))
                        .ViewGridTab(context: context, ss: ss, view: view)
                        .ViewFiltersTab(context: context, ss: ss, view: view)
                        .ViewSortersTab(context: context, ss: ss, view: view)
                        .ViewCalendarTab(context: context, ss: ss, view: view, _using: hasCalendar)
                        .ViewCrosstabTab(context: context, ss: ss, view: view, _using: hasCrosstab)
                        .ViewGanttTab(context: context, ss: ss, view: view, _using: hasGantt)
                        .ViewTimeSeriesTab(context: context, ss: ss, view: view, _using: hasTimeSeries)
                        .ViewKambanTab(context: context, ss: ss, view: view, _using: hasKamban))
                    .P(css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "AddView",
                            text: Displays.Add(context: context),
                            controlCss: "button-icon validate",
                            onClick: "$p.send($(this));",
                            icon: "ui-icon-disk",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "NewView")
                        .Button(
                            controlId: "UpdateView",
                            text: Displays.Change(context: context),
                            controlCss: "button-icon validate",
                            onClick: "$p.send($(this));",
                            icon: "ui-icon-disk",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "EditView")
                        .Button(
                            text: Displays.Cancel(context: context),
                            controlCss: "button-icon",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ViewGridTab(
            this HtmlBuilder hb, Context context, SiteSettings ss, View view)
        {
            return hb.FieldSet(id: "ViewGridTab", action: () => hb
                .FieldSet(
                    css: " enclosed-thin",
                    legendText: Displays.ListSettings(context: context),
                    action: () => hb
                        .FieldSelectable(
                            controlId: "ViewGridColumns",
                            fieldCss: "field-vertical",
                            controlContainerCss: "container-selectable",
                            controlWrapperCss: " h350",
                            controlCss: " always-send send-all",
                            labelText: Displays.CurrentSettings(context: context),
                            listItemCollection: ss.ViewGridSelectableOptions(
                                context: context,
                                gridColumns: view.GridColumns ?? ss.GridColumns),
                            selectedValueCollection: new List<string>(),
                            commandOptionPositionIsTop: true,
                            commandOptionAction: () => hb
                                .Div(css: "command-center", action: () => hb
                                    .Button(
                                        controlId: "MoveUpViewGridColumns",
                                        controlCss: "button-icon",
                                        text: Displays.MoveUp(context: context),
                                        onClick: "$p.moveColumns($(this),'ViewGrid',false,true);",
                                        icon: "ui-icon-circle-triangle-n")
                                    .Button(
                                        controlId: "MoveDownViewGridColumns",
                                        controlCss: "button-icon",
                                        text: Displays.MoveDown(context: context),
                                        onClick: "$p.moveColumns($(this),'ViewGrid',false,true);",
                                        icon: "ui-icon-circle-triangle-s")
                                    .Button(
                                        controlId: "ToDisableViewGridColumns",
                                        controlCss: "button-icon",
                                        text: Displays.ToDisable(context: context),
                                        onClick: "$p.moveColumns($(this),'ViewGrid',false,true);",
                                        icon: "ui-icon-circle-triangle-e")))
                        .FieldSelectable(
                            controlId: "ViewGridSourceColumns",
                            fieldCss: "field-vertical",
                            controlContainerCss: "container-selectable",
                            controlWrapperCss: " h350",
                            labelText: Displays.OptionList(context: context),
                            listItemCollection: ss.ViewGridSelectableOptions(
                                context: context,
                                gridColumns: view.GridColumns ?? ss.GridColumns,
                                enabled: false),
                            commandOptionPositionIsTop: true,
                            commandOptionAction: () => hb
                                .Div(css: "command-left", action: () => hb
                                    .Button(
                                        controlId: "ToEnableViewGridColumns",
                                        text: Displays.ToEnable(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.moveColumns($(this),'ViewGrid',false,true);",
                                        icon: "ui-icon-circle-triangle-w")
                                    .FieldDropDown(
                                        context: context,
                                        controlId: "ViewGridJoin",
                                        fieldCss: "w150",
                                        controlCss: " auto-postback always-send",
                                        optionCollection: ss.JoinOptionHash,
                                        addSelectedValue: false,
                                        action: "SetSiteSettings",
                                        method: "post")))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ViewFiltersTab(
            this HtmlBuilder hb, Context context, SiteSettings ss, View view)
        {
            return hb.FieldSet(id: "ViewFiltersTab", action: () => hb
                .Div(css: "items", action: () => hb
                    .FieldCheckBox(
                        controlId: "ViewFilters_Incomplete",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.Incomplete(context: context),
                        _checked: view.Incomplete == true,
                        labelPositionIsRight: true)
                    .FieldCheckBox(
                        controlId: "ViewFilters_Own",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.Own(context: context),
                        _checked: view.Own == true,
                        labelPositionIsRight: true)
                    .FieldCheckBox(
                        controlId: "ViewFilters_NearCompletionTime",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.NearCompletionTime(context: context),
                        _checked: view.NearCompletionTime == true,
                        labelPositionIsRight: true)
                    .FieldCheckBox(
                        controlId: "ViewFilters_Delay",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.Delay(context: context),
                        _checked: view.Delay == true,
                        labelPositionIsRight: true)
                    .FieldCheckBox(
                        controlId: "ViewFilters_Overdue",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.Overdue(context: context),
                        _checked: view.Overdue == true,
                        labelPositionIsRight: true)
                    .FieldTextBox(
                        controlId: "ViewFilters_Search",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.Search(context: context),
                        text: view.Search)
                    .ViewColumnFilters(context: context, ss: ss, view: view))
                .Div(css: "both", action: () => hb
                    .FieldDropDown(
                        context: context,
                        controlId: "ViewFilterSelector",
                        fieldCss: "field-auto-thin",
                        controlCss: " always-send",
                        optionCollection: ss.ViewFilterOptions(context: context))
                    .Button(
                        controlId: "AddViewFilter",
                        controlCss: "button-icon",
                        text: Displays.Add(context: context),
                        onClick: "$p.send($(this));",
                        icon: "ui-icon-plus",
                        action: "SetSiteSettings",
                        method: "post")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder ViewColumnFilters(
            this HtmlBuilder hb, Context context, SiteSettings ss, View view)
        {
            view.ColumnFilterHash?.ForEach(data => hb
                .ViewFilter(
                    context: context,
                    ss: ss,
                    column: ss.GetColumn(
                        context: context,
                        columnName: data.Key),
                    value: data.Value));
            return hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder ViewFilter(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Column column,
            string value = null)
        {
            var labelTitle = ss.LabelTitle(column);
            var controlId = "ViewFilters__" + column.ColumnName;
            switch (column.TypeName.CsTypeSummary())
            {
                case Types.CsBool:
                    switch (column.CheckFilterControlType)
                    {
                        case ColumnUtilities.CheckFilterControlTypes.OnOnly:
                            return hb.FieldCheckBox(
                                controlId: controlId,
                                fieldCss: "field-auto-thin",
                                labelText: column.LabelText,
                                labelTitle: labelTitle,
                                _checked: value.ToBool());
                        case ColumnUtilities.CheckFilterControlTypes.OnAndOff:
                            return hb.FieldDropDown(
                                context: context,
                                controlId: controlId,
                                fieldCss: "field-auto-thin",
                                labelText: column.LabelText,
                                labelTitle: labelTitle,
                                optionCollection: ColumnUtilities
                                    .CheckFilterTypeOptions(context: context),
                                selectedValue: value,
                                addSelectedValue: false,
                                insertBlank: true);
                        default:
                            return hb;
                    }
                case Types.CsDateTime:
                    return hb.FieldDropDown(
                        context: context,
                        controlId: controlId,
                        fieldCss: "field-auto-thin",
                        controlCss: " auto-postback",
                        labelText: column.LabelText,
                        labelTitle: labelTitle,
                        optionCollection: column.DateFilterOptions(context: context),
                        selectedValue: value,
                        multiple: true,
                        addSelectedValue: false);
                case Types.CsNumeric:
                    return hb.FieldDropDown(
                        context: context,
                        controlId: controlId,
                        fieldCss: "field-auto-thin",
                        controlCss: " auto-postback",
                        labelText: column.LabelText,
                        labelTitle: labelTitle,
                        optionCollection: column.HasChoices()
                            ? column.EditChoices(context: context)
                            : column.NumFilterOptions(context: context),
                        selectedValue: value,
                        multiple: true,
                        addSelectedValue: false);
                case Types.CsString:
                    return column.HasChoices()
                        ? hb.FieldDropDown(
                            context: context,
                            controlId: controlId,
                            fieldCss: "field-auto-thin",
                            controlCss: " auto-postback",
                            labelText: column.LabelText,
                            labelTitle: labelTitle,
                            optionCollection: column.EditChoices(context: context),
                            selectedValue: value,
                            multiple: true,
                            addSelectedValue: false)
                        : hb.FieldTextBox(
                            controlId: controlId,
                            fieldCss: "field-auto-thin",
                            labelText: column.LabelText,
                            labelTitle: labelTitle,
                            text: value);
                default:
                    return hb;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ViewSortersTab(
            this HtmlBuilder hb, Context context, SiteSettings ss, View view)
        {
            return hb.FieldSet(id: "ViewSortersTab", action: () => hb
                .FieldBasket(
                    controlId: "ViewSorters",
                    fieldCss: "field-wide",
                    controlCss: "control-basket cf",
                    listItemCollection: view.ColumnSorterHash?.ToDictionary(
                        o => $"{o.Key}&{o.Value}",
                        o => new ControlData(
                            $"{ss.LabelTitle(context: context, columnName: o.Key)}" +
                            $"({DisplayOrder(context: context, type: o)})")),
                    labelAction: () => hb
                        .Text(text: Displays.Sorters(context: context)))
                .FieldDropDown(
                    context: context,
                    controlId: "ViewSorterSelector",
                    fieldCss: "field-auto-thin",
                    controlCss: " always-send",
                    optionCollection: ss.ViewSorterOptions(context: context))
                .FieldDropDown(
                    context: context,
                    controlId: "ViewSorterOrderTypes",
                    fieldCss: "field-auto-thin",
                    controlCss: " always-send",
                    optionCollection: new Dictionary<string, string>
                    {
                        { "asc", Displays.OrderAsc(context: context) },
                        { "desc", Displays.OrderDesc(context: context) }
                    })
                .Button(
                    controlId: "AddViewSorter",
                    controlCss: "button-icon",
                    text: Displays.Add(context: context),
                    icon: "ui-icon-plus"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string DisplayOrder(
            Context context, KeyValuePair<string, SqlOrderBy.Types> type)
        {
            return Displays.Get(
                context: context,
                id: "Order" + type.Value.ToString().ToUpperFirstChar());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ViewCalendarTab(
            this HtmlBuilder hb, Context context, SiteSettings ss, View view, bool _using)
        {
            return _using
                ? hb.FieldSet(id: "ViewCalendarTab", action: () => hb
                    .FieldDropDown(
                        context: context,
                        controlId: "CalendarFromTo",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.Column(context: context),
                        optionCollection: ss.CalendarColumnOptions(context: context),
                        selectedValue: view.GetCalendarFromTo(ss)))
                : hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ViewCrosstabTab(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view,
            bool _using)
        {
            return _using
                ? hb.FieldSet(id: "ViewCrosstabTab", action: () => hb
                    .FieldDropDown(
                        context: context,
                        controlId: "CrosstabGroupByX",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.GroupByX(context: context),
                        optionCollection: ss.CrosstabGroupByXOptions(context: context),
                        selectedValue: view.GetCrosstabGroupByX(context: context, ss: ss))
                    .FieldDropDown(
                        context: context,
                        controlId: "CrosstabGroupByY",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.GroupByY(context: context),
                        optionCollection: ss.CrosstabGroupByYOptions(context: context),
                        selectedValue: view.GetCrosstabGroupByY(context: context, ss: ss))
                    .FieldDropDown(
                        context: context,
                        controlId: "CrosstabColumns",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.NumericColumn(context: context),
                        optionCollection: ss.CrosstabColumnsOptions(),
                        selectedValue: view.CrosstabColumns,
                        multiple: true)
                    .FieldDropDown(
                        context: context,
                        controlId: "CrosstabAggregateType",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.AggregationType(context: context),
                        optionCollection: ss.CrosstabAggregationTypeOptions(context: context),
                        selectedValue: view.GetCrosstabAggregateType(ss))
                    .FieldDropDown(
                        context: context,
                        controlId: "CrosstabValue",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.AggregationTarget(context: context),
                        optionCollection: ss.CrosstabColumnsOptions(),
                        selectedValue: view.GetCrosstabValue(ss))
                    .FieldDropDown(
                        context: context,
                        controlId: "CrosstabTimePeriod",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.Period(context: context),
                        optionCollection: ss.CrosstabTimePeriodOptions(context: context),
                        selectedValue: view.GetCrosstabTimePeriod(ss)))
                : hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ViewGanttTab(
            this HtmlBuilder hb, Context context, SiteSettings ss, View view, bool _using)
        {
            return _using
                ? hb.FieldSet(id: "ViewGanttTab", action: () => hb
                    .FieldDropDown(
                        context: context,
                        controlId: "GanttGroupBy",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.GroupBy(context: context),
                        optionCollection: ss.GanttGroupByOptions(),
                        selectedValue: view.GetGanttGroupBy(),
                        insertBlank: true)
                    .FieldDropDown(
                        context: context,
                        controlId: "GanttSortBy",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.SortBy(context: context),
                        optionCollection: ss.GanttSortByOptions(context: context),
                        selectedValue: view.GetGanttSortBy(),
                        insertBlank: true))
                : hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ViewTimeSeriesTab(
            this HtmlBuilder hb, Context context, SiteSettings ss, View view, bool _using)
        {
            return _using
                ? hb.FieldSet(id: "ViewTimeSeriesTab", action: () => hb
                    .FieldDropDown(
                        context: context,
                        controlId: "TimeSeriesGroupBy",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.GroupBy(context: context),
                        optionCollection: ss.TimeSeriesGroupByOptions(),
                        selectedValue: view.TimeSeriesGroupBy)
                    .FieldDropDown(
                        context: context,
                        controlId: "TimeSeriesAggregateType",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.AggregationType(context: context),
                        optionCollection: ss.TimeSeriesAggregationTypeOptions(context: context),
                        selectedValue: view.TimeSeriesAggregateType)
                    .FieldDropDown(
                        context: context,
                        controlId: "TimeSeriesValue",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.AggregationTarget(context: context),
                        optionCollection: ss.TimeSeriesValueOptions(),
                        selectedValue: view.TimeSeriesValue))
                : hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ViewKambanTab(
            this HtmlBuilder hb, Context context, SiteSettings ss, View view, bool _using)
        {
            return _using
                ? hb.FieldSet(id: "ViewKambanTab", action: () => hb
                    .FieldDropDown(
                        context: context,
                        controlId: "KambanGroupByX",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.GroupByX(context: context),
                        optionCollection: ss.KambanGroupByOptions(context: context),
                        selectedValue: view.KambanGroupByX)
                    .FieldDropDown(
                        context: context,
                        controlId: "KambanGroupByY",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.GroupByY(context: context),
                        optionCollection: ss.KambanGroupByOptions(context: context),
                        selectedValue: view.KambanGroupByY,
                        insertBlank: true)
                    .FieldDropDown(
                        context: context,
                        controlId: "KambanAggregateType",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.AggregationType(context: context),
                        optionCollection: ss.KambanAggregationTypeOptions(context: context),
                        selectedValue: view.KambanAggregateType)
                    .FieldDropDown(
                        context: context,
                        controlId: "KambanValue",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.AggregationTarget(context: context),
                        optionCollection: ss.KambanValueOptions(),
                        selectedValue: view.KambanValue)
                    .FieldDropDown(
                        context: context,
                        controlId: "KambanColumns",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.MaxColumns(context: context),
                        optionCollection: Enumerable.Range(
                            Parameters.General.KambanMinColumns,
                            Parameters.General.KambanMaxColumns)
                                .ToDictionary(o => o.ToString(), o => o.ToString()),
                        selectedValue: view.KambanColumns?.ToString())
                    .FieldCheckBox(
                        controlId: "KambanAggregationView",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.AggregationView(context: context),
                        _checked: view.KambanAggregationView == true,
                        labelPositionIsRight: true))
                : hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ResponseCollection ViewResponses(
            this ResponseCollection res, SiteSettings ss, IEnumerable<int> selected = null)
        {
            return res
                .Html("#Views", new HtmlBuilder().SelectableItems(
                    listItemCollection: ss.ViewSelectableOptions(),
                    selectedValueTextCollection: selected?.Select(o => o.ToString())))
                .SetData("#Views");
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder NotificationsSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            if (context.ContractSettings.Notice == false) return hb;
            return hb.FieldSet(id: "NotificationsSettingsEditor", action: () => hb
                .Div(css: "command-left", action: () => hb
                    .Button(
                        controlId: "MoveUpNotifications",
                        controlCss: "button-icon",
                        text: Displays.MoveUp(context: context),
                        onClick: "$p.setAndSend('#EditNotification', $(this));",
                        icon: "ui-icon-circle-triangle-n",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "MoveDownNotifications",
                        controlCss: "button-icon",
                        text: Displays.MoveDown(context: context),
                        onClick: "$p.setAndSend('#EditNotification', $(this));",
                        icon: "ui-icon-circle-triangle-s",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "NewNotification",
                        text: Displays.New(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.openNotificationDialog($(this));",
                        icon: "ui-icon-gear",
                        action: "SetSiteSettings",
                        method: "put")
                    .Button(
                        controlId: "DeleteNotifications",
                        text: Displays.Delete(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.setAndSend('#EditNotification', $(this));",
                        icon: "ui-icon-trash",
                        action: "SetSiteSettings",
                        method: "delete",
                        confirm: Displays.ConfirmDelete(context: context)))
                .EditNotification(context: context, ss: ss));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditNotification(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            var selected = Forms.Data("EditNotification").Deserialize<IEnumerable<int>>();
            return hb.Table(
                id: "EditNotification",
                css: "grid",
                attributes: new HtmlAttributes()
                    .DataName("NotificationId")
                    .DataFunc("openNotificationDialog")
                    .DataAction("SetSiteSettings")
                    .DataMethod("post"),
                action: () => hb
                    .EditNotificationHeader(
                        context: context,
                        ss: ss,
                        selected: selected)
                    .EditNotificationBody(
                        context: context,
                        ss: ss,
                        selected: selected));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditNotificationHeader(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            IEnumerable<int> selected)
        {
            return hb.THead(action: () => hb
                .Tr(css: "ui-widget-header", action: () => hb
                    .Th(action: () => hb
                        .CheckBox(
                            controlCss: "select-all",
                            _checked: ss.Summaries?.All(o =>
                                selected?.Contains(o.Id) == true) == true))
                    .Th(action: () => hb
                        .Text(text: Displays.Id(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.NotificationType(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Prefix(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Address(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Notifications(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.BeforeCondition(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Expression(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.AfterCondition(context: context)))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditNotificationBody(
            this HtmlBuilder hb, Context context, SiteSettings ss, IEnumerable<int> selected)
        {
            return hb.TBody(action: () => ss
                .Notifications?.ForEach(notification =>
                {
                    var beforeCondition = ss.Views?.Get(notification.BeforeCondition);
                    var afterCondition = ss.Views?.Get(notification.AfterCondition);
                    hb.Tr(
                        css: "grid-row",
                        attributes: new HtmlAttributes()
                            .DataId(notification.Id.ToString()),
                        action: () => hb
                            .Td(action: () => hb
                                .CheckBox(
                                    controlCss: "select",
                                    _checked: selected?
                                        .Contains(notification.Id) == true))
                            .Td(action: () => hb
                                .Text(text: notification.Id.ToString()))
                            .Td(action: () => hb
                                .Text(text: Displays.Get(
                                    context: context,
                                    id: notification.Type.ToString())))
                            .Td(action: () => hb
                                .Text(text: notification.Prefix))
                            .Td(action: () => hb
                                .Text(text: notification.Address))
                            .Td(action: () => hb
                                .Text(text: notification.MonitorChangesColumns?
                                    .Select(columnName => ss.GetColumn(
                                        context: context,
                                        columnName: columnName).LabelText)
                                    .Join(", ")))
                            .Td(action: () => hb
                                .Text(text: beforeCondition?.Name))
                            .Td(action: () => hb
                                .Text(text: beforeCondition != null && afterCondition != null
                                    ? Displays.Get(
                                        context: context,
                                        id: notification.Expression.ToString())
                                    : null))
                            .Td(action: () => hb
                                .Text(text: afterCondition?.Name)));
                }));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder NotificationDialog(
            Context context, SiteSettings ss, string controlId, Notification notification)
        {
            var hb = new HtmlBuilder();
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("NotificationForm")
                    .Action(Locations.ItemAction(ss.SiteId)),
                action: () => hb
                    .FieldText(
                        controlId: "NotificationId",
                        controlCss: " always-send",
                        labelText: Displays.Id(context: context),
                        text: notification.Id.ToString(),
                        _using: controlId == "EditNotification")
                    .FieldDropDown(
                        context: context,
                        controlId: "NotificationType",
                        controlCss: " always-send",
                        labelText: Displays.NotificationType(context: context),
                        optionCollection: NotificationUtilities.Types(context: context),
                        selectedValue: notification.Type.ToInt().ToString())
                    .FieldTextBox(
                        controlId: "NotificationPrefix",
                        controlCss: " always-send",
                        labelText: Displays.Prefix(context: context),
                        text: notification.Prefix)
                    .FieldTextBox(
                        controlId: "NotificationAddress",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.Address(context: context),
                        text: notification.Address,
                        validateRequired: true)
                    .FieldTextBox(
                        fieldId: "NotificationTokenField",
                        controlId: "NotificationToken",
                        fieldCss: "field-wide" + (!NotificationUtilities.RequireToken(notification)
                            ? " hidden"
                            : string.Empty),
                        controlCss: " always-send",
                        labelText: Displays.Token(context: context),
                        text: notification.Token)
                    .Hidden(
                        controlId: "NotificationTokenEnableList",
                        value: NotificationUtilities.Tokens())
                    .FieldCheckBox(
                        fieldId: "NotificationIsGroupField",
                        controlId: "NotificationIsGroup",
                        fieldCss: "field-wide" + (NotificationUtilities.RequireGroupCheck(notification) ? "" : " hidden"),
                        controlCss: " always-send",
                        labelText: Displays.LineGroup(context: context),
                        _checked: notification.IsGroup == true)
                    .Hidden(
                        controlId: "NotificationIsGroupEnableList",
                        value: NotificationUtilities.GroupChecks())
                    .Div(_using: ss.Views?.Any() == true, action: () => hb
                        .FieldDropDown(
                            context: context,
                            controlId: "BeforeCondition",
                            controlCss: " always-send",
                            labelText: Displays.BeforeCondition(context: context),
                            optionCollection: ss.ViewSelectableOptions(),
                            selectedValue: notification.BeforeCondition.ToString(),
                            insertBlank: true)
                        .FieldDropDown(
                            context: context,
                            controlId: "Expression",
                            controlCss: " always-send",
                            labelText: Displays.Expression(context: context),
                            optionCollection: new Dictionary<string, string>
                            {
                                {
                                    Notification.Expressions.Or.ToInt().ToString(),
                                    Displays.Or(context: context)
                                },
                                {
                                    Notification.Expressions.And.ToInt().ToString(),
                                    Displays.And(context: context)
                                }
                            },
                            selectedValue: notification.Expression.ToInt().ToString())
                        .FieldDropDown(
                            context: context,
                            controlId: "AfterCondition",
                            controlCss: " always-send",
                            labelText: Displays.AfterCondition(context: context),
                            optionCollection: ss.ViewSelectableOptions(),
                            selectedValue: notification.AfterCondition.ToString(),
                            insertBlank: true))
                    .FieldSet(
                        css: " enclosed",
                        legendText: Displays.MonitorChangesColumns(context: context),
                        action: () => hb
                            .FieldSelectable(
                                controlId: "MonitorChangesColumns",
                                fieldCss: "field-vertical",
                                controlContainerCss: "container-selectable",
                                controlWrapperCss: " h200",
                                controlCss: " always-send send-all",
                                labelText: Displays.CurrentSettings(context: context),
                                listItemCollection: ss
                                    .MonitorChangesSelectableOptions(
                                        context: context,
                                        monitorChangesColumns: notification.MonitorChangesColumns),
                                commandOptionPositionIsTop: true,
                                commandOptionAction: () => hb
                                    .Div(css: "command-center", action: () => hb
                                        .Button(
                                            controlId: "MoveUpMonitorChangesColumns",
                                            text: Displays.MoveUp(context: context),
                                            controlCss: "button-icon",
                                            onClick: "$p.moveColumns($(this),'MonitorChanges');",
                                            icon: "ui-icon-circle-triangle-n")
                                        .Button(
                                            controlId: "MoveDownMonitorChangesColumns",
                                            text: Displays.MoveDown(context: context),
                                            controlCss: "button-icon",
                                            onClick: "$p.moveColumns($(this),'MonitorChanges');",
                                            icon: "ui-icon-circle-triangle-s")
                                        .Button(
                                            controlId: "ToDisableMonitorChangesColumns",
                                            text: Displays.ToDisable(context: context),
                                            controlCss: "button-icon",
                                            onClick: "$p.moveColumns($(this),'MonitorChanges');",
                                            icon: "ui-icon-circle-triangle-e")))
                            .FieldSelectable(
                                controlId: "MonitorChangesSourceColumns",
                                fieldCss: "field-vertical",
                                controlContainerCss: "container-selectable",
                                controlWrapperCss: " h200",
                                labelText: Displays.OptionList(context: context),
                                listItemCollection: ss
                                    .MonitorChangesSelectableOptions(
                                        context: context,
                                        monitorChangesColumns: notification.MonitorChangesColumns,
                                        enabled: false),
                                commandOptionPositionIsTop: true,
                                commandOptionAction: () => hb
                                    .Div(css: "command-center", action: () => hb
                                        .Button(
                                            controlId: "ToEnableMonitorChangesColumns",
                                            text: Displays.ToEnable(context: context),
                                            controlCss: "button-icon",
                                            onClick: "$p.moveColumns($(this),'MonitorChanges');",
                                            icon: "ui-icon-circle-triangle-w"))))
                    .P(css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "AddNotification",
                            text: Displays.Add(context: context),
                            controlCss: "button-icon validate",
                            icon: "ui-icon-disk",
                            onClick: "$p.setNotification($(this));",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "NewNotification")
                        .Button(
                            controlId: "UpdateNotification",
                            text: Displays.Change(context: context),
                            controlCss: "button-icon validate",
                            onClick: "$p.setNotification($(this));",
                            icon: "ui-icon-disk",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "EditNotification")
                        .Button(
                            text: Displays.Cancel(context: context),
                            controlCss: "button-icon",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder RemindersSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            if (context.ContractSettings.Remind == false) return hb;
            return hb.FieldSet(id: "RemindersSettingsEditor", action: () => hb
                .Div(css: "command-left", action: () => hb
                    .Button(
                        controlId: "MoveUpReminders",
                        controlCss: "button-icon",
                        text: Displays.MoveUp(context: context),
                        onClick: "$p.setAndSend('#EditReminder', $(this));",
                        icon: "ui-icon-circle-triangle-n",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "MoveDownReminders",
                        controlCss: "button-icon",
                        text: Displays.MoveDown(context: context),
                        onClick: "$p.setAndSend('#EditReminder', $(this));",
                        icon: "ui-icon-circle-triangle-s",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "NewReminder",
                        text: Displays.New(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.openReminderDialog($(this));",
                        icon: "ui-icon-gear",
                        action: "SetSiteSettings",
                        method: "put")
                    .Button(
                        controlId: "DeleteReminders",
                        text: Displays.Delete(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.setAndSend('#EditReminder', $(this));",
                        icon: "ui-icon-trash",
                        action: "SetSiteSettings",
                        method: "delete",
                        confirm: Displays.ConfirmDelete(context: context))
                    .Button(
                        controlId: "TestReminders",
                        text: Displays.Test(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.setAndSend('#EditReminder', $(this));",
                        icon: "ui-icon-mail-closed",
                        action: "SetSiteSettings",
                        method: "post",
                        confirm: Displays.ConfirmSendMail(context: context)))
                .EditReminder(context: context, ss: ss));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditReminder(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            var selected = Forms.Data("EditReminder").Deserialize<IEnumerable<int>>();
            return hb.Table(
                id: "EditReminder",
                css: "grid",
                attributes: new HtmlAttributes()
                    .DataName("ReminderId")
                    .DataFunc("openReminderDialog")
                    .DataAction("SetSiteSettings")
                    .DataMethod("post"),
                action: () => hb
                    .EditReminderHeader(
                        context: context,
                        ss: ss,
                        selected: selected)
                    .EditReminderBody(
                        context: context,
                        ss: ss,
                        selected: selected));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditReminderHeader(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            IEnumerable<int> selected)
        {
            return hb.THead(action: () => hb
                .Tr(css: "ui-widget-header", action: () => hb
                    .Th(action: () => hb
                        .CheckBox(
                            controlCss: "select-all",
                            _checked: ss.Summaries?.All(o =>
                                selected?.Contains(o.Id) == true) == true))
                    .Th(action: () => hb
                        .Text(text: Displays.Id(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Subject(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Body(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Row(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.From(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.To(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Column(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.StartDateTime(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.PeriodType(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Range(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.SendCompletedInPast(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.NotSendIfNotApplicable(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Condition(context: context)))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditReminderBody(
            this HtmlBuilder hb, Context context, SiteSettings ss, IEnumerable<int> selected)
        {
            return hb.TBody(action: () => ss
                .Reminders?.ForEach(reminder =>
                {
                    var condition = ss.Views?.Get(reminder.Condition);
                    hb.Tr(
                        css: "grid-row",
                        attributes: new HtmlAttributes()
                            .DataId(reminder.Id.ToString()),
                        action: () => hb
                            .Td(action: () => hb
                                .CheckBox(
                                    controlCss: "select",
                                    _checked: selected?
                                        .Contains(reminder.Id) == true))
                            .Td(action: () => hb
                                .Text(text: reminder.Id.ToString()))
                            .Td(action: () => hb
                                .Text(text: reminder.Subject))
                            .Td(action: () => hb
                                .Text(text: reminder.Body))
                            .Td(action: () => hb
                                .Text(text: reminder.DisplayLine(ss)))
                            .Td(action: () => hb
                                .Text(text: reminder.From))
                            .Td(action: () => hb
                                .Text(text: reminder.To))
                            .Td(action: () => hb
                                .Text(text: ss.GetColumn(
                                    context: context,
                                    columnName: reminder.Column)?.LabelText))
                            .Td(action: () => hb
                                .Text(text: reminder.StartDateTime
                                    .ToString(context.CultureInfo())))
                            .Td(action: () => hb
                                .Text(text: Displays.Get(
                                    context: context,
                                    id: reminder.Type.ToString())))
                            .Td(action: () => hb
                                .Text(text: reminder.Range.ToString()))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: reminder.SendCompletedInPast == true))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: reminder.NotSendIfNotApplicable == true))
                            .Td(action: () => hb
                                .Text(text: condition?.Name)));
                }));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder ReminderDialog(
            Context context, SiteSettings ss, string controlId, Reminder reminder)
        {
            var hb = new HtmlBuilder();
            var conditions = ss.ViewSelectableOptions();
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("ReminderForm")
                    .Action(Locations.ItemAction(ss.SiteId)),
                action: () => hb
                    .FieldText(
                        controlId: "ReminderId",
                        controlCss: " always-send",
                        labelText: Displays.Id(context: context),
                        text: reminder.Id.ToString(),
                        _using: controlId == "EditReminder")
                    .FieldTextBox(
                        controlId: "ReminderSubject",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.Subject(context: context),
                        text: reminder.Subject,
                        validateRequired: true)
                    .FieldTextBox(
                        textType: HtmlTypes.TextTypes.MultiLine,
                        controlId: "ReminderBody",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.Body(context: context),
                        text: reminder.Body,
                        validateRequired: true)
                    .FieldTextBox(
                        controlId: "ReminderLine",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.Row(context: context),
                        text: reminder.DisplayLine(ss),
                        validateRequired: true)
                    .FieldTextBox(
                        controlId: "ReminderFrom",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.From(context: context),
                        text: reminder.From,
                        validateRequired: true)
                    .FieldTextBox(
                        controlId: "ReminderTo",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.To(context: context),
                        text: reminder.To,
                        validateRequired: true)
                    .FieldDropDown(
                        context: context,
                        controlId: "ReminderColumn",
                        controlCss: " always-send",
                        labelText: Displays.Column(context: context),
                        optionCollection: ss.ReminderColumnOptions(),
                        selectedValue: reminder.GetColumn(ss))
                    .FieldTextBox(
                        textType: HtmlTypes.TextTypes.DateTime,
                        controlId: "ReminderStartDateTime",
                        controlCss: " always-send",
                        labelText: Displays.StartDateTime(context: context),
                        text: reminder.StartDateTime.InRange()
                            ? reminder.StartDateTime.ToString(Displays.Get(
                                context: context,
                                id: "YmdhmFormat"))
                            : null,
                        timepiker: true,
                        validateRequired: true,
                        validateDate: true)
                    .FieldDropDown(
                        context: context,
                        controlId: "ReminderType",
                        controlCss: " always-send",
                        labelText: Displays.PeriodType(context: context),
                        optionCollection: new Dictionary<string, string>
                        {
                            {
                                Times.RepeatTypes.Daily.ToInt().ToString(),
                                Displays.Daily(context: context)
                            },
                            {
                                Times.RepeatTypes.Weekly.ToInt().ToString(),
                                Displays.Weekly(context: context)
                            },
                            {
                                Times.RepeatTypes.NumberWeekly.ToInt().ToString(),
                                Displays.NumberWeekly(context: context)
                            },
                            {
                                Times.RepeatTypes.Monthly.ToInt().ToString(),
                                Displays.Monthly(context: context)
                            },
                            {
                                Times.RepeatTypes.EndOfMonth.ToInt().ToString(),
                                Displays.EndOfMonth(context: context)
                            },
                            {
                                Times.RepeatTypes.Yearly.ToInt().ToString(),
                                Displays.Yearly(context: context)
                            }
                        },
                        selectedValue: reminder.Type.ToInt().ToString())
                    .FieldSpinner(
                        controlId: "ReminderRange",
                        controlCss: " always-send",
                        labelText: Displays.Range(context: context),
                        value: reminder.Range,
                        min: Parameters.Reminder.MinRange,
                        max: Parameters.Reminder.MaxRange,
                        step: 1,
                        width: 25,
                        unit: Displays.Day(context: context))
                    .FieldCheckBox(
                        controlId: "ReminderSendCompletedInPast",
                        controlCss: " always-send",
                        labelText: Displays.SendCompletedInPast(context: context),
                        _checked: reminder.SendCompletedInPast == true)
                    .FieldCheckBox(
                        controlId: "NotSendIfNotApplicable",
                        controlCss: " always-send",
                        labelText: Displays.NotSendIfNotApplicable(context: context),
                        _checked: reminder.NotSendIfNotApplicable == true)
                    .FieldDropDown(
                        context: context,
                        controlId: "ReminderCondition",
                        controlCss: " always-send",
                        labelText: Displays.Condition(context: context),
                        optionCollection: conditions,
                        selectedValue: reminder.Condition.ToString(),
                        insertBlank: true,
                        _using: conditions?.Any() == true)
                    .P(css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "AddReminder",
                            text: Displays.Add(context: context),
                            controlCss: "button-icon validate",
                            icon: "ui-icon-disk",
                            onClick: "$p.setReminder($(this));",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "NewReminder")
                        .Button(
                            controlId: "UpdateReminder",
                            text: Displays.Change(context: context),
                            controlCss: "button-icon validate",
                            onClick: "$p.setReminder($(this));",
                            icon: "ui-icon-disk",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "EditReminder")
                        .Button(
                            text: Displays.Cancel(context: context),
                            controlCss: "button-icon",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ExportsSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            if (context.ContractSettings.Export == false) return hb;
            return hb.FieldSet(id: "ExportsSettingsEditor", action: () => hb
                .Div(css: "command-left", action: () => hb
                    .Button(
                        controlId: "MoveUpExports",
                        controlCss: "button-icon",
                        text: Displays.MoveUp(context: context),
                        onClick: "$p.setAndSend('#EditExport', $(this));",
                        icon: "ui-icon-circle-triangle-n",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "MoveDownExports",
                        controlCss: "button-icon",
                        text: Displays.MoveDown(context: context),
                        onClick: "$p.setAndSend('#EditExport', $(this));",
                        icon: "ui-icon-circle-triangle-s",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "NewExport",
                        text: Displays.New(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.openExportDialog($(this));",
                        icon: "ui-icon-gear",
                        action: "SetSiteSettings",
                        method: "put")
                    .Button(
                        controlId: "DeleteExports",
                        text: Displays.Delete(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.setAndSend('#EditExport', $(this));",
                        icon: "ui-icon-trash",
                        action: "SetSiteSettings",
                        method: "delete",
                        confirm: Displays.ConfirmDelete(context: context)))
                .EditExport(context: context, ss: ss));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditExport(this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            ss.SetExports(context: context);
            var selected = Forms.Data("EditExport").Deserialize<IEnumerable<int>>();
            return hb.Table(
                id: "EditExport",
                css: "grid",
                attributes: new HtmlAttributes()
                    .DataName("ExportId")
                    .DataFunc("openExportDialog")
                    .DataAction("SetSiteSettings")
                    .DataMethod("post"),
                action: () => hb
                    .EditExportHeader(
                        context: context,
                        ss: ss,
                        selected: selected)
                    .EditExportBody(
                        ss: ss,
                        selected: selected,
                        tables: ss.ExportJoinOptions(context: context)));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditExportHeader(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            IEnumerable<int> selected)
        {
            return hb.THead(action: () => hb
                .Tr(css: "ui-widget-header", action: () => hb
                    .Th(action: () => hb
                        .CheckBox(
                            controlCss: "select-all",
                            _checked: ss.Summaries?.All(o =>
                                selected?.Contains(o.Id) == true) == true))
                    .Th(action: () => hb
                        .Text(text: Displays.Id(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Name(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Tables(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.OutputHeader(context: context)))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditExportBody(
            this HtmlBuilder hb,
            SiteSettings ss,
            IEnumerable<int> selected,
            Dictionary<string, string> tables)
        {
            return hb.TBody(action: () => ss.Exports?
                .ForEach(export => hb
                    .Tr(
                        css: "grid-row",
                        attributes: new HtmlAttributes()
                            .DataId(export.Id.ToString()),
                        action: () => hb
                            .Td(action: () => hb
                                .CheckBox(
                                    controlCss: "select",
                                    _checked: selected?
                                        .Contains(export.Id) == true))
                            .Td(action: () => hb
                                .Text(text: export.Id.ToString()))
                            .Td(action: () => hb
                                .Text(text: export.Name))
                            .Td(action: () => hb
                                .Text(text: tables.Get(export.Join.ToJson())))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: export.Header == true)))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder ExportDialog(
            Context context, SiteSettings ss, string controlId, Export export)
        {
            var hb = new HtmlBuilder();
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("ExportForm")
                    .Action(Locations.ItemAction(ss.SiteId)),
                action: () => hb
                    .FieldText(
                        controlId: "ExportId",
                        controlCss: " always-send",
                        labelText: Displays.Id(context: context),
                        text: export.Id.ToString(),
                        _using: controlId == "EditExport")
                    .FieldTextBox(
                        controlId: "ExportName",
                        controlCss: " always-send",
                        labelText: Displays.Name(context: context),
                        text: export.Name,
                        validateRequired: true)
                    .FieldCheckBox(
                        controlId: "ExportHeader",
                        controlCss: " always-send",
                        labelText: Displays.OutputHeader(context: context),
                        _checked: export.Header == true)
                    .FieldDropDown(
                        context: context,
                        controlId: "ExportJoin",
                        fieldCss: " field-wide",
                        controlCss: " auto-postback always-send",
                        labelText: Displays.Tables(context: context),
                        optionCollection: ss.ExportJoinOptions(context: context),
                        selectedValue: export.Join.ToJson(),
                        addSelectedValue: false,
                        action: "SetSiteSettings",
                        method: "post")
                    .FieldSet(
                        css: " enclosed",
                        legendText: Displays.ExportColumns(context: context),
                        action: () => hb
                            .FieldSelectable(
                                controlId: "ExportColumns",
                                fieldCss: "field-vertical",
                                controlContainerCss: "container-selectable",
                                controlWrapperCss: " h300",
                                controlCss: " always-send send-all",
                                labelText: Displays.CurrentSettings(context: context),
                                listItemCollection: ExportUtilities
                                    .CurrentColumnOptions(export.Columns),
                                commandOptionPositionIsTop: true,
                                commandOptionAction: () => hb
                                    .Div(css: "command-center", action: () => hb
                                        .Button(
                                            controlId: "MoveUpExportColumns",
                                            text: Displays.MoveUp(context: context),
                                            controlCss: "button-icon",
                                            onClick: "$p.moveColumns($(this),'Export',true);",
                                            icon: "ui-icon-circle-triangle-n")
                                        .Button(
                                            controlId: "MoveDownExportColumns",
                                            text: Displays.MoveDown(context: context),
                                            controlCss: "button-icon",
                                            onClick: "$p.moveColumns($(this),'Export',true);",
                                            icon: "ui-icon-circle-triangle-s")
                                        .Button(
                                            controlId: "OpenExportColumnsDialog",
                                            text: Displays.AdvancedSetting(context: context),
                                            controlCss: "button-icon",
                                            onClick: "$p.openExportColumnsDialog($(this));",
                                            icon: "ui-icon-circle-triangle-s",
                                            action: "SetSiteSettings",
                                            method: "post")
                                        .Button(
                                            controlId: "ToDisableExportColumns",
                                            text: Displays.ToDisable(context: context),
                                            controlCss: "button-icon",
                                            onClick: "$p.moveColumns($(this),'Export',true);",
                                            icon: "ui-icon-circle-triangle-e")))
                            .FieldSelectable(
                                controlId: "ExportSourceColumns",
                                fieldCss: "field-vertical",
                                controlContainerCss: "container-selectable",
                                controlWrapperCss: " h300",
                                labelText: Displays.OptionList(context: context),
                                listItemCollection: ExportUtilities
                                    .SourceColumnOptions(
                                        context: context,
                                        ss: ss,
                                        join: export.Join),
                                commandOptionPositionIsTop: true,
                                commandOptionAction: () => hb
                                    .Div(css: "command-left", action: () => hb
                                        .Button(
                                            controlId: "ToEnableExportColumns",
                                            text: Displays.ToEnable(context: context),
                                            controlCss: "button-icon",
                                            onClick: "$p.moveColumns($(this),'Export',true);",
                                            icon: "ui-icon-circle-triangle-w")
                                        .Span(css: "ui-icon ui-icon-search")
                                        .TextBox(
                                            controlId: "SearchExportColumns",
                                            controlCss: " auto-postback w100",
                                            placeholder: Displays.Search(context: context),
                                            action: "SetSiteSettings",
                                            method: "post"))))
                    .P(css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "AddExport",
                            text: Displays.Add(context: context),
                            controlCss: "button-icon validate",
                            icon: "ui-icon-disk",
                            onClick: "$p.setExport($(this));",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "NewExport")
                        .Button(
                            controlId: "UpdateExport",
                            text: Displays.Change(context: context),
                            controlCss: "button-icon validate",
                            onClick: "$p.setExport($(this));",
                            icon: "ui-icon-disk",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "EditExport")
                        .Button(
                            text: Displays.Cancel(context: context),
                            controlCss: "button-icon",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder ExportColumnsDialog(
            Context context, SiteSettings ss, string controlId, ExportColumn exportColumn)
        {
            var hb = new HtmlBuilder();
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("ExportColumnsForm")
                    .Action(Locations.ItemAction(ss.SiteId)),
                action: () => hb
                    .FieldText(
                        labelText: Displays.Column(context: context),
                        text: exportColumn.GetColumnLabelText())
                    .FieldTextBox(
                        controlId: "ExportColumnLabelText",
                        controlCss: " always-send",
                        labelText: Displays.DisplayName(context: context),
                        text: exportColumn.GetLabelText(),
                        validateRequired: true)
                    .FieldDropDown(
                        context: context,
                        controlId: "ExportColumnType",
                        controlCss: " always-send",
                        labelText: Displays.Output(context: context),
                        optionCollection: new Dictionary<string, string>
                        {
                            {
                                ExportColumn.Types.Text.ToInt().ToString(),
                                Displays.DisplayName(context: context)
                            },
                            {
                                ExportColumn.Types.TextMini.ToInt().ToString(),
                                Displays.ShortDisplayName(context: context)
                            },
                            {
                                ExportColumn.Types.Value.ToInt().ToString(),
                                Displays.Value(context: context)
                            }
                        },
                        selectedValue: exportColumn.GetType())
                    .FieldDropDown(
                        context: context,
                        controlId: "ExportFormat",
                        controlCss: " always-send",
                        labelText: Displays.ExportFormat(context: context),
                        optionCollection: DateTimeOptions(context: context),
                        selectedValue: exportColumn.GetFormat(),
                        _using: exportColumn.Column.TypeName == "datetime")
                    .Hidden(
                        controlId: "ExportColumnId",
                        css: " always-send",
                        value: exportColumn.Id.ToString())
                    .P(id: "ExportColumnsMessage", css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "UpdateExportColumn",
                            text: Displays.Change(context: context),
                            controlCss: "button-icon validate",
                            onClick: "$p.setExportColumn($(this));",
                            icon: "ui-icon-disk",
                            action: "SetSiteSettings",
                            method: "post")
                        .Button(
                            text: Displays.Cancel(context: context),
                            controlCss: "button-icon",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder CalendarSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return Def.ViewModeDefinitionCollection
                .Where(o => o.Name == "Calendar")
                .Any(o => o.ReferenceType == ss.ReferenceType)
                    ? hb.FieldSet(id: "CalendarSettingsEditor", action: () => hb
                        .FieldCheckBox(
                            controlId: "EnableCalendar",
                            fieldCss: "field-auto-thin",
                            labelText: Displays.Enabled(context: context),
                            _checked: ss.EnableCalendar == true))
                    : hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder CrosstabSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return Def.ViewModeDefinitionCollection
                .Where(o => o.Name == "Crosstab")
                .Any(o => o.ReferenceType == ss.ReferenceType)
                    ? hb.FieldSet(id: "CrosstabSettingsEditor", action: () => hb
                        .FieldCheckBox(
                            controlId: "EnableCrosstab",
                            fieldCss: "field-auto-thin",
                            labelText: Displays.Enabled(context: context),
                            _checked: ss.EnableCrosstab == true))
                    : hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder GanttSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return Def.ViewModeDefinitionCollection
                .Where(o => o.Name == "Gantt")
                .Any(o => o.ReferenceType == ss.ReferenceType)
                    ? hb.FieldSet(id: "GanttSettingsEditor", action: () => hb
                        .FieldCheckBox(
                            controlId: "EnableGantt",
                            fieldCss: "field-auto-thin",
                            labelText: Displays.Enabled(context: context),
                            _checked: ss.EnableGantt == true)
                        .FieldCheckBox(
                            controlId: "ShowGanttProgressRate",
                            fieldCss: "field-auto-thin",
                            labelText: Displays.ShowProgressRate(context: context),
                            _checked: ss.ShowGanttProgressRate == true))
                    : hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder BurnDownSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return Def.ViewModeDefinitionCollection
                .Where(o => o.Name == "BurnDown")
                .Any(o => o.ReferenceType == ss.ReferenceType)
                    ? hb.FieldSet(id: "BurnDownSettingsEditor", action: () => hb
                        .FieldCheckBox(
                            controlId: "EnableBurnDown",
                            fieldCss: "field-auto-thin",
                            labelText: Displays.Enabled(context: context),
                            _checked: ss.EnableBurnDown == true))
                    : hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder TimeSeriesSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return Def.ViewModeDefinitionCollection
                .Where(o => o.Name == "TimeSeries")
                .Any(o => o.ReferenceType == ss.ReferenceType)
                    ? hb.FieldSet(id: "TimeSeriesSettingsEditor", action: () => hb
                        .FieldCheckBox(
                            controlId: "EnableTimeSeries",
                            fieldCss: "field-auto-thin",
                            labelText: Displays.Enabled(context: context),
                            _checked: ss.EnableTimeSeries == true))
                    : hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder KambanSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return Def.ViewModeDefinitionCollection
                .Where(o => o.Name == "Kamban")
                .Any(o => o.ReferenceType == ss.ReferenceType)
                    ? hb.FieldSet(id: "KambanSettingsEditor", action: () => hb
                        .FieldCheckBox(
                            controlId: "EnableKamban",
                            fieldCss: "field-auto-thin",
                            labelText: Displays.Enabled(context: context),
                            _checked: ss.EnableKamban == true))
                    : hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ImageLibSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            if (context.ContractSettings.Images() == false) return hb;
            return Def.ViewModeDefinitionCollection
                .Where(o => o.Name == "ImageLib")
                .Any(o => o.ReferenceType == ss.ReferenceType)
                    ? hb.FieldSet(id: "ImageLibSettingsEditor", action: () => hb
                        .FieldCheckBox(
                            controlId: "EnableImageLib",
                            fieldCss: "field-auto-thin",
                            labelText: Displays.Enabled(context: context),
                            _checked: ss.EnableImageLib == true)
                        .FieldSpinner(
                            controlId: "ImageLibPageSize",
                            fieldCss: "field-auto-thin",
                            labelText: Displays.NumberPerPage(context: context),
                            value: ss.ImageLibPageSize.ToDecimal(),
                            min: Parameters.General.ImageLibPageSizeMin,
                            max: Parameters.General.ImageLibPageSizeMax,
                            step: 1,
                            width: 25))
                    : hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SearchSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return hb.FieldSet(id: "SearchSettingsEditor", action: () => hb
                .FieldDropDown(
                        context: context,
                    controlId: "SearchType",
                    controlCss: " always-send",
                    labelText: Displays.SearchTypes(context: context),
                    optionCollection: new Dictionary<string, string>()
                    {
                        {
                            SiteSettings.SearchTypes.FullText.ToInt().ToString(),
                            Displays.FullText(context: context)
                        },
                        {
                            SiteSettings.SearchTypes.PartialMatch.ToInt().ToString(),
                            Displays.PartialMatch(context: context)
                        },
                        {
                            SiteSettings.SearchTypes.MatchInFrontOfTitle.ToInt().ToString(),
                            Displays.MatchInFrontOfTitle(context: context)
                        },
                        {
                            SiteSettings.SearchTypes.BroadMatchOfTitle.ToInt().ToString(),
                            Displays.BroadMatchOfTitle(context: context)
                        }
                    },
                    selectedValue: ss.SearchType.ToInt().ToString()));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder MailSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            if (context.ContractSettings.Mail == false) return hb;
            return hb.FieldSet(id: "MailSettingsEditor", action: () => hb
                .FieldTextBox(
                    textType: HtmlTypes.TextTypes.MultiLine,
                    controlId: "AddressBook",
                    fieldCss: "field-wide",
                    labelText: Displays.DefaultAddressBook(context: context),
                    text: ss.AddressBook.ToStr())
                .FieldSet(
                    css: " enclosed-thin",
                    legendText: Displays.DefaultDestinations(context: context),
                    action: () => hb
                        .FieldTextBox(
                            textType: HtmlTypes.TextTypes.MultiLine,
                            controlId: "MailToDefault",
                            fieldCss: "field-wide",
                            labelText: Displays.OutgoingMails_To(context: context),
                            text: ss.MailToDefault.ToStr())
                        .FieldTextBox(
                            textType: HtmlTypes.TextTypes.MultiLine,
                            controlId: "MailCcDefault",
                            fieldCss: "field-wide",
                            labelText: Displays.OutgoingMails_Cc(context: context),
                            text: ss.MailCcDefault.ToStr())
                        .FieldTextBox(
                            textType: HtmlTypes.TextTypes.MultiLine,
                            controlId: "MailBccDefault",
                            fieldCss: "field-wide",
                            labelText: Displays.OutgoingMails_Bcc(context: context),
                            text: ss.MailBccDefault.ToStr())));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SiteIntegrationEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return hb.FieldSet(id: "SiteIntegrationEditor", action: () => hb
                .FieldTextBox(
                    controlId: "IntegratedSites",
                    fieldCss: "field-wide",
                    labelText: Displays.SiteId(context: context),
                    text: ss.IntegratedSites?.Join()));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder StylesSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            if (context.ContractSettings.Style == false) return hb;
            return hb.FieldSet(id: "StylesSettingsEditor", action: () => hb
                .Div(css: "command-left", action: () => hb
                    .Button(
                        controlId: "MoveUpStyles",
                        controlCss: "button-icon",
                        text: Displays.MoveUp(context: context),
                        onClick: "$p.setAndSend('#EditStyle', $(this));",
                        icon: "ui-icon-circle-triangle-n",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "MoveDownStyles",
                        controlCss: "button-icon",
                        text: Displays.MoveDown(context: context),
                        onClick: "$p.setAndSend('#EditStyle', $(this));",
                        icon: "ui-icon-circle-triangle-s",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "NewStyle",
                        text: Displays.New(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.openStyleDialog($(this));",
                        icon: "ui-icon-gear",
                        action: "SetSiteSettings",
                        method: "put")
                    .Button(
                        controlId: "DeleteStyles",
                        text: Displays.Delete(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.setAndSend('#EditStyle', $(this));",
                        icon: "ui-icon-trash",
                        action: "SetSiteSettings",
                        method: "delete",
                        confirm: Displays.ConfirmDelete(context: context)))
                .EditStyle(
                    context: context,
                    ss: ss));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditStyle(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            var selected = Forms.Data("EditStyle").Deserialize<IEnumerable<int>>();
            return hb.Table(
                id: "EditStyle",
                css: "grid",
                attributes: new HtmlAttributes()
                    .DataName("StyleId")
                    .DataFunc("openStyleDialog")
                    .DataAction("SetSiteSettings")
                    .DataMethod("post"),
                action: () => hb
                    .EditStyleHeader(
                        context: context,
                        ss: ss,
                        selected: selected)
                    .EditStyleBody(ss: ss, selected: selected));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditStyleHeader(
            this HtmlBuilder hb, Context context, SiteSettings ss, IEnumerable<int> selected)
        {
            return hb.THead(action: () => hb
                .Tr(css: "ui-widget-header", action: () => hb
                    .Th(action: () => hb
                        .CheckBox(
                            controlCss: "select-all",
                            _checked: ss.Styles?.All(o =>
                                selected?.Contains(o.Id) == true) == true))
                    .Th(action: () => hb
                        .Text(text: Displays.Id(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Title(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.All(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.New(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Edit(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Index(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Calendar(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Crosstab(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Gantt(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.BurnDown(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.TimeSeries(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Kamban(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.ImageLib(context: context)))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditStyleBody(
            this HtmlBuilder hb, SiteSettings ss, IEnumerable<int> selected)
        {
            return hb.TBody(action: () => ss
                .Styles?.ForEach(style => hb
                    .Tr(
                        css: "grid-row",
                        attributes: new HtmlAttributes()
                            .DataId(style.Id.ToString()),
                        action: () => hb
                            .Td(action: () => hb
                                .CheckBox(
                                    controlCss: "select",
                                    _checked: selected?
                                        .Contains(style.Id) == true))
                            .Td(action: () => hb
                                .Text(text: style.Id.ToString()))
                            .Td(action: () => hb
                                .Text(text: style.Title))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: style.All == true))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: style.New == true))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: style.Edit == true))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: style.Index == true))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: style.Calendar == true))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: style.Crosstab == true))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: style.Gantt == true))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: style.BurnDown == true))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: style.TimeSeries == true))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: style.Kamban == true))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: style.ImageLib == true)))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder StyleDialog(
            Context context, SiteSettings ss, string controlId, Style style)
        {
            var hb = new HtmlBuilder();
            var conditions = ss.ViewSelectableOptions();
            var outputDestinationCss = " output-destination-style" +
                (style.All == true
                    ? " hidden"
                    : string.Empty);
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("StyleForm")
                    .Action(Locations.ItemAction(ss.SiteId)),
                action: () => hb
                    .FieldText(
                        controlId: "StyleId",
                        controlCss: " always-send",
                        labelText: Displays.Id(context: context),
                        text: style.Id.ToString(),
                        _using: controlId == "EditStyle")
                    .FieldTextBox(
                        controlId: "StyleTitle",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.Title(context: context),
                        text: style.Title,
                        validateRequired: true)
                    .FieldTextBox(
                        textType: HtmlTypes.TextTypes.MultiLine,
                        controlId: "StyleBody",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.Style(context: context),
                        text: style.Body)
                    .FieldSet(
                        css: " enclosed",
                        legendText: Displays.OutputDestination(context: context),
                        action: () => hb
                            .FieldCheckBox(
                                fieldId: "StyleAllField",
                                controlId: "StyleAll",
                                controlCss: " always-send",
                                labelText: Displays.All(context: context),
                                _checked: style.All == true)
                            .FieldCheckBox(
                                controlId: "StyleNew",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.New(context: context),
                                _checked: style.New == true)
                            .FieldCheckBox(
                                controlId: "StyleEdit",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.Edit(context: context),
                                _checked: style.Edit == true)
                            .FieldCheckBox(
                                controlId: "StyleIndex",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.Index(context: context),
                                _checked: style.Index == true)
                            .FieldCheckBox(
                                controlId: "StyleCalendar",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.Calendar(context: context),
                                _checked: style.Calendar == true)
                            .FieldCheckBox(
                                controlId: "StyleCrosstab",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.Crosstab(context: context),
                                _checked: style.Crosstab == true)
                            .FieldCheckBox(
                                controlId: "StyleGantt",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.Gantt(context: context),
                                _checked: style.Gantt == true)
                            .FieldCheckBox(
                                controlId: "StyleBurnDown",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.BurnDown(context: context),
                                _checked: style.BurnDown == true)
                            .FieldCheckBox(
                                controlId: "StyleTimeSeries",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.TimeSeries(context: context),
                                _checked: style.TimeSeries == true)
                            .FieldCheckBox(
                                controlId: "StyleKamban",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.Kamban(context: context),
                                _checked: style.Kamban == true)
                            .FieldCheckBox(
                                controlId: "StyleImageLib",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.ImageLib(context: context),
                                _checked: style.ImageLib == true))
                    .P(css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "AddStyle",
                            text: Displays.Add(context: context),
                            controlCss: "button-icon validate",
                            icon: "ui-icon-disk",
                            onClick: "$p.setStyle($(this));",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "NewStyle")
                        .Button(
                            controlId: "UpdateStyle",
                            text: Displays.Change(context: context),
                            controlCss: "button-icon validate",
                            onClick: "$p.setStyle($(this));",
                            icon: "ui-icon-disk",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "EditStyle")
                        .Button(
                            text: Displays.Cancel(context: context),
                            controlCss: "button-icon",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ScriptsSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            if (context.ContractSettings.Script == false) return hb;
            return hb.FieldSet(id: "ScriptsSettingsEditor", action: () => hb
                .Div(css: "command-left", action: () => hb
                    .Button(
                        controlId: "MoveUpScripts",
                        controlCss: "button-icon",
                        text: Displays.MoveUp(context: context),
                        onClick: "$p.setAndSend('#EditScript', $(this));",
                        icon: "ui-icon-circle-triangle-n",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "MoveDownScripts",
                        controlCss: "button-icon",
                        text: Displays.MoveDown(context: context),
                        onClick: "$p.setAndSend('#EditScript', $(this));",
                        icon: "ui-icon-circle-triangle-s",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "NewScript",
                        text: Displays.New(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.openScriptDialog($(this));",
                        icon: "ui-icon-gear",
                        action: "SetSiteSettings",
                        method: "put")
                    .Button(
                        controlId: "DeleteScripts",
                        text: Displays.Delete(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.setAndSend('#EditScript', $(this));",
                        icon: "ui-icon-trash",
                        action: "SetSiteSettings",
                        method: "delete",
                        confirm: Displays.ConfirmDelete(context: context)))
                .EditScript(
                    context: context,
                    ss: ss));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditScript(this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            var selected = Forms.Data("EditScript").Deserialize<IEnumerable<int>>();
            return hb.Table(
                id: "EditScript",
                css: "grid",
                attributes: new HtmlAttributes()
                    .DataName("ScriptId")
                    .DataFunc("openScriptDialog")
                    .DataAction("SetSiteSettings")
                    .DataMethod("post"),
                action: () => hb
                    .EditScriptHeader(
                        context: context,
                        ss: ss,
                        selected: selected)
                    .EditScriptBody(
                        ss: ss,
                        selected: selected));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditScriptHeader(
            this HtmlBuilder hb, Context context, SiteSettings ss, IEnumerable<int> selected)
        {
            return hb.THead(action: () => hb
                .Tr(css: "ui-widget-header", action: () => hb
                    .Th(action: () => hb
                        .CheckBox(
                            controlCss: "select-all",
                            _checked: ss.Scripts?.All(o =>
                                selected?.Contains(o.Id) == true) == true))
                    .Th(action: () => hb
                        .Text(text: Displays.Id(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Title(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.All(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.New(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Edit(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Index(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Calendar(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Crosstab(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Gantt(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.BurnDown(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.TimeSeries(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Kamban(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.ImageLib(context: context)))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditScriptBody(
            this HtmlBuilder hb, SiteSettings ss, IEnumerable<int> selected)
        {
            return hb.TBody(action: () => ss
                .Scripts?.ForEach(script => hb
                    .Tr(
                        css: "grid-row",
                        attributes: new HtmlAttributes()
                            .DataId(script.Id.ToString()),
                        action: () => hb
                            .Td(action: () => hb
                                .CheckBox(
                                    controlCss: "select",
                                    _checked: selected?
                                        .Contains(script.Id) == true))
                            .Td(action: () => hb
                                .Text(text: script.Id.ToString()))
                            .Td(action: () => hb
                                .Text(text: script.Title))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: script.All == true))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: script.New == true))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: script.Edit == true))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: script.Index == true))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: script.Calendar == true))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: script.Crosstab == true))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: script.Gantt == true))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: script.BurnDown == true))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: script.TimeSeries == true))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: script.Kamban == true))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: script.ImageLib == true)))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder ScriptDialog(
            Context context, SiteSettings ss, string controlId, Script script)
        {
            var hb = new HtmlBuilder();
            var conditions = ss.ViewSelectableOptions();
            var outputDestinationCss = " output-destination-script" +
                (script.All == true
                    ? " hidden"
                    : string.Empty);
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("ScriptForm")
                    .Action(Locations.ItemAction(ss.SiteId)),
                action: () => hb
                    .FieldText(
                        controlId: "ScriptId",
                        controlCss: " always-send",
                        labelText: Displays.Id(context: context),
                        text: script.Id.ToString(),
                        _using: controlId == "EditScript")
                    .FieldTextBox(
                        controlId: "ScriptTitle",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.Title(context: context),
                        text: script.Title,
                        validateRequired: true)
                    .FieldTextBox(
                        textType: HtmlTypes.TextTypes.MultiLine,
                        controlId: "ScriptBody",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.Script(context: context),
                        text: script.Body)
                    .FieldSet(
                        css: " enclosed",
                        legendText: Displays.OutputDestination(context: context),
                        action: () => hb
                            .FieldCheckBox(
                                fieldId: "ScriptAllField",
                                controlId: "ScriptAll",
                                controlCss: " always-send",
                                labelText: Displays.All(context: context),
                                _checked: script.All == true)
                            .FieldCheckBox(
                                controlId: "ScriptNew",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.New(context: context),
                                _checked: script.New == true)
                            .FieldCheckBox(
                                controlId: "ScriptEdit",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.Edit(context: context),
                                _checked: script.Edit == true)
                            .FieldCheckBox(
                                controlId: "ScriptIndex",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.Index(context: context),
                                _checked: script.Index == true)
                            .FieldCheckBox(
                                controlId: "ScriptCalendar",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.Calendar(context: context),
                                _checked: script.Calendar == true)
                            .FieldCheckBox(
                                controlId: "ScriptCrosstab",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.Crosstab(context: context),
                                _checked: script.Crosstab == true)
                            .FieldCheckBox(
                                controlId: "ScriptGantt",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.Gantt(context: context),
                                _checked: script.Gantt == true)
                            .FieldCheckBox(
                                controlId: "ScriptBurnDown",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.BurnDown(context: context),
                                _checked: script.BurnDown == true)
                            .FieldCheckBox(
                                controlId: "ScriptTimeSeries",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.TimeSeries(context: context),
                                _checked: script.TimeSeries == true)
                            .FieldCheckBox(
                                controlId: "ScriptKamban",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.Kamban(context: context),
                                _checked: script.Kamban == true)
                            .FieldCheckBox(
                                controlId: "ScriptImageLib",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.ImageLib(context: context),
                                _checked: script.ImageLib == true))
                    .P(css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "AddScript",
                            text: Displays.Add(context: context),
                            controlCss: "button-icon validate",
                            icon: "ui-icon-disk",
                            onClick: "$p.setScript($(this));",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "NewScript")
                        .Button(
                            controlId: "UpdateScript",
                            text: Displays.Change(context: context),
                            controlCss: "button-icon validate",
                            onClick: "$p.setScript($(this));",
                            icon: "ui-icon-disk",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "EditScript")
                        .Button(
                            text: Displays.Cancel(context: context),
                            controlCss: "button-icon",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder DeleteSiteDialog(this HtmlBuilder hb, Context context)
        {
            return hb.Div(
                attributes: new HtmlAttributes()
                    .Id("DeleteSiteDialog")
                    .Class("dialog")
                    .Title(Displays.ConfirmDeleteSite(context: context)),
                action: () => hb
                    .FieldTextBox(
                        controlId: "DeleteSiteTitle",
                        labelText: Displays.SiteTitle(context: context))
                    .FieldTextBox(
                        controlId: "Users_LoginId",
                        labelText: Displays.Users_LoginId(context: context),
                        _using: !Authentications.Windows())
                    .FieldTextBox(
                        textType: HtmlTypes.TextTypes.Password,
                        controlId: "Users_Password",
                        labelText: Displays.Users_Password(context: context),
                        _using: !Authentications.Windows())
                    .P(css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            text: Displays.DeleteSite(context: context),
                            controlCss: "button-icon",
                            onClick: "$p.send($(this));",
                            icon: "ui-icon-trash",
                            action: "Delete",
                            method: "delete",
                            confirm: "ConfirmDelete")
                        .Button(
                            text: Displays.Cancel(context: context),
                            controlCss: "button-icon",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static Permissions.Types SiteTopPermission(Context context)
        {
            return context.UserSettings?.DisableTopSiteCreation == true
                ? Permissions.Types.Read
                : (Permissions.Types)Parameters.Permissions.Manager;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string SynchronizeTitles(Context context, SiteModel siteModel)
        {
            var ss = siteModel.SiteSettings;
            var invalid = SiteValidators.OnUpdating(
                context: context, ss: ss, siteModel: siteModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            ItemUtilities.UpdateTitles(context: context, ss: ss);
            return Messages.ResponseSynchronizationCompleted(context: context).ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string SynchronizeSummaries(Context context, SiteModel siteModel)
        {
            siteModel.SetSiteSettingsPropertiesBySession(context: context);
            siteModel.SiteSettings = SiteSettingsUtilities.Get(
                context: context, siteModel: siteModel, referenceId: siteModel.SiteId);
            var ss = siteModel.SiteSettings;
            var invalid = SiteValidators.OnUpdating(
                context: context, ss: ss, siteModel: siteModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var selected = Forms.IntList("EditSummary");
            if (selected?.Any() != true)
            {
                return Messages.ResponseSelectTargets(context: context).ToJson();
            }
            else
            {
                selected.ForEach(id => Summaries.Synchronize(
                    context: context,
                    ss: ss,
                    id: id));
                return Messages.ResponseSynchronizationCompleted(context: context).ToJson();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string SynchronizeFormulas(Context context, SiteModel siteModel)
        {
            siteModel.SetSiteSettingsPropertiesBySession(context: context);
            siteModel.SiteSettings = SiteSettingsUtilities.Get(
                context: context, siteModel: siteModel, referenceId: siteModel.SiteId);
            var ss = siteModel.SiteSettings;
            var invalid = SiteValidators.OnUpdating(
                context: context, ss: ss, siteModel: siteModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var selected = Forms.IntList("EditFormula");
            if (selected?.Any() != true)
            {
                return Messages.ResponseSelectTargets(context: context).ToJson();
            }
            else
            {
                ss.SetChoiceHash(context: context);
                FormulaUtilities.Synchronize(
                    context: context,
                    siteModel: siteModel,
                    selected: selected);
                return Messages.ResponseSynchronizationCompleted(context: context).ToJson();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditRelatingColumns(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            var selected = Forms.Data("EditRelatingColumns").Deserialize<IEnumerable<int>>();
            return hb.Table(
                id: "EditRelatingColumns",
                css: "grid",
                attributes: new HtmlAttributes()
                    .DataName("RelatingColumnId")
                    .DataFunc("openRelatingColumnDialog")
                    .DataAction("SetSiteSettings")
                    .DataMethod("post"),
                action: () => hb
                    .EditRelatingColumnsHeader(
                        context: context,
                        ss: ss,
                        selected: selected)
                    .EditRelatingColumnsBody(
                        ss: ss,
                        selected: selected));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditRelatingColumnsHeader(
            this HtmlBuilder hb, Context context, SiteSettings ss, IEnumerable<int> selected)
        {
            return hb.THead(action: () => hb
                .Tr(css: "ui-widget-header", action: () => hb
                    .Th(action: () => hb
                        .CheckBox(
                            controlCss: "select-all",
                            _checked: false))
                    .Th(action: () => hb
                        .Text(text: Displays.Title(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Links(context: context)))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditRelatingColumnsBody(
            this HtmlBuilder hb, SiteSettings ss, IEnumerable<int> selected)
        {
            return hb.TBody(action: () => ss
                .RelatingColumns?.ForEach(relatingColumn => hb
                    .Tr(
                        css: "grid-row",
                        attributes: new HtmlAttributes()
                            .DataId(relatingColumn.Id.ToString()),
                        action: () => hb
                            .Td(action: () => hb
                                .CheckBox(
                                    controlCss: "select",
                                    _checked: selected?
                                        .Contains(relatingColumn.Id) == true))
                            .Td(action: () => hb
                                .Text(text: relatingColumn.Title))
                            .Td(action: () => hb
                                .Text(text: relatingColumn.Columns?
                                    .Select(o => GetClassLabelText(ss, o)).Join(", "))))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string GetClassLabelText(SiteSettings ss, string className)
        {
            return (ss?.ColumnHash?.FirstOrDefault(o => o.Key == className))?.Value?.LabelText ?? className;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder RelatingColumnDialog(
            Context context, SiteSettings ss, string controlId, RelatingColumn relatingColumn)
        {
            var hb = new HtmlBuilder();
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("RelatingColumnForm")
                    .Action(Locations.ItemAction(ss.SiteId)),
                action: () => hb
                    .FieldText(
                        controlId: "RelatingColumnId",
                        controlCss: " always-send",
                        labelText: Displays.Id(context: context),
                        text: relatingColumn.Id.ToString(),
                        _using: controlId == "EditRelatingColumns")
                    .FieldTextBox(
                        controlId: "RelatingColumnTitle",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.Title(context: context),
                        text: relatingColumn.Title,
                        validateRequired: true)
                .FieldSet(
                    css: " enclosed",
                    legendText: Displays.RelatingColumnSettings(context: context),
                    action: () => hb
                        .FieldSelectable(
                            controlId: "RelatingColumnColumns",
                            fieldCss: "field-vertical",
                            controlContainerCss: "container-selectable",
                            controlWrapperCss: " h350",
                            controlCss: " always-send send-all",
                            labelText: Displays.CurrentSettings(context: context),
                            listItemCollection: ss.RelatingColumnSelectableOptions(
                                context: context,
                                id: relatingColumn.Id),
                            commandOptionPositionIsTop: true,
                            commandOptionAction: () => hb
                                .Div(css: "command-center", action: () => hb
                                    .Button(
                                        controlId: "MoveUpRelatingColumnColumnsLocal",
                                        text: Displays.MoveUp(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.moveColumns($(this),'RelatingColumn');",
                                        icon: "ui-icon-circle-triangle-n")
                                    .Button(
                                        controlId: "MoveDownRelatingColumnColumnsLocal",
                                        text: Displays.MoveDown(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.moveColumns($(this),'RelatingColumn');",
                                        icon: "ui-icon-circle-triangle-s")
                                    .Button(
                                        controlId: "ToDisableRelatingColumnColumnsLocal",
                                        text: Displays.ToDisable(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.moveColumns($(this),'RelatingColumn');",
                                        icon: "ui-icon-circle-triangle-e")))
                        .FieldSelectable(
                            controlId: "RelatingColumnSourceColumns",
                            fieldCss: "field-vertical",
                            controlContainerCss: "container-selectable",
                            controlWrapperCss: " h350",
                            labelText: Displays.OptionList(context: context),
                            listItemCollection: ss.RelatingColumnSelectableOptions(
                                context: context,
                                id: relatingColumn.Id,
                                enabled: false),
                            commandOptionPositionIsTop: true,
                            commandOptionAction: () => hb
                                .Div(css: "command-center", action: () => hb
                                    .Button(
                                        controlId: "ToEnableRelatingColumnColumnsLocal",
                                        text: Displays.ToEnable(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.moveColumns($(this),'RelatingColumn');",
                                        icon: "ui-icon-circle-triangle-w"))))
                    .P(css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "AddRelatingColumn",
                            text: Displays.Add(context: context),
                            controlCss: "button-icon validate",
                            icon: "ui-icon-disk",
                            onClick: "$p.setRelatingColumn($(this));",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "NewRelatingColumn")
                        .Button(
                            controlId: "UpdateRelatingColumn",
                            text: Displays.Change(context: context),
                            controlCss: "button-icon validate",
                            onClick: "$p.setRelatingColumn($(this));",
                            icon: "ui-icon-disk",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "EditRelatingColumns")
                        .Button(
                            text: Displays.Cancel(context: context),
                            controlCss: "button-icon",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }
    }
}
