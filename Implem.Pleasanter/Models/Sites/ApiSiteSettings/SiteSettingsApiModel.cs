using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;

namespace Implem.Pleasanter.Models.ApiSiteSettings
{
    [Serializable]
    public class SiteSettingsApiModel
    {
        public List<ServerScriptApiSettingModel> ServerScripts { get; set; }
        public List<ScriptApiSettingModel> Scripts { get; set; }
        public List<StyleApiSettingModel> Styles { get; set; }
        public List<HtmlApiSettingModel> Htmls { get; set; }
        public List<ProcessApiSettingModel> Processes { get; set; }
        public List<StatusControlApiSettingModel> StatusControls { get; set; }
        public List<ColumnApiSettingModel> Columns { get; set; }
        public Dictionary<string, List<string>> EditorColumnHash;
        public List<string> GridColumns;
        public List<string> FilterColumns;
        public int? SectionLatestId;
        public List<SectionApiSettingModel> Sections { get; set; }
        public List<LinkApiSettingModel> Links;



        public int? TabLatestId;
        public SettingList<Tab> Tabs;

        public SiteSettingsApiModel()
        {
        }
    }
}
