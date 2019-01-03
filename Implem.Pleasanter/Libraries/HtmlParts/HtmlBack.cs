using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
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
            return !context.Ajax
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
            var referer = HttpUtility.UrlDecode(context.UrlReferrer);
            switch (context.Controller)
            {
                case "admins":
                    return Locations.Top(context: context);
                case "versions":
                    return referer != null
                        ? referer
                        : Locations.Top(context: context);
                case "tenants":
                    return AdminsOrTop(context: context);
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
                                Locations.Get(
                                    context: context,
                                    parts: context.Controller));
                        case "editapi":
                            return referer != null
                                ? referer
                                : Locations.Top(context: context);
                        default:
                            return AdminsOrTop(context: context);
                    }
                default:
                    switch (referenceType)
                    {
                        case "Sites":
                            switch (context.Action)
                            {
                                case "new":
                                case "trashbox":
                                    return Locations.ItemIndex(
                                        context: context,
                                        id: siteId);
                                case "edit":
                                    switch (siteReferenceType)
                                    {
                                        case "Wikis":
                                            return Locations.ItemIndex(
                                                context: context,
                                                id: parentId);
                                        default:
                                            return Locations.ItemIndex(
                                                context: context,
                                                id: siteId);
                                    }
                                default:
                                    return Locations.ItemIndex(
                                        context: context,
                                        id: parentId);
                            }
                        case "Wikis":
                            return context.QueryStrings.Int("back") == 1
                                && !referer.IsNullOrEmpty()
                                    ? referer
                                    : Locations.ItemIndex(
                                        context: context,
                                        id: parentId);
                        default:
                            switch (context.Action)
                            {
                                case "new":
                                case "edit":
                                    return context.QueryStrings.Int("back") == 1
                                        && !referer.IsNullOrEmpty()
                                            ? referer
                                            : Locations.Get(
                                                context: context,
                                                parts: new string[]
                                                {
                                                    context.Publish
                                                        ? "Publishes"
                                                        : "Items",
                                                    siteId.ToString(),
                                                    Requests.ViewModes.GetSessionData(
                                                        context: context,
                                                        siteId: siteId)
                                                });
                                case "trashbox":
                                    return Locations.ItemIndex(
                                        context: context,
                                        id: siteId);
                                default:
                                    return Locations.ItemIndex(
                                        context: context,
                                        id: parentId);
                            }
                    }
            }
        }

        private static string AdminsOrTop(Context context)
        {
            return Permissions.CanManageTenant(context: context)
                ? Locations.Admins(context: context)
                : Locations.Top(context: context);
        }
    }
}