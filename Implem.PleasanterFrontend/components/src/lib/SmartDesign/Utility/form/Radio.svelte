<script lang="ts">
    export let model: string | number | boolean | null | undefined;
    export let value: string | number | boolean | undefined;
    export let disabled: boolean | undefined = false;
    export let init: string | number | boolean | null | undefined = undefined;

    if (model === undefined) model = init;
</script>

<label class="inputRadio">
    <span class="icon"
        ><input
            type="radio"
            {disabled}
            checked={model === value}
            on:change={() => {
                model = value;
            }}
        /></span
    ><span class="text"><slot /></span>
</label>

<style lang="scss">
    @use '../shared';

    .inputRadio {
        display: flex;
        gap: 4px;
        align-items: center;
        cursor: pointer;
        &:has(input:disabled) {
            cursor: default;
        }
        .icon {
            display: flex;
            align-items: center;
            justify-content: center;
            width: 20px;
            height: 20px;
            background-color: var(--control-bg);
            border: 1px solid var(--control-border);
            border-radius: 100%;
            transition: background-color 150ms ease-out;
            input {
                display: none;
            }
            &::before {
                width: 12px;
                height: 12px;
                content: ' ';
                background-color: var(--control-bg);
                border-radius: 100%;
                transition: background-color 150ms ease-out;
            }
            &:has(input:checked) {
                background-color: var(--nonColor16);
                &::before {
                    background-color: var(--primaryColor);
                }
            }
            &:has(input:disabled) {
                &,
                &::before {
                    background-color: var(--control-bg-read);
                }
                &:has(input:checked) {
                    &::before {
                        background-color: var(--control-text);
                        opacity: 0.5;
                    }
                }
                &:not(:has(input:checked)) {
                    + .text {
                        opacity: 0.5;
                    }
                }
            }
        }
        .text {
            display: block;
            padding-top: 2px;
            line-height: 1.2;
            text-align: left;
        }
    }
</style>
