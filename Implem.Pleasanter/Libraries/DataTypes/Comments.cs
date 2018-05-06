using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Extensions;
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
    public class Comments : List<Comment>, IConvertable
    {
        public Comments()
        {
        }

        public Comments(Comments source)
        {
            AddRange(new List<Comment>(source));
        }

        public Comments Prepend(string body)
        {
            if (body.Trim() != string.Empty)
            {
                Insert(0, new Comment
                {
                    CommentId = CommentId(),
                    CreatedTime = DateTime.Now,
                    Creator = Sessions.UserId(),
                    Body = body,
                    Created = true
                });
            }
            return this;
        }

        public void Update(int commentId, string body)
        {
            this.FirstOrDefault(o => o.CommentId == commentId)?.Update(body);
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
            if (Count == 0)
            {
                return 1;
            }
            else
            {
                return this.Select(o => o.CommentId).Max() + 1;
            }
        }

        public Comments ToLocal()
        {
            var comments = new Comments();
            ForEach(o => comments.Add(o.ToLocal()));
            return comments;
        }

        public string ToControl(SiteSettings ss, Column column)
        {
            return string.Empty;
        }

        public string ToResponse()
        {
            return string.Empty;
        }

        public HtmlBuilder Td(HtmlBuilder hb, Column column)
        {
            var css = GridCss();
            return hb.Td(action: () => this?
                .Take(DisplayCount())
                .ForEach(comment => comment
                    .Html(
                        hb: hb,
                        allowEditing: column.SiteSettings.AllowEditingComments == true,
                        allowImage: column.AllowImage == true,
                        mobile: column.SiteSettings.Mobile,
                        css: css,
                        readOnly: true)));
        }

        public string GridText(Column column)
        {
            return this?.Take(DisplayCount()).Select(comment =>
                "{0} {1}  \n{2}".Params(
                    comment.CreatedTimeDisplayValue(),
                    SiteInfo.UserName(comment.Creator),
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

        private string GridCss()
        {
            if (DisplayCount() == 3)
            {
                switch (this.Count())
                {
                    case 1: return " one-third";
                    case 2: return " half";
                }
            }
            return null;
        }

        public string ToExport(Column column, ExportColumn exportColumn = null)
        {
            return this.Select(o =>
                o.CreatedTime.ToLocal().ToViewText() + " " +
                SiteInfo.UserName(o.Creator) + "\n" +
                o.Body).Join("\n\n");
        }

        public string ToNotice(string saved, Column column, bool updated, bool update)
        {
            var body = string.Empty;
            if (Routes.Action() == "deletecomment")
            {
                body = Displays.CommentDeleted() + "\n";
            }
            if (this.Any())
            {
                body += this.FirstOrDefault(o => o.Created)?
                    .Body
                        .ToNoticeLine(
                            string.Empty,
                            column,
                            updated,
                            update);
                this.Where(o => o.Updated).ForEach(comment =>
                    body += comment.Body
                        .ToNoticeLine(
                            string.Empty,
                            column,
                            updated,
                            update,
                            Displays.CommentUpdated()));
                return body;
            }
            else
            {
                return string.Empty;
            }
        }

        public bool InitialValue()
        {
            return this?.Any() != true;
        }
    }
}