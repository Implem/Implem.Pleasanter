using Implem.Libraries.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
namespace Implem.Pleasanter.Libraries.Requests
{
    [Serializable]
    public class FormDataValue
    {
        public string Value;
        public DateTime CreatedTime;

        public FormDataValue(string value)
        {
            Value = value;
            CreatedTime = DateTime.Now;
        }
    }

    [Serializable]
    public class FormData : Dictionary<string, FormDataValue>
    {
        public FormData(NameValueCollection nameValueCollection)
        {
            foreach(var key in nameValueCollection.Keys)
            {
                this.Add(key.ToString(), new FormDataValue(nameValueCollection[key.ToString()]));
            }
        }

        protected FormData(
            SerializationInfo serializationInfo, 
            StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {
        }

        public string Get(string key)
        {
            return ContainsKey(key) ? this[key].Value : string.Empty;
        }

        public FormData Update(NameValueCollection nameValueCollection)
        {
            foreach (string key in nameValueCollection.Keys)
            {
                var value = nameValueCollection[key];
                if (ContainsKey(key))
                {
                    if (value != string.Empty)
                    {
                        this[key].Value = value;
                    }
                    else
                    {
                        this.Remove(key);
                    }
                }
                else
                {
                    if (value != string.Empty)
                    {
                        this.Add(key, new FormDataValue(value));
                    }
                }
            }
            return this;
        }

        public FormData RemoveIfEmpty()
        {
            this
                .Where(o => o.Value.Value == string.Empty)
                .Select(o => o.Key)
                .ToList()
                .ForEach(key => this.Remove(key));
            return this;
        }

        public bool Checked(string key)
        {
            return ContainsKey(key) && this[key].Value.ToBool();
        }
    }
}