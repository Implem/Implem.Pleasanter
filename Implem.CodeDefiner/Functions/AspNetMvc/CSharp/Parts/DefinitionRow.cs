using Implem.DefinitionAccessor;
using System.Collections.Generic;
using System.Linq;
namespace Implem.CodeDefiner.Functions.AspNetMvc.CSharp.Parts
{
    internal static class DefinitionRow
    {
        internal static void SetCodeCollection(
            CodeDefinition codeDefinition,
            List<string> codeCollection,
            DataContainer dataContainer)
        {
            var code = string.Empty;
            dataContainer
                .XlsIoCollection[dataContainer.DefinitionName]
                .XlsSheet
                .AsEnumerable()
                .Skip(1)
                .Where(o => o[0].ToString() != string.Empty)
                .ForEach(definitionRow =>
                    Creators.SetCodeCollection(
                        ref code,
                        codeCollection,
                        codeDefinition,
                        dataContainer,
                        () => code = code.Replace(
                            "#Id#",
                            ReservedWords.ValidName(definitionRow[0].ToString()))));
        }
    }
}
