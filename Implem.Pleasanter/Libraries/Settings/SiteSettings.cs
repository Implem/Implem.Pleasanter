using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.ServerData;
using Implem.Pleasanter.Libraries.Utilities;
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
        public string ReferenceType;
        public decimal? NearCompletionTimeAfterDays;
        public decimal? NearCompletionTimeBeforeDays;
        public int? GridPageSize;
        public List<string> GridColumnsOrder;
        public List<string> EditorColumnsOrder;
        public List<string> TitleColumnsOrder;
        public List<string> LinkColumnsOrder;
        public List<string> HistoryColumnsOrder;
        public List<Column> ColumnCollection;
        public List<Aggregation> AggregationCollection;
        public Dictionary<string, long> LinkColumnSiteIdHash;
        public List<Summary> SummaryCollection;
        public string TitleSeparator = ")";
        public string AddressBook;
        public string MailToDefault;
        public string MailCcDefault;
        public string MailBccDefault;
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
            NearCompletionTimeBeforeDays = NearCompletionTimeBeforeDays ??
                Parameters.General.NearCompletionTimeBeforeDays;
            NearCompletionTimeAfterDays = NearCompletionTimeAfterDays ??
                Parameters.General.NearCompletionTimeAfterDays;
            GridPageSize = GridPageSize ?? Parameters.General.GridPageSize;
            UpdateGridColumnsOrder();
            UpdateEditorColumnsOrder();
            UpdateTitleColumnsOrder();
            UpdateLinkColumnsOrder();
            UpdateHistoryColumnsOrder();
            UpdateColumnCollection();
            if (AggregationCollection == null) AggregationCollection = new List<Aggregation>();
            if (LinkColumnSiteIdHash == null) LinkColumnSiteIdHash = new Dictionary<string, long>();
            if (SummaryCollection == null) SummaryCollection = new List<Summary>();
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext streamingContext)
        {
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
            if (self.GridColumnsOrder.SequenceEqual(def.GridColumnsOrder)) self.GridColumnsOrder = null;
            if (self.EditorColumnsOrder.SequenceEqual(def.EditorColumnsOrder)) self.EditorColumnsOrder = null;
            if (self.TitleColumnsOrder.SequenceEqual(def.TitleColumnsOrder)) self.TitleColumnsOrder = null;
            if (self.TitleSeparator == def.TitleSeparator) self.TitleSeparator = null;
            if (self.LinkColumnsOrder.SequenceEqual(def.LinkColumnsOrder)) self.LinkColumnsOrder = null;
            if (self.HistoryColumnsOrder.SequenceEqual(def.HistoryColumnsOrder)) self.HistoryColumnsOrder = null;
            if (self.ColumnCollection.SequenceEqual(def.ColumnCollection)) self.ColumnCollection = null;
            if (self.AggregationCollection.SequenceEqual(def.AggregationCollection)) self.AggregationCollection = null;
            if (self.LinkColumnSiteIdHash.SequenceEqual(def.LinkColumnSiteIdHash)) self.LinkColumnSiteIdHash = null;
            if (self.SummaryCollection.SequenceEqual(def.SummaryCollection)) self.SummaryCollection = null;
            if (AddressBook == string.Empty) self.AddressBook = null;
            if (MailToDefault == string.Empty) self.MailToDefault = null;
            if (MailCcDefault == string.Empty) self.MailCcDefault = null;
            if (MailBccDefault == string.Empty) self.MailBccDefault = null;
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
                    if (column.GridVisible == columnDefinition.GridVisible) column.GridVisible = null;
                    if (column.GridDateTime == columnDefinition.GridDateTime) column.GridDateTime = null;
                    if (column.ControlDateTime == columnDefinition.ControlDateTime) column.ControlDateTime = null;
                    if (column.ControlType == columnDefinition.ControlType) column.ControlType = null;
                    if (column.DecimalPlaces == columnDefinition.DecimalPlaces) column.DecimalPlaces = null;
                    if (column.Min == columnDefinition.Min) column.Min = null;
                    if (column.Max == DefaultMax(columnDefinition)) column.Max = null;
                    if (column.Step == DefaultStep(columnDefinition)) column.Step = null;
                    if (column.EditorVisible == columnDefinition.EditorVisible) column.EditorVisible = null;
                    if (column.EditorReadOnly == false) column.EditorReadOnly = null;
                    if (column.FieldCss == columnDefinition.FieldCss) column.FieldCss = null;
                    if (column.TitleVisible == DefaultTitleVisible(column)) column.TitleVisible = null;
                    if (column.LinkVisible == columnDefinition.LinkVisible) column.LinkVisible = null;
                    if (column.HistoryVisible == columnDefinition.HistoryVisible) column.HistoryVisible = null;
                    if (column.Unit == columnDefinition.Unit) column.Unit = null;
                    if (column.Link == false) column.Link = null;
                }
            });
            self.ColumnCollection.RemoveAll(o => removeCollection.Contains(o.ColumnName));
            return self.ToJson();
        }

        private void UpdateGridColumnsOrder()
        {
            if (GridColumnsOrder == null) GridColumnsOrder = new List<string>();
            GridColumnsOrder.AddRange(Def.ColumnDefinitionCollection
                .Where(o => !GridColumnsOrder.Any(p => p == o.ColumnName))
                .Where(o => o.TableName == ReferenceType)
                .Where(o => o.GridColumn > 0)
                .OrderBy(o => o.GridColumn)
                .Select(o => o.ColumnName).ToList<string>());
            GridColumnsOrder.RemoveAll(o =>
                !Def.ColumnDefinitionCollection.Any(p =>
                    p.ColumnName == o &&
                    p.TableName == ReferenceType &&
                    p.GridColumn > 0));
        }

        private void UpdateEditorColumnsOrder()
        {
            if (EditorColumnsOrder == null) EditorColumnsOrder = new List<string>();
            EditorColumnsOrder.AddRange(Def.ColumnDefinitionCollection
                .Where(o => !EditorColumnsOrder.Any(p => p == o.ColumnName))
                .Where(o => o.TableName == ReferenceType && o.EditorColumn)
                .Where(o => !o.NotEditorSettings)
                .OrderBy(o => o.No)
                .Select(o => o.ColumnName).ToList<string>());
            EditorColumnsOrder.RemoveAll(o => 
                !Def.ColumnDefinitionCollection.Any(p =>
                    p.ColumnName == o && 
                    p.TableName == ReferenceType &&
                    p.EditorColumn &&
                    !p.NotEditorSettings));
        }

        private void UpdateTitleColumnsOrder()
        {
            if (TitleColumnsOrder == null) TitleColumnsOrder = new List<string>();
            TitleColumnsOrder.AddRange(Def.ColumnDefinitionCollection
                .Where(o => !TitleColumnsOrder.Any(p => p == o.ColumnName))
                .Where(o => o.TableName == ReferenceType)
                .Where(o => o.TitleColumn > 0)
                .OrderBy(o => o.No)
                .Select(o => o.ColumnName).ToList<string>());
            TitleColumnsOrder.RemoveAll(o =>
                !Def.ColumnDefinitionCollection.Any(p =>
                    p.TableName == ReferenceType &&
                    p.ColumnName == o &&
                    p.TitleColumn > 0 &&
                    !p.NotEditorSettings));
        }

        private void UpdateLinkColumnsOrder()
        {
            if (LinkColumnsOrder == null) LinkColumnsOrder = new List<string>();
            LinkColumnsOrder.AddRange(Def.ColumnDefinitionCollection
                .Where(o => !LinkColumnsOrder.Any(p => p == o.ColumnName))
                .Where(o => o.TableName == ReferenceType)
                .Where(o => o.LinkColumn > 0)
                .OrderBy(o => o.LinkColumn)
                .Select(o => o.ColumnName).ToList<string>());
            LinkColumnsOrder.RemoveAll(o =>
                !Def.ColumnDefinitionCollection.Any(p =>
                    p.TableName == ReferenceType &&
                    p.ColumnName == o &&
                    p.LinkColumn > 0));
        }

        private void UpdateHistoryColumnsOrder()
        {
            if (HistoryColumnsOrder == null) HistoryColumnsOrder = new List<string>();
            HistoryColumnsOrder.AddRange(Def.ColumnDefinitionCollection
                .Where(o => !HistoryColumnsOrder.Any(p => p == o.ColumnName))
                .Where(o => o.TableName == ReferenceType)
                .Where(o => o.HistoryColumn > 0)
                .OrderBy(o => o.HistoryColumn)
                .Select(o => o.ColumnName).ToList<string>());
            HistoryColumnsOrder.RemoveAll(o =>
                !Def.ColumnDefinitionCollection.Any(p =>
                    p.TableName == ReferenceType &&
                    p.ColumnName == o &&
                    p.HistoryColumn > 0));
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
                column.GridVisible = column.GridVisible ?? columnDefinition.GridVisible;
                column.GridDateTime = column.GridDateTime ?? columnDefinition.GridDateTime;
                column.ControlType = column.ControlType ?? columnDefinition.ControlType;
                column.DecimalPlaces = column.DecimalPlaces ?? columnDefinition.DecimalPlaces;
                column.Min = column.Min ?? columnDefinition.Min;
                column.Max = column.Max ?? DefaultMax(columnDefinition);
                column.Step = column.Step ?? DefaultStep(columnDefinition);
                column.EditorVisible = column.EditorVisible ?? columnDefinition.EditorVisible;
                column.EditorReadOnly = column.EditorReadOnly ?? false;
                column.FieldCss = column.FieldCss ?? columnDefinition.FieldCss;
                column.ControlDateTime = column.ControlDateTime ?? columnDefinition.ControlDateTime;
                column.TitleVisible = column.TitleVisible ?? DefaultTitleVisible(column);
                column.LinkVisible = column.LinkVisible ?? columnDefinition.LinkVisible;
                column.HistoryVisible = column.HistoryVisible ?? columnDefinition.HistoryVisible;
                column.Unit = column.Unit ?? columnDefinition.Unit;
                column.Size = columnDefinition.Size;
                column.Nullable = columnDefinition.Nullable;
                column.ReadPermission = Permissions.Get(columnDefinition.ReadPermission);
                column.CreatePermission = Permissions.Get(columnDefinition.CreatePermission);
                column.UpdatePermission = Permissions.Get(columnDefinition.UpdatePermission);
                column.NotSelect = columnDefinition.NotSelect;
                column.NotUpdate = columnDefinition.NotUpdate;
                column.EditSelf = !columnDefinition.NotEditSelf;
                column.GridColumn = columnDefinition.GridColumn > 0;
                column.EditorColumn = columnDefinition.EditorColumn;
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

        private static bool DefaultTitleVisible(Column column)
        {
            return column.ColumnName == "Title";
        }

        public Column AllColumn(string columnName)
        {
            return ColumnCollection.FirstOrDefault(o => o.ColumnName == columnName);
        }

        public Column GridColumn(string columnName)
        {
            return ColumnCollection.FirstOrDefault(o => o.ColumnName == columnName && o.GridColumn);
        }

        public IEnumerable<Column> GridColumnCollection(bool withTitle = false)
        {
            foreach (var columnName in GridColumnsOrder)
            {
                var column = GridColumn(columnName);
                if (column != null &&
                    (column.GridVisible.Value ||
                    (withTitle && column.TitleVisible.Value)))
                {
                    yield return column; 
                }
            }
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

        public IEnumerable<Column> TitleColumnCollection()
        {
            foreach (var columnName in TitleColumnsOrder)
            {
                var column = TitleColumn(columnName);
                if (column != null && column.TitleVisible.Value)
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
                if (column != null && column.LinkVisible.Value)
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
                if (column != null && column.HistoryVisible.Value)
                {
                    yield return column;
                }
            }
        }

        public Dictionary<string, string> GridColumnsHash()
        {
            return ColumnHash(GridColumnsOrder, "Grid");
        }

        public Dictionary<string, string> EditorColumnsHash()
        {
            return ColumnHash(EditorColumnsOrder, "Editor");
        }

        public Dictionary<string, string> TitleColumnsHash()
        {
            return ColumnHash(TitleColumnsOrder, "Title");
        }

        public Dictionary<string, string> LinkColumnsHash()
        {
            return ColumnHash(LinkColumnsOrder, "Link");
        }

        public Dictionary<string, string> HistoryColumnsHash()
        {
            return ColumnHash(HistoryColumnsOrder, "History");
        }

        private Dictionary<string, string> ColumnHash(IEnumerable<string> columnNames, string type)
        {
            var hash = new Dictionary<string, string>();
            columnNames?.ForEach(columnName =>
            {
                var column = GridColumn(columnName);
                if (column != null) Visible(hash, column, type);
            });
            return hash;
        }

        private static void Visible(Dictionary<string, string> hash, Column column, string type)
        {
            var visible = false;
            switch (type)
            {
                case "Grid": visible = column.GridVisible.ToBool(); break;
                case "Editor": visible = column.EditorVisible.ToBool(); break;
                case "Title": visible = column.TitleVisible.ToBool(); break;
                case "Link": visible = column.LinkVisible.ToBool(); break;
                case "History": visible = column.HistoryVisible.ToBool(); break;
            }
            hash.Add(column.ColumnName, visible
                ? Displays.Get(column.LabelText)
                : Displays.Get(column.LabelText) + " (" + Displays.Hidden() + ")");
        }

        public Dictionary<string, string> AggregationDestination()
        {
            return AggregationCollection?.ToDictionary(
                o => o.Id.ToString(),
                o => (o.GroupBy == "[NotGroupBy]"
                    ? Displays.SettingNotGroupBy() 
                    : AllColumn(o.GroupBy).LabelText) +
                        " (" +
                        Displays.Get(o.Type.ToString()) +
                        (o.Target != string.Empty
                            ? ": " + AllColumn(o.Target).LabelText
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
                    o => AllColumn(o.ColumnName).LabelText));
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
                case "TitleSeparator": TitleSeparator = value; break;
                case "AddressBook": AddressBook = value; break;
                case "MailToDefault": MailToDefault = value; break;
                case "MailCcDefault": MailCcDefault = value; break;
                case "MailBccDefault": MailBccDefault = value; break;
                case "GridScript": GridScript = value; break;
                case "NewScript": NewScript = value; break;
                case "EditScript": EditScript = value; break;
            }
        }

        public void SetGridColumns(
            ResponseCollection responseCollection,
            string controlId,
            IEnumerable<string> selectedColumns)
        {
            var order = GridColumnsOrder.ToArray();
            if (controlId == "MoveDownGridColumns") Array.Reverse(order);
            order.Select((o, i) => new { ColumnName = o, Index = i }).ForEach(data =>
            {
                if (selectedColumns.Contains(data.ColumnName))
                {
                    switch (controlId)
                    {
                        case "MoveUpGridColumns":
                        case "MoveDownGridColumns":
                            if (data.Index > 0 &&
                                selectedColumns.Contains(order[data.Index - 1]) == false)
                            {
                                order = Arrays.Swap(order, data.Index, data.Index - 1);
                            }
                            break;
                        case "ShowGridColumns":
                            GridColumn(order[data.Index]).GridVisible = true;
                            break;
                        case "HideGridColumns":
                            GridColumn(order[data.Index]).GridVisible = false;
                            break;
                    }
                }
            });
            if (controlId == "MoveDownGridColumns") Array.Reverse(order);
            GridColumnsOrder = order.ToList<string>();
            responseCollection.Html("#GridColumns",
                new HtmlBuilder().SelectableItems(
                    listItemCollection: GridColumnsHash(),
                    selectedValueTextCollection: selectedColumns));
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

        public void SetEditorColumns(
            ResponseCollection responseCollection,
            string controlId,
            IEnumerable<string> selectedColumns)
        {
            var order = EditorColumnsOrder.ToArray();
            if (controlId == "MoveDownEditorColumns") Array.Reverse(order);
            order.Select((o, i) => new { ColumnName = o, Index = i }).ForEach(data =>
            {
                if (selectedColumns.Contains(data.ColumnName))
                {
                    switch (controlId)
                    {
                        case "MoveUpEditorColumns":
                        case "MoveDownEditorColumns":
                            if (data.Index > 0 &&
                                selectedColumns.Contains(order[data.Index - 1]) == false)
                            {
                                order = Arrays.Swap(order, data.Index, data.Index - 1);
                            }
                            break;
                        case "ShowEditorColumns":
                            EditorColumn(order[data.Index]).EditorVisible = true;
                            break;
                        case "HideEditorColumns":
                            EditorColumn(order[data.Index]).EditorVisible = false;
                            break;
                    }
                }
            });
            if (controlId == "MoveDownEditorColumns") Array.Reverse(order);
            EditorColumnsOrder = order.ToList<string>();
            responseCollection.Html("#EditorColumns",
                new HtmlBuilder().SelectableItems(
                    listItemCollection: EditorColumnsHash(),
                    selectedValueTextCollection: selectedColumns));
        }

        public void SetColumnProperty(Column column, string propertyName, string value)
        {
            switch (propertyName)
            {
                case "ColumnName": column.ColumnName = value; break;
                case "LabelText": column.LabelText = value; break;
                case "ControlType": column.ControlType = value; break;
                case "DecimalPlaces": column.DecimalPlaces = value.ToInt(); break;
                case "Max": column.Max = value.ToDecimal(); break;
                case "Min": column.Min = value.ToDecimal(); break;
                case "Step": column.Step = value.ToDecimal(); break;
                case "EditorReadOnly": column.EditorReadOnly = value.ToBool(); break;
                case "FieldCss": column.FieldCss = value; break;
                case "ChoicesText": SetChoices(column, value); break;
                case "DefaultInput": column.DefaultInput = value; break;
                case "GridDateTime": column.GridDateTime = value; break;
                case "ControlDateTime": column.ControlDateTime = value; break;
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
                var id = SummaryCollection.Count() > 0
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

        public void SetTitleColumns(
            ResponseCollection responseCollection,
            string controlId,
            IEnumerable<string> selectedColumns)
        {
            var order = TitleColumnsOrder.ToArray();
            if (controlId == "MoveDownTitleColumns") Array.Reverse(order);
            order.Select((o, i) => new { ColumnName = o, Index = i }).ForEach(data =>
            {
                if (selectedColumns.Contains(data.ColumnName))
                {
                    switch (controlId)
                    {
                        case "MoveUpTitleColumns":
                        case "MoveDownTitleColumns":
                            if (data.Index > 0 &&
                                selectedColumns.Contains(order[data.Index - 1]) == false)
                            {
                                order = Arrays.Swap(order, data.Index, data.Index - 1);
                            }
                            break;
                        case "ShowTitleColumns":
                            TitleColumn(order[data.Index]).TitleVisible = true;
                            break;
                        case "HideTitleColumns":
                            TitleColumn(order[data.Index]).TitleVisible = false;
                            break;
                    }
                }
            });
            if (controlId == "MoveDownTitleColumns") Array.Reverse(order);
            TitleColumnsOrder = order.ToList<string>();
            responseCollection.Html("#TitleColumns",
                new HtmlBuilder().SelectableItems(
                    listItemCollection: TitleColumnsHash(),
                    selectedValueTextCollection: selectedColumns));
        }

        public void SetLinkColumns(
            ResponseCollection responseCollection,
            string controlId,
            IEnumerable<string> selectedColumns)
        {
            var order = LinkColumnsOrder.ToArray();
            if (controlId == "MoveDownLinkColumns") Array.Reverse(order);
            order.Select((o, i) => new { ColumnName = o, Index = i }).ForEach(data =>
            {
                if (selectedColumns.Contains(data.ColumnName))
                {
                    switch (controlId)
                    {
                        case "MoveUpLinkColumns":
                        case "MoveDownLinkColumns":
                            if (data.Index > 0 &&
                                selectedColumns.Contains(order[data.Index - 1]) == false)
                            {
                                order = Arrays.Swap(order, data.Index, data.Index - 1);
                            }
                            break;
                        case "ShowLinkColumns":
                            LinkColumn(order[data.Index]).LinkVisible = true;
                            break;
                        case "HideLinkColumns":
                            LinkColumn(order[data.Index]).LinkVisible = false;
                            break;
                    }
                }
            });
            if (controlId == "MoveDownLinkColumns") Array.Reverse(order);
            LinkColumnsOrder = order.ToList<string>();
            responseCollection.Html("#LinkColumns",
                new HtmlBuilder().SelectableItems(
                    listItemCollection: LinkColumnsHash(),
                    selectedValueTextCollection: selectedColumns));
        }

        public void SetHistoryColumns(
            ResponseCollection responseCollection,
            string controlId,
            IEnumerable<string> selectedColumns)
        {
            var order = HistoryColumnsOrder.ToArray();
            if (controlId == "MoveDownHistoryColumns") Array.Reverse(order);
            order.Select((o, i) => new { ColumnName = o, Index = i }).ForEach(data =>
            {
                if (selectedColumns.Contains(data.ColumnName))
                {
                    switch (controlId)
                    {
                        case "MoveUpHistoryColumns":
                        case "MoveDownHistoryColumns":
                            if (data.Index > 0 &&
                                selectedColumns.Contains(order[data.Index - 1]) == false)
                            {
                                order = Arrays.Swap(order, data.Index, data.Index - 1);
                            }
                            break;
                        case "ShowHistoryColumns":
                            HistoryColumn(order[data.Index]).HistoryVisible = true;
                            break;
                        case "HideHistoryColumns":
                            HistoryColumn(order[data.Index]).HistoryVisible = false;
                            break;
                    }
                }
            });
            if (controlId == "MoveDownHistoryColumns") Array.Reverse(order);
            HistoryColumnsOrder = order.ToList<string>();
            responseCollection.Html("#HistoryColumns",
                new HtmlBuilder().SelectableItems(
                    listItemCollection: HistoryColumnsHash(),
                    selectedValueTextCollection: selectedColumns));
        }

        public void SetLinks()
        {
            if (LinkColumnSiteIdHash?.Count > 0)
            {
                var dataRows = Rds.ExecuteTable(
                    statements: Rds.SelectItems(
                        column: Rds.ItemsColumn()
                            .ReferenceId()
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
                    SetLinks(linkColumnSiteId, dataRows));
            }
        }

        private void SetLinks(
            KeyValuePair<string, long> linkColumnSiteId, EnumerableRowCollection<DataRow> dataRows)
        {
            var columnKey = linkColumnSiteId.Key;
            var columnName = columnKey.Substring(0, columnKey.LastIndexOf('_'));
            var column = AllColumn(columnName);
            column.ChoicesText = column.ChoicesText.SplitReturn()
                .Select(o => o.Trim())
                .Where(o => o != string.Empty)
                .Select(o => ChoicesTextLine(linkColumnSiteId, dataRows, o))
                .Join("\n");
        }

        private static string ChoicesTextLine(
            KeyValuePair<string, long> linkColumnSiteId,
            EnumerableRowCollection<DataRow> dataRows,
            string line)
        {
            return line != "[[{0}]]".Params(linkColumnSiteId.Value.ToString())
                ? line
                : dataRows
                    .Select(p => new
                    {
                        ReferenceId = p["ReferenceId"].ToLong(),
                        SiteId = p["SiteId"].ToLong(),
                        Title = p["Title"].ToString()
                    })
                    .Where(p => p.SiteId == linkColumnSiteId.Value)
                    .Select(p => "{0},{0}: {1}".Params(
                        p.ReferenceId,
                        p.Title))
                    .Join("\n");
        }

        public EnumerableRowCollection<DataRow> SummarySiteDataRows()
        {
            if (LinkColumnSiteIdHash == null) return null;
            var siteIdCollection = LinkColumnSiteIdHash.Values.ToList<long>();
            if (siteIdCollection.Count == 0)
            {
                siteIdCollection.Add(-1);
            }
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
    }
}