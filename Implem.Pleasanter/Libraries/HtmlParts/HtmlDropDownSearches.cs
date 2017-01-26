using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Responses;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlDropDownSearches
    {
        public static HtmlBuilder DropDownSearchDialog(
            this HtmlBuilder hb, string controller, long id)
        {
            return hb.Div(
                attributes: new HtmlAttributes()
                    .Id("DropDownSearchDialog")
                    .Class("dialog"),
                action: () => hb
                    .Form(
                        attributes: new HtmlAttributes()
                            .Id("DropDownSearchDialogForm")
                            .Action(Locations.Action(controller, id)),
                        action: () => hb
                            .FieldSelectable(
                                controlId: "DropDownSearchResults",
                                fieldCss: "field-vertical w600",
                                controlContainerCss: "container-selectable",
                                controlWrapperCss: " h300",
                                listItemCollection: null,
                                selectedValueCollection: null,
                                commandOptionPositionIsTop: true,
                                commandOptionAction: () => hb
                                    .FieldTextBox(
                                        controlId: "DropDownSearchText",
                                        fieldCss: "field-auto-thin",
                                        controlCss: " auto-postback must-transport",
                                        labelText: Displays.Search(),
                                        action: "SearchDropDown",
                                        method: "post"))
                            .Hidden(
                                controlId: "DropDownSearchTarget",
                                css: "must-transport")
                            .Hidden(
                                controlId: "DropDownSearchMultiple",
                                css: "must-transport")
                            .P(css: "message-dialog")
                            .Div(css: "command-center", action: () => hb
                                .Button(
                                    text: Displays.Select(),
                                    controlCss: "button-icon",
                                    onClick: "$p.send($(this));",
                                    icon: "ui-icon-disk",
                                    action: "SelectSearchDropDown",
                                    method: "post")
                                .Button(
                                    text: Displays.Cancel(),
                                    controlCss: "button-icon",
                                    onClick: "$p.closeDialog($(this));",
                                    icon: "ui-icon-cancel"))));
        }
    }
}