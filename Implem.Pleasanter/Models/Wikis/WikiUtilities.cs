using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Converts;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
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
            this HtmlBuilder hb, Column column, WikiModel wikiModel)
        {
            switch (column.ColumnName)
            {
                case "SiteId": return hb.Td(column: column, value: wikiModel.SiteId);
                case "UpdatedTime": return hb.Td(column: column, value: wikiModel.UpdatedTime);
                case "WikiId": return hb.Td(column: column, value: wikiModel.WikiId);
                case "Ver": return hb.Td(column: column, value: wikiModel.Ver);
                case "Title": return hb.Td(column: column, value: wikiModel.Title);
                case "Body": return hb.Td(column: column, value: wikiModel.Body);
                case "TitleBody": return hb.Td(column: column, value: wikiModel.TitleBody);
                case "Comments": return hb.Td(column: column, value: wikiModel.Comments);
                case "Creator": return hb.Td(column: column, value: wikiModel.Creator);
                case "Updator": return hb.Td(column: column, value: wikiModel.Updator);
                case "CreatedTime": return hb.Td(column: column, value: wikiModel.CreatedTime);
                default: return hb;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string EditorNew(SiteModel siteModel)
        {
            var wikiId = Rds.ExecuteScalar_long(statements:
                Rds.SelectWikis(
                    column: Rds.WikisColumn().WikiId(),
                    where: Rds.WikisWhere().SiteId(siteModel.SiteId)));
            return wikiId == 0
                ? Editor(siteModel, new WikiModel(
                    siteModel.WikisSiteSettings(), methodType: BaseModel.MethodTypes.New))
                : new HtmlBuilder().NotFoundTemplate().ToString();
        }

        public static string Editor(SiteModel siteModel, long wikiId, bool clearSessions)
        {
            var siteSettings = siteModel.WikisSiteSettings();
            var wikiModel = new WikiModel(
                siteSettings: siteSettings,
                wikiId: wikiId,
                clearSessions: clearSessions,
                methodType: BaseModel.MethodTypes.Edit);
            wikiModel.SwitchTargets = GetSwitchTargets(
                siteSettings, wikiModel.SiteId);
            return Editor(siteModel, wikiModel);
        }

        public static string Editor(SiteModel siteModel, WikiModel wikiModel)
        {
            var hb = new HtmlBuilder();
            return hb.Template(
                permissionType: siteModel.PermissionType,
                verType: wikiModel.VerType,
                methodType: wikiModel.MethodType,
                allowAccess:
                    siteModel.PermissionType.CanRead() &&
                    wikiModel.AccessStatus != Databases.AccessStatuses.NotFound,
                siteId: siteModel.SiteId,
                referenceType: "Wikis",
                title: wikiModel.MethodType == BaseModel.MethodTypes.New
                    ? siteModel.Title.DisplayValue + " - " + Displays.New()
                    : wikiModel.Title.DisplayValue,
                userScript: wikiModel.MethodType == BaseModel.MethodTypes.New
                    ? wikiModel.SiteSettings.NewScript
                    : wikiModel.SiteSettings.EditScript,
                userStyle: wikiModel.MethodType == BaseModel.MethodTypes.New
                    ? wikiModel.SiteSettings.NewStyle
                    : wikiModel.SiteSettings.EditStyle,
                action: () =>
                {
                    hb
                        .Editor(
                            siteSettings: wikiModel.SiteSettings,
                            wikiModel: wikiModel,
                            siteModel: siteModel)
                        .Hidden(controlId: "TableName", value: "Wikis")
                        .Hidden(controlId: "Id", value: wikiModel.WikiId.ToString());
                }).ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Editor(
            this HtmlBuilder hb,
            SiteModel siteModel,
            SiteSettings siteSettings,
            WikiModel wikiModel)
        {
            return hb.Div(id: "Editor", action: () => hb
                .Form(
                    attributes: new HtmlAttributes()
                        .Id("WikiForm")
                        .Class("main-form")
                        .Action(Navigations.ItemAction(wikiModel.WikiId != 0
                            ? wikiModel.WikiId
                            : siteModel.SiteId)),
                    action: () => hb
                        .RecordHeader(
                            permissionType: siteModel.PermissionType,
                            baseModel: wikiModel,
                            tableName: "Wikis")
                        .Div(id: "EditorComments", action: () => hb
                            .Comments(
                                comments: wikiModel.Comments,
                                verType: wikiModel.VerType))
                        .Div(id: "EditorTabsContainer", action: () => hb
                            .EditorTabs(wikiModel: wikiModel)
                            .FieldSetGeneral(
                                wikiModel: wikiModel,
                                permissionType: siteModel.PermissionType,
                                siteSettings: siteSettings)
                            .FieldSet(
                                attributes: new HtmlAttributes()
                                    .Id("FieldSetHistories")
                                    .DataAction("Histories")
                                    .DataMethod("get"),
                                _using: wikiModel.MethodType != BaseModel.MethodTypes.New)
                            .MainCommands(
                                siteId: siteModel.SiteId,
                                permissionType: siteModel.PermissionType,
                                verType: wikiModel.VerType,
                                referenceType: "items",
                                referenceId: wikiModel.WikiId,
                                updateButton: true,
                                copyButton: false,
                                moveButton: false,
                                mailButton: true,
                                deleteButton: true))
                        .Hidden(controlId: "BaseUrl", value: Navigations.BaseUrl())
                        .Hidden(controlId: "MethodType", value: "edit")
                        .Hidden(
                            controlId: "Wikis_Timestamp",
                            css: "must-transport",
                            value: wikiModel.Timestamp)
                        .Hidden(
                            controlId: "SwitchTargets",
                            css: "must-transport",
                            value: wikiModel.WikiId.ToString(),
                            _using: !Request.IsAjax() || Routes.Action() == "create"))
                .OutgoingMailsForm("Wikis", wikiModel.WikiId, wikiModel.Ver)
                .CopyDialog("items", wikiModel.WikiId)
                .MoveDialog()
                .OutgoingMailDialog());
        }

        private static HtmlBuilder EditorTabs(this HtmlBuilder hb, WikiModel wikiModel)
        {
            return hb.Ul(id: "EditorTabs", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetGeneral", 
                        text: Displays.Basic()))
                .Li(
                    _using: wikiModel.MethodType != BaseModel.MethodTypes.New,
                    action: () => hb
                        .A(
                            href: "#FieldSetHistories",
                            text: Displays.Histories())));
        }

        private static HtmlBuilder FieldSetGeneral(
            this HtmlBuilder hb,
            WikiModel wikiModel,
            Permissions.Types permissionType,
            SiteSettings siteSettings)
        {
            return hb.FieldSet(id: "FieldSetGeneral", action: () =>
            {
                siteSettings.EditorColumnCollection().ForEach(column =>
                {
                    switch (column.ColumnName)
                    {
                        case "WikiId": hb.Field(siteSettings, column, wikiModel.MethodType, wikiModel.WikiId.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "Ver": hb.Field(siteSettings, column, wikiModel.MethodType, wikiModel.Ver.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "Title": hb.Field(siteSettings, column, wikiModel.MethodType, wikiModel.Title.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "Body": hb.Field(siteSettings, column, wikiModel.MethodType, wikiModel.Body.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                    }
                });
                hb.VerUpCheckBox(wikiModel);
            });
        }

        private static HtmlBuilder MainCommandExtensions(
            this HtmlBuilder hb,
            WikiModel wikiModel,
            SiteSettings siteSettings)
        {
            return hb;
        }

        private static HtmlBuilder EditorExtensions(
            this HtmlBuilder hb,
            WikiModel wikiModel,
            SiteSettings siteSettings)
        {
            return hb;
        }

        public static string EditorJson(
            SiteSettings siteSettings, Permissions.Types permissionType, long wikiId)
        {
            return EditorResponse(new WikiModel(siteSettings, wikiId))
                .ToJson();
        }

        private static ResponseCollection EditorResponse(
            WikiModel wikiModel, Message message = null)
        {
            var siteModel = new SiteModel(wikiModel.SiteId);
            wikiModel.MethodType = BaseModel.MethodTypes.Edit;
            return new WikisResponseCollection(wikiModel)
                .Invoke("clearDialogs")
                .ReplaceAll("#MainContainer", Editor(siteModel, wikiModel))
                .Invoke("setCurrentIndex")
                .Invoke("validateWikis")
                .Message(message)
                .ClearFormData();
        }

        public static List<long> GetSwitchTargets(SiteSettings siteSettings, long siteId)
        {
            var switchTargets = Forms.Data("SwitchTargets").Split(',')
                .Select(o => o.ToLong())
                .Where(o => o != 0)
                .ToList();
            if (switchTargets.Count() == 0)
            {
                var formData = DataViewFilters.SessionFormData(siteId);
                switchTargets = Rds.ExecuteTable(
                    transactional: false,
                    statements: Rds.SelectWikis(
                        column: Rds.WikisColumn().WikiId(),
                        where: DataViewFilters.Get(
                            siteSettings: siteSettings,
                            tableName: "Wikis",
                            formData: formData,
                            where: Rds.WikisWhere().SiteId(siteId)),
                        orderBy: GridSorters.Get(
                            formData, Rds.WikisOrderBy().UpdatedTime(SqlOrderBy.Types.desc))))
                                .AsEnumerable()
                                .Select(o => o["WikiId"].ToLong())
                                .ToList();    
            }
            return switchTargets;
        }

        public static ResponseCollection FormResponse(
            this ResponseCollection responseCollection,
            Permissions.Types permissionType,
            WikiModel wikiModel)
        {
            Forms.All().Keys.ForEach(key =>
            {
                switch (key)
                {
                    default: break;
                }
            });
            return responseCollection;
        }

        public static ResponseCollection Formula(
            this ResponseCollection responseCollection,
            Permissions.Types permissionType,
            WikiModel wikiModel)
        {
            wikiModel.SiteSettings.FormulaHash?.Keys.ForEach(columnName =>
            {
                var column = wikiModel.SiteSettings.GetColumn(columnName);
                switch (columnName)
                {
                    default: break;
                }
            });
            return responseCollection;
        }

        public static string Update(
            SiteSettings siteSettings, Permissions.Types permissionType, long wikiId)
        {
            var wikiModel = new WikiModel(siteSettings, wikiId, setByForm: true);
            var invalid = WikiValidators.OnUpdating(siteSettings, permissionType, wikiModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return new ResponseCollection().Message(invalid.Message()).ToJson();
            }
            if (wikiModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts().ToJson();
            }
            var error = wikiModel.Update();
            if (error.Has())
            {
                return error == Error.Types.UpdateConflicts
                    ? Messages.ResponseUpdateConflicts(wikiModel.Updator.FullName()).ToJson()
                    : new ResponseCollection().Message(error.Message()).ToJson();
            }
            else
            {
                var responseCollection = new WikisResponseCollection(wikiModel);
                responseCollection.ReplaceAll("#Breadcrumb", new HtmlBuilder()
                    .Breadcrumb(siteSettings.SiteId));
                return ResponseByUpdate(permissionType, wikiModel, responseCollection)
                    .PrependComment(wikiModel.Comments, wikiModel.VerType)
                    .ToJson();
            }
        }

        private static ResponseCollection ResponseByUpdate(
            Permissions.Types permissionType,
            WikiModel wikiModel,
            WikisResponseCollection responseCollection)
        {
            return responseCollection
                .Ver()
                .Timestamp()
                .Val("#VerUp", false)
                .FormResponse(permissionType, wikiModel)
                .Formula(permissionType, wikiModel)
                .Disabled("#VerUp", false)
                .Html("#HeaderTitle", wikiModel.Title.DisplayValue)
                .Html("#RecordInfo", new HtmlBuilder().RecordInfo(
                    baseModel: wikiModel, tableName: "Wikis"))
                .Html("#Links", new HtmlBuilder().Links(wikiModel.WikiId))
                .Message(Messages.Updated(wikiModel.Title.ToString()))
                .RemoveComment(wikiModel.DeleteCommentId, _using: wikiModel.DeleteCommentId != 0)
                .ClearFormData();
        }

        public static string Copy(SiteSettings siteSettings, Permissions.Types permissionType, long wikiId)
        {
            var wikiModel = new WikiModel(siteSettings, wikiId, setByForm: true);
            wikiModel.WikiId = 0;
            if (siteSettings.EditorColumnsOrder.Contains("Title"))
            {
                wikiModel.Title.Value += Displays.SuffixCopy();
            }
            if (!Forms.Data("CopyWithComments").ToBool())
            {
                wikiModel.Comments.Clear();
            }
            var error = wikiModel.Create(paramAll: true);
            if (error.Has())
            {
                return error.MessageJson();
            }
            else
            {
                return EditorResponse(wikiModel).ToJson();
            }
        }

        public static string Delete(
            SiteSettings siteSettings, Permissions.Types permissionType, long wikiId)
        {
            var wikiModel = new WikiModel(siteSettings, wikiId);
            var invalid = WikiValidators.OnDeleting(siteSettings, permissionType, wikiModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var error = wikiModel.Delete();
            if (error.Has())
            {
                return error.MessageJson();
            }
            else
            {
                Sessions.Set("Message", Messages.Deleted(wikiModel.Title.Value).Html);
                var responseCollection = new WikisResponseCollection(wikiModel);
                responseCollection.Href(Navigations.ItemIndex(
                    new SiteModel(wikiModel.SiteId).ParentId));
                return responseCollection.ToJson();
            }
        }

        public static string Restore(long wikiId)
        {
            var wikiModel = new WikiModel();
            var invalid = WikiValidators.OnRestoring();
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var error = wikiModel.Restore(wikiId);
            if (error.Has())
            {
                return error.MessageJson();
            }
            else
            {
                var responseCollection = new WikisResponseCollection(wikiModel);
                return responseCollection.ToJson();
            }
        }

        public static string Histories(
            SiteSettings siteSettings, Permissions.Types permissionType, long wikiId)
        {
            var wikiModel = new WikiModel(siteSettings, wikiId);
            var columns = siteSettings.HistoryColumnCollection();
            var hb = new HtmlBuilder();
            hb.Table(
                attributes: new HtmlAttributes().Class("grid"),
                action: () => hb
                    .THead(action: () => hb
                        .GridHeader(
                            columnCollection: columns,
                            sort: false,
                            checkRow: false))
                    .TBody(action: () =>
                        new WikiCollection(
                            siteSettings: siteSettings,
                            permissionType: permissionType,
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
                                                .TdValue(column, wikiModelHistory))))));
            return new WikisResponseCollection(wikiModel)
                .Html("#FieldSetHistories", hb).ToJson();
        }

        public static string History(
            SiteSettings siteSettings, Permissions.Types permissionType, long wikiId)
        {
            var wikiModel = new WikiModel(siteSettings, wikiId);
            wikiModel.Get(
                where: Rds.WikisWhere()
                    .WikiId(wikiModel.WikiId)
                    .Ver(Forms.Int("Ver")),
                tableType: Sqls.TableTypes.NormalAndHistory);
            wikiModel.VerType =  Forms.Bool("Latest")
                ? Versions.VerTypes.Latest
                : Versions.VerTypes.History;
            return EditorResponse(wikiModel).ToJson();
        }

        public static string TitleDisplayValue(SiteSettings siteSettings, WikiModel wikiModel)
        {
            var displayValue = siteSettings.TitleColumnCollection()
                .Select(column => TitleDisplayValue(column, wikiModel))
                .Where(o => o != string.Empty)
                .Join(siteSettings.TitleSeparator);
            return displayValue != string.Empty
                ? displayValue
                : Displays.NoTitle();
        }

        private static string TitleDisplayValue(Column column, WikiModel wikiModel)
        {
            switch (column.ColumnName)
            {
                case "Title": return column.HasChoices()
                    ? column.Choice(wikiModel.Title.Value).Text
                    : wikiModel.Title.Value;
                default: return string.Empty;
            }
        }

        public static string TitleDisplayValue(SiteSettings siteSettings, DataRow dataRow)
        {
            var displayValue = siteSettings.TitleColumnCollection()
                .Select(column => TitleDisplayValue(column, dataRow))
                .Where(o => o != string.Empty)
                .Join(siteSettings.TitleSeparator);
            return displayValue != string.Empty
                ? displayValue
                : Displays.NoTitle();
        }

        private static string TitleDisplayValue(Column column, DataRow dataRow)
        {
            switch (column.ColumnName)
            {
                case "Title": return column.HasChoices()
                    ? column.Choice(dataRow["Title"].ToString()).Text
                    : dataRow["Title"].ToString();
                default: return string.Empty;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Index(SiteSettings siteSettings, Permissions.Types permissionType)
        {
            return new HtmlBuilder().NotFoundTemplate().ToString();
        }
    }
}
