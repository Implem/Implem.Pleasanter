<script lang="ts">
    import { onMount, onDestroy } from 'svelte';
    import {
        appState,
        tabState,
        apiLoaded,
        appMessage,
        submitState,
        rowData,
        siteData,
        joinedSites,
        defaultColumns,
        columnCollection,
        editorColumnHash,
        gridColumns,
        filterColumns,
        editTabs,
        linkTable,
        sections,
        columnParamHash,
        editTabCurrentId,
        cloneRssItems
    } from './store';
    import Body from './layout/Body.svelte';
    import { promiseApi } from './Utility/functions';
    import type { AxiosError } from 'axios';
    import type { SubmitData, SectionData } from './types';
    import { get } from 'svelte/store';

    $joinedSites = JSON.parse((document.getElementById('JoinedSites') as HTMLInputElement).value);
    const appPath = (document.getElementById('ApplicationPath') as HTMLInputElement).value;
    const siteId: number = Number((document.getElementById('SiteId') as HTMLInputElement).value);
    const currentSite = get(joinedSites).find(data => data.SiteId === siteId);
    const apiLastPath = import.meta.env.VITE_API_LAST_PATH;
    const apiSubmitPath = import.meta.env.VITE_API_SUBMIT_PATH;

    let scrollPosition: number = 0;
    $apiLoaded = false;
    $tabState = 'edit';
    $editTabCurrentId = 'General';

    const createDelayedPromise = (value: boolean, delay: number) => {
        return new Promise(resolve => {
            setTimeout(() => resolve(value), delay);
        });
    };
    if (currentSite) {
        $siteData = currentSite;
        const getApiCall = async () => {
            try {
                const [response] = await Promise.all([
                    promiseApi({
                        method: 'get',
                        url: `${appPath}items/${siteId}/${apiLastPath}`,
                        headers: {
                            'X-Requested-With': 'XMLHttpRequest'
                        }
                    }),
                    createDelayedPromise(true, 750)
                ]);
                const data = response.data;
                $rowData = data;
                $columnCollection = data.SiteSettings.Columns;
                $editorColumnHash = data.SiteSettings.EditorColumnHash;
                $defaultColumns = data.DefaultColumns;
                $gridColumns = data.SiteSettings.GridColumns;
                $filterColumns = data.SiteSettings.FilterColumns;
                $columnParamHash = data.SmartDesignParamHash;
                $linkTable = data.SiteSettings.Links;
                setTabs({
                    LatestId: data.SiteSettings.TabLatestId,
                    items: data.SiteSettings.Tabs || [],
                    generalLabel: data.SiteSettings.GeneralTabLabelText
                });
                setSectionColumns({
                    LatestId: data.SiteSettings.SectionLatestId,
                    items: data.SiteSettings.Sections || []
                });
                setBreakColumns();
                setCloneRssItems();

                //編集から除外する項目を設定
                $columnParamHash.Comments.State.Edit = -1;
                $columnParamHash.UpdatedTime.State.Edit = -1;
                $columnParamHash.Locked.Type = 'Locked';
                $columnParamHash.Locked.Category = 'Preset';

                $apiLoaded = true;
            } catch (err: unknown) {
                console.error(err);
                alert('An error occurred while connecting to the API. Please try again.');
                $appState = false;
            }
        };
        getApiCall();
    } else {
        $appState = false;
        alert('An error occurred while connecting to the API. Please try again.');
    }

    const setCloneRssItems = () => {
        cloneRssItems.update(list => {
            return list.map(data => {
                if (data.Hash.Category === 'Custom') {
                    const items = Object.keys(get(columnParamHash)).filter(key => {
                        const item = get(columnParamHash)[key];
                        return item.Category === 'Custom' && item.Type === data.Hash.Type && item.State.Edit === 0;
                    });
                    return {
                        ...data,
                        Hash: { ...data.Hash, Count: items.length }
                    };
                }
                return data;
            });
        });
    };
    const setTabs = (data: { LatestId: number; items: SectionData[]; generalLabel: string }) => {
        $editTabs = {
            LatestId: data.LatestId || 0,
            items: data.items || []
        };
        data.items.forEach(tab => {
            const tabId = `_Tab-${tab.Id}`;
            if (!get(editorColumnHash)[tabId]) {
                editorColumnHash.update(hash => ({ ...hash, [tabId]: [] }));
            }
        });
        $editTabs = {
            ...$editTabs,
            items: [{ LabelText: data.generalLabel, Id: 0 }, ...$editTabs.items]
        };
    };

    const setSectionColumns = (data: { LatestId: number; items: SectionData[] }) => {
        $sections = {
            LatestId: data.LatestId,
            items: data.items
        };
        data.items.forEach(section => {
            const colName = `_Section-${section.Id}`;
            $columnCollection = [...$columnCollection, { ...section, ColumnName: colName }];
            $columnParamHash[colName] = {
                Type: 'Section',
                Category: 'Others',
                State: {
                    Edit: 1,
                    Grid: -1,
                    Filter: -1
                }
            };
        });
    };

    const setBreakColumns = () => {
        const breakHashItem = get(cloneRssItems).find(data => data.Hash.Type === 'LineBreak');
        let breakCount = breakHashItem?.Hash.Count ?? 0;
        get(columnCollection).forEach((item, index) => {
            const editorHash = get(editorColumnHash);
            const tabID = Object.keys(editorHash).find(k => editorHash[k].includes(item.ColumnName)) ?? null;
            if (item.NoWrap && tabID && breakHashItem) {
                breakCount++;
                const colName = `_Break-${breakCount}`;
                editorColumnHash.update(hash => {
                    const colindex = hash[tabID].findIndex(name => name === item.ColumnName);
                    if (colindex <= 0) return hash;
                    return {
                        ...hash,
                        [tabID]: [...hash[tabID].slice(0, colindex), colName, ...hash[tabID].slice(colindex)]
                    };
                });
                $columnParamHash[colName] = {
                    Type: 'LineBreak',
                    Category: 'Others',
                    State: {
                        Edit: 1,
                        Grid: -1,
                        Filter: -1
                    }
                };
            }
            if (item.NoWrap) {
                $columnCollection[index].NoWrap = false;
            }
        });
        if (breakHashItem) {
            cloneRssItems.update(list =>
                list.map(data =>
                    data.Hash.Type === 'LineBreak' ? { ...data, Hash: { ...data.Hash, Count: breakCount } } : data
                )
            );
        }
    };

    const onSubmit = () => {
        $submitState = true;
        const columns = [...get(columnCollection).filter(item => !item.ColumnName.match(/^_Section-/))];
        const editorHashData: { [key: string]: string[] } = {};
        for (const [tabKey, hash] of Object.entries(get(editorColumnHash))) {
            editorHashData[tabKey] = [];
            hash.forEach((name, index) => {
                if (name.match(/_Break-/)) {
                    const targetColumn = hash[index + 1];
                    const edit = columns.find(data => data.ColumnName === targetColumn);
                    if (edit) edit.NoWrap = true;
                } else {
                    editorHashData[tabKey].push(name);
                }
            });
        }

        const tabItems = [...get(editTabs).items];
        const generalIndex = tabItems.findIndex(data => data.Id === 0);
        const [generalTab] = tabItems.splice(generalIndex, 1);

        const submitData: SubmitData = {
            GridColumns: get(gridColumns),
            FilterColumns: get(filterColumns),
            EditorColumnHash: editorHashData,
            SectionLatestId: get(sections).LatestId,
            Sections: get(sections).items,
            Columns: columns,
            GeneralTabLabelText: generalTab.LabelText!,
            TabLatestId: get(editTabs).LatestId,
            Tabs: tabItems,
            Links: get(linkTable),
            Timestamp: get(rowData)!.Timestamp
        };

        const sendApiCall = async () => {
            try {
                const [response] = await Promise.all([
                    promiseApi({
                        method: 'post',
                        url: `${appPath}items/${siteId}/${apiSubmitPath}`,
                        data: submitData,
                        headers: {
                            'X-Requested-With': 'XMLHttpRequest'
                        }
                    }),
                    createDelayedPromise(true, 500)
                ]);
                const data = response.data;
                window.onbeforeunload = null;
                if (data.Value) {
                    $submitState = false;
                    $appMessage = data.Value;
                } else if (data.Url) {
                    location.replace(data.Url);
                } else {
                    location.reload();
                }
            } catch (err: unknown) {
                const axiosErr = err as AxiosError<{ Value?: string }>;
                if (axiosErr.response?.data?.Value) {
                    $appMessage = axiosErr.response.data.Value;
                }
                $submitState = false;
            }
        };
        return sendApiCall();
    };

    onMount(() => {
        scrollPosition = window.scrollY;
        const scrollWidth = window.innerWidth - document.documentElement.clientWidth;
        const bodyStyle = window.getComputedStyle(document.body);

        document.body.style.overflow = 'hidden';
        document.body.style.position = 'fixed';
        document.body.style.top = `-${scrollPosition}px`;
        document.body.style.width = `calc(100% - ${bodyStyle.getPropertyValue('margin-left')})`;
        document.body.style.paddingRight = `${scrollWidth}px`;
    });

    onDestroy(() => {
        document.body.style.removeProperty('overflow');
        document.body.style.removeProperty('position');
        document.body.style.removeProperty('top');
        document.body.style.removeProperty('width');
        document.body.style.removeProperty('padding-right');
        window.scrollTo(0, scrollPosition);
    });
</script>

<Body {onSubmit}>
    <slot />
</Body>
