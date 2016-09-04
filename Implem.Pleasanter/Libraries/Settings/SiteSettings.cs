using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Models;
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
        public int? FirstDayOfWeek;
        public int? FirstMonth;
        public List<string> GridColumnsOrder;
        public List<string> FilterColumnsOrder;
        public List<string> EditorColumnsOrder;
        public List<string> TitleColumnsOrder;
        public List<string> LinkColumnsOrder;
        public List<string> HistoryColumnsOrder;
        public List<Column> ColumnCollection;
        public List<Aggregation> AggregationCollection;
        public Dictionary<string, long> LinkColumnSiteIdHash;
        public List<Summary> SummaryCollection;
        public Dictionary<string, Formula> FormulaHash;
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
            if (AggregationCollection == null) AggregationCollection = new List<Aggregation>();
            if (LinkColumnSiteIdHash == null) LinkColumnSiteIdHash = new Dictionary<string, long>();
            if (SummaryCollection == null) SummaryCollection = new List<Summary>();
            if (FormulaHash == null) FormulaHash = new Dictionary<string, Formula>();
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
            if (self.FirstDayOfWeek == def.FirstDayOfWeek) self.FirstDayOfWeek = null;
            if (self.FirstMonth == def.FirstMonth) self.FirstMonth = null;
            if (self.GridColumnsOrder.SequenceEqual(def.GridColumnsOrder)) self.GridColumnsOrder = null;
            if (self.FilterColumnsOrder.SequenceEqual(def.FilterColumnsOrder)) self.FilterColumnsOrder = null;
            if (self.EditorColumnsOrder.SequenceEqual(def.EditorColumnsOrder)) self.EditorColumnsOrder = null;
            if (self.TitleColumnsOrder.SequenceEqual(def.TitleColumnsOrder)) self.TitleColumnsOrder = null;
            if (self.TitleSeparator == def.TitleSeparator) self.TitleSeparator = null;
            if (self.LinkColumnsOrder.SequenceEqual(def.LinkColumnsOrder)) self.LinkColumnsOrder = null;
            if (self.HistoryColumnsOrder.SequenceEqual(def.HistoryColumnsOrder)) self.HistoryColumnsOrder = null;
            if (self.ColumnCollection.SequenceEqual(def.ColumnCollection)) self.ColumnCollection = null;
            if (self.AggregationCollection.SequenceEqual(def.AggregationCollection)) self.AggregationCollection = null;
            if (self.LinkColumnSiteIdHash.SequenceEqual(def.LinkColumnSiteIdHash)) self.LinkColumnSiteIdHash = null;
            if (self.SummaryCollection.SequenceEqual(def.SummaryCollection)) self.SummaryCollection = null;
            if (self.FormulaHash.SequenceEqual(def.FormulaHash)) self.FormulaHash = null;
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
                    if (column.LabelText == Displays.Get(columnDefinition.Id)) column.LabelText = null;
                    if (column.ChoicesText == columnDefinition.ChoicesText) column.ChoicesText = null;
                    if (column.DefaultInput == columnDefinition.DefaultInput) column.DefaultInput = null;
                    if (column.GridFormat == columnDefinition.GridFormat) column.GridFormat = null;
                    if (column.ControlFormat == columnDefinition.ControlFormat) column.ControlFormat = null;
                    if (column.ControlType == columnDefinition.ControlType) column.ControlType = null;
                    if (column.Format?.Trim() == string.Empty) column.Format = null;
                    if (column.DecimalPlaces == columnDefinition.DecimalPlaces) column.DecimalPlaces = null;
                    if (column.Min == columnDefinition.Min) column.Min = null;
                    if (column.Max == DefaultMax(columnDefinition)) column.Max = null;
                    if (column.Step == DefaultStep(columnDefinition)) column.Step = null;
                    if (column.EditorReadOnly == false) column.EditorReadOnly = null;
                    if (column.FieldCss == columnDefinition.FieldCss) column.FieldCss = null;
                    if (column.Unit == columnDefinition.Unit) column.Unit = null;
                    if (column.Link == false) column.Link = null;
                }
            });
            self.ColumnCollection.RemoveAll(o => removeCollection.Contains(o.ColumnName));
            return self.ToJson();
        }

        private void UpdateGridColumnsOrder()
        {
            if (GridColumnsOrder == null)
            {
                GridColumnsOrder = GridColumnDefinitions(visibleOnly: true)
                    .Select(o => o.ColumnName)
                    .ToList();
            }
            else
            {
                GridColumnsOrder.RemoveAll(o =>
                    !Def.ColumnDefinitionCollection.Any(p =>
                        p.ColumnName == o &&
                        p.TableName == ReferenceType &&
                        p.GridColumn > 0));
            }
        }

        private IEnumerable<ColumnDefinition> GridColumnDefinitions(bool visibleOnly = false)
        {
            return Def.ColumnDefinitionCollection
                .Where(o => o.TableName == ReferenceType)
                .Where(o => o.GridColumn > 0)
                .Where(o => o.GridVisible || !visibleOnly)
                .OrderBy(o => o.GridColumn);
        }

        private void UpdateFilterColumnsOrder()
        {
            if (FilterColumnsOrder == null)
            {
                FilterColumnsOrder = FilterColumnDefinitions(visibleOnly: true)
                    .Select(o => o.ColumnName)
                    .ToList();
            }
            else
            {
                FilterColumnsOrder.RemoveAll(o =>
                    !Def.ColumnDefinitionCollection.Any(p =>
                        p.ColumnName == o &&
                        p.TableName == ReferenceType &&
                        p.FilterColumn > 0));
            }
        }

        private IEnumerable<ColumnDefinition> FilterColumnDefinitions(bool visibleOnly = false)
        {
            return Def.ColumnDefinitionCollection
                .Where(o => o.TableName == ReferenceType)
                .Where(o => o.FilterColumn > 0)
                .Where(o => o.FilterVisible || !visibleOnly)
                .OrderBy(o => o.FilterColumn);
        }

        private void UpdateEditorColumnsOrder()
        {
            if (EditorColumnsOrder == null)
            {
                EditorColumnsOrder = EditorColumnDefinitions(visibleOnly: true)
                    .Select(o => o.ColumnName)
                    .ToList();
            }
            else
            {
                EditorColumnsOrder.RemoveAll(o =>
                    !Def.ColumnDefinitionCollection.Any(p =>
                        p.ColumnName == o &&
                        p.TableName == ReferenceType &&
                        p.EditorColumn &&
                        !p.NotEditorSettings));
            }
        }

        private IEnumerable<ColumnDefinition> EditorColumnDefinitions(bool visibleOnly = false)
        {
            return Def.ColumnDefinitionCollection
                .Where(o => o.TableName == ReferenceType)
                .Where(o => o.EditorColumn)
                .Where(o => o.EditorVisible || !visibleOnly)
                .Where(o => !o.NotEditorSettings)
                .OrderBy(o => o.No);
        }

        private void UpdateTitleColumnsOrder()
        {
            if (TitleColumnsOrder == null)
            {
                TitleColumnsOrder = TitleColumnDefinitions(visibleOnly: true)
                    .Select(o => o.ColumnName)
                    .ToList();
            }
            else
            {
                TitleColumnsOrder.RemoveAll(o =>
                    !Def.ColumnDefinitionCollection.Any(p =>
                        p.TableName == ReferenceType &&
                        p.ColumnName == o &&
                        p.TitleColumn > 0 &&
                        !p.NotEditorSettings));
            }
        }

        private IEnumerable<ColumnDefinition> TitleColumnDefinitions(bool visibleOnly = false)
        {
            return Def.ColumnDefinitionCollection
                .Where(o => o.TableName == ReferenceType)
                .Where(o => o.TitleColumn > 0)
                .Where(o => o.ColumnName == "Title" || !visibleOnly)
                .OrderBy(o => o.TitleColumn);
        }

        private void UpdateLinkColumnsOrder()
        {
            if (LinkColumnsOrder == null)
            {
                LinkColumnsOrder = LinkColumnDefinitions(visibleOnly: true)
                    .Select(o => o.ColumnName)
                    .ToList();
            }
            else
            {
                LinkColumnsOrder.RemoveAll(o =>
                    !Def.ColumnDefinitionCollection.Any(p =>
                        p.TableName == ReferenceType &&
                        p.ColumnName == o &&
                        p.LinkColumn > 0));
            }
        }

        private IEnumerable<ColumnDefinition> LinkColumnDefinitions(bool visibleOnly = false)
        {
            return Def.ColumnDefinitionCollection
                .Where(o => o.TableName == ReferenceType)
                .Where(o => o.LinkColumn > 0)
                .Where(o => o.LinkVisible || !visibleOnly)
                .OrderBy(o => o.LinkColumn);
        }

        private void UpdateHistoryColumnsOrder()
        {
            if (HistoryColumnsOrder == null)
            {
                HistoryColumnsOrder = HistoryColumnDefinitions(visibleOnly: true)
                    .Select(o => o.ColumnName)
                    .ToList();
            }
            else
            {
                HistoryColumnsOrder.RemoveAll(o =>
                    !Def.ColumnDefinitionCollection.Any(p =>
                        p.TableName == ReferenceType &&
                        p.ColumnName == o &&
                        p.HistoryColumn > 0));
            }
        }

        private IEnumerable<ColumnDefinition> HistoryColumnDefinitions(bool visibleOnly = false)
        {
            return Def.ColumnDefinitionCollection
                .Where(o => o.TableName == ReferenceType)
                .Where(o => o.HistoryColumn > 0)
                .Where(o => o.HistoryVisible || !visibleOnly)
                .OrderBy(o => o.HistoryColumn);
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
                column.ChoicesText = column.ChoicesText ?? columnDefinition.ChoicesText;
                column.DefaultInput = column.DefaultInput ?? columnDefinition.DefaultInput;
                column.GridFormat = column.GridFormat ?? columnDefinition.GridFormat;
                column.ControlFormat = column.ControlFormat ?? columnDefinition.ControlFormat;
                column.ControlType = column.ControlType ?? columnDefinition.ControlType;
                column.DecimalPlaces = column.DecimalPlaces ?? columnDefinition.DecimalPlaces;
                column.Min = column.Min ?? columnDefinition.Min;
                column.Max = column.Max ?? DefaultMax(columnDefinition);
                column.Step = column.Step ?? DefaultStep(columnDefinition);
                column.EditorReadOnly = column.EditorReadOnly ?? false;
                column.FieldCss = column.FieldCss ?? columnDefinition.FieldCss;
                column.Unit = column.Unit ?? columnDefinition.Unit;
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
                column.Validators = columnDefinition.Validators;
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

        public IEnumerable<Column> GridColumnCollection(bool withTitle = false)
        {
            foreach (var columnName in GridColumnsOrder)
            {
                var column = GridColumn(columnName);
                if (column != null &&
                    (!withTitle || withTitle && GridColumnsOrder.Contains(columnName)))
                {
                    yield return column; 
                }
            }
        }

        public IEnumerable<Column> FilterColumnCollection()
        {
            foreach (var columnName in FilterColumnsOrder)
            {
                var column = FilterColumn(columnName);
                if (column != null)
                {
                    yield return column;
                }
            }
        }

        public IEnumerable<Column> EditorColumnCollection()
        {
            foreach (var columnName in EditorColumnsOrder)
            {
                var column = EditorColumn(columnName);
                if (column != null)
                {
                    yield return column;
                }
            }
        }

        public IEnumerable<Column> TitleColumnCollection()
        {
            foreach (var columnName in TitleColumnsOrder)
            {
                var column = TitleColumn(columnName);
                if (column != null)
                {
                    yield return column;
                }
            }
        }

        public IEnumerable<Column> LinkColumnCollection()
        {
            foreach (var columnName in LinkColumnsOrder)
            {
                var column = LinkColumn(columnName);
                if (column != null)
                {
                    yield return column;
                }
            }
        }

        public IEnumerable<Column> HistoryColumnCollection()
        {
            foreach (var columnName in HistoryColumnsOrder)
            {
                var column = HistoryColumn(columnName);
                if (column != null)
                {
                    yield return column;
                }
            }
        }

        public IEnumerable<Column> SelectColumnCollection()
        {
            return ColumnCollection.Where(o =>
                !o.Nullable.ToBool() ||
                EditorColumnsOrder.Contains(o.ColumnName) ||
                EditorColumnsOrder.Contains(o.ColumnName));
        }

        public Dictionary<string, string> GridSelectableItems(bool visible = true)
        {
            return visible
                ? SelectableItems(GridColumnsOrder, visible)
                : SelectableItems(GridColumnDefinitions()
                    .Where(o => !GridColumnsOrder.Contains(o.ColumnName))
                    .Select(o => o.ColumnName), visible);
        }

        public Dictionary<string, string> FilterSelectableItems(bool visible = true)
        {
            return visible
                ? SelectableItems(FilterColumnsOrder, visible)
                : SelectableItems(FilterColumnDefinitions()
                    .Where(o => !FilterColumnsOrder.Contains(o.ColumnName))
                    .Select(o => o.ColumnName), visible);
        }

        public Dictionary<string, string> EditorSelectableItems(bool visible = true)
        {
            return visible
                ? SelectableItems(EditorColumnsOrder, visible)
                : SelectableItems(EditorColumnDefinitions()
                    .Where(o => !EditorColumnsOrder.Contains(o.ColumnName))
                    .Select(o => o.ColumnName), visible);
        }

        public Dictionary<string, string> TitleSelectableItems(bool visible = true)
        {
            return visible
                ? SelectableItems(TitleColumnsOrder, visible)
                : SelectableItems(TitleColumnDefinitions()
                    .Where(o => !TitleColumnsOrder.Contains(o.ColumnName))
                    .Select(o => o.ColumnName), visible);
        }

        public Dictionary<string, string> LinkSelectableItems(bool visible = true)
        {
            return visible
                ? SelectableItems(LinkColumnsOrder, visible)
                : SelectableItems(LinkColumnDefinitions()
                    .Where(o => !LinkColumnsOrder.Contains(o.ColumnName))
                    .Select(o => o.ColumnName), visible);
        }

        public Dictionary<string, string> HistorySelectableItems(bool visible = true)
        {
            return visible
                ? SelectableItems(HistoryColumnsOrder, visible)
                : SelectableItems(HistoryColumnDefinitions()
                    .Where(o => !HistoryColumnsOrder.Contains(o.ColumnName))
                    .Select(o => o.ColumnName), visible);
        }

        private Dictionary<string, string> SelectableItems(IEnumerable<string> columns, bool visible)
        {
            return columns.ToDictionary(
                o => o,
                o => visible
                    ? Displays.Get(GetColumn(o).LabelText)
                    : Displays.Get(GetColumn(o).LabelText) + " (" + Displays.Hidden() + ")");
        }

        public Dictionary<string, string> AggregationDestination()
        {
            return AggregationCollection?.ToDictionary(
                o => o.Id.ToString(),
                o => (o.GroupBy == "[NotGroupBy]"
                    ? Displays.SettingNotGroupBy() 
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
                { "[NotGroupBy]", Displays.SettingNotGroupBy() }
            };
            return aggregationSource.AddRange(Def.ColumnDefinitionCollection
                .Where(o => o.TableName == ReferenceType)
                .Where(o => o.Aggregatable)
                .ToDictionary(
                    o => o.ColumnName,
                    o => GetColumn(o.ColumnName).LabelText));
        }

        public int NextPageOffset(int gridOffset, int resultCount)
        {
            return (resultCount > 0 && GridPageSize == resultCount
                ? gridOffset + GridPageSize
                : -1).ToInt();
        }

        public void Set(string propertyName, string value)
        {
            switch (propertyName)
            {
                case "NearCompletionTimeBeforeDays": NearCompletionTimeBeforeDays = value.ToInt(); break;
                case "NearCompletionTimeAfterDays": NearCompletionTimeAfterDays = value.ToInt(); break;
                case "GridPageSize": GridPageSize = value.ToInt(); break;
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
            ResponseCollection responseCollection,
            string controlId,
            IEnumerable<string> selectedDestination,
            IEnumerable<string> selectedSource)
        {
            switch (controlId)
            {
                case "AddAggregations":
                    var idCollection = new List<string>();
                    selectedSource.ForEach(groupBy =>
                    {
                        var id = AggregationCollection.Count > 0
                            ? AggregationCollection.Max(o => o.Id) + 1
                            : 1;
                        idCollection.Add(id.ToString());
                        AggregationCollection.Add(new Aggregation(id, groupBy));
                    });
                    selectedDestination = idCollection;
                    selectedSource = null;
                    break;
                case "DeleteAggregations":
                    AggregationCollection
                        .RemoveAll(o => selectedDestination.Contains(o.Id.ToString()));
                    selectedSource = selectedDestination;
                    selectedDestination = null;
                    break;
                case "MoveUpAggregations":
                case "MoveDownAggregations":
                    var order = AggregationCollection.Select(o => o.Id.ToString()).ToArray();
                    if (controlId == "MoveDownAggregations") Array.Reverse(order);
                    order.Select((o, i) => new { Id = o, Index = i }).ForEach(data =>
                    {
                        if (selectedDestination.Contains(data.Id) &&
                            data.Index > 0 &&
                            !selectedDestination.Contains(order[data.Index - 1]))
                        {
                            order = Arrays.Swap(order, data.Index, data.Index - 1);
                        }
                    });
                    if (controlId == "MoveDownAggregations") Array.Reverse(order);
                    AggregationCollection = order.ToList().Select(id => AggregationCollection
                        .FirstOrDefault(o => o.Id.ToString() == id)).ToList();
                    break;
                case "SetAggregationDetails":
                    Aggregation.Types type;
                    Enum.TryParse<Aggregation.Types>(
                        Forms.Data("AggregationType"), out type);
                    var aggregationTarget = type != Aggregation.Types.Count
                        ? Forms.Data("AggregationTarget")
                        : string.Empty;
                    AggregationCollection
                        .Where(o => selectedDestination.Contains(o.Id.ToString()))
                        .ForEach(aggregation =>
                        {
                            aggregation.Type = type;
                            aggregation.Target = aggregationTarget;
                        });
                    break;
            }
            responseCollection
                .Html("#AggregationDestination", new HtmlBuilder()
                    .SelectableItems(
                        listItemCollection: AggregationDestination(),
                        selectedValueTextCollection: selectedDestination))
                .SetFormData("AggregationDestination", selectedDestination?.Join(";"));
        }

        public void SetGridColumns(
            ResponseCollection responseCollection,
            string controlId,
            List<string> selectedColumns,
            List<string> selectedSourceColumns)
        {
            if (controlId.EndsWith("GridColumns"))
            {
                var command = ChangeCommand(controlId);
                GridColumnsOrder = ChangedColumns(
                    GridColumnsOrder, command, selectedColumns, selectedSourceColumns);
                SetResponseAfterChangeColumns(
                    responseCollection,
                    command,
                    "Grid",
                    GridSelectableItems(),
                    selectedColumns,
                    GridSelectableItems(visible: false),
                    selectedSourceColumns);
            }
        }

        public void SetFilterColumns(
            ResponseCollection responseCollection,
            string controlId,
            List<string> selectedColumns,
            List<string> selectedSourceColumns)
        {
            if (controlId.EndsWith("FilterColumns"))
            {
                var command = ChangeCommand(controlId);
                FilterColumnsOrder = ChangedColumns(
                    FilterColumnsOrder, command, selectedColumns, selectedSourceColumns);
                SetResponseAfterChangeColumns(
                    responseCollection,
                    command,
                    "Filter",
                    FilterSelectableItems(),
                    selectedColumns,
                    FilterSelectableItems(visible: false),
                    selectedSourceColumns);
            }
        }

        public void SetEditorColumns(
            ResponseCollection responseCollection,
            string controlId,
            List<string> selectedColumns,
            List<string> selectedSourceColumns)
        {
            if (controlId.EndsWith("EditorColumns"))
            {
                var command = ChangeCommand(controlId);
                EditorColumnsOrder = ChangedColumns(
                    EditorColumnsOrder, command, selectedColumns, selectedSourceColumns);
                SetResponseAfterChangeColumns(
                    responseCollection,
                    command,
                    "Editor",
                    EditorSelectableItems(),
                    selectedColumns,
                    EditorSelectableItems(visible: false),
                    selectedSourceColumns);
            }
        }

        public void SetTitleColumns(
            ResponseCollection responseCollection,
            string controlId,
            List<string> selectedColumns,
            List<string> selectedSourceColumns)
        {
            if (controlId.EndsWith("TitleColumns"))
            {
                var command = ChangeCommand(controlId);
                TitleColumnsOrder = ChangedColumns(
                    TitleColumnsOrder, command, selectedColumns, selectedSourceColumns);
                SetResponseAfterChangeColumns(
                    responseCollection,
                    command,
                    "Title",
                    TitleSelectableItems(),
                    selectedColumns,
                    TitleSelectableItems(visible: false),
                    selectedSourceColumns);
            }
        }

        public void SetLinkColumns(
            ResponseCollection responseCollection,
            string controlId,
            List<string> selectedColumns,
            List<string> selectedSourceColumns)
        {
            if (controlId.EndsWith("LinkColumns"))
            {
                var command = ChangeCommand(controlId);
                LinkColumnsOrder = ChangedColumns(
                    LinkColumnsOrder, command, selectedColumns, selectedSourceColumns);
                SetResponseAfterChangeColumns(
                    responseCollection,
                    command,
                    "Link",
                    LinkSelectableItems(),
                    selectedColumns,
                    LinkSelectableItems(visible: false),
                    selectedSourceColumns);
            }
        }

        public void SetHistoryColumns(
            ResponseCollection responseCollection,
            string controlId,
            List<string> selectedColumns,
            List<string> selectedSourceColumns)
        {
            if (controlId.EndsWith("HistoryColumns"))
            {
                var command = ChangeCommand(controlId);
                HistoryColumnsOrder = ChangedColumns(
                    HistoryColumnsOrder, command, selectedColumns, selectedSourceColumns);
                SetResponseAfterChangeColumns(
                    responseCollection,
                    command,
                    "History",
                    HistorySelectableItems(),
                    selectedColumns,
                    HistorySelectableItems(visible: false),
                    selectedSourceColumns);
            }
        }

        public string ChangeCommand(string controlId)
        {
            if (controlId.StartsWith("MoveUp")) return "MoveUp";
            if (controlId.StartsWith("MoveDown")) return "MoveDown";
            if (controlId.StartsWith("Hide")) return "Hide";
            if (controlId.StartsWith("Show")) return "Show";
            return null;
        }

        public List<string> ChangedColumns(
            List<string> order,
            string command,
            List<string> selectedColumns,
            List<string> selectedSourceColumns)
        {
            switch (command)
            {
                case "MoveUp":
                case "MoveDown":
                    order = SortedColumns(order.ToArray(), command, selectedColumns);
                    break;
                case "Hide":
                    order.RemoveAll(o => selectedColumns.Contains(o));
                    break;
                case "Show":
                    order.AddRange(selectedSourceColumns);
                    break;
            }
            return order;
        }

        private static List<string> SortedColumns(
            string[] order, string command, List<string> selectedColumns)
        {
            if (command == "MoveDown") Array.Reverse(order);
            order.Select((o, i) => new { ColumnName = o, Index = i }).ForEach(data =>
            {
                if (selectedColumns.Contains(data.ColumnName) &&
                    data.Index > 0 &&
                    !selectedColumns.Contains(order[data.Index - 1]))
                {
                    order = Arrays.Swap(order, data.Index, data.Index - 1);
                }
            });
            if (command == "MoveDown") Array.Reverse(order);
            return order.ToList();
        }

        private void SetResponseAfterChangeColumns(
            ResponseCollection responseCollection,
            string command,
            string typeName,
            Dictionary<string, string> editorSelectableItems,
            List<string> selectedColumns,
            Dictionary<string, string> editorSourceSelectableItems,
            List<string> selectedSourceColumns)
        {
            switch (command)
            {
                case "Hide":
                    Move(selectedColumns, selectedSourceColumns);
                    break;
                case "Show":
                    Move(selectedSourceColumns, selectedColumns);
                    break;
            }
            responseCollection
                .Html("#" + typeName + "Columns",
                    new HtmlBuilder().SelectableItems(
                        listItemCollection: editorSelectableItems,
                        selectedValueTextCollection: selectedColumns))
                .SetFormData(typeName + "Columns", selectedColumns.Join(";"))
                .Html("#" + typeName + "SourceColumns",
                    new HtmlBuilder().SelectableItems(
                        listItemCollection: editorSourceSelectableItems,
                        selectedValueTextCollection: selectedSourceColumns))
                .SetFormData(typeName + "SourceColumns", selectedSourceColumns.Join(";"));
        }

        private void Move(List<string> sources, List<string> destinations)
        {
            destinations.Clear();
            destinations.AddRange(sources);
            sources.Clear();
        }

        public void SetColumnProperty(Column column, string propertyName, string value)
        {
            switch (propertyName)
            {
                case "ColumnName": column.ColumnName = value; break;
                case "LabelText": column.LabelText = value; break;
                case "ControlType": column.ControlType = value; break;
                case "Format": column.Format = value; break;
                case "DecimalPlaces": column.DecimalPlaces = value.ToInt(); break;
                case "Max": column.Max = value.ToDecimal(); break;
                case "Min": column.Min = value.ToDecimal(); break;
                case "Step": column.Step = value.ToDecimal(); break;
                case "EditorReadOnly": column.EditorReadOnly = value.ToBool(); break;
                case "FieldCss": column.FieldCss = value; break;
                case "ChoicesText": SetChoices(column, value); break;
                case "DefaultInput": column.DefaultInput = value; break;
                case "GridFormat": column.GridFormat = value; break;
                case "ControlFormat": column.ControlFormat = value; break;
                case "Unit": column.Unit = value; break;
            }
        }

        public void AddSummary(
            ResponseCollection responseCollection,
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
            }
            else
            {
                responseCollection.Message(Messages.AlreadyAdded());
            }
        }

        public void DeleteSummary(ResponseCollection responseCollection, long id)
        {
            SummaryCollection.Remove(SummaryCollection.FirstOrDefault(o => o.Id == id));
        }

        private void SetChoices(Column column, string value)
        {
            column.ChoicesText = value;
            column.Link = false;
            LinkColumnSiteIdHash.RemoveAll((key, o) =>
                key.StartsWith(column.ColumnName + "_"));
            value.SplitReturn()
                .Select(o => o.Trim())
                .Where(o => o.RegexExists(@"^\[\[[0-9]*\]\]$"))
                .Select(o => o.RegexFirst("[0-9]+").ToLong())
                .ForEach(siteId =>
                {
                    column.Link = true;
                    var key = column.ColumnName + "_" + siteId;
                    if (!LinkColumnSiteIdHash.ContainsKey(key))
                    {
                        if (new SiteModel(siteId).AccessStatus ==
                            Databases.AccessStatuses.Selected)
                        {
                            LinkColumnSiteIdHash.Add(key, siteId);
                        }
                    }
                });
        }
        
        public void SetChoicesByLinks()
        {
            if (LinkColumnSiteIdHash?.Count > 0)
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
                                value: LinkColumnSiteIdHash
                                    .Select(o => o.Value)
                                    .Distinct()),
                        orderBy: Rds.ItemsOrderBy()
                            .Title())).AsEnumerable();
                LinkColumnSiteIdHash.ForEach(linkColumnSiteId =>
                    SetChoicesByLinks(linkColumnSiteId, dataRows));
            }
        }

        private void SetChoicesByLinks(
            KeyValuePair<string, long> linkColumnSiteId, EnumerableRowCollection<DataRow> dataRows)
        {
            var columnKey = linkColumnSiteId.Key;
            var columnName = columnKey.Substring(0, columnKey.LastIndexOf('_'));
            var column = GetColumn(columnName);
            column.ChoicesText = column.ChoicesText.SplitReturn()
                .Select(o => o.Trim())
                .Where(o => o != string.Empty)
                .Select(o => LinkedChoices(linkColumnSiteId.Value, dataRows, o))
                .Join("\n");
        }

        private static string LinkedChoices(
            long siteId, EnumerableRowCollection<DataRow> dataRows, string line)
        {
            return line != "[[{0}]]".Params(siteId.ToString())
                ? line
                : dataRows.Any(o =>
                    o["SiteId"].ToLong() == siteId &&
                    o["ReferenceType"].ToString() == "Wikis")
                        ? Rds.ExecuteScalar_string(statements:
                            Rds.SelectWikis(
                                column: Rds.WikisColumn().Body(),
                                where: Rds.WikisWhere().SiteId(siteId))).Trim()
                        : dataRows
                            .Where(p => p["SiteId"].ToLong() == siteId)
                            .Select(p => "{0},{0}: {1}".Params(
                                p["ReferenceId"],
                                p["Title"]))
                            .Join("\n");
        }

        public void SetChoicesByPlaceholders()
        {
            ColumnCollection.Where(o => o.HasChoices()).ForEach(column =>
                column.SetChoicesByPlaceholders(InheritPermission));
        }

        public EnumerableRowCollection<DataRow> SummarySiteDataRows()
        {
            if (LinkColumnSiteIdHash == null) return null;
            var siteIdCollection = LinkColumnSiteIdHash.Values.ToList<long>();
            return Rds.ExecuteTable(statements: Rds.SelectSites(
                column: Rds.SitesColumn()
                    .SiteId()
                    .ReferenceType()
                    .Title()
                    .SiteSettings(),
                where: Rds.SitesWhere()
                    .TenantId(Sessions.TenantId())
                    .SiteId_In(siteIdCollection)))
                        .AsEnumerable();
        }

        public void SetFormulas(
            ResponseCollection responseCollection,
            string controlId,
            IEnumerable<string> selectedColumns)
        {
            var order = FormulaHash.Keys.ToArray();
            switch (controlId)
            {
                case "MoveUpFormulas":
                case "MoveDownFormulas":
                    if (controlId == "MoveDownFormulas") Array.Reverse(order);
                    order.Select((o, i) => new { ColumnName = o, Index = i }).ForEach(data =>
                    {
                        if (selectedColumns.Contains(data.ColumnName))
                        {
                            if (data.Index > 0 &&
                                !selectedColumns.Contains(order[data.Index - 1]))
                            {
                                order = Arrays.Swap(order, data.Index, data.Index - 1);
                            }
                        }
                    });
                    if (controlId == "MoveDownFormulas") Array.Reverse(order);
                    FormulaHash = order
                        .Where(o => FormulaHash.Keys.Contains(o))
                        .ToDictionary(o => o, o => FormulaHash[o]);
                    break;
                case "AddFormula":
                    AddFormula(responseCollection);
                    break;
                case "DeleteFormulas":
                    DeleteFormulas(responseCollection);
                    break;

            }
            responseCollection.Html("#Formulas", new HtmlBuilder()
                .SelectableItems(
                    listItemCollection: FormulaItemCollection(),
                    selectedValueTextCollection: selectedColumns));
        }

        private void AddFormula(ResponseCollection responseCollection)
        {
            var formula = Forms.Data("Formula");
            var parts = Forms.Data("Formula")
                .Replace("(", "( ")
                .Replace(")", " )")
                .Split(' ')
                .Select(o => o.Trim())
                .Where(o => o != string.Empty);
            if (parts.Count() < 3)
            {
                responseCollection.Message(Messages.InvalidFormula());
                return;
            }
            var target = FormulaColumn(parts.Take(1).Last());
            if (target == null)
            {
                responseCollection.Message(Messages.InvalidFormula());
                return;
            }
            if (FormulaHash.Keys.Contains(target.ColumnName))
            {
                responseCollection.Message(Messages.AlreadyAdded());
                return;
            }
            if (parts.Take(2).Last() != "=")
            {
                responseCollection.Message(Messages.InvalidFormula());
                return;
            }
            var stack = new Stack<Formula>();
            var root = new Formula();
            var current = new Formula();
            root.Add(current);
            stack.Push(root);
            parts.Skip(2).ForEach(part =>
            {
                switch (part)
                {
                    case "+":
                        if (!AddFormulaOperator(
                            responseCollection,
                            stack,
                            current,
                            Formula.OperatorTypes.Addition)) return;
                        break;
                    case "-":
                        if (!AddFormulaOperator(
                            responseCollection,
                            stack,
                            current,
                            Formula.OperatorTypes.Subtraction)) return;
                        break;
                    case "*":
                        if (!AddFormulaOperator(
                            responseCollection,
                            stack,
                            current,
                            Formula.OperatorTypes.Multiplication)) return;
                        break;
                    case "/":
                        if (!AddFormulaOperator(
                            responseCollection,
                            stack,
                            current,
                            Formula.OperatorTypes.Division)) return;
                        break;
                    case "(":
                        Formula container;
                        if (stack.First().Children.Last().OperatorType !=
                            Formula.OperatorTypes.NotSet)
                        {
                            container = stack.First().Children.Last();
                        }
                        else
                        {
                            container = new Formula();
                            stack.First().Add(container);
                        }
                        container.Add(new Formula());
                        stack.Push(container);
                        break;
                    case ")":
                        if (stack.Count <= 1)
                        {
                            responseCollection.Message(Messages.InvalidFormula());
                            return;
                        }
                        stack.Pop();
                        break;
                    default:
                        var columnName = FormulaColumn(part)?.ColumnName;
                        if (columnName != null)
                        {
                            stack.First().Children.Last().ColumnName = columnName;
                        }
                        else if (part.RegexExists(@"^[0-9]*\.?[0-9]+$"))
                        {
                            stack.First().Children.Last().RawValue = part.ToDecimal();
                        }
                        else
                        {
                            responseCollection.Message(Messages.InvalidFormula());
                            return;
                        }
                        break;
                }
            });
            if (stack.Count != 1)
            {
                responseCollection.Message(Messages.InvalidFormula());
                return;
            }
            FormulaHash.Add(target.ColumnName, root);
            responseCollection.Val("#Formula", string.Empty);
        }

        private void DeleteFormulas(ResponseCollection responseCollection)
        {
            var selected = Forms.Data("Formulas").Split(';');
            FormulaHash.RemoveAll((key, value) => selected.Contains(key));
        }

        private bool AddFormulaOperator(
            ResponseCollection responseCollection,
            Stack<Formula> stack,
            Formula current,
            Formula.OperatorTypes operatorType)
        {
            if (!stack.First().Children.Last().Completion())
            {
                responseCollection.Message(Messages.InvalidFormula());
                return false;
            }
            else
            {
                current = new Formula(operatorType);
                stack.First().Add(current);
                return true;
            }
        }

        public Column FormulaColumn(string name)
        {
            return ColumnCollection
                .Where(o => o.ColumnName == name || o.LabelText == name)
                .Where(o => o.Computable)
                .Where(o => o.TypeName != "datetime")
                .FirstOrDefault();
        }

        public Dictionary<string, string> FormulaItemCollection()
        {
            return FormulaHash.ToDictionary(
                o => o.Key,
                o =>
                    FormulaColumn(o.Key).LabelText + " = " +
                    o.Value.ToString(this));
        }

        public decimal FormulaResult(string columnName, Dictionary<string, decimal> data)
        {
            return GetColumn(columnName).Round(FormulaHash[columnName].GetResult(data));
        }
    }
}