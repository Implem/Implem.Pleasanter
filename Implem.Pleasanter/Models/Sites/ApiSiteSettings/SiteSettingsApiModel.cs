using Implem.Pleasanter.Models.Shared;
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
        public SiteSettingsApiModel()
        {
        }
    }
}
