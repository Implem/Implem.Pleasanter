using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;

namespace Implem.Pleasanter.Models.ApiSiteSettings
{
    [Serializable]
    public class LabelApiSettingModel
    {
        public int Id;
        public string Body;
        public string LabelType;
        public bool? Hide;

        public LabelApiSettingModel()
        {
        }

        public Label GetRecordingData(SiteSettings ss)
        {
            var label = new Label();
            label.Id = Id;
            label.Body = Body;
            label.LabelType = LabelTypes.Normalize(LabelType);
            return label;
        }
    }
}
