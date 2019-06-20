using Implem.IRds;
using Npgsql;
using System;
using System.Data;
namespace Implem.PostgreSql
{
    class PostgreSqlConnection : ISqlConnection
    {
        NpgsqlConnection _instance;
        internal NpgsqlConnection InnerInstance => _instance;

        public PostgreSqlConnection(string connectionString)
        {
            //TODO 接続文字列
            _instance = new NpgsqlConnection(connectionString);
            //_instance = new NpgsqlConnection("Server=192.168.56.102;Port=5432;Database=postgres;UID=postgres;PWD=postgres");
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
