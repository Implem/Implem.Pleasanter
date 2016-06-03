using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.DataTypes
{
    public class Comment
    {
        public int CommentId;
        public DateTime CreatedTime;
        public int Creator;
        public string Body;

        public HtmlBuilder Html(
            HtmlBuilder hb,
            string controlId = "",
            Action action = null)
        {
            return hb.Div(
                id: controlId,
                css: "comment",
                action: () =>
                {
                    if (action != null) action();
                    hb
                        .P(css: "time", action: () => hb
                            .Text(text: new Time(CreatedTime).ToViewText(
                                Displays.Get("YmdahmFormat"))))
                        .HtmlUser(Creator)
                        .P(css: "body markup", action: () => hb
                            .Text(text: Body));
                });
        }
    }

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

        public string ToControl(Column column)
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
                this?.Take(DisplayCount()).ForEach(comment =>
                    comment.Html(hb: hb)));
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
    }
}