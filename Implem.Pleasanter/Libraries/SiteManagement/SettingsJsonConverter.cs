using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Requests;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Implem.Pleasanter.Libraries.SiteManagement
{
    public partial class SettingsJsonConverter
    {
        public class Log
        {
            public enum LogLevel
            {
                Fatal,
                Error,
                Warning,
                Info,
                Debug
            }
            [JsonConverter(typeof(StringEnumConverter))]
            public LogLevel Level;
            public string Section;
            public string Message;
            public long? SiteId;

            public Log() { }

            public Log(LogLevel level, string section, string message)
            {
                Level = level;
                Section = section;
                Message = message;
            }
        }

        public class Param
        {
            public List<SelectedSite> SelectedSites;
            public List<Log> Logs = new ();
            public string[] Types;
            public int ErdLinkDepth = Math.Clamp(Parameters.PleasanterExtensions.SiteVisualizer.ErdLinkDepth, 0, 100);
            public int ErdLinkLimit = Math.Clamp(Parameters.PleasanterExtensions.SiteVisualizer.ErdLinkLimit, 1, 200);
        }

        public class SelectedSite : IEquatable<SelectedSite>
        {
            public long SiteId;
            public int Ver;

            public bool Equals(SelectedSite other)
            {
                return SiteId.Equals(other.SiteId) && Ver.Equals(other.Ver);
            }

            public override int GetHashCode()
            {
                return SiteId.GetHashCode() ^ Ver.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as SelectedSite);
            }
        }

        public class FileInfoData
        {
            public DateTime CreateDate = DateTime.Now;
            public List<Log> Logs;
        }

        public FileInfoData Info;
        public SiteSettingData SiteSetting;
        public EntityRelationshipDiagramsData ERDiagrams;

        public static SettingsJsonConverter Convert(
            Context context,
            Param param)
        {
            var converter = new SettingsJsonConverter();
            var info = new FileInfoData();
            converter.Info = info;
            if (param.Types.Contains("sitesetting"))
            {
                converter.SiteSetting = SettingsJsonConverter.ConvertSiteSetting(context: context, param: param);
            }
            if (param.Types.Contains("erd"))
            {
                converter.ERDiagrams = SettingsJsonConverter.ConvertERDiagrams(context: context, param: param);
            }
            if ((converter.Info.Logs?.Count ?? 0) == 0) converter.Info.Logs = null;
            return converter;
        }

        public string RecordingJson(Context context)
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
            };
            return JsonConvert.SerializeObject(this, settings);
        }

        public static SettingsJsonConverter Deserialize(string json)
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
            };
            return JsonConvert.DeserializeObject<SettingsJsonConverter>(json, settings);
        }
    }
}
