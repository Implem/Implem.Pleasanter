using Implem.IRds;
using Microsoft.Data.SqlClient;
namespace Implem.SqlServer
{
    internal class SqlServerConnectionStringBuilder : ISqlConnectionStringBuilder
    {
        private SqlConnectionStringBuilder instance;

        public string ConnectionString
        {
            get
            {
                return instance.ConnectionString;
            }
            set
            {
                instance.ConnectionString = value;
            }
        }

        public string InitialCatalog
        {
            get
            {
                return instance.InitialCatalog;
            }
            set
            {
                instance.InitialCatalog = value;
            }
        }

        public string SearchPath
        {
            get
            {
                return string.Empty;
            }
            set
            {
            }
        }

        public SqlServerConnectionStringBuilder(string connectionString)
        {
            instance = new SqlConnectionStringBuilder(connectionString);
        }
    }
}
