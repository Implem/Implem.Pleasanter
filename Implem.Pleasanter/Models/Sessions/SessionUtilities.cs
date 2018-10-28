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
    public static class SessionUtilities
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public enum Types : int
        {
            Messages
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Get(Context context, Types type, bool remove = false)
        {
            var where = Rds.SessionsWhere()
                .SessionGuid(context.SessionGuid)
                .Type(type);
            var statements = new List<SqlStatement>()
            {
                Rds.SelectSessions(
                    column: Rds.SessionsColumn().Value(),
                    where: where)
            };
            if (remove)
            {
                statements.Add(Rds.PhysicalDeleteSessions(where: where));
            }
            return Rds.ExecuteScalar_string(
                context: context,
                statements: statements.ToArray());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static void Set(Context context, Types type, string value)
        {
            Rds.ExecuteNonQuery(
                context: context,
                statements: Rds.UpdateOrInsertSessions(
                    param: Rds.SessionsParam()
                        .SessionGuid(context.SessionGuid)
                        .Type(type)
                        .Value(value),
                    where: Rds.SessionsWhere()
                        .SessionGuid(context.SessionGuid)));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static void Set(Context context, Message message)
        {
            Set(
                context: context,
                type: Types.Messages,
                value: message.ToJson());
        }
    }
}
