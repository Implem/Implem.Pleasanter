using Implem.Pleasanter.Models.Shared;
using System;
using System.Collections.Generic;

namespace Implem.Pleasanter.Models.ApiSiteSettings
{
    [Serializable]
    public class ScriptApiSettingModel :ISiteSettingBaseProperties
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public bool? Disabled { get; set; }
        public string Body { get; set; }
        public bool? ScriptAll { get; set; }
        public bool? ScriptNew { get; set; }
        public bool? ScriptEdit { get; set; }
        public bool? ScriptIndex { get; set; }
        public bool? ScriptCalendar { get; set; }
        public bool? ScriptCrosstab { get; set; }
        public bool? ScriptGantt { get; set; }
        public bool? ScriptBurnDown { get; set; }
        public bool? ScriptTimeSeries { get; set; }
        public bool? ScriptKamban { get; set; }
        public bool? ScriptImageLib { get; set; }
        public int? Delete { get; set; }
        public string HtmlPositionType { get; set; }

        public ScriptApiSettingModel()
        {
        }
    }
}
