<script lang="ts">
    /* module */
    import type { ColumnData, ParamHash } from './types';
    import { getIcon, columnCollection, columnParamHash } from './store';
    import { pDisplay } from './Utility/$p';

    /* params */
    export let columnName: string;
    export let addState: -1 | 0 | 1 = -1;
    export let onModalOpen: ((ColumnName: string) => void) | undefined = undefined;

    let item: ColumnData | undefined;
    let hash: ParamHash | undefined;

    $: {
        item = $columnCollection.find(data => data.ColumnName === columnName);
        hash = $columnParamHash[columnName];
    }
</script>

{#if item && hash}
    <div
        class="unit"
        data-column-name={columnName}
        class:is-added={addState === 1}
        class:is-hidden={hash.State.Grid === -1}
        role="button"
        tabindex="0"
        on:dblclick={() => item && onModalOpen && onModalOpen(item.ColumnName)}
    >
        <div class="unit-inner">
            <div class="unit-header">
                <div class="icon">
                    <span class="material-symbols-sharp is-fill">{getIcon(hash.Type, columnName)}</span>
                </div>
                <p class="label">{item.LabelText}</p>
                <div class="handler"><span class="material-symbols-sharp">drag_indicator</span></div>
            </div>
            <div class="unit-body">
                <button class="btn-setting" on:click={() => item && onModalOpen && onModalOpen(item.ColumnName)}>
                    {#if hash.Type !== 'LinkTable'}
                        <span class="material-symbols-sharp is-fill">settings</span>
                    {:else}
                        <span class="material-symbols-sharp is-fill is-delete">delete</span>
                    {/if}
                </button>
                <div>
                    <dl>
                        <dt>{pDisplay('SetCellWidth')}</dt>
                        <dd>
                            {#if item.CellWidth}
                                <span class="state is-active">{item.CellWidth}px</span>
                            {:else}
                                <span class="state">{pDisplay('NotSetForLabel')}</span>
                            {/if}
                        </dd>
                        <dt>{pDisplay('StickyOnLeftEdge')}</dt>
                        <dd>
                            {#if item.CellSticky}
                                <span class="state is-active">{pDisplay('Enabled')}</span>
                            {:else}
                                <span class="state">{pDisplay('Disabled')}</span>
                            {/if}
                        </dd>
                        <dt>{pDisplay('WrapTextInCell')}</dt>
                        <dd>
                            {#if item.CellWordWrap}
                                <span class="state is-active">{pDisplay('Enabled')}</span>
                            {:else}
                                <span class="state">{pDisplay('Disabled')}</span>
                            {/if}
                        </dd>
                    </dl>
                </div>
            </div>
            <div class="unit-footer">
                {#if hash.Category === 'Preset'}
                    <p class="tag">{pDisplay('DefaultColumns')}</p>
                {:else if hash.Category === 'Custom'}
                    <p class="tag">{pDisplay('AdditionalColumns')}</p>
                {:else if hash.Category === 'Links'}
                    <p class="tag">{pDisplay('LinkTable')}</p>
                {/if}
                {#if getIcon(hash.Type, columnName) === 'link'}
                    <p class="tag">{pDisplay('LinkDestinations')} : {item.ColumnName.split('_Links-')[1]}</p>
                {:else if getIcon(hash.Type, columnName) === 'add_link'}
                    <p class="tag">{pDisplay('LinkSources')} : {item.ColumnName.split('_Links-')[1]}</p>
                {:else}
                    <p class="tag">{pDisplay(item.ColumnName)}</p>
                {/if}
            </div>
        </div>
    </div>
{:else if !item && hash}
    <div data-column-name={columnName} class="unit" class:is-hidden={true}></div>
{/if}

<style lang="scss">
    @use 'css/shared';
    @use 'css/unit';

    .unit {
        /* -------------------------------------------
        .grid-resource
        ------------------------------------------- */
        :global(.column-resource) & {
            .unit-body,
            .unit-footer {
                display: none;
            }
            .btn-setting {
                display: none;
            }
        }

        /* -------------------------------------------
        .grid-sortable
        ------------------------------------------- */
        :global(.column-sortable) &,
        &.drag-ghost {
            position: relative;
            display: flex;
            margin-left: -1px;
            background: var(--sd-unit-grid-cell-bg);
            .unit-inner {
                display: flex;
                flex: 1;
                flex-direction: column;
                width: 100%;
                height: 50vh;
                min-height: 350px;
                padding: 0;
                border: 0;
                border: 1px solid var(--sd-unit-grid-cell-vborder);
                border-radius: 0;
            }
            .unit-header {
                gap: 8px;
                padding: 8px;
                margin: 4px;
                color: var(--sd-unit-grid-header-text);
                background: var(--sd-unit-grid-header-bg);
                border-radius: 4px;
                transition: background-color 200ms ease-out;
                .icon {
                    color: var(--sd-unit-grid-header-bg);
                    background: var(--sd-unit-grid-header-text);
                }
                .label {
                    max-width: 30vw;
                    padding-right: 8px;
                    overflow: hidden;
                    text-overflow: ellipsis;
                    white-space: nowrap;
                }
                .handler {
                    display: flex;
                    align-items: center;
                    span {
                        font-size: 16px;
                        color: var(--sd-unit-grid-header-text);
                    }
                }
                &:hover {
                    background: var(--sd-unit-grid-header-hover);
                }
            }
            .unit-body {
                position: relative;
                bottom: 0;
                display: block;
                flex: 1;
                width: 100%;
                padding: 16px;
                cursor: default;
                .btn-setting {
                    position: absolute;
                    top: 2px;
                    right: 6px;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    width: 24px;
                    height: 24px;
                    color: var(--sd-unit-icon-setting);
                    cursor: pointer;
                    outline: none;
                    background: transparent;
                    border: 0;
                    border-radius: 4px;
                    span {
                        font-size: 16px;
                    }
                }
                dl {
                    display: flex;
                    flex-direction: column;
                    dt {
                        font-size: 12px;
                        border-bottom: var(--sd-unit-grid-cell-border) 1px dashed;
                    }
                    dd {
                        margin: 8px 0 16px;
                    }
                    .state {
                        display: inline-block;
                        padding: 2px 8px;
                        font-size: 12px;
                        color: var(--sd-unit-grid-tag-text);
                        background: var(--sd-unit-grid-tag-bg);
                        border-radius: 4px;
                        &.is-active {
                            padding: 2px 8px;
                            font-weight: bold;
                            color: var(--sd-unit-grid-tag-enabled-text);
                            background: var(--sd-unit-grid-tag-enabled-bg);
                        }
                    }
                }
            }
            .unit-footer {
                display: flex;
                flex-wrap: wrap;
                gap: 4px;
                padding: 8px;
                cursor: default;
                .tag {
                    padding: 0 4px;
                    font-size: 12px;
                    color: var(--sd-unit-edit-tag-text);
                    background: var(--sd-unit-edit-tag-bg);
                    border: var(--sd-unit-edit-tag-border) 1px solid;
                    border-radius: 2px;
                }
            }
            // ghost
            &:is(:global(.drag-ghost)) {
                z-index: 1;
                box-shadow: 0 0 10px var(--sd-base-shadow);
                .unit-inner {
                    opacity: var(--sd-unit-filter-opacity);
                }
            }
        }

        :global(.is-grid-scrolling) & {
            .unit-body,
            .unit-footer {
                cursor: grabbing;
            }
        }
    }
</style>
