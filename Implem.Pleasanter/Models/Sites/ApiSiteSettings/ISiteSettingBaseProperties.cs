using Implem.Pleasanter.Models.Shared;
using System;
using System.Collections.Generic;

namespace Implem.Pleasanter.Models.ApiSiteSettings
{
    public interface ISiteSettingBaseProperties
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public int? Delete { get; set; }
        public string HtmlPositionType { get; set; }
    }
}
