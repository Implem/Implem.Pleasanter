using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Implem.Pleasanter.MCP.Translator
{
    public class DisplayTranslator
    {
        private const string DateDisplayFormat = "yyyy/MM/dd H:mm:ss";
        private const string DeptsPattern = "[[Depts";
        private const string GroupsPattern = "[[Groups";
        private const string UsersPattern = "[[Users";

        private static readonly string[] DateColumnPrefixes = new[]
        {
            "Date"
        };

        private static readonly HashSet<string> DateColumnNames = new HashSet<string>
        {
            "CompletionTime",
            "CreatedTime",
            "StartTime",
            "UpdatedTime"
        };

        private static readonly HashSet<string> ExcludedFields = new HashSet<string>
        {
            "ApiVersion",
            "AttachmentsHash",
            "CheckHash",
            "ClassHash",
            "DateHash",
            "DescriptionHash",
            "ItemTitle",
            "NumHash"
        };

        private static readonly HashSet<string> HashFields = new HashSet<string>
        {
            "AttachmentsHash",
            "CheckHash",
            "ClassHash",
            "DateHash",
            "DescriptionHash",
            "NumHash"
        };

        private readonly ColumnNameConverter columnConverter;
        private readonly Context context;
        private readonly SiteSettings ss;

        public DisplayTranslator(
            Context context,
            SiteSettings ss)
        {
            this.context = context;
            this.ss = ss;
            this.columnConverter = new ColumnNameConverter(
                context: context,
                ss: ss);
        }

        public string ColumnToLabel(string columnName)
        {
            return columnConverter.ToLabelText(columnName: columnName);
        }

        public string TranslateApiResponse(string apiResponseJson)
        {
            if (string.IsNullOrEmpty(apiResponseJson))
            {
                return apiResponseJson;
            }
            var jsonObject = JObject.Parse(apiResponseJson);
            var responseObject = jsonObject["Response"] as JObject;
            if (responseObject == null)
            {
                return apiResponseJson;
            }
            var dataArray = responseObject["Data"] as JArray;
            if (dataArray == null ||
                !dataArray.Any())
            {
                return apiResponseJson;
            }
            var translatedData = TranslateDataArray(dataArray: dataArray);
            responseObject["Data"] = JArray.FromObject(translatedData);
            return jsonObject.ToString(Formatting.Indented);
        }

        public string TranslateToDisplay(
            string columnName,
            string codeValue)
        {
            if (string.IsNullOrEmpty(codeValue))
            {
                return codeValue;
            }
            var column = ss.GetColumn(
                context: context,
                columnName: columnName);
            if (column == null)
            {
                if (IsDateColumnByName(columnName: columnName))
                {
                    return ToDateDisplayValue(dateValue: codeValue);
                }
                return codeValue;
            }
            return TranslateByColumnType(
                column: column,
                codeValue: codeValue);
        }

        private void EnsureChoiceHash(Column column)
        {
            if (column.ChoiceHash != null &&
                column.ChoiceHash.Count > 0)
            {
                return;
            }
            column.SetChoiceHash(
                context: context,
                siteId: ss.InheritPermission);
        }

        private void ExpandHashFields(
            Dictionary<string, object> result,
            JObject hashObject)
        {
            if (hashObject == null)
            {
                return;
            }
            foreach (var property in hashObject.Properties())
            {
                var columnName = property.Name;
                var value = property.Value;
                var labelText = ColumnToLabel(columnName: columnName);
                object translatedValue;
                if (IsDateColumnByName(columnName: columnName))
                {
                    translatedValue = ToDateDisplayValue(
                        dateValue: value.Value<string>());
                }
                else
                {
                    translatedValue = TranslateValue(
                        columnName: columnName,
                        value: value);
                }
                result[labelText] = translatedValue;
            }
        }

        private static bool IsDateColumn(Column column)
        {
            return column.TypeName == "datetime";
        }

        private static bool IsDateColumnByName(string columnName)
        {
            if (string.IsNullOrEmpty(columnName))
            {
                return false;
            }
            if (DateColumnNames.Contains(columnName))
            {
                return true;
            }
            foreach (var prefix in DateColumnPrefixes)
            {
                if (columnName.StartsWith(prefix) &&
                    columnName.Length > prefix.Length &&
                    char.IsUpper(columnName[prefix.Length]))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsDeptField(Column column)
        {
            return column.ChoicesText?.Contains(DeptsPattern) == true;
        }

        private static bool IsGroupField(Column column)
        {
            return column.ChoicesText?.Contains(GroupsPattern) == true;
        }

        private static bool IsUserField(Column column)
        {
            return column.ChoicesText?.Contains(UsersPattern) == true;
        }

        private static string ToDateDisplayValue(string dateValue)
        {
            if (string.IsNullOrEmpty(dateValue))
            {
                return string.Empty;
            }
            if (!DateTime.TryParse(dateValue, out var dateTime))
            {
                return dateValue;
            }
            if (!dateTime.InRange())
            {
                return string.Empty;
            }
            return dateTime.ToString(DateDisplayFormat);
        }

        private string ToDeptName(string deptId)
        {
            if (string.IsNullOrEmpty(deptId))
            {
                return deptId;
            }
            if (!int.TryParse(deptId, out var id) ||
                id <= 0)
            {
                return deptId;
            }
            var dept = SiteInfo.Dept(
                tenantId: context.TenantId,
                deptId: id);
            if (dept == null ||
                string.IsNullOrEmpty(dept.Name))
            {
                return deptId;
            }
            return dept.Name;
        }

        private string ToDisplayValue(
            Column column,
            string codeValue)
        {
            if (string.IsNullOrEmpty(codeValue))
            {
                return codeValue;
            }
            if (column?.ChoiceHash == null ||
                column.ChoiceHash.Count == 0)
            {
                return codeValue;
            }
            column.AddToChoiceHash(
                context: context,
                value: codeValue);
            var choice = column.Choice(
                selectedValue: codeValue,
                nullCase: codeValue);
            if (choice != null &&
                !string.IsNullOrEmpty(choice.Text))
            {
                return choice.Text;
            }
            return codeValue;
        }

        private string ToDisplayValues(
            Column column,
            string codeValue)
        {
            if (string.IsNullOrEmpty(codeValue))
            {
                return codeValue;
            }
            if (column?.ChoiceHash == null ||
                column.ChoiceHash.Count == 0)
            {
                return codeValue;
            }
            if (column.MultipleSelections == true)
            {
                var values = codeValue.Deserialize<List<string>>();
                if (values != null)
                {
                    var displayValues = values
                        .Select(v => ToDisplayValue(
                            column: column,
                            codeValue: v))
                        .Where(v => !string.IsNullOrEmpty(v))
                        .ToList();
                    return string.Join(", ", displayValues);
                }
            }
            return ToDisplayValue(
                column: column,
                codeValue: codeValue);
        }

        private string ToGroupName(string groupId)
        {
            if (string.IsNullOrEmpty(groupId))
            {
                return groupId;
            }
            if (!int.TryParse(groupId, out var id) ||
                id <= 0)
            {
                return groupId;
            }
            var group = SiteInfo.Group(
                tenantId: context.TenantId,
                groupId: id);
            if (group == null ||
                string.IsNullOrEmpty(group.Name))
            {
                return groupId;
            }
            return group.Name;
        }

        private string ToUserName(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return userId;
            }
            if (!int.TryParse(userId, out var id) ||
                id <= 0)
            {
                return userId;
            }
            var userName = SiteInfo.UserName(
                context: context,
                userId: id,
                notSet: false);
            if (string.IsNullOrEmpty(userName))
            {
                return userId;
            }
            return userName;
        }

        private string TranslateByColumnType(
            Column column,
            string codeValue)
        {
            if (IsDateColumn(column: column))
            {
                return ToDateDisplayValue(dateValue: codeValue);
            }
            if (IsDeptField(column: column))
            {
                return ToDeptName(deptId: codeValue);
            }
            if (IsGroupField(column: column))
            {
                return ToGroupName(groupId: codeValue);
            }
            if (IsUserField(column: column))
            {
                return ToUserName(userId: codeValue);
            }
            if (column.HasChoices())
            {
                return TranslateChoiceValue(
                    column: column,
                    codeValue: codeValue);
            }
            return codeValue;
        }

        private string TranslateChoiceValue(
            Column column,
            string codeValue)
        {
            EnsureChoiceHash(column: column);
            var displayValue = ToDisplayValues(
                column: column,
                codeValue: codeValue);
            if (string.IsNullOrEmpty(displayValue))
            {
                return codeValue;
            }
            return displayValue;
        }

        private List<Dictionary<string, object>> TranslateDataArray(JArray dataArray)
        {
            var result = new List<Dictionary<string, object>>();
            foreach (var item in dataArray)
            {
                if (item is JObject record)
                {
                    var translatedRecord = TranslateRecord(record: record);
                    result.Add(translatedRecord);
                }
            }
            return result;
        }

        private Dictionary<string, object> TranslateRecord(JObject record)
        {
            var result = new Dictionary<string, object>();
            foreach (var property in record.Properties())
            {
                var columnName = property.Name;
                var value = property.Value;
                if (HashFields.Contains(columnName))
                {
                    ExpandHashFields(
                        result: result,
                        hashObject: value as JObject);
                    continue;
                }
                if (ExcludedFields.Contains(columnName))
                {
                    continue;
                }
                var labelText = ColumnToLabel(columnName: columnName);
                var translatedValue = TranslateValue(
                    columnName: columnName,
                    value: value);
                result[labelText] = translatedValue;
            }
            return result;
        }

        private object TranslateValue(
            string columnName,
            JToken value)
        {
            if (value == null ||
                value.Type == JTokenType.Null)
            {
                return null;
            }
            switch (value.Type)
            {
                case JTokenType.String:
                    var stringValue = value.Value<string>();
                    return TranslateToDisplay(
                        columnName: columnName,
                        codeValue: stringValue);
                case JTokenType.Integer:
                    var intValue = value.Value<long>();
                    var translatedInt = TranslateToDisplay(
                        columnName: columnName,
                        codeValue: intValue.ToString());
                    if (translatedInt != intValue.ToString())
                    {
                        return translatedInt;
                    }
                    return intValue;
                case JTokenType.Float:
                    return value.Value<decimal>();
                case JTokenType.Boolean:
                    return value.Value<bool>();
                case JTokenType.Array:
                    return value.ToString();
                case JTokenType.Object:
                    return value.ToString();
                default:
                    return value.ToString();
            }
        }
    }
}
