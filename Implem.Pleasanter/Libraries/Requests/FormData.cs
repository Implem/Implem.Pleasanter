using Implem.Libraries.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
namespace Implem.Pleasanter.Libraries.Requests
{
    [Serializable]
    public class FormData : Dictionary<string, string>
    {
        public FormData(NameValueCollection nameValueCollection)
        {
            foreach(var key in nameValueCollection.Keys)
            {
                this.Add(key.ToString(), nameValueCollection[key.ToString()]);
            }
        }

        protected FormData(
            SerializationInfo serializationInfo, 
            StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {
        }

        public string Get(string key)
        {
            return ContainsKey(key) ? this[key] : string.Empty;
        }

        public FormData Update(NameValueCollection nameValueCollection)
        {
            foreach (var key in nameValueCollection.Keys)
            {
                var value = nameValueCollection[key.ToString()];
                if (ContainsKey(key.ToString()))
                {
                    this[key.ToString()] = value;
                }
                else
                {
                    this.Add(key.ToString(), value);
                }
            }
            return this;
        }

        public FormData RemoveEmpty()
        {
            this
                .Where(o => o.Value == string.Empty)
                .Select(o => o.Key)
                .ToList()
                .ForEach(key => this.Remove(key));
            return this;
        }

        public bool Checked(string key)
        {
            return ContainsKey(key) && this[key].ToBool();
        }
    }
}