using ModelContextProtocol.Server;
using System.ComponentModel;

namespace Implem.Pleasanter.MCP.Resources
{
    [McpServerResourceType]
    public static class ItemFieldsResource
    {
        [McpServerResource(
            Name = "item-fields",
            UriTemplate = "resource://pleasanter/specs/item-fields")]
        [Description("レコード操作で使用可能な項目名の一覧と JSON 構造")]
        public static string GetItemFieldsSpec()
        {
            return """
レコード項目のJSON仕様

標準項目: Title(string,タイトル), Body(string,内容), Status(int,状況コード), Manager(int,管理者のユーザーID), Owner(int,担当者のユーザーID), StartTime(string,開始日時), CompletionTime(string,完了日時)

拡張項目(Hash形式,サイトで有効な項目のみ使用可):
ClassHash(ClassA等,string,分類), NumHash(NumA等,number,数値), DateHash(DateA等,string,日付), DescriptionHash(DescriptionA等,string,説明), CheckHash(CheckA等,bool,チェック), AttachmentsHash(AttachmentsA等,array,添付ファイル)

JSON例 - 標準項目: {"Title":"タイトル","Body":"内容","Status":200}
拡張項目: {"ClassHash":{"ClassA":"分類値"},"NumHash":{"NumA":100},"DateHash":{"DateA":"2026/01/15"},"CheckHash":{"CheckA":true}}
複合: {"Title":"タイトル","Status":200,"ClassHash":{"ClassA":"完了","ClassB":"高"},"NumHash":{"NumA":500}}

ManagerとOwnerの区別: Manager=管理者, Owner=担当者。曖昧な表現時はユーザーに確認。どちらもユーザーID(数値)で指定。0で未設定。

注意: JSON項目キーは英語名(Title, ClassHash等)を使用。分類項目はCreateUpdateItemJson経由なら日本語表示値→内部コード自動変換あり。直接UpdateItemに渡す場合は内部コード指定が必要。
""";
        }
    }
}
