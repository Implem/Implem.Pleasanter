using System;
namespace Implem.Pleasanter.Models
{
    [Serializable]
    public class CreateTenantApiModel
    {
        public string ApiKey { get; set; }
        public string TenantName { get; set; }
        public string Title { get; set; }
        public string LoginId { get; set; }
        public string Name { get; set; }
        public string NotifyMailAddress { get; set; }
        public string Language { get; set; }
    }
}
