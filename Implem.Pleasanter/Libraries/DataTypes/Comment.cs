using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using System;
namespace Implem.Pleasanter.Libraries.DataTypes
{
    public class Comment
    {
        public int CommentId;
        public DateTime CreatedTime;
        public int Creator;
        public string Body;

        public HtmlBuilder Html(HtmlBuilder hb, string controlId = "", Action action = null)
        {
            return hb.Div(
                id: controlId,
                css: "comment",
                action: () =>
                {
                    action?.Invoke();
                    hb
                        .P(css: "time", action: () => hb
                            .Text(text: CreatedTimeDisplayValue()))
                        .HtmlUser(Creator)
                        .P(css: "body markup", action: () => hb
                            .Text(text: Body));
                });
        }

        public string CreatedTimeDisplayValue()
        {
            return CreatedTime.ToLocal(Displays.Get("YmdahmFormat"));
        }
    }
}