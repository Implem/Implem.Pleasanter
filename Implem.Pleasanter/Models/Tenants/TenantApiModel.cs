using System;
namespace Implem.Pleasanter.Models
{
    [Serializable]
    public class TenantApiModel
    {
        public int? TenantId;
        public int? Ver;
        public string TenantName;
        public string Title;
        public string Body;
        public string ContractSettings;
        public DateTime? ContractDeadline;
        public int? LogoType;
        public string HtmlTitleTop;
        public string HtmlTitleSite;
        public string HtmlTitleRecord;
        public string Comments;
        public int? Creator;
        public int? Updator;
        public DateTime? CreatedTime;
        public DateTime? UpdatedTime;
        public bool? VerUp;
    }
}
