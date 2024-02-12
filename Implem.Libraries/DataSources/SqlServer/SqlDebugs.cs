using Implem.IRds;
using Implem.Libraries.Utilities;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
namespace Implem.Libraries.DataSources.SqlServer
{
    public static class SqlDebugs
    {
        private static object WriteLockObject = new object();

        [Conditional("DEBUG")]
        public static void WriteSqlLog(string provider, string rdsName, ISqlCommand sqlCommand, string logsPath)
        {
            switch (provider)
            {
                case "SQLServer":
                    WriteSqlServerLog(rdsName, sqlCommand, logsPath);
                        break;
                case "PostgreSQL":
                    WritePostgreSqlLog(sqlCommand, logsPath);
                    break;
            }
        }
               
        public static void WriteSqlServerLog(string rdsName, ISqlCommand sqlCommand, string logsPath)
        {
            var commandTextForDebugging = new StringBuilder();
            commandTextForDebugging.Append("use [", rdsName, "];\r\n");
            commandTextForDebugging.Append(DeclareParametersText(sqlCommand));
            commandTextForDebugging.Append(FormattedCommandText(sqlCommand));
            lock (WriteLockObject)
            {
                commandTextForDebugging.ToString()
                    .Write(Path.Combine(logsPath, "CommandTextForDebugging.sql"));
            }
        }
               
        public static void WritePostgreSqlLog(ISqlCommand sqlCommand, string logsPath)
        {
            var commandTextForDebugging = FormattedCommandText(sqlCommand);
            var parameters = sqlCommand.SqlParameters()
                .Select(o => new
                {
                    Name = "@" + GetParameterName(o),
                    Value = GetParameterValue(o)
                })
                .OrderByDescending(o => o.Name.Length);
            foreach (var param in parameters)
            {
                commandTextForDebugging = commandTextForDebugging
                    .Replace(param.Name, param.Value);
            }
            var comment = $$"""
                /*
                {{DeclareParametersText(sqlCommand)}}
                */

                """;
            lock (WriteLockObject)
            {
                (comment + commandTextForDebugging)
                    .Write(Path.Combine(logsPath, "CommandTextForDebugging.sql"));
            }
        }

        private static string GetParameterValue(ISqlParameter parameter)
        {
            return parameter.DbType switch
            {
                System.Data.DbType.String
                    or System.Data.DbType.AnsiString
                    or System.Data.DbType.AnsiStringFixedLength
                    or System.Data.DbType.StringFixedLength
                    or System.Data.DbType.Date
                    or System.Data.DbType.DateTime
                    or System.Data.DbType.DateTime2
                    or System.Data.DbType.DateTimeOffset => $"'{parameter.Value.ToStr()}'",
                _ => parameter.Value.ToStr()
            };
        }

        private static string FormattedCommandText(ISqlCommand sqlCommand)
        {
            var commandTextFormatted = new StringBuilder();
            var commandTextTemp = sqlCommand.CommandText;
            commandTextTemp.RegexValues(
                NewLineDelimiters().Select(o => @"(?<!\n)\b" + o + @"\b").Join("|"),
                RegexOptions.Multiline)
                    .Distinct()
                    .ForEach(match =>
                        commandTextTemp = commandTextTemp.Replace(match, "\r\n" + match));
            commandTextTemp.SplitReturn().ForEach(line =>
                commandTextFormatted.Append(line.Split(',')
                    .Chunk(3)
                    .Select(o => o.Join(", "))
                    .Join(",\r\n") + "\r\n"));
            return commandTextFormatted.ToString();
        }

        private static string DeclareParametersText(ISqlCommand sqlCommand)
        {
            var commandParameters = new StringBuilder();
            foreach (ISqlParameter parameter in sqlCommand.SqlParameters())
            {
                commandParameters.Append(
                    "{0, -50}".Params(DeclareParameterText(parameter)),
                    "set @",
                    GetParameterName(parameter),
                    " = '",
                    parameter.Value.ToStr(),
                    "';\r\n");
            }
            return commandParameters.ToString();
        }

        private static string DeclareParameterText(ISqlParameter parameter)
        {
            return parameter.Size == 0
                ? "declare @{0} {1}; ".Params(
                    GetParameterName(parameter),
                    parameter.SqlDbType.ToString().ToLower())
                : "declare @{0} {1}({2}); ".Params(
                    GetParameterName(parameter),
                    parameter.SqlDbType.ToString().ToLower(),
                    parameter.Size);
        }

        private static string GetParameterName(ISqlParameter parameter)
        {
            return parameter.ParameterName.StartsWith("@")
                ? parameter.ParameterName.Substring(1)
                : parameter.ParameterName;
        }

        private static string[] NewLineDelimiters()
        {
            return new string[]
            {
                "begin", "commit", "select", "insert into", "values", "update",
                "from", "inner join", "left outer join", "where", "group by", "having",
                "order by"
            };
        }
    }
}
