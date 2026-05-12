<script lang="ts">
    import ReadField from './ReadField.svelte';
    import { pDisplay } from '../$p';

    export let model: string | number | boolean | undefined;
    export let required: boolean | undefined = undefined;
    export let readOnly: boolean | undefined = undefined;
    export let fieldSize: string | undefined = undefined;
    export let align: number | undefined = undefined;
    export let user: boolean | undefined = undefined;
    export let init: string | number | boolean | undefined = undefined;
    export let options: [string | number | boolean | undefined, string][] | string[][] | string | undefined = undefined;
    export let onInit: ((model: string | number | boolean | undefined) => void) | undefined = undefined;
    export let onChange: ((event: Event) => void) | undefined = undefined;

    export let optionList: [string | number | boolean | undefined, string][] | string[][] | undefined = undefined;
    let myName: string | undefined;

    if (model === undefined) model = init;

    $: if (options) {
        if (isString(options)) {
            const list: string[][] = [];
            try {
                JSON.parse(options); // JSON.parseを試みる
                const json = JSON.parse(options)[0];
                if (Array.isArray(json)) {
                    list.push([]);
                    list[0].push(`[[${json[0]}]]`);
                    optionList = list;
                } else if (json.SearchFormat) {
                    list.push([]);
                    list[0].push(json.SearchFormat);
                    optionList = list;
                } else {
                    list.push([]);
                    list[0].push(`[[${pDisplay('OptionList')}]]`);
                    optionList = list;
                }
            } catch (e) {
                options.split(/\t|\n/).forEach(data => {
                    const option = data.split(',');
                    list.push(option);
                });
                optionList = list;
                setTimeout(() => {
                    model = undefined;
                });
            }
        } else {
            optionList = options;
        }
    }

    const isString = (value: unknown): value is string => {
        return typeof value === 'string' || value instanceof String;
    };

    if (user) {
        myName = document.querySelector('#AccountUserName .account-name')?.textContent || undefined;
    }

    if (onInit) {
        setTimeout(() => {
            onInit(model);
        }, 10);
    }
</script>

{#if !readOnly}
    <label class="selectbox" class:is-user={user}>
        <div class="select-inner">
            {#if optionList}
                <select bind:value={model} on:change={onChange}>
                    {#if myName}
                        <option value={'[[Self]]'}>{myName}</option>
                    {/if}
                    {#each optionList as [value, label]}
                        {#if value !== undefined}<option {value}>{label || value}</option>{/if}
                    {/each}
                </select>
            {:else}
                <select bind:value={model}>
                    <slot />
                </select>
            {/if}
            {#if user}
                <button type="button" class="btn-user" on:click={() => (model = '[[Self]]')}>
                    <span class="material-symbols-sharp is-fill">person</span>
                </button>
            {/if}
        </div>
        {#if required && !model && !readOnly}<p class="error">{pDisplay('ValidateRequired')}</p>{/if}
    </label>
{:else}
    <ReadField {model} {align} {fieldSize} />
{/if}

<style lang="scss">
    @use '../shared';

    .selectbox {
        .select-inner {
            position: relative;
        }
        select {
            width: 100%;
            height: 32px;
            padding: 4px 8px;
            color: var(--control-text);
            outline: none;
            background-color: var(--control-bg);
            border: 1px solid var(--control-border);
            border-radius: 4px;
            &:focus {
                background-color: var(--control-bg-focus);
                border-color: var(--control-border-focus);
            }
        }
        .error {
            padding: 4px 0 0;
            color: var(--control-error);
        }
        &.is-user {
            select {
                padding-right: 50px;
                appearance: none;
            }
            .btn-user {
                position: absolute;
                top: 0;
                right: 0;
                width: 32px;
                height: 100%;
                padding: 0;
                margin: 0;
                cursor: pointer;
                outline: none;
                background-color: transparent;
                border: 0;
                span {
                    font-size: 18px;
                }
            }
        }
    }
</style>
