using Implem.Pleasanter.Models.Shared;
using System;
using System.Collections.Generic;

namespace Implem.Pleasanter.Models.ApiSiteSettings
{
    [Serializable]
    public class StyleApiSettingModel : ISiteSettingBaseProperties
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public bool? Disabled { get; set; }
        public string Body { get; set; }
        public bool? StyleAll { get; set; }
        public bool? StyleNew { get; set; }
        public bool? StyleEdit { get; set; }
        public bool? StyleIndex { get; set; }
        public bool? StyleCalendar { get; set; }
        public bool? StyleCrosstab { get; set; }
        public bool? StyleGantt { get; set; }
        public bool? StyleBurnDown { get; set; }
        public bool? StyleTimeSeries { get; set; }
        public bool? StyleAnaly { get; set; }
        public bool? StyleKamban { get; set; }
        public bool? StyleImageLib { get; set; }
        public int? Delete { get; set; }
        public string HtmlPositionType { get; set; }

        public StyleApiSettingModel()
        {
        }
    }
}
