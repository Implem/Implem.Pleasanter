using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Linq;
namespace Implem.CodeDefiner.Functions.AspNetMvc.CSharp.Parts
{
    internal static class JoinFrom
    {
        internal static void SetCodeCollection(
            CodeDefinition codeDefinition,
            List<string> codeCollection,
            DataContainer dataContainer)
        {
            var code = string.Empty;
            var prosecced = new List<string>();
            Def.ColumnDefinitionCollection
                .Where(o => !Column.CheckExclude(codeDefinition, o))
                .Where(o => o.TableName == dataContainer.TableName)
                .Where(o => o.JoinTableName != string.Empty)
                .OrderBy(o => o.No)
                .ForEach(columnDefinition =>
                {
                    dataContainer.ColumnName = columnDefinition.ColumnName;
                    var tableNameAlias = Join.TableNameAlias(columnDefinition);
                    if (!prosecced.Any(o => o == tableNameAlias))
                    {
                        Creators.SetCodeCollection(
                            ref code, 
                            codeCollection, 
                            codeDefinition, 
                            dataContainer, () =>
                            {
                                Join.ReplaceCodeOfJoin(
                                    ref code,
                                    codeDefinition,
                                    columnDefinition,
                                    tableNameAlias: Join.TableNameAlias(columnDefinition),
                                    columnNameAlias: columnDefinition
                                        .SelectColumns.IsNotEmpty(columnDefinition.ColumnName),
                                    joinExpression: Join.Epression(
                                        columnDefinition,
                                        tableNameAlias,
                                        dataContainer.TableName));
                            });
                        prosecced.Add(tableNameAlias);
                    }
                    dataContainer.ColumnName = string.Empty;
                });
        }
    }
}
