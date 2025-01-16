using Microsoft.ClearScript.V8;
using Microsoft.ClearScript;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System;
using System.Linq;

namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptCsv
    {
        public string Csv2Text(
            ScriptObject callback,
            IList<object> csv)
        {
            try
            {
                var csvConfiguration = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.CurrentCulture)
                {
                    HasHeaderRecord = false
                };
                using var writer = new StringWriter();
                using var csvHelper = new CsvHelper.CsvWriter(writer, csvConfiguration);
                foreach (var record in csv)
                {
                    foreach (var value in record as IEnumerable<object>)
                    {
                        csvHelper.WriteField(value.ToString());
                    }
                    csvHelper.NextRecord();
                }
                return writer.ToString();
            }
            catch (Exception e)
            {
                callback.InvokeAsFunction(
                    "Exception",
                    $"{e.GetType().Name}@{e.Message}");
                return null;
            }
        }

        public dynamic Text2Csv(
            ScriptObject callback,
            string text)
        {
            try
            {
                var allAry = new List<object>();
                var csvConfiguration = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.CurrentCulture)
                {
                    HasHeaderRecord = false
                };
                using var reader = new StringReader(text);
                using var data = new CsvHelper.CsvReader(reader, csvConfiguration);
                while (data.Read())
                {
                    allAry.Add(V8ScriptEngine.Current.Script.Array.from(data.Parser.Record.ToArray()));
                }
                return V8ScriptEngine.Current.Script.Array.from(allAry);
            }
            catch (Exception e)
            {
                callback.InvokeAsFunction(
                    "Exception",
                    $"{e.GetType().Name}@{e.Message}");
                return null;
            }
        }

        public string Script()
        {
            return """
                $ps.CSV =
                {
                    str2csv: function(text)
                    {
                        return $ps._utils._f0((cb) => _csv_cs.Text2Csv(cb, text));
                    },
                    csv2str: function(csv)
                    {
                        return $ps._utils._f0((cb) => _csv_cs.Csv2Text(cb, csv));
                    },
                }
                """;
        }
    }
}
