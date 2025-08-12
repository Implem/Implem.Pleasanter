using ClosedXML.Excel;
using Implem.Libraries.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Implem.Pleasanter.Libraries.SiteManagement
{
    internal class Json2XlsxConvertor
    {
        internal class Log
        {
            public enum LogLevel
            {
                Fatal,
                Error,
                Warning,
                Info,
                Debug
            }
            public LogLevel Level;
            public string Section;
            public string Message;
            public long? SiteId;

            public Log() { }

            public Log(LogLevel level, string section, string message, long? siteId = 0)
            {
                Level = level;
                Section = section;
                Message = message;
                SiteId = siteId;
            }
        }

        internal class Param
        {
            public CellStyleManager CellStyleManager;
            public string WorkDir;
            public string ZipFileName;
            public List<Log> Logs = new ();

            public Param()
            {
            }

            public Param(Param param)
            {
                CellStyleManager = new CellStyleManager(param.CellStyleManager);
                WorkDir = param.WorkDir;
                ZipFileName = param.ZipFileName;
            }
        }

        internal class CellStyleManager
        {
            public XLColor HeaderColor { get; set; } = XLColor.FromHtml("#e6f2ff");
            public XLColor LeftEdgeColor { get; set; } = XLColor.FromHtml("#f6faff");
            public XLColor ReadOnlyOrEmptyColor { get; set; } = XLColor.FromHtml("#f2f2f2");
            public XLColor ChangedColor { get; set; } = XLColor.FromHtml("#fffbe6");

            public CellStyleManager()
            {
            }

            public CellStyleManager(JObject styleConfig)
            {
                if (styleConfig != null)
                {
                    ApplyStyleConfig(styleConfig);
                }
            }

            public CellStyleManager(CellStyleManager source)
            {
                HeaderColor = source.HeaderColor;
                LeftEdgeColor = source.LeftEdgeColor;
                ReadOnlyOrEmptyColor = source.ReadOnlyOrEmptyColor;
                ChangedColor = source.ChangedColor;
            }

            public void ApplyStyleConfig(JObject styleConfig)
            {
                if (styleConfig != null)
                {
                    HeaderColor = ParseColor(colorString: styleConfig["HeaderColor"]?.ToString() ?? string.Empty, defaultColor: HeaderColor);
                    LeftEdgeColor = ParseColor(colorString: styleConfig["LeftEdgeColor"]?.ToString() ?? string.Empty, defaultColor: LeftEdgeColor);
                    ReadOnlyOrEmptyColor = ParseColor(colorString: styleConfig["ReadOnlyOrEmptyColor"]?.ToString() ?? string.Empty, defaultColor: ReadOnlyOrEmptyColor);
                    ChangedColor = ParseColor(colorString: styleConfig["ChangedColor"]?.ToString() ?? string.Empty, defaultColor: ChangedColor);
                }
            }

            private XLColor ParseColor(string colorString, XLColor defaultColor)
            {
                if (colorString.IsNullOrEmpty()) return defaultColor;
                try
                {
                    return XLColor.FromHtml(colorString);
                }
                catch
                {
                    return defaultColor;
                }
            }

            public void ApplyHeaderStyle(IXLCell cell)
            {
                cell.Style.Fill.BackgroundColor = HeaderColor;
            }

            public void ApplyLeftEdgeStyle(IXLCell cell)
            {
                cell.Style.Fill.BackgroundColor = LeftEdgeColor;
            }

            public void ApplyReadOnlyOrEmptyStyle(IXLCell cell)
            {
                cell.Style.Fill.BackgroundColor = ReadOnlyOrEmptyColor;
            }

            public void ApplyChangedStyle(IXLCell cell)
            {
                cell.Style.Fill.BackgroundColor = ChangedColor;
            }
        }

        public static bool Convert(
            string jsonText,
            Param param)
        {
            var root = JObject.Parse(jsonText);
            var workDir = Path.Combine(param.WorkDir, Path.GetFileNameWithoutExtension(param.ZipFileName));
            Directory.CreateDirectory(workDir);
            var globalStyleConfig = root["StyleConfig"] as JObject;
            if (globalStyleConfig != null)
            {
                param.CellStyleManager.ApplyStyleConfig(styleConfig: globalStyleConfig);
            }
            bool flowControl = SiteSettingConvertor.Convert(root: root, param: param, workDir: workDir);
            if (!flowControl)
            {
                return false;
            }
            string zipPath = Path.Combine(param.WorkDir, param.ZipFileName);
            if (File.Exists(zipPath)) File.Delete(zipPath);
            ZipFile.CreateFromDirectory(workDir, zipPath);
            return true;
        }

        internal class SiteSettingConvertor
        {
            internal static bool Convert(
                JObject root,
                Param param,
                string workDir)
            {
                var siteSetting = root["SiteSetting"] as JObject;
                var sites = siteSetting?["Sites"] as JArray;
                if (sites == null)
                {
                    param.Logs.Add(new Log(
                        level: Log.LogLevel.Info,
                        section: "Json2Xlsx",
                        message: "Sites node not found"));
                    return false;
                }
                var excelFiles = new List<string>();
                foreach (var site in sites)
                {
                    var siteInfo = site["Info"];
                    var siteId = siteInfo?["SiteId"]?.ToLong() ?? 0;
                    var ver = siteInfo?["Ver"]?.ToInt() ?? 0;
                    string siteTitle = siteInfo?["Title"]?.ToString() ?? "Site";
                    string safeTitle = string.Join("_", siteTitle.Split(Path.GetInvalidFileNameChars()));
                    string excelName = $"{siteId.ToString()?.PadLeft(8, '0') ?? "0"}_{ver.ToString()?.PadLeft(4, '0') ?? "0"}_{safeTitle}.xlsx";
                    string excelPath = Path.Combine(workDir, excelName);
                    var tabs = site["Tabs"] as JObject;
                    if (tabs == null) continue;
                    var siteStyleConfig = site["StyleConfig"] as JObject;
                    var siteParam = new Param(param);
                    if (siteStyleConfig != null)
                    {
                        siteParam.CellStyleManager.ApplyStyleConfig(styleConfig: siteStyleConfig);
                    }
                    using (var workbook = new XLWorkbook())
                    {
                        var sheetNameSet = new HashSet<string>();
                        int globalTableIdx = 1;
                        foreach (var tabProp in tabs.Properties())
                        {
                            string tabName = tabProp.Name;
                            var tabObj = tabProp.Value as JObject;
                            if (tabObj == null) continue;
                            var tablesArr = tabObj["Tables"] as JArray;
                            if (tablesArr == null) continue;
                            foreach (var table in tablesArr)
                            {
                                string label = table["Label"]?.ToString() ?? table["Name"]?.ToString() ?? $"Table{globalTableIdx}";
                                string tableType = table["TableType"]?.ToString() ?? string.Empty;
                                string baseSheetName = $"{tabName}_{label}";
                                string sheetName = baseSheetName;
                                int nameIdx = 1;
                                while (sheetNameSet.Contains(sheetName) || sheetName.Length > 31)
                                {
                                    string suffix = $"_{nameIdx}";
                                    int maxBaseLen = 31 - suffix.Length;
                                    string shortBase = baseSheetName.Length > maxBaseLen ? baseSheetName.Substring(0, maxBaseLen) : baseSheetName;
                                    sheetName = shortBase + suffix;
                                    nameIdx++;
                                }
                                sheetNameSet.Add(sheetName);
                                var ws = workbook.Worksheets.Add(sheetName);
                                try
                                {
                                    if (tableType == "TwoLineHeaderTable")
                                    {
                                        ProcessTwoLineHeaderTable(
                                            ws: ws,
                                            table: table,
                                            param: siteParam,
                                            siteId: siteId);
                                    }
                                    else if (tableType == "KeyValue")
                                    {
                                        ProcessKeyValueTable(
                                            ws: ws,
                                            table: table,
                                            param: siteParam,
                                            siteId: siteId);
                                    }
                                    else if (tableType == "List" || tableType == "OneLineHeaderTable")
                                    {
                                        ProcessListTable(
                                            ws: ws,
                                            table: table,
                                            param: siteParam,
                                            siteId: siteId);
                                    }
                                    else
                                    {
                                        ProcessGenericTable(
                                            ws: ws,
                                            table: table,
                                            param: siteParam,
                                            siteId: siteId);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    param.Logs.Add(new Log(
                                        level: Log.LogLevel.Error,
                                        section: "Json2Xlsx",
                                        message: $"[Table output error] {sheetName}: {ex.Message}",
                                        siteId: siteId));
                                }
                                globalTableIdx++;
                            }
                        }
                        workbook.SaveAs(excelPath);
                        excelFiles.Add(excelPath);
                    }
                }
                return true;
            }

            static void ProcessTwoLineHeaderTable(
                IXLWorksheet ws,
                JToken table,
                Param param,
                long siteId)
            {
                var styleManager = param.CellStyleManager;
                var headerLabels = table["Header"]?["Labels"] as JArray;
                var columns = table["Columns"] as JArray;
                if (headerLabels == null || columns == null)
                {
                    param.Logs.Add(new Log(
                        level: Log.LogLevel.Info,
                        section: "Json2Xlsx",
                        message: "[ViewFilter] Header.Labels or Columns not found",
                        siteId: siteId));
                    return;
                }
                var groups = new List<ViewFilterGroup>();
                var readOnlyKeys = new HashSet<string>();
                foreach (var tab in headerLabels)
                {
                    var tabObj = tab as JObject;
                    if (tabObj?["TabName"] == null || tabObj["Labels"] == null) continue;
                    var tabName = tabObj["TabName"]?["Value"]?.ToString() ?? string.Empty;
                    var labels = tabObj["Labels"] as JArray;
                    if (labels == null) continue;
                    var group = new ViewFilterGroup
                    {
                        GroupName = tabName,
                        Keys = new List<string>(),
                        Headers = new List<string>()
                    };
                    foreach (var label in labels)
                    {
                        var labelObj = label as JObject;
                        var key = labelObj?["Key"]?.ToString() ?? string.Empty;
                        var text = labelObj?["Text"]?.ToString() ?? key;
                        var readOnly = labelObj?["ReadOnly"]?.ToObject<bool>() == true;
                        group.Keys.Add(key);
                        group.Headers.Add(text);
                        if (readOnly)
                        {
                            readOnlyKeys.Add(key);
                        }
                    }
                    groups.Add(group);
                }
                int headerColIdx = 1;
                foreach (var group in groups)
                {
                    var cell = ws.Cell(1, headerColIdx);
                    cell.Value = group.GroupName;
                    styleManager.ApplyHeaderStyle(cell);
                    if (group.Keys.Count > 1)
                    {
                        ws.Range(1, headerColIdx, 1, headerColIdx + group.Keys.Count - 1).Merge();
                    }
                    for (int i = 0; i < group.Headers.Count; i++)
                    {
                        var cell2 = ws.Cell(2, headerColIdx + i);
                        cell2.Value = group.Headers[i];
                        styleManager.ApplyHeaderStyle(cell2);
                    }
                    headerColIdx += group.Keys.Count;
                }
                int dataRowOffset = 2;
                int currentRow = dataRowOffset + 1;
                for (int rowIdx = 0; rowIdx < columns.Count; rowIdx++)
                {
                    var col = columns[rowIdx] as JObject;
                    if (col == null) continue;
                    var changedColumns = col["ChangedColumns"] as JArray;
                    var changedSet = new HashSet<string>();
                    if (changedColumns != null)
                    {
                        foreach (var c in changedColumns) changedSet.Add(c.ToString());
                    }
                    var readOnlyColumns = col["ReadOnlyColumns"] as JArray;
                    var readOnlySet = new HashSet<string>();
                    if (readOnlyColumns != null)
                    {
                        foreach (var c in readOnlyColumns) readOnlySet.Add(c.ToString());
                    }
                    var maxNestRows = CalculateMaxNestRows(col: col, groups: groups);
                    for (int nestRowIdx = 0; nestRowIdx < maxNestRows; nestRowIdx++)
                    {
                        int dataColIdx = 1;
                        foreach (var group in groups)
                        {
                            foreach (var key in group.Keys)
                            {
                                var cell = ws.Cell(currentRow + nestRowIdx, dataColIdx);
                                var value = GetNestedValue(col: col, key: key, nestRowIdx: nestRowIdx);
                                cell.Value = value ?? string.Empty;
                                bool isChanged = false;
                                if (key.Contains("."))
                                {
                                    var parts = key.Split('.');
                                    var parentKey = parts[0];
                                    var childKey = parts[1];
                                    var parentArray = col[parentKey] as JArray;
                                    if (parentArray != null && nestRowIdx < parentArray.Count)
                                    {
                                        var parentObj = parentArray[nestRowIdx] as JObject;
                                        var changedColumnsNest = parentObj?["ChangedColumns"] as JArray;
                                        if (changedColumnsNest != null && changedColumnsNest.Any(c => c.ToString() == childKey))
                                        {
                                            isChanged = true;
                                        }
                                    }
                                }
                                else
                                {
                                    isChanged = changedSet.Contains(key);
                                }
                                bool isReadOnly = false;
                                if (key.Contains("."))
                                {
                                    var parts = key.Split('.');
                                    var parentKey = parts[0];
                                    var childKey = parts[1];
                                    var parentArray = col[parentKey] as JArray;
                                    if (parentArray != null && nestRowIdx < parentArray.Count)
                                    {
                                        var parentObj = parentArray[nestRowIdx] as JObject;
                                        var readOnlyColumnsNest = parentObj?["ReadOnlyColumns"] as JArray;
                                        if (readOnlyColumnsNest != null && readOnlyColumnsNest.Any(c => c.ToString() == childKey))
                                        {
                                            isReadOnly = true;
                                        }
                                    }
                                }
                                isReadOnly = isReadOnly || readOnlyKeys.Contains(key) || readOnlySet.Contains(key);
                                ApplyCellStyle(
                                    cell: cell,
                                    colIdx: dataColIdx,
                                    value: value ?? string.Empty,
                                    isReadOnly: isReadOnly,
                                    isChanged: isChanged,
                                    param);
                                dataColIdx++;
                            }
                        }
                    }
                    currentRow += maxNestRows;
                }
            }

            static string GetNestedValue(
                JObject col,
                string key,
                int nestRowIdx)
            {
                if (key.Contains("."))
                {
                    var parts = key.Split('.');
                    var parentKey = parts[0];
                    var childKey = parts[1];
                    var parentArray = col[parentKey] as JArray;
                    if (parentArray != null && nestRowIdx < parentArray.Count)
                    {
                        var parentObj = parentArray[nestRowIdx] as JObject;
                        if (parentObj != null)
                        {
                            var value = parentObj[childKey];
                            return value?.ToString() ?? string.Empty;
                        }
                    }
                    return string.Empty;
                }
                else
                {
                    var value = col[key];
                    return value?.ToString() ?? string.Empty;
                }
            }

            static int CalculateMaxNestRows(
                JObject col,
                List<ViewFilterGroup> groups)
            {
                int maxRows = 1;
                foreach (var group in groups)
                {
                    foreach (var key in group.Keys)
                    {
                        if (key.Contains("."))
                        {
                            var parentKey = key.Split('.')[0];
                            var parentArray = col[parentKey] as JArray;
                            if (parentArray != null)
                            {
                                maxRows = Math.Max(maxRows, parentArray.Count);
                            }
                        }
                    }
                }
                return maxRows;
            }

            static void ApplyCellStyle(
                IXLCell cell,
                int colIdx,
                string value,
                bool isReadOnly,
                bool isChanged,
                Param param)
            {
                var styleManager = param.CellStyleManager;
                if (colIdx == 1)
                {
                    styleManager.ApplyLeftEdgeStyle(cell: cell);
                }
                else if (isReadOnly || value.IsNullOrEmpty())
                {
                    styleManager.ApplyReadOnlyOrEmptyStyle(cell: cell);
                }
                else if (isChanged)
                {
                    styleManager.ApplyChangedStyle(cell: cell);
                }
            }

            static void ProcessKeyValueTable(
                IXLWorksheet ws,
                JToken table,
                Param param,
                long siteId)
            {
                var styleManager = param.CellStyleManager;
                var headerLabels = table["Header"]?["Labels"] as JArray;
                var columns = table["Columns"] as JArray;
                if (headerLabels != null)
                {
                    for (int i = 0; i < headerLabels.Count; i++)
                    {
                        var h = headerLabels[i];
                        string headerText = h.Type == JTokenType.Object
                            ? h["Text"]?.ToString() ?? h["Key"]?.ToString() ?? h.ToString()
                            : h.ToString();
                        var cell = ws.Cell(1, i + 1);
                        cell.Value = headerText;
                        styleManager.ApplyHeaderStyle(cell: cell);
                    }
                }
                if (columns != null)
                {
                    for (int i = 0; i < columns.Count; i++)
                    {
                        var col = columns[i] as JObject;
                        if (col == null) continue;
                        var cell0 = ws.Cell(i + 2, 1);
                        cell0.Value = col["Label"]?.ToString() ?? string.Empty;
                        styleManager.ApplyLeftEdgeStyle(cell0);
                        var cell1 = ws.Cell(i + 2, 2);
                        var value = col["Value"]?.ToString() ?? string.Empty;
                        cell1.Value = value;
                        if (col["ReadOnly"]?.ToObject<bool>() == true)
                        {
                            styleManager.ApplyReadOnlyOrEmptyStyle(cell1);
                        }
                        else if (col["Changed"]?.ToObject<bool>() == true)
                        {
                            styleManager.ApplyChangedStyle(cell1);
                        }
                        else if (string.IsNullOrEmpty(value))
                        {
                            styleManager.ApplyReadOnlyOrEmptyStyle(cell1);
                        }
                    }
                }
            }

            static void ProcessListTable(
                IXLWorksheet ws,
                JToken table,
                Param param,
                long siteId)
            {
                var styleManager = param.CellStyleManager;
                string tableType = table["TableType"]?.ToString() ?? string.Empty;
                var headerLabels = table["Header"]?["Labels"] as JArray;
                var columns = table["Columns"] as JArray;
                if (headerLabels != null)
                {
                    if (headerLabels.Count > 0 && headerLabels[0] is JObject obj && obj["TabName"] != null)
                    {
                        int colIdx = 1;
                        foreach (var tab in headerLabels)
                        {
                            var tabObj = tab as JObject;
                            var tabName = tabObj["TabName"]?["Value"]?.ToString() ?? string.Empty;
                            var labels = tabObj["Labels"] as JArray;
                            int tabColCount = labels?.Count ?? 0;
                            if (tabColCount == 0) continue;
                            var cell = ws.Cell(1, colIdx);
                            cell.Value = tabName;
                            styleManager.ApplyHeaderStyle(cell);
                            if (tabColCount > 1)
                            {
                                ws.Range(1, colIdx, 1, colIdx + tabColCount - 1).Merge();
                            }
                            for (int i = 0; i < tabColCount; i++)
                            {
                                var l = labels[i];
                                string headerText = l["Text"]?.ToString() ?? l["Key"]?.ToString() ?? l.ToString();
                                var cell2 = ws.Cell(2, colIdx + i);
                                cell2.Value = headerText;
                                styleManager.ApplyHeaderStyle(cell2);
                            }
                            colIdx += tabColCount;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < headerLabels.Count; i++)
                        {
                            var h = headerLabels[i];
                            string headerText = h.Type == JTokenType.Object ? h["Text"]?.ToString() ?? h["Key"]?.ToString() ?? h.ToString() : h.ToString();
                            var cell = ws.Cell(1, i + 1);
                            cell.Value = headerText;
                            styleManager.ApplyHeaderStyle(cell);
                        }
                    }
                }
                if (columns != null)
                {
                    int dataRowOffset = (headerLabels != null && headerLabels.Count > 0 && headerLabels[0] is JObject obj && obj["TabName"] != null) ? 2 : 1;
                    for (int i = 0; i < columns.Count; i++)
                    {
                        var col = columns[i] as JObject;
                        if (col == null) continue;
                        var rowChanged = col["ChangedColumns"] as JArray;
                        var changedSet = new HashSet<string>();
                        if (rowChanged != null)
                        {
                            foreach (var c in rowChanged) changedSet.Add(c.ToString());
                        }
                        var rowReadOnly = col["ReadOnlyColumns"] as JArray;
                        var readOnlySet = new HashSet<string>();
                        if (rowReadOnly != null)
                        {
                            foreach (var c in rowReadOnly) readOnlySet.Add(c.ToString());
                        }
                        int colIdx = 1;
                        if (headerLabels != null && headerLabels.Count > 0 && headerLabels[0] is JObject obj2 && obj2["TabName"] != null)
                        {
                            foreach (var tab in headerLabels)
                            {
                                var tabObj = tab as JObject;
                                var labels = tabObj["Labels"] as JArray;
                                foreach (var l in labels)
                                {
                                    string key = l["Key"]?.ToString() ?? l.ToString();
                                    var cell = ws.Cell(i + dataRowOffset + 1, colIdx);
                                    var value = col[key]?.ToString() ?? string.Empty;
                                    cell.Value = value;
                                    if (colIdx == 1)
                                    {
                                        styleManager.ApplyLeftEdgeStyle(cell);
                                    }
                                    else if (col[key + "_ReadOnly"]?.ToObject<bool>() == true || readOnlySet.Contains(key) || col["ReadOnly"]?.ToObject<bool>() == true)
                                    {
                                        styleManager.ApplyReadOnlyOrEmptyStyle(cell);
                                    }
                                    else if (col[key + "_Changed"]?.ToObject<bool>() == true || changedSet.Contains(key) || col["Changed"]?.ToObject<bool>() == true)
                                    {
                                        styleManager.ApplyChangedStyle(cell);
                                    }
                                    else if (string.IsNullOrEmpty(value))
                                    {
                                        styleManager.ApplyReadOnlyOrEmptyStyle(cell);
                                    }
                                    colIdx++;
                                }
                            }
                        }
                        else
                        {
                            for (int j = 0; j < (headerLabels?.Count ?? 0); j++)
                            {
                                var h = headerLabels[j];
                                string key = h.Type == JTokenType.Object ? h["Key"]?.ToString() ?? h["Value"]?.ToString() ?? h.ToString() : h.ToString();
                                var cell = ws.Cell(i + dataRowOffset + 1, j + 1);
                                var value = col[key]?.ToString() ?? string.Empty;
                                cell.Value = value;
                                if (j == 0)
                                {
                                    styleManager.ApplyLeftEdgeStyle(cell);
                                }
                                else if (col[key + "_ReadOnly"]?.ToObject<bool>() == true || readOnlySet.Contains(key) || col["ReadOnly"]?.ToObject<bool>() == true)
                                {
                                    styleManager.ApplyReadOnlyOrEmptyStyle(cell);
                                }
                                else if (col[key + "_Changed"]?.ToObject<bool>() == true || changedSet.Contains(key) || col["Changed"]?.ToObject<bool>() == true)
                                {
                                    styleManager.ApplyChangedStyle(cell);
                                }
                                else if (string.IsNullOrEmpty(value))
                                {
                                    styleManager.ApplyReadOnlyOrEmptyStyle(cell);
                                }
                            }
                        }
                    }
                }
            }

            static void ProcessGenericTable(
                IXLWorksheet ws,
                JToken table,
                Param param,
                long siteId)
            {
                var styleManager = param.CellStyleManager;
                var headerLabels = table["Header"]?["Labels"] as JArray;
                var columns = table["Columns"] as JArray;
                if (headerLabels != null)
                {
                    for (int i = 0; i < headerLabels.Count; i++)
                    {
                        var h = headerLabels[i];
                        string headerText = h.Type == JTokenType.Object
                            ? h["Text"]?.ToString() ?? h["Key"]?.ToString() ?? h.ToString()
                            : h.ToString();
                        var cell = ws.Cell(1, i + 1);
                        cell.Value = headerText;
                        styleManager.ApplyHeaderStyle(cell);
                    }
                }
                if (columns != null)
                {
                    for (int i = 0; i < columns.Count; i++)
                    {
                        var col = columns[i];
                        if (col is JArray arr)
                        {
                            for (int j = 0; j < arr.Count; j++)
                            {
                                var cell = ws.Cell(i + 2, j + 1);
                                cell.Value = arr[j]?.ToString() ?? string.Empty;
                            }
                        }
                        else
                        {
                            var cell = ws.Cell(i + 2, 1);
                            cell.Value = col?.ToString() ?? string.Empty;
                        }
                    }
                }
            }

            class ViewFilterGroup
            {
                public string GroupName { get; set; } = string.Empty;
                public List<string> Keys { get; set; } = new List<string>();
                public List<string> Headers { get; set; } = new List<string>();
            }
        }
    }
}
