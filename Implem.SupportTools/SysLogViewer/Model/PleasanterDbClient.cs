using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Implem.SupportTools.SysLogViewer.Model
{
    public class PleasanterDbClient : DbClient
    {
        private string dbName = "Implem.Pleasanter";
        public PleasanterDbClient(string connectionString, string dbName) : base(connectionString)
        {
            
        }

        public async Task<IEnumerable<SysLogModel>> GetSysLogsAsync(int count, DateTime? startDate, DateTime? endDate)
        {
            var where = "";
            if (startDate != null)
            {
                where += $" '{startDate?.ToString("yyyy-MM-dd HH:mm:ss.fff")}' <= [CreatedTime]";
            }
            var endDate2 = endDate + TimeSpan.FromDays(1);
            var and = (where == string.Empty) ? "" : " AND";
            if(endDate2 != null)
            {
                where += $"{and} [CreatedTime] < '{endDate2?.ToString("yyyy-MM-dd HH:mm:ss.fff")}'";
            }
            if(where != string.Empty)
            {
                where = "WHERE" + where;
            }
            return await connection.QueryAsync<SysLogModel>(
                $"SELECT * FROM [{dbName}].[dbo].[SysLogs] {where} ORDER BY [CreatedTime] DESC");
        }

        public async Task<IEnumerable<SysLogModel>> GetSysLogsAsync(int count)
        {
            return await connection.QueryAsync<SysLogModel>(
                $"SELECT Top {count} * FROM [{dbName}].[dbo].[SysLogs] ORDER BY [CreatedTime] DESC");
        }
        

        public async Task<IEnumerable<SysLogModel>> GetSysLogsAsync(DateTime lastCreatedTime)
        {
            return await connection.QueryAsync<SysLogModel>(
$@"SELECT 
    * 
FROM 
    [{dbName}].[dbo].[SysLogs]
WHERE
    [CreatedTime] > '{lastCreatedTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}' 
ORDER BY 
    [CreatedTime] DESC");
        }
    }
}
