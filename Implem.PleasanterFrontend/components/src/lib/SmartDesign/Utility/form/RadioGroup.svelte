<script lang="ts">
    import { onMount } from 'svelte';
    import Radio from './Radio.svelte';
    import { pDisplay } from '../$p';

    export let model: string | number | boolean | null | undefined = undefined;
    export let required: boolean | undefined = undefined;
    export let init: string | number | boolean | null | undefined = undefined;
    export let disabled: boolean | undefined = false;
    export let options: [string | number | boolean | undefined, string][] | string[][] | undefined | string = undefined;

    let slotContent: HTMLElement;

    $: if (options) {
        if (isString(options)) {
            const format: string[][] = [];
            options.split(/\t|\n/).forEach(data => {
                const option = data.split(',');
                format.push(option);
            });
            options = format;
        }
    }

    const isString = (value: unknown): value is string => {
        return typeof value === 'string' || value instanceof String;
    };

    onMount(() => {
        if ($$slots.default) {
            [...slotContent.children].forEach(child => {
                const li = document.createElement('li');
                li.appendChild(child);
                slotContent.appendChild(li);
            });
        }
    });
</script>

<div class="radio-group">
    {#if options}
        <ul>
            {#each options as [value, label]}
                {#if value || label}<Radio bind:model {value} {disabled} {init}>{label || value}</Radio>{/if}
            {/each}
        </ul>
    {:else}
        <ul bind:this={slotContent}>
            <slot />
        </ul>
    {/if}
    {#if required && !model}<p class="error">{pDisplay('ValidateRequired')}</p>{/if}
</div>

<style lang="scss">
    @use '../shared';

    .radio-group {
        ul {
            display: flex;
            flex-wrap: wrap;
            gap: 8px 16px;
            padding: 0;
            margin: 0;
            list-style-type: none;
        }
        .error {
            padding: 4px 0 0;
            color: var(--control-error);
        }
    }
</style>
