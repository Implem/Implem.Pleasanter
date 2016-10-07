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
    public static class MailAddressUtilities
    {
        private static string DataViewTemplate(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            MailAddressCollection mailAddressCollection,
            FormData formData,
            string dataViewName,
            Action dataViewBody)
        {
            return hb.Template(
                permissionType: permissionType,
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.Index,
                allowAccess: permissionType.CanRead(),
                siteId: siteSettings.SiteId,
                parentId: siteSettings.ParentId,
                referenceType: "MailAddresses",
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
                            .Id("MailAddressesForm")
                            .Class("main-form")
                            .Action(Navigations.ItemAction(siteSettings.SiteId)),
                        action: () => hb
                            .DataViewFilters(siteSettings: siteSettings)
                            .Aggregations(
                                siteSettings: siteSettings,
                                aggregations: mailAddressCollection.Aggregations)
                            .Div(id: "DataViewContainer", action: () => dataViewBody())
                            .MainCommands(
                                siteId: siteSettings.SiteId,
                                permissionType: permissionType,
                                verType: Versions.VerTypes.Latest,
                                bulkMoveButton: true,
                                bulkDeleteButton: true,
                                importButton: true,
                                exportButton: true)
                            .Div(css: "margin-bottom")
                            .Hidden(controlId: "TableName", value: "MailAddresses")
                            .Hidden(controlId: "BaseUrl", value: Navigations.BaseUrl()))
                .MoveDialog(bulk: true)
                .Div(attributes: new HtmlAttributes()
                    .Id("ExportSettingsDialog")
                    .Class("dialog")
                    .Title(Displays.ExportSettings())))
                .ToString();
        }

        public static string Index(SiteSettings siteSettings, Permissions.Types permissionType)
        {
            var hb = new HtmlBuilder();
            var formData = DataViewFilters.SessionFormData();
            var mailAddressCollection = MailAddressCollection(siteSettings, permissionType, formData);
            var dataViewName = DataViewSelectors.Get(siteSettings.SiteId);
            return hb.DataViewTemplate(
                siteSettings: siteSettings,
                permissionType: permissionType,
                mailAddressCollection: mailAddressCollection,
                formData: formData,
                dataViewName: dataViewName,
                dataViewBody: () => hb.Grid(
                   mailAddressCollection: mailAddressCollection,
                   siteSettings: siteSettings,
                   permissionType: permissionType,
                   formData: formData));
        }

        public static string IndexJson(SiteSettings siteSettings, Permissions.Types permissionType)
        {
            var formData = DataViewFilters.SessionFormData();
            var mailAddressCollection = MailAddressCollection(siteSettings, permissionType, formData);
            return new ResponseCollection()
                .Html("#DataViewContainer", new HtmlBuilder().Grid(
                    siteSettings: siteSettings,
                    mailAddressCollection: mailAddressCollection,
                    permissionType: permissionType,
                    formData: formData))
                .DataViewFilters(siteSettings: siteSettings)
                .ReplaceAll("#Aggregations", new HtmlBuilder().Aggregations(
                    siteSettings: siteSettings,
                    aggregations: mailAddressCollection.Aggregations))
                .WindowScrollTop().ToJson();
        }

        private static MailAddressCollection MailAddressCollection(
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData,
            int offset = 0)
        {
            return new MailAddressCollection(
                siteSettings: siteSettings,
                permissionType: permissionType,
                column: GridSqlColumnCollection(siteSettings),
                where: DataViewFilters.Get(
                    siteSettings: siteSettings,
                    tableName: "MailAddresses",
                    formData: formData,
                    where: Rds.MailAddressesWhere()),
                orderBy: GridSorters.Get(
                    formData, Rds.MailAddressesOrderBy().UpdatedTime(SqlOrderBy.Types.desc)),
                offset: offset,
                pageSize: siteSettings.GridPageSize.ToInt(),
                countRecord: true,
                aggregationCollection: siteSettings.AggregationCollection);
        }

        private static HtmlBuilder Grid(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            MailAddressCollection mailAddressCollection,
            FormData formData)
        {
            return hb
                .Table(
                    attributes: new HtmlAttributes()
                        .Id("Grid")
                        .Class("grid")
                        .DataAction("GridRows")
                        .DataMethod("post"),
                    action: () => hb
                        .GridRows(
                            siteSettings: siteSettings,
                            mailAddressCollection: mailAddressCollection,
                            formData: formData))
                .Hidden(
                    controlId: "GridOffset",
                    value: siteSettings.GridPageSize == mailAddressCollection.Count()
                        ? siteSettings.GridPageSize.ToString()
                        : "-1");
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
            var mailAddressCollection = MailAddressCollection(siteSettings, permissionType, formData, offset);
            return (responseCollection ?? new ResponseCollection())
                .Remove(".grid tr", _using: offset == 0)
                .ClearFormData("GridCheckAll", _using: clearCheck)
                .ClearFormData("GridUnCheckedItems", _using: clearCheck)
                .ClearFormData("GridCheckedItems", _using: clearCheck)
                .Message(message)
                .Append("#Grid", new HtmlBuilder().GridRows(
                    siteSettings: siteSettings,
                    mailAddressCollection: mailAddressCollection,
                    formData: formData,
                    addHeader: offset == 0,
                    clearCheck: clearCheck))
                .Val("#GridOffset", siteSettings.NextPageOffset(offset, mailAddressCollection.Count()))
                .ToJson();
        }

        private static HtmlBuilder GridRows(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            MailAddressCollection mailAddressCollection,
            FormData formData,
            bool addHeader = true,
            bool clearCheck = false)
        {
            var checkAll = clearCheck ? false : Forms.Bool("GridCheckAll");
            return hb
                .THead(
                    _using: addHeader,
                    action: () => hb
                        .GridHeader(
                            columnCollection: siteSettings.GridColumnCollection(), 
                            formData: formData,
                            checkAll: checkAll))
                .TBody(action: () => mailAddressCollection
                    .ForEach(mailAddressModel => hb
                        .Tr(
                            attributes: new HtmlAttributes()
                                .Class("grid-row")
                                .DataId(mailAddressModel.MailAddressId.ToString()),
                            action: () =>
                            {
                                hb.Td(action: () => hb
                                    .CheckBox(
                                        controlCss: "grid-check",
                                        _checked: checkAll,
                                        dataId: mailAddressModel.MailAddressId.ToString()));
                                siteSettings.GridColumnCollection()
                                    .ForEach(column => hb
                                        .TdValue(
                                            column: column,
                                            mailAddressModel: mailAddressModel));
                            })));
        }

        private static SqlColumnCollection GridSqlColumnCollection(SiteSettings siteSettings)
        {
            var gridSqlColumn = Rds.MailAddressesColumn()
                .MailAddressId()
                .Creator()
                .Updator();
            siteSettings.GridColumnCollection(withTitle: true).ForEach(column =>
                Rds.MailAddressesColumn(gridSqlColumn, column.ColumnName));
            return gridSqlColumn;
        }

        public static HtmlBuilder TdValue(
            this HtmlBuilder hb, Column column, MailAddressModel mailAddressModel)
        {
            switch (column.ColumnName)
            {
                case "Ver": return hb.Td(column: column, value: mailAddressModel.Ver);
                case "Comments": return hb.Td(column: column, value: mailAddressModel.Comments);
                case "Creator": return hb.Td(column: column, value: mailAddressModel.Creator);
                case "Updator": return hb.Td(column: column, value: mailAddressModel.Updator);
                case "CreatedTime": return hb.Td(column: column, value: mailAddressModel.CreatedTime);
                case "UpdatedTime": return hb.Td(column: column, value: mailAddressModel.UpdatedTime);
                default: return hb;
            }
        }

        public static string EditorNew()
        {
            return Editor(
                new MailAddressModel(
                    SiteSettingsUtility.MailAddressesSiteSettings(),
                    Permissions.Admins(),
                    methodType: BaseModel.MethodTypes.New));
        }

        public static string Editor(long mailAddressId, bool clearSessions)
        {
            var mailAddressModel = new MailAddressModel(
                    SiteSettingsUtility.MailAddressesSiteSettings(),
                    Permissions.Admins(),
                mailAddressId: mailAddressId,
                clearSessions: clearSessions,
                methodType: BaseModel.MethodTypes.Edit);
            mailAddressModel.SwitchTargets = GetSwitchTargets(
                SiteSettingsUtility.MailAddressesSiteSettings());
            return Editor(mailAddressModel);
        }

        public static string Editor(MailAddressModel mailAddressModel)
        {
            var hb = new HtmlBuilder();
            var permissionType = Permissions.Admins();
            return hb.Template(
                permissionType: permissionType,
                verType: mailAddressModel.VerType,
                methodType: mailAddressModel.MethodType,
                allowAccess:
                    permissionType.CanEditTenant() &&
                    mailAddressModel.AccessStatus != Databases.AccessStatuses.NotFound,
                referenceType: "MailAddresses",
                title: mailAddressModel.MethodType == BaseModel.MethodTypes.New
                    ? Displays.MailAddresses() + " - " + Displays.New()
                    : mailAddressModel.Title.Value,
                action: () =>
                {
                    hb
                        .Editor(
                            mailAddressModel: mailAddressModel,
                            permissionType: permissionType,
                            siteSettings: mailAddressModel.SiteSettings)
                        .Hidden(controlId: "TableName", value: "MailAddresses")
                        .Hidden(controlId: "Id", value: mailAddressModel.MailAddressId.ToString());
                }).ToString();
        }

        private static HtmlBuilder Editor(
            this HtmlBuilder hb,
            MailAddressModel mailAddressModel,
            Permissions.Types permissionType,
            SiteSettings siteSettings)
        {
            return hb.Div(id: "Editor", action: () => hb
                .Form(
                    attributes: new HtmlAttributes()
                        .Id("MailAddressForm")
                        .Class("main-form")
                        .Action(mailAddressModel.MailAddressId != 0
                            ? Navigations.Action("MailAddresses", mailAddressModel.MailAddressId)
                            : Navigations.Action("MailAddresses")),
                    action: () => hb
                        .RecordHeader(
                            baseModel: mailAddressModel,
                            tableName: "MailAddresses")
                        .Div(id: "EditorComments", action: () => hb
                            .Comments(
                                comments: mailAddressModel.Comments,
                                verType: mailAddressModel.VerType))
                        .Div(id: "EditorTabsContainer", action: () => hb
                            .EditorTabs(mailAddressModel: mailAddressModel)
                            .FieldSetGeneral(
                                siteSettings: siteSettings,
                                permissionType: permissionType,
                                mailAddressModel: mailAddressModel)
                            .FieldSet(
                                attributes: new HtmlAttributes()
                                    .Id("FieldSetHistories")
                                    .DataAction("Histories")
                                    .DataMethod("get"),
                                _using: mailAddressModel.MethodType != BaseModel.MethodTypes.New)
                            .MainCommands(
                                siteId: 0,
                                permissionType: permissionType,
                                verType: mailAddressModel.VerType,
                                referenceType: "MailAddresses",
                                referenceId: mailAddressModel.MailAddressId,
                                updateButton: true,
                                mailButton: true,
                                deleteButton: true,
                                extensions: () => hb
                                    .MainCommandExtensions(
                                        mailAddressModel: mailAddressModel,
                                        siteSettings: siteSettings)))
                        .Hidden(controlId: "BaseUrl", value: Navigations.BaseUrl())
                        .Hidden(
                            controlId: "MethodType",
                            value: mailAddressModel.MethodType.ToString().ToLower())
                        .Hidden(
                            controlId: "MailAddresses_Timestamp",
                            css: "must-transport",
                            value: mailAddressModel.Timestamp)
                        .Hidden(
                            controlId: "SwitchTargets",
                            css: "must-transport",
                            value: mailAddressModel.SwitchTargets?.Join(),
                            _using: !Request.IsAjax()))
                .OutgoingMailsForm("MailAddresses", mailAddressModel.MailAddressId, mailAddressModel.Ver)
                .CopyDialog("MailAddresses", mailAddressModel.MailAddressId)
                .OutgoingMailDialog()
                .EditorExtensions(mailAddressModel: mailAddressModel, siteSettings: siteSettings));
        }

        private static HtmlBuilder EditorTabs(this HtmlBuilder hb, MailAddressModel mailAddressModel)
        {
            return hb.Ul(id: "EditorTabs", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetGeneral", 
                        text: Displays.Basic()))
                .Li(
                    _using: mailAddressModel.MethodType != BaseModel.MethodTypes.New,
                    action: () => hb
                        .A(
                            href: "#FieldSetHistories",
                            text: Displays.Histories())));
        }

        private static HtmlBuilder FieldSetGeneral(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            MailAddressModel mailAddressModel)
        {
            return hb.FieldSet(id: "FieldSetGeneral", action: () =>
            {
                siteSettings.EditorColumnCollection().ForEach(column =>
                {
                    switch (column.ColumnName)
                    {
                        case "OwnerId": hb.Field(siteSettings, column, mailAddressModel.MethodType, mailAddressModel.OwnerId.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "OwnerType": hb.Field(siteSettings, column, mailAddressModel.MethodType, mailAddressModel.OwnerType.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "MailAddressId": hb.Field(siteSettings, column, mailAddressModel.MethodType, mailAddressModel.MailAddressId.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "Ver": hb.Field(siteSettings, column, mailAddressModel.MethodType, mailAddressModel.Ver.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "MailAddress": hb.Field(siteSettings, column, mailAddressModel.MethodType, mailAddressModel.MailAddress.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "Title": hb.Field(siteSettings, column, mailAddressModel.MethodType, mailAddressModel.Title.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                    }
                });
                hb.VerUpCheckBox(mailAddressModel);
            });
        }

        private static HtmlBuilder MainCommandExtensions(
            this HtmlBuilder hb,
            MailAddressModel mailAddressModel,
            SiteSettings siteSettings)
        {
            return hb;
        }

        private static HtmlBuilder EditorExtensions(
            this HtmlBuilder hb,
            MailAddressModel mailAddressModel,
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
                    statements: Rds.SelectMailAddresses(
                        column: Rds.MailAddressesColumn().MailAddressId(),
                        where: DataViewFilters.Get(
                            siteSettings: siteSettings,
                            tableName: "MailAddresses",
                            formData: formData,
                            where: Rds.MailAddressesWhere()),
                        orderBy: GridSorters.Get(
                            formData, Rds.MailAddressesOrderBy().UpdatedTime(SqlOrderBy.Types.desc))))
                                .AsEnumerable()
                                .Select(o => o["MailAddressId"].ToLong())
                                .ToList();    
            }
            return switchTargets;
        }

        public static ResponseCollection FormResponse(
            this ResponseCollection responseCollection, MailAddressModel mailAddressModel)
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
    }
}
