using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Initializers
{
    public static class ItemsInitializer
    {
        public static void Initialize(Context context)
        {
            if (!context.HasPrivilege) return;
            var sqlExists = "exists (select * from {0} where {1}={2})";
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.PhysicalDeleteItems(
                    where: Rds.ItemsWhere().Add(raw: sqlExists.Params(
                        "[Items_Deleted]",
                        "[Items_Deleted].[ReferenceId]",
                        "[Items].[ReferenceId]"))));
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.PhysicalDeleteSites(
                    where: Rds.ItemsWhere().Add(raw: sqlExists.Params(
                        "[Sites_Deleted]",
                        "[Sites_Deleted].[SiteId]",
                        "[Sites].[SiteId]"))));
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.DeleteSites(
                    factory: context,
                    where: Rds.ItemsWhere().Add(raw: sqlExists.Params(
                        "[Items_Deleted]",
                        "[Items_Deleted].[ReferenceId]",
                        "[Sites].[SiteId]"))));
            new SiteCollection(
                context: context,
                where: Rds.SitesWhere().Add(raw: "not " + sqlExists.Params(
                    "[Items]",
                    "[Items].[ReferenceId]",
                    "[Sites].[SiteId]")),
                tableType: Sqls.TableTypes.Normal)
                    .ForEach(siteModel =>
                    {
                        if (siteModel.SiteSettings != null)
                        {
                            var fullText = siteModel.FullText(
                                context: new Context(tenantId: siteModel.TenantId),
                                ss: siteModel.SiteSettings);
                            Repository.ExecuteNonQuery(
                                context: new Context(tenantId: siteModel.TenantId),
                                connectionString: Parameters.Rds.OwnerConnectionString,
                                statements: new SqlStatement[]
                                {
                                    Rds.IdentityInsertItems(
                                        factory: context,
                                        on: true),
                                    Rds.InsertItems(
                                        param: Rds.ItemsParam()
                                            .ReferenceId(siteModel.SiteId)
                                            .ReferenceType("Sites")
                                            .SiteId(siteModel.SiteId)
                                            .Title(siteModel.Title.Value)
                                            .FullText(fullText, _using: fullText != null)
                                            .SearchIndexCreatedTime(DateTime.Now)),
                                    Rds.IdentityInsertItems(
                                        factory: context,
                                        on: false)
                                });
                        }
                    });
            new SiteCollection(
                context: context,
                where: Rds.SitesWhere().Add(raw: "not " + sqlExists.Params(
                    "[Items_Deleted]",
                    "[Items_Deleted].[ReferenceId]",
                    "[Sites].[SiteId]")),
                tableType: Sqls.TableTypes.Deleted)
                    .ForEach(siteModel =>
                    {
                        if (siteModel.SiteSettings != null)
                        {
                            Repository.ExecuteNonQuery(
                                context: new Context(tenantId: siteModel.TenantId),
                                statements: new SqlStatement[]
                                {
                                    Rds.InsertItems(
                                        tableType: Sqls.TableTypes.Deleted,
                                        param: Rds.ItemsParam()
                                            .ReferenceId(siteModel.SiteId)
                                            .Ver(siteModel.Ver)
                                            .ReferenceType("Sites")
                                            .SiteId(siteModel.SiteId)
                                            .Title(siteModel.Title.Value))
                                });
                }
            });
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.PhysicalDeleteIssues(
                    where: Rds.ItemsWhere().Add(raw: sqlExists.Params(
                        "[Issues_Deleted]",
                        "[Issues_Deleted].[IssueId]",
                        "[Issues].[IssueId]"))));
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.DeleteIssues(
                    factory: context,
                    where: Rds.ItemsWhere().Add(raw: sqlExists.Params(
                        "[Items_Deleted]",
                        "[Items_Deleted].[ReferenceId]",
                        "[Issues].[IssueId]"))));
            Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectIssues(
                    tableType: Sqls.TableTypes.Normal,
                    column: Rds.IssuesColumn()
                        .SiteId()
                        .IssueId()
                        .Ver()
                        .Sites_TenantId(),
                    join: Rds.IssuesJoinDefault()
                        .Add(
                            tableName: "Items",
                            joinType: SqlJoin.JoinTypes.LeftOuter,
                            joinExpression: "[Items].[ReferenceId]=[Issues].[IssueId]")
                        .Add(
                            tableName: "Sites",
                            joinType: SqlJoin.JoinTypes.LeftOuter,
                            joinExpression: "[Sites].[SiteId]=[Issues].[SiteId]"),
                    where: Rds.ItemsWhere()
                        .ReferenceId(
                            tableName: "Items",
                            _operator: " is null")))
                                .AsEnumerable()
                                .ForEach(dataRow =>
                                {
                                    var siteId = dataRow.Long("SiteId");
                                    var ss = new SiteModel().Get(
                                        context: new Context(tenantId: dataRow.Int("TenantId")),
                                        where: Rds.SitesWhere().SiteId(siteId))?
                                            .IssuesSiteSettings(
                                                context: new Context(tenantId: dataRow.Int("TenantId")),
                                                referenceId: dataRow.Long("IssueId"));
                                    var issueModel = new IssueModel(
                                        context: new Context(tenantId: dataRow.Int("TenantId")),
                                        ss: ss)
                                            .Get(
                                                context: new Context(tenantId: dataRow.Int("TenantId")),
                                                ss: ss,
                                                tableType: Sqls.TableTypes.Normal,
                                                where: Rds.IssuesWhere()
                                                    .SiteId(dataRow.Long("SiteId"))
                                                    .IssueId(dataRow.Long("IssueId"))
                                                    .Ver(dataRow.Int("Ver")));
                                    if (ss != null &&
                                        issueModel.AccessStatus == Databases.AccessStatuses.Selected)
                                    {
                                        var fullText = issueModel.FullText(
                                            context: new Context(tenantId: dataRow.Int("TenantId")),
                                            ss: ss);
                                        Repository.ExecuteNonQuery(
                                            context: new Context(tenantId: dataRow.Int("TenantId")),
                                            connectionString: Parameters.Rds.OwnerConnectionString,
                                            statements: new SqlStatement[]
                                            {
                                                Rds.IdentityInsertItems(
                                                    factory: context,
                                                    on: true),
                                                Rds.InsertItems(
                                                    param: Rds.ItemsParam()
                                                        .ReferenceId(issueModel.IssueId)
                                                        .ReferenceType("Issues")
                                                        .SiteId(issueModel.SiteId)
                                                        .Title(issueModel.Title.DisplayValue)
                                                        .FullText(fullText, _using: fullText != null)
                                                        .SearchIndexCreatedTime(DateTime.Now)),
                                                Rds.IdentityInsertItems(
                                                    factory: context,
                                                    on: false)
                                            });
                                    }
                                });
            Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectIssues(
                    tableType: Sqls.TableTypes.Deleted,
                    column: Rds.IssuesColumn()
                        .SiteId()
                        .IssueId()
                        .Ver(),
                    join: Rds.IssuesJoinDefault()
                        .Add(
                            tableName: "Items_Deleted",
                            joinType: SqlJoin.JoinTypes.LeftOuter,
                            joinExpression: "[Items_Deleted].[ReferenceId]=[Issues].[IssueId]")
                        .Add(
                            tableName: "Sites",
                            joinType: SqlJoin.JoinTypes.LeftOuter,
                            joinExpression: "[Sites].[SiteId]=[Issues].[SiteId]"),
                    where: Rds.ItemsWhere()
                        .ReferenceId(
                            tableName: "Items_Deleted",
                            _operator: " is null")))
                                .AsEnumerable()
                                .ForEach(dataRow =>
                                {
                                    var siteId = dataRow.Long("SiteId");
                                    var ss = new SiteModel().Get(
                                        context: new Context(tenantId: dataRow.Int("TenantId")),
                                        where: Rds.SitesWhere().SiteId(siteId))?
                                            .IssuesSiteSettings(
                                                context: new Context(tenantId: dataRow.Int("TenantId")),
                                                referenceId: dataRow.Long("IssueId"));
                                    var issueModel = new IssueModel(
                                        context: new Context(tenantId: dataRow.Int("TenantId")),
                                        ss: ss)
                                            .Get(
                                                context: new Context(tenantId: dataRow.Int("TenantId")),
                                                ss: ss,
                                                tableType: Sqls.TableTypes.Deleted,
                                                where: Rds.IssuesWhere()
                                                    .SiteId(dataRow.Long("SiteId"))
                                                    .IssueId(dataRow.Long("IssueId"))
                                                    .Ver(dataRow.Int("Ver")));
                                    if (ss != null &&
                                        issueModel.AccessStatus == Databases.AccessStatuses.Selected)
                                    {
                                        Repository.ExecuteNonQuery(
                                            context: new Context(tenantId: dataRow.Int("TenantId")),
                                            statements: new SqlStatement[]
                                            {
                                                Rds.InsertItems(
                                                    tableType: Sqls.TableTypes.Deleted,
                                                    param: Rds.ItemsParam()
                                                        .ReferenceId(issueModel.IssueId)
                                                        .Ver(issueModel.Ver)
                                                        .ReferenceType("Issues")
                                                        .SiteId(issueModel.SiteId)
                                                        .Title(issueModel.Title.DisplayValue))
                                            });
                                    }
                                });
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.PhysicalDeleteResults(
                    where: Rds.ItemsWhere().Add(raw: sqlExists.Params(
                        "[Results_Deleted]",
                        "[Results_Deleted].[ResultId]",
                        "[Results].[ResultId]"))));
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.DeleteResults(
                    factory: context,
                    where: Rds.ItemsWhere().Add(raw: sqlExists.Params(
                        "[Items_Deleted]",
                        "[Items_Deleted].[ReferenceId]",
                        "[Results].[ResultId]"))));
            Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectResults(
                    tableType: Sqls.TableTypes.Normal,
                    column: Rds.ResultsColumn()
                        .SiteId()
                        .ResultId()
                        .Ver()
                        .Sites_TenantId(),
                    join: Rds.ResultsJoinDefault()
                        .Add(
                            tableName: "Items",
                            joinType: SqlJoin.JoinTypes.LeftOuter,
                            joinExpression: "[Items].[ReferenceId]=[Results].[ResultId]")
                        .Add(
                            tableName: "Sites",
                            joinType: SqlJoin.JoinTypes.LeftOuter,
                            joinExpression: "[Sites].[SiteId]=[Results].[SiteId]"),
                    where: Rds.ItemsWhere()
                        .ReferenceId(
                            tableName: "Items",
                            _operator: " is null")))
                                .AsEnumerable()
                                .ForEach(dataRow =>
                                {
                                    var siteId = dataRow.Long("SiteId");
                                    var ss = new SiteModel().Get(
                                        context: new Context(tenantId: dataRow.Int("TenantId")),
                                        where: Rds.SitesWhere().SiteId(siteId))?
                                            .ResultsSiteSettings(
                                                context: new Context(tenantId: dataRow.Int("TenantId")),
                                                referenceId: dataRow.Long("ResultId"));
                                    var resultModel = new ResultModel(
                                        context: new Context(tenantId: dataRow.Int("TenantId")),
                                        ss: ss)
                                            .Get(
                                                context: new Context(tenantId: dataRow.Int("TenantId")),
                                                ss: ss,
                                                tableType: Sqls.TableTypes.Normal,
                                                where: Rds.ResultsWhere()
                                                    .SiteId(dataRow.Long("SiteId"))
                                                    .ResultId(dataRow.Long("ResultId"))
                                                    .Ver(dataRow.Int("Ver")));
                                    if (ss != null &&
                                        resultModel.AccessStatus == Databases.AccessStatuses.Selected)
                                    {
                                        var fullText = resultModel.FullText(
                                            context: new Context(tenantId: dataRow.Int("TenantId")),
                                            ss: ss);
                                        Repository.ExecuteNonQuery(
                                            context: new Context(tenantId: dataRow.Int("TenantId")),
                                            connectionString: Parameters.Rds.OwnerConnectionString,
                                            statements: new SqlStatement[]
                                            {
                                                Rds.IdentityInsertItems(
                                                    factory: context,
                                                    on: true),
                                                Rds.InsertItems(
                                                    param: Rds.ItemsParam()
                                                        .ReferenceId(resultModel.ResultId)
                                                        .ReferenceType("Results")
                                                        .SiteId(resultModel.SiteId)
                                                        .Title(resultModel.Title.DisplayValue)
                                                        .FullText(fullText, _using: fullText != null)
                                                        .SearchIndexCreatedTime(DateTime.Now)),
                                                Rds.IdentityInsertItems(
                                                    factory: context,
                                                    on: false)
                                            });
                                    }
                                });
            Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectResults(
                    tableType: Sqls.TableTypes.Deleted,
                    column: Rds.ResultsColumn()
                        .SiteId()
                        .ResultId()
                        .Ver(),
                    join: Rds.ResultsJoinDefault()
                        .Add(
                            tableName: "Items_Deleted",
                            joinType: SqlJoin.JoinTypes.LeftOuter,
                            joinExpression: "[Items_Deleted].[ReferenceId]=[Results].[ResultId]")
                        .Add(
                            tableName: "Sites",
                            joinType: SqlJoin.JoinTypes.LeftOuter,
                            joinExpression: "[Sites].[SiteId]=[Results].[SiteId]"),
                    where: Rds.ItemsWhere()
                        .ReferenceId(
                            tableName: "Items_Deleted",
                            _operator: " is null")))
                                .AsEnumerable()
                                .ForEach(dataRow =>
                                {
                                    var siteId = dataRow.Long("SiteId");
                                    var ss = new SiteModel().Get(
                                        context: new Context(tenantId: dataRow.Int("TenantId")),
                                        where: Rds.SitesWhere().SiteId(siteId))?
                                            .ResultsSiteSettings(
                                                context: new Context(tenantId: dataRow.Int("TenantId")),
                                                referenceId: dataRow.Long("ResultId"));
                                    var resultModel = new ResultModel(
                                        context: new Context(tenantId: dataRow.Int("TenantId")),
                                        ss: ss)
                                            .Get(
                                                context: new Context(tenantId: dataRow.Int("TenantId")),
                                                ss: ss,
                                                tableType: Sqls.TableTypes.Deleted,
                                                where: Rds.ResultsWhere()
                                                    .SiteId(dataRow.Long("SiteId"))
                                                    .ResultId(dataRow.Long("ResultId"))
                                                    .Ver(dataRow.Int("Ver")));
                                    if (ss != null &&
                                        resultModel.AccessStatus == Databases.AccessStatuses.Selected)
                                    {
                                        Repository.ExecuteNonQuery(
                                            context: new Context(tenantId: dataRow.Int("TenantId")),
                                            statements: new SqlStatement[]
                                            {
                                                Rds.InsertItems(
                                                    tableType: Sqls.TableTypes.Deleted,
                                                    param: Rds.ItemsParam()
                                                        .ReferenceId(resultModel.ResultId)
                                                        .Ver(resultModel.Ver)
                                                        .ReferenceType("Results")
                                                        .SiteId(resultModel.SiteId)
                                                        .Title(resultModel.Title.DisplayValue))
                                            });
                                    }
                                });
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.PhysicalDeleteWikis(
                    where: Rds.ItemsWhere().Add(raw: sqlExists.Params(
                        "[Wikis_Deleted]",
                        "[Wikis_Deleted].[WikiId]",
                        "[Wikis].[WikiId]"))));
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.DeleteWikis(
                    factory: context,
                    where: Rds.ItemsWhere().Add(raw: sqlExists.Params(
                        "[Items_Deleted]",
                        "[Items_Deleted].[ReferenceId]",
                        "[Wikis].[WikiId]"))));
            Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectWikis(
                    tableType: Sqls.TableTypes.Normal,
                    column: Rds.WikisColumn()
                        .SiteId()
                        .WikiId()
                        .Ver()
                        .Sites_TenantId(),
                    join: Rds.WikisJoinDefault()
                        .Add(
                            tableName: "Items",
                            joinType: SqlJoin.JoinTypes.LeftOuter,
                            joinExpression: "[Items].[ReferenceId]=[Wikis].[WikiId]")
                        .Add(
                            tableName: "Sites",
                            joinType: SqlJoin.JoinTypes.LeftOuter,
                            joinExpression: "[Sites].[SiteId]=[Wikis].[SiteId]"),
                    where: Rds.ItemsWhere()
                        .ReferenceId(
                            tableName: "Items",
                            _operator: " is null")))
                                .AsEnumerable()
                                .ForEach(dataRow =>
                                {
                                    var siteId = dataRow.Long("SiteId");
                                    var ss = new SiteModel().Get(
                                        context: new Context(tenantId: dataRow.Int("TenantId")),
                                        where: Rds.SitesWhere().SiteId(siteId))?
                                            .WikisSiteSettings(
                                                context: new Context(tenantId: dataRow.Int("TenantId")),
                                                referenceId: dataRow.Long("WikiId"));
                                    var wikiModel = new WikiModel(
                                        context: new Context(tenantId: dataRow.Int("TenantId")),
                                        ss: ss)
                                            .Get(
                                                context: new Context(tenantId: dataRow.Int("TenantId")),
                                                ss: ss,
                                                tableType: Sqls.TableTypes.Normal,
                                                where: Rds.WikisWhere()
                                                    .SiteId(dataRow.Long("SiteId"))
                                                    .WikiId(dataRow.Long("WikiId"))
                                                    .Ver(dataRow.Int("Ver")));
                                    if (ss != null &&
                                        wikiModel.AccessStatus == Databases.AccessStatuses.Selected)
                                    {
                                        var fullText = wikiModel.FullText(
                                            context: new Context(tenantId: dataRow.Int("TenantId")),
                                            ss: ss);
                                        Repository.ExecuteNonQuery(
                                            context: new Context(tenantId: dataRow.Int("TenantId")),
                                            connectionString: Parameters.Rds.OwnerConnectionString,
                                            statements: new SqlStatement[]
                                            {
                                                Rds.IdentityInsertItems(
                                                    factory: context,
                                                    on: true),
                                                Rds.InsertItems(
                                                    param: Rds.ItemsParam()
                                                        .ReferenceId(wikiModel.WikiId)
                                                        .ReferenceType("Wikis")
                                                        .SiteId(wikiModel.SiteId)
                                                        .Title(wikiModel.Title.DisplayValue)
                                                        .FullText(fullText, _using: fullText != null)
                                                        .SearchIndexCreatedTime(DateTime.Now)),
                                                Rds.IdentityInsertItems(
                                                    factory: context,
                                                    on: false)
                                            });
                                    }
                                });
            Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectWikis(
                    tableType: Sqls.TableTypes.Deleted,
                    column: Rds.WikisColumn()
                        .SiteId()
                        .WikiId()
                        .Ver(),
                    join: Rds.WikisJoinDefault()
                        .Add(
                            tableName: "Items_Deleted",
                            joinType: SqlJoin.JoinTypes.LeftOuter,
                            joinExpression: "[Items_Deleted].[ReferenceId]=[Wikis].[WikiId]")
                        .Add(
                            tableName: "Sites",
                            joinType: SqlJoin.JoinTypes.LeftOuter,
                            joinExpression: "[Sites].[SiteId]=[Wikis].[SiteId]"),
                    where: Rds.ItemsWhere()
                        .ReferenceId(
                            tableName: "Items_Deleted",
                            _operator: " is null")))
                                .AsEnumerable()
                                .ForEach(dataRow =>
                                {
                                    var siteId = dataRow.Long("SiteId");
                                    var ss = new SiteModel().Get(
                                        context: new Context(tenantId: dataRow.Int("TenantId")),
                                        where: Rds.SitesWhere().SiteId(siteId))?
                                            .WikisSiteSettings(
                                                context: new Context(tenantId: dataRow.Int("TenantId")),
                                                referenceId: dataRow.Long("WikiId"));
                                    var wikiModel = new WikiModel(
                                        context: new Context(tenantId: dataRow.Int("TenantId")),
                                        ss: ss)
                                            .Get(
                                                context: new Context(tenantId: dataRow.Int("TenantId")),
                                                ss: ss,
                                                tableType: Sqls.TableTypes.Deleted,
                                                where: Rds.WikisWhere()
                                                    .SiteId(dataRow.Long("SiteId"))
                                                    .WikiId(dataRow.Long("WikiId"))
                                                    .Ver(dataRow.Int("Ver")));
                                    if (ss != null &&
                                        wikiModel.AccessStatus == Databases.AccessStatuses.Selected)
                                    {
                                        Repository.ExecuteNonQuery(
                                            context: new Context(tenantId: dataRow.Int("TenantId")),
                                            statements: new SqlStatement[]
                                            {
                                                Rds.InsertItems(
                                                    tableType: Sqls.TableTypes.Deleted,
                                                    param: Rds.ItemsParam()
                                                        .ReferenceId(wikiModel.WikiId)
                                                        .Ver(wikiModel.Ver)
                                                        .ReferenceType("Wikis")
                                                        .SiteId(wikiModel.SiteId)
                                                        .Title(wikiModel.Title.DisplayValue))
                                            });
                                    }
                                });
        }
    }
}
