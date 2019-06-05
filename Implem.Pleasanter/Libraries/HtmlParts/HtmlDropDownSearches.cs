﻿using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlDropDownSearches
    {
        public static HtmlBuilder DropDownSearchDialog(
            this HtmlBuilder hb, Context context, long id)
        {
            return hb.Div(
                attributes: new HtmlAttributes()
                    .Id("DropDownSearchDialog")
                    .Class("dialog"),
                action: () => hb
                    .Form(
                        attributes: new HtmlAttributes()
                            .Id("DropDownSearchDialogForm")
                            .Action(Locations.ItemAction(
                                context: context,
                                id: id)),
                        action: () => hb
                            .FieldSelectable(
                                controlId: "DropDownSearchResults",
                                fieldCss: "field-vertical w600",
                                controlContainerCss: "container-selectable",
                                controlWrapperCss: " h300",
                                commandOptionPositionIsTop: true,
                                commandOptionAction: () => hb
                                    .Div(css: "command-left", action: () => hb
                                        .TextBox(
                                            controlId: "DropDownSearchText",
                                            controlCss: " auto-postback always-send w200",
                                            action: "SearchDropDown",
                                            method: "post")
                                        .Button(
                                            text: Displays.Search(context: context),
                                            controlCss: "button-icon",
                                            onClick: "$p.send($('#DropDownSearchText'));",
                                            icon: "ui-icon-search")))
                            .Hidden(
                                controlId: "DropDownSearchTarget",
                                css: "always-send")
                            .Hidden(
                                controlId: "DropDownSearchMultiple",
                                css: "always-send")
                            .Hidden(
                                controlId: "DropDownSearchResultsOffset",
                                value: "0",
                                css: "always-send")
                            .Hidden(
                                controlId: "DropDownSearchParentClass",
                                value: "",
                                css: "always-send")
                            .Hidden(
                                controlId: "DropDownSearchParentDataId",
                                value: "0",
                                css: "always-send")
                            .P(css: "message-dialog")
                            .Div(css: "command-center", action: () => hb
                                .Button(
                                    text: Displays.Select(context: context),
                                    controlCss: "button-icon",
                                    onClick: "$p.send($(this));",
                                    icon: "ui-icon-disk",
                                    action: "SelectSearchDropDown",
                                    method: "post")
                                .Button(
                                    text: Displays.Cancel(context: context),
                                    controlCss: "button-icon",
                                    onClick: "$p.closeDialog($(this));",
                                    icon: "ui-icon-cancel"))));
        }
    }
}