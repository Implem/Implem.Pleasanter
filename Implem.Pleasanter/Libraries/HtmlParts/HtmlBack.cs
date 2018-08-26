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
            Context context,
            long siteId,
            long parentId,
            string referenceType,
            string siteReferenceType)
        {
            return !Request.IsAjax()
                ? hb.Hidden(
                    controlId: "BackUrl",
                    rawValue: BackUrl(
                        context: context,
                        siteId: siteId,
                        parentId: parentId,
                        referenceType: referenceType,
                        siteReferenceType: siteReferenceType))
                : hb;
        }

        private static string BackUrl(
            Context context,
            long siteId,
            long parentId,
            string referenceType,
            string siteReferenceType)
        {
            var referer = HttpUtility.UrlDecode(Url.UrlReferrer());
            switch (context.Controller)
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
                    switch (context.Action)
                    {
                        case "new":
                        case "edit":
                            return Strings.CoalesceEmpty(
                                referer?.EndsWith("/new") == false
                                    ? referer
                                    : null,
                                Locations.Get(context.Controller));
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
                            switch (context.Action)
                            {
                                case "new":
                                case "trashbox":
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
                            switch (context.Action)
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
                                case "trashbox":
                                    return Locations.ItemIndex(siteId);
                                default:
                                    return Locations.ItemIndex(parentId);
                            }
                    }
            }
        }
    }
}