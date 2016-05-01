using Implem.CodeDefiner.Utilities;
using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Linq;
namespace Implem.CodeDefiner.Functions.AspNetMvc.CSharp.Parts
{
    internal static class DefinitionFile
    {
        internal static void SetCodeCollection(
            CodeDefinition codeDefinition,
            List<string> codeCollection,
            DataContainer dataContainer)
        {
            var code = string.Empty;
            dataContainer.XlsIoCollection.Keys.ForEach(definitionName =>
            {
                dataContainer.DefinitionName = definitionName;
                Creators.SetCodeCollection(
                    ref code,
                    codeCollection,
                    codeDefinition,
                    dataContainer,
                    () => ReplaceCode(
                        ref code,
                        codeDefinition,
                        dataContainer));
            });
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
                    case "File":
                        code = code.Replace("#File#", dataContainer.DefinitionName);
                        break;
                    case "file":
                        code = code.Replace("#file#", dataContainer.DefinitionName.ToLowerFirstChar());
                        break;
                    case "ColumnNames":
                        code = code.Replace("#ColumnNames#", ColumnNames(dataContainer));
                        break;
                }
            }
            if (codeDefinition.ReplaceOld != string.Empty)
            {
                code = code.Replace(codeDefinition.ReplaceOld, codeDefinition.ReplaceNew);
            }
        }

        private static string ColumnNames(DataContainer dataContainer)
        {
            return dataContainer
                .XlsIoCollection[dataContainer.DefinitionName]
                .XlsSheet
                .Columns
                .Select(o => "\"" + o.Replace("-", "_") + "\"")
                .Join(", ");
        }

        internal static void SetCodeCollection_Default(
            CodeDefinition codeDefinition,
            List<string> codeCollection,
            DataContainer dataContainer)
        {
            if (!CheckExcludeConditionsByDefName(codeDefinition, dataContainer))
            {
                codeCollection.Add(Creators.Create(codeDefinition, dataContainer));
            }
        }

        private static bool CheckExcludeConditionsByDefName(
            CodeDefinition codeDefinition, DataContainer dataContainer)
        {
            return codeDefinition.Include != string.Empty &&
                !codeDefinition.Include.Split(',').Contains(dataContainer.DefinitionName);
        }
    }
}
