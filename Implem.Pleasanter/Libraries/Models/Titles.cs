using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Data;
namespace Implem.Pleasanter.Libraries.Models
{
    public static class Titles
    {
        public static string DisplayValue(SiteSettings ss, DataRow dataRow)
        {
            switch (ss.ReferenceType)
            {
                case "Issues": return IssueUtilities.TitleDisplayValue(ss, dataRow);
                case "Results": return ResultUtilities.TitleDisplayValue(ss, dataRow);
                case "Wikis": return WikiUtilities.TitleDisplayValue(ss, dataRow);
                default: return string.Empty;
            }
        }
    }
}
