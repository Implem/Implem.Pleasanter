using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlComments
    {
        public static HtmlBuilder Comments(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Comments comments,
            Column column,
            Versions.VerTypes verType,
            Permissions.ColumnPermissionTypes columnPermissionType)
        {
            var readOnly = verType != Versions.VerTypes.Latest ||
                !context.CanUpdate(ss: ss) ||
                column?.EditorReadOnly == true ||
                columnPermissionType != Permissions.ColumnPermissionTypes.Update;
            return hb
                .TextArea(
                    context: context,
                    ss: ss,
                    title: column?.Description,
                    labelText: column?.LabelText,
                    allowImage: column?.AllowImage == true,
                    mobile: column?.SiteSettings.Mobile == true,
                    _using: !readOnly)
                .Div(id: "CommentList", action: () => comments
                    .ForEach(comment => hb
                        .Comment(
                            context: context,
                            ss: ss,
                            column: column,
                            comment: comment,
                            readOnly: readOnly)));
        }

        public static HtmlBuilder Comment(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Column column,
            Comment comment,
            bool readOnly)
        {
            return comment.Html(
                hb: hb,
                context: context,
                ss: ss,
                allowEditing: ss.AllowEditingComments == true,
                allowImage: column.AllowImage == true,
                mobile: ss.Mobile,
                readOnly: readOnly,
                controlId: "Comment" + comment.CommentId,
                action: () => hb
                    .DeleteComment(comment: comment, readOnly: readOnly));
        }

        private static HtmlBuilder TextArea(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            string title,
            string labelText,
            bool allowImage,
            bool mobile,
            bool _using = true)
        {
            return _using
                ? hb.Div(id: "CommentField", action: () =>
                    hb
                        .TextArea(
                            attributes: new HtmlAttributes()
                                .Id("Comments")
                                .Class("control-textarea" +
                                    (context.ContractSettings.Images() && allowImage
                                        ? " upload-image"
                                        : string.Empty))
                                .Title(title)
                                .Placeholder(labelText))
                        .MarkDownCommands(
                            context: context,
                            controlId: "Comments",
                            readOnly: false,
                            allowImage: allowImage,
                            mobile: mobile,
                            preview: false))
                : hb;
        }

        private static HtmlBuilder DeleteComment(
            this HtmlBuilder hb, Comment comment, bool readOnly)
        {
            return !readOnly
                ? hb.P(
                    attributes: new HtmlAttributes()
                        .Id("DeleteComment," + comment.CommentId)
                        .Class("button delete")
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