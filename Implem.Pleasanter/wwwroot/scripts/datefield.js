class InputDate extends HTMLElement {
    constructor() {
        super();
        this.attachShadow({ mode: 'open' });
    }

    connectedCallback() {
        this.shadowRoot.innerHTML = this.render();
        if(!this.querySelector('input')) return;

        const params = {
            isRwd: $('head').css('font-family') === 'responsive',
            inputElm: this.querySelector('input'),
            dateFormat: this.querySelector('input').dataset.format.replace(/s/g, 'S'),
            dateFnsFormat: null,
            language: document.getElementById('Language').value,
            currentElem: this.shadowRoot.querySelector('.current-date'),
            timeZoneOffset: document.getElementById('TimeZoneOffset').value,
        };

        const init = () => {
            //flatpickrの初期化
            initDatePicker();

            // formatを取得
            setDateFormat();

            // current-dateボタンのイベントを追加
            params.currentElem.addEventListener('click', onCurrent);
        };

        const initDatePicker = () => {
            this.dataPicker = flatpickr(params.inputElm, {
                locale: {
                    ...flatpickr.l10ns.default,
                    ...(params.language === 'ja' ? flatpickr.l10ns.ja : {}),
                    firstDayOfWeek: 1
                },
                enableTime: params.inputElm.dataset.timepicker === '1',
                enableSeconds: params.inputElm.dataset.format?.includes(':s') || false,
                minuteIncrement: Number(params.inputElm.dataset.step || 1),
                allowInput: !params.isRwd ? true : false,
                disableMobile: true,
                dateFormat: params.dateFormat,
                onReady: function(selectedDates, dateStr, instance) {
                    const timeInputs = instance.timeContainer?.querySelectorAll("input[type='number']");
                    if (timeInputs) {
                        timeInputs.forEach(input => {
                            input.addEventListener("focus", function (e) {
                                input.type = "text";
                                const val = input.value;
                                input.setSelectionRange(val.length, val.length);
                                input.type = "number";
                            });
                        });
                    }
                }
            });
        };

        const onCurrent = () => {
            params.inputElm.value = moment().utcOffset(params.timeZoneOffset).format(params.dateFnsFormat);
            $p.set($(params.inputElm), params.inputElm.value);
            this.dataPicker.setDate(params.inputElm.value, false);
            params.inputElm.dispatchEvent(new Event('change'));
        }

        const setDateFormat = () => {
            let dateFnsFormat;
            switch (params.inputElm.dataset.format) {
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
            params.dateFnsFormat = dateFnsFormat;
        };


        if (typeof flatpickr !== 'undefined' ){
            return init();
        }else{
            window.addEventListener('load', () => {
                return init();
            });
        }
    }

    disconnectedCallback() {
        if (this.dataPicker) this.dataPicker.destroy();
    }

    render() {
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
                padding 0;
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
