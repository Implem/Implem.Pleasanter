using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Initializers
{
    public static class ItemsInitializer
    {
        public static void Initialize()
        {
            if (Rds.ExecuteScalar_int(statements: 
                Rds.SelectItems(column: Rds.ItemsColumn().ItemsCount())) == 0)
            {
                new SiteCollection().ForEach(siteModel =>
                {
                    if (siteModel.SiteSettings != null)
                    {
                        Rds.ExecuteNonQuery(
                            connectionString: Def.Parameters.RdsOwnerConnectionString,
                            statements: new SqlStatement[]
                            {
                                Rds.IdentityInsertItems(on: true),
                                Rds.InsertItems(
                                    param: Rds.ItemsParam()
                                        .ReferenceId(siteModel.SiteId)
                                        .ReferenceType("Sites")
                                        .SiteId(siteModel.SiteId)
                                        .Title(siteModel.Title.Value)
                                        .Subset(Jsons.ToJson(new SiteSubset(
                                            siteModel, siteModel.SiteSettings)))),
                                Rds.IdentityInsertItems(on: false)
                            });
                    }
                });
                new SiteCollection(tableType: Sqls.TableTypes.Deleted).ForEach(siteModel =>
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
                                        .Title(siteModel.Title.Value)
                                        .Subset(Jsons.ToJson(new SiteSubset(
                                            siteModel, siteModel.SiteSettings))))
                            });
                    }
                });
                new SiteCollection(tableType: Sqls.TableTypes.History).ForEach(siteModel =>
                {
                    if (siteModel.SiteSettings != null)
                    {
                        Rds.ExecuteNonQuery(
                            statements: new SqlStatement[]
                            {
                                Rds.InsertItems(
                                    tableType: Sqls.TableTypes.History,
                                    param: Rds.ItemsParam()
                                        .ReferenceId(siteModel.SiteId)
                                        .Ver(siteModel.Ver)
                                        .ReferenceType("Sites")
                                        .SiteId(siteModel.SiteId)
                                        .Title(siteModel.Title.Value)
                                        .Subset(Jsons.ToJson(new SiteSubset(
                                            siteModel, siteModel.SiteSettings))))
                            });
                    }
                });
                Rds.ExecuteTable(statements: Rds.SelectIssues(
                    tableType: Sqls.TableTypes.Normal,
                    column: Rds.IssuesColumn()
                        .SiteId()
                        .IssueId()
                        .Ver())).AsEnumerable().ForEach(dataRow =>
                    {
                        var siteSettings = new SiteModel().Get(where:
                            Rds.SitesWhere().SiteId(dataRow["SiteId"].ToLong()))?
                                .IssuesSiteSettings();
                        var issueModel = new IssueModel(siteSettings).Get(
                            tableType: Sqls.TableTypes.Normal,
                            where: Rds.IssuesWhere()
                                .SiteId(dataRow["SiteId"].ToLong())
                                .IssueId(dataRow["IssueId"].ToLong())
                                .Ver(dataRow["Ver"].ToInt()));
                        if (siteSettings != null &&
                            issueModel.AccessStatus == Databases.AccessStatuses.Selected)
                        {
                            Rds.ExecuteNonQuery(
                                connectionString: Def.Parameters.RdsOwnerConnectionString,
                                statements: new SqlStatement[]
                                {
                                    Rds.IdentityInsertItems(on: true),
                                    Rds.InsertItems(
                                        param: Rds.ItemsParam()
                                            .ReferenceId(issueModel.IssueId)
                                            .ReferenceType("Issues")
                                            .SiteId(issueModel.SiteId)
                                            .Title(IssuesUtility.TitleDisplayValue(
                                                siteSettings, issueModel))
                                            .Subset(Jsons.ToJson(new IssueSubset(
                                                issueModel, siteSettings)))),
                                    Rds.IdentityInsertItems(on: false)
                                });
                        }
                    });
                Rds.ExecuteTable(statements: Rds.SelectIssues(
                    tableType: Sqls.TableTypes.Deleted,
                    column: Rds.IssuesColumn()
                        .SiteId()
                        .IssueId()
                        .Ver())).AsEnumerable().ForEach(dataRow =>
                    {
                        var siteSettings = new SiteModel().Get(where:
                            Rds.SitesWhere().SiteId(dataRow["SiteId"].ToLong()))?
                                .IssuesSiteSettings();
                        var issueModel = new IssueModel(siteSettings).Get(
                            tableType: Sqls.TableTypes.Deleted,
                            where: Rds.IssuesWhere()
                                .SiteId(dataRow["SiteId"].ToLong())
                                .IssueId(dataRow["IssueId"].ToLong())
                                .Ver(dataRow["Ver"].ToInt()));
                        if (siteSettings != null &&
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
                                            .Title(IssuesUtility.TitleDisplayValue(
                                                siteSettings, issueModel))
                                            .Subset(Jsons.ToJson(new IssueSubset(
                                                issueModel, siteSettings))))
                                });
                        }
                    });
                Rds.ExecuteTable(statements: Rds.SelectIssues(
                    tableType: Sqls.TableTypes.History,
                    column: Rds.IssuesColumn()
                        .SiteId()
                        .IssueId()
                        .Ver())).AsEnumerable().ForEach(dataRow =>
                    {
                        var siteSettings = new SiteModel().Get(where:
                            Rds.SitesWhere().SiteId(dataRow["SiteId"].ToLong()))?
                                .IssuesSiteSettings();
                        var issueModel = new IssueModel(siteSettings).Get(
                            tableType: Sqls.TableTypes.History,
                            where: Rds.IssuesWhere()
                                .SiteId(dataRow["SiteId"].ToLong())
                                .IssueId(dataRow["IssueId"].ToLong())
                                .Ver(dataRow["Ver"].ToInt()));
                        if (siteSettings != null &&
                            issueModel.AccessStatus == Databases.AccessStatuses.Selected)
                        {
                            Rds.ExecuteNonQuery(
                                statements: new SqlStatement[]
                                {
                                    Rds.InsertItems(
                                        tableType: Sqls.TableTypes.History,
                                        param: Rds.ItemsParam()
                                            .ReferenceId(issueModel.IssueId)
                                            .Ver(issueModel.Ver)
                                            .ReferenceType("Issues")
                                            .SiteId(issueModel.SiteId)
                                            .Title(IssuesUtility.TitleDisplayValue(
                                                siteSettings, issueModel))
                                            .Subset(Jsons.ToJson(new IssueSubset(
                                                issueModel, siteSettings))))
                                });
                        }
                    });
                Rds.ExecuteTable(statements: Rds.SelectResults(
                    tableType: Sqls.TableTypes.Normal,
                    column: Rds.ResultsColumn()
                        .SiteId()
                        .ResultId()
                        .Ver())).AsEnumerable().ForEach(dataRow =>
                    {
                        var siteSettings = new SiteModel().Get(where:
                            Rds.SitesWhere().SiteId(dataRow["SiteId"].ToLong()))?
                                .ResultsSiteSettings();
                        var resultModel = new ResultModel(siteSettings).Get(
                            tableType: Sqls.TableTypes.Normal,
                            where: Rds.ResultsWhere()
                                .SiteId(dataRow["SiteId"].ToLong())
                                .ResultId(dataRow["ResultId"].ToLong())
                                .Ver(dataRow["Ver"].ToInt()));
                        if (siteSettings != null &&
                            resultModel.AccessStatus == Databases.AccessStatuses.Selected)
                        {
                            Rds.ExecuteNonQuery(
                                connectionString: Def.Parameters.RdsOwnerConnectionString,
                                statements: new SqlStatement[]
                                {
                                    Rds.IdentityInsertItems(on: true),
                                    Rds.InsertItems(
                                        param: Rds.ItemsParam()
                                            .ReferenceId(resultModel.ResultId)
                                            .ReferenceType("Results")
                                            .SiteId(resultModel.SiteId)
                                            .Title(ResultsUtility.TitleDisplayValue(
                                                siteSettings, resultModel))
                                            .Subset(Jsons.ToJson(new ResultSubset(
                                                resultModel, siteSettings)))),
                                    Rds.IdentityInsertItems(on: false)
                                });
                        }
                    });
                Rds.ExecuteTable(statements: Rds.SelectResults(
                    tableType: Sqls.TableTypes.Deleted,
                    column: Rds.ResultsColumn()
                        .SiteId()
                        .ResultId()
                        .Ver())).AsEnumerable().ForEach(dataRow =>
                    {
                        var siteSettings = new SiteModel().Get(where:
                            Rds.SitesWhere().SiteId(dataRow["SiteId"].ToLong()))?
                                .ResultsSiteSettings();
                        var resultModel = new ResultModel(siteSettings).Get(
                            tableType: Sqls.TableTypes.Deleted,
                            where: Rds.ResultsWhere()
                                .SiteId(dataRow["SiteId"].ToLong())
                                .ResultId(dataRow["ResultId"].ToLong())
                                .Ver(dataRow["Ver"].ToInt()));
                        if (siteSettings != null &&
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
                                            .Title(ResultsUtility.TitleDisplayValue(
                                                siteSettings, resultModel))
                                            .Subset(Jsons.ToJson(new ResultSubset(
                                                resultModel, siteSettings))))
                                });
                        }
                    });
                Rds.ExecuteTable(statements: Rds.SelectResults(
                    tableType: Sqls.TableTypes.History,
                    column: Rds.ResultsColumn()
                        .SiteId()
                        .ResultId()
                        .Ver())).AsEnumerable().ForEach(dataRow =>
                    {
                        var siteSettings = new SiteModel().Get(where:
                            Rds.SitesWhere().SiteId(dataRow["SiteId"].ToLong()))?
                                .ResultsSiteSettings();
                        var resultModel = new ResultModel(siteSettings).Get(
                            tableType: Sqls.TableTypes.History,
                            where: Rds.ResultsWhere()
                                .SiteId(dataRow["SiteId"].ToLong())
                                .ResultId(dataRow["ResultId"].ToLong())
                                .Ver(dataRow["Ver"].ToInt()));
                        if (siteSettings != null &&
                            resultModel.AccessStatus == Databases.AccessStatuses.Selected)
                        {
                            Rds.ExecuteNonQuery(
                                statements: new SqlStatement[]
                                {
                                    Rds.InsertItems(
                                        tableType: Sqls.TableTypes.History,
                                        param: Rds.ItemsParam()
                                            .ReferenceId(resultModel.ResultId)
                                            .Ver(resultModel.Ver)
                                            .ReferenceType("Results")
                                            .SiteId(resultModel.SiteId)
                                            .Title(ResultsUtility.TitleDisplayValue(
                                                siteSettings, resultModel))
                                            .Subset(Jsons.ToJson(new ResultSubset(
                                                resultModel, siteSettings))))
                                });
                        }
                    });
                Rds.ExecuteTable(statements: Rds.SelectWikis(
                    tableType: Sqls.TableTypes.Normal,
                    column: Rds.WikisColumn()
                        .SiteId()
                        .WikiId()
                        .Ver())).AsEnumerable().ForEach(dataRow =>
                    {
                        var siteSettings = new SiteModel().Get(where:
                            Rds.SitesWhere().SiteId(dataRow["SiteId"].ToLong()))?
                                .WikisSiteSettings();
                        var wikiModel = new WikiModel(siteSettings).Get(
                            tableType: Sqls.TableTypes.Normal,
                            where: Rds.WikisWhere()
                                .SiteId(dataRow["SiteId"].ToLong())
                                .WikiId(dataRow["WikiId"].ToLong())
                                .Ver(dataRow["Ver"].ToInt()));
                        if (siteSettings != null &&
                            wikiModel.AccessStatus == Databases.AccessStatuses.Selected)
                        {
                            Rds.ExecuteNonQuery(
                                connectionString: Def.Parameters.RdsOwnerConnectionString,
                                statements: new SqlStatement[]
                                {
                                    Rds.IdentityInsertItems(on: true),
                                    Rds.InsertItems(
                                        param: Rds.ItemsParam()
                                            .ReferenceId(wikiModel.WikiId)
                                            .ReferenceType("Wikis")
                                            .SiteId(wikiModel.SiteId)
                                            .Title(WikisUtility.TitleDisplayValue(
                                                siteSettings, wikiModel))
                                            .Subset(Jsons.ToJson(new WikiSubset(
                                                wikiModel, siteSettings)))),
                                    Rds.IdentityInsertItems(on: false)
                                });
                        }
                    });
                Rds.ExecuteTable(statements: Rds.SelectWikis(
                    tableType: Sqls.TableTypes.Deleted,
                    column: Rds.WikisColumn()
                        .SiteId()
                        .WikiId()
                        .Ver())).AsEnumerable().ForEach(dataRow =>
                    {
                        var siteSettings = new SiteModel().Get(where:
                            Rds.SitesWhere().SiteId(dataRow["SiteId"].ToLong()))?
                                .WikisSiteSettings();
                        var wikiModel = new WikiModel(siteSettings).Get(
                            tableType: Sqls.TableTypes.Deleted,
                            where: Rds.WikisWhere()
                                .SiteId(dataRow["SiteId"].ToLong())
                                .WikiId(dataRow["WikiId"].ToLong())
                                .Ver(dataRow["Ver"].ToInt()));
                        if (siteSettings != null &&
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
                                            .Title(WikisUtility.TitleDisplayValue(
                                                siteSettings, wikiModel))
                                            .Subset(Jsons.ToJson(new WikiSubset(
                                                wikiModel, siteSettings))))
                                });
                        }
                    });
                Rds.ExecuteTable(statements: Rds.SelectWikis(
                    tableType: Sqls.TableTypes.History,
                    column: Rds.WikisColumn()
                        .SiteId()
                        .WikiId()
                        .Ver())).AsEnumerable().ForEach(dataRow =>
                    {
                        var siteSettings = new SiteModel().Get(where:
                            Rds.SitesWhere().SiteId(dataRow["SiteId"].ToLong()))?
                                .WikisSiteSettings();
                        var wikiModel = new WikiModel(siteSettings).Get(
                            tableType: Sqls.TableTypes.History,
                            where: Rds.WikisWhere()
                                .SiteId(dataRow["SiteId"].ToLong())
                                .WikiId(dataRow["WikiId"].ToLong())
                                .Ver(dataRow["Ver"].ToInt()));
                        if (siteSettings != null &&
                            wikiModel.AccessStatus == Databases.AccessStatuses.Selected)
                        {
                            Rds.ExecuteNonQuery(
                                statements: new SqlStatement[]
                                {
                                    Rds.InsertItems(
                                        tableType: Sqls.TableTypes.History,
                                        param: Rds.ItemsParam()
                                            .ReferenceId(wikiModel.WikiId)
                                            .Ver(wikiModel.Ver)
                                            .ReferenceType("Wikis")
                                            .SiteId(wikiModel.SiteId)
                                            .Title(WikisUtility.TitleDisplayValue(
                                                siteSettings, wikiModel))
                                            .Subset(Jsons.ToJson(new WikiSubset(
                                                wikiModel, siteSettings))))
                                });
                        }
                    });
            }
        }
    }
}
