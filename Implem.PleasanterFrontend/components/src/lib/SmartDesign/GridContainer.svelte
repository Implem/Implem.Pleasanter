<script lang="ts">
    import { onMount } from 'svelte';
    import { fade, fly } from 'svelte/transition';
    import { quintOut } from 'svelte/easing';
    import { get } from 'svelte/store';
    import Sortable from 'sortablejs';
    import type { ColumnData } from './types';
    import { typeKeys, defaultColumns, columnCollection, gridColumns, columnParamHash, setAppEdited } from './store';
    import Draggable from './Utility/UiDraggable.svelte';
    import GridUnit from './GridUnit.svelte';
    import GridModal from './GridModal.svelte';
    import { pDisplay } from './Utility/$p';

    /* params */
    const sdElem = document.querySelector('smart-design');
    let unitItem: ColumnData | null;
    let isSorting = false;
    let isRemovable = false;
    let customList: string[] = [];
    let draggableContainer: { options: Sortable.Options & { stage?: HTMLElement } };
    let hiddenUnitCount: number;
    let emptyState: boolean;
    let sortableColumns = get(gridColumns);
    let setSortableDisabled: (disabled: boolean) => void;
    let shadowListElement: HTMLElement;
    let htmlString: string | undefined = undefined;
    let gridWrap: HTMLElement;
    let isMouseHeld = false;
    let mousePosition: number[] = [];
    let isEntered = false;
    let isScrollable = false;
    let resizeObserver: ResizeObserver;

    $: {
        emptyState = $gridColumns.length === hiddenUnitCount;
        if ($gridColumns.length !== sortableColumns.length && !htmlString) {
            sortableColumns = $gridColumns;
        }
        customList = Object.keys($columnParamHash)
            .filter(key => {
                const item = $columnParamHash[key];
                return item.Category === 'Custom' && item.State.Edit === 1;
            })
            .sort((a, b) => {
                const aType = $columnParamHash[a].Type;
                const bType = $columnParamHash[b].Type;
                const typeOrder = typeKeys.indexOf(aType) - typeKeys.indexOf(bType);
                if (typeOrder !== 0) return typeOrder;
                const aNum = a.match(/\d+$/);
                const bNum = b.match(/\d+$/);
                if (aNum && bNum) {
                    return parseInt(aNum[0]) - parseInt(bNum[0]);
                }
                if (aNum) return 1;
                if (bNum) return -1;
                return a.localeCompare(b);
            });
    }

    const resourceOptions = {
        filter: '.is-added',
        animation: 150,
        group: {
            name: 'grid',
            pull: 'clone',
            put: false
        },
        sort: false,
        onEnd: event => onDraggableClone(event)
    } as Sortable.Options;

    const sortableOptions = {
        handle: '.unit-header',
        direction: 'horizontal',
        animation: 150,
        ghostClass: 'drag-ghost',
        group: 'grid',
        onStart: () => {
            isSorting = true;
        },
        onEnd: event => onDraggableSort(event),
        onAdd: event => {
            if (draggableContainer.options.stage) {
                htmlString = draggableContainer.options.stage.innerHTML;
            }
            event.clone.replaceWith(event.item);
        }
    } as Sortable.Options;

    const removeOptions = {
        ghostClass: 'drag-ghost',
        group: 'grid',
        sort: false
    } as Sortable.Options;

    /* method */
    const onDraggableClone = (event: Sortable.SortableEvent) => {
        const { newIndex, from, to } = event;
        if (from === to || newIndex === undefined) return false;

        //機能停止
        setSortableDisabled(true);

        let cloneItem: ColumnData;
        const itemEl = event.item;
        const index = get(columnCollection).findIndex(item => item.ColumnName === itemEl.dataset.columnName);
        cloneItem = get(columnCollection)[index];

        if ($columnParamHash[cloneItem.ColumnName]) $columnParamHash[cloneItem.ColumnName].State.Grid = 1;

        //GridColumnsに追加
        const _list = get(gridColumns);
        _list.splice(newIndex, 0, cloneItem!.ColumnName);
        sortableColumns = [];

        //不要なunitが残ってたら削除
        const dustElems = draggableContainer.options.stage?.querySelectorAll('.unit');
        dustElems &&
            dustElems.forEach(element => {
                element.remove();
            });

        if (shadowListElement && htmlString) {
            shadowListElement.innerHTML = htmlString;
        }
        $gridColumns = _list;
        setAppEdited();

        setTimeout(() => {
            setDragScrllable();
            sortableColumns = _list;
            shadowListElement.innerHTML = '';
            htmlString = undefined;
            //機能再開
            setSortableDisabled(false);
        });
    };

    const onDraggableSort = (event: Sortable.SortableEvent) => {
        const { newIndex, oldIndex, from, to } = event;
        const itemEl = event.item;
        if (from === to) {
            if (newIndex !== undefined && oldIndex !== undefined) {
                gridColumns.update(() => {
                    const _list: string[] = [];
                    Array.from(to.children).forEach(element => {
                        const columnName = (element as HTMLElement).dataset.columnName;
                        columnName && _list.push(columnName);
                    });
                    if ($gridColumns.toString() !== _list.toString()) setAppEdited();
                    return _list;
                });
            }
        } else {
            if (oldIndex !== undefined && to) {
                if (!isRemovable) {
                    if (oldIndex < event.from.children.length) {
                        event.from.children[oldIndex].before(event.item);
                    } else {
                        event.from.children[oldIndex - 1].after(event.item);
                    }
                } else {
                    if (itemEl.dataset.columnName) $columnParamHash[itemEl.dataset.columnName].State.Grid = 0;
                    gridColumns.update(list => {
                        const _list = [...list];
                        const _index = _list.findIndex(columnName => columnName === itemEl.dataset.columnName);
                        _list.splice(_index, 1);
                        if (list.toString() !== _list.toString()) setAppEdited();
                        return _list;
                    });
                    isRemovable = true;
                    setDragScrllable();
                    itemEl.remove();
                }
            }
        }
        isSorting = false;
    };

    const setDragScrllable = () => {
        isScrollable = gridWrap.scrollWidth > gridWrap.clientWidth;
    };

    const onModalOpen = (columnName: string) => {
        const type = $columnParamHash[columnName].Type;
        let editData: ColumnData | undefined;
        switch (type) {
            case 'LinkTable':
                editData = {
                    ColumnName: columnName,
                    LabelText: $columnParamHash[columnName].LinkName
                };
                break;
            default:
                editData = get(columnCollection).find(data => {
                    return data.ColumnName === columnName;
                });
                break;
        }
        editData && (unitItem = editData);
    };

    const onScrollStart = (e: Event) => {
        if (!isScrollable) return;
        const event = e as MouseEvent;
        if (!(event.composedPath()[0] as HTMLElement).closest('.unit-header')) {
            if (isEntered) {
                event.preventDefault();
                gridWrap.classList.add('is-grid-scrolling');
                isMouseHeld = true;
                mousePosition = [event.movementX, event.movementY];
            }
        }
    };

    const onScrollEnd = () => {
        if (!isScrollable) return;
        isMouseHeld = false;
        mousePosition = [];
        gridWrap.classList.remove('is-grid-scrolling');
    };

    const onScrollAction = (e: Event) => {
        if (!isScrollable) return;
        const event = e as MouseEvent;
        if (
            mousePosition.length === 2 &&
            event.movementX === mousePosition[0] &&
            event.movementY === mousePosition[1]
        ) {
            return;
        }
        if (isMouseHeld) {
            if (isEntered) {
                e.preventDefault();
                const dx = event.movementX;
                const dy = event.movementY;
                gridWrap!.scrollLeft -= dx;
                gridWrap!.scrollTop -= dy;
            } else {
                const selection = window.getSelection();
                if (selection) {
                    selection.removeAllRanges();
                }
            }
        }
    };

    onMount(() => {
        if (draggableContainer.options.stage) {
            hiddenUnitCount = draggableContainer.options.stage.querySelectorAll('.unit.is-hidden').length;

            resizeObserver = new ResizeObserver(entries => {
                for (let entry of entries) {
                    if (entry.target === gridWrap) {
                        setDragScrllable();
                    }
                }
            });
            resizeObserver.observe(gridWrap);

            sdElem?.addEventListener('mousedown', onScrollStart);
            sdElem?.addEventListener('mouseup', onScrollEnd);
            sdElem?.addEventListener('mousemove', onScrollAction);
        }
        return () => {
            sdElem?.removeEventListener('mousedown', onScrollStart);
            sdElem?.removeEventListener('mouseup', onScrollEnd);
            sdElem?.removeEventListener('mousemove', onScrollAction);
            resizeObserver.disconnect();
        };
    });
</script>

<div class="column-resource is_grid">
    {#if isSorting}
        <div
            class="remove-area"
            class:hover={isRemovable}
            in:fly={{ duration: 600, easing: quintOut, y: -50 }}
            out:fade={{ duration: 150 }}
        >
            <Draggable options={removeOptions}>
                <div
                    role="button"
                    tabindex="0"
                    class="remove-box"
                    on:dragenter={() => (isRemovable = true)}
                    on:dragleave={() => (isRemovable = false)}
                >
                    <p class="icon"><span class="material-symbols-sharp is-fill">variable_remove</span></p>
                    <p class="text">{pDisplay('UnusedGridItems')}</p>
                </div>
            </Draggable>
        </div>
        <div class="remove-bg" transition:fade={{ duration: 150 }}></div>
    {/if}
    <div class="resource-inner">
        <details class="accordion">
            <summary class="ac-ttl">
                <span class="label">{pDisplay('DefaultColumns')}</span>
                <span class="material-symbols-sharp"></span>
            </summary>
            <div class="ac-body">
                <Draggable options={resourceOptions}>
                    {#each $defaultColumns.filter(name => $columnParamHash[name] && $columnParamHash[name].State.Grid !== -1) as name (name)}
                        <GridUnit addState={$columnParamHash[name].State.Grid} columnName={name} />
                    {/each}
                </Draggable>
            </div>
        </details>
        <details class="accordion" open>
            <summary class="ac-ttl">
                <span class="label">{pDisplay('AdditionalColumns')}</span>
                <span class="material-symbols-sharp"></span>
            </summary>
            <div class="ac-body">
                <Draggable options={resourceOptions}>
                    {#each customList as name (name)}
                        <GridUnit addState={$columnParamHash[name].State.Grid} columnName={name} />
                    {/each}
                </Draggable>
            </div>
        </details>
    </div>
</div>
<div class="main-container is_grid">
    <div class="column-sortable">
        {#if emptyState}
            <div class="empty-display">
                <div class="empty-inner">
                    <span class="material-symbols-sharp icon"> place_item </span>
                    <p>{pDisplay('DragColumn')}</p>
                </div>
            </div>
        {/if}
        <div
            class="grid-wrap"
            role="button"
            tabindex="0"
            bind:this={gridWrap}
            on:mouseenter={() => (isEntered = true)}
            on:mouseleave={() => (isEntered = false)}
        >
            <div class="draggable-container" bind:this={shadowListElement}></div>
            <Draggable options={sortableOptions} bind:this={draggableContainer} bind:setSortableDisabled>
                {#each sortableColumns as columnName (columnName)}
                    <GridUnit addState={-1} {columnName} {onModalOpen} />
                {/each}
            </Draggable>
        </div>
    </div>
</div>

<GridModal bind:unitItem />

<style lang="scss">
    @use 'css/shared';
    @use 'css/container';
    .column-resource {
        .accordion:has(:global(.draggable-container:empty)) {
            display: none;
        }
    }
    .main-container {
        .column-sortable {
            :global(.draggable-container) {
                position: static;
                display: flex;
                width: fit-content;
                min-width: 100%;
                min-height: 176px;
                padding: 0;
                overflow: hidden;
                background-image: repeating-linear-gradient(
                    135deg,
                    rgb(255 255 255 / 0%) 2% 4%,
                    rgb(255 255 255 / 50%) 4% 6%
                );
                border-radius: 8px;
            }
        }
        .grid-wrap {
            position: relative;
            padding: 0 0 5px;
            margin: 24px 17px;
            overflow: auto hidden;
            white-space: nowrap;

            &::-webkit-scrollbar {
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
    }
</style>
