using Implem.Pleasanter.Libraries.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.DefinitionAccessor;
using System.Data;

namespace Implem.Pleasanter.Libraries.DataSources
{
    public static class Repository
    {
        public static SqlResponse ExecuteScalar_response(
            Context context,
            bool transactional = false,
            string connectionString = null,
            bool selectIdentity = false,
            params SqlStatement[] statements)
        {
            var response = Rds.ExecuteScalar(
                context: context,
                connectionString: connectionString,
                transactional: true,
                func: (transaction, connection) =>
                {
                    SqlResponse sqlResponse = null;
                    int count = 0;
                    foreach (var statement in statements)
                    {
                        if (statement.Using)
                        {
                            if (statement.SelectIdentity)
                            {
                                sqlResponse = Rds.ExecuteScalar_response(
                                    context: context,
                                    dbTransaction: transaction,
                                    dbConnection: connection,
                                    selectIdentity: selectIdentity,
                                    statements: statement);
                            }
                            else if (statement.IfDuplicated)
                            {
                                int exists = Rds.ExecuteScalar_int(
                                    context: context,
                                    dbTransaction: transaction,
                                    dbConnection: connection,
                                    statements: statement);
                                if (exists > 0)
                                {
                                    return (false,
                                    new SqlResponse
                                    {
                                        Event = "Duplicated",
                                        Id = 0,
                                        ColumnName = statement
                                        .SqlParamCollection
                                        .FirstOrDefault()?
                                        .Name,
                                    });
                                }
                            }
                            else if (statement.IfConflicted)
                            {
                                if (count == 0)
                                {
                                    return (false,
                                    new SqlResponse
                                    {
                                        Event = "Conflicted",
                                        Id = 0,
                                        Count = count,
                                    });
                                }
                                sqlResponse = new SqlResponse
                                {
                                    Id = 0,
                                    Count = count,
                                };
                            }
                            else if (statement.IsRowCount)
                            {
                                sqlResponse = new SqlResponse
                                {
                                    Id = 0,
                                    Count = count,
                                };
                            }
                            else
                            {
                                if (sqlResponse != null)
                                {
                                    statement.AdditionalParams = new SqlParam[]
                                    {
                                    new SqlParam(
                                        columnBracket:null,
                                        name: $"{Parameters.Parameter.SqlParameterPrefix}I".Substring(1), value: sqlResponse.Id)
                                    };
                                }
                                count = statement is SqlDelete
                                ? context.SqlResult.DeleteCount(
                                    data: Rds.ExecuteTable(
                                        context: context,
                                        dbTransaction: transaction,
                                        dbConnection: connection,
                                        statements: statement))
                                : Rds.ExecuteNonQuery(
                                    context: context,
                                    dbTransaction: transaction,
                                    dbConnection: connection,
                                    statements: statement);
                            }
                        }
                    }
                    return (true, sqlResponse);
                });
            return response;
        }

        public static bool ExecuteScalar_bool(
            Context context,
            bool transactional = false,
            params SqlStatement[] statements)
        {
            var response = Rds.ExecuteScalar<bool>(
                context: context,
                transactional: true,
                func: (transaction, connection) =>
                {
                    var value = Rds.ExecuteScalar_bool(
                        context: context,
                        dbTransaction: transaction,
                        dbConnection: connection,
                        statements: statements);
                    return (true, value);
                });
            return response;
        }

        public static int ExecuteScalar_int(
            Context context,
            bool transactional = false,
            params SqlStatement[] statements)
        {
            var response = Rds.ExecuteScalar<int>(
                context: context,
                transactional: true,
                func: (transaction, connection) =>
                {
                    var value = Rds.ExecuteScalar_int(
                        context: context,
                        dbTransaction: transaction,
                        dbConnection: connection,
                        statements: statements);
                    return (true, value);
                });
            return response;
        }

        public static long ExecuteScalar_long(
            Context context,
            bool transactional = false,
            params SqlStatement[] statements)
        {
            var response = Rds.ExecuteScalar<long>(
                context: context,
                transactional: true,
                func: (transaction, connection) =>
                {
                    var value = Rds.ExecuteScalar_long(
                        context: context,
                        dbTransaction: transaction,
                        dbConnection: connection,
                        statements: statements);
                    return (true, value);
                });
            return response;
        }

        public static string ExecuteScalar_string(
            Context context,
            bool transactional = false,
            params SqlStatement[] statements)
        {
            var response = Rds.ExecuteScalar<string>(
                context: context,
                transactional: true,
                func: (transaction, connection) =>
                {
                    var value = Rds.ExecuteScalar_string(
                        context: context,
                        dbTransaction: transaction,
                        dbConnection: connection,
                        statements: statements);
                    return (true, value);
                });
            return response;
        }

        public static DateTime ExecuteScalar_datetime(
            Context context,
            bool transactional = false,
            params SqlStatement[] statements)
        {
            var response = Rds.ExecuteScalar<DateTime>(
                context: context,
                transactional: true,
                func: (transaction, connection) =>
                {
                    var value = Rds.ExecuteScalar_datetime(
                        context: context,
                        dbTransaction: transaction,
                        dbConnection: connection,
                        statements: statements);
                    return (true, value);
                });
            return response;
        }

        public static int ExecuteNonQuery(
            Context context,
            string connectionString = null,
            bool transactional = false,
            bool writeSqlToDebugLog = true,
            params SqlStatement[] statements)
        {
            var response = Rds.ExecuteNonQuery(
                context: context,
                connectionString: connectionString,
                transactional: transactional,
                func: (transaction, connection) =>
                {
                    int count = Rds.ExecuteNonQuery(
                        context: context,
                        dbTransaction: transaction,
                        writeSqlToDebugLog: writeSqlToDebugLog,
                        dbConnection: connection,
                        statements: statements);
                    return (true, count);
                });
            return response;
        }

        public static DataTable ExecuteTable(
            Context context,
            bool transactional = false,
            params SqlStatement[] statements)
        {
            var table = Rds.ExecuteScalar(
                context: context,
                transactional: transactional,
                func: (transaction, connection) =>
                {
                    DataTable dataTable = Rds.ExecuteTable(
                        context: context,
                        dbTransaction: transaction,
                        dbConnection: connection,
                        statements: statements);
                    return (true, dataTable);
                });
            return table;
        }

        public static DataSet ExecuteDataSet(
            Context context,
            bool transactional = false,
            params SqlStatement[] statements)
        {
            var set = Rds.ExecuteScalar(
                context: context,
                transactional: transactional,
                func: (transaction, connection) =>
                {
                    DataSet dataSet = Rds.ExecuteDataSet(
                        context: context,
                        dbTransaction: transaction,
                        dbConnection: connection,
                        statements: statements);
                    return (true, dataSet);
                });
            return set;
        }
    }
}
