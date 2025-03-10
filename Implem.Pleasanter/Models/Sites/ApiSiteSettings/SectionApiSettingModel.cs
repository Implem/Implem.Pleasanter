using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;

namespace Implem.Pleasanter.Models.ApiSiteSettings
{
    [Serializable]
    public class SectionApiSettingModel
    {
        public int Id;
        public string LabelText;
        public bool? AllowExpand;
        public bool? Expand;
        public bool? Hide;

        public SectionApiSettingModel()
        {
        }
        public Section GetRecordingData(SiteSettings ss)
        {
            var section = new Section();
            section.Id = Id;
            section.LabelText = LabelText;
            section.AllowExpand = AllowExpand;
            section.Expand = Expand ?? true;
            return section;
        }
    }
}