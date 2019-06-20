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

        public XlsIo(string xlsPath)
        {
            Path = xlsPath;
            ReadXls();
        }

        private void ReadXls()
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
