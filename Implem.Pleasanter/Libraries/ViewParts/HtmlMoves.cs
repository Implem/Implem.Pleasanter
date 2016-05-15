using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Responses;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.ViewParts
{
    public static class HtmlMoves
    {
        public static HtmlBuilder Dialog_Move(
            this HtmlBuilder hb, string referenceType, long id, bool bulk = false)
        {
            return hb.Div(
                attributes: Html.Attributes()
                    .Id_Css("Dialog_Move", "dialog")
                    .Title(Displays.MoveSettings()),
                action: () => hb
                    .Form(
                        attributes: Html.Attributes()
                            .Action(Navigations.Action(referenceType, id)),
                        action: () => hb
                            .FieldDropDown(
                                controlId: "Dialog_MoveTargets",
                                labelText: Displays.Destination(),
                                optionCollection: new Dictionary<string, ControlData>())
                            .P(css: "message-dialog")
                            .Div(css: "command-center", action: () => hb
                                .Button(
                                    text: Displays.Move(),
                                    controlCss: "button-copy",
                                    onClick: Def.JavaScript.Move,
                                    action: bulk ? "BulkMove" : "Move",
                                    method: "put")
                                .Button(
                                    text: Displays.Cancel(),
                                    controlCss: "button-cancel",
                                    onClick: Def.JavaScript.CancelDialog))));
        }
    }
}