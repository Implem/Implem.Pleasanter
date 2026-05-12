<svelte:options
    customElement={{
        tag: 'smart-design'
    }}
/>

<script lang="ts">
    import { appState, tabState } from './store';
    import App from './App.svelte';
    import EditContainer from './EditContainer.svelte';
    import GridContainer from './GridContainer.svelte';
    import FilterContainer from './FilterContainer.svelte';
    import ConfirmModal from './ConfirmModal.svelte';
</script>

{#if $appState}
    <App>
        {#if $tabState === 'edit'}
            <EditContainer />
        {:else if $tabState === 'grid'}
            <GridContainer />
        {:else if $tabState === 'filter'}
            <FilterContainer />
        {/if}
    </App>
    <ConfirmModal />
{/if}
