using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.ViewModes
{
    public class AnalyPart
    {
        public AnalyPartSetting Setting { get; set; }
        public List<AnalyPartElement> Elements { get; set; } = new List<AnalyPartElement>();

        public AnalyPart(AnalyPartSetting setting)
        {
            Setting = setting;
        }
    }
}
