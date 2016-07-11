using Implem.CodeDefiner.Functions.AspNetMvc.CSharp.Parts;
using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using System.Data;
using System.Linq;
namespace Implem.CodeDefiner.Functions.AspNetMvc.CSharp
{
    internal class MvcCreator
    {
        internal static void Create(string target)
        {
            CreateEachTable(target);
            CreateNotRepeat(target);
        }

        private static void CreateEachTable(string target)
        {
            Def.TableNameCollection().ForEach(tableName =>
                Def.CodeDefinitionCollection
                    .Where(o => target.IsNullOrEmpty() || o.Id == target)
                    .Where(o => o.Source == "Mvc")
                    .Where(o => o.RepeatType == "Table")
                    .Where(o => !Table.CheckExclude(o, tableName))
                    .ForEach(codeDefinition =>
                    {
                        var dataContainer = new DataContainer("Table");
                        var modelName = Def.ModelNameByTableName(tableName);
                        dataContainer.TableName = tableName;
                        dataContainer.ModelName = modelName;
                        var code = ReplacePlaceholder(
                            Creators.Create(codeDefinition, dataContainer), tableName, modelName);
                        var fileName = ReplacePlaceholder(
                            Directories.Outputs(codeDefinition.OutputPath), tableName, modelName);
                        Merger.Merge(fileName, code, codeDefinition.MergeToExisting);
                    }));
        }

        private static string ReplacePlaceholder(string self, string tableName, string modelName)
        {
            return self
                .Replace("#ServiceName#", Environments.ServiceName)
                .Replace("#TableName#", tableName)
                .Replace("#tableName#", tableName.ToLowerFirstChar())
                .Replace("#ModelName#", modelName)
                .Replace("#modelName#", modelName.ToLowerFirstChar());
        }

        private static void CreateNotRepeat(string target)
        {
            Def.CodeDefinitionCollection
                .Where(o => target.IsNullOrEmpty() || o.Id == target)
                .Where(o => o.Source == "Mvc")
                .Where(o => o.RepeatType == string.Empty)
                .ForEach(codeDefinition =>
                    Merger.Merge(
                        Directories.Outputs(codeDefinition.OutputPath),
                        Creators.Create(codeDefinition, new DataContainer("Table")),
                        codeDefinition.MergeToExisting));
        }
    }
}
