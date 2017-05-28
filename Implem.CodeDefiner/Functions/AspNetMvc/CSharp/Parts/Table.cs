using Implem.CodeDefiner.Utilities;
using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Linq;
namespace Implem.CodeDefiner.Functions.AspNetMvc.CSharp.Parts
{
    internal static class Table
    {
        internal static void SetCodeCollection(
            CodeDefinition codeDefinition,
            List<string> codeCollection,
            DataContainer dataContainer)
        {
            dataContainer.Type = "Table";
            var code = string.Empty;
            var parentTableName = dataContainer.TableName;
            var parentModelName = dataContainer.ModelName;
            Def.TableNameCollection(order: codeDefinition.Order)
                .Where(o => !CheckExclude(codeDefinition, o))
                .ForEach(tableName =>
                {
                    dataContainer.TableName = tableName;
                    dataContainer.ModelName = Def.ModelNameByTableName(tableName);
                    Creators.SetCodeCollection(
                        ref code,
                        codeCollection,
                        codeDefinition,
                        dataContainer,
                        () => ReplaceCode(ref code, codeDefinition, dataContainer));
                });
            dataContainer.TableName = parentTableName;
            dataContainer.ModelName = parentModelName;
        }

        internal static bool CheckExclude(CodeDefinition codeDefinition, string tableName)
        {
            var columns = Def.ColumnDefinitionCollection
                .Where(o => o.TableName == tableName)
                .ToList();
            if (codeDefinition.ItemOnly && !columns.Any(o => o.ItemId > 0)) return true;
            if (codeDefinition.NotItem && columns.Any(o => o.ItemId > 0)) return true;
            if (codeDefinition.GenericUi && !columns.Any(o => o.GenericUi)) return true;
            if (codeDefinition.UpdateMonitor && !columns.Any(o => o.UpdateMonitor)) return true;
            if (codeDefinition.HasIdentity && !columns.Any(o => o.Identity)) return true;
            if (codeDefinition.HasNotIdentity && columns.Any(o => o.Identity)) return true;
            if (codeDefinition.HasTableNameId && tableName.CsTypeIdColumn() == string.Empty) return true;
            if (codeDefinition.HasNotTableNameId && tableName.CsTypeIdColumn() != string.Empty) return true;
            if (codeDefinition.Exclude.Split(',').Contains(tableName)) return true;
            if (codeDefinition.Include != string.Empty && !codeDefinition.Include.Split(',').Contains(tableName)) return true;
            return false;
        }

        private static void ReplaceCode(
            ref string code,
            CodeDefinition codeDefinition,
            DataContainer dataContainer)
        {
            foreach (var placeholder in code.RegexValues(CodePatterns.ReplacementPlaceholder))
            {
                switch (placeholder)
                {
                    case "ModelName":
                        code = code.Replace("#ModelName#", dataContainer.ModelName);
                        break;
                    case "modelName":
                        code = code.Replace("#modelName#", dataContainer.ModelName.ToLowerFirstChar());
                        break;
                    case "modelname":
                        code = code.Replace("#modelname#", dataContainer.ModelName.ToLower());
                        break;
                    case "TableName":
                        code = code.Replace("#TableName#", dataContainer.TableName);
                        break;
                    case "tableName":
                        code = code.Replace("#tableName#", dataContainer.TableName.ToLowerFirstChar());
                        break;
                    case "tablename":
                        code = code.Replace("#tablename#", dataContainer.TableName.ToLower());
                        break;
                }
            }
            if (codeDefinition.ReplaceOld != string.Empty)
            {
                code = code.Replace(codeDefinition.ReplaceOld, codeDefinition.ReplaceNew);
            }
        }
    }
}
