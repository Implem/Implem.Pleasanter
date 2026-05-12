<script lang="ts">
    import { onMount } from 'svelte';

    let slotContent: HTMLElement;

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

<div class="checkbox-group">
    <ul bind:this={slotContent}>
        <slot />
    </ul>
</div>

<style lang="scss">
    @use '../shared';

    .checkbox-group {
        ul {
            display: flex;
            flex-wrap: wrap;
            gap: 8px 16px;
            padding: 8px 0 0;
            margin: 0;
            list-style-type: none;
        }
    }
</style>
