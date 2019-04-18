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
        public static Dictionary<string, string> Get(Context context, bool includeUserArea = false)
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
                            .Add(raw: "(([UserArea] is null) or ([UserArea] <> 1))", _using: !includeUserArea)
                            .Or(or: Rds.SessionsWhere()
                                .Page(context.Page, _using: context.Page != null)
                                .Page(raw: "''"))),
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
        public static bool Bool(Context context, string key)
        {
            return Rds.ExecuteScalar_bool(
                context: context,
                statements: Rds.SelectSessions(
                    column: Rds.SessionsColumn().Value(),
                    where: Rds.SessionsWhere()
                        .SessionGuid(context.SessionGuid)
                        .Key(key)));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static void Set(
            Context context,
            string key,
            string value,
            bool readOnce = false,
            bool page = false,
            bool userArea = false)
        {
            SetContext(
                context: context,
                key: key,
                value: value);
            SetRds(
                context: context,
                key: key,
                value: value,
                readOnce: readOnce,
                page: page,
                userArea: userArea);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void SetContext(Context context, string key, string value)
        {
            if (context.SessionData.ContainsKey(key))
            {
                if (value != null)
                {
                    context.SessionData[key] = value;
                }
                else
                {
                    context.SessionData.Remove(key);
                }
            }
            else
            {
                context.SessionData.Add(key, value);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void SetRds(
            Context context,
            string key,
            string value,
            bool readOnce,
            bool page,
            bool userArea)
        {
            if (value != null)
            {
                Rds.ExecuteNonQuery(
                    context: context,
                    statements: Rds.UpdateOrInsertSessions(
                        param: Rds.SessionsParam()
                            .SessionGuid(context.SessionGuid)
                            .Key(key)
                            .Page(page
                                ? context.Page ?? string.Empty
                                : string.Empty)
                            .Value(value)
                            .ReadOnce(readOnce)
                            .UserArea(userArea),
                        where: Rds.SessionsWhere()
                            .SessionGuid(context.SessionGuid)
                            .Key(key)
                            .Page(context.Page, _using: page)));
            }
            else
            {
                Remove(
                    context: context,
                    key: key,
                    page: page);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static void SetStartTime(Context context)
        {
            Set(
                context: context,
                key: "StartTime",
                value: DateTime.Now.ToString());
            SetLastAccessTime(context: context);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static void SetLastAccessTime(Context context)
        {
            Set(
                context: context,
                key: "LastAccessTime",
                value: DateTime.Now.ToString());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static void Set(Context context, SiteSettings ss, string key, View view)
        {
            Set(
                context: context,
                key: key,
                value: view.GetRecordingData(ss: ss).ToJson(),
                page: true);
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

        /// <summary>
        /// Fixed:
        /// </summary>
        public static void Remove(Context context, string key, bool page)
        {
            Rds.ExecuteNonQuery(
                context: context,
                statements: Rds.PhysicalDeleteSessions(
                    where: Rds.SessionsWhere()
                        .SessionGuid(context.SessionGuid)
                        .Key(key)
                        .Page(context.Page, _using: page)));
        }
        
        /// <summary>
        /// Fixed:
        /// </summary>
        private static void RemoveAll(Context context)
        {
            Rds.ExecuteNonQuery(
                context: context,
                statements: Rds.PhysicalDeleteSessions(
                    where: Rds.SessionsWhere()
                        .SessionGuid(context.SessionGuid)));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static void Abandon(Context context)
        {
            var current = System.Web.HttpContext.Current;
            current.Session.Clear();
            current.Session.Abandon();
            current.Response.Cookies.Add(
                new System.Web.HttpCookie("ASP.NET_SessionId", string.Empty));
            RemoveAll(context: context);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static void SetUserArea(Context context, string key, string value, bool page)
        {
            Set(
                context: context,
                key: $"User_{key}",
                value: value,
                page: page,
                userArea: true);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string GetUserArea(Context context, string key)
        {
            return context.SessionData.TryGetValue(
                $"User_{key}",
                out string value) ? value : null;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static void DeleteUserArea(Context context, string key, bool page)
        {
            Remove(
                context: context,
                key: $"User_{key}",
                page: page);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static System.Web.Mvc.ContentResult GetByApi(Context context)
        {
            var api = context.RequestDataString.Deserialize<SessionApi>();
            if (api == null || api.SessionKey.IsNullOrEmpty())
            {
                return ApiResults.Get(ApiResponses.BadRequest(context: context));
            }
            var value = GetUserArea(context, api.SessionKey);
            if (value == null)
            {
                return ApiResults.Get(ApiResponses.NotFound(context));
            }
            return ApiResults.Get(new
            {
                StatusCode = 200,
                Response = new
                {
                    context.UserId,
                    Key = api.SessionKey,
                    Value = value
                }
            }.ToJson());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static System.Web.Mvc.ContentResult SetByApi(Context context)
        {
            var api = context.RequestDataString.Deserialize<SessionApi>();
            if (api == null || api.SessionKey.IsNullOrEmpty() || api.SessionValue.IsNullOrEmpty())
            {
                return ApiResults.Get(ApiResponses.BadRequest(context: context));
            }
            try
            {
                SetUserArea(
                    context: context, 
                    key: api.SessionKey, 
                    value: api.SessionValue,
                    page: false);
            }
            catch
            {
                return ApiResults.Get(ApiResponses.Error(context, Error.Types.InternalServerError));
            }
            return ApiResults.Get(new
            {
                StatusCode = 200,
                Response = new
                {
                    context.UserId,
                    Key = api.SessionKey
                }
            }.ToJson());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static System.Web.Mvc.ContentResult DeleteByApi(Context context)
        {
            var api = context.RequestDataString.Deserialize<SessionApi>();
            if (api == null || api.SessionKey.IsNullOrEmpty())
            {
                return ApiResults.Get(ApiResponses.BadRequest(context: context));
            }
            if (GetUserArea(context, api.SessionKey) == null)
            {
                return ApiResults.Get(ApiResponses.NotFound(context));
            }
            try
            {
                DeleteUserArea(
                    context: context, 
                    key: api.SessionKey,
                    page: false);
            }
            catch
            {
                return ApiResults.Get(ApiResponses.Error(context, Error.Types.InternalServerError));
            }
            return ApiResults.Get(new
            {
                StatusCode = 200,
                Response = new
                {
                    context.UserId,
                    Key = api.SessionKey
                }
            }.ToJson());
        }
    }
}
