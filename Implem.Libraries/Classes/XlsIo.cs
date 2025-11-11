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

        public XlsIo(string path, string name)
        {
            Path = path;
            ReadXls(name: name);
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

        private void ReadXls(string name)
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
                ReadDefinitionFiles(
                    name: name,
                    dir: dir,
                    hash: hash);
            }
            AccessStatus = Files.AccessStatuses.Read;
        }

        private void ReadDefinitionFiles(
            string name,
            DirectoryInfo dir,
            Dictionary<string, XlsRow> hash) 
        {
            foreach (var file in dir.GetFiles().OrderBy(o => o.Name.FileNameOnly()))
            {
                if (file.Name.EndsWith("_Body.txt"))
                {
                    var data = Files.Read(file.FullName);
                    hash[file.Name.Replace("_Body.txt", ".json")]["Body"] = data;
                }
                else if (name == "Demo" && file.Name.EndsWith("_Body.json"))
                {
                    var data = Files.Read(file.FullName);
                    hash[file.Name.Replace("_Body.json", ".json")]["Body"] = data;
                }
                else if (file.Name.EndsWith("_SiteSettingsTemplate.json"))
                {
                    var data = Files.Read(file.FullName);
                    hash[file.Name.Replace("_SiteSettingsTemplate.json", ".json")]["SiteSettingsTemplate"] = data;
                }
                else if (file.Name != "__ColumnSettings.json")
                {
                    var data = XlsSheet.Columns.ToDictionary(o => o, o => string.Empty);
                    Files.Read(file.FullName).Deserialize<Dictionary<string, string>>()
                        .ForEach(part => data[part.Key] = part.Value.Replace("\n", "\r\n"));
                    if (data.ContainsKey("RepeatType") && data["RepeatType"] == "Table")
                    {
                        // QRTZ系のテーブルを除外設定に追加
                        string[] qrtzExcluce = 
                        {
                            "QRTZ_BLOB_TRIGGERS",
                            "QRTZ_CALENDARS",
                            "QRTZ_CRON_TRIGGERS",
                            "QRTZ_FIRED_TRIGGERS",
                            "QRTZ_JOB_DETAILS",
                            "QRTZ_LOCKS",
                            "QRTZ_PAUSED_TRIGGER_GRPS",
                            "QRTZ_SCHEDULER_STATE",
                            "QRTZ_SIMPLE_TRIGGERS",
                            "QRTZ_SIMPROP_TRIGGERS",
                            "QRTZ_TRIGGERS",
                        };
                        var exclude = (data.ContainsKey("Exclude") ? data["Exclude"] : "").Split(',');
                        exclude = exclude.Concat(qrtzExcluce).ToArray();
                        data["Exclude"] = string.Join(",", exclude);
                    }
                    var xlsRow = new XlsRow(data);
                    hash.Add(file.Name, xlsRow);
                    XlsSheet.Add(xlsRow);
                }
            }
            foreach (var subdir in dir.GetDirectories())
            {
                ReadDefinitionFiles(
                    name: name,
                    dir: subdir,
                    hash: hash);
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
            AddRange(list);
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
            data.ForEach(o => Add(o.Key, o.Value));
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
