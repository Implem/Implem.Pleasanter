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
    public static class ResultUtilities
    {
        public static string Index(SiteSettings siteSettings, Permissions.Types permissionType)
        {
            var hb = new HtmlBuilder();
            var formData = DataViewFilters.SessionFormData(siteSettings.SiteId);
            var resultCollection = ResultCollection(siteSettings, permissionType, formData);
            var dataViewName = DataViewSelectors.Get(siteSettings.SiteId);
            return hb.DataViewTemplate(
                siteSettings: siteSettings,
                permissionType: permissionType,
                resultCollection: resultCollection,
                formData: formData,
                dataViewName: dataViewName,
                dataViewBody: () => hb.Grid(
                   resultCollection: resultCollection,
                   siteSettings: siteSettings,
                   permissionType: permissionType,
                   formData: formData));
        }

        private static string DataViewTemplate(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            ResultCollection resultCollection,
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
                referenceType: "Results",
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
                            .Id("ResultsForm")
                            .Class("main-form")
                            .Action(Navigations.ItemAction(siteSettings.SiteId)),
                        action: () => hb
                            .DataViewFilters(siteSettings: siteSettings)
                            .Aggregations(
                                siteSettings: siteSettings,
                                aggregations: resultCollection.Aggregations)
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
                            .Hidden(controlId: "TableName", value: "Results")
                            .Hidden(controlId: "BaseUrl", value: Navigations.BaseUrl()))
                    .MoveDialog(bulk: true)
                    .ImportSettingsDialog()
                    .Div(attributes: new HtmlAttributes()
                        .Id("ExportSettingsDialog")
                        .Class("dialog")
                        .Title(Displays.ExportSettings())))
                    .ToString();
        }

        public static string IndexJson(SiteSettings siteSettings, Permissions.Types permissionType)
        {
            var formData = DataViewFilters.SessionFormData(siteSettings.SiteId);
            var resultCollection = ResultCollection(siteSettings, permissionType, formData);
            return new ResponseCollection()
                .Html("#DataViewContainer", new HtmlBuilder().Grid(
                    siteSettings: siteSettings,
                    resultCollection: resultCollection,
                    permissionType: permissionType,
                    formData: formData))
                .DataViewFilters(siteSettings: siteSettings)
                .ReplaceAll("#Aggregations", new HtmlBuilder().Aggregations(
                    siteSettings: siteSettings,
                    aggregations: resultCollection.Aggregations))
                .WindowScrollTop().ToJson();
        }

        private static ResultCollection ResultCollection(
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData,
            int offset = 0)
        {
            return new ResultCollection(
                siteSettings: siteSettings,
                permissionType: permissionType,
                column: GridSqlColumnCollection(siteSettings),
                where: DataViewFilters.Get(
                    siteSettings: siteSettings,
                    tableName: "Results",
                    formData: formData,
                    where: Rds.ResultsWhere().SiteId(siteSettings.SiteId)),
                orderBy: GridSorters.Get(
                    formData, Rds.ResultsOrderBy().UpdatedTime(SqlOrderBy.Types.desc)),
                offset: offset,
                pageSize: siteSettings.GridPageSize.ToInt(),
                countRecord: true,
                aggregationCollection: siteSettings.AggregationCollection);
        }

        private static HtmlBuilder Grid(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            ResultCollection resultCollection,
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
                            resultCollection: resultCollection,
                            formData: formData))
                .Hidden(
                    controlId: "GridOffset",
                    value: siteSettings.GridPageSize == resultCollection.Count()
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
            var formData = DataViewFilters.SessionFormData(siteSettings.SiteId);
            var resultCollection = ResultCollection(siteSettings, permissionType, formData, offset);
            return (responseCollection ?? new ResponseCollection())
                .Remove(".grid tr", _using: offset == 0)
                .ClearFormData("GridCheckAll", _using: clearCheck)
                .ClearFormData("GridUnCheckedItems", _using: clearCheck)
                .ClearFormData("GridCheckedItems", _using: clearCheck)
                .Message(message)
                .Append("#Grid", new HtmlBuilder().GridRows(
                    siteSettings: siteSettings,
                    resultCollection: resultCollection,
                    formData: formData,
                    addHeader: offset == 0,
                    clearCheck: clearCheck))
                .Val("#GridOffset", siteSettings.NextPageOffset(offset, resultCollection.Count()))
                .ToJson();
        }

        private static HtmlBuilder GridRows(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            ResultCollection resultCollection,
            FormData formData,
            bool addHeader = true,
            bool clearCheck = false)
        {
            var checkAll = clearCheck ? false : Forms.Bool("GridCheckAll");
            var columns = siteSettings.GridColumnCollection();
            return hb
                .THead(
                    _using: addHeader,
                    action: () => hb
                        .GridHeader(
                            columnCollection: columns, 
                            formData: formData,
                            checkAll: checkAll))
                .TBody(action: () => resultCollection
                    .ForEach(resultModel => hb
                        .Tr(
                            attributes: new HtmlAttributes()
                                .Class("grid-row")
                                .DataId(resultModel.ResultId.ToString()),
                            action: () =>
                            {
                                hb.Td(action: () => hb
                                    .CheckBox(
                                        controlCss: "grid-check",
                                        _checked: checkAll,
                                        dataId: resultModel.ResultId.ToString()));
                                columns
                                    .ForEach(column => hb
                                        .TdValue(
                                            column: column,
                                            resultModel: resultModel));
                            })));
        }

        private static SqlColumnCollection GridSqlColumnCollection(SiteSettings siteSettings)
        {
            var sqlColumnCollection = Rds.ResultsColumn();
            new List<string> { "SiteId", "ResultId", "Creator", "Updator" }
                .Concat(siteSettings.GridColumnsOrder)
                .Concat(siteSettings.TitleColumnsOrder)
                    .Distinct().ForEach(column =>
                        sqlColumnCollection.ResultsColumn(column));
            return sqlColumnCollection;
        }

        public static HtmlBuilder TdValue(
            this HtmlBuilder hb, Column column, ResultModel resultModel)
        {
            switch (column.ColumnName)
            {
                case "SiteId": return hb.Td(column: column, value: resultModel.SiteId);
                case "UpdatedTime": return hb.Td(column: column, value: resultModel.UpdatedTime);
                case "ResultId": return hb.Td(column: column, value: resultModel.ResultId);
                case "Ver": return hb.Td(column: column, value: resultModel.Ver);
                case "Title": return hb.Td(column: column, value: resultModel.Title);
                case "Body": return hb.Td(column: column, value: resultModel.Body);
                case "TitleBody": return hb.Td(column: column, value: resultModel.TitleBody);
                case "Status": return hb.Td(column: column, value: resultModel.Status);
                case "Manager": return hb.Td(column: column, value: resultModel.Manager);
                case "Owner": return hb.Td(column: column, value: resultModel.Owner);
                case "ClassA": return hb.Td(column: column, value: resultModel.ClassA);
                case "ClassB": return hb.Td(column: column, value: resultModel.ClassB);
                case "ClassC": return hb.Td(column: column, value: resultModel.ClassC);
                case "ClassD": return hb.Td(column: column, value: resultModel.ClassD);
                case "ClassE": return hb.Td(column: column, value: resultModel.ClassE);
                case "ClassF": return hb.Td(column: column, value: resultModel.ClassF);
                case "ClassG": return hb.Td(column: column, value: resultModel.ClassG);
                case "ClassH": return hb.Td(column: column, value: resultModel.ClassH);
                case "ClassI": return hb.Td(column: column, value: resultModel.ClassI);
                case "ClassJ": return hb.Td(column: column, value: resultModel.ClassJ);
                case "ClassK": return hb.Td(column: column, value: resultModel.ClassK);
                case "ClassL": return hb.Td(column: column, value: resultModel.ClassL);
                case "ClassM": return hb.Td(column: column, value: resultModel.ClassM);
                case "ClassN": return hb.Td(column: column, value: resultModel.ClassN);
                case "ClassO": return hb.Td(column: column, value: resultModel.ClassO);
                case "ClassP": return hb.Td(column: column, value: resultModel.ClassP);
                case "ClassQ": return hb.Td(column: column, value: resultModel.ClassQ);
                case "ClassR": return hb.Td(column: column, value: resultModel.ClassR);
                case "ClassS": return hb.Td(column: column, value: resultModel.ClassS);
                case "ClassT": return hb.Td(column: column, value: resultModel.ClassT);
                case "ClassU": return hb.Td(column: column, value: resultModel.ClassU);
                case "ClassV": return hb.Td(column: column, value: resultModel.ClassV);
                case "ClassW": return hb.Td(column: column, value: resultModel.ClassW);
                case "ClassX": return hb.Td(column: column, value: resultModel.ClassX);
                case "ClassY": return hb.Td(column: column, value: resultModel.ClassY);
                case "ClassZ": return hb.Td(column: column, value: resultModel.ClassZ);
                case "NumA": return hb.Td(column: column, value: resultModel.NumA);
                case "NumB": return hb.Td(column: column, value: resultModel.NumB);
                case "NumC": return hb.Td(column: column, value: resultModel.NumC);
                case "NumD": return hb.Td(column: column, value: resultModel.NumD);
                case "NumE": return hb.Td(column: column, value: resultModel.NumE);
                case "NumF": return hb.Td(column: column, value: resultModel.NumF);
                case "NumG": return hb.Td(column: column, value: resultModel.NumG);
                case "NumH": return hb.Td(column: column, value: resultModel.NumH);
                case "NumI": return hb.Td(column: column, value: resultModel.NumI);
                case "NumJ": return hb.Td(column: column, value: resultModel.NumJ);
                case "NumK": return hb.Td(column: column, value: resultModel.NumK);
                case "NumL": return hb.Td(column: column, value: resultModel.NumL);
                case "NumM": return hb.Td(column: column, value: resultModel.NumM);
                case "NumN": return hb.Td(column: column, value: resultModel.NumN);
                case "NumO": return hb.Td(column: column, value: resultModel.NumO);
                case "NumP": return hb.Td(column: column, value: resultModel.NumP);
                case "NumQ": return hb.Td(column: column, value: resultModel.NumQ);
                case "NumR": return hb.Td(column: column, value: resultModel.NumR);
                case "NumS": return hb.Td(column: column, value: resultModel.NumS);
                case "NumT": return hb.Td(column: column, value: resultModel.NumT);
                case "NumU": return hb.Td(column: column, value: resultModel.NumU);
                case "NumV": return hb.Td(column: column, value: resultModel.NumV);
                case "NumW": return hb.Td(column: column, value: resultModel.NumW);
                case "NumX": return hb.Td(column: column, value: resultModel.NumX);
                case "NumY": return hb.Td(column: column, value: resultModel.NumY);
                case "NumZ": return hb.Td(column: column, value: resultModel.NumZ);
                case "DateA": return hb.Td(column: column, value: resultModel.DateA);
                case "DateB": return hb.Td(column: column, value: resultModel.DateB);
                case "DateC": return hb.Td(column: column, value: resultModel.DateC);
                case "DateD": return hb.Td(column: column, value: resultModel.DateD);
                case "DateE": return hb.Td(column: column, value: resultModel.DateE);
                case "DateF": return hb.Td(column: column, value: resultModel.DateF);
                case "DateG": return hb.Td(column: column, value: resultModel.DateG);
                case "DateH": return hb.Td(column: column, value: resultModel.DateH);
                case "DateI": return hb.Td(column: column, value: resultModel.DateI);
                case "DateJ": return hb.Td(column: column, value: resultModel.DateJ);
                case "DateK": return hb.Td(column: column, value: resultModel.DateK);
                case "DateL": return hb.Td(column: column, value: resultModel.DateL);
                case "DateM": return hb.Td(column: column, value: resultModel.DateM);
                case "DateN": return hb.Td(column: column, value: resultModel.DateN);
                case "DateO": return hb.Td(column: column, value: resultModel.DateO);
                case "DateP": return hb.Td(column: column, value: resultModel.DateP);
                case "DateQ": return hb.Td(column: column, value: resultModel.DateQ);
                case "DateR": return hb.Td(column: column, value: resultModel.DateR);
                case "DateS": return hb.Td(column: column, value: resultModel.DateS);
                case "DateT": return hb.Td(column: column, value: resultModel.DateT);
                case "DateU": return hb.Td(column: column, value: resultModel.DateU);
                case "DateV": return hb.Td(column: column, value: resultModel.DateV);
                case "DateW": return hb.Td(column: column, value: resultModel.DateW);
                case "DateX": return hb.Td(column: column, value: resultModel.DateX);
                case "DateY": return hb.Td(column: column, value: resultModel.DateY);
                case "DateZ": return hb.Td(column: column, value: resultModel.DateZ);
                case "DescriptionA": return hb.Td(column: column, value: resultModel.DescriptionA);
                case "DescriptionB": return hb.Td(column: column, value: resultModel.DescriptionB);
                case "DescriptionC": return hb.Td(column: column, value: resultModel.DescriptionC);
                case "DescriptionD": return hb.Td(column: column, value: resultModel.DescriptionD);
                case "DescriptionE": return hb.Td(column: column, value: resultModel.DescriptionE);
                case "DescriptionF": return hb.Td(column: column, value: resultModel.DescriptionF);
                case "DescriptionG": return hb.Td(column: column, value: resultModel.DescriptionG);
                case "DescriptionH": return hb.Td(column: column, value: resultModel.DescriptionH);
                case "DescriptionI": return hb.Td(column: column, value: resultModel.DescriptionI);
                case "DescriptionJ": return hb.Td(column: column, value: resultModel.DescriptionJ);
                case "DescriptionK": return hb.Td(column: column, value: resultModel.DescriptionK);
                case "DescriptionL": return hb.Td(column: column, value: resultModel.DescriptionL);
                case "DescriptionM": return hb.Td(column: column, value: resultModel.DescriptionM);
                case "DescriptionN": return hb.Td(column: column, value: resultModel.DescriptionN);
                case "DescriptionO": return hb.Td(column: column, value: resultModel.DescriptionO);
                case "DescriptionP": return hb.Td(column: column, value: resultModel.DescriptionP);
                case "DescriptionQ": return hb.Td(column: column, value: resultModel.DescriptionQ);
                case "DescriptionR": return hb.Td(column: column, value: resultModel.DescriptionR);
                case "DescriptionS": return hb.Td(column: column, value: resultModel.DescriptionS);
                case "DescriptionT": return hb.Td(column: column, value: resultModel.DescriptionT);
                case "DescriptionU": return hb.Td(column: column, value: resultModel.DescriptionU);
                case "DescriptionV": return hb.Td(column: column, value: resultModel.DescriptionV);
                case "DescriptionW": return hb.Td(column: column, value: resultModel.DescriptionW);
                case "DescriptionX": return hb.Td(column: column, value: resultModel.DescriptionX);
                case "DescriptionY": return hb.Td(column: column, value: resultModel.DescriptionY);
                case "DescriptionZ": return hb.Td(column: column, value: resultModel.DescriptionZ);
                case "CheckA": return hb.Td(column: column, value: resultModel.CheckA);
                case "CheckB": return hb.Td(column: column, value: resultModel.CheckB);
                case "CheckC": return hb.Td(column: column, value: resultModel.CheckC);
                case "CheckD": return hb.Td(column: column, value: resultModel.CheckD);
                case "CheckE": return hb.Td(column: column, value: resultModel.CheckE);
                case "CheckF": return hb.Td(column: column, value: resultModel.CheckF);
                case "CheckG": return hb.Td(column: column, value: resultModel.CheckG);
                case "CheckH": return hb.Td(column: column, value: resultModel.CheckH);
                case "CheckI": return hb.Td(column: column, value: resultModel.CheckI);
                case "CheckJ": return hb.Td(column: column, value: resultModel.CheckJ);
                case "CheckK": return hb.Td(column: column, value: resultModel.CheckK);
                case "CheckL": return hb.Td(column: column, value: resultModel.CheckL);
                case "CheckM": return hb.Td(column: column, value: resultModel.CheckM);
                case "CheckN": return hb.Td(column: column, value: resultModel.CheckN);
                case "CheckO": return hb.Td(column: column, value: resultModel.CheckO);
                case "CheckP": return hb.Td(column: column, value: resultModel.CheckP);
                case "CheckQ": return hb.Td(column: column, value: resultModel.CheckQ);
                case "CheckR": return hb.Td(column: column, value: resultModel.CheckR);
                case "CheckS": return hb.Td(column: column, value: resultModel.CheckS);
                case "CheckT": return hb.Td(column: column, value: resultModel.CheckT);
                case "CheckU": return hb.Td(column: column, value: resultModel.CheckU);
                case "CheckV": return hb.Td(column: column, value: resultModel.CheckV);
                case "CheckW": return hb.Td(column: column, value: resultModel.CheckW);
                case "CheckX": return hb.Td(column: column, value: resultModel.CheckX);
                case "CheckY": return hb.Td(column: column, value: resultModel.CheckY);
                case "CheckZ": return hb.Td(column: column, value: resultModel.CheckZ);
                case "Comments": return hb.Td(column: column, value: resultModel.Comments);
                case "Creator": return hb.Td(column: column, value: resultModel.Creator);
                case "Updator": return hb.Td(column: column, value: resultModel.Updator);
                case "CreatedTime": return hb.Td(column: column, value: resultModel.CreatedTime);
                default: return hb;
            }
        }

        public static string EditorNew(SiteModel siteModel)
        {
            return Editor(siteModel, new ResultModel(
                siteModel.ResultsSiteSettings(), methodType: BaseModel.MethodTypes.New));
        }

        public static string Editor(SiteModel siteModel, long resultId, bool clearSessions)
        {
            var siteSettings = siteModel.ResultsSiteSettings();
            var resultModel = new ResultModel(
                siteSettings: siteSettings,
                resultId: resultId,
                clearSessions: clearSessions,
                methodType: BaseModel.MethodTypes.Edit);
            resultModel.SwitchTargets = GetSwitchTargets(
                siteSettings, resultModel.ResultId, resultModel.SiteId);
            return Editor(siteModel, resultModel);
        }

        public static string Editor(SiteModel siteModel, ResultModel resultModel)
        {
            var hb = new HtmlBuilder();
            return hb.Template(
                permissionType: siteModel.PermissionType,
                verType: resultModel.VerType,
                methodType: resultModel.MethodType,
                allowAccess:
                    siteModel.PermissionType.CanRead() &&
                    resultModel.AccessStatus != Databases.AccessStatuses.NotFound,
                siteId: siteModel.SiteId,
                referenceType: "Results",
                title: resultModel.MethodType == BaseModel.MethodTypes.New
                    ? siteModel.Title.DisplayValue + " - " + Displays.New()
                    : resultModel.Title.DisplayValue,
                userScript: resultModel.MethodType == BaseModel.MethodTypes.New
                    ? resultModel.SiteSettings.NewScript
                    : resultModel.SiteSettings.EditScript,
                userStyle: resultModel.MethodType == BaseModel.MethodTypes.New
                    ? resultModel.SiteSettings.NewStyle
                    : resultModel.SiteSettings.EditStyle,
                action: () =>
                {
                    hb
                        .Editor(
                            siteSettings: resultModel.SiteSettings,
                            resultModel: resultModel,
                            siteModel: siteModel)
                        .Hidden(controlId: "TableName", value: "Results")
                        .Hidden(controlId: "Id", value: resultModel.ResultId.ToString());
                }).ToString();
        }

        private static HtmlBuilder Editor(
            this HtmlBuilder hb,
            SiteModel siteModel,
            SiteSettings siteSettings,
            ResultModel resultModel)
        {
            return hb.Div(id: "Editor", action: () => hb
                .Form(
                    attributes: new HtmlAttributes()
                        .Id("ResultForm")
                        .Class("main-form")
                        .Action(Navigations.ItemAction(resultModel.ResultId != 0 
                            ? resultModel.ResultId
                            : siteModel.SiteId)),
                    action: () => hb
                        .RecordHeader(
                            permissionType: siteModel.PermissionType,
                            baseModel: resultModel,
                            tableName: "Results")
                        .Div(id: "EditorComments", action: () => hb
                            .Comments(
                                comments: resultModel.Comments,
                                verType: resultModel.VerType))
                        .Div(id: "EditorTabsContainer", action: () => hb
                            .EditorTabs(resultModel: resultModel)
                            .FieldSetGeneral(
                                siteSettings: siteSettings,
                                permissionType: siteModel.PermissionType,
                                resultModel: resultModel)
                            .FieldSet(
                                attributes: new HtmlAttributes()
                                    .Id("FieldSetHistories")
                                    .DataAction("Histories")
                                    .DataMethod("get"),
                                _using: resultModel.MethodType != BaseModel.MethodTypes.New)
                            .MainCommands(
                                siteId: siteModel.SiteId,
                                permissionType: siteModel.PermissionType,
                                verType: resultModel.VerType,
                                referenceType: "items",
                                referenceId: resultModel.ResultId,
                                updateButton: true,
                                copyButton: true,
                                moveButton: true,
                                mailButton: true,
                                deleteButton: true,
                                extensions: () => hb
                                    .MainCommandExtensions(
                                        resultModel: resultModel,
                                        siteSettings: siteSettings)))
                        .Hidden(controlId: "BaseUrl", value: Navigations.BaseUrl())
                        .Hidden(
                            controlId: "MethodType",
                            value: resultModel.MethodType.ToString().ToLower())
                        .Hidden(
                            controlId: "Results_Timestamp",
                            css: "must-transport",
                            value: resultModel.Timestamp)
                        .Hidden(
                            controlId: "SwitchTargets",
                            css: "must-transport",
                            value: resultModel.SwitchTargets?.Join(),
                            _using: !Request.IsAjax()))
                .OutgoingMailsForm("Results", resultModel.ResultId, resultModel.Ver)
                .CopyDialog("items", resultModel.ResultId)
                .MoveDialog()
                .OutgoingMailDialog()
                .EditorExtensions(resultModel: resultModel, siteSettings: siteSettings));
        }

        private static HtmlBuilder EditorTabs(this HtmlBuilder hb, ResultModel resultModel)
        {
            return hb.Ul(id: "EditorTabs", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetGeneral", 
                        text: Displays.Basic()))
                .Li(
                    _using: resultModel.MethodType != BaseModel.MethodTypes.New,
                    action: () => hb
                        .A(
                            href: "#FieldSetHistories",
                            text: Displays.Histories())));
        }

        private static HtmlBuilder FieldSetGeneral(
            this HtmlBuilder hb,
            ResultModel resultModel,
            Permissions.Types permissionType,
            SiteSettings siteSettings)
        {
            return hb.FieldSet(id: "FieldSetGeneral", action: () =>
            {
                siteSettings.EditorColumnCollection().ForEach(column =>
                {
                    switch (column.ColumnName)
                    {
                        case "ResultId": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ResultId.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "Ver": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.Ver.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "Title": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.Title.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "Body": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.Body.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "Status": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.Status.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "Manager": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.Manager.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "Owner": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.Owner.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "ClassA": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassA.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "ClassB": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassB.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "ClassC": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassC.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "ClassD": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassD.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "ClassE": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassE.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "ClassF": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassF.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "ClassG": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassG.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "ClassH": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassH.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "ClassI": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassI.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "ClassJ": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassJ.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "ClassK": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassK.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "ClassL": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassL.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "ClassM": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassM.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "ClassN": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassN.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "ClassO": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassO.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "ClassP": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassP.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "ClassQ": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassQ.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "ClassR": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassR.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "ClassS": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassS.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "ClassT": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassT.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "ClassU": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassU.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "ClassV": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassV.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "ClassW": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassW.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "ClassX": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassX.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "ClassY": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassY.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "ClassZ": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassZ.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "NumA": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumA.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "NumB": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumB.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "NumC": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumC.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "NumD": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumD.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "NumE": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumE.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "NumF": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumF.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "NumG": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumG.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "NumH": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumH.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "NumI": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumI.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "NumJ": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumJ.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "NumK": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumK.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "NumL": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumL.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "NumM": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumM.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "NumN": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumN.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "NumO": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumO.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "NumP": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumP.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "NumQ": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumQ.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "NumR": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumR.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "NumS": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumS.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "NumT": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumT.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "NumU": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumU.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "NumV": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumV.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "NumW": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumW.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "NumX": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumX.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "NumY": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumY.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "NumZ": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumZ.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DateA": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateA.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DateB": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateB.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DateC": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateC.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DateD": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateD.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DateE": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateE.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DateF": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateF.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DateG": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateG.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DateH": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateH.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DateI": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateI.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DateJ": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateJ.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DateK": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateK.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DateL": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateL.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DateM": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateM.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DateN": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateN.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DateO": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateO.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DateP": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateP.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DateQ": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateQ.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DateR": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateR.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DateS": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateS.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DateT": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateT.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DateU": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateU.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DateV": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateV.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DateW": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateW.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DateX": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateX.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DateY": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateY.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DateZ": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateZ.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DescriptionA": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionA.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DescriptionB": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionB.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DescriptionC": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionC.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DescriptionD": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionD.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DescriptionE": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionE.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DescriptionF": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionF.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DescriptionG": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionG.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DescriptionH": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionH.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DescriptionI": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionI.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DescriptionJ": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionJ.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DescriptionK": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionK.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DescriptionL": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionL.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DescriptionM": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionM.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DescriptionN": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionN.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DescriptionO": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionO.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DescriptionP": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionP.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DescriptionQ": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionQ.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DescriptionR": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionR.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DescriptionS": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionS.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DescriptionT": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionT.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DescriptionU": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionU.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DescriptionV": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionV.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DescriptionW": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionW.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DescriptionX": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionX.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DescriptionY": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionY.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DescriptionZ": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionZ.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "CheckA": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckA.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "CheckB": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckB.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "CheckC": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckC.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "CheckD": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckD.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "CheckE": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckE.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "CheckF": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckF.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "CheckG": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckG.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "CheckH": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckH.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "CheckI": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckI.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "CheckJ": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckJ.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "CheckK": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckK.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "CheckL": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckL.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "CheckM": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckM.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "CheckN": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckN.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "CheckO": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckO.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "CheckP": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckP.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "CheckQ": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckQ.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "CheckR": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckR.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "CheckS": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckS.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "CheckT": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckT.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "CheckU": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckU.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "CheckV": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckV.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "CheckW": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckW.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "CheckX": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckX.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "CheckY": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckY.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "CheckZ": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckZ.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                    }
                });
                hb.VerUpCheckBox(resultModel);
                hb
                    .Div(id: "LinkCreations", css: "links", action: () => hb
                        .LinkCreations(
                            siteSettings: siteSettings,
                            linkId: resultModel.ResultId,
                            methodType: resultModel.MethodType))
                    .Div(id: "Links", css: "links", action: () => hb
                        .Links(linkId: resultModel.ResultId));
            });
        }

        private static HtmlBuilder MainCommandExtensions(
            this HtmlBuilder hb,
            ResultModel resultModel,
            SiteSettings siteSettings)
        {
            return hb;
        }

        private static HtmlBuilder EditorExtensions(
            this HtmlBuilder hb,
            ResultModel resultModel,
            SiteSettings siteSettings)
        {
            return hb;
        }

        public static string EditorJson(
            SiteSettings siteSettings, Permissions.Types permissionType, long resultId)
        {
            return EditorResponse(new ResultModel(siteSettings, resultId))
                .ToJson();
        }

        private static ResponseCollection EditorResponse(
            ResultModel resultModel, Message message = null, string switchTargets = null)
        {
            var siteModel = new SiteModel(resultModel.SiteId);
            resultModel.MethodType = BaseModel.MethodTypes.Edit;
            return new ResultsResponseCollection(resultModel)
                .Invoke("clearDialogs")
                .ReplaceAll("#MainContainer", Editor(siteModel, resultModel))
                .Val("#SwitchTargets", switchTargets, _using: switchTargets != null)
                .Invoke("setCurrentIndex")
                .Invoke("validateResults")
                .Message(message)
                .ClearFormData();
        }

        public static List<long> GetSwitchTargets(
            SiteSettings siteSettings, long resultId, long siteId)
        {
            var formData = DataViewFilters.SessionFormData(siteId);
            var switchTargets = Rds.ExecuteTable(
                transactional: false,
                statements: Rds.SelectResults(
                    column: Rds.ResultsColumn().ResultId(),
                    where: DataViewFilters.Get(
                        siteSettings: siteSettings,
                        tableName: "Results",
                        formData: formData,
                        where: Rds.ResultsWhere().SiteId(siteId)),
                    orderBy: GridSorters.Get(
                        formData, Rds.ResultsOrderBy().UpdatedTime(SqlOrderBy.Types.desc))))
                            .AsEnumerable()
                            .Select(o => o["ResultId"].ToLong())
                            .ToList();
            if (!switchTargets.Contains(resultId))
            {
                switchTargets.Add(resultId);
            }
            return switchTargets;
        }

        public static ResponseCollection FormResponse(
            this ResponseCollection responseCollection,
            Permissions.Types permissionType,
            ResultModel resultModel)
        {
            Forms.All().Keys.ForEach(key =>
            {
                switch (key)
                {
                    case "Results_NumA": responseCollection.Val("#" + key, resultModel.NumA.ToControl(resultModel.SiteSettings.GetColumn("NumA"), permissionType)); break;
                    case "Results_NumB": responseCollection.Val("#" + key, resultModel.NumB.ToControl(resultModel.SiteSettings.GetColumn("NumB"), permissionType)); break;
                    case "Results_NumC": responseCollection.Val("#" + key, resultModel.NumC.ToControl(resultModel.SiteSettings.GetColumn("NumC"), permissionType)); break;
                    case "Results_NumD": responseCollection.Val("#" + key, resultModel.NumD.ToControl(resultModel.SiteSettings.GetColumn("NumD"), permissionType)); break;
                    case "Results_NumE": responseCollection.Val("#" + key, resultModel.NumE.ToControl(resultModel.SiteSettings.GetColumn("NumE"), permissionType)); break;
                    case "Results_NumF": responseCollection.Val("#" + key, resultModel.NumF.ToControl(resultModel.SiteSettings.GetColumn("NumF"), permissionType)); break;
                    case "Results_NumG": responseCollection.Val("#" + key, resultModel.NumG.ToControl(resultModel.SiteSettings.GetColumn("NumG"), permissionType)); break;
                    case "Results_NumH": responseCollection.Val("#" + key, resultModel.NumH.ToControl(resultModel.SiteSettings.GetColumn("NumH"), permissionType)); break;
                    case "Results_NumI": responseCollection.Val("#" + key, resultModel.NumI.ToControl(resultModel.SiteSettings.GetColumn("NumI"), permissionType)); break;
                    case "Results_NumJ": responseCollection.Val("#" + key, resultModel.NumJ.ToControl(resultModel.SiteSettings.GetColumn("NumJ"), permissionType)); break;
                    case "Results_NumK": responseCollection.Val("#" + key, resultModel.NumK.ToControl(resultModel.SiteSettings.GetColumn("NumK"), permissionType)); break;
                    case "Results_NumL": responseCollection.Val("#" + key, resultModel.NumL.ToControl(resultModel.SiteSettings.GetColumn("NumL"), permissionType)); break;
                    case "Results_NumM": responseCollection.Val("#" + key, resultModel.NumM.ToControl(resultModel.SiteSettings.GetColumn("NumM"), permissionType)); break;
                    case "Results_NumN": responseCollection.Val("#" + key, resultModel.NumN.ToControl(resultModel.SiteSettings.GetColumn("NumN"), permissionType)); break;
                    case "Results_NumO": responseCollection.Val("#" + key, resultModel.NumO.ToControl(resultModel.SiteSettings.GetColumn("NumO"), permissionType)); break;
                    case "Results_NumP": responseCollection.Val("#" + key, resultModel.NumP.ToControl(resultModel.SiteSettings.GetColumn("NumP"), permissionType)); break;
                    case "Results_NumQ": responseCollection.Val("#" + key, resultModel.NumQ.ToControl(resultModel.SiteSettings.GetColumn("NumQ"), permissionType)); break;
                    case "Results_NumR": responseCollection.Val("#" + key, resultModel.NumR.ToControl(resultModel.SiteSettings.GetColumn("NumR"), permissionType)); break;
                    case "Results_NumS": responseCollection.Val("#" + key, resultModel.NumS.ToControl(resultModel.SiteSettings.GetColumn("NumS"), permissionType)); break;
                    case "Results_NumT": responseCollection.Val("#" + key, resultModel.NumT.ToControl(resultModel.SiteSettings.GetColumn("NumT"), permissionType)); break;
                    case "Results_NumU": responseCollection.Val("#" + key, resultModel.NumU.ToControl(resultModel.SiteSettings.GetColumn("NumU"), permissionType)); break;
                    case "Results_NumV": responseCollection.Val("#" + key, resultModel.NumV.ToControl(resultModel.SiteSettings.GetColumn("NumV"), permissionType)); break;
                    case "Results_NumW": responseCollection.Val("#" + key, resultModel.NumW.ToControl(resultModel.SiteSettings.GetColumn("NumW"), permissionType)); break;
                    case "Results_NumX": responseCollection.Val("#" + key, resultModel.NumX.ToControl(resultModel.SiteSettings.GetColumn("NumX"), permissionType)); break;
                    case "Results_NumY": responseCollection.Val("#" + key, resultModel.NumY.ToControl(resultModel.SiteSettings.GetColumn("NumY"), permissionType)); break;
                    case "Results_NumZ": responseCollection.Val("#" + key, resultModel.NumZ.ToControl(resultModel.SiteSettings.GetColumn("NumZ"), permissionType)); break;
                    default: break;
                }
            });
            return responseCollection;
        }

        public static ResponseCollection Formula(
            this ResponseCollection responseCollection,
            Permissions.Types permissionType,
            ResultModel resultModel)
        {
            resultModel.SiteSettings.FormulaHash?.Keys.ForEach(columnName =>
            {
                var column = resultModel.SiteSettings.GetColumn(columnName);
                switch (columnName)
                {
                    case "NumA": responseCollection.Val("#Results_NumA", resultModel.NumA.ToControl(column, permissionType)); break;
                    case "NumB": responseCollection.Val("#Results_NumB", resultModel.NumB.ToControl(column, permissionType)); break;
                    case "NumC": responseCollection.Val("#Results_NumC", resultModel.NumC.ToControl(column, permissionType)); break;
                    case "NumD": responseCollection.Val("#Results_NumD", resultModel.NumD.ToControl(column, permissionType)); break;
                    case "NumE": responseCollection.Val("#Results_NumE", resultModel.NumE.ToControl(column, permissionType)); break;
                    case "NumF": responseCollection.Val("#Results_NumF", resultModel.NumF.ToControl(column, permissionType)); break;
                    case "NumG": responseCollection.Val("#Results_NumG", resultModel.NumG.ToControl(column, permissionType)); break;
                    case "NumH": responseCollection.Val("#Results_NumH", resultModel.NumH.ToControl(column, permissionType)); break;
                    case "NumI": responseCollection.Val("#Results_NumI", resultModel.NumI.ToControl(column, permissionType)); break;
                    case "NumJ": responseCollection.Val("#Results_NumJ", resultModel.NumJ.ToControl(column, permissionType)); break;
                    case "NumK": responseCollection.Val("#Results_NumK", resultModel.NumK.ToControl(column, permissionType)); break;
                    case "NumL": responseCollection.Val("#Results_NumL", resultModel.NumL.ToControl(column, permissionType)); break;
                    case "NumM": responseCollection.Val("#Results_NumM", resultModel.NumM.ToControl(column, permissionType)); break;
                    case "NumN": responseCollection.Val("#Results_NumN", resultModel.NumN.ToControl(column, permissionType)); break;
                    case "NumO": responseCollection.Val("#Results_NumO", resultModel.NumO.ToControl(column, permissionType)); break;
                    case "NumP": responseCollection.Val("#Results_NumP", resultModel.NumP.ToControl(column, permissionType)); break;
                    case "NumQ": responseCollection.Val("#Results_NumQ", resultModel.NumQ.ToControl(column, permissionType)); break;
                    case "NumR": responseCollection.Val("#Results_NumR", resultModel.NumR.ToControl(column, permissionType)); break;
                    case "NumS": responseCollection.Val("#Results_NumS", resultModel.NumS.ToControl(column, permissionType)); break;
                    case "NumT": responseCollection.Val("#Results_NumT", resultModel.NumT.ToControl(column, permissionType)); break;
                    case "NumU": responseCollection.Val("#Results_NumU", resultModel.NumU.ToControl(column, permissionType)); break;
                    case "NumV": responseCollection.Val("#Results_NumV", resultModel.NumV.ToControl(column, permissionType)); break;
                    case "NumW": responseCollection.Val("#Results_NumW", resultModel.NumW.ToControl(column, permissionType)); break;
                    case "NumX": responseCollection.Val("#Results_NumX", resultModel.NumX.ToControl(column, permissionType)); break;
                    case "NumY": responseCollection.Val("#Results_NumY", resultModel.NumY.ToControl(column, permissionType)); break;
                    case "NumZ": responseCollection.Val("#Results_NumZ", resultModel.NumZ.ToControl(column, permissionType)); break;
                    default: break;
                }
            });
            return responseCollection;
        }

        public static string Create(SiteSettings siteSettings, Permissions.Types permissionType)
        {
            var resultModel = new ResultModel(siteSettings, 0, setByForm: true);
            var invalid = ResultValidators.OnCreating(siteSettings, permissionType, resultModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var error = resultModel.Create(notice: true);
            if (error.Has())
            {
                return error.MessageJson();
            }
            else
            {
                return EditorResponse(
                    resultModel,
                    Messages.Created(resultModel.Title.Value),
                    GetSwitchTargets(
                        siteSettings, resultModel.ResultId, resultModel.SiteId).Join())
                            .ToJson();
            }
        }

        public static string Update(
            SiteSettings siteSettings, Permissions.Types permissionType, long resultId)
        {
            var resultModel = new ResultModel(siteSettings, resultId, setByForm: true);
            var invalid = ResultValidators.OnUpdating(siteSettings, permissionType, resultModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return new ResponseCollection().Message(invalid.Message()).ToJson();
            }
            if (resultModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts().ToJson();
            }
            var error = resultModel.Update(notice: true);
            if (error.Has())
            {
                return error == Error.Types.UpdateConflicts
                    ? Messages.ResponseUpdateConflicts(resultModel.Updator.FullName()).ToJson()
                    : new ResponseCollection().Message(error.Message()).ToJson();
            }
            else
            {
                var responseCollection = new ResultsResponseCollection(resultModel);
                return ResponseByUpdate(permissionType, resultModel, responseCollection)
                    .PrependComment(resultModel.Comments, resultModel.VerType)
                    .ToJson();
            }
        }

        private static ResponseCollection ResponseByUpdate(
            Permissions.Types permissionType,
            ResultModel resultModel,
            ResultsResponseCollection responseCollection)
        {
            return responseCollection
                .Ver()
                .Timestamp()
                .Val("#VerUp", false)
                .FormResponse(permissionType, resultModel)
                .Formula(permissionType, resultModel)
                .Disabled("#VerUp", false)
                .Html("#HeaderTitle", resultModel.Title.DisplayValue)
                .Html("#RecordInfo", new HtmlBuilder().RecordInfo(
                    baseModel: resultModel, tableName: "Results"))
                .Html("#Links", new HtmlBuilder().Links(resultModel.ResultId))
                .Message(Messages.Updated(resultModel.Title.ToString()))
                .RemoveComment(resultModel.DeleteCommentId, _using: resultModel.DeleteCommentId != 0)
                .ClearFormData();
        }

        public static string Copy(SiteSettings siteSettings, Permissions.Types permissionType, long resultId)
        {
            var resultModel = new ResultModel(siteSettings, resultId, setByForm: true);
            resultModel.ResultId = 0;
            if (siteSettings.EditorColumnsOrder.Contains("Title"))
            {
                resultModel.Title.Value += Displays.SuffixCopy();
            }
            if (!Forms.Data("CopyWithComments").ToBool())
            {
                resultModel.Comments.Clear();
            }
            var error = resultModel.Create(paramAll: true);
            if (error.Has())
            {
                return error.MessageJson();
            }
            else
            {
            return EditorResponse(
                resultModel,
                Messages.Copied(resultModel.Title.Value),
                GetSwitchTargets(
                    siteSettings, resultModel.ResultId, resultModel.SiteId).Join())
                        .ToJson();
            }
        }

        public static string Move(
            SiteSettings siteSettings, Permissions.Types permissionType, long resultId)
        {
            var targetSiteId = Forms.Long("MoveTargets");
            var resultModel = new ResultModel(siteSettings, resultId);
            var invalid = ResultValidators.OnMoving(
                permissionType, Permissions.GetBySiteId(targetSiteId));
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var error = resultModel.Move(targetSiteId);
            if (error.Has())
            {
                return error.MessageJson();
            }
            else
            {
                return EditorResponse(resultModel)
                    .Message(Messages.Moved(resultModel.Title.Value))
                    .Val("#BackUrl", Navigations.ItemIndex(targetSiteId))
                    .ToJson();
            }
        }

        public static string Delete(
            SiteSettings siteSettings, Permissions.Types permissionType, long resultId)
        {
            var resultModel = new ResultModel(siteSettings, resultId);
            var invalid = ResultValidators.OnDeleting(siteSettings, permissionType, resultModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var error = resultModel.Delete(notice: true);
            if (error.Has())
            {
                return error.MessageJson();
            }
            else
            {
                Sessions.Set("Message", Messages.Deleted(resultModel.Title.Value).Html);
                var responseCollection = new ResultsResponseCollection(resultModel);
                responseCollection.Href(Navigations.ItemIndex(resultModel.SiteId));
                return responseCollection.ToJson();
            }
        }

        public static string Restore(long resultId)
        {
            var resultModel = new ResultModel();
            var invalid = ResultValidators.OnRestoring();
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var error = resultModel.Restore(resultId);
            if (error.Has())
            {
                return error.MessageJson();
            }
            else
            {
                var responseCollection = new ResultsResponseCollection(resultModel);
                return responseCollection.ToJson();
            }
        }

        public static string Histories(
            SiteSettings siteSettings, Permissions.Types permissionType, long resultId)
        {
            var resultModel = new ResultModel(siteSettings, resultId);
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
                        new ResultCollection(
                            siteSettings: siteSettings,
                            permissionType: permissionType,
                            where: Rds.ResultsWhere().ResultId(resultModel.ResultId),
                            orderBy: Rds.ResultsOrderBy().Ver(SqlOrderBy.Types.desc),
                            tableType: Sqls.TableTypes.NormalAndHistory)
                                .ForEach(resultModelHistory => hb
                                    .Tr(
                                        attributes: new HtmlAttributes()
                                            .Class("grid-row history not-link")
                                            .DataAction("History")
                                            .DataMethod("post")
                                            .DataVer(resultModelHistory.Ver)
                                            .DataLatest(1, _using:
                                                resultModelHistory.Ver == resultModel.Ver),
                                        action: () => columns
                                            .ForEach(column => hb
                                                .TdValue(column, resultModelHistory))))));
            return new ResultsResponseCollection(resultModel)
                .Html("#FieldSetHistories", hb).ToJson();
        }

        public static string History(
            SiteSettings siteSettings, Permissions.Types permissionType, long resultId)
        {
            var resultModel = new ResultModel(siteSettings, resultId);
            resultModel.Get(
                where: Rds.ResultsWhere()
                    .ResultId(resultModel.ResultId)
                    .Ver(Forms.Int("Ver")),
                tableType: Sqls.TableTypes.NormalAndHistory);
            resultModel.VerType =  Forms.Bool("Latest")
                ? Versions.VerTypes.Latest
                : Versions.VerTypes.History;
            return EditorResponse(resultModel).ToJson();
        }

        public static string BulkMove(SiteSettings siteSettings, Permissions.Types permissionType)
        {
            var siteId = Forms.Long("MoveTargets");
            if (Permissions.CanMove(permissionType, Permissions.GetBySiteId(siteId)))
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
                    Rds.UpdateResults(
                        where: DataViewFilters.Get(
                            siteSettings: siteSettings,
                            tableName: "Results",
                            formData: DataViewFilters.SessionFormData(siteSettings.SiteId),
                            where: Rds.ResultsWhere()
                                .SiteId(siteSettings.SiteId)
                                .ResultId_In(
                                    value: checkedItems,
                                    negative: negative,
                                    _using: checkedItems.Any())),
                        param: Rds.ResultsParam().SiteId(siteId),
                        countRecord: true),
                    Rds.UpdateItems(
                        where: Rds.ItemsWhere()
                            .ReferenceId_In(
                                sub: Rds.SelectResults(
                                    column: Rds.ResultsColumn().ResultId(),
                                    where: Rds.ResultsWhere().SiteId(siteId)))
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
                tableName: "Results",
                formData: DataViewFilters.SessionFormData(siteSettings.SiteId),
                where: Rds.ResultsWhere()
                    .SiteId(siteSettings.SiteId)
                    .ResultId_In(
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
                                sub: Rds.SelectResults(
                                    column: Rds.ResultsColumn().ResultId(),
                                    where: where))),
                    Rds.DeleteResults(
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
                var siteSettings = siteModel.ResultsSiteSettings();
                var columnHash = new Dictionary<int, Column>();
                csv.Headers.Select((o, i) => new { Header = o, Index = i }).ForEach(data =>
                {
                    var column = siteSettings.ColumnCollection
                        .Where(o => o.LabelText == data.Header)
                        .FirstOrDefault();
                    if (column != null) columnHash.Add(data.Index, column);
                });
                var error = Imports.ColumnValidate(siteSettings, columnHash.Values
                    .Select(o => o.ColumnName));
                if (error != null) return error;
                var paramHash = new Dictionary<int, SqlParamCollection>();
                csv.Rows.Select((o, i) => new { Row = o, Index = i }).ForEach(data =>
                {
                    var param = Rds.ResultsParam();
                    param.ResultId(raw: Def.Sql.Identity);
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
                                case "Status": param.Status(recordingData, _using: recordingData != null); break;
                                case "Manager": param.Manager(recordingData, _using: recordingData != null); break;
                                case "Owner": param.Owner(recordingData, _using: recordingData != null); break;
                                case "ClassA": param.ClassA(recordingData, _using: recordingData != null); break;
                                case "ClassB": param.ClassB(recordingData, _using: recordingData != null); break;
                                case "ClassC": param.ClassC(recordingData, _using: recordingData != null); break;
                                case "ClassD": param.ClassD(recordingData, _using: recordingData != null); break;
                                case "ClassE": param.ClassE(recordingData, _using: recordingData != null); break;
                                case "ClassF": param.ClassF(recordingData, _using: recordingData != null); break;
                                case "ClassG": param.ClassG(recordingData, _using: recordingData != null); break;
                                case "ClassH": param.ClassH(recordingData, _using: recordingData != null); break;
                                case "ClassI": param.ClassI(recordingData, _using: recordingData != null); break;
                                case "ClassJ": param.ClassJ(recordingData, _using: recordingData != null); break;
                                case "ClassK": param.ClassK(recordingData, _using: recordingData != null); break;
                                case "ClassL": param.ClassL(recordingData, _using: recordingData != null); break;
                                case "ClassM": param.ClassM(recordingData, _using: recordingData != null); break;
                                case "ClassN": param.ClassN(recordingData, _using: recordingData != null); break;
                                case "ClassO": param.ClassO(recordingData, _using: recordingData != null); break;
                                case "ClassP": param.ClassP(recordingData, _using: recordingData != null); break;
                                case "ClassQ": param.ClassQ(recordingData, _using: recordingData != null); break;
                                case "ClassR": param.ClassR(recordingData, _using: recordingData != null); break;
                                case "ClassS": param.ClassS(recordingData, _using: recordingData != null); break;
                                case "ClassT": param.ClassT(recordingData, _using: recordingData != null); break;
                                case "ClassU": param.ClassU(recordingData, _using: recordingData != null); break;
                                case "ClassV": param.ClassV(recordingData, _using: recordingData != null); break;
                                case "ClassW": param.ClassW(recordingData, _using: recordingData != null); break;
                                case "ClassX": param.ClassX(recordingData, _using: recordingData != null); break;
                                case "ClassY": param.ClassY(recordingData, _using: recordingData != null); break;
                                case "ClassZ": param.ClassZ(recordingData, _using: recordingData != null); break;
                                case "NumA": param.NumA(recordingData, _using: recordingData != null); break;
                                case "NumB": param.NumB(recordingData, _using: recordingData != null); break;
                                case "NumC": param.NumC(recordingData, _using: recordingData != null); break;
                                case "NumD": param.NumD(recordingData, _using: recordingData != null); break;
                                case "NumE": param.NumE(recordingData, _using: recordingData != null); break;
                                case "NumF": param.NumF(recordingData, _using: recordingData != null); break;
                                case "NumG": param.NumG(recordingData, _using: recordingData != null); break;
                                case "NumH": param.NumH(recordingData, _using: recordingData != null); break;
                                case "NumI": param.NumI(recordingData, _using: recordingData != null); break;
                                case "NumJ": param.NumJ(recordingData, _using: recordingData != null); break;
                                case "NumK": param.NumK(recordingData, _using: recordingData != null); break;
                                case "NumL": param.NumL(recordingData, _using: recordingData != null); break;
                                case "NumM": param.NumM(recordingData, _using: recordingData != null); break;
                                case "NumN": param.NumN(recordingData, _using: recordingData != null); break;
                                case "NumO": param.NumO(recordingData, _using: recordingData != null); break;
                                case "NumP": param.NumP(recordingData, _using: recordingData != null); break;
                                case "NumQ": param.NumQ(recordingData, _using: recordingData != null); break;
                                case "NumR": param.NumR(recordingData, _using: recordingData != null); break;
                                case "NumS": param.NumS(recordingData, _using: recordingData != null); break;
                                case "NumT": param.NumT(recordingData, _using: recordingData != null); break;
                                case "NumU": param.NumU(recordingData, _using: recordingData != null); break;
                                case "NumV": param.NumV(recordingData, _using: recordingData != null); break;
                                case "NumW": param.NumW(recordingData, _using: recordingData != null); break;
                                case "NumX": param.NumX(recordingData, _using: recordingData != null); break;
                                case "NumY": param.NumY(recordingData, _using: recordingData != null); break;
                                case "NumZ": param.NumZ(recordingData, _using: recordingData != null); break;
                                case "DateA": param.DateA(recordingData, _using: recordingData != null); break;
                                case "DateB": param.DateB(recordingData, _using: recordingData != null); break;
                                case "DateC": param.DateC(recordingData, _using: recordingData != null); break;
                                case "DateD": param.DateD(recordingData, _using: recordingData != null); break;
                                case "DateE": param.DateE(recordingData, _using: recordingData != null); break;
                                case "DateF": param.DateF(recordingData, _using: recordingData != null); break;
                                case "DateG": param.DateG(recordingData, _using: recordingData != null); break;
                                case "DateH": param.DateH(recordingData, _using: recordingData != null); break;
                                case "DateI": param.DateI(recordingData, _using: recordingData != null); break;
                                case "DateJ": param.DateJ(recordingData, _using: recordingData != null); break;
                                case "DateK": param.DateK(recordingData, _using: recordingData != null); break;
                                case "DateL": param.DateL(recordingData, _using: recordingData != null); break;
                                case "DateM": param.DateM(recordingData, _using: recordingData != null); break;
                                case "DateN": param.DateN(recordingData, _using: recordingData != null); break;
                                case "DateO": param.DateO(recordingData, _using: recordingData != null); break;
                                case "DateP": param.DateP(recordingData, _using: recordingData != null); break;
                                case "DateQ": param.DateQ(recordingData, _using: recordingData != null); break;
                                case "DateR": param.DateR(recordingData, _using: recordingData != null); break;
                                case "DateS": param.DateS(recordingData, _using: recordingData != null); break;
                                case "DateT": param.DateT(recordingData, _using: recordingData != null); break;
                                case "DateU": param.DateU(recordingData, _using: recordingData != null); break;
                                case "DateV": param.DateV(recordingData, _using: recordingData != null); break;
                                case "DateW": param.DateW(recordingData, _using: recordingData != null); break;
                                case "DateX": param.DateX(recordingData, _using: recordingData != null); break;
                                case "DateY": param.DateY(recordingData, _using: recordingData != null); break;
                                case "DateZ": param.DateZ(recordingData, _using: recordingData != null); break;
                                case "DescriptionA": param.DescriptionA(recordingData, _using: recordingData != null); break;
                                case "DescriptionB": param.DescriptionB(recordingData, _using: recordingData != null); break;
                                case "DescriptionC": param.DescriptionC(recordingData, _using: recordingData != null); break;
                                case "DescriptionD": param.DescriptionD(recordingData, _using: recordingData != null); break;
                                case "DescriptionE": param.DescriptionE(recordingData, _using: recordingData != null); break;
                                case "DescriptionF": param.DescriptionF(recordingData, _using: recordingData != null); break;
                                case "DescriptionG": param.DescriptionG(recordingData, _using: recordingData != null); break;
                                case "DescriptionH": param.DescriptionH(recordingData, _using: recordingData != null); break;
                                case "DescriptionI": param.DescriptionI(recordingData, _using: recordingData != null); break;
                                case "DescriptionJ": param.DescriptionJ(recordingData, _using: recordingData != null); break;
                                case "DescriptionK": param.DescriptionK(recordingData, _using: recordingData != null); break;
                                case "DescriptionL": param.DescriptionL(recordingData, _using: recordingData != null); break;
                                case "DescriptionM": param.DescriptionM(recordingData, _using: recordingData != null); break;
                                case "DescriptionN": param.DescriptionN(recordingData, _using: recordingData != null); break;
                                case "DescriptionO": param.DescriptionO(recordingData, _using: recordingData != null); break;
                                case "DescriptionP": param.DescriptionP(recordingData, _using: recordingData != null); break;
                                case "DescriptionQ": param.DescriptionQ(recordingData, _using: recordingData != null); break;
                                case "DescriptionR": param.DescriptionR(recordingData, _using: recordingData != null); break;
                                case "DescriptionS": param.DescriptionS(recordingData, _using: recordingData != null); break;
                                case "DescriptionT": param.DescriptionT(recordingData, _using: recordingData != null); break;
                                case "DescriptionU": param.DescriptionU(recordingData, _using: recordingData != null); break;
                                case "DescriptionV": param.DescriptionV(recordingData, _using: recordingData != null); break;
                                case "DescriptionW": param.DescriptionW(recordingData, _using: recordingData != null); break;
                                case "DescriptionX": param.DescriptionX(recordingData, _using: recordingData != null); break;
                                case "DescriptionY": param.DescriptionY(recordingData, _using: recordingData != null); break;
                                case "DescriptionZ": param.DescriptionZ(recordingData, _using: recordingData != null); break;
                                case "CheckA": param.CheckA(recordingData, _using: recordingData != null); break;
                                case "CheckB": param.CheckB(recordingData, _using: recordingData != null); break;
                                case "CheckC": param.CheckC(recordingData, _using: recordingData != null); break;
                                case "CheckD": param.CheckD(recordingData, _using: recordingData != null); break;
                                case "CheckE": param.CheckE(recordingData, _using: recordingData != null); break;
                                case "CheckF": param.CheckF(recordingData, _using: recordingData != null); break;
                                case "CheckG": param.CheckG(recordingData, _using: recordingData != null); break;
                                case "CheckH": param.CheckH(recordingData, _using: recordingData != null); break;
                                case "CheckI": param.CheckI(recordingData, _using: recordingData != null); break;
                                case "CheckJ": param.CheckJ(recordingData, _using: recordingData != null); break;
                                case "CheckK": param.CheckK(recordingData, _using: recordingData != null); break;
                                case "CheckL": param.CheckL(recordingData, _using: recordingData != null); break;
                                case "CheckM": param.CheckM(recordingData, _using: recordingData != null); break;
                                case "CheckN": param.CheckN(recordingData, _using: recordingData != null); break;
                                case "CheckO": param.CheckO(recordingData, _using: recordingData != null); break;
                                case "CheckP": param.CheckP(recordingData, _using: recordingData != null); break;
                                case "CheckQ": param.CheckQ(recordingData, _using: recordingData != null); break;
                                case "CheckR": param.CheckR(recordingData, _using: recordingData != null); break;
                                case "CheckS": param.CheckS(recordingData, _using: recordingData != null); break;
                                case "CheckT": param.CheckT(recordingData, _using: recordingData != null); break;
                                case "CheckU": param.CheckU(recordingData, _using: recordingData != null); break;
                                case "CheckV": param.CheckV(recordingData, _using: recordingData != null); break;
                                case "CheckW": param.CheckW(recordingData, _using: recordingData != null); break;
                                case "CheckX": param.CheckX(recordingData, _using: recordingData != null); break;
                                case "CheckY": param.CheckY(recordingData, _using: recordingData != null); break;
                                case "CheckZ": param.CheckZ(recordingData, _using: recordingData != null); break;
                                case "Comments": param.Comments(recordingData, _using: recordingData != null); break;
                            }
                        }
                    });
                    paramHash.Add(data.Index, param);
                });
                paramHash.Values.ForEach(param =>
                    new ResultModel(siteSettings)
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
            var formData = DataViewFilters.SessionFormData(siteModel.SiteId);
            var resultCollection = new ResultCollection(
                siteSettings: siteSettings,
                permissionType: permissionType,
                where: DataViewFilters.Get(
                    siteSettings: siteModel.SiteSettings,
                    tableName: "Results",
                    formData: formData,
                    where: Rds.ResultsWhere().SiteId(siteModel.SiteId)),
                orderBy: GridSorters.Get(
                    formData, Rds.ResultsOrderBy().UpdatedTime(SqlOrderBy.Types.desc)));
            var csv = new System.Text.StringBuilder();
            var exportColumns = (Sessions.PageSession(
                siteModel.Id, 
                "ExportSettings_ExportColumns").ToString().Deserialize<ExportColumns>());
            var columnHash = exportColumns.ColumnHash(siteModel.ResultsSiteSettings());
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
            resultCollection.ForEach(resultModel =>
            {
                var row = new List<string>();
                exportColumns
                    .Columns
                    .Where(o => o.Value)
                    .ForEach(exportColumn =>
                        row.Add(CsvColumn(
                            resultModel, 
                            exportColumn.Key, 
                            columnHash[exportColumn.Key])));
                csv.Append(row.Join(","), "\n");
            });
            return new ResponseFile(csv.ToString(), ResponseFileNames.Csv(siteModel));
        }

        private static string CsvColumn(
            ResultModel resultModel, string columnName, Column column)
        {
            var value = string.Empty;
            switch (columnName)
            {
                case "SiteId": value = resultModel.SiteId.ToExport(column); break;
                case "UpdatedTime": value = resultModel.UpdatedTime.ToExport(column); break;
                case "ResultId": value = resultModel.ResultId.ToExport(column); break;
                case "Ver": value = resultModel.Ver.ToExport(column); break;
                case "Title": value = resultModel.Title.ToExport(column); break;
                case "Body": value = resultModel.Body.ToExport(column); break;
                case "TitleBody": value = resultModel.TitleBody.ToExport(column); break;
                case "Status": value = resultModel.Status.ToExport(column); break;
                case "Manager": value = resultModel.Manager.ToExport(column); break;
                case "Owner": value = resultModel.Owner.ToExport(column); break;
                case "ClassA": value = resultModel.ClassA.ToExport(column); break;
                case "ClassB": value = resultModel.ClassB.ToExport(column); break;
                case "ClassC": value = resultModel.ClassC.ToExport(column); break;
                case "ClassD": value = resultModel.ClassD.ToExport(column); break;
                case "ClassE": value = resultModel.ClassE.ToExport(column); break;
                case "ClassF": value = resultModel.ClassF.ToExport(column); break;
                case "ClassG": value = resultModel.ClassG.ToExport(column); break;
                case "ClassH": value = resultModel.ClassH.ToExport(column); break;
                case "ClassI": value = resultModel.ClassI.ToExport(column); break;
                case "ClassJ": value = resultModel.ClassJ.ToExport(column); break;
                case "ClassK": value = resultModel.ClassK.ToExport(column); break;
                case "ClassL": value = resultModel.ClassL.ToExport(column); break;
                case "ClassM": value = resultModel.ClassM.ToExport(column); break;
                case "ClassN": value = resultModel.ClassN.ToExport(column); break;
                case "ClassO": value = resultModel.ClassO.ToExport(column); break;
                case "ClassP": value = resultModel.ClassP.ToExport(column); break;
                case "ClassQ": value = resultModel.ClassQ.ToExport(column); break;
                case "ClassR": value = resultModel.ClassR.ToExport(column); break;
                case "ClassS": value = resultModel.ClassS.ToExport(column); break;
                case "ClassT": value = resultModel.ClassT.ToExport(column); break;
                case "ClassU": value = resultModel.ClassU.ToExport(column); break;
                case "ClassV": value = resultModel.ClassV.ToExport(column); break;
                case "ClassW": value = resultModel.ClassW.ToExport(column); break;
                case "ClassX": value = resultModel.ClassX.ToExport(column); break;
                case "ClassY": value = resultModel.ClassY.ToExport(column); break;
                case "ClassZ": value = resultModel.ClassZ.ToExport(column); break;
                case "NumA": value = resultModel.NumA.ToExport(column); break;
                case "NumB": value = resultModel.NumB.ToExport(column); break;
                case "NumC": value = resultModel.NumC.ToExport(column); break;
                case "NumD": value = resultModel.NumD.ToExport(column); break;
                case "NumE": value = resultModel.NumE.ToExport(column); break;
                case "NumF": value = resultModel.NumF.ToExport(column); break;
                case "NumG": value = resultModel.NumG.ToExport(column); break;
                case "NumH": value = resultModel.NumH.ToExport(column); break;
                case "NumI": value = resultModel.NumI.ToExport(column); break;
                case "NumJ": value = resultModel.NumJ.ToExport(column); break;
                case "NumK": value = resultModel.NumK.ToExport(column); break;
                case "NumL": value = resultModel.NumL.ToExport(column); break;
                case "NumM": value = resultModel.NumM.ToExport(column); break;
                case "NumN": value = resultModel.NumN.ToExport(column); break;
                case "NumO": value = resultModel.NumO.ToExport(column); break;
                case "NumP": value = resultModel.NumP.ToExport(column); break;
                case "NumQ": value = resultModel.NumQ.ToExport(column); break;
                case "NumR": value = resultModel.NumR.ToExport(column); break;
                case "NumS": value = resultModel.NumS.ToExport(column); break;
                case "NumT": value = resultModel.NumT.ToExport(column); break;
                case "NumU": value = resultModel.NumU.ToExport(column); break;
                case "NumV": value = resultModel.NumV.ToExport(column); break;
                case "NumW": value = resultModel.NumW.ToExport(column); break;
                case "NumX": value = resultModel.NumX.ToExport(column); break;
                case "NumY": value = resultModel.NumY.ToExport(column); break;
                case "NumZ": value = resultModel.NumZ.ToExport(column); break;
                case "DateA": value = resultModel.DateA.ToExport(column); break;
                case "DateB": value = resultModel.DateB.ToExport(column); break;
                case "DateC": value = resultModel.DateC.ToExport(column); break;
                case "DateD": value = resultModel.DateD.ToExport(column); break;
                case "DateE": value = resultModel.DateE.ToExport(column); break;
                case "DateF": value = resultModel.DateF.ToExport(column); break;
                case "DateG": value = resultModel.DateG.ToExport(column); break;
                case "DateH": value = resultModel.DateH.ToExport(column); break;
                case "DateI": value = resultModel.DateI.ToExport(column); break;
                case "DateJ": value = resultModel.DateJ.ToExport(column); break;
                case "DateK": value = resultModel.DateK.ToExport(column); break;
                case "DateL": value = resultModel.DateL.ToExport(column); break;
                case "DateM": value = resultModel.DateM.ToExport(column); break;
                case "DateN": value = resultModel.DateN.ToExport(column); break;
                case "DateO": value = resultModel.DateO.ToExport(column); break;
                case "DateP": value = resultModel.DateP.ToExport(column); break;
                case "DateQ": value = resultModel.DateQ.ToExport(column); break;
                case "DateR": value = resultModel.DateR.ToExport(column); break;
                case "DateS": value = resultModel.DateS.ToExport(column); break;
                case "DateT": value = resultModel.DateT.ToExport(column); break;
                case "DateU": value = resultModel.DateU.ToExport(column); break;
                case "DateV": value = resultModel.DateV.ToExport(column); break;
                case "DateW": value = resultModel.DateW.ToExport(column); break;
                case "DateX": value = resultModel.DateX.ToExport(column); break;
                case "DateY": value = resultModel.DateY.ToExport(column); break;
                case "DateZ": value = resultModel.DateZ.ToExport(column); break;
                case "DescriptionA": value = resultModel.DescriptionA.ToExport(column); break;
                case "DescriptionB": value = resultModel.DescriptionB.ToExport(column); break;
                case "DescriptionC": value = resultModel.DescriptionC.ToExport(column); break;
                case "DescriptionD": value = resultModel.DescriptionD.ToExport(column); break;
                case "DescriptionE": value = resultModel.DescriptionE.ToExport(column); break;
                case "DescriptionF": value = resultModel.DescriptionF.ToExport(column); break;
                case "DescriptionG": value = resultModel.DescriptionG.ToExport(column); break;
                case "DescriptionH": value = resultModel.DescriptionH.ToExport(column); break;
                case "DescriptionI": value = resultModel.DescriptionI.ToExport(column); break;
                case "DescriptionJ": value = resultModel.DescriptionJ.ToExport(column); break;
                case "DescriptionK": value = resultModel.DescriptionK.ToExport(column); break;
                case "DescriptionL": value = resultModel.DescriptionL.ToExport(column); break;
                case "DescriptionM": value = resultModel.DescriptionM.ToExport(column); break;
                case "DescriptionN": value = resultModel.DescriptionN.ToExport(column); break;
                case "DescriptionO": value = resultModel.DescriptionO.ToExport(column); break;
                case "DescriptionP": value = resultModel.DescriptionP.ToExport(column); break;
                case "DescriptionQ": value = resultModel.DescriptionQ.ToExport(column); break;
                case "DescriptionR": value = resultModel.DescriptionR.ToExport(column); break;
                case "DescriptionS": value = resultModel.DescriptionS.ToExport(column); break;
                case "DescriptionT": value = resultModel.DescriptionT.ToExport(column); break;
                case "DescriptionU": value = resultModel.DescriptionU.ToExport(column); break;
                case "DescriptionV": value = resultModel.DescriptionV.ToExport(column); break;
                case "DescriptionW": value = resultModel.DescriptionW.ToExport(column); break;
                case "DescriptionX": value = resultModel.DescriptionX.ToExport(column); break;
                case "DescriptionY": value = resultModel.DescriptionY.ToExport(column); break;
                case "DescriptionZ": value = resultModel.DescriptionZ.ToExport(column); break;
                case "CheckA": value = resultModel.CheckA.ToExport(column); break;
                case "CheckB": value = resultModel.CheckB.ToExport(column); break;
                case "CheckC": value = resultModel.CheckC.ToExport(column); break;
                case "CheckD": value = resultModel.CheckD.ToExport(column); break;
                case "CheckE": value = resultModel.CheckE.ToExport(column); break;
                case "CheckF": value = resultModel.CheckF.ToExport(column); break;
                case "CheckG": value = resultModel.CheckG.ToExport(column); break;
                case "CheckH": value = resultModel.CheckH.ToExport(column); break;
                case "CheckI": value = resultModel.CheckI.ToExport(column); break;
                case "CheckJ": value = resultModel.CheckJ.ToExport(column); break;
                case "CheckK": value = resultModel.CheckK.ToExport(column); break;
                case "CheckL": value = resultModel.CheckL.ToExport(column); break;
                case "CheckM": value = resultModel.CheckM.ToExport(column); break;
                case "CheckN": value = resultModel.CheckN.ToExport(column); break;
                case "CheckO": value = resultModel.CheckO.ToExport(column); break;
                case "CheckP": value = resultModel.CheckP.ToExport(column); break;
                case "CheckQ": value = resultModel.CheckQ.ToExport(column); break;
                case "CheckR": value = resultModel.CheckR.ToExport(column); break;
                case "CheckS": value = resultModel.CheckS.ToExport(column); break;
                case "CheckT": value = resultModel.CheckT.ToExport(column); break;
                case "CheckU": value = resultModel.CheckU.ToExport(column); break;
                case "CheckV": value = resultModel.CheckV.ToExport(column); break;
                case "CheckW": value = resultModel.CheckW.ToExport(column); break;
                case "CheckX": value = resultModel.CheckX.ToExport(column); break;
                case "CheckY": value = resultModel.CheckY.ToExport(column); break;
                case "CheckZ": value = resultModel.CheckZ.ToExport(column); break;
                case "Comments": value = resultModel.Comments.ToExport(column); break;
                case "Creator": value = resultModel.Creator.ToExport(column); break;
                case "Updator": value = resultModel.Updator.ToExport(column); break;
                case "CreatedTime": value = resultModel.CreatedTime.ToExport(column); break;
                default: return string.Empty;
            }
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }

        public static string TitleDisplayValue(SiteSettings siteSettings, ResultModel resultModel)
        {
            var displayValue = siteSettings.TitleColumnCollection()
                .Select(column => TitleDisplayValue(column, resultModel))
                .Where(o => o != string.Empty)
                .Join(siteSettings.TitleSeparator);
            return displayValue != string.Empty
                ? displayValue
                : Displays.NoTitle();
        }

        private static string TitleDisplayValue(Column column, ResultModel resultModel)
        {
            switch (column.ColumnName)
            {
                case "Title": return column.HasChoices()
                    ? column.Choice(resultModel.Title.Value).Text
                    : resultModel.Title.Value;
                case "ClassA": return column.HasChoices()
                    ? column.Choice(resultModel.ClassA).Text
                    : resultModel.ClassA;
                case "ClassB": return column.HasChoices()
                    ? column.Choice(resultModel.ClassB).Text
                    : resultModel.ClassB;
                case "ClassC": return column.HasChoices()
                    ? column.Choice(resultModel.ClassC).Text
                    : resultModel.ClassC;
                case "ClassD": return column.HasChoices()
                    ? column.Choice(resultModel.ClassD).Text
                    : resultModel.ClassD;
                case "ClassE": return column.HasChoices()
                    ? column.Choice(resultModel.ClassE).Text
                    : resultModel.ClassE;
                case "ClassF": return column.HasChoices()
                    ? column.Choice(resultModel.ClassF).Text
                    : resultModel.ClassF;
                case "ClassG": return column.HasChoices()
                    ? column.Choice(resultModel.ClassG).Text
                    : resultModel.ClassG;
                case "ClassH": return column.HasChoices()
                    ? column.Choice(resultModel.ClassH).Text
                    : resultModel.ClassH;
                case "ClassI": return column.HasChoices()
                    ? column.Choice(resultModel.ClassI).Text
                    : resultModel.ClassI;
                case "ClassJ": return column.HasChoices()
                    ? column.Choice(resultModel.ClassJ).Text
                    : resultModel.ClassJ;
                case "ClassK": return column.HasChoices()
                    ? column.Choice(resultModel.ClassK).Text
                    : resultModel.ClassK;
                case "ClassL": return column.HasChoices()
                    ? column.Choice(resultModel.ClassL).Text
                    : resultModel.ClassL;
                case "ClassM": return column.HasChoices()
                    ? column.Choice(resultModel.ClassM).Text
                    : resultModel.ClassM;
                case "ClassN": return column.HasChoices()
                    ? column.Choice(resultModel.ClassN).Text
                    : resultModel.ClassN;
                case "ClassO": return column.HasChoices()
                    ? column.Choice(resultModel.ClassO).Text
                    : resultModel.ClassO;
                case "ClassP": return column.HasChoices()
                    ? column.Choice(resultModel.ClassP).Text
                    : resultModel.ClassP;
                case "ClassQ": return column.HasChoices()
                    ? column.Choice(resultModel.ClassQ).Text
                    : resultModel.ClassQ;
                case "ClassR": return column.HasChoices()
                    ? column.Choice(resultModel.ClassR).Text
                    : resultModel.ClassR;
                case "ClassS": return column.HasChoices()
                    ? column.Choice(resultModel.ClassS).Text
                    : resultModel.ClassS;
                case "ClassT": return column.HasChoices()
                    ? column.Choice(resultModel.ClassT).Text
                    : resultModel.ClassT;
                case "ClassU": return column.HasChoices()
                    ? column.Choice(resultModel.ClassU).Text
                    : resultModel.ClassU;
                case "ClassV": return column.HasChoices()
                    ? column.Choice(resultModel.ClassV).Text
                    : resultModel.ClassV;
                case "ClassW": return column.HasChoices()
                    ? column.Choice(resultModel.ClassW).Text
                    : resultModel.ClassW;
                case "ClassX": return column.HasChoices()
                    ? column.Choice(resultModel.ClassX).Text
                    : resultModel.ClassX;
                case "ClassY": return column.HasChoices()
                    ? column.Choice(resultModel.ClassY).Text
                    : resultModel.ClassY;
                case "ClassZ": return column.HasChoices()
                    ? column.Choice(resultModel.ClassZ).Text
                    : resultModel.ClassZ;
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
                case "ClassA": return column.HasChoices()
                    ? column.Choice(dataRow["ClassA"].ToString()).Text
                    : dataRow["ClassA"].ToString();
                case "ClassB": return column.HasChoices()
                    ? column.Choice(dataRow["ClassB"].ToString()).Text
                    : dataRow["ClassB"].ToString();
                case "ClassC": return column.HasChoices()
                    ? column.Choice(dataRow["ClassC"].ToString()).Text
                    : dataRow["ClassC"].ToString();
                case "ClassD": return column.HasChoices()
                    ? column.Choice(dataRow["ClassD"].ToString()).Text
                    : dataRow["ClassD"].ToString();
                case "ClassE": return column.HasChoices()
                    ? column.Choice(dataRow["ClassE"].ToString()).Text
                    : dataRow["ClassE"].ToString();
                case "ClassF": return column.HasChoices()
                    ? column.Choice(dataRow["ClassF"].ToString()).Text
                    : dataRow["ClassF"].ToString();
                case "ClassG": return column.HasChoices()
                    ? column.Choice(dataRow["ClassG"].ToString()).Text
                    : dataRow["ClassG"].ToString();
                case "ClassH": return column.HasChoices()
                    ? column.Choice(dataRow["ClassH"].ToString()).Text
                    : dataRow["ClassH"].ToString();
                case "ClassI": return column.HasChoices()
                    ? column.Choice(dataRow["ClassI"].ToString()).Text
                    : dataRow["ClassI"].ToString();
                case "ClassJ": return column.HasChoices()
                    ? column.Choice(dataRow["ClassJ"].ToString()).Text
                    : dataRow["ClassJ"].ToString();
                case "ClassK": return column.HasChoices()
                    ? column.Choice(dataRow["ClassK"].ToString()).Text
                    : dataRow["ClassK"].ToString();
                case "ClassL": return column.HasChoices()
                    ? column.Choice(dataRow["ClassL"].ToString()).Text
                    : dataRow["ClassL"].ToString();
                case "ClassM": return column.HasChoices()
                    ? column.Choice(dataRow["ClassM"].ToString()).Text
                    : dataRow["ClassM"].ToString();
                case "ClassN": return column.HasChoices()
                    ? column.Choice(dataRow["ClassN"].ToString()).Text
                    : dataRow["ClassN"].ToString();
                case "ClassO": return column.HasChoices()
                    ? column.Choice(dataRow["ClassO"].ToString()).Text
                    : dataRow["ClassO"].ToString();
                case "ClassP": return column.HasChoices()
                    ? column.Choice(dataRow["ClassP"].ToString()).Text
                    : dataRow["ClassP"].ToString();
                case "ClassQ": return column.HasChoices()
                    ? column.Choice(dataRow["ClassQ"].ToString()).Text
                    : dataRow["ClassQ"].ToString();
                case "ClassR": return column.HasChoices()
                    ? column.Choice(dataRow["ClassR"].ToString()).Text
                    : dataRow["ClassR"].ToString();
                case "ClassS": return column.HasChoices()
                    ? column.Choice(dataRow["ClassS"].ToString()).Text
                    : dataRow["ClassS"].ToString();
                case "ClassT": return column.HasChoices()
                    ? column.Choice(dataRow["ClassT"].ToString()).Text
                    : dataRow["ClassT"].ToString();
                case "ClassU": return column.HasChoices()
                    ? column.Choice(dataRow["ClassU"].ToString()).Text
                    : dataRow["ClassU"].ToString();
                case "ClassV": return column.HasChoices()
                    ? column.Choice(dataRow["ClassV"].ToString()).Text
                    : dataRow["ClassV"].ToString();
                case "ClassW": return column.HasChoices()
                    ? column.Choice(dataRow["ClassW"].ToString()).Text
                    : dataRow["ClassW"].ToString();
                case "ClassX": return column.HasChoices()
                    ? column.Choice(dataRow["ClassX"].ToString()).Text
                    : dataRow["ClassX"].ToString();
                case "ClassY": return column.HasChoices()
                    ? column.Choice(dataRow["ClassY"].ToString()).Text
                    : dataRow["ClassY"].ToString();
                case "ClassZ": return column.HasChoices()
                    ? column.Choice(dataRow["ClassZ"].ToString()).Text
                    : dataRow["ClassZ"].ToString();
                default: return string.Empty;
            }
        }

        public static string UpdateByKamban(SiteModel siteModel)
        {
            var siteSettings = siteModel.ResultsSiteSettings();
            var resultModel = new ResultModel(
                siteSettings, Forms.Long("KambanId"), setByForm: true);
            resultModel.VerUp = Versions.MustVerUp(resultModel);
            resultModel.Update();
            return KambanJson(siteSettings, siteModel.PermissionType);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string TimeSeries(SiteSettings siteSettings, Permissions.Types permissionType)
        {
            var hb = new HtmlBuilder();
            var formData = DataViewFilters.SessionFormData(siteSettings.SiteId);
            var resultCollection = ResultCollection(siteSettings, permissionType, formData);
            var dataViewName = DataViewSelectors.Get(siteSettings.SiteId);
            return hb.DataViewTemplate(
                siteSettings: siteSettings,
                permissionType: permissionType,
                resultCollection: resultCollection,
                formData: formData,
                dataViewName: dataViewName,
                dataViewBody: () => hb.TimeSeries(
                    siteSettings: siteSettings,
                    permissionType: permissionType,
                    formData: formData,
                    bodyOnly: false));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string TimeSeriesJson(
            SiteSettings siteSettings, Permissions.Types permissionType)
        {
            var formData = DataViewFilters.SessionFormData(siteSettings.SiteId);
            var resultCollection = ResultCollection(siteSettings, permissionType, formData);
            var bodyOnly = Forms.ControlId().StartsWith("TimeSeries");
            return new ResponseCollection()
                .Html(
                    !bodyOnly ? "#DataViewContainer" : "#TimeSeriesBody",
                    new HtmlBuilder().TimeSeries(
                        siteSettings: siteSettings,
                        permissionType: permissionType,
                        formData: formData,
                        bodyOnly: bodyOnly))
                .DataViewFilters(siteSettings: siteSettings)
                .ReplaceAll(
                    "#Aggregations", new HtmlBuilder().Aggregations(
                    siteSettings: siteSettings,
                    aggregations: resultCollection.Aggregations))
                .Invoke("drawTimeSeries")
                .WindowScrollTop().ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder TimeSeries(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData,
            bool bodyOnly)
        {
            var groupByColumn = formData.Keys.Contains("TimeSeriesGroupByColumn")
                ? formData["TimeSeriesGroupByColumn"].Value
                : "Owner";
            var aggregateType = formData.Keys.Contains("TimeSeriesAggregateType")
                ? formData["TimeSeriesAggregateType"].Value
                : "Count";
            var valueColumn = formData.Keys.Contains("TimeSeriesValueColumn")
                ? formData["TimeSeriesValueColumn"].Value
                : "NumA";
            var dataRows = TimeSeriesDataRows(
                siteSettings: siteSettings,
                formData: formData,
                groupByColumn: groupByColumn,
                valueColumn: valueColumn);
            return !bodyOnly
                ? hb.TimeSeries(
                    siteSettings: siteSettings,
                    groupByColumn: groupByColumn,
                    aggregateType: aggregateType,
                    valueColumn: valueColumn,
                    permissionType: permissionType,
                    dataRows: dataRows)
                : hb.TimeSeriesBody(
                    siteSettings: siteSettings,
                    groupByColumn: groupByColumn,
                    aggregateType: aggregateType,
                    valueColumn: valueColumn,
                    dataRows: dataRows);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static EnumerableRowCollection<DataRow> TimeSeriesDataRows(
            SiteSettings siteSettings, FormData formData, string groupByColumn, string valueColumn)
        {
            return groupByColumn != string.Empty && valueColumn != string.Empty
                ? Rds.ExecuteTable(statements:
                    Rds.SelectResults(
                        tableType: Sqls.TableTypes.NormalAndHistory,
                        column: Rds.ResultsColumn()
                            .ResultId(_as: "Id")
                            .Ver()
                            .UpdatedTime()
                            .ResultsColumn(groupByColumn, _as: "Index")
                            .ResultsColumn(valueColumn, _as: "Value"),
                        where: DataViewFilters.Get(
                            siteSettings,
                            "Results",
                            formData,
                            Rds.ResultsWhere().SiteId(siteSettings.SiteId))))
                                .AsEnumerable()
                : null;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Kamban(SiteSettings siteSettings, Permissions.Types permissionType)
        {
            var hb = new HtmlBuilder();
            var formData = DataViewFilters.SessionFormData(siteSettings.SiteId);
            var resultCollection = ResultCollection(siteSettings, permissionType, formData);
            var dataViewName = DataViewSelectors.Get(siteSettings.SiteId);
            return hb.DataViewTemplate(
                siteSettings: siteSettings,
                permissionType: permissionType,
                resultCollection: resultCollection,
                formData: formData,
                dataViewName: dataViewName,
                dataViewBody: () => hb.Kamban(
                    siteSettings: siteSettings,
                    permissionType: permissionType,
                    formData: formData,
                    bodyOnly: false));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string KambanJson(
            SiteSettings siteSettings, Permissions.Types permissionType)
        {
            var formData = DataViewFilters.SessionFormData(siteSettings.SiteId);
            var resultCollection = ResultCollection(siteSettings, permissionType, formData);
            var bodyOnly = Forms.ControlId().StartsWith("Kamban");
            return new ResponseCollection()
                .Html(
                    !bodyOnly ? "#DataViewContainer" : "#KambanBody",
                    new HtmlBuilder().Kamban(
                        siteSettings: siteSettings,
                        permissionType: permissionType,
                        formData: formData,
                        bodyOnly: bodyOnly,
                        changedItemId: Forms.Long("KambanId")))
                .DataViewFilters(siteSettings: siteSettings)
                .ReplaceAll(
                    "#Aggregations", new HtmlBuilder().Aggregations(
                    siteSettings: siteSettings,
                    aggregations: resultCollection.Aggregations))
                .ClearFormData()
                .Invoke("setKamban").ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Kamban(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData,
            bool bodyOnly,
            long changedItemId = 0)
        {
            var groupByColumn = formData.Keys.Contains("KambanGroupByColumn")
                ? formData["KambanGroupByColumn"].Value
                : "Status";
            var aggregateType = formData.Keys.Contains("KambanAggregateType")
                ? formData["KambanAggregateType"].Value
                : "Total";
            var valueColumn = formData.Keys.Contains("KambanValueColumn")
                ? formData["KambanValueColumn"].Value
                : KambanValueColumn(siteSettings);
            var column = Rds.ResultsColumn()
                .ResultId()
                .Manager()
                .Owner();
            siteSettings.TitleColumnCollection().ForEach(titleColumn =>
                column.ResultsColumn(titleColumn.ColumnName));
            column.ResultsColumn(groupByColumn);
            column.ResultsColumn(valueColumn);
            var data = new ResultCollection(
                siteSettings: siteSettings,
                permissionType: permissionType,
                column: column,
                where: DataViewFilters.Get(
                    siteSettings: siteSettings,
                    tableName: "Results",
                    formData: formData,
                    where: Rds.ResultsWhere().SiteId(siteSettings.SiteId)),
                orderBy: GridSorters.Get(
                    formData, Rds.IssuesOrderBy().UpdatedTime(SqlOrderBy.Types.desc)))
                        .Select(o => new Libraries.DataViews.KambanElement()
                        {
                            Id = o.Id,
                            Title = o.Title.DisplayValue,
                            Manager = o.Manager,
                            Owner = o.Owner,
                            Group = o.PropertyValue(groupByColumn),
                            Value = o.PropertyValue(valueColumn).ToDecimal()
                        });
            return !bodyOnly
                ? hb.Kamban(
                    siteSettings: siteSettings,
                    groupByColumn: groupByColumn,
                    aggregateType: aggregateType,
                    valueColumn: valueColumn,
                    permissionType: permissionType,
                    data: data)
                : hb.KambanBody(
                    siteSettings: siteSettings,
                    groupByColumn: siteSettings.GetColumn(groupByColumn),
                    aggregateType: aggregateType,
                    valueColumn: siteSettings.GetColumn(valueColumn),
                    data: data,
                    changedItemId: changedItemId);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string KambanValueColumn(SiteSettings siteSettings)
        {
            var column = siteSettings.EditorColumnCollection()
                .Where(o => o.Computable)
                .Where(o => o.TypeName != "datetime")
                .FirstOrDefault();
            return column != null
                ? column.ColumnName
                : string.Empty;
        }
    }
}
