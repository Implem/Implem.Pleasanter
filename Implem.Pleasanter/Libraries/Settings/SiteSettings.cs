using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
namespace Implem.Pleasanter.Libraries.Settings
{
    [Serializable()]
    public class SiteSettings
    {
        public enum SearchTypes : int
        {
            FullText = 10,
            PartialMatch = 15,
            MatchInFrontOfTitle = 20,
            BroadMatchOfTitle = 30,
        }

        public decimal Version;
        [NonSerialized]
        public List<SiteSettings> Destinations;
        [NonSerialized]
        public List<SiteSettings> Sources;
        [NonSerialized]
        public bool Migrated;
        [NonSerialized]
        public long SiteId;
        [NonSerialized]
        public long ReferenceId;
        [NonSerialized]
        public string Title;
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
        public Dictionary<long, SiteSettings> JoinedSsHash;
        [NonSerialized]
        public Dictionary<string, string> JoinOptionHash;
        [NonSerialized]
        public bool Linked;
        [NonSerialized]
        public string DuplicatedColumn;
        public string ReferenceType;
        public decimal? NearCompletionTimeAfterDays;
        public decimal? NearCompletionTimeBeforeDays;
        public int? GridPageSize;
        public int? GridView;
        public bool? EditInDialog;
        public int? FirstDayOfWeek;
        public int? FirstMonth;
        public List<string> GridColumns;
        public List<string> FilterColumns;
        public List<string> EditorColumns;
        public List<string> TitleColumns;
        public List<string> LinkColumns;
        public List<string> HistoryColumns;
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
        public bool? AllowEditingComments;
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
        public string TitleSeparator;
        public SearchTypes? SearchType;
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
            Version = Parameters.Asset.SiteSettingsVersion;
            NearCompletionTimeBeforeDays = NearCompletionTimeBeforeDays ??
                Parameters.General.NearCompletionTimeBeforeDays;
            NearCompletionTimeAfterDays = NearCompletionTimeAfterDays ??
                Parameters.General.NearCompletionTimeAfterDays;
            GridPageSize = GridPageSize ?? Parameters.General.GridPageSize;
            FirstDayOfWeek = FirstDayOfWeek ?? Parameters.General.FirstDayOfWeek;
            FirstMonth = FirstMonth ?? Parameters.General.FirstMonth;
            UpdateColumnDefinitionHash();
            UpdateGridColumns(context: context);
            UpdateFilterColumns();
            UpdateEditorColumns(context: context);
            UpdateTitleColumns();
            UpdateLinkColumns(context: context);
            UpdateHistoryColumns(context: context);
            UpdateColumns(context: context);
            UpdateColumnHash();
            var accessControlColumns = Columns
                .Where(o => o.EditorColumn || o.ColumnName == "Comments")
                .Where(o => !o.NotEditorSettings)
                .Where(o => !o.Id_Ver)
                .ToList();
            Update_CreateColumnAccessControls(accessControlColumns);
            Update_ReadColumnAccessControls(accessControlColumns);
            Update_UpdateColumnAccessControls(accessControlColumns);
            if (Aggregations == null) Aggregations = new List<Aggregation>();
            if (Links == null) Links = new List<Link>();
            if (Summaries == null) Summaries = new SettingList<Summary>();
            if (Formulas == null) Formulas = new SettingList<FormulaSet>();
            ViewLatestId = ViewLatestId ?? 0;
            if (Notifications == null) Notifications = new SettingList<Notification>();
            if (Reminders == null) Reminders = new SettingList<Reminder>();
            if (Exports == null) Exports = new SettingList<Export>();
            if (Scripts == null) Scripts = new SettingList<Script>();
            if (RelatingColumns == null) RelatingColumns = new SettingList<RelatingColumn>();
            if (Styles == null) Styles = new SettingList<Style>();
            AllowEditingComments = AllowEditingComments ?? false;
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
        }

        public void SetLinkedSiteSettings(Context context)
        {
            var dataSet = Rds.ExecuteDataSet(
                context: context,
                statements: new SqlStatement[]
                {
                    Rds.SelectSites(
                        dataTableName: "Destinations",
                        column: Rds.SitesColumn()
                            .SiteId()
                            .Title()
                            .ReferenceType()
                            .ParentId()
                            .InheritPermission()
                            .SiteSettings(),
                        where: Rds.SitesWhere()
                            .SiteId_In(sub: Rds.SelectLinks(
                                column: Rds.LinksColumn().DestinationId(),
                                where: Rds.LinksWhere().SourceId(SiteId)))
                            .ReferenceType("Wikis", _operator: "<>")),
                    Rds.SelectSites(
                        dataTableName: "Sources",
                        column: Rds.SitesColumn()
                            .SiteId()
                            .Title()
                            .ReferenceType()
                            .ParentId()
                            .InheritPermission()
                            .SiteSettings(),
                        where: Rds.SitesWhere()
                            .SiteId_In(sub: Rds.SelectLinks(
                                column: Rds.LinksColumn().SourceId(),
                                where: Rds.LinksWhere().DestinationId(SiteId)))
                            .ReferenceType("Wikis", _operator: "<>"))
                });
            Destinations = SiteSettingsList(
                context: context, dataTable: dataSet.Tables["Destinations"]);
            Sources = SiteSettingsList(
                context: context, dataTable: dataSet.Tables["Sources"]);
            SetRelatingColumnsLinkedClass();
        }

        public void SetPermissions(Context context, long referenceId)
        {
            var targets = new List<long> { InheritPermission, referenceId };
            targets.AddRange(Destinations?.Select(o => o.InheritPermission) ?? new List<long>());
            targets.AddRange(Sources?.Select(o => o.InheritPermission) ?? new List<long>());
            var permissions = Permissions.Get(context: context, targets: targets.Distinct());
            SetPermissions(
                context: context,
                ss: this,
                permissions: permissions,
                referenceId: referenceId);
            Destinations?.ForEach(ss =>
                SetPermissions(
                    context: context,
                    ss: ss,
                    permissions: permissions));
            Sources?.ForEach(ss => SetPermissions(
                context: context,
                ss: ss,
                permissions: permissions));
        }

        private void SetPermissions(
            Context context,
            SiteSettings ss,
            Dictionary<long, Permissions.Types> permissions,
            long referenceId = 0)
        {
            if (context.Publish)
            {
                ss.PermissionType = Permissions.Types.Read;
                ss.ItemPermissionType = Permissions.Types.Read;
            }
            else if (context.Controller != "publishes")
            {
                if (permissions.ContainsKey(ss.InheritPermission))
                {
                    ss.PermissionType = permissions[ss.InheritPermission];
                }
                if (referenceId != 0 && permissions.ContainsKey(referenceId))
                {
                    ss.ItemPermissionType = permissions[referenceId];
                }
            }
        }

        private List<SiteSettings> SiteSettingsList(Context context, DataTable dataTable)
        {
            var ssList = new List<SiteSettings>();
            dataTable.AsEnumerable().ForEach(dataRow =>
            {
                var ss = SiteSettingsUtilities.Get(context: context, dataRow: dataRow);
                ss.SiteId = dataRow.Long("SiteId");
                ss.Title = dataRow.String("Title");
                ss.ReferenceType = dataRow.String("ReferenceType");
                ss.ParentId = dataRow.Long("ParentId");
                ss.InheritPermission = dataRow.Long("InheritPermission");
                ss.Linked = true;
                ssList.Add(ss);
            });
            return ssList;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext streamingContext)
        {
            if (Version != Parameters.Asset.SiteSettingsVersion)
            {
                Migrators.SiteSettingsMigrator.Migrate(this);
            }
            Init(context: new Context(item: false));
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext streamingContext)
        {
        }

        public bool IsSite(Context context)
        {
            return SiteId == context.Id;
        }

        public bool IsSiteEditor(Context context)
        {
            return IsSite(context: context) && context.Action == "edit";
        }

        public string RecordingJson(Context context)
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
            if (EditInDialog == true)
            {
                ss.EditInDialog = EditInDialog;
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
            if (!EditorColumns.SequenceEqual(DefaultEditorColumns(context: context)))
            {
                ss.EditorColumns = EditorColumns;
            }
            if (!TitleColumns.SequenceEqual(DefaultTitleColumns()))
            {
                ss.TitleColumns = TitleColumns;
            }
            if (AllowEditingComments == true)
            {
                ss.AllowEditingComments = AllowEditingComments;
            }
            if (SwitchRecordWithAjax==true)
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
            if (ImageLibPageSize != Parameters.General.ImageLibPageSize)
            {
                ss.ImageLibPageSize = ImageLibPageSize;
            }
            if (TitleSeparator != ")")
            {
                ss.TitleSeparator = TitleSeparator;
            }
            if (SearchType != SearchTypes.FullText)
            {
                ss.SearchType = SearchType;
            }
            if (!LinkColumns.SequenceEqual(DefaultLinkColumns(context: context)))
            {
                ss.LinkColumns = LinkColumns;
            }
            if (!HistoryColumns.SequenceEqual(DefaultHistoryColumns(context: context)))
            {
                ss.HistoryColumns = HistoryColumns;
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
                ss.Notifications.Add(notification.GetRecordingData());
            });
            Reminders?.ForEach(notification =>
            {
                if (ss.Reminders == null)
                {
                    ss.Reminders = new SettingList<Reminder>();
                }
                ss.Reminders.Add(notification.GetRecordingData(context: context));
            });
            Exports?.ForEach(exportSetting =>
            {
                if (ss.Exports == null)
                {
                    ss.Exports = new SettingList<Export>();
                }
                ss.Exports.Add(exportSetting.GetRecordingData());
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
            Styles?.ForEach(style =>
            {
                if (ss.Styles == null)
                {
                    ss.Styles = new SettingList<Style>();
                }
                ss.Styles.Add(style.GetRecordingData());
            });
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
            Columns?.ForEach(column =>
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
                    if (column.DecimalPlaces != columnDefinition.DecimalPlaces)
                    {
                        enabled = true;
                        newColumn.DecimalPlaces = column.DecimalPlaces;
                    }
                    if (column.Min != columnDefinition.Min)
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
                    if (column.FieldCss != columnDefinition.FieldCss)
                    {
                        enabled = true;
                        newColumn.FieldCss = column.FieldCss;
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
            return ss.ToJson();
        }

        private void UpdateColumnDefinitionHash()
        {
            ColumnDefinitionHash = GetColumnDefinitionHash(ReferenceType);
        }

        private static Dictionary<string, ColumnDefinition> GetColumnDefinitionHash(
            string referenceType)
        {
            var excludeColumns = Parameters.ExcludeColumns.Get(referenceType);
            return Def.ColumnDefinitionCollection
                .Where(o => o.TableName == referenceType)
                .Where(o => excludeColumns?.Contains(o.ColumnName) != true)
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

        private void UpdateFilterColumns()
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
            if (EditorColumns == null)
            {
                EditorColumns = DefaultEditorColumns(context: context);
            }
            else
            {
                EditorColumns.RemoveAll(o => !EditorColumn(ColumnDefinitionHash.Get(o)));
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

        private void UpdateTitleColumns()
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
                column.DecimalPlaces = column.DecimalPlaces ?? columnDefinition.DecimalPlaces;
                column.Min = column.Min ?? columnDefinition.Min;
                column.Max = column.Max ?? DefaultMax(columnDefinition);
                column.Step = column.Step ?? DefaultStep(columnDefinition);
                column.NoDuplication = column.NoDuplication ?? false;
                column.CopyByDefault = column.CopyByDefault ?? false;
                column.EditorReadOnly = column.EditorReadOnly ?? columnDefinition.EditorReadOnly;
                column.AllowImage = column.AllowImage ?? true;
                column.FieldCss = column.FieldCss ?? columnDefinition.FieldCss;
                column.Unit = column.Unit ?? columnDefinition.Unit;
                column.CheckFilterControlType = column.CheckFilterControlType ?? ColumnUtilities.CheckFilterControlTypes.OnOnly;
                column.NumFilterMin = column.NumFilterMin ?? columnDefinition.NumFilterMin;
                column.NumFilterMax = column.NumFilterMax ?? columnDefinition.NumFilterMax;
                column.NumFilterStep = column.NumFilterStep ?? columnDefinition.NumFilterStep;
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
                column.UserColumn = columnDefinition.UserColumn;
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

        private string LanguagesLabelText(Context context, string labelText)
        {
            var hash = labelText?.Deserialize<Dictionary<string, string>>();
            if (hash != null)
            {
                return hash.Get(context.Language) ?? hash.Get("");
            }
            else
            {
                return labelText;
            }
        }

        private void UpdateColumnHash()
        {
            ColumnHash = Columns
                .GroupBy(o => o.ColumnName)
                .Select(o => o.First())
                .ToDictionary(o => o.ColumnName, o => o);
        }

        public void SetExports(Context context)
        {
            SetJoinedSsHash(context: context);
            Exports?.ForEach(export => SetExport(context: context, export: export));
        }

        public void SetExport(Context context, Export export)
        {
            SetJoinedSsHash(context: context);
            export.Header = export.Header ?? true;
            export.Columns
                .Where(o => JoinedSsHash.Get(o.SiteId) != null)
                .ForEach(o => o.Init(
                    context: context, 
                    ss: JoinedSsHash.Get(o.SiteId)));
        }

        public void SetJoinedSsHash(Context context)
        {
            if (JoinedSsHash == null)
            {
                JoinedSsHash = new Dictionary<long, SiteSettings>()
                {
                    { SiteId, this }
                }.AddRange(GetJoinedSsHash(
                    context: context, links: Links, hash: new Dictionary<long, SiteSettings>()));
                JoinedSsHash.Values.ForEach(ss =>
                    Columns
                        .Where(o => o.ColumnName.Contains(","))
                        .Where(o => new ColumnNameInfo(o).SiteId == ss.SiteId)
                        .ForEach(o => UpdateColumn(
                            context: context,
                            ss: ss,
                            columnDefinition: ss.ColumnDefinitionHash.Get(new ColumnNameInfo(o).Name),
                            column: o)));
                JoinOptionHash = GetJoinOptionHash(context: context);
            }
        }

        private Dictionary<string, string> GetJoinOptionHash(Context context)
        {
            return TableJoins(
                context: context,
                links: Links,
                join: new Join(Title),
                joins: new List<Join> { new Join(Title) })
                    .ToDictionary(
                        o => o.Select(p => $"{p.ColumnName}~{p.SiteId}").Join("-"),
                        o => o.GetTitle(reverce: true, bracket: true));
        }

        private Dictionary<long, SiteSettings> GetJoinedSsHash(
            Context context, List<Link> links, Dictionary<long, SiteSettings> hash)
        {
            links?
                .Select(link => link.SiteId)
                .Distinct()
                .Where(siteId => siteId != SiteId)
                .Where(siteId => !hash.ContainsKey(siteId))
                .Where(siteId => Permissions.Can(
                    context: context,
                    siteId: Permissions.InheritPermission(
                        context: context,
                        id: siteId),
                    type: Permissions.Types.Read))
                .ToList()
                .Distinct()
                .ForEach(siteId =>
                {
                    var ss = SiteSettingsUtilities.GetByDataRow(
                        context: context,
                        siteId: siteId);
                    if (ss != null && !hash.ContainsKey(siteId) && ss.ReferenceType != "Wikis")
                    {
                        hash.Add(siteId, ss);
                        GetJoinedSsHash(context: context, links: ss.Links, hash: hash);
                    }
                });
            return hash;
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
            CreateColumnAccessControls.ForEach(o =>
            {
                var column = GetColumn(context: context, columnName: o.ColumnName);
                if (column != null)
                {
                    column.CanCreate =
                        o.Allowed(
                            context: context,
                            ss: this,
                            type: PermissionType,
                            mine: mine) &&
                        column.EditorReadOnly != true;
                }
            });
            ReadColumnAccessControls.ForEach(o =>
            {
                var column = GetColumn(context: context, columnName: o.ColumnName);
                if (column != null)
                {
                    column.CanRead = o.Allowed(
                        context: context,
                        ss: this,
                        type: PermissionType,
                        mine: mine);
                }
            });
            UpdateColumnAccessControls.ForEach(o =>
            {
                var column = GetColumn(context: context, columnName: o.ColumnName);
                if (column != null)
                {
                    column.CanUpdate =
                        o.Allowed(
                            context: context,
                            ss: this,
                            type: PermissionType,
                            mine: mine) &&
                        column.EditorReadOnly != true;
                }
            });
        }

        private decimal DefaultMax(ColumnDefinition columnDefinition)
        {
            return (columnDefinition.Max > 0
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
                JoinOptionHash?.ContainsKey(columnName.Split_1st()) == true)
            {
                column = AddJoinedColumn(context: context, columnName: columnName);
            }
            return column;
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
                    column.SetChoiceHash(context: context, siteId: ss.InheritPermission);
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

        public IEnumerable<Column> GetGridColumns(
            Context context, View view = null, bool checkPermission = false)
        {
            return (view?.GridColumns ?? GridColumns)
                .Select(columnName => GetColumn(context: context, columnName: columnName))
                .Where(column => column != null)
                .AllowedColumns(checkPermission: checkPermission)
                .Where(o => context.ContractSettings.Attachments()
                    || o.ControlType != "Attachments")
                .ToList();
        }

        public IEnumerable<Column> GetFilterColumns(Context context, bool checkPermission = false)
        {
            return FilterColumns
                .Select(columnName => GetColumn(context: context, columnName: columnName))
                .Where(column => column != null)
                .AllowedColumns(checkPermission: checkPermission)
                .ToList();
        }

        public IEnumerable<Column> GetEditorColumns(Context context)
        {
            return EditorColumns
                .Select(columnName => GetColumn(context: context, columnName: columnName))
                .Where(column => column != null)
                .Where(o => context.ContractSettings.Attachments()
                    || o.ControlType != "Attachments")
                .ToList();
        }

        public IEnumerable<Column> GetTitleColumns(Context context)
        {
            return TitleColumns?
                .Select(columnName => GetColumn(context: context, columnName: columnName))
                .Where(column => column != null)
                .ToList();
        }

        public IEnumerable<Column> GetLinkColumns(Context context, bool checkPermission = false)
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
            return Columns?.Where(o =>
                o.Required || EditorColumns?.Contains(o.ColumnName) == true);
        }

        public Dictionary<string, ControlData> GridSelectableOptions(
            Context context, bool enabled = true, string join = null)
        {
            SetJoinedSsHash(context: context);
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
            SetJoinedSsHash(context: context);
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
            SetJoinedSsHash(context: context);
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
            JoinOptionHash?.ForEach(join =>
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
            JoinOptionHash?.ForEach(join =>
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
                    columns: EditorColumns,
                    order: ColumnDefinitionHash.EditorDefinitions(context: context)
                        .OrderBy(o => o.EditorColumn)
                        .Select(o => o.ColumnName).ToList())
                : ColumnUtilities.SelectableOptions(
                    context: context,
                    ss: this,
                    columns: ColumnDefinitionHash.EditorDefinitions(context: context)
                        .Where(o => !EditorColumns.Contains(o.ColumnName))
                        .OrderBy(o => o.EditorColumn)
                        .Select(o => o.ColumnName),
                    order: ColumnDefinitionHash?.EditorDefinitions(context: context)?
                        .OrderBy(o => o.EditorColumn)
                        .Select(o => o.ColumnName).ToList());
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
                        var column = ss.GetColumn(context: context, columnName: link.ColumnName);
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
                    .Split_2nd('~')
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
            SetJoinedSsHash(context: context);
            return CrosstabGroupByOptions(
                context: context,
                datetime: true);
        }

        public Dictionary<string, string> CrosstabGroupByYOptions(Context context)
        {
            SetJoinedSsHash(context: context);
            var hash = CrosstabGroupByOptions(context: context);
            hash.Add("Columns", Displays.NumericColumn(context: context));
            return hash;
        }

        private Dictionary<string, string> CrosstabGroupByOptions(
            Context context, bool datetime = false)
        {
            var hash = new Dictionary<string, string>();
            JoinOptionHash?.ForEach(join =>
            {
                var siteId = ColumnUtilities.GetSiteIdByTableAlias(join.Key, SiteId);
                var ss = JoinedSsHash.Get(siteId);
                if (ss != null)
                {
                    hash.AddRange(ss.Columns
                        .Where(o => o.HasChoices() || (o.TypeName == "datetime" && datetime))
                        .Where(o => !o.Joined)
                        .Where(o => ss.EditorColumns.Contains(o.Name))
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
            JoinOptionHash?.ForEach(join =>
            {
                var siteId = ColumnUtilities.GetSiteIdByTableAlias(join.Key, SiteId);
                var ss = JoinedSsHash.Get(siteId);
                if (ss != null)
                {
                    hash.AddRange(ss.Columns
                        .Where(o => o.Computable)
                        .Where(o => o.TypeName != "datetime")
                        .Where(o => !o.Joined)
                        .Where(o => ss.EditorColumns.Contains(o.Name))
                        .Where(o => o.CanRead)
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
            return EditorColumns
                .Select(columnName => GetColumn(context: context, columnName: columnName))
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

        public List<Column> ReadableColumns(bool noJoined = false)
        {
            return Columns
                .Where(o =>
                    EditorColumns.Contains(o.ColumnName) ||
                    GridColumns.Contains(o.ColumnName))
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
                EditorColumns?.Contains("Comments") == true &&
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
                case "EditInDialog": EditInDialog = value.ToBool(); break;
                case "FirstDayOfWeek": FirstDayOfWeek = value.ToInt(); break;
                case "FirstMonth": FirstMonth = value.ToInt(); break;
                case "AllowEditingComments": AllowEditingComments = value.ToBool(); break;
                case "SwitchRecordWithAjax":SwitchRecordWithAjax = value.ToBool(); break;
                case "EnableCalendar": EnableCalendar = value.ToBool(); break;
                case "EnableCrosstab": EnableCrosstab = value.ToBool(); break;
                case "EnableGantt": EnableGantt = value.ToBool(); break;
                case "ShowGanttProgressRate": ShowGanttProgressRate = value.ToBool(); break;
                case "EnableBurnDown": EnableBurnDown = value.ToBool(); break;
                case "EnableTimeSeries": EnableTimeSeries = value.ToBool(); break;
                case "EnableKamban": EnableKamban = value.ToBool(); break;
                case "EnableImageLib": EnableImageLib = value.ToBool(); break;
                case "ImageLibPageSize": ImageLibPageSize = value.ToInt(); break;
                case "SearchType": SearchType = (SearchTypes)value.ToInt(); break;
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
                case "EditorColumnsAll": EditorColumns = context.Forms.List(propertyName); break;
                case "TitleColumnsAll": TitleColumns = context.Forms.List(propertyName); break;
                case "LinkColumnsAll": LinkColumns = context.Forms.List(propertyName); break;
                case "HistoryColumnsAll": HistoryColumns = context.Forms.List(propertyName); break;
                case "ViewsAll":
                    Views = Views?.Join(context.Forms.List(propertyName).Select((val, key) => new { Key = key, Val = val }), v => v.Id, l => l.Val.ToInt(),
                        (v, l) => new { Views = v, OrderNo = l.Key })
                        .OrderBy(v => v.OrderNo)
                        .Select(v => v.Views)
                        .ToList();
                    break;
            }
        }

        public void SetAggregations(
            string controlId,
            IEnumerable<string> selectedColumns,
            IEnumerable<string> selectedSourceColumns)
        {
            switch (controlId)
            {
                case "AddAggregations":
                    var idCollection = new List<string>();
                    selectedSourceColumns.ForEach(groupBy =>
                    {
                        var id = Aggregations.Count > 0
                            ? Aggregations.Max(o => o.Id) + 1
                            : 1;
                        idCollection.Add(id.ToString());
                        Aggregations.Add(new Aggregation(id, groupBy));
                    });
                    selectedColumns = idCollection;
                    selectedSourceColumns = null;
                    break;
                case "DeleteAggregations":
                    Aggregations
                        .RemoveAll(o => selectedColumns.Contains(o.Id.ToString()));
                    selectedSourceColumns = selectedColumns;
                    selectedColumns = null;
                    break;
                case "MoveUpAggregations":
                case "MoveDownAggregations":
                    var order = Aggregations.Select(o => o.Id.ToString()).ToArray();
                    if (controlId == "MoveDownAggregations") Array.Reverse(order);
                    order.Select((o, i) => new { Id = o, Index = i }).ForEach(data =>
                    {
                        if (selectedColumns.Contains(data.Id) &&
                            data.Index > 0 &&
                            !selectedColumns.Contains(order[data.Index - 1]))
                        {
                            order = Arrays.Swap(order, data.Index, data.Index - 1);
                        }
                    });
                    if (controlId == "MoveDownAggregations") Array.Reverse(order);
                    Aggregations = order.ToList().Select(id => Aggregations
                        .FirstOrDefault(o => o.Id.ToString() == id)).ToList();
                    break;
            }
        }

        public void SetAggregationDetails(
            Aggregation.Types type,
            string target,
            IEnumerable<string> selectedColumns,
            IEnumerable<string> selectedSourceColumns)
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
                case "DecimalPlaces": column.DecimalPlaces = value.ToInt(); break;
                case "Max": column.Max = value.ToDecimal(); break;
                case "Min": column.Min = value.ToDecimal(); break;
                case "Step": column.Step = value.ToDecimal(); break;
                case "NoDuplication": column.NoDuplication = value.ToBool(); break;
                case "CopyByDefault": column.CopyByDefault = value.ToBool(); break;
                case "EditorReadOnly": column.EditorReadOnly = value.ToBool(); break;
                case "AllowImage": column.AllowImage = value.ToBool(); break;
                case "FieldCss": column.FieldCss = value; break;
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

        public IEnumerable<string> IncludedColumns()
        {
            return IncludedColumns(Columns
                .Where(o => !o.GridDesign.IsNullOrEmpty())
                .Select(o => o.GridDesign)
                .Join(string.Empty))
                    .Select(o => o.ColumnName);
        }

        public IEnumerable<Column> IncludedColumns(string value, bool labelText = false)
        {
            foreach (Match match in value.RegexMatches(@"(?<=\[).+?(?=\])"))
            {
                var column = labelText
                    ? Columns.FirstOrDefault(o =>
                        o.LabelText == match.Value)
                    : Columns.FirstOrDefault(o =>
                        o.ColumnName == match.Value);
                if (column != null) yield return column;
            }
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
            var columns = dataRows.Columns();
            Columns
                .Where(o => !Aggregations.Any(p => p.GroupBy == o.ColumnName))
                .Where(o => columns.Contains("Linked__" + o.ColumnName))
                .Where(o => o.Linked())
                .Where(o => o.ChoiceHash.Any() == false)
                .ForEach(column =>
                {
                    column.ChoiceHash = new Dictionary<string, Choice>();
                    dataRows
                        .GroupBy(o => o.Long(column.ColumnName))
                        .Select(o => o.First())
                        .ForEach(dataRow =>
                            column.ChoiceHash.Add(
                                dataRow.String(column.ColumnName),
                                new Choice(
                                    dataRow.String(column.ColumnName),
                                    dataRow.String("Linked__" + column.ColumnName))));
                    column.LinkedChoiceHashCreated = true;
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
            return Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectItems(
                    column: Rds.ItemsColumn()
                        .ReferenceId()
                        .Title(),
                    join: new SqlJoinCollection(
                        new SqlJoin(
                            tableBracket: "[Sites]",
                            joinType: SqlJoin.JoinTypes.Inner,
                            joinExpression: "[Items].[SiteId]=[Sites].[SiteId]")),
                    where: Rds.ItemsWhere()
                        .ReferenceId_In(idList)
                        .SiteId_In(siteIdList, _using: siteIdList != null)
                        .CanRead(context: context, idColumnBracket: "[Items].[ReferenceId]")))
                            .AsEnumerable();
        }

        public void SetChoiceHash(Context context, bool withLink = true, bool all = false)
        {
            var siteIdList = LinkedSiteIdList();
            var searchSiteIdList = SearchSiteIdList(
                context: context,
                siteIdList: siteIdList);
            var linkHash = withLink
                ? LinkHash(
                    context: context,
                    siteIdList: siteIdList,
                    searchSiteIdList: searchSiteIdList,
                    all: all)
                : null;
            SetUseSearch(context: context, searchSiteIdList: searchSiteIdList);
            SetChoiceHash(
                context: context,
                columnName: null,
                searchText: null,
                linkHash: linkHash);
            Destinations?.ForEach(ss => ss.SetChoiceHash(
                context: context,
                columnName: null,
                searchText: null,
                linkHash: linkHash));
            Sources?.ForEach(ss => ss.SetChoiceHash(
                context: context,
                columnName: null,
                searchText: null,
                linkHash: linkHash));
        }

        public void SetChoiceHash(
            Context context,
            string columnName,
            string searchText = null,
            IEnumerable<string> selectedValues = null,
            bool noLimit = false)
        {
            SetChoiceHash(
                context: context,
                columnName: columnName,
                searchText: searchText,
                linkHash: LinkHash(
                    context: context,
                    columnName: columnName,
                    searchText: searchText,
                    selectedValues: selectedValues,
                    noLimit: noLimit));
        }

        public void SetChoiceHash(
            Context context,
            string columnName,
            string searchText,
            Dictionary<string, List<string>> linkHash)
        {
            Columns?
                .Where(o => o.HasChoices())
                .Where(o => columnName == null || o.ColumnName == columnName)
                .ForEach(column =>
                    column.SetChoiceHash(
                        context: context,
                        siteId: InheritPermission,
                        linkHash: linkHash,
                        searchIndexes: searchText
                            .SearchIndexes(context: context)));
        }

        private Dictionary<string, List<string>> LinkHash(
            Context context,
            List<long> siteIdList,
            List<long> searchSiteIdList,
            bool all)
        {
            var dataRows = Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectItems(
                    column: Rds.ItemsColumn()
                        .ReferenceId()
                        .ReferenceType()
                        .SiteId()
                        .Title(),
                    join: new SqlJoinCollection(
                        new SqlJoin(
                            tableBracket: "[Sites]",
                            joinType: SqlJoin.JoinTypes.Inner,
                            joinExpression: "[Items].[SiteId]=[Sites].[SiteId]")),
                    where: Rds.ItemsWhere()
                        .ReferenceType("Sites", _operator: "<>")
                        .SiteId_In(siteIdList)
                        .CanRead(context: context, idColumnBracket: "[Items].[ReferenceId]")
                        .Or(
                            or: Rds.ItemsWhere()
                                .ReferenceType("Wikis")
                                .SiteId_In(
                                    LinkHashTargetSiteIdList(
                                        siteIdList: siteIdList,
                                        searchSiteIdList: searchSiteIdList,
                                        all: all)),
                            _using: searchSiteIdList.Any()),
                    orderBy: Rds.ItemsOrderBy()
                        .Title())).AsEnumerable();
            return dataRows
                .Select(dataRow => dataRow.Long("SiteId"))
                .Distinct()
                .ToDictionary(
                    siteId => $"[[{siteId}]]",
                    siteId => LinkValue(
                        context: context,
                        siteId: siteId,
                        dataRows: dataRows.Where(dataRow => dataRow.Long("SiteId") == siteId)));
        }

        private List<long> LinkHashTargetSiteIdList(
            List<long> siteIdList,
            List<long> searchSiteIdList,
            bool all)
        {
            return siteIdList
                .Where(siteId =>
                    all ||
                    !searchSiteIdList.Contains(siteId) ||
                    Links.Any(p =>
                        p.SiteId == siteId &&
                        Aggregations.Any(q => q.GroupBy == p.ColumnName)))
                .ToList();
        }

        private List<long> LinkedSiteIdList()
        {
            var siteIdList = Links.Select(o => o.SiteId).ToList();
            siteIdList.AddRange(Destinations?
                .SelectMany(o => o.Links.Select(p => p.SiteId)) ?? new List<long>());
            siteIdList.AddRange(Sources?
                .SelectMany(o => o.Links.Select(p => p.SiteId)) ?? new List<long>());
            return siteIdList.Distinct().ToList();
        }

        private void SetUseSearch(Context context, List<long> searchSiteIdList)
        {
            searchSiteIdList
                .ForEach(siteId =>
                    Links
                        .Where(o => o.SiteId == siteId)
                        .Select(o => GetColumn(context: context, columnName: o.ColumnName))
                        .ForEach(column =>
                            column.UseSearch = true));
        }

        private static List<long> SearchSiteIdList(Context context, List<long> siteIdList)
        {
            return Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectItems(
                    column: Rds.ItemsColumn().SiteId(),
                    join: new SqlJoinCollection(
                        new SqlJoin(
                            tableBracket: "[Sites]",
                            joinType: SqlJoin.JoinTypes.Inner,
                            joinExpression: "[Items].[SiteId]=[Sites].[SiteId]")),
                    where: Rds.ItemsWhere().SiteId_In(siteIdList),
                    groupBy: Rds.ItemsGroupBy().SiteId(),
                    having: Rds.ItemsHaving().ItemsCount(
                        Parameters.General.DropDownSearchPageSize, _operator: ">")))
                            .AsEnumerable()
                            .Select(dataRow => dataRow.Long("SiteId"))
                            .ToList();
        }

        public Dictionary<string, List<string>> LinkHash(
            Context context,
            string columnName,
            string searchText = null,
            bool searchColumnOnly = true,
            IEnumerable<string> selectedValues = null,
            int offset = 0,
            bool noLimit = false,
            string parentClass = "",
            int parentId = 0)
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
                    searchText: searchText,
                    selectedValues: selectedValues?.Select(o => o.ToLong()),
                    link: link,
                    hash: hash,
                    offset: offset,
                    noLimit: noLimit,
                    referenceType: Destinations?
                        .Where(d => d.SiteId == link.SiteId)
                        .Select(d => d.ReferenceType)
                        .FirstOrDefault(),
                    parentColumn: GetColumn(context: context, columnName: parentClass),
                    parentId: parentId));
            return hash;
        }

        private void LinkHash(
            Context context,
            string searchText,
            IEnumerable<long> selectedValues,
            Link link,
            Dictionary<string, List<string>> hash,
            int offset,
            bool noLimit,
            string referenceType,
            Column parentColumn,
            int parentId)
        {
            var select = SearchIndexUtilities.Select(
                context: context,
                ss: Destinations?.Get(link.SiteId),
                searchText: searchText,
                siteIdList: link.SiteId.ToSingleList());
            var dataSet = Rds.ExecuteDataSet(
                context: context,
                statements: Rds.SelectItems(
                    dataTableName: "Main",
                    column: Rds.ItemsColumn()
                        .ReferenceId()
                        .ReferenceType()
                        .SiteId()
                        .Title(),
                    join: new SqlJoinCollection(
                        new SqlJoin(
                            tableBracket: "[Sites]",
                            joinType: SqlJoin.JoinTypes.Inner,
                            joinExpression: "[Items].[SiteId]=[Sites].[SiteId]")),
                    where: Rds.ItemsWhere()
                        .ReferenceId(
                            _operator: " in ",
                            sub: select,
                            subPrefix: false,
                            _using: select != null)
                        .ReferenceId_In(
                            selectedValues,
                            _using: selectedValues?.Any() == true)
                        .ReferenceId_In(
                            sub: new SqlStatement(LinkHashRelatingColumnsSubQuery(
                                referenceType: referenceType,
                                parentColumn: parentColumn,
                                parentId: parentId)),
                            _using: (referenceType == "Results"
                                || referenceType == "Issues")
                                && parentId != 0
                                && parentColumn != null)
                        .ReferenceType("Sites", _operator: "<>")
                        .SiteId(link.SiteId)
                        .CanRead(context: context, idColumnBracket: "[Items].[ReferenceId]"),
                    orderBy: Rds.ItemsOrderBy().ReferenceId(),
                    offset: !noLimit
                        ? offset
                        : 0,
                    pageSize: !noLimit
                        ? Parameters.General.DropDownSearchPageSize
                        : 0,
                    countRecord: true));
            var dataRows = dataSet.Tables["Main"].AsEnumerable();
            GetColumn(context: context, columnName: link.ColumnName)
                .TotalCount = Rds.Count(dataSet);
            if (dataRows.Any())
            {
                hash.Add($"[[{link.SiteId}]]", LinkValue(
                    context: context,
                    siteId: link.SiteId,
                    dataRows: dataRows));
            }
        }

        private static List<string> LinkValue(
            Context context, long siteId, EnumerableRowCollection<DataRow> dataRows)
        {
            return dataRows.Any(dataRow =>
                dataRow.Long("SiteId") == siteId &&
                dataRow.String("ReferenceType") == "Wikis")
                    ? Rds.ExecuteScalar_string(
                        context: context,
                        statements: Rds.SelectWikis(
                            column: Rds.WikisColumn().Body(),
                            where: Rds.WikisWhere().SiteId(siteId)))
                                .SplitReturn()
                                .Where(o => o.Trim() != string.Empty)
                                .GroupBy(o => o.Split_1st())
                                .Select(o => o.First())
                                .ToList()
                    : dataRows
                        .Where(dataRow => dataRow.Long("SiteId") == siteId)
                        .Select(dataRow =>
                            dataRow.String("ReferenceId") + "," + dataRow.String("Title"))
                        .ToList();
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
            var columns = EditorColumns.Where(o => o != "Ver").ToList();
            return ColumnDefinitionHash.ExportDefinitions()
                .Where(o => columns.Contains(o.ColumnName) || o.ExportColumn)
                .Select(o => new ExportColumn(
                    context: context,
                    ss: this,
                    columnName: o.ColumnName))
                .ToList();
        }

        public List<ExportColumn> ExportColumns(Context context, string searchText)
        {
            return ColumnDefinitionHash.ExportDefinitions()
                .OrderBy(o => o.EditorColumn)
                .Select((o, i) => new ExportColumn(
                    context: context,
                    ss: this,
                    columnName: o.ColumnName))
                .Where(o =>
                    searchText.IsNullOrEmpty() ||
                    Title.Contains(searchText) ||
                    o.ColumnName.Contains(searchText) ||
                    o.GetLabelText().Contains(searchText))
                .ToList();
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
                        hash = Rds.ExecuteTable(
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
                                                .Deserialize<SiteSettings>());
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

        public SqlJoinCollection Join(
            Context context, bool withColumn = false, List<string> columns = null)
        {
            return SqlJoinCollection(context: context, columns: (withColumn
                ? Arrays.Concat(GridColumns, FilterColumns, UseSearchTitleColumns(), columns)
                : Arrays.Concat(GridColumns, FilterColumns, UseSearchTitleColumns(), columns))
                    .Where(o => o?.Contains(",") == true)
                    .Select(columnName => GetColumn(context: context, columnName: columnName))
                    .ToList());
        }

        public SqlJoinCollection SqlJoinCollection(Context context, IEnumerable<Column> columns)        {
            return new SqlJoinCollection(columns
                .Where(o => o != null)
                .SelectMany(o => o.SqlJoinCollection(context: context, ss: this))
                .OrderBy(o => o.JoinExpression.Length)
                .GroupBy(o => o.JoinExpression)
                .Select(o => o.First())
                .ToArray());
        }

        private List<string> UseSearchTitleColumns(bool titleOnly = false)
        {
            return Columns
                .Select(column => new
                {
                    Link = column.SiteSettings?.Links
                        .FirstOrDefault(p => p.ColumnName == column.Name),
                    Column = column
                })
                .Where(o => o.Link != null)
                .Select(o => (!o.Column.TableAlias.IsNullOrEmpty()
                    ? o.Column.TableAlias + "-"
                    : string.Empty) +
                        o.Link.ColumnName + $"~{o.Link.SiteId},Title")
                .ToList();
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
            var join = JoinOptionHash?.FirstOrDefault(o => o.Key == column.TableName());
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

        public string GridCss()
        {
            switch (TableType)
            {
                case Sqls.TableTypes.History:
                    return "grid history";
                case Sqls.TableTypes.Deleted:
                    return "grid deleted not-link";
                default:
                    return "grid";
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
            Context context,  int id, bool enabled = true)
        {
            return enabled
                ? ColumnUtilities.SelectableOptions(
                    context: context,
                    ss: this,
                    columns: GetRelatingColumn(id).Columns ?? new List<string>(),
                    order: EditorColumns
                        .Where(o => o.StartsWith("Class")).ToList<string>())
                : ColumnUtilities.SelectableSourceOptions(
                    context: context,
                    ss: this,
                    columns: EditorColumns
                        .Where(o => o.StartsWith("Class")
                            && GetRelatingColumn(id).Columns?.Contains(o) != true),
                    order: EditorColumns
                        .Where(o => o.StartsWith("Class")).ToList<string>());
        }

        private static string LinkHashRelatingColumnsSubQuery(string referenceType, Column parentColumn, int parentId)
        {
            return (parentColumn == null || referenceType == "Wikis" || referenceType == "Sites") ? null
                : $"select [{Rds.IdColumn(referenceType)}] from [{referenceType}] where try_cast([{parentColumn.ColumnName}] as bigint) = {parentId}";
        }

        public void SetRelatingColumnsLinkedClass()
        {
            Dictionary<string, string> linkedClasses = new Dictionary<string, string>();
            foreach(var relcol in RelatingColumns.Where(o => o.Columns != null).Select(o => o))
            {
                var precol = "";
                foreach (var col in relcol.Columns)
                {
                    if(precol != "")
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
            var parentSiteId = Links.Where(o => o.ColumnName == parentClassName).Select(o => o.SiteId).FirstOrDefault();
            var childSiteId = Links.Where(o => o.ColumnName == childClassName).Select(o => o.SiteId).FirstOrDefault();
            return Destinations
                .Where(dest => dest.SiteId == childSiteId)
                .Select(dest =>
                    dest.Links
                    .Where(l => l.SiteId == parentSiteId)
                    .Select(l => new {
                        l.ColumnName,
                        OrderNo = dest.EditorColumns.Contains(l.ColumnName)
                            ? dest.EditorColumns.Select((v,k) => new { Key = k, Value = v }).Where(d => d.Value == l.ColumnName).Select(d => d.Key).First()
                            : int.MaxValue
                    })
                    .OrderBy(o => o.OrderNo)
                    .Select(o => o.ColumnName)
                    .FirstOrDefault())
                .SingleOrDefault();
        }
    }
}