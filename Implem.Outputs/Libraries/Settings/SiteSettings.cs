using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.ServerData;
using Implem.Pleasanter.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Views;
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
        public decimal? NearDeadlineAfterDays;
        public decimal? NearDeadlineBeforeDays;
        public int? GridPageSize;
        public List<string> GridOrder = new List<string>();
        public List<string> EditorOrder = new List<string>();
        public List<string> TitleOrder = new List<string>();
        public string TitleSeparator = ")";
        public List<string> HistoryGrid = new List<string>();
        public List<Column> ColumnCollection = new List<Column>();
        public List<Aggregation> AggregationCollection = new List<Aggregation>();
        public Dictionary<string, long> LinkColumnSiteIdHash = new Dictionary<string, long>();
        public List<Summary> SummaryCollection = new List<Summary>();
        public string AddressBook;
        public string MailToDefault;
        public string MailCcDefault;
        public string MailBccDefault;

        public SiteSettings()
        {
        }

        public SiteSettings(string referenceType)
        {
            ReferenceType = referenceType;
            Init();
        }

        public SiteSettings(SiteSettings source)
        {
            ReferenceType = source.ReferenceType;
            SiteId = source.SiteId;
            InheritPermission = source.InheritPermission;
            ParentId = source.ParentId;
            Title = source.Title;
            AccessStatus = source.AccessStatus;
            NearDeadlineAfterDays = source.NearDeadlineAfterDays;
            NearDeadlineBeforeDays = source.NearDeadlineBeforeDays;
            GridPageSize = source.GridPageSize;
            GridOrder = new List<string>(source.GridOrder);
            EditorOrder = new List<string>(source.EditorOrder);
            TitleOrder = new List<string>(source.TitleOrder);
            TitleSeparator = source.TitleSeparator;
            ColumnCollection = new List<Column>(source.ColumnCollection);
            AggregationCollection = new List<Aggregation>(source.AggregationCollection);
            LinkColumnSiteIdHash = new Dictionary<string, long>(source.LinkColumnSiteIdHash);
            SummaryCollection = new List<Summary>(source.SummaryCollection);
            AddressBook = source.AddressBook;
            MailToDefault = source.MailToDefault;
            MailCcDefault = source.MailCcDefault;
            MailBccDefault = source.MailBccDefault;
        }

        public void Init()
        {
            NearDeadlineBeforeDays = NearDeadlineBeforeDays ?? Parameters.NearDeadlineBeforeDays;
            NearDeadlineAfterDays = NearDeadlineAfterDays ?? Parameters.NearDeadlineAfterDays;
            GridPageSize = GridPageSize ?? Parameters.GridPageSize;
            UpdateGrid();
            UpdateEditor();
            UpdateTableSet();
            UpdateHistoryGrid();
            UpdateColumnCollection();
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

        public string ToJson()
        {
            return Jsons.ToJson(new SiteSettings(this));
        }

        private void UpdateGrid()
        {
            GridOrder.AddRange(Def.ColumnDefinitionCollection
                .Where(o => !GridOrder.Any(p => p == o.ColumnName))
                .Where(o => o.TableName == ReferenceType)
                .Where(o => o.GridColumn > 0)
                .OrderBy(o => o.GridColumn)
                .Select(o => o.ColumnName).ToList<string>());
            GridOrder.RemoveAll(o =>
                !Def.ColumnDefinitionCollection.Any(p =>
                    p.ColumnName == o &&
                    p.TableName == ReferenceType &&
                    p.GridColumn > 0));
        }

        private void UpdateEditor()
        {
            EditorOrder.AddRange(Def.ColumnDefinitionCollection
                .Where(o => !EditorOrder.Any(p => p == o.ColumnName))
                .Where(o => o.TableName == ReferenceType && o.EditorColumn)
                .Where(o => !o.NotSettings)
                .OrderBy(o => o.No)
                .Select(o => o.ColumnName).ToList<string>());
            EditorOrder.RemoveAll(o => 
                !Def.ColumnDefinitionCollection.Any(p =>
                    p.ColumnName == o && 
                    p.TableName == ReferenceType &&
                    p.EditorColumn &&
                    !p.NotSettings));
        }

        private void UpdateTableSet()
        {
            TitleOrder.AddRange(Def.ColumnDefinitionCollection
                .Where(o => !TitleOrder.Any(p => p == o.ColumnName))
                .Where(o => o.TableName == ReferenceType)
                .Where(o => o.TitleColumn)
                .OrderBy(o => o.No)
                .Select(o => o.ColumnName).ToList<string>());
            TitleOrder.RemoveAll(o =>
                !Def.ColumnDefinitionCollection.Any(p =>
                    p.TableName == ReferenceType &&
                    p.ColumnName == o &&
                    p.TitleColumn &&
                    !p.NotSettings));
        }

        private void UpdateHistoryGrid()
        {
            HistoryGrid.AddRange(Def.ColumnDefinitionCollection
                .Where(o => !HistoryGrid.Any(p => p == o.ColumnName))
                .Where(o => o.TableName == ReferenceType)
                .Where(o => o.HistoryGrid > 0)
                .OrderBy(o => o.HistoryGrid)
                .Select(o => o.ColumnName).ToList<string>());
            HistoryGrid.RemoveAll(o =>
                !Def.ColumnDefinitionCollection.Any(p => p.ColumnName == o && p.HistoryGrid > 0));
        }

        private void UpdateColumnCollection(bool onSerializing = false)
        {
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
                column.No = column.No ?? columnDefinition.No;
                column.ColumnName = column.ColumnName ?? columnDefinition.ColumnName;
                column.LabelText = column.LabelText ?? Displays.Get(columnDefinition.Id);
                column.ChoicesText = column.ChoicesText ?? columnDefinition.ChoicesText;
                column.DefaultInput = column.DefaultInput ?? columnDefinition.DefaultInput;
                column.GridVisible = column.GridVisible ?? columnDefinition.GridVisible;
                column.GridDateTime = column.GridDateTime ?? columnDefinition.GridDateTime;
                column.ControlType = column.ControlType ?? columnDefinition.ControlType;
                column.DecimalPlaces = column.DecimalPlaces ?? columnDefinition.DecimalPlaces;
                column.Min = column.Min ?? columnDefinition.Min;
                column.Max = column.Max ?? (columnDefinition.Max > 0
                    ? columnDefinition.Max
                    : columnDefinition.MaxLength);
                column.Step = column.Step ?? (columnDefinition.Step > 0
                    ? columnDefinition.Step
                    : 1);
                column.EditorVisible = column.EditorVisible ?? columnDefinition.EditorVisible;
                column.EditorReadOnly = column.EditorReadOnly ?? false;
                column.FieldCss = column.FieldCss ?? columnDefinition.FieldCss;
                column.ControlDateTime = column.ControlDateTime ?? columnDefinition.ControlDateTime;
                column.TitleVisible = column.TitleVisible ?? column.ColumnName == "Title"
                    ? true
                    : false;
                column.Unit = column.Unit ?? columnDefinition.Unit;
                column.Nullable = columnDefinition.Nullable;
                column.ReadPermission = Permissions.Get(columnDefinition.ReadPermission);
                column.CreatePermission = Permissions.Get(columnDefinition.CreatePermission);
                column.UpdatePermission = Permissions.Get(columnDefinition.UpdatePermission);
                column.NotSelect = columnDefinition.NotSelect;
                column.NotUpdate = columnDefinition.NotUpdate;
                column.EditSelf = !columnDefinition.NotEditSelf;
                column.GridColumn = columnDefinition.GridColumn > 0;
                column.EditorColumn = columnDefinition.EditorColumn;
                column.TitleColumn = columnDefinition.TitleColumn;
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
                column.Validations = columnDefinition.Validations;
            }
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
            foreach (var columnName in GridOrder)
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

        public IEnumerable<Column> EditorColumnCollection()
        {
            foreach (var columnName in EditorOrder)
            {
                var column = EditorColumn(columnName);
                if (column != null && column.EditorVisible.Value)
                {
                    yield return column;
                }
            }
        }

        public Column TitleColumn(string columnName)
        {
            return ColumnCollection.FirstOrDefault(o => o.ColumnName == columnName && o.TitleColumn);
        }

        public IEnumerable<Column> TitleColumnCollection()
        {
            foreach (var columnName in TitleOrder)
            {
                var column = TitleColumn(columnName);
                if (column != null && column.TitleVisible.Value)
                {
                    yield return column;
                }
            }
        }

        public IEnumerable<Column> HistoryGridColumnCollection()
        {
            foreach (var columnName in HistoryGrid)
            {
                yield return AllColumn(columnName);
            }
        }

        public Dictionary<string, string> GridColumnHash()
        {
            return ColumnHash(GridOrder, "Grid");
        }

        public Dictionary<string, string> EditorColumnHash()
        {
            return ColumnHash(EditorOrder, "Editor");
        }

        public Dictionary<string, string> TitleColumnHash()
        {
            return ColumnHash(TitleOrder, "Title");
        }

        private Dictionary<string, string> ColumnHash(IEnumerable<string> columnNames, string type)
        {
            var hash = new Dictionary<string, string>();
            columnNames.ForEach(columnName =>
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
            }
            hash.Add(column.ColumnName, visible
                ? Displays.Get(column.LabelText)
                : Displays.Get(column.LabelText) + " (" + Displays.Hidden() + ")");
        }

        public Dictionary<string, string> AggregationDestination()
        {
            return AggregationCollection.ToDictionary(
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
                case "NearDeadlineBeforeDays": NearDeadlineBeforeDays = value.ToInt(); break;
                case "NearDeadlineAfterDays": NearDeadlineAfterDays = value.ToInt(); break;
                case "GridPageSize": GridPageSize = value.ToInt(); break;
                case "AddressBook": AddressBook = value; break;
                case "MailToDefault": MailToDefault = value; break;
                case "MailCcDefault": MailCcDefault = value; break;
                case "MailBccDefault": MailBccDefault = value; break;
                case "TitleSeparator": TitleSeparator = value; break;
            }
        }

        public void SetGridColumns(
            ResponseCollection responseCollection,
            string controlId,
            IEnumerable<string> selectedColumns)
        {
            var order = GridOrder.ToArray();
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
            GridOrder = order.ToList<string>();
            responseCollection.Html("#GridColumns",
                Html.Builder().SelectableItems(
                    listItemCollection: GridColumnHash(),
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
                .Html("#AggregationDestination", Html.Builder()
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
            var order = EditorOrder.ToArray();
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
            EditorOrder = order.ToList<string>();
            responseCollection.Html("#EditorColumns",
                Html.Builder().SelectableItems(
                    listItemCollection: EditorColumnHash(),
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
            var order = TitleOrder.ToArray();
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
            TitleOrder = order.ToList<string>();
            responseCollection.Html("#TitleColumns",
                Html.Builder().SelectableItems(
                    listItemCollection: TitleColumnHash(),
                    selectedValueTextCollection: selectedColumns));
        }

        public void SetLinks()
        {
            if (LinkColumnSiteIdHash.Count > 0)
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