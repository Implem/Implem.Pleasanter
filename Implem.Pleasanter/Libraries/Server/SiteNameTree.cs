using Implem.Libraries.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Implem.Pleasanter.Libraries.Server
{
    public class SiteNameTree
    {
        public readonly List<SiteNameTreeItem> Items = new List<SiteNameTreeItem>();
        public readonly Dictionary<string, List<long>> Names = new Dictionary<string, List<long>>();
        public readonly List<(string name, long triggerId, long nameId, long ticks)> Caches = new List<(string, long, long, long)>();

        public SiteNameTree(Dictionary<long, DataRow> sites)
        {
            foreach (var site in sites)
            {
                AddNode(site.Value.String("ReferenceType"), site.Key, site.Value.Long("ParentId"), site.Value.String("SiteName"));
            }
            Items.Sort();
        }

        private void AddNode(string type, long id, long parentId, string name)
        {
            if (name != null)
            {
                if (Names.ContainsKey(name) == false)
                {
                    Names.Add(name, new List<long>());
                }
                Names[name].Add(id);
            }
            Items.Add(new SiteNameTreeItem(
                id: id,
                parentId: parentId,
                type: type));
        }

        public long Find(long startId, string name)
        {
            if (Names.ContainsKey(name) == false) return -1;
            var nameIdList = Names[name];
            if (nameIdList.Count == 1) return nameIdList[0];
            var cacheNode = FindCaches(
                name: name,
                triggerId: startId);
            if (cacheNode.name != null)
            {
                cacheNode.ticks = DateTime.UtcNow.Ticks;
                return cacheNode.nameId;
            }
            var triggerNode = SearchItem(startId: startId);
            if (triggerNode == null) return -1;
            var foundNode = FindSameLevel(
                nameIdList: nameIdList,
                folderId: triggerNode.ParentId,
                id: triggerNode.Id,
                isUp: false);
            if (Caches.Count > 1024)
            {
                Caches.RemoveAt(Caches.Select((v, index) => (v, index)).MinBy(v => v.v.ticks).index);
            }
            Caches.Add(new(name, startId, foundNode?.Id ?? -1, DateTime.UtcNow.Ticks));
            return foundNode?.Id ?? -1;
        }

        private (string name, long triggerId, long nameId, long ticks) FindCaches(string name, long triggerId)
        {
            return Caches.FirstOrDefault(v => v.name == name && v.triggerId == triggerId);
        }

        private SiteNameTreeItem SearchItem(long startId)
        {
            var idx = Items.BinarySearch(SiteNameTreeItem.SearchKey(id: startId));
            return (idx >= 0) ? Items[idx] : null;
        }

        private SiteNameTreeItem FindSameLevel(List<long> nameIdList, long folderId, long id, bool isUp)
        {
            var sameLevel = Items.Where(v => v.ParentId == folderId).OrderBy((p) => p.Id == id ? -1 : p.Id).ToArray();
            var foundNode = sameLevel.FirstOrDefault(v1 => nameIdList.Any(v2 => v2 == v1.Id));
            if (foundNode != null)
            {
                return foundNode;
            }
            foreach (var item in sameLevel.Where(v => v.Type == SiteNameTreeItem.SiteType.Folder))
            {
                if (id == item.Id && isUp) continue;
                var sameFolder = FindSameLevel(
                    nameIdList: nameIdList,
                    folderId: item.Id,
                    id: -1,
                    isUp: false);
                if (sameFolder != null) return sameFolder;
            }
            if (!isUp && id == -1) return null;
            var parentNode = SearchItem(startId: folderId);
            return (parentNode != null)
                    ? FindSameLevel(
                        nameIdList: nameIdList,
                        folderId: parentNode.ParentId,
                        id: parentNode.Id,
                        isUp: true)
                    : null;
        }
    }

    public class SiteNameTreeItem : IComparable<SiteNameTreeItem>
    {
        public enum SiteType : short
        {
            Folder,
            Issues,
            Results,
            Wiki,
            Dashboard
        }
        public readonly long Id;
        public readonly long ParentId;
        public readonly SiteType Type;

        public SiteNameTreeItem(long id, long parentId, string type)
        {
            Type = GetSiteType(type);
            Id = id;
            ParentId = parentId;
        }

        public SiteNameTreeItem(long id, long parentId, SiteType type)
        {
            Type = type;
            Id = id;
            ParentId = parentId;
        }

        public static SiteType GetSiteType(string str)
        {
            switch (str)
            {
                case "Dashboards": return SiteType.Dashboard;
                case "Issues": return SiteType.Issues;
                case "Results": return SiteType.Results;
                case "Sites": return SiteType.Folder;
                case "Wikis": return SiteType.Wiki;
            }
            return SiteType.Issues;
        }

        public static SiteNameTreeItem SearchKey(long id)
        {
            return new SiteNameTreeItem(
                id: id,
                parentId: 0,
                type: SiteType.Results);
        }

        int IComparable<SiteNameTreeItem>.CompareTo(SiteNameTreeItem other)
        {
            return Id.CompareTo(other?.Id);
        }

        public override bool Equals(object obj)
        {
            return obj is SiteNameTreeItem item
                && Id == item.Id
                && ParentId == item.ParentId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, ParentId);
        }
    }
}