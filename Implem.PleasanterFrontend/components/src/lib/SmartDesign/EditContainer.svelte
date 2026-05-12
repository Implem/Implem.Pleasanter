<script lang="ts">
    import { get } from 'svelte/store';
    import Sortable from 'sortablejs';
    import { cloneDeep } from 'lodash';
    import type { ColumnData, SectionData } from './types';
    import {
        defaultColumns,
        editorColumnHash,
        columnCollection,
        linkTable,
        sections,
        columnParamHash,
        cloneRssItems,
        setAppEdited,
        supportUrl,
        editTabCurrentId,
        editTabDraggingId,
        editTabDraggingColumn,
        editTabDropData
    } from './store';
    import Draggable from './Utility/UiDraggable.svelte';
    import EditTabs from './EditTabs.svelte';
    import EditUnit from './EditUnit.svelte';
    import EditModal from './EditModal.svelte';
    import { pDisplay } from './Utility/$p';

    /* params */
    let unitItem: ColumnData | null;
    let draggableContainer: {
        options: Sortable.Options & { stage?: HTMLElement };
        withSortablePaused: (domOperation: () => void | Promise<void>) => Promise<void>;
    };
    let sortableColumns = get(editorColumnHash)[get(editTabCurrentId)];
    let setSortableDisabled: (disabled: boolean) => void;
    let shadowListElement: HTMLElement;
    let htmlString: string | undefined = undefined;

    $: {
        if (!htmlString) {
            sortableColumns = get(editorColumnHash)[get(editTabCurrentId)];
        }
    }

    const resourceOptions = {
        filter: '.is-added',
        group: {
            name: 'edit',
            pull: 'clone',
            put: false
        },
        sort: false,
        onStart: event => {
            onColumnDragStart(event.item);
        },
        onEnd: event => {
            onDraggableClone(event);
            onColumnDragEnd();
        }
    } as Sortable.Options;

    const sortableOptions = {
        swapThreshold: 0.5,
        invertSwap: true,
        direction: 'vertical',
        animation: 200,
        ghostClass: 'drag-ghost',
        group: 'edit',
        onStart: event => {
            onColumnDragStart(event.item);
        },
        onEnd: event => {
            onColumnDragEnd();
            onDraggableSort(event);
        },
        onAdd: event => {
            if (draggableContainer.options.stage) {
                htmlString = draggableContainer.options.stage.innerHTML;
            }
            event.clone.replaceWith(event.item);
        }
    } as Sortable.Options;

    /* method */
    const onDraggableClone = (event: Sortable.SortableEvent) => {
        const { newIndex, from, to } = event;
        if ((from === to || newIndex === undefined) && !get(editTabDropData)) return;
        if (get(editTabDropData) && get(editTabDropData)?.tabId === get(editTabCurrentId) && !htmlString) return;

        //機能停止
        setSortableDisabled(true);

        let cloneItem: ColumnData | undefined;
        const itemEl = event.item;
        const role = itemEl.dataset.columnRole;
        const columnName = itemEl.dataset.columnName;
        const index = get(columnCollection).findIndex(item => item.ColumnName === columnName);
        let cloneIndex = newIndex || 0;
        if (get(editTabDropData) && get(editTabDropData)!.tabId !== get(editTabCurrentId)) {
            cloneIndex = get(editorColumnHash)[get(editTabDropData)!.tabId].length;
        }
        let noSave = false;
        if (index > -1) {
            cloneItem = get(columnCollection)[index];
            $columnParamHash[cloneItem.ColumnName].State.Edit = 1;
        } else {
            const rssItem = get(cloneRssItems).find(item => item.Column.ColumnName === columnName);
            switch (role) {
                case 'Custom':
                    if (!rssItem) return false;
                    const hashKey = Object.keys($columnParamHash).find(key => {
                        const _hash = $columnParamHash[key];
                        if (_hash.Type === rssItem.Hash.Type && _hash.State.Edit === 0 && _hash.Category === 'Custom') {
                            return key;
                        }
                    });
                    if (hashKey) {
                        const columnKey = hashKey?.split(rssItem.Hash.Type)[1];
                        cloneItem = { ...rssItem.Column };
                        cloneItem.ColumnName = cloneItem.ColumnName + columnKey;
                        cloneItem.LabelText = (cloneItem.LabelText || rssItem.Column.ColumnName) + columnKey;
                        columnCollection.update(list => {
                            const _list = [...list];
                            if (cloneItem) _list.push(cloneItem);
                            return _list;
                        });
                        cloneRssItems.update(list => {
                            return list.map(item => {
                                if (item.Hash.Type === rssItem.Hash.Type) {
                                    return {
                                        ...item,
                                        Hash: { ...item.Hash, Count: item.Hash.Count - 1 }
                                    };
                                }
                                return item;
                            });
                        });
                        $columnParamHash[cloneItem.ColumnName].State.Edit = 1;
                    }
                    break;
                case 'Others':
                    if (!rssItem) return false;
                    switch (rssItem.Hash.Type) {
                        case 'Section':
                            $sections.LatestId++;
                            cloneItem = { ...rssItem.Column };
                            cloneItem.ColumnName = `_Section-${$sections.LatestId}`;
                            cloneItem.Id = $sections.LatestId;
                            $sections = { ...$sections, items: [...$sections.items, cloneItem as SectionData] };
                            $columnCollection = [...$columnCollection, cloneItem];
                            $columnParamHash[cloneItem.ColumnName] = {
                                Type: 'Section',
                                Category: 'Others',
                                State: {
                                    Edit: 1,
                                    Grid: -1,
                                    Filter: -1
                                }
                            };
                            break;
                        case 'LineBreak':
                            if (cloneIndex <= 0 || cloneIndex >= get(editorColumnHash)[get(editTabCurrentId)].length) {
                                //先頭と最後は改行不可
                                noSave = true;
                            } else {
                                rssItem.Hash.Count++;
                                cloneItem = { ...rssItem.Column };
                                cloneItem.ColumnName = `_Break-${rssItem.Hash.Count}`;

                                //重複処理
                                let _list = cloneDeep(get(editorColumnHash)[get(editTabCurrentId)]);
                                _list.splice(cloneIndex, 0, cloneItem.ColumnName);
                                _list = _list.filter(columnName => {
                                    if (cloneItem?.ColumnName === columnName) {
                                        return true;
                                    } else {
                                        return get(columnParamHash)[columnName].State.Edit !== -1;
                                    }
                                });
                                const _index = _list.findIndex(columnName => columnName === cloneItem?.ColumnName);
                                if (
                                    _index <= 0 ||
                                    _index >= _list.length - 1 ||
                                    isBreakColumn(_list[_index + 1]) ||
                                    isBreakColumn(_list[_index - 1]) ||
                                    !isEditableColumn(_list[_index + 1])
                                ) {
                                    noSave = true;
                                } else {
                                    $columnParamHash[cloneItem.ColumnName] = {
                                        Type: 'LineBreak',
                                        Category: 'Others',
                                        State: {
                                            Edit: 1,
                                            Grid: -1,
                                            Filter: -1
                                        }
                                    };
                                }
                            }
                            break;
                    }
                    break;
                case 'Links':
                    if (!columnName) return false;
                    cloneItem = { ColumnName: columnName };
                    $columnParamHash[cloneItem.ColumnName].State.Edit = 1;
                    break;
            }
        }

        if (!noSave && cloneItem) {
            // EditorColumnHashに追加
            const tabId = get(editTabDropData)?.tabId || get(editTabCurrentId);
            const _list = [...get(editorColumnHash)[tabId]];
            _list.splice(cloneIndex, 0, cloneItem.ColumnName);

            //無効な改行を削除
            const _deleteList = getInvalidLineBreaks(_list);
            if (_deleteList.length) {
                for (const _deleteIndex of _deleteList) {
                    _list.splice(_deleteIndex, 1);
                }
            }

            if (!get(editTabDropData)) {
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
            }

            editorColumnHash.update(hash => ({ ...hash, [tabId]: _list }));
            setAppEdited();

            setTimeout(() => {
                shadowListElement.innerHTML = '';
                htmlString = undefined;
                //機能再開
                setSortableDisabled(false);
            });
        } else {
            //機能再開
            setSortableDisabled(false);
            noSave = true;
        }

        $editTabDropData = null;
    };

    const onDraggableSort = (event: Sortable.SortableEvent) => {
        const { from, to, newIndex, oldIndex } = event;
        if (from !== to || (newIndex === oldIndex && !get(editTabDropData)) || oldIndex === undefined) return;
        if (get(editTabDropData) && get(editTabDropData)?.tabId === get(editTabCurrentId)) {
            $editTabDropData = null;
        }

        void draggableContainer.withSortablePaused(async () => {
            await new Promise<void>(resolve => {
                let _list: string[] = [];
                Array.from(to.children).forEach(element => {
                    const columnName = (element as HTMLElement).dataset.columnName;
                    if (columnName) {
                        if (!get(editTabDropData) || get(editTabDropData)?.columnId !== columnName) {
                            _list.push(columnName);
                        } else {
                            const targetTabId = get(editTabDropData)!.tabId;
                            editorColumnHash.update(hash => ({
                                ...hash,
                                [targetTabId]: [...hash[targetTabId], columnName]
                            }));
                            element.remove();
                        }
                    }
                });

                //無効な改行を削除
                const _deleteList = getInvalidLineBreaks(_list, get(editorColumnHash)[get(editTabCurrentId)][oldIndex]);
                if (_deleteList.length) {
                    for (const _deleteIndex of _deleteList) {
                        _list.splice(_deleteIndex, 1);
                    }
                }

                $editTabDropData = null;
                sortableColumns = _list;
                editorColumnHash.update(hash => ({ ...hash, [$editTabCurrentId]: _list }));
                setAppEdited();
                setTimeout(() => {
                    resolve();
                }, 240);
            });
        });
    };

    // Tab:DragStart
    const onColumnDragStart = (elem: HTMLElement) => {
        const columnName = elem.dataset.columnName;
        if (columnName && !elem.classList.contains('is-break')) {
            $editTabDraggingColumn = columnName;
        }
    };

    // Tab:DragEnd
    const onColumnDragEnd = () => {
        if (!get(editTabDraggingColumn)) return;
        $editTabDraggingColumn = undefined;
    };

    const getInvalidLineBreaks = (array: string[], target?: string) => {
        let list: number[] = [];
        let _copyList = array.filter((columnName: string) => {
            if (target && target === columnName) {
                return true;
            } else {
                return get(columnParamHash)[columnName].State.Edit !== -1;
            }
        });

        /* 無効な改行をリスト化*/
        if (isBreakColumn(_copyList[0])) {
            list.push(array.indexOf(_copyList[0]));
        } else if (isBreakColumn(_copyList[_copyList.length - 1])) {
            list.push(array.indexOf(_copyList[_copyList.length - 1]));
        } else {
            _copyList.forEach((_col, _idx) => {
                if (!isEditableColumn(_col) && isBreakColumn(_copyList[_idx - 1])) {
                    list.push(array.indexOf(_copyList[_idx - 1]));
                }
            });
        }
        return list;
    };

    const isBreakColumn = (columnName: string) => {
        return !!columnName?.match(/_Break-/);
    };
    const isEditableColumn = (columnName: string) => {
        if (!columnName) {
            return false;
        } else if (columnName.match(/_Links-/)) {
            return false;
        } else if (columnName.match(/_Break-/)) {
            return false;
        } else if (columnName.match(/_Section-/)) {
            return false;
        } else {
            return true;
        }
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
            case 'Section':
                editData = get(sections).items.find(data => {
                    return data.ColumnName === columnName;
                }) as ColumnData;
                break;
            case 'LineBreak':
                editData = {
                    ColumnName: columnName,
                    LabelText: pDisplay('LineBreak')
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

    const onTabChange = async (tab: string) => {
        sortableColumns = [];
        await draggableContainer.withSortablePaused(async () => {
            //不要なunitが残ってたら削除
            const dustElems = draggableContainer.options.stage?.querySelectorAll('.unit');
            dustElems &&
                dustElems.forEach(element => {
                    element.remove();
                });
            await new Promise<void>(resolve => {
                sortableColumns = $editorColumnHash[get(editTabCurrentId)];
                resolve();
            });
        });
    };
</script>

<div class="column-resource is_edit">
    <div class="resource-inner">
        <details class="accordion">
            <summary class="ac-ttl">
                <span class="label">{pDisplay('DefaultColumns')}</span>
                <span class="material-symbols-sharp"></span>
            </summary>
            <div class="ac-body">
                <Draggable options={resourceOptions}>
                    {#each get(defaultColumns).filter(name => $columnParamHash[name] && $columnParamHash[name].State.Edit !== -1) as name (name)}
                        <EditUnit
                            addState={$columnParamHash[name].State.Edit}
                            isResource={true}
                            isRole="Preset"
                            columnName={name}
                        />
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
                    {#each $cloneRssItems.filter(item => item.Hash.Category === 'Custom') as item (item.Column.ColumnName)}
                        <EditUnit addState={0} isRole="Custom" isResource={true} columnName={item.Column.ColumnName} />
                    {/each}
                </Draggable>
                {#if get(supportUrl)}
                    <div class="extendcolumns-link">
                        <a href={get(supportUrl)} target="_blank" rel="noreferrer noopener">
                            {pDisplay('DoNotHaveEnoughColumns')}
                        </a>
                    </div>
                {/if}
            </div>
        </details>
        {#if $linkTable.length}
            <details class="accordion">
                <summary class="ac-ttl">
                    <span class="label">{pDisplay('LinkTable')}</span>
                    <span class="material-symbols-sharp"></span>
                </summary>
                <div class="ac-body">
                    <Draggable options={resourceOptions}>
                        {#each $linkTable as { SiteId } (SiteId)}
                            <EditUnit
                                addState={$columnParamHash[`_Links-${SiteId}`].State.Edit}
                                isRole="Links"
                                isResource={true}
                                columnName={`_Links-${SiteId}`}
                            />
                        {/each}
                    </Draggable>
                </div>
            </details>
        {/if}
        <details class="accordion">
            <summary class="ac-ttl">
                <span class="label">{pDisplay('Others')}</span>
                <span class="material-symbols-sharp"></span>
            </summary>
            <div class="ac-body">
                <Draggable options={resourceOptions}>
                    {#each $cloneRssItems.filter(item => item.Hash.Category === 'Others') as item (item.Column.ColumnName)}
                        <EditUnit addState={0} isRole="Others" isResource={true} columnName={item.Hash.Type} />
                    {/each}
                </Draggable>
            </div>
        </details>
    </div>
</div>
<div class="main-container is_edit">
    <EditTabs {onTabChange} />
    <div class="column-sortable" class:isTagDragging={$editTabDraggingId}>
        {#if !$editorColumnHash[$editTabCurrentId].filter(name => name !== 'Comments').length}
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
                <EditUnit addState={-1} {columnName} {onModalOpen} />
            {/each}
        </Draggable>
    </div>
</div>

<EditModal bind:unitItem />

<style lang="scss">
    @use 'css/shared';
    @use 'css/container';

    .main-container {
        display: flex;
        flex-direction: column;
    }
    .column-sortable {
        :global(.draggable-container) {
            display: flex;
            flex-wrap: wrap;
            gap: 16px;
        }
        :global(.drag-ghost) {
            transition:
                filter 200ms ease-out,
                opacity 200ms ease-out;
        }
        &.isTagDragging {
            :global(.drag-ghost) {
                opacity: 0.5;
                filter: grayscale(100%);
            }
        }
    }
    .extendcolumns-link {
        padding: 12px 8px 0;
        margin-bottom: -4px;
        font-size: 13px;
        text-align: right;
        a {
            font-weight: 500;
            color: var(--link-text);
            text-decoration: none;
            &:hover {
                text-decoration: underline;
            }
        }
    }
</style>
