using Implem.IRds;
using System;
using System.Data;
using System.Threading.Tasks;
namespace Implem.MySql
{
    internal class MySqlConnection : ISqlConnection
    {
        private MySqlConnector.MySqlConnection instance;

        internal MySqlConnector.MySqlConnection InnerInstance
        {
            get
            {
                return instance;
            }
        }

        public MySqlConnection(string connectionString)
        {
            instance = new MySqlConnector.MySqlConnection(connectionString);
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

        public async Task OpenAsync()
        {
            await instance.OpenAsync();
        }
    }
}
