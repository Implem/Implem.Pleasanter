using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
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
        public List<SiteSettings> Destinations;
        [NonSerialized]
        public List<SiteSettings> Sources;
        [NonSerialized]
        public bool Migrated;
        [NonSerialized]
        public long SiteId;
        [NonSerialized]
        public string Title;
        [NonSerialized]
        public long ParentId;
        [NonSerialized]
        public long InheritPermission;
        [NonSerialized]
        public Permissions.Types PermissionType;
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
        public List<Column> Columns;
        public List<Aggregation> Aggregations;
        public List<Link> Links;
        public SettingList<Summary> Summaries;
        public SettingList<FormulaSet> Formulas;
        public int ViewLatestId;
        public List<View> Views;
        public SettingList<Notification> Notifications;
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
        // compatibility Version 1.006
        public List<Column> ColumnCollection;
        public List<Aggregation> AggregationCollection;
        public List<Link> LinkCollection;
        public List<Summary> SummaryCollection;

        public SiteSettings()
        {
        }

        public SiteSettings(long id)
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
            UpdateGridColumns();
            UpdateFilterColumns();
            UpdateEditorColumns();
            UpdateTitleColumns();
            UpdateLinkColumns();
            UpdateHistoryColumns();
            UpdateColumns();
            UpdateColumnHash();
            if (Aggregations == null) Aggregations = new List<Aggregation>();
            if (Links == null) Links = new List<Link>();
            if (Summaries == null) Summaries = new SettingList<Summary>();
            if (Formulas == null) Formulas = new SettingList<FormulaSet>();
            if (Notifications == null) Notifications = new SettingList<Notification>();
        }

        public void SetLinkedSiteSettings()
        {
            var dataSet = Rds.ExecuteDataSet(statements: new SqlStatement[]
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
            Destinations = SiteSettingsList(dataSet.Tables["Destinations"]);
            Sources = SiteSettingsList(dataSet.Tables["Sources"]);
            SetPermissions();
        }

        private void SetPermissions()
        {
            var targets = new List<long> { InheritPermission };
            targets.AddRange(Destinations?.Select(o => o.InheritPermission));
            targets.AddRange(Sources?.Select(o => o.InheritPermission));
            var permissions = Permissions.Get(targets.Distinct());
            SetPermissions(this, permissions);
            Destinations?.ForEach(o => SetPermissions(o, permissions));
            Sources?.ForEach(o => SetPermissions(o, permissions));
        }

        private void SetPermissions(
            SiteSettings ss, Dictionary<long, Permissions.Types> permissions)
        {
            if (permissions.ContainsKey(ss.InheritPermission))
            {
                ss.PermissionType = permissions[ss.InheritPermission];
            }
        }

        private List<SiteSettings> SiteSettingsList(DataTable dataTable)
        {
            var ssList = new List<SiteSettings>();
            dataTable.AsEnumerable().ForEach(dataRow =>
            {
                var ss = SiteSettingsUtilities.Get(dataRow);
                ss.SiteId = dataRow["SiteId"].ToLong();
                ss.Title = dataRow["Title"].ToString();
                ss.ReferenceType = dataRow["ReferenceType"].ToString();
                ss.ParentId = dataRow["ParentId"].ToLong();
                ss.InheritPermission = dataRow["InheritPermission"].ToLong();
                ss.SetChoiceHash();
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
            Init();
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext streamingContext)
        {
            UpdateColumns(onSerializing: true);
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
            if (self.Columns.SequenceEqual(def.Columns)) self.Columns = null;
            self.Views?.ForEach(view => view.SetRecordingData());
            if (self.Views?.Count == 0) self.Views = null;
            if (!self.Notifications.Any()) self.Notifications = null;
            if (self.Aggregations.SequenceEqual(def.Aggregations)) self.Aggregations = null;
            if (self.Links.SequenceEqual(def.Links)) self.Links = null;
            if (self.Summaries.SequenceEqual(def.Summaries)) self.Summaries = null;
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
            self.Columns.ForEach(column =>
            {
                var columnDefinition = Def.ColumnDefinitionCollection
                    .Where(o => o.TableName == ReferenceType)
                    .Where(o => o.ColumnName == column.ColumnName)
                    .FirstOrDefault();
                if (column.ToJson() == def.Columns.FirstOrDefault(o =>
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
                    if (column.UseSearch == columnDefinition.UseSearch) column.UseSearch = null;
                    if (column.DefaultInput == columnDefinition.DefaultInput) column.DefaultInput = null;
                    if (column.GridFormat == columnDefinition.GridFormat) column.GridFormat = null;
                    if (column.EditorFormat == columnDefinition.EditorFormat) column.EditorFormat = null;
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
                    if (column.NumFilterMin == columnDefinition.NumFilterMin) column.NumFilterMin = null;
                    if (column.NumFilterMax == columnDefinition.NumFilterMax) column.NumFilterMax = null;
                    if (column.NumFilterStep == columnDefinition.NumFilterStep) column.NumFilterStep = null;
                    if (column.DateFilterMinSpan == Parameters.General.DateFilterMinSpan) column.DateFilterMinSpan = null;
                    if (column.DateFilterMaxSpan == Parameters.General.DateFilterMaxSpan) column.DateFilterMaxSpan = null;
                    if (column.DateFilterFy.ToBool()) column.DateFilterFy = null;
                    if (column.DateFilterHalf.ToBool()) column.DateFilterHalf = null;
                    if (column.DateFilterQuarter.ToBool()) column.DateFilterQuarter = null;
                    if (column.DateFilterMonth.ToBool()) column.DateFilterMonth = null;
                }
            });
            self.Columns.RemoveAll(o => removeCollection.Contains(o.ColumnName));
            return self.ToJson();
        }

        private void UpdateGridColumns()
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

        private void UpdateFilterColumns()
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

        private void UpdateEditorColumns()
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

        private void UpdateTitleColumns()
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

        private void UpdateLinkColumns()
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

        private void UpdateHistoryColumns()
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

        private void UpdateColumns(bool onSerializing = false)
        {
            if (Columns == null) Columns = new List<Column>();
            Def.ColumnDefinitionCollection
                .Where(o => o.TableName == ReferenceType)
                .ForEach(columnDefinition =>
                {
                    if (!onSerializing)
                    {
                        if (!Columns.Exists(o =>
                            o.ColumnName == columnDefinition.ColumnName))
                        {
                            Columns.Add(new Column(columnDefinition.ColumnName));
                        }
                        UpdateColumn(columnDefinition);
                    }
                });
            Columns.RemoveAll(o =>
                !Def.ColumnDefinitionCollection.Any(p =>
                    p.TableName == ReferenceType && p.ColumnName == o.ColumnName));
        }

        private void UpdateColumn(ColumnDefinition columnDefinition)
        {
            var column = Columns.Find(o => o.ColumnName == columnDefinition.ColumnName);
            if (column != null)
            {
                column.Id = column.Id ?? columnDefinition.Id;
                column.No = columnDefinition.No;
                column.Id_Ver = columnDefinition.Unique || columnDefinition.ColumnName == "Ver";
                column.ColumnName = column.ColumnName ?? columnDefinition.ColumnName;
                column.LabelText = column.LabelText ?? Displays.Get(columnDefinition.Id);
                column.GridLabelText = column.GridLabelText ?? column.LabelText;
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
                column.EditorReadOnly = column.EditorReadOnly ?? false;
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
            ColumnHash = Columns.ToDictionary(o => o.ColumnName, o => o);
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
            return columnName != null && ColumnHash.Keys.Contains(columnName)
                ? ColumnHash[columnName]
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
                .Where(o => o.Computable)
                .Where(o => !o.NotUpdate)
                .Where(o => o.TypeName != "datetime")
                .FirstOrDefault();
        }

        public IEnumerable<Column> GetGridColumns()
        {
            return GridColumns
                .Select(o => GetColumn(o))
                .Where(o => o != null)
                .ToList();
        }

        public IEnumerable<Column> GetFilterColumns()
        {
            return FilterColumns
                .Select(o => GetColumn(o))
                .Where(o => o != null)
                .ToList();
        }

        public IEnumerable<Column> GetEditorColumns()
        {
            return EditorColumns
                .Select(o => GetColumn(o))
                .Where(o => o != null)
                .ToList();
        }

        public IEnumerable<Column> GetTitleColumns()
        {
            return TitleColumns
                .Select(o => GetColumn(o))
                .Where(o => o != null)
                .ToList();
        }

        public IEnumerable<Column> GetLinkColumns()
        {
            return LinkColumns
                .Select(o => GetColumn(o))
                .Where(o => o != null)
                .ToList();
        }

        public IEnumerable<Column> GetHistoryColumns()
        {
            return HistoryColumns
                .Select(o => GetColumn(o))
                .Where(o => o != null)
                .ToList();
        }

        public IEnumerable<Column> SelectColumns()
        {
            return Columns.Where(o =>
                !o.Nullable.ToBool() ||
                EditorColumns.Contains(o.ColumnName) ||
                EditorColumns.Contains(o.ColumnName));
        }

        public Dictionary<string, ControlData> GridSelectableOptions(bool enabled = true)
        {
            return enabled
                ? ColumnUtilities.SelectableOptions(this, GridColumns)
                : ColumnUtilities.SelectableOptions(
                    this, ColumnUtilities.GridDefinitions(ReferenceType)
                        .Where(o => !GridColumns.Contains(o.ColumnName))
                        .Select(o => o.ColumnName));
        }

        public Dictionary<string, ControlData> FilterSelectableOptions(bool enabled = true)
        {
            return enabled
                ? ColumnUtilities.SelectableOptions(this, FilterColumns)
                : ColumnUtilities.SelectableOptions(
                    this, ColumnUtilities.FilterDefinitions(ReferenceType)
                        .Where(o => !FilterColumns.Contains(o.ColumnName))
                        .Select(o => o.ColumnName));
        }

        public Dictionary<string, ControlData> EditorSelectableOptions(bool enabled = true)
        {
            return enabled
                ? ColumnUtilities.SelectableOptions(this, EditorColumns)
                : ColumnUtilities.SelectableOptions(
                    this, ColumnUtilities.EditorDefinitions(ReferenceType)
                        .Where(o => !EditorColumns.Contains(o.ColumnName))
                        .Select(o => o.ColumnName));
        }

        public Dictionary<string, ControlData> TitleSelectableOptions(
            IEnumerable<string> titleColumns, bool enabled = true)
        {
            return enabled
                ? ColumnUtilities.SelectableOptions(this, titleColumns)
                : ColumnUtilities.SelectableOptions(
                    this, ColumnUtilities.TitleDefinitions(ReferenceType)
                        .Where(o => !titleColumns.Contains(o.ColumnName))
                        .Select(o => o.ColumnName));
        }

        public Dictionary<string, ControlData> LinkSelectableOptions(bool enabled = true)
        {
            return enabled
                ? ColumnUtilities.SelectableOptions(this, LinkColumns)
                : ColumnUtilities.SelectableOptions(
                    this, ColumnUtilities.LinkDefinitions(ReferenceType)
                        .Where(o => !LinkColumns.Contains(o.ColumnName))
                        .Select(o => o.ColumnName));
        }

        public Dictionary<string, ControlData> HistorySelectableOptions(bool enabled = true)
        {
            return enabled
                ? ColumnUtilities.SelectableOptions(this, HistoryColumns)
                : ColumnUtilities.SelectableOptions(
                    this, ColumnUtilities.HistoryDefinitions(ReferenceType)
                        .Where(o => !HistoryColumns.Contains(o.ColumnName))
                        .Select(o => o.ColumnName));
        }

        public Dictionary<string, ControlData> FormulaTargetSelectableOptions()
        {
            return Columns
                .Where(o => o.Computable)
                .Where(o => !o.NotUpdate)
                .Where(o => o.TypeName != "datetime")
                .ToDictionary(o => o.ColumnName, o => new ControlData(o.LabelText));
        }

        public Dictionary<string, ControlData> ViewSelectableOptions()
        {
            return Views != null
                ? Views.ToDictionary(o => o.Id.ToString(), o => new ControlData(o.Name))
                : null;
        }

        public Dictionary<string, ControlData> MonitorChangesSelectableOptions(
            IEnumerable<string> monitorChangesColumns, bool enabled = true)
        {
            return enabled
                ? ColumnUtilities.SelectableOptions(this, monitorChangesColumns)
                : ColumnUtilities.SelectableOptions(
                    this, ColumnUtilities.MonitorChangesDefinitions(ReferenceType)
                        .Where(o => !monitorChangesColumns.Contains(o.ColumnName))
                        .Select(o => o.ColumnName));
        }

        public Dictionary<string, ControlData> AggregationDestination()
        {
            return Aggregations?.ToDictionary(
                o => o.Id.ToString(),
                o => new ControlData((o.GroupBy == "[NotGroupBy]"
                    ? Displays.NoClassification()
                    : GetColumn(o.GroupBy)?.LabelText) +
                        " (" + Displays.Get(o.Type.ToString()) +
                            (o.Target != string.Empty
                                ? ": " + GetColumn(o.Target)?.LabelText
                                : string.Empty) + ")"));
        }

        public Dictionary<string, ControlData> AggregationSource()
        {
            var aggregationSource = new Dictionary<string, ControlData>
            {
                { "[NotGroupBy]", new ControlData(Displays.NoClassification()) }
            };
            return aggregationSource.AddRange(Def.ColumnDefinitionCollection
                .Where(o => o.TableName == ReferenceType)
                .Where(o => o.Aggregatable)
                .ToDictionary(
                    o => o.ColumnName,
                    o => new ControlData(GetColumn(o.ColumnName).LabelText)));
        }

        public Dictionary<string, string> GanttGroupByOptions()
        {
            return Columns
                .Where(o => o.HasChoices())
                .OrderBy(o => o.No)
                .ToDictionary(o => o.ColumnName, o => o.GridLabelText);
        }

        public Dictionary<string, string> TimeSeriesGroupByOptions()
        {
            return Columns
                .Where(o => o.HasChoices())
                .OrderBy(o => o.No)
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
            return Columns
                .Where(o => o.Computable)
                .Where(o => o.TypeName != "datetime")
                .OrderBy(o => o.No)
                .ToDictionary(o => o.ColumnName, o => o.LabelText);
        }

        public Dictionary<string, string> KambanGroupByOptions()
        {
            return Columns
                .Where(o => o.HasChoices())
                .OrderBy(o => o.No)
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

        public Dictionary<string, string> KambanValueOptions()
        {
            return Columns
                .Where(o => o.Computable)
                .Where(o => o.TypeName != "datetime")
                .OrderBy(o => o.No)
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
                case "TitleSeparator": TitleSeparator = value; break;
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

        private void SetLinks(Column column)
        {
            column.Link = false;
            Links.RemoveAll(o => o.ColumnName == column.ColumnName);
            column.ChoicesText.SplitReturn()
                .Select(o => o.Trim())
                .Where(o => o.RegexExists(@"^\[\[[0-9]*\]\]$"))
                .Select(o => o.RegexFirst("[0-9]+").ToLong())
                .ForEach(siteId =>
                {
                    column.Link = true;
                    if (!Links.Any(o => o.ColumnName == column.ColumnName && o.SiteId == siteId))
                    {
                        if (new SiteModel(siteId).AccessStatus ==
                            Databases.AccessStatuses.Selected)
                        {
                            Links.Add(new Link(column.ColumnName, siteId));
                        }
                    }
                });
        }

        public void SetChoiceHash(bool withLink = true, bool all = false)
        {
            SetChoiceHash(
                columnName: null,
                searchIndexes: null,
                linkHash: withLink
                    ? LinkHash(all: all)
                    : null);
        }

        public void SetChoiceHash(
            string columnName,
            IEnumerable<string> searchIndexes = null,
            IEnumerable<long> selectedValues = null)
        {
            SetChoiceHash(
                columnName: columnName,
                searchIndexes: searchIndexes,
                linkHash: LinkHash(columnName, searchIndexes, selectedValues));
        }

        private void SetChoiceHash(
            string columnName,
            IEnumerable<string> searchIndexes,
            Dictionary<string, IEnumerable<string>> linkHash)
        {
            Columns?
                .Where(o => o.HasChoices())
                .Where(o => columnName == null || o.ColumnName == columnName)
                .ForEach(column =>
                    column.SetChoiceHash(InheritPermission, linkHash, searchIndexes));
        }

        private Dictionary<string, IEnumerable<string>> LinkHash(bool all)
        {
            var allowSites = Permissions.AllowSites(Links?.Select(o => o.SiteId).Distinct());
            var targetSites = Links?
                .Where(o => all || GetColumn(o.ColumnName)?.UseSearch != true)
                .Select(o => o.SiteId)
                .Distinct();
            var dataRows = Rds.ExecuteTable(
                statements: Rds.SelectItems(
                    column: Rds.ItemsColumn()
                        .ReferenceId()
                        .ReferenceType()
                        .SiteId()
                        .Title(),
                    where: Rds.ItemsWhere()
                        .ReferenceType("Sites", _operator: "<>")
                        .SiteId_In(allowSites)
                        .Or(Rds.ItemsWhere()
                            .ReferenceType("Wikis")
                            .SiteId_In(targetSites, _using: targetSites.Any())),
                    orderBy: Rds.ItemsOrderBy()
                        .Title())).AsEnumerable();
            return allowSites
                .Distinct()
                .ToDictionary(
                    siteId => "[[" + siteId + "]]",
                    siteId => LinkValue(
                        siteId, dataRows.Where(o => o["SiteId"].ToLong() == siteId)));
        }

        private Dictionary<string, IEnumerable<string>> LinkHash(
            string columnName, IEnumerable<string> searchIndexes, IEnumerable<long> selectedValues)
        {
            var hash = new Dictionary<string, IEnumerable<string>>();
            var allowSites = Permissions.AllowSites(Links?.Select(o => o.SiteId));
            Links?
                .Where(o => o.ColumnName == columnName)
                .Where(o => GetColumn(o.ColumnName)?.UseSearch == true)
                .GroupBy(o => o.SiteId)
                .Select(o => o.FirstOrDefault())
                .ForEach(link =>
                {
                    var results = searchIndexes?.Any() == true
                        ? SearchIndexUtilities.Get(
                            searchIndexes: searchIndexes,
                            column: Rds.SearchIndexesColumn().ReferenceId(),
                            siteId: link.SiteId,
                            pageSize: Parameters.General.DropDownSearchLimit)
                                .Tables[0]
                                .AsEnumerable()
                                .Select(o => o["ReferenceId"].ToLong())
                        : null;
                    var dataRows = Rds.ExecuteTable(statements:
                        Rds.SelectItems(
                            column: Rds.ItemsColumn()
                                .ReferenceId()
                                .ReferenceType()
                                .SiteId()
                                .Title(),
                            where: Rds.ItemsWhere()
                                .ReferenceId_In(results, _using:
                                    results?.Any() == true ||
                                    searchIndexes?.Any() == true)
                                .Or(Rds.ItemsWhere()
                                        .ReferenceType("Wikis")
                                        .ReferenceId_In(selectedValues),
                                    _using: selectedValues?.Any() == true)
                                .ReferenceType("Sites", _operator: "<>")
                                .SiteId(link.SiteId)))
                                    .AsEnumerable();
                    if (dataRows != null && allowSites?.Contains(link.SiteId) == true)
                    {
                        hash.Add("[[" + link.SiteId + "]]", LinkValue(link.SiteId, dataRows));
                    }
                });
            return hash;
        }

        private static IEnumerable<string> LinkValue(
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
                                .Where(o => o.Trim() != string.Empty)
                                .GroupBy(o => o.Split_1st())
                                .Select(o => o.First())
                    : dataRows
                        .Where(p => p["SiteId"].ToLong() == siteId)
                        .Select(p => p["ReferenceId"].ToString() + "," + p["Title"].ToString());
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
            view.Id = ViewLatestId;
            if (Views == null) Views = new List<View>();
            Views.Add(view);
        }

        public void EnableNotifications(bool before, DataSet dataSet)
        {
            Notifications
                .Select((o, i) => new
                {
                    Notification = o,
                    Exists = dataSet.Tables[i].Rows.Count == 1
                })
                .ForEach(o =>
                {
                    if (before)
                    {
                        o.Notification.Enabled = o.Exists;
                    }
                    else if (Views?.Get(o.Notification.AfterCondition) != null)
                    {
                        if (Views?.Get(o.Notification.BeforeCondition) == null)
                        {
                            o.Notification.Enabled = o.Exists;
                        }
                        else if (o.Notification.Expression == Notification.Expressions.And)
                        {
                            o.Notification.Enabled &= o.Exists;
                        }
                        else
                        {
                            o.Notification.Enabled |= o.Exists;
                        }
                    }
                });
        }
    }
}