<script lang="ts">
    import { onMount, onDestroy } from 'svelte';
    import flatpickr from 'flatpickr';
    import { format as dFormat, addDays } from 'date-fns';
    import type { Options } from 'flatpickr/dist/types/options';
    import type { Instance } from 'flatpickr/dist/types/instance';
    import { Japanese } from 'flatpickr/dist/l10n/ja.js';
    import ReadField from './ReadField.svelte';
    import { pDisplay } from '../$p';

    export let model: string | undefined;
    export let required: boolean | undefined = undefined;
    export let readOnly: boolean | undefined = undefined;
    export let fieldSize: string | undefined = undefined;
    export let align: number | undefined = undefined;
    export let step: number | undefined = undefined;
    export let diff: number | undefined = undefined;
    export let format: 'Ymd' | 'Ymdhm' | 'Ymdhms' = 'Ymd';
    export let targetElem: HTMLElement | undefined = undefined;
    export let placeholder: string | undefined = undefined;
    export let onInput: ((event: InputEvent) => void) | undefined = undefined;
    export let onChange: ((event: Event) => void) | undefined = undefined;

    let inputElm: HTMLInputElement;
    let shadowStage: HTMLElement;
    let fp: Instance;
    let errorMessage: string | undefined = undefined;
    let clientLang = (document.getElementById('Language') as HTMLInputElement).value;
    let dateFormat: { Ymd: string; Ymdhm: string; Ymdhms: string };
    let dateFnsFormat: { Ymd: string; Ymdhm: string; Ymdhms: string };

    switch (clientLang) {
        case 'zh':
        case 'ja':
        case 'es':
        case 'vn':
            dateFormat = {
                Ymd: 'Y/m/d',
                Ymdhm: 'Y/m/d H:i',
                Ymdhms: 'Y/m/d H:i:S'
            };
            dateFnsFormat = {
                Ymd: 'yyyy/MM/dd',
                Ymdhm: 'yyyy/MM/dd HH:mm',
                Ymdhms: 'yyyy/MM/dd HH:mm:ss'
            };
            break;
        case 'ko':
            dateFormat = {
                Ymd: 'Y.m.d.',
                Ymdhm: 'Y.m.d. H:i',
                Ymdhms: 'Y.m.d. H:i:S'
            };
            dateFnsFormat = {
                Ymd: 'yyyy.MM.dd.',
                Ymdhm: 'yyyy.MM.dd. HH:mm',
                Ymdhms: 'yyyy.MM.dd. HH:mm:ss'
            };
            break;
        case 'de':
            dateFormat = {
                Ymd: 'Y.m.d',
                Ymdhm: 'Y.m.d H:i',
                Ymdhms: 'Y.m.d H:i:S'
            };
            dateFnsFormat = {
                Ymd: 'yyyy.MM.dd',
                Ymdhm: 'yyyy.MM.dd HH:mm',
                Ymdhms: 'yyyy.MM.dd HH:mm:ss'
            };
            break;
        default:
            dateFormat = {
                Ymd: 'm/d/Y',
                Ymdhm: 'm/d/Y H:i',
                Ymdhms: 'm/d/Y H:i:S'
            };
            dateFnsFormat = {
                Ymd: 'MM/dd/yyyy',
                Ymdhm: 'MM/dd/yyyy HH:mm',
                Ymdhms: 'MM/dd/yyyy HH:mm:ss'
            };
            break;
    }

    const fpOptions: Options = {
        minuteIncrement: step || 5,
        enableTime: false,
        allowInput: true,
        enableSeconds: format === 'Ymdhms' ? true : false,
        disableMobile: true,
        dateFormat: format ? dateFormat[format] : dateFormat.Ymd
    };
    if (clientLang === 'ja') {
        fpOptions.locale = Japanese;
    }

    $: if (format) {
        if (inputElm && !readOnly) {
            if (fp) fp.destroy();
            const setStep = step || 10;
            fpOptions['dateFormat'] = dateFormat[format] || dateFormat.Ymd;
            fpOptions['enableTime'] = format === 'Ymd' ? false : true;
            fpOptions['enableSeconds'] = format === 'Ymdhms' ? true : false;
            fpOptions['minuteIncrement'] = format === 'Ymdhm' ? setStep : 10;
            fp = flatpickr(inputElm, fpOptions);
        }
    }

    $: if (step) {
        if (inputElm && !readOnly) {
            fp.destroy();
            const setStep = step || 10;
            fpOptions['minuteIncrement'] = format === 'Ymdhm' ? setStep : 10;
            fp = flatpickr(inputElm, fpOptions);
        }
    }

    $: if (diff || diff === 0) {
        onSetDateValue(diff);
    }

    $: if (required !== undefined) onValidation();
    const onValidation = () => {
        if (required && !model) {
            errorMessage = pDisplay('ValidateRequired');
        } else if (errorMessage) {
            errorMessage = undefined;
        }
    };

    $: if (targetElem && !readOnly) {
        setTimeout(() => {
            createDatePiker();
        });
    }

    const onSetDateValue = (diff: number | undefined = 0) => {
        let now = new Date();
        if (diff) {
            now = addDays(now, diff);
        }
        model = dFormat(now, dateFnsFormat[format]);
    };

    const createDatePiker = () => {
        if (fp) fp.destroy();
        if (shadowStage !== undefined) shadowStage.remove();
        shadowStage = document.createElement('div');
        const style = document.createElement('style');
        const ApplicationPath = document.querySelector('#ApplicationPath') as HTMLInputElement;
        style.textContent = /*css*/ `
            @import url('${ApplicationPath.value}assets/plugins/flatpickr/flatpickr.min.css');

            .flatpickr-months .flatpickr-month,
            .flatpickr-weekdays{
                background: var(--primaryColor);
            }
            .flatpickr-months .flatpickr-month{
                border-radius: 5px 5px 0 0;
            }
            .flatpickr-current-month .flatpickr-monthDropdown-months,
            .flatpickr-current-month input.cur-year,
            span.flatpickr-weekday{
                font-size: 15px;
                font-weight: normal;
                color: var(--nonColor16);
                option{
                color: var(--nonColor01);
                }
            }
            .flatpickr-current-month .numInputWrapper span.arrowUp:after {
                border-bottom-color: var(--nonColor16);
            }
            .flatpickr-current-month .numInputWrapper span.arrowDown:after {
                border-top-color: var(--nonColor16);
            }
            .flatpickr-months .flatpickr-prev-month,
            .flatpickr-months .flatpickr-next-month{
                color: var(--nonColor16)!important;
                fill: var(--nonColor16)!important;
            }
            .flatpickr-day.selected{
                background: var(--primaryColor);
                color: var(--nonColor16);
                border-color: var(--primaryColor);
            }
            .flatpickr-day.selected:hover{
                background: var(--primaryDark);
                border-color: var(--primaryDark);
            }
        `;
        const shadowRoot = shadowStage.attachShadow({ mode: 'open' });
        const rootElem = targetElem || document.querySelector('body');
        shadowRoot.appendChild(style);
        rootElem?.append(shadowStage);
        fpOptions['appendTo'] = shadowRoot as unknown as HTMLElement;
        fpOptions.onOpen = (selectedDates, dateStr, instance) => {
            if (rootElem)
                setTimeout(() => {
                    let top = Number(instance.calendarContainer.style.top.split('px')[0]);
                    instance.calendarContainer.style.top = `${top + rootElem.scrollTop}px`;
                });
        };
        fp = flatpickr(inputElm, fpOptions);
    };

    onMount(() => {
        if (targetElem && !readOnly) {
            createDatePiker();
        }
    });

    onDestroy(() => {
        if (fp) fp.destroy();
        if (shadowStage) shadowStage.remove();
    });
</script>

{#if !readOnly}
    <label class="textfield" class:is-center={align === 15} class:is-right={align === 20}>
        <div class="field-inner">
            <input
                type="text"
                bind:this={inputElm}
                bind:value={model}
                {placeholder}
                on:input={onInput}
                on:input={onValidation}
                on:change={onChange}
                autocomplete="off"
            />
            <button
                type="button"
                class="set-today"
                on:click={() => {
                    onSetDateValue();
                }}><span class="material-symbols-sharp is-fill">schedule</span></button
            >
        </div>
        {#if errorMessage && model !== undefined}
            <p class="error">{errorMessage}</p>
        {/if}
    </label>
{:else}
    <ReadField {model} {align} {fieldSize} />
{/if}

<style lang="scss">
    @use '../shared';

    .shadow-stage {
        position: fixed;
        top: 0;
        left: 0;
        z-index: 101;
    }
    .textfield {
        text-align: left;
        .field-inner {
            position: relative;
            .set-today {
                position: absolute;
                top: 50%;
                right: 0;
                display: flex;
                align-items: center;
                justify-content: center;
                width: 24px;
                height: 32px;
                color: var(--control-text);
                cursor: pointer;
                outline: none;
                background-color: transparent;
                border: 0;
                transform: translateY(-50%);
                transition: background-color 150ms ease-out;
                span {
                    font-size: 16px;
                    &::selection {
                        background-color: transparent;
                    }
                }
            }
        }
        input {
            width: 100%;
            height: 32px;
            padding: 4px 24px 4px 8px;
            color: var(--control-text);
            outline: none;
            background-color: var(--control-bg);
            border: 1px solid var(--control-border);
            border-radius: 4px;
            transition:
                color 150ms ease-out,
                background 150ms ease-out,
                border 150ms ease-out;
            &:focus {
                background-color: var(--control-bg-focus);
                border-color: var(--control-border-focus);
            }
            &::placeholder {
                color: silver;
            }
        }
        .error {
            padding: 4px 0 0;
            color: var(--control-error);
        }
        &.is-center {
            text-align: center;
            input {
                text-align: center;
            }
        }
        &.is-right {
            text-align: right;
            input {
                text-align: right;
            }
        }
    }
</style>
