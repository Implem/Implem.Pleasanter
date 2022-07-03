﻿using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Implem.Pleasanter.Libraries.Settings
{
    public class DataChange : ISettingListItem
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public enum Types
        {
            CopyValue,
            CopyDisplayValue,
            InputValue,
            InputDate,
            InputDateTime,
            InputUser,
            InputDept
        }

        public int Id { get; set; }
        public Types Type { get; set; }
        public string ColumnName { get; set; }
        public string Value { get; set; }

        public DataChange()
        {
        }

        public DataChange(
            int id,
            Types type,
            string columnName,
            string value)
        {
            Id = id;
            Type = type;
            ColumnName = columnName;
            Value = value;
        }

        public void Update(
            Types type,
            string columnName,
            string value)
        {
            Type = type;
            ColumnName = columnName;
            Value = value;
        }

        public DataChange GetRecordingData(
            Context context,
            SiteSettings ss)
        {
            var dataChange = new DataChange();
            dataChange.Id = Id;
            dataChange.Type = Type;
            dataChange.ColumnName = ColumnName;
            dataChange.Value = Value;
            return dataChange;
        }

        public bool Visible(string type)
        {
            switch (Type)
            {
                case Types.CopyValue:
                case Types.CopyDisplayValue:
                    return type == "Column";
                case Types.InputValue:
                    return type == "Value";
                case Types.InputDate:
                case Types.InputDateTime:
                    return type == "DateTime";
                case Types.InputUser:
                case Types.InputDept:
                    return type == "Context";
                default:
                    return false;
            }
        }

        public string DisplayValue(Context context, SiteSettings ss)
        {
            if (Visible(type: "Column"))
            {
                return ss.GetColumn(
                    context: context,
                    columnName: Value)?.LabelText;
            }
            else if (Visible(type: "DateTime"))
            {
                var period = Displays.Get(
                    context: context,
                    id: DateTimePeriod());
                return $"{DateTimeNumber()} {period}";
            }
            else
            {
                return ss.ColumnNameToLabelText(Value);
            }
        }

        public int DateTimeNumber()
        {
            return Value?.Split_1st().ToInt() ?? 0;
        }

        public string DateTimePeriod()
        {
            return Value?.Split_2nd();
        }

        public string ValueData(Dictionary<string, string> KeyValues)
        {
            var ret = Value;
            if (KeyValues.Any())
            {
                var temp = new Dictionary<string, string>();
                KeyValues.ForEach(data =>
                {
                    var guid = $"[{Strings.NewGuid()}]";
                    ret = ret.Replace($"[{data.Key}]", guid);
                    temp.Add(guid, data.Key);
                });
                temp.ForEach(data =>
                {
                    ret = ret.Replace(data.Key, KeyValues.Get(data.Value) ?? string.Empty);
                });
            }
            return ret;
        }

        public DateTime DateTimeValue(Context context)
        {
            var ret = DateTime.Now;
            switch (Type)
            {
                case Types.InputDate:
                    ret = ret.ToLocal(context: context).Date;
                    break;
                case Types.InputDateTime:
                    ret = ret.ToLocal(context: context);
                    break;
            }
            var number = DateTimeNumber();
            if (number != 0)
            {
                var period = DateTimePeriod();
                switch (period)
                {
                    case "Days":
                        ret = ret.AddDays(number);
                        break;
                    case "Months":
                        ret = ret.AddMonths(number);
                        break;
                    case "Years":
                        ret = ret.AddYears(number);
                        break;
                    case "Hours":
                        ret = ret.AddHours(number);
                        break;
                    case "Minutes":
                        ret = ret.AddMinutes(number);
                        break;
                    case "Seconds":
                        ret = ret.AddSeconds(number);
                        break;
                }
            }
            return ret;
        }
    }
}