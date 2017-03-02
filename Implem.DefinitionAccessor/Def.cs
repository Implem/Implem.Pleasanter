using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Implem.DefinitionAccessor
{
    public static class Def
    {
        public static bool ExistsModel(
            string modelName,
            Func<ColumnDefinition, bool> peredicate = null)
        {
            return Def.ColumnDefinitionCollection
                .Where(o => o.ModelName == modelName)
                .Where(peredicate.IsNullCaseToTrue())
                .Any();
        }

        public static bool ExistsTable(
            string tableName,
            Func<ColumnDefinition,
            bool> peredicate = null)
        {
            return Def.ColumnDefinitionCollection
                .Where(o => o.TableName == tableName)
                .Where(peredicate.IsNullCaseToTrue())
                .Any();
        }

        public static bool ExistsColumnBase(
            Func<ColumnDefinition, bool> peredicate = null)
        {
            return ColumnDefinitionCollection
                .Where(o => o.ModelName == "_Base")
                .Where(peredicate.IsNullCaseToTrue())
                .Any();
        }

        public static bool ExistsColumnBaseItem(
            Func<ColumnDefinition, bool> peredicate = null)
        {
            return ColumnDefinitionCollection
                .Where(o => o.ModelName.StartsWith("_Base"))
                .Where(peredicate.IsNullCaseToTrue())
                .Where(o => ColumnDefinitionCollection
                    .Where(p => p.ModelName.StartsWith("_Base"))
                    .Where(p =>
                        p.ColumnName == o.ColumnName).Count() == 1 ||
                        o.ModelName == "_BaseItem").Any();
        }

        public static IEnumerable<string> ModelNameCollection(
            Func<ColumnDefinition, bool> peredicate = null,
            string order = "")
        {
            return ColumnDefinitionCollection
                .Where(o => !o.ModelName.StartsWith("_Base"))
                .Where(peredicate.IsNullCaseToTrue())
                .OrderBy(o => o[Strings.CoalesceEmpty(order, "No")])
                .Select(o => o.ModelName)
                .Distinct();
        }

        public static List<string> ItemModelNameCollection(
            Func<ColumnDefinition, bool> peredicate = null,
            string order = "")
        {
            return ColumnDefinitionCollection
                .Where(o => !o.ModelName.StartsWith("_Base"))
                .Where(o => o.ItemId > 0)
                .Where(peredicate.IsNullCaseToTrue())
                .OrderBy(o => o[Strings.CoalesceEmpty(order, "No")])
                .Select(o => o.ModelName)
                .Distinct()
                .ToList<string>();
        }

        public static IEnumerable<string> TableNameCollection(
            Func<ColumnDefinition, bool> peredicate = null,
            string order = "")
        {
            return ColumnDefinitionCollection
                .Where(o => !o.ModelName.StartsWith("_Base"))
                .Where(peredicate.IsNullCaseToTrue())
                .OrderBy(o => o[Strings.CoalesceEmpty(order, "No")])
                .Select(o => o.TableName)
                .Distinct();
        }

        public static List<string> ItemTableNameCollection(
            Func<ColumnDefinition, bool> peredicate = null,
            string order = "")
        {
            return ColumnDefinitionCollection
                .Where(o => !o.ModelName.StartsWith("_Base"))
                .Where(o => o.ItemId > 0)
                .Where(peredicate.IsNullCaseToTrue())
                .OrderBy(o => o[Strings.CoalesceEmpty(order, "No")])
                .Select(o => o.TableName)
                .Distinct()
                .ToList<string>();
        }

        public static string TableNameByModelName(string modelName)
        {
            return ColumnDefinitionCollection
                .Where(o => o.ModelName == modelName)
                .Select(o => o.TableName)
                .FirstOrDefault();
        }

        public static string ModelNameByTableName(string tableName)
        {
            return ColumnDefinitionCollection
                .Where(o => o.TableName.ToLower() == tableName.ToLower())
                .Select(o => o.ModelName)
                .FirstOrDefault();
        }

        public static IEnumerable<ColumnDefinition> BaseColumnDefinitionCollection(
            Func<ColumnDefinition, bool> peredicate = null,
            string order = "")
        {
            return ColumnDefinitionCollection
                .Where(o => o.ModelName == "_Base")
                .Where(peredicate.IsNullCaseToTrue())
                .OrderBy(o => o[Strings.CoalesceEmpty(order, "No")]);
        }

        public static IEnumerable<ColumnDefinition> BaseItemColumnDefinitionCollection(
            Func<ColumnDefinition, bool> peredicate = null,
            string order = "")
        {
            return ColumnDefinitionCollection
                .Where(o => o.ModelName == "_BaseItem")
                .Where(peredicate.IsNullCaseToTrue())
                .OrderBy(o => o[Strings.CoalesceEmpty(order, "No")])
                .Where(o => !ExistsColumnBase(p => p.ColumnName == o.ColumnName));
        }

        public static SqlIo SqlIoBySysem(
            RdsUser rdsUser = null,
            bool transactional = false,
            bool writeSqlToDebugLog = true,
            params SqlStatement[] statements)
        {
            return new SqlIo(CommandContainer(
                Parameters.Rds.SaConnectionString,
                rdsUser,
                transactional,
                writeSqlToDebugLog,
                statements));
        }

        public static SqlIo SqlIoByAdmin(
            RdsUser rdsUser = null,
            bool transactional = false,
            bool writeSqlToDebugLog = true,
            params SqlStatement[] statements)
        {
            return new SqlIo(CommandContainer(
                Parameters.Rds.OwnerConnectionString,
                rdsUser,
                transactional,
                writeSqlToDebugLog,
                statements));
        }

        public static SqlIo SqlIoByUser(
            string connectionString = null,
            RdsUser rdsUser = null,
            bool transactional = false,
            bool writeSqlToDebugLog = true,
            params SqlStatement[] statements)
        {
            return new SqlIo(CommandContainer(
                !connectionString.IsNullOrEmpty()
                    ? connectionString
                    : Parameters.Rds.UserConnectionString,
                rdsUser,
                transactional,
                writeSqlToDebugLog,
                statements));
        }

        private static SqlContainer CommandContainer(
            string connectionString,
            RdsUser rdsUser = null,
            bool transactional = false,
            bool writeSqlToDebugLog = true,
            params SqlStatement[] statements)
        {
            return new SqlContainer
            {
                RdsUser = rdsUser ?? new RdsUser(RdsUser.UserTypes.System),
                RdsName = Environments.ServiceName,
                RdsProvider = Environments.RdsProvider,
                ConnectionString = connectionString,
                SqlStatementCollection = SqlStatementCollection(statements),
                CommandTimeOut = Parameters.Rds.SqlCommandTimeOut,
                Transactional = transactional,
                WriteSqlToDebugLog = writeSqlToDebugLog
            };
        }

        private static List<SqlStatement> SqlStatementCollection(
            params SqlStatement[] statements)
        {
            return statements == null
                ? new List<SqlStatement>()
                : statements.Where(o => o != null).ToList();
        }

        public static Func<ColumnDefinition, bool> IsNullCaseToTrue(
            this Func<ColumnDefinition, bool> peredicate)
        {
            return peredicate != null
                ? peredicate
                : (o) => true;
        }

        public static XlsIo CodeXls;
        public static List<CodeDefinition> CodeDefinitionCollection;
        public static CodeColumn2nd Code;
        public static CodeTable CodeTable;

        public static void SetCodeDefinition()
        {
            ConstructCodeDefinitions();
            if (CodeXls.AccessStatus != Files.AccessStatuses.Read) { return; }
            CodeXls.XlsSheet.ForEach(definitionRow =>
            {
                switch (definitionRow[0].ToString())
                {
                    case "Initializer": Code.Initializer = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Initializer, definitionRow, CodeXls); break;
                    case "Def_SetDefinitions": Code.Def_SetDefinitions = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Def_SetDefinitions, definitionRow, CodeXls); break;
                    case "Def": Code.Def = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Def, definitionRow, CodeXls); break;
                    case "Def_FileDefinition": Code.Def_FileDefinition = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Def_FileDefinition, definitionRow, CodeXls); break;
                    case "Def_FileDefinition_SetColumn2ndAndTable": Code.Def_FileDefinition_SetColumn2ndAndTable = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Def_FileDefinition_SetColumn2ndAndTable, definitionRow, CodeXls); break;
                    case "Def_FileDefinition_NoSpace": Code.Def_FileDefinition_NoSpace = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Def_FileDefinition_NoSpace, definitionRow, CodeXls); break;
                    case "Def_FileDefinition_SetDefinition": Code.Def_FileDefinition_SetDefinition = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Def_FileDefinition_SetDefinition, definitionRow, CodeXls); break;
                    case "Def_FileDefinition_SetTable": Code.Def_FileDefinition_SetTable = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Def_FileDefinition_SetTable, definitionRow, CodeXls); break;
                    case "Def_SetDefinitionOption": Code.Def_SetDefinitionOption = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Def_SetDefinitionOption, definitionRow, CodeXls); break;
                    case "Def_SetDefinitionOption_Cases": Code.Def_SetDefinitionOption_Cases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Def_SetDefinitionOption_Cases, definitionRow, CodeXls); break;
                    case "Def_DefinitionClass": Code.Def_DefinitionClass = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Def_DefinitionClass, definitionRow, CodeXls); break;
                    case "Def_DefinitionClass_Definition": Code.Def_DefinitionClass_Definition = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Def_DefinitionClass_Definition, definitionRow, CodeXls); break;
                    case "Def_DefinitionClass_SetProperties": Code.Def_DefinitionClass_SetProperties = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Def_DefinitionClass_SetProperties, definitionRow, CodeXls); break;
                    case "Def_DefinitionClass_RestoreBySavedMemory": Code.Def_DefinitionClass_RestoreBySavedMemory = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Def_DefinitionClass_RestoreBySavedMemory, definitionRow, CodeXls); break;
                    case "Def_DefinitionClass_DefinitionAccessor": Code.Def_DefinitionClass_DefinitionAccessor = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Def_DefinitionClass_DefinitionAccessor, definitionRow, CodeXls); break;
                    case "Def_DefinitionClass_DefinitionColumn2nd": Code.Def_DefinitionClass_DefinitionColumn2nd = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Def_DefinitionClass_DefinitionColumn2nd, definitionRow, CodeXls); break;
                    case "Def_DefinitionClass_DefinitionTable": Code.Def_DefinitionClass_DefinitionTable = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Def_DefinitionClass_DefinitionTable, definitionRow, CodeXls); break;
                    case "Controller": Code.Controller = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Controller, definitionRow, CodeXls); break;
                    case "Controller_Groups": Code.Controller_Groups = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Controller_Groups, definitionRow, CodeXls); break;
                    case "Base": Code.Base = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Base, definitionRow, CodeXls); break;
                    case "Base_Property": Code.Base_Property = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Base_Property, definitionRow, CodeXls); break;
                    case "Base_PropertyCalc": Code.Base_PropertyCalc = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Base_PropertyCalc, definitionRow, CodeXls); break;
                    case "Base_SavedProperty": Code.Base_SavedProperty = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Base_SavedProperty, definitionRow, CodeXls); break;
                    case "Base_PropertyUpdated": Code.Base_PropertyUpdated = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Base_PropertyUpdated, definitionRow, CodeXls); break;
                    case "Base_PropertyUpdated_NotNull": Code.Base_PropertyUpdated_NotNull = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Base_PropertyUpdated_NotNull, definitionRow, CodeXls); break;
                    case "Model": Code.Model = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model, definitionRow, CodeXls); break;
                    case "Model_InheritBase": Code.Model_InheritBase = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_InheritBase, definitionRow, CodeXls); break;
                    case "Model_InheritBaseItem": Code.Model_InheritBaseItem = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_InheritBaseItem, definitionRow, CodeXls); break;
                    case "Model_IConvertable": Code.Model_IConvertable = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_IConvertable, definitionRow, CodeXls); break;
                    case "Model_ItemProperties": Code.Model_ItemProperties = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_ItemProperties, definitionRow, CodeXls); break;
                    case "Model_ItemProperties_Sites": Code.Model_ItemProperties_Sites = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_ItemProperties_Sites, definitionRow, CodeXls); break;
                    case "Model_UrlId": Code.Model_UrlId = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_UrlId, definitionRow, CodeXls); break;
                    case "Model_SessionProperties": Code.Model_SessionProperties = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SessionProperties, definitionRow, CodeXls); break;
                    case "Model_PropertyValue": Code.Model_PropertyValue = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_PropertyValue, definitionRow, CodeXls); break;
                    case "Model_PropertyValue_ColumnCases": Code.Model_PropertyValue_ColumnCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_PropertyValue_ColumnCases, definitionRow, CodeXls); break;
                    case "Model_PropertyValue_ColumnCases_ToString": Code.Model_PropertyValue_ColumnCases_ToString = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_PropertyValue_ColumnCases_ToString, definitionRow, CodeXls); break;
                    case "Model_SwitchTargets": Code.Model_SwitchTargets = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SwitchTargets, definitionRow, CodeXls); break;
                    case "Model_Constructor_Items": Code.Model_Constructor_Items = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Constructor_Items, definitionRow, CodeXls); break;
                    case "Model_Constructor": Code.Model_Constructor = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Constructor, definitionRow, CodeXls); break;
                    case "Model_IdentityParameters": Code.Model_IdentityParameters = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_IdentityParameters, definitionRow, CodeXls); break;
                    case "Model_SetSiteSettingsNew": Code.Model_SetSiteSettingsNew = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SetSiteSettingsNew, definitionRow, CodeXls); break;
                    case "Model_SetUserId": Code.Model_SetUserId = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SetUserId, definitionRow, CodeXls); break;
                    case "Model_SetIdentity": Code.Model_SetIdentity = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SetIdentity, definitionRow, CodeXls); break;
                    case "Model_SetTitleDisplayValue": Code.Model_SetTitleDisplayValue = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SetTitleDisplayValue, definitionRow, CodeXls); break;
                    case "Model_ClearSessions": Code.Model_ClearSessions = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_ClearSessions, definitionRow, CodeXls); break;
                    case "Model_Get": Code.Model_Get = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Get, definitionRow, CodeXls); break;
                    case "Model_GetDefaultColumns": Code.Model_GetDefaultColumns = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_GetDefaultColumns, definitionRow, CodeXls); break;
                    case "Model_GetSitesDefaultColumns": Code.Model_GetSitesDefaultColumns = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_GetSitesDefaultColumns, definitionRow, CodeXls); break;
                    case "Model_GetItemsDefaultColumns": Code.Model_GetItemsDefaultColumns = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_GetItemsDefaultColumns, definitionRow, CodeXls); break;
                    case "Model_SetSiteSettingsProperties": Code.Model_SetSiteSettingsProperties = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SetSiteSettingsProperties, definitionRow, CodeXls); break;
                    case "Model_SetTenantId": Code.Model_SetTenantId = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SetTenantId, definitionRow, CodeXls); break;
                    case "Model_SearchIndexHash": Code.Model_SearchIndexHash = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SearchIndexHash, definitionRow, CodeXls); break;
                    case "Model_SearchIndexHashColumnCases": Code.Model_SearchIndexHashColumnCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SearchIndexHashColumnCases, definitionRow, CodeXls); break;
                    case "Model_Create": Code.Model_Create = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Create, definitionRow, CodeXls); break;
                    case "Model_CreateParams": Code.Model_CreateParams = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_CreateParams, definitionRow, CodeXls); break;
                    case "Model_OnCreating_Binaries": Code.Model_OnCreating_Binaries = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_OnCreating_Binaries, definitionRow, CodeXls); break;
                    case "Model_OnCreating_Users": Code.Model_OnCreating_Users = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_OnCreating_Users, definitionRow, CodeXls); break;
                    case "Model_CheckNotificationConditions": Code.Model_CheckNotificationConditions = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_CheckNotificationConditions, definitionRow, CodeXls); break;
                    case "Model_CreatedNotice": Code.Model_CreatedNotice = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_CreatedNotice, definitionRow, CodeXls); break;
                    case "Model_InsertItems": Code.Model_InsertItems = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_InsertItems, definitionRow, CodeXls); break;
                    case "Model_InsertItemsAfter": Code.Model_InsertItemsAfter = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_InsertItemsAfter, definitionRow, CodeXls); break;
                    case "Model_SelectIdentity": Code.Model_SelectIdentity = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SelectIdentity, definitionRow, CodeXls); break;
                    case "Model_InsertGroupMember": Code.Model_InsertGroupMember = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_InsertGroupMember, definitionRow, CodeXls); break;
                    case "Model_InsertLinksByCreate": Code.Model_InsertLinksByCreate = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_InsertLinksByCreate, definitionRow, CodeXls); break;
                    case "Model_Insert": Code.Model_Insert = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Insert, definitionRow, CodeXls); break;
                    case "Model_InsertIdentity": Code.Model_InsertIdentity = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_InsertIdentity, definitionRow, CodeXls); break;
                    case "Model_InsertIdentitySet": Code.Model_InsertIdentitySet = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_InsertIdentitySet, definitionRow, CodeXls); break;
                    case "Model_Update": Code.Model_Update = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Update, definitionRow, CodeXls); break;
                    case "Model_UpdateParams": Code.Model_UpdateParams = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_UpdateParams, definitionRow, CodeXls); break;
                    case "Model_Update_Sites": Code.Model_Update_Sites = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Update_Sites, definitionRow, CodeXls); break;
                    case "Model_UpdateRelatedRecords": Code.Model_UpdateRelatedRecords = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_UpdateRelatedRecords, definitionRow, CodeXls); break;
                    case "Model_OnUpdatedNotice": Code.Model_OnUpdatedNotice = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_OnUpdatedNotice, definitionRow, CodeXls); break;
                    case "Model_OnUpdated_Depts": Code.Model_OnUpdated_Depts = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_OnUpdated_Depts, definitionRow, CodeXls); break;
                    case "Model_OnUpdated_Groups": Code.Model_OnUpdated_Groups = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_OnUpdated_Groups, definitionRow, CodeXls); break;
                    case "Model_OnUpdated_Users": Code.Model_OnUpdated_Users = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_OnUpdated_Users, definitionRow, CodeXls); break;
                    case "Model_OnUpdated_SetSiteMenu": Code.Model_OnUpdated_SetSiteMenu = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_OnUpdated_SetSiteMenu, definitionRow, CodeXls); break;
                    case "Model_OnUpdated_OutgoingMails": Code.Model_OnUpdated_OutgoingMails = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_OnUpdated_OutgoingMails, definitionRow, CodeXls); break;
                    case "Model_SynchronizeSummaryExecute": Code.Model_SynchronizeSummaryExecute = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SynchronizeSummaryExecute, definitionRow, CodeXls); break;
                    case "Model_SynchronizeSummary": Code.Model_SynchronizeSummary = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SynchronizeSummary, definitionRow, CodeXls); break;
                    case "Model_SynchronizeSummaryColumnCases": Code.Model_SynchronizeSummaryColumnCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SynchronizeSummaryColumnCases, definitionRow, CodeXls); break;
                    case "Model_UpdateFormulaColumns": Code.Model_UpdateFormulaColumns = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_UpdateFormulaColumns, definitionRow, CodeXls); break;
                    case "Model_UpdateFormulaColumns_ColumnCases": Code.Model_UpdateFormulaColumns_ColumnCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_UpdateFormulaColumns_ColumnCases, definitionRow, CodeXls); break;
                    case "Model_UpdateOrCreate": Code.Model_UpdateOrCreate = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_UpdateOrCreate, definitionRow, CodeXls); break;
                    case "Model_UpdateRelatedRecordsMethod": Code.Model_UpdateRelatedRecordsMethod = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_UpdateRelatedRecordsMethod, definitionRow, CodeXls); break;
                    case "Model_InsertLinksByUpdate": Code.Model_InsertLinksByUpdate = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_InsertLinksByUpdate, definitionRow, CodeXls); break;
                    case "Model_InsertLinksByUpdate_Site": Code.Model_InsertLinksByUpdate_Site = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_InsertLinksByUpdate_Site, definitionRow, CodeXls); break;
                    case "Model_UpdateSites_Wikis": Code.Model_UpdateSites_Wikis = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_UpdateSites_Wikis, definitionRow, CodeXls); break;
                    case "Model_InsertLinks": Code.Model_InsertLinks = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_InsertLinks, definitionRow, CodeXls); break;
                    case "Model_InsertLinksCases": Code.Model_InsertLinksCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_InsertLinksCases, definitionRow, CodeXls); break;
                    case "Model_Move": Code.Model_Move = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Move, definitionRow, CodeXls); break;
                    case "Model_Delete": Code.Model_Delete = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Delete, definitionRow, CodeXls); break;
                    case "Model_DeleteParams": Code.Model_DeleteParams = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_DeleteParams, definitionRow, CodeXls); break;
                    case "Model_Delete_Item": Code.Model_Delete_Item = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Delete_Item, definitionRow, CodeXls); break;
                    case "Model_Delete_GroupMembers": Code.Model_Delete_GroupMembers = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Delete_GroupMembers, definitionRow, CodeXls); break;
                    case "Model_Delete_Sites": Code.Model_Delete_Sites = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Delete_Sites, definitionRow, CodeXls); break;
                    case "Model_Delete_SitesItems": Code.Model_Delete_SitesItems = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Delete_SitesItems, definitionRow, CodeXls); break;
                    case "Model_OnDeleted_SetSiteInfo": Code.Model_OnDeleted_SetSiteInfo = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_OnDeleted_SetSiteInfo, definitionRow, CodeXls); break;
                    case "Model_OnDeletedNotice": Code.Model_OnDeletedNotice = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_OnDeletedNotice, definitionRow, CodeXls); break;
                    case "Model_Restore": Code.Model_Restore = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Restore, definitionRow, CodeXls); break;
                    case "Model_Restore_Item": Code.Model_Restore_Item = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Restore_Item, definitionRow, CodeXls); break;
                    case "Model_PhysicalDelete": Code.Model_PhysicalDelete = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_PhysicalDelete, definitionRow, CodeXls); break;
                    case "Model_SetByForm": Code.Model_SetByForm = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SetByForm, definitionRow, CodeXls); break;
                    case "Model_SetByForm_ColumnCases": Code.Model_SetByForm_ColumnCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SetByForm_ColumnCases, definitionRow, CodeXls); break;
                    case "Model_SetByForm_SetByFormula": Code.Model_SetByForm_SetByFormula = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SetByForm_SetByFormula, definitionRow, CodeXls); break;
                    case "Model_ToUniversal": Code.Model_ToUniversal = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_ToUniversal, definitionRow, CodeXls); break;
                    case "Model_SetByForm_Files": Code.Model_SetByForm_Files = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SetByForm_Files, definitionRow, CodeXls); break;
                    case "Model_SetByForm_Site": Code.Model_SetByForm_Site = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SetByForm_Site, definitionRow, CodeXls); break;
                    case "Model_CreateIndexes": Code.Model_CreateIndexes = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_CreateIndexes, definitionRow, CodeXls); break;
                    case "Model_AddSqlParamIdentity": Code.Model_AddSqlParamIdentity = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_AddSqlParamIdentity, definitionRow, CodeXls); break;
                    case "Model_AddSqlParamPk": Code.Model_AddSqlParamPk = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_AddSqlParamPk, definitionRow, CodeXls); break;
                    case "Model_AddSqlParam": Code.Model_AddSqlParam = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_AddSqlParam, definitionRow, CodeXls); break;
                    case "Model_AddSqlParamPkHistory": Code.Model_AddSqlParamPkHistory = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_AddSqlParamPkHistory, definitionRow, CodeXls); break;
                    case "Model_SetByFormula": Code.Model_SetByFormula = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SetByFormula, definitionRow, CodeXls); break;
                    case "Model_SetByFormula_Data": Code.Model_SetByFormula_Data = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SetByFormula_Data, definitionRow, CodeXls); break;
                    case "Model_SetByFormula_ColumnCases": Code.Model_SetByFormula_ColumnCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SetByFormula_ColumnCases, definitionRow, CodeXls); break;
                    case "Model_SetBySession": Code.Model_SetBySession = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SetBySession, definitionRow, CodeXls); break;
                    case "Model_SetPk": Code.Model_SetPk = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SetPk, definitionRow, CodeXls); break;
                    case "Model_Set": Code.Model_Set = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Set, definitionRow, CodeXls); break;
                    case "Model_PushState": Code.Model_PushState = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_PushState, definitionRow, CodeXls); break;
                    case "Model_PushState_Item": Code.Model_PushState_Item = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_PushState_Item, definitionRow, CodeXls); break;
                    case "Model_Matched": Code.Model_Matched = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Matched, definitionRow, CodeXls); break;
                    case "Model_Matched_Incomplete": Code.Model_Matched_Incomplete = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Matched_Incomplete, definitionRow, CodeXls); break;
                    case "Model_Matched_Own": Code.Model_Matched_Own = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Matched_Own, definitionRow, CodeXls); break;
                    case "Model_Matched_Issues": Code.Model_Matched_Issues = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Matched_Issues, definitionRow, CodeXls); break;
                    case "Model_Matched_ColumnCases": Code.Model_Matched_ColumnCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Matched_ColumnCases, definitionRow, CodeXls); break;
                    case "Model_Notice": Code.Model_Notice = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Notice, definitionRow, CodeXls); break;
                    case "Model_NoticeColumnCases": Code.Model_NoticeColumnCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_NoticeColumnCases, definitionRow, CodeXls); break;
                    case "Model_SwitchItems": Code.Model_SwitchItems = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SwitchItems, definitionRow, CodeXls); break;
                    case "Model_IndexCases": Code.Model_IndexCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_IndexCases, definitionRow, CodeXls); break;
                    case "Model_IndexJsonCases": Code.Model_IndexJsonCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_IndexJsonCases, definitionRow, CodeXls); break;
                    case "Model_GanttCases": Code.Model_GanttCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_GanttCases, definitionRow, CodeXls); break;
                    case "Model_GanttJsonCases": Code.Model_GanttJsonCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_GanttJsonCases, definitionRow, CodeXls); break;
                    case "Model_BurnDownCases": Code.Model_BurnDownCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_BurnDownCases, definitionRow, CodeXls); break;
                    case "Model_BurnDownJsonCases": Code.Model_BurnDownJsonCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_BurnDownJsonCases, definitionRow, CodeXls); break;
                    case "Model_BurnDownRecordDetailsJsonCases": Code.Model_BurnDownRecordDetailsJsonCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_BurnDownRecordDetailsJsonCases, definitionRow, CodeXls); break;
                    case "Model_TimeSeriesCases": Code.Model_TimeSeriesCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_TimeSeriesCases, definitionRow, CodeXls); break;
                    case "Model_TimeSeriesJsonCases": Code.Model_TimeSeriesJsonCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_TimeSeriesJsonCases, definitionRow, CodeXls); break;
                    case "Model_KambanCases": Code.Model_KambanCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_KambanCases, definitionRow, CodeXls); break;
                    case "Model_KambanJsonCases": Code.Model_KambanJsonCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_KambanJsonCases, definitionRow, CodeXls); break;
                    case "Model_NewCases": Code.Model_NewCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_NewCases, definitionRow, CodeXls); break;
                    case "Model_EditorCases": Code.Model_EditorCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_EditorCases, definitionRow, CodeXls); break;
                    case "Model_ImportCases": Code.Model_ImportCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_ImportCases, definitionRow, CodeXls); break;
                    case "Model_ExportCases": Code.Model_ExportCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_ExportCases, definitionRow, CodeXls); break;
                    case "Model_GridRowsCases": Code.Model_GridRowsCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_GridRowsCases, definitionRow, CodeXls); break;
                    case "Model_CreateCases": Code.Model_CreateCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_CreateCases, definitionRow, CodeXls); break;
                    case "Model_UpdateCases": Code.Model_UpdateCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_UpdateCases, definitionRow, CodeXls); break;
                    case "Model_DeleteCommentCases": Code.Model_DeleteCommentCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_DeleteCommentCases, definitionRow, CodeXls); break;
                    case "Model_CopyCases": Code.Model_CopyCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_CopyCases, definitionRow, CodeXls); break;
                    case "Model_MoveCases": Code.Model_MoveCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_MoveCases, definitionRow, CodeXls); break;
                    case "Model_BulkMoveCases": Code.Model_BulkMoveCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_BulkMoveCases, definitionRow, CodeXls); break;
                    case "Model_DeleteCases": Code.Model_DeleteCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_DeleteCases, definitionRow, CodeXls); break;
                    case "Model_BulkDeleteCases": Code.Model_BulkDeleteCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_BulkDeleteCases, definitionRow, CodeXls); break;
                    case "Model_RestoreCases": Code.Model_RestoreCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_RestoreCases, definitionRow, CodeXls); break;
                    case "Model_EditSeparateSettingsCases": Code.Model_EditSeparateSettingsCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_EditSeparateSettingsCases, definitionRow, CodeXls); break;
                    case "Model_SeparateCases": Code.Model_SeparateCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SeparateCases, definitionRow, CodeXls); break;
                    case "Model_HistoriesCases": Code.Model_HistoriesCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_HistoriesCases, definitionRow, CodeXls); break;
                    case "Model_HistoryCases": Code.Model_HistoryCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_HistoryCases, definitionRow, CodeXls); break;
                    case "Model_EditorJsonCases": Code.Model_EditorJsonCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_EditorJsonCases, definitionRow, CodeXls); break;
                    case "Model_GetCases": Code.Model_GetCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_GetCases, definitionRow, CodeXls); break;
                    case "Model_UpdateByKambanCases": Code.Model_UpdateByKambanCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_UpdateByKambanCases, definitionRow, CodeXls); break;
                    case "Model_ParentIdParameter": Code.Model_ParentIdParameter = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_ParentIdParameter, definitionRow, CodeXls); break;
                    case "Model_InheritPermissionParameter": Code.Model_InheritPermissionParameter = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_InheritPermissionParameter, definitionRow, CodeXls); break;
                    case "Model_SwitchTargetsParameter": Code.Model_SwitchTargetsParameter = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SwitchTargetsParameter, definitionRow, CodeXls); break;
                    case "Model_SetSiteId": Code.Model_SetSiteId = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SetSiteId, definitionRow, CodeXls); break;
                    case "Model_SetSiteSettings": Code.Model_SetSiteSettings = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SetSiteSettings, definitionRow, CodeXls); break;
                    case "Model_SetPermissionType": Code.Model_SetPermissionType = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SetPermissionType, definitionRow, CodeXls); break;
                    case "Model_SetParentId": Code.Model_SetParentId = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SetParentId, definitionRow, CodeXls); break;
                    case "Model_SetInheritPermission": Code.Model_SetInheritPermission = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SetInheritPermission, definitionRow, CodeXls); break;
                    case "Model_SetSwitchTargets": Code.Model_SetSwitchTargets = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SetSwitchTargets, definitionRow, CodeXls); break;
                    case "Model_SiteSettings": Code.Model_SiteSettings = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SiteSettings, definitionRow, CodeXls); break;
                    case "Model_SiteSettingsOnly": Code.Model_SiteSettingsOnly = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SiteSettingsOnly, definitionRow, CodeXls); break;
                    case "Model_SiteSettings_Sites": Code.Model_SiteSettings_Sites = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SiteSettings_Sites, definitionRow, CodeXls); break;
                    case "Model_SiteSettings_SitesOnly": Code.Model_SiteSettings_SitesOnly = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SiteSettings_SitesOnly, definitionRow, CodeXls); break;
                    case "Model_SiteSettingsWithParameterName": Code.Model_SiteSettingsWithParameterName = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SiteSettingsWithParameterName, definitionRow, CodeXls); break;
                    case "Model_SiteSettingsWithParameterNameLower": Code.Model_SiteSettingsWithParameterNameLower = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SiteSettingsWithParameterNameLower, definitionRow, CodeXls); break;
                    case "Model_SiteSettingsParam": Code.Model_SiteSettingsParam = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SiteSettingsParam, definitionRow, CodeXls); break;
                    case "Model_SiteSettingsParamOnly": Code.Model_SiteSettingsParamOnly = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SiteSettingsParamOnly, definitionRow, CodeXls); break;
                    case "Collection": Code.Collection = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Collection, definitionRow, CodeXls); break;
                    case "Collection_SiteSettings": Code.Collection_SiteSettings = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Collection_SiteSettings, definitionRow, CodeXls); break;
                    case "Collection_SiteSettingsArgument": Code.Collection_SiteSettingsArgument = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Collection_SiteSettingsArgument, definitionRow, CodeXls); break;
                    case "Collection_Aggregation": Code.Collection_Aggregation = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Collection_Aggregation, definitionRow, CodeXls); break;
                    case "Collection_ItemAggregation": Code.Collection_ItemAggregation = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Collection_ItemAggregation, definitionRow, CodeXls); break;
                    case "Model_Utilities": Code.Model_Utilities = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities, definitionRow, CodeXls); break;
                    case "Model_Utilities_Index": Code.Model_Utilities_Index = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_Index, definitionRow, CodeXls); break;
                    case "Model_Utilities_SetChoiceHash": Code.Model_Utilities_SetChoiceHash = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_SetChoiceHash, definitionRow, CodeXls); break;
                    case "Model_Utilities_DropDownSearchDialog": Code.Model_Utilities_DropDownSearchDialog = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_DropDownSearchDialog, definitionRow, CodeXls); break;
                    case "Model_Utilities_ImportSettings": Code.Model_Utilities_ImportSettings = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_ImportSettings, definitionRow, CodeXls); break;
                    case "Model_Utilities_GridRows_OnClick": Code.Model_Utilities_GridRows_OnClick = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_GridRows_OnClick, definitionRow, CodeXls); break;
                    case "Model_Utilities_GridRows_OnClickItem": Code.Model_Utilities_GridRows_OnClickItem = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_GridRows_OnClickItem, definitionRow, CodeXls); break;
                    case "Model_Utilities_TdValue": Code.Model_Utilities_TdValue = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_TdValue, definitionRow, CodeXls); break;
                    case "Model_Utilities_TdValueCases": Code.Model_Utilities_TdValueCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_TdValueCases, definitionRow, CodeXls); break;
                    case "Model_Utilities_TdValueCustomValueCases": Code.Model_Utilities_TdValueCustomValueCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_TdValueCustomValueCases, definitionRow, CodeXls); break;
                    case "Model_Utilities_SqlColumn_SiteId": Code.Model_Utilities_SqlColumn_SiteId = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_SqlColumn_SiteId, definitionRow, CodeXls); break;
                    case "Model_Utilities_GridSqlWhereTenantId": Code.Model_Utilities_GridSqlWhereTenantId = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_GridSqlWhereTenantId, definitionRow, CodeXls); break;
                    case "Model_Utilities_GridSqlWhereSiteId": Code.Model_Utilities_GridSqlWhereSiteId = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_GridSqlWhereSiteId, definitionRow, CodeXls); break;
                    case "Model_Utilities_Editor": Code.Model_Utilities_Editor = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_Editor, definitionRow, CodeXls); break;
                    case "Model_Utilities_SiteSettingsUtilities": Code.Model_Utilities_SiteSettingsUtilities = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_SiteSettingsUtilities, definitionRow, CodeXls); break;
                    case "Model_Utilities_EditorItem": Code.Model_Utilities_EditorItem = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_EditorItem, definitionRow, CodeXls); break;
                    case "Model_Utilities_SetSwitchTargets": Code.Model_Utilities_SetSwitchTargets = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_SetSwitchTargets, definitionRow, CodeXls); break;
                    case "Model_Utilities_FieldCases": Code.Model_Utilities_FieldCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_FieldCases, definitionRow, CodeXls); break;
                    case "Model_Utilities_FieldCases_Item": Code.Model_Utilities_FieldCases_Item = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_FieldCases_Item, definitionRow, CodeXls); break;
                    case "Model_Utilities_Links": Code.Model_Utilities_Links = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_Links, definitionRow, CodeXls); break;
                    case "Model_Utilities_EditorJson": Code.Model_Utilities_EditorJson = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_EditorJson, definitionRow, CodeXls); break;
                    case "Model_Utilities_EditorResponse": Code.Model_Utilities_EditorResponse = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_EditorResponse, definitionRow, CodeXls); break;
                    case "Model_Utilities_GetSwitchTargets": Code.Model_Utilities_GetSwitchTargets = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_GetSwitchTargets, definitionRow, CodeXls); break;
                    case "Model_Utilities_SqlWhereTenantId": Code.Model_Utilities_SqlWhereTenantId = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_SqlWhereTenantId, definitionRow, CodeXls); break;
                    case "Model_Utilities_SqlWhereSiteId": Code.Model_Utilities_SqlWhereSiteId = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_SqlWhereSiteId, definitionRow, CodeXls); break;
                    case "Model_Utilities_SiteId": Code.Model_Utilities_SiteId = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_SiteId, definitionRow, CodeXls); break;
                    case "Model_Utilities_SiteIdParam": Code.Model_Utilities_SiteIdParam = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_SiteIdParam, definitionRow, CodeXls); break;
                    case "Model_Utilities_FieldResponse": Code.Model_Utilities_FieldResponse = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_FieldResponse, definitionRow, CodeXls); break;
                    case "Model_Utilities_FieldResponse_ColumnCases": Code.Model_Utilities_FieldResponse_ColumnCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_FieldResponse_ColumnCases, definitionRow, CodeXls); break;
                    case "Model_Utilities_TableName": Code.Model_Utilities_TableName = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_TableName, definitionRow, CodeXls); break;
                    case "Model_Utilities_TableNameCases": Code.Model_Utilities_TableNameCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_TableNameCases, definitionRow, CodeXls); break;
                    case "Model_Utilities_Create": Code.Model_Utilities_Create = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_Create, definitionRow, CodeXls); break;
                    case "Model_Utilities_CreateParams": Code.Model_Utilities_CreateParams = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_CreateParams, definitionRow, CodeXls); break;
                    case "Model_Utilities_CreateParams_Sites": Code.Model_Utilities_CreateParams_Sites = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_CreateParams_Sites, definitionRow, CodeXls); break;
                    case "Model_Utilities_CreatedResponse": Code.Model_Utilities_CreatedResponse = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_CreatedResponse, definitionRow, CodeXls); break;
                    case "Model_Utilities_CreatedResponse_Items": Code.Model_Utilities_CreatedResponse_Items = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_CreatedResponse_Items, definitionRow, CodeXls); break;
                    case "Model_Utilities_CreatedResponse_Sites": Code.Model_Utilities_CreatedResponse_Sites = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_CreatedResponse_Sites, definitionRow, CodeXls); break;
                    case "Model_Utilities_Update": Code.Model_Utilities_Update = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_Update, definitionRow, CodeXls); break;
                    case "Model_Utilities_Update_NewModel": Code.Model_Utilities_Update_NewModel = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_Update_NewModel, definitionRow, CodeXls); break;
                    case "Model_Utilities_Update_NewModel_Sites": Code.Model_Utilities_Update_NewModel_Sites = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_Update_NewModel_Sites, definitionRow, CodeXls); break;
                    case "Model_Utilities_UpdateInvalid_Users": Code.Model_Utilities_UpdateInvalid_Users = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_UpdateInvalid_Users, definitionRow, CodeXls); break;
                    case "Model_Utilities_OnUpdated_Issues": Code.Model_Utilities_OnUpdated_Issues = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_OnUpdated_Issues, definitionRow, CodeXls); break;
                    case "Model_Utilities_OnUpdated_Breadcrumb": Code.Model_Utilities_OnUpdated_Breadcrumb = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_OnUpdated_Breadcrumb, definitionRow, CodeXls); break;
                    case "Model_Utilities_ResponseByUpdate_FieldResponse": Code.Model_Utilities_ResponseByUpdate_FieldResponse = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_ResponseByUpdate_FieldResponse, definitionRow, CodeXls); break;
                    case "Model_Utilities_Copy": Code.Model_Utilities_Copy = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_Copy, definitionRow, CodeXls); break;
                    case "Model_Utilities_CopyResponse": Code.Model_Utilities_CopyResponse = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_CopyResponse, definitionRow, CodeXls); break;
                    case "Model_Utilities_CopyResponse_Items": Code.Model_Utilities_CopyResponse_Items = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_CopyResponse_Items, definitionRow, CodeXls); break;
                    case "Model_Utilities_Copy_Sites": Code.Model_Utilities_Copy_Sites = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_Copy_Sites, definitionRow, CodeXls); break;
                    case "Model_Utilities_Move": Code.Model_Utilities_Move = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_Move, definitionRow, CodeXls); break;
                    case "Model_Utilities_Delete": Code.Model_Utilities_Delete = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_Delete, definitionRow, CodeXls); break;
                    case "Model_Utilities_RedirectAfterDelete": Code.Model_Utilities_RedirectAfterDelete = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_RedirectAfterDelete, definitionRow, CodeXls); break;
                    case "Model_Utilities_RedirectAfterDeleteNotItem": Code.Model_Utilities_RedirectAfterDeleteNotItem = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_RedirectAfterDeleteNotItem, definitionRow, CodeXls); break;
                    case "Model_Utilities_RedirectAfterDeleteItem": Code.Model_Utilities_RedirectAfterDeleteItem = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_RedirectAfterDeleteItem, definitionRow, CodeXls); break;
                    case "Model_Utilities_RedirectAfterDelete_Sites": Code.Model_Utilities_RedirectAfterDelete_Sites = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_RedirectAfterDelete_Sites, definitionRow, CodeXls); break;
                    case "Model_Utilities_RedirectAfterDelete_Wikis": Code.Model_Utilities_RedirectAfterDelete_Wikis = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_RedirectAfterDelete_Wikis, definitionRow, CodeXls); break;
                    case "Model_Utilities_Restore": Code.Model_Utilities_Restore = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_Restore, definitionRow, CodeXls); break;
                    case "Model_Utilities_Histories": Code.Model_Utilities_Histories = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_Histories, definitionRow, CodeXls); break;
                    case "Model_Utilities_HistoriesParams": Code.Model_Utilities_HistoriesParams = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_HistoriesParams, definitionRow, CodeXls); break;
                    case "Model_Utilities_HistoriesParams_Sites": Code.Model_Utilities_HistoriesParams_Sites = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_HistoriesParams_Sites, definitionRow, CodeXls); break;
                    case "Model_Utilities_HistoryParams": Code.Model_Utilities_HistoryParams = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_HistoryParams, definitionRow, CodeXls); break;
                    case "Model_Utilities_HistoryParams_Sites": Code.Model_Utilities_HistoryParams_Sites = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_HistoryParams_Sites, definitionRow, CodeXls); break;
                    case "Model_Utilities_TitleDisplay": Code.Model_Utilities_TitleDisplay = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_TitleDisplay, definitionRow, CodeXls); break;
                    case "Model_Utilities_ResponseLinks": Code.Model_Utilities_ResponseLinks = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_ResponseLinks, definitionRow, CodeXls); break;
                    case "Model_Utilities_SiteModel": Code.Model_Utilities_SiteModel = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_SiteModel, definitionRow, CodeXls); break;
                    case "Model_Utilities_Separate": Code.Model_Utilities_Separate = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_Separate, definitionRow, CodeXls); break;
                    case "Model_Utilities_Title": Code.Model_Utilities_Title = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_Title, definitionRow, CodeXls); break;
                    case "Model_Utilities_BulkMove": Code.Model_Utilities_BulkMove = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_BulkMove, definitionRow, CodeXls); break;
                    case "Model_Utilities_BulkDelete": Code.Model_Utilities_BulkDelete = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_BulkDelete, definitionRow, CodeXls); break;
                    case "Model_Utilities_Import": Code.Model_Utilities_Import = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_Import, definitionRow, CodeXls); break;
                    case "Model_Utilities_ImportCases": Code.Model_Utilities_ImportCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_ImportCases, definitionRow, CodeXls); break;
                    case "Model_Utilities_ImportValidatorHeaders": Code.Model_Utilities_ImportValidatorHeaders = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_ImportValidatorHeaders, definitionRow, CodeXls); break;
                    case "Model_Utilities_ImportValidatorCases": Code.Model_Utilities_ImportValidatorCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_ImportValidatorCases, definitionRow, CodeXls); break;
                    case "Model_Utilities_Export": Code.Model_Utilities_Export = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_Export, definitionRow, CodeXls); break;
                    case "Model_Utilities_CsvColumnCases": Code.Model_Utilities_CsvColumnCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_CsvColumnCases, definitionRow, CodeXls); break;
                    case "Model_Utilities_NotNull": Code.Model_Utilities_NotNull = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_NotNull, definitionRow, CodeXls); break;
                    case "Model_Utilities_TitleDisplayValue": Code.Model_Utilities_TitleDisplayValue = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_TitleDisplayValue, definitionRow, CodeXls); break;
                    case "Model_Utilities_TitleDisplayValueCases_Item": Code.Model_Utilities_TitleDisplayValueCases_Item = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_TitleDisplayValueCases_Item, definitionRow, CodeXls); break;
                    case "Model_Utilities_TitleDisplayValueCases_DataRow": Code.Model_Utilities_TitleDisplayValueCases_DataRow = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_TitleDisplayValueCases_DataRow, definitionRow, CodeXls); break;
                    case "Model_Utilities_UpdateByKamban": Code.Model_Utilities_UpdateByKamban = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_UpdateByKamban, definitionRow, CodeXls); break;
                    case "Model_Utilities_SetNoticeParam": Code.Model_Utilities_SetNoticeParam = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_SetNoticeParam, definitionRow, CodeXls); break;
                    case "Model_Utilities_SearchIndexes": Code.Model_Utilities_SearchIndexes = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_SearchIndexes, definitionRow, CodeXls); break;
                    case "Model_Utilities_SearchIndexes_TableCases": Code.Model_Utilities_SearchIndexes_TableCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_SearchIndexes_TableCases, definitionRow, CodeXls); break;
                    case "Model_Utilities_ItemTitle": Code.Model_Utilities_ItemTitle = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utilities_ItemTitle, definitionRow, CodeXls); break;
                    case "Model_Validator": Code.Model_Validator = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Validator, definitionRow, CodeXls); break;
                    case "Model_ValidatorMethods": Code.Model_ValidatorMethods = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_ValidatorMethods, definitionRow, CodeXls); break;
                    case "Model_Validator_OnMoving": Code.Model_Validator_OnMoving = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Validator_OnMoving, definitionRow, CodeXls); break;
                    case "Model_Validator_OnCreatingCases": Code.Model_Validator_OnCreatingCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Validator_OnCreatingCases, definitionRow, CodeXls); break;
                    case "Model_Validator_OnUpdating_Users": Code.Model_Validator_OnUpdating_Users = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Validator_OnUpdating_Users, definitionRow, CodeXls); break;
                    case "Model_Validator_OnUpdatingCases": Code.Model_Validator_OnUpdatingCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Validator_OnUpdatingCases, definitionRow, CodeXls); break;
                    case "Model_ValidatorMethods_Binaries": Code.Model_ValidatorMethods_Binaries = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_ValidatorMethods_Binaries, definitionRow, CodeXls); break;
                    case "Rds": Code.Rds = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds, definitionRow, CodeXls); break;
                    case "Rds_SqlStatement": Code.Rds_SqlStatement = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_SqlStatement, definitionRow, CodeXls); break;
                    case "Rds_SqlSelect": Code.Rds_SqlSelect = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_SqlSelect, definitionRow, CodeXls); break;
                    case "Rds_SqlExists": Code.Rds_SqlExists = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_SqlExists, definitionRow, CodeXls); break;
                    case "Rds_SqlInsert": Code.Rds_SqlInsert = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_SqlInsert, definitionRow, CodeXls); break;
                    case "Rds_SqlUpdate": Code.Rds_SqlUpdate = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_SqlUpdate, definitionRow, CodeXls); break;
                    case "Rds_SqlUpdateOrInsert": Code.Rds_SqlUpdateOrInsert = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_SqlUpdateOrInsert, definitionRow, CodeXls); break;
                    case "Rds_SqlDelete": Code.Rds_SqlDelete = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_SqlDelete, definitionRow, CodeXls); break;
                    case "Rds_SqlPhysicalDelete": Code.Rds_SqlPhysicalDelete = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_SqlPhysicalDelete, definitionRow, CodeXls); break;
                    case "Rds_SqlRestore": Code.Rds_SqlRestore = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_SqlRestore, definitionRow, CodeXls); break;
                    case "Rds_AggregationTableCases": Code.Rds_AggregationTableCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_AggregationTableCases, definitionRow, CodeXls); break;
                    case "Rds_Aggregation": Code.Rds_Aggregation = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Aggregation, definitionRow, CodeXls); break;
                    case "Rds_AggregationTotalCases": Code.Rds_AggregationTotalCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_AggregationTotalCases, definitionRow, CodeXls); break;
                    case "Rds_AggregationAverageCases": Code.Rds_AggregationAverageCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_AggregationAverageCases, definitionRow, CodeXls); break;
                    case "Rds_AggregationGroupByCases": Code.Rds_AggregationGroupByCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_AggregationGroupByCases, definitionRow, CodeXls); break;
                    case "Rds_AggregationOverdue": Code.Rds_AggregationOverdue = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_AggregationOverdue, definitionRow, CodeXls); break;
                    case "Rds_SaveHistory": Code.Rds_SaveHistory = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_SaveHistory, definitionRow, CodeXls); break;
                    case "Rds_SaveHistory_Columns": Code.Rds_SaveHistory_Columns = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_SaveHistory_Columns, definitionRow, CodeXls); break;
                    case "Rds_SaveHistory_HistoryColumns": Code.Rds_SaveHistory_HistoryColumns = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_SaveHistory_HistoryColumns, definitionRow, CodeXls); break;
                    case "Rds_SaveHistory_WhereColumns": Code.Rds_SaveHistory_WhereColumns = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_SaveHistory_WhereColumns, definitionRow, CodeXls); break;
                    case "Rds_Delete": Code.Rds_Delete = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Delete, definitionRow, CodeXls); break;
                    case "Rds_Delete_Columns": Code.Rds_Delete_Columns = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Delete_Columns, definitionRow, CodeXls); break;
                    case "Rds_Delete_DeletedColumns": Code.Rds_Delete_DeletedColumns = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Delete_DeletedColumns, definitionRow, CodeXls); break;
                    case "Rds_Delete_WhereColumns": Code.Rds_Delete_WhereColumns = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Delete_WhereColumns, definitionRow, CodeXls); break;
                    case "Rds_Delete_WhereDeletedColumns": Code.Rds_Delete_WhereDeletedColumns = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Delete_WhereDeletedColumns, definitionRow, CodeXls); break;
                    case "Rds_Restore": Code.Rds_Restore = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Restore, definitionRow, CodeXls); break;
                    case "Rds_Restore_Columns": Code.Rds_Restore_Columns = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Restore_Columns, definitionRow, CodeXls); break;
                    case "Rds_Restore_DeletedColumns": Code.Rds_Restore_DeletedColumns = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Restore_DeletedColumns, definitionRow, CodeXls); break;
                    case "Rds_Restore_WhereColumns": Code.Rds_Restore_WhereColumns = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Restore_WhereColumns, definitionRow, CodeXls); break;
                    case "Rds_Restore_WhereDeletedColumns": Code.Rds_Restore_WhereDeletedColumns = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Restore_WhereDeletedColumns, definitionRow, CodeXls); break;
                    case "Rds_Restore_IdentityOn": Code.Rds_Restore_IdentityOn = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Restore_IdentityOn, definitionRow, CodeXls); break;
                    case "Rds_Restore_IdentityOff": Code.Rds_Restore_IdentityOff = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Restore_IdentityOff, definitionRow, CodeXls); break;
                    case "Rds_Columns": Code.Rds_Columns = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns, definitionRow, CodeXls); break;
                    case "Rds_Columns_SqlClasses": Code.Rds_Columns_SqlClasses = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns_SqlClasses, definitionRow, CodeXls); break;
                    case "Rds_Columns_ConstSqlWhereLike": Code.Rds_Columns_ConstSqlWhereLike = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns_ConstSqlWhereLike, definitionRow, CodeXls); break;
                    case "Rds_Columns_ConstSqlWhereExists": Code.Rds_Columns_ConstSqlWhereExists = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns_ConstSqlWhereExists, definitionRow, CodeXls); break;
                    case "Rds_Columns_ConstSqlWhereNotExists": Code.Rds_Columns_ConstSqlWhereNotExists = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns_ConstSqlWhereNotExists, definitionRow, CodeXls); break;
                    case "Rds_Columns_SqlColumnCases": Code.Rds_Columns_SqlColumnCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns_SqlColumnCases, definitionRow, CodeXls); break;
                    case "Rds_Columns_SqlColumn": Code.Rds_Columns_SqlColumn = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns_SqlColumn, definitionRow, CodeXls); break;
                    case "Rds_Columns_SqlColumn_SelectColumns": Code.Rds_Columns_SqlColumn_SelectColumns = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns_SqlColumn_SelectColumns, definitionRow, CodeXls); break;
                    case "Rds_Columns_SqlColumn_ComputeColumn": Code.Rds_Columns_SqlColumn_ComputeColumn = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns_SqlColumn_ComputeColumn, definitionRow, CodeXls); break;
                    case "Rds_Columns_SqlWhere": Code.Rds_Columns_SqlWhere = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns_SqlWhere, definitionRow, CodeXls); break;
                    case "Rds_Columns_SqlWhere_SelectColumns": Code.Rds_Columns_SqlWhere_SelectColumns = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns_SqlWhere_SelectColumns, definitionRow, CodeXls); break;
                    case "Rds_Columns_SqlWhere_ComputeColumn": Code.Rds_Columns_SqlWhere_ComputeColumn = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns_SqlWhere_ComputeColumn, definitionRow, CodeXls); break;
                    case "Rds_Columns_SqlWhere_In": Code.Rds_Columns_SqlWhere_In = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns_SqlWhere_In, definitionRow, CodeXls); break;
                    case "Rds_Columns_SqlWhere_Between_Number": Code.Rds_Columns_SqlWhere_Between_Number = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns_SqlWhere_Between_Number, definitionRow, CodeXls); break;
                    case "Rds_Columns_SqlWhere_Between_DateTime": Code.Rds_Columns_SqlWhere_Between_DateTime = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns_SqlWhere_Between_DateTime, definitionRow, CodeXls); break;
                    case "Rds_Columns_SqlGroupByCases": Code.Rds_Columns_SqlGroupByCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns_SqlGroupByCases, definitionRow, CodeXls); break;
                    case "Rds_Columns_SqlGroupBy": Code.Rds_Columns_SqlGroupBy = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns_SqlGroupBy, definitionRow, CodeXls); break;
                    case "Rds_Columns_SqlHavingComputes": Code.Rds_Columns_SqlHavingComputes = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns_SqlHavingComputes, definitionRow, CodeXls); break;
                    case "Rds_Columns_SqlOrderBy1": Code.Rds_Columns_SqlOrderBy1 = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns_SqlOrderBy1, definitionRow, CodeXls); break;
                    case "Rds_Columns_SqlOrderBy2": Code.Rds_Columns_SqlOrderBy2 = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns_SqlOrderBy2, definitionRow, CodeXls); break;
                    case "Rds_Columns_SqlParam_ItemId": Code.Rds_Columns_SqlParam_ItemId = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns_SqlParam_ItemId, definitionRow, CodeXls); break;
                    case "Rds_Columns_SqlParam": Code.Rds_Columns_SqlParam = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns_SqlParam, definitionRow, CodeXls); break;
                    case "Rds_Columns_SqlParam_Enum": Code.Rds_Columns_SqlParam_Enum = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns_SqlParam_Enum, definitionRow, CodeXls); break;
                    case "Rds_Defaults": Code.Rds_Defaults = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Defaults, definitionRow, CodeXls); break;
                    case "Rds_ColumnDefault": Code.Rds_ColumnDefault = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_ColumnDefault, definitionRow, CodeXls); break;
                    case "Rds_ItemEditorColumns": Code.Rds_ItemEditorColumns = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_ItemEditorColumns, definitionRow, CodeXls); break;
                    case "Rds_ColumnDefaultPk": Code.Rds_ColumnDefaultPk = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_ColumnDefaultPk, definitionRow, CodeXls); break;
                    case "Rds_JoinDefault": Code.Rds_JoinDefault = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_JoinDefault, definitionRow, CodeXls); break;
                    case "Rds_WherePk": Code.Rds_WherePk = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_WherePk, definitionRow, CodeXls); break;
                    case "Rds_Where_TenantId": Code.Rds_Where_TenantId = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Where_TenantId, definitionRow, CodeXls); break;
                    case "Rds_Where_ItemId": Code.Rds_Where_ItemId = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Where_ItemId, definitionRow, CodeXls); break;
                    case "Rds_WhereHistory": Code.Rds_WhereHistory = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_WhereHistory, definitionRow, CodeXls); break;
                    case "Rds_ParamDefault": Code.Rds_ParamDefault = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_ParamDefault, definitionRow, CodeXls); break;
                    case "Rds_ParamDefault_SetDefault": Code.Rds_ParamDefault_SetDefault = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_ParamDefault_SetDefault, definitionRow, CodeXls); break;
                    case "Rds_ParamDefault_ParamAll": Code.Rds_ParamDefault_ParamAll = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_ParamDefault_ParamAll, definitionRow, CodeXls); break;
                    case "Rds_ParamItemId": Code.Rds_ParamItemId = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_ParamItemId, definitionRow, CodeXls); break;
                    case "Rds_SiteId": Code.Rds_SiteId = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_SiteId, definitionRow, CodeXls); break;
                    case "Rds_TitleColumn": Code.Rds_TitleColumn = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_TitleColumn, definitionRow, CodeXls); break;
                    case "Rds_TitleColumnCases": Code.Rds_TitleColumnCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_TitleColumnCases, definitionRow, CodeXls); break;
                    case "Titles": Code.Titles = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Titles, definitionRow, CodeXls); break;
                    case "Titles_TitleDisplayValueCases": Code.Titles_TitleDisplayValueCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Titles_TitleDisplayValueCases, definitionRow, CodeXls); break;
                    case "Indexes": Code.Indexes = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Indexes, definitionRow, CodeXls); break;
                    case "Indexes_TableCases": Code.Indexes_TableCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Indexes_TableCases, definitionRow, CodeXls); break;
                    case "ItemsInitializer": Code.ItemsInitializer = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.ItemsInitializer, definitionRow, CodeXls); break;
                    case "ItemsInitializer_InitItems": Code.ItemsInitializer_InitItems = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.ItemsInitializer_InitItems, definitionRow, CodeXls); break;
                    case "SiteSettings": Code.SiteSettings = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.SiteSettings, definitionRow, CodeXls); break;
                    case "SiteSettings_GetCases": Code.SiteSettings_GetCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.SiteSettings_GetCases, definitionRow, CodeXls); break;
                    case "SiteSettings_GetByItemCases": Code.SiteSettings_GetByItemCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.SiteSettings_GetByItemCases, definitionRow, CodeXls); break;
                    case "SiteSettings_GetByReferenceCases": Code.SiteSettings_GetByReferenceCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.SiteSettings_GetByReferenceCases, definitionRow, CodeXls); break;
                    case "SiteSettings_GetByItemReferenceCases": Code.SiteSettings_GetByItemReferenceCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.SiteSettings_GetByItemReferenceCases, definitionRow, CodeXls); break;
                    case "SiteSettings_GetModels": Code.SiteSettings_GetModels = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.SiteSettings_GetModels, definitionRow, CodeXls); break;
                    case "SiteSettings_GetModels_Choices": Code.SiteSettings_GetModels_Choices = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.SiteSettings_GetModels_Choices, definitionRow, CodeXls); break;
                    case "SiteSettings_GetModels_Items": Code.SiteSettings_GetModels_Items = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.SiteSettings_GetModels_Items, definitionRow, CodeXls); break;
                    case "SiteSettings_GetModels_Items_Choices": Code.SiteSettings_GetModels_Items_Choices = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.SiteSettings_GetModels_Items_Choices, definitionRow, CodeXls); break;
                    case "SiteSettings_GetModels_Includes": Code.SiteSettings_GetModels_Includes = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.SiteSettings_GetModels_Includes, definitionRow, CodeXls); break;
                    case "SiteSettings_UpdateTitles": Code.SiteSettings_UpdateTitles = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.SiteSettings_UpdateTitles, definitionRow, CodeXls); break;
                    case "SiteSettings_UpdateTitles_TableCases": Code.SiteSettings_UpdateTitles_TableCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.SiteSettings_UpdateTitles_TableCases, definitionRow, CodeXls); break;
                    case "View": Code.View = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.View, definitionRow, CodeXls); break;
                    case "View_Search_TableCases": Code.View_Search_TableCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.View_Search_TableCases, definitionRow, CodeXls); break;
                    case "View_Sorter_TableCases": Code.View_Sorter_TableCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.View_Sorter_TableCases, definitionRow, CodeXls); break;
                    case "View_Sorter_ColumnCases": Code.View_Sorter_ColumnCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.View_Sorter_ColumnCases, definitionRow, CodeXls); break;
                    case "HtmlLinks": Code.HtmlLinks = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.HtmlLinks, definitionRow, CodeXls); break;
                    case "HtmlLinks_DataSetTableCases": Code.HtmlLinks_DataSetTableCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.HtmlLinks_DataSetTableCases, definitionRow, CodeXls); break;
                    case "HtmlLinks_SelectStatementTableCases": Code.HtmlLinks_SelectStatementTableCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.HtmlLinks_SelectStatementTableCases, definitionRow, CodeXls); break;
                    case "HtmlLinks_TableCases": Code.HtmlLinks_TableCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.HtmlLinks_TableCases, definitionRow, CodeXls); break;
                    case "HtmlLinks_Headers": Code.HtmlLinks_Headers = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.HtmlLinks_Headers, definitionRow, CodeXls); break;
                    case "HtmlLinks_Rows": Code.HtmlLinks_Rows = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.HtmlLinks_Rows, definitionRow, CodeXls); break;
                    case "HtmlLinks_RowColumns": Code.HtmlLinks_RowColumns = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.HtmlLinks_RowColumns, definitionRow, CodeXls); break;
                    case "Summaries": Code.Summaries = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Summaries, definitionRow, CodeXls); break;
                    case "Summaries_SynchronizeCases": Code.Summaries_SynchronizeCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Summaries_SynchronizeCases, definitionRow, CodeXls); break;
                    case "Summaries_SynchronizeTables": Code.Summaries_SynchronizeTables = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Summaries_SynchronizeTables, definitionRow, CodeXls); break;
                    case "Summaries_ParamCases": Code.Summaries_ParamCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Summaries_ParamCases, definitionRow, CodeXls); break;
                    case "Summaries_ZeroParamCases": Code.Summaries_ZeroParamCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Summaries_ZeroParamCases, definitionRow, CodeXls); break;
                    case "Summaries_SelectCountTables": Code.Summaries_SelectCountTables = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Summaries_SelectCountTables, definitionRow, CodeXls); break;
                    case "Summaries_SelectTotalTableCases": Code.Summaries_SelectTotalTableCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Summaries_SelectTotalTableCases, definitionRow, CodeXls); break;
                    case "Summaries_SelectTotalColumns": Code.Summaries_SelectTotalColumns = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Summaries_SelectTotalColumns, definitionRow, CodeXls); break;
                    case "Summaries_SelectTotalColumnCases": Code.Summaries_SelectTotalColumnCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Summaries_SelectTotalColumnCases, definitionRow, CodeXls); break;
                    case "Summaries_SelectAverageTableCases": Code.Summaries_SelectAverageTableCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Summaries_SelectAverageTableCases, definitionRow, CodeXls); break;
                    case "Summaries_SelectAverageColumns": Code.Summaries_SelectAverageColumns = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Summaries_SelectAverageColumns, definitionRow, CodeXls); break;
                    case "Summaries_SelectAverageColumnCases": Code.Summaries_SelectAverageColumnCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Summaries_SelectAverageColumnCases, definitionRow, CodeXls); break;
                    case "Summaries_SelectMinTableCases": Code.Summaries_SelectMinTableCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Summaries_SelectMinTableCases, definitionRow, CodeXls); break;
                    case "Summaries_SelectMinColumns": Code.Summaries_SelectMinColumns = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Summaries_SelectMinColumns, definitionRow, CodeXls); break;
                    case "Summaries_SelectMinColumnCases": Code.Summaries_SelectMinColumnCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Summaries_SelectMinColumnCases, definitionRow, CodeXls); break;
                    case "Summaries_SelectMaxTableCases": Code.Summaries_SelectMaxTableCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Summaries_SelectMaxTableCases, definitionRow, CodeXls); break;
                    case "Summaries_SelectMaxColumns": Code.Summaries_SelectMaxColumns = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Summaries_SelectMaxColumns, definitionRow, CodeXls); break;
                    case "Summaries_SelectMaxColumnCases": Code.Summaries_SelectMaxColumnCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Summaries_SelectMaxColumnCases, definitionRow, CodeXls); break;
                    case "Summaries_WhereTables": Code.Summaries_WhereTables = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Summaries_WhereTables, definitionRow, CodeXls); break;
                    case "Summaries_WhereColumnCases": Code.Summaries_WhereColumnCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Summaries_WhereColumnCases, definitionRow, CodeXls); break;
                    case "FormulaUtilities": Code.FormulaUtilities = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.FormulaUtilities, definitionRow, CodeXls); break;
                    case "FormulaUtilities_TableCases": Code.FormulaUtilities_TableCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.FormulaUtilities_TableCases, definitionRow, CodeXls); break;
                    case "FormulaUtilities_Updates": Code.FormulaUtilities_Updates = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.FormulaUtilities_Updates, definitionRow, CodeXls); break;
                    case "Messages": Code.Messages = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Messages, definitionRow, CodeXls); break;
                    case "Messages_Parts": Code.Messages_Parts = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Messages_Parts, definitionRow, CodeXls); break;
                    case "Messages_Resonses": Code.Messages_Resonses = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Messages_Resonses, definitionRow, CodeXls); break;
                    case "ResponseCollection": Code.ResponseCollection = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.ResponseCollection, definitionRow, CodeXls); break;
                    case "ResponseCollection_Models": Code.ResponseCollection_Models = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.ResponseCollection_Models, definitionRow, CodeXls); break;
                    case "ResponseCollection_ValueModels": Code.ResponseCollection_ValueModels = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.ResponseCollection_ValueModels, definitionRow, CodeXls); break;
                    case "ResponseCollection_ValueColumns": Code.ResponseCollection_ValueColumns = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.ResponseCollection_ValueColumns, definitionRow, CodeXls); break;
                    case "Displays": Code.Displays = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Displays, definitionRow, CodeXls); break;
                    case "Displays_Parts": Code.Displays_Parts = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Displays_Parts, definitionRow, CodeXls); break;
                    case "Error": Code.Error = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Error, definitionRow, CodeXls); break;
                    case "Error_Types": Code.Error_Types = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Error_Types, definitionRow, CodeXls); break;
                    case "Error_Messages": Code.Error_Messages = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Error_Messages, definitionRow, CodeXls); break;
                    case "Css": Code.Css = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Css, definitionRow, CodeXls); break;
                    case "ClientDisplays": Code.ClientDisplays = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.ClientDisplays, definitionRow, CodeXls); break;
                    case "ClientDisplays_Parts": Code.ClientDisplays_Parts = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.ClientDisplays_Parts, definitionRow, CodeXls); break;
                    case "Comma": Code.Comma = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Comma, definitionRow, CodeXls); break;
                    default: break;
                }
            });
            CodeXls.XlsSheet.AsEnumerable().Skip(1).Where(o => o[0].ToString() != string.Empty).ForEach(definitionRow =>
            {
                var newCodeDefinition = new CodeDefinition();
                if (definitionRow.ContainsKey("Id")) { newCodeDefinition.Id = definitionRow["Id"].ToString(); newCodeDefinition.SavedId = newCodeDefinition.Id; }
                if (definitionRow.ContainsKey("Body")) { newCodeDefinition.Body = definitionRow["Body"].ToString(); newCodeDefinition.SavedBody = newCodeDefinition.Body; }
                if (definitionRow.ContainsKey("OutputPath")) { newCodeDefinition.OutputPath = definitionRow["OutputPath"].ToString(); newCodeDefinition.SavedOutputPath = newCodeDefinition.OutputPath; }
                if (definitionRow.ContainsKey("MergeToExisting")) { newCodeDefinition.MergeToExisting = definitionRow["MergeToExisting"].ToBool(); newCodeDefinition.SavedMergeToExisting = newCodeDefinition.MergeToExisting; }
                if (definitionRow.ContainsKey("Source")) { newCodeDefinition.Source = definitionRow["Source"].ToString(); newCodeDefinition.SavedSource = newCodeDefinition.Source; }
                if (definitionRow.ContainsKey("RepeatType")) { newCodeDefinition.RepeatType = definitionRow["RepeatType"].ToString(); newCodeDefinition.SavedRepeatType = newCodeDefinition.RepeatType; }
                if (definitionRow.ContainsKey("Indent")) { newCodeDefinition.Indent = definitionRow["Indent"].ToInt(); newCodeDefinition.SavedIndent = newCodeDefinition.Indent; }
                if (definitionRow.ContainsKey("Separator")) { newCodeDefinition.Separator = definitionRow["Separator"].ToString(); newCodeDefinition.SavedSeparator = newCodeDefinition.Separator; }
                if (definitionRow.ContainsKey("Order")) { newCodeDefinition.Order = definitionRow["Order"].ToString(); newCodeDefinition.SavedOrder = newCodeDefinition.Order; }
                if (definitionRow.ContainsKey("Pk")) { newCodeDefinition.Pk = definitionRow["Pk"].ToBool(); newCodeDefinition.SavedPk = newCodeDefinition.Pk; }
                if (definitionRow.ContainsKey("NotPk")) { newCodeDefinition.NotPk = definitionRow["NotPk"].ToBool(); newCodeDefinition.SavedNotPk = newCodeDefinition.NotPk; }
                if (definitionRow.ContainsKey("Identity")) { newCodeDefinition.Identity = definitionRow["Identity"].ToBool(); newCodeDefinition.SavedIdentity = newCodeDefinition.Identity; }
                if (definitionRow.ContainsKey("NotIdentity")) { newCodeDefinition.NotIdentity = definitionRow["NotIdentity"].ToBool(); newCodeDefinition.SavedNotIdentity = newCodeDefinition.NotIdentity; }
                if (definitionRow.ContainsKey("IdentityOrPk")) { newCodeDefinition.IdentityOrPk = definitionRow["IdentityOrPk"].ToBool(); newCodeDefinition.SavedIdentityOrPk = newCodeDefinition.IdentityOrPk; }
                if (definitionRow.ContainsKey("Unique")) { newCodeDefinition.Unique = definitionRow["Unique"].ToBool(); newCodeDefinition.SavedUnique = newCodeDefinition.Unique; }
                if (definitionRow.ContainsKey("NotUnique")) { newCodeDefinition.NotUnique = definitionRow["NotUnique"].ToBool(); newCodeDefinition.SavedNotUnique = newCodeDefinition.NotUnique; }
                if (definitionRow.ContainsKey("NotDefault")) { newCodeDefinition.NotDefault = definitionRow["NotDefault"].ToBool(); newCodeDefinition.SavedNotDefault = newCodeDefinition.NotDefault; }
                if (definitionRow.ContainsKey("Like")) { newCodeDefinition.Like = definitionRow["Like"].ToBool(); newCodeDefinition.SavedLike = newCodeDefinition.Like; }
                if (definitionRow.ContainsKey("HasIdentity")) { newCodeDefinition.HasIdentity = definitionRow["HasIdentity"].ToBool(); newCodeDefinition.SavedHasIdentity = newCodeDefinition.HasIdentity; }
                if (definitionRow.ContainsKey("HasNotIdentity")) { newCodeDefinition.HasNotIdentity = definitionRow["HasNotIdentity"].ToBool(); newCodeDefinition.SavedHasNotIdentity = newCodeDefinition.HasNotIdentity; }
                if (definitionRow.ContainsKey("HasTableNameId")) { newCodeDefinition.HasTableNameId = definitionRow["HasTableNameId"].ToBool(); newCodeDefinition.SavedHasTableNameId = newCodeDefinition.HasTableNameId; }
                if (definitionRow.ContainsKey("HasNotTableNameId")) { newCodeDefinition.HasNotTableNameId = definitionRow["HasNotTableNameId"].ToBool(); newCodeDefinition.SavedHasNotTableNameId = newCodeDefinition.HasNotTableNameId; }
                if (definitionRow.ContainsKey("ItemId")) { newCodeDefinition.ItemId = definitionRow["ItemId"].ToBool(); newCodeDefinition.SavedItemId = newCodeDefinition.ItemId; }
                if (definitionRow.ContainsKey("NotItemId")) { newCodeDefinition.NotItemId = definitionRow["NotItemId"].ToBool(); newCodeDefinition.SavedNotItemId = newCodeDefinition.NotItemId; }
                if (definitionRow.ContainsKey("Calc")) { newCodeDefinition.Calc = definitionRow["Calc"].ToBool(); newCodeDefinition.SavedCalc = newCodeDefinition.Calc; }
                if (definitionRow.ContainsKey("NotCalc")) { newCodeDefinition.NotCalc = definitionRow["NotCalc"].ToBool(); newCodeDefinition.SavedNotCalc = newCodeDefinition.NotCalc; }
                if (definitionRow.ContainsKey("SearchIndex")) { newCodeDefinition.SearchIndex = definitionRow["SearchIndex"].ToBool(); newCodeDefinition.SavedSearchIndex = newCodeDefinition.SearchIndex; }
                if (definitionRow.ContainsKey("NotByForm")) { newCodeDefinition.NotByForm = definitionRow["NotByForm"].ToBool(); newCodeDefinition.SavedNotByForm = newCodeDefinition.NotByForm; }
                if (definitionRow.ContainsKey("Form")) { newCodeDefinition.Form = definitionRow["Form"].ToBool(); newCodeDefinition.SavedForm = newCodeDefinition.Form; }
                if (definitionRow.ContainsKey("Select")) { newCodeDefinition.Select = definitionRow["Select"].ToBool(); newCodeDefinition.SavedSelect = newCodeDefinition.Select; }
                if (definitionRow.ContainsKey("Update")) { newCodeDefinition.Update = definitionRow["Update"].ToBool(); newCodeDefinition.SavedUpdate = newCodeDefinition.Update; }
                if (definitionRow.ContainsKey("SelectColumns")) { newCodeDefinition.SelectColumns = definitionRow["SelectColumns"].ToBool(); newCodeDefinition.SavedSelectColumns = newCodeDefinition.SelectColumns; }
                if (definitionRow.ContainsKey("NotSelectColumn")) { newCodeDefinition.NotSelectColumn = definitionRow["NotSelectColumn"].ToBool(); newCodeDefinition.SavedNotSelectColumn = newCodeDefinition.NotSelectColumn; }
                if (definitionRow.ContainsKey("ComputeColumn")) { newCodeDefinition.ComputeColumn = definitionRow["ComputeColumn"].ToBool(); newCodeDefinition.SavedComputeColumn = newCodeDefinition.ComputeColumn; }
                if (definitionRow.ContainsKey("NotComputeColumn")) { newCodeDefinition.NotComputeColumn = definitionRow["NotComputeColumn"].ToBool(); newCodeDefinition.SavedNotComputeColumn = newCodeDefinition.NotComputeColumn; }
                if (definitionRow.ContainsKey("Aggregatable")) { newCodeDefinition.Aggregatable = definitionRow["Aggregatable"].ToBool(); newCodeDefinition.SavedAggregatable = newCodeDefinition.Aggregatable; }
                if (definitionRow.ContainsKey("Computable")) { newCodeDefinition.Computable = definitionRow["Computable"].ToBool(); newCodeDefinition.SavedComputable = newCodeDefinition.Computable; }
                if (definitionRow.ContainsKey("Join")) { newCodeDefinition.Join = definitionRow["Join"].ToBool(); newCodeDefinition.SavedJoin = newCodeDefinition.Join; }
                if (definitionRow.ContainsKey("NotJoin")) { newCodeDefinition.NotJoin = definitionRow["NotJoin"].ToBool(); newCodeDefinition.SavedNotJoin = newCodeDefinition.NotJoin; }
                if (definitionRow.ContainsKey("JoinExpression")) { newCodeDefinition.JoinExpression = definitionRow["JoinExpression"].ToBool(); newCodeDefinition.SavedJoinExpression = newCodeDefinition.JoinExpression; }
                if (definitionRow.ContainsKey("NotTypeCs")) { newCodeDefinition.NotTypeCs = definitionRow["NotTypeCs"].ToBool(); newCodeDefinition.SavedNotTypeCs = newCodeDefinition.NotTypeCs; }
                if (definitionRow.ContainsKey("ItemOnly")) { newCodeDefinition.ItemOnly = definitionRow["ItemOnly"].ToBool(); newCodeDefinition.SavedItemOnly = newCodeDefinition.ItemOnly; }
                if (definitionRow.ContainsKey("NotItem")) { newCodeDefinition.NotItem = definitionRow["NotItem"].ToBool(); newCodeDefinition.SavedNotItem = newCodeDefinition.NotItem; }
                if (definitionRow.ContainsKey("GenericUi")) { newCodeDefinition.GenericUi = definitionRow["GenericUi"].ToBool(); newCodeDefinition.SavedGenericUi = newCodeDefinition.GenericUi; }
                if (definitionRow.ContainsKey("Session")) { newCodeDefinition.Session = definitionRow["Session"].ToBool(); newCodeDefinition.SavedSession = newCodeDefinition.Session; }
                if (definitionRow.ContainsKey("GridColumn")) { newCodeDefinition.GridColumn = definitionRow["GridColumn"].ToBool(); newCodeDefinition.SavedGridColumn = newCodeDefinition.GridColumn; }
                if (definitionRow.ContainsKey("FilterColumn")) { newCodeDefinition.FilterColumn = definitionRow["FilterColumn"].ToBool(); newCodeDefinition.SavedFilterColumn = newCodeDefinition.FilterColumn; }
                if (definitionRow.ContainsKey("EditorColumn")) { newCodeDefinition.EditorColumn = definitionRow["EditorColumn"].ToBool(); newCodeDefinition.SavedEditorColumn = newCodeDefinition.EditorColumn; }
                if (definitionRow.ContainsKey("TitleColumn")) { newCodeDefinition.TitleColumn = definitionRow["TitleColumn"].ToBool(); newCodeDefinition.SavedTitleColumn = newCodeDefinition.TitleColumn; }
                if (definitionRow.ContainsKey("UserColumn")) { newCodeDefinition.UserColumn = definitionRow["UserColumn"].ToBool(); newCodeDefinition.SavedUserColumn = newCodeDefinition.UserColumn; }
                if (definitionRow.ContainsKey("EnumColumn")) { newCodeDefinition.EnumColumn = definitionRow["EnumColumn"].ToBool(); newCodeDefinition.SavedEnumColumn = newCodeDefinition.EnumColumn; }
                if (definitionRow.ContainsKey("Include")) { newCodeDefinition.Include = definitionRow["Include"].ToString(); newCodeDefinition.SavedInclude = newCodeDefinition.Include; }
                if (definitionRow.ContainsKey("Exclude")) { newCodeDefinition.Exclude = definitionRow["Exclude"].ToString(); newCodeDefinition.SavedExclude = newCodeDefinition.Exclude; }
                if (definitionRow.ContainsKey("IncludeTypeName")) { newCodeDefinition.IncludeTypeName = definitionRow["IncludeTypeName"].ToString(); newCodeDefinition.SavedIncludeTypeName = newCodeDefinition.IncludeTypeName; }
                if (definitionRow.ContainsKey("ExcludeTypeName")) { newCodeDefinition.ExcludeTypeName = definitionRow["ExcludeTypeName"].ToString(); newCodeDefinition.SavedExcludeTypeName = newCodeDefinition.ExcludeTypeName; }
                if (definitionRow.ContainsKey("IncludeDefaultCs")) { newCodeDefinition.IncludeDefaultCs = definitionRow["IncludeDefaultCs"].ToString(); newCodeDefinition.SavedIncludeDefaultCs = newCodeDefinition.IncludeDefaultCs; }
                if (definitionRow.ContainsKey("ExcludeDefaultCs")) { newCodeDefinition.ExcludeDefaultCs = definitionRow["ExcludeDefaultCs"].ToString(); newCodeDefinition.SavedExcludeDefaultCs = newCodeDefinition.ExcludeDefaultCs; }
                if (definitionRow.ContainsKey("History")) { newCodeDefinition.History = definitionRow["History"].ToBool(); newCodeDefinition.SavedHistory = newCodeDefinition.History; }
                if (definitionRow.ContainsKey("PkHistory")) { newCodeDefinition.PkHistory = definitionRow["PkHistory"].ToBool(); newCodeDefinition.SavedPkHistory = newCodeDefinition.PkHistory; }
                if (definitionRow.ContainsKey("ControlType")) { newCodeDefinition.ControlType = definitionRow["ControlType"].ToString(); newCodeDefinition.SavedControlType = newCodeDefinition.ControlType; }
                if (definitionRow.ContainsKey("ReplaceOld")) { newCodeDefinition.ReplaceOld = definitionRow["ReplaceOld"].ToString(); newCodeDefinition.SavedReplaceOld = newCodeDefinition.ReplaceOld; }
                if (definitionRow.ContainsKey("ReplaceNew")) { newCodeDefinition.ReplaceNew = definitionRow["ReplaceNew"].ToString(); newCodeDefinition.SavedReplaceNew = newCodeDefinition.ReplaceNew; }
                if (definitionRow.ContainsKey("NotWhereSpecial")) { newCodeDefinition.NotWhereSpecial = definitionRow["NotWhereSpecial"].ToBool(); newCodeDefinition.SavedNotWhereSpecial = newCodeDefinition.NotWhereSpecial; }
                if (definitionRow.ContainsKey("NoSpace")) { newCodeDefinition.NoSpace = definitionRow["NoSpace"].ToBool(); newCodeDefinition.SavedNoSpace = newCodeDefinition.NoSpace; }
                if (definitionRow.ContainsKey("NotBase")) { newCodeDefinition.NotBase = definitionRow["NotBase"].ToBool(); newCodeDefinition.SavedNotBase = newCodeDefinition.NotBase; }
                if (definitionRow.ContainsKey("NotNull")) { newCodeDefinition.NotNull = definitionRow["NotNull"].ToBool(); newCodeDefinition.SavedNotNull = newCodeDefinition.NotNull; }
                if (definitionRow.ContainsKey("Validators")) { newCodeDefinition.Validators = definitionRow["Validators"].ToBool(); newCodeDefinition.SavedValidators = newCodeDefinition.Validators; }
                if (definitionRow.ContainsKey("DisplayType")) { newCodeDefinition.DisplayType = definitionRow["DisplayType"].ToString(); newCodeDefinition.SavedDisplayType = newCodeDefinition.DisplayType; }
                if (definitionRow.ContainsKey("DisplayLanguages")) { newCodeDefinition.DisplayLanguages = definitionRow["DisplayLanguages"].ToBool(); newCodeDefinition.SavedDisplayLanguages = newCodeDefinition.DisplayLanguages; }
                if (definitionRow.ContainsKey("ClientScript")) { newCodeDefinition.ClientScript = definitionRow["ClientScript"].ToBool(); newCodeDefinition.SavedClientScript = newCodeDefinition.ClientScript; }
                CodeDefinitionCollection.Add(newCodeDefinition);
            });
        }

        private static void SetCodeTable(CodeDefinition definition, XlsRow definitionRow, XlsIo codexls)
        {
            if (definitionRow.ContainsKey("Id")) { definition.Id = definitionRow["Id"].ToString(); definition.SavedId = definition.Id; }
            if (definitionRow.ContainsKey("Body")) { definition.Body = definitionRow["Body"].ToString(); definition.SavedBody = definition.Body; }
            if (definitionRow.ContainsKey("OutputPath")) { definition.OutputPath = definitionRow["OutputPath"].ToString(); definition.SavedOutputPath = definition.OutputPath; }
            if (definitionRow.ContainsKey("MergeToExisting")) { definition.MergeToExisting = definitionRow["MergeToExisting"].ToBool(); definition.SavedMergeToExisting = definition.MergeToExisting; }
            if (definitionRow.ContainsKey("Source")) { definition.Source = definitionRow["Source"].ToString(); definition.SavedSource = definition.Source; }
            if (definitionRow.ContainsKey("RepeatType")) { definition.RepeatType = definitionRow["RepeatType"].ToString(); definition.SavedRepeatType = definition.RepeatType; }
            if (definitionRow.ContainsKey("Indent")) { definition.Indent = definitionRow["Indent"].ToInt(); definition.SavedIndent = definition.Indent; }
            if (definitionRow.ContainsKey("Separator")) { definition.Separator = definitionRow["Separator"].ToString(); definition.SavedSeparator = definition.Separator; }
            if (definitionRow.ContainsKey("Order")) { definition.Order = definitionRow["Order"].ToString(); definition.SavedOrder = definition.Order; }
            if (definitionRow.ContainsKey("Pk")) { definition.Pk = definitionRow["Pk"].ToBool(); definition.SavedPk = definition.Pk; }
            if (definitionRow.ContainsKey("NotPk")) { definition.NotPk = definitionRow["NotPk"].ToBool(); definition.SavedNotPk = definition.NotPk; }
            if (definitionRow.ContainsKey("Identity")) { definition.Identity = definitionRow["Identity"].ToBool(); definition.SavedIdentity = definition.Identity; }
            if (definitionRow.ContainsKey("NotIdentity")) { definition.NotIdentity = definitionRow["NotIdentity"].ToBool(); definition.SavedNotIdentity = definition.NotIdentity; }
            if (definitionRow.ContainsKey("IdentityOrPk")) { definition.IdentityOrPk = definitionRow["IdentityOrPk"].ToBool(); definition.SavedIdentityOrPk = definition.IdentityOrPk; }
            if (definitionRow.ContainsKey("Unique")) { definition.Unique = definitionRow["Unique"].ToBool(); definition.SavedUnique = definition.Unique; }
            if (definitionRow.ContainsKey("NotUnique")) { definition.NotUnique = definitionRow["NotUnique"].ToBool(); definition.SavedNotUnique = definition.NotUnique; }
            if (definitionRow.ContainsKey("NotDefault")) { definition.NotDefault = definitionRow["NotDefault"].ToBool(); definition.SavedNotDefault = definition.NotDefault; }
            if (definitionRow.ContainsKey("Like")) { definition.Like = definitionRow["Like"].ToBool(); definition.SavedLike = definition.Like; }
            if (definitionRow.ContainsKey("HasIdentity")) { definition.HasIdentity = definitionRow["HasIdentity"].ToBool(); definition.SavedHasIdentity = definition.HasIdentity; }
            if (definitionRow.ContainsKey("HasNotIdentity")) { definition.HasNotIdentity = definitionRow["HasNotIdentity"].ToBool(); definition.SavedHasNotIdentity = definition.HasNotIdentity; }
            if (definitionRow.ContainsKey("HasTableNameId")) { definition.HasTableNameId = definitionRow["HasTableNameId"].ToBool(); definition.SavedHasTableNameId = definition.HasTableNameId; }
            if (definitionRow.ContainsKey("HasNotTableNameId")) { definition.HasNotTableNameId = definitionRow["HasNotTableNameId"].ToBool(); definition.SavedHasNotTableNameId = definition.HasNotTableNameId; }
            if (definitionRow.ContainsKey("ItemId")) { definition.ItemId = definitionRow["ItemId"].ToBool(); definition.SavedItemId = definition.ItemId; }
            if (definitionRow.ContainsKey("NotItemId")) { definition.NotItemId = definitionRow["NotItemId"].ToBool(); definition.SavedNotItemId = definition.NotItemId; }
            if (definitionRow.ContainsKey("Calc")) { definition.Calc = definitionRow["Calc"].ToBool(); definition.SavedCalc = definition.Calc; }
            if (definitionRow.ContainsKey("NotCalc")) { definition.NotCalc = definitionRow["NotCalc"].ToBool(); definition.SavedNotCalc = definition.NotCalc; }
            if (definitionRow.ContainsKey("SearchIndex")) { definition.SearchIndex = definitionRow["SearchIndex"].ToBool(); definition.SavedSearchIndex = definition.SearchIndex; }
            if (definitionRow.ContainsKey("NotByForm")) { definition.NotByForm = definitionRow["NotByForm"].ToBool(); definition.SavedNotByForm = definition.NotByForm; }
            if (definitionRow.ContainsKey("Form")) { definition.Form = definitionRow["Form"].ToBool(); definition.SavedForm = definition.Form; }
            if (definitionRow.ContainsKey("Select")) { definition.Select = definitionRow["Select"].ToBool(); definition.SavedSelect = definition.Select; }
            if (definitionRow.ContainsKey("Update")) { definition.Update = definitionRow["Update"].ToBool(); definition.SavedUpdate = definition.Update; }
            if (definitionRow.ContainsKey("SelectColumns")) { definition.SelectColumns = definitionRow["SelectColumns"].ToBool(); definition.SavedSelectColumns = definition.SelectColumns; }
            if (definitionRow.ContainsKey("NotSelectColumn")) { definition.NotSelectColumn = definitionRow["NotSelectColumn"].ToBool(); definition.SavedNotSelectColumn = definition.NotSelectColumn; }
            if (definitionRow.ContainsKey("ComputeColumn")) { definition.ComputeColumn = definitionRow["ComputeColumn"].ToBool(); definition.SavedComputeColumn = definition.ComputeColumn; }
            if (definitionRow.ContainsKey("NotComputeColumn")) { definition.NotComputeColumn = definitionRow["NotComputeColumn"].ToBool(); definition.SavedNotComputeColumn = definition.NotComputeColumn; }
            if (definitionRow.ContainsKey("Aggregatable")) { definition.Aggregatable = definitionRow["Aggregatable"].ToBool(); definition.SavedAggregatable = definition.Aggregatable; }
            if (definitionRow.ContainsKey("Computable")) { definition.Computable = definitionRow["Computable"].ToBool(); definition.SavedComputable = definition.Computable; }
            if (definitionRow.ContainsKey("Join")) { definition.Join = definitionRow["Join"].ToBool(); definition.SavedJoin = definition.Join; }
            if (definitionRow.ContainsKey("NotJoin")) { definition.NotJoin = definitionRow["NotJoin"].ToBool(); definition.SavedNotJoin = definition.NotJoin; }
            if (definitionRow.ContainsKey("JoinExpression")) { definition.JoinExpression = definitionRow["JoinExpression"].ToBool(); definition.SavedJoinExpression = definition.JoinExpression; }
            if (definitionRow.ContainsKey("NotTypeCs")) { definition.NotTypeCs = definitionRow["NotTypeCs"].ToBool(); definition.SavedNotTypeCs = definition.NotTypeCs; }
            if (definitionRow.ContainsKey("ItemOnly")) { definition.ItemOnly = definitionRow["ItemOnly"].ToBool(); definition.SavedItemOnly = definition.ItemOnly; }
            if (definitionRow.ContainsKey("NotItem")) { definition.NotItem = definitionRow["NotItem"].ToBool(); definition.SavedNotItem = definition.NotItem; }
            if (definitionRow.ContainsKey("GenericUi")) { definition.GenericUi = definitionRow["GenericUi"].ToBool(); definition.SavedGenericUi = definition.GenericUi; }
            if (definitionRow.ContainsKey("Session")) { definition.Session = definitionRow["Session"].ToBool(); definition.SavedSession = definition.Session; }
            if (definitionRow.ContainsKey("GridColumn")) { definition.GridColumn = definitionRow["GridColumn"].ToBool(); definition.SavedGridColumn = definition.GridColumn; }
            if (definitionRow.ContainsKey("FilterColumn")) { definition.FilterColumn = definitionRow["FilterColumn"].ToBool(); definition.SavedFilterColumn = definition.FilterColumn; }
            if (definitionRow.ContainsKey("EditorColumn")) { definition.EditorColumn = definitionRow["EditorColumn"].ToBool(); definition.SavedEditorColumn = definition.EditorColumn; }
            if (definitionRow.ContainsKey("TitleColumn")) { definition.TitleColumn = definitionRow["TitleColumn"].ToBool(); definition.SavedTitleColumn = definition.TitleColumn; }
            if (definitionRow.ContainsKey("UserColumn")) { definition.UserColumn = definitionRow["UserColumn"].ToBool(); definition.SavedUserColumn = definition.UserColumn; }
            if (definitionRow.ContainsKey("EnumColumn")) { definition.EnumColumn = definitionRow["EnumColumn"].ToBool(); definition.SavedEnumColumn = definition.EnumColumn; }
            if (definitionRow.ContainsKey("Include")) { definition.Include = definitionRow["Include"].ToString(); definition.SavedInclude = definition.Include; }
            if (definitionRow.ContainsKey("Exclude")) { definition.Exclude = definitionRow["Exclude"].ToString(); definition.SavedExclude = definition.Exclude; }
            if (definitionRow.ContainsKey("IncludeTypeName")) { definition.IncludeTypeName = definitionRow["IncludeTypeName"].ToString(); definition.SavedIncludeTypeName = definition.IncludeTypeName; }
            if (definitionRow.ContainsKey("ExcludeTypeName")) { definition.ExcludeTypeName = definitionRow["ExcludeTypeName"].ToString(); definition.SavedExcludeTypeName = definition.ExcludeTypeName; }
            if (definitionRow.ContainsKey("IncludeDefaultCs")) { definition.IncludeDefaultCs = definitionRow["IncludeDefaultCs"].ToString(); definition.SavedIncludeDefaultCs = definition.IncludeDefaultCs; }
            if (definitionRow.ContainsKey("ExcludeDefaultCs")) { definition.ExcludeDefaultCs = definitionRow["ExcludeDefaultCs"].ToString(); definition.SavedExcludeDefaultCs = definition.ExcludeDefaultCs; }
            if (definitionRow.ContainsKey("History")) { definition.History = definitionRow["History"].ToBool(); definition.SavedHistory = definition.History; }
            if (definitionRow.ContainsKey("PkHistory")) { definition.PkHistory = definitionRow["PkHistory"].ToBool(); definition.SavedPkHistory = definition.PkHistory; }
            if (definitionRow.ContainsKey("ControlType")) { definition.ControlType = definitionRow["ControlType"].ToString(); definition.SavedControlType = definition.ControlType; }
            if (definitionRow.ContainsKey("ReplaceOld")) { definition.ReplaceOld = definitionRow["ReplaceOld"].ToString(); definition.SavedReplaceOld = definition.ReplaceOld; }
            if (definitionRow.ContainsKey("ReplaceNew")) { definition.ReplaceNew = definitionRow["ReplaceNew"].ToString(); definition.SavedReplaceNew = definition.ReplaceNew; }
            if (definitionRow.ContainsKey("NotWhereSpecial")) { definition.NotWhereSpecial = definitionRow["NotWhereSpecial"].ToBool(); definition.SavedNotWhereSpecial = definition.NotWhereSpecial; }
            if (definitionRow.ContainsKey("NoSpace")) { definition.NoSpace = definitionRow["NoSpace"].ToBool(); definition.SavedNoSpace = definition.NoSpace; }
            if (definitionRow.ContainsKey("NotBase")) { definition.NotBase = definitionRow["NotBase"].ToBool(); definition.SavedNotBase = definition.NotBase; }
            if (definitionRow.ContainsKey("NotNull")) { definition.NotNull = definitionRow["NotNull"].ToBool(); definition.SavedNotNull = definition.NotNull; }
            if (definitionRow.ContainsKey("Validators")) { definition.Validators = definitionRow["Validators"].ToBool(); definition.SavedValidators = definition.Validators; }
            if (definitionRow.ContainsKey("DisplayType")) { definition.DisplayType = definitionRow["DisplayType"].ToString(); definition.SavedDisplayType = definition.DisplayType; }
            if (definitionRow.ContainsKey("DisplayLanguages")) { definition.DisplayLanguages = definitionRow["DisplayLanguages"].ToBool(); definition.SavedDisplayLanguages = definition.DisplayLanguages; }
            if (definitionRow.ContainsKey("ClientScript")) { definition.ClientScript = definitionRow["ClientScript"].ToBool(); definition.SavedClientScript = definition.ClientScript; }
        }

        private static void ConstructCodeDefinitions()
        {
            CodeXls = Initializer.DefinitionFile("definition_Code.xlsm");
            CodeDefinitionCollection = new List<CodeDefinition>();
            Code = new CodeColumn2nd();
            CodeTable = new CodeTable();
        }

        public static XlsIo ColumnXls;
        public static List<ColumnDefinition> ColumnDefinitionCollection;
        public static ColumnColumn2nd Column;
        public static ColumnTable ColumnTable;

        public static void SetColumnDefinition()
        {
            ConstructColumnDefinitions();
            if (ColumnXls.AccessStatus != Files.AccessStatuses.Read) { return; }
            ColumnXls.XlsSheet.ForEach(definitionRow =>
            {
                switch (definitionRow[0].ToString())
                {
                    case "_BaseItems_SiteId": Column._BaseItems_SiteId = definitionRow[1].ToString(); SetColumnTable(ColumnTable._BaseItems_SiteId, definitionRow, ColumnXls); break;
                    case "_BaseItems_UpdatedTime": Column._BaseItems_UpdatedTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable._BaseItems_UpdatedTime, definitionRow, ColumnXls); break;
                    case "_BaseItems_Title": Column._BaseItems_Title = definitionRow[1].ToString(); SetColumnTable(ColumnTable._BaseItems_Title, definitionRow, ColumnXls); break;
                    case "_BaseItems_Body": Column._BaseItems_Body = definitionRow[1].ToString(); SetColumnTable(ColumnTable._BaseItems_Body, definitionRow, ColumnXls); break;
                    case "_BaseItems_TitleBody": Column._BaseItems_TitleBody = definitionRow[1].ToString(); SetColumnTable(ColumnTable._BaseItems_TitleBody, definitionRow, ColumnXls); break;
                    case "_Bases_Ver": Column._Bases_Ver = definitionRow[1].ToString(); SetColumnTable(ColumnTable._Bases_Ver, definitionRow, ColumnXls); break;
                    case "_Bases_Comments": Column._Bases_Comments = definitionRow[1].ToString(); SetColumnTable(ColumnTable._Bases_Comments, definitionRow, ColumnXls); break;
                    case "_Bases_Creator": Column._Bases_Creator = definitionRow[1].ToString(); SetColumnTable(ColumnTable._Bases_Creator, definitionRow, ColumnXls); break;
                    case "_Bases_Updator": Column._Bases_Updator = definitionRow[1].ToString(); SetColumnTable(ColumnTable._Bases_Updator, definitionRow, ColumnXls); break;
                    case "_Bases_CreatedTime": Column._Bases_CreatedTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable._Bases_CreatedTime, definitionRow, ColumnXls); break;
                    case "_Bases_UpdatedTime": Column._Bases_UpdatedTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable._Bases_UpdatedTime, definitionRow, ColumnXls); break;
                    case "_Bases_VerUp": Column._Bases_VerUp = definitionRow[1].ToString(); SetColumnTable(ColumnTable._Bases_VerUp, definitionRow, ColumnXls); break;
                    case "_Bases_Timestamp": Column._Bases_Timestamp = definitionRow[1].ToString(); SetColumnTable(ColumnTable._Bases_Timestamp, definitionRow, ColumnXls); break;
                    case "Tenants_TenantId": Column.Tenants_TenantId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Tenants_TenantId, definitionRow, ColumnXls); break;
                    case "Tenants_TenantName": Column.Tenants_TenantName = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Tenants_TenantName, definitionRow, ColumnXls); break;
                    case "Tenants_Title": Column.Tenants_Title = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Tenants_Title, definitionRow, ColumnXls); break;
                    case "Tenants_Body": Column.Tenants_Body = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Tenants_Body, definitionRow, ColumnXls); break;
                    case "Demos_DemoId": Column.Demos_DemoId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Demos_DemoId, definitionRow, ColumnXls); break;
                    case "Demos_TenantId": Column.Demos_TenantId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Demos_TenantId, definitionRow, ColumnXls); break;
                    case "Demos_Title": Column.Demos_Title = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Demos_Title, definitionRow, ColumnXls); break;
                    case "Demos_Passphrase": Column.Demos_Passphrase = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Demos_Passphrase, definitionRow, ColumnXls); break;
                    case "Demos_MailAddress": Column.Demos_MailAddress = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Demos_MailAddress, definitionRow, ColumnXls); break;
                    case "Demos_Initialized": Column.Demos_Initialized = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Demos_Initialized, definitionRow, ColumnXls); break;
                    case "Demos_TimeLag": Column.Demos_TimeLag = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Demos_TimeLag, definitionRow, ColumnXls); break;
                    case "SysLogs_CreatedTime": Column.SysLogs_CreatedTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SysLogs_CreatedTime, definitionRow, ColumnXls); break;
                    case "SysLogs_SysLogId": Column.SysLogs_SysLogId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SysLogs_SysLogId, definitionRow, ColumnXls); break;
                    case "SysLogs_StartTime": Column.SysLogs_StartTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SysLogs_StartTime, definitionRow, ColumnXls); break;
                    case "SysLogs_EndTime": Column.SysLogs_EndTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SysLogs_EndTime, definitionRow, ColumnXls); break;
                    case "SysLogs_SysLogType": Column.SysLogs_SysLogType = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SysLogs_SysLogType, definitionRow, ColumnXls); break;
                    case "SysLogs_OnAzure": Column.SysLogs_OnAzure = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SysLogs_OnAzure, definitionRow, ColumnXls); break;
                    case "SysLogs_MachineName": Column.SysLogs_MachineName = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SysLogs_MachineName, definitionRow, ColumnXls); break;
                    case "SysLogs_ServiceName": Column.SysLogs_ServiceName = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SysLogs_ServiceName, definitionRow, ColumnXls); break;
                    case "SysLogs_TenantName": Column.SysLogs_TenantName = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SysLogs_TenantName, definitionRow, ColumnXls); break;
                    case "SysLogs_Application": Column.SysLogs_Application = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SysLogs_Application, definitionRow, ColumnXls); break;
                    case "SysLogs_Class": Column.SysLogs_Class = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SysLogs_Class, definitionRow, ColumnXls); break;
                    case "SysLogs_Method": Column.SysLogs_Method = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SysLogs_Method, definitionRow, ColumnXls); break;
                    case "SysLogs_RequestData": Column.SysLogs_RequestData = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SysLogs_RequestData, definitionRow, ColumnXls); break;
                    case "SysLogs_HttpMethod": Column.SysLogs_HttpMethod = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SysLogs_HttpMethod, definitionRow, ColumnXls); break;
                    case "SysLogs_RequestSize": Column.SysLogs_RequestSize = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SysLogs_RequestSize, definitionRow, ColumnXls); break;
                    case "SysLogs_ResponseSize": Column.SysLogs_ResponseSize = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SysLogs_ResponseSize, definitionRow, ColumnXls); break;
                    case "SysLogs_Elapsed": Column.SysLogs_Elapsed = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SysLogs_Elapsed, definitionRow, ColumnXls); break;
                    case "SysLogs_ApplicationAge": Column.SysLogs_ApplicationAge = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SysLogs_ApplicationAge, definitionRow, ColumnXls); break;
                    case "SysLogs_ApplicationRequestInterval": Column.SysLogs_ApplicationRequestInterval = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SysLogs_ApplicationRequestInterval, definitionRow, ColumnXls); break;
                    case "SysLogs_SessionAge": Column.SysLogs_SessionAge = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SysLogs_SessionAge, definitionRow, ColumnXls); break;
                    case "SysLogs_SessionRequestInterval": Column.SysLogs_SessionRequestInterval = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SysLogs_SessionRequestInterval, definitionRow, ColumnXls); break;
                    case "SysLogs_WorkingSet64": Column.SysLogs_WorkingSet64 = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SysLogs_WorkingSet64, definitionRow, ColumnXls); break;
                    case "SysLogs_VirtualMemorySize64": Column.SysLogs_VirtualMemorySize64 = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SysLogs_VirtualMemorySize64, definitionRow, ColumnXls); break;
                    case "SysLogs_ProcessId": Column.SysLogs_ProcessId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SysLogs_ProcessId, definitionRow, ColumnXls); break;
                    case "SysLogs_ProcessName": Column.SysLogs_ProcessName = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SysLogs_ProcessName, definitionRow, ColumnXls); break;
                    case "SysLogs_BasePriority": Column.SysLogs_BasePriority = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SysLogs_BasePriority, definitionRow, ColumnXls); break;
                    case "SysLogs_Url": Column.SysLogs_Url = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SysLogs_Url, definitionRow, ColumnXls); break;
                    case "SysLogs_UrlReferer": Column.SysLogs_UrlReferer = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SysLogs_UrlReferer, definitionRow, ColumnXls); break;
                    case "SysLogs_UserHostName": Column.SysLogs_UserHostName = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SysLogs_UserHostName, definitionRow, ColumnXls); break;
                    case "SysLogs_UserHostAddress": Column.SysLogs_UserHostAddress = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SysLogs_UserHostAddress, definitionRow, ColumnXls); break;
                    case "SysLogs_UserLanguage": Column.SysLogs_UserLanguage = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SysLogs_UserLanguage, definitionRow, ColumnXls); break;
                    case "SysLogs_UserAgent": Column.SysLogs_UserAgent = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SysLogs_UserAgent, definitionRow, ColumnXls); break;
                    case "SysLogs_SessionGuid": Column.SysLogs_SessionGuid = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SysLogs_SessionGuid, definitionRow, ColumnXls); break;
                    case "SysLogs_ErrMessage": Column.SysLogs_ErrMessage = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SysLogs_ErrMessage, definitionRow, ColumnXls); break;
                    case "SysLogs_ErrStackTrace": Column.SysLogs_ErrStackTrace = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SysLogs_ErrStackTrace, definitionRow, ColumnXls); break;
                    case "SysLogs_Title": Column.SysLogs_Title = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SysLogs_Title, definitionRow, ColumnXls); break;
                    case "SysLogs_InDebug": Column.SysLogs_InDebug = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SysLogs_InDebug, definitionRow, ColumnXls); break;
                    case "SysLogs_AssemblyVersion": Column.SysLogs_AssemblyVersion = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SysLogs_AssemblyVersion, definitionRow, ColumnXls); break;
                    case "Depts_TenantId": Column.Depts_TenantId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Depts_TenantId, definitionRow, ColumnXls); break;
                    case "Depts_DeptId": Column.Depts_DeptId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Depts_DeptId, definitionRow, ColumnXls); break;
                    case "Depts_DeptCode": Column.Depts_DeptCode = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Depts_DeptCode, definitionRow, ColumnXls); break;
                    case "Depts_Dept": Column.Depts_Dept = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Depts_Dept, definitionRow, ColumnXls); break;
                    case "Depts_DeptName": Column.Depts_DeptName = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Depts_DeptName, definitionRow, ColumnXls); break;
                    case "Depts_Body": Column.Depts_Body = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Depts_Body, definitionRow, ColumnXls); break;
                    case "Depts_Title": Column.Depts_Title = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Depts_Title, definitionRow, ColumnXls); break;
                    case "Groups_TenantId": Column.Groups_TenantId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Groups_TenantId, definitionRow, ColumnXls); break;
                    case "Groups_GroupId": Column.Groups_GroupId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Groups_GroupId, definitionRow, ColumnXls); break;
                    case "Groups_GroupName": Column.Groups_GroupName = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Groups_GroupName, definitionRow, ColumnXls); break;
                    case "Groups_Body": Column.Groups_Body = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Groups_Body, definitionRow, ColumnXls); break;
                    case "Groups_Title": Column.Groups_Title = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Groups_Title, definitionRow, ColumnXls); break;
                    case "GroupMembers_GroupId": Column.GroupMembers_GroupId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.GroupMembers_GroupId, definitionRow, ColumnXls); break;
                    case "GroupMembers_DeptId": Column.GroupMembers_DeptId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.GroupMembers_DeptId, definitionRow, ColumnXls); break;
                    case "GroupMembers_UserId": Column.GroupMembers_UserId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.GroupMembers_UserId, definitionRow, ColumnXls); break;
                    case "GroupMembers_Admin": Column.GroupMembers_Admin = definitionRow[1].ToString(); SetColumnTable(ColumnTable.GroupMembers_Admin, definitionRow, ColumnXls); break;
                    case "Users_TenantId": Column.Users_TenantId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_TenantId, definitionRow, ColumnXls); break;
                    case "Users_UserId": Column.Users_UserId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_UserId, definitionRow, ColumnXls); break;
                    case "Users_LoginId": Column.Users_LoginId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_LoginId, definitionRow, ColumnXls); break;
                    case "Users_Disabled": Column.Users_Disabled = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_Disabled, definitionRow, ColumnXls); break;
                    case "Users_UserCode": Column.Users_UserCode = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_UserCode, definitionRow, ColumnXls); break;
                    case "Users_Password": Column.Users_Password = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_Password, definitionRow, ColumnXls); break;
                    case "Users_PasswordValidate": Column.Users_PasswordValidate = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_PasswordValidate, definitionRow, ColumnXls); break;
                    case "Users_PasswordDummy": Column.Users_PasswordDummy = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_PasswordDummy, definitionRow, ColumnXls); break;
                    case "Users_RememberMe": Column.Users_RememberMe = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_RememberMe, definitionRow, ColumnXls); break;
                    case "Users_LastName": Column.Users_LastName = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_LastName, definitionRow, ColumnXls); break;
                    case "Users_FirstName": Column.Users_FirstName = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_FirstName, definitionRow, ColumnXls); break;
                    case "Users_FullName1": Column.Users_FullName1 = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_FullName1, definitionRow, ColumnXls); break;
                    case "Users_FullName2": Column.Users_FullName2 = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_FullName2, definitionRow, ColumnXls); break;
                    case "Users_Birthday": Column.Users_Birthday = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_Birthday, definitionRow, ColumnXls); break;
                    case "Users_Gender": Column.Users_Gender = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_Gender, definitionRow, ColumnXls); break;
                    case "Users_Language": Column.Users_Language = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_Language, definitionRow, ColumnXls); break;
                    case "Users_TimeZone": Column.Users_TimeZone = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_TimeZone, definitionRow, ColumnXls); break;
                    case "Users_TimeZoneInfo": Column.Users_TimeZoneInfo = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_TimeZoneInfo, definitionRow, ColumnXls); break;
                    case "Users_DeptId": Column.Users_DeptId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_DeptId, definitionRow, ColumnXls); break;
                    case "Users_Dept": Column.Users_Dept = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_Dept, definitionRow, ColumnXls); break;
                    case "Users_FirstAndLastNameOrder": Column.Users_FirstAndLastNameOrder = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_FirstAndLastNameOrder, definitionRow, ColumnXls); break;
                    case "Users_Title": Column.Users_Title = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_Title, definitionRow, ColumnXls); break;
                    case "Users_LastLoginTime": Column.Users_LastLoginTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_LastLoginTime, definitionRow, ColumnXls); break;
                    case "Users_PasswordExpirationTime": Column.Users_PasswordExpirationTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_PasswordExpirationTime, definitionRow, ColumnXls); break;
                    case "Users_PasswordChangeTime": Column.Users_PasswordChangeTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_PasswordChangeTime, definitionRow, ColumnXls); break;
                    case "Users_NumberOfLogins": Column.Users_NumberOfLogins = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_NumberOfLogins, definitionRow, ColumnXls); break;
                    case "Users_NumberOfDenial": Column.Users_NumberOfDenial = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_NumberOfDenial, definitionRow, ColumnXls); break;
                    case "Users_TenantManager": Column.Users_TenantManager = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_TenantManager, definitionRow, ColumnXls); break;
                    case "Users_ServiceManager": Column.Users_ServiceManager = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_ServiceManager, definitionRow, ColumnXls); break;
                    case "Users_Developer": Column.Users_Developer = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_Developer, definitionRow, ColumnXls); break;
                    case "Users_OldPassword": Column.Users_OldPassword = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_OldPassword, definitionRow, ColumnXls); break;
                    case "Users_ChangedPassword": Column.Users_ChangedPassword = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_ChangedPassword, definitionRow, ColumnXls); break;
                    case "Users_ChangedPasswordValidator": Column.Users_ChangedPasswordValidator = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_ChangedPasswordValidator, definitionRow, ColumnXls); break;
                    case "Users_AfterResetPassword": Column.Users_AfterResetPassword = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_AfterResetPassword, definitionRow, ColumnXls); break;
                    case "Users_AfterResetPasswordValidator": Column.Users_AfterResetPasswordValidator = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_AfterResetPasswordValidator, definitionRow, ColumnXls); break;
                    case "Users_MailAddresses": Column.Users_MailAddresses = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_MailAddresses, definitionRow, ColumnXls); break;
                    case "Users_DemoMailAddress": Column.Users_DemoMailAddress = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_DemoMailAddress, definitionRow, ColumnXls); break;
                    case "Users_SessionGuid": Column.Users_SessionGuid = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_SessionGuid, definitionRow, ColumnXls); break;
                    case "MailAddresses_OwnerId": Column.MailAddresses_OwnerId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.MailAddresses_OwnerId, definitionRow, ColumnXls); break;
                    case "MailAddresses_OwnerType": Column.MailAddresses_OwnerType = definitionRow[1].ToString(); SetColumnTable(ColumnTable.MailAddresses_OwnerType, definitionRow, ColumnXls); break;
                    case "MailAddresses_MailAddressId": Column.MailAddresses_MailAddressId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.MailAddresses_MailAddressId, definitionRow, ColumnXls); break;
                    case "MailAddresses_MailAddress": Column.MailAddresses_MailAddress = definitionRow[1].ToString(); SetColumnTable(ColumnTable.MailAddresses_MailAddress, definitionRow, ColumnXls); break;
                    case "MailAddresses_Title": Column.MailAddresses_Title = definitionRow[1].ToString(); SetColumnTable(ColumnTable.MailAddresses_Title, definitionRow, ColumnXls); break;
                    case "Permissions_ReferenceType": Column.Permissions_ReferenceType = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Permissions_ReferenceType, definitionRow, ColumnXls); break;
                    case "Permissions_ReferenceId": Column.Permissions_ReferenceId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Permissions_ReferenceId, definitionRow, ColumnXls); break;
                    case "Permissions_DeptId": Column.Permissions_DeptId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Permissions_DeptId, definitionRow, ColumnXls); break;
                    case "Permissions_GroupId": Column.Permissions_GroupId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Permissions_GroupId, definitionRow, ColumnXls); break;
                    case "Permissions_UserId": Column.Permissions_UserId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Permissions_UserId, definitionRow, ColumnXls); break;
                    case "Permissions_DeptName": Column.Permissions_DeptName = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Permissions_DeptName, definitionRow, ColumnXls); break;
                    case "Permissions_GroupName": Column.Permissions_GroupName = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Permissions_GroupName, definitionRow, ColumnXls); break;
                    case "Permissions_FullName1": Column.Permissions_FullName1 = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Permissions_FullName1, definitionRow, ColumnXls); break;
                    case "Permissions_FullName2": Column.Permissions_FullName2 = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Permissions_FullName2, definitionRow, ColumnXls); break;
                    case "Permissions_FirstAndLastNameOrder": Column.Permissions_FirstAndLastNameOrder = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Permissions_FirstAndLastNameOrder, definitionRow, ColumnXls); break;
                    case "Permissions_PermissionType": Column.Permissions_PermissionType = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Permissions_PermissionType, definitionRow, ColumnXls); break;
                    case "OutgoingMails_ReferenceType": Column.OutgoingMails_ReferenceType = definitionRow[1].ToString(); SetColumnTable(ColumnTable.OutgoingMails_ReferenceType, definitionRow, ColumnXls); break;
                    case "OutgoingMails_ReferenceId": Column.OutgoingMails_ReferenceId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.OutgoingMails_ReferenceId, definitionRow, ColumnXls); break;
                    case "OutgoingMails_ReferenceVer": Column.OutgoingMails_ReferenceVer = definitionRow[1].ToString(); SetColumnTable(ColumnTable.OutgoingMails_ReferenceVer, definitionRow, ColumnXls); break;
                    case "OutgoingMails_OutgoingMailId": Column.OutgoingMails_OutgoingMailId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.OutgoingMails_OutgoingMailId, definitionRow, ColumnXls); break;
                    case "OutgoingMails_Host": Column.OutgoingMails_Host = definitionRow[1].ToString(); SetColumnTable(ColumnTable.OutgoingMails_Host, definitionRow, ColumnXls); break;
                    case "OutgoingMails_Port": Column.OutgoingMails_Port = definitionRow[1].ToString(); SetColumnTable(ColumnTable.OutgoingMails_Port, definitionRow, ColumnXls); break;
                    case "OutgoingMails_From": Column.OutgoingMails_From = definitionRow[1].ToString(); SetColumnTable(ColumnTable.OutgoingMails_From, definitionRow, ColumnXls); break;
                    case "OutgoingMails_To": Column.OutgoingMails_To = definitionRow[1].ToString(); SetColumnTable(ColumnTable.OutgoingMails_To, definitionRow, ColumnXls); break;
                    case "OutgoingMails_Cc": Column.OutgoingMails_Cc = definitionRow[1].ToString(); SetColumnTable(ColumnTable.OutgoingMails_Cc, definitionRow, ColumnXls); break;
                    case "OutgoingMails_Bcc": Column.OutgoingMails_Bcc = definitionRow[1].ToString(); SetColumnTable(ColumnTable.OutgoingMails_Bcc, definitionRow, ColumnXls); break;
                    case "OutgoingMails_Title": Column.OutgoingMails_Title = definitionRow[1].ToString(); SetColumnTable(ColumnTable.OutgoingMails_Title, definitionRow, ColumnXls); break;
                    case "OutgoingMails_Body": Column.OutgoingMails_Body = definitionRow[1].ToString(); SetColumnTable(ColumnTable.OutgoingMails_Body, definitionRow, ColumnXls); break;
                    case "OutgoingMails_SentTime": Column.OutgoingMails_SentTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable.OutgoingMails_SentTime, definitionRow, ColumnXls); break;
                    case "OutgoingMails_DestinationSearchRange": Column.OutgoingMails_DestinationSearchRange = definitionRow[1].ToString(); SetColumnTable(ColumnTable.OutgoingMails_DestinationSearchRange, definitionRow, ColumnXls); break;
                    case "OutgoingMails_DestinationSearchText": Column.OutgoingMails_DestinationSearchText = definitionRow[1].ToString(); SetColumnTable(ColumnTable.OutgoingMails_DestinationSearchText, definitionRow, ColumnXls); break;
                    case "SearchIndexes_Word": Column.SearchIndexes_Word = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SearchIndexes_Word, definitionRow, ColumnXls); break;
                    case "SearchIndexes_ReferenceId": Column.SearchIndexes_ReferenceId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SearchIndexes_ReferenceId, definitionRow, ColumnXls); break;
                    case "SearchIndexes_Priority": Column.SearchIndexes_Priority = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SearchIndexes_Priority, definitionRow, ColumnXls); break;
                    case "SearchIndexes_ReferenceType": Column.SearchIndexes_ReferenceType = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SearchIndexes_ReferenceType, definitionRow, ColumnXls); break;
                    case "SearchIndexes_Title": Column.SearchIndexes_Title = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SearchIndexes_Title, definitionRow, ColumnXls); break;
                    case "SearchIndexes_Subset": Column.SearchIndexes_Subset = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SearchIndexes_Subset, definitionRow, ColumnXls); break;
                    case "SearchIndexes_PermissionType": Column.SearchIndexes_PermissionType = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SearchIndexes_PermissionType, definitionRow, ColumnXls); break;
                    case "Items_ReferenceId": Column.Items_ReferenceId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Items_ReferenceId, definitionRow, ColumnXls); break;
                    case "Items_ReferenceType": Column.Items_ReferenceType = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Items_ReferenceType, definitionRow, ColumnXls); break;
                    case "Items_SiteId": Column.Items_SiteId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Items_SiteId, definitionRow, ColumnXls); break;
                    case "Items_Title": Column.Items_Title = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Items_Title, definitionRow, ColumnXls); break;
                    case "Items_Site": Column.Items_Site = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Items_Site, definitionRow, ColumnXls); break;
                    case "Sites_TenantId": Column.Sites_TenantId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Sites_TenantId, definitionRow, ColumnXls); break;
                    case "Sites_SiteId": Column.Sites_SiteId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Sites_SiteId, definitionRow, ColumnXls); break;
                    case "Sites_ReferenceType": Column.Sites_ReferenceType = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Sites_ReferenceType, definitionRow, ColumnXls); break;
                    case "Sites_ParentId": Column.Sites_ParentId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Sites_ParentId, definitionRow, ColumnXls); break;
                    case "Sites_InheritPermission": Column.Sites_InheritPermission = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Sites_InheritPermission, definitionRow, ColumnXls); break;
                    case "Sites_SiteSettings": Column.Sites_SiteSettings = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Sites_SiteSettings, definitionRow, ColumnXls); break;
                    case "Sites_Ancestors": Column.Sites_Ancestors = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Sites_Ancestors, definitionRow, ColumnXls); break;
                    case "Sites_PermissionSourceCollection": Column.Sites_PermissionSourceCollection = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Sites_PermissionSourceCollection, definitionRow, ColumnXls); break;
                    case "Sites_PermissionDestinationCollection": Column.Sites_PermissionDestinationCollection = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Sites_PermissionDestinationCollection, definitionRow, ColumnXls); break;
                    case "Sites_SiteMenu": Column.Sites_SiteMenu = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Sites_SiteMenu, definitionRow, ColumnXls); break;
                    case "Sites_MonitorChangesColumns": Column.Sites_MonitorChangesColumns = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Sites_MonitorChangesColumns, definitionRow, ColumnXls); break;
                    case "Sites_TitleColumns": Column.Sites_TitleColumns = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Sites_TitleColumns, definitionRow, ColumnXls); break;
                    case "Orders_ReferenceId": Column.Orders_ReferenceId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Orders_ReferenceId, definitionRow, ColumnXls); break;
                    case "Orders_ReferenceType": Column.Orders_ReferenceType = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Orders_ReferenceType, definitionRow, ColumnXls); break;
                    case "Orders_OwnerId": Column.Orders_OwnerId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Orders_OwnerId, definitionRow, ColumnXls); break;
                    case "Orders_Data": Column.Orders_Data = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Orders_Data, definitionRow, ColumnXls); break;
                    case "ExportSettings_ReferenceType": Column.ExportSettings_ReferenceType = definitionRow[1].ToString(); SetColumnTable(ColumnTable.ExportSettings_ReferenceType, definitionRow, ColumnXls); break;
                    case "ExportSettings_ReferenceId": Column.ExportSettings_ReferenceId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.ExportSettings_ReferenceId, definitionRow, ColumnXls); break;
                    case "ExportSettings_Title": Column.ExportSettings_Title = definitionRow[1].ToString(); SetColumnTable(ColumnTable.ExportSettings_Title, definitionRow, ColumnXls); break;
                    case "ExportSettings_ExportSettingId": Column.ExportSettings_ExportSettingId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.ExportSettings_ExportSettingId, definitionRow, ColumnXls); break;
                    case "ExportSettings_AddHeader": Column.ExportSettings_AddHeader = definitionRow[1].ToString(); SetColumnTable(ColumnTable.ExportSettings_AddHeader, definitionRow, ColumnXls); break;
                    case "ExportSettings_ExportColumns": Column.ExportSettings_ExportColumns = definitionRow[1].ToString(); SetColumnTable(ColumnTable.ExportSettings_ExportColumns, definitionRow, ColumnXls); break;
                    case "Links_DestinationId": Column.Links_DestinationId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Links_DestinationId, definitionRow, ColumnXls); break;
                    case "Links_SourceId": Column.Links_SourceId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Links_SourceId, definitionRow, ColumnXls); break;
                    case "Links_ReferenceType": Column.Links_ReferenceType = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Links_ReferenceType, definitionRow, ColumnXls); break;
                    case "Links_SiteId": Column.Links_SiteId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Links_SiteId, definitionRow, ColumnXls); break;
                    case "Links_Title": Column.Links_Title = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Links_Title, definitionRow, ColumnXls); break;
                    case "Links_Subset": Column.Links_Subset = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Links_Subset, definitionRow, ColumnXls); break;
                    case "Links_SiteTitle": Column.Links_SiteTitle = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Links_SiteTitle, definitionRow, ColumnXls); break;
                    case "Binaries_ReferenceId": Column.Binaries_ReferenceId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Binaries_ReferenceId, definitionRow, ColumnXls); break;
                    case "Binaries_BinaryId": Column.Binaries_BinaryId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Binaries_BinaryId, definitionRow, ColumnXls); break;
                    case "Binaries_BinaryType": Column.Binaries_BinaryType = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Binaries_BinaryType, definitionRow, ColumnXls); break;
                    case "Binaries_Title": Column.Binaries_Title = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Binaries_Title, definitionRow, ColumnXls); break;
                    case "Binaries_Body": Column.Binaries_Body = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Binaries_Body, definitionRow, ColumnXls); break;
                    case "Binaries_Bin": Column.Binaries_Bin = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Binaries_Bin, definitionRow, ColumnXls); break;
                    case "Binaries_Thumbnail": Column.Binaries_Thumbnail = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Binaries_Thumbnail, definitionRow, ColumnXls); break;
                    case "Binaries_Icon": Column.Binaries_Icon = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Binaries_Icon, definitionRow, ColumnXls); break;
                    case "Binaries_FileName": Column.Binaries_FileName = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Binaries_FileName, definitionRow, ColumnXls); break;
                    case "Binaries_Extension": Column.Binaries_Extension = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Binaries_Extension, definitionRow, ColumnXls); break;
                    case "Binaries_Size": Column.Binaries_Size = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Binaries_Size, definitionRow, ColumnXls); break;
                    case "Binaries_BinarySettings": Column.Binaries_BinarySettings = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Binaries_BinarySettings, definitionRow, ColumnXls); break;
                    case "Issues_IssueId": Column.Issues_IssueId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_IssueId, definitionRow, ColumnXls); break;
                    case "Issues_StartTime": Column.Issues_StartTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_StartTime, definitionRow, ColumnXls); break;
                    case "Issues_CompletionTime": Column.Issues_CompletionTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_CompletionTime, definitionRow, ColumnXls); break;
                    case "Issues_WorkValue": Column.Issues_WorkValue = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_WorkValue, definitionRow, ColumnXls); break;
                    case "Issues_ProgressRate": Column.Issues_ProgressRate = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_ProgressRate, definitionRow, ColumnXls); break;
                    case "Issues_RemainingWorkValue": Column.Issues_RemainingWorkValue = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_RemainingWorkValue, definitionRow, ColumnXls); break;
                    case "Issues_Status": Column.Issues_Status = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_Status, definitionRow, ColumnXls); break;
                    case "Issues_Manager": Column.Issues_Manager = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_Manager, definitionRow, ColumnXls); break;
                    case "Issues_Owner": Column.Issues_Owner = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_Owner, definitionRow, ColumnXls); break;
                    case "Issues_ClassA": Column.Issues_ClassA = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_ClassA, definitionRow, ColumnXls); break;
                    case "Issues_ClassB": Column.Issues_ClassB = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_ClassB, definitionRow, ColumnXls); break;
                    case "Issues_ClassC": Column.Issues_ClassC = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_ClassC, definitionRow, ColumnXls); break;
                    case "Issues_ClassD": Column.Issues_ClassD = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_ClassD, definitionRow, ColumnXls); break;
                    case "Issues_ClassE": Column.Issues_ClassE = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_ClassE, definitionRow, ColumnXls); break;
                    case "Issues_ClassF": Column.Issues_ClassF = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_ClassF, definitionRow, ColumnXls); break;
                    case "Issues_ClassG": Column.Issues_ClassG = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_ClassG, definitionRow, ColumnXls); break;
                    case "Issues_ClassH": Column.Issues_ClassH = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_ClassH, definitionRow, ColumnXls); break;
                    case "Issues_ClassI": Column.Issues_ClassI = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_ClassI, definitionRow, ColumnXls); break;
                    case "Issues_ClassJ": Column.Issues_ClassJ = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_ClassJ, definitionRow, ColumnXls); break;
                    case "Issues_ClassK": Column.Issues_ClassK = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_ClassK, definitionRow, ColumnXls); break;
                    case "Issues_ClassL": Column.Issues_ClassL = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_ClassL, definitionRow, ColumnXls); break;
                    case "Issues_ClassM": Column.Issues_ClassM = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_ClassM, definitionRow, ColumnXls); break;
                    case "Issues_ClassN": Column.Issues_ClassN = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_ClassN, definitionRow, ColumnXls); break;
                    case "Issues_ClassO": Column.Issues_ClassO = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_ClassO, definitionRow, ColumnXls); break;
                    case "Issues_ClassP": Column.Issues_ClassP = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_ClassP, definitionRow, ColumnXls); break;
                    case "Issues_ClassQ": Column.Issues_ClassQ = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_ClassQ, definitionRow, ColumnXls); break;
                    case "Issues_ClassR": Column.Issues_ClassR = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_ClassR, definitionRow, ColumnXls); break;
                    case "Issues_ClassS": Column.Issues_ClassS = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_ClassS, definitionRow, ColumnXls); break;
                    case "Issues_ClassT": Column.Issues_ClassT = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_ClassT, definitionRow, ColumnXls); break;
                    case "Issues_ClassU": Column.Issues_ClassU = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_ClassU, definitionRow, ColumnXls); break;
                    case "Issues_ClassV": Column.Issues_ClassV = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_ClassV, definitionRow, ColumnXls); break;
                    case "Issues_ClassW": Column.Issues_ClassW = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_ClassW, definitionRow, ColumnXls); break;
                    case "Issues_ClassX": Column.Issues_ClassX = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_ClassX, definitionRow, ColumnXls); break;
                    case "Issues_ClassY": Column.Issues_ClassY = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_ClassY, definitionRow, ColumnXls); break;
                    case "Issues_ClassZ": Column.Issues_ClassZ = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_ClassZ, definitionRow, ColumnXls); break;
                    case "Issues_NumA": Column.Issues_NumA = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_NumA, definitionRow, ColumnXls); break;
                    case "Issues_NumB": Column.Issues_NumB = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_NumB, definitionRow, ColumnXls); break;
                    case "Issues_NumC": Column.Issues_NumC = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_NumC, definitionRow, ColumnXls); break;
                    case "Issues_NumD": Column.Issues_NumD = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_NumD, definitionRow, ColumnXls); break;
                    case "Issues_NumE": Column.Issues_NumE = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_NumE, definitionRow, ColumnXls); break;
                    case "Issues_NumF": Column.Issues_NumF = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_NumF, definitionRow, ColumnXls); break;
                    case "Issues_NumG": Column.Issues_NumG = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_NumG, definitionRow, ColumnXls); break;
                    case "Issues_NumH": Column.Issues_NumH = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_NumH, definitionRow, ColumnXls); break;
                    case "Issues_NumI": Column.Issues_NumI = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_NumI, definitionRow, ColumnXls); break;
                    case "Issues_NumJ": Column.Issues_NumJ = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_NumJ, definitionRow, ColumnXls); break;
                    case "Issues_NumK": Column.Issues_NumK = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_NumK, definitionRow, ColumnXls); break;
                    case "Issues_NumL": Column.Issues_NumL = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_NumL, definitionRow, ColumnXls); break;
                    case "Issues_NumM": Column.Issues_NumM = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_NumM, definitionRow, ColumnXls); break;
                    case "Issues_NumN": Column.Issues_NumN = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_NumN, definitionRow, ColumnXls); break;
                    case "Issues_NumO": Column.Issues_NumO = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_NumO, definitionRow, ColumnXls); break;
                    case "Issues_NumP": Column.Issues_NumP = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_NumP, definitionRow, ColumnXls); break;
                    case "Issues_NumQ": Column.Issues_NumQ = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_NumQ, definitionRow, ColumnXls); break;
                    case "Issues_NumR": Column.Issues_NumR = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_NumR, definitionRow, ColumnXls); break;
                    case "Issues_NumS": Column.Issues_NumS = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_NumS, definitionRow, ColumnXls); break;
                    case "Issues_NumT": Column.Issues_NumT = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_NumT, definitionRow, ColumnXls); break;
                    case "Issues_NumU": Column.Issues_NumU = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_NumU, definitionRow, ColumnXls); break;
                    case "Issues_NumV": Column.Issues_NumV = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_NumV, definitionRow, ColumnXls); break;
                    case "Issues_NumW": Column.Issues_NumW = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_NumW, definitionRow, ColumnXls); break;
                    case "Issues_NumX": Column.Issues_NumX = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_NumX, definitionRow, ColumnXls); break;
                    case "Issues_NumY": Column.Issues_NumY = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_NumY, definitionRow, ColumnXls); break;
                    case "Issues_NumZ": Column.Issues_NumZ = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_NumZ, definitionRow, ColumnXls); break;
                    case "Issues_DateA": Column.Issues_DateA = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DateA, definitionRow, ColumnXls); break;
                    case "Issues_DateB": Column.Issues_DateB = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DateB, definitionRow, ColumnXls); break;
                    case "Issues_DateC": Column.Issues_DateC = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DateC, definitionRow, ColumnXls); break;
                    case "Issues_DateD": Column.Issues_DateD = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DateD, definitionRow, ColumnXls); break;
                    case "Issues_DateE": Column.Issues_DateE = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DateE, definitionRow, ColumnXls); break;
                    case "Issues_DateF": Column.Issues_DateF = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DateF, definitionRow, ColumnXls); break;
                    case "Issues_DateG": Column.Issues_DateG = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DateG, definitionRow, ColumnXls); break;
                    case "Issues_DateH": Column.Issues_DateH = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DateH, definitionRow, ColumnXls); break;
                    case "Issues_DateI": Column.Issues_DateI = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DateI, definitionRow, ColumnXls); break;
                    case "Issues_DateJ": Column.Issues_DateJ = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DateJ, definitionRow, ColumnXls); break;
                    case "Issues_DateK": Column.Issues_DateK = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DateK, definitionRow, ColumnXls); break;
                    case "Issues_DateL": Column.Issues_DateL = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DateL, definitionRow, ColumnXls); break;
                    case "Issues_DateM": Column.Issues_DateM = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DateM, definitionRow, ColumnXls); break;
                    case "Issues_DateN": Column.Issues_DateN = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DateN, definitionRow, ColumnXls); break;
                    case "Issues_DateO": Column.Issues_DateO = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DateO, definitionRow, ColumnXls); break;
                    case "Issues_DateP": Column.Issues_DateP = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DateP, definitionRow, ColumnXls); break;
                    case "Issues_DateQ": Column.Issues_DateQ = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DateQ, definitionRow, ColumnXls); break;
                    case "Issues_DateR": Column.Issues_DateR = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DateR, definitionRow, ColumnXls); break;
                    case "Issues_DateS": Column.Issues_DateS = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DateS, definitionRow, ColumnXls); break;
                    case "Issues_DateT": Column.Issues_DateT = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DateT, definitionRow, ColumnXls); break;
                    case "Issues_DateU": Column.Issues_DateU = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DateU, definitionRow, ColumnXls); break;
                    case "Issues_DateV": Column.Issues_DateV = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DateV, definitionRow, ColumnXls); break;
                    case "Issues_DateW": Column.Issues_DateW = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DateW, definitionRow, ColumnXls); break;
                    case "Issues_DateX": Column.Issues_DateX = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DateX, definitionRow, ColumnXls); break;
                    case "Issues_DateY": Column.Issues_DateY = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DateY, definitionRow, ColumnXls); break;
                    case "Issues_DateZ": Column.Issues_DateZ = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DateZ, definitionRow, ColumnXls); break;
                    case "Issues_DescriptionA": Column.Issues_DescriptionA = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DescriptionA, definitionRow, ColumnXls); break;
                    case "Issues_DescriptionB": Column.Issues_DescriptionB = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DescriptionB, definitionRow, ColumnXls); break;
                    case "Issues_DescriptionC": Column.Issues_DescriptionC = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DescriptionC, definitionRow, ColumnXls); break;
                    case "Issues_DescriptionD": Column.Issues_DescriptionD = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DescriptionD, definitionRow, ColumnXls); break;
                    case "Issues_DescriptionE": Column.Issues_DescriptionE = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DescriptionE, definitionRow, ColumnXls); break;
                    case "Issues_DescriptionF": Column.Issues_DescriptionF = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DescriptionF, definitionRow, ColumnXls); break;
                    case "Issues_DescriptionG": Column.Issues_DescriptionG = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DescriptionG, definitionRow, ColumnXls); break;
                    case "Issues_DescriptionH": Column.Issues_DescriptionH = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DescriptionH, definitionRow, ColumnXls); break;
                    case "Issues_DescriptionI": Column.Issues_DescriptionI = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DescriptionI, definitionRow, ColumnXls); break;
                    case "Issues_DescriptionJ": Column.Issues_DescriptionJ = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DescriptionJ, definitionRow, ColumnXls); break;
                    case "Issues_DescriptionK": Column.Issues_DescriptionK = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DescriptionK, definitionRow, ColumnXls); break;
                    case "Issues_DescriptionL": Column.Issues_DescriptionL = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DescriptionL, definitionRow, ColumnXls); break;
                    case "Issues_DescriptionM": Column.Issues_DescriptionM = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DescriptionM, definitionRow, ColumnXls); break;
                    case "Issues_DescriptionN": Column.Issues_DescriptionN = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DescriptionN, definitionRow, ColumnXls); break;
                    case "Issues_DescriptionO": Column.Issues_DescriptionO = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DescriptionO, definitionRow, ColumnXls); break;
                    case "Issues_DescriptionP": Column.Issues_DescriptionP = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DescriptionP, definitionRow, ColumnXls); break;
                    case "Issues_DescriptionQ": Column.Issues_DescriptionQ = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DescriptionQ, definitionRow, ColumnXls); break;
                    case "Issues_DescriptionR": Column.Issues_DescriptionR = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DescriptionR, definitionRow, ColumnXls); break;
                    case "Issues_DescriptionS": Column.Issues_DescriptionS = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DescriptionS, definitionRow, ColumnXls); break;
                    case "Issues_DescriptionT": Column.Issues_DescriptionT = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DescriptionT, definitionRow, ColumnXls); break;
                    case "Issues_DescriptionU": Column.Issues_DescriptionU = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DescriptionU, definitionRow, ColumnXls); break;
                    case "Issues_DescriptionV": Column.Issues_DescriptionV = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DescriptionV, definitionRow, ColumnXls); break;
                    case "Issues_DescriptionW": Column.Issues_DescriptionW = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DescriptionW, definitionRow, ColumnXls); break;
                    case "Issues_DescriptionX": Column.Issues_DescriptionX = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DescriptionX, definitionRow, ColumnXls); break;
                    case "Issues_DescriptionY": Column.Issues_DescriptionY = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DescriptionY, definitionRow, ColumnXls); break;
                    case "Issues_DescriptionZ": Column.Issues_DescriptionZ = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_DescriptionZ, definitionRow, ColumnXls); break;
                    case "Issues_CheckA": Column.Issues_CheckA = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_CheckA, definitionRow, ColumnXls); break;
                    case "Issues_CheckB": Column.Issues_CheckB = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_CheckB, definitionRow, ColumnXls); break;
                    case "Issues_CheckC": Column.Issues_CheckC = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_CheckC, definitionRow, ColumnXls); break;
                    case "Issues_CheckD": Column.Issues_CheckD = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_CheckD, definitionRow, ColumnXls); break;
                    case "Issues_CheckE": Column.Issues_CheckE = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_CheckE, definitionRow, ColumnXls); break;
                    case "Issues_CheckF": Column.Issues_CheckF = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_CheckF, definitionRow, ColumnXls); break;
                    case "Issues_CheckG": Column.Issues_CheckG = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_CheckG, definitionRow, ColumnXls); break;
                    case "Issues_CheckH": Column.Issues_CheckH = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_CheckH, definitionRow, ColumnXls); break;
                    case "Issues_CheckI": Column.Issues_CheckI = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_CheckI, definitionRow, ColumnXls); break;
                    case "Issues_CheckJ": Column.Issues_CheckJ = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_CheckJ, definitionRow, ColumnXls); break;
                    case "Issues_CheckK": Column.Issues_CheckK = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_CheckK, definitionRow, ColumnXls); break;
                    case "Issues_CheckL": Column.Issues_CheckL = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_CheckL, definitionRow, ColumnXls); break;
                    case "Issues_CheckM": Column.Issues_CheckM = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_CheckM, definitionRow, ColumnXls); break;
                    case "Issues_CheckN": Column.Issues_CheckN = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_CheckN, definitionRow, ColumnXls); break;
                    case "Issues_CheckO": Column.Issues_CheckO = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_CheckO, definitionRow, ColumnXls); break;
                    case "Issues_CheckP": Column.Issues_CheckP = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_CheckP, definitionRow, ColumnXls); break;
                    case "Issues_CheckQ": Column.Issues_CheckQ = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_CheckQ, definitionRow, ColumnXls); break;
                    case "Issues_CheckR": Column.Issues_CheckR = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_CheckR, definitionRow, ColumnXls); break;
                    case "Issues_CheckS": Column.Issues_CheckS = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_CheckS, definitionRow, ColumnXls); break;
                    case "Issues_CheckT": Column.Issues_CheckT = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_CheckT, definitionRow, ColumnXls); break;
                    case "Issues_CheckU": Column.Issues_CheckU = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_CheckU, definitionRow, ColumnXls); break;
                    case "Issues_CheckV": Column.Issues_CheckV = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_CheckV, definitionRow, ColumnXls); break;
                    case "Issues_CheckW": Column.Issues_CheckW = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_CheckW, definitionRow, ColumnXls); break;
                    case "Issues_CheckX": Column.Issues_CheckX = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_CheckX, definitionRow, ColumnXls); break;
                    case "Issues_CheckY": Column.Issues_CheckY = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_CheckY, definitionRow, ColumnXls); break;
                    case "Issues_CheckZ": Column.Issues_CheckZ = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_CheckZ, definitionRow, ColumnXls); break;
                    case "Results_ResultId": Column.Results_ResultId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_ResultId, definitionRow, ColumnXls); break;
                    case "Results_Title": Column.Results_Title = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_Title, definitionRow, ColumnXls); break;
                    case "Results_Status": Column.Results_Status = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_Status, definitionRow, ColumnXls); break;
                    case "Results_Manager": Column.Results_Manager = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_Manager, definitionRow, ColumnXls); break;
                    case "Results_Owner": Column.Results_Owner = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_Owner, definitionRow, ColumnXls); break;
                    case "Results_ClassA": Column.Results_ClassA = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_ClassA, definitionRow, ColumnXls); break;
                    case "Results_ClassB": Column.Results_ClassB = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_ClassB, definitionRow, ColumnXls); break;
                    case "Results_ClassC": Column.Results_ClassC = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_ClassC, definitionRow, ColumnXls); break;
                    case "Results_ClassD": Column.Results_ClassD = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_ClassD, definitionRow, ColumnXls); break;
                    case "Results_ClassE": Column.Results_ClassE = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_ClassE, definitionRow, ColumnXls); break;
                    case "Results_ClassF": Column.Results_ClassF = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_ClassF, definitionRow, ColumnXls); break;
                    case "Results_ClassG": Column.Results_ClassG = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_ClassG, definitionRow, ColumnXls); break;
                    case "Results_ClassH": Column.Results_ClassH = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_ClassH, definitionRow, ColumnXls); break;
                    case "Results_ClassI": Column.Results_ClassI = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_ClassI, definitionRow, ColumnXls); break;
                    case "Results_ClassJ": Column.Results_ClassJ = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_ClassJ, definitionRow, ColumnXls); break;
                    case "Results_ClassK": Column.Results_ClassK = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_ClassK, definitionRow, ColumnXls); break;
                    case "Results_ClassL": Column.Results_ClassL = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_ClassL, definitionRow, ColumnXls); break;
                    case "Results_ClassM": Column.Results_ClassM = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_ClassM, definitionRow, ColumnXls); break;
                    case "Results_ClassN": Column.Results_ClassN = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_ClassN, definitionRow, ColumnXls); break;
                    case "Results_ClassO": Column.Results_ClassO = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_ClassO, definitionRow, ColumnXls); break;
                    case "Results_ClassP": Column.Results_ClassP = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_ClassP, definitionRow, ColumnXls); break;
                    case "Results_ClassQ": Column.Results_ClassQ = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_ClassQ, definitionRow, ColumnXls); break;
                    case "Results_ClassR": Column.Results_ClassR = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_ClassR, definitionRow, ColumnXls); break;
                    case "Results_ClassS": Column.Results_ClassS = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_ClassS, definitionRow, ColumnXls); break;
                    case "Results_ClassT": Column.Results_ClassT = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_ClassT, definitionRow, ColumnXls); break;
                    case "Results_ClassU": Column.Results_ClassU = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_ClassU, definitionRow, ColumnXls); break;
                    case "Results_ClassV": Column.Results_ClassV = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_ClassV, definitionRow, ColumnXls); break;
                    case "Results_ClassW": Column.Results_ClassW = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_ClassW, definitionRow, ColumnXls); break;
                    case "Results_ClassX": Column.Results_ClassX = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_ClassX, definitionRow, ColumnXls); break;
                    case "Results_ClassY": Column.Results_ClassY = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_ClassY, definitionRow, ColumnXls); break;
                    case "Results_ClassZ": Column.Results_ClassZ = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_ClassZ, definitionRow, ColumnXls); break;
                    case "Results_NumA": Column.Results_NumA = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_NumA, definitionRow, ColumnXls); break;
                    case "Results_NumB": Column.Results_NumB = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_NumB, definitionRow, ColumnXls); break;
                    case "Results_NumC": Column.Results_NumC = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_NumC, definitionRow, ColumnXls); break;
                    case "Results_NumD": Column.Results_NumD = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_NumD, definitionRow, ColumnXls); break;
                    case "Results_NumE": Column.Results_NumE = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_NumE, definitionRow, ColumnXls); break;
                    case "Results_NumF": Column.Results_NumF = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_NumF, definitionRow, ColumnXls); break;
                    case "Results_NumG": Column.Results_NumG = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_NumG, definitionRow, ColumnXls); break;
                    case "Results_NumH": Column.Results_NumH = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_NumH, definitionRow, ColumnXls); break;
                    case "Results_NumI": Column.Results_NumI = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_NumI, definitionRow, ColumnXls); break;
                    case "Results_NumJ": Column.Results_NumJ = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_NumJ, definitionRow, ColumnXls); break;
                    case "Results_NumK": Column.Results_NumK = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_NumK, definitionRow, ColumnXls); break;
                    case "Results_NumL": Column.Results_NumL = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_NumL, definitionRow, ColumnXls); break;
                    case "Results_NumM": Column.Results_NumM = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_NumM, definitionRow, ColumnXls); break;
                    case "Results_NumN": Column.Results_NumN = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_NumN, definitionRow, ColumnXls); break;
                    case "Results_NumO": Column.Results_NumO = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_NumO, definitionRow, ColumnXls); break;
                    case "Results_NumP": Column.Results_NumP = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_NumP, definitionRow, ColumnXls); break;
                    case "Results_NumQ": Column.Results_NumQ = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_NumQ, definitionRow, ColumnXls); break;
                    case "Results_NumR": Column.Results_NumR = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_NumR, definitionRow, ColumnXls); break;
                    case "Results_NumS": Column.Results_NumS = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_NumS, definitionRow, ColumnXls); break;
                    case "Results_NumT": Column.Results_NumT = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_NumT, definitionRow, ColumnXls); break;
                    case "Results_NumU": Column.Results_NumU = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_NumU, definitionRow, ColumnXls); break;
                    case "Results_NumV": Column.Results_NumV = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_NumV, definitionRow, ColumnXls); break;
                    case "Results_NumW": Column.Results_NumW = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_NumW, definitionRow, ColumnXls); break;
                    case "Results_NumX": Column.Results_NumX = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_NumX, definitionRow, ColumnXls); break;
                    case "Results_NumY": Column.Results_NumY = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_NumY, definitionRow, ColumnXls); break;
                    case "Results_NumZ": Column.Results_NumZ = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_NumZ, definitionRow, ColumnXls); break;
                    case "Results_DateA": Column.Results_DateA = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DateA, definitionRow, ColumnXls); break;
                    case "Results_DateB": Column.Results_DateB = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DateB, definitionRow, ColumnXls); break;
                    case "Results_DateC": Column.Results_DateC = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DateC, definitionRow, ColumnXls); break;
                    case "Results_DateD": Column.Results_DateD = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DateD, definitionRow, ColumnXls); break;
                    case "Results_DateE": Column.Results_DateE = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DateE, definitionRow, ColumnXls); break;
                    case "Results_DateF": Column.Results_DateF = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DateF, definitionRow, ColumnXls); break;
                    case "Results_DateG": Column.Results_DateG = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DateG, definitionRow, ColumnXls); break;
                    case "Results_DateH": Column.Results_DateH = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DateH, definitionRow, ColumnXls); break;
                    case "Results_DateI": Column.Results_DateI = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DateI, definitionRow, ColumnXls); break;
                    case "Results_DateJ": Column.Results_DateJ = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DateJ, definitionRow, ColumnXls); break;
                    case "Results_DateK": Column.Results_DateK = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DateK, definitionRow, ColumnXls); break;
                    case "Results_DateL": Column.Results_DateL = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DateL, definitionRow, ColumnXls); break;
                    case "Results_DateM": Column.Results_DateM = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DateM, definitionRow, ColumnXls); break;
                    case "Results_DateN": Column.Results_DateN = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DateN, definitionRow, ColumnXls); break;
                    case "Results_DateO": Column.Results_DateO = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DateO, definitionRow, ColumnXls); break;
                    case "Results_DateP": Column.Results_DateP = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DateP, definitionRow, ColumnXls); break;
                    case "Results_DateQ": Column.Results_DateQ = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DateQ, definitionRow, ColumnXls); break;
                    case "Results_DateR": Column.Results_DateR = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DateR, definitionRow, ColumnXls); break;
                    case "Results_DateS": Column.Results_DateS = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DateS, definitionRow, ColumnXls); break;
                    case "Results_DateT": Column.Results_DateT = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DateT, definitionRow, ColumnXls); break;
                    case "Results_DateU": Column.Results_DateU = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DateU, definitionRow, ColumnXls); break;
                    case "Results_DateV": Column.Results_DateV = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DateV, definitionRow, ColumnXls); break;
                    case "Results_DateW": Column.Results_DateW = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DateW, definitionRow, ColumnXls); break;
                    case "Results_DateX": Column.Results_DateX = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DateX, definitionRow, ColumnXls); break;
                    case "Results_DateY": Column.Results_DateY = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DateY, definitionRow, ColumnXls); break;
                    case "Results_DateZ": Column.Results_DateZ = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DateZ, definitionRow, ColumnXls); break;
                    case "Results_DescriptionA": Column.Results_DescriptionA = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DescriptionA, definitionRow, ColumnXls); break;
                    case "Results_DescriptionB": Column.Results_DescriptionB = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DescriptionB, definitionRow, ColumnXls); break;
                    case "Results_DescriptionC": Column.Results_DescriptionC = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DescriptionC, definitionRow, ColumnXls); break;
                    case "Results_DescriptionD": Column.Results_DescriptionD = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DescriptionD, definitionRow, ColumnXls); break;
                    case "Results_DescriptionE": Column.Results_DescriptionE = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DescriptionE, definitionRow, ColumnXls); break;
                    case "Results_DescriptionF": Column.Results_DescriptionF = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DescriptionF, definitionRow, ColumnXls); break;
                    case "Results_DescriptionG": Column.Results_DescriptionG = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DescriptionG, definitionRow, ColumnXls); break;
                    case "Results_DescriptionH": Column.Results_DescriptionH = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DescriptionH, definitionRow, ColumnXls); break;
                    case "Results_DescriptionI": Column.Results_DescriptionI = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DescriptionI, definitionRow, ColumnXls); break;
                    case "Results_DescriptionJ": Column.Results_DescriptionJ = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DescriptionJ, definitionRow, ColumnXls); break;
                    case "Results_DescriptionK": Column.Results_DescriptionK = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DescriptionK, definitionRow, ColumnXls); break;
                    case "Results_DescriptionL": Column.Results_DescriptionL = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DescriptionL, definitionRow, ColumnXls); break;
                    case "Results_DescriptionM": Column.Results_DescriptionM = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DescriptionM, definitionRow, ColumnXls); break;
                    case "Results_DescriptionN": Column.Results_DescriptionN = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DescriptionN, definitionRow, ColumnXls); break;
                    case "Results_DescriptionO": Column.Results_DescriptionO = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DescriptionO, definitionRow, ColumnXls); break;
                    case "Results_DescriptionP": Column.Results_DescriptionP = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DescriptionP, definitionRow, ColumnXls); break;
                    case "Results_DescriptionQ": Column.Results_DescriptionQ = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DescriptionQ, definitionRow, ColumnXls); break;
                    case "Results_DescriptionR": Column.Results_DescriptionR = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DescriptionR, definitionRow, ColumnXls); break;
                    case "Results_DescriptionS": Column.Results_DescriptionS = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DescriptionS, definitionRow, ColumnXls); break;
                    case "Results_DescriptionT": Column.Results_DescriptionT = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DescriptionT, definitionRow, ColumnXls); break;
                    case "Results_DescriptionU": Column.Results_DescriptionU = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DescriptionU, definitionRow, ColumnXls); break;
                    case "Results_DescriptionV": Column.Results_DescriptionV = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DescriptionV, definitionRow, ColumnXls); break;
                    case "Results_DescriptionW": Column.Results_DescriptionW = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DescriptionW, definitionRow, ColumnXls); break;
                    case "Results_DescriptionX": Column.Results_DescriptionX = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DescriptionX, definitionRow, ColumnXls); break;
                    case "Results_DescriptionY": Column.Results_DescriptionY = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DescriptionY, definitionRow, ColumnXls); break;
                    case "Results_DescriptionZ": Column.Results_DescriptionZ = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_DescriptionZ, definitionRow, ColumnXls); break;
                    case "Results_CheckA": Column.Results_CheckA = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_CheckA, definitionRow, ColumnXls); break;
                    case "Results_CheckB": Column.Results_CheckB = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_CheckB, definitionRow, ColumnXls); break;
                    case "Results_CheckC": Column.Results_CheckC = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_CheckC, definitionRow, ColumnXls); break;
                    case "Results_CheckD": Column.Results_CheckD = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_CheckD, definitionRow, ColumnXls); break;
                    case "Results_CheckE": Column.Results_CheckE = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_CheckE, definitionRow, ColumnXls); break;
                    case "Results_CheckF": Column.Results_CheckF = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_CheckF, definitionRow, ColumnXls); break;
                    case "Results_CheckG": Column.Results_CheckG = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_CheckG, definitionRow, ColumnXls); break;
                    case "Results_CheckH": Column.Results_CheckH = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_CheckH, definitionRow, ColumnXls); break;
                    case "Results_CheckI": Column.Results_CheckI = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_CheckI, definitionRow, ColumnXls); break;
                    case "Results_CheckJ": Column.Results_CheckJ = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_CheckJ, definitionRow, ColumnXls); break;
                    case "Results_CheckK": Column.Results_CheckK = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_CheckK, definitionRow, ColumnXls); break;
                    case "Results_CheckL": Column.Results_CheckL = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_CheckL, definitionRow, ColumnXls); break;
                    case "Results_CheckM": Column.Results_CheckM = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_CheckM, definitionRow, ColumnXls); break;
                    case "Results_CheckN": Column.Results_CheckN = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_CheckN, definitionRow, ColumnXls); break;
                    case "Results_CheckO": Column.Results_CheckO = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_CheckO, definitionRow, ColumnXls); break;
                    case "Results_CheckP": Column.Results_CheckP = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_CheckP, definitionRow, ColumnXls); break;
                    case "Results_CheckQ": Column.Results_CheckQ = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_CheckQ, definitionRow, ColumnXls); break;
                    case "Results_CheckR": Column.Results_CheckR = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_CheckR, definitionRow, ColumnXls); break;
                    case "Results_CheckS": Column.Results_CheckS = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_CheckS, definitionRow, ColumnXls); break;
                    case "Results_CheckT": Column.Results_CheckT = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_CheckT, definitionRow, ColumnXls); break;
                    case "Results_CheckU": Column.Results_CheckU = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_CheckU, definitionRow, ColumnXls); break;
                    case "Results_CheckV": Column.Results_CheckV = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_CheckV, definitionRow, ColumnXls); break;
                    case "Results_CheckW": Column.Results_CheckW = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_CheckW, definitionRow, ColumnXls); break;
                    case "Results_CheckX": Column.Results_CheckX = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_CheckX, definitionRow, ColumnXls); break;
                    case "Results_CheckY": Column.Results_CheckY = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_CheckY, definitionRow, ColumnXls); break;
                    case "Results_CheckZ": Column.Results_CheckZ = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_CheckZ, definitionRow, ColumnXls); break;
                    case "Wikis_WikiId": Column.Wikis_WikiId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Wikis_WikiId, definitionRow, ColumnXls); break;
                    case "Tenants_Ver": Column.Tenants_Ver = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Tenants_Ver, definitionRow, ColumnXls); break;
                    case "Tenants_Comments": Column.Tenants_Comments = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Tenants_Comments, definitionRow, ColumnXls); break;
                    case "Tenants_Creator": Column.Tenants_Creator = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Tenants_Creator, definitionRow, ColumnXls); break;
                    case "Tenants_Updator": Column.Tenants_Updator = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Tenants_Updator, definitionRow, ColumnXls); break;
                    case "Tenants_CreatedTime": Column.Tenants_CreatedTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Tenants_CreatedTime, definitionRow, ColumnXls); break;
                    case "Tenants_UpdatedTime": Column.Tenants_UpdatedTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Tenants_UpdatedTime, definitionRow, ColumnXls); break;
                    case "Tenants_VerUp": Column.Tenants_VerUp = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Tenants_VerUp, definitionRow, ColumnXls); break;
                    case "Tenants_Timestamp": Column.Tenants_Timestamp = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Tenants_Timestamp, definitionRow, ColumnXls); break;
                    case "Demos_Ver": Column.Demos_Ver = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Demos_Ver, definitionRow, ColumnXls); break;
                    case "Demos_Comments": Column.Demos_Comments = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Demos_Comments, definitionRow, ColumnXls); break;
                    case "Demos_Creator": Column.Demos_Creator = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Demos_Creator, definitionRow, ColumnXls); break;
                    case "Demos_Updator": Column.Demos_Updator = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Demos_Updator, definitionRow, ColumnXls); break;
                    case "Demos_CreatedTime": Column.Demos_CreatedTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Demos_CreatedTime, definitionRow, ColumnXls); break;
                    case "Demos_UpdatedTime": Column.Demos_UpdatedTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Demos_UpdatedTime, definitionRow, ColumnXls); break;
                    case "Demos_VerUp": Column.Demos_VerUp = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Demos_VerUp, definitionRow, ColumnXls); break;
                    case "Demos_Timestamp": Column.Demos_Timestamp = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Demos_Timestamp, definitionRow, ColumnXls); break;
                    case "SysLogs_Ver": Column.SysLogs_Ver = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SysLogs_Ver, definitionRow, ColumnXls); break;
                    case "SysLogs_Comments": Column.SysLogs_Comments = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SysLogs_Comments, definitionRow, ColumnXls); break;
                    case "SysLogs_Creator": Column.SysLogs_Creator = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SysLogs_Creator, definitionRow, ColumnXls); break;
                    case "SysLogs_Updator": Column.SysLogs_Updator = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SysLogs_Updator, definitionRow, ColumnXls); break;
                    case "SysLogs_UpdatedTime": Column.SysLogs_UpdatedTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SysLogs_UpdatedTime, definitionRow, ColumnXls); break;
                    case "SysLogs_VerUp": Column.SysLogs_VerUp = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SysLogs_VerUp, definitionRow, ColumnXls); break;
                    case "SysLogs_Timestamp": Column.SysLogs_Timestamp = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SysLogs_Timestamp, definitionRow, ColumnXls); break;
                    case "Depts_Ver": Column.Depts_Ver = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Depts_Ver, definitionRow, ColumnXls); break;
                    case "Depts_Comments": Column.Depts_Comments = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Depts_Comments, definitionRow, ColumnXls); break;
                    case "Depts_Creator": Column.Depts_Creator = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Depts_Creator, definitionRow, ColumnXls); break;
                    case "Depts_Updator": Column.Depts_Updator = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Depts_Updator, definitionRow, ColumnXls); break;
                    case "Depts_CreatedTime": Column.Depts_CreatedTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Depts_CreatedTime, definitionRow, ColumnXls); break;
                    case "Depts_UpdatedTime": Column.Depts_UpdatedTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Depts_UpdatedTime, definitionRow, ColumnXls); break;
                    case "Depts_VerUp": Column.Depts_VerUp = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Depts_VerUp, definitionRow, ColumnXls); break;
                    case "Depts_Timestamp": Column.Depts_Timestamp = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Depts_Timestamp, definitionRow, ColumnXls); break;
                    case "Groups_Ver": Column.Groups_Ver = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Groups_Ver, definitionRow, ColumnXls); break;
                    case "Groups_Comments": Column.Groups_Comments = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Groups_Comments, definitionRow, ColumnXls); break;
                    case "Groups_Creator": Column.Groups_Creator = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Groups_Creator, definitionRow, ColumnXls); break;
                    case "Groups_Updator": Column.Groups_Updator = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Groups_Updator, definitionRow, ColumnXls); break;
                    case "Groups_CreatedTime": Column.Groups_CreatedTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Groups_CreatedTime, definitionRow, ColumnXls); break;
                    case "Groups_UpdatedTime": Column.Groups_UpdatedTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Groups_UpdatedTime, definitionRow, ColumnXls); break;
                    case "Groups_VerUp": Column.Groups_VerUp = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Groups_VerUp, definitionRow, ColumnXls); break;
                    case "Groups_Timestamp": Column.Groups_Timestamp = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Groups_Timestamp, definitionRow, ColumnXls); break;
                    case "GroupMembers_Ver": Column.GroupMembers_Ver = definitionRow[1].ToString(); SetColumnTable(ColumnTable.GroupMembers_Ver, definitionRow, ColumnXls); break;
                    case "GroupMembers_Comments": Column.GroupMembers_Comments = definitionRow[1].ToString(); SetColumnTable(ColumnTable.GroupMembers_Comments, definitionRow, ColumnXls); break;
                    case "GroupMembers_Creator": Column.GroupMembers_Creator = definitionRow[1].ToString(); SetColumnTable(ColumnTable.GroupMembers_Creator, definitionRow, ColumnXls); break;
                    case "GroupMembers_Updator": Column.GroupMembers_Updator = definitionRow[1].ToString(); SetColumnTable(ColumnTable.GroupMembers_Updator, definitionRow, ColumnXls); break;
                    case "GroupMembers_CreatedTime": Column.GroupMembers_CreatedTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable.GroupMembers_CreatedTime, definitionRow, ColumnXls); break;
                    case "GroupMembers_UpdatedTime": Column.GroupMembers_UpdatedTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable.GroupMembers_UpdatedTime, definitionRow, ColumnXls); break;
                    case "GroupMembers_VerUp": Column.GroupMembers_VerUp = definitionRow[1].ToString(); SetColumnTable(ColumnTable.GroupMembers_VerUp, definitionRow, ColumnXls); break;
                    case "GroupMembers_Timestamp": Column.GroupMembers_Timestamp = definitionRow[1].ToString(); SetColumnTable(ColumnTable.GroupMembers_Timestamp, definitionRow, ColumnXls); break;
                    case "Users_Ver": Column.Users_Ver = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_Ver, definitionRow, ColumnXls); break;
                    case "Users_Comments": Column.Users_Comments = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_Comments, definitionRow, ColumnXls); break;
                    case "Users_Creator": Column.Users_Creator = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_Creator, definitionRow, ColumnXls); break;
                    case "Users_Updator": Column.Users_Updator = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_Updator, definitionRow, ColumnXls); break;
                    case "Users_CreatedTime": Column.Users_CreatedTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_CreatedTime, definitionRow, ColumnXls); break;
                    case "Users_UpdatedTime": Column.Users_UpdatedTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_UpdatedTime, definitionRow, ColumnXls); break;
                    case "Users_VerUp": Column.Users_VerUp = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_VerUp, definitionRow, ColumnXls); break;
                    case "Users_Timestamp": Column.Users_Timestamp = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_Timestamp, definitionRow, ColumnXls); break;
                    case "MailAddresses_Ver": Column.MailAddresses_Ver = definitionRow[1].ToString(); SetColumnTable(ColumnTable.MailAddresses_Ver, definitionRow, ColumnXls); break;
                    case "MailAddresses_Comments": Column.MailAddresses_Comments = definitionRow[1].ToString(); SetColumnTable(ColumnTable.MailAddresses_Comments, definitionRow, ColumnXls); break;
                    case "MailAddresses_Creator": Column.MailAddresses_Creator = definitionRow[1].ToString(); SetColumnTable(ColumnTable.MailAddresses_Creator, definitionRow, ColumnXls); break;
                    case "MailAddresses_Updator": Column.MailAddresses_Updator = definitionRow[1].ToString(); SetColumnTable(ColumnTable.MailAddresses_Updator, definitionRow, ColumnXls); break;
                    case "MailAddresses_CreatedTime": Column.MailAddresses_CreatedTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable.MailAddresses_CreatedTime, definitionRow, ColumnXls); break;
                    case "MailAddresses_UpdatedTime": Column.MailAddresses_UpdatedTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable.MailAddresses_UpdatedTime, definitionRow, ColumnXls); break;
                    case "MailAddresses_VerUp": Column.MailAddresses_VerUp = definitionRow[1].ToString(); SetColumnTable(ColumnTable.MailAddresses_VerUp, definitionRow, ColumnXls); break;
                    case "MailAddresses_Timestamp": Column.MailAddresses_Timestamp = definitionRow[1].ToString(); SetColumnTable(ColumnTable.MailAddresses_Timestamp, definitionRow, ColumnXls); break;
                    case "Permissions_Ver": Column.Permissions_Ver = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Permissions_Ver, definitionRow, ColumnXls); break;
                    case "Permissions_Comments": Column.Permissions_Comments = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Permissions_Comments, definitionRow, ColumnXls); break;
                    case "Permissions_Creator": Column.Permissions_Creator = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Permissions_Creator, definitionRow, ColumnXls); break;
                    case "Permissions_Updator": Column.Permissions_Updator = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Permissions_Updator, definitionRow, ColumnXls); break;
                    case "Permissions_CreatedTime": Column.Permissions_CreatedTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Permissions_CreatedTime, definitionRow, ColumnXls); break;
                    case "Permissions_UpdatedTime": Column.Permissions_UpdatedTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Permissions_UpdatedTime, definitionRow, ColumnXls); break;
                    case "Permissions_VerUp": Column.Permissions_VerUp = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Permissions_VerUp, definitionRow, ColumnXls); break;
                    case "Permissions_Timestamp": Column.Permissions_Timestamp = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Permissions_Timestamp, definitionRow, ColumnXls); break;
                    case "OutgoingMails_Ver": Column.OutgoingMails_Ver = definitionRow[1].ToString(); SetColumnTable(ColumnTable.OutgoingMails_Ver, definitionRow, ColumnXls); break;
                    case "OutgoingMails_Comments": Column.OutgoingMails_Comments = definitionRow[1].ToString(); SetColumnTable(ColumnTable.OutgoingMails_Comments, definitionRow, ColumnXls); break;
                    case "OutgoingMails_Creator": Column.OutgoingMails_Creator = definitionRow[1].ToString(); SetColumnTable(ColumnTable.OutgoingMails_Creator, definitionRow, ColumnXls); break;
                    case "OutgoingMails_Updator": Column.OutgoingMails_Updator = definitionRow[1].ToString(); SetColumnTable(ColumnTable.OutgoingMails_Updator, definitionRow, ColumnXls); break;
                    case "OutgoingMails_CreatedTime": Column.OutgoingMails_CreatedTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable.OutgoingMails_CreatedTime, definitionRow, ColumnXls); break;
                    case "OutgoingMails_UpdatedTime": Column.OutgoingMails_UpdatedTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable.OutgoingMails_UpdatedTime, definitionRow, ColumnXls); break;
                    case "OutgoingMails_VerUp": Column.OutgoingMails_VerUp = definitionRow[1].ToString(); SetColumnTable(ColumnTable.OutgoingMails_VerUp, definitionRow, ColumnXls); break;
                    case "OutgoingMails_Timestamp": Column.OutgoingMails_Timestamp = definitionRow[1].ToString(); SetColumnTable(ColumnTable.OutgoingMails_Timestamp, definitionRow, ColumnXls); break;
                    case "SearchIndexes_Ver": Column.SearchIndexes_Ver = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SearchIndexes_Ver, definitionRow, ColumnXls); break;
                    case "SearchIndexes_Comments": Column.SearchIndexes_Comments = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SearchIndexes_Comments, definitionRow, ColumnXls); break;
                    case "SearchIndexes_Creator": Column.SearchIndexes_Creator = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SearchIndexes_Creator, definitionRow, ColumnXls); break;
                    case "SearchIndexes_Updator": Column.SearchIndexes_Updator = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SearchIndexes_Updator, definitionRow, ColumnXls); break;
                    case "SearchIndexes_CreatedTime": Column.SearchIndexes_CreatedTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SearchIndexes_CreatedTime, definitionRow, ColumnXls); break;
                    case "SearchIndexes_UpdatedTime": Column.SearchIndexes_UpdatedTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SearchIndexes_UpdatedTime, definitionRow, ColumnXls); break;
                    case "SearchIndexes_VerUp": Column.SearchIndexes_VerUp = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SearchIndexes_VerUp, definitionRow, ColumnXls); break;
                    case "SearchIndexes_Timestamp": Column.SearchIndexes_Timestamp = definitionRow[1].ToString(); SetColumnTable(ColumnTable.SearchIndexes_Timestamp, definitionRow, ColumnXls); break;
                    case "Items_Ver": Column.Items_Ver = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Items_Ver, definitionRow, ColumnXls); break;
                    case "Items_Comments": Column.Items_Comments = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Items_Comments, definitionRow, ColumnXls); break;
                    case "Items_Creator": Column.Items_Creator = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Items_Creator, definitionRow, ColumnXls); break;
                    case "Items_Updator": Column.Items_Updator = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Items_Updator, definitionRow, ColumnXls); break;
                    case "Items_CreatedTime": Column.Items_CreatedTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Items_CreatedTime, definitionRow, ColumnXls); break;
                    case "Items_UpdatedTime": Column.Items_UpdatedTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Items_UpdatedTime, definitionRow, ColumnXls); break;
                    case "Items_VerUp": Column.Items_VerUp = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Items_VerUp, definitionRow, ColumnXls); break;
                    case "Items_Timestamp": Column.Items_Timestamp = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Items_Timestamp, definitionRow, ColumnXls); break;
                    case "Sites_UpdatedTime": Column.Sites_UpdatedTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Sites_UpdatedTime, definitionRow, ColumnXls); break;
                    case "Sites_Title": Column.Sites_Title = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Sites_Title, definitionRow, ColumnXls); break;
                    case "Sites_Body": Column.Sites_Body = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Sites_Body, definitionRow, ColumnXls); break;
                    case "Sites_TitleBody": Column.Sites_TitleBody = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Sites_TitleBody, definitionRow, ColumnXls); break;
                    case "Sites_Ver": Column.Sites_Ver = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Sites_Ver, definitionRow, ColumnXls); break;
                    case "Sites_Comments": Column.Sites_Comments = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Sites_Comments, definitionRow, ColumnXls); break;
                    case "Sites_Creator": Column.Sites_Creator = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Sites_Creator, definitionRow, ColumnXls); break;
                    case "Sites_Updator": Column.Sites_Updator = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Sites_Updator, definitionRow, ColumnXls); break;
                    case "Sites_CreatedTime": Column.Sites_CreatedTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Sites_CreatedTime, definitionRow, ColumnXls); break;
                    case "Sites_VerUp": Column.Sites_VerUp = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Sites_VerUp, definitionRow, ColumnXls); break;
                    case "Sites_Timestamp": Column.Sites_Timestamp = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Sites_Timestamp, definitionRow, ColumnXls); break;
                    case "Orders_Ver": Column.Orders_Ver = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Orders_Ver, definitionRow, ColumnXls); break;
                    case "Orders_Comments": Column.Orders_Comments = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Orders_Comments, definitionRow, ColumnXls); break;
                    case "Orders_Creator": Column.Orders_Creator = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Orders_Creator, definitionRow, ColumnXls); break;
                    case "Orders_Updator": Column.Orders_Updator = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Orders_Updator, definitionRow, ColumnXls); break;
                    case "Orders_CreatedTime": Column.Orders_CreatedTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Orders_CreatedTime, definitionRow, ColumnXls); break;
                    case "Orders_UpdatedTime": Column.Orders_UpdatedTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Orders_UpdatedTime, definitionRow, ColumnXls); break;
                    case "Orders_VerUp": Column.Orders_VerUp = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Orders_VerUp, definitionRow, ColumnXls); break;
                    case "Orders_Timestamp": Column.Orders_Timestamp = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Orders_Timestamp, definitionRow, ColumnXls); break;
                    case "ExportSettings_Ver": Column.ExportSettings_Ver = definitionRow[1].ToString(); SetColumnTable(ColumnTable.ExportSettings_Ver, definitionRow, ColumnXls); break;
                    case "ExportSettings_Comments": Column.ExportSettings_Comments = definitionRow[1].ToString(); SetColumnTable(ColumnTable.ExportSettings_Comments, definitionRow, ColumnXls); break;
                    case "ExportSettings_Creator": Column.ExportSettings_Creator = definitionRow[1].ToString(); SetColumnTable(ColumnTable.ExportSettings_Creator, definitionRow, ColumnXls); break;
                    case "ExportSettings_Updator": Column.ExportSettings_Updator = definitionRow[1].ToString(); SetColumnTable(ColumnTable.ExportSettings_Updator, definitionRow, ColumnXls); break;
                    case "ExportSettings_CreatedTime": Column.ExportSettings_CreatedTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable.ExportSettings_CreatedTime, definitionRow, ColumnXls); break;
                    case "ExportSettings_UpdatedTime": Column.ExportSettings_UpdatedTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable.ExportSettings_UpdatedTime, definitionRow, ColumnXls); break;
                    case "ExportSettings_VerUp": Column.ExportSettings_VerUp = definitionRow[1].ToString(); SetColumnTable(ColumnTable.ExportSettings_VerUp, definitionRow, ColumnXls); break;
                    case "ExportSettings_Timestamp": Column.ExportSettings_Timestamp = definitionRow[1].ToString(); SetColumnTable(ColumnTable.ExportSettings_Timestamp, definitionRow, ColumnXls); break;
                    case "Links_Ver": Column.Links_Ver = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Links_Ver, definitionRow, ColumnXls); break;
                    case "Links_Comments": Column.Links_Comments = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Links_Comments, definitionRow, ColumnXls); break;
                    case "Links_Creator": Column.Links_Creator = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Links_Creator, definitionRow, ColumnXls); break;
                    case "Links_Updator": Column.Links_Updator = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Links_Updator, definitionRow, ColumnXls); break;
                    case "Links_CreatedTime": Column.Links_CreatedTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Links_CreatedTime, definitionRow, ColumnXls); break;
                    case "Links_UpdatedTime": Column.Links_UpdatedTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Links_UpdatedTime, definitionRow, ColumnXls); break;
                    case "Links_VerUp": Column.Links_VerUp = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Links_VerUp, definitionRow, ColumnXls); break;
                    case "Links_Timestamp": Column.Links_Timestamp = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Links_Timestamp, definitionRow, ColumnXls); break;
                    case "Binaries_Ver": Column.Binaries_Ver = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Binaries_Ver, definitionRow, ColumnXls); break;
                    case "Binaries_Comments": Column.Binaries_Comments = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Binaries_Comments, definitionRow, ColumnXls); break;
                    case "Binaries_Creator": Column.Binaries_Creator = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Binaries_Creator, definitionRow, ColumnXls); break;
                    case "Binaries_Updator": Column.Binaries_Updator = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Binaries_Updator, definitionRow, ColumnXls); break;
                    case "Binaries_CreatedTime": Column.Binaries_CreatedTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Binaries_CreatedTime, definitionRow, ColumnXls); break;
                    case "Binaries_UpdatedTime": Column.Binaries_UpdatedTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Binaries_UpdatedTime, definitionRow, ColumnXls); break;
                    case "Binaries_VerUp": Column.Binaries_VerUp = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Binaries_VerUp, definitionRow, ColumnXls); break;
                    case "Binaries_Timestamp": Column.Binaries_Timestamp = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Binaries_Timestamp, definitionRow, ColumnXls); break;
                    case "Issues_SiteId": Column.Issues_SiteId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_SiteId, definitionRow, ColumnXls); break;
                    case "Issues_UpdatedTime": Column.Issues_UpdatedTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_UpdatedTime, definitionRow, ColumnXls); break;
                    case "Issues_Title": Column.Issues_Title = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_Title, definitionRow, ColumnXls); break;
                    case "Issues_Body": Column.Issues_Body = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_Body, definitionRow, ColumnXls); break;
                    case "Issues_TitleBody": Column.Issues_TitleBody = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_TitleBody, definitionRow, ColumnXls); break;
                    case "Issues_Ver": Column.Issues_Ver = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_Ver, definitionRow, ColumnXls); break;
                    case "Issues_Comments": Column.Issues_Comments = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_Comments, definitionRow, ColumnXls); break;
                    case "Issues_Creator": Column.Issues_Creator = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_Creator, definitionRow, ColumnXls); break;
                    case "Issues_Updator": Column.Issues_Updator = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_Updator, definitionRow, ColumnXls); break;
                    case "Issues_CreatedTime": Column.Issues_CreatedTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_CreatedTime, definitionRow, ColumnXls); break;
                    case "Issues_VerUp": Column.Issues_VerUp = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_VerUp, definitionRow, ColumnXls); break;
                    case "Issues_Timestamp": Column.Issues_Timestamp = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Issues_Timestamp, definitionRow, ColumnXls); break;
                    case "Results_SiteId": Column.Results_SiteId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_SiteId, definitionRow, ColumnXls); break;
                    case "Results_UpdatedTime": Column.Results_UpdatedTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_UpdatedTime, definitionRow, ColumnXls); break;
                    case "Results_Body": Column.Results_Body = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_Body, definitionRow, ColumnXls); break;
                    case "Results_TitleBody": Column.Results_TitleBody = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_TitleBody, definitionRow, ColumnXls); break;
                    case "Results_Ver": Column.Results_Ver = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_Ver, definitionRow, ColumnXls); break;
                    case "Results_Comments": Column.Results_Comments = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_Comments, definitionRow, ColumnXls); break;
                    case "Results_Creator": Column.Results_Creator = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_Creator, definitionRow, ColumnXls); break;
                    case "Results_Updator": Column.Results_Updator = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_Updator, definitionRow, ColumnXls); break;
                    case "Results_CreatedTime": Column.Results_CreatedTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_CreatedTime, definitionRow, ColumnXls); break;
                    case "Results_VerUp": Column.Results_VerUp = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_VerUp, definitionRow, ColumnXls); break;
                    case "Results_Timestamp": Column.Results_Timestamp = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Results_Timestamp, definitionRow, ColumnXls); break;
                    case "Wikis_SiteId": Column.Wikis_SiteId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Wikis_SiteId, definitionRow, ColumnXls); break;
                    case "Wikis_UpdatedTime": Column.Wikis_UpdatedTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Wikis_UpdatedTime, definitionRow, ColumnXls); break;
                    case "Wikis_Title": Column.Wikis_Title = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Wikis_Title, definitionRow, ColumnXls); break;
                    case "Wikis_Body": Column.Wikis_Body = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Wikis_Body, definitionRow, ColumnXls); break;
                    case "Wikis_TitleBody": Column.Wikis_TitleBody = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Wikis_TitleBody, definitionRow, ColumnXls); break;
                    case "Wikis_Ver": Column.Wikis_Ver = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Wikis_Ver, definitionRow, ColumnXls); break;
                    case "Wikis_Comments": Column.Wikis_Comments = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Wikis_Comments, definitionRow, ColumnXls); break;
                    case "Wikis_Creator": Column.Wikis_Creator = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Wikis_Creator, definitionRow, ColumnXls); break;
                    case "Wikis_Updator": Column.Wikis_Updator = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Wikis_Updator, definitionRow, ColumnXls); break;
                    case "Wikis_CreatedTime": Column.Wikis_CreatedTime = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Wikis_CreatedTime, definitionRow, ColumnXls); break;
                    case "Wikis_VerUp": Column.Wikis_VerUp = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Wikis_VerUp, definitionRow, ColumnXls); break;
                    case "Wikis_Timestamp": Column.Wikis_Timestamp = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Wikis_Timestamp, definitionRow, ColumnXls); break;
                    default: break;
                }
            });
            ColumnXls.XlsSheet.AsEnumerable().Skip(1).Where(o => o[0].ToString() != string.Empty).ForEach(definitionRow =>
            {
                var newColumnDefinition = new ColumnDefinition();
                if (definitionRow.ContainsKey("Id")) { newColumnDefinition.Id = definitionRow["Id"].ToString(); newColumnDefinition.SavedId = newColumnDefinition.Id; }
                if (definitionRow.ContainsKey("ModelName")) { newColumnDefinition.ModelName = definitionRow["ModelName"].ToString(); newColumnDefinition.SavedModelName = newColumnDefinition.ModelName; }
                if (definitionRow.ContainsKey("TableName")) { newColumnDefinition.TableName = definitionRow["TableName"].ToString(); newColumnDefinition.SavedTableName = newColumnDefinition.TableName; }
                if (definitionRow.ContainsKey("Base")) { newColumnDefinition.Base = definitionRow["Base"].ToBool(); newColumnDefinition.SavedBase = newColumnDefinition.Base; }
                if (definitionRow.ContainsKey("EachModel")) { newColumnDefinition.EachModel = definitionRow["EachModel"].ToBool(); newColumnDefinition.SavedEachModel = newColumnDefinition.EachModel; }
                if (definitionRow.ContainsKey("Label")) { newColumnDefinition.Label = definitionRow["Label"].ToString(); newColumnDefinition.SavedLabel = newColumnDefinition.Label; }
                if (definitionRow.ContainsKey("ColumnName")) { newColumnDefinition.ColumnName = definitionRow["ColumnName"].ToString(); newColumnDefinition.SavedColumnName = newColumnDefinition.ColumnName; }
                if (definitionRow.ContainsKey("ColumnLabel")) { newColumnDefinition.ColumnLabel = definitionRow["ColumnLabel"].ToString(); newColumnDefinition.SavedColumnLabel = newColumnDefinition.ColumnLabel; }
                if (definitionRow.ContainsKey("No")) { newColumnDefinition.No = definitionRow["No"].ToInt(); newColumnDefinition.SavedNo = newColumnDefinition.No; }
                if (definitionRow.ContainsKey("History")) { newColumnDefinition.History = definitionRow["History"].ToInt(); newColumnDefinition.SavedHistory = newColumnDefinition.History; }
                if (definitionRow.ContainsKey("Import")) { newColumnDefinition.Import = definitionRow["Import"].ToInt(); newColumnDefinition.SavedImport = newColumnDefinition.Import; }
                if (definitionRow.ContainsKey("Export")) { newColumnDefinition.Export = definitionRow["Export"].ToInt(); newColumnDefinition.SavedExport = newColumnDefinition.Export; }
                if (definitionRow.ContainsKey("GridColumn")) { newColumnDefinition.GridColumn = definitionRow["GridColumn"].ToInt(); newColumnDefinition.SavedGridColumn = newColumnDefinition.GridColumn; }
                if (definitionRow.ContainsKey("GridEnabled")) { newColumnDefinition.GridEnabled = definitionRow["GridEnabled"].ToBool(); newColumnDefinition.SavedGridEnabled = newColumnDefinition.GridEnabled; }
                if (definitionRow.ContainsKey("FilterColumn")) { newColumnDefinition.FilterColumn = definitionRow["FilterColumn"].ToInt(); newColumnDefinition.SavedFilterColumn = newColumnDefinition.FilterColumn; }
                if (definitionRow.ContainsKey("FilterEnabled")) { newColumnDefinition.FilterEnabled = definitionRow["FilterEnabled"].ToBool(); newColumnDefinition.SavedFilterEnabled = newColumnDefinition.FilterEnabled; }
                if (definitionRow.ContainsKey("EditorColumn")) { newColumnDefinition.EditorColumn = definitionRow["EditorColumn"].ToBool(); newColumnDefinition.SavedEditorColumn = newColumnDefinition.EditorColumn; }
                if (definitionRow.ContainsKey("EditorEnabled")) { newColumnDefinition.EditorEnabled = definitionRow["EditorEnabled"].ToBool(); newColumnDefinition.SavedEditorEnabled = newColumnDefinition.EditorEnabled; }
                if (definitionRow.ContainsKey("TitleColumn")) { newColumnDefinition.TitleColumn = definitionRow["TitleColumn"].ToInt(); newColumnDefinition.SavedTitleColumn = newColumnDefinition.TitleColumn; }
                if (definitionRow.ContainsKey("LinkColumn")) { newColumnDefinition.LinkColumn = definitionRow["LinkColumn"].ToInt(); newColumnDefinition.SavedLinkColumn = newColumnDefinition.LinkColumn; }
                if (definitionRow.ContainsKey("LinkEnabled")) { newColumnDefinition.LinkEnabled = definitionRow["LinkEnabled"].ToBool(); newColumnDefinition.SavedLinkEnabled = newColumnDefinition.LinkEnabled; }
                if (definitionRow.ContainsKey("HistoryColumn")) { newColumnDefinition.HistoryColumn = definitionRow["HistoryColumn"].ToInt(); newColumnDefinition.SavedHistoryColumn = newColumnDefinition.HistoryColumn; }
                if (definitionRow.ContainsKey("HistoryEnabled")) { newColumnDefinition.HistoryEnabled = definitionRow["HistoryEnabled"].ToBool(); newColumnDefinition.SavedHistoryEnabled = newColumnDefinition.HistoryEnabled; }
                if (definitionRow.ContainsKey("TypeName")) { newColumnDefinition.TypeName = definitionRow["TypeName"].ToString(); newColumnDefinition.SavedTypeName = newColumnDefinition.TypeName; }
                if (definitionRow.ContainsKey("TypeCs")) { newColumnDefinition.TypeCs = definitionRow["TypeCs"].ToString(); newColumnDefinition.SavedTypeCs = newColumnDefinition.TypeCs; }
                if (definitionRow.ContainsKey("RecordingData")) { newColumnDefinition.RecordingData = definitionRow["RecordingData"].ToString(); newColumnDefinition.SavedRecordingData = newColumnDefinition.RecordingData; }
                if (definitionRow.ContainsKey("MaxLength")) { newColumnDefinition.MaxLength = definitionRow["MaxLength"].ToInt(); newColumnDefinition.SavedMaxLength = newColumnDefinition.MaxLength; }
                if (definitionRow.ContainsKey("Size")) { newColumnDefinition.Size = definitionRow["Size"].ToString(); newColumnDefinition.SavedSize = newColumnDefinition.Size; }
                if (definitionRow.ContainsKey("Pk")) { newColumnDefinition.Pk = definitionRow["Pk"].ToInt(); newColumnDefinition.SavedPk = newColumnDefinition.Pk; }
                if (definitionRow.ContainsKey("PkOrderBy")) { newColumnDefinition.PkOrderBy = definitionRow["PkOrderBy"].ToString(); newColumnDefinition.SavedPkOrderBy = newColumnDefinition.PkOrderBy; }
                if (definitionRow.ContainsKey("PkHistory")) { newColumnDefinition.PkHistory = definitionRow["PkHistory"].ToInt(); newColumnDefinition.SavedPkHistory = newColumnDefinition.PkHistory; }
                if (definitionRow.ContainsKey("PkHistoryOrderBy")) { newColumnDefinition.PkHistoryOrderBy = definitionRow["PkHistoryOrderBy"].ToString(); newColumnDefinition.SavedPkHistoryOrderBy = newColumnDefinition.PkHistoryOrderBy; }
                if (definitionRow.ContainsKey("Ix1")) { newColumnDefinition.Ix1 = definitionRow["Ix1"].ToInt(); newColumnDefinition.SavedIx1 = newColumnDefinition.Ix1; }
                if (definitionRow.ContainsKey("Ix1OrderBy")) { newColumnDefinition.Ix1OrderBy = definitionRow["Ix1OrderBy"].ToString(); newColumnDefinition.SavedIx1OrderBy = newColumnDefinition.Ix1OrderBy; }
                if (definitionRow.ContainsKey("Ix2")) { newColumnDefinition.Ix2 = definitionRow["Ix2"].ToInt(); newColumnDefinition.SavedIx2 = newColumnDefinition.Ix2; }
                if (definitionRow.ContainsKey("Ix2OrderBy")) { newColumnDefinition.Ix2OrderBy = definitionRow["Ix2OrderBy"].ToString(); newColumnDefinition.SavedIx2OrderBy = newColumnDefinition.Ix2OrderBy; }
                if (definitionRow.ContainsKey("Ix3")) { newColumnDefinition.Ix3 = definitionRow["Ix3"].ToInt(); newColumnDefinition.SavedIx3 = newColumnDefinition.Ix3; }
                if (definitionRow.ContainsKey("Ix3OrderBy")) { newColumnDefinition.Ix3OrderBy = definitionRow["Ix3OrderBy"].ToString(); newColumnDefinition.SavedIx3OrderBy = newColumnDefinition.Ix3OrderBy; }
                if (definitionRow.ContainsKey("Nullable")) { newColumnDefinition.Nullable = definitionRow["Nullable"].ToBool(); newColumnDefinition.SavedNullable = newColumnDefinition.Nullable; }
                if (definitionRow.ContainsKey("Default")) { newColumnDefinition.Default = definitionRow["Default"].ToString(); newColumnDefinition.SavedDefault = newColumnDefinition.Default; }
                if (definitionRow.ContainsKey("DefaultCs")) { newColumnDefinition.DefaultCs = definitionRow["DefaultCs"].ToString(); newColumnDefinition.SavedDefaultCs = newColumnDefinition.DefaultCs; }
                if (definitionRow.ContainsKey("Identity")) { newColumnDefinition.Identity = definitionRow["Identity"].ToBool(); newColumnDefinition.SavedIdentity = newColumnDefinition.Identity; }
                if (definitionRow.ContainsKey("Unique")) { newColumnDefinition.Unique = definitionRow["Unique"].ToBool(); newColumnDefinition.SavedUnique = newColumnDefinition.Unique; }
                if (definitionRow.ContainsKey("Seed")) { newColumnDefinition.Seed = definitionRow["Seed"].ToInt(); newColumnDefinition.SavedSeed = newColumnDefinition.Seed; }
                if (definitionRow.ContainsKey("JoinTableName")) { newColumnDefinition.JoinTableName = definitionRow["JoinTableName"].ToString(); newColumnDefinition.SavedJoinTableName = newColumnDefinition.JoinTableName; }
                if (definitionRow.ContainsKey("JoinType")) { newColumnDefinition.JoinType = definitionRow["JoinType"].ToString(); newColumnDefinition.SavedJoinType = newColumnDefinition.JoinType; }
                if (definitionRow.ContainsKey("JoinExpression")) { newColumnDefinition.JoinExpression = definitionRow["JoinExpression"].ToString(); newColumnDefinition.SavedJoinExpression = newColumnDefinition.JoinExpression; }
                if (definitionRow.ContainsKey("Like")) { newColumnDefinition.Like = definitionRow["Like"].ToBool(); newColumnDefinition.SavedLike = newColumnDefinition.Like; }
                if (definitionRow.ContainsKey("WhereSpecial")) { newColumnDefinition.WhereSpecial = definitionRow["WhereSpecial"].ToBool(); newColumnDefinition.SavedWhereSpecial = newColumnDefinition.WhereSpecial; }
                if (definitionRow.ContainsKey("ReadPermission")) { newColumnDefinition.ReadPermission = definitionRow["ReadPermission"].ToString(); newColumnDefinition.SavedReadPermission = newColumnDefinition.ReadPermission; }
                if (definitionRow.ContainsKey("CreatePermission")) { newColumnDefinition.CreatePermission = definitionRow["CreatePermission"].ToString(); newColumnDefinition.SavedCreatePermission = newColumnDefinition.CreatePermission; }
                if (definitionRow.ContainsKey("UpdatePermission")) { newColumnDefinition.UpdatePermission = definitionRow["UpdatePermission"].ToString(); newColumnDefinition.SavedUpdatePermission = newColumnDefinition.UpdatePermission; }
                if (definitionRow.ContainsKey("NotEditSelf")) { newColumnDefinition.NotEditSelf = definitionRow["NotEditSelf"].ToBool(); newColumnDefinition.SavedNotEditSelf = newColumnDefinition.NotEditSelf; }
                if (definitionRow.ContainsKey("SearchIndexPriority")) { newColumnDefinition.SearchIndexPriority = definitionRow["SearchIndexPriority"].ToInt(); newColumnDefinition.SavedSearchIndexPriority = newColumnDefinition.SearchIndexPriority; }
                if (definitionRow.ContainsKey("NotForm")) { newColumnDefinition.NotForm = definitionRow["NotForm"].ToBool(); newColumnDefinition.SavedNotForm = newColumnDefinition.NotForm; }
                if (definitionRow.ContainsKey("NotSelect")) { newColumnDefinition.NotSelect = definitionRow["NotSelect"].ToBool(); newColumnDefinition.SavedNotSelect = newColumnDefinition.NotSelect; }
                if (definitionRow.ContainsKey("NotUpdate")) { newColumnDefinition.NotUpdate = definitionRow["NotUpdate"].ToBool(); newColumnDefinition.SavedNotUpdate = newColumnDefinition.NotUpdate; }
                if (definitionRow.ContainsKey("ByForm")) { newColumnDefinition.ByForm = definitionRow["ByForm"].ToString(); newColumnDefinition.SavedByForm = newColumnDefinition.ByForm; }
                if (definitionRow.ContainsKey("ByDataRow")) { newColumnDefinition.ByDataRow = definitionRow["ByDataRow"].ToString(); newColumnDefinition.SavedByDataRow = newColumnDefinition.ByDataRow; }
                if (definitionRow.ContainsKey("BySession")) { newColumnDefinition.BySession = definitionRow["BySession"].ToString(); newColumnDefinition.SavedBySession = newColumnDefinition.BySession; }
                if (definitionRow.ContainsKey("ToControl")) { newColumnDefinition.ToControl = definitionRow["ToControl"].ToString(); newColumnDefinition.SavedToControl = newColumnDefinition.ToControl; }
                if (definitionRow.ContainsKey("SelectColumns")) { newColumnDefinition.SelectColumns = definitionRow["SelectColumns"].ToString(); newColumnDefinition.SavedSelectColumns = newColumnDefinition.SelectColumns; }
                if (definitionRow.ContainsKey("ComputeColumn")) { newColumnDefinition.ComputeColumn = definitionRow["ComputeColumn"].ToString(); newColumnDefinition.SavedComputeColumn = newColumnDefinition.ComputeColumn; }
                if (definitionRow.ContainsKey("OrderByColumns")) { newColumnDefinition.OrderByColumns = definitionRow["OrderByColumns"].ToString(); newColumnDefinition.SavedOrderByColumns = newColumnDefinition.OrderByColumns; }
                if (definitionRow.ContainsKey("ItemId")) { newColumnDefinition.ItemId = definitionRow["ItemId"].ToInt(); newColumnDefinition.SavedItemId = newColumnDefinition.ItemId; }
                if (definitionRow.ContainsKey("GenericUi")) { newColumnDefinition.GenericUi = definitionRow["GenericUi"].ToBool(); newColumnDefinition.SavedGenericUi = newColumnDefinition.GenericUi; }
                if (definitionRow.ContainsKey("FieldCss")) { newColumnDefinition.FieldCss = definitionRow["FieldCss"].ToString(); newColumnDefinition.SavedFieldCss = newColumnDefinition.FieldCss; }
                if (definitionRow.ContainsKey("ControlCss")) { newColumnDefinition.ControlCss = definitionRow["ControlCss"].ToString(); newColumnDefinition.SavedControlCss = newColumnDefinition.ControlCss; }
                if (definitionRow.ContainsKey("MarkDown")) { newColumnDefinition.MarkDown = definitionRow["MarkDown"].ToBool(); newColumnDefinition.SavedMarkDown = newColumnDefinition.MarkDown; }
                if (definitionRow.ContainsKey("GridStyle")) { newColumnDefinition.GridStyle = definitionRow["GridStyle"].ToString(); newColumnDefinition.SavedGridStyle = newColumnDefinition.GridStyle; }
                if (definitionRow.ContainsKey("Hash")) { newColumnDefinition.Hash = definitionRow["Hash"].ToBool(); newColumnDefinition.SavedHash = newColumnDefinition.Hash; }
                if (definitionRow.ContainsKey("Calc")) { newColumnDefinition.Calc = definitionRow["Calc"].ToString(); newColumnDefinition.SavedCalc = newColumnDefinition.Calc; }
                if (definitionRow.ContainsKey("Session")) { newColumnDefinition.Session = definitionRow["Session"].ToBool(); newColumnDefinition.SavedSession = newColumnDefinition.Session; }
                if (definitionRow.ContainsKey("UserColumn")) { newColumnDefinition.UserColumn = definitionRow["UserColumn"].ToBool(); newColumnDefinition.SavedUserColumn = newColumnDefinition.UserColumn; }
                if (definitionRow.ContainsKey("EnumColumn")) { newColumnDefinition.EnumColumn = definitionRow["EnumColumn"].ToBool(); newColumnDefinition.SavedEnumColumn = newColumnDefinition.EnumColumn; }
                if (definitionRow.ContainsKey("NotEditorSettings")) { newColumnDefinition.NotEditorSettings = definitionRow["NotEditorSettings"].ToBool(); newColumnDefinition.SavedNotEditorSettings = newColumnDefinition.NotEditorSettings; }
                if (definitionRow.ContainsKey("ControlType")) { newColumnDefinition.ControlType = definitionRow["ControlType"].ToString(); newColumnDefinition.SavedControlType = newColumnDefinition.ControlType; }
                if (definitionRow.ContainsKey("GridFormat")) { newColumnDefinition.GridFormat = definitionRow["GridFormat"].ToString(); newColumnDefinition.SavedGridFormat = newColumnDefinition.GridFormat; }
                if (definitionRow.ContainsKey("EditorFormat")) { newColumnDefinition.EditorFormat = definitionRow["EditorFormat"].ToString(); newColumnDefinition.SavedEditorFormat = newColumnDefinition.EditorFormat; }
                if (definitionRow.ContainsKey("ExportFormat")) { newColumnDefinition.ExportFormat = definitionRow["ExportFormat"].ToString(); newColumnDefinition.SavedExportFormat = newColumnDefinition.ExportFormat; }
                if (definitionRow.ContainsKey("Aggregatable")) { newColumnDefinition.Aggregatable = definitionRow["Aggregatable"].ToBool(); newColumnDefinition.SavedAggregatable = newColumnDefinition.Aggregatable; }
                if (definitionRow.ContainsKey("Computable")) { newColumnDefinition.Computable = definitionRow["Computable"].ToBool(); newColumnDefinition.SavedComputable = newColumnDefinition.Computable; }
                if (definitionRow.ContainsKey("ChoicesText")) { newColumnDefinition.ChoicesText = definitionRow["ChoicesText"].ToString(); newColumnDefinition.SavedChoicesText = newColumnDefinition.ChoicesText; }
                if (definitionRow.ContainsKey("UseSearch")) { newColumnDefinition.UseSearch = definitionRow["UseSearch"].ToBool(); newColumnDefinition.SavedUseSearch = newColumnDefinition.UseSearch; }
                if (definitionRow.ContainsKey("DefaultInput")) { newColumnDefinition.DefaultInput = definitionRow["DefaultInput"].ToString(); newColumnDefinition.SavedDefaultInput = newColumnDefinition.DefaultInput; }
                if (definitionRow.ContainsKey("Own")) { newColumnDefinition.Own = definitionRow["Own"].ToBool(); newColumnDefinition.SavedOwn = newColumnDefinition.Own; }
                if (definitionRow.ContainsKey("FormName")) { newColumnDefinition.FormName = definitionRow["FormName"].ToString(); newColumnDefinition.SavedFormName = newColumnDefinition.FormName; }
                if (definitionRow.ContainsKey("ValidateRequired")) { newColumnDefinition.ValidateRequired = definitionRow["ValidateRequired"].ToBool(); newColumnDefinition.SavedValidateRequired = newColumnDefinition.ValidateRequired; }
                if (definitionRow.ContainsKey("ValidateNumber")) { newColumnDefinition.ValidateNumber = definitionRow["ValidateNumber"].ToBool(); newColumnDefinition.SavedValidateNumber = newColumnDefinition.ValidateNumber; }
                if (definitionRow.ContainsKey("ValidateDate")) { newColumnDefinition.ValidateDate = definitionRow["ValidateDate"].ToBool(); newColumnDefinition.SavedValidateDate = newColumnDefinition.ValidateDate; }
                if (definitionRow.ContainsKey("ValidateEmail")) { newColumnDefinition.ValidateEmail = definitionRow["ValidateEmail"].ToBool(); newColumnDefinition.SavedValidateEmail = newColumnDefinition.ValidateEmail; }
                if (definitionRow.ContainsKey("ValidateEqualTo")) { newColumnDefinition.ValidateEqualTo = definitionRow["ValidateEqualTo"].ToString(); newColumnDefinition.SavedValidateEqualTo = newColumnDefinition.ValidateEqualTo; }
                if (definitionRow.ContainsKey("DecimalPlaces")) { newColumnDefinition.DecimalPlaces = definitionRow["DecimalPlaces"].ToInt(); newColumnDefinition.SavedDecimalPlaces = newColumnDefinition.DecimalPlaces; }
                if (definitionRow.ContainsKey("Min")) { newColumnDefinition.Min = definitionRow["Min"].ToDecimal(); newColumnDefinition.SavedMin = newColumnDefinition.Min; }
                if (definitionRow.ContainsKey("Max")) { newColumnDefinition.Max = definitionRow["Max"].ToDecimal(); newColumnDefinition.SavedMax = newColumnDefinition.Max; }
                if (definitionRow.ContainsKey("Step")) { newColumnDefinition.Step = definitionRow["Step"].ToDecimal(); newColumnDefinition.SavedStep = newColumnDefinition.Step; }
                if (definitionRow.ContainsKey("StringFormat")) { newColumnDefinition.StringFormat = definitionRow["StringFormat"].ToString(); newColumnDefinition.SavedStringFormat = newColumnDefinition.StringFormat; }
                if (definitionRow.ContainsKey("Unit")) { newColumnDefinition.Unit = definitionRow["Unit"].ToString(); newColumnDefinition.SavedUnit = newColumnDefinition.Unit; }
                if (definitionRow.ContainsKey("NumFilterMin")) { newColumnDefinition.NumFilterMin = definitionRow["NumFilterMin"].ToDecimal(); newColumnDefinition.SavedNumFilterMin = newColumnDefinition.NumFilterMin; }
                if (definitionRow.ContainsKey("NumFilterMax")) { newColumnDefinition.NumFilterMax = definitionRow["NumFilterMax"].ToDecimal(); newColumnDefinition.SavedNumFilterMax = newColumnDefinition.NumFilterMax; }
                if (definitionRow.ContainsKey("NumFilterStep")) { newColumnDefinition.NumFilterStep = definitionRow["NumFilterStep"].ToDecimal(); newColumnDefinition.SavedNumFilterStep = newColumnDefinition.NumFilterStep; }
                if (definitionRow.ContainsKey("Width")) { newColumnDefinition.Width = definitionRow["Width"].ToInt(); newColumnDefinition.SavedWidth = newColumnDefinition.Width; }
                if (definitionRow.ContainsKey("SettingEnable")) { newColumnDefinition.SettingEnable = definitionRow["SettingEnable"].ToBool(); newColumnDefinition.SavedSettingEnable = newColumnDefinition.SettingEnable; }
                if (definitionRow.ContainsKey("OldColumnName")) { newColumnDefinition.OldColumnName = definitionRow["OldColumnName"].ToString(); newColumnDefinition.SavedOldColumnName = newColumnDefinition.OldColumnName; }
                ColumnDefinitionCollection.Add(newColumnDefinition);
            });
        }

        private static void SetColumnTable(ColumnDefinition definition, XlsRow definitionRow, XlsIo columnxls)
        {
            if (definitionRow.ContainsKey("Id")) { definition.Id = definitionRow["Id"].ToString(); definition.SavedId = definition.Id; }
            if (definitionRow.ContainsKey("ModelName")) { definition.ModelName = definitionRow["ModelName"].ToString(); definition.SavedModelName = definition.ModelName; }
            if (definitionRow.ContainsKey("TableName")) { definition.TableName = definitionRow["TableName"].ToString(); definition.SavedTableName = definition.TableName; }
            if (definitionRow.ContainsKey("Base")) { definition.Base = definitionRow["Base"].ToBool(); definition.SavedBase = definition.Base; }
            if (definitionRow.ContainsKey("EachModel")) { definition.EachModel = definitionRow["EachModel"].ToBool(); definition.SavedEachModel = definition.EachModel; }
            if (definitionRow.ContainsKey("Label")) { definition.Label = definitionRow["Label"].ToString(); definition.SavedLabel = definition.Label; }
            if (definitionRow.ContainsKey("ColumnName")) { definition.ColumnName = definitionRow["ColumnName"].ToString(); definition.SavedColumnName = definition.ColumnName; }
            if (definitionRow.ContainsKey("ColumnLabel")) { definition.ColumnLabel = definitionRow["ColumnLabel"].ToString(); definition.SavedColumnLabel = definition.ColumnLabel; }
            if (definitionRow.ContainsKey("No")) { definition.No = definitionRow["No"].ToInt(); definition.SavedNo = definition.No; }
            if (definitionRow.ContainsKey("History")) { definition.History = definitionRow["History"].ToInt(); definition.SavedHistory = definition.History; }
            if (definitionRow.ContainsKey("Import")) { definition.Import = definitionRow["Import"].ToInt(); definition.SavedImport = definition.Import; }
            if (definitionRow.ContainsKey("Export")) { definition.Export = definitionRow["Export"].ToInt(); definition.SavedExport = definition.Export; }
            if (definitionRow.ContainsKey("GridColumn")) { definition.GridColumn = definitionRow["GridColumn"].ToInt(); definition.SavedGridColumn = definition.GridColumn; }
            if (definitionRow.ContainsKey("GridEnabled")) { definition.GridEnabled = definitionRow["GridEnabled"].ToBool(); definition.SavedGridEnabled = definition.GridEnabled; }
            if (definitionRow.ContainsKey("FilterColumn")) { definition.FilterColumn = definitionRow["FilterColumn"].ToInt(); definition.SavedFilterColumn = definition.FilterColumn; }
            if (definitionRow.ContainsKey("FilterEnabled")) { definition.FilterEnabled = definitionRow["FilterEnabled"].ToBool(); definition.SavedFilterEnabled = definition.FilterEnabled; }
            if (definitionRow.ContainsKey("EditorColumn")) { definition.EditorColumn = definitionRow["EditorColumn"].ToBool(); definition.SavedEditorColumn = definition.EditorColumn; }
            if (definitionRow.ContainsKey("EditorEnabled")) { definition.EditorEnabled = definitionRow["EditorEnabled"].ToBool(); definition.SavedEditorEnabled = definition.EditorEnabled; }
            if (definitionRow.ContainsKey("TitleColumn")) { definition.TitleColumn = definitionRow["TitleColumn"].ToInt(); definition.SavedTitleColumn = definition.TitleColumn; }
            if (definitionRow.ContainsKey("LinkColumn")) { definition.LinkColumn = definitionRow["LinkColumn"].ToInt(); definition.SavedLinkColumn = definition.LinkColumn; }
            if (definitionRow.ContainsKey("LinkEnabled")) { definition.LinkEnabled = definitionRow["LinkEnabled"].ToBool(); definition.SavedLinkEnabled = definition.LinkEnabled; }
            if (definitionRow.ContainsKey("HistoryColumn")) { definition.HistoryColumn = definitionRow["HistoryColumn"].ToInt(); definition.SavedHistoryColumn = definition.HistoryColumn; }
            if (definitionRow.ContainsKey("HistoryEnabled")) { definition.HistoryEnabled = definitionRow["HistoryEnabled"].ToBool(); definition.SavedHistoryEnabled = definition.HistoryEnabled; }
            if (definitionRow.ContainsKey("TypeName")) { definition.TypeName = definitionRow["TypeName"].ToString(); definition.SavedTypeName = definition.TypeName; }
            if (definitionRow.ContainsKey("TypeCs")) { definition.TypeCs = definitionRow["TypeCs"].ToString(); definition.SavedTypeCs = definition.TypeCs; }
            if (definitionRow.ContainsKey("RecordingData")) { definition.RecordingData = definitionRow["RecordingData"].ToString(); definition.SavedRecordingData = definition.RecordingData; }
            if (definitionRow.ContainsKey("MaxLength")) { definition.MaxLength = definitionRow["MaxLength"].ToInt(); definition.SavedMaxLength = definition.MaxLength; }
            if (definitionRow.ContainsKey("Size")) { definition.Size = definitionRow["Size"].ToString(); definition.SavedSize = definition.Size; }
            if (definitionRow.ContainsKey("Pk")) { definition.Pk = definitionRow["Pk"].ToInt(); definition.SavedPk = definition.Pk; }
            if (definitionRow.ContainsKey("PkOrderBy")) { definition.PkOrderBy = definitionRow["PkOrderBy"].ToString(); definition.SavedPkOrderBy = definition.PkOrderBy; }
            if (definitionRow.ContainsKey("PkHistory")) { definition.PkHistory = definitionRow["PkHistory"].ToInt(); definition.SavedPkHistory = definition.PkHistory; }
            if (definitionRow.ContainsKey("PkHistoryOrderBy")) { definition.PkHistoryOrderBy = definitionRow["PkHistoryOrderBy"].ToString(); definition.SavedPkHistoryOrderBy = definition.PkHistoryOrderBy; }
            if (definitionRow.ContainsKey("Ix1")) { definition.Ix1 = definitionRow["Ix1"].ToInt(); definition.SavedIx1 = definition.Ix1; }
            if (definitionRow.ContainsKey("Ix1OrderBy")) { definition.Ix1OrderBy = definitionRow["Ix1OrderBy"].ToString(); definition.SavedIx1OrderBy = definition.Ix1OrderBy; }
            if (definitionRow.ContainsKey("Ix2")) { definition.Ix2 = definitionRow["Ix2"].ToInt(); definition.SavedIx2 = definition.Ix2; }
            if (definitionRow.ContainsKey("Ix2OrderBy")) { definition.Ix2OrderBy = definitionRow["Ix2OrderBy"].ToString(); definition.SavedIx2OrderBy = definition.Ix2OrderBy; }
            if (definitionRow.ContainsKey("Ix3")) { definition.Ix3 = definitionRow["Ix3"].ToInt(); definition.SavedIx3 = definition.Ix3; }
            if (definitionRow.ContainsKey("Ix3OrderBy")) { definition.Ix3OrderBy = definitionRow["Ix3OrderBy"].ToString(); definition.SavedIx3OrderBy = definition.Ix3OrderBy; }
            if (definitionRow.ContainsKey("Nullable")) { definition.Nullable = definitionRow["Nullable"].ToBool(); definition.SavedNullable = definition.Nullable; }
            if (definitionRow.ContainsKey("Default")) { definition.Default = definitionRow["Default"].ToString(); definition.SavedDefault = definition.Default; }
            if (definitionRow.ContainsKey("DefaultCs")) { definition.DefaultCs = definitionRow["DefaultCs"].ToString(); definition.SavedDefaultCs = definition.DefaultCs; }
            if (definitionRow.ContainsKey("Identity")) { definition.Identity = definitionRow["Identity"].ToBool(); definition.SavedIdentity = definition.Identity; }
            if (definitionRow.ContainsKey("Unique")) { definition.Unique = definitionRow["Unique"].ToBool(); definition.SavedUnique = definition.Unique; }
            if (definitionRow.ContainsKey("Seed")) { definition.Seed = definitionRow["Seed"].ToInt(); definition.SavedSeed = definition.Seed; }
            if (definitionRow.ContainsKey("JoinTableName")) { definition.JoinTableName = definitionRow["JoinTableName"].ToString(); definition.SavedJoinTableName = definition.JoinTableName; }
            if (definitionRow.ContainsKey("JoinType")) { definition.JoinType = definitionRow["JoinType"].ToString(); definition.SavedJoinType = definition.JoinType; }
            if (definitionRow.ContainsKey("JoinExpression")) { definition.JoinExpression = definitionRow["JoinExpression"].ToString(); definition.SavedJoinExpression = definition.JoinExpression; }
            if (definitionRow.ContainsKey("Like")) { definition.Like = definitionRow["Like"].ToBool(); definition.SavedLike = definition.Like; }
            if (definitionRow.ContainsKey("WhereSpecial")) { definition.WhereSpecial = definitionRow["WhereSpecial"].ToBool(); definition.SavedWhereSpecial = definition.WhereSpecial; }
            if (definitionRow.ContainsKey("ReadPermission")) { definition.ReadPermission = definitionRow["ReadPermission"].ToString(); definition.SavedReadPermission = definition.ReadPermission; }
            if (definitionRow.ContainsKey("CreatePermission")) { definition.CreatePermission = definitionRow["CreatePermission"].ToString(); definition.SavedCreatePermission = definition.CreatePermission; }
            if (definitionRow.ContainsKey("UpdatePermission")) { definition.UpdatePermission = definitionRow["UpdatePermission"].ToString(); definition.SavedUpdatePermission = definition.UpdatePermission; }
            if (definitionRow.ContainsKey("NotEditSelf")) { definition.NotEditSelf = definitionRow["NotEditSelf"].ToBool(); definition.SavedNotEditSelf = definition.NotEditSelf; }
            if (definitionRow.ContainsKey("SearchIndexPriority")) { definition.SearchIndexPriority = definitionRow["SearchIndexPriority"].ToInt(); definition.SavedSearchIndexPriority = definition.SearchIndexPriority; }
            if (definitionRow.ContainsKey("NotForm")) { definition.NotForm = definitionRow["NotForm"].ToBool(); definition.SavedNotForm = definition.NotForm; }
            if (definitionRow.ContainsKey("NotSelect")) { definition.NotSelect = definitionRow["NotSelect"].ToBool(); definition.SavedNotSelect = definition.NotSelect; }
            if (definitionRow.ContainsKey("NotUpdate")) { definition.NotUpdate = definitionRow["NotUpdate"].ToBool(); definition.SavedNotUpdate = definition.NotUpdate; }
            if (definitionRow.ContainsKey("ByForm")) { definition.ByForm = definitionRow["ByForm"].ToString(); definition.SavedByForm = definition.ByForm; }
            if (definitionRow.ContainsKey("ByDataRow")) { definition.ByDataRow = definitionRow["ByDataRow"].ToString(); definition.SavedByDataRow = definition.ByDataRow; }
            if (definitionRow.ContainsKey("BySession")) { definition.BySession = definitionRow["BySession"].ToString(); definition.SavedBySession = definition.BySession; }
            if (definitionRow.ContainsKey("ToControl")) { definition.ToControl = definitionRow["ToControl"].ToString(); definition.SavedToControl = definition.ToControl; }
            if (definitionRow.ContainsKey("SelectColumns")) { definition.SelectColumns = definitionRow["SelectColumns"].ToString(); definition.SavedSelectColumns = definition.SelectColumns; }
            if (definitionRow.ContainsKey("ComputeColumn")) { definition.ComputeColumn = definitionRow["ComputeColumn"].ToString(); definition.SavedComputeColumn = definition.ComputeColumn; }
            if (definitionRow.ContainsKey("OrderByColumns")) { definition.OrderByColumns = definitionRow["OrderByColumns"].ToString(); definition.SavedOrderByColumns = definition.OrderByColumns; }
            if (definitionRow.ContainsKey("ItemId")) { definition.ItemId = definitionRow["ItemId"].ToInt(); definition.SavedItemId = definition.ItemId; }
            if (definitionRow.ContainsKey("GenericUi")) { definition.GenericUi = definitionRow["GenericUi"].ToBool(); definition.SavedGenericUi = definition.GenericUi; }
            if (definitionRow.ContainsKey("FieldCss")) { definition.FieldCss = definitionRow["FieldCss"].ToString(); definition.SavedFieldCss = definition.FieldCss; }
            if (definitionRow.ContainsKey("ControlCss")) { definition.ControlCss = definitionRow["ControlCss"].ToString(); definition.SavedControlCss = definition.ControlCss; }
            if (definitionRow.ContainsKey("MarkDown")) { definition.MarkDown = definitionRow["MarkDown"].ToBool(); definition.SavedMarkDown = definition.MarkDown; }
            if (definitionRow.ContainsKey("GridStyle")) { definition.GridStyle = definitionRow["GridStyle"].ToString(); definition.SavedGridStyle = definition.GridStyle; }
            if (definitionRow.ContainsKey("Hash")) { definition.Hash = definitionRow["Hash"].ToBool(); definition.SavedHash = definition.Hash; }
            if (definitionRow.ContainsKey("Calc")) { definition.Calc = definitionRow["Calc"].ToString(); definition.SavedCalc = definition.Calc; }
            if (definitionRow.ContainsKey("Session")) { definition.Session = definitionRow["Session"].ToBool(); definition.SavedSession = definition.Session; }
            if (definitionRow.ContainsKey("UserColumn")) { definition.UserColumn = definitionRow["UserColumn"].ToBool(); definition.SavedUserColumn = definition.UserColumn; }
            if (definitionRow.ContainsKey("EnumColumn")) { definition.EnumColumn = definitionRow["EnumColumn"].ToBool(); definition.SavedEnumColumn = definition.EnumColumn; }
            if (definitionRow.ContainsKey("NotEditorSettings")) { definition.NotEditorSettings = definitionRow["NotEditorSettings"].ToBool(); definition.SavedNotEditorSettings = definition.NotEditorSettings; }
            if (definitionRow.ContainsKey("ControlType")) { definition.ControlType = definitionRow["ControlType"].ToString(); definition.SavedControlType = definition.ControlType; }
            if (definitionRow.ContainsKey("GridFormat")) { definition.GridFormat = definitionRow["GridFormat"].ToString(); definition.SavedGridFormat = definition.GridFormat; }
            if (definitionRow.ContainsKey("EditorFormat")) { definition.EditorFormat = definitionRow["EditorFormat"].ToString(); definition.SavedEditorFormat = definition.EditorFormat; }
            if (definitionRow.ContainsKey("ExportFormat")) { definition.ExportFormat = definitionRow["ExportFormat"].ToString(); definition.SavedExportFormat = definition.ExportFormat; }
            if (definitionRow.ContainsKey("Aggregatable")) { definition.Aggregatable = definitionRow["Aggregatable"].ToBool(); definition.SavedAggregatable = definition.Aggregatable; }
            if (definitionRow.ContainsKey("Computable")) { definition.Computable = definitionRow["Computable"].ToBool(); definition.SavedComputable = definition.Computable; }
            if (definitionRow.ContainsKey("ChoicesText")) { definition.ChoicesText = definitionRow["ChoicesText"].ToString(); definition.SavedChoicesText = definition.ChoicesText; }
            if (definitionRow.ContainsKey("UseSearch")) { definition.UseSearch = definitionRow["UseSearch"].ToBool(); definition.SavedUseSearch = definition.UseSearch; }
            if (definitionRow.ContainsKey("DefaultInput")) { definition.DefaultInput = definitionRow["DefaultInput"].ToString(); definition.SavedDefaultInput = definition.DefaultInput; }
            if (definitionRow.ContainsKey("Own")) { definition.Own = definitionRow["Own"].ToBool(); definition.SavedOwn = definition.Own; }
            if (definitionRow.ContainsKey("FormName")) { definition.FormName = definitionRow["FormName"].ToString(); definition.SavedFormName = definition.FormName; }
            if (definitionRow.ContainsKey("ValidateRequired")) { definition.ValidateRequired = definitionRow["ValidateRequired"].ToBool(); definition.SavedValidateRequired = definition.ValidateRequired; }
            if (definitionRow.ContainsKey("ValidateNumber")) { definition.ValidateNumber = definitionRow["ValidateNumber"].ToBool(); definition.SavedValidateNumber = definition.ValidateNumber; }
            if (definitionRow.ContainsKey("ValidateDate")) { definition.ValidateDate = definitionRow["ValidateDate"].ToBool(); definition.SavedValidateDate = definition.ValidateDate; }
            if (definitionRow.ContainsKey("ValidateEmail")) { definition.ValidateEmail = definitionRow["ValidateEmail"].ToBool(); definition.SavedValidateEmail = definition.ValidateEmail; }
            if (definitionRow.ContainsKey("ValidateEqualTo")) { definition.ValidateEqualTo = definitionRow["ValidateEqualTo"].ToString(); definition.SavedValidateEqualTo = definition.ValidateEqualTo; }
            if (definitionRow.ContainsKey("DecimalPlaces")) { definition.DecimalPlaces = definitionRow["DecimalPlaces"].ToInt(); definition.SavedDecimalPlaces = definition.DecimalPlaces; }
            if (definitionRow.ContainsKey("Min")) { definition.Min = definitionRow["Min"].ToDecimal(); definition.SavedMin = definition.Min; }
            if (definitionRow.ContainsKey("Max")) { definition.Max = definitionRow["Max"].ToDecimal(); definition.SavedMax = definition.Max; }
            if (definitionRow.ContainsKey("Step")) { definition.Step = definitionRow["Step"].ToDecimal(); definition.SavedStep = definition.Step; }
            if (definitionRow.ContainsKey("StringFormat")) { definition.StringFormat = definitionRow["StringFormat"].ToString(); definition.SavedStringFormat = definition.StringFormat; }
            if (definitionRow.ContainsKey("Unit")) { definition.Unit = definitionRow["Unit"].ToString(); definition.SavedUnit = definition.Unit; }
            if (definitionRow.ContainsKey("NumFilterMin")) { definition.NumFilterMin = definitionRow["NumFilterMin"].ToDecimal(); definition.SavedNumFilterMin = definition.NumFilterMin; }
            if (definitionRow.ContainsKey("NumFilterMax")) { definition.NumFilterMax = definitionRow["NumFilterMax"].ToDecimal(); definition.SavedNumFilterMax = definition.NumFilterMax; }
            if (definitionRow.ContainsKey("NumFilterStep")) { definition.NumFilterStep = definitionRow["NumFilterStep"].ToDecimal(); definition.SavedNumFilterStep = definition.NumFilterStep; }
            if (definitionRow.ContainsKey("Width")) { definition.Width = definitionRow["Width"].ToInt(); definition.SavedWidth = definition.Width; }
            if (definitionRow.ContainsKey("SettingEnable")) { definition.SettingEnable = definitionRow["SettingEnable"].ToBool(); definition.SavedSettingEnable = definition.SettingEnable; }
            if (definitionRow.ContainsKey("OldColumnName")) { definition.OldColumnName = definitionRow["OldColumnName"].ToString(); definition.SavedOldColumnName = definition.OldColumnName; }
        }

        private static void ConstructColumnDefinitions()
        {
            ColumnXls = Initializer.DefinitionFile("definition_Column.xlsm");
            ColumnDefinitionCollection = new List<ColumnDefinition>();
            Column = new ColumnColumn2nd();
            ColumnTable = new ColumnTable();
        }

        public static XlsIo CssXls;
        public static List<CssDefinition> CssDefinitionCollection;
        public static CssColumn2nd Css;
        public static CssTable CssTable;

        public static void SetCssDefinition()
        {
            ConstructCssDefinitions();
            if (CssXls.AccessStatus != Files.AccessStatuses.Read) { return; }
            CssXls.XlsSheet.ForEach(definitionRow =>
            {
                switch (definitionRow[0].ToString())
                {
                    case "html": Css.html = definitionRow[1].ToString(); SetCssTable(CssTable.html, definitionRow, CssXls); break;
                    case "body": Css.body = definitionRow[1].ToString(); SetCssTable(CssTable.body, definitionRow, CssXls); break;
                    case "h1": Css.h1 = definitionRow[1].ToString(); SetCssTable(CssTable.h1, definitionRow, CssXls); break;
                    case "h2": Css.h2 = definitionRow[1].ToString(); SetCssTable(CssTable.h2, definitionRow, CssXls); break;
                    case "h3": Css.h3 = definitionRow[1].ToString(); SetCssTable(CssTable.h3, definitionRow, CssXls); break;
                    case "legend": Css.legend = definitionRow[1].ToString(); SetCssTable(CssTable.legend, definitionRow, CssXls); break;
                    case "legend_space___space__asterisk_": Css.legend_space___space__asterisk_ = definitionRow[1].ToString(); SetCssTable(CssTable.legend_space___space__asterisk_, definitionRow, CssXls); break;
                    case "label": Css.label = definitionRow[1].ToString(); SetCssTable(CssTable.label, definitionRow, CssXls); break;
                    case "input": Css.input = definitionRow[1].ToString(); SetCssTable(CssTable.input, definitionRow, CssXls); break;
                    case "select": Css.select = definitionRow[1].ToString(); SetCssTable(CssTable.select, definitionRow, CssXls); break;
                    case "table": Css.table = definitionRow[1].ToString(); SetCssTable(CssTable.table, definitionRow, CssXls); break;
                    case "td": Css.td = definitionRow[1].ToString(); SetCssTable(CssTable.td, definitionRow, CssXls); break;
                    case "pre": Css.pre = definitionRow[1].ToString(); SetCssTable(CssTable.pre, definitionRow, CssXls); break;
                    case "_sharp_Logo": Css._sharp_Logo = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_Logo, definitionRow, CssXls); break;
                    case "_sharp_CorpLogo": Css._sharp_CorpLogo = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_CorpLogo, definitionRow, CssXls); break;
                    case "_sharp_ProductLogo": Css._sharp_ProductLogo = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_ProductLogo, definitionRow, CssXls); break;
                    case "_sharp_Login": Css._sharp_Login = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_Login, definitionRow, CssXls); break;
                    case "_sharp_LoginCommands": Css._sharp_LoginCommands = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_LoginCommands, definitionRow, CssXls); break;
                    case "_sharp_Demo": Css._sharp_Demo = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_Demo, definitionRow, CssXls); break;
                    case "_sharp_DemoFields": Css._sharp_DemoFields = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_DemoFields, definitionRow, CssXls); break;
                    case "_sharp_Breadcrumb": Css._sharp_Breadcrumb = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_Breadcrumb, definitionRow, CssXls); break;
                    case "_sharp_Breadcrumb_space__dot_item": Css._sharp_Breadcrumb_space__dot_item = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_Breadcrumb_space__dot_item, definitionRow, CssXls); break;
                    case "_sharp_Breadcrumb_space__dot_separator": Css._sharp_Breadcrumb_space__dot_separator = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_Breadcrumb_space__dot_separator, definitionRow, CssXls); break;
                    case "_sharp_Header": Css._sharp_Header = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_Header, definitionRow, CssXls); break;
                    case "_sharp_Navigations": Css._sharp_Navigations = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_Navigations, definitionRow, CssXls); break;
                    case "_sharp_NavigationMenu": Css._sharp_NavigationMenu = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_NavigationMenu, definitionRow, CssXls); break;
                    case "_sharp_NavigationMenu_space___space_li": Css._sharp_NavigationMenu_space___space_li = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_NavigationMenu_space___space_li, definitionRow, CssXls); break;
                    case "_sharp_NavigationMenu_space___space_li_space___space_div": Css._sharp_NavigationMenu_space___space_li_space___space_div = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_NavigationMenu_space___space_li_space___space_div, definitionRow, CssXls); break;
                    case "_sharp_NavigationMenu_space___space_li_space___space_div_colon_hover": Css._sharp_NavigationMenu_space___space_li_space___space_div_colon_hover = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_NavigationMenu_space___space_li_space___space_div_colon_hover, definitionRow, CssXls); break;
                    case "_sharp_NavigationMenu_space___space_li_dot_sub_menu_space___space_div_dot_hover": Css._sharp_NavigationMenu_space___space_li_dot_sub_menu_space___space_div_dot_hover = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_NavigationMenu_space___space_li_dot_sub_menu_space___space_div_dot_hover, definitionRow, CssXls); break;
                    case "_sharp_NavigationMenu_space___space_li_space___space_div_space___space_a": Css._sharp_NavigationMenu_space___space_li_space___space_div_space___space_a = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_NavigationMenu_space___space_li_space___space_div_space___space_a, definitionRow, CssXls); break;
                    case "_sharp_NavigationMenu_space___space_li_space___space_div_colon_hover_space___space_a": Css._sharp_NavigationMenu_space___space_li_space___space_div_colon_hover_space___space_a = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_NavigationMenu_space___space_li_space___space_div_colon_hover_space___space_a, definitionRow, CssXls); break;
                    case "_sharp_NavigationMenu_space__dot_menu": Css._sharp_NavigationMenu_space__dot_menu = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_NavigationMenu_space__dot_menu, definitionRow, CssXls); break;
                    case "_sharp_NavigationMenu_space__dot_menu_space___space_li_space___space_a": Css._sharp_NavigationMenu_space__dot_menu_space___space_li_space___space_a = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_NavigationMenu_space__dot_menu_space___space_li_space___space_a, definitionRow, CssXls); break;
                    case "_sharp_NavigationMenu_space__dot_menu_space___space_li_space___space_a_dot_ui_state_active": Css._sharp_NavigationMenu_space__dot_menu_space___space_li_space___space_a_dot_ui_state_active = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_NavigationMenu_space__dot_menu_space___space_li_space___space_a_dot_ui_state_active, definitionRow, CssXls); break;
                    case "_sharp_SearchField": Css._sharp_SearchField = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_SearchField, definitionRow, CssXls); break;
                    case "_sharp_Search": Css._sharp_Search = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_Search, definitionRow, CssXls); break;
                    case "_sharp_Application": Css._sharp_Application = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_Application, definitionRow, CssXls); break;
                    case "_sharp_Application_space___space__dot_site_image_icon": Css._sharp_Application_space___space__dot_site_image_icon = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_Application_space___space__dot_site_image_icon, definitionRow, CssXls); break;
                    case "_sharp_HeaderTitleContainer": Css._sharp_HeaderTitleContainer = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_HeaderTitleContainer, definitionRow, CssXls); break;
                    case "_sharp_HeaderTitle": Css._sharp_HeaderTitle = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_HeaderTitle, definitionRow, CssXls); break;
                    case "_sharp_Notes_space___space__asterisk_": Css._sharp_Notes_space___space__asterisk_ = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_Notes_space___space__asterisk_, definitionRow, CssXls); break;
                    case "_sharp_Notes_space___space__dot_history": Css._sharp_Notes_space___space__dot_history = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_Notes_space___space__dot_history, definitionRow, CssXls); break;
                    case "_sharp_Notes_space___space__dot_readonly": Css._sharp_Notes_space___space__dot_readonly = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_Notes_space___space__dot_readonly, definitionRow, CssXls); break;
                    case "_sharp_ViewSelectorField": Css._sharp_ViewSelectorField = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_ViewSelectorField, definitionRow, CssXls); break;
                    case "_sharp_ViewFilters": Css._sharp_ViewFilters = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_ViewFilters, definitionRow, CssXls); break;
                    case "_sharp_ViewFilters_dot_reduced": Css._sharp_ViewFilters_dot_reduced = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_ViewFilters_dot_reduced, definitionRow, CssXls); break;
                    case "_sharp_ViewFilters_space___space__dot_field_auto_thin": Css._sharp_ViewFilters_space___space__dot_field_auto_thin = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_ViewFilters_space___space__dot_field_auto_thin, definitionRow, CssXls); break;
                    case "_sharp_ViewFilters_Reset": Css._sharp_ViewFilters_Reset = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_ViewFilters_Reset, definitionRow, CssXls); break;
                    case "_sharp_ViewFilters_space___space__dot_display_control": Css._sharp_ViewFilters_space___space__dot_display_control = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_ViewFilters_space___space__dot_display_control, definitionRow, CssXls); break;
                    case "_sharp_Aggregations": Css._sharp_Aggregations = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_Aggregations, definitionRow, CssXls); break;
                    case "_sharp_Aggregations_dot_reduced": Css._sharp_Aggregations_dot_reduced = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_Aggregations_dot_reduced, definitionRow, CssXls); break;
                    case "_sharp_Aggregations_space__dot_label": Css._sharp_Aggregations_space__dot_label = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_Aggregations_space__dot_label, definitionRow, CssXls); break;
                    case "_sharp_Aggregations_space__dot_label_dot_overdue": Css._sharp_Aggregations_space__dot_label_dot_overdue = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_Aggregations_space__dot_label_dot_overdue, definitionRow, CssXls); break;
                    case "_sharp_Aggregations_space__dot_data": Css._sharp_Aggregations_space__dot_data = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_Aggregations_space__dot_data, definitionRow, CssXls); break;
                    case "_sharp_Aggregations_space__dot_data_dot_overdue": Css._sharp_Aggregations_space__dot_data_dot_overdue = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_Aggregations_space__dot_data_dot_overdue, definitionRow, CssXls); break;
                    case "_sharp_Aggregations_space_em": Css._sharp_Aggregations_space_em = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_Aggregations_space_em, definitionRow, CssXls); break;
                    case "_sharp_Aggregations_space___space__dot_display_control": Css._sharp_Aggregations_space___space__dot_display_control = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_Aggregations_space___space__dot_display_control, definitionRow, CssXls); break;
                    case "_sharp_Gantt": Css._sharp_Gantt = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_Gantt, definitionRow, CssXls); break;
                    case "_sharp_Gantt_space__dot_date": Css._sharp_Gantt_space__dot_date = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_Gantt_space__dot_date, definitionRow, CssXls); break;
                    case "_sharp_Gantt_space__dot_now": Css._sharp_Gantt_space__dot_now = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_Gantt_space__dot_now, definitionRow, CssXls); break;
                    case "_sharp_Gantt_space__dot_planned_space_rect": Css._sharp_Gantt_space__dot_planned_space_rect = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_Gantt_space__dot_planned_space_rect, definitionRow, CssXls); break;
                    case "_sharp_Gantt_space__dot_planned_space_rect_dot_summary": Css._sharp_Gantt_space__dot_planned_space_rect_dot_summary = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_Gantt_space__dot_planned_space_rect_dot_summary, definitionRow, CssXls); break;
                    case "_sharp_Gantt_space__dot_earned_space_rect": Css._sharp_Gantt_space__dot_earned_space_rect = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_Gantt_space__dot_earned_space_rect, definitionRow, CssXls); break;
                    case "_sharp_Gantt_space__dot_earned_space_rect_dot_summary": Css._sharp_Gantt_space__dot_earned_space_rect_dot_summary = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_Gantt_space__dot_earned_space_rect_dot_summary, definitionRow, CssXls); break;
                    case "_sharp_Gantt_space_rect_dot_delay": Css._sharp_Gantt_space_rect_dot_delay = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_Gantt_space_rect_dot_delay, definitionRow, CssXls); break;
                    case "_sharp_Gantt_space_rect_dot_delay_dot_summary": Css._sharp_Gantt_space_rect_dot_delay_dot_summary = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_Gantt_space_rect_dot_delay_dot_summary, definitionRow, CssXls); break;
                    case "_sharp_Gantt_space_rect_dot_completed": Css._sharp_Gantt_space_rect_dot_completed = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_Gantt_space_rect_dot_completed, definitionRow, CssXls); break;
                    case "_sharp_Gantt_space__dot_title_space_text": Css._sharp_Gantt_space__dot_title_space_text = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_Gantt_space__dot_title_space_text, definitionRow, CssXls); break;
                    case "_sharp_Gantt_space__dot_title_space_text_dot_delay": Css._sharp_Gantt_space__dot_title_space_text_dot_delay = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_Gantt_space__dot_title_space_text_dot_delay, definitionRow, CssXls); break;
                    case "_sharp_Gantt_space__dot_title_space_text_dot_summary": Css._sharp_Gantt_space__dot_title_space_text_dot_summary = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_Gantt_space__dot_title_space_text_dot_summary, definitionRow, CssXls); break;
                    case "_sharp_GanttAxis": Css._sharp_GanttAxis = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_GanttAxis, definitionRow, CssXls); break;
                    case "_sharp_GanttAxis_space_text": Css._sharp_GanttAxis_space_text = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_GanttAxis_space_text, definitionRow, CssXls); break;
                    case "_sharp_BurnDown": Css._sharp_BurnDown = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_BurnDown, definitionRow, CssXls); break;
                    case "_sharp_BurnDown_space__dot_now": Css._sharp_BurnDown_space__dot_now = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_BurnDown_space__dot_now, definitionRow, CssXls); break;
                    case "_sharp_BurnDown_space__dot_total": Css._sharp_BurnDown_space__dot_total = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_BurnDown_space__dot_total, definitionRow, CssXls); break;
                    case "_sharp_BurnDown_space__dot_planned": Css._sharp_BurnDown_space__dot_planned = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_BurnDown_space__dot_planned, definitionRow, CssXls); break;
                    case "_sharp_BurnDown_space__dot_earned": Css._sharp_BurnDown_space__dot_earned = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_BurnDown_space__dot_earned, definitionRow, CssXls); break;
                    case "_sharp_BurnDown_space__dot_total_space_circle": Css._sharp_BurnDown_space__dot_total_space_circle = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_BurnDown_space__dot_total_space_circle, definitionRow, CssXls); break;
                    case "_sharp_BurnDown_space__dot_planned_space_circle": Css._sharp_BurnDown_space__dot_planned_space_circle = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_BurnDown_space__dot_planned_space_circle, definitionRow, CssXls); break;
                    case "_sharp_BurnDown_space__dot_earned_space_circle": Css._sharp_BurnDown_space__dot_earned_space_circle = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_BurnDown_space__dot_earned_space_circle, definitionRow, CssXls); break;
                    case "_sharp_BurnDownDetails_space___space_tbody_space___space_tr_space___space_td": Css._sharp_BurnDownDetails_space___space_tbody_space___space_tr_space___space_td = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_BurnDownDetails_space___space_tbody_space___space_tr_space___space_td, definitionRow, CssXls); break;
                    case "_sharp_BurnDownDetails_space___space_tbody_space___space_tr_space___space_td_dot_warning": Css._sharp_BurnDownDetails_space___space_tbody_space___space_tr_space___space_td_dot_warning = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_BurnDownDetails_space___space_tbody_space___space_tr_space___space_td_dot_warning, definitionRow, CssXls); break;
                    case "_sharp_BurnDownDetails_space___space_tbody_space___space_tr_space___space_td_dot_difference": Css._sharp_BurnDownDetails_space___space_tbody_space___space_tr_space___space_td_dot_difference = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_BurnDownDetails_space___space_tbody_space___space_tr_space___space_td_dot_difference, definitionRow, CssXls); break;
                    case "_sharp_BurnDownDetails_space___space_tbody_space___space_tr_space___space_td_dot_difference_dot_warning": Css._sharp_BurnDownDetails_space___space_tbody_space___space_tr_space___space_td_dot_difference_dot_warning = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_BurnDownDetails_space___space_tbody_space___space_tr_space___space_td_dot_difference_dot_warning, definitionRow, CssXls); break;
                    case "_sharp_BurnDownDetails_space__dot_user_info": Css._sharp_BurnDownDetails_space__dot_user_info = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_BurnDownDetails_space__dot_user_info, definitionRow, CssXls); break;
                    case "_sharp_BurnDownDetails_space__dot_items": Css._sharp_BurnDownDetails_space__dot_items = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_BurnDownDetails_space__dot_items, definitionRow, CssXls); break;
                    case "_sharp_BurnDownDetails_space__dot_items_space_a": Css._sharp_BurnDownDetails_space__dot_items_space_a = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_BurnDownDetails_space__dot_items_space_a, definitionRow, CssXls); break;
                    case "_sharp_BurnDownDetails_space__dot_items_space_a_colon_hover": Css._sharp_BurnDownDetails_space__dot_items_space_a_colon_hover = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_BurnDownDetails_space__dot_items_space_a_colon_hover, definitionRow, CssXls); break;
                    case "_sharp_TimeSeries": Css._sharp_TimeSeries = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_TimeSeries, definitionRow, CssXls); break;
                    case "_sharp_TimeSeries_space__dot_surface": Css._sharp_TimeSeries_space__dot_surface = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_TimeSeries_space__dot_surface, definitionRow, CssXls); break;
                    case "_sharp_TimeSeries_space__dot_index": Css._sharp_TimeSeries_space__dot_index = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_TimeSeries_space__dot_index, definitionRow, CssXls); break;
                    case "_sharp_KambanBody_space__dot_kamban_row": Css._sharp_KambanBody_space__dot_kamban_row = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_KambanBody_space__dot_kamban_row, definitionRow, CssXls); break;
                    case "_sharp_KambanBody_space__dot_kamban_container_space___space_div": Css._sharp_KambanBody_space__dot_kamban_container_space___space_div = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_KambanBody_space__dot_kamban_container_space___space_div, definitionRow, CssXls); break;
                    case "_sharp_KambanBody_space__dot_kamban_container_dot_hover": Css._sharp_KambanBody_space__dot_kamban_container_dot_hover = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_KambanBody_space__dot_kamban_container_dot_hover, definitionRow, CssXls); break;
                    case "_sharp_KambanBody_space__dot_kamban_container_space__dot_kamban_item_colon_last_child": Css._sharp_KambanBody_space__dot_kamban_container_space__dot_kamban_item_colon_last_child = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_KambanBody_space__dot_kamban_container_space__dot_kamban_item_colon_last_child, definitionRow, CssXls); break;
                    case "_sharp_KambanBody_space__dot_kamban_item": Css._sharp_KambanBody_space__dot_kamban_item = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_KambanBody_space__dot_kamban_item, definitionRow, CssXls); break;
                    case "_sharp_KambanBody_space__dot_kamban_item_colon_hover": Css._sharp_KambanBody_space__dot_kamban_item_colon_hover = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_KambanBody_space__dot_kamban_item_colon_hover, definitionRow, CssXls); break;
                    case "_sharp_KambanBody_space__dot_kamban_item_dot_changed": Css._sharp_KambanBody_space__dot_kamban_item_dot_changed = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_KambanBody_space__dot_kamban_item_dot_changed, definitionRow, CssXls); break;
                    case "_sharp_KambanBody_space__dot_kamban_item_space__dot_ui_icon": Css._sharp_KambanBody_space__dot_kamban_item_space__dot_ui_icon = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_KambanBody_space__dot_kamban_item_space__dot_ui_icon, definitionRow, CssXls); break;
                    case "_sharp_RecordHeader": Css._sharp_RecordHeader = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_RecordHeader, definitionRow, CssXls); break;
                    case "_sharp_RecordInfo": Css._sharp_RecordInfo = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_RecordInfo, definitionRow, CssXls); break;
                    case "_sharp_RecordInfo_space_div": Css._sharp_RecordInfo_space_div = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_RecordInfo_space_div, definitionRow, CssXls); break;
                    case "_sharp_RecordInfo_space_div_space_p": Css._sharp_RecordInfo_space_div_space_p = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_RecordInfo_space_div_space_p, definitionRow, CssXls); break;
                    case "_sharp_RecordInfo_space_div_space_p_space__dot_elapsed_time": Css._sharp_RecordInfo_space_div_space_p_space__dot_elapsed_time = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_RecordInfo_space_div_space_p_space__dot_elapsed_time, definitionRow, CssXls); break;
                    case "_sharp_RecordSwitchers": Css._sharp_RecordSwitchers = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_RecordSwitchers, definitionRow, CssXls); break;
                    case "_sharp_RecordSwitchers_space___space__asterisk_": Css._sharp_RecordSwitchers_space___space__asterisk_ = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_RecordSwitchers_space___space__asterisk_, definitionRow, CssXls); break;
                    case "_sharp_RecordSwitchers_space__dot_current": Css._sharp_RecordSwitchers_space__dot_current = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_RecordSwitchers_space__dot_current, definitionRow, CssXls); break;
                    case "_sharp_Editor": Css._sharp_Editor = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_Editor, definitionRow, CssXls); break;
                    case "_sharp_EditorTabsContainer": Css._sharp_EditorTabsContainer = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_EditorTabsContainer, definitionRow, CssXls); break;
                    case "_sharp_EditorTabsContainer_dot_max": Css._sharp_EditorTabsContainer_dot_max = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_EditorTabsContainer_dot_max, definitionRow, CssXls); break;
                    case "_sharp_MailEditorTabsContainer": Css._sharp_MailEditorTabsContainer = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_MailEditorTabsContainer, definitionRow, CssXls); break;
                    case "_sharp_EditorComments": Css._sharp_EditorComments = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_EditorComments, definitionRow, CssXls); break;
                    case "_sharp_EditorComments_space__dot_title_header": Css._sharp_EditorComments_space__dot_title_header = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_EditorComments_space__dot_title_header, definitionRow, CssXls); break;
                    case "_sharp_CommentField": Css._sharp_CommentField = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_CommentField, definitionRow, CssXls); break;
                    case "_sharp_OutgoingMailsForm": Css._sharp_OutgoingMailsForm = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_OutgoingMailsForm, definitionRow, CssXls); break;
                    case "_sharp_OutgoingMailsForm_space___space__dot_item": Css._sharp_OutgoingMailsForm_space___space__dot_item = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_OutgoingMailsForm_space___space__dot_item, definitionRow, CssXls); break;
                    case "_sharp_OutgoingMailsForm_space__dot_content": Css._sharp_OutgoingMailsForm_space__dot_content = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_OutgoingMailsForm_space__dot_content, definitionRow, CssXls); break;
                    case "_sharp_DropDownSearchDialogForm": Css._sharp_DropDownSearchDialogForm = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_DropDownSearchDialogForm, definitionRow, CssXls); break;
                    case "_sharp_ViewTabsContainer": Css._sharp_ViewTabsContainer = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_ViewTabsContainer, definitionRow, CssXls); break;
                    case "_sharp_SearchResults": Css._sharp_SearchResults = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_SearchResults, definitionRow, CssXls); break;
                    case "_sharp_SearchResults_space__dot_count": Css._sharp_SearchResults_space__dot_count = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_SearchResults_space__dot_count, definitionRow, CssXls); break;
                    case "_sharp_SearchResults_space__dot_count_space__dot_label": Css._sharp_SearchResults_space__dot_count_space__dot_label = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_SearchResults_space__dot_count_space__dot_label, definitionRow, CssXls); break;
                    case "_sharp_SearchResults_space__dot_count_space__dot_data": Css._sharp_SearchResults_space__dot_count_space__dot_data = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_SearchResults_space__dot_count_space__dot_data, definitionRow, CssXls); break;
                    case "_sharp_SearchResults_space__dot_result": Css._sharp_SearchResults_space__dot_result = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_SearchResults_space__dot_result, definitionRow, CssXls); break;
                    case "_sharp_SearchResults_space__dot_result_space___space_ul": Css._sharp_SearchResults_space__dot_result_space___space_ul = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_SearchResults_space__dot_result_space___space_ul, definitionRow, CssXls); break;
                    case "_sharp_SearchResults_space__dot_result_space___space_h3": Css._sharp_SearchResults_space__dot_result_space___space_h3 = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_SearchResults_space__dot_result_space___space_h3, definitionRow, CssXls); break;
                    case "_sharp_SearchResults_space__dot_result_space___space_h3_space___space_a": Css._sharp_SearchResults_space__dot_result_space___space_h3_space___space_a = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_SearchResults_space__dot_result_space___space_h3_space___space_a, definitionRow, CssXls); break;
                    case "_sharp_SearchResults_space__dot_result_space___space_p": Css._sharp_SearchResults_space__dot_result_space___space_p = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_SearchResults_space__dot_result_space___space_p, definitionRow, CssXls); break;
                    case "_sharp_SearchResults_space__dot_result_colon_hover": Css._sharp_SearchResults_space__dot_result_colon_hover = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_SearchResults_space__dot_result_colon_hover, definitionRow, CssXls); break;
                    case "_sharp_MainCommandsContainer": Css._sharp_MainCommandsContainer = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_MainCommandsContainer, definitionRow, CssXls); break;
                    case "_sharp_MainCommands": Css._sharp_MainCommands = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_MainCommands, definitionRow, CssXls); break;
                    case "_sharp_MainCommands_space___space_button": Css._sharp_MainCommands_space___space_button = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_MainCommands_space___space_button, definitionRow, CssXls); break;
                    case "_sharp_BottomMargin": Css._sharp_BottomMargin = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_BottomMargin, definitionRow, CssXls); break;
                    case "_sharp_Footer": Css._sharp_Footer = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_Footer, definitionRow, CssXls); break;
                    case "_sharp_Footer_space_a": Css._sharp_Footer_space_a = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_Footer_space_a, definitionRow, CssXls); break;
                    case "_dot_main_form": Css._dot_main_form = definitionRow[1].ToString(); SetCssTable(CssTable._dot_main_form, definitionRow, CssXls); break;
                    case "_dot_nav_sites": Css._dot_nav_sites = definitionRow[1].ToString(); SetCssTable(CssTable._dot_nav_sites, definitionRow, CssXls); break;
                    case "_dot_nav_site": Css._dot_nav_site = definitionRow[1].ToString(); SetCssTable(CssTable._dot_nav_site, definitionRow, CssXls); break;
                    case "_dot_nav_site_dot_sites": Css._dot_nav_site_dot_sites = definitionRow[1].ToString(); SetCssTable(CssTable._dot_nav_site_dot_sites, definitionRow, CssXls); break;
                    case "_dot_nav_site_space__dot_heading": Css._dot_nav_site_space__dot_heading = definitionRow[1].ToString(); SetCssTable(CssTable._dot_nav_site_space__dot_heading, definitionRow, CssXls); break;
                    case "_dot_nav_site_space__dot_stacking1": Css._dot_nav_site_space__dot_stacking1 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_nav_site_space__dot_stacking1, definitionRow, CssXls); break;
                    case "_dot_nav_site_space__dot_stacking2": Css._dot_nav_site_space__dot_stacking2 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_nav_site_space__dot_stacking2, definitionRow, CssXls); break;
                    case "_dot_nav_site_space_a": Css._dot_nav_site_space_a = definitionRow[1].ToString(); SetCssTable(CssTable._dot_nav_site_space_a, definitionRow, CssXls); break;
                    case "_dot_nav_site_space_a_colon_hover": Css._dot_nav_site_space_a_colon_hover = definitionRow[1].ToString(); SetCssTable(CssTable._dot_nav_site_space_a_colon_hover, definitionRow, CssXls); break;
                    case "_dot_nav_site_dot_has_image_space_a": Css._dot_nav_site_dot_has_image_space_a = definitionRow[1].ToString(); SetCssTable(CssTable._dot_nav_site_dot_has_image_space_a, definitionRow, CssXls); break;
                    case "_dot_nav_site_space_span_dot_title": Css._dot_nav_site_space_span_dot_title = definitionRow[1].ToString(); SetCssTable(CssTable._dot_nav_site_space_span_dot_title, definitionRow, CssXls); break;
                    case "_dot_nav_site_dot_to_parent": Css._dot_nav_site_dot_to_parent = definitionRow[1].ToString(); SetCssTable(CssTable._dot_nav_site_dot_to_parent, definitionRow, CssXls); break;
                    case "_dot_nav_site_dot_to_parent_space_a": Css._dot_nav_site_dot_to_parent_space_a = definitionRow[1].ToString(); SetCssTable(CssTable._dot_nav_site_dot_to_parent_space_a, definitionRow, CssXls); break;
                    case "_dot_nav_site_dot_to_parent_dot_has_image_space_a": Css._dot_nav_site_dot_to_parent_dot_has_image_space_a = definitionRow[1].ToString(); SetCssTable(CssTable._dot_nav_site_dot_to_parent_dot_has_image_space_a, definitionRow, CssXls); break;
                    case "_dot_nav_site_dot_to_parent_space__dot_ui_icon": Css._dot_nav_site_dot_to_parent_space__dot_ui_icon = definitionRow[1].ToString(); SetCssTable(CssTable._dot_nav_site_dot_to_parent_space__dot_ui_icon, definitionRow, CssXls); break;
                    case "_dot_nav_site_space__dot_site_image_thumbnail": Css._dot_nav_site_space__dot_site_image_thumbnail = definitionRow[1].ToString(); SetCssTable(CssTable._dot_nav_site_space__dot_site_image_thumbnail, definitionRow, CssXls); break;
                    case "_dot_nav_site_space__dot_site_image_icon": Css._dot_nav_site_space__dot_site_image_icon = definitionRow[1].ToString(); SetCssTable(CssTable._dot_nav_site_space__dot_site_image_icon, definitionRow, CssXls); break;
                    case "_dot_nav_site_space__dot_conditions": Css._dot_nav_site_space__dot_conditions = definitionRow[1].ToString(); SetCssTable(CssTable._dot_nav_site_space__dot_conditions, definitionRow, CssXls); break;
                    case "_dot_nav_site_space__dot_conditions_space_span": Css._dot_nav_site_space__dot_conditions_space_span = definitionRow[1].ToString(); SetCssTable(CssTable._dot_nav_site_space__dot_conditions_space_span, definitionRow, CssXls); break;
                    case "_dot_nav_site_space__dot_conditions_space_span_dot_overdue": Css._dot_nav_site_space__dot_conditions_space_span_dot_overdue = definitionRow[1].ToString(); SetCssTable(CssTable._dot_nav_site_space__dot_conditions_space_span_dot_overdue, definitionRow, CssXls); break;
                    case "_dot_nav_site_space__dot_conditions_space_span_dot_elapsed_time_dot_old": Css._dot_nav_site_space__dot_conditions_space_span_dot_elapsed_time_dot_old = definitionRow[1].ToString(); SetCssTable(CssTable._dot_nav_site_space__dot_conditions_space_span_dot_elapsed_time_dot_old, definitionRow, CssXls); break;
                    case "_dot_error_page": Css._dot_error_page = definitionRow[1].ToString(); SetCssTable(CssTable._dot_error_page, definitionRow, CssXls); break;
                    case "_dot_error_page_title": Css._dot_error_page_title = definitionRow[1].ToString(); SetCssTable(CssTable._dot_error_page_title, definitionRow, CssXls); break;
                    case "_dot_error_page_body": Css._dot_error_page_body = definitionRow[1].ToString(); SetCssTable(CssTable._dot_error_page_body, definitionRow, CssXls); break;
                    case "_dot_error_page_message": Css._dot_error_page_message = definitionRow[1].ToString(); SetCssTable(CssTable._dot_error_page_message, definitionRow, CssXls); break;
                    case "_dot_error_page_action": Css._dot_error_page_action = definitionRow[1].ToString(); SetCssTable(CssTable._dot_error_page_action, definitionRow, CssXls); break;
                    case "_dot_error_page_action_space_em": Css._dot_error_page_action_space_em = definitionRow[1].ToString(); SetCssTable(CssTable._dot_error_page_action_space_em, definitionRow, CssXls); break;
                    case "_dot_error_page_stacktrace": Css._dot_error_page_stacktrace = definitionRow[1].ToString(); SetCssTable(CssTable._dot_error_page_stacktrace, definitionRow, CssXls); break;
                    case "_dot_fieldset": Css._dot_fieldset = definitionRow[1].ToString(); SetCssTable(CssTable._dot_fieldset, definitionRow, CssXls); break;
                    case "_dot_fieldset_dot_enclosed": Css._dot_fieldset_dot_enclosed = definitionRow[1].ToString(); SetCssTable(CssTable._dot_fieldset_dot_enclosed, definitionRow, CssXls); break;
                    case "_dot_fieldset_dot_enclosed_half": Css._dot_fieldset_dot_enclosed_half = definitionRow[1].ToString(); SetCssTable(CssTable._dot_fieldset_dot_enclosed_half, definitionRow, CssXls); break;
                    case "_dot_fieldset_dot_enclosed_thin": Css._dot_fieldset_dot_enclosed_thin = definitionRow[1].ToString(); SetCssTable(CssTable._dot_fieldset_dot_enclosed_thin, definitionRow, CssXls); break;
                    case "_dot_fieldset_dot_enclosed_thin_space__class_asterisk__equal__yen_field_auto_yen__": Css._dot_fieldset_dot_enclosed_thin_space__class_asterisk__equal__yen_field_auto_yen__ = definitionRow[1].ToString(); SetCssTable(CssTable._dot_fieldset_dot_enclosed_thin_space__class_asterisk__equal__yen_field_auto_yen__, definitionRow, CssXls); break;
                    case "_dot_fieldset_dot_enclosed_auto": Css._dot_fieldset_dot_enclosed_auto = definitionRow[1].ToString(); SetCssTable(CssTable._dot_fieldset_dot_enclosed_auto, definitionRow, CssXls); break;
                    case "_dot_fieldset_class_caret__equal__yen_enclosed_yen___space___space_legend": Css._dot_fieldset_class_caret__equal__yen_enclosed_yen___space___space_legend = definitionRow[1].ToString(); SetCssTable(CssTable._dot_fieldset_class_caret__equal__yen_enclosed_yen___space___space_legend, definitionRow, CssXls); break;
                    case "_dot_command_field": Css._dot_command_field = definitionRow[1].ToString(); SetCssTable(CssTable._dot_command_field, definitionRow, CssXls); break;
                    case "_dot_command_field_space___space_button": Css._dot_command_field_space___space_button = definitionRow[1].ToString(); SetCssTable(CssTable._dot_command_field_space___space_button, definitionRow, CssXls); break;
                    case "_dot_command_center": Css._dot_command_center = definitionRow[1].ToString(); SetCssTable(CssTable._dot_command_center, definitionRow, CssXls); break;
                    case "_dot_command_center_space___space_button": Css._dot_command_center_space___space_button = definitionRow[1].ToString(); SetCssTable(CssTable._dot_command_center_space___space_button, definitionRow, CssXls); break;
                    case "_dot_command_left": Css._dot_command_left = definitionRow[1].ToString(); SetCssTable(CssTable._dot_command_left, definitionRow, CssXls); break;
                    case "_dot_command_left_space___space__asterisk_": Css._dot_command_left_space___space__asterisk_ = definitionRow[1].ToString(); SetCssTable(CssTable._dot_command_left_space___space__asterisk_, definitionRow, CssXls); break;
                    case "_dot_command_left_space___space_button": Css._dot_command_left_space___space_button = definitionRow[1].ToString(); SetCssTable(CssTable._dot_command_left_space___space_button, definitionRow, CssXls); break;
                    case "_dot_command_left_space___space__dot_ui_icon": Css._dot_command_left_space___space__dot_ui_icon = definitionRow[1].ToString(); SetCssTable(CssTable._dot_command_left_space___space__dot_ui_icon, definitionRow, CssXls); break;
                    case "_dot_command_right": Css._dot_command_right = definitionRow[1].ToString(); SetCssTable(CssTable._dot_command_right, definitionRow, CssXls); break;
                    case "_dot_command_right_space___space_button": Css._dot_command_right_space___space_button = definitionRow[1].ToString(); SetCssTable(CssTable._dot_command_right_space___space_button, definitionRow, CssXls); break;
                    case "_dot_field_normal": Css._dot_field_normal = definitionRow[1].ToString(); SetCssTable(CssTable._dot_field_normal, definitionRow, CssXls); break;
                    case "_dot_field_normal_space___space__dot_field_label": Css._dot_field_normal_space___space__dot_field_label = definitionRow[1].ToString(); SetCssTable(CssTable._dot_field_normal_space___space__dot_field_label, definitionRow, CssXls); break;
                    case "_dot_field_normal_space___space__dot_field_control": Css._dot_field_normal_space___space__dot_field_control = definitionRow[1].ToString(); SetCssTable(CssTable._dot_field_normal_space___space__dot_field_control, definitionRow, CssXls); break;
                    case "_dot_field_normal_space__dot_container_normal": Css._dot_field_normal_space__dot_container_normal = definitionRow[1].ToString(); SetCssTable(CssTable._dot_field_normal_space__dot_container_normal, definitionRow, CssXls); break;
                    case "_dot_field_normal_space___space__dot_buttons": Css._dot_field_normal_space___space__dot_buttons = definitionRow[1].ToString(); SetCssTable(CssTable._dot_field_normal_space___space__dot_buttons, definitionRow, CssXls); break;
                    case "_dot_field_normal_space__dot_control_text": Css._dot_field_normal_space__dot_control_text = definitionRow[1].ToString(); SetCssTable(CssTable._dot_field_normal_space__dot_control_text, definitionRow, CssXls); break;
                    case "_dot_field_wide": Css._dot_field_wide = definitionRow[1].ToString(); SetCssTable(CssTable._dot_field_wide, definitionRow, CssXls); break;
                    case "_dot_field_wide_space___space__dot_field_label": Css._dot_field_wide_space___space__dot_field_label = definitionRow[1].ToString(); SetCssTable(CssTable._dot_field_wide_space___space__dot_field_label, definitionRow, CssXls); break;
                    case "_dot_field_wide_space___space__dot_field_control": Css._dot_field_wide_space___space__dot_field_control = definitionRow[1].ToString(); SetCssTable(CssTable._dot_field_wide_space___space__dot_field_control, definitionRow, CssXls); break;
                    case "_dot_field_wide_space__dot_container_normal": Css._dot_field_wide_space__dot_container_normal = definitionRow[1].ToString(); SetCssTable(CssTable._dot_field_wide_space__dot_container_normal, definitionRow, CssXls); break;
                    case "_dot_field_auto": Css._dot_field_auto = definitionRow[1].ToString(); SetCssTable(CssTable._dot_field_auto, definitionRow, CssXls); break;
                    case "_dot_field_auto_space___space__dot_field_label": Css._dot_field_auto_space___space__dot_field_label = definitionRow[1].ToString(); SetCssTable(CssTable._dot_field_auto_space___space__dot_field_label, definitionRow, CssXls); break;
                    case "_dot_field_auto_space___space__dot_field_label_space___space_label": Css._dot_field_auto_space___space__dot_field_label_space___space_label = definitionRow[1].ToString(); SetCssTable(CssTable._dot_field_auto_space___space__dot_field_label_space___space_label, definitionRow, CssXls); break;
                    case "_dot_field_auto_space___space__dot_field_control": Css._dot_field_auto_space___space__dot_field_control = definitionRow[1].ToString(); SetCssTable(CssTable._dot_field_auto_space___space__dot_field_control, definitionRow, CssXls); break;
                    case "_dot_field_auto_thin": Css._dot_field_auto_thin = definitionRow[1].ToString(); SetCssTable(CssTable._dot_field_auto_thin, definitionRow, CssXls); break;
                    case "_dot_field_auto_thin_space___space__dot_field_label": Css._dot_field_auto_thin_space___space__dot_field_label = definitionRow[1].ToString(); SetCssTable(CssTable._dot_field_auto_thin_space___space__dot_field_label, definitionRow, CssXls); break;
                    case "_dot_field_auto_thin_space___space__dot_field_label_space___space_label": Css._dot_field_auto_thin_space___space__dot_field_label_space___space_label = definitionRow[1].ToString(); SetCssTable(CssTable._dot_field_auto_thin_space___space__dot_field_label_space___space_label, definitionRow, CssXls); break;
                    case "_dot_field_auto_thin_space___space__dot_field_control": Css._dot_field_auto_thin_space___space__dot_field_control = definitionRow[1].ToString(); SetCssTable(CssTable._dot_field_auto_thin_space___space__dot_field_control, definitionRow, CssXls); break;
                    case "_dot_field_auto_thin_space_select": Css._dot_field_auto_thin_space_select = definitionRow[1].ToString(); SetCssTable(CssTable._dot_field_auto_thin_space_select, definitionRow, CssXls); break;
                    case "_dot_field_vertical": Css._dot_field_vertical = definitionRow[1].ToString(); SetCssTable(CssTable._dot_field_vertical, definitionRow, CssXls); break;
                    case "_dot_field_vertical_space___space__dot_field_label": Css._dot_field_vertical_space___space__dot_field_label = definitionRow[1].ToString(); SetCssTable(CssTable._dot_field_vertical_space___space__dot_field_label, definitionRow, CssXls); break;
                    case "_dot_field_vertical_space___space__dot_field_control": Css._dot_field_vertical_space___space__dot_field_control = definitionRow[1].ToString(); SetCssTable(CssTable._dot_field_vertical_space___space__dot_field_control, definitionRow, CssXls); break;
                    case "_dot_field_label": Css._dot_field_label = definitionRow[1].ToString(); SetCssTable(CssTable._dot_field_label, definitionRow, CssXls); break;
                    case "_dot_field_control_space__dot_unit": Css._dot_field_control_space__dot_unit = definitionRow[1].ToString(); SetCssTable(CssTable._dot_field_control_space__dot_unit, definitionRow, CssXls); break;
                    case "_dot_container_left": Css._dot_container_left = definitionRow[1].ToString(); SetCssTable(CssTable._dot_container_left, definitionRow, CssXls); break;
                    case "_dot_container_right": Css._dot_container_right = definitionRow[1].ToString(); SetCssTable(CssTable._dot_container_right, definitionRow, CssXls); break;
                    case "_dot_container_right_space___space__asterisk_": Css._dot_container_right_space___space__asterisk_ = definitionRow[1].ToString(); SetCssTable(CssTable._dot_container_right_space___space__asterisk_, definitionRow, CssXls); break;
                    case "_dot_control_text": Css._dot_control_text = definitionRow[1].ToString(); SetCssTable(CssTable._dot_control_text, definitionRow, CssXls); break;
                    case "_dot_control_textbox": Css._dot_control_textbox = definitionRow[1].ToString(); SetCssTable(CssTable._dot_control_textbox, definitionRow, CssXls); break;
                    case "_dot_control_textbox_dot_with_unit": Css._dot_control_textbox_dot_with_unit = definitionRow[1].ToString(); SetCssTable(CssTable._dot_control_textbox_dot_with_unit, definitionRow, CssXls); break;
                    case "_dot_control_textarea": Css._dot_control_textarea = definitionRow[1].ToString(); SetCssTable(CssTable._dot_control_textarea, definitionRow, CssXls); break;
                    case "_dot_control_markup": Css._dot_control_markup = definitionRow[1].ToString(); SetCssTable(CssTable._dot_control_markup, definitionRow, CssXls); break;
                    case "_dot_md": Css._dot_md = definitionRow[1].ToString(); SetCssTable(CssTable._dot_md, definitionRow, CssXls); break;
                    case "_dot_md_space_h1": Css._dot_md_space_h1 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_md_space_h1, definitionRow, CssXls); break;
                    case "_dot_md_space_h2": Css._dot_md_space_h2 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_md_space_h2, definitionRow, CssXls); break;
                    case "_dot_md_space_h3": Css._dot_md_space_h3 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_md_space_h3, definitionRow, CssXls); break;
                    case "_dot_md_space_h4": Css._dot_md_space_h4 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_md_space_h4, definitionRow, CssXls); break;
                    case "_dot_md_space_h5": Css._dot_md_space_h5 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_md_space_h5, definitionRow, CssXls); break;
                    case "_dot_md_space_h6": Css._dot_md_space_h6 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_md_space_h6, definitionRow, CssXls); break;
                    case "_dot_md_space_ol": Css._dot_md_space_ol = definitionRow[1].ToString(); SetCssTable(CssTable._dot_md_space_ol, definitionRow, CssXls); break;
                    case "_dot_md_space_p": Css._dot_md_space_p = definitionRow[1].ToString(); SetCssTable(CssTable._dot_md_space_p, definitionRow, CssXls); break;
                    case "_dot_md_space_table": Css._dot_md_space_table = definitionRow[1].ToString(); SetCssTable(CssTable._dot_md_space_table, definitionRow, CssXls); break;
                    case "_dot_md_space_td": Css._dot_md_space_td = definitionRow[1].ToString(); SetCssTable(CssTable._dot_md_space_td, definitionRow, CssXls); break;
                    case "_dot_md_space_th": Css._dot_md_space_th = definitionRow[1].ToString(); SetCssTable(CssTable._dot_md_space_th, definitionRow, CssXls); break;
                    case "_dot_md_space_tbody_space_tr_colon_nth_child_odd_": Css._dot_md_space_tbody_space_tr_colon_nth_child_odd_ = definitionRow[1].ToString(); SetCssTable(CssTable._dot_md_space_tbody_space_tr_colon_nth_child_odd_, definitionRow, CssXls); break;
                    case "_dot_md_space_ul": Css._dot_md_space_ul = definitionRow[1].ToString(); SetCssTable(CssTable._dot_md_space_ul, definitionRow, CssXls); break;
                    case "_dot_control_markdown": Css._dot_control_markdown = definitionRow[1].ToString(); SetCssTable(CssTable._dot_control_markdown, definitionRow, CssXls); break;
                    case "_dot_control_dropdown": Css._dot_control_dropdown = definitionRow[1].ToString(); SetCssTable(CssTable._dot_control_dropdown, definitionRow, CssXls); break;
                    case "_dot_control_spinner": Css._dot_control_spinner = definitionRow[1].ToString(); SetCssTable(CssTable._dot_control_spinner, definitionRow, CssXls); break;
                    case "_dot_control_checkbox": Css._dot_control_checkbox = definitionRow[1].ToString(); SetCssTable(CssTable._dot_control_checkbox, definitionRow, CssXls); break;
                    case "_dot_control_checkbox_space__plus__space_label": Css._dot_control_checkbox_space__plus__space_label = definitionRow[1].ToString(); SetCssTable(CssTable._dot_control_checkbox_space__plus__space_label, definitionRow, CssXls); break;
                    case "_dot_control_radio": Css._dot_control_radio = definitionRow[1].ToString(); SetCssTable(CssTable._dot_control_radio, definitionRow, CssXls); break;
                    case "_dot_control_radio_space__plus__space_label": Css._dot_control_radio_space__plus__space_label = definitionRow[1].ToString(); SetCssTable(CssTable._dot_control_radio_space__plus__space_label, definitionRow, CssXls); break;
                    case "_dot_control_slider": Css._dot_control_slider = definitionRow[1].ToString(); SetCssTable(CssTable._dot_control_slider, definitionRow, CssXls); break;
                    case "_dot_control_slider_ui": Css._dot_control_slider_ui = definitionRow[1].ToString(); SetCssTable(CssTable._dot_control_slider_ui, definitionRow, CssXls); break;
                    case "_dot_control_anchor": Css._dot_control_anchor = definitionRow[1].ToString(); SetCssTable(CssTable._dot_control_anchor, definitionRow, CssXls); break;
                    case "_dot_container_selectable_space__dot_wrapper": Css._dot_container_selectable_space__dot_wrapper = definitionRow[1].ToString(); SetCssTable(CssTable._dot_container_selectable_space__dot_wrapper, definitionRow, CssXls); break;
                    case "_dot_control_selectable": Css._dot_control_selectable = definitionRow[1].ToString(); SetCssTable(CssTable._dot_control_selectable, definitionRow, CssXls); break;
                    case "_dot_control_selectable_space_li": Css._dot_control_selectable_space_li = definitionRow[1].ToString(); SetCssTable(CssTable._dot_control_selectable_space_li, definitionRow, CssXls); break;
                    case "_dot_control_selectable_space__dot_ui_selecting": Css._dot_control_selectable_space__dot_ui_selecting = definitionRow[1].ToString(); SetCssTable(CssTable._dot_control_selectable_space__dot_ui_selecting, definitionRow, CssXls); break;
                    case "_dot_control_selectable_space__dot_ui_selected": Css._dot_control_selectable_space__dot_ui_selected = definitionRow[1].ToString(); SetCssTable(CssTable._dot_control_selectable_space__dot_ui_selected, definitionRow, CssXls); break;
                    case "_dot_control_basket": Css._dot_control_basket = definitionRow[1].ToString(); SetCssTable(CssTable._dot_control_basket, definitionRow, CssXls); break;
                    case "_dot_control_basket_space___space_li": Css._dot_control_basket_space___space_li = definitionRow[1].ToString(); SetCssTable(CssTable._dot_control_basket_space___space_li, definitionRow, CssXls); break;
                    case "_dot_control_basket_space___space_li_space___space_span": Css._dot_control_basket_space___space_li_space___space_span = definitionRow[1].ToString(); SetCssTable(CssTable._dot_control_basket_space___space_li_space___space_span, definitionRow, CssXls); break;
                    case "_dot_comment": Css._dot_comment = definitionRow[1].ToString(); SetCssTable(CssTable._dot_comment, definitionRow, CssXls); break;
                    case "_dot_comment_space___space__dot_time": Css._dot_comment_space___space__dot_time = definitionRow[1].ToString(); SetCssTable(CssTable._dot_comment_space___space__dot_time, definitionRow, CssXls); break;
                    case "_dot_comment_space___space__dot_body": Css._dot_comment_space___space__dot_body = definitionRow[1].ToString(); SetCssTable(CssTable._dot_comment_space___space__dot_body, definitionRow, CssXls); break;
                    case "_dot_comment_space___space__dot_button": Css._dot_comment_space___space__dot_button = definitionRow[1].ToString(); SetCssTable(CssTable._dot_comment_space___space__dot_button, definitionRow, CssXls); break;
                    case "_dot_user": Css._dot_user = definitionRow[1].ToString(); SetCssTable(CssTable._dot_user, definitionRow, CssXls); break;
                    case "_dot_user_space___space_span": Css._dot_user_space___space_span = definitionRow[1].ToString(); SetCssTable(CssTable._dot_user_space___space_span, definitionRow, CssXls); break;
                    case "_dot_dept": Css._dot_dept = definitionRow[1].ToString(); SetCssTable(CssTable._dot_dept, definitionRow, CssXls); break;
                    case "_dot_dept_space___space_span": Css._dot_dept_space___space_span = definitionRow[1].ToString(); SetCssTable(CssTable._dot_dept_space___space_span, definitionRow, CssXls); break;
                    case "_dot_both": Css._dot_both = definitionRow[1].ToString(); SetCssTable(CssTable._dot_both, definitionRow, CssXls); break;
                    case "_dot_hidden": Css._dot_hidden = definitionRow[1].ToString(); SetCssTable(CssTable._dot_hidden, definitionRow, CssXls); break;
                    case "_dot_right": Css._dot_right = definitionRow[1].ToString(); SetCssTable(CssTable._dot_right, definitionRow, CssXls); break;
                    case "_dot_tooltip": Css._dot_tooltip = definitionRow[1].ToString(); SetCssTable(CssTable._dot_tooltip, definitionRow, CssXls); break;
                    case "_dot_no_border": Css._dot_no_border = definitionRow[1].ToString(); SetCssTable(CssTable._dot_no_border, definitionRow, CssXls); break;
                    case "_dot_grid": Css._dot_grid = definitionRow[1].ToString(); SetCssTable(CssTable._dot_grid, definitionRow, CssXls); break;
                    case "_dot_grid_dot_fixed": Css._dot_grid_dot_fixed = definitionRow[1].ToString(); SetCssTable(CssTable._dot_grid_dot_fixed, definitionRow, CssXls); break;
                    case "_dot_grid_space___space_thead_space___space_tr_space___space_caption": Css._dot_grid_space___space_thead_space___space_tr_space___space_caption = definitionRow[1].ToString(); SetCssTable(CssTable._dot_grid_space___space_thead_space___space_tr_space___space_caption, definitionRow, CssXls); break;
                    case "_dot_grid_space___space_thead_space___space_tr_space___space_th": Css._dot_grid_space___space_thead_space___space_tr_space___space_th = definitionRow[1].ToString(); SetCssTable(CssTable._dot_grid_space___space_thead_space___space_tr_space___space_th, definitionRow, CssXls); break;
                    case "_dot_grid_space___space_thead_space___space_tr_space___space_th_space___space_div": Css._dot_grid_space___space_thead_space___space_tr_space___space_th_space___space_div = definitionRow[1].ToString(); SetCssTable(CssTable._dot_grid_space___space_thead_space___space_tr_space___space_th_space___space_div, definitionRow, CssXls); break;
                    case "_dot_grid_space___space_thead_space___space_tr_space___space_th_space_span": Css._dot_grid_space___space_thead_space___space_tr_space___space_th_space_span = definitionRow[1].ToString(); SetCssTable(CssTable._dot_grid_space___space_thead_space___space_tr_space___space_th_space_span, definitionRow, CssXls); break;
                    case "_dot_grid_space___space_thead_space___space_tr_colon_first_child_space___space_th_colon_first_child": Css._dot_grid_space___space_thead_space___space_tr_colon_first_child_space___space_th_colon_first_child = definitionRow[1].ToString(); SetCssTable(CssTable._dot_grid_space___space_thead_space___space_tr_colon_first_child_space___space_th_colon_first_child, definitionRow, CssXls); break;
                    case "_dot_grid_space___space_thead_space___space_tr_colon_first_child_space___space_th_colon_last_child": Css._dot_grid_space___space_thead_space___space_tr_colon_first_child_space___space_th_colon_last_child = definitionRow[1].ToString(); SetCssTable(CssTable._dot_grid_space___space_thead_space___space_tr_colon_first_child_space___space_th_colon_last_child, definitionRow, CssXls); break;
                    case "_dot_grid_space___space_thead_space___space_tr_space___space_th_dot_sortable_colon_hover": Css._dot_grid_space___space_thead_space___space_tr_space___space_th_dot_sortable_colon_hover = definitionRow[1].ToString(); SetCssTable(CssTable._dot_grid_space___space_thead_space___space_tr_space___space_th_dot_sortable_colon_hover, definitionRow, CssXls); break;
                    case "_dot_grid_space___space_tbody_space___space_tr_space___space_td": Css._dot_grid_space___space_tbody_space___space_tr_space___space_td = definitionRow[1].ToString(); SetCssTable(CssTable._dot_grid_space___space_tbody_space___space_tr_space___space_td, definitionRow, CssXls); break;
                    case "_dot_grid_space___space_tbody_space___space_tr_space__class_asterisk__equal__yen_status__yen__": Css._dot_grid_space___space_tbody_space___space_tr_space__class_asterisk__equal__yen_status__yen__ = definitionRow[1].ToString(); SetCssTable(CssTable._dot_grid_space___space_tbody_space___space_tr_space__class_asterisk__equal__yen_status__yen__, definitionRow, CssXls); break;
                    case "_dot_grid_space___space_tbody_space___space_tr_space___space_th": Css._dot_grid_space___space_tbody_space___space_tr_space___space_th = definitionRow[1].ToString(); SetCssTable(CssTable._dot_grid_space___space_tbody_space___space_tr_space___space_th, definitionRow, CssXls); break;
                    case "_dot_grid_row": Css._dot_grid_row = definitionRow[1].ToString(); SetCssTable(CssTable._dot_grid_row, definitionRow, CssXls); break;
                    case "_dot_grid_row_space__dot_comment": Css._dot_grid_row_space__dot_comment = definitionRow[1].ToString(); SetCssTable(CssTable._dot_grid_row_space__dot_comment, definitionRow, CssXls); break;
                    case "_dot_grid_row_colon_hover": Css._dot_grid_row_colon_hover = definitionRow[1].ToString(); SetCssTable(CssTable._dot_grid_row_colon_hover, definitionRow, CssXls); break;
                    case "_dot_grid_row_colon_hover_space__dot_comment": Css._dot_grid_row_colon_hover_space__dot_comment = definitionRow[1].ToString(); SetCssTable(CssTable._dot_grid_row_colon_hover_space__dot_comment, definitionRow, CssXls); break;
                    case "_dot_grid_row_colon_hover_space__dot_grid_title_body": Css._dot_grid_row_colon_hover_space__dot_grid_title_body = definitionRow[1].ToString(); SetCssTable(CssTable._dot_grid_row_colon_hover_space__dot_grid_title_body, definitionRow, CssXls); break;
                    case "_dot_grid_row_space_p": Css._dot_grid_row_space_p = definitionRow[1].ToString(); SetCssTable(CssTable._dot_grid_row_space_p, definitionRow, CssXls); break;
                    case "_dot_grid_row_space_p_dot_body": Css._dot_grid_row_space_p_dot_body = definitionRow[1].ToString(); SetCssTable(CssTable._dot_grid_row_space_p_dot_body, definitionRow, CssXls); break;
                    case "_dot_grid_title_body": Css._dot_grid_title_body = definitionRow[1].ToString(); SetCssTable(CssTable._dot_grid_title_body, definitionRow, CssXls); break;
                    case "_dot_grid_title_body_space___space__dot_title_space__plus__space__dot_body": Css._dot_grid_title_body_space___space__dot_title_space__plus__space__dot_body = definitionRow[1].ToString(); SetCssTable(CssTable._dot_grid_title_body_space___space__dot_title_space__plus__space__dot_body, definitionRow, CssXls); break;
                    case "_dot_links": Css._dot_links = definitionRow[1].ToString(); SetCssTable(CssTable._dot_links, definitionRow, CssXls); break;
                    case "_dot_link_creations_space_button": Css._dot_link_creations_space_button = definitionRow[1].ToString(); SetCssTable(CssTable._dot_link_creations_space_button, definitionRow, CssXls); break;
                    case "_dot_text": Css._dot_text = definitionRow[1].ToString(); SetCssTable(CssTable._dot_text, definitionRow, CssXls); break;
                    case "_dot_datepicker": Css._dot_datepicker = definitionRow[1].ToString(); SetCssTable(CssTable._dot_datepicker, definitionRow, CssXls); break;
                    case "_dot_dropdown": Css._dot_dropdown = definitionRow[1].ToString(); SetCssTable(CssTable._dot_dropdown, definitionRow, CssXls); break;
                    case "_class_asterisk__equal__yen_limit__yen__": Css._class_asterisk__equal__yen_limit__yen__ = definitionRow[1].ToString(); SetCssTable(CssTable._class_asterisk__equal__yen_limit__yen__, definitionRow, CssXls); break;
                    case "_dot_limit_normal": Css._dot_limit_normal = definitionRow[1].ToString(); SetCssTable(CssTable._dot_limit_normal, definitionRow, CssXls); break;
                    case "_dot_limit_warning1": Css._dot_limit_warning1 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_limit_warning1, definitionRow, CssXls); break;
                    case "_dot_limit_warning2": Css._dot_limit_warning2 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_limit_warning2, definitionRow, CssXls); break;
                    case "_dot_limit_warning3": Css._dot_limit_warning3 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_limit_warning3, definitionRow, CssXls); break;
                    case "_dot_message": Css._dot_message = definitionRow[1].ToString(); SetCssTable(CssTable._dot_message, definitionRow, CssXls); break;
                    case "_dot_message_space___space_span": Css._dot_message_space___space_span = definitionRow[1].ToString(); SetCssTable(CssTable._dot_message_space___space_span, definitionRow, CssXls); break;
                    case "_dot_message_dialog": Css._dot_message_dialog = definitionRow[1].ToString(); SetCssTable(CssTable._dot_message_dialog, definitionRow, CssXls); break;
                    case "_dot_message_form_bottom": Css._dot_message_form_bottom = definitionRow[1].ToString(); SetCssTable(CssTable._dot_message_form_bottom, definitionRow, CssXls); break;
                    case "_dot_alert_error": Css._dot_alert_error = definitionRow[1].ToString(); SetCssTable(CssTable._dot_alert_error, definitionRow, CssXls); break;
                    case "_dot_alert_success": Css._dot_alert_success = definitionRow[1].ToString(); SetCssTable(CssTable._dot_alert_success, definitionRow, CssXls); break;
                    case "_dot_alert_warning": Css._dot_alert_warning = definitionRow[1].ToString(); SetCssTable(CssTable._dot_alert_warning, definitionRow, CssXls); break;
                    case "_dot_alert_information": Css._dot_alert_information = definitionRow[1].ToString(); SetCssTable(CssTable._dot_alert_information, definitionRow, CssXls); break;
                    case "label_dot_error": Css.label_dot_error = definitionRow[1].ToString(); SetCssTable(CssTable.label_dot_error, definitionRow, CssXls); break;
                    case "_dot_error": Css._dot_error = definitionRow[1].ToString(); SetCssTable(CssTable._dot_error, definitionRow, CssXls); break;
                    case "_dot_button_edit_markdown": Css._dot_button_edit_markdown = definitionRow[1].ToString(); SetCssTable(CssTable._dot_button_edit_markdown, definitionRow, CssXls); break;
                    case "_dot_button_delete_address": Css._dot_button_delete_address = definitionRow[1].ToString(); SetCssTable(CssTable._dot_button_delete_address, definitionRow, CssXls); break;
                    case "_dot_button_right_justified": Css._dot_button_right_justified = definitionRow[1].ToString(); SetCssTable(CssTable._dot_button_right_justified, definitionRow, CssXls); break;
                    case "_dot_status_new": Css._dot_status_new = definitionRow[1].ToString(); SetCssTable(CssTable._dot_status_new, definitionRow, CssXls); break;
                    case "_dot_status_preparation": Css._dot_status_preparation = definitionRow[1].ToString(); SetCssTable(CssTable._dot_status_preparation, definitionRow, CssXls); break;
                    case "_dot_status_inprogress": Css._dot_status_inprogress = definitionRow[1].ToString(); SetCssTable(CssTable._dot_status_inprogress, definitionRow, CssXls); break;
                    case "_dot_status_review": Css._dot_status_review = definitionRow[1].ToString(); SetCssTable(CssTable._dot_status_review, definitionRow, CssXls); break;
                    case "_dot_status_closed": Css._dot_status_closed = definitionRow[1].ToString(); SetCssTable(CssTable._dot_status_closed, definitionRow, CssXls); break;
                    case "_dot_status_rejected": Css._dot_status_rejected = definitionRow[1].ToString(); SetCssTable(CssTable._dot_status_rejected, definitionRow, CssXls); break;
                    case "h3_dot_title_header": Css.h3_dot_title_header = definitionRow[1].ToString(); SetCssTable(CssTable.h3_dot_title_header, definitionRow, CssXls); break;
                    case "_dot_outgoing_mail_space__dot_dialog": Css._dot_outgoing_mail_space__dot_dialog = definitionRow[1].ToString(); SetCssTable(CssTable._dot_outgoing_mail_space__dot_dialog, definitionRow, CssXls); break;
                    case "_dot_outgoing_mail_space__dot_ui_dialog_titlebar": Css._dot_outgoing_mail_space__dot_ui_dialog_titlebar = definitionRow[1].ToString(); SetCssTable(CssTable._dot_outgoing_mail_space__dot_ui_dialog_titlebar, definitionRow, CssXls); break;
                    case "_dot_svg_work_value": Css._dot_svg_work_value = definitionRow[1].ToString(); SetCssTable(CssTable._dot_svg_work_value, definitionRow, CssXls); break;
                    case "_dot_svg_work_value_space_text": Css._dot_svg_work_value_space_text = definitionRow[1].ToString(); SetCssTable(CssTable._dot_svg_work_value_space_text, definitionRow, CssXls); break;
                    case "_dot_svg_work_value_space_rect_colon_nth_of_type_1_": Css._dot_svg_work_value_space_rect_colon_nth_of_type_1_ = definitionRow[1].ToString(); SetCssTable(CssTable._dot_svg_work_value_space_rect_colon_nth_of_type_1_, definitionRow, CssXls); break;
                    case "_dot_svg_work_value_space_rect_colon_nth_of_type_2_": Css._dot_svg_work_value_space_rect_colon_nth_of_type_2_ = definitionRow[1].ToString(); SetCssTable(CssTable._dot_svg_work_value_space_rect_colon_nth_of_type_2_, definitionRow, CssXls); break;
                    case "_dot_svg_progress_rate": Css._dot_svg_progress_rate = definitionRow[1].ToString(); SetCssTable(CssTable._dot_svg_progress_rate, definitionRow, CssXls); break;
                    case "_dot_svg_progress_rate_space_text": Css._dot_svg_progress_rate_space_text = definitionRow[1].ToString(); SetCssTable(CssTable._dot_svg_progress_rate_space_text, definitionRow, CssXls); break;
                    case "_dot_svg_progress_rate_dot_warning_space_text": Css._dot_svg_progress_rate_dot_warning_space_text = definitionRow[1].ToString(); SetCssTable(CssTable._dot_svg_progress_rate_dot_warning_space_text, definitionRow, CssXls); break;
                    case "_dot_svg_progress_rate_space_rect_colon_nth_of_type_1_": Css._dot_svg_progress_rate_space_rect_colon_nth_of_type_1_ = definitionRow[1].ToString(); SetCssTable(CssTable._dot_svg_progress_rate_space_rect_colon_nth_of_type_1_, definitionRow, CssXls); break;
                    case "_dot_svg_progress_rate_space_rect_colon_nth_of_type_2_": Css._dot_svg_progress_rate_space_rect_colon_nth_of_type_2_ = definitionRow[1].ToString(); SetCssTable(CssTable._dot_svg_progress_rate_space_rect_colon_nth_of_type_2_, definitionRow, CssXls); break;
                    case "_dot_svg_progress_rate_space_rect_colon_nth_of_type_3_": Css._dot_svg_progress_rate_space_rect_colon_nth_of_type_3_ = definitionRow[1].ToString(); SetCssTable(CssTable._dot_svg_progress_rate_space_rect_colon_nth_of_type_3_, definitionRow, CssXls); break;
                    case "_dot_svg_progress_rate_dot_warning_space_rect_colon_nth_of_type_3_": Css._dot_svg_progress_rate_dot_warning_space_rect_colon_nth_of_type_3_ = definitionRow[1].ToString(); SetCssTable(CssTable._dot_svg_progress_rate_dot_warning_space_rect_colon_nth_of_type_3_, definitionRow, CssXls); break;
                    case "_dot_axis": Css._dot_axis = definitionRow[1].ToString(); SetCssTable(CssTable._dot_axis, definitionRow, CssXls); break;
                    case "_dot_h2": Css._dot_h2 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_h2, definitionRow, CssXls); break;
                    case "_dot_h3": Css._dot_h3 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_h3, definitionRow, CssXls); break;
                    case "_dot_h4": Css._dot_h4 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_h4, definitionRow, CssXls); break;
                    case "_dot_h5": Css._dot_h5 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_h5, definitionRow, CssXls); break;
                    case "_dot_h6": Css._dot_h6 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_h6, definitionRow, CssXls); break;
                    case "_dot_h2_space___space_h2": Css._dot_h2_space___space_h2 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_h2_space___space_h2, definitionRow, CssXls); break;
                    case "_dot_h3_space___space_h3": Css._dot_h3_space___space_h3 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_h3_space___space_h3, definitionRow, CssXls); break;
                    case "_dot_h4_space___space_h4": Css._dot_h4_space___space_h4 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_h4_space___space_h4, definitionRow, CssXls); break;
                    case "_dot_h5_space___space_h5": Css._dot_h5_space___space_h5 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_h5_space___space_h5, definitionRow, CssXls); break;
                    case "_dot_h6_space___space_h6": Css._dot_h6_space___space_h6 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_h6_space___space_h6, definitionRow, CssXls); break;
                    case "_dot_w50": Css._dot_w50 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_w50, definitionRow, CssXls); break;
                    case "_dot_w100": Css._dot_w100 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_w100, definitionRow, CssXls); break;
                    case "_dot_w150": Css._dot_w150 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_w150, definitionRow, CssXls); break;
                    case "_dot_w200": Css._dot_w200 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_w200, definitionRow, CssXls); break;
                    case "_dot_w250": Css._dot_w250 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_w250, definitionRow, CssXls); break;
                    case "_dot_w300": Css._dot_w300 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_w300, definitionRow, CssXls); break;
                    case "_dot_w350": Css._dot_w350 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_w350, definitionRow, CssXls); break;
                    case "_dot_w400": Css._dot_w400 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_w400, definitionRow, CssXls); break;
                    case "_dot_w450": Css._dot_w450 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_w450, definitionRow, CssXls); break;
                    case "_dot_w500": Css._dot_w500 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_w500, definitionRow, CssXls); break;
                    case "_dot_w550": Css._dot_w550 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_w550, definitionRow, CssXls); break;
                    case "_dot_w600": Css._dot_w600 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_w600, definitionRow, CssXls); break;
                    case "_dot_h100": Css._dot_h100 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_h100, definitionRow, CssXls); break;
                    case "_dot_h150": Css._dot_h150 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_h150, definitionRow, CssXls); break;
                    case "_dot_h200": Css._dot_h200 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_h200, definitionRow, CssXls); break;
                    case "_dot_h250": Css._dot_h250 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_h250, definitionRow, CssXls); break;
                    case "_dot_h300": Css._dot_h300 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_h300, definitionRow, CssXls); break;
                    case "_dot_h350": Css._dot_h350 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_h350, definitionRow, CssXls); break;
                    case "_dot_h400": Css._dot_h400 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_h400, definitionRow, CssXls); break;
                    case "_dot_h450": Css._dot_h450 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_h450, definitionRow, CssXls); break;
                    case "_dot_h500": Css._dot_h500 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_h500, definitionRow, CssXls); break;
                    case "_dot_h550": Css._dot_h550 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_h550, definitionRow, CssXls); break;
                    case "_dot_h600": Css._dot_h600 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_h600, definitionRow, CssXls); break;
                    case "_dot_m_l10": Css._dot_m_l10 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_m_l10, definitionRow, CssXls); break;
                    case "_dot_m_l20": Css._dot_m_l20 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_m_l20, definitionRow, CssXls); break;
                    case "_dot_m_l30": Css._dot_m_l30 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_m_l30, definitionRow, CssXls); break;
                    case "_dot_m_l40": Css._dot_m_l40 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_m_l40, definitionRow, CssXls); break;
                    case "_dot_m_l50": Css._dot_m_l50 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_m_l50, definitionRow, CssXls); break;
                    case "_dot_paragraph": Css._dot_paragraph = definitionRow[1].ToString(); SetCssTable(CssTable._dot_paragraph, definitionRow, CssXls); break;
                    case "_dot_dialog": Css._dot_dialog = definitionRow[1].ToString(); SetCssTable(CssTable._dot_dialog, definitionRow, CssXls); break;
                    case "_dot_dialog_space__dot_fieldset": Css._dot_dialog_space__dot_fieldset = definitionRow[1].ToString(); SetCssTable(CssTable._dot_dialog_space__dot_fieldset, definitionRow, CssXls); break;
                    case "_dot_link": Css._dot_link = definitionRow[1].ToString(); SetCssTable(CssTable._dot_link, definitionRow, CssXls); break;
                    case "_dot_histories_form": Css._dot_histories_form = definitionRow[1].ToString(); SetCssTable(CssTable._dot_histories_form, definitionRow, CssXls); break;
                    case "_dot_ui_widget_space_input_comma__space__dot_ui_widget_space_select_comma__space__dot_ui_widget_space_button": Css._dot_ui_widget_space_input_comma__space__dot_ui_widget_space_select_comma__space__dot_ui_widget_space_button = definitionRow[1].ToString(); SetCssTable(CssTable._dot_ui_widget_space_input_comma__space__dot_ui_widget_space_select_comma__space__dot_ui_widget_space_button, definitionRow, CssXls); break;
                    case "_dot_ui_widget_space_textarea": Css._dot_ui_widget_space_textarea = definitionRow[1].ToString(); SetCssTable(CssTable._dot_ui_widget_space_textarea, definitionRow, CssXls); break;
                    case "_dot_ui_widget": Css._dot_ui_widget = definitionRow[1].ToString(); SetCssTable(CssTable._dot_ui_widget, definitionRow, CssXls); break;
                    case "_dot_ui_button": Css._dot_ui_button = definitionRow[1].ToString(); SetCssTable(CssTable._dot_ui_button, definitionRow, CssXls); break;
                    case "_dot_ui_dialog": Css._dot_ui_dialog = definitionRow[1].ToString(); SetCssTable(CssTable._dot_ui_dialog, definitionRow, CssXls); break;
                    case "_dot_ui_icon_dot_a": Css._dot_ui_icon_dot_a = definitionRow[1].ToString(); SetCssTable(CssTable._dot_ui_icon_dot_a, definitionRow, CssXls); break;
                    case "_dot_ui_spinner": Css._dot_ui_spinner = definitionRow[1].ToString(); SetCssTable(CssTable._dot_ui_spinner, definitionRow, CssXls); break;
                    case "_dot_ui_multiselect": Css._dot_ui_multiselect = definitionRow[1].ToString(); SetCssTable(CssTable._dot_ui_multiselect, definitionRow, CssXls); break;
                    case "_dot_ui_multiselect_checkboxes": Css._dot_ui_multiselect_checkboxes = definitionRow[1].ToString(); SetCssTable(CssTable._dot_ui_multiselect_checkboxes, definitionRow, CssXls); break;
                    case "_dot_ui_multiselect_checkboxes_space_input": Css._dot_ui_multiselect_checkboxes_space_input = definitionRow[1].ToString(); SetCssTable(CssTable._dot_ui_multiselect_checkboxes_space_input, definitionRow, CssXls); break;
                    case "_dot_ui_corner_all_dot_ui_state_hover": Css._dot_ui_corner_all_dot_ui_state_hover = definitionRow[1].ToString(); SetCssTable(CssTable._dot_ui_corner_all_dot_ui_state_hover, definitionRow, CssXls); break;
                    case "_dot_height_auto": Css._dot_height_auto = definitionRow[1].ToString(); SetCssTable(CssTable._dot_height_auto, definitionRow, CssXls); break;
                    case "_dot_focus_inform": Css._dot_focus_inform = definitionRow[1].ToString(); SetCssTable(CssTable._dot_focus_inform, definitionRow, CssXls); break;
                    case "_dot_sortable": Css._dot_sortable = definitionRow[1].ToString(); SetCssTable(CssTable._dot_sortable, definitionRow, CssXls); break;
                    case "_dot_menu_sort": Css._dot_menu_sort = definitionRow[1].ToString(); SetCssTable(CssTable._dot_menu_sort, definitionRow, CssXls); break;
                    case "_dot_menu_sort_space___space_li": Css._dot_menu_sort_space___space_li = definitionRow[1].ToString(); SetCssTable(CssTable._dot_menu_sort_space___space_li, definitionRow, CssXls); break;
                    case "_dot_menu_sort_space___space_li_colon_hover": Css._dot_menu_sort_space___space_li_colon_hover = definitionRow[1].ToString(); SetCssTable(CssTable._dot_menu_sort_space___space_li_colon_hover, definitionRow, CssXls); break;
                    case "_dot_menu_sort_space___space_li_space___space__asterisk_": Css._dot_menu_sort_space___space_li_space___space__asterisk_ = definitionRow[1].ToString(); SetCssTable(CssTable._dot_menu_sort_space___space_li_space___space__asterisk_, definitionRow, CssXls); break;
                    default: break;
                }
            });
            CssXls.XlsSheet.AsEnumerable().Skip(1).Where(o => o[0].ToString() != string.Empty).ForEach(definitionRow =>
            {
                var newCssDefinition = new CssDefinition();
                if (definitionRow.ContainsKey("Id")) { newCssDefinition.Id = definitionRow["Id"].ToString(); newCssDefinition.SavedId = newCssDefinition.Id; }
                if (definitionRow.ContainsKey("Specific")) { newCssDefinition.Specific = definitionRow["Specific"].ToString(); newCssDefinition.SavedSpecific = newCssDefinition.Specific; }
                if (definitionRow.ContainsKey("width")) { newCssDefinition.width = definitionRow["width"].ToString(); newCssDefinition.Savedwidth = newCssDefinition.width; }
                if (definitionRow.ContainsKey("min-width")) { newCssDefinition.min_width = definitionRow["min-width"].ToString(); newCssDefinition.Savedmin_width = newCssDefinition.min_width; }
                if (definitionRow.ContainsKey("max-width")) { newCssDefinition.max_width = definitionRow["max-width"].ToString(); newCssDefinition.Savedmax_width = newCssDefinition.max_width; }
                if (definitionRow.ContainsKey("height")) { newCssDefinition.height = definitionRow["height"].ToString(); newCssDefinition.Savedheight = newCssDefinition.height; }
                if (definitionRow.ContainsKey("min-height")) { newCssDefinition.min_height = definitionRow["min-height"].ToString(); newCssDefinition.Savedmin_height = newCssDefinition.min_height; }
                if (definitionRow.ContainsKey("max-height")) { newCssDefinition.max_height = definitionRow["max-height"].ToString(); newCssDefinition.Savedmax_height = newCssDefinition.max_height; }
                if (definitionRow.ContainsKey("display")) { newCssDefinition.display = definitionRow["display"].ToString(); newCssDefinition.Saveddisplay = newCssDefinition.display; }
                if (definitionRow.ContainsKey("float")) { newCssDefinition._float = definitionRow["float"].ToString(); newCssDefinition.Saved_float = newCssDefinition._float; }
                if (definitionRow.ContainsKey("margin")) { newCssDefinition.margin = definitionRow["margin"].ToString(); newCssDefinition.Savedmargin = newCssDefinition.margin; }
                if (definitionRow.ContainsKey("margin-left")) { newCssDefinition.margin_left = definitionRow["margin-left"].ToString(); newCssDefinition.Savedmargin_left = newCssDefinition.margin_left; }
                if (definitionRow.ContainsKey("margin-right")) { newCssDefinition.margin_right = definitionRow["margin-right"].ToString(); newCssDefinition.Savedmargin_right = newCssDefinition.margin_right; }
                if (definitionRow.ContainsKey("padding")) { newCssDefinition.padding = definitionRow["padding"].ToString(); newCssDefinition.Savedpadding = newCssDefinition.padding; }
                if (definitionRow.ContainsKey("padding-bottom")) { newCssDefinition.padding_bottom = definitionRow["padding-bottom"].ToString(); newCssDefinition.Savedpadding_bottom = newCssDefinition.padding_bottom; }
                if (definitionRow.ContainsKey("text-align")) { newCssDefinition.text_align = definitionRow["text-align"].ToString(); newCssDefinition.Savedtext_align = newCssDefinition.text_align; }
                if (definitionRow.ContainsKey("vertical-align")) { newCssDefinition.vertical_align = definitionRow["vertical-align"].ToString(); newCssDefinition.Savedvertical_align = newCssDefinition.vertical_align; }
                if (definitionRow.ContainsKey("line-height")) { newCssDefinition.line_height = definitionRow["line-height"].ToString(); newCssDefinition.Savedline_height = newCssDefinition.line_height; }
                if (definitionRow.ContainsKey("font-size")) { newCssDefinition.font_size = definitionRow["font-size"].ToString(); newCssDefinition.Savedfont_size = newCssDefinition.font_size; }
                if (definitionRow.ContainsKey("font-family")) { newCssDefinition.font_family = definitionRow["font-family"].ToString(); newCssDefinition.Savedfont_family = newCssDefinition.font_family; }
                if (definitionRow.ContainsKey("font-weight")) { newCssDefinition.font_weight = definitionRow["font-weight"].ToString(); newCssDefinition.Savedfont_weight = newCssDefinition.font_weight; }
                if (definitionRow.ContainsKey("font-style")) { newCssDefinition.font_style = definitionRow["font-style"].ToString(); newCssDefinition.Savedfont_style = newCssDefinition.font_style; }
                if (definitionRow.ContainsKey("color")) { newCssDefinition.color = definitionRow["color"].ToString(); newCssDefinition.Savedcolor = newCssDefinition.color; }
                if (definitionRow.ContainsKey("background")) { newCssDefinition.background = definitionRow["background"].ToString(); newCssDefinition.Savedbackground = newCssDefinition.background; }
                if (definitionRow.ContainsKey("background-color")) { newCssDefinition.background_color = definitionRow["background-color"].ToString(); newCssDefinition.Savedbackground_color = newCssDefinition.background_color; }
                if (definitionRow.ContainsKey("border")) { newCssDefinition.border = definitionRow["border"].ToString(); newCssDefinition.Savedborder = newCssDefinition.border; }
                if (definitionRow.ContainsKey("border-top")) { newCssDefinition.border_top = definitionRow["border-top"].ToString(); newCssDefinition.Savedborder_top = newCssDefinition.border_top; }
                if (definitionRow.ContainsKey("border-bottom")) { newCssDefinition.border_bottom = definitionRow["border-bottom"].ToString(); newCssDefinition.Savedborder_bottom = newCssDefinition.border_bottom; }
                if (definitionRow.ContainsKey("border-left")) { newCssDefinition.border_left = definitionRow["border-left"].ToString(); newCssDefinition.Savedborder_left = newCssDefinition.border_left; }
                if (definitionRow.ContainsKey("border-right")) { newCssDefinition.border_right = definitionRow["border-right"].ToString(); newCssDefinition.Savedborder_right = newCssDefinition.border_right; }
                if (definitionRow.ContainsKey("border-collapse")) { newCssDefinition.border_collapse = definitionRow["border-collapse"].ToString(); newCssDefinition.Savedborder_collapse = newCssDefinition.border_collapse; }
                if (definitionRow.ContainsKey("border-spacing")) { newCssDefinition.border_spacing = definitionRow["border-spacing"].ToString(); newCssDefinition.Savedborder_spacing = newCssDefinition.border_spacing; }
                if (definitionRow.ContainsKey("position")) { newCssDefinition.position = definitionRow["position"].ToString(); newCssDefinition.Savedposition = newCssDefinition.position; }
                if (definitionRow.ContainsKey("top")) { newCssDefinition.top = definitionRow["top"].ToString(); newCssDefinition.Savedtop = newCssDefinition.top; }
                if (definitionRow.ContainsKey("right")) { newCssDefinition.right = definitionRow["right"].ToString(); newCssDefinition.Savedright = newCssDefinition.right; }
                if (definitionRow.ContainsKey("left")) { newCssDefinition.left = definitionRow["left"].ToString(); newCssDefinition.Savedleft = newCssDefinition.left; }
                if (definitionRow.ContainsKey("bottom")) { newCssDefinition.bottom = definitionRow["bottom"].ToString(); newCssDefinition.Savedbottom = newCssDefinition.bottom; }
                if (definitionRow.ContainsKey("cursor")) { newCssDefinition.cursor = definitionRow["cursor"].ToString(); newCssDefinition.Savedcursor = newCssDefinition.cursor; }
                if (definitionRow.ContainsKey("clear")) { newCssDefinition.clear = definitionRow["clear"].ToString(); newCssDefinition.Savedclear = newCssDefinition.clear; }
                if (definitionRow.ContainsKey("overflow")) { newCssDefinition.overflow = definitionRow["overflow"].ToString(); newCssDefinition.Savedoverflow = newCssDefinition.overflow; }
                if (definitionRow.ContainsKey("word-wrap")) { newCssDefinition.word_wrap = definitionRow["word-wrap"].ToString(); newCssDefinition.Savedword_wrap = newCssDefinition.word_wrap; }
                if (definitionRow.ContainsKey("word-break")) { newCssDefinition.word_break = definitionRow["word-break"].ToString(); newCssDefinition.Savedword_break = newCssDefinition.word_break; }
                if (definitionRow.ContainsKey("white-space")) { newCssDefinition.white_space = definitionRow["white-space"].ToString(); newCssDefinition.Savedwhite_space = newCssDefinition.white_space; }
                if (definitionRow.ContainsKey("table-layout")) { newCssDefinition.table_layout = definitionRow["table-layout"].ToString(); newCssDefinition.Savedtable_layout = newCssDefinition.table_layout; }
                if (definitionRow.ContainsKey("text-decoration")) { newCssDefinition.text_decoration = definitionRow["text-decoration"].ToString(); newCssDefinition.Savedtext_decoration = newCssDefinition.text_decoration; }
                if (definitionRow.ContainsKey("list-style-type")) { newCssDefinition.list_style_type = definitionRow["list-style-type"].ToString(); newCssDefinition.Savedlist_style_type = newCssDefinition.list_style_type; }
                if (definitionRow.ContainsKey("visibility")) { newCssDefinition.visibility = definitionRow["visibility"].ToString(); newCssDefinition.Savedvisibility = newCssDefinition.visibility; }
                if (definitionRow.ContainsKey("border-radius")) { newCssDefinition.border_radius = definitionRow["border-radius"].ToString(); newCssDefinition.Savedborder_radius = newCssDefinition.border_radius; }
                if (definitionRow.ContainsKey("z-index")) { newCssDefinition.z_index = definitionRow["z-index"].ToString(); newCssDefinition.Savedz_index = newCssDefinition.z_index; }
                if (definitionRow.ContainsKey("fill")) { newCssDefinition.fill = definitionRow["fill"].ToString(); newCssDefinition.Savedfill = newCssDefinition.fill; }
                if (definitionRow.ContainsKey("fill-opacity")) { newCssDefinition.fill_opacity = definitionRow["fill-opacity"].ToString(); newCssDefinition.Savedfill_opacity = newCssDefinition.fill_opacity; }
                if (definitionRow.ContainsKey("stroke")) { newCssDefinition.stroke = definitionRow["stroke"].ToString(); newCssDefinition.Savedstroke = newCssDefinition.stroke; }
                if (definitionRow.ContainsKey("stroke-width")) { newCssDefinition.stroke_width = definitionRow["stroke-width"].ToString(); newCssDefinition.Savedstroke_width = newCssDefinition.stroke_width; }
                if (definitionRow.ContainsKey("text-anchor")) { newCssDefinition.text_anchor = definitionRow["text-anchor"].ToString(); newCssDefinition.Savedtext_anchor = newCssDefinition.text_anchor; }
                if (definitionRow.ContainsKey("shape-rendering")) { newCssDefinition.shape_rendering = definitionRow["shape-rendering"].ToString(); newCssDefinition.Savedshape_rendering = newCssDefinition.shape_rendering; }
                CssDefinitionCollection.Add(newCssDefinition);
            });
        }

        private static void SetCssTable(CssDefinition definition, XlsRow definitionRow, XlsIo cssxls)
        {
            if (definitionRow.ContainsKey("Id")) { definition.Id = definitionRow["Id"].ToString(); definition.SavedId = definition.Id; }
            if (definitionRow.ContainsKey("Specific")) { definition.Specific = definitionRow["Specific"].ToString(); definition.SavedSpecific = definition.Specific; }
            if (definitionRow.ContainsKey("width")) { definition.width = definitionRow["width"].ToString(); definition.Savedwidth = definition.width; }
            if (definitionRow.ContainsKey("min-width")) { definition.min_width = definitionRow["min-width"].ToString(); definition.Savedmin_width = definition.min_width; }
            if (definitionRow.ContainsKey("max-width")) { definition.max_width = definitionRow["max-width"].ToString(); definition.Savedmax_width = definition.max_width; }
            if (definitionRow.ContainsKey("height")) { definition.height = definitionRow["height"].ToString(); definition.Savedheight = definition.height; }
            if (definitionRow.ContainsKey("min-height")) { definition.min_height = definitionRow["min-height"].ToString(); definition.Savedmin_height = definition.min_height; }
            if (definitionRow.ContainsKey("max-height")) { definition.max_height = definitionRow["max-height"].ToString(); definition.Savedmax_height = definition.max_height; }
            if (definitionRow.ContainsKey("display")) { definition.display = definitionRow["display"].ToString(); definition.Saveddisplay = definition.display; }
            if (definitionRow.ContainsKey("float")) { definition._float = definitionRow["float"].ToString(); definition.Saved_float = definition._float; }
            if (definitionRow.ContainsKey("margin")) { definition.margin = definitionRow["margin"].ToString(); definition.Savedmargin = definition.margin; }
            if (definitionRow.ContainsKey("margin-left")) { definition.margin_left = definitionRow["margin-left"].ToString(); definition.Savedmargin_left = definition.margin_left; }
            if (definitionRow.ContainsKey("margin-right")) { definition.margin_right = definitionRow["margin-right"].ToString(); definition.Savedmargin_right = definition.margin_right; }
            if (definitionRow.ContainsKey("padding")) { definition.padding = definitionRow["padding"].ToString(); definition.Savedpadding = definition.padding; }
            if (definitionRow.ContainsKey("padding-bottom")) { definition.padding_bottom = definitionRow["padding-bottom"].ToString(); definition.Savedpadding_bottom = definition.padding_bottom; }
            if (definitionRow.ContainsKey("text-align")) { definition.text_align = definitionRow["text-align"].ToString(); definition.Savedtext_align = definition.text_align; }
            if (definitionRow.ContainsKey("vertical-align")) { definition.vertical_align = definitionRow["vertical-align"].ToString(); definition.Savedvertical_align = definition.vertical_align; }
            if (definitionRow.ContainsKey("line-height")) { definition.line_height = definitionRow["line-height"].ToString(); definition.Savedline_height = definition.line_height; }
            if (definitionRow.ContainsKey("font-size")) { definition.font_size = definitionRow["font-size"].ToString(); definition.Savedfont_size = definition.font_size; }
            if (definitionRow.ContainsKey("font-family")) { definition.font_family = definitionRow["font-family"].ToString(); definition.Savedfont_family = definition.font_family; }
            if (definitionRow.ContainsKey("font-weight")) { definition.font_weight = definitionRow["font-weight"].ToString(); definition.Savedfont_weight = definition.font_weight; }
            if (definitionRow.ContainsKey("font-style")) { definition.font_style = definitionRow["font-style"].ToString(); definition.Savedfont_style = definition.font_style; }
            if (definitionRow.ContainsKey("color")) { definition.color = definitionRow["color"].ToString(); definition.Savedcolor = definition.color; }
            if (definitionRow.ContainsKey("background")) { definition.background = definitionRow["background"].ToString(); definition.Savedbackground = definition.background; }
            if (definitionRow.ContainsKey("background-color")) { definition.background_color = definitionRow["background-color"].ToString(); definition.Savedbackground_color = definition.background_color; }
            if (definitionRow.ContainsKey("border")) { definition.border = definitionRow["border"].ToString(); definition.Savedborder = definition.border; }
            if (definitionRow.ContainsKey("border-top")) { definition.border_top = definitionRow["border-top"].ToString(); definition.Savedborder_top = definition.border_top; }
            if (definitionRow.ContainsKey("border-bottom")) { definition.border_bottom = definitionRow["border-bottom"].ToString(); definition.Savedborder_bottom = definition.border_bottom; }
            if (definitionRow.ContainsKey("border-left")) { definition.border_left = definitionRow["border-left"].ToString(); definition.Savedborder_left = definition.border_left; }
            if (definitionRow.ContainsKey("border-right")) { definition.border_right = definitionRow["border-right"].ToString(); definition.Savedborder_right = definition.border_right; }
            if (definitionRow.ContainsKey("border-collapse")) { definition.border_collapse = definitionRow["border-collapse"].ToString(); definition.Savedborder_collapse = definition.border_collapse; }
            if (definitionRow.ContainsKey("border-spacing")) { definition.border_spacing = definitionRow["border-spacing"].ToString(); definition.Savedborder_spacing = definition.border_spacing; }
            if (definitionRow.ContainsKey("position")) { definition.position = definitionRow["position"].ToString(); definition.Savedposition = definition.position; }
            if (definitionRow.ContainsKey("top")) { definition.top = definitionRow["top"].ToString(); definition.Savedtop = definition.top; }
            if (definitionRow.ContainsKey("right")) { definition.right = definitionRow["right"].ToString(); definition.Savedright = definition.right; }
            if (definitionRow.ContainsKey("left")) { definition.left = definitionRow["left"].ToString(); definition.Savedleft = definition.left; }
            if (definitionRow.ContainsKey("bottom")) { definition.bottom = definitionRow["bottom"].ToString(); definition.Savedbottom = definition.bottom; }
            if (definitionRow.ContainsKey("cursor")) { definition.cursor = definitionRow["cursor"].ToString(); definition.Savedcursor = definition.cursor; }
            if (definitionRow.ContainsKey("clear")) { definition.clear = definitionRow["clear"].ToString(); definition.Savedclear = definition.clear; }
            if (definitionRow.ContainsKey("overflow")) { definition.overflow = definitionRow["overflow"].ToString(); definition.Savedoverflow = definition.overflow; }
            if (definitionRow.ContainsKey("word-wrap")) { definition.word_wrap = definitionRow["word-wrap"].ToString(); definition.Savedword_wrap = definition.word_wrap; }
            if (definitionRow.ContainsKey("word-break")) { definition.word_break = definitionRow["word-break"].ToString(); definition.Savedword_break = definition.word_break; }
            if (definitionRow.ContainsKey("white-space")) { definition.white_space = definitionRow["white-space"].ToString(); definition.Savedwhite_space = definition.white_space; }
            if (definitionRow.ContainsKey("table-layout")) { definition.table_layout = definitionRow["table-layout"].ToString(); definition.Savedtable_layout = definition.table_layout; }
            if (definitionRow.ContainsKey("text-decoration")) { definition.text_decoration = definitionRow["text-decoration"].ToString(); definition.Savedtext_decoration = definition.text_decoration; }
            if (definitionRow.ContainsKey("list-style-type")) { definition.list_style_type = definitionRow["list-style-type"].ToString(); definition.Savedlist_style_type = definition.list_style_type; }
            if (definitionRow.ContainsKey("visibility")) { definition.visibility = definitionRow["visibility"].ToString(); definition.Savedvisibility = definition.visibility; }
            if (definitionRow.ContainsKey("border-radius")) { definition.border_radius = definitionRow["border-radius"].ToString(); definition.Savedborder_radius = definition.border_radius; }
            if (definitionRow.ContainsKey("z-index")) { definition.z_index = definitionRow["z-index"].ToString(); definition.Savedz_index = definition.z_index; }
            if (definitionRow.ContainsKey("fill")) { definition.fill = definitionRow["fill"].ToString(); definition.Savedfill = definition.fill; }
            if (definitionRow.ContainsKey("fill-opacity")) { definition.fill_opacity = definitionRow["fill-opacity"].ToString(); definition.Savedfill_opacity = definition.fill_opacity; }
            if (definitionRow.ContainsKey("stroke")) { definition.stroke = definitionRow["stroke"].ToString(); definition.Savedstroke = definition.stroke; }
            if (definitionRow.ContainsKey("stroke-width")) { definition.stroke_width = definitionRow["stroke-width"].ToString(); definition.Savedstroke_width = definition.stroke_width; }
            if (definitionRow.ContainsKey("text-anchor")) { definition.text_anchor = definitionRow["text-anchor"].ToString(); definition.Savedtext_anchor = definition.text_anchor; }
            if (definitionRow.ContainsKey("shape-rendering")) { definition.shape_rendering = definitionRow["shape-rendering"].ToString(); definition.Savedshape_rendering = definition.shape_rendering; }
        }

        private static void ConstructCssDefinitions()
        {
            CssXls = Initializer.DefinitionFile("definition_Css.xlsm");
            CssDefinitionCollection = new List<CssDefinition>();
            Css = new CssColumn2nd();
            CssTable = new CssTable();
        }

        public static XlsIo DemoXls;
        public static List<DemoDefinition> DemoDefinitionCollection;
        public static DemoColumn2nd Demo;
        public static DemoTable DemoTable;

        public static void SetDemoDefinition()
        {
            ConstructDemoDefinitions();
            if (DemoXls.AccessStatus != Files.AccessStatuses.Read) { return; }
            DemoXls.XlsSheet.ForEach(definitionRow =>
            {
                switch (definitionRow[0].ToString())
                {
                    case "Dept1": Demo.Dept1 = definitionRow[1].ToString(); SetDemoTable(DemoTable.Dept1, definitionRow, DemoXls); break;
                    case "Dept2": Demo.Dept2 = definitionRow[1].ToString(); SetDemoTable(DemoTable.Dept2, definitionRow, DemoXls); break;
                    case "Dept3": Demo.Dept3 = definitionRow[1].ToString(); SetDemoTable(DemoTable.Dept3, definitionRow, DemoXls); break;
                    case "Dept4": Demo.Dept4 = definitionRow[1].ToString(); SetDemoTable(DemoTable.Dept4, definitionRow, DemoXls); break;
                    case "Dept5": Demo.Dept5 = definitionRow[1].ToString(); SetDemoTable(DemoTable.Dept5, definitionRow, DemoXls); break;
                    case "Dept6": Demo.Dept6 = definitionRow[1].ToString(); SetDemoTable(DemoTable.Dept6, definitionRow, DemoXls); break;
                    case "Dept7": Demo.Dept7 = definitionRow[1].ToString(); SetDemoTable(DemoTable.Dept7, definitionRow, DemoXls); break;
                    case "Dept8": Demo.Dept8 = definitionRow[1].ToString(); SetDemoTable(DemoTable.Dept8, definitionRow, DemoXls); break;
                    case "User1": Demo.User1 = definitionRow[1].ToString(); SetDemoTable(DemoTable.User1, definitionRow, DemoXls); break;
                    case "User2": Demo.User2 = definitionRow[1].ToString(); SetDemoTable(DemoTable.User2, definitionRow, DemoXls); break;
                    case "User3": Demo.User3 = definitionRow[1].ToString(); SetDemoTable(DemoTable.User3, definitionRow, DemoXls); break;
                    case "User4": Demo.User4 = definitionRow[1].ToString(); SetDemoTable(DemoTable.User4, definitionRow, DemoXls); break;
                    case "User5": Demo.User5 = definitionRow[1].ToString(); SetDemoTable(DemoTable.User5, definitionRow, DemoXls); break;
                    case "User6": Demo.User6 = definitionRow[1].ToString(); SetDemoTable(DemoTable.User6, definitionRow, DemoXls); break;
                    case "User7": Demo.User7 = definitionRow[1].ToString(); SetDemoTable(DemoTable.User7, definitionRow, DemoXls); break;
                    case "User8": Demo.User8 = definitionRow[1].ToString(); SetDemoTable(DemoTable.User8, definitionRow, DemoXls); break;
                    case "User9": Demo.User9 = definitionRow[1].ToString(); SetDemoTable(DemoTable.User9, definitionRow, DemoXls); break;
                    case "User10": Demo.User10 = definitionRow[1].ToString(); SetDemoTable(DemoTable.User10, definitionRow, DemoXls); break;
                    case "User11": Demo.User11 = definitionRow[1].ToString(); SetDemoTable(DemoTable.User11, definitionRow, DemoXls); break;
                    case "User12": Demo.User12 = definitionRow[1].ToString(); SetDemoTable(DemoTable.User12, definitionRow, DemoXls); break;
                    case "User13": Demo.User13 = definitionRow[1].ToString(); SetDemoTable(DemoTable.User13, definitionRow, DemoXls); break;
                    case "User14": Demo.User14 = definitionRow[1].ToString(); SetDemoTable(DemoTable.User14, definitionRow, DemoXls); break;
                    case "User15": Demo.User15 = definitionRow[1].ToString(); SetDemoTable(DemoTable.User15, definitionRow, DemoXls); break;
                    case "User16": Demo.User16 = definitionRow[1].ToString(); SetDemoTable(DemoTable.User16, definitionRow, DemoXls); break;
                    case "User17": Demo.User17 = definitionRow[1].ToString(); SetDemoTable(DemoTable.User17, definitionRow, DemoXls); break;
                    case "User18": Demo.User18 = definitionRow[1].ToString(); SetDemoTable(DemoTable.User18, definitionRow, DemoXls); break;
                    case "User19": Demo.User19 = definitionRow[1].ToString(); SetDemoTable(DemoTable.User19, definitionRow, DemoXls); break;
                    case "User20": Demo.User20 = definitionRow[1].ToString(); SetDemoTable(DemoTable.User20, definitionRow, DemoXls); break;
                    case "Site1": Demo.Site1 = definitionRow[1].ToString(); SetDemoTable(DemoTable.Site1, definitionRow, DemoXls); break;
                    case "Site2": Demo.Site2 = definitionRow[1].ToString(); SetDemoTable(DemoTable.Site2, definitionRow, DemoXls); break;
                    case "Site3": Demo.Site3 = definitionRow[1].ToString(); SetDemoTable(DemoTable.Site3, definitionRow, DemoXls); break;
                    case "Site4": Demo.Site4 = definitionRow[1].ToString(); SetDemoTable(DemoTable.Site4, definitionRow, DemoXls); break;
                    case "DefineNetworks": Demo.DefineNetworks = definitionRow[1].ToString(); SetDemoTable(DemoTable.DefineNetworks, definitionRow, DemoXls); break;
                    case "DefineServers": Demo.DefineServers = definitionRow[1].ToString(); SetDemoTable(DemoTable.DefineServers, definitionRow, DemoXls); break;
                    case "DefineSecurity": Demo.DefineSecurity = definitionRow[1].ToString(); SetDemoTable(DemoTable.DefineSecurity, definitionRow, DemoXls); break;
                    case "DefineOperationSystems": Demo.DefineOperationSystems = definitionRow[1].ToString(); SetDemoTable(DemoTable.DefineOperationSystems, definitionRow, DemoXls); break;
                    case "DesignNetworks": Demo.DesignNetworks = definitionRow[1].ToString(); SetDemoTable(DemoTable.DesignNetworks, definitionRow, DemoXls); break;
                    case "DesignServers": Demo.DesignServers = definitionRow[1].ToString(); SetDemoTable(DemoTable.DesignServers, definitionRow, DemoXls); break;
                    case "DesignOperationSystems": Demo.DesignOperationSystems = definitionRow[1].ToString(); SetDemoTable(DemoTable.DesignOperationSystems, definitionRow, DemoXls); break;
                    case "DesignApplication": Demo.DesignApplication = definitionRow[1].ToString(); SetDemoTable(DemoTable.DesignApplication, definitionRow, DemoXls); break;
                    case "DeveropDataMigrationTools": Demo.DeveropDataMigrationTools = definitionRow[1].ToString(); SetDemoTable(DemoTable.DeveropDataMigrationTools, definitionRow, DemoXls); break;
                    case "ConfigNetwork": Demo.ConfigNetwork = definitionRow[1].ToString(); SetDemoTable(DemoTable.ConfigNetwork, definitionRow, DemoXls); break;
                    case "ConfigServers": Demo.ConfigServers = definitionRow[1].ToString(); SetDemoTable(DemoTable.ConfigServers, definitionRow, DemoXls); break;
                    case "ConfigOperationSystems": Demo.ConfigOperationSystems = definitionRow[1].ToString(); SetDemoTable(DemoTable.ConfigOperationSystems, definitionRow, DemoXls); break;
                    case "ConfigApplications": Demo.ConfigApplications = definitionRow[1].ToString(); SetDemoTable(DemoTable.ConfigApplications, definitionRow, DemoXls); break;
                    case "TestNetworks": Demo.TestNetworks = definitionRow[1].ToString(); SetDemoTable(DemoTable.TestNetworks, definitionRow, DemoXls); break;
                    case "TestServers": Demo.TestServers = definitionRow[1].ToString(); SetDemoTable(DemoTable.TestServers, definitionRow, DemoXls); break;
                    case "TestOperationSystems": Demo.TestOperationSystems = definitionRow[1].ToString(); SetDemoTable(DemoTable.TestOperationSystems, definitionRow, DemoXls); break;
                    case "TestApplications": Demo.TestApplications = definitionRow[1].ToString(); SetDemoTable(DemoTable.TestApplications, definitionRow, DemoXls); break;
                    case "WriteOperationDocuments": Demo.WriteOperationDocuments = definitionRow[1].ToString(); SetDemoTable(DemoTable.WriteOperationDocuments, definitionRow, DemoXls); break;
                    case "WriteUserDocuments": Demo.WriteUserDocuments = definitionRow[1].ToString(); SetDemoTable(DemoTable.WriteUserDocuments, definitionRow, DemoXls); break;
                    case "DesignSupportDesk": Demo.DesignSupportDesk = definitionRow[1].ToString(); SetDemoTable(DemoTable.DesignSupportDesk, definitionRow, DemoXls); break;
                    case "TestSystems": Demo.TestSystems = definitionRow[1].ToString(); SetDemoTable(DemoTable.TestSystems, definitionRow, DemoXls); break;
                    case "Transition": Demo.Transition = definitionRow[1].ToString(); SetDemoTable(DemoTable.Transition, definitionRow, DemoXls); break;
                    case "Report": Demo.Report = definitionRow[1].ToString(); SetDemoTable(DemoTable.Report, definitionRow, DemoXls); break;
                    case "Issue1": Demo.Issue1 = definitionRow[1].ToString(); SetDemoTable(DemoTable.Issue1, definitionRow, DemoXls); break;
                    case "Result1": Demo.Result1 = definitionRow[1].ToString(); SetDemoTable(DemoTable.Result1, definitionRow, DemoXls); break;
                    case "Comment1": Demo.Comment1 = definitionRow[1].ToString(); SetDemoTable(DemoTable.Comment1, definitionRow, DemoXls); break;
                    case "Comment2": Demo.Comment2 = definitionRow[1].ToString(); SetDemoTable(DemoTable.Comment2, definitionRow, DemoXls); break;
                    case "Comment3": Demo.Comment3 = definitionRow[1].ToString(); SetDemoTable(DemoTable.Comment3, definitionRow, DemoXls); break;
                    case "Comment4": Demo.Comment4 = definitionRow[1].ToString(); SetDemoTable(DemoTable.Comment4, definitionRow, DemoXls); break;
                    case "Comment5": Demo.Comment5 = definitionRow[1].ToString(); SetDemoTable(DemoTable.Comment5, definitionRow, DemoXls); break;
                    case "Comment6": Demo.Comment6 = definitionRow[1].ToString(); SetDemoTable(DemoTable.Comment6, definitionRow, DemoXls); break;
                    case "Comment7": Demo.Comment7 = definitionRow[1].ToString(); SetDemoTable(DemoTable.Comment7, definitionRow, DemoXls); break;
                    case "Comment8": Demo.Comment8 = definitionRow[1].ToString(); SetDemoTable(DemoTable.Comment8, definitionRow, DemoXls); break;
                    default: break;
                }
            });
            DemoXls.XlsSheet.AsEnumerable().Skip(1).Where(o => o[0].ToString() != string.Empty).ForEach(definitionRow =>
            {
                var newDemoDefinition = new DemoDefinition();
                if (definitionRow.ContainsKey("Id")) { newDemoDefinition.Id = definitionRow["Id"].ToString(); newDemoDefinition.SavedId = newDemoDefinition.Id; }
                if (definitionRow.ContainsKey("Body")) { newDemoDefinition.Body = definitionRow["Body"].ToString(); newDemoDefinition.SavedBody = newDemoDefinition.Body; }
                if (definitionRow.ContainsKey("Type")) { newDemoDefinition.Type = definitionRow["Type"].ToString(); newDemoDefinition.SavedType = newDemoDefinition.Type; }
                if (definitionRow.ContainsKey("ParentId")) { newDemoDefinition.ParentId = definitionRow["ParentId"].ToString(); newDemoDefinition.SavedParentId = newDemoDefinition.ParentId; }
                if (definitionRow.ContainsKey("Title")) { newDemoDefinition.Title = definitionRow["Title"].ToString(); newDemoDefinition.SavedTitle = newDemoDefinition.Title; }
                if (definitionRow.ContainsKey("WorkValue")) { newDemoDefinition.WorkValue = definitionRow["WorkValue"].ToDecimal(); newDemoDefinition.SavedWorkValue = newDemoDefinition.WorkValue; }
                if (definitionRow.ContainsKey("ProgressRate")) { newDemoDefinition.ProgressRate = definitionRow["ProgressRate"].ToDecimal(); newDemoDefinition.SavedProgressRate = newDemoDefinition.ProgressRate; }
                if (definitionRow.ContainsKey("Status")) { newDemoDefinition.Status = definitionRow["Status"].ToInt(); newDemoDefinition.SavedStatus = newDemoDefinition.Status; }
                if (definitionRow.ContainsKey("ClassA")) { newDemoDefinition.ClassA = definitionRow["ClassA"].ToString(); newDemoDefinition.SavedClassA = newDemoDefinition.ClassA; }
                if (definitionRow.ContainsKey("ClassB")) { newDemoDefinition.ClassB = definitionRow["ClassB"].ToString(); newDemoDefinition.SavedClassB = newDemoDefinition.ClassB; }
                if (definitionRow.ContainsKey("ClassC")) { newDemoDefinition.ClassC = definitionRow["ClassC"].ToString(); newDemoDefinition.SavedClassC = newDemoDefinition.ClassC; }
                if (definitionRow.ContainsKey("Manager")) { newDemoDefinition.Manager = definitionRow["Manager"].ToString(); newDemoDefinition.SavedManager = newDemoDefinition.Manager; }
                if (definitionRow.ContainsKey("Owner")) { newDemoDefinition.Owner = definitionRow["Owner"].ToString(); newDemoDefinition.SavedOwner = newDemoDefinition.Owner; }
                if (definitionRow.ContainsKey("Creator")) { newDemoDefinition.Creator = definitionRow["Creator"].ToString(); newDemoDefinition.SavedCreator = newDemoDefinition.Creator; }
                if (definitionRow.ContainsKey("Updator")) { newDemoDefinition.Updator = definitionRow["Updator"].ToString(); newDemoDefinition.SavedUpdator = newDemoDefinition.Updator; }
                if (definitionRow.ContainsKey("StartTime")) { newDemoDefinition.StartTime = definitionRow["StartTime"].ToDateTime(); newDemoDefinition.SavedStartTime = newDemoDefinition.StartTime; }
                if (definitionRow.ContainsKey("CompletionTime")) { newDemoDefinition.CompletionTime = definitionRow["CompletionTime"].ToDateTime(); newDemoDefinition.SavedCompletionTime = newDemoDefinition.CompletionTime; }
                if (definitionRow.ContainsKey("CreatedTime")) { newDemoDefinition.CreatedTime = definitionRow["CreatedTime"].ToDateTime(); newDemoDefinition.SavedCreatedTime = newDemoDefinition.CreatedTime; }
                if (definitionRow.ContainsKey("UpdatedTime")) { newDemoDefinition.UpdatedTime = definitionRow["UpdatedTime"].ToDateTime(); newDemoDefinition.SavedUpdatedTime = newDemoDefinition.UpdatedTime; }
                DemoDefinitionCollection.Add(newDemoDefinition);
            });
        }

        private static void SetDemoTable(DemoDefinition definition, XlsRow definitionRow, XlsIo demoxls)
        {
            if (definitionRow.ContainsKey("Id")) { definition.Id = definitionRow["Id"].ToString(); definition.SavedId = definition.Id; }
            if (definitionRow.ContainsKey("Body")) { definition.Body = definitionRow["Body"].ToString(); definition.SavedBody = definition.Body; }
            if (definitionRow.ContainsKey("Type")) { definition.Type = definitionRow["Type"].ToString(); definition.SavedType = definition.Type; }
            if (definitionRow.ContainsKey("ParentId")) { definition.ParentId = definitionRow["ParentId"].ToString(); definition.SavedParentId = definition.ParentId; }
            if (definitionRow.ContainsKey("Title")) { definition.Title = definitionRow["Title"].ToString(); definition.SavedTitle = definition.Title; }
            if (definitionRow.ContainsKey("WorkValue")) { definition.WorkValue = definitionRow["WorkValue"].ToDecimal(); definition.SavedWorkValue = definition.WorkValue; }
            if (definitionRow.ContainsKey("ProgressRate")) { definition.ProgressRate = definitionRow["ProgressRate"].ToDecimal(); definition.SavedProgressRate = definition.ProgressRate; }
            if (definitionRow.ContainsKey("Status")) { definition.Status = definitionRow["Status"].ToInt(); definition.SavedStatus = definition.Status; }
            if (definitionRow.ContainsKey("ClassA")) { definition.ClassA = definitionRow["ClassA"].ToString(); definition.SavedClassA = definition.ClassA; }
            if (definitionRow.ContainsKey("ClassB")) { definition.ClassB = definitionRow["ClassB"].ToString(); definition.SavedClassB = definition.ClassB; }
            if (definitionRow.ContainsKey("ClassC")) { definition.ClassC = definitionRow["ClassC"].ToString(); definition.SavedClassC = definition.ClassC; }
            if (definitionRow.ContainsKey("Manager")) { definition.Manager = definitionRow["Manager"].ToString(); definition.SavedManager = definition.Manager; }
            if (definitionRow.ContainsKey("Owner")) { definition.Owner = definitionRow["Owner"].ToString(); definition.SavedOwner = definition.Owner; }
            if (definitionRow.ContainsKey("Creator")) { definition.Creator = definitionRow["Creator"].ToString(); definition.SavedCreator = definition.Creator; }
            if (definitionRow.ContainsKey("Updator")) { definition.Updator = definitionRow["Updator"].ToString(); definition.SavedUpdator = definition.Updator; }
            if (definitionRow.ContainsKey("StartTime")) { definition.StartTime = definitionRow["StartTime"].ToDateTime(); definition.SavedStartTime = definition.StartTime; }
            if (definitionRow.ContainsKey("CompletionTime")) { definition.CompletionTime = definitionRow["CompletionTime"].ToDateTime(); definition.SavedCompletionTime = definition.CompletionTime; }
            if (definitionRow.ContainsKey("CreatedTime")) { definition.CreatedTime = definitionRow["CreatedTime"].ToDateTime(); definition.SavedCreatedTime = definition.CreatedTime; }
            if (definitionRow.ContainsKey("UpdatedTime")) { definition.UpdatedTime = definitionRow["UpdatedTime"].ToDateTime(); definition.SavedUpdatedTime = definition.UpdatedTime; }
        }

        private static void ConstructDemoDefinitions()
        {
            DemoXls = Initializer.DefinitionFile("definition_Demo.xlsm");
            DemoDefinitionCollection = new List<DemoDefinition>();
            Demo = new DemoColumn2nd();
            DemoTable = new DemoTable();
        }

        public static XlsIo SqlXls;
        public static List<SqlDefinition> SqlDefinitionCollection;
        public static SqlColumn2nd Sql;
        public static SqlTable SqlTable;

        public static void SetSqlDefinition()
        {
            ConstructSqlDefinitions();
            if (SqlXls.AccessStatus != Files.AccessStatuses.Read) { return; }
            SqlXls.XlsSheet.ForEach(definitionRow =>
            {
                switch (definitionRow[0].ToString())
                {
                    case "BeginTransaction": Sql.BeginTransaction = definitionRow[1].ToString(); SetSqlTable(SqlTable.BeginTransaction, definitionRow, SqlXls); break;
                    case "CommitTransaction": Sql.CommitTransaction = definitionRow[1].ToString(); SetSqlTable(SqlTable.CommitTransaction, definitionRow, SqlXls); break;
                    case "CanRead": Sql.CanRead = definitionRow[1].ToString(); SetSqlTable(SqlTable.CanRead, definitionRow, SqlXls); break;
                    case "SiteDepts": Sql.SiteDepts = definitionRow[1].ToString(); SetSqlTable(SqlTable.SiteDepts, definitionRow, SqlXls); break;
                    case "ProgressRateDelay": Sql.ProgressRateDelay = definitionRow[1].ToString(); SetSqlTable(SqlTable.ProgressRateDelay, definitionRow, SqlXls); break;
                    case "MoveTarget": Sql.MoveTarget = definitionRow[1].ToString(); SetSqlTable(SqlTable.MoveTarget, definitionRow, SqlXls); break;
                    case "CreateDatabase": Sql.CreateDatabase = definitionRow[1].ToString(); SetSqlTable(SqlTable.CreateDatabase, definitionRow, SqlXls); break;
                    case "SpWho": Sql.SpWho = definitionRow[1].ToString(); SetSqlTable(SqlTable.SpWho, definitionRow, SqlXls); break;
                    case "KillSpid": Sql.KillSpid = definitionRow[1].ToString(); SetSqlTable(SqlTable.KillSpid, definitionRow, SqlXls); break;
                    case "RecreateLoginUser": Sql.RecreateLoginUser = definitionRow[1].ToString(); SetSqlTable(SqlTable.RecreateLoginUser, definitionRow, SqlXls); break;
                    case "RecreateLoginAdmin": Sql.RecreateLoginAdmin = definitionRow[1].ToString(); SetSqlTable(SqlTable.RecreateLoginAdmin, definitionRow, SqlXls); break;
                    case "ExistsTable": Sql.ExistsTable = definitionRow[1].ToString(); SetSqlTable(SqlTable.ExistsTable, definitionRow, SqlXls); break;
                    case "CreateTable": Sql.CreateTable = definitionRow[1].ToString(); SetSqlTable(SqlTable.CreateTable, definitionRow, SqlXls); break;
                    case "CreatePk": Sql.CreatePk = definitionRow[1].ToString(); SetSqlTable(SqlTable.CreatePk, definitionRow, SqlXls); break;
                    case "CreateIx": Sql.CreateIx = definitionRow[1].ToString(); SetSqlTable(SqlTable.CreateIx, definitionRow, SqlXls); break;
                    case "DeleteDefault": Sql.DeleteDefault = definitionRow[1].ToString(); SetSqlTable(SqlTable.DeleteDefault, definitionRow, SqlXls); break;
                    case "CreateDefault": Sql.CreateDefault = definitionRow[1].ToString(); SetSqlTable(SqlTable.CreateDefault, definitionRow, SqlXls); break;
                    case "Columns": Sql.Columns = definitionRow[1].ToString(); SetSqlTable(SqlTable.Columns, definitionRow, SqlXls); break;
                    case "Defaults": Sql.Defaults = definitionRow[1].ToString(); SetSqlTable(SqlTable.Defaults, definitionRow, SqlXls); break;
                    case "Indexes": Sql.Indexes = definitionRow[1].ToString(); SetSqlTable(SqlTable.Indexes, definitionRow, SqlXls); break;
                    case "DropConstraint": Sql.DropConstraint = definitionRow[1].ToString(); SetSqlTable(SqlTable.DropConstraint, definitionRow, SqlXls); break;
                    case "DropIndex": Sql.DropIndex = definitionRow[1].ToString(); SetSqlTable(SqlTable.DropIndex, definitionRow, SqlXls); break;
                    case "MigrateTableWithIdentity": Sql.MigrateTableWithIdentity = definitionRow[1].ToString(); SetSqlTable(SqlTable.MigrateTableWithIdentity, definitionRow, SqlXls); break;
                    case "MigrateTable": Sql.MigrateTable = definitionRow[1].ToString(); SetSqlTable(SqlTable.MigrateTable, definitionRow, SqlXls); break;
                    case "BulkInsert": Sql.BulkInsert = definitionRow[1].ToString(); SetSqlTable(SqlTable.BulkInsert, definitionRow, SqlXls); break;
                    case "Identity": Sql.Identity = definitionRow[1].ToString(); SetSqlTable(SqlTable.Identity, definitionRow, SqlXls); break;
                    default: break;
                }
            });
            SqlXls.XlsSheet.AsEnumerable().Skip(1).Where(o => o[0].ToString() != string.Empty).ForEach(definitionRow =>
            {
                var newSqlDefinition = new SqlDefinition();
                if (definitionRow.ContainsKey("Id")) { newSqlDefinition.Id = definitionRow["Id"].ToString(); newSqlDefinition.SavedId = newSqlDefinition.Id; }
                if (definitionRow.ContainsKey("Body")) { newSqlDefinition.Body = definitionRow["Body"].ToString(); newSqlDefinition.SavedBody = newSqlDefinition.Body; }
                SqlDefinitionCollection.Add(newSqlDefinition);
            });
        }

        private static void SetSqlTable(SqlDefinition definition, XlsRow definitionRow, XlsIo sqlxls)
        {
            if (definitionRow.ContainsKey("Id")) { definition.Id = definitionRow["Id"].ToString(); definition.SavedId = definition.Id; }
            if (definitionRow.ContainsKey("Body")) { definition.Body = definitionRow["Body"].ToString(); definition.SavedBody = definition.Body; }
        }

        private static void ConstructSqlDefinitions()
        {
            SqlXls = Initializer.DefinitionFile("definition_Sql.xlsm");
            SqlDefinitionCollection = new List<SqlDefinition>();
            Sql = new SqlColumn2nd();
            SqlTable = new SqlTable();
        }

        public static XlsIo ViewModeXls;
        public static List<ViewModeDefinition> ViewModeDefinitionCollection;
        public static ViewModeColumn2nd ViewMode;
        public static ViewModeTable ViewModeTable;

        public static void SetViewModeDefinition()
        {
            ConstructViewModeDefinitions();
            if (ViewModeXls.AccessStatus != Files.AccessStatuses.Read) { return; }
            ViewModeXls.XlsSheet.ForEach(definitionRow =>
            {
                switch (definitionRow[0].ToString())
                {
                    case "Issues_Index": ViewMode.Issues_Index = definitionRow[1].ToString(); SetViewModeTable(ViewModeTable.Issues_Index, definitionRow, ViewModeXls); break;
                    case "Issues_Gantt": ViewMode.Issues_Gantt = definitionRow[1].ToString(); SetViewModeTable(ViewModeTable.Issues_Gantt, definitionRow, ViewModeXls); break;
                    case "Issues_BurnDown": ViewMode.Issues_BurnDown = definitionRow[1].ToString(); SetViewModeTable(ViewModeTable.Issues_BurnDown, definitionRow, ViewModeXls); break;
                    case "Issues_TimeSeries": ViewMode.Issues_TimeSeries = definitionRow[1].ToString(); SetViewModeTable(ViewModeTable.Issues_TimeSeries, definitionRow, ViewModeXls); break;
                    case "Issues_Kamban": ViewMode.Issues_Kamban = definitionRow[1].ToString(); SetViewModeTable(ViewModeTable.Issues_Kamban, definitionRow, ViewModeXls); break;
                    case "Results_Index": ViewMode.Results_Index = definitionRow[1].ToString(); SetViewModeTable(ViewModeTable.Results_Index, definitionRow, ViewModeXls); break;
                    case "Results_TimeSeries": ViewMode.Results_TimeSeries = definitionRow[1].ToString(); SetViewModeTable(ViewModeTable.Results_TimeSeries, definitionRow, ViewModeXls); break;
                    case "Results_Kamban": ViewMode.Results_Kamban = definitionRow[1].ToString(); SetViewModeTable(ViewModeTable.Results_Kamban, definitionRow, ViewModeXls); break;
                    default: break;
                }
            });
            ViewModeXls.XlsSheet.AsEnumerable().Skip(1).Where(o => o[0].ToString() != string.Empty).ForEach(definitionRow =>
            {
                var newViewModeDefinition = new ViewModeDefinition();
                if (definitionRow.ContainsKey("Id")) { newViewModeDefinition.Id = definitionRow["Id"].ToString(); newViewModeDefinition.SavedId = newViewModeDefinition.Id; }
                if (definitionRow.ContainsKey("ReferenceType")) { newViewModeDefinition.ReferenceType = definitionRow["ReferenceType"].ToString(); newViewModeDefinition.SavedReferenceType = newViewModeDefinition.ReferenceType; }
                if (definitionRow.ContainsKey("Name")) { newViewModeDefinition.Name = definitionRow["Name"].ToString(); newViewModeDefinition.SavedName = newViewModeDefinition.Name; }
                ViewModeDefinitionCollection.Add(newViewModeDefinition);
            });
        }

        private static void SetViewModeTable(ViewModeDefinition definition, XlsRow definitionRow, XlsIo viewModexls)
        {
            if (definitionRow.ContainsKey("Id")) { definition.Id = definitionRow["Id"].ToString(); definition.SavedId = definition.Id; }
            if (definitionRow.ContainsKey("ReferenceType")) { definition.ReferenceType = definitionRow["ReferenceType"].ToString(); definition.SavedReferenceType = definition.ReferenceType; }
            if (definitionRow.ContainsKey("Name")) { definition.Name = definitionRow["Name"].ToString(); definition.SavedName = definition.Name; }
        }

        private static void ConstructViewModeDefinitions()
        {
            ViewModeXls = Initializer.DefinitionFile("definition_ViewMode.xlsm");
            ViewModeDefinitionCollection = new List<ViewModeDefinition>();
            ViewMode = new ViewModeColumn2nd();
            ViewModeTable = new ViewModeTable();
        }

        public static void SetCodeDefinitionOption(
            string placeholder, CodeDefinition codeDefinition)
        {
            placeholder.RegexFirst("(?<=\\().+(?=\\))").Split(',')
                .Where(o => !o.IsNullOrEmpty()).ForEach(option =>
                {
                    var optionName = option.Split_1st('=').Trim();
                    var optionValue = option.Split_2nd('=').Trim();
                    switch (optionName)
                    {
                        case "Id": codeDefinition.Id = optionValue.ToString(); break;
                        case "Body": codeDefinition.Body = optionValue.ToString(); break;
                        case "OutputPath": codeDefinition.OutputPath = optionValue.ToString(); break;
                        case "MergeToExisting": codeDefinition.MergeToExisting = optionValue.ToBool(); break;
                        case "Source": codeDefinition.Source = optionValue.ToString(); break;
                        case "RepeatType": codeDefinition.RepeatType = optionValue.ToString(); break;
                        case "Indent": codeDefinition.Indent = optionValue.ToInt(); break;
                        case "Separator": codeDefinition.Separator = optionValue.ToString(); break;
                        case "Order": codeDefinition.Order = optionValue.ToString(); break;
                        case "Pk": codeDefinition.Pk = optionValue.ToBool(); break;
                        case "NotPk": codeDefinition.NotPk = optionValue.ToBool(); break;
                        case "Identity": codeDefinition.Identity = optionValue.ToBool(); break;
                        case "NotIdentity": codeDefinition.NotIdentity = optionValue.ToBool(); break;
                        case "IdentityOrPk": codeDefinition.IdentityOrPk = optionValue.ToBool(); break;
                        case "Unique": codeDefinition.Unique = optionValue.ToBool(); break;
                        case "NotUnique": codeDefinition.NotUnique = optionValue.ToBool(); break;
                        case "NotDefault": codeDefinition.NotDefault = optionValue.ToBool(); break;
                        case "Like": codeDefinition.Like = optionValue.ToBool(); break;
                        case "HasIdentity": codeDefinition.HasIdentity = optionValue.ToBool(); break;
                        case "HasNotIdentity": codeDefinition.HasNotIdentity = optionValue.ToBool(); break;
                        case "HasTableNameId": codeDefinition.HasTableNameId = optionValue.ToBool(); break;
                        case "HasNotTableNameId": codeDefinition.HasNotTableNameId = optionValue.ToBool(); break;
                        case "ItemId": codeDefinition.ItemId = optionValue.ToBool(); break;
                        case "NotItemId": codeDefinition.NotItemId = optionValue.ToBool(); break;
                        case "Calc": codeDefinition.Calc = optionValue.ToBool(); break;
                        case "NotCalc": codeDefinition.NotCalc = optionValue.ToBool(); break;
                        case "SearchIndex": codeDefinition.SearchIndex = optionValue.ToBool(); break;
                        case "NotByForm": codeDefinition.NotByForm = optionValue.ToBool(); break;
                        case "Form": codeDefinition.Form = optionValue.ToBool(); break;
                        case "Select": codeDefinition.Select = optionValue.ToBool(); break;
                        case "Update": codeDefinition.Update = optionValue.ToBool(); break;
                        case "SelectColumns": codeDefinition.SelectColumns = optionValue.ToBool(); break;
                        case "NotSelectColumn": codeDefinition.NotSelectColumn = optionValue.ToBool(); break;
                        case "ComputeColumn": codeDefinition.ComputeColumn = optionValue.ToBool(); break;
                        case "NotComputeColumn": codeDefinition.NotComputeColumn = optionValue.ToBool(); break;
                        case "Aggregatable": codeDefinition.Aggregatable = optionValue.ToBool(); break;
                        case "Computable": codeDefinition.Computable = optionValue.ToBool(); break;
                        case "Join": codeDefinition.Join = optionValue.ToBool(); break;
                        case "NotJoin": codeDefinition.NotJoin = optionValue.ToBool(); break;
                        case "JoinExpression": codeDefinition.JoinExpression = optionValue.ToBool(); break;
                        case "NotTypeCs": codeDefinition.NotTypeCs = optionValue.ToBool(); break;
                        case "ItemOnly": codeDefinition.ItemOnly = optionValue.ToBool(); break;
                        case "NotItem": codeDefinition.NotItem = optionValue.ToBool(); break;
                        case "GenericUi": codeDefinition.GenericUi = optionValue.ToBool(); break;
                        case "Session": codeDefinition.Session = optionValue.ToBool(); break;
                        case "GridColumn": codeDefinition.GridColumn = optionValue.ToBool(); break;
                        case "FilterColumn": codeDefinition.FilterColumn = optionValue.ToBool(); break;
                        case "EditorColumn": codeDefinition.EditorColumn = optionValue.ToBool(); break;
                        case "TitleColumn": codeDefinition.TitleColumn = optionValue.ToBool(); break;
                        case "UserColumn": codeDefinition.UserColumn = optionValue.ToBool(); break;
                        case "EnumColumn": codeDefinition.EnumColumn = optionValue.ToBool(); break;
                        case "Include": codeDefinition.Include = optionValue.ToString(); break;
                        case "Exclude": codeDefinition.Exclude = optionValue.ToString(); break;
                        case "IncludeTypeName": codeDefinition.IncludeTypeName = optionValue.ToString(); break;
                        case "ExcludeTypeName": codeDefinition.ExcludeTypeName = optionValue.ToString(); break;
                        case "IncludeDefaultCs": codeDefinition.IncludeDefaultCs = optionValue.ToString(); break;
                        case "ExcludeDefaultCs": codeDefinition.ExcludeDefaultCs = optionValue.ToString(); break;
                        case "History": codeDefinition.History = optionValue.ToBool(); break;
                        case "PkHistory": codeDefinition.PkHistory = optionValue.ToBool(); break;
                        case "ControlType": codeDefinition.ControlType = optionValue.ToString(); break;
                        case "ReplaceOld": codeDefinition.ReplaceOld = optionValue.ToString(); break;
                        case "ReplaceNew": codeDefinition.ReplaceNew = optionValue.ToString(); break;
                        case "NotWhereSpecial": codeDefinition.NotWhereSpecial = optionValue.ToBool(); break;
                        case "NoSpace": codeDefinition.NoSpace = optionValue.ToBool(); break;
                        case "NotBase": codeDefinition.NotBase = optionValue.ToBool(); break;
                        case "NotNull": codeDefinition.NotNull = optionValue.ToBool(); break;
                        case "Validators": codeDefinition.Validators = optionValue.ToBool(); break;
                        case "DisplayType": codeDefinition.DisplayType = optionValue.ToString(); break;
                        case "DisplayLanguages": codeDefinition.DisplayLanguages = optionValue.ToBool(); break;
                        case "ClientScript": codeDefinition.ClientScript = optionValue.ToBool(); break;
                    }
                });
        }

        public static void SetColumnDefinitionOption(
            string placeholder, ColumnDefinition columnDefinition)
        {
            placeholder.RegexFirst("(?<=\\().+(?=\\))").Split(',')
                .Where(o => !o.IsNullOrEmpty()).ForEach(option =>
                {
                    var optionName = option.Split_1st('=').Trim();
                    var optionValue = option.Split_2nd('=').Trim();
                    switch (optionName)
                    {
                        case "Id": columnDefinition.Id = optionValue.ToString(); break;
                        case "ModelName": columnDefinition.ModelName = optionValue.ToString(); break;
                        case "TableName": columnDefinition.TableName = optionValue.ToString(); break;
                        case "Base": columnDefinition.Base = optionValue.ToBool(); break;
                        case "EachModel": columnDefinition.EachModel = optionValue.ToBool(); break;
                        case "Label": columnDefinition.Label = optionValue.ToString(); break;
                        case "ColumnName": columnDefinition.ColumnName = optionValue.ToString(); break;
                        case "ColumnLabel": columnDefinition.ColumnLabel = optionValue.ToString(); break;
                        case "No": columnDefinition.No = optionValue.ToInt(); break;
                        case "History": columnDefinition.History = optionValue.ToInt(); break;
                        case "Import": columnDefinition.Import = optionValue.ToInt(); break;
                        case "Export": columnDefinition.Export = optionValue.ToInt(); break;
                        case "GridColumn": columnDefinition.GridColumn = optionValue.ToInt(); break;
                        case "GridEnabled": columnDefinition.GridEnabled = optionValue.ToBool(); break;
                        case "FilterColumn": columnDefinition.FilterColumn = optionValue.ToInt(); break;
                        case "FilterEnabled": columnDefinition.FilterEnabled = optionValue.ToBool(); break;
                        case "EditorColumn": columnDefinition.EditorColumn = optionValue.ToBool(); break;
                        case "EditorEnabled": columnDefinition.EditorEnabled = optionValue.ToBool(); break;
                        case "TitleColumn": columnDefinition.TitleColumn = optionValue.ToInt(); break;
                        case "LinkColumn": columnDefinition.LinkColumn = optionValue.ToInt(); break;
                        case "LinkEnabled": columnDefinition.LinkEnabled = optionValue.ToBool(); break;
                        case "HistoryColumn": columnDefinition.HistoryColumn = optionValue.ToInt(); break;
                        case "HistoryEnabled": columnDefinition.HistoryEnabled = optionValue.ToBool(); break;
                        case "TypeName": columnDefinition.TypeName = optionValue.ToString(); break;
                        case "TypeCs": columnDefinition.TypeCs = optionValue.ToString(); break;
                        case "RecordingData": columnDefinition.RecordingData = optionValue.ToString(); break;
                        case "MaxLength": columnDefinition.MaxLength = optionValue.ToInt(); break;
                        case "Size": columnDefinition.Size = optionValue.ToString(); break;
                        case "Pk": columnDefinition.Pk = optionValue.ToInt(); break;
                        case "PkOrderBy": columnDefinition.PkOrderBy = optionValue.ToString(); break;
                        case "PkHistory": columnDefinition.PkHistory = optionValue.ToInt(); break;
                        case "PkHistoryOrderBy": columnDefinition.PkHistoryOrderBy = optionValue.ToString(); break;
                        case "Ix1": columnDefinition.Ix1 = optionValue.ToInt(); break;
                        case "Ix1OrderBy": columnDefinition.Ix1OrderBy = optionValue.ToString(); break;
                        case "Ix2": columnDefinition.Ix2 = optionValue.ToInt(); break;
                        case "Ix2OrderBy": columnDefinition.Ix2OrderBy = optionValue.ToString(); break;
                        case "Ix3": columnDefinition.Ix3 = optionValue.ToInt(); break;
                        case "Ix3OrderBy": columnDefinition.Ix3OrderBy = optionValue.ToString(); break;
                        case "Nullable": columnDefinition.Nullable = optionValue.ToBool(); break;
                        case "Default": columnDefinition.Default = optionValue.ToString(); break;
                        case "DefaultCs": columnDefinition.DefaultCs = optionValue.ToString(); break;
                        case "Identity": columnDefinition.Identity = optionValue.ToBool(); break;
                        case "Unique": columnDefinition.Unique = optionValue.ToBool(); break;
                        case "Seed": columnDefinition.Seed = optionValue.ToInt(); break;
                        case "JoinTableName": columnDefinition.JoinTableName = optionValue.ToString(); break;
                        case "JoinType": columnDefinition.JoinType = optionValue.ToString(); break;
                        case "JoinExpression": columnDefinition.JoinExpression = optionValue.ToString(); break;
                        case "Like": columnDefinition.Like = optionValue.ToBool(); break;
                        case "WhereSpecial": columnDefinition.WhereSpecial = optionValue.ToBool(); break;
                        case "ReadPermission": columnDefinition.ReadPermission = optionValue.ToString(); break;
                        case "CreatePermission": columnDefinition.CreatePermission = optionValue.ToString(); break;
                        case "UpdatePermission": columnDefinition.UpdatePermission = optionValue.ToString(); break;
                        case "NotEditSelf": columnDefinition.NotEditSelf = optionValue.ToBool(); break;
                        case "SearchIndexPriority": columnDefinition.SearchIndexPriority = optionValue.ToInt(); break;
                        case "NotForm": columnDefinition.NotForm = optionValue.ToBool(); break;
                        case "NotSelect": columnDefinition.NotSelect = optionValue.ToBool(); break;
                        case "NotUpdate": columnDefinition.NotUpdate = optionValue.ToBool(); break;
                        case "ByForm": columnDefinition.ByForm = optionValue.ToString(); break;
                        case "ByDataRow": columnDefinition.ByDataRow = optionValue.ToString(); break;
                        case "BySession": columnDefinition.BySession = optionValue.ToString(); break;
                        case "ToControl": columnDefinition.ToControl = optionValue.ToString(); break;
                        case "SelectColumns": columnDefinition.SelectColumns = optionValue.ToString(); break;
                        case "ComputeColumn": columnDefinition.ComputeColumn = optionValue.ToString(); break;
                        case "OrderByColumns": columnDefinition.OrderByColumns = optionValue.ToString(); break;
                        case "ItemId": columnDefinition.ItemId = optionValue.ToInt(); break;
                        case "GenericUi": columnDefinition.GenericUi = optionValue.ToBool(); break;
                        case "FieldCss": columnDefinition.FieldCss = optionValue.ToString(); break;
                        case "ControlCss": columnDefinition.ControlCss = optionValue.ToString(); break;
                        case "MarkDown": columnDefinition.MarkDown = optionValue.ToBool(); break;
                        case "GridStyle": columnDefinition.GridStyle = optionValue.ToString(); break;
                        case "Hash": columnDefinition.Hash = optionValue.ToBool(); break;
                        case "Calc": columnDefinition.Calc = optionValue.ToString(); break;
                        case "Session": columnDefinition.Session = optionValue.ToBool(); break;
                        case "UserColumn": columnDefinition.UserColumn = optionValue.ToBool(); break;
                        case "EnumColumn": columnDefinition.EnumColumn = optionValue.ToBool(); break;
                        case "NotEditorSettings": columnDefinition.NotEditorSettings = optionValue.ToBool(); break;
                        case "ControlType": columnDefinition.ControlType = optionValue.ToString(); break;
                        case "GridFormat": columnDefinition.GridFormat = optionValue.ToString(); break;
                        case "EditorFormat": columnDefinition.EditorFormat = optionValue.ToString(); break;
                        case "ExportFormat": columnDefinition.ExportFormat = optionValue.ToString(); break;
                        case "Aggregatable": columnDefinition.Aggregatable = optionValue.ToBool(); break;
                        case "Computable": columnDefinition.Computable = optionValue.ToBool(); break;
                        case "ChoicesText": columnDefinition.ChoicesText = optionValue.ToString(); break;
                        case "UseSearch": columnDefinition.UseSearch = optionValue.ToBool(); break;
                        case "DefaultInput": columnDefinition.DefaultInput = optionValue.ToString(); break;
                        case "Own": columnDefinition.Own = optionValue.ToBool(); break;
                        case "FormName": columnDefinition.FormName = optionValue.ToString(); break;
                        case "ValidateRequired": columnDefinition.ValidateRequired = optionValue.ToBool(); break;
                        case "ValidateNumber": columnDefinition.ValidateNumber = optionValue.ToBool(); break;
                        case "ValidateDate": columnDefinition.ValidateDate = optionValue.ToBool(); break;
                        case "ValidateEmail": columnDefinition.ValidateEmail = optionValue.ToBool(); break;
                        case "ValidateEqualTo": columnDefinition.ValidateEqualTo = optionValue.ToString(); break;
                        case "DecimalPlaces": columnDefinition.DecimalPlaces = optionValue.ToInt(); break;
                        case "Min": columnDefinition.Min = optionValue.ToDecimal(); break;
                        case "Max": columnDefinition.Max = optionValue.ToDecimal(); break;
                        case "Step": columnDefinition.Step = optionValue.ToDecimal(); break;
                        case "StringFormat": columnDefinition.StringFormat = optionValue.ToString(); break;
                        case "Unit": columnDefinition.Unit = optionValue.ToString(); break;
                        case "NumFilterMin": columnDefinition.NumFilterMin = optionValue.ToDecimal(); break;
                        case "NumFilterMax": columnDefinition.NumFilterMax = optionValue.ToDecimal(); break;
                        case "NumFilterStep": columnDefinition.NumFilterStep = optionValue.ToDecimal(); break;
                        case "Width": columnDefinition.Width = optionValue.ToInt(); break;
                        case "SettingEnable": columnDefinition.SettingEnable = optionValue.ToBool(); break;
                        case "OldColumnName": columnDefinition.OldColumnName = optionValue.ToString(); break;
                    }
                });
        }

        public static void SetCssDefinitionOption(
            string placeholder, CssDefinition cssDefinition)
        {
            placeholder.RegexFirst("(?<=\\().+(?=\\))").Split(',')
                .Where(o => !o.IsNullOrEmpty()).ForEach(option =>
                {
                    var optionName = option.Split_1st('=').Trim();
                    var optionValue = option.Split_2nd('=').Trim();
                    switch (optionName)
                    {
                        case "Id": cssDefinition.Id = optionValue.ToString(); break;
                        case "Specific": cssDefinition.Specific = optionValue.ToString(); break;
                        case "width": cssDefinition.width = optionValue.ToString(); break;
                        case "min-width": cssDefinition.min_width = optionValue.ToString(); break;
                        case "max-width": cssDefinition.max_width = optionValue.ToString(); break;
                        case "height": cssDefinition.height = optionValue.ToString(); break;
                        case "min-height": cssDefinition.min_height = optionValue.ToString(); break;
                        case "max-height": cssDefinition.max_height = optionValue.ToString(); break;
                        case "display": cssDefinition.display = optionValue.ToString(); break;
                        case "float": cssDefinition._float = optionValue.ToString(); break;
                        case "margin": cssDefinition.margin = optionValue.ToString(); break;
                        case "margin-left": cssDefinition.margin_left = optionValue.ToString(); break;
                        case "margin-right": cssDefinition.margin_right = optionValue.ToString(); break;
                        case "padding": cssDefinition.padding = optionValue.ToString(); break;
                        case "padding-bottom": cssDefinition.padding_bottom = optionValue.ToString(); break;
                        case "text-align": cssDefinition.text_align = optionValue.ToString(); break;
                        case "vertical-align": cssDefinition.vertical_align = optionValue.ToString(); break;
                        case "line-height": cssDefinition.line_height = optionValue.ToString(); break;
                        case "font-size": cssDefinition.font_size = optionValue.ToString(); break;
                        case "font-family": cssDefinition.font_family = optionValue.ToString(); break;
                        case "font-weight": cssDefinition.font_weight = optionValue.ToString(); break;
                        case "font-style": cssDefinition.font_style = optionValue.ToString(); break;
                        case "color": cssDefinition.color = optionValue.ToString(); break;
                        case "background": cssDefinition.background = optionValue.ToString(); break;
                        case "background-color": cssDefinition.background_color = optionValue.ToString(); break;
                        case "border": cssDefinition.border = optionValue.ToString(); break;
                        case "border-top": cssDefinition.border_top = optionValue.ToString(); break;
                        case "border-bottom": cssDefinition.border_bottom = optionValue.ToString(); break;
                        case "border-left": cssDefinition.border_left = optionValue.ToString(); break;
                        case "border-right": cssDefinition.border_right = optionValue.ToString(); break;
                        case "border-collapse": cssDefinition.border_collapse = optionValue.ToString(); break;
                        case "border-spacing": cssDefinition.border_spacing = optionValue.ToString(); break;
                        case "position": cssDefinition.position = optionValue.ToString(); break;
                        case "top": cssDefinition.top = optionValue.ToString(); break;
                        case "right": cssDefinition.right = optionValue.ToString(); break;
                        case "left": cssDefinition.left = optionValue.ToString(); break;
                        case "bottom": cssDefinition.bottom = optionValue.ToString(); break;
                        case "cursor": cssDefinition.cursor = optionValue.ToString(); break;
                        case "clear": cssDefinition.clear = optionValue.ToString(); break;
                        case "overflow": cssDefinition.overflow = optionValue.ToString(); break;
                        case "word-wrap": cssDefinition.word_wrap = optionValue.ToString(); break;
                        case "word-break": cssDefinition.word_break = optionValue.ToString(); break;
                        case "white-space": cssDefinition.white_space = optionValue.ToString(); break;
                        case "table-layout": cssDefinition.table_layout = optionValue.ToString(); break;
                        case "text-decoration": cssDefinition.text_decoration = optionValue.ToString(); break;
                        case "list-style-type": cssDefinition.list_style_type = optionValue.ToString(); break;
                        case "visibility": cssDefinition.visibility = optionValue.ToString(); break;
                        case "border-radius": cssDefinition.border_radius = optionValue.ToString(); break;
                        case "z-index": cssDefinition.z_index = optionValue.ToString(); break;
                        case "fill": cssDefinition.fill = optionValue.ToString(); break;
                        case "fill-opacity": cssDefinition.fill_opacity = optionValue.ToString(); break;
                        case "stroke": cssDefinition.stroke = optionValue.ToString(); break;
                        case "stroke-width": cssDefinition.stroke_width = optionValue.ToString(); break;
                        case "text-anchor": cssDefinition.text_anchor = optionValue.ToString(); break;
                        case "shape-rendering": cssDefinition.shape_rendering = optionValue.ToString(); break;
                    }
                });
        }

        public static void SetDemoDefinitionOption(
            string placeholder, DemoDefinition demoDefinition)
        {
            placeholder.RegexFirst("(?<=\\().+(?=\\))").Split(',')
                .Where(o => !o.IsNullOrEmpty()).ForEach(option =>
                {
                    var optionName = option.Split_1st('=').Trim();
                    var optionValue = option.Split_2nd('=').Trim();
                    switch (optionName)
                    {
                        case "Id": demoDefinition.Id = optionValue.ToString(); break;
                        case "Body": demoDefinition.Body = optionValue.ToString(); break;
                        case "Type": demoDefinition.Type = optionValue.ToString(); break;
                        case "ParentId": demoDefinition.ParentId = optionValue.ToString(); break;
                        case "Title": demoDefinition.Title = optionValue.ToString(); break;
                        case "WorkValue": demoDefinition.WorkValue = optionValue.ToDecimal(); break;
                        case "ProgressRate": demoDefinition.ProgressRate = optionValue.ToDecimal(); break;
                        case "Status": demoDefinition.Status = optionValue.ToInt(); break;
                        case "ClassA": demoDefinition.ClassA = optionValue.ToString(); break;
                        case "ClassB": demoDefinition.ClassB = optionValue.ToString(); break;
                        case "ClassC": demoDefinition.ClassC = optionValue.ToString(); break;
                        case "Manager": demoDefinition.Manager = optionValue.ToString(); break;
                        case "Owner": demoDefinition.Owner = optionValue.ToString(); break;
                        case "Creator": demoDefinition.Creator = optionValue.ToString(); break;
                        case "Updator": demoDefinition.Updator = optionValue.ToString(); break;
                        case "StartTime": demoDefinition.StartTime = optionValue.ToDateTime(); break;
                        case "CompletionTime": demoDefinition.CompletionTime = optionValue.ToDateTime(); break;
                        case "CreatedTime": demoDefinition.CreatedTime = optionValue.ToDateTime(); break;
                        case "UpdatedTime": demoDefinition.UpdatedTime = optionValue.ToDateTime(); break;
                    }
                });
        }

        public static void SetSqlDefinitionOption(
            string placeholder, SqlDefinition sqlDefinition)
        {
            placeholder.RegexFirst("(?<=\\().+(?=\\))").Split(',')
                .Where(o => !o.IsNullOrEmpty()).ForEach(option =>
                {
                    var optionName = option.Split_1st('=').Trim();
                    var optionValue = option.Split_2nd('=').Trim();
                    switch (optionName)
                    {
                        case "Id": sqlDefinition.Id = optionValue.ToString(); break;
                        case "Body": sqlDefinition.Body = optionValue.ToString(); break;
                    }
                });
        }

        public static void SetViewModeDefinitionOption(
            string placeholder, ViewModeDefinition viewModeDefinition)
        {
            placeholder.RegexFirst("(?<=\\().+(?=\\))").Split(',')
                .Where(o => !o.IsNullOrEmpty()).ForEach(option =>
                {
                    var optionName = option.Split_1st('=').Trim();
                    var optionValue = option.Split_2nd('=').Trim();
                    switch (optionName)
                    {
                        case "Id": viewModeDefinition.Id = optionValue.ToString(); break;
                        case "ReferenceType": viewModeDefinition.ReferenceType = optionValue.ToString(); break;
                        case "Name": viewModeDefinition.Name = optionValue.ToString(); break;
                    }
                });
        }
    }

    public class CodeDefinition
    {
        public string Id; public string SavedId;
        public string Body; public string SavedBody;
        public string OutputPath; public string SavedOutputPath;
        public bool MergeToExisting; public bool SavedMergeToExisting;
        public string Source; public string SavedSource;
        public string RepeatType; public string SavedRepeatType;
        public int Indent; public int SavedIndent;
        public string Separator; public string SavedSeparator;
        public string Order; public string SavedOrder;
        public bool Pk; public bool SavedPk;
        public bool NotPk; public bool SavedNotPk;
        public bool Identity; public bool SavedIdentity;
        public bool NotIdentity; public bool SavedNotIdentity;
        public bool IdentityOrPk; public bool SavedIdentityOrPk;
        public bool Unique; public bool SavedUnique;
        public bool NotUnique; public bool SavedNotUnique;
        public bool NotDefault; public bool SavedNotDefault;
        public bool Like; public bool SavedLike;
        public bool HasIdentity; public bool SavedHasIdentity;
        public bool HasNotIdentity; public bool SavedHasNotIdentity;
        public bool HasTableNameId; public bool SavedHasTableNameId;
        public bool HasNotTableNameId; public bool SavedHasNotTableNameId;
        public bool ItemId; public bool SavedItemId;
        public bool NotItemId; public bool SavedNotItemId;
        public bool Calc; public bool SavedCalc;
        public bool NotCalc; public bool SavedNotCalc;
        public bool SearchIndex; public bool SavedSearchIndex;
        public bool NotByForm; public bool SavedNotByForm;
        public bool Form; public bool SavedForm;
        public bool Select; public bool SavedSelect;
        public bool Update; public bool SavedUpdate;
        public bool SelectColumns; public bool SavedSelectColumns;
        public bool NotSelectColumn; public bool SavedNotSelectColumn;
        public bool ComputeColumn; public bool SavedComputeColumn;
        public bool NotComputeColumn; public bool SavedNotComputeColumn;
        public bool Aggregatable; public bool SavedAggregatable;
        public bool Computable; public bool SavedComputable;
        public bool Join; public bool SavedJoin;
        public bool NotJoin; public bool SavedNotJoin;
        public bool JoinExpression; public bool SavedJoinExpression;
        public bool NotTypeCs; public bool SavedNotTypeCs;
        public bool ItemOnly; public bool SavedItemOnly;
        public bool NotItem; public bool SavedNotItem;
        public bool GenericUi; public bool SavedGenericUi;
        public bool Session; public bool SavedSession;
        public bool GridColumn; public bool SavedGridColumn;
        public bool FilterColumn; public bool SavedFilterColumn;
        public bool EditorColumn; public bool SavedEditorColumn;
        public bool TitleColumn; public bool SavedTitleColumn;
        public bool UserColumn; public bool SavedUserColumn;
        public bool EnumColumn; public bool SavedEnumColumn;
        public string Include; public string SavedInclude;
        public string Exclude; public string SavedExclude;
        public string IncludeTypeName; public string SavedIncludeTypeName;
        public string ExcludeTypeName; public string SavedExcludeTypeName;
        public string IncludeDefaultCs; public string SavedIncludeDefaultCs;
        public string ExcludeDefaultCs; public string SavedExcludeDefaultCs;
        public bool History; public bool SavedHistory;
        public bool PkHistory; public bool SavedPkHistory;
        public string ControlType; public string SavedControlType;
        public string ReplaceOld; public string SavedReplaceOld;
        public string ReplaceNew; public string SavedReplaceNew;
        public bool NotWhereSpecial; public bool SavedNotWhereSpecial;
        public bool NoSpace; public bool SavedNoSpace;
        public bool NotBase; public bool SavedNotBase;
        public bool NotNull; public bool SavedNotNull;
        public bool Validators; public bool SavedValidators;
        public string DisplayType; public string SavedDisplayType;
        public bool DisplayLanguages; public bool SavedDisplayLanguages;
        public bool ClientScript; public bool SavedClientScript;

        public CodeDefinition()
        {
        }

        public CodeDefinition(Dictionary<string, string> propertyCollection)
        {
            if (propertyCollection.ContainsKey("Id")) Id = propertyCollection["Id"].ToString(); else Id = string.Empty;
            if (propertyCollection.ContainsKey("Body")) Body = propertyCollection["Body"].ToString(); else Body = string.Empty;
            if (propertyCollection.ContainsKey("OutputPath")) OutputPath = propertyCollection["OutputPath"].ToString(); else OutputPath = string.Empty;
            if (propertyCollection.ContainsKey("MergeToExisting")) MergeToExisting = propertyCollection["MergeToExisting"].ToBool(); else MergeToExisting = false;
            if (propertyCollection.ContainsKey("Source")) Source = propertyCollection["Source"].ToString(); else Source = string.Empty;
            if (propertyCollection.ContainsKey("RepeatType")) RepeatType = propertyCollection["RepeatType"].ToString(); else RepeatType = string.Empty;
            if (propertyCollection.ContainsKey("Indent")) Indent = propertyCollection["Indent"].ToInt(); else Indent = 0;
            if (propertyCollection.ContainsKey("Separator")) Separator = propertyCollection["Separator"].ToString(); else Separator = string.Empty;
            if (propertyCollection.ContainsKey("Order")) Order = propertyCollection["Order"].ToString(); else Order = string.Empty;
            if (propertyCollection.ContainsKey("Pk")) Pk = propertyCollection["Pk"].ToBool(); else Pk = false;
            if (propertyCollection.ContainsKey("NotPk")) NotPk = propertyCollection["NotPk"].ToBool(); else NotPk = false;
            if (propertyCollection.ContainsKey("Identity")) Identity = propertyCollection["Identity"].ToBool(); else Identity = false;
            if (propertyCollection.ContainsKey("NotIdentity")) NotIdentity = propertyCollection["NotIdentity"].ToBool(); else NotIdentity = false;
            if (propertyCollection.ContainsKey("IdentityOrPk")) IdentityOrPk = propertyCollection["IdentityOrPk"].ToBool(); else IdentityOrPk = false;
            if (propertyCollection.ContainsKey("Unique")) Unique = propertyCollection["Unique"].ToBool(); else Unique = false;
            if (propertyCollection.ContainsKey("NotUnique")) NotUnique = propertyCollection["NotUnique"].ToBool(); else NotUnique = false;
            if (propertyCollection.ContainsKey("NotDefault")) NotDefault = propertyCollection["NotDefault"].ToBool(); else NotDefault = false;
            if (propertyCollection.ContainsKey("Like")) Like = propertyCollection["Like"].ToBool(); else Like = false;
            if (propertyCollection.ContainsKey("HasIdentity")) HasIdentity = propertyCollection["HasIdentity"].ToBool(); else HasIdentity = false;
            if (propertyCollection.ContainsKey("HasNotIdentity")) HasNotIdentity = propertyCollection["HasNotIdentity"].ToBool(); else HasNotIdentity = false;
            if (propertyCollection.ContainsKey("HasTableNameId")) HasTableNameId = propertyCollection["HasTableNameId"].ToBool(); else HasTableNameId = false;
            if (propertyCollection.ContainsKey("HasNotTableNameId")) HasNotTableNameId = propertyCollection["HasNotTableNameId"].ToBool(); else HasNotTableNameId = false;
            if (propertyCollection.ContainsKey("ItemId")) ItemId = propertyCollection["ItemId"].ToBool(); else ItemId = false;
            if (propertyCollection.ContainsKey("NotItemId")) NotItemId = propertyCollection["NotItemId"].ToBool(); else NotItemId = false;
            if (propertyCollection.ContainsKey("Calc")) Calc = propertyCollection["Calc"].ToBool(); else Calc = false;
            if (propertyCollection.ContainsKey("NotCalc")) NotCalc = propertyCollection["NotCalc"].ToBool(); else NotCalc = false;
            if (propertyCollection.ContainsKey("SearchIndex")) SearchIndex = propertyCollection["SearchIndex"].ToBool(); else SearchIndex = false;
            if (propertyCollection.ContainsKey("NotByForm")) NotByForm = propertyCollection["NotByForm"].ToBool(); else NotByForm = false;
            if (propertyCollection.ContainsKey("Form")) Form = propertyCollection["Form"].ToBool(); else Form = false;
            if (propertyCollection.ContainsKey("Select")) Select = propertyCollection["Select"].ToBool(); else Select = false;
            if (propertyCollection.ContainsKey("Update")) Update = propertyCollection["Update"].ToBool(); else Update = false;
            if (propertyCollection.ContainsKey("SelectColumns")) SelectColumns = propertyCollection["SelectColumns"].ToBool(); else SelectColumns = false;
            if (propertyCollection.ContainsKey("NotSelectColumn")) NotSelectColumn = propertyCollection["NotSelectColumn"].ToBool(); else NotSelectColumn = false;
            if (propertyCollection.ContainsKey("ComputeColumn")) ComputeColumn = propertyCollection["ComputeColumn"].ToBool(); else ComputeColumn = false;
            if (propertyCollection.ContainsKey("NotComputeColumn")) NotComputeColumn = propertyCollection["NotComputeColumn"].ToBool(); else NotComputeColumn = false;
            if (propertyCollection.ContainsKey("Aggregatable")) Aggregatable = propertyCollection["Aggregatable"].ToBool(); else Aggregatable = false;
            if (propertyCollection.ContainsKey("Computable")) Computable = propertyCollection["Computable"].ToBool(); else Computable = false;
            if (propertyCollection.ContainsKey("Join")) Join = propertyCollection["Join"].ToBool(); else Join = false;
            if (propertyCollection.ContainsKey("NotJoin")) NotJoin = propertyCollection["NotJoin"].ToBool(); else NotJoin = false;
            if (propertyCollection.ContainsKey("JoinExpression")) JoinExpression = propertyCollection["JoinExpression"].ToBool(); else JoinExpression = false;
            if (propertyCollection.ContainsKey("NotTypeCs")) NotTypeCs = propertyCollection["NotTypeCs"].ToBool(); else NotTypeCs = false;
            if (propertyCollection.ContainsKey("ItemOnly")) ItemOnly = propertyCollection["ItemOnly"].ToBool(); else ItemOnly = false;
            if (propertyCollection.ContainsKey("NotItem")) NotItem = propertyCollection["NotItem"].ToBool(); else NotItem = false;
            if (propertyCollection.ContainsKey("GenericUi")) GenericUi = propertyCollection["GenericUi"].ToBool(); else GenericUi = false;
            if (propertyCollection.ContainsKey("Session")) Session = propertyCollection["Session"].ToBool(); else Session = false;
            if (propertyCollection.ContainsKey("GridColumn")) GridColumn = propertyCollection["GridColumn"].ToBool(); else GridColumn = false;
            if (propertyCollection.ContainsKey("FilterColumn")) FilterColumn = propertyCollection["FilterColumn"].ToBool(); else FilterColumn = false;
            if (propertyCollection.ContainsKey("EditorColumn")) EditorColumn = propertyCollection["EditorColumn"].ToBool(); else EditorColumn = false;
            if (propertyCollection.ContainsKey("TitleColumn")) TitleColumn = propertyCollection["TitleColumn"].ToBool(); else TitleColumn = false;
            if (propertyCollection.ContainsKey("UserColumn")) UserColumn = propertyCollection["UserColumn"].ToBool(); else UserColumn = false;
            if (propertyCollection.ContainsKey("EnumColumn")) EnumColumn = propertyCollection["EnumColumn"].ToBool(); else EnumColumn = false;
            if (propertyCollection.ContainsKey("Include")) Include = propertyCollection["Include"].ToString(); else Include = string.Empty;
            if (propertyCollection.ContainsKey("Exclude")) Exclude = propertyCollection["Exclude"].ToString(); else Exclude = string.Empty;
            if (propertyCollection.ContainsKey("IncludeTypeName")) IncludeTypeName = propertyCollection["IncludeTypeName"].ToString(); else IncludeTypeName = string.Empty;
            if (propertyCollection.ContainsKey("ExcludeTypeName")) ExcludeTypeName = propertyCollection["ExcludeTypeName"].ToString(); else ExcludeTypeName = string.Empty;
            if (propertyCollection.ContainsKey("IncludeDefaultCs")) IncludeDefaultCs = propertyCollection["IncludeDefaultCs"].ToString(); else IncludeDefaultCs = string.Empty;
            if (propertyCollection.ContainsKey("ExcludeDefaultCs")) ExcludeDefaultCs = propertyCollection["ExcludeDefaultCs"].ToString(); else ExcludeDefaultCs = string.Empty;
            if (propertyCollection.ContainsKey("History")) History = propertyCollection["History"].ToBool(); else History = false;
            if (propertyCollection.ContainsKey("PkHistory")) PkHistory = propertyCollection["PkHistory"].ToBool(); else PkHistory = false;
            if (propertyCollection.ContainsKey("ControlType")) ControlType = propertyCollection["ControlType"].ToString(); else ControlType = string.Empty;
            if (propertyCollection.ContainsKey("ReplaceOld")) ReplaceOld = propertyCollection["ReplaceOld"].ToString(); else ReplaceOld = string.Empty;
            if (propertyCollection.ContainsKey("ReplaceNew")) ReplaceNew = propertyCollection["ReplaceNew"].ToString(); else ReplaceNew = string.Empty;
            if (propertyCollection.ContainsKey("NotWhereSpecial")) NotWhereSpecial = propertyCollection["NotWhereSpecial"].ToBool(); else NotWhereSpecial = false;
            if (propertyCollection.ContainsKey("NoSpace")) NoSpace = propertyCollection["NoSpace"].ToBool(); else NoSpace = false;
            if (propertyCollection.ContainsKey("NotBase")) NotBase = propertyCollection["NotBase"].ToBool(); else NotBase = false;
            if (propertyCollection.ContainsKey("NotNull")) NotNull = propertyCollection["NotNull"].ToBool(); else NotNull = false;
            if (propertyCollection.ContainsKey("Validators")) Validators = propertyCollection["Validators"].ToBool(); else Validators = false;
            if (propertyCollection.ContainsKey("DisplayType")) DisplayType = propertyCollection["DisplayType"].ToString(); else DisplayType = string.Empty;
            if (propertyCollection.ContainsKey("DisplayLanguages")) DisplayLanguages = propertyCollection["DisplayLanguages"].ToBool(); else DisplayLanguages = false;
            if (propertyCollection.ContainsKey("ClientScript")) ClientScript = propertyCollection["ClientScript"].ToBool(); else ClientScript = false;
        }

        public object this[string key]
        {
            get{
                switch(key)
                {
                    case "Id": return Id;
                    case "Body": return Body;
                    case "OutputPath": return OutputPath;
                    case "MergeToExisting": return MergeToExisting;
                    case "Source": return Source;
                    case "RepeatType": return RepeatType;
                    case "Indent": return Indent;
                    case "Separator": return Separator;
                    case "Order": return Order;
                    case "Pk": return Pk;
                    case "NotPk": return NotPk;
                    case "Identity": return Identity;
                    case "NotIdentity": return NotIdentity;
                    case "IdentityOrPk": return IdentityOrPk;
                    case "Unique": return Unique;
                    case "NotUnique": return NotUnique;
                    case "NotDefault": return NotDefault;
                    case "Like": return Like;
                    case "HasIdentity": return HasIdentity;
                    case "HasNotIdentity": return HasNotIdentity;
                    case "HasTableNameId": return HasTableNameId;
                    case "HasNotTableNameId": return HasNotTableNameId;
                    case "ItemId": return ItemId;
                    case "NotItemId": return NotItemId;
                    case "Calc": return Calc;
                    case "NotCalc": return NotCalc;
                    case "SearchIndex": return SearchIndex;
                    case "NotByForm": return NotByForm;
                    case "Form": return Form;
                    case "Select": return Select;
                    case "Update": return Update;
                    case "SelectColumns": return SelectColumns;
                    case "NotSelectColumn": return NotSelectColumn;
                    case "ComputeColumn": return ComputeColumn;
                    case "NotComputeColumn": return NotComputeColumn;
                    case "Aggregatable": return Aggregatable;
                    case "Computable": return Computable;
                    case "Join": return Join;
                    case "NotJoin": return NotJoin;
                    case "JoinExpression": return JoinExpression;
                    case "NotTypeCs": return NotTypeCs;
                    case "ItemOnly": return ItemOnly;
                    case "NotItem": return NotItem;
                    case "GenericUi": return GenericUi;
                    case "Session": return Session;
                    case "GridColumn": return GridColumn;
                    case "FilterColumn": return FilterColumn;
                    case "EditorColumn": return EditorColumn;
                    case "TitleColumn": return TitleColumn;
                    case "UserColumn": return UserColumn;
                    case "EnumColumn": return EnumColumn;
                    case "Include": return Include;
                    case "Exclude": return Exclude;
                    case "IncludeTypeName": return IncludeTypeName;
                    case "ExcludeTypeName": return ExcludeTypeName;
                    case "IncludeDefaultCs": return IncludeDefaultCs;
                    case "ExcludeDefaultCs": return ExcludeDefaultCs;
                    case "History": return History;
                    case "PkHistory": return PkHistory;
                    case "ControlType": return ControlType;
                    case "ReplaceOld": return ReplaceOld;
                    case "ReplaceNew": return ReplaceNew;
                    case "NotWhereSpecial": return NotWhereSpecial;
                    case "NoSpace": return NoSpace;
                    case "NotBase": return NotBase;
                    case "NotNull": return NotNull;
                    case "Validators": return Validators;
                    case "DisplayType": return DisplayType;
                    case "DisplayLanguages": return DisplayLanguages;
                    case "ClientScript": return ClientScript;
                    default: return null;
                }
            }
        }

        public void RestoreBySavedMemory()
        {
            Id = SavedId;
            Body = SavedBody;
            OutputPath = SavedOutputPath;
            MergeToExisting = SavedMergeToExisting;
            Source = SavedSource;
            RepeatType = SavedRepeatType;
            Indent = SavedIndent;
            Separator = SavedSeparator;
            Order = SavedOrder;
            Pk = SavedPk;
            NotPk = SavedNotPk;
            Identity = SavedIdentity;
            NotIdentity = SavedNotIdentity;
            IdentityOrPk = SavedIdentityOrPk;
            Unique = SavedUnique;
            NotUnique = SavedNotUnique;
            NotDefault = SavedNotDefault;
            Like = SavedLike;
            HasIdentity = SavedHasIdentity;
            HasNotIdentity = SavedHasNotIdentity;
            HasTableNameId = SavedHasTableNameId;
            HasNotTableNameId = SavedHasNotTableNameId;
            ItemId = SavedItemId;
            NotItemId = SavedNotItemId;
            Calc = SavedCalc;
            NotCalc = SavedNotCalc;
            SearchIndex = SavedSearchIndex;
            NotByForm = SavedNotByForm;
            Form = SavedForm;
            Select = SavedSelect;
            Update = SavedUpdate;
            SelectColumns = SavedSelectColumns;
            NotSelectColumn = SavedNotSelectColumn;
            ComputeColumn = SavedComputeColumn;
            NotComputeColumn = SavedNotComputeColumn;
            Aggregatable = SavedAggregatable;
            Computable = SavedComputable;
            Join = SavedJoin;
            NotJoin = SavedNotJoin;
            JoinExpression = SavedJoinExpression;
            NotTypeCs = SavedNotTypeCs;
            ItemOnly = SavedItemOnly;
            NotItem = SavedNotItem;
            GenericUi = SavedGenericUi;
            Session = SavedSession;
            GridColumn = SavedGridColumn;
            FilterColumn = SavedFilterColumn;
            EditorColumn = SavedEditorColumn;
            TitleColumn = SavedTitleColumn;
            UserColumn = SavedUserColumn;
            EnumColumn = SavedEnumColumn;
            Include = SavedInclude;
            Exclude = SavedExclude;
            IncludeTypeName = SavedIncludeTypeName;
            ExcludeTypeName = SavedExcludeTypeName;
            IncludeDefaultCs = SavedIncludeDefaultCs;
            ExcludeDefaultCs = SavedExcludeDefaultCs;
            History = SavedHistory;
            PkHistory = SavedPkHistory;
            ControlType = SavedControlType;
            ReplaceOld = SavedReplaceOld;
            ReplaceNew = SavedReplaceNew;
            NotWhereSpecial = SavedNotWhereSpecial;
            NoSpace = SavedNoSpace;
            NotBase = SavedNotBase;
            NotNull = SavedNotNull;
            Validators = SavedValidators;
            DisplayType = SavedDisplayType;
            DisplayLanguages = SavedDisplayLanguages;
            ClientScript = SavedClientScript;
        }
    }

    public class CodeColumn2nd
    {
        public string Initializer;
        public string Def_SetDefinitions;
        public string Def;
        public string Def_FileDefinition;
        public string Def_FileDefinition_SetColumn2ndAndTable;
        public string Def_FileDefinition_NoSpace;
        public string Def_FileDefinition_SetDefinition;
        public string Def_FileDefinition_SetTable;
        public string Def_SetDefinitionOption;
        public string Def_SetDefinitionOption_Cases;
        public string Def_DefinitionClass;
        public string Def_DefinitionClass_Definition;
        public string Def_DefinitionClass_SetProperties;
        public string Def_DefinitionClass_RestoreBySavedMemory;
        public string Def_DefinitionClass_DefinitionAccessor;
        public string Def_DefinitionClass_DefinitionColumn2nd;
        public string Def_DefinitionClass_DefinitionTable;
        public string Controller;
        public string Controller_Groups;
        public string Base;
        public string Base_Property;
        public string Base_PropertyCalc;
        public string Base_SavedProperty;
        public string Base_PropertyUpdated;
        public string Base_PropertyUpdated_NotNull;
        public string Model;
        public string Model_InheritBase;
        public string Model_InheritBaseItem;
        public string Model_IConvertable;
        public string Model_ItemProperties;
        public string Model_ItemProperties_Sites;
        public string Model_UrlId;
        public string Model_SessionProperties;
        public string Model_PropertyValue;
        public string Model_PropertyValue_ColumnCases;
        public string Model_PropertyValue_ColumnCases_ToString;
        public string Model_SwitchTargets;
        public string Model_Constructor_Items;
        public string Model_Constructor;
        public string Model_IdentityParameters;
        public string Model_SetSiteSettingsNew;
        public string Model_SetUserId;
        public string Model_SetIdentity;
        public string Model_SetTitleDisplayValue;
        public string Model_ClearSessions;
        public string Model_Get;
        public string Model_GetDefaultColumns;
        public string Model_GetSitesDefaultColumns;
        public string Model_GetItemsDefaultColumns;
        public string Model_SetSiteSettingsProperties;
        public string Model_SetTenantId;
        public string Model_SearchIndexHash;
        public string Model_SearchIndexHashColumnCases;
        public string Model_Create;
        public string Model_CreateParams;
        public string Model_OnCreating_Binaries;
        public string Model_OnCreating_Users;
        public string Model_CheckNotificationConditions;
        public string Model_CreatedNotice;
        public string Model_InsertItems;
        public string Model_InsertItemsAfter;
        public string Model_SelectIdentity;
        public string Model_InsertGroupMember;
        public string Model_InsertLinksByCreate;
        public string Model_Insert;
        public string Model_InsertIdentity;
        public string Model_InsertIdentitySet;
        public string Model_Update;
        public string Model_UpdateParams;
        public string Model_Update_Sites;
        public string Model_UpdateRelatedRecords;
        public string Model_OnUpdatedNotice;
        public string Model_OnUpdated_Depts;
        public string Model_OnUpdated_Groups;
        public string Model_OnUpdated_Users;
        public string Model_OnUpdated_SetSiteMenu;
        public string Model_OnUpdated_OutgoingMails;
        public string Model_SynchronizeSummaryExecute;
        public string Model_SynchronizeSummary;
        public string Model_SynchronizeSummaryColumnCases;
        public string Model_UpdateFormulaColumns;
        public string Model_UpdateFormulaColumns_ColumnCases;
        public string Model_UpdateOrCreate;
        public string Model_UpdateRelatedRecordsMethod;
        public string Model_InsertLinksByUpdate;
        public string Model_InsertLinksByUpdate_Site;
        public string Model_UpdateSites_Wikis;
        public string Model_InsertLinks;
        public string Model_InsertLinksCases;
        public string Model_Move;
        public string Model_Delete;
        public string Model_DeleteParams;
        public string Model_Delete_Item;
        public string Model_Delete_GroupMembers;
        public string Model_Delete_Sites;
        public string Model_Delete_SitesItems;
        public string Model_OnDeleted_SetSiteInfo;
        public string Model_OnDeletedNotice;
        public string Model_Restore;
        public string Model_Restore_Item;
        public string Model_PhysicalDelete;
        public string Model_SetByForm;
        public string Model_SetByForm_ColumnCases;
        public string Model_SetByForm_SetByFormula;
        public string Model_ToUniversal;
        public string Model_SetByForm_Files;
        public string Model_SetByForm_Site;
        public string Model_CreateIndexes;
        public string Model_AddSqlParamIdentity;
        public string Model_AddSqlParamPk;
        public string Model_AddSqlParam;
        public string Model_AddSqlParamPkHistory;
        public string Model_SetByFormula;
        public string Model_SetByFormula_Data;
        public string Model_SetByFormula_ColumnCases;
        public string Model_SetBySession;
        public string Model_SetPk;
        public string Model_Set;
        public string Model_PushState;
        public string Model_PushState_Item;
        public string Model_Matched;
        public string Model_Matched_Incomplete;
        public string Model_Matched_Own;
        public string Model_Matched_Issues;
        public string Model_Matched_ColumnCases;
        public string Model_Notice;
        public string Model_NoticeColumnCases;
        public string Model_SwitchItems;
        public string Model_IndexCases;
        public string Model_IndexJsonCases;
        public string Model_GanttCases;
        public string Model_GanttJsonCases;
        public string Model_BurnDownCases;
        public string Model_BurnDownJsonCases;
        public string Model_BurnDownRecordDetailsJsonCases;
        public string Model_TimeSeriesCases;
        public string Model_TimeSeriesJsonCases;
        public string Model_KambanCases;
        public string Model_KambanJsonCases;
        public string Model_NewCases;
        public string Model_EditorCases;
        public string Model_ImportCases;
        public string Model_ExportCases;
        public string Model_GridRowsCases;
        public string Model_CreateCases;
        public string Model_UpdateCases;
        public string Model_DeleteCommentCases;
        public string Model_CopyCases;
        public string Model_MoveCases;
        public string Model_BulkMoveCases;
        public string Model_DeleteCases;
        public string Model_BulkDeleteCases;
        public string Model_RestoreCases;
        public string Model_EditSeparateSettingsCases;
        public string Model_SeparateCases;
        public string Model_HistoriesCases;
        public string Model_HistoryCases;
        public string Model_EditorJsonCases;
        public string Model_GetCases;
        public string Model_UpdateByKambanCases;
        public string Model_ParentIdParameter;
        public string Model_InheritPermissionParameter;
        public string Model_SwitchTargetsParameter;
        public string Model_SetSiteId;
        public string Model_SetSiteSettings;
        public string Model_SetPermissionType;
        public string Model_SetParentId;
        public string Model_SetInheritPermission;
        public string Model_SetSwitchTargets;
        public string Model_SiteSettings;
        public string Model_SiteSettingsOnly;
        public string Model_SiteSettings_Sites;
        public string Model_SiteSettings_SitesOnly;
        public string Model_SiteSettingsWithParameterName;
        public string Model_SiteSettingsWithParameterNameLower;
        public string Model_SiteSettingsParam;
        public string Model_SiteSettingsParamOnly;
        public string Collection;
        public string Collection_SiteSettings;
        public string Collection_SiteSettingsArgument;
        public string Collection_Aggregation;
        public string Collection_ItemAggregation;
        public string Model_Utilities;
        public string Model_Utilities_Index;
        public string Model_Utilities_SetChoiceHash;
        public string Model_Utilities_DropDownSearchDialog;
        public string Model_Utilities_ImportSettings;
        public string Model_Utilities_GridRows_OnClick;
        public string Model_Utilities_GridRows_OnClickItem;
        public string Model_Utilities_TdValue;
        public string Model_Utilities_TdValueCases;
        public string Model_Utilities_TdValueCustomValueCases;
        public string Model_Utilities_SqlColumn_SiteId;
        public string Model_Utilities_GridSqlWhereTenantId;
        public string Model_Utilities_GridSqlWhereSiteId;
        public string Model_Utilities_Editor;
        public string Model_Utilities_SiteSettingsUtilities;
        public string Model_Utilities_EditorItem;
        public string Model_Utilities_SetSwitchTargets;
        public string Model_Utilities_FieldCases;
        public string Model_Utilities_FieldCases_Item;
        public string Model_Utilities_Links;
        public string Model_Utilities_EditorJson;
        public string Model_Utilities_EditorResponse;
        public string Model_Utilities_GetSwitchTargets;
        public string Model_Utilities_SqlWhereTenantId;
        public string Model_Utilities_SqlWhereSiteId;
        public string Model_Utilities_SiteId;
        public string Model_Utilities_SiteIdParam;
        public string Model_Utilities_FieldResponse;
        public string Model_Utilities_FieldResponse_ColumnCases;
        public string Model_Utilities_TableName;
        public string Model_Utilities_TableNameCases;
        public string Model_Utilities_Create;
        public string Model_Utilities_CreateParams;
        public string Model_Utilities_CreateParams_Sites;
        public string Model_Utilities_CreatedResponse;
        public string Model_Utilities_CreatedResponse_Items;
        public string Model_Utilities_CreatedResponse_Sites;
        public string Model_Utilities_Update;
        public string Model_Utilities_Update_NewModel;
        public string Model_Utilities_Update_NewModel_Sites;
        public string Model_Utilities_UpdateInvalid_Users;
        public string Model_Utilities_OnUpdated_Issues;
        public string Model_Utilities_OnUpdated_Breadcrumb;
        public string Model_Utilities_ResponseByUpdate_FieldResponse;
        public string Model_Utilities_Copy;
        public string Model_Utilities_CopyResponse;
        public string Model_Utilities_CopyResponse_Items;
        public string Model_Utilities_Copy_Sites;
        public string Model_Utilities_Move;
        public string Model_Utilities_Delete;
        public string Model_Utilities_RedirectAfterDelete;
        public string Model_Utilities_RedirectAfterDeleteNotItem;
        public string Model_Utilities_RedirectAfterDeleteItem;
        public string Model_Utilities_RedirectAfterDelete_Sites;
        public string Model_Utilities_RedirectAfterDelete_Wikis;
        public string Model_Utilities_Restore;
        public string Model_Utilities_Histories;
        public string Model_Utilities_HistoriesParams;
        public string Model_Utilities_HistoriesParams_Sites;
        public string Model_Utilities_HistoryParams;
        public string Model_Utilities_HistoryParams_Sites;
        public string Model_Utilities_TitleDisplay;
        public string Model_Utilities_ResponseLinks;
        public string Model_Utilities_SiteModel;
        public string Model_Utilities_Separate;
        public string Model_Utilities_Title;
        public string Model_Utilities_BulkMove;
        public string Model_Utilities_BulkDelete;
        public string Model_Utilities_Import;
        public string Model_Utilities_ImportCases;
        public string Model_Utilities_ImportValidatorHeaders;
        public string Model_Utilities_ImportValidatorCases;
        public string Model_Utilities_Export;
        public string Model_Utilities_CsvColumnCases;
        public string Model_Utilities_NotNull;
        public string Model_Utilities_TitleDisplayValue;
        public string Model_Utilities_TitleDisplayValueCases_Item;
        public string Model_Utilities_TitleDisplayValueCases_DataRow;
        public string Model_Utilities_UpdateByKamban;
        public string Model_Utilities_SetNoticeParam;
        public string Model_Utilities_SearchIndexes;
        public string Model_Utilities_SearchIndexes_TableCases;
        public string Model_Utilities_ItemTitle;
        public string Model_Validator;
        public string Model_ValidatorMethods;
        public string Model_Validator_OnMoving;
        public string Model_Validator_OnCreatingCases;
        public string Model_Validator_OnUpdating_Users;
        public string Model_Validator_OnUpdatingCases;
        public string Model_ValidatorMethods_Binaries;
        public string Rds;
        public string Rds_SqlStatement;
        public string Rds_SqlSelect;
        public string Rds_SqlExists;
        public string Rds_SqlInsert;
        public string Rds_SqlUpdate;
        public string Rds_SqlUpdateOrInsert;
        public string Rds_SqlDelete;
        public string Rds_SqlPhysicalDelete;
        public string Rds_SqlRestore;
        public string Rds_AggregationTableCases;
        public string Rds_Aggregation;
        public string Rds_AggregationTotalCases;
        public string Rds_AggregationAverageCases;
        public string Rds_AggregationGroupByCases;
        public string Rds_AggregationOverdue;
        public string Rds_SaveHistory;
        public string Rds_SaveHistory_Columns;
        public string Rds_SaveHistory_HistoryColumns;
        public string Rds_SaveHistory_WhereColumns;
        public string Rds_Delete;
        public string Rds_Delete_Columns;
        public string Rds_Delete_DeletedColumns;
        public string Rds_Delete_WhereColumns;
        public string Rds_Delete_WhereDeletedColumns;
        public string Rds_Restore;
        public string Rds_Restore_Columns;
        public string Rds_Restore_DeletedColumns;
        public string Rds_Restore_WhereColumns;
        public string Rds_Restore_WhereDeletedColumns;
        public string Rds_Restore_IdentityOn;
        public string Rds_Restore_IdentityOff;
        public string Rds_Columns;
        public string Rds_Columns_SqlClasses;
        public string Rds_Columns_ConstSqlWhereLike;
        public string Rds_Columns_ConstSqlWhereExists;
        public string Rds_Columns_ConstSqlWhereNotExists;
        public string Rds_Columns_SqlColumnCases;
        public string Rds_Columns_SqlColumn;
        public string Rds_Columns_SqlColumn_SelectColumns;
        public string Rds_Columns_SqlColumn_ComputeColumn;
        public string Rds_Columns_SqlWhere;
        public string Rds_Columns_SqlWhere_SelectColumns;
        public string Rds_Columns_SqlWhere_ComputeColumn;
        public string Rds_Columns_SqlWhere_In;
        public string Rds_Columns_SqlWhere_Between_Number;
        public string Rds_Columns_SqlWhere_Between_DateTime;
        public string Rds_Columns_SqlGroupByCases;
        public string Rds_Columns_SqlGroupBy;
        public string Rds_Columns_SqlHavingComputes;
        public string Rds_Columns_SqlOrderBy1;
        public string Rds_Columns_SqlOrderBy2;
        public string Rds_Columns_SqlParam_ItemId;
        public string Rds_Columns_SqlParam;
        public string Rds_Columns_SqlParam_Enum;
        public string Rds_Defaults;
        public string Rds_ColumnDefault;
        public string Rds_ItemEditorColumns;
        public string Rds_ColumnDefaultPk;
        public string Rds_JoinDefault;
        public string Rds_WherePk;
        public string Rds_Where_TenantId;
        public string Rds_Where_ItemId;
        public string Rds_WhereHistory;
        public string Rds_ParamDefault;
        public string Rds_ParamDefault_SetDefault;
        public string Rds_ParamDefault_ParamAll;
        public string Rds_ParamItemId;
        public string Rds_SiteId;
        public string Rds_TitleColumn;
        public string Rds_TitleColumnCases;
        public string Titles;
        public string Titles_TitleDisplayValueCases;
        public string Indexes;
        public string Indexes_TableCases;
        public string ItemsInitializer;
        public string ItemsInitializer_InitItems;
        public string SiteSettings;
        public string SiteSettings_GetCases;
        public string SiteSettings_GetByItemCases;
        public string SiteSettings_GetByReferenceCases;
        public string SiteSettings_GetByItemReferenceCases;
        public string SiteSettings_GetModels;
        public string SiteSettings_GetModels_Choices;
        public string SiteSettings_GetModels_Items;
        public string SiteSettings_GetModels_Items_Choices;
        public string SiteSettings_GetModels_Includes;
        public string SiteSettings_UpdateTitles;
        public string SiteSettings_UpdateTitles_TableCases;
        public string View;
        public string View_Search_TableCases;
        public string View_Sorter_TableCases;
        public string View_Sorter_ColumnCases;
        public string HtmlLinks;
        public string HtmlLinks_DataSetTableCases;
        public string HtmlLinks_SelectStatementTableCases;
        public string HtmlLinks_TableCases;
        public string HtmlLinks_Headers;
        public string HtmlLinks_Rows;
        public string HtmlLinks_RowColumns;
        public string Summaries;
        public string Summaries_SynchronizeCases;
        public string Summaries_SynchronizeTables;
        public string Summaries_ParamCases;
        public string Summaries_ZeroParamCases;
        public string Summaries_SelectCountTables;
        public string Summaries_SelectTotalTableCases;
        public string Summaries_SelectTotalColumns;
        public string Summaries_SelectTotalColumnCases;
        public string Summaries_SelectAverageTableCases;
        public string Summaries_SelectAverageColumns;
        public string Summaries_SelectAverageColumnCases;
        public string Summaries_SelectMinTableCases;
        public string Summaries_SelectMinColumns;
        public string Summaries_SelectMinColumnCases;
        public string Summaries_SelectMaxTableCases;
        public string Summaries_SelectMaxColumns;
        public string Summaries_SelectMaxColumnCases;
        public string Summaries_WhereTables;
        public string Summaries_WhereColumnCases;
        public string FormulaUtilities;
        public string FormulaUtilities_TableCases;
        public string FormulaUtilities_Updates;
        public string Messages;
        public string Messages_Parts;
        public string Messages_Resonses;
        public string ResponseCollection;
        public string ResponseCollection_Models;
        public string ResponseCollection_ValueModels;
        public string ResponseCollection_ValueColumns;
        public string Displays;
        public string Displays_Parts;
        public string Error;
        public string Error_Types;
        public string Error_Messages;
        public string Css;
        public string ClientDisplays;
        public string ClientDisplays_Parts;
        public string Comma;
    }

    public class CodeTable
    {
        public CodeDefinition Initializer = new CodeDefinition();
        public CodeDefinition Def_SetDefinitions = new CodeDefinition();
        public CodeDefinition Def = new CodeDefinition();
        public CodeDefinition Def_FileDefinition = new CodeDefinition();
        public CodeDefinition Def_FileDefinition_SetColumn2ndAndTable = new CodeDefinition();
        public CodeDefinition Def_FileDefinition_NoSpace = new CodeDefinition();
        public CodeDefinition Def_FileDefinition_SetDefinition = new CodeDefinition();
        public CodeDefinition Def_FileDefinition_SetTable = new CodeDefinition();
        public CodeDefinition Def_SetDefinitionOption = new CodeDefinition();
        public CodeDefinition Def_SetDefinitionOption_Cases = new CodeDefinition();
        public CodeDefinition Def_DefinitionClass = new CodeDefinition();
        public CodeDefinition Def_DefinitionClass_Definition = new CodeDefinition();
        public CodeDefinition Def_DefinitionClass_SetProperties = new CodeDefinition();
        public CodeDefinition Def_DefinitionClass_RestoreBySavedMemory = new CodeDefinition();
        public CodeDefinition Def_DefinitionClass_DefinitionAccessor = new CodeDefinition();
        public CodeDefinition Def_DefinitionClass_DefinitionColumn2nd = new CodeDefinition();
        public CodeDefinition Def_DefinitionClass_DefinitionTable = new CodeDefinition();
        public CodeDefinition Controller = new CodeDefinition();
        public CodeDefinition Controller_Groups = new CodeDefinition();
        public CodeDefinition Base = new CodeDefinition();
        public CodeDefinition Base_Property = new CodeDefinition();
        public CodeDefinition Base_PropertyCalc = new CodeDefinition();
        public CodeDefinition Base_SavedProperty = new CodeDefinition();
        public CodeDefinition Base_PropertyUpdated = new CodeDefinition();
        public CodeDefinition Base_PropertyUpdated_NotNull = new CodeDefinition();
        public CodeDefinition Model = new CodeDefinition();
        public CodeDefinition Model_InheritBase = new CodeDefinition();
        public CodeDefinition Model_InheritBaseItem = new CodeDefinition();
        public CodeDefinition Model_IConvertable = new CodeDefinition();
        public CodeDefinition Model_ItemProperties = new CodeDefinition();
        public CodeDefinition Model_ItemProperties_Sites = new CodeDefinition();
        public CodeDefinition Model_UrlId = new CodeDefinition();
        public CodeDefinition Model_SessionProperties = new CodeDefinition();
        public CodeDefinition Model_PropertyValue = new CodeDefinition();
        public CodeDefinition Model_PropertyValue_ColumnCases = new CodeDefinition();
        public CodeDefinition Model_PropertyValue_ColumnCases_ToString = new CodeDefinition();
        public CodeDefinition Model_SwitchTargets = new CodeDefinition();
        public CodeDefinition Model_Constructor_Items = new CodeDefinition();
        public CodeDefinition Model_Constructor = new CodeDefinition();
        public CodeDefinition Model_IdentityParameters = new CodeDefinition();
        public CodeDefinition Model_SetSiteSettingsNew = new CodeDefinition();
        public CodeDefinition Model_SetUserId = new CodeDefinition();
        public CodeDefinition Model_SetIdentity = new CodeDefinition();
        public CodeDefinition Model_SetTitleDisplayValue = new CodeDefinition();
        public CodeDefinition Model_ClearSessions = new CodeDefinition();
        public CodeDefinition Model_Get = new CodeDefinition();
        public CodeDefinition Model_GetDefaultColumns = new CodeDefinition();
        public CodeDefinition Model_GetSitesDefaultColumns = new CodeDefinition();
        public CodeDefinition Model_GetItemsDefaultColumns = new CodeDefinition();
        public CodeDefinition Model_SetSiteSettingsProperties = new CodeDefinition();
        public CodeDefinition Model_SetTenantId = new CodeDefinition();
        public CodeDefinition Model_SearchIndexHash = new CodeDefinition();
        public CodeDefinition Model_SearchIndexHashColumnCases = new CodeDefinition();
        public CodeDefinition Model_Create = new CodeDefinition();
        public CodeDefinition Model_CreateParams = new CodeDefinition();
        public CodeDefinition Model_OnCreating_Binaries = new CodeDefinition();
        public CodeDefinition Model_OnCreating_Users = new CodeDefinition();
        public CodeDefinition Model_CheckNotificationConditions = new CodeDefinition();
        public CodeDefinition Model_CreatedNotice = new CodeDefinition();
        public CodeDefinition Model_InsertItems = new CodeDefinition();
        public CodeDefinition Model_InsertItemsAfter = new CodeDefinition();
        public CodeDefinition Model_SelectIdentity = new CodeDefinition();
        public CodeDefinition Model_InsertGroupMember = new CodeDefinition();
        public CodeDefinition Model_InsertLinksByCreate = new CodeDefinition();
        public CodeDefinition Model_Insert = new CodeDefinition();
        public CodeDefinition Model_InsertIdentity = new CodeDefinition();
        public CodeDefinition Model_InsertIdentitySet = new CodeDefinition();
        public CodeDefinition Model_Update = new CodeDefinition();
        public CodeDefinition Model_UpdateParams = new CodeDefinition();
        public CodeDefinition Model_Update_Sites = new CodeDefinition();
        public CodeDefinition Model_UpdateRelatedRecords = new CodeDefinition();
        public CodeDefinition Model_OnUpdatedNotice = new CodeDefinition();
        public CodeDefinition Model_OnUpdated_Depts = new CodeDefinition();
        public CodeDefinition Model_OnUpdated_Groups = new CodeDefinition();
        public CodeDefinition Model_OnUpdated_Users = new CodeDefinition();
        public CodeDefinition Model_OnUpdated_SetSiteMenu = new CodeDefinition();
        public CodeDefinition Model_OnUpdated_OutgoingMails = new CodeDefinition();
        public CodeDefinition Model_SynchronizeSummaryExecute = new CodeDefinition();
        public CodeDefinition Model_SynchronizeSummary = new CodeDefinition();
        public CodeDefinition Model_SynchronizeSummaryColumnCases = new CodeDefinition();
        public CodeDefinition Model_UpdateFormulaColumns = new CodeDefinition();
        public CodeDefinition Model_UpdateFormulaColumns_ColumnCases = new CodeDefinition();
        public CodeDefinition Model_UpdateOrCreate = new CodeDefinition();
        public CodeDefinition Model_UpdateRelatedRecordsMethod = new CodeDefinition();
        public CodeDefinition Model_InsertLinksByUpdate = new CodeDefinition();
        public CodeDefinition Model_InsertLinksByUpdate_Site = new CodeDefinition();
        public CodeDefinition Model_UpdateSites_Wikis = new CodeDefinition();
        public CodeDefinition Model_InsertLinks = new CodeDefinition();
        public CodeDefinition Model_InsertLinksCases = new CodeDefinition();
        public CodeDefinition Model_Move = new CodeDefinition();
        public CodeDefinition Model_Delete = new CodeDefinition();
        public CodeDefinition Model_DeleteParams = new CodeDefinition();
        public CodeDefinition Model_Delete_Item = new CodeDefinition();
        public CodeDefinition Model_Delete_GroupMembers = new CodeDefinition();
        public CodeDefinition Model_Delete_Sites = new CodeDefinition();
        public CodeDefinition Model_Delete_SitesItems = new CodeDefinition();
        public CodeDefinition Model_OnDeleted_SetSiteInfo = new CodeDefinition();
        public CodeDefinition Model_OnDeletedNotice = new CodeDefinition();
        public CodeDefinition Model_Restore = new CodeDefinition();
        public CodeDefinition Model_Restore_Item = new CodeDefinition();
        public CodeDefinition Model_PhysicalDelete = new CodeDefinition();
        public CodeDefinition Model_SetByForm = new CodeDefinition();
        public CodeDefinition Model_SetByForm_ColumnCases = new CodeDefinition();
        public CodeDefinition Model_SetByForm_SetByFormula = new CodeDefinition();
        public CodeDefinition Model_ToUniversal = new CodeDefinition();
        public CodeDefinition Model_SetByForm_Files = new CodeDefinition();
        public CodeDefinition Model_SetByForm_Site = new CodeDefinition();
        public CodeDefinition Model_CreateIndexes = new CodeDefinition();
        public CodeDefinition Model_AddSqlParamIdentity = new CodeDefinition();
        public CodeDefinition Model_AddSqlParamPk = new CodeDefinition();
        public CodeDefinition Model_AddSqlParam = new CodeDefinition();
        public CodeDefinition Model_AddSqlParamPkHistory = new CodeDefinition();
        public CodeDefinition Model_SetByFormula = new CodeDefinition();
        public CodeDefinition Model_SetByFormula_Data = new CodeDefinition();
        public CodeDefinition Model_SetByFormula_ColumnCases = new CodeDefinition();
        public CodeDefinition Model_SetBySession = new CodeDefinition();
        public CodeDefinition Model_SetPk = new CodeDefinition();
        public CodeDefinition Model_Set = new CodeDefinition();
        public CodeDefinition Model_PushState = new CodeDefinition();
        public CodeDefinition Model_PushState_Item = new CodeDefinition();
        public CodeDefinition Model_Matched = new CodeDefinition();
        public CodeDefinition Model_Matched_Incomplete = new CodeDefinition();
        public CodeDefinition Model_Matched_Own = new CodeDefinition();
        public CodeDefinition Model_Matched_Issues = new CodeDefinition();
        public CodeDefinition Model_Matched_ColumnCases = new CodeDefinition();
        public CodeDefinition Model_Notice = new CodeDefinition();
        public CodeDefinition Model_NoticeColumnCases = new CodeDefinition();
        public CodeDefinition Model_SwitchItems = new CodeDefinition();
        public CodeDefinition Model_IndexCases = new CodeDefinition();
        public CodeDefinition Model_IndexJsonCases = new CodeDefinition();
        public CodeDefinition Model_GanttCases = new CodeDefinition();
        public CodeDefinition Model_GanttJsonCases = new CodeDefinition();
        public CodeDefinition Model_BurnDownCases = new CodeDefinition();
        public CodeDefinition Model_BurnDownJsonCases = new CodeDefinition();
        public CodeDefinition Model_BurnDownRecordDetailsJsonCases = new CodeDefinition();
        public CodeDefinition Model_TimeSeriesCases = new CodeDefinition();
        public CodeDefinition Model_TimeSeriesJsonCases = new CodeDefinition();
        public CodeDefinition Model_KambanCases = new CodeDefinition();
        public CodeDefinition Model_KambanJsonCases = new CodeDefinition();
        public CodeDefinition Model_NewCases = new CodeDefinition();
        public CodeDefinition Model_EditorCases = new CodeDefinition();
        public CodeDefinition Model_ImportCases = new CodeDefinition();
        public CodeDefinition Model_ExportCases = new CodeDefinition();
        public CodeDefinition Model_GridRowsCases = new CodeDefinition();
        public CodeDefinition Model_CreateCases = new CodeDefinition();
        public CodeDefinition Model_UpdateCases = new CodeDefinition();
        public CodeDefinition Model_DeleteCommentCases = new CodeDefinition();
        public CodeDefinition Model_CopyCases = new CodeDefinition();
        public CodeDefinition Model_MoveCases = new CodeDefinition();
        public CodeDefinition Model_BulkMoveCases = new CodeDefinition();
        public CodeDefinition Model_DeleteCases = new CodeDefinition();
        public CodeDefinition Model_BulkDeleteCases = new CodeDefinition();
        public CodeDefinition Model_RestoreCases = new CodeDefinition();
        public CodeDefinition Model_EditSeparateSettingsCases = new CodeDefinition();
        public CodeDefinition Model_SeparateCases = new CodeDefinition();
        public CodeDefinition Model_HistoriesCases = new CodeDefinition();
        public CodeDefinition Model_HistoryCases = new CodeDefinition();
        public CodeDefinition Model_EditorJsonCases = new CodeDefinition();
        public CodeDefinition Model_GetCases = new CodeDefinition();
        public CodeDefinition Model_UpdateByKambanCases = new CodeDefinition();
        public CodeDefinition Model_ParentIdParameter = new CodeDefinition();
        public CodeDefinition Model_InheritPermissionParameter = new CodeDefinition();
        public CodeDefinition Model_SwitchTargetsParameter = new CodeDefinition();
        public CodeDefinition Model_SetSiteId = new CodeDefinition();
        public CodeDefinition Model_SetSiteSettings = new CodeDefinition();
        public CodeDefinition Model_SetPermissionType = new CodeDefinition();
        public CodeDefinition Model_SetParentId = new CodeDefinition();
        public CodeDefinition Model_SetInheritPermission = new CodeDefinition();
        public CodeDefinition Model_SetSwitchTargets = new CodeDefinition();
        public CodeDefinition Model_SiteSettings = new CodeDefinition();
        public CodeDefinition Model_SiteSettingsOnly = new CodeDefinition();
        public CodeDefinition Model_SiteSettings_Sites = new CodeDefinition();
        public CodeDefinition Model_SiteSettings_SitesOnly = new CodeDefinition();
        public CodeDefinition Model_SiteSettingsWithParameterName = new CodeDefinition();
        public CodeDefinition Model_SiteSettingsWithParameterNameLower = new CodeDefinition();
        public CodeDefinition Model_SiteSettingsParam = new CodeDefinition();
        public CodeDefinition Model_SiteSettingsParamOnly = new CodeDefinition();
        public CodeDefinition Collection = new CodeDefinition();
        public CodeDefinition Collection_SiteSettings = new CodeDefinition();
        public CodeDefinition Collection_SiteSettingsArgument = new CodeDefinition();
        public CodeDefinition Collection_Aggregation = new CodeDefinition();
        public CodeDefinition Collection_ItemAggregation = new CodeDefinition();
        public CodeDefinition Model_Utilities = new CodeDefinition();
        public CodeDefinition Model_Utilities_Index = new CodeDefinition();
        public CodeDefinition Model_Utilities_SetChoiceHash = new CodeDefinition();
        public CodeDefinition Model_Utilities_DropDownSearchDialog = new CodeDefinition();
        public CodeDefinition Model_Utilities_ImportSettings = new CodeDefinition();
        public CodeDefinition Model_Utilities_GridRows_OnClick = new CodeDefinition();
        public CodeDefinition Model_Utilities_GridRows_OnClickItem = new CodeDefinition();
        public CodeDefinition Model_Utilities_TdValue = new CodeDefinition();
        public CodeDefinition Model_Utilities_TdValueCases = new CodeDefinition();
        public CodeDefinition Model_Utilities_TdValueCustomValueCases = new CodeDefinition();
        public CodeDefinition Model_Utilities_SqlColumn_SiteId = new CodeDefinition();
        public CodeDefinition Model_Utilities_GridSqlWhereTenantId = new CodeDefinition();
        public CodeDefinition Model_Utilities_GridSqlWhereSiteId = new CodeDefinition();
        public CodeDefinition Model_Utilities_Editor = new CodeDefinition();
        public CodeDefinition Model_Utilities_SiteSettingsUtilities = new CodeDefinition();
        public CodeDefinition Model_Utilities_EditorItem = new CodeDefinition();
        public CodeDefinition Model_Utilities_SetSwitchTargets = new CodeDefinition();
        public CodeDefinition Model_Utilities_FieldCases = new CodeDefinition();
        public CodeDefinition Model_Utilities_FieldCases_Item = new CodeDefinition();
        public CodeDefinition Model_Utilities_Links = new CodeDefinition();
        public CodeDefinition Model_Utilities_EditorJson = new CodeDefinition();
        public CodeDefinition Model_Utilities_EditorResponse = new CodeDefinition();
        public CodeDefinition Model_Utilities_GetSwitchTargets = new CodeDefinition();
        public CodeDefinition Model_Utilities_SqlWhereTenantId = new CodeDefinition();
        public CodeDefinition Model_Utilities_SqlWhereSiteId = new CodeDefinition();
        public CodeDefinition Model_Utilities_SiteId = new CodeDefinition();
        public CodeDefinition Model_Utilities_SiteIdParam = new CodeDefinition();
        public CodeDefinition Model_Utilities_FieldResponse = new CodeDefinition();
        public CodeDefinition Model_Utilities_FieldResponse_ColumnCases = new CodeDefinition();
        public CodeDefinition Model_Utilities_TableName = new CodeDefinition();
        public CodeDefinition Model_Utilities_TableNameCases = new CodeDefinition();
        public CodeDefinition Model_Utilities_Create = new CodeDefinition();
        public CodeDefinition Model_Utilities_CreateParams = new CodeDefinition();
        public CodeDefinition Model_Utilities_CreateParams_Sites = new CodeDefinition();
        public CodeDefinition Model_Utilities_CreatedResponse = new CodeDefinition();
        public CodeDefinition Model_Utilities_CreatedResponse_Items = new CodeDefinition();
        public CodeDefinition Model_Utilities_CreatedResponse_Sites = new CodeDefinition();
        public CodeDefinition Model_Utilities_Update = new CodeDefinition();
        public CodeDefinition Model_Utilities_Update_NewModel = new CodeDefinition();
        public CodeDefinition Model_Utilities_Update_NewModel_Sites = new CodeDefinition();
        public CodeDefinition Model_Utilities_UpdateInvalid_Users = new CodeDefinition();
        public CodeDefinition Model_Utilities_OnUpdated_Issues = new CodeDefinition();
        public CodeDefinition Model_Utilities_OnUpdated_Breadcrumb = new CodeDefinition();
        public CodeDefinition Model_Utilities_ResponseByUpdate_FieldResponse = new CodeDefinition();
        public CodeDefinition Model_Utilities_Copy = new CodeDefinition();
        public CodeDefinition Model_Utilities_CopyResponse = new CodeDefinition();
        public CodeDefinition Model_Utilities_CopyResponse_Items = new CodeDefinition();
        public CodeDefinition Model_Utilities_Copy_Sites = new CodeDefinition();
        public CodeDefinition Model_Utilities_Move = new CodeDefinition();
        public CodeDefinition Model_Utilities_Delete = new CodeDefinition();
        public CodeDefinition Model_Utilities_RedirectAfterDelete = new CodeDefinition();
        public CodeDefinition Model_Utilities_RedirectAfterDeleteNotItem = new CodeDefinition();
        public CodeDefinition Model_Utilities_RedirectAfterDeleteItem = new CodeDefinition();
        public CodeDefinition Model_Utilities_RedirectAfterDelete_Sites = new CodeDefinition();
        public CodeDefinition Model_Utilities_RedirectAfterDelete_Wikis = new CodeDefinition();
        public CodeDefinition Model_Utilities_Restore = new CodeDefinition();
        public CodeDefinition Model_Utilities_Histories = new CodeDefinition();
        public CodeDefinition Model_Utilities_HistoriesParams = new CodeDefinition();
        public CodeDefinition Model_Utilities_HistoriesParams_Sites = new CodeDefinition();
        public CodeDefinition Model_Utilities_HistoryParams = new CodeDefinition();
        public CodeDefinition Model_Utilities_HistoryParams_Sites = new CodeDefinition();
        public CodeDefinition Model_Utilities_TitleDisplay = new CodeDefinition();
        public CodeDefinition Model_Utilities_ResponseLinks = new CodeDefinition();
        public CodeDefinition Model_Utilities_SiteModel = new CodeDefinition();
        public CodeDefinition Model_Utilities_Separate = new CodeDefinition();
        public CodeDefinition Model_Utilities_Title = new CodeDefinition();
        public CodeDefinition Model_Utilities_BulkMove = new CodeDefinition();
        public CodeDefinition Model_Utilities_BulkDelete = new CodeDefinition();
        public CodeDefinition Model_Utilities_Import = new CodeDefinition();
        public CodeDefinition Model_Utilities_ImportCases = new CodeDefinition();
        public CodeDefinition Model_Utilities_ImportValidatorHeaders = new CodeDefinition();
        public CodeDefinition Model_Utilities_ImportValidatorCases = new CodeDefinition();
        public CodeDefinition Model_Utilities_Export = new CodeDefinition();
        public CodeDefinition Model_Utilities_CsvColumnCases = new CodeDefinition();
        public CodeDefinition Model_Utilities_NotNull = new CodeDefinition();
        public CodeDefinition Model_Utilities_TitleDisplayValue = new CodeDefinition();
        public CodeDefinition Model_Utilities_TitleDisplayValueCases_Item = new CodeDefinition();
        public CodeDefinition Model_Utilities_TitleDisplayValueCases_DataRow = new CodeDefinition();
        public CodeDefinition Model_Utilities_UpdateByKamban = new CodeDefinition();
        public CodeDefinition Model_Utilities_SetNoticeParam = new CodeDefinition();
        public CodeDefinition Model_Utilities_SearchIndexes = new CodeDefinition();
        public CodeDefinition Model_Utilities_SearchIndexes_TableCases = new CodeDefinition();
        public CodeDefinition Model_Utilities_ItemTitle = new CodeDefinition();
        public CodeDefinition Model_Validator = new CodeDefinition();
        public CodeDefinition Model_ValidatorMethods = new CodeDefinition();
        public CodeDefinition Model_Validator_OnMoving = new CodeDefinition();
        public CodeDefinition Model_Validator_OnCreatingCases = new CodeDefinition();
        public CodeDefinition Model_Validator_OnUpdating_Users = new CodeDefinition();
        public CodeDefinition Model_Validator_OnUpdatingCases = new CodeDefinition();
        public CodeDefinition Model_ValidatorMethods_Binaries = new CodeDefinition();
        public CodeDefinition Rds = new CodeDefinition();
        public CodeDefinition Rds_SqlStatement = new CodeDefinition();
        public CodeDefinition Rds_SqlSelect = new CodeDefinition();
        public CodeDefinition Rds_SqlExists = new CodeDefinition();
        public CodeDefinition Rds_SqlInsert = new CodeDefinition();
        public CodeDefinition Rds_SqlUpdate = new CodeDefinition();
        public CodeDefinition Rds_SqlUpdateOrInsert = new CodeDefinition();
        public CodeDefinition Rds_SqlDelete = new CodeDefinition();
        public CodeDefinition Rds_SqlPhysicalDelete = new CodeDefinition();
        public CodeDefinition Rds_SqlRestore = new CodeDefinition();
        public CodeDefinition Rds_AggregationTableCases = new CodeDefinition();
        public CodeDefinition Rds_Aggregation = new CodeDefinition();
        public CodeDefinition Rds_AggregationTotalCases = new CodeDefinition();
        public CodeDefinition Rds_AggregationAverageCases = new CodeDefinition();
        public CodeDefinition Rds_AggregationGroupByCases = new CodeDefinition();
        public CodeDefinition Rds_AggregationOverdue = new CodeDefinition();
        public CodeDefinition Rds_SaveHistory = new CodeDefinition();
        public CodeDefinition Rds_SaveHistory_Columns = new CodeDefinition();
        public CodeDefinition Rds_SaveHistory_HistoryColumns = new CodeDefinition();
        public CodeDefinition Rds_SaveHistory_WhereColumns = new CodeDefinition();
        public CodeDefinition Rds_Delete = new CodeDefinition();
        public CodeDefinition Rds_Delete_Columns = new CodeDefinition();
        public CodeDefinition Rds_Delete_DeletedColumns = new CodeDefinition();
        public CodeDefinition Rds_Delete_WhereColumns = new CodeDefinition();
        public CodeDefinition Rds_Delete_WhereDeletedColumns = new CodeDefinition();
        public CodeDefinition Rds_Restore = new CodeDefinition();
        public CodeDefinition Rds_Restore_Columns = new CodeDefinition();
        public CodeDefinition Rds_Restore_DeletedColumns = new CodeDefinition();
        public CodeDefinition Rds_Restore_WhereColumns = new CodeDefinition();
        public CodeDefinition Rds_Restore_WhereDeletedColumns = new CodeDefinition();
        public CodeDefinition Rds_Restore_IdentityOn = new CodeDefinition();
        public CodeDefinition Rds_Restore_IdentityOff = new CodeDefinition();
        public CodeDefinition Rds_Columns = new CodeDefinition();
        public CodeDefinition Rds_Columns_SqlClasses = new CodeDefinition();
        public CodeDefinition Rds_Columns_ConstSqlWhereLike = new CodeDefinition();
        public CodeDefinition Rds_Columns_ConstSqlWhereExists = new CodeDefinition();
        public CodeDefinition Rds_Columns_ConstSqlWhereNotExists = new CodeDefinition();
        public CodeDefinition Rds_Columns_SqlColumnCases = new CodeDefinition();
        public CodeDefinition Rds_Columns_SqlColumn = new CodeDefinition();
        public CodeDefinition Rds_Columns_SqlColumn_SelectColumns = new CodeDefinition();
        public CodeDefinition Rds_Columns_SqlColumn_ComputeColumn = new CodeDefinition();
        public CodeDefinition Rds_Columns_SqlWhere = new CodeDefinition();
        public CodeDefinition Rds_Columns_SqlWhere_SelectColumns = new CodeDefinition();
        public CodeDefinition Rds_Columns_SqlWhere_ComputeColumn = new CodeDefinition();
        public CodeDefinition Rds_Columns_SqlWhere_In = new CodeDefinition();
        public CodeDefinition Rds_Columns_SqlWhere_Between_Number = new CodeDefinition();
        public CodeDefinition Rds_Columns_SqlWhere_Between_DateTime = new CodeDefinition();
        public CodeDefinition Rds_Columns_SqlGroupByCases = new CodeDefinition();
        public CodeDefinition Rds_Columns_SqlGroupBy = new CodeDefinition();
        public CodeDefinition Rds_Columns_SqlHavingComputes = new CodeDefinition();
        public CodeDefinition Rds_Columns_SqlOrderBy1 = new CodeDefinition();
        public CodeDefinition Rds_Columns_SqlOrderBy2 = new CodeDefinition();
        public CodeDefinition Rds_Columns_SqlParam_ItemId = new CodeDefinition();
        public CodeDefinition Rds_Columns_SqlParam = new CodeDefinition();
        public CodeDefinition Rds_Columns_SqlParam_Enum = new CodeDefinition();
        public CodeDefinition Rds_Defaults = new CodeDefinition();
        public CodeDefinition Rds_ColumnDefault = new CodeDefinition();
        public CodeDefinition Rds_ItemEditorColumns = new CodeDefinition();
        public CodeDefinition Rds_ColumnDefaultPk = new CodeDefinition();
        public CodeDefinition Rds_JoinDefault = new CodeDefinition();
        public CodeDefinition Rds_WherePk = new CodeDefinition();
        public CodeDefinition Rds_Where_TenantId = new CodeDefinition();
        public CodeDefinition Rds_Where_ItemId = new CodeDefinition();
        public CodeDefinition Rds_WhereHistory = new CodeDefinition();
        public CodeDefinition Rds_ParamDefault = new CodeDefinition();
        public CodeDefinition Rds_ParamDefault_SetDefault = new CodeDefinition();
        public CodeDefinition Rds_ParamDefault_ParamAll = new CodeDefinition();
        public CodeDefinition Rds_ParamItemId = new CodeDefinition();
        public CodeDefinition Rds_SiteId = new CodeDefinition();
        public CodeDefinition Rds_TitleColumn = new CodeDefinition();
        public CodeDefinition Rds_TitleColumnCases = new CodeDefinition();
        public CodeDefinition Titles = new CodeDefinition();
        public CodeDefinition Titles_TitleDisplayValueCases = new CodeDefinition();
        public CodeDefinition Indexes = new CodeDefinition();
        public CodeDefinition Indexes_TableCases = new CodeDefinition();
        public CodeDefinition ItemsInitializer = new CodeDefinition();
        public CodeDefinition ItemsInitializer_InitItems = new CodeDefinition();
        public CodeDefinition SiteSettings = new CodeDefinition();
        public CodeDefinition SiteSettings_GetCases = new CodeDefinition();
        public CodeDefinition SiteSettings_GetByItemCases = new CodeDefinition();
        public CodeDefinition SiteSettings_GetByReferenceCases = new CodeDefinition();
        public CodeDefinition SiteSettings_GetByItemReferenceCases = new CodeDefinition();
        public CodeDefinition SiteSettings_GetModels = new CodeDefinition();
        public CodeDefinition SiteSettings_GetModels_Choices = new CodeDefinition();
        public CodeDefinition SiteSettings_GetModels_Items = new CodeDefinition();
        public CodeDefinition SiteSettings_GetModels_Items_Choices = new CodeDefinition();
        public CodeDefinition SiteSettings_GetModels_Includes = new CodeDefinition();
        public CodeDefinition SiteSettings_UpdateTitles = new CodeDefinition();
        public CodeDefinition SiteSettings_UpdateTitles_TableCases = new CodeDefinition();
        public CodeDefinition View = new CodeDefinition();
        public CodeDefinition View_Search_TableCases = new CodeDefinition();
        public CodeDefinition View_Sorter_TableCases = new CodeDefinition();
        public CodeDefinition View_Sorter_ColumnCases = new CodeDefinition();
        public CodeDefinition HtmlLinks = new CodeDefinition();
        public CodeDefinition HtmlLinks_DataSetTableCases = new CodeDefinition();
        public CodeDefinition HtmlLinks_SelectStatementTableCases = new CodeDefinition();
        public CodeDefinition HtmlLinks_TableCases = new CodeDefinition();
        public CodeDefinition HtmlLinks_Headers = new CodeDefinition();
        public CodeDefinition HtmlLinks_Rows = new CodeDefinition();
        public CodeDefinition HtmlLinks_RowColumns = new CodeDefinition();
        public CodeDefinition Summaries = new CodeDefinition();
        public CodeDefinition Summaries_SynchronizeCases = new CodeDefinition();
        public CodeDefinition Summaries_SynchronizeTables = new CodeDefinition();
        public CodeDefinition Summaries_ParamCases = new CodeDefinition();
        public CodeDefinition Summaries_ZeroParamCases = new CodeDefinition();
        public CodeDefinition Summaries_SelectCountTables = new CodeDefinition();
        public CodeDefinition Summaries_SelectTotalTableCases = new CodeDefinition();
        public CodeDefinition Summaries_SelectTotalColumns = new CodeDefinition();
        public CodeDefinition Summaries_SelectTotalColumnCases = new CodeDefinition();
        public CodeDefinition Summaries_SelectAverageTableCases = new CodeDefinition();
        public CodeDefinition Summaries_SelectAverageColumns = new CodeDefinition();
        public CodeDefinition Summaries_SelectAverageColumnCases = new CodeDefinition();
        public CodeDefinition Summaries_SelectMinTableCases = new CodeDefinition();
        public CodeDefinition Summaries_SelectMinColumns = new CodeDefinition();
        public CodeDefinition Summaries_SelectMinColumnCases = new CodeDefinition();
        public CodeDefinition Summaries_SelectMaxTableCases = new CodeDefinition();
        public CodeDefinition Summaries_SelectMaxColumns = new CodeDefinition();
        public CodeDefinition Summaries_SelectMaxColumnCases = new CodeDefinition();
        public CodeDefinition Summaries_WhereTables = new CodeDefinition();
        public CodeDefinition Summaries_WhereColumnCases = new CodeDefinition();
        public CodeDefinition FormulaUtilities = new CodeDefinition();
        public CodeDefinition FormulaUtilities_TableCases = new CodeDefinition();
        public CodeDefinition FormulaUtilities_Updates = new CodeDefinition();
        public CodeDefinition Messages = new CodeDefinition();
        public CodeDefinition Messages_Parts = new CodeDefinition();
        public CodeDefinition Messages_Resonses = new CodeDefinition();
        public CodeDefinition ResponseCollection = new CodeDefinition();
        public CodeDefinition ResponseCollection_Models = new CodeDefinition();
        public CodeDefinition ResponseCollection_ValueModels = new CodeDefinition();
        public CodeDefinition ResponseCollection_ValueColumns = new CodeDefinition();
        public CodeDefinition Displays = new CodeDefinition();
        public CodeDefinition Displays_Parts = new CodeDefinition();
        public CodeDefinition Error = new CodeDefinition();
        public CodeDefinition Error_Types = new CodeDefinition();
        public CodeDefinition Error_Messages = new CodeDefinition();
        public CodeDefinition Css = new CodeDefinition();
        public CodeDefinition ClientDisplays = new CodeDefinition();
        public CodeDefinition ClientDisplays_Parts = new CodeDefinition();
        public CodeDefinition Comma = new CodeDefinition();
    }

    public class ColumnDefinition
    {
        public string Id; public string SavedId;
        public string ModelName; public string SavedModelName;
        public string TableName; public string SavedTableName;
        public bool Base; public bool SavedBase;
        public bool EachModel; public bool SavedEachModel;
        public string Label; public string SavedLabel;
        public string ColumnName; public string SavedColumnName;
        public string ColumnLabel; public string SavedColumnLabel;
        public int No; public int SavedNo;
        public int History; public int SavedHistory;
        public int Import; public int SavedImport;
        public int Export; public int SavedExport;
        public int GridColumn; public int SavedGridColumn;
        public bool GridEnabled; public bool SavedGridEnabled;
        public int FilterColumn; public int SavedFilterColumn;
        public bool FilterEnabled; public bool SavedFilterEnabled;
        public bool EditorColumn; public bool SavedEditorColumn;
        public bool EditorEnabled; public bool SavedEditorEnabled;
        public int TitleColumn; public int SavedTitleColumn;
        public int LinkColumn; public int SavedLinkColumn;
        public bool LinkEnabled; public bool SavedLinkEnabled;
        public int HistoryColumn; public int SavedHistoryColumn;
        public bool HistoryEnabled; public bool SavedHistoryEnabled;
        public string TypeName; public string SavedTypeName;
        public string TypeCs; public string SavedTypeCs;
        public string RecordingData; public string SavedRecordingData;
        public int MaxLength; public int SavedMaxLength;
        public string Size; public string SavedSize;
        public int Pk; public int SavedPk;
        public string PkOrderBy; public string SavedPkOrderBy;
        public int PkHistory; public int SavedPkHistory;
        public string PkHistoryOrderBy; public string SavedPkHistoryOrderBy;
        public int Ix1; public int SavedIx1;
        public string Ix1OrderBy; public string SavedIx1OrderBy;
        public int Ix2; public int SavedIx2;
        public string Ix2OrderBy; public string SavedIx2OrderBy;
        public int Ix3; public int SavedIx3;
        public string Ix3OrderBy; public string SavedIx3OrderBy;
        public bool Nullable; public bool SavedNullable;
        public string Default; public string SavedDefault;
        public string DefaultCs; public string SavedDefaultCs;
        public bool Identity; public bool SavedIdentity;
        public bool Unique; public bool SavedUnique;
        public int Seed; public int SavedSeed;
        public string JoinTableName; public string SavedJoinTableName;
        public string JoinType; public string SavedJoinType;
        public string JoinExpression; public string SavedJoinExpression;
        public bool Like; public bool SavedLike;
        public bool WhereSpecial; public bool SavedWhereSpecial;
        public string ReadPermission; public string SavedReadPermission;
        public string CreatePermission; public string SavedCreatePermission;
        public string UpdatePermission; public string SavedUpdatePermission;
        public bool NotEditSelf; public bool SavedNotEditSelf;
        public int SearchIndexPriority; public int SavedSearchIndexPriority;
        public bool NotForm; public bool SavedNotForm;
        public bool NotSelect; public bool SavedNotSelect;
        public bool NotUpdate; public bool SavedNotUpdate;
        public string ByForm; public string SavedByForm;
        public string ByDataRow; public string SavedByDataRow;
        public string BySession; public string SavedBySession;
        public string ToControl; public string SavedToControl;
        public string SelectColumns; public string SavedSelectColumns;
        public string ComputeColumn; public string SavedComputeColumn;
        public string OrderByColumns; public string SavedOrderByColumns;
        public int ItemId; public int SavedItemId;
        public bool GenericUi; public bool SavedGenericUi;
        public string FieldCss; public string SavedFieldCss;
        public string ControlCss; public string SavedControlCss;
        public bool MarkDown; public bool SavedMarkDown;
        public string GridStyle; public string SavedGridStyle;
        public bool Hash; public bool SavedHash;
        public string Calc; public string SavedCalc;
        public bool Session; public bool SavedSession;
        public bool UserColumn; public bool SavedUserColumn;
        public bool EnumColumn; public bool SavedEnumColumn;
        public bool NotEditorSettings; public bool SavedNotEditorSettings;
        public string ControlType; public string SavedControlType;
        public string GridFormat; public string SavedGridFormat;
        public string EditorFormat; public string SavedEditorFormat;
        public string ExportFormat; public string SavedExportFormat;
        public bool Aggregatable; public bool SavedAggregatable;
        public bool Computable; public bool SavedComputable;
        public string ChoicesText; public string SavedChoicesText;
        public bool UseSearch; public bool SavedUseSearch;
        public string DefaultInput; public string SavedDefaultInput;
        public bool Own; public bool SavedOwn;
        public string FormName; public string SavedFormName;
        public bool ValidateRequired; public bool SavedValidateRequired;
        public bool ValidateNumber; public bool SavedValidateNumber;
        public bool ValidateDate; public bool SavedValidateDate;
        public bool ValidateEmail; public bool SavedValidateEmail;
        public string ValidateEqualTo; public string SavedValidateEqualTo;
        public int DecimalPlaces; public int SavedDecimalPlaces;
        public decimal Min; public decimal SavedMin;
        public decimal Max; public decimal SavedMax;
        public decimal Step; public decimal SavedStep;
        public string StringFormat; public string SavedStringFormat;
        public string Unit; public string SavedUnit;
        public decimal NumFilterMin; public decimal SavedNumFilterMin;
        public decimal NumFilterMax; public decimal SavedNumFilterMax;
        public decimal NumFilterStep; public decimal SavedNumFilterStep;
        public int Width; public int SavedWidth;
        public bool SettingEnable; public bool SavedSettingEnable;
        public string OldColumnName; public string SavedOldColumnName;

        public ColumnDefinition()
        {
        }

        public ColumnDefinition(Dictionary<string, string> propertyCollection)
        {
            if (propertyCollection.ContainsKey("Id")) Id = propertyCollection["Id"].ToString(); else Id = string.Empty;
            if (propertyCollection.ContainsKey("ModelName")) ModelName = propertyCollection["ModelName"].ToString(); else ModelName = string.Empty;
            if (propertyCollection.ContainsKey("TableName")) TableName = propertyCollection["TableName"].ToString(); else TableName = string.Empty;
            if (propertyCollection.ContainsKey("Base")) Base = propertyCollection["Base"].ToBool(); else Base = false;
            if (propertyCollection.ContainsKey("EachModel")) EachModel = propertyCollection["EachModel"].ToBool(); else EachModel = false;
            if (propertyCollection.ContainsKey("Label")) Label = propertyCollection["Label"].ToString(); else Label = string.Empty;
            if (propertyCollection.ContainsKey("ColumnName")) ColumnName = propertyCollection["ColumnName"].ToString(); else ColumnName = string.Empty;
            if (propertyCollection.ContainsKey("ColumnLabel")) ColumnLabel = propertyCollection["ColumnLabel"].ToString(); else ColumnLabel = string.Empty;
            if (propertyCollection.ContainsKey("No")) No = propertyCollection["No"].ToInt(); else No = 0;
            if (propertyCollection.ContainsKey("History")) History = propertyCollection["History"].ToInt(); else History = 0;
            if (propertyCollection.ContainsKey("Import")) Import = propertyCollection["Import"].ToInt(); else Import = 0;
            if (propertyCollection.ContainsKey("Export")) Export = propertyCollection["Export"].ToInt(); else Export = 0;
            if (propertyCollection.ContainsKey("GridColumn")) GridColumn = propertyCollection["GridColumn"].ToInt(); else GridColumn = 0;
            if (propertyCollection.ContainsKey("GridEnabled")) GridEnabled = propertyCollection["GridEnabled"].ToBool(); else GridEnabled = false;
            if (propertyCollection.ContainsKey("FilterColumn")) FilterColumn = propertyCollection["FilterColumn"].ToInt(); else FilterColumn = 0;
            if (propertyCollection.ContainsKey("FilterEnabled")) FilterEnabled = propertyCollection["FilterEnabled"].ToBool(); else FilterEnabled = false;
            if (propertyCollection.ContainsKey("EditorColumn")) EditorColumn = propertyCollection["EditorColumn"].ToBool(); else EditorColumn = false;
            if (propertyCollection.ContainsKey("EditorEnabled")) EditorEnabled = propertyCollection["EditorEnabled"].ToBool(); else EditorEnabled = false;
            if (propertyCollection.ContainsKey("TitleColumn")) TitleColumn = propertyCollection["TitleColumn"].ToInt(); else TitleColumn = 0;
            if (propertyCollection.ContainsKey("LinkColumn")) LinkColumn = propertyCollection["LinkColumn"].ToInt(); else LinkColumn = 0;
            if (propertyCollection.ContainsKey("LinkEnabled")) LinkEnabled = propertyCollection["LinkEnabled"].ToBool(); else LinkEnabled = false;
            if (propertyCollection.ContainsKey("HistoryColumn")) HistoryColumn = propertyCollection["HistoryColumn"].ToInt(); else HistoryColumn = 0;
            if (propertyCollection.ContainsKey("HistoryEnabled")) HistoryEnabled = propertyCollection["HistoryEnabled"].ToBool(); else HistoryEnabled = false;
            if (propertyCollection.ContainsKey("TypeName")) TypeName = propertyCollection["TypeName"].ToString(); else TypeName = string.Empty;
            if (propertyCollection.ContainsKey("TypeCs")) TypeCs = propertyCollection["TypeCs"].ToString(); else TypeCs = string.Empty;
            if (propertyCollection.ContainsKey("RecordingData")) RecordingData = propertyCollection["RecordingData"].ToString(); else RecordingData = string.Empty;
            if (propertyCollection.ContainsKey("MaxLength")) MaxLength = propertyCollection["MaxLength"].ToInt(); else MaxLength = 0;
            if (propertyCollection.ContainsKey("Size")) Size = propertyCollection["Size"].ToString(); else Size = string.Empty;
            if (propertyCollection.ContainsKey("Pk")) Pk = propertyCollection["Pk"].ToInt(); else Pk = 0;
            if (propertyCollection.ContainsKey("PkOrderBy")) PkOrderBy = propertyCollection["PkOrderBy"].ToString(); else PkOrderBy = string.Empty;
            if (propertyCollection.ContainsKey("PkHistory")) PkHistory = propertyCollection["PkHistory"].ToInt(); else PkHistory = 0;
            if (propertyCollection.ContainsKey("PkHistoryOrderBy")) PkHistoryOrderBy = propertyCollection["PkHistoryOrderBy"].ToString(); else PkHistoryOrderBy = string.Empty;
            if (propertyCollection.ContainsKey("Ix1")) Ix1 = propertyCollection["Ix1"].ToInt(); else Ix1 = 0;
            if (propertyCollection.ContainsKey("Ix1OrderBy")) Ix1OrderBy = propertyCollection["Ix1OrderBy"].ToString(); else Ix1OrderBy = string.Empty;
            if (propertyCollection.ContainsKey("Ix2")) Ix2 = propertyCollection["Ix2"].ToInt(); else Ix2 = 0;
            if (propertyCollection.ContainsKey("Ix2OrderBy")) Ix2OrderBy = propertyCollection["Ix2OrderBy"].ToString(); else Ix2OrderBy = string.Empty;
            if (propertyCollection.ContainsKey("Ix3")) Ix3 = propertyCollection["Ix3"].ToInt(); else Ix3 = 0;
            if (propertyCollection.ContainsKey("Ix3OrderBy")) Ix3OrderBy = propertyCollection["Ix3OrderBy"].ToString(); else Ix3OrderBy = string.Empty;
            if (propertyCollection.ContainsKey("Nullable")) Nullable = propertyCollection["Nullable"].ToBool(); else Nullable = false;
            if (propertyCollection.ContainsKey("Default")) Default = propertyCollection["Default"].ToString(); else Default = string.Empty;
            if (propertyCollection.ContainsKey("DefaultCs")) DefaultCs = propertyCollection["DefaultCs"].ToString(); else DefaultCs = string.Empty;
            if (propertyCollection.ContainsKey("Identity")) Identity = propertyCollection["Identity"].ToBool(); else Identity = false;
            if (propertyCollection.ContainsKey("Unique")) Unique = propertyCollection["Unique"].ToBool(); else Unique = false;
            if (propertyCollection.ContainsKey("Seed")) Seed = propertyCollection["Seed"].ToInt(); else Seed = 0;
            if (propertyCollection.ContainsKey("JoinTableName")) JoinTableName = propertyCollection["JoinTableName"].ToString(); else JoinTableName = string.Empty;
            if (propertyCollection.ContainsKey("JoinType")) JoinType = propertyCollection["JoinType"].ToString(); else JoinType = string.Empty;
            if (propertyCollection.ContainsKey("JoinExpression")) JoinExpression = propertyCollection["JoinExpression"].ToString(); else JoinExpression = string.Empty;
            if (propertyCollection.ContainsKey("Like")) Like = propertyCollection["Like"].ToBool(); else Like = false;
            if (propertyCollection.ContainsKey("WhereSpecial")) WhereSpecial = propertyCollection["WhereSpecial"].ToBool(); else WhereSpecial = false;
            if (propertyCollection.ContainsKey("ReadPermission")) ReadPermission = propertyCollection["ReadPermission"].ToString(); else ReadPermission = string.Empty;
            if (propertyCollection.ContainsKey("CreatePermission")) CreatePermission = propertyCollection["CreatePermission"].ToString(); else CreatePermission = string.Empty;
            if (propertyCollection.ContainsKey("UpdatePermission")) UpdatePermission = propertyCollection["UpdatePermission"].ToString(); else UpdatePermission = string.Empty;
            if (propertyCollection.ContainsKey("NotEditSelf")) NotEditSelf = propertyCollection["NotEditSelf"].ToBool(); else NotEditSelf = false;
            if (propertyCollection.ContainsKey("SearchIndexPriority")) SearchIndexPriority = propertyCollection["SearchIndexPriority"].ToInt(); else SearchIndexPriority = 0;
            if (propertyCollection.ContainsKey("NotForm")) NotForm = propertyCollection["NotForm"].ToBool(); else NotForm = false;
            if (propertyCollection.ContainsKey("NotSelect")) NotSelect = propertyCollection["NotSelect"].ToBool(); else NotSelect = false;
            if (propertyCollection.ContainsKey("NotUpdate")) NotUpdate = propertyCollection["NotUpdate"].ToBool(); else NotUpdate = false;
            if (propertyCollection.ContainsKey("ByForm")) ByForm = propertyCollection["ByForm"].ToString(); else ByForm = string.Empty;
            if (propertyCollection.ContainsKey("ByDataRow")) ByDataRow = propertyCollection["ByDataRow"].ToString(); else ByDataRow = string.Empty;
            if (propertyCollection.ContainsKey("BySession")) BySession = propertyCollection["BySession"].ToString(); else BySession = string.Empty;
            if (propertyCollection.ContainsKey("ToControl")) ToControl = propertyCollection["ToControl"].ToString(); else ToControl = string.Empty;
            if (propertyCollection.ContainsKey("SelectColumns")) SelectColumns = propertyCollection["SelectColumns"].ToString(); else SelectColumns = string.Empty;
            if (propertyCollection.ContainsKey("ComputeColumn")) ComputeColumn = propertyCollection["ComputeColumn"].ToString(); else ComputeColumn = string.Empty;
            if (propertyCollection.ContainsKey("OrderByColumns")) OrderByColumns = propertyCollection["OrderByColumns"].ToString(); else OrderByColumns = string.Empty;
            if (propertyCollection.ContainsKey("ItemId")) ItemId = propertyCollection["ItemId"].ToInt(); else ItemId = 0;
            if (propertyCollection.ContainsKey("GenericUi")) GenericUi = propertyCollection["GenericUi"].ToBool(); else GenericUi = false;
            if (propertyCollection.ContainsKey("FieldCss")) FieldCss = propertyCollection["FieldCss"].ToString(); else FieldCss = string.Empty;
            if (propertyCollection.ContainsKey("ControlCss")) ControlCss = propertyCollection["ControlCss"].ToString(); else ControlCss = string.Empty;
            if (propertyCollection.ContainsKey("MarkDown")) MarkDown = propertyCollection["MarkDown"].ToBool(); else MarkDown = false;
            if (propertyCollection.ContainsKey("GridStyle")) GridStyle = propertyCollection["GridStyle"].ToString(); else GridStyle = string.Empty;
            if (propertyCollection.ContainsKey("Hash")) Hash = propertyCollection["Hash"].ToBool(); else Hash = false;
            if (propertyCollection.ContainsKey("Calc")) Calc = propertyCollection["Calc"].ToString(); else Calc = string.Empty;
            if (propertyCollection.ContainsKey("Session")) Session = propertyCollection["Session"].ToBool(); else Session = false;
            if (propertyCollection.ContainsKey("UserColumn")) UserColumn = propertyCollection["UserColumn"].ToBool(); else UserColumn = false;
            if (propertyCollection.ContainsKey("EnumColumn")) EnumColumn = propertyCollection["EnumColumn"].ToBool(); else EnumColumn = false;
            if (propertyCollection.ContainsKey("NotEditorSettings")) NotEditorSettings = propertyCollection["NotEditorSettings"].ToBool(); else NotEditorSettings = false;
            if (propertyCollection.ContainsKey("ControlType")) ControlType = propertyCollection["ControlType"].ToString(); else ControlType = string.Empty;
            if (propertyCollection.ContainsKey("GridFormat")) GridFormat = propertyCollection["GridFormat"].ToString(); else GridFormat = string.Empty;
            if (propertyCollection.ContainsKey("EditorFormat")) EditorFormat = propertyCollection["EditorFormat"].ToString(); else EditorFormat = string.Empty;
            if (propertyCollection.ContainsKey("ExportFormat")) ExportFormat = propertyCollection["ExportFormat"].ToString(); else ExportFormat = string.Empty;
            if (propertyCollection.ContainsKey("Aggregatable")) Aggregatable = propertyCollection["Aggregatable"].ToBool(); else Aggregatable = false;
            if (propertyCollection.ContainsKey("Computable")) Computable = propertyCollection["Computable"].ToBool(); else Computable = false;
            if (propertyCollection.ContainsKey("ChoicesText")) ChoicesText = propertyCollection["ChoicesText"].ToString(); else ChoicesText = string.Empty;
            if (propertyCollection.ContainsKey("UseSearch")) UseSearch = propertyCollection["UseSearch"].ToBool(); else UseSearch = false;
            if (propertyCollection.ContainsKey("DefaultInput")) DefaultInput = propertyCollection["DefaultInput"].ToString(); else DefaultInput = string.Empty;
            if (propertyCollection.ContainsKey("Own")) Own = propertyCollection["Own"].ToBool(); else Own = false;
            if (propertyCollection.ContainsKey("FormName")) FormName = propertyCollection["FormName"].ToString(); else FormName = string.Empty;
            if (propertyCollection.ContainsKey("ValidateRequired")) ValidateRequired = propertyCollection["ValidateRequired"].ToBool(); else ValidateRequired = false;
            if (propertyCollection.ContainsKey("ValidateNumber")) ValidateNumber = propertyCollection["ValidateNumber"].ToBool(); else ValidateNumber = false;
            if (propertyCollection.ContainsKey("ValidateDate")) ValidateDate = propertyCollection["ValidateDate"].ToBool(); else ValidateDate = false;
            if (propertyCollection.ContainsKey("ValidateEmail")) ValidateEmail = propertyCollection["ValidateEmail"].ToBool(); else ValidateEmail = false;
            if (propertyCollection.ContainsKey("ValidateEqualTo")) ValidateEqualTo = propertyCollection["ValidateEqualTo"].ToString(); else ValidateEqualTo = string.Empty;
            if (propertyCollection.ContainsKey("DecimalPlaces")) DecimalPlaces = propertyCollection["DecimalPlaces"].ToInt(); else DecimalPlaces = 0;
            if (propertyCollection.ContainsKey("Min")) Min = propertyCollection["Min"].ToDecimal(); else Min = 0;
            if (propertyCollection.ContainsKey("Max")) Max = propertyCollection["Max"].ToDecimal(); else Max = 0;
            if (propertyCollection.ContainsKey("Step")) Step = propertyCollection["Step"].ToDecimal(); else Step = 0;
            if (propertyCollection.ContainsKey("StringFormat")) StringFormat = propertyCollection["StringFormat"].ToString(); else StringFormat = string.Empty;
            if (propertyCollection.ContainsKey("Unit")) Unit = propertyCollection["Unit"].ToString(); else Unit = string.Empty;
            if (propertyCollection.ContainsKey("NumFilterMin")) NumFilterMin = propertyCollection["NumFilterMin"].ToDecimal(); else NumFilterMin = 0;
            if (propertyCollection.ContainsKey("NumFilterMax")) NumFilterMax = propertyCollection["NumFilterMax"].ToDecimal(); else NumFilterMax = 0;
            if (propertyCollection.ContainsKey("NumFilterStep")) NumFilterStep = propertyCollection["NumFilterStep"].ToDecimal(); else NumFilterStep = 0;
            if (propertyCollection.ContainsKey("Width")) Width = propertyCollection["Width"].ToInt(); else Width = 0;
            if (propertyCollection.ContainsKey("SettingEnable")) SettingEnable = propertyCollection["SettingEnable"].ToBool(); else SettingEnable = false;
            if (propertyCollection.ContainsKey("OldColumnName")) OldColumnName = propertyCollection["OldColumnName"].ToString(); else OldColumnName = string.Empty;
        }

        public object this[string key]
        {
            get{
                switch(key)
                {
                    case "Id": return Id;
                    case "ModelName": return ModelName;
                    case "TableName": return TableName;
                    case "Base": return Base;
                    case "EachModel": return EachModel;
                    case "Label": return Label;
                    case "ColumnName": return ColumnName;
                    case "ColumnLabel": return ColumnLabel;
                    case "No": return No;
                    case "History": return History;
                    case "Import": return Import;
                    case "Export": return Export;
                    case "GridColumn": return GridColumn;
                    case "GridEnabled": return GridEnabled;
                    case "FilterColumn": return FilterColumn;
                    case "FilterEnabled": return FilterEnabled;
                    case "EditorColumn": return EditorColumn;
                    case "EditorEnabled": return EditorEnabled;
                    case "TitleColumn": return TitleColumn;
                    case "LinkColumn": return LinkColumn;
                    case "LinkEnabled": return LinkEnabled;
                    case "HistoryColumn": return HistoryColumn;
                    case "HistoryEnabled": return HistoryEnabled;
                    case "TypeName": return TypeName;
                    case "TypeCs": return TypeCs;
                    case "RecordingData": return RecordingData;
                    case "MaxLength": return MaxLength;
                    case "Size": return Size;
                    case "Pk": return Pk;
                    case "PkOrderBy": return PkOrderBy;
                    case "PkHistory": return PkHistory;
                    case "PkHistoryOrderBy": return PkHistoryOrderBy;
                    case "Ix1": return Ix1;
                    case "Ix1OrderBy": return Ix1OrderBy;
                    case "Ix2": return Ix2;
                    case "Ix2OrderBy": return Ix2OrderBy;
                    case "Ix3": return Ix3;
                    case "Ix3OrderBy": return Ix3OrderBy;
                    case "Nullable": return Nullable;
                    case "Default": return Default;
                    case "DefaultCs": return DefaultCs;
                    case "Identity": return Identity;
                    case "Unique": return Unique;
                    case "Seed": return Seed;
                    case "JoinTableName": return JoinTableName;
                    case "JoinType": return JoinType;
                    case "JoinExpression": return JoinExpression;
                    case "Like": return Like;
                    case "WhereSpecial": return WhereSpecial;
                    case "ReadPermission": return ReadPermission;
                    case "CreatePermission": return CreatePermission;
                    case "UpdatePermission": return UpdatePermission;
                    case "NotEditSelf": return NotEditSelf;
                    case "SearchIndexPriority": return SearchIndexPriority;
                    case "NotForm": return NotForm;
                    case "NotSelect": return NotSelect;
                    case "NotUpdate": return NotUpdate;
                    case "ByForm": return ByForm;
                    case "ByDataRow": return ByDataRow;
                    case "BySession": return BySession;
                    case "ToControl": return ToControl;
                    case "SelectColumns": return SelectColumns;
                    case "ComputeColumn": return ComputeColumn;
                    case "OrderByColumns": return OrderByColumns;
                    case "ItemId": return ItemId;
                    case "GenericUi": return GenericUi;
                    case "FieldCss": return FieldCss;
                    case "ControlCss": return ControlCss;
                    case "MarkDown": return MarkDown;
                    case "GridStyle": return GridStyle;
                    case "Hash": return Hash;
                    case "Calc": return Calc;
                    case "Session": return Session;
                    case "UserColumn": return UserColumn;
                    case "EnumColumn": return EnumColumn;
                    case "NotEditorSettings": return NotEditorSettings;
                    case "ControlType": return ControlType;
                    case "GridFormat": return GridFormat;
                    case "EditorFormat": return EditorFormat;
                    case "ExportFormat": return ExportFormat;
                    case "Aggregatable": return Aggregatable;
                    case "Computable": return Computable;
                    case "ChoicesText": return ChoicesText;
                    case "UseSearch": return UseSearch;
                    case "DefaultInput": return DefaultInput;
                    case "Own": return Own;
                    case "FormName": return FormName;
                    case "ValidateRequired": return ValidateRequired;
                    case "ValidateNumber": return ValidateNumber;
                    case "ValidateDate": return ValidateDate;
                    case "ValidateEmail": return ValidateEmail;
                    case "ValidateEqualTo": return ValidateEqualTo;
                    case "DecimalPlaces": return DecimalPlaces;
                    case "Min": return Min;
                    case "Max": return Max;
                    case "Step": return Step;
                    case "StringFormat": return StringFormat;
                    case "Unit": return Unit;
                    case "NumFilterMin": return NumFilterMin;
                    case "NumFilterMax": return NumFilterMax;
                    case "NumFilterStep": return NumFilterStep;
                    case "Width": return Width;
                    case "SettingEnable": return SettingEnable;
                    case "OldColumnName": return OldColumnName;
                    default: return null;
                }
            }
        }

        public void RestoreBySavedMemory()
        {
            Id = SavedId;
            ModelName = SavedModelName;
            TableName = SavedTableName;
            Base = SavedBase;
            EachModel = SavedEachModel;
            Label = SavedLabel;
            ColumnName = SavedColumnName;
            ColumnLabel = SavedColumnLabel;
            No = SavedNo;
            History = SavedHistory;
            Import = SavedImport;
            Export = SavedExport;
            GridColumn = SavedGridColumn;
            GridEnabled = SavedGridEnabled;
            FilterColumn = SavedFilterColumn;
            FilterEnabled = SavedFilterEnabled;
            EditorColumn = SavedEditorColumn;
            EditorEnabled = SavedEditorEnabled;
            TitleColumn = SavedTitleColumn;
            LinkColumn = SavedLinkColumn;
            LinkEnabled = SavedLinkEnabled;
            HistoryColumn = SavedHistoryColumn;
            HistoryEnabled = SavedHistoryEnabled;
            TypeName = SavedTypeName;
            TypeCs = SavedTypeCs;
            RecordingData = SavedRecordingData;
            MaxLength = SavedMaxLength;
            Size = SavedSize;
            Pk = SavedPk;
            PkOrderBy = SavedPkOrderBy;
            PkHistory = SavedPkHistory;
            PkHistoryOrderBy = SavedPkHistoryOrderBy;
            Ix1 = SavedIx1;
            Ix1OrderBy = SavedIx1OrderBy;
            Ix2 = SavedIx2;
            Ix2OrderBy = SavedIx2OrderBy;
            Ix3 = SavedIx3;
            Ix3OrderBy = SavedIx3OrderBy;
            Nullable = SavedNullable;
            Default = SavedDefault;
            DefaultCs = SavedDefaultCs;
            Identity = SavedIdentity;
            Unique = SavedUnique;
            Seed = SavedSeed;
            JoinTableName = SavedJoinTableName;
            JoinType = SavedJoinType;
            JoinExpression = SavedJoinExpression;
            Like = SavedLike;
            WhereSpecial = SavedWhereSpecial;
            ReadPermission = SavedReadPermission;
            CreatePermission = SavedCreatePermission;
            UpdatePermission = SavedUpdatePermission;
            NotEditSelf = SavedNotEditSelf;
            SearchIndexPriority = SavedSearchIndexPriority;
            NotForm = SavedNotForm;
            NotSelect = SavedNotSelect;
            NotUpdate = SavedNotUpdate;
            ByForm = SavedByForm;
            ByDataRow = SavedByDataRow;
            BySession = SavedBySession;
            ToControl = SavedToControl;
            SelectColumns = SavedSelectColumns;
            ComputeColumn = SavedComputeColumn;
            OrderByColumns = SavedOrderByColumns;
            ItemId = SavedItemId;
            GenericUi = SavedGenericUi;
            FieldCss = SavedFieldCss;
            ControlCss = SavedControlCss;
            MarkDown = SavedMarkDown;
            GridStyle = SavedGridStyle;
            Hash = SavedHash;
            Calc = SavedCalc;
            Session = SavedSession;
            UserColumn = SavedUserColumn;
            EnumColumn = SavedEnumColumn;
            NotEditorSettings = SavedNotEditorSettings;
            ControlType = SavedControlType;
            GridFormat = SavedGridFormat;
            EditorFormat = SavedEditorFormat;
            ExportFormat = SavedExportFormat;
            Aggregatable = SavedAggregatable;
            Computable = SavedComputable;
            ChoicesText = SavedChoicesText;
            UseSearch = SavedUseSearch;
            DefaultInput = SavedDefaultInput;
            Own = SavedOwn;
            FormName = SavedFormName;
            ValidateRequired = SavedValidateRequired;
            ValidateNumber = SavedValidateNumber;
            ValidateDate = SavedValidateDate;
            ValidateEmail = SavedValidateEmail;
            ValidateEqualTo = SavedValidateEqualTo;
            DecimalPlaces = SavedDecimalPlaces;
            Min = SavedMin;
            Max = SavedMax;
            Step = SavedStep;
            StringFormat = SavedStringFormat;
            Unit = SavedUnit;
            NumFilterMin = SavedNumFilterMin;
            NumFilterMax = SavedNumFilterMax;
            NumFilterStep = SavedNumFilterStep;
            Width = SavedWidth;
            SettingEnable = SavedSettingEnable;
            OldColumnName = SavedOldColumnName;
        }
    }

    public class ColumnColumn2nd
    {
        public string _BaseItems_SiteId;
        public string _BaseItems_UpdatedTime;
        public string _BaseItems_Title;
        public string _BaseItems_Body;
        public string _BaseItems_TitleBody;
        public string _Bases_Ver;
        public string _Bases_Comments;
        public string _Bases_Creator;
        public string _Bases_Updator;
        public string _Bases_CreatedTime;
        public string _Bases_UpdatedTime;
        public string _Bases_VerUp;
        public string _Bases_Timestamp;
        public string Tenants_TenantId;
        public string Tenants_TenantName;
        public string Tenants_Title;
        public string Tenants_Body;
        public string Demos_DemoId;
        public string Demos_TenantId;
        public string Demos_Title;
        public string Demos_Passphrase;
        public string Demos_MailAddress;
        public string Demos_Initialized;
        public string Demos_TimeLag;
        public string SysLogs_CreatedTime;
        public string SysLogs_SysLogId;
        public string SysLogs_StartTime;
        public string SysLogs_EndTime;
        public string SysLogs_SysLogType;
        public string SysLogs_OnAzure;
        public string SysLogs_MachineName;
        public string SysLogs_ServiceName;
        public string SysLogs_TenantName;
        public string SysLogs_Application;
        public string SysLogs_Class;
        public string SysLogs_Method;
        public string SysLogs_RequestData;
        public string SysLogs_HttpMethod;
        public string SysLogs_RequestSize;
        public string SysLogs_ResponseSize;
        public string SysLogs_Elapsed;
        public string SysLogs_ApplicationAge;
        public string SysLogs_ApplicationRequestInterval;
        public string SysLogs_SessionAge;
        public string SysLogs_SessionRequestInterval;
        public string SysLogs_WorkingSet64;
        public string SysLogs_VirtualMemorySize64;
        public string SysLogs_ProcessId;
        public string SysLogs_ProcessName;
        public string SysLogs_BasePriority;
        public string SysLogs_Url;
        public string SysLogs_UrlReferer;
        public string SysLogs_UserHostName;
        public string SysLogs_UserHostAddress;
        public string SysLogs_UserLanguage;
        public string SysLogs_UserAgent;
        public string SysLogs_SessionGuid;
        public string SysLogs_ErrMessage;
        public string SysLogs_ErrStackTrace;
        public string SysLogs_Title;
        public string SysLogs_InDebug;
        public string SysLogs_AssemblyVersion;
        public string Depts_TenantId;
        public string Depts_DeptId;
        public string Depts_DeptCode;
        public string Depts_Dept;
        public string Depts_DeptName;
        public string Depts_Body;
        public string Depts_Title;
        public string Groups_TenantId;
        public string Groups_GroupId;
        public string Groups_GroupName;
        public string Groups_Body;
        public string Groups_Title;
        public string GroupMembers_GroupId;
        public string GroupMembers_DeptId;
        public string GroupMembers_UserId;
        public string GroupMembers_Admin;
        public string Users_TenantId;
        public string Users_UserId;
        public string Users_LoginId;
        public string Users_Disabled;
        public string Users_UserCode;
        public string Users_Password;
        public string Users_PasswordValidate;
        public string Users_PasswordDummy;
        public string Users_RememberMe;
        public string Users_LastName;
        public string Users_FirstName;
        public string Users_FullName1;
        public string Users_FullName2;
        public string Users_Birthday;
        public string Users_Gender;
        public string Users_Language;
        public string Users_TimeZone;
        public string Users_TimeZoneInfo;
        public string Users_DeptId;
        public string Users_Dept;
        public string Users_FirstAndLastNameOrder;
        public string Users_Title;
        public string Users_LastLoginTime;
        public string Users_PasswordExpirationTime;
        public string Users_PasswordChangeTime;
        public string Users_NumberOfLogins;
        public string Users_NumberOfDenial;
        public string Users_TenantManager;
        public string Users_ServiceManager;
        public string Users_Developer;
        public string Users_OldPassword;
        public string Users_ChangedPassword;
        public string Users_ChangedPasswordValidator;
        public string Users_AfterResetPassword;
        public string Users_AfterResetPasswordValidator;
        public string Users_MailAddresses;
        public string Users_DemoMailAddress;
        public string Users_SessionGuid;
        public string MailAddresses_OwnerId;
        public string MailAddresses_OwnerType;
        public string MailAddresses_MailAddressId;
        public string MailAddresses_MailAddress;
        public string MailAddresses_Title;
        public string Permissions_ReferenceType;
        public string Permissions_ReferenceId;
        public string Permissions_DeptId;
        public string Permissions_GroupId;
        public string Permissions_UserId;
        public string Permissions_DeptName;
        public string Permissions_GroupName;
        public string Permissions_FullName1;
        public string Permissions_FullName2;
        public string Permissions_FirstAndLastNameOrder;
        public string Permissions_PermissionType;
        public string OutgoingMails_ReferenceType;
        public string OutgoingMails_ReferenceId;
        public string OutgoingMails_ReferenceVer;
        public string OutgoingMails_OutgoingMailId;
        public string OutgoingMails_Host;
        public string OutgoingMails_Port;
        public string OutgoingMails_From;
        public string OutgoingMails_To;
        public string OutgoingMails_Cc;
        public string OutgoingMails_Bcc;
        public string OutgoingMails_Title;
        public string OutgoingMails_Body;
        public string OutgoingMails_SentTime;
        public string OutgoingMails_DestinationSearchRange;
        public string OutgoingMails_DestinationSearchText;
        public string SearchIndexes_Word;
        public string SearchIndexes_ReferenceId;
        public string SearchIndexes_Priority;
        public string SearchIndexes_ReferenceType;
        public string SearchIndexes_Title;
        public string SearchIndexes_Subset;
        public string SearchIndexes_PermissionType;
        public string Items_ReferenceId;
        public string Items_ReferenceType;
        public string Items_SiteId;
        public string Items_Title;
        public string Items_Site;
        public string Sites_TenantId;
        public string Sites_SiteId;
        public string Sites_ReferenceType;
        public string Sites_ParentId;
        public string Sites_InheritPermission;
        public string Sites_SiteSettings;
        public string Sites_Ancestors;
        public string Sites_PermissionSourceCollection;
        public string Sites_PermissionDestinationCollection;
        public string Sites_SiteMenu;
        public string Sites_MonitorChangesColumns;
        public string Sites_TitleColumns;
        public string Orders_ReferenceId;
        public string Orders_ReferenceType;
        public string Orders_OwnerId;
        public string Orders_Data;
        public string ExportSettings_ReferenceType;
        public string ExportSettings_ReferenceId;
        public string ExportSettings_Title;
        public string ExportSettings_ExportSettingId;
        public string ExportSettings_AddHeader;
        public string ExportSettings_ExportColumns;
        public string Links_DestinationId;
        public string Links_SourceId;
        public string Links_ReferenceType;
        public string Links_SiteId;
        public string Links_Title;
        public string Links_Subset;
        public string Links_SiteTitle;
        public string Binaries_ReferenceId;
        public string Binaries_BinaryId;
        public string Binaries_BinaryType;
        public string Binaries_Title;
        public string Binaries_Body;
        public string Binaries_Bin;
        public string Binaries_Thumbnail;
        public string Binaries_Icon;
        public string Binaries_FileName;
        public string Binaries_Extension;
        public string Binaries_Size;
        public string Binaries_BinarySettings;
        public string Issues_IssueId;
        public string Issues_StartTime;
        public string Issues_CompletionTime;
        public string Issues_WorkValue;
        public string Issues_ProgressRate;
        public string Issues_RemainingWorkValue;
        public string Issues_Status;
        public string Issues_Manager;
        public string Issues_Owner;
        public string Issues_ClassA;
        public string Issues_ClassB;
        public string Issues_ClassC;
        public string Issues_ClassD;
        public string Issues_ClassE;
        public string Issues_ClassF;
        public string Issues_ClassG;
        public string Issues_ClassH;
        public string Issues_ClassI;
        public string Issues_ClassJ;
        public string Issues_ClassK;
        public string Issues_ClassL;
        public string Issues_ClassM;
        public string Issues_ClassN;
        public string Issues_ClassO;
        public string Issues_ClassP;
        public string Issues_ClassQ;
        public string Issues_ClassR;
        public string Issues_ClassS;
        public string Issues_ClassT;
        public string Issues_ClassU;
        public string Issues_ClassV;
        public string Issues_ClassW;
        public string Issues_ClassX;
        public string Issues_ClassY;
        public string Issues_ClassZ;
        public string Issues_NumA;
        public string Issues_NumB;
        public string Issues_NumC;
        public string Issues_NumD;
        public string Issues_NumE;
        public string Issues_NumF;
        public string Issues_NumG;
        public string Issues_NumH;
        public string Issues_NumI;
        public string Issues_NumJ;
        public string Issues_NumK;
        public string Issues_NumL;
        public string Issues_NumM;
        public string Issues_NumN;
        public string Issues_NumO;
        public string Issues_NumP;
        public string Issues_NumQ;
        public string Issues_NumR;
        public string Issues_NumS;
        public string Issues_NumT;
        public string Issues_NumU;
        public string Issues_NumV;
        public string Issues_NumW;
        public string Issues_NumX;
        public string Issues_NumY;
        public string Issues_NumZ;
        public string Issues_DateA;
        public string Issues_DateB;
        public string Issues_DateC;
        public string Issues_DateD;
        public string Issues_DateE;
        public string Issues_DateF;
        public string Issues_DateG;
        public string Issues_DateH;
        public string Issues_DateI;
        public string Issues_DateJ;
        public string Issues_DateK;
        public string Issues_DateL;
        public string Issues_DateM;
        public string Issues_DateN;
        public string Issues_DateO;
        public string Issues_DateP;
        public string Issues_DateQ;
        public string Issues_DateR;
        public string Issues_DateS;
        public string Issues_DateT;
        public string Issues_DateU;
        public string Issues_DateV;
        public string Issues_DateW;
        public string Issues_DateX;
        public string Issues_DateY;
        public string Issues_DateZ;
        public string Issues_DescriptionA;
        public string Issues_DescriptionB;
        public string Issues_DescriptionC;
        public string Issues_DescriptionD;
        public string Issues_DescriptionE;
        public string Issues_DescriptionF;
        public string Issues_DescriptionG;
        public string Issues_DescriptionH;
        public string Issues_DescriptionI;
        public string Issues_DescriptionJ;
        public string Issues_DescriptionK;
        public string Issues_DescriptionL;
        public string Issues_DescriptionM;
        public string Issues_DescriptionN;
        public string Issues_DescriptionO;
        public string Issues_DescriptionP;
        public string Issues_DescriptionQ;
        public string Issues_DescriptionR;
        public string Issues_DescriptionS;
        public string Issues_DescriptionT;
        public string Issues_DescriptionU;
        public string Issues_DescriptionV;
        public string Issues_DescriptionW;
        public string Issues_DescriptionX;
        public string Issues_DescriptionY;
        public string Issues_DescriptionZ;
        public string Issues_CheckA;
        public string Issues_CheckB;
        public string Issues_CheckC;
        public string Issues_CheckD;
        public string Issues_CheckE;
        public string Issues_CheckF;
        public string Issues_CheckG;
        public string Issues_CheckH;
        public string Issues_CheckI;
        public string Issues_CheckJ;
        public string Issues_CheckK;
        public string Issues_CheckL;
        public string Issues_CheckM;
        public string Issues_CheckN;
        public string Issues_CheckO;
        public string Issues_CheckP;
        public string Issues_CheckQ;
        public string Issues_CheckR;
        public string Issues_CheckS;
        public string Issues_CheckT;
        public string Issues_CheckU;
        public string Issues_CheckV;
        public string Issues_CheckW;
        public string Issues_CheckX;
        public string Issues_CheckY;
        public string Issues_CheckZ;
        public string Results_ResultId;
        public string Results_Title;
        public string Results_Status;
        public string Results_Manager;
        public string Results_Owner;
        public string Results_ClassA;
        public string Results_ClassB;
        public string Results_ClassC;
        public string Results_ClassD;
        public string Results_ClassE;
        public string Results_ClassF;
        public string Results_ClassG;
        public string Results_ClassH;
        public string Results_ClassI;
        public string Results_ClassJ;
        public string Results_ClassK;
        public string Results_ClassL;
        public string Results_ClassM;
        public string Results_ClassN;
        public string Results_ClassO;
        public string Results_ClassP;
        public string Results_ClassQ;
        public string Results_ClassR;
        public string Results_ClassS;
        public string Results_ClassT;
        public string Results_ClassU;
        public string Results_ClassV;
        public string Results_ClassW;
        public string Results_ClassX;
        public string Results_ClassY;
        public string Results_ClassZ;
        public string Results_NumA;
        public string Results_NumB;
        public string Results_NumC;
        public string Results_NumD;
        public string Results_NumE;
        public string Results_NumF;
        public string Results_NumG;
        public string Results_NumH;
        public string Results_NumI;
        public string Results_NumJ;
        public string Results_NumK;
        public string Results_NumL;
        public string Results_NumM;
        public string Results_NumN;
        public string Results_NumO;
        public string Results_NumP;
        public string Results_NumQ;
        public string Results_NumR;
        public string Results_NumS;
        public string Results_NumT;
        public string Results_NumU;
        public string Results_NumV;
        public string Results_NumW;
        public string Results_NumX;
        public string Results_NumY;
        public string Results_NumZ;
        public string Results_DateA;
        public string Results_DateB;
        public string Results_DateC;
        public string Results_DateD;
        public string Results_DateE;
        public string Results_DateF;
        public string Results_DateG;
        public string Results_DateH;
        public string Results_DateI;
        public string Results_DateJ;
        public string Results_DateK;
        public string Results_DateL;
        public string Results_DateM;
        public string Results_DateN;
        public string Results_DateO;
        public string Results_DateP;
        public string Results_DateQ;
        public string Results_DateR;
        public string Results_DateS;
        public string Results_DateT;
        public string Results_DateU;
        public string Results_DateV;
        public string Results_DateW;
        public string Results_DateX;
        public string Results_DateY;
        public string Results_DateZ;
        public string Results_DescriptionA;
        public string Results_DescriptionB;
        public string Results_DescriptionC;
        public string Results_DescriptionD;
        public string Results_DescriptionE;
        public string Results_DescriptionF;
        public string Results_DescriptionG;
        public string Results_DescriptionH;
        public string Results_DescriptionI;
        public string Results_DescriptionJ;
        public string Results_DescriptionK;
        public string Results_DescriptionL;
        public string Results_DescriptionM;
        public string Results_DescriptionN;
        public string Results_DescriptionO;
        public string Results_DescriptionP;
        public string Results_DescriptionQ;
        public string Results_DescriptionR;
        public string Results_DescriptionS;
        public string Results_DescriptionT;
        public string Results_DescriptionU;
        public string Results_DescriptionV;
        public string Results_DescriptionW;
        public string Results_DescriptionX;
        public string Results_DescriptionY;
        public string Results_DescriptionZ;
        public string Results_CheckA;
        public string Results_CheckB;
        public string Results_CheckC;
        public string Results_CheckD;
        public string Results_CheckE;
        public string Results_CheckF;
        public string Results_CheckG;
        public string Results_CheckH;
        public string Results_CheckI;
        public string Results_CheckJ;
        public string Results_CheckK;
        public string Results_CheckL;
        public string Results_CheckM;
        public string Results_CheckN;
        public string Results_CheckO;
        public string Results_CheckP;
        public string Results_CheckQ;
        public string Results_CheckR;
        public string Results_CheckS;
        public string Results_CheckT;
        public string Results_CheckU;
        public string Results_CheckV;
        public string Results_CheckW;
        public string Results_CheckX;
        public string Results_CheckY;
        public string Results_CheckZ;
        public string Wikis_WikiId;
        public string Tenants_Ver;
        public string Tenants_Comments;
        public string Tenants_Creator;
        public string Tenants_Updator;
        public string Tenants_CreatedTime;
        public string Tenants_UpdatedTime;
        public string Tenants_VerUp;
        public string Tenants_Timestamp;
        public string Demos_Ver;
        public string Demos_Comments;
        public string Demos_Creator;
        public string Demos_Updator;
        public string Demos_CreatedTime;
        public string Demos_UpdatedTime;
        public string Demos_VerUp;
        public string Demos_Timestamp;
        public string SysLogs_Ver;
        public string SysLogs_Comments;
        public string SysLogs_Creator;
        public string SysLogs_Updator;
        public string SysLogs_UpdatedTime;
        public string SysLogs_VerUp;
        public string SysLogs_Timestamp;
        public string Depts_Ver;
        public string Depts_Comments;
        public string Depts_Creator;
        public string Depts_Updator;
        public string Depts_CreatedTime;
        public string Depts_UpdatedTime;
        public string Depts_VerUp;
        public string Depts_Timestamp;
        public string Groups_Ver;
        public string Groups_Comments;
        public string Groups_Creator;
        public string Groups_Updator;
        public string Groups_CreatedTime;
        public string Groups_UpdatedTime;
        public string Groups_VerUp;
        public string Groups_Timestamp;
        public string GroupMembers_Ver;
        public string GroupMembers_Comments;
        public string GroupMembers_Creator;
        public string GroupMembers_Updator;
        public string GroupMembers_CreatedTime;
        public string GroupMembers_UpdatedTime;
        public string GroupMembers_VerUp;
        public string GroupMembers_Timestamp;
        public string Users_Ver;
        public string Users_Comments;
        public string Users_Creator;
        public string Users_Updator;
        public string Users_CreatedTime;
        public string Users_UpdatedTime;
        public string Users_VerUp;
        public string Users_Timestamp;
        public string MailAddresses_Ver;
        public string MailAddresses_Comments;
        public string MailAddresses_Creator;
        public string MailAddresses_Updator;
        public string MailAddresses_CreatedTime;
        public string MailAddresses_UpdatedTime;
        public string MailAddresses_VerUp;
        public string MailAddresses_Timestamp;
        public string Permissions_Ver;
        public string Permissions_Comments;
        public string Permissions_Creator;
        public string Permissions_Updator;
        public string Permissions_CreatedTime;
        public string Permissions_UpdatedTime;
        public string Permissions_VerUp;
        public string Permissions_Timestamp;
        public string OutgoingMails_Ver;
        public string OutgoingMails_Comments;
        public string OutgoingMails_Creator;
        public string OutgoingMails_Updator;
        public string OutgoingMails_CreatedTime;
        public string OutgoingMails_UpdatedTime;
        public string OutgoingMails_VerUp;
        public string OutgoingMails_Timestamp;
        public string SearchIndexes_Ver;
        public string SearchIndexes_Comments;
        public string SearchIndexes_Creator;
        public string SearchIndexes_Updator;
        public string SearchIndexes_CreatedTime;
        public string SearchIndexes_UpdatedTime;
        public string SearchIndexes_VerUp;
        public string SearchIndexes_Timestamp;
        public string Items_Ver;
        public string Items_Comments;
        public string Items_Creator;
        public string Items_Updator;
        public string Items_CreatedTime;
        public string Items_UpdatedTime;
        public string Items_VerUp;
        public string Items_Timestamp;
        public string Sites_UpdatedTime;
        public string Sites_Title;
        public string Sites_Body;
        public string Sites_TitleBody;
        public string Sites_Ver;
        public string Sites_Comments;
        public string Sites_Creator;
        public string Sites_Updator;
        public string Sites_CreatedTime;
        public string Sites_VerUp;
        public string Sites_Timestamp;
        public string Orders_Ver;
        public string Orders_Comments;
        public string Orders_Creator;
        public string Orders_Updator;
        public string Orders_CreatedTime;
        public string Orders_UpdatedTime;
        public string Orders_VerUp;
        public string Orders_Timestamp;
        public string ExportSettings_Ver;
        public string ExportSettings_Comments;
        public string ExportSettings_Creator;
        public string ExportSettings_Updator;
        public string ExportSettings_CreatedTime;
        public string ExportSettings_UpdatedTime;
        public string ExportSettings_VerUp;
        public string ExportSettings_Timestamp;
        public string Links_Ver;
        public string Links_Comments;
        public string Links_Creator;
        public string Links_Updator;
        public string Links_CreatedTime;
        public string Links_UpdatedTime;
        public string Links_VerUp;
        public string Links_Timestamp;
        public string Binaries_Ver;
        public string Binaries_Comments;
        public string Binaries_Creator;
        public string Binaries_Updator;
        public string Binaries_CreatedTime;
        public string Binaries_UpdatedTime;
        public string Binaries_VerUp;
        public string Binaries_Timestamp;
        public string Issues_SiteId;
        public string Issues_UpdatedTime;
        public string Issues_Title;
        public string Issues_Body;
        public string Issues_TitleBody;
        public string Issues_Ver;
        public string Issues_Comments;
        public string Issues_Creator;
        public string Issues_Updator;
        public string Issues_CreatedTime;
        public string Issues_VerUp;
        public string Issues_Timestamp;
        public string Results_SiteId;
        public string Results_UpdatedTime;
        public string Results_Body;
        public string Results_TitleBody;
        public string Results_Ver;
        public string Results_Comments;
        public string Results_Creator;
        public string Results_Updator;
        public string Results_CreatedTime;
        public string Results_VerUp;
        public string Results_Timestamp;
        public string Wikis_SiteId;
        public string Wikis_UpdatedTime;
        public string Wikis_Title;
        public string Wikis_Body;
        public string Wikis_TitleBody;
        public string Wikis_Ver;
        public string Wikis_Comments;
        public string Wikis_Creator;
        public string Wikis_Updator;
        public string Wikis_CreatedTime;
        public string Wikis_VerUp;
        public string Wikis_Timestamp;
    }

    public class ColumnTable
    {
        public ColumnDefinition _BaseItems_SiteId = new ColumnDefinition();
        public ColumnDefinition _BaseItems_UpdatedTime = new ColumnDefinition();
        public ColumnDefinition _BaseItems_Title = new ColumnDefinition();
        public ColumnDefinition _BaseItems_Body = new ColumnDefinition();
        public ColumnDefinition _BaseItems_TitleBody = new ColumnDefinition();
        public ColumnDefinition _Bases_Ver = new ColumnDefinition();
        public ColumnDefinition _Bases_Comments = new ColumnDefinition();
        public ColumnDefinition _Bases_Creator = new ColumnDefinition();
        public ColumnDefinition _Bases_Updator = new ColumnDefinition();
        public ColumnDefinition _Bases_CreatedTime = new ColumnDefinition();
        public ColumnDefinition _Bases_UpdatedTime = new ColumnDefinition();
        public ColumnDefinition _Bases_VerUp = new ColumnDefinition();
        public ColumnDefinition _Bases_Timestamp = new ColumnDefinition();
        public ColumnDefinition Tenants_TenantId = new ColumnDefinition();
        public ColumnDefinition Tenants_TenantName = new ColumnDefinition();
        public ColumnDefinition Tenants_Title = new ColumnDefinition();
        public ColumnDefinition Tenants_Body = new ColumnDefinition();
        public ColumnDefinition Demos_DemoId = new ColumnDefinition();
        public ColumnDefinition Demos_TenantId = new ColumnDefinition();
        public ColumnDefinition Demos_Title = new ColumnDefinition();
        public ColumnDefinition Demos_Passphrase = new ColumnDefinition();
        public ColumnDefinition Demos_MailAddress = new ColumnDefinition();
        public ColumnDefinition Demos_Initialized = new ColumnDefinition();
        public ColumnDefinition Demos_TimeLag = new ColumnDefinition();
        public ColumnDefinition SysLogs_CreatedTime = new ColumnDefinition();
        public ColumnDefinition SysLogs_SysLogId = new ColumnDefinition();
        public ColumnDefinition SysLogs_StartTime = new ColumnDefinition();
        public ColumnDefinition SysLogs_EndTime = new ColumnDefinition();
        public ColumnDefinition SysLogs_SysLogType = new ColumnDefinition();
        public ColumnDefinition SysLogs_OnAzure = new ColumnDefinition();
        public ColumnDefinition SysLogs_MachineName = new ColumnDefinition();
        public ColumnDefinition SysLogs_ServiceName = new ColumnDefinition();
        public ColumnDefinition SysLogs_TenantName = new ColumnDefinition();
        public ColumnDefinition SysLogs_Application = new ColumnDefinition();
        public ColumnDefinition SysLogs_Class = new ColumnDefinition();
        public ColumnDefinition SysLogs_Method = new ColumnDefinition();
        public ColumnDefinition SysLogs_RequestData = new ColumnDefinition();
        public ColumnDefinition SysLogs_HttpMethod = new ColumnDefinition();
        public ColumnDefinition SysLogs_RequestSize = new ColumnDefinition();
        public ColumnDefinition SysLogs_ResponseSize = new ColumnDefinition();
        public ColumnDefinition SysLogs_Elapsed = new ColumnDefinition();
        public ColumnDefinition SysLogs_ApplicationAge = new ColumnDefinition();
        public ColumnDefinition SysLogs_ApplicationRequestInterval = new ColumnDefinition();
        public ColumnDefinition SysLogs_SessionAge = new ColumnDefinition();
        public ColumnDefinition SysLogs_SessionRequestInterval = new ColumnDefinition();
        public ColumnDefinition SysLogs_WorkingSet64 = new ColumnDefinition();
        public ColumnDefinition SysLogs_VirtualMemorySize64 = new ColumnDefinition();
        public ColumnDefinition SysLogs_ProcessId = new ColumnDefinition();
        public ColumnDefinition SysLogs_ProcessName = new ColumnDefinition();
        public ColumnDefinition SysLogs_BasePriority = new ColumnDefinition();
        public ColumnDefinition SysLogs_Url = new ColumnDefinition();
        public ColumnDefinition SysLogs_UrlReferer = new ColumnDefinition();
        public ColumnDefinition SysLogs_UserHostName = new ColumnDefinition();
        public ColumnDefinition SysLogs_UserHostAddress = new ColumnDefinition();
        public ColumnDefinition SysLogs_UserLanguage = new ColumnDefinition();
        public ColumnDefinition SysLogs_UserAgent = new ColumnDefinition();
        public ColumnDefinition SysLogs_SessionGuid = new ColumnDefinition();
        public ColumnDefinition SysLogs_ErrMessage = new ColumnDefinition();
        public ColumnDefinition SysLogs_ErrStackTrace = new ColumnDefinition();
        public ColumnDefinition SysLogs_Title = new ColumnDefinition();
        public ColumnDefinition SysLogs_InDebug = new ColumnDefinition();
        public ColumnDefinition SysLogs_AssemblyVersion = new ColumnDefinition();
        public ColumnDefinition Depts_TenantId = new ColumnDefinition();
        public ColumnDefinition Depts_DeptId = new ColumnDefinition();
        public ColumnDefinition Depts_DeptCode = new ColumnDefinition();
        public ColumnDefinition Depts_Dept = new ColumnDefinition();
        public ColumnDefinition Depts_DeptName = new ColumnDefinition();
        public ColumnDefinition Depts_Body = new ColumnDefinition();
        public ColumnDefinition Depts_Title = new ColumnDefinition();
        public ColumnDefinition Groups_TenantId = new ColumnDefinition();
        public ColumnDefinition Groups_GroupId = new ColumnDefinition();
        public ColumnDefinition Groups_GroupName = new ColumnDefinition();
        public ColumnDefinition Groups_Body = new ColumnDefinition();
        public ColumnDefinition Groups_Title = new ColumnDefinition();
        public ColumnDefinition GroupMembers_GroupId = new ColumnDefinition();
        public ColumnDefinition GroupMembers_DeptId = new ColumnDefinition();
        public ColumnDefinition GroupMembers_UserId = new ColumnDefinition();
        public ColumnDefinition GroupMembers_Admin = new ColumnDefinition();
        public ColumnDefinition Users_TenantId = new ColumnDefinition();
        public ColumnDefinition Users_UserId = new ColumnDefinition();
        public ColumnDefinition Users_LoginId = new ColumnDefinition();
        public ColumnDefinition Users_Disabled = new ColumnDefinition();
        public ColumnDefinition Users_UserCode = new ColumnDefinition();
        public ColumnDefinition Users_Password = new ColumnDefinition();
        public ColumnDefinition Users_PasswordValidate = new ColumnDefinition();
        public ColumnDefinition Users_PasswordDummy = new ColumnDefinition();
        public ColumnDefinition Users_RememberMe = new ColumnDefinition();
        public ColumnDefinition Users_LastName = new ColumnDefinition();
        public ColumnDefinition Users_FirstName = new ColumnDefinition();
        public ColumnDefinition Users_FullName1 = new ColumnDefinition();
        public ColumnDefinition Users_FullName2 = new ColumnDefinition();
        public ColumnDefinition Users_Birthday = new ColumnDefinition();
        public ColumnDefinition Users_Gender = new ColumnDefinition();
        public ColumnDefinition Users_Language = new ColumnDefinition();
        public ColumnDefinition Users_TimeZone = new ColumnDefinition();
        public ColumnDefinition Users_TimeZoneInfo = new ColumnDefinition();
        public ColumnDefinition Users_DeptId = new ColumnDefinition();
        public ColumnDefinition Users_Dept = new ColumnDefinition();
        public ColumnDefinition Users_FirstAndLastNameOrder = new ColumnDefinition();
        public ColumnDefinition Users_Title = new ColumnDefinition();
        public ColumnDefinition Users_LastLoginTime = new ColumnDefinition();
        public ColumnDefinition Users_PasswordExpirationTime = new ColumnDefinition();
        public ColumnDefinition Users_PasswordChangeTime = new ColumnDefinition();
        public ColumnDefinition Users_NumberOfLogins = new ColumnDefinition();
        public ColumnDefinition Users_NumberOfDenial = new ColumnDefinition();
        public ColumnDefinition Users_TenantManager = new ColumnDefinition();
        public ColumnDefinition Users_ServiceManager = new ColumnDefinition();
        public ColumnDefinition Users_Developer = new ColumnDefinition();
        public ColumnDefinition Users_OldPassword = new ColumnDefinition();
        public ColumnDefinition Users_ChangedPassword = new ColumnDefinition();
        public ColumnDefinition Users_ChangedPasswordValidator = new ColumnDefinition();
        public ColumnDefinition Users_AfterResetPassword = new ColumnDefinition();
        public ColumnDefinition Users_AfterResetPasswordValidator = new ColumnDefinition();
        public ColumnDefinition Users_MailAddresses = new ColumnDefinition();
        public ColumnDefinition Users_DemoMailAddress = new ColumnDefinition();
        public ColumnDefinition Users_SessionGuid = new ColumnDefinition();
        public ColumnDefinition MailAddresses_OwnerId = new ColumnDefinition();
        public ColumnDefinition MailAddresses_OwnerType = new ColumnDefinition();
        public ColumnDefinition MailAddresses_MailAddressId = new ColumnDefinition();
        public ColumnDefinition MailAddresses_MailAddress = new ColumnDefinition();
        public ColumnDefinition MailAddresses_Title = new ColumnDefinition();
        public ColumnDefinition Permissions_ReferenceType = new ColumnDefinition();
        public ColumnDefinition Permissions_ReferenceId = new ColumnDefinition();
        public ColumnDefinition Permissions_DeptId = new ColumnDefinition();
        public ColumnDefinition Permissions_GroupId = new ColumnDefinition();
        public ColumnDefinition Permissions_UserId = new ColumnDefinition();
        public ColumnDefinition Permissions_DeptName = new ColumnDefinition();
        public ColumnDefinition Permissions_GroupName = new ColumnDefinition();
        public ColumnDefinition Permissions_FullName1 = new ColumnDefinition();
        public ColumnDefinition Permissions_FullName2 = new ColumnDefinition();
        public ColumnDefinition Permissions_FirstAndLastNameOrder = new ColumnDefinition();
        public ColumnDefinition Permissions_PermissionType = new ColumnDefinition();
        public ColumnDefinition OutgoingMails_ReferenceType = new ColumnDefinition();
        public ColumnDefinition OutgoingMails_ReferenceId = new ColumnDefinition();
        public ColumnDefinition OutgoingMails_ReferenceVer = new ColumnDefinition();
        public ColumnDefinition OutgoingMails_OutgoingMailId = new ColumnDefinition();
        public ColumnDefinition OutgoingMails_Host = new ColumnDefinition();
        public ColumnDefinition OutgoingMails_Port = new ColumnDefinition();
        public ColumnDefinition OutgoingMails_From = new ColumnDefinition();
        public ColumnDefinition OutgoingMails_To = new ColumnDefinition();
        public ColumnDefinition OutgoingMails_Cc = new ColumnDefinition();
        public ColumnDefinition OutgoingMails_Bcc = new ColumnDefinition();
        public ColumnDefinition OutgoingMails_Title = new ColumnDefinition();
        public ColumnDefinition OutgoingMails_Body = new ColumnDefinition();
        public ColumnDefinition OutgoingMails_SentTime = new ColumnDefinition();
        public ColumnDefinition OutgoingMails_DestinationSearchRange = new ColumnDefinition();
        public ColumnDefinition OutgoingMails_DestinationSearchText = new ColumnDefinition();
        public ColumnDefinition SearchIndexes_Word = new ColumnDefinition();
        public ColumnDefinition SearchIndexes_ReferenceId = new ColumnDefinition();
        public ColumnDefinition SearchIndexes_Priority = new ColumnDefinition();
        public ColumnDefinition SearchIndexes_ReferenceType = new ColumnDefinition();
        public ColumnDefinition SearchIndexes_Title = new ColumnDefinition();
        public ColumnDefinition SearchIndexes_Subset = new ColumnDefinition();
        public ColumnDefinition SearchIndexes_PermissionType = new ColumnDefinition();
        public ColumnDefinition Items_ReferenceId = new ColumnDefinition();
        public ColumnDefinition Items_ReferenceType = new ColumnDefinition();
        public ColumnDefinition Items_SiteId = new ColumnDefinition();
        public ColumnDefinition Items_Title = new ColumnDefinition();
        public ColumnDefinition Items_Site = new ColumnDefinition();
        public ColumnDefinition Sites_TenantId = new ColumnDefinition();
        public ColumnDefinition Sites_SiteId = new ColumnDefinition();
        public ColumnDefinition Sites_ReferenceType = new ColumnDefinition();
        public ColumnDefinition Sites_ParentId = new ColumnDefinition();
        public ColumnDefinition Sites_InheritPermission = new ColumnDefinition();
        public ColumnDefinition Sites_SiteSettings = new ColumnDefinition();
        public ColumnDefinition Sites_Ancestors = new ColumnDefinition();
        public ColumnDefinition Sites_PermissionSourceCollection = new ColumnDefinition();
        public ColumnDefinition Sites_PermissionDestinationCollection = new ColumnDefinition();
        public ColumnDefinition Sites_SiteMenu = new ColumnDefinition();
        public ColumnDefinition Sites_MonitorChangesColumns = new ColumnDefinition();
        public ColumnDefinition Sites_TitleColumns = new ColumnDefinition();
        public ColumnDefinition Orders_ReferenceId = new ColumnDefinition();
        public ColumnDefinition Orders_ReferenceType = new ColumnDefinition();
        public ColumnDefinition Orders_OwnerId = new ColumnDefinition();
        public ColumnDefinition Orders_Data = new ColumnDefinition();
        public ColumnDefinition ExportSettings_ReferenceType = new ColumnDefinition();
        public ColumnDefinition ExportSettings_ReferenceId = new ColumnDefinition();
        public ColumnDefinition ExportSettings_Title = new ColumnDefinition();
        public ColumnDefinition ExportSettings_ExportSettingId = new ColumnDefinition();
        public ColumnDefinition ExportSettings_AddHeader = new ColumnDefinition();
        public ColumnDefinition ExportSettings_ExportColumns = new ColumnDefinition();
        public ColumnDefinition Links_DestinationId = new ColumnDefinition();
        public ColumnDefinition Links_SourceId = new ColumnDefinition();
        public ColumnDefinition Links_ReferenceType = new ColumnDefinition();
        public ColumnDefinition Links_SiteId = new ColumnDefinition();
        public ColumnDefinition Links_Title = new ColumnDefinition();
        public ColumnDefinition Links_Subset = new ColumnDefinition();
        public ColumnDefinition Links_SiteTitle = new ColumnDefinition();
        public ColumnDefinition Binaries_ReferenceId = new ColumnDefinition();
        public ColumnDefinition Binaries_BinaryId = new ColumnDefinition();
        public ColumnDefinition Binaries_BinaryType = new ColumnDefinition();
        public ColumnDefinition Binaries_Title = new ColumnDefinition();
        public ColumnDefinition Binaries_Body = new ColumnDefinition();
        public ColumnDefinition Binaries_Bin = new ColumnDefinition();
        public ColumnDefinition Binaries_Thumbnail = new ColumnDefinition();
        public ColumnDefinition Binaries_Icon = new ColumnDefinition();
        public ColumnDefinition Binaries_FileName = new ColumnDefinition();
        public ColumnDefinition Binaries_Extension = new ColumnDefinition();
        public ColumnDefinition Binaries_Size = new ColumnDefinition();
        public ColumnDefinition Binaries_BinarySettings = new ColumnDefinition();
        public ColumnDefinition Issues_IssueId = new ColumnDefinition();
        public ColumnDefinition Issues_StartTime = new ColumnDefinition();
        public ColumnDefinition Issues_CompletionTime = new ColumnDefinition();
        public ColumnDefinition Issues_WorkValue = new ColumnDefinition();
        public ColumnDefinition Issues_ProgressRate = new ColumnDefinition();
        public ColumnDefinition Issues_RemainingWorkValue = new ColumnDefinition();
        public ColumnDefinition Issues_Status = new ColumnDefinition();
        public ColumnDefinition Issues_Manager = new ColumnDefinition();
        public ColumnDefinition Issues_Owner = new ColumnDefinition();
        public ColumnDefinition Issues_ClassA = new ColumnDefinition();
        public ColumnDefinition Issues_ClassB = new ColumnDefinition();
        public ColumnDefinition Issues_ClassC = new ColumnDefinition();
        public ColumnDefinition Issues_ClassD = new ColumnDefinition();
        public ColumnDefinition Issues_ClassE = new ColumnDefinition();
        public ColumnDefinition Issues_ClassF = new ColumnDefinition();
        public ColumnDefinition Issues_ClassG = new ColumnDefinition();
        public ColumnDefinition Issues_ClassH = new ColumnDefinition();
        public ColumnDefinition Issues_ClassI = new ColumnDefinition();
        public ColumnDefinition Issues_ClassJ = new ColumnDefinition();
        public ColumnDefinition Issues_ClassK = new ColumnDefinition();
        public ColumnDefinition Issues_ClassL = new ColumnDefinition();
        public ColumnDefinition Issues_ClassM = new ColumnDefinition();
        public ColumnDefinition Issues_ClassN = new ColumnDefinition();
        public ColumnDefinition Issues_ClassO = new ColumnDefinition();
        public ColumnDefinition Issues_ClassP = new ColumnDefinition();
        public ColumnDefinition Issues_ClassQ = new ColumnDefinition();
        public ColumnDefinition Issues_ClassR = new ColumnDefinition();
        public ColumnDefinition Issues_ClassS = new ColumnDefinition();
        public ColumnDefinition Issues_ClassT = new ColumnDefinition();
        public ColumnDefinition Issues_ClassU = new ColumnDefinition();
        public ColumnDefinition Issues_ClassV = new ColumnDefinition();
        public ColumnDefinition Issues_ClassW = new ColumnDefinition();
        public ColumnDefinition Issues_ClassX = new ColumnDefinition();
        public ColumnDefinition Issues_ClassY = new ColumnDefinition();
        public ColumnDefinition Issues_ClassZ = new ColumnDefinition();
        public ColumnDefinition Issues_NumA = new ColumnDefinition();
        public ColumnDefinition Issues_NumB = new ColumnDefinition();
        public ColumnDefinition Issues_NumC = new ColumnDefinition();
        public ColumnDefinition Issues_NumD = new ColumnDefinition();
        public ColumnDefinition Issues_NumE = new ColumnDefinition();
        public ColumnDefinition Issues_NumF = new ColumnDefinition();
        public ColumnDefinition Issues_NumG = new ColumnDefinition();
        public ColumnDefinition Issues_NumH = new ColumnDefinition();
        public ColumnDefinition Issues_NumI = new ColumnDefinition();
        public ColumnDefinition Issues_NumJ = new ColumnDefinition();
        public ColumnDefinition Issues_NumK = new ColumnDefinition();
        public ColumnDefinition Issues_NumL = new ColumnDefinition();
        public ColumnDefinition Issues_NumM = new ColumnDefinition();
        public ColumnDefinition Issues_NumN = new ColumnDefinition();
        public ColumnDefinition Issues_NumO = new ColumnDefinition();
        public ColumnDefinition Issues_NumP = new ColumnDefinition();
        public ColumnDefinition Issues_NumQ = new ColumnDefinition();
        public ColumnDefinition Issues_NumR = new ColumnDefinition();
        public ColumnDefinition Issues_NumS = new ColumnDefinition();
        public ColumnDefinition Issues_NumT = new ColumnDefinition();
        public ColumnDefinition Issues_NumU = new ColumnDefinition();
        public ColumnDefinition Issues_NumV = new ColumnDefinition();
        public ColumnDefinition Issues_NumW = new ColumnDefinition();
        public ColumnDefinition Issues_NumX = new ColumnDefinition();
        public ColumnDefinition Issues_NumY = new ColumnDefinition();
        public ColumnDefinition Issues_NumZ = new ColumnDefinition();
        public ColumnDefinition Issues_DateA = new ColumnDefinition();
        public ColumnDefinition Issues_DateB = new ColumnDefinition();
        public ColumnDefinition Issues_DateC = new ColumnDefinition();
        public ColumnDefinition Issues_DateD = new ColumnDefinition();
        public ColumnDefinition Issues_DateE = new ColumnDefinition();
        public ColumnDefinition Issues_DateF = new ColumnDefinition();
        public ColumnDefinition Issues_DateG = new ColumnDefinition();
        public ColumnDefinition Issues_DateH = new ColumnDefinition();
        public ColumnDefinition Issues_DateI = new ColumnDefinition();
        public ColumnDefinition Issues_DateJ = new ColumnDefinition();
        public ColumnDefinition Issues_DateK = new ColumnDefinition();
        public ColumnDefinition Issues_DateL = new ColumnDefinition();
        public ColumnDefinition Issues_DateM = new ColumnDefinition();
        public ColumnDefinition Issues_DateN = new ColumnDefinition();
        public ColumnDefinition Issues_DateO = new ColumnDefinition();
        public ColumnDefinition Issues_DateP = new ColumnDefinition();
        public ColumnDefinition Issues_DateQ = new ColumnDefinition();
        public ColumnDefinition Issues_DateR = new ColumnDefinition();
        public ColumnDefinition Issues_DateS = new ColumnDefinition();
        public ColumnDefinition Issues_DateT = new ColumnDefinition();
        public ColumnDefinition Issues_DateU = new ColumnDefinition();
        public ColumnDefinition Issues_DateV = new ColumnDefinition();
        public ColumnDefinition Issues_DateW = new ColumnDefinition();
        public ColumnDefinition Issues_DateX = new ColumnDefinition();
        public ColumnDefinition Issues_DateY = new ColumnDefinition();
        public ColumnDefinition Issues_DateZ = new ColumnDefinition();
        public ColumnDefinition Issues_DescriptionA = new ColumnDefinition();
        public ColumnDefinition Issues_DescriptionB = new ColumnDefinition();
        public ColumnDefinition Issues_DescriptionC = new ColumnDefinition();
        public ColumnDefinition Issues_DescriptionD = new ColumnDefinition();
        public ColumnDefinition Issues_DescriptionE = new ColumnDefinition();
        public ColumnDefinition Issues_DescriptionF = new ColumnDefinition();
        public ColumnDefinition Issues_DescriptionG = new ColumnDefinition();
        public ColumnDefinition Issues_DescriptionH = new ColumnDefinition();
        public ColumnDefinition Issues_DescriptionI = new ColumnDefinition();
        public ColumnDefinition Issues_DescriptionJ = new ColumnDefinition();
        public ColumnDefinition Issues_DescriptionK = new ColumnDefinition();
        public ColumnDefinition Issues_DescriptionL = new ColumnDefinition();
        public ColumnDefinition Issues_DescriptionM = new ColumnDefinition();
        public ColumnDefinition Issues_DescriptionN = new ColumnDefinition();
        public ColumnDefinition Issues_DescriptionO = new ColumnDefinition();
        public ColumnDefinition Issues_DescriptionP = new ColumnDefinition();
        public ColumnDefinition Issues_DescriptionQ = new ColumnDefinition();
        public ColumnDefinition Issues_DescriptionR = new ColumnDefinition();
        public ColumnDefinition Issues_DescriptionS = new ColumnDefinition();
        public ColumnDefinition Issues_DescriptionT = new ColumnDefinition();
        public ColumnDefinition Issues_DescriptionU = new ColumnDefinition();
        public ColumnDefinition Issues_DescriptionV = new ColumnDefinition();
        public ColumnDefinition Issues_DescriptionW = new ColumnDefinition();
        public ColumnDefinition Issues_DescriptionX = new ColumnDefinition();
        public ColumnDefinition Issues_DescriptionY = new ColumnDefinition();
        public ColumnDefinition Issues_DescriptionZ = new ColumnDefinition();
        public ColumnDefinition Issues_CheckA = new ColumnDefinition();
        public ColumnDefinition Issues_CheckB = new ColumnDefinition();
        public ColumnDefinition Issues_CheckC = new ColumnDefinition();
        public ColumnDefinition Issues_CheckD = new ColumnDefinition();
        public ColumnDefinition Issues_CheckE = new ColumnDefinition();
        public ColumnDefinition Issues_CheckF = new ColumnDefinition();
        public ColumnDefinition Issues_CheckG = new ColumnDefinition();
        public ColumnDefinition Issues_CheckH = new ColumnDefinition();
        public ColumnDefinition Issues_CheckI = new ColumnDefinition();
        public ColumnDefinition Issues_CheckJ = new ColumnDefinition();
        public ColumnDefinition Issues_CheckK = new ColumnDefinition();
        public ColumnDefinition Issues_CheckL = new ColumnDefinition();
        public ColumnDefinition Issues_CheckM = new ColumnDefinition();
        public ColumnDefinition Issues_CheckN = new ColumnDefinition();
        public ColumnDefinition Issues_CheckO = new ColumnDefinition();
        public ColumnDefinition Issues_CheckP = new ColumnDefinition();
        public ColumnDefinition Issues_CheckQ = new ColumnDefinition();
        public ColumnDefinition Issues_CheckR = new ColumnDefinition();
        public ColumnDefinition Issues_CheckS = new ColumnDefinition();
        public ColumnDefinition Issues_CheckT = new ColumnDefinition();
        public ColumnDefinition Issues_CheckU = new ColumnDefinition();
        public ColumnDefinition Issues_CheckV = new ColumnDefinition();
        public ColumnDefinition Issues_CheckW = new ColumnDefinition();
        public ColumnDefinition Issues_CheckX = new ColumnDefinition();
        public ColumnDefinition Issues_CheckY = new ColumnDefinition();
        public ColumnDefinition Issues_CheckZ = new ColumnDefinition();
        public ColumnDefinition Results_ResultId = new ColumnDefinition();
        public ColumnDefinition Results_Title = new ColumnDefinition();
        public ColumnDefinition Results_Status = new ColumnDefinition();
        public ColumnDefinition Results_Manager = new ColumnDefinition();
        public ColumnDefinition Results_Owner = new ColumnDefinition();
        public ColumnDefinition Results_ClassA = new ColumnDefinition();
        public ColumnDefinition Results_ClassB = new ColumnDefinition();
        public ColumnDefinition Results_ClassC = new ColumnDefinition();
        public ColumnDefinition Results_ClassD = new ColumnDefinition();
        public ColumnDefinition Results_ClassE = new ColumnDefinition();
        public ColumnDefinition Results_ClassF = new ColumnDefinition();
        public ColumnDefinition Results_ClassG = new ColumnDefinition();
        public ColumnDefinition Results_ClassH = new ColumnDefinition();
        public ColumnDefinition Results_ClassI = new ColumnDefinition();
        public ColumnDefinition Results_ClassJ = new ColumnDefinition();
        public ColumnDefinition Results_ClassK = new ColumnDefinition();
        public ColumnDefinition Results_ClassL = new ColumnDefinition();
        public ColumnDefinition Results_ClassM = new ColumnDefinition();
        public ColumnDefinition Results_ClassN = new ColumnDefinition();
        public ColumnDefinition Results_ClassO = new ColumnDefinition();
        public ColumnDefinition Results_ClassP = new ColumnDefinition();
        public ColumnDefinition Results_ClassQ = new ColumnDefinition();
        public ColumnDefinition Results_ClassR = new ColumnDefinition();
        public ColumnDefinition Results_ClassS = new ColumnDefinition();
        public ColumnDefinition Results_ClassT = new ColumnDefinition();
        public ColumnDefinition Results_ClassU = new ColumnDefinition();
        public ColumnDefinition Results_ClassV = new ColumnDefinition();
        public ColumnDefinition Results_ClassW = new ColumnDefinition();
        public ColumnDefinition Results_ClassX = new ColumnDefinition();
        public ColumnDefinition Results_ClassY = new ColumnDefinition();
        public ColumnDefinition Results_ClassZ = new ColumnDefinition();
        public ColumnDefinition Results_NumA = new ColumnDefinition();
        public ColumnDefinition Results_NumB = new ColumnDefinition();
        public ColumnDefinition Results_NumC = new ColumnDefinition();
        public ColumnDefinition Results_NumD = new ColumnDefinition();
        public ColumnDefinition Results_NumE = new ColumnDefinition();
        public ColumnDefinition Results_NumF = new ColumnDefinition();
        public ColumnDefinition Results_NumG = new ColumnDefinition();
        public ColumnDefinition Results_NumH = new ColumnDefinition();
        public ColumnDefinition Results_NumI = new ColumnDefinition();
        public ColumnDefinition Results_NumJ = new ColumnDefinition();
        public ColumnDefinition Results_NumK = new ColumnDefinition();
        public ColumnDefinition Results_NumL = new ColumnDefinition();
        public ColumnDefinition Results_NumM = new ColumnDefinition();
        public ColumnDefinition Results_NumN = new ColumnDefinition();
        public ColumnDefinition Results_NumO = new ColumnDefinition();
        public ColumnDefinition Results_NumP = new ColumnDefinition();
        public ColumnDefinition Results_NumQ = new ColumnDefinition();
        public ColumnDefinition Results_NumR = new ColumnDefinition();
        public ColumnDefinition Results_NumS = new ColumnDefinition();
        public ColumnDefinition Results_NumT = new ColumnDefinition();
        public ColumnDefinition Results_NumU = new ColumnDefinition();
        public ColumnDefinition Results_NumV = new ColumnDefinition();
        public ColumnDefinition Results_NumW = new ColumnDefinition();
        public ColumnDefinition Results_NumX = new ColumnDefinition();
        public ColumnDefinition Results_NumY = new ColumnDefinition();
        public ColumnDefinition Results_NumZ = new ColumnDefinition();
        public ColumnDefinition Results_DateA = new ColumnDefinition();
        public ColumnDefinition Results_DateB = new ColumnDefinition();
        public ColumnDefinition Results_DateC = new ColumnDefinition();
        public ColumnDefinition Results_DateD = new ColumnDefinition();
        public ColumnDefinition Results_DateE = new ColumnDefinition();
        public ColumnDefinition Results_DateF = new ColumnDefinition();
        public ColumnDefinition Results_DateG = new ColumnDefinition();
        public ColumnDefinition Results_DateH = new ColumnDefinition();
        public ColumnDefinition Results_DateI = new ColumnDefinition();
        public ColumnDefinition Results_DateJ = new ColumnDefinition();
        public ColumnDefinition Results_DateK = new ColumnDefinition();
        public ColumnDefinition Results_DateL = new ColumnDefinition();
        public ColumnDefinition Results_DateM = new ColumnDefinition();
        public ColumnDefinition Results_DateN = new ColumnDefinition();
        public ColumnDefinition Results_DateO = new ColumnDefinition();
        public ColumnDefinition Results_DateP = new ColumnDefinition();
        public ColumnDefinition Results_DateQ = new ColumnDefinition();
        public ColumnDefinition Results_DateR = new ColumnDefinition();
        public ColumnDefinition Results_DateS = new ColumnDefinition();
        public ColumnDefinition Results_DateT = new ColumnDefinition();
        public ColumnDefinition Results_DateU = new ColumnDefinition();
        public ColumnDefinition Results_DateV = new ColumnDefinition();
        public ColumnDefinition Results_DateW = new ColumnDefinition();
        public ColumnDefinition Results_DateX = new ColumnDefinition();
        public ColumnDefinition Results_DateY = new ColumnDefinition();
        public ColumnDefinition Results_DateZ = new ColumnDefinition();
        public ColumnDefinition Results_DescriptionA = new ColumnDefinition();
        public ColumnDefinition Results_DescriptionB = new ColumnDefinition();
        public ColumnDefinition Results_DescriptionC = new ColumnDefinition();
        public ColumnDefinition Results_DescriptionD = new ColumnDefinition();
        public ColumnDefinition Results_DescriptionE = new ColumnDefinition();
        public ColumnDefinition Results_DescriptionF = new ColumnDefinition();
        public ColumnDefinition Results_DescriptionG = new ColumnDefinition();
        public ColumnDefinition Results_DescriptionH = new ColumnDefinition();
        public ColumnDefinition Results_DescriptionI = new ColumnDefinition();
        public ColumnDefinition Results_DescriptionJ = new ColumnDefinition();
        public ColumnDefinition Results_DescriptionK = new ColumnDefinition();
        public ColumnDefinition Results_DescriptionL = new ColumnDefinition();
        public ColumnDefinition Results_DescriptionM = new ColumnDefinition();
        public ColumnDefinition Results_DescriptionN = new ColumnDefinition();
        public ColumnDefinition Results_DescriptionO = new ColumnDefinition();
        public ColumnDefinition Results_DescriptionP = new ColumnDefinition();
        public ColumnDefinition Results_DescriptionQ = new ColumnDefinition();
        public ColumnDefinition Results_DescriptionR = new ColumnDefinition();
        public ColumnDefinition Results_DescriptionS = new ColumnDefinition();
        public ColumnDefinition Results_DescriptionT = new ColumnDefinition();
        public ColumnDefinition Results_DescriptionU = new ColumnDefinition();
        public ColumnDefinition Results_DescriptionV = new ColumnDefinition();
        public ColumnDefinition Results_DescriptionW = new ColumnDefinition();
        public ColumnDefinition Results_DescriptionX = new ColumnDefinition();
        public ColumnDefinition Results_DescriptionY = new ColumnDefinition();
        public ColumnDefinition Results_DescriptionZ = new ColumnDefinition();
        public ColumnDefinition Results_CheckA = new ColumnDefinition();
        public ColumnDefinition Results_CheckB = new ColumnDefinition();
        public ColumnDefinition Results_CheckC = new ColumnDefinition();
        public ColumnDefinition Results_CheckD = new ColumnDefinition();
        public ColumnDefinition Results_CheckE = new ColumnDefinition();
        public ColumnDefinition Results_CheckF = new ColumnDefinition();
        public ColumnDefinition Results_CheckG = new ColumnDefinition();
        public ColumnDefinition Results_CheckH = new ColumnDefinition();
        public ColumnDefinition Results_CheckI = new ColumnDefinition();
        public ColumnDefinition Results_CheckJ = new ColumnDefinition();
        public ColumnDefinition Results_CheckK = new ColumnDefinition();
        public ColumnDefinition Results_CheckL = new ColumnDefinition();
        public ColumnDefinition Results_CheckM = new ColumnDefinition();
        public ColumnDefinition Results_CheckN = new ColumnDefinition();
        public ColumnDefinition Results_CheckO = new ColumnDefinition();
        public ColumnDefinition Results_CheckP = new ColumnDefinition();
        public ColumnDefinition Results_CheckQ = new ColumnDefinition();
        public ColumnDefinition Results_CheckR = new ColumnDefinition();
        public ColumnDefinition Results_CheckS = new ColumnDefinition();
        public ColumnDefinition Results_CheckT = new ColumnDefinition();
        public ColumnDefinition Results_CheckU = new ColumnDefinition();
        public ColumnDefinition Results_CheckV = new ColumnDefinition();
        public ColumnDefinition Results_CheckW = new ColumnDefinition();
        public ColumnDefinition Results_CheckX = new ColumnDefinition();
        public ColumnDefinition Results_CheckY = new ColumnDefinition();
        public ColumnDefinition Results_CheckZ = new ColumnDefinition();
        public ColumnDefinition Wikis_WikiId = new ColumnDefinition();
        public ColumnDefinition Tenants_Ver = new ColumnDefinition();
        public ColumnDefinition Tenants_Comments = new ColumnDefinition();
        public ColumnDefinition Tenants_Creator = new ColumnDefinition();
        public ColumnDefinition Tenants_Updator = new ColumnDefinition();
        public ColumnDefinition Tenants_CreatedTime = new ColumnDefinition();
        public ColumnDefinition Tenants_UpdatedTime = new ColumnDefinition();
        public ColumnDefinition Tenants_VerUp = new ColumnDefinition();
        public ColumnDefinition Tenants_Timestamp = new ColumnDefinition();
        public ColumnDefinition Demos_Ver = new ColumnDefinition();
        public ColumnDefinition Demos_Comments = new ColumnDefinition();
        public ColumnDefinition Demos_Creator = new ColumnDefinition();
        public ColumnDefinition Demos_Updator = new ColumnDefinition();
        public ColumnDefinition Demos_CreatedTime = new ColumnDefinition();
        public ColumnDefinition Demos_UpdatedTime = new ColumnDefinition();
        public ColumnDefinition Demos_VerUp = new ColumnDefinition();
        public ColumnDefinition Demos_Timestamp = new ColumnDefinition();
        public ColumnDefinition SysLogs_Ver = new ColumnDefinition();
        public ColumnDefinition SysLogs_Comments = new ColumnDefinition();
        public ColumnDefinition SysLogs_Creator = new ColumnDefinition();
        public ColumnDefinition SysLogs_Updator = new ColumnDefinition();
        public ColumnDefinition SysLogs_UpdatedTime = new ColumnDefinition();
        public ColumnDefinition SysLogs_VerUp = new ColumnDefinition();
        public ColumnDefinition SysLogs_Timestamp = new ColumnDefinition();
        public ColumnDefinition Depts_Ver = new ColumnDefinition();
        public ColumnDefinition Depts_Comments = new ColumnDefinition();
        public ColumnDefinition Depts_Creator = new ColumnDefinition();
        public ColumnDefinition Depts_Updator = new ColumnDefinition();
        public ColumnDefinition Depts_CreatedTime = new ColumnDefinition();
        public ColumnDefinition Depts_UpdatedTime = new ColumnDefinition();
        public ColumnDefinition Depts_VerUp = new ColumnDefinition();
        public ColumnDefinition Depts_Timestamp = new ColumnDefinition();
        public ColumnDefinition Groups_Ver = new ColumnDefinition();
        public ColumnDefinition Groups_Comments = new ColumnDefinition();
        public ColumnDefinition Groups_Creator = new ColumnDefinition();
        public ColumnDefinition Groups_Updator = new ColumnDefinition();
        public ColumnDefinition Groups_CreatedTime = new ColumnDefinition();
        public ColumnDefinition Groups_UpdatedTime = new ColumnDefinition();
        public ColumnDefinition Groups_VerUp = new ColumnDefinition();
        public ColumnDefinition Groups_Timestamp = new ColumnDefinition();
        public ColumnDefinition GroupMembers_Ver = new ColumnDefinition();
        public ColumnDefinition GroupMembers_Comments = new ColumnDefinition();
        public ColumnDefinition GroupMembers_Creator = new ColumnDefinition();
        public ColumnDefinition GroupMembers_Updator = new ColumnDefinition();
        public ColumnDefinition GroupMembers_CreatedTime = new ColumnDefinition();
        public ColumnDefinition GroupMembers_UpdatedTime = new ColumnDefinition();
        public ColumnDefinition GroupMembers_VerUp = new ColumnDefinition();
        public ColumnDefinition GroupMembers_Timestamp = new ColumnDefinition();
        public ColumnDefinition Users_Ver = new ColumnDefinition();
        public ColumnDefinition Users_Comments = new ColumnDefinition();
        public ColumnDefinition Users_Creator = new ColumnDefinition();
        public ColumnDefinition Users_Updator = new ColumnDefinition();
        public ColumnDefinition Users_CreatedTime = new ColumnDefinition();
        public ColumnDefinition Users_UpdatedTime = new ColumnDefinition();
        public ColumnDefinition Users_VerUp = new ColumnDefinition();
        public ColumnDefinition Users_Timestamp = new ColumnDefinition();
        public ColumnDefinition MailAddresses_Ver = new ColumnDefinition();
        public ColumnDefinition MailAddresses_Comments = new ColumnDefinition();
        public ColumnDefinition MailAddresses_Creator = new ColumnDefinition();
        public ColumnDefinition MailAddresses_Updator = new ColumnDefinition();
        public ColumnDefinition MailAddresses_CreatedTime = new ColumnDefinition();
        public ColumnDefinition MailAddresses_UpdatedTime = new ColumnDefinition();
        public ColumnDefinition MailAddresses_VerUp = new ColumnDefinition();
        public ColumnDefinition MailAddresses_Timestamp = new ColumnDefinition();
        public ColumnDefinition Permissions_Ver = new ColumnDefinition();
        public ColumnDefinition Permissions_Comments = new ColumnDefinition();
        public ColumnDefinition Permissions_Creator = new ColumnDefinition();
        public ColumnDefinition Permissions_Updator = new ColumnDefinition();
        public ColumnDefinition Permissions_CreatedTime = new ColumnDefinition();
        public ColumnDefinition Permissions_UpdatedTime = new ColumnDefinition();
        public ColumnDefinition Permissions_VerUp = new ColumnDefinition();
        public ColumnDefinition Permissions_Timestamp = new ColumnDefinition();
        public ColumnDefinition OutgoingMails_Ver = new ColumnDefinition();
        public ColumnDefinition OutgoingMails_Comments = new ColumnDefinition();
        public ColumnDefinition OutgoingMails_Creator = new ColumnDefinition();
        public ColumnDefinition OutgoingMails_Updator = new ColumnDefinition();
        public ColumnDefinition OutgoingMails_CreatedTime = new ColumnDefinition();
        public ColumnDefinition OutgoingMails_UpdatedTime = new ColumnDefinition();
        public ColumnDefinition OutgoingMails_VerUp = new ColumnDefinition();
        public ColumnDefinition OutgoingMails_Timestamp = new ColumnDefinition();
        public ColumnDefinition SearchIndexes_Ver = new ColumnDefinition();
        public ColumnDefinition SearchIndexes_Comments = new ColumnDefinition();
        public ColumnDefinition SearchIndexes_Creator = new ColumnDefinition();
        public ColumnDefinition SearchIndexes_Updator = new ColumnDefinition();
        public ColumnDefinition SearchIndexes_CreatedTime = new ColumnDefinition();
        public ColumnDefinition SearchIndexes_UpdatedTime = new ColumnDefinition();
        public ColumnDefinition SearchIndexes_VerUp = new ColumnDefinition();
        public ColumnDefinition SearchIndexes_Timestamp = new ColumnDefinition();
        public ColumnDefinition Items_Ver = new ColumnDefinition();
        public ColumnDefinition Items_Comments = new ColumnDefinition();
        public ColumnDefinition Items_Creator = new ColumnDefinition();
        public ColumnDefinition Items_Updator = new ColumnDefinition();
        public ColumnDefinition Items_CreatedTime = new ColumnDefinition();
        public ColumnDefinition Items_UpdatedTime = new ColumnDefinition();
        public ColumnDefinition Items_VerUp = new ColumnDefinition();
        public ColumnDefinition Items_Timestamp = new ColumnDefinition();
        public ColumnDefinition Sites_UpdatedTime = new ColumnDefinition();
        public ColumnDefinition Sites_Title = new ColumnDefinition();
        public ColumnDefinition Sites_Body = new ColumnDefinition();
        public ColumnDefinition Sites_TitleBody = new ColumnDefinition();
        public ColumnDefinition Sites_Ver = new ColumnDefinition();
        public ColumnDefinition Sites_Comments = new ColumnDefinition();
        public ColumnDefinition Sites_Creator = new ColumnDefinition();
        public ColumnDefinition Sites_Updator = new ColumnDefinition();
        public ColumnDefinition Sites_CreatedTime = new ColumnDefinition();
        public ColumnDefinition Sites_VerUp = new ColumnDefinition();
        public ColumnDefinition Sites_Timestamp = new ColumnDefinition();
        public ColumnDefinition Orders_Ver = new ColumnDefinition();
        public ColumnDefinition Orders_Comments = new ColumnDefinition();
        public ColumnDefinition Orders_Creator = new ColumnDefinition();
        public ColumnDefinition Orders_Updator = new ColumnDefinition();
        public ColumnDefinition Orders_CreatedTime = new ColumnDefinition();
        public ColumnDefinition Orders_UpdatedTime = new ColumnDefinition();
        public ColumnDefinition Orders_VerUp = new ColumnDefinition();
        public ColumnDefinition Orders_Timestamp = new ColumnDefinition();
        public ColumnDefinition ExportSettings_Ver = new ColumnDefinition();
        public ColumnDefinition ExportSettings_Comments = new ColumnDefinition();
        public ColumnDefinition ExportSettings_Creator = new ColumnDefinition();
        public ColumnDefinition ExportSettings_Updator = new ColumnDefinition();
        public ColumnDefinition ExportSettings_CreatedTime = new ColumnDefinition();
        public ColumnDefinition ExportSettings_UpdatedTime = new ColumnDefinition();
        public ColumnDefinition ExportSettings_VerUp = new ColumnDefinition();
        public ColumnDefinition ExportSettings_Timestamp = new ColumnDefinition();
        public ColumnDefinition Links_Ver = new ColumnDefinition();
        public ColumnDefinition Links_Comments = new ColumnDefinition();
        public ColumnDefinition Links_Creator = new ColumnDefinition();
        public ColumnDefinition Links_Updator = new ColumnDefinition();
        public ColumnDefinition Links_CreatedTime = new ColumnDefinition();
        public ColumnDefinition Links_UpdatedTime = new ColumnDefinition();
        public ColumnDefinition Links_VerUp = new ColumnDefinition();
        public ColumnDefinition Links_Timestamp = new ColumnDefinition();
        public ColumnDefinition Binaries_Ver = new ColumnDefinition();
        public ColumnDefinition Binaries_Comments = new ColumnDefinition();
        public ColumnDefinition Binaries_Creator = new ColumnDefinition();
        public ColumnDefinition Binaries_Updator = new ColumnDefinition();
        public ColumnDefinition Binaries_CreatedTime = new ColumnDefinition();
        public ColumnDefinition Binaries_UpdatedTime = new ColumnDefinition();
        public ColumnDefinition Binaries_VerUp = new ColumnDefinition();
        public ColumnDefinition Binaries_Timestamp = new ColumnDefinition();
        public ColumnDefinition Issues_SiteId = new ColumnDefinition();
        public ColumnDefinition Issues_UpdatedTime = new ColumnDefinition();
        public ColumnDefinition Issues_Title = new ColumnDefinition();
        public ColumnDefinition Issues_Body = new ColumnDefinition();
        public ColumnDefinition Issues_TitleBody = new ColumnDefinition();
        public ColumnDefinition Issues_Ver = new ColumnDefinition();
        public ColumnDefinition Issues_Comments = new ColumnDefinition();
        public ColumnDefinition Issues_Creator = new ColumnDefinition();
        public ColumnDefinition Issues_Updator = new ColumnDefinition();
        public ColumnDefinition Issues_CreatedTime = new ColumnDefinition();
        public ColumnDefinition Issues_VerUp = new ColumnDefinition();
        public ColumnDefinition Issues_Timestamp = new ColumnDefinition();
        public ColumnDefinition Results_SiteId = new ColumnDefinition();
        public ColumnDefinition Results_UpdatedTime = new ColumnDefinition();
        public ColumnDefinition Results_Body = new ColumnDefinition();
        public ColumnDefinition Results_TitleBody = new ColumnDefinition();
        public ColumnDefinition Results_Ver = new ColumnDefinition();
        public ColumnDefinition Results_Comments = new ColumnDefinition();
        public ColumnDefinition Results_Creator = new ColumnDefinition();
        public ColumnDefinition Results_Updator = new ColumnDefinition();
        public ColumnDefinition Results_CreatedTime = new ColumnDefinition();
        public ColumnDefinition Results_VerUp = new ColumnDefinition();
        public ColumnDefinition Results_Timestamp = new ColumnDefinition();
        public ColumnDefinition Wikis_SiteId = new ColumnDefinition();
        public ColumnDefinition Wikis_UpdatedTime = new ColumnDefinition();
        public ColumnDefinition Wikis_Title = new ColumnDefinition();
        public ColumnDefinition Wikis_Body = new ColumnDefinition();
        public ColumnDefinition Wikis_TitleBody = new ColumnDefinition();
        public ColumnDefinition Wikis_Ver = new ColumnDefinition();
        public ColumnDefinition Wikis_Comments = new ColumnDefinition();
        public ColumnDefinition Wikis_Creator = new ColumnDefinition();
        public ColumnDefinition Wikis_Updator = new ColumnDefinition();
        public ColumnDefinition Wikis_CreatedTime = new ColumnDefinition();
        public ColumnDefinition Wikis_VerUp = new ColumnDefinition();
        public ColumnDefinition Wikis_Timestamp = new ColumnDefinition();
    }

    public class CssDefinition
    {
        public string Id; public string SavedId;
        public string Specific; public string SavedSpecific;
        public string width; public string Savedwidth;
        public string min_width; public string Savedmin_width;
        public string max_width; public string Savedmax_width;
        public string height; public string Savedheight;
        public string min_height; public string Savedmin_height;
        public string max_height; public string Savedmax_height;
        public string display; public string Saveddisplay;
        public string _float; public string Saved_float;
        public string margin; public string Savedmargin;
        public string margin_left; public string Savedmargin_left;
        public string margin_right; public string Savedmargin_right;
        public string padding; public string Savedpadding;
        public string padding_bottom; public string Savedpadding_bottom;
        public string text_align; public string Savedtext_align;
        public string vertical_align; public string Savedvertical_align;
        public string line_height; public string Savedline_height;
        public string font_size; public string Savedfont_size;
        public string font_family; public string Savedfont_family;
        public string font_weight; public string Savedfont_weight;
        public string font_style; public string Savedfont_style;
        public string color; public string Savedcolor;
        public string background; public string Savedbackground;
        public string background_color; public string Savedbackground_color;
        public string border; public string Savedborder;
        public string border_top; public string Savedborder_top;
        public string border_bottom; public string Savedborder_bottom;
        public string border_left; public string Savedborder_left;
        public string border_right; public string Savedborder_right;
        public string border_collapse; public string Savedborder_collapse;
        public string border_spacing; public string Savedborder_spacing;
        public string position; public string Savedposition;
        public string top; public string Savedtop;
        public string right; public string Savedright;
        public string left; public string Savedleft;
        public string bottom; public string Savedbottom;
        public string cursor; public string Savedcursor;
        public string clear; public string Savedclear;
        public string overflow; public string Savedoverflow;
        public string word_wrap; public string Savedword_wrap;
        public string word_break; public string Savedword_break;
        public string white_space; public string Savedwhite_space;
        public string table_layout; public string Savedtable_layout;
        public string text_decoration; public string Savedtext_decoration;
        public string list_style_type; public string Savedlist_style_type;
        public string visibility; public string Savedvisibility;
        public string border_radius; public string Savedborder_radius;
        public string z_index; public string Savedz_index;
        public string fill; public string Savedfill;
        public string fill_opacity; public string Savedfill_opacity;
        public string stroke; public string Savedstroke;
        public string stroke_width; public string Savedstroke_width;
        public string text_anchor; public string Savedtext_anchor;
        public string shape_rendering; public string Savedshape_rendering;

        public CssDefinition()
        {
        }

        public CssDefinition(Dictionary<string, string> propertyCollection)
        {
            if (propertyCollection.ContainsKey("Id")) Id = propertyCollection["Id"].ToString(); else Id = string.Empty;
            if (propertyCollection.ContainsKey("Specific")) Specific = propertyCollection["Specific"].ToString(); else Specific = string.Empty;
            if (propertyCollection.ContainsKey("width")) width = propertyCollection["width"].ToString(); else width = string.Empty;
            if (propertyCollection.ContainsKey("min_width")) min_width = propertyCollection["min_width"].ToString(); else min_width = string.Empty;
            if (propertyCollection.ContainsKey("max_width")) max_width = propertyCollection["max_width"].ToString(); else max_width = string.Empty;
            if (propertyCollection.ContainsKey("height")) height = propertyCollection["height"].ToString(); else height = string.Empty;
            if (propertyCollection.ContainsKey("min_height")) min_height = propertyCollection["min_height"].ToString(); else min_height = string.Empty;
            if (propertyCollection.ContainsKey("max_height")) max_height = propertyCollection["max_height"].ToString(); else max_height = string.Empty;
            if (propertyCollection.ContainsKey("display")) display = propertyCollection["display"].ToString(); else display = string.Empty;
            if (propertyCollection.ContainsKey("_float")) _float = propertyCollection["_float"].ToString(); else _float = string.Empty;
            if (propertyCollection.ContainsKey("margin")) margin = propertyCollection["margin"].ToString(); else margin = string.Empty;
            if (propertyCollection.ContainsKey("margin_left")) margin_left = propertyCollection["margin_left"].ToString(); else margin_left = string.Empty;
            if (propertyCollection.ContainsKey("margin_right")) margin_right = propertyCollection["margin_right"].ToString(); else margin_right = string.Empty;
            if (propertyCollection.ContainsKey("padding")) padding = propertyCollection["padding"].ToString(); else padding = string.Empty;
            if (propertyCollection.ContainsKey("padding_bottom")) padding_bottom = propertyCollection["padding_bottom"].ToString(); else padding_bottom = string.Empty;
            if (propertyCollection.ContainsKey("text_align")) text_align = propertyCollection["text_align"].ToString(); else text_align = string.Empty;
            if (propertyCollection.ContainsKey("vertical_align")) vertical_align = propertyCollection["vertical_align"].ToString(); else vertical_align = string.Empty;
            if (propertyCollection.ContainsKey("line_height")) line_height = propertyCollection["line_height"].ToString(); else line_height = string.Empty;
            if (propertyCollection.ContainsKey("font_size")) font_size = propertyCollection["font_size"].ToString(); else font_size = string.Empty;
            if (propertyCollection.ContainsKey("font_family")) font_family = propertyCollection["font_family"].ToString(); else font_family = string.Empty;
            if (propertyCollection.ContainsKey("font_weight")) font_weight = propertyCollection["font_weight"].ToString(); else font_weight = string.Empty;
            if (propertyCollection.ContainsKey("font_style")) font_style = propertyCollection["font_style"].ToString(); else font_style = string.Empty;
            if (propertyCollection.ContainsKey("color")) color = propertyCollection["color"].ToString(); else color = string.Empty;
            if (propertyCollection.ContainsKey("background")) background = propertyCollection["background"].ToString(); else background = string.Empty;
            if (propertyCollection.ContainsKey("background_color")) background_color = propertyCollection["background_color"].ToString(); else background_color = string.Empty;
            if (propertyCollection.ContainsKey("border")) border = propertyCollection["border"].ToString(); else border = string.Empty;
            if (propertyCollection.ContainsKey("border_top")) border_top = propertyCollection["border_top"].ToString(); else border_top = string.Empty;
            if (propertyCollection.ContainsKey("border_bottom")) border_bottom = propertyCollection["border_bottom"].ToString(); else border_bottom = string.Empty;
            if (propertyCollection.ContainsKey("border_left")) border_left = propertyCollection["border_left"].ToString(); else border_left = string.Empty;
            if (propertyCollection.ContainsKey("border_right")) border_right = propertyCollection["border_right"].ToString(); else border_right = string.Empty;
            if (propertyCollection.ContainsKey("border_collapse")) border_collapse = propertyCollection["border_collapse"].ToString(); else border_collapse = string.Empty;
            if (propertyCollection.ContainsKey("border_spacing")) border_spacing = propertyCollection["border_spacing"].ToString(); else border_spacing = string.Empty;
            if (propertyCollection.ContainsKey("position")) position = propertyCollection["position"].ToString(); else position = string.Empty;
            if (propertyCollection.ContainsKey("top")) top = propertyCollection["top"].ToString(); else top = string.Empty;
            if (propertyCollection.ContainsKey("right")) right = propertyCollection["right"].ToString(); else right = string.Empty;
            if (propertyCollection.ContainsKey("left")) left = propertyCollection["left"].ToString(); else left = string.Empty;
            if (propertyCollection.ContainsKey("bottom")) bottom = propertyCollection["bottom"].ToString(); else bottom = string.Empty;
            if (propertyCollection.ContainsKey("cursor")) cursor = propertyCollection["cursor"].ToString(); else cursor = string.Empty;
            if (propertyCollection.ContainsKey("clear")) clear = propertyCollection["clear"].ToString(); else clear = string.Empty;
            if (propertyCollection.ContainsKey("overflow")) overflow = propertyCollection["overflow"].ToString(); else overflow = string.Empty;
            if (propertyCollection.ContainsKey("word_wrap")) word_wrap = propertyCollection["word_wrap"].ToString(); else word_wrap = string.Empty;
            if (propertyCollection.ContainsKey("word_break")) word_break = propertyCollection["word_break"].ToString(); else word_break = string.Empty;
            if (propertyCollection.ContainsKey("white_space")) white_space = propertyCollection["white_space"].ToString(); else white_space = string.Empty;
            if (propertyCollection.ContainsKey("table_layout")) table_layout = propertyCollection["table_layout"].ToString(); else table_layout = string.Empty;
            if (propertyCollection.ContainsKey("text_decoration")) text_decoration = propertyCollection["text_decoration"].ToString(); else text_decoration = string.Empty;
            if (propertyCollection.ContainsKey("list_style_type")) list_style_type = propertyCollection["list_style_type"].ToString(); else list_style_type = string.Empty;
            if (propertyCollection.ContainsKey("visibility")) visibility = propertyCollection["visibility"].ToString(); else visibility = string.Empty;
            if (propertyCollection.ContainsKey("border_radius")) border_radius = propertyCollection["border_radius"].ToString(); else border_radius = string.Empty;
            if (propertyCollection.ContainsKey("z_index")) z_index = propertyCollection["z_index"].ToString(); else z_index = string.Empty;
            if (propertyCollection.ContainsKey("fill")) fill = propertyCollection["fill"].ToString(); else fill = string.Empty;
            if (propertyCollection.ContainsKey("fill_opacity")) fill_opacity = propertyCollection["fill_opacity"].ToString(); else fill_opacity = string.Empty;
            if (propertyCollection.ContainsKey("stroke")) stroke = propertyCollection["stroke"].ToString(); else stroke = string.Empty;
            if (propertyCollection.ContainsKey("stroke_width")) stroke_width = propertyCollection["stroke_width"].ToString(); else stroke_width = string.Empty;
            if (propertyCollection.ContainsKey("text_anchor")) text_anchor = propertyCollection["text_anchor"].ToString(); else text_anchor = string.Empty;
            if (propertyCollection.ContainsKey("shape_rendering")) shape_rendering = propertyCollection["shape_rendering"].ToString(); else shape_rendering = string.Empty;
        }

        public object this[string key]
        {
            get{
                switch(key)
                {
                    case "Id": return Id;
                    case "Specific": return Specific;
                    case "width": return width;
                    case "min_width": return min_width;
                    case "max_width": return max_width;
                    case "height": return height;
                    case "min_height": return min_height;
                    case "max_height": return max_height;
                    case "display": return display;
                    case "_float": return _float;
                    case "margin": return margin;
                    case "margin_left": return margin_left;
                    case "margin_right": return margin_right;
                    case "padding": return padding;
                    case "padding_bottom": return padding_bottom;
                    case "text_align": return text_align;
                    case "vertical_align": return vertical_align;
                    case "line_height": return line_height;
                    case "font_size": return font_size;
                    case "font_family": return font_family;
                    case "font_weight": return font_weight;
                    case "font_style": return font_style;
                    case "color": return color;
                    case "background": return background;
                    case "background_color": return background_color;
                    case "border": return border;
                    case "border_top": return border_top;
                    case "border_bottom": return border_bottom;
                    case "border_left": return border_left;
                    case "border_right": return border_right;
                    case "border_collapse": return border_collapse;
                    case "border_spacing": return border_spacing;
                    case "position": return position;
                    case "top": return top;
                    case "right": return right;
                    case "left": return left;
                    case "bottom": return bottom;
                    case "cursor": return cursor;
                    case "clear": return clear;
                    case "overflow": return overflow;
                    case "word_wrap": return word_wrap;
                    case "word_break": return word_break;
                    case "white_space": return white_space;
                    case "table_layout": return table_layout;
                    case "text_decoration": return text_decoration;
                    case "list_style_type": return list_style_type;
                    case "visibility": return visibility;
                    case "border_radius": return border_radius;
                    case "z_index": return z_index;
                    case "fill": return fill;
                    case "fill_opacity": return fill_opacity;
                    case "stroke": return stroke;
                    case "stroke_width": return stroke_width;
                    case "text_anchor": return text_anchor;
                    case "shape_rendering": return shape_rendering;
                    default: return null;
                }
            }
        }

        public void RestoreBySavedMemory()
        {
            Id = SavedId;
            Specific = SavedSpecific;
            width = Savedwidth;
            min_width = Savedmin_width;
            max_width = Savedmax_width;
            height = Savedheight;
            min_height = Savedmin_height;
            max_height = Savedmax_height;
            display = Saveddisplay;
            _float = Saved_float;
            margin = Savedmargin;
            margin_left = Savedmargin_left;
            margin_right = Savedmargin_right;
            padding = Savedpadding;
            padding_bottom = Savedpadding_bottom;
            text_align = Savedtext_align;
            vertical_align = Savedvertical_align;
            line_height = Savedline_height;
            font_size = Savedfont_size;
            font_family = Savedfont_family;
            font_weight = Savedfont_weight;
            font_style = Savedfont_style;
            color = Savedcolor;
            background = Savedbackground;
            background_color = Savedbackground_color;
            border = Savedborder;
            border_top = Savedborder_top;
            border_bottom = Savedborder_bottom;
            border_left = Savedborder_left;
            border_right = Savedborder_right;
            border_collapse = Savedborder_collapse;
            border_spacing = Savedborder_spacing;
            position = Savedposition;
            top = Savedtop;
            right = Savedright;
            left = Savedleft;
            bottom = Savedbottom;
            cursor = Savedcursor;
            clear = Savedclear;
            overflow = Savedoverflow;
            word_wrap = Savedword_wrap;
            word_break = Savedword_break;
            white_space = Savedwhite_space;
            table_layout = Savedtable_layout;
            text_decoration = Savedtext_decoration;
            list_style_type = Savedlist_style_type;
            visibility = Savedvisibility;
            border_radius = Savedborder_radius;
            z_index = Savedz_index;
            fill = Savedfill;
            fill_opacity = Savedfill_opacity;
            stroke = Savedstroke;
            stroke_width = Savedstroke_width;
            text_anchor = Savedtext_anchor;
            shape_rendering = Savedshape_rendering;
        }
    }

    public class CssColumn2nd
    {
        public string html;
        public string body;
        public string h1;
        public string h2;
        public string h3;
        public string legend;
        public string legend_space___space__asterisk_;
        public string label;
        public string input;
        public string select;
        public string table;
        public string td;
        public string pre;
        public string _sharp_Logo;
        public string _sharp_CorpLogo;
        public string _sharp_ProductLogo;
        public string _sharp_Login;
        public string _sharp_LoginCommands;
        public string _sharp_Demo;
        public string _sharp_DemoFields;
        public string _sharp_Breadcrumb;
        public string _sharp_Breadcrumb_space__dot_item;
        public string _sharp_Breadcrumb_space__dot_separator;
        public string _sharp_Header;
        public string _sharp_Navigations;
        public string _sharp_NavigationMenu;
        public string _sharp_NavigationMenu_space___space_li;
        public string _sharp_NavigationMenu_space___space_li_space___space_div;
        public string _sharp_NavigationMenu_space___space_li_space___space_div_colon_hover;
        public string _sharp_NavigationMenu_space___space_li_dot_sub_menu_space___space_div_dot_hover;
        public string _sharp_NavigationMenu_space___space_li_space___space_div_space___space_a;
        public string _sharp_NavigationMenu_space___space_li_space___space_div_colon_hover_space___space_a;
        public string _sharp_NavigationMenu_space__dot_menu;
        public string _sharp_NavigationMenu_space__dot_menu_space___space_li_space___space_a;
        public string _sharp_NavigationMenu_space__dot_menu_space___space_li_space___space_a_dot_ui_state_active;
        public string _sharp_SearchField;
        public string _sharp_Search;
        public string _sharp_Application;
        public string _sharp_Application_space___space__dot_site_image_icon;
        public string _sharp_HeaderTitleContainer;
        public string _sharp_HeaderTitle;
        public string _sharp_Notes_space___space__asterisk_;
        public string _sharp_Notes_space___space__dot_history;
        public string _sharp_Notes_space___space__dot_readonly;
        public string _sharp_ViewSelectorField;
        public string _sharp_ViewFilters;
        public string _sharp_ViewFilters_dot_reduced;
        public string _sharp_ViewFilters_space___space__dot_field_auto_thin;
        public string _sharp_ViewFilters_Reset;
        public string _sharp_ViewFilters_space___space__dot_display_control;
        public string _sharp_Aggregations;
        public string _sharp_Aggregations_dot_reduced;
        public string _sharp_Aggregations_space__dot_label;
        public string _sharp_Aggregations_space__dot_label_dot_overdue;
        public string _sharp_Aggregations_space__dot_data;
        public string _sharp_Aggregations_space__dot_data_dot_overdue;
        public string _sharp_Aggregations_space_em;
        public string _sharp_Aggregations_space___space__dot_display_control;
        public string _sharp_Gantt;
        public string _sharp_Gantt_space__dot_date;
        public string _sharp_Gantt_space__dot_now;
        public string _sharp_Gantt_space__dot_planned_space_rect;
        public string _sharp_Gantt_space__dot_planned_space_rect_dot_summary;
        public string _sharp_Gantt_space__dot_earned_space_rect;
        public string _sharp_Gantt_space__dot_earned_space_rect_dot_summary;
        public string _sharp_Gantt_space_rect_dot_delay;
        public string _sharp_Gantt_space_rect_dot_delay_dot_summary;
        public string _sharp_Gantt_space_rect_dot_completed;
        public string _sharp_Gantt_space__dot_title_space_text;
        public string _sharp_Gantt_space__dot_title_space_text_dot_delay;
        public string _sharp_Gantt_space__dot_title_space_text_dot_summary;
        public string _sharp_GanttAxis;
        public string _sharp_GanttAxis_space_text;
        public string _sharp_BurnDown;
        public string _sharp_BurnDown_space__dot_now;
        public string _sharp_BurnDown_space__dot_total;
        public string _sharp_BurnDown_space__dot_planned;
        public string _sharp_BurnDown_space__dot_earned;
        public string _sharp_BurnDown_space__dot_total_space_circle;
        public string _sharp_BurnDown_space__dot_planned_space_circle;
        public string _sharp_BurnDown_space__dot_earned_space_circle;
        public string _sharp_BurnDownDetails_space___space_tbody_space___space_tr_space___space_td;
        public string _sharp_BurnDownDetails_space___space_tbody_space___space_tr_space___space_td_dot_warning;
        public string _sharp_BurnDownDetails_space___space_tbody_space___space_tr_space___space_td_dot_difference;
        public string _sharp_BurnDownDetails_space___space_tbody_space___space_tr_space___space_td_dot_difference_dot_warning;
        public string _sharp_BurnDownDetails_space__dot_user_info;
        public string _sharp_BurnDownDetails_space__dot_items;
        public string _sharp_BurnDownDetails_space__dot_items_space_a;
        public string _sharp_BurnDownDetails_space__dot_items_space_a_colon_hover;
        public string _sharp_TimeSeries;
        public string _sharp_TimeSeries_space__dot_surface;
        public string _sharp_TimeSeries_space__dot_index;
        public string _sharp_KambanBody_space__dot_kamban_row;
        public string _sharp_KambanBody_space__dot_kamban_container_space___space_div;
        public string _sharp_KambanBody_space__dot_kamban_container_dot_hover;
        public string _sharp_KambanBody_space__dot_kamban_container_space__dot_kamban_item_colon_last_child;
        public string _sharp_KambanBody_space__dot_kamban_item;
        public string _sharp_KambanBody_space__dot_kamban_item_colon_hover;
        public string _sharp_KambanBody_space__dot_kamban_item_dot_changed;
        public string _sharp_KambanBody_space__dot_kamban_item_space__dot_ui_icon;
        public string _sharp_RecordHeader;
        public string _sharp_RecordInfo;
        public string _sharp_RecordInfo_space_div;
        public string _sharp_RecordInfo_space_div_space_p;
        public string _sharp_RecordInfo_space_div_space_p_space__dot_elapsed_time;
        public string _sharp_RecordSwitchers;
        public string _sharp_RecordSwitchers_space___space__asterisk_;
        public string _sharp_RecordSwitchers_space__dot_current;
        public string _sharp_Editor;
        public string _sharp_EditorTabsContainer;
        public string _sharp_EditorTabsContainer_dot_max;
        public string _sharp_MailEditorTabsContainer;
        public string _sharp_EditorComments;
        public string _sharp_EditorComments_space__dot_title_header;
        public string _sharp_CommentField;
        public string _sharp_OutgoingMailsForm;
        public string _sharp_OutgoingMailsForm_space___space__dot_item;
        public string _sharp_OutgoingMailsForm_space__dot_content;
        public string _sharp_DropDownSearchDialogForm;
        public string _sharp_ViewTabsContainer;
        public string _sharp_SearchResults;
        public string _sharp_SearchResults_space__dot_count;
        public string _sharp_SearchResults_space__dot_count_space__dot_label;
        public string _sharp_SearchResults_space__dot_count_space__dot_data;
        public string _sharp_SearchResults_space__dot_result;
        public string _sharp_SearchResults_space__dot_result_space___space_ul;
        public string _sharp_SearchResults_space__dot_result_space___space_h3;
        public string _sharp_SearchResults_space__dot_result_space___space_h3_space___space_a;
        public string _sharp_SearchResults_space__dot_result_space___space_p;
        public string _sharp_SearchResults_space__dot_result_colon_hover;
        public string _sharp_MainCommandsContainer;
        public string _sharp_MainCommands;
        public string _sharp_MainCommands_space___space_button;
        public string _sharp_BottomMargin;
        public string _sharp_Footer;
        public string _sharp_Footer_space_a;
        public string _dot_main_form;
        public string _dot_nav_sites;
        public string _dot_nav_site;
        public string _dot_nav_site_dot_sites;
        public string _dot_nav_site_space__dot_heading;
        public string _dot_nav_site_space__dot_stacking1;
        public string _dot_nav_site_space__dot_stacking2;
        public string _dot_nav_site_space_a;
        public string _dot_nav_site_space_a_colon_hover;
        public string _dot_nav_site_dot_has_image_space_a;
        public string _dot_nav_site_space_span_dot_title;
        public string _dot_nav_site_dot_to_parent;
        public string _dot_nav_site_dot_to_parent_space_a;
        public string _dot_nav_site_dot_to_parent_dot_has_image_space_a;
        public string _dot_nav_site_dot_to_parent_space__dot_ui_icon;
        public string _dot_nav_site_space__dot_site_image_thumbnail;
        public string _dot_nav_site_space__dot_site_image_icon;
        public string _dot_nav_site_space__dot_conditions;
        public string _dot_nav_site_space__dot_conditions_space_span;
        public string _dot_nav_site_space__dot_conditions_space_span_dot_overdue;
        public string _dot_nav_site_space__dot_conditions_space_span_dot_elapsed_time_dot_old;
        public string _dot_error_page;
        public string _dot_error_page_title;
        public string _dot_error_page_body;
        public string _dot_error_page_message;
        public string _dot_error_page_action;
        public string _dot_error_page_action_space_em;
        public string _dot_error_page_stacktrace;
        public string _dot_fieldset;
        public string _dot_fieldset_dot_enclosed;
        public string _dot_fieldset_dot_enclosed_half;
        public string _dot_fieldset_dot_enclosed_thin;
        public string _dot_fieldset_dot_enclosed_thin_space__class_asterisk__equal__yen_field_auto_yen__;
        public string _dot_fieldset_dot_enclosed_auto;
        public string _dot_fieldset_class_caret__equal__yen_enclosed_yen___space___space_legend;
        public string _dot_command_field;
        public string _dot_command_field_space___space_button;
        public string _dot_command_center;
        public string _dot_command_center_space___space_button;
        public string _dot_command_left;
        public string _dot_command_left_space___space__asterisk_;
        public string _dot_command_left_space___space_button;
        public string _dot_command_left_space___space__dot_ui_icon;
        public string _dot_command_right;
        public string _dot_command_right_space___space_button;
        public string _dot_field_normal;
        public string _dot_field_normal_space___space__dot_field_label;
        public string _dot_field_normal_space___space__dot_field_control;
        public string _dot_field_normal_space__dot_container_normal;
        public string _dot_field_normal_space___space__dot_buttons;
        public string _dot_field_normal_space__dot_control_text;
        public string _dot_field_wide;
        public string _dot_field_wide_space___space__dot_field_label;
        public string _dot_field_wide_space___space__dot_field_control;
        public string _dot_field_wide_space__dot_container_normal;
        public string _dot_field_auto;
        public string _dot_field_auto_space___space__dot_field_label;
        public string _dot_field_auto_space___space__dot_field_label_space___space_label;
        public string _dot_field_auto_space___space__dot_field_control;
        public string _dot_field_auto_thin;
        public string _dot_field_auto_thin_space___space__dot_field_label;
        public string _dot_field_auto_thin_space___space__dot_field_label_space___space_label;
        public string _dot_field_auto_thin_space___space__dot_field_control;
        public string _dot_field_auto_thin_space_select;
        public string _dot_field_vertical;
        public string _dot_field_vertical_space___space__dot_field_label;
        public string _dot_field_vertical_space___space__dot_field_control;
        public string _dot_field_label;
        public string _dot_field_control_space__dot_unit;
        public string _dot_container_left;
        public string _dot_container_right;
        public string _dot_container_right_space___space__asterisk_;
        public string _dot_control_text;
        public string _dot_control_textbox;
        public string _dot_control_textbox_dot_with_unit;
        public string _dot_control_textarea;
        public string _dot_control_markup;
        public string _dot_md;
        public string _dot_md_space_h1;
        public string _dot_md_space_h2;
        public string _dot_md_space_h3;
        public string _dot_md_space_h4;
        public string _dot_md_space_h5;
        public string _dot_md_space_h6;
        public string _dot_md_space_ol;
        public string _dot_md_space_p;
        public string _dot_md_space_table;
        public string _dot_md_space_td;
        public string _dot_md_space_th;
        public string _dot_md_space_tbody_space_tr_colon_nth_child_odd_;
        public string _dot_md_space_ul;
        public string _dot_control_markdown;
        public string _dot_control_dropdown;
        public string _dot_control_spinner;
        public string _dot_control_checkbox;
        public string _dot_control_checkbox_space__plus__space_label;
        public string _dot_control_radio;
        public string _dot_control_radio_space__plus__space_label;
        public string _dot_control_slider;
        public string _dot_control_slider_ui;
        public string _dot_control_anchor;
        public string _dot_container_selectable_space__dot_wrapper;
        public string _dot_control_selectable;
        public string _dot_control_selectable_space_li;
        public string _dot_control_selectable_space__dot_ui_selecting;
        public string _dot_control_selectable_space__dot_ui_selected;
        public string _dot_control_basket;
        public string _dot_control_basket_space___space_li;
        public string _dot_control_basket_space___space_li_space___space_span;
        public string _dot_comment;
        public string _dot_comment_space___space__dot_time;
        public string _dot_comment_space___space__dot_body;
        public string _dot_comment_space___space__dot_button;
        public string _dot_user;
        public string _dot_user_space___space_span;
        public string _dot_dept;
        public string _dot_dept_space___space_span;
        public string _dot_both;
        public string _dot_hidden;
        public string _dot_right;
        public string _dot_tooltip;
        public string _dot_no_border;
        public string _dot_grid;
        public string _dot_grid_dot_fixed;
        public string _dot_grid_space___space_thead_space___space_tr_space___space_caption;
        public string _dot_grid_space___space_thead_space___space_tr_space___space_th;
        public string _dot_grid_space___space_thead_space___space_tr_space___space_th_space___space_div;
        public string _dot_grid_space___space_thead_space___space_tr_space___space_th_space_span;
        public string _dot_grid_space___space_thead_space___space_tr_colon_first_child_space___space_th_colon_first_child;
        public string _dot_grid_space___space_thead_space___space_tr_colon_first_child_space___space_th_colon_last_child;
        public string _dot_grid_space___space_thead_space___space_tr_space___space_th_dot_sortable_colon_hover;
        public string _dot_grid_space___space_tbody_space___space_tr_space___space_td;
        public string _dot_grid_space___space_tbody_space___space_tr_space__class_asterisk__equal__yen_status__yen__;
        public string _dot_grid_space___space_tbody_space___space_tr_space___space_th;
        public string _dot_grid_row;
        public string _dot_grid_row_space__dot_comment;
        public string _dot_grid_row_colon_hover;
        public string _dot_grid_row_colon_hover_space__dot_comment;
        public string _dot_grid_row_colon_hover_space__dot_grid_title_body;
        public string _dot_grid_row_space_p;
        public string _dot_grid_row_space_p_dot_body;
        public string _dot_grid_title_body;
        public string _dot_grid_title_body_space___space__dot_title_space__plus__space__dot_body;
        public string _dot_links;
        public string _dot_link_creations_space_button;
        public string _dot_text;
        public string _dot_datepicker;
        public string _dot_dropdown;
        public string _class_asterisk__equal__yen_limit__yen__;
        public string _dot_limit_normal;
        public string _dot_limit_warning1;
        public string _dot_limit_warning2;
        public string _dot_limit_warning3;
        public string _dot_message;
        public string _dot_message_space___space_span;
        public string _dot_message_dialog;
        public string _dot_message_form_bottom;
        public string _dot_alert_error;
        public string _dot_alert_success;
        public string _dot_alert_warning;
        public string _dot_alert_information;
        public string label_dot_error;
        public string _dot_error;
        public string _dot_button_edit_markdown;
        public string _dot_button_delete_address;
        public string _dot_button_right_justified;
        public string _dot_status_new;
        public string _dot_status_preparation;
        public string _dot_status_inprogress;
        public string _dot_status_review;
        public string _dot_status_closed;
        public string _dot_status_rejected;
        public string h3_dot_title_header;
        public string _dot_outgoing_mail_space__dot_dialog;
        public string _dot_outgoing_mail_space__dot_ui_dialog_titlebar;
        public string _dot_svg_work_value;
        public string _dot_svg_work_value_space_text;
        public string _dot_svg_work_value_space_rect_colon_nth_of_type_1_;
        public string _dot_svg_work_value_space_rect_colon_nth_of_type_2_;
        public string _dot_svg_progress_rate;
        public string _dot_svg_progress_rate_space_text;
        public string _dot_svg_progress_rate_dot_warning_space_text;
        public string _dot_svg_progress_rate_space_rect_colon_nth_of_type_1_;
        public string _dot_svg_progress_rate_space_rect_colon_nth_of_type_2_;
        public string _dot_svg_progress_rate_space_rect_colon_nth_of_type_3_;
        public string _dot_svg_progress_rate_dot_warning_space_rect_colon_nth_of_type_3_;
        public string _dot_axis;
        public string _dot_h2;
        public string _dot_h3;
        public string _dot_h4;
        public string _dot_h5;
        public string _dot_h6;
        public string _dot_h2_space___space_h2;
        public string _dot_h3_space___space_h3;
        public string _dot_h4_space___space_h4;
        public string _dot_h5_space___space_h5;
        public string _dot_h6_space___space_h6;
        public string _dot_w50;
        public string _dot_w100;
        public string _dot_w150;
        public string _dot_w200;
        public string _dot_w250;
        public string _dot_w300;
        public string _dot_w350;
        public string _dot_w400;
        public string _dot_w450;
        public string _dot_w500;
        public string _dot_w550;
        public string _dot_w600;
        public string _dot_h100;
        public string _dot_h150;
        public string _dot_h200;
        public string _dot_h250;
        public string _dot_h300;
        public string _dot_h350;
        public string _dot_h400;
        public string _dot_h450;
        public string _dot_h500;
        public string _dot_h550;
        public string _dot_h600;
        public string _dot_m_l10;
        public string _dot_m_l20;
        public string _dot_m_l30;
        public string _dot_m_l40;
        public string _dot_m_l50;
        public string _dot_paragraph;
        public string _dot_dialog;
        public string _dot_dialog_space__dot_fieldset;
        public string _dot_link;
        public string _dot_histories_form;
        public string _dot_ui_widget_space_input_comma__space__dot_ui_widget_space_select_comma__space__dot_ui_widget_space_button;
        public string _dot_ui_widget_space_textarea;
        public string _dot_ui_widget;
        public string _dot_ui_button;
        public string _dot_ui_dialog;
        public string _dot_ui_icon_dot_a;
        public string _dot_ui_spinner;
        public string _dot_ui_multiselect;
        public string _dot_ui_multiselect_checkboxes;
        public string _dot_ui_multiselect_checkboxes_space_input;
        public string _dot_ui_corner_all_dot_ui_state_hover;
        public string _dot_height_auto;
        public string _dot_focus_inform;
        public string _dot_sortable;
        public string _dot_menu_sort;
        public string _dot_menu_sort_space___space_li;
        public string _dot_menu_sort_space___space_li_colon_hover;
        public string _dot_menu_sort_space___space_li_space___space__asterisk_;
    }

    public class CssTable
    {
        public CssDefinition html = new CssDefinition();
        public CssDefinition body = new CssDefinition();
        public CssDefinition h1 = new CssDefinition();
        public CssDefinition h2 = new CssDefinition();
        public CssDefinition h3 = new CssDefinition();
        public CssDefinition legend = new CssDefinition();
        public CssDefinition legend_space___space__asterisk_ = new CssDefinition();
        public CssDefinition label = new CssDefinition();
        public CssDefinition input = new CssDefinition();
        public CssDefinition select = new CssDefinition();
        public CssDefinition table = new CssDefinition();
        public CssDefinition td = new CssDefinition();
        public CssDefinition pre = new CssDefinition();
        public CssDefinition _sharp_Logo = new CssDefinition();
        public CssDefinition _sharp_CorpLogo = new CssDefinition();
        public CssDefinition _sharp_ProductLogo = new CssDefinition();
        public CssDefinition _sharp_Login = new CssDefinition();
        public CssDefinition _sharp_LoginCommands = new CssDefinition();
        public CssDefinition _sharp_Demo = new CssDefinition();
        public CssDefinition _sharp_DemoFields = new CssDefinition();
        public CssDefinition _sharp_Breadcrumb = new CssDefinition();
        public CssDefinition _sharp_Breadcrumb_space__dot_item = new CssDefinition();
        public CssDefinition _sharp_Breadcrumb_space__dot_separator = new CssDefinition();
        public CssDefinition _sharp_Header = new CssDefinition();
        public CssDefinition _sharp_Navigations = new CssDefinition();
        public CssDefinition _sharp_NavigationMenu = new CssDefinition();
        public CssDefinition _sharp_NavigationMenu_space___space_li = new CssDefinition();
        public CssDefinition _sharp_NavigationMenu_space___space_li_space___space_div = new CssDefinition();
        public CssDefinition _sharp_NavigationMenu_space___space_li_space___space_div_colon_hover = new CssDefinition();
        public CssDefinition _sharp_NavigationMenu_space___space_li_dot_sub_menu_space___space_div_dot_hover = new CssDefinition();
        public CssDefinition _sharp_NavigationMenu_space___space_li_space___space_div_space___space_a = new CssDefinition();
        public CssDefinition _sharp_NavigationMenu_space___space_li_space___space_div_colon_hover_space___space_a = new CssDefinition();
        public CssDefinition _sharp_NavigationMenu_space__dot_menu = new CssDefinition();
        public CssDefinition _sharp_NavigationMenu_space__dot_menu_space___space_li_space___space_a = new CssDefinition();
        public CssDefinition _sharp_NavigationMenu_space__dot_menu_space___space_li_space___space_a_dot_ui_state_active = new CssDefinition();
        public CssDefinition _sharp_SearchField = new CssDefinition();
        public CssDefinition _sharp_Search = new CssDefinition();
        public CssDefinition _sharp_Application = new CssDefinition();
        public CssDefinition _sharp_Application_space___space__dot_site_image_icon = new CssDefinition();
        public CssDefinition _sharp_HeaderTitleContainer = new CssDefinition();
        public CssDefinition _sharp_HeaderTitle = new CssDefinition();
        public CssDefinition _sharp_Notes_space___space__asterisk_ = new CssDefinition();
        public CssDefinition _sharp_Notes_space___space__dot_history = new CssDefinition();
        public CssDefinition _sharp_Notes_space___space__dot_readonly = new CssDefinition();
        public CssDefinition _sharp_ViewSelectorField = new CssDefinition();
        public CssDefinition _sharp_ViewFilters = new CssDefinition();
        public CssDefinition _sharp_ViewFilters_dot_reduced = new CssDefinition();
        public CssDefinition _sharp_ViewFilters_space___space__dot_field_auto_thin = new CssDefinition();
        public CssDefinition _sharp_ViewFilters_Reset = new CssDefinition();
        public CssDefinition _sharp_ViewFilters_space___space__dot_display_control = new CssDefinition();
        public CssDefinition _sharp_Aggregations = new CssDefinition();
        public CssDefinition _sharp_Aggregations_dot_reduced = new CssDefinition();
        public CssDefinition _sharp_Aggregations_space__dot_label = new CssDefinition();
        public CssDefinition _sharp_Aggregations_space__dot_label_dot_overdue = new CssDefinition();
        public CssDefinition _sharp_Aggregations_space__dot_data = new CssDefinition();
        public CssDefinition _sharp_Aggregations_space__dot_data_dot_overdue = new CssDefinition();
        public CssDefinition _sharp_Aggregations_space_em = new CssDefinition();
        public CssDefinition _sharp_Aggregations_space___space__dot_display_control = new CssDefinition();
        public CssDefinition _sharp_Gantt = new CssDefinition();
        public CssDefinition _sharp_Gantt_space__dot_date = new CssDefinition();
        public CssDefinition _sharp_Gantt_space__dot_now = new CssDefinition();
        public CssDefinition _sharp_Gantt_space__dot_planned_space_rect = new CssDefinition();
        public CssDefinition _sharp_Gantt_space__dot_planned_space_rect_dot_summary = new CssDefinition();
        public CssDefinition _sharp_Gantt_space__dot_earned_space_rect = new CssDefinition();
        public CssDefinition _sharp_Gantt_space__dot_earned_space_rect_dot_summary = new CssDefinition();
        public CssDefinition _sharp_Gantt_space_rect_dot_delay = new CssDefinition();
        public CssDefinition _sharp_Gantt_space_rect_dot_delay_dot_summary = new CssDefinition();
        public CssDefinition _sharp_Gantt_space_rect_dot_completed = new CssDefinition();
        public CssDefinition _sharp_Gantt_space__dot_title_space_text = new CssDefinition();
        public CssDefinition _sharp_Gantt_space__dot_title_space_text_dot_delay = new CssDefinition();
        public CssDefinition _sharp_Gantt_space__dot_title_space_text_dot_summary = new CssDefinition();
        public CssDefinition _sharp_GanttAxis = new CssDefinition();
        public CssDefinition _sharp_GanttAxis_space_text = new CssDefinition();
        public CssDefinition _sharp_BurnDown = new CssDefinition();
        public CssDefinition _sharp_BurnDown_space__dot_now = new CssDefinition();
        public CssDefinition _sharp_BurnDown_space__dot_total = new CssDefinition();
        public CssDefinition _sharp_BurnDown_space__dot_planned = new CssDefinition();
        public CssDefinition _sharp_BurnDown_space__dot_earned = new CssDefinition();
        public CssDefinition _sharp_BurnDown_space__dot_total_space_circle = new CssDefinition();
        public CssDefinition _sharp_BurnDown_space__dot_planned_space_circle = new CssDefinition();
        public CssDefinition _sharp_BurnDown_space__dot_earned_space_circle = new CssDefinition();
        public CssDefinition _sharp_BurnDownDetails_space___space_tbody_space___space_tr_space___space_td = new CssDefinition();
        public CssDefinition _sharp_BurnDownDetails_space___space_tbody_space___space_tr_space___space_td_dot_warning = new CssDefinition();
        public CssDefinition _sharp_BurnDownDetails_space___space_tbody_space___space_tr_space___space_td_dot_difference = new CssDefinition();
        public CssDefinition _sharp_BurnDownDetails_space___space_tbody_space___space_tr_space___space_td_dot_difference_dot_warning = new CssDefinition();
        public CssDefinition _sharp_BurnDownDetails_space__dot_user_info = new CssDefinition();
        public CssDefinition _sharp_BurnDownDetails_space__dot_items = new CssDefinition();
        public CssDefinition _sharp_BurnDownDetails_space__dot_items_space_a = new CssDefinition();
        public CssDefinition _sharp_BurnDownDetails_space__dot_items_space_a_colon_hover = new CssDefinition();
        public CssDefinition _sharp_TimeSeries = new CssDefinition();
        public CssDefinition _sharp_TimeSeries_space__dot_surface = new CssDefinition();
        public CssDefinition _sharp_TimeSeries_space__dot_index = new CssDefinition();
        public CssDefinition _sharp_KambanBody_space__dot_kamban_row = new CssDefinition();
        public CssDefinition _sharp_KambanBody_space__dot_kamban_container_space___space_div = new CssDefinition();
        public CssDefinition _sharp_KambanBody_space__dot_kamban_container_dot_hover = new CssDefinition();
        public CssDefinition _sharp_KambanBody_space__dot_kamban_container_space__dot_kamban_item_colon_last_child = new CssDefinition();
        public CssDefinition _sharp_KambanBody_space__dot_kamban_item = new CssDefinition();
        public CssDefinition _sharp_KambanBody_space__dot_kamban_item_colon_hover = new CssDefinition();
        public CssDefinition _sharp_KambanBody_space__dot_kamban_item_dot_changed = new CssDefinition();
        public CssDefinition _sharp_KambanBody_space__dot_kamban_item_space__dot_ui_icon = new CssDefinition();
        public CssDefinition _sharp_RecordHeader = new CssDefinition();
        public CssDefinition _sharp_RecordInfo = new CssDefinition();
        public CssDefinition _sharp_RecordInfo_space_div = new CssDefinition();
        public CssDefinition _sharp_RecordInfo_space_div_space_p = new CssDefinition();
        public CssDefinition _sharp_RecordInfo_space_div_space_p_space__dot_elapsed_time = new CssDefinition();
        public CssDefinition _sharp_RecordSwitchers = new CssDefinition();
        public CssDefinition _sharp_RecordSwitchers_space___space__asterisk_ = new CssDefinition();
        public CssDefinition _sharp_RecordSwitchers_space__dot_current = new CssDefinition();
        public CssDefinition _sharp_Editor = new CssDefinition();
        public CssDefinition _sharp_EditorTabsContainer = new CssDefinition();
        public CssDefinition _sharp_EditorTabsContainer_dot_max = new CssDefinition();
        public CssDefinition _sharp_MailEditorTabsContainer = new CssDefinition();
        public CssDefinition _sharp_EditorComments = new CssDefinition();
        public CssDefinition _sharp_EditorComments_space__dot_title_header = new CssDefinition();
        public CssDefinition _sharp_CommentField = new CssDefinition();
        public CssDefinition _sharp_OutgoingMailsForm = new CssDefinition();
        public CssDefinition _sharp_OutgoingMailsForm_space___space__dot_item = new CssDefinition();
        public CssDefinition _sharp_OutgoingMailsForm_space__dot_content = new CssDefinition();
        public CssDefinition _sharp_DropDownSearchDialogForm = new CssDefinition();
        public CssDefinition _sharp_ViewTabsContainer = new CssDefinition();
        public CssDefinition _sharp_SearchResults = new CssDefinition();
        public CssDefinition _sharp_SearchResults_space__dot_count = new CssDefinition();
        public CssDefinition _sharp_SearchResults_space__dot_count_space__dot_label = new CssDefinition();
        public CssDefinition _sharp_SearchResults_space__dot_count_space__dot_data = new CssDefinition();
        public CssDefinition _sharp_SearchResults_space__dot_result = new CssDefinition();
        public CssDefinition _sharp_SearchResults_space__dot_result_space___space_ul = new CssDefinition();
        public CssDefinition _sharp_SearchResults_space__dot_result_space___space_h3 = new CssDefinition();
        public CssDefinition _sharp_SearchResults_space__dot_result_space___space_h3_space___space_a = new CssDefinition();
        public CssDefinition _sharp_SearchResults_space__dot_result_space___space_p = new CssDefinition();
        public CssDefinition _sharp_SearchResults_space__dot_result_colon_hover = new CssDefinition();
        public CssDefinition _sharp_MainCommandsContainer = new CssDefinition();
        public CssDefinition _sharp_MainCommands = new CssDefinition();
        public CssDefinition _sharp_MainCommands_space___space_button = new CssDefinition();
        public CssDefinition _sharp_BottomMargin = new CssDefinition();
        public CssDefinition _sharp_Footer = new CssDefinition();
        public CssDefinition _sharp_Footer_space_a = new CssDefinition();
        public CssDefinition _dot_main_form = new CssDefinition();
        public CssDefinition _dot_nav_sites = new CssDefinition();
        public CssDefinition _dot_nav_site = new CssDefinition();
        public CssDefinition _dot_nav_site_dot_sites = new CssDefinition();
        public CssDefinition _dot_nav_site_space__dot_heading = new CssDefinition();
        public CssDefinition _dot_nav_site_space__dot_stacking1 = new CssDefinition();
        public CssDefinition _dot_nav_site_space__dot_stacking2 = new CssDefinition();
        public CssDefinition _dot_nav_site_space_a = new CssDefinition();
        public CssDefinition _dot_nav_site_space_a_colon_hover = new CssDefinition();
        public CssDefinition _dot_nav_site_dot_has_image_space_a = new CssDefinition();
        public CssDefinition _dot_nav_site_space_span_dot_title = new CssDefinition();
        public CssDefinition _dot_nav_site_dot_to_parent = new CssDefinition();
        public CssDefinition _dot_nav_site_dot_to_parent_space_a = new CssDefinition();
        public CssDefinition _dot_nav_site_dot_to_parent_dot_has_image_space_a = new CssDefinition();
        public CssDefinition _dot_nav_site_dot_to_parent_space__dot_ui_icon = new CssDefinition();
        public CssDefinition _dot_nav_site_space__dot_site_image_thumbnail = new CssDefinition();
        public CssDefinition _dot_nav_site_space__dot_site_image_icon = new CssDefinition();
        public CssDefinition _dot_nav_site_space__dot_conditions = new CssDefinition();
        public CssDefinition _dot_nav_site_space__dot_conditions_space_span = new CssDefinition();
        public CssDefinition _dot_nav_site_space__dot_conditions_space_span_dot_overdue = new CssDefinition();
        public CssDefinition _dot_nav_site_space__dot_conditions_space_span_dot_elapsed_time_dot_old = new CssDefinition();
        public CssDefinition _dot_error_page = new CssDefinition();
        public CssDefinition _dot_error_page_title = new CssDefinition();
        public CssDefinition _dot_error_page_body = new CssDefinition();
        public CssDefinition _dot_error_page_message = new CssDefinition();
        public CssDefinition _dot_error_page_action = new CssDefinition();
        public CssDefinition _dot_error_page_action_space_em = new CssDefinition();
        public CssDefinition _dot_error_page_stacktrace = new CssDefinition();
        public CssDefinition _dot_fieldset = new CssDefinition();
        public CssDefinition _dot_fieldset_dot_enclosed = new CssDefinition();
        public CssDefinition _dot_fieldset_dot_enclosed_half = new CssDefinition();
        public CssDefinition _dot_fieldset_dot_enclosed_thin = new CssDefinition();
        public CssDefinition _dot_fieldset_dot_enclosed_thin_space__class_asterisk__equal__yen_field_auto_yen__ = new CssDefinition();
        public CssDefinition _dot_fieldset_dot_enclosed_auto = new CssDefinition();
        public CssDefinition _dot_fieldset_class_caret__equal__yen_enclosed_yen___space___space_legend = new CssDefinition();
        public CssDefinition _dot_command_field = new CssDefinition();
        public CssDefinition _dot_command_field_space___space_button = new CssDefinition();
        public CssDefinition _dot_command_center = new CssDefinition();
        public CssDefinition _dot_command_center_space___space_button = new CssDefinition();
        public CssDefinition _dot_command_left = new CssDefinition();
        public CssDefinition _dot_command_left_space___space__asterisk_ = new CssDefinition();
        public CssDefinition _dot_command_left_space___space_button = new CssDefinition();
        public CssDefinition _dot_command_left_space___space__dot_ui_icon = new CssDefinition();
        public CssDefinition _dot_command_right = new CssDefinition();
        public CssDefinition _dot_command_right_space___space_button = new CssDefinition();
        public CssDefinition _dot_field_normal = new CssDefinition();
        public CssDefinition _dot_field_normal_space___space__dot_field_label = new CssDefinition();
        public CssDefinition _dot_field_normal_space___space__dot_field_control = new CssDefinition();
        public CssDefinition _dot_field_normal_space__dot_container_normal = new CssDefinition();
        public CssDefinition _dot_field_normal_space___space__dot_buttons = new CssDefinition();
        public CssDefinition _dot_field_normal_space__dot_control_text = new CssDefinition();
        public CssDefinition _dot_field_wide = new CssDefinition();
        public CssDefinition _dot_field_wide_space___space__dot_field_label = new CssDefinition();
        public CssDefinition _dot_field_wide_space___space__dot_field_control = new CssDefinition();
        public CssDefinition _dot_field_wide_space__dot_container_normal = new CssDefinition();
        public CssDefinition _dot_field_auto = new CssDefinition();
        public CssDefinition _dot_field_auto_space___space__dot_field_label = new CssDefinition();
        public CssDefinition _dot_field_auto_space___space__dot_field_label_space___space_label = new CssDefinition();
        public CssDefinition _dot_field_auto_space___space__dot_field_control = new CssDefinition();
        public CssDefinition _dot_field_auto_thin = new CssDefinition();
        public CssDefinition _dot_field_auto_thin_space___space__dot_field_label = new CssDefinition();
        public CssDefinition _dot_field_auto_thin_space___space__dot_field_label_space___space_label = new CssDefinition();
        public CssDefinition _dot_field_auto_thin_space___space__dot_field_control = new CssDefinition();
        public CssDefinition _dot_field_auto_thin_space_select = new CssDefinition();
        public CssDefinition _dot_field_vertical = new CssDefinition();
        public CssDefinition _dot_field_vertical_space___space__dot_field_label = new CssDefinition();
        public CssDefinition _dot_field_vertical_space___space__dot_field_control = new CssDefinition();
        public CssDefinition _dot_field_label = new CssDefinition();
        public CssDefinition _dot_field_control_space__dot_unit = new CssDefinition();
        public CssDefinition _dot_container_left = new CssDefinition();
        public CssDefinition _dot_container_right = new CssDefinition();
        public CssDefinition _dot_container_right_space___space__asterisk_ = new CssDefinition();
        public CssDefinition _dot_control_text = new CssDefinition();
        public CssDefinition _dot_control_textbox = new CssDefinition();
        public CssDefinition _dot_control_textbox_dot_with_unit = new CssDefinition();
        public CssDefinition _dot_control_textarea = new CssDefinition();
        public CssDefinition _dot_control_markup = new CssDefinition();
        public CssDefinition _dot_md = new CssDefinition();
        public CssDefinition _dot_md_space_h1 = new CssDefinition();
        public CssDefinition _dot_md_space_h2 = new CssDefinition();
        public CssDefinition _dot_md_space_h3 = new CssDefinition();
        public CssDefinition _dot_md_space_h4 = new CssDefinition();
        public CssDefinition _dot_md_space_h5 = new CssDefinition();
        public CssDefinition _dot_md_space_h6 = new CssDefinition();
        public CssDefinition _dot_md_space_ol = new CssDefinition();
        public CssDefinition _dot_md_space_p = new CssDefinition();
        public CssDefinition _dot_md_space_table = new CssDefinition();
        public CssDefinition _dot_md_space_td = new CssDefinition();
        public CssDefinition _dot_md_space_th = new CssDefinition();
        public CssDefinition _dot_md_space_tbody_space_tr_colon_nth_child_odd_ = new CssDefinition();
        public CssDefinition _dot_md_space_ul = new CssDefinition();
        public CssDefinition _dot_control_markdown = new CssDefinition();
        public CssDefinition _dot_control_dropdown = new CssDefinition();
        public CssDefinition _dot_control_spinner = new CssDefinition();
        public CssDefinition _dot_control_checkbox = new CssDefinition();
        public CssDefinition _dot_control_checkbox_space__plus__space_label = new CssDefinition();
        public CssDefinition _dot_control_radio = new CssDefinition();
        public CssDefinition _dot_control_radio_space__plus__space_label = new CssDefinition();
        public CssDefinition _dot_control_slider = new CssDefinition();
        public CssDefinition _dot_control_slider_ui = new CssDefinition();
        public CssDefinition _dot_control_anchor = new CssDefinition();
        public CssDefinition _dot_container_selectable_space__dot_wrapper = new CssDefinition();
        public CssDefinition _dot_control_selectable = new CssDefinition();
        public CssDefinition _dot_control_selectable_space_li = new CssDefinition();
        public CssDefinition _dot_control_selectable_space__dot_ui_selecting = new CssDefinition();
        public CssDefinition _dot_control_selectable_space__dot_ui_selected = new CssDefinition();
        public CssDefinition _dot_control_basket = new CssDefinition();
        public CssDefinition _dot_control_basket_space___space_li = new CssDefinition();
        public CssDefinition _dot_control_basket_space___space_li_space___space_span = new CssDefinition();
        public CssDefinition _dot_comment = new CssDefinition();
        public CssDefinition _dot_comment_space___space__dot_time = new CssDefinition();
        public CssDefinition _dot_comment_space___space__dot_body = new CssDefinition();
        public CssDefinition _dot_comment_space___space__dot_button = new CssDefinition();
        public CssDefinition _dot_user = new CssDefinition();
        public CssDefinition _dot_user_space___space_span = new CssDefinition();
        public CssDefinition _dot_dept = new CssDefinition();
        public CssDefinition _dot_dept_space___space_span = new CssDefinition();
        public CssDefinition _dot_both = new CssDefinition();
        public CssDefinition _dot_hidden = new CssDefinition();
        public CssDefinition _dot_right = new CssDefinition();
        public CssDefinition _dot_tooltip = new CssDefinition();
        public CssDefinition _dot_no_border = new CssDefinition();
        public CssDefinition _dot_grid = new CssDefinition();
        public CssDefinition _dot_grid_dot_fixed = new CssDefinition();
        public CssDefinition _dot_grid_space___space_thead_space___space_tr_space___space_caption = new CssDefinition();
        public CssDefinition _dot_grid_space___space_thead_space___space_tr_space___space_th = new CssDefinition();
        public CssDefinition _dot_grid_space___space_thead_space___space_tr_space___space_th_space___space_div = new CssDefinition();
        public CssDefinition _dot_grid_space___space_thead_space___space_tr_space___space_th_space_span = new CssDefinition();
        public CssDefinition _dot_grid_space___space_thead_space___space_tr_colon_first_child_space___space_th_colon_first_child = new CssDefinition();
        public CssDefinition _dot_grid_space___space_thead_space___space_tr_colon_first_child_space___space_th_colon_last_child = new CssDefinition();
        public CssDefinition _dot_grid_space___space_thead_space___space_tr_space___space_th_dot_sortable_colon_hover = new CssDefinition();
        public CssDefinition _dot_grid_space___space_tbody_space___space_tr_space___space_td = new CssDefinition();
        public CssDefinition _dot_grid_space___space_tbody_space___space_tr_space__class_asterisk__equal__yen_status__yen__ = new CssDefinition();
        public CssDefinition _dot_grid_space___space_tbody_space___space_tr_space___space_th = new CssDefinition();
        public CssDefinition _dot_grid_row = new CssDefinition();
        public CssDefinition _dot_grid_row_space__dot_comment = new CssDefinition();
        public CssDefinition _dot_grid_row_colon_hover = new CssDefinition();
        public CssDefinition _dot_grid_row_colon_hover_space__dot_comment = new CssDefinition();
        public CssDefinition _dot_grid_row_colon_hover_space__dot_grid_title_body = new CssDefinition();
        public CssDefinition _dot_grid_row_space_p = new CssDefinition();
        public CssDefinition _dot_grid_row_space_p_dot_body = new CssDefinition();
        public CssDefinition _dot_grid_title_body = new CssDefinition();
        public CssDefinition _dot_grid_title_body_space___space__dot_title_space__plus__space__dot_body = new CssDefinition();
        public CssDefinition _dot_links = new CssDefinition();
        public CssDefinition _dot_link_creations_space_button = new CssDefinition();
        public CssDefinition _dot_text = new CssDefinition();
        public CssDefinition _dot_datepicker = new CssDefinition();
        public CssDefinition _dot_dropdown = new CssDefinition();
        public CssDefinition _class_asterisk__equal__yen_limit__yen__ = new CssDefinition();
        public CssDefinition _dot_limit_normal = new CssDefinition();
        public CssDefinition _dot_limit_warning1 = new CssDefinition();
        public CssDefinition _dot_limit_warning2 = new CssDefinition();
        public CssDefinition _dot_limit_warning3 = new CssDefinition();
        public CssDefinition _dot_message = new CssDefinition();
        public CssDefinition _dot_message_space___space_span = new CssDefinition();
        public CssDefinition _dot_message_dialog = new CssDefinition();
        public CssDefinition _dot_message_form_bottom = new CssDefinition();
        public CssDefinition _dot_alert_error = new CssDefinition();
        public CssDefinition _dot_alert_success = new CssDefinition();
        public CssDefinition _dot_alert_warning = new CssDefinition();
        public CssDefinition _dot_alert_information = new CssDefinition();
        public CssDefinition label_dot_error = new CssDefinition();
        public CssDefinition _dot_error = new CssDefinition();
        public CssDefinition _dot_button_edit_markdown = new CssDefinition();
        public CssDefinition _dot_button_delete_address = new CssDefinition();
        public CssDefinition _dot_button_right_justified = new CssDefinition();
        public CssDefinition _dot_status_new = new CssDefinition();
        public CssDefinition _dot_status_preparation = new CssDefinition();
        public CssDefinition _dot_status_inprogress = new CssDefinition();
        public CssDefinition _dot_status_review = new CssDefinition();
        public CssDefinition _dot_status_closed = new CssDefinition();
        public CssDefinition _dot_status_rejected = new CssDefinition();
        public CssDefinition h3_dot_title_header = new CssDefinition();
        public CssDefinition _dot_outgoing_mail_space__dot_dialog = new CssDefinition();
        public CssDefinition _dot_outgoing_mail_space__dot_ui_dialog_titlebar = new CssDefinition();
        public CssDefinition _dot_svg_work_value = new CssDefinition();
        public CssDefinition _dot_svg_work_value_space_text = new CssDefinition();
        public CssDefinition _dot_svg_work_value_space_rect_colon_nth_of_type_1_ = new CssDefinition();
        public CssDefinition _dot_svg_work_value_space_rect_colon_nth_of_type_2_ = new CssDefinition();
        public CssDefinition _dot_svg_progress_rate = new CssDefinition();
        public CssDefinition _dot_svg_progress_rate_space_text = new CssDefinition();
        public CssDefinition _dot_svg_progress_rate_dot_warning_space_text = new CssDefinition();
        public CssDefinition _dot_svg_progress_rate_space_rect_colon_nth_of_type_1_ = new CssDefinition();
        public CssDefinition _dot_svg_progress_rate_space_rect_colon_nth_of_type_2_ = new CssDefinition();
        public CssDefinition _dot_svg_progress_rate_space_rect_colon_nth_of_type_3_ = new CssDefinition();
        public CssDefinition _dot_svg_progress_rate_dot_warning_space_rect_colon_nth_of_type_3_ = new CssDefinition();
        public CssDefinition _dot_axis = new CssDefinition();
        public CssDefinition _dot_h2 = new CssDefinition();
        public CssDefinition _dot_h3 = new CssDefinition();
        public CssDefinition _dot_h4 = new CssDefinition();
        public CssDefinition _dot_h5 = new CssDefinition();
        public CssDefinition _dot_h6 = new CssDefinition();
        public CssDefinition _dot_h2_space___space_h2 = new CssDefinition();
        public CssDefinition _dot_h3_space___space_h3 = new CssDefinition();
        public CssDefinition _dot_h4_space___space_h4 = new CssDefinition();
        public CssDefinition _dot_h5_space___space_h5 = new CssDefinition();
        public CssDefinition _dot_h6_space___space_h6 = new CssDefinition();
        public CssDefinition _dot_w50 = new CssDefinition();
        public CssDefinition _dot_w100 = new CssDefinition();
        public CssDefinition _dot_w150 = new CssDefinition();
        public CssDefinition _dot_w200 = new CssDefinition();
        public CssDefinition _dot_w250 = new CssDefinition();
        public CssDefinition _dot_w300 = new CssDefinition();
        public CssDefinition _dot_w350 = new CssDefinition();
        public CssDefinition _dot_w400 = new CssDefinition();
        public CssDefinition _dot_w450 = new CssDefinition();
        public CssDefinition _dot_w500 = new CssDefinition();
        public CssDefinition _dot_w550 = new CssDefinition();
        public CssDefinition _dot_w600 = new CssDefinition();
        public CssDefinition _dot_h100 = new CssDefinition();
        public CssDefinition _dot_h150 = new CssDefinition();
        public CssDefinition _dot_h200 = new CssDefinition();
        public CssDefinition _dot_h250 = new CssDefinition();
        public CssDefinition _dot_h300 = new CssDefinition();
        public CssDefinition _dot_h350 = new CssDefinition();
        public CssDefinition _dot_h400 = new CssDefinition();
        public CssDefinition _dot_h450 = new CssDefinition();
        public CssDefinition _dot_h500 = new CssDefinition();
        public CssDefinition _dot_h550 = new CssDefinition();
        public CssDefinition _dot_h600 = new CssDefinition();
        public CssDefinition _dot_m_l10 = new CssDefinition();
        public CssDefinition _dot_m_l20 = new CssDefinition();
        public CssDefinition _dot_m_l30 = new CssDefinition();
        public CssDefinition _dot_m_l40 = new CssDefinition();
        public CssDefinition _dot_m_l50 = new CssDefinition();
        public CssDefinition _dot_paragraph = new CssDefinition();
        public CssDefinition _dot_dialog = new CssDefinition();
        public CssDefinition _dot_dialog_space__dot_fieldset = new CssDefinition();
        public CssDefinition _dot_link = new CssDefinition();
        public CssDefinition _dot_histories_form = new CssDefinition();
        public CssDefinition _dot_ui_widget_space_input_comma__space__dot_ui_widget_space_select_comma__space__dot_ui_widget_space_button = new CssDefinition();
        public CssDefinition _dot_ui_widget_space_textarea = new CssDefinition();
        public CssDefinition _dot_ui_widget = new CssDefinition();
        public CssDefinition _dot_ui_button = new CssDefinition();
        public CssDefinition _dot_ui_dialog = new CssDefinition();
        public CssDefinition _dot_ui_icon_dot_a = new CssDefinition();
        public CssDefinition _dot_ui_spinner = new CssDefinition();
        public CssDefinition _dot_ui_multiselect = new CssDefinition();
        public CssDefinition _dot_ui_multiselect_checkboxes = new CssDefinition();
        public CssDefinition _dot_ui_multiselect_checkboxes_space_input = new CssDefinition();
        public CssDefinition _dot_ui_corner_all_dot_ui_state_hover = new CssDefinition();
        public CssDefinition _dot_height_auto = new CssDefinition();
        public CssDefinition _dot_focus_inform = new CssDefinition();
        public CssDefinition _dot_sortable = new CssDefinition();
        public CssDefinition _dot_menu_sort = new CssDefinition();
        public CssDefinition _dot_menu_sort_space___space_li = new CssDefinition();
        public CssDefinition _dot_menu_sort_space___space_li_colon_hover = new CssDefinition();
        public CssDefinition _dot_menu_sort_space___space_li_space___space__asterisk_ = new CssDefinition();
    }

    public class DemoDefinition
    {
        public string Id; public string SavedId;
        public string Body; public string SavedBody;
        public string Type; public string SavedType;
        public string ParentId; public string SavedParentId;
        public string Title; public string SavedTitle;
        public decimal WorkValue; public decimal SavedWorkValue;
        public decimal ProgressRate; public decimal SavedProgressRate;
        public int Status; public int SavedStatus;
        public string ClassA; public string SavedClassA;
        public string ClassB; public string SavedClassB;
        public string ClassC; public string SavedClassC;
        public string Manager; public string SavedManager;
        public string Owner; public string SavedOwner;
        public string Creator; public string SavedCreator;
        public string Updator; public string SavedUpdator;
        public DateTime StartTime; public DateTime SavedStartTime;
        public DateTime CompletionTime; public DateTime SavedCompletionTime;
        public DateTime CreatedTime; public DateTime SavedCreatedTime;
        public DateTime UpdatedTime; public DateTime SavedUpdatedTime;

        public DemoDefinition()
        {
        }

        public DemoDefinition(Dictionary<string, string> propertyCollection)
        {
            if (propertyCollection.ContainsKey("Id")) Id = propertyCollection["Id"].ToString(); else Id = string.Empty;
            if (propertyCollection.ContainsKey("Body")) Body = propertyCollection["Body"].ToString(); else Body = string.Empty;
            if (propertyCollection.ContainsKey("Type")) Type = propertyCollection["Type"].ToString(); else Type = string.Empty;
            if (propertyCollection.ContainsKey("ParentId")) ParentId = propertyCollection["ParentId"].ToString(); else ParentId = string.Empty;
            if (propertyCollection.ContainsKey("Title")) Title = propertyCollection["Title"].ToString(); else Title = string.Empty;
            if (propertyCollection.ContainsKey("WorkValue")) WorkValue = propertyCollection["WorkValue"].ToDecimal(); else WorkValue = 0;
            if (propertyCollection.ContainsKey("ProgressRate")) ProgressRate = propertyCollection["ProgressRate"].ToDecimal(); else ProgressRate = 0;
            if (propertyCollection.ContainsKey("Status")) Status = propertyCollection["Status"].ToInt(); else Status = 0;
            if (propertyCollection.ContainsKey("ClassA")) ClassA = propertyCollection["ClassA"].ToString(); else ClassA = string.Empty;
            if (propertyCollection.ContainsKey("ClassB")) ClassB = propertyCollection["ClassB"].ToString(); else ClassB = string.Empty;
            if (propertyCollection.ContainsKey("ClassC")) ClassC = propertyCollection["ClassC"].ToString(); else ClassC = string.Empty;
            if (propertyCollection.ContainsKey("Manager")) Manager = propertyCollection["Manager"].ToString(); else Manager = string.Empty;
            if (propertyCollection.ContainsKey("Owner")) Owner = propertyCollection["Owner"].ToString(); else Owner = string.Empty;
            if (propertyCollection.ContainsKey("Creator")) Creator = propertyCollection["Creator"].ToString(); else Creator = string.Empty;
            if (propertyCollection.ContainsKey("Updator")) Updator = propertyCollection["Updator"].ToString(); else Updator = string.Empty;
            if (propertyCollection.ContainsKey("StartTime")) StartTime = propertyCollection["StartTime"].ToDateTime(); else StartTime = 0.ToDateTime();
            if (propertyCollection.ContainsKey("CompletionTime")) CompletionTime = propertyCollection["CompletionTime"].ToDateTime(); else CompletionTime = 0.ToDateTime();
            if (propertyCollection.ContainsKey("CreatedTime")) CreatedTime = propertyCollection["CreatedTime"].ToDateTime(); else CreatedTime = 0.ToDateTime();
            if (propertyCollection.ContainsKey("UpdatedTime")) UpdatedTime = propertyCollection["UpdatedTime"].ToDateTime(); else UpdatedTime = 0.ToDateTime();
        }

        public object this[string key]
        {
            get{
                switch(key)
                {
                    case "Id": return Id;
                    case "Body": return Body;
                    case "Type": return Type;
                    case "ParentId": return ParentId;
                    case "Title": return Title;
                    case "WorkValue": return WorkValue;
                    case "ProgressRate": return ProgressRate;
                    case "Status": return Status;
                    case "ClassA": return ClassA;
                    case "ClassB": return ClassB;
                    case "ClassC": return ClassC;
                    case "Manager": return Manager;
                    case "Owner": return Owner;
                    case "Creator": return Creator;
                    case "Updator": return Updator;
                    case "StartTime": return StartTime;
                    case "CompletionTime": return CompletionTime;
                    case "CreatedTime": return CreatedTime;
                    case "UpdatedTime": return UpdatedTime;
                    default: return null;
                }
            }
        }

        public void RestoreBySavedMemory()
        {
            Id = SavedId;
            Body = SavedBody;
            Type = SavedType;
            ParentId = SavedParentId;
            Title = SavedTitle;
            WorkValue = SavedWorkValue;
            ProgressRate = SavedProgressRate;
            Status = SavedStatus;
            ClassA = SavedClassA;
            ClassB = SavedClassB;
            ClassC = SavedClassC;
            Manager = SavedManager;
            Owner = SavedOwner;
            Creator = SavedCreator;
            Updator = SavedUpdator;
            StartTime = SavedStartTime;
            CompletionTime = SavedCompletionTime;
            CreatedTime = SavedCreatedTime;
            UpdatedTime = SavedUpdatedTime;
        }
    }

    public class DemoColumn2nd
    {
        public string Dept1;
        public string Dept2;
        public string Dept3;
        public string Dept4;
        public string Dept5;
        public string Dept6;
        public string Dept7;
        public string Dept8;
        public string User1;
        public string User2;
        public string User3;
        public string User4;
        public string User5;
        public string User6;
        public string User7;
        public string User8;
        public string User9;
        public string User10;
        public string User11;
        public string User12;
        public string User13;
        public string User14;
        public string User15;
        public string User16;
        public string User17;
        public string User18;
        public string User19;
        public string User20;
        public string Site1;
        public string Site2;
        public string Site3;
        public string Site4;
        public string DefineNetworks;
        public string DefineServers;
        public string DefineSecurity;
        public string DefineOperationSystems;
        public string DesignNetworks;
        public string DesignServers;
        public string DesignOperationSystems;
        public string DesignApplication;
        public string DeveropDataMigrationTools;
        public string ConfigNetwork;
        public string ConfigServers;
        public string ConfigOperationSystems;
        public string ConfigApplications;
        public string TestNetworks;
        public string TestServers;
        public string TestOperationSystems;
        public string TestApplications;
        public string WriteOperationDocuments;
        public string WriteUserDocuments;
        public string DesignSupportDesk;
        public string TestSystems;
        public string Transition;
        public string Report;
        public string Issue1;
        public string Result1;
        public string Comment1;
        public string Comment2;
        public string Comment3;
        public string Comment4;
        public string Comment5;
        public string Comment6;
        public string Comment7;
        public string Comment8;
    }

    public class DemoTable
    {
        public DemoDefinition Dept1 = new DemoDefinition();
        public DemoDefinition Dept2 = new DemoDefinition();
        public DemoDefinition Dept3 = new DemoDefinition();
        public DemoDefinition Dept4 = new DemoDefinition();
        public DemoDefinition Dept5 = new DemoDefinition();
        public DemoDefinition Dept6 = new DemoDefinition();
        public DemoDefinition Dept7 = new DemoDefinition();
        public DemoDefinition Dept8 = new DemoDefinition();
        public DemoDefinition User1 = new DemoDefinition();
        public DemoDefinition User2 = new DemoDefinition();
        public DemoDefinition User3 = new DemoDefinition();
        public DemoDefinition User4 = new DemoDefinition();
        public DemoDefinition User5 = new DemoDefinition();
        public DemoDefinition User6 = new DemoDefinition();
        public DemoDefinition User7 = new DemoDefinition();
        public DemoDefinition User8 = new DemoDefinition();
        public DemoDefinition User9 = new DemoDefinition();
        public DemoDefinition User10 = new DemoDefinition();
        public DemoDefinition User11 = new DemoDefinition();
        public DemoDefinition User12 = new DemoDefinition();
        public DemoDefinition User13 = new DemoDefinition();
        public DemoDefinition User14 = new DemoDefinition();
        public DemoDefinition User15 = new DemoDefinition();
        public DemoDefinition User16 = new DemoDefinition();
        public DemoDefinition User17 = new DemoDefinition();
        public DemoDefinition User18 = new DemoDefinition();
        public DemoDefinition User19 = new DemoDefinition();
        public DemoDefinition User20 = new DemoDefinition();
        public DemoDefinition Site1 = new DemoDefinition();
        public DemoDefinition Site2 = new DemoDefinition();
        public DemoDefinition Site3 = new DemoDefinition();
        public DemoDefinition Site4 = new DemoDefinition();
        public DemoDefinition DefineNetworks = new DemoDefinition();
        public DemoDefinition DefineServers = new DemoDefinition();
        public DemoDefinition DefineSecurity = new DemoDefinition();
        public DemoDefinition DefineOperationSystems = new DemoDefinition();
        public DemoDefinition DesignNetworks = new DemoDefinition();
        public DemoDefinition DesignServers = new DemoDefinition();
        public DemoDefinition DesignOperationSystems = new DemoDefinition();
        public DemoDefinition DesignApplication = new DemoDefinition();
        public DemoDefinition DeveropDataMigrationTools = new DemoDefinition();
        public DemoDefinition ConfigNetwork = new DemoDefinition();
        public DemoDefinition ConfigServers = new DemoDefinition();
        public DemoDefinition ConfigOperationSystems = new DemoDefinition();
        public DemoDefinition ConfigApplications = new DemoDefinition();
        public DemoDefinition TestNetworks = new DemoDefinition();
        public DemoDefinition TestServers = new DemoDefinition();
        public DemoDefinition TestOperationSystems = new DemoDefinition();
        public DemoDefinition TestApplications = new DemoDefinition();
        public DemoDefinition WriteOperationDocuments = new DemoDefinition();
        public DemoDefinition WriteUserDocuments = new DemoDefinition();
        public DemoDefinition DesignSupportDesk = new DemoDefinition();
        public DemoDefinition TestSystems = new DemoDefinition();
        public DemoDefinition Transition = new DemoDefinition();
        public DemoDefinition Report = new DemoDefinition();
        public DemoDefinition Issue1 = new DemoDefinition();
        public DemoDefinition Result1 = new DemoDefinition();
        public DemoDefinition Comment1 = new DemoDefinition();
        public DemoDefinition Comment2 = new DemoDefinition();
        public DemoDefinition Comment3 = new DemoDefinition();
        public DemoDefinition Comment4 = new DemoDefinition();
        public DemoDefinition Comment5 = new DemoDefinition();
        public DemoDefinition Comment6 = new DemoDefinition();
        public DemoDefinition Comment7 = new DemoDefinition();
        public DemoDefinition Comment8 = new DemoDefinition();
    }

    public class SqlDefinition
    {
        public string Id; public string SavedId;
        public string Body; public string SavedBody;

        public SqlDefinition()
        {
        }

        public SqlDefinition(Dictionary<string, string> propertyCollection)
        {
            if (propertyCollection.ContainsKey("Id")) Id = propertyCollection["Id"].ToString(); else Id = string.Empty;
            if (propertyCollection.ContainsKey("Body")) Body = propertyCollection["Body"].ToString(); else Body = string.Empty;
        }

        public object this[string key]
        {
            get{
                switch(key)
                {
                    case "Id": return Id;
                    case "Body": return Body;
                    default: return null;
                }
            }
        }

        public void RestoreBySavedMemory()
        {
            Id = SavedId;
            Body = SavedBody;
        }
    }

    public class SqlColumn2nd
    {
        public string BeginTransaction;
        public string CommitTransaction;
        public string CanRead;
        public string SiteDepts;
        public string ProgressRateDelay;
        public string MoveTarget;
        public string CreateDatabase;
        public string SpWho;
        public string KillSpid;
        public string RecreateLoginUser;
        public string RecreateLoginAdmin;
        public string ExistsTable;
        public string CreateTable;
        public string CreatePk;
        public string CreateIx;
        public string DeleteDefault;
        public string CreateDefault;
        public string Columns;
        public string Defaults;
        public string Indexes;
        public string DropConstraint;
        public string DropIndex;
        public string MigrateTableWithIdentity;
        public string MigrateTable;
        public string BulkInsert;
        public string Identity;
    }

    public class SqlTable
    {
        public SqlDefinition BeginTransaction = new SqlDefinition();
        public SqlDefinition CommitTransaction = new SqlDefinition();
        public SqlDefinition CanRead = new SqlDefinition();
        public SqlDefinition SiteDepts = new SqlDefinition();
        public SqlDefinition ProgressRateDelay = new SqlDefinition();
        public SqlDefinition MoveTarget = new SqlDefinition();
        public SqlDefinition CreateDatabase = new SqlDefinition();
        public SqlDefinition SpWho = new SqlDefinition();
        public SqlDefinition KillSpid = new SqlDefinition();
        public SqlDefinition RecreateLoginUser = new SqlDefinition();
        public SqlDefinition RecreateLoginAdmin = new SqlDefinition();
        public SqlDefinition ExistsTable = new SqlDefinition();
        public SqlDefinition CreateTable = new SqlDefinition();
        public SqlDefinition CreatePk = new SqlDefinition();
        public SqlDefinition CreateIx = new SqlDefinition();
        public SqlDefinition DeleteDefault = new SqlDefinition();
        public SqlDefinition CreateDefault = new SqlDefinition();
        public SqlDefinition Columns = new SqlDefinition();
        public SqlDefinition Defaults = new SqlDefinition();
        public SqlDefinition Indexes = new SqlDefinition();
        public SqlDefinition DropConstraint = new SqlDefinition();
        public SqlDefinition DropIndex = new SqlDefinition();
        public SqlDefinition MigrateTableWithIdentity = new SqlDefinition();
        public SqlDefinition MigrateTable = new SqlDefinition();
        public SqlDefinition BulkInsert = new SqlDefinition();
        public SqlDefinition Identity = new SqlDefinition();
    }

    public class ViewModeDefinition
    {
        public string Id; public string SavedId;
        public string ReferenceType; public string SavedReferenceType;
        public string Name; public string SavedName;

        public ViewModeDefinition()
        {
        }

        public ViewModeDefinition(Dictionary<string, string> propertyCollection)
        {
            if (propertyCollection.ContainsKey("Id")) Id = propertyCollection["Id"].ToString(); else Id = string.Empty;
            if (propertyCollection.ContainsKey("ReferenceType")) ReferenceType = propertyCollection["ReferenceType"].ToString(); else ReferenceType = string.Empty;
            if (propertyCollection.ContainsKey("Name")) Name = propertyCollection["Name"].ToString(); else Name = string.Empty;
        }

        public object this[string key]
        {
            get{
                switch(key)
                {
                    case "Id": return Id;
                    case "ReferenceType": return ReferenceType;
                    case "Name": return Name;
                    default: return null;
                }
            }
        }

        public void RestoreBySavedMemory()
        {
            Id = SavedId;
            ReferenceType = SavedReferenceType;
            Name = SavedName;
        }
    }

    public class ViewModeColumn2nd
    {
        public string Issues_Index;
        public string Issues_Gantt;
        public string Issues_BurnDown;
        public string Issues_TimeSeries;
        public string Issues_Kamban;
        public string Results_Index;
        public string Results_TimeSeries;
        public string Results_Kamban;
    }

    public class ViewModeTable
    {
        public ViewModeDefinition Issues_Index = new ViewModeDefinition();
        public ViewModeDefinition Issues_Gantt = new ViewModeDefinition();
        public ViewModeDefinition Issues_BurnDown = new ViewModeDefinition();
        public ViewModeDefinition Issues_TimeSeries = new ViewModeDefinition();
        public ViewModeDefinition Issues_Kamban = new ViewModeDefinition();
        public ViewModeDefinition Results_Index = new ViewModeDefinition();
        public ViewModeDefinition Results_TimeSeries = new ViewModeDefinition();
        public ViewModeDefinition Results_Kamban = new ViewModeDefinition();
    }
}
