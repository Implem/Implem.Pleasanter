using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Converts;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.DataTypes
{
    public class Comments : List<Comment>, IConvertable
    {
        public Comments()
        {
        }

        public Comments(Comments source)
        {
            this.AddRange(new List<Comment>(source));
        }

        public Comments Prepend(string body)
        {
            if (body.Trim() != string.Empty)
            {
                this.Insert(0, new Comment
                {
                    CommentId = CommentId(),
                    CreatedTime = DateTime.Now,
                    Creator = Sessions.UserId(),
                    Body = body
                });
            }
            return this;
        }

        public new string ToString()
        {
            return string.Empty;
        }

        public string ToJson()
        {
            return Jsons.ToJson(new Comments(this));
        }

        private int CommentId()
        {
            if (this.Count == 0)
            {
                return 1;
            }
            else
            {
                return this.Select(o => o.CommentId).Max() + 1;
            }
        }

        public string ToControl(Column column, Permissions.Types pt)
        {
            return string.Empty;
        }

        public string ToResponse()
        {
            return string.Empty;
        }

        public HtmlBuilder Td(HtmlBuilder hb, Column column)
        {
            return hb.Td(action: () =>
                this?.Take(DisplayCount()).ForEach(comment => comment.Html(hb: hb)));
        }

        public string GridText(Column column)
        {
            return this?.Take(DisplayCount()).Select(comment =>
                "{0} {1}  \n{2}".Params(
                    comment.CreatedTimeDisplayValue(),
                    SiteInfo.UserFullName(comment.Creator),
                    comment.Body))
                        .Join("\n\n");
        }

        private int DisplayCount()
        {
            switch (Routes.Action())
            {
                case "histories": return 1;
                default: return 3;
            }
        }

        public string ToExport(Column column)
        {
            return this.Select(o =>
                new Time(o.CreatedTime).ToViewText() + " " +
                SiteInfo.UserFullName(o.Creator) + "\n" +
                o.Body).Join("\n\n");
        }

        public string ToNotice(
            string saved,
            Column column,
            bool updated,
            bool update)
        {
            switch (Routes.Action())
            {
                case "deletecomment":
                    return Displays.CommentDeleted();
                default:
                    return this.Any()
                        ? this.FirstOrDefault().Body.ToNoticeLine(
                            string.Empty,
                            column,
                            updated,
                            update)
                        : string.Empty;
            }
        }
    }
}