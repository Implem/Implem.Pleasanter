﻿using Implem.Libraries.Utilities;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace Implem.Libraries.DataSources.SqlServer
{
    public static class SqlDebugs
    {
        private static object WriteLockObject = new object();

        [Conditional("DEBUG")]
        public static void WriteSqlLog(string rdsName, SqlCommand sqlCommand, string logsPath, ref string commandText)
        {
            var commandTextForDebugging = new StringBuilder();
            commandTextForDebugging.Append("use \"", rdsName, "\";\r\n");
            commandTextForDebugging.Append(DeclareParametersText(sqlCommand));
            commandTextForDebugging.Append(FormattedCommandText(sqlCommand));
            var commandTextStringForDebugging = commandTextForDebugging.ToString();
            commandText = commandTextStringForDebugging;
            Task.Run(() =>
            {
                lock (WriteLockObject)
                {
                    commandTextStringForDebugging.Write(Path.Combine(logsPath, "CommandTextForDebugging.sql"));
                }
            });
        }

        private static string FormattedCommandText(SqlCommand sqlCommand)
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

        private static string DeclareParametersText(SqlCommand sqlCommand)
        {
            var commandParameters = new StringBuilder();
            foreach (SqlParameter parameter in sqlCommand.Parameters)
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

        private static string DeclareParameterText(SqlParameter parameter)
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

        private static string GetParameterName(SqlParameter parameter)
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
