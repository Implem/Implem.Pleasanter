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
    public static class WikiUtilities
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Index(SiteSettings siteSettings, Permissions.Types permissionType)
        {
            return new HtmlBuilder().NotFoundTemplate().ToString();
        }

        private static WikiCollection WikiCollection(
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData, int offset = 0)
        {
            return new WikiCollection(
                siteSettings: siteSettings,
                permissionType: permissionType,
                column: GridSqlColumnCollection(siteSettings),
                where: DataViewFilters.Get(
                    siteSettings: siteSettings,
                    tableName: "Wikis",
                    formData: formData,
                    where: Rds.WikisWhere().SiteId(siteSettings.SiteId)),
                orderBy: GridSorters.Get(
                    formData, Rds.WikisOrderBy().UpdatedTime(SqlOrderBy.Types.desc)),
                offset: offset,
                pageSize: siteSettings.GridPageSize.ToInt(),
                countRecord: true,
                aggregationCollection: siteSettings.AggregationCollection);
        }

        public static HtmlBuilder DataView(
            this HtmlBuilder hb,
            WikiCollection wikiCollection,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData,
            string dataViewName)
        {
            switch (dataViewName)
            {
                default: return hb.Grid(
                    wikiCollection: wikiCollection,
                    siteSettings: siteSettings,
                    permissionType: permissionType,
                    formData: formData);
            }
        }

        public static string DataView(
            SiteSettings siteSettings, Permissions.Types permissionType)
        {
            siteSettings.SetChoicesByLinks();
            siteSettings.SetChoicesByPlaceholders();
            switch (DataViewSelectors.Get(siteSettings.SiteId))
            {
                default: return Grid(siteSettings: siteSettings, permissionType: permissionType);
            }
        }

        private static HtmlBuilder Grid(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            WikiCollection wikiCollection,
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
                            wikiCollection: wikiCollection,
                            formData: formData))
                .Hidden(
                    controlId: "GridOffset",
                    value: siteSettings.GridPageSize == wikiCollection.Count()
                        ? siteSettings.GridPageSize.ToString()
                        : "-1");
        }

        private static string Grid(SiteSettings siteSettings, Permissions.Types permissionType)
        {
            var formData = DataViewFilters.SessionFormData(siteSettings.SiteId);
            var wikiCollection = WikiCollection(siteSettings, permissionType, formData);
            return new ResponseCollection()
                .Html("#DataViewContainer", new HtmlBuilder().Grid(
                    siteSettings: siteSettings,
                    wikiCollection: wikiCollection,
                    permissionType: permissionType,
                    formData: formData))
                .Html("#Aggregations", new HtmlBuilder().Aggregations(
                    siteSettings: siteSettings,
                    aggregations: wikiCollection.Aggregations,
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
            siteSettings.SetChoicesByLinks();
            siteSettings.SetChoicesByPlaceholders();
            var formData = DataViewFilters.SessionFormData(siteSettings.SiteId);
            var wikiCollection = WikiCollection(siteSettings, permissionType, formData, offset);
            return (responseCollection ?? new ResponseCollection())
                .Remove(".grid tr", _using: offset == 0)
                .ClearFormData("GridCheckAll", _using: clearCheck)
                .ClearFormData("GridUnCheckedItems", _using: clearCheck)
                .ClearFormData("GridCheckedItems", _using: clearCheck)
                .Message(message)
                .Append("#Grid", new HtmlBuilder().GridRows(
                    siteSettings: siteSettings,
                    wikiCollection: wikiCollection,
                    formData: formData,
                    addHeader: offset == 0,
                    clearCheck: clearCheck))
                .Html("#Aggregations", new HtmlBuilder().Aggregations(
                    siteSettings: siteSettings,
                    aggregations: wikiCollection.Aggregations,
                    container: false))
                .Val("#GridOffset", siteSettings.NextPageOffset(offset, wikiCollection.Count()))
                .Markup()
                .ToJson();
        }

        private static HtmlBuilder GridRows(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            WikiCollection wikiCollection,
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
                .TBody(action: () => wikiCollection
                    .ForEach(wikiModel => hb
                        .Tr(
                            attributes: new HtmlAttributes()
                                .Class("grid-row")
                                .DataId(wikiModel.WikiId.ToString()),
                            action: () =>
                            {
                                hb.Td(action: () => hb
                                    .CheckBox(
                                        controlCss: "grid-check",
                                        _checked: checkAll,
                                        dataId: wikiModel.WikiId.ToString()));
                                siteSettings.GridColumnCollection()
                                    .ForEach(column => hb
                                        .TdValue(
                                            column: column,
                                            wikiModel: wikiModel));
                            })));
        }

        private static SqlColumnCollection GridSqlColumnCollection(SiteSettings siteSettings)
        {
            var gridSqlColumn = Rds.WikisColumn()
                .SiteId()
                .WikiId()
                .Creator()
                .Updator();
            siteSettings.GridColumnCollection(withTitle: true).ForEach(column =>
                Rds.WikisColumn(gridSqlColumn, column.ColumnName));
            return gridSqlColumn;
        }

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
        public static string EditorNew(SiteModel siteModel, long siteId, bool byRest)
        {
            var wikiId = Rds.ExecuteScalar_long(statements:
                Rds.SelectWikis(
                    column: Rds.WikisColumn().WikiId(),
                    where: Rds.WikisWhere().SiteId(siteId)));
            return wikiId == 0
                ? Editor(
                    siteModel,
                    new WikiModel(
                        siteModel.WikisSiteSettings(),
                        siteModel.PermissionType,
                        methodType: BaseModel.MethodTypes.New)
                    {
                        SiteId = siteId
                    },
                    byRest: byRest)
                : new HtmlBuilder().NotFoundTemplate().ToString();
        }

        public static string Editor(SiteModel siteModel, long wikiId, bool clearSessions)
        {
            var siteSettings = siteModel.WikisSiteSettings();
            var wikiModel = new WikiModel(
                siteSettings: siteSettings,
                permissionType: siteModel.PermissionType,
                wikiId: wikiId,
                clearSessions: clearSessions,
                methodType: BaseModel.MethodTypes.Edit);
            wikiModel.SwitchTargets = WikiUtilities.GetSwitchTargets(
                siteSettings, wikiModel.SiteId);
            return Editor(siteModel, wikiModel, byRest: false);
        }

        public static string Editor(SiteModel siteModel, WikiModel wikiModel, bool byRest)
        {
            var hb = new HtmlBuilder();
            wikiModel.SiteSettings.SetChoicesByLinks();
            wikiModel.SiteSettings.SetChoicesByPlaceholders();
            return hb.Template(
                siteId: siteModel.SiteId,
                referenceType: "Wikis",
                title: wikiModel.MethodType == BaseModel.MethodTypes.New
                    ? siteModel.Title.DisplayValue + " - " + Displays.New()
                    : wikiModel.Title.DisplayValue,
                permissionType: wikiModel.PermissionType,
                verType: wikiModel.VerType,
                methodType: wikiModel.MethodType,
                allowAccess:
                    wikiModel.PermissionType.CanRead() &&
                    wikiModel.AccessStatus != Databases.AccessStatuses.NotFound,
                userScript: wikiModel.MethodType == BaseModel.MethodTypes.New
                    ? wikiModel.SiteSettings.NewScript
                    : wikiModel.SiteSettings.EditScript,
                userStyle: wikiModel.MethodType == BaseModel.MethodTypes.New
                    ? wikiModel.SiteSettings.NewStyle
                    : wikiModel.SiteSettings.EditStyle,
                byRest: byRest,
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
            return hb.Div(css: "edit-form", action: () => hb
                .Form(
                    attributes: new HtmlAttributes()
                        .Id("WikiForm")
                        .Class("main-form")
                        .Action(Navigations.ItemAction(wikiModel.WikiId != 0
                            ? wikiModel.WikiId
                            : siteModel.SiteId)),
                    action: () => hb
                        .RecordHeader(
                            id: wikiModel.WikiId,
                            baseModel: wikiModel,
                            tableName: "Wikis",
                            switchTargets: wikiModel.SwitchTargets)
                        .Div(css: "edit-form-comments", action: () => hb
                            .Comments(
                                comments: wikiModel.Comments,
                                verType: wikiModel.VerType))
                        .Div(css: "edit-form-tabs", action: () => hb
                            .FieldTabs(wikiModel: wikiModel)
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
                                backUrl: Navigations.ItemIndex(siteModel.ParentId),
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
                            value: wikiModel.SwitchTargets?.Join()))
                .OutgoingMailsForm("Wikis", wikiModel.WikiId, wikiModel.Ver)
                .CopyDialog("items", wikiModel.WikiId)
                .MoveDialog("items", wikiModel.WikiId)
                .OutgoingMailDialog());
        }

        private static HtmlBuilder FieldTabs(this HtmlBuilder hb, WikiModel wikiModel)
        {
            return hb.Ul(css: "field-tab", action: () => hb
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
            this ResponseCollection responseCollection, WikiModel wikiModel)
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
            this ResponseCollection responseCollection, WikiModel wikiModel)
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

        public static string BulkMove(SiteSettings siteSettings, Permissions.Types permissionType)
        {
            var siteId = Forms.Long("MoveTargets");
            if (Permissions.CanMove(siteSettings.SiteId, siteId))
            {
                var count = 0;
                if (Forms.Bool("GridCheckAll"))
                {
                    count = BulkMove(
                        siteId,
                        siteSettings,
                        GridItems("GridUnCheckedItems"),
                        negative: true);
                }
                else
                {
                    var checkedItems = GridItems("GridCheckedItems");
                    if (checkedItems.Any())
                    {
                        count = BulkMove(
                            siteId,
                            siteSettings,
                            checkedItems);
                    }
                    else
                    {
                        return Messages.ResponseSelectTargets().ToJson();
                    }
                }
                return GridRows(
                    siteSettings,
                    permissionType,
                    clearCheck: true,
                    message: Messages.BulkMoved(count.ToString()));
            }
            else
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
        }

        private static int BulkMove(
            long siteId,
            SiteSettings siteSettings,
            IEnumerable<long> checkedItems,
            bool negative = false)
        {
            return Rds.ExecuteScalar_int(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.UpdateWikis(
                        where: DataViewFilters.Get(
                            siteSettings: siteSettings,
                            tableName: "Wikis",
                            formData: DataViewFilters.SessionFormData(siteSettings.SiteId),
                            where: Rds.WikisWhere()
                                .SiteId(siteSettings.SiteId)
                                .WikiId_In(
                                    value: checkedItems,
                                    negative: negative,
                                    _using: checkedItems.Any())),
                        param: Rds.WikisParam().SiteId(siteId),
                        countRecord: true),
                    Rds.UpdateItems(
                        where: Rds.ItemsWhere()
                            .ReferenceId_In(
                                sub: Rds.SelectWikis(
                                    column: Rds.WikisColumn().WikiId(),
                                    where: Rds.WikisWhere().SiteId(siteId)))
                            .SiteId(siteId, _operator: "<>"),
                        param: Rds.ItemsParam().SiteId(siteId))
                });
        }

        public static string BulkDelete(
            Permissions.Types permissionType,
            SiteSettings siteSettings)
        {
            if (permissionType.CanDelete())
            {
                var count = 0;
                if (Forms.Bool("GridCheckAll"))
                {
                    count = BulkDelete(
                        siteSettings,
                        GridItems("GridUnCheckedItems"),
                        negative: true);
                }
                else
                {
                    var checkedItems = GridItems("GridCheckedItems");
                    if (checkedItems.Any())
                    {
                        count = BulkDelete(
                            siteSettings,
                            checkedItems);
                    }
                    else
                    {
                        return Messages.ResponseSelectTargets().ToJson();
                    }
                }
                return GridRows(
                    siteSettings,
                    permissionType,
                    clearCheck: true,
                    message: Messages.BulkDeleted(count.ToString()));
            }
            else
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
        }

        private static int BulkDelete(
            SiteSettings siteSettings,
            IEnumerable<long> checkedItems,
            bool negative = false)
        {
            var where = DataViewFilters.Get(
                siteSettings: siteSettings,
                tableName: "Wikis",
                formData: DataViewFilters.SessionFormData(siteSettings.SiteId),
                where: Rds.WikisWhere()
                    .SiteId(siteSettings.SiteId)
                    .WikiId_In(
                        value: checkedItems,
                        negative: negative,
                        _using: checkedItems.Any()));
            return Rds.ExecuteScalar_int(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.DeleteItems(
                        where: Rds.ItemsWhere()
                            .ReferenceId_In(
                                sub: Rds.SelectWikis(
                                    column: Rds.WikisColumn().WikiId(),
                                    where: where))),
                    Rds.DeleteWikis(
                        where: where, 
                        countRecord: true)
                });
        }

        private static IEnumerable<long> GridItems(string name)
        {
            return Forms.Data(name)
                .Split(',')
                .Select(o => o.ToLong())
                .Where(o => o != 0)
                .Distinct();
        }

        public static string Import(SiteModel siteModel)
        {
            if (!siteModel.PermissionType.CanCreate())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            var responseCollection = new ResponseCollection();
            Csv csv;
            try
            {
                csv = new Csv(Forms.File("Import"), Forms.Data("Encoding"));
            }
            catch
            {
                return Messages.ResponseFailedReadFile().ToJson();
            }
            if (csv != null && csv.Rows.Count() != 0)
            {
                var siteSettings = siteModel.WikisSiteSettings();
                var columnHash = new Dictionary<int, Column>();
                csv.Headers.Select((o, i) => new { Header = o, Index = i }).ForEach(data =>
                {
                    var column = siteSettings.ColumnCollection
                        .Where(o => o.LabelText == data.Header)
                        .FirstOrDefault();
                    if (column != null) columnHash.Add(data.Index, column);
                });
                var error = Imports.ColumnValidate(siteSettings, columnHash.Values
                    .Select(o => o.ColumnName), "Title");
                if (error != null) return error;
                var paramHash = new Dictionary<int, SqlParamCollection>();
                csv.Rows.Select((o, i) => new { Row = o, Index = i }).ForEach(data =>
                {
                    var param = Rds.WikisParam();
                    param.WikiId(raw: Def.Sql.Identity);
                    param.SiteId(siteModel.SiteId);
                    columnHash.ForEach(column =>
                    {
                        var recordingData = ImportRecordingData(
                            column.Value, data.Row[column.Key], siteModel.InheritPermission);
                        if (!param.Any(o => o.Name == column.Value.ColumnName))
                        {
                            switch (column.Value.ColumnName)
                            {
                                case "Title": param.Title(recordingData, _using: recordingData != null); break;
                                case "Body": param.Body(recordingData, _using: recordingData != null); break;
                                case "Comments": param.Comments(recordingData, _using: recordingData != null); break;
                            }
                        }
                    });
                    paramHash.Add(data.Index, param);
                });
                var errorTitle = Imports.Validate(
                    paramHash, siteSettings.GetColumn("Title"));
                if (errorTitle != null) return errorTitle;
                paramHash.Values.ForEach(param =>
                    new WikiModel(siteSettings, siteModel.PermissionType)
                    {
                        SiteId = siteModel.SiteId,
                        Title = new Title(param.FirstOrDefault(o =>
                            o.Name == "Title").Value.ToString()),
                        SiteSettings = siteSettings
                    }.Create(param: param));
                return GridRows(siteSettings, siteModel.PermissionType, responseCollection
                    .WindowScrollTop()
                    .CloseDialog("#ImportSettingsDialog")
                    .Message(Messages.Imported(csv.Rows.Count().ToString())));
            }
            else
            {
                return Messages.ResponseFileNotFound().ToJson();
            }
        }

        private static object ImportRecordingData(
            Column column, string value, long inheritPermission)
        {
            var recordingData = column.RecordingData(value, inheritPermission);
            return recordingData;
        }

        public static ResponseFile Export(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            SiteModel siteModel)
        {
            siteModel.SiteSettings.SetChoicesByLinks();
            siteModel.SiteSettings.SetChoicesByPlaceholders();
            var formData = DataViewFilters.SessionFormData(siteModel.SiteId);
            var wikiCollection = new WikiCollection(
                siteSettings: siteSettings,
                permissionType: permissionType,
                where: DataViewFilters.Get(
                    siteSettings: siteModel.SiteSettings,
                    tableName: "Wikis",
                    formData: formData,
                    where: Rds.WikisWhere().SiteId(siteModel.SiteId)),
                orderBy: GridSorters.Get(
                    formData, Rds.WikisOrderBy().UpdatedTime(SqlOrderBy.Types.desc)));
            var csv = new System.Text.StringBuilder();
            var exportColumns = (Sessions.PageSession(
                siteModel.Id, 
                "ExportSettings_ExportColumns").ToString().Deserialize<ExportColumns>());
            var columnHash = exportColumns.ColumnHash(siteModel.WikisSiteSettings());
            if (Sessions.PageSession(siteModel.Id, "ExportSettings_AddHeader").ToBool())
            {
                var header = new List<string>();
                exportColumns
                    .Columns
                    .Where(o => o.Value)
                    .ForEach(exportColumn => header.Add(
                        "\"" + columnHash[exportColumn.Key].LabelText + "\""));
                csv.Append(header.Join(","), "\n");
            }
            wikiCollection.ForEach(wikiModel =>
            {
                var row = new List<string>();
                exportColumns
                    .Columns
                    .Where(o => o.Value)
                    .ForEach(exportColumn =>
                        row.Add(CsvColumn(
                            wikiModel, 
                            exportColumn.Key, 
                            columnHash[exportColumn.Key])));
                csv.Append(row.Join(","), "\n");
            });
            return new ResponseFile(csv.ToString(), ResponseFileNames.Csv(siteModel));
        }

        private static string CsvColumn(
            WikiModel wikiModel, string columnName, Column column)
        {
            var value = string.Empty;
            switch (columnName)
            {
                case "SiteId": value = wikiModel.SiteId.ToExport(column); break;
                case "UpdatedTime": value = wikiModel.UpdatedTime.ToExport(column); break;
                case "WikiId": value = wikiModel.WikiId.ToExport(column); break;
                case "Ver": value = wikiModel.Ver.ToExport(column); break;
                case "Title": value = wikiModel.Title.ToExport(column); break;
                case "Body": value = wikiModel.Body.ToExport(column); break;
                case "TitleBody": value = wikiModel.TitleBody.ToExport(column); break;
                case "Comments": value = wikiModel.Comments.ToExport(column); break;
                case "Creator": value = wikiModel.Creator.ToExport(column); break;
                case "Updator": value = wikiModel.Updator.ToExport(column); break;
                case "CreatedTime": value = wikiModel.CreatedTime.ToExport(column); break;
                default: return string.Empty;
            }
            return "\"" + value.Replace("\"", "\"\"") + "\"";
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
                    ? column.Choice(wikiModel.Title.Value).Text()
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
                    ? column.Choice(dataRow["Title"].ToString()).Text()
                    : dataRow["Title"].ToString();
                default: return string.Empty;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        /// <returns></returns>
        public static string Editor(
            SiteModel siteModel,
            SiteSettings siteSettings,
            WikiModel wikiModel)
        {
            var hb = new HtmlBuilder();
            siteSettings.SetChoicesByLinks();
            siteSettings.SetChoicesByPlaceholders();
            return hb.Template(
                siteId: siteModel.SiteId,
                referenceType: "Wikis",
                title: wikiModel.MethodType != BaseModel.MethodTypes.New
                    ? wikiModel.Title.DisplayValue + " - " + Displays.Edit()
                    : siteModel.Title.DisplayValue + " - " + Displays.New(),
                permissionType: siteModel.PermissionType,
                verType: wikiModel.VerType,
                methodType: wikiModel.MethodType,
                allowAccess:
                    siteModel.PermissionType.CanRead() &&
                    wikiModel.AccessStatus != Databases.AccessStatuses.NotFound,
                useNavigationButtons: false,
                action: () =>
                {
                    hb
                        .Editor(
                            siteSettings: siteSettings,
                            wikiModel: wikiModel,
                            siteModel: siteModel)
                        .Hidden(controlId: "TableName", value: "Wikis");
                }).ToString();
        }
    }
}
