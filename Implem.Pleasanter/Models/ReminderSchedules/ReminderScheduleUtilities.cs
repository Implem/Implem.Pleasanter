using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.Interfaces;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Resources;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Web;
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
namespace Implem.Pleasanter.Models
{
    public static class ReminderScheduleUtilities
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Remind(Context context)
        {
            var now = DateTime.Now;
            while ((DateTime.Now - now).Seconds <= Parameters.Reminder.Span)
            {
                var targets = Repository.ExecuteTable(
                    context: context,
                    statements: Rds.SelectReminderSchedules(
                        column: Rds.ReminderSchedulesColumn()
                            .SiteId()
                            .Id()
                            .ScheduledTime()
                            .Sites_Updator()
                            .Sites_TenantId()
                            .Users_DeptId()
                            .Tenants_ContractDeadline(),
                        join: Rds.ReminderSchedulesJoin()
                            .Add(
                                tableName: "\"Sites\"",
                                joinType: SqlJoin.JoinTypes.Inner,
                                joinExpression: "\"Sites\".\"SiteId\"=\"ReminderSchedules\".\"SiteId\"")
                            .Add(
                                tableName: "\"Users\"",
                                joinType: SqlJoin.JoinTypes.LeftOuter,
                                joinExpression: "\"Users\".\"UserId\"=\"Sites\".\"Updator\"")
                            .Add(
                                tableName: "\"Tenants\"",
                                joinType: SqlJoin.JoinTypes.LeftOuter,
                                joinExpression: "\"Tenants\".\"TenantId\"=\"Sites\".\"TenantId\""),
                        where: Rds.ReminderSchedulesWhere()
                            .ScheduledTime(
                                DateTime.Now.ToLocal(context: context),
                                _operator: "<=")))
                                    .AsEnumerable();
                targets.ForEach(dataRow =>
                {
                    if (!dataRow.IsNull("ContractDeadline")
                        && dataRow.DateTime("ContractDeadline") < DateTime.Now)
                    {
                        Rds.ExecuteNonQuery(
                            context: context,
                            statements: Rds.DeleteReminderSchedules(
                            factory: context,
                            where: Rds.ReminderSchedulesWhere()
                                .SiteId(value: dataRow.Long("SiteId"))));
                    }
                    else
                    {
                        Remind(
                            context: context,
                            dataRow: dataRow);
                        System.Threading.Thread.Sleep(Parameters.Reminder.Interval);
                    }
                });
                if (targets.Any() == false) break;
                System.Threading.Thread.Sleep(100);
            }
            return string.Empty;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void Remind(Context context, DataRow dataRow)
        {
            if (Parameters.BackgroundService.Reminder)
            {
                context = new Context(
                    tenantId: dataRow.Int("TenantId"),
                    userId: dataRow.Int("Updator"),
                    deptId: dataRow.Int("DeptId"),
                    request: false,
                    setAuthenticated: true);
                context.AbsoluteUri = Parameters.Service.AbsoluteUri;
            }
            else
            {
                context = new Context(
                    tenantId: dataRow.Int("TenantId"),
                    userId: dataRow.Int("Updator"),
                    deptId: dataRow.Int("DeptId"),
                    setAuthenticated: true);
            }
            context.ServerScriptDisabled = true;
            SiteSettingsUtilities.Get(
                context: context,
                siteId: dataRow.Long("SiteId"),
                setSiteIntegration: true)
                    ?.Remind(
                        context: context,
                        idList: dataRow.Int("Id").ToSingleList(),
                        scheduledTime: dataRow.DateTime("ScheduledTime"));
        }
    }
}
