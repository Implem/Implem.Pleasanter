using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using NLog;
using NLog.Config;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Implem.Pleasanter.Controllers
{
    [AllowAnonymous]
    public class CspReportController : Controller
    {
        private static readonly Logger Logger = InitializeLogger();
        private readonly bool IsDevelopment;

        public CspReportController(IWebHostEnvironment env)
        {
            IsDevelopment = env.IsDevelopment();
        }

        [HttpPost]
        public async Task<IActionResult> Report()
        {
            var logMessage = await CreateLogMessageAsync();
            if (!logMessage.IsNullOrEmpty())
            {
                Logger.Info(logMessage);
            }
            return NoContent();
        }

        private static Logger InitializeLogger()
        {
            var nullLogger = LogManager.CreateNullLogger();
            try
            {
                var config = new XmlLoggingConfiguration("NLog.CspReport.config");
                try
                {
                    LogManager.Configuration = config;
                }
                catch (NLogConfigurationException ex)
                when (ex.InnerException?.Source == "Azure.Data.Tables")
                {
                    config.RemoveTarget("azureDataTable");
                }
                const string key = "targetName";
                if (!config.Variables.ContainsKey(key))
                {
                    return nullLogger;
                }
                var targetName = config.Variables[key].Render(LogEventInfo.CreateNullEvent());
                var target = config.FindTargetByName(targetName);
                if (target == null)
                {
                    return nullLogger;
                }
                LogManager.Configuration = config;
                return LogManager.GetCurrentClassLogger();
            }
            catch (Exception e)
            {
                var context = new Context();
                new SysLogModel(
                    context: context,
                    e: e);
                return nullLogger;
            }
        }

        private async Task<string> CreateLogMessageAsync()
        {
            const string notAvailable = "N/A";
            var contentType = Request.ContentType ?? notAvailable;
            if (!IsContentType(contentType))
            {
                return string.Empty;
            }
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();
            try
            {
                var jObject = JObject.Parse(body);
                if (IsDevelopmentPolicyViolation(jObject))
                {
                    return string.Empty;
                }
            }
            catch
            {
                return string.Empty;
            }
            var remoteIpAddress = HttpContext.Connection.RemoteIpAddress;
            var ipAddress = remoteIpAddress?.ToString() ?? notAvailable;
            var userAgent = Request.Headers["User-Agent"].ToString() ?? notAvailable;
            return $"IP: {ipAddress} | ContentType: {contentType} | UserAgent: {userAgent} | body: {body}";
        }

        private bool IsContentType(string contentType)
        {
            var lowerContentType = contentType.ToLowerInvariant();
            return lowerContentType == "application/json"
                || lowerContentType == "application/csp-report"
                || lowerContentType == "application/reports+json";
        }

        private bool IsDevelopmentPolicyViolation(JObject jObject)
        {
            if (!IsDevelopment)
            {
                return false;
            }
            var blockedUri = jObject["csp-report"]?["blocked-uri"]?.ToString() ?? string.Empty;
            return blockedUri.EndsWith("/_vs/browserLink")
                || blockedUri.EndsWith("/_framework/aspnetcore-browser-refresh.js");
        }
    }
}