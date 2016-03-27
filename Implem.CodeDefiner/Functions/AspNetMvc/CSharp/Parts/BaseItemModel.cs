using Implem.DefinitionAccessor;
using System.Collections.Generic;
using System.Linq;
namespace Implem.CodeDefiner.Functions.AspNetMvc.CSharp.Parts
{
    internal static class BaseItemModel
    {
        internal static void SetCodeCollection(
            CodeDefinition codeDefinition,
            List<string> codeCollection,
            DataContainer dataContainer)
        {
            var code = string.Empty;
            Def.BaseItemColumnDefinitionCollection(order: codeDefinition.Order)
                .Where(o => Column.CheckExclude(codeDefinition, o) == false)
                .Where(o => !o.EachModel)
                .ForEach(columnDefinition =>
                {
                    dataContainer.TableName = "_BaseItems";
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
