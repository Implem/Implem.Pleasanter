declare const $: JQueryStatic;
declare let $p: { set: (target: JQuery<HTMLElement>, value: string) => void };

// eslint-disable-next-line @typescript-eslint/no-explicit-any
declare let flatpickr: any;
// eslint-disable-next-line @typescript-eslint/no-explicit-any
declare let moment: any;

class InputDate extends HTMLElement {
    private static isRwd: boolean = $('head').css('font-family') === 'responsive';
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    private dataPicker: any;
    private inputElm: HTMLInputElement | null = null;
    private currentElem: HTMLElement | null = null;
    private dateFormat: string = 'Y/m/d H:i';
    private dateFnsFormat?: string | null;
    private language: string = 'ja';
    private timeZoneOffset: string = '0';

    constructor() {
        super();
        this.attachShadow({ mode: 'open' });
    }

    connectedCallback() {
        if (!this.shadowRoot) return;
        this.shadowRoot.innerHTML = this.render();
        this.inputElm = this.querySelector('input');
        if (!this.inputElm) return;

        this.currentElem = this.shadowRoot.querySelector('.current-date');
        if (this.dataset.hideCurrent) {
            const currentDateElem = this.shadowRoot.querySelector('.current-date');
            currentDateElem?.remove();
        }

        if (this.inputElm.dataset.format) {
            this.dateFormat = this.inputElm.dataset.format.replace(/s/g, 'S');
        }
        this.language = (document.getElementById('Language') as HTMLInputElement)?.value ?? 'ja';
        this.timeZoneOffset = (document.getElementById('TimeZoneOffset') as HTMLInputElement)?.value ?? '0';

        this.init();
    }

    disconnectedCallback() {
        if (this.dataPicker) this.dataPicker.destroy();
    }

    private init() {
        this.initDatePicker();
        this.setDateFormat();

        if (this.currentElem) this.currentElem.addEventListener('click', this.onCurrent);
    }

    private setDateFormat = () => {
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
    };

    private onCurrent = () => {
        if (!this.inputElm || !this.dateFnsFormat) return;
        this.inputElm.value = moment().utcOffset(this.timeZoneOffset).format(this.dateFnsFormat);
        $p.set($(this.inputElm), this.inputElm.value);
        if (this.dataPicker) this.dataPicker.setDate(this.inputElm.value, false);
        this.inputElm.dispatchEvent(
            new Event('change', {
                bubbles: true,
                cancelable: true
            })
        );
    };

    private initDatePicker = () => {
        if (!this.inputElm) return;
        const dialog = this.inputElm.closest('.ui-dialog');

        this.dataPicker = flatpickr(this.inputElm, {
            locale: Object.assign({}, flatpickr.l10ns.default, this.language === 'ja' ? flatpickr.l10ns.ja : {}, {
                firstDayOfWeek: 1
            }),
            appendTo: dialog ? dialog : document.body,
            positionElement: this.inputElm,
            enableTime: this.inputElm.dataset.timepicker === '1',
            enableSeconds: (this.inputElm.dataset.format && this.inputElm.dataset.format.includes(':s')) || false,
            minuteIncrement: Number(this.inputElm.dataset.step || 1),
            allowInput: !InputDate.isRwd ? true : false,
            disableMobile: true,
            dateFormat: this.dateFormat,
            // eslint-disable-next-line @typescript-eslint/no-explicit-any
            onOpen: function (_selectedDates: Date[], _dateStr: string, instance: any) {
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
            // eslint-disable-next-line @typescript-eslint/no-explicit-any
            onReady: function (_selectedDates: Date[], _dateStr: string, instance: any) {
                if (!instance.timeContainer) return false;
                const timeInputs = instance.timeContainer.querySelectorAll("input[type='number']");
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
        });
    };

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
