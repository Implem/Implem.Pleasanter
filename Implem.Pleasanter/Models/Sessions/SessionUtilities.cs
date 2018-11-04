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
        public static Dictionary<string, string> Get(Context context)
        {
            return Rds.ExecuteTable(
                context: context,
                statements: new SqlStatement[]
                {
                    Rds.SelectSessions(
                        column: Rds.SessionsColumn()
                            .Key()
                            .Value(),
                        where: Rds.SessionsWhere()
                            .SessionGuid(context.SessionGuid)
                            .ReadOneByOne(_operator: " is null")),
                    Rds.PhysicalDeleteSessions(
                        where: Rds.SessionsWhere()
                            .SessionGuid(context.SessionGuid)
                            .ReadOnce(1)),
                    Rds.PhysicalDeleteSessions(
                        where: Rds.SessionsWhere()
                            .UpdatedTime(
                                DateTime.Now.AddMinutes(Parameters.Session.RetentionPeriod * -1),
                                _operator: "<"))
                })
                    .AsEnumerable()
                    .ToDictionary(
                        dataRow => dataRow.String("Key"),
                        dataRow => dataRow.String("Value"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Language(Context context)
        {
            return Rds.ExecuteScalar_string(
                context: context,
                statements: Rds.SelectSessions(
                    column: Rds.SessionsColumn().Value(),
                    where: Rds.SessionsWhere()
                        .SessionGuid(context.SessionGuid)
                        .Key("Language")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static View View(Context context, string key)
        {
            return Rds.ExecuteScalar_string(
                context: context,
                statements: Rds.SelectSessions(
                    column: Rds.SessionsColumn().Value(),
                    where: Rds.SessionsWhere()
                        .SessionGuid(context.SessionGuid)
                        .Key(key))).Deserialize<View>();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static void Set(
            Context context,
            string key,
            string value,
            bool readOnce = false,
            bool readOneByOne = false)
        {
            if (context.SessionData.ContainsKey(key))
            {
                context.SessionData[key] = value;
            }
            else
            {
                context.SessionData.Add(key, value);
            }
            Rds.ExecuteNonQuery(
                context: context,
                statements: Rds.UpdateOrInsertSessions(
                    param: Rds.SessionsParam()
                        .SessionGuid(context.SessionGuid)
                        .Key(key)
                        .Value(value)
                        .ReadOnce(readOnce),
                    where: Rds.SessionsWhere()
                        .SessionGuid(context.SessionGuid)
                        .Key(key)));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static void Set(Context context, string key, View view)
        {
            Set(
                context: context,
                key: key,
                value: view.ToJson(),
                readOneByOne: true);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static void Set(Context context, Message message)
        {
            Set(
                context: context,
                key: "Message",
                value: message.ToJson(),
                readOnce: true);
        }
    }
}
