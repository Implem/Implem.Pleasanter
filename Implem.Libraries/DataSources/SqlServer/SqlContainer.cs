using Implem.Libraries.Classes;
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
        public bool WriteSqlToDebugLog = true;
        public List<string> DataTableNames = new List<string>();

        public SqlDataAdapter SqlDataAdapter(SqlCommand sqlCommand)
        {
            var sqlDataAdapter = new SqlDataAdapter(sqlCommand);
            var number = string.Empty;
            DataTableNames.Select((o, i) => new { TableName = o, Index = i }).ForEach(data =>
            {
                if (data.Index != 0) number = data.Index.ToString();
                sqlDataAdapter.TableMappings.Add("Table" + number, data.TableName);
            });
            return sqlDataAdapter;
        }
    }
}
