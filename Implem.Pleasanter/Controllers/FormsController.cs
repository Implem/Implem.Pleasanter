using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Search;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.Web;
using Implem.Pleasanter.Models;
using Implem.PleasanterFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Antiforgery;
using System.Web.Helpers;
using System.Text.Json;
using System.Threading.Tasks;
using Implem.DefinitionAccessor;
using System.Reflection;
using Implem.Pleasanter.Libraries.Security.Captcha;
namespace Implem.Pleasanter.Controllers
{
    [AllowAnonymous]
    [FormsAttributes]
    [ConditionalValidateAntiForgeryToken]
    public class FormsController : Controller
    {
        private const string FormCreateSuccess = "FormCreateSuccess";
        private readonly IAntiforgery _antiforgery;
        private readonly ICaptchaVerificationService _captchaVerificationService;

        public FormsController(IAntiforgery antiforgery, ICaptchaVerificationService captchaVerificationService)
        {
            _antiforgery = antiforgery;
            _captchaVerificationService = captchaVerificationService;
        }

        [AcceptVerbs(HttpVerbs.Get, HttpVerbs.Post)]
        public ActionResult New(string guid)
        {
            var context = new Context();
            var id = context.Id; // context.IsFormの場合には new Context()中で SiteId を取得する
            var log = new SysLogModel(context: context);
            var model = new ItemModel(context: context, referenceId: id);
            var ss = model.GetSite(context: context).SiteSettings;
            if (!IsCurrentDateInPeriod(context, ss.FormStartDateTime?.ToLocal(context), ss.FormEndDateTime?.ToLocal(context)))
            {
                var htmlUnavailable = model.FormUnavailable(context: context);
                ViewBag.HtmlBody = htmlUnavailable;
                log.Finish(context: context, responseSize: htmlUnavailable.Length);
                return context.RedirectData.Url.IsNullOrEmpty()
                    ? View()
                        : (ActionResult)Redirect(context.RedirectData.Url);
            }

            context.Tokens = _antiforgery.GetAndStoreTokens(HttpContext);

            var html = model.New(context: context);
            ViewBag.HtmlBody = html;
            log.Finish(context: context, responseSize: html.Length);
            return context.RedirectData.Url.IsNullOrEmpty()
                ? View()
                    : (ActionResult)Redirect(context.RedirectData.Url);
        }


        private bool IsCurrentDateInPeriod(Context context, DateTime? localStartDateTime, DateTime? localEndDateTime)
        {
            var currentDateTime = DateTime.Now;

            if (localStartDateTime.HasValue && currentDateTime <= localStartDateTime.Value)
            {
                return false;
            }

            if (localEndDateTime.HasValue && currentDateTime > localEndDateTime.Value)
            {
                return false;
            }

            return true;
        }


        [HttpPost]
        public async Task<string> Create(string guid)
        {
            var context = new Context();
            var id = context.Id; // context.IsFormの場合には new Context()中で SiteId を取得する
            var log = new SysLogModel(context: context);

            var captchaResult = await _captchaVerificationService.VerifyAsync(context);
            if (!captchaResult.Success)
            {
                var errorJson = captchaResult.ErrorType.MessageJson(context: context);
                var errorCodes = string.Join(",", captchaResult.ErrorCodes);
                log.Finish(context: context, responseSize: errorJson.Length, message: $"CAPTCHA verification failed: {errorCodes}");
                return errorJson;
            }

            var model = new ItemModel(context: context, referenceId: id);
            var json = model.Create(context: context);
            bool isSuccess = Regex.IsMatch(json, @"""Method""\s*:\s*""Href""\s*,\s*""Value""\s*:\s*""/forms/[a-fA-F0-9]{32}/thanks""");
            if (isSuccess)
            {
                TempData[FormCreateSuccess] = true;
            }
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [AcceptVerbs(HttpVerbs.Get, HttpVerbs.Post)]
        public ActionResult Thanks(string guid)
        {
            if (TempData[FormCreateSuccess] is not true)
            {
                return RedirectToRoute("Binaries", new { controller = "forms", action = "new", guid = guid });
            }
            var context = new Context();
            var id = int.TryParse(guid, out int parsed) ? parsed : context.Id;
            var log = new SysLogModel(context: context);
            var model = new ItemModel(context: context, referenceId: id);
            var html = model.FormThanks(context: context);
            ViewBag.HtmlBody = html;
            log.Finish(context: context, responseSize: html.Length);
            return context.RedirectData.Url.IsNullOrEmpty()
                ? View()
                    : (ActionResult)Redirect(context.RedirectData.Url);
        }

    }
}