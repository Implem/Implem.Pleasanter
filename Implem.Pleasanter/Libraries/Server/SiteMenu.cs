using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Server
{
    public class SiteMenu : Dictionary<long, SiteMenuElement>
    {
        public SiteMenu(Context context)
        {
            Get(context: context);
        }

        private void Get(Context context)
        {
            Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectSites(
                    column: Rds.SitesColumn()
                        .TenantId()
                        .SiteId()
                        .ReferenceType()
                        .ParentId()
                        .Title(),
                    where: Rds.SitesWhere().TenantId(context.TenantId)))
                            .AsEnumerable()
                            .ForEach(dataRow =>
                                Add(dataRow.Long("SiteId"), new SiteMenuElement(
                                    context: context,
                                    siteId: dataRow.Long("SiteId"),
                                    referenceType: dataRow.String("ReferenceType"),
                                    parentId: dataRow.Long("ParentId"),
                                    title: dataRow.String("Title"))));
        }

        public SiteMenuElement Get(Context context, long siteId)
        {
            if (siteId == 0)
            {
                return null;
            }
            else
            {
                if (!ContainsKey(siteId))
                {
                    Set(context: context, siteId: siteId);
                }
                return ContainsKey(siteId)
                    ? this[siteId]
                    : null;
            }
        }

        private void Set(Context context, long siteId)
        {
            var dataRow = SiteMenuElementDataRow(context: context, siteId: siteId);
            if (dataRow != null)
            {
                var siteMenuElement = new SiteMenuElement(
                    context: context,
                    siteId: siteId,
                    referenceType: dataRow.String("ReferenceType"),
                    parentId: dataRow.Long("ParentId"),
                    title: dataRow.String("Title"));
                if (ContainsKey(siteId))
                {
                    this[siteId] = siteMenuElement;
                }
                else
                {
                    Add(siteId, siteMenuElement);
                }
            }
        }

        public IEnumerable<SiteMenuElement> Children(
            Context context,
            long siteId,
            List<SiteMenuElement> data = null,
            bool withParent = false)
        {
            if (data == null)
            {
                data = new List<SiteMenuElement>();
                if (withParent) data.Add(Get(context: context, siteId: siteId));
            }
            this.Select(o => o.Value)
                .Where(o => o.TenantId == context.TenantId)
                .Where(o => o.ParentId == siteId)
                .ForEach(element =>
                {
                    data.Add(element);
                    Children(
                        context: context,
                        siteId: element.SiteId,
                        data: data);
                });
            return data;
        }

        public IEnumerable<SiteMenuElement> Breadcrumb(Context context, long siteId)
        {
            var ret = new List<SiteMenuElement>();
            if (siteId != 0)
            {
                var current = Get(context: context, siteId: siteId);
                if (current != null)
                {
                    ret.Add(current);
                    while (current.ParentId != 0)
                    {
                        current = Get(context: context, siteId: current.ParentId);
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

        public IEnumerable<SiteCondition> SiteConditions(Context context, SiteSettings ss)
        {
            var hash = ChildHash(context: context, ss: ss);
            var sites = Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectSites(
                    column: Rds.SitesColumn()
                        .SiteId()
                        .ReferenceType(),
                    where: Rds.SitesWhere()
                        .TenantId(context.TenantId)
                        .SiteId_In(hash.SelectMany(o => o.Value))
                        .ReferenceType("Sites", _operator: "<>")
                        .Add(
                            raw: Def.Sql.CanReadSites,
                            _using: !context.HasPrivilege)))
                                .AsEnumerable();
            var issues = sites
                .Where(o => o.String("ReferenceType") == "Issues")
                .Select(o => o["SiteId"].ToLong());
            var dataSet = Rds.ExecuteDataSet(
                context: context,
                statements: new SqlStatement[]
                {
                    Rds.SelectItems(
                        dataTableName: "Items",
                        column: Rds.ItemsColumn()
                            .SiteId()
                            .ItemsCount()
                            .UpdatedTime(function: Sqls.Functions.Max),
                        where: Rds.ItemsWhere()
                            .SiteId_In(sites.Select(o => o["SiteId"].ToLong()))
                            .ReferenceType("Sites", _operator: "<>"),
                        groupBy: Rds.ItemsGroupBy()
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
                        groupBy: Rds.IssuesGroupBy()
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
                items.Any()
                    ? items.Max(o => o["UpdatedTimeMax"].ToDateTime())
                    : DateTime.MinValue);
        }

        private Dictionary<long, List<long>> ChildHash(Context context, SiteSettings ss)
        {
            var ret = new Dictionary<long, List<long>>();
            var hash = this
                .Select(o => o.Value)
                .Where(o => o.TenantId == context.TenantId)
                .ToList();
            hash.Where(o => o.ParentId == ss.SiteId)
                .ForEach(child =>
                    ret.Add(child.SiteId, ChildHashValues(hash, child.SiteId)));
            return ret;
        }

        private List<long> ChildHashValues(
            List<SiteMenuElement> hash,
            long siteId,
            List<long> data = null)
        {
            if (data == null) data = new List<long>();
            data.Add(siteId);
            hash.Where(o => o.ParentId == siteId)
                .ForEach(child =>
                    ChildHashValues(hash, child.SiteId, data));
            return data;
        }

        private DataRow SiteMenuElementDataRow(Context context, long siteId)
        {
            return Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectSites(
                    column: Rds.SitesColumn()
                        .TenantId()
                        .ReferenceType()
                        .ParentId()
                        .Title(),
                    where: Rds.SitesWhere()
                        .TenantId(context.TenantId, _using: context.TenantId != 0)
                        .SiteId(siteId)))
                            .AsEnumerable()
                            .FirstOrDefault();
        }
    }
}