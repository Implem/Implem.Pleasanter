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
            this HtmlBuilder hb, Column column, SiteModel siteModel)
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
                .Invoke("validateSites")
                .Message(message)
                .ClearFormData();
        }

        public static ResponseCollection FormResponse(
            this ResponseCollection res,
            Permissions.Types pt,
            SiteModel siteModel)
        {
            Forms.All().Keys.ForEach(key =>
            {
                switch (key)
                {
                    default: break;
                }
            });
            return res;
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
                            .ReplaceAll(
                                "#ItemValidator",
                                new HtmlBuilder().ItemValidator(referenceType: "Wikis"))
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
                .FormResponse(pt, siteModel)
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
            var columns = ss.HistoryColumnCollection();
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
                                                .TdValue(column, siteModelHistory))))));
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
            var displayValue = ss.TitleColumnCollection()
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
            var displayValue = ss.TitleColumnCollection()
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
                siteModel.PermissionType,
                Permissions.GetById(sourceId),
                Permissions.GetById(destinationId));
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
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
            SiteInfo.SiteMenu.Set(sourceId);
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
                                        href: "#MailerSettingsEditor",
                                        text: Displays.MailerSettingsEditor()))
                                .Li(action: () => hb
                                    .A(
                                        href: "#NotificationSettingsEditor",
                                        text: Displays.NotificationSettingsEditor()))
                                .Li(action: () => hb
                                    .A(
                                        href: "#StyleSettingsEditor",
                                        text: Displays.StyleSettingsEditor()))
                                .Li(action: () => hb
                                    .A(
                                        href: "#ScriptSettingsEditor",
                                        text: Displays.ScriptSettingsEditor()));
                            break;
                        default:
                            hb
                                .Li(action: () => hb
                                    .A(
                                        href: "#GridSettingsEditor",
                                        text: Displays.GridSettingsEditor()))
                                .Li(action: () => hb
                                    .A(
                                        href: "#EditorSettingsEditor",
                                        text: Displays.EditorSettingsEditor()))
                                .Li(action: () => hb
                                    .A(
                                        href: "#SummarySettingsEditor",
                                        text: Displays.SummarySettingsEditor()))
                                .Li(action: () => hb
                                    .A(
                                        href: "#NotificationSettingsEditor",
                                        text: Displays.NotificationSettingsEditor()))
                                .Li(action: () => hb
                                    .A(
                                        href: "#MailerSettingsEditor",
                                        text: Displays.MailerSettingsEditor()))
                                .Li(action: () => hb
                                    .A(
                                        href: "#StyleSettingsEditor",
                                        text: Displays.StyleSettingsEditor()))
                                .Li(action: () => hb
                                    .A(
                                        href: "#ScriptSettingsEditor",
                                        text: Displays.ScriptSettingsEditor()));
                            break;
                    }
                    hb.Li(action: () => hb
                        .A(
                            href: "#FieldSetHistories",
                            text: Displays.Histories()));
                }
                hb.Hidden(controlId: "TableName", value: "Sites");
            });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string SiteTop(SiteSettings ss)
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
                action: () =>
                {
                    hb.Form(
                        attributes: new HtmlAttributes()
                            .Id("SitesForm")
                            .Class("main-form")
                            .Action(Locations.ItemAction(0)),
                        action: () => hb
                            .SiteMenu(
                                ss: ss,
                                pt: pt,
                                siteModel: null,
                                siteConditions: siteConditions)
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
                action: () =>
                {
                    hb.Form(
                        attributes: new HtmlAttributes()
                            .Id("SitesForm")
                            .Class("main-form")
                            .Action(Locations.ItemAction(ss.SiteId)),
                        action: () => hb
                            .SiteMenu(
                                ss: ss,
                                pt: siteModel.PermissionType,
                                siteModel: siteModel,
                                siteConditions: siteConditions)
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

        private static HtmlBuilder SiteMenu(
            this HtmlBuilder hb,
            SiteSettings ss,
            Permissions.Types pt,
            SiteModel siteModel,
            IEnumerable<SiteCondition> siteConditions)
        {
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
                        DataViewSelectors.Get(siteId));
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
                    .Id("NotificationDialog")
                    .Class("dialog")
                    .Title(Displays.NotificationSettingsEditor())));
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
                            .NotificationSettingsEditor(siteModel.SiteSettings)
                            .MailerSettingsEditor(siteModel.SiteSettings)
                            .StyleSettingsEditor(siteModel.SiteSettings)
                            .ScriptSettingsEditor(siteModel.SiteSettings);
                        break;
                    default:
                        hb
                            .GridSettingsEditor(siteModel.SiteSettings)
                            .EditorSettingsEditor(siteModel.SiteSettings)
                            .NotificationSettingsEditor(siteModel.SiteSettings)
                            .SummarySettingsEditor(siteModel.SiteSettings)
                            .MailerSettingsEditor(siteModel.SiteSettings)
                            .StyleSettingsEditor(siteModel.SiteSettings)
                            .ScriptSettingsEditor(siteModel.SiteSettings);
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
            return hb.FieldSet(
                id: "SiteImageSettingsEditor",
                action: () => hb
                    .FieldSet(
                        css: " enclosed",
                        legendText: Displays.Icon(),
                        action: () => hb
                            .FieldTextBox(
                                textType: HtmlTypes.TextTypes.File,
                                controlId: "SiteSettings,SiteImage",
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
            return hb.FieldSet(
                id: "GridSettingsEditor",
                action: () => hb
                    .GridColumns(ss)
                    .FilterColumns(ss)
                    .Aggregations(ss)
                    .FieldSpinner(
                        controlId: "SiteSettings,GridPageSize",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.SettingGridPageSize(),
                        value: ss.GridPageSize.ToDecimal(),
                        min: Parameters.General.GridPageSizeMin,
                        max: Parameters.General.GridPageSizeMax,
                        step: 1,
                        width: 25)
                    .FieldSpinner(
                        controlId: "SiteSettings,NearCompletionTimeBeforeDays",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.SettingNearCompletionTimeBeforeDays(),
                        value: ss.NearCompletionTimeBeforeDays.ToDecimal(),
                        min: Parameters.General.NearCompletionTimeBeforeDaysMin,
                        max: Parameters.General.NearCompletionTimeBeforeDaysMax,
                        step: 1,
                        width: 25)
                    .FieldSpinner(
                        controlId: "SiteSettings,NearCompletionTimeAfterDays",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.SettingNearCompletionTimeAfterDays(),
                        value: ss.NearCompletionTimeAfterDays.ToDecimal(),
                        min: Parameters.General.NearCompletionTimeAfterDaysMin,
                        max: Parameters.General.NearCompletionTimeAfterDaysMax,
                        step: 1,
                        width: 25)
                    .AggregationDetailsDialog(ss));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder GridColumns(this HtmlBuilder hb, SiteSettings ss)
        {
            return hb.FieldSet(
                css: " enclosed-thin",
                legendText: Displays.SettingGridColumns(),
                action: () => hb
                    .FieldSelectable(
                        controlId: "GridColumns",
                        fieldCss: "field-vertical",
                        controlContainerCss: "container-selectable",
                        controlWrapperCss: " h350",
                        labelText: Displays.EnabledList(),
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
                        labelText: Displays.DisabledList(),
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
        private static HtmlBuilder FilterColumns(this HtmlBuilder hb, SiteSettings ss)
        {
            return hb.FieldSet(
                css: " enclosed-thin",
                legendText: Displays.SettingFilterColumns(),
                action: () => hb
                    .FieldSelectable(
                        controlId: "FilterColumns",
                        fieldCss: "field-vertical",
                        controlContainerCss: "container-selectable",
                        controlWrapperCss: " h350",
                        labelText: Displays.EnabledList(),
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
                        labelText: Displays.DisabledList(),
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
                                    method: "put"))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Aggregations(this HtmlBuilder hb, SiteSettings ss)
        {
            return hb.FieldSet(
                css: " enclosed-thin",
                legendText: Displays.SettingAggregations(),
                action: () => hb
                    .FieldSelectable(
                        controlId: "AggregationDestination",
                        fieldCss: "field-vertical both",
                        controlContainerCss: "container-selectable",
                        controlWrapperCss: " h350",
                        labelText: Displays.SettingAggregationList(),
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
                        labelText: Displays.SettingSelectionList(),
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
                                    method: "post"))));
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
                        labelText: Displays.SettingAggregationType(),
                        optionCollection: new Dictionary<string, string>
                        {
                            { "Count", Displays.Count() },
                            { "Total", Displays.Total() },
                            { "Average", Displays.Average() }
                        })
                    .FieldDropDown(
                        controlId: "AggregationTarget",
                        fieldCss: " hidden togglable",
                        labelText: Displays.SettingAggregationTarget(),
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
            return hb.FieldSet(
                id: "EditorSettingsEditor",
                action: () => hb
                    .SiteSettingEditorColumns(ss)
                    .SiteSettingLinkColumns(ss)
                    .SiteSettingHistoryColumns(ss)
                    .SiteSettingFormulas(ss));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SiteSettingEditorColumns(this HtmlBuilder hb, SiteSettings ss)
        {
            return hb.FieldSet(
                legendText: Displays.SettingEditorColumns(),
                css: " enclosed",
                action: () => hb
                    .FieldSelectable(
                        controlId: "EditorColumns",
                        fieldCss: "field-vertical",
                        controlContainerCss: "container-selectable",
                        controlWrapperCss: " h350",
                        labelText: Displays.EnabledList(),
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
                                    controlId: "OpenColumnPropertiesDialog",
                                    text: Displays.AdvancedSetting(),
                                    controlCss: "button-icon",
                                    onClick: "$p.openColumnPropertiesDialog($(this));",
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
                        labelText: Displays.DisabledList(),
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
                                    method: "put")))
                    .Div(attributes: new HtmlAttributes()
                        .Id("ColumnPropertiesDialog")
                        .Class("dialog")
                        .Title(Displays.AdvancedSetting())));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SiteSettingLinkColumns(this HtmlBuilder hb, SiteSettings ss)
        {
            return hb.FieldSet(
                legendText: Displays.SettingLinkColumns(),
                css: " enclosed",
                action: () => hb
                    .FieldSelectable(
                        controlId: "LinkColumns",
                        fieldCss: "field-vertical",
                        controlContainerCss: "container-selectable",
                        controlWrapperCss: " h350",
                        labelText: Displays.EnabledList(),
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
                        labelText: Displays.DisabledList(),
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
                                    method: "put"))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SiteSettingHistoryColumns(this HtmlBuilder hb, SiteSettings ss)
        {
            return hb.FieldSet(
                legendText: Displays.SettingHistoryColumns(),
                css: " enclosed",
                action: () => hb
                    .FieldSelectable(
                        controlId: "HistoryColumns",
                        fieldCss: "field-vertical",
                        controlContainerCss: "container-selectable",
                        controlWrapperCss: " h350",
                        labelText: Displays.EnabledList(),
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
                        labelText: Displays.DisabledList(),
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
                                    method: "put"))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder ColumnProperties(
            SiteSettings ss, Column column, IEnumerable<string> titleColumns)
        {
            var hb = new HtmlBuilder();
            hb.FieldSet(
                css: " enclosed",
                legendText: column.LabelTextDefault,
                action: () =>
                {
                    hb.FieldTextBox(
                        controlId: "ColumnProperty,LabelText",
                        labelText: Displays.SettingLabel(),
                        text: column.LabelText);
                    switch (column.TypeName.CsTypeSummary())
                    {
                        case Types.CsBool:
                            break;
                        default:
                            if (column.Max != -1)
                            {
                                hb.FieldDropDown(
                                    controlId: "ColumnProperty,FieldCss",
                                    labelText: Displays.Style(),
                                    optionCollection: new Dictionary<string, string>
                                    {
                                        { "field-normal", Displays.Normal() },
                                        { "field-wide", Displays.Wide() },
                                        { "field-auto", Displays.Auto() }
                                    },
                                    selectedValue: column.FieldCss);
                            }
                            break;
                    }
                    hb.FieldCheckBox(
                        controlId: "ColumnProperty,EditorReadOnly",
                        labelText: Displays.ReadOnly(),
                        _checked: column.EditorReadOnly.ToBool(),
                        _using: column.Nullable);
                    if (column.TypeName == "datetime")
                    {
                        hb
                            .FieldDropDown(
                                controlId: "ColumnProperty,GridFormat",
                                labelText: Displays.SettingGridFormat(),
                                optionCollection: DateTimeOptions(),
                                selectedValue: column.GridFormat)
                            .FieldDropDown(
                                controlId: "ColumnProperty,ControlFormat",
                                labelText: Displays.SettingControlFormat(),
                                optionCollection: DateTimeOptions(forControl: true),
                                selectedValue: column.ControlFormat)
                            .FieldDropDown(
                                controlId: "ColumnProperty,ExportFormat",
                                labelText: Displays.SettingExportFormat(),
                                optionCollection: DateTimeOptions(),
                                selectedValue: column.ExportFormat);
                    }
                    switch (column.TypeName.CsTypeSummary())
                    {
                        case Types.CsBool:
                            hb.FieldCheckBox(
                                controlId: "ColumnProperty,DefaultInput",
                                labelText: Displays.DefaultInput(),
                                _checked: column.DefaultInput.ToBool());
                            break;
                        case Types.CsNumeric:
                            if (column.ControlType != "ChoicesText")
                            {
                                var maxDecimalPlaces = MaxDecimalPlaces(column);
                                hb
                                    .FieldTextBox(
                                        controlId: "ColumnProperty,DefaultInput",
                                        labelText: Displays.DefaultInput(),
                                        text: column.DefaultInput.ToLong().ToString(),
                                        _using: !column.Id_Ver)
                                    .FormatColumnProperty(column: column)
                                    .FieldTextBox(
                                        controlId: "ColumnProperty,Unit",
                                        controlCss: " w50",
                                        labelText: Displays.SettingUnit(),
                                        text: column.Unit,
                                        _using: !column.Id_Ver)
                                    .FieldSpinner(
                                        controlId: "ColumnProperty,DecimalPlaces",
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
                                            controlId: "ColumnProperty,ControlType",
                                            labelText: Displays.ControlType(),
                                            optionCollection: new Dictionary<string, string>
                                            {
                                                { "Normal", Displays.Normal() },
                                                { "Spinner", Displays.Spinner() }
                                            },
                                            selectedValue: column.ControlType)
                                        .FieldTextBox(
                                            fieldId: "ColumnPropertyField,Min",
                                            controlId: "ColumnProperty,Min",
                                            fieldCss: " both" + hidden,
                                            labelText: Displays.Min(),
                                            text: column.Display(column.Min.ToDecimal()))
                                        .FieldTextBox(
                                            fieldId: "ColumnPropertyField,Max",
                                            controlId: "ColumnProperty,Max",
                                            fieldCss: hidden,
                                            labelText: Displays.Max(),
                                            text: column.Display(column.Max.ToDecimal()))
                                        .FieldTextBox(
                                            fieldId: "ColumnPropertyField,Step",
                                            controlId: "ColumnProperty,Step",
                                            fieldCss: hidden,
                                            labelText: Displays.Step(),
                                            text: column.Display(column.Step.ToDecimal()));
                                }
                            }
                            break;
                        case Types.CsDateTime:
                            hb.FieldSpinner(
                                controlId: "ColumnProperty,DefaultInput",
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
                            hb
                                .FieldTextBox(
                                    controlId: "ColumnProperty,DefaultInput",
                                    fieldCss: column.FieldCss,
                                    labelText: Displays.DefaultInput(),
                                    text: column.DefaultInput,
                                    _using: !column.MarkDown)
                                .FieldTextBox(
                                    textType: HtmlTypes.TextTypes.MultiLine,
                                    controlId: "ColumnProperty,DefaultInput",
                                    fieldCss: column.FieldCss,
                                    labelText: Displays.DefaultInput(),
                                    text: column.DefaultInput,
                                    _using: column.MarkDown);
                            break;
                    }
                    switch (column.ControlType)
                    {
                        case "ChoicesText":
                            hb.TextBox(
                                textType: HtmlTypes.TextTypes.MultiLine,
                                controlId: "ColumnProperty,ChoicesText",
                                controlCss: " choices",
                                placeholder: Displays.SettingSelectionList(),
                                text: column.ChoicesText);
                            break;
                        default:
                            break;
                    }
                    if (column.ColumnName == "Title")
                    {
                        hb.TitleColumnProperty(ss, titleColumns);
                    }
                });
            return hb
                .P(css: "message-dialog")
                .Div(css: "command-center", action: () => hb
                    .Button(
                        controlId: "SetColumnProperties",
                        text: Displays.Setting(),
                        controlCss: "button-icon",
                        onClick: "$p.sendByDialog($(this));",
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
        private static HtmlBuilder FormatColumnProperty(this HtmlBuilder hb, Column column)
        {
            var formats = Parameters.Formats
                .Where(o => (o.Type & ParameterAccessor.Parts.Format.Types.NumColumn) > 0);
            var format = formats.FirstOrDefault(o => o.String == column.Format);
            var other = !column.Format.IsNullOrEmpty() && format == null;
            return hb
                .FieldDropDown(
                    controlId: "ColumnProperty,FormatSelector",
                    controlCss: " not-transport",
                    labelText: Displays.SettingFormat(),
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
                    fieldId: "ColumnPropertyField,Format",
                    controlId: "ColumnProperty,Format",
                    fieldCss: other ? string.Empty : " hidden",
                    labelText: Displays.Custom(),
                    text: other
                        ? column.Format
                        : string.Empty);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder TitleColumnProperty(
            this HtmlBuilder hb, SiteSettings ss, IEnumerable<string> titleColumns)
        {
            return hb.FieldSelectable(
                controlId: "TitleColumns",
                fieldCss: "field-vertical",
                controlContainerCss: "container-selectable",
                controlWrapperCss: " h200",
                labelText: Displays.EnabledList(),
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
                labelText: Displays.DisabledList(),
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
                controlId: "SiteSettings,TitleSeparator",
                fieldCss: " both",
                labelText: Displays.SettingTitleSeparator(),
                text: ss.TitleSeparator);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static Dictionary<string, string> DateTimeOptions(bool forControl = false)
        {
            return forControl
                ? Def.DisplayDefinitionCollection
                    .Where(o => new string[] { "Ymd", "Ymdhm", "Ymdhms" }.Contains(o.Name))
                    .Where(o => o.Language == string.Empty)
                    .ToDictionary(o => o.Id, o => Displays.Get(o.Id))
                : Def.DisplayDefinitionCollection
                    .Where(o => o.Type == "Date")
                    .Where(o => o.Language == string.Empty)
                    .ToDictionary(o => o.Id, o => Displays.Get(o.Id));
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
        private static HtmlBuilder SiteSettingFormulas(this HtmlBuilder hb, SiteSettings ss)
        {
            return hb.FieldSet(
                legendText: Displays.SettingFormulas(),
                css: " enclosed",
                action: () => hb
                    .FieldSelectable(
                        controlId: "Formulas",
                        fieldCss: "field-vertical w600",
                        controlContainerCss: "container-selectable",
                        controlWrapperCss: " h200",
                        labelText: Displays.SettingColumnList(),
                        listItemCollection: ss.FormulaItemCollection(),
                        commandOptionAction: () => hb
                            .Div(css: "command-left", action: () => hb
                                .TextBox(
                                    controlId: "Formula",
                                    controlCss: " w250")
                                .Button(
                                    controlId: "AddFormula",
                                    controlCss: "button-icon",
                                    text: Displays.Add(),
                                    onClick: "$p.send($(this));",
                                    icon: "ui-icon-plus",
                                    action: "SetSiteSettings",
                                    method: "post")
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
        private static HtmlBuilder NotificationSettingsEditor(this HtmlBuilder hb, SiteSettings ss)
        {
            return hb.FieldSet(
                id: "NotificationSettingsEditor",
                action: () => hb
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
                            { Notification.Types.Mail.ToInt().ToString(), Displays.Mail() },
                            { Notification.Types.Slack.ToInt().ToString(), Displays.Slack() }
                        },
                        selectedValue: notification.Type.ToInt().ToString(),
                        disabled: controlId == "EditNotification")
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
                        text: notification.Address)
                    .FieldSet(
                        css: " enclosed",
                        legendText: Displays.MonitorChangesColumns(),
                        action: () =>
                        {
                            hb
                                .FieldSelectable(
                                    controlId: "MonitorChangesColumns",
                                    fieldCss: "field-vertical",
                                    controlContainerCss: "container-selectable",
                                    controlWrapperCss: " h200",
                                    labelText: Displays.EnabledList(),
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
                                    labelText: Displays.DisabledList(),
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
                                                method: "put")));
                        })
                    .P(css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "CreateNotification",
                            text: Displays.Setting(),
                            controlCss: "button-icon validate",
                            icon: "ui-icon-disk",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "NewNotification")
                        .Button(
                            controlId: "UpdateNotification",
                            text: Displays.Setting(),
                            controlCss: "button-icon validate",
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
                            .Text(text: Displays.NotificationSettingsEditor()))
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
            {
                ss.Notifications
                    .Select((o, i) => new { Notification = o, Id = i })
                    .ForEach(data =>
                    {
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
                                    .Text(text: data.Notification.MonitorChangesColumns
                                        .Select(o => ss.GetColumn(o).LabelText)
                                        .Join(", ")))
                                .Td(action: () => hb
                                    .Button(
                                        controlCss: "button-icon delete",
                                        text: Displays.Delete(),
                                        dataId: data.Id.ToString(),
                                        icon: "ui-icon-trash")));
                    });
            });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SummarySettingsEditor(this HtmlBuilder hb, SiteSettings ss)
        {
            var siteDataRows = ss.SummarySiteDataRows();
            if (siteDataRows == null)
            {
                return hb.SummarySettingsEditorNoLinks();
            }
            var summarySiteIdHash = SummarySiteIdHash(siteDataRows, ss);
            var firstSiteId = summarySiteIdHash.Select(o => o.Key.ToLong()).FirstOrDefault();
            return siteDataRows.Any()
                ? hb.FieldSet(
                    id: "SummarySettingsEditor",
                    action: () =>
                        hb.FieldSet(
                            legendText: Displays.SettingSummaryColumns(),
                            css: " enclosed",
                            action: () => hb
                                .FieldDropDown(
                                    controlId: "SummarySiteId",
                                    controlCss: " auto-postback",
                                    labelText: Displays.SummarySiteId(),
                                    optionCollection: summarySiteIdHash,
                                    action: "SetSiteSettings",
                                    method: "post")
                                .SummaryDestinationColumn(
                                    siteId: firstSiteId,
                                    referenceType: ss.ReferenceType,
                                    siteDataRows: siteDataRows)
                                .SummaryLinkColumn(
                                    ss: ss,
                                    siteId: firstSiteId)
                                .FieldDropDown(
                                    controlId: "SummaryType",
                                    controlCss: " auto-postback",
                                    labelText: Displays.SummaryType(),
                                    optionCollection: SummaryTypeCollection(),
                                    action: "SetSiteSettings",
                                    method: "post")
                                .SummarySourceColumn(ss)
                                .FieldContainer(actionOptions: () => hb
                                    .Div(css: "buttons", action: () => hb
                                        .Button(
                                            controlId: "AddSummary",
                                            text: Displays.Add(),
                                            controlCss: "button-icon",
                                            onClick: "$p.addSummary($(this));",
                                            icon: "ui-icon-plus",
                                            action: "SetSiteSettings",
                                            method: "put")))
                                .SummarySettings(sourceSiteSettings: ss)))
                : hb.SummarySettingsEditorNoLinks();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SummarySettingsEditorNoLinks(this HtmlBuilder hb)
        {
            return hb.FieldSet(
                id: "SummarySettingsEditor",
                action: () => hb
                    .P(action: () => hb
                        .Text(text: Displays.NoLinks())));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder SummaryDestinationColumn(
            this HtmlBuilder hb,
            long siteId,
            string referenceType,
            EnumerableRowCollection<DataRow> siteDataRows)
        {
            return hb.FieldDropDown(
                fieldId: "SummaryDestinationColumnField",
                controlId: "SummaryDestinationColumn",
                labelText: Displays.SummaryDestinationColumn(),
                optionCollection: SummaryDestinationColumnCollection(
                    siteDataRows, siteId, referenceType),
                action: "SetSiteSettings",
                method: "post");
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static Dictionary<string, string> SummaryDestinationColumnCollection(
            EnumerableRowCollection<DataRow> siteDataRows,
            long siteId,
            string referenceType)
        {
            return siteDataRows
                .Where(o => o["SiteId"].ToLong() == siteId)
                .Select(o => (
                    o["SiteSettings"].ToString().Deserialize<SiteSettings>() ??
                    SiteSettingsUtility.Get(siteId, referenceType)).ColumnCollection)
                .FirstOrDefault()?
                .Where(o => o.Computable)
                .Where(o => o.TypeName != "datetime")
                .Where(o => !o.NotUpdate)
                .OrderBy(o => o.No)
                .ToDictionary(
                    o => o.ColumnName,
                    o => o.LabelText);
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
            this HtmlBuilder hb, SiteSettings ss, long siteId)
        {
            return hb.FieldDropDown(
                fieldId: "SummaryLinkColumnField",
                controlId: "SummaryLinkColumn",
                labelText: Displays.SummaryLinkColumn(),
                optionCollection: ss.LinkCollection
                    .Where(o => o.SiteId == siteId)
                    .ToDictionary(
                        o => o.ColumnName,
                        o => ss.GetColumn(o.ColumnName).LabelText),
                action: "SetSiteSettings",
                method: "post");
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder SummarySourceColumn(
            this HtmlBuilder hb, SiteSettings ss, string type = "Count")
        {
            switch (type)
            {
                case "Count":
                    return hb.FieldContainer(
                        fieldId: "SummarySourceColumnField",
                        fieldCss: " hidden");
                default:
                    return hb.FieldDropDown(
                        fieldId: "SummarySourceColumnField",
                        controlId: "SummarySourceColumn",
                        labelText: Displays.SummarySourceColumn(),
                        optionCollection: ss.ColumnCollection
                            .Where(o => o.Computable)
                            .ToDictionary(o => o.ColumnName, o => o.LabelText),
                        action: "SetSiteSettings",
                        method: "post");
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder SummarySettings(
            this HtmlBuilder hb, SiteSettings sourceSiteSettings)
        {
            return hb.Div(id: "SummarySettings", action: () => hb
                .Table(css: "grid", action: () =>
                {
                    hb.THead(action: () => hb
                        .Tr(css: "ui-widget-header", action: () => hb
                            .Th(action: () => hb
                                .Text(Displays.SummarySiteId()))
                            .Th(action: () => hb
                                .Text(Displays.SummaryDestinationColumn()))
                            .Th(action: () => hb
                                .Text(Displays.SummaryLinkColumn()))
                            .Th(action: () => hb
                                .Text(Displays.SummaryType()))
                            .Th(action: () => hb
                                .Text(Displays.SummarySourceColumn()))
                            .Th(action: () => hb
                                .Text(Displays.Operations()))));
                    if (sourceSiteSettings.SummaryCollection.Count > 0)
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
                                    .SiteId_In(sourceSiteSettings.SummaryCollection
                                        .Select(o => o.SiteId)))).AsEnumerable();
                        hb.TBody(action: () =>
                        {
                            sourceSiteSettings.SummaryCollection.ForEach(summary =>
                            {
                                var dataRow = dataRows.FirstOrDefault(o =>
                                    o["SiteId"].ToLong() == summary.SiteId);
                                var destinationSiteSettings = dataRow["SiteSettings"]
                                    .ToString()
                                    .Deserialize<SiteSettings>() ??
                                        SiteSettingsUtility.Get(
                                            dataRow["SiteId"].ToLong(),
                                            dataRow["ReferenceType"].ToString());
                                if (destinationSiteSettings != null)
                                {
                                    hb.Tr(css: "grid-row not-link", action: () => hb
                                        .Td(action: () => hb
                                            .Text(dataRow["Title"].ToString()))
                                        .Td(action: () => hb
                                            .Text(destinationSiteSettings.GetColumn(
                                                summary.DestinationColumn)?.LabelText))
                                        .Td(action: () => hb
                                            .Text(sourceSiteSettings.GetColumn(
                                                summary.LinkColumn)?.LabelText))
                                        .Td(action: () => hb
                                            .Text(SummaryType(summary.Type)))
                                        .Td(action: () => hb
                                            .Text(sourceSiteSettings.GetColumn(
                                                summary.SourceColumn)?.LabelText))
                                        .Td(action: () => hb
                                            .Button(
                                                controlCss: "button-icon synchronize-summary",
                                                text: Displays.Synchronize(),
                                                dataId: summary.Id.ToString(),
                                                icon: "ui-icon-refresh",
                                                selector: "#SynchronizeSummary")
                                            .Button(
                                                controlCss: "button-icon delete-summary",
                                                text: Displays.Delete(),
                                                dataId: summary.Id.ToString(),
                                                icon: "ui-icon-trash",
                                                selector: "#DeleteSummary")));
                                }
                            });
                        });
                    }
                })
                .Hidden(attributes: new HtmlAttributes()
                    .Id("SynchronizeSummary")
                    .DataAction("SynchronizeSummary")
                    .DataMethod("put")
                    .DataConfirm(Displays.ConfirmSynchronize()))
                .Hidden(attributes: new HtmlAttributes()
                    .Id("DeleteSummary")
                    .DataAction("SetSiteSettings")
                    .DataMethod("delete")
                    .DataConfirm(Displays.ConfirmDelete())));
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
        private static HtmlBuilder MailerSettingsEditor(this HtmlBuilder hb, SiteSettings ss)
        {
            return hb.FieldSet(
                id: "MailerSettingsEditor",
                action: () => hb
                    .FieldTextBox(
                        textType: HtmlTypes.TextTypes.MultiLine,
                        controlId: "SiteSettings,AddressBook",
                        fieldCss: "field-wide",
                        labelText: Displays.DefaultAddressBook(),
                        text: ss.AddressBook.ToStr())
                    .FieldSet(
                        legendText: Displays.DefaultDestinations(),
                        css: " enclosed-thin",
                        action: () => hb
                            .FieldTextBox(
                                textType: HtmlTypes.TextTypes.MultiLine,
                                controlId: "SiteSettings,MailToDefault",
                                fieldCss: "field-wide",
                                labelText: Displays.OutgoingMails_To(),
                                text: ss.MailToDefault.ToStr())
                            .FieldTextBox(
                                textType: HtmlTypes.TextTypes.MultiLine,
                                controlId: "SiteSettings,MailCcDefault",
                                fieldCss: "field-wide",
                                labelText: Displays.OutgoingMails_Cc(),
                                text: ss.MailCcDefault.ToStr())
                            .FieldTextBox(
                                textType: HtmlTypes.TextTypes.MultiLine,
                                controlId: "SiteSettings,MailBccDefault",
                                fieldCss: "field-wide",
                                labelText: Displays.OutgoingMails_Bcc(),
                                text: ss.MailBccDefault.ToStr())));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder StyleSettingsEditor(this HtmlBuilder hb, SiteSettings ss)
        {
            return hb.FieldSet(
                id: "StyleSettingsEditor",
                action: () => hb
                    .FieldTextBox(
                        textType: HtmlTypes.TextTypes.MultiLine,
                        controlId: "SiteSettings,GridStyle",
                        fieldCss: "field-wide",
                        labelText: Displays.GridStyle(),
                        text: ss.GridStyle.ToStr())
                    .FieldTextBox(
                        textType: HtmlTypes.TextTypes.MultiLine,
                        controlId: "SiteSettings,NewStyle",
                        fieldCss: "field-wide",
                        labelText: Displays.NewStyle(),
                        text: ss.NewStyle.ToStr())
                    .FieldTextBox(
                        textType: HtmlTypes.TextTypes.MultiLine,
                        controlId: "SiteSettings,EditStyle",
                        fieldCss: "field-wide",
                        labelText: Displays.EditStyle(),
                        text: ss.EditStyle.ToStr()));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ScriptSettingsEditor(this HtmlBuilder hb, SiteSettings ss)
        {
            return hb.FieldSet(
                id: "ScriptSettingsEditor",
                action: () => hb
                    .FieldTextBox(
                        textType: HtmlTypes.TextTypes.MultiLine,
                        controlId: "SiteSettings,GridScript",
                        fieldCss: "field-wide",
                        labelText: Displays.GridScript(),
                        text: ss.GridScript.ToStr())
                    .FieldTextBox(
                        textType: HtmlTypes.TextTypes.MultiLine,
                        controlId: "SiteSettings,NewScript",
                        fieldCss: "field-wide",
                        labelText: Displays.NewScript(),
                        text: ss.NewScript.ToStr())
                    .FieldTextBox(
                        textType: HtmlTypes.TextTypes.MultiLine,
                        controlId: "SiteSettings,EditScript",
                        fieldCss: "field-wide",
                        labelText: Displays.EditScript(),
                        text: ss.EditScript.ToStr()));
        }
    }
}
