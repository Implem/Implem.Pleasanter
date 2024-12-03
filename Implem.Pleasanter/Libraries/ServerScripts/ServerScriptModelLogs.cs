using Implem.Libraries.Utilities;
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
            string message,
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
            string message,
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
            string message,
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
            string message,
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
            string message,
            string method = "",
            bool console = true,
            bool sysLogs = true)
        {
            var ret = Log(
                message: message,
                sysLogType: SysLogModel.SysLogTypes.Execption,
                method: method,
                console: console,
                sysLogs: sysLogs);
            return ret;
        }

        public bool Log(
            int type,
            string message,
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
        string message,
        string method = "",
        bool console = true,
        bool sysLogs = true)
        {
            try
            {
                if (console)
                {
                    var methodBody = method.IsNullOrEmpty()
                        ? string.Empty
                        : $"[{method}]";
                    var body = $"({sysLogType}):{methodBody}{message}";
                    Context.LogBuilder.Append(body);
                }
                if (sysLogs)
                {
                    new SysLogModel(
                        context: Context,
                        method: method,
                        message: message,
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