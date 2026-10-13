import type { Calendar } from '@fullcalendar/core';
// $p へ Object.assign で自身を足しているモジュール。
// 足した分の型をここで $p に合流させる (末尾の交差型を参照)。
import { type Display } from '../scripts/generals/display';
import { type HistoryDiff } from '../scripts/generals/historydiff';
import { type Passkey } from '../scripts/generals/passkey';

// グローバルの $p の型定義。
//
// $p は各スクリプトが実行時に機能を足していく入れ物で、実体は一箇所にまとまっていない。
// ここに書くのは TypeScript から参照するものだけなので、この一覧は $p の持ち物すべてではない。
// TypeScript から新しく呼ぶものが増えたら、その都度ここへ足していく。
declare global {
    interface Window {
        $p: {
            // 入力欄を持つ画面か (forms.js)
            isForm: () => boolean;

            // 通信 (_ajax.js)
            multiUpload: (
                url: string,
                data: FormData,
                $control?: JQuery<HTMLElement>,
                statusBar?: object,
                callback?: function
            ) => void;
            ajax: (
                url: string,
                methodType: string,
                data?: unknown,
                $control?: JQuery<HTMLElement> | null,
                _async?: boolean,
                clearMessage?: boolean
            ) => number | false | void;
            // 変更のあったコントロールと常時送信の指定があるものを送る (_form.js)
            send: (target: JQuery<HTMLElement>) => void;
            // send を同期で行う。戻ったときには応答の反映が済んでいる (_form.js)
            syncSend: (target: JQuery<HTMLElement>, value?: string) => void;
            // 画面遷移 (navigation.js)
            transition: (url: string) => void;

            // 画面に持たせている値の出し入れ (_data.js)
            getData: (target: JQuery<HTMLElement>) => Record<string, unknown>;
            setData: (target: JQuery<HTMLElement>) => void;
            clearData: () => void;
            // コントロールへ値を入れる
            set: (target: JQuery<HTMLElement>, value: string) => void;

            // ダイアログ・別画面
            openVideo: (controlId: string) => void;
            openEditorColumnDialog: (target: JQuery<HTMLElement>) => void;
            modal?: Record<string, HTMLElement>;

            // 読み込み中の表示 (loading.js)。loading で出して loaded で消す
            loading: (target?: JQuery<HTMLBUTTONElement>) => void;
            loaded: () => void;

            // サイトの設定で列を選ぶ操作 (fieldselectable.js / sitesettings.js)
            enableColumns: (
                event: Event,
                $control: JQuery<HTMLElement>,
                columnHeader: string,
                columnsTypeControl: string
            ) => void;
            moveColumns: (
                event: Event,
                $control: JQuery<HTMLElement>,
                columnHeader: string,
                isKeepSource?: boolean,
                isJoin?: boolean,
                type?: string
            ) => void;
            resetEditorColumn: (target: JQuery<HTMLElement>) => void;

            // 画面の差し替えをまたいでスクロール位置を保つ (scroll.js)
            saveScroll: () => void;
            loadScroll: () => void;

            // 日付の計算と書式 (_time.js)
            beginningMonth: (date: Date) => Date;
            shortDateString: (date: Date) => string;
            shortDate: (date: Date) => Date;
            dateDiff: (unit: string, date1: Date, date2: Date) => number;
            dateAdd: (unit: string, amount: number, date: Date) => Date;
            dateTimeFormatString: (date: Date, format: string) => string;

            // カレンダー (calendar.ts)
            fullCalendars: Record<string, Calendar>;
            moveCalendar: (type: string, calendarSuffix: string | number) => void;
            setCalendar: (suffix?: string) => void;

            // メッセージ欄 (message.js)
            // value は { Css, Text } を JSON にした文字列
            setMessage: (target: string, value: string) => void;
            // サーバの応答から Message の指定を拾って表示する (_dispatch.js)
            handleMessageFromJson: (jsonData: unknown[]) => boolean;

            validateImageUploadFileSize: (blob: File | Blob | null) => boolean;
        } & typeof Passkey &
            typeof Display &
            typeof HistoryDiff;
    }

    // スクリプトからは window を挟まず $p / FullCalendar として直に参照する
    const $p: Window['$p'];
    const FullCalendar: {
        Calendar: new (el: HTMLElement, optionOverrides?: Record<string, unknown>) => Calendar;
    };
}

export {};
