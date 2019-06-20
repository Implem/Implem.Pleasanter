using System;
using System.Data.SqlClient;

namespace Implem.SupportTools.SysLogViewer.Model
{
    public class DbClient : IDisposable
    {
        protected SqlConnection connection;
        protected SqlTransaction transaction;

        public DbClient(string connectionString)
        {
            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();
            }
            catch (Exception e)
            {
                throw new DbClientConnectionException($"Db接続に失敗しました。(ConnectionString={connectionString})", e);
            }
        }

        public void BeginTransaction()
        {
            transaction?.Rollback();
            transaction = connection.BeginTransaction();
        }

        public void Commit()
        {
            transaction?.Commit();
            transaction = null;
        }

        public void Rollback()
        {
            transaction?.Rollback();
            transaction = null;
        }

        public void Dispose()
        {
            connection.Dispose();
        }

    }


}
