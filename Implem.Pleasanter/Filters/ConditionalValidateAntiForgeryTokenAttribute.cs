using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class ConditionalValidateAntiForgeryTokenAttribute : Attribute, IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext filterContext)
    {
        if (filterContext == null)
        {
            throw new ArgumentNullException(nameof(filterContext));
        }

        // GET, HEAD, OPTIONS, TRACEのような安全なメソッドの場合は検証をスキップ
        var method = filterContext.HttpContext.Request.Method;
        if (HttpMethods.IsGet(method) ||
            HttpMethods.IsHead(method) ||
            HttpMethods.IsOptions(method) ||
            HttpMethods.IsTrace(method))
        {
            return;
        }

        // 実行されるアクションに [IgnoreAntiforgeryToken] が付与されている場合は検証をスキップ
        var endpoint = filterContext.HttpContext.GetEndpoint();
        if (endpoint?.Metadata.GetMetadata<IAntiforgeryPolicy>() is IAntiforgeryPolicy policy && policy is IgnoreAntiforgeryTokenAttribute)
        {
            return;
        }

        var context = new Context(
                                        sessionStatus: false,
                                        sessionData: false,
                                        item: false,
                                        setPermissions: false);

        if (!Parameters.Security.TokenCheck && !context.IsForm)
        {
            return;
        }

        try
        {
            // DIコンテナからIAntiforgeryサービスを取得
            var antiforgery = filterContext.HttpContext.RequestServices.GetRequiredService<IAntiforgery>();

            //標準の検証ロジックを実行
            await antiforgery.ValidateRequestAsync(filterContext.HttpContext);
        }
        catch (AntiforgeryValidationException)
        {
            // 検証に失敗した場合の処理
            // CheckContextAttributes.cs の Parameters.Security.TokenCheck による処理と同内容

            // レスポンスのステータスコードを設定
            filterContext.HttpContext.Response.StatusCode = 400;

            // クライアントのJavaScriptはjQueryの$.ajaxを使用しているため、このヘッダーの有無でAJAXリクエストを判定できる。
            var isAjaxRequest = filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (isAjaxRequest)
            {
                // AJAXリクエストの場合、JsonResultを返す
                filterContext.Result = new JsonResult(new
                {
                    // Displays.InvalidRequestを使用して既存のメッセージを再利用
                    Message = Displays.InvalidRequest(context: context)
                });
            }
            else
            {
                // 通常リクエストの場合、ContentResultを返す
                filterContext.Result = new ContentResult()
                {
                    Content = Displays.InvalidRequest(context: context)
                };
            }
        }
    }
}