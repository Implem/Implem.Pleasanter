using Implem.IRds;
using Npgsql;
using System;
using System.Data;
namespace Implem.PostgreSql
{
    internal class PostgreSqlConnection : ISqlConnection
    {
        private NpgsqlConnection instance;

        internal NpgsqlConnection InnerInstance
        {
            get
            {
                return instance;
            }
        }

        public PostgreSqlConnection(string connectionString)
        {
            instance = new NpgsqlConnection(connectionString);
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

        public int ConnectionTimeout
        {
            get
            {
                return instance.ConnectionTimeout;
            }
        }

        public string Database
        {
            get
            {
                return instance.Database;
            }
        }

        public ConnectionState State
        {
            get
            {
                return instance.State;
            }
        }

        public IDbTransaction BeginTransaction()
        {
            return instance.BeginTransaction();
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            return instance.BeginTransaction(il);
        }

        public void ChangeDatabase(string databaseName)
        {
            instance.ChangeDatabase(databaseName);
        }

        public object Clone()
        {
            return ((ICloneable)instance).Clone();
        }

        public void Close()
        {
            instance.Close();
        }

        public IDbCommand CreateCommand()
        {
            return instance.CreateCommand();
        }

        public void Dispose()
        {
            instance.Dispose();
        }

        public void Open()
        {
            instance.Open();
        }
    }
}
