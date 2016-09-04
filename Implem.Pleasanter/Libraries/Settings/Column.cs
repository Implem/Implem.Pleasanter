using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Validators;
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
        public string GridFormat;
        public string ControlFormat;
        public string ExportFormat;
        public string ControlType;
        public string Format;
        public int? DecimalPlaces;
        public decimal? Min;
        public decimal? Max;
        public decimal? Step;
        public bool? EditorReadOnly;
        public string FieldCss;
        public string Unit;
        public bool? Link;
        [NonSerialized]
        public int? No;
        [NonSerialized]
        public bool Id_Ver;
        [NonSerialized]
        public string Size;
        [NonSerialized]
        public bool Nullable;
        [NonSerialized]
        public bool RecordedTime;
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
        public bool FilterColumn;
        [NonSerialized]
        public bool EditSelf;
        [NonSerialized]
        public bool EditorColumn;
        [NonSerialized]
        public bool NotEditorSettings;
        [NonSerialized]
        public bool TitleColumn;
        [NonSerialized]
        public bool LinkColumn;
        [NonSerialized]
        public bool HistoryColumn;
        [NonSerialized]
        public bool Export;
        [NonSerialized]
        public string LabelTextDefault;
        [NonSerialized]
        public string TypeName;
        [NonSerialized]
        public string TypeCs;
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
        // compatibility
        public bool? GridVisible;
        public bool? FilterVisible;
        public bool? EditorVisible;
        public bool? TitleVisible;
        public bool? LinkVisible;
        public bool? HistoryVisible;
        // compatibility Version 1.001
        public string GridDateTime;
        public string ControlDateTime;

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

        public void SetChoicesByPlaceholders(long siteId)
        {
            var tenantId = Sessions.TenantId();
            var choicesHash = new Dictionary<string, string>();
            ChoicesText.SplitReturn()
                .Select((o, i) => new { KeyValue = KeyValue(o), Index = i })
                .ForEach(data =>
                {
                    switch (data.KeyValue.Key)
                    {
                        case "[[Depts]]":
                            SiteInfo.GetDepts()
                                .Where(o => o.Value.TenantId == tenantId)
                                .ForEach(o => Add(
                                    choicesHash,
                                    o.Key.ToString(),
                                    SiteInfo.DeptModel(o.Key).DeptName));
                            break;
                        case "[[Users]]":
                            SiteInfo.UserIdCollection(siteId)
                                .ForEach(o => Add(
                                    choicesHash,
                                    o.ToString(),
                                    SiteInfo.UserFullName(o)));
                            break;
                        case "[[Users*]]":
                            SiteInfo.Users
                                .Where(o => o.Value.TenantId == tenantId)
                                .ForEach(o => Add(
                                    choicesHash,
                                    o.Key.ToString(),
                                    o.Value.FullName()));
                            break;
                        case "[[TimeZones]]":
                            TimeZoneInfo.GetSystemTimeZones()
                                .ForEach(o => Add(
                                    choicesHash,
                                    o.Id,
                                    o.StandardName));
                            break;
                        default:
                            var key = TypeName != "bit"
                                ? data.KeyValue.Key
                                : (data.Index == 0).ToOneOrZeroString();
                            Add(choicesHash, key, data.KeyValue.Value);
                            break;
                    }
                });
            ChoicesText = choicesHash.Select(o => 
                o.Key + (o.Value != string.Empty 
                    ? "," + o.Value
                    : string.Empty)).Join("\r\n");
        }

        private KeyValuePair<string, string> KeyValue(string choice)
        {
            return choice.Contains(',')
                ? new KeyValuePair<string, string>(
                    choice.Substring(0, choice.IndexOf(',')),
                    choice.Substring(choice.IndexOf(',') + 1))
                : new KeyValuePair<string, string>(choice, string.Empty);
        }

        private void Add(Dictionary<string, string> choicesHash, string key, string value)
        {
            if (!choicesHash.Keys.Contains(key))
            {
                choicesHash.Add(key, value);
            }
        }

        public Dictionary<string, ControlData> EditChoices(
            long siteId,
            bool insertBlank = false,
            bool shorten = false,
            bool addNotSet = false)
        {
            var tenantId = Sessions.TenantId();
            var editChoices = new Dictionary<string, ControlData>();
            if (insertBlank)
            {
                editChoices.Add(
                    UserColumn
                        ? User.UserTypes.Anonymous.ToInt().ToString()
                        : string.Empty,
                    new ControlData(string.Empty));
            }
            Choices().Select((o, i) => new { Choice = o, Index = i }).ForEach(data =>
            {
                if (!editChoices.ContainsKey(data.Choice.SelectedValue))
                {
                    editChoices.Add(
                        data.Choice.SelectedValue,
                        new ControlData(
                            text: data.Choice.Text(),
                            css: data.Choice.CssClass(),
                            style: data.Choice.Style()));
                }
            });
            if (addNotSet && Nullable)
            {
                editChoices.Add("\t", new ControlData(Displays.NotSet()));
            }
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
                    .Where(o => o.Value.FullName() == value)
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

        public Dictionary<string, string> ValidationMessages()
        {
            return ClientValidators.MessageCollection(Validators);
        }

        public string Display(decimal value)
        {
            return !Format.IsNullOrEmpty()
                ? value.ToString(Format, Sessions.CultureInfo())
                : DecimalPlaces.ToInt() == 0
                    ? value.ToString("0", "0")
                    : TrimZero(value.ToString("0", "0." + new String('0', DecimalPlaces.ToInt())));
        }

        private static string TrimZero(string str)
        {
            return str.Contains(".")
                ? str.TrimEnd('0')
                : str;
        }

        public string Display(decimal value, Permissions.Types permissionType)
        {
            return Display(value) + (EditorReadOnly.ToBool() || !this.CanUpdate(permissionType)
                ? Unit
                : string.Empty);
        }

        public string Display(decimal value, bool unit)
        {
            return Display(value) + (unit
                ? Unit
                : string.Empty);
        }

        public string DisplayControl(DateTime value)
        {
            return Display(value, ControlFormat);
        }

        public string DisplayGrid(DateTime value)
        {
            return Display(value, GridFormat);
        }

        private string Display(DateTime value, string format)
        {
            return value.InRange()
                ? !format.IsNullOrEmpty()
                    ? value.ToString(
                        Displays.Get(format + "Format"),
                        Sessions.CultureInfo())
                    : value.ToString(Sessions.CultureInfo())
                : string.Empty;
        }

        public decimal Round(decimal value)
        {
             return Math.Round(value, DecimalPlaces.ToInt(), MidpointRounding.AwayFromZero);
        }
    }
}