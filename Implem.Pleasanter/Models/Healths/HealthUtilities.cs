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
    public static class HealthUtilities
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public static void Maintain()
        {
            var thisHour = DateTime.Now.ToString("yyyy/MM/dd HH:00:00").ToDateTime();
            if (thisHour.Hour % Parameters.Health.Interval == 0)
            {
                if (Rds.ExecuteScalar_datetime(statements:
                    Rds.SelectHealths(
                        column: Rds.HealthsColumn().CreatedTime(),
                        orderBy: Rds.HealthsOrderBy().HealthId(orderType: SqlOrderBy.Types.desc),
                        top: 1)) < thisHour)
                {
                    new HealthModel(thisHour);
                }
            }
        }
    }
}
