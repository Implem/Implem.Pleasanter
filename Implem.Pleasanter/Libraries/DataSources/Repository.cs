using Implem.Pleasanter.Libraries.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;

namespace Implem.Pleasanter.Libraries.DataSources
{
    public static class Repository
    {
        public static SqlResponse ExecuteScalar_response(Context context, bool transactional, params SqlStatement[] statements)
        {
            var response = Rds.ExecuteScalar(
                context: context,
                transactional: true,
                func: (transaction, connection) =>
                {
                    var statementListList = statements
                    .Aggregate(
                        new List<List<SqlStatement>>(),
                        (list, statement) =>
                        {
                            if (statement.SelectIdentity ||
                            statement.IfConflicted ||
                            statement.IfDuplicated)
                            {
                                list.Add(
                                    new List<SqlStatement> { statement });
                                return list;
                            }
                            if (list.LastOrDefault()?
                            .LastOrDefault()?
                            .SelectIdentity == true)
                            {
                                list.Add(new List<SqlStatement>());
                            }
                            if (!list.Any())
                            {
                                list.Add(new List<SqlStatement>());
                            }
                            list.Last().Add(statement);
                        return list;
                    });
                    SqlResponse sqlResponse = null;
                    int count = 0;
                    foreach (var statementList in statementListList)
                    {
                        if (statementList.First().SelectIdentity)
                        {
                            sqlResponse = Rds.ExecuteScalar_response(
                                context: context,
                                dbTransaction: transaction,
                                dbConnection: connection,
                                statements: statementList.First());
                        }
                        else if (statementList.First().IfDuplicated)
                        {
                            int exists = Rds.ExecuteScalar_int(
                                context: context,
                                dbTransaction: transaction,
                                dbConnection: connection,
                                statements: statementList.First());
                            if (exists > 0)
                            {
                                return (false,
                                new SqlResponse
                                {
                                    Event = "Duplicated",
                                    Id = 0,
                                    ColumnName = "Title",
                                });
                            }
                        }
                        else if (statementList.First().IfConflicted)
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
                        else
                        {
                            statementList?
                                .Where(st => st.SqlParamCollection != null)
                                .SelectMany(
                                st => st.SqlParamCollection)
                                .Where(param => param.Raw == "@_I")
                                .ForEach(param =>
                                {
                                    param.Raw = null;
                                    param.Value = sqlResponse.Id;
                                });
                            count = Rds.ExecuteNonQuery(
                                context: context,
                                dbTransaction: transaction,
                                dbConnection: connection,
                                statements: statementList.ToArray());
                        }
                    }
                    return (true, sqlResponse);
                }
                );
            return response;
        }

        public static int ExecuteNonQuery(Context context, bool transactional, params SqlStatement[] statements)
        {
            var response = Rds.ExecuteNonQuery(
                context: context,
                transactional: true,
                func: (transaction, connection) =>
                {
                    int count = Rds.ExecuteNonQuery(
                        context: context,
                        dbTransaction: transaction,
                        dbConnection: connection,
                        statements: statements);
                    return (true, count);
                });
            return response;
        }
    }
}
