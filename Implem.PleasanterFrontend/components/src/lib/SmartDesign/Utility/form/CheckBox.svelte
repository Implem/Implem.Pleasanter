<script lang="ts">
    import { pDisplay } from '../$p';
    export let model: boolean | undefined;
    export let required: boolean | undefined = undefined;
    export let readOnly: boolean | undefined = undefined;
    export let align: number | undefined = undefined;
    export let onChange: ((event: Event) => void) | undefined = undefined;
</script>

<label class="inputCheckbox" class:is-center={align === 15} class:is-right={align === 20}>
    <span class="icon"><input type="checkbox" bind:checked={model} disabled={readOnly} on:change={onChange} /></span>
    <span class="text"><slot /></span>
</label>
{#if required && !model && !readOnly}<p class="error">{pDisplay('ValidateRequired')}</p>{/if}

<style lang="scss">
    @use '../shared';

    .inputCheckbox {
        display: flex;
        gap: 4px;
        cursor: pointer;
        &.is-center {
            justify-content: center;
        }
        &.is-right {
            justify-content: flex-end;
        }
        &:has(input:disabled) {
            cursor: default;
        }
        .icon {
            display: flex;
            flex-shrink: 0;
            align-items: center;
            justify-content: center;
            width: 20px;
            height: 20px;
            margin: 5px 0;
            overflow: hidden;
            background-color: var(--control-bg);
            border: 1px solid var(--control-border);
            border-radius: 4px;
            transition: background-color 150ms ease-out;
            input {
                display: none;
            }
            &::before {
                padding: 0;
                margin: 0;
                font-family: 'Material Symbols Sharp';
                font-size: 18px;
                font-variation-settings:
                    'FILL' 0,
                    'wght' 700,
                    'GRAD' 0,
                    'opsz' 48;
                font-weight: normal;
                font-feature-settings: 'liga';
                line-height: 1;
                color: var(--control-bg);
                text-indent: 0;
                text-transform: none;
                letter-spacing: normal;
                overflow-wrap: normal;
                white-space: nowrap;
                content: 'check';
                background: none;
                direction: ltr;
                -webkit-font-smoothing: antialiased;
            }
            &:has(input:checked) {
                background-color: var(--primaryColor);
                border-color: transparent;
                &::before {
                    color: var(--nonColor16);
                }
            }
            &:has(input:disabled) {
                background-color: var(--control-bg-read);
                border: 1px solid var(--control-border);
                &:has(input:checked) {
                    background-color: var(--control-border);
                    &::before {
                        color: var(--nonColor14);
                    }
                }
                &:not(:has(input:checked)) {
                    &::before {
                        color: var(--control-bg-read);
                    }
                    + .text {
                        color: var(--control-text-read);
                    }
                }
            }
        }
        .text {
            display: block;
            margin: 5px 0;
        }
    }
    .error {
        padding: 4px 0 0;
        color: var(--control-error);
    }
</style>
