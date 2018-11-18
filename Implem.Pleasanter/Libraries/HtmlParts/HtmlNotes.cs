using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlNotes
    {
        public static HtmlBuilder Notes(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Versions.VerTypes verType)
        {
            var notes = new Dictionary<string, string>();
            if (!context.CanUpdate(ss: ss))
            {
                notes.Add("readonly", Displays.CanNotUpdate(context: context));
            }
            if (verType == Versions.VerTypes.History)
            {
                notes.Add("history", Displays.ReadOnlyBecausePreviousVer(context: context));
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