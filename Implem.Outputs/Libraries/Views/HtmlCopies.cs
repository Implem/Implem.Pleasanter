using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Responses;
namespace Implem.Pleasanter.Libraries.Views
{
    public static class HtmlCopies
    {
        public static HtmlBuilder Dialog_Copy(
            this HtmlBuilder hb, string referenceType, long id)
        {
            return hb.Div(
                attributes: Html.Attributes()
                    .Id_Css("Dialog_ConfirmCopy", "dialog")
                    .Title(Displays.CopySettings()),
                action: () => hb
                    .Form(
                        attributes: Html.Attributes()
                            .Action(Navigations.Action(referenceType, id)),
                        action: () => hb
                            .FieldCheckBox(
                                controlId: "Dialog_ConfirmCopy_WithComments",
                                labelText: Displays.CopyWithComments(),
                                _checked: true,
                                fieldCss: "field-wide",
                                controlContainerCss: "m-l50",
                                controlCss: " must-transport",
                                labelPositionIsRight: true)
                            .P(css: "message-dialog")
                            .Div(css: "command-center", action: () => hb
                                .Button(
                                    text: Displays.Copy(),
                                    controlCss: "button-copy",
                                    onClick: Def.JavaScript.CloseDialogAndSubmit,
                                    action: "Copy",
                                    method: "post")
                                .Button(
                                    text: Displays.Cancel(),
                                    controlCss: "button-cancel",
                                    onClick: Def.JavaScript.CancelDialog))));
        }
    }
}