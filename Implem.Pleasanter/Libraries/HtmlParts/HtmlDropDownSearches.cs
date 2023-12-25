using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
                            .Action(DropDownSearchDialogFormAction(
                                context: context,
                                id: id)),
                        action: () => hb
                            .Div(id: "DropDownSearchDialogBody")
                            .Hidden(
                                controlId: "DropDownSearchReferenceId",
                                css: "always-send")
                            .Hidden(
                                controlId: "DropDownSearchSelectedValues",
                                css: "always-send")
                            .Hidden(
                                controlId: "DropDownSearchTarget",
                                css: "always-send",
                                action: "SearchDropDown",
                                method: "post")
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

        private static string DropDownSearchDialogFormAction(Context context, long id)
        {
            switch (context.Controller)
            {
                case "items":
                case "publishes":
                    return Locations.ItemAction(
                        context: context,
                        id: id);
                default:
                    return Locations.Action(
                        context: context,
                        controller: context.Controller);
            }
        }

        public static HtmlBuilder DropDownSearchDialogBody(this HtmlBuilder hb, Context context, Column column, bool filter)
        {
            var selectedValues = context.Forms.List("DropDownSearchSelectedValues");
            selectedValues.ForEach(value =>
                column?.AddToChoiceHash(
                    context: context,
                    value: value));
            var listItemCollection = column?.EditChoices(
                context: context,
                addNotSet: column?.MultipleSelections != true,
                own: filter);
            if (column?.MultipleSelections == true || filter)
            {
                return hb
                    .FieldSelectable(
                        controlId: "DropDownSearchResults",
                        fieldCss: "field-vertical w350",
                        controlContainerCss: "container-selectable",
                        controlWrapperCss: " h300",
                        controlCss: " always-send send-all",
                        listItemCollection: listItemCollection
                            .Where(o => selectedValues.Contains(o.Key))
                            .ToDictionary(o => o.Key, o => o.Value),
                        commandOptionPositionIsTop: true,
                        alwaysDataValue: true,
                        commandOptionAction: () => hb
                            .Div(css: "command-right", action: () => hb
                                .Button(
                                    controlId: "ToDisableDropDownSearchResults",
                                    controlCss: "button-icon",
                                    text: Displays.ToDisable(context: context),
                                    onClick: "$p.moveColumns(event, $(this),'DropDownSearch',false,false,'Results');",
                                    icon: "ui-icon-circle-triangle-e")))
                    .FieldSelectable(
                        controlId: "DropDownSearchSourceResults",
                        fieldCss: "field-vertical w350",
                        controlContainerCss: "container-selectable",
                        controlWrapperCss: " h300",
                        listItemCollection: listItemCollection
                            .Where(o => !selectedValues.Contains(o.Key))
                            .ToDictionary(o => o.Key, o => o.Value),
                        commandOptionPositionIsTop: true,
                        alwaysDataValue: true,
                        action: "SearchDropDown",
                        method: "post",
                        commandOptionAction: () => hb
                            .Div(css: "command-left", action: () => hb
                                .Button(
                                    controlId: "ToEnableDropDownSearchResults",
                                    text: Displays.ToEnable(context: context),
                                    controlCss: "button-icon",
                                    onClick: "$p.moveColumns(event, $(this),'DropDownSearch',false,false,'Results');",
                                    icon: "ui-icon-circle-triangle-w")
                                .TextBox(
                                    controlId: "DropDownSearchText",
                                    controlCss: " auto-postback always-send w150",
                                    action: "SearchDropDown",
                                    method: "post")
                                .Button(
                                    text: Displays.Search(context: context),
                                    controlCss: "button-icon",
                                    onClick: "$p.send($('#DropDownSearchText'));",
                                    icon: "ui-icon-search")));
            }
            else
            {
                return hb.FieldSelectable(
                    controlId: "DropDownSearchResults",
                    fieldCss: "field-vertical w600",
                    controlContainerCss: "container-selectable",
                    controlWrapperCss: " h300",
                    listItemCollection: listItemCollection,
                    commandOptionPositionIsTop: true,
                    action: "SearchDropDown",
                    method: "post",
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
                                icon: "ui-icon-search")));
            }
        }

        public static HtmlBuilder DropDownSearchDialogBodyInheritPermission(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            int offset,
            int pageSize)
        {
            var listItemCollection = PermissionUtilities.InheritTargets(
                context: context,
                ss: ss,
                offset: offset,
                pageSize: pageSize);

            return hb.FieldSelectable(
                controlId: "DropDownSearchResults",
                fieldCss: "field-vertical w600",
                controlContainerCss: "container-selectable",
                controlWrapperCss: " h300",
                listItemCollection: listItemCollection,
                commandOptionPositionIsTop: true,
                action: "SearchDropDown",
                method: "post",
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
                            icon: "ui-icon-search")));
        }
    }
}