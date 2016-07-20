using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Server
{
    public class SiteMenu : Dictionary<long, SiteMenuElement>
    {
        public SiteMenuElement Get(long siteId)
        {
            if (siteId == 0)
            {
                return null;
            }
            else
            {
                if (!HasAvailableCache(siteId))
                {
                    Set(siteId);
                }
                return ContainsKey(siteId)
                    ? this[siteId]
                    : null;
            }
        }

        public void Set(long siteId)
        {
            var dataRow = SiteMenuElementDataRow(siteId);
            var siteMenuElement = new SiteMenuElement(
                dataRow["TenantId"].ToInt(),
                siteId,
                dataRow["ReferenceType"].ToString(),
                dataRow["ParentId"].ToLong(),
                dataRow["Title"].ToString());
            if (ContainsKey(siteId))
            {
                this[siteId] = siteMenuElement;
            }
            else
            {
                Add(siteId, siteMenuElement);
            }
        }

        public IEnumerable<SiteMenuElement> Breadcrumb(long siteId)
        {
            var ret = new List<SiteMenuElement>();
            if (siteId != 0)
            {
                var current = Get(siteId);
                if (current != null)
                {
                    ret.Add(current);
                    while (current.ParentId != 0)
                    {
                        current = Get(current.ParentId);
                        if (current != null)
                        {
                            ret.Add(current);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                ret.Reverse();
            }
            return ret;
        }

        public IEnumerable<SiteCondition> SiteConditions(long siteId)
        {
            var hash = ChildHash(siteId);
            var sites = Rds.ExecuteTable(statements: Rds.SelectSites(
                column: Rds.SitesColumn()
                    .SiteId()
                    .ReferenceType(),
                where: Rds.SitesWhere()
                    .TenantId(Sessions.TenantId())
                    .SiteId_In(hash.SelectMany(o => o.Value))
                    .ReferenceType("Sites", _operator: "<>")
                    .PermissionType(_operator: " & " +
                        Permissions.Types.Read.ToInt().ToString() + "<>0")))
                            .AsEnumerable();
            var issues = sites
                .Where(o => o["ReferenceType"].ToString() == "Issues")
                .Select(o => o["SiteId"].ToLong());
            var dataSet = Rds.ExecuteDataSet(statements: new SqlStatement[]
                {
                    Rds.SelectItems(
                        dataTableName: "Items",
                        column: Rds.ItemsColumn()
                            .SiteId()
                            .ItemsCount()
                            .UpdatedTimeMax(),
                        where: Rds.ItemsWhere()
                            .SiteId_In(sites.Select(o => o["SiteId"].ToLong()))
                            .ReferenceType("Sites", _operator: "<>"),
                        groupBy: Rds.SitesGroupBy()
                            .SiteId()),
                    Rds.SelectIssues(
                        dataTableName: "OverdueIssues",
                        column: Rds.IssuesColumn()
                            .SiteId()
                            .IssuesCount(_as: "OverdueCount"),
                        where: Rds.IssuesWhere()
                            .SiteId_In(issues)
                            .Status(_operator: "<{0}".Params(Parameters.General.CompletionCode))
                            .CompletionTime(_operator: "<getdate()"),
                        groupBy: Rds.SitesGroupBy()
                            .SiteId())
                });
            return hash.Select(o => SiteCondition(dataSet, o.Key, o.Value));
        }

        private SiteCondition SiteCondition(DataSet dataSet, long siteId, List<long> children)
        {
            var items = dataSet.Tables["Items"]
                .AsEnumerable()
                .Where(o => children.Contains(o["SiteId"].ToLong()));
            var overdueIssues = dataSet.Tables["OverdueIssues"]
                .AsEnumerable()
                .Where(o => children.Contains(o["SiteId"].ToLong()));
            return new SiteCondition(
                siteId,
                items.Sum(o => o["ItemsCount"].ToLong()),
                overdueIssues.Sum(o => o["OverdueCount"].ToLong()),
                items.Count() > 0
                    ? items.Max(o => o["UpdatedTimeMax"].ToDateTime())
                    : DateTime.MinValue);
        }

        public Dictionary<long, List<long>> ChildHash(long siteId)
        {
            var ret = new Dictionary<long, List<long>>();
            this.Select(o => o.Value)
                .Where(o => o.TenantId == Sessions.TenantId())
                .Where(o => o.ParentId == siteId)
                .ForEach(child =>
                    ret.Add(child.SiteId, Children(child.SiteId)));
            return ret;
        }

        public List<long> Children(long siteId, List<long> data = null)
        {
            if (data == null) data = new List<long>();
            data.Add(siteId);
            this.Select(o => o.Value)
                .Where(o => o.TenantId == Sessions.TenantId())
                .Where(o => o.ParentId == siteId)
                .ForEach(child =>
                    Children(child.SiteId, data));
            return data;
        }

        private bool HasAvailableCache(long siteId)
        {
            return
                ContainsKey(siteId) &&
                (BackgroundTasks.Enabled() ||
                (DateTime.Now - this[siteId].CreatedTime).Milliseconds <
                    Parameters.Cache.SiteMenuAvailableTime);
        }

        private DataRow SiteMenuElementDataRow(long siteId)
        {
            var tenantId = Sessions.TenantId();
            return Rds.ExecuteTable(statements: Rds.SelectSites(
                column: Rds.SitesColumn()
                    .TenantId()
                    .ReferenceType()
                    .ParentId()
                    .Title(),
                where: Rds.SitesWhere()
                    .TenantId(tenantId, _using: tenantId != 0)
                    .SiteId(siteId)))
                        .AsEnumerable()
                        .FirstOrDefault();
        }
    }
}