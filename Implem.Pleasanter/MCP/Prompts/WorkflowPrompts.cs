using ModelContextProtocol.Server;
using System.ComponentModel;
using Microsoft.Extensions.AI;

namespace Implem.Pleasanter.MCP.Prompts
{
    [McpServerPromptType]
    public static class WorkflowPrompts
    {
        [McpServerPrompt(Name = "assign-user-to-record")]
        [Description("対象ユーザー名で担当者・管理者を設定")]
        public static ChatMessage AssignUserToRecord(
            [Description("担当者・管理者の設定リクエスト（対象ユーザー名・レコードID等）")]
                string request)
        {
            return new ChatMessage(
                ChatRole.User,
                $$"""
リクエスト: {{request}}

## 手順
1. GetUserIdByName で対象ユーザーのIDを特定（複数ヒット時はユーザーに確認、未検出時は中断）
2. GetItem で現在のレコード内容を確認
3. CreateUpdateItemJson で Manager/Owner に対象ユーザーのIDを設定
4. 更新内容をユーザーに確認後、UpdateItem で実行

## 注意
- Manager = 管理者、Owner = 担当者（詳細は resource://pleasanter/specs/item-fields 参照）
- 対象ユーザー未検出・複数ヒット時は確認なしに更新しない
""");
        }

        [McpServerPrompt(Name = "manage-views")]
        [Description("ビューの更新・削除・コピーなどの管理操作")]
        public static ChatMessage ManageViews(
            [Description("ビュー管理リクエスト（更新・削除・コピーの操作内容）")]
                string request)
        {
            return new ChatMessage(
                ChatRole.User,
                $$"""
リクエスト: {{request}}

## 手順
1. ビューID特定: ビュー名既知 → GetViewIdByViewName / ビューID不明 → GetSite でビュー一覧確認
2. 操作実行:
   - 更新: GetView で現在の設定を確認 → 変更箇所を反映した View JSON を CreateViewJson で生成 → UpdateView で上書き（名前変更時は viewName も指定）
   - 削除: 対象情報を提示 → ユーザー確認後 DeleteView（viewId=0は全削除、特に慎重に）
   - コピー: CopyView（コピー後の名前はコピー元と同じ）

## 注意
- 削除は復元不可、実行前に必ずユーザーに確認
- 新規作成の場合はAddViewで保存する手順をユーザーに案内
- 全ビュー削除時は現在のビュー数と全名を一覧表示して確認
""");
        }

        [McpServerPrompt(Name = "notify-overdue-records")]
        [Description("期限超過レコードを抽出して担当者に通知")]
        public static ChatMessage NotifyOverdueRecords(
            [Description("期限超過の通知リクエスト（サイト名またはサイトID・通知先等）")]
                string request)
        {
            return new ChatMessage(
                ChatRole.User,
                $$"""
リクエスト: {{request}}

## 手順
1. サイトID特定: サイトID指定済み → そのまま使用 / サイト名指定 → GetSiteIdByTitle で特定
2. CreateViewJson(overdue: true)で条件作成
3. GetItems で抽出（レスポンスのTotalCountとoffsetを比較し、未取得分があればoffsetを加算して次ページ取得。TotalCount以上になったら終了）
4. 0件なら通知不要で終了
5. サマリ作成 → ユーザーに宛先・件名・本文を確認後 send_email で送信

## 注意
- 0件時はメール送信しない
- 送信前に必ずユーザーに確認
- 複数担当者がいる場合は個別送信かユーザーに確認
""");
        }

        [McpServerPrompt(Name = "resolve-choice-values")]
        [Description("選択肢の表示値から保存値を解決してレコード操作")]
        public static ChatMessage ResolveChoiceValues(
            [Description("日本語表示値でのレコード操作リクエスト（フィルタ・更新等）")]
                string request)
        {
            return new ChatMessage(
                ChatRole.User,
                $$"""
リクエスト: {{request}}

## 手順
1. GetSite で選択肢定義を確認
2. パターン識別:
   - 単純文字列（例: "新規"）→ そのまま使用
   - "保存値,表示値" 形式（例: "100,新規"）→ 保存値 "100" を使用
3. フィルタ: columnFilterHashには保存値を指定（表示値では検索不可）
4. 更新:
   - 分類項目(ClassA〜Z): 表示値で指定可（CreateUpdateItemJsonが自動変換）
   - Status: 数値の保存値で指定（自動変換されない。例: 実施中 → 200）

## 注意
- フィルタ時は必ず保存値を使用
- 詳細は resource://pleasanter/specs/choices-pattern 参照
""");
        }

        [McpServerPrompt(Name = "save-view")]
        [Description("ビューを作成してサイトに保存")]
        public static ChatMessage SaveView(
            [Description("ビュー作成・保存リクエスト（検索条件・ビュー名等）")]
                string request)
        {
            return new ChatMessage(
                ChatRole.User,
                $$"""
リクエスト: {{request}}

## 手順
1. CreateViewJson で検索条件のView JSONを作成
2. AddView でサイトに保存（viewNameにわかりやすい名前を設定）

## 注意
- 「保存」「登録」「追加」が明示された場合のみ使用
- 単に「表示して」という要求の場合はGetItemsで検索・表示のみ行い、AddViewは呼ばない
- 既存ビューの更新・削除・コピーはmanage-viewsの手順（GetView/UpdateView/DeleteView/CopyView）で対応
""");
        }

        [McpServerPrompt(Name = "search-records")]
        [Description("サイトIDまたはサイト名からレコードを検索・表示")]
        public static ChatMessage SearchRecords(
            [Description("レコード検索リクエスト（サイトIDまたはサイト名・検索条件等）")]
                string request)
        {
            return new ChatMessage(
                ChatRole.User,
                $$"""
リクエスト: {{request}}

## 手順
1. サイトID特定: サイトID指定済み → そのまま使用 / サイト名指定 → GetSiteIdByTitle で特定（複数ヒット時はユーザーに確認）
2. CreateViewJson で検索条件作成
3. GetItems のviewJsonに渡して検索実行
4. 結果をユーザーに表示

## 注意
- CreateViewJson単独で終わりにしない（必ずGetItemsまで実行）
- AddViewは呼ばない（保存が明示された場合のみAddViewを使用）
- 200件超はoffsetで次ページ取得（TotalCountに達したら終了）
""");
        }

        [McpServerPrompt(Name = "search-users")]
        [Description("Pleasanterユーザーを検索・取得")]
        public static ChatMessage SearchUsers(
            [Description("ユーザーの検索リクエスト（名前・条件等）")]
                string request)
        {
            return new ChatMessage(
                ChatRole.User,
                $$"""
リクエスト: {{request}}

## 手順
1. 検索方法を選択:
   - ユーザー名で特定 → GetUserIdByName
   - 条件付き一覧取得 → CreateViewJson → GetUsers
   - メールアドレスも取得 → CreateViewJson(apiGetMailAddresses: true) → GetUsers
2. 結果をユーザーに表示

## 注意
- 複数ヒット時は一覧を提示してユーザーに確認
""");
        }

        [McpServerPrompt(Name = "send-email")]
        [Description("レコードに関連するメールを送信")]
        public static ChatMessage SendEmail(
            [Description("メール送信リクエスト（レコードID・宛先・本文等）")]
                string request)
        {
            return new ChatMessage(
                ChatRole.User,
                $$"""
リクエスト: {{request}}

## 手順
1. 対象レコードIDとreference（issues/results等）を特定（不明時はGetItemで確認）
2. 宛先（To/Cc/Bcc）・タイトル・本文を確認
3. ユーザーに送信内容を提示し確認後、send_email を実行

## 注意
- 誤送信防止のため、送信前に必ずユーザーに宛先・件名・本文を確認
- 複数宛先はカンマ区切り
- 対象レコードが見つからない場合は送信せず報告して終了
""");
        }

        [McpServerPrompt(Name = "update-records")]
        [Description("レコードを更新")]
        public static ChatMessage UpdateRecords(
            [Description("レコード更新リクエスト（レコードID・更新内容等）")]
                string request)
        {
            return new ChatMessage(
                ChatRole.User,
                $$"""
リクエスト: {{request}}

## 手順
1. GetItem で現在のレコード内容を確認
2. CreateUpdateItemJson で更新用JSON作成（分類項目は日本語表示値で指定可、自動変換される）
3. 更新内容をユーザーに確認
4. UpdateItem で更新実行

## 注意
- 更新前に必ずGetItemで現状確認
- CreateUpdateItemJson単独で終わりにしない（必ずUpdateItemまで実行）
- 一括更新時は対象件数を提示しユーザーの確認を得る
- 矛盾する値が指定された場合はどの値を採用するかユーザーに確認
""");
        }
    }
}
