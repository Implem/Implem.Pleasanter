using Implem.Pleasanter.Libraries.Converts;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
namespace Implem.Pleasanter.Models
{
    public class SiteSubset
    {
        public int TenantId;
        public string TenantId_LabelText;
        public long SiteId;
        public string SiteId_LabelText;
        public Time UpdatedTime;
        public string UpdatedTime_LabelText;
        public int Ver;
        public string Ver_LabelText;
        public Title Title;
        public string Title_LabelText;
        public string Body;
        public string Body_LabelText;
        public TitleBody TitleBody;
        public string TitleBody_LabelText;
        public string ReferenceType;
        public string ReferenceType_LabelText;
        public long ParentId;
        public string ParentId_LabelText;
        public long InheritPermission;
        public string InheritPermission_LabelText;
        public Permissions.Types PermissionType;
        public string PermissionType_LabelText;
        public SiteSettings SiteSettings;
        public string SiteSettings_LabelText;
        public SiteCollection Ancestors;
        public string Ancestors_LabelText;
        public PermissionCollection PermissionSourceCollection;
        public string PermissionSourceCollection_LabelText;
        public PermissionCollection PermissionDestinationCollection;
        public string PermissionDestinationCollection_LabelText;
        public int SiteMenu;
        public string SiteMenu_LabelText;
        public Comments Comments;
        public string Comments_LabelText;
        public User Creator;
        public string Creator_LabelText;
        public User Updator;
        public string Updator_LabelText;
        public Time CreatedTime;
        public string CreatedTime_LabelText;
        public bool VerUp;
        public string VerUp_LabelText;
        public string Timestamp;
        public string Timestamp_LabelText;

        public SiteSubset()
        {
        }

        public SiteSubset(SiteModel siteModel, SiteSettings siteSettings)
        {
            TenantId = siteModel.TenantId;
            TenantId_LabelText = siteSettings.EditorColumn("TenantId")?.LabelText;
            SiteId = siteModel.SiteId;
            SiteId_LabelText = siteSettings.EditorColumn("SiteId")?.LabelText;
            UpdatedTime = siteModel.UpdatedTime;
            UpdatedTime_LabelText = siteSettings.EditorColumn("UpdatedTime")?.LabelText;
            Ver = siteModel.Ver;
            Ver_LabelText = siteSettings.EditorColumn("Ver")?.LabelText;
            Title = siteModel.Title;
            Title_LabelText = siteSettings.EditorColumn("Title")?.LabelText;
            Body = siteModel.Body;
            Body_LabelText = siteSettings.EditorColumn("Body")?.LabelText;
            TitleBody = siteModel.TitleBody;
            TitleBody_LabelText = siteSettings.EditorColumn("TitleBody")?.LabelText;
            ReferenceType = siteModel.ReferenceType;
            ReferenceType_LabelText = siteSettings.EditorColumn("ReferenceType")?.LabelText;
            ParentId = siteModel.ParentId;
            ParentId_LabelText = siteSettings.EditorColumn("ParentId")?.LabelText;
            InheritPermission = siteModel.InheritPermission;
            InheritPermission_LabelText = siteSettings.EditorColumn("InheritPermission")?.LabelText;
            PermissionType = siteModel.PermissionType;
            PermissionType_LabelText = siteSettings.EditorColumn("PermissionType")?.LabelText;
            SiteSettings = siteModel.SiteSettings;
            SiteSettings_LabelText = siteSettings.EditorColumn("SiteSettings")?.LabelText;
            Ancestors = siteModel.Ancestors;
            Ancestors_LabelText = siteSettings.EditorColumn("Ancestors")?.LabelText;
            PermissionSourceCollection = siteModel.PermissionSourceCollection;
            PermissionSourceCollection_LabelText = siteSettings.EditorColumn("PermissionSourceCollection")?.LabelText;
            PermissionDestinationCollection = siteModel.PermissionDestinationCollection;
            PermissionDestinationCollection_LabelText = siteSettings.EditorColumn("PermissionDestinationCollection")?.LabelText;
            SiteMenu = siteModel.SiteMenu;
            SiteMenu_LabelText = siteSettings.EditorColumn("SiteMenu")?.LabelText;
            Comments = siteModel.Comments;
            Comments_LabelText = siteSettings.EditorColumn("Comments")?.LabelText;
            Creator = siteModel.Creator;
            Creator_LabelText = siteSettings.EditorColumn("Creator")?.LabelText;
            Updator = siteModel.Updator;
            Updator_LabelText = siteSettings.EditorColumn("Updator")?.LabelText;
            CreatedTime = siteModel.CreatedTime;
            CreatedTime_LabelText = siteSettings.EditorColumn("CreatedTime")?.LabelText;
            VerUp = siteModel.VerUp;
            VerUp_LabelText = siteSettings.EditorColumn("VerUp")?.LabelText;
            Timestamp = siteModel.Timestamp;
            Timestamp_LabelText = siteSettings.EditorColumn("Timestamp")?.LabelText;
        }

        public Dictionary<string, int> SearchIndexCollection()
        {
            var searchIndexHash = new Dictionary<string, int>();
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
