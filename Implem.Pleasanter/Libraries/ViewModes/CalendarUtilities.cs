using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
namespace Implem.Pleasanter.Libraries.ViewModes
{
    public static class CalendarUtilities
    {
        public static bool InRange(Context context, EnumerableRowCollection<DataRow> dataRows )
        {
            var inRange = dataRows.Count() <= Parameters.General.CalendarLimit;
            if (!inRange)
            {
                SessionUtilities.Set(
                    context: context,
                    message: Messages.TooManyCases(
                        context: context,
                        data: Parameters.General.CalendarLimit.ToString()));
            }
            return inRange;
        }

        public static bool InRangeY(Context context, EnumerableRowCollection<DataRow> dataRows, Column groupBy)
        {
            var inRange = groupBy == null
                || dataRows.Select(o => o.String(groupBy.ColumnName)).Distinct().Count() <=
                    Parameters.General.CalendarYLimit;
            if (!inRange)
            {
                SessionUtilities.Set(
                    context: context,
                    message: Messages.TooManyRowCases(
                        context: context,
                        data: Parameters.General.CalendarYLimit.ToString()));
            }
            return inRange;
        }

    }
}