using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Implem.Libraries.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
namespace Implem.Libraries.Classes
{
    public class XlsIo
    {
        public string Path = string.Empty;
        public XlsSheet XlsSheet = new XlsSheet();
        public Files.AccessStatuses AccessStatus = Files.AccessStatuses.Initialized;

        public XlsIo(string path)
        {
            Path = path;
            ReadXls();
        }

        public List<Dictionary<string, string>> DefinitionRows()
        {
            return XlsSheet
                .AsEnumerable()
                .Skip(1)
                .Where(o => o[0].ToString() != string.Empty)
                .Select(o => o.ToDictionary(p => p.Key, p => p.Value))
                .ToList();
        }

        private void ReadXls()
        {
            if (Files.Exists(System.IO.Path.Combine(Path, "Definition.json")))
            {
                XlsSheet = Files.Read(System.IO.Path.Combine(Path, "Definition.json")).Deserialize<XlsSheet>();
                XlsSheet.Columns = XlsSheet[0].Keys.ToList();
            }
            else
            {
                var hash = new Dictionary<string, XlsRow>();
                var dir = new DirectoryInfo(Path);
                var cs = Files.Read(System.IO.Path.Combine(Path, "__ColumnSettings.json")).Deserialize<Dictionary<string, string>>();
                XlsSheet.Columns = cs.Keys.ToList();
                XlsSheet.Add(new XlsRow(cs));
                foreach (var file in dir.GetFiles())
                {
                    if (file.Name.EndsWith("_Body.txt"))
                    {
                        var data = Files.Read(file.FullName);
                        hash[file.Name.Replace("_Body.txt", ".json")]["Body"] = data;
                    }
                    else if (file.Name.EndsWith("_SiteSettingsTemplate.json"))
                    {
                        var data = Files.Read(file.FullName);
                        hash[file.Name.Replace("_SiteSettingsTemplate.json", ".json")]["SiteSettingsTemplate"] = data;
                    }
                    else if (file.Name != "__ColumnSettings.json")
                    {
                        var data = XlsSheet.Columns.ToDictionary(o => o, o => string.Empty);
                        var row = Files.Read(file.FullName).Deserialize<Dictionary<string, string>>();
                        Files.Read(file.FullName).Deserialize<Dictionary<string, string>>()
                            .ForEach(part => data[part.Key] = part.Value.Replace("\n", "\r\n"));
                        var xlsRow = new XlsRow(data);
                        hash.Add(file.Name, xlsRow);
                        XlsSheet.Add(xlsRow);
                    }
                }
            }
            AccessStatus = Files.AccessStatuses.Read;
        }

        private void ReadXlsTemp()
        {
            FileInfo xls = null;
            if (new FileInfo(Path).Exists)
            {
                try
                {
                    xls = new FileInfo(Path);
                    AccessStatus = Files.AccessStatuses.Read;
                }
                catch (Exception e)
                {
                    AccessStatus = Files.AccessStatuses.Failed;
                    Consoles.Write(e.Message, Consoles.Types.Error, abort: true);
                    return;
                }
            }
            else
            {
                AccessStatus = Files.AccessStatuses.NotFound;
                return;
            }
            var document = SpreadsheetDocument.Open(xls.FullName, isEditable: false);
            var workbookPart = document.WorkbookPart;
            var sheet = Sheet(workbookPart);
            if (sheet == null)
            {
                return;
            }
            (workbookPart.GetPartById(sheet.Id) as WorksheetPart).Worksheet.Descendants<Row>()
                .ForEach(row =>
                {
                    if (row.RowIndex == 1)
                    {
                        row.Elements<Cell>().ForEach(xlsCell => XlsSheet.Columns
                            .Add(CellValue(workbookPart, xlsCell)
                            .Replace("\n", "\r\n")));
                    }
                    else
                    {
                        XlsSheet.Add(new XlsRow(row.Elements<Cell>().Select((o, i) =>
                        new
                        {
                            Index = XlsSheet.Columns[i],
                            Value = CellValue(workbookPart, o).Replace("\n", "\r\n")
                        })
                        .ToDictionary(o => o.Index, o => o.Value)));
                    }
                });
            document.Close();
        }

        private static DocumentFormat.OpenXml.Spreadsheet.Sheet Sheet(WorkbookPart workbookPart)
        {
            return workbookPart.Workbook.Descendants<Sheet>()
                .Where(o => o.Name == workbookPart.Workbook.Descendants<Sheet>().ElementAt(0).Name)
                .FirstOrDefault();
        }

        private string CellValue(WorkbookPart workbookPart, Cell cell)
        {
            if (cell == null || cell.CellValue == null) 
            { 
                return String.Empty; 
            }
            if (cell.DataType == null)
            {
                return cell.InnerText.ToString();
            }
            switch(cell.DataType.Value)
            {
                case CellValues.Date:
                case CellValues.Boolean:
                case CellValues.InlineString:
                case CellValues.Number:
                case CellValues.String:
                    return cell.CellValue.InnerText;
                case CellValues.SharedString:
                    return workbookPart.SharedStringTablePart.SharedStringTable
                        .Elements<SharedStringItem>()
                        .ElementAt(int.Parse(cell.InnerText))
                        .Where(o => o.LocalName != "rPh")
                        .Select(o => o.InnerText)
                        .Join(string.Empty);
                case CellValues.Error:
                default:
                    return string.Empty;
            }
        }
    }

    public class XlsSheet : List<XlsRow>
    {
        public List<string> Columns = new List<string>();

        public XlsSheet()
        {
        }

        public XlsSheet(List<XlsRow> xlsRowCollection)
        {
            this.Concat(xlsRowCollection);
        }

        public XlsSheet(List<XlsRow> list, List<string> columns)
        {
            this.AddRange(list);
            Columns = columns;
        }
    }

    [Serializable]
    public class XlsRow : Dictionary<string, string>
    {
        public XlsRow()
        {
        }

        public XlsRow(Dictionary<string, string> data)
        {
            data.ForEach(o => this.Add(o.Key, o.Value));
        }

        protected XlsRow(
            SerializationInfo serializationInfo,
            StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }

        public override void GetObjectData(
            SerializationInfo serializationInfo,
            StreamingContext streamingContext)
        {
            base.GetObjectData(serializationInfo, streamingContext);
        }

        public string this[int index]
        {
            get
            {
                var value = this.Skip(index).FirstOrDefault().Value;
                return value != null ? value : string.Empty;
            }
        }
    }
}
