using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
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
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
                var targets = Rds.ExecuteTable(
                    context: context,
                    statements: Rds.SelectReminderSchedules(
                        column: Rds.ReminderSchedulesColumn()
                            .SiteId()
                            .Id()
                            .Updator()
                            .Sites_TenantId(),
                        join: Rds.ReminderSchedulesJoin()
                            .Add(
                                tableName: "Sites",
                                joinType: SqlJoin.JoinTypes.Inner,
                                joinExpression: "[Sites].[SiteId]=[ReminderSchedules].[SiteId]"),
                        where: Rds.ReminderSchedulesWhere()
                            .ScheduledTime(
                                DateTime.Now.ToLocal(),
                                _operator: "<=")))
                                    .AsEnumerable();
                targets.ForEach(dataRow => Remind(
                    context: context,
                    dataRow: dataRow));
                System.Threading.Thread.Sleep(Parameters.Reminder.Interval);
            }
            return string.Empty;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void Remind(Context context, DataRow dataRow)
        {
            context.TenantId = dataRow.Int("TenantId");
            context.UserId = dataRow.Int("Updator");
            context.DeptId = dataRow.Int("DeptId");
            SiteSettingsUtilities.Get(
                context: context,
                siteId: dataRow.Long("SiteId"),
                setSiteIntegration: true)?
                    .Remind(
                        context: context,
                        idList: dataRow.Int("Id").ToSingleList());
            Sessions.Clear("TenantId");
            Sessions.Clear("RdsUser");
        }
    }
}
