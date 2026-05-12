import { get, writable } from 'svelte/store';
import { pDisplay } from './Utility/$p';
import type {
    SiteData,
    ColumnData,
    LinkData,
    EditTabsData,
    SectionData,
    CloneRssData,
    ParamHash,
    RowData
} from './types';

export const appState = writable<boolean>(false);
export const apiLoaded = writable<boolean>(false);
export const submitState = writable<boolean>(false);
export const supportUrl = writable<string | undefined>(undefined);

export const hasAppEdited = writable<boolean>(false);
export const beforeunloadState = writable<boolean>(false);
export const confirmDisplay = writable<{ message: string; onExecute: () => void } | null>(null);

export const appMessage = writable<string | undefined>();
export const tabState = writable<'edit' | 'grid' | 'filter'>('edit');

export const editTabCurrentId = writable<string>('General');
export const editTabDraggingId = writable<string | undefined>(undefined);
export const editTabDraggingColumn = writable<string | undefined>(undefined);
export const editTabDropData = writable<{
    tabId: string;
    columnId: string;
} | null>(null);

export const getIcon = (key: string, ColumnName?: string) => {
    const icons = {
        Issues: 'view_timeline',
        Results: 'table',
        Class: 'text_fields',
        Num: 'timer_10',
        Date: 'calendar_month',
        Description: 'edit_note',
        Check: 'check',
        Attachments: 'attach_file',
        User: 'person',
        Title: 'title',
        TitleBody: 'titlecase',
        Status: 'person_celebrate',
        Time: 'schedule',
        Comments: 'chat',
        LinkTable: 'link',
        AddLinkTable: 'add_link',
        Section: 'h_mobiledata',
        LineBreak: 'subdirectory_arrow_right',
        ResultId: 'pin',
        IssueId: 'pin',
        Ver: 'pin',
        CompletionTime: 'schedule',
        WorkValue: 'timer_10',
        ProgressRate: 'timer_10',
        Locked: 'lock'
    };

    if (key === 'LinkTable') {
        const id = Number(ColumnName && ColumnName.split('_Links-')[1]);
        const linkTargetCol = get(linkTable).find(item => item.SiteId === id)?.ColumnName;
        return linkTargetCol ? icons[key] : icons['AddLinkTable'];
    } else if (icons[key]) {
        return icons[key];
    } else {
        return (ColumnName && icons[ColumnName]) || 'info_i';
    }
};

export const typeKeys = ['Class', 'Num', 'Date', 'Description', 'Check', 'Attachments'];
export const viewType = (key: string) => {
    switch (key) {
        case 'Class':
        case 'Title':
        case 'Status':
        case 'User':
            return 'Class';
        case 'Num':
        case 'WorkValue':
        case 'ProgressRate':
            return 'Num';
        case 'Date':
        case 'Time':
        case 'CompletionTime':
            return 'Date';
        case 'Description':
            return 'Description';
        case 'Check':
        case 'Locked':
            return 'Check';
        case 'Attachments':
            return 'Attachments';
        case undefined:
            return 'ReadOnly';
        default:
            return key;
    }
};

export const onAppClose = () => {
    if (!get(submitState) && get(apiLoaded)) {
        appState.set(false);
        apiLoaded.set(false);
        hasAppEdited.set(false);
        confirmDisplay.set(null);

        if (get(beforeunloadState)) {
            beforeunloadState.set(false);
            window.onbeforeunload = null;
        }
    }
};

export const setAppEdited = () => {
    if (!get(hasAppEdited)) {
        hasAppEdited.set(true);
    }
    if (!get(beforeunloadState)) {
        beforeunloadState.set(true);
        window.onbeforeunload = function (e: Event) {
            e.preventDefault();
            return pDisplay('ConfirmUnload');
        };
    }
};

export const siteData = writable<SiteData>();
export const joinedSites = writable(<SiteData[]>[]);
export const defaultColumns = writable(<string[]>[]);
export const editorColumnHash = writable(<{ [key: string]: string[] }>{});
export const gridColumns = writable(<string[]>[]);
export const filterColumns = writable(<string[]>[]);
export const columnCollection = writable(<ColumnData[]>[]);
export const columnParamHash = writable(<{ [key: string]: ParamHash }>{});
export const editTabs = writable(<{ LatestId: number; items: EditTabsData[] }>{});
export const linkTable = writable(<LinkData[]>[]);
export const sections = writable(<{ LatestId: number; items: SectionData[] }>{});
export const rowData = writable<RowData | null>(null);
export const cloneRssItems = writable(<CloneRssData[]>[
    {
        Column: {
            ColumnName: 'Class',
            LabelText: 'Class',
            TextAlign: 10,
            ValidateRequired: false,
            FieldCss: '',
            NoDuplication: false,
            EditorReadOnly: false,
            ChoicesControlType: 'DropDown',
            IsNewEnable: true
        },
        Hash: {
            Category: 'Custom',
            Type: 'Class',
            Count: 0
        }
    },
    {
        Column: {
            ColumnName: 'Num',
            ControlType: 'Normal',
            LabelText: 'Num',
            TextAlign: 10,
            ValidateRequired: false,
            FieldCss: '',
            NoDuplication: false,
            EditorReadOnly: false,
            DecimalPlaces: 0,
            RoundingType: 10,
            Min: -999999999999999,
            Max: 999999999999999,
            IsNewEnable: true
        },
        Hash: {
            Category: 'Custom',
            Type: 'Num',
            Count: 0
        }
    },
    {
        Column: {
            ColumnName: 'Date',
            LabelText: 'Date',
            TextAlign: 10,
            ValidateRequired: false,
            FieldCss: '',
            NoDuplication: false,
            EditorReadOnly: false,
            EditorFormat: 'Ymd',
            DateTimeStep: 10,
            IsNewEnable: true
        },
        Hash: {
            Category: 'Custom',
            Type: 'Date',
            Count: 0
        }
    },
    {
        Column: {
            ColumnName: 'Description',
            LabelText: 'Description',
            TextAlign: 10,
            FieldCss: 'field-markdown',
            ValidateRequired: false,
            NoDuplication: false,
            EditorReadOnly: false,
            DefaultInput: '',
            ViewerSwitchingType: 1,
            AllowImage: true,
            IsNewEnable: true
        },
        Hash: {
            Category: 'Custom',
            Type: 'Description',
            Count: 0
        }
    },
    {
        Column: {
            ColumnName: 'Check',
            DefaultInput: false,
            LabelText: 'Check',
            TextAlign: 10,
            FieldCss: '',
            ValidateRequired: false,
            EditorReadOnly: false,
            IsNewEnable: true
        },
        Hash: {
            Category: 'Custom',
            Type: 'Check',
            Count: 0
        }
    },
    {
        Column: {
            ColumnName: 'Attachments',
            LabelText: 'Attachments',
            TextAlign: 10,
            ValidateRequired: false,
            EditorReadOnly: false,
            AllowDeleteAttachments: true,
            LimitQuantity: 30,
            LimitSize: 50,
            LimitTotalSize: 1024,
            IsNewEnable: true
        },
        Hash: {
            Category: 'Custom',
            Type: 'Attachments',
            Count: 0
        }
    },
    {
        Column: {
            ColumnName: 'LineBreak',
            LabelText: 'LineBreak'
        },
        Hash: {
            Category: 'Others',
            Type: 'LineBreak',
            Count: 0
        }
    },
    {
        Column: {
            ColumnName: 'Section',
            LabelText: 'Section',
            AllowExpand: false,
            Expand: true
        },
        Hash: {
            Category: 'Others',
            Type: 'Section',
            Count: 0
        }
    }
]);
