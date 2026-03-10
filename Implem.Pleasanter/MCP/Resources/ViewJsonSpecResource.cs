using ModelContextProtocol.Server;
using System.ComponentModel;

namespace Implem.Pleasanter.MCP.Resources
{
    [McpServerResourceType]
    public static class ViewJsonSpecResource
    {
        [McpServerResource(
            Name = "view-json-spec",
            UriTemplate = "resource://pleasanter/specs/view-json")]
        [Description("CreateViewJson の columnFilterHash 等の詳細仕様")]
        public static string GetViewJsonSpec()
        {
            return """
CreateViewJsonパラメータ詳細仕様

columnFilterHash: キー=列名(日本語表示名可), 値=検索値の配列。同一列複数値=OR検索, 異なる列=AND検索。
基本例: {"状況":["実施中"]}, 複数値OR: {"状況":["実施中","完了"]}, 自分: {"担当者":["Own"]}
特殊値: 文字列未入力=" "(半角スペース), ドロップダウン/日付未設定="\t"(タブ文字)
数値範囲検索: "開始値,終了値" 例:{"数値A":["5,10"]} ※個別値列挙不可
日付範囲検索: "開始日時,終了日時" フォーマット:"yyyy/MM/dd H:mm:ss"(M,dはゼロ埋め不要) 終了日時は"23:59:59.997"で当日含む
例(2026年1月): {"開始":["2026/01/01 0:00:00,2026/01/31 23:59:59.997"]}
複数期間OR: {"開始":["2026/01/01 0:00:00,2026/01/31 23:59:59.997","2026/02/01 0:00:00,2026/02/28 23:59:59.997"]}
固定キーワード: 今日="Today", 今月="ThisMonth", 今年="ThisYear"

columnFilterSearchTypes: ExactMatch(完全一致), PartialMatch(部分一致), ForwardMatch(前方一致)。複数文字列OR用: PartialMatchMultiple/ExactMatchMultiple/ForwardMatchMultiple。選択肢付き分類項目の複数値ORはcolumnFilterHash配列指定だけでOK。

columnFilterNegatives: 列名を指定(日本語表示名可)。プリセットフィルタ否定: ViewFilters_Incomplete, ViewFilters_Own, ViewFilters_NearCompletionTime, ViewFilters_Delay, ViewFilters_Overdue, ViewFilters_Search

columnSorterHash: キー=列名(日本語表示名可), 値="asc"|"desc" 例:{"期限":"desc","状況":"asc"}

gridColumns: 日本語表示名使用可。親テーブル:"リンク列名~親サイトID,取得項目名"
""";
        }
    }
}
