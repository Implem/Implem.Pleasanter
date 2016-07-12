using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Converts;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
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
    public static class SysLogUtilities
    {
        public static ResponseCollection FormResponse(
            this ResponseCollection responseCollection, SysLogModel sysLogModel)
        {
            Forms.All().Keys.ForEach(key =>
            {
                switch (key)
                {
                    default: break;
                }
            });
            return responseCollection;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static void Maintenance()
        {
            if (Parameters.SysLog.RetentionPeriod > 0)
            {
                if ((DateTime.Now - Applications.LogMaintenanceDate).Days > 0)
                {
                    Rds.ExecuteNonQuery(statements:
                        Rds.PhysicalDeleteSysLogs(
                            where: Rds.SysLogsWhere().CreatedTime(
                                DateTime.Now.Date.AddDays(
                                    Parameters.SysLog.RetentionPeriod * -1),
                                _operator: "<")));
                    Applications.LogMaintenanceDate = DateTime.Now;
                }
            }
        }
    }
}
