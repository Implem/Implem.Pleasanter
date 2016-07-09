using System;
namespace Implem.Pleasanter.Libraries.Server
{
    public class SiteMenuElement
    {
        public int TenantId;
        public long SiteId;
        public string ReferenceType;
        public long ParentId;
        public string Title;
        public DateTime CreatedTime;

        public SiteMenuElement(
            int tenantId,
            long siteId,
            string referenceType,
            long parentId,
            string title)
        {
            TenantId = tenantId;
            SiteId = siteId;
            ReferenceType = referenceType;
            ParentId = parentId;
            Title = title;
            CreatedTime = DateTime.Now;
        }
    }
}