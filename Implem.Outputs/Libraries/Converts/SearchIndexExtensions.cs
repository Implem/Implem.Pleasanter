using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.ServerData;
using Implem.Pleasanter.Libraries.Utilities;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Converts
{
    public static class SearchIndexExtensions
    {
        public static void SearchIndexes(
            this int self, ref Dictionary<string, int> searchIndexHash, int searchPriority)
        {
            Update(searchIndexHash, self.ToString(), searchPriority);
        }

        public static void SearchIndexes(
            this long self, ref Dictionary<string, int> searchIndexHash, int searchPriority)
        {
            Update(searchIndexHash, self.ToString(), searchPriority);
        }

        public static void SearchIndexes(
            this string self, ref Dictionary<string, int> searchIndexHash, int searchPriority)
        {
            SearchIndexes(searchIndexHash, self, searchPriority);
        }

        public static void SearchIndexes(
            this Title self, ref Dictionary<string, int> searchIndexHash, int searchPriority)
        {
            SearchIndexes(searchIndexHash, self.Value, searchPriority);
        }

        public static void SearchIndexes(
            this User self, ref Dictionary<string, int> searchIndexHash, int searchPriority)
        {
            SearchIndexes(searchIndexHash, self.FullName, searchPriority);
        }

        public static void SearchIndexes(
            this Comments self, ref Dictionary<string, int> searchIndexHash, int searchPriority)
        {
            SearchIndexes(
                searchIndexHash,
                self.Select(o => SiteInfo.UserFullName(o.Creator) + " " + o.Body).Join(" "),
                searchPriority);
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
            return new WordBreaker(self, createIndex).Results
                .Select(o => o.Trim())
                .Where(o => o != string.Empty)
                .Distinct();
        }
    }
}