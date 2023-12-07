using Implem.Pleasanter.Models.Shared;
using System;
using System.Collections.Generic;

namespace Implem.Pleasanter.Models.ApiSiteSettings
{
    [Serializable]
    public class ServerScriptApiSettingModel : ISiteSettingBaseProperties
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string Body { get; set; }
        public bool? ServerScriptWhenloadingSiteSettings { get; set; }
        public bool? ServerScriptWhenViewProcessing { get; set; }
        public bool? ServerScriptWhenloadingRecord { get; set; }
        public bool? ServerScriptBeforeFormula { get; set; }
        public bool? ServerScriptAfterFormula { get; set; }
        public bool? CalenServerScriptBeforeFormuladarGuide { get; set; }
        public bool? ServerScriptBeforeCreate { get; set; }
        public bool? ServerScriptAfterCreate { get; set; }
        public bool? ServerScriptBeforeUpdate { get; set; }
        public bool? ServerScriptAfterUpdate { get; set; }
        public bool? ServerScriptBeforeDelete { get; set; }
        public bool? ServerScriptAfterDelete { get; set; }
        public bool? ServerScriptBeforeOpeningPage { get; set; }
        public bool? ServerScriptBeforeOpeningRow { get; set; }
        public bool? ServerScriptShared { get; set; }
        public int? Delete { get; set; }
        public string HtmlPositionType { get; set; }

        public ServerScriptApiSettingModel()
        {
        }
    }
}
