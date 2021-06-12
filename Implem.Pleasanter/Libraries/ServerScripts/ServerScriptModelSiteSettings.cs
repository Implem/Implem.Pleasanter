using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptModelSiteSettings
    {
        public int? DefaultViewId { get; set; }
        public List<Section> Sections { get; set; }
    }
}