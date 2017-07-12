using Microsoft.VisualBasic.FileIO;
using System.Collections.Generic;
using System.IO;
using System.Text;
namespace Implem.Libraries.Classes
{
    public class Csv
    {
        public List<string> Headers = new List<string>();
        public List<List<string>> Rows = new List<List<string>>();

        public Csv(byte[] csv, string encoding)
        {
            switch (encoding)
            {
                case "Shift-JIS": Construct(Encoding.Convert(
                    Encoding.GetEncoding("Shift_JIS"), Encoding.UTF8, csv)); break;
                default: Construct(csv); break;
            }
        }

        private void Construct(byte[] csv)
        {
            var stream = new MemoryStream(csv);
            var parser = new TextFieldParser(stream);
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            var header = true;
            while (parser.EndOfData == false)
            {
                var fields = parser.ReadFields();
                if (header)
                {
                    for (int i = 0; i < fields.Length; i++)
                    {
                        Headers.Add(fields[i]);
                    }
                    header = false;
                }
                else
                {
                    var row = new List<string>();
                    for (int i = 0; i < fields.Length; i++)
                    {
                        row.Add(fields[i]);
                    }
                    while (Headers.Count > row.Count)
                    {
                        row.Add(string.Empty);
                    }
                    Rows.Add(row);
                }
            }
        }
    }
}