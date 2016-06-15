using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Data;
namespace Implem.Pleasanter.Libraries.Models
{
    public static class Titles
    {
        public static string DisplayValue(SiteSettings siteSettings, DataRow dataRow)
        {
            switch (siteSettings.ReferenceType)
            {
                case "Issues": return IssuesUtility.TitleDisplayValue(siteSettings, dataRow);
                case "Results": return ResultsUtility.TitleDisplayValue(siteSettings, dataRow);
                case "Wikis": return WikisUtility.TitleDisplayValue(siteSettings, dataRow);
                default: return string.Empty;
            }
        }
    }
}
