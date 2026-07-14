<script lang="ts">
    import { get } from 'svelte/store';
    import { isEqual, cloneDeep } from 'lodash';
    import type { ColumnData, ParamHash, SectionData } from './types';
    import {
        getIcon,
        viewType,
        confirmDisplay,
        columnCollection,
        cloneRssItems,
        editorColumnHash,
        gridColumns,
        filterColumns,
        linkTable,
        sections,
        columnParamHash,
        setAppEdited,
        editTabCurrentId
    } from './store';
    import UiModal from './Utility/UiModal.svelte';
    import TextField from './Utility/form/TextField.svelte';
    import TextArea from './Utility/form/TextArea.svelte';
    import NumberField from './Utility/form/NumberField.svelte';
    import DateField from './Utility/form/DateField.svelte';
    import SelectBox from './Utility/form/SelectBox.svelte';
    import CheckBox from './Utility/form/CheckBox.svelte';
    import FileField from './Utility/form/FileField.svelte';
    import CheckBoxGroup from './Utility/form/CheckBoxGroup.svelte';
    import RadioGroup from './Utility/form/RadioGroup.svelte';
    import ReadField from './Utility/form/ReadField.svelte';
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

    let demo: any;
    let paramHash: ParamHash;
    let attachmentsError: boolean = false;

    $: if (unitItem) {
        if (!itemState) {
            item = cloneDeep(unitItem);
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
        demo = undefined;
    };

    // @click onExit
    const onExit = () => {
        if (isEqual(unitItem, item)) {
            onClose();
        } else {
            $confirmDisplay = {
                message: pDisplay('CloseDialogWithoutSaving'),
                onExecute: onClose
            };
        }
    };

    // @onDemoChange
    const onDemoChange = (event: CustomEvent<{ value: string | number | boolean | undefined }>) => {
        demo = event.detail.value;
    };

    // @onUpdate
    const onUpdate = () => {
        if (unitItem) {
            const index = get(columnCollection).findIndex(data => data.ColumnName === item.ColumnName);

            if (paramHash.State.Grid === 1) {
                if (!get(gridColumns).includes(item.ColumnName)) $gridColumns = [...$gridColumns, item.ColumnName];
            } else if (paramHash.State.Grid === 0) {
                gridColumns.update(list => {
                    return list.filter(id => id !== item.ColumnName);
                });
            }

            if (paramHash.State.Filter === 1) {
                if (!get(filterColumns).includes(item.ColumnName))
                    $filterColumns = [...$filterColumns, item.ColumnName];
            } else if (paramHash.State.Filter === 0) {
                filterColumns.update(list => {
                    return list.filter(id => id !== item.ColumnName);
                });
            }

            if (index > -1) {
                if (!item.ColumnName.match(/Section/)) {
                    $columnCollection[index] = item;
                } else {
                    sections.update(data => {
                        const _index = data.items.findIndex(d => d.ColumnName === item.ColumnName);
                        if (_index > -1) {
                            const updatedItems = [...data.items];
                            updatedItems[_index] = item as SectionData;
                            return { ...data, items: updatedItems };
                        }
                        return data;
                    });
                }
            }
            setAppEdited();
            onClose();
        }
    };

    // @onDelete
    const onDelete = () => {
        if (unitItem) {
            const index = get(columnCollection).findIndex(data => data.ColumnName === item.ColumnName);
            const sortIndex = $editorColumnHash[get(editTabCurrentId)].findIndex(
                ColumnName => ColumnName === item.ColumnName
            );

            editorColumnHash.update(data => {
                const _list = [...data[get(editTabCurrentId)]];
                _list.splice(sortIndex, 1);
                return { ...data, [get(editTabCurrentId)]: _list };
            });

            const shadowRoot = document.querySelector('smart-design')?.shadowRoot;
            const unitElms = shadowRoot?.querySelectorAll('.column-sortable .unit');
            unitElms?.forEach(elm => {
                if (elm.getAttribute('data-column-name') === item.ColumnName) {
                    elm.remove();
                }
            });

            switch (paramHash.Category) {
                case 'Preset':
                case 'Links':
                case 'Custom':
                    $columnParamHash[unitItem.ColumnName].State.Edit = 0;

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
                            return list.filter(id => id !== item.ColumnName);
                        });
                        if ($linkTable.find(item => item.ColumnName === unitItem?.ColumnName)) {
                            //LinkされているClassが削除されると該当するLinkも除外
                            const linkItem = $linkTable.find(item => item.ColumnName === unitItem?.ColumnName);
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
                                const _index = _list.findIndex(data => data.ColumnName === unitItem?.ColumnName);
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
                        delete _data[item.ColumnName];
                        return _data;
                    });
                    switch (paramHash.Type) {
                        case 'Section':
                            const _id = Number(item.ColumnName.split('_Section-')[1]);
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

    const hasValueByList = (value: string, list: string[]) => {
        return list.includes(value);
    };

    const onDefaultValue = () => {
        demo = item.DefaultInput;
        if (item.FieldCss === 'field-rte') {
            const target = detailModalElem.querySelector('rt-editor');
            if (target) (target as any).value = demo;
        }
    };

    //DEMOの初期値
    $: if (unitItem) {
        if (unitItem.ColumnName === 'RemainingWorkValue') demo = 5;
    }
</script>

<UiModal bind:isState={isDetailState} bind:dialogContainer={detailContainer} {onClose} {onExit}>
    {#if item && paramHash}
        <div class="detail-modal" bind:this={detailModalElem}>
            <header class="modal-header">
                <div class="icon">
                    <span class="material-symbols-sharp is-fill">{getIcon(paramHash.Type, item.ColumnName)}</span>
                </div>
                {#if viewType(paramHash.Type) !== 'Section'}
                    <h1 class="title">{pDisplay(item.ColumnName)}</h1>
                    <h2 class="column-name">{item.ColumnName}</h2>
                {:else}
                    <h1 class="title">{pDisplay(paramHash.Type)}</h1>
                {/if}
            </header>
            <div class="modal-body">
                <h2 class="hdg"><span>{pDisplay('BasicSettings')}</span></h2>
                <div class="editor-section">
                    {#if viewType(paramHash.Type) === 'Section'}
                        <div class="unit">
                            <p class="ttl is-required">{pDisplay('Id')}</p>
                            <div class="form-item">
                                <ReadField bind:model={item.Id} />
                            </div>
                        </div>
                    {/if}
                    <div class="unit">
                        <p class="ttl is-required">{pDisplay('DisplayName')}</p>
                        <div class="form-item">
                            <TextField bind:model={item.LabelText} required />
                        </div>
                    </div>
                    {#if viewType(paramHash.Type) !== 'Section'}
                        <div class="unit">
                            <p class="ttl">{pDisplay('TextAlign')}</p>
                            <div class="form-item">
                                <SelectBox bind:model={item.TextAlign}>
                                    <option value={10}>{pDisplay('LeftAlignment')}</option>
                                    <option value={20}>{pDisplay('RightAlignment')}</option>
                                    <option value={15}>{pDisplay('CenterAlignment')}</option>
                                </SelectBox>
                            </div>
                        </div>
                        {#if paramHash.Type !== 'User' && hasValueByList( viewType(paramHash.Type), ['Class', 'Description'] ) && item.ColumnName !== 'Status' && item.FieldCss !== 'field-rte'}
                            <div class="unit">
                                <p class="ttl">{pDisplay('MaxLength')}</p>
                                <div class="form-item">
                                    <NumberField
                                        bind:model={item.MaxLength}
                                        step={1}
                                        max={1024}
                                        min={1}
                                        unit={pDisplay('Characters')}
                                        isSpinner
                                    />
                                </div>
                            </div>
                        {/if}
                        {#if viewType(paramHash.Type) !== 'Attachments'}
                            <div class="unit">
                                <p class="ttl">{pDisplay('Style')}</p>
                                <div class="form-item">
                                    <SelectBox bind:model={item.FieldCss}>
                                        {#if item.FieldCss === ''}
                                            <option value={''}>{pDisplay('Normal')}</option>
                                        {:else}
                                            <option value={'field-normal'}>{pDisplay('Normal')}</option>
                                        {/if}
                                        <option value={'field-wide'}>{pDisplay('Wide')}</option>
                                        {#if viewType(paramHash.Type) === 'Description'}
                                            <option value={'field-markdown'}>{pDisplay('MarkDown')}</option>
                                            <option value={'field-rte'}>{pDisplay('RichTextEditor')}</option>
                                        {/if}
                                    </SelectBox>
                                </div>
                            </div>

                            {#if item.ColumnName !== 'RemainingWorkValue' && viewType(paramHash.Type) !== 'ReadOnly'}
                                <div
                                    class="unit"
                                    class:isFill={(paramHash.Type !== 'User' &&
                                        item.ColumnName !== 'Status' &&
                                        viewType(paramHash.Type) === 'Class') ||
                                        viewType(paramHash.Type) === 'Description'}
                                >
                                    <p class="ttl">{pDisplay('DefaultInput')}</p>
                                    {#if viewType(paramHash.Type) === 'Check'}
                                        <div class="form-item">
                                            <SelectBox
                                                bind:model={item.DefaultInput}
                                                init={false}
                                                onInit={() => (demo = item.DefaultInput)}
                                                onChange={() => (demo = item.DefaultInput)}
                                                options={[
                                                    [true, 'ON'],
                                                    [false, 'OFF']
                                                ]}
                                            />
                                        </div>
                                    {:else if viewType(paramHash.Type) === 'Date'}
                                        <div class="form-item">
                                            <NumberField
                                                bind:model={item.DefaultInput}
                                                max={365}
                                                min={-365}
                                                isSpinner
                                            />
                                        </div>
                                    {:else if viewType(paramHash.Type) === 'Description' && (item.FieldCss === 'field-markdown' || item.FieldCss === 'field-rte')}
                                        <div class="form-item">
                                            <TextArea
                                                bind:model={item.DefaultInput}
                                                viewerType={3}
                                                onInit={() => onDefaultValue()}
                                                onInput={() => onDefaultValue()}
                                            />
                                        </div>
                                    {:else}
                                        <div class="form-item">
                                            <TextField
                                                bind:model={item.DefaultInput}
                                                onInit={() => onDefaultValue()}
                                                onInput={() => onDefaultValue()}
                                            />
                                        </div>
                                    {/if}
                                </div>
                            {/if}
                        {/if}

                        {#if paramHash.Type !== 'User' && !hasValueByList( viewType(paramHash.Type), ['Check', 'ReadOnly'] ) && !hasValueByList( item.ColumnName, ['Status', 'RemainingWorkValue', 'WorkValue', 'ProgressRate'] )}
                            <div class="unit is-fill">
                                <p class="ttl">{pDisplay('InputGuide')}</p>
                                <div class="form-item">
                                    <TextField bind:model={item.InputGuide} />
                                </div>
                            </div>
                        {/if}
                        {#if item.ColumnName !== 'RemainingWorkValue' && viewType(paramHash.Type) !== 'ReadOnly'}
                            <div class="unit is-fill">
                                <div class="form-item">
                                    <CheckBoxGroup>
                                        <CheckBox
                                            bind:model={item.ValidateRequired}
                                            readOnly={item.ColumnName === 'CompletionTime'}
                                            >{pDisplay('Required')}</CheckBox
                                        >
                                        <CheckBox bind:model={item.EditorReadOnly}>{pDisplay('ReadOnly')}</CheckBox>
                                    </CheckBoxGroup>
                                </div>
                            </div>
                        {/if}
                    {/if}

                    {#if item.ColumnName === 'RemainingWorkValue'}
                        <div class="unit">
                            <p class="ttl">{pDisplay('Unit')}</p>
                            <div class="form-item">
                                <TextField bind:model={item.Unit} />
                            </div>
                        </div>
                    {/if}
                </div>

                {#if viewType(paramHash.Type) === 'Class' && paramHash.Type !== 'Title'}
                    <h2 class="hdg"><span>{pDisplay('ClassSettings')}</span></h2>
                    <div class="editor-section">
                        <div class="unit is-fill">
                            <p class="ttl">{pDisplay('OptionList')}</p>
                            <div class="form-item">
                                <RadioGroup
                                    bind:model={item.ChoicesControlType}
                                    init={'DropDown'}
                                    disabled={!Boolean(item.ChoicesText)}
                                    options={[
                                        ['DropDown', pDisplay('DropDownList')],
                                        ['Radio', pDisplay('RadioButton')]
                                    ]}
                                />
                            </div>
                            <div class="form-item">
                                <TextArea bind:model={item.ChoicesText} viewerType={3} />
                            </div>
                        </div>
                    </div>
                {:else if viewType(paramHash.Type) === 'Num' && item.ColumnName !== 'RemainingWorkValue'}
                    <h2 class="hdg"><span>{pDisplay('NumSettings')}</span></h2>
                    <div class="editor-section">
                        <div class="unit is-small">
                            <p class="ttl">{pDisplay('ControlType')}</p>
                            <div class="form-item">
                                <RadioGroup
                                    bind:model={item.ControlType}
                                    init={'Normal'}
                                    options={[
                                        ['Normal', pDisplay('Normal')],
                                        ['Spinner', pDisplay('Spinner')]
                                    ]}
                                />
                            </div>
                        </div>
                        {#if item.ControlType === 'Spinner'}
                            <div class="unit is-small">
                                <p class="ttl">{pDisplay('Step')}</p>
                                <div class="form-item">
                                    <NumberField bind:model={item.Step} />
                                </div>
                            </div>
                        {/if}
                        <div class="unit">
                            <p class="ttl">{pDisplay('Unit')}</p>
                            <div class="form-item">
                                <TextField bind:model={item.Unit} />
                            </div>
                        </div>
                        <div class="unit">
                            <p class="ttl">{pDisplay('Min')}</p>
                            <div class="form-item">
                                <NumberField bind:model={item.Min} />
                            </div>
                        </div>
                        <div class="unit">
                            <p class="ttl">{pDisplay('Max')}</p>
                            <div class="form-item">
                                <NumberField bind:model={item.Max} />
                            </div>
                        </div>
                    </div>
                {:else if viewType(paramHash.Type) === 'Date'}
                    <h2 class="hdg"><span>{pDisplay('DateSettings')}</span></h2>
                    <div class="editor-section">
                        <div class="unit">
                            <p class="ttl">{pDisplay('EditorFormat')}</p>
                            <div class="form-item">
                                <RadioGroup
                                    bind:model={item.EditorFormat}
                                    init={'Ymd'}
                                    options={[
                                        ['Ymd', pDisplay('Ymd')],
                                        ['Ymdhm', pDisplay('Ymdhm')],
                                        ['Ymdhms', pDisplay('Ymdhms')]
                                    ]}
                                />
                            </div>
                        </div>
                        {#if item.EditorFormat === 'Ymdhm'}
                            <div class="unit">
                                <p class="ttl">{pDisplay('MinutesStep')}</p>
                                <div class="form-item">
                                    <SelectBox
                                        bind:model={item.DateTimeStep}
                                        init={10}
                                        options={[
                                            [1, '1'],
                                            [2, '2'],
                                            [3, '3'],
                                            [5, '5'],
                                            [6, '6'],
                                            [10, '10'],
                                            [15, '15'],
                                            [20, '20'],
                                            [30, '30'],
                                            [60, '60']
                                        ]}
                                    />
                                </div>
                            </div>
                        {/if}
                    </div>
                {:else if item.FieldCss === 'field-markdown'}
                    <h2 class="hdg"><span>{pDisplay('DescriptionSettings')}</span></h2>
                    <div class="editor-section">
                        <div class="unit">
                            <p class="ttl">{pDisplay('ViewerSwitchingType')}</p>
                            <div class="form-item">
                                <RadioGroup
                                    bind:model={item.ViewerSwitchingType}
                                    init={1}
                                    options={[
                                        [1, pDisplay('Auto')],
                                        [2, pDisplay('Manual')],
                                        [3, pDisplay('Disabled')]
                                    ]}
                                />
                            </div>
                        </div>
                        <div class="unit is-min">
                            <div class="form-item">
                                <p class="ttl">{pDisplay('Image')}</p>
                                <div class="check-item">
                                    <CheckBox bind:model={item.AllowImage}>{pDisplay('AllowImage')}</CheckBox>
                                </div>
                            </div>
                        </div>
                        {#if item.AllowImage}
                            <div class="unit is-small">
                                <p class="ttl">{pDisplay('ThumbnailLimitSize')}</p>
                                <div class="form-item">
                                    <NumberField
                                        bind:model={item.ThumbnailLimitSize}
                                        isSpinner
                                        min={100}
                                        max={1000}
                                        unit="px"
                                    />
                                </div>
                            </div>
                        {/if}
                    </div>
                {:else if viewType(paramHash.Type) === 'Section'}
                    <h2 class="hdg"><span>{pDisplay('SectionSettings')}</span></h2>
                    <div class="editor-section">
                        <div class="unit is-small">
                            <p class="ttl">{pDisplay('AllowCollapse')}</p>
                            <div class="form-item">
                                <CheckBoxGroup>
                                    <CheckBox bind:model={item.AllowExpand}>{pDisplay('AllowExpand')}</CheckBox>
                                </CheckBoxGroup>
                            </div>
                        </div>
                        {#if item.AllowExpand}
                            <div class="unit is-small">
                                <p class="ttl">{pDisplay('DefaultViewMode')}</p>
                                <div class="form-item">
                                    <RadioGroup
                                        bind:model={item.Expand}
                                        init={1}
                                        options={[
                                            [true, pDisplay('Open')],
                                            [false, pDisplay('Close')]
                                        ]}
                                    />
                                </div>
                            </div>
                        {/if}
                    </div>
                {/if}
            </div>

            {#if hasValueByList(paramHash.Category, ['Preset', 'Custom'])}
                <div class="preview">
                    <p class="preview-ttl"><span>{pDisplay('InputItemPreview')}</span></p>
                    <div class="preview-inner">
                        <div
                            class="preview-field"
                            class:is-wide={item.FieldCss === 'field-wide' ||
                                item.FieldCss === 'field-markdown' ||
                                item.FieldCss === 'field-rte' ||
                                paramHash.Type === 'Attachments' ||
                                (item.ChoicesControlType === 'Radio' && item.ChoicesText)}
                            class:is-center={item.TextAlign === 15}
                            class:is-right={item.TextAlign === 20}
                            class:is-required={item.ValidateRequired && !item.EditorReadOnly}
                        >
                            <p class="field-label">
                                {#if viewType(paramHash.Type) !== 'Check'}
                                    <span class="required-target">{item.LabelText}</span>
                                {/if}
                            </p>
                            <div class="field-control">
                                {#if viewType(paramHash.Type) === 'Class'}
                                    <div class="container-normal">
                                        {#if item.ChoicesControlType === 'DropDown' && item.ChoicesText}
                                            <SelectBox
                                                bind:model={demo}
                                                align={item.TextAlign}
                                                required={item.ValidateRequired}
                                                readOnly={item.EditorReadOnly}
                                                fieldSize={item.FieldCss}
                                                options={item.ChoicesText}
                                                user={paramHash.Type === 'User'}
                                            />
                                        {:else if item.ChoicesControlType === 'Radio' && item.ChoicesText}
                                            <div class="checkable-preview">
                                                <RadioGroup
                                                    bind:model={demo}
                                                    required={item.ValidateRequired}
                                                    disabled={item.EditorReadOnly}
                                                    options={item.ChoicesText}
                                                />
                                            </div>
                                        {:else}
                                            <TextField
                                                bind:model={demo}
                                                align={item.TextAlign}
                                                required={item.ValidateRequired}
                                                readOnly={item.EditorReadOnly}
                                                fieldSize={item.FieldCss}
                                                maxLength={item.MaxLength}
                                                placeholder={item.InputGuide || item.LabelText}
                                            />
                                        {/if}
                                    </div>
                                {:else if viewType(paramHash.Type) === 'Num'}
                                    <NumberField
                                        bind:model={demo}
                                        required={item.ValidateRequired}
                                        readOnly={item.EditorReadOnly || item.ColumnName === 'RemainingWorkValue'}
                                        fieldSize={item.FieldCss}
                                        align={item.TextAlign}
                                        unit={item.Unit}
                                        max={item.Max}
                                        min={item.Min}
                                        step={item.Step}
                                        isSpinner={item.ControlType === 'Spinner'}
                                        placeholder={item.InputGuide || item.LabelText}
                                    />
                                {:else if viewType(paramHash.Type) === 'Description'}
                                    {#if item.FieldCss === 'field-markdown'}
                                        <TextArea
                                            bind:model={demo}
                                            required={item.ValidateRequired}
                                            readOnly={item.EditorReadOnly}
                                            fieldSize={item.FieldCss}
                                            align={item.TextAlign}
                                            maxLength={item.MaxLength}
                                            placeholder={item.InputGuide || item.LabelText}
                                            viewerType={item.ViewerSwitchingType}
                                            AllowImage={item.AllowImage}
                                        />
                                    {:else if item.FieldCss === 'field-rte'}
                                        {#if !item.EditorReadOnly}
                                            <rt-editor data-smartdesign="1" on:demochange={onDemoChange}>
                                                <textarea
                                                    bind:value={demo}
                                                    placeholder={item.InputGuide || item.LabelText}></textarea>
                                            </rt-editor>
                                            {#if !demo && item.ValidateRequired}
                                                <p class="error">{pDisplay('ValidateRequired')}</p>
                                            {/if}
                                        {:else}
                                            <rt-editor data-smartdesign="1">
                                                <textarea
                                                    bind:value={demo}
                                                    placeholder={item.InputGuide || item.LabelText}
                                                    data-readonly="true"></textarea>
                                            </rt-editor>
                                        {/if}
                                    {:else}
                                        <TextField
                                            bind:model={demo}
                                            required={item.ValidateRequired}
                                            readOnly={item.EditorReadOnly}
                                            fieldSize={item.FieldCss}
                                            align={item.TextAlign}
                                            maxLength={item.MaxLength}
                                            placeholder={item.InputGuide || item.LabelText}
                                        />
                                    {/if}
                                {:else if viewType(paramHash.Type) === 'Date'}
                                    <div class="date-preview">
                                        <DateField
                                            bind:model={demo}
                                            required={item.ValidateRequired}
                                            readOnly={item.EditorReadOnly}
                                            fieldSize={item.FieldCss}
                                            align={item.TextAlign}
                                            step={item.DateTimeStep}
                                            diff={Number(item.DefaultInput)}
                                            format={item.EditorFormat}
                                            targetElem={detailContainer}
                                            placeholder={item.InputGuide || item.LabelText}
                                        />
                                    </div>
                                {:else if viewType(paramHash.Type) === 'Check'}
                                    <div class="checkable-preview">
                                        <CheckBox
                                            bind:model={demo}
                                            required={item.ValidateRequired}
                                            readOnly={item.EditorReadOnly}
                                            align={item.TextAlign}
                                            ><span class="required-target">{item.LabelText}</span></CheckBox
                                        >
                                    </div>
                                {:else if viewType(paramHash.Type) === 'Attachments'}
                                    <FileField
                                        bind:model={demo}
                                        required={item.ValidateRequired}
                                        readOnly={item.EditorReadOnly}
                                        align={item.TextAlign}
                                        fileDelete={item.AllowDeleteAttachments}
                                        hasError={attachmentsError}
                                        placeholder={item.InputGuide}
                                    />
                                {:else if viewType(paramHash.Type) === 'ReadOnly'}
                                    <ReadField model={item.ColumnName === 'Ver' ? 1 : 12345} align={item.TextAlign} />
                                {/if}
                            </div>
                        </div>
                    </div>
                    {#if viewType(paramHash.Type) === 'Attachments'}
                        <div class="preview-controller">
                            <div class="controller-inner">
                                <CheckBoxGroup>
                                    <CheckBox bind:model={attachmentsError}>{pDisplay('ShowErrorMessage')}</CheckBox>
                                    <CheckBox bind:model={demo}>{pDisplay('ShowAttachmentStatus')}</CheckBox>
                                </CheckBoxGroup>
                            </div>
                        </div>
                    {/if}
                </div>
            {/if}

            <footer class="modal-footer">
                <p><Button onClick={onClose} icon={'close'}>{pDisplay('Close')}</Button></p>
                <p>
                    <Button onClick={onUpdate} type="positive" icon={'save'} disabled={!Boolean(item?.LabelText)}
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
            <h1 class="title">{item?.LabelText}</h1>
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
            width: 800px;
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
        .preview {
            position: relative;
            margin-top: 40px;
            background-image: repeating-linear-gradient(
                135deg,
                var(--sd-modal-preview-bg01) 2% 4%,
                var(--sd-modal-preview-bg02) 4% 6%
            );
            background-size: auto 450px;
            border-top: 1px solid var(--sd-modal-border);
            .preview-ttl {
                padding: 0 16px;
                transform: translateY(-50%);
                span {
                    position: relative;
                    z-index: 1;
                    padding: 8px 16px;
                    font-size: 14px;
                    font-weight: bold;
                    background-color: var(--sd-modal-bg);
                    border: 1px solid var(--sd-modal-border);
                    border-radius: 4px;
                    box-shadow:
                        0 1px 0 var(--sd-modal-border),
                        1px 2px 0 var(--sd-modal-border),
                        2px 3px 0 var(--sd-modal-border),
                        3px 4px 0 var(--sd-modal-border);
                }
            }
            .preview-inner {
                display: flex;
                align-items: center;
                justify-content: center;
                min-height: 150px;
                padding: 16px;
                font-size: 12px;
            }
            .preview-field {
                display: flex;
                width: 346px;
                padding: 16px 16px 16px 8px;
                font-size: 13px;
                background-color: var(--base-bg);
                border: 1px solid var(--page-bg);
                border-radius: 4px;
                box-shadow: 0 0 5px var(--base-shadow);
                .field-label {
                    width: 120px;
                    padding: 6px 7px 6px 0;
                    text-align: right;
                }
                .field-control {
                    flex: 1;
                    min-inline-size: 200px;
                }
                &.is-wide {
                    flex: 1;
                }
                &.is-center {
                    :global(.inputCheckbox .text) {
                        text-align: center;
                    }
                }
                &.is-right {
                    .field-label {
                        text-align: right;
                    }
                    :global(.inputCheckbox .text) {
                        text-align: right;
                    }
                }
                &.is-required {
                    .required-target {
                        position: relative;
                        &::after {
                            margin-left: 3px;
                            color: #f00;
                            content: '*';
                        }
                    }
                }
                .checkable-preview {
                    margin: 8px 0;
                }
                .error {
                    padding: 4px 0 0;
                    color: var(--control-error);
                }
            }
            .preview-controller {
                display: flex;
                justify-content: flex-end;
                .controller-inner {
                    padding: 0 16px 8px;
                    color: var(--sd-modal-preview-controller-text);
                    background-color: var(--sd-modal-preview-controller-bg);
                    border-radius: 6px 0 0;
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
