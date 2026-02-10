using Npgsql;
using Implem.IRds;
namespace Implem.PostgreSql
{
    internal class PostgreSqlConnectionStringBuilder : ISqlConnectionStringBuilder
    {
        private NpgsqlConnectionStringBuilder instance;

        internal NpgsqlConnectionStringBuilder InnerInstance
        {
            get
            {
                return instance;
            }
        }

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
                return instance.Database;
            }
            set
            {
                instance.Database = value;
            }
        }

        public string SearchPath
        {
            get
            {
                return instance.SearchPath;
            }
            set
            {
                instance.SearchPath = value;
            }
        }

        public PostgreSqlConnectionStringBuilder(string connectionString)
        {
            instance = new NpgsqlConnectionStringBuilder(connectionString);
        }
    }
}
