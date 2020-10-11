using Implem.Libraries.DataSources.SqlServer;
using Implem.Pleasanter.Libraries.Requests;
using System;
using System.Collections.Generic;
using System.Data;
namespace Implem.Pleasanter.Libraries.DataSources
{
    public static class Repository
    {
        public static SqlResponse ExecuteScalar_response(
            Context context,
            string connectionString = null,
            bool transactional = false,
            bool selectIdentity = false,
            bool writeSqlToDebugLog = true,
            params SqlStatement[] statements)
        {
            return Rds.ExecuteScalar_response(
                context: context,
                connectionString: connectionString,
                transactional: transactional,
                selectIdentity: selectIdentity,
                writeSqlToDebugLog: writeSqlToDebugLog,
                statements: statements);
        }

        public static List<SqlResponse> ExecuteDataSet_responses(
            Context context,
            string connectionString = null,
            bool transactional = false,
            bool writeSqlToDebugLog = true,
            params SqlStatement[] statements)
        {
            return Rds.ExecuteDataSet_responses(
                context: context,
                connectionString: connectionString,
                transactional: transactional,
                writeSqlToDebugLog: writeSqlToDebugLog,
                statements: statements);
        }

        public static bool ExecuteScalar_bool(
            Context context,
            string connectionString = null,
            bool transactional = false,
            params SqlStatement[] statements)
        {
            return Rds.ExecuteScalar_bool(
                context: context,
                connectionString: connectionString,
                transactional: transactional,
                statements: statements);
        }

        public static int ExecuteScalar_int(
            Context context,
            string connectionString = null,
            bool transactional = false,
            params SqlStatement[] statements)
        {
            return Rds.ExecuteScalar_int(
                context: context,
                connectionString: connectionString,
                transactional: transactional,
                statements: statements);
        }

        public static long ExecuteScalar_long(
            Context context,
            string connectionString = null,
            bool transactional = false,
            params SqlStatement[] statements)
        {
            return Rds.ExecuteScalar_long(
                context: context,
                connectionString: connectionString,
                transactional: transactional,
                statements: statements);
        }

        public static decimal ExecuteScalar_decimal(
            Context context,
            string connectionString = null,
            bool transactional = false,
            params SqlStatement[] statements)
        {
            return Rds.ExecuteScalar_decimal(
                context: context,
                connectionString: connectionString,
                transactional: transactional,
                statements: statements);
        }

        public static string ExecuteScalar_string(
            Context context,
            string connectionString = null,
            bool transactional = false,
            params SqlStatement[] statements)
        {
            return Rds.ExecuteScalar_string(
                context: context,
                connectionString: connectionString,
                transactional: transactional,
                statements: statements);
        }

        public static DateTime ExecuteScalar_datetime(
            Context context,
            string connectionString = null,
            bool transactional = false,
            params SqlStatement[] statements)
        {
            return Rds.ExecuteScalar_datetime(
                context: context,
                connectionString: connectionString,
                transactional: transactional,
                statements: statements);
        }

        public static byte[] ExecuteScalar_bytes(
            Context context,
            string connectionString = null,
            bool transactional = false,
            params SqlStatement[] statements)
        {
            return Rds.ExecuteScalar_bytes(
                context: context,
                connectionString: connectionString,
                transactional: transactional,
                statements: statements);
        }

        public static void ExecuteNonQuery(
            Context context,
            string connectionString = null,
            bool transactional = false,
            bool writeSqlToDebugLog = true,
            params SqlStatement[] statements)
        {
            Rds.ExecuteNonQuery(
                context: context,
                connectionString: connectionString,
                transactional: transactional,
                writeSqlToDebugLog: writeSqlToDebugLog,
                statements: statements);
        }

        public static DataTable ExecuteTable(
            Context context,
            string connectionString = null,
            bool transactional = false,
            params SqlStatement[] statements)
        {
            return Rds.ExecuteTable(
                context: context,
                connectionString: connectionString,
                transactional: transactional,
                statements: statements);
        }

        public static DataSet ExecuteDataSet(
            Context context,
            string connectionString = null,
            bool transactional = false,
            params SqlStatement[] statements)
        {
            return Rds.ExecuteDataSet(
                context: context,
                connectionString: connectionString,
                transactional: transactional,
                statements: statements);
        }
    }
}
