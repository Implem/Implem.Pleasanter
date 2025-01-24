using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.ServerScripts;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    public class BackgroundServerScriptJob : ExecutionTimerBase
    {
        public override async Task Execute(IJobExecutionContext jobContext)
        {
            // バックグランドサーバスクリプト用のQuartzから呼び出されるメソッド
            if (Parameters.Script.ServerScript != true || Parameters.Script.BackgroundServerScript != true) return;
            var dataMap = jobContext.MergedJobDataMap;
            var tenatId = dataMap.GetInt("tenantId");
            var scriptId = dataMap.GetInt("scriptId");
            var userId = dataMap.GetInt("userId");
            var scheduleId = dataMap.GetInt("scheduleId");
            var paramScripts = dataMap.GetString("scripts")?.Deserialize<BackgroundServerScripts>();
            await Task.Run(() =>
            {
                var sqlContext = CreateContext(tenantId: tenatId, userId: userId);
                try
                {
                    var ss = SiteSettingsUtilities.TenantsSiteSettings(context: sqlContext);
                    // 管理画面の即時実行時はパラメータにスクリプトが入る。それ以外はDBから取得
                    var inScripts = paramScripts ?? GetScriptsFromDB(
                        tenantId: tenatId,
                        sqlContext: sqlContext,
                        ss: ss);
                    if (inScripts != null)
                    {
                        var scripts = new List<ServerScript>();
                        scripts.AddRange(inScripts
                            .Scripts
                            .Where(s => s.Disabled != true && s.Background == true && s.Shared == true));
                        var targetScript = inScripts
                            .Scripts
                            .Where(s => s.Disabled != true && s.Background == true && s.Shared == false)
                            .FirstOrDefault(s => s.Id == scriptId);
                        if (targetScript == null || !BackgroundServerScriptValidators.CanExecuteUser(context: sqlContext, script: targetScript))
                        {
                            new SysLogModel(
                                context: sqlContext,
                                method: nameof(BackgroundServerScriptJob) + ":" + nameof(Execute),
                                message: $"Skip BGServerScript TenantId={tenatId},ScriptId={scriptId},ScheduleId={scheduleId},UserId={userId}",
                                sysLogType: SysLogModel.SysLogTypes.Info);
                            return;
                        }
                        scripts.Add(targetScript);
                        targetScript.SetDebug();
                        var log = new SysLogModel(
                            context: sqlContext,
                            method: nameof(BackgroundServerScriptJob) + ":" + nameof(Execute),
                            message: $"Exec BGServerScript TenantId={tenatId},ScriptId={scriptId},ScheduleId={scheduleId},UserId={userId}",
                            sysLogType: SysLogModel.SysLogTypes.Info);
                        var ServerScriptModelRow = ServerScriptUtilities.Execute(
                            context: sqlContext,
                            ss: ss,
                            gridData: null,
                            itemModel: null,
                            view: null,
                            scripts: scripts.ToArray(),
                            condition: ServerScriptModel.ServerScriptConditions.BackgroundServerScript,
                            debug: paramScripts != null && targetScript.Debug);
                        log.Finish(context: sqlContext);
                    }
                }
                catch (Exception e)
                {
                    new SysLogModel(
                        context: sqlContext,
                        e: e);
                }
            }, jobContext.CancellationToken);
        }

        private static BackgroundServerScripts GetScriptsFromDB(
            int tenantId,
            Context sqlContext,
            SiteSettings ss)
        {
            var dataRow = Repository.ExecuteTable(
                context: sqlContext,
                statements: Rds.SelectTenants(
                    tableType: Sqls.TableTypes.Normal,
                    column: Rds.TenantsColumn(),
                    where: Rds.TenantsWhere().TenantId(tenantId)))
                        .AsEnumerable()
                        .FirstOrDefault();
            var tenant = new TenantModel(context: sqlContext, ss: ss, dataRow: dataRow);
            return tenant.TenantSettings.BackgroundServerScripts;
        }

        private Context CreateContext(
            int tenantId,
            int userId)
        {
            var user = SiteInfo.User(
                context: new Context(tenantId: tenantId, request: false),
                userId: userId);
            var context = new Context(
                tenantId: tenantId,
                userId: userId,
                deptId: user.DeptId,
                request: false,
                setAuthenticated: true);
            context.SetTenantProperties(force: true);
            context.BackgroundServerScript = true;
            context.AbsoluteUri = Parameters.Service.AbsoluteUri;
            return context;
        }
    }
}
