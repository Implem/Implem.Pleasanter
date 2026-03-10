using ModelContextProtocol.Server;
using System.ComponentModel;

namespace Implem.Pleasanter.MCP.Resources
{
    [McpServerResourceType]
    public static class ChoicesPatternResource
    {
        [McpServerResource(
            Name = "choices-pattern",
            UriTemplate = "resource://pleasanter/specs/choices-pattern")]
        [Description("選択肢一覧の記述パターンと保存値・表示値の仕様")]
        public static string GetChoicesPatternSpec()
        {
            return """
選択肢一覧の記述パターン仕様

パターン一覧:
[1] 単純文字列 例:"要件定義" → 保存値=表示値="要件定義" → filter:{"ClassA":"要件定義"}
[2] 保存値,表示値 例:"AC7459,製品A" → filter:{"ClassB":"AC7459"}(保存値で指定)
[3] 保存値,表示値,短縮名,クラス 例:"100,未着手,未,status-new" → filter:{"Status":"100"}(保存値で指定)
[4] エスケープ付き 例:"1\,000円まで" → カンマを含む表示値

重要ルール: columnFilterHash/UpdateItemには必ず「保存値」を指定。表示値で指定すると0件or不整合になる。
保存値の確認方法: GetSiteで選択肢定義を取得。ユーザーが表示値で指示した場合は保存値に変換。

状況項目(Status)の一般的な保存値: 100=未着手, 150=準備, 200=実施中, 300=レビュー, 900=完了, 910=保留 ※実際の値はサイト定義により異なる

Manager/Ownerは選択肢パターン対象外(ユーザーIDで指定)。詳細はresource://pleasanter/specs/item-fieldsを参照。
""";
        }
    }
}
