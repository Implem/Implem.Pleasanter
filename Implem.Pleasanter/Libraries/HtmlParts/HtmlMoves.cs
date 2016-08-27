using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Responses;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlMoves
    {
        public static HtmlBuilder MoveDialog(
            this HtmlBuilder hb, string referenceType, long id, bool bulk = false)
        {
            return hb.Div(
                attributes: new HtmlAttributes()
                    .Id("MoveDialog")
                    .Class("dialog")
                    .Title(Displays.MoveSettings()),
                action: () => hb
                    .Form(
                        attributes: new HtmlAttributes()
                            .Action(Navigations.Action(referenceType, id)),
                        action: () => hb
                            .FieldDropDown(
                                controlId: "MoveTargets",
                                labelText: Displays.Destination(),
                                optionCollection: new Dictionary<string, ControlData>())
                            .P(css: "message-dialog")
                            .Div(css: "command-center", action: () => hb
                                .Button(
                                    text: Displays.Move(),
                                    controlCss: "button-copy",
                                    onClick: "$p.move($(this));",
                                    action: bulk ? "BulkMove" : "Move",
                                    method: "put")
                                .Button(
                                    text: Displays.Cancel(),
                                    controlCss: "button-cancel",
                                    onClick: "$p.closeDialog($(this));"))));
        }
    }
}