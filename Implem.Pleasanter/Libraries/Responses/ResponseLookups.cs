using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using System.Linq;

namespace Implem.Pleasanter.Libraries.Responses
{
    public static class ResponseLookups
    {
        public static ResponseCollection LookupClearFormData(
            this ResponseCollection res,
            Context context,
            SiteSettings ss)
        {
            ss.Links
                ?.Where(link => $"{ss.ReferenceType}_{link.ColumnName}" == context.Forms.ControlId())
                .Where(link => link.Lookups != null)
                .SelectMany(link => link.Lookups)
                .Where(lookup => lookup.OverwriteForm == true)
                .ForEach(lookup => res.ClearFormData($"{ss.ReferenceType}_{lookup.To}"));
            return res;
        }
    }
}
