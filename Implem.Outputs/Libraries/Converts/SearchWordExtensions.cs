using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.ServerData;
using Implem.Pleasanter.Libraries.Utilities;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Converts
{
    public static class SearchWordExtensions
    {
        public static void SearchWords(
            this int self, ref Dictionary<string, int> searchWordHash, int searchPriority)
        {
            Update(searchWordHash, self.ToString(), searchPriority);
        }

        public static void SearchWords(
            this long self, ref Dictionary<string, int> searchWordHash, int searchPriority)
        {
            Update(searchWordHash, self.ToString(), searchPriority);
        }

        public static void SearchWords(
            this string self, ref Dictionary<string, int> searchWordHash, int searchPriority)
        {
            SearchWords(searchWordHash, self, searchPriority);
        }

        public static void SearchWords(
            this Title self, ref Dictionary<string, int> searchWordHash, int searchPriority)
        {
            SearchWords(searchWordHash, self.Value, searchPriority);
        }

        public static void SearchWords(
            this User self, ref Dictionary<string, int> searchWordHash, int searchPriority)
        {
            SearchWords(searchWordHash, self.FullName, searchPriority);
        }

        public static void SearchWords(
            this Comments self, ref Dictionary<string, int> searchWordHash, int searchPriority)
        {
            SearchWords(
                searchWordHash,
                self.Select(o => SiteInfo.UserFullName(o.Creator) + " " + o.Body).Join(" "),
                searchPriority);
        }

        private static void SearchWords(
            Dictionary<string, int> searchWordHash,
            string text,
            int searchPriority)
        {
            SearchWords(text, createIndex: true).ForEach(searchWord =>
                Update(searchWordHash, searchWord, searchPriority));
        }

        private static void Update(
            Dictionary<string, int> searchWordHash, string searchWord, int searchPriority)
        {
            var word = searchWord.ToLower().Trim();
            word = CSharp.Japanese.Kanaxs.KanaEx.ToHankaku(word);
            word = CSharp.Japanese.Kanaxs.KanaEx.ToZenkakuKana(word);
            if (word != string.Empty)
            {
                if (!searchWordHash.ContainsKey(word))
                {
                    searchWordHash.Add(word, searchPriority);
                }
                else if (searchWordHash[word] > searchPriority)
                {
                    searchWordHash[word] = searchPriority;
                }
            }
        }

        public static IEnumerable<string> SearchWords(this string self, bool createIndex = false)
        {
            return new WordBreaker(self, createIndex).Results
                .Select(o => o.Trim())
                .Where(o => o != string.Empty)
                .Distinct();
        }
    }
}