using Implem.Pleasanter.Models.Shared;
using System;
using System.Collections.Generic;

namespace Implem.Pleasanter.Models.ApiSiteSettings
{
    /// <summary>
    /// Interface <c>ICommonProperties</c> contains common properties of SiteSetingByApi and is mainly used for validator purposes
    /// </summary>    
    public interface ISiteSettingBaseProperties
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public int? Delete { get; set; }

        /// <summary>
        /// Property <c>HtmlPositionType</c> is specifically for updating the Htmls tab
        /// </summary>  
        public string HtmlPositionType { get; set; }
    }
}
