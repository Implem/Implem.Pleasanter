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

        public enum ConnectionTypes
        {
            System,
            Admin,
            User,
        }

        public enum TableTypes
        {
            Normal,
            History,
            HistoryWithoutFlag,
            NormalAndHistory,
            Deleted
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

        public static SqlJoinCollection SqlJoinCollection(
            params SqlJoin[] sqlJoinCollection)
        {
            return new SqlJoinCollection(sqlJoinCollection);
        }

        public static SqlParamCollection SqlParamCollection(
            params SqlParam[] sqlParamCollection)
        {
            return new SqlParamCollection(sqlParamCollection);
        }

        public static IEnumerable<string> SearchTextCollection(string searchText)
        {
            return searchText.Replace("　", " ").Split(' ')
                .Select(o => o.Trim())
                .Where(o => o != string.Empty);
        }

        public static SqlWhereCollection SqlWhereLike(
            this SqlWhereCollection self,
            string searchText,
            params string[] clauseCollection)
        {
            var searchTextCollection = Sqls.SearchTextCollection(searchText);
            return self.Add(
                name: "SearchText",
                value: searchTextCollection,
                raw: "(@SearchText#ParamCount#_#CommandCount# = '' or (" +
                    clauseCollection.Join(" or ") + "))",
                _using: searchTextCollection.Count() > 0);
        }

        public static SqlWhereCollection SqlWhereExists(
            this SqlWhereCollection self,
            string clauseFormat,
            params string[] whereCollection)
        {
            return self.Add(raw:
                clauseFormat.Replace("#SqlWhere#", whereCollection.Join(" and ")));
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
    }
}
