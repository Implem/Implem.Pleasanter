<script lang="ts">
    import { onMount } from 'svelte';
    import { fade, fly } from 'svelte/transition';
    import { quintOut } from 'svelte/easing';
    import { get } from 'svelte/store';
    import Sortable from 'sortablejs';
    import type { ColumnData } from './types';
    import { typeKeys, defaultColumns, columnCollection, filterColumns, columnParamHash, setAppEdited } from './store';
    import Draggable from './Utility/UiDraggable.svelte';
    import FilterUnit from './FilterUnit.svelte';
    import { pDisplay } from './Utility/$p';

    /* params */
    let isSorting = false;
    let isRemovable = false;
    let customList: string[] = [];
    let draggableContainer: { options: Sortable.Options & { stage?: HTMLElement } };
    let hiddenUnitCount: number;
    let emptyState: boolean;
    let sortableColumns = get(filterColumns);
    let setSortableDisabled: (disabled: boolean) => void;
    let shadowListElement: HTMLElement;
    let htmlString: string | undefined = undefined;

    $: {
        emptyState = $filterColumns.length === hiddenUnitCount;
        if ($filterColumns.length !== sortableColumns.length && !htmlString) {
            sortableColumns = $filterColumns;
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
            name: 'filter',
            pull: 'clone',
            put: false
        },
        sort: false,
        onEnd: event => onDraggableClone(event)
    } as Sortable.Options;

    const sortableOptions = {
        direction: 'horizontal',
        animation: 150,
        ghostClass: 'drag-ghost',
        group: 'filter',
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
        group: 'filter',
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

        if ($columnParamHash[cloneItem.ColumnName]) $columnParamHash[cloneItem.ColumnName].State.Filter = 1;

        //FilterColumnsに追加
        const _list = get(filterColumns);
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
        $filterColumns = _list;
        setAppEdited();

        setTimeout(() => {
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
                filterColumns.update(() => {
                    const _list: string[] = [];
                    Array.from(to.children).forEach(element => {
                        const columnName = (element as HTMLElement).dataset.columnName;
                        columnName && _list.push(columnName);
                    });
                    if ($filterColumns.toString() !== _list.toString()) setAppEdited();
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
                    if (itemEl.dataset.columnName) $columnParamHash[itemEl.dataset.columnName].State.Filter = 0;
                    filterColumns.update(list => {
                        const _list = [...list];
                        const _index = _list.findIndex(ColumnName => ColumnName === itemEl.dataset.columnName);
                        _list.splice(_index, 1);
                        if (list.toString() !== _list.toString()) setAppEdited();
                        return _list;
                    });
                    isRemovable = true;
                    itemEl.remove();
                }
            }
        }
        isSorting = false;
    };

    onMount(() => {
        if (draggableContainer.options.stage) {
            hiddenUnitCount = draggableContainer.options.stage.querySelectorAll('.unit.is-hidden').length;
        }
    });
</script>

<div class="column-resource is_filter">
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
                    <p class="icon"><span class="material-symbols-sharp is-fill">filter_alt_off</span></p>
                    <p class="text">{pDisplay('UnusedFilterItems')}</p>
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
                    {#each $defaultColumns.filter(name => $columnParamHash[name] && $columnParamHash[name].State.Filter !== -1) as name (name)}
                        <FilterUnit addState={$columnParamHash[name].State.Filter} columnName={name} />
                    {/each}
                </Draggable>
            </div>
        </details>
        {#if customList.length}
            <details class="accordion" open>
                <summary class="ac-ttl">
                    <span class="label">{pDisplay('AdditionalColumns')}</span>
                    <span class="material-symbols-sharp"></span>
                </summary>
                <div class="ac-body">
                    <Draggable options={resourceOptions}>
                        {#each customList.filter(name => $columnParamHash[name].State.Filter !== -1 && ($columnParamHash[name].State.Edit || $columnParamHash[name].State.Grid)) as name (name)}
                            <FilterUnit addState={$columnParamHash[name].State.Filter} columnName={name} />
                        {/each}
                    </Draggable>
                </div>
            </details>
        {/if}
    </div>
</div>
<div class="main-container is_filter">
    <div class="column-sortable">
        {#if emptyState}
            <div class="empty-display">
                <div class="empty-inner">
                    <span class="material-symbols-sharp icon"> place_item </span>
                    <p>{pDisplay('DragColumn')}</p>
                </div>
            </div>
        {/if}
        <div class="draggable-container" bind:this={shadowListElement}></div>
        <Draggable options={sortableOptions} bind:this={draggableContainer} bind:setSortableDisabled>
            {#each sortableColumns as columnName (columnName)}
                <FilterUnit addState={-1} {columnName} />
            {/each}
        </Draggable>
    </div>
</div>

<style lang="scss">
    @use 'css/shared';
    @use 'css/container';
    .column-sortable {
        :global(.draggable-container) {
            display: flex;
            flex-wrap: wrap;
            gap: 8px;
        }
    }
</style>
