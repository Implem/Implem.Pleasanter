using Implem.Libraries.Classes;
using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlContainer
    {
        public RdsUser RdsUser;
        public string RdsName;
        public string RdsProvider;
        public string ConnectionString;
        public List<SqlStatement> SqlStatementCollection = new List<SqlStatement>();
        public int ConnectionTimeOut;
        public int CommandTimeOut;
        public bool Transactional = false;
        public bool SelectIdentity = false;
        public bool WriteSqlToDebugLog = true;

        public SqlDataAdapter SqlDataAdapter(SqlCommand sqlCommand)
        {
            var adapter = new SqlDataAdapter(sqlCommand);
            var number = string.Empty;
            SqlStatementCollection
                .Where(statement => statement.Using)
                .Where(statement => !statement.DataTableName.IsNullOrEmpty())
                .Select((o, i) => new
                {
                    Statement = o,
                    Index = i
                })
                .ForEach(data =>
                {
                    if (data.Index != 0)
                    {
                        number = data.Index.ToString();
                    }
                    adapter.TableMappings.Add(
                        sourceTable: "Table" + number,
                        dataSetTable: data.Statement.DataTableName);
                });
            return adapter;
        }
    }
}
