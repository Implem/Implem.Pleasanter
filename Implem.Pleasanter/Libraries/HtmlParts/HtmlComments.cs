using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlComments
    {
        public static HtmlBuilder Comments(
            this HtmlBuilder hb,
            Comments comments,
            Versions.VerTypes verType,
            Permissions.ColumnPermissionTypes columnPermissionType)
        {
            return hb
                .TextArea(_using: verType == Versions.VerTypes.Latest &&
                    columnPermissionType == Permissions.ColumnPermissionTypes.Update)
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

        private static HtmlBuilder TextArea(this HtmlBuilder hb, bool _using = true)
        {
            return _using
                ? hb.Div(id: "CommentField", action: () =>
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