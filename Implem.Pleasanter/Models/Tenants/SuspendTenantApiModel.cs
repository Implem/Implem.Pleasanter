using System;
namespace Implem.Pleasanter.Models
{
    [Serializable]
    public class SuspendTenantApiModel
    {
        public string ApiKey { get; set; }
        public DateTime? SuspendDate { get; set; }
    }
}
