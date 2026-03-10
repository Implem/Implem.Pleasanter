using ModelContextProtocol.Server;
using System.ComponentModel;

namespace Implem.Pleasanter.MCP.Resources
{
    [McpServerResourceType]
    public static class ToolCapabilitiesResource
    {
        [McpServerResource(
            Name = "tool-capabilities",
            UriTemplate = "resource://pleasanter/specs/tool-capabilities")]
        [Description("Pleasanter MCP ツールの全体概要と推奨ワークフロー")]
        public static string GetToolCapabilitiesSpec()
        {
            return """
Pleasanter MCP ツール概要

■ レコード操作(ItemsTool)
  GetItem: レコードID指定で詳細取得
  GetItems: サイトID指定でレコード一覧取得(viewJsonで絞り込み、offsetでページング)
  CreateUpdateItemJson: 更新用JSON生成(日本語表示値→内部コード自動変換)
  UpdateItem: レコード更新(CreateUpdateItemJsonの出力を渡す)

■ サイト操作(SitesTool)
  GetSiteIdByTitle: サイト名→サイトID検索
  GetSite: サイト設定取得(項目定義・選択肢等の確認)

■ ユーザー操作(UsersTool)
  GetUserIdByName: ユーザー名→ユーザーID検索
  GetUsers: ユーザー一覧取得(viewJsonで絞り込み)

■ ビュー操作(ViewsTool)
  CreateViewJson: 検索条件JSON生成(GetItems/GetUsersのviewJsonに使用)
  AddView/GetView/GetViewIdByViewName/UpdateView/CopyView/DeleteView: 保存ビュー管理

■ メール送信(OutgoingMailsTool)
  send_email: アイテムに関連するメール送信

■ 推奨ワークフロー
  検索: GetSiteIdByTitle → CreateViewJson → GetItems
  更新: GetItem → CreateUpdateItemJson → UpdateItem
  ビュー保存: CreateViewJson → AddView

■ 検索モード(searchType)
  ExactMatch(完全一致,デフォルト), PartialMatch(部分一致), ForwardMatch(前方一致)
  無効値はExactMatchにフォールバック。

■ リソース一覧
  resource://pleasanter/specs/view-json - CreateViewJsonのcolumnFilterHash等の詳細仕様
  resource://pleasanter/specs/item-fields - レコード項目のJSON構造
  resource://pleasanter/specs/choices-pattern - 選択肢の保存値・表示値の仕様
  resource://pleasanter/specs/site-settings - GetSite応答の読み取り仕様
  resource://pleasanter/specs/paging - GetItemsのページング仕様
""";
        }
    }
}
