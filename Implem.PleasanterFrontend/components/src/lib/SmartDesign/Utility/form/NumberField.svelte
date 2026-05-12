<script lang="ts">
    import ReadField from './ReadField.svelte';
    import { pDisplay } from '../$p';

    export let model: number | string | null | undefined;
    export let required: boolean | undefined = undefined;
    export let readOnly: boolean | undefined = undefined;
    export let fieldSize: string | undefined = undefined;
    export let align: number | undefined = undefined;
    export let unit: string | undefined = undefined;
    export let isSpinner: boolean | undefined = false;
    export let max: number | null | undefined = undefined;
    export let min: number | null | undefined = undefined;
    export let step: number | null | undefined = undefined;
    export let placeholder: string | undefined = undefined;
    export let onInput: ((event: Event) => void) | undefined = undefined;

    let lastKey: string | undefined = undefined;
    let oldValue: number | null | undefined = undefined;
    let input: HTMLInputElement;
    let errorMessage: string | undefined = undefined;

    $: if (readOnly !== undefined) onValidation();
    $: if (required !== undefined) onValidation();
    $: if (max !== undefined) onValidation();
    $: if (min !== undefined) onValidation();
    $: {
        if (model !== oldValue) {
            if ((lastKey === '-' || lastKey === '+') && oldValue && !model) {
                model = oldValue;
            } else {
                oldValue = Number(model);
            }
        }
    }

    const onValidation = () => {
        errorMessage = undefined;
        if (isNumCheck(Number(model)) && model !== null) {
            if (isNumCheck(max) && Number(max) < Number(model)) {
                if (isSpinner) {
                    model = max;
                } else {
                    errorMessage = pDisplay('ValidateMaxNumber');
                }
            }
            if (isNumCheck(min) && Number(model) < Number(min)) {
                if (isSpinner) {
                    model = min;
                } else {
                    errorMessage = pDisplay('ValidateMinNumber');
                }
            }
        } else {
            if (required && model === null) {
                errorMessage = pDisplay('ValidateRequired');
            }
        }
    };

    const isNumCheck = (value: number | undefined | null) => {
        return typeof value === 'number' && !isNaN(value);
    };

    const onSpinner = (vector: 'up' | 'down') => {
        switch (vector) {
            case 'up':
                input.stepUp();
                break;
            case 'down':
                input.stepDown();
                break;
        }
        input.dispatchEvent(new Event('input'));
    };

    const onKeyDown = (event: KeyboardEvent) => {
        lastKey = event.key.toLowerCase();
        if (lastKey === 'e') {
            event.preventDefault();
        }
    };
</script>

{#if !readOnly}
    {#if isSpinner}
        <div class="spinnerfield" class:is-center={align === 15} class:is-right={align === 20}>
            <label class="spinner-wrap">
                <div class="spinner-Controller">
                    <input
                        type="number"
                        bind:this={input}
                        bind:value={model}
                        {placeholder}
                        {step}
                        on:input={onInput}
                        on:input={onValidation}
                        on:keydown={onKeyDown}
                        on:wheel|passive
                    />
                    <button type="button" class="spin-btn is-up" on:click={() => onSpinner('up')}>
                        <span class="material-symbols-sharp">arrow_drop_up</span>
                    </button>
                    <button type="button" class="spin-btn is-down" on:click={() => onSpinner('down')}>
                        <span class="material-symbols-sharp">arrow_drop_down</span>
                    </button>
                </div>
                {#if unit}
                    <span class="unit">{unit}</span>
                {/if}
            </label>
            {#if errorMessage}
                <p class="error">{errorMessage}</p>
            {/if}
        </div>
    {:else}
        <label class="numberfield" class:is-center={align === 15} class:is-right={align === 20}>
            <span class="field-inner">
                <input
                    type="number"
                    bind:value={model}
                    {placeholder}
                    on:input={onInput}
                    on:keydown={onKeyDown}
                    on:input={onValidation}
                />
                {#if unit}
                    <span class="unit">{unit}</span>
                {/if}
            </span>
            {#if errorMessage}
                <p class="error">{errorMessage}</p>
            {/if}
        </label>
    {/if}
{:else}
    <ReadField {model} {align} {unit} {fieldSize} />
{/if}

<style lang="scss">
    @use '../shared';

    .spinnerfield {
        text-align: left;
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
        .spinner-wrap {
            display: flex;
            gap: 5px;
            align-items: center;
            text-align: left;
            .spinner-Controller {
                position: relative;
                flex: 1;
                overflow: hidden;
                background-color: var(--control-bg);
                border: 1px solid var(--control-border);
                border-radius: 4px;
                transition:
                    color 150ms ease-out,
                    background 150ms ease-out,
                    border 150ms ease-out;
                &:has(input:focus) {
                    background-color: var(--control-bg-focus);
                    border-color: var(--control-border-focus);
                }
            }
        }
        input {
            width: 100%;
            height: 30px;
            padding: 4px 20px 4px 8px;
            color: var(--control-text);
            outline: none;
            background-color: transparent;
            border: 0;
            &::placeholder {
                color: silver;
            }
            &::-webkit-outer-spin-button,
            &::-webkit-inner-spin-button {
                margin: 0;
                appearance: none;
            }
        }
        .spin-btn {
            position: absolute;
            right: 0;
            display: flex;
            align-items: center;
            justify-content: center;
            width: 20px;
            height: 15px;
            outline: none;
            background: var(--btn-normal-bg);
            border: 0;
            border-left: 1px solid var(--btn-normal-border);
            transition:
                background-color 150ms ease-out,
                box-shadow 150ms ease-out;
            &:hover {
                background: var(--btn-normal-hover);
            }
            &::selection {
                background-color: transparent;
            }
            &.is-up {
                top: 0;
                border-bottom: 1px solid var(--btn-normal-border);
            }
            &.is-down {
                bottom: 0;
            }
            span {
                font-size: 20px;
                &::selection {
                    background-color: transparent;
                }
            }
        }
    }

    .numberfield {
        text-align: left;
        .field-inner {
            display: flex;
            gap: 5px;
            align-items: center;
            width: 100%;
        }
        input {
            flex: 1;
            width: 100%;
            height: 32px;
            padding: 4px 8px;
            color: var(--control-text);
            outline: none;
            background-color: var(--control-bg);
            border: 1px solid var(--control-border);
            border-radius: 4px;
            transition:
                color 150ms ease-out,
                background 150ms ease-out,
                border 150ms ease-out;
            &:has(+ .unit) {
                width: 70%;
            }
            &:focus {
                background-color: var(--control-bg-focus);
                border-color: var(--control-border-focus);
            }
            &::placeholder {
                color: silver;
            }
            &::-webkit-outer-spin-button,
            &::-webkit-inner-spin-button {
                margin: 0;
                appearance: none;
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
