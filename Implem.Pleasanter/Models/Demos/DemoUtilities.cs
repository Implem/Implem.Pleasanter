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
    public static class DemoUtilities
    {
        private static string DataViewTemplate(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            DemoCollection demoCollection,
            FormData formData,
            string dataViewName,
            Action dataViewBody)
        {
            return hb.Template(
                siteId: siteSettings.SiteId,
                referenceType: "Demos",
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
                            .Id("DemosForm")
                            .Class("main-form")
                            .Action(Navigations.ItemAction(siteSettings.SiteId)),
                        action: () => hb
                            .DataViewSelector(
                                referenceType: "Demos",
                                dataViewName: dataViewName)
                            .DataViewFilters(
                                siteSettings: siteSettings,
                                siteId: siteSettings.SiteId)
                            .Aggregations(
                                siteSettings: siteSettings,
                                aggregations: demoCollection.Aggregations)
                            .Div(id: "DataViewContainer", action: () => dataViewBody())
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
                            .Hidden(controlId: "TableName", value: "Demos")
                            .Hidden(controlId: "BaseUrl", value: Navigations.BaseUrl()))
                .MoveDialog("items", siteSettings.SiteId, bulk: true)
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
            var demoCollection = DemoCollection(siteSettings, permissionType, formData);
            var dataViewName = DataViewSelectors.Get(siteSettings.SiteId);
            return hb.DataViewTemplate(
                siteSettings: siteSettings,
                permissionType: permissionType,
                demoCollection: demoCollection,
                formData: formData,
                dataViewName: dataViewName,
                dataViewBody: () => hb.Grid(
                   demoCollection: demoCollection,
                   siteSettings: siteSettings,
                   permissionType: permissionType,
                   formData: formData));
        }

        public static string IndexJson(SiteSettings siteSettings, Permissions.Types permissionType)
        {
            var formData = DataViewFilters.SessionFormData();
            var demoCollection = DemoCollection(siteSettings, permissionType, formData);
            return new ResponseCollection()
                .Html("#DataViewContainer", new HtmlBuilder().Grid(
                    siteSettings: siteSettings,
                    demoCollection: demoCollection,
                    permissionType: permissionType,
                    formData: formData))
                .Html("#Aggregations", new HtmlBuilder().Aggregations(
                    siteSettings: siteSettings,
                    aggregations: demoCollection.Aggregations,
                    container: false))
                .WindowScrollTop().ToJson();
        }

        private static DemoCollection DemoCollection(
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData, int offset = 0)
        {
            return new DemoCollection(
                siteSettings: siteSettings,
                permissionType: permissionType,
                column: GridSqlColumnCollection(siteSettings),
                where: DataViewFilters.Get(
                    siteSettings: siteSettings,
                    tableName: "Demos",
                    formData: formData,
                    where: Rds.DemosWhere().TenantId(Sessions.TenantId())),
                orderBy: GridSorters.Get(
                    formData, Rds.DemosOrderBy().UpdatedTime(SqlOrderBy.Types.desc)),
                offset: offset,
                pageSize: siteSettings.GridPageSize.ToInt(),
                countRecord: true,
                aggregationCollection: siteSettings.AggregationCollection);
        }

        public static HtmlBuilder DataView(
            this HtmlBuilder hb,
            DemoCollection demoCollection,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData,
            string dataViewName)
        {
            switch (dataViewName)
            {
                default: return hb.Grid(
                    demoCollection: demoCollection,
                    siteSettings: siteSettings,
                    permissionType: permissionType,
                    formData: formData);
            }
        }

        private static HtmlBuilder Grid(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            DemoCollection demoCollection,
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
                            demoCollection: demoCollection,
                            formData: formData))
                .Hidden(
                    controlId: "GridOffset",
                    value: siteSettings.GridPageSize == demoCollection.Count()
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
            var demoCollection = DemoCollection(siteSettings, permissionType, formData, offset);
            return (responseCollection ?? new ResponseCollection())
                .Remove(".grid tr", _using: offset == 0)
                .ClearFormData("GridCheckAll", _using: clearCheck)
                .ClearFormData("GridUnCheckedItems", _using: clearCheck)
                .ClearFormData("GridCheckedItems", _using: clearCheck)
                .Message(message)
                .Append("#Grid", new HtmlBuilder().GridRows(
                    siteSettings: siteSettings,
                    demoCollection: demoCollection,
                    formData: formData,
                    addHeader: offset == 0,
                    clearCheck: clearCheck))
                .Html("#Aggregations", new HtmlBuilder().Aggregations(
                    siteSettings: siteSettings,
                    aggregations: demoCollection.Aggregations,
                    container: false))
                .Val("#GridOffset", siteSettings.NextPageOffset(offset, demoCollection.Count()))
                .ToJson();
        }

        private static HtmlBuilder GridRows(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            DemoCollection demoCollection,
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
                .TBody(action: () => demoCollection
                    .ForEach(demoModel => hb
                        .Tr(
                            attributes: new HtmlAttributes()
                                .Class("grid-row")
                                .DataId(demoModel.DemoId.ToString()),
                            action: () =>
                            {
                                hb.Td(action: () => hb
                                    .CheckBox(
                                        controlCss: "grid-check",
                                        _checked: checkAll,
                                        dataId: demoModel.DemoId.ToString()));
                                siteSettings.GridColumnCollection()
                                    .ForEach(column => hb
                                        .TdValue(
                                            column: column,
                                            demoModel: demoModel));
                            })));
        }

        private static SqlColumnCollection GridSqlColumnCollection(SiteSettings siteSettings)
        {
            var gridSqlColumn = Rds.DemosColumn()
                .DemoId()
                .Creator()
                .Updator();
            siteSettings.GridColumnCollection(withTitle: true).ForEach(column =>
                Rds.DemosColumn(gridSqlColumn, column.ColumnName));
            return gridSqlColumn;
        }

        public static HtmlBuilder TdValue(
            this HtmlBuilder hb, Column column, DemoModel demoModel)
        {
            switch (column.ColumnName)
            {
                case "Ver": return hb.Td(column: column, value: demoModel.Ver);
                case "Comments": return hb.Td(column: column, value: demoModel.Comments);
                case "Creator": return hb.Td(column: column, value: demoModel.Creator);
                case "Updator": return hb.Td(column: column, value: demoModel.Updator);
                case "CreatedTime": return hb.Td(column: column, value: demoModel.CreatedTime);
                case "UpdatedTime": return hb.Td(column: column, value: demoModel.UpdatedTime);
                default: return hb;
            }
        }

        public static string EditorNew()
        {
            return Editor(
                new DemoModel(
                    SiteSettingsUtility.DemosSiteSettings(),
                    Permissions.Admins(),
                    methodType: BaseModel.MethodTypes.New));
        }

        public static string Editor(int demoId, bool clearSessions)
        {
            var demoModel = new DemoModel(
                    SiteSettingsUtility.DemosSiteSettings(),
                    Permissions.Admins(),
                demoId: demoId,
                clearSessions: clearSessions,
                methodType: BaseModel.MethodTypes.Edit);
            demoModel.SwitchTargets = GetSwitchTargets(
                SiteSettingsUtility.DemosSiteSettings());
            return Editor(demoModel);
        }

        public static string Editor(DemoModel demoModel)
        {
            var hb = new HtmlBuilder();
            var permissionType = Permissions.Admins();
            return hb.Template(
                siteId: 0,
                referenceType: "Demos",
                title: demoModel.MethodType == BaseModel.MethodTypes.New
                    ? Displays.Demos() + " - " + Displays.New()
                    : demoModel.Title.Value,
                permissionType: permissionType,
                verType: demoModel.VerType,
                methodType: demoModel.MethodType,
                allowAccess:
                    permissionType.CanEditTenant() &&
                    demoModel.AccessStatus != Databases.AccessStatuses.NotFound,
                action: () =>
                {
                    hb
                        .Editor(
                            demoModel: demoModel,
                            permissionType: permissionType,
                            siteSettings: demoModel.SiteSettings)
                        .Hidden(controlId: "TableName", value: "Demos")
                        .Hidden(controlId: "Id", value: demoModel.DemoId.ToString());
                }).ToString();
        }

        private static HtmlBuilder Editor(
            this HtmlBuilder hb,
            DemoModel demoModel,
            Permissions.Types permissionType,
            SiteSettings siteSettings)
        {
            return hb.Div(css: "edit-form", action: () => hb
                .Form(
                    attributes: new HtmlAttributes()
                        .Id("DemoForm")
                        .Class("main-form")
                        .Action(demoModel.DemoId != 0
                            ? Navigations.Action("Demos", demoModel.DemoId)
                            : Navigations.Action("Demos")),
                    action: () => hb
                        .RecordHeader(
                            baseModel: demoModel,
                            tableName: "Demos")
                        .Div(css: "edit-form-comments", action: () => hb
                            .Comments(
                                comments: demoModel.Comments,
                                verType: demoModel.VerType))
                        .Div(css: "edit-form-tabs", action: () => hb
                            .FieldTabs(demoModel: demoModel)
                            .FieldSetGeneral(
                                siteSettings: siteSettings,
                                permissionType: permissionType,
                                demoModel: demoModel)
                            .FieldSet(
                                attributes: new HtmlAttributes()
                                    .Id("FieldSetHistories")
                                    .DataAction("Histories")
                                    .DataMethod("get"),
                                _using: demoModel.MethodType != BaseModel.MethodTypes.New)
                            .MainCommands(
                                siteId: 0,
                                permissionType: permissionType,
                                verType: demoModel.VerType,
                                backUrl: Navigations.Index("Demos"),
                                referenceType: "Demos",
                                referenceId: demoModel.DemoId,
                                updateButton: true,
                                mailButton: true,
                                deleteButton: true,
                                extensions: () => hb
                                    .MainCommandExtensions(
                                        demoModel: demoModel,
                                        siteSettings: siteSettings)))
                        .Hidden(controlId: "BaseUrl", value: Navigations.BaseUrl())
                        .Hidden(
                            controlId: "MethodType",
                            value: demoModel.MethodType.ToString().ToLower())
                        .Hidden(
                            controlId: "Demos_Timestamp",
                            css: "must-transport",
                            value: demoModel.Timestamp)
                        .Hidden(
                            controlId: "SwitchTargets",
                            css: "must-transport",
                            value: demoModel.SwitchTargets?.Join(),
                            _using: !Request.IsAjax()))
                .OutgoingMailsForm("Demos", demoModel.DemoId, demoModel.Ver)
                .CopyDialog("Demos", demoModel.DemoId)
                .OutgoingMailDialog()
                .EditorExtensions(demoModel: demoModel, siteSettings: siteSettings));
        }

        private static HtmlBuilder FieldTabs(this HtmlBuilder hb, DemoModel demoModel)
        {
            return hb.Ul(css: "field-tab", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetGeneral", 
                        text: Displays.Basic()))
                .Li(
                    _using: demoModel.MethodType != BaseModel.MethodTypes.New,
                    action: () => hb
                        .A(
                            href: "#FieldSetHistories",
                            text: Displays.Histories())));
        }

        private static HtmlBuilder FieldSetGeneral(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            DemoModel demoModel)
        {
            return hb.FieldSet(id: "FieldSetGeneral", action: () =>
            {
                siteSettings.EditorColumnCollection().ForEach(column =>
                {
                    switch (column.ColumnName)
                    {
                        case "Ver": hb.Field(siteSettings, column, demoModel.MethodType, demoModel.Ver.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                    }
                });
                hb.VerUpCheckBox(demoModel);
            });
        }

        private static HtmlBuilder MainCommandExtensions(
            this HtmlBuilder hb,
            DemoModel demoModel,
            SiteSettings siteSettings)
        {
            return hb;
        }

        private static HtmlBuilder EditorExtensions(
            this HtmlBuilder hb,
            DemoModel demoModel,
            SiteSettings siteSettings)
        {
            return hb;
        }

        public static List<int> GetSwitchTargets(SiteSettings siteSettings)
        {
            var switchTargets = Forms.Data("SwitchTargets").Split(',')
                .Select(o => o.ToInt())
                .Where(o => o != 0)
                .ToList();
            if (switchTargets.Count() == 0)
            {
                var formData = DataViewFilters.SessionFormData();
                switchTargets = Rds.ExecuteTable(
                    transactional: false,
                    statements: Rds.SelectDemos(
                        column: Rds.DemosColumn().DemoId(),
                        where: DataViewFilters.Get(
                            siteSettings: siteSettings,
                            tableName: "Demos",
                            formData: formData,
                            where: Rds.DemosWhere().TenantId(Sessions.TenantId())),
                        orderBy: GridSorters.Get(
                            formData, Rds.DemosOrderBy().UpdatedTime(SqlOrderBy.Types.desc))))
                                .AsEnumerable()
                                .Select(o => o["DemoId"].ToInt())
                                .ToList();    
            }
            return switchTargets;
        }

        public static ResponseCollection FormResponse(
            this ResponseCollection responseCollection, DemoModel demoModel)
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
        public static string Register()
        {
            var passphrase = Strings.NewGuid();
            var mailAddress = Forms.Data("Users_DemoMailAddress");
            var tenantModel = new TenantModel()
            {
                SiteSettings = SiteSettingsUtility.TenantsSiteSettings(),
                PermissionType = Permissions.Types.ServiceAdmin,
                TenantName = mailAddress
            };
            tenantModel.Create();
            var demoModel = new DemoModel()
            {
                SiteSettings = SiteSettingsUtility.DemosSiteSettings(),
                PermissionType = Permissions.Types.ServiceAdmin,
                TenantId = tenantModel.TenantId,
                Passphrase = passphrase,
                MailAddress = mailAddress
            };
            demoModel.Create();
            var outgoingMailModel = new OutgoingMailModel()
            {
                SiteSettings = SiteSettingsUtility.OutgoingMailsSiteSettings(),
                PermissionType = Permissions.Types.Manager,
                Title = new Title(Displays.DemoMailTitle()),
                Body = Displays.DemoMailBody(Url.Server(), passphrase),
                From = new System.Net.Mail.MailAddress(Parameters.Mail.SupportFrom),
                To = mailAddress,
                Bcc = Parameters.Mail.SupportFrom
            };
            outgoingMailModel.Send();
            return Messages.ResponseSentAcceptanceMail()
                .Remove("#DemoForm")
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static bool Login()
        {
            var demoModel = new DemoModel().Get(where: Rds.DemosWhere()
                .Passphrase(QueryStrings.Data("passphrase"))
                .CreatedTime(
                    DateTime.Now.AddDays(Parameters.Service.DemoUsagePeriod * -1),
                    _operator: ">="));
            if (demoModel.AccessStatus == Databases.AccessStatuses.Selected)
            {
                System.Web.HttpContext.Current.Session["TenantId"] = demoModel.TenantId;
                demoModel.Initialize();
                return Sessions.LoggedIn();
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void Initialize(this DemoModel demoModel)
        {
            var idHash = new Dictionary<string, long>();
            var loginId = LoginId(demoModel, "User1");
            var password = Strings.NewGuid().Sha512Cng();
            if (demoModel.Initialized)
            {
                Rds.ExecuteNonQuery(statements: Rds.UpdateUsers(
                    param: Rds.UsersParam().Password(password),
                    where: Rds.UsersWhere().LoginId(loginId)));
            }
            else
            {
                demoModel.InitializeTimeLag();
                InitializeDepts(demoModel, idHash);
                InitializeUsers(demoModel, idHash, password);
                InitializeSites(demoModel, idHash);
                InitializeIssues(demoModel, idHash);
                InitializeResults(demoModel, idHash);
                InitializeLinks(demoModel, idHash);
                InitializePermissions(idHash);
                Rds.ExecuteNonQuery(statements: Rds.UpdateDemos(
                    param: Rds.DemosParam().Initialized(true),
                    where: Rds.DemosWhere().Passphrase(demoModel.Passphrase)));
                Libraries.Migrators.SiteSettingsMigrator.Migrate();
            }
            var userModel = new UserModel()
            {
                LoginId = loginId,
                Password = password
            }.Authenticate(string.Empty);
            SetSites(idHash);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void InitializeDepts(DemoModel demoModel, Dictionary<string, long> idHash)
        {
            Def.DemoDefinitionCollection
                .Where(o => o.Type == "Depts")
                .ForEach(demoDefinition =>
                    idHash.Add(demoDefinition.Id, Rds.ExecuteScalar_long(statements:
                        Rds.InsertDepts(
                            selectIdentity: true,
                            param: Rds.DeptsParam()
                                .TenantId(demoModel.TenantId)
                                .DeptCode(demoDefinition.ClassA)
                                .DeptName(demoDefinition.Title)
                                .CreatedTime(demoDefinition.CreatedTime.DemoTime(demoModel))
                                .UpdatedTime(demoDefinition.UpdatedTime.DemoTime(demoModel))))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void InitializeUsers(
            DemoModel demoModel, Dictionary<string, long> idHash, string password)
        {
            Def.DemoDefinitionCollection
                .Where(o => o.Type == "Users")
                .ForEach(demoDefinition =>
                {
                    var loginId = LoginId(demoModel, demoDefinition.Id);
                    idHash.Add(demoDefinition.Id, Rds.ExecuteScalar_long(statements: new SqlStatement[]
                    {
                        Rds.InsertUsers(
                            selectIdentity: true,
                            param: Rds.UsersParam()
                                .TenantId(demoModel.TenantId)
                                .LoginId(loginId)
                                .Password(password)
                                .LastName(demoDefinition.Title.Split_1st(' '))
                                .FirstName(demoDefinition.Title.Split_2nd(' '))
                                .DeptId(idHash[demoDefinition.ParentId].ToInt())
                                .FirstAndLastNameOrder(demoDefinition.ClassA == "1"
                                    ? Names.FirstAndLastNameOrders.FirstNameIsFirst
                                    : Names.FirstAndLastNameOrders.LastNameIsFirst)
                                .Birthday(demoDefinition.ClassC.ToDateTime())
                                .Sex(demoDefinition.ClassB)
                                .CreatedTime(demoDefinition.CreatedTime.DemoTime(demoModel))
                                .UpdatedTime(demoDefinition.UpdatedTime.DemoTime(demoModel))),
                        Rds.InsertMailAddresses(
                            param: Rds.MailAddressesParam()
                                .OwnerId(raw: Def.Sql.Identity)
                                .OwnerType("Users")
                                .MailAddress(loginId + "@example.com"))
                    }));
                });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void InitializeSites(DemoModel demoModel, Dictionary<string, long> idHash)
        {
            var topId = Def.DemoDefinitionCollection.First(o =>
                o.Type == "Sites" && o.ParentId == string.Empty).Id;
            Def.DemoDefinitionCollection
                .Where(o => o.Type == "Sites")
                .ForEach(demoDefinition =>
                    idHash.Add(demoDefinition.Id, Rds.ExecuteScalar_long(statements:
                        new SqlStatement[]
                        {
                            Rds.InsertItems(
                                selectIdentity: true,
                                param: Rds.ItemsParam()
                                    .ReferenceType("Sites")
                                    .Creator(idHash[demoDefinition.Creator])
                                    .Updator(idHash[demoDefinition.Updator])
                                    .CreatedTime(demoDefinition.CreatedTime.DemoTime(demoModel))
                                    .UpdatedTime(demoDefinition.UpdatedTime.DemoTime(demoModel)),
                                addUpdatorParam: false),
                            Rds.InsertSites(
                                selectIdentity: true,
                                param: Rds.SitesParam()
                                    .TenantId(demoModel.TenantId)
                                    .SiteId(raw: Def.Sql.Identity)
                                    .Title(demoDefinition.Title)
                                    .ReferenceType(demoDefinition.ClassA)
                                    .ParentId(idHash.ContainsKey(demoDefinition.ParentId)
                                        ? idHash[demoDefinition.ParentId]
                                        : 0)
                                    .InheritPermission(idHash, topId, demoDefinition.ParentId)
                                    .SiteSettings(demoDefinition.Body.Replace(idHash))
                                    .Creator(idHash[demoDefinition.Creator])
                                    .Updator(idHash[demoDefinition.Updator])
                                    .CreatedTime(demoDefinition.CreatedTime.DemoTime(demoModel))
                                    .UpdatedTime(demoDefinition.UpdatedTime.DemoTime(demoModel)),
                                addUpdatorParam: false)
                        })));
            new SiteCollection(where: Rds.SitesWhere().TenantId(demoModel.TenantId))
                .ForEach(siteModel =>
                {
                    Rds.ExecuteNonQuery(statements: Rds.UpdateItems(
                        param: Rds.ItemsParam()
                            .SiteId(siteModel.SiteId)
                            .Title(siteModel.Title.DisplayValue)
                            .Subset(Jsons.ToJson(new SiteSubset(
                                siteModel, siteModel.SiteSettings))),
                        where: Rds.ItemsWhere().ReferenceId(siteModel.SiteId),
                        addUpdatorParam: false,
                        addUpdatedTimeParam: false));
                    SiteInfo.SiteMenu.Set(siteModel.SiteId);
                });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static Rds.SitesParamCollection InheritPermission(
             this Rds.SitesParamCollection self,
             Dictionary<string, long> idHash,
             string topId,
             string parentId)
        {
            if (parentId == string.Empty)
            {
                return self.InheritPermission(raw: Def.Sql.Identity);
            }
            else
            {
                return self.InheritPermission(idHash[topId]);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void InitializeIssues(DemoModel demoModel, Dictionary<string, long> idHash)
        {
            Def.DemoDefinitionCollection
                .Where(o => o.Type == "Issues")
                .ForEach(demoDefinition =>
                {
                    var issueId = Rds.ExecuteScalar_long(statements: new SqlStatement[]
                    {
                        Rds.InsertItems(
                            selectIdentity: true,
                            param: Rds.ItemsParam()
                                .ReferenceType("Issues")
                                .Creator(idHash[demoDefinition.Creator])
                                .Updator(idHash[demoDefinition.Updator])
                                .CreatedTime(demoDefinition.CreatedTime.DemoTime(demoModel))
                                .UpdatedTime(demoDefinition.CreatedTime.DemoTime(demoModel)),
                            addUpdatorParam: false),
                        Rds.InsertIssues(
                            selectIdentity: true,
                            param: Rds.IssuesParam()
                                .SiteId(idHash[demoDefinition.ParentId])
                                .IssueId(raw: Def.Sql.Identity)
                                .Title(demoDefinition.Title)
                                .Body(demoDefinition.Body.Replace(idHash))
                                .StartTime(demoDefinition.StartTime.DemoTime(demoModel))
                                .CompletionTime(demoDefinition.CompletionTime
                                    .AddDays(1).DemoTime(demoModel))
                                .WorkValue(demoDefinition.WorkValue)
                                .ProgressRate(0)
                                .Status(demoDefinition.Status)
                                .Manager(idHash[demoDefinition.Manager])
                                .Owner(idHash[demoDefinition.Owner])
                                .ClassA(demoDefinition.ClassA.Replace(idHash))
                                .ClassB(demoDefinition.ClassB.Replace(idHash))
                                .ClassC(demoDefinition.ClassC.Replace(idHash))
                                .Comments(Comments(demoModel, idHash, demoDefinition.Id))
                                .Creator(idHash[demoDefinition.Creator])
                                .Updator(idHash[demoDefinition.Updator])
                                .CreatedTime(demoDefinition.CreatedTime.DemoTime(demoModel))
                                .UpdatedTime(demoDefinition.CreatedTime.DemoTime(demoModel)),
                            addUpdatorParam: false)
                    });
                    idHash.Add(demoDefinition.Id, issueId);
                    var siteModel = new SiteModel(idHash[demoDefinition.ParentId]);
                    var siteSettings = siteModel.IssuesSiteSettings();
                    var issueModel = new IssueModel(
                        siteSettings, Permissions.Types.Manager, issueId);
                    Rds.ExecuteNonQuery(statements:
                        Rds.UpdateItems(
                            param: Rds.ItemsParam()
                                .SiteId(issueModel.SiteId)
                                .Title(issueModel.Title.DisplayValue)
                                .Subset(Jsons.ToJson(new IssueSubset(
                                    issueModel, siteSettings))),
                            where: Rds.ItemsWhere().ReferenceId(issueModel.IssueId),
                            addUpdatorParam: false,
                            addUpdatedTimeParam: false));
                    var days = issueModel.CompletionTime.Value < DateTime.Now
                        ? (issueModel.CompletionTime.Value - issueModel.StartTime).Days
                        : (DateTime.Now - issueModel.StartTime).Days;
                    if (demoDefinition.ProgressRate > 0)
                    {
                        var startTime = issueModel.StartTime;
                        var progressRate = demoDefinition.ProgressRate;
                        var status = issueModel.Status.Value;
                        var creator = issueModel.Creator.Id;
                        var updator = issueModel.Updator.Id;
                        for (var d = 0; d < days -1; d++)
                        {
                            issueModel.VerUp = true;
                            issueModel.Update();
                            var recordingTime = d > 0
                                ? startTime
                                    .AddDays(d)
                                    .AddHours(-6)
                                    .AddMinutes(new Random().Next(-360, +360))
                                : issueModel.CreatedTime.Value;
                            Rds.ExecuteNonQuery(statements:
                                Rds.UpdateIssues(
                                    tableType: Sqls.TableTypes.History,
                                    addUpdatedTimeParam: false,
                                    addUpdatorParam: false,
                                    param: Rds.IssuesParam()
                                        .ProgressRate(ProgressRate(progressRate, days, d))
                                        .Status(d > 0 ? 200 : 100)
                                        .Creator(creator)
                                        .Updator(updator)
                                        .CreatedTime(recordingTime)
                                        .UpdatedTime(recordingTime),
                                    where: Rds.IssuesWhere()
                                        .IssueId(issueModel.IssueId)
                                        .Ver(sub: Rds.SelectIssues(
                                            tableType: Sqls.TableTypes.HistoryWithoutFlag,
                                            column: Rds.IssuesColumn().Add("max(Ver)"),
                                            where: Rds.IssuesWhere()
                                                .IssueId(issueModel.IssueId)))));
                        }
                        Rds.ExecuteNonQuery(statements:
                            Rds.UpdateIssues(
                                addUpdatorParam: false,
                                addUpdatedTimeParam: false,
                                param: Rds.IssuesParam()
                                    .ProgressRate(progressRate)
                                    .Status(status)
                                    .Creator(creator)
                                    .Updator(updator)
                                    .CreatedTime(demoDefinition.CreatedTime.DemoTime(demoModel))
                                    .UpdatedTime(demoDefinition.UpdatedTime.DemoTime(demoModel)),
                                where: Rds.IssuesWhere()
                                    .IssueId(issueModel.IssueId)));
                    }
                });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static decimal ProgressRate(decimal progressRate, int days, int d)
        {
            return d == 0
                ? 0
                : progressRate / days * (d + (new Random().NextDouble() - 0.4).ToDecimal());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void InitializeResults(DemoModel demoModel, Dictionary<string, long> idHash)
        {
            Def.DemoDefinitionCollection
                .Where(o => o.Type == "Results")
                .ForEach(demoDefinition =>
                {
                    var resultId = Rds.ExecuteScalar_long(statements: new SqlStatement[]
                    {
                        Rds.InsertItems(
                            selectIdentity: true,
                            param: Rds.ItemsParam()
                                .ReferenceType("Results")
                                .Creator(idHash[demoDefinition.Creator])
                                .Updator(idHash[demoDefinition.Updator])
                                .CreatedTime(demoDefinition.CreatedTime.DemoTime(demoModel))
                                .UpdatedTime(demoDefinition.UpdatedTime.DemoTime(demoModel)),
                            addUpdatorParam: false),
                        Rds.InsertResults(
                            selectIdentity: true,
                            param: Rds.ResultsParam()
                                .SiteId(idHash[demoDefinition.ParentId])
                                .ResultId(raw: Def.Sql.Identity)
                                .Title(demoDefinition.Title)
                                .Body(demoDefinition.Body.Replace(idHash))
                                .Status(demoDefinition.Status)
                                .Manager(idHash[demoDefinition.Manager])
                                .Owner(idHash[demoDefinition.Owner])
                                .ClassA(demoDefinition.ClassA.Replace(idHash))
                                .ClassB(demoDefinition.ClassB.Replace(idHash))
                                .ClassC(demoDefinition.ClassC.Replace(idHash))
                                .Comments(Comments(demoModel, idHash, demoDefinition.Id))
                                .Creator(idHash[demoDefinition.Creator])
                                .Updator(idHash[demoDefinition.Updator])
                                .CreatedTime(demoDefinition.CreatedTime.DemoTime(demoModel))
                                .UpdatedTime(demoDefinition.UpdatedTime.DemoTime(demoModel)),
                            addUpdatorParam: false)
                    });
                    idHash.Add(demoDefinition.Id, resultId);
                    var siteModel = new SiteModel(idHash[demoDefinition.ParentId]);
                    var siteSettings = siteModel.ResultsSiteSettings();
                    var resultModel = new ResultModel(
                        siteSettings, Permissions.Types.Manager, resultId);
                    Rds.ExecuteNonQuery(statements:
                        Rds.UpdateItems(
                            param: Rds.ItemsParam()
                                .SiteId(resultModel.SiteId)
                                .Title(resultModel.Title.DisplayValue)
                                .Subset(Jsons.ToJson(new ResultSubset(
                                    resultModel, siteSettings))),
                            where: Rds.ItemsWhere().ReferenceId(resultModel.ResultId),
                            addUpdatorParam: false,
                            addUpdatedTimeParam: false));
                });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void InitializeLinks(DemoModel demoModel, Dictionary<string, long> idHash)
        {
            Def.DemoDefinitionCollection
                .Where(o => o.Type == "Sites")
                .Where(o => o.ClassB.Trim() != string.Empty)
                .ForEach(demoDefinition =>
                    Rds.ExecuteNonQuery(statements:
                        Rds.InsertLinks(param: Rds.LinksParam()
                            .DestinationId(idHash[demoDefinition.ClassB])
                            .SourceId(idHash[demoDefinition.Id]))));
            Def.DemoDefinitionCollection
                .Where(o => o.ClassA.RegexExists("^#[A-Za-z0-9]+?#$"))
                .ForEach(demoDefinition =>
                    Rds.ExecuteNonQuery(statements:
                        Rds.InsertLinks(param: Rds.LinksParam()
                            .DestinationId(idHash[demoDefinition.ClassA
                                .Substring(1, demoDefinition.ClassA.Length - 2)])
                            .SourceId(idHash[demoDefinition.Id]))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void InitializePermissions(Dictionary<string, long> idHash)
        {
            idHash.Where(o => o.Key.StartsWith("Site")).Select(o => o.Value).ForEach(siteId =>
            {
                Rds.ExecuteNonQuery(statements:
                    Rds.InsertPermissions(
                        param: Rds.PermissionsParam()
                            .ReferenceType("Sites")
                            .ReferenceId(siteId)
                            .DeptId(0)
                            .UserId(idHash["User1"])
                            .PermissionType(Permissions.Types.Manager)));
                idHash.Where(o => o.Key.StartsWith("Dept")).Select(o => o.Value).ForEach(deptId =>
                {
                    Rds.ExecuteNonQuery(statements:
                        Rds.InsertPermissions(
                            param: Rds.PermissionsParam()
                                .ReferenceType("Sites")
                                .ReferenceId(siteId)
                                .DeptId(deptId)
                                .UserId(0)
                                .PermissionType(Permissions.Types.ReadWrite)));
                });
            });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void SetSites(Dictionary<string, long> idHash)
        {
            idHash.Where(o => o.Key.StartsWith("Site")).Select(o => o.Value).ForEach(siteId =>
                SiteInfo.SetSiteUserIdCollection(
                    new SiteModel(siteId).InheritPermission, reload: true));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string Comments(
            DemoModel demoModel,
            Dictionary<string, long> idHash,
            string parentId)
        {
            var comments = new Comments();
            Def.DemoDefinitionCollection
                .Where(o => o.Type == "Comments")
                .Where(o => o.ParentId == parentId)
                .Select((o, i) => new { DemoDefinition = o, Index = i })
                .ForEach(data =>
                    comments.Add(new Comment
                    {
                        CommentId = data.Index,
                        CreatedTime = data.DemoDefinition.CreatedTime.DemoTime(demoModel),
                        Creator = idHash[data.DemoDefinition.Creator].ToInt(),
                        Body = data.DemoDefinition.Body.Replace(idHash)
                    }));
            return comments.ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string Replace(this string self, Dictionary<string, long> idHash)
        {
            foreach (var id in self.RegexValues("#[A-Za-z0-9]+?#").Distinct())
            {
                self = self.Replace(
                    id, idHash[id.ToString().Substring(1, id.Length - 2)].ToString());
            }
            return self;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string LoginId(DemoModel demoModel, string userId)
        {
            return "Tenant" + demoModel.TenantId + "_" + userId;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static DateTime DemoTime(this DateTime self, DemoModel demoModel)
        {
            return self.AddDays(demoModel.TimeLag).ToUniversal();
        }
    }
}
