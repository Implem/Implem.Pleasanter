﻿using Implem.DefinitionAccessor;
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
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
namespace Implem.Pleasanter.Models
{
    public static class SessionUtilities
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public static Dictionary<string, string> Get(Context context, bool includeUserArea = false, string sessionGuid = null)
        {
            if (Parameters.Session.UseKeyValueStore)
            {
                var key = sessionGuid ?? context.SessionGuid;
                StackExchange.Redis.IDatabase iDatabase = Implem.Pleasanter.Libraries.Redis.CacheForRedisConnection.Connection.GetDatabase();
                var sessions = iDatabase.HashGetAll(key)
                    .Where(dataRow =>
                        dataRow.Name.ToString().Split('_').Count() == 1 ||
                        (context.Page == null && sessionGuid == null) ||
                        dataRow.Name.ToString().Split('_')[1] == context.Page)
                    .ToDictionary(dataRow => dataRow.Name.ToString().Split('_')[0], dataRow => dataRow.Value.ToString());
                if (iDatabase.KeyExists(key + "_readOnce"))
                {
                    sessions = sessions.Concat(iDatabase.HashGetAll(key + "_readOnce").Where(dataRow =>
                            dataRow.Name.ToString().Split('_').Count() == 1 ||
                            (context.Page == null && sessionGuid == null) ||
                            dataRow.Name.ToString().Split('_')[1] == context.Page)
                        .ToDictionary(dataRow => dataRow.Name.ToString().Split('_')[0], dataRow => dataRow.Value.ToString()))
                    .ToDictionary(dataRow => dataRow.Key, dataRow => dataRow.Value);
                    Implem.Pleasanter.Libraries.Redis.CacheForRedisConnection.Remove(key + "_readOnce");
                }
                return sessions;
            }
            var where = Rds.SessionsWhere()
                            .SessionGuid(sessionGuid ?? context.SessionGuid)
                            .Add(raw: $"((\"UserArea\" is null) or (\"UserArea\" {context.Sqls.IsNotTrue}))", _using: !includeUserArea)
                            .Add(or: Rds.SessionsWhere()
                                .Page(context.Page, _using: context.Page != null)
                                .Page(raw: "''"));
            return Repository.ExecuteTable(
                context: context,
                statements: new SqlStatement[]
                {
                    Rds.SelectSessions(
                        column: Rds.SessionsColumn()
                            .Key()
                            .Value(),
                        where: Rds.SessionsWhere()
                            .SessionGuid(sessionGuid?? context.SessionGuid)
                            .Add(raw: $"((\"UserArea\" is null) or (\"UserArea\" {context.Sqls.IsNotTrue}))", _using: !includeUserArea)
                            .Add(or: Rds.SessionsWhere()
                                .Page(context.Page, _using: context.Page != null)
                                .Page(raw: "''"))),
                    Rds.PhysicalDeleteSessions(
                        where: Rds.SessionsWhere()
                            .SessionGuid(sessionGuid ?? context.SessionGuid)
                            .ReadOnce(true),
                        _using: context.ApiRequestBody == null)
                })
                    .AsEnumerable()
                    .ToDictionary(
                        dataRow => dataRow.String("Key"),
                        dataRow => dataRow.String("Value"));
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
            bool userArea = false,
            string sessionGuid = null)
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
                userArea: userArea,
                sessionGuid: sessionGuid);
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
            bool userArea,
            string sessionGuid = null)
        {
            if (value != null)
            {
                string pageName = page
                    ? context.Page ?? string.Empty
                    : string.Empty;
                sessionGuid = sessionGuid ?? context.SessionGuid;
                if (Parameters.Session.UseKeyValueStore && !userArea)
                {
                    sessionGuid += readOnce ? "_readOnce" : string.Empty;
                    StackExchange.Redis.IDatabase iDatabase = Implem.Pleasanter.Libraries.Redis.CacheForRedisConnection.Connection.GetDatabase();
                    string fieldName = pageName.IsNullOrEmpty() ? $"{key}" : $"{key}_{pageName}";
                    StackExchange.Redis.HashEntry[] hashEntrys = new StackExchange.Redis.HashEntry[]{ new StackExchange.Redis.HashEntry(fieldName, value) };
                    iDatabase.HashSet(
                        sessionGuid,
                        hashEntrys
                    );
                    iDatabase.KeyExpire(sessionGuid, TimeSpan.FromMinutes(Parameters.Session.RetentionPeriod));
                }
                else
                {
                    Repository.ExecuteNonQuery(
                    context: context,
                    statements: Rds.UpdateOrInsertSessions(
                        param: Rds.SessionsParam()
                            .SessionGuid(sessionGuid)
                            .Key(key)
                            .Page(pageName)
                            .Value(value)
                            .ReadOnce(readOnce)
                            .UserArea(userArea),
                        where: Rds.SessionsWhere()
                            .SessionGuid(sessionGuid)
                            .Key(key)
                            .Page(context.Page ?? string.Empty, _using: page)));
                }
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
        public static void Set(Context context, SiteSettings ss, string key, View view, string sessionGuid = null)
        {
            Set(
                context: context,
                key: key,
                value: view.GetRecordingData(
                    context: context,
                    ss: ss)
                        .ToJson(),
                page: true,
                sessionGuid: sessionGuid);
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
        public static void Remove(Context context, string key, bool page, string sessionGuid = null)
        {
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.PhysicalDeleteSessions(
                    where: Rds.SessionsWhere()
                        .SessionGuid(sessionGuid ?? context.SessionGuid)
                        .Key(key)
                        .Page(context.Page, _using: page)));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void RemoveAll(Context context, string sessionGuid = null)
        {
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.PhysicalDeleteSessions(
                    where: Rds.SessionsWhere()
                        .SessionGuid(sessionGuid ?? context.SessionGuid)));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static void Abandon(Context context)
        {
            context.SessionAbandon();
            RemoveAll(context: context);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static void SetUserArea(Context context, string key, string value, bool page, string sessionGuid = null)
        {
            Set(
                context: context,
                key: $"User_{key}",
                value: value,
                page: page,
                userArea: true,
                sessionGuid: sessionGuid);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string GetUserArea(Context context, string key, bool useUserSessionData = false)
        {
            var sessionData = useUserSessionData ? context.UserSessionData : context.SessionData;
            return sessionData.TryGetValue(
                $"User_{key}",
                out string value) ? value : null;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static void DeleteUserArea(Context context, string key, bool page, string sessionGuid = null)
        {
            Remove(
                context: context,
                key: $"User_{key}",
                page: page,
                sessionGuid: sessionGuid);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ContentResultInheritance GetByApi(Context context)
        {
            if (!Mime.ValidateOnApi(contentType: context.ContentType))
            {
                return ApiResults.BadRequest(context: context);
            }
            var api = context.RequestDataString.Deserialize<SessionApi>();
            if (api == null || api.SessionKey.IsNullOrEmpty())
            {
                return ApiResults.Get(ApiResponses.BadRequest(context: context));
            }
            var value = GetUserArea(
                context: context,
                key: api.SessionKey,
                useUserSessionData: api.SavePerUser);
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
        public static ContentResultInheritance SetByApi(Context context)
        {
            if (!Mime.ValidateOnApi(contentType: context.ContentType))
            {
                return ApiResults.BadRequest(context: context);
            }
            var api = context.RequestDataString.Deserialize<SessionApi>();
            if (api == null || api.SessionKey.IsNullOrEmpty() || api.SessionValue.IsNullOrEmpty())
            {
                return ApiResults.Get(ApiResponses.BadRequest(context: context));
            }
            var sessionGuid = api.SavePerUser ? "@" + context.UserId : context.SessionGuid;
            try
            {
                SetUserArea(
                    context: context,
                    key: api.SessionKey,
                    value: api.SessionValue,
                    page: false,
                    sessionGuid: sessionGuid);
            }
            catch
            {
                return ApiResults.Get(ApiResponses.Error(context, new ErrorData(Error.Types.InternalServerError)));
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
        public static ContentResultInheritance DeleteByApi(Context context)
        {
            if (!Mime.ValidateOnApi(contentType: context.ContentType))
            {
                return ApiResults.BadRequest(context: context);
            }
            var api = context.RequestDataString.Deserialize<SessionApi>();
            if (api == null || api.SessionKey.IsNullOrEmpty())
            {
                return ApiResults.Get(ApiResponses.BadRequest(context: context));
            }
            if (GetUserArea(context, api.SessionKey, api.SavePerUser) == null)
            {
                return ApiResults.Get(ApiResponses.NotFound(context));
            }
            try
            {
                DeleteUserArea(
                    context: context,
                    key: api.SessionKey,
                    page: false,
                    sessionGuid: api.SavePerUser ? "@" + context.UserId : context.SessionGuid);
            }
            catch
            {
                return ApiResults.Get(ApiResponses.Error(context, new ErrorData(Error.Types.InternalServerError)));
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
        public static void DeleteOldSessions(Context context)
        {
            var before = SiteInfo.SessionCleanedUpDate.ToLocal(context: context).ToString("yyyy/MM/dd");
            var now = DateTime.Now.ToLocal(context: context).ToString("yyyy/MM/dd");
            if (before != now)
            {
                SiteInfo.SessionCleanedUpDate = DateTime.Now;
                Rds.ExecuteNonQuery(
                    context: context,
                    statements: Rds.PhysicalDeleteSessions(
                        where: Rds.SessionsWhere()
                            .UpdatedTime(
                                DateTime.Now.AddMinutes(Parameters.Session.RetentionPeriod * -1),
                                _operator: "<")
                            .Add(raw: "( \"SessionGuid\" not like '@%' )")));
            }
        }
    }
}
