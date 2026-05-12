<script lang="ts">
    import { tick } from 'svelte';
    import { get } from 'svelte/store';
    import Sortable from 'sortablejs';
    import {
        editorColumnHash,
        editTabs,
        editTabCurrentId,
        editTabDraggingId,
        editTabDraggingColumn,
        editTabDropData,
        setAppEdited
    } from './store';
    import type { EditTabsData } from './types';
    import Draggable from './Utility/UiDraggable.svelte';
    import TabsModal from './EditTabsModal.svelte';
    import { pDisplay } from './Utility/$p';

    export let onTabChange: (tab: string) => void;

    /* params */
    const SORT_ANIMATION_MS = 150;
    let tabSortingId: string | undefined = undefined;
    let tabDeleteHover: boolean = false;
    let tabItem: EditTabsData | null;
    let isDeleteModal: boolean = false;
    let draggableRef: { setSortableDisabled: (disabled: boolean) => void };

    let sortableTabs = get(editTabs).items;

    const tabOptions = {
        draggable: '.isSortable',
        animation: SORT_ANIMATION_MS,
        onStart: e => {
            tabSortingId = e.item.dataset.tabId;
        },
        onUnchoose: () => {
            tabSortingId = undefined;
        },
        onEnd: e => {
            const { oldIndex, newIndex, to } = e;
            if (oldIndex === newIndex || oldIndex === undefined || newIndex === undefined) return;
            // animation中はドラッグ操作を無効化する
            draggableRef?.setSortableDisabled(true);
            setTimeout(() => {
                const current = get(editTabs);
                const items: EditTabsData[] = [];
                Array.from(to.children).forEach(element => {
                    const tabId = (element as HTMLElement).dataset.tabId;
                    const data = current.items.find(obj => obj.Id === Number(tabId));
                    data && items.push(data);
                });
                editTabs.set({ ...current, items });

                setAppEdited();
                draggableRef?.setSortableDisabled(false);
            }, SORT_ANIMATION_MS + 10);
        }
    } as Sortable.Options;

    const dataSync = (reload?: boolean) => {
        sortableTabs = [];
        tick().then(() => {
            sortableTabs = get(editTabs).items;
            if (reload) onTabChange(get(editTabCurrentId));
        });
    };

    // Tab:Click
    const onTabClick = (e: MouseEvent) => {
        const composedPath = e.composedPath();
        const tabElem = (composedPath[0] as HTMLElement).closest('.tab-unit') as HTMLElement;
        if ((composedPath[0] as HTMLElement).closest('.setting')) {
            onModalOpen(Number(tabElem.dataset.tabId));
        } else {
            if (get(editTabCurrentId) === tabElem.id) return;
            $editTabCurrentId = tabElem.id;
            onTabChange(tabElem.id);
        }
    };

    const onTabDoubleClick = (e: MouseEvent) => {
        const composedPath = e.composedPath();
        const tabElem = (composedPath[0] as HTMLElement).closest('.tab-unit') as HTMLElement;
        onModalOpen(Number(tabElem.dataset.tabId));
    };

    // Tab:HoverEnter
    const onTabDragEnter = (e: DragEvent) => {
        if (!get(editTabDraggingColumn)) return;
        $editTabDraggingId = (e.composedPath()[0] as HTMLElement).id;
    };

    // Tab:HoverOut
    const onTabDragLeave = (e: DragEvent) => {
        if (!get(editTabDraggingId)) return;
        $editTabDraggingId = undefined;
    };

    // Tab:Drop
    const onTabDrop = (e: DragEvent) => {
        if (get(editTabDraggingColumn)) {
            e.preventDefault();
            if (get(editTabDraggingId)) {
                $editTabDropData = {
                    tabId: get(editTabDraggingId)!,
                    columnId: get(editTabDraggingColumn)!
                };
            }
        }
        $editTabDraggingColumn = undefined;
        $editTabDraggingId = undefined;
    };

    // Tab:Add
    const onTabAdd = () => {
        const newID = get(editTabs).LatestId + 1;
        onModalOpen(newID);
    };

    // Tab:Remove
    const onTabDelete = () => {
        deleteTab(Number(tabSortingId));
    };

    // Tab:HoverEnter
    const onTabDeleteEnter = () => {
        if (!tabSortingId) return;
        tabDeleteHover = true;
    };

    // Tab:HoverLeave
    const onTabDeleteLeave = () => {
        if (!tabSortingId) return;
        tabDeleteHover = false;
    };

    const onModalOpen = (id: number) => {
        isDeleteModal = false;
        const item = get(editTabs).items.find(item => item.Id === id);
        if (item) {
            tabItem = get(editTabs).items.find(item => item.Id === id) || null;
        } else {
            tabItem = {
                LabelText: undefined,
                Id: id
            };
        }
    };

    const deleteTab = (id: number) => {
        isDeleteModal = true;
        tabItem = get(editTabs).items.find(item => item.Id === id) || null;
    };

    const getTabId = (id: number) => {
        if (id === 0) {
            return 'General';
        } else {
            return `_Tab-${id}`;
        }
    };
</script>

<div class="tab-sortable" class:isDragging={$editTabDraggingColumn} class:isSorting={!!tabSortingId}>
    <div class="tab-sortable-outer">
        <div class="tab-sortable-inner">
            <Draggable options={tabOptions} bind:this={draggableRef}>
                {#each sortableTabs as data (data.Id)}
                    <button
                        class="tab-unit"
                        id={getTabId(data.Id)}
                        type="button"
                        tabindex="0"
                        data-tab-id={data.Id}
                        class:isSortable={data.Id !== 0}
                        class:isActive={$editTabCurrentId === getTabId(data.Id)}
                        class:isHover={$editTabDraggingColumn && $editTabDraggingId === getTabId(data.Id)}
                        on:click={onTabClick}
                        on:dblclick={onTabDoubleClick}
                        on:dragenter={onTabDragEnter}
                        on:dragleave={onTabDragLeave}
                        on:drop|capture={onTabDrop}
                    >
                        <div class="tab-inner">
                            {#if getTabId(data.Id) !== 'General'}
                                <div class="handler"><span class="material-symbols-sharp">drag_indicator</span></div>
                            {/if}
                            <span class="label">{data.LabelText}</span>
                            <span class="count"
                                >{$editorColumnHash[getTabId(data.Id)].filter(name => name !== 'Comments').length || 0}
                            </span>
                            <div class="setting"><span class="material-symbols-sharp is-fill">settings</span></div>
                        </div>
                    </button>
                {/each}
            </Draggable>
            <div class="tab-stepper">
                {#if !tabSortingId}
                    <button
                        class="btn is-add"
                        type="button"
                        title={pDisplay('NewTab')}
                        on:click={onTabAdd}
                        tabindex="0"
                    >
                        <div class="icon"><span class="material-symbols-sharp is-fill">add</span></div>
                    </button>
                {:else}
                    <button
                        class="btn is-remove"
                        type="button"
                        class:isHover={tabDeleteHover}
                        on:dragenter={onTabDeleteEnter}
                        on:dragleave={onTabDeleteLeave}
                        on:drop={onTabDelete}
                        tabindex="0"
                    >
                        <div class="icon"><span class="material-symbols-sharp is-fill">remove</span></div>
                    </button>
                {/if}
            </div>
        </div>
    </div>
</div>

<TabsModal bind:tabItem bind:isDeleteModal {dataSync} />

<style lang="scss">
    @use 'css/shared';
    @use 'css/container';

    .tab-sortable {
        position: relative;
        z-index: 10;
        background-color: var(--sd-base-bg);
        &::after {
            position: absolute;
            top: 100%;
            left: 0;
            width: 100%;
            height: 16px;
            pointer-events: none;
            content: ' ';
            background: linear-gradient(
                to bottom,
                var(--sd-body-inner) 0%,
                var(--sd-body-inner) 40%,
                rgb(255 0 0 / 0%) 100%
            );
        }
        .tab-sortable-outer {
            overflow-x: auto;
            &::-webkit-scrollbar {
                width: 0;
                height: 4px;
            }
            &::-webkit-scrollbar-track {
                background: var(--sd-body-inner);
            }
            &::-webkit-scrollbar-thumb {
                cursor: grab;
                background-color: var(--sd-sortable-scroll);
                border-radius: 8px;
                &:hover {
                    background-color: var(--sd-sortable-scroll-hover);
                }
                &:active {
                    cursor: grabbing;
                }
            }
        }
        .tab-sortable-inner {
            display: flex;
            gap: 8px;
            min-width: max-content;
            padding: 8px 16px 0;
            border-bottom: 1px solid var(--sd-base-border);
        }

        :global(.draggable-container) {
            display: flex;
            gap: 8px;
        }
        .tab-unit {
            position: relative;
            min-width: 120px;
            max-width: 300px;
            padding: 16px 32px;
            margin-bottom: -1px;
            font-size: 14px;
            color: var(--sd-base-text);
            text-align: center;
            cursor: pointer;
            background-color: var(--sd-edit-tab-bg);
            border: 1px solid var(--sd-base-border);
            border-radius: 8px 8px 0 0;
            .tab-inner {
                display: flex;
                gap: 8px;
            }
            .label {
                overflow: hidden;
                text-overflow: ellipsis;
                white-space: nowrap;
            }
            .count {
                display: block;
                align-self: center;
                min-width: 24px;
                padding: 2px 4px;
                font-size: 11px;
                line-height: 1.2;
                color: var(--sd-unit-edit-count-text);
                background-color: var(--sd-unit-edit-count-bg);
                border-radius: 4px;
            }
            .handler {
                position: absolute;
                top: 50%;
                left: 8px;
                transform: translateY(-50%);
                .material-symbols-sharp {
                    display: inline-block;
                    font-size: 16px;
                }
            }
            .setting {
                position: absolute;
                top: 0;
                right: 0;
                display: none;
                align-items: center;
                justify-content: center;
                width: 24px;
                height: 24px;
                color: var(--sd-unit-icon-setting);
                cursor: pointer;
                .material-symbols-sharp {
                    font-size: 14px;
                }
            }
            &:hover {
                .setting {
                    display: flex;
                }
            }
            &.isActive {
                background-color: var(--sd-body-inner);
                border-bottom-color: transparent;
                .count {
                    background-color: var(--primaryColor);
                }
            }
            &.isHover {
                color: #fff;
                background-color: var(--primaryColor);
                border: 1px solid var(--primaryColor) !important;
            }
            &:global(.sortable-ghost) {
                border: 1px dashed #888;
                border-bottom: 1px solid var(--sd-base-border);
                box-shadow: 0 0 10px var(--sd-base-shadow);
                opacity: var(--sd-unit-ghost-opacity);
            }
            &:active {
                &:has(.handler) {
                    cursor: grabbing;
                }
            }
        }
        .tab-stepper {
            align-self: center;
            .btn {
                width: 40px;
                height: 40px;
                padding: 0;
                cursor: pointer;
                outline: none;
                border-radius: 4px;
                transition:
                    background 200ms ease-out,
                    color 200ms ease-out;
                &.is-add {
                    color: var(--sd-edit-tab-stepper-icon);
                    background-color: var(--primaryColor);
                    border: 1px solid var(--primaryColor);
                    &:hover {
                        background-color: var(--primaryDark);
                    }
                }
                &.is-remove {
                    color: var(--btn-negative-bg);
                    background-color: var(--sd-edit-tab-stepper-remove);
                    border: 1px solid var(--btn-negative-bg);
                    .icon {
                        pointer-events: none;
                    }
                    &.isHover {
                        color: var(--sd-edit-tab-stepper-icon);
                        background-color: var(--btn-negative-bg);
                    }
                }
            }
        }
        &.isDragging {
            .tab-unit {
                .handler {
                    opacity: 0;
                }
                &:not(.isActive) {
                    align-self: start;
                    padding: 12px 32px;
                    margin-top: 4px;
                    background-color: var(--sd-body-inner);
                    border: 1px dashed var(--primaryColor);
                    border-radius: 4px;
                    .count {
                        background-color: var(--primaryColor);
                    }
                    &.isHover {
                        background-color: var(--primaryColor);
                    }
                }
                .tab-inner {
                    pointer-events: none;
                }
            }
            .tab-stepper {
                display: none;
            }
        }
    }
</style>
