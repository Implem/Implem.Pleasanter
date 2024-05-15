﻿using Npgsql;
using Implem.IRds;
namespace Implem.MySql
{
    internal class MySqlConnectionStringBuilder : ISqlConnectionStringBuilder
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
                instance.Database = value; ;
            }
        }

        public MySqlConnectionStringBuilder(string connectionString)
        {
            instance = new NpgsqlConnectionStringBuilder(connectionString);
        }
    }
}
