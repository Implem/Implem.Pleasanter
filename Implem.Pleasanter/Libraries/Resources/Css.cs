﻿using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using System.Web.Mvc;
namespace Implem.Pleasanter.Libraries.Resources
{
    public static class Css
    {
        public static string Class(string _default, string additional)
        {
            if (additional.IsNullOrEmpty())
            {
                return _default?.Trim();
            }
            else if (_default?.EndsWith(" ") == true)
            {
                return (_default + additional).Trim();
            }
            else if (additional.Substring(0, 1) == " ")
            {
                return (_default?.Trim() + additional).Trim();
            }
            else
            {
                return additional;
            }
        }

        public static ContentResult Get(Context context)
        {
            var siteId = context.QueryStrings.Long("site-id");
            var id = context.QueryStrings.Long("id");
            var controller = context.QueryStrings.Data("controller");
            var action = context.QueryStrings.Data("action");
            var siteTop = siteId == 0 && id == 0 && controller == "items" && action == "index";
            return new ContentResult
            {
                ContentType = "text/css",
                Content = HtmlStyles.ExtendedStyles(
                    context: context,
                    deptId: context.DeptId,
                    groups: context.Groups,
                    userId: context.UserId,
                    siteTop: siteTop,
                    siteId: siteId,
                    id: id,
                    controller: controller,
                    action: action)
            };
        }
    }
}