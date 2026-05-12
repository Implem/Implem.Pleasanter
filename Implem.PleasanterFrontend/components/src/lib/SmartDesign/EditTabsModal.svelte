<script lang="ts">
    import { get } from 'svelte/store';
    import { isEqual, cloneDeep } from 'lodash';
    import type { EditTabsData } from './types';
    import {
        confirmDisplay,
        columnCollection,
        cloneRssItems,
        editorColumnHash,
        editTabs,
        gridColumns,
        linkTable,
        sections,
        columnParamHash,
        setAppEdited,
        editTabCurrentId
    } from './store';
    import UiModal from './Utility/UiModal.svelte';
    import TextField from './Utility/form/TextField.svelte';
    import Button from './Utility/Button.svelte';
    import { pDisplay } from './Utility/$p';

    export let tabItem: EditTabsData | null = null;
    export let isDeleteModal: boolean = false;
    export let dataSync: (reload?: boolean) => void;

    let item: EditTabsData | null;
    let itemState = false;
    let isNewTab: boolean = false;
    let isDetailState = false;
    let isDeleteState = false;
    let detailContainer: HTMLElement;
    let detailModalElem: HTMLElement;
    let tabViewId: string | undefined;
    let columnCount: number | undefined;
    /* watch */
    $: if (tabItem) {
        if (!itemState) {
            itemState = true;
            tabViewId = getTabId(tabItem.Id);
            item = cloneDeep(tabItem);
            isNewTab = get(editTabs).items.findIndex(item => item.Id === tabItem?.Id) === -1;
            columnCount = get(editorColumnHash)[tabViewId]?.length || 0;
            if (!isDeleteModal) {
                isDetailState = true;
                isDeleteState = false;
            } else {
                if (columnCount) {
                    isDetailState = false;
                    isDeleteState = true;
                } else {
                    onDelete();
                }
            }
        }
    } else {
        onClose();
    }

    /* method */
    // @click onClose
    const onClose = () => {
        isDeleteModal = false;
        isDetailState = false;
        isDeleteState = false;
        tabViewId = undefined;
        columnCount = undefined;
        setTimeout(() => {
            tabItem = null;
            item = null;
            itemState = false;
            isNewTab = false;
        }, 240);
    };

    // @click onExit
    const onExit = () => {
        if (isEqual(tabItem, item)) {
            onClose();
        } else {
            $confirmDisplay = {
                message: pDisplay('CloseDialogWithoutSaving'),
                onExecute: onClose
            };
        }
    };

    // @onUpdate
    const onUpdate = () => {
        if (tabItem && item) {
            const index = get(editTabs).items.findIndex(data => data.Id === item?.Id);
            if (index > -1) {
                const current = get(editTabs);
                const items = [...current.items];
                items[index] = item;
                editTabs.set({ ...current, items });
                dataSync();
            } else {
                $editTabs = { LatestId: item.Id, items: [...$editTabs.items, item] };
                $editorColumnHash[tabViewId!] = [];
                dataSync();
            }
            setAppEdited();
            onClose();
        }
    };

    // @onDelete
    const onDelete = () => {
        if (tabItem && item) {
            //editTabs.itemsから削除
            const tabIndex = get(editTabs).items.findIndex(data => data.Id === item?.Id);
            const current = get(editTabs);
            const items = [...current.items];
            items.splice(tabIndex, 1);
            editTabs.set({ ...current, items });

            // ColumnParamHashのStateを変更
            if (columnCount && tabViewId) {
                get(editorColumnHash)[tabViewId].forEach(columnName => {
                    const paramHash = get(columnParamHash)[columnName];
                    const index = get(columnCollection).findIndex(data => data.ColumnName === columnName);
                    if (!paramHash) return;
                    switch (paramHash.Category) {
                        case 'Preset':
                        case 'Links':
                        case 'Custom':
                            $columnParamHash[columnName].State.Edit = 0;

                            if (paramHash.Category === 'Custom') {
                                cloneRssItems.update(list => {
                                    return list.map(data => {
                                        if (data.Hash.Type === paramHash.Type) {
                                            return {
                                                ...data,
                                                Hash: { ...data.Hash, Count: data.Hash.Count + 1 }
                                            };
                                        }
                                        return data;
                                    });
                                });
                                gridColumns.update(list => {
                                    return list.filter(id => id !== columnName);
                                });
                                if ($linkTable.find(item => item.ColumnName === columnName)) {
                                    //LinkされているClassが削除されると該当するLinkも除外
                                    const linkItem = $linkTable.find(item => item.ColumnName === columnName);
                                    const linkColumnName = `_Links-${linkItem?.SiteId}`;

                                    //EditorColumnHashから削除
                                    editorColumnHash.update(data => {
                                        const newData = { ...data };
                                        for (const key in newData) {
                                            if (Array.isArray(newData[key])) {
                                                const _index = newData[key].findIndex(name => name === linkColumnName);
                                                if (_index >= 0) {
                                                    newData[key] = [...newData[key]];
                                                    newData[key].splice(_index, 1);
                                                }
                                            }
                                        }
                                        return newData;
                                    });

                                    //ColumnParamHashから削除
                                    columnParamHash.update(data => {
                                        const _data = { ...data };
                                        delete _data[linkColumnName];
                                        return _data;
                                    });

                                    //Linksから削除
                                    linkTable.update(list => {
                                        const _list = [...list];
                                        const _index = _list.findIndex(data => data.ColumnName === columnName);
                                        if (_index >= 0) {
                                            _list.splice(_index, 1);
                                        }
                                        return _list;
                                    });
                                }

                                columnCollection.update(list => {
                                    const _list = [...list];
                                    _list.splice(index, 1);
                                    return _list;
                                });
                            }
                            break;
                        case 'Others':
                            columnParamHash.update(data => {
                                const _data = { ...data };
                                delete _data[columnName];
                                return _data;
                            });
                            switch (paramHash.Type) {
                                case 'Section':
                                    const _id = Number(columnName.split('_Section-')[1]);
                                    sections.update(data => {
                                        const _data = { ...data };
                                        return { ...data, items: _data.items.filter(section => section.Id !== _id) };
                                    });
                                    columnCollection.update(list => {
                                        const _list = [...list];
                                        _list.splice(index, 1);
                                        return _list;
                                    });
                                    break;

                                case 'LineBreak':
                                    break;
                            }
                            break;
                    }
                });
            }

            //EditorColumnHashから削除
            if (tabViewId && get(editorColumnHash)[tabViewId]) {
                const targetTabId = tabViewId;
                editorColumnHash.update(data => {
                    const { [targetTabId]: _, ...rest } = data;
                    return rest;
                });
            }

            //表示中のタブは削除後Generalを表示
            if (tabViewId === get(editTabCurrentId)) {
                $editTabCurrentId = getTabId(0);
                dataSync(true);
            } else {
                dataSync();
            }
        }
        setAppEdited();
        onClose();
    };

    // @click onDeleteClose
    const onDeleteClose = () => {
        if (!isDeleteModal) {
            isDeleteState = false;
        } else {
            onClose();
        }
    };

    const getTabId = (id: number) => {
        if (id === 0) {
            return 'General';
        } else {
            return `_Tab-${id}`;
        }
    };
</script>

<UiModal bind:isState={isDetailState} bind:dialogContainer={detailContainer} {onClose} {onExit}>
    {#if item}
        <div class="detail-modal" bind:this={detailModalElem}>
            <header class="modal-header">
                <div class="icon">
                    <span class="material-symbols-sharp is-fill">crop_16_9</span>
                </div>
                <h1 class="title">
                    {#if tabItem?.LabelText}
                        {tabItem?.LabelText}
                    {:else}
                        {pDisplay('NewTab')}
                    {/if}
                </h1>
                <h2 class="tab-id">ID:{item?.Id}</h2>
            </header>
            <div class="modal-body">
                <h2 class="hdg"><span>{pDisplay('TabSettings')}</span></h2>
                <div class="editor-section">
                    <div class="unit is-fill">
                        <p class="ttl is-required">{pDisplay('DisplayName')}</p>
                        <div class="form-item">
                            <TextField bind:model={item.LabelText} required />
                        </div>
                    </div>
                </div>
            </div>

            <footer class="modal-footer">
                <p><Button onClick={onClose} icon={'close'}>{pDisplay('Close')}</Button></p>
                <p>
                    <Button onClick={onUpdate} type="positive" icon={'save'} disabled={!item?.LabelText}>
                        {#if !isNewTab}
                            {pDisplay('Update')}
                        {:else}
                            {pDisplay('Add')}
                        {/if}
                    </Button>
                </p>
                {#if item?.Id !== 0 && !isNewTab}
                    <p>
                        <Button onClick={() => (isDeleteState = true)} type="negative" icon={'delete'}
                            >{pDisplay('Delete')}</Button
                        >
                    </p>
                {/if}
            </footer>
        </div>
    {/if}
</UiModal>

<UiModal bind:isState={isDeleteState} onClose={onDeleteClose}>
    <div class="detail-modal">
        <header class="modal-header">
            <div class="icon">
                <span class="material-symbols-sharp is-fill">crop_16_9</span>
            </div>
            <h1 class="title">{tabItem?.LabelText}</h1>
        </header>
        <div class="delete-body">
            <p>{pDisplay('ConfirmDeleteItem', item?.LabelText || tabItem?.LabelText)}</p>
            {#if columnCount}
                <p class="warning">
                    {pDisplay('TabUsedInputItemsDeleted')}<br />
                    {pDisplay('ResetAdditionalItemsEditing')}
                </p>
            {/if}
        </div>
        <footer class="modal-footer">
            <p><Button onClick={onDeleteClose} icon={'arrow_back_ios_new'}>{pDisplay('Cancel')}</Button></p>
            <p><Button onClick={onDelete} type="negative" icon={'delete'}>{pDisplay('Delete')}</Button></p>
        </footer>
    </div>
</UiModal>

<style lang="scss">
    @use 'css/shared';

    .detail-modal {
        position: relative;
        font-size: 13px;
        color: var(--sd-modal-text);
        background-color: var(--sd-modal-bg);
        border-radius: 5px;
        box-shadow: 0 0 10px var(--sd-base-shadow);
        .modal-header {
            position: sticky;
            top: -2px;
            z-index: 100;
            display: flex;
            gap: 8px;
            align-items: center;
            max-width: 440px;
            padding: 4px 8px;
            color: var(--sd-modal-header-text);
            background-color: var(--sd-modal-header-bg);
            border-radius: 4px 4px 0 0;
            .icon {
                display: flex;
                align-items: center;
                justify-content: center;
            }
            .title {
                overflow: hidden;
                text-overflow: ellipsis;
                font-size: 12px;
                font-weight: normal;
                white-space: nowrap;
            }
            .tab-id {
                padding: 2px 8px;
                margin-left: auto;
                font-size: 12px;
                font-weight: normal;
                background-color: var(--sd-modal-header-sub);
                border-radius: 4px;
            }
        }

        .modal-body {
            width: 440px;
            padding: 24px;
        }
        .hdg {
            position: relative;
            margin-bottom: 16px;
            font-size: 14px;
            line-height: 1.2;
            &::before {
                position: absolute;
                top: 50%;
                z-index: 0;
                width: 100%;
                height: 1px;
                content: '';
                border-top: 1px solid var(--sd-modal-heading-border);
            }
            span {
                position: relative;
                z-index: 1;
                padding-right: 8px;
                background-color: var(--sd-modal-bg);
            }
        }
        .editor-section {
            display: flex;
            flex-wrap: wrap;
            gap: 16px 0;
            align-items: flex-start;
            width: 100%;
            + .hdg {
                margin-top: 40px;
            }

            .unit {
                width: 50%;
                padding: 0 8px;
                &.isFill,
                &.is-fill {
                    width: 100%;
                }
                &.is-small {
                    flex: 1;
                    width: 30%;
                }
                &.is-min {
                    width: auto;
                }
                .ttl {
                    margin-bottom: 4px;
                    &.is-required {
                        position: relative;
                        &::after {
                            margin-left: 3px;
                            color: var(--sd-modal-required-text);
                            content: '*';
                        }
                    }
                }
                .form-item {
                    min-height: 32px;
                    + .form-item {
                        margin-top: 8px;
                    }
                    .check-item {
                        padding-top: 8px;
                    }
                }
                &:not(:has(.ttl)) {
                    align-self: flex-end;
                }
            }
        }

        .modal-footer {
            position: sticky;
            bottom: -2px;
            z-index: 100;
            display: flex;
            gap: 8px;
            justify-content: center;
            width: 100%;
            padding: 16px 0;
            overflow: hidden;
            background: var(--sd-footer-command);
            border-radius: 0 0 4px 4px;
        }

        .delete-body {
            display: flex;
            flex-direction: column;
            gap: 8px;
            align-items: center;
            justify-content: center;
            min-width: 400px;
            max-width: 440px;
            min-height: 100px;
            padding: 16px;
            overflow-wrap: anywhere;
            .warning {
                color: var(--sd-modal-delete-warning);
                text-align: center;
            }
        }
    }
</style>
