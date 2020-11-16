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
using System.Web;
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
                            .Sites_Updator()
                            .Sites_TenantId()
                            .Users_DeptId(),
                        join: Rds.ReminderSchedulesJoin()
                            .Add(
                                tableName: "Sites",
                                joinType: SqlJoin.JoinTypes.Inner,
                                joinExpression: "[Sites].[SiteId]=[ReminderSchedules].[SiteId]")
                            .Add(
                                tableName: "Users",
                                joinType: SqlJoin.JoinTypes.LeftOuter,
                                joinExpression: "[Users].[UserId]=[Sites].[Updator]"),
                        where: Rds.ReminderSchedulesWhere()
                            .ScheduledTime(
                                DateTime.Now.ToLocal(context: context),
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
            context = new Context(
                tenantId: dataRow.Int("TenantId"),
                userId: dataRow.Int("Updator"),
                deptId: dataRow.Int("DeptId"));
            SiteSettingsUtilities.Get(
                context: context,
                siteId: dataRow.Long("SiteId"),
                setSiteIntegration: true)?
                    .Remind(
                        context: context,
                        idList: dataRow.Int("Id").ToSingleList());
        }
    }
}
