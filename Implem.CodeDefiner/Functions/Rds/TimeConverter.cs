using Implem.DefinitionAccessor;
using Implem.IRds;
using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.CodeDefiner.Functions.Rds
{
    public static class TimeConverter
    {
        public static void Convert(ISqlObjectFactory factory)
        {
            Def.ColumnDefinitionCollection
                .Where(columnDefinition => columnDefinition.TableName != "_Bases")
                .Where(columnDefinition => columnDefinition.TableName != "_BaseItems")
                .Select(columnDefinition => columnDefinition.TableName)
                .Distinct()
                .ForEach(tableName =>
                {
                    Convert(
                        factory: factory,
                        tableName: tableName,
                        suffix: string.Empty,
                        offset: -9);
                    Convert(
                        factory: factory,
                        tableName: tableName,
                        suffix: "_deleted",
                        offset: -9);
                    Convert(
                        factory: factory,
                        tableName: tableName,
                        suffix: "_history",
                        offset: -9);
                });
        }

        private static void Convert(
            ISqlObjectFactory factory,
            string tableName,
            string suffix,
            int offset)
        {
            Consoles.Write(tableName + suffix, Consoles.Types.Info);
            ConvertDateTimeColumns(
                factory: factory,
                tableName: tableName,
                suffix: suffix,
                offset: offset);
            ConvertComments(
                factory: factory,
                tableName: tableName,
                suffix: suffix);
        }

        private static void ConvertDateTimeColumns(
            ISqlObjectFactory factory,
            string tableName,
            string suffix,
            int offset)
        {
            var commandText = $"update \"{tableName}{suffix}\" set\n"
                + Def.ColumnDefinitionCollection
                    .Where(columnDefinition => columnDefinition.TableName == tableName)
                    .Where(columnDefinition => columnDefinition.TypeName == "datetime")
                    .Where(columnDefinition => !columnDefinition.NotUpdate)
                    .Select(columnDefinition => columnDefinition.ColumnName)
                    .Select(columnName => $"\"{columnName}\"=dateadd(hour, {offset}, \"{columnName}\")")
                    .Join("\n,");
            Def.SqlIoBySa(
                factory: factory,
                initialCatalog: Environments.ServiceName)
                    .ExecuteNonQuery(
                        factory: factory,
                        dbTransaction: null,
                        dbConnection: null,
                        commandText: commandText);

        }

        private static void ConvertComments(
            ISqlObjectFactory factory,
            string tableName,
            string suffix)
        {
            var idColumn = Def.ColumnDefinitionCollection
                .Where(columnDefinition => columnDefinition.TableName == tableName)
                .FirstOrDefault(columnDefinition => columnDefinition.ColumnName == columnDefinition.ModelName + "Id");
            if (idColumn != null)
            {
                var commandText = $@"
                    select
                        ""{idColumn.TableName}{suffix}"".""{idColumn.ColumnName}""
                        ,""{idColumn.TableName}{suffix}"".""Comments""
                    from ""{idColumn.TableName}{suffix}""
                    where ""{idColumn.TableName}{suffix}"".""Comments"" is not null
                        and ""{idColumn.TableName}{suffix}"".""Comments""<>'[]';";
                var dataTable = Def.SqlIoBySa(
                    factory: factory,
                    initialCatalog: Environments.ServiceName)
                        .ExecuteTable(
                            factory: factory,
                            commandText: commandText);
                dataTable.AsEnumerable().ForEach(dataRow =>
                    UpdateComments(
                        factory: factory,
                        idColumn: idColumn,
                        dataRow: dataRow,
                        suffix: suffix));
            }
        }

        private static void UpdateComments(
            ISqlObjectFactory factory,
            ColumnDefinition idColumn,
            DataRow dataRow,
            string suffix)
        {
            var comments = dataRow.String("Comments").Deserialize<List<Comment>>();
            if (comments?.Any() == true)
            {
                comments.ForEach(comment =>
                {
                    comment.CreatedTime = comment.CreatedTime.AddHours(-9);
                    if (comment.UpdatedTime != null)
                    {
                        comment.UpdatedTime = comment.UpdatedTime.ToDateTime().AddHours(-9);
                    }
                    var commandText = $@"
                        update ""{idColumn.TableName}{suffix}""
                        set ""Comments""=@Comments
                        where ""{idColumn.TableName}{suffix}"".""{idColumn.ColumnName}""=@Id";
                    var io = Def.SqlIoBySa(
                        factory: factory,
                        initialCatalog: Environments.ServiceName);
                    io.SqlCommand.Parameters_AddWithValue("Id", dataRow[idColumn.ColumnName]);
                    io.SqlCommand.Parameters_AddWithValue("Comments", comments.ToJson());
                    io.ExecuteTable(
                        factory: factory,
                        commandText: commandText);
                });
            }
        }
    }
}
