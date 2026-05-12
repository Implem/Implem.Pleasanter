<script lang="ts">
    import { onMount, onDestroy, tick } from 'svelte'; // tick を追加
    import Sortable from 'sortablejs';

    export let options: Sortable.Options & { stage?: HTMLElement };
    let sortableInstance: Sortable | undefined;
    let el: HTMLElement;

    export const setSortableDisabled = (disabled: boolean) => {
        if (sortableInstance) {
            sortableInstance.option('disabled', disabled);
        }
    };

    export const withSortablePaused = async (domOperation: () => void | Promise<void>) => {
        setSortableDisabled(true);
        try {
            await domOperation();
            await tick();
        } finally {
            setSortableDisabled(false);
        }
    };

    onMount(() => {
        sortableInstance = Sortable.create(el, options);
        options.stage = el;
    });

    onDestroy(() => {
        if (sortableInstance) {
            sortableInstance.destroy();
        }
    });
</script>

<div class="draggable-container" bind:this={el}>
    <slot />
</div>
