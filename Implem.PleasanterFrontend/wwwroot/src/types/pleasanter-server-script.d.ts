export {};

declare global {
    /**
     * .NETの配列です。インデックスアクセス、Length、for...ofを使用できます。
     * JavaScriptの配列メソッドを使用する場合はArray.fromで変換してください。
     */
    interface PleasanterHostArray<T> extends Iterable<T> {
        readonly [index: number]: T;
        readonly Length: number;
    }
    /**
     * .NETのListまたはIListです。インデックスアクセス、Count、for...ofを使用できます。
     * JavaScriptの配列メソッドを使用する場合はArray.fromで変換してください。
     */
    interface PleasanterHostList<T> extends Iterable<T> {
        readonly [index: number]: T;
        readonly Count: number;
    }
    /** 現在の要求、ユーザ、サイト、メッセージ、レスポンスに関する情報を扱います。 */
    const context: PleasanterServerScriptContext;
    /** コンソールおよびシステムログを出力します。 */
    const logs: PleasanterServerScriptLogs;
    /** 現在のサイト設定を参照または変更します。 */
    const siteSettings: PleasanterServerScriptSiteSettings;
    /** フィルタ、検索方法、ソート、拡張SQLの条件を設定します。 */
    const view: PleasanterServerScriptView;
    /** 一覧画面の件数や選択されたレコードを取得します。 */
    const grid: PleasanterServerScriptGrid;
    /** 項目の表示、CSS、HTML、入力制御、選択肢を変更します。 */
    const columns: PleasanterServerScriptColumns;
    /** ナビゲーションメニューやコマンドボタンを制御します。 */
    const elements: PleasanterServerScriptElements;
    /** 日時の範囲確認やBase64変換を行います。 */
    const utilities: PleasanterServerScriptUtilities;
    /** 現在処理しているレコードの値を参照または変更します。 */
    const model: PleasanterServerScriptRecord;
    /** 保存前のレコードの値を参照します。 */
    const saved: PleasanterServerScriptSavedRecord;
    /** レコードとサイトの取得、作成、更新、削除、集計を行います。 */
    const items: PleasanterServerScriptItems;
    /** ユーザ情報を取得します。 */
    const users: PleasanterServerScriptUsers;
    /** 組織情報を取得します。 */
    const depts: PleasanterServerScriptDepts;
    /** グループ情報を取得または更新します。 */
    const groups: PleasanterServerScriptGroups;
    /** 通知設定を取得または新規作成します。 */
    const notifications: PleasanterServerScriptNotifications;
    /** HTMLのhidden要素に出力する情報を追加します。 */
    const hidden: PleasanterServerScriptHidden;
    /** 外部のHTTPエンドポイントへリクエストを送信します。 */
    const httpClient: PleasanterServerScriptHttpClient;
    /** CSV、JSON、ファイル操作を提供するサーバスクリプト用オブジェクトです。 */
    const $ps: PleasanterServerScriptPs;
    /** API実行を許可した拡張SQLを実行します。 */
    const extendedSql: PleasanterServerScriptExtendedSql;
    /** クライアントに返すレスポンスを制御します。 */
    const responses: PleasanterServerScriptResponses;

    /** メッセージ欄のCSSクラス名です。既定の4種のほか、独自のCSSクラス名も指定できます。 */
    type PleasanterMessageCss = 'alert-information' | 'alert-warning' | 'alert-success' | 'alert-error' | (string & {});
    /** クライアントレスポンスの操作種別です。 */
    type PleasanterResponseMethod = 'ReplaceAll' | 'Set' | 'Href' | (string & {});
    /** ログの種類です。10:Info、50:Warning、60:UserError、80:SystemError、90:Exceptionです。 */
    type PleasanterLogType = 10 | 50 | 60 | 80 | 90;
    /** 要素の表示状態です。0:標準、1:無し、2:無効、3:非表示です。 */
    type PleasanterDisplayType = 0 | 1 | 2 | 3;
    /** 通知種別です。1:Mail、2:Slack、3:ChatWork、4:Line、5:LineGroup、6:Teams、7:RocketChat、8:InCircleです。 */
    type PleasanterNotificationType = 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8;
    /** 項目の検索方法です。 */
    type PleasanterViewSearchType =
        | 'PartialMatch'
        | 'ExactMatch'
        | 'ForwardMatch'
        | 'PartialMatchMultiple'
        | 'ExactMatchMultiple'
        | 'ForwardMatchMultiple'
        | (string & {});
    /** ソート順です。'asc':昇順、'desc':降順、'release':解除です。 */
    type PleasanterViewSorterOrder = 'asc' | 'desc' | 'release' | (string & {});

    interface PleasanterServerScriptContext {
        /** 複数のサーバスクリプト間でデータを共有します。 */
        readonly UserData: Record<string, unknown>;
        /** URLのクエリパラメータを取得します。 */
        readonly QueryStrings: Record<string, unknown>;
        /** 送信されたフォーム情報を取得します。 */
        readonly Forms: Record<string, unknown>;
        /** フォームに入力された未加工の情報です。 */
        readonly FormStringRaw: string;
        /** フォームに入力された情報です。 */
        readonly FormString: string;
        /** Ajaxリクエストの場合はtrueです。 */
        readonly Ajax: boolean;
        /** モバイル表示の場合はtrueです。 */
        readonly Mobile: boolean;
        /** アプリケーションパスです。 */
        readonly ApplicationPath: string;
        /** 現在の絶対URIです。 */
        readonly AbsoluteUri: string;
        /** 現在の絶対パスです。 */
        readonly AbsolutePath: string;
        /** 現在のURLです。 */
        readonly Url: string;
        /** 直前の要求元URLです。 */
        readonly UrlReferrer: string;
        /** コントローラ名です。 */
        readonly Controller: string;
        /** URLのクエリ文字列です。 */
        readonly Query: string;
        /** アクション名です。 */
        readonly Action: string;
        /** テナントIDです。 */
        readonly TenantId: number;
        /**
         * サイトIDです。
         *
         * @remarks JavaScriptのNumber.MAX_SAFE_INTEGERを超えるIDは、精度が失われる可能性があります。
         */
        readonly SiteId: number;
        /**
         * レコードIDです。
         *
         * @remarks JavaScriptのNumber.MAX_SAFE_INTEGERを超えるIDは、精度が失われる可能性があります。
         */
        readonly Id: number;
        /** ログインユーザが所属するグループIDのコレクションです。 */
        readonly Groups: PleasanterHostList<number>;
        /** テナントのタイトルです。 */
        readonly TenantTitle: string;
        /** サイトのタイトルです。 */
        readonly SiteTitle: string;
        /** レコードのタイトルです。 */
        readonly RecordTitle: string;
        /** 組織IDです。 */
        readonly DeptId: number;
        /** ユーザIDです。 */
        readonly UserId: number;
        /** ログインIDです。 */
        readonly LoginId: string;
        /** 設定言語です。 */
        readonly Language: string;
        /** タイムゾーンです。 */
        readonly TimeZoneInfo: string;
        /** 操作中のユーザが特権ユーザの場合はtrueです。 */
        readonly HasPrivilege: boolean;
        /** APIのバージョンです。 */
        readonly ApiVersion: number;
        /** APIのリクエスト内容です。 */
        readonly ApiRequestBody: string;
        /** リクエストデータです。 */
        readonly RequestDataString: string;
        /** レスポンスのContent-Typeです。 */
        readonly ContentType: string;
        /** 実行中のサーバスクリプトに関する情報です。 */
        readonly ServerScript: PleasanterServerScriptInformation;
        /** 要求元コントロールのIDです。 */
        readonly ControlId: string;
        /** 現在のサーバスクリプトの条件名です。 */
        readonly Condition: string;
        /**
         * 記録テーブルはResults、期限付きテーブルはIssuesです。
         *
         * @since 1.4.17.0
         */
        readonly ReferenceType: string;
        /**
         * 現在のHTTPメソッドです。
         *
         * @since 1.4.17.0
         */
        readonly HttpMethod: string;
        /**
         * ブラウザ画面の下部にメッセージを表示します。複数回呼び出すと複数のメッセージを表示します。
         *
         * @param message 表示するメッセージ。
         * @param css メッセージ欄のCSSクラス名。省略時は'alert-information'（青）です。'alert-warning'（黄）、'alert-success'（緑）、'alert-error'（赤）や独自のクラスも指定できます。
         *
         * @example
         * if (context.DeptId === 3 && model.Status === 900) {
         *     context.AddMessage('条件に該当しました。', 'alert-information');
         * }
         *
         * @see https://pleasanter.org/ja/manual/server-script-context-add-message
         */
        AddMessage(message: string, css?: PleasanterMessageCss): void;
        /**
         * 任意のクライアントレスポンスを返します。指定した画面要素の置き換えやフォームへの値の格納などを行います。
         *
         * @param method レスポンスの操作種別。'ReplaceAll'（targetの要素をvalueで置き換え）、'Set'（フォームに値を格納）、'Href'（遷移先URLを指定）などを指定します。
         * @param target 対象要素のID。
         * @param value 置き換える要素や格納する値。
         *
         * @example
         * // 分類Cのフィールドを赤字のdivで置き換える
         * context.AddResponse('ReplaceAll', '#Results_ClassCField', '<div class="field-normal" style="color:red">Pleasanter</div>');
         *
         * @example
         * // 数値A項目に123を格納する
         * context.AddResponse('Set', 'NumA', 123);
         *
         * @see https://pleasanter.org/ja/manual/server-script-context-add-response
         */
        AddResponse(method: PleasanterResponseMethod, target?: string, value?: unknown): void;
        /**
         * レコードの作成前・更新前・削除前の処理をキャンセルし、エラーメッセージを画面に表示します。
         *
         * @param message 表示するエラーメッセージ。
         *
         * @remarks サーバスクリプトの条件が「作成前」「更新前」「削除前」の場合に有効です。呼び出してもサーバスクリプト自体は止まりませんが、クライアントへはエラーメッセージのみが返り、以降のフォームへの変更は反映されません。
         *
         * @example
         * if (context.DeptId === 3 && model.Status === 900) {
         *     context.Error('条件に該当したため、更新をキャンセルしました。');
         * }
         *
         * @see https://pleasanter.org/ja/manual/server-script-context-error
         */
        Error(message: string): void;
        /**
         * サーバスクリプトのログをブラウザのコンソールに出力します。サーバ側で出力した内容をクライアントへ送り、ブラウザのコンソールに表示します。
         *
         * @param message コンソールに表示する内容。文字列以外の値も指定できます。
         *
         * @example
         * try {
         *     context.Log('処理を開始しました。');
         * } catch (e) {
         *     context.Log(e.stack);
         * }
         *
         * @see https://pleasanter.org/ja/manual/server-script-context-log
         */
        Log(message: unknown): void;
        /**
         * ブラウザを指定したURLへ遷移させます。
         *
         * @param url 遷移先のURL。
         *
         * @remarks サーバスクリプトの条件が「画面表示の前」の場合のみ使用できます。
         *
         * @example
         * context.Redirect('http://www.example.com/');
         *
         * @see https://pleasanter.org/ja/manual/server-script-context-redirect
         */
        Redirect(url: string): void;
        /**
         * フォームに値を格納します。context.AddResponseのmethodに'Set'を指定した場合と同じ動作をします。
         *
         * @param target 対象要素のID。
         * @param value 格納する値。
         *
         * @example
         * // 数値A項目に123を格納する（context.AddResponse('Set', 'NumA', 123)と同じ）
         * context.ResponseSet('NumA', 123);
         *
         * @see https://pleasanter.org/ja/manual/server-script-context-response-set
         */
        ResponseSet(target?: string, value?: unknown): void;
    }
    interface PleasanterServerScriptInformation {
        /** サーバスクリプトが実行された深さです。 */
        readonly ScriptDepth: number;
    }
    interface PleasanterServerScriptLogs {
        /**
         * 指定した種類でシステムログを出力します。
         *
         * @param type ログの種類。10:Info、50:Warning、60:UserError、80:SystemError、90:Exceptionを指定します。
         * @param message 出力する内容。
         * @param method 呼び出し元のメソッド名。省略可能で、既定は空文字です。
         * @param console ブラウザの開発者ツールのコンソールに表示するかどうか。省略可能で、既定はtrueです。
         * @param syslogs システムログに出力するかどうか。省略可能で、既定はtrueです。
         * @returns 出力できた場合はtrue、できなかった場合はfalse。
         *
         * @example
         * logs.Log(50, '警告メッセージ');
         *
         * @see https://pleasanter.org/ja/manual/server-script-logs-log
         */
        Log(type: PleasanterLogType, message: unknown, method?: string, console?: boolean, syslogs?: boolean): boolean;
        /**
         * ログの種類をInfoとしてシステムログを出力します。
         *
         * @param message 出力する内容。
         * @param method 呼び出し元のメソッド名。省略可能で、既定は空文字です。
         * @param console ブラウザの開発者ツールのコンソールに表示するかどうか。省略可能で、既定はtrueです。
         * @param syslogs システムログに出力するかどうか。省略可能で、既定はtrueです。
         * @returns 出力できた場合はtrue、できなかった場合はfalse。
         *
         * @example
         * logs.LogInfo('処理を開始しました。');
         *
         * @see https://pleasanter.org/ja/manual/server-script-logs-log-info
         */
        LogInfo(message: unknown, method?: string, console?: boolean, syslogs?: boolean): boolean;
        /**
         * ログの種類をWarningとしてシステムログを出力します。
         *
         * @param message 出力する内容。
         * @param method 呼び出し元のメソッド名。省略可能で、既定は空文字です。
         * @param console ブラウザの開発者ツールのコンソールに表示するかどうか。省略可能で、既定はtrueです。
         * @param syslogs システムログに出力するかどうか。省略可能で、既定はtrueです。
         * @returns 出力できた場合はtrue、できなかった場合はfalse。
         *
         * @see https://pleasanter.org/ja/manual/server-script-logs-log-warning
         */
        LogWarning(message: unknown, method?: string, console?: boolean, syslogs?: boolean): boolean;
        /**
         * ログの種類をUserErrorとしてシステムログを出力します。
         *
         * @param message 出力する内容。
         * @param method 呼び出し元のメソッド名。省略可能で、既定は空文字です。
         * @param console ブラウザの開発者ツールのコンソールに表示するかどうか。省略可能で、既定はtrueです。
         * @param syslogs システムログに出力するかどうか。省略可能で、既定はtrueです。
         * @returns 出力できた場合はtrue、できなかった場合はfalse。
         *
         * @see https://pleasanter.org/ja/manual/server-script-logs-log-user-error
         */
        LogUserError(message: unknown, method?: string, console?: boolean, syslogs?: boolean): boolean;
        /**
         * ログの種類をSystemErrorとしてシステムログを出力します。
         *
         * @param message 出力する内容。
         * @param method 呼び出し元のメソッド名。省略可能で、既定は空文字です。
         * @param console ブラウザの開発者ツールのコンソールに表示するかどうか。省略可能で、既定はtrueです。
         * @param syslogs システムログに出力するかどうか。省略可能で、既定はtrueです。
         * @returns 出力できた場合はtrue、できなかった場合はfalse。
         *
         * @see https://pleasanter.org/ja/manual/server-script-logs-log-system-error
         */
        LogSystemError(message: unknown, method?: string, console?: boolean, syslogs?: boolean): boolean;
        /**
         * ログの種類をExceptionとしてシステムログを出力します。
         *
         * @param message 出力する内容。例外の内容などを指定します。
         * @param method 呼び出し元のメソッド名。省略可能で、既定は空文字です。
         * @param console ブラウザの開発者ツールのコンソールに表示するかどうか。省略可能で、既定はtrueです。
         * @param syslogs システムログに出力するかどうか。省略可能で、既定はtrueです。
         * @returns 出力できた場合はtrue、できなかった場合はfalse。
         *
         * @example
         * try {
         *     // 何らかの処理
         * } catch (e) {
         *     logs.LogException(e.stack);
         * }
         *
         * @see https://pleasanter.org/ja/manual/server-script-logs-log-exception
         */
        LogException(message: unknown, method?: string, console?: boolean, syslogs?: boolean): boolean;
    }
    interface PleasanterServerScriptSiteSettings {
        /** 既定のビューIDです。設定されていない場合はnullです。 */
        DefaultViewId: number | null;
        /** セクション設定の.NETリストです。設定されていない場合はnullです。 */
        Sections: PleasanterHostList<PleasanterServerScriptSection> | null;
        /**
         * サイトのタイトルからサイトIDを取得します。
         *
         * @param title 取得対象のサイトのタイトル。
         * @returns 該当するサイトのサイトID。指定したタイトルのサイトが見つからない場合は0を返します。
         *
         * @remarks JavaScriptのNumber.MAX_SAFE_INTEGERを超えるIDは、精度が失われる可能性があります。
         *
         * @example
         * const siteId = siteSettings.SiteId('My site title');
         *
         * @see https://pleasanter.org/ja/manual/server-script-site-settings-site-id
         */
        SiteId(title: string): number;
    }
    /** セクションの設定です。 */
    interface PleasanterServerScriptSection {
        /** 対象セクションのIDです。 */
        readonly Id: number;
        /** 対象セクションのラベル名です。 */
        LabelText: string;
        /** セクションの折りたたみを許可するかを指定します。設定されていない場合はnullです。 */
        AllowExpand: boolean | null;
        /** セクションの既定の表示（展開）を指定します。設定されていない場合はnullです。 */
        Expand: boolean | null;
    }
    /** ビューの項目ごとの指定です。項目名（カラム名）をキーに値を設定します。 */
    type PleasanterViewColumnValues<V = string> = {
        [K in keyof PleasanterStandardColumns | PleasanterExtendedColumnName]: V;
    } & {
        [columnName: string]: V;
    };
    interface PleasanterServerScriptView {
        /**
         * 一覧画面で現在選択されているビューのIDです。ビューが未選択の場合は0です。
         *
         * @remarks 読取専用です。条件が「ビュー処理時」の場合のみ使用できます。
         *
         * @example
         * context.Log('選択ビューのID : ' + view.Id);
         *
         * @see https://pleasanter.org/ja/manual/server-script-view-id
         */
        readonly Id: number;
        /**
         * 画面上に無い項目をmodelオブジェクトへ読み込むための指定です。Addメソッドで対象のカラム名を追加します。
         *
         * @remarks 条件が「ビュー処理時」の場合に有効です。
         *
         * @example
         * // 画面に無い状況項目をmodelで取得できるようにする
         * view.AlwaysGetColumns.Add('Status');
         *
         * @see https://pleasanter.org/ja/manual/server-script-view-always-get-columns
         */
        readonly AlwaysGetColumns: PleasanterServerScriptAlwaysGetColumns;
        /**
         * 拡張SQLのOnSelectingWhereを動的に指定します。適用する拡張SQLの名前を設定します。
         *
         * @example
         * if (context.UserId !== 3) {
         *     view.OnSelectingWhere = 'SelectingWhereName';
         * }
         *
         * @see https://pleasanter.org/ja/manual/server-script-view-on-selecting-where
         */
        OnSelectingWhere: string;
        /**
         * 一覧画面やエディタに表示するレコードをフィルタします。項目名をキーに条件を設定すると、ユーザに見せるレコードを制限できます。
         *
         * @remarks 条件が「ビュー処理時」の場合のみ使用できます。添付ファイル項目・コメント項目は使用できません。フィルタを設定した項目は、一覧画面のフィルタ操作より優先されます。
         *
         * @example
         * view.Filters.ClassA = '東京都中野区';
         *
         * @see https://pleasanter.org/ja/manual/server-script-view-filters
         */
        readonly Filters: PleasanterViewColumnValues;
        /**
         * 項目ごとの検索方法を指定します。項目名をキーに'PartialMatch'（部分一致）、'ExactMatch'（完全一致）、'ForwardMatch'（前方一致）や、配列用の'PartialMatchMultiple'等を設定します。
         *
         * @remarks 分類項目の形式（フリーテキスト／選択肢一覧／リンク設定）によって指定できる検索方法が異なります。
         *
         * @example
         * view.Filters.ClassA = '東京';
         * view.SearchTypes.ClassA = 'PartialMatch';
         *
         * @see https://pleasanter.org/ja/manual/server-script-view-search-types
         */
        readonly SearchTypes: PleasanterViewColumnValues<PleasanterViewSearchType>;
        /**
         * レコードのソートを指定します。項目名をキーに'asc'（昇順）、'desc'（降順）、'release'（解除）を設定します。
         *
         * @example
         * // NumAを降順で表示する
         * view.Sorters.NumA = 'desc';
         *
         * @see https://pleasanter.org/ja/manual/server-script-view-sorters
         */
        readonly Sorters: PleasanterViewColumnValues<PleasanterViewSorterOrder>;
        /**
         * view.ClearFiltersが呼び出し済みの場合はtrueです。
         *
         * @remarks 読取専用です。条件が「ビュー処理時」の場合のみ使用できます。
         *
         * @example
         * view.ClearFilters();
         * context.Log(view.FiltersCleared); // true
         *
         * @see https://pleasanter.org/ja/manual/server-script-view-filters-cleared
         */
        readonly FiltersCleared: boolean;
        /**
         * 指定した項目を否定条件でフィルタします。
         *
         * @param name 対象項目のカラム名。
         * @param negative 否定にする場合はtrue。省略時はtrueです。
         *
         * @remarks 「否定フィルタを使用する」の設定がオンになっている必要があります。
         *
         * @example
         * view.Filters.ClassA = '東京都中野区';
         * view.FilterNegative('ClassA');
         *
         * @see https://pleasanter.org/ja/manual/server-script-view-filter-negative
         */
        FilterNegative(name: string, negative?: boolean): void;
        /**
         * 設定済みのフィルタ条件をクリアします。WebUIで設定されたフィルタをサーバスクリプトで無効化する場合などに使用します。
         *
         * @remarks 呼び出し後にview.Filtersで新たに設定した条件は有効になります。条件が「ビュー処理時」の場合のみ使用できます。
         *
         * @example
         * view.ClearFilters();
         *
         * @see https://pleasanter.org/ja/manual/server-script-view-clear-filters
         */
        ClearFilters(): void;
        /**
         * 拡張SQLのOnSelectingWhereのプレースホルダに、動的なカラム名を指定します。
         *
         * @param placeholder 置き換え対象のプレースホルダ文字列。
         * @param columnName 置き換えるカラム名。
         *
         * @remarks カラム名以外の文字列は指定できません。
         *
         * @example
         * view.OnSelectingWhere = 'SelectingWhereName';
         * view.AddColumnPlaceholder('DynamicColumn', 'ClassA');
         *
         * @see https://pleasanter.org/ja/manual/server-script-view-add-column-placeholder
         */
        AddColumnPlaceholder(placeholder: string, columnName: string): void;
    }
    interface PleasanterServerScriptAlwaysGetColumns {
        /**
         * modelオブジェクトへ追加で読み込む項目のカラム名を指定します。
         *
         * @param columnName 読み込む項目のカラム名。
         *
         * @example
         * // 画面上に無い状況項目をmodelで取得できるようにする
         * view.AlwaysGetColumns.Add('Status');
         *
         * @see https://pleasanter.org/ja/manual/server-script-view-always-get-columns
         */
        Add(columnName: string): void;
    }
    /** 拡張項目の項目記号（A～Z）です。 */
    type PleasanterExtendedColumnSuffix =
        | 'A'
        | 'B'
        | 'C'
        | 'D'
        | 'E'
        | 'F'
        | 'G'
        | 'H'
        | 'I'
        | 'J'
        | 'K'
        | 'L'
        | 'M'
        | 'N'
        | 'O'
        | 'P'
        | 'Q'
        | 'R'
        | 'S'
        | 'T'
        | 'U'
        | 'V'
        | 'W'
        | 'X'
        | 'Y'
        | 'Z';
    /** 拡張項目（分類・数値・日付・説明・チェック・添付ファイルのA～Z）の項目名です。 */
    type PleasanterExtendedColumnName =
        `${'Class' | 'Num' | 'Date' | 'Description' | 'Check' | 'Attachments'}${PleasanterExtendedColumnSuffix}`;
    /** 分類A～Zです。 */
    type PleasanterClassColumns = { [K in `Class${PleasanterExtendedColumnSuffix}`]: string };
    /** 数値A～Zです。 */
    type PleasanterNumColumns = { [K in `Num${PleasanterExtendedColumnSuffix}`]: number };
    /** 日付A～Zです。 */
    type PleasanterDateColumns = { [K in `Date${PleasanterExtendedColumnSuffix}`]: Date };
    /** 説明A～Zです。 */
    type PleasanterDescriptionColumns = { [K in `Description${PleasanterExtendedColumnSuffix}`]: string };
    /** チェックA～Zです。 */
    type PleasanterCheckColumns = { [K in `Check${PleasanterExtendedColumnSuffix}`]: boolean };
    /** 添付ファイルA～Zです。読取専用です。 */
    type PleasanterAttachmentsColumns = { readonly [K in `Attachments${PleasanterExtendedColumnSuffix}`]: string };
    /** 拡張項目（分類・数値・日付・説明・チェック・添付ファイルのA～Z）一式です。 */
    type PleasanterExtendedColumns = PleasanterClassColumns &
        PleasanterNumColumns &
        PleasanterDateColumns &
        PleasanterDescriptionColumns &
        PleasanterCheckColumns &
        PleasanterAttachmentsColumns;
    /** modelおよびsavedに読み込まれた拡張項目です。 */
    type PleasanterOptionalExtendedColumns = Partial<PleasanterExtendedColumns>;
    /** レコードの標準項目です。 */
    interface PleasanterStandardColumns {
        /** 期限付きテーブルのレコードIDです。期限付きテーブルの場合のみ存在します。 */
        readonly IssueId?: number;
        /** 記録テーブルのレコードIDです。記録テーブルの場合のみ存在します。 */
        readonly ResultId?: number;
        /** サイトIDです。 */
        readonly SiteId: number;
        /** 作成者IDです。 */
        readonly Creator: number;
        /** 作成日時です。 */
        readonly CreatedTime: Date;
        /** 更新者IDです。 */
        readonly Updator: number;
        /** 更新日時です。 */
        readonly UpdatedTime: Date;
        /** バージョンです。 */
        readonly Ver: number;
        /** タイトルです。 */
        Title: string;
        /** 内容です。 */
        Body: string;
        /** 開始日時です。期限付きテーブルの場合のみ存在します。 */
        StartTime?: Date;
        /** 完了日時です。期限付きテーブルの場合のみ存在します。 */
        CompletionTime?: Date;
        /** 作業量です。期限付きテーブルの場合のみ存在します。 */
        WorkValue?: number;
        /** 進捗率です。期限付きテーブルの場合のみ存在します。 */
        ProgressRate?: number;
        /** 残作業量です。期限付きテーブルの場合のみ存在します。 */
        readonly RemainingWorkValue?: number;
        /** 状況です。 */
        Status: number;
        /** 管理者IDです。 */
        Manager: number;
        /** 担当者IDです。 */
        Owner: number;
        /** ロック状態です。 */
        Locked: boolean;
        /** コメントです。 */
        Comments: string;
    }
    /** レコードの表示や動作を制御する項目です。 */
    interface PleasanterRecordControls {
        /** 行CSSです。 */
        ExtendedRowCss: string;
        /** 行のdata属性です。 */
        ExtendedRowData: string;
        /** サーバスクリプト終了後にレコードを更新します。 */
        UpdateOnExit: boolean;
        /** レコードを読取専用にします。 */
        ReadOnly: boolean;
    }
    interface PleasanterServerScriptRecord
        extends PleasanterStandardColumns, PleasanterOptionalExtendedColumns, PleasanterRecordControls {}
    interface PleasanterServerScriptGrid {
        /** 一覧のレコード総数です。 */
        readonly TotalCount: number;
        /**
         * 一覧画面でチェックを入れて選択しているレコードのIDを取得します。
         *
         * @returns 選択されているレコードIDの.NETリスト。取得できない場合はnull。
         *
         * @remarks JavaScriptのNumber.MAX_SAFE_INTEGERを超えるIDは、精度が失われる可能性があります。
         *
         * @example
         * const selectedIds = grid.SelectedIds();
         * if (selectedIds !== null) {
         *     for (const id of selectedIds) {
         *         context.Log(id);
         *     }
         * }
         *
         * @see https://pleasanter.org/ja/manual/server-script-grid-selected-ids
         */
        SelectedIds(): PleasanterHostList<number> | null;
    }
    /** 標準項目および拡張項目それぞれの項目設定です。 */
    type PleasanterNamedColumnSettings = {
        readonly [K in keyof PleasanterStandardColumns | PleasanterExtendedColumnName]: PleasanterServerScriptColumn;
    };
    interface PleasanterServerScriptColumns extends PleasanterNamedColumnSettings {
        /** 任意の項目名を指定して項目設定を取得します。存在しない項目名の場合はundefinedです。 */
        [columnName: string]: PleasanterServerScriptColumn | undefined;
    }
    interface PleasanterServerScriptColumn {
        /** 一覧画面のセルCSSです。 */
        ExtendedCellCss: string;
        /** 編集画面のフィールドCSSです。 */
        ExtendedFieldCss: string;
        /** 編集画面のコントロールCSSです。 */
        ExtendedControlCss: string;
        /** フィールドの前に追加するHTMLです。 */
        ExtendedHtmlBeforeField: string;
        /** ラベルの前に追加するHTMLです。 */
        ExtendedHtmlBeforeLabel: string;
        /** ラベルとコントロールの間に追加するHTMLです。 */
        ExtendedHtmlBetweenLabelAndControl: string;
        /** コントロールの後に追加するHTMLです。 */
        ExtendedHtmlAfterControl: string;
        /** フィールドの後に追加するHTMLです。 */
        ExtendedHtmlAfterField: string;
        /** 項目を非表示にします。 */
        Hide: boolean;
        /** 項目を入力必須にします。 */
        ValidateRequired: boolean;
        /** 一覧画面の表示値を置き換えます。 */
        RawText: string;
        /** 項目を読取専用にします。 */
        ReadOnly: boolean;
        /**
         * 分類項目の選択肢一覧に選択肢を動的に追加します。複数回呼び出すことで選択肢を追加できます。
         *
         * @param key 追加する選択肢のキー。文字列または数値を指定します。
         * @param value 追加する選択肢の表示値。文字列または数値を指定します。
         *
         * @remarks 分類項目のみで使用できます。1回目の呼び出し時に、実行前に設定されていた選択肢はすべてクリアされます。条件が「画面表示の前」「行表示の前」の場合に有効です。
         *
         * @example
         * // 分類AにTEST1～TEST5の選択肢を設定する
         * for (let i = 1; i <= 5; i++) {
         *     columns.ClassA.AddChoiceHash(i, 'TEST' + i);
         * }
         *
         * @see https://pleasanter.org/ja/manual/server-script-columns-add-choice-hash
         */
        AddChoiceHash(key: string | number, value: string | number): void;
        /**
         * 分類項目の選択肢一覧に選択肢を動的に追加します。表示値にはkeyと同じ値が使われます。
         *
         * @param key 追加する選択肢のキー。表示値にも同じ値が使われます。
         *
         * @remarks 分類項目のみで使用できます。1回目の呼び出し時に、実行前に設定されていた選択肢はすべてクリアされます。条件が「画面表示の前」「行表示の前」の場合に有効です。
         *
         * @example
         * // 分類Aに1～5の選択肢を設定する
         * for (let i = 1; i <= 5; i++) {
         *     columns.ClassA.AddChoiceHash(i);
         * }
         *
         * @since 1.4.23.0
         *
         * @see https://pleasanter.org/ja/manual/server-script-columns-add-choice-hash
         */
        AddChoiceHash(key: string | number): void;
        /**
         * 分類項目の選択肢一覧をクリアします。
         *
         * @remarks 分類項目のみで使用できます。条件が「画面表示の前」「行表示の前」の場合に有効です。
         *
         * @example
         * columns.ClassA.ClearChoiceHash();
         *
         * @see https://pleasanter.org/ja/manual/server-script-columns-clear-choice-hash
         */
        ClearChoiceHash(): void;
    }
    interface PleasanterServerScriptElements {
        /**
         * ナビゲーションメニュー、コマンドボタン、プロセスで追加したボタンの表示状態を変更します。
         *
         * @param id 対象要素のHTMLのID属性。
         * @param displayType 表示状態。0:標準、1:無し、2:無効、3:非表示を指定します。
         *
         * @remarks 戻るボタンには適用できません。ナビゲーションメニューに2:無効は指定できません。条件が「画面表示の前」の場合に有効です。
         *
         * @example
         * // 削除ボタンを無効化する
         * elements.DisplayType('DeleteCommand', 2);
         *
         * @see https://pleasanter.org/ja/manual/server-script-elements-display-type
         */
        DisplayType(id: string, displayType: PleasanterDisplayType): void;
        /**
         * ナビゲーションメニュー、コマンドボタン、プロセスで追加したボタンの表示名を変更します。
         *
         * @param key 対象要素のHTMLのID属性。
         * @param labelText 変更後の表示名。
         *
         * @remarks 戻るボタンには適用できません。
         *
         * @example
         * // 更新ボタンの表示名を変更する
         * elements.LabelText('UpdateCommand', 'アップデート');
         *
         * @see https://pleasanter.org/ja/manual/server-script-elements-label-text
         */
        LabelText(key: string, labelText: string): void;
    }
    interface PleasanterServerScriptUtilities {
        /**
         * 指定した文字列をBase64形式の文字列に変換します。
         *
         * @param str 変換する文字列。
         * @param encoding 文字列のエンコーディング名。省略時は'utf-8'です。
         * @returns Base64形式の文字列。
         *
         * @example
         * const base64 = utilities.ConvertToBase64String('プリザンター', 'shift_jis');
         * context.Log(base64);
         *
         * @see https://pleasanter.org/ja/manual/server-script-utilities-convert-to-base64-string
         */
        ConvertToBase64String(str: string, encoding?: string): string;
        /**
         * 未設定を表す日時「0001/01/01 0:00:00」を返します。設定可能な日付の範囲外のため、サーバ側では未設定値として扱われます。
         *
         * @returns 未設定を表す日時。
         *
         * @example
         * // 日付Aを未設定にする
         * model.DateA = utilities.EmptyTime();
         *
         * @since 1.4.15.0
         *
         * @see https://pleasanter.org/ja/manual/server-script-utilities-emptytime
         */
        EmptyTime(): Date;
        /**
         * Pleasanterが扱える最小日時（General.jsonのMinTimeで指定した日時）をUTCで返します。
         *
         * @returns 最小日時。
         *
         * @example
         * model.DateA = utilities.MinTime();
         *
         * @since 1.4.15.0
         *
         * @see https://pleasanter.org/ja/manual/server-script-utilities-mintime
         */
        MinTime(): Date;
        /**
         * Pleasanterが扱える最大日時（General.jsonのMaxTimeで指定した日時）をUTCで返します。
         *
         * @returns 最大日時。
         *
         * @example
         * model.DateA = utilities.MaxTime();
         *
         * @since 1.4.15.0
         *
         * @see https://pleasanter.org/ja/manual/server-script-utilities-maxtime
         */
        MaxTime(): Date;
        /**
         * ローカル時間における当日0時の日時をUTCで返します。
         *
         * @returns 当日0時の日時（UTC）。
         *
         * @remarks サーバスクリプト内ではUTCとして扱われますが、スクリプト終了後にレコードへ格納される際はローカル時刻へ変換されます。
         *
         * @example
         * // 日付Aに当日の日時を入力する
         * model.DateA = utilities.Today();
         *
         * @see https://pleasanter.org/ja/manual/server-script-utilities-today
         */
        Today(): Date;
        /**
         * 指定した日時がPleasanterで扱える有効範囲内かどうかを判定します。
         *
         * @param datetime 判定する日時。
         * @returns 有効範囲内の場合はtrue、範囲外またはDate型でない場合はfalse。
         *
         * @example
         * if (utilities.InRange(model.DateA)) {
         *     context.Log('In range');
         * }
         *
         * @see https://pleasanter.org/ja/manual/server-script-utilities-inrange
         */
        InRange(datetime: Date): boolean;
    }

    type PleasanterServerScriptSavedRecord = Readonly<PleasanterStandardColumns & PleasanterOptionalExtendedColumns>;

    interface PleasanterServerScriptItems {
        /**
         * 指定したレコード、または指定したテーブルの条件に一致するレコードをapiModelの.NET配列として取得します。
         * サイト統合を設定しているテーブルの場合は、統合された各レコードもあわせて取得します。
         *
         * @param id 取得対象のサイトIDまたはレコードID。
         * @param data 取得するレコードのフィルタやソートをView形式のJSON文字列で指定します。省略時は全レコードが対象になります。
         * @returns 条件に一致するapiModelの.NET配列。該当が無い場合は空配列を返します。
         *
         * @remarks 一度に取得できる件数はApi.jsonのPageSize（既定200件）が上限です。200件を超える場合はページング処理が必要です。JavaScriptの配列メソッドを使用する場合はArray.fromで変換してください。
         *
         * @example
         * // サイトID 2 のテーブルで Status（状況）が 900（完了）のレコードを取得する
         * const view = { View: { ColumnFilterHash: { Status: '["900"]' } } };
         * const records = items.Get(2, JSON.stringify(view));
         * for (const record of records) {
         *     context.Log(record.Title);
         * }
         *
         * @see https://pleasanter.org/ja/manual/server-script-items-get
         */
        Get(id: number, data?: string): PleasanterHostArray<PleasanterServerScriptApiModel>;
        /**
         * 指定したサイトにレコードを作成します。
         *
         * @param siteId レコードを作成する対象サイトのサイトID。
         * @param model 作成内容。items.New系で取得したapiModelオブジェクト、またはItem形式のJSON文字列を指定します。
         * @returns 作成に成功した場合はtrue、失敗した場合はfalse。
         *
         * @example
         * // apiModelオブジェクトで作成する
         * const model = items.NewResult();
         * model.Title = '新規レコード';
         * model.ClassA = '分類A';
         * const created = items.Create(2, model);
         *
         * @example
         * // JSON文字列で作成する
         * const json = JSON.stringify({ Title: '新規レコード', ClassHash: { ClassA: '分類A' } });
         * const created = items.Create(2, json);
         *
         * @see https://pleasanter.org/ja/manual/server-script-items-create
         */
        Create(
            siteId: number,
            model: string | PleasanterServerScriptApiModel | PleasanterServerScriptSiteApiModel
        ): boolean;
        /**
         * 指定したレコードを更新します。
         *
         * @param recordId 更新対象のレコードID。
         * @param data 更新内容。items.Getなどで取得したapiModelオブジェクト、またはItem形式のJSON文字列を指定します。JSON文字列ではVerUpを指定できます。
         * @returns 更新に成功した場合はtrue、失敗した場合はfalse。
         *
         * @remarks VerUpの指定は1.5.6.0以降で有効です。それより前のバージョンでは指定しても無視され、自動バージョンアップの設定に従います。
         *
         * @example
         * const recordId = 12345;
         * const updateModel = JSON.stringify({ Body: '内容', VerUp: true });
         * const updated = items.Update(recordId, updateModel);
         * if (updated) {
         *     context.Log('更新成功');
         * } else {
         *     context.Log('更新失敗');
         * }
         *
         * @see https://pleasanter.org/ja/manual/server-script-items-update
         */
        Update(
            recordId: number,
            data: string | PleasanterServerScriptApiModel | PleasanterServerScriptSiteApiModel
        ): boolean;
        /**
         * 指定したサイトでキー項目に一致するレコードを更新し、存在しない場合は新規作成します。
         *
         * @param siteId 対象サイトのサイトID。
         * @param json Keys（キー項目名の配列）と更新内容を含むItem形式のJSON文字列。items.New系のapiModelオブジェクトも指定できます。VerUpも指定可能です。
         * @returns 作成または更新に成功した場合はtrue、失敗した場合はfalse。
         *
         * @remarks 作成・更新したレコードのIDは戻り値からは取得できません。Keysに指定した項目の値がNULLの場合はキー比較されず新規作成されます。VerUpの指定は1.5.6.0以降で有効です。それより前のバージョンでは指定しても無視され、自動バージョンアップの設定に従います。
         *
         * @example
         * const upsertModel = { Keys: ['Title'], Title: 'タイトル1', Body: '内容1', VerUp: true };
         * const result = items.Upsert(2, JSON.stringify(upsertModel));
         *
         * @see https://pleasanter.org/ja/manual/server-script-items-upsert
         */
        Upsert(siteId: number, json: string | PleasanterServerScriptApiModel): boolean;
        /**
         * 指定したレコードを削除します。
         *
         * @param recordId 削除対象のレコードID。
         * @returns 削除に成功した場合はtrue、失敗した場合はfalse。
         *
         * @example
         * const deleted = items.Delete(12345);
         *
         * @see https://pleasanter.org/ja/manual/server-script-items-delete
         */
        Delete(recordId: number): boolean;
        /**
         * 指定したサイトのレコードを一括削除します。条件を指定して削除対象を絞り込めます。
         *
         * @param siteId 対象サイトのサイトID。
         * @param data 削除対象を指定するJSON文字列。View（絞り込み条件）、Selected（レコードIDの配列）、All（true で全件）のいずれかで指定します。
         * @returns 削除したレコード数。削除できなかった場合は0。
         *
         * @remarks 戻り値がJavaScriptのNumber.MAX_SAFE_INTEGERを超える場合は、精度が失われる可能性があります。
         *
         * @example
         * // Status が 900（完了）のレコードを削除する
         * const data = { View: { ColumnFilterHash: { Status: '[900]' } } };
         * const count = items.BulkDelete(123, JSON.stringify(data));
         *
         * @see https://pleasanter.org/ja/manual/server-script-items-bulk-delete
         */
        BulkDelete(siteId: number, data: string): number;
        /**
         * 指定したサイトのレコード件数を集計します。条件を指定して集計対象を絞り込めます。
         *
         * @param siteId 対象サイトのサイトID。
         * @param view 集計対象を絞り込むView形式のJSON文字列。省略時はサイト内の全レコードが対象になります。
         * @returns レコードの件数。
         *
         * @example
         * const count = items.Count(2);
         *
         * @see https://pleasanter.org/ja/manual/server-script-items-count
         */
        Count(siteId: number, view?: string): number;
        /**
         * 指定したサイトの数値項目の合計値を取得します。
         *
         * @param siteId 集計対象のサイトID。
         * @param columnName 集計対象の数値項目名（例: 'NumA'）。
         * @param view 集計対象を絞り込むView形式のJSON文字列。省略時はサイト内の全レコードが対象になります。
         * @returns 指定した数値項目の合計値。
         *
         * @remarks 数値項目以外の項目を指定した場合は使用できません。
         *
         * @example
         * // サイトID 2 で Status が 900（完了）のレコードの NumA の合計値を取得する
         * const view = { View: { ColumnFilterHash: { Status: '["900"]' } } };
         * const total = items.Sum(2, 'NumA', JSON.stringify(view));
         * context.Log(total);
         *
         * @see https://pleasanter.org/ja/manual/server-script-items-sum
         */
        Sum(siteId: number, columnName: string, view?: string): number;
        /**
         * 指定したサイトの数値項目の平均値を取得します。
         *
         * @param siteId 集計対象のサイトID。
         * @param columnName 集計対象の数値項目名（例: 'NumA'）。
         * @param view 集計対象を絞り込むView形式のJSON文字列。省略時はサイト内の全レコードが対象になります。
         * @returns 指定した数値項目の平均値。
         *
         * @remarks 数値項目以外の項目を指定した場合は使用できません。
         *
         * @example
         * const avg = items.Average(2, 'NumA');
         *
         * @see https://pleasanter.org/ja/manual/server-script-items-average
         */
        Average(siteId: number, columnName: string, view?: string): number;
        /**
         * 指定したサイトの数値項目の最大値を取得します。
         *
         * @param siteId 集計対象のサイトID。
         * @param columnName 集計対象の数値項目名（例: 'NumA'）。
         * @param view 集計対象を絞り込むView形式のJSON文字列。省略時はサイト内の全レコードが対象になります。
         * @returns 指定した数値項目の最大値。
         *
         * @remarks 数値項目以外の項目を指定した場合は使用できません。
         *
         * @example
         * const max = items.Max(2, 'NumA');
         *
         * @see https://pleasanter.org/ja/manual/server-script-items-max
         */
        Max(siteId: number, columnName: string, view?: string): number;
        /**
         * 指定したサイトの数値項目の最小値を取得します。
         *
         * @param siteId 集計対象のサイトID。
         * @param columnName 集計対象の数値項目名（例: 'NumA'）。
         * @param view 集計対象を絞り込むView形式のJSON文字列。省略時はサイト内の全レコードが対象になります。
         * @returns 指定した数値項目の最小値。
         *
         * @remarks 数値項目以外の項目を指定した場合は使用できません。
         *
         * @example
         * const min = items.Min(2, 'NumA');
         *
         * @see https://pleasanter.org/ja/manual/server-script-items-min
         */
        Min(siteId: number, columnName: string, view?: string): number;
        /**
         * 指定したサイトの日付項目の最大値を取得します。
         *
         * @param siteId 集計対象のサイトID。
         * @param columnName 集計対象の日付項目名（例: 'DateA'）。
         * @param view 集計対象を絞り込むView形式のJSON文字列。省略可能。
         * @returns 指定した日付項目の最大値。該当する日付が無い場合はutilities.EmptyTimeの日時を返します。
         *
         * @remarks 日付項目以外の項目を指定した場合は使用できません。
         *
         * @example
         * const maxDate = items.MaxDate(2, 'DateA');
         * context.Log(maxDate);
         *
         * @since 1.4.16.0
         *
         * @see https://pleasanter.org/ja/manual/server-script-items-maxdate
         */
        MaxDate(siteId: number, columnName: string, view?: string): Date;
        /**
         * 指定したサイトの日付項目の最小値を取得します。
         *
         * @param siteId 集計対象のサイトID。
         * @param columnName 集計対象の日付項目名（例: 'DateA'）。
         * @param view 集計対象を絞り込むView形式のJSON文字列。省略可能。
         * @returns 指定した日付項目の最小値。該当する日付が無い場合はutilities.EmptyTimeの日時を返します。
         *
         * @remarks 日付項目以外の項目を指定した場合は使用できません。
         *
         * @example
         * const minDate = items.MinDate(2, 'DateA');
         *
         * @since 1.4.16.0
         *
         * @see https://pleasanter.org/ja/manual/server-script-items-mindate
         */
        MinDate(siteId: number, columnName: string, view?: string): Date;
        /**
         * 指定したサイトIDのサイト情報をapiModelの.NET配列として取得します。
         *
         * @param id 取得対象のサイトID。
         * @returns 該当するサイトのapiModelの.NET配列。
         *
         * @example
         * const sites = Array.from(items.GetSite(2));
         * if (sites.length > 0) {
         *     context.Log(sites[0].Title);
         * }
         *
         * @see https://pleasanter.org/ja/manual/server-script-items-get-site
         */
        GetSite(id: number): PleasanterHostArray<PleasanterServerScriptSiteApiModel>;
        /**
         * サイトのタイトルを指定して、該当するサイト情報をapiModelの.NET配列として取得します。
         *
         * @param title 取得対象のサイトのタイトル。
         * @returns 該当するサイトのapiModelの.NET配列。
         *
         * @see https://pleasanter.org/ja/manual/server-script-items-get-site-by-title
         */
        GetSiteByTitle(title: string): PleasanterHostArray<PleasanterServerScriptSiteApiModel>;
        /**
         * サイト名を指定して、該当するサイト情報をapiModelの.NET配列として取得します。
         *
         * @param name 取得対象のサイト名。サイトのタイトルではなく「サイト名」で検索します。
         * @returns 該当するサイトのapiModelの.NET配列。
         *
         * @see https://pleasanter.org/ja/manual/server-script-items-get-site-by-name
         */
        GetSiteByName(name: string): PleasanterHostArray<PleasanterServerScriptSiteApiModel>;
        /**
         * サイトグループ名を指定して、該当するサイト情報をapiModelの.NET配列として取得します。
         *
         * @param groupName 取得対象のサイトグループ名。
         * @returns 該当するサイトのapiModelの.NET配列。
         *
         * @see https://pleasanter.org/ja/manual/server-script-items-get-site-by-group-name
         */
        GetSiteByGroupName(groupName: string): PleasanterHostArray<PleasanterServerScriptSiteApiModel>;
        /**
         * 指定したサイト名に最も近いサイトのapiModelを取得します。
         *
         * @param name 検索対象のサイト名。サイトのタイトルではなく「サイト名」で検索します。
         * @param id 検索の起点となるサイトID。省略時はサーバスクリプトが格納されたサイトIDが使われます。
         * @returns 該当サイトのapiModel。見つからない場合やアクセス権が無い場合はnullを返します。
         *
         * @example
         * const site = items.GetClosestSite('顧客マスタ');
         * if (site !== null) {
         *     context.Log(site.SiteId);
         * }
         *
         * @see https://pleasanter.org/ja/manual/server-script-items-get-closest-site
         */
        GetClosestSite(name: string, id?: number): PleasanterServerScriptSiteApiModel | null;
        /**
         * 期限付きテーブル用の新しいapiModelを生成します。値を設定してitems.Createへ渡すことでレコードを作成します。
         *
         * @returns 期限付きテーブル用のapiModel。
         *
         * @example
         * const item = items.NewIssue();
         * item.Title = 'プリザンターのバージョンアップについて';
         * items.Create(2, item);
         * context.Log(item.IssueId); // 作成したレコードのID
         *
         * @see https://pleasanter.org/ja/manual/server-script-items-new-issue
         */
        NewIssue(): PleasanterServerScriptApiModel;
        /**
         * 記録テーブル用の新しいapiModelを生成します。値を設定してitems.Createへ渡すことでレコードを作成します。
         *
         * @returns 記録テーブル用のapiModel。
         *
         * @example
         * const item = items.NewResult();
         * item.Title = '新規レコード';
         * items.Create(2, item);
         *
         * @see https://pleasanter.org/ja/manual/server-script-items-new-result
         */
        NewResult(): PleasanterServerScriptApiModel;
        /**
         * サイト用の新しいapiModelを生成します。値を設定してitems.Createへ渡すことでサイトを作成します。
         *
         * @param referenceType サイト種別。'Sites'（サイト）、'Issues'（期限付きテーブル）、'Results'（記録テーブル）、'Wikis'（Wiki）、'DashBoard'（ダッシュボード）を指定します。
         * @returns サイト用のapiModel。
         *
         * @example
         * const item = items.NewSite('Issues');
         * item.Title = '課題管理';
         * items.Create(2, item);
         * context.Log(item.SiteId); // 作成したサイトのID
         *
         * @see https://pleasanter.org/ja/manual/server-script-items-new-site
         */
        NewSite(referenceType: string): PleasanterServerScriptSiteApiModel;
        /**
         * 新しいapiModelを返します。
         *
         * @deprecated テーブル種類に応じてNewIssue、NewResult、NewSiteを使用してください。
         * @returns 新しいapiModel。
         */
        New(): PleasanterServerScriptApiModel;
    }
    interface PleasanterServerScriptUsers {
        /**
         * 指定したユーザIDのユーザ情報をuserオブジェクトとして取得します。
         *
         * @param userId 取得対象のユーザID。
         * @returns 読取専用のuserオブジェクト。該当するユーザが存在しない場合はnull。
         *
         * @example
         * const user = users.Get(1);
         * if (user !== null) {
         *     context.Log(`${user.Name},${user.DeptId}`);
         * }
         *
         * @see https://pleasanter.org/ja/manual/server-script-users
         */
        Get(userId: number): PleasanterServerScriptUser | null;
    }
    interface PleasanterServerScriptDepts {
        /**
         * 指定した組織IDの組織情報をdeptオブジェクトとして取得します。
         *
         * @param deptId 取得対象の組織ID。
         * @returns 読取専用のdeptオブジェクト。該当する組織が存在しない場合はnull。
         *
         * @example
         * const dept = depts.Get(1);
         * if (dept !== null) {
         *     context.Log(dept.DeptName);
         * }
         *
         * @see https://pleasanter.org/ja/manual/server-script-depts
         */
        Get(deptId: number): PleasanterServerScriptDept | null;
    }
    interface PleasanterServerScriptGroups {
        /**
         * 指定したグループIDのグループ情報をgroupオブジェクトとして取得します。
         *
         * @param groupId 取得対象のグループID。
         * @returns 読取専用のgroupオブジェクト。該当するグループが存在しない場合はnull。
         *
         * @example
         * const group = groups.Get(1);
         * if (group !== null) {
         *     context.Log(group.GroupName);
         * }
         *
         * @see https://pleasanter.org/ja/manual/server-script-group
         */
        Get(groupId: number): PleasanterServerScriptGroup | null;
        /**
         * 指定したグループを更新します。メンバーは指定した内容ですべて洗い替えになります。
         *
         * @param groupId 更新対象のグループID。
         * @param data 更新内容のJSON文字列。
         * @returns 更新に成功した場合はtrue、失敗した場合はfalse。
         *
         * @remarks サイトの管理者権限が必要です。権限が無い場合は管理権限を持つユーザのAPIキーの指定が必要です。
         *
         * @see https://pleasanter.org/ja/manual/server-script-groups-update
         */
        Update(groupId: number, data: string): boolean;
    }
    interface PleasanterServerScriptNotifications {
        /**
         * 指定したIDの通知設定をnotificationオブジェクトとして取得します。
         *
         * @param id 取得対象の通知ID。
         * @returns notificationオブジェクト。
         *
         * @see https://pleasanter.org/ja/manual/server-script-notifications-get
         */
        Get(id: number): PleasanterServerScriptNotification;
        /**
         * 新しい空のnotificationオブジェクトを生成して返します。任意の宛先へ通知を送る場合に使用します。
         *
         * @returns 新しいnotificationオブジェクト。
         *
         * @example
         * const notification = notifications.New();
         * notification.Address = 'xxxxx@example.com';
         * notification.Title = '通知テスト';
         * notification.Body = 'サーバスクリプトから通知しています。';
         * notification.Send();
         *
         * @see https://pleasanter.org/ja/manual/server-script-notifications-new
         */
        New(): PleasanterServerScriptNotification;
    }
    interface PleasanterServerScriptHidden {
        /**
         * HTMLのhidden要素へ出力する情報を追加します。クライアント側のスクリプトから参照させたい値の受け渡しに使用します。
         *
         * @param key hidden要素のID。
         * @param value hidden要素の値。
         *
         * @example
         * // 所属グループIDの配列をJSON化してhidden要素へ出力する
         * hidden.Add('MyGroups', JSON.stringify(Array.from(context.Groups)));
         *
         * @see https://pleasanter.org/ja/manual/server-script-hidden-add
         */
        Add(key: string, value: unknown): void;
    }
    interface PleasanterServerScriptHttpClient {
        /** 接続先のURIです。 */
        RequestUri: string;
        /** 送信するデータです。 */
        Content: string;
        /** エンコーディングです。既定値はutf-8です。 */
        Encoding: string;
        /** メディアタイプです。既定値はapplication/jsonです。 */
        MediaType: string;
        /** リクエストヘッダです。 */
        readonly RequestHeaders: PleasanterServerScriptRequestHeaders;
        /** 直前のレスポンスヘッダです。 */
        readonly ResponseHeaders: Record<string, unknown>;
        /** 直前のステータスコードが200～299の場合はtrueです。 */
        readonly IsSuccess: boolean;
        /** 直前のHTTPステータスコードです。 */
        readonly StatusCode: number;
        /**
         * HTTPリクエストのタイムアウト時間（ミリ秒）です。
         *
         * @since 1.4.19.0
         */
        TimeOut: number;
        /**
         * 直前のリクエストがタイムアウトした場合はtrueです。
         *
         * @since 1.4.20.0
         */
        readonly IsTimeOut: boolean;
        /**
         * RequestUriへGETリクエストを送信し、レスポンス本文を返します。
         *
         * @returns レスポンス本文の文字列。
         *
         * @example
         * // Basic認証を付けてGETする
         * const base64 = utilities.ConvertToBase64String('userName:password');
         * httpClient.RequestHeaders.Add('Authorization', 'Basic ' + base64);
         * httpClient.RequestUri = 'https://servername/api/.....';
         * const result = httpClient.Get();
         *
         * @see https://pleasanter.org/ja/manual/server-script-http-client-get
         */
        Get(): string;
        /**
         * RequestUriへContentの内容をPOSTし、レスポンス本文を返します。
         *
         * @returns レスポンス本文の文字列。
         *
         * @example
         * const data = { data1: 'abc', data2: '123' };
         * httpClient.RequestUri = 'https://servername/api/.....';
         * httpClient.Content = JSON.stringify(data);
         * const response = httpClient.Post();
         * if (httpClient.IsSuccess) {
         *     context.Log('Success: ' + response);
         * } else {
         *     context.Log('Error: (' + httpClient.StatusCode + ')' + response);
         * }
         *
         * @see https://pleasanter.org/ja/manual/server-script-http-client-post
         */
        Post(): string;
        /**
         * RequestUriへContentの内容をPUTし、レスポンス本文を返します。
         *
         * @returns レスポンス本文の文字列。
         *
         * @see https://pleasanter.org/ja/manual/server-script-http-client-put
         */
        Put(): string;
        /**
         * RequestUriへContentの内容をPATCHし、レスポンス本文を返します。
         *
         * @returns レスポンス本文の文字列。
         *
         * @see https://pleasanter.org/ja/manual/server-script-http-client-patch
         */
        Patch(): string;
        /**
         * RequestUriへDELETEリクエストを送信し、レスポンス本文を返します。
         *
         * @returns レスポンス本文の文字列。
         *
         * @see https://pleasanter.org/ja/manual/server-script-http-client-delete
         */
        Delete(): string;
    }
    interface PleasanterServerScriptRequestHeaders {
        /**
         * リクエストヘッダを追加します。
         *
         * @param name ヘッダ名。
         * @param value ヘッダの値。
         *
         * @example
         * httpClient.RequestHeaders.Add('Authorization', 'Bearer ' + token);
         */
        Add(name: string, value: string): void;
        /**
         * 追加済みのリクエストヘッダをすべてクリアします。続けて別のヘッダを設定して送信する場合に使用します。
         *
         * @example
         * httpClient.RequestHeaders.Clear();
         * httpClient.RequestHeaders.Add('Authorization', 'Bearer ' + token);
         */
        Clear(): void;
    }
    interface PleasanterServerScriptPs {
        /** CSVと文字列を相互変換します。 */
        readonly CSV: PleasanterServerScriptCsv;
        /** 管理されたセクション内でファイルとディレクトリを操作します。 */
        readonly file: PleasanterServerScriptFile;
        /** JSONをシリアライズまたはデシリアライズします。 */
        readonly JSON: PleasanterServerScriptJson;
    }
    interface PleasanterServerScriptCsv {
        /**
         * 2次元配列をカンマ区切りの文字列（CSV）に変換します。
         *
         * @param csv 変換する2次元配列。
         * @returns カンマ区切りに変換した文字列。
         *
         * @example
         * const csv = [
         *     ['label', 'num1', 'num2'],
         *     ['a', '1', '3'],
         *     ['b', '2', '4']
         * ];
         * const text = $ps.CSV.csv2str(csv);
         *
         * @see https://pleasanter.org/ja/manual/server-script-ps-csv-csv2str
         */
        csv2str(csv: unknown): string;
        /**
         * カンマ区切りの文字列を2次元配列に変換します。
         *
         * @param text 変換するテキスト。
         * @returns 文字列×文字列の2次元配列。すべての要素は文字列です（数値・日付への変換は行いません）。
         *
         * @remarks 先頭行はヘッダとして扱いません。ヘッダの有無は利用者側で判断してください。
         *
         * @example
         * const rows = $ps.CSV.str2csv('a,b\n1,2');
         *
         * @see https://pleasanter.org/ja/manual/server-script-ps-csv-str2csv
         */
        str2csv(text: string): string[][];
    }
    interface PleasanterServerScriptFile {
        /**
         * テキストファイルの内容をすべて読み込みます。
         *
         * @param section セクション名。ファイル操作の基準となる管理単位です。
         * @param path ファイル名。ディレクトリの区切りはWindows/Linuxとも'/'を使います。
         * @param encode 文字エンコーディング名。省略時は'utf-8'です。
         * @returns ファイルの内容。ファイルが存在しない場合はnull。
         *
         * @remarks Script.jsonのDisableServerScriptFileをfalseに設定する必要があります。
         *
         * @example
         * const text = $ps.file.readAllText('01_develop', 'parts/01_parts.txt');
         *
         * @see https://pleasanter.org/ja/manual/server-script-ps-file-read-all-text
         */
        readAllText(section: string, path: string, encode?: string): string | null;
        /**
         * テキストファイルを書き込みます。同名ファイルがあれば上書き、なければ新規作成します。
         *
         * @param section セクション名。
         * @param path ファイル名。ディレクトリの区切りは'/'を使います。
         * @param data 出力する文字列。
         * @param encode 文字エンコーディング名。省略時は'utf-8'です。
         * @returns 出力できた場合はtrue、できなかった場合はfalse。
         *
         * @example
         * $ps.file.writeAllText('01_develop', 'parts/01_parts.txt', 'write data');
         *
         * @see https://pleasanter.org/ja/manual/server-script-ps-file-write-all-text
         */
        writeAllText(section: string, path: string, data: unknown, encode?: string): boolean;
        /**
         * ファイルをコピーします。
         *
         * @param section セクション名。
         * @param sourcePath コピー元のファイル名。
         * @param destPath コピー先のファイル名。
         * @returns コピーできた場合はtrue、できなかった場合はfalse。
         *
         * @since 1.4.19.0
         *
         * @see https://pleasanter.org/ja/manual/server-script-ps-file-copy-file
         */
        copyFile(section: string, sourcePath: string, destPath: string): boolean;
        /**
         * ファイルを移動、または名前を変更します。
         *
         * @param section セクション名。
         * @param oldName 変更前のファイル名。
         * @param newName 変更後のファイル名。
         * @returns 移動できた場合はtrue、できなかった場合はfalse。
         *
         * @see https://pleasanter.org/ja/manual/server-script-ps-file-move-file
         */
        moveFile(section: string, oldName: string, newName: string): boolean;
        /**
         * ファイルを削除します。
         *
         * @param section セクション名。
         * @param path 削除するファイル名。
         * @returns 削除できた場合はtrue、できなかった場合はfalse。
         *
         * @see https://pleasanter.org/ja/manual/server-script-ps-file-remove-file
         */
        removeFile(section: string, path: string): boolean;
        /**
         * 指定したディレクトリ内のファイル名一覧を取得します。
         *
         * @param section セクション名。
         * @param path ディレクトリ名。
         * @returns ファイル名文字列の配列。
         *
         * @example
         * for (const name of $ps.file.getFileList('01_develop', 'parts')) {
         *     context.Log(name);
         * }
         *
         * @see https://pleasanter.org/ja/manual/server-script-ps-file-get-file-list
         */
        getFileList(section: string, path: string): string[];
        /**
         * ディレクトリを作成します。
         *
         * @param section セクション名。
         * @param path 作成するディレクトリ名。
         * @returns 作成できた場合はtrue、できなかった場合はfalse。
         *
         * @see https://pleasanter.org/ja/manual/server-script-ps-file-create-directory
         */
        createDirectory(section: string, path: string): boolean;
        /**
         * ディレクトリを移動、または名前を変更します。
         *
         * @param section セクション名。
         * @param oldName 変更前のディレクトリ名。
         * @param newName 変更後のディレクトリ名。
         * @returns 移動できた場合はtrue、できなかった場合はfalse。
         *
         * @see https://pleasanter.org/ja/manual/server-script-ps-file-move-directory
         */
        moveDirectory(section: string, oldName: string, newName: string): boolean;
        /**
         * ディレクトリを削除します。
         *
         * @param section セクション名。
         * @param path 削除するディレクトリ名。
         * @returns 削除できた場合はtrue、できなかった場合はfalse。
         *
         * @see https://pleasanter.org/ja/manual/server-script-ps-file-remove-directory
         */
        removeDirectory(section: string, path: string): boolean;
        /**
         * 指定したディレクトリ内のディレクトリ名一覧を取得します。
         *
         * @param section セクション名。
         * @param path ディレクトリ名。
         * @returns ディレクトリ名文字列の配列。
         *
         * @see https://pleasanter.org/ja/manual/server-script-ps-file-get-directory-list
         */
        getDirectoryList(section: string, path: string): string[];
        /**
         * ファイル操作用のセクションを作成します。
         *
         * @param section 作成するセクション名。
         * @returns 作成できた場合はtrue、できなかった場合はfalse。
         *
         * @see https://pleasanter.org/ja/manual/server-script-ps-file-create-section
         */
        createSection(section: string): boolean;
        /**
         * ファイル操作用のセクションを削除します。
         *
         * @param section 削除するセクション名。
         * @returns 削除できた場合はtrue、できなかった場合はfalse。
         *
         * @see https://pleasanter.org/ja/manual/server-script-ps-file-remove-section
         */
        removeSection(section: string): boolean;
        /**
         * 指定したファイルをサイトへインポートします。
         *
         * @param section セクション名。
         * @param path インポートするファイル名。
         * @param siteId インポート先のサイトID。
         * @param json インポート用パラメータのJSON文字列。
         * @returns 成功した場合は新規登録数と更新数を持つオブジェクト（例: {insertCount:10, updateCount:0}）。失敗した場合はnull。
         *
         * @remarks Script.jsonのDisableServerScriptFileをfalseに設定し、テーブルのインポート権限が必要です。
         *
         * @example
         * const result = $ps.file.import('01_develop', 'parts/01_parts.csv', 100, $ps.JSON.stringify({}));
         * context.Log($ps.JSON.stringify(result));
         *
         * @since 1.4.13.0
         *
         * @see https://pleasanter.org/ja/manual/server-script-ps-file-import
         */
        import(section: string, path: string, siteId: number, json: string): unknown;
        /**
         * サイトのデータを指定したファイルへエクスポートします。
         *
         * @param section セクション名。
         * @param path エクスポート先のファイル名。
         * @param siteId エクスポート元のサイトID。
         * @param json エクスポート用パラメータのJSON文字列。
         * @returns 成功した場合はtrue、失敗した場合はfalse。
         *
         * @remarks Script.jsonのDisableServerScriptFileをfalseに設定し、テーブルのエクスポート権限が必要です。
         *
         * @example
         * const ok = $ps.file.export('01_develop', 'parts/01_parts.csv', 100, $ps.JSON.stringify({}));
         *
         * @since 1.4.13.0
         *
         * @see https://pleasanter.org/ja/manual/server-script-ps-file-export
         */
        export(section: string, path: string, siteId: number, json: string): boolean;
    }
    interface PleasanterServerScriptJson {
        /**
         * JSON文字列をオブジェクトへ変換（デシリアライズ）します。
         *
         * @param text JSON文字列。
         * @returns 変換したオブジェクト。
         *
         * @example
         * const obj = $ps.JSON.parse('{"a":1}');
         *
         * @see https://pleasanter.org/ja/manual/server-script-ps-json-parse
         */
        parse(text: string): unknown;
        /**
         * オブジェクトをJSON文字列へ変換（シリアライズ）します。modelなどサーバスクリプトのオブジェクトも変換できます。
         *
         * @param json 変換するオブジェクト。
         * @returns シリアライズしたJSON文字列。
         *
         * @example
         * const text = $ps.JSON.stringify(model);
         *
         * @see https://pleasanter.org/ja/manual/server-script-ps-json-stringify
         */
        stringify(json: unknown): string;
    }
    interface PleasanterServerScriptExtendedSql {
        /**
         * API実行を許可した拡張SQLを実行し、DataSetを返します。
         *
         * @param name 実行する拡張SQLの名前。
         * @param parameters 拡張SQLに渡すパラメータ。省略可能です。
         * @returns 実行結果のDataSet。
         *
         * @see https://pleasanter.org/ja/manual/extended-sql
         */
        ExecuteDataSet(name: string, parameters?: unknown): unknown;
        /**
         * API実行を許可した拡張SQLを実行し、DataTableを返します。
         *
         * @param name 実行する拡張SQLの名前。
         * @param parameters 拡張SQLに渡すパラメータ。省略可能です。
         * @returns 実行結果のDataTable。
         *
         * @see https://pleasanter.org/ja/manual/extended-sql
         */
        ExecuteTable(name: string, parameters?: unknown): unknown;
        /**
         * API実行を許可した拡張SQLを実行し、先頭行を返します。
         *
         * @param name 実行する拡張SQLの名前。
         * @param parameters 拡張SQLに渡すパラメータ。省略可能です。
         * @returns 実行結果の先頭行。
         *
         * @see https://pleasanter.org/ja/manual/extended-sql
         */
        ExecuteRow(name: string, parameters?: unknown): unknown;
        /**
         * API実行を許可した拡張SQLを実行し、先頭行の最初の列の値を返します。
         *
         * @param name 実行する拡張SQLの名前。
         * @param parameters 拡張SQLに渡すパラメータ。省略可能です。
         * @returns 先頭行の最初の列の値。
         *
         * @see https://pleasanter.org/ja/manual/extended-sql
         */
        ExecuteScalar(name: string, parameters?: unknown): unknown;
        /**
         * API実行を許可した拡張SQLを実行します。結果は返しません。
         *
         * @param name 実行する拡張SQLの名前。
         * @param parameters 拡張SQLに渡すパラメータ。省略可能です。
         *
         * @see https://pleasanter.org/ja/manual/extended-sql
         */
        ExecuteNonQuery(name: string, parameters?: unknown): void;
    }
    interface PleasanterServerScriptResponses {
        /**
         * 指定した画面項目の再読み込みをクライアントへ指示します。
         *
         * @param type 再読み込みを行う項目の領域。
         * @param id 再読み込みを行う項目のID。
         *
         * @example
         * // 一覧画面のフィルタにある分類Jの選択肢を作り直して再読み込みする
         * responses.Reload('Filter', 'ClassJ');
         *
         * @see https://pleasanter.org/ja/manual/server-script-responses-reload
         */
        Reload(type: 'Filter' | (string & {}), id: string): void;
    }
    /** apiModelに共通する項目と操作です。 */
    interface PleasanterServerScriptApiModelBase {
        /** サイトIDです。 */
        readonly SiteId: number;
        /** 作成者IDです。 */
        readonly Creator: number;
        /** 作成日時です。 */
        readonly CreatedTime: Date;
        /** 更新者IDです。 */
        readonly Updator: number;
        /** 更新日時です。 */
        readonly UpdatedTime: Date;
        /** バージョンです。 */
        readonly Ver: number;
        /**
         * バージョンを自動更新する場合はtrueです。
         *
         * @remarks 更新時にこの指定が反映されるのは1.5.6.0以降です。それより前のバージョンでは無視され、自動バージョンアップの設定に従います。
         *
         * @since 1.5.6.0
         */
        VerUp: boolean;
        /** タイトルです。 */
        Title: string;
        /** 内容です。 */
        Body: string;
        /**
         * このapiModelの内容で指定したサイトにレコードを作成します。
         *
         * @param siteId 作成先のサイトID。
         * @returns 作成に成功した場合はtrue、失敗した場合はfalse。
         *
         * @remarks 作成後のレコードIDなどはこのapiModelへ反映されます（戻り値からは取得できません）。
         *
         * @example
         * const item = items.NewIssue();
         * item.Title = 'プリザンターのバージョンアップ手順について';
         * item.Create(123);
         * context.Log(item.IssueId); // 作成したレコードのID
         *
         * @see https://pleasanter.org/ja/manual/server-script-api-model-create
         */
        Create(siteId: number): boolean;
        /**
         * このapiModelに対応する既存レコードを、現在の内容で更新します。
         *
         * @returns 更新に成功した場合はtrue、失敗した場合はfalse。
         *
         * @example
         * const records = items.Get(123);
         * if (records.Length === 1) {
         *     const record = records[0];
         *     record.Status = 900;
         *     record.Update();
         * }
         *
         * @see https://pleasanter.org/ja/manual/server-script-api-model-update
         */
        Update(): boolean;
        /**
         * このapiModelに対応する既存レコードを削除します。
         *
         * @returns 削除に成功した場合はtrue、失敗した場合はfalse。
         *
         * @example
         * const records = items.Get(123);
         * if (records.Length === 1) {
         *     records[0].Delete();
         * }
         *
         * @see https://pleasanter.org/ja/manual/server-script-api-model-delete
         */
        Delete(): boolean;
    }
    /** 期限付きテーブルまたは記録テーブルのapiModelです。 */
    interface PleasanterServerScriptApiModel extends PleasanterServerScriptApiModelBase, PleasanterExtendedColumns {
        /** 期限付きテーブルのレコードIDです。 */
        readonly IssueId: number;
        /** 記録テーブルのレコードIDです。 */
        readonly ResultId: number;
        /** 開始日時です。 */
        StartTime: Date;
        /** 完了日時です。 */
        CompletionTime: Date;
        /** 作業量です。 */
        WorkValue: number;
        /** 進捗率です。 */
        ProgressRate: number;
        /** 残作業量です。 */
        readonly RemainingWorkValue: number;
        /** 状況です。 */
        Status: number;
        /** 管理者IDです。 */
        Manager: number;
        /** 担当者IDです。 */
        Owner: number;
        /** ロック状態です。 */
        Locked: boolean;
        /** レコードを読取専用にします。 */
        ReadOnly: boolean;
        /**
         * 実行するプロセスIDです。
         *
         * @since 1.5.1.0
         */
        ProcessId: number | null;
        /**
         * 実行するプロセスIDの.NET配列です。
         *
         * @since 1.5.2.0
         */
        ProcessIds: PleasanterHostArray<number | null> | null;
    }
    /** サイトのapiModelです。 */
    interface PleasanterServerScriptSiteApiModel extends PleasanterServerScriptApiModelBase {
        /** サイト名です。 */
        SiteName: string;
        /** サイトグループ名です。 */
        SiteGroupName: string;
        /** 一覧画面のガイドです。 */
        GridGuide: string;
        /** 編集画面のガイドです。 */
        EditorGuide: string;
        /** カレンダーのガイドです。 */
        CalendarGuide: string;
        /** クロス集計のガイドです。 */
        CrosstabGuide: string;
        /** ガントチャートのガイドです。 */
        GanttGuide: string;
        /** バーンダウンチャートのガイドです。 */
        BurnDownGuide: string;
        /** 時系列チャートのガイドです。 */
        TimeSeriesGuide: string;
        /** 分析チャートのガイドです。 */
        AnalyGuide: string;
        /** カンバンのガイドです。 */
        KambanGuide: string;
        /** 画像ライブラリのガイドです。 */
        ImageLibGuide: string;
        /** サイトの種類です。 */
        ReferenceType: string;
        /** 親サイトIDです。 */
        ParentId: number;
        /** 権限を継承するサイトIDです。 */
        InheritPermission: number;
        /** サイト設定です。 */
        SiteSettings: unknown;
        /** サイトを公開する場合はtrueです。 */
        Publish: boolean;
        /** 横断検索を無効にする場合はtrueです。 */
        DisableCrossSearch: boolean;
        /** ロック日時です。 */
        LockedTime: Date;
        /** ロックしたユーザです。 */
        LockedUser: unknown;
        /** API使用回数の集計日です。 */
        ApiCountDate: Date;
        /** API使用回数です。 */
        ApiCount: number;
    }
    interface PleasanterServerScriptUser {
        /** テナントIDです。 */
        readonly TenantId: number;
        /** ユーザIDです。 */
        readonly UserId: number;
        /** 組織IDです。 */
        readonly DeptId: number;
        /** ログインIDです。 */
        readonly LoginId: string;
        /** ユーザ名です。 */
        readonly Name: string;
        /** ユーザコードです。 */
        readonly UserCode: string;
        /** テナント管理者の場合はtrueです。 */
        readonly TenantManager: boolean;
        /** 無効なユーザの場合はtrueです。 */
        readonly Disabled: boolean;
    }
    interface PleasanterServerScriptDept {
        /** 組織IDです。 */
        readonly DeptId: number;
        /** 組織コードです。 */
        readonly DeptCode: string;
        /** 組織名です。 */
        readonly DeptName: string;
        /**
         * 組織に所属するuserオブジェクトの一覧を取得します。
         *
         * @returns 組織に所属するuserオブジェクトの.NETリスト。
         *
         * @example
         * const dept = depts.Get(1);
         * for (const user of dept.GetMembers()) {
         *     context.Log(user.Name);
         * }
         *
         * @see https://pleasanter.org/ja/manual/server-script-dept-get-members
         */
        GetMembers(): PleasanterHostList<PleasanterServerScriptUser>;
    }
    interface PleasanterServerScriptGroup {
        /** グループIDです。 */
        readonly GroupId: number;
        /** グループ名です。 */
        readonly GroupName: string;
        /** グループの内容です。 */
        readonly Body: string;
        /** 無効なグループの場合はtrueです。 */
        readonly Disabled: boolean;
        /**
         * グループに所属するgroupMemberオブジェクトの一覧を取得します。
         *
         * @returns groupMemberオブジェクトの.NETリスト。
         *
         * @example
         * const group = groups.Get(1);
         * for (const member of group.GetMembers()) {
         *     context.Log(`${member.DeptId},${member.UserId}`);
         * }
         *
         * @see https://pleasanter.org/ja/manual/server-script-group-get-members
         */
        GetMembers(): PleasanterHostList<PleasanterServerScriptGroupMember>;
        /**
         * グループに含まれる子グループのgroupオブジェクトの一覧を取得します。
         *
         * @returns 子グループのgroupオブジェクトの.NETリスト。
         *
         * @see https://pleasanter.org/ja/manual/server-script-group-children
         */
        GetChildren(): PleasanterHostList<PleasanterServerScriptGroup>;
        /**
         * 指定した子グループがこのグループに含まれるかどうかを判定します。
         *
         * @param groupId 判定する子グループのグループID。
         * @returns 含まれる場合はtrue。
         *
         * @see https://pleasanter.org/ja/manual/server-script-group-contains-child
         */
        ContainsChild(groupId: number): boolean;
        /**
         * 指定した組織がこのグループに含まれるかどうかを判定します。
         *
         * @param deptId 判定する組織の組織ID。
         * @returns 含まれる場合はtrue。
         *
         * @see https://pleasanter.org/ja/manual/server-script-group-contains-dept
         */
        ContainsDept(deptId: number): boolean;
        /**
         * 指定したユーザ、またはそのユーザの所属組織がこのグループに含まれるかどうかを判定します。
         *
         * @param userId 判定するユーザのユーザID。
         * @returns 含まれる場合はtrue。
         *
         * @see https://pleasanter.org/ja/manual/server-script-group-contains-user
         */
        ContainsUser(userId: number): boolean;
    }
    interface PleasanterServerScriptGroupMember {
        /** 子グループのグループIDです。 */
        readonly GroupId: number;
        /**
         * 子グループのグループ名です。
         *
         * @since 1.5.2.0
         */
        readonly GroupName: string;
        /** 組織メンバーの組織IDです。 */
        readonly DeptId: number;
        /**
         * 組織メンバーの組織名です。
         *
         * @since 1.5.2.0
         */
        readonly DeptName: string;
        /**
         * 組織メンバーの組織コードです。
         *
         * @since 1.5.2.0
         */
        readonly DeptCode: string;
        /** ユーザメンバーのユーザIDです。 */
        readonly UserId: number;
        /**
         * ユーザメンバーのログインIDです。
         *
         * @since 1.5.2.0
         */
        readonly LoginId: string;
        /**
         * ユーザメンバーのユーザ名です。
         *
         * @since 1.5.2.0
         */
        readonly Name: string;
        /**
         * ユーザメンバーのユーザコードです。
         *
         * @since 1.5.2.0
         */
        readonly UserCode: string;
        /**
         * ユーザメンバーがテナント管理者の場合はtrueです。
         *
         * @since 1.5.2.0
         */
        readonly TenantManager: boolean;
        /**
         * メンバーが無効な場合はtrueです。
         *
         * @since 1.5.2.0
         */
        readonly Disabled: boolean;
        /** グループの管理権限を持つ場合はtrueです。 */
        readonly Admin: boolean;
    }
    interface PleasanterServerScriptNotification {
        /** 通知IDです。 */
        Id: number;
        /** 通知種別です。1:Mail、2:Slack、3:ChatWork、4:Line、5:LineGroup、6:Teams、7:RocketChat、8:InCircleです。 */
        Type: PleasanterNotificationType;
        /** 通知のプレフィックスです。 */
        Prefix: string;
        /** 通知先アドレスです。 */
        Address: string;
        /** Mail通知のCcアドレスです。 */
        CcAddress: string;
        /** Mail通知のBccアドレスです。 */
        BccAddress: string;
        /** 通知に使用するトークンです。 */
        Token: string;
        /** カスタムデザインを使用する場合はtrueです。 */
        UseCustomFormat: boolean;
        /** 通知の書式です。 */
        Format: string;
        /** 通知を無効にする場合はtrueです。 */
        Disabled: boolean;
        /** 通知のタイトルです。 */
        Title: string;
        /** 通知の内容です。 */
        Body: string;
        /**
         * 設定した内容で通知を送信します。
         *
         * @returns 処理の成否にかかわらずtrueを返します。メールサーバへの送信に失敗した場合はSyslogsへ記録されます。
         *
         * @remarks Notification.jsonで対象の通知種別（Mail等）をtrueに設定する必要があります。
         *
         * @example
         * const notification = notifications.New();
         * notification.Address = 'xxxxx@example.com';
         * notification.Title = '通知テスト';
         * notification.Body = 'サーバスクリプトから通知しています。';
         * notification.Send();
         *
         * @see https://pleasanter.org/ja/manual/server-script-notification-send
         */
        Send(): boolean;
    }
}
