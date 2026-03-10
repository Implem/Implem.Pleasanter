using ModelContextProtocol.Server;
using System.ComponentModel;

namespace Implem.Pleasanter.MCP.Resources
{
    [McpServerResourceType]
    public static class SiteSettingsResource
    {
        [McpServerResource(
            Name = "site-settings",
            UriTemplate = "resource://pleasanter/specs/site-settings")]
        [Description("GetSite の応答に含まれるサイト設定の読み取り仕様")]
        public static string GetSiteSettingsSpec()
        {
            return """
サイト設定(SiteSettings)の読み取り仕様
※MCPツールでのサイト設定更新は非対応。更新が必要な場合は画面操作を案内してください。

主なプロパティ: Title(string,サイト名), ReferenceType(string,Issues/Results等), ParentId(long,親サイトID), InheritPermission(long,権限継承元サイトID), SiteSettings(object,サイト設定)

SiteSettings内: Version(decimal), ReferenceType(string), GridColumns(string[],一覧表示項目), EditorColumnHash(object,編集画面項目)

GetSite応答例:
{"Title":"プロジェクト管理","ReferenceType":"Issues","ParentId":99999,"InheritPermission":99999,"SiteSettings":{"Version":1.017,"ReferenceType":"Issues","GridColumns":["IssueId","TitleBody","Status"],"EditorColumnHash":{"General":["IssueId","Title","Body","Status"]}}}
""";
        }
    }
}
