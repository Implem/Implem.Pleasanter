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
    public static class OutgoingMailUtilities
    {
        public static string Index(SiteSettings siteSettings, Permissions.Types permissionType)
        {
            var hb = new HtmlBuilder();
            var formData = DataViewFilters.SessionFormData();
            var outgoingMailCollection = OutgoingMailCollection(siteSettings, permissionType, formData);
            var dataViewName = DataViewSelectors.Get(siteSettings.SiteId);
            return hb.Template(
                siteId: siteSettings.SiteId,
                referenceType: "OutgoingMails",
                title: siteSettings.Title + " - " + Displays.List(),
                permissionType: permissionType,
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.Index,
                allowAccess: permissionType.CanRead(),
                script: Libraries.Scripts.JavaScripts.DataView(
                    siteSettings: siteSettings,
                    permissionType: permissionType,
                    formData: formData,
                    dataViewName: dataViewName),
                userScript: siteSettings.GridScript,
                userStyle: siteSettings.GridStyle,
                action: () => hb
                    .Form(
                        attributes: new HtmlAttributes()
                            .Id_Css("OutgoingMailsForm", "main-form")
                            .Action(Navigations.ItemAction(siteSettings.SiteId)),
                        action: () => hb
                            .DataViewSelector(
                                referenceType: "OutgoingMails",
                                dataViewName: dataViewName)
                            .DataViewFilters(
                                siteSettings: siteSettings,
                                siteId: siteSettings.SiteId)
                            .Aggregations(
                                siteSettings: siteSettings,
                                aggregations: outgoingMailCollection.Aggregations)
                            .Div(id: "DataViewContainer", action: () => hb
                                .DataView(
                                    outgoingMailCollection: outgoingMailCollection,
                                    siteSettings: siteSettings,
                                    permissionType: permissionType,
                                    formData: formData,
                                    dataViewName: dataViewName))
                            .MainCommands(
                                siteId: siteSettings.SiteId,
                                permissionType: permissionType,
                                verType: Versions.VerTypes.Latest,
                                backUrl: Navigations.Index("Admins"),
                                bulkMoveButton: true,
                                bulkDeleteButton: true,
                                importButton: true,
                                exportButton: true)
                            .Div(css: "margin-bottom")
                            .Hidden(controlId: "TableName", value: "OutgoingMails")
                            .Hidden(controlId: "BaseUrl", value: Navigations.BaseUrl()))
                .MoveDialog("items", siteSettings.SiteId, bulk: true)
                .Div(attributes: new HtmlAttributes()
                    .Id_Css("ExportSettingsDialog", "dialog")
                    .Title(Displays.ExportSettings())))
                .ToString();
        }

        private static OutgoingMailCollection OutgoingMailCollection(
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData, int offset = 0)
        {
            return new OutgoingMailCollection(
                column: GridSqlColumnCollection(siteSettings),
                where: DataViewFilters.Get(
                    siteSettings: siteSettings,
                    tableName: "OutgoingMails",
                    formData: formData,
                    where: Rds.OutgoingMailsWhere()),
                orderBy: GridSorters.Get(
                    formData, Rds.OutgoingMailsOrderBy().UpdatedTime(SqlOrderBy.Types.desc)),
                offset: offset,
                pageSize: siteSettings.GridPageSize.ToInt(),
                countRecord: true,
                aggregationCollection: siteSettings.AggregationCollection);
        }

        public static HtmlBuilder DataView(
            this HtmlBuilder hb,
            OutgoingMailCollection outgoingMailCollection,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData,
            string dataViewName)
        {
            switch (dataViewName)
            {
                default: return hb.Grid(
                    outgoingMailCollection: outgoingMailCollection,
                    siteSettings: siteSettings,
                    permissionType: permissionType,
                    formData: formData);
            }
        }

        public static string DataView(
            SiteSettings siteSettings, Permissions.Types permissionType)
        {
            switch (DataViewSelectors.Get(siteSettings.SiteId))
            {
                default: return Grid(siteSettings: siteSettings, permissionType: permissionType);
            }
        }

        private static HtmlBuilder Grid(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            OutgoingMailCollection outgoingMailCollection,
            FormData formData)
        {
            return hb
                .Table(
                    attributes: new HtmlAttributes()
                        .Id_Css("Grid", "grid")
                        .DataAction("GridRows")
                        .DataMethod("post"),
                    action: () => hb
                        .GridRows(
                            siteSettings: siteSettings,
                            outgoingMailCollection: outgoingMailCollection,
                            formData: formData))
                .Hidden(
                    controlId: "GridOffset",
                    value: siteSettings.GridPageSize == outgoingMailCollection.Count()
                        ? siteSettings.GridPageSize.ToString()
                        : "-1");
        }

        private static string Grid(SiteSettings siteSettings, Permissions.Types permissionType)
        {
            var formData = DataViewFilters.SessionFormData();
            var outgoingMailCollection = OutgoingMailCollection(siteSettings, permissionType, formData);
            return new ResponseCollection()
                .Html("#DataViewContainer", new HtmlBuilder().Grid(
                    siteSettings: siteSettings,
                    outgoingMailCollection: outgoingMailCollection,
                    permissionType: permissionType,
                    formData: formData))
                .Html("#Aggregations", new HtmlBuilder().Aggregations(
                    siteSettings: siteSettings,
                    aggregations: outgoingMailCollection.Aggregations,
                    container: false))
                .WindowScrollTop().ToJson();
        }

        public static string GridRows(
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            ResponseCollection responseCollection = null,
            int offset = 0,
            bool clearCheck = false,
            Message message = null)
        {
            var formData = DataViewFilters.SessionFormData();
            var outgoingMailCollection = OutgoingMailCollection(siteSettings, permissionType, formData, offset);
            return (responseCollection ?? new ResponseCollection())
                .Remove(".grid tr", _using: offset == 0)
                .ClearFormData("GridCheckAll", _using: clearCheck)
                .ClearFormData("GridUnCheckedItems", _using: clearCheck)
                .ClearFormData("GridCheckedItems", _using: clearCheck)
                .Message(message)
                .Append("#Grid", new HtmlBuilder().GridRows(
                    siteSettings: siteSettings,
                    outgoingMailCollection: outgoingMailCollection,
                    formData: formData,
                    addHeader: offset == 0,
                    clearCheck: clearCheck))
                .Html("#Aggregations", new HtmlBuilder().Aggregations(
                    siteSettings: siteSettings,
                    aggregations: outgoingMailCollection.Aggregations,
                    container: false))
                .Val("#GridOffset", siteSettings.NextPageOffset(offset, outgoingMailCollection.Count()))
                .Markup()
                .ToJson();
        }

        private static HtmlBuilder GridRows(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            OutgoingMailCollection outgoingMailCollection,
            FormData formData,
            bool addHeader = true,
            bool clearCheck = false)
        {
            var checkAll = clearCheck ? false : Forms.Bool("GridCheckAll");
            if (addHeader)
            {
                hb.GridHeader(
                    columnCollection: siteSettings.GridColumnCollection(), 
                    formData: formData,
                    checkAll: checkAll);
            }
            outgoingMailCollection.ForEach(outgoingMailModel => hb
                .Tr(
                    attributes: new HtmlAttributes()
                        .Class("grid-row")
                        .DataId(outgoingMailModel.OutgoingMailId.ToString()),
                    action: () =>
                    {
                        hb.Td(action: () => hb
                            .CheckBox(
                                controlCss: "grid-check",
                                _checked: checkAll,
                                dataId: outgoingMailModel.OutgoingMailId.ToString()));
                        siteSettings.GridColumnCollection()
                            .ForEach(column => hb
                                .TdValue(
                                    column: column,
                                    outgoingMailModel: outgoingMailModel));
                    }));
            return hb;
        }

        private static SqlColumnCollection GridSqlColumnCollection(SiteSettings siteSettings)
        {
            var select = Rds.OutgoingMailsColumn()
                .OutgoingMailId()
                .Creator()
                .Updator();
            siteSettings.GridColumnCollection(withTitle: true).ForEach(columnGrid =>
            {
                switch (columnGrid.ColumnName)
                {
                    case "ReferenceType": select.ReferenceType(); break;
                    case "ReferenceId": select.ReferenceId(); break;
                    case "ReferenceVer": select.ReferenceVer(); break;
                    case "OutgoingMailId": select.OutgoingMailId(); break;
                    case "Ver": select.Ver(); break;
                    case "Host": select.Host(); break;
                    case "Port": select.Port(); break;
                    case "From": select.From(); break;
                    case "To": select.To(); break;
                    case "Cc": select.Cc(); break;
                    case "Bcc": select.Bcc(); break;
                    case "Title": select.Title(); break;
                    case "Body": select.Body(); break;
                    case "SentTime": select.SentTime(); break;
                    case "Comments": select.Comments(); break;
                    case "Creator": select.Creator(); break;
                    case "Updator": select.Updator(); break;
                    case "CreatedTime": select.CreatedTime(); break;
                    case "UpdatedTime": select.UpdatedTime(); break;
                }
            });
            return select;
        }

        public static HtmlBuilder TdValue(
            this HtmlBuilder hb, Column column, OutgoingMailModel outgoingMailModel)
        {
            switch (column.ColumnName)
            {
                case "Ver": return hb.Td(column: column, value: outgoingMailModel.Ver);
                case "Comments": return hb.Td(column: column, value: outgoingMailModel.Comments);
                case "Creator": return hb.Td(column: column, value: outgoingMailModel.Creator);
                case "Updator": return hb.Td(column: column, value: outgoingMailModel.Updator);
                case "CreatedTime": return hb.Td(column: column, value: outgoingMailModel.CreatedTime);
                case "UpdatedTime": return hb.Td(column: column, value: outgoingMailModel.UpdatedTime);
                default: return hb;
            }
        }

        public static string EditorNew()
        {
            return Editor(
                new OutgoingMailModel(
                    methodType: BaseModel.MethodTypes.New),
                byRest: false);
        }

        public static string Editor(long outgoingMailId, bool clearSessions)
        {
            var outgoingMailModel = new OutgoingMailModel(
                outgoingMailId: outgoingMailId,
                clearSessions: clearSessions,
                methodType: BaseModel.MethodTypes.Edit);
            outgoingMailModel.SwitchTargets = OutgoingMailUtilities.GetSwitchTargets(
                SiteSettingsUtility.OutgoingMailsSiteSettings());
            return Editor(outgoingMailModel, byRest: false);
        }

        public static string Editor(OutgoingMailModel outgoingMailModel, bool byRest)
        {
            var hb = new HtmlBuilder();
            var permissionType = Permissions.Admins();
            outgoingMailModel.SiteSettings.SetChoicesByPlaceholders();
            return hb.Template(
                siteId: 0,
                referenceType: "OutgoingMails",
                title: outgoingMailModel.MethodType == BaseModel.MethodTypes.New
                    ? Displays.OutgoingMails() + " - " + Displays.New()
                    : outgoingMailModel.Title.Value,
                permissionType: permissionType,
                verType: outgoingMailModel.VerType,
                methodType: outgoingMailModel.MethodType,
                allowAccess:
                    permissionType.CanEditTenant() &&
                    outgoingMailModel.AccessStatus != Databases.AccessStatuses.NotFound,
                byRest: byRest,
                action: () =>
                {
                    permissionType = Permissions.Types.Manager;
                    hb
                        .Editor(
                            outgoingMailModel: outgoingMailModel,
                            permissionType: permissionType,
                            siteSettings: outgoingMailModel.SiteSettings)
                        .Hidden(controlId: "TableName", value: "OutgoingMails")
                        .Hidden(controlId: "Id", value: outgoingMailModel.OutgoingMailId.ToString());
                }).ToString();
        }

        private static HtmlBuilder Editor(
            this HtmlBuilder hb,
            OutgoingMailModel outgoingMailModel,
            Permissions.Types permissionType,
            SiteSettings siteSettings)
        {
            return hb.Div(css: "edit-form", action: () => hb
                .Form(
                    attributes: new HtmlAttributes()
                        .Id_Css("OutgoingMailForm", "main-form")
                        .Action(outgoingMailModel.OutgoingMailId != 0
                            ? Navigations.Action("OutgoingMails", outgoingMailModel.OutgoingMailId)
                            : Navigations.Action("OutgoingMails")),
                    action: () => hb
                        .RecordHeader(
                            id: outgoingMailModel.OutgoingMailId,
                            baseModel: outgoingMailModel,
                            tableName: "OutgoingMails",
                            switchTargets: outgoingMailModel.SwitchTargets?
                                .Select(o => o.ToLong()).ToList())
                        .Div(css: "edit-form-comments", action: () => hb
                            .Comments(
                                comments: outgoingMailModel.Comments,
                                verType: outgoingMailModel.VerType))
                        .Div(css: "edit-form-tabs", action: () => hb
                            .FieldTabs(outgoingMailModel: outgoingMailModel)
                            .FieldSetGeneral(
                                siteSettings: siteSettings,
                                permissionType: permissionType,
                                outgoingMailModel: outgoingMailModel)
                            .FieldSet(
                                attributes: new HtmlAttributes()
                                    .Id("FieldSetHistories")
                                    .DataAction("Histories")
                                    .DataMethod("get"),
                                _using: outgoingMailModel.MethodType != BaseModel.MethodTypes.New)
                            .MainCommands(
                                siteId: 0,
                                permissionType: permissionType,
                                verType: outgoingMailModel.VerType,
                                backUrl: Navigations.Index("OutgoingMails"),
                                referenceType: "OutgoingMails",
                                referenceId: outgoingMailModel.OutgoingMailId,
                                updateButton: true,
                                mailButton: true,
                                deleteButton: true,
                                extensions: () => hb
                                    .MainCommandExtensions(
                                        outgoingMailModel: outgoingMailModel,
                                        siteSettings: siteSettings)))
                        .Hidden(
                            controlId: "MethodType",
                            value: outgoingMailModel.MethodType.ToString().ToLower())
                        .Hidden(
                            controlId: "OutgoingMails_Timestamp",
                            css: "must-transport",
                            value: outgoingMailModel.Timestamp)
                        .Hidden(
                            controlId: "SwitchTargets",
                            css: "must-transport",
                            value: outgoingMailModel.SwitchTargets?.Join()))
                .OutgoingMailsForm("OutgoingMails", outgoingMailModel.OutgoingMailId, outgoingMailModel.Ver)
                .CopyDialog("OutgoingMails", outgoingMailModel.OutgoingMailId)
                .OutgoingMailDialog()
                .EditorExtensions(outgoingMailModel: outgoingMailModel, siteSettings: siteSettings));
        }

        private static HtmlBuilder FieldTabs(this HtmlBuilder hb, OutgoingMailModel outgoingMailModel)
        {
            return hb.Ul(css: "field-tab", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetGeneral", 
                        text: Displays.Basic()))
                .Li(
                    _using: outgoingMailModel.MethodType != BaseModel.MethodTypes.New,
                    action: () => hb
                        .A(
                            href: "#FieldSetHistories",
                            text: Displays.Histories())));
        }

        private static HtmlBuilder FieldSetGeneral(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            OutgoingMailModel outgoingMailModel)
        {
            return hb.FieldSet(id: "FieldSetGeneral", action: () =>
            {
                siteSettings.EditorColumnCollection().ForEach(column =>
                {
                    switch (column.ColumnName)
                    {
                        case "ReferenceType": hb.Field(siteSettings, column, outgoingMailModel.MethodType, outgoingMailModel.ReferenceType.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "ReferenceId": hb.Field(siteSettings, column, outgoingMailModel.MethodType, outgoingMailModel.ReferenceId.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "OutgoingMailId": hb.Field(siteSettings, column, outgoingMailModel.MethodType, outgoingMailModel.OutgoingMailId.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "Ver": hb.Field(siteSettings, column, outgoingMailModel.MethodType, outgoingMailModel.Ver.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "To": hb.Field(siteSettings, column, outgoingMailModel.MethodType, outgoingMailModel.To.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "Cc": hb.Field(siteSettings, column, outgoingMailModel.MethodType, outgoingMailModel.Cc.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "Bcc": hb.Field(siteSettings, column, outgoingMailModel.MethodType, outgoingMailModel.Bcc.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "Title": hb.Field(siteSettings, column, outgoingMailModel.MethodType, outgoingMailModel.Title.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "Body": hb.Field(siteSettings, column, outgoingMailModel.MethodType, outgoingMailModel.Body.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "SentTime": hb.Field(siteSettings, column, outgoingMailModel.MethodType, outgoingMailModel.SentTime.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DestinationSearchRange": hb.Field(siteSettings, column, outgoingMailModel.MethodType, outgoingMailModel.DestinationSearchRange.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DestinationSearchText": hb.Field(siteSettings, column, outgoingMailModel.MethodType, outgoingMailModel.DestinationSearchText.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                    }
                });
                hb.VerUpCheckBox(outgoingMailModel);
            });
        }

        private static HtmlBuilder MainCommandExtensions(
            this HtmlBuilder hb,
            OutgoingMailModel outgoingMailModel,
            SiteSettings siteSettings)
        {
            return hb;
        }

        private static HtmlBuilder EditorExtensions(
            this HtmlBuilder hb,
            OutgoingMailModel outgoingMailModel,
            SiteSettings siteSettings)
        {
            return hb;
        }

        public static List<long> GetSwitchTargets(SiteSettings siteSettings)
        {
            var switchTargets = Forms.Data("SwitchTargets").Split(',')
                .Select(o => o.ToLong())
                .Where(o => o != 0)
                .ToList();
            if (switchTargets.Count() == 0)
            {
                var formData = DataViewFilters.SessionFormData();
                switchTargets = Rds.ExecuteTable(
                    transactional: false,
                    statements: Rds.SelectOutgoingMails(
                        column: Rds.OutgoingMailsColumn().OutgoingMailId(),
                        where: DataViewFilters.Get(
                            siteSettings: siteSettings,
                            tableName: "OutgoingMails",
                            formData: formData,
                            where: Rds.OutgoingMailsWhere()),
                        orderBy: GridSorters.Get(
                            formData, Rds.OutgoingMailsOrderBy().UpdatedTime(SqlOrderBy.Types.desc))))
                                .AsEnumerable()
                                .Select(o => o["OutgoingMailId"].ToLong())
                                .ToList();    
            }
            return switchTargets;
        }

        public static ResponseCollection FormResponse(
            this ResponseCollection responseCollection, OutgoingMailModel outgoingMailModel)
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

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder OutgoingMailsForm(
            this HtmlBuilder hb, string referenceType, long referenceId, int referenceVer)
        {
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id_Css("OutgoingMailsForm", "edit-form-mail-list")
                    .Action(Navigations.ItemAction(referenceId, "OutgoingMails")),
                action: () =>
                    new OutgoingMailCollection(
                        where: Rds.OutgoingMailsWhere()
                            .ReferenceType(referenceType)
                            .ReferenceId(referenceId)
                            .ReferenceVer(referenceVer, _operator: "<="),
                        orderBy: Rds.OutgoingMailsOrderBy()
                            .OutgoingMailId(SqlOrderBy.Types.desc))
                                .ForEach(outgoingMailModel => hb
                                    .OutgoingMailListItem(outgoingMailModel)));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder OutgoingMailListItem(
            this HtmlBuilder hb, OutgoingMailModel outgoingMailModel, string selector = "")
        {
            return hb.Div(
                id: selector,
                css: "mail-list",
                action: () => hb
                    .H(number: 3, css: "title-header", action: () => hb
                        .Displays_SentMail())
                    .Div(css: "mail-content", action: () => hb
                        .FieldText(
                            controlId: string.Empty,
                            labelText: Displays.OutgoingMails_SentTime(),
                            text: outgoingMailModel.SentTime.ToString(),
                            fieldCss: "field-auto")
                        .FieldText(
                            controlId: string.Empty,
                            labelText: Displays.OutgoingMails_From(),
                            text: outgoingMailModel.From.ToString(),
                            fieldCss: "field-auto-thin")
                        .OutgoingMailListItemDestination(
                            outgoingMailModel.To, Displays.OutgoingMails_To())
                        .OutgoingMailListItemDestination(
                            outgoingMailModel.Cc, Displays.OutgoingMails_Cc())
                        .OutgoingMailListItemDestination(
                            outgoingMailModel.Bcc, Displays.OutgoingMails_Bcc())
                        .FieldText(
                            controlId: string.Empty,
                            labelText: Displays.OutgoingMails_Title(),
                            text: outgoingMailModel.Title.Value,
                            fieldCss: "field-wide")
                        .FieldMarkUp(
                            controlId: string.Empty,
                            labelText: Displays.OutgoingMails_Body(),
                            text: outgoingMailModel.Body,
                            fieldCss: "field-wide")
                        .Div(css: "command-right", action: () => hb
                            .Button(
                                text: Displays.Reply(),
                                controlCss: "button-send-mail",
                                onClick: "$p.openOutgoingMailReplyDialog($(this));",
                                dataId: outgoingMailModel.OutgoingMailId.ToString(),
                                action: "Reply",
                                method: "put"))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder OutgoingMailListItemDestination(
            this HtmlBuilder hb, string destinations, string labelText)
        {
            if (destinations != string.Empty)
            {
                return hb.FieldText(
                    controlId: string.Empty,
                    labelText: labelText,
                    text: destinations,
                    fieldCss: "field-wide");
            }
            else
            {
                return hb;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder OutgoingMailDialog(this HtmlBuilder hb)
        {
            return hb.Div(attributes: new HtmlAttributes()
                .Id_Css("OutgoingMailDialog", "dialog"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Editor(string referenceType, long referenceId)
        {
            if (FromMailAddress() == string.Empty)
            {
                return new ResponseCollection()
                    .CloseDialog()
                    .Message(Messages.MailAddressHasNotSet())
                    .ToJson();
            }
            var siteModel = new ItemModel(referenceId).GetSite();
            var siteSettings = siteModel.SitesSiteSettings();
            var outgoingMailModel = new OutgoingMailModel().Get(
                where: Rds.OutgoingMailsWhere().OutgoingMailId(
                    Forms.Long("OutgoingMails_OutgoingMailId")));
            var hb = new HtmlBuilder();
            return new ResponseCollection()
                .Html("#OutgoingMailDialog", hb
                    .Div(css: "edit-form-tabs-max no-border", action: () => hb
                        .Ul(css: "field-tab", action: () => hb
                            .Li(action: () => hb
                                .A(
                                    href: "#FieldSetMailEditor",
                                    text: Displays.Mail()))
                            .Li(action: () => hb
                                .A(
                                    href: "#FieldSetAddressBook",
                                    text: Displays.AddressBook())))
                        .FieldSet(id: "FieldSetMailEditor", action: () => hb
                            .Form(
                                attributes: new HtmlAttributes()
                                    .Id("OutgoingMailForm")
                                    .Action(Navigations.Action(
                                        referenceType, referenceId, "OutgoingMails")),
                                action: () => hb
                                    .Editor(
                                        siteSettings: siteSettings,
                                        outgoingMailModel: outgoingMailModel)))
                        .FieldSet(id: "FieldSetAddressBook", action: () => hb
                            .Form(
                                attributes: new HtmlAttributes()
                                    .Id("OutgoingMailDestinationForm")
                                    .Action(Navigations.Action(
                                        referenceType, referenceId, "OutgoingMails")),
                                action: () => hb
                                    .Destinations(
                                        siteSettings: siteSettings,
                                        referenceId: siteModel.InheritPermission)))))
                .Invoke("initOutgoingMailDialog")
                .Invoke("validateOutgoingMails")
                .Focus("#OutgoingMails_Body")
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Editor(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            OutgoingMailModel outgoingMailModel)
        {
            outgoingMailModel.SiteSettings = SiteSettingsUtility.OutgoingMailsSiteSettings();
            return hb
                .FieldBasket(
                    controlId: "OutgoingMails_To",
                    fieldCss: "field-wide",
                    controlCss: "control-basket cf",
                    labelAction: () => hb
                        .Displays_OutgoingMails_To())
                .FieldBasket(
                    controlId: "OutgoingMails_Cc",
                    fieldCss: "field-wide",
                    controlCss: "control-basket cf",
                    labelAction: () => hb
                        .Displays_OutgoingMails_Cc())
                .FieldBasket(
                    controlId: "OutgoingMails_Bcc",
                    fieldCss: "field-wide",
                    controlCss: "control-basket cf",
                    labelAction: () => hb
                        .Displays_OutgoingMails_Bcc())
                .FieldTextBox(
                    controlId: "OutgoingMails_Title",
                    fieldCss: "field-wide",
                    controlCss: " must-transport",
                    labelText: Displays.OutgoingMails_Title(),
                    text: ReplyTitle(outgoingMailModel),
                    attributes: outgoingMailModel.SiteSettings.GetColumn("Title")
                        .ValidationMessages())
                .FieldTextBox(
                    textType: HtmlTypes.TextTypes.MultiLine,
                    controlId: "OutgoingMails_Body",
                    fieldCss: "field-wide",
                    controlCss: " must-transport h300",
                    labelText: Displays.OutgoingMails_Body(),
                    text: ReplyBody(outgoingMailModel),
                    attributes: outgoingMailModel.SiteSettings.GetColumn("Body")
                        .ValidationMessages())
                .P(css: "message-dialog")
                .Div(css: "command-center", action: () => hb
                    .Button(
                        controlId: "OutgoingMails_Send",
                        controlCss: "button-send-mail validate",
                        text: Displays.SendMail(),
                        onClick: "$p.sendMail($(this));",
                        action: "Send",
                        method: "post",
                        confirm: "Displays_ConfirmSendMail")
                    .Button(
                        controlId: "OutgoingMails_Cancel",
                        controlCss: "button-cancel",
                        text: Displays.Cancel(),
                        onClick: "$p.closeDialog($(this));"))
            .Hidden(controlId: "OutgoingMails_Location", value: Location())
            .Hidden(
                controlId: "OutgoingMails_Reply",
                value: outgoingMailModel.AccessStatus == Databases.AccessStatuses.Selected
                    ? "1"
                    : "0")
            .Hidden(
                controlId: "MailToDefault",
                value: MailDefault(outgoingMailModel, siteSettings.MailToDefault, "to"))
            .Hidden(
                controlId: "MailCcDefault",
                value: MailDefault(outgoingMailModel, siteSettings.MailCcDefault, "cc"))
            .Hidden(
                controlId: "MailBccDefault",
                value: MailDefault(outgoingMailModel, siteSettings.MailBccDefault, "bcc"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string ReplyTitle(OutgoingMailModel outgoingMailModel)
        {
            var title = outgoingMailModel.Title.Value;
            return outgoingMailModel.AccessStatus == Databases.AccessStatuses.Selected
                ? title.StartsWith("Re: ") ? title : "Re: " + title
                : string.Empty;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string ReplyBody(OutgoingMailModel outgoingMailModel)
        {
            return outgoingMailModel.AccessStatus == Databases.AccessStatuses.Selected
                ? Displays.OriginalMessage().Params(
                    Location(),
                    outgoingMailModel.From,
                    outgoingMailModel.SentTime.DisplayValue.ToString(
                        Displays.Get("YmdahmsFormat"), Sessions.CultureInfo()),
                    outgoingMailModel.Title.Value,
                    outgoingMailModel.Body)
                : string.Empty;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string MailDefault(
            OutgoingMailModel outgoingMailModel, string mailDefault, string type)
        {
            var myAddress = new MailAddressModel(Sessions.UserId()).MailAddress;
            if (outgoingMailModel.AccessStatus == Databases.AccessStatuses.Selected)
            {
                switch (type)
                {
                    case "to":
                        var to = outgoingMailModel.To
                            .Split(';')
                            .Where(o => Libraries.Mails.Addresses.Get(o) != myAddress)
                            .Where(o => o.Trim() != string.Empty)
                            .Join(";");
                        return to.Trim() != string.Empty
                            ? outgoingMailModel.From.ToString() + ";" + to
                            : outgoingMailModel.From.ToString();
                    case "cc":
                        return outgoingMailModel.Cc;
                    case "bcc":
                        return outgoingMailModel.Bcc;
                }
            }
            return mailDefault;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string Location()
        {
            var location = Url.AbsoluteUri().ToLower();
            return location.Substring(0, location.IndexOf("/outgoingmails"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Destinations(
            this HtmlBuilder hb, SiteSettings siteSettings, long referenceId)
        {
            var addressBook = AddressBook(siteSettings);
            var searchRangeDefault = SiteInfo.IsItem()
                ? addressBook.Count > 0
                    ? "DefaultAddressBook"
                    : "SiteUser"
                : "All";
            return hb
                .Div(css: "container-left", action: () => hb
                    .FieldDropDown(
                        controlId: "OutgoingMails_DestinationSearchRange",
                        labelText: Displays.OutgoingMails_DestinationSearchRange(),
                        optionCollection: SearchRangeOptionCollection(
                            searchRangeDefault: searchRangeDefault,
                            addressBook: addressBook),
                        controlCss: " auto-postback must-transport",
                        action: "GetDestinations",
                        method: "put")
                    .FieldTextBox(
                        controlId: "OutgoingMails_DestinationSearchText",
                        labelText: Displays.OutgoingMails_DestinationSearchText(),
                        controlCss: " auto-postback",
                        action: "GetDestinations",
                        method: "put"))
                .Div(css: "container-right", action: () => hb
                    .Div(action: () => hb
                        .FieldSelectable(
                            controlId: "OutgoingMails_MailAddresses",
                            fieldCss: "field-vertical both",
                            controlContainerCss: "container-selectable",
                            controlCss: " h500",
                            listItemCollection: Destinations(
                                referenceId: referenceId,
                                addressBook: addressBook,
                                searchRange: searchRangeDefault),
                            selectedValueCollection: new List<string>())
                        .Div(css: "command-left", action: () => hb
                            .Button(
                                controlId: "OutgoingMails_AddTo",
                                text: Displays.OutgoingMails_To(),
                                controlCss: "button-person")
                            .Button(
                                controlId: "OutgoingMails_AddCc",
                                text: Displays.OutgoingMails_Cc(),
                                controlCss: "button-person")
                            .Button(
                                controlId: "OutgoingMails_AddBcc",
                                text: Displays.OutgoingMails_Bcc(),
                                controlCss: "button-person"))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Dictionary<string, string> AddressBook(SiteSettings siteSettings)
        {
            return siteSettings.AddressBook
                .SplitReturn()
                .Select(o => o.Trim())
                .Where(o => o != string.Empty)
                .ToDictionary(o => o, o => o);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static Dictionary<string, ControlData> SearchRangeOptionCollection(
            string searchRangeDefault, Dictionary<string, string> addressBook)
        {
            switch (searchRangeDefault)
            {
                case "DefaultAddressBook":
                    return new Dictionary<string, ControlData>
                    {
                        { "DefaultAddressBook", new ControlData(Displays.DefaultAddressBook()) },
                        { "SiteUser", new ControlData(Displays.SiteUser()) },
                        { "All", new ControlData(Displays.All()) }
                    };
                case "SiteUser":
                    return new Dictionary<string, ControlData>
                    {
                        { "SiteUser", new ControlData(Displays.SiteUser()) },
                        { "All", new ControlData(Displays.All()) }
                    };
                default:
                    return new Dictionary<string, ControlData>
                    {
                        { "All", new ControlData(Displays.All()) }
                    };
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Dictionary<string, string> Destinations(
            long referenceId,
            Dictionary<string, string> addressBook,
            string searchRange,
            string searchText = "")
        {
            var joinMailAddresses = new SqlJoin(
                "inner join [MailAddresses] on [MailAddresses].[OwnerId]=[t0].[UserId]");
            switch (searchRange)
            {
                case "DefaultAddressBook":
                    return searchText == string.Empty
                        ? addressBook
                        : addressBook
                            .Where(o => o.Value.IndexOf(searchText,
                                System.Globalization.CompareOptions.IgnoreCase |
                                System.Globalization.CompareOptions.IgnoreKanaType |
                                System.Globalization.CompareOptions.IgnoreWidth) != -1)
                            .ToDictionary(o => o.Key, o => o.Value);
                case "SiteUser":
                    var joinPermissions = new SqlJoin(
                        "inner join [Permissions] on " +
                        "([t0].[UserId]=[Permissions].[UserId] and [Permissions].[UserId] <> 0) or " +
                        "([t0].[DeptId]=[Permissions].[DeptId] and [Permissions].[DeptId] <> 0)");
                    return DestinationCollection(
                        Sqls.SqlJoinCollection(joinMailAddresses, joinPermissions),
                        Rds.UsersWhere()
                            .MailAddresses_OwnerType("Users")
                            .Permissions_ReferenceType("Sites")
                            .Permissions_ReferenceId(referenceId)
                            .SearchText(searchText)
                            .Users_TenantId(Sessions.TenantId(), "t0"));
                case "All":
                default:
                    return !searchText.IsNullOrEmpty()
                        ? DestinationCollection(
                            Sqls.SqlJoinCollection(joinMailAddresses),
                            Rds.UsersWhere()
                                .MailAddresses_OwnerType("Users")
                                .SearchText(searchText)
                                .Users_TenantId(Sessions.TenantId(), "t0"))
                        : new Dictionary<string, string>();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static SqlWhereCollection SearchText(
            this SqlWhereCollection self, string searchText)
        {
            return self.SqlWhereLike(searchText,
                Rds.Users_UserId_WhereLike(),
                Rds.Users_LoginId_WhereLike(),
                Rds.Users_FirstName_WhereLike(),
                Rds.Users_LastName_WhereLike(),
                Rds.MailAddresses_MailAddress_WhereLike("MailAddresses"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static Dictionary<string, string> DestinationCollection(
            SqlJoinCollection join, SqlWhereCollection where)
        {
            return Rds.ExecuteTable(
                transactional: false,
                statements: Rds.SelectUsers(
                    column: Rds.UsersColumn()
                        .FirstName()
                        .LastName()
                        .FirstAndLastNameOrder()
                        .MailAddresses_MailAddress(),
                    join: join,
                    where: where,
                    distinct: true)).AsEnumerable()
                        .Select((o, i) => new {
                            Name = Names.FullName(
                            (Names.FirstAndLastNameOrders)o["FirstAndLastNameOrder"],
                            "\"" + o["FirstName"].ToString() + " " + o["LastName"].ToString() +
                                "\" <" + o["MailAddress"].ToString() + ">",
                            "\"" + o["LastName"].ToString() + " " + o["FirstName"].ToString() +
                                "\" <" + o["MailAddress"].ToString() + ">"),
                            Index = i
                        })
                        .ToDictionary(
                            o => o.Index.ToString(),
                            o => o.Name);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string FromMailAddress()
        {
            return Rds.ExecuteScalar_string(statements:
                Rds.SelectMailAddresses(
                    top: 1,
                    column: Rds.MailAddressesColumn().MailAddress(),
                    where: Rds.MailAddressesWhere()
                        .OwnerId(Sessions.UserId())
                        .OwnerType("Users"),
                    orderBy: Rds.MailAddressesOrderBy().MailAddressId()));
        }
    }
}
