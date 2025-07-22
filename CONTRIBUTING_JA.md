[English version is here](CONTRIBUTING.md)

# 開発環境の構築手順

## Windows と Visual Studioでの開発

WindowsでVisual Studioを使用して開発を行う場合の環境構築手順を説明します。

### 1. ツールのインストール

#### Visual Studio 2022のインストール

Visual Studio 2022を下記リンクからダウンロードし、インストールしてください。  
https://visualstudio.microsoft.com/downloads/

#### .NET 8 SDKのインストール

.NET SDK 8.0の最新バージョンを下記リンクからダウンロードし、インストールしてください。
https://dotnet.microsoft.com/download/dotnet/8.0

#### Node.jsのインストール

#####  バージョン管理ツール「VOLTA」からインストール（推奨）
- Node.jsのバージョン管理ツール「VOLTA」を導入済みの場合、ビルド時に自動的に必要なバージョンがインストールされます。  
- 新規でVOLTAを導入する場合は下記リンクからインストールし、最新のNodeを取得してください。  
https://volta.sh/  
`volta install node@latest`  
**※Node.js導入済み、もしくは別のバージョン管理ツールをお使いの場合は、予めアンインストールする必要があります。**

##### バージョンを指定してNode.jsをインストール
Node.js単体でインストールする場合は下記リンクからインストールしてください。  
https://nodejs.org/

必要なバージョンは下記ファイルの`volta`をご確認ください。
```
Implem.PleasanterFrontend\wwwroot\package.json
```
#### VSCodeのインストール (任意)
フロントエンドの開発を行う場合はVSCodeをインストールしてください。  
https://code.visualstudio.com/

### 2. データベースの準備
プリザンターは、SQLServer、PostgreSQL、MySQLのいずれかのデータベースを使用して動作します。
開発環境で利用するデータベースを選び、下記の手順に従ってインストールしてください。

- SQLServer: https://pleasanter.org/manual/install-sql-server2022-express
- PostgreSQL: https://pleasanter.org/manual/install-postgresql-on-windows
- MySQL: https://pleasanter.org/manual/install-mysql-on-windows

#### 接続文字列を環境変数に設定
下記例を参考にして、データベースの接続文字列を環境変数に設定してください。
- {SA用パスワード}には、データベースインストール時に設定した管理者（スーパーユーザー）のパスワードを設定してください。
- {Owner用パスワード}および{User用のパスワード}には、任意のパスワードを設定してください。ここに設定してパスワードで各ユーザーが作成されます。


SQLServerの場合の設定例

|環境変数名|値|
|--|--|
|Implem.Pleasanter_Rds_SQLServer_SaConnectionString|Server=(local);Database=master;UID=sa;PWD={SA用パスワード};Connection Timeout=30;|
|Implem.Pleasanter_Rds_SQLServer_OwnerConnectionString|Server=(local);Database=#ServiceName#;UID=#ServiceName#_Owner;PWD={Owner用パスワード};Connection Timeout=30;|
|Implem.Pleasanter_Rds_SQLServer_UserConnectionString|Server=(local);Database=#ServiceName#;UID=#ServiceName#_User;PWD={User用パスワード};Connection Timeout=30;|

PostgreSQLの場合の設定例

|環境変数名|値|
|--|--|
|Implem.Pleasanter_Rds_PostgreSQL_SaConnectionString|Server=localhost;Database=postgres;UID=postgres;PWD={SA用パスワード}|
|Implem.Pleasanter_Rds_PostgreSQL_OwnerConnectionString|Server=localhost;Database=#ServiceName#;UID=#ServiceName#_Owner;PWD={Owner用パスワード}|
|Implem.Pleasanter_Rds_PostgreSQL_UserConnectionString|Server=localhost;Database=#ServiceName#;UID=#ServiceName#_User;PWD={User用パスワード}|

MySQLの場合の設定例

|環境変数名|値|
|--|--|
|Implem.Pleasanter_Rds_MySQL_SaConnectionString|Server=localhost;Database=mysql;UID=root;PWD={SA用パスワード}|
|Implem.Pleasanter_Rds_MySQL_OwnerConnectionString|Server=localhost;Database=#ServiceName#;UID=#ServiceName#_Owner;PWD={Owner用パスワード}|
|Implem.Pleasanter_Rds_MySQL_UserConnectionString|Server=localhost;Database=#ServiceName#;UID=#ServiceName#_User;PWD={User用パスワード}|


接続文字列の設定方法の詳細は下記リンクを参照してください。  
https://pleasanter.org/manual/rds-json


### 3. リポジトリのクローン

[Implem.Pleasanter](https://github.com/Implem/Implem.Pleasanter)のリポジトリをクローンしてください。

```bash
git clone https://github.com/Implem/Implem.Pleasanter
```

### 4. フロントエンド開発環境のインストール
`Implem.PleasanterFrontend\wwwroot`ディレクトリで下記のコマンドを実行してください。
```
npm install
```

### 5. ソースコードのビルド

クローンしたリポジトリのルートディレクトリにあるImplem.Pleasanter.slnをVisual Studioで開き、ビルドしてください。

### 6. データベースの初期化

1. ソリューションエクスプローラでプロジェクト「Implem.CodeDefiner」を右クリックし、「スタートアッププロジェクトに設定」を選択してください。
1. デバッグのプロファイルで「Implem.CodeDefiner_rds」を選択してください。
1. Implem.CodeDefinerプロジェクトをデバッグ実行してください。
1. コンソールに表示されるメッセージに従って、データベースを初期化してください。

### 7. プリザンターのデバッグ実行

1. ソリューションエクスプローラでプロジェクト「Implem.Pleasanter」を右クリックし、「スタートアッププロジェクトに設定」を選択してください。
1. Implem.Pleasanterプロジェクトをデバッグ実行してください。

### 8. フロントエンドのデバッグ実行

1. VSCodeで作業フォルダ：`Implem.PleasanterFrontend\wwwroot`を開いてください。  
作業フォルダ展開後、VSCodeの拡張機能のインストール許可のポップアップが表示されるのでそれぞれインストールしてください。  
1. ターミナルで`npm run dev`を起動し、デバッグを開始してください。  
必要であればVSCodeのデバッグメニューからLunchを選びデバッグを実行してください。