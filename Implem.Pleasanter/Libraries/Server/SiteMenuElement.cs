﻿using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using System;
namespace Implem.Pleasanter.Libraries.Server
{
    public class SiteMenuElement
    {
        public int TenantId;
        public long SiteId;
        public string ReferenceType;
        public long ParentId;
        public long OnlyOneChildId;
        public string Title;
        public DateTime CreatedTime;

        public SiteMenuElement(
            Context context,
            long siteId,
            string referenceType,
            long parentId,
            string title)
        {
            TenantId = context.TenantId;
            SiteId = siteId;
            ReferenceType = referenceType;
            ParentId = parentId;
            Title = title;
            CreatedTime = DateTime.Now;
            if (HasOnlyOneChild())
            {
                OnlyOneChildId = Rds.ExecuteScalar_long(
                    context: context,
                    statements: Rds.SelectWikis(
                        column: Rds.WikisColumn().WikiId(),
                        where: Rds.WikisWhere().SiteId(siteId)));
            }
        }

        public bool HasOnlyOneChild()
        {
            switch (ReferenceType)
            {
                case "Wikis": return true;
                default: return false;
            }
        }
    }
}