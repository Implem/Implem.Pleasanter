using Implem.Pleasanter.Libraries.Converts;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
namespace Implem.Pleasanter.Models
{
    public class SiteSubset
    {
        public int TenantId;
        public long SiteId;
        public Time UpdatedTime;
        public int Ver;
        public Title Title;
        public string Body;
        public TitleBody TitleBody;
        public string ReferenceType;
        public long ParentId;
        public long InheritPermission;
        public Permissions.Types PermissionType;
        public SiteSettings SiteSettings;
        public SiteCollection Ancestors;
        public PermissionCollection PermissionSourceCollection;
        public PermissionCollection PermissionDestinationCollection;
        public int SiteMenu;
        public List<string> MonitorChangesColumns;
        public List<string> TitleColumns;
        public Comments Comments;
        public User Creator;
        public User Updator;
        public Time CreatedTime;
        public bool VerUp;
        public string Timestamp;

        public SiteSubset()
        {
        }

        public SiteSubset(SiteModel siteModel, SiteSettings siteSettings)
        {
            TenantId = siteModel.TenantId;
            SiteId = siteModel.SiteId;
            UpdatedTime = siteModel.UpdatedTime;
            Ver = siteModel.Ver;
            Title = siteModel.Title;
            Body = siteModel.Body;
            TitleBody = siteModel.TitleBody;
            ReferenceType = siteModel.ReferenceType;
            ParentId = siteModel.ParentId;
            InheritPermission = siteModel.InheritPermission;
            PermissionType = siteModel.PermissionType;
            SiteSettings = siteModel.SiteSettings;
            Ancestors = siteModel.Ancestors;
            PermissionSourceCollection = siteModel.PermissionSourceCollection;
            PermissionDestinationCollection = siteModel.PermissionDestinationCollection;
            SiteMenu = siteModel.SiteMenu;
            MonitorChangesColumns = siteModel.MonitorChangesColumns;
            TitleColumns = siteModel.TitleColumns;
            Comments = siteModel.Comments;
            Creator = siteModel.Creator;
            Updator = siteModel.Updator;
            CreatedTime = siteModel.CreatedTime;
            VerUp = siteModel.VerUp;
            Timestamp = siteModel.Timestamp;
        }

        public Dictionary<string, int> SearchIndexCollection()
        {
            var searchIndexHash = new Dictionary<string, int>();
            SiteInfo.SiteMenu.Breadcrumb(SiteId).SearchIndexes(searchIndexHash, 100);
            UpdatedTime.SearchIndexes(searchIndexHash, 200);
            Title.SearchIndexes(searchIndexHash, 4);
            Body.SearchIndexes(searchIndexHash, 200);
            Comments.SearchIndexes(searchIndexHash, 200);
            Creator.SearchIndexes(searchIndexHash, 100);
            Updator.SearchIndexes(searchIndexHash, 100);
            CreatedTime.SearchIndexes(searchIndexHash, 200);
            SearchIndexExtensions.OutgoingMailsSearchIndexes(searchIndexHash, "Sites", SiteId);
            return searchIndexHash;
        }
    }
}
