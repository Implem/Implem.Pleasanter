using Implem.Pleasanter.Libraries.Converts;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
namespace Implem.Pleasanter.Models
{
    public class WikiSubset
    {
        public long SiteId;
        public Time UpdatedTime;
        public long WikiId;
        public Title Title;
        public string Body;
        public Comments Comments;
        public User Creator;
        public User Updator;
        public Time CreatedTime;

        public WikiSubset()
        {
        }

        public WikiSubset(WikiModel wikiModel, SiteSettings siteSettings)
        {
            SiteId = wikiModel.SiteId;
            UpdatedTime = wikiModel.UpdatedTime;
            WikiId = wikiModel.WikiId;
            Title = wikiModel.Title;
            Body = wikiModel.Body;
            Comments = wikiModel.Comments;
            Creator = wikiModel.Creator;
            Updator = wikiModel.Updator;
            CreatedTime = wikiModel.CreatedTime;
        }

        public Dictionary<string, int> SearchIndexCollection()
        {
            var searchIndexHash = new Dictionary<string, int>();
            SiteInfo.SiteMenu.Breadcrumb(SiteId).SearchIndexes(searchIndexHash, 100);
            SiteId.SearchIndexes(searchIndexHash, 200);
            UpdatedTime.SearchIndexes(searchIndexHash, 200);
            WikiId.SearchIndexes(searchIndexHash, 1);
            Title.SearchIndexes(searchIndexHash, 4);
            Body.SearchIndexes(searchIndexHash, 200);
            Comments.SearchIndexes(searchIndexHash, 200);
            Creator.SearchIndexes(searchIndexHash, 100);
            Updator.SearchIndexes(searchIndexHash, 100);
            CreatedTime.SearchIndexes(searchIndexHash, 200);
            SearchIndexExtensions.OutgoingMailsSearchIndexes(searchIndexHash, "Wikis", WikiId);
            return searchIndexHash;
        }
    }
}
