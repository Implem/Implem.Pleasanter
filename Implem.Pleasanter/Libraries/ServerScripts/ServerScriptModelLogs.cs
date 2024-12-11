using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System;

namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptModelLogs
    {
        private readonly Context Context;
        private readonly SiteSettings SiteSettings;

        public ServerScriptModelLogs(Context context, SiteSettings ss)
        {
            Context = context;
            SiteSettings = ss;
        }

        public bool LogInfo(
            object message,
            string method = "",
            bool console = true,
            bool sysLogs = true)
        {
            var ret = Log(
                message: message,
                sysLogType: SysLogModel.SysLogTypes.Info,
                method: method,
                console: console,
                sysLogs: sysLogs);
            return ret;
        }

        public bool LogWarning(
            object message,
            string method = "",
            bool console = true,
            bool sysLogs = true)
        {
            var ret = Log(
                message: message,
                sysLogType: SysLogModel.SysLogTypes.Warning,
                method: method,
                console: console,
                sysLogs: sysLogs);
            return ret;
        }

        public bool LogUserError(
            object message,
            string method = "",
            bool console = true,
            bool sysLogs = true)
        {
            var ret = Log(
                message: message,
                sysLogType: SysLogModel.SysLogTypes.UserError,
                method: method,
                console: console,
                sysLogs: sysLogs);
            return ret;
        }

        public bool LogSystemError(
            object message,
            string method = "",
            bool console = true,
            bool sysLogs = true)
        {
            var ret = Log(
                message: message,
                sysLogType: SysLogModel.SysLogTypes.SystemError,
                method: method,
                console: console,
                sysLogs: sysLogs);
            return ret;
        }

        public bool LogException(
            object message,
            string method = "",
            bool console = true,
            bool sysLogs = true)
        {
            var ret = Log(
                message: message,
                sysLogType: SysLogModel.SysLogTypes.Exception,
                method: method,
                console: console,
                sysLogs: sysLogs);
            return ret;
        }

        public bool Log(
            int type,
            object message,
            string method = "",
            bool console = true,
            bool sysLogs = true)
        {
            var ret = Log(
                message: message,
                sysLogType: Enum.IsDefined(typeof(SysLogModel.SysLogTypes), type)
                    ? (SysLogModel.SysLogTypes)type
                    : SysLogModel.SysLogTypes.Info,
                method: method,
                console: console,
                sysLogs: sysLogs);
            return ret;
        }

        private bool Log(
        SysLogModel.SysLogTypes sysLogType,
        object message,
        string method = "",
        bool console = true,
        bool sysLogs = true)
        {
            try
            {
                var strMessage = message?.ToString() ?? string.Empty;
                if (console)
                {
                    var methodBody = method.IsNullOrEmpty()
                        ? string.Empty
                        : $"[{method}]";
                    var body = $"({sysLogType}):{methodBody}{strMessage}";
                    Context.LogBuilder.AppendLine(body);
                }
                if (sysLogs)
                {
                    new SysLogModel(
                        context: Context,
                        method: method,
                        message: strMessage,
                        sysLogType: sysLogType);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}