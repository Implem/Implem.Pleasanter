using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.HtmlParts;
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
        public decimal Version;
        [NonSerialized]
        public bool Migrated;
        [NonSerialized]
        public long SiteId;
        [NonSerialized]
        public long InheritPermission;
        [NonSerialized]
        public long ParentId;
        [NonSerialized]
        public string Title;
        [NonSerialized]
        public Databases.AccessStatuses AccessStatus;
        [NonSerialized]
        public Dictionary<string, Column> ColumnHash;
        public string ReferenceType;
        public decimal? NearCompletionTimeAfterDays;
        public decimal? NearCompletionTimeBeforeDays;
        public int? GridPageSize;
        public int? GridView;
        public int? FirstDayOfWeek;
        public int? FirstMonth;
        public List<string> GridColumns;
        public List<string> FilterColumns;
        public List<string> EditorColumns;
        public List<string> TitleColumns;
        public List<string> LinkColumns;
        public List<string> HistoryColumns;
        public List<Column> ColumnCollection;
        public int ViewLatestId;
        public List<View> Views;
        public List<Notification> Notifications;
        public List<Aggregation> AggregationCollection;
        public List<Link> LinkCollection;
        public List<Summary> SummaryCollection;
        public List<FormulaSet> Formulas;
        public string TitleSeparator = ")";
        public string AddressBook;
        public string MailToDefault;
        public string MailCcDefault;
        public string MailBccDefault;
        public string GridStyle;
        public string NewStyle;
        public string EditStyle;
        public string GridScript;
        public string NewScript;
        public string EditScript;
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

        public SiteSettings()
        {
        }

        public SiteSettings(string referenceType)
        {
            ReferenceType = referenceType;
            Init();
        }

        public void Init()
        {
            Version = Parameters.Asset.SiteSettingsVersion;
            NearCompletionTimeBeforeDays = NearCompletionTimeBeforeDays ??
                Parameters.General.NearCompletionTimeBeforeDays;
            NearCompletionTimeAfterDays = NearCompletionTimeAfterDays ??
                Parameters.General.NearCompletionTimeAfterDays;
            GridPageSize = GridPageSize ?? Parameters.General.GridPageSize;
            FirstDayOfWeek = FirstDayOfWeek ?? Parameters.General.FirstDayOfWeek;
            FirstMonth = FirstMonth ?? Parameters.General.FirstMonth;
            UpdateGridColumnsOrder();
            UpdateFilterColumnsOrder();
            UpdateEditorColumnsOrder();
            UpdateTitleColumnsOrder();
            UpdateLinkColumnsOrder();
            UpdateHistoryColumnsOrder();
            UpdateColumnCollection();
            UpdateColumnHash();
            if (Notifications == null) Notifications = new List<Notification>();
            if (AggregationCollection == null) AggregationCollection = new List<Aggregation>();
            if (LinkCollection == null) LinkCollection = new List<Link>();
            if (SummaryCollection == null) SummaryCollection = new List<Summary>();
            if (Formulas == null) Formulas = new List<FormulaSet>();
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext streamingContext)
        {
            if (Version != Parameters.Asset.SiteSettingsVersion)
            {
                Migrators.SiteSettingsMigrator.Migrate(this);
            }
            Init();
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext streamingContext)
        {
            UpdateColumnCollection(onSerializing: true);
        }

        public string RecordingJson()
        {
            var def = new SiteSettings(ReferenceType);
            var self = this.ToJson().Deserialize<SiteSettings>();
            if (self.NearCompletionTimeAfterDays == def.NearCompletionTimeAfterDays) self.NearCompletionTimeAfterDays = null;
            if (self.NearCompletionTimeBeforeDays == def.NearCompletionTimeBeforeDays) self.NearCompletionTimeBeforeDays = null;
            if (self.GridPageSize == def.GridPageSize) self.GridPageSize = null;
            if (self.GridView == 0) self.GridView = null;
            if (self.FirstDayOfWeek == def.FirstDayOfWeek) self.FirstDayOfWeek = null;
            if (self.FirstMonth == def.FirstMonth) self.FirstMonth = null;
            if (self.GridColumns.SequenceEqual(def.GridColumns)) self.GridColumns = null;
            if (self.FilterColumns.SequenceEqual(def.FilterColumns)) self.FilterColumns = null;
            if (self.EditorColumns.SequenceEqual(def.EditorColumns)) self.EditorColumns = null;
            if (self.TitleColumns.SequenceEqual(def.TitleColumns)) self.TitleColumns = null;
            if (self.TitleSeparator == def.TitleSeparator) self.TitleSeparator = null;
            if (self.LinkColumns.SequenceEqual(def.LinkColumns)) self.LinkColumns = null;
            if (self.HistoryColumns.SequenceEqual(def.HistoryColumns)) self.HistoryColumns = null;
            if (self.ColumnCollection.SequenceEqual(def.ColumnCollection)) self.ColumnCollection = null;
            if (self.Views?.Count == 0) self.Views = null;
            if (!self.Notifications.Any()) self.Notifications = null;
            if (self.AggregationCollection.SequenceEqual(def.AggregationCollection)) self.AggregationCollection = null;
            if (self.LinkCollection.SequenceEqual(def.LinkCollection)) self.LinkCollection = null;
            if (self.SummaryCollection.SequenceEqual(def.SummaryCollection)) self.SummaryCollection = null;
            if (self.Formulas.SequenceEqual(def.Formulas)) self.Formulas = null;
            if (AddressBook == string.Empty) self.AddressBook = null;
            if (MailToDefault == string.Empty) self.MailToDefault = null;
            if (MailCcDefault == string.Empty) self.MailCcDefault = null;
            if (MailBccDefault == string.Empty) self.MailBccDefault = null;
            if (GridStyle == string.Empty) self.GridStyle = null;
            if (NewStyle == string.Empty) self.NewStyle = null;
            if (EditStyle == string.Empty) self.EditStyle = null;
            if (GridScript == string.Empty) self.GridScript = null;
            if (NewScript == string.Empty) self.NewScript = null;
            if (EditScript == string.Empty) self.EditScript = null;
            var removeCollection = new HashSet<string>();
            self.ColumnCollection.ForEach(column =>
            {
                var columnDefinition = Def.ColumnDefinitionCollection
                    .Where(o => o.TableName == ReferenceType)
                    .Where(o => o.ColumnName == column.ColumnName)
                    .FirstOrDefault();
                if (column.ToJson() == def.ColumnCollection.FirstOrDefault(o =>
                    o.ColumnName == column.ColumnName)?.ToJson())
                {
                    removeCollection.Add(column.ColumnName);
                }
                else
                {
                    var labelText = column.LabelText;
                    if (column.LabelText == Displays.Get(columnDefinition.Id)) column.LabelText = null;
                    if (column.GridLabelText == labelText) column.GridLabelText = null;
                    if (column.ChoicesText == columnDefinition.ChoicesText) column.ChoicesText = null;
                    if (column.DefaultInput == columnDefinition.DefaultInput) column.DefaultInput = null;
                    if (column.GridFormat == columnDefinition.GridFormat) column.GridFormat = null;
                    if (column.ControlFormat == columnDefinition.ControlFormat) column.ControlFormat = null;
                    if (column.ExportFormat == columnDefinition.ExportFormat) column.ExportFormat = null;
                    if (column.ControlType == columnDefinition.ControlType) column.ControlType = null;
                    if (column.Format?.Trim() == string.Empty) column.Format = null;
                    if (column.ValidateRequired == columnDefinition.ValidateRequired) column.ValidateRequired = null;
                    if (column.ValidateNumber == columnDefinition.ValidateNumber) column.ValidateNumber = null;
                    if (column.ValidateDate == columnDefinition.ValidateDate) column.ValidateDate = null;
                    if (column.ValidateEmail == columnDefinition.ValidateEmail) column.ValidateEmail = null;
                    if (column.ValidateEqualTo == columnDefinition.ValidateEqualTo) column.ValidateEqualTo = null;
                    if (column.ValidateMaxLength == columnDefinition.MaxLength) column.ValidateMaxLength = null;
                    if (column.DecimalPlaces == columnDefinition.DecimalPlaces) column.DecimalPlaces = null;
                    if (column.Min == columnDefinition.Min) column.Min = null;
                    if (column.Max == DefaultMax(columnDefinition)) column.Max = null;
                    if (column.Step == DefaultStep(columnDefinition)) column.Step = null;
                    if (column.EditorReadOnly == false) column.EditorReadOnly = null;
                    if (column.FieldCss == columnDefinition.FieldCss) column.FieldCss = null;
                    if (column.Unit == columnDefinition.Unit) column.Unit = null;
                    if (column.Link == false) column.Link = null;
                    if (column.CheckFilterControlType == ColumnUtilities.CheckFilterControlTypes.OnOnly) column.CheckFilterControlType = null;
                    if (column.NumFilterMin == Parameters.General.NumFilterMin) column.NumFilterMin = null;
                    if (column.NumFilterMax == Parameters.General.NumFilterMax) column.NumFilterMax = null;
                    if (column.NumFilterStep == Parameters.General.NumFilterStep) column.NumFilterStep = null;
                    if (column.DateFilterMinSpan == Parameters.General.DateFilterMinSpan) column.DateFilterMinSpan = null;
                    if (column.DateFilterMaxSpan == Parameters.General.DateFilterMaxSpan) column.DateFilterMaxSpan = null;
                    if (column.DateFilterFy.ToBool()) column.DateFilterFy = null;
                    if (column.DateFilterHalf.ToBool()) column.DateFilterHalf = null;
                    if (column.DateFilterQuarter.ToBool()) column.DateFilterQuarter = null;
                    if (column.DateFilterMonth.ToBool()) column.DateFilterMonth = null;
                }
            });
            self.ColumnCollection.RemoveAll(o => removeCollection.Contains(o.ColumnName));
            return self.ToJson();
        }

        private void UpdateGridColumnsOrder()
        {
            if (GridColumns == null)
            {
                GridColumns = ColumnUtilities.GridDefinitions(
                    ReferenceType, enableOnly: true)
                        .Select(o => o.ColumnName)
                        .ToList();
            }
            else
            {
                GridColumns.RemoveAll(o =>
                    !Def.ColumnDefinitionCollection.Any(p =>
                        p.ColumnName == o &&
                        p.TableName == ReferenceType &&
                        p.GridColumn > 0));
            }
        }

        private void UpdateFilterColumnsOrder()
        {
            if (FilterColumns == null)
            {
                FilterColumns = ColumnUtilities.FilterDefinitions(
                    ReferenceType, enableOnly: true)
                        .Select(o => o.ColumnName)
                        .ToList();
            }
            else
            {
                FilterColumns.RemoveAll(o =>
                    !Def.ColumnDefinitionCollection.Any(p =>
                        p.ColumnName == o &&
                        p.TableName == ReferenceType &&
                        p.FilterColumn > 0));
            }
        }

        private void UpdateEditorColumnsOrder()
        {
            if (EditorColumns == null)
            {
                EditorColumns = ColumnUtilities.EditorDefinitions(
                    ReferenceType, enableOnly: true)
                        .Select(o => o.ColumnName)
                        .ToList();
            }
            else
            {
                EditorColumns.RemoveAll(o =>
                    !Def.ColumnDefinitionCollection.Any(p =>
                        p.ColumnName == o &&
                        p.TableName == ReferenceType &&
                        p.EditorColumn &&
                        !p.NotEditorSettings));
            }
        }

        private void UpdateTitleColumnsOrder()
        {
            if (TitleColumns == null)
            {
                TitleColumns = ColumnUtilities.TitleDefinitions(
                    ReferenceType, enableOnly: true)
                        .Select(o => o.ColumnName)
                        .ToList();
            }
            else
            {
                TitleColumns.RemoveAll(o =>
                    !Def.ColumnDefinitionCollection.Any(p =>
                        p.TableName == ReferenceType &&
                        p.ColumnName == o &&
                        p.TitleColumn > 0 &&
                        !p.NotEditorSettings));
            }
        }

        private void UpdateLinkColumnsOrder()
        {
            if (LinkColumns == null)
            {
                LinkColumns = ColumnUtilities.LinkDefinitions(
                    ReferenceType, enableOnly: true)
                        .Select(o => o.ColumnName)
                        .ToList();
            }
            else
            {
                LinkColumns.RemoveAll(o =>
                    !Def.ColumnDefinitionCollection.Any(p =>
                        p.TableName == ReferenceType &&
                        p.ColumnName == o &&
                        p.LinkColumn > 0));
            }
        }

        private void UpdateHistoryColumnsOrder()
        {
            if (HistoryColumns == null)
            {
                HistoryColumns = ColumnUtilities.HistoryDefinitions(
                    ReferenceType, enableOnly: true)
                        .Select(o => o.ColumnName)
                        .ToList();
            }
            else
            {
                HistoryColumns.RemoveAll(o =>
                    !Def.ColumnDefinitionCollection.Any(p =>
                        p.TableName == ReferenceType &&
                        p.ColumnName == o &&
                        p.HistoryColumn > 0));
            }
        }

        private void UpdateColumnCollection(bool onSerializing = false)
        {
            if (ColumnCollection == null) ColumnCollection = new List<Column>();
            Def.ColumnDefinitionCollection
                .Where(o => o.TableName == ReferenceType)
                .ForEach(columnDefinition =>
                {
                    if (!onSerializing)
                    {
                        if (!ColumnCollection.Exists(o =>
                            o.ColumnName == columnDefinition.ColumnName))
                        {
                            ColumnCollection.Add(new Column(columnDefinition.ColumnName));
                        }
                        UpdateColumn(columnDefinition);
                    }
                });
            ColumnCollection.RemoveAll(o =>
                !Def.ColumnDefinitionCollection.Any(p =>
                    p.TableName == ReferenceType && p.ColumnName == o.ColumnName));
        }

        private void UpdateColumn(ColumnDefinition columnDefinition)
        {
            var column = ColumnCollection.Find(o => o.ColumnName == columnDefinition.ColumnName);
            if (column != null)
            {
                column.Id = column.Id ?? columnDefinition.Id;
                column.No = columnDefinition.No;
                column.Id_Ver = columnDefinition.Unique || columnDefinition.ColumnName == "Ver";
                column.ColumnName = column.ColumnName ?? columnDefinition.ColumnName;
                column.LabelText = column.LabelText ?? Displays.Get(columnDefinition.Id);
                column.GridLabelText = column.GridLabelText ?? column.LabelText;
                column.ChoicesText = column.ChoicesText ?? columnDefinition.ChoicesText;
                column.DefaultInput = column.DefaultInput ?? columnDefinition.DefaultInput;
                column.GridFormat = column.GridFormat ?? columnDefinition.GridFormat;
                column.ControlFormat = column.ControlFormat ?? columnDefinition.ControlFormat;
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
                column.EditorReadOnly = column.EditorReadOnly ?? false;
                column.FieldCss = column.FieldCss ?? columnDefinition.FieldCss;
                column.Unit = column.Unit ?? columnDefinition.Unit;
                column.CheckFilterControlType = column.CheckFilterControlType ?? ColumnUtilities.CheckFilterControlTypes.OnOnly;
                column.NumFilterMin = column.NumFilterMin ?? Parameters.General.NumFilterMin;
                column.NumFilterMax = column.NumFilterMax ?? Parameters.General.NumFilterMax;
                column.NumFilterStep = column.NumFilterStep ?? Parameters.General.NumFilterStep;
                column.DateFilterMinSpan = column.DateFilterMinSpan ?? Parameters.General.DateFilterMinSpan;
                column.DateFilterMaxSpan = column.DateFilterMaxSpan ?? Parameters.General.DateFilterMaxSpan;
                column.DateFilterFy = column.DateFilterFy ?? true;
                column.DateFilterHalf = column.DateFilterHalf ?? true;
                column.DateFilterQuarter = column.DateFilterQuarter ?? true;
                column.DateFilterMonth = column.DateFilterMonth ?? true;
                column.Size = columnDefinition.Size;
                column.Nullable = columnDefinition.Nullable;
                column.RecordedTime = columnDefinition.Default == "now";
                column.ReadPermission = Permissions.Get(columnDefinition.ReadPermission);
                column.CreatePermission = Permissions.Get(columnDefinition.CreatePermission);
                column.UpdatePermission = Permissions.Get(columnDefinition.UpdatePermission);
                column.NotSelect = columnDefinition.NotSelect;
                column.NotUpdate = columnDefinition.NotUpdate;
                column.EditSelf = !columnDefinition.NotEditSelf;
                column.GridColumn = columnDefinition.GridColumn > 0;
                column.FilterColumn = columnDefinition.FilterColumn > 0;
                column.EditorColumn = columnDefinition.EditorColumn;
                column.NotEditorSettings = columnDefinition.NotEditorSettings;
                column.TitleColumn = columnDefinition.TitleColumn > 0;
                column.LinkColumn = columnDefinition.LinkColumn > 0;
                column.HistoryColumn = columnDefinition.HistoryColumn > 0;
                column.Export = columnDefinition.Export > 0;
                column.LabelTextDefault = Displays.Get(columnDefinition.Id);
                column.TypeName = columnDefinition.TypeName;
                column.TypeCs = columnDefinition.TypeCs;
                column.UserColumn = columnDefinition.UserColumn;
                column.Hash = columnDefinition.Hash;
                column.StringFormat = columnDefinition.StringFormat;
                column.UnitDefault = columnDefinition.Unit;
                column.Width = columnDefinition.Width;
                column.ControlCss = columnDefinition.ControlCss;
                column.MarkDown = columnDefinition.MarkDown;
                column.GridStyle = columnDefinition.GridStyle;
                column.Aggregatable = columnDefinition.Aggregatable;
                column.Computable = columnDefinition.Computable;
            }
        }

        private void UpdateColumnHash()
        {
            ColumnHash = ColumnCollection.ToDictionary(o => o.ColumnName, o => o);
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

        public Column GetColumn(string columnName)
        {
            return ColumnHash.Keys.Contains(columnName)
                ? ColumnHash[columnName]
                : null;
        }

        public Column GridColumn(string columnName)
        {
            return ColumnCollection.FirstOrDefault(o =>
                o.ColumnName == columnName && o.GridColumn);
        }

        public Column FilterColumn(string columnName)
        {
            return ColumnCollection.FirstOrDefault(o =>
                o.ColumnName == columnName && o.FilterColumn);
        }

        public Column EditorColumn(string columnName)
        {
            return ColumnCollection.FirstOrDefault(o =>
                o.ColumnName == columnName && o.EditorColumn);
        }

        public Column TitleColumn(string columnName)
        {
            return ColumnCollection.FirstOrDefault(o =>
                o.ColumnName == columnName && o.TitleColumn);
        }

        public Column LinkColumn(string columnName)
        {
            return ColumnCollection.FirstOrDefault(o =>
                o.ColumnName == columnName && o.LinkColumn);
        }

        public Column HistoryColumn(string columnName)
        {
            return ColumnCollection.FirstOrDefault(o =>
                o.ColumnName == columnName && o.HistoryColumn);
        }

        public Column FormulaColumn(string name)
        {
            return ColumnCollection
                .Where(o => o.ColumnName == name || o.LabelText == name)
                .Where(o => o.Computable)
                .Where(o => !o.NotUpdate)
                .Where(o => o.TypeName != "datetime")
                .FirstOrDefault();
        }

        public IEnumerable<Column> GridColumnCollection()
        {
            return GridColumns
                .Select(o => GetColumn(o))
                .Where(o => o != null)
                .ToList();
        }

        public IEnumerable<Column> FilterColumnCollection()
        {
            return FilterColumns
                .Select(o => GetColumn(o))
                .Where(o => o != null)
                .ToList();
        }

        public IEnumerable<Column> EditorColumnCollection()
        {
            return EditorColumns
                .Select(o => GetColumn(o))
                .Where(o => o != null)
                .ToList();
        }

        public IEnumerable<Column> TitleColumnCollection()
        {
            return TitleColumns
                .Select(o => GetColumn(o))
                .Where(o => o != null)
                .ToList();
        }

        public IEnumerable<Column> LinkColumnCollection()
        {
            return LinkColumns
                .Select(o => GetColumn(o))
                .Where(o => o != null)
                .ToList();
        }

        public IEnumerable<Column> HistoryColumnCollection()
        {
            return HistoryColumns
                .Select(o => GetColumn(o))
                .Where(o => o != null)
                .ToList();
        }

        public IEnumerable<Column> SelectColumnCollection()
        {
            return ColumnCollection.Where(o =>
                !o.Nullable.ToBool() ||
                EditorColumns.Contains(o.ColumnName) ||
                EditorColumns.Contains(o.ColumnName));
        }

        public Dictionary<string, string> GridSelectableOptions(bool enabled = true)
        {
            return enabled
                ? ColumnUtilities.SelectableOptions(this, GridColumns)
                : ColumnUtilities.SelectableOptions(
                    this, ColumnUtilities.GridDefinitions(ReferenceType)
                        .Where(o => !GridColumns.Contains(o.ColumnName))
                        .Select(o => o.ColumnName));
        }

        public Dictionary<string, string> FilterSelectableOptions(bool enabled = true)
        {
            return enabled
                ? ColumnUtilities.SelectableOptions(this, FilterColumns)
                : ColumnUtilities.SelectableOptions(
                    this, ColumnUtilities.FilterDefinitions(ReferenceType)
                        .Where(o => !FilterColumns.Contains(o.ColumnName))
                        .Select(o => o.ColumnName));
        }

        public Dictionary<string, string> EditorSelectableOptions(bool enabled = true)
        {
            return enabled
                ? ColumnUtilities.SelectableOptions(this, EditorColumns)
                : ColumnUtilities.SelectableOptions(
                    this, ColumnUtilities.EditorDefinitions(ReferenceType)
                        .Where(o => !EditorColumns.Contains(o.ColumnName))
                        .Select(o => o.ColumnName));
        }

        public Dictionary<string, string> TitleSelectableOptions(
            IEnumerable<string> titleColumns, bool enabled = true)
        {
            return enabled
                ? ColumnUtilities.SelectableOptions(this, titleColumns)
                : ColumnUtilities.SelectableOptions(
                    this, ColumnUtilities.TitleDefinitions(ReferenceType)
                        .Where(o => !titleColumns.Contains(o.ColumnName))
                        .Select(o => o.ColumnName));
        }

        public Dictionary<string, string> LinkSelectableOptions(bool enabled = true)
        {
            return enabled
                ? ColumnUtilities.SelectableOptions(this, LinkColumns)
                : ColumnUtilities.SelectableOptions(
                    this, ColumnUtilities.LinkDefinitions(ReferenceType)
                        .Where(o => !LinkColumns.Contains(o.ColumnName))
                        .Select(o => o.ColumnName));
        }

        public Dictionary<string, string> HistorySelectableOptions(bool enabled = true)
        {
            return enabled
                ? ColumnUtilities.SelectableOptions(this, HistoryColumns)
                : ColumnUtilities.SelectableOptions(
                    this, ColumnUtilities.HistoryDefinitions(ReferenceType)
                        .Where(o => !HistoryColumns.Contains(o.ColumnName))
                        .Select(o => o.ColumnName));
        }

        public Dictionary<string, string> FormulaTargetSelectableOptions()
        {
            return ColumnCollection
                .Where(o => o.Computable)
                .Where(o => !o.NotUpdate)
                .Where(o => o.TypeName != "datetime")
                .ToDictionary(o => o.ColumnName, o => o.LabelText);
        }

        public Dictionary<string, string> ViewSelectableOptions()
        {
            return Views != null
                ? Views.ToDictionary(o => o.Id.ToString(), o => o.Name)
                : new Dictionary<string, string>();
        }

        public Dictionary<string, string> MonitorChangesSelectableOptions(
            IEnumerable<string> monitorChangesColumns, bool enabled = true)
        {
            return enabled
                ? ColumnUtilities.SelectableOptions(this, monitorChangesColumns)
                : ColumnUtilities.SelectableOptions(
                    this, ColumnUtilities.MonitorChangesDefinitions(ReferenceType)
                        .Where(o => !monitorChangesColumns.Contains(o.ColumnName))
                        .Select(o => o.ColumnName));
        }

        public Dictionary<string, string> AggregationDestination()
        {
            return AggregationCollection?.ToDictionary(
                o => o.Id.ToString(),
                o => (o.GroupBy == "[NotGroupBy]"
                    ? Displays.NoClassification() 
                    : GetColumn(o.GroupBy).LabelText) +
                        " (" +
                        Displays.Get(o.Type.ToString()) +
                        (o.Target != string.Empty
                            ? ": " + GetColumn(o.Target).LabelText
                            : string.Empty) +
                        ")");
        }

        public Dictionary<string, string> AggregationSource()
        {
            var aggregationSource = new Dictionary<string, string>
            {
                { "[NotGroupBy]", Displays.NoClassification() }
            };
            return aggregationSource.AddRange(Def.ColumnDefinitionCollection
                .Where(o => o.TableName == ReferenceType)
                .Where(o => o.Aggregatable)
                .ToDictionary(
                    o => o.ColumnName,
                    o => GetColumn(o.ColumnName).LabelText));
        }

        public Dictionary<string, string> GanttGroupByOptions()
        {
            return ColumnCollection
                .Where(o => o.HasChoices())
                .ToDictionary(o => o.ColumnName, o => o.GridLabelText);
        }

        public Dictionary<string, string> TimeSeriesGroupByOptions()
        {
            return ColumnCollection
                .Where(o => o.HasChoices())
                .ToDictionary(o => o.ColumnName, o => o.LabelText);
        }

        public Dictionary<string, string> TimeSeriesAggregationTypeOptions()
        {
            return new Dictionary<string, string>
            {
                { "Count", Displays.Count() },
                { "Total", Displays.Total() },
                { "Average", Displays.Average() },
                { "Max", Displays.Max() },
                { "Min", Displays.Min() }
            };
        }

        public Dictionary<string, string> TimeSeriesValueOptions()
        {
            return ColumnCollection
                .Where(o => o.Computable)
                .Where(o => o.TypeName != "datetime")
                .ToDictionary(o => o.ColumnName, o => o.LabelText);
        }

        public Dictionary<string, string> KambanGroupByOptions()
        {
            return ColumnCollection.Where(o => o.HasChoices())
                .ToDictionary(o => o.ColumnName, o => o.GridLabelText);
        }

        public Dictionary<string, string> KambanAggregationTypeOptions()
        {
            return new Dictionary<string, string>
            {
                { "Total", Displays.Total() },
                { "Average", Displays.Average() },
                { "Max", Displays.Max() },
                { "Min", Displays.Min() }
            };
        }

        public Dictionary<string, string> KamvanValueOptions()
        {
            return ColumnCollection
                .Where(o => o.Computable)
                .Where(o => o.TypeName != "datetime")
                .ToDictionary(o => o.ColumnName, o => o.GridLabelText);
        }

        public int GridNextOffset(int offset, int count, int totalCount)
        {
            return offset + count < totalCount
                ? offset + GridPageSize.ToInt()
                : -1;
        }

        public void Set(string propertyName, string value)
        {
            switch (propertyName)
            {
                case "NearCompletionTimeBeforeDays": NearCompletionTimeBeforeDays = value.ToInt(); break;
                case "NearCompletionTimeAfterDays": NearCompletionTimeAfterDays = value.ToInt(); break;
                case "GridPageSize": GridPageSize = value.ToInt(); break;
                case "GridView": GridView = value.ToInt(); break;
                case "FirstDayOfWeek": FirstDayOfWeek = value.ToInt(); break;
                case "FirstMonth": FirstMonth = value.ToInt(); break;
                case "TitleSeparator": TitleSeparator = value; break;
                case "AddressBook": AddressBook = value; break;
                case "MailToDefault": MailToDefault = value; break;
                case "MailCcDefault": MailCcDefault = value; break;
                case "MailBccDefault": MailBccDefault = value; break;
                case "GridStyle": GridStyle = value; break;
                case "NewStyle": NewStyle = value; break;
                case "EditStyle": EditStyle = value; break;
                case "GridScript": GridScript = value; break;
                case "NewScript": NewScript = value; break;
                case "EditScript": EditScript = value; break;
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
                        var id = AggregationCollection.Count > 0
                            ? AggregationCollection.Max(o => o.Id) + 1
                            : 1;
                        idCollection.Add(id.ToString());
                        AggregationCollection.Add(new Aggregation(id, groupBy));
                    });
                    selectedColumns = idCollection;
                    selectedSourceColumns = null;
                    break;
                case "DeleteAggregations":
                    AggregationCollection
                        .RemoveAll(o => selectedColumns.Contains(o.Id.ToString()));
                    selectedSourceColumns = selectedColumns;
                    selectedColumns = null;
                    break;
                case "MoveUpAggregations":
                case "MoveDownAggregations":
                    var order = AggregationCollection.Select(o => o.Id.ToString()).ToArray();
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
                    AggregationCollection = order.ToList().Select(id => AggregationCollection
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
            AggregationCollection
                .Where(o => selectedColumns.Contains(o.Id.ToString()))
                .ForEach(aggregation =>
                {
                    aggregation.Type = type;
                    aggregation.Target = target;
                });
        }

        public void SetGridColumns(
            string command, List<string> selectedColumns, List<string> selectedSourceColumns)
        {
            GridColumns = ColumnUtilities.GetChanged(
                GridColumns, command, selectedColumns, selectedSourceColumns);
        }

        public void SetFilterColumns(
            string command, List<string> selectedColumns, List<string> selectedSourceColumns)
        {
            FilterColumns = ColumnUtilities.GetChanged(
                FilterColumns, command, selectedColumns, selectedSourceColumns);
        }

        public void SetEditorColumns(
            string command, List<string> selectedColumns, List<string> selectedSourceColumns)
        {
            EditorColumns = ColumnUtilities.GetChanged(
                EditorColumns, command, selectedColumns, selectedSourceColumns);
        }

        public void SetLinkColumns(
            string command, List<string> selectedColumns, List<string> selectedSourceColumns)
        {
            LinkColumns = ColumnUtilities.GetChanged(
                LinkColumns, command, selectedColumns, selectedSourceColumns);
        }

        public void SetHistoryColumns(
            string command, List<string> selectedColumns, List<string> selectedSourceColumns)
        {
            HistoryColumns = ColumnUtilities.GetChanged(
                HistoryColumns, command, selectedColumns, selectedSourceColumns);
        }

        public void SetViewsOrder(string command, IEnumerable<int> selectedColumns)
        {
            Views = ColumnUtilities.GetChanged(
                Views, command, Views.Where(o => selectedColumns.Contains(o.Id)).ToList());
        }

        public void SetColumnProperty(Column column, string propertyName, string value)
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
                case "GridDesign":
                    column.GridDesign = GridDesignRecordingData(column, value);
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
                case "EditorReadOnly": column.EditorReadOnly = value.ToBool(); break;
                case "FieldCss": column.FieldCss = value; break;
                case "ChoicesText": column.ChoicesText = value; SetLinks(column); break;
                case "DefaultInput": column.DefaultInput = value; break;
                case "GridFormat": column.GridFormat = value; break;
                case "ControlFormat": column.ControlFormat = value; break;
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
            }
        }

        private string GridDesignRecordingData(Column currentColumn, string value)
        {
            if (!value.IsNullOrEmpty())
            {
                IncludedColumns(value, labelText: true).ForEach(column =>
                    value = value.Replace(
                        "[" + column.LabelText + "]", "[" + column.ColumnName + "]"));
                return value != "[" + currentColumn.ColumnName + "]"
                    ? value
                    : null;
            }
            else
            {
                return null;
            }
        }

        public string GridDesignEditorText(Column column)
        {
            return column.GridDesign.IsNullOrEmpty()
                ? "[" + column.LabelText + "]"
                : GridDesignEditorText(column.GridDesign);
        }

        private string GridDesignEditorText(string gridDesign)
        {
            IncludedColumns(gridDesign).ForEach(column =>
                gridDesign = gridDesign.Replace(
                    "[" + column.ColumnName + "]", "[" + column.LabelText + "]"));
            return gridDesign;
        }

        public IEnumerable<string> IncludedColumns()
        {
            return IncludedColumns(ColumnCollection
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
                    ? ColumnCollection.FirstOrDefault(o =>
                        o.LabelText == match.Value)
                    : ColumnCollection.FirstOrDefault(o =>
                        o.ColumnName == match.Value);
                if (column != null) yield return column;
            }
        }

        public void AddView(View view)
        {
            ViewLatestId++;
            view.Id = ViewLatestId;
            if (Views == null) Views = new List<View>();
            Views.Add(view);
        }

        public Error.Types AddSummary(
            long siteId,
            string destinationReferenceType,
            string destinationColumn,
            string linkColumn,
            string type,
            string sourceColumn)
        {
            if (!SummaryCollection.Any(o =>
                o.SiteId == siteId &&
                o.DestinationColumn == destinationColumn &&
                o.LinkColumn == linkColumn))
            {
                var id = SummaryCollection.Any()
                    ? SummaryCollection.Select(o => o.Id).Max() + 1
                    : 1;
                SummaryCollection.Add(new Summary(
                    id,
                    siteId,
                    destinationReferenceType,
                    destinationColumn,
                    linkColumn,
                    type,
                    sourceColumn));
                return Error.Types.None;
            }
            else
            {
                return Error.Types.AlreadyAdded;
            }
        }

        public void DeleteSummary(long id)
        {
            SummaryCollection.Remove(SummaryCollection.FirstOrDefault(o => o.Id == id));
        }

        private void SetLinks(Column column)
        {
            column.Link = false;
            LinkCollection.RemoveAll(o => o.ColumnName == column.ColumnName);
            column.ChoicesText.SplitReturn()
                .Select(o => o.Trim())
                .Where(o => o.RegexExists(@"^\[\[[0-9]*\]\]$"))
                .Select(o => o.RegexFirst("[0-9]+").ToLong())
                .ForEach(siteId =>
                {
                    column.Link = true;
                    if (!LinkCollection.Any(o =>
                        o.ColumnName == column.ColumnName && o.SiteId == siteId))
                    {
                        if (new SiteModel(siteId).AccessStatus ==
                            Databases.AccessStatuses.Selected)
                        {
                            LinkCollection.Add(new Link(column.ColumnName, siteId));
                        }
                    }
                });
        }

        public void SetChoiceHash(bool withLink = true)
        {
            var linkHash = withLink
                ? LinkHash()
                : null;
            ColumnCollection.Where(o => o.HasChoices()).ForEach(column =>
                column.SetChoiceHash(InheritPermission, linkHash));
        }

        private Dictionary<string, Dictionary<string, string>> LinkHash()
        {
            if (LinkCollection?.Count > 0)
            {
                var dataRows = Rds.ExecuteTable(
                    statements: Rds.SelectItems(
                        column: Rds.ItemsColumn()
                            .ReferenceId()
                            .ReferenceType()
                            .SiteId()
                            .Title(),
                        where: Rds.ItemsWhere()
                            .ReferenceType("Sites", _operator: "<>")
                            .SiteId_In(
                                value: LinkCollection
                                    .Select(o => o.SiteId)
                                    .Distinct()),
                        orderBy: Rds.ItemsOrderBy()
                            .Title())).AsEnumerable();
                return LinkCollection
                    .Select(o => o.SiteId)
                    .Distinct()
                    .ToDictionary(
                        siteId => "[[" + siteId + "]]",
                        siteId => LinkValue(siteId, dataRows));
            }
            else
            {
                return null;
            }
        }

        private static Dictionary<string, string> LinkValue(
            long siteId, EnumerableRowCollection<DataRow> dataRows)
        {
            return dataRows.Any(o =>
                o["SiteId"].ToLong() == siteId &&
                o["ReferenceType"].ToString() == "Wikis")
                    ? Rds.ExecuteScalar_string(statements:
                        Rds.SelectWikis(
                            column: Rds.WikisColumn().Body(),
                            where: Rds.WikisWhere().SiteId(siteId)))
                                .SplitReturn()
                                .ToDictionary(
                                    p => p.Split_1st(),
                                    p => p.Split_2nd())
                    : dataRows
                        .Where(p => p["SiteId"].ToLong() == siteId)
                        .ToDictionary(
                            p => p["ReferenceId"].ToString(),
                            p => p["ReferenceId"].ToString() + ": " + p["Title"].ToString());
        }

        public EnumerableRowCollection<DataRow> SummarySiteDataRows()
        {
            if (LinkCollection == null) return null;
            return Rds.ExecuteTable(statements: Rds.SelectSites(
                column: Rds.SitesColumn()
                    .SiteId()
                    .ReferenceType()
                    .Title()
                    .SiteSettings(),
                where: Rds.SitesWhere()
                    .TenantId(Sessions.TenantId())
                    .SiteId_In(LinkCollection.Select(o => o.SiteId))))
                        .AsEnumerable();
        }

        public void SetFormulas(string controlId, IEnumerable<int> selected)
        {
            var order = Formulas.Select(o => o.Id).ToArray();
            switch (controlId)
            {
                case "MoveUpFormulas":
                case "MoveDownFormulas":
                    if (controlId == "MoveDownFormulas") Array.Reverse(order);
                    order.Select((o, i) => new { ColumnName = o, Index = i }).ForEach(data =>
                    {
                        if (selected.Contains(data.ColumnName))
                        {
                            if (data.Index > 0 &&
                                !selected.Contains(order[data.Index - 1]))
                            {
                                order = Arrays.Swap(order, data.Index, data.Index - 1);
                            }
                        }
                    });
                    if (controlId == "MoveDownFormulas") Array.Reverse(order);
                    Formulas = order
                        .Select(o => Formulas.FirstOrDefault(p => p.Id == o ))
                        .Where(o => o != null)
                        .ToList();
                    break;
            }
        }

        public void DeleteFormulas(IEnumerable<int> selected)
        {
            Formulas.RemoveAll(o => selected.Contains(o.Id));
        }

        public Dictionary<string, string> FormulaItemCollection()
        {
            return Formulas?.ToDictionary(
                o => o.Id.ToString(),
                o => o.ToString(this));
        }

        public decimal FormulaResult(
            string columnName, Formula formula, Dictionary<string, decimal> data)
        {
            return formula != null
                ? GetColumn(columnName).Round(formula.GetResult(data))
                : data[columnName];
        }
    }
}