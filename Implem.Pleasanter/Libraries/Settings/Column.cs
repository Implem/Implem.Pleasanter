using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.ServerData;
using Implem.Pleasanter.Libraries.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Settings
{
    public class Column
    {
        public string Id;
        public string ColumnName;
        public string LabelText;
        public string ChoicesText;
        public string DefaultInput;
        public bool? GridVisible;
        public string GridDateTime;
        public string ControlDateTime;
        public string ControlType;
        public int? DecimalPlaces;
        public decimal? Min;
        public decimal? Max;
        public decimal? Step;
        public bool? EditorVisible;
        public bool? EditorReadOnly;
        public string FieldCss;
        public bool? TitleVisible;
        public string Unit;
        public bool? Link;
        [NonSerialized]
        public int? No;
        [NonSerialized]
        public bool Nullable;
        [NonSerialized]
        public Permissions.Types ReadPermission;
        [NonSerialized]
        public Permissions.Types CreatePermission;
        [NonSerialized]
        public Permissions.Types UpdatePermission;
        [NonSerialized]
        public bool NotSelect;
        [NonSerialized]
        public bool NotUpdate;
        [NonSerialized]
        public bool GridColumn;
        [NonSerialized]
        public bool EditSelf;
        [NonSerialized]
        public bool EditorColumn;
        [NonSerialized]
        public bool TitleColumn;
        [NonSerialized]
        public bool Export;
        [NonSerialized]
        public string LabelTextDefault;
        [NonSerialized]
        public string TypeName;
        [NonSerialized]
        public string TypeCs;
        [NonSerialized]
        public string ColumnType;
        [NonSerialized]
        public bool UserColumn;
        [NonSerialized]
        public bool Hash;
        [NonSerialized]
        public string StringFormat;
        [NonSerialized]
        public string UnitDefault;
        [NonSerialized]
        public int Width;
        [NonSerialized]
        public string ControlCss;
        [NonSerialized]
        public bool MarkDown;
        [NonSerialized]
        public string GridStyle;
        [NonSerialized]
        public bool Aggregatable;
        [NonSerialized]
        public bool Computable;
        [NonSerialized]
        public string Validators;
        [NonSerialized]
        public bool? FloatClear;

        public bool HasChoices()
        {
            return ControlType == "ChoicesText" && ChoicesText.Trim() != string.Empty;
        }

        public Column()
        {
        }

        public Column(string columnName)
        {
            ColumnName = columnName;
        }

        public Column(Column source)
        {
            Id = source.Id;
            No = source.No;
            ColumnName = source.ColumnName;
            LabelText = source.LabelText;
            ChoicesText = source.ChoicesText;
            DefaultInput = source.DefaultInput;
            GridVisible = source.GridVisible;
            GridDateTime = source.GridDateTime;
            ControlDateTime = source.ControlDateTime;
            ControlType = source.ControlType;
            DecimalPlaces = source.DecimalPlaces;
            Min = source.Min;
            Max = source.Max;
            Step = source.Step;
            EditorVisible = source.EditorVisible;
            EditorReadOnly = source.EditorReadOnly;
            TitleVisible = source.TitleVisible;
            Unit = source.Unit;
            Link = source.Link;
            Nullable = source.Nullable;
            ReadPermission = source.ReadPermission;
            CreatePermission = source.CreatePermission;
            UpdatePermission = source.UpdatePermission;
            NotSelect = source.NotSelect;
            NotUpdate = source.NotUpdate;
            GridColumn = source.GridColumn;
            EditSelf = source.EditSelf;
            EditorColumn = source.EditorColumn;
            TitleColumn = source.TitleColumn;
            Export = source.Export;
            LabelTextDefault = source.LabelTextDefault;
            TypeName = source.TypeName;
            TypeCs = source.TypeCs;
            ColumnType = source.ColumnType;
            ControlType = source.ControlType;
            UserColumn = source.UserColumn;
            UserColumn = source.Hash;
            StringFormat = source.StringFormat;
            UnitDefault = source.UnitDefault;
            Width = source.Width;
            FieldCss = source.FieldCss;
            ControlCss = source.ControlCss;
            MarkDown = source.MarkDown;
            GridStyle = source.GridStyle;
            Aggregatable = source.Aggregatable;
            Computable = source.Computable;
            Validators = source.Validators;
            FloatClear = source.FloatClear;
        }

        public Dictionary<string, ControlData> EditChoices(
            long siteId,
            bool addBlank = false,
            bool shorten = false)
        {
            var tenantId = Sessions.TenantId();
            var editChoices = new Dictionary<string, ControlData>();
            if (Nullable || addBlank)
            {
                editChoices.Add(string.Empty, new ControlData(string.Empty));
            }
            Choices().Select((o, i) => new { Choice = o, Index = i }).ForEach(data =>
            {
                var value = data.Choice.SelectedValue;
                switch (value)
                {
                    case "[[Depts]]":
                        editChoices = editChoices
                            .Concat(SiteInfo.GetDepts()
                                .Where(o => o.Value.TenantId == tenantId)
                                .ToDictionary(
                                    o => o.Key.ToString(),
                                    o => new ControlData(SiteInfo.DeptModel(o.Key).DeptName)))
                            .ToDictionary(o => o.Key, o => o.Value);
                        break;
                    case "[[Users]]":
                        editChoices = editChoices
                            .Concat(SiteInfo.UserIdCollection(siteId)
                                .ToDictionary(
                                    o => o.ToString(),
                                    o => new ControlData(SiteInfo.UserFullName(o))))
                            .ToDictionary(o => o.Key, o => o.Value);
                        break;
                    case "[[Users*]]":
                        editChoices = editChoices
                            .Concat(SiteInfo.Users
                                .Where(o => o.Value.TenantId == tenantId)
                                .ToDictionary(
                                    o => o.Key.ToString(),
                                    o => new ControlData(o.Value.FullName)))
                            .ToDictionary(o => o.Key, o => o.Value);
                        break;
                    case "[[TimeZones]]":
                        editChoices = editChoices
                            .Concat(TimeZoneInfo.GetSystemTimeZones()
                                .ToDictionary(
                                    o => o.Id,
                                    o => new ControlData(o.StandardName)))
                            .ToDictionary(o => o.Key, o => o.Value);
                        break;
                    default:
                        var key = TypeName != "bit"
                                ? value
                                : (data.Index == 0).ToOneOrZeroString();
                        if (!editChoices.ContainsKey(key))
                        {
                            editChoices.Add(
                                key,
                                new ControlData(
                                    text: data.Choice.Text(),
                                    css: data.Choice.CssClass(),
                                    style: data.Choice.Style()));
                        }
                        break;
                }
            });
            return editChoices;
        }

        private IEnumerable<Choice> Choices()
        {
            foreach (var selectedValue in ChoicesText.SplitReturn()
                .Where(o => o.Trim() != string.Empty)
                .Select(o => o.Trim().Split_1st()))
            {
                yield return Choice(selectedValue);
            }
        }

        public Choice Choice(string selectedValue)
        {
            return new Choice(
                ChoicesText.SplitReturn().FirstOrDefault(o => o.Split_1st(',') == selectedValue),
                selectedValue);
        }

        public object RecordingData(string value, long siteId)
        {
            object recordingData = value;
            if (UserColumn)
            {
                recordingData = SiteInfo.Users
                    .Where(o => o.Value.FullName == value)
                    .Select(o => o.Value.Id)
                    .FirstOrDefault(o => SiteInfo.UserIdCollection(siteId).Any(p => p == o));
            }
            else if (TypeCs == "Comments")
            {
                return new Comments().Prepend(value).ToJson();
            }
            else if (HasChoices())
            {
                recordingData = Choices().Where(o => o.Text() == value)
                    .Select(o => o.Value())
                    .FirstOrDefault();
            }
            if (recordingData == null) return null;
            switch (TypeName)
            {
                case "bit": return recordingData.ToBool();
                case "int": return recordingData.ToInt();
                case "bigint": return recordingData.ToLong();
                case "float": return recordingData.ToFloat();
                case "decimal": return recordingData.ToDecimal();
                case "datetime": return recordingData.ToDateTime().ToUniversal();
                case "nchar":
                case "nvarchar": return recordingData;
                default: return recordingData;
            }
        }

        public string Format(decimal value)
        {
            return value.ToString("0", DecimalPlaces.ToInt() == 0
                ? "0"
                : "0." + new String('0', DecimalPlaces.ToInt()));
        }
    }
}