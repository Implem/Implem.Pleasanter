using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Models;
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
            Versions.VerTypes? verType = null,
            string controlId = null,
            Action action = null)
        {
            return hb.Div(
                id: !controlId.IsNullOrEmpty()
                    ? controlId + ".wrapper"
                    : null,
                css: "comment",
                action: () =>
                {
                    action?.Invoke();
                    hb
                        .P(css: "time", action: () => hb
                            .Text(text: CreatedTimeDisplayValue()))
                        .HtmlUser(Updator ?? Creator);
                    if (CanEdit(allowEditing, verType))
                    {
                        hb.MarkDown(
                            controlId: controlId,
                            text: Body);
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

        private bool CanEdit(bool allowEditing, Versions.VerTypes? verType)
        {
            return
                allowEditing &&
                verType == Versions.VerTypes.Latest &&
                Creator == Sessions.UserId();
        }

        public void Update(string body)
        {
            UpdatedTime = DateTime.Now;
            Updator = Sessions.UserId();
            Body = body;
            Updated = true;
        }
    }
}