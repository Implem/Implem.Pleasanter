using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Collections.Generic;

namespace Implem.Pleasanter.Libraries.Models
{
    public class SummarySynchronizationTargets
    {
        public Dictionary<string, HashSet<long>> Forward { get; } =
            new Dictionary<string, HashSet<long>>();

        public HashSet<long> Sources { get; } = new HashSet<long>();

        public void CollectBeforeSave(
            SiteSettings ss,
            BaseItemModel model)
        {
            if (ss?.Summaries == null || model == null)
            {
                return;
            }
            ss.Summaries.ForEach(summary => AddForward(
                linkColumn: summary.LinkColumn,
                id: model.GetSavedClass(summary.LinkColumn).ToLong()));
        }

        public void CollectAfterSave(
            SiteSettings ss,
            BaseItemModel model,
            long id)
        {
            if (ss?.Summaries == null || model == null)
            {
                return;
            }
            ss.Summaries.ForEach(summary => AddForward(
                linkColumn: summary.LinkColumn,
                id: model.GetClass(summary.LinkColumn).ToLong()));
            if (id != 0)
            {
                Sources.Add(id);
            }
        }

        public HashSet<long> GetForward(string linkColumn)
        {
            if (string.IsNullOrEmpty(linkColumn))
            {
                return new HashSet<long>();
            }
            return Forward.TryGetValue(linkColumn, out var ids)
                ? ids
                : new HashSet<long>();
        }

        public HashSet<long> GetSources()
        {
            return Sources;
        }

        private void AddForward(
            string linkColumn,
            long id)
        {
            if (string.IsNullOrEmpty(linkColumn) || id == 0)
            {
                return;
            }
            if (Forward.TryGetValue(linkColumn, out var ids) == false)
            {
                ids = new HashSet<long>();
                Forward.Add(linkColumn, ids);
            }
            ids.Add(id);
        }
    }
}
