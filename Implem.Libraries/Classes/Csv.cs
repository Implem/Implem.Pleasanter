using System.Collections.Generic;
using System.Globalization;
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
                case "Shift-JIS":
                    Construct(Encoding.Convert(
                        Encoding.GetEncoding("Shift_JIS"),
                        Encoding.UTF8, csv));
                    break;
                default:
                    Construct(csv);
                    break;
            }
        }

        private void Construct(byte[] csv)
        {
            using (var stream = new MemoryStream(csv))
            using (var reader = new StreamReader(stream))
            using (var data = new CsvHelper.CsvReader(reader, CultureInfo.CurrentCulture))
            {
                data.Configuration.HasHeaderRecord = false;
                var header = true;
                while (data.Read())
                {
                    if (header)
                    {
                        foreach (var value in data.Context.Record)
                        {
                            Headers.Add(value);
                        }
                        header = false;
                    }
                    else
                    {
                        var row = new List<string>();
                        foreach (var value in data.Context.Record)
                        {
                            row.Add(value);
                        }
                        Rows.Add(row);
                    }
                }
            }
        }
    }
}