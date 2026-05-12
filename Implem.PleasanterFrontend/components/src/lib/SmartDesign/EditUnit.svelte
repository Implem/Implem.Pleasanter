<script lang="ts">
    /* module */
    import type { ColumnData, ParamHash, CloneRssData } from './types';
    import { getIcon, viewType, cloneRssItems, columnCollection, columnParamHash, sections } from './store';
    import { pDisplay } from './Utility/$p';

    /* params */
    export let columnName: string;
    export let isResource: boolean = false;
    export let isRole: string | undefined = undefined;
    export let addState: -1 | 0 | 1 = -1;
    export let onModalOpen: ((ColumnName: string) => void) | undefined = undefined;

    let item: ColumnData | undefined;
    let hash: ParamHash | undefined;
    let customParam: CloneRssData | undefined;

    $: {
        switch (isRole) {
            case 'Custom':
                customParam = $cloneRssItems.find(data => {
                    return data.Column.ColumnName === columnName;
                });
                if (customParam) {
                    item = customParam.Column;
                    if (item.LabelText) {
                        item.LabelText = pDisplay(item.ColumnName);
                    }
                    hash = {
                        ...customParam.Hash,
                        State: {
                            Edit: 0,
                            Grid: -1,
                            Filter: -1
                        }
                    };
                }
                if (customParam && 0 >= customParam.Hash.Count) {
                    addState = 1;
                } else {
                    addState = 0;
                }
                break;
            case 'Links':
                hash = $columnParamHash[columnName];
                item = {
                    ColumnName: columnName,
                    LabelText: hash.LinkName
                };
                break;
            case 'Others':
                const rssItem = $cloneRssItems.find(data => data.Hash.Type === columnName);
                if (rssItem) {
                    item = rssItem.Column;
                    if (item.LabelText) {
                        item.LabelText = pDisplay(item.ColumnName);
                    }
                    hash = {
                        ...rssItem.Hash,
                        State: {
                            Edit: 1,
                            Grid: -1,
                            Filter: -1
                        }
                    };
                }
                break;
            default:
                hash = $columnParamHash[columnName];
                if (!hash) break;
                if (hash.Type === 'Section') {
                    item = $sections.items.find(data => {
                        if (`_Section-${data.Id}` === columnName) {
                            return data;
                        }
                    }) as ColumnData;
                    if (item) {
                        item.ColumnName = columnName;
                    }
                } else {
                    item = $columnCollection.find(data => data.ColumnName === columnName);
                }
                if (!item && hash) {
                    switch (hash.Type) {
                        case 'LinkTable':
                            item = {
                                ColumnName: columnName,
                                LabelText: hash.LinkName
                            };
                            break;
                    }
                }
                break;
        }
    }
</script>

{#if item && hash}
    <div
        role="button"
        tabindex="-1"
        data-column-name={item.ColumnName}
        data-column-role={isRole}
        class="unit"
        class:is-break={columnName === 'LineBreak'}
        class:is-section={hash.Type === 'Section' || item.ColumnName === 'Section'}
        class:is-wide={item.FieldCss === 'field-wide' || item.ChoicesControlType === 'Radio'}
        class:is-markdown={item.FieldCss === 'field-markdown' ||
            item.FieldCss === 'field-rte' ||
            hash.Type === 'Attachments'}
        class:is-linktable={hash.Type === 'LinkTable'}
        class:is-added={addState === 1}
        class:is-hidden={hash.State.Edit === -1}
        on:dblclick={() => item && onModalOpen && onModalOpen(item.ColumnName)}
    >
        <div class="unit-inner">
            {#if isResource}
                <div class="unit-header">
                    <div class="icon">
                        <span class="material-symbols-sharp is-fill">{getIcon(hash.Type, columnName)}</span>
                    </div>
                    {#if isRole === 'Preset' || isRole === 'Links'}
                        <p class="label">{item.LabelText}</p>
                    {:else}
                        <p class="label">{item.LabelText}</p>
                    {/if}
                    {#if isRole === 'Custom' && customParam}
                        <div class="count">
                            <span>{pDisplay('Remaining')}</span>
                            <span>{customParam.Hash.Count}</span>
                        </div>
                    {/if}
                    <div class="handler"><span class="material-symbols-sharp">drag_indicator</span></div>
                </div>
            {:else}
                <div class="unit-header">
                    <div class="icon">
                        <span class="material-symbols-sharp is-fill">{getIcon(hash.Type, columnName)}</span>
                    </div>
                    <p class="label">{item.LabelText}</p>
                </div>
                {#if isRole !== 'Others' && hash.Type !== 'Section'}
                    <div class="unit-body">
                        {#if item.ValidateRequired}
                            <p class="tag required">{pDisplay('Required')}</p>
                        {/if}
                        {#if item.EditorReadOnly || item.ColumnName === 'RemainingWorkValue'}
                            <p class="tag readonly">{pDisplay('ReadOnly')}</p>
                        {/if}
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

                        {#if viewType(hash.Type) === 'Class'}
                            {#if item.ChoicesControlType === 'DropDown' && item.ChoicesText}
                                <p class="tag">{pDisplay('DropDownList')}</p>
                            {:else if item.ChoicesControlType === 'Radio' && item.ChoicesText}
                                <p class="tag">{pDisplay('RadioButton')}</p>
                            {/if}
                        {:else if viewType(hash.Type) === 'Num'}
                            {#if item.ControlType === 'Spinner'}
                                <p class="tag">{pDisplay('Spinner')}</p>
                            {/if}
                        {:else if viewType(hash.Type) === 'Date'}
                            {#if item.EditorFormat === 'Ymd'}
                                <p class="tag">{pDisplay('Ymd')}</p>
                            {:else if item.EditorFormat === 'Ymdhm'}
                                <p class="tag">{pDisplay('Ymdhm')}</p>
                            {:else if item.EditorFormat === 'Ymdhms'}
                                <p class="tag">{pDisplay('Ymdhms')}</p>
                            {/if}
                        {:else if viewType(hash.Type) === 'Description'}
                            {#if item.FieldCss === 'field-markdown'}
                                <p class="tag">{pDisplay('MarkDown')}</p>
                            {:else if item.FieldCss === 'field-rte'}
                                <p class="tag">{pDisplay('RichTextEditor')}</p>
                            {/if}
                        {/if}
                    </div>
                {/if}
            {/if}
        </div>

        {#if onModalOpen}
            <button class="btn-setting" on:click={() => item && onModalOpen(item.ColumnName)}>
                {#if hash.Type !== 'LinkTable'}
                    <span class="material-symbols-sharp is-fill">settings</span>
                {:else}
                    <span class="material-symbols-sharp is-fill is-delete">delete</span>
                {/if}
            </button>
        {/if}
    </div>
{:else if hash && hash.Type === 'LineBreak'}
    <div
        role="button"
        tabindex="-1"
        data-column-name={columnName}
        data-column-role={isRole}
        class="unit is-break"
        on:dblclick={() => onModalOpen && onModalOpen(columnName)}
    >
        {#if onModalOpen}
            <button class="btn-setting" on:click={() => onModalOpen && onModalOpen(columnName)}>
                <span class="material-symbols-sharp is-fill">delete</span>
            </button>
        {/if}
    </div>
{/if}

<style lang="scss">
    @use 'css/shared';
    @use 'css/unit';

    .unit {
        /* -------------------------------------------
        .edit-resource
        ------------------------------------------- */
        :global(.column-resource) & {
            .unit-inner {
                .count {
                    display: flex;
                    gap: 2px;
                    align-items: center;
                    padding: 4px 8px;
                    font-size: 13px;
                    line-height: 1;
                    color: var(--sd-unit-edit-count-text);
                    background: var(--sd-unit-edit-count-bg);
                    border-radius: 4px;
                }
            }
        }

        /* -------------------------------------------
        .edit-sortable
        ------------------------------------------- */
        :global(.column-sortable) &,
        &.drag-ghost {
            .unit-inner {
                display: flex;
                flex-direction: column;
                justify-content: space-between;
                width: calc(340px - 16px);
                min-height: calc(100px - 16px);
                background: var(--sd-unit-sortable-bg);
                border: 1px solid var(--sd-unit-border);
                .handler {
                    display: none;
                }
                .label {
                    padding-right: 16px;
                    font-size: 16px;
                }
                .count {
                    display: none;
                }
            }
            .btn-setting {
                position: absolute;
                top: 0;
                right: 0;
                display: flex;
                align-items: center;
                justify-content: center;
                width: 30px;
                height: 30px;
                color: var(--sd-unit-icon-setting);
                cursor: pointer;
                outline: none;
                background: transparent;
                border: 0;
                border-radius: 4px;
                span {
                    font-size: 16px;
                }
                .is-delete {
                    font-size: 22px;
                    &:hover {
                        color: var(--sd-unit-edit-delete-bg);
                        transition: color 200ms ease-out;
                    }
                }
            }
            &:not(.is-section) {
                .unit-header {
                    gap: 8px;
                    .icon {
                        width: 32px;
                        height: 32px;
                        .material-symbols-sharp {
                            font-size: 18px;
                        }
                    }
                }
                .unit-body {
                    display: flex;
                    flex-wrap: wrap;
                    gap: 4px;
                    margin-top: 8px;
                    .tag {
                        padding: 0 4px;
                        font-size: 12px;
                        color: var(--sd-unit-edit-tag-text);
                        background: var(--sd-unit-edit-tag-bg);
                        border: var(--sd-unit-edit-tag-border) 1px solid;
                        border-radius: 2px;
                        &.required {
                            color: var(--sd-unit-edit-tag-required-text);
                            background: var(--sd-unit-edit-tag-required-bg);
                            border: var(--sd-unit-edit-tag-required-border) 1px solid;
                        }
                        &.readonly {
                            color: var(--sd-unit-edit-tag-readonly-text);
                            background: var(--sd-unit-edit-tag-readonly-bg);
                            border: var(--sd-unit-edit-tag-readonly-border) 1px solid;
                        }
                    }
                }
            }

            // 幅ワイド
            &.is-wide {
                width: 100%;
                .unit-inner {
                    width: initial;
                }
            }

            // マークダウン
            &.is-markdown {
                width: 100%;
                .unit-inner {
                    width: initial;
                    height: 130px;
                }
            }

            // リンクテーブル
            &.is-linktable {
                width: 100%;
                .unit-inner {
                    width: initial;
                    height: 160px;
                }
            }

            // 見出し
            &.is-section {
                width: 100%;
                .unit-inner {
                    width: initial;
                    height: auto;
                    min-height: 0;
                    color: var(--sd-unit-edit-section-text);
                    background: var(--sd-unit-edit-section-bg);
                    border: none;
                }
                .btn-setting {
                    top: 50%;
                    color: var(--sd-unit-edit-section-text);
                    transform: translateY(-50%);
                }
            }

            // 改行
            &.is-break {
                width: 100%;
                height: 0;
                padding: 0;
                background-color: transparent;
                border-top: 4px dashed var(--sd-unit-edit-break-border);
                &::after {
                    position: absolute;
                    top: 0;
                    left: 0;
                    z-index: 1;
                    display: block;
                    width: 100%;
                    height: 20px;
                    content: ' ';
                    background-color: transparent;
                    transform: translateY(calc(-50% - 2px));
                }
                .unit-inner {
                    display: none;
                }
                .btn-setting {
                    z-index: 2;
                    color: var(--sd-unit-edit-delete-icon);
                    background-color: var(--sd-unit-edit-delete-bg);
                    opacity: 0;
                    transform: translateY(calc(-50% - 2px));
                    transition: opacity 200ms ease-out;
                }
                &:hover {
                    .btn-setting {
                        opacity: 1;
                    }
                }
            }

            // ghost
            &:is(:global(.drag-ghost)) {
                .btn-setting {
                    display: none;
                }
            }
        }
    }
</style>
