using System.Collections.Generic;

namespace Implem.Pleasanter.Libraries.Settings
{
    public class ApiSiteSetting
    {
        public enum DeleteFlag : int
        {
            IsDelete = 1,
            IsNotDelete = 0
        }

        public static List<string> HtmlPositionTypes { get; } = new List<string>
        {
            "Headtop",
            "Headbottom",
            "Bodyscripttop",
            "Bodyscriptbottom"
        };

        public static List<string> ServerScriptRefTypes { get; } = new List<string>
        {
            "Results",
            "Issues"
        };
    }
}