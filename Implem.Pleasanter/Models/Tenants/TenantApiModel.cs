using Implem.Pleasanter.Models.Shared;
using System;
namespace Implem.Pleasanter.Models
{
    [Serializable]
    public class TenantApiModel : _BaseApiModel
    {
        public int? TenantId { get; set; }
        public int? Ver { get; set; }
        public string TenantName { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string ContractSettings { get; set; }
        public DateTime? ContractDeadline { get; set; }
        public bool? DisableAllUsersPermission { get; set; }
        public bool? DisableStartGuide { get; set; }
        public int? LogoType { get; set; }
        public string HtmlTitleTop { get; set; }
        public string HtmlTitleSite { get; set; }
        public string HtmlTitleRecord { get; set; }
        public string Comments { get; set; }
        public int? Creator { get; set; }
        public int? Updator { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }

        public TenantApiModel()
        {
        }
    }
}
