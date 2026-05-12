<script lang="ts">
    import ReadField from './ReadField.svelte';
    import { pDisplay } from '../$p';

    export let model: string | number | undefined;
    export let required: boolean | undefined = undefined;
    export let readOnly: boolean | undefined = undefined;
    export let fieldSize: string | undefined = undefined;
    export let align: number | undefined = undefined;
    export let maxLength: number | undefined = undefined;
    export let placeholder: string | undefined = undefined;
    export let onInit: ((model: string | number | undefined) => void) | undefined = undefined;
    export let onInput: ((event: InputEvent) => void) | undefined = undefined;

    let errorMessage: string | undefined = undefined;

    $: if (required !== undefined) onValidation();
    $: if (maxLength !== undefined) onValidation();
    const onValidation = () => {
        if (maxLength !== undefined && maxLength > 0 && model) {
            if (maxLength < String(model).length) {
                errorMessage = pDisplay('ValidateMaxLength', maxLength);
            } else {
                errorMessage = undefined;
            }
        } else if (required && model === '') {
            errorMessage = pDisplay('ValidateRequired');
        } else if (errorMessage) {
            errorMessage = undefined;
        }
    };
    if (onInit) {
        setTimeout(() => {
            onInit(model);
        }, 10);
    }
</script>

{#if !readOnly}
    <label class="textfield" class:is-center={align === 15} class:is-right={align === 20}>
        <input type="text" bind:value={model} {placeholder} on:input={onInput} on:input={onValidation} />
        {#if errorMessage && model !== undefined}
            <p class="error">{errorMessage}</p>
        {/if}
    </label>
{:else}
    <ReadField {model} {align} {fieldSize} />
{/if}

<style lang="scss">
    @use '../shared';
    .textfield {
        text-align: left;
        input {
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
