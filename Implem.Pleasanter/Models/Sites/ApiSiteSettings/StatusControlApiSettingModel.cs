using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;

namespace Implem.Pleasanter.Models.ApiSiteSettings
{
    [Serializable]
    public class StatusControlApiSettingModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? Status { get; set; }
        public bool? ReadOnly { get; set; }
        public Dictionary<string, StatusControl.ControlConstraintsTypes> ColumnHash { get; set; }
        public View View { get; set; }
        public ApiSiteSettingPermission Permission { get; set; }
        public int? Delete { get; set; }

        public StatusControlApiSettingModel()
        {
        }
    }
}
