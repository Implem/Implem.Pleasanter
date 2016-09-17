using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Models;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlNotes
    {
        public static HtmlBuilder Notes(this HtmlBuilder hb, BaseModel baseModel)
        {
            if (!baseModel.PermissionType.CanUpdate())
            {
                hb.Notes(
                    baseModel,
                    Displays.CanNotUpdate(),
                    "readonly");
            }
            if (baseModel.VerType == Versions.VerTypes.History)
            {
                hb.Notes(
                    baseModel,
                    Displays.ReadOnlyBecausePreviousVer(),
                    "history");
            }
            return hb;
        }

        private static HtmlBuilder Notes(
            this HtmlBuilder hb, BaseModel baseModel, string text, string css)
        {
            return hb.P(id: "Notes", css: "notes", action: () => hb
                .Span(css: css, action: () => hb
                    .Text(text: text)));
        }
    }
}   