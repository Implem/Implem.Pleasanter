using System.Collections.Generic;

namespace Implem.Pleasanter.Libraries.Settings
{
    /// <summary>
    /// SiteSetting By Api Master Config
    /// </summary>
    public class ApiSiteSetting
    {
        public enum DeleteFlag : int
        {
            IsDelete = 1,
            IsNotDelete = 0
        }

        public const string SITE_SETTING_TYPE_SERVER_SCRIPT = "ServerScripts";
        public const string SITE_SETTING_TYPE_SCRIPT = "Scripts";
        public const string SITE_SETTING_TYPE_HTML = "Htmls";
        public const string SITE_SETTING_TYPE_STYLE = "Styles";

        public static List<string> HtmlPositionTypes { get; } = new List<string>
        {
            "Headtop",
            "Headbottom",
            "Bodyscripttop",
            "Bodyscriptbottom"
        };

        /// <summary>
        /// ReferenceType contains ServerScript setting
        /// </summary>
        public static List<string> ServerScriptRefType { get; } = new List<string>
        {
            "Results",
            "Issues"
        };
    }
}