using Implem.Libraries.DataSources.SqlServer;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using System.Collections.Generic;
using System.Data;

namespace Implem.PleasanterTest.Utilities
{
    public static class RdsUtilities
    {

        /// <summary>
        /// 指定したタイトルを持つIssueレコードのDataRowを取得
        /// </summary>
        public static IEnumerable<DataRow> SelectIssueRows(Context context, string siteTitle, SqlWhereCollection where = null)
        {
            if (where == null) where = Rds.IssuesWhere();
            var dataset = Repository.ExecuteDataSet(
                context: context,
                statements: Rds.SelectIssues(
                    column: Rds.IssuesDefaultColumns(),
                    join: [new SqlJoin(
                            tableBracket:"\"Sites\"",
                            joinExpression: "\"Sites\".\"SiteId\"=\"Issues\".\"SiteId\"")],
                    where: where
                        .Sites_Title(siteTitle)
                        .Sites_TenantId(context.TenantId)));
            foreach (DataRow row in dataset.Tables[0].Rows)
            {
                yield return row;
            }
        }

        /// <summary>
        /// 指定したタイトルを持つIssueレコードのDataRowを取得
        /// </summary>
        public static IEnumerable<DataRow> SelectResultRows(Context context, string siteTitle, SqlWhereCollection where = null)
        {
            if (where == null) where = Rds.ResultsWhere();
            var dataset = Repository.ExecuteDataSet(
                context: context,
                statements: Rds.SelectResults(
                    column: Rds.ResultsDefaultColumns(),
                    join: [new SqlJoin(
                            tableBracket:"\"Sites\"",
                            joinExpression: "\"Sites\".\"SiteId\"=\"Results\".\"SiteId\"")],
                    where: where
                        .Sites_Title(siteTitle)
                        .Sites_TenantId(context.TenantId)));
            foreach (DataRow row in dataset.Tables[0].Rows)
            {
                yield return row;
            }
        }

        /// <summary>
        /// 指定したタイトルを持つサイト数を取得します。
        /// </summary>
        public static int SitesCount(Context context, string siteTitle)
        {
            return Repository.ExecuteScalar_int(
                context: context,
                statements: Rds.SelectSites(
                    column: Rds.SitesColumn().SitesCount(),
                    where: Rds.SitesWhere().Title(siteTitle).TenantId(context.TenantId)));
        }

        /// <summary>
        /// 指定したタイトルを持つ期限付きテーブルの数を取得します。
        /// </summary>
        public static int IssuesCount(Context context, string title)
        {

            return Repository.ExecuteScalar_int(
                context: context,
                statements: Rds.SelectIssues(
                    column: Rds.IssuesColumn().IssuesCount(),
                    join: [new SqlJoin(
                            tableBracket:"\"Sites\"",
                            joinExpression: "\"Sites\".\"SiteId\"=\"Issues\".\"SiteId\"")],
                    where: Rds.IssuesWhere()
                        .Title(title)
                        .Sites_TenantId(context.TenantId)));
        }

        /// <summary>
        /// 指定したタイトルを持つ記録テーブルの数を取得します。
        /// </summary>
        public static int ResultsCount(Context context, string title)
        {
            return Repository.ExecuteScalar_int(
                context: context,
                statements: Rds.SelectResults(
                    column: Rds.ResultsColumn().ResultsCount(),
                    join: [new SqlJoin(
                            tableBracket:"\"Sites\"",
                            joinExpression: "\"Sites\".\"SiteId\"=\"Results\".\"SiteId\"")],
                    where: Rds.ResultsWhere()
                        .Title(title)
                        .Sites_TenantId(context.TenantId)));
        }
    }
}
