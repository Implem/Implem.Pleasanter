import flatpickr from 'flatpickr';
import { Japanese } from 'flatpickr/dist/l10n/ja.js';
import { German } from 'flatpickr/dist/l10n/de.js';
import { Korean } from 'flatpickr/dist/l10n/ko.js';
import { Spanish } from 'flatpickr/dist/l10n/es.js';
import { Mandarin } from 'flatpickr/dist/l10n/zh.js';
import { MandarinTraditional } from 'flatpickr/dist/l10n/zh-tw.js';
import { Vietnamese } from 'flatpickr/dist/l10n/vn.js';
import type { Options } from 'flatpickr/dist/types/options';
import type { CustomLocale } from 'flatpickr/dist/types/locale';
import type { Instance } from 'flatpickr/dist/types/instance';
import moment from 'moment';
import $ from 'jquery';

// Pleasanter の言語コードに対応する flatpickr のロケール。
// en は flatpickr の既定のため定義しない。
// 言語を追加する場合はここに追記する。
const locales: Record<string, CustomLocale> = {
    ja: Japanese,
    de: German,
    ko: Korean,
    es: Spanish,
    zh: Mandarin,
    zh_tw: MandarinTraditional,
    vn: Vietnamese
};

class InputDate extends HTMLElement {
    static isRwd: boolean;
    static language: string;
    static timeZoneOffset: string;
    private dataPicker?: Instance;
    private inputElm: HTMLInputElement | null = null;
    private currentElem: HTMLElement | null = null;
    private dateFormat: string = 'Y/m/d H:i';
    private dateFnsFormat?: string | null;

    constructor() {
        super();
        this.attachShadow({ mode: 'open' });
    }

    // 表示言語に対応する flatpickr のロケールを返す。
    // 該当が無い場合は空とし、flatpickr の既定 (英語) に委ねる。
    static locale(): Partial<CustomLocale> {
        return locales[InputDate.language] ?? {};
    }

    // 週の開始曜日 (0:日曜〜6:土曜) を、日付項目単位の設定、ページ全体の設定の順に参照する。
    // 取得できない場合は undefined を返し、flatpickr のロケール既定に委ねる。
    static firstDayOfWeek(inputElm: HTMLInputElement): number | undefined {
        const value =
            inputElm.dataset.firstdayofweek ??
            (document.getElementById('ResolvedFirstDayOfWeek') as HTMLInputElement)?.value;
        if (value === undefined || value === '') return undefined;
        const parsed = Number(value);
        return Number.isNaN(parsed) || parsed < 0 || parsed > 6 ? undefined : parsed;
    }

    connectedCallback() {
        if (!this.shadowRoot) return;
        this.shadowRoot.innerHTML = this.render();
        this.inputElm = this.querySelector('input');
        if (!this.inputElm) return;

        if (InputDate.isRwd === undefined) {
            InputDate.isRwd = $('head').css('font-family') === 'responsive';
        }
        if (InputDate.language === undefined) {
            InputDate.language = (document.getElementById('Language') as HTMLInputElement)?.value ?? 'ja';
        }
        if (InputDate.timeZoneOffset === undefined) {
            InputDate.timeZoneOffset = (document.getElementById('TimeZoneOffset') as HTMLInputElement)?.value ?? '0';
        }

        this.currentElem = this.shadowRoot.querySelector('.current-date');
        if (this.dataset.hideCurrent) {
            const currentDateElem = this.shadowRoot.querySelector('.current-date');
            currentDateElem?.remove();
        }
        if (this.inputElm.dataset.format) {
            this.dateFormat = this.inputElm.dataset.format.replace(/s/g, 'S');
        }

        this.init();
    }

    disconnectedCallback() {
        if (this.dataPicker) this.dataPicker.destroy();
        if (this.inputElm) this.inputElm.removeEventListener('change', this.onChange);
        if (this.currentElem) this.currentElem.removeEventListener('click', this.onCurrent);
    }

    private init() {
        this.initDatePicker();
        this.setDateFormat();

        if (this.inputElm) this.inputElm.addEventListener('change', this.onChange);
        if (this.currentElem) this.currentElem.addEventListener('click', this.onCurrent);
    }

    private setDateFormat() {
        let dateFnsFormat: string | undefined;
        switch (this.inputElm?.dataset.format) {
            case 'Y/m/d':
                dateFnsFormat = 'YYYY/MM/DD';
                break;
            case 'Y.m.d':
                dateFnsFormat = 'YYYY.MM.DD';
                break;
            case 'Y.m.d.':
                dateFnsFormat = 'YYYY.MM.DD.';
                break;
            case 'm/d/Y':
                dateFnsFormat = 'MM/DD/YYYY';
                break;
            case 'Y/m/d H:i':
                dateFnsFormat = 'YYYY/MM/DD HH:mm';
                break;
            case 'Y.m.d H:i':
                dateFnsFormat = 'YYYY.MM.DD HH:mm';
                break;
            case 'Y.m.d. H:i':
                dateFnsFormat = 'YYYY.MM.DD. HH:mm';
                break;
            case 'm/d/Y H:i':
                dateFnsFormat = 'MM/DD/YYYY HH:mm';
                break;
            case 'Y/m/d H:i:s':
                dateFnsFormat = 'YYYY/MM/DD HH:mm:ss';
                break;
            case 'Y.m.d H:i:s':
                dateFnsFormat = 'YYYY.MM.DD HH:mm:ss';
                break;
            case 'Y.m.d. H:i:s':
                dateFnsFormat = 'YYYY.MM.DD. HH:mm:ss';
                break;
            case 'm/d/Y H:i:s':
                dateFnsFormat = 'MM/DD/YYYY HH:mm:ss';
                break;
        }
        this.dateFnsFormat = dateFnsFormat;
    }

    private onChange = () => {
        if (!this.inputElm) return;
        $p.set($(this.inputElm), this.inputElm.value);
        if (this.inputElm.classList.contains('auto-postback')) {
            $p.send($(this.inputElm));
        }
    };

    private onCurrent = () => {
        if (!this.inputElm || !this.dateFnsFormat) return;
        this.inputElm.value = moment().utcOffset(InputDate.timeZoneOffset).format(this.dateFnsFormat);
        $p.set($(this.inputElm), this.inputElm.value);
        if (this.dataPicker) this.dataPicker.setDate(this.inputElm.value, false);
        this.inputElm.dispatchEvent(
            new Event('change', {
                bubbles: true,
                cancelable: true
            })
        );
    };

    private initDatePicker() {
        if (!this.inputElm) return;
        const dialog = this.inputElm.closest('.ui-dialog');
        const firstDayOfWeek = InputDate.firstDayOfWeek(this.inputElm);
        const fpOptions: Options = {
            locale: Object.assign(
                {},
                flatpickr.l10ns.default,
                InputDate.locale(),
                firstDayOfWeek !== undefined ? { firstDayOfWeek } : {}
            ),
            appendTo: dialog ? (dialog as HTMLElement) : document.body,
            positionElement: this.inputElm,
            enableTime: this.inputElm.dataset.timepicker === '1',
            enableSeconds: (this.inputElm.dataset.format && this.inputElm.dataset.format.includes(':s')) || false,
            minuteIncrement: Number(this.inputElm.dataset.step || 1),
            allowInput: !InputDate.isRwd ? true : false,
            disableMobile: true,
            dateFormat: this.dateFormat,
            onOpen: function (_selectedDates: Date[], _dateStr: string, instance: Instance) {
                if (dialog) {
                    requestAnimationFrame(() => {
                        const cal = instance.calendarContainer as HTMLElement;
                        const dialogRect = dialog.getBoundingClientRect();
                        const top = parseFloat(cal.style.top) - parseFloat(dialogRect.top.toString()) - window.scrollY;
                        const left =
                            parseFloat(cal.style.left) - parseFloat(dialogRect.left.toString()) - window.scrollX;
                        cal.style.top = `${top}px`;
                        cal.style.left = `${left}px`;
                    });
                }
            },

            onReady: function (_selectedDates: Date[], _dateStr: string, instance: Instance) {
                if (!instance.timeContainer) return false;
                const timeInputs: NodeListOf<HTMLInputElement> =
                    instance.timeContainer.querySelectorAll("input[type='number']");
                if (timeInputs) {
                    timeInputs.forEach((input: HTMLInputElement) => {
                        input.addEventListener('focus', function () {
                            input.type = 'text';
                            const val = input.value;
                            input.setSelectionRange(val.length, val.length);
                            input.type = 'number';
                        });
                    });
                }
            }
        };

        this.dataPicker = flatpickr(this.inputElm as HTMLElement, fpOptions);
    }

    render(): string {
        return `
        <div class='field-date'>
            <div class='input-date'>
                <slot></slot>
            </div>
            <button type='button' class='current-date'>
                <span class='material-symbols-sharp is-fill'>schedule</span>
            </button>
        </div>

        <style>
            .field-date {
                position: relative;
            }
            .current-date {
                position: absolute;
                top: 0;
                right: 0;
                z-index: 2;
                display: flex;
                align-items: center;
                justify-content: center;
                width: 24px;
                height: 100%;
                margin: 0;
                padding: 0;
                color: var(--base-text);
                background: transparent;
                border: none;
                outline: none;
                cursor: pointer;
                font-size: 16px;
            }
            .material-symbols-sharp {
                font-family: 'Material Symbols outlined';
                font-weight: normal;
                font-style: normal;
                font-variation-settings: 'FILL' 1, 'wght' 400, 'GRAD' 0, 'opsz' 24;
                vertical-align: bottom;
                line-height: 1;
                letter-spacing: normal;
                text-transform: none;
                display: inline-block;
                white-space: nowrap;
                word-wrap: normal;
                direction: ltr;
                font-feature-settings: 'liga';
                -webkit-font-feature-settings: 'liga';
                -webkit-font-smoothing: antialiased;
                -moz-osx-font-smoothing: grayscale;
            }
        </style>
        `;
    }
}

window.customElements.define('date-field', InputDate);

export default InputDate;
