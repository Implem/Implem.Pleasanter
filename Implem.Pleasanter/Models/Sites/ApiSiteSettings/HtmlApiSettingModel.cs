using Implem.Pleasanter.Models.Shared;
using System;
using System.Collections.Generic;

namespace Implem.Pleasanter.Models.ApiSiteSettings
{
    [Serializable]
    public class HtmlApiSettingModel : ISiteSettingBaseProperties
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public bool? Disabled { get; set; }
        public string Body { get; set; }
        public string HtmlPositionType { get; set; }
        public bool? HtmlAll { get; set; }
        public bool? HtmlNew { get; set; }
        public bool? HtmlEdit { get; set; }
        public bool? HtmlIndex { get; set; }
        public bool? HtmlCalendar { get; set; }
        public bool? HtmlCrosstab { get; set; }
        public bool? HtmlGantt { get; set; }
        public bool? HtmlBurnDown { get; set; }
        public bool? HtmlTimeSeries { get; set; }
        public bool? HtmlAnaly { get; set; }
        public bool? HtmlKamban { get; set; }
        public bool? HtmlImageLib { get; set; }
        public int? Delete { get; set; }

        public HtmlApiSettingModel()
        {
        }
    }
}
