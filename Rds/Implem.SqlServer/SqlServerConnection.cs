using Implem.IRds;
using System;
using System.Data;
using System.Data.SqlClient;
namespace Implem.SqlServer
{
    class SqlServerConnection : ISqlConnection
    {
        SqlConnection _instance;
        internal SqlConnection InnerInstance => _instance;

        public SqlServerConnection(string connectionString)
        {
            _instance = new SqlConnection(connectionString);
        }

        public string ConnectionString { get => _instance.ConnectionString; set => _instance.ConnectionString = value; }

        public int ConnectionTimeout => _instance.ConnectionTimeout;

        public string Database => _instance.Database;

        public ConnectionState State => _instance.State;

        public IDbTransaction BeginTransaction()
        {
            return _instance.BeginTransaction();
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            return _instance.BeginTransaction(il);
        }

        public void ChangeDatabase(string databaseName)
        {
            _instance.ChangeDatabase(databaseName);
        }

        public object Clone()
        {
            return ((ICloneable)_instance).Clone();
        }

        public void Close()
        {
            _instance.Close();
        }

        public IDbCommand CreateCommand()
        {
            return _instance.CreateCommand();
        }

        public void Dispose()
        {
            _instance.Dispose();
        }

        public void Open()
        {
            _instance.Open();
        }
    }
}
