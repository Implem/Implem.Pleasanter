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
        public PleasanterDbClient(string connectionString) : base(connectionString)
        {
            
        }

        public async Task<IEnumerable<SysLogModel>> GetSysLogsAsync(int count)
        {
            return await connection.QueryAsync<SysLogModel>(
                $"SELECT Top {count} * FROM [Implem.Pleasanter].[dbo].[SysLogs] ORDER BY [CreatedTime] DESC");
        }

        public async Task<IEnumerable<SysLogModel>> GetSysLogsAsync(DateTime lastCreatedTime)
        {
            return await connection.QueryAsync<SysLogModel>(
$@"SELECT 
    * 
FROM 
    [Implem.Pleasanter].[dbo].[SysLogs]
WHERE
    [CreatedTime] > '{lastCreatedTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}' 
ORDER BY 
    [CreatedTime] DESC");
        }
    }
}
