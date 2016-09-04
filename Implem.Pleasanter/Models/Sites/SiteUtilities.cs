using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Converts;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
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
                    statements: Rds.SelectSites(
                        column: Rds.SitesColumn().SiteId(),
                        where: DataViewFilters.Get(
                            siteSettings: siteSettings,
                            tableName: "Sites",
                            formData: formData,
                            where: Rds.SitesWhere().TenantId(Sessions.TenantId()).SiteId(siteId)),
                        orderBy: GridSorters.Get(
                            formData, Rds.SitesOrderBy().UpdatedTime(SqlOrderBy.Types.desc))))
                                .AsEnumerable()
                                .Select(o => o["SiteId"].ToLong())
                                .ToList();    
            }
            return switchTargets;
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

        public static string TitleDisplayValue(SiteSettings siteSettings, SiteModel siteModel)
        {
            var displayValue = siteSettings.TitleColumnCollection()
                .Select(column => TitleDisplayValue(column, siteModel))
                .Where(o => o != string.Empty)
                .Join(siteSettings.TitleSeparator);
            return displayValue != string.Empty
                ? displayValue
                : Displays.NoTitle();
        }

        private static string TitleDisplayValue(Column column, SiteModel siteModel)
        {
            switch (column.ColumnName)
            {
                case "Title": return column.HasChoices()
                    ? column.Choice(siteModel.Title.Value).Text()
                    : siteModel.Title.Value;
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
                    ? column.Choice(dataRow["Title"].ToString()).Text()
                    : dataRow["Title"].ToString();
                default: return string.Empty;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Editor(long siteId, bool clearSessions)
        {
            return Editor(
                new SiteModel(
                    siteId, clearSessions, methodType: BaseModel.MethodTypes.Edit),
                byRest: true);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder FieldTabs(this HtmlBuilder hb, SiteModel siteModel)
        {
            return hb.Ul(css: "tabmenu", action: () =>
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
        public static string SiteTop(SiteSettings siteSettings)
        {
            var hb = new HtmlBuilder();
            var permissionType = Permissions.Types.Manager;
            var verType = Versions.VerTypes.Latest;
            var siteConditions = SiteInfo.SiteMenu.SiteConditions(0);
            return hb.Template(
                siteId: 0,
                referenceType: "Sites",
                title: Displays.Top(),
                permissionType: permissionType,
                verType: verType,
                methodType: BaseModel.MethodTypes.Index,
                allowAccess: true,
                action: () =>
                {
                    hb.Form(
                        attributes: new HtmlAttributes()
                            .Id("SitesForm")
                            .Class("main-form")
                            .Action(Navigations.ItemAction(0)),
                        action: () => hb
                            .Nav(action: () => hb
                                .Ul(css: "nav-sites sortable", action: () =>
                                    Menu(0).ForEach(siteModelChild => hb
                                        .SiteMenu(
                                            siteSettings: siteSettings,
                                            permissionType: permissionType,
                                            siteId: siteModelChild.SiteId,
                                            referenceType: siteModelChild.ReferenceType,
                                            title: siteModelChild.Title.Value,
                                            siteConditions: siteConditions))))
                            .SiteMenuData());
                    hb.MainCommands(
                        siteId: 0,
                        permissionType: permissionType,
                        verType: verType,
                        backUrl: string.Empty);
                }).ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string SiteMenu(SiteModel siteModel)
        {
            var hb = new HtmlBuilder();
            var siteSettings = siteModel.SitesSiteSettings();
            var siteConditions = SiteInfo.SiteMenu.SiteConditions(siteModel.SiteId);
            return hb.Template(
                siteId: siteModel.SiteId,
                referenceType: "Sites",
                title: siteModel.Title.Value,
                permissionType: siteModel.PermissionType,
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.Index,
                allowAccess:
                    siteModel.PermissionType.CanRead() &&
                    siteModel.AccessStatus != Databases.AccessStatuses.NotFound,
                action: () =>
                {
                    hb.Form(
                        attributes: new HtmlAttributes()
                            .Id("SitesForm")
                            .Class("main-form")
                            .Action(Navigations.ItemAction(siteSettings.SiteId)),
                        action: () => hb
                            .Nav(css: "cf", action: () => hb
                                .Ul(css: "nav-sites", action: () => hb
                                    .ToParent(siteModel)))
                            .Nav(css: "cf", action: () => hb
                                .Ul(css: "nav-sites sortable", action: () =>
                                    Menu(siteSettings.SiteId).ForEach(siteModelChild => hb
                                        .SiteMenu(
                                            siteSettings: siteSettings,
                                            permissionType: siteModel.PermissionType,
                                            siteId: siteModelChild.SiteId,
                                            referenceType: siteModelChild.ReferenceType,
                                            title: siteModelChild.Title.Value,
                                            siteConditions: siteConditions))))
                            .SiteMenuData());
                    if (siteSettings.SiteId != 0)
                    {
                        hb.MainCommands(
                            siteId: siteModel.SiteId,
                            permissionType: siteModel.PermissionType,
                            verType: Versions.VerTypes.Latest,
                            backUrl: Navigations.ItemIndex(siteSettings.ParentId));
                    }
                }).ToString();
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
        private static HtmlBuilder ToParent(this HtmlBuilder hb, SiteModel siteModel)
        {
            return siteModel.SiteId != 0
                ? hb.SiteMenu(
                    siteSettings: siteModel.SiteSettings,
                    permissionType: siteModel.PermissionType,
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
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            long siteId,
            string referenceType,
            string title,
            bool toParent = false,
            IEnumerable<SiteCondition> siteConditions = null)
        {
            var binaryModel = new BinaryModel(permissionType, siteId);
            var hasImage = binaryModel.ExistsSiteImage(
                Libraries.Images.ImageData.SizeTypes.Thumbnail);
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
                            .Href(SiteHref(siteSettings, siteId, referenceType)),
                        action: () =>
                        {
                            if (toParent)
                            {
                                if (hasImage)
                                {
                                    hb
                                        .Img(
                                            src: Navigations.Get(
                                                "Items",
                                                siteId.ToString(),
                                                "Binaries",
                                                "SiteImageIcon",
                                                binaryModel.SiteImagePrefix(
                                                    Libraries.Images.ImageData.SizeTypes.Thumbnail)),
                                            css: "site-image-icon")
                                        .Span(css: "title", action: () => hb
                                            .Text(title));
                                }
                                else
                                {
                                    hb.Icon(
                                        iconCss: "ui-icon-circle-arrow-n",
                                        cssText: "title",
                                        text: title);
                                }
                            }
                            else
                            {
                                if (hasImage)
                                {
                                    hb.Img(
                                        src: Navigations.Get(
                                            "Items",
                                            siteId.ToString(),
                                            "Binaries",
                                            "SiteImageThumbnail",
                                            binaryModel.SiteImagePrefix(
                                                Libraries.Images.ImageData.SizeTypes.Thumbnail)),
                                        css: "site-image-thumbnail");
                                }
                                hb.Span(css: "title", action: () => hb
                                    .Text(title));
                            }
                            if (referenceType == "Sites")
                            {
                                hb.Div(css: "heading");
                            }
                            else
                            {
                                switch (referenceType)
                                {
                                    case "Wikis": break;
                                    default:
                                        hb.Div(css: "stacking1").Div(css: "stacking2");
                                        break;
                                }
                            }
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
                        }));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string SiteHref(
            SiteSettings siteSettings, long siteId, string referenceType)
        {
            switch (referenceType)
            {
                case "Wikis":
                    return Navigations.ItemEdit(Rds.ExecuteScalar_long(
                        statements: Rds.SelectWikis(
                            column: Rds.WikisColumn().WikiId(),
                            where: Rds.WikisWhere().SiteId(siteId))));
                default:
                    return Navigations.ItemIndex(siteId);
            }
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
        public static string EditorNew(Permissions.Types permissionType, long siteId)
        {
            return Editor(
                new SiteModel()
                {
                    SiteSettings = new SiteSettings("Sites"),
                    MethodType = BaseModel.MethodTypes.New,
                    SiteId = siteId,
                    PermissionType = permissionType
                },
                byRest: false);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Editor(SiteModel siteModel, bool byRest)
        {
            var hb = new HtmlBuilder();
            return hb.Template(
                siteId: siteModel.SiteId,
                referenceType: "Sites",
                title: siteModel.MethodType == BaseModel.MethodTypes.New
                    ? Displays.Sites() + " - " + Displays.New()
                    : siteModel.Title + " - " + Displays.EditSettings(),
                permissionType: siteModel.PermissionType,
                verType: siteModel.VerType,
                methodType: siteModel.MethodType,
                allowAccess: AllowAccess(siteModel),
                action: () => hb
                    .Editor(siteModel: siteModel)
                    .Hidden(controlId: "BaseUrl", value: Navigations.BaseUrl())
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
            return hb.Div(css: "edit-form", action: () => hb
                .Form(
                    attributes: new HtmlAttributes()
                        .Id("SiteForm")
                        .Class("main-form")
                        .Action(Navigations.ItemAction(siteModel.SiteId)),
                    action: () => hb
                        .RecordHeader(
                            id: siteModel.SiteId,
                            baseModel: siteModel,
                            tableName: "Sites",
                            switcher: false)
                        .Div(css: "edit-form-comments", action: () => hb
                            .Comments(
                                comments: siteModel.Comments,
                                verType: siteModel.VerType))
                        .Div(css: "edit-form-tabs", action: () => hb
                            .FieldTabs(siteModel: siteModel)
                            .FieldSetGeneral(siteModel: siteModel)
                            .FieldSet(
                                attributes: new HtmlAttributes()
                                    .Id("FieldSetHistories")
                                    .DataAction("Histories")
                                    .DataMethod("get"),
                                _using: siteModel.MethodType != BaseModel.MethodTypes.New)
                            .MainCommands(
                                siteId: siteModel.SiteId,
                                permissionType: siteModel.PermissionType,
                                verType: siteModel.VerType,
                                backUrl: EditorBackUrl(siteModel),
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
                            value: siteModel.Timestamp))
                .OutgoingMailsForm("Sites", siteModel.SiteId, siteModel.Ver)
                .CopyDialog("items", siteModel.SiteId)
                .OutgoingMailDialog());
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
                        ? Navigations.ItemEdit(wikiId)
                        : Navigations.ItemIndex(siteModel.ParentId);
                default:
                    return Navigations.ItemIndex(siteModel.SiteId);
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
                        attributes: siteModel.SiteSettings.GetColumn("Title").ValidationMessages(),
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
                        href: Navigations.ItemEdit(siteModel.SiteId, "Permissions"));
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
                            .MailerSettingsEditor(siteModel.SiteSettings)
                            .StyleSettingsEditor(siteModel.SiteSettings)
                            .ScriptSettingsEditor(siteModel.SiteSettings);
                        break;
                    default:
                        hb
                            .GridSettingsEditor(siteModel.SiteSettings)
                            .EditorSettingsEditor(siteModel.SiteSettings)
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
        private static HtmlBuilder SiteImageSettingsEditor(
            this HtmlBuilder hb, SiteSettings siteSettings)
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
                                controlCss: "button-save",
                                text: Displays.Setting(),
                                onClick: "$p.uploadSiteImage($(this));",
                                action: "binaries/updatesiteimage",
                                method: "post")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder GridSettingsEditor(
            this HtmlBuilder hb, SiteSettings siteSettings)
        {
            return hb.FieldSet(
                id: "GridSettingsEditor",
                action: () => hb
                    .GridColumns(siteSettings)
                    .FilterColumns(siteSettings)
                    .Aggregations(siteSettings)
                    .FieldSpinner(
                        controlId: "SiteSettings,GridPageSize",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.SettingGridPageSize(),
                        value: siteSettings.GridPageSize.ToDecimal(),
                        min: Parameters.General.GridPageSizeMin,
                        max: Parameters.General.GridPageSizeMax,
                        step: 1,
                        width: 25)
                    .FieldSpinner(
                        controlId: "SiteSettings,NearCompletionTimeBeforeDays",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.SettingNearCompletionTimeBeforeDays(),
                        value: siteSettings.NearCompletionTimeBeforeDays.ToDecimal(),
                        min: Parameters.General.NearCompletionTimeBeforeDaysMin,
                        max: Parameters.General.NearCompletionTimeBeforeDaysMax,
                        step: 1,
                        width: 25)
                    .FieldSpinner(
                        controlId: "SiteSettings,NearCompletionTimeAfterDays",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.SettingNearCompletionTimeAfterDays(),
                        value: siteSettings.NearCompletionTimeAfterDays.ToDecimal(),
                        min: Parameters.General.NearCompletionTimeAfterDaysMin,
                        max: Parameters.General.NearCompletionTimeAfterDaysMax,
                        step: 1,
                        width: 25)
                    .AggregationDetailsDialog(siteSettings));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder GridColumns(this HtmlBuilder hb, SiteSettings siteSettings)
        {
            return hb.FieldSet(
                css: " enclosed-thin",
                legendText: Displays.SettingGridColumns(),
                action: () => hb
                    .FieldSelectable(
                        controlId: "GridColumns",
                        fieldCss: "field-vertical",
                        controlContainerCss: "container-selectable",
                        controlCss: " h350",
                        labelText: Displays.ShowList(),
                        listItemCollection: siteSettings.GridSelectableItems(),
                        selectedValueCollection: new List<string>(),
                        commandOptionPositionIsTop: true,
                        commandOptionAction: () => hb
                            .Div(css: "command-center", action: () => hb
                                .Button(
                                    controlId: "MoveUpGridColumns",
                                    controlCss: "button-up",
                                    text: Displays.MoveUp(),
                                    onClick: "$p.send($(this));",
                                    action: "SetSiteSettings",
                                    method: "post")
                                .Button(
                                    controlId: "MoveDownGridColumns",
                                    controlCss: "button-down",
                                    text: Displays.MoveDown(),
                                    onClick: "$p.send($(this));",
                                    action: "SetSiteSettings",
                                    method: "post")
                                .Button(
                                    controlId: "HideGridColumns",
                                    controlCss: "button-hide",
                                    text: Displays.Hide(),
                                    onClick: "$p.send($(this));",
                                    action: "SetSiteSettings",
                                    method: "put")))
                    .FieldSelectable(
                        controlId: "GridSourceColumns",
                        fieldCss: "field-vertical",
                        controlContainerCss: "container-selectable",
                        controlCss: " h350",
                        labelText: Displays.HideList(),
                        listItemCollection: siteSettings.GridSelectableItems(visible: false),
                        commandOptionPositionIsTop: true,
                        commandOptionAction: () => hb
                            .Div(css: "command-center", action: () => hb
                                .Button(
                                    controlId: "ShowGridColumns",
                                    text: Displays.Show(),
                                    controlCss: "button-visible",
                                    onClick: "$p.send($(this));",
                                    action: "SetSiteSettings",
                                    method: "put"))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder FilterColumns(this HtmlBuilder hb, SiteSettings siteSettings)
        {
            return hb.FieldSet(
                css: " enclosed-thin",
                legendText: Displays.SettingFilterColumns(),
                action: () => hb
                    .FieldSelectable(
                        controlId: "FilterColumns",
                        fieldCss: "field-vertical",
                        controlContainerCss: "container-selectable",
                        controlCss: " h350",
                        labelText: Displays.ShowList(),
                        listItemCollection: siteSettings.FilterSelectableItems(),
                        selectedValueCollection: new List<string>(),
                        commandOptionPositionIsTop: true,
                        commandOptionAction: () => hb
                            .Div(css: "command-center", action: () => hb
                                .Button(
                                    controlId: "MoveUpFilterColumns",
                                    controlCss: "button-up",
                                    text: Displays.MoveUp(),
                                    onClick: "$p.send($(this));",
                                    action: "SetSiteSettings",
                                    method: "post")
                                .Button(
                                    controlId: "MoveDownFilterColumns",
                                    controlCss: "button-down",
                                    text: Displays.MoveDown(),
                                    onClick: "$p.send($(this));",
                                    action: "SetSiteSettings",
                                    method: "post")
                                .Button(
                                    controlId: "HideFilterColumns",
                                    controlCss: "button-hide",
                                    text: Displays.Hide(),
                                    onClick: "$p.send($(this));",
                                    action: "SetSiteSettings",
                                    method: "put")))
                    .FieldSelectable(
                        controlId: "FilterSourceColumns",
                        fieldCss: "field-vertical",
                        controlContainerCss: "container-selectable",
                        controlCss: " h350",
                        labelText: Displays.HideList(),
                        listItemCollection: siteSettings.FilterSelectableItems(visible: false),
                        commandOptionPositionIsTop: true,
                        commandOptionAction: () => hb
                            .Div(css: "command-center", action: () => hb
                                .Button(
                                    controlId: "ShowFilterColumns",
                                    text: Displays.Show(),
                                    controlCss: "button-visible",
                                    onClick: "$p.send($(this));",
                                    action: "SetSiteSettings",
                                    method: "put"))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Aggregations(this HtmlBuilder hb, SiteSettings siteSettings)
        {
            return hb.FieldSet(
                css: " enclosed-thin",
                legendText: Displays.SettingAggregations(),
                action: () => hb
                    .FieldSelectable(
                        controlId: "AggregationDestination",
                        fieldCss: "field-vertical both",
                        controlContainerCss: "container-selectable",
                        controlCss: " h350",
                        labelText: Displays.SettingAggregationList(),
                        listItemCollection: siteSettings.AggregationDestination(),
                        selectedValueCollection: new List<string>(),
                        commandOptionPositionIsTop: true,
                        commandOptionAction: () => hb
                            .Div(css: "command-center", action: () => hb
                                .Button(
                                    controlId: "MoveUpAggregations",
                                    controlCss: "button-up",
                                    text: Displays.MoveUp(),
                                    onClick: "$p.send($(this));",
                                    action: "SetSiteSettings",
                                    method: "post")
                                .Button(
                                    controlId: "MoveDownAggregations",
                                    controlCss: "button-down",
                                    text: Displays.MoveDown(),
                                    onClick: "$p.send($(this));",
                                    action: "SetSiteSettings",
                                    method: "post")
                                .Button(
                                    text: Displays.AdvancedSetting(),
                                    controlCss: "button-setting open-dialog",
                                    onClick: "$p.openDialog($(this));",
                                    selector: "#AggregationDetailsDialog")
                                .Button(
                                    controlId: "DeleteAggregations",
                                    controlCss: "button-to-right",
                                    text: Displays.Delete(),
                                    onClick: "$p.send($(this));",
                                    action: "SetSiteSettings",
                                    method: "put")))
                    .FieldSelectable(
                        controlId: "AggregationSource",
                        fieldCss: "field-vertical",
                        controlContainerCss: "container-selectable",
                        controlCss: " h350",
                        labelText: Displays.SettingSelectionList(),
                        listItemCollection: siteSettings.AggregationSource(),
                        selectedValueCollection: new List<string>(),
                        commandOptionPositionIsTop: true,
                        commandOptionAction: () => hb
                            .Div(css: "command-center", action: () => hb
                                .Button(
                                    controlId: "AddAggregations",
                                    controlCss: "button-to-left",
                                    text: Displays.Add(),
                                    onClick: "$p.send($(this));",
                                    action: "SetSiteSettings",
                                    method: "post"))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder AggregationDetailsDialog(
            this HtmlBuilder hb, SiteSettings siteSettings)
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
                            .Where(o => o.TableName == siteSettings.ReferenceType)
                            .Where(o => o.Computable)
                            .Where(o => o.TypeName != "datetime")
                            .ToDictionary(
                                o => o.ColumnName,
                                o => siteSettings.GetColumn(o.ColumnName).LabelText))
                    .P(css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "SetAggregationDetails",
                            text: Displays.Setting(),
                            controlCss: "button-setting",
                            onClick: "$p.setAggregationDetails($(this));",
                            action: "SetSiteSettings",
                            method: "post")
                        .Button(
                            text: Displays.Cancel(),
                            controlCss: "button-cancel",
                            onClick: "$p.closeDialog($(this));")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditorSettingsEditor(
            this HtmlBuilder hb, SiteSettings siteSettings)
        {
            return hb.FieldSet(
                id: "EditorSettingsEditor",
                action: () => hb
                    .SiteSettingEditorColumns(siteSettings)
                    .SiteSettingLinkColumns(siteSettings)
                    .SiteSettingHistoryColumns(siteSettings)
                    .SiteSettingFormulas(siteSettings));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SiteSettingEditorColumns(
            this HtmlBuilder hb, SiteSettings siteSettings)
        {
            return hb.FieldSet(
                legendText: Displays.SettingEditorColumns(),
                css: " enclosed",
                action: () => hb
                    .FieldSelectable(
                        controlId: "EditorColumns",
                        fieldCss: "field-vertical",
                        controlContainerCss: "container-selectable",
                        controlCss: " h350",
                        labelText: Displays.ShowList(),
                        listItemCollection: siteSettings.EditorSelectableItems(),
                        commandOptionPositionIsTop: true,
                        commandOptionAction: () => hb
                            .Div(css: "command-center", action: () => hb
                                .Button(
                                    controlId: "MoveUpEditorColumns",
                                    text: Displays.MoveUp(),
                                    controlCss: "button-up",
                                    onClick: "$p.send($(this));",
                                    action: "SetSiteSettings",
                                    method: "post")
                                .Button(
                                    controlId: "MoveDownEditorColumns",
                                    text: Displays.MoveDown(),
                                    controlCss: "button-down",
                                    onClick: "$p.send($(this));",
                                    action: "SetSiteSettings",
                                    method: "post")
                                .Button(
                                    controlId: "HideEditorColumns",
                                    text: Displays.Hide(),
                                    controlCss: "button-hide",
                                    onClick: "$p.send($(this));",
                                    action: "SetSiteSettings",
                                    method: "put")
                                .Button(
                                    controlId: "OpenColumnPropertiesDialog",
                                    text: Displays.AdvancedSetting(),
                                    controlCss: "button-setting",
                                    onClick: "$p.openColumnPropertiesDialog($(this));",
                                    action: "SetSiteSettings",
                                    method: "put")))
                    .FieldSelectable(
                        controlId: "EditorSourceColumns",
                        fieldCss: "field-vertical",
                        controlContainerCss: "container-selectable",
                        controlCss: " h350",
                        labelText: Displays.HideList(),
                        listItemCollection: siteSettings.EditorSelectableItems(visible: false),
                        commandOptionPositionIsTop: true,
                        commandOptionAction: () => hb
                            .Div(css: "command-center", action: () => hb
                                .Button(
                                    controlId: "ShowEditorColumns",
                                    text: Displays.Show(),
                                    controlCss: "button-visible",
                                    onClick: "$p.send($(this));",
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
        private static HtmlBuilder SiteSettingLinkColumns(
            this HtmlBuilder hb, SiteSettings siteSettings)
        {
            return hb.FieldSet(
                legendText: Displays.SettingLinkColumns(),
                css: " enclosed",
                action: () => hb
                    .FieldSelectable(
                        controlId: "LinkColumns",
                        fieldCss: "field-vertical",
                        controlContainerCss: "container-selectable",
                        controlCss: " h350",
                        labelText: Displays.ShowList(),
                        listItemCollection: siteSettings.LinkSelectableItems(),
                        commandOptionPositionIsTop: true,
                        commandOptionAction: () => hb
                            .Div(css: "command-center", action: () => hb
                                .Button(
                                    controlId: "MoveUpLinkColumns",
                                    text: Displays.MoveUp(),
                                    controlCss: "button-up",
                                    onClick: "$p.send($(this));",
                                    action: "SetSiteSettings",
                                    method: "post")
                                .Button(
                                    controlId: "MoveDownLinkColumns",
                                    text: Displays.MoveDown(),
                                    controlCss: "button-down",
                                    onClick: "$p.send($(this));",
                                    action: "SetSiteSettings",
                                    method: "post")
                                .Button(
                                    controlId: "HideLinkColumns",
                                    text: Displays.Hide(),
                                    controlCss: "button-hide",
                                    onClick: "$p.send($(this));",
                                    action: "SetSiteSettings",
                                    method: "put")))
                    .FieldSelectable(
                        controlId: "LinkSourceColumns",
                        fieldCss: "field-vertical",
                        controlContainerCss: "container-selectable",
                        controlCss: " h350",
                        labelText: Displays.HideList(),
                        listItemCollection: siteSettings.LinkSelectableItems(visible: false),
                        commandOptionPositionIsTop: true,
                        commandOptionAction: () => hb
                            .Div(css: "command-center", action: () => hb
                                .Button(
                                    controlId: "ShowLinkColumns",
                                    text: Displays.Show(),
                                    controlCss: "button-visible",
                                    onClick: "$p.send($(this));",
                                    action: "SetSiteSettings",
                                    method: "put"))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SiteSettingHistoryColumns(
            this HtmlBuilder hb, SiteSettings siteSettings)
        {
            return hb.FieldSet(
                legendText: Displays.SettingHistoryColumns(),
                css: " enclosed",
                action: () => hb
                    .FieldSelectable(
                        controlId: "HistoryColumns",
                        fieldCss: "field-vertical",
                        controlContainerCss: "container-selectable",
                        controlCss: " h350",
                        labelText: Displays.ShowList(),
                        listItemCollection: siteSettings.HistorySelectableItems(),
                        commandOptionPositionIsTop: true,
                        commandOptionAction: () => hb
                            .Div(css: "command-center", action: () => hb
                                .Button(
                                    controlId: "MoveUpHistoryColumns",
                                    text: Displays.MoveUp(),
                                    controlCss: "button-up",
                                    onClick: "$p.send($(this));",
                                    action: "SetSiteSettings",
                                    method: "post")
                                .Button(
                                    controlId: "MoveDownHistoryColumns",
                                    text: Displays.MoveDown(),
                                    controlCss: "button-down",
                                    onClick: "$p.send($(this));",
                                    action: "SetSiteSettings",
                                    method: "post")
                                .Button(
                                    controlId: "HideHistoryColumns",
                                    text: Displays.Hide(),
                                    controlCss: "button-hide",
                                    onClick: "$p.send($(this));",
                                    action: "SetSiteSettings",
                                    method: "put")))
                    .FieldSelectable(
                        controlId: "HistorySourceColumns",
                        fieldCss: "field-vertical",
                        controlContainerCss: "container-selectable",
                        controlCss: " h350",
                        labelText: Displays.HideList(),
                        listItemCollection: siteSettings.HistorySelectableItems(visible: false),
                        commandOptionPositionIsTop: true,
                        commandOptionAction: () => hb
                            .Div(css: "command-center", action: () => hb
                                .Button(
                                    controlId: "ShowHistoryColumns",
                                    text: Displays.Show(),
                                    controlCss: "button-visible",
                                    onClick: "$p.send($(this));",
                                    action: "SetSiteSettings",
                                    method: "put"))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder ColumnProperties(SiteSettings siteSettings, Column column)
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
                                optionCollection: DateTimeOptions(),
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
                        hb.TitleColumnProperty(siteSettings);
                    }
                });
            return hb
                .P(css: "message-dialog")
                .Div(css: "command-center", action: () => hb
                    .Button(
                        controlId: "SetColumnProperties",
                        text: Displays.Setting(),
                        controlCss: "button-setting",
                        onClick: "$p.sendByDialog($(this));",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        text: Displays.Cancel(),
                        controlCss: "button-cancel",
                        onClick: "$p.closeDialog($(this));"));
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
            this HtmlBuilder hb, SiteSettings siteSettings)
        {
            return hb.FieldSelectable(
                controlId: "TitleColumns",
                fieldCss: "field-vertical",
                controlContainerCss: "container-selectable",
                controlCss: " h350",
                labelText: Displays.ShowList(),
                listItemCollection: siteSettings.TitleSelectableItems(),
                commandOptionPositionIsTop: true,
                commandOptionAction: () => hb
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "MoveUpTitleColumns",
                            text: Displays.MoveUp(),
                            controlCss: "button-up",
                            onClick: "$p.send($(this));",
                            action: "SetSiteSettings",
                            method: "post")
                        .Button(
                            controlId: "MoveDownTitleColumns",
                            text: Displays.MoveDown(),
                            controlCss: "button-down",
                            onClick: "$p.send($(this));",
                            action: "SetSiteSettings",
                            method: "post")
                        .Button(
                            controlId: "HideTitleColumns",
                            text: Displays.Hide(),
                            controlCss: "button-hide",
                            onClick: "$p.send($(this));",
                            action: "SetSiteSettings",
                            method: "put")))
            .FieldSelectable(
                controlId: "TitleSourceColumns",
                fieldCss: "field-vertical",
                controlContainerCss: "container-selectable",
                controlCss: " h350",
                labelText: Displays.HideList(),
                listItemCollection: siteSettings.TitleSelectableItems(visible: false),
                commandOptionPositionIsTop: true,
                commandOptionAction: () => hb
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "ShowTitleColumns",
                            text: Displays.Show(),
                            controlCss: "button-visible",
                            onClick: "$p.send($(this));",
                            action: "SetSiteSettings",
                            method: "put")))
            .FieldTextBox(
                controlId: "SiteSettings,TitleSeparator",
                fieldCss: " both",
                labelText: Displays.SettingTitleSeparator(),
                text: siteSettings.TitleSeparator);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static Dictionary<string, string> DateTimeOptions()
        {
            return Def.DisplayDefinitionCollection
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
        private static HtmlBuilder SiteSettingFormulas(
            this HtmlBuilder hb, SiteSettings siteSettings)
        {
            return hb.FieldSet(
                legendText: Displays.SettingFormulas(),
                css: " enclosed",
                action: () => hb
                    .FieldSelectable(
                        controlId: "Formulas",
                        fieldCss: "field-vertical w600",
                        controlContainerCss: "container-selectable",
                        controlCss: " h200",
                        labelText: Displays.SettingColumnList(),
                        listItemCollection: siteSettings.FormulaItemCollection(),
                        commandOptionAction: () => hb
                            .Div(css: "command-left", action: () => hb
                                .TextBox(
                                    controlId: "Formula",
                                    controlCss: " w250")
                                .Button(
                                    controlId: "AddFormula",
                                    controlCss: "button-create",
                                    text: Displays.Add(),
                                    onClick: "$p.send($(this));",
                                    action: "SetSiteSettings",
                                    method: "post")
                                .Button(
                                    controlId: "MoveUpFormulas",
                                    controlCss: "button-up",
                                    text: Displays.MoveUp(),
                                    onClick: "$p.send($(this));",
                                    action: "SetSiteSettings",
                                    method: "post")
                                .Button(
                                    controlId: "MoveDownFormulas",
                                    controlCss: "button-down",
                                    text: Displays.MoveDown(),
                                    onClick: "$p.send($(this));",
                                    action: "SetSiteSettings",
                                    method: "post")
                                .Button(
                                    controlId: "DeleteFormulas",
                                    controlCss: "button-delete",
                                    text: Displays.Delete(),
                                    onClick: "$p.send($(this));",
                                    action: "SetSiteSettings",
                                    method: "post")
                                .Button(
                                    controlId: "SynchronizeFormulas",
                                    controlCss: "button-synchronize",
                                    text: Displays.Synchronize(),
                                    onClick: "$p.send($(this));",
                                    action: "SynchronizeFormulas",
                                    method: "put",
                                    confirm: Displays.ConfirmSynchronize()))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SummarySettingsEditor(
            this HtmlBuilder hb, SiteSettings siteSettings)
        {
            var siteDataRows = siteSettings.SummarySiteDataRows();
            if (siteDataRows == null)
            {
                return hb.SummarySettingsEditorNoLinks();
            }
            var summarySiteIdHash = SummarySiteIdHash(siteDataRows, siteSettings);
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
                                    referenceType: siteSettings.ReferenceType,
                                    siteDataRows: siteDataRows)
                                .SummaryLinkColumn(
                                    siteId: firstSiteId,
                                    siteSettings: siteSettings)
                                .FieldDropDown(
                                    controlId: "SummaryType",
                                    controlCss: " auto-postback",
                                    labelText: Displays.SummaryType(),
                                    optionCollection: SummaryTypeCollection(),
                                    action: "SetSiteSettings",
                                    method: "post")
                                .SummarySourceColumn(siteSettings)
                                .FieldContainer(actionOptions: () => hb
                                    .Div(css: "buttons", action: () => hb
                                        .Button(
                                            controlId: "AddSummary",
                                            text: Displays.Add(),
                                            controlCss: "button-create",
                                            onClick: "$p.addSummary($(this));",
                                            action: "SetSiteSettings",
                                            method: "put")))
                                .SummarySettings(sourceSiteSettings: siteSettings)))
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
            EnumerableRowCollection<DataRow> summarySiteCollection,
            SiteSettings siteSettings)
        {
            return summarySiteCollection
                .OrderBy(o =>
                    o["SiteId"].ToLong() != siteSettings.SiteId)
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
            this HtmlBuilder hb,
            long siteId,
            SiteSettings siteSettings)
        {
            return hb.FieldDropDown(
                fieldId: "SummaryLinkColumnField",
                controlId: "SummaryLinkColumn",
                labelText: Displays.SummaryLinkColumn(),
                optionCollection: siteSettings.LinkColumnSiteIdHash
                    .Where(o => o.Value == siteId)
                    .ToDictionary(
                        o => o.Key.Split('_')._1st(),
                        o => siteSettings.GetColumn(o.Key.Split('_')._1st()).LabelText),
                action: "SetSiteSettings",
                method: "post");
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder SummarySourceColumn(
            this HtmlBuilder hb, SiteSettings siteSettings, string type = "Count")
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
                        optionCollection: siteSettings.ColumnCollection
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
            return hb.Div(id: "SummarySettings", action: () =>
            {
                hb.Table(css: "grid", action: () =>
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
                                                controlId: "SynchronizeSummary," + summary.Id,
                                                controlCss: "button-synchronize",
                                                text: Displays.Synchronize(),
                                                onClick: "$p.send($(this));",
                                                action: "SynchronizeSummary",
                                                method: "put",
                                                confirm: Displays.ConfirmSynchronize())
                                            .Button(
                                                controlId: "DeleteSummary," + summary.Id,
                                                controlCss: "button-delete",
                                                text: Displays.Delete(),
                                                onClick: "$p.send($(this));",
                                                action: "SetSiteSettings",
                                                method: "delete")));
                                }
                            });
                        });
                    }
                });
            });
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
        private static HtmlBuilder MailerSettingsEditor(
            this HtmlBuilder hb, SiteSettings siteSettings)
        {
            return hb.FieldSet(
                id: "MailerSettingsEditor",
                action: () => hb
                    .FieldTextBox(
                        textType: HtmlTypes.TextTypes.MultiLine,
                        controlId: "SiteSettings,AddressBook",
                        fieldCss: "field-wide",
                        labelText: Displays.DefaultAddressBook(),
                        text: siteSettings.AddressBook.ToStr())
                    .FieldSet(
                        legendText: Displays.DefaultDestinations(),
                        css: " enclosed-thin",
                        action: () => hb
                            .FieldTextBox(
                                textType: HtmlTypes.TextTypes.MultiLine,
                                controlId: "SiteSettings,MailToDefault",
                                fieldCss: "field-wide",
                                labelText: Displays.OutgoingMails_To(),
                                text: siteSettings.MailToDefault.ToStr())
                            .FieldTextBox(
                                textType: HtmlTypes.TextTypes.MultiLine,
                                controlId: "SiteSettings,MailCcDefault",
                                fieldCss: "field-wide",
                                labelText: Displays.OutgoingMails_Cc(),
                                text: siteSettings.MailCcDefault.ToStr())
                            .FieldTextBox(
                                textType: HtmlTypes.TextTypes.MultiLine,
                                controlId: "SiteSettings,MailBccDefault",
                                fieldCss: "field-wide",
                                labelText: Displays.OutgoingMails_Bcc(),
                                text: siteSettings.MailBccDefault.ToStr())));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder StyleSettingsEditor(
            this HtmlBuilder hb, SiteSettings siteSettings)
        {
            return hb.FieldSet(
                id: "StyleSettingsEditor",
                action: () => hb
                    .FieldTextBox(
                        textType: HtmlTypes.TextTypes.MultiLine,
                        controlId: "SiteSettings,GridStyle",
                        fieldCss: "field-wide",
                        labelText: Displays.GridStyle(),
                        text: siteSettings.GridStyle.ToStr())
                    .FieldTextBox(
                        textType: HtmlTypes.TextTypes.MultiLine,
                        controlId: "SiteSettings,NewStyle",
                        fieldCss: "field-wide",
                        labelText: Displays.NewStyle(),
                        text: siteSettings.NewStyle.ToStr())
                    .FieldTextBox(
                        textType: HtmlTypes.TextTypes.MultiLine,
                        controlId: "SiteSettings,EditStyle",
                        fieldCss: "field-wide",
                        labelText: Displays.EditStyle(),
                        text: siteSettings.EditStyle.ToStr()));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ScriptSettingsEditor(
            this HtmlBuilder hb, SiteSettings siteSettings)
        {
            return hb.FieldSet(
                id: "ScriptSettingsEditor",
                action: () => hb
                    .FieldTextBox(
                        textType: HtmlTypes.TextTypes.MultiLine,
                        controlId: "SiteSettings,GridScript",
                        fieldCss: "field-wide",
                        labelText: Displays.GridScript(),
                        text: siteSettings.GridScript.ToStr())
                    .FieldTextBox(
                        textType: HtmlTypes.TextTypes.MultiLine,
                        controlId: "SiteSettings,NewScript",
                        fieldCss: "field-wide",
                        labelText: Displays.NewScript(),
                        text: siteSettings.NewScript.ToStr())
                    .FieldTextBox(
                        textType: HtmlTypes.TextTypes.MultiLine,
                        controlId: "SiteSettings,EditScript",
                        fieldCss: "field-wide",
                        labelText: Displays.EditScript(),
                        text: siteSettings.EditScript.ToStr()));
        }
    }
}
