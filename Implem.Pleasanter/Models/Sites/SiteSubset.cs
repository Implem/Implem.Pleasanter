using Implem.Pleasanter.Libraries.Converts;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
namespace Implem.Pleasanter.Models
{
    public class SiteSubset
    {
        public long SiteId;
        public Time UpdatedTime;
        public Title Title;
        public string Body;
        public Comments Comments;
        public User Creator;
        public User Updator;
        public Time CreatedTime;

        public SiteSubset()
        {
        }

        public SiteSubset(SiteModel siteModel, SiteSettings siteSettings)
        {
            SiteId = siteModel.SiteId;
            UpdatedTime = siteModel.UpdatedTime;
            Title = siteModel.Title;
            Body = siteModel.Body;
            Comments = siteModel.Comments;
            Creator = siteModel.Creator;
            Updator = siteModel.Updator;
            CreatedTime = siteModel.CreatedTime;
        }

        public Dictionary<string, int> SearchIndexCollection()
        {
            var searchIndexHash = new Dictionary<string, int>();
            SiteInfo.SiteMenu.Breadcrumb(SiteId).SearchIndexes(searchIndexHash, 100);
            SiteId.SearchIndexes(searchIndexHash, 1);
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
