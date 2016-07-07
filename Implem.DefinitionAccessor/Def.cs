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
            string connectionString = "",
            RdsUser rdsUser = null,
            bool transactional = false,
            bool writeSqlToDebugLog = true,
            params SqlStatement[] statements)
        {
            return new SqlIo(CommandContainer(
                connectionString != ""
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
            if (statements == null)
            {
                return new List<SqlStatement>();
            }
            else
            {
                return statements.ToList();
            }
        }

        public static Func<ColumnDefinition, bool> IsNullCaseToTrue(
            this Func<ColumnDefinition, bool> peredicate)
        {
            if (peredicate != null)
            {
                return peredicate;
            }
            else {
                return (o) => true;
            }
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
                    case "BundleConfig": Code.BundleConfig = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.BundleConfig, definitionRow, CodeXls); break;
                    case "BundleConfig_Validators": Code.BundleConfig_Validators = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.BundleConfig_Validators, definitionRow, CodeXls); break;
                    case "Controller": Code.Controller = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Controller, definitionRow, CodeXls); break;
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
                    case "Model_SetSiteSettingsProperties": Code.Model_SetSiteSettingsProperties = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SetSiteSettingsProperties, definitionRow, CodeXls); break;
                    case "Model_SetTenantId": Code.Model_SetTenantId = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SetTenantId, definitionRow, CodeXls); break;
                    case "Model_SetSiteId": Code.Model_SetSiteId = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SetSiteId, definitionRow, CodeXls); break;
                    case "Model_Create": Code.Model_Create = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Create, definitionRow, CodeXls); break;
                    case "Model_ValidateBeforeCreate": Code.Model_ValidateBeforeCreate = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_ValidateBeforeCreate, definitionRow, CodeXls); break;
                    case "Model_ValidateBeforeCreateCases": Code.Model_ValidateBeforeCreateCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_ValidateBeforeCreateCases, definitionRow, CodeXls); break;
                    case "Model_InsertItems": Code.Model_InsertItems = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_InsertItems, definitionRow, CodeXls); break;
                    case "Model_InsertItemsAfter": Code.Model_InsertItemsAfter = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_InsertItemsAfter, definitionRow, CodeXls); break;
                    case "Model_SelectIdentity": Code.Model_SelectIdentity = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SelectIdentity, definitionRow, CodeXls); break;
                    case "Model_Insert": Code.Model_Insert = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Insert, definitionRow, CodeXls); break;
                    case "Model_InsertIdentity": Code.Model_InsertIdentity = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_InsertIdentity, definitionRow, CodeXls); break;
                    case "Model_InsertIdentitySet": Code.Model_InsertIdentitySet = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_InsertIdentitySet, definitionRow, CodeXls); break;
                    case "Model_Update": Code.Model_Update = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Update, definitionRow, CodeXls); break;
                    case "Model_TitleDisplay": Code.Model_TitleDisplay = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_TitleDisplay, definitionRow, CodeXls); break;
                    case "Model_ValidateBeforeUpdate": Code.Model_ValidateBeforeUpdate = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_ValidateBeforeUpdate, definitionRow, CodeXls); break;
                    case "Model_ValidateBeforeUpdateCases": Code.Model_ValidateBeforeUpdateCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_ValidateBeforeUpdateCases, definitionRow, CodeXls); break;
                    case "Model_UpdateItems": Code.Model_UpdateItems = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_UpdateItems, definitionRow, CodeXls); break;
                    case "Model_UpdateItems_Wikis": Code.Model_UpdateItems_Wikis = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_UpdateItems_Wikis, definitionRow, CodeXls); break;
                    case "Model_UpdateItems_OutgoingMails": Code.Model_UpdateItems_OutgoingMails = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_UpdateItems_OutgoingMails, definitionRow, CodeXls); break;
                    case "Model_SynchronizeSummaryExecute": Code.Model_SynchronizeSummaryExecute = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SynchronizeSummaryExecute, definitionRow, CodeXls); break;
                    case "Model_SynchronizeSummary": Code.Model_SynchronizeSummary = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SynchronizeSummary, definitionRow, CodeXls); break;
                    case "Model_SynchronizeSummaryColumnCases": Code.Model_SynchronizeSummaryColumnCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SynchronizeSummaryColumnCases, definitionRow, CodeXls); break;
                    case "Model_UpdateFormulaColumns": Code.Model_UpdateFormulaColumns = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_UpdateFormulaColumns, definitionRow, CodeXls); break;
                    case "Model_UpdateFormulaColumns_ColumnCases": Code.Model_UpdateFormulaColumns_ColumnCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_UpdateFormulaColumns_ColumnCases, definitionRow, CodeXls); break;
                    case "Model_UpdateOrCreate": Code.Model_UpdateOrCreate = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_UpdateOrCreate, definitionRow, CodeXls); break;
                    case "Model_InsertLinksByCreate": Code.Model_InsertLinksByCreate = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_InsertLinksByCreate, definitionRow, CodeXls); break;
                    case "Model_InsertLinksByUpdate": Code.Model_InsertLinksByUpdate = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_InsertLinksByUpdate, definitionRow, CodeXls); break;
                    case "Model_InsertLinksByUpdate_Site": Code.Model_InsertLinksByUpdate_Site = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_InsertLinksByUpdate_Site, definitionRow, CodeXls); break;
                    case "Model_InsertLinks": Code.Model_InsertLinks = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_InsertLinks, definitionRow, CodeXls); break;
                    case "Model_InsertLinksCases": Code.Model_InsertLinksCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_InsertLinksCases, definitionRow, CodeXls); break;
                    case "Model_ResponseLinks": Code.Model_ResponseLinks = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_ResponseLinks, definitionRow, CodeXls); break;
                    case "Model_ResponseByUpdate_FormulaResponse": Code.Model_ResponseByUpdate_FormulaResponse = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_ResponseByUpdate_FormulaResponse, definitionRow, CodeXls); break;
                    case "Model_Copy": Code.Model_Copy = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Copy, definitionRow, CodeXls); break;
                    case "Model_Move": Code.Model_Move = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Move, definitionRow, CodeXls); break;
                    case "Model_Delete": Code.Model_Delete = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Delete, definitionRow, CodeXls); break;
                    case "Model_Delete_Item": Code.Model_Delete_Item = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Delete_Item, definitionRow, CodeXls); break;
                    case "Model_Restore": Code.Model_Restore = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Restore, definitionRow, CodeXls); break;
                    case "Model_Restore_Item": Code.Model_Restore_Item = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Restore_Item, definitionRow, CodeXls); break;
                    case "Model_PhysicalDelete": Code.Model_PhysicalDelete = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_PhysicalDelete, definitionRow, CodeXls); break;
                    case "Model_Separate": Code.Model_Separate = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Separate, definitionRow, CodeXls); break;
                    case "Model_AddSqlParamIdentity": Code.Model_AddSqlParamIdentity = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_AddSqlParamIdentity, definitionRow, CodeXls); break;
                    case "Model_AddSqlParamPk": Code.Model_AddSqlParamPk = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_AddSqlParamPk, definitionRow, CodeXls); break;
                    case "Model_AddSqlParam": Code.Model_AddSqlParam = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_AddSqlParam, definitionRow, CodeXls); break;
                    case "Model_CanCreate": Code.Model_CanCreate = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_CanCreate, definitionRow, CodeXls); break;
                    case "Model_CanUpdate": Code.Model_CanUpdate = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_CanUpdate, definitionRow, CodeXls); break;
                    case "Model_CanDelete": Code.Model_CanDelete = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_CanDelete, definitionRow, CodeXls); break;
                    case "Model_CanDownLoadFile": Code.Model_CanDownLoadFile = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_CanDownLoadFile, definitionRow, CodeXls); break;
                    case "Model_CanExport": Code.Model_CanExport = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_CanExport, definitionRow, CodeXls); break;
                    case "Model_CanImport": Code.Model_CanImport = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_CanImport, definitionRow, CodeXls); break;
                    case "Model_CanEditSite": Code.Model_CanEditSite = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_CanEditSite, definitionRow, CodeXls); break;
                    case "Model_CanEditPermission": Code.Model_CanEditPermission = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_CanEditPermission, definitionRow, CodeXls); break;
                    case "Model_Self": Code.Model_Self = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Self, definitionRow, CodeXls); break;
                    case "Model_CanEditTenant": Code.Model_CanEditTenant = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_CanEditTenant, definitionRow, CodeXls); break;
                    case "Model_CanEditSys": Code.Model_CanEditSys = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_CanEditSys, definitionRow, CodeXls); break;
                    case "Model_Title": Code.Model_Title = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Title, definitionRow, CodeXls); break;
                    case "Model_AddSqlParamPkHistory": Code.Model_AddSqlParamPkHistory = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_AddSqlParamPkHistory, definitionRow, CodeXls); break;
                    case "Model_SetByForm": Code.Model_SetByForm = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SetByForm, definitionRow, CodeXls); break;
                    case "Model_SetByForm_ColumnCases": Code.Model_SetByForm_ColumnCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SetByForm_ColumnCases, definitionRow, CodeXls); break;
                    case "Model_SetByForm_SetByFormula": Code.Model_SetByForm_SetByFormula = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SetByForm_SetByFormula, definitionRow, CodeXls); break;
                    case "Model_ToUniversal": Code.Model_ToUniversal = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_ToUniversal, definitionRow, CodeXls); break;
                    case "Model_SetByForm_Files": Code.Model_SetByForm_Files = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SetByForm_Files, definitionRow, CodeXls); break;
                    case "Model_SetByForm_Site": Code.Model_SetByForm_Site = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SetByForm_Site, definitionRow, CodeXls); break;
                    case "Model_SetByFormula": Code.Model_SetByFormula = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SetByFormula, definitionRow, CodeXls); break;
                    case "Model_SetByFormula_Data": Code.Model_SetByFormula_Data = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SetByFormula_Data, definitionRow, CodeXls); break;
                    case "Model_SetByFormula_ColumnCases": Code.Model_SetByFormula_ColumnCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SetByFormula_ColumnCases, definitionRow, CodeXls); break;
                    case "Model_SetBySession": Code.Model_SetBySession = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SetBySession, definitionRow, CodeXls); break;
                    case "Model_SetPk": Code.Model_SetPk = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SetPk, definitionRow, CodeXls); break;
                    case "Model_Set": Code.Model_Set = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Set, definitionRow, CodeXls); break;
                    case "Model_RedirectAfterDelete": Code.Model_RedirectAfterDelete = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_RedirectAfterDelete, definitionRow, CodeXls); break;
                    case "Model_RedirectAfterDeleteNotItem": Code.Model_RedirectAfterDeleteNotItem = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_RedirectAfterDeleteNotItem, definitionRow, CodeXls); break;
                    case "Model_RedirectAfterDeleteItem": Code.Model_RedirectAfterDeleteItem = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_RedirectAfterDeleteItem, definitionRow, CodeXls); break;
                    case "Model_RedirectAfterDeleteSite": Code.Model_RedirectAfterDeleteSite = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_RedirectAfterDeleteSite, definitionRow, CodeXls); break;
                    case "Model_Histories": Code.Model_Histories = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Histories, definitionRow, CodeXls); break;
                    case "Model_SwitchRecord": Code.Model_SwitchRecord = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SwitchRecord, definitionRow, CodeXls); break;
                    case "Model_SiteModel": Code.Model_SiteModel = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SiteModel, definitionRow, CodeXls); break;
                    case "Model_PushState": Code.Model_PushState = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_PushState, definitionRow, CodeXls); break;
                    case "Model_PushState_Item": Code.Model_PushState_Item = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_PushState_Item, definitionRow, CodeXls); break;
                    case "Model_SiteId": Code.Model_SiteId = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SiteId, definitionRow, CodeXls); break;
                    case "Model_SwitchItems": Code.Model_SwitchItems = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SwitchItems, definitionRow, CodeXls); break;
                    case "Model_IndexCases": Code.Model_IndexCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_IndexCases, definitionRow, CodeXls); break;
                    case "Model_SiteMenu": Code.Model_SiteMenu = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SiteMenu, definitionRow, CodeXls); break;
                    case "Model_Index": Code.Model_Index = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Index, definitionRow, CodeXls); break;
                    case "Model_NewCases": Code.Model_NewCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_NewCases, definitionRow, CodeXls); break;
                    case "Model_EditorCases": Code.Model_EditorCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_EditorCases, definitionRow, CodeXls); break;
                    case "Model_ImportCases": Code.Model_ImportCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_ImportCases, definitionRow, CodeXls); break;
                    case "Model_ExportCases": Code.Model_ExportCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_ExportCases, definitionRow, CodeXls); break;
                    case "Model_DataViewCases": Code.Model_DataViewCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_DataViewCases, definitionRow, CodeXls); break;
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
                    case "Model_PreviousCases": Code.Model_PreviousCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_PreviousCases, definitionRow, CodeXls); break;
                    case "Model_NextCases": Code.Model_NextCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_NextCases, definitionRow, CodeXls); break;
                    case "Model_ReloadCases": Code.Model_ReloadCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_ReloadCases, definitionRow, CodeXls); break;
                    case "Model_GetCases": Code.Model_GetCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_GetCases, definitionRow, CodeXls); break;
                    case "Model_BurnDownRecordDetailsCases": Code.Model_BurnDownRecordDetailsCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_BurnDownRecordDetailsCases, definitionRow, CodeXls); break;
                    case "Model_UpdateByKambanCases": Code.Model_UpdateByKambanCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_UpdateByKambanCases, definitionRow, CodeXls); break;
                    case "Model_Editor": Code.Model_Editor = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Editor, definitionRow, CodeXls); break;
                    case "Model_SiteSettingsParameters": Code.Model_SiteSettingsParameters = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SiteSettingsParameters, definitionRow, CodeXls); break;
                    case "Model_PermissionTypeParameters": Code.Model_PermissionTypeParameters = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_PermissionTypeParameters, definitionRow, CodeXls); break;
                    case "Model_SetSiteSettings": Code.Model_SetSiteSettings = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SetSiteSettings, definitionRow, CodeXls); break;
                    case "Model_SetPermissionType": Code.Model_SetPermissionType = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SetPermissionType, definitionRow, CodeXls); break;
                    case "Model_EditorParam": Code.Model_EditorParam = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_EditorParam, definitionRow, CodeXls); break;
                    case "Model_SiteSettings": Code.Model_SiteSettings = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SiteSettings, definitionRow, CodeXls); break;
                    case "Model_SiteSettingsWithParameterName": Code.Model_SiteSettingsWithParameterName = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SiteSettingsWithParameterName, definitionRow, CodeXls); break;
                    case "Model_SiteSettingsWithParameterNameLower": Code.Model_SiteSettingsWithParameterNameLower = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SiteSettingsWithParameterNameLower, definitionRow, CodeXls); break;
                    case "Model_SiteSettingsParam": Code.Model_SiteSettingsParam = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SiteSettingsParam, definitionRow, CodeXls); break;
                    case "Model_PermissionType": Code.Model_PermissionType = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_PermissionType, definitionRow, CodeXls); break;
                    case "Model_PermissionTypeWithParameterName": Code.Model_PermissionTypeWithParameterName = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_PermissionTypeWithParameterName, definitionRow, CodeXls); break;
                    case "Model_PermissionTypeWithParameterNameLower": Code.Model_PermissionTypeWithParameterNameLower = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_PermissionTypeWithParameterNameLower, definitionRow, CodeXls); break;
                    case "Model_SiteSettingsUtility": Code.Model_SiteSettingsUtility = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_SiteSettingsUtility, definitionRow, CodeXls); break;
                    case "Model_PermissionTypesAdmins": Code.Model_PermissionTypesAdmins = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_PermissionTypesAdmins, definitionRow, CodeXls); break;
                    case "Model_Subset": Code.Model_Subset = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Subset, definitionRow, CodeXls); break;
                    case "Model_Subset_Properties": Code.Model_Subset_Properties = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Subset_Properties, definitionRow, CodeXls); break;
                    case "Model_Subset_SetProperties": Code.Model_Subset_SetProperties = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Subset_SetProperties, definitionRow, CodeXls); break;
                    case "Model_Subset_SearchIndexes": Code.Model_Subset_SearchIndexes = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Subset_SearchIndexes, definitionRow, CodeXls); break;
                    case "Model_Collection": Code.Model_Collection = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Collection, definitionRow, CodeXls); break;
                    case "Model_Utility": Code.Model_Utility = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utility, definitionRow, CodeXls); break;
                    case "Model_Utility_Index": Code.Model_Utility_Index = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utility_Index, definitionRow, CodeXls); break;
                    case "Model_Utility_ImportSettings": Code.Model_Utility_ImportSettings = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utility_ImportSettings, definitionRow, CodeXls); break;
                    case "Model_Utility_GridRows_BackUrl": Code.Model_Utility_GridRows_BackUrl = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utility_GridRows_BackUrl, definitionRow, CodeXls); break;
                    case "Model_Utility_GridRows_BackUrlItem": Code.Model_Utility_GridRows_BackUrlItem = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utility_GridRows_BackUrlItem, definitionRow, CodeXls); break;
                    case "Model_Utility_GridRows_OnClick": Code.Model_Utility_GridRows_OnClick = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utility_GridRows_OnClick, definitionRow, CodeXls); break;
                    case "Model_Utility_GridRows_OnClickItem": Code.Model_Utility_GridRows_OnClickItem = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utility_GridRows_OnClickItem, definitionRow, CodeXls); break;
                    case "Model_Utility_Td": Code.Model_Utility_Td = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utility_Td, definitionRow, CodeXls); break;
                    case "Model_Utility_TdCases": Code.Model_Utility_TdCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utility_TdCases, definitionRow, CodeXls); break;
                    case "Model_Utility_AddSqlColumnSiteId": Code.Model_Utility_AddSqlColumnSiteId = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utility_AddSqlColumnSiteId, definitionRow, CodeXls); break;
                    case "Model_Utility_AddSqlColumn": Code.Model_Utility_AddSqlColumn = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utility_AddSqlColumn, definitionRow, CodeXls); break;
                    case "Model_Utility_GridSqlWhereTenantId": Code.Model_Utility_GridSqlWhereTenantId = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utility_GridSqlWhereTenantId, definitionRow, CodeXls); break;
                    case "Model_Utility_GridSqlWhereSiteId": Code.Model_Utility_GridSqlWhereSiteId = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utility_GridSqlWhereSiteId, definitionRow, CodeXls); break;
                    case "Model_Utility_Editor": Code.Model_Utility_Editor = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utility_Editor, definitionRow, CodeXls); break;
                    case "Model_Utility_UserSelf": Code.Model_Utility_UserSelf = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utility_UserSelf, definitionRow, CodeXls); break;
                    case "Model_Utility_EditorItem": Code.Model_Utility_EditorItem = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utility_EditorItem, definitionRow, CodeXls); break;
                    case "Model_Utility_EditorProfilePermission": Code.Model_Utility_EditorProfilePermission = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utility_EditorProfilePermission, definitionRow, CodeXls); break;
                    case "Model_Utility_EditorBackUrl": Code.Model_Utility_EditorBackUrl = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utility_EditorBackUrl, definitionRow, CodeXls); break;
                    case "Model_Utility_EditorBackUrl_Users": Code.Model_Utility_EditorBackUrl_Users = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utility_EditorBackUrl_Users, definitionRow, CodeXls); break;
                    case "Model_Utility_FieldCases": Code.Model_Utility_FieldCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utility_FieldCases, definitionRow, CodeXls); break;
                    case "Model_Utility_FieldCases_Item": Code.Model_Utility_FieldCases_Item = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utility_FieldCases_Item, definitionRow, CodeXls); break;
                    case "Model_Utility_GetSwitchTargets": Code.Model_Utility_GetSwitchTargets = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utility_GetSwitchTargets, definitionRow, CodeXls); break;
                    case "Model_Utility_SqlWhereTenantId": Code.Model_Utility_SqlWhereTenantId = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utility_SqlWhereTenantId, definitionRow, CodeXls); break;
                    case "Model_Utility_SqlWhereSiteId": Code.Model_Utility_SqlWhereSiteId = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utility_SqlWhereSiteId, definitionRow, CodeXls); break;
                    case "Model_Utility_SiteId": Code.Model_Utility_SiteId = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utility_SiteId, definitionRow, CodeXls); break;
                    case "Model_Utility_SiteIdParam": Code.Model_Utility_SiteIdParam = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utility_SiteIdParam, definitionRow, CodeXls); break;
                    case "Model_Utility_FormResponse": Code.Model_Utility_FormResponse = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utility_FormResponse, definitionRow, CodeXls); break;
                    case "Model_Utility_FormResponse_ColumnCases": Code.Model_Utility_FormResponse_ColumnCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utility_FormResponse_ColumnCases, definitionRow, CodeXls); break;
                    case "Model_Utility_FormulaResponse": Code.Model_Utility_FormulaResponse = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utility_FormulaResponse, definitionRow, CodeXls); break;
                    case "Model_Utility_FormulaResponse_ColumnCases": Code.Model_Utility_FormulaResponse_ColumnCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utility_FormulaResponse_ColumnCases, definitionRow, CodeXls); break;
                    case "Model_Utility_TableName": Code.Model_Utility_TableName = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utility_TableName, definitionRow, CodeXls); break;
                    case "Model_Utility_TableNameCases": Code.Model_Utility_TableNameCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utility_TableNameCases, definitionRow, CodeXls); break;
                    case "Model_Utility_BulkMove": Code.Model_Utility_BulkMove = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utility_BulkMove, definitionRow, CodeXls); break;
                    case "Model_Utility_BulkDelete": Code.Model_Utility_BulkDelete = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utility_BulkDelete, definitionRow, CodeXls); break;
                    case "Model_Utility_Import": Code.Model_Utility_Import = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utility_Import, definitionRow, CodeXls); break;
                    case "Model_Utility_ImportCases": Code.Model_Utility_ImportCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utility_ImportCases, definitionRow, CodeXls); break;
                    case "Model_Utility_ImportValidatorHeaders": Code.Model_Utility_ImportValidatorHeaders = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utility_ImportValidatorHeaders, definitionRow, CodeXls); break;
                    case "Model_Utility_ImportValidatorCases": Code.Model_Utility_ImportValidatorCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utility_ImportValidatorCases, definitionRow, CodeXls); break;
                    case "Model_Utility_Export": Code.Model_Utility_Export = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utility_Export, definitionRow, CodeXls); break;
                    case "Model_Utility_CsvColumnCases": Code.Model_Utility_CsvColumnCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utility_CsvColumnCases, definitionRow, CodeXls); break;
                    case "Model_Utility_NotNull": Code.Model_Utility_NotNull = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utility_NotNull, definitionRow, CodeXls); break;
                    case "Model_Utility_TitleDisplayValue": Code.Model_Utility_TitleDisplayValue = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utility_TitleDisplayValue, definitionRow, CodeXls); break;
                    case "Model_Utility_TitleDisplayValueCases_Item": Code.Model_Utility_TitleDisplayValueCases_Item = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utility_TitleDisplayValueCases_Item, definitionRow, CodeXls); break;
                    case "Model_Utility_TitleDisplayValueCases_DataRow": Code.Model_Utility_TitleDisplayValueCases_DataRow = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utility_TitleDisplayValueCases_DataRow, definitionRow, CodeXls); break;
                    case "Model_Utility_UpdateByKamban": Code.Model_Utility_UpdateByKamban = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Model_Utility_UpdateByKamban, definitionRow, CodeXls); break;
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
                    case "Rds_Columns_SqlTotalCases": Code.Rds_Columns_SqlTotalCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns_SqlTotalCases, definitionRow, CodeXls); break;
                    case "Rds_Columns_SqlAverageCases": Code.Rds_Columns_SqlAverageCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns_SqlAverageCases, definitionRow, CodeXls); break;
                    case "Rds_Columns_SqlMaxCases": Code.Rds_Columns_SqlMaxCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns_SqlMaxCases, definitionRow, CodeXls); break;
                    case "Rds_Columns_SqlMinCases": Code.Rds_Columns_SqlMinCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns_SqlMinCases, definitionRow, CodeXls); break;
                    case "Rds_Columns_SqlColumn": Code.Rds_Columns_SqlColumn = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns_SqlColumn, definitionRow, CodeXls); break;
                    case "Rds_Columns_SqlColumn_SelectColumns": Code.Rds_Columns_SqlColumn_SelectColumns = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns_SqlColumn_SelectColumns, definitionRow, CodeXls); break;
                    case "Rds_Columns_SqlColumn_ComputeColumn": Code.Rds_Columns_SqlColumn_ComputeColumn = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns_SqlColumn_ComputeColumn, definitionRow, CodeXls); break;
                    case "Rds_Columns_SqlColumnComputes1": Code.Rds_Columns_SqlColumnComputes1 = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns_SqlColumnComputes1, definitionRow, CodeXls); break;
                    case "Rds_Columns_SqlColumnComputes2": Code.Rds_Columns_SqlColumnComputes2 = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns_SqlColumnComputes2, definitionRow, CodeXls); break;
                    case "Rds_Columns_SqlWhere": Code.Rds_Columns_SqlWhere = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns_SqlWhere, definitionRow, CodeXls); break;
                    case "Rds_Columns_SqlWhere_SelectColumns": Code.Rds_Columns_SqlWhere_SelectColumns = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns_SqlWhere_SelectColumns, definitionRow, CodeXls); break;
                    case "Rds_Columns_SqlWhere_ComputeColumn": Code.Rds_Columns_SqlWhere_ComputeColumn = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns_SqlWhere_ComputeColumn, definitionRow, CodeXls); break;
                    case "Rds_Columns_SqlWhere_In": Code.Rds_Columns_SqlWhere_In = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns_SqlWhere_In, definitionRow, CodeXls); break;
                    case "Rds_Columns_SqlWhere_Between_Number": Code.Rds_Columns_SqlWhere_Between_Number = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns_SqlWhere_Between_Number, definitionRow, CodeXls); break;
                    case "Rds_Columns_SqlWhere_Between_DateTime": Code.Rds_Columns_SqlWhere_Between_DateTime = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns_SqlWhere_Between_DateTime, definitionRow, CodeXls); break;
                    case "Rds_Columns_SqlGroupByCases": Code.Rds_Columns_SqlGroupByCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns_SqlGroupByCases, definitionRow, CodeXls); break;
                    case "Rds_Columns_SqlGroupBy": Code.Rds_Columns_SqlGroupBy = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns_SqlGroupBy, definitionRow, CodeXls); break;
                    case "Rds_Columns_SqlHavingComputes1": Code.Rds_Columns_SqlHavingComputes1 = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns_SqlHavingComputes1, definitionRow, CodeXls); break;
                    case "Rds_Columns_SqlHavingComputes2": Code.Rds_Columns_SqlHavingComputes2 = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns_SqlHavingComputes2, definitionRow, CodeXls); break;
                    case "Rds_Columns_SqlOrderBy1": Code.Rds_Columns_SqlOrderBy1 = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns_SqlOrderBy1, definitionRow, CodeXls); break;
                    case "Rds_Columns_SqlOrderBy2": Code.Rds_Columns_SqlOrderBy2 = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns_SqlOrderBy2, definitionRow, CodeXls); break;
                    case "Rds_Columns_SqlOrderByComputes1": Code.Rds_Columns_SqlOrderByComputes1 = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns_SqlOrderByComputes1, definitionRow, CodeXls); break;
                    case "Rds_Columns_SqlOrderByComputes2": Code.Rds_Columns_SqlOrderByComputes2 = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns_SqlOrderByComputes2, definitionRow, CodeXls); break;
                    case "Rds_Columns_SqlParam_ItemId": Code.Rds_Columns_SqlParam_ItemId = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns_SqlParam_ItemId, definitionRow, CodeXls); break;
                    case "Rds_Columns_SqlParam": Code.Rds_Columns_SqlParam = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns_SqlParam, definitionRow, CodeXls); break;
                    case "Rds_Columns_SqlParam_Enum": Code.Rds_Columns_SqlParam_Enum = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Columns_SqlParam_Enum, definitionRow, CodeXls); break;
                    case "Rds_Defaults": Code.Rds_Defaults = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_Defaults, definitionRow, CodeXls); break;
                    case "Rds_ColumnDefault": Code.Rds_ColumnDefault = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_ColumnDefault, definitionRow, CodeXls); break;
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
                    case "Rds_SiteSettings_SiteId": Code.Rds_SiteSettings_SiteId = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_SiteSettings_SiteId, definitionRow, CodeXls); break;
                    case "Rds_TitleColumn": Code.Rds_TitleColumn = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_TitleColumn, definitionRow, CodeXls); break;
                    case "Rds_TitleColumnCases": Code.Rds_TitleColumnCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Rds_TitleColumnCases, definitionRow, CodeXls); break;
                    case "Titles": Code.Titles = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Titles, definitionRow, CodeXls); break;
                    case "Titles_TitleDisplayValueCases": Code.Titles_TitleDisplayValueCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Titles_TitleDisplayValueCases, definitionRow, CodeXls); break;
                    case "Indexes": Code.Indexes = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Indexes, definitionRow, CodeXls); break;
                    case "Indexes_TableCases": Code.Indexes_TableCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Indexes_TableCases, definitionRow, CodeXls); break;
                    case "Responses": Code.Responses = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Responses, definitionRow, CodeXls); break;
                    case "Responses_ItemSubsets": Code.Responses_ItemSubsets = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Responses_ItemSubsets, definitionRow, CodeXls); break;
                    case "ItemsInitializer": Code.ItemsInitializer = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.ItemsInitializer, definitionRow, CodeXls); break;
                    case "ItemsInitializer_InitItems": Code.ItemsInitializer_InitItems = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.ItemsInitializer_InitItems, definitionRow, CodeXls); break;
                    case "SiteSettings": Code.SiteSettings = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.SiteSettings, definitionRow, CodeXls); break;
                    case "SiteSettings_Get": Code.SiteSettings_Get = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.SiteSettings_Get, definitionRow, CodeXls); break;
                    case "SiteSettings_GetCases": Code.SiteSettings_GetCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.SiteSettings_GetCases, definitionRow, CodeXls); break;
                    case "SiteSettings_GetModels": Code.SiteSettings_GetModels = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.SiteSettings_GetModels, definitionRow, CodeXls); break;
                    case "SiteSettings_GetModels_Items": Code.SiteSettings_GetModels_Items = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.SiteSettings_GetModels_Items, definitionRow, CodeXls); break;
                    case "SiteSettings_GetModels_Includes": Code.SiteSettings_GetModels_Includes = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.SiteSettings_GetModels_Includes, definitionRow, CodeXls); break;
                    case "DataViewFilters": Code.DataViewFilters = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.DataViewFilters, definitionRow, CodeXls); break;
                    case "DataViewFilters_Search_TableCases": Code.DataViewFilters_Search_TableCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.DataViewFilters_Search_TableCases, definitionRow, CodeXls); break;
                    case "GridSorters": Code.GridSorters = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.GridSorters, definitionRow, CodeXls); break;
                    case "GridSorters_Model": Code.GridSorters_Model = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.GridSorters_Model, definitionRow, CodeXls); break;
                    case "GridSorters_Cases": Code.GridSorters_Cases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.GridSorters_Cases, definitionRow, CodeXls); break;
                    case "HtmlLinks": Code.HtmlLinks = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.HtmlLinks, definitionRow, CodeXls); break;
                    case "HtmlLinks_TableCases": Code.HtmlLinks_TableCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.HtmlLinks_TableCases, definitionRow, CodeXls); break;
                    case "HtmlLinks_HeaderCases": Code.HtmlLinks_HeaderCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.HtmlLinks_HeaderCases, definitionRow, CodeXls); break;
                    case "HtmlLinks_Headers": Code.HtmlLinks_Headers = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.HtmlLinks_Headers, definitionRow, CodeXls); break;
                    case "HtmlLinks_Rows": Code.HtmlLinks_Rows = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.HtmlLinks_Rows, definitionRow, CodeXls); break;
                    case "HtmlLinks_RowColumns": Code.HtmlLinks_RowColumns = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.HtmlLinks_RowColumns, definitionRow, CodeXls); break;
                    case "Summaries": Code.Summaries = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Summaries, definitionRow, CodeXls); break;
                    case "Summaries_SynchronizeCases": Code.Summaries_SynchronizeCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Summaries_SynchronizeCases, definitionRow, CodeXls); break;
                    case "Summaries_SynchronizeTables": Code.Summaries_SynchronizeTables = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Summaries_SynchronizeTables, definitionRow, CodeXls); break;
                    case "Summaries_ParamCases": Code.Summaries_ParamCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Summaries_ParamCases, definitionRow, CodeXls); break;
                    case "Summaries_SelectCountTables": Code.Summaries_SelectCountTables = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Summaries_SelectCountTables, definitionRow, CodeXls); break;
                    case "Summaries_SelectTotalTableCases": Code.Summaries_SelectTotalTableCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Summaries_SelectTotalTableCases, definitionRow, CodeXls); break;
                    case "Summaries_SelectTotalColumns": Code.Summaries_SelectTotalColumns = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Summaries_SelectTotalColumns, definitionRow, CodeXls); break;
                    case "Summaries_SelectTotalColumnCases": Code.Summaries_SelectTotalColumnCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Summaries_SelectTotalColumnCases, definitionRow, CodeXls); break;
                    case "Summaries_SelectAverageTableCases": Code.Summaries_SelectAverageTableCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Summaries_SelectAverageTableCases, definitionRow, CodeXls); break;
                    case "Summaries_SelectAverageColumns": Code.Summaries_SelectAverageColumns = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Summaries_SelectAverageColumns, definitionRow, CodeXls); break;
                    case "Summaries_SelectAverageColumnCases": Code.Summaries_SelectAverageColumnCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Summaries_SelectAverageColumnCases, definitionRow, CodeXls); break;
                    case "Summaries_SelectMaxTableCases": Code.Summaries_SelectMaxTableCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Summaries_SelectMaxTableCases, definitionRow, CodeXls); break;
                    case "Summaries_SelectMaxColumns": Code.Summaries_SelectMaxColumns = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Summaries_SelectMaxColumns, definitionRow, CodeXls); break;
                    case "Summaries_SelectMaxColumnCases": Code.Summaries_SelectMaxColumnCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Summaries_SelectMaxColumnCases, definitionRow, CodeXls); break;
                    case "Summaries_SelectMinTableCases": Code.Summaries_SelectMinTableCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Summaries_SelectMinTableCases, definitionRow, CodeXls); break;
                    case "Summaries_SelectMinColumns": Code.Summaries_SelectMinColumns = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Summaries_SelectMinColumns, definitionRow, CodeXls); break;
                    case "Summaries_SelectMinColumnCases": Code.Summaries_SelectMinColumnCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Summaries_SelectMinColumnCases, definitionRow, CodeXls); break;
                    case "Summaries_WhereTables": Code.Summaries_WhereTables = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Summaries_WhereTables, definitionRow, CodeXls); break;
                    case "Summaries_WhereColumnCases": Code.Summaries_WhereColumnCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Summaries_WhereColumnCases, definitionRow, CodeXls); break;
                    case "Formulas": Code.Formulas = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Formulas, definitionRow, CodeXls); break;
                    case "Formulas_TableCases": Code.Formulas_TableCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Formulas_TableCases, definitionRow, CodeXls); break;
                    case "Formulas_Updates": Code.Formulas_Updates = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Formulas_Updates, definitionRow, CodeXls); break;
                    case "Messages": Code.Messages = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Messages, definitionRow, CodeXls); break;
                    case "Messages_Parts": Code.Messages_Parts = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Messages_Parts, definitionRow, CodeXls); break;
                    case "Messages_Resonses": Code.Messages_Resonses = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Messages_Resonses, definitionRow, CodeXls); break;
                    case "ResponseCollection": Code.ResponseCollection = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.ResponseCollection, definitionRow, CodeXls); break;
                    case "ResponseCollection_Models": Code.ResponseCollection_Models = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.ResponseCollection_Models, definitionRow, CodeXls); break;
                    case "ResponseCollection_ValueModels": Code.ResponseCollection_ValueModels = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.ResponseCollection_ValueModels, definitionRow, CodeXls); break;
                    case "ResponseCollection_ValueColumns": Code.ResponseCollection_ValueColumns = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.ResponseCollection_ValueColumns, definitionRow, CodeXls); break;
                    case "Displays": Code.Displays = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Displays, definitionRow, CodeXls); break;
                    case "Displays_Parts": Code.Displays_Parts = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.Displays_Parts, definitionRow, CodeXls); break;
                    case "HtmlScripts": Code.HtmlScripts = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.HtmlScripts, definitionRow, CodeXls); break;
                    case "HtmlScripts_ValidatorCases": Code.HtmlScripts_ValidatorCases = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.HtmlScripts_ValidatorCases, definitionRow, CodeXls); break;
                    case "ClientValidators": Code.ClientValidators = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.ClientValidators, definitionRow, CodeXls); break;
                    case "ClientValidators_Form": Code.ClientValidators_Form = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.ClientValidators_Form, definitionRow, CodeXls); break;
                    case "ClientValidators_Rules": Code.ClientValidators_Rules = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.ClientValidators_Rules, definitionRow, CodeXls); break;
                    case "ClientValidators_Messages": Code.ClientValidators_Messages = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetCodeTable(CodeTable.ClientValidators_Messages, definitionRow, CodeXls); break;
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
                if (definitionRow.ContainsKey("NotTypeCs")) { newCodeDefinition.NotTypeCs = definitionRow["NotTypeCs"].ToBool(); newCodeDefinition.SavedNotTypeCs = newCodeDefinition.NotTypeCs; }
                if (definitionRow.ContainsKey("ItemOnly")) { newCodeDefinition.ItemOnly = definitionRow["ItemOnly"].ToBool(); newCodeDefinition.SavedItemOnly = newCodeDefinition.ItemOnly; }
                if (definitionRow.ContainsKey("NotItem")) { newCodeDefinition.NotItem = definitionRow["NotItem"].ToBool(); newCodeDefinition.SavedNotItem = newCodeDefinition.NotItem; }
                if (definitionRow.ContainsKey("Session")) { newCodeDefinition.Session = definitionRow["Session"].ToBool(); newCodeDefinition.SavedSession = newCodeDefinition.Session; }
                if (definitionRow.ContainsKey("GridColumn")) { newCodeDefinition.GridColumn = definitionRow["GridColumn"].ToBool(); newCodeDefinition.SavedGridColumn = newCodeDefinition.GridColumn; }
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
            if (definitionRow.ContainsKey("NotTypeCs")) { definition.NotTypeCs = definitionRow["NotTypeCs"].ToBool(); definition.SavedNotTypeCs = definition.NotTypeCs; }
            if (definitionRow.ContainsKey("ItemOnly")) { definition.ItemOnly = definitionRow["ItemOnly"].ToBool(); definition.SavedItemOnly = definition.ItemOnly; }
            if (definitionRow.ContainsKey("NotItem")) { definition.NotItem = definitionRow["NotItem"].ToBool(); definition.SavedNotItem = definition.NotItem; }
            if (definitionRow.ContainsKey("Session")) { definition.Session = definitionRow["Session"].ToBool(); definition.SavedSession = definition.Session; }
            if (definitionRow.ContainsKey("GridColumn")) { definition.GridColumn = definitionRow["GridColumn"].ToBool(); definition.SavedGridColumn = definition.GridColumn; }
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
                    case "Depts_ParentDeptId": Column.Depts_ParentDeptId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Depts_ParentDeptId, definitionRow, ColumnXls); break;
                    case "Depts_ParentDept": Column.Depts_ParentDept = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Depts_ParentDept, definitionRow, ColumnXls); break;
                    case "Depts_DeptCode": Column.Depts_DeptCode = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Depts_DeptCode, definitionRow, ColumnXls); break;
                    case "Depts_Dept": Column.Depts_Dept = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Depts_Dept, definitionRow, ColumnXls); break;
                    case "Depts_DeptName": Column.Depts_DeptName = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Depts_DeptName, definitionRow, ColumnXls); break;
                    case "Depts_Body": Column.Depts_Body = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Depts_Body, definitionRow, ColumnXls); break;
                    case "Depts_Title": Column.Depts_Title = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Depts_Title, definitionRow, ColumnXls); break;
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
                    case "Users_Sex": Column.Users_Sex = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_Sex, definitionRow, ColumnXls); break;
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
                    case "Users_TenantAdmin": Column.Users_TenantAdmin = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_TenantAdmin, definitionRow, ColumnXls); break;
                    case "Users_ServiceAdmin": Column.Users_ServiceAdmin = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Users_ServiceAdmin, definitionRow, ColumnXls); break;
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
                    case "Permissions_UserId": Column.Permissions_UserId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Permissions_UserId, definitionRow, ColumnXls); break;
                    case "Permissions_DeptName": Column.Permissions_DeptName = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Permissions_DeptName, definitionRow, ColumnXls); break;
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
                    case "Items_Subset": Column.Items_Subset = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Items_Subset, definitionRow, ColumnXls); break;
                    case "Items_MaintenanceTarget": Column.Items_MaintenanceTarget = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Items_MaintenanceTarget, definitionRow, ColumnXls); break;
                    case "Items_Site": Column.Items_Site = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Items_Site, definitionRow, ColumnXls); break;
                    case "Sites_TenantId": Column.Sites_TenantId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Sites_TenantId, definitionRow, ColumnXls); break;
                    case "Sites_SiteId": Column.Sites_SiteId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Sites_SiteId, definitionRow, ColumnXls); break;
                    case "Sites_ReferenceType": Column.Sites_ReferenceType = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Sites_ReferenceType, definitionRow, ColumnXls); break;
                    case "Sites_ParentId": Column.Sites_ParentId = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Sites_ParentId, definitionRow, ColumnXls); break;
                    case "Sites_InheritPermission": Column.Sites_InheritPermission = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Sites_InheritPermission, definitionRow, ColumnXls); break;
                    case "Sites_PermissionType": Column.Sites_PermissionType = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Sites_PermissionType, definitionRow, ColumnXls); break;
                    case "Sites_SiteSettings": Column.Sites_SiteSettings = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Sites_SiteSettings, definitionRow, ColumnXls); break;
                    case "Sites_Ancestors": Column.Sites_Ancestors = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Sites_Ancestors, definitionRow, ColumnXls); break;
                    case "Sites_PermissionSourceCollection": Column.Sites_PermissionSourceCollection = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Sites_PermissionSourceCollection, definitionRow, ColumnXls); break;
                    case "Sites_PermissionDestinationCollection": Column.Sites_PermissionDestinationCollection = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Sites_PermissionDestinationCollection, definitionRow, ColumnXls); break;
                    case "Sites_SiteMenu": Column.Sites_SiteMenu = definitionRow[1].ToString(); SetColumnTable(ColumnTable.Sites_SiteMenu, definitionRow, ColumnXls); break;
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
                if (definitionRow.ContainsKey("GridVisible")) { newColumnDefinition.GridVisible = definitionRow["GridVisible"].ToBool(); newColumnDefinition.SavedGridVisible = newColumnDefinition.GridVisible; }
                if (definitionRow.ContainsKey("FilterColumn")) { newColumnDefinition.FilterColumn = definitionRow["FilterColumn"].ToInt(); newColumnDefinition.SavedFilterColumn = newColumnDefinition.FilterColumn; }
                if (definitionRow.ContainsKey("FilterVisible")) { newColumnDefinition.FilterVisible = definitionRow["FilterVisible"].ToBool(); newColumnDefinition.SavedFilterVisible = newColumnDefinition.FilterVisible; }
                if (definitionRow.ContainsKey("EditorColumn")) { newColumnDefinition.EditorColumn = definitionRow["EditorColumn"].ToBool(); newColumnDefinition.SavedEditorColumn = newColumnDefinition.EditorColumn; }
                if (definitionRow.ContainsKey("EditorVisible")) { newColumnDefinition.EditorVisible = definitionRow["EditorVisible"].ToBool(); newColumnDefinition.SavedEditorVisible = newColumnDefinition.EditorVisible; }
                if (definitionRow.ContainsKey("TitleColumn")) { newColumnDefinition.TitleColumn = definitionRow["TitleColumn"].ToInt(); newColumnDefinition.SavedTitleColumn = newColumnDefinition.TitleColumn; }
                if (definitionRow.ContainsKey("LinkColumn")) { newColumnDefinition.LinkColumn = definitionRow["LinkColumn"].ToInt(); newColumnDefinition.SavedLinkColumn = newColumnDefinition.LinkColumn; }
                if (definitionRow.ContainsKey("LinkVisible")) { newColumnDefinition.LinkVisible = definitionRow["LinkVisible"].ToBool(); newColumnDefinition.SavedLinkVisible = newColumnDefinition.LinkVisible; }
                if (definitionRow.ContainsKey("HistoryColumn")) { newColumnDefinition.HistoryColumn = definitionRow["HistoryColumn"].ToInt(); newColumnDefinition.SavedHistoryColumn = newColumnDefinition.HistoryColumn; }
                if (definitionRow.ContainsKey("HistoryVisible")) { newColumnDefinition.HistoryVisible = definitionRow["HistoryVisible"].ToBool(); newColumnDefinition.SavedHistoryVisible = newColumnDefinition.HistoryVisible; }
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
                if (definitionRow.ContainsKey("ColumnNameOld")) { newColumnDefinition.ColumnNameOld = definitionRow["ColumnNameOld"].ToString(); newColumnDefinition.SavedColumnNameOld = newColumnDefinition.ColumnNameOld; }
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
                if (definitionRow.ContainsKey("ControlDateTime")) { newColumnDefinition.ControlDateTime = definitionRow["ControlDateTime"].ToString(); newColumnDefinition.SavedControlDateTime = newColumnDefinition.ControlDateTime; }
                if (definitionRow.ContainsKey("GridDateTime")) { newColumnDefinition.GridDateTime = definitionRow["GridDateTime"].ToString(); newColumnDefinition.SavedGridDateTime = newColumnDefinition.GridDateTime; }
                if (definitionRow.ContainsKey("Aggregatable")) { newColumnDefinition.Aggregatable = definitionRow["Aggregatable"].ToBool(); newColumnDefinition.SavedAggregatable = newColumnDefinition.Aggregatable; }
                if (definitionRow.ContainsKey("Computable")) { newColumnDefinition.Computable = definitionRow["Computable"].ToBool(); newColumnDefinition.SavedComputable = newColumnDefinition.Computable; }
                if (definitionRow.ContainsKey("ChoicesText")) { newColumnDefinition.ChoicesText = definitionRow["ChoicesText"].ToString(); newColumnDefinition.SavedChoicesText = newColumnDefinition.ChoicesText; }
                if (definitionRow.ContainsKey("DefaultInput")) { newColumnDefinition.DefaultInput = definitionRow["DefaultInput"].ToString(); newColumnDefinition.SavedDefaultInput = newColumnDefinition.DefaultInput; }
                if (definitionRow.ContainsKey("Own")) { newColumnDefinition.Own = definitionRow["Own"].ToBool(); newColumnDefinition.SavedOwn = newColumnDefinition.Own; }
                if (definitionRow.ContainsKey("FormName")) { newColumnDefinition.FormName = definitionRow["FormName"].ToString(); newColumnDefinition.SavedFormName = newColumnDefinition.FormName; }
                if (definitionRow.ContainsKey("Validators")) { newColumnDefinition.Validators = definitionRow["Validators"].ToString(); newColumnDefinition.SavedValidators = newColumnDefinition.Validators; }
                if (definitionRow.ContainsKey("DecimalPlaces")) { newColumnDefinition.DecimalPlaces = definitionRow["DecimalPlaces"].ToInt(); newColumnDefinition.SavedDecimalPlaces = newColumnDefinition.DecimalPlaces; }
                if (definitionRow.ContainsKey("Min")) { newColumnDefinition.Min = definitionRow["Min"].ToDecimal(); newColumnDefinition.SavedMin = newColumnDefinition.Min; }
                if (definitionRow.ContainsKey("Max")) { newColumnDefinition.Max = definitionRow["Max"].ToDecimal(); newColumnDefinition.SavedMax = newColumnDefinition.Max; }
                if (definitionRow.ContainsKey("Step")) { newColumnDefinition.Step = definitionRow["Step"].ToDecimal(); newColumnDefinition.SavedStep = newColumnDefinition.Step; }
                if (definitionRow.ContainsKey("StringFormat")) { newColumnDefinition.StringFormat = definitionRow["StringFormat"].ToString(); newColumnDefinition.SavedStringFormat = newColumnDefinition.StringFormat; }
                if (definitionRow.ContainsKey("Unit")) { newColumnDefinition.Unit = definitionRow["Unit"].ToString(); newColumnDefinition.SavedUnit = newColumnDefinition.Unit; }
                if (definitionRow.ContainsKey("Width")) { newColumnDefinition.Width = definitionRow["Width"].ToInt(); newColumnDefinition.SavedWidth = newColumnDefinition.Width; }
                if (definitionRow.ContainsKey("SettingEnable")) { newColumnDefinition.SettingEnable = definitionRow["SettingEnable"].ToBool(); newColumnDefinition.SavedSettingEnable = newColumnDefinition.SettingEnable; }
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
            if (definitionRow.ContainsKey("GridVisible")) { definition.GridVisible = definitionRow["GridVisible"].ToBool(); definition.SavedGridVisible = definition.GridVisible; }
            if (definitionRow.ContainsKey("FilterColumn")) { definition.FilterColumn = definitionRow["FilterColumn"].ToInt(); definition.SavedFilterColumn = definition.FilterColumn; }
            if (definitionRow.ContainsKey("FilterVisible")) { definition.FilterVisible = definitionRow["FilterVisible"].ToBool(); definition.SavedFilterVisible = definition.FilterVisible; }
            if (definitionRow.ContainsKey("EditorColumn")) { definition.EditorColumn = definitionRow["EditorColumn"].ToBool(); definition.SavedEditorColumn = definition.EditorColumn; }
            if (definitionRow.ContainsKey("EditorVisible")) { definition.EditorVisible = definitionRow["EditorVisible"].ToBool(); definition.SavedEditorVisible = definition.EditorVisible; }
            if (definitionRow.ContainsKey("TitleColumn")) { definition.TitleColumn = definitionRow["TitleColumn"].ToInt(); definition.SavedTitleColumn = definition.TitleColumn; }
            if (definitionRow.ContainsKey("LinkColumn")) { definition.LinkColumn = definitionRow["LinkColumn"].ToInt(); definition.SavedLinkColumn = definition.LinkColumn; }
            if (definitionRow.ContainsKey("LinkVisible")) { definition.LinkVisible = definitionRow["LinkVisible"].ToBool(); definition.SavedLinkVisible = definition.LinkVisible; }
            if (definitionRow.ContainsKey("HistoryColumn")) { definition.HistoryColumn = definitionRow["HistoryColumn"].ToInt(); definition.SavedHistoryColumn = definition.HistoryColumn; }
            if (definitionRow.ContainsKey("HistoryVisible")) { definition.HistoryVisible = definitionRow["HistoryVisible"].ToBool(); definition.SavedHistoryVisible = definition.HistoryVisible; }
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
            if (definitionRow.ContainsKey("ColumnNameOld")) { definition.ColumnNameOld = definitionRow["ColumnNameOld"].ToString(); definition.SavedColumnNameOld = definition.ColumnNameOld; }
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
            if (definitionRow.ContainsKey("ControlDateTime")) { definition.ControlDateTime = definitionRow["ControlDateTime"].ToString(); definition.SavedControlDateTime = definition.ControlDateTime; }
            if (definitionRow.ContainsKey("GridDateTime")) { definition.GridDateTime = definitionRow["GridDateTime"].ToString(); definition.SavedGridDateTime = definition.GridDateTime; }
            if (definitionRow.ContainsKey("Aggregatable")) { definition.Aggregatable = definitionRow["Aggregatable"].ToBool(); definition.SavedAggregatable = definition.Aggregatable; }
            if (definitionRow.ContainsKey("Computable")) { definition.Computable = definitionRow["Computable"].ToBool(); definition.SavedComputable = definition.Computable; }
            if (definitionRow.ContainsKey("ChoicesText")) { definition.ChoicesText = definitionRow["ChoicesText"].ToString(); definition.SavedChoicesText = definition.ChoicesText; }
            if (definitionRow.ContainsKey("DefaultInput")) { definition.DefaultInput = definitionRow["DefaultInput"].ToString(); definition.SavedDefaultInput = definition.DefaultInput; }
            if (definitionRow.ContainsKey("Own")) { definition.Own = definitionRow["Own"].ToBool(); definition.SavedOwn = definition.Own; }
            if (definitionRow.ContainsKey("FormName")) { definition.FormName = definitionRow["FormName"].ToString(); definition.SavedFormName = definition.FormName; }
            if (definitionRow.ContainsKey("Validators")) { definition.Validators = definitionRow["Validators"].ToString(); definition.SavedValidators = definition.Validators; }
            if (definitionRow.ContainsKey("DecimalPlaces")) { definition.DecimalPlaces = definitionRow["DecimalPlaces"].ToInt(); definition.SavedDecimalPlaces = definition.DecimalPlaces; }
            if (definitionRow.ContainsKey("Min")) { definition.Min = definitionRow["Min"].ToDecimal(); definition.SavedMin = definition.Min; }
            if (definitionRow.ContainsKey("Max")) { definition.Max = definitionRow["Max"].ToDecimal(); definition.SavedMax = definition.Max; }
            if (definitionRow.ContainsKey("Step")) { definition.Step = definitionRow["Step"].ToDecimal(); definition.SavedStep = definition.Step; }
            if (definitionRow.ContainsKey("StringFormat")) { definition.StringFormat = definitionRow["StringFormat"].ToString(); definition.SavedStringFormat = definition.StringFormat; }
            if (definitionRow.ContainsKey("Unit")) { definition.Unit = definitionRow["Unit"].ToString(); definition.SavedUnit = definition.Unit; }
            if (definitionRow.ContainsKey("Width")) { definition.Width = definitionRow["Width"].ToInt(); definition.SavedWidth = definition.Width; }
            if (definitionRow.ContainsKey("SettingEnable")) { definition.SettingEnable = definitionRow["SettingEnable"].ToBool(); definition.SavedSettingEnable = definition.SettingEnable; }
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
                    case "_dot_header": Css._dot_header = definitionRow[1].ToString(); SetCssTable(CssTable._dot_header, definitionRow, CssXls); break;
                    case "_dot_footer": Css._dot_footer = definitionRow[1].ToString(); SetCssTable(CssTable._dot_footer, definitionRow, CssXls); break;
                    case "_dot_footer_space_a": Css._dot_footer_space_a = definitionRow[1].ToString(); SetCssTable(CssTable._dot_footer_space_a, definitionRow, CssXls); break;
                    case "html": Css.html = definitionRow[1].ToString(); SetCssTable(CssTable.html, definitionRow, CssXls); break;
                    case "body": Css.body = definitionRow[1].ToString(); SetCssTable(CssTable.body, definitionRow, CssXls); break;
                    case "h1": Css.h1 = definitionRow[1].ToString(); SetCssTable(CssTable.h1, definitionRow, CssXls); break;
                    case "h2": Css.h2 = definitionRow[1].ToString(); SetCssTable(CssTable.h2, definitionRow, CssXls); break;
                    case "h3": Css.h3 = definitionRow[1].ToString(); SetCssTable(CssTable.h3, definitionRow, CssXls); break;
                    case "form": Css.form = definitionRow[1].ToString(); SetCssTable(CssTable.form, definitionRow, CssXls); break;
                    case "legend": Css.legend = definitionRow[1].ToString(); SetCssTable(CssTable.legend, definitionRow, CssXls); break;
                    case "legend_space___space__asterisk_": Css.legend_space___space__asterisk_ = definitionRow[1].ToString(); SetCssTable(CssTable.legend_space___space__asterisk_, definitionRow, CssXls); break;
                    case "label": Css.label = definitionRow[1].ToString(); SetCssTable(CssTable.label, definitionRow, CssXls); break;
                    case "input": Css.input = definitionRow[1].ToString(); SetCssTable(CssTable.input, definitionRow, CssXls); break;
                    case "select": Css.select = definitionRow[1].ToString(); SetCssTable(CssTable.select, definitionRow, CssXls); break;
                    case "table": Css.table = definitionRow[1].ToString(); SetCssTable(CssTable.table, definitionRow, CssXls); break;
                    case "td": Css.td = definitionRow[1].ToString(); SetCssTable(CssTable.td, definitionRow, CssXls); break;
                    case "pre": Css.pre = definitionRow[1].ToString(); SetCssTable(CssTable.pre, definitionRow, CssXls); break;
                    case "_dot_logo": Css._dot_logo = definitionRow[1].ToString(); SetCssTable(CssTable._dot_logo, definitionRow, CssXls); break;
                    case "_dot_logo_corp": Css._dot_logo_corp = definitionRow[1].ToString(); SetCssTable(CssTable._dot_logo_corp, definitionRow, CssXls); break;
                    case "_dot_logo_product": Css._dot_logo_product = definitionRow[1].ToString(); SetCssTable(CssTable._dot_logo_product, definitionRow, CssXls); break;
                    case "_dot_login_user": Css._dot_login_user = definitionRow[1].ToString(); SetCssTable(CssTable._dot_login_user, definitionRow, CssXls); break;
                    case "_dot_login_user_space___space_p": Css._dot_login_user_space___space_p = definitionRow[1].ToString(); SetCssTable(CssTable._dot_login_user_space___space_p, definitionRow, CssXls); break;
                    case "_dot_login_user_space___space_p_space___space_span": Css._dot_login_user_space___space_p_space___space_span = definitionRow[1].ToString(); SetCssTable(CssTable._dot_login_user_space___space_p_space___space_span, definitionRow, CssXls); break;
                    case "_dot_head": Css._dot_head = definitionRow[1].ToString(); SetCssTable(CssTable._dot_head, definitionRow, CssXls); break;
                    case "_dot_main_form_space___space_nav_colon_first_child": Css._dot_main_form_space___space_nav_colon_first_child = definitionRow[1].ToString(); SetCssTable(CssTable._dot_main_form_space___space_nav_colon_first_child, definitionRow, CssXls); break;
                    case "_dot_nav_breadcrumbs": Css._dot_nav_breadcrumbs = definitionRow[1].ToString(); SetCssTable(CssTable._dot_nav_breadcrumbs, definitionRow, CssXls); break;
                    case "_dot_nav_breadcrumb": Css._dot_nav_breadcrumb = definitionRow[1].ToString(); SetCssTable(CssTable._dot_nav_breadcrumb, definitionRow, CssXls); break;
                    case "_dot_nav_breadcrumb_separator": Css._dot_nav_breadcrumb_separator = definitionRow[1].ToString(); SetCssTable(CssTable._dot_nav_breadcrumb_separator, definitionRow, CssXls); break;
                    case "_dot_nav_functions": Css._dot_nav_functions = definitionRow[1].ToString(); SetCssTable(CssTable._dot_nav_functions, definitionRow, CssXls); break;
                    case "_dot_nav_function": Css._dot_nav_function = definitionRow[1].ToString(); SetCssTable(CssTable._dot_nav_function, definitionRow, CssXls); break;
                    case "_dot_nav_function_space___space__asterisk_": Css._dot_nav_function_space___space__asterisk_ = definitionRow[1].ToString(); SetCssTable(CssTable._dot_nav_function_space___space__asterisk_, definitionRow, CssXls); break;
                    case "_dot_nav_function_space___space__dot_ui_icon": Css._dot_nav_function_space___space__dot_ui_icon = definitionRow[1].ToString(); SetCssTable(CssTable._dot_nav_function_space___space__dot_ui_icon, definitionRow, CssXls); break;
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
                    case "_dot_nav_site_dot_to_upper": Css._dot_nav_site_dot_to_upper = definitionRow[1].ToString(); SetCssTable(CssTable._dot_nav_site_dot_to_upper, definitionRow, CssXls); break;
                    case "_dot_nav_site_dot_to_upper_space_a": Css._dot_nav_site_dot_to_upper_space_a = definitionRow[1].ToString(); SetCssTable(CssTable._dot_nav_site_dot_to_upper_space_a, definitionRow, CssXls); break;
                    case "_dot_nav_site_dot_to_upper_dot_has_image_space_a": Css._dot_nav_site_dot_to_upper_dot_has_image_space_a = definitionRow[1].ToString(); SetCssTable(CssTable._dot_nav_site_dot_to_upper_dot_has_image_space_a, definitionRow, CssXls); break;
                    case "_dot_nav_site_dot_to_upper_space__dot_ui_icon": Css._dot_nav_site_dot_to_upper_space__dot_ui_icon = definitionRow[1].ToString(); SetCssTable(CssTable._dot_nav_site_dot_to_upper_space__dot_ui_icon, definitionRow, CssXls); break;
                    case "_dot_nav_site_space__dot_site_image_thumbnail": Css._dot_nav_site_space__dot_site_image_thumbnail = definitionRow[1].ToString(); SetCssTable(CssTable._dot_nav_site_space__dot_site_image_thumbnail, definitionRow, CssXls); break;
                    case "_dot_nav_site_space__dot_site_image_icon": Css._dot_nav_site_space__dot_site_image_icon = definitionRow[1].ToString(); SetCssTable(CssTable._dot_nav_site_space__dot_site_image_icon, definitionRow, CssXls); break;
                    case "_dot_login": Css._dot_login = definitionRow[1].ToString(); SetCssTable(CssTable._dot_login, definitionRow, CssXls); break;
                    case "_dot_login_commands": Css._dot_login_commands = definitionRow[1].ToString(); SetCssTable(CssTable._dot_login_commands, definitionRow, CssXls); break;
                    case "_dot_demo": Css._dot_demo = definitionRow[1].ToString(); SetCssTable(CssTable._dot_demo, definitionRow, CssXls); break;
                    case "_dot_demo_space___space_fieldset_space___space_div": Css._dot_demo_space___space_fieldset_space___space_div = definitionRow[1].ToString(); SetCssTable(CssTable._dot_demo_space___space_fieldset_space___space_div, definitionRow, CssXls); break;
                    case "_dot_search_results": Css._dot_search_results = definitionRow[1].ToString(); SetCssTable(CssTable._dot_search_results, definitionRow, CssXls); break;
                    case "_dot_search_results_space__dot_count": Css._dot_search_results_space__dot_count = definitionRow[1].ToString(); SetCssTable(CssTable._dot_search_results_space__dot_count, definitionRow, CssXls); break;
                    case "_dot_search_results_space__dot_count_space__dot_label": Css._dot_search_results_space__dot_count_space__dot_label = definitionRow[1].ToString(); SetCssTable(CssTable._dot_search_results_space__dot_count_space__dot_label, definitionRow, CssXls); break;
                    case "_dot_search_results_space__dot_count_space__dot_data": Css._dot_search_results_space__dot_count_space__dot_data = definitionRow[1].ToString(); SetCssTable(CssTable._dot_search_results_space__dot_count_space__dot_data, definitionRow, CssXls); break;
                    case "_dot_search_results_space__dot_result": Css._dot_search_results_space__dot_result = definitionRow[1].ToString(); SetCssTable(CssTable._dot_search_results_space__dot_result, definitionRow, CssXls); break;
                    case "_dot_search_results_space__dot_result_space___space_h3": Css._dot_search_results_space__dot_result_space___space_h3 = definitionRow[1].ToString(); SetCssTable(CssTable._dot_search_results_space__dot_result_space___space_h3, definitionRow, CssXls); break;
                    case "_dot_search_results_space__dot_result_space___space_p": Css._dot_search_results_space__dot_result_space___space_p = definitionRow[1].ToString(); SetCssTable(CssTable._dot_search_results_space__dot_result_space___space_p, definitionRow, CssXls); break;
                    case "_dot_search_results_space__dot_result_colon_hover": Css._dot_search_results_space__dot_result_colon_hover = definitionRow[1].ToString(); SetCssTable(CssTable._dot_search_results_space__dot_result_colon_hover, definitionRow, CssXls); break;
                    case "_dot_error_page": Css._dot_error_page = definitionRow[1].ToString(); SetCssTable(CssTable._dot_error_page, definitionRow, CssXls); break;
                    case "_dot_error_page_title": Css._dot_error_page_title = definitionRow[1].ToString(); SetCssTable(CssTable._dot_error_page_title, definitionRow, CssXls); break;
                    case "_dot_error_page_body": Css._dot_error_page_body = definitionRow[1].ToString(); SetCssTable(CssTable._dot_error_page_body, definitionRow, CssXls); break;
                    case "_dot_error_page_message": Css._dot_error_page_message = definitionRow[1].ToString(); SetCssTable(CssTable._dot_error_page_message, definitionRow, CssXls); break;
                    case "_dot_error_page_action": Css._dot_error_page_action = definitionRow[1].ToString(); SetCssTable(CssTable._dot_error_page_action, definitionRow, CssXls); break;
                    case "_dot_error_page_action_space_em": Css._dot_error_page_action_space_em = definitionRow[1].ToString(); SetCssTable(CssTable._dot_error_page_action_space_em, definitionRow, CssXls); break;
                    case "_dot_error_page_stacktrace": Css._dot_error_page_stacktrace = definitionRow[1].ToString(); SetCssTable(CssTable._dot_error_page_stacktrace, definitionRow, CssXls); break;
                    case "_sharp_application": Css._sharp_application = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_application, definitionRow, CssXls); break;
                    case "_sharp_application_space___space__dot_site_image_icon": Css._sharp_application_space___space__dot_site_image_icon = definitionRow[1].ToString(); SetCssTable(CssTable._sharp_application_space___space__dot_site_image_icon, definitionRow, CssXls); break;
                    case "_dot_application_title": Css._dot_application_title = definitionRow[1].ToString(); SetCssTable(CssTable._dot_application_title, definitionRow, CssXls); break;
                    case "_dot_application_title_space___space__asterisk_": Css._dot_application_title_space___space__asterisk_ = definitionRow[1].ToString(); SetCssTable(CssTable._dot_application_title_space___space__asterisk_, definitionRow, CssXls); break;
                    case "_dot_record_header": Css._dot_record_header = definitionRow[1].ToString(); SetCssTable(CssTable._dot_record_header, definitionRow, CssXls); break;
                    case "_dot_record_header_space___space_div": Css._dot_record_header_space___space_div = definitionRow[1].ToString(); SetCssTable(CssTable._dot_record_header_space___space_div, definitionRow, CssXls); break;
                    case "_dot_record_info": Css._dot_record_info = definitionRow[1].ToString(); SetCssTable(CssTable._dot_record_info, definitionRow, CssXls); break;
                    case "_dot_record_info_space_div": Css._dot_record_info_space_div = definitionRow[1].ToString(); SetCssTable(CssTable._dot_record_info_space_div, definitionRow, CssXls); break;
                    case "_dot_record_info_space_div_space_p": Css._dot_record_info_space_div_space_p = definitionRow[1].ToString(); SetCssTable(CssTable._dot_record_info_space_div_space_p, definitionRow, CssXls); break;
                    case "_dot_record_histories": Css._dot_record_histories = definitionRow[1].ToString(); SetCssTable(CssTable._dot_record_histories, definitionRow, CssXls); break;
                    case "_dot_record_switchers": Css._dot_record_switchers = definitionRow[1].ToString(); SetCssTable(CssTable._dot_record_switchers, definitionRow, CssXls); break;
                    case "_dot_record_switchers_space___space__asterisk_": Css._dot_record_switchers_space___space__asterisk_ = definitionRow[1].ToString(); SetCssTable(CssTable._dot_record_switchers_space___space__asterisk_, definitionRow, CssXls); break;
                    case "_dot_record_switchers_space__dot_current": Css._dot_record_switchers_space__dot_current = definitionRow[1].ToString(); SetCssTable(CssTable._dot_record_switchers_space__dot_current, definitionRow, CssXls); break;
                    case "_dot_edit_form": Css._dot_edit_form = definitionRow[1].ToString(); SetCssTable(CssTable._dot_edit_form, definitionRow, CssXls); break;
                    case "_dot_edit_form_tabs": Css._dot_edit_form_tabs = definitionRow[1].ToString(); SetCssTable(CssTable._dot_edit_form_tabs, definitionRow, CssXls); break;
                    case "_dot_edit_form_tabs_max": Css._dot_edit_form_tabs_max = definitionRow[1].ToString(); SetCssTable(CssTable._dot_edit_form_tabs_max, definitionRow, CssXls); break;
                    case "_dot_edit_form_comments": Css._dot_edit_form_comments = definitionRow[1].ToString(); SetCssTable(CssTable._dot_edit_form_comments, definitionRow, CssXls); break;
                    case "_dot_edit_form_comments_space__dot_title_header": Css._dot_edit_form_comments_space__dot_title_header = definitionRow[1].ToString(); SetCssTable(CssTable._dot_edit_form_comments_space__dot_title_header, definitionRow, CssXls); break;
                    case "_dot_edit_form_mail_list": Css._dot_edit_form_mail_list = definitionRow[1].ToString(); SetCssTable(CssTable._dot_edit_form_mail_list, definitionRow, CssXls); break;
                    case "_dot_field_tab": Css._dot_field_tab = definitionRow[1].ToString(); SetCssTable(CssTable._dot_field_tab, definitionRow, CssXls); break;
                    case "_dot_fieldset": Css._dot_fieldset = definitionRow[1].ToString(); SetCssTable(CssTable._dot_fieldset, definitionRow, CssXls); break;
                    case "_dot_fieldset_dot_enclosed": Css._dot_fieldset_dot_enclosed = definitionRow[1].ToString(); SetCssTable(CssTable._dot_fieldset_dot_enclosed, definitionRow, CssXls); break;
                    case "_dot_fieldset_dot_enclosed_thin": Css._dot_fieldset_dot_enclosed_thin = definitionRow[1].ToString(); SetCssTable(CssTable._dot_fieldset_dot_enclosed_thin, definitionRow, CssXls); break;
                    case "_dot_fieldset_dot_enclosed_thin_space__class_asterisk__equal__yen_field_auto_yen__": Css._dot_fieldset_dot_enclosed_thin_space__class_asterisk__equal__yen_field_auto_yen__ = definitionRow[1].ToString(); SetCssTable(CssTable._dot_fieldset_dot_enclosed_thin_space__class_asterisk__equal__yen_field_auto_yen__, definitionRow, CssXls); break;
                    case "_dot_fieldset_dot_enclosed_auto": Css._dot_fieldset_dot_enclosed_auto = definitionRow[1].ToString(); SetCssTable(CssTable._dot_fieldset_dot_enclosed_auto, definitionRow, CssXls); break;
                    case "_dot_fieldset_class_caret__equal__yen_enclosed_yen___space___space_legend": Css._dot_fieldset_class_caret__equal__yen_enclosed_yen___space___space_legend = definitionRow[1].ToString(); SetCssTable(CssTable._dot_fieldset_class_caret__equal__yen_enclosed_yen___space___space_legend, definitionRow, CssXls); break;
                    case "_dot_command_main_container": Css._dot_command_main_container = definitionRow[1].ToString(); SetCssTable(CssTable._dot_command_main_container, definitionRow, CssXls); break;
                    case "_dot_command_main": Css._dot_command_main = definitionRow[1].ToString(); SetCssTable(CssTable._dot_command_main, definitionRow, CssXls); break;
                    case "_dot_command_main_space___space_button": Css._dot_command_main_space___space_button = definitionRow[1].ToString(); SetCssTable(CssTable._dot_command_main_space___space_button, definitionRow, CssXls); break;
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
                    case "_dot_control_textarea": Css._dot_control_textarea = definitionRow[1].ToString(); SetCssTable(CssTable._dot_control_textarea, definitionRow, CssXls); break;
                    case "_dot_control_markup": Css._dot_control_markup = definitionRow[1].ToString(); SetCssTable(CssTable._dot_control_markup, definitionRow, CssXls); break;
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
                    case "_dot_container_selectable_space__dot_control": Css._dot_container_selectable_space__dot_control = definitionRow[1].ToString(); SetCssTable(CssTable._dot_container_selectable_space__dot_control, definitionRow, CssXls); break;
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
                    case "_dot_tooltip": Css._dot_tooltip = definitionRow[1].ToString(); SetCssTable(CssTable._dot_tooltip, definitionRow, CssXls); break;
                    case "_dot_no_border": Css._dot_no_border = definitionRow[1].ToString(); SetCssTable(CssTable._dot_no_border, definitionRow, CssXls); break;
                    case "_dot_mail_list": Css._dot_mail_list = definitionRow[1].ToString(); SetCssTable(CssTable._dot_mail_list, definitionRow, CssXls); break;
                    case "_dot_mail_list_space___space__dot_mail_content": Css._dot_mail_list_space___space__dot_mail_content = definitionRow[1].ToString(); SetCssTable(CssTable._dot_mail_list_space___space__dot_mail_content, definitionRow, CssXls); break;
                    case "_dot_dataview_filters": Css._dot_dataview_filters = definitionRow[1].ToString(); SetCssTable(CssTable._dot_dataview_filters, definitionRow, CssXls); break;
                    case "_dot_dataview_filters_space___space_div_space___space_div": Css._dot_dataview_filters_space___space_div_space___space_div = definitionRow[1].ToString(); SetCssTable(CssTable._dot_dataview_filters_space___space_div_space___space_div, definitionRow, CssXls); break;
                    case "_dot_dataview_filters_space___space_div_space___space_button": Css._dot_dataview_filters_space___space_div_space___space_button = definitionRow[1].ToString(); SetCssTable(CssTable._dot_dataview_filters_space___space_div_space___space_button, definitionRow, CssXls); break;
                    case "_dot_dataview_selector": Css._dot_dataview_selector = definitionRow[1].ToString(); SetCssTable(CssTable._dot_dataview_selector, definitionRow, CssXls); break;
                    case "_dot_dataview_selector_space___space_button": Css._dot_dataview_selector_space___space_button = definitionRow[1].ToString(); SetCssTable(CssTable._dot_dataview_selector_space___space_button, definitionRow, CssXls); break;
                    case "_dot_aggregations": Css._dot_aggregations = definitionRow[1].ToString(); SetCssTable(CssTable._dot_aggregations, definitionRow, CssXls); break;
                    case "_dot_aggregations_space__dot_label": Css._dot_aggregations_space__dot_label = definitionRow[1].ToString(); SetCssTable(CssTable._dot_aggregations_space__dot_label, definitionRow, CssXls); break;
                    case "_dot_aggregations_space__dot_data": Css._dot_aggregations_space__dot_data = definitionRow[1].ToString(); SetCssTable(CssTable._dot_aggregations_space__dot_data, definitionRow, CssXls); break;
                    case "_dot_aggregations_space_em": Css._dot_aggregations_space_em = definitionRow[1].ToString(); SetCssTable(CssTable._dot_aggregations_space_em, definitionRow, CssXls); break;
                    case "_dot_grid": Css._dot_grid = definitionRow[1].ToString(); SetCssTable(CssTable._dot_grid, definitionRow, CssXls); break;
                    case "_dot_grid_dot_fixed": Css._dot_grid_dot_fixed = definitionRow[1].ToString(); SetCssTable(CssTable._dot_grid_dot_fixed, definitionRow, CssXls); break;
                    case "_dot_grid_space_caption": Css._dot_grid_space_caption = definitionRow[1].ToString(); SetCssTable(CssTable._dot_grid_space_caption, definitionRow, CssXls); break;
                    case "_dot_grid_space_th": Css._dot_grid_space_th = definitionRow[1].ToString(); SetCssTable(CssTable._dot_grid_space_th, definitionRow, CssXls); break;
                    case "_dot_grid_space_th_space___space_div": Css._dot_grid_space_th_space___space_div = definitionRow[1].ToString(); SetCssTable(CssTable._dot_grid_space_th_space___space_div, definitionRow, CssXls); break;
                    case "_dot_grid_space_th_space_span": Css._dot_grid_space_th_space_span = definitionRow[1].ToString(); SetCssTable(CssTable._dot_grid_space_th_space_span, definitionRow, CssXls); break;
                    case "_dot_grid_space_th_colon_first_child": Css._dot_grid_space_th_colon_first_child = definitionRow[1].ToString(); SetCssTable(CssTable._dot_grid_space_th_colon_first_child, definitionRow, CssXls); break;
                    case "_dot_grid_space_th_colon_last_child": Css._dot_grid_space_th_colon_last_child = definitionRow[1].ToString(); SetCssTable(CssTable._dot_grid_space_th_colon_last_child, definitionRow, CssXls); break;
                    case "_dot_grid_space_th_dot_sortable_colon_hover": Css._dot_grid_space_th_dot_sortable_colon_hover = definitionRow[1].ToString(); SetCssTable(CssTable._dot_grid_space_th_dot_sortable_colon_hover, definitionRow, CssXls); break;
                    case "_dot_grid_space_td": Css._dot_grid_space_td = definitionRow[1].ToString(); SetCssTable(CssTable._dot_grid_space_td, definitionRow, CssXls); break;
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
                    case "_dot_gantt_chart": Css._dot_gantt_chart = definitionRow[1].ToString(); SetCssTable(CssTable._dot_gantt_chart, definitionRow, CssXls); break;
                    case "_dot_gantt": Css._dot_gantt = definitionRow[1].ToString(); SetCssTable(CssTable._dot_gantt, definitionRow, CssXls); break;
                    case "_dot_gantt_space__dot_date": Css._dot_gantt_space__dot_date = definitionRow[1].ToString(); SetCssTable(CssTable._dot_gantt_space__dot_date, definitionRow, CssXls); break;
                    case "_dot_gantt_space__dot_now": Css._dot_gantt_space__dot_now = definitionRow[1].ToString(); SetCssTable(CssTable._dot_gantt_space__dot_now, definitionRow, CssXls); break;
                    case "_dot_gantt_space__dot_planned_space_rect": Css._dot_gantt_space__dot_planned_space_rect = definitionRow[1].ToString(); SetCssTable(CssTable._dot_gantt_space__dot_planned_space_rect, definitionRow, CssXls); break;
                    case "_dot_gantt_space__dot_earned_space_rect": Css._dot_gantt_space__dot_earned_space_rect = definitionRow[1].ToString(); SetCssTable(CssTable._dot_gantt_space__dot_earned_space_rect, definitionRow, CssXls); break;
                    case "_dot_gantt_space_rect_dot_delay": Css._dot_gantt_space_rect_dot_delay = definitionRow[1].ToString(); SetCssTable(CssTable._dot_gantt_space_rect_dot_delay, definitionRow, CssXls); break;
                    case "_dot_gantt_space_rect_dot_completed": Css._dot_gantt_space_rect_dot_completed = definitionRow[1].ToString(); SetCssTable(CssTable._dot_gantt_space_rect_dot_completed, definitionRow, CssXls); break;
                    case "_dot_gantt_space__dot_title_space_text": Css._dot_gantt_space__dot_title_space_text = definitionRow[1].ToString(); SetCssTable(CssTable._dot_gantt_space__dot_title_space_text, definitionRow, CssXls); break;
                    case "_dot_gantt_space__dot_title_space_text_dot_delay": Css._dot_gantt_space__dot_title_space_text_dot_delay = definitionRow[1].ToString(); SetCssTable(CssTable._dot_gantt_space__dot_title_space_text_dot_delay, definitionRow, CssXls); break;
                    case "_dot_gantt_axis": Css._dot_gantt_axis = definitionRow[1].ToString(); SetCssTable(CssTable._dot_gantt_axis, definitionRow, CssXls); break;
                    case "_dot_gantt_axis_space_text": Css._dot_gantt_axis_space_text = definitionRow[1].ToString(); SetCssTable(CssTable._dot_gantt_axis_space_text, definitionRow, CssXls); break;
                    case "_dot_burn_down": Css._dot_burn_down = definitionRow[1].ToString(); SetCssTable(CssTable._dot_burn_down, definitionRow, CssXls); break;
                    case "_dot_burn_down_space__dot_now": Css._dot_burn_down_space__dot_now = definitionRow[1].ToString(); SetCssTable(CssTable._dot_burn_down_space__dot_now, definitionRow, CssXls); break;
                    case "_dot_burn_down_space__dot_total": Css._dot_burn_down_space__dot_total = definitionRow[1].ToString(); SetCssTable(CssTable._dot_burn_down_space__dot_total, definitionRow, CssXls); break;
                    case "_dot_burn_down_space__dot_planned": Css._dot_burn_down_space__dot_planned = definitionRow[1].ToString(); SetCssTable(CssTable._dot_burn_down_space__dot_planned, definitionRow, CssXls); break;
                    case "_dot_burn_down_space__dot_earned": Css._dot_burn_down_space__dot_earned = definitionRow[1].ToString(); SetCssTable(CssTable._dot_burn_down_space__dot_earned, definitionRow, CssXls); break;
                    case "_dot_burn_down_space__dot_total_space_circle": Css._dot_burn_down_space__dot_total_space_circle = definitionRow[1].ToString(); SetCssTable(CssTable._dot_burn_down_space__dot_total_space_circle, definitionRow, CssXls); break;
                    case "_dot_burn_down_space__dot_planned_space_circle": Css._dot_burn_down_space__dot_planned_space_circle = definitionRow[1].ToString(); SetCssTable(CssTable._dot_burn_down_space__dot_planned_space_circle, definitionRow, CssXls); break;
                    case "_dot_burn_down_space__dot_earned_space_circle": Css._dot_burn_down_space__dot_earned_space_circle = definitionRow[1].ToString(); SetCssTable(CssTable._dot_burn_down_space__dot_earned_space_circle, definitionRow, CssXls); break;
                    case "_dot_burn_down_details_row_space_td": Css._dot_burn_down_details_row_space_td = definitionRow[1].ToString(); SetCssTable(CssTable._dot_burn_down_details_row_space_td, definitionRow, CssXls); break;
                    case "_dot_burn_down_details_row_space_td_dot_warning": Css._dot_burn_down_details_row_space_td_dot_warning = definitionRow[1].ToString(); SetCssTable(CssTable._dot_burn_down_details_row_space_td_dot_warning, definitionRow, CssXls); break;
                    case "_dot_burn_down_details_row_space_td_dot_difference": Css._dot_burn_down_details_row_space_td_dot_difference = definitionRow[1].ToString(); SetCssTable(CssTable._dot_burn_down_details_row_space_td_dot_difference, definitionRow, CssXls); break;
                    case "_dot_burn_down_details_row_space_td_dot_difference_dot_warning": Css._dot_burn_down_details_row_space_td_dot_difference_dot_warning = definitionRow[1].ToString(); SetCssTable(CssTable._dot_burn_down_details_row_space_td_dot_difference_dot_warning, definitionRow, CssXls); break;
                    case "_dot_burn_down_record_details_space__dot_user_info": Css._dot_burn_down_record_details_space__dot_user_info = definitionRow[1].ToString(); SetCssTable(CssTable._dot_burn_down_record_details_space__dot_user_info, definitionRow, CssXls); break;
                    case "_dot_burn_down_record_details_space__dot_items": Css._dot_burn_down_record_details_space__dot_items = definitionRow[1].ToString(); SetCssTable(CssTable._dot_burn_down_record_details_space__dot_items, definitionRow, CssXls); break;
                    case "_dot_burn_down_record_details_space__dot_items_space_a": Css._dot_burn_down_record_details_space__dot_items_space_a = definitionRow[1].ToString(); SetCssTable(CssTable._dot_burn_down_record_details_space__dot_items_space_a, definitionRow, CssXls); break;
                    case "_dot_burn_down_record_details_space__dot_items_space_a_colon_hover": Css._dot_burn_down_record_details_space__dot_items_space_a_colon_hover = definitionRow[1].ToString(); SetCssTable(CssTable._dot_burn_down_record_details_space__dot_items_space_a_colon_hover, definitionRow, CssXls); break;
                    case "_dot_time_series": Css._dot_time_series = definitionRow[1].ToString(); SetCssTable(CssTable._dot_time_series, definitionRow, CssXls); break;
                    case "_dot_time_series_space__dot_surface": Css._dot_time_series_space__dot_surface = definitionRow[1].ToString(); SetCssTable(CssTable._dot_time_series_space__dot_surface, definitionRow, CssXls); break;
                    case "_dot_time_series_space__dot_index": Css._dot_time_series_space__dot_index = definitionRow[1].ToString(); SetCssTable(CssTable._dot_time_series_space__dot_index, definitionRow, CssXls); break;
                    case "_dot_kamban_row": Css._dot_kamban_row = definitionRow[1].ToString(); SetCssTable(CssTable._dot_kamban_row, definitionRow, CssXls); break;
                    case "_dot_kamban_container_dot_hover": Css._dot_kamban_container_dot_hover = definitionRow[1].ToString(); SetCssTable(CssTable._dot_kamban_container_dot_hover, definitionRow, CssXls); break;
                    case "_dot_kamban_container_space__dot_kamban_item_colon_last_child": Css._dot_kamban_container_space__dot_kamban_item_colon_last_child = definitionRow[1].ToString(); SetCssTable(CssTable._dot_kamban_container_space__dot_kamban_item_colon_last_child, definitionRow, CssXls); break;
                    case "_dot_kamban_item": Css._dot_kamban_item = definitionRow[1].ToString(); SetCssTable(CssTable._dot_kamban_item, definitionRow, CssXls); break;
                    case "_dot_kamban_item_colon_hover": Css._dot_kamban_item_colon_hover = definitionRow[1].ToString(); SetCssTable(CssTable._dot_kamban_item_colon_hover, definitionRow, CssXls); break;
                    case "_dot_kamban_item_dot_changed": Css._dot_kamban_item_dot_changed = definitionRow[1].ToString(); SetCssTable(CssTable._dot_kamban_item_dot_changed, definitionRow, CssXls); break;
                    case "_dot_kamban_item_space__dot_ui_icon": Css._dot_kamban_item_space__dot_ui_icon = definitionRow[1].ToString(); SetCssTable(CssTable._dot_kamban_item_space__dot_ui_icon, definitionRow, CssXls); break;
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
                    case "_dot_grid_space__class_asterisk__equal__yen_status__yen__": Css._dot_grid_space__class_asterisk__equal__yen_status__yen__ = definitionRow[1].ToString(); SetCssTable(CssTable._dot_grid_space__class_asterisk__equal__yen_status__yen__, definitionRow, CssXls); break;
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
                    case "_dot_button_space__dot_ui_button_icon_primary": Css._dot_button_space__dot_ui_button_icon_primary = definitionRow[1].ToString(); SetCssTable(CssTable._dot_button_space__dot_ui_button_icon_primary, definitionRow, CssXls); break;
                    case "_dot_ui_button_text": Css._dot_ui_button_text = definitionRow[1].ToString(); SetCssTable(CssTable._dot_ui_button_text, definitionRow, CssXls); break;
                    case "_dot_ui_dialog": Css._dot_ui_dialog = definitionRow[1].ToString(); SetCssTable(CssTable._dot_ui_dialog, definitionRow, CssXls); break;
                    case "_dot_ui_icon_dot_a": Css._dot_ui_icon_dot_a = definitionRow[1].ToString(); SetCssTable(CssTable._dot_ui_icon_dot_a, definitionRow, CssXls); break;
                    case "_dot_ui_spinner": Css._dot_ui_spinner = definitionRow[1].ToString(); SetCssTable(CssTable._dot_ui_spinner, definitionRow, CssXls); break;
                    case "_dot_margin_bottom": Css._dot_margin_bottom = definitionRow[1].ToString(); SetCssTable(CssTable._dot_margin_bottom, definitionRow, CssXls); break;
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

        public static XlsIo DataViewXls;
        public static List<DataViewDefinition> DataViewDefinitionCollection;
        public static DataViewColumn2nd DataView;
        public static DataViewTable DataViewTable;

        public static void SetDataViewDefinition()
        {
            ConstructDataViewDefinitions();
            if (DataViewXls.AccessStatus != Files.AccessStatuses.Read) { return; }
            DataViewXls.XlsSheet.ForEach(definitionRow =>
            {
                switch (definitionRow[0].ToString())
                {
                    case "Issues_Gantt": DataView.Issues_Gantt = definitionRow[1].ToString(); SetDataViewTable(DataViewTable.Issues_Gantt, definitionRow, DataViewXls); break;
                    case "Issues_BurnDown": DataView.Issues_BurnDown = definitionRow[1].ToString(); SetDataViewTable(DataViewTable.Issues_BurnDown, definitionRow, DataViewXls); break;
                    case "Issues_TimeSeries": DataView.Issues_TimeSeries = definitionRow[1].ToString(); SetDataViewTable(DataViewTable.Issues_TimeSeries, definitionRow, DataViewXls); break;
                    case "Issues_Kamban": DataView.Issues_Kamban = definitionRow[1].ToString(); SetDataViewTable(DataViewTable.Issues_Kamban, definitionRow, DataViewXls); break;
                    case "Results_TimeSeries": DataView.Results_TimeSeries = definitionRow[1].ToString(); SetDataViewTable(DataViewTable.Results_TimeSeries, definitionRow, DataViewXls); break;
                    case "Results_Kamban": DataView.Results_Kamban = definitionRow[1].ToString(); SetDataViewTable(DataViewTable.Results_Kamban, definitionRow, DataViewXls); break;
                    default: break;
                }
            });
            DataViewXls.XlsSheet.AsEnumerable().Skip(1).Where(o => o[0].ToString() != string.Empty).ForEach(definitionRow =>
            {
                var newDataViewDefinition = new DataViewDefinition();
                if (definitionRow.ContainsKey("Id")) { newDataViewDefinition.Id = definitionRow["Id"].ToString(); newDataViewDefinition.SavedId = newDataViewDefinition.Id; }
                if (definitionRow.ContainsKey("ReferenceType")) { newDataViewDefinition.ReferenceType = definitionRow["ReferenceType"].ToString(); newDataViewDefinition.SavedReferenceType = newDataViewDefinition.ReferenceType; }
                if (definitionRow.ContainsKey("Name")) { newDataViewDefinition.Name = definitionRow["Name"].ToString(); newDataViewDefinition.SavedName = newDataViewDefinition.Name; }
                DataViewDefinitionCollection.Add(newDataViewDefinition);
            });
        }

        private static void SetDataViewTable(DataViewDefinition definition, XlsRow definitionRow, XlsIo dataViewxls)
        {
            if (definitionRow.ContainsKey("Id")) { definition.Id = definitionRow["Id"].ToString(); definition.SavedId = definition.Id; }
            if (definitionRow.ContainsKey("ReferenceType")) { definition.ReferenceType = definitionRow["ReferenceType"].ToString(); definition.SavedReferenceType = definition.ReferenceType; }
            if (definitionRow.ContainsKey("Name")) { definition.Name = definitionRow["Name"].ToString(); definition.SavedName = definition.Name; }
        }

        private static void ConstructDataViewDefinitions()
        {
            DataViewXls = Initializer.DefinitionFile("definition_DataView.xlsm");
            DataViewDefinitionCollection = new List<DataViewDefinition>();
            DataView = new DataViewColumn2nd();
            DataViewTable = new DataViewTable();
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
                    case "Issue1": Demo.Issue1 = definitionRow[1].ToString(); SetDemoTable(DemoTable.Issue1, definitionRow, DemoXls); break;
                    case "Issue2": Demo.Issue2 = definitionRow[1].ToString(); SetDemoTable(DemoTable.Issue2, definitionRow, DemoXls); break;
                    case "Issue3": Demo.Issue3 = definitionRow[1].ToString(); SetDemoTable(DemoTable.Issue3, definitionRow, DemoXls); break;
                    case "Issue4": Demo.Issue4 = definitionRow[1].ToString(); SetDemoTable(DemoTable.Issue4, definitionRow, DemoXls); break;
                    case "Issue5": Demo.Issue5 = definitionRow[1].ToString(); SetDemoTable(DemoTable.Issue5, definitionRow, DemoXls); break;
                    case "Issue6": Demo.Issue6 = definitionRow[1].ToString(); SetDemoTable(DemoTable.Issue6, definitionRow, DemoXls); break;
                    case "Issue7": Demo.Issue7 = definitionRow[1].ToString(); SetDemoTable(DemoTable.Issue7, definitionRow, DemoXls); break;
                    case "Issue8": Demo.Issue8 = definitionRow[1].ToString(); SetDemoTable(DemoTable.Issue8, definitionRow, DemoXls); break;
                    case "Issue9": Demo.Issue9 = definitionRow[1].ToString(); SetDemoTable(DemoTable.Issue9, definitionRow, DemoXls); break;
                    case "Issue10": Demo.Issue10 = definitionRow[1].ToString(); SetDemoTable(DemoTable.Issue10, definitionRow, DemoXls); break;
                    case "Issue11": Demo.Issue11 = definitionRow[1].ToString(); SetDemoTable(DemoTable.Issue11, definitionRow, DemoXls); break;
                    case "Issue12": Demo.Issue12 = definitionRow[1].ToString(); SetDemoTable(DemoTable.Issue12, definitionRow, DemoXls); break;
                    case "Issue13": Demo.Issue13 = definitionRow[1].ToString(); SetDemoTable(DemoTable.Issue13, definitionRow, DemoXls); break;
                    case "Issue14": Demo.Issue14 = definitionRow[1].ToString(); SetDemoTable(DemoTable.Issue14, definitionRow, DemoXls); break;
                    case "Issue15": Demo.Issue15 = definitionRow[1].ToString(); SetDemoTable(DemoTable.Issue15, definitionRow, DemoXls); break;
                    case "Result1": Demo.Result1 = definitionRow[1].ToString(); SetDemoTable(DemoTable.Result1, definitionRow, DemoXls); break;
                    case "Comment1": Demo.Comment1 = definitionRow[1].ToString(); SetDemoTable(DemoTable.Comment1, definitionRow, DemoXls); break;
                    case "Comment2": Demo.Comment2 = definitionRow[1].ToString(); SetDemoTable(DemoTable.Comment2, definitionRow, DemoXls); break;
                    case "Comment3": Demo.Comment3 = definitionRow[1].ToString(); SetDemoTable(DemoTable.Comment3, definitionRow, DemoXls); break;
                    case "Comment4": Demo.Comment4 = definitionRow[1].ToString(); SetDemoTable(DemoTable.Comment4, definitionRow, DemoXls); break;
                    case "Comment5": Demo.Comment5 = definitionRow[1].ToString(); SetDemoTable(DemoTable.Comment5, definitionRow, DemoXls); break;
                    case "Comment6": Demo.Comment6 = definitionRow[1].ToString(); SetDemoTable(DemoTable.Comment6, definitionRow, DemoXls); break;
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
                if (definitionRow.ContainsKey("Manager")) { newDemoDefinition.Manager = definitionRow["Manager"].ToString(); newDemoDefinition.SavedManager = newDemoDefinition.Manager; }
                if (definitionRow.ContainsKey("Owner")) { newDemoDefinition.Owner = definitionRow["Owner"].ToString(); newDemoDefinition.SavedOwner = newDemoDefinition.Owner; }
                if (definitionRow.ContainsKey("ClassA")) { newDemoDefinition.ClassA = definitionRow["ClassA"].ToString(); newDemoDefinition.SavedClassA = newDemoDefinition.ClassA; }
                if (definitionRow.ContainsKey("ClassB")) { newDemoDefinition.ClassB = definitionRow["ClassB"].ToString(); newDemoDefinition.SavedClassB = newDemoDefinition.ClassB; }
                if (definitionRow.ContainsKey("ClassC")) { newDemoDefinition.ClassC = definitionRow["ClassC"].ToString(); newDemoDefinition.SavedClassC = newDemoDefinition.ClassC; }
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
            if (definitionRow.ContainsKey("Manager")) { definition.Manager = definitionRow["Manager"].ToString(); definition.SavedManager = definition.Manager; }
            if (definitionRow.ContainsKey("Owner")) { definition.Owner = definitionRow["Owner"].ToString(); definition.SavedOwner = definition.Owner; }
            if (definitionRow.ContainsKey("ClassA")) { definition.ClassA = definitionRow["ClassA"].ToString(); definition.SavedClassA = definition.ClassA; }
            if (definitionRow.ContainsKey("ClassB")) { definition.ClassB = definitionRow["ClassB"].ToString(); definition.SavedClassB = definition.ClassB; }
            if (definitionRow.ContainsKey("ClassC")) { definition.ClassC = definitionRow["ClassC"].ToString(); definition.SavedClassC = definition.ClassC; }
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

        public static XlsIo DisplayXls;
        public static List<DisplayDefinition> DisplayDefinitionCollection;
        public static DisplayColumn2nd Display;
        public static DisplayTable DisplayTable;

        public static void SetDisplayDefinition()
        {
            ConstructDisplayDefinitions();
            if (DisplayXls.AccessStatus != Files.AccessStatuses.Read) { return; }
            DisplayXls.XlsSheet.ForEach(definitionRow =>
            {
                switch (definitionRow[0].ToString())
                {
                    case "Login": Display.Login = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Login, definitionRow, DisplayXls); break;
                    case "Login_ja": Display.Login_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Login_ja, definitionRow, DisplayXls); break;
                    case "Top": Display.Top = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Top, definitionRow, DisplayXls); break;
                    case "Top_ja": Display.Top_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Top_ja, definitionRow, DisplayXls); break;
                    case "List": Display.List = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.List, definitionRow, DisplayXls); break;
                    case "List_ja": Display.List_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.List_ja, definitionRow, DisplayXls); break;
                    case "New": Display.New = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.New, definitionRow, DisplayXls); break;
                    case "New_ja": Display.New_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.New_ja, definitionRow, DisplayXls); break;
                    case "EditSettings": Display.EditSettings = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.EditSettings, definitionRow, DisplayXls); break;
                    case "EditSettings_ja": Display.EditSettings_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.EditSettings_ja, definitionRow, DisplayXls); break;
                    case "Create": Display.Create = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Create, definitionRow, DisplayXls); break;
                    case "Create_ja": Display.Create_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Create_ja, definitionRow, DisplayXls); break;
                    case "Update": Display.Update = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Update, definitionRow, DisplayXls); break;
                    case "Update_ja": Display.Update_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Update_ja, definitionRow, DisplayXls); break;
                    case "Save": Display.Save = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Save, definitionRow, DisplayXls); break;
                    case "Save_ja": Display.Save_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Save_ja, definitionRow, DisplayXls); break;
                    case "Change": Display.Change = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Change, definitionRow, DisplayXls); break;
                    case "Change_ja": Display.Change_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Change_ja, definitionRow, DisplayXls); break;
                    case "Add": Display.Add = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Add, definitionRow, DisplayXls); break;
                    case "Add_ja": Display.Add_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Add_ja, definitionRow, DisplayXls); break;
                    case "Register": Display.Register = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Register, definitionRow, DisplayXls); break;
                    case "Register_ja": Display.Register_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Register_ja, definitionRow, DisplayXls); break;
                    case "Reset": Display.Reset = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Reset, definitionRow, DisplayXls); break;
                    case "Reset_ja": Display.Reset_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Reset_ja, definitionRow, DisplayXls); break;
                    case "Copy": Display.Copy = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Copy, definitionRow, DisplayXls); break;
                    case "Copy_ja": Display.Copy_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Copy_ja, definitionRow, DisplayXls); break;
                    case "Move": Display.Move = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Move, definitionRow, DisplayXls); break;
                    case "Move_ja": Display.Move_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Move_ja, definitionRow, DisplayXls); break;
                    case "BulkMove": Display.BulkMove = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.BulkMove, definitionRow, DisplayXls); break;
                    case "BulkMove_ja": Display.BulkMove_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.BulkMove_ja, definitionRow, DisplayXls); break;
                    case "Mail": Display.Mail = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Mail, definitionRow, DisplayXls); break;
                    case "Mail_ja": Display.Mail_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Mail_ja, definitionRow, DisplayXls); break;
                    case "MailAddress": Display.MailAddress = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.MailAddress, definitionRow, DisplayXls); break;
                    case "MailAddress_ja": Display.MailAddress_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.MailAddress_ja, definitionRow, DisplayXls); break;
                    case "SendMail": Display.SendMail = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SendMail, definitionRow, DisplayXls); break;
                    case "SendMail_ja": Display.SendMail_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SendMail_ja, definitionRow, DisplayXls); break;
                    case "Delete": Display.Delete = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Delete, definitionRow, DisplayXls); break;
                    case "Delete_ja": Display.Delete_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Delete_ja, definitionRow, DisplayXls); break;
                    case "Separate": Display.Separate = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Separate, definitionRow, DisplayXls); break;
                    case "Separate_ja": Display.Separate_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Separate_ja, definitionRow, DisplayXls); break;
                    case "BulkDelete": Display.BulkDelete = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.BulkDelete, definitionRow, DisplayXls); break;
                    case "BulkDelete_ja": Display.BulkDelete_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.BulkDelete_ja, definitionRow, DisplayXls); break;
                    case "Import": Display.Import = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Import, definitionRow, DisplayXls); break;
                    case "Import_ja": Display.Import_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Import_ja, definitionRow, DisplayXls); break;
                    case "Export": Display.Export = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Export, definitionRow, DisplayXls); break;
                    case "Export_ja": Display.Export_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Export_ja, definitionRow, DisplayXls); break;
                    case "Cancel": Display.Cancel = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Cancel, definitionRow, DisplayXls); break;
                    case "Cancel_ja": Display.Cancel_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Cancel_ja, definitionRow, DisplayXls); break;
                    case "GoBack": Display.GoBack = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.GoBack, definitionRow, DisplayXls); break;
                    case "GoBack_ja": Display.GoBack_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.GoBack_ja, definitionRow, DisplayXls); break;
                    case "Synchronize": Display.Synchronize = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Synchronize, definitionRow, DisplayXls); break;
                    case "Synchronize_ja": Display.Synchronize_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Synchronize_ja, definitionRow, DisplayXls); break;
                    case "Operations": Display.Operations = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Operations, definitionRow, DisplayXls); break;
                    case "Operations_ja": Display.Operations_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Operations_ja, definitionRow, DisplayXls); break;
                    case "GroupBy": Display.GroupBy = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.GroupBy, definitionRow, DisplayXls); break;
                    case "GroupBy_ja": Display.GroupBy_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.GroupBy_ja, definitionRow, DisplayXls); break;
                    case "Date": Display.Date = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Date, definitionRow, DisplayXls); break;
                    case "Date_ja": Display.Date_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Date_ja, definitionRow, DisplayXls); break;
                    case "Count": Display.Count = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Count, definitionRow, DisplayXls); break;
                    case "Count_ja": Display.Count_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Count_ja, definitionRow, DisplayXls); break;
                    case "Total": Display.Total = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Total, definitionRow, DisplayXls); break;
                    case "Total_ja": Display.Total_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Total_ja, definitionRow, DisplayXls); break;
                    case "Average": Display.Average = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Average, definitionRow, DisplayXls); break;
                    case "Average_ja": Display.Average_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Average_ja, definitionRow, DisplayXls); break;
                    case "Max": Display.Max = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Max, definitionRow, DisplayXls); break;
                    case "Max_ja": Display.Max_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Max_ja, definitionRow, DisplayXls); break;
                    case "Min": Display.Min = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Min, definitionRow, DisplayXls); break;
                    case "Min_ja": Display.Min_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Min_ja, definitionRow, DisplayXls); break;
                    case "Step": Display.Step = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Step, definitionRow, DisplayXls); break;
                    case "Step_ja": Display.Step_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Step_ja, definitionRow, DisplayXls); break;
                    case "DecimalPlaces": Display.DecimalPlaces = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.DecimalPlaces, definitionRow, DisplayXls); break;
                    case "DecimalPlaces_ja": Display.DecimalPlaces_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.DecimalPlaces_ja, definitionRow, DisplayXls); break;
                    case "ToUpper": Display.ToUpper = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.ToUpper, definitionRow, DisplayXls); break;
                    case "ToUpper_ja": Display.ToUpper_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.ToUpper_ja, definitionRow, DisplayXls); break;
                    case "MoveUp": Display.MoveUp = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.MoveUp, definitionRow, DisplayXls); break;
                    case "MoveUp_ja": Display.MoveUp_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.MoveUp_ja, definitionRow, DisplayXls); break;
                    case "MoveDown": Display.MoveDown = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.MoveDown, definitionRow, DisplayXls); break;
                    case "MoveDown_ja": Display.MoveDown_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.MoveDown_ja, definitionRow, DisplayXls); break;
                    case "Previous": Display.Previous = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Previous, definitionRow, DisplayXls); break;
                    case "Previous_ja": Display.Previous_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Previous_ja, definitionRow, DisplayXls); break;
                    case "Next": Display.Next = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Next, definitionRow, DisplayXls); break;
                    case "Next_ja": Display.Next_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Next_ja, definitionRow, DisplayXls); break;
                    case "Reload": Display.Reload = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Reload, definitionRow, DisplayXls); break;
                    case "Reload_ja": Display.Reload_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Reload_ja, definitionRow, DisplayXls); break;
                    case "Newer": Display.Newer = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Newer, definitionRow, DisplayXls); break;
                    case "Newer_ja": Display.Newer_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Newer_ja, definitionRow, DisplayXls); break;
                    case "Older": Display.Older = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Older, definitionRow, DisplayXls); break;
                    case "Older_ja": Display.Older_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Older_ja, definitionRow, DisplayXls); break;
                    case "Latest": Display.Latest = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Latest, definitionRow, DisplayXls); break;
                    case "Latest_ja": Display.Latest_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Latest_ja, definitionRow, DisplayXls); break;
                    case "Visible": Display.Visible = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Visible, definitionRow, DisplayXls); break;
                    case "Visible_ja": Display.Visible_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Visible_ja, definitionRow, DisplayXls); break;
                    case "Hide": Display.Hide = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Hide, definitionRow, DisplayXls); break;
                    case "Hide_ja": Display.Hide_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Hide_ja, definitionRow, DisplayXls); break;
                    case "PlannedValue": Display.PlannedValue = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.PlannedValue, definitionRow, DisplayXls); break;
                    case "PlannedValue_ja": Display.PlannedValue_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.PlannedValue_ja, definitionRow, DisplayXls); break;
                    case "EarnedValue": Display.EarnedValue = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.EarnedValue, definitionRow, DisplayXls); break;
                    case "EarnedValue_ja": Display.EarnedValue_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.EarnedValue_ja, definitionRow, DisplayXls); break;
                    case "Difference": Display.Difference = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Difference, definitionRow, DisplayXls); break;
                    case "Difference_ja": Display.Difference_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Difference_ja, definitionRow, DisplayXls); break;
                    case "ChangePassword": Display.ChangePassword = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.ChangePassword, definitionRow, DisplayXls); break;
                    case "ChangePassword_ja": Display.ChangePassword_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.ChangePassword_ja, definitionRow, DisplayXls); break;
                    case "ResetPassword": Display.ResetPassword = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.ResetPassword, definitionRow, DisplayXls); break;
                    case "ResetPassword_ja": Display.ResetPassword_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.ResetPassword_ja, definitionRow, DisplayXls); break;
                    case "ReadOnly": Display.ReadOnly = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.ReadOnly, definitionRow, DisplayXls); break;
                    case "ReadOnly_ja": Display.ReadOnly_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.ReadOnly_ja, definitionRow, DisplayXls); break;
                    case "ReadWrite": Display.ReadWrite = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.ReadWrite, definitionRow, DisplayXls); break;
                    case "ReadWrite_ja": Display.ReadWrite_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.ReadWrite_ja, definitionRow, DisplayXls); break;
                    case "Leader": Display.Leader = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Leader, definitionRow, DisplayXls); break;
                    case "Leader_ja": Display.Leader_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Leader_ja, definitionRow, DisplayXls); break;
                    case "Manager": Display.Manager = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Manager, definitionRow, DisplayXls); break;
                    case "Manager_ja": Display.Manager_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Manager_ja, definitionRow, DisplayXls); break;
                    case "DeletePermission": Display.DeletePermission = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.DeletePermission, definitionRow, DisplayXls); break;
                    case "DeletePermission_ja": Display.DeletePermission_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.DeletePermission_ja, definitionRow, DisplayXls); break;
                    case "AddPermission": Display.AddPermission = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.AddPermission, definitionRow, DisplayXls); break;
                    case "AddPermission_ja": Display.AddPermission_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.AddPermission_ja, definitionRow, DisplayXls); break;
                    case "NotInheritPermission": Display.NotInheritPermission = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.NotInheritPermission, definitionRow, DisplayXls); break;
                    case "NotInheritPermission_ja": Display.NotInheritPermission_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.NotInheritPermission_ja, definitionRow, DisplayXls); break;
                    case "NoTitle": Display.NoTitle = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.NoTitle, definitionRow, DisplayXls); break;
                    case "NoTitle_ja": Display.NoTitle_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.NoTitle_ja, definitionRow, DisplayXls); break;
                    case "Error": Display.Error = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Error, definitionRow, DisplayXls); break;
                    case "Error_ja": Display.Error_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Error_ja, definitionRow, DisplayXls); break;
                    case "Index": Display.Index = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Index, definitionRow, DisplayXls); break;
                    case "Index_ja": Display.Index_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Index_ja, definitionRow, DisplayXls); break;
                    case "Edit": Display.Edit = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Edit, definitionRow, DisplayXls); break;
                    case "Edit_ja": Display.Edit_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Edit_ja, definitionRow, DisplayXls); break;
                    case "History": Display.History = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.History, definitionRow, DisplayXls); break;
                    case "History_ja": Display.History_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.History_ja, definitionRow, DisplayXls); break;
                    case "Histories": Display.Histories = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Histories, definitionRow, DisplayXls); break;
                    case "Histories_ja": Display.Histories_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Histories_ja, definitionRow, DisplayXls); break;
                    case "Links": Display.Links = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Links, definitionRow, DisplayXls); break;
                    case "Links_ja": Display.Links_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Links_ja, definitionRow, DisplayXls); break;
                    case "LinkDestinations": Display.LinkDestinations = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LinkDestinations, definitionRow, DisplayXls); break;
                    case "LinkDestinations_ja": Display.LinkDestinations_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LinkDestinations_ja, definitionRow, DisplayXls); break;
                    case "LinkSources": Display.LinkSources = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LinkSources, definitionRow, DisplayXls); break;
                    case "LinkSources_ja": Display.LinkSources_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LinkSources_ja, definitionRow, DisplayXls); break;
                    case "LinkCreations": Display.LinkCreations = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LinkCreations, definitionRow, DisplayXls); break;
                    case "LinkCreations_ja": Display.LinkCreations_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LinkCreations_ja, definitionRow, DisplayXls); break;
                    case "Comments": Display.Comments = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Comments, definitionRow, DisplayXls); break;
                    case "Comments_ja": Display.Comments_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Comments_ja, definitionRow, DisplayXls); break;
                    case "SentMail": Display.SentMail = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SentMail, definitionRow, DisplayXls); break;
                    case "SentMail_ja": Display.SentMail_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SentMail_ja, definitionRow, DisplayXls); break;
                    case "Reply": Display.Reply = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Reply, definitionRow, DisplayXls); break;
                    case "Reply_ja": Display.Reply_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Reply_ja, definitionRow, DisplayXls); break;
                    case "OriginalMessage": Display.OriginalMessage = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.OriginalMessage, definitionRow, DisplayXls); break;
                    case "Logout": Display.Logout = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Logout, definitionRow, DisplayXls); break;
                    case "Logout_ja": Display.Logout_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Logout_ja, definitionRow, DisplayXls); break;
                    case "EditProfile": Display.EditProfile = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.EditProfile, definitionRow, DisplayXls); break;
                    case "EditProfile_ja": Display.EditProfile_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.EditProfile_ja, definitionRow, DisplayXls); break;
                    case "CreatedTime": Display.CreatedTime = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.CreatedTime, definitionRow, DisplayXls); break;
                    case "CreatedTime_ja": Display.CreatedTime_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.CreatedTime_ja, definitionRow, DisplayXls); break;
                    case "UpdatedTime": Display.UpdatedTime = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.UpdatedTime, definitionRow, DisplayXls); break;
                    case "UpdatedTime_ja": Display.UpdatedTime_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.UpdatedTime_ja, definitionRow, DisplayXls); break;
                    case "PermissionDestination": Display.PermissionDestination = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.PermissionDestination, definitionRow, DisplayXls); break;
                    case "PermissionDestination_ja": Display.PermissionDestination_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.PermissionDestination_ja, definitionRow, DisplayXls); break;
                    case "PermissionSource": Display.PermissionSource = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.PermissionSource, definitionRow, DisplayXls); break;
                    case "PermissionSource_ja": Display.PermissionSource_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.PermissionSource_ja, definitionRow, DisplayXls); break;
                    case "SettingColumnList": Display.SettingColumnList = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingColumnList, definitionRow, DisplayXls); break;
                    case "SettingColumnList_ja": Display.SettingColumnList_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingColumnList_ja, definitionRow, DisplayXls); break;
                    case "SettingTitleColumn": Display.SettingTitleColumn = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingTitleColumn, definitionRow, DisplayXls); break;
                    case "SettingTitleColumn_ja": Display.SettingTitleColumn_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingTitleColumn_ja, definitionRow, DisplayXls); break;
                    case "SettingTitleSeparator": Display.SettingTitleSeparator = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingTitleSeparator, definitionRow, DisplayXls); break;
                    case "SettingTitleSeparator_ja": Display.SettingTitleSeparator_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingTitleSeparator_ja, definitionRow, DisplayXls); break;
                    case "SettingAggregationList": Display.SettingAggregationList = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingAggregationList, definitionRow, DisplayXls); break;
                    case "SettingAggregationList_ja": Display.SettingAggregationList_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingAggregationList_ja, definitionRow, DisplayXls); break;
                    case "SettingNotGroupBy": Display.SettingNotGroupBy = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingNotGroupBy, definitionRow, DisplayXls); break;
                    case "SettingNotGroupBy_ja": Display.SettingNotGroupBy_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingNotGroupBy_ja, definitionRow, DisplayXls); break;
                    case "SettingNearCompletionTimeBeforeDays": Display.SettingNearCompletionTimeBeforeDays = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingNearCompletionTimeBeforeDays, definitionRow, DisplayXls); break;
                    case "SettingNearCompletionTimeBeforeDays_ja": Display.SettingNearCompletionTimeBeforeDays_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingNearCompletionTimeBeforeDays_ja, definitionRow, DisplayXls); break;
                    case "SettingNearCompletionTimeAfterDays": Display.SettingNearCompletionTimeAfterDays = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingNearCompletionTimeAfterDays, definitionRow, DisplayXls); break;
                    case "SettingNearCompletionTimeAfterDays_ja": Display.SettingNearCompletionTimeAfterDays_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingNearCompletionTimeAfterDays_ja, definitionRow, DisplayXls); break;
                    case "SettingGridPageSize": Display.SettingGridPageSize = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingGridPageSize, definitionRow, DisplayXls); break;
                    case "SettingGridPageSize_ja": Display.SettingGridPageSize_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingGridPageSize_ja, definitionRow, DisplayXls); break;
                    case "SettingLabel": Display.SettingLabel = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingLabel, definitionRow, DisplayXls); break;
                    case "SettingLabel_ja": Display.SettingLabel_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingLabel_ja, definitionRow, DisplayXls); break;
                    case "SettingEnable": Display.SettingEnable = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingEnable, definitionRow, DisplayXls); break;
                    case "SettingEnable_ja": Display.SettingEnable_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingEnable_ja, definitionRow, DisplayXls); break;
                    case "SettingOrder": Display.SettingOrder = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingOrder, definitionRow, DisplayXls); break;
                    case "SettingOrder_ja": Display.SettingOrder_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingOrder_ja, definitionRow, DisplayXls); break;
                    case "SettingUnit": Display.SettingUnit = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingUnit, definitionRow, DisplayXls); break;
                    case "SettingUnit_ja": Display.SettingUnit_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingUnit_ja, definitionRow, DisplayXls); break;
                    case "SettingControlDateTime": Display.SettingControlDateTime = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingControlDateTime, definitionRow, DisplayXls); break;
                    case "SettingControlDateTime_ja": Display.SettingControlDateTime_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingControlDateTime_ja, definitionRow, DisplayXls); break;
                    case "SettingGridDateTime": Display.SettingGridDateTime = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingGridDateTime, definitionRow, DisplayXls); break;
                    case "SettingGridDateTime_ja": Display.SettingGridDateTime_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingGridDateTime_ja, definitionRow, DisplayXls); break;
                    case "SettingSelectionList": Display.SettingSelectionList = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingSelectionList, definitionRow, DisplayXls); break;
                    case "SettingSelectionList_ja": Display.SettingSelectionList_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingSelectionList_ja, definitionRow, DisplayXls); break;
                    case "SettingChoicesVisible": Display.SettingChoicesVisible = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingChoicesVisible, definitionRow, DisplayXls); break;
                    case "SettingChoicesVisible_ja": Display.SettingChoicesVisible_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingChoicesVisible_ja, definitionRow, DisplayXls); break;
                    case "SettingLimitDefault": Display.SettingLimitDefault = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingLimitDefault, definitionRow, DisplayXls); break;
                    case "SettingLimitDefault_ja": Display.SettingLimitDefault_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingLimitDefault_ja, definitionRow, DisplayXls); break;
                    case "SettingGridColumns": Display.SettingGridColumns = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingGridColumns, definitionRow, DisplayXls); break;
                    case "SettingGridColumns_ja": Display.SettingGridColumns_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingGridColumns_ja, definitionRow, DisplayXls); break;
                    case "SettingFilterColumns": Display.SettingFilterColumns = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingFilterColumns, definitionRow, DisplayXls); break;
                    case "SettingFilterColumns_ja": Display.SettingFilterColumns_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingFilterColumns_ja, definitionRow, DisplayXls); break;
                    case "SettingSummaryColumns": Display.SettingSummaryColumns = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingSummaryColumns, definitionRow, DisplayXls); break;
                    case "SettingSummaryColumns_ja": Display.SettingSummaryColumns_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingSummaryColumns_ja, definitionRow, DisplayXls); break;
                    case "SettingEditorColumns": Display.SettingEditorColumns = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingEditorColumns, definitionRow, DisplayXls); break;
                    case "SettingEditorColumns_ja": Display.SettingEditorColumns_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingEditorColumns_ja, definitionRow, DisplayXls); break;
                    case "SettingLinkColumns": Display.SettingLinkColumns = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingLinkColumns, definitionRow, DisplayXls); break;
                    case "SettingLinkColumns_ja": Display.SettingLinkColumns_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingLinkColumns_ja, definitionRow, DisplayXls); break;
                    case "SettingHistoryColumns": Display.SettingHistoryColumns = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingHistoryColumns, definitionRow, DisplayXls); break;
                    case "SettingHistoryColumns_ja": Display.SettingHistoryColumns_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingHistoryColumns_ja, definitionRow, DisplayXls); break;
                    case "SettingFormulas": Display.SettingFormulas = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingFormulas, definitionRow, DisplayXls); break;
                    case "SettingFormulas_ja": Display.SettingFormulas_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingFormulas_ja, definitionRow, DisplayXls); break;
                    case "SettingAggregations": Display.SettingAggregations = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingAggregations, definitionRow, DisplayXls); break;
                    case "SettingAggregations_ja": Display.SettingAggregations_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingAggregations_ja, definitionRow, DisplayXls); break;
                    case "SettingAggregationType": Display.SettingAggregationType = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingAggregationType, definitionRow, DisplayXls); break;
                    case "SettingAggregationType_ja": Display.SettingAggregationType_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingAggregationType_ja, definitionRow, DisplayXls); break;
                    case "SettingAggregationTarget": Display.SettingAggregationTarget = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingAggregationTarget, definitionRow, DisplayXls); break;
                    case "SettingAggregationTarget_ja": Display.SettingAggregationTarget_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SettingAggregationTarget_ja, definitionRow, DisplayXls); break;
                    case "CurrentPassword": Display.CurrentPassword = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.CurrentPassword, definitionRow, DisplayXls); break;
                    case "CurrentPassword_ja": Display.CurrentPassword_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.CurrentPassword_ja, definitionRow, DisplayXls); break;
                    case "ReEnter": Display.ReEnter = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.ReEnter, definitionRow, DisplayXls); break;
                    case "ReEnter_ja": Display.ReEnter_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.ReEnter_ja, definitionRow, DisplayXls); break;
                    case "VerUp": Display.VerUp = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.VerUp, definitionRow, DisplayXls); break;
                    case "VerUp_ja": Display.VerUp_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.VerUp_ja, definitionRow, DisplayXls); break;
                    case "Hidden": Display.Hidden = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Hidden, definitionRow, DisplayXls); break;
                    case "Hidden_ja": Display.Hidden_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Hidden_ja, definitionRow, DisplayXls); break;
                    case "Output": Display.Output = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Output, definitionRow, DisplayXls); break;
                    case "Output_ja": Display.Output_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Output_ja, definitionRow, DisplayXls); break;
                    case "NotOutput": Display.NotOutput = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.NotOutput, definitionRow, DisplayXls); break;
                    case "NotOutput_ja": Display.NotOutput_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.NotOutput_ja, definitionRow, DisplayXls); break;
                    case "CopyWithComments": Display.CopyWithComments = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.CopyWithComments, definitionRow, DisplayXls); break;
                    case "CopyWithComments_ja": Display.CopyWithComments_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.CopyWithComments_ja, definitionRow, DisplayXls); break;
                    case "Hyphen": Display.Hyphen = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Hyphen, definitionRow, DisplayXls); break;
                    case "Search": Display.Search = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Search, definitionRow, DisplayXls); break;
                    case "Search_ja": Display.Search_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Search_ja, definitionRow, DisplayXls); break;
                    case "Admin": Display.Admin = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Admin, definitionRow, DisplayXls); break;
                    case "Admin_ja": Display.Admin_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Admin_ja, definitionRow, DisplayXls); break;
                    case "Default": Display.Default = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Default, definitionRow, DisplayXls); break;
                    case "Default_ja": Display.Default_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Default_ja, definitionRow, DisplayXls); break;
                    case "DefaultInput": Display.DefaultInput = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.DefaultInput, definitionRow, DisplayXls); break;
                    case "DefaultInput_ja": Display.DefaultInput_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.DefaultInput_ja, definitionRow, DisplayXls); break;
                    case "AddressBook": Display.AddressBook = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.AddressBook, definitionRow, DisplayXls); break;
                    case "AddressBook_ja": Display.AddressBook_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.AddressBook_ja, definitionRow, DisplayXls); break;
                    case "DefaultAddressBook": Display.DefaultAddressBook = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.DefaultAddressBook, definitionRow, DisplayXls); break;
                    case "DefaultAddressBook_ja": Display.DefaultAddressBook_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.DefaultAddressBook_ja, definitionRow, DisplayXls); break;
                    case "DefaultDestinations": Display.DefaultDestinations = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.DefaultDestinations, definitionRow, DisplayXls); break;
                    case "DefaultDestinations_ja": Display.DefaultDestinations_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.DefaultDestinations_ja, definitionRow, DisplayXls); break;
                    case "LimitJust": Display.LimitJust = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitJust, definitionRow, DisplayXls); break;
                    case "LimitJust_ja": Display.LimitJust_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitJust_ja, definitionRow, DisplayXls); break;
                    case "LimitAfterSecond": Display.LimitAfterSecond = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitAfterSecond, definitionRow, DisplayXls); break;
                    case "LimitAfterSecond_ja": Display.LimitAfterSecond_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitAfterSecond_ja, definitionRow, DisplayXls); break;
                    case "LimitAfterSeconds": Display.LimitAfterSeconds = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitAfterSeconds, definitionRow, DisplayXls); break;
                    case "LimitAfterSeconds_ja": Display.LimitAfterSeconds_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitAfterSeconds_ja, definitionRow, DisplayXls); break;
                    case "LimitAfterMinute": Display.LimitAfterMinute = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitAfterMinute, definitionRow, DisplayXls); break;
                    case "LimitAfterMinute_ja": Display.LimitAfterMinute_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitAfterMinute_ja, definitionRow, DisplayXls); break;
                    case "LimitAfterMinutes": Display.LimitAfterMinutes = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitAfterMinutes, definitionRow, DisplayXls); break;
                    case "LimitAfterMinutes_ja": Display.LimitAfterMinutes_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitAfterMinutes_ja, definitionRow, DisplayXls); break;
                    case "LimitAfterHour": Display.LimitAfterHour = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitAfterHour, definitionRow, DisplayXls); break;
                    case "LimitAfterHour_ja": Display.LimitAfterHour_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitAfterHour_ja, definitionRow, DisplayXls); break;
                    case "LimitAfterHours": Display.LimitAfterHours = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitAfterHours, definitionRow, DisplayXls); break;
                    case "LimitAfterHours_ja": Display.LimitAfterHours_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitAfterHours_ja, definitionRow, DisplayXls); break;
                    case "LimitAfterDay": Display.LimitAfterDay = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitAfterDay, definitionRow, DisplayXls); break;
                    case "LimitAfterDay_ja": Display.LimitAfterDay_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitAfterDay_ja, definitionRow, DisplayXls); break;
                    case "LimitAfterDays": Display.LimitAfterDays = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitAfterDays, definitionRow, DisplayXls); break;
                    case "LimitAfterDays_ja": Display.LimitAfterDays_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitAfterDays_ja, definitionRow, DisplayXls); break;
                    case "LimitAfterMonth": Display.LimitAfterMonth = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitAfterMonth, definitionRow, DisplayXls); break;
                    case "LimitAfterMonth_ja": Display.LimitAfterMonth_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitAfterMonth_ja, definitionRow, DisplayXls); break;
                    case "LimitAfterMonths": Display.LimitAfterMonths = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitAfterMonths, definitionRow, DisplayXls); break;
                    case "LimitAfterMonths_ja": Display.LimitAfterMonths_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitAfterMonths_ja, definitionRow, DisplayXls); break;
                    case "LimitAfterYear": Display.LimitAfterYear = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitAfterYear, definitionRow, DisplayXls); break;
                    case "LimitAfterYear_ja": Display.LimitAfterYear_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitAfterYear_ja, definitionRow, DisplayXls); break;
                    case "LimitAfterYears": Display.LimitAfterYears = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitAfterYears, definitionRow, DisplayXls); break;
                    case "LimitAfterYears_ja": Display.LimitAfterYears_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitAfterYears_ja, definitionRow, DisplayXls); break;
                    case "LimitBeforeSecond": Display.LimitBeforeSecond = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitBeforeSecond, definitionRow, DisplayXls); break;
                    case "LimitBeforeSecond_ja": Display.LimitBeforeSecond_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitBeforeSecond_ja, definitionRow, DisplayXls); break;
                    case "LimitBeforeSeconds": Display.LimitBeforeSeconds = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitBeforeSeconds, definitionRow, DisplayXls); break;
                    case "LimitBeforeSeconds_ja": Display.LimitBeforeSeconds_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitBeforeSeconds_ja, definitionRow, DisplayXls); break;
                    case "LimitBeforeMinute": Display.LimitBeforeMinute = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitBeforeMinute, definitionRow, DisplayXls); break;
                    case "LimitBeforeMinute_ja": Display.LimitBeforeMinute_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitBeforeMinute_ja, definitionRow, DisplayXls); break;
                    case "LimitBeforeMinutes": Display.LimitBeforeMinutes = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitBeforeMinutes, definitionRow, DisplayXls); break;
                    case "LimitBeforeMinutes_ja": Display.LimitBeforeMinutes_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitBeforeMinutes_ja, definitionRow, DisplayXls); break;
                    case "LimitBeforeHour": Display.LimitBeforeHour = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitBeforeHour, definitionRow, DisplayXls); break;
                    case "LimitBeforeHour_ja": Display.LimitBeforeHour_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitBeforeHour_ja, definitionRow, DisplayXls); break;
                    case "LimitBeforeHours": Display.LimitBeforeHours = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitBeforeHours, definitionRow, DisplayXls); break;
                    case "LimitBeforeHours_ja": Display.LimitBeforeHours_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitBeforeHours_ja, definitionRow, DisplayXls); break;
                    case "LimitBeforeDay": Display.LimitBeforeDay = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitBeforeDay, definitionRow, DisplayXls); break;
                    case "LimitBeforeDay_ja": Display.LimitBeforeDay_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitBeforeDay_ja, definitionRow, DisplayXls); break;
                    case "LimitBeforeDays": Display.LimitBeforeDays = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitBeforeDays, definitionRow, DisplayXls); break;
                    case "LimitBeforeDays_ja": Display.LimitBeforeDays_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitBeforeDays_ja, definitionRow, DisplayXls); break;
                    case "LimitBeforeMonth": Display.LimitBeforeMonth = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitBeforeMonth, definitionRow, DisplayXls); break;
                    case "LimitBeforeMonth_ja": Display.LimitBeforeMonth_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitBeforeMonth_ja, definitionRow, DisplayXls); break;
                    case "LimitBeforeMonths": Display.LimitBeforeMonths = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitBeforeMonths, definitionRow, DisplayXls); break;
                    case "LimitBeforeMonths_ja": Display.LimitBeforeMonths_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitBeforeMonths_ja, definitionRow, DisplayXls); break;
                    case "LimitBeforeYear": Display.LimitBeforeYear = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitBeforeYear, definitionRow, DisplayXls); break;
                    case "LimitBeforeYear_ja": Display.LimitBeforeYear_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitBeforeYear_ja, definitionRow, DisplayXls); break;
                    case "LimitBeforeYears": Display.LimitBeforeYears = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitBeforeYears, definitionRow, DisplayXls); break;
                    case "LimitBeforeYears_ja": Display.LimitBeforeYears_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LimitBeforeYears_ja, definitionRow, DisplayXls); break;
                    case "WriteComment": Display.WriteComment = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.WriteComment, definitionRow, DisplayXls); break;
                    case "WriteComment_ja": Display.WriteComment_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.WriteComment_ja, definitionRow, DisplayXls); break;
                    case "Filters": Display.Filters = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Filters, definitionRow, DisplayXls); break;
                    case "Filters_ja": Display.Filters_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Filters_ja, definitionRow, DisplayXls); break;
                    case "Incomplete": Display.Incomplete = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Incomplete, definitionRow, DisplayXls); break;
                    case "Incomplete_ja": Display.Incomplete_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Incomplete_ja, definitionRow, DisplayXls); break;
                    case "Blank": Display.Blank = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Blank, definitionRow, DisplayXls); break;
                    case "Blank_ja": Display.Blank_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Blank_ja, definitionRow, DisplayXls); break;
                    case "Own": Display.Own = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Own, definitionRow, DisplayXls); break;
                    case "Own_ja": Display.Own_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Own_ja, definitionRow, DisplayXls); break;
                    case "NearCompletionTime": Display.NearCompletionTime = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.NearCompletionTime, definitionRow, DisplayXls); break;
                    case "NearCompletionTime_ja": Display.NearCompletionTime_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.NearCompletionTime_ja, definitionRow, DisplayXls); break;
                    case "Delay": Display.Delay = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Delay, definitionRow, DisplayXls); break;
                    case "Delay_ja": Display.Delay_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Delay_ja, definitionRow, DisplayXls); break;
                    case "Overdue": Display.Overdue = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Overdue, definitionRow, DisplayXls); break;
                    case "Overdue_ja": Display.Overdue_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Overdue_ja, definitionRow, DisplayXls); break;
                    case "OrderAsc": Display.OrderAsc = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.OrderAsc, definitionRow, DisplayXls); break;
                    case "OrderAsc_ja": Display.OrderAsc_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.OrderAsc_ja, definitionRow, DisplayXls); break;
                    case "OrderDesc": Display.OrderDesc = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.OrderDesc, definitionRow, DisplayXls); break;
                    case "OrderDesc_ja": Display.OrderDesc_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.OrderDesc_ja, definitionRow, DisplayXls); break;
                    case "OrderRelease": Display.OrderRelease = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.OrderRelease, definitionRow, DisplayXls); break;
                    case "OrderRelease_ja": Display.OrderRelease_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.OrderRelease_ja, definitionRow, DisplayXls); break;
                    case "ResetOrder": Display.ResetOrder = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.ResetOrder, definitionRow, DisplayXls); break;
                    case "ResetOrder_ja": Display.ResetOrder_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.ResetOrder_ja, definitionRow, DisplayXls); break;
                    case "Csv": Display.Csv = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Csv, definitionRow, DisplayXls); break;
                    case "Csv_ja": Display.Csv_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Csv_ja, definitionRow, DisplayXls); break;
                    case "CsvFile": Display.CsvFile = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.CsvFile, definitionRow, DisplayXls); break;
                    case "CsvFile_ja": Display.CsvFile_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.CsvFile_ja, definitionRow, DisplayXls); break;
                    case "CharacterCode": Display.CharacterCode = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.CharacterCode, definitionRow, DisplayXls); break;
                    case "CharacterCode_ja": Display.CharacterCode_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.CharacterCode_ja, definitionRow, DisplayXls); break;
                    case "Excel": Display.Excel = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Excel, definitionRow, DisplayXls); break;
                    case "Excel_ja": Display.Excel_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Excel_ja, definitionRow, DisplayXls); break;
                    case "Setting": Display.Setting = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Setting, definitionRow, DisplayXls); break;
                    case "Setting_ja": Display.Setting_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Setting_ja, definitionRow, DisplayXls); break;
                    case "AdvancedSetting": Display.AdvancedSetting = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.AdvancedSetting, definitionRow, DisplayXls); break;
                    case "AdvancedSetting_ja": Display.AdvancedSetting_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.AdvancedSetting_ja, definitionRow, DisplayXls); break;
                    case "Basic": Display.Basic = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Basic, definitionRow, DisplayXls); break;
                    case "Basic_ja": Display.Basic_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Basic_ja, definitionRow, DisplayXls); break;
                    case "Normal": Display.Normal = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Normal, definitionRow, DisplayXls); break;
                    case "Normal_ja": Display.Normal_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Normal_ja, definitionRow, DisplayXls); break;
                    case "Wide": Display.Wide = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Wide, definitionRow, DisplayXls); break;
                    case "Wide_ja": Display.Wide_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Wide_ja, definitionRow, DisplayXls); break;
                    case "Auto": Display.Auto = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Auto, definitionRow, DisplayXls); break;
                    case "Auto_ja": Display.Auto_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Auto_ja, definitionRow, DisplayXls); break;
                    case "ControlType": Display.ControlType = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.ControlType, definitionRow, DisplayXls); break;
                    case "ControlType_ja": Display.ControlType_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.ControlType_ja, definitionRow, DisplayXls); break;
                    case "Spinner": Display.Spinner = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Spinner, definitionRow, DisplayXls); break;
                    case "Spinner_ja": Display.Spinner_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Spinner_ja, definitionRow, DisplayXls); break;
                    case "SiteImageSettingsEditor": Display.SiteImageSettingsEditor = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SiteImageSettingsEditor, definitionRow, DisplayXls); break;
                    case "SiteImageSettingsEditor_ja": Display.SiteImageSettingsEditor_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SiteImageSettingsEditor_ja, definitionRow, DisplayXls); break;
                    case "GridSettingsEditor": Display.GridSettingsEditor = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.GridSettingsEditor, definitionRow, DisplayXls); break;
                    case "GridSettingsEditor_ja": Display.GridSettingsEditor_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.GridSettingsEditor_ja, definitionRow, DisplayXls); break;
                    case "EditorSettingsEditor": Display.EditorSettingsEditor = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.EditorSettingsEditor, definitionRow, DisplayXls); break;
                    case "EditorSettingsEditor_ja": Display.EditorSettingsEditor_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.EditorSettingsEditor_ja, definitionRow, DisplayXls); break;
                    case "MailerSettingsEditor": Display.MailerSettingsEditor = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.MailerSettingsEditor, definitionRow, DisplayXls); break;
                    case "MailerSettingsEditor_ja": Display.MailerSettingsEditor_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.MailerSettingsEditor_ja, definitionRow, DisplayXls); break;
                    case "StyleSettingsEditor": Display.StyleSettingsEditor = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.StyleSettingsEditor, definitionRow, DisplayXls); break;
                    case "StyleSettingsEditor_ja": Display.StyleSettingsEditor_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.StyleSettingsEditor_ja, definitionRow, DisplayXls); break;
                    case "ScriptSettingsEditor": Display.ScriptSettingsEditor = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.ScriptSettingsEditor, definitionRow, DisplayXls); break;
                    case "ScriptSettingsEditor_ja": Display.ScriptSettingsEditor_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.ScriptSettingsEditor_ja, definitionRow, DisplayXls); break;
                    case "SummarySettingsEditor": Display.SummarySettingsEditor = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SummarySettingsEditor, definitionRow, DisplayXls); break;
                    case "SummarySettingsEditor_ja": Display.SummarySettingsEditor_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SummarySettingsEditor_ja, definitionRow, DisplayXls); break;
                    case "PermissionSetting": Display.PermissionSetting = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.PermissionSetting, definitionRow, DisplayXls); break;
                    case "PermissionSetting_ja": Display.PermissionSetting_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.PermissionSetting_ja, definitionRow, DisplayXls); break;
                    case "EditPermissions": Display.EditPermissions = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.EditPermissions, definitionRow, DisplayXls); break;
                    case "EditPermissions_ja": Display.EditPermissions_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.EditPermissions_ja, definitionRow, DisplayXls); break;
                    case "CopySettings": Display.CopySettings = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.CopySettings, definitionRow, DisplayXls); break;
                    case "CopySettings_ja": Display.CopySettings_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.CopySettings_ja, definitionRow, DisplayXls); break;
                    case "AggregationDetails": Display.AggregationDetails = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.AggregationDetails, definitionRow, DisplayXls); break;
                    case "AggregationDetails_ja": Display.AggregationDetails_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.AggregationDetails_ja, definitionRow, DisplayXls); break;
                    case "MoveSettings": Display.MoveSettings = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.MoveSettings, definitionRow, DisplayXls); break;
                    case "MoveSettings_ja": Display.MoveSettings_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.MoveSettings_ja, definitionRow, DisplayXls); break;
                    case "Destination": Display.Destination = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Destination, definitionRow, DisplayXls); break;
                    case "Destination_ja": Display.Destination_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Destination_ja, definitionRow, DisplayXls); break;
                    case "SeparateSettings": Display.SeparateSettings = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SeparateSettings, definitionRow, DisplayXls); break;
                    case "SeparateSettings_ja": Display.SeparateSettings_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SeparateSettings_ja, definitionRow, DisplayXls); break;
                    case "SummarySiteId": Display.SummarySiteId = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SummarySiteId, definitionRow, DisplayXls); break;
                    case "SummarySiteId_ja": Display.SummarySiteId_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SummarySiteId_ja, definitionRow, DisplayXls); break;
                    case "SummaryDestinationColumn": Display.SummaryDestinationColumn = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SummaryDestinationColumn, definitionRow, DisplayXls); break;
                    case "SummaryDestinationColumn_ja": Display.SummaryDestinationColumn_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SummaryDestinationColumn_ja, definitionRow, DisplayXls); break;
                    case "SummaryLinkColumn": Display.SummaryLinkColumn = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SummaryLinkColumn, definitionRow, DisplayXls); break;
                    case "SummaryLinkColumn_ja": Display.SummaryLinkColumn_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SummaryLinkColumn_ja, definitionRow, DisplayXls); break;
                    case "SummaryType": Display.SummaryType = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SummaryType, definitionRow, DisplayXls); break;
                    case "SummaryType_ja": Display.SummaryType_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SummaryType_ja, definitionRow, DisplayXls); break;
                    case "SummarySourceColumn": Display.SummarySourceColumn = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SummarySourceColumn, definitionRow, DisplayXls); break;
                    case "SummarySourceColumn_ja": Display.SummarySourceColumn_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SummarySourceColumn_ja, definitionRow, DisplayXls); break;
                    case "Title": Display.Title = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Title, definitionRow, DisplayXls); break;
                    case "Title_ja": Display.Title_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Title_ja, definitionRow, DisplayXls); break;
                    case "WorkValue": Display.WorkValue = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.WorkValue, definitionRow, DisplayXls); break;
                    case "WorkValue_ja": Display.WorkValue_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.WorkValue_ja, definitionRow, DisplayXls); break;
                    case "SeparateNumber": Display.SeparateNumber = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SeparateNumber, definitionRow, DisplayXls); break;
                    case "SeparateNumber_ja": Display.SeparateNumber_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SeparateNumber_ja, definitionRow, DisplayXls); break;
                    case "OutgoingMail": Display.OutgoingMail = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.OutgoingMail, definitionRow, DisplayXls); break;
                    case "OutgoingMail_ja": Display.OutgoingMail_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.OutgoingMail_ja, definitionRow, DisplayXls); break;
                    case "Icon": Display.Icon = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Icon, definitionRow, DisplayXls); break;
                    case "Icon_ja": Display.Icon_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Icon_ja, definitionRow, DisplayXls); break;
                    case "File": Display.File = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.File, definitionRow, DisplayXls); break;
                    case "File_ja": Display.File_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.File_ja, definitionRow, DisplayXls); break;
                    case "NotSet": Display.NotSet = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.NotSet, definitionRow, DisplayXls); break;
                    case "NotSet_ja": Display.NotSet_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.NotSet_ja, definitionRow, DisplayXls); break;
                    case "SiteUser": Display.SiteUser = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SiteUser, definitionRow, DisplayXls); break;
                    case "SiteUser_ja": Display.SiteUser_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SiteUser_ja, definitionRow, DisplayXls); break;
                    case "All": Display.All = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.All, definitionRow, DisplayXls); break;
                    case "All_ja": Display.All_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.All_ja, definitionRow, DisplayXls); break;
                    case "Quantity": Display.Quantity = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Quantity, definitionRow, DisplayXls); break;
                    case "Quantity_ja": Display.Quantity_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Quantity_ja, definitionRow, DisplayXls); break;
                    case "SuffixCopy": Display.SuffixCopy = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SuffixCopy, definitionRow, DisplayXls); break;
                    case "SuffixCopy_ja": Display.SuffixCopy_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SuffixCopy_ja, definitionRow, DisplayXls); break;
                    case "DataViewSelector": Display.DataViewSelector = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.DataViewSelector, definitionRow, DisplayXls); break;
                    case "DataViewSelector_ja": Display.DataViewSelector_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.DataViewSelector_ja, definitionRow, DisplayXls); break;
                    case "Grid": Display.Grid = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Grid, definitionRow, DisplayXls); break;
                    case "Grid_ja": Display.Grid_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Grid_ja, definitionRow, DisplayXls); break;
                    case "Style": Display.Style = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Style, definitionRow, DisplayXls); break;
                    case "Style_ja": Display.Style_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Style_ja, definitionRow, DisplayXls); break;
                    case "GridStyle": Display.GridStyle = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.GridStyle, definitionRow, DisplayXls); break;
                    case "GridStyle_ja": Display.GridStyle_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.GridStyle_ja, definitionRow, DisplayXls); break;
                    case "NewStyle": Display.NewStyle = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.NewStyle, definitionRow, DisplayXls); break;
                    case "NewStyle_ja": Display.NewStyle_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.NewStyle_ja, definitionRow, DisplayXls); break;
                    case "EditStyle": Display.EditStyle = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.EditStyle, definitionRow, DisplayXls); break;
                    case "EditStyle_ja": Display.EditStyle_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.EditStyle_ja, definitionRow, DisplayXls); break;
                    case "Script": Display.Script = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Script, definitionRow, DisplayXls); break;
                    case "Script_ja": Display.Script_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Script_ja, definitionRow, DisplayXls); break;
                    case "GridScript": Display.GridScript = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.GridScript, definitionRow, DisplayXls); break;
                    case "GridScript_ja": Display.GridScript_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.GridScript_ja, definitionRow, DisplayXls); break;
                    case "NewScript": Display.NewScript = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.NewScript, definitionRow, DisplayXls); break;
                    case "NewScript_ja": Display.NewScript_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.NewScript_ja, definitionRow, DisplayXls); break;
                    case "EditScript": Display.EditScript = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.EditScript, definitionRow, DisplayXls); break;
                    case "EditScript_ja": Display.EditScript_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.EditScript_ja, definitionRow, DisplayXls); break;
                    case "BurnDown": Display.BurnDown = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.BurnDown, definitionRow, DisplayXls); break;
                    case "BurnDown_ja": Display.BurnDown_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.BurnDown_ja, definitionRow, DisplayXls); break;
                    case "Gantt": Display.Gantt = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Gantt, definitionRow, DisplayXls); break;
                    case "Gantt_ja": Display.Gantt_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Gantt_ja, definitionRow, DisplayXls); break;
                    case "TimeSeries": Display.TimeSeries = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.TimeSeries, definitionRow, DisplayXls); break;
                    case "TimeSeries_ja": Display.TimeSeries_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.TimeSeries_ja, definitionRow, DisplayXls); break;
                    case "Kamban": Display.Kamban = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Kamban, definitionRow, DisplayXls); break;
                    case "Kamban_ja": Display.Kamban_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Kamban_ja, definitionRow, DisplayXls); break;
                    case "NoData": Display.NoData = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.NoData, definitionRow, DisplayXls); break;
                    case "NoData_ja": Display.NoData_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.NoData_ja, definitionRow, DisplayXls); break;
                    case "NoLinks": Display.NoLinks = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.NoLinks, definitionRow, DisplayXls); break;
                    case "NoLinks_ja": Display.NoLinks_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.NoLinks_ja, definitionRow, DisplayXls); break;
                    case "ViewDemoEnvironment": Display.ViewDemoEnvironment = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.ViewDemoEnvironment, definitionRow, DisplayXls); break;
                    case "ViewDemoEnvironment_ja": Display.ViewDemoEnvironment_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.ViewDemoEnvironment_ja, definitionRow, DisplayXls); break;
                    case "DemoMailTitle": Display.DemoMailTitle = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.DemoMailTitle, definitionRow, DisplayXls); break;
                    case "DemoMailTitle_ja": Display.DemoMailTitle_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.DemoMailTitle_ja, definitionRow, DisplayXls); break;
                    case "DemoMailBody": Display.DemoMailBody = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.DemoMailBody, definitionRow, DisplayXls); break;
                    case "DemoMailBody_ja": Display.DemoMailBody_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.DemoMailBody_ja, definitionRow, DisplayXls); break;
                    case "Fy": Display.Fy = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Fy, definitionRow, DisplayXls); break;
                    case "Fy_ja": Display.Fy_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Fy_ja, definitionRow, DisplayXls); break;
                    case "Half1": Display.Half1 = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Half1, definitionRow, DisplayXls); break;
                    case "Half1_ja": Display.Half1_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Half1_ja, definitionRow, DisplayXls); break;
                    case "Half2": Display.Half2 = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Half2, definitionRow, DisplayXls); break;
                    case "Half2_ja": Display.Half2_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Half2_ja, definitionRow, DisplayXls); break;
                    case "Quarter": Display.Quarter = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Quarter, definitionRow, DisplayXls); break;
                    case "Quarter_ja": Display.Quarter_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Quarter_ja, definitionRow, DisplayXls); break;
                    case "Ymd": Display.Ymd = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Ymd, definitionRow, DisplayXls); break;
                    case "Ymd_ja": Display.Ymd_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Ymd_ja, definitionRow, DisplayXls); break;
                    case "Ymda": Display.Ymda = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Ymda, definitionRow, DisplayXls); break;
                    case "Ymda_ja": Display.Ymda_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Ymda_ja, definitionRow, DisplayXls); break;
                    case "Ym": Display.Ym = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Ym, definitionRow, DisplayXls); break;
                    case "Ym_ja": Display.Ym_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Ym_ja, definitionRow, DisplayXls); break;
                    case "Ymdhms": Display.Ymdhms = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Ymdhms, definitionRow, DisplayXls); break;
                    case "Ymdhms_ja": Display.Ymdhms_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Ymdhms_ja, definitionRow, DisplayXls); break;
                    case "Ymdahms": Display.Ymdahms = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Ymdahms, definitionRow, DisplayXls); break;
                    case "Ymdahms_ja": Display.Ymdahms_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Ymdahms_ja, definitionRow, DisplayXls); break;
                    case "Ymdhm": Display.Ymdhm = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Ymdhm, definitionRow, DisplayXls); break;
                    case "Ymdhm_ja": Display.Ymdhm_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Ymdhm_ja, definitionRow, DisplayXls); break;
                    case "Ymdahm": Display.Ymdahm = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Ymdahm, definitionRow, DisplayXls); break;
                    case "Ymdahm_ja": Display.Ymdahm_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Ymdahm_ja, definitionRow, DisplayXls); break;
                    case "YmdFormat": Display.YmdFormat = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.YmdFormat, definitionRow, DisplayXls); break;
                    case "YmdFormat_ja": Display.YmdFormat_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.YmdFormat_ja, definitionRow, DisplayXls); break;
                    case "YmdaFormat": Display.YmdaFormat = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.YmdaFormat, definitionRow, DisplayXls); break;
                    case "YmdaFormat_ja": Display.YmdaFormat_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.YmdaFormat_ja, definitionRow, DisplayXls); break;
                    case "YmFormat": Display.YmFormat = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.YmFormat, definitionRow, DisplayXls); break;
                    case "YmFormat_ja": Display.YmFormat_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.YmFormat_ja, definitionRow, DisplayXls); break;
                    case "YmdhmsFormat": Display.YmdhmsFormat = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.YmdhmsFormat, definitionRow, DisplayXls); break;
                    case "YmdhmsFormat_ja": Display.YmdhmsFormat_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.YmdhmsFormat_ja, definitionRow, DisplayXls); break;
                    case "YmdahmsFormat": Display.YmdahmsFormat = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.YmdahmsFormat, definitionRow, DisplayXls); break;
                    case "YmdahmsFormat_ja": Display.YmdahmsFormat_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.YmdahmsFormat_ja, definitionRow, DisplayXls); break;
                    case "YmdhmFormat": Display.YmdhmFormat = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.YmdhmFormat, definitionRow, DisplayXls); break;
                    case "YmdhmFormat_ja": Display.YmdhmFormat_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.YmdhmFormat_ja, definitionRow, DisplayXls); break;
                    case "YmdahmFormat": Display.YmdahmFormat = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.YmdahmFormat, definitionRow, DisplayXls); break;
                    case "YmdahmFormat_ja": Display.YmdahmFormat_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.YmdahmFormat_ja, definitionRow, DisplayXls); break;
                    case "ExceptionTitle": Display.ExceptionTitle = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.ExceptionTitle, definitionRow, DisplayXls); break;
                    case "ExceptionTitle_ja": Display.ExceptionTitle_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.ExceptionTitle_ja, definitionRow, DisplayXls); break;
                    case "ExceptionBody": Display.ExceptionBody = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.ExceptionBody, definitionRow, DisplayXls); break;
                    case "ExceptionBody_ja": Display.ExceptionBody_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.ExceptionBody_ja, definitionRow, DisplayXls); break;
                    case "InvalidRequest": Display.InvalidRequest = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.InvalidRequest, definitionRow, DisplayXls); break;
                    case "InvalidRequest_ja": Display.InvalidRequest_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.InvalidRequest_ja, definitionRow, DisplayXls); break;
                    case "Authentication": Display.Authentication = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Authentication, definitionRow, DisplayXls); break;
                    case "Authentication_ja": Display.Authentication_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Authentication_ja, definitionRow, DisplayXls); break;
                    case "PasswordNotChanged": Display.PasswordNotChanged = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.PasswordNotChanged, definitionRow, DisplayXls); break;
                    case "PasswordNotChanged_ja": Display.PasswordNotChanged_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.PasswordNotChanged_ja, definitionRow, DisplayXls); break;
                    case "UpdateConflicts": Display.UpdateConflicts = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.UpdateConflicts, definitionRow, DisplayXls); break;
                    case "UpdateConflicts_ja": Display.UpdateConflicts_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.UpdateConflicts_ja, definitionRow, DisplayXls); break;
                    case "DeleteConflicts": Display.DeleteConflicts = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.DeleteConflicts, definitionRow, DisplayXls); break;
                    case "DeleteConflicts_ja": Display.DeleteConflicts_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.DeleteConflicts_ja, definitionRow, DisplayXls); break;
                    case "HasNotPermission": Display.HasNotPermission = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.HasNotPermission, definitionRow, DisplayXls); break;
                    case "HasNotPermission_ja": Display.HasNotPermission_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.HasNotPermission_ja, definitionRow, DisplayXls); break;
                    case "IncorrectCurrentPassword": Display.IncorrectCurrentPassword = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.IncorrectCurrentPassword, definitionRow, DisplayXls); break;
                    case "IncorrectCurrentPassword_ja": Display.IncorrectCurrentPassword_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.IncorrectCurrentPassword_ja, definitionRow, DisplayXls); break;
                    case "PermissionNotSelfChange": Display.PermissionNotSelfChange = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.PermissionNotSelfChange, definitionRow, DisplayXls); break;
                    case "PermissionNotSelfChange_ja": Display.PermissionNotSelfChange_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.PermissionNotSelfChange_ja, definitionRow, DisplayXls); break;
                    case "DefinitionNotFound": Display.DefinitionNotFound = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.DefinitionNotFound, definitionRow, DisplayXls); break;
                    case "DefinitionNotFound_ja": Display.DefinitionNotFound_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.DefinitionNotFound_ja, definitionRow, DisplayXls); break;
                    case "CantSetAtTopOfSite": Display.CantSetAtTopOfSite = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.CantSetAtTopOfSite, definitionRow, DisplayXls); break;
                    case "CantSetAtTopOfSite_ja": Display.CantSetAtTopOfSite_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.CantSetAtTopOfSite_ja, definitionRow, DisplayXls); break;
                    case "NotFound": Display.NotFound = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.NotFound, definitionRow, DisplayXls); break;
                    case "NotFound_ja": Display.NotFound_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.NotFound_ja, definitionRow, DisplayXls); break;
                    case "RequireMailAddresses": Display.RequireMailAddresses = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.RequireMailAddresses, definitionRow, DisplayXls); break;
                    case "RequireMailAddresses_ja": Display.RequireMailAddresses_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.RequireMailAddresses_ja, definitionRow, DisplayXls); break;
                    case "RequireColumn": Display.RequireColumn = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.RequireColumn, definitionRow, DisplayXls); break;
                    case "RequireColumn_ja": Display.RequireColumn_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.RequireColumn_ja, definitionRow, DisplayXls); break;
                    case "ExternalMailAddress": Display.ExternalMailAddress = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.ExternalMailAddress, definitionRow, DisplayXls); break;
                    case "ExternalMailAddress_ja": Display.ExternalMailAddress_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.ExternalMailAddress_ja, definitionRow, DisplayXls); break;
                    case "BadMailAddress": Display.BadMailAddress = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.BadMailAddress, definitionRow, DisplayXls); break;
                    case "BadMailAddress_ja": Display.BadMailAddress_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.BadMailAddress_ja, definitionRow, DisplayXls); break;
                    case "MailAddressHasNotSet": Display.MailAddressHasNotSet = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.MailAddressHasNotSet, definitionRow, DisplayXls); break;
                    case "MailAddressHasNotSet_ja": Display.MailAddressHasNotSet_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.MailAddressHasNotSet_ja, definitionRow, DisplayXls); break;
                    case "FileNotFound": Display.FileNotFound = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.FileNotFound, definitionRow, DisplayXls); break;
                    case "FileNotFound_ja": Display.FileNotFound_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.FileNotFound_ja, definitionRow, DisplayXls); break;
                    case "NotRequiredColumn": Display.NotRequiredColumn = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.NotRequiredColumn, definitionRow, DisplayXls); break;
                    case "NotRequiredColumn_ja": Display.NotRequiredColumn_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.NotRequiredColumn_ja, definitionRow, DisplayXls); break;
                    case "InvalidCsvData": Display.InvalidCsvData = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.InvalidCsvData, definitionRow, DisplayXls); break;
                    case "InvalidCsvData_ja": Display.InvalidCsvData_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.InvalidCsvData_ja, definitionRow, DisplayXls); break;
                    case "FailedReadFile": Display.FailedReadFile = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.FailedReadFile, definitionRow, DisplayXls); break;
                    case "FailedReadFile_ja": Display.FailedReadFile_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.FailedReadFile_ja, definitionRow, DisplayXls); break;
                    case "CanNotHide": Display.CanNotHide = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.CanNotHide, definitionRow, DisplayXls); break;
                    case "CanNotHide_ja": Display.CanNotHide_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.CanNotHide_ja, definitionRow, DisplayXls); break;
                    case "AlreadyAdded": Display.AlreadyAdded = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.AlreadyAdded, definitionRow, DisplayXls); break;
                    case "AlreadyAdded_ja": Display.AlreadyAdded_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.AlreadyAdded_ja, definitionRow, DisplayXls); break;
                    case "InvalidFormula": Display.InvalidFormula = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.InvalidFormula, definitionRow, DisplayXls); break;
                    case "InvalidFormula_ja": Display.InvalidFormula_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.InvalidFormula_ja, definitionRow, DisplayXls); break;
                    case "SelectTargets": Display.SelectTargets = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SelectTargets, definitionRow, DisplayXls); break;
                    case "SelectTargets_ja": Display.SelectTargets_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SelectTargets_ja, definitionRow, DisplayXls); break;
                    case "LoginIn": Display.LoginIn = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LoginIn, definitionRow, DisplayXls); break;
                    case "LoginIn_ja": Display.LoginIn_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.LoginIn_ja, definitionRow, DisplayXls); break;
                    case "Deleted": Display.Deleted = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Deleted, definitionRow, DisplayXls); break;
                    case "Deleted_ja": Display.Deleted_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Deleted_ja, definitionRow, DisplayXls); break;
                    case "BulkMoved": Display.BulkMoved = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.BulkMoved, definitionRow, DisplayXls); break;
                    case "BulkMoved_ja": Display.BulkMoved_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.BulkMoved_ja, definitionRow, DisplayXls); break;
                    case "BulkDeleted": Display.BulkDeleted = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.BulkDeleted, definitionRow, DisplayXls); break;
                    case "BulkDeleted_ja": Display.BulkDeleted_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.BulkDeleted_ja, definitionRow, DisplayXls); break;
                    case "Separated": Display.Separated = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Separated, definitionRow, DisplayXls); break;
                    case "Separated_ja": Display.Separated_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Separated_ja, definitionRow, DisplayXls); break;
                    case "Imported": Display.Imported = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Imported, definitionRow, DisplayXls); break;
                    case "Imported_ja": Display.Imported_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Imported_ja, definitionRow, DisplayXls); break;
                    case "Created": Display.Created = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Created, definitionRow, DisplayXls); break;
                    case "Created_ja": Display.Created_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Created_ja, definitionRow, DisplayXls); break;
                    case "Updated": Display.Updated = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Updated, definitionRow, DisplayXls); break;
                    case "Updated_ja": Display.Updated_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.Updated_ja, definitionRow, DisplayXls); break;
                    case "CodeDefinerCompleted": Display.CodeDefinerCompleted = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.CodeDefinerCompleted, definitionRow, DisplayXls); break;
                    case "CodeDefinerCompleted_ja": Display.CodeDefinerCompleted_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.CodeDefinerCompleted_ja, definitionRow, DisplayXls); break;
                    case "CodeDefinerRdsCompleted": Display.CodeDefinerRdsCompleted = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.CodeDefinerRdsCompleted, definitionRow, DisplayXls); break;
                    case "CodeDefinerRdsCompleted_ja": Display.CodeDefinerRdsCompleted_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.CodeDefinerRdsCompleted_ja, definitionRow, DisplayXls); break;
                    case "CodeDefinerDefCompleted": Display.CodeDefinerDefCompleted = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.CodeDefinerDefCompleted, definitionRow, DisplayXls); break;
                    case "CodeDefinerDefCompleted_ja": Display.CodeDefinerDefCompleted_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.CodeDefinerDefCompleted_ja, definitionRow, DisplayXls); break;
                    case "CodeDefinerMvcCompleted": Display.CodeDefinerMvcCompleted = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.CodeDefinerMvcCompleted, definitionRow, DisplayXls); break;
                    case "CodeDefinerMvcCompleted_ja": Display.CodeDefinerMvcCompleted_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.CodeDefinerMvcCompleted_ja, definitionRow, DisplayXls); break;
                    case "CodeDefinerCssCompleted": Display.CodeDefinerCssCompleted = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.CodeDefinerCssCompleted, definitionRow, DisplayXls); break;
                    case "CodeDefinerCssCompleted_ja": Display.CodeDefinerCssCompleted_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.CodeDefinerCssCompleted_ja, definitionRow, DisplayXls); break;
                    case "CodeDefinerBackupCompleted": Display.CodeDefinerBackupCompleted = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.CodeDefinerBackupCompleted, definitionRow, DisplayXls); break;
                    case "CodeDefinerBackupCompleted_ja": Display.CodeDefinerBackupCompleted_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.CodeDefinerBackupCompleted_ja, definitionRow, DisplayXls); break;
                    case "MailTransmissionCompletion": Display.MailTransmissionCompletion = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.MailTransmissionCompletion, definitionRow, DisplayXls); break;
                    case "MailTransmissionCompletion_ja": Display.MailTransmissionCompletion_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.MailTransmissionCompletion_ja, definitionRow, DisplayXls); break;
                    case "ChangingPasswordComplete": Display.ChangingPasswordComplete = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.ChangingPasswordComplete, definitionRow, DisplayXls); break;
                    case "ChangingPasswordComplete_ja": Display.ChangingPasswordComplete_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.ChangingPasswordComplete_ja, definitionRow, DisplayXls); break;
                    case "PasswordResetCompleted": Display.PasswordResetCompleted = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.PasswordResetCompleted, definitionRow, DisplayXls); break;
                    case "PasswordResetCompleted_ja": Display.PasswordResetCompleted_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.PasswordResetCompleted_ja, definitionRow, DisplayXls); break;
                    case "FileUpdateCompleted": Display.FileUpdateCompleted = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.FileUpdateCompleted, definitionRow, DisplayXls); break;
                    case "FileUpdateCompleted_ja": Display.FileUpdateCompleted_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.FileUpdateCompleted_ja, definitionRow, DisplayXls); break;
                    case "SynchronizationCompleted": Display.SynchronizationCompleted = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SynchronizationCompleted, definitionRow, DisplayXls); break;
                    case "SynchronizationCompleted_ja": Display.SynchronizationCompleted_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SynchronizationCompleted_ja, definitionRow, DisplayXls); break;
                    case "SentAcceptanceMail_space_": Display.SentAcceptanceMail_space_ = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SentAcceptanceMail_space_, definitionRow, DisplayXls); break;
                    case "SentAcceptanceMail_space__ja": Display.SentAcceptanceMail_space__ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.SentAcceptanceMail_space__ja, definitionRow, DisplayXls); break;
                    case "ConfirmDelete": Display.ConfirmDelete = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.ConfirmDelete, definitionRow, DisplayXls); break;
                    case "ConfirmDelete_ja": Display.ConfirmDelete_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.ConfirmDelete_ja, definitionRow, DisplayXls); break;
                    case "ConfirmSeparate": Display.ConfirmSeparate = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.ConfirmSeparate, definitionRow, DisplayXls); break;
                    case "ConfirmSeparate_ja": Display.ConfirmSeparate_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.ConfirmSeparate_ja, definitionRow, DisplayXls); break;
                    case "ConfirmSendMail": Display.ConfirmSendMail = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.ConfirmSendMail, definitionRow, DisplayXls); break;
                    case "ConfirmSendMail_ja": Display.ConfirmSendMail_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.ConfirmSendMail_ja, definitionRow, DisplayXls); break;
                    case "ConfirmSynchronize": Display.ConfirmSynchronize = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.ConfirmSynchronize, definitionRow, DisplayXls); break;
                    case "ConfirmSynchronize_ja": Display.ConfirmSynchronize_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.ConfirmSynchronize_ja, definitionRow, DisplayXls); break;
                    case "CanNotUpdate": Display.CanNotUpdate = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.CanNotUpdate, definitionRow, DisplayXls); break;
                    case "CanNotUpdate_ja": Display.CanNotUpdate_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.CanNotUpdate_ja, definitionRow, DisplayXls); break;
                    case "ReadOnlyBecausePreviousVer": Display.ReadOnlyBecausePreviousVer = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.ReadOnlyBecausePreviousVer, definitionRow, DisplayXls); break;
                    case "ReadOnlyBecausePreviousVer_ja": Display.ReadOnlyBecausePreviousVer_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.ReadOnlyBecausePreviousVer_ja, definitionRow, DisplayXls); break;
                    case "InCopying": Display.InCopying = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.InCopying, definitionRow, DisplayXls); break;
                    case "InCopying_ja": Display.InCopying_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.InCopying_ja, definitionRow, DisplayXls); break;
                    case "InCompression": Display.InCompression = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.InCompression, definitionRow, DisplayXls); break;
                    case "InCompression_ja": Display.InCompression_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.InCompression_ja, definitionRow, DisplayXls); break;
                    case "HasBeenMoved": Display.HasBeenMoved = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.HasBeenMoved, definitionRow, DisplayXls); break;
                    case "HasBeenMoved_ja": Display.HasBeenMoved_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.HasBeenMoved_ja, definitionRow, DisplayXls); break;
                    case "HasBeenDeleted": Display.HasBeenDeleted = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.HasBeenDeleted, definitionRow, DisplayXls); break;
                    case "HasBeenDeleted_ja": Display.HasBeenDeleted_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.HasBeenDeleted_ja, definitionRow, DisplayXls); break;
                    case "ValidateRequired": Display.ValidateRequired = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.ValidateRequired, definitionRow, DisplayXls); break;
                    case "ValidateRequired_ja": Display.ValidateRequired_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.ValidateRequired_ja, definitionRow, DisplayXls); break;
                    case "ValidateNumber": Display.ValidateNumber = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.ValidateNumber, definitionRow, DisplayXls); break;
                    case "ValidateNumber_ja": Display.ValidateNumber_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.ValidateNumber_ja, definitionRow, DisplayXls); break;
                    case "ValidateDate": Display.ValidateDate = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.ValidateDate, definitionRow, DisplayXls); break;
                    case "ValidateDate_ja": Display.ValidateDate_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.ValidateDate_ja, definitionRow, DisplayXls); break;
                    case "ValidateMail": Display.ValidateMail = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.ValidateMail, definitionRow, DisplayXls); break;
                    case "ValidateMail_ja": Display.ValidateMail_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.ValidateMail_ja, definitionRow, DisplayXls); break;
                    case "ValidateEqualTo": Display.ValidateEqualTo = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.ValidateEqualTo, definitionRow, DisplayXls); break;
                    case "ValidateEqualTo_ja": Display.ValidateEqualTo_ja = definitionRow[1].ToString(); SetDisplayTable(DisplayTable.ValidateEqualTo_ja, definitionRow, DisplayXls); break;
                    default: break;
                }
            });
            DisplayXls.XlsSheet.AsEnumerable().Skip(1).Where(o => o[0].ToString() != string.Empty).ForEach(definitionRow =>
            {
                var newDisplayDefinition = new DisplayDefinition();
                if (definitionRow.ContainsKey("Id")) { newDisplayDefinition.Id = definitionRow["Id"].ToString(); newDisplayDefinition.SavedId = newDisplayDefinition.Id; }
                if (definitionRow.ContainsKey("Content")) { newDisplayDefinition.Content = definitionRow["Content"].ToString(); newDisplayDefinition.SavedContent = newDisplayDefinition.Content; }
                if (definitionRow.ContainsKey("Type")) { newDisplayDefinition.Type = definitionRow["Type"].ToString(); newDisplayDefinition.SavedType = newDisplayDefinition.Type; }
                if (definitionRow.ContainsKey("Name")) { newDisplayDefinition.Name = definitionRow["Name"].ToString(); newDisplayDefinition.SavedName = newDisplayDefinition.Name; }
                if (definitionRow.ContainsKey("Language")) { newDisplayDefinition.Language = definitionRow["Language"].ToString(); newDisplayDefinition.SavedLanguage = newDisplayDefinition.Language; }
                if (definitionRow.ContainsKey("CssClass")) { newDisplayDefinition.CssClass = definitionRow["CssClass"].ToString(); newDisplayDefinition.SavedCssClass = newDisplayDefinition.CssClass; }
                if (definitionRow.ContainsKey("ClientScript")) { newDisplayDefinition.ClientScript = definitionRow["ClientScript"].ToBool(); newDisplayDefinition.SavedClientScript = newDisplayDefinition.ClientScript; }
                DisplayDefinitionCollection.Add(newDisplayDefinition);
            });
        }

        private static void SetDisplayTable(DisplayDefinition definition, XlsRow definitionRow, XlsIo displayxls)
        {
            if (definitionRow.ContainsKey("Id")) { definition.Id = definitionRow["Id"].ToString(); definition.SavedId = definition.Id; }
            if (definitionRow.ContainsKey("Content")) { definition.Content = definitionRow["Content"].ToString(); definition.SavedContent = definition.Content; }
            if (definitionRow.ContainsKey("Type")) { definition.Type = definitionRow["Type"].ToString(); definition.SavedType = definition.Type; }
            if (definitionRow.ContainsKey("Name")) { definition.Name = definitionRow["Name"].ToString(); definition.SavedName = definition.Name; }
            if (definitionRow.ContainsKey("Language")) { definition.Language = definitionRow["Language"].ToString(); definition.SavedLanguage = definition.Language; }
            if (definitionRow.ContainsKey("CssClass")) { definition.CssClass = definitionRow["CssClass"].ToString(); definition.SavedCssClass = definition.CssClass; }
            if (definitionRow.ContainsKey("ClientScript")) { definition.ClientScript = definitionRow["ClientScript"].ToBool(); definition.SavedClientScript = definition.ClientScript; }
        }

        private static void ConstructDisplayDefinitions()
        {
            DisplayXls = Initializer.DefinitionFile("definition_Display.xlsm");
            DisplayDefinitionCollection = new List<DisplayDefinition>();
            Display = new DisplayColumn2nd();
            DisplayTable = new DisplayTable();
        }

        public static XlsIo JavaScriptXls;
        public static List<JavaScriptDefinition> JavaScriptDefinitionCollection;
        public static JavaScriptColumn2nd JavaScript;
        public static JavaScriptTable JavaScriptTable;

        public static void SetJavaScriptDefinition()
        {
            ConstructJavaScriptDefinitions();
            if (JavaScriptXls.AccessStatus != Files.AccessStatuses.Read) { return; }
            JavaScriptXls.XlsSheet.ForEach(definitionRow =>
            {
                switch (definitionRow[0].ToString())
                {
                    case "Create": JavaScript.Create = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetJavaScriptTable(JavaScriptTable.Create, definitionRow, JavaScriptXls); break;
                    case "Update": JavaScript.Update = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetJavaScriptTable(JavaScriptTable.Update, definitionRow, JavaScriptXls); break;
                    case "CloseDialogAndSubmit": JavaScript.CloseDialogAndSubmit = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetJavaScriptTable(JavaScriptTable.CloseDialogAndSubmit, definitionRow, JavaScriptXls); break;
                    case "MoveTargets": JavaScript.MoveTargets = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetJavaScriptTable(JavaScriptTable.MoveTargets, definitionRow, JavaScriptXls); break;
                    case "Move": JavaScript.Move = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetJavaScriptTable(JavaScriptTable.Move, definitionRow, JavaScriptXls); break;
                    case "SetAggregationDetails": JavaScript.SetAggregationDetails = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetJavaScriptTable(JavaScriptTable.SetAggregationDetails, definitionRow, JavaScriptXls); break;
                    case "SendMail": JavaScript.SendMail = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetJavaScriptTable(JavaScriptTable.SendMail, definitionRow, JavaScriptXls); break;
                    case "Delete": JavaScript.Delete = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetJavaScriptTable(JavaScriptTable.Delete, definitionRow, JavaScriptXls); break;
                    case "WindowScrollTopAndSubmit": JavaScript.WindowScrollTopAndSubmit = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetJavaScriptTable(JavaScriptTable.WindowScrollTopAndSubmit, definitionRow, JavaScriptXls); break;
                    case "Submit": JavaScript.Submit = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetJavaScriptTable(JavaScriptTable.Submit, definitionRow, JavaScriptXls); break;
                    case "SetSiteImage": JavaScript.SetSiteImage = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetJavaScriptTable(JavaScriptTable.SetSiteImage, definitionRow, JavaScriptXls); break;
                    case "NewByLink": JavaScript.NewByLink = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetJavaScriptTable(JavaScriptTable.NewByLink, definitionRow, JavaScriptXls); break;
                    case "OpenDialog_ColumnProperties": JavaScript.OpenDialog_ColumnProperties = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetJavaScriptTable(JavaScriptTable.OpenDialog_ColumnProperties, definitionRow, JavaScriptXls); break;
                    case "EditOutgoingMail": JavaScript.EditOutgoingMail = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetJavaScriptTable(JavaScriptTable.EditOutgoingMail, definitionRow, JavaScriptXls); break;
                    case "EditExportSettings": JavaScript.EditExportSettings = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetJavaScriptTable(JavaScriptTable.EditExportSettings, definitionRow, JavaScriptXls); break;
                    case "EditImportSettings": JavaScript.EditImportSettings = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetJavaScriptTable(JavaScriptTable.EditImportSettings, definitionRow, JavaScriptXls); break;
                    case "EditSeparateSettings": JavaScript.EditSeparateSettings = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetJavaScriptTable(JavaScriptTable.EditSeparateSettings, definitionRow, JavaScriptXls); break;
                    case "AddSummary": JavaScript.AddSummary = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetJavaScriptTable(JavaScriptTable.AddSummary, definitionRow, JavaScriptXls); break;
                    case "Reply": JavaScript.Reply = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetJavaScriptTable(JavaScriptTable.Reply, definitionRow, JavaScriptXls); break;
                    case "EditMarkDown": JavaScript.EditMarkDown = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetJavaScriptTable(JavaScriptTable.EditMarkDown, definitionRow, JavaScriptXls); break;
                    case "CancelDialog": JavaScript.CancelDialog = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetJavaScriptTable(JavaScriptTable.CancelDialog, definitionRow, JavaScriptXls); break;
                    case "OpenDialog": JavaScript.OpenDialog = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetJavaScriptTable(JavaScriptTable.OpenDialog, definitionRow, JavaScriptXls); break;
                    case "Go": JavaScript.Go = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetJavaScriptTable(JavaScriptTable.Go, definitionRow, JavaScriptXls); break;
                    case "Import": JavaScript.Import = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetJavaScriptTable(JavaScriptTable.Import, definitionRow, JavaScriptXls); break;
                    case "DrawGantt": JavaScript.DrawGantt = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetJavaScriptTable(JavaScriptTable.DrawGantt, definitionRow, JavaScriptXls); break;
                    case "DrawBurnDown": JavaScript.DrawBurnDown = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetJavaScriptTable(JavaScriptTable.DrawBurnDown, definitionRow, JavaScriptXls); break;
                    case "DrawTimeSeries": JavaScript.DrawTimeSeries = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetJavaScriptTable(JavaScriptTable.DrawTimeSeries, definitionRow, JavaScriptXls); break;
                    case "SetKamban": JavaScript.SetKamban = definitionRow[1].ToString().NoSpace(definitionRow["NoSpace"].ToBool()); SetJavaScriptTable(JavaScriptTable.SetKamban, definitionRow, JavaScriptXls); break;
                    default: break;
                }
            });
            JavaScriptXls.XlsSheet.AsEnumerable().Skip(1).Where(o => o[0].ToString() != string.Empty).ForEach(definitionRow =>
            {
                var newJavaScriptDefinition = new JavaScriptDefinition();
                if (definitionRow.ContainsKey("Id")) { newJavaScriptDefinition.Id = definitionRow["Id"].ToString(); newJavaScriptDefinition.SavedId = newJavaScriptDefinition.Id; }
                if (definitionRow.ContainsKey("Body")) { newJavaScriptDefinition.Body = definitionRow["Body"].ToString(); newJavaScriptDefinition.SavedBody = newJavaScriptDefinition.Body; }
                if (definitionRow.ContainsKey("NoSpace")) { newJavaScriptDefinition.NoSpace = definitionRow["NoSpace"].ToBool(); newJavaScriptDefinition.SavedNoSpace = newJavaScriptDefinition.NoSpace; }
                JavaScriptDefinitionCollection.Add(newJavaScriptDefinition);
            });
        }

        private static void SetJavaScriptTable(JavaScriptDefinition definition, XlsRow definitionRow, XlsIo javaScriptxls)
        {
            if (definitionRow.ContainsKey("Id")) { definition.Id = definitionRow["Id"].ToString(); definition.SavedId = definition.Id; }
            if (definitionRow.ContainsKey("Body")) { definition.Body = definitionRow["Body"].ToString(); definition.SavedBody = definition.Body; }
            if (definitionRow.ContainsKey("NoSpace")) { definition.NoSpace = definitionRow["NoSpace"].ToBool(); definition.SavedNoSpace = definition.NoSpace; }
        }

        private static void ConstructJavaScriptDefinitions()
        {
            JavaScriptXls = Initializer.DefinitionFile("definition_JavaScript.xlsm");
            JavaScriptDefinitionCollection = new List<JavaScriptDefinition>();
            JavaScript = new JavaScriptColumn2nd();
            JavaScriptTable = new JavaScriptTable();
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
                    case "SiteBreadcrumb": Sql.SiteBreadcrumb = definitionRow[1].ToString(); SetSqlTable(SqlTable.SiteBreadcrumb, definitionRow, SqlXls); break;
                    case "SiteDepts": Sql.SiteDepts = definitionRow[1].ToString(); SetSqlTable(SqlTable.SiteDepts, definitionRow, SqlXls); break;
                    case "SiteUsers": Sql.SiteUsers = definitionRow[1].ToString(); SetSqlTable(SqlTable.SiteUsers, definitionRow, SqlXls); break;
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
                        case "NotTypeCs": codeDefinition.NotTypeCs = optionValue.ToBool(); break;
                        case "ItemOnly": codeDefinition.ItemOnly = optionValue.ToBool(); break;
                        case "NotItem": codeDefinition.NotItem = optionValue.ToBool(); break;
                        case "Session": codeDefinition.Session = optionValue.ToBool(); break;
                        case "GridColumn": codeDefinition.GridColumn = optionValue.ToBool(); break;
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
                        case "GridVisible": columnDefinition.GridVisible = optionValue.ToBool(); break;
                        case "FilterColumn": columnDefinition.FilterColumn = optionValue.ToInt(); break;
                        case "FilterVisible": columnDefinition.FilterVisible = optionValue.ToBool(); break;
                        case "EditorColumn": columnDefinition.EditorColumn = optionValue.ToBool(); break;
                        case "EditorVisible": columnDefinition.EditorVisible = optionValue.ToBool(); break;
                        case "TitleColumn": columnDefinition.TitleColumn = optionValue.ToInt(); break;
                        case "LinkColumn": columnDefinition.LinkColumn = optionValue.ToInt(); break;
                        case "LinkVisible": columnDefinition.LinkVisible = optionValue.ToBool(); break;
                        case "HistoryColumn": columnDefinition.HistoryColumn = optionValue.ToInt(); break;
                        case "HistoryVisible": columnDefinition.HistoryVisible = optionValue.ToBool(); break;
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
                        case "ColumnNameOld": columnDefinition.ColumnNameOld = optionValue.ToString(); break;
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
                        case "ControlDateTime": columnDefinition.ControlDateTime = optionValue.ToString(); break;
                        case "GridDateTime": columnDefinition.GridDateTime = optionValue.ToString(); break;
                        case "Aggregatable": columnDefinition.Aggregatable = optionValue.ToBool(); break;
                        case "Computable": columnDefinition.Computable = optionValue.ToBool(); break;
                        case "ChoicesText": columnDefinition.ChoicesText = optionValue.ToString(); break;
                        case "DefaultInput": columnDefinition.DefaultInput = optionValue.ToString(); break;
                        case "Own": columnDefinition.Own = optionValue.ToBool(); break;
                        case "FormName": columnDefinition.FormName = optionValue.ToString(); break;
                        case "Validators": columnDefinition.Validators = optionValue.ToString(); break;
                        case "DecimalPlaces": columnDefinition.DecimalPlaces = optionValue.ToInt(); break;
                        case "Min": columnDefinition.Min = optionValue.ToDecimal(); break;
                        case "Max": columnDefinition.Max = optionValue.ToDecimal(); break;
                        case "Step": columnDefinition.Step = optionValue.ToDecimal(); break;
                        case "StringFormat": columnDefinition.StringFormat = optionValue.ToString(); break;
                        case "Unit": columnDefinition.Unit = optionValue.ToString(); break;
                        case "Width": columnDefinition.Width = optionValue.ToInt(); break;
                        case "SettingEnable": columnDefinition.SettingEnable = optionValue.ToBool(); break;
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

        public static void SetDataViewDefinitionOption(
            string placeholder, DataViewDefinition dataViewDefinition)
        {
            placeholder.RegexFirst("(?<=\\().+(?=\\))").Split(',')
                .Where(o => !o.IsNullOrEmpty()).ForEach(option =>
                {
                    var optionName = option.Split_1st('=').Trim();
                    var optionValue = option.Split_2nd('=').Trim();
                    switch (optionName)
                    {
                        case "Id": dataViewDefinition.Id = optionValue.ToString(); break;
                        case "ReferenceType": dataViewDefinition.ReferenceType = optionValue.ToString(); break;
                        case "Name": dataViewDefinition.Name = optionValue.ToString(); break;
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
                        case "Manager": demoDefinition.Manager = optionValue.ToString(); break;
                        case "Owner": demoDefinition.Owner = optionValue.ToString(); break;
                        case "ClassA": demoDefinition.ClassA = optionValue.ToString(); break;
                        case "ClassB": demoDefinition.ClassB = optionValue.ToString(); break;
                        case "ClassC": demoDefinition.ClassC = optionValue.ToString(); break;
                        case "Creator": demoDefinition.Creator = optionValue.ToString(); break;
                        case "Updator": demoDefinition.Updator = optionValue.ToString(); break;
                        case "StartTime": demoDefinition.StartTime = optionValue.ToDateTime(); break;
                        case "CompletionTime": demoDefinition.CompletionTime = optionValue.ToDateTime(); break;
                        case "CreatedTime": demoDefinition.CreatedTime = optionValue.ToDateTime(); break;
                        case "UpdatedTime": demoDefinition.UpdatedTime = optionValue.ToDateTime(); break;
                    }
                });
        }

        public static void SetDisplayDefinitionOption(
            string placeholder, DisplayDefinition displayDefinition)
        {
            placeholder.RegexFirst("(?<=\\().+(?=\\))").Split(',')
                .Where(o => !o.IsNullOrEmpty()).ForEach(option =>
                {
                    var optionName = option.Split_1st('=').Trim();
                    var optionValue = option.Split_2nd('=').Trim();
                    switch (optionName)
                    {
                        case "Id": displayDefinition.Id = optionValue.ToString(); break;
                        case "Content": displayDefinition.Content = optionValue.ToString(); break;
                        case "Type": displayDefinition.Type = optionValue.ToString(); break;
                        case "Name": displayDefinition.Name = optionValue.ToString(); break;
                        case "Language": displayDefinition.Language = optionValue.ToString(); break;
                        case "CssClass": displayDefinition.CssClass = optionValue.ToString(); break;
                        case "ClientScript": displayDefinition.ClientScript = optionValue.ToBool(); break;
                    }
                });
        }

        public static void SetJavaScriptDefinitionOption(
            string placeholder, JavaScriptDefinition javaScriptDefinition)
        {
            placeholder.RegexFirst("(?<=\\().+(?=\\))").Split(',')
                .Where(o => !o.IsNullOrEmpty()).ForEach(option =>
                {
                    var optionName = option.Split_1st('=').Trim();
                    var optionValue = option.Split_2nd('=').Trim();
                    switch (optionName)
                    {
                        case "Id": javaScriptDefinition.Id = optionValue.ToString(); break;
                        case "Body": javaScriptDefinition.Body = optionValue.ToString(); break;
                        case "NoSpace": javaScriptDefinition.NoSpace = optionValue.ToBool(); break;
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
        public bool NotTypeCs; public bool SavedNotTypeCs;
        public bool ItemOnly; public bool SavedItemOnly;
        public bool NotItem; public bool SavedNotItem;
        public bool Session; public bool SavedSession;
        public bool GridColumn; public bool SavedGridColumn;
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
            if (propertyCollection.ContainsKey("NotTypeCs")) NotTypeCs = propertyCollection["NotTypeCs"].ToBool(); else NotTypeCs = false;
            if (propertyCollection.ContainsKey("ItemOnly")) ItemOnly = propertyCollection["ItemOnly"].ToBool(); else ItemOnly = false;
            if (propertyCollection.ContainsKey("NotItem")) NotItem = propertyCollection["NotItem"].ToBool(); else NotItem = false;
            if (propertyCollection.ContainsKey("Session")) Session = propertyCollection["Session"].ToBool(); else Session = false;
            if (propertyCollection.ContainsKey("GridColumn")) GridColumn = propertyCollection["GridColumn"].ToBool(); else GridColumn = false;
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
                    case "NotTypeCs": return NotTypeCs;
                    case "ItemOnly": return ItemOnly;
                    case "NotItem": return NotItem;
                    case "Session": return Session;
                    case "GridColumn": return GridColumn;
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
            NotTypeCs = SavedNotTypeCs;
            ItemOnly = SavedItemOnly;
            NotItem = SavedNotItem;
            Session = SavedSession;
            GridColumn = SavedGridColumn;
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
        public string BundleConfig;
        public string BundleConfig_Validators;
        public string Controller;
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
        public string Model_SetSiteSettingsProperties;
        public string Model_SetTenantId;
        public string Model_SetSiteId;
        public string Model_Create;
        public string Model_ValidateBeforeCreate;
        public string Model_ValidateBeforeCreateCases;
        public string Model_InsertItems;
        public string Model_InsertItemsAfter;
        public string Model_SelectIdentity;
        public string Model_Insert;
        public string Model_InsertIdentity;
        public string Model_InsertIdentitySet;
        public string Model_Update;
        public string Model_TitleDisplay;
        public string Model_ValidateBeforeUpdate;
        public string Model_ValidateBeforeUpdateCases;
        public string Model_UpdateItems;
        public string Model_UpdateItems_Wikis;
        public string Model_UpdateItems_OutgoingMails;
        public string Model_SynchronizeSummaryExecute;
        public string Model_SynchronizeSummary;
        public string Model_SynchronizeSummaryColumnCases;
        public string Model_UpdateFormulaColumns;
        public string Model_UpdateFormulaColumns_ColumnCases;
        public string Model_UpdateOrCreate;
        public string Model_InsertLinksByCreate;
        public string Model_InsertLinksByUpdate;
        public string Model_InsertLinksByUpdate_Site;
        public string Model_InsertLinks;
        public string Model_InsertLinksCases;
        public string Model_ResponseLinks;
        public string Model_ResponseByUpdate_FormulaResponse;
        public string Model_Copy;
        public string Model_Move;
        public string Model_Delete;
        public string Model_Delete_Item;
        public string Model_Restore;
        public string Model_Restore_Item;
        public string Model_PhysicalDelete;
        public string Model_Separate;
        public string Model_AddSqlParamIdentity;
        public string Model_AddSqlParamPk;
        public string Model_AddSqlParam;
        public string Model_CanCreate;
        public string Model_CanUpdate;
        public string Model_CanDelete;
        public string Model_CanDownLoadFile;
        public string Model_CanExport;
        public string Model_CanImport;
        public string Model_CanEditSite;
        public string Model_CanEditPermission;
        public string Model_Self;
        public string Model_CanEditTenant;
        public string Model_CanEditSys;
        public string Model_Title;
        public string Model_AddSqlParamPkHistory;
        public string Model_SetByForm;
        public string Model_SetByForm_ColumnCases;
        public string Model_SetByForm_SetByFormula;
        public string Model_ToUniversal;
        public string Model_SetByForm_Files;
        public string Model_SetByForm_Site;
        public string Model_SetByFormula;
        public string Model_SetByFormula_Data;
        public string Model_SetByFormula_ColumnCases;
        public string Model_SetBySession;
        public string Model_SetPk;
        public string Model_Set;
        public string Model_RedirectAfterDelete;
        public string Model_RedirectAfterDeleteNotItem;
        public string Model_RedirectAfterDeleteItem;
        public string Model_RedirectAfterDeleteSite;
        public string Model_Histories;
        public string Model_SwitchRecord;
        public string Model_SiteModel;
        public string Model_PushState;
        public string Model_PushState_Item;
        public string Model_SiteId;
        public string Model_SwitchItems;
        public string Model_IndexCases;
        public string Model_SiteMenu;
        public string Model_Index;
        public string Model_NewCases;
        public string Model_EditorCases;
        public string Model_ImportCases;
        public string Model_ExportCases;
        public string Model_DataViewCases;
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
        public string Model_PreviousCases;
        public string Model_NextCases;
        public string Model_ReloadCases;
        public string Model_GetCases;
        public string Model_BurnDownRecordDetailsCases;
        public string Model_UpdateByKambanCases;
        public string Model_Editor;
        public string Model_SiteSettingsParameters;
        public string Model_PermissionTypeParameters;
        public string Model_SetSiteSettings;
        public string Model_SetPermissionType;
        public string Model_EditorParam;
        public string Model_SiteSettings;
        public string Model_SiteSettingsWithParameterName;
        public string Model_SiteSettingsWithParameterNameLower;
        public string Model_SiteSettingsParam;
        public string Model_PermissionType;
        public string Model_PermissionTypeWithParameterName;
        public string Model_PermissionTypeWithParameterNameLower;
        public string Model_SiteSettingsUtility;
        public string Model_PermissionTypesAdmins;
        public string Model_Subset;
        public string Model_Subset_Properties;
        public string Model_Subset_SetProperties;
        public string Model_Subset_SearchIndexes;
        public string Model_Collection;
        public string Model_Utility;
        public string Model_Utility_Index;
        public string Model_Utility_ImportSettings;
        public string Model_Utility_GridRows_BackUrl;
        public string Model_Utility_GridRows_BackUrlItem;
        public string Model_Utility_GridRows_OnClick;
        public string Model_Utility_GridRows_OnClickItem;
        public string Model_Utility_Td;
        public string Model_Utility_TdCases;
        public string Model_Utility_AddSqlColumnSiteId;
        public string Model_Utility_AddSqlColumn;
        public string Model_Utility_GridSqlWhereTenantId;
        public string Model_Utility_GridSqlWhereSiteId;
        public string Model_Utility_Editor;
        public string Model_Utility_UserSelf;
        public string Model_Utility_EditorItem;
        public string Model_Utility_EditorProfilePermission;
        public string Model_Utility_EditorBackUrl;
        public string Model_Utility_EditorBackUrl_Users;
        public string Model_Utility_FieldCases;
        public string Model_Utility_FieldCases_Item;
        public string Model_Utility_GetSwitchTargets;
        public string Model_Utility_SqlWhereTenantId;
        public string Model_Utility_SqlWhereSiteId;
        public string Model_Utility_SiteId;
        public string Model_Utility_SiteIdParam;
        public string Model_Utility_FormResponse;
        public string Model_Utility_FormResponse_ColumnCases;
        public string Model_Utility_FormulaResponse;
        public string Model_Utility_FormulaResponse_ColumnCases;
        public string Model_Utility_TableName;
        public string Model_Utility_TableNameCases;
        public string Model_Utility_BulkMove;
        public string Model_Utility_BulkDelete;
        public string Model_Utility_Import;
        public string Model_Utility_ImportCases;
        public string Model_Utility_ImportValidatorHeaders;
        public string Model_Utility_ImportValidatorCases;
        public string Model_Utility_Export;
        public string Model_Utility_CsvColumnCases;
        public string Model_Utility_NotNull;
        public string Model_Utility_TitleDisplayValue;
        public string Model_Utility_TitleDisplayValueCases_Item;
        public string Model_Utility_TitleDisplayValueCases_DataRow;
        public string Model_Utility_UpdateByKamban;
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
        public string Rds_Columns_SqlTotalCases;
        public string Rds_Columns_SqlAverageCases;
        public string Rds_Columns_SqlMaxCases;
        public string Rds_Columns_SqlMinCases;
        public string Rds_Columns_SqlColumn;
        public string Rds_Columns_SqlColumn_SelectColumns;
        public string Rds_Columns_SqlColumn_ComputeColumn;
        public string Rds_Columns_SqlColumnComputes1;
        public string Rds_Columns_SqlColumnComputes2;
        public string Rds_Columns_SqlWhere;
        public string Rds_Columns_SqlWhere_SelectColumns;
        public string Rds_Columns_SqlWhere_ComputeColumn;
        public string Rds_Columns_SqlWhere_In;
        public string Rds_Columns_SqlWhere_Between_Number;
        public string Rds_Columns_SqlWhere_Between_DateTime;
        public string Rds_Columns_SqlGroupByCases;
        public string Rds_Columns_SqlGroupBy;
        public string Rds_Columns_SqlHavingComputes1;
        public string Rds_Columns_SqlHavingComputes2;
        public string Rds_Columns_SqlOrderBy1;
        public string Rds_Columns_SqlOrderBy2;
        public string Rds_Columns_SqlOrderByComputes1;
        public string Rds_Columns_SqlOrderByComputes2;
        public string Rds_Columns_SqlParam_ItemId;
        public string Rds_Columns_SqlParam;
        public string Rds_Columns_SqlParam_Enum;
        public string Rds_Defaults;
        public string Rds_ColumnDefault;
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
        public string Rds_SiteSettings_SiteId;
        public string Rds_TitleColumn;
        public string Rds_TitleColumnCases;
        public string Titles;
        public string Titles_TitleDisplayValueCases;
        public string Indexes;
        public string Indexes_TableCases;
        public string Responses;
        public string Responses_ItemSubsets;
        public string ItemsInitializer;
        public string ItemsInitializer_InitItems;
        public string SiteSettings;
        public string SiteSettings_Get;
        public string SiteSettings_GetCases;
        public string SiteSettings_GetModels;
        public string SiteSettings_GetModels_Items;
        public string SiteSettings_GetModels_Includes;
        public string DataViewFilters;
        public string DataViewFilters_Search_TableCases;
        public string GridSorters;
        public string GridSorters_Model;
        public string GridSorters_Cases;
        public string HtmlLinks;
        public string HtmlLinks_TableCases;
        public string HtmlLinks_HeaderCases;
        public string HtmlLinks_Headers;
        public string HtmlLinks_Rows;
        public string HtmlLinks_RowColumns;
        public string Summaries;
        public string Summaries_SynchronizeCases;
        public string Summaries_SynchronizeTables;
        public string Summaries_ParamCases;
        public string Summaries_SelectCountTables;
        public string Summaries_SelectTotalTableCases;
        public string Summaries_SelectTotalColumns;
        public string Summaries_SelectTotalColumnCases;
        public string Summaries_SelectAverageTableCases;
        public string Summaries_SelectAverageColumns;
        public string Summaries_SelectAverageColumnCases;
        public string Summaries_SelectMaxTableCases;
        public string Summaries_SelectMaxColumns;
        public string Summaries_SelectMaxColumnCases;
        public string Summaries_SelectMinTableCases;
        public string Summaries_SelectMinColumns;
        public string Summaries_SelectMinColumnCases;
        public string Summaries_WhereTables;
        public string Summaries_WhereColumnCases;
        public string Formulas;
        public string Formulas_TableCases;
        public string Formulas_Updates;
        public string Messages;
        public string Messages_Parts;
        public string Messages_Resonses;
        public string ResponseCollection;
        public string ResponseCollection_Models;
        public string ResponseCollection_ValueModels;
        public string ResponseCollection_ValueColumns;
        public string Displays;
        public string Displays_Parts;
        public string HtmlScripts;
        public string HtmlScripts_ValidatorCases;
        public string ClientValidators;
        public string ClientValidators_Form;
        public string ClientValidators_Rules;
        public string ClientValidators_Messages;
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
        public CodeDefinition BundleConfig = new CodeDefinition();
        public CodeDefinition BundleConfig_Validators = new CodeDefinition();
        public CodeDefinition Controller = new CodeDefinition();
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
        public CodeDefinition Model_SetSiteSettingsProperties = new CodeDefinition();
        public CodeDefinition Model_SetTenantId = new CodeDefinition();
        public CodeDefinition Model_SetSiteId = new CodeDefinition();
        public CodeDefinition Model_Create = new CodeDefinition();
        public CodeDefinition Model_ValidateBeforeCreate = new CodeDefinition();
        public CodeDefinition Model_ValidateBeforeCreateCases = new CodeDefinition();
        public CodeDefinition Model_InsertItems = new CodeDefinition();
        public CodeDefinition Model_InsertItemsAfter = new CodeDefinition();
        public CodeDefinition Model_SelectIdentity = new CodeDefinition();
        public CodeDefinition Model_Insert = new CodeDefinition();
        public CodeDefinition Model_InsertIdentity = new CodeDefinition();
        public CodeDefinition Model_InsertIdentitySet = new CodeDefinition();
        public CodeDefinition Model_Update = new CodeDefinition();
        public CodeDefinition Model_TitleDisplay = new CodeDefinition();
        public CodeDefinition Model_ValidateBeforeUpdate = new CodeDefinition();
        public CodeDefinition Model_ValidateBeforeUpdateCases = new CodeDefinition();
        public CodeDefinition Model_UpdateItems = new CodeDefinition();
        public CodeDefinition Model_UpdateItems_Wikis = new CodeDefinition();
        public CodeDefinition Model_UpdateItems_OutgoingMails = new CodeDefinition();
        public CodeDefinition Model_SynchronizeSummaryExecute = new CodeDefinition();
        public CodeDefinition Model_SynchronizeSummary = new CodeDefinition();
        public CodeDefinition Model_SynchronizeSummaryColumnCases = new CodeDefinition();
        public CodeDefinition Model_UpdateFormulaColumns = new CodeDefinition();
        public CodeDefinition Model_UpdateFormulaColumns_ColumnCases = new CodeDefinition();
        public CodeDefinition Model_UpdateOrCreate = new CodeDefinition();
        public CodeDefinition Model_InsertLinksByCreate = new CodeDefinition();
        public CodeDefinition Model_InsertLinksByUpdate = new CodeDefinition();
        public CodeDefinition Model_InsertLinksByUpdate_Site = new CodeDefinition();
        public CodeDefinition Model_InsertLinks = new CodeDefinition();
        public CodeDefinition Model_InsertLinksCases = new CodeDefinition();
        public CodeDefinition Model_ResponseLinks = new CodeDefinition();
        public CodeDefinition Model_ResponseByUpdate_FormulaResponse = new CodeDefinition();
        public CodeDefinition Model_Copy = new CodeDefinition();
        public CodeDefinition Model_Move = new CodeDefinition();
        public CodeDefinition Model_Delete = new CodeDefinition();
        public CodeDefinition Model_Delete_Item = new CodeDefinition();
        public CodeDefinition Model_Restore = new CodeDefinition();
        public CodeDefinition Model_Restore_Item = new CodeDefinition();
        public CodeDefinition Model_PhysicalDelete = new CodeDefinition();
        public CodeDefinition Model_Separate = new CodeDefinition();
        public CodeDefinition Model_AddSqlParamIdentity = new CodeDefinition();
        public CodeDefinition Model_AddSqlParamPk = new CodeDefinition();
        public CodeDefinition Model_AddSqlParam = new CodeDefinition();
        public CodeDefinition Model_CanCreate = new CodeDefinition();
        public CodeDefinition Model_CanUpdate = new CodeDefinition();
        public CodeDefinition Model_CanDelete = new CodeDefinition();
        public CodeDefinition Model_CanDownLoadFile = new CodeDefinition();
        public CodeDefinition Model_CanExport = new CodeDefinition();
        public CodeDefinition Model_CanImport = new CodeDefinition();
        public CodeDefinition Model_CanEditSite = new CodeDefinition();
        public CodeDefinition Model_CanEditPermission = new CodeDefinition();
        public CodeDefinition Model_Self = new CodeDefinition();
        public CodeDefinition Model_CanEditTenant = new CodeDefinition();
        public CodeDefinition Model_CanEditSys = new CodeDefinition();
        public CodeDefinition Model_Title = new CodeDefinition();
        public CodeDefinition Model_AddSqlParamPkHistory = new CodeDefinition();
        public CodeDefinition Model_SetByForm = new CodeDefinition();
        public CodeDefinition Model_SetByForm_ColumnCases = new CodeDefinition();
        public CodeDefinition Model_SetByForm_SetByFormula = new CodeDefinition();
        public CodeDefinition Model_ToUniversal = new CodeDefinition();
        public CodeDefinition Model_SetByForm_Files = new CodeDefinition();
        public CodeDefinition Model_SetByForm_Site = new CodeDefinition();
        public CodeDefinition Model_SetByFormula = new CodeDefinition();
        public CodeDefinition Model_SetByFormula_Data = new CodeDefinition();
        public CodeDefinition Model_SetByFormula_ColumnCases = new CodeDefinition();
        public CodeDefinition Model_SetBySession = new CodeDefinition();
        public CodeDefinition Model_SetPk = new CodeDefinition();
        public CodeDefinition Model_Set = new CodeDefinition();
        public CodeDefinition Model_RedirectAfterDelete = new CodeDefinition();
        public CodeDefinition Model_RedirectAfterDeleteNotItem = new CodeDefinition();
        public CodeDefinition Model_RedirectAfterDeleteItem = new CodeDefinition();
        public CodeDefinition Model_RedirectAfterDeleteSite = new CodeDefinition();
        public CodeDefinition Model_Histories = new CodeDefinition();
        public CodeDefinition Model_SwitchRecord = new CodeDefinition();
        public CodeDefinition Model_SiteModel = new CodeDefinition();
        public CodeDefinition Model_PushState = new CodeDefinition();
        public CodeDefinition Model_PushState_Item = new CodeDefinition();
        public CodeDefinition Model_SiteId = new CodeDefinition();
        public CodeDefinition Model_SwitchItems = new CodeDefinition();
        public CodeDefinition Model_IndexCases = new CodeDefinition();
        public CodeDefinition Model_SiteMenu = new CodeDefinition();
        public CodeDefinition Model_Index = new CodeDefinition();
        public CodeDefinition Model_NewCases = new CodeDefinition();
        public CodeDefinition Model_EditorCases = new CodeDefinition();
        public CodeDefinition Model_ImportCases = new CodeDefinition();
        public CodeDefinition Model_ExportCases = new CodeDefinition();
        public CodeDefinition Model_DataViewCases = new CodeDefinition();
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
        public CodeDefinition Model_PreviousCases = new CodeDefinition();
        public CodeDefinition Model_NextCases = new CodeDefinition();
        public CodeDefinition Model_ReloadCases = new CodeDefinition();
        public CodeDefinition Model_GetCases = new CodeDefinition();
        public CodeDefinition Model_BurnDownRecordDetailsCases = new CodeDefinition();
        public CodeDefinition Model_UpdateByKambanCases = new CodeDefinition();
        public CodeDefinition Model_Editor = new CodeDefinition();
        public CodeDefinition Model_SiteSettingsParameters = new CodeDefinition();
        public CodeDefinition Model_PermissionTypeParameters = new CodeDefinition();
        public CodeDefinition Model_SetSiteSettings = new CodeDefinition();
        public CodeDefinition Model_SetPermissionType = new CodeDefinition();
        public CodeDefinition Model_EditorParam = new CodeDefinition();
        public CodeDefinition Model_SiteSettings = new CodeDefinition();
        public CodeDefinition Model_SiteSettingsWithParameterName = new CodeDefinition();
        public CodeDefinition Model_SiteSettingsWithParameterNameLower = new CodeDefinition();
        public CodeDefinition Model_SiteSettingsParam = new CodeDefinition();
        public CodeDefinition Model_PermissionType = new CodeDefinition();
        public CodeDefinition Model_PermissionTypeWithParameterName = new CodeDefinition();
        public CodeDefinition Model_PermissionTypeWithParameterNameLower = new CodeDefinition();
        public CodeDefinition Model_SiteSettingsUtility = new CodeDefinition();
        public CodeDefinition Model_PermissionTypesAdmins = new CodeDefinition();
        public CodeDefinition Model_Subset = new CodeDefinition();
        public CodeDefinition Model_Subset_Properties = new CodeDefinition();
        public CodeDefinition Model_Subset_SetProperties = new CodeDefinition();
        public CodeDefinition Model_Subset_SearchIndexes = new CodeDefinition();
        public CodeDefinition Model_Collection = new CodeDefinition();
        public CodeDefinition Model_Utility = new CodeDefinition();
        public CodeDefinition Model_Utility_Index = new CodeDefinition();
        public CodeDefinition Model_Utility_ImportSettings = new CodeDefinition();
        public CodeDefinition Model_Utility_GridRows_BackUrl = new CodeDefinition();
        public CodeDefinition Model_Utility_GridRows_BackUrlItem = new CodeDefinition();
        public CodeDefinition Model_Utility_GridRows_OnClick = new CodeDefinition();
        public CodeDefinition Model_Utility_GridRows_OnClickItem = new CodeDefinition();
        public CodeDefinition Model_Utility_Td = new CodeDefinition();
        public CodeDefinition Model_Utility_TdCases = new CodeDefinition();
        public CodeDefinition Model_Utility_AddSqlColumnSiteId = new CodeDefinition();
        public CodeDefinition Model_Utility_AddSqlColumn = new CodeDefinition();
        public CodeDefinition Model_Utility_GridSqlWhereTenantId = new CodeDefinition();
        public CodeDefinition Model_Utility_GridSqlWhereSiteId = new CodeDefinition();
        public CodeDefinition Model_Utility_Editor = new CodeDefinition();
        public CodeDefinition Model_Utility_UserSelf = new CodeDefinition();
        public CodeDefinition Model_Utility_EditorItem = new CodeDefinition();
        public CodeDefinition Model_Utility_EditorProfilePermission = new CodeDefinition();
        public CodeDefinition Model_Utility_EditorBackUrl = new CodeDefinition();
        public CodeDefinition Model_Utility_EditorBackUrl_Users = new CodeDefinition();
        public CodeDefinition Model_Utility_FieldCases = new CodeDefinition();
        public CodeDefinition Model_Utility_FieldCases_Item = new CodeDefinition();
        public CodeDefinition Model_Utility_GetSwitchTargets = new CodeDefinition();
        public CodeDefinition Model_Utility_SqlWhereTenantId = new CodeDefinition();
        public CodeDefinition Model_Utility_SqlWhereSiteId = new CodeDefinition();
        public CodeDefinition Model_Utility_SiteId = new CodeDefinition();
        public CodeDefinition Model_Utility_SiteIdParam = new CodeDefinition();
        public CodeDefinition Model_Utility_FormResponse = new CodeDefinition();
        public CodeDefinition Model_Utility_FormResponse_ColumnCases = new CodeDefinition();
        public CodeDefinition Model_Utility_FormulaResponse = new CodeDefinition();
        public CodeDefinition Model_Utility_FormulaResponse_ColumnCases = new CodeDefinition();
        public CodeDefinition Model_Utility_TableName = new CodeDefinition();
        public CodeDefinition Model_Utility_TableNameCases = new CodeDefinition();
        public CodeDefinition Model_Utility_BulkMove = new CodeDefinition();
        public CodeDefinition Model_Utility_BulkDelete = new CodeDefinition();
        public CodeDefinition Model_Utility_Import = new CodeDefinition();
        public CodeDefinition Model_Utility_ImportCases = new CodeDefinition();
        public CodeDefinition Model_Utility_ImportValidatorHeaders = new CodeDefinition();
        public CodeDefinition Model_Utility_ImportValidatorCases = new CodeDefinition();
        public CodeDefinition Model_Utility_Export = new CodeDefinition();
        public CodeDefinition Model_Utility_CsvColumnCases = new CodeDefinition();
        public CodeDefinition Model_Utility_NotNull = new CodeDefinition();
        public CodeDefinition Model_Utility_TitleDisplayValue = new CodeDefinition();
        public CodeDefinition Model_Utility_TitleDisplayValueCases_Item = new CodeDefinition();
        public CodeDefinition Model_Utility_TitleDisplayValueCases_DataRow = new CodeDefinition();
        public CodeDefinition Model_Utility_UpdateByKamban = new CodeDefinition();
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
        public CodeDefinition Rds_Columns_SqlTotalCases = new CodeDefinition();
        public CodeDefinition Rds_Columns_SqlAverageCases = new CodeDefinition();
        public CodeDefinition Rds_Columns_SqlMaxCases = new CodeDefinition();
        public CodeDefinition Rds_Columns_SqlMinCases = new CodeDefinition();
        public CodeDefinition Rds_Columns_SqlColumn = new CodeDefinition();
        public CodeDefinition Rds_Columns_SqlColumn_SelectColumns = new CodeDefinition();
        public CodeDefinition Rds_Columns_SqlColumn_ComputeColumn = new CodeDefinition();
        public CodeDefinition Rds_Columns_SqlColumnComputes1 = new CodeDefinition();
        public CodeDefinition Rds_Columns_SqlColumnComputes2 = new CodeDefinition();
        public CodeDefinition Rds_Columns_SqlWhere = new CodeDefinition();
        public CodeDefinition Rds_Columns_SqlWhere_SelectColumns = new CodeDefinition();
        public CodeDefinition Rds_Columns_SqlWhere_ComputeColumn = new CodeDefinition();
        public CodeDefinition Rds_Columns_SqlWhere_In = new CodeDefinition();
        public CodeDefinition Rds_Columns_SqlWhere_Between_Number = new CodeDefinition();
        public CodeDefinition Rds_Columns_SqlWhere_Between_DateTime = new CodeDefinition();
        public CodeDefinition Rds_Columns_SqlGroupByCases = new CodeDefinition();
        public CodeDefinition Rds_Columns_SqlGroupBy = new CodeDefinition();
        public CodeDefinition Rds_Columns_SqlHavingComputes1 = new CodeDefinition();
        public CodeDefinition Rds_Columns_SqlHavingComputes2 = new CodeDefinition();
        public CodeDefinition Rds_Columns_SqlOrderBy1 = new CodeDefinition();
        public CodeDefinition Rds_Columns_SqlOrderBy2 = new CodeDefinition();
        public CodeDefinition Rds_Columns_SqlOrderByComputes1 = new CodeDefinition();
        public CodeDefinition Rds_Columns_SqlOrderByComputes2 = new CodeDefinition();
        public CodeDefinition Rds_Columns_SqlParam_ItemId = new CodeDefinition();
        public CodeDefinition Rds_Columns_SqlParam = new CodeDefinition();
        public CodeDefinition Rds_Columns_SqlParam_Enum = new CodeDefinition();
        public CodeDefinition Rds_Defaults = new CodeDefinition();
        public CodeDefinition Rds_ColumnDefault = new CodeDefinition();
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
        public CodeDefinition Rds_SiteSettings_SiteId = new CodeDefinition();
        public CodeDefinition Rds_TitleColumn = new CodeDefinition();
        public CodeDefinition Rds_TitleColumnCases = new CodeDefinition();
        public CodeDefinition Titles = new CodeDefinition();
        public CodeDefinition Titles_TitleDisplayValueCases = new CodeDefinition();
        public CodeDefinition Indexes = new CodeDefinition();
        public CodeDefinition Indexes_TableCases = new CodeDefinition();
        public CodeDefinition Responses = new CodeDefinition();
        public CodeDefinition Responses_ItemSubsets = new CodeDefinition();
        public CodeDefinition ItemsInitializer = new CodeDefinition();
        public CodeDefinition ItemsInitializer_InitItems = new CodeDefinition();
        public CodeDefinition SiteSettings = new CodeDefinition();
        public CodeDefinition SiteSettings_Get = new CodeDefinition();
        public CodeDefinition SiteSettings_GetCases = new CodeDefinition();
        public CodeDefinition SiteSettings_GetModels = new CodeDefinition();
        public CodeDefinition SiteSettings_GetModels_Items = new CodeDefinition();
        public CodeDefinition SiteSettings_GetModels_Includes = new CodeDefinition();
        public CodeDefinition DataViewFilters = new CodeDefinition();
        public CodeDefinition DataViewFilters_Search_TableCases = new CodeDefinition();
        public CodeDefinition GridSorters = new CodeDefinition();
        public CodeDefinition GridSorters_Model = new CodeDefinition();
        public CodeDefinition GridSorters_Cases = new CodeDefinition();
        public CodeDefinition HtmlLinks = new CodeDefinition();
        public CodeDefinition HtmlLinks_TableCases = new CodeDefinition();
        public CodeDefinition HtmlLinks_HeaderCases = new CodeDefinition();
        public CodeDefinition HtmlLinks_Headers = new CodeDefinition();
        public CodeDefinition HtmlLinks_Rows = new CodeDefinition();
        public CodeDefinition HtmlLinks_RowColumns = new CodeDefinition();
        public CodeDefinition Summaries = new CodeDefinition();
        public CodeDefinition Summaries_SynchronizeCases = new CodeDefinition();
        public CodeDefinition Summaries_SynchronizeTables = new CodeDefinition();
        public CodeDefinition Summaries_ParamCases = new CodeDefinition();
        public CodeDefinition Summaries_SelectCountTables = new CodeDefinition();
        public CodeDefinition Summaries_SelectTotalTableCases = new CodeDefinition();
        public CodeDefinition Summaries_SelectTotalColumns = new CodeDefinition();
        public CodeDefinition Summaries_SelectTotalColumnCases = new CodeDefinition();
        public CodeDefinition Summaries_SelectAverageTableCases = new CodeDefinition();
        public CodeDefinition Summaries_SelectAverageColumns = new CodeDefinition();
        public CodeDefinition Summaries_SelectAverageColumnCases = new CodeDefinition();
        public CodeDefinition Summaries_SelectMaxTableCases = new CodeDefinition();
        public CodeDefinition Summaries_SelectMaxColumns = new CodeDefinition();
        public CodeDefinition Summaries_SelectMaxColumnCases = new CodeDefinition();
        public CodeDefinition Summaries_SelectMinTableCases = new CodeDefinition();
        public CodeDefinition Summaries_SelectMinColumns = new CodeDefinition();
        public CodeDefinition Summaries_SelectMinColumnCases = new CodeDefinition();
        public CodeDefinition Summaries_WhereTables = new CodeDefinition();
        public CodeDefinition Summaries_WhereColumnCases = new CodeDefinition();
        public CodeDefinition Formulas = new CodeDefinition();
        public CodeDefinition Formulas_TableCases = new CodeDefinition();
        public CodeDefinition Formulas_Updates = new CodeDefinition();
        public CodeDefinition Messages = new CodeDefinition();
        public CodeDefinition Messages_Parts = new CodeDefinition();
        public CodeDefinition Messages_Resonses = new CodeDefinition();
        public CodeDefinition ResponseCollection = new CodeDefinition();
        public CodeDefinition ResponseCollection_Models = new CodeDefinition();
        public CodeDefinition ResponseCollection_ValueModels = new CodeDefinition();
        public CodeDefinition ResponseCollection_ValueColumns = new CodeDefinition();
        public CodeDefinition Displays = new CodeDefinition();
        public CodeDefinition Displays_Parts = new CodeDefinition();
        public CodeDefinition HtmlScripts = new CodeDefinition();
        public CodeDefinition HtmlScripts_ValidatorCases = new CodeDefinition();
        public CodeDefinition ClientValidators = new CodeDefinition();
        public CodeDefinition ClientValidators_Form = new CodeDefinition();
        public CodeDefinition ClientValidators_Rules = new CodeDefinition();
        public CodeDefinition ClientValidators_Messages = new CodeDefinition();
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
        public bool GridVisible; public bool SavedGridVisible;
        public int FilterColumn; public int SavedFilterColumn;
        public bool FilterVisible; public bool SavedFilterVisible;
        public bool EditorColumn; public bool SavedEditorColumn;
        public bool EditorVisible; public bool SavedEditorVisible;
        public int TitleColumn; public int SavedTitleColumn;
        public int LinkColumn; public int SavedLinkColumn;
        public bool LinkVisible; public bool SavedLinkVisible;
        public int HistoryColumn; public int SavedHistoryColumn;
        public bool HistoryVisible; public bool SavedHistoryVisible;
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
        public string ColumnNameOld; public string SavedColumnNameOld;
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
        public string ControlDateTime; public string SavedControlDateTime;
        public string GridDateTime; public string SavedGridDateTime;
        public bool Aggregatable; public bool SavedAggregatable;
        public bool Computable; public bool SavedComputable;
        public string ChoicesText; public string SavedChoicesText;
        public string DefaultInput; public string SavedDefaultInput;
        public bool Own; public bool SavedOwn;
        public string FormName; public string SavedFormName;
        public string Validators; public string SavedValidators;
        public int DecimalPlaces; public int SavedDecimalPlaces;
        public decimal Min; public decimal SavedMin;
        public decimal Max; public decimal SavedMax;
        public decimal Step; public decimal SavedStep;
        public string StringFormat; public string SavedStringFormat;
        public string Unit; public string SavedUnit;
        public int Width; public int SavedWidth;
        public bool SettingEnable; public bool SavedSettingEnable;

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
            if (propertyCollection.ContainsKey("GridVisible")) GridVisible = propertyCollection["GridVisible"].ToBool(); else GridVisible = false;
            if (propertyCollection.ContainsKey("FilterColumn")) FilterColumn = propertyCollection["FilterColumn"].ToInt(); else FilterColumn = 0;
            if (propertyCollection.ContainsKey("FilterVisible")) FilterVisible = propertyCollection["FilterVisible"].ToBool(); else FilterVisible = false;
            if (propertyCollection.ContainsKey("EditorColumn")) EditorColumn = propertyCollection["EditorColumn"].ToBool(); else EditorColumn = false;
            if (propertyCollection.ContainsKey("EditorVisible")) EditorVisible = propertyCollection["EditorVisible"].ToBool(); else EditorVisible = false;
            if (propertyCollection.ContainsKey("TitleColumn")) TitleColumn = propertyCollection["TitleColumn"].ToInt(); else TitleColumn = 0;
            if (propertyCollection.ContainsKey("LinkColumn")) LinkColumn = propertyCollection["LinkColumn"].ToInt(); else LinkColumn = 0;
            if (propertyCollection.ContainsKey("LinkVisible")) LinkVisible = propertyCollection["LinkVisible"].ToBool(); else LinkVisible = false;
            if (propertyCollection.ContainsKey("HistoryColumn")) HistoryColumn = propertyCollection["HistoryColumn"].ToInt(); else HistoryColumn = 0;
            if (propertyCollection.ContainsKey("HistoryVisible")) HistoryVisible = propertyCollection["HistoryVisible"].ToBool(); else HistoryVisible = false;
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
            if (propertyCollection.ContainsKey("ColumnNameOld")) ColumnNameOld = propertyCollection["ColumnNameOld"].ToString(); else ColumnNameOld = string.Empty;
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
            if (propertyCollection.ContainsKey("ControlDateTime")) ControlDateTime = propertyCollection["ControlDateTime"].ToString(); else ControlDateTime = string.Empty;
            if (propertyCollection.ContainsKey("GridDateTime")) GridDateTime = propertyCollection["GridDateTime"].ToString(); else GridDateTime = string.Empty;
            if (propertyCollection.ContainsKey("Aggregatable")) Aggregatable = propertyCollection["Aggregatable"].ToBool(); else Aggregatable = false;
            if (propertyCollection.ContainsKey("Computable")) Computable = propertyCollection["Computable"].ToBool(); else Computable = false;
            if (propertyCollection.ContainsKey("ChoicesText")) ChoicesText = propertyCollection["ChoicesText"].ToString(); else ChoicesText = string.Empty;
            if (propertyCollection.ContainsKey("DefaultInput")) DefaultInput = propertyCollection["DefaultInput"].ToString(); else DefaultInput = string.Empty;
            if (propertyCollection.ContainsKey("Own")) Own = propertyCollection["Own"].ToBool(); else Own = false;
            if (propertyCollection.ContainsKey("FormName")) FormName = propertyCollection["FormName"].ToString(); else FormName = string.Empty;
            if (propertyCollection.ContainsKey("Validators")) Validators = propertyCollection["Validators"].ToString(); else Validators = string.Empty;
            if (propertyCollection.ContainsKey("DecimalPlaces")) DecimalPlaces = propertyCollection["DecimalPlaces"].ToInt(); else DecimalPlaces = 0;
            if (propertyCollection.ContainsKey("Min")) Min = propertyCollection["Min"].ToDecimal(); else Min = 0;
            if (propertyCollection.ContainsKey("Max")) Max = propertyCollection["Max"].ToDecimal(); else Max = 0;
            if (propertyCollection.ContainsKey("Step")) Step = propertyCollection["Step"].ToDecimal(); else Step = 0;
            if (propertyCollection.ContainsKey("StringFormat")) StringFormat = propertyCollection["StringFormat"].ToString(); else StringFormat = string.Empty;
            if (propertyCollection.ContainsKey("Unit")) Unit = propertyCollection["Unit"].ToString(); else Unit = string.Empty;
            if (propertyCollection.ContainsKey("Width")) Width = propertyCollection["Width"].ToInt(); else Width = 0;
            if (propertyCollection.ContainsKey("SettingEnable")) SettingEnable = propertyCollection["SettingEnable"].ToBool(); else SettingEnable = false;
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
                    case "GridVisible": return GridVisible;
                    case "FilterColumn": return FilterColumn;
                    case "FilterVisible": return FilterVisible;
                    case "EditorColumn": return EditorColumn;
                    case "EditorVisible": return EditorVisible;
                    case "TitleColumn": return TitleColumn;
                    case "LinkColumn": return LinkColumn;
                    case "LinkVisible": return LinkVisible;
                    case "HistoryColumn": return HistoryColumn;
                    case "HistoryVisible": return HistoryVisible;
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
                    case "ColumnNameOld": return ColumnNameOld;
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
                    case "ControlDateTime": return ControlDateTime;
                    case "GridDateTime": return GridDateTime;
                    case "Aggregatable": return Aggregatable;
                    case "Computable": return Computable;
                    case "ChoicesText": return ChoicesText;
                    case "DefaultInput": return DefaultInput;
                    case "Own": return Own;
                    case "FormName": return FormName;
                    case "Validators": return Validators;
                    case "DecimalPlaces": return DecimalPlaces;
                    case "Min": return Min;
                    case "Max": return Max;
                    case "Step": return Step;
                    case "StringFormat": return StringFormat;
                    case "Unit": return Unit;
                    case "Width": return Width;
                    case "SettingEnable": return SettingEnable;
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
            GridVisible = SavedGridVisible;
            FilterColumn = SavedFilterColumn;
            FilterVisible = SavedFilterVisible;
            EditorColumn = SavedEditorColumn;
            EditorVisible = SavedEditorVisible;
            TitleColumn = SavedTitleColumn;
            LinkColumn = SavedLinkColumn;
            LinkVisible = SavedLinkVisible;
            HistoryColumn = SavedHistoryColumn;
            HistoryVisible = SavedHistoryVisible;
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
            ColumnNameOld = SavedColumnNameOld;
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
            ControlDateTime = SavedControlDateTime;
            GridDateTime = SavedGridDateTime;
            Aggregatable = SavedAggregatable;
            Computable = SavedComputable;
            ChoicesText = SavedChoicesText;
            DefaultInput = SavedDefaultInput;
            Own = SavedOwn;
            FormName = SavedFormName;
            Validators = SavedValidators;
            DecimalPlaces = SavedDecimalPlaces;
            Min = SavedMin;
            Max = SavedMax;
            Step = SavedStep;
            StringFormat = SavedStringFormat;
            Unit = SavedUnit;
            Width = SavedWidth;
            SettingEnable = SavedSettingEnable;
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
        public string Depts_ParentDeptId;
        public string Depts_ParentDept;
        public string Depts_DeptCode;
        public string Depts_Dept;
        public string Depts_DeptName;
        public string Depts_Body;
        public string Depts_Title;
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
        public string Users_Sex;
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
        public string Users_TenantAdmin;
        public string Users_ServiceAdmin;
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
        public string Permissions_UserId;
        public string Permissions_DeptName;
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
        public string Items_Subset;
        public string Items_MaintenanceTarget;
        public string Items_Site;
        public string Sites_TenantId;
        public string Sites_SiteId;
        public string Sites_ReferenceType;
        public string Sites_ParentId;
        public string Sites_InheritPermission;
        public string Sites_PermissionType;
        public string Sites_SiteSettings;
        public string Sites_Ancestors;
        public string Sites_PermissionSourceCollection;
        public string Sites_PermissionDestinationCollection;
        public string Sites_SiteMenu;
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
        public ColumnDefinition Depts_ParentDeptId = new ColumnDefinition();
        public ColumnDefinition Depts_ParentDept = new ColumnDefinition();
        public ColumnDefinition Depts_DeptCode = new ColumnDefinition();
        public ColumnDefinition Depts_Dept = new ColumnDefinition();
        public ColumnDefinition Depts_DeptName = new ColumnDefinition();
        public ColumnDefinition Depts_Body = new ColumnDefinition();
        public ColumnDefinition Depts_Title = new ColumnDefinition();
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
        public ColumnDefinition Users_Sex = new ColumnDefinition();
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
        public ColumnDefinition Users_TenantAdmin = new ColumnDefinition();
        public ColumnDefinition Users_ServiceAdmin = new ColumnDefinition();
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
        public ColumnDefinition Permissions_UserId = new ColumnDefinition();
        public ColumnDefinition Permissions_DeptName = new ColumnDefinition();
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
        public ColumnDefinition Items_Subset = new ColumnDefinition();
        public ColumnDefinition Items_MaintenanceTarget = new ColumnDefinition();
        public ColumnDefinition Items_Site = new ColumnDefinition();
        public ColumnDefinition Sites_TenantId = new ColumnDefinition();
        public ColumnDefinition Sites_SiteId = new ColumnDefinition();
        public ColumnDefinition Sites_ReferenceType = new ColumnDefinition();
        public ColumnDefinition Sites_ParentId = new ColumnDefinition();
        public ColumnDefinition Sites_InheritPermission = new ColumnDefinition();
        public ColumnDefinition Sites_PermissionType = new ColumnDefinition();
        public ColumnDefinition Sites_SiteSettings = new ColumnDefinition();
        public ColumnDefinition Sites_Ancestors = new ColumnDefinition();
        public ColumnDefinition Sites_PermissionSourceCollection = new ColumnDefinition();
        public ColumnDefinition Sites_PermissionDestinationCollection = new ColumnDefinition();
        public ColumnDefinition Sites_SiteMenu = new ColumnDefinition();
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
        public string _dot_header;
        public string _dot_footer;
        public string _dot_footer_space_a;
        public string html;
        public string body;
        public string h1;
        public string h2;
        public string h3;
        public string form;
        public string legend;
        public string legend_space___space__asterisk_;
        public string label;
        public string input;
        public string select;
        public string table;
        public string td;
        public string pre;
        public string _dot_logo;
        public string _dot_logo_corp;
        public string _dot_logo_product;
        public string _dot_login_user;
        public string _dot_login_user_space___space_p;
        public string _dot_login_user_space___space_p_space___space_span;
        public string _dot_head;
        public string _dot_main_form_space___space_nav_colon_first_child;
        public string _dot_nav_breadcrumbs;
        public string _dot_nav_breadcrumb;
        public string _dot_nav_breadcrumb_separator;
        public string _dot_nav_functions;
        public string _dot_nav_function;
        public string _dot_nav_function_space___space__asterisk_;
        public string _dot_nav_function_space___space__dot_ui_icon;
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
        public string _dot_nav_site_dot_to_upper;
        public string _dot_nav_site_dot_to_upper_space_a;
        public string _dot_nav_site_dot_to_upper_dot_has_image_space_a;
        public string _dot_nav_site_dot_to_upper_space__dot_ui_icon;
        public string _dot_nav_site_space__dot_site_image_thumbnail;
        public string _dot_nav_site_space__dot_site_image_icon;
        public string _dot_login;
        public string _dot_login_commands;
        public string _dot_demo;
        public string _dot_demo_space___space_fieldset_space___space_div;
        public string _dot_search_results;
        public string _dot_search_results_space__dot_count;
        public string _dot_search_results_space__dot_count_space__dot_label;
        public string _dot_search_results_space__dot_count_space__dot_data;
        public string _dot_search_results_space__dot_result;
        public string _dot_search_results_space__dot_result_space___space_h3;
        public string _dot_search_results_space__dot_result_space___space_p;
        public string _dot_search_results_space__dot_result_colon_hover;
        public string _dot_error_page;
        public string _dot_error_page_title;
        public string _dot_error_page_body;
        public string _dot_error_page_message;
        public string _dot_error_page_action;
        public string _dot_error_page_action_space_em;
        public string _dot_error_page_stacktrace;
        public string _sharp_application;
        public string _sharp_application_space___space__dot_site_image_icon;
        public string _dot_application_title;
        public string _dot_application_title_space___space__asterisk_;
        public string _dot_record_header;
        public string _dot_record_header_space___space_div;
        public string _dot_record_info;
        public string _dot_record_info_space_div;
        public string _dot_record_info_space_div_space_p;
        public string _dot_record_histories;
        public string _dot_record_switchers;
        public string _dot_record_switchers_space___space__asterisk_;
        public string _dot_record_switchers_space__dot_current;
        public string _dot_edit_form;
        public string _dot_edit_form_tabs;
        public string _dot_edit_form_tabs_max;
        public string _dot_edit_form_comments;
        public string _dot_edit_form_comments_space__dot_title_header;
        public string _dot_edit_form_mail_list;
        public string _dot_field_tab;
        public string _dot_fieldset;
        public string _dot_fieldset_dot_enclosed;
        public string _dot_fieldset_dot_enclosed_thin;
        public string _dot_fieldset_dot_enclosed_thin_space__class_asterisk__equal__yen_field_auto_yen__;
        public string _dot_fieldset_dot_enclosed_auto;
        public string _dot_fieldset_class_caret__equal__yen_enclosed_yen___space___space_legend;
        public string _dot_command_main_container;
        public string _dot_command_main;
        public string _dot_command_main_space___space_button;
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
        public string _dot_control_textarea;
        public string _dot_control_markup;
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
        public string _dot_container_selectable_space__dot_control;
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
        public string _dot_tooltip;
        public string _dot_no_border;
        public string _dot_mail_list;
        public string _dot_mail_list_space___space__dot_mail_content;
        public string _dot_dataview_filters;
        public string _dot_dataview_filters_space___space_div_space___space_div;
        public string _dot_dataview_filters_space___space_div_space___space_button;
        public string _dot_dataview_selector;
        public string _dot_dataview_selector_space___space_button;
        public string _dot_aggregations;
        public string _dot_aggregations_space__dot_label;
        public string _dot_aggregations_space__dot_data;
        public string _dot_aggregations_space_em;
        public string _dot_grid;
        public string _dot_grid_dot_fixed;
        public string _dot_grid_space_caption;
        public string _dot_grid_space_th;
        public string _dot_grid_space_th_space___space_div;
        public string _dot_grid_space_th_space_span;
        public string _dot_grid_space_th_colon_first_child;
        public string _dot_grid_space_th_colon_last_child;
        public string _dot_grid_space_th_dot_sortable_colon_hover;
        public string _dot_grid_space_td;
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
        public string _dot_gantt_chart;
        public string _dot_gantt;
        public string _dot_gantt_space__dot_date;
        public string _dot_gantt_space__dot_now;
        public string _dot_gantt_space__dot_planned_space_rect;
        public string _dot_gantt_space__dot_earned_space_rect;
        public string _dot_gantt_space_rect_dot_delay;
        public string _dot_gantt_space_rect_dot_completed;
        public string _dot_gantt_space__dot_title_space_text;
        public string _dot_gantt_space__dot_title_space_text_dot_delay;
        public string _dot_gantt_axis;
        public string _dot_gantt_axis_space_text;
        public string _dot_burn_down;
        public string _dot_burn_down_space__dot_now;
        public string _dot_burn_down_space__dot_total;
        public string _dot_burn_down_space__dot_planned;
        public string _dot_burn_down_space__dot_earned;
        public string _dot_burn_down_space__dot_total_space_circle;
        public string _dot_burn_down_space__dot_planned_space_circle;
        public string _dot_burn_down_space__dot_earned_space_circle;
        public string _dot_burn_down_details_row_space_td;
        public string _dot_burn_down_details_row_space_td_dot_warning;
        public string _dot_burn_down_details_row_space_td_dot_difference;
        public string _dot_burn_down_details_row_space_td_dot_difference_dot_warning;
        public string _dot_burn_down_record_details_space__dot_user_info;
        public string _dot_burn_down_record_details_space__dot_items;
        public string _dot_burn_down_record_details_space__dot_items_space_a;
        public string _dot_burn_down_record_details_space__dot_items_space_a_colon_hover;
        public string _dot_time_series;
        public string _dot_time_series_space__dot_surface;
        public string _dot_time_series_space__dot_index;
        public string _dot_kamban_row;
        public string _dot_kamban_container_dot_hover;
        public string _dot_kamban_container_space__dot_kamban_item_colon_last_child;
        public string _dot_kamban_item;
        public string _dot_kamban_item_colon_hover;
        public string _dot_kamban_item_dot_changed;
        public string _dot_kamban_item_space__dot_ui_icon;
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
        public string _dot_grid_space__class_asterisk__equal__yen_status__yen__;
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
        public string _dot_button_space__dot_ui_button_icon_primary;
        public string _dot_ui_button_text;
        public string _dot_ui_dialog;
        public string _dot_ui_icon_dot_a;
        public string _dot_ui_spinner;
        public string _dot_margin_bottom;
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
        public CssDefinition _dot_header = new CssDefinition();
        public CssDefinition _dot_footer = new CssDefinition();
        public CssDefinition _dot_footer_space_a = new CssDefinition();
        public CssDefinition html = new CssDefinition();
        public CssDefinition body = new CssDefinition();
        public CssDefinition h1 = new CssDefinition();
        public CssDefinition h2 = new CssDefinition();
        public CssDefinition h3 = new CssDefinition();
        public CssDefinition form = new CssDefinition();
        public CssDefinition legend = new CssDefinition();
        public CssDefinition legend_space___space__asterisk_ = new CssDefinition();
        public CssDefinition label = new CssDefinition();
        public CssDefinition input = new CssDefinition();
        public CssDefinition select = new CssDefinition();
        public CssDefinition table = new CssDefinition();
        public CssDefinition td = new CssDefinition();
        public CssDefinition pre = new CssDefinition();
        public CssDefinition _dot_logo = new CssDefinition();
        public CssDefinition _dot_logo_corp = new CssDefinition();
        public CssDefinition _dot_logo_product = new CssDefinition();
        public CssDefinition _dot_login_user = new CssDefinition();
        public CssDefinition _dot_login_user_space___space_p = new CssDefinition();
        public CssDefinition _dot_login_user_space___space_p_space___space_span = new CssDefinition();
        public CssDefinition _dot_head = new CssDefinition();
        public CssDefinition _dot_main_form_space___space_nav_colon_first_child = new CssDefinition();
        public CssDefinition _dot_nav_breadcrumbs = new CssDefinition();
        public CssDefinition _dot_nav_breadcrumb = new CssDefinition();
        public CssDefinition _dot_nav_breadcrumb_separator = new CssDefinition();
        public CssDefinition _dot_nav_functions = new CssDefinition();
        public CssDefinition _dot_nav_function = new CssDefinition();
        public CssDefinition _dot_nav_function_space___space__asterisk_ = new CssDefinition();
        public CssDefinition _dot_nav_function_space___space__dot_ui_icon = new CssDefinition();
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
        public CssDefinition _dot_nav_site_dot_to_upper = new CssDefinition();
        public CssDefinition _dot_nav_site_dot_to_upper_space_a = new CssDefinition();
        public CssDefinition _dot_nav_site_dot_to_upper_dot_has_image_space_a = new CssDefinition();
        public CssDefinition _dot_nav_site_dot_to_upper_space__dot_ui_icon = new CssDefinition();
        public CssDefinition _dot_nav_site_space__dot_site_image_thumbnail = new CssDefinition();
        public CssDefinition _dot_nav_site_space__dot_site_image_icon = new CssDefinition();
        public CssDefinition _dot_login = new CssDefinition();
        public CssDefinition _dot_login_commands = new CssDefinition();
        public CssDefinition _dot_demo = new CssDefinition();
        public CssDefinition _dot_demo_space___space_fieldset_space___space_div = new CssDefinition();
        public CssDefinition _dot_search_results = new CssDefinition();
        public CssDefinition _dot_search_results_space__dot_count = new CssDefinition();
        public CssDefinition _dot_search_results_space__dot_count_space__dot_label = new CssDefinition();
        public CssDefinition _dot_search_results_space__dot_count_space__dot_data = new CssDefinition();
        public CssDefinition _dot_search_results_space__dot_result = new CssDefinition();
        public CssDefinition _dot_search_results_space__dot_result_space___space_h3 = new CssDefinition();
        public CssDefinition _dot_search_results_space__dot_result_space___space_p = new CssDefinition();
        public CssDefinition _dot_search_results_space__dot_result_colon_hover = new CssDefinition();
        public CssDefinition _dot_error_page = new CssDefinition();
        public CssDefinition _dot_error_page_title = new CssDefinition();
        public CssDefinition _dot_error_page_body = new CssDefinition();
        public CssDefinition _dot_error_page_message = new CssDefinition();
        public CssDefinition _dot_error_page_action = new CssDefinition();
        public CssDefinition _dot_error_page_action_space_em = new CssDefinition();
        public CssDefinition _dot_error_page_stacktrace = new CssDefinition();
        public CssDefinition _sharp_application = new CssDefinition();
        public CssDefinition _sharp_application_space___space__dot_site_image_icon = new CssDefinition();
        public CssDefinition _dot_application_title = new CssDefinition();
        public CssDefinition _dot_application_title_space___space__asterisk_ = new CssDefinition();
        public CssDefinition _dot_record_header = new CssDefinition();
        public CssDefinition _dot_record_header_space___space_div = new CssDefinition();
        public CssDefinition _dot_record_info = new CssDefinition();
        public CssDefinition _dot_record_info_space_div = new CssDefinition();
        public CssDefinition _dot_record_info_space_div_space_p = new CssDefinition();
        public CssDefinition _dot_record_histories = new CssDefinition();
        public CssDefinition _dot_record_switchers = new CssDefinition();
        public CssDefinition _dot_record_switchers_space___space__asterisk_ = new CssDefinition();
        public CssDefinition _dot_record_switchers_space__dot_current = new CssDefinition();
        public CssDefinition _dot_edit_form = new CssDefinition();
        public CssDefinition _dot_edit_form_tabs = new CssDefinition();
        public CssDefinition _dot_edit_form_tabs_max = new CssDefinition();
        public CssDefinition _dot_edit_form_comments = new CssDefinition();
        public CssDefinition _dot_edit_form_comments_space__dot_title_header = new CssDefinition();
        public CssDefinition _dot_edit_form_mail_list = new CssDefinition();
        public CssDefinition _dot_field_tab = new CssDefinition();
        public CssDefinition _dot_fieldset = new CssDefinition();
        public CssDefinition _dot_fieldset_dot_enclosed = new CssDefinition();
        public CssDefinition _dot_fieldset_dot_enclosed_thin = new CssDefinition();
        public CssDefinition _dot_fieldset_dot_enclosed_thin_space__class_asterisk__equal__yen_field_auto_yen__ = new CssDefinition();
        public CssDefinition _dot_fieldset_dot_enclosed_auto = new CssDefinition();
        public CssDefinition _dot_fieldset_class_caret__equal__yen_enclosed_yen___space___space_legend = new CssDefinition();
        public CssDefinition _dot_command_main_container = new CssDefinition();
        public CssDefinition _dot_command_main = new CssDefinition();
        public CssDefinition _dot_command_main_space___space_button = new CssDefinition();
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
        public CssDefinition _dot_control_textarea = new CssDefinition();
        public CssDefinition _dot_control_markup = new CssDefinition();
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
        public CssDefinition _dot_container_selectable_space__dot_control = new CssDefinition();
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
        public CssDefinition _dot_tooltip = new CssDefinition();
        public CssDefinition _dot_no_border = new CssDefinition();
        public CssDefinition _dot_mail_list = new CssDefinition();
        public CssDefinition _dot_mail_list_space___space__dot_mail_content = new CssDefinition();
        public CssDefinition _dot_dataview_filters = new CssDefinition();
        public CssDefinition _dot_dataview_filters_space___space_div_space___space_div = new CssDefinition();
        public CssDefinition _dot_dataview_filters_space___space_div_space___space_button = new CssDefinition();
        public CssDefinition _dot_dataview_selector = new CssDefinition();
        public CssDefinition _dot_dataview_selector_space___space_button = new CssDefinition();
        public CssDefinition _dot_aggregations = new CssDefinition();
        public CssDefinition _dot_aggregations_space__dot_label = new CssDefinition();
        public CssDefinition _dot_aggregations_space__dot_data = new CssDefinition();
        public CssDefinition _dot_aggregations_space_em = new CssDefinition();
        public CssDefinition _dot_grid = new CssDefinition();
        public CssDefinition _dot_grid_dot_fixed = new CssDefinition();
        public CssDefinition _dot_grid_space_caption = new CssDefinition();
        public CssDefinition _dot_grid_space_th = new CssDefinition();
        public CssDefinition _dot_grid_space_th_space___space_div = new CssDefinition();
        public CssDefinition _dot_grid_space_th_space_span = new CssDefinition();
        public CssDefinition _dot_grid_space_th_colon_first_child = new CssDefinition();
        public CssDefinition _dot_grid_space_th_colon_last_child = new CssDefinition();
        public CssDefinition _dot_grid_space_th_dot_sortable_colon_hover = new CssDefinition();
        public CssDefinition _dot_grid_space_td = new CssDefinition();
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
        public CssDefinition _dot_gantt_chart = new CssDefinition();
        public CssDefinition _dot_gantt = new CssDefinition();
        public CssDefinition _dot_gantt_space__dot_date = new CssDefinition();
        public CssDefinition _dot_gantt_space__dot_now = new CssDefinition();
        public CssDefinition _dot_gantt_space__dot_planned_space_rect = new CssDefinition();
        public CssDefinition _dot_gantt_space__dot_earned_space_rect = new CssDefinition();
        public CssDefinition _dot_gantt_space_rect_dot_delay = new CssDefinition();
        public CssDefinition _dot_gantt_space_rect_dot_completed = new CssDefinition();
        public CssDefinition _dot_gantt_space__dot_title_space_text = new CssDefinition();
        public CssDefinition _dot_gantt_space__dot_title_space_text_dot_delay = new CssDefinition();
        public CssDefinition _dot_gantt_axis = new CssDefinition();
        public CssDefinition _dot_gantt_axis_space_text = new CssDefinition();
        public CssDefinition _dot_burn_down = new CssDefinition();
        public CssDefinition _dot_burn_down_space__dot_now = new CssDefinition();
        public CssDefinition _dot_burn_down_space__dot_total = new CssDefinition();
        public CssDefinition _dot_burn_down_space__dot_planned = new CssDefinition();
        public CssDefinition _dot_burn_down_space__dot_earned = new CssDefinition();
        public CssDefinition _dot_burn_down_space__dot_total_space_circle = new CssDefinition();
        public CssDefinition _dot_burn_down_space__dot_planned_space_circle = new CssDefinition();
        public CssDefinition _dot_burn_down_space__dot_earned_space_circle = new CssDefinition();
        public CssDefinition _dot_burn_down_details_row_space_td = new CssDefinition();
        public CssDefinition _dot_burn_down_details_row_space_td_dot_warning = new CssDefinition();
        public CssDefinition _dot_burn_down_details_row_space_td_dot_difference = new CssDefinition();
        public CssDefinition _dot_burn_down_details_row_space_td_dot_difference_dot_warning = new CssDefinition();
        public CssDefinition _dot_burn_down_record_details_space__dot_user_info = new CssDefinition();
        public CssDefinition _dot_burn_down_record_details_space__dot_items = new CssDefinition();
        public CssDefinition _dot_burn_down_record_details_space__dot_items_space_a = new CssDefinition();
        public CssDefinition _dot_burn_down_record_details_space__dot_items_space_a_colon_hover = new CssDefinition();
        public CssDefinition _dot_time_series = new CssDefinition();
        public CssDefinition _dot_time_series_space__dot_surface = new CssDefinition();
        public CssDefinition _dot_time_series_space__dot_index = new CssDefinition();
        public CssDefinition _dot_kamban_row = new CssDefinition();
        public CssDefinition _dot_kamban_container_dot_hover = new CssDefinition();
        public CssDefinition _dot_kamban_container_space__dot_kamban_item_colon_last_child = new CssDefinition();
        public CssDefinition _dot_kamban_item = new CssDefinition();
        public CssDefinition _dot_kamban_item_colon_hover = new CssDefinition();
        public CssDefinition _dot_kamban_item_dot_changed = new CssDefinition();
        public CssDefinition _dot_kamban_item_space__dot_ui_icon = new CssDefinition();
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
        public CssDefinition _dot_grid_space__class_asterisk__equal__yen_status__yen__ = new CssDefinition();
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
        public CssDefinition _dot_button_space__dot_ui_button_icon_primary = new CssDefinition();
        public CssDefinition _dot_ui_button_text = new CssDefinition();
        public CssDefinition _dot_ui_dialog = new CssDefinition();
        public CssDefinition _dot_ui_icon_dot_a = new CssDefinition();
        public CssDefinition _dot_ui_spinner = new CssDefinition();
        public CssDefinition _dot_margin_bottom = new CssDefinition();
        public CssDefinition _dot_height_auto = new CssDefinition();
        public CssDefinition _dot_focus_inform = new CssDefinition();
        public CssDefinition _dot_sortable = new CssDefinition();
        public CssDefinition _dot_menu_sort = new CssDefinition();
        public CssDefinition _dot_menu_sort_space___space_li = new CssDefinition();
        public CssDefinition _dot_menu_sort_space___space_li_colon_hover = new CssDefinition();
        public CssDefinition _dot_menu_sort_space___space_li_space___space__asterisk_ = new CssDefinition();
    }

    public class DataViewDefinition
    {
        public string Id; public string SavedId;
        public string ReferenceType; public string SavedReferenceType;
        public string Name; public string SavedName;

        public DataViewDefinition()
        {
        }

        public DataViewDefinition(Dictionary<string, string> propertyCollection)
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

    public class DataViewColumn2nd
    {
        public string Issues_Gantt;
        public string Issues_BurnDown;
        public string Issues_TimeSeries;
        public string Issues_Kamban;
        public string Results_TimeSeries;
        public string Results_Kamban;
    }

    public class DataViewTable
    {
        public DataViewDefinition Issues_Gantt = new DataViewDefinition();
        public DataViewDefinition Issues_BurnDown = new DataViewDefinition();
        public DataViewDefinition Issues_TimeSeries = new DataViewDefinition();
        public DataViewDefinition Issues_Kamban = new DataViewDefinition();
        public DataViewDefinition Results_TimeSeries = new DataViewDefinition();
        public DataViewDefinition Results_Kamban = new DataViewDefinition();
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
        public string Manager; public string SavedManager;
        public string Owner; public string SavedOwner;
        public string ClassA; public string SavedClassA;
        public string ClassB; public string SavedClassB;
        public string ClassC; public string SavedClassC;
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
            if (propertyCollection.ContainsKey("Manager")) Manager = propertyCollection["Manager"].ToString(); else Manager = string.Empty;
            if (propertyCollection.ContainsKey("Owner")) Owner = propertyCollection["Owner"].ToString(); else Owner = string.Empty;
            if (propertyCollection.ContainsKey("ClassA")) ClassA = propertyCollection["ClassA"].ToString(); else ClassA = string.Empty;
            if (propertyCollection.ContainsKey("ClassB")) ClassB = propertyCollection["ClassB"].ToString(); else ClassB = string.Empty;
            if (propertyCollection.ContainsKey("ClassC")) ClassC = propertyCollection["ClassC"].ToString(); else ClassC = string.Empty;
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
                    case "Manager": return Manager;
                    case "Owner": return Owner;
                    case "ClassA": return ClassA;
                    case "ClassB": return ClassB;
                    case "ClassC": return ClassC;
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
            Manager = SavedManager;
            Owner = SavedOwner;
            ClassA = SavedClassA;
            ClassB = SavedClassB;
            ClassC = SavedClassC;
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
        public string Issue1;
        public string Issue2;
        public string Issue3;
        public string Issue4;
        public string Issue5;
        public string Issue6;
        public string Issue7;
        public string Issue8;
        public string Issue9;
        public string Issue10;
        public string Issue11;
        public string Issue12;
        public string Issue13;
        public string Issue14;
        public string Issue15;
        public string Result1;
        public string Comment1;
        public string Comment2;
        public string Comment3;
        public string Comment4;
        public string Comment5;
        public string Comment6;
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
        public DemoDefinition Issue1 = new DemoDefinition();
        public DemoDefinition Issue2 = new DemoDefinition();
        public DemoDefinition Issue3 = new DemoDefinition();
        public DemoDefinition Issue4 = new DemoDefinition();
        public DemoDefinition Issue5 = new DemoDefinition();
        public DemoDefinition Issue6 = new DemoDefinition();
        public DemoDefinition Issue7 = new DemoDefinition();
        public DemoDefinition Issue8 = new DemoDefinition();
        public DemoDefinition Issue9 = new DemoDefinition();
        public DemoDefinition Issue10 = new DemoDefinition();
        public DemoDefinition Issue11 = new DemoDefinition();
        public DemoDefinition Issue12 = new DemoDefinition();
        public DemoDefinition Issue13 = new DemoDefinition();
        public DemoDefinition Issue14 = new DemoDefinition();
        public DemoDefinition Issue15 = new DemoDefinition();
        public DemoDefinition Result1 = new DemoDefinition();
        public DemoDefinition Comment1 = new DemoDefinition();
        public DemoDefinition Comment2 = new DemoDefinition();
        public DemoDefinition Comment3 = new DemoDefinition();
        public DemoDefinition Comment4 = new DemoDefinition();
        public DemoDefinition Comment5 = new DemoDefinition();
        public DemoDefinition Comment6 = new DemoDefinition();
    }

    public class DisplayDefinition
    {
        public string Id; public string SavedId;
        public string Content; public string SavedContent;
        public string Type; public string SavedType;
        public string Name; public string SavedName;
        public string Language; public string SavedLanguage;
        public string CssClass; public string SavedCssClass;
        public bool ClientScript; public bool SavedClientScript;

        public DisplayDefinition()
        {
        }

        public DisplayDefinition(Dictionary<string, string> propertyCollection)
        {
            if (propertyCollection.ContainsKey("Id")) Id = propertyCollection["Id"].ToString(); else Id = string.Empty;
            if (propertyCollection.ContainsKey("Content")) Content = propertyCollection["Content"].ToString(); else Content = string.Empty;
            if (propertyCollection.ContainsKey("Type")) Type = propertyCollection["Type"].ToString(); else Type = string.Empty;
            if (propertyCollection.ContainsKey("Name")) Name = propertyCollection["Name"].ToString(); else Name = string.Empty;
            if (propertyCollection.ContainsKey("Language")) Language = propertyCollection["Language"].ToString(); else Language = string.Empty;
            if (propertyCollection.ContainsKey("CssClass")) CssClass = propertyCollection["CssClass"].ToString(); else CssClass = string.Empty;
            if (propertyCollection.ContainsKey("ClientScript")) ClientScript = propertyCollection["ClientScript"].ToBool(); else ClientScript = false;
        }

        public object this[string key]
        {
            get{
                switch(key)
                {
                    case "Id": return Id;
                    case "Content": return Content;
                    case "Type": return Type;
                    case "Name": return Name;
                    case "Language": return Language;
                    case "CssClass": return CssClass;
                    case "ClientScript": return ClientScript;
                    default: return null;
                }
            }
        }

        public void RestoreBySavedMemory()
        {
            Id = SavedId;
            Content = SavedContent;
            Type = SavedType;
            Name = SavedName;
            Language = SavedLanguage;
            CssClass = SavedCssClass;
            ClientScript = SavedClientScript;
        }
    }

    public class DisplayColumn2nd
    {
        public string Login;
        public string Login_ja;
        public string Top;
        public string Top_ja;
        public string List;
        public string List_ja;
        public string New;
        public string New_ja;
        public string EditSettings;
        public string EditSettings_ja;
        public string Create;
        public string Create_ja;
        public string Update;
        public string Update_ja;
        public string Save;
        public string Save_ja;
        public string Change;
        public string Change_ja;
        public string Add;
        public string Add_ja;
        public string Register;
        public string Register_ja;
        public string Reset;
        public string Reset_ja;
        public string Copy;
        public string Copy_ja;
        public string Move;
        public string Move_ja;
        public string BulkMove;
        public string BulkMove_ja;
        public string Mail;
        public string Mail_ja;
        public string MailAddress;
        public string MailAddress_ja;
        public string SendMail;
        public string SendMail_ja;
        public string Delete;
        public string Delete_ja;
        public string Separate;
        public string Separate_ja;
        public string BulkDelete;
        public string BulkDelete_ja;
        public string Import;
        public string Import_ja;
        public string Export;
        public string Export_ja;
        public string Cancel;
        public string Cancel_ja;
        public string GoBack;
        public string GoBack_ja;
        public string Synchronize;
        public string Synchronize_ja;
        public string Operations;
        public string Operations_ja;
        public string GroupBy;
        public string GroupBy_ja;
        public string Date;
        public string Date_ja;
        public string Count;
        public string Count_ja;
        public string Total;
        public string Total_ja;
        public string Average;
        public string Average_ja;
        public string Max;
        public string Max_ja;
        public string Min;
        public string Min_ja;
        public string Step;
        public string Step_ja;
        public string DecimalPlaces;
        public string DecimalPlaces_ja;
        public string ToUpper;
        public string ToUpper_ja;
        public string MoveUp;
        public string MoveUp_ja;
        public string MoveDown;
        public string MoveDown_ja;
        public string Previous;
        public string Previous_ja;
        public string Next;
        public string Next_ja;
        public string Reload;
        public string Reload_ja;
        public string Newer;
        public string Newer_ja;
        public string Older;
        public string Older_ja;
        public string Latest;
        public string Latest_ja;
        public string Visible;
        public string Visible_ja;
        public string Hide;
        public string Hide_ja;
        public string PlannedValue;
        public string PlannedValue_ja;
        public string EarnedValue;
        public string EarnedValue_ja;
        public string Difference;
        public string Difference_ja;
        public string ChangePassword;
        public string ChangePassword_ja;
        public string ResetPassword;
        public string ResetPassword_ja;
        public string ReadOnly;
        public string ReadOnly_ja;
        public string ReadWrite;
        public string ReadWrite_ja;
        public string Leader;
        public string Leader_ja;
        public string Manager;
        public string Manager_ja;
        public string DeletePermission;
        public string DeletePermission_ja;
        public string AddPermission;
        public string AddPermission_ja;
        public string NotInheritPermission;
        public string NotInheritPermission_ja;
        public string NoTitle;
        public string NoTitle_ja;
        public string Error;
        public string Error_ja;
        public string Index;
        public string Index_ja;
        public string Edit;
        public string Edit_ja;
        public string History;
        public string History_ja;
        public string Histories;
        public string Histories_ja;
        public string Links;
        public string Links_ja;
        public string LinkDestinations;
        public string LinkDestinations_ja;
        public string LinkSources;
        public string LinkSources_ja;
        public string LinkCreations;
        public string LinkCreations_ja;
        public string Comments;
        public string Comments_ja;
        public string SentMail;
        public string SentMail_ja;
        public string Reply;
        public string Reply_ja;
        public string OriginalMessage;
        public string Logout;
        public string Logout_ja;
        public string EditProfile;
        public string EditProfile_ja;
        public string CreatedTime;
        public string CreatedTime_ja;
        public string UpdatedTime;
        public string UpdatedTime_ja;
        public string PermissionDestination;
        public string PermissionDestination_ja;
        public string PermissionSource;
        public string PermissionSource_ja;
        public string SettingColumnList;
        public string SettingColumnList_ja;
        public string SettingTitleColumn;
        public string SettingTitleColumn_ja;
        public string SettingTitleSeparator;
        public string SettingTitleSeparator_ja;
        public string SettingAggregationList;
        public string SettingAggregationList_ja;
        public string SettingNotGroupBy;
        public string SettingNotGroupBy_ja;
        public string SettingNearCompletionTimeBeforeDays;
        public string SettingNearCompletionTimeBeforeDays_ja;
        public string SettingNearCompletionTimeAfterDays;
        public string SettingNearCompletionTimeAfterDays_ja;
        public string SettingGridPageSize;
        public string SettingGridPageSize_ja;
        public string SettingLabel;
        public string SettingLabel_ja;
        public string SettingEnable;
        public string SettingEnable_ja;
        public string SettingOrder;
        public string SettingOrder_ja;
        public string SettingUnit;
        public string SettingUnit_ja;
        public string SettingControlDateTime;
        public string SettingControlDateTime_ja;
        public string SettingGridDateTime;
        public string SettingGridDateTime_ja;
        public string SettingSelectionList;
        public string SettingSelectionList_ja;
        public string SettingChoicesVisible;
        public string SettingChoicesVisible_ja;
        public string SettingLimitDefault;
        public string SettingLimitDefault_ja;
        public string SettingGridColumns;
        public string SettingGridColumns_ja;
        public string SettingFilterColumns;
        public string SettingFilterColumns_ja;
        public string SettingSummaryColumns;
        public string SettingSummaryColumns_ja;
        public string SettingEditorColumns;
        public string SettingEditorColumns_ja;
        public string SettingLinkColumns;
        public string SettingLinkColumns_ja;
        public string SettingHistoryColumns;
        public string SettingHistoryColumns_ja;
        public string SettingFormulas;
        public string SettingFormulas_ja;
        public string SettingAggregations;
        public string SettingAggregations_ja;
        public string SettingAggregationType;
        public string SettingAggregationType_ja;
        public string SettingAggregationTarget;
        public string SettingAggregationTarget_ja;
        public string CurrentPassword;
        public string CurrentPassword_ja;
        public string ReEnter;
        public string ReEnter_ja;
        public string VerUp;
        public string VerUp_ja;
        public string Hidden;
        public string Hidden_ja;
        public string Output;
        public string Output_ja;
        public string NotOutput;
        public string NotOutput_ja;
        public string CopyWithComments;
        public string CopyWithComments_ja;
        public string Hyphen;
        public string Search;
        public string Search_ja;
        public string Admin;
        public string Admin_ja;
        public string Default;
        public string Default_ja;
        public string DefaultInput;
        public string DefaultInput_ja;
        public string AddressBook;
        public string AddressBook_ja;
        public string DefaultAddressBook;
        public string DefaultAddressBook_ja;
        public string DefaultDestinations;
        public string DefaultDestinations_ja;
        public string LimitJust;
        public string LimitJust_ja;
        public string LimitAfterSecond;
        public string LimitAfterSecond_ja;
        public string LimitAfterSeconds;
        public string LimitAfterSeconds_ja;
        public string LimitAfterMinute;
        public string LimitAfterMinute_ja;
        public string LimitAfterMinutes;
        public string LimitAfterMinutes_ja;
        public string LimitAfterHour;
        public string LimitAfterHour_ja;
        public string LimitAfterHours;
        public string LimitAfterHours_ja;
        public string LimitAfterDay;
        public string LimitAfterDay_ja;
        public string LimitAfterDays;
        public string LimitAfterDays_ja;
        public string LimitAfterMonth;
        public string LimitAfterMonth_ja;
        public string LimitAfterMonths;
        public string LimitAfterMonths_ja;
        public string LimitAfterYear;
        public string LimitAfterYear_ja;
        public string LimitAfterYears;
        public string LimitAfterYears_ja;
        public string LimitBeforeSecond;
        public string LimitBeforeSecond_ja;
        public string LimitBeforeSeconds;
        public string LimitBeforeSeconds_ja;
        public string LimitBeforeMinute;
        public string LimitBeforeMinute_ja;
        public string LimitBeforeMinutes;
        public string LimitBeforeMinutes_ja;
        public string LimitBeforeHour;
        public string LimitBeforeHour_ja;
        public string LimitBeforeHours;
        public string LimitBeforeHours_ja;
        public string LimitBeforeDay;
        public string LimitBeforeDay_ja;
        public string LimitBeforeDays;
        public string LimitBeforeDays_ja;
        public string LimitBeforeMonth;
        public string LimitBeforeMonth_ja;
        public string LimitBeforeMonths;
        public string LimitBeforeMonths_ja;
        public string LimitBeforeYear;
        public string LimitBeforeYear_ja;
        public string LimitBeforeYears;
        public string LimitBeforeYears_ja;
        public string WriteComment;
        public string WriteComment_ja;
        public string Filters;
        public string Filters_ja;
        public string Incomplete;
        public string Incomplete_ja;
        public string Blank;
        public string Blank_ja;
        public string Own;
        public string Own_ja;
        public string NearCompletionTime;
        public string NearCompletionTime_ja;
        public string Delay;
        public string Delay_ja;
        public string Overdue;
        public string Overdue_ja;
        public string OrderAsc;
        public string OrderAsc_ja;
        public string OrderDesc;
        public string OrderDesc_ja;
        public string OrderRelease;
        public string OrderRelease_ja;
        public string ResetOrder;
        public string ResetOrder_ja;
        public string Csv;
        public string Csv_ja;
        public string CsvFile;
        public string CsvFile_ja;
        public string CharacterCode;
        public string CharacterCode_ja;
        public string Excel;
        public string Excel_ja;
        public string Setting;
        public string Setting_ja;
        public string AdvancedSetting;
        public string AdvancedSetting_ja;
        public string Basic;
        public string Basic_ja;
        public string Normal;
        public string Normal_ja;
        public string Wide;
        public string Wide_ja;
        public string Auto;
        public string Auto_ja;
        public string ControlType;
        public string ControlType_ja;
        public string Spinner;
        public string Spinner_ja;
        public string SiteImageSettingsEditor;
        public string SiteImageSettingsEditor_ja;
        public string GridSettingsEditor;
        public string GridSettingsEditor_ja;
        public string EditorSettingsEditor;
        public string EditorSettingsEditor_ja;
        public string MailerSettingsEditor;
        public string MailerSettingsEditor_ja;
        public string StyleSettingsEditor;
        public string StyleSettingsEditor_ja;
        public string ScriptSettingsEditor;
        public string ScriptSettingsEditor_ja;
        public string SummarySettingsEditor;
        public string SummarySettingsEditor_ja;
        public string PermissionSetting;
        public string PermissionSetting_ja;
        public string EditPermissions;
        public string EditPermissions_ja;
        public string CopySettings;
        public string CopySettings_ja;
        public string AggregationDetails;
        public string AggregationDetails_ja;
        public string MoveSettings;
        public string MoveSettings_ja;
        public string Destination;
        public string Destination_ja;
        public string SeparateSettings;
        public string SeparateSettings_ja;
        public string SummarySiteId;
        public string SummarySiteId_ja;
        public string SummaryDestinationColumn;
        public string SummaryDestinationColumn_ja;
        public string SummaryLinkColumn;
        public string SummaryLinkColumn_ja;
        public string SummaryType;
        public string SummaryType_ja;
        public string SummarySourceColumn;
        public string SummarySourceColumn_ja;
        public string Title;
        public string Title_ja;
        public string WorkValue;
        public string WorkValue_ja;
        public string SeparateNumber;
        public string SeparateNumber_ja;
        public string OutgoingMail;
        public string OutgoingMail_ja;
        public string Icon;
        public string Icon_ja;
        public string File;
        public string File_ja;
        public string NotSet;
        public string NotSet_ja;
        public string SiteUser;
        public string SiteUser_ja;
        public string All;
        public string All_ja;
        public string Quantity;
        public string Quantity_ja;
        public string SuffixCopy;
        public string SuffixCopy_ja;
        public string DataViewSelector;
        public string DataViewSelector_ja;
        public string Grid;
        public string Grid_ja;
        public string Style;
        public string Style_ja;
        public string GridStyle;
        public string GridStyle_ja;
        public string NewStyle;
        public string NewStyle_ja;
        public string EditStyle;
        public string EditStyle_ja;
        public string Script;
        public string Script_ja;
        public string GridScript;
        public string GridScript_ja;
        public string NewScript;
        public string NewScript_ja;
        public string EditScript;
        public string EditScript_ja;
        public string BurnDown;
        public string BurnDown_ja;
        public string Gantt;
        public string Gantt_ja;
        public string TimeSeries;
        public string TimeSeries_ja;
        public string Kamban;
        public string Kamban_ja;
        public string NoData;
        public string NoData_ja;
        public string NoLinks;
        public string NoLinks_ja;
        public string ViewDemoEnvironment;
        public string ViewDemoEnvironment_ja;
        public string DemoMailTitle;
        public string DemoMailTitle_ja;
        public string DemoMailBody;
        public string DemoMailBody_ja;
        public string Fy;
        public string Fy_ja;
        public string Half1;
        public string Half1_ja;
        public string Half2;
        public string Half2_ja;
        public string Quarter;
        public string Quarter_ja;
        public string Ymd;
        public string Ymd_ja;
        public string Ymda;
        public string Ymda_ja;
        public string Ym;
        public string Ym_ja;
        public string Ymdhms;
        public string Ymdhms_ja;
        public string Ymdahms;
        public string Ymdahms_ja;
        public string Ymdhm;
        public string Ymdhm_ja;
        public string Ymdahm;
        public string Ymdahm_ja;
        public string YmdFormat;
        public string YmdFormat_ja;
        public string YmdaFormat;
        public string YmdaFormat_ja;
        public string YmFormat;
        public string YmFormat_ja;
        public string YmdhmsFormat;
        public string YmdhmsFormat_ja;
        public string YmdahmsFormat;
        public string YmdahmsFormat_ja;
        public string YmdhmFormat;
        public string YmdhmFormat_ja;
        public string YmdahmFormat;
        public string YmdahmFormat_ja;
        public string ExceptionTitle;
        public string ExceptionTitle_ja;
        public string ExceptionBody;
        public string ExceptionBody_ja;
        public string InvalidRequest;
        public string InvalidRequest_ja;
        public string Authentication;
        public string Authentication_ja;
        public string PasswordNotChanged;
        public string PasswordNotChanged_ja;
        public string UpdateConflicts;
        public string UpdateConflicts_ja;
        public string DeleteConflicts;
        public string DeleteConflicts_ja;
        public string HasNotPermission;
        public string HasNotPermission_ja;
        public string IncorrectCurrentPassword;
        public string IncorrectCurrentPassword_ja;
        public string PermissionNotSelfChange;
        public string PermissionNotSelfChange_ja;
        public string DefinitionNotFound;
        public string DefinitionNotFound_ja;
        public string CantSetAtTopOfSite;
        public string CantSetAtTopOfSite_ja;
        public string NotFound;
        public string NotFound_ja;
        public string RequireMailAddresses;
        public string RequireMailAddresses_ja;
        public string RequireColumn;
        public string RequireColumn_ja;
        public string ExternalMailAddress;
        public string ExternalMailAddress_ja;
        public string BadMailAddress;
        public string BadMailAddress_ja;
        public string MailAddressHasNotSet;
        public string MailAddressHasNotSet_ja;
        public string FileNotFound;
        public string FileNotFound_ja;
        public string NotRequiredColumn;
        public string NotRequiredColumn_ja;
        public string InvalidCsvData;
        public string InvalidCsvData_ja;
        public string FailedReadFile;
        public string FailedReadFile_ja;
        public string CanNotHide;
        public string CanNotHide_ja;
        public string AlreadyAdded;
        public string AlreadyAdded_ja;
        public string InvalidFormula;
        public string InvalidFormula_ja;
        public string SelectTargets;
        public string SelectTargets_ja;
        public string LoginIn;
        public string LoginIn_ja;
        public string Deleted;
        public string Deleted_ja;
        public string BulkMoved;
        public string BulkMoved_ja;
        public string BulkDeleted;
        public string BulkDeleted_ja;
        public string Separated;
        public string Separated_ja;
        public string Imported;
        public string Imported_ja;
        public string Created;
        public string Created_ja;
        public string Updated;
        public string Updated_ja;
        public string CodeDefinerCompleted;
        public string CodeDefinerCompleted_ja;
        public string CodeDefinerRdsCompleted;
        public string CodeDefinerRdsCompleted_ja;
        public string CodeDefinerDefCompleted;
        public string CodeDefinerDefCompleted_ja;
        public string CodeDefinerMvcCompleted;
        public string CodeDefinerMvcCompleted_ja;
        public string CodeDefinerCssCompleted;
        public string CodeDefinerCssCompleted_ja;
        public string CodeDefinerBackupCompleted;
        public string CodeDefinerBackupCompleted_ja;
        public string MailTransmissionCompletion;
        public string MailTransmissionCompletion_ja;
        public string ChangingPasswordComplete;
        public string ChangingPasswordComplete_ja;
        public string PasswordResetCompleted;
        public string PasswordResetCompleted_ja;
        public string FileUpdateCompleted;
        public string FileUpdateCompleted_ja;
        public string SynchronizationCompleted;
        public string SynchronizationCompleted_ja;
        public string SentAcceptanceMail_space_;
        public string SentAcceptanceMail_space__ja;
        public string ConfirmDelete;
        public string ConfirmDelete_ja;
        public string ConfirmSeparate;
        public string ConfirmSeparate_ja;
        public string ConfirmSendMail;
        public string ConfirmSendMail_ja;
        public string ConfirmSynchronize;
        public string ConfirmSynchronize_ja;
        public string CanNotUpdate;
        public string CanNotUpdate_ja;
        public string ReadOnlyBecausePreviousVer;
        public string ReadOnlyBecausePreviousVer_ja;
        public string InCopying;
        public string InCopying_ja;
        public string InCompression;
        public string InCompression_ja;
        public string HasBeenMoved;
        public string HasBeenMoved_ja;
        public string HasBeenDeleted;
        public string HasBeenDeleted_ja;
        public string ValidateRequired;
        public string ValidateRequired_ja;
        public string ValidateNumber;
        public string ValidateNumber_ja;
        public string ValidateDate;
        public string ValidateDate_ja;
        public string ValidateMail;
        public string ValidateMail_ja;
        public string ValidateEqualTo;
        public string ValidateEqualTo_ja;
    }

    public class DisplayTable
    {
        public DisplayDefinition Login = new DisplayDefinition();
        public DisplayDefinition Login_ja = new DisplayDefinition();
        public DisplayDefinition Top = new DisplayDefinition();
        public DisplayDefinition Top_ja = new DisplayDefinition();
        public DisplayDefinition List = new DisplayDefinition();
        public DisplayDefinition List_ja = new DisplayDefinition();
        public DisplayDefinition New = new DisplayDefinition();
        public DisplayDefinition New_ja = new DisplayDefinition();
        public DisplayDefinition EditSettings = new DisplayDefinition();
        public DisplayDefinition EditSettings_ja = new DisplayDefinition();
        public DisplayDefinition Create = new DisplayDefinition();
        public DisplayDefinition Create_ja = new DisplayDefinition();
        public DisplayDefinition Update = new DisplayDefinition();
        public DisplayDefinition Update_ja = new DisplayDefinition();
        public DisplayDefinition Save = new DisplayDefinition();
        public DisplayDefinition Save_ja = new DisplayDefinition();
        public DisplayDefinition Change = new DisplayDefinition();
        public DisplayDefinition Change_ja = new DisplayDefinition();
        public DisplayDefinition Add = new DisplayDefinition();
        public DisplayDefinition Add_ja = new DisplayDefinition();
        public DisplayDefinition Register = new DisplayDefinition();
        public DisplayDefinition Register_ja = new DisplayDefinition();
        public DisplayDefinition Reset = new DisplayDefinition();
        public DisplayDefinition Reset_ja = new DisplayDefinition();
        public DisplayDefinition Copy = new DisplayDefinition();
        public DisplayDefinition Copy_ja = new DisplayDefinition();
        public DisplayDefinition Move = new DisplayDefinition();
        public DisplayDefinition Move_ja = new DisplayDefinition();
        public DisplayDefinition BulkMove = new DisplayDefinition();
        public DisplayDefinition BulkMove_ja = new DisplayDefinition();
        public DisplayDefinition Mail = new DisplayDefinition();
        public DisplayDefinition Mail_ja = new DisplayDefinition();
        public DisplayDefinition MailAddress = new DisplayDefinition();
        public DisplayDefinition MailAddress_ja = new DisplayDefinition();
        public DisplayDefinition SendMail = new DisplayDefinition();
        public DisplayDefinition SendMail_ja = new DisplayDefinition();
        public DisplayDefinition Delete = new DisplayDefinition();
        public DisplayDefinition Delete_ja = new DisplayDefinition();
        public DisplayDefinition Separate = new DisplayDefinition();
        public DisplayDefinition Separate_ja = new DisplayDefinition();
        public DisplayDefinition BulkDelete = new DisplayDefinition();
        public DisplayDefinition BulkDelete_ja = new DisplayDefinition();
        public DisplayDefinition Import = new DisplayDefinition();
        public DisplayDefinition Import_ja = new DisplayDefinition();
        public DisplayDefinition Export = new DisplayDefinition();
        public DisplayDefinition Export_ja = new DisplayDefinition();
        public DisplayDefinition Cancel = new DisplayDefinition();
        public DisplayDefinition Cancel_ja = new DisplayDefinition();
        public DisplayDefinition GoBack = new DisplayDefinition();
        public DisplayDefinition GoBack_ja = new DisplayDefinition();
        public DisplayDefinition Synchronize = new DisplayDefinition();
        public DisplayDefinition Synchronize_ja = new DisplayDefinition();
        public DisplayDefinition Operations = new DisplayDefinition();
        public DisplayDefinition Operations_ja = new DisplayDefinition();
        public DisplayDefinition GroupBy = new DisplayDefinition();
        public DisplayDefinition GroupBy_ja = new DisplayDefinition();
        public DisplayDefinition Date = new DisplayDefinition();
        public DisplayDefinition Date_ja = new DisplayDefinition();
        public DisplayDefinition Count = new DisplayDefinition();
        public DisplayDefinition Count_ja = new DisplayDefinition();
        public DisplayDefinition Total = new DisplayDefinition();
        public DisplayDefinition Total_ja = new DisplayDefinition();
        public DisplayDefinition Average = new DisplayDefinition();
        public DisplayDefinition Average_ja = new DisplayDefinition();
        public DisplayDefinition Max = new DisplayDefinition();
        public DisplayDefinition Max_ja = new DisplayDefinition();
        public DisplayDefinition Min = new DisplayDefinition();
        public DisplayDefinition Min_ja = new DisplayDefinition();
        public DisplayDefinition Step = new DisplayDefinition();
        public DisplayDefinition Step_ja = new DisplayDefinition();
        public DisplayDefinition DecimalPlaces = new DisplayDefinition();
        public DisplayDefinition DecimalPlaces_ja = new DisplayDefinition();
        public DisplayDefinition ToUpper = new DisplayDefinition();
        public DisplayDefinition ToUpper_ja = new DisplayDefinition();
        public DisplayDefinition MoveUp = new DisplayDefinition();
        public DisplayDefinition MoveUp_ja = new DisplayDefinition();
        public DisplayDefinition MoveDown = new DisplayDefinition();
        public DisplayDefinition MoveDown_ja = new DisplayDefinition();
        public DisplayDefinition Previous = new DisplayDefinition();
        public DisplayDefinition Previous_ja = new DisplayDefinition();
        public DisplayDefinition Next = new DisplayDefinition();
        public DisplayDefinition Next_ja = new DisplayDefinition();
        public DisplayDefinition Reload = new DisplayDefinition();
        public DisplayDefinition Reload_ja = new DisplayDefinition();
        public DisplayDefinition Newer = new DisplayDefinition();
        public DisplayDefinition Newer_ja = new DisplayDefinition();
        public DisplayDefinition Older = new DisplayDefinition();
        public DisplayDefinition Older_ja = new DisplayDefinition();
        public DisplayDefinition Latest = new DisplayDefinition();
        public DisplayDefinition Latest_ja = new DisplayDefinition();
        public DisplayDefinition Visible = new DisplayDefinition();
        public DisplayDefinition Visible_ja = new DisplayDefinition();
        public DisplayDefinition Hide = new DisplayDefinition();
        public DisplayDefinition Hide_ja = new DisplayDefinition();
        public DisplayDefinition PlannedValue = new DisplayDefinition();
        public DisplayDefinition PlannedValue_ja = new DisplayDefinition();
        public DisplayDefinition EarnedValue = new DisplayDefinition();
        public DisplayDefinition EarnedValue_ja = new DisplayDefinition();
        public DisplayDefinition Difference = new DisplayDefinition();
        public DisplayDefinition Difference_ja = new DisplayDefinition();
        public DisplayDefinition ChangePassword = new DisplayDefinition();
        public DisplayDefinition ChangePassword_ja = new DisplayDefinition();
        public DisplayDefinition ResetPassword = new DisplayDefinition();
        public DisplayDefinition ResetPassword_ja = new DisplayDefinition();
        public DisplayDefinition ReadOnly = new DisplayDefinition();
        public DisplayDefinition ReadOnly_ja = new DisplayDefinition();
        public DisplayDefinition ReadWrite = new DisplayDefinition();
        public DisplayDefinition ReadWrite_ja = new DisplayDefinition();
        public DisplayDefinition Leader = new DisplayDefinition();
        public DisplayDefinition Leader_ja = new DisplayDefinition();
        public DisplayDefinition Manager = new DisplayDefinition();
        public DisplayDefinition Manager_ja = new DisplayDefinition();
        public DisplayDefinition DeletePermission = new DisplayDefinition();
        public DisplayDefinition DeletePermission_ja = new DisplayDefinition();
        public DisplayDefinition AddPermission = new DisplayDefinition();
        public DisplayDefinition AddPermission_ja = new DisplayDefinition();
        public DisplayDefinition NotInheritPermission = new DisplayDefinition();
        public DisplayDefinition NotInheritPermission_ja = new DisplayDefinition();
        public DisplayDefinition NoTitle = new DisplayDefinition();
        public DisplayDefinition NoTitle_ja = new DisplayDefinition();
        public DisplayDefinition Error = new DisplayDefinition();
        public DisplayDefinition Error_ja = new DisplayDefinition();
        public DisplayDefinition Index = new DisplayDefinition();
        public DisplayDefinition Index_ja = new DisplayDefinition();
        public DisplayDefinition Edit = new DisplayDefinition();
        public DisplayDefinition Edit_ja = new DisplayDefinition();
        public DisplayDefinition History = new DisplayDefinition();
        public DisplayDefinition History_ja = new DisplayDefinition();
        public DisplayDefinition Histories = new DisplayDefinition();
        public DisplayDefinition Histories_ja = new DisplayDefinition();
        public DisplayDefinition Links = new DisplayDefinition();
        public DisplayDefinition Links_ja = new DisplayDefinition();
        public DisplayDefinition LinkDestinations = new DisplayDefinition();
        public DisplayDefinition LinkDestinations_ja = new DisplayDefinition();
        public DisplayDefinition LinkSources = new DisplayDefinition();
        public DisplayDefinition LinkSources_ja = new DisplayDefinition();
        public DisplayDefinition LinkCreations = new DisplayDefinition();
        public DisplayDefinition LinkCreations_ja = new DisplayDefinition();
        public DisplayDefinition Comments = new DisplayDefinition();
        public DisplayDefinition Comments_ja = new DisplayDefinition();
        public DisplayDefinition SentMail = new DisplayDefinition();
        public DisplayDefinition SentMail_ja = new DisplayDefinition();
        public DisplayDefinition Reply = new DisplayDefinition();
        public DisplayDefinition Reply_ja = new DisplayDefinition();
        public DisplayDefinition OriginalMessage = new DisplayDefinition();
        public DisplayDefinition Logout = new DisplayDefinition();
        public DisplayDefinition Logout_ja = new DisplayDefinition();
        public DisplayDefinition EditProfile = new DisplayDefinition();
        public DisplayDefinition EditProfile_ja = new DisplayDefinition();
        public DisplayDefinition CreatedTime = new DisplayDefinition();
        public DisplayDefinition CreatedTime_ja = new DisplayDefinition();
        public DisplayDefinition UpdatedTime = new DisplayDefinition();
        public DisplayDefinition UpdatedTime_ja = new DisplayDefinition();
        public DisplayDefinition PermissionDestination = new DisplayDefinition();
        public DisplayDefinition PermissionDestination_ja = new DisplayDefinition();
        public DisplayDefinition PermissionSource = new DisplayDefinition();
        public DisplayDefinition PermissionSource_ja = new DisplayDefinition();
        public DisplayDefinition SettingColumnList = new DisplayDefinition();
        public DisplayDefinition SettingColumnList_ja = new DisplayDefinition();
        public DisplayDefinition SettingTitleColumn = new DisplayDefinition();
        public DisplayDefinition SettingTitleColumn_ja = new DisplayDefinition();
        public DisplayDefinition SettingTitleSeparator = new DisplayDefinition();
        public DisplayDefinition SettingTitleSeparator_ja = new DisplayDefinition();
        public DisplayDefinition SettingAggregationList = new DisplayDefinition();
        public DisplayDefinition SettingAggregationList_ja = new DisplayDefinition();
        public DisplayDefinition SettingNotGroupBy = new DisplayDefinition();
        public DisplayDefinition SettingNotGroupBy_ja = new DisplayDefinition();
        public DisplayDefinition SettingNearCompletionTimeBeforeDays = new DisplayDefinition();
        public DisplayDefinition SettingNearCompletionTimeBeforeDays_ja = new DisplayDefinition();
        public DisplayDefinition SettingNearCompletionTimeAfterDays = new DisplayDefinition();
        public DisplayDefinition SettingNearCompletionTimeAfterDays_ja = new DisplayDefinition();
        public DisplayDefinition SettingGridPageSize = new DisplayDefinition();
        public DisplayDefinition SettingGridPageSize_ja = new DisplayDefinition();
        public DisplayDefinition SettingLabel = new DisplayDefinition();
        public DisplayDefinition SettingLabel_ja = new DisplayDefinition();
        public DisplayDefinition SettingEnable = new DisplayDefinition();
        public DisplayDefinition SettingEnable_ja = new DisplayDefinition();
        public DisplayDefinition SettingOrder = new DisplayDefinition();
        public DisplayDefinition SettingOrder_ja = new DisplayDefinition();
        public DisplayDefinition SettingUnit = new DisplayDefinition();
        public DisplayDefinition SettingUnit_ja = new DisplayDefinition();
        public DisplayDefinition SettingControlDateTime = new DisplayDefinition();
        public DisplayDefinition SettingControlDateTime_ja = new DisplayDefinition();
        public DisplayDefinition SettingGridDateTime = new DisplayDefinition();
        public DisplayDefinition SettingGridDateTime_ja = new DisplayDefinition();
        public DisplayDefinition SettingSelectionList = new DisplayDefinition();
        public DisplayDefinition SettingSelectionList_ja = new DisplayDefinition();
        public DisplayDefinition SettingChoicesVisible = new DisplayDefinition();
        public DisplayDefinition SettingChoicesVisible_ja = new DisplayDefinition();
        public DisplayDefinition SettingLimitDefault = new DisplayDefinition();
        public DisplayDefinition SettingLimitDefault_ja = new DisplayDefinition();
        public DisplayDefinition SettingGridColumns = new DisplayDefinition();
        public DisplayDefinition SettingGridColumns_ja = new DisplayDefinition();
        public DisplayDefinition SettingFilterColumns = new DisplayDefinition();
        public DisplayDefinition SettingFilterColumns_ja = new DisplayDefinition();
        public DisplayDefinition SettingSummaryColumns = new DisplayDefinition();
        public DisplayDefinition SettingSummaryColumns_ja = new DisplayDefinition();
        public DisplayDefinition SettingEditorColumns = new DisplayDefinition();
        public DisplayDefinition SettingEditorColumns_ja = new DisplayDefinition();
        public DisplayDefinition SettingLinkColumns = new DisplayDefinition();
        public DisplayDefinition SettingLinkColumns_ja = new DisplayDefinition();
        public DisplayDefinition SettingHistoryColumns = new DisplayDefinition();
        public DisplayDefinition SettingHistoryColumns_ja = new DisplayDefinition();
        public DisplayDefinition SettingFormulas = new DisplayDefinition();
        public DisplayDefinition SettingFormulas_ja = new DisplayDefinition();
        public DisplayDefinition SettingAggregations = new DisplayDefinition();
        public DisplayDefinition SettingAggregations_ja = new DisplayDefinition();
        public DisplayDefinition SettingAggregationType = new DisplayDefinition();
        public DisplayDefinition SettingAggregationType_ja = new DisplayDefinition();
        public DisplayDefinition SettingAggregationTarget = new DisplayDefinition();
        public DisplayDefinition SettingAggregationTarget_ja = new DisplayDefinition();
        public DisplayDefinition CurrentPassword = new DisplayDefinition();
        public DisplayDefinition CurrentPassword_ja = new DisplayDefinition();
        public DisplayDefinition ReEnter = new DisplayDefinition();
        public DisplayDefinition ReEnter_ja = new DisplayDefinition();
        public DisplayDefinition VerUp = new DisplayDefinition();
        public DisplayDefinition VerUp_ja = new DisplayDefinition();
        public DisplayDefinition Hidden = new DisplayDefinition();
        public DisplayDefinition Hidden_ja = new DisplayDefinition();
        public DisplayDefinition Output = new DisplayDefinition();
        public DisplayDefinition Output_ja = new DisplayDefinition();
        public DisplayDefinition NotOutput = new DisplayDefinition();
        public DisplayDefinition NotOutput_ja = new DisplayDefinition();
        public DisplayDefinition CopyWithComments = new DisplayDefinition();
        public DisplayDefinition CopyWithComments_ja = new DisplayDefinition();
        public DisplayDefinition Hyphen = new DisplayDefinition();
        public DisplayDefinition Search = new DisplayDefinition();
        public DisplayDefinition Search_ja = new DisplayDefinition();
        public DisplayDefinition Admin = new DisplayDefinition();
        public DisplayDefinition Admin_ja = new DisplayDefinition();
        public DisplayDefinition Default = new DisplayDefinition();
        public DisplayDefinition Default_ja = new DisplayDefinition();
        public DisplayDefinition DefaultInput = new DisplayDefinition();
        public DisplayDefinition DefaultInput_ja = new DisplayDefinition();
        public DisplayDefinition AddressBook = new DisplayDefinition();
        public DisplayDefinition AddressBook_ja = new DisplayDefinition();
        public DisplayDefinition DefaultAddressBook = new DisplayDefinition();
        public DisplayDefinition DefaultAddressBook_ja = new DisplayDefinition();
        public DisplayDefinition DefaultDestinations = new DisplayDefinition();
        public DisplayDefinition DefaultDestinations_ja = new DisplayDefinition();
        public DisplayDefinition LimitJust = new DisplayDefinition();
        public DisplayDefinition LimitJust_ja = new DisplayDefinition();
        public DisplayDefinition LimitAfterSecond = new DisplayDefinition();
        public DisplayDefinition LimitAfterSecond_ja = new DisplayDefinition();
        public DisplayDefinition LimitAfterSeconds = new DisplayDefinition();
        public DisplayDefinition LimitAfterSeconds_ja = new DisplayDefinition();
        public DisplayDefinition LimitAfterMinute = new DisplayDefinition();
        public DisplayDefinition LimitAfterMinute_ja = new DisplayDefinition();
        public DisplayDefinition LimitAfterMinutes = new DisplayDefinition();
        public DisplayDefinition LimitAfterMinutes_ja = new DisplayDefinition();
        public DisplayDefinition LimitAfterHour = new DisplayDefinition();
        public DisplayDefinition LimitAfterHour_ja = new DisplayDefinition();
        public DisplayDefinition LimitAfterHours = new DisplayDefinition();
        public DisplayDefinition LimitAfterHours_ja = new DisplayDefinition();
        public DisplayDefinition LimitAfterDay = new DisplayDefinition();
        public DisplayDefinition LimitAfterDay_ja = new DisplayDefinition();
        public DisplayDefinition LimitAfterDays = new DisplayDefinition();
        public DisplayDefinition LimitAfterDays_ja = new DisplayDefinition();
        public DisplayDefinition LimitAfterMonth = new DisplayDefinition();
        public DisplayDefinition LimitAfterMonth_ja = new DisplayDefinition();
        public DisplayDefinition LimitAfterMonths = new DisplayDefinition();
        public DisplayDefinition LimitAfterMonths_ja = new DisplayDefinition();
        public DisplayDefinition LimitAfterYear = new DisplayDefinition();
        public DisplayDefinition LimitAfterYear_ja = new DisplayDefinition();
        public DisplayDefinition LimitAfterYears = new DisplayDefinition();
        public DisplayDefinition LimitAfterYears_ja = new DisplayDefinition();
        public DisplayDefinition LimitBeforeSecond = new DisplayDefinition();
        public DisplayDefinition LimitBeforeSecond_ja = new DisplayDefinition();
        public DisplayDefinition LimitBeforeSeconds = new DisplayDefinition();
        public DisplayDefinition LimitBeforeSeconds_ja = new DisplayDefinition();
        public DisplayDefinition LimitBeforeMinute = new DisplayDefinition();
        public DisplayDefinition LimitBeforeMinute_ja = new DisplayDefinition();
        public DisplayDefinition LimitBeforeMinutes = new DisplayDefinition();
        public DisplayDefinition LimitBeforeMinutes_ja = new DisplayDefinition();
        public DisplayDefinition LimitBeforeHour = new DisplayDefinition();
        public DisplayDefinition LimitBeforeHour_ja = new DisplayDefinition();
        public DisplayDefinition LimitBeforeHours = new DisplayDefinition();
        public DisplayDefinition LimitBeforeHours_ja = new DisplayDefinition();
        public DisplayDefinition LimitBeforeDay = new DisplayDefinition();
        public DisplayDefinition LimitBeforeDay_ja = new DisplayDefinition();
        public DisplayDefinition LimitBeforeDays = new DisplayDefinition();
        public DisplayDefinition LimitBeforeDays_ja = new DisplayDefinition();
        public DisplayDefinition LimitBeforeMonth = new DisplayDefinition();
        public DisplayDefinition LimitBeforeMonth_ja = new DisplayDefinition();
        public DisplayDefinition LimitBeforeMonths = new DisplayDefinition();
        public DisplayDefinition LimitBeforeMonths_ja = new DisplayDefinition();
        public DisplayDefinition LimitBeforeYear = new DisplayDefinition();
        public DisplayDefinition LimitBeforeYear_ja = new DisplayDefinition();
        public DisplayDefinition LimitBeforeYears = new DisplayDefinition();
        public DisplayDefinition LimitBeforeYears_ja = new DisplayDefinition();
        public DisplayDefinition WriteComment = new DisplayDefinition();
        public DisplayDefinition WriteComment_ja = new DisplayDefinition();
        public DisplayDefinition Filters = new DisplayDefinition();
        public DisplayDefinition Filters_ja = new DisplayDefinition();
        public DisplayDefinition Incomplete = new DisplayDefinition();
        public DisplayDefinition Incomplete_ja = new DisplayDefinition();
        public DisplayDefinition Blank = new DisplayDefinition();
        public DisplayDefinition Blank_ja = new DisplayDefinition();
        public DisplayDefinition Own = new DisplayDefinition();
        public DisplayDefinition Own_ja = new DisplayDefinition();
        public DisplayDefinition NearCompletionTime = new DisplayDefinition();
        public DisplayDefinition NearCompletionTime_ja = new DisplayDefinition();
        public DisplayDefinition Delay = new DisplayDefinition();
        public DisplayDefinition Delay_ja = new DisplayDefinition();
        public DisplayDefinition Overdue = new DisplayDefinition();
        public DisplayDefinition Overdue_ja = new DisplayDefinition();
        public DisplayDefinition OrderAsc = new DisplayDefinition();
        public DisplayDefinition OrderAsc_ja = new DisplayDefinition();
        public DisplayDefinition OrderDesc = new DisplayDefinition();
        public DisplayDefinition OrderDesc_ja = new DisplayDefinition();
        public DisplayDefinition OrderRelease = new DisplayDefinition();
        public DisplayDefinition OrderRelease_ja = new DisplayDefinition();
        public DisplayDefinition ResetOrder = new DisplayDefinition();
        public DisplayDefinition ResetOrder_ja = new DisplayDefinition();
        public DisplayDefinition Csv = new DisplayDefinition();
        public DisplayDefinition Csv_ja = new DisplayDefinition();
        public DisplayDefinition CsvFile = new DisplayDefinition();
        public DisplayDefinition CsvFile_ja = new DisplayDefinition();
        public DisplayDefinition CharacterCode = new DisplayDefinition();
        public DisplayDefinition CharacterCode_ja = new DisplayDefinition();
        public DisplayDefinition Excel = new DisplayDefinition();
        public DisplayDefinition Excel_ja = new DisplayDefinition();
        public DisplayDefinition Setting = new DisplayDefinition();
        public DisplayDefinition Setting_ja = new DisplayDefinition();
        public DisplayDefinition AdvancedSetting = new DisplayDefinition();
        public DisplayDefinition AdvancedSetting_ja = new DisplayDefinition();
        public DisplayDefinition Basic = new DisplayDefinition();
        public DisplayDefinition Basic_ja = new DisplayDefinition();
        public DisplayDefinition Normal = new DisplayDefinition();
        public DisplayDefinition Normal_ja = new DisplayDefinition();
        public DisplayDefinition Wide = new DisplayDefinition();
        public DisplayDefinition Wide_ja = new DisplayDefinition();
        public DisplayDefinition Auto = new DisplayDefinition();
        public DisplayDefinition Auto_ja = new DisplayDefinition();
        public DisplayDefinition ControlType = new DisplayDefinition();
        public DisplayDefinition ControlType_ja = new DisplayDefinition();
        public DisplayDefinition Spinner = new DisplayDefinition();
        public DisplayDefinition Spinner_ja = new DisplayDefinition();
        public DisplayDefinition SiteImageSettingsEditor = new DisplayDefinition();
        public DisplayDefinition SiteImageSettingsEditor_ja = new DisplayDefinition();
        public DisplayDefinition GridSettingsEditor = new DisplayDefinition();
        public DisplayDefinition GridSettingsEditor_ja = new DisplayDefinition();
        public DisplayDefinition EditorSettingsEditor = new DisplayDefinition();
        public DisplayDefinition EditorSettingsEditor_ja = new DisplayDefinition();
        public DisplayDefinition MailerSettingsEditor = new DisplayDefinition();
        public DisplayDefinition MailerSettingsEditor_ja = new DisplayDefinition();
        public DisplayDefinition StyleSettingsEditor = new DisplayDefinition();
        public DisplayDefinition StyleSettingsEditor_ja = new DisplayDefinition();
        public DisplayDefinition ScriptSettingsEditor = new DisplayDefinition();
        public DisplayDefinition ScriptSettingsEditor_ja = new DisplayDefinition();
        public DisplayDefinition SummarySettingsEditor = new DisplayDefinition();
        public DisplayDefinition SummarySettingsEditor_ja = new DisplayDefinition();
        public DisplayDefinition PermissionSetting = new DisplayDefinition();
        public DisplayDefinition PermissionSetting_ja = new DisplayDefinition();
        public DisplayDefinition EditPermissions = new DisplayDefinition();
        public DisplayDefinition EditPermissions_ja = new DisplayDefinition();
        public DisplayDefinition CopySettings = new DisplayDefinition();
        public DisplayDefinition CopySettings_ja = new DisplayDefinition();
        public DisplayDefinition AggregationDetails = new DisplayDefinition();
        public DisplayDefinition AggregationDetails_ja = new DisplayDefinition();
        public DisplayDefinition MoveSettings = new DisplayDefinition();
        public DisplayDefinition MoveSettings_ja = new DisplayDefinition();
        public DisplayDefinition Destination = new DisplayDefinition();
        public DisplayDefinition Destination_ja = new DisplayDefinition();
        public DisplayDefinition SeparateSettings = new DisplayDefinition();
        public DisplayDefinition SeparateSettings_ja = new DisplayDefinition();
        public DisplayDefinition SummarySiteId = new DisplayDefinition();
        public DisplayDefinition SummarySiteId_ja = new DisplayDefinition();
        public DisplayDefinition SummaryDestinationColumn = new DisplayDefinition();
        public DisplayDefinition SummaryDestinationColumn_ja = new DisplayDefinition();
        public DisplayDefinition SummaryLinkColumn = new DisplayDefinition();
        public DisplayDefinition SummaryLinkColumn_ja = new DisplayDefinition();
        public DisplayDefinition SummaryType = new DisplayDefinition();
        public DisplayDefinition SummaryType_ja = new DisplayDefinition();
        public DisplayDefinition SummarySourceColumn = new DisplayDefinition();
        public DisplayDefinition SummarySourceColumn_ja = new DisplayDefinition();
        public DisplayDefinition Title = new DisplayDefinition();
        public DisplayDefinition Title_ja = new DisplayDefinition();
        public DisplayDefinition WorkValue = new DisplayDefinition();
        public DisplayDefinition WorkValue_ja = new DisplayDefinition();
        public DisplayDefinition SeparateNumber = new DisplayDefinition();
        public DisplayDefinition SeparateNumber_ja = new DisplayDefinition();
        public DisplayDefinition OutgoingMail = new DisplayDefinition();
        public DisplayDefinition OutgoingMail_ja = new DisplayDefinition();
        public DisplayDefinition Icon = new DisplayDefinition();
        public DisplayDefinition Icon_ja = new DisplayDefinition();
        public DisplayDefinition File = new DisplayDefinition();
        public DisplayDefinition File_ja = new DisplayDefinition();
        public DisplayDefinition NotSet = new DisplayDefinition();
        public DisplayDefinition NotSet_ja = new DisplayDefinition();
        public DisplayDefinition SiteUser = new DisplayDefinition();
        public DisplayDefinition SiteUser_ja = new DisplayDefinition();
        public DisplayDefinition All = new DisplayDefinition();
        public DisplayDefinition All_ja = new DisplayDefinition();
        public DisplayDefinition Quantity = new DisplayDefinition();
        public DisplayDefinition Quantity_ja = new DisplayDefinition();
        public DisplayDefinition SuffixCopy = new DisplayDefinition();
        public DisplayDefinition SuffixCopy_ja = new DisplayDefinition();
        public DisplayDefinition DataViewSelector = new DisplayDefinition();
        public DisplayDefinition DataViewSelector_ja = new DisplayDefinition();
        public DisplayDefinition Grid = new DisplayDefinition();
        public DisplayDefinition Grid_ja = new DisplayDefinition();
        public DisplayDefinition Style = new DisplayDefinition();
        public DisplayDefinition Style_ja = new DisplayDefinition();
        public DisplayDefinition GridStyle = new DisplayDefinition();
        public DisplayDefinition GridStyle_ja = new DisplayDefinition();
        public DisplayDefinition NewStyle = new DisplayDefinition();
        public DisplayDefinition NewStyle_ja = new DisplayDefinition();
        public DisplayDefinition EditStyle = new DisplayDefinition();
        public DisplayDefinition EditStyle_ja = new DisplayDefinition();
        public DisplayDefinition Script = new DisplayDefinition();
        public DisplayDefinition Script_ja = new DisplayDefinition();
        public DisplayDefinition GridScript = new DisplayDefinition();
        public DisplayDefinition GridScript_ja = new DisplayDefinition();
        public DisplayDefinition NewScript = new DisplayDefinition();
        public DisplayDefinition NewScript_ja = new DisplayDefinition();
        public DisplayDefinition EditScript = new DisplayDefinition();
        public DisplayDefinition EditScript_ja = new DisplayDefinition();
        public DisplayDefinition BurnDown = new DisplayDefinition();
        public DisplayDefinition BurnDown_ja = new DisplayDefinition();
        public DisplayDefinition Gantt = new DisplayDefinition();
        public DisplayDefinition Gantt_ja = new DisplayDefinition();
        public DisplayDefinition TimeSeries = new DisplayDefinition();
        public DisplayDefinition TimeSeries_ja = new DisplayDefinition();
        public DisplayDefinition Kamban = new DisplayDefinition();
        public DisplayDefinition Kamban_ja = new DisplayDefinition();
        public DisplayDefinition NoData = new DisplayDefinition();
        public DisplayDefinition NoData_ja = new DisplayDefinition();
        public DisplayDefinition NoLinks = new DisplayDefinition();
        public DisplayDefinition NoLinks_ja = new DisplayDefinition();
        public DisplayDefinition ViewDemoEnvironment = new DisplayDefinition();
        public DisplayDefinition ViewDemoEnvironment_ja = new DisplayDefinition();
        public DisplayDefinition DemoMailTitle = new DisplayDefinition();
        public DisplayDefinition DemoMailTitle_ja = new DisplayDefinition();
        public DisplayDefinition DemoMailBody = new DisplayDefinition();
        public DisplayDefinition DemoMailBody_ja = new DisplayDefinition();
        public DisplayDefinition Fy = new DisplayDefinition();
        public DisplayDefinition Fy_ja = new DisplayDefinition();
        public DisplayDefinition Half1 = new DisplayDefinition();
        public DisplayDefinition Half1_ja = new DisplayDefinition();
        public DisplayDefinition Half2 = new DisplayDefinition();
        public DisplayDefinition Half2_ja = new DisplayDefinition();
        public DisplayDefinition Quarter = new DisplayDefinition();
        public DisplayDefinition Quarter_ja = new DisplayDefinition();
        public DisplayDefinition Ymd = new DisplayDefinition();
        public DisplayDefinition Ymd_ja = new DisplayDefinition();
        public DisplayDefinition Ymda = new DisplayDefinition();
        public DisplayDefinition Ymda_ja = new DisplayDefinition();
        public DisplayDefinition Ym = new DisplayDefinition();
        public DisplayDefinition Ym_ja = new DisplayDefinition();
        public DisplayDefinition Ymdhms = new DisplayDefinition();
        public DisplayDefinition Ymdhms_ja = new DisplayDefinition();
        public DisplayDefinition Ymdahms = new DisplayDefinition();
        public DisplayDefinition Ymdahms_ja = new DisplayDefinition();
        public DisplayDefinition Ymdhm = new DisplayDefinition();
        public DisplayDefinition Ymdhm_ja = new DisplayDefinition();
        public DisplayDefinition Ymdahm = new DisplayDefinition();
        public DisplayDefinition Ymdahm_ja = new DisplayDefinition();
        public DisplayDefinition YmdFormat = new DisplayDefinition();
        public DisplayDefinition YmdFormat_ja = new DisplayDefinition();
        public DisplayDefinition YmdaFormat = new DisplayDefinition();
        public DisplayDefinition YmdaFormat_ja = new DisplayDefinition();
        public DisplayDefinition YmFormat = new DisplayDefinition();
        public DisplayDefinition YmFormat_ja = new DisplayDefinition();
        public DisplayDefinition YmdhmsFormat = new DisplayDefinition();
        public DisplayDefinition YmdhmsFormat_ja = new DisplayDefinition();
        public DisplayDefinition YmdahmsFormat = new DisplayDefinition();
        public DisplayDefinition YmdahmsFormat_ja = new DisplayDefinition();
        public DisplayDefinition YmdhmFormat = new DisplayDefinition();
        public DisplayDefinition YmdhmFormat_ja = new DisplayDefinition();
        public DisplayDefinition YmdahmFormat = new DisplayDefinition();
        public DisplayDefinition YmdahmFormat_ja = new DisplayDefinition();
        public DisplayDefinition ExceptionTitle = new DisplayDefinition();
        public DisplayDefinition ExceptionTitle_ja = new DisplayDefinition();
        public DisplayDefinition ExceptionBody = new DisplayDefinition();
        public DisplayDefinition ExceptionBody_ja = new DisplayDefinition();
        public DisplayDefinition InvalidRequest = new DisplayDefinition();
        public DisplayDefinition InvalidRequest_ja = new DisplayDefinition();
        public DisplayDefinition Authentication = new DisplayDefinition();
        public DisplayDefinition Authentication_ja = new DisplayDefinition();
        public DisplayDefinition PasswordNotChanged = new DisplayDefinition();
        public DisplayDefinition PasswordNotChanged_ja = new DisplayDefinition();
        public DisplayDefinition UpdateConflicts = new DisplayDefinition();
        public DisplayDefinition UpdateConflicts_ja = new DisplayDefinition();
        public DisplayDefinition DeleteConflicts = new DisplayDefinition();
        public DisplayDefinition DeleteConflicts_ja = new DisplayDefinition();
        public DisplayDefinition HasNotPermission = new DisplayDefinition();
        public DisplayDefinition HasNotPermission_ja = new DisplayDefinition();
        public DisplayDefinition IncorrectCurrentPassword = new DisplayDefinition();
        public DisplayDefinition IncorrectCurrentPassword_ja = new DisplayDefinition();
        public DisplayDefinition PermissionNotSelfChange = new DisplayDefinition();
        public DisplayDefinition PermissionNotSelfChange_ja = new DisplayDefinition();
        public DisplayDefinition DefinitionNotFound = new DisplayDefinition();
        public DisplayDefinition DefinitionNotFound_ja = new DisplayDefinition();
        public DisplayDefinition CantSetAtTopOfSite = new DisplayDefinition();
        public DisplayDefinition CantSetAtTopOfSite_ja = new DisplayDefinition();
        public DisplayDefinition NotFound = new DisplayDefinition();
        public DisplayDefinition NotFound_ja = new DisplayDefinition();
        public DisplayDefinition RequireMailAddresses = new DisplayDefinition();
        public DisplayDefinition RequireMailAddresses_ja = new DisplayDefinition();
        public DisplayDefinition RequireColumn = new DisplayDefinition();
        public DisplayDefinition RequireColumn_ja = new DisplayDefinition();
        public DisplayDefinition ExternalMailAddress = new DisplayDefinition();
        public DisplayDefinition ExternalMailAddress_ja = new DisplayDefinition();
        public DisplayDefinition BadMailAddress = new DisplayDefinition();
        public DisplayDefinition BadMailAddress_ja = new DisplayDefinition();
        public DisplayDefinition MailAddressHasNotSet = new DisplayDefinition();
        public DisplayDefinition MailAddressHasNotSet_ja = new DisplayDefinition();
        public DisplayDefinition FileNotFound = new DisplayDefinition();
        public DisplayDefinition FileNotFound_ja = new DisplayDefinition();
        public DisplayDefinition NotRequiredColumn = new DisplayDefinition();
        public DisplayDefinition NotRequiredColumn_ja = new DisplayDefinition();
        public DisplayDefinition InvalidCsvData = new DisplayDefinition();
        public DisplayDefinition InvalidCsvData_ja = new DisplayDefinition();
        public DisplayDefinition FailedReadFile = new DisplayDefinition();
        public DisplayDefinition FailedReadFile_ja = new DisplayDefinition();
        public DisplayDefinition CanNotHide = new DisplayDefinition();
        public DisplayDefinition CanNotHide_ja = new DisplayDefinition();
        public DisplayDefinition AlreadyAdded = new DisplayDefinition();
        public DisplayDefinition AlreadyAdded_ja = new DisplayDefinition();
        public DisplayDefinition InvalidFormula = new DisplayDefinition();
        public DisplayDefinition InvalidFormula_ja = new DisplayDefinition();
        public DisplayDefinition SelectTargets = new DisplayDefinition();
        public DisplayDefinition SelectTargets_ja = new DisplayDefinition();
        public DisplayDefinition LoginIn = new DisplayDefinition();
        public DisplayDefinition LoginIn_ja = new DisplayDefinition();
        public DisplayDefinition Deleted = new DisplayDefinition();
        public DisplayDefinition Deleted_ja = new DisplayDefinition();
        public DisplayDefinition BulkMoved = new DisplayDefinition();
        public DisplayDefinition BulkMoved_ja = new DisplayDefinition();
        public DisplayDefinition BulkDeleted = new DisplayDefinition();
        public DisplayDefinition BulkDeleted_ja = new DisplayDefinition();
        public DisplayDefinition Separated = new DisplayDefinition();
        public DisplayDefinition Separated_ja = new DisplayDefinition();
        public DisplayDefinition Imported = new DisplayDefinition();
        public DisplayDefinition Imported_ja = new DisplayDefinition();
        public DisplayDefinition Created = new DisplayDefinition();
        public DisplayDefinition Created_ja = new DisplayDefinition();
        public DisplayDefinition Updated = new DisplayDefinition();
        public DisplayDefinition Updated_ja = new DisplayDefinition();
        public DisplayDefinition CodeDefinerCompleted = new DisplayDefinition();
        public DisplayDefinition CodeDefinerCompleted_ja = new DisplayDefinition();
        public DisplayDefinition CodeDefinerRdsCompleted = new DisplayDefinition();
        public DisplayDefinition CodeDefinerRdsCompleted_ja = new DisplayDefinition();
        public DisplayDefinition CodeDefinerDefCompleted = new DisplayDefinition();
        public DisplayDefinition CodeDefinerDefCompleted_ja = new DisplayDefinition();
        public DisplayDefinition CodeDefinerMvcCompleted = new DisplayDefinition();
        public DisplayDefinition CodeDefinerMvcCompleted_ja = new DisplayDefinition();
        public DisplayDefinition CodeDefinerCssCompleted = new DisplayDefinition();
        public DisplayDefinition CodeDefinerCssCompleted_ja = new DisplayDefinition();
        public DisplayDefinition CodeDefinerBackupCompleted = new DisplayDefinition();
        public DisplayDefinition CodeDefinerBackupCompleted_ja = new DisplayDefinition();
        public DisplayDefinition MailTransmissionCompletion = new DisplayDefinition();
        public DisplayDefinition MailTransmissionCompletion_ja = new DisplayDefinition();
        public DisplayDefinition ChangingPasswordComplete = new DisplayDefinition();
        public DisplayDefinition ChangingPasswordComplete_ja = new DisplayDefinition();
        public DisplayDefinition PasswordResetCompleted = new DisplayDefinition();
        public DisplayDefinition PasswordResetCompleted_ja = new DisplayDefinition();
        public DisplayDefinition FileUpdateCompleted = new DisplayDefinition();
        public DisplayDefinition FileUpdateCompleted_ja = new DisplayDefinition();
        public DisplayDefinition SynchronizationCompleted = new DisplayDefinition();
        public DisplayDefinition SynchronizationCompleted_ja = new DisplayDefinition();
        public DisplayDefinition SentAcceptanceMail_space_ = new DisplayDefinition();
        public DisplayDefinition SentAcceptanceMail_space__ja = new DisplayDefinition();
        public DisplayDefinition ConfirmDelete = new DisplayDefinition();
        public DisplayDefinition ConfirmDelete_ja = new DisplayDefinition();
        public DisplayDefinition ConfirmSeparate = new DisplayDefinition();
        public DisplayDefinition ConfirmSeparate_ja = new DisplayDefinition();
        public DisplayDefinition ConfirmSendMail = new DisplayDefinition();
        public DisplayDefinition ConfirmSendMail_ja = new DisplayDefinition();
        public DisplayDefinition ConfirmSynchronize = new DisplayDefinition();
        public DisplayDefinition ConfirmSynchronize_ja = new DisplayDefinition();
        public DisplayDefinition CanNotUpdate = new DisplayDefinition();
        public DisplayDefinition CanNotUpdate_ja = new DisplayDefinition();
        public DisplayDefinition ReadOnlyBecausePreviousVer = new DisplayDefinition();
        public DisplayDefinition ReadOnlyBecausePreviousVer_ja = new DisplayDefinition();
        public DisplayDefinition InCopying = new DisplayDefinition();
        public DisplayDefinition InCopying_ja = new DisplayDefinition();
        public DisplayDefinition InCompression = new DisplayDefinition();
        public DisplayDefinition InCompression_ja = new DisplayDefinition();
        public DisplayDefinition HasBeenMoved = new DisplayDefinition();
        public DisplayDefinition HasBeenMoved_ja = new DisplayDefinition();
        public DisplayDefinition HasBeenDeleted = new DisplayDefinition();
        public DisplayDefinition HasBeenDeleted_ja = new DisplayDefinition();
        public DisplayDefinition ValidateRequired = new DisplayDefinition();
        public DisplayDefinition ValidateRequired_ja = new DisplayDefinition();
        public DisplayDefinition ValidateNumber = new DisplayDefinition();
        public DisplayDefinition ValidateNumber_ja = new DisplayDefinition();
        public DisplayDefinition ValidateDate = new DisplayDefinition();
        public DisplayDefinition ValidateDate_ja = new DisplayDefinition();
        public DisplayDefinition ValidateMail = new DisplayDefinition();
        public DisplayDefinition ValidateMail_ja = new DisplayDefinition();
        public DisplayDefinition ValidateEqualTo = new DisplayDefinition();
        public DisplayDefinition ValidateEqualTo_ja = new DisplayDefinition();
    }

    public class JavaScriptDefinition
    {
        public string Id; public string SavedId;
        public string Body; public string SavedBody;
        public bool NoSpace; public bool SavedNoSpace;

        public JavaScriptDefinition()
        {
        }

        public JavaScriptDefinition(Dictionary<string, string> propertyCollection)
        {
            if (propertyCollection.ContainsKey("Id")) Id = propertyCollection["Id"].ToString(); else Id = string.Empty;
            if (propertyCollection.ContainsKey("Body")) Body = propertyCollection["Body"].ToString(); else Body = string.Empty;
            if (propertyCollection.ContainsKey("NoSpace")) NoSpace = propertyCollection["NoSpace"].ToBool(); else NoSpace = false;
        }

        public object this[string key]
        {
            get{
                switch(key)
                {
                    case "Id": return Id;
                    case "Body": return Body;
                    case "NoSpace": return NoSpace;
                    default: return null;
                }
            }
        }

        public void RestoreBySavedMemory()
        {
            Id = SavedId;
            Body = SavedBody;
            NoSpace = SavedNoSpace;
        }
    }

    public class JavaScriptColumn2nd
    {
        public string Create;
        public string Update;
        public string CloseDialogAndSubmit;
        public string MoveTargets;
        public string Move;
        public string SetAggregationDetails;
        public string SendMail;
        public string Delete;
        public string WindowScrollTopAndSubmit;
        public string Submit;
        public string SetSiteImage;
        public string NewByLink;
        public string OpenDialog_ColumnProperties;
        public string EditOutgoingMail;
        public string EditExportSettings;
        public string EditImportSettings;
        public string EditSeparateSettings;
        public string AddSummary;
        public string Reply;
        public string EditMarkDown;
        public string CancelDialog;
        public string OpenDialog;
        public string Go;
        public string Import;
        public string DrawGantt;
        public string DrawBurnDown;
        public string DrawTimeSeries;
        public string SetKamban;
    }

    public class JavaScriptTable
    {
        public JavaScriptDefinition Create = new JavaScriptDefinition();
        public JavaScriptDefinition Update = new JavaScriptDefinition();
        public JavaScriptDefinition CloseDialogAndSubmit = new JavaScriptDefinition();
        public JavaScriptDefinition MoveTargets = new JavaScriptDefinition();
        public JavaScriptDefinition Move = new JavaScriptDefinition();
        public JavaScriptDefinition SetAggregationDetails = new JavaScriptDefinition();
        public JavaScriptDefinition SendMail = new JavaScriptDefinition();
        public JavaScriptDefinition Delete = new JavaScriptDefinition();
        public JavaScriptDefinition WindowScrollTopAndSubmit = new JavaScriptDefinition();
        public JavaScriptDefinition Submit = new JavaScriptDefinition();
        public JavaScriptDefinition SetSiteImage = new JavaScriptDefinition();
        public JavaScriptDefinition NewByLink = new JavaScriptDefinition();
        public JavaScriptDefinition OpenDialog_ColumnProperties = new JavaScriptDefinition();
        public JavaScriptDefinition EditOutgoingMail = new JavaScriptDefinition();
        public JavaScriptDefinition EditExportSettings = new JavaScriptDefinition();
        public JavaScriptDefinition EditImportSettings = new JavaScriptDefinition();
        public JavaScriptDefinition EditSeparateSettings = new JavaScriptDefinition();
        public JavaScriptDefinition AddSummary = new JavaScriptDefinition();
        public JavaScriptDefinition Reply = new JavaScriptDefinition();
        public JavaScriptDefinition EditMarkDown = new JavaScriptDefinition();
        public JavaScriptDefinition CancelDialog = new JavaScriptDefinition();
        public JavaScriptDefinition OpenDialog = new JavaScriptDefinition();
        public JavaScriptDefinition Go = new JavaScriptDefinition();
        public JavaScriptDefinition Import = new JavaScriptDefinition();
        public JavaScriptDefinition DrawGantt = new JavaScriptDefinition();
        public JavaScriptDefinition DrawBurnDown = new JavaScriptDefinition();
        public JavaScriptDefinition DrawTimeSeries = new JavaScriptDefinition();
        public JavaScriptDefinition SetKamban = new JavaScriptDefinition();
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
        public string SiteBreadcrumb;
        public string SiteDepts;
        public string SiteUsers;
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
        public SqlDefinition SiteBreadcrumb = new SqlDefinition();
        public SqlDefinition SiteDepts = new SqlDefinition();
        public SqlDefinition SiteUsers = new SqlDefinition();
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
}
