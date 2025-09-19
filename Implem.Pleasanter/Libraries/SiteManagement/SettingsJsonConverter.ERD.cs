using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Implem.Pleasanter.Libraries.SiteManagement
{
    public partial class SettingsJsonConverter
    {
        private static EntityRelationshipDiagramsData ConvertERDiagrams(
            Context context,
            Param param)
        {
            var siteIds = EntityRelationshipDiagramsData.GetChainLinkIds(
                context: context,
                param: param);
            var erdData = new EntityRelationshipDiagramsData();
            erdData.Tables = EntityRelationshipDiagramsData.ConvertTables(
                context: context,
                param: param,
                siteIds: siteIds);
            erdData.Info.Logs = param.Logs.Count == 0 ? null : param.Logs;
            return erdData;
        }

        public class EntityRelationshipDiagramsInfoData
        {
            public List<Log> Logs = new();
        }

        public class EntityRelationshipDiagramsData
        {
            public class Table
            {
                public long SiteId;
                public int Ver;
                public string Title;
                public string ReferenceType;
                public class Column
                {
                    public string Label;
                    public string Type;
                    public string TypeRaw;
                    public long? RelationSiteId;
                }
                public List<Column> Columns = new();
            }
            public EntityRelationshipDiagramsInfoData Info = new();
            public List<Table> Tables;

            internal static IEnumerable<long> GetChainLinkIds(
                Context context,
                Param param)
            {
                var initialSiteIds = param.SelectedSites
                    .Select(v => v.SiteId)
                    .Distinct();
                var selectedTenantSiteIds = SelectTenantSiteIds(
                    context: context,
                    siteIds: initialSiteIds);
                var chainLinkSiteIds = GetRecursiveLinkIds(
                    context: context,
                    param: param,
                    siteIds: selectedTenantSiteIds,
                    depth: param.ErdLinkDepth,
                    limit: param.ErdLinkLimit);
                var resultTenantSiteIds = SelectTenantSiteIds(
                    context: context,
                    siteIds: chainLinkSiteIds);
                return resultTenantSiteIds;
            }

            private static IEnumerable<long> SelectTenantSiteIds(
                Context context,
                IEnumerable<long> siteIds)
            {
                return Repository.ExecuteTable(
                    context: context,
                    statements: Rds.SelectSites(
                        column: Rds.SitesColumn()
                            .Sites_SiteId(),
                        where: Rds.SitesWhere()
                            .TenantId(context.TenantId)
                            .SiteId_In(siteIds)))
                    .AsEnumerable()
                    .Select(dataRow => dataRow.Long("SiteId"))
                    .ToArray();
            }

            private static IEnumerable<long> GetRecursiveLinkIds(
                Context context,
                Param param,
                IEnumerable<long> siteIds,
                int depth,
                int limit)
            {
                var visited = new HashSet<long>(siteIds);
                var current = new List<long>(siteIds);
                for (var depthCnt = 1; depthCnt <= (depth + 1); depthCnt++)
                {
                    if (current.Count == 0) break;
                    if (depthCnt == depth)
                    {
                        param.Logs.Add(new Log(Log.LogLevel.Error, "ConvertERD", $"Recursive Search Depth Exceeded.({depth})"));
                        break;
                    }
                    if (visited.Count >= limit)
                    {
                        param.Logs.Add(new Log(Log.LogLevel.Error, "ConvertERD", $"Recursive Search Count Exceeded.({limit})"));
                        break;
                    }
                    var response = Repository.ExecuteTable(
                        context: context,
                        statements: new SqlStatement[]
                        {
                            Rds.SelectLinks(
                                column: Rds.LinksColumn()
                                    .DestinationId(_as: "SiteId"),
                                where: Rds.LinksWhere()
                                    .SourceId_In(current)),
                            Rds.SelectLinks(
                                unionType: Sqls.UnionTypes.Union,
                                column: Rds.LinksColumn()
                                    .SourceId(_as: "SiteId"),
                                where: Rds.LinksWhere()
                                    .DestinationId_In(current)),
                        })
                            .AsEnumerable()
                            .Select(dataRow => dataRow.Long("SiteId"));
                    current.Clear();
                    foreach (var id in response)
                    {
                        if (!visited.Contains(id))
                        {
                            visited.Add(id);
                            current.Add(id);
                        }
                    }
                }
                return visited.ToArray();
            }

            internal static List<Table> ConvertTables(
                Context context,
                Param param,
                IEnumerable<long> siteIds)
            {
                var userSelectedSiteIds = param.SelectedSites
                    .Select(v => v.SiteId)
                    .Distinct();
                var siteModels = new Dictionary<long, SiteModel>();
                var linkPairs = new List<(long id1, long id2)>();
                foreach (var siteId in siteIds)
                {
                    var siteModel = new SiteModel()
                        .Get(
                            context: context,
                            where: DataSources.Rds.SitesWhere()
                                .TenantId(context.TenantId)
                                .SiteId(siteId)
                                .Or(or: Rds.SitesWhere()
                                    .ReferenceType(raw: "'Issues'")
                                    .ReferenceType(raw: "'Results'")));
                    if (siteModel.SiteId == 0)
                    {
                        if (userSelectedSiteIds.Any(v => v == siteId))
                        {
                            param.Logs.Add(new Log(Log.LogLevel.Error, "ConvertERD", $"SiteId {siteId} was not found."));
                        }
                        continue;
                    }
                    var ss = siteModel.SiteSettings;
                    if (context.CanManageSite(ss: ss) == false)
                    {
                        if (userSelectedSiteIds.Any(v => v == siteId))
                        {
                            param.Logs.Add(new Log(Log.LogLevel.Error, "ConvertERD", $"SiteId {siteId} was access denied."));
                        }
                        continue;
                    }
                    siteModels.Add(siteId, siteModel);
                    linkPairs.AddRange(ss.Links?.Select(v => (id1: siteId, id2: v.SiteId)) ?? new List<(long id1, long id2)>());
                    linkPairs.AddRange(ss.Links?.Select(v => (id1: v.SiteId, id2: siteId)) ?? new List<(long id1, long id2)>());
                }
                var current = new List<long>(userSelectedSiteIds);
                var visited = new List<long>();
                while (current.Count != 0)
                {
                    visited.AddRange(current);
                    var next = new List<long>();
                    foreach (var siteId in current)
                    {
                        next.AddRange(linkPairs.Where(v => v.id1 == siteId)
                            .Select(v => v.id2)
                            .Where(v => !visited.Contains(v))
                            .Where(v => siteModels.ContainsKey(v)));
                    }
                    current = next.Distinct().ToList();
                }
                var result = new List<Table>();
                foreach (var siteModel in siteModels.Where(v => visited.Any(v2 => v.Key == v2)).Select(v => v.Value))
                {
                    Table table = null;
                    try
                    {
                        var ss = siteModel.SiteSettings;
                        ss.Update_ColumnAccessControls();
                        table = new();
                        table.SiteId = siteModel.SiteId;
                        table.Ver = siteModel.Ver;
                        table.Title = siteModel.Title.DisplayValue.IsNotEmpty();
                        table.ReferenceType = SiteUtilities.ReferenceTypeDisplayName(
                            context: context,
                            referenceType: siteModel.ReferenceType);
                        foreach (var edit in ss.EditorColumnHash
                            .SelectMany(v => v.Value)
                            .Where(v => !v.StartsWith("_")))
                        {
                            var column = ss.ColumnHash[edit];
                            var c = new Table.Column();
                            c.TypeRaw = edit;
                            c.Type = column.LabelTextDefault;
                            c.Label = column.LabelText;
                            var link = ss.Links
                                .Where(v => v.ColumnName == column.ColumnName)
                                .FirstOrDefault();
                            if (link != null && visited.Any(v => v == link.SiteId))
                            {
                                c.RelationSiteId = link.SiteId;
                            }
                            table.Columns.Add(c);
                        }
                    }
                    catch (Exception e)
                    {
                        var item = new Log(Log.LogLevel.Error, "ConvertERD", $"SiteId {siteModel.SiteId} conversion failed. {e.Message}");
                        param.Logs.Add(item);
                    }
                    finally
                    {
                        if (table != null)
                        {
                            result.Add(table);
                        }
                    }
                }
                return result.Any() ? result : null;
            }
        }
    }
}
