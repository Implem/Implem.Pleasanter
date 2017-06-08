# Implem.Pleasanter
## 概要
.NET製のアプリケーションPaaS。現場のマネジメントを快適化することで、生産性を向上し続ける仕組みを実現。商用、非商用を問わず無料で使えるオープンソース・ソフトウェア。  

オープンソースのアプリケーションPaaSというジャンルでデファクトスタンダードを目指します。プルリクエスト大歓迎。ご興味ある方はお気軽にご連絡ください。  
## 機能概要
プリザンターの機能の概要をアニメーションGIFを用いて説明します。  
http://qiita.com/Implem/items/39ef5be388f40aa21c04

## 利用ガイド
プリザンターのセットアップ手順及び、操作手順を説明します。  
https://github.com/Implem/Implem.Pleasanter/wiki

## デモサイト
下記のサイトでe-mailアドレスを登録するとプリザンターを試行することが出来ます。  
https://pleasanter.azurewebsites.net

## ダウンロード
セットアップモジュールのダウンロードサイトです。  
http://pleasanter.org

## ブログ
プリザンターの活用方法を紹介するブログです。  
https://implem.co.jp/category/blog/

## 動作イメージ
* テーブル管理
![default](https://cloud.githubusercontent.com/assets/12204265/19873886/e25c990e-a004-11e6-8e74-4d157e5fc2db.gif)

* カレンダー
![default](https://user-images.githubusercontent.com/17098267/26912816-ddbdcc48-4c51-11e7-9626-fe6e14864ec2.gif)

* クロス集計
![default](https://user-images.githubusercontent.com/17098267/26912817-ddc33a34-4c51-11e7-85f9-e4265a74b862.gif)

* ガントチャート
![default](https://user-images.githubusercontent.com/17098267/26912845-0672efe2-4c52-11e7-906a-247d2b3bc13c.gif)

* バーンダウンチャート
![default](https://user-images.githubusercontent.com/17098267/26912848-08ead8ca-4c52-11e7-8159-bb6d2184f84c.gif)

* 時系列チャート
![default](https://user-images.githubusercontent.com/17098267/26912851-0c1b82f6-4c52-11e7-9461-8efbfd6cfea4.gif)

* カンバン
![default](https://user-images.githubusercontent.com/17098267/26912853-0d61e2b8-4c52-11e7-8eb4-56feb7576d24.gif)

## 動作条件
|条件|Windows Server 2012 R2 / 2016|Windows 10|Microsoft Azure|
|:--|:--:|:--:|:--:|
|.NET Framework 4.5|導入済|導入済|-|
|IIS|◯|◯|-|
|ASP.NET 4.5|◯|◯|-|
|SQL Server 2012/2014/2016|◯|◯|-|
|Web Deploy v3.5|◯|◯|-|
|Microsoft Azure Web App|-|-|◯|
|Microsoft Azure SQL Database|-|-|◯|

## 機能一覧
| 項目               | 説明                                  |
|:-------------------|:--------------------------------------|
|[サイトメニュー](https://github.com/Implem/Implem.Pleasanter/wiki/サイト機能：サイトメニュー)|ファイルサーバのような階層構造|
|[サイト](https://github.com/Implem/Implem.Pleasanter/wiki/サイト機能：サイト)|情報の入れ物|
|[期限付きテーブル](https://github.com/Implem/Implem.Pleasanter/wiki/サイト機能：期限付きテーブル)|タスク管理など期限のあるデータを表形式で管理するテーブルの入れ物|
|[記録テーブル](https://github.com/Implem/Implem.Pleasanter/wiki/サイト機能：記録テーブル)|顧客リストなど期限の無いデータを表形式で管理するテーブルの入れ物|
|[Wiki](https://github.com/Implem/Implem.Pleasanter/wiki/サイト機能：Wiki)|マークダウン記法に対応したマニュアルやリンク集のページ|
|[サイト設定](https://github.com/Implem/Implem.Pleasanter/wiki/サイト機能：サイト設定)|サイトのカスタマイズ|
|[アクセス制御](https://github.com/Implem/Implem.Pleasanter/wiki/サイト機能：サイト設定：サイトのアクセス制御)|サイト単位に利用者の権限を設定|
|[メール](https://github.com/Implem/Implem.Pleasanter/wiki/データ管理：基本機能：メール)|テーブルからメールを送信|
|[分割](https://github.com/Implem/Implem.Pleasanter/wiki/データ管理：基本機能：分割)|テーブルを複数のテーブルに分割|
|[コメント](https://github.com/Implem/Implem.Pleasanter/wiki/データ管理：基本機能：コメント)|テーブルにコメントを追加|
|[変更履歴](https://github.com/Implem/Implem.Pleasanter/wiki/データ管理：基本機能：変更履歴)|テーブルの更新履歴の保存と参照|
|[インポート](https://github.com/Implem/Implem.Pleasanter/wiki/データ管理：基本機能：インポート)|テーブルをCSVファイルからインポート|
|[エクスポート](https://github.com/Implem/Implem.Pleasanter/wiki/データ管理：基本機能：エクスポート)|テーブルをCSVファイルにエクスポート|
|[カスタム項目](https://github.com/Implem/Implem.Pleasanter/wiki#%E3%82%AB%E3%82%B9%E3%82%BF%E3%83%A0%E9%A0%85%E7%9B%AE)|テーブルのカスタマイズ可能な入力フィールドの設定|
|[リンク](https://github.com/Implem/Implem.Pleasanter/wiki/データ管理：ビジネスロジック：リンク)|テーブルでサイト間のテーブルの親子関係を設定|
|[サマリ](https://github.com/Implem/Implem.Pleasanter/wiki/データ管理：ビジネスロジック：サマリ)|リンクしているテーブルの件数または数値フィールドの合計、平均、最大、最小をカスタムフィールドに格納|
|[計算式](https://github.com/Implem/Implem.Pleasanter/wiki/データ管理：ビジネスロジック：計算式)|テーブルで四則演算の結果をカスタムフィールドに格納|
|[スクリプト](https://github.com/Implem/Implem.Pleasanter/wiki/データ管理：ビジネスロジック：スクリプト)|カスタムJavaScript|
|[フィルタ](https://github.com/Implem/Implem.Pleasanter/wiki/データ管理：アウトプット：フィルタ)|テーブルのフィルタリング|
|[ソータ](https://github.com/Implem/Implem.Pleasanter/wiki/データ管理：アウトプット：ソータ)|一覧の並び替え|
|[ビューモード](https://github.com/Implem/Implem.Pleasanter/wiki/データ管理：アウトプット：ビューモード)|テーブルの表示形式を一覧、ガントチャート、バーンダウンチャート、時系列チャート、カンバンに切り替え|
|[レコード一覧](https://github.com/Implem/Implem.Pleasanter/wiki/データ管理：アウトプット：レコード一覧)|レコードの一覧表示|
|[集計](https://github.com/Implem/Implem.Pleasanter/wiki/データ管理：アウトプット：集計)|テーブルの件数または数値フィールドの合計、平均、最大、最小を分類毎に集計して表示|
|[スタイル](https://github.com/Implem/Implem.Pleasanter/wiki/データ管理：アウトプット：スタイル)|カスタムCSS|
|[通知](https://github.com/Implem/Implem.Pleasanter/wiki/データ管理：アウトプット：通知)|テーブルでテーブルの追加、変更、削除をSlackまたはメールで通知|
|[キーワード検索](https://github.com/Implem/Implem.Pleasanter/wiki/データ管理：アウトプット：キーワード検索)|テーブルやWikiの横断検索|
|[カレンダー](https://github.com/Implem/Implem.Pleasanter/wiki/ビューモードの種類：カレンダー)|テーブルのカレンダー表示|
|[クロス集計](https://github.com/Implem/Implem.Pleasanter/wiki/ビューモードの種類：クロス集計)|テーブルのクロス集計表示|
|[ガントチャート](https://github.com/Implem/Implem.Pleasanter/wiki/ビューモードの種類：ガントチャート)|テーブルのガントチャート表示|
|[バーンダウンチャート](https://github.com/Implem/Implem.Pleasanter/wiki/ビューモードの種類：バーンダウンチャート)|テーブルのバーンダウンチャート表示|
|[時系列チャート](https://github.com/Implem/Implem.Pleasanter/wiki/ビューモードの種類：時系列チャート)|テーブルのテーブルの件数または数値フィールドの合計、平均、最大、最小を面グラフで表示|
|[カンバン](https://github.com/Implem/Implem.Pleasanter/wiki/ビューモードの種類：カンバン)|テーブルの状況やカスタムフィールドの分類をカンバン表示|
|[認証](https://github.com/Implem/Implem.Pleasanter/wiki/システム機能：認証)|ローカル認証、LDAP認証|
|[マルチ言語](https://github.com/Implem/Implem.Pleasanter/wiki/システム機能：マルチ言語)|日英（拡張可能）|
|[マークダウン](https://github.com/Implem/Implem.Pleasanter/wiki/その他：マークダウン)|マークダウン記法でテキストをスタイリング|

## Auther
Implem Inc.  
<https://implem.co.jp>
