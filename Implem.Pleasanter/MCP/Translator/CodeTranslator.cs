using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Implem.Pleasanter.MCP.Translator
{
    public class CodeTranslator
    {
        private const string DeptsPattern = "[[Depts";
        private const string GroupsPattern = "[[Groups";
        private const string UsersPattern = "[[Users";

        private readonly ColumnNameConverter columnConverter;
        private readonly Context context;
        private readonly SiteSettings ss;

        public CodeTranslator(
            Context context,
            SiteSettings ss)
        {
            this.context = context;
            this.ss = ss;
            this.columnConverter = new ColumnNameConverter(
                context: context,
                ss: ss);
        }

        public string LabelToColumn(string labelText)
        {
            return columnConverter.ToColumnName(labelText: labelText);
        }

        public string TranslateToCode(
            string columnName,
            string displayValue)
        {
            if (string.IsNullOrEmpty(displayValue))
            {
                return displayValue;
            }
            var column = ss.GetColumn(
                context: context,
                columnName: columnName);
            if (column == null)
            {
                return displayValue;
            }
            return TranslateByColumnType(
                column: column,
                displayValue: displayValue);
        }

        private static readonly (string Prefix, string HashName)[] HashPrefixes = new[]
        {
            ("Class", "ClassHash"),
            ("Num", "NumHash"),
            ("Date", "DateHash"),
            ("Description", "DescriptionHash"),
            ("Check", "CheckHash"),
            ("Attachments", "AttachmentsHash")
        };

        public string TranslateToCodeString(Dictionary<string, object> data)
        {
            if (data == null)
            {
                return string.Empty;
            }
            var convertedData = ConvertValue(
                columnName: string.Empty,
                value: data);
            if (convertedData is Dictionary<string, object> flatData)
            {
                var groupedData = GroupIntoHashes(data: flatData);
                return JsonConvert.SerializeObject(value: groupedData);
            }
            return JsonConvert.SerializeObject(value: convertedData);
        }

        private static Dictionary<string, object> GroupIntoHashes(
            Dictionary<string, object> data)
        {
            var result = new Dictionary<string, object>();
            var hashGroups = new Dictionary<string, Dictionary<string, object>>();
            foreach (var kvp in data)
            {
                var hashName = GetHashName(columnName: kvp.Key);
                if (hashName != null)
                {
                    if (!hashGroups.TryGetValue(
                        key: hashName,
                        value: out var group))
                    {
                        group = new Dictionary<string, object>();
                        hashGroups[hashName] = group;
                    }
                    group[kvp.Key] = kvp.Value;
                }
                else
                {
                    result[kvp.Key] = kvp.Value;
                }
            }
            foreach (var hashGroup in hashGroups)
            {
                result[hashGroup.Key] = hashGroup.Value;
            }
            return result;
        }

        private static string GetHashName(string columnName)
        {
            if (string.IsNullOrEmpty(columnName))
            {
                return null;
            }
            foreach (var (prefix, hashName) in HashPrefixes)
            {
                if (columnName.StartsWith(prefix)
                    && columnName.Length > prefix.Length)
                {
                    var suffix = columnName.Substring(startIndex: prefix.Length);
                    if (IsExtendedColumnSuffix(suffix: suffix))
                    {
                        return hashName;
                    }
                }
            }
            return null;
        }

        private static bool IsExtendedColumnSuffix(string suffix)
        {
            if (suffix.Length == 1
                && suffix[0] >= 'A'
                && suffix[0] <= 'Z')
            {
                return true;
            }
            if (suffix.Length == 3
                && int.TryParse(s: suffix, result: out var num)
                && num >= 1
                && num <= 100)
            {
                return true;
            }
            return false;
        }

        private Dictionary<string, object> ConvertDictionary(
            Dictionary<string, object> data)
        {
            return data.ToDictionary(
                kvp => LabelToColumn(labelText: kvp.Key),
                kvp =>
                {
                    var columnName = LabelToColumn(labelText: kvp.Key);
                    return ConvertValue(
                        columnName: columnName,
                        value: kvp.Value);
                });
        }

        private List<object> ConvertList(
            string columnName,
            List<object> list)
        {
            return list
                .Select(v => ConvertValue(
                    columnName: columnName,
                    value: v))
                .ToList();
        }

        private object ConvertValue(
            string columnName,
            object value)
        {
            value = Normalize(value: value);
            if (value == null)
            {
                return null;
            }
            switch (value)
            {
                case Dictionary<string, object> dictValue:
                    return ConvertDictionary(data: dictValue);
                case string stringValue:
                    return TranslateToCode(
                        columnName: columnName,
                        displayValue: stringValue);
                case List<object> listValue:
                    return ConvertList(
                        columnName: columnName,
                        list: listValue);
                default:
                    return value;
            }
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

        private static object ExtractJsonElementValue(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.String:
                    return element.GetString();
                case JsonValueKind.Number:
                    if (element.TryGetInt64(out var longValue))
                    {
                        return longValue;
                    }
                    return element.GetDecimal();
                case JsonValueKind.True:
                    return true;
                case JsonValueKind.False:
                    return false;
                case JsonValueKind.Null:
                    return null;
                case JsonValueKind.Array:
                    return element.EnumerateArray()
                        .Select(child => ExtractJsonElementValue(element: child))
                        .ToList();
                case JsonValueKind.Object:
                    return element.EnumerateObject()
                        .ToDictionary(
                            p => p.Name,
                            p => ExtractJsonElementValue(element: p.Value));
                default:
                    return element.GetRawText();
            }
        }

        private static object ExtractJTokenValue(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.String:
                    return token.Value<string>();
                case JTokenType.Integer:
                    return token.Value<long>();
                case JTokenType.Float:
                    return token.Value<decimal>();
                case JTokenType.Boolean:
                    return token.Value<bool>();
                case JTokenType.Null:
                    return null;
                case JTokenType.Array:
                    return token.Children()
                        .Select(child => ExtractJTokenValue(token: child))
                        .ToList();
                case JTokenType.Object:
                    return ((JObject)token).Properties()
                        .ToDictionary(
                            p => p.Name,
                            p => ExtractJTokenValue(token: p.Value));
                default:
                    return token.ToString();
            }
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

        private static object Normalize(object value)
        {
            if (value is JsonElement jsonElement)
            {
                return ExtractJsonElementValue(element: jsonElement);
            }
            if (value is JToken jToken)
            {
                return ExtractJTokenValue(token: jToken);
            }
            return value;
        }

        private string ToDeptId(string deptName)
        {
            if (string.IsNullOrEmpty(deptName))
            {
                return deptName;
            }
            var depts = SiteInfo.TenantCaches
                .Get(context.TenantId)?
                .DeptHash?
                .Values;
            if (depts == null)
            {
                return deptName;
            }
            var dept = depts.FirstOrDefault(d =>
                d.Name == deptName ||
                d.Code == deptName);
            if (dept == null ||
                dept.Id <= 0)
            {
                return deptName;
            }
            return dept.Id.ToString();
        }

        private string ToGroupId(string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                return groupName;
            }
            var groups = SiteInfo.TenantCaches
                .Get(context.TenantId)?
                .GroupHash?
                .Values;
            if (groups == null)
            {
                return groupName;
            }
            var group = groups.FirstOrDefault(g =>
                g.Name == groupName);
            if (group == null ||
                group.Id <= 0)
            {
                return groupName;
            }
            return group.Id.ToString();
        }

        private static string ToRecordingValue(
            Column column,
            string displayValue)
        {
            if (string.IsNullOrEmpty(displayValue))
            {
                return displayValue;
            }
            if (column?.ChoiceHash == null ||
                column.ChoiceHash.Count == 0)
            {
                return displayValue;
            }
            var choice = column.ChoiceHash.Values
                .FirstOrDefault(c =>
                    c.Value == displayValue ||
                    c.Text == displayValue ||
                    c.TextMini == displayValue ||
                    c.CssClass == displayValue);
            if (choice != null)
            {
                return choice.Value;
            }
            return displayValue;
        }

        private static string ToRecordingValues(
            Column column,
            string displayValue)
        {
            if (string.IsNullOrEmpty(displayValue))
            {
                return displayValue;
            }
            if (column?.ChoiceHash == null ||
                column.ChoiceHash.Count == 0)
            {
                return displayValue;
            }
            if (column.MultipleSelections == true)
            {
                var values = displayValue.Deserialize<List<string>>();
                if (values != null)
                {
                    var convertedValues = values
                        .Select(v => ToRecordingValue(
                            column: column,
                            displayValue: v))
                        .Where(v => !string.IsNullOrEmpty(v))
                        .ToList();
                    return JsonConvert.SerializeObject(value: convertedValues);
                }
            }
            return ToRecordingValue(
                column: column,
                displayValue: displayValue);
        }

        private string ToUserId(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return userName;
            }
            var users = SiteInfo.TenantCaches
                .Get(context.TenantId)?
                .UserHash?
                .Values;
            if (users == null)
            {
                return userName;
            }
            var user = users.FirstOrDefault(u =>
                u.Name == userName ||
                SiteInfo.UserName(
                    context: context,
                    userId: u.Id) == userName);
            if (user == null ||
                user.Id <= 0)
            {
                return userName;
            }
            return user.Id.ToString();
        }

        private string TranslateByColumnType(
            Column column,
            string displayValue)
        {
            if (IsDeptField(column: column))
            {
                return ToDeptId(deptName: displayValue);
            }
            if (IsGroupField(column: column))
            {
                return ToGroupId(groupName: displayValue);
            }
            if (IsUserField(column: column))
            {
                return ToUserId(userName: displayValue);
            }
            if (column.HasChoices())
            {
                return TranslateChoiceValue(
                    column: column,
                    displayValue: displayValue);
            }
            return displayValue;
        }

        private string TranslateChoiceValue(
            Column column,
            string displayValue)
        {
            EnsureChoiceHash(column: column);
            var translateValue = ToRecordingValues(
                column: column,
                displayValue: displayValue);
            if (string.IsNullOrEmpty(translateValue))
            {
                return displayValue;
            }
            return translateValue;
        }
    }
}
