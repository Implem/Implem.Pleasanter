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
            var referer = HttpUtility.UrlDecode(Url.UrlReferrer());
            switch (controller)
            {
                case "admins":
                    return Locations.Top();
                case "versions":
                    return referer != null
                        ? referer
                        : Locations.Top();
                case "depts":
                case "groups":
                case "users":
                    switch (Routes.Action())
                    {
                        case "new":
                        case "edit":
                            return Strings.CoalesceEmpty(
                                referer?.EndsWith("/new") == false
                                    ? referer
                                    : null,
                                Locations.Get(controller));
                        case "editapi":
                            return referer != null
                                ? referer
                                : Locations.Top();
                        default:
                            return Locations.Get("Admins");
                    }
                default:
                    switch (referenceType)
                    {
                        case "Sites":
                            switch (Routes.Action())
                            {
                                case "new":
                                    return Locations.ItemIndex(siteId);
                                case "edit":
                                    switch (siteReferenceType)
                                    {
                                        case "Wikis":
                                            return Locations.ItemIndex(parentId);
                                        default:
                                            return Locations.ItemIndex(siteId);
                                    }
                                default:
                                    return Locations.ItemIndex(parentId);
                            }
                        case "Wikis":
                            return QueryStrings.Int("back") == 1 && !referer.IsNullOrEmpty()
                                ? referer
                                : Locations.ItemIndex(parentId);
                        default:
                            switch (Routes.Action())
                            {
                                case "new":
                                case "edit":
                                    return
                                        QueryStrings.Int("back") == 1 &&
                                        !referer.IsNullOrEmpty()
                                            ? referer
                                            : Locations.Get(
                                                "Items",
                                                siteId.ToString(),
                                                Requests.ViewModes.GetBySession(siteId));
                                default:
                                    return Locations.ItemIndex(parentId);
                            }
                    }
            }
        }
    }
}