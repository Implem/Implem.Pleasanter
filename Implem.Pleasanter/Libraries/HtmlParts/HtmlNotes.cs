using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlNotes
    {
        public static HtmlBuilder Notes(this HtmlBuilder hb, BaseModel baseModel)
        {
            var notes = new Dictionary<string, string>();
            if (!baseModel.PermissionType.CanUpdate())
            {
                notes.Add("readonly", Displays.CanNotUpdate());
            }
            if (baseModel.VerType == Versions.VerTypes.History)
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