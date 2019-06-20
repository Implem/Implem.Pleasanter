![image](https://user-images.githubusercontent.com/12204265/48656589-f785b200-ea69-11e8-8278-3cf084ccbd27.png)

## プリザンター クロスプラットフォーム beta 版

プリザンター クロスプラットフォーム beta 版のデバッグへのご協力まことにありがとうございます。  
本プロダクトは開発途中のものであり、まだ多くの不具合が含まれております。不具合を発見された場合は issue にてお知らせください。  

技術概要資料(PDF)は[こちら](docs/20190308.pdf)です。

## 製品版の README
製品版（Windows 版）の README は[こちら](https://github.com/Implem/Implem.Pleasanter/blob/master/README.md)です。  
製品版（Windows 版）のリポジトリは[こちら](https://github.com/Implem/Implem.Pleasanter)です。

## ダウンロード
クロスプラットフォーム beta 版のセットアップモジュールはまだありません。  
ソースコードからビルドしてください。  
開発/ビルドは Windows で行います。  
Linux でビルド/実行する場合は、下記「Linux だけでビルド/実行する」を参照してください。

## 開発/ビルド環境を構成する（Windows）
* Visual Studio 2017 [[download](https://visualstudio.microsoft.com/ja/downloads/)]
* .NET Framework 4.7.2 [[download](https://dotnet.microsoft.com/download)]
* .NET Core 2.2 [[download](https://dotnet.microsoft.com/download)]

## ビルド/デバッグ実行をする（Windows）
ソースコード（ソリューション）を Visual Studio で開きビルド/デバッグ実行をします。  
実行可能なプロジェクト次の４つです。

| プロジェクト | 概要 | プラットフォーム |
|:-|:-|:-|
| Implem.CodeDefiner.NetCore | データベース構成ツール | クロスプラットフォーム（.NET Core） |
| Implem.CodeDefiner.NetFramework | データベース構成ツール | Windows（.NET Framework） |
| Implem.Pleasanter.NetCore | プリザンター | クロスプラットフォーム（.NET Core） |
| Implem.Pleasanter.NetFramework | プリザンター | Windows（.NET Framework） |

## 発行（publish）（Windowsで操作）

Visual Studio からクロスプラットフォーム（.NET Core）実行環境へ配置するバイナリの発行を行います。  

#### 作業対象プロジェクト

1. Implem.CodeDefiner.NetCore
1. Implem.Pleasanter.NetCore

#### 発行の設定
| 項目 | 設定 |
|:-|:-|
| 発行方法 | ファイルシステム |
| 構成 | Release |
| ターゲットフレームワーク | .netcoreapp2.2 |
| 配置モード | フレームワーク依存 |
| ターゲットランタイム | ポータブル |

## 実行環境を構築する（Windowsの場合）
* .NET Framework 4.7.2 [[download](https://dotnet.microsoft.com/download)]
* .NET Core 2.2 [[download](https://dotnet.microsoft.com/download)]
* SQL Server 2017 [[download](https://www.microsoft.com/ja-jp/sql-server/sql-server-downloads)]

## 実行環境を構築する（Linuxの場合）
* .NET Core 2.2 [[download](https://dotnet.microsoft.com/download)]  または [[パッケージ管理システム](https://dotnet.microsoft.com/download/linux-package-manager/rhel/sdk-2.2.105)]
* SQL Server 2017 [[download](https://www.microsoft.com/ja-jp/sql-server/sql-server-downloads)] または [[パッケージ管理システム](https://docs.microsoft.com/ja-jp/sql/linux/quickstart-install-connect-ubuntu?view=sql-server-linux-2017)]

* GDI+ のインストール

参考：  
CentOS
```
yum install -y epel-release
yum install -y libgdiplus
```
Ubuntu
```
apt-get install -y libgdiplus
```

## SQL Server を構成する

1. SQL Server をインストールした Windows または Linux へソースコードおよび発行したバイナリをコピーします。  
※フォルダ構成を維持したままコピーしてください。
1. Implem.CodeDefiner.NetCore プロジェクトの発行先フォルダへ移動します。  
通常は Implem.CodeDefiner.NetCore\bin\Debug\netcoreapp2.2\publish\ または Implem.CodeDefiner.NetCore\bin\Release\netcoreapp2.2\publish\ です。
1. SQL Server の接続情報を書き換えます。  
接続情報が記載されたファイルは `Implem.Pleasanter\App_Data\Parameters\Rds.json` です。下記行のパスワード部分をSAのパスワード(※)に書き換えてください。  
```"SaConnectionString": "Server=(local);Database=master;UID=sa;PWD=********;Connection Timeout=30;",  ```  
※ SAのパスワードはSQL Serverのインストール時に設定したSQL Serverのシステム管理者のパスワードです。
1. 次のコマンドで SQL Server を構築します。
```
dotnet Implem.CodeDefiner.NetCore.dll _rds
```

## プリザンターの配置

1. プリザンターを実行する Windows または Linux へ Implem.Pleasanter.NetCore プロジェクトから発行したバイナリをコピーします。  
通常は Implem.Pleasanter.NetCore\bin\Debug\netcoreapp2.2\publish\ または \Implem.Pleasanter.NetCore\bin\Release\netcoreapp2.2\publish\ です。

## 実行
次のコマンドでプリザンターを実行します。
```
dotnet Implem.Pleasanter.NetCore.dll
```

#### ブラウザでアクセス
```
http://localhost:5000/
```

## Linux だけでビルド/実行する

1. 上記「実行環境を構築する（Linuxの場合）」で .NET Core と SQL Server をインストールします。
1. ダウンロードしたプリザンターのソースファイル一式を Linux 上にコピーします。
1. ソースファイルの中の cmdnetcore ディレクトリへ移動します。
1. ```build.sh``` を実行しビルドを行います。
1. ```codedefiner.sh``` を実行し SQL Server を構成します。
1. ```pleasanter.sh``` を実行しプリザンターを実行します。
1. ブラウザで ```http://localhost:5000/``` へアクセスします。

## 常駐プログラムのスクリプトの動作環境を構築する（Linux）
リマインダー機能やActive Directoryのユーザ情報を同期する場合は、スクリプトの動作環境として Python3 系をインストールします。  
Linux に既に Python3 系がインストールされている場合は作業は不要です。  

参考：  
CentOS [[パッケージ管理システム](https://www.softwarecollections.org/en/scls/rhscl/rh-python36/)]

## プリザンターのリマインダー機能を有効化する（Linux）

リマインダーを使用すると指定した時間にタスクの状況などを通知することが可能です。リマインダーを動作させるためには、常駐プログラムによりプリザンターの URL にリクエストを送信し続けます。
### **スクリプトの配置**
---
[Reminder.py](https://github.com/Implem/Implem.Pleasanter.NetCore/tree/master/Implem.Pleasanter.NetCore/Tools/Reminder.py)を /opt/pleasanter-tools 等任意のディレクトリにコピーします。[Reminder.py](https://github.com/Implem/Implem.Pleasanter.NetCore/tree/master/Implem.Pleasanter.NetCore/Tools/Reminder.py) を vim 等の任意のエディターで編集し http://localhost/ の部分を、クライアントからアクセス可能なURLに変更します。localhost のままでもリマインダーを動かす事ができますが、送信されたメールに記載されるレコードの URL が http://localhost/ となり、クライアントからアクセスできません。
### **cron の設定**
---
スクリプトを cron に登録します。以下のコマンドを参考に設定を行ってください。  
   
1. crontab を編集します。  
crontab -e

1. crontab をサーバの起動時にリマインダー機能を実行するように設定します。  
@reboot python3 /opt/pleasanter-tools/Reminder.py

1. スクリプトを実行します。次回サーバ起動時には自動実行されるため初回のみ必要です。  
python3 /opt/pleasanter-tools/Reminder.py


## データベースバックアップスクリプトの動作環境を構成する（Linux）
データベースバックアップスクリプト ```Implem.Pleasanter.NetCore/Tools/DbBackup.py``` を使用する場合は、Microsoft ODBC Driver 17 for SQL Server をインストールします。

* Microsoft ODBC Driver 17 for SQL Server [[パッケージ管理システム](https://docs.microsoft.com/ja-jp/sql/connect/odbc/linux-mac/installing-the-microsoft-odbc-driver-for-sql-server?view=sql-server-2017)]

## デバッグする

## issue を立てる

プリザンター クロスプラットフォーム beta 版のデバッグへのご協力まことにありがとうございます。  
本プロダクトは開発途中のものであり、まだ多くの不具合が含まれております。不具合を発見された場合は issue にてお知らせください。

## キャラクター
HAYATO  
![HAYATO](https://user-images.githubusercontent.com/12204265/54112024-9d4d9a00-4428-11e9-87a0-1423e403f300.png)
