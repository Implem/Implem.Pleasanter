<script lang="ts">
    /* module */
    import type { ColumnData, ParamHash } from './types';
    import { getIcon, columnCollection, columnParamHash } from './store';

    /* params */
    export let columnName: string;
    export let addState: -1 | 0 | 1 = -1;

    let item: ColumnData | undefined;
    let hash: ParamHash | undefined;

    $: {
        item = $columnCollection.find(data => data.ColumnName === columnName);
        hash = $columnParamHash[columnName];
    }
</script>

{#if item && hash && ((hash.Category === 'Preset' && (hash.State.Edit === 1 || hash.State.Grid === 1)) || (hash.Category === 'Custom' && (hash.State.Edit === 1 || hash.State.Grid === 1)))}
    <div
        data-column-name={columnName}
        class="unit"
        class:is-added={addState === 1}
        class:is-hidden={hash.State.Filter === -1}
    >
        <div class="unit-inner">
            <div class="unit-header">
                <div class="icon">
                    <span class="material-symbols-sharp is-fill">{getIcon(hash.Type, columnName)}</span>
                </div>
                <p class="label">{item.LabelText}</p>
                <div class="handler"><span class="material-symbols-sharp">drag_indicator</span></div>
            </div>
        </div>
    </div>
{:else}
    <div data-column-name={columnName} class="unit" class:is-hidden={true}></div>
{/if}

<style lang="scss">
    @use 'css/shared';
    @use 'css/unit';

    .unit {
        /* -------------------------------------------
        .filter-resource
        ------------------------------------------- */
        // :global(.column-resource) & {
        // }

        /* -------------------------------------------
        .filter-sortable
        ------------------------------------------- */
        :global(.column-sortable) &,
        &.drag-ghost {
            box-shadow: 0 0 8px var(--sd-base-shadow);
            .unit-inner {
                display: flex;
                width: calc(200px - 16px);
                height: 100%;
                background: var(--sd-unit-sortable-bg);
                border-radius: 0;
                .unit-header {
                    flex: 1;
                }
                .label {
                    font-size: 14px;
                }
                .handler {
                    display: flex;
                    align-items: center;
                    span {
                        font-size: 16px;
                    }
                }
            }
        }
    }
</style>
