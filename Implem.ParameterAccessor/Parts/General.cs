using System;
using System.ComponentModel;
namespace Implem.ParameterAccessor.Parts
{
    public class General
    {
        public string HtmlHeadKeywords { get; set; }
        public string HtmlHeadDescription { get; set; }
        public string HtmlHeadViewport { get; set; }
        public string HtmlLogoText { get; set; }
        public string HtmlPortalUrl { get; set; }
        public string HtmlApplicationBuildingGuideUrl { get; set; }
        public string HtmlUserManualUrl { get; set; }
        public string HtmlBlogUrl { get; set; }
        public string HtmlSupportUrl { get; set; }
        public string HtmlContactUrl { get; set; }
        public string HtmlAGPLUrl { get; set; }
        public string HtmlEnterPriseEditionUrl { get; set; }
        public string HtmlTrialLicenseUrl { get; set; }
        public string HtmlCasesUrl { get; set; }
        public string HtmlUrlPrefix { get; set; }
        public string RecommendUrl1 { get; set; }
        public string RecommendUrl2 { get; set; }
        public string PleasanterSource { get; set; }
        public bool DisplayLogoText { get; set; }
        public bool DisableAutoComplete { get; set; }
        public int SiteMenuHotSpan { get; set; }
        public bool AnchorTargetBlank { get; set; }
        public int LimitWarning1 { get; set; }
        public int LimitWarning2 { get; set; }
        public int LimitWarning3 { get; set; }
        public int DeleteTempOldThan { get; set; }
        public int DeleteHistoriesOldThan { get; set; }
        public int NearCompletionTimeBeforeDays { get; set; }
        public int NearCompletionTimeBeforeDaysMin { get; set; }
        public int NearCompletionTimeBeforeDaysMax { get; set; }
        public int NearCompletionTimeAfterDays { get; set; }
        public int NearCompletionTimeAfterDaysMin { get; set; }
        public int NearCompletionTimeAfterDaysMax { get; set; }
        public int GridPageSize { get; set; }
        public int GridPageSizeMin { get; set; }
        public int GridPageSizeMax { get; set; }
        public bool AllowViewReset { get; set; }
        public int ExportOutputColumnMax { get; set; }
        public string ImportEncoding { get; set; }
        public bool UpdatableImport { get; set; }
        public bool AllowStandardExport { get; set; }
        public int ImportMax { get; set; }
        public int ViewerSwitchingType { get; set; }
        public bool UseNegativeFilters { get; set; }
        public bool AllowCopy { get; set; }
        public bool AllowReferenceCopy { get; set; }
        public string CharToAddWhenCopying { get; set; }
        [DefaultValue(@"(?<!\\),")]
        public string ChoiceSplitRegexPattern { get; set; } = @"(?<!\\),";
        [DefaultValue(@"\\(,)")]
        public string ChoiceReplaceRegexPattern { get; set; } = @"\\(,)";
        [DefaultValue(@"$1")]
        public string ChoiceReplaceRegexReplacement { get; set; } = @"$1";
        public int UpdateResponseType { get; set; }
        public string SolutionBackupPath { get; set; }
        public string SolutionBackupExcludeDirectories { get; set; }
        public int SizeToUseTextArea { get; set; }
        public int CompletionCode { get; set; }
        public int CommentDisplayLimitHistories { get; set; }
        public int CommentDisplayLimit { get; set; }
        public int WorkValueHeight { get; set; }
        public int WorkValueTextTop { get; set; }
        public int ProgressRateWidth { get; set; }
        public int ProgressRateItemHeight { get; set; }
        public int ProgressRateTextTop { get; set; }
        public int CalendarBegin { get; set; }
        public int CalendarEnd { get; set; }
        public int CalendarLimit { get; set; }
        public int CalendarYLimit { get; set; }
        public int DefaultCalendarType { get; set; }
        public bool DefaultCalendarDisable { get; set; }
        public int CrosstabBegin { get; set; }
        public int CrosstabEnd { get; set; }
        public int CrosstabXLimit { get; set; }
        public int CrosstabYLimit { get; set; }
        public bool DefaultCrosstabDisable { get; set; }
        public int GanttLimit { get; set; }
        public int GanttPeriodMin { get; set; }
        public int GanttPeriodMax { get; set; }
        public bool DefaultGanttDisable { get; set; }
        public int BurnDownLimit { get; set; }
        public bool DefaultBurnDownDisable { get; set; }
        public int TimeSeriesLimit { get; set; }
        public bool DefaultTimeSeriesDisable {  get; set; }
        public int AnalyPartPeriodValueMin { get; set; }
        public int AnalyPartPeriodValueMax { get; set; }
        public bool DefaultAnalyDisable { get; set; }
        public int KambanLimit { get; set; }
        public int KambanXLimit { get; set; }
        public int KambanYLimit { get; set; }
        public int KambanMinColumns { get; set; }
        public int KambanMaxColumns { get; set; }
        public int KambanColumns { get; set; }
        public bool DefaultKambanDisable { get; set; }
        public int ImageLibPageSize { get; set; }
        public int ImageLibPageSizeMin { get; set; }
        public int ImageLibPageSizeMax { get; set; }
        public int ImageSizeRegular { get; set; }
        public int ImageSizeThumbnail { get; set; }
        public int ImageSizeIcon { get; set; }
        public int ImageSizeLogo { get; set; }
        public bool DefaultImageLibDisable { get; set; }
        public int DropDownSearchPageSize { get; set; }
        public int SwitchTargetsLimit { get; set; }
        public int SeparateMin { get; set; }
        public int SeparateMax { get; set; }
        public int FirstDayOfWeek { get; set; }
        public int FirstMonth { get; set; }
        public int DateFilterMinSpan { get; set; }
        public int DateFilterMaxSpan { get; set; }
        public DateTime MinTime { get; set; }
        public DateTime MaxTime { get; set; }
        public int DateTimeStep { get; set; }
        public bool HideCurrentTimeIcon { get; set; }
        public bool HideCurrentUserIcon { get; set; }
        public bool HideCurrentDeptIcon { get; set; }
        public bool EnableLightBox { get; set; }
        public bool EnableCodeEditor { get; set; }
        public int GroupsDepthMax { get; set; }
        public int BulkUpsertMax { get; set; }
        public bool EnableExpandLinkPath { get; set; }
        public bool BlockSiteTaskWhileRunning { get; set; }
    }
}
