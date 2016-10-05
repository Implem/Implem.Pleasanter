using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using System.Web;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlBack
    {
        public static HtmlBuilder BackUrl(
            this HtmlBuilder hb,
            long siteId,
            long parentId,
            string referenceType,
            string siteReferenceType)
        {
            return !Request.IsAjax()
                ? hb.Hidden(
                    controlId: "BackUrl",
                    rawValue: BackUrl(siteId, parentId, referenceType, siteReferenceType))
                : hb;
        }

        private static string BackUrl(
            long siteId, long parentId, string referenceType, string siteReferenceType)
        {
            var controller = Routes.Controller();
            var referer = HttpUtility.UrlDecode(new Request(HttpContext.Current).UrlReferrer());
            switch (controller)
            {
                case "admins":
                    return Navigations.Top();
                case "depts":
                case "users":
                    switch (Routes.Action())
                    {
                        case "new":
                        case "edit":
                            return Strings.CoalesceEmpty(
                                referer, Navigations.Get(controller));
                        default:
                            return Navigations.Get("Admins");
                    }
                default:
                    switch (referenceType)
                    {
                        case "Sites":
                            switch (Routes.Action())
                            {
                                case "new":
                                    return Navigations.ItemIndex(siteId);
                                case "edit":
                                    switch (siteReferenceType)
                                    {
                                        case "Wikis":
                                            return Navigations.ItemIndex(parentId);
                                        default:
                                            return Navigations.ItemIndex(siteId);
                                    }
                                default:
                                    return Navigations.ItemIndex(parentId);
                            }
                        default:
                            switch (Routes.Action())
                            {
                                case "new":
                                case "edit":
                                    return Strings.CoalesceEmpty(
                                        referer, Navigations.ItemIndex(siteId));
                                default:
                                    return Navigations.ItemIndex(parentId);
                            }
                    }
            }
        }
    }
}