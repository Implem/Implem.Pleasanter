using Implem.CodeDefiner.Functions.Rds.Parts;
using Implem.DefinitionAccessor;
using Implem.IRds;
using Implem.Libraries.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
namespace Implem.CodeDefiner.Functions.Rds
{
    public static class TimeConverter
    {
        private static int ParseTimeOffset(string timeOffset)
        {
            if (string.IsNullOrWhiteSpace(timeOffset))
            {
                throw new ArgumentException("TimeOffset cannot be null or empty", nameof(timeOffset));
            }

            timeOffset = timeOffset.Trim();

            var pattern = @"^([+-]?)([01]?\d|2[0-3])(?::([0-5]\d))?$";
            var match = Regex.Match(timeOffset, pattern);

            if (!match.Success)
            {
                throw new FormatException($"Invalid time format: {timeOffset}. Expected format: H:mm or H (e.g., '9:00', '-5:30', '9'). Minutes are optional and must be two digits (00-59) when specified.");
            }

            var isNegative = match.Groups[1].Value == "-";
            var hours = int.Parse(match.Groups[2].Value);
            var minutes = match.Groups[3].Success ? int.Parse(match.Groups[3].Value) : 0;

            var totalMinutes = hours * 60 + minutes;
            return isNegative ? -totalMinutes : totalMinutes;
        }

        public static void Convert(
            ISqlObjectFactory factory,
            string timeOffset)
        {
            var minuteOffset = ParseTimeOffset(timeOffset);
            Convert(factory: factory, minuteOffset: minuteOffset);
        }

        public static void Convert(
            ISqlObjectFactory factory,
            int minuteOffset)
        {
            Def.ColumnDefinitionCollection
                .Where(columnDefinition => columnDefinition.TableName != "_Bases")
                .Where(columnDefinition => columnDefinition.TableName != "_BaseItems")
                .Select(columnDefinition => columnDefinition.TableName)
                .Distinct()
                .Where(tableName => !Tables.IsQuartzTable(tableName))
                .ForEach(tableName =>
                {
                    Convert(
                        factory: factory,
                        tableName: tableName,
                        suffix: string.Empty,
                        minuteOffset: minuteOffset);
                    Convert(
                        factory: factory,
                        tableName: tableName,
                        suffix: "_deleted",
                        minuteOffset: minuteOffset);
                    Convert(
                        factory: factory,
                        tableName: tableName,
                        suffix: "_history",
                        minuteOffset: minuteOffset);
                });
        }

        private static void Convert(
            ISqlObjectFactory factory,
            string tableName,
            string suffix,
            int minuteOffset)
        {
            Consoles.Write(tableName + suffix, Consoles.Types.Info);
            ConvertDateTimeColumns(
                factory: factory,
                tableName: tableName,
                suffix: suffix,
                minuteOffset: minuteOffset);
            ConvertComments(
                factory: factory,
                tableName: tableName,
                suffix: suffix,
                minuteOffset: minuteOffset);
        }

        private static void ConvertDateTimeColumns(
            ISqlObjectFactory factory,
            string tableName,
            string suffix,
            int minuteOffset)
        {
            var commandText = $"update \"{tableName}{suffix}\" set\n"
                + Def.ColumnDefinitionCollection
                    .Where(columnDefinition => columnDefinition.TableName == tableName)
                    .Where(columnDefinition => columnDefinition.TypeName == "datetime")
                    .Where(columnDefinition => !columnDefinition.NotUpdate)
                    .Select(columnDefinition => columnDefinition.ColumnName)
                    .Select(columnName => $"\"{columnName}\"={factory.Sqls.DateAddMinute(minuteOffset, $"\"{columnName}\"")}")
                    .Join("\n,");
            Def.SqlIoByAdmin(
                factory: factory)
                    .ExecuteNonQuery(
                        factory: factory,
                        dbTransaction: null,
                        dbConnection: null,
                        commandText: commandText);

        }

        private static void ConvertComments(
            ISqlObjectFactory factory,
            string tableName,
            string suffix,
            int minuteOffset)
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
                var dataTable = Def.SqlIoByAdmin(
                    factory: factory)
                        .ExecuteTable(
                            factory: factory,
                            commandText: commandText);
                dataTable.AsEnumerable().ForEach(dataRow =>
                    UpdateComments(
                        factory: factory,
                        idColumn: idColumn,
                        dataRow: dataRow,
                        suffix: suffix,
                        minuteOffset: minuteOffset));
            }
        }

        private static void UpdateComments(
            ISqlObjectFactory factory,
            ColumnDefinition idColumn,
            DataRow dataRow,
            string suffix,
            int minuteOffset)
        {
            var comments = dataRow.String("Comments").Deserialize<List<Comment>>();
            if (comments?.Any() == true)
            {
                comments.ForEach(comment =>
                {
                    comment.CreatedTime = comment.CreatedTime.AddMinutes(minuteOffset);
                    if (comment.UpdatedTime != null)
                    {
                        comment.UpdatedTime = comment.UpdatedTime.ToDateTime().AddMinutes(minuteOffset);
                    }
                    var commandText = $@"
                        update ""{idColumn.TableName}{suffix}""
                        set ""Comments""=@Comments
                        where ""{idColumn.TableName}{suffix}"".""{idColumn.ColumnName}""=@Id";
                    var io = Def.SqlIoByAdmin(
                        factory: factory);
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
