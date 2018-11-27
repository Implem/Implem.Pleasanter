﻿using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlMoves
    {
        public static HtmlBuilder MoveDialog(
            this HtmlBuilder hb, Context context, bool bulk = false)
        {
            return hb.Div(
                attributes: new HtmlAttributes()
                    .Id("MoveDialog")
                    .Class("dialog")
                    .Title(Displays.MoveSettings(context: context)),
                action: () => hb
                    .FieldDropDown(
                        context: context,
                        controlId: "MoveTargets",
                        controlCss: " always-send not-set-form-changed",
                        labelText: Displays.Destination(context: context),
                        optionCollection: new Dictionary<string, ControlData>())
                    .P(css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            text: Displays.Move(context: context),
                            controlCss: "button-icon",
                            onClick: "$p.move($(this));",
                            icon: "ui-icon-copy",
                            action: bulk ? "BulkMove" : "Move",
                            method: "put")
                        .Button(
                            text: Displays.Cancel(context: context),
                            controlCss: "button-icon",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }
    }
}