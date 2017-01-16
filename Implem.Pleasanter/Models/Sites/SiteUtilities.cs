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
    public static class SiteUtilities
    {
        public static HtmlBuilder TdValue(
            this HtmlBuilder hb, SiteSettings ss, Column column, SiteModel siteModel)
        {
            if (!column.GridDesign.IsNullOrEmpty())
            {
                return hb.TdCustomValue(
                    ss: ss,
                    gridDesign: column.GridDesign,
                    siteModel: siteModel);
            }
            else
            {
                switch (column.ColumnName)
                {
                    case "SiteId": return hb.Td(column: column, value: siteModel.SiteId);
                    case "UpdatedTime": return hb.Td(column: column, value: siteModel.UpdatedTime);
                    case "Ver": return hb.Td(column: column, value: siteModel.Ver);
                    case "Title": return hb.Td(column: column, value: siteModel.Title);
                    case "Body": return hb.Td(column: column, value: siteModel.Body);
                    case "TitleBody": return hb.Td(column: column, value: siteModel.TitleBody);
                    case "Comments": return hb.Td(column: column, value: siteModel.Comments);
                    case "Creator": return hb.Td(column: column, value: siteModel.Creator);
                    case "Updator": return hb.Td(column: column, value: siteModel.Updator);
                    case "CreatedTime": return hb.Td(column: column, value: siteModel.CreatedTime);
                    default: return hb;
                }
            }
        }

        public static HtmlBuilder TdCustomValue(
            this HtmlBuilder hb, SiteSettings ss, string gridDesign, SiteModel siteModel)
        {
            ss.IncludedColumns(gridDesign).ForEach(column =>
            {
                var value = string.Empty;
                switch (column.ColumnName)
                {
                    case "SiteId": value = siteModel.SiteId.GridText(column: column); break;
                    case "UpdatedTime": value = siteModel.UpdatedTime.GridText(column: column); break;
                    case "Ver": value = siteModel.Ver.GridText(column: column); break;
                    case "Title": value = siteModel.Title.GridText(column: column); break;
                    case "Body": value = siteModel.Body.GridText(column: column); break;
                    case "TitleBody": value = siteModel.TitleBody.GridText(column: column); break;
                    case "Comments": value = siteModel.Comments.GridText(column: column); break;
                    case "Creator": value = siteModel.Creator.GridText(column: column); break;
                    case "Updator": value = siteModel.Updator.GridText(column: column); break;
                    case "CreatedTime": value = siteModel.CreatedTime.GridText(column: column); break;
                }
                gridDesign = gridDesign.Replace("[" + column.ColumnName + "]", value);
            });
            return hb.Td(action: () => hb
                .Div(css: "markup", action: () => hb
                    .Text(text: gridDesign)));
        }

        public static string EditorJson(long siteId)
        {
            return EditorResponse(new SiteModel(siteId)).ToJson();
        }

        private static ResponseCollection EditorResponse(
            SiteModel siteModel, Message message = null, string switchTargets = null)
        {
            siteModel.MethodType = BaseModel.MethodTypes.Edit;
            return new SitesResponseCollection(siteModel)
                .Invoke("clearDialogs")
                .ReplaceAll("#MainContainer", Editor(siteModel))
                .Val("#SwitchTargets", switchTargets, _using: switchTargets != null)
                .Invoke("setCurrentIndex")
                .Message(message)
                .ClearFormData();
        }

        private static HtmlBuilder ReferenceType(
            this HtmlBuilder hb, string selectedValue, BaseModel.MethodTypes methodType)
        {
            return methodType == BaseModel.MethodTypes.New
                ? hb.Select(
                    attributes: new HtmlAttributes()
                        .Id("Sites_ReferenceType")
                        .Class("control-dropdown"),
                    action: () => hb
                        .OptionCollection(optionCollection: new Dictionary<string, string>
                        {
                            { "Sites", Displays.Sites() },
                            { "Issues", Displays.Issues() },
                            { "Results", Displays.Results() },
                            { "Wikis", Displays.Wikis() }
                        },
                        selectedValue: selectedValue))
                : hb.Span(css: "control-text", action: () => hb
                    .Text(text: Displays.Get(selectedValue)));
        }

        public static string Create(
            Permissions.Types pt,
            long parentId,
            long inheritPermission)
        {
            var siteModel = new SiteModel(parentId, inheritPermission, setByForm: true);
            var ss = siteModel.SiteSettings;
            var invalid = SiteValidators.OnCreating(ss, pt, siteModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var error = siteModel.Create();
            if (error.Has())
            {
                return error.MessageJson();
            }
            else
            {
                switch (siteModel.ReferenceType)
                {
                    case "Wikis":
                        var wikiModel = new WikiModel(siteModel.WikisSiteSettings())
                            .Get(where: Rds.WikisWhere().SiteId(siteModel.SiteId));
                        return new ResponseCollection()
                            .ReplaceAll(
                                "#MainContainer",
                                WikiUtilities.Editor(siteModel, wikiModel))
                            .Val("#BackUrl", Locations.ItemIndex(siteModel.ParentId))
                            .Invoke("setSwitchTargets")
                            .ToJson();
                    default:
                        return pt.CanEditSite()
                            ? EditorResponse(
                                siteModel, Messages.Created(siteModel.Title.ToString())).ToJson()
                            : new ResponseCollection().Href(
                                Locations.ItemIndex(siteModel.SiteId)).ToJson();
                }
            }
        }

        public static string Update(SiteSettings ss, Permissions.Types pt, long siteId)
        {
            var siteModel = new SiteModel(siteId, setByForm: true);
            var invalid = SiteValidators.OnUpdating(ss, pt, siteModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return new ResponseCollection().Message(invalid.Message()).ToJson();
            }
            if (siteModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts().ToJson();
            }
            var error = siteModel.Update();
            if (error.Has())
            {
                return error == Error.Types.UpdateConflicts
                    ? Messages.ResponseUpdateConflicts(siteModel.Updator.FullName()).ToJson()
                    : new ResponseCollection().Message(error.Message()).ToJson();
            }
            else
            {
                var res = new SitesResponseCollection(siteModel);
                res.ReplaceAll("#Breadcrumb", new HtmlBuilder()
                    .Breadcrumb(ss.SiteId));
                return ResponseByUpdate(pt, res, siteModel)
                    .PrependComment(siteModel.Comments, siteModel.VerType)
                    .ToJson();
            }
        }

        private static ResponseCollection ResponseByUpdate(
            Permissions.Types pt, SitesResponseCollection res, SiteModel siteModel)
        {
            return res
                .Ver()
                .Timestamp()
                .Val("#VerUp", false)
                .Disabled("#VerUp", false)
                .Html("#HeaderTitle", siteModel.Title.DisplayValue)
                .Html("#RecordInfo", new HtmlBuilder().RecordInfo(
                    baseModel: siteModel, tableName: "Sites"))
                .Message(Messages.Updated(siteModel.Title.ToString()))
                .RemoveComment(siteModel.DeleteCommentId, _using: siteModel.DeleteCommentId != 0)
                .ClearFormData();
        }

        public static string Copy(SiteModel siteModel)
        {
            siteModel.Title.Value += Displays.SuffixCopy();
            if (!Forms.Data("CopyWithComments").ToBool())
            {
                siteModel.Comments.Clear();
            }
            var error = siteModel.Create(paramAll: true);
            if (error.Has())
            {
                return error.MessageJson();
            }
            else
            {
                return EditorResponse(siteModel).ToJson();
            }
        }

        public static string Delete(long siteId)
        {
            var siteModel = new SiteModel(siteId);
            var ss = siteModel.SiteSettings;
            var pt = siteModel.PermissionType;
            var invalid = SiteValidators.OnDeleting(ss, pt, siteModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var error = siteModel.Delete();
            if (error.Has())
            {
                return error.MessageJson();
            }
            else
            {
                Sessions.Set("Message", Messages.Deleted(siteModel.Title.Value).Html);
                var res = new SitesResponseCollection(siteModel);
                res.Href(Locations.ItemIndex(siteModel.ParentId));
                return res.ToJson();
            }
        }

        public static string Restore(long siteId)
        {
            var siteModel = new SiteModel();
            var invalid = SiteValidators.OnRestoring();
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var error = siteModel.Restore(siteId);
            if (error.Has())
            {
                return error.MessageJson();
            }
            else
            {
                var res = new SitesResponseCollection(siteModel);
                return res.ToJson();
            }
        }

        public static string Histories(SiteModel siteModel)
        {
            var ss = siteModel.SitesSiteSettings();
            var pt = siteModel.PermissionType;
            var columns = ss.GetHistoryColumns();
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
                        new SiteCollection(
                            where: Rds.SitesWhere().SiteId(siteModel.SiteId),
                            orderBy: Rds.SitesOrderBy().Ver(SqlOrderBy.Types.desc),
                            tableType: Sqls.TableTypes.NormalAndHistory)
                                .ForEach(siteModelHistory => hb
                                    .Tr(
                                        attributes: new HtmlAttributes()
                                            .Class("grid-row history not-link")
                                            .DataAction("History")
                                            .DataMethod("post")
                                            .DataVer(siteModelHistory.Ver)
                                            .DataLatest(1, _using:
                                                siteModelHistory.Ver == siteModel.Ver),
                                        action: () => columns
                                            .ForEach(column => hb
                                                .TdValue(
                                                    ss: ss,
                                                    column: column,
                                                    siteModel: siteModelHistory))))));
            return new SitesResponseCollection(siteModel)
                .Html("#FieldSetHistories", hb).ToJson();
        }

        public static string History(SiteModel siteModel)
        {
            var ss = siteModel.SitesSiteSettings();
            var pt = siteModel.PermissionType;
            siteModel.Get(
                where: Rds.SitesWhere()
                    .SiteId(siteModel.SiteId)
                    .Ver(Forms.Int("Ver")),
                tableType: Sqls.TableTypes.NormalAndHistory);
            siteModel.VerType =  Forms.Bool("Latest")
                ? Versions.VerTypes.Latest
                : Versions.VerTypes.History;
            return EditorResponse(siteModel).ToJson();
        }

        public static string TitleDisplayValue(SiteSettings ss, SiteModel siteModel)
        {
            var displayValue = ss.GetTitleColumns()
                .Select(column => TitleDisplayValue(column, siteModel))
                .Where(o => o != string.Empty)
                .Join(ss.TitleSeparator);
            return displayValue != string.Empty
                ? displayValue
                : Displays.NoTitle();
        }

        private static string TitleDisplayValue(Column column, SiteModel siteModel)
        {
            switch (column.ColumnName)
            {
                case "Title": return column.HasChoices()
                    ? column.Choice(siteModel.Title.Value).Text
                    : siteModel.Title.Value;
                default: return string.Empty;
            }
        }

        public static string TitleDisplayValue(SiteSettings ss, DataRow dataRow)
        {
            var displayValue = ss.GetTitleColumns()
                .Select(column => TitleDisplayValue(column, dataRow))
                .Where(o => o != string.Empty)
                .Join(ss.TitleSeparator);
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
        public static string MoveSiteMenu(long id)
        {
            var siteModel = new SiteModel(id);
            var sourceId = Forms.Long("SiteId");
            var destinationId = Forms.Long("DestinationId");
            var toParent = id != 0 && SiteInfo.SiteMenu.Get(id).ParentId == destinationId;
            var invalid = SiteValidators.OnMoving(
                id,
                destinationId,
                siteModel.PermissionType,
                Permissions.GetById(sourceId),
                Permissions.GetById(destinationId));
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return SiteMenuError(id, siteModel, invalid);
            }
            MoveSiteMenu(sourceId, destinationId);
            var res = new ResponseCollection().Remove(".nav-site[data-id=\"" + sourceId + "\"]");
            return toParent
                ? res.ToJson()
                : res
                    .ReplaceAll(
                        "[data-id=\"" + destinationId + "\"]",
                        siteModel.ReplaceSiteMenu(sourceId, destinationId))
                    .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void MoveSiteMenu(long sourceId, long destinationId)
        {
            Rds.ExecuteNonQuery(statements: Rds.UpdateSites(
                where: Rds.SitesWhere()
                    .TenantId(Sessions.TenantId())
                    .SiteId(sourceId),
                param: Rds.SitesParam().ParentId(destinationId)));
            SiteInfo.Reflesh();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string SortSiteMenu(long siteId)
        {
            var siteModel = new SiteModel(siteId);
            var invalid = SiteValidators.OnSorting(siteId, siteModel.PermissionType);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return SiteMenuError(siteId, siteModel, invalid);
            }
            var ownerId = siteModel.SiteId == 0
                ? Sessions.UserId()
                : 0;
            SortSiteMenu(siteModel, ownerId);
            return new ResponseCollection().ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void SortSiteMenu(SiteModel siteModel, int ownerId)
        {
            new OrderModel()
            {
                ReferenceId = siteModel.SiteId,
                ReferenceType = "Sites",
                OwnerId = ownerId,
                Data = Forms.LongList("Data").Where(o => o != 0).ToList()
            }.UpdateOrCreate();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string SiteMenuError(long id, SiteModel siteModel, Error.Types invalid)
        {
            return new ResponseCollection()
                .ReplaceAll("#SiteMenu", new HtmlBuilder().SiteMenu(
                    siteModel: id != 0 ? siteModel : null,
                    siteConditions: SiteInfo.SiteMenu.SiteConditions(0)))
                .Invoke("setSiteMenu")
                .Message(invalid.Message())
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Editor(long siteId, bool clearSessions)
        {
            return Editor(
                new SiteModel(
                    siteId, clearSessions, methodType: BaseModel.MethodTypes.Edit));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditorTabs(this HtmlBuilder hb, SiteModel siteModel)
        {
            return hb.Ul(id: "EditorTabs", action: () =>
            {
                hb.Li(action: () => hb
                    .A(
                        href: "#FieldSetGeneral",
                        text: Displays.Basic()));
                if (siteModel.MethodType != BaseModel.MethodTypes.New)
                {
                    hb.Li(action: () => hb
                        .A(
                            href: "#SiteImageSettingsEditor",
                            text: Displays.SiteImageSettingsEditor()));
                    switch (siteModel.ReferenceType)
                    {
                        case "Sites":
                            break;
                        case "Wikis":
                            hb
                                .Li(action: () => hb
                                    .A(
                                        href: "#MailSettingsEditor",
                                        text: Displays.Mail()))
                                .Li(action: () => hb
                                    .A(
                                        href: "#NotificationsSettingsEditor",
                                        text: Displays.Notifications()))
                                .Li(action: () => hb
                                    .A(
                                        href: "#StylesSettingsEditor",
                                        text: Displays.Styles()))
                                .Li(action: () => hb
                                    .A(
                                        href: "#ScriptsSettingsEditor",
                                        text: Displays.Scripts()));
                            break;
                        default:
                            hb
                                .Li(action: () => hb
                                    .A(
                                        href: "#GridSettingsEditor",
                                        text: Displays.Grid()))
                                .Li(action: () => hb
                                    .A(
                                        href: "#FiltersSettingsEditor",
                                        text: Displays.Filters()))
                                .Li(action: () => hb
                                    .A(
                                        href: "#AggregationsSettingsEditor",
                                        text: Displays.Aggregations()))
                                .Li(action: () => hb
                                    .A(
                                        href: "#EditorSettingsEditor",
                                        text: Displays.Editor()))
                                .Li(action: () => hb
                                    .A(
                                        href: "#LinksSettingsEditor",
                                        text: Displays.Links()))
                                .Li(action: () => hb
                                    .A(
                                        href: "#HistoriesSettingsEditor",
                                        text: Displays.Histories()))
                                .Li(action: () => hb
                                    .A(
                                        href: "#SummariesSettingsEditor",
                                        text: Displays.Summaries()))
                                .Li(action: () => hb
                                    .A(
                                        href: "#FormulasSettingsEditor",
                                        text: Displays.Formulas()))
                                .Li(action: () => hb
                                    .A(
                                        href: "#ViewsSettingsEditor",
                                        text: Displays.DataView()))
                                .Li(action: () => hb
                                    .A(
                                        href: "#NotificationsSettingsEditor",
                                        text: Displays.Notifications()))
                                .Li(action: () => hb
                                    .A(
                                        href: "#MailSettingsEditor",
                                        text: Displays.Mail()))
                                .Li(action: () => hb
                                    .A(
                                        href: "#StylesSettingsEditor",
                                        text: Displays.Styles()))
                                .Li(action: () => hb
                                    .A(
                                        href: "#ScriptsSettingsEditor",
                                        text: Displays.Scripts()));
                            break;
                    }
                    hb.Li(action: () => hb
                        .A(
                            href: "#FieldSetHistories",
                            text: Displays.ChangeHistoryList()));
                }
                hb.Hidden(controlId: "TableName", value: "Sites");
            });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string SiteTop()
        {
            var hb = new HtmlBuilder();
            var pt = Permissions.Admins() | Permissions.Types.Manager;
            var verType = Versions.VerTypes.Latest;
            var siteConditions = SiteInfo.SiteMenu.SiteConditions(0);
            return hb.Template(
                pt: pt,
                verType: verType,
                methodType: BaseModel.MethodTypes.Index,
                allowAccess: true,
                referenceType: "Sites",
                script: "$p.setSiteMenu();",
                action: () =>
                {
                    hb.Form(
                        attributes: new HtmlAttributes()
                            .Id("SitesForm")
                            .Class("main-form")
                            .Action(Locations.ItemAction(0)),
                        action: () => hb
                            .SiteMenu(siteModel: null, siteConditions: siteConditions)
                            .SiteMenuData());
                    hb.MainCommands(
                        siteId: 0,
                        pt: pt,
                        verType: verType,
                        backButton: false);
                }).ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string SiteMenu(SiteModel siteModel)
        {
            var hb = new HtmlBuilder();
            var ss = siteModel.SitesSiteSettings();
            var siteConditions = SiteInfo.SiteMenu.SiteConditions(siteModel.SiteId);
            return hb.Template(
                pt: siteModel.PermissionType,
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.Index,
                allowAccess:
                    siteModel.PermissionType.CanRead() &&
                    siteModel.AccessStatus != Databases.AccessStatuses.NotFound,
                siteId: siteModel.SiteId,
                parentId: siteModel.ParentId,
                referenceType: "Sites",
                script: "$p.setSiteMenu();",
                action: () =>
                {
                    hb.Form(
                        attributes: new HtmlAttributes()
                            .Id("SitesForm")
                            .Class("main-form")
                            .Action(Locations.ItemAction(ss.SiteId)),
                        action: () => hb
                            .SiteMenu(siteModel: siteModel, siteConditions: siteConditions)
                            .SiteMenuData());
                    if (ss.SiteId != 0)
                    {
                        hb.MainCommands(
                            siteId: siteModel.SiteId,
                            pt: siteModel.PermissionType,
                            verType: Versions.VerTypes.Latest);
                    }
                }).ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SiteMenu(
            this HtmlBuilder hb, SiteModel siteModel, IEnumerable<SiteCondition> siteConditions)
        {
            var ss = siteModel != null
                ? siteModel.SiteSettings
                : SiteSettingsUtility.SitesSiteSettings(0);
            var pt = siteModel != null
                ? siteModel.PermissionType
                : Permissions.Admins() | Permissions.Types.Manager;
            return hb.Div(id: "SiteMenu", action: () => hb
                .Nav(css: "cf", _using: siteModel != null, action: () => hb
                    .Ul(css: "nav-sites", action: () => hb
                        .ToParent(siteModel)))
                .Nav(css: "cf", action: () => hb
                    .Ul(css: "nav-sites sortable", action: () =>
                        Menu(ss.SiteId).ForEach(siteModelChild => hb
                            .SiteMenu(
                                ss: ss,
                                pt: pt,
                                siteId: siteModelChild.SiteId,
                                referenceType: siteModelChild.ReferenceType,
                                title: siteModelChild.Title.Value,
                                siteConditions: siteConditions))))
                .SiteMenuData());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ToParent(this HtmlBuilder hb, SiteModel siteModel)
        {
            return siteModel.SiteId != 0
                ? hb.SiteMenu(
                    ss: siteModel.SiteSettings,
                    pt: siteModel.PermissionType,
                    siteId: siteModel.ParentId,
                    referenceType: "Sites",
                    title: Displays.ToParent(),
                    toParent: true)
                : hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder SiteMenu(
            this HtmlBuilder hb,
            SiteSettings ss,
            Permissions.Types pt,
            long siteId,
            string referenceType,
            string title,
            bool toParent = false,
            IEnumerable<SiteCondition> siteConditions = null)
        {
            var hasImage = BinaryUtilities.ExistsSiteImage(
                pt, siteId, Libraries.Images.ImageData.SizeTypes.Thumbnail);
            var siteImagePrefix = BinaryUtilities.SiteImagePrefix(
                pt, siteId, Libraries.Images.ImageData.SizeTypes.Thumbnail);
            return hb.Li(
                attributes: new HtmlAttributes()
                    .Class(Libraries.Styles.Css.Class("nav-site " + referenceType.ToLower() +
                        (hasImage
                            ? " has-image"
                            : string.Empty),
                         toParent
                            ? " to-parent"
                            : string.Empty))
                    .DataId(siteId.ToString())
                    .DataType(referenceType),
                action: () => hb
                    .A(
                        attributes: new HtmlAttributes()
                            .Href(SiteHref(ss, siteId, referenceType)),
                        action: () => hb
                            .SiteMenuInnerElements(
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
        private static string SiteHref(SiteSettings ss, long siteId, string referenceType)
        {
            switch (referenceType)
            {
                case "Wikis":
                    return Locations.ItemEdit(Rds.ExecuteScalar_long(
                        statements: Rds.SelectWikis(
                            column: Rds.WikisColumn().WikiId(),
                            where: Rds.WikisWhere().SiteId(siteId))));
                default:
                    return Locations.Get(
                        "Items",
                        siteId.ToString(),
                        ViewModes.GetBySession(siteId));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SiteMenuInnerElements(
            this HtmlBuilder hb,
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
                .SiteMenuStyle(referenceType)
                .SiteMenuConditions(siteId, siteConditions);
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
                    default: return hb.Div(css: "stacking1").Div(css: "stacking2");
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SiteMenuConditions(
            this HtmlBuilder hb,
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
                        .ElapsedTime(condition.UpdatedTime.ToLocal())
                        .Span(
                            attributes: new HtmlAttributes()
                                .Class("count")
                                .Title(Displays.Quantity()),
                            action: () => hb
                                .Text(condition.ItemCount.ToString()))
                        .Span(
                            attributes: new HtmlAttributes()
                                .Class("overdue")
                                .Title(Displays.Overdue()),
                            _using: condition.OverdueCount > 0,
                            action: () => hb
                                .Text("({0})".Params(condition.OverdueCount))));
            }
            return hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static IEnumerable<SiteModel> Menu(long parentId)
        {
            var siteDataRows = new SiteCollection(
                column: Rds.SitesColumn()
                    .SiteId()
                    .Title()
                    .ReferenceType()
                    .PermissionType(),
                where: Rds.SitesWhere()
                    .TenantId(Sessions.TenantId())
                    .ParentId(parentId)
                    .PermissionType(_operator: " & " +
                        Permissions.Types.Read.ToInt().ToString() + "<>0"));
            var orderModel = new OrderModel(parentId, "Sites");
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
        public static string EditorNew(Permissions.Types pt, long siteId)
        {
            return Editor(
                new SiteModel()
                {
                    SiteSettings = new SiteSettings("Sites"),
                    MethodType = BaseModel.MethodTypes.New,
                    SiteId = siteId,
                    PermissionType = pt
                });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Editor(SiteModel siteModel)
        {
            var hb = new HtmlBuilder();
            return hb.Template(
                pt: siteModel.PermissionType,
                verType: siteModel.VerType,
                methodType: siteModel.MethodType,
                allowAccess: AllowAccess(siteModel),
                siteId: siteModel.SiteId,
                parentId: siteModel.ParentId,
                referenceType: "Sites",
                siteReferenceType: siteModel.ReferenceType,
                title: siteModel.MethodType == BaseModel.MethodTypes.New
                    ? Displays.Sites() + " - " + Displays.New()
                    : siteModel.Title + " - " + Displays.EditSettings(),
                action: () => hb
                    .Editor(siteModel: siteModel)
                    .Hidden(controlId: "BaseUrl", value: Locations.BaseUrl())
                    .Hidden(controlId: "ReferenceType", value: "Sites")).ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static bool AllowAccess(SiteModel siteModel)
        {
            if (siteModel.AccessStatus == Databases.AccessStatuses.NotFound)
            {
                return false;
            }
            switch (siteModel.MethodType)
            {
                case BaseModel.MethodTypes.New:
                    return siteModel.PermissionType.CanCreate();
                default:
                    return siteModel.PermissionType.CanEditSite();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Editor(this HtmlBuilder hb, SiteModel siteModel)
        {
            return hb.Div(id: "Editor", action: () => hb
                .Form(
                    attributes: new HtmlAttributes()
                        .Id("SiteForm")
                        .Class("main-form")
                        .Action(Locations.ItemAction(siteModel.SiteId)),
                    action: () => hb
                        .RecordHeader(
                            pt: siteModel.PermissionType,
                            baseModel: siteModel,
                            tableName: "Sites",
                            switcher: false)
                        .Div(id: "EditorComments", action: () => hb
                            .Comments(
                                comments: siteModel.Comments,
                                verType: siteModel.VerType))
                        .Div(id: "EditorTabsContainer", action: () => hb
                            .EditorTabs(siteModel: siteModel)
                            .FieldSetGeneral(siteModel: siteModel)
                            .FieldSet(
                                attributes: new HtmlAttributes()
                                    .Id("FieldSetHistories")
                                    .DataAction("Histories")
                                    .DataMethod("get"),
                                _using: siteModel.MethodType != BaseModel.MethodTypes.New)
                            .MainCommands(
                                siteId: siteModel.SiteId,
                                pt: siteModel.PermissionType,
                                verType: siteModel.VerType,
                                referenceType: "items",
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
                            css: "control-hidden must-transport",
                            value: siteModel.Timestamp)
                        .Hidden(controlId: "Id", value: siteModel.SiteId.ToString()))
                .OutgoingMailsForm("Sites", siteModel.SiteId, siteModel.Ver)
                .CopyDialog("items", siteModel.SiteId)
                .OutgoingMailDialog()
                .Div(attributes: new HtmlAttributes()
                    .Id("GridColumnDialog")
                    .Class("dialog")
                    .Title(Displays.AdvancedSetting()))
                .Div(attributes: new HtmlAttributes()
                    .Id("FilterColumnDialog")
                    .Class("dialog")
                    .Title(Displays.AdvancedSetting()))
                .Div(attributes: new HtmlAttributes()
                    .Id("EditorColumnDialog")
                    .Class("dialog")
                    .Title(Displays.AdvancedSetting()))
                .Div(attributes: new HtmlAttributes()
                    .Id("SummaryDialog")
                    .Class("dialog")
                    .Title(Displays.AdvancedSetting()))
                .Div(attributes: new HtmlAttributes()
                    .Id("FormulaDialog")
                    .Class("dialog")
                    .Title(Displays.AdvancedSetting()))
                .Div(attributes: new HtmlAttributes()
                    .Id("ViewDialog")
                    .Class("dialog")
                    .Title(Displays.DataView()))
                .Div(attributes: new HtmlAttributes()
                    .Id("NotificationDialog")
                    .Class("dialog")
                    .Title(Displays.Notifications())));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string EditorBackUrl(SiteModel siteModel)
        {
            switch (siteModel.ReferenceType)
            {
                case "Wikis":
                    var wikiId = Rds.ExecuteScalar_long(statements:
                        Rds.SelectWikis(
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
        private static HtmlBuilder FieldSetGeneral(this HtmlBuilder hb, SiteModel siteModel)
        {
            var titleColumn = siteModel.SiteSettings.GetColumn("Title");
            hb.FieldSet(id: "FieldSetGeneral", action: () =>
            {
                hb
                    .FieldText(
                        controlId: "Sites_SiteId",
                        labelText: Displays.Sites_SiteId(),
                        text: siteModel.SiteId.ToString())
                    .FieldText(
                        controlId: "Sites_Ver",
                        controlCss: siteModel.SiteSettings?.GetColumn("Ver").ControlCss,
                        labelText: Displays.Sites_Ver(),
                        text: siteModel.Ver.ToString())
                    .FieldTextBox(
                        controlId: "Sites_Title",
                        fieldCss: "field-wide",
                        controlCss: " focus",
                        labelText: Displays.Sites_Title(),
                        text: siteModel.Title.Value.ToString(),
                        validateRequired: titleColumn.ValidateRequired ?? false,
                        validateMaxLength: titleColumn.ValidateMaxLength ?? 0,
                        _using: siteModel.ReferenceType != "Wikis")
                    .FieldMarkDown(
                        controlId: "Sites_Body",
                        fieldCss: "field-wide",
                        labelText: Displays.Sites_Body(),
                        text: siteModel.Body,
                        _using: siteModel.ReferenceType != "Wikis")
                    .Field(
                        controlId: "Sites_ReferenceType",
                        labelText: Displays.Sites_ReferenceType(),
                        controlAction: () => hb
                            .ReferenceType(
                                selectedValue: siteModel.ReferenceType,
                                methodType: siteModel.MethodType))
                    .VerUpCheckBox(siteModel);
                if (siteModel.PermissionType.CanEditPermission() &&
                    siteModel.MethodType != BaseModel.MethodTypes.New)
                {
                    hb.FieldAnchor(
                        controlContainerCss: "m-l30",
                        iconCss: "ui-icon-person a",
                        text: Displays.EditPermissions(),
                        href: Locations.ItemEdit(siteModel.SiteId, "Permissions"));
                }
            });
            if (siteModel.MethodType != BaseModel.MethodTypes.New)
            {
                hb.SiteImageSettingsEditor(siteModel.SiteSettings);
                switch (siteModel.ReferenceType)
                {
                    case "Sites":
                        break;
                    case "Wikis":
                        hb
                            .NotificationsSettingsEditor(siteModel.SiteSettings)
                            .MailSettingsEditor(siteModel.SiteSettings)
                            .StylesSettingsEditor(siteModel.SiteSettings)
                            .ScriptsSettingsEditor(siteModel.SiteSettings);
                        break;
                    default:
                        hb
                            .GridSettingsEditor(siteModel.SiteSettings)
                            .FiltersSettingsEditor(siteModel.SiteSettings)
                            .AggregationsSettingsEditor(siteModel.SiteSettings)
                            .EditorSettingsEditor(siteModel.SiteSettings)
                            .LinksSettingsEditor(siteModel.SiteSettings)
                            .HistoriesSettingsEditor(siteModel.SiteSettings)
                            .SummariesSettingsEditor(siteModel.SiteSettings)
                            .FormulasSettingsEditor(siteModel.SiteSettings)
                            .ViewsSettingsEditor(siteModel.SiteSettings)
                            .NotificationsSettingsEditor(siteModel.SiteSettings)
                            .MailSettingsEditor(siteModel.SiteSettings)
                            .StylesSettingsEditor(siteModel.SiteSettings)
                            .ScriptsSettingsEditor(siteModel.SiteSettings);
                        break;
                }
            }
            return hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SiteImageSettingsEditor(this HtmlBuilder hb, SiteSettings ss)
        {
            return hb.FieldSet(id: "SiteImageSettingsEditor", action: () => hb
                .FieldSet(
                    css: " enclosed",
                    legendText: Displays.Icon(),
                    action: () => hb
                        .FieldTextBox(
                            textType: HtmlTypes.TextTypes.File,
                            controlId: "SiteImage",
                            fieldCss: "field-auto-thin",
                            controlCss: " w400",
                            labelText: Displays.File())
                        .Button(
                            controlId: "SetSiteImage",
                            controlCss: "button-icon",
                            text: Displays.Setting(),
                            onClick: "$p.uploadSiteImage($(this));",
                            icon: "ui-icon-disk",
                            action: "binaries/updatesiteimage",
                            method: "post")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder GridSettingsEditor(this HtmlBuilder hb, SiteSettings ss)
        {
            return hb.FieldSet(id: "GridSettingsEditor", action: () => hb
                .GridColumns(ss)
                .FieldSpinner(
                    controlId: "GridPageSize",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.GridPageSize(),
                    value: ss.GridPageSize.ToDecimal(),
                    min: Parameters.General.GridPageSizeMin,
                    max: Parameters.General.GridPageSizeMax,
                    step: 1,
                    width: 25)
                .FieldSpinner(
                    controlId: "NearCompletionTimeBeforeDays",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.NearCompletionTimeBeforeDays(),
                    value: ss.NearCompletionTimeBeforeDays.ToDecimal(),
                    min: Parameters.General.NearCompletionTimeBeforeDaysMin,
                    max: Parameters.General.NearCompletionTimeBeforeDaysMax,
                    step: 1,
                    width: 25)
                .FieldSpinner(
                    controlId: "NearCompletionTimeAfterDays",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.NearCompletionTimeAfterDays(),
                    value: ss.NearCompletionTimeAfterDays.ToDecimal(),
                    min: Parameters.General.NearCompletionTimeAfterDaysMin,
                    max: Parameters.General.NearCompletionTimeAfterDaysMax,
                    step: 1,
                    width: 25)
                .FieldDropDown(
                    controlId: "GridView",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.DefaultView(),
                    optionCollection: ss.ViewSelectableOptions(),
                    selectedValue: ss.GridView?.ToString(),
                    insertBlank: true,
                    _using: ss.Views?.Any() == true)
                .AggregationDetailsDialog(ss));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder GridColumns(this HtmlBuilder hb, SiteSettings ss)
        {
            return hb.FieldSet(
                css: " enclosed-thin",
                legendText: Displays.ListSettings(),
                action: () => hb
                    .FieldSelectable(
                        controlId: "GridColumns",
                        fieldCss: "field-vertical",
                        controlContainerCss: "container-selectable",
                        controlWrapperCss: " h350",
                        labelText: Displays.CurrentSettings(),
                        listItemCollection: ss.GridSelectableOptions(),
                        selectedValueCollection: new List<string>(),
                        commandOptionPositionIsTop: true,
                        commandOptionAction: () => hb
                            .Div(css: "command-center", action: () => hb
                                .Button(
                                    controlId: "MoveUpGridColumns",
                                    controlCss: "button-icon",
                                    text: Displays.MoveUp(),
                                    onClick: "$p.send($(this));",
                                    icon: "ui-icon-circle-triangle-n",
                                    action: "SetSiteSettings",
                                    method: "post")
                                .Button(
                                    controlId: "MoveDownGridColumns",
                                    controlCss: "button-icon",
                                    text: Displays.MoveDown(),
                                    onClick: "$p.send($(this));",
                                    icon: "ui-icon-circle-triangle-s",
                                    action: "SetSiteSettings",
                                    method: "post")
                                .Button(
                                    controlId: "OpenGridColumnDialog",
                                    text: Displays.AdvancedSetting(),
                                    controlCss: "button-icon",
                                    onClick: "$p.openGridColumnDialog($(this));",
                                    icon: "ui-icon-gear",
                                    action: "SetSiteSettings",
                                    method: "put")
                                .Button(
                                    controlId: "ToDisableGridColumns",
                                    controlCss: "button-icon",
                                    text: Displays.ToDisable(),
                                    onClick: "$p.send($(this));",
                                    icon: "ui-icon-circle-triangle-e",
                                    action: "SetSiteSettings",
                                    method: "put")))
                    .FieldSelectable(
                        controlId: "GridSourceColumns",
                        fieldCss: "field-vertical",
                        controlContainerCss: "container-selectable",
                        controlWrapperCss: " h350",
                        labelText: Displays.OptionList(),
                        listItemCollection: ss.GridSelectableOptions(enabled: false),
                        commandOptionPositionIsTop: true,
                        commandOptionAction: () => hb
                            .Div(css: "command-center", action: () => hb
                                .Button(
                                    controlId: "ToEnableGridColumns",
                                    text: Displays.ToEnable(),
                                    controlCss: "button-icon",
                                    onClick: "$p.send($(this));",
                                    icon: "ui-icon-circle-triangle-w",
                                    action: "SetSiteSettings",
                                    method: "put"))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder GridColumnDialog(SiteSettings ss, Column column)
        {
            var hb = new HtmlBuilder();
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("GridColumnForm")
                    .Action(Locations.ItemAction(ss.SiteId)),
                action: () => hb
                    .GridColumnDialog(ss: ss, column: column));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder GridColumnDialog(
            this HtmlBuilder hb, SiteSettings ss, Column column)
        {
            hb.FieldSet(
                css: " enclosed",
                legendText: column.LabelTextDefault,
                action: () =>
                {
                    hb.FieldTextBox(
                        controlId: "GridLabelText",
                        labelText: Displays.DisplayName(),
                        text: column.GridLabelText,
                        validateRequired: true);
                    if (column.TypeName == "datetime")
                    {
                        hb
                            .FieldDropDown(
                                controlId: "GridFormat",
                                labelText: Displays.GridFormat(),
                                optionCollection: DateTimeOptions(),
                                selectedValue: column.GridFormat);
                    }
                    hb
                        .FieldCheckBox(
                            controlId: "UseGridDesign",
                            labelText: Displays.UseCustomDesign(),
                            _checked: !column.GridDesign.IsNullOrEmpty())
                        .FieldMarkDown(
                            fieldId: "GridDesignField",
                            controlId: "GridDesign",
                            fieldCss: "field-wide" + (!column.GridDesign.IsNullOrEmpty()
                                ? string.Empty
                                : " hidden"),
                            labelText: Displays.CustomDesign(),
                            placeholder: Displays.CustomDesign(),
                            text: ss.GridDesignEditorText(column));
                });
            return hb
                .Hidden(
                    controlId: "GridColumnName",
                    css: "must-transport",
                    value: column.ColumnName)
                .P(css: "message-dialog")
                .Div(css: "command-center", action: () => hb
                    .Button(
                        controlId: "SetGridColumn",
                        text: Displays.Setting(),
                        controlCss: "button-icon validate",
                        onClick: "$p.setGridColumn($(this));",
                        icon: "ui-icon-gear",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        text: Displays.Cancel(),
                        controlCss: "button-icon",
                        onClick: "$p.closeDialog($(this));",
                        icon: "ui-icon-cancel"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder FiltersSettingsEditor(this HtmlBuilder hb, SiteSettings ss)
        {
            return hb.FieldSet(id: "FiltersSettingsEditor", action: () => hb
                .FieldSet(
                    css: " enclosed-thin",
                    legendText: Displays.ListSettings(),
                    action: () => hb
                        .FieldSelectable(
                            controlId: "FilterColumns",
                            fieldCss: "field-vertical",
                            controlContainerCss: "container-selectable",
                            controlWrapperCss: " h350",
                            labelText: Displays.CurrentSettings(),
                            listItemCollection: ss.FilterSelectableOptions(),
                            selectedValueCollection: new List<string>(),
                            commandOptionPositionIsTop: true,
                            commandOptionAction: () => hb
                                .Div(css: "command-center", action: () => hb
                                    .Button(
                                        controlId: "MoveUpFilterColumns",
                                        controlCss: "button-icon",
                                        text: Displays.MoveUp(),
                                        onClick: "$p.send($(this));",
                                        icon: "ui-icon-circle-triangle-n",
                                        action: "SetSiteSettings",
                                        method: "post")
                                    .Button(
                                        controlId: "MoveDownFilterColumns",
                                        controlCss: "button-icon",
                                        text: Displays.MoveDown(),
                                        onClick: "$p.send($(this));",
                                        icon: "ui-icon-circle-triangle-s",
                                        action: "SetSiteSettings",
                                        method: "post")
                                    .Button(
                                        controlId: "OpenFilterColumnDialog",
                                        text: Displays.AdvancedSetting(),
                                        controlCss: "button-icon",
                                        onClick: "$p.openFilterColumnDialog($(this));",
                                        icon: "ui-icon-gear",
                                        action: "SetSiteSettings",
                                        method: "put")
                                    .Button(
                                        controlId: "ToDisableFilterColumns",
                                        controlCss: "button-icon",
                                        text: Displays.ToDisable(),
                                        onClick: "$p.send($(this));",
                                        icon: "ui-icon-circle-triangle-e",
                                        action: "SetSiteSettings",
                                        method: "put")))
                        .FieldSelectable(
                            controlId: "FilterSourceColumns",
                            fieldCss: "field-vertical",
                            controlContainerCss: "container-selectable",
                            controlWrapperCss: " h350",
                            labelText: Displays.OptionList(),
                            listItemCollection: ss.FilterSelectableOptions(enabled: false),
                            commandOptionPositionIsTop: true,
                            commandOptionAction: () => hb
                                .Div(css: "command-center", action: () => hb
                                    .Button(
                                        controlId: "ToEnableFilterColumns",
                                        text: Displays.ToEnable(),
                                        controlCss: "button-icon",
                                        onClick: "$p.send($(this));",
                                        icon: "ui-icon-circle-triangle-w",
                                        action: "SetSiteSettings",
                                        method: "put")))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder FilterColumnDialog(SiteSettings ss, Column column)
        {
            var hb = new HtmlBuilder();
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("FilterColumnForm")
                    .Action(Locations.ItemAction(ss.SiteId)),
                action: () => hb
                    .FilterColumnDialog(ss: ss, column: column));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder FilterColumnDialog(
            this HtmlBuilder hb, SiteSettings ss, Column column)
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
                                controlId: "CheckFilterControlType",
                                fieldCss: "field-auto-thin",
                                labelText: Displays.ControlType(),
                                optionCollection: ColumnUtilities.CheckFilterControlTypeOptions(),
                                selectedValue: column.CheckFilterControlType.ToInt().ToString());
                            break;
                        case Types.CsNumeric:
                            hb
                                .FieldTextBox(
                                    controlId: "NumFilterMin",
                                    fieldCss: "field-auto-thin",
                                    labelText: Displays.Min(),
                                    text: column.NumFilterMin.TrimEndZero(),
                                    validateRequired: true,
                                    validateNumber: true)
                                .FieldTextBox(
                                    controlId: "NumFilterMax",
                                    fieldCss: "field-auto-thin",
                                    labelText: Displays.Max(),
                                    text: column.NumFilterMax.TrimEndZero(),
                                    validateRequired: true,
                                    validateNumber: true)
                                .FieldTextBox(
                                    controlId: "NumFilterStep",
                                    fieldCss: "field-auto-thin",
                                    labelText: Displays.Step(),
                                    text: column.NumFilterStep.TrimEndZero(),
                                    validateRequired: true,
                                    validateNumber: true);
                            break;
                        case Types.CsDateTime:
                            hb
                                .FieldTextBox(
                                    controlId: "DateFilterMinSpan",
                                    fieldCss: "field-auto-thin",
                                    labelText: Displays.Min(),
                                    text: column.DateFilterMinSpan.ToString(),
                                    validateRequired: true,
                                    validateNumber: true)
                                .FieldTextBox(
                                    controlId: "DateFilterMaxSpan",
                                    fieldCss: "field-auto-thin",
                                    labelText: Displays.Max(),
                                    text: column.DateFilterMaxSpan.ToString(),
                                    validateRequired: true,
                                    validateNumber: true)
                                .FieldCheckBox(
                                    controlId: "DateFilterFy",
                                    fieldCss: "field-auto-thin",
                                    labelText: Displays.UseFy(),
                                    _checked: column.DateFilterFy.ToBool())
                                .FieldCheckBox(
                                    controlId: "DateFilterHalf",
                                    fieldCss: "field-auto-thin",
                                    labelText: Displays.UseHalf(),
                                    _checked: column.DateFilterHalf.ToBool())
                                .FieldCheckBox(
                                    controlId: "DateFilterQuarter",
                                    fieldCss: "field-auto-thin",
                                    labelText: Displays.UseQuarter(),
                                    _checked: column.DateFilterQuarter.ToBool())
                                .FieldCheckBox(
                                    controlId: "DateFilterMonth",
                                    fieldCss: "field-auto-thin",
                                    labelText: Displays.UseMonth(),
                                    _checked: column.DateFilterMonth.ToBool());
                            break;
                    }
                });
            return hb
                .Hidden(
                    controlId: "FilterColumnName",
                    css: "must-transport",
                    value: column.ColumnName)
                .P(css: "message-dialog")
                .Div(css: "command-center", action: () => hb
                    .Button(
                        controlId: "SetFilterColumn",
                        text: Displays.Setting(),
                        controlCss: "button-icon validate",
                        onClick: "$p.send($(this));",
                        icon: "ui-icon-gear",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        text: Displays.Cancel(),
                        controlCss: "button-icon",
                        onClick: "$p.closeDialog($(this));",
                        icon: "ui-icon-cancel"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder AggregationsSettingsEditor(this HtmlBuilder hb, SiteSettings ss)
        {
            return hb.FieldSet(id: "AggregationsSettingsEditor", action: () => hb
                .FieldSet(
                    css: " enclosed-thin",
                    legendText: Displays.AggregationSettings(),
                    action: () => hb
                        .FieldSelectable(
                            controlId: "AggregationDestination",
                            fieldCss: "field-vertical both",
                            controlContainerCss: "container-selectable",
                            controlWrapperCss: " h350",
                            labelText: Displays.CurrentSettings(),
                            listItemCollection: ss.AggregationDestination(),
                            selectedValueCollection: new List<string>(),
                            commandOptionPositionIsTop: true,
                            commandOptionAction: () => hb
                                .Div(css: "command-center", action: () => hb
                                    .Button(
                                        controlId: "MoveUpAggregations",
                                        controlCss: "button-icon",
                                        text: Displays.MoveUp(),
                                        onClick: "$p.send($(this));",
                                        icon: "ui-icon-circle-triangle-n",
                                        action: "SetSiteSettings",
                                        method: "post")
                                    .Button(
                                        controlId: "MoveDownAggregations",
                                        controlCss: "button-icon",
                                        text: Displays.MoveDown(),
                                        onClick: "$p.send($(this));",
                                        icon: "ui-icon-circle-triangle-s",
                                        action: "SetSiteSettings",
                                        method: "post")
                                    .Button(
                                        text: Displays.AdvancedSetting(),
                                        controlCss: "button-icon open-dialog",
                                        onClick: "$p.openDialog($(this), '.main-form');",
                                        icon: "ui-icon-gear",
                                        selector: "#AggregationDetailsDialog")
                                    .Button(
                                        controlId: "DeleteAggregations",
                                        controlCss: "button-icon",
                                        text: Displays.Delete(),
                                        onClick: "$p.send($(this));",
                                        icon: "ui-icon-circle-triangle-e",
                                        action: "SetSiteSettings",
                                        method: "put")))
                        .FieldSelectable(
                            controlId: "AggregationSource",
                            fieldCss: "field-vertical",
                            controlContainerCss: "container-selectable",
                            controlWrapperCss: " h350",
                            labelText: Displays.OptionList(),
                            listItemCollection: ss.AggregationSource(),
                            selectedValueCollection: new List<string>(),
                            commandOptionPositionIsTop: true,
                            commandOptionAction: () => hb
                                .Div(css: "command-center", action: () => hb
                                    .Button(
                                        controlId: "AddAggregations",
                                        controlCss: "button-icon",
                                        text: Displays.Add(),
                                        onClick: "$p.send($(this));",
                                        icon: "ui-icon-circle-triangle-w",
                                        action: "SetSiteSettings",
                                        method: "post")))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder AggregationDetailsDialog(this HtmlBuilder hb, SiteSettings ss)
        {
            return hb.Div(
                attributes: new HtmlAttributes()
                    .Id("AggregationDetailsDialog")
                    .Class("dialog")
                    .Title(Displays.AggregationDetails()),
                action: () => hb
                    .FieldDropDown(
                        controlId: "AggregationType",
                        labelText: Displays.AggregationType(),
                        optionCollection: new Dictionary<string, string>
                        {
                            { "Count", Displays.Count() },
                            { "Total", Displays.Total() },
                            { "Average", Displays.Average() }
                        })
                    .FieldDropDown(
                        controlId: "AggregationTarget",
                        fieldCss: " hidden togglable",
                        labelText: Displays.AggregationTarget(),
                        optionCollection: Def.ColumnDefinitionCollection
                            .Where(o => o.TableName == ss.ReferenceType)
                            .Where(o => o.Computable)
                            .Where(o => o.TypeName != "datetime")
                            .ToDictionary(
                                o => o.ColumnName,
                                o => ss.GetColumn(o.ColumnName).LabelText))
                    .P(css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "SetAggregationDetails",
                            text: Displays.Setting(),
                            controlCss: "button-icon",
                            onClick: "$p.setAggregationDetails($(this));",
                            icon: "ui-icon-gear",
                            action: "SetSiteSettings",
                            method: "post")
                        .Button(
                            text: Displays.Cancel(),
                            controlCss: "button-icon",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditorSettingsEditor(this HtmlBuilder hb, SiteSettings ss)
        {
            return hb.FieldSet(id: "EditorSettingsEditor", action: () => hb
                .FieldSet(
                    css: " enclosed",
                    legendText: Displays.EditorSettings(),
                    action: () => hb
                        .FieldSelectable(
                            controlId: "EditorColumns",
                            fieldCss: "field-vertical",
                            controlContainerCss: "container-selectable",
                            controlWrapperCss: " h350",
                            labelText: Displays.CurrentSettings(),
                            listItemCollection: ss.EditorSelectableOptions(),
                            commandOptionPositionIsTop: true,
                            commandOptionAction: () => hb
                                .Div(css: "command-center", action: () => hb
                                    .Button(
                                        controlId: "MoveUpEditorColumns",
                                        text: Displays.MoveUp(),
                                        controlCss: "button-icon",
                                        onClick: "$p.send($(this));",
                                        icon: "ui-icon-circle-triangle-n",
                                        action: "SetSiteSettings",
                                        method: "post")
                                    .Button(
                                        controlId: "MoveDownEditorColumns",
                                        text: Displays.MoveDown(),
                                        controlCss: "button-icon",
                                        onClick: "$p.send($(this));",
                                        icon: "ui-icon-circle-triangle-s",
                                        action: "SetSiteSettings",
                                        method: "post")
                                    .Button(
                                        controlId: "OpenEditorColumnDialog",
                                        text: Displays.AdvancedSetting(),
                                        controlCss: "button-icon",
                                        onClick: "$p.openEditorColumnDialog($(this));",
                                        icon: "ui-icon-gear",
                                        action: "SetSiteSettings",
                                        method: "put")
                                    .Button(
                                        controlId: "ToDisableEditorColumns",
                                        text: Displays.ToDisable(),
                                        controlCss: "button-icon",
                                        onClick: "$p.send($(this));",
                                        icon: "ui-icon-circle-triangle-e",
                                        action: "SetSiteSettings",
                                        method: "put")))
                        .FieldSelectable(
                            controlId: "EditorSourceColumns",
                            fieldCss: "field-vertical",
                            controlContainerCss: "container-selectable",
                            controlWrapperCss: " h350",
                            labelText: Displays.OptionList(),
                            listItemCollection: ss.EditorSelectableOptions(enabled: false),
                            commandOptionPositionIsTop: true,
                            commandOptionAction: () => hb
                                .Div(css: "command-center", action: () => hb
                                    .Button(
                                        controlId: "ToEnableEditorColumns",
                                        text: Displays.ToEnable(),
                                        controlCss: "button-icon",
                                        onClick: "$p.send($(this));",
                                        icon: "ui-icon-circle-triangle-w",
                                        action: "SetSiteSettings",
                                        method: "put")))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditorColumnDialog(
            SiteSettings ss, Column column, IEnumerable<string> titleColumns)
        {
            var hb = new HtmlBuilder();
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("EditorColumnForm")
                    .Action(Locations.ItemAction(ss.SiteId)),
                action: () => hb
                    .EditorColumnDialog(ss: ss, column: column, titleColumns: titleColumns));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditorColumnDialog(
            this HtmlBuilder hb, SiteSettings ss, Column column, IEnumerable<string> titleColumns)
        {
            hb.FieldSet(
                css: " enclosed",
                legendText: column.LabelTextDefault,
                action: () =>
                {
                    hb.FieldTextBox(
                        controlId: "LabelText",
                        labelText: Displays.DisplayName(),
                        text: column.LabelText,
                        validateRequired: true);
                    switch (column.TypeName.CsTypeSummary())
                    {
                        case Types.CsBool:
                            break;
                        default:
                            hb
                                .FieldDropDown(
                                    controlId: "FieldCss",
                                    labelText: Displays.Style(),
                                    optionCollection: new Dictionary<string, string>
                                    {
                                            { "field-normal", Displays.Normal() },
                                            { "field-wide", Displays.Wide() },
                                            { "field-auto", Displays.Auto() }
                                    },
                                    selectedValue: column.FieldCss,
                                    _using: !column.MarkDown)
                                .FieldCheckBox(
                                    controlId: "ValidateRequired",
                                    labelText: Displays.Required(),
                                    _checked: column.ValidateRequired ?? false,
                                    disabled: !column.Nullable,
                                    _using: !column.Id_Ver);
                            break;
                    }
                    hb.FieldCheckBox(
                        controlId: "EditorReadOnly",
                        labelText: Displays.ReadOnly(),
                        _checked: column.EditorReadOnly.ToBool(),
                        _using: column.Nullable);
                    if (column.TypeName == "datetime")
                    {
                        hb
                            .FieldDropDown(
                                controlId: "EditorFormat",
                                labelText: Displays.EditorFormat(),
                                optionCollection: DateTimeOptions(forControl: true),
                                selectedValue: column.EditorFormat)
                            .FieldDropDown(
                                controlId: "ExportFormat",
                                labelText: Displays.ExportFormat(),
                                optionCollection: DateTimeOptions(),
                                selectedValue: column.ExportFormat);
                    }
                    switch (column.TypeName.CsTypeSummary())
                    {
                        case Types.CsBool:
                            hb.FieldCheckBox(
                                controlId: "DefaultInput",
                                labelText: Displays.DefaultInput(),
                                _checked: column.DefaultInput.ToBool());
                            break;
                        case Types.CsNumeric:
                            if (column.ControlType != "ChoicesText")
                            {
                                var maxDecimalPlaces = MaxDecimalPlaces(column);
                                hb
                                    .FieldTextBox(
                                        controlId: "DefaultInput",
                                        labelText: Displays.DefaultInput(),
                                        text: column.DefaultInput.ToLong().ToString(),
                                        validateNumber: true,
                                        _using: !column.Id_Ver)
                                    .EditorColumnFormatProperties(column: column)
                                    .FieldTextBox(
                                        controlId: "Unit",
                                        controlCss: " w50",
                                        labelText: Displays.Unit(),
                                        text: column.Unit,
                                        _using: !column.Id_Ver)
                                    .FieldSpinner(
                                        controlId: "DecimalPlaces",
                                        labelText: Displays.DecimalPlaces(),
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
                                            controlId: "ControlType",
                                            labelText: Displays.ControlType(),
                                            optionCollection: new Dictionary<string, string>
                                            {
                                                { "Normal", Displays.Normal() },
                                                { "Spinner", Displays.Spinner() }
                                            },
                                            selectedValue: column.ControlType)
                                        .FieldTextBox(
                                            fieldId: "MinField",
                                            controlId: "Min",
                                            fieldCss: " both" + hidden,
                                            labelText: Displays.Min(),
                                            text: column.Min.ToString())
                                        .FieldTextBox(
                                            fieldId: "MaxField",
                                            controlId: "Max",
                                            fieldCss: hidden,
                                            labelText: Displays.Max(),
                                            text: column.Max.ToString())
                                        .FieldTextBox(
                                            fieldId: "StepField",
                                            controlId: "Step",
                                            fieldCss: hidden,
                                            labelText: Displays.Step(),
                                            text: column.Step.ToString());
                                }
                            }
                            break;
                        case Types.CsDateTime:
                            hb.FieldSpinner(
                                controlId: "DefaultInput",
                                controlCss: " allow-blank",
                                labelText: Displays.DefaultInput(),
                                value: column.DefaultInput != string.Empty
                                    ? column.DefaultInput.ToDecimal()
                                    : (decimal?)null,
                                min: column.Min.ToInt(),
                                max: column.Max.ToInt(),
                                step: column.Step.ToInt(),
                                width: column.Width);
                            break;
                        case Types.CsString:
                            if (column.MarkDown)
                            {
                                hb.FieldTextBox(
                                    textType: HtmlTypes.TextTypes.MultiLine,
                                    controlId: "DefaultInput",
                                    fieldCss: column.FieldCss,
                                    labelText: Displays.DefaultInput(),
                                    text: column.DefaultInput);
                            }
                            else
                            {
                                hb.FieldTextBox(
                                    controlId: "DefaultInput",
                                    fieldCss: column.FieldCss,
                                    labelText: Displays.DefaultInput(),
                                    text: column.DefaultInput);
                            }
                            break;
                    }
                    switch (column.ControlType)
                    {
                        case "ChoicesText":
                            hb.FieldTextBox(
                                textType: HtmlTypes.TextTypes.MultiLine,
                                controlId: "ChoicesText",
                                fieldCss: "field-wide",
                                labelText: Displays.OptionList(),
                                text: column.ChoicesText);
                            break;
                        default:
                            break;
                    }
                    if (column.ColumnName == "Title")
                    {
                        hb.EditorColumnTitleProperties(ss, titleColumns);
                    }
                });
            return hb
                .Hidden(
                    controlId: "EditorColumnName",
                    css: "must-transport",
                    value: column.ColumnName)
                .P(css: "message-dialog")
                .Div(css: "command-center", action: () => hb
                    .Button(
                        controlId: "SetEditorColumn",
                        text: Displays.Setting(),
                        controlCss: "button-icon validate",
                        onClick: "$p.send($(this));",
                        icon: "ui-icon-gear",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        text: Displays.Cancel(),
                        controlCss: "button-icon",
                        onClick: "$p.closeDialog($(this));",
                        icon: "ui-icon-cancel"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditorColumnFormatProperties(this HtmlBuilder hb, Column column)
        {
            var formats = Parameters.Formats
                .Where(o => (o.Type & ParameterAccessor.Parts.Format.Types.NumColumn) > 0);
            var format = formats.FirstOrDefault(o => o.String == column.Format);
            var other = !column.Format.IsNullOrEmpty() && format == null;
            return hb
                .FieldDropDown(
                    controlId: "FormatSelector",
                    controlCss: " not-transport",
                    labelText: Displays.Format(),
                    optionCollection: formats
                        .ToDictionary(o => o.String, o => Displays.Get(o.Name)),
                    selectedValue: format != null
                        ? format.String
                        : other
                            ? "\t"
                            : string.Empty,
                    insertBlank: true,
                    _using: !column.Id_Ver)
                .FieldTextBox(
                    fieldId: "FormatField",
                    controlId: "Format",
                    fieldCss: other ? string.Empty : " hidden",
                    labelText: Displays.Custom(),
                    text: other
                        ? column.Format
                        : string.Empty);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditorColumnTitleProperties(
            this HtmlBuilder hb, SiteSettings ss, IEnumerable<string> titleColumns)
        {
            return hb
                .FieldSelectable(
                    controlId: "TitleColumns",
                    fieldCss: "field-vertical",
                    controlContainerCss: "container-selectable",
                    controlWrapperCss: " h200",
                    labelText: Displays.CurrentSettings(),
                    listItemCollection: ss
                        .TitleSelectableOptions(titleColumns),
                    commandOptionPositionIsTop: true,
                    commandOptionAction: () => hb
                        .Div(css: "command-center", action: () => hb
                            .Button(
                                controlId: "MoveUpTitleColumns",
                                text: Displays.MoveUp(),
                                controlCss: "button-icon",
                                onClick: "$p.send($(this));",
                                icon: "ui-icon-circle-triangle-n",
                                action: "SetSiteSettings",
                                method: "post")
                            .Button(
                                controlId: "MoveDownTitleColumns",
                                text: Displays.MoveDown(),
                                controlCss: "button-icon",
                                onClick: "$p.send($(this));",
                                icon: "ui-icon-circle-triangle-s",
                                action: "SetSiteSettings",
                                method: "post")
                            .Button(
                                controlId: "ToDisableTitleColumns",
                                text: Displays.ToDisable(),
                                controlCss: "button-icon",
                                onClick: "$p.send($(this));",
                                icon: "ui-icon-circle-triangle-e",
                                action: "SetSiteSettings",
                                method: "put")))
                .FieldSelectable(
                    controlId: "TitleSourceColumns",
                    fieldCss: "field-vertical",
                    controlContainerCss: "container-selectable",
                    controlWrapperCss: " h200",
                    labelText: Displays.OptionList(),
                    listItemCollection: ss
                        .TitleSelectableOptions(titleColumns, enabled: false),
                    commandOptionPositionIsTop: true,
                    commandOptionAction: () => hb
                        .Div(css: "command-center", action: () => hb
                            .Button(
                                controlId: "ToEnableTitleColumns",
                                text: Displays.ToEnable(),
                                controlCss: "button-icon",
                                onClick: "$p.send($(this));",
                                icon: "ui-icon-circle-triangle-w",
                                action: "SetSiteSettings",
                                method: "put")))
                .FieldTextBox(
                    controlId: "TitleSeparator",
                    fieldCss: " both",
                    labelText: Displays.TitleSeparator(),
                    text: ss.TitleSeparator);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static Dictionary<string, string> DateTimeOptions(bool forControl = false)
        {
            return forControl
                ? DisplayAccessor.Displays.DisplayHash
                    .Where(o => new string[] { "Ymd", "Ymdhm", "Ymdhms" }.Contains(o.Key))
                    .ToDictionary(o => o.Key, o => Displays.Get(o.Key))
                : DisplayAccessor.Displays.DisplayHash
                    .Where(o => o.Value.Type == DisplayAccessor.Displays.Types.Date)
                    .ToDictionary(o => o.Key, o => Displays.Get(o.Key));
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
        private static HtmlBuilder LinksSettingsEditor(this HtmlBuilder hb, SiteSettings ss)
        {
            return hb.FieldSet(id: "LinksSettingsEditor", action: () => hb
                .FieldSet(
                    css: " enclosed",
                    legendText: Displays.ListSettings(),
                    action: () => hb
                        .FieldSelectable(
                            controlId: "LinkColumns",
                            fieldCss: "field-vertical",
                            controlContainerCss: "container-selectable",
                            controlWrapperCss: " h350",
                            labelText: Displays.CurrentSettings(),
                            listItemCollection: ss.LinkSelectableOptions(),
                            commandOptionPositionIsTop: true,
                            commandOptionAction: () => hb
                                .Div(css: "command-center", action: () => hb
                                    .Button(
                                        controlId: "MoveUpLinkColumns",
                                        text: Displays.MoveUp(),
                                        controlCss: "button-icon",
                                        onClick: "$p.send($(this));",
                                        icon: "ui-icon-circle-triangle-n",
                                        action: "SetSiteSettings",
                                        method: "post")
                                    .Button(
                                        controlId: "MoveDownLinkColumns",
                                        text: Displays.MoveDown(),
                                        controlCss: "button-icon",
                                        onClick: "$p.send($(this));",
                                        icon: "ui-icon-circle-triangle-s",
                                        action: "SetSiteSettings",
                                        method: "post")
                                    .Button(
                                        controlId: "ToDisableLinkColumns",
                                        text: Displays.ToDisable(),
                                        controlCss: "button-icon",
                                        onClick: "$p.send($(this));",
                                        icon: "ui-icon-circle-triangle-e",
                                        action: "SetSiteSettings",
                                        method: "put")))
                        .FieldSelectable(
                            controlId: "LinkSourceColumns",
                            fieldCss: "field-vertical",
                            controlContainerCss: "container-selectable",
                            controlWrapperCss: " h350",
                            labelText: Displays.OptionList(),
                            listItemCollection: ss.LinkSelectableOptions(enabled: false),
                            commandOptionPositionIsTop: true,
                            commandOptionAction: () => hb
                                .Div(css: "command-center", action: () => hb
                                    .Button(
                                        controlId: "ToEnableLinkColumns",
                                        text: Displays.ToEnable(),
                                        controlCss: "button-icon",
                                        onClick: "$p.send($(this));",
                                        icon: "ui-icon-circle-triangle-w",
                                        action: "SetSiteSettings",
                                        method: "put")))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder HistoriesSettingsEditor(this HtmlBuilder hb, SiteSettings ss)
        {
            return hb.FieldSet(id: "HistoriesSettingsEditor", action: () => hb
                .FieldSet(
                    css: " enclosed",
                    legendText: Displays.ListSettings(),
                    action: () => hb
                        .FieldSelectable(
                            controlId: "HistoryColumns",
                            fieldCss: "field-vertical",
                            controlContainerCss: "container-selectable",
                            controlWrapperCss: " h350",
                            labelText: Displays.CurrentSettings(),
                            listItemCollection: ss.HistorySelectableOptions(),
                            commandOptionPositionIsTop: true,
                            commandOptionAction: () => hb
                                .Div(css: "command-center", action: () => hb
                                    .Button(
                                        controlId: "MoveUpHistoryColumns",
                                        text: Displays.MoveUp(),
                                        controlCss: "button-icon",
                                        onClick: "$p.send($(this));",
                                        icon: "ui-icon-circle-triangle-n",
                                        action: "SetSiteSettings",
                                        method: "post")
                                    .Button(
                                        controlId: "MoveDownHistoryColumns",
                                        text: Displays.MoveDown(),
                                        controlCss: "button-icon",
                                        onClick: "$p.send($(this));",
                                        icon: "ui-icon-circle-triangle-s",
                                        action: "SetSiteSettings",
                                        method: "post")
                                    .Button(
                                        controlId: "ToDisableHistoryColumns",
                                        text: Displays.ToDisable(),
                                        controlCss: "button-icon",
                                        onClick: "$p.send($(this));",
                                        icon: "ui-icon-circle-triangle-e",
                                        action: "SetSiteSettings",
                                        method: "put")))
                        .FieldSelectable(
                            controlId: "HistorySourceColumns",
                            fieldCss: "field-vertical",
                            controlContainerCss: "container-selectable",
                            controlWrapperCss: " h350",
                            labelText: Displays.OptionList(),
                            listItemCollection: ss.HistorySelectableOptions(enabled: false),
                            commandOptionPositionIsTop: true,
                            commandOptionAction: () => hb
                                .Div(css: "command-center", action: () => hb
                                    .Button(
                                        controlId: "ToEnableHistoryColumns",
                                        text: Displays.ToEnable(),
                                        controlCss: "button-icon",
                                        onClick: "$p.send($(this));",
                                        icon: "ui-icon-circle-triangle-w",
                                        action: "SetSiteSettings",
                                        method: "put")))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SummariesSettingsEditor(this HtmlBuilder hb, SiteSettings ss)
        {
            return hb.FieldSet(id: "SummariesSettingsEditor", action: () => hb
                .Div(css: "command-left", action: () => hb
                    .Button(
                        controlId: "MoveUpSummaries",
                        controlCss: "button-icon",
                        text: Displays.MoveUp(),
                        onClick: "$p.send($(this));",
                        icon: "ui-icon-circle-triangle-n",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "MoveDownSummaries",
                        controlCss: "button-icon",
                        text: Displays.MoveDown(),
                        onClick: "$p.send($(this));",
                        icon: "ui-icon-circle-triangle-s",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "NewSummary",
                        text: Displays.New(),
                        controlCss: "button-icon",
                        onClick: "$p.openSummaryDialog($(this));",
                        icon: "ui-icon-gear",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "DeleteSummaries",
                        controlCss: "button-icon",
                        text: Displays.Delete(),
                        onClick: "$p.send($(this));",
                        icon: "ui-icon-trash",
                        action: "SetSiteSettings",
                        method: "post",
                        confirm: Displays.ConfirmDelete())
                    .Button(
                        controlId: "SynchronizeSummaries",
                        controlCss: "button-icon",
                        text: Displays.Synchronize(),
                        onClick: "$p.send($(this));",
                        icon: "ui-icon-refresh",
                        action: "SynchronizeSummaries",
                        method: "put",
                        confirm: Displays.ConfirmSynchronize()))
                .EditSummary(ss: ss));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder SummaryDialog(SiteSettings ss, string controlId, Summary summary)
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
                        controlCss: " must-transport",
                        labelText: Displays.Id(),
                        text: summary.Id.ToString(),
                        _using: controlId == "EditSummary")
                    .FieldSet(
                        css: "fieldset enclosed-half h250 both",
                        legendText: Displays.DataStorageDestination(),
                        action: () => hb
                            .FieldDropDown(
                                controlId: "SummarySiteId",
                                controlCss: " auto-postback must-transport",
                                labelText: Displays.Sites(),
                                optionCollection: destinationSiteHash,
                                action: "SetSiteSettings",
                                method: "post")
                            .SummaryDestinationColumn(
                                destinationSs: destinationSs,
                                destinationColumn: summary.DestinationColumn)
                            .FieldDropDown(
                                controlId: "SummaryDestinationCondition",
                                controlCss: " must-transport",
                                labelText: Displays.Condition(),
                                optionCollection: destinationSs.ViewSelectableOptions(),
                                selectedValue: summary.DestinationCondition.ToString(),
                                insertBlank: true,
                                _using: destinationSs.Views?.Any() == true)
                            .FieldCheckBox(
                                fieldId: "SummarySetZeroWhenOutOfConditionField",
                                controlId: "SummarySetZeroWhenOutOfCondition",
                                fieldCss: destinationSs.Views?.Any(o =>
                                    o.Id == summary.DestinationCondition) == true
                                        ? null
                                        : " hidden",
                                controlCss: " must-transport",
                                labelText: Displays.SetZeroWhenOutOfCondition(),
                                _checked: summary.SetZeroWhenOutOfCondition == true))
                    .FieldSet(
                        css: "fieldset enclosed-half h250",
                        legendText: ss.Title,
                        action: () => hb
                            .SummaryLinkColumn(
                                ss: ss,
                                siteId: summary.SiteId,
                                linkColumn: summary.LinkColumn)
                            .FieldDropDown(
                                controlId: "SummaryType",
                                controlCss: " auto-postback must-transport",
                                labelText: Displays.SummaryType(),
                                optionCollection: SummaryTypeCollection(),
                                selectedValue: summary.Type,
                                action: "SetSiteSettings",
                                method: "post")
                            .SummarySourceColumn(ss, summary.Type, summary.SourceColumn)
                            .FieldDropDown(
                                controlId: "SummarySourceCondition",
                                controlCss: " must-transport",
                                labelText: Displays.Condition(),
                                optionCollection: ss.ViewSelectableOptions(),
                                selectedValue: summary.SourceCondition.ToString(),
                                insertBlank: true,
                                _using: ss.Views?.Any() == true))
                    .P(css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "AddSummary",
                            text: Displays.Setting(),
                            controlCss: "button-icon validate",
                            onClick: "$p.setSummary($(this));",
                            icon: "ui-icon-disk",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "NewSummary")
                        .Button(
                            controlId: "UpdateSummary",
                            text: Displays.Setting(),
                            controlCss: "button-icon validate",
                            onClick: "$p.setSummary($(this));",
                            icon: "ui-icon-disk",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "EditSummary")
                        .Button(
                            text: Displays.Cancel(),
                            controlCss: "button-icon",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder SummaryDestinationColumn(
            this HtmlBuilder hb, SiteSettings destinationSs, string destinationColumn = null)
        {
            return hb.FieldDropDown(
                fieldId: "SummaryDestinationColumnField",
                controlId: "SummaryDestinationColumn",
                controlCss: " must-transport",
                labelText: Displays.Column(),
                optionCollection: destinationSs?.Columns
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
        private static Dictionary<string, string> SummarySiteIdHash(
            EnumerableRowCollection<DataRow> summarySiteCollection, SiteSettings ss)
        {
            return summarySiteCollection
                .OrderBy(o =>
                    o["SiteId"].ToLong() != ss.SiteId)
                .ToDictionary(
                    o => o["SiteId"].ToString(),
                    o => o["Title"].ToString());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static Dictionary<string, string> SummaryTypeCollection()
        {
            return new Dictionary<string, string>
            {
                { "Count", Displays.Count() },
                { "Total", Displays.Total() },
                { "Average", Displays.Average() },
                { "Min", Displays.Min() },
                { "Max", Displays.Max() }
            };
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder SummaryLinkColumn(
            this HtmlBuilder hb, SiteSettings ss, long siteId, string linkColumn = null)
        {
            return hb.FieldDropDown(
                fieldId: "SummaryLinkColumnField",
                controlId: "SummaryLinkColumn",
                controlCss: " must-transport",
                labelText: Displays.SummaryLinkColumn(),
                optionCollection: ss.Links
                    .Where(o => o.SiteId == siteId)
                    .ToDictionary(
                        o => o.ColumnName,
                        o => ss.GetColumn(o.ColumnName).LabelText),
                selectedValue: linkColumn,
                action: "SetSiteSettings",
                method: "post");
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder SummarySourceColumn(
            this HtmlBuilder hb,
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
                        fieldId: "SummarySourceColumnField",
                        controlId: "SummarySourceColumn",
                        controlCss: " must-transport",
                        labelText: Displays.SummarySourceColumn(),
                        optionCollection: ss.Columns
                            .Where(o => o.Computable)
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
        public static HtmlBuilder EditSummary(this HtmlBuilder hb, SiteSettings ss)
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
                        .SummariesHeader(ss: ss, selected: selected)
                        .SummariesBody(ss: ss, selected: selected));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SummariesHeader(
            this HtmlBuilder hb, SiteSettings ss, IEnumerable<int> selected)
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
                            .Text(text: Displays.Id()))
                    .Th(attributes: new HtmlAttributes()
                            .Colspan(3),
                        action: () => hb
                            .Text(text: Displays.DataStorageDestination()))
                    .Th(attributes: new HtmlAttributes()
                            .Colspan(4),
                        action: () => hb
                            .Text(text: ss.Title)))
                .Tr(css: "ui-widget-header", action: () => hb
                    .Th(action: () => hb
                        .Text(text: Displays.Sites()))
                    .Th(action: () => hb
                        .Text(text: Displays.Column()))
                    .Th(action: () => hb
                        .Text(text: Displays.Condition()))
                    .Th(action: () => hb
                        .Text(text: Displays.SummaryLinkColumn()))
                    .Th(action: () => hb
                        .Text(text: Displays.SummaryType()))
                    .Th(action: () => hb
                        .Text(text: Displays.SummarySourceColumn()))
                    .Th(action: () => hb
                        .Text(text: Displays.Condition()))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SummariesBody(
            this HtmlBuilder hb, SiteSettings ss, IEnumerable<int> selected)
        {
            if (ss.Summaries?.Any() == true)
            {
                var dataRows = Rds.ExecuteTable(statements:
                    Rds.SelectSites(
                        column: Rds.SitesColumn()
                            .SiteId()
                            .ReferenceType()
                            .Title()
                            .SiteSettings(),
                        where: Rds.SitesWhere()
                            .TenantId(Sessions.TenantId())
                            .SiteId_In(ss.Summaries?
                                .Select(o => o.SiteId)))).AsEnumerable();
                hb.TBody(action: () =>
                {
                    ss.Summaries?.ForEach(summary =>
                    {
                        var dataRow = dataRows.FirstOrDefault(o =>
                            o["SiteId"].ToLong() == summary.SiteId);
                        var destinationSs = dataRow?["SiteSettings"]
                            .ToString()
                            .Deserialize<SiteSettings>() ??
                                SiteSettingsUtility.Get(
                                    dataRow["SiteId"].ToLong(),
                                    dataRow["ReferenceType"].ToString());
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
                                            summary.DestinationColumn)?.LabelText))
                                    .Td(action: () => hb
                                        .Text(text: destinationSs.Views?.Get(
                                            summary.DestinationCondition)?.Name))
                                    .Td(action: () => hb
                                        .Text(text: ss.GetColumn(
                                            summary.LinkColumn)?.LabelText))
                                    .Td(action: () => hb
                                        .Text(text: SummaryType(summary.Type)))
                                    .Td(action: () => hb
                                        .Text(text: ss.GetColumn(
                                            summary.SourceColumn)?.LabelText))
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
        private static string SummaryType(string type)
        {
            switch (type)
            {
                case "Count": return Displays.Count();
                case "Total": return Displays.Total();
                case "Average": return Displays.Average();
                case "Min": return Displays.Min();
                case "Max": return Displays.Max();
                default: return string.Empty;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder FormulasSettingsEditor(this HtmlBuilder hb, SiteSettings ss)
        {
            return hb.FieldSet(id: "FormulasSettingsEditor", action: () => hb
                .FieldSelectable(
                    controlId: "Formulas",
                    fieldCss: "field-vertical w600",
                    controlContainerCss: "container-selectable",
                    controlWrapperCss: " h200",
                    listItemCollection: ss.FormulaItemCollection(),
                    commandOptionPositionIsTop: true,
                    commandOptionAction: () => hb
                        .Div(css: "command-left", action: () => hb
                            .Button(
                                controlId: "MoveUpFormulas",
                                controlCss: "button-icon",
                                text: Displays.MoveUp(),
                                onClick: "$p.send($(this));",
                                icon: "ui-icon-circle-triangle-n",
                                action: "SetSiteSettings",
                                method: "post")
                            .Button(
                                controlId: "MoveDownFormulas",
                                controlCss: "button-icon",
                                text: Displays.MoveDown(),
                                onClick: "$p.send($(this));",
                                icon: "ui-icon-circle-triangle-s",
                                action: "SetSiteSettings",
                                method: "post")
                            .Button(
                                controlId: "NewFormula",
                                text: Displays.New(),
                                controlCss: "button-icon",
                                onClick: "$p.openFormulaDialog($(this));",
                                icon: "ui-icon-gear",
                                action: "SetSiteSettings",
                                method: "post")
                            .Button(
                                controlId: "EditFormula",
                                text: Displays.AdvancedSetting(),
                                controlCss: "button-icon",
                                onClick: "$p.openFormulaDialog($(this));",
                                icon: "ui-icon-gear",
                                action: "SetSiteSettings",
                                method: "put")
                            .Button(
                                controlId: "DeleteFormulas",
                                controlCss: "button-icon",
                                text: Displays.Delete(),
                                onClick: "$p.send($(this));",
                                icon: "ui-icon-trash",
                                action: "SetSiteSettings",
                                method: "post",
                                confirm: Displays.ConfirmDelete())
                            .Button(
                                controlId: "SynchronizeFormulas",
                                controlCss: "button-icon",
                                text: Displays.Synchronize(),
                                onClick: "$p.send($(this));",
                                icon: "ui-icon-refresh",
                                action: "SynchronizeFormulas",
                                method: "put",
                                confirm: Displays.ConfirmSynchronize()))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder FormulaDialog(
            SiteSettings ss, string controlId, FormulaSet formulaSet)
        {
            var hb = new HtmlBuilder();
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("FormulaForm")
                    .Action(Locations.ItemAction(ss.SiteId)),
                action: () => hb
                    .FieldText(
                        controlId: "FormulaId",
                        controlCss: " must-transport",
                        labelText: Displays.Id(),
                        text: formulaSet.Id.ToString(),
                        _using: controlId == "EditFormula")
                    .FieldDropDown(
                        controlId: "FormulaTarget",
                        controlCss: " must-transport",
                        labelText: Displays.Target(),
                        optionCollection: ss.FormulaTargetSelectableOptions(),
                        selectedValue: formulaSet.Target?.ToString())
                    .FieldTextBox(
                        controlId: "Formula",
                        controlCss: " must-transport",
                        fieldCss: "field-wide",
                        labelText: Displays.Formulas(),
                        text: formulaSet.Formula?.ToString(ss),
                        validateRequired: true)
                    .FieldDropDown(
                        controlId: "FormulaCondition",
                        controlCss: " must-transport",
                        labelText: Displays.Condition(),
                        optionCollection: ss.ViewSelectableOptions(),
                        selectedValue: formulaSet.Condition?.ToString(),
                        insertBlank: true,
                        _using: ss.Views?.Any() == true)
                    .FieldTextBox(
                        fieldId: "FormulaOutOfConditionField",
                        controlId: "FormulaOutOfCondition",
                        controlCss: " must-transport",
                        fieldCss: "field-wide" + (ss.Views?
                            .Any(o => o.Id == formulaSet.Condition) == true
                                ? string.Empty
                                : " hidden"),
                        labelText: Displays.OutOfCondition(),
                        text: formulaSet.OutOfCondition?.ToString(ss))
                    .P(css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "AddFormula",
                            text: Displays.Setting(),
                            controlCss: "button-icon validate",
                            onClick: "$p.send($(this));",
                            icon: "ui-icon-disk",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "NewFormula")
                        .Button(
                            controlId: "UpdateFormula",
                            text: Displays.Setting(),
                            controlCss: "button-icon validate",
                            onClick: "$p.send($(this));",
                            icon: "ui-icon-disk",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "EditFormula")
                        .Button(
                            text: Displays.Cancel(),
                            controlCss: "button-icon",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ViewsSettingsEditor(this HtmlBuilder hb, SiteSettings ss)
        {
            return hb.FieldSet(id: "ViewsSettingsEditor", action: () => hb
                .FieldSelectable(
                    controlId: "Views",
                    fieldCss: "field-vertical w400",
                    controlContainerCss: "container-selectable",
                    controlWrapperCss: " h350",
                    listItemCollection: ss.ViewSelectableOptions(),
                    commandOptionPositionIsTop: true,
                    commandOptionAction: () => hb
                        .Div(css: "command-center", action: () => hb
                            .Button(
                                controlId: "MoveUpViews",
                                text: Displays.MoveUp(),
                                controlCss: "button-icon",
                                onClick: "$p.send($(this));",
                                icon: "ui-icon-circle-triangle-n",
                                action: "SetSiteSettings",
                                method: "post")
                            .Button(
                                controlId: "MoveDownViews",
                                text: Displays.MoveDown(),
                                controlCss: "button-icon",
                                onClick: "$p.send($(this));",
                                icon: "ui-icon-circle-triangle-s",
                                action: "SetSiteSettings",
                                method: "post")
                            .Button(
                                controlId: "NewView",
                                text: Displays.New(),
                                controlCss: "button-icon",
                                onClick: "$p.openViewDialog($(this));",
                                icon: "ui-icon-gear",
                                action: "SetSiteSettings",
                                method: "put")
                            .Button(
                                controlId: "EditView",
                                text: Displays.AdvancedSetting(),
                                controlCss: "button-icon",
                                onClick: "$p.openViewDialog($(this));",
                                icon: "ui-icon-gear",
                                action: "SetSiteSettings",
                                method: "put")
                            .Button(
                                controlId: "DeleteViews",
                                text: Displays.Delete(),
                                controlCss: "button-icon",
                                onClick: "$p.send($(this));",
                                icon: "ui-icon-circle-triangle-e",
                                action: "SetSiteSettings",
                                method: "put"))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder ViewDialog(SiteSettings ss, string controlId, View view)
        {
            var hb = new HtmlBuilder();
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
                        controlCss: " must-transport",
                        labelText: Displays.Id(),
                        text: view.Id.ToString())
                    .FieldTextBox(
                        controlId: "ViewName",
                        labelText: Displays.Name(),
                        text: view.Name,
                        validateRequired: true)
                    .Div(id: "ViewTabsContainer", action: () => hb
                        .Ul(id: "ViewTabs", action: () => hb
                            .Li(action: () => hb
                                .A(
                                    href: "#ViewFiltersTab",
                                    text: Displays.Filters()))
                            .Li(action: () => hb
                                .A(
                                    href: "#ViewSortersTab",
                                    text: Displays.Sorters()))
                            .Li(
                                action: () => hb
                                    .A(
                                        href: "#ViewGanttTab",
                                        text: Displays.Gantt()),
                                _using: hasGantt)
                            .Li(
                                action: () => hb
                                    .A(
                                        href: "#ViewTimeSeriesTab",
                                        text: Displays.TimeSeries()),
                                _using: hasTimeSeries)
                            .Li(
                                action: () => hb
                                    .A(
                                        href: "#ViewKambanTab",
                                        text: Displays.Kamban()),
                                _using: hasKamban))
                        .ViewFiltersTab(ss: ss, view: view)
                        .ViewSortersTab(ss: ss, view: view)
                        .ViewGanttTab(ss: ss, view: view, _using: hasGantt)
                        .ViewTimeSeriesTab(ss: ss, view: view, _using: hasTimeSeries)
                        .ViewKambanTab(ss: ss, view: view, _using: hasKamban))
                    .P(css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "AddView",
                            text: Displays.Setting(),
                            controlCss: "button-icon validate",
                            onClick: "$p.send($(this));",
                            icon: "ui-icon-disk",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "NewView")
                        .Button(
                            controlId: "UpdateView",
                            text: Displays.Setting(),
                            controlCss: "button-icon validate",
                            onClick: "$p.send($(this));",
                            icon: "ui-icon-disk",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "EditView")
                        .Button(
                            text: Displays.Cancel(),
                            controlCss: "button-icon",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ViewFiltersTab(this HtmlBuilder hb, SiteSettings ss, View view)
        {
            return hb.FieldSet(id: "ViewFiltersTab", action: () => hb
                .Div(css: "items", action: () => hb
                    .FieldCheckBox(
                        controlId: "ViewFilters_Incomplete",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.Incomplete(),
                        _checked: view.Incomplete.ToBool(),
                        labelPositionIsRight: true)
                    .FieldCheckBox(
                        controlId: "ViewFilters_Own",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.Own(),
                        _checked: view.Own.ToBool(),
                        labelPositionIsRight: true)
                    .FieldCheckBox(
                        controlId: "ViewFilters_NearCompletionTime",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.NearCompletionTime(),
                        _checked: view.NearCompletionTime.ToBool(),
                        labelPositionIsRight: true)
                    .FieldCheckBox(
                        controlId: "ViewFilters_Delay",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.Delay(),
                        _checked: view.Delay.ToBool(),
                        labelPositionIsRight: true)
                    .FieldCheckBox(
                        controlId: "ViewFilters_Overdue",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.Overdue(),
                        _checked: view.Overdue.ToBool(),
                        labelPositionIsRight: true)
                    .FieldTextBox(
                        controlId: "ViewFilters_Search",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.Search(),
                        text: view.Search)
                    .ViewColumnFilters(ss: ss, view: view))
                .Div(css: "both", action: () => hb
                    .FieldDropDown(
                        controlId: "ViewFilterSelector",
                        fieldCss: "field-auto-thin",
                        controlCss: " must-transport",
                        optionCollection: ColumnUtilities.FilterDefinitions(ss.ReferenceType)
                            .Where(o => !view.FilterContains(o.ColumnName))
                            .ToDictionary(
                                o => o.ColumnName,
                                o => ss.GetColumn(o.ColumnName).LabelText))
                    .Button(
                        controlId: "AddViewFilter",
                        controlCss: "button-icon",
                        text: Displays.Add(),
                        onClick: "$p.send($(this));",
                        icon: "ui-icon-plus",
                        action: "SetSiteSettings",
                        method: "post")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder ViewColumnFilters(
            this HtmlBuilder hb, SiteSettings ss, View view)
        {
            view.ColumnFilterHash?.ForEach(data => hb
                .ViewFilter(ss.GetColumn(data.Key), data.Value));
            return hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder ViewFilter(
            this HtmlBuilder hb, Column column, string value = null)
        {
            switch (column.TypeName.CsTypeSummary())
            {
                case Types.CsBool:
                    switch (column.CheckFilterControlType)
                    {
                        case ColumnUtilities.CheckFilterControlTypes.OnOnly:
                            return hb.FieldCheckBox(
                                controlId: "ViewFilters_" + column.Id,
                                fieldCss: "field-auto-thin",
                                labelText: Displays.Get(column.GridLabelText),
                                _checked: value.ToBool());
                        case ColumnUtilities.CheckFilterControlTypes.OnAndOff:
                            return hb.FieldDropDown(
                                controlId: "ViewFilters_" + column.Id,
                                fieldCss: "field-auto-thin",
                                labelText: Displays.Get(column.GridLabelText),
                                optionCollection: ColumnUtilities.CheckFilterTypeOptions(),
                                selectedValue: value,
                                addSelectedValue: false,
                                insertBlank: true);
                        default:
                            return hb;
                    }
                case Types.CsDateTime:
                    return hb.FieldDropDown(
                        controlId: "ViewFilters_" + column.Id,
                        fieldCss: "field-auto-thin",
                        controlCss: " auto-postback",
                        labelText: Displays.Get(column.GridLabelText),
                        optionCollection: column.DateFilterOptions(),
                        selectedValue: value,
                        multiple: true,
                        addSelectedValue: false);
                case Types.CsNumeric:
                    return hb.FieldDropDown(
                        controlId: "ViewFilters_" + column.Id,
                        fieldCss: "field-auto-thin",
                        controlCss: " auto-postback",
                        labelText: Displays.Get(column.GridLabelText),
                        optionCollection: column.HasChoices()
                            ? column.EditChoices()
                            : column.NumFilterOptions(),
                        selectedValue: value,
                        multiple: true,
                        addSelectedValue: false);
                case Types.CsString:
                    return column.HasChoices()
                        ? hb.FieldDropDown(
                            controlId: "ViewFilters_" + column.Id,
                            fieldCss: "field-auto-thin",
                            controlCss: " auto-postback",
                            labelText: Displays.Get(column.GridLabelText),
                            optionCollection: column.EditChoices(),
                            selectedValue: value,
                            multiple: true,
                            addSelectedValue: false)
                        : hb.FieldTextBox(
                            controlId: "ViewFilters_" + column.Id,
                            fieldCss: "field-auto-thin",
                            labelText: Displays.Get(column.GridLabelText),
                            text: value);
                default:
                    return hb;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ViewSortersTab(this HtmlBuilder hb, SiteSettings ss, View view)
        {
            return hb.FieldSet(id: "ViewSortersTab", action: () => hb
                .FieldBasket(
                    controlId: "ViewSorters",
                    fieldCss: "field-wide",
                    controlCss: "control-basket cf",
                    listItemCollection: view.ColumnSorterHash?.ToDictionary(
                        o => "{0},{1}".Params(o.Key, o.Value),
                        o => "{0}({1})".Params(
                            ss.GetColumn(o.Key)?.LabelText,
                            Displays.Get("Order" + o.Value.ToString().ToUpperFirstChar()))),
                    labelAction: () => hb
                        .Text(text: Displays.Sorters()))
                .FieldDropDown(
                    controlId: "ViewSorterSelector",
                    fieldCss: "field-auto-thin",
                    controlCss: " must-transport",
                    optionCollection: ColumnUtilities.GridDefinitions(ss.ReferenceType)
                        .Where(o => !view.SorterContains(o.ColumnName))
                        .ToDictionary(
                            o => o.ColumnName,
                            o => ss.GetColumn(o.ColumnName).LabelText))
                .FieldDropDown(
                    controlId: "ViewSorterOrderTypes",
                    fieldCss: "field-auto-thin",
                    controlCss: " must-transport",
                    optionCollection: new Dictionary<string, string>
                    {
                        { "Asc", Displays.OrderAsc() },
                        { "Desc", Displays.OrderDesc() }
                    })
                .Button(
                    controlId: "AddViewSorter",
                    controlCss: "button-icon",
                    text: Displays.Add(),
                    icon: "ui-icon-plus"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ViewGanttTab(
            this HtmlBuilder hb, SiteSettings ss, View view, bool _using)
        {
            return _using
                ? hb.FieldSet(id: "ViewGanttTab", action: () => hb
                    .FieldDropDown(
                        controlId: "GanttGroupBy",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.GroupBy(),
                        optionCollection: ss.GanttGroupByOptions(),
                        selectedValue: view.GanttGroupBy,
                        insertBlank: true))
                : hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ViewTimeSeriesTab(
            this HtmlBuilder hb, SiteSettings ss, View view, bool _using)
        {
            return _using
                ? hb.FieldSet(id: "ViewTimeSeriesTab", action: () => hb
                    .FieldDropDown(
                        controlId: "TimeSeriesGroupBy",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.GroupBy(),
                        optionCollection: ss.TimeSeriesGroupByOptions(),
                        selectedValue: view.TimeSeriesGroupBy)
                    .FieldDropDown(
                        controlId: "TimeSeriesAggregateType",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.AggregationType(),
                        optionCollection: ss.TimeSeriesAggregationTypeOptions(),
                        selectedValue: view.TimeSeriesAggregateType)
                    .FieldDropDown(
                        controlId: "TimeSeriesValue",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.AggregationTarget(),
                        optionCollection: ss.TimeSeriesValueOptions(),
                        selectedValue: view.TimeSeriesValue))
                : hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ViewKambanTab(
            this HtmlBuilder hb, SiteSettings ss, View view, bool _using)
        {
            return _using
                ? hb.FieldSet(id: "ViewKambanTab", action: () => hb
                    .FieldDropDown(
                        controlId: "KambanGroupBy",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.GroupBy(),
                        optionCollection: ss.KambanGroupByOptions(),
                        selectedValue: view.KambanGroupBy)
                    .FieldDropDown(
                        controlId: "KambanAggregateType",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.AggregationType(),
                        optionCollection: ss.KambanAggregationTypeOptions(),
                        selectedValue: view.KambanAggregateType)
                    .FieldDropDown(
                        controlId: "KambanValue",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.AggregationTarget(),
                        optionCollection: ss.KamvanValueOptions(),
                        selectedValue: view.KambanValue))
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
        private static HtmlBuilder NotificationsSettingsEditor(this HtmlBuilder hb, SiteSettings ss)
        {
            return hb.FieldSet(id: "NotificationsSettingsEditor", action: () => hb
                .NotificationSettings(ss)
                .Button(
                    controlId: "NewNotification",
                    text: Displays.New(),
                    controlCss: "button-icon",
                    onClick: "$p.openNotificationDialog($(this));",
                    icon: "ui-icon-gear",
                    action: "SetSiteSettings",
                    method: "put")
                .Div(attributes: new HtmlAttributes()
                    .Id("EditNotification")
                    .DataAction("SetSiteSettings")
                    .DataMethod("post"))
                .Div(attributes: new HtmlAttributes()
                    .Id("DeleteNotification")
                    .DataAction("SetSiteSettings")
                    .DataMethod("post")
                    .DataConfirm("ConfirmDelete")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder NotificationDialog(
            SiteSettings ss, string controlId, Notification notification)
        {
            var hb = new HtmlBuilder();
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("NotificationForm")
                    .Action(Locations.ItemAction(ss.SiteId)),
                action: () => hb
                    .FieldDropDown(
                        controlId: "NotificationType",
                        controlCss: " must-transport",
                        labelText: Displays.NotificationType(),
                        optionCollection: new Dictionary<string, string>
                        {
                            {
                                Notification.Types.Mail.ToInt().ToString(),
                                Displays.Mail()
                            },
                            {
                                Notification.Types.Slack.ToInt().ToString(),
                                Displays.Slack()
                            },
                            {
                                Notification.Types.ChatWork.ToInt().ToString(),
                                Displays.ChatWork()
                            }
                        },
                        selectedValue: notification.Type.ToInt().ToString())
                    .FieldTextBox(
                        controlId: "NotificationPrefix",
                        controlCss: " must-transport",
                        labelText: Displays.Prefix(),
                        text: notification.Prefix)
                    .FieldTextBox(
                        controlId: "NotificationAddress",
                        fieldCss: "field-wide",
                        controlCss: " must-transport",
                        labelText: Displays.Address(),
                        text: notification.Address,
                        validateRequired: true)
                    .FieldTextBox(
                        fieldId: "NotificationTokenField",
                        controlId: "NotificationToken",
                        fieldCss: "field-wide" + (!TokenList().Contains(notification.Type.ToInt())
                            ? " hidden"
                            : string.Empty),
                        controlCss: " must-transport",
                        labelText: Displays.Token(),
                        text: notification.Token)
                    .Hidden(
                        controlId: "NotificationTokenEnableList",
                        value: TokenList().Join())
                    .Div(_using: ss.Views?.Any() == true, action: () => hb
                        .FieldDropDown(
                            controlId: "BeforeCondition",
                            controlCss: " must-transport",
                            labelText: Displays.BeforeCondition(),
                            optionCollection: ss.ViewSelectableOptions(),
                            selectedValue: notification.BeforeCondition.ToString(),
                            insertBlank: true)
                        .FieldDropDown(
                            controlId: "Expression",
                            controlCss: " must-transport",
                            labelText: Displays.Expression(),
                            optionCollection: new Dictionary<string, string>
                            {
                                {
                                    Notification.Expressions.Or.ToInt().ToString(),
                                    Displays.Or()
                                },
                                {
                                    Notification.Expressions.And.ToInt().ToString(),
                                    Displays.And()
                                }
                            },
                            selectedValue: notification.Expression.ToInt().ToString())
                        .FieldDropDown(
                            controlId: "AfterCondition",
                            controlCss: " must-transport",
                            labelText: Displays.AfterCondition(),
                            optionCollection: ss.ViewSelectableOptions(),
                            selectedValue: notification.AfterCondition.ToString(),
                            insertBlank: true))
                    .FieldSet(
                        css: " enclosed",
                        legendText: Displays.MonitorChangesColumns(),
                        action: () => hb
                            .FieldSelectable(
                                controlId: "MonitorChangesColumns",
                                fieldCss: "field-vertical",
                                controlContainerCss: "container-selectable",
                                controlWrapperCss: " h200",
                                labelText: Displays.CurrentSettings(),
                                listItemCollection: ss
                                    .MonitorChangesSelectableOptions(
                                        notification.MonitorChangesColumns),
                                commandOptionPositionIsTop: true,
                                commandOptionAction: () => hb
                                    .Div(css: "command-center", action: () => hb
                                        .Button(
                                            controlId: "MoveUpMonitorChangesColumns",
                                            text: Displays.MoveUp(),
                                            controlCss: "button-icon",
                                            onClick: "$p.send($(this));",
                                            icon: "ui-icon-circle-triangle-n",
                                            action: "SetSiteSettings",
                                            method: "post")
                                        .Button(
                                            controlId: "MoveDownMonitorChangesColumns",
                                            text: Displays.MoveDown(),
                                            controlCss: "button-icon",
                                            onClick: "$p.send($(this));",
                                            icon: "ui-icon-circle-triangle-s",
                                            action: "SetSiteSettings",
                                            method: "post")
                                        .Button(
                                            controlId: "ToDisableMonitorChangesColumns",
                                            text: Displays.ToDisable(),
                                            controlCss: "button-icon",
                                            onClick: "$p.send($(this));",
                                            icon: "ui-icon-circle-triangle-e",
                                            action: "SetSiteSettings",
                                            method: "put")))
                            .FieldSelectable(
                                controlId: "MonitorChangesSourceColumns",
                                fieldCss: "field-vertical",
                                controlContainerCss: "container-selectable",
                                controlWrapperCss: " h200",
                                labelText: Displays.OptionList(),
                                listItemCollection: ss
                                    .MonitorChangesSelectableOptions(
                                        notification.MonitorChangesColumns,
                                        enabled: false),
                                commandOptionPositionIsTop: true,
                                commandOptionAction: () => hb
                                    .Div(css: "command-center", action: () => hb
                                        .Button(
                                            controlId: "ToEnableMonitorChangesColumns",
                                            text: Displays.ToEnable(),
                                            controlCss: "button-icon",
                                            onClick: "$p.send($(this));",
                                            icon: "ui-icon-circle-triangle-w",
                                            action: "SetSiteSettings",
                                            method: "put"))))
                    .P(css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "AddNotification",
                            text: Displays.Setting(),
                            controlCss: "button-icon validate",
                            icon: "ui-icon-disk",
                            onClick: "$p.setNotification($(this));",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "NewNotification")
                        .Button(
                            controlId: "UpdateNotification",
                            text: Displays.Setting(),
                            controlCss: "button-icon validate",
                            onClick: "$p.setNotification($(this));",
                            icon: "ui-icon-disk",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "EditNotification")
                        .Button(
                            text: Displays.Cancel(),
                            controlCss: "button-icon",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static IEnumerable<int> TokenList()
        {
            return new List<int> { Notification.Types.ChatWork.ToInt() };
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder NotificationSettings(this HtmlBuilder hb, SiteSettings ss)
        {
            return hb.Table(id: "NotificationSettings", css: "grid", action: () => hb
                .THead(action: () => hb
                    .Tr(css: "ui-widget-header", action: () => hb
                        .Th(action: () => hb
                            .Text(text: Displays.NotificationType()))
                        .Th(action: () => hb
                            .Text(text: Displays.Prefix()))
                        .Th(action: () => hb
                            .Text(text: Displays.Address()))
                        .Th(action: () => hb
                            .Text(text: Displays.Notifications()))
                        .Th(action: () => hb
                            .Text(text: Displays.BeforeCondition()))
                        .Th(action: () => hb
                            .Text(text: Displays.Expression()))
                        .Th(action: () => hb
                            .Text(text: Displays.AfterCondition()))
                        .Th(action: () => hb
                            .Text(text: Displays.Operations()))))
                .NotificationSettingsTBody(ss: ss));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder NotificationSettingsTBody(this HtmlBuilder hb, SiteSettings ss)
        {
            return hb.TBody(action: () =>
                ss.Notifications
                    .Select((o, i) => new { Notification = o, Id = i })
                    .ForEach(data =>
                    {
                        var beforeCondition = ss.Views?.Get(data.Notification.BeforeCondition);
                        var afterCondition = ss.Views?.Get(data.Notification.AfterCondition);
                        hb.Tr(
                            attributes: new HtmlAttributes()
                                .Class("grid-row not-link")
                                .DataId(data.Id.ToString()),
                            action: () => hb
                                .Td(action: () => hb
                                    .Text(text: Displays.Get(data.Notification.Type.ToString())))
                                .Td(action: () => hb
                                    .Text(text: data.Notification.Prefix))
                                .Td(action: () => hb
                                    .Text(text: data.Notification.Address))
                                .Td(action: () => hb
                                    .Text(text: data.Notification.MonitorChangesColumns?
                                        .Select(o => ss.GetColumn(o).LabelText)
                                        .Join(", ")))
                                .Td(action: () => hb
                                    .Text(text: beforeCondition?.Name))
                                .Td(action: () => hb
                                    .Text(text: beforeCondition != null && afterCondition != null
                                        ? Displays.Get(data.Notification.Expression.ToString())
                                        : null))
                                .Td(action: () => hb
                                    .Text(text: afterCondition?.Name))
                                .Td(action: () => hb
                                    .Button(
                                        controlCss: "button-icon delete",
                                        text: Displays.Delete(),
                                        dataId: data.Id.ToString(),
                                        icon: "ui-icon-trash")));
                    }));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder MailSettingsEditor(this HtmlBuilder hb, SiteSettings ss)
        {
            return hb.FieldSet(id: "MailSettingsEditor", action: () => hb
                .FieldTextBox(
                    textType: HtmlTypes.TextTypes.MultiLine,
                    controlId: "AddressBook",
                    fieldCss: "field-wide",
                    labelText: Displays.DefaultAddressBook(),
                    text: ss.AddressBook.ToStr())
                .FieldSet(
                    css: " enclosed-thin",
                    legendText: Displays.DefaultDestinations(),
                    action: () => hb
                        .FieldTextBox(
                            textType: HtmlTypes.TextTypes.MultiLine,
                            controlId: "MailToDefault",
                            fieldCss: "field-wide",
                            labelText: Displays.OutgoingMails_To(),
                            text: ss.MailToDefault.ToStr())
                        .FieldTextBox(
                            textType: HtmlTypes.TextTypes.MultiLine,
                            controlId: "MailCcDefault",
                            fieldCss: "field-wide",
                            labelText: Displays.OutgoingMails_Cc(),
                            text: ss.MailCcDefault.ToStr())
                        .FieldTextBox(
                            textType: HtmlTypes.TextTypes.MultiLine,
                            controlId: "MailBccDefault",
                            fieldCss: "field-wide",
                            labelText: Displays.OutgoingMails_Bcc(),
                            text: ss.MailBccDefault.ToStr())));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder StylesSettingsEditor(this HtmlBuilder hb, SiteSettings ss)
        {
            return hb.FieldSet(id: "StylesSettingsEditor", action: () => hb
                .FieldTextBox(
                    textType: HtmlTypes.TextTypes.MultiLine,
                    controlId: "GridStyle",
                    fieldCss: "field-wide",
                    labelText: Displays.GridStyle(),
                    text: ss.GridStyle.ToStr())
                .FieldTextBox(
                    textType: HtmlTypes.TextTypes.MultiLine,
                    controlId: "NewStyle",
                    fieldCss: "field-wide",
                    labelText: Displays.NewStyle(),
                    text: ss.NewStyle.ToStr())
                .FieldTextBox(
                    textType: HtmlTypes.TextTypes.MultiLine,
                    controlId: "EditStyle",
                    fieldCss: "field-wide",
                    labelText: Displays.EditStyle(),
                    text: ss.EditStyle.ToStr()));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ScriptsSettingsEditor(this HtmlBuilder hb, SiteSettings ss)
        {
            return hb.FieldSet(id: "ScriptsSettingsEditor", action: () => hb
                .FieldTextBox(
                    textType: HtmlTypes.TextTypes.MultiLine,
                    controlId: "GridScript",
                    fieldCss: "field-wide",
                    labelText: Displays.GridScript(),
                    text: ss.GridScript.ToStr())
                .FieldTextBox(
                    textType: HtmlTypes.TextTypes.MultiLine,
                    controlId: "NewScript",
                    fieldCss: "field-wide",
                    labelText: Displays.NewScript(),
                    text: ss.NewScript.ToStr())
                .FieldTextBox(
                    textType: HtmlTypes.TextTypes.MultiLine,
                    controlId: "EditScript",
                    fieldCss: "field-wide",
                    labelText: Displays.EditScript(),
                    text: ss.EditScript.ToStr()));
        }
    }
}
