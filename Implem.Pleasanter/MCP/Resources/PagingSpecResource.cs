using ModelContextProtocol.Server;
using System.ComponentModel;

namespace Implem.Pleasanter.MCP.Resources
{
    [McpServerResourceType]
    public static class PagingSpecResource
    {
        [McpServerResource(
            Name = "paging-spec",
            UriTemplate = "resource://pleasanter/specs/paging")]
        [Description("GetItems のページング仕様（offset によるページ取得）")]
        public static string GetPagingSpec()
        {
            return """
GetItems ページング仕様

レスポンス項目: Offset(現在のスキップ件数), PageSize(1ページ件数,デフォルト200), TotalCount(全件数)

ページ取得: offset=0で1〜200件, offset=200で201〜400件, Nページ目はoffset=(N-1)×PageSize

全件取得判定: Offset+PageSize>=TotalCountで完了。offset=0から開始しPageSizeずつ加算して繰り返す。

TotalCount超のoffset指定時: エラーにならず空配列(0件)返却。TotalCountは実件数が返る。

注意: offsetはスキップ件数(ページ番号ではない)。デフォルトPageSizeは200件。
""";
        }
    }
}
