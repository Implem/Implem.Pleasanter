using Implem.IRds;
using System.Data.SqlClient;
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

        public SqlServerConnectionStringBuilder(string connectionString)
        {
            instance = new SqlConnectionStringBuilder(connectionString);
        }
    }
}
