﻿using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
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
        public static void Initialize()
        {
            var siteExists = "not exists (select * from [{0}] where [{0}].[ReferenceId]=[Sites].[SiteId])";
            new SiteCollection(
                where: Rds.SitesWhere().Add(raw: siteExists.Params("Items")),
                tableType: Sqls.TableTypes.Normal)
                    .ForEach(siteModel =>
                    {
                        if (siteModel.SiteSettings != null)
                        {
                            var fullText = siteModel.FullText(siteModel.SiteSettings);
                            Rds.ExecuteNonQuery(
                                connectionString: Parameters.Rds.OwnerConnectionString,
                                statements: new SqlStatement[]
                                {
                                    Rds.IdentityInsertItems(on: true),
                                    Rds.InsertItems(
                                        param: Rds.ItemsParam()
                                            .ReferenceId(siteModel.SiteId)
                                            .ReferenceType("Sites")
                                            .SiteId(siteModel.SiteId)
                                            .Title(siteModel.Title.Value)
                                            .FullText(fullText, _using: fullText != null)
                                            .SearchIndexCreatedTime(DateTime.Now)),
                                    Rds.IdentityInsertItems(on: false)
                                });
                        }
                    });
            new SiteCollection(
                where: Rds.SitesWhere().Add(raw: siteExists.Params("Items_Deleted")),
                tableType: Sqls.TableTypes.Deleted)
                    .ForEach(siteModel =>
                    {
                        if (siteModel.SiteSettings != null)
                        {
                            Rds.ExecuteNonQuery(
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
            Rds.ExecuteTable(statements: Rds.SelectIssues(
                tableType: Sqls.TableTypes.Normal,
                column: Rds.IssuesColumn()
                    .SiteId()
                    .IssueId()
                    .Ver(),
                join: Rds.IssuesJoinDefault()
                    .Add(
                        tableName: "Items",
                        joinType: SqlJoin.JoinTypes.LeftOuter,
                        joinExpression: "[Items].[ReferenceId]=[Issues].[IssueId]"),
                where: Rds.ItemsWhere()
                    .ReferenceId(
                        tableName: "Items",
                        _operator: " is null")))
                            .AsEnumerable()
                            .ForEach(dataRow =>
                            {
                                var siteId = dataRow["SiteId"].ToLong();
                                var ss = new SiteModel().Get(where:
                                    Rds.SitesWhere().SiteId(siteId))?
                                        .IssuesSiteSettings(dataRow["IssueId"].ToLong());
                                var issueModel = new IssueModel(ss).Get(
                                    ss: ss,
                                    tableType: Sqls.TableTypes.Normal,
                                    where: Rds.IssuesWhere()
                                        .SiteId(dataRow["SiteId"].ToLong())
                                        .IssueId(dataRow["IssueId"].ToLong())
                                        .Ver(dataRow["Ver"].ToInt()));
                                if (ss != null &&
                                    issueModel.AccessStatus == Databases.AccessStatuses.Selected)
                                {
                                    var fullText = issueModel.FullText(ss);
                                    Rds.ExecuteNonQuery(
                                        connectionString: Parameters.Rds.OwnerConnectionString,
                                        statements: new SqlStatement[]
                                        {
                                            Rds.IdentityInsertItems(on: true),
                                            Rds.InsertItems(
                                                param: Rds.ItemsParam()
                                                    .ReferenceId(issueModel.IssueId)
                                                    .ReferenceType("Issues")
                                                    .SiteId(issueModel.SiteId)
                                                    .Title(issueModel.Title.DisplayValue)
                                                    .FullText(fullText, _using: fullText != null)
                                                    .SearchIndexCreatedTime(DateTime.Now)),
                                            Rds.IdentityInsertItems(on: false)
                                        });
                                }
                            });
            Rds.ExecuteTable(statements: Rds.SelectIssues(
                tableType: Sqls.TableTypes.Deleted,
                column: Rds.IssuesColumn()
                    .SiteId()
                    .IssueId()
                    .Ver(),
                join: Rds.IssuesJoinDefault()
                    .Add(
                        tableName: "Items_Deleted",
                        joinType: SqlJoin.JoinTypes.LeftOuter,
                        joinExpression: "[Items_Deleted].[ReferenceId]=[Issues].[IssueId]"),
                where: Rds.ItemsWhere()
                    .ReferenceId(
                        tableName: "Items_Deleted",
                        _operator: " is null")))
                            .AsEnumerable()
                            .ForEach(dataRow =>
                            {
                                var siteId = dataRow["SiteId"].ToLong();
                                var ss = new SiteModel().Get(where:
                                    Rds.SitesWhere().SiteId(siteId))?
                                        .IssuesSiteSettings(dataRow["IssueId"].ToLong());
                                var issueModel = new IssueModel(ss).Get(
                                    ss: ss,
                                    tableType: Sqls.TableTypes.Deleted,
                                    where: Rds.IssuesWhere()
                                        .SiteId(dataRow["SiteId"].ToLong())
                                        .IssueId(dataRow["IssueId"].ToLong())
                                        .Ver(dataRow["Ver"].ToInt()));
                                if (ss != null &&
                                    issueModel.AccessStatus == Databases.AccessStatuses.Selected)
                                {
                                    Rds.ExecuteNonQuery(
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
            Rds.ExecuteTable(statements: Rds.SelectResults(
                tableType: Sqls.TableTypes.Normal,
                column: Rds.ResultsColumn()
                    .SiteId()
                    .ResultId()
                    .Ver(),
                join: Rds.ResultsJoinDefault()
                    .Add(
                        tableName: "Items",
                        joinType: SqlJoin.JoinTypes.LeftOuter,
                        joinExpression: "[Items].[ReferenceId]=[Results].[ResultId]"),
                where: Rds.ItemsWhere()
                    .ReferenceId(
                        tableName: "Items",
                        _operator: " is null")))
                            .AsEnumerable()
                            .ForEach(dataRow =>
                            {
                                var siteId = dataRow["SiteId"].ToLong();
                                var ss = new SiteModel().Get(where:
                                    Rds.SitesWhere().SiteId(siteId))?
                                        .ResultsSiteSettings(dataRow["ResultId"].ToLong());
                                var resultModel = new ResultModel(ss).Get(
                                    ss: ss,
                                    tableType: Sqls.TableTypes.Normal,
                                    where: Rds.ResultsWhere()
                                        .SiteId(dataRow["SiteId"].ToLong())
                                        .ResultId(dataRow["ResultId"].ToLong())
                                        .Ver(dataRow["Ver"].ToInt()));
                                if (ss != null &&
                                    resultModel.AccessStatus == Databases.AccessStatuses.Selected)
                                {
                                    var fullText = resultModel.FullText(ss);
                                    Rds.ExecuteNonQuery(
                                        connectionString: Parameters.Rds.OwnerConnectionString,
                                        statements: new SqlStatement[]
                                        {
                                            Rds.IdentityInsertItems(on: true),
                                            Rds.InsertItems(
                                                param: Rds.ItemsParam()
                                                    .ReferenceId(resultModel.ResultId)
                                                    .ReferenceType("Results")
                                                    .SiteId(resultModel.SiteId)
                                                    .Title(resultModel.Title.DisplayValue)
                                                    .FullText(fullText, _using: fullText != null)
                                                    .SearchIndexCreatedTime(DateTime.Now)),
                                            Rds.IdentityInsertItems(on: false)
                                        });
                                }
                            });
            Rds.ExecuteTable(statements: Rds.SelectResults(
                tableType: Sqls.TableTypes.Deleted,
                column: Rds.ResultsColumn()
                    .SiteId()
                    .ResultId()
                    .Ver(),
                join: Rds.ResultsJoinDefault()
                    .Add(
                        tableName: "Items_Deleted",
                        joinType: SqlJoin.JoinTypes.LeftOuter,
                        joinExpression: "[Items_Deleted].[ReferenceId]=[Results].[ResultId]"),
                where: Rds.ItemsWhere()
                    .ReferenceId(
                        tableName: "Items_Deleted",
                        _operator: " is null")))
                            .AsEnumerable()
                            .ForEach(dataRow =>
                            {
                                var siteId = dataRow["SiteId"].ToLong();
                                var ss = new SiteModel().Get(where:
                                    Rds.SitesWhere().SiteId(siteId))?
                                        .ResultsSiteSettings(dataRow["ResultId"].ToLong());
                                var resultModel = new ResultModel(ss).Get(
                                    ss: ss,
                                    tableType: Sqls.TableTypes.Deleted,
                                    where: Rds.ResultsWhere()
                                        .SiteId(dataRow["SiteId"].ToLong())
                                        .ResultId(dataRow["ResultId"].ToLong())
                                        .Ver(dataRow["Ver"].ToInt()));
                                if (ss != null &&
                                    resultModel.AccessStatus == Databases.AccessStatuses.Selected)
                                {
                                    Rds.ExecuteNonQuery(
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
            Rds.ExecuteTable(statements: Rds.SelectWikis(
                tableType: Sqls.TableTypes.Normal,
                column: Rds.WikisColumn()
                    .SiteId()
                    .WikiId()
                    .Ver(),
                join: Rds.WikisJoinDefault()
                    .Add(
                        tableName: "Items",
                        joinType: SqlJoin.JoinTypes.LeftOuter,
                        joinExpression: "[Items].[ReferenceId]=[Wikis].[WikiId]"),
                where: Rds.ItemsWhere()
                    .ReferenceId(
                        tableName: "Items",
                        _operator: " is null")))
                            .AsEnumerable()
                            .ForEach(dataRow =>
                            {
                                var siteId = dataRow["SiteId"].ToLong();
                                var ss = new SiteModel().Get(where:
                                    Rds.SitesWhere().SiteId(siteId))?
                                        .WikisSiteSettings(dataRow["WikiId"].ToLong());
                                var wikiModel = new WikiModel(ss).Get(
                                    ss: ss,
                                    tableType: Sqls.TableTypes.Normal,
                                    where: Rds.WikisWhere()
                                        .SiteId(dataRow["SiteId"].ToLong())
                                        .WikiId(dataRow["WikiId"].ToLong())
                                        .Ver(dataRow["Ver"].ToInt()));
                                if (ss != null &&
                                    wikiModel.AccessStatus == Databases.AccessStatuses.Selected)
                                {
                                    var fullText = wikiModel.FullText(ss);
                                    Rds.ExecuteNonQuery(
                                        connectionString: Parameters.Rds.OwnerConnectionString,
                                        statements: new SqlStatement[]
                                        {
                                            Rds.IdentityInsertItems(on: true),
                                            Rds.InsertItems(
                                                param: Rds.ItemsParam()
                                                    .ReferenceId(wikiModel.WikiId)
                                                    .ReferenceType("Wikis")
                                                    .SiteId(wikiModel.SiteId)
                                                    .Title(wikiModel.Title.DisplayValue)
                                                    .FullText(fullText, _using: fullText != null)
                                                    .SearchIndexCreatedTime(DateTime.Now)),
                                            Rds.IdentityInsertItems(on: false)
                                        });
                                }
                            });
            Rds.ExecuteTable(statements: Rds.SelectWikis(
                tableType: Sqls.TableTypes.Deleted,
                column: Rds.WikisColumn()
                    .SiteId()
                    .WikiId()
                    .Ver(),
                join: Rds.WikisJoinDefault()
                    .Add(
                        tableName: "Items_Deleted",
                        joinType: SqlJoin.JoinTypes.LeftOuter,
                        joinExpression: "[Items_Deleted].[ReferenceId]=[Wikis].[WikiId]"),
                where: Rds.ItemsWhere()
                    .ReferenceId(
                        tableName: "Items_Deleted",
                        _operator: " is null")))
                            .AsEnumerable()
                            .ForEach(dataRow =>
                            {
                                var siteId = dataRow["SiteId"].ToLong();
                                var ss = new SiteModel().Get(where:
                                    Rds.SitesWhere().SiteId(siteId))?
                                        .WikisSiteSettings(dataRow["WikiId"].ToLong());
                                var wikiModel = new WikiModel(ss).Get(
                                    ss: ss,
                                    tableType: Sqls.TableTypes.Deleted,
                                    where: Rds.WikisWhere()
                                        .SiteId(dataRow["SiteId"].ToLong())
                                        .WikiId(dataRow["WikiId"].ToLong())
                                        .Ver(dataRow["Ver"].ToInt()));
                                if (ss != null &&
                                    wikiModel.AccessStatus == Databases.AccessStatuses.Selected)
                                {
                                    Rds.ExecuteNonQuery(
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
