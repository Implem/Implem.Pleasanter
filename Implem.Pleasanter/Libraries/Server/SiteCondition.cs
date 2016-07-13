using System;
namespace Implem.Pleasanter.Libraries.Server
{
    public class SiteCondition
    {
        public long SiteId;
        public long ItemCount;
        public long OverdueCount;
        public DateTime UpdatedTime;

        public SiteCondition(long siteId, long itemCount, long overdueCount, DateTime updatedTime)
        {
            SiteId = siteId;
            ItemCount = itemCount;
            OverdueCount = overdueCount;
            UpdatedTime = updatedTime;
        }
    }
}