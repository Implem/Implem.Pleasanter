using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Resources;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
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
                            .Text(text: CreatedTimeDisplayValue()))
                        .HtmlUser(Updator ?? Creator);
                    if (CanEdit(allowEditing, readOnly))
                    {
                        hb.MarkDown(
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

        public string CreatedTimeDisplayValue()
        {
            return UpdatedTime == null
                ? CreatedTime.ToLocal(Displays.Get("YmdahmFormat"))
                : UpdatedTime
                    .ToDateTime()
                    .ToLocal(Displays.Get("YmdahmFormat"))
                        + " [" + Displays.CommentUpdated() + "]";
        }

        private bool CanEdit(bool allowEditing, bool readOnly)
        {
            return
                allowEditing && !readOnly && Creator == Sessions.UserId();
        }

        public void Update(string body)
        {
            UpdatedTime = DateTime.Now;
            Updator = Sessions.UserId();
            Body = body;
            Updated = true;
        }

        public Comment ToLocal()
        {
            return new Comment()
            {
                CommentId = CommentId,
                CreatedTime = CreatedTime.ToLocal(),
                UpdatedTime = UpdatedTime?.ToLocal(),
                Creator = Creator,
                Updator = Updator,
                Body = Body
            };
        }
    }
}