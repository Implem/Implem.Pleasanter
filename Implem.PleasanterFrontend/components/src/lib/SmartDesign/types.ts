export type ColumnData = {
    AllowBulkUpdate?: boolean;
    AllowDeleteAttachments?: boolean;
    AllowImage?: boolean;
    Anchor?: boolean;
    AutoNumberingDefault?: number;
    AutoNumberingResetType?: string;
    AutoNumberingStep?: number;
    AutoPostBack?: boolean;
    BinaryStorageProvider?: string;
    CellSticky?: boolean;
    CellWidth?: number | null;
    CellWordWrap?: boolean;
    CheckFilterControlType?: number;
    ChoicesControlType?: string;
    ChoicesText?: string;
    ClientRegexValidation?: string;
    ColumnName: string;
    ControlType?: string;
    CopyByDefault?: boolean;
    DateFilterFy?: boolean;
    DateFilterHalf?: boolean;
    DateFilterMaxSpan?: number;
    DateFilterMinSpan?: number;
    DateFilterMonth?: boolean;
    DateFilterQuarter?: boolean;
    DateFilterSetMode?: number;
    DateTimeStep?: number;
    DecimalPlaces?: number;
    DefaultInput?: string;
    DefaultMaxValue?: number;
    DefaultMinValue?: number;
    EditorFormat?: 'Ymd' | 'Ymdhm' | 'Ymdhms';
    EditorReadOnly?: boolean;
    ExportFormat?: string;
    FieldCss?: '' | 'field-normal' | 'field-wide' | 'field-markdown' | 'field-rte';
    FullTextType?: number;
    GridFormat?: string;
    GridLabelText?: string;
    Id?: string | number;
    ImportKey?: boolean;
    LabelText?: string;
    LimitQuantity?: number;
    LimitSize?: number;
    LocalFolderLimitSize?: number;
    LocalFolderTotalLimitSize?: number;
    Max?: number;
    Min?: number;
    MultipleSelections?: boolean;
    NoDuplication?: boolean;
    NotDeleteExistHistory?: boolean;
    NotInsertBlankChoice?: boolean;
    Nullable?: boolean;
    NumFilterMax?: number;
    NumFilterMin?: number;
    NumFilterStep?: number;
    OpenAnchorNewTab?: boolean;
    OverwriteSameFileName?: boolean;
    RegexValidationMessage?: string;
    RoundingType?: number;
    SearchType?: string;
    ServerRegexValidation?: string;
    Step?: number;
    TextAlign?: number;
    TotalLimitSize?: number;
    Unit?: string;
    UseSearch?: boolean;
    ValidateDate?: boolean;
    ValidateEmail?: boolean;
    ValidateEqualTo?: string;
    ValidateMaxLength?: number;
    ValidateNumber?: boolean;
    ValidateRequired?: boolean;
    ViewerSwitchingType?: number;
    InputGuide?: string;
    MaxLength?: number;
    Expand?: boolean;
    AllowExpand?: boolean;
    LocalFolderLimitTotalSize?: number;
    ThumbnailLimitSize?: number;
    SectionId?: number;
    NoWrap?: boolean;
    IsNewEnable?: boolean;
};

export type SiteData = {
    ReferenceType: string;
    SiteId: number;
    Title: string;
};

export type CloneRssData = {
    Column: ColumnData;
    Hash: {
        Category: string;
        Type: string;
        Count: number;
    };
};

export type LinkData = {
    ColumnName: string;
    SiteId: number;
};

export type EditTabsData = {
    LabelText: string | undefined;
    Id: number;
};

export type SectionData = {
    AllowExpand: boolean;
    Expand: boolean;
    Id: number;
    LabelText: string;
    ColumnName?: string;
};

export type ParamHash = {
    Category: string;
    State: {
        Edit: -1 | 0 | 1;
        Filter: -1 | 0 | 1;
        Grid: -1 | 0 | 1;
    };
    Type: string;
    LinkName?: string;
};

export type SiteSettingsData = {
    Columns: ColumnData[];
    EditorColumnHash: { [key: string]: string[] };
    GridColumns: string[];
    FilterColumns: string[];
    Links: LinkData[];
    TabLatestId: number;
    Tabs: EditTabsData[];
    GeneralTabLabelText: string;
    SectionLatestId: number;
    Sections: SectionData[];
};

export type RowData = {
    Timestamp: string;
    SiteSettings: SiteSettingsData;
    DefaultColumns: string[];
    SmartDesignParamHash: { [key: string]: ParamHash };
};

export type SubmitData = {
    GridColumns: string[];
    FilterColumns: string[];
    EditorColumnHash: {
        [key: string]: string[];
    };
    Columns: ColumnData[];
    Links: LinkData[];
    GeneralTabLabelText: string;
    TabLatestId: number;
    Tabs: EditTabsData[];
    SectionLatestId: number;
    Sections: SectionData[];
    Timestamp: string;
};
