using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.WebPages;
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
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

        public string ToControl(Context context, SiteSettings ss, Column column)
        {
            return string.Empty;
        }

        public string ToResponse(Context context, SiteSettings ss, Column column)
        {
            return string.Empty;
        }

        public string ToDisplay(Context context, SiteSettings ss, Column column)
        {
            return ToDisplay(context: context);
        }

        private string ToDisplay(Context context)
        {
            return this.Select(o =>
                o.CreatedTime.ToLocal(context: context).ToViewText(context: context) + " " +
                SiteInfo.UserName(
                    context: context,
                    userId: o.Creator) + "\n" +
                o.Body).Join("\n\n");
        }

        public string ToLookup(Context context, SiteSettings ss, Column column, Lookup.Types? type)
        {
            switch (type)
            {
                case Lookup.Types.DisplayName:
                    return ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                default:
                    return string.Empty;
            }
        }

        public HtmlBuilder Td(
            HtmlBuilder hb,
            Context context,
            Column column,
            int? tabIndex,
            ServerScriptModelColumn serverScriptModelColumn)
        {
            var css = GridCss(context: context);
            return hb.Td(
                css: column.CellCss(serverScriptModelColumn?.ExtendedCellCss),
                action: () => this?
                    .Take(DisplayCount(context: context))
                    .ForEach(comment => comment
                        .Html(
                            hb: hb,
                            context: context,
                            ss: column.SiteSettings,
                            allowEditing: column.SiteSettings.AllowEditingComments == true,
                            allowImage: column.AllowImage == true,
                            mobile: context.Mobile,
                            css: css,
                            readOnly: true)));
        }

        public string GridText(Context context, Column column)
        {
            return this?.Take(DisplayCount(context: context)).Select(comment =>
                "{0} {1}  \n{2}".Params(
                    comment.CreatedTimeDisplayValue(context: context),
                    SiteInfo.UserName(
                        context: context,
                        userId: comment.Creator),
                    comment.Body))
                        .Join("\n\n");
        }

        public object ToApiDisplayValue(Context context, SiteSettings ss, Column column)
        {
            return this.Any()
                ? ToJson()
                : null;
        }

        public object ToApiValue(Context context, SiteSettings ss, Column column)
        {
            return this.Any()
                ? ToJson()
                : null;
        }

        public string ToExport(Context context, Column column, ExportColumn exportColumn = null)
        {
            return ToDisplay(context: context);
        }

        public string ToNotice(
            Context context,
            string saved,
            Column column,
            NotificationColumnFormat notificationColumnFormat,
            bool updated,
            bool update)
        {
            var body = string.Empty;
            if (context.Action == "deletecomment")
            {
                body = Displays.CommentDeleted(context: context) + "\n";
            }
            if (this.Any())
            {
                var created = this.FirstOrDefault(o => o.Created)?.Body;
                if (created != null)
                {
                    body += notificationColumnFormat.DisplayText(
                        self: created,
                        saved: string.Empty,
                        column: column,
                        updated: updated,
                        update: update);
                }
                this.Where(o => o.Updated).ForEach(comment =>
                    body += notificationColumnFormat.DisplayText(
                        self: comment.Body,
                        saved: string.Empty,
                        column: column,
                        updated: updated,
                        update: update));
                return body;
            }
            else
            {
                return string.Empty;
            }
        }

        public bool InitialValue(Context context)
        {
            return this?.Any() != true;
        }

        public Comments Prepend(
            Context context,
            SiteSettings ss,
            string body,
            bool force = false)
        {
            if (body.Trim() != string.Empty || force == true)
            {
                Insert(0, new Comment
                {
                    CommentId = CommentId(),
                    CreatedTime = DateTime.Now,
                    Creator = context.UserId,
                    Body = body,
                    Created = true
                });
            }
            return this;
        }

        public Comment GetCreated(Context context, SiteSettings ss)
        {
            if (!this.Any(comment => comment.Created))
            {
                Prepend(
                    context: context,
                    ss: ss,
                    body: string.Empty,
                    force: true);
            }
            return this.FirstOrDefault(comment => comment.Created);
        }

        public void Update(Context context, SiteSettings ss, int commentId, string body)
        {
            this.FirstOrDefault(o => o.CommentId == commentId)?
                .Update(context: context, ss: ss, body: body);
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

        public Comments ToLocal(Context context)
        {
            var comments = new Comments();
            ForEach(o => comments.Add(o.ToLocal(context: context)));
            return comments;
        }

        private int DisplayCount(Context context)
        {
            switch (context.Action)
            {
                case "histories": return Parameters.General.CommentDisplayLimitHistories;
                case "deletehistory": return Parameters.General.CommentDisplayLimitHistories;
                default: return Parameters.General.CommentDisplayLimit;
            }
        }

        private string GridCss(Context context)
        {
            if (DisplayCount(context: context) == 3)
            {
                switch (this.Count())
                {
                    case 1: return " one-third";
                    case 2: return " half";
                }
            }
            return null;
        }

        public Comments ClearAndSplitPrepend(
            Context context,
            SiteSettings ss,
            string body,
            bool update,
            bool force = false)
        {
            if (body.Trim() != string.Empty || force == true)
            {
                var splitComments = ToSplitComments(
                    context: context,
                    comments: body);
                if (update
                    && splitComments.Count == 1
                    && splitComments.First().Body == body)
                {
                    return this;
                }
                Clear();
                foreach (var splitComment in splitComments)
                {
                    Insert(0, splitComment);
                }
            }
            return this;
        }

        private List<Comment> ToSplitComments(Context context, string comments)
        {
            var originalComments = new List<Comment>
            {
                new Comment
                {
                    CommentId = CommentId(),
                    CreatedTime = DateTime.Now,
                    Creator = context.UserId,
                    Body = comments,
                    Created = true
                }
            };
            if (string.IsNullOrWhiteSpace(comments)
                || !comments.TrimStart().StartsWith("[")
                || !comments.TrimEnd().EndsWith("]"))
            {
                return originalComments;
            }
            try
            {
                var commentsJArray = JArray.Parse(comments);
                var splitComments = commentsJArray
                    .Reverse()
                    .OfType<JObject>()
                    .Where(jobject => jobject.TryGetValue("Body", out var bodyToken)
                    && !string.IsNullOrWhiteSpace((string?)bodyToken))
                    .Select(token => new Comment
                    {
                        CommentId = (int?)token["CommentId"] ?? CommentId(),
                        CreatedTime = (DateTime?)token["CreatedTime"] ?? DateTime.Now,
                        Creator = (int?)token["Creator"] ?? context.UserId,
                        Body = (string?)token["Body"] ?? string.Empty,
                        Created = true
                    })
                    .OrderBy(comment => comment.CommentId)
                    .ToList();
                return splitComments.Count == 0
                    ? originalComments
                    : splitComments;
            }
            catch (Exception e)
            {
                new SysLogModel(
                    context: context,
                    e: e);
                return originalComments;
            }
        }
    }
}