using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Responses;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlComments
    {
        public static HtmlBuilder Comments(
            this HtmlBuilder hb, Comments comments, Versions.VerTypes verType)
        {
            return hb
                .TextArea(verType: verType)
                .Div(id: "CommentList", action: () => comments
                    .ForEach(comment => hb
                        .Comment(comment, verType)));
        }

        public static HtmlBuilder Comment(
            this HtmlBuilder hb, Comment comment, Versions.VerTypes verType)
        {
            return comment.Html(
                hb: hb,
                controlId: "Comment" + comment.CommentId,
                action: () => hb
                    .DeleteComment(comment: comment, verType: verType));
        }

        private static HtmlBuilder TextArea(this HtmlBuilder hb, Versions.VerTypes verType)
        {
            return verType == Versions.VerTypes.Latest
                ? hb.Div(action: () =>
                    hb.TextArea(
                        id: "Comments",
                        css: "control-textarea upload-image",
                        placeholder: Displays.Comments()))
                : hb;
        }

        private static HtmlBuilder DeleteComment(
            this HtmlBuilder hb, Comment comment, Versions.VerTypes verType)
        {
            return verType == Versions.VerTypes.Latest
                ? hb.P(
                    attributes: new HtmlAttributes()
                        .Id("DeleteComment," + comment.CommentId)
                        .Class("button")
                        .OnClick("$p.send($(this));")
                        .DataAction("DeleteComment")
                        .DataMethod("delete")
                        .DataConfirm("ConfirmDelete"),
                    action: () => hb
                        .Icon(iconCss: "ui-icon ui-icon-closethick"))
                : hb;
        }
    }
}