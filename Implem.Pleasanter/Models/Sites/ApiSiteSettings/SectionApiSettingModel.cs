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
    }
}