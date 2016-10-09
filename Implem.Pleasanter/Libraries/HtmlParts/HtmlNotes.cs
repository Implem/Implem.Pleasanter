using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlNotes
    {
        public static HtmlBuilder Notes(
            this HtmlBuilder hb,
            Permissions.Types permissionType,
            Versions.VerTypes verType)
        {
            var notes = new Dictionary<string, string>();
            if (!permissionType.CanUpdate())
            {
                notes.Add("readonly", Displays.CanNotUpdate());
            }
            if (verType == Versions.VerTypes.History)
            {
                notes.Add("history", Displays.ReadOnlyBecausePreviousVer());
            }
            if (notes.Any())
            {
                hb.Div(id: "Notes", action: () =>
                    notes.ForEach(part => hb
                        .P(css: part.Key, action: () => hb
                            .Text(text: part.Value))));
            }
            return hb;
        }
    }
}