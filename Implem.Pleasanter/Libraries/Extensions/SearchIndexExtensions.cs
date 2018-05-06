using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Extensions
{
    public static class SearchIndexExtensions
    {
        public static void SearchIndexes(
            this int self, Dictionary<string, int> searchIndexHash, int searchPriority)
        {
            Update(searchIndexHash, self.ToString(), searchPriority);
        }

        public static void SearchIndexes(
            this long self, Dictionary<string, int> searchIndexHash, int searchPriority)
        {
            Update(searchIndexHash, self.ToString(), searchPriority);
        }

        public static void SearchIndexes(
            this decimal self, Dictionary<string, int> searchIndexHash, int searchPriority)
        {
            Update(searchIndexHash, self.ToString(), searchPriority);
        }

        public static void SearchIndexes(
            this DateTime self, Dictionary<string, int> searchIndexHash, int searchPriority)
        {
            Update(searchIndexHash, self.ToLocal().ToString(), searchPriority);
        }

        public static void SearchIndexes(
            this string self, Dictionary<string, int> searchIndexHash, int searchPriority)
        {
            SearchIndexes(searchIndexHash, self, searchPriority);
        }

        public static void SearchIndexes(
            this string self, Column column, Dictionary<string, int> searchIndexHash, int searchPriority)
        {
            if (column?.HasChoices() == true)
            {
                SearchIndexes(searchIndexHash, column.Choice(self).SearchText(), searchPriority);
            }
            else
            {
                SearchIndexes(searchIndexHash, self, searchPriority);
            }
        }

        public static void SearchIndexes(
            this IEnumerable<SiteMenuElement> self,
            Dictionary<string, int> searchIndexHash,
            int searchPriority)
        {
            SearchIndexes(searchIndexHash, self.Select(o => o.Title).Join(" "), searchPriority);
        }

        public static void SearchIndexes(
            this ProgressRate self, Dictionary<string, int> searchIndexHash, int searchPriority)
        {
            SearchIndexes(searchIndexHash, self.Value.ToString(), searchPriority);
        }

        public static void SearchIndexes(
            this Status self,
            Column column,
            Dictionary<string, int> searchIndexHash,
            int searchPriority)
        {
            if (column?.HasChoices() == true)
            {
                SearchIndexes(
                    searchIndexHash,
                    column.Choice(self.Value.ToString()).SearchText(),
                    searchPriority);
            }
            else
            {
                SearchIndexes(searchIndexHash, self.Value.ToString(), searchPriority);
            }
        }

        public static void SearchIndexes(
            this Time self, Dictionary<string, int> searchIndexHash, int searchPriority)
        {
            SearchIndexes(searchIndexHash, self.Value.ToLocal().ToString(), searchPriority);
        }

        public static void SearchIndexes(
            this Title self, Dictionary<string, int> searchIndexHash, int searchPriority)
        {
            SearchIndexes(searchIndexHash, self.Value, searchPriority);
        }

        public static void SearchIndexes(
            this User self, Dictionary<string, int> searchIndexHash, int searchPriority)
        {
            SearchIndexes(searchIndexHash, self.Name, searchPriority);
        }

        public static void SearchIndexes(
            this Comments self, Dictionary<string, int> searchIndexHash, int searchPriority)
        {
            SearchIndexes(
                searchIndexHash,
                self.Select(o => SiteInfo.UserName(o.Creator) + " " + o.Body).Join(" "),
                searchPriority);
        }

        public static void SearchIndexes(
            this WorkValue self, Dictionary<string, int> searchIndexHash, int searchPriority)
        {
            SearchIndexes(searchIndexHash, self.Value.ToString(), searchPriority);
        }

        public static void SearchIndexes(
            this Attachments self, Dictionary<string, int> searchIndexHash, int searchPriority)
        {
            SearchIndexes(
                searchIndexHash,
                self.Select(o => o.Name).Join(" "),
                searchPriority);
        }

        public static void OutgoingMailsSearchIndexes(
            Dictionary<string, int> searchIndexHash,
            string referenceType,
            long referenceId)
        {
            new OutgoingMailCollection(where: Rds.OutgoingMailsWhere()
                .ReferenceType(referenceType)
                .ReferenceId(referenceId)).Select(o => " ".JoinParam(
                    o.From.ToString(),
                    o.To,
                    o.Cc,
                    o.Bcc,
                    o.Title.Value,
                    o.Body)).Join(" ")
                        .SearchIndexes(createIndex: true)
                        .Distinct()
                        .ForEach(searchIndex =>
                           Update(searchIndexHash, searchIndex, 300));
        }

        private static void SearchIndexes(
            Dictionary<string, int> searchIndexHash,
            string text,
            int searchPriority)
        {
            SearchIndexes(text, createIndex: true).ForEach(searchIndex =>
                Update(searchIndexHash, searchIndex, searchPriority));
        }

        private static void Update(
            Dictionary<string, int> searchIndexHash, string searchIndex, int searchPriority)
        {
            var word = searchIndex.ToLower().Trim();
            word = CSharp.Japanese.Kanaxs.KanaEx.ToHankaku(word);
            word = CSharp.Japanese.Kanaxs.KanaEx.ToZenkakuKana(word);
            if (word != string.Empty)
            {
                if (!searchIndexHash.ContainsKey(word))
                {
                    searchIndexHash.Add(word, searchPriority);
                }
                else if (searchIndexHash[word] > searchPriority)
                {
                    searchIndexHash[word] = searchPriority;
                }
            }
        }

        public static IEnumerable<string> SearchIndexes(this string self, bool createIndex = false)
        {
            return new Search.WordBreaker(self, createIndex).Results
                .Select(o => o.Trim().ToLower())
                .Where(o => o != string.Empty)
                .Distinct()
                .ToList();
        }
    }
}