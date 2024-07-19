using Implem.Libraries.DataSources.SqlServer;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;

namespace Implem.PleasanterTest.Utilities
{
    public static class RdsUtilities
    {

        /// <summary>
        /// 指定したタイトルを持つIssueレコードのBodyを取得します。
        /// </summary>
        public static string GetIssueBody(Context context, string title)
        {
            return Repository.ExecuteScalar_string(
                context: context,
                statements: Rds.SelectIssues(
                    column: Rds.IssuesColumn()
                        .Issues_Body(),
                    join: [new SqlJoin(
                            tableBracket:"\"Sites\"",
                            joinExpression: "\"Sites\".\"SiteId\"=\"Issues\".\"SiteId\"")],
                    where: Rds.IssuesWhere()
                        .Title(title)
                        .Sites_TenantId(context.TenantId)));
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
