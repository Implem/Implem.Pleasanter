using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
namespace Implem.Pleasanter.Libraries.Models
{
    public class JsonConverter
    {
    }

    public class IExportModelConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type objectType)
            => objectType == typeof(IExportModel);

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            if (!jObject.Value<string>("ResultId").IsNullOrEmpty())
            {
                return jObject.ToObject<ResultExportModel>();
            }
            if (!jObject.Value<string>("IssueId").IsNullOrEmpty())
            {
                return jObject.ToObject<IssueExportModel>();
            }
            throw new NotSupportedException();
        }

        public override bool CanWrite => false;

        public override void WriteJson(
            JsonWriter writer,
            object value,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public class DefaultConverer : Newtonsoft.Json.JsonConverter
    {
        public override bool CanRead => false;

        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType)
        {
            return false;
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(
            JsonWriter writer,
            object value,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}