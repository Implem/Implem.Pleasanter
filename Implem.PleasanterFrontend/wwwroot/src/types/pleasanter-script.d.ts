export {};

declare global {
    /** Pleasanterの画面情報、項目操作、Ajax API、画面イベントを提供します。 */
    const $p: PleasanterScriptApi;

    interface PleasanterScriptApi {
        /** 画面イベントを設定します。 */
        readonly events: PleasanterScriptEvents;
        /** ユーザが変更した値を保持するクライアント側の変数です。$p.setで格納された値が送信時にPOSTされます。 */
        readonly data: Record<string, unknown>;
        /**
         * 現在のフォルダまたはテーブルの種類を取得します。
         *
         * @returns テーブルの種類（'Sites'、'Issues'、'Results'、'Wikis'など）。
         *
         * @example
         * const tableName = $p.tableName();
         *
         * @see https://pleasanter.org/ja/manual/script-table-name
         */
        tableName(): string;
        /**
         * 現在のコントローラ名を取得します。
         *
         * @returns コントローラ名（'items'、'users'など）。
         *
         * @see https://pleasanter.org/ja/manual/script-controller
         */
        controller(): string;
        /**
         * 現在の画面のアクション名を取得します。
         *
         * @returns アクション名（'index'、'new'、'edit'、'calendar'、'kamban'など）。
         *
         * @see https://pleasanter.org/ja/manual/script-action
         */
        action(): string;
        /**
         * 現在のサイトIDまたはレコードIDを取得します。編集画面ではレコードID、一覧画面ではサイトIDを返します。
         *
         * @returns 現在のIDの数値。
         *
         * @remarks JavaScriptのNumber.MAX_SAFE_INTEGERを超えるIDは、精度が失われる可能性があります。
         *
         * @example
         * const id = $p.id();
         *
         * @see https://pleasanter.org/ja/manual/script-id
         */
        id(): number;
        /**
         * サイトIDを取得します。引数を省略すると現在のサイトIDを、サイト名を指定するとリンク先テーブルのサイトIDを返します。
         *
         * @param siteName リンク先テーブルのサイト名。省略すると現在のサイトIDを返します。
         * @returns サイトIDの数値。サイト名を指定して見つからない場合はundefinedを返します。
         *
         * @remarks JavaScriptのNumber.MAX_SAFE_INTEGERを超えるIDは、精度が失われる可能性があります。
         *
         * @example
         * const currentSiteId = $p.siteId();
         * const linkedSiteId = $p.siteId('顧客マスタ');
         *
         * @see https://pleasanter.org/ja/manual/script-site-id
         */
        siteId(): number;
        siteId(siteName: string): number | undefined;
        /**
         * ログインユーザの組織IDを取得します。
         *
         * @returns 組織IDの数値。
         *
         * @see https://pleasanter.org/ja/manual/script-dept-id
         */
        deptId(): number;
        /**
         * ログインユーザが所属するグループIDの一覧を取得します。
         *
         * @returns グループIDの配列。
         *
         * @since 1.4.18.0
         *
         * @see https://pleasanter.org/ja/manual/script-groupIds
         */
        groupIds(): number[];
        /**
         * ログインIDを取得します。
         *
         * @returns ログインIDの文字列。
         *
         * @see https://pleasanter.org/ja/manual/script-login-id
         */
        loginId(): string;
        /**
         * ログインユーザのユーザIDを取得します。
         *
         * @returns ユーザIDの数値。
         *
         * @see https://pleasanter.org/ja/manual/script-user-id
         */
        userId(): number;
        /**
         * ログインユーザのユーザ名を取得します。
         *
         * @returns ユーザ名の文字列。
         *
         * @see https://pleasanter.org/ja/manual/script-user-name
         */
        userName(): string;
        /**
         * レスポンシブ表示が有効かどうかを取得します。
         *
         * @returns レスポンシブ表示が有効な場合はtrue。
         *
         * @see https://pleasanter.org/ja/manual/script-responsive
         */
        responsive(): boolean;
        /**
         * 一覧画面でチェックを入れて選択しているレコードのIDの配列を取得します。
         *
         * @returns 選択されているレコードIDの配列。
         *
         * @remarks JavaScriptのNumber.MAX_SAFE_INTEGERを超えるIDは、精度が失われる可能性があります。
         *
         * @example
         * const ids = $p.selectedIds();
         *
         * @see https://pleasanter.org/ja/manual/script-selected-ids
         */
        selectedIds(): number[];
        /**
         * 表示名からデータベースの項目名（カラム名）を取得します。
         *
         * @param displayName 項目の表示名。
         * @returns データベースの項目名。該当が無い場合はundefined。
         *
         * @example
         * const columnName = $p.getColumnName('顧客');
         *
         * @see https://pleasanter.org/ja/manual/script-get-column-name
         */
        getColumnName(displayName: string): string | undefined;
        /**
         * 編集画面で対象の項目の入力コントロールをjQueryオブジェクトとして取得します。
         *
         * @param name 項目名または表示名。
         * @returns 入力コントロールのjQueryオブジェクト。該当が無い場合はundefined。
         *
         * @example
         * const control = $p.getControl('顧客');
         *
         * @see https://pleasanter.org/ja/manual/script-get-control
         */
        getControl(name: string): JQuery | undefined;
        /**
         * 編集画面で対象の項目のラベルとコントロールを含むフィールド要素をjQueryオブジェクトとして取得します。
         *
         * @param name 項目名または表示名。
         * @returns フィールド要素のjQueryオブジェクト。該当が無い場合はundefined。
         *
         * @see https://pleasanter.org/ja/manual/script-get-field
         */
        getField(name: string): JQuery | undefined;
        /**
         * 編集画面で対象の項目の現在の値を取得します。
         *
         * @param name 項目名または表示名。
         * @returns 項目の値。チェック項目は真偽値、それ以外は文字列で取得します。該当が無い場合はundefinedです。添付ファイル項目は配列形式のJSON文字列です。
         *
         * @remarks コメント項目の値は取得できません。数値・日付項目は文字列として取得されるため、必要に応じて型変換してください。
         *
         * @example
         * const value = $p.getValue('顧客');
         *
         * @see https://pleasanter.org/ja/manual/script-get-value
         */
        getValue(name: string): string | boolean | undefined;
        /**
         * 画面上の値の変更と$p.dataへの格納を同時に行います。保存対象の値を変更するときに使用します。
         *
         * @param $control 変更対象のコントロール（$p.getControlで取得したjQueryオブジェクト）。
         * @param value 変更後の値。
         *
         * @remarks jQueryで直接val()を変更しても$p.dataに反映されず保存されないため、値の変更にはこのメソッドを使用します。
         *
         * @example
         * $p.set($p.getControl('ClassA'), '変更後の値');
         *
         * @see https://pleasanter.org/ja/manual/script-set
         */
        set($control: JQuery, value: unknown): void;
        /**
         * 一覧画面で指定したレコードの行（tr要素）をjQueryオブジェクトとして取得します。
         *
         * @param recordId 対象レコードのID。
         * @returns 行要素のjQueryオブジェクト。
         *
         * @see https://pleasanter.org/ja/manual/script-get-grid-row
         */
        getGridRow(recordId: number): JQuery;
        /**
         * 一覧画面で指定したレコードと表示名のセル（td要素）をjQueryオブジェクトとして取得します。
         *
         * @param recordId 対象レコードのID。
         * @param displayName 項目の表示名。
         * @returns セル要素のjQueryオブジェクト。
         *
         * @see https://pleasanter.org/ja/manual/script-get-grid-cell
         */
        getGridCell(recordId: number, displayName: string): JQuery;
        /**
         * 一覧画面で指定した表示名の項目が何列目にあるかを取得します。
         *
         * @param displayName 項目の表示名。
         * @returns 列の位置（0始まり）。該当が無い場合は-1。
         *
         * @see https://pleasanter.org/ja/manual/script-get-column-index
         */
        getGridColumnIndex(displayName: string): number;
        /**
         * 指定した項目のイベントを監視し、発生時に処理を実行します。
         *
         * @param eventName Ajaxのイベント名（'change'など）。
         * @param columnName 監視対象の項目のカラム名。
         * @param handler イベント発生時に実行する処理。
         *
         * @example
         * $p.on('change', 'ClassA', function () {
         *     console.log('分類Aの値が変更されました。');
         * });
         *
         * @see https://pleasanter.org/ja/manual/script-on
         */
        on(eventName: string, columnName: string, handler: (...args: unknown[]) => void): void;
        /**
         * 指定した要素の内容をAjaxでサーバへ送信し、レスポンスをもとに対象の画面項目を再描画します。
         *
         * @param $element 送信対象のHTML要素（jQueryオブジェクト）。
         *
         * @example
         * // 一覧画面を2秒ごとに再描画する
         * setInterval(function () {
         *     $p.send($('#Grid'));
         * }, 2000);
         *
         * @see https://pleasanter.org/ja/manual/script-send
         */
        send($element: JQuery): void;
        /**
         * 指定したボタン要素に対応するプロセスを実行します。
         *
         * @param $buttonElement プロセスを実行するボタンのHTML要素（jQueryオブジェクト）。実行種別が「追加したボタン」の場合は該当のプロセスボタン、「作成または更新」の場合は'#CreateCommand'や'#UpdateCommand'を指定します。
         *
         * @remarks 編集画面でのみ実行できます。一覧画面の一括処理では実行しません。対象ボタンが画面上に存在しない場合や、elements.DisplayTypeで「1:無し」にした場合はプロセスは実行されません。
         *
         * @example
         * // プロセスID:3のプロセスを実行する
         * $p.execProcess($('#Process_3'));
         *
         * @see https://pleasanter.org/ja/manual/script-execprocess
         */
        execProcess($buttonElement: JQuery): void;
        /**
         * 画面下部に表示されているメッセージを消去します。
         *
         * @example
         * $p.clearMessage();
         *
         * @see https://pleasanter.org/ja/manual/script-clear-message
         */
        clearMessage(): void;
        /**
         * 画面下部に成功・警告・エラーのメッセージを表示します。
         *
         * @param selector メッセージを表示する要素のセレクタ（通常は'#Message'）。
         * @param json CssとTextを持つオブジェクトをJSON.stringifyした文字列。Cssには'alert-success'（緑）、'alert-warning'（黄）、'alert-error'（赤）を指定します。
         *
         * @example
         * $p.setMessage('#Message', JSON.stringify({ Css: 'alert-success', Text: '処理が完了しました。' }));
         *
         * @see https://pleasanter.org/ja/manual/script-set-message
         */
        setMessage(selector: string, json: string): void;
        /**
         * 指定したサイトのレコード情報を取得します。サイト統合を設定している場合は統合された各レコードもあわせて取得します。
         *
         * @param options id（対象サイトID）とdata（取得条件のView）、done/fail/alwaysコールバックを指定します。
         *
         * @remarks 取得できる件数はApi.jsonのPageSize（既定200件）が上限です。
         *
         * @example
         * $p.apiGet({
         *     id: 123,
         *     data: { View: { ColumnFilterHash: { ClassA: '中野区' } } },
         *     done: function (data) {
         *         console.log(data);
         *     }
         * });
         *
         * @see https://pleasanter.org/ja/manual/script-api-get
         */
        apiGet(options: PleasanterApiOptions): void;
        /**
         * 指定したサイトにレコードを作成します。
         *
         * @param options id（作成先のサイトID）とdata（作成内容）、done/fail/alwaysコールバックを指定します。
         *
         * @example
         * $p.apiCreate({
         *     id: 123,
         *     data: { Title: '新規レコード', ClassHash: { ClassA: '分類A' } },
         *     done: function (data) {
         *         console.log('作成しました。');
         *     }
         * });
         *
         * @see https://pleasanter.org/ja/manual/script-api-create
         */
        apiCreate(options: PleasanterApiOptions): void;
        /**
         * 指定したレコードまたはWikiを更新します。
         *
         * @param options id（対象レコードID）とdata（更新内容）、done/fail/alwaysコールバックを指定します。
         *
         * @example
         * $p.apiUpdate({
         *     id: 12345,
         *     data: { Body: '更新後の内容' },
         *     done: function (data) {
         *         console.log('更新しました。');
         *     }
         * });
         *
         * @see https://pleasanter.org/ja/manual/script-api-update
         */
        apiUpdate(options: PleasanterApiOptions): void;
        /**
         * キー項目に一致するレコードを更新し、存在しない場合は作成します。
         *
         * @param options id（対象サイトID）とdata（キー項目と更新内容）、done/fail/alwaysコールバックを指定します。
         *
         * @see https://pleasanter.org/ja/manual/script-api-upsert
         */
        apiUpsert(options: PleasanterApiOptions): void;
        /**
         * 指定したレコードまたはWikiを削除します。
         *
         * @param options id（対象レコードID）とdone/fail/alwaysコールバックを指定します。
         *
         * @see https://pleasanter.org/ja/manual/script-api-delete
         */
        apiDelete(options: PleasanterApiOptions): void;
        /**
         * 指定した条件に一致するレコードを一括削除します。
         *
         * @param options id（対象サイトID）とdata（削除条件のView）、done/fail/alwaysコールバックを指定します。
         *
         * @see https://pleasanter.org/ja/manual/script-api-bulk-delete
         */
        apiBulkDelete(options: PleasanterApiOptions): void;
        /**
         * 指定したサイトと操作に対応するAPIのURLを取得します。
         *
         * @param siteId 対象のサイトIDまたはレコードID。
         * @param action 操作の種類。'get'、'create'、'update'、'delete'などを指定します。
         * @returns 組み立てられたAPIのURL文字列。
         *
         * @example
         * const url = $p.apiUrl(123, 'get');
         *
         * @see https://pleasanter.org/ja/manual/script-api-url
         */
        apiUrl(siteId: number, action: PleasanterApiAction): string;
        /**
         * 指定したサイトの情報を取得します。
         *
         * @param options id（対象サイトID）とdone/fail/alwaysコールバックを指定します。
         *
         * @see https://pleasanter.org/ja/manual/script-api-get-site
         */
        apiGetSite(options: PleasanterApiOptions): void;
        /**
         * 指定した親サイトの下にサイトを作成します。親サイトIDが0の場合はトップに作成します。
         *
         * @param options id（親サイトID）とdata（作成内容）、done/fail/alwaysコールバックを指定します。
         *
         * @see https://pleasanter.org/ja/manual/script-api-create-site
         */
        apiCreateSite(options: PleasanterApiOptions): void;
        /**
         * 指定したサイトを更新します。
         *
         * @param options id（対象サイトID）とdata（更新内容）、done/fail/alwaysコールバックを指定します。
         *
         * @see https://pleasanter.org/ja/manual/script-api-update-site
         */
        apiUpdateSite(options: PleasanterApiOptions): void;
        /**
         * 指定したサイトを削除します。
         *
         * @param options id（対象サイトID）とdone/fail/alwaysコールバックを指定します。
         *
         * @see https://pleasanter.org/ja/manual/script-api-delete-site
         */
        apiDeleteSite(options: PleasanterApiOptions): void;
        /**
         * サイト名検索で該当サイトに最も近いサイトのIDを取得します。
         *
         * @param options id（検索起点のサイトID）とdata（サイト名）、done/fail/alwaysコールバックを指定します。
         *
         * @see https://pleasanter.org/ja/manual/script-api-get-closest-siteid
         */
        apiGetClosestSiteId(options: PleasanterApiOptions): void;
        /**
         * 指定したユーザの情報を取得します。
         *
         * @param options id（対象ユーザID）とdone/fail/alwaysコールバックを指定します。
         *
         * @see https://pleasanter.org/ja/manual/script-api-users-get
         */
        apiUsersGet(options: PleasanterApiOptions): void;
        /**
         * ユーザを作成します。
         *
         * @param options data（作成内容）とdone/fail/alwaysコールバックを指定します。
         *
         * @see https://pleasanter.org/ja/manual/script-api-users-create
         */
        apiUsersCreate(options: PleasanterApiOptions): void;
        /**
         * 指定したユーザを更新します。
         *
         * @param options id（対象ユーザID）とdata（更新内容）、done/fail/alwaysコールバックを指定します。
         *
         * @see https://pleasanter.org/ja/manual/script-api-users-update
         */
        apiUsersUpdate(options: PleasanterApiOptions): void;
        /**
         * 指定したユーザを削除します。
         *
         * @param options id（対象ユーザID）とdone/fail/alwaysコールバックを指定します。
         *
         * @see https://pleasanter.org/ja/manual/script-api-users-delete
         */
        apiUsersDelete(options: PleasanterApiOptions): void;
        /**
         * 指定した組織の情報を取得します。
         *
         * @param options id（対象組織ID）とdone/fail/alwaysコールバックを指定します。
         *
         * @see https://pleasanter.org/ja/manual/script-api-depts-get
         */
        apiDeptsGet(options: PleasanterApiOptions): void;
        /**
         * 指定したグループの情報を取得します。dataで複数のグループIDを指定することもできます。
         *
         * @param options id（対象グループID）またはdata（複数グループID）、done/fail/alwaysコールバックを指定します。
         *
         * @see https://pleasanter.org/ja/manual/script-api-groups-get
         */
        apiGroupsGet(options: PleasanterApiOptions): void;
        /**
         * グループを作成します。
         *
         * @param options data（作成内容）とdone/fail/alwaysコールバックを指定します。
         *
         * @see https://pleasanter.org/ja/manual/script-api-groups-create
         */
        apiGroupsCreate(options: PleasanterApiOptions): void;
        /**
         * 指定したグループを更新します。
         *
         * @param options id（対象グループID）とdata（更新内容）、done/fail/alwaysコールバックを指定します。
         *
         * @see https://pleasanter.org/ja/manual/script-api-groups-update
         */
        apiGroupsUpdate(options: PleasanterApiOptions): void;
        /**
         * 指定したグループを削除します。
         *
         * @param options id（対象グループID）とdone/fail/alwaysコールバックを指定します。
         *
         * @see https://pleasanter.org/ja/manual/script-api-groups-delete
         */
        apiGroupsDelete(options: PleasanterApiOptions): void;
        /**
         * 指定したサイトの設定を使用してメールを送信します。
         *
         * @param options id（対象サイトID）とdata（To/Cc/Bcc/Title/Body）、done/fail/alwaysコールバックを指定します。
         *
         * @example
         * $p.apiSendMail({
         *     id: 123,
         *     data: {
         *         To: 'xxxxx@example.com',
         *         Title: 'メール件名',
         *         Body: 'メール本文'
         *     }
         * });
         *
         * @see https://pleasanter.org/ja/manual/script-api-send-mail
         */
        apiSendMail(options: PleasanterApiOptions): void;
    }

    /** APIのURLで指定する操作の種類です。 */
    type PleasanterApiAction = 'get' | 'create' | 'update' | 'delete' | (string & {});

    /** Ajax APIのオプションです。 */
    interface PleasanterApiOptions {
        /** 操作対象のサイトIDまたはレコードIDです。 */
        id?: number;
        /** POSTするJSONデータです。 */
        data?: unknown;
        /** API通信成功時の処理です。 */
        done?: (data: unknown) => void;
        /** API通信失敗時の処理です。 */
        fail?: (data: unknown) => void;
        /** API通信完了時の処理です。 */
        always?: (data?: unknown) => void;
    }

    type PleasanterScriptControlEventBaseName =
        'before_validate' | 'after_validate' | 'before_send' | 'after_send' | 'before_set' | 'after_set';
    /** コントロールIDまたはdata-actionを末尾に指定できるイベント名です。 */
    type PleasanterScriptControlEventName = `${PleasanterScriptControlEventBaseName}_${string}`;
    type PleasanterScriptControlEvents = {
        [K in PleasanterScriptControlEventName]?: (args: unknown) => boolean | void;
    };

    interface PleasanterScriptEvents extends PleasanterScriptControlEvents {
        /**
         * 編集画面を読み込んだときに実行します。
         *
         * @remarks 同じイベントを複数のスクリプトで設定した場合は、後から読み込まれた設定で上書きされます。
         *
         * @example
         * $p.events.on_editor_load = function () {
         *     console.log($p.getValue('ClassA'));
         * };
         *
         * @see https://pleasanter.org/ja/manual/script-events-on-editor-load
         */
        on_editor_load?: () => void;
        /**
         * 一覧またはダッシュボードを読み込んだとき、およびフィルタ変更後に実行します。
         *
         * @remarks 同じイベントを複数のスクリプトで設定した場合は、後から読み込まれた設定で上書きされます。
         *
         * @see https://pleasanter.org/ja/manual/script-events-on-grid-load
         */
        on_grid_load?: () => void;
        /**
         * カレンダーを読み込んだとき、または表示内容を変更したときに実行します。
         *
         * @remarks 同じイベントを複数のスクリプトで設定した場合は、後から読み込まれた設定で上書きされます。
         *
         * @see https://pleasanter.org/ja/manual/script-events-on-calendar-load
         */
        on_calendar_load?: () => void;
        /**
         * クロス集計を読み込んだとき、または表示内容を変更したときに実行します。
         *
         * @remarks 同じイベントを複数のスクリプトで設定した場合は、後から読み込まれた設定で上書きされます。
         *
         * @see https://pleasanter.org/ja/manual/script-events-on-crosstab-load
         */
        on_crosstab_load?: () => void;
        /**
         * ガントチャートを読み込んだとき、または表示内容を変更したときに実行します。
         *
         * @remarks 同じイベントを複数のスクリプトで設定した場合は、後から読み込まれた設定で上書きされます。
         *
         * @see https://pleasanter.org/ja/manual/script-events-on-gantt-load
         */
        on_gantt_load?: () => void;
        /**
         * バーンダウンチャートを読み込んだとき、または表示内容を変更したときに実行します。
         *
         * @remarks 同じイベントを複数のスクリプトで設定した場合は、後から読み込まれた設定で上書きされます。
         *
         * @see https://pleasanter.org/ja/manual/script-events-on-burndown-load
         */
        on_burndown_load?: () => void;
        /**
         * 時系列チャートを読み込んだとき、または表示内容を変更したときに実行します。
         *
         * @remarks 同じイベントを複数のスクリプトで設定した場合は、後から読み込まれた設定で上書きされます。
         *
         * @see https://pleasanter.org/ja/manual/script-events-on-timeseries-load
         */
        on_timeseries_load?: () => void;
        /**
         * 分析チャートを読み込んだとき、または表示内容を変更したときに実行します。
         *
         * @remarks 同じイベントを複数のスクリプトで設定した場合は、後から読み込まれた設定で上書きされます。
         *
         * @see https://pleasanter.org/ja/manual/script-events-on-analy-load
         */
        on_analy_load?: () => void;
        /**
         * カンバンを読み込んだとき、または表示内容を変更したときに実行します。
         *
         * @remarks 同じイベントを複数のスクリプトで設定した場合は、後から読み込まれた設定で上書きされます。
         *
         * @see https://pleasanter.org/ja/manual/script-events-on-kamban-load
         */
        on_kamban_load?: () => void;
        /**
         * 入力検証の前に実行します。
         *
         * @param args イベント情報（URL、メソッド種別、送信データなど）。
         * @returns falseを返すと以降の検証・送信処理を中止します。
         *
         * @remarks 同じイベントを複数のスクリプトで設定した場合は、後から読み込まれた設定で上書きされます。
         *
         * @example
         * $p.events.before_validate = function (args) {
         *     if ($p.getValue('ClassA') === '') {
         *         return false;
         *     }
         * };
         *
         * @see https://pleasanter.org/ja/manual/script-events-before-validate
         */
        before_validate?: (args: unknown) => boolean | void;
        /**
         * 入力検証の後に実行します。
         *
         * @param args イベント情報（URL、メソッド種別、送信データなど）。
         * @returns falseを返すと以降の送信処理を中止します。
         *
         * @remarks 同じイベントを複数のスクリプトで設定した場合は、後から読み込まれた設定で上書きされます。
         *
         * @see https://pleasanter.org/ja/manual/script-events-after-validate
         */
        after_validate?: (args: unknown) => boolean | void;
        /**
         * サーバへデータを送信する前に実行します。
         *
         * @param args イベント情報（URL、メソッド種別、送信データなど）。
         * @returns falseを返すと送信処理を中止します。
         *
         * @remarks 同じイベントを複数のスクリプトで設定した場合は、後から読み込まれた設定で上書きされます。
         *
         * @see https://pleasanter.org/ja/manual/script-events-before-send
         */
        before_send?: (args: unknown) => boolean | void;
        /**
         * サーバへデータを送信した後に実行します。
         *
         * @param args イベント情報（URL、メソッド種別、レスポンスなど）。
         *
         * @remarks 戻り値は処理に影響しません。同じイベントを複数のスクリプトで設定した場合は、後から読み込まれた設定で上書きされます。
         *
         * @see https://pleasanter.org/ja/manual/script-events-after-send
         */
        after_send?: (args: unknown) => void;
        /**
         * レスポンスを受信して画面を更新する前に実行します。
         *
         * @param args イベント情報（URL、メソッド種別、レスポンスなど）。
         *
         * @remarks 戻り値は処理に影響しません。同じイベントを複数のスクリプトで設定した場合は、後から読み込まれた設定で上書きされます。
         *
         * @see https://pleasanter.org/ja/manual/script-events-before-set
         */
        before_set?: (args: unknown) => void;
        /**
         * レスポンスを画面へ反映した後に実行します。
         *
         * @param args イベント情報（URL、メソッド種別、レスポンスなど）。
         *
         * @remarks 戻り値は処理に影響しません。同じイベントを複数のスクリプトで設定した場合は、後から読み込まれた設定で上書きされます。
         *
         * @see https://pleasanter.org/ja/manual/script-events-after-set
         */
        after_set?: (args: unknown) => void;
    }
}
