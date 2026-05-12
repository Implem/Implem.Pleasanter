<script lang="ts">
    export let icon: string | undefined = '';
    export let type: string | undefined = '';
    export let disabled: boolean | undefined = undefined;
    export let loader: boolean | undefined = undefined;
    export let onClick: ((event: Event) => void) | undefined = undefined;
</script>

<button
    type="button"
    class="c-btn"
    class:isNormal={type === 'normal'}
    class:isPositive={type === 'positive'}
    class:isNegative={type === 'negative'}
    {disabled}
    on:click={onClick}
>
    {#if loader}
        <span class="material-symbols-sharp is-fill is-rotate">progress_activity</span>
    {:else if icon}
        <span class="icon material-symbols-sharp is-fill">{icon}</span>
    {/if}
    <span class="label"><slot /></span>
</button>

<style lang="scss">
    @use './shared';

    .c-btn {
        display: flex;
        gap: 8px;
        align-items: center;
        padding: 8px 16px;
        font-weight: bold;
        color: var(--btn-neutral-label);
        cursor: pointer;
        background: var(--btn-neutral-bg);
        border: 1px solid var(--btn-neutral-border);
        border-radius: 4px;
        box-shadow: 0 1px 2px var(--base-shadow);
        transition:
            background 150ms ease-out,
            box-shadow 150ms ease-out;
        &:hover {
            background-color: var(--btn-neutral-hover);
            box-shadow: 0 2px 4px var(--base-shadow);
        }
        .material-symbols-sharp {
            font-size: 16px;
            + .label {
                padding-right: 8px;
            }
        }
        &:disabled {
            pointer-events: none;
            .icon,
            .label {
                opacity: 0.5;
            }
        }
        &.isNormal {
            color: var(--btn-normal-normal);
            background: var(--btn-normal-bg);
            border: 1px solid var(--btn-normal-border);
            &:hover {
                background-color: var(--btn-normal-hover);
            }
        }
        &.isPositive {
            color: var(--btn-positive-label);
            background: var(--btn-positive-bg);
            border: 1px solid var(--btn-positive-border);
            &:hover {
                background-color: var(--btn-positive-hover);
            }
        }
        &.isNegative {
            color: var(--btn-negative-label);
            background: var(--btn-negative-bg);
            border: 1px solid var(--btn-negative-border);
            &:hover {
                background-color: var(--btn-negative-hover);
            }
        }
        &:has(.is-rotate) {
            pointer-events: none;
            .label {
                opacity: 0.7;
            }
            .is-rotate {
                animation: l25 1s infinite linear;
            }
        }

        @keyframes l25 {
            100% {
                transform: rotate(1turn);
            }
        }
    }
</style>
