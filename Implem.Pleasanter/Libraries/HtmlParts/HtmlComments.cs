using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
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
            var css = column.TextAlign == SiteSettings.TextAlignTypes.Right
                ? " right-align "
                : string.Empty;
            return hb
                .TextArea(
                    context: context,
                    ss: ss,
                    css: css,
                    title: column?.Description,
                    labelText: column?.LabelText,
                    allowImage: column?.AllowImage == true,
                    mobile: context.Mobile == true,
                    _using: !readOnly,
                    validateMaxLength: column.MaxLength.ToInt(),
                    validateRegex: column.ClientRegexValidation,
                    validateRegexErrorMessage:column.RegexValidationMessage)
                .Div(id: "CommentList", css: css, action: () => comments
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
                mobile: context.Mobile,
                readOnly: readOnly,
                controlId: "Comment" + comment.CommentId,
                action: () => hb
                    .DeleteComment(comment: comment, readOnly: readOnly));
        }

        private static HtmlBuilder TextArea(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            string css,
            string title,
            string labelText,
            string validateRegex,
            string validateRegexErrorMessage,
            bool allowImage,
            bool mobile,
            bool _using = true,
            int validateMaxLength = 0)
        {
            return _using
                ? hb.Div(id: "CommentField", css: css, action: () =>
                    hb
                        .TextArea(
                            attributes: new HtmlAttributes()
                                .Id("Comments")
                                .Name("Comments")
                                .Class("control-textarea" +
                                    (context.ContractSettings.Images() && allowImage
                                        ? " upload-image"
                                        : string.Empty) +
                                    css)
                                .Title(title)
                                .DataValidateMaxLength(validateMaxLength)
                                .DataValidateRegex(validateRegex)
                                .DataValidateRegexErrorMessage(validateRegexErrorMessage)
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