using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Resources;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
namespace Implem.Pleasanter.Libraries.DataTypes
{
    public class Comment
    {
        public int CommentId;
        public DateTime CreatedTime;
        public DateTime? UpdatedTime;
        public int Creator;
        public int? Updator;
        public string Body;
        [NonSerialized]
        public bool Created;
        [NonSerialized]
        public bool Updated;

        public HtmlBuilder Html(
            HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            bool allowEditing,
            bool allowImage,
            bool mobile,
            bool readOnly = false,
            string controlId = null,
            string css = null,
            Action action = null)
        {
            return hb.Div(
                id: !controlId.IsNullOrEmpty()
                    ? controlId + ".wrapper"
                    : null,
                css: Css.Class("comment", css),
                action: () =>
                {
                    action?.Invoke();
                    hb
                        .P(css: "time", action: () => hb
                            .Text(text: CreatedTimeDisplayValue(context: context)))
                        .HtmlUser(
                            context: context,
                            id: Updator ?? Creator);
                    if (CanEdit(
                        context: context,
                        ss: ss,
                        allowEditing: allowEditing,
                        readOnly: readOnly))
                    {
                        hb.MarkDown(
                            context: context,
                            controlId: controlId,
                            text: Body,
                            allowImage: allowImage,
                            mobile: mobile);
                    }
                    else
                    {
                        hb
                            .P(css: "body markup", action: () => hb
                                .Text(text: Body));
                    }
                });
        }

        public string CreatedTimeDisplayValue(Context context)
        {
            return UpdatedTime == null
                ? CreatedTime.ToLocal(
                    context: context,
                    format: Displays.Get(
                        context: context,
                        id: "YmdahmFormat"))
                : UpdatedTime
                    .ToDateTime()
                    .ToLocal(
                        context: context,
                        format: Displays.Get(
                            context: context,
                            id: "YmdahmFormat"))
                                + $" [{Displays.CommentUpdated(context: context)}]";
        }

        private bool CanEdit(Context context, SiteSettings ss, bool allowEditing, bool readOnly)
        {
            return allowEditing && !readOnly && Creator == context.UserId;
        }

        public void Update(Context context, SiteSettings ss, string body)
        {
            UpdatedTime = DateTime.Now;
            Updator = context.UserId;
            Body = body;
            Updated = true;
        }

        public Comment ToLocal(Context context)
        {
            return new Comment()
            {
                CommentId = CommentId,
                CreatedTime = CreatedTime.ToLocal(context: context),
                UpdatedTime = UpdatedTime?.ToLocal(context: context),
                Creator = Creator,
                Updator = Updator,
                Body = Body
            };
        }
    }
}