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
using static Implem.Pleasanter.Libraries.DataSources.Rds;

namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    public class BackgroundServerScriptJob : ExecutionTimerBase
    {
        public override async Task Execute(IJobExecutionContext jobContext)
        {
            // バックグランドサーバスクリプト用のQuartzから呼び出されるメソッド
            if (Parameters.Script.ServerScript != true || Parameters.Script.BackgroundServerScript != true) return;
            var dataMap = jobContext.JobDetail.JobDataMap;
            var tenatId = dataMap.GetInt("tenantId");
            var scriptId = dataMap.GetInt("scriptId");
            var userId = dataMap.GetInt("userId");
            var paramScripts = dataMap.GetString("scripts")?.Deserialize<BackgroundServerScripts>();
            await Task.Run(() =>
            {
                var sqlContext = CreateContext(tenantId: tenatId, userId: userId);
                try
                {
                    var ss = GetSitemSettings(context: sqlContext);
                    // 管理画面の即時実行時はパラメータにスクリプトが入る。それ以外はDBから取得
                    var inScripts = paramScripts ?? GetScriptsFromDB(
                        tenatId: tenatId,
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
                                method: nameof(BackgroundServerScriptJob),
                                message: $"BackgroundServerScriptJob Execute() Skip TenantId={tenatId},ScriptId={scriptId},UserId={userId}",
                                sysLogType: SysLogModel.SysLogTypes.Info);
                            return;
                        }
                        scripts.Add(targetScript);
                        targetScript.SetDebug();
                        var ServerScriptModelRow = ServerScriptUtilities.Execute(
                            context: sqlContext,
                            ss: ss,
                            gridData: null,
                            itemModel: null,
                            view: null,
                            scripts: scripts.ToArray(),
                            condition: "BackgroundServerScript",
                            debug: paramScripts != null && targetScript.Debug);
                    }
                }
                catch (Exception ex)
                {
                    new SysLogModel(
                        context: sqlContext,
                        e: ex);
                }
            }, jobContext.CancellationToken);
        }

        private static BackgroundServerScripts GetScriptsFromDB(
            int tenatId,
            Context sqlContext,
            SiteSettings ss)
        {
            var dataRow = Repository.ExecuteTable(
                context: sqlContext,
                statements: Rds.SelectTenants(
                    tableType: Sqls.TableTypes.Normal,
                    column: TenantsColumn().TenantSettings(),
                    where: Rds.TenantsWhere().TenantId(tenatId)))
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
            return context;
        }

        private static SiteSettings GetSitemSettings(
            Requests.Context context)
        {
            return SiteSettingsUtilities.TenantsSiteSettings(context: context);
        }
    }
}
