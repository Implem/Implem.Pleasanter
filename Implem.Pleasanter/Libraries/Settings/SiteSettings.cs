using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.Interfaces;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Exceptions;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Search;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.ServerScripts;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
namespace Implem.Pleasanter.Libraries.Settings
{
    [Serializable()]
    public class SiteSettings
    {
        public enum GridEditorTypes : int
        {
            None = 0,
            Grid = 10,
            Dialog = 20
        }

        public enum SearchTypes : int
        {
            FullText = 10,
            PartialMatch = 15,
            MatchInFrontOfTitle = 20,
            BroadMatchOfTitle = 30,
        }

        public enum SaveViewTypes : int
        {
            None = 0,
            Session = 1,
            User = 2
        }

        public enum RoundingTypes : int
        {
            AwayFromZero = 10,
            Ceiling = 20,
            Truncate = 30,
            Floor = 40,
            ToEven = 50
        }

        public enum TextAlignTypes : int
        {
            Left = 10,
            Center = 15,
            Right = 20
        }

        public enum CalendarTypes : int
        {
            Standard = 1,
            FullCalendar = 2
        }

        public decimal Version;
        [NonSerialized]
        public bool Migrated;
        [NonSerialized]
        public Time LockedTableTime;
        [NonSerialized]
        public User LockedTableUser;
        [NonSerialized]
        public Time LockedRecordTime;
        [NonSerialized]
        public User LockedRecordUser;
        [NonSerialized]
        public Dictionary<long, SiteSettings> Destinations;
        [NonSerialized]
        public Dictionary<long, SiteSettings> Sources;
        [NonSerialized]
        public Dictionary<long, SiteSettings> JoinedSsHash;
        [NonSerialized]
        public Dictionary<string, Dictionary<string, Choice>> ChoiceHashCache = new Dictionary<string, Dictionary<string, Choice>>();
        [NonSerialized]
        public long SiteId;
        [NonSerialized]
        public long ReferenceId;
        [NonSerialized]
        public string Title;
        [NonSerialized]
        public string Body;
        public bool? DisableSiteConditions;
        public bool? GuideAllowExpand;
        public string GuideExpand;
        [NonSerialized]
        public string GridGuide;
        [NonSerialized]
        public string EditorGuide;
        [NonSerialized]
        public string CalendarGuide;
        [NonSerialized]
        public string CrosstabGuide;
        [NonSerialized]
        public string GanttGuide;
        [NonSerialized]
        public string BurnDownGuide;
        [NonSerialized]
        public string TimeSeriesGuide;
        [NonSerialized]
        public string AnalyGuide;
        [NonSerialized]
        public string KambanGuide;
        [NonSerialized]
        public string ImageLibGuide;
        [NonSerialized]
        public long ParentId;
        [NonSerialized]
        public Sqls.TableTypes TableType = Sqls.TableTypes.Normal;
        [NonSerialized]
        public List<long> AllowedIntegratedSites;
        [NonSerialized]
        public long InheritPermission;
        [NonSerialized]
        public Permissions.Types? PermissionType;
        [NonSerialized]
        public Dictionary<long, Permissions.Types> PermissionTypeCache;
        [NonSerialized]
        public Permissions.Types? ItemPermissionType;
        [NonSerialized]
        public bool Publish;
        [NonSerialized]
        public Databases.AccessStatuses AccessStatus;
        [NonSerialized]
        public Dictionary<string, Column> ColumnHash;
        [NonSerialized]
        public Dictionary<string, Column> ColumnAccessControlCaches = new Dictionary<string, Column>();
        [NonSerialized]
        public Dictionary<string, ColumnDefinition> ColumnDefinitionHash;
        [NonSerialized]
        public List<JoinStack> JoinStacks = new List<JoinStack>();
        [NonSerialized]
        public bool Linked;
        [NonSerialized]
        public bool SetAllChoices;
        [NonSerialized]
        public DateTime ApiCountDate;
        [NonSerialized]
        public int ApiCount;
        [NonSerialized]
        public List<ServerScript> ServerScriptsAndExtended;
        public string ReferenceType;
        public decimal? NearCompletionTimeAfterDays;
        public decimal? NearCompletionTimeBeforeDays;
        public int? GridPageSize;
        public int? GridView;
        public bool? AllowViewReset;
        public GridEditorTypes? GridEditorType;
        public bool? HistoryOnGrid;
        public bool? AlwaysRequestSearchCondition;
        public bool? DisableLinkToEdit;
        public bool? OpenEditInNewTab;
        public bool? EnableExpandLinkPath;
        public int? LinkTableView;
        public int? FirstDayOfWeek;
        public int? FirstMonth;
        public List<string> GridColumns;
        public List<string> FilterColumns;
        public Dictionary<string, List<string>> EditorColumnHash;
        public string GeneralTabLabelText;
        public int? TabLatestId;
        public SettingList<Tab> Tabs;
        public int? SectionLatestId;
        public List<Section> Sections;
        public List<string> TitleColumns;
        public List<string> LinkColumns;
        public List<string> HistoryColumns;
        public List<long> MoveTargets;
        public List<Column> Columns;
        public List<Aggregation> Aggregations;
        public List<Link> Links;
        public SettingList<Summary> Summaries;
        public SettingList<FormulaSet> Formulas;
        public SettingList<Process> Processes;
        public SettingList<StatusControl> StatusControls;
        public int? ViewLatestId;
        public List<View> Views;
        public SettingList<Notification> Notifications;
        public SettingList<Reminder> Reminders;
        public string ImportEncoding;
        public bool? UpdatableImport;
        public bool? RejectNullImport;
        public string DefaultImportKey;
        public SettingList<Export> Exports;
        public bool? AllowStandardExport;
        public SettingList<Style> Styles;
        public bool? StylesAllDisabled;
        public bool? Responsive;
        public bool? DashboardPartsAsynchronousLoading;
        public SettingList<Script> Scripts;
        public bool? ScriptsAllDisabled;
        public SettingList<Html> Htmls;
        public bool? HtmlsAllDisabled;
        public SettingList<ServerScript> ServerScripts;
        public bool? ServerScriptsAllDisabled;
        public SettingList<BulkUpdateColumn> BulkUpdateColumns;
        public SettingList<RelatingColumn> RelatingColumns;
        public SettingList<DashboardPart> DashboardParts;
        public string ExtendedHeader;
        public Versions.AutoVerUpTypes? AutoVerUpType;
        public bool? AllowEditingComments;
        public bool? AllowCopy;
        public bool? AllowReferenceCopy;
        public string CharToAddWhenCopying;
        public bool? AllowSeparate;
        public bool? AllowLockTable;
        public bool? AllowRestoreHistories;
        public bool? AllowPhysicalDeleteHistories;
        public bool? HideLink;
        public bool? SwitchRecordWithAjax;
        public bool? SwitchCommandButtonsAutoPostBack;
        public bool? DeleteImageWhenDeleting;
        public bool? EnableCalendar;
        public CalendarTypes? CalendarType;
        public bool? EnableCrosstab;
        public bool? NoDisplayCrosstabGraph;
        public bool? EnableGantt;
        public bool? ShowGanttProgressRate;
        public bool? EnableBurnDown;
        public bool? EnableTimeSeries;
        public bool? EnableAnaly;
        public bool? EnableKamban;
        public bool? EnableImageLib;
        public int? ImageLibPageSize;
        public bool? UseFilterButton;
        public bool? UseFiltersArea;
        public bool? UseGridHeaderFilters;
        public bool? UseNegativeFilters;
        public bool? UseRelatingColumnsOnFilter;
        public bool? UseIncompleteFilter;
        public bool? UseOwnFilter;
        public bool? UseNearCompletionTimeFilter;
        public bool? UseDelayFilter;
        public bool? UseOverdueFilter;
        public bool? UseSearchFilter;
        public bool? OutputFormulaLogs;
        public string TitleSeparator;
        public SearchTypes? SearchType;
        public bool? FullTextIncludeBreadcrumb;
        public bool? FullTextIncludeSiteId;
        public bool? FullTextIncludeSiteTitle;
        public int? FullTextNumberOfMails;
        public SaveViewTypes? SaveViewType;
        public string AddressBook;
        public string MailToDefault;
        public string MailCcDefault;
        public string MailBccDefault;
        public List<string> IntegratedSites;
        public bool NoDisplayIfReadOnly;
        public bool NotInheritPermissionsWhenCreatingSite;
        public Dictionary<string, Permissions.Types> PermissionForCreating;
        public Dictionary<string, Permissions.Types> PermissionForUpdating;
        public List<ColumnAccessControl> CreateColumnAccessControls;
        public List<ColumnAccessControl> ReadColumnAccessControls;
        public List<ColumnAccessControl> UpdateColumnAccessControls;
        private ServerScriptModel.ServerScriptModelRow ServerScriptModelRowCache;
        // compatibility Version 1.002
        public Dictionary<string, long> LinkColumnSiteIdHash;
        // compatibility Version 1.003
        public List<string> GridColumnsOrder;
        public List<string> FilterColumnsOrder;
        public List<string> EditorColumnsOrder;
        public List<string> TitleColumnsOrder;
        public List<string> LinkColumnsOrder;
        public List<string> HistoryColumnsOrder;
        // compatibility Version 1.004
        public Dictionary<string, Formula> FormulaHash;
        // compatibility Version 1.006
        public List<Column> ColumnCollection;
        public List<Aggregation> AggregationCollection;
        public List<Link> LinkCollection;
        public List<Summary> SummaryCollection;
        // compatibility Version 1.011
        public string NewStyle;
        public string EditStyle;
        public string GridStyle;
        public string NewScript;
        public string EditScript;
        public string GridScript;
        // compatibility Version 1.015
        public bool? EditInDialog;
        // compatibility Version 1.016
        public List<string> EditorColumns;
        // compatibility Version 1.017
        public bool? ProcessOutputFormulaLogs;

        public SiteSettings()
        {
        }

        public SiteSettings(Context context, string referenceType)
        {
            ReferenceType = referenceType;
            Init(context: context);
        }

        public SiteSettings SiteSettingsOnUpdate(Context context)
        {
            var ss = new SiteSettings
            {
                SiteId = SiteId,
                InheritPermission = InheritPermission,
                PermissionType = PermissionType,
                ReferenceType = ReferenceType
            };
            ss.Init(context: context);
            return ss;
        }

        public void Init(Context context)
        {
            if (!ReferenceType.IsNullOrEmpty()
                && !Def.ColumnDefinitionCollection.Any(o => o.TableName == ReferenceType))
            {
                throw new IllegalSiteSettingsException($"ReferenceType: {ReferenceType}");
            }
            Version = SiteSettingsUtilities.Version;
            NearCompletionTimeBeforeDays = NearCompletionTimeBeforeDays ??
                Parameters.General.NearCompletionTimeBeforeDays;
            NearCompletionTimeAfterDays = NearCompletionTimeAfterDays ??
                Parameters.General.NearCompletionTimeAfterDays;
            GridPageSize = GridPageSize ?? Parameters.General.GridPageSize;
            AllowViewReset = AllowViewReset ?? Parameters.General.AllowViewReset;
            FirstDayOfWeek = FirstDayOfWeek ?? Parameters.General.FirstDayOfWeek;
            FirstMonth = FirstMonth ?? Parameters.General.FirstMonth;
            UpdateColumnDefinitionHash();
            UpdateGridColumns(context: context);
            UpdateFilterColumns(context: context);
            UpdateEditorColumns(context: context);
            GeneralTabLabelText = GeneralTabLabelText ?? Displays.General(context);
            TabLatestId = TabLatestId ?? 0;
            SectionLatestId = SectionLatestId ?? 0;
            UpdateTitleColumns(context: context);
            UpdateLinkColumns(context: context);
            UpdateHistoryColumns(context: context);
            UpdateColumns(context: context);
            UpdateColumnHash();
            if (context.TrashboxActions())
            {
                GridColumns.RemoveAll(o => o.Contains("~~"));
                FilterColumns.RemoveAll(o => o.Contains("~~"));
            }
            if (Aggregations == null) Aggregations = new List<Aggregation>();
            if (Links == null) Links = new List<Link>();
            if (Summaries == null) Summaries = new SettingList<Summary>();
            if (Formulas == null) Formulas = new SettingList<FormulaSet>();
            if (Processes == null) Processes = new SettingList<Process>();
            if (StatusControls == null) StatusControls = new SettingList<StatusControl>();
            ViewLatestId = ViewLatestId ?? 0;
            if (Views == null) Views = new List<View>();
            if (Notifications == null) Notifications = new SettingList<Notification>();
            if (Reminders == null) Reminders = new SettingList<Reminder>();
            ImportEncoding = ImportEncoding ?? Parameters.General.ImportEncoding;
            UpdatableImport = UpdatableImport ?? Parameters.General.UpdatableImport;
            if (Exports == null) Exports = new SettingList<Export>();
            AllowStandardExport = AllowStandardExport ?? Parameters.General.AllowStandardExport;
            if (Styles == null) Styles = new SettingList<Style>();
            if (Responsive == null) Responsive = Parameters.Mobile.SiteSettingsResponsive;
            if (DashboardPartsAsynchronousLoading == null) DashboardPartsAsynchronousLoading = Parameters.Dashboard.AsynchronousLoadingDefault;
            if (Scripts == null) Scripts = new SettingList<Script>();
            if (Htmls == null) Htmls = new SettingList<Html>();
            if (ServerScripts == null) ServerScripts = new SettingList<ServerScript>();
            if (BulkUpdateColumns == null) BulkUpdateColumns = new SettingList<BulkUpdateColumn>();
            if (RelatingColumns == null) RelatingColumns = new SettingList<RelatingColumn>();
            if (DashboardParts == null) DashboardParts = new SettingList<DashboardPart>();
            AutoVerUpType = AutoVerUpType ?? Versions.AutoVerUpTypes.Default;
            AllowEditingComments = AllowEditingComments ?? false;
            AllowCopy = AllowCopy ?? Parameters.General.AllowCopy;
            AllowReferenceCopy = AllowReferenceCopy ?? Parameters.General.AllowReferenceCopy;
            CharToAddWhenCopying = CharToAddWhenCopying ?? Parameters.General.CharToAddWhenCopying;
            AllowSeparate = AllowSeparate ?? false;
            AllowLockTable = AllowLockTable ?? false;
            RejectNullImport = RejectNullImport ?? false;
            AllowRestoreHistories = AllowRestoreHistories ?? true;
            AllowPhysicalDeleteHistories = AllowPhysicalDeleteHistories ?? true;
            HideLink = HideLink ?? false;
            SwitchRecordWithAjax = SwitchRecordWithAjax ?? false;
            SwitchCommandButtonsAutoPostBack = SwitchCommandButtonsAutoPostBack ?? false;
            DeleteImageWhenDeleting = DeleteImageWhenDeleting ?? true;
            EnableCalendar = EnableCalendar ?? true;
            CalendarType = CalendarType ?? (CalendarTypes)Parameters.General.DefaultCalendarType;
            EnableCrosstab = EnableCrosstab ?? true;
            NoDisplayCrosstabGraph = NoDisplayCrosstabGraph ?? false;
            EnableGantt = EnableGantt ?? true;
            ShowGanttProgressRate = ShowGanttProgressRate ?? true;
            EnableBurnDown = EnableBurnDown ?? true;
            EnableTimeSeries = EnableTimeSeries ?? true;
            EnableAnaly = EnableAnaly ?? true;
            EnableKamban = EnableKamban ?? true;
            EnableImageLib = EnableImageLib ?? true;
            ImageLibPageSize = ImageLibPageSize ?? Parameters.General.ImageLibPageSize;
            TitleSeparator = TitleSeparator ?? ")";
            UseFilterButton = UseFilterButton ?? false;
            UseFiltersArea = UseFiltersArea ?? true;
            UseGridHeaderFilters = UseGridHeaderFilters ?? false;
            UseNegativeFilters = UseNegativeFilters ?? Parameters.General.UseNegativeFilters;
            UseRelatingColumnsOnFilter = UseRelatingColumnsOnFilter ?? false;
            UseIncompleteFilter = UseIncompleteFilter ?? true;
            UseOwnFilter = UseOwnFilter ?? true;
            UseNearCompletionTimeFilter = UseNearCompletionTimeFilter ?? true;
            UseDelayFilter = UseDelayFilter ?? true;
            UseOverdueFilter = UseOverdueFilter ?? true;
            UseSearchFilter = UseSearchFilter ?? true;
            OutputFormulaLogs = OutputFormulaLogs ?? false;
            SearchType = SearchType ?? SearchTypes.PartialMatch;
            FullTextIncludeBreadcrumb = FullTextIncludeBreadcrumb ?? Parameters.Search.FullTextIncludeBreadcrumb;
            FullTextIncludeSiteId = FullTextIncludeSiteId ?? Parameters.Search.FullTextIncludeSiteId;
            FullTextIncludeSiteTitle = FullTextIncludeSiteTitle ?? Parameters.Search.FullTextIncludeSiteTitle;
            FullTextNumberOfMails = FullTextNumberOfMails ?? Parameters.Search.FullTextNumberOfMails;
            SaveViewType = SaveViewType ?? SaveViewTypes.Session;
            ProcessOutputFormulaLogs = ProcessOutputFormulaLogs ?? false;
            ServerScriptsAllDisabled = ServerScriptsAllDisabled ?? false;
            ScriptsAllDisabled = ScriptsAllDisabled ?? false;
            StylesAllDisabled = StylesAllDisabled ?? false;
            HtmlsAllDisabled = HtmlsAllDisabled ?? false;
        }

        public void SetLinkedSiteSettings(
            Context context,
            Dictionary<long, SiteSettings> joinedSsHash = null,
            bool destinations = true,
            bool sources = true,
            List<long> previously = null,
            bool? enableExpandLinkPath = null)
        {
            if ((!destinations || Destinations != null) && (!sources || Sources != null))
            {
                return;
            }
            if (joinedSsHash == null)
            {
                joinedSsHash = new Dictionary<long, SiteSettings>()
                {
                    { SiteId, this }
                };
            }
            JoinedSsHash = joinedSsHash;
            enableExpandLinkPath = enableExpandLinkPath
                ?? (Parameters.General.EnableExpandLinkPath == true
                    && EnableExpandLinkPath == true);
            if (destinations)
            {
                Destinations = SiteSettingsList(
                    context: context,
                    direction: "Destinations",
                    joinedSsHash: joinedSsHash,
                    joinStacks: JoinStacks,
                    links: Links,
                    previously: previously,
                    enableExpandLinkPath: enableExpandLinkPath);
            }
            if (sources)
            {
                Sources = SiteSettingsList(
                    context: context,
                    direction: "Sources",
                    joinedSsHash: joinedSsHash,
                    joinStacks: JoinStacks,
                    links: Links,
                    previously: previously,
                    enableExpandLinkPath: enableExpandLinkPath);
            }
            if (destinations && sources)
            {
                SetRelatingColumnsLinkedClass();
            }
        }

        private Dictionary<long, SiteSettings> SiteSettingsList(
            Context context,
            string direction,
            Dictionary<long, SiteSettings> joinedSsHash,
            List<JoinStack> joinStacks,
            List<Link> links,
            List<long> previously,
            bool? enableExpandLinkPath,
            Dictionary<long, DataSet> cache = null)
        {
            var hash = new Dictionary<long, SiteSettings>();
            var linkIds = direction == "Destinations"
                ? SiteInfo.Links(context: context)?.SourceKeyValues.Get(SiteId)
                : SiteInfo.Links(context: context)?.DestinationKeyValues.Get(SiteId);
            if (linkIds == null) return hash;
            SiteInfo.Sites(context: context)
                .Where(o => linkIds?.Any(siteId => siteId == o.Key) == true)
                .Select(o => o.Value)
                .Where(dataRow => previously?.Contains(dataRow.Long("SiteId")) != true)
                .ToList()
                .ForEach(dataRow =>
                {
                    var ss = SiteSettingsUtilities.Get(context: context, dataRow: dataRow);
                    ss.SiteId = dataRow.Long("SiteId");
                    ss.Title = dataRow.String("Title");
                    ss.Body = dataRow.String("Body");
                    ss.GridGuide = dataRow.String("GridGuide");
                    ss.EditorGuide = dataRow.String("EditorGuide");
                    ss.CalendarGuide = dataRow.String("CalendarGuide");
                    ss.CrosstabGuide = dataRow.String("CrosstabGuide");
                    ss.GanttGuide = dataRow.String("GanttGuide");
                    ss.BurnDownGuide = dataRow.String("BurnDownGuide");
                    ss.TimeSeriesGuide = dataRow.String("TimeSeriesGuide");
                    ss.AnalyGuide = dataRow.String("AnalyGuide");
                    ss.KambanGuide = dataRow.String("KambanGuide");
                    ss.ImageLibGuide = dataRow.String("ImageLibGuide");
                    ss.ReferenceType = dataRow.String("ReferenceType");
                    ss.ParentId = dataRow.Long("ParentId");
                    ss.InheritPermission = dataRow.Long("InheritPermission");
                    ss.Linked = true;
                    if (enableExpandLinkPath == true)
                    {
                        previously = (previously == null)
                           ? new List<long>()
                           : previously.Copy();
                    }
                    else
                    {
                        if (previously == null)
                        {
                            previously = new List<long>();
                        }
                    }
                    previously.Add(ss.SiteId);
                    switch (direction)
                    {
                        case "Destinations":
                            links
                                .Where(o => o.SiteId > 0)
                                .Where(o => o.SiteId == ss.SiteId)
                                .ForEach(o =>
                                    ss.JoinStacks.Add(new JoinStack(
                                        title: ss.Title,
                                        destinationId: o.SiteId,
                                        sourceId: SiteId,
                                        columnName: o.ColumnName,
                                        direction: direction,
                                        next: joinStacks?.FirstOrDefault(p =>
                                            p.DestinationId == SiteId))));
                            ss.SetLinkedSiteSettings(
                                context: context,
                                joinedSsHash: joinedSsHash,
                                destinations: true,
                                sources: false,
                                previously: previously,
                                enableExpandLinkPath: enableExpandLinkPath);
                            ss.SetPermissions(context: context, referenceId: ss.ReferenceId);
                            break;
                        case "Sources":
                            ss.Links
                                .Where(o => o.SiteId > 0)
                                .Where(o => o.SiteId == SiteId)
                                .ForEach(o =>
                                    ss.JoinStacks.Add(new JoinStack(
                                        title: ss.Title,
                                        destinationId: o.SiteId,
                                        sourceId: ss.SiteId,
                                        columnName: o.ColumnName,
                                        direction: direction,
                                        next: joinStacks?.FirstOrDefault(p =>
                                            p.SourceId == o.SiteId))));
                            ss.SetLinkedSiteSettings(
                                context: context,
                                joinedSsHash: joinedSsHash,
                                destinations: false,
                                sources: true,
                                previously: previously,
                                enableExpandLinkPath: enableExpandLinkPath);
                            ss.SetPermissions(context: context, referenceId: ss.ReferenceId);
                            break;
                    }
                    hash.Add(ss.SiteId, ss);
                    if (!joinedSsHash.ContainsKey(ss.SiteId))
                    {
                        joinedSsHash.Add(ss.SiteId, ss);
                    }
                });
            return hash;
        }

        public void SetPermissions(Context context, long referenceId)
        {
            SetPermissions(
                context: context,
                ss: this,
                referenceId: referenceId);
            Destinations?.Values.ForEach(ss =>
                SetPermissions(
                    context: context,
                    ss: ss));
            Sources?.Values.ForEach(ss =>
                SetPermissions(
                    context: context,
                    ss: ss));
        }

        private void SetPermissions(
            Context context,
            SiteSettings ss,
            long referenceId = 0)
        {
            if (context.Publish)
            {
                ss.PermissionType = Permissions.Types.Read;
                ss.ItemPermissionType = Permissions.Types.Read;
            }
            else if (context.Controller != "publishes")
            {
                if (context.PermissionHash?.ContainsKey(ss.InheritPermission) == true)
                {
                    ss.PermissionType = context.PermissionHash[ss.InheritPermission];
                }
                if (referenceId != 0 && context.PermissionHash?.ContainsKey(referenceId) == true)
                {
                    ss.ItemPermissionType = context.PermissionHash[referenceId];
                }
                if (LockedTable())
                {
                    var lockedPermissionType = Permissions.Types.Read
                        | Permissions.Types.Export
                        | Permissions.Types.SendMail;
                    ss.PermissionType &= lockedPermissionType;
                    ss.ItemPermissionType &= lockedPermissionType;
                }
            }
        }

        public bool Locked()
        {
            return LockedTable() || LockedRecord();
        }

        public bool LockedTable()
        {
            return LockedTableTime?.Value.InRange() == true
                && LockedTableUser?.Anonymous() == false;
        }

        public bool LockedRecord()
        {
            return LockedRecordTime?.Value.InRange() == true
                && LockedRecordUser?.Anonymous() == false;
        }

        public void SetLockedRecord(Context context, Time time, User user)
        {
            LockedRecordTime = time;
            LockedRecordUser = user;
            GetColumn(
                context: context,
                columnName: "Locked")
                    .EditorReadOnly = !context.HasPrivilege
                        && user.Id != context.UserId;
        }

        public bool IsSite(Context context)
        {
            return SiteId == context.Id;
        }

        public bool IsTable()
        {
            return ReferenceType == "Issues" || ReferenceType == "Results";
        }

        public bool IsSiteEditor(Context context)
        {
            if (!IsSite(context: context))
            {
                return false;
            }
            switch (context.Action)
            {
                case "edit":
                case "update":
                case "deletecomment":
                case "delete":
                case "restore":
                case "restorefromhistory":
                case "copy":
                case "histories":
                case "history":
                case "reply":
                case "getdestinations":
                case "send":
                    return true;
                default:
                    return false;
            }
        }

        public bool IsDashboardEditor(Context context)
        {
            if (ReferenceType != "Dashboards")
            {
                return false;
            }
            switch (context.Action)
            {
                case "copy":
                case "delete":
                case "deletecomment":
                case "deletehistory":
                case "edit":
                case "histories":
                case "history":
                case "permissions":
                case "restore":
                case "restorefromhistory":
                case "searchdropdown":
                case "selectsearchdropdown":
                case "setsitesettings":
                case "update":
                    return true;
                default:
                    return false;
            }
        }

        public string RecordingJson(Context context)
        {
            return RecordingData(context: context).ToJson();
        }

        public SiteSettings RecordingData(Context context)
        {
            var param = Parameters.General;
            var ss = new SiteSettings()
            {
                SiteId = SiteId,
                Version = Version,
                ReferenceType = ReferenceType
            };
            if (DisableSiteConditions == true)
            {
                ss.DisableSiteConditions = DisableSiteConditions;
            }
            if (GuideAllowExpand == true)
            {
                ss.GuideAllowExpand = GuideAllowExpand;
                if (GuideExpand == "0")
                {
                    ss.GuideExpand = GuideExpand;
                }
                else
                {
                    ss.GuideExpand = "1";
                }
            }
            if (NearCompletionTimeAfterDays != param.NearCompletionTimeAfterDays)
            {
                ss.NearCompletionTimeAfterDays = NearCompletionTimeAfterDays;
            }
            if (NearCompletionTimeBeforeDays != param.NearCompletionTimeBeforeDays)
            {
                ss.NearCompletionTimeBeforeDays = NearCompletionTimeBeforeDays;
            }
            if (GridPageSize != param.GridPageSize)
            {
                ss.GridPageSize = GridPageSize;
            }
            if (GridView != 0)
            {
                ss.GridView = GridView;
            }
            if (AllowViewReset != Parameters.General.AllowViewReset)
            {
                ss.AllowViewReset = AllowViewReset;
            }
            if (GridEditorType != GridEditorTypes.None)
            {
                ss.GridEditorType = GridEditorType;
            }
            if (HistoryOnGrid == true)
            {
                ss.HistoryOnGrid = HistoryOnGrid;
            }
            if (AlwaysRequestSearchCondition == true)
            {
                ss.AlwaysRequestSearchCondition = AlwaysRequestSearchCondition;
            }
            if (DisableLinkToEdit == true)
            {
                ss.DisableLinkToEdit = DisableLinkToEdit;
            }
            if (OpenEditInNewTab == true)
            {
                ss.OpenEditInNewTab = OpenEditInNewTab;
            }
            if (RejectNullImport == true)
            {
                ss.RejectNullImport = RejectNullImport;
            }
            if (EnableExpandLinkPath == true)
            {
                ss.EnableExpandLinkPath = EnableExpandLinkPath;
            }
            if (LinkTableView != 0)
            {
                ss.LinkTableView = LinkTableView;
            }
            if (FirstDayOfWeek != param.FirstDayOfWeek)
            {
                ss.FirstDayOfWeek = FirstDayOfWeek;
            }
            if (FirstMonth != Parameters.General.FirstMonth)
            {
                ss.FirstMonth = FirstMonth;
            }
            if (!GridColumns.SequenceEqual(DefaultGridColumns(context: context)))
            {
                ss.GridColumns = GridColumns;
            }
            if (!FilterColumns.SequenceEqual(DefaultFilterColumns()))
            {
                ss.FilterColumns = FilterColumns;
            }
            if (!(EditorColumnHash
                .Get(TabName(0))
                ?.SequenceEqual(DefaultEditorColumns(context: context)) == true)
                    || EditorColumnHash
                        ?.Any(o => TabId(o.Key) > 0 && o.Value?.Any() == true) == true)
            {
                ss.EditorColumnHash = EditorColumnHash;
            }
            if (GeneralTabLabelText != Displays.General(context)
                && !GeneralTabLabelText.IsNullOrEmpty())
            {
                ss.GeneralTabLabelText = GeneralTabLabelText;
            }
            if (TabLatestId != 0)
            {
                ss.TabLatestId = TabLatestId;
            }
            Tabs?.ForEach(tab =>
            {
                if (ss.Tabs == null)
                {
                    ss.Tabs = new SettingList<Tab>();
                }
                ss.Tabs.Add(tab.GetRecordingData(ss: this));
            });
            if (SectionLatestId != 0)
            {
                ss.SectionLatestId = SectionLatestId;
            }
            Sections?.ForEach(section =>
            {
                if (ss.Sections == null)
                {
                    ss.Sections = new List<Section>();
                }
                ss.Sections.Add(section.GetRecordingData(ss: this));
            });
            if (!TitleColumns.SequenceEqual(DefaultTitleColumns()))
            {
                ss.TitleColumns = TitleColumns;
            }
            if (AutoVerUpType != Versions.AutoVerUpTypes.Default)
            {
                ss.AutoVerUpType = AutoVerUpType;
            }
            if (AllowEditingComments == true)
            {
                ss.AllowEditingComments = AllowEditingComments;
            }
            if (AllowCopy != Parameters.General.AllowCopy)
            {
                ss.AllowCopy = AllowCopy;
            }
            if (AllowReferenceCopy != Parameters.General.AllowReferenceCopy)
            {
                ss.AllowReferenceCopy = AllowReferenceCopy;
            }
            if (CharToAddWhenCopying != Parameters.General.CharToAddWhenCopying)
            {
                ss.CharToAddWhenCopying = CharToAddWhenCopying;
            }
            if (AllowSeparate == true)
            {
                ss.AllowSeparate = AllowSeparate;
            }
            if (AllowLockTable == true)
            {
                ss.AllowLockTable = AllowLockTable;
            }
            if (AllowRestoreHistories == false)
            {
                ss.AllowRestoreHistories = AllowRestoreHistories;
            }
            if (AllowPhysicalDeleteHistories == false)
            {
                ss.AllowPhysicalDeleteHistories = AllowPhysicalDeleteHistories;
            }
            if (HideLink == true)
            {
                ss.HideLink = HideLink;
            }
            if (SwitchRecordWithAjax == true)
            {
                ss.SwitchRecordWithAjax = SwitchRecordWithAjax;
            }
            if (SwitchCommandButtonsAutoPostBack == true)
            {
                ss.SwitchCommandButtonsAutoPostBack = SwitchCommandButtonsAutoPostBack;
            }
            if (DeleteImageWhenDeleting == false)
            {
                ss.DeleteImageWhenDeleting = DeleteImageWhenDeleting;
            }
            if (EnableCalendar == false)
            {
                ss.EnableCalendar = EnableCalendar;
            }
            if (CalendarType != (CalendarTypes)Parameters.General.DefaultCalendarType)
            {
                ss.CalendarType = CalendarType;
            }
            if (EnableCrosstab == false)
            {
                ss.EnableCrosstab = EnableCrosstab;
            }
            if (NoDisplayCrosstabGraph == true)
            {
                ss.NoDisplayCrosstabGraph = NoDisplayCrosstabGraph;
            }
            if (EnableGantt == false)
            {
                ss.EnableGantt = EnableGantt;
            }
            if (ShowGanttProgressRate == false)
            {
                ss.ShowGanttProgressRate = ShowGanttProgressRate;
            }
            if (EnableBurnDown == false)
            {
                ss.EnableBurnDown = EnableBurnDown;
            }
            if (EnableTimeSeries == false)
            {
                ss.EnableTimeSeries = EnableTimeSeries;
            }
            if (EnableAnaly == false)
            {
                ss.EnableAnaly = EnableAnaly;
            }
            if (EnableKamban == false)
            {
                ss.EnableKamban = EnableKamban;
            }
            if (EnableImageLib == false)
            {
                ss.EnableImageLib = EnableImageLib;
            }
            if (UseFilterButton == true)
            {
                ss.UseFilterButton = UseFilterButton;
            }
            if (UseFiltersArea == false)
            {
                ss.UseFiltersArea = UseFiltersArea;
            }
            if (UseGridHeaderFilters == true)
            {
                ss.UseGridHeaderFilters = UseGridHeaderFilters;
            }
            if (UseNegativeFilters != Parameters.General.UseNegativeFilters)
            {
                ss.UseNegativeFilters = UseNegativeFilters;
            }
            if (UseRelatingColumnsOnFilter == true)
            {
                ss.UseRelatingColumnsOnFilter = UseRelatingColumnsOnFilter;
            }
            if (UseIncompleteFilter == false)
            {
                ss.UseIncompleteFilter = UseIncompleteFilter;
            }
            if (UseOwnFilter == false)
            {
                ss.UseOwnFilter = UseOwnFilter;
            }
            if (UseNearCompletionTimeFilter == false)
            {
                ss.UseNearCompletionTimeFilter = UseNearCompletionTimeFilter;
            }
            if (UseDelayFilter == false)
            {
                ss.UseDelayFilter = UseDelayFilter;
            }
            if (UseOverdueFilter == false)
            {
                ss.UseOverdueFilter = UseOverdueFilter;
            }
            if (UseSearchFilter == false)
            {
                ss.UseSearchFilter = UseSearchFilter;
            }
            if (OutputFormulaLogs == true)
            {
                ss.OutputFormulaLogs = OutputFormulaLogs;
            }
            if (ImageLibPageSize != Parameters.General.ImageLibPageSize)
            {
                ss.ImageLibPageSize = ImageLibPageSize;
            }
            if (TitleSeparator != ")")
            {
                ss.TitleSeparator = TitleSeparator;
            }
            if (SearchType != SearchTypes.PartialMatch)
            {
                ss.SearchType = SearchType;
            }
            if (FullTextIncludeBreadcrumb != Parameters.Search.FullTextIncludeBreadcrumb)
            {
                ss.FullTextIncludeBreadcrumb = FullTextIncludeBreadcrumb;
            }
            if (FullTextIncludeSiteId != Parameters.Search.FullTextIncludeSiteId)
            {
                ss.FullTextIncludeSiteId = FullTextIncludeSiteId;
            }
            if (FullTextIncludeSiteTitle != Parameters.Search.FullTextIncludeSiteTitle)
            {
                ss.FullTextIncludeSiteTitle = FullTextIncludeSiteTitle;
            }
            if (FullTextNumberOfMails != Parameters.Search.FullTextNumberOfMails)
            {
                ss.FullTextNumberOfMails = FullTextNumberOfMails;
            }
            if (SaveViewType != SaveViewTypes.Session)
            {
                ss.SaveViewType = SaveViewType;
            }
            if (!LinkColumns.SequenceEqual(DefaultLinkColumns(context: context)))
            {
                ss.LinkColumns = LinkColumns;
            }
            if (!HistoryColumns.SequenceEqual(DefaultHistoryColumns(context: context)))
            {
                ss.HistoryColumns = HistoryColumns;
            }
            if (MoveTargets?.Any() == true)
            {
                ss.MoveTargets = MoveTargets;
            }
            if (ProcessOutputFormulaLogs == true)
            {
                ss.ProcessOutputFormulaLogs = ProcessOutputFormulaLogs;
            }
            if (ServerScriptsAllDisabled == true)
            {
                ss.ServerScriptsAllDisabled = ServerScriptsAllDisabled;
            }
            if (ScriptsAllDisabled == true)
            {
                ss.ScriptsAllDisabled = ScriptsAllDisabled;
            }
            if (StylesAllDisabled == true)
            {
                ss.StylesAllDisabled = StylesAllDisabled;
            }
            if (HtmlsAllDisabled == true)
            {
                ss.HtmlsAllDisabled = HtmlsAllDisabled;
            }
            Aggregations?.ForEach(aggregations =>
            {
                if (ss.Aggregations == null)
                {
                    ss.Aggregations = new List<Aggregation>();
                }
                ss.Aggregations.Add(aggregations.GetRecordingData());
            });
            Links?.ForEach(link =>
            {
                if (ss.Links == null)
                {
                    ss.Links = new List<Link>();
                }
                ss.Links.Add(link.GetRecordingData(
                    context: context,
                    ss: this));
            });
            Summaries?.ForEach(summaries =>
            {
                if (ss.Summaries == null)
                {
                    ss.Summaries = new SettingList<Summary>();
                }
                ss.Summaries.Add(summaries.GetRecordingData());
            });
            Formulas?.ForEach(formulas =>
            {
                if (ss.Formulas == null)
                {
                    ss.Formulas = new SettingList<FormulaSet>();
                }
                ss.Formulas.Add(formulas.GetRecordingData());
            });
            Processes?.ForEach(process =>
            {
                if (ss.Processes == null)
                {
                    ss.Processes = new SettingList<Process>();
                }
                ss.Processes.Add(process.GetRecordingData(
                    context: context,
                    ss: this));
            });
            StatusControls?.ForEach(statusControl =>
            {
                if (ss.StatusControls == null)
                {
                    ss.StatusControls = new SettingList<StatusControl>();
                }
                ss.StatusControls.Add(statusControl.GetRecordingData(
                    context: context,
                    ss: this));
            });
            if (ViewLatestId != 0)
            {
                ss.ViewLatestId = ViewLatestId;
            }
            Views?.ForEach(view =>
            {
                if (ss.Views == null)
                {
                    ss.Views = new List<View>();
                }
                ss.Views.Add(view.GetRecordingData(
                    context: context,
                    ss: this));
            });
            Notifications?.ForEach(notification =>
            {
                if (ss.Notifications == null)
                {
                    ss.Notifications = new SettingList<Notification>();
                }
                ss.Notifications.Add(notification.GetRecordingData(
                    context: context,
                    ss: ss));
            });
            Reminders?.ForEach(reminder =>
            {
                if (ss.Reminders == null)
                {
                    ss.Reminders = new SettingList<Reminder>();
                }
                ss.Reminders.Add(reminder.GetRecordingData(context: context));
            });
            if (ImportEncoding != Parameters.General.ImportEncoding)
            {
                ss.ImportEncoding = ImportEncoding;
            }
            if (UpdatableImport != Parameters.General.UpdatableImport)
            {
                ss.UpdatableImport = UpdatableImport;
            }
            if (!DefaultImportKey.IsNullOrEmpty())
            {
                ss.DefaultImportKey = DefaultImportKey;
            }
            Exports?.ForEach(exportSetting =>
            {
                if (ss.Exports == null)
                {
                    ss.Exports = new SettingList<Export>();
                }
                ss.Exports.Add(exportSetting.GetRecordingData());
            });
            if (AllowStandardExport != Parameters.General.AllowStandardExport)
            {
                ss.AllowStandardExport = AllowStandardExport;
            }
            Styles?.ForEach(style =>
            {
                if (ss.Styles == null)
                {
                    ss.Styles = new SettingList<Style>();
                }
                ss.Styles.Add(style.GetRecordingData());
            });
            if (Responsive != Parameters.Mobile.SiteSettingsResponsive)
            {
                ss.Responsive = Responsive;
            }
            if (DashboardPartsAsynchronousLoading != Parameters.Dashboard.AsynchronousLoadingDefault)
            {
                ss.DashboardPartsAsynchronousLoading = DashboardPartsAsynchronousLoading;
            }
            Scripts?.ForEach(script =>
            {
                if (ss.Scripts == null)
                {
                    ss.Scripts = new SettingList<Script>();
                }
                ss.Scripts.Add(script.GetRecordingData());
            });
            Htmls?.ForEach(html =>
            {
                if (ss.Htmls == null)
                {
                    ss.Htmls = new SettingList<Html>();
                }
                ss.Htmls.Add(html.GetRecordingData());
            });
            ServerScripts?.ForEach(script =>
            {
                if (ss.ServerScripts == null)
                {
                    ss.ServerScripts = new SettingList<ServerScript>();
                }
                ss.ServerScripts.Add(script.GetRecordingData());
            });
            BulkUpdateColumns?.ForEach(bulkUpdateColumn =>
            {
                if (ss.BulkUpdateColumns == null)
                {
                    ss.BulkUpdateColumns = new SettingList<BulkUpdateColumn>();
                }
                ss.BulkUpdateColumns.Add(bulkUpdateColumn.GetRecordingData());
            });
            RelatingColumns?.ForEach(relatingColumn =>
            {
                if (ss.RelatingColumns == null)
                {
                    ss.RelatingColumns = new SettingList<RelatingColumn>();
                }
                ss.RelatingColumns.Add(relatingColumn.GetRecordingData());
            });
            DashboardParts?.ForEach(dashboards =>
            {
                if (ss.DashboardParts == null)
                {
                    ss.DashboardParts = new SettingList<DashboardPart>();
                }
                ss.DashboardParts.Add(dashboards.GetRecordingData(context: context));
            });
            if (!ExtendedHeader.IsNullOrEmpty())
            {
                ss.ExtendedHeader = ExtendedHeader;
            }
            if (!AddressBook.IsNullOrEmpty())
            {
                ss.AddressBook = AddressBook;
            }
            if (!MailToDefault.IsNullOrEmpty())
            {
                ss.MailToDefault = MailToDefault;
            }
            if (!MailCcDefault.IsNullOrEmpty())
            {
                ss.MailCcDefault = MailCcDefault;
            }
            if (!MailBccDefault.IsNullOrEmpty())
            {
                ss.MailBccDefault = MailBccDefault;
            }
            if (IntegratedSites?.Any() == true)
            {
                ss.IntegratedSites = IntegratedSites;
            }
            if (NoDisplayIfReadOnly == true)
            {
                ss.NoDisplayIfReadOnly = NoDisplayIfReadOnly;
            }
            if (NotInheritPermissionsWhenCreatingSite == true)
            {
                ss.NotInheritPermissionsWhenCreatingSite = NotInheritPermissionsWhenCreatingSite;
            }
            if (ProcessOutputFormulaLogs == true)
            {
                ss.ProcessOutputFormulaLogs = ProcessOutputFormulaLogs;
            }
            if (ServerScriptsAllDisabled == true)
            {
                ss.ServerScriptsAllDisabled = ServerScriptsAllDisabled;
            }
            if (ScriptsAllDisabled == true)
            {
                ss.ScriptsAllDisabled = ScriptsAllDisabled;
            }
            if (StylesAllDisabled == true)
            {
                ss.StylesAllDisabled = StylesAllDisabled;
            }
            if (HtmlsAllDisabled == true)
            {
                ss.HtmlsAllDisabled = HtmlsAllDisabled;
            }
            PermissionForCreating?.Where(o => o.Value > 0).ForEach(data =>
            {
                if (ss.PermissionForCreating == null)
                {
                    ss.PermissionForCreating = new Dictionary<string, Permissions.Types>();
                }
                ss.PermissionForCreating.Add(data.Key, data.Value);
            });
            PermissionForUpdating?.Where(o => o.Value > 0).ForEach(data =>
            {
                if (ss.PermissionForUpdating == null)
                {
                    ss.PermissionForUpdating = new Dictionary<string, Permissions.Types>();
                }
                ss.PermissionForUpdating.Add(data.Key, data.Value);
            });
            CreateColumnAccessControls?
                .Where(o => !o.IsDefault(this, "Create"))
                .ForEach(columnAccessControl =>
                {
                    if (ss.CreateColumnAccessControls == null)
                    {
                        ss.CreateColumnAccessControls = new List<ColumnAccessControl>();
                    }
                    ss.CreateColumnAccessControls.Add(columnAccessControl.RecordingData());
                });
            ReadColumnAccessControls?
                .Where(o => !o.IsDefault(this, "Read"))
                .ForEach(columnAccessControl =>
                {
                    if (ss.ReadColumnAccessControls == null)
                    {
                        ss.ReadColumnAccessControls = new List<ColumnAccessControl>();
                    }
                    ss.ReadColumnAccessControls.Add(columnAccessControl.RecordingData());
                });
            UpdateColumnAccessControls?
                .Where(o => !o.IsDefault(this, "Update"))
                .ForEach(columnAccessControl =>
                {
                    if (ss.UpdateColumnAccessControls == null)
                    {
                        ss.UpdateColumnAccessControls = new List<ColumnAccessControl>();
                    }
                    ss.UpdateColumnAccessControls.Add(columnAccessControl.RecordingData());
                });
            Columns?.Where(o => !o.Joined).ForEach(column =>
            {
                var newColumn = new Column() { ColumnName = column.ColumnName };
                var enabled = false;
                var columnDefinition = (column.Joined
                    ? JoinedSsHash.Get(column.SiteId)?.ColumnDefinitionHash
                    : ColumnDefinitionHash)
                        .Get(column.Name);
                if (columnDefinition != null)
                {
                    var labelText = column.LabelText;
                    if (column.LabelText != Displays.Get(
                        context: context,
                        id: columnDefinition.Id))
                    {
                        enabled = true;
                        newColumn.LabelText = column.LabelText;
                    }
                    if (column.GridLabelText != labelText)
                    {
                        enabled = true;
                        newColumn.GridLabelText = column.GridLabelText;
                    }
                    if (column.Description?.Trim().IsNullOrEmpty() == false)
                    {
                        enabled = true;
                        newColumn.Description = column.Description;
                    }
                    if (column.InputGuide?.Trim().IsNullOrEmpty() == false)
                    {
                        enabled = true;
                        newColumn.InputGuide = column.InputGuide;
                    }
                    if (column.ChoicesText.Replace("\r\n", "\n") != columnDefinition.ChoicesText.Replace("\r\n", "\n"))
                    {
                        enabled = true;
                        newColumn.ChoicesText = column.ChoicesText;
                    }
                    if (column.UseSearch != columnDefinition.UseSearch)
                    {
                        enabled = true;
                        newColumn.UseSearch = column.UseSearch;
                    }
                    if (column.MultipleSelections == true)
                    {
                        enabled = true;
                        newColumn.MultipleSelections = column.MultipleSelections;
                    }
                    if (column.NotInsertBlankChoice == true)
                    {
                        enabled = true;
                        newColumn.NotInsertBlankChoice = column.NotInsertBlankChoice;
                    }
                    if (column.DefaultInput != columnDefinition.DefaultInput)
                    {
                        enabled = true;
                        newColumn.DefaultInput = column.DefaultInput;
                    }
                    if (column.ImportKey != columnDefinition.ImportKey)
                    {
                        enabled = true;
                        newColumn.ImportKey = column.ImportKey;
                    }
                    if (column.Anchor == true)
                    {
                        enabled = true;
                        newColumn.Anchor = column.Anchor;
                        if (column.OpenAnchorNewTab == true)
                        {
                            newColumn.OpenAnchorNewTab = column.OpenAnchorNewTab;
                        }
                        if (!column.AnchorFormat.IsNullOrEmpty())
                        {
                            newColumn.AnchorFormat = column.AnchorFormat;
                        }
                    }
                    if (column.MaxLength.ToDecimal() > 0)
                    {
                        enabled = true;
                        newColumn.MaxLength = column.MaxLength;
                    }
                    if (column.GridFormat != columnDefinition.GridFormat)
                    {
                        enabled = true;
                        newColumn.GridFormat = column.GridFormat;
                    }
                    if (column.EditorFormat != columnDefinition.EditorFormat)
                    {
                        enabled = true;
                        newColumn.EditorFormat = column.EditorFormat;
                    }
                    if (column.ExportFormat != columnDefinition.ExportFormat)
                    {
                        enabled = true;
                        newColumn.ExportFormat = column.ExportFormat;
                    }
                    if (column.ControlType != columnDefinition.ControlType)
                    {
                        enabled = true;
                        newColumn.ControlType = column.ControlType;
                    }
                    if (column.ChoicesControlType == HtmlFields.ControlTypes.Radio.ToString())
                    {
                        enabled = true;
                        newColumn.ChoicesControlType = column.ChoicesControlType;
                    }
                    if (column.Format?.Trim().IsNullOrEmpty() == false)
                    {
                        enabled = true;
                        newColumn.Format = column.Format;
                    }
                    if (column.NoWrap == true)
                    {
                        enabled = true;
                        newColumn.NoWrap = column.NoWrap;
                    }
                    if (column.Hide == true)
                    {
                        enabled = true;
                        newColumn.Hide = column.Hide;
                    }
                    if (column.AutoNumberingFormat?.Trim().IsNullOrEmpty() == false)
                    {
                        enabled = true;
                        newColumn.AutoNumberingFormat = column.AutoNumberingFormat;
                    }
                    if (column.AutoNumberingResetType != Column.AutoNumberingResetTypes.None)
                    {
                        enabled = true;
                        newColumn.AutoNumberingResetType = column.AutoNumberingResetType;
                    }
                    if (column.AutoNumberingDefault != 1)
                    {
                        enabled = true;
                        newColumn.AutoNumberingDefault = column.AutoNumberingDefault;
                    }
                    if (column.AutoNumberingStep != 1)
                    {
                        enabled = true;
                        newColumn.AutoNumberingStep = column.AutoNumberingStep;
                    }
                    if (column.ExtendedCellCss?.Trim().IsNullOrEmpty() == false)
                    {
                        enabled = true;
                        newColumn.ExtendedCellCss = column.ExtendedCellCss;
                    }
                    if (column.ExtendedFieldCss?.Trim().IsNullOrEmpty() == false)
                    {
                        enabled = true;
                        newColumn.ExtendedFieldCss = column.ExtendedFieldCss;
                    }
                    if (column.ExtendedControlCss?.Trim().IsNullOrEmpty() == false)
                    {
                        enabled = true;
                        newColumn.ExtendedControlCss = column.ExtendedControlCss;
                    }
                    if (column.Section?.Trim().IsNullOrEmpty() == false)
                    {
                        enabled = true;
                        newColumn.Section = column.Section;
                    }
                    if (column.GridDesign != null &&
                        column.GridDesign != LabelTextBracket(column))
                    {
                        enabled = true;
                        newColumn.GridDesign = column.GridDesign;
                    }
                    if (column.ValidateRequired != columnDefinition.ValidateRequired)
                    {
                        enabled = true;
                        newColumn.ValidateRequired = column.ValidateRequired;
                    }
                    if (column.ValidateNumber != columnDefinition.ValidateNumber)
                    {
                        enabled = true;
                        newColumn.ValidateNumber = column.ValidateNumber;
                    }
                    if (column.ValidateDate != columnDefinition.ValidateDate)
                    {
                        enabled = true;
                        newColumn.ValidateDate = column.ValidateDate;
                    }
                    if (column.ValidateEmail != columnDefinition.ValidateEmail)
                    {
                        enabled = true;
                        newColumn.ValidateEmail = column.ValidateEmail;
                    }
                    if (column.ValidateEqualTo != columnDefinition.ValidateEqualTo)
                    {
                        enabled = true;
                        newColumn.ValidateEqualTo = column.ValidateEqualTo;
                    }
                    if (column.ValidateMaxLength != columnDefinition.MaxLength)
                    {
                        enabled = true;
                        newColumn.ValidateMaxLength = column.ValidateMaxLength;
                    }
                    if (column.ClientRegexValidation != columnDefinition.ClientRegexValidation)
                    {
                        enabled = true;
                        newColumn.ClientRegexValidation = column.ClientRegexValidation;
                    }
                    if (column.ServerRegexValidation != columnDefinition.ServerRegexValidation)
                    {
                        enabled = true;
                        newColumn.ServerRegexValidation = column.ServerRegexValidation;
                    }
                    if (column.RegexValidationMessage != columnDefinition.RegexValidationMessage)
                    {
                        enabled = true;
                        newColumn.RegexValidationMessage = column.RegexValidationMessage;
                    }
                    if (column.ExtendedHtmlBeforeField?.Trim().IsNullOrEmpty() == false)
                    {
                        enabled = true;
                        newColumn.ExtendedHtmlBeforeField = column.ExtendedHtmlBeforeField;
                    }
                    if (column.ExtendedHtmlBeforeLabel?.Trim().IsNullOrEmpty() == false)
                    {
                        enabled = true;
                        newColumn.ExtendedHtmlBeforeLabel = column.ExtendedHtmlBeforeLabel;
                    }
                    if (column.ExtendedHtmlBetweenLabelAndControl?.Trim().IsNullOrEmpty() == false)
                    {
                        enabled = true;
                        newColumn.ExtendedHtmlBetweenLabelAndControl = column.ExtendedHtmlBetweenLabelAndControl;
                    }
                    if (column.ExtendedHtmlAfterControl?.Trim().IsNullOrEmpty() == false)
                    {
                        enabled = true;
                        newColumn.ExtendedHtmlAfterControl = column.ExtendedHtmlAfterControl;
                    }
                    if (column.ExtendedHtmlAfterField?.Trim().IsNullOrEmpty() == false)
                    {
                        enabled = true;
                        newColumn.ExtendedHtmlAfterField = column.ExtendedHtmlAfterField;
                    }
                    if (column.Nullable == true)
                    {
                        enabled = true;
                        newColumn.Nullable = column.Nullable;
                    }
                    if (column.Unit != columnDefinition.Unit)
                    {
                        enabled = true;
                        newColumn.Unit = column.Unit;
                    }
                    if (column.DecimalPlaces != columnDefinition.DecimalPlaces)
                    {
                        enabled = true;
                        newColumn.DecimalPlaces = column.DecimalPlaces;
                    }
                    if (column.RoundingType != RoundingTypes.AwayFromZero)
                    {
                        enabled = true;
                        newColumn.RoundingType = column.RoundingType;
                    }
                    if (column.Min != DefaultMin(columnDefinition))
                    {
                        enabled = true;
                        newColumn.Min = column.Min;
                    }
                    if (column.Max != DefaultMax(columnDefinition))
                    {
                        enabled = true;
                        newColumn.Max = column.Max;
                    }
                    if (column.Step != DefaultStep(columnDefinition))
                    {
                        enabled = true;
                        newColumn.Step = column.Step;
                    }
                    if (column.NoDuplication == true)
                    {
                        enabled = true;
                        newColumn.NoDuplication = column.NoDuplication;
                    }
                    if (!column.MessageWhenDuplicated.IsNullOrEmpty())
                    {
                        enabled = true;
                        newColumn.MessageWhenDuplicated = column.MessageWhenDuplicated;
                    }
                    if (column.CopyByDefault != columnDefinition.CopyByDefault)
                    {
                        enabled = true;
                        newColumn.CopyByDefault = column.CopyByDefault;
                    }
                    if (column.EditorReadOnly != columnDefinition.EditorReadOnly)
                    {
                        enabled = true;
                        newColumn.EditorReadOnly = column.EditorReadOnly;
                    }
                    if (column.AutoPostBack == true)
                    {
                        enabled = true;
                        newColumn.AutoPostBack = column.AutoPostBack;
                    }
                    if (!column.ColumnsReturnedWhenAutomaticPostback.IsNullOrEmpty())
                    {
                        enabled = true;
                        newColumn.ColumnsReturnedWhenAutomaticPostback = column.ColumnsReturnedWhenAutomaticPostback;
                    }
                    if (column.AllowDeleteAttachments != true)
                    {
                        enabled = true;
                        newColumn.AllowDeleteAttachments = column.AllowDeleteAttachments;
                    }
                    if (column.NotDeleteExistHistory == true)
                    {
                        enabled = true;
                        newColumn.NotDeleteExistHistory = column.NotDeleteExistHistory;
                    }
                    if (column.AllowImage != true)
                    {
                        enabled = true;
                        newColumn.AllowImage = column.AllowImage;
                    }
                    if (column.AllowBulkUpdate == true)
                    {
                        enabled = true;
                        newColumn.AllowBulkUpdate = column.AllowBulkUpdate;
                    }
                    if (column.FieldCss != columnDefinition.FieldCss
                        && !(column.FieldCss == "field-normal" && columnDefinition.FieldCss.IsNullOrEmpty()))
                    {
                        enabled = true;
                        newColumn.FieldCss = column.FieldCss;
                    }
                    if (column.ViewerSwitchingType != (Column.ViewerSwitchingTypes)Parameters.General.ViewerSwitchingType)
                    {
                        enabled = true;
                        newColumn.ViewerSwitchingType = column.ViewerSwitchingType;
                    }
                    if (column.TextAlign != TextAlignTypes.Left)
                    {
                        enabled = true;
                        newColumn.TextAlign = column.TextAlign;
                    }
                    if (column.Link == true)
                    {
                        enabled = true;
                        newColumn.Link = column.Link;
                    }
                    if (column.CheckFilterControlType != (ColumnUtilities.CheckFilterControlTypes)columnDefinition.CheckFilterControlType)
                    {
                        enabled = true;
                        newColumn.CheckFilterControlType = column.CheckFilterControlType;
                    }
                    if (column.NumFilterMin != columnDefinition.NumFilterMin)
                    {
                        enabled = true;
                        newColumn.NumFilterMin = column.NumFilterMin;
                    }
                    if (column.NumFilterMax != columnDefinition.NumFilterMax)
                    {
                        enabled = true;
                        newColumn.NumFilterMax = column.NumFilterMax;
                    }
                    if (column.NumFilterStep != columnDefinition.NumFilterStep)
                    {
                        enabled = true;
                        newColumn.NumFilterStep = column.NumFilterStep;
                    }
                    if (column.DateFilterSetMode != ColumnUtilities.GetDateFilterSetMode(columnDefinition: columnDefinition))
                    {
                        enabled = true;
                        newColumn.DateFilterSetMode = column.DateFilterSetMode;
                    }
                    if (column.SearchType != column.SearchTypeDefault())
                    {
                        enabled = true;
                        newColumn.SearchType = column.SearchType;
                    }
                    if (column.FullTextType != (Column.FullTextTypes)columnDefinition.FullTextType)
                    {
                        enabled = true;
                        newColumn.FullTextType = column.FullTextType;
                    }
                    if (column.DateFilterMinSpan != Parameters.General.DateFilterMinSpan)
                    {
                        enabled = true;
                        newColumn.DateFilterMinSpan = column.DateFilterMinSpan;
                    }
                    if (column.DateFilterMaxSpan != Parameters.General.DateFilterMaxSpan)
                    {
                        enabled = true;
                        newColumn.DateFilterMaxSpan = column.DateFilterMaxSpan;
                    }
                    if (column.DateFilterFy == false)
                    {
                        enabled = true;
                        newColumn.DateFilterFy = column.DateFilterFy;
                    }
                    if (column.DateFilterHalf == false)
                    {
                        enabled = true;
                        newColumn.DateFilterHalf = column.DateFilterHalf;
                    }
                    if (column.DateFilterQuarter == false)
                    {
                        enabled = true;
                        newColumn.DateFilterQuarter = column.DateFilterQuarter;
                    }
                    if (column.DateFilterMonth == false)
                    {
                        enabled = true;
                        newColumn.DateFilterMonth = column.DateFilterMonth;
                    }
                    if (column.OverwriteSameFileName == true)
                    {
                        enabled = true;
                        newColumn.OverwriteSameFileName = column.OverwriteSameFileName;
                    }
                    if (column.LimitQuantity != Parameters.BinaryStorage.LimitQuantity)
                    {
                        enabled = true;
                        newColumn.LimitQuantity = column.LimitQuantity;
                    }
                    if (column.LimitSize != Parameters.BinaryStorage.LimitSize)
                    {
                        enabled = true;
                        newColumn.LimitSize = column.LimitSize;
                    }
                    if (column.TotalLimitSize != Parameters.BinaryStorage.LimitTotalSize)
                    {
                        enabled = true;
                        newColumn.TotalLimitSize = column.TotalLimitSize;
                    }
                    if (column.LocalFolderLimitSize != Parameters.BinaryStorage.LocalFolderLimitSize)
                    {
                        enabled = true;
                        newColumn.LocalFolderLimitSize = column.LocalFolderLimitSize;
                    }
                    if (column.LocalFolderTotalLimitSize != Parameters.BinaryStorage.LocalFolderLimitTotalSize)
                    {
                        enabled = true;
                        newColumn.LocalFolderTotalLimitSize = column.LocalFolderTotalLimitSize;
                    }
                    if (column.ControlType == "Attachments"
                        && !string.IsNullOrEmpty(column.BinaryStorageProvider)
                        && column.BinaryStorageProvider != Parameters.BinaryStorage.DefaultBinaryStorageProvider)
                    {
                        enabled = true;
                        newColumn.BinaryStorageProvider = column.BinaryStorageProvider;
                    }
                    if (column.DateTimeStep != Parameters.General.DateTimeStep)
                    {
                        enabled = true;
                        newColumn.DateTimeStep = column.DateTimeStep;
                    }
                    if (column.ThumbnailLimitSize != null
                        && column.ThumbnailLimitSize != Parameters.BinaryStorage.ThumbnailLimitSize
                        && column.ThumbnailLimitSize >= Parameters.BinaryStorage.ThumbnailMinSize
                        && column.ThumbnailLimitSize <= Parameters.BinaryStorage.ThumbnailMaxSize)
                    {
                        enabled = true;
                        newColumn.ThumbnailLimitSize = column.ThumbnailLimitSize;
                    }
                }
                if (enabled)
                {
                    if (ss.Columns == null)
                    {
                        ss.Columns = new List<Column>();
                    }
                    ss.Columns.Add(newColumn);
                }
            });
            return ss;
        }

        private void UpdateColumnDefinitionHash()
        {
            ColumnDefinitionHash = GetColumnDefinitionHash(
                ReferenceType == "Dashboards"
                    ? "Issues"
                    : ReferenceType);
        }

        private static Dictionary<string, ColumnDefinition> GetColumnDefinitionHash(
            string referenceType)
        {
            return Def.ColumnDefinitionCollection
                .Where(o => o.TableName == referenceType)
                .ToDictionary(o => o.ColumnName, o => o);
        }

        private void UpdateGridColumns(Context context)
        {
            if (GridColumns == null)
            {
                GridColumns = DefaultGridColumns(context: context);
            }
        }

        private List<string> DefaultGridColumns(Context context)
        {
            return ColumnDefinitionHash.GridDefinitions(context: context, enableOnly: true)
                .Select(o => o.ColumnName)
                .ToList();
        }

        private void UpdateFilterColumns(Context context)
        {
            if (FilterColumns == null)
            {
                FilterColumns = DefaultFilterColumns();
            }
        }

        private List<string> DefaultFilterColumns()
        {
            return ColumnDefinitionHash.FilterDefinitions(enableOnly: true)
                .Select(o => o.ColumnName)
                .ToList();
        }

        private void UpdateEditorColumns(Context context)
        {
            if (EditorColumnHash == null || EditorColumnHash.All(tab => tab.Value?.Any() != true))
            {
                AddOrUpdateEditorColumnHash(
                    editorColumnsAll: DefaultEditorColumns(context: context));
            }
            else
            {
                EditorColumnHash.ForEach(tab => tab.Value?.RemoveAll(o => !EditorColumn(
                    columnDefinition: ColumnDefinitionHash.Get(o)) && !o.StartsWith("_")));
            }
        }

        private List<string> DefaultEditorColumns(Context context)
        {
            return ColumnDefinitionHash.EditorDefinitions(context: context, enableOnly: true)
                .Select(o => o.ColumnName)
                .ToList();
        }

        public List<string> GetDefaultColumns(Context context)
        {
            List<string> defaultColumns = DefaultGridColumns(context)
                .Concat(DefaultEditorColumns(context))
                .Concat(DefaultFilterColumns().Where(o => o == "Locked"))
                .Distinct()
                .ToList();
            return defaultColumns;
        }

        private bool EditorColumn(ColumnDefinition columnDefinition)
        {
            return
                columnDefinition?.EditorColumn > 0 &&
                columnDefinition?.NotEditorSettings != true;
        }

        private void UpdateTitleColumns(Context context)
        {
            if (TitleColumns == null)
            {
                TitleColumns = DefaultTitleColumns();
            }
            else
            {
                TitleColumns.RemoveAll(o => !TitleColumn(ColumnDefinitionHash.Get(o)));
            }
        }

        private List<string> DefaultTitleColumns()
        {
            return ColumnDefinitionHash.TitleDefinitions(enableOnly: true)
                .Select(o => o.ColumnName)
                .ToList();
        }

        private bool TitleColumn(ColumnDefinition columnDefinition)
        {
            return
                columnDefinition?.TitleColumn > 0 &&
                columnDefinition?.NotEditorSettings != true;
        }

        private void UpdateLinkColumns(Context context)
        {
            if (LinkColumns == null)
            {
                LinkColumns = DefaultLinkColumns(context: context);
            }
            else
            {
                LinkColumns.RemoveAll(o => !LinkColumn(ColumnDefinitionHash.Get(o)));
            }
        }

        private List<string> DefaultLinkColumns(Context context)
        {
            return ColumnDefinitionHash.LinkDefinitions(context: context, enableOnly: true)
                .Select(o => o.ColumnName)
                .ToList();
        }

        private bool LinkColumn(ColumnDefinition columnDefinition)
        {
            return columnDefinition?.LinkColumn > 0;
        }

        private void UpdateHistoryColumns(Context context)
        {
            if (HistoryColumns == null)
            {
                HistoryColumns = DefaultHistoryColumns(context: context);
            }
            else
            {
                HistoryColumns.RemoveAll(o => !HistoryColumn(ColumnDefinitionHash.Get(o)));
            }
        }

        private List<string> DefaultHistoryColumns(Context context)
        {
            return ColumnDefinitionHash.HistoryDefinitions(context: context, enableOnly: true)
                .Select(o => o.ColumnName)
                .ToList();
        }

        private bool HistoryColumn(ColumnDefinition columnDefinition)
        {
            return columnDefinition?.LinkColumn > 0;
        }

        private void UpdateColumns(Context context)
        {
            if (Columns == null) Columns = new List<Column>();
            var columnHash = Columns.ToDictionary(
                column => column.ColumnName,
                column => column);
            ColumnDefinitionHash?.Values.ForEach(columnDefinition =>
            {
                var column = columnHash.Get(columnDefinition.ColumnName);
                if (column == null)
                {
                    column = new Column(columnDefinition.ColumnName);
                    Columns.Add(column);
                    columnHash.Add(column.ColumnName, column);
                }
                UpdateColumn(
                    context: context,
                    ss: this,
                    columnDefinition: columnDefinition,
                    column: column);
            });
        }

        private void UpdateColumn(
            Context context,
            SiteSettings ss,
            ColumnDefinition columnDefinition,
            Column column,
            ColumnNameInfo columnNameInfo = null)
        {
            if (column != null)
            {
                column.Id = column.Id ?? columnDefinition.Id;
                column.LowSchemaVersion = columnDefinition.LowSchemaVersion();
                column.No = columnDefinition.No;
                column.Id_Ver =
                    ((columnDefinition.Unique || columnDefinition.Pk > 0)
                        && columnDefinition.TypeName == "bigint")
                            || columnDefinition.ColumnName == "Ver";
                column.ColumnName = column.ColumnName ?? columnDefinition.ColumnName;
                column.LabelText = ModifiedLabelText(
                    context: context,
                    column: column)
                        ?? column.LabelText
                        ?? Displays.Get(
                            context: context,
                            id: columnDefinition.Id);
                column.GridLabelText = ModifiedLabelText(
                    context: context,
                    column: column)
                        ?? column.GridLabelText
                        ?? column.LabelText;
                column.ChoicesText = column.ChoicesText ?? columnDefinition.ChoicesText;
                column.UseSearch = column.UseSearch ?? columnDefinition.UseSearch;
                column.MultipleSelections = column.MultipleSelections ?? false;
                column.NotInsertBlankChoice = column.NotInsertBlankChoice ?? false;
                column.DefaultInput = column.DefaultInput ?? columnDefinition.DefaultInput;
                column.ImportKey = column.ImportKey ?? columnDefinition.ImportKey;
                column.Anchor = column.Anchor ?? false;
                column.OpenAnchorNewTab = column.OpenAnchorNewTab ?? false;
                column.GridFormat = column.GridFormat ?? columnDefinition.GridFormat;
                column.EditorFormat = column.EditorFormat ?? columnDefinition.EditorFormat;
                column.ExportFormat = column.ExportFormat ?? columnDefinition.ExportFormat;
                column.ControlType = column.ControlType ?? columnDefinition.ControlType;
                column.ChoicesControlType = column.ChoicesControlType ?? HtmlFields.ControlTypes.DropDown.ToString();
                column.AutoNumberingResetType = column.AutoNumberingResetType ?? Column.AutoNumberingResetTypes.None;
                column.AutoNumberingDefault = column.AutoNumberingDefault ?? 1;
                column.AutoNumberingStep = column.AutoNumberingStep ?? 1;
                column.ValidateRequired = column.ValidateRequired ?? columnDefinition.ValidateRequired;
                column.ValidateNumber = column.ValidateNumber ?? columnDefinition.ValidateNumber;
                column.ValidateDate = column.ValidateDate ?? columnDefinition.ValidateDate;
                column.ValidateEmail = column.ValidateEmail ?? columnDefinition.ValidateEmail;
                column.ValidateEqualTo = column.ValidateEqualTo ?? columnDefinition.ValidateEqualTo;
                column.ValidateMaxLength = column.ValidateMaxLength ?? columnDefinition.MaxLength;
                column.ClientRegexValidation = column.ClientRegexValidation ?? columnDefinition.ClientRegexValidation;
                column.ServerRegexValidation = column.ServerRegexValidation ?? columnDefinition.ServerRegexValidation;
                column.RegexValidationMessage = column.RegexValidationMessage ?? columnDefinition.RegexValidationMessage;
                column.Nullable = column.Nullable ?? false;
                column.Unit = column.Unit ?? columnDefinition.Unit;
                column.DecimalPlaces = column.DecimalPlaces ?? columnDefinition.DecimalPlaces;
                column.RoundingType = column.RoundingType ?? RoundingTypes.AwayFromZero;
                column.Min = column.Min ?? DefaultMin(columnDefinition);
                column.Max = column.Max ?? DefaultMax(columnDefinition);
                column.Step = column.Step ?? DefaultStep(columnDefinition);
                column.DefaultMinValue = column.DefaultMinValue ?? columnDefinition.DefaultMinValue;
                column.DefaultMaxValue = column.DefaultMaxValue ?? columnDefinition.DefaultMaxValue;
                column.NoDuplication = column.NoDuplication ?? false;
                column.CopyByDefault = column.CopyByDefault ?? columnDefinition.CopyByDefault;
                column.EditorReadOnly = column.EditorReadOnly ?? columnDefinition.EditorReadOnly;
                column.AutoPostBack = column.AutoPostBack ?? false;
                column.AllowBulkUpdate = column.AllowBulkUpdate ?? false;
                column.AllowDeleteAttachments = column.AllowDeleteAttachments ?? true;
                column.NotDeleteExistHistory = column.NotDeleteExistHistory ?? false;
                column.AllowImage = column.AllowImage ?? true;
                column.ThumbnailLimitSize = column.ThumbnailLimitSize ?? Parameters.BinaryStorage.ThumbnailLimitSize;
                column.FieldCss = column.FieldCss ?? columnDefinition.FieldCss;
                column.ViewerSwitchingType = column.ViewerSwitchingType ?? (Column.ViewerSwitchingTypes)Parameters.General.ViewerSwitchingType;
                column.TextAlign = column.TextAlign ?? TextAlignTypes.Left;
                column.CheckFilterControlType = column.CheckFilterControlType ?? (ColumnUtilities.CheckFilterControlTypes)columnDefinition.CheckFilterControlType;
                column.NumFilterMin = column.NumFilterMin ?? columnDefinition.NumFilterMin;
                column.NumFilterMax = column.NumFilterMax ?? columnDefinition.NumFilterMax;
                column.NumFilterStep = column.NumFilterStep ?? columnDefinition.NumFilterStep;
                column.DateFilterSetMode = column.DateFilterSetMode ?? ColumnUtilities.GetDateFilterSetMode(columnDefinition: columnDefinition);
                column.SearchType = column.SearchType ?? column.SearchTypeDefault();
                column.FullTextType = column.FullTextType ?? (Column.FullTextTypes)columnDefinition.FullTextType;
                column.DateFilterMinSpan = column.DateFilterMinSpan ?? Parameters.General.DateFilterMinSpan;
                column.DateFilterMaxSpan = column.DateFilterMaxSpan ?? Parameters.General.DateFilterMaxSpan;
                column.DateFilterFy = column.DateFilterFy ?? true;
                column.DateFilterHalf = column.DateFilterHalf ?? true;
                column.DateFilterQuarter = column.DateFilterQuarter ?? true;
                column.DateFilterMonth = column.DateFilterMonth ?? true;
                column.OverwriteSameFileName = column.OverwriteSameFileName ?? false;
                column.LimitQuantity = column.LimitQuantity ?? Parameters.BinaryStorage.LimitQuantity;
                column.LimitSize = column.LimitSize ?? Parameters.BinaryStorage.LimitSize;
                column.TotalLimitSize = column.TotalLimitSize ?? Parameters.BinaryStorage.LimitTotalSize;
                column.LocalFolderLimitSize = column.LocalFolderLimitSize ?? Parameters.BinaryStorage.LocalFolderLimitSize;
                column.LocalFolderTotalLimitSize = column.LocalFolderTotalLimitSize ?? Parameters.BinaryStorage.LocalFolderLimitTotalSize;
                column.Size = columnDefinition.Size;
                column.DefaultNotNull = columnDefinition.DefaultNotNull;
                column.Required = columnDefinition.Required;
                column.RecordedTime = columnDefinition.Default == "now";
                column.NotSelect = columnDefinition.NotSelect;
                column.NotUpdate = columnDefinition.NotUpdate;
                column.EditSelf = !columnDefinition.NotEditSelf;
                column.GridColumn = columnDefinition.GridColumn > 0;
                column.FilterColumn = columnDefinition.FilterColumn > 0;
                column.EditorColumn = columnDefinition.EditorColumn > 0;
                column.NotEditorSettings = columnDefinition.NotEditorSettings;
                column.TitleColumn = columnDefinition.TitleColumn > 0;
                column.LinkColumn = columnDefinition.LinkColumn > 0;
                column.HistoryColumn = columnDefinition.HistoryColumn > 0;
                column.Export = columnDefinition.Export > 0;
                column.LabelTextDefault = Displays.Get(
                    context: context,
                    id: columnDefinition.Id);
                column.TypeName = columnDefinition.TypeName;
                column.TypeCs = columnDefinition.TypeCs;
                column.DbNullable = columnDefinition.Nullable;
                column.JoinTableName = columnDefinition.JoinTableName;
                column.Type = columnDefinition.UserColumn
                    ? Column.Types.User
                    : Column.Types.Normal;
                column.Hash = columnDefinition.Hash;
                column.StringFormat = columnDefinition.StringFormat;
                column.UnitDefault = columnDefinition.Unit;
                column.Width = columnDefinition.Width;
                column.ControlCss = columnDefinition.ControlCss;
                column.GridStyle = columnDefinition.GridStyle;
                column.Aggregatable = columnDefinition.Aggregatable;
                column.Computable = columnDefinition.Computable;
                columnNameInfo = columnNameInfo ?? new ColumnNameInfo(column.ColumnName);
                column.SiteSettings = ss;
                column.Name = columnNameInfo.Name;
                column.TableAlias = columnNameInfo.TableAlias;
                column.SiteId = columnNameInfo.Joined
                    ? columnNameInfo.SiteId
                    : SiteId;
                column.BinaryStorageProvider = column.BinaryStorageProvider ?? Parameters.BinaryStorage.DefaultBinaryStorageProvider;
                column.DateTimeStep = DefaultDateTimeStep(column.DateTimeStep ?? Parameters.General.DateTimeStep);
                column.Joined = columnNameInfo.Joined;
            }
        }

        public Column ResetColumn(Context context, string columnName)
        {
            var columnDefinition = ColumnDefinitionHash.Get(columnName);
            var column = new Column(columnDefinition.ColumnName);
            UpdateColumn(
                context: context,
                ss: this,
                columnDefinition: columnDefinition,
                column: column);
            ColumnHash.AddOrUpdate(columnName, column);
            Columns.RemoveAll(o => o.ColumnName == columnName);
            Columns.Add(column);
            return column;
        }

        private string ModifiedLabelText(Context context, Column column)
        {
            switch (TableType)
            {
                case Sqls.TableTypes.Deleted:
                    switch (column.ColumnName)
                    {
                        case "Updator":
                            return Displays.Deleter(context: context);
                        case "UpdatedTime":
                            return Displays.DeletedTime(context: context);
                    }
                    break;
            }
            return null;
        }

        private void UpdateColumnHash()
        {
            ColumnHash = Columns
                .GroupBy(o => o.ColumnName)
                .Select(o => o.First())
                .Where(o => ColumnDefinitionHash?.ContainsKey(o?.Name ?? string.Empty) == true)
                .Where(o => !o.LowSchemaVersion)
                .ToDictionary(o => o.ColumnName, o => o);
        }

        private void Update_CreateColumnAccessControls(IEnumerable<Column> columns)
        {
            var columnAccessControls = columns
                .Where(o => !o.Required)
                .Select(column => new ColumnAccessControl(this, column, "Create"))
                .ToList();
            CreateColumnAccessControls?.ForEach(o =>
                SetColumnAccessControl(columnAccessControls, o));
            CreateColumnAccessControls = columnAccessControls;
        }

        private void Update_ReadColumnAccessControls(IEnumerable<Column> columns)
        {
            var columnAccessControls = columns
                .Select(column => new ColumnAccessControl(this, column, "Read"))
                .ToList();
            ReadColumnAccessControls?.ForEach(o =>
                SetColumnAccessControl(columnAccessControls, o));
            ReadColumnAccessControls = columnAccessControls;
        }

        private void Update_UpdateColumnAccessControls(IEnumerable<Column> columns)
        {
            var columnAccessControls = columns
                .Select(column => new ColumnAccessControl(this, column, "Update"))
                .ToList();
            UpdateColumnAccessControls?.ForEach(o =>
                SetColumnAccessControl(columnAccessControls, o));
            UpdateColumnAccessControls = columnAccessControls;
        }

        public void Update_ColumnAccessControls()
        {
            var accessControlColumns = Columns
                .Where(o => o.EditorColumn || o.ColumnName == "Comments")
                .Where(o => !o.NotEditorSettings)
                .Where(o => !o.Id_Ver)
                .ToList();
            Update_CreateColumnAccessControls(accessControlColumns);
            Update_ReadColumnAccessControls(accessControlColumns);
            Update_UpdateColumnAccessControls(accessControlColumns);
        }

        private void SetColumnAccessControl(
            IEnumerable<ColumnAccessControl> columnAccessControls,
            ColumnAccessControl columnAccessControl)
        {
            var data = columnAccessControls.FirstOrDefault(o =>
                o.ColumnName == columnAccessControl.ColumnName);
            if (data != null)
            {
                data.Depts = columnAccessControl.Depts;
                data.Groups = columnAccessControl.Groups;
                data.Users = columnAccessControl.Users;
                data.Type = columnAccessControl.Type;
                data.RecordUsers = columnAccessControl.RecordUsers;
            }
        }

        public decimal DefaultMin(ColumnDefinition columnDefinition)
        {
            return columnDefinition.ExtendedColumnType == "Num"
                && columnDefinition.DefaultMinValue != 0
                    ? columnDefinition.DefaultMinValue
                    : columnDefinition.Min;
        }

        public decimal DefaultMax(ColumnDefinition columnDefinition)
        {
            return columnDefinition.ExtendedColumnType == "Num"
                && columnDefinition.DefaultMaxValue != 0
                    ? columnDefinition.DefaultMaxValue
                    : (columnDefinition.Max > 0
                        ? columnDefinition.Max
                        : columnDefinition.MaxLength);
        }

        private decimal DefaultStep(ColumnDefinition columnDefinition)
        {
            return (columnDefinition.Step > 0
                ? columnDefinition.Step
                : 1);
        }

        private int DefaultDateTimeStep(int datetimeStep)
        {
            return datetimeStep > 0
                ? datetimeStep
                : 10;
        }

        public Column GetColumn(
            Context context,
            string columnName)
        {
            var column = ColumnHash.Get(columnName);
            if (column == null
                && columnName?.Contains(',') == true
                && JoinOptions().ContainsKey(columnName.Split_1st()) == true)
            {
                column = AddJoinedColumn(
                    context: context,
                    columnName: columnName);
                return column;
            }
            if (column?.SiteSettings?.ColumnDefinitionHash?.ContainsKey(column?.Name ?? string.Empty) != true)
            {
                return null;
            }
            return column;
        }

        public Column GetColumnOrExtendedFieldColumn(
            Context context,
            string columnName,
            string extendedFieldType)
        {
            var column = GetColumn(
                context: context,
                columnName: columnName);
            if (column == null)
            {
                column = context.ExtendedFieldColumn(
                    ss: this,
                    columnName: columnName,
                    extendedFieldType: extendedFieldType);
            }
            return column;
        }

        public bool HasAllColumns(Context context, params string[] parts)
        {
            return parts.All(columnName => GetColumn(
                context: context,
                columnName: columnName) != null);
        }

        private Column AddJoinedColumn(Context context, string columnName)
        {
            var columnNameInfo = new ColumnNameInfo(columnName);
            var ss = JoinedSsHash.Get(columnNameInfo.SiteId);
            if (!columnNameInfo.Exists(
                ss: ss,
                joinedSsHash: JoinedSsHash))
            {
                return null;
            }
            var columnDefinition = ss?.ColumnDefinitionHash.Get(columnNameInfo.Name);
            if (columnDefinition != null)
            {
                var column = ss.ColumnHash.Get(columnNameInfo.Name);
                if (column != null)
                {
                    var type = column.Type;
                    column = column.Copy();
                    column.ColumnName = columnName;
                    UpdateColumn(
                        context: context,
                        ss: ss,
                        columnDefinition: columnDefinition,
                        column: column,
                        columnNameInfo: columnNameInfo);
                    column.ChoiceHash = ss.ColumnHash
                        .Get(columnNameInfo.Name)
                        .ChoiceHash
                        ?.ToDictionary(o => o.Key, o => o.Value);
                    column.Type = type;
                    Columns.Add(column);
                    ColumnHash.Add(columnName, column);
                }
                return column;
            }
            else
            {
                return null;
            }
        }

        public Column LinkedTitleColumn(Context context, Column column)
        {
            return column?.Linked(context: context) == true
                ? GetColumn(
                    context: context,
                    columnName: ColumnUtilities.ColumnName(column.TableAlias, "Title"))
                : null;
        }

        public Column GridColumn(string columnName)
        {
            return Columns.FirstOrDefault(o =>
                o.ColumnName == columnName && o.GridColumn);
        }

        public Column FilterColumn(string columnName)
        {
            return Columns.FirstOrDefault(o =>
                o.ColumnName == columnName && o.FilterColumn);
        }

        public Column EditorColumn(string columnName)
        {
            return Columns.FirstOrDefault(o =>
                o.ColumnName == columnName && o.EditorColumn);
        }

        public Column TitleColumn(string columnName)
        {
            return Columns.FirstOrDefault(o =>
                o.ColumnName == columnName && o.TitleColumn);
        }

        public Column LinkColumn(string columnName)
        {
            return Columns.FirstOrDefault(o =>
                o.ColumnName == columnName && o.LinkColumn);
        }

        public Column HistoryColumn(string columnName)
        {
            return Columns.FirstOrDefault(o =>
                o.ColumnName == columnName && o.HistoryColumn);
        }

        public Column FormulaColumn(string name, string calculationMethod = null)
        {
            return (string.IsNullOrEmpty(calculationMethod) || calculationMethod == FormulaSet.CalculationMethods.Default.ToString()
                ? Columns
                    .Where(o => o.ColumnName == name || o.LabelText == name)
                    .Where(o => o.TypeName == "decimal")
                    .Where(o => !o.NotUpdate)
                    .Where(o => !o.Joined)
                : Columns
                    .Where(o => o.ColumnName == name || o.LabelText == name)
                    .Where(o => o.ControlType != "Attachments")
                    .Where(o => !o.NotUpdate)
                    .Where(o => !o.Id_Ver)
                    .Where(o => !o.Joined)
                    .Where(o => !o.OtherColumn())
                    .Where(o => o.Name != "SiteId"
                        && o.Name != "Comments"))
                .FirstOrDefault();
        }

        public List<Column> FormulaColumnList()
        {
            return Columns
                .Where(o => o.ControlType != "Attachments")
                .Where(o => !o.NotUpdate)
                .Where(o => !o.Id_Ver)
                .Where(o => !o.Joined)
                .Where(o => !o.OtherColumn())
                .Where(o => o.Name != "SiteId"
                    && o.Name != "Comments")
                .OrderByDescending(o => o.LabelText.Length)
                .ToList();
        }

        public List<Column> GetGridColumns(
            Context context,
            View view = null,
            bool checkPermission = false,
            bool includedColumns = false)
        {
            var columns = (view?.GridColumns ?? GridColumns)
                .Select(columnName => GetColumn(
                    context: context,
                    columnName: columnName))
                .Where(column => column != null)
                .AllowedColumns(
                    context: context,
                    ss: this,
                    checkPermission: checkPermission)
                .Where(o => context.ContractSettings.Attachments()
                    || o.ControlType != "Attachments")
                .ToList();
            if (view?.ShowHistory == true
                && !columns.Any(o => o.ColumnName == "Ver"))
            {
                columns.Add(GetColumn(
                    context: context,
                    columnName: "Ver"));
            }
            if (includedColumns)
            {
                IncludedColumns()
                    .Where(columnName => !columns.Any(o => o.ColumnName == columnName))
                    .ForEach(columnName =>
                        columns.Add(GetColumn(
                            context: context,
                            columnName: columnName)));
            }
            return columns;
        }

        public List<Column> GetLinkTableColumns(
            Context context, View view = null, bool checkPermission = false)
        {
            return (view?.GridColumns ?? LinkColumns)
                .Select(columnName => GetColumn(context: context, columnName: columnName))
                .Where(column => column != null)
                .Where(column => !column.Joined)
                .AllowedColumns(
                    context: context,
                    ss: this,
                    checkPermission: checkPermission)
                .Where(o => context.ContractSettings.Attachments()
                    || o.ControlType != "Attachments")
                .ToList();
        }

        public List<Column> GetFilterColumns(
            Context context,
            View view,
            bool checkPermission = false)
        {
            var columns = (view?.FilterColumns ?? FilterColumns)
                .Select(columnName => GetColumn(
                    context: context,
                    columnName: columnName))
                .Where(column => column != null)
                .Where(column =>
                    GridColumns.Contains(column.ColumnName)
                    || GetEditorColumnNames().Contains(column.ColumnName)
                    || column.Id_Ver
                    || column.ColumnName.Contains("~")
                    || column.ColumnName == "Creator"
                    || column.ColumnName == "Updator"
                    || column.ColumnName == "CreatedTime"
                    || column.ColumnName == "UpdatedTime")
                .AllowedColumns(
                    context: context,
                    ss: this,
                    checkPermission: checkPermission)
                .ToList();
            foreach (var column in context.ExtendedFieldColumns(
                ss: this,
                extendedFieldType: "Filter"))
            {
                if (column.After.IsNullOrEmpty())
                {
                    columns.Insert(0, column);
                }
                else
                {
                    var index = columns
                        .Select((Column, Index) => new { Column, Index })
                        .FirstOrDefault(o => o.Column.ColumnName == column.After)
                        ?.Index;
                    if (index != null)
                    {
                        columns.Insert(index.ToInt() + 1, column);
                    }
                    else
                    {
                        columns.Add(column);
                    }
                }
            }
            return columns;
        }

        public List<Column> GetEditorColumns(Context context, bool columnOnly = true)
        {
            return GetEditorColumns(
                context: context,
                tab: null,
                columnOnly: columnOnly);
        }

        public List<Column> GetEditorColumns(Context context, Tab tab, bool columnOnly = true)
        {
            return (tab == null
                ? GetEditorColumnNames()
                : EditorColumnHash.Get(key: TabName(tabId: tab?.Id)))
                    ?.Select(columnName => columnOnly
                        ? GetColumn(
                            context: context,
                            columnName: columnName)
                        : GetColumn(
                            context: context,
                            columnName: columnName)
                                ?? new Column(columnName))
                    .Where(column => column != null)
                    .Where(o => context.ContractSettings.Attachments()
                        || o.ControlType != "Attachments")
                    .ToList();
        }

        public List<string> GetEditorColumnNames(Column postbackColumn = null)
        {
            var columnNames = (EditorColumnHash.Get(TabName(0))
                ?? Enumerable.Empty<string>())
                    .Union(EditorColumnHash
                        ?.Where(hash => TabId(hash.Key) > 0)
                        .Select(hash => new
                        {
                            Id = TabId(hash.Key),
                            Hash = hash
                        })
                        .Where(hash => hash.Id > 0)
                        .SelectMany(hash => hash.Hash.Value)
                            ?? Enumerable.Empty<string>())
                    .ToList();
            var postbackTargets = postbackColumn?.ColumnsReturnedWhenAutomaticPostback?.Split(',');
            if (postbackTargets?.Any() == true)
            {
                columnNames = postbackTargets
                    .Where(columnName => columnNames.Contains(columnName))
                    .ToList();
            }
            return columnNames;
        }

        public List<string> GetEditorColumnNames(
            Context context,
            bool columnOnly)
        {
            return GetEditorColumnNames()
                .Where(columnName => !columnOnly || GetColumn(
                    context: context,
                    columnName: columnName) != null)
                .ToList();
        }

        public Dictionary<string, string> GetAllowBulkUpdateOptions(Context context)
        {
            var options = BulkUpdateColumns?.ToDictionary(o => $"BulkUpdate_{o.Id}", o => o.Title)
                ?? new Dictionary<string, string>();
            options = options
                .Concat(GetAllowBulkUpdateColumns(context: context)
                    .ToDictionary(o => o.ColumnName, o => o.LabelText))
                .ToDictionary(o => o.Key, o => o.Value);
            return options;
        }

        public List<Column> GetAllowBulkUpdateColumns(Context context)
        {
            return GetEditorColumns(context: context)
                .Where(c => !c.Id_Ver)
                .Where(c => !c.GetEditorReadOnly())
                .Where(c => c.NoDuplication != true)
                .Where(c => c.ColumnName != "Comments")
                .Where(column => !Formulas.Any(formulaSet =>
                    (string.IsNullOrEmpty(formulaSet.CalculationMethod)
                        || formulaSet.CalculationMethod == FormulaSet.CalculationMethods.Default.ToString())
                    && (formulaSet.Target == column.ColumnName
                        || ContainsFormulaColumn(
                            columnName: column.ColumnName,
                            children: formulaSet.Formula.Children))))
                .Where(column => column.AllowBulkUpdate == true)
                .Where(column => column.CanUpdate(
                    context: context,
                    ss: this,
                    mine: null))
                .ToList();
        }

        private bool ContainsFormulaColumn(string columnName, List<Formula> children)
        {
            if (children != null)
            {
                foreach (var formula in children)
                {
                    if (formula.ColumnName == columnName)
                    {
                        return true;
                    }
                    var ret = ContainsFormulaColumn(
                        columnName: columnName,
                        children: formula.Children);
                    if (ret) return true;
                }
            }
            return false;
        }

        public List<Column> GetTitleColumns(Context context)
        {
            return TitleColumns?
                .Select(columnName => GetColumn(context: context, columnName: columnName))
                .Where(column => column != null)
                .ToList();
        }

        public List<Column> GetLinkColumns(Context context, bool checkPermission = false)
        {
            return LinkColumns
                .Select(columnName => GetColumn(context: context, columnName: columnName))
                .Where(column => column != null)
                .AllowedColumns(
                    context: context,
                    ss: this,
                    checkPermission: checkPermission)
                .Where(o => context.ContractSettings.Attachments()
                    || o.ControlType != "Attachments")
                .ToList();
        }

        public List<Column> GetHistoryColumns(Context context, bool checkPermission = false)
        {
            return HistoryColumns
                .Select(columnName => GetColumn(
                    context: context,
                    columnName: columnName))
                .Where(column => column != null)
                .AllowedColumns(
                    context: context,
                    ss: this,
                    checkPermission: checkPermission)
                .Where(o => context.ContractSettings.Attachments()
                    || o.ControlType != "Attachments")
                .ToList();
        }

        public IEnumerable<Column> SelectColumns()
        {
            return Columns
                ?.Where(o => !o.NotSelect)
                .Where(o => o.Required
                    // 規定値が含まれている項目も select して Set の際に Saved に保存済の値を格納する必用がある
                    // Saved に保存済の値が格納されていない場合、model 初期化時の SetDefault にて Updated 状態となり
                    // 項目のアクセス制御の権限が無い場合に、更新操作が失敗する
                    || !o.DefaultInput.IsNullOrEmpty()
                    || EditorColumnHash?.Any(tab => tab
                        .Value
                        ?.Contains(o.ColumnName) == true) == true);
        }

        public string TabName(int? tabId)
        {
            return tabId > 0 ? $"_Tab-{tabId}" : "General";
        }

        public int TabId(string tabName)
        {
            return tabName?.StartsWith("_Tab-") == true
                ? tabName.Substring("_Tab-".Length).ToInt()
                : 0;
        }

        public void AddOrUpdateEditorColumnHash(
            List<string> editorColumnsAll,
            string editorColumnsTabsTarget = "0")
        {
            if (!editorColumnsTabsTarget.IsNullOrEmpty())
            {
                var tabId = editorColumnsTabsTarget.ToInt();
                if (tabId == 0 || Tabs?.Get(tabId) != null)
                {
                    (EditorColumnHash ?? (EditorColumnHash = new Dictionary<string, List<string>>()))
                        .AddOrUpdate(
                            key: TabName(tabId: tabId),
                            value: editorColumnsAll);
                }
            }
        }

        public string LinkId(SiteSettings ss)
        {
            return $"_Links-{ss.SiteId}";
        }

        public string LinkId(long siteId)
        {
            return $"_Links-{siteId}";
        }

        public long LinkId(string columnName)
        {
            return columnName.StartsWith("_Links-")
                ? columnName.Substring("_Links-".Length).ToInt()
                : 0;
        }

        public int SectionId(string columnName)
        {
            return columnName.StartsWith("_Section-")
                ? columnName.Substring("_Section-".Length).ToInt()
                : 0;
        }

        public string SectionName(int? sectionId)
        {
            return sectionId > 0 ? $"_Section-{sectionId}" : null;
        }

        public Dictionary<string, ControlData> TabSelectableOptions(
            Context context,
            bool habGeneral = false)
        {
            return (habGeneral
                ? new List<Tab>
                {
                    new Tab
                    {
                        Id = 0,
                        LabelText = GeneralTabLabelText.IsNullOrEmpty()
                            ? Displays.General(context:context)
                            : GeneralTabLabelText
                    }
                }
                    .Union(Tabs ?? Enumerable.Empty<Tab>())
                : Tabs)
                    ?.ToDictionary(
                        o => o.Id.ToString(),
                        o => new ControlData(o.LabelText));
        }

        public Dictionary<string, ControlData> EditorLinksSelectableOptions(
            Context context,
            bool enabled = true)
        {
            return enabled
                ? Sources
                    .Union(Destinations)
                    .GroupBy(currentSs => currentSs.Key)
                    .Select(currentSs => currentSs.First())
                    .OrderBy(currentSs => currentSs.Key)
                    .ToDictionary(
                        currentSs => LinkId(currentSs.Value),
                        currentSs => new ControlData(currentSs.Value.Title))
                : Sources.Union(Destinations)
                    .Where(currentSs => !GetEditorColumnNames()
                        .Contains(LinkId(currentSs.Value)))
                    .GroupBy(currentSs => currentSs.Key)
                    .Select(currentSs => currentSs.First())
                    .OrderBy(currentSs => currentSs.Key)
                    .ToDictionary(
                        currentSs => LinkId(currentSs.Value),
                        currentSs => new ControlData(currentSs.Value.Title));
        }

        public Dictionary<string, ControlData> GridSelectableOptions(
            Context context, bool enabled = true, string join = null)
        {
            var currentSs = GetJoinedSs(join);
            return enabled
                ? ColumnUtilities.SelectableOptions(
                    context: context,
                    ss: currentSs,
                    columns: GridColumns,
                    labelType: "Grid",
                    order: currentSs?.ColumnDefinitionHash?.GridDefinitions(context: context)?
                        .OrderBy(o => o.No)
                        .Select(o => join.IsNullOrEmpty()
                            ? o.ColumnName
                            : join + "," + o.ColumnName).ToList())
                : ColumnUtilities.SelectableSourceOptions(
                    context: context,
                    ss: currentSs,
                    columns: currentSs?.ColumnDefinitionHash?.GridDefinitions(context: context)
                        .OrderBy(o => o.No)
                        .Select(o => join.IsNullOrEmpty()
                            ? o.ColumnName
                            : join + "," + o.ColumnName)
                        .Where(o => !GridColumns.Contains(o)),
                    labelType: "Grid",
                    order: currentSs?.ColumnDefinitionHash?.GridDefinitions(context: context)?
                        .OrderBy(o => o.No)
                        .Select(o => join.IsNullOrEmpty()
                            ? o.ColumnName
                            : join + "," + o.ColumnName).ToList());
        }

        public Dictionary<string, ControlData> FilterSelectableOptions(
            Context context, bool enabled = true, string join = null)
        {
            var currentSs = GetJoinedSs(join);
            return enabled
                ? ColumnUtilities.SelectableOptions(
                    context: context,
                    ss: currentSs,
                    columns: FilterColumns,
                    order: currentSs?.ColumnDefinitionHash?.FilterDefinitions()?
                        .OrderBy(o => o.No)
                        .Select(o => join.IsNullOrEmpty()
                            ? o.ColumnName
                            : join + "," + o.ColumnName).ToList())
                : ColumnUtilities.SelectableSourceOptions(
                    context: context,
                    ss: currentSs,
                    columns: currentSs.ColumnDefinitionHash.FilterDefinitions()
                        .OrderBy(o => o.No)
                        .Select(o => join.IsNullOrEmpty()
                            ? o.ColumnName
                            : join + "," + o.ColumnName)
                        .Where(o => !FilterColumns.Contains(o)),
                    order: currentSs?.ColumnDefinitionHash?.FilterDefinitions()?
                        .OrderBy(o => o.No)
                        .Select(o => join.IsNullOrEmpty()
                            ? o.ColumnName
                            : join + "," + o.ColumnName).ToList());
        }

        public Dictionary<string, ControlData> ViewGridSelectableOptions(
            Context context, List<string> gridColumns, bool enabled = true, string join = null)
        {
            var currentSs = GetJoinedSs(join);
            return enabled
                ? ColumnUtilities.SelectableOptions(
                    context: context,
                    ss: currentSs,
                    columns: gridColumns,
                    labelType: "Grid",
                    order: currentSs?.ColumnDefinitionHash?.GridDefinitions(context: context)?
                        .OrderBy(o => o.No)
                        .Select(o => join.IsNullOrEmpty()
                            ? o.ColumnName
                            : join + "," + o.ColumnName).ToList())
                : ColumnUtilities.SelectableSourceOptions(
                    context: context,
                    ss: currentSs,
                    columns: currentSs?.ColumnDefinitionHash?.GridDefinitions(context: context)
                        .OrderBy(o => o.No)
                        .Select(o => join.IsNullOrEmpty()
                            ? o.ColumnName
                            : join + "," + o.ColumnName)
                        .Where(o => !gridColumns.Contains(o)),
                    labelType: "Grid",
                    order: currentSs?.ColumnDefinitionHash?.GridDefinitions(context: context)?
                        .OrderBy(o => o.No)
                        .Select(o => join.IsNullOrEmpty()
                            ? o.ColumnName
                            : join + "," + o.ColumnName).ToList());
        }

        public Dictionary<string, ControlData> ViewFilterSelectableOptions(
            Context context, List<string> filterColumns, bool enabled = true, string join = null)
        {
            var currentSs = GetJoinedSs(join);
            return enabled
                ? ColumnUtilities.SelectableOptions(
                    context: context,
                    ss: currentSs,
                    columns: filterColumns,
                    order: currentSs?.ColumnDefinitionHash?.FilterDefinitions()?
                        .OrderBy(o => o.No)
                        .Select(o => join.IsNullOrEmpty()
                            ? o.ColumnName
                            : join + "," + o.ColumnName).ToList())
                : ColumnUtilities.SelectableSourceOptions(
                    context: context,
                    ss: currentSs,
                    columns: currentSs.ColumnDefinitionHash.FilterDefinitions()
                        .OrderBy(o => o.No)
                        .Select(o => join.IsNullOrEmpty()
                            ? o.ColumnName
                            : join + "," + o.ColumnName)
                        .Where(o => !filterColumns.Contains(o)),
                    order: currentSs?.ColumnDefinitionHash?.FilterDefinitions()?
                        .OrderBy(o => o.No)
                        .Select(o => join.IsNullOrEmpty()
                            ? o.ColumnName
                            : join + "," + o.ColumnName).ToList());
        }

        public Dictionary<string, string> ViewFilterOptions(
            Context context,
            View view,
            bool currentTableOnly)
        {
            var hash = new Dictionary<string, string>();
            JoinOptions(currentTableOnly: currentTableOnly).ForEach(join =>
            {
                var siteId = ColumnUtilities.GetSiteIdByTableAlias(join.Key, SiteId);
                var ss = JoinedSsHash.Get(siteId);
                if (ss != null)
                {
                    hash.AddRange(ss.ColumnDefinitionHash.Values
                        .Where(o => o.FilterColumn > 0)
                        .Where(o => view?.ColumnFilterHash?.ContainsKey(o.ColumnName) != true)
                        .OrderBy(o => o.FilterColumn)
                        .Select(o => ss.GetColumn(
                            context: context,
                            columnName: o.ColumnName))
                        .ToDictionary(
                            o => ColumnUtilities.ColumnName(join.Key, o.Name),
                            o => join.Value + " " + o.LabelText));
                }
            });
            return hash;
        }

        public Dictionary<string, string> ViewSorterOptions(
            Context context,
            bool currentTableOnly = false)
        {
            var hash = new Dictionary<string, string>();
            JoinOptions(currentTableOnly: currentTableOnly).ForEach(join =>
            {
                var siteId = ColumnUtilities.GetSiteIdByTableAlias(join.Key, SiteId);
                var ss = JoinedSsHash.Get(siteId);
                if (ss != null)
                {
                    hash.AddRange(ss.ColumnDefinitionHash.Values
                        .Where(o => o.GridColumn > 0)
                        .OrderBy(o => o.GridColumn)
                        .Select(o => ss.GetColumn(context: context, columnName: o.ColumnName))
                        .ToDictionary(
                            o => ColumnUtilities.ColumnName(join.Key, o.Name),
                            o => join.Value + " " + o.LabelText));
                }
            });
            return hash;
        }

        public Dictionary<string, ControlData> EditorSelectableOptions(
            Context context,
            bool enabled = true,
            string selection = "",
            string keyWord = "")
        {
            return enabled
                ? ColumnUtilities.SelectableOptions(
                    context: context,
                    ss: this,
                    columns: EditorColumnHash
                        ?.Where(o => o.Value?.Contains(context
                            .Forms
                            .Data("EditorColumnName")) == true)
                        .Select(o => o.Value)
                        .FirstOrDefault() ?? new List<string>(),
                    order: ColumnDefinitionHash.EditorDefinitions(context: context)
                        .OrderBy(o => o.EditorColumn)
                        .Select(o => o.ColumnName).ToList())
                : ColumnUtilities.SelectableOptions(
                    context: context,
                    ss: this,
                    columns: ColumnDefinitionHash.EditorDefinitions(context: context)
                        .Where(o => !GetEditorColumnNames().Contains(o.ColumnName))
                        .Where(o => FilterColumn(
                            context: context,
                            ss: this,
                            def: o,
                            selection: selection,
                            keyWord: keyWord))
                        .OrderBy(o => o.EditorColumn)
                        .Select(o => o.ColumnName),
                    order: ColumnDefinitionHash?.EditorDefinitions(context: context)?
                        .OrderBy(o => o.EditorColumn)
                        .Select(o => o.ColumnName).ToList());
        }

        private bool FilterColumn(
            Context context,
            SiteSettings ss,
            ColumnDefinition def,
            string selection,
            string keyWord)
        {
            switch (selection)
            {
                case "KeyWord":
                    var keyWords = keyWord.Replace("　", " ").Split(" ");
                    return keyWords.All(o => def.ColumnName.Contains(
                        value: o,
                        comparisonType: StringComparison.OrdinalIgnoreCase))
                            || keyWords
                                .All(o => def.LabelText.Contains(
                                    value: o,
                                    comparisonType: StringComparison.OrdinalIgnoreCase))
                            || keyWords
                                .All(o => ss.GetColumn(context, def.ColumnName).LabelText.Contains(
                                    value: o,
                                    comparisonType: StringComparison.OrdinalIgnoreCase));
                case "Basic":
                    //「分類」「数値」「日付」「説明」「チェック」「添付ファイル」の
                    //何れでもないカラムならばTrueを返却する（例「担当者」はTrue）
                    return new List<string>
                    {
                        "Class",
                        "Num",
                        "Date",
                        "Description",
                        "Check",
                        "Attachments"
                    }
                        .All(o => !def.ColumnName.StartsWith(o));
                case "Class":
                case "Num":
                case "Date":
                case "Description":
                case "Check":
                case "Attachments":
                    return def.ColumnName.StartsWith(selection);
                default:
                    return true;
            }
        }

        public Dictionary<string, ControlData> EditorSelectableOptions(
            Context context,
            int tabId)
        {
            return EditorSelectableOptions(
                context: context,
                EditorColumnHash?.Get(TabName(tabId)) ?? new List<string>());
        }

        private Dictionary<string, ControlData> EditorSelectableOptions(
            Context context,
            List<string> editorColumns,
            bool enabled = true)
        {
            return enabled
                ? ColumnUtilities.SelectableOptions(
                    context: context,
                    ss: this,
                    columns: editorColumns,
                    order: ColumnDefinitionHash.EditorDefinitions(context: context)
                        .OrderBy(o => o.EditorColumn)
                        .Select(o => o.ColumnName).ToList())
                : ColumnUtilities.SelectableOptions(
                    context: context,
                    ss: this,
                    columns: ColumnDefinitionHash.EditorDefinitions(context: context)
                        .Where(o => !editorColumns.Contains(o.ColumnName))
                        .OrderBy(o => o.EditorColumn)
                        .Select(o => o.ColumnName),
                    order: ColumnDefinitionHash?.EditorDefinitions(context: context)
                        ?.OrderBy(o => o.EditorColumn)
                        .Select(o => o.ColumnName)
                        .ToList());
        }

        public Dictionary<string, ControlData> TitleSelectableOptions(
            Context context, IEnumerable<string> titleColumns, bool enabled = true)
        {
            return enabled
                ? ColumnUtilities.SelectableOptions(
                    context: context,
                    ss: this,
                    columns: titleColumns,
                    order: ColumnDefinitionHash?.TitleDefinitions()?
                        .OrderBy(o => o.TitleColumn)
                        .Select(o => o.ColumnName).ToList())
                : ColumnUtilities.SelectableOptions(
                    context: context,
                    ss: this,
                    columns: ColumnDefinitionHash.TitleDefinitions()
                        .Where(o => !titleColumns.Contains(o.ColumnName))
                        .OrderBy(o => o.TitleColumn)
                        .Select(o => o.ColumnName),
                    order: ColumnDefinitionHash?.TitleDefinitions()?
                        .OrderBy(o => o.TitleColumn)
                        .Select(o => o.ColumnName).ToList());
        }

        public Dictionary<string, ControlData> LinkSelectableOptions(
            Context context, bool enabled = true)
        {
            return enabled
                ? ColumnUtilities.SelectableOptions(
                    context: context,
                    ss: this,
                    columns: LinkColumns,
                    order: ColumnDefinitionHash.LinkDefinitions(context: context)
                        .OrderBy(o => o.LinkColumn)
                        .Select(o => o.ColumnName).ToList())
                : ColumnUtilities.SelectableOptions(
                    context: context,
                    ss: this,
                    columns: ColumnDefinitionHash.LinkDefinitions(context: context)
                        .Where(o => !LinkColumns.Contains(o.ColumnName))
                        .OrderBy(o => o.LinkColumn)
                        .Select(o => o.ColumnName),
                    order: ColumnDefinitionHash?.LinkDefinitions(context: context)
                        .OrderBy(o => o.LinkColumn)
                        .Select(o => o.ColumnName).ToList());
        }

        public Dictionary<string, ControlData> HistorySelectableOptions(
            Context context, bool enabled = true)
        {
            return enabled
                ? ColumnUtilities.SelectableOptions(
                    context: context,
                    ss: this,
                    columns: HistoryColumns,
                    order: ColumnDefinitionHash.HistoryDefinitions(context: context)
                        .OrderBy(o => o.HistoryColumn)
                        .Select(o => o.ColumnName).ToList())
                : ColumnUtilities.SelectableOptions(
                    context: context,
                    ss: this,
                    columns: ColumnDefinitionHash.HistoryDefinitions(context: context)
                        .Where(o => !HistoryColumns.Contains(o.ColumnName))
                        .OrderBy(o => o.HistoryColumn)
                        .Select(o => o.ColumnName),
                    order: ColumnDefinitionHash?.HistoryDefinitions(context: context)?
                        .OrderBy(o => o.HistoryColumn)
                        .Select(o => o.ColumnName).ToList());
        }

        public Dictionary<string, ControlData> MoveTargetsSelectableOptions(Context context)
        {
            var options = MoveTargetsOptions(sites: NumberOfMoveTargetsTable(context));
            return MoveTargets?.Any() == true
                ? options
                    .Where(o => MoveTargets.Contains(o.Key.ToLong()))
                    .OrderBy(o => MoveTargets.IndexOf(o.Key.ToLong()))
                    .ToDictionary(o => o.Key, o => o.Value)
                : null;
        }

        public EnumerableRowCollection<DataRow> NumberOfMoveTargetsTable(Context context)
        {
            return Repository.ExecuteTable(
                context: context,
                statements: new SqlStatement(
                    commandText: Def.Sql.MoveTarget,
                    param: Rds.SitesParam()
                        .TenantId(context.TenantId)
                        .ReferenceType(ReferenceType)
                        .SiteId(SiteId)
                        .Add(name: "HasPrivilege", value: context.HasPrivilege)))
                            .AsEnumerable();
        }

        public Dictionary<string, ControlData> MoveTargetsOptions(IEnumerable<DataRow> sites)
        {
            var targets = new Dictionary<string, ControlData>();
            foreach (var dataRow in sites.Where(dataRow => dataRow.String("ReferenceType") == ReferenceType))
            {
                var siteId = dataRow.String("SiteId");
                var title = $"[{siteId}] {dataRow.String("Title")}";
                targets.Add(siteId, new ControlData(title));
            }
            return targets;
        }

        public Dictionary<string, ControlData> ProcessValidateInputSelectableOptions()
        {
            return ProcessValidateInputColumns()
                .ToDictionary(
                    column => column.ColumnName,
                    column => new ControlData(
                        text: column.LabelText,
                        attributes: ProcessValidateInputColumnAttributes(column: column)));
        }

        public Dictionary<string, ControlData> BulkProcessingItems(Context context, SiteSettings ss)
        {
            var items = Processes
                ?.Where(process => process.Accessable(
                    context: context,
                    ss: ss))
                .Where(process => process.GetAllowBulkProcessing())
                .ToDictionary(
                    process => process.Id.ToString(),
                    process => new ControlData(process.GetDisplayName()));
            return items?.Any() == true
                ? new Dictionary<string, ControlData>()
                {
                    {
                        string.Empty,
                        new ControlData($"({Displays.SelectBulkProcessing(context: context)})")
                    }
                }
                    .Concat(items)
                    .ToDictionary(o => o.Key, o => o.Value)
                : null;
        }

        private IEnumerable<Column> ProcessValidateInputColumns()
        {
            return Columns
                .Where(o => !o.NotUpdate)
                .Where(o => !o.Id_Ver)
                .Where(o => !o.Joined)
                .Where(o => !o.RecordedTime)
                .Where(o => o.ColumnName != "SiteId")
                .Where(o => o.ColumnName != "Creator")
                .Where(o => o.ColumnName != "Updator")
                .Where(o => o.ColumnName != "Comments")
                .Where(o => o.EditorColumn)
                .OrderBy(o => o.No);
        }

        private Dictionary<string, string> ProcessValidateInputColumnAttributes(Column column)
        {
            var data = new Dictionary<string, string>();
            data.Add("data-required", "1");
            switch (column.TypeName.CsTypeSummary())
            {
                case "string":
                    if (!column.ColumnName.StartsWith("Attachments"))
                    {
                        data.Add("data-string", "1");
                    }
                    break;
                case "numeric":
                    if (column.TypeName == "decimal")
                    {
                        data.Add("data-num", "1");
                    }
                    break;
            }
            return data;
        }

        public Dictionary<string, ControlData> StatusControlColumnHashOptions(
            Context context,
            Dictionary<string, StatusControl.ControlConstraintsTypes> columnHash)
        {
            return ColumnDefinitionHash.EditorDefinitions(context: context)
                .Where(o => o.ColumnName != "Comments"
                    && o.ColumnName != "Creator"
                    && o.ColumnName != "Updator"
                    && o.ColumnName != "CreatedTime"
                    && o.ColumnName != "UpdatedTime")
                .OrderBy(columnDefinition => columnDefinition.EditorColumn)
                .Select(columnDefinition => new
                {
                    Column = GetColumn(
                        context: context,
                        columnName: columnDefinition.ColumnName),
                    ControlType = columnHash.Get(columnDefinition.ColumnName)
                })
                .Where(o => o.Column != null)
                .Select(o => new
                {
                    Key = $"{o.Column.ColumnName},{o.ControlType}",
                    Text = o.ControlType == StatusControl.ControlConstraintsTypes.None
                        ? o.Column.LabelText
                        : $"{o.Column.LabelText}"
                })
                .ToDictionary(
                    o => o.Key,
                    o => new ControlData(text: o.Text));
        }

        public Dictionary<string, ControlData> FormulaTargetSelectableOptions(string calculationMethod)
        {
            return (string.IsNullOrEmpty(calculationMethod)
                || calculationMethod == FormulaSet.CalculationMethods.Default.ToString()
                    ? Columns
                        .Where(o => o.TypeName == "decimal")
                        .Where(o => !o.NotUpdate)
                        .Where(o => !o.Joined)
                    : Columns
                        .Where(o => o.ControlType != "Attachments")
                        .Where(o => !o.NotUpdate)
                        .Where(o => !o.Id_Ver)
                        .Where(o => !o.Joined)
                        .Where(o => !o.OtherColumn())
                        .Where(o => o.Name != "SiteId"
                            && o.Name != "Comments"))
                .OrderBy(o => o.No)
                .ToDictionary(o => o.ColumnName, o => new ControlData(o.LabelText));
        }

        public bool FormulaTarget(string columnName)
        {
            return Formulas?.Any(p => p.Target == columnName) == true;
        }

        public Dictionary<string, ControlData> ViewSelectableOptions()
        {
            return Views?.ToDictionary(o => o.Id.ToString(), o => new ControlData(o.Name));
        }

        public Dictionary<string, ControlData> FormulaCalculationMethodSelectableOptions(Context context)
        {
            return Enum.GetValues(typeof(FormulaSet.CalculationMethods))
               .Cast<FormulaSet.CalculationMethods>()
               .ToDictionary(
                    o => o.ToString(),
                    o => new ControlData(
                        text: Displays.Get(
                            context: context,
                            id: Enum.GetName(typeof(FormulaSet.CalculationMethods), o)),
                        attributes: new Dictionary<string, string>() { { "data-action", "SetSiteSettings" } }
                    )
                );
        }

        public Dictionary<string, ControlData> MonitorChangesSelectableOptions(
            Context context, IEnumerable<string> monitorChangesColumns, bool enabled = true)
        {
            return enabled
                ? ColumnUtilities.SelectableOptions(
                    context: context,
                    ss: this,
                    columns: monitorChangesColumns,
                    order: ColumnDefinitionHash?.MonitorChangesDefinitions()
                        .OrderBy(o => o.EditorColumn)
                        .Select(o => o.ColumnName).ToList())
                : ColumnUtilities.SelectableOptions(
                    context: context,
                    ss: this,
                    columns: ColumnDefinitionHash.MonitorChangesDefinitions()
                        .Where(o => !monitorChangesColumns.Contains(o.ColumnName))
                        .OrderBy(o => o.EditorColumn)
                        .Select(o => o.ColumnName),
                    order: ColumnDefinitionHash?.MonitorChangesDefinitions()?
                        .OrderBy(o => o.EditorColumn)
                        .Select(o => o.ColumnName).ToList());
        }

        public Dictionary<string, ControlData> ReminderColumnOptions()
        {
            return Columns
                .Where(o => o.TypeName == "datetime")
                .Where(o => !o.RecordedTime)
                .ToDictionary(o => o.ColumnName, o => new ControlData(o.LabelText));
        }

        public SiteSettings GetJoinedSs(string join)
        {
            return join?.Contains("~") == true
                ? JoinedSsHash.Get(join
                    .Split_1st(',')
                    .Split('-')
                    .Last()
                    .Split('~')
                    .Last()
                    .ToLong())
                : this;
        }

        public void ClearColumnAccessControlCaches(BaseModel baseModel)
        {
            baseModel.MineCache = null;
            ColumnAccessControlCaches.Values.ForEach(column =>
            {
                column.CanCreateCache = null;
                column.CanReadCache = null;
                column.CanUpdateCache = null;
            });
            ColumnAccessControlCaches.Clear();
        }

        public Dictionary<string, ControlData> ColumnAccessControlOptions(
            Context context,
            string type,
            IEnumerable<ColumnAccessControl> columnAccessControls = null)
        {
            return columnAccessControls != null
                ? columnAccessControls
                    .ToDictionary(o => o.ToJson(), o => o.ControlData(
                        context: context, ss: this, type: type))
                : ColumnAccessControl(type)
                    .OrderBy(o => o.No)
                    .ToDictionary(o => o.ToJson(), o => o.ControlData(
                        context: context, ss: this, type: type));
        }

        private List<ColumnAccessControl> ColumnAccessControl(string type)
        {
            Update_ColumnAccessControls();
            switch (type)
            {
                case "Create": return CreateColumnAccessControls;
                case "Read": return ReadColumnAccessControls;
                case "Update": return UpdateColumnAccessControls;
                default: return null;
            }
        }

        public bool ColumnAccessControlNullableOnly(string type)
        {
            switch (type)
            {
                case "Create": return true;
                default: return false;
            }
        }

        public Dictionary<string, string> PeriodOptions(Context context)
        {
            return new Dictionary<string, string>()
            {
                { "Days", Displays.Days(context: context) },
                { "Months", Displays.Months(context: context) },
                { "Years", Displays.Years(context: context) },
                { "Hours", Displays.Hours(context: context) },
                { "Minutes", Displays.Minutes(context: context) },
                { "Seconds", Displays.Seconds(context: context) }
            };
        }

        public Dictionary<string, string> AnalyPeriodOptions(Context context)
        {
            return new Dictionary<string, string>()
            {
                { "DaysAgoNoArgs", Displays.DaysAgoNoArgs(context: context) },
                { "MonthsAgoNoArgs", Displays.MonthsAgoNoArgs(context: context) },
                { "YearsAgoNoArgs", Displays.YearsAgoNoArgs(context: context) },
                { "HoursAgoNoArgs", Displays.HoursAgoNoArgs(context: context) },
                { "MinutesAgoNoArgs", Displays.MinutesAgoNoArgs(context: context) },
                { "SecondsAgoNoArgs", Displays.SecondsAgoNoArgs(context: context) }
            };
        }

        public Dictionary<string, ControlData> AggregationDestination(Context context)
        {
            return Aggregations?
                .GroupBy(o => o.Id)
                .Select(o => o.First())
                .ToDictionary(
                    o => o.Id.ToString(),
                    o => new ControlData((o.GroupBy == "[NotGroupBy]"
                        ? Displays.NoClassification(context: context)
                        : GetColumn(context: context, columnName: o.GroupBy)?.LabelText) +
                            " (" + Displays.Get(
                                context: context,
                                id: o.Type.ToString())
                                    + (o.Target != string.Empty
                                        ? ": " + GetColumn(
                                            context: context,
                                            columnName: o.Target)?
                                                .LabelText
                                        : string.Empty) + ")"));
        }

        public Dictionary<string, ControlData> AggregationSource(Context context)
        {
            var aggregationSource = new Dictionary<string, ControlData>
            {
                { "[NotGroupBy]", new ControlData(Displays.NoClassification(context: context)) }
            };
            return aggregationSource.AddRange(Def.ColumnDefinitionCollection
                .Where(o => o.TableName == ReferenceType)
                .Where(o => o.Aggregatable)
                .ToDictionary(
                    o => o.ColumnName,
                    o => new ControlData(GetColumn(
                        context: context, columnName: o.ColumnName).LabelText)));
        }

        public Dictionary<string, string> CalendarTimePeriodOptions(Context context)
        {
            return new Dictionary<string, string>
            {
                { "Yearly", Displays.Year(context: context) },
                { "Monthly", Displays.Month(context: context) },
                { "Weekly", Displays.Week(context: context) }
            };
        }

        public Dictionary<string, string> CalendarColumnOptions(Context context)
        {
            var hash = new Dictionary<string, string>();
            var startTime = GetColumn(context: context, columnName: "StartTime");
            var completionTime = GetColumn(context: context, columnName: "CompletionTime");
            if (startTime != null && completionTime != null)
            {
                hash.Add(
                    $"{startTime.ColumnName}-{completionTime.ColumnName}",
                    $"{startTime.GridLabelText} - {completionTime.GridLabelText}");
            }
            hash.AddRange(Columns
                .Where(o => o.TypeName == "datetime")
                .Where(o => !o.Joined)
                .Where(o => o.CanRead(
                    context: context,
                    ss: this,
                    mine: null))
                .OrderBy(o => o.No)
                .ToDictionary(o => o.ColumnName, o => o.GridLabelText));
            return hash;
        }

        public Dictionary<string, string> CalendarViewTypeOptions(Context context)
        {
            return new Dictionary<string, string>
            {
                { "dayGridMonth", Displays.Month(context: context) },
                { "timeGridWeek", Displays.Week(context: context) },
                { "timeGridDay", Displays.Day(context: context) },
                { "listMonth", Displays.List(context: context) }
            };
        }

        public Dictionary<string, string> CrosstabGroupByXOptions(Context context)
        {
            return CrosstabGroupByOptions(
                context: context,
                datetime: true);
        }

        public Dictionary<string, string> CrosstabGroupByYOptions(Context context)
        {
            var hash = CrosstabGroupByOptions(context: context);
            hash.Add("Columns", Displays.NumericColumn(context: context));
            return hash;
        }

        private Dictionary<string, string> CrosstabGroupByOptions(
            Context context, bool datetime = false)
        {
            var hash = new Dictionary<string, string>();
            JoinOptions(sources: false).ForEach(join =>
            {
                var siteId = ColumnUtilities.GetSiteIdByTableAlias(join.Key, SiteId);
                var ss = JoinedSsHash.Get(siteId);
                if (ss != null)
                {
                    hash.AddRange(ss.Columns
                        .Where(o => o.HasChoices() || (o.TypeName == "datetime" && datetime))
                        .Where(o => o.MultipleSelections != true)
                        .Where(o => !o.Joined)
                        .Where(o => ss.GetEditorColumnNames().Contains(o.Name))
                        .Where(o => o.CanRead(
                            context: context,
                            ss: ss,
                            mine: null))
                        .ToDictionary(
                            o => ColumnUtilities.ColumnName(join.Key, o.Name),
                            o => join.Value + " " + o.LabelText));
                    if (datetime)
                    {
                        hash.Add(
                            ColumnUtilities.ColumnName(join.Key, "CreatedTime"),
                            join.Value + " " + Displays.CreatedTime(context: context));
                        hash.Add(
                            ColumnUtilities.ColumnName(join.Key, "UpdatedTime"),
                            join.Value + " " + Displays.UpdatedTime(context: context));
                    }
                }
            });
            return hash;
        }

        public Dictionary<string, string> CrosstabColumnsOptions(Context context)
        {
            var hash = new Dictionary<string, string>();
            JoinOptions().ForEach(join =>
            {
                var siteId = ColumnUtilities.GetSiteIdByTableAlias(join.Key, SiteId);
                var ss = JoinedSsHash.Get(siteId);
                if (ss != null)
                {
                    hash.AddRange(ss.Columns
                        .Where(o => o.Computable)
                        .Where(o => o.TypeName != "datetime")
                        .Where(o => !o.Joined)
                        .Where(o => ss.GetEditorColumnNames().Contains(o.Name))
                        .Where(o => o.CanRead(
                            context: context,
                            ss: ss,
                            mine: null))
                        .OrderBy(o => o.LabelText)
                        .ToDictionary(
                            o => ColumnUtilities.ColumnName(join.Key, o.Name),
                            o => join.Value + " " + o.LabelText));
                }
            });
            return hash;
        }

        public Dictionary<string, string> CrosstabAggregationTypeOptions(Context context)
        {
            return new Dictionary<string, string>
            {
                { "Count", Displays.Count(context: context) },
                { "Total", Displays.Total(context: context) },
                { "Average", Displays.Average(context: context) },
                { "Max", Displays.Max(context: context) },
                { "Min", Displays.Min(context: context) }
            };
        }

        public Dictionary<string, string> CrosstabTimePeriodOptions(Context context)
        {
            return new Dictionary<string, string>
            {
                { "Yearly", Displays.Year(context: context) },
                { "Monthly", Displays.Month(context: context) },
                { "Weekly", Displays.Week(context: context) },
                { "Daily", Displays.Day(context: context) }
            };
        }

        public Dictionary<string, string> GanttGroupByOptions(Context context)
        {
            return Columns
                .Where(o => o.HasChoices())
                .Where(o => o.MultipleSelections != true)
                .Where(o => !o.Joined)
                .Where(o => o.CanRead(
                    context: context,
                    ss: this,
                    mine: null))
                .OrderBy(o => o.No)
                .ToDictionary(o => o.ColumnName, o => o.GridLabelText);
        }

        public Dictionary<string, string> CalendarGroupByOptions(Context context)
        {
            return Columns
                .Where(o => o.HasChoices())
                .Where(o => o.MultipleSelections != true)
                .Where(o => !o.Joined)
                .Where(o => o.CanRead(
                    context: context,
                    ss: this,
                    mine: null))
                .OrderBy(o => o.No)
                .ToDictionary(o => o.ColumnName, o => o.GridLabelText);
        }

        public Dictionary<string, string> GanttSortByOptions(Context context)
        {
            return GetEditorColumnNames()
                .Select(columnName => GetColumn(context: context, columnName: columnName))
                .Where(column => column != null)
                .Where(o => o.CanRead(
                    context: context,
                    ss: this,
                    mine: null))
                .ToDictionary(o => o.ColumnName, o => o.GridLabelText);
        }

        public Dictionary<string, string> TimeSeriesGroupByOptions(Context context)
        {
            return Columns
                .Where(o => o.HasChoices())
                .Where(o => o.MultipleSelections != true)
                .Where(o => !o.Joined)
                .Where(o => o.CanRead(
                    context: context,
                    ss: this,
                    mine: null))
                .OrderBy(o => o.No)
                .ToDictionary(o => o.ColumnName, o => o.LabelText);
        }

        public Dictionary<string, string> TimeSeriesAggregationTypeOptions(Context context)
        {
            return new Dictionary<string, string>
            {
                { "Count", Displays.Count(context: context) },
                { "Total", Displays.Total(context: context) },
                { "Average", Displays.Average(context: context) },
                { "Max", Displays.Max(context: context) },
                { "Min", Displays.Min(context: context) }
            };
        }

        public Dictionary<string, string> TimeSeriesValueOptions(Context context)
        {
            return Columns
                .Where(o => o.Computable)
                .Where(o => o.TypeName != "datetime")
                .Where(o => !o.Joined)
                .Where(o => o.CanRead(
                    context: context,
                    ss: this,
                    mine: null))
                .OrderBy(o => o.No)
                .ToDictionary(o => o.ColumnName, o => o.LabelText);
        }

        public Dictionary<string, string> TimeSeriesChartTypeOptions(Context context)
        {
            return new Dictionary<string, string>
            {
                { "AreaChart", Displays.AreaChart(context: context) },
                { "LineChart", Displays.LineChart(context: context) }
            };
        }

        public Dictionary<string, string> TimeSeriesHorizontalAxisOptions(Context context)
        {
            var hash = new Dictionary<string, string>
            {
                { "Histories", Displays.Histories(context: context) }
            };
            hash.AddRange(Columns
                .Where(o => o.TypeName == "datetime")
                .Where(o => !o.Joined)
                .Where(o => GetEditorColumnNames().Contains(o.Name))
                .Where(o => o.CanRead(
                    context: context,
                    ss: this,
                    mine: null))
                .OrderBy(o => o.No)
                .ToDictionary(o => o.ColumnName, o => o.GridLabelText));
            return hash;
        }

        public Dictionary<string, string> AnalyGroupByOptions(Context context)
        {
            return Columns
                .Where(o => o.HasChoices())
                .Where(o => o.MultipleSelections != true)
                .Where(o => !o.Joined)
                .Where(o => o.CanRead(
                    context: context,
                    ss: this,
                    mine: null))
                .OrderBy(o => o.No)
                .ToDictionary(o => o.ColumnName, o => o.LabelText);
        }

        public Dictionary<string, string> AnalyAggregationTypeOptions(Context context)
        {
            return new Dictionary<string, string>
            {
                { "Count", Displays.Count(context: context) },
                { "Total", Displays.Total(context: context) },
                { "Average", Displays.Average(context: context) },
                { "Max", Displays.Max(context: context) },
                { "Min", Displays.Min(context: context) }
            };
        }

        public Dictionary<string, string> AnalyAggregationTargetOptions(Context context)
        {
            var hash = new Dictionary<string, string>();
            JoinOptions().ForEach(join =>
            {
                var siteId = ColumnUtilities.GetSiteIdByTableAlias(join.Key, SiteId);
                var ss = JoinedSsHash.Get(siteId);
                if (ss != null)
                {
                    hash.AddRange(ss.Columns
                        .Where(o => o.Computable)
                        .Where(o => o.TypeName != "datetime")
                        .Where(o => !o.Joined)
                        .Where(o => ss.GetEditorColumnNames().Contains(o.Name))
                        .Where(o => o.CanRead(
                            context: context,
                            ss: ss,
                            mine: null))
                        .OrderBy(o => o.LabelText)
                        .ToDictionary(
                            o => ColumnUtilities.ColumnName(join.Key, o.Name),
                            o => join.Value + " " + o.LabelText));
                }
            });
            return hash;
        }

        public Dictionary<string, string> KambanGroupByOptions(
            Context context, bool addNothing = false)
        {
            var hash = new Dictionary<string, string>();
            if (addNothing)
            {
                hash.Add("Nothing", Displays.NoClassification(context: context));
            }
            hash.AddRange(Columns
                .Where(o => o.HasChoices())
                .Where(o => o.MultipleSelections != true)
                .Where(o => !o.Joined)
                .Where(o => o.CanRead(
                    context: context,
                    ss: this,
                    mine: null))
                .OrderBy(o => o.No)
                .ToDictionary(o => o.ColumnName, o => o.GridLabelText));
            return hash;
        }

        public Dictionary<string, string> KambanAggregationTypeOptions(Context context)
        {
            return new Dictionary<string, string>
            {
                { "Count", Displays.Count(context: context) },
                { "Total", Displays.Total(context: context) },
                { "Average", Displays.Average(context: context) },
                { "Max", Displays.Max(context: context) },
                { "Min", Displays.Min(context: context) }
            };
        }

        public Dictionary<string, string> KambanValueOptions(Context context)
        {
            return Columns
                .Where(o => o.Computable)
                .Where(o => o.TypeName != "datetime")
                .Where(o => !o.Joined)
                .Where(o => o.CanRead(
                    context: context,
                    ss: this,
                    mine: null))
                .OrderBy(o => o.No)
                .ToDictionary(o => o.ColumnName, o => o.GridLabelText);
        }

        public Dictionary<string, string> AutoNumberingColumnOptions(Context context)
        {
            return Columns
                .Where(o => o.TypeName == "nvarchar")
                .Where(o => o.ControlType != "Attachments")
                .Where(o => o.ColumnName != "Comments")
                .Where(o => o.ColumnName != "Timestamp")
                .Where(o => !o.Joined)
                .Where(o => o.CanRead(
                    context: context,
                    ss: this,
                    mine: null))
                .OrderBy(o => o.No)
                .ToDictionary(o => o.ColumnName, o => o.GridLabelText);
        }

        public Dictionary<string, string> JoinOptions(
            SiteSettings ss = null,
            bool destinations = true,
            bool sources = true,
            bool currentTableOnly = false)
        {
            var hash = new Dictionary<string, string>();
            if (ss == null)
            {
                ss = this;
                hash.Add(string.Empty, $"[{Title}]");
                // JOINされたテーブルを含まず現在のテーブルの項目のみ返却
                if (currentTableOnly)
                {
                    return hash;
                }
            }
            else
            {
                ss.JoinStacks.ForEach(joinStack =>
                    hash.AddIfNotConainsKey(
                        joinStack.TableName(),
                        joinStack.DisplayName(currentTitle: Title)));
            }
            if (destinations)
            {
                ss.Destinations?.Values.ForEach(currentSs =>
                    hash.AddRangeIfNotConainsKey(JoinOptions(ss: currentSs)));
            }
            if (sources)
            {
                ss.Sources?.Values.ForEach(currentSs =>
                    hash.AddRangeIfNotConainsKey(JoinOptions(ss: currentSs)));
            }
            return hash;
        }

        public List<Column> ReadableColumns(Context context, bool noJoined = false)
        {
            return Columns
                .Where(o => GetEditorColumnNames().Contains(o.ColumnName)
                    || GridColumns.Contains(o.ColumnName)
                    || !Def.ExtendedColumnTypes.ContainsKey(o.ColumnName))
                .Where(o => !noJoined || !o.Joined)
                .Where(o => o.CanRead(
                    context: context,
                    ss: this,
                    mine: null))
                .ToList();
        }

        public int GridNextOffset(int offset, int count, int totalCount)
        {
            return offset + count < totalCount
                ? offset + GridPageSize.ToInt()
                : -1;
        }

        public int ImageLibNextOffset(int offset, int count, int totalCount)
        {
            return offset + count < totalCount
                ? offset + ImageLibPageSize.ToInt()
                : -1;
        }

        public bool ShowComments(Permissions.ColumnPermissionTypes columnPermissionType)
        {
            return
                GetEditorColumnNames()?.Contains("Comments") == true &&
                columnPermissionType != Permissions.ColumnPermissionTypes.Deny;
        }

        public void Set(Context context, string propertyName, string value)
        {
            switch (propertyName)
            {
                case "DisableSiteConditions": DisableSiteConditions = value.ToBool(); break;
                case "GuideAllowExpand": GuideAllowExpand = value.ToBool(); break;
                case "GuideExpand": GuideExpand = value; break;
                case "NearCompletionTimeBeforeDays": NearCompletionTimeBeforeDays = value.ToInt(); break;
                case "NearCompletionTimeAfterDays": NearCompletionTimeAfterDays = value.ToInt(); break;
                case "GridPageSize": GridPageSize = value.ToInt(); break;
                case "GridView": GridView = value.ToInt(); break;
                case "AllowViewReset": AllowViewReset = value.ToBool(); break;
                case "GridEditorType": GridEditorType = (GridEditorTypes)value.ToInt(); break;
                case "HistoryOnGrid": HistoryOnGrid = value.ToBool(); break;
                case "AlwaysRequestSearchCondition": AlwaysRequestSearchCondition = value.ToBool(); break;
                case "DisableLinkToEdit": DisableLinkToEdit = value.ToBool(); break;
                case "OpenEditInNewTab": OpenEditInNewTab = value.ToBool(); break;
                case "EnableExpandLinkPath": EnableExpandLinkPath = value.ToBool(); break;
                case "LinkTableView": LinkTableView = value.ToInt(); break;
                case "FirstDayOfWeek": FirstDayOfWeek = value.ToInt(); break;
                case "FirstMonth": FirstMonth = value.ToInt(); break;
                case "Responsive": Responsive = value.ToBool(); break;
                case "AsynchronousLoadingDefault": DashboardPartsAsynchronousLoading = value.ToBool(); break;
                case "AutoVerUpType": AutoVerUpType = (Versions.AutoVerUpTypes)value.ToInt(); break;
                case "AllowCopy": AllowCopy = value.ToBool(); break;
                case "AllowReferenceCopy": AllowReferenceCopy = value.ToBool(); break;
                case "CharToAddWhenCopying": CharToAddWhenCopying = value; break;
                case "AllowEditingComments": AllowEditingComments = value.ToBool(); break;
                case "AllowSeparate": AllowSeparate = value.ToBool(); break;
                case "AllowLockTable": AllowLockTable = value.ToBool(); break;
                case "AllowRestoreHistories": AllowRestoreHistories = value.ToBool(); break;
                case "AllowPhysicalDeleteHistories": AllowPhysicalDeleteHistories = value.ToBool(); break;
                case "HideLink": HideLink = value.ToBool(); break;
                case "SwitchRecordWithAjax": SwitchRecordWithAjax = value.ToBool(); break;
                case "SwitchCommandButtonsAutoPostBack": SwitchCommandButtonsAutoPostBack = value.ToBool(); break;
                case "DeleteImageWhenDeleting": DeleteImageWhenDeleting = value.ToBool(); break;
                case "ImportEncoding": ImportEncoding = value; break;
                case "UpdatableImport": UpdatableImport = value.ToBool(); break;
                case "RejectNullImport": RejectNullImport = value.ToBool(); break;
                case "DefaultImportKey": DefaultImportKey = value; break;
                case "AllowStandardExport": AllowStandardExport = value.ToBool(); break;
                case "EnableCalendar": EnableCalendar = value.ToBool(); break;
                case "CalendarType": CalendarType = value.ToEnum(defaultValue: (CalendarTypes)Parameters.General.DefaultCalendarType); break;
                case "EnableCrosstab": EnableCrosstab = value.ToBool(); break;
                case "NoDisplayCrosstabGraph": NoDisplayCrosstabGraph = value.ToBool(); break;
                case "EnableGantt": EnableGantt = value.ToBool(); break;
                case "ShowGanttProgressRate": ShowGanttProgressRate = value.ToBool(); break;
                case "EnableBurnDown": EnableBurnDown = value.ToBool(); break;
                case "EnableTimeSeries": EnableTimeSeries = value.ToBool(); break;
                case "EnableAnaly": EnableAnaly = value.ToBool(); break;
                case "EnableKamban": EnableKamban = value.ToBool(); break;
                case "EnableImageLib": EnableImageLib = value.ToBool(); break;
                case "UseFilterButton": UseFilterButton = value.ToBool(); break;
                case "UseFiltersArea": UseFiltersArea = value.ToBool(); break;
                case "UseGridHeaderFilters": UseGridHeaderFilters = value.ToBool(); break;
                case "UseNegativeFilters": UseNegativeFilters = value.ToBool(); break;
                case "UseRelatingColumnsOnFilter": UseRelatingColumnsOnFilter = value.ToBool(); break;
                case "UseIncompleteFilter": UseIncompleteFilter = value.ToBool(); break;
                case "UseOwnFilter": UseOwnFilter = value.ToBool(); break;
                case "UseNearCompletionTimeFilter": UseNearCompletionTimeFilter = value.ToBool(); break;
                case "UseDelayFilter": UseDelayFilter = value.ToBool(); break;
                case "UseOverdueFilter": UseOverdueFilter = value.ToBool(); break;
                case "UseSearchFilter": UseSearchFilter = value.ToBool(); break;
                case "OutputFormulaLogs": OutputFormulaLogs = value.ToBool(); break;
                case "ImageLibPageSize": ImageLibPageSize = value.ToInt(); break;
                case "SearchType": SearchType = (SearchTypes)value.ToInt(); break;
                case "FullTextIncludeBreadcrumb": FullTextIncludeBreadcrumb = value.ToBool(); break;
                case "FullTextIncludeSiteId": FullTextIncludeSiteId = value.ToBool(); break;
                case "FullTextIncludeSiteTitle": FullTextIncludeSiteTitle = value.ToBool(); break;
                case "FullTextNumberOfMails": FullTextNumberOfMails = value.ToInt(); break;
                case "SaveViewType": SaveViewType = (SaveViewTypes)value.ToInt(); break;
                case "AddressBook": AddressBook = value; break;
                case "MailToDefault": MailToDefault = value; break;
                case "MailCcDefault": MailCcDefault = value; break;
                case "MailBccDefault": MailBccDefault = value; break;
                case "IntegratedSites": SetIntegratedSites(value); break;
                case "NoDisplayIfReadOnly": NoDisplayIfReadOnly = value.ToBool(); break;
                case "NotInheritPermissionsWhenCreatingSite": NotInheritPermissionsWhenCreatingSite = value.ToBool(); break;
                case "CurrentPermissionForCreatingAll": SetPermissionForCreating(value); break;
                case "CurrentPermissionForUpdatingAll": SetPermissionForUpdating(value); break;
                case "CreateColumnAccessControlAll": SetCreateColumnAccessControl(value); break;
                case "ReadColumnAccessControlAll": SetReadColumnAccessControl(value); break;
                case "UpdateColumnAccessControlAll": SetUpdateColumnAccessControl(value); break;
                case "GridColumnsAll": GridColumns = context.Forms.List(propertyName); break;
                case "FilterColumnsAll": FilterColumns = context.Forms.List(propertyName); break;
                case "AggregationDestinationAll": Aggregations = GetAggregations(context: context); break;
                case "EditorColumnsAll":
                    AddOrUpdateEditorColumnHash(
                        editorColumnsTabsTarget: context
                            .Forms
                            .Data(key: "EditorColumnsTabsTarget"),
                        editorColumnsAll: context.Forms.List(propertyName));
                    Sections = EditorColumnHash
                        .SelectMany(o => o
                            .Value?
                            .Select(columnName => SectionId(columnName))
                            .Where(sectionId => sectionId != 0))
                        .Select(sectionId => new Section
                        {
                            Id = sectionId,
                            LabelText = Sections?
                                .FirstOrDefault(section => section.Id == sectionId)
                                ?.LabelText
                                    ?? Displays.Section(context: context),
                            AllowExpand = Sections?
                                .FirstOrDefault(section => section.Id == sectionId)
                                ?.AllowExpand
                                    ?? false,
                            Expand = Sections?
                                .FirstOrDefault(section => section.Id == sectionId)
                                ?.Expand
                                    ?? true
                        })
                        .ToList();
                    break;
                case "TabsAll":
                    Tabs = Tabs?.Join(context.Forms.List(propertyName).Select((val, key) => new { Key = key, Val = val }), v => v.Id, l => l.Val.ToInt(),
                        (v, l) => new { Tabs = v, OrderNo = l.Key })
                        .OrderBy(v => v.OrderNo)
                        .Select(v => v.Tabs)
                        .Aggregate(new SettingList<Tab>(), (list, data) => { list.Add(data); return list; });
                    break;
                case "SectionsAll":
                    Sections = Sections?.Join(context.Forms.List(propertyName).Select((val, key) => new { Key = key, Val = val }), v => v.Id, l => l.Val.ToInt(),
                        (v, l) => new { Sections = v, OrderNo = l.Key })
                        .OrderBy(v => v.OrderNo)
                        .Select(v => v.Sections)
                        .ToList();
                    break;
                case "TitleColumnsAll": TitleColumns = context.Forms.List(propertyName); break;
                case "LinkColumnsAll": LinkColumns = context.Forms.List(propertyName); break;
                case "HistoryColumnsAll": HistoryColumns = context.Forms.List(propertyName); break;
                case "MoveTargetsColumnsAll": MoveTargets = context.Forms.LongList(propertyName); break;
                case "ViewsAll":
                    Views = Views?.Join(context.Forms.List(propertyName).Select((val, key) => new { Key = key, Val = val }), v => v.Id, l => l.Val.ToInt(),
                        (v, l) => new { Views = v, OrderNo = l.Key })
                        .OrderBy(v => v.OrderNo)
                        .Select(v => v.Views)
                        .ToList();
                    break;
                case "ProcessOutputFormulaLogs": ProcessOutputFormulaLogs = value.ToBool(); break;
                case "ServerScriptsAllDisabled": ServerScriptsAllDisabled = value.ToBool(); break;
                case "ScriptsAllDisabled": ScriptsAllDisabled = value.ToBool(); break;
                case "StylesAllDisabled": StylesAllDisabled = value.ToBool(); break;
                case "HtmlsAllDisabled": HtmlsAllDisabled = value.ToBool(); break;
            }
        }

        public List<Aggregation> GetAggregations(Context context)
        {
            var aggregations = new List<Aggregation>();
            context.Forms.List("AggregationDestinationAll").ForEach(a =>
            {
                var aggrigation = Aggregations
                    .Where(o => o.Id == a.ToInt())
                    .FirstOrDefault();
                if (aggrigation != null)
                {
                    aggregations.Add(new Aggregation()
                    {
                        Id = aggrigation.Id,
                        GroupBy = aggrigation.GroupBy,
                        Type = aggrigation.Type,
                        Target = aggrigation.Target,
                        Data = aggrigation.Data
                    });
                }
            });
            return aggregations;
        }

        public List<string> SetAggregations(
            string controlId,
            List<string> selected,
            List<string> selectedSources)
        {
            switch (controlId)
            {
                case "AddAggregations":
                    var added = new List<string>();
                    selectedSources.ForEach(groupBy =>
                    {
                        var id = Aggregations.Count > 0
                            ? Aggregations.Max(o => o.Id) + 1
                            : 1;
                        added.Add(id.ToString());
                        Aggregations.Add(new Aggregation(id, groupBy));
                    });
                    return added;
                case "DeleteAggregations":
                    Aggregations.RemoveAll(o =>
                        selected.Contains(o.Id.ToString()));
                    return null;
                default:
                    return selected;
            }
        }

        public void SetAggregationDetails(
            Aggregation.Types type,
            string target,
            IEnumerable<string> selectedColumns)
        {
            Aggregations
                .Where(o => selectedColumns.Contains(o.Id.ToString()))
                .ForEach(aggregation =>
                {
                    aggregation.Type = type;
                    aggregation.Target = target;
                });
        }

        public void SetColumnProperty(
            Context context, Column column, string propertyName, string value)
        {
            switch (propertyName)
            {
                case "ColumnName": column.ColumnName = value; break;
                case "LabelText":
                    var labelText = column.LabelText;
                    column.LabelText = value;
                    if (column.GridLabelText == labelText)
                    {
                        column.GridLabelText = value;
                    }
                    break;
                case "GridLabelText": column.GridLabelText = value; break;
                case "ControlType": column.ControlType = value; break;
                case "ChoicesControlType": column.ChoicesControlType = value; break;
                case "Format": column.Format = value; break;
                case "NoWrap": column.NoWrap = value.ToBool(); break;
                case "Hide": column.Hide = value.ToBool(); break;
                case "AutoNumberingFormat":
                    column.AutoNumberingFormat = LabelTextToColumnName(
                        column: column,
                        value: value);
                    break;
                case "AutoNumberingResetType": column.AutoNumberingResetType = (Column.AutoNumberingResetTypes)value.ToInt(); break;
                case "AutoNumberingDefault": column.AutoNumberingDefault = value.ToInt(); break;
                case "AutoNumberingStep": column.AutoNumberingStep = value.ToInt(); break;
                case "ExtendedCellCss": column.ExtendedCellCss = value; break;
                case "ExtendedFieldCss": column.ExtendedFieldCss = value; break;
                case "ExtendedControlCss": column.ExtendedControlCss = value; break;
                case "Section": column.Section = value; break;
                case "GridDesign":
                    column.GridDesign = LabelTextToColumnName(
                        column: column,
                        value: value);
                    break;
                case "ValidateRequired": column.ValidateRequired = value.ToBool(); break;
                case "ValidateNumber": column.ValidateNumber = value.ToBool(); break;
                case "ValidateDate": column.ValidateDate = value.ToBool(); break;
                case "ValidateEmail": column.ValidateEmail = value.ToBool(); break;
                case "ValidateEqualTo": column.ValidateEqualTo = value.ToString(); break;
                case "ValidateMaxLength": column.ValidateMaxLength = value.ToInt(); break;
                case "MaxLength": column.MaxLength = value.ToDecimal(); break;
                case "ClientRegexValidation": column.ClientRegexValidation = value; break;
                case "ServerRegexValidation": column.ServerRegexValidation = value; break;
                case "RegexValidationMessage": column.RegexValidationMessage = value; break;
                case "ExtendedHtmlBeforeField": column.ExtendedHtmlBeforeField = value; break;
                case "ExtendedHtmlBeforeLabel": column.ExtendedHtmlBeforeLabel = value; break;
                case "ExtendedHtmlBetweenLabelAndControl": column.ExtendedHtmlBetweenLabelAndControl = value; break;
                case "ExtendedHtmlAfterControl": column.ExtendedHtmlAfterControl = value; break;
                case "ExtendedHtmlAfterField": column.ExtendedHtmlAfterField = value; break;
                case "Nullable": column.Nullable = value.ToBool(); break;
                case "Unit": column.Unit = value; break;
                case "DecimalPlaces": column.DecimalPlaces = value.ToInt(); break;
                case "RoundingType": column.RoundingType = (RoundingTypes)value.ToInt(); break;
                case "Max": column.Max = value.ToDecimal(); break;
                case "Min": column.Min = value.ToDecimal(); break;
                case "Step": column.Step = value.ToDecimal(); break;
                case "NoDuplication": column.NoDuplication = value.ToBool(); break;
                case "MessageWhenDuplicated": column.MessageWhenDuplicated = value; break;
                case "CopyByDefault": column.CopyByDefault = value.ToBool(); break;
                case "EditorReadOnly": column.EditorReadOnly = value.ToBool(); break;
                case "AutoPostBack": column.AutoPostBack = value.ToBool(); break;
                case "ColumnsReturnedWhenAutomaticPostback": column.ColumnsReturnedWhenAutomaticPostback = value; break;
                case "AllowBulkUpdate": column.AllowBulkUpdate = value.ToBool(); break;
                case "AllowDeleteAttachments": column.AllowDeleteAttachments = value.ToBool(); break;
                case "NotDeleteExistHistory": column.NotDeleteExistHistory = value.ToBool(); break;
                case "AllowImage": column.AllowImage = value.ToBool(); break;
                case "ThumbnailLimitSize": column.ThumbnailLimitSize = value.ToDecimal(); break;
                case "FieldCss": column.FieldCss = value; break;
                case "ViewerSwitchingType": column.ViewerSwitchingType = (Column.ViewerSwitchingTypes)value.ToInt(); break;
                case "TextAlign": column.TextAlign = (TextAlignTypes)value.ToInt(); break;
                case "Description": column.Description = value; break;
                case "InputGuide": column.InputGuide = value; break;
                case "ChoicesText": column.ChoicesText = value; SetLinks(context: context, column: column); break;
                case "UseSearch": column.UseSearch = value.ToBool(); break;
                case "MultipleSelections": column.MultipleSelections = value.ToBool(); break;
                case "NotInsertBlankChoice": column.NotInsertBlankChoice = value.ToBool(); break;
                case "DefaultInput": column.DefaultInput = value; break;
                case "ImportKey": column.ImportKey = value.ToBool(); break;
                case "Anchor": column.Anchor = value.ToBool(); break;
                case "OpenAnchorNewTab": column.OpenAnchorNewTab = value.ToBool(); break;
                case "AnchorFormat": column.AnchorFormat = value; break;
                case "GridFormat": column.GridFormat = value; break;
                case "EditorFormat": column.EditorFormat = value; break;
                case "ExportFormat": column.ExportFormat = value; break;
                case "CheckFilterControlType": column.CheckFilterControlType = (ColumnUtilities.CheckFilterControlTypes)value.ToInt(); break;
                case "NumFilterMin": column.NumFilterMin = value.ToDecimal(); break;
                case "NumFilterMax": column.NumFilterMax = value.ToDecimal(); break;
                case "NumFilterStep": column.NumFilterStep = value.ToDecimal(); break;
                case "DateFilterSetMode": column.DateFilterSetMode = (ColumnUtilities.DateFilterSetMode)value.ToInt(); break;
                case "SearchTypes": column.SearchType = (Column.SearchTypes)value.ToInt(); break;
                case "FullTextTypes": column.FullTextType = (Column.FullTextTypes)value.ToInt(); break;
                case "DateFilterMinSpan": column.DateFilterMinSpan = value.ToInt(); break;
                case "DateFilterMaxSpan": column.DateFilterMaxSpan = value.ToInt(); break;
                case "DateFilterFy": column.DateFilterFy = value.ToBool(); break;
                case "DateFilterHalf": column.DateFilterHalf = value.ToBool(); break;
                case "DateFilterQuarter": column.DateFilterQuarter = value.ToBool(); break;
                case "DateFilterMonth": column.DateFilterMonth = value.ToBool(); break;
                case "OverwriteSameFileName": column.OverwriteSameFileName = value.ToBool(); break;
                case "LimitQuantity": column.LimitQuantity = value.ToDecimal(); break;
                case "LimitSize": column.LimitSize = value.ToDecimal(); break;
                case "LimitTotalSize": column.TotalLimitSize = value.ToDecimal(); break;
                case "TitleSeparator": TitleSeparator = value; break;
                case "BinaryStorageProvider": column.BinaryStorageProvider = value; break;
                case "LocalFolderLimitSize": column.LocalFolderLimitSize = value.ToDecimal(); break;
                case "LocalFolderLimitTotalSize": column.LocalFolderTotalLimitSize = value.ToDecimal(); break;
                case "DateTimeStep": column.DateTimeStep = value.ToInt(); break;
            }
        }

        private string LabelTextToColumnName(Column column, string value)
        {
            if (!value.IsNullOrEmpty())
            {
                value = LabelTextToColumnName(value);
                return value != $"[{column.ColumnName}]"
                    ? value
                    : null;
            }
            else
            {
                return null;
            }
        }

        public string LabelTextToColumnName(string text)
        {
            IncludedColumns(text, labelText: true).ForEach(column =>
                text = text.Replace($"[{column.LabelText}]", $"[{column.ColumnName}]"));
            return text;
        }

        public string GridDesignEditorText(Column column)
        {
            return column.GridDesign.IsNullOrEmpty()
                ? LabelTextBracket(column)
                : ColumnNameToLabelText(column.GridDesign);
        }

        public string LabelTextBracket(Column column)
        {
            return $"[{column.LabelText}]";
        }

        public string ColumnNameToLabelText(string text)
        {
            IncludedColumns(text).ForEach(column =>
                text = text.Replace(
                    $"[{column.ColumnName}]", $"[{column.LabelText}]"));
            return text;
        }

        public List<string> IncludedColumns()
        {
            return IncludedColumns(Columns
                .Where(o => !o.GridDesign.IsNullOrEmpty())
                .Select(o => o.GridDesign)
                .Join(string.Empty))
                    .Select(o => o.ColumnName)
                    .ToList();
        }

        public List<Column> IncludedColumns(string value, bool labelText = false)
        {
            var columns = new List<Column>();
            if (!value.IsNullOrEmpty())
            {
                foreach (Match match in value.RegexMatches(@"(?<=\[).+?(?=\])"))
                {
                    var isValue = false;
                    var columnName = match.Value;
                    // 値の要求(Column.OutputTypes.Value)があるかチェック
                    if (!labelText && columnName.StartsWith("@"))
                    {
                        isValue = true;
                        columnName = columnName.Substring(1);
                    }
                    var column = labelText
                        ? Columns.FirstOrDefault(o =>
                            o.LabelText == columnName)
                        : Columns.FirstOrDefault(o =>
                            o.ColumnName == columnName);
                    if (column != null)
                    {
                        column.OutputType = isValue
                            ? Column.OutputTypes.Value
                            : Column.OutputTypes.DisplayValue;
                        columns.Add(column);
                    }
                }
            }
            return columns
                .DistinctBy(column => column.ColumnName)
                .ToList();
        }

        public List<Link> GetUseSearchLinks(Context context)
        {
            return Links?
                .Where(o => o.SiteId > 0)
                .Where(o => GetColumn(
                    context: context,
                    columnName: o.ColumnName)?
                        .UseSearch == true)
                .ToList();
        }

        public void SetLinks(Context context)
        {
            Columns
                ?.Where(column => column.HasChoices())
                .ForEach(column => SetLinks(
                    context: context,
                    column: column));
        }

        public void SetLinks(Context context, Column column)
        {
            column.Link = false;
            Links.RemoveAll(o => o.ColumnName == column.ColumnName);
            var links = column.ChoicesText.Deserialize<List<Link>>();
            if (links != null)
            {
                links.ForEach(link =>
                {
                    link.ColumnName = column.ColumnName;
                    link.JsonFormat = true;
                });
                Links.AddRange(links);
            }
            else
            {
                column.ChoicesText.SplitReturn()
                    .Select(o => o.Trim())
                    .Where(o => o.RegexExists(@"^\[\[.+\]\]$"))
                    .Select(settings => new Link(
                        columnName: column.ColumnName,
                        settings: settings))
                    .Where(link => link.SiteId > 0)
                    .ForEach(link =>
                    {
                        column.Link = true;
                        if (!Links
                            .Where(o => o.SiteId > 0)
                            .Any(o => o.ColumnName == column.ColumnName
                                && o.SiteId == link.SiteId))
                        {
                            if (new SiteModel(
                                context: context,
                                siteId: link.SiteId)
                                    .AccessStatus == Databases.AccessStatuses.Selected)
                            {
                                Links.Add(link);
                            }
                        }
                    });
            }
        }

        public void SetChoiceHash(
            Context context,
            EnumerableRowCollection<DataRow> dataRows)
        {
            var dataColumns = dataRows.Columns()
                .Where(columnName => columnName.EndsWith(",ItemTitle"))
                .ToList();
            Columns
                .Where(column => column.Linked(context: context))
                .Where(column => !column.ColumnName.Contains("~~"))
                .ForEach(column =>
                {
                    if (column.ChoiceHash == null)
                    {
                        column.ChoiceHash = new Dictionary<string, Choice>();
                    }
                    var link = column.SiteSettings.Links
                        .Where(o => o.SiteId > 0)
                        .Where(o => column.Name == o.ColumnName)
                        .FirstOrDefault(o => dataColumns.Any(p =>
                            p.EndsWith(o.LinkedTableName() + ",ItemTitle")));
                    if (link != null)
                    {
                        dataRows
                            .GroupBy(o => o.Long(column.ColumnName))
                            .Select(o => o.First())
                            .Select(dataRow => LinkedChoice(
                                context: context,
                                column: column,
                                dataColumns: dataColumns,
                                dataRow: dataRow,
                                link: link))
                            .Where(choice => choice != null)
                            .ForEach(choice =>
                                column.ChoiceHash.AddOrUpdate(
                                    choice.Value,
                                    choice));
                    }
                });
        }

        private Choice LinkedChoice(
            Context context,
            Column column,
            List<string> dataColumns,
            DataRow dataRow,
            Link link)
        {
            var linkedColumnName = dataColumns.FirstOrDefault(o =>
                o.EndsWith(link.LinkedTableName() + ",ItemTitle"));
            if (linkedColumnName != null
                && dataRow[linkedColumnName] != DBNull.Value)
            {
                if (Permissions.CanRead(
                    context: context,
                    siteId: link.SiteId,
                    id: dataRow.Long(column.ColumnName)))
                {
                    var choice = new Choice(
                        value: dataRow.String(column.ColumnName),
                        text: dataRow.String(linkedColumnName));
                    return choice;
                }
            }
            return null;
        }

        public string LinkedItemTitle(
            Context context, long referenceId, List<long> siteIdList)
        {
            var dataRows = LinkedItemTitles(
                context: context,
                idList: referenceId.ToSingleList(),
                siteIdList: siteIdList);
            return dataRows.Any()
                ? dataRows.First().String("Title")
                : null;
        }

        private EnumerableRowCollection<DataRow> LinkedItemTitles(
            Context context, List<long> idList, List<long> siteIdList = null)
        {
            return Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectItems(
                    tableType: Sqls.TableTypes.NormalAndDeleted,
                    column: Rds.ItemsColumn()
                        .ReferenceId()
                        .Title(),
                    join: new SqlJoinCollection(
                        new SqlJoin(
                            tableBracket: "\"Sites\"",
                            joinType: SqlJoin.JoinTypes.Inner,
                            joinExpression: "\"Items\".\"SiteId\"=\"Sites\".\"SiteId\"")),
                    where: Rds.ItemsWhere()
                        .ReferenceId_In(idList)
                        .SiteId_In(siteIdList, _using: siteIdList != null)
                        .CanRead(context: context, idColumnBracket: "\"Items\".\"ReferenceId\"")))
                            .AsEnumerable();
        }

        public void SetChoiceHash(Context context, bool withLink = true, bool all = false)
        {
            SetAllChoices = all;
            if (JoinedSsHash == null)
            {
                SetChoiceHash(
                    context: context,
                    columnName: null,
                    searchIndexes: null,
                    selectedValues: null,
                    linkHash: null);
            }
            else
            {
                var siteIdList = JoinedSsHash
                    ?.SelectMany(o => o.Value.Links
                        .Where(p => p.JsonFormat != true)
                        .Where(p => p.SiteId > 0)
                        .Select(p => p.SiteId))
                    .Distinct()
                    .ToList() ?? new List<long>();
                var linkHash = withLink
                    ? LinkHash(
                        context: context,
                        siteIdList: siteIdList,
                        all: all,
                        searchIndexes: null)
                    : null;
                // 自己参照の際にカレントテーブルのSetChoiceHashが実行されない問題へ対応
                if (Destinations.ContainsKey(SiteId))
                {
                    SetChoiceHash(
                        context: context,
                        columnName: null,
                        searchIndexes: null,
                        selectedValues: null,
                        linkHash: linkHash);
                }
                // リンクのツリーを再帰的にJoinedSsHashに追加していくと現在のサイトの直接のリンク先が
                // JoinedSsHashに含まれないケースがあるため、Destinationsのインスタンスで置き換える
                // 処理を追加した。DestinationsのサイトはSearchFormatを表示するためにSetChoiceHashが
                // 必要だが、今後はChoiceHashが必要になったタイミングでSetChoiceHashを実施する方式に
                // 変更していく
                Destinations.ForEach(data =>
                    JoinedSsHash[data.Key] = data.Value);
                JoinedSsHash.ForEach(data => data.Value.SetChoiceHash(
                    context: context,
                    columnName: null,
                    searchIndexes: null,
                    selectedValues: null,
                    linkHash: linkHash));
            }
        }

        public void SetChoiceHash(
            Context context,
            string columnName,
            List<string> searchIndexes = null,
            IEnumerable<string> selectedValues = null,
            bool setTotalCount = false,
            bool searchColumnOnly = true)
        {
            SetChoiceHash(
                context: context,
                columnName: columnName,
                searchIndexes: searchIndexes,
                selectedValues: selectedValues,
                linkHash: LinkHash(
                    context: context,
                    columnName: columnName,
                    searchIndexes: searchIndexes,
                    searchColumnOnly: searchColumnOnly,
                    selectedValues: selectedValues,
                    setTotalCount: setTotalCount));
        }

        public void SetChoiceHash(
            Context context,
            string columnName,
            List<string> searchIndexes,
            IEnumerable<string> selectedValues,
            Dictionary<string, List<string>> linkHash)
        {
            var columns = new List<Column>();
            Columns?
                .Where(o => o.HasChoices())
                .Where(o => !o.AddChoiceHashByServerScript)
                .Where(o => columnName == null || o.ColumnName == columnName)
                .ForEach(column =>
                {
                    var key = column.ChoiceHashKey();
                    var same = columns.FirstOrDefault(o => o.ChoiceHashKey() == key);
                    if (same == null)
                    {
                        columns.Add(column);
                    }
                    else
                    {
                        column.ChoiceHash = same.ChoiceHash;
                    }
                    var link = Links?
                        .Where(o => o.JsonFormat == true)
                        .FirstOrDefault(o => o.ColumnName == column.ColumnName);
                    if (link != null)
                    {
                        column.SetChoiceHash(
                            context: context,
                            ss: this,
                            link: link,
                            searchText: searchIndexes?.Join(" "),
                            selectedValues: selectedValues,
                            setAllChoices: SetAllChoices,
                            setChoices: same == null);
                    }
                    else
                    {
                        column.SetChoiceHash(
                            context: context,
                            siteId: InheritPermission,
                            linkHash: linkHash,
                            searchIndexes: searchIndexes,
                            setAllChoices: SetAllChoices,
                            setChoices: same == null);
                    }
                });
        }

        private Dictionary<string, List<string>> LinkHash(
            Context context,
            List<long> siteIdList,
            bool all,
            List<string> searchIndexes)
        {
            var notUseSearchSiteIdList = Links
                .Where(o => o.JsonFormat != true)
                .Where(o => o.SiteId > 0)
                .Where(o => siteIdList.Contains(o.SiteId))
                .Where(o => Columns.Any(p =>
                    p.ColumnName == o.ColumnName
                    && p.UseSearch != true))
                .Select(o => o.SiteId)
                .Distinct()
                .ToList();
            var dataSet = siteIdList.Any()
                ? Repository.ExecuteDataSet(
                    context: context,
                    statements: siteIdList.Select(siteId =>
                        Rds.SelectItems(
                            dataTableName: siteId.ToString(),
                            column: Rds.ItemsColumn()
                                .ReferenceId()
                                .ReferenceType()
                                .SiteId()
                                .Title(),
                            join: new SqlJoinCollection(
                                new SqlJoin(
                                    tableBracket: "\"Sites\"",
                                    joinType: SqlJoin.JoinTypes.Inner,
                                    joinExpression: "\"Items\".\"SiteId\"=\"Sites\".\"SiteId\"")),
                            where: Rds.ItemsWhere()
                                .ReferenceType("Sites", _operator: "<>")
                                .SiteId(siteId)
                                .CanRead(
                                    context: context,
                                    idColumnBracket: "\"Items\".\"ReferenceId\"",
                                    _using: !context.Publish)
                                .Add(
                                    or: Rds.ItemsWhere()
                                        .ReferenceType(raw: "'Wikis'")
                                        .SiteId_In(notUseSearchSiteIdList),
                                    _using: !all),
                            orderBy: Rds.ItemsOrderBy().Title(),
                            top: all
                                ? 0
                                : Parameters.General.DropDownSearchPageSize)).ToArray())
                : null;
            return siteIdList.ToDictionary(
                siteId => $"[[{siteId}]]",
                siteId => LinkValue(
                    context: context,
                    siteId: siteId,
                    dataRows: dataSet.Tables[siteId.ToString()].AsEnumerable(),
                    searchIndexes: searchIndexes));
        }

        public Dictionary<string, List<string>> LinkHash(
            Context context,
            string columnName,
            List<string> searchIndexes = null,
            bool searchColumnOnly = true,
            IEnumerable<string> selectedValues = null,
            int offset = 0,
            string parentClass = "",
            IEnumerable<long> parentIds = null,
            bool setTotalCount = false)
        {
            var hash = new Dictionary<string, List<string>>();
            Links?
                .Where(o => o.JsonFormat != true)
                .Where(o => o.SiteId > 0)
                .Where(o => o.ColumnName == columnName)
                .Where(o => !searchColumnOnly
                    || GetColumn(
                        context: context,
                        columnName: o.ColumnName)?
                            .UseSearch == true)
                .GroupBy(o => o.SiteId)
                .Select(o => o.FirstOrDefault())
                .ForEach(link => LinkHash(
                    context: context,
                    searchIndexes: searchIndexes,
                    selectedValues: selectedValues?.Select(o => o.ToLong()),
                    link: link,
                    hash: hash,
                    offset: offset,
                    referenceType: Destinations
                        ?.Values
                        .Where(d => d.SiteId == link.SiteId)
                        .Select(d => d.ReferenceType)
                        .FirstOrDefault(),
                    columnName: columnName,
                    parentColumn: GetColumn(
                        context: context,
                        columnName: parentClass),
                    parentIds: parentIds,
                    setTotalCount: setTotalCount));
            return hash;
        }

        private void LinkHash(
            Context context,
            IEnumerable<long> selectedValues,
            List<string> searchIndexes,
            Link link,
            Dictionary<string, List<string>> hash,
            int offset,
            string referenceType,
            string columnName,
            Column parentColumn,
            IEnumerable<long> parentIds,
            bool setTotalCount = false)
        {
            var join = new SqlJoinCollection(
                new SqlJoin(
                    tableBracket: "\"Sites\"",
                    joinType: SqlJoin.JoinTypes.Inner,
                    joinExpression: "\"Items\".\"SiteId\"=\"Sites\".\"SiteId\""));
            var where = Rds.ItemsWhere()
                .ReferenceId_In(
                    selectedValues,
                    _using: selectedValues?.Any() == true
                        && Repository.ExecuteScalar_string(
                            context: context,
                            statements: Rds.SelectSites(
                                column: Rds.SitesColumn().ReferenceType(),
                                where: Rds.SitesWhere().SiteId(link.SiteId))) != "Wikis")
                .ReferenceId_In(
                    sub: LinkHashRelatingColumnsSubQueryStatement(
                        context: context,
                        referenceType: referenceType,
                        columnName: columnName,
                        parentColumn: parentColumn,
                        parentIds: parentIds),
                    _using: (referenceType == "Results"
                        || referenceType == "Issues")
                            && (parentIds?.Any() ?? false)
                            && parentColumn != null)
                .ReferenceType("Sites", _operator: "<>")
                .Sites_TenantId(context.TenantId)
                .Sites_SiteId(link.SiteId)
                .SearchTextWhere(
                    context: context,
                    ss: JoinedSsHash?.Get(link.SiteId),
                    searchText: searchIndexes?.Join(" "))
                .CanRead(
                    context: context,
                    idColumnBracket: "\"Items\".\"ReferenceId\"");
            var statements = new List<SqlStatement>
            {
                Rds.SelectItems(
                    dataTableName: "Main",
                    column: Rds.ItemsColumn()
                        .ReferenceId()
                        .ReferenceType()
                        .SiteId()
                        .Title(),
                    join: join,
                    where: where,
                    orderBy: Rds.ItemsOrderBy().Title(),
                    offset: offset,
                    pageSize: Parameters.General.DropDownSearchPageSize)
            };
            if (setTotalCount)
            {
                statements.Add(
                    Rds.SelectCount(
                        tableName: "Items",
                        join: join,
                        where: where));
            }
            var dataSet = Repository.ExecuteDataSet(
                context: context,
                statements: statements.ToArray());
            var dataRows = dataSet.Tables["Main"].AsEnumerable();
            if (setTotalCount)
            {
                GetColumn(context: context, columnName: link.ColumnName)
                    .TotalCount = Rds.Count(dataSet);
            }
            if (dataRows.Any())
            {
                hash.Add($"[[{link.SiteId}]]", LinkValue(
                    context: context,
                    siteId: link.SiteId,
                    dataRows: dataRows,
                    searchIndexes: searchIndexes));
            }
        }

        private static List<string> LinkValue(
            Context context,
            long siteId,
            EnumerableRowCollection<DataRow> dataRows,
            List<string> searchIndexes)
        {
            return dataRows.Any(dataRow =>
                dataRow.Long("SiteId") == siteId &&
                dataRow.String("ReferenceType") == "Wikis")
                    ? Repository.ExecuteScalar_string(
                        context: context,
                        statements: Rds.SelectWikis(
                            column: Rds.WikisColumn().Body(),
                            where: Rds.WikisWhere().SiteId(siteId)))
                                .SplitReturn()
                                .Where(o => o.Trim() != string.Empty)
                                .GroupBy(o => o.Split_1st())
                                .Select(o => o.First())
                                .Where(o => searchIndexes?.Any() != true
                                    || searchIndexes.All(p =>
                                        o.Split_1st().RegexLike(p).Any()
                                        || o.Split_2nd().RegexLike(p).Any()))
                                .ToList()
                    : dataRows
                        .Where(dataRow => dataRow.Long("SiteId") == siteId)
                        .Select(dataRow =>
                            dataRow.String("ReferenceId") + "," + dataRow.String("Title"))
                        .ToList();
        }

        public Link ColumnFilterExpressionsLink(Context context, Column column)
        {
            return Links
                ?.Where(o => o.ColumnName == column.ColumnName)
                .Where(o => o.JsonFormat == true)
                .Where(o => o.View?.ColumnFilterExpressions?.Any() == true)
                .FirstOrDefault();
        }

        public SiteSettings GetLinkedSiteSettings(Context context, Link link)
        {
            switch (link.TableName)
            {
                case "Depts":
                    return SiteSettingsUtilities.DeptsSiteSettings(context: context);
                case "Groups":
                    return SiteSettingsUtilities.GroupsSiteSettings(context: context);
                case "Users":
                    return SiteSettingsUtilities.UsersSiteSettings(context: context);
                default:
                    return JoinedSsHash.Get(link.SiteId);
            }
        }

        public Column GetFilterExpressionsColumn(
            Context context,
            Link link,
            string columnName)
        {
            switch (link.TableName)
            {
                case "Depts":
                case "Users":
                    switch (columnName)
                    {
                        case "Groups":
                            // GetColumnで取得できない項目を仮想的にColumnFilterHashExpressionsに渡すための設定
                            return new Column()
                            {
                                ColumnName = "Groups",
                                TypeName = "nvarchar",
                                ControlType = "ChoicesText",
                                ChoicesText = "HasChoices"
                            };
                        default:
                            break;
                    }
                    break;
                case "Groups":
                    switch (columnName)
                    {
                        case "GroupMembers":
                            return new Column()
                            {
                                ColumnName = "GroupMembers",
                                TypeName = "nvarchar",
                                ControlType = "ChoicesText",
                                ChoicesText = "HasChoices"
                            };
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
            return GetColumn(
                context: context,
                columnName: columnName);
        }

        public Tab AddTab(Tab tab)
        {
            TabLatestId = TabLatestId ?? 0;
            TabLatestId++;
            tab.Id = TabLatestId.ToInt();
            if (Tabs == null)
            {
                Tabs = new SettingList<Tab>();
            }
            Tabs.Add(tab);
            return tab;
        }

        public Section AddSection(Section section)
        {
            SectionLatestId = SectionLatestId ?? 0;
            SectionLatestId++;
            section.Id = SectionLatestId.ToInt();
            if (Sections == null)
            {
                Sections = new List<Section>();
            }
            Sections.Add(section);
            return section;
        }

        public Error.Types AddSummary(
            long siteId,
            string destinationReferenceType,
            string destinationColumn,
            int? destinationCondition,
            bool? setZeroWhenOutOfCondition,
            string linkColumn,
            string type,
            string sourceColumn,
            int? sourceCondition)
        {
            var id = Summaries.Any()
                ? Summaries.Select(o => o.Id).Max() + 1
                : 1;
            Summaries.Add(new Summary(
                id,
                siteId,
                destinationReferenceType,
                destinationColumn,
                destinationCondition,
                setZeroWhenOutOfCondition,
                linkColumn,
                type,
                sourceColumn,
                sourceCondition));
            return Error.Types.None;
        }

        public Error.Types UpdateSummary(
            int id,
            long siteId,
            string destinationReferenceType,
            string destinationColumn,
            int? destinationCondition,
            bool? setZeroWhenOutOfCondition,
            string linkColumn,
            string type,
            string sourceColumn,
            int? sourceCondition)
        {
            var summary = Summaries?.Get(id);
            if (summary != null)
            {
                summary.Update(
                    siteId,
                    destinationReferenceType,
                    destinationColumn,
                    destinationCondition,
                    setZeroWhenOutOfCondition,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition);
                return Error.Types.None;
            }
            else
            {
                return Error.Types.NotFound;
            }
        }

        public void AddView(View view)
        {
            ViewLatestId++;
            view.Id = ViewLatestId.ToInt();
            if (Views == null) Views = new List<View>();
            Views.Add(view);
        }

        public void Remind(
            Context context,
            List<int> idList,
            DateTime scheduledTime,
            bool test = false)
        {
            Reminders?
                .Where(o => idList.Contains(o.Id))
                .ForEach(reminder => reminder.Remind(
                    context: context,
                    ss: this,
                    scheduledTime: scheduledTime,
                    test: test));
        }

        public Export GetExport(Context context, int id = 0)
        {
            return Exports
                ?.Where(o => o.Accessable(context: context))
                .FirstOrDefault(o => o.Id == id)
                    ?? new Export(DefaultExportColumns(context: context));
        }

        public List<ExportColumn> DefaultExportColumns(Context context)
        {
            var columns = GetEditorColumnNames(
                context: context,
                columnOnly: true)
                    .Concat(GridColumns)
                    .Where(o => o != "Ver")
                    .ToList();
            return ColumnDefinitionHash.ExportDefinitions()
                .Where(o => columns.Contains(o.ColumnName) || o.ExportColumn)
                .Select(o => new ExportColumn(
                    context: context,
                    column: GetColumn(
                        context: context,
                        columnName: o.ColumnName)))
                .ToList();
        }

        public List<ExportColumn> ExportColumns(Context context, string join = null)
        {
            return GetJoinedSs(join: join)?.ColumnDefinitionHash.ExportDefinitions()
                .Where(o => o.TypeCs != "Attachments")
                .OrderBy(o => o.EditorColumn)
                .Select((o, i) => new ExportColumn(
                    context: context,
                    column: GetColumn(
                        context: context,
                        columnName: join.IsNullOrEmpty()
                            ? o.ColumnName
                            : join + "," + o.ColumnName)))
                .ToList() ?? new List<ExportColumn>();
        }

        public bool EnableViewMode(Context context, string name)
        {
            switch (name)
            {
                case "Index": return true;
                case "Calendar": return EnableCalendar == true;
                case "Crosstab": return EnableCrosstab == true;
                case "Gantt": return EnableGantt == true;
                case "BurnDown": return EnableBurnDown == true;
                case "TimeSeries": return EnableTimeSeries == true;
                case "Analy": return EnableAnaly == true;
                case "Kamban": return EnableKamban == true;
                case "ImageLib": return context.ContractSettings.Images() && EnableImageLib == true;
                default: return false;
            }
        }

        public Permissions.Types GetPermissionType(Context context, bool site = false)
        {
            var permission = Permissions.Types.NotSet;
            if (PermissionType != null)
            {
                permission |= (Permissions.Types)PermissionType;
            }
            if (ItemPermissionType != null && !site)
            {
                permission |= (Permissions.Types)ItemPermissionType;
            }
            return permission;
        }

        public Permission GetPermissionForCreating(string key)
        {
            return PermissionForCreating?.ContainsKey(key) == true
                ? new Permission(key, 0, PermissionForCreating[key])
                : new Permission(key, 0, Permissions.Types.NotSet, source: true);
        }

        public Permission GetPermissionForUpdating(string key)
        {
            return PermissionForUpdating?.ContainsKey(key) == true
                ? new Permission(key, 0, PermissionForUpdating[key])
                : new Permission(key, 0, Permissions.Types.NotSet, source: true);
        }

        public void SetDashboardParts(DashboardPart dashboardPart)
        {
            DashboardParts = new SettingList<DashboardPart>
            {
                dashboardPart
            };
        }

        public void SetSiteIntegration(Context context)
        {
            if (IntegratedSites?.Any() == true)
            {
                SetAllowedIntegratedSites(context: context);
                SetSiteTitleChoicesText(context: context);
                SetSiteIntegrationChoicesText(context: context);
            }
        }

        private void SetAllowedIntegratedSites(Context context)
        {
            AllowedIntegratedSites = new List<long>();
            var sites = GetIntegratedSites(context: context);
            var allows = Permissions.AllowSites(
                context: context,
                sites: sites,
                referenceType: ReferenceType);
            AllowedIntegratedSites.AddRange(allows);
        }

        public List<long> GetIntegratedSites(Context context)
        {
            var sites = new List<long>() { SiteId };
            IntegratedSites?.ForEach(site =>
            {
                var isName = false;
                var dataRows = SiteInfo.Sites(context: context).Values;
                dataRows
                    .Where(dataRow =>
                        dataRow.String("SiteName") == site
                        || dataRow.String("SiteGroupName") == site)
                    .ForEach(dataRow =>
                    {
                        sites.Add(dataRow.Long("SiteId"));
                        isName = true;
                    });
                if (!isName && site.ToLong() > 0)
                {
                    sites.Add(site.ToLong());
                }
            });
            return sites;
        }

        private void SetSiteTitleChoicesText(Context context)
        {
            var column = GetColumn(context: context, columnName: "SiteTitle");
            if (column != null)
            {
                var siteMenu = SiteInfo.TenantCaches.Get(context.TenantId)?.SiteMenu;
                if (siteMenu != null)
                {
                    column.ChoicesText = AllowedIntegratedSites
                        .Select(o => siteMenu.Get(o))
                        .Where(o => o != null)
                        .Select(o => $"{o.SiteId},{o.Title}")
                    .Join("\n");
                }
            }
        }

        private void SetSiteIntegrationChoicesText(Context context)
        {
            Dictionary<long, SiteSettings> hash = null;
            Columns
                .Where(o => o.ChoicesText?.Contains("[[Integration]]") == true)
                .ForEach(column =>
                {
                    if (hash == null)
                    {
                        hash = Repository.ExecuteTable(
                            context: context,
                            statements: Rds.SelectSites(
                                column: Rds.SitesColumn()
                                    .SiteId()
                                    .SiteSettings(),
                                where: Rds.SitesWhere()
                                    .TenantId(context.TenantId)
                                    .SiteId_In(AllowedIntegratedSites)))
                                        .AsEnumerable()
                                        .ToDictionary(
                                            o => o["SiteId"].ToLong(),
                                            o => o["SiteSettings"]
                                                .ToString()
                                                .DeserializeSiteSettings(context: context));
                    }
                    column.ChoicesText = column.ChoicesText.Replace(
                        "[[Integration]]", AllowedIntegratedSites
                            .Select(siteId => hash.Get(siteId)?.GetColumn(
                                context: context, columnName: column.ColumnName)?.ChoicesText)
                            .Where(o => o != null)
                            .SelectMany(o => o.Split(','))
                            .Distinct()
                            .Where(o => o != "[[Integration]]")
                            .Join("\n"));
                });
        }

        private void SetIntegratedSites(string value)
        {
            IntegratedSites = value
                .Split(',')
                .Select(o => o.Trim())
                .Where(o => !o.IsNullOrEmpty())
                .Distinct()
                .ToList();
        }

        private void SetPermissionForCreating(string value)
        {
            PermissionForCreating = Permissions.Get(value.Deserialize<List<string>>())
                .ToDictionary(o => o.Name, o => o.Type);
        }

        private void SetPermissionForUpdating(string value)
        {
            PermissionForUpdating = Permissions.Get(value.Deserialize<List<string>>())
                .ToDictionary(o => o.Name, o => o.Type);
        }

        private void SetCreateColumnAccessControl(string value)
        {
            CreateColumnAccessControls = value.Deserialize<List<string>>()
                .Select(o => o.Deserialize<ColumnAccessControl>())
                .Where(o => !o.IsDefault(this, "Create"))
                .ToList();
        }

        private void SetReadColumnAccessControl(string value)
        {
            ReadColumnAccessControls = value.Deserialize<List<string>>()
                .Select(o => o.Deserialize<ColumnAccessControl>())
                .Where(o => !o.IsDefault(this, "Read"))
                .ToList();
        }

        private void SetUpdateColumnAccessControl(string value)
        {
            UpdateColumnAccessControls = value.Deserialize<List<string>>()
                .Select(o => o.Deserialize<ColumnAccessControl>())
                .Where(o => !o.IsDefault(this, "Update"))
                .ToList();
        }

        public SqlJoinCollection Join(Context context, params IJoin[] join)
        {
            return SqlJoinCollection(
                context: context,
                tableNames: join
                    .Where(o => o != null)
                    .SelectMany(o => o.JoinTableNames())
                    .Distinct()
                    .ToList());
        }

        public SqlJoinCollection SqlJoinCollection(Context context, List<string> tableNames)
        {
            var join = new SqlJoinCollection(tableNames
                .Where(o => o != null)
                .Distinct()
                .OrderBy(o => o.Length)
                .SelectMany(o => SqlJoinCollection(context: context, tableAlias: o))
                .GroupBy(o => o.JoinExpression)
                .Select(o => o.First())
                .ToArray());
            join.ItemJoin(
                tableName: ReferenceType,
                tableType: TableType);
            return join;
        }

        private SqlJoinCollection SqlJoinCollection(Context context, string tableAlias)
        {
            var join = new SqlJoinCollection();
            var left = new List<string>();
            var leftTableName = ReferenceType;
            var leftAlias = ReferenceType;
            var path = new List<string>();
            foreach (var part in tableAlias.Split('-'))
            {
                var siteId = part.Split('~').Last().ToLong();
                var currentSs = JoinedSsHash.Get(siteId);
                var tableName = currentSs?.ReferenceType;
                var name = part.Split_1st('~').RegexFirst("[A-Za-z0-9]+");
                var column = currentSs?.GetColumn(
                    context: context,
                    columnName: name);
                if (column == null) continue;
                path.Add(part);
                var alias = path.Join("-");
                if (!tableName.IsNullOrEmpty() && !name.IsNullOrEmpty())
                {
                    if (alias.Contains("~~"))
                    {
                        join.Add(new SqlJoin(
                            tableBracket: "\"" + tableName + "\"",
                            joinType: SqlJoin.JoinTypes.LeftOuter,
                            joinExpression: $"\"{leftAlias}\".\"{Rds.IdColumn(leftTableName)}\"={context.SqlCommandText.CreateTryCast(alias, name, column.TypeName, "bigint")} and \"{alias}\".\"SiteId\"={siteId}",
                            _as: alias));
                        join.Add(
                            tableName: "\"Items\"",
                            joinType: SqlJoin.JoinTypes.LeftOuter,
                            joinExpression: $"\"{alias}\".\"{Rds.IdColumn(tableName)}\"=\"{alias}_Items\".\"ReferenceId\"",
                            _as: alias + "_Items");
                    }
                    else
                    {
                        join.Add(
                            tableName: "\"Items\"",
                            joinType: SqlJoin.JoinTypes.LeftOuter,
                            joinExpression: $"{context.SqlCommandText.CreateTryCast(left.Any() ? left.Join("-") : ReferenceType, name, column.TypeName, "bigint")}=\"{alias}_Items\".\"ReferenceId\" and \"{alias}_Items\".\"SiteId\"={siteId}",
                            _as: alias + "_Items");
                        join.Add(new SqlJoin(
                            tableBracket: "\"" + tableName + "\"",
                            joinType: SqlJoin.JoinTypes.LeftOuter,
                            joinExpression: $"\"{alias}_Items\".\"ReferenceId\"=\"{alias}\".\"{Rds.IdColumn(tableName)}\"",
                            _as: alias));
                    }
                    left.Add(part);
                    leftTableName = tableName;
                    leftAlias = alias;
                }
                else
                {
                    break;
                }
            }
            return join;
        }

        public string LabelTitle(Context context, string columnName)
        {
            var column = GetColumn(context: context, columnName: columnName);
            return column != null
                ? LabelTitle(column)
                : null;
        }

        public string LabelTitle(Column column)
        {
            var join = JoinOptions()?.FirstOrDefault(o => o.Key == column.TableName());
            return (join?.Key == null
                ? string.Empty
                : join.Value.Value + " ")
                    + column.LabelText;
        }

        public string EditorStyles(Context context, BaseModel.MethodTypes methodType)
        {
            switch (methodType)
            {
                case BaseModel.MethodTypes.New:
                    return GetStyleBody(
                        context: context, peredicate: o => o.New == true);
                default:
                    return GetStyleBody(
                        context: context, peredicate: o => o.Edit == true);
            }
        }

        public string ViewModeStyles(Context context)
        {
            switch (context.Action)
            {
                case "index":
                    return GetStyleBody(
                        context: context, peredicate: o => o.Index == true);
                case "calendar":
                    return GetStyleBody(
                        context: context, peredicate: o => o.Calendar == true);
                case "crosstab":
                    return GetStyleBody(
                        context: context, peredicate: o => o.Crosstab == true);
                case "gantt":
                    return GetStyleBody(
                        context: context, peredicate: o => o.Gantt == true);
                case "burndown":
                    return GetStyleBody(
                        context: context, peredicate: o => o.BurnDown == true);
                case "timeseries":
                    return GetStyleBody(
                        context: context, peredicate: o => o.TimeSeries == true);
                case "analy":
                    return GetStyleBody(
                        context: context, peredicate: o => o.Analy == true);
                case "kamban":
                    return GetStyleBody(
                        context: context, peredicate: o => o.Kamban == true);
                case "imagelib":
                    return GetStyleBody(
                        context: context, peredicate: o => o.ImageLib == true);
                default:
                    return null;
            }
        }

        public string GetStyleBody(Context context, Func<Style, bool> peredicate)
        {
            return !IsSiteEditor(context: context)
                ? Styles?
                    .Where(style => style.Disabled != true
                        && peredicate(style))
                    .Select(o => o.Body).Join("\n")
                : null;
        }

        public string EditorScripts(Context context, BaseModel.MethodTypes methodType)
        {
            switch (methodType)
            {
                case BaseModel.MethodTypes.New:
                    return GetScriptBody(
                        context: context, peredicate: o => o.New == true);
                default:
                    return GetScriptBody(
                        context: context, peredicate: o => o.Edit == true);
            }
        }

        public string ViewModeScripts(Context context)
        {
            switch (context.Action)
            {
                case "index":
                    return GetScriptBody(
                        context: context, peredicate: o => o.Index == true);
                case "calendar":
                    return GetScriptBody(
                        context: context, peredicate: o => o.Calendar == true);
                case "crosstab":
                    return GetScriptBody(
                        context: context, peredicate: o => o.Crosstab == true);
                case "gantt":
                    return GetScriptBody(
                        context: context, peredicate: o => o.Gantt == true);
                case "burndown":
                    return GetScriptBody(
                        context: context, peredicate: o => o.BurnDown == true);
                case "timeseries":
                    return GetScriptBody(
                        context: context, peredicate: o => o.TimeSeries == true);
                case "analy":
                    return GetScriptBody(
                        context: context, peredicate: o => o.Analy == true);
                case "kamban":
                    return GetScriptBody(
                        context: context, peredicate: o => o.Kamban == true);
                case "imagelib":
                    return GetScriptBody(
                        context: context, peredicate: o => o.ImageLib == true);
                default:
                    return null;
            }
        }

        public string GetScriptBody(Context context, Func<Script, bool> peredicate)
        {
            return !IsSiteEditor(context: context)
                ? Scripts?
                    .Where(script => script.Disabled != true
                        && peredicate(script))
                    .Select(o => o.Body).Join("\n")
                : null;
        }

        public string ViewModeHtmls(Context context, Html.PositionTypes positionType)
        {
            switch (context.Action)
            {
                case "index":
                    return GetHtmlBody(
                        context: context,
                        peredicate: o => o.Index == true,
                        positionType: positionType);
                case "calendar":
                    return GetHtmlBody(
                        context: context,
                        peredicate: o => o.Calendar == true,
                        positionType: positionType);
                case "crosstab":
                    return GetHtmlBody(
                        context: context,
                        peredicate: o => o.Crosstab == true,
                        positionType: positionType);
                case "gantt":
                    return GetHtmlBody(
                        context: context,
                        peredicate: o => o.Gantt == true,
                        positionType: positionType);
                case "burndown":
                    return GetHtmlBody(
                        context: context,
                        peredicate: o => o.BurnDown == true,
                        positionType: positionType);
                case "timeseries":
                    return GetHtmlBody(
                        context: context,
                        peredicate: o => o.TimeSeries == true,
                        positionType: positionType);
                case "analy":
                    return GetHtmlBody(
                        context: context,
                        peredicate: o => o.Analy == true,
                        positionType: positionType);
                case "kamban":
                    return GetHtmlBody(
                        context: context,
                        peredicate: o => o.Kamban == true,
                        positionType: positionType);
                case "imagelib":
                    return GetHtmlBody(
                        context: context,
                        peredicate: o => o.ImageLib == true,
                        positionType: positionType);
                default:
                    return null;
            }
        }

        public string GetHtmlBody(Context context, Func<Html, bool> peredicate, Html.PositionTypes positionType)
        {
            return !IsSiteEditor(context: context)
                ? Htmls
                    ?.Where(html => html.Disabled != true
                        && html.PositionType == positionType
                        && peredicate(html))
                    .Select(o => o.Body).Join("\n")
                : null;
        }

        public string GridCss(Context context)
        {
            switch (TableType)
            {
                case Sqls.TableTypes.History:
                    return "history";
                case Sqls.TableTypes.Deleted:
                    return "deleted not-link";
                default:
                    return context.Forms.Bool("EditOnGrid")
                        ? "confirm-unload not-link"
                        : DisableLinkToEdit == true
                            ? "not-link"
                            : OpenEditInNewTab == true
                                ? "new-tab"
                                : string.Empty;
            }
        }

        public bool InitialValue(Context context)
        {
            return RecordingJson(context: context) == "[]";
        }

        public BulkUpdateColumn GetBulkUpdateColumn(int id)
        {
            return BulkUpdateColumns?.FirstOrDefault(o => o.Id == id) ?? new BulkUpdateColumn();
        }

        public List<string> GetBulkUpdateColumnNames(Context context)
        {
            return ColumnDefinitionHash.EditorDefinitions(context: context)
                .Select(columnDefinition => GetColumn(
                    context: context,
                    columnName: columnDefinition.ColumnName))
                .Where(column => column.ColumnName != "Comments"
                    && column.ControlType != "Attachments"
                    && !column.Id_Ver
                    && !column.NotUpdate
                    && !column.OtherColumn())
                .Select(o => o.ColumnName)
                .ToList();
        }

        public Dictionary<string, ControlData> BulkUpdateColumnSelectableOptions(
            Context context, int id, bool enabled = true)
        {
            return enabled
                ? ColumnUtilities.SelectableOptions(
                    context: context,
                    ss: this,
                    columns: GetBulkUpdateColumn(id).Columns ?? new List<string>(),
                    order: GetBulkUpdateColumnNames(context: context))
                : ColumnUtilities.SelectableSourceOptions(
                    context: context,
                    ss: this,
                    columns: GetBulkUpdateColumnNames(context: context)
                        .Where(o => GetBulkUpdateColumn(id).Columns?.Contains(o) != true),
                    order: GetBulkUpdateColumnNames(context: context));
        }

        public List<Column> GetBulkUpdateColumns(
            Context context,
            string target)
        {
            var columnNames = target.StartsWith("BulkUpdate_")
                ? BulkUpdateColumns.Get(target.Split_2nd('_').ToInt())?.Columns
                : target.ToSingleList();
            var columns = columnNames
                .Select(columnName => GetColumn(
                    context: context,
                    columnName: columnName))
                .ToList();
            return columns;
        }

        public void SetBulkUpdateColumnDetail(
            List<Column> columns,
            string target)
        {
            var bulkUpdateColumns = target.StartsWith("BulkUpdate_")
                ? BulkUpdateColumns.Get(target.Split_2nd('_').ToInt())
                : null;
            if (bulkUpdateColumns != null)
            {
                columns.ForEach(column =>
                {
                    var detail = bulkUpdateColumns.Details.Get(column.ColumnName);
                    column.DefaultInput = detail?.DefaultInput
                        ?? string.Empty;
                    column.ValidateRequired = detail?.ValidateRequired
                        ?? column.ValidateRequired;
                    column.EditorReadOnly = detail?.EditorReadOnly
                        ?? column.EditorReadOnly;
                });
            }
        }

        public RelatingColumn GetRelatingColumn(int id)
        {
            return RelatingColumns?.FirstOrDefault(o => o.Id == id) ?? new RelatingColumn();
        }

        public Dictionary<string, ControlData> RelatingColumnSelectableOptions(
            Context context, int id, bool enabled = true)
        {
            return enabled
                ? ColumnUtilities.SelectableOptions(
                    context: context,
                    ss: this,
                    columns: GetRelatingColumn(id).Columns ?? new List<string>(),
                    order: GetEditorColumnNames()
                        .Where(o => o.StartsWith("Class")).ToList<string>())
                : ColumnUtilities.SelectableSourceOptions(
                    context: context,
                    ss: this,
                    columns: GetEditorColumnNames()
                        .Where(o => o.StartsWith("Class")
                            && GetRelatingColumn(id).Columns?.Contains(o) != true),
                    order: GetEditorColumnNames()
                        .Where(o => o.StartsWith("Class")).ToList<string>());
        }

        private SqlStatement LinkHashRelatingColumnsSubQueryStatement(
            Context context,
            string referenceType,
            string columnName,
            Column parentColumn,
            IEnumerable<long> parentIds)
        {
            var siteId = Links?.FirstOrDefault(o => o.ColumnName == columnName)?.SiteId ?? 0;
            var ss = Destinations?.Get(siteId);
            return new SqlStatement(ss?.LinkHashRelatingColumnsSubQuery(
                context: context,
                referenceType: referenceType,
                parentColumn: parentColumn,
                parentIds: parentIds));
        }

        public string LinkHashRelatingColumnsSubQuery(
            Context context,
            string referenceType,
            Column parentColumn,
            IEnumerable<long> parentIds)
        {
            if (parentColumn == null
                || referenceType == "Wikis"
                || referenceType == "Sites"
                || parentIds == null)
            {
                return null;
            }
            var multipleSelections = RelatingColumnMultipleSelections(
                context: context,
                parentColumn: parentColumn);
            var whereNullorEmpty = parentIds?.Contains(-1) == true
                ? $"\"{parentColumn.ColumnName}\" is null or \"{parentColumn.ColumnName}\" = '' "
                : string.Empty;
            var whereIn = parentIds.Where(n => n >= 0).Any()
                ? multipleSelections
                    ? "(" + parentIds
                        .Select(id => $"\"{parentColumn.ColumnName}\" like '%\"{id}\"%'")
                        .Join(" or ") + ")"
                    : $"{context.SqlCommandText.CreateTryCast(referenceType, parentColumn.ColumnName, parentColumn.TypeName, "bigint")} in ({parentIds.Where(n => n >= 0).Join()})"
                : string.Empty;
            var Or = (whereNullorEmpty == string.Empty || whereIn == string.Empty)
                ? string.Empty
                : " or ";
            return $"select \"{Rds.IdColumn(referenceType)}\" from \"{referenceType}\" where \"SiteId\"={SiteId} and ({whereNullorEmpty}{Or}{whereIn})";
        }

        private bool RelatingColumnMultipleSelections(Context context, Column parentColumn)
        {
            var column = GetColumn(
                context: context,
                columnName: parentColumn.ColumnName);
            return column?.MultipleSelections ?? false;
        }

        public void SetRelatingColumnsLinkedClass()
        {
            Dictionary<string, string> linkedClasses = new Dictionary<string, string>();
            foreach (var relcol in RelatingColumns
                .Where(o => o.Columns != null)
                .Select(o => o))
            {
                var precol = "";
                foreach (var col in relcol.Columns)
                {
                    if (precol != "")
                    {
                        linkedClasses[col] = GetParentLinkedClass(precol, col);
                    }
                    precol = col;
                }
                relcol.ColumnsLinkedClass = linkedClasses;
            }
        }

        private string GetParentLinkedClass(string parentClassName, string childClassName)
        {
            var parentSiteId = Links
                .Where(o => o.SiteId > 0)
                .Where(o => o.ColumnName == parentClassName)
                .Select(o => o.SiteId)
                .FirstOrDefault();
            var childSiteId = Links
                .Where(o => o.SiteId > 0)
                .Where(o => o.ColumnName == childClassName)
                .Select(o => o.SiteId)
                .FirstOrDefault();
            return Destinations
                .Values
                .Where(dest => dest.SiteId == childSiteId)
                .Select(dest => dest.Links
                    .Where(o => o.SiteId > 0)
                    .Where(l => l.SiteId == parentSiteId)
                    .Select(l => new
                    {
                        l.ColumnName,
                        OrderNo = dest
                            .GetEditorColumnNames()
                            .Contains(l.ColumnName)
                                ? dest.GetEditorColumnNames()
                                    .Select((v, k) => new { Key = k, Value = v })
                                    .Where(d => d.Value == l.ColumnName)
                                    .Select(d => d.Key)
                                    .First()
                                : int.MaxValue
                    })
                    .OrderBy(o => o.OrderNo)
                    .Select(o => o.ColumnName)
                    .FirstOrDefault())
                .SingleOrDefault();
        }

        public bool CheckRow(Context context, List<string> gridColumns)
        {
            return GridColumnsHasSources(gridColumns: gridColumns)
                ? false
                : context.CanUpdate(ss: this)
                    || context.CanDelete(ss: this)
                    || context.CanExport(ss: this)
                    || GetProcess(
                        context: context,
                        id: context.Forms.Int("BulkProcessingItems")) != null;
        }

        public bool GridColumnsHasSources(List<string> gridColumns)
        {
            return (gridColumns ?? GridColumns)?.Any(o => o.Contains("~~")) == true;
        }

        public string ColumnsJson()
        {
            return Columns
                ?.Where(column =>
                    GridColumns.Contains(column.ColumnName)
                    || GetEditorColumnNames().Contains(column.ColumnName)
                    || TitleColumns.Contains(column.ColumnName)
                    || LinkColumns.Contains(column.ColumnName)
                    || HistoryColumns.Contains(column.ColumnName)
                    || column.LabelText != column.LabelTextDefault)
                .Select(column => new
                {
                    column.ColumnName,
                    column.LabelText
                })
                .ToJson();
        }

        public bool IsDefaultFilterColumn(Column column)
        {
            return ColumnDefinitionHash.FilterDefinitions(enableOnly: false)
                .Any(o => o.ColumnName == column.Name);
        }

        public List<string> ReplaceFieldColumns(
            Context context,
            ServerScriptModelRow serverScriptModelRow)
        {
            var controlId = context.Forms.ControlId();
            var columns = serverScriptModelRow?.ReplaceFieldColumns(context: context)
                ?? new List<string>();
            columns.AddRange(Links?
                .Where(o => $"{ReferenceType}_{o.ColumnName}" == controlId)
                .Where(o => o.Lookups != null)
                .SelectMany(o => o.Lookups.Select(p => p.To)));
            columns.AddRange(Links?
                .Where(o => o.View?.ColumnFilterExpressions?.Any() == true)
                .Select(o => o.ColumnName));
            columns.AddRange(Columns
                .Where(o => o.UseSearch == true)
                .Select(o => o.ColumnName));
            columns.AddRange(StatusControls?
                .Where(statusControl => statusControl.ColumnHash != null)
                .SelectMany(statusControl => statusControl.ColumnHash)
                .Where(o => o.Value != StatusControl.ControlConstraintsTypes.None)
                .Select(o => o.Key));
            return columns
                .Distinct()
                .ToList();
        }

        public List<ServerScript> GetServerScripts(Context context)
        {
            if (ServerScriptsAndExtended != null)
            {
                return ServerScriptsAndExtended;
            }
            else
            {
                ServerScriptsAndExtended = Parameters.ExtendedServerScripts
                    .ExtensionWhere<ParameterAccessor.Parts.ExtendedServerScript>(
                        context: context,
                        siteId: SiteId)
                    .Select(extendedServerScript => new ServerScript()
                    {
                        Name = extendedServerScript.Name,
                        WhenloadingSiteSettings = extendedServerScript.WhenloadingSiteSettings,
                        WhenViewProcessing = extendedServerScript.WhenViewProcessing,
                        WhenloadingRecord = extendedServerScript.WhenloadingRecord,
                        BeforeFormula = extendedServerScript.BeforeFormula,
                        AfterFormula = extendedServerScript.AfterFormula,
                        BeforeCreate = extendedServerScript.BeforeCreate,
                        AfterCreate = extendedServerScript.AfterCreate,
                        BeforeUpdate = extendedServerScript.BeforeUpdate,
                        AfterUpdate = extendedServerScript.AfterUpdate,
                        BeforeDelete = extendedServerScript.BeforeDelete,
                        BeforeBulkDelete = extendedServerScript.BeforeBulkDelete,
                        AfterDelete = extendedServerScript.AfterDelete,
                        AfterBulkDelete = extendedServerScript.AfterBulkDelete,
                        BeforeOpeningPage = extendedServerScript.BeforeOpeningPage,
                        BeforeOpeningRow = extendedServerScript.BeforeOpeningRow,
                        Shared = extendedServerScript.Shared,
                        Functionalize = extendedServerScript.Functionalize,
                        TryCatch = extendedServerScript.TryCatch,
                        Body = extendedServerScript.Body
                    })
                        .Concat(ServerScripts.Where(script => script.Disabled != true))
                        .ToList();
                ServerScriptsAndExtended
                    .Where(serverScript =>
                        serverScript.WhenloadingSiteSettings == true
                        || serverScript.WhenViewProcessing == true
                        || serverScript.WhenloadingRecord == true
                        || serverScript.BeforeFormula == true
                        || serverScript.AfterFormula == true
                        || serverScript.BeforeCreate == true
                        || serverScript.AfterCreate == true
                        || serverScript.BeforeUpdate == true
                        || serverScript.AfterUpdate == true
                        || serverScript.BeforeDelete == true
                        || serverScript.BeforeBulkDelete == true
                        || serverScript.AfterDelete == true
                        || serverScript.AfterBulkDelete == true
                        || serverScript.BeforeOpeningPage == true
                        || serverScript.BeforeOpeningRow == true)
                    .ForEach(serverScript =>
                    {
                        serverScript.SetDebug();
                        var body = serverScript.Body;
                        var sharedServerScripts = SharedServerScripts(serverScripts: ServerScriptsAndExtended);
                        if (!sharedServerScripts.IsNullOrEmpty())
                        {
                            body = sharedServerScripts + "\n" + body;
                        }
                        serverScript.Body = body;
                    });
                ServerScriptsAndExtended.ForEach(serverScript =>
                {
                    var body = serverScript.Body;
                    body = IncludedServerScripts(
                        serverScripts: ServerScriptsAndExtended,
                        body: body);
                    serverScript.Body = body;
                });
                return ServerScriptsAndExtended;
            }
        }

        private string SharedServerScripts(List<ServerScript> serverScripts)
        {
            return serverScripts
                .Where(o => o.Shared == true)
                .Select(o => o.Body)
                .Join("\n");
        }

        private string IncludedServerScripts(
            List<ServerScript> serverScripts,
            string body,
            int depth = 0)
        {
            if (body.IsNullOrEmpty())
            {
                return body;
            }
            if (depth > Parameters.Script.ServerScriptIncludeDepthLimit)
            {
                return body;
            }
            if (!body.Contains("//Include:"))
            {
                return body;
            }
            var replacedBody = new List<string>();
            foreach (var line in body.Split('\n'))
            {
                if (line.StartsWith("//Include:"))
                {
                    var name = line.Substring(line.IndexOf(":") + 1).Trim();
                    var includeBody = serverScripts
                        .Where(o => o.Name == name)
                        .Select(o => o.Body)
                        .Join("\n");
                    includeBody = IncludedServerScripts(
                        serverScripts: serverScripts,
                        body: includeBody,
                        depth: depth + 1);
                    replacedBody.Add(includeBody);
                }
                else
                {
                    replacedBody.Add(line);
                }
            }
            body = replacedBody.Join("\n");
            return body;
        }

        public bool GetNoDisplayIfReadOnly(Context context)
        {
            return context.HasPrivilege
                ? false
                : PermissionType == Permissions.Types.Read && NoDisplayIfReadOnly;
        }

        public void LinkActions(
            Context context,
            string type,
            Dictionary<string, string> data = null,
            SqlSelect sub = null)
        {
            Sources?.Values.ForEach(ss => ss?.Links?
                .Where(link => link.SiteId == SiteId)
                .ForEach(link =>
                    link.Action(
                        context: context,
                        siteId: ss.SiteId,
                        type: type,
                        data: data,
                        sub: sub)));
        }

        public ServerScriptModelRow GetServerScriptModelRow(
            Context context,
            BaseItemModel itemModel = null,
            View view = null,
            GridData gridData = null)
        {
            if (ServerScriptModelRowCache == null)
            {
                ServerScriptModelRowCache = itemModel != null
                    ? itemModel.SetByBeforeOpeningPageServerScript(
                        context: context,
                        ss: this,
                        view: view,
                        gridData: gridData)
                    : new ItemModel().SetByBeforeOpeningPageServerScript(
                        context: context,
                        ss: this,
                        view: view,
                        gridData: gridData);
            }
            return ServerScriptModelRowCache;
        }

        public Process GetProcess(Context context, int id)
        {
            return Processes
                ?.Where(o => o.Accessable(
                    context: context,
                    ss: this))
                .FirstOrDefault(o => o.Id == id);
        }
    }
}