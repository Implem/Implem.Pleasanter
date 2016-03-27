using Implem.DefinitionAccessor;
using System.Collections.Generic;
using System.Linq;
namespace Implem.CodeDefiner.Functions.AspNetMvc.CSharp.Parts
{
    internal static class BaseModel
    {
        internal static void SetCodeCollection(
            CodeDefinition codeDefinition,
            List<string> codeCollection,
            DataContainer dataContainer)
        {
            var code = string.Empty;
            Def.BaseColumnDefinitionCollection(order: codeDefinition.Order)
                .Where(o => !Column.CheckExclude(codeDefinition, o))
                .Where(o => !o.EachModel)
                .ForEach(columnDefinition =>
                {
                    dataContainer.TableName = "_Bases";
                    dataContainer.ColumnName = columnDefinition.ColumnName;
                    Creators.SetCodeCollection(
                        ref code,
                        codeCollection,
                        codeDefinition,
                        dataContainer,
                        () => Column.ReplaceCode(
                            ref code,
                            codeDefinition,
                            columnDefinition));
                    dataContainer.TableName = string.Empty;
                    dataContainer.ColumnName = string.Empty;
                });
        }
    }
}
