<script lang="ts">
    import { get } from 'svelte/store';
    import { isEqual, cloneDeep } from 'lodash';
    import type { ColumnData, ParamHash } from './types';
    import { getIcon, confirmDisplay, columnCollection, gridColumns, columnParamHash, setAppEdited } from './store';
    import UiModal from './Utility/UiModal.svelte';
    import NumberField from './Utility/form/NumberField.svelte';
    import CheckBox from './Utility/form/CheckBox.svelte';
    import CheckBoxGroup from './Utility/form/CheckBoxGroup.svelte';
    import Button from './Utility/Button.svelte';
    import { pDisplay } from './Utility/$p';

    export let unitItem: ColumnData | null = null;
    let item: ColumnData;
    let itemState = false;
    let isDeleteModal = false;
    let isDetailState = false;
    let isDeleteState = false;
    let detailContainer: HTMLElement;
    let detailModalElem: HTMLElement;

    let paramHash: ParamHash;

    $: if (unitItem) {
        if (!itemState) {
            item = cloneDeep(unitItem);
            if (item.CellWidth === 0) item.CellWidth = null;
            itemState = true;
        }
    }

    /* watch */
    $: if (unitItem) {
        paramHash = $columnParamHash[unitItem.ColumnName];
        isDeleteModal = paramHash.Type === 'LineBreak' || paramHash.Type === 'LinkTable';
        if (!isDeleteModal) {
            isDetailState = true;
            isDeleteState = false;
        } else {
            isDetailState = false;
            isDeleteState = true;
        }
    } else {
        unitItem = null;
        isDetailState = false;
        isDeleteState = false;
    }

    /* method */
    // @click onClose
    const onClose = () => {
        isDetailState = false;
        isDeleteState = false;
        unitItem = null;
        itemState = false;
    };

    // @click onExit
    const onExit = () => {
        if (unitItem && !Object.hasOwn(unitItem, 'CellSticky')) unitItem.CellSticky = false;
        if (unitItem && !Object.hasOwn(unitItem, 'CellWordWrap')) unitItem.CellWordWrap = false;
        if (item.CellWidth === null && unitItem?.CellWidth === 0) unitItem.CellWidth = null;

        if (isEqual(unitItem, item)) {
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
        if (unitItem) {
            const index = get(columnCollection).findIndex(data => data.ColumnName === item.ColumnName);
            if (index > -1) {
                if (item.CellWidth === null) item.CellWidth = 0;
                $columnCollection[index] = item;
            }
            setAppEdited();
            onClose();
        }
    };

    // @onDelete
    const onDelete = () => {
        if (unitItem) {
            const index = $gridColumns.findIndex(ColumnName => ColumnName === item.ColumnName);
            gridColumns.update(data => {
                return data.filter((_, i) => i !== index);
            });

            const shadowRoot = document.querySelector('smart-design')?.shadowRoot;
            const unitElms = shadowRoot?.querySelectorAll('.column-sortable .unit');
            unitElms?.forEach(elm => {
                if (elm.getAttribute('data-column-name') === item.ColumnName) {
                    elm.remove();
                }
            });

            $columnParamHash[unitItem.ColumnName].State.Grid = 0;
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
</script>

<UiModal bind:isState={isDetailState} bind:dialogContainer={detailContainer} {onClose} {onExit}>
    {#if item && paramHash}
        <div class="detail-modal" bind:this={detailModalElem}>
            <header class="modal-header">
                <div class="icon">
                    <span class="material-symbols-sharp is-fill">{getIcon(paramHash.Type, item.ColumnName)}</span>
                </div>
                <h1 class="title">{item?.LabelText || unitItem?.LabelText}</h1>
                <h2 class="column-name">{item.ColumnName}</h2>
            </header>
            <div class="modal-body">
                <h2 class="hdg"><span>{pDisplay('CellSettings')}</span></h2>
                <div class="editor-section">
                    <div class="unit is-fill">
                        <p class="ttl">{pDisplay('SetCellWidth')}</p>
                        <div class="form-item">
                            <NumberField bind:model={item.CellWidth} step={1} unit="px" isSpinner />
                        </div>
                    </div>
                    <div class="unit is-fill">
                        <div class="form-item">
                            <CheckBoxGroup>
                                <CheckBox bind:model={item.CellSticky}>{pDisplay('StickyOnLeftEdge')}</CheckBox>
                                <CheckBox bind:model={item.CellWordWrap}>{pDisplay('WrapTextInCell')}</CheckBox>
                            </CheckBoxGroup>
                        </div>
                    </div>
                </div>
            </div>

            <footer class="modal-footer">
                <p><Button onClick={onClose} icon={'close'}>{pDisplay('Close')}</Button></p>
                <p>
                    <Button onClick={onUpdate} type="positive" icon={'save'} disabled={!item?.LabelText}
                        >{pDisplay('Update')}</Button
                    >
                </p>
                <p>
                    <Button onClick={() => (isDeleteState = true)} type="negative" icon={'delete'}
                        >{pDisplay('Delete')}</Button
                    >
                </p>
            </footer>
        </div>
    {/if}
</UiModal>

<UiModal bind:isState={isDeleteState} onClose={onDeleteClose}>
    <div class="detail-modal is-delete">
        <header class="modal-header">
            <div class="icon">
                <span class="material-symbols-sharp is-fill">{getIcon(paramHash?.Type, item?.ColumnName)}</span>
            </div>
            <h1 class="title">{item?.LabelText || unitItem?.LabelText}</h1>
        </header>
        <div class="delete-body">
            <p>{pDisplay('ConfirmDeleteItem', item?.LabelText || unitItem?.LabelText)}</p>
            {#if paramHash?.Category === 'Custom'}
                <p class="warning">{pDisplay('ResetAdditionalItemsEditing')}</p>
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
        max-width: 500px;
        font-size: 13px;
        color: var(--sd-modal-text);
        background-color: var(--sd-modal-bg);
        border-radius: 5px;
        box-shadow: 0 0 10px var(--sd-base-shadow);
        &.is-delete {
            max-width: 440px;
        }
        .modal-header {
            position: sticky;
            top: -2px;
            z-index: 100;
            display: flex;
            gap: 8px;
            align-items: center;
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
            .column-name {
                padding: 2px 8px;
                margin-left: auto;
                font-size: 12px;
                font-weight: normal;
                background-color: var(--sd-modal-header-sub);
                border-radius: 4px;
            }
        }

        .modal-body {
            width: 500px;
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
