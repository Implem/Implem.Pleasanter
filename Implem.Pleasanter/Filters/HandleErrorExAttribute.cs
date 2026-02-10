using Implem.DefinitionAccessor;
using Implem.Libraries.Exceptions;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.ClearScript;
using System;
using System.Linq;
using System.Text.RegularExpressions;
namespace Implem.PleasanterFilters
{
    public class HandleErrorExAttribute : ActionFilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            var context = new Context(sessionData: false);
            var ss = SiteSettingsUtilities.Get(
                context: context,
                siteId: context.SiteId);
            ArgumentNullException.ThrowIfNull(filterContext);
            try
            {
                _ = new SysLogModel(
                    context: context,
                    method: nameof(OnException),
                    message: $"{filterContext.Exception.GetType().Name}: {filterContext.Exception.Message}",
                    errStackTrace: Parameters.SysLog.OutputErrorDetails
                        ? filterContext.Exception.ToString()
                        : filterContext.Exception.StackTrace,
                    sysLogType: SysLogModel.SysLogTypes.Exception);
                var siteId = CanManageSiteId(context: context);
                SessionUtilities.Set(
                    context: context,
                    key: "ExceptionSiteId",
                    value: siteId.ToString());
            }
            catch
            {
            }
            filterContext.ExceptionHandled = true;
            if (context.Ajax)
            {
                if (filterContext.Exception is ScriptEngineException se)
                {
                    var message = se.Message;
                    var (line, column, code) = TryParseLocationFromErrorDetails(se.ErrorDetails);
                    var errorDetail = line.IsNullOrWhiteSpace() || column.IsNullOrWhiteSpace() || code.IsNullOrWhiteSpace()
                        ? string.Empty
                        : $"\n{Displays.ServerScriptErrorPosition(context: context, line, column, code)}";
                    var responseCollection = new ResponseCollection();
                    if (ss.ServerScriptsGetErrorDetails.HasValue && ss.ServerScriptsGetErrorDetails.Value)
                    {
                        responseCollection.Log(Displays.ServerScriptErrorDetail(context, message, errorDetail));
                    }
                    responseCollection.Message(Messages.ServerScriptExecutionFailed(context));
                    filterContext.Result = new ContentResult()
                    {
                        Content = responseCollection.ToJson(),
                        StatusCode = 422
                    };
                }
                else if (filterContext.Exception is FormulaErrorException fe)
                {
                    var message = fe.Message;
                    var responseCollection = new ResponseCollection();
                    if (ss.FormulasGetErrorDetails.HasValue && ss.FormulasGetErrorDetails.Value)
                    {
                        responseCollection.Log(Displays.ServerScriptErrorDetail(context, message, string.Empty));
                    }
                    responseCollection.Message(Messages.FormulaExecutionFailed(context));
                    filterContext.Result = new ContentResult()
                    {
                        Content = responseCollection.ToJson(),
                        StatusCode = 422
                    };
                }
                else
                {
                    filterContext.Result = new ContentResult()
                    {
                        Content = Error.Types.ApplicationError.MessageJson(context: context),
                        StatusCode = 500
                    };
                }
            }
            else
            {
                if (filterContext.Exception is ScriptEngineException)
                {
                     filterContext.Result = new RedirectResult(Locations.ServerScriptError(context: context));
                }
                else if (filterContext.Exception is FormulaErrorException)
                {
                    filterContext.Result = new RedirectResult(Locations.FormulaError(context: context));
                }
                else
                {
                    filterContext.Result = new RedirectResult(Locations.ApplicationError(context: context));
                }
            }
        }

        private static long CanManageSiteId(Context context)
        {
            if (context.SiteId == 0)
            {
                return 0;
            }
            else
            {
                var ss = SiteSettingsUtilities.Get(
                    context: context,
                    siteId: context.SiteId);
                var siteId = context.CanManageSite(
                    ss: ss,
                    site: true)
                        ? context.SiteId
                        : 0;
                return siteId;
            }
        }

        private static readonly Regex ScriptLocationRegex = new(
            pattern: @"at\s+Script\s*\[(?<scriptId>[^\]]+)\]\s*:\s*(?<line>\d+)\s*:\s*(?<col>\d+)\s*(?:->\s*(?<code>.*))?$",
            options: RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private static (string line, string col, string code) TryParseLocationFromErrorDetails(string errorDetails)
        {
            if (errorDetails.IsNullOrWhiteSpace())
            {
                return (null, null, null);
            }
            var lines = errorDetails
                .Split(["\r\n", "\n"], StringSplitOptions.None)
                .Select(l => (l ?? string.Empty).Trim())
                .Where(l => !string.IsNullOrEmpty(l));
            foreach (var line in lines)
            {
                var m = ScriptLocationRegex.Match(line);
                if (!m.Success) continue;
                return (
                    line: m.Groups["line"]?.Success == true ? m.Groups["line"].Value : null,
                    col: m.Groups["col"]?.Success == true ? m.Groups["col"].Value : null,
                    code: m.Groups["code"]?.Success == true ? m.Groups["code"].Value : null
                );
            }
            return (null, null, null);
        }
    }
}
