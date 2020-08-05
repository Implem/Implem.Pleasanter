using Implem.DefinitionAccessor;
using Implem.IRds;
using Implem.Libraries.DataSources.Interfaces;
using Implem.Libraries.DataSources.SqlServer;
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
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
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
            Right = 20
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
        public long SiteId;
        [NonSerialized]
        public long ReferenceId;
        [NonSerialized]
        public string Title;
        [NonSerialized]
        public string Body;
        [NonSerialized]
        public string GridGuide;
        [NonSerialized]
        public string EditorGuide;
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
        public Permissions.Types? ItemPermissionType;
        [NonSerialized]
        public bool Publish;
        [NonSerialized]
        public Databases.AccessStatuses AccessStatus;
        [NonSerialized]
        public Dictionary<string, Column> ColumnHash;
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
        public string ReferenceType;
        public decimal? NearCompletionTimeAfterDays;
        public decimal? NearCompletionTimeBeforeDays;
        public int? GridPageSize;
        public int? GridView;
        public GridEditorTypes? GridEditorType;
        public bool? HistoryOnGrid;
        public bool? AlwaysRequestSearchCondition;
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
        public int? ViewLatestId;
        public List<View> Views;
        public SettingList<Notification> Notifications;
        public SettingList<Reminder> Reminders;
        public SettingList<Export> Exports;
        public SettingList<Style> Styles;
        public SettingList<Script> Scripts;
        public SettingList<RelatingColumn> RelatingColumns;
        public string ExtendedHeader;
        public Versions.AutoVerUpTypes? AutoVerUpType;
        public bool? AllowEditingComments;
        public bool? AllowSeparate;
        public bool? AllowLockTable;
        public bool? SwitchRecordWithAjax;
        public bool? EnableCalendar;
        public bool? EnableCrosstab;
        public bool? EnableGantt;
        public bool? ShowGanttProgressRate;
        public bool? EnableBurnDown;
        public bool? EnableTimeSeries;
        public bool? EnableKamban;
        public bool? EnableImageLib;
        public int? ImageLibPageSize;
        public bool? UseFiltersArea;
        public bool? UseGridHeaderFilters;
        public bool? UseRelatingColumnsOnFilter;
        public string TitleSeparator;
        public SearchTypes? SearchType;
        public SaveViewTypes? SaveViewType;
        public string AddressBook;
        public string MailToDefault;
        public string MailCcDefault;
        public string MailBccDefault;
        public List<long> IntegratedSites;
        public Dictionary<string, Permissions.Types> PermissionForCreating;
        public List<ColumnAccessControl> CreateColumnAccessControls;
        public List<ColumnAccessControl> ReadColumnAccessControls;
        public List<ColumnAccessControl> UpdateColumnAccessControls;
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
            Version = SiteSettingsUtilities.Version;
            NearCompletionTimeBeforeDays = NearCompletionTimeBeforeDays ??
                Parameters.General.NearCompletionTimeBeforeDays;
            NearCompletionTimeAfterDays = NearCompletionTimeAfterDays ??
                Parameters.General.NearCompletionTimeAfterDays;
            GridPageSize = GridPageSize ?? Parameters.General.GridPageSize;
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
            Update_ColumnAccessControls();
            if (context.TrashboxActions())
            {
                GridColumns.RemoveAll(o => o.Contains("~~"));
                FilterColumns.RemoveAll(o => o.Contains("~~"));
            }
            if (Aggregations == null) Aggregations = new List<Aggregation>();
            if (Links == null) Links = new List<Link>();
            if (Summaries == null) Summaries = new SettingList<Summary>();
            if (Formulas == null) Formulas = new SettingList<FormulaSet>();
            ViewLatestId = ViewLatestId ?? 0;
            if (Notifications == null) Notifications = new SettingList<Notification>();
            if (Reminders == null) Reminders = new SettingList<Reminder>();
            if (Exports == null) Exports = new SettingList<Export>();
            if (Styles == null) Styles = new SettingList<Style>();
            if (Scripts == null) Scripts = new SettingList<Script>();
            if (RelatingColumns == null) RelatingColumns = new SettingList<RelatingColumn>();
            AutoVerUpType = AutoVerUpType ?? Versions.AutoVerUpTypes.Default;
            AllowEditingComments = AllowEditingComments ?? false;
            AllowSeparate = AllowSeparate ?? false;
            AllowLockTable = AllowLockTable ?? false;
            SwitchRecordWithAjax = SwitchRecordWithAjax ?? false;
            EnableCalendar = EnableCalendar ?? true;
            EnableCrosstab = EnableCrosstab ?? true;
            EnableGantt = EnableGantt ?? true;
            ShowGanttProgressRate = ShowGanttProgressRate ?? true;
            EnableBurnDown = EnableBurnDown ?? true;
            EnableTimeSeries = EnableTimeSeries ?? true;
            EnableKamban = EnableKamban ?? true;
            EnableImageLib = EnableImageLib ?? true;
            ImageLibPageSize = ImageLibPageSize ?? Parameters.General.ImageLibPageSize;
            TitleSeparator = TitleSeparator ?? ")";
            UseFiltersArea = UseFiltersArea ?? true;
            UseGridHeaderFilters = UseGridHeaderFilters ?? false;
            UseRelatingColumnsOnFilter = UseRelatingColumnsOnFilter ?? false;
            SearchType = SearchType ?? SearchTypes.PartialMatch;
            SaveViewType = SaveViewType ?? SaveViewTypes.Session;
        }

        public void SetLinkedSiteSettings(
            Context context,
            Dictionary<long, SiteSettings> joinedSsHash = null,
            bool destinations = true,
            bool sources = true,
            List<long> previously = null)
        {
            var dataSet = Repository.ExecuteDataSet(
                context: context,
                statements: new SqlStatement[]
                {
                    Rds.SelectSites(
                        dataTableName: "Destinations",
                        column: Rds.SitesColumn()
                            .SiteId()
                            .Title()
                            .Body()
                            .GridGuide()
                            .EditorGuide()
                            .ReferenceType()
                            .ParentId()
                            .InheritPermission()
                            .SiteSettings(),
                        where: Rds.SitesWhere()
                            .TenantId(context.TenantId)
                            .SiteId_In(sub: Rds.SelectLinks(
                                column: Rds.LinksColumn().DestinationId(),
                                where: Rds.LinksWhere().SourceId(SiteId)))
                            .ReferenceType("Wikis", _operator: "<>"),
                        _using: destinations),
                    Rds.SelectSites(
                        dataTableName: "Sources",
                        column: Rds.SitesColumn()
                            .SiteId()
                            .Title()
                            .Body()
                            .GridGuide()
                            .EditorGuide()
                            .ReferenceType()
                            .ParentId()
                            .InheritPermission()
                            .SiteSettings(),
                        where: Rds.SitesWhere()
                            .TenantId(context.TenantId)
                            .SiteId_In(sub: Rds.SelectLinks(
                                column: Rds.LinksColumn().SourceId(),
                                where: Rds.LinksWhere().DestinationId(SiteId)))
                            .ReferenceType("Wikis", _operator: "<>"),
                        _using: sources)
                });
            if (joinedSsHash == null)
            {
                joinedSsHash = new Dictionary<long, SiteSettings>()
                {
                    { SiteId, this }
                };
                JoinedSsHash = joinedSsHash;
            }
            if (destinations)
            {
                Destinations = SiteSettingsList(
                    context: context,
                    dataSet: dataSet,
                    direction: "Destinations",
                    joinedSsHash: joinedSsHash,
                    joinStacks: JoinStacks,
                    links: Links,
                    previously: previously);
            }
            if (sources)
            {
                Sources = SiteSettingsList(
                    context: context,
                    dataSet: dataSet,
                    direction: "Sources",
                    joinedSsHash: joinedSsHash,
                    joinStacks: JoinStacks,
                    links: Links,
                    previously: previously);
            }
            if (destinations && sources)
            {
                SetRelatingColumnsLinkedClass();
            }
        }

        private Dictionary<long, SiteSettings> SiteSettingsList(
            Context context,
            DataSet dataSet,
            string direction,
            Dictionary<long, SiteSettings> joinedSsHash,
            List<JoinStack> joinStacks,
            List<Link> links,
            List<long> previously)
        {
            var hash = new Dictionary<long, SiteSettings>();
            dataSet.Tables[direction].AsEnumerable()
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
                    ss.ReferenceType = dataRow.String("ReferenceType");
                    ss.ParentId = dataRow.Long("ParentId");
                    ss.InheritPermission = dataRow.Long("InheritPermission");
                    ss.Linked = true;
                    if (previously == null) previously = new List<long>();
                    previously.Add(ss.SiteId);
                    switch (direction)
                    {
                        case "Destinations":
                            links
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
                                previously: previously);
                            break;
                        case "Sources":
                            ss.Links
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
                                previously: previously);
                            break;
                    }
                    hash.Add(ss.SiteId, ss);
                    if (!joinedSsHash.ContainsKey(ss.SiteId))
                    {
                        ss.Update_ColumnAccessControls();
                        joinedSsHash.Add(ss.SiteId, ss);
                    }
                });
            return hash;
        }

        public void SetPermissions(Context context, long referenceId)
        {
            var targets = new List<long> { InheritPermission, referenceId };
            targets.AddRange(Destinations
                ?.Values
                .Select(o => o.InheritPermission) ?? new List<long>());
            targets.AddRange(Sources
                ?.Values
                .Select(o => o.InheritPermission) ?? new List<long>());
            SetPermissions(
                context: context,
                ss: this,
                referenceId: referenceId);
            Destinations?.Values.ForEach(ss =>
                SetPermissions(
                    context: context,
                    ss: ss));
            Sources?.Values.ForEach(ss => SetPermissions(
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
            return IsSite(context: context) && context.Action == "edit";
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
            if (AllowSeparate == true)
            {
                ss.AllowSeparate = AllowSeparate;
            }
            if (AllowLockTable == true)
            {
                ss.AllowLockTable = AllowLockTable;
            }
            if (SwitchRecordWithAjax == true)
            {
                ss.SwitchRecordWithAjax = SwitchRecordWithAjax;
            }
            if (EnableCalendar == false)
            {
                ss.EnableCalendar = EnableCalendar;
            }
            if (EnableCrosstab == false)
            {
                ss.EnableCrosstab = EnableCrosstab;
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
            if (EnableKamban == false)
            {
                ss.EnableKamban = EnableKamban;
            }
            if (EnableImageLib == false)
            {
                ss.EnableImageLib = EnableImageLib;
            }
            if (UseFiltersArea == false)
            {
                ss.UseFiltersArea = UseFiltersArea;
            }
            if (UseGridHeaderFilters == true)
            {
                ss.UseGridHeaderFilters = UseGridHeaderFilters;
            }
            if (UseRelatingColumnsOnFilter == true)
            {
                ss.UseRelatingColumnsOnFilter = UseRelatingColumnsOnFilter;
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
                ss.Links.Add(link.GetRecordingData());
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
                ss.Views.Add(view.GetRecordingData(ss: this));
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
            Exports?.ForEach(exportSetting =>
            {
                if (ss.Exports == null)
                {
                    ss.Exports = new SettingList<Export>();
                }
                ss.Exports.Add(exportSetting.GetRecordingData());
            });
            Styles?.ForEach(style =>
            {
                if (ss.Styles == null)
                {
                    ss.Styles = new SettingList<Style>();
                }
                ss.Styles.Add(style.GetRecordingData());
            });
            Scripts?.ForEach(script =>
            {
                if (ss.Scripts == null)
                {
                    ss.Scripts = new SettingList<Script>();
                }
                ss.Scripts.Add(script.GetRecordingData());
            });
            RelatingColumns?.ForEach(relatingColumn =>
            {
                if (ss.RelatingColumns == null)
                {
                    ss.RelatingColumns = new SettingList<RelatingColumn>();
                }
                ss.RelatingColumns.Add(relatingColumn.GetRecordingData());
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
            PermissionForCreating?.Where(o => o.Value > 0).ForEach(data =>
            {
                if (ss.PermissionForCreating == null)
                {
                    ss.PermissionForCreating = new Dictionary<string, Permissions.Types>();
                }
                ss.PermissionForCreating.Add(data.Key, data.Value);
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
                    if (column.ChoicesText != columnDefinition.ChoicesText)
                    {
                        enabled = true;
                        newColumn.ChoicesText = column.ChoicesText;
                    }
                    if (column.UseSearch != columnDefinition.UseSearch)
                    {
                        enabled = true;
                        newColumn.UseSearch = column.UseSearch;
                    }
                    if (column.DefaultInput != columnDefinition.DefaultInput)
                    {
                        enabled = true;
                        newColumn.DefaultInput = column.DefaultInput;
                    }
                    if (column.MaxLength != null)
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
                    if (column.ExtendedFieldCss?.Trim().IsNullOrEmpty() == false)
                    {
                        enabled = true;
                        newColumn.ExtendedFieldCss = column.ExtendedFieldCss;
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
                    if (column.CopyByDefault == true)
                    {
                        enabled = true;
                        newColumn.CopyByDefault = column.CopyByDefault;
                    }
                    if (column.EditorReadOnly != columnDefinition.EditorReadOnly)
                    {
                        enabled = true;
                        newColumn.EditorReadOnly = column.EditorReadOnly;
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
                    if (column.FieldCss != columnDefinition.FieldCss)
                    {
                        enabled = true;
                        newColumn.FieldCss = column.FieldCss;
                    }
                    if (column.TextAlign != TextAlignTypes.Left)
                    {
                        enabled = true;
                        newColumn.TextAlign = column.TextAlign;
                    }
                    if (column.Unit != columnDefinition.Unit)
                    {
                        enabled = true;
                        newColumn.Unit = column.Unit;
                    }
                    if (column.Link == true)
                    {
                        enabled = true;
                        newColumn.Link = column.Link;
                    }
                    if (column.CheckFilterControlType != ColumnUtilities.CheckFilterControlTypes.OnOnly)
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
                    if (column.DateFilterSetMode != ColumnUtilities.DateFilterSetMode.Default)
                    {
                        enabled = true;
                        newColumn.DateFilterSetMode = column.DateFilterSetMode;
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
            ColumnDefinitionHash = GetColumnDefinitionHash(ReferenceType);
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

        private void UpdateColumns(Context context, bool onSerializing = false)
        {
            if (Columns == null) Columns = new List<Column>();
            ColumnDefinitionHash?.Values.ForEach(columnDefinition =>
            {
                if (!onSerializing)
                {
                    var column = Columns.FirstOrDefault(o =>
                        o.ColumnName == columnDefinition.ColumnName);
                    if (column == null)
                    {
                        column = new Column(columnDefinition.ColumnName);
                        Columns.Add(column);
                    }
                    UpdateColumn(
                        context: context,
                        ss: this,
                        columnDefinition: columnDefinition,
                        column: column);
                }
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
                column.No = columnDefinition.No;
                column.Id_Ver =
                    (columnDefinition.Unique && columnDefinition.TypeName == "bigint") ||
                    columnDefinition.ColumnName == "Ver";
                column.ColumnName = column.ColumnName ?? columnDefinition.ColumnName;
                column.LabelText = ModifiedLabelText(
                    context: context,
                    column: column,
                    columnDefinition: columnDefinition)
                        ?? column.LabelText
                        ?? Displays.Get(
                            context: context,
                            id: columnDefinition.Id);
                column.GridLabelText = ModifiedLabelText(
                    context: context,
                    column: column,
                    columnDefinition: columnDefinition)
                        ?? column.GridLabelText
                        ?? column.LabelText;
                column.ChoicesText = column.ChoicesText ?? columnDefinition.ChoicesText;
                column.UseSearch = column.UseSearch ?? columnDefinition.UseSearch;
                column.DefaultInput = column.DefaultInput ?? columnDefinition.DefaultInput;
                column.GridFormat = column.GridFormat ?? columnDefinition.GridFormat;
                column.EditorFormat = column.EditorFormat ?? columnDefinition.EditorFormat;
                column.ExportFormat = column.ExportFormat ?? columnDefinition.ExportFormat;
                column.ControlType = column.ControlType ?? columnDefinition.ControlType;
                column.ValidateRequired = column.ValidateRequired ?? columnDefinition.ValidateRequired;
                column.ValidateNumber = column.ValidateNumber ?? columnDefinition.ValidateNumber;
                column.ValidateDate = column.ValidateDate ?? columnDefinition.ValidateDate;
                column.ValidateEmail = column.ValidateEmail ?? columnDefinition.ValidateEmail;
                column.ValidateEqualTo = column.ValidateEqualTo ?? columnDefinition.ValidateEqualTo;
                column.ValidateMaxLength = column.ValidateMaxLength ?? columnDefinition.MaxLength;
                column.ClientRegexValidation = column.ClientRegexValidation ?? columnDefinition.ClientRegexValidation;
                column.ServerRegexValidation = column.ServerRegexValidation ?? columnDefinition.ServerRegexValidation;
                column.RegexValidationMessage = column.RegexValidationMessage ?? columnDefinition.RegexValidationMessage;
                column.DecimalPlaces = column.DecimalPlaces ?? columnDefinition.DecimalPlaces;
                column.RoundingType = column.RoundingType ?? RoundingTypes.AwayFromZero;
                column.Min = column.Min ?? DefaultMin(columnDefinition);
                column.Max = column.Max ?? DefaultMax(columnDefinition);
                column.Step = column.Step ?? DefaultStep(columnDefinition);
                column.DefaultMinValue = column.DefaultMinValue ?? columnDefinition.DefaultMinValue;
                column.DefaultMaxValue = column.DefaultMaxValue ?? columnDefinition.DefaultMaxValue;
                column.NoDuplication = column.NoDuplication ?? false;
                column.CopyByDefault = column.CopyByDefault ?? false;
                column.EditorReadOnly = column.EditorReadOnly ?? columnDefinition.EditorReadOnly;
                column.AllowBulkUpdate = column.AllowBulkUpdate ?? false;
                column.AllowImage = column.AllowImage ?? true;
                column.ThumbnailLimitSize = column.ThumbnailLimitSize ?? Parameters.BinaryStorage.ThumbnailLimitSize;
                column.FieldCss = column.FieldCss ?? columnDefinition.FieldCss;
                column.TextAlign = column.TextAlign ?? TextAlignTypes.Left;
                column.Unit = column.Unit ?? columnDefinition.Unit;
                column.CheckFilterControlType = column.CheckFilterControlType ?? ColumnUtilities.CheckFilterControlTypes.OnOnly;
                column.NumFilterMin = column.NumFilterMin ?? columnDefinition.NumFilterMin;
                column.NumFilterMax = column.NumFilterMax ?? columnDefinition.NumFilterMax;
                column.NumFilterStep = column.NumFilterStep ?? columnDefinition.NumFilterStep;
                column.DateFilterSetMode = column.DateFilterSetMode ?? ColumnUtilities.DateFilterSetMode.Default;
                column.DateFilterMinSpan = column.DateFilterMinSpan ?? Parameters.General.DateFilterMinSpan;
                column.DateFilterMaxSpan = column.DateFilterMaxSpan ?? Parameters.General.DateFilterMaxSpan;
                column.DateFilterFy = column.DateFilterFy ?? true;
                column.DateFilterHalf = column.DateFilterHalf ?? true;
                column.DateFilterQuarter = column.DateFilterQuarter ?? true;
                column.DateFilterMonth = column.DateFilterMonth ?? true;
                column.LimitQuantity = column.LimitQuantity ?? Parameters.BinaryStorage.LimitQuantity;
                column.LimitSize = column.LimitSize ?? Parameters.BinaryStorage.LimitSize;
                column.TotalLimitSize = column.TotalLimitSize ?? Parameters.BinaryStorage.LimitTotalSize;
                column.Size = columnDefinition.Size;
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
                column.Joined = columnNameInfo.Joined;
            }
        }

        private string ModifiedLabelText(
            Context context, Column column, ColumnDefinition columnDefinition)
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

        private void Update_ColumnAccessControls()
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

        private void Update_UpdateColumnAccessControls(IEnumerable<Column> columns)
        {
            var columnAccessControls = columns
                .Select(column => new ColumnAccessControl(this, column, "Update"))
                .ToList();
            UpdateColumnAccessControls?.ForEach(o =>
                SetColumnAccessControl(columnAccessControls, o));
            UpdateColumnAccessControls = columnAccessControls;
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

        public void SetColumnAccessControls(Context context, List<string> mine = null)
        {
            ColumnHash.Values.ForEach(column =>
            {
                SetColumnAccessControls(
                    context: context,
                    column: column,
                    mine: mine);
            });
        }

        private void SetColumnAccessControls(
            Context context, Column column, List<string> mine = null)
        {
            var ss = column.SiteSettings;
            ss.CreateColumnAccessControls
                .Where(o => o.ColumnName == column.Name)
                .ForEach(o => column.CanCreate = o.Allowed(
                    context: context,
                    ss: this,
                    type: PermissionType,
                    mine: mine));
            ss.ReadColumnAccessControls
                .Where(o => o.ColumnName == column.Name)
                .ForEach(o => column.CanRead = o.Allowed(
                    context: context,
                    ss: this,
                    type: PermissionType,
                    mine: mine));
            ss.UpdateColumnAccessControls
                .Where(o => o.ColumnName == column.Name)
                .ForEach(o => column.CanUpdate = o.Allowed(
                    context: context,
                    ss: this,
                    type: PermissionType,
                    mine: mine));
        }

        private decimal DefaultMin(ColumnDefinition columnDefinition)
        {
            return columnDefinition.ExtendedColumnType == "Num"
                && columnDefinition.DefaultMinValue != 0
                    ? columnDefinition.DefaultMinValue
                    : columnDefinition.Min;
        }

        private decimal DefaultMax(ColumnDefinition columnDefinition)
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

        public Column GetColumn(Context context, string columnName)
        {
            var column = ColumnHash.Get(columnName);
            if (column == null &&
                columnName?.Contains(',') == true &&
                JoinOptions().ContainsKey(columnName.Split_1st()) == true)
            {
                column = AddJoinedColumn(context: context, columnName: columnName);
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
            var columnDefinition = ss?.ColumnDefinitionHash.Get(columnNameInfo.Name);
            if (columnDefinition != null)
            {
                var column = ss.ColumnHash
                    .Get(columnNameInfo.Name)?
                    .Copy();
                if (column != null)
                {
                    column.ColumnName = columnName;
                    UpdateColumn(
                        context: context,
                        ss: ss,
                        columnDefinition: columnDefinition,
                        column: column,
                        columnNameInfo: columnNameInfo);
                    SetColumnAccessControls(
                        context: context,
                        column: column);
                    column.ChoiceHash = ss.ColumnHash
                        .Get(columnNameInfo.Name)
                        .ChoiceHash
                        ?.ToDictionary(o => o.Key, o => o.Value);
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
            return column?.Linked() == true
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

        public Column FormulaColumn(string name)
        {
            return Columns
                .Where(o => o.ColumnName == name || o.LabelText == name)
                .Where(o => o.TypeName == "decimal")
                .Where(o => !o.NotUpdate)
                .Where(o => !o.Joined)
                .FirstOrDefault();
        }

        public List<Column> GetGridColumns(
            Context context, View view = null, bool checkPermission = false)
        {
            var columns = (view?.GridColumns ?? GridColumns)
                .Select(columnName => GetColumn(context: context, columnName: columnName))
                .Where(column => column != null)
                .AllowedColumns(checkPermission: checkPermission)
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
            return columns;
        }

        public List<Column> GetLinkTableColumns(
            Context context, View view = null, bool checkPermission = false)
        {
            return (view?.GridColumns ?? LinkColumns)
                .Select(columnName => GetColumn(context: context, columnName: columnName))
                .Where(column => column != null)
                .Where(column => !column.Joined)
                .AllowedColumns(checkPermission: checkPermission)
                .Where(o => context.ContractSettings.Attachments()
                    || o.ControlType != "Attachments")
                .ToList();
        }

        public List<Column> GetFilterColumns(Context context, bool checkPermission = false)
        {
            return FilterColumns
                .Select(columnName => GetColumn(context: context, columnName: columnName))
                .Where(column => column != null)
                .AllowedColumns(checkPermission: checkPermission)
                .ToList();
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

        public List<string> GetEditorColumnNames()
        {
            return (EditorColumnHash.Get(TabName(0))
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

        public List<Column> GetAllowBulkUpdateColumns(Context context, SiteSettings ss)
        {
            return GetEditorColumns(context: context)
                .Where(c => !c.Id_Ver)
                .Where(c => c.EditorReadOnly != true)
                .Where(c => c.NoDuplication != true)
                .Where(c => c.ColumnName != "Comments")
                .Where(column => !Formulas.Any(formulaSet =>
                    formulaSet.Target == column.ColumnName
                    || ContainsFormulaColumn(
                        columnName: column.ColumnName,
                        children: formulaSet.Formula.Children)))
                .Where(column => column.AllowBulkUpdate == true)
                .Where(column => column.CanUpdate)
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
                .AllowedColumns(checkPermission: checkPermission)
                .Where(o => context.ContractSettings.Attachments()
                    || o.ControlType != "Attachments")
                .ToList();
        }

        public List<Column> GetHistoryColumns(Context context, bool checkPermission = false)
        {
            return HistoryColumns
                .Select(columnName => GetColumn(context: context, columnName: columnName))
                .Where(column => column != null)
                .AllowedColumns(checkPermission: checkPermission)
                .Where(o => context.ContractSettings.Attachments()
                    || o.ControlType != "Attachments")
                .ToList();
        }

        public IEnumerable<Column> SelectColumns()
        {
            return Columns
                ?.Where(o => !o.NotSelect)
                .Where(o => o.Required
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

        public Dictionary<string, string> ViewFilterOptions(Context context, View view)
        {
            var hash = new Dictionary<string, string>();
            JoinOptions().ForEach(join =>
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

        public Dictionary<string, string> ViewSorterOptions(Context context)
        {
            var hash = new Dictionary<string, string>();
            JoinOptions().ForEach(join =>
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
            Context context, bool enabled = true)
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
                        .OrderBy(o => o.EditorColumn)
                        .Select(o => o.ColumnName),
                    order: ColumnDefinitionHash?.EditorDefinitions(context: context)?
                        .OrderBy(o => o.EditorColumn)
                        .Select(o => o.ColumnName).ToList());
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

        public Dictionary<string, ControlData> MoveTargetsSelectableOptions(
            Context context, bool enabled = true)
        {
            var options = MoveTargetsOptions(sites: Repository.ExecuteTable(
                context: context,
                statements: new SqlStatement(
                    commandText: Def.Sql.MoveTarget,
                    param: Rds.SitesParam()
                        .TenantId(context.TenantId)
                        .ReferenceType(ReferenceType)
                        .SiteId(SiteId)
                        .Add(name: "HasPrivilege", value: context.HasPrivilege)))
                            .AsEnumerable());
            return enabled
                ? MoveTargets?.Any() == true
                    ? options
                        .Where(o => MoveTargets.Contains(o.Key.ToLong()))
                        .ToDictionary(o => o.Key, o => o.Value)
                    : null
                : options
                    .Where(o => MoveTargets?.Contains(o.Key.ToLong()) != true)
                    .ToDictionary(o => o.Key, o => o.Value);
        }

        private Dictionary<string, ControlData> MoveTargetsOptions(IEnumerable<DataRow> sites)
        {
            var targets = new Dictionary<string, ControlData>();
            sites
                .Where(dataRow => dataRow.String("ReferenceType") == ReferenceType)
                .ForEach(dataRow =>
                {
                    var current = dataRow;
                    var titles = new List<string>()
                    {
                        current.String("Title")
                    };
                    while (sites.Any(o => o.Long("SiteId") == current.Long("ParentId")))
                    {
                        current = sites.First(o => o.Long("SiteId") == current.Long("ParentId"));
                        titles.Insert(0, current.String("Title"));
                    }
                    targets.Add(
                        dataRow.String("SiteId"),
                        new ControlData(titles.Join(" / ")));
                });
            return targets;
        }

        public Dictionary<string, ControlData> FormulaTargetSelectableOptions()
        {
            return Columns
                .Where(o => o.TypeName == "decimal")
                .Where(o => !o.NotUpdate)
                .Where(o => !o.Joined)
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

        public Dictionary<string, string> ExportJoinOptions(Context context)
        {
            return TableJoins(
                context: context,
                links: Links,
                join: new Join(Title),
                joins: new List<Join> { new Join(Title) })
                    .ToDictionary(o => o.ToJson(), o => o.GetTitle());
        }

        private List<Join> TableJoins(
            Context context, List<Link> links, Join join, List<Join> joins)
        {
            links?
                .Where(o =>
                    o.SiteId != SiteId &&
                    !join.Any(p => p.SiteId == o.SiteId))
                .ForEach(link =>
                {
                    var ss = JoinedSsHash.Get(link.SiteId);
                    if (ss != null)
                    {
                        var column = ss.GetColumn(
                            context: context,
                            columnName: link.ColumnName);
                        if (column != null)
                        {
                            var copy = join.ToList();
                            copy.Add(link, ss.Title);
                            joins.Add(copy);
                            TableJoins(
                                context: context,
                                links: ss?.Links,
                                join: copy,
                                joins: joins);
                        }
                    }
                });
            return joins;
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
                { "Monthly", Displays.Month(context: context) }
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
                .Where(o => o.CanRead)
                .OrderBy(o => o.No)
                .ToDictionary(o => o.ColumnName, o => o.GridLabelText));
            return hash;
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
                        .Where(o => !o.Joined)
                        .Where(o => ss.GetEditorColumnNames().Contains(o.Name))
                        .Where(o => o.CanRead)
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

        public Dictionary<string, string> CrosstabColumnsOptions()
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
                        .Where(o => o.CanRead)
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

        public Dictionary<string, string> GanttGroupByOptions()
        {
            return Columns
                .Where(o => o.HasChoices())
                .Where(o => !o.Joined)
                .Where(o => o.CanRead)
                .OrderBy(o => o.No)
                .ToDictionary(o => o.ColumnName, o => o.GridLabelText);
        }

        public Dictionary<string, string> GanttSortByOptions(Context context)
        {
            return GetEditorColumnNames()
                .Select(columnName => GetColumn(context: context, columnName: columnName))
                .Where(column => column != null)
                .Where(column => column.CanRead)
                .ToDictionary(o => o.ColumnName, o => o.GridLabelText);
        }

        public Dictionary<string, string> TimeSeriesGroupByOptions()
        {
            return Columns
                .Where(o => o.HasChoices())
                .Where(o => !o.Joined)
                .Where(o => o.CanRead)
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

        public Dictionary<string, string> TimeSeriesValueOptions()
        {
            return Columns
                .Where(o => o.Computable)
                .Where(o => o.TypeName != "datetime")
                .Where(o => !o.Joined)
                .Where(o => o.CanRead)
                .OrderBy(o => o.No)
                .ToDictionary(o => o.ColumnName, o => o.LabelText);
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
                .Where(o => !o.Joined)
                .Where(o => o.CanRead)
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

        public Dictionary<string, string> KambanValueOptions()
        {
            return Columns
                .Where(o => o.Computable)
                .Where(o => o.TypeName != "datetime")
                .Where(o => !o.Joined)
                .Where(o => o.CanRead)
                .OrderBy(o => o.No)
                .ToDictionary(o => o.ColumnName, o => o.GridLabelText);
        }

        public Dictionary<string, string> JoinOptions(
            SiteSettings ss = null,
            bool destinations = true,
            bool sources = true)
        {
            var hash = new Dictionary<string, string>();
            if (ss == null)
            {
                ss = this;
                hash.Add(string.Empty, $"[{Title}]");
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

        public List<Column> ReadableColumns(bool noJoined = false)
        {
            return Columns
                .Where(o =>
                    GetEditorColumnNames().Contains(o.ColumnName)
                    || GridColumns.Contains(o.ColumnName)
                    || !Def.ExtendedColumnTypes.ContainsKey(o.ColumnName))
                .Where(o => !noJoined || !o.Joined)
                .Where(o => o.CanRead)
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
                case "NearCompletionTimeBeforeDays": NearCompletionTimeBeforeDays = value.ToInt(); break;
                case "NearCompletionTimeAfterDays": NearCompletionTimeAfterDays = value.ToInt(); break;
                case "GridPageSize": GridPageSize = value.ToInt(); break;
                case "GridView": GridView = value.ToInt(); break;
                case "GridEditorType": GridEditorType = (GridEditorTypes)value.ToInt(); break;
                case "HistoryOnGrid": HistoryOnGrid = value.ToBool(); break;
                case "AlwaysRequestSearchCondition": AlwaysRequestSearchCondition = value.ToBool(); break;
                case "LinkTableView": LinkTableView = value.ToInt(); break;
                case "FirstDayOfWeek": FirstDayOfWeek = value.ToInt(); break;
                case "FirstMonth": FirstMonth = value.ToInt(); break;
                case "AutoVerUpType": AutoVerUpType = (Versions.AutoVerUpTypes)value.ToInt(); break;
                case "AllowEditingComments": AllowEditingComments = value.ToBool(); break;
                case "AllowSeparate": AllowSeparate = value.ToBool(); break;
                case "AllowLockTable": AllowLockTable = value.ToBool(); break;
                case "SwitchRecordWithAjax": SwitchRecordWithAjax = value.ToBool(); break;
                case "EnableCalendar": EnableCalendar = value.ToBool(); break;
                case "EnableCrosstab": EnableCrosstab = value.ToBool(); break;
                case "EnableGantt": EnableGantt = value.ToBool(); break;
                case "ShowGanttProgressRate": ShowGanttProgressRate = value.ToBool(); break;
                case "EnableBurnDown": EnableBurnDown = value.ToBool(); break;
                case "EnableTimeSeries": EnableTimeSeries = value.ToBool(); break;
                case "EnableKamban": EnableKamban = value.ToBool(); break;
                case "EnableImageLib": EnableImageLib = value.ToBool(); break;
                case "UseFiltersArea": UseFiltersArea = value.ToBool(); break;
                case "UseGridHeaderFilters": UseGridHeaderFilters = value.ToBool(); break;
                case "UseRelatingColumnsOnFilter": UseRelatingColumnsOnFilter = value.ToBool(); break;
                case "ImageLibPageSize": ImageLibPageSize = value.ToInt(); break;
                case "SearchType": SearchType = (SearchTypes)value.ToInt(); break;
                case "SaveViewType": SaveViewType = (SaveViewTypes)value.ToInt(); break;
                case "AddressBook": AddressBook = value; break;
                case "MailToDefault": MailToDefault = value; break;
                case "MailCcDefault": MailCcDefault = value; break;
                case "MailBccDefault": MailBccDefault = value; break;
                case "IntegratedSites": SetIntegratedSites(value); break;
                case "CurrentPermissionForCreatingAll": SetPermissionForCreating(value); break;
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
                case "Format": column.Format = value; break;
                case "NoWrap": column.NoWrap = value.ToBool(); break;
                case "Hide": column.Hide = value.ToBool(); break;
                case "ExtendedFieldCss": column.ExtendedFieldCss = value; break;
                case "Section": column.Section = value; break;
                case "GridDesign":
                    column.GridDesign = LabelTextToColumnName(column, value);
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
                case "DecimalPlaces": column.DecimalPlaces = value.ToInt(); break;
                case "RoundingType": column.RoundingType = (RoundingTypes)value.ToInt(); break;
                case "Max": column.Max = value.ToDecimal(); break;
                case "Min": column.Min = value.ToDecimal(); break;
                case "Step": column.Step = value.ToDecimal(); break;
                case "NoDuplication": column.NoDuplication = value.ToBool(); break;
                case "CopyByDefault": column.CopyByDefault = value.ToBool(); break;
                case "EditorReadOnly": column.EditorReadOnly = value.ToBool(); break;
                case "AllowBulkUpdate": column.AllowBulkUpdate = value.ToBool(); break;
                case "AllowImage": column.AllowImage = value.ToBool(); break;
                case "ThumbnailLimitSize": column.ThumbnailLimitSize = value.ToDecimal(); break;
                case "FieldCss": column.FieldCss = value; break;
                case "TextAlign": column.TextAlign = (TextAlignTypes)value.ToInt(); break;
                case "Description": column.Description = value; break;
                case "ChoicesText": column.ChoicesText = value; SetLinks(
                    context: context, column: column); break;
                case "UseSearch": column.UseSearch = value.ToBool(); break;
                case "DefaultInput": column.DefaultInput = value; break;
                case "GridFormat": column.GridFormat = value; break;
                case "EditorFormat": column.EditorFormat = value; break;
                case "ExportFormat": column.ExportFormat = value; break;
                case "Unit": column.Unit = value; break;
                case "CheckFilterControlType": column.CheckFilterControlType =
                        (ColumnUtilities.CheckFilterControlTypes)value.ToInt(); break;
                case "NumFilterMin": column.NumFilterMin = value.ToDecimal(); break;
                case "NumFilterMax": column.NumFilterMax = value.ToDecimal(); break;
                case "NumFilterStep": column.NumFilterStep = value.ToDecimal(); break;
                case "DateFilterSetMode": column.DateFilterSetMode =
                        (ColumnUtilities.DateFilterSetMode)value.ToInt(); break;
                case "DateFilterMinSpan": column.DateFilterMinSpan = value.ToInt(); break;
                case "DateFilterMaxSpan": column.DateFilterMaxSpan = value.ToInt(); break;
                case "DateFilterFy": column.DateFilterFy = value.ToBool(); break;
                case "DateFilterHalf": column.DateFilterHalf = value.ToBool(); break;
                case "DateFilterQuarter": column.DateFilterQuarter = value.ToBool(); break;
                case "DateFilterMonth": column.DateFilterMonth = value.ToBool(); break;
                case "LimitQuantity": column.LimitQuantity = value.ToDecimal(); break;
                case "LimitSize": column.LimitSize = value.ToDecimal(); break;
                case "LimitTotalSize": column.TotalLimitSize = value.ToDecimal(); break;
                case "TitleSeparator": TitleSeparator = value; break;
            }
        }

        private string LabelTextToColumnName(Column currentColumn, string value)
        {
            if (!value.IsNullOrEmpty())
            {
                value = LabelTextToColumnName(value);
                return value != $"[{currentColumn.ColumnName}]"
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
                    var column = labelText
                        ? Columns.FirstOrDefault(o =>
                            o.LabelText == match.Value)
                        : Columns.FirstOrDefault(o =>
                            o.ColumnName == match.Value);
                    if (column != null)
                    {
                        columns.Add(column);
                    }
                }
            }
            return columns;
        }

        public List<Link> GetUseSearchLinks(Context context)
        {
            return Links?
                .Where(o => GetColumn(context: context, columnName: o.ColumnName)?
                    .UseSearch == true)
                .ToList();
        }

        public void SetLinks(Context context, Column column)
        {
            column.Link = false;
            Links.RemoveAll(o => o.ColumnName == column.ColumnName);
            column.ChoicesText.SplitReturn()
                .Select(o => o.Trim())
                .Where(o => o.RegexExists(@"^\[\[.+\]\]$"))
                .Select(settings => new Link(
                    columnName: column.ColumnName,
                    settings: settings))
                .Where(link => link.SiteId != 0)
                .ForEach(link =>
                {
                    column.Link = true;
                    if (!Links.Any(o => o.ColumnName == column.ColumnName
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

        public void SetChoiceHash(EnumerableRowCollection<DataRow> dataRows)
        {
            var dataColumns = dataRows.Columns()
                .Where(columnName => columnName.EndsWith(",ItemTitle"))
                .ToList();
            Columns
                .Where(column => column.Linked())
                .ForEach(column =>
                {
                    if (column.ChoiceHash == null)
                    {
                        column.ChoiceHash = new Dictionary<string, Choice>();
                    }
                    var links = column.SiteSettings.Links
                        .Where(link => column.Name == link.ColumnName)
                        .Select(link => link.LinkedTableName() + ",ItemTitle")
                        .ToList();
                    if (dataColumns.Any(o => links.Any(p => o.EndsWith(p))))
                    {
                        dataRows
                            .GroupBy(o => o.Long(column.ColumnName))
                            .Select(o => o.First())
                            .Select(dataRow => new
                            {
                                Value = dataRow.String(column.ColumnName),
                                Text = dataColumns
                                    .Where(columnName =>
                                        links.Any(link =>
                                            columnName.EndsWith(link)))
                                    .Where(columnName => dataRow[columnName] != DBNull.Value)
                                    .Select(columnName => dataRow[columnName].ToString())
                                    .FirstOrDefault()
                            })
                            .Where(data => data.Text != null)
                            .ForEach(data =>
                                column.ChoiceHash.AddOrUpdate(
                                    data.Value,
                                    new Choice(
                                        value: data.Value,
                                        text: data.Text)));
                    }
                });
        }

        public string LinkedItemTitle(
            Context context, long referenceId, IEnumerable<long> siteIdList)
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
            Context context, IEnumerable<long> idList, IEnumerable<long> siteIdList = null)
        {
            return Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectItems(
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
                    linkHash: null);
            }
            else
            {
                var siteIdList = JoinedSsHash
                    ?.SelectMany(o => o.Value.Links.Select(p => p.SiteId))
                    .Distinct()
                    .ToList() ?? new List<long>();
                var linkHash = withLink
                    ? LinkHash(
                        context: context,
                        siteIdList: siteIdList,
                        all: all,
                        searchIndexes: null)
                    : null;
                JoinedSsHash.ForEach(data => data.Value.SetChoiceHash(
                    context: context,
                    columnName: null,
                    searchIndexes: null,
                    linkHash: linkHash));
            }
        }

        public void SetChoiceHash(
            Context context,
            string columnName,
            List<string> searchIndexes = null,
            IEnumerable<string> selectedValues = null,
            bool noLimit = false,
            bool setTotalCount = false,
            bool searchColumnOnly = true)
        {
            SetChoiceHash(
                context: context,
                columnName: columnName,
                searchIndexes: searchIndexes,
                linkHash: LinkHash(
                    context: context,
                    columnName: columnName,
                    searchIndexes: searchIndexes,
                    searchColumnOnly: searchColumnOnly,
                    selectedValues: selectedValues,
                    noLimit: noLimit,
                    setTotalCount: setTotalCount));
        }

        public void SetChoiceHash(
            Context context,
            string columnName,
            List<string> searchIndexes,
            Dictionary<string, List<string>> linkHash)
        {
            var columns = new List<Column>();
            Columns?
                .Where(o => o.HasChoices())
                .Where(o => columnName == null || o.ColumnName == columnName)
                .ForEach(column =>
                {
                    var same = columns.FirstOrDefault(o => o.ChoicesText == column.ChoicesText);
                    if (same == null)
                    {
                        columns.Add(column);
                    }
                    else
                    {
                        column.ChoiceHash = same.ChoiceHash;
                    }
                    column.SetChoiceHash(
                        context: context,
                        siteId: InheritPermission,
                        linkHash: linkHash,
                        searchIndexes: searchIndexes,
                        setAllChoices: SetAllChoices,
                        setChoices: same == null);
                });
        }

        private Dictionary<string, List<string>> LinkHash(
            Context context,
            List<long> siteIdList,
            bool all,
            List<string> searchIndexes)
        {
            var notUseSearchSiteIdList = Links
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
                                .CanRead(context: context, idColumnBracket: "\"Items\".\"ReferenceId\"")
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
            bool noLimit = false,
            string parentClass = "",
            IEnumerable<long> parentIds = null,
            bool setTotalCount = false)
        {
            var hash = new Dictionary<string, List<string>>();
            Links?
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
                    noLimit: noLimit,
                    referenceType: Destinations
                        ?.Values
                        .Where(d => d.SiteId == link.SiteId)
                        .Select(d => d.ReferenceType)
                        .FirstOrDefault(),
                    parentColumn: GetColumn(context: context, columnName: parentClass),
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
            bool noLimit,
            string referenceType,
            Column parentColumn,
            IEnumerable<long> parentIds,
            bool setTotalCount = false)
        {
            var select = Indexes.Select(
                context: context,
                ss: Destinations?.Get(link.SiteId),
                searchText: searchIndexes?.Join(" "),
                siteIdList: link.SiteId.ToSingleList());
            var join = new SqlJoinCollection(
                new SqlJoin(
                    tableBracket: "\"Sites\"",
                    joinType: SqlJoin.JoinTypes.Inner,
                    joinExpression: "\"Items\".\"SiteId\"=\"Sites\".\"SiteId\""));
            var where = Rds.ItemsWhere()
                .ReferenceId(
                    _operator: " in ",
                    sub: select,
                    subPrefix: false,
                    _using: select != null)
                .ReferenceId_In(
                    selectedValues,
                    _using: selectedValues?.Any() == true
                        && Repository.ExecuteScalar_string(
                            context: context,
                            statements: Rds.SelectSites(
                                column: Rds.SitesColumn().ReferenceType(),
                                where: Rds.SitesWhere().SiteId(link.SiteId))) != "Wikis")
                .ReferenceId_In(
                    sub: new SqlStatement(LinkHashRelatingColumnsSubQuery(
                        context: context,
                        referenceType: referenceType,
                        parentColumn: parentColumn,
                        parentIds: parentIds)),
                    _using: (referenceType == "Results"
                        || referenceType == "Issues")
                        && (parentIds?.Any() ?? false)
                        && parentColumn != null)
                .ReferenceType("Sites", _operator: "<>")
                .SiteId(link.SiteId)
                .CanRead(context: context, idColumnBracket: "\"Items\".\"ReferenceId\"");
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
                    offset: !noLimit
                        ? offset
                        : 0,
                    pageSize: !noLimit
                        ? Parameters.General.DropDownSearchPageSize
                        : 0)
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

        public void Remind(Context context, List<int> idList, bool test = false)
        {
            Reminders?
                .Where(o => idList.Contains(o.Id))
                .ForEach(reminder => reminder.Remind(context: context, ss: this, test: test));
        }

        public Export GetExport(Context context, int id = 0)
        {
            return Exports?.FirstOrDefault(o => o.Id == id)
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
            var canRead = context.CanRead(ss: this);
            switch (name)
            {
                case "Index": return true;
                case "Calendar": return canRead && EnableCalendar == true;
                case "Crosstab": return canRead && EnableCrosstab == true;
                case "Gantt": return canRead && EnableGantt == true;
                case "BurnDown": return canRead && EnableBurnDown == true;
                case "TimeSeries": return canRead && EnableTimeSeries == true;
                case "Kamban": return canRead && EnableKamban == true;
                case "ImageLib": return context.ContractSettings.Images()
       && canRead && EnableImageLib == true;
                default: return false;
            }
        }

        public Permissions.Types GetPermissionType(bool site = false)
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
            AllowedIntegratedSites = new List<long> { SiteId };
            var allows = Permissions.AllowSites(
                context: context,
                sites: IntegratedSites,
                referenceType: ReferenceType);
            AllowedIntegratedSites.AddRange(IntegratedSites.Where(o => allows.Contains(o)));
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
                .Select(o => o.ToLong())
                .Where(o => o != 0)
                .Distinct()
                .ToList();
        }

        private void SetPermissionForCreating(string value)
        {
            PermissionForCreating = Permissions.Get(value.Deserialize<List<string>>())
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
                var name = currentSs?.GetColumn(
                    context: context,
                    columnName: part.Split_1st('~'))?.Name;
                path.Add(part);
                var alias = path.Join("-");
                if (tableName != null && name != null)
                {
                    if (alias.Contains("~~"))
                    {
                        join.Add(new SqlJoin(
                            tableBracket: "\"" + tableName + "\"",
                            joinType: SqlJoin.JoinTypes.LeftOuter,
                            joinExpression: $"\"{leftAlias}\".\"{Rds.IdColumn(leftTableName)}\"=try_cast(\"{alias}\".\"{name}\" as bigint) and \"{alias}\".\"SiteId\"={siteId}",
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
                            joinExpression: $"{context.SqlCommandText.CreateTryCast(left.Any() ? left.Join("-") : ReferenceType, name, "bigint")}=\"{alias}_Items\".\"ReferenceId\" and \"{alias}_Items\".\"SiteId\"={siteId}",
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
                    .Where(style => peredicate(style))
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
                    .Where(script => peredicate(script))
                    .Select(o => o.Body).Join("\n")
                : null;
        }

        public string GridCss(Context context)
        {
            switch (TableType)
            {
                case Sqls.TableTypes.History:
                    return "grid history";
                case Sqls.TableTypes.Deleted:
                    return "grid deleted not-link";
                default:
                    return "grid" + (context.Forms.Bool("EditOnGrid")
                        ? " confirm-unload not-link"
                        : string.Empty);
            }
        }

        public bool InitialValue(Context context)
        {
            return RecordingJson(context: context) == "[]";
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

        private static string LinkHashRelatingColumnsSubQuery(Context context, string referenceType, Column parentColumn, IEnumerable<long> parentIds)
        {
            if (parentColumn == null
                || referenceType == "Wikis"
                || referenceType == "Sites"
                || parentIds == null)
            {
                return null;
            }
            var whereNullorEmpty = parentIds?.Contains(-1) == true
                ? $"\"{parentColumn.ColumnName}\" is null or \"{parentColumn.ColumnName}\" = '' " : string.Empty;
            var whereIn = parentIds.Where(n => n >= 0).Any()
                ? $"{context.SqlCommandText.CreateTryCast(referenceType, parentColumn.ColumnName, "bigint")} in ({parentIds.Where(n => n >= 0).Join()})"
                : string.Empty;
            var Or = (whereNullorEmpty == string.Empty || whereIn == string.Empty) ? string.Empty : " or ";
            return $"select \"{Rds.IdColumn(referenceType)}\" from \"{referenceType}\" where " + whereNullorEmpty + Or + whereIn;
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
                .Where(o => o.ColumnName == parentClassName)
                .Select(o => o.SiteId)
                .FirstOrDefault();
            var childSiteId = Links
                .Where(o => o.ColumnName == childClassName)
                .Select(o => o.SiteId)
                .FirstOrDefault();
            return Destinations
                .Values
                .Where(dest => dest.SiteId == childSiteId)
                .Select(dest => dest.Links
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

        public bool CheckRow(Context context)
        {
            return GridColumnsHasSources()
                ? false
                : context.CanUpdate(ss: this)
                    || context.CanDelete(ss: this)
                    || context.CanExport(ss: this);
        }

        public bool GridColumnsHasSources()
        {
            return GridColumns?.Any(o => o.Contains("~~")) == true;
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
    }
}