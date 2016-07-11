using Implem.Pleasanter.Libraries.Converts;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
namespace Implem.Pleasanter.Models
{
    public class WikiSubset
    {
        public long SiteId;
        public string SiteId_LabelText;
        public Time UpdatedTime;
        public string UpdatedTime_LabelText;
        public long WikiId;
        public string WikiId_LabelText;
        public int Ver;
        public string Ver_LabelText;
        public Title Title;
        public string Title_LabelText;
        public string Body;
        public string Body_LabelText;
        public TitleBody TitleBody;
        public string TitleBody_LabelText;
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

        public WikiSubset()
        {
        }

        public WikiSubset(WikiModel wikiModel, SiteSettings siteSettings)
        {
            SiteId = wikiModel.SiteId;
            SiteId_LabelText = siteSettings.EditorColumn("SiteId")?.LabelText;
            UpdatedTime = wikiModel.UpdatedTime;
            UpdatedTime_LabelText = siteSettings.EditorColumn("UpdatedTime")?.LabelText;
            WikiId = wikiModel.WikiId;
            WikiId_LabelText = siteSettings.EditorColumn("WikiId")?.LabelText;
            Ver = wikiModel.Ver;
            Ver_LabelText = siteSettings.EditorColumn("Ver")?.LabelText;
            Title = wikiModel.Title;
            Title_LabelText = siteSettings.EditorColumn("Title")?.LabelText;
            Body = wikiModel.Body;
            Body_LabelText = siteSettings.EditorColumn("Body")?.LabelText;
            TitleBody = wikiModel.TitleBody;
            TitleBody_LabelText = siteSettings.EditorColumn("TitleBody")?.LabelText;
            Comments = wikiModel.Comments;
            Comments_LabelText = siteSettings.EditorColumn("Comments")?.LabelText;
            Creator = wikiModel.Creator;
            Creator_LabelText = siteSettings.EditorColumn("Creator")?.LabelText;
            Updator = wikiModel.Updator;
            Updator_LabelText = siteSettings.EditorColumn("Updator")?.LabelText;
            CreatedTime = wikiModel.CreatedTime;
            CreatedTime_LabelText = siteSettings.EditorColumn("CreatedTime")?.LabelText;
            VerUp = wikiModel.VerUp;
            VerUp_LabelText = siteSettings.EditorColumn("VerUp")?.LabelText;
            Timestamp = wikiModel.Timestamp;
            Timestamp_LabelText = siteSettings.EditorColumn("Timestamp")?.LabelText;
        }

        public Dictionary<string, int> SearchIndexCollection()
        {
            var searchIndexHash = new Dictionary<string, int>();
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
