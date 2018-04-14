using Implem.Libraries.Utilities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
namespace Implem.Libraries.DataSources.SqlServer
{
    public static class Sqls
    {
        public static string LogsPath;
        public static string BeginTransaction;
        public static string CommitTransaction;

        public enum TableTypes
        {
            Normal,
            History,
            HistoryWithoutFlag,
            NormalAndDeleted,
            NormalAndHistory,
            Deleted,
            All
        }

        public enum LogicalOperatorTypes
        {
            and,
            or
        }

        public enum UnionTypes
        {
            None,
            Union,
            UnionAll
        }

        public enum Functions
        {
            None,
            SingleColumn,
            Count,
            Sum,
            Min,
            Max,
            Avg
        }

        public static Functions Function(string function)
        {
            switch (function)
            {
                case "None": return Functions.None;
                case "Count": return Functions.Count;
                case "Total": return Functions.Sum;
                case "Min": return Functions.Min;
                case "Max": return Functions.Max;
                case "Average": return Functions.Avg;
                default: return Functions.None;
            }
        }

        public static string TableAndColumnBracket(string tableBracket, string columnBracket)
        {
            return columnBracket.StartsWith("[")
                ? tableBracket + "." + columnBracket
                : columnBracket.StartsWith("(")
                    ? columnBracket.Replace("#TableBracket#", tableBracket)
                    : columnBracket;
        }

        public static SqlJoinCollection SqlJoinCollection(
            params SqlJoin[] sqlFromCollection)
        {
            return new SqlJoinCollection(sqlFromCollection);
        }

        public static SqlParamCollection SqlParamCollection(
            params SqlParam[] sqlParamCollection)
        {
            return new SqlParamCollection(sqlParamCollection);
        }

        public static IEnumerable<string> SearchTextCollection(string searchText)
        {
            return searchText?.Replace("　", " ").Split(' ')
                .Select(o => o.Trim())
                .Where(o => o != string.Empty) ?? new List<string>();
        }

        public static SqlWhereCollection SqlWhereLike(
            this SqlWhereCollection self,
            string name,
            string searchText,
            List<string> clauseCollection)
        {
            var searchTextCollection = SearchTextCollection(searchText);
            return self.Add(
                tableName: null,
                name: name,
                value: searchTextCollection,
                raw: $"(@{name}#ParamCount#_#CommandCount# = '' or (" +
                    clauseCollection.Join(" or ") + "))",
                _using: searchTextCollection.Any());
        }

        public static SqlWhereCollection SqlWhereExists(
            this SqlWhereCollection self,
            string tableName,
            string clauseFormat,
            params string[] whereCollection)
        {
            return self.Add(
                tableName: tableName,
                raw: clauseFormat.Replace("#SqlWhere#", whereCollection.Join(" and ")));
        }

        public static bool TryOpenConnections(
            out int number,
            out string message,
            params string[] connectionStrings)
        {
            try
            {
                connectionStrings.ForEach(connectionString =>
                {
                    var sqlConnection = new SqlConnection(connectionString);
                    sqlConnection.Open();
                    sqlConnection.Close();
                });
                number = 0;
                message = string.Empty;
                return true;
            }
            catch (SqlException e)
            {
                number = e.Number;
                message = e.Message;
                return false;
            }
            catch (Exception e)
            {
                number = -1;
                message = e.Message;
                return false;
            }
        }

        public static string GetTableBracket(string tableName)
        {
            return "[" + tableName + "]";
        }
    }
}
