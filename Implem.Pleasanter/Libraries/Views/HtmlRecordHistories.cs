using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Utilities;
namespace Implem.Pleasanter.Libraries.Views
{
    public static class HtmlRecordHistories
    {
        public static HtmlBuilder RecordHistories(
            this HtmlBuilder hb, int ver, Versions.VerTypes verType)
        {
            var hasHistories = !(ver == 1 && verType == Versions.VerTypes.Latest);
            return hb
                .Button(
                    text: Displays.Older(),
                    controlCss: "button-previous",
                    onClick: hasHistories && ver != 1
                        ? Def.JavaScript.History.Params(ver)
                        : string.Empty,
                    action: "PreviousHistory",
                    method: "post")
                .Button(
                    text: Displays.HistoryList(),
                    controlCss: "button-history",
                    onClick: Def.JavaScript.Histories,
                    action: "Histories",
                    method: "get")
                .Button(
                    text: Displays.Newer(),
                    controlCss: "button-next",
                    onClick: hasHistories && verType != Versions.VerTypes.Latest
                        ? Def.JavaScript.History.Params(ver)
                        : string.Empty,
                    action: "NextHistory",
                    method: "post")
                .Button(
                    text: Displays.Latest(),
                    controlCss: "button-history",
                    onClick: Def.JavaScript.Submit,
                    action: "Reload",
                    method: "post");
        }

        public static HtmlBuilder Dialog_Histories(this HtmlBuilder hb, string action)
        {
            return hb.Div(
                attributes: Html.Attributes()
                    .Id_Css("Dialog_Histories", "dialog")
                    .Title(Displays.HistoryList()),
                action: () => hb
                    .Form(attributes: Html.Attributes()
                        .Id_Css("HistoriesForm", "histories-form")
                        .Action(action)));
        }
    }
}