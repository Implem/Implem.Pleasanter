using Implem.Libraries.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace Implem.Pleasanter.Libraries.Requests
{
    public class Forms : Dictionary<string, string>
    {
        public bool Exists(string key)
        {
            return this.Get(key) != null;
        }

        public string Data(string key)
        {
            return this.Get(key) ?? string.Empty;
        }

        public string String()
        {
            return HttpUtility.UrlDecode(HttpContext.Current.Request.Form.ToString(), System.Text.Encoding.UTF8);
        }

        public string ControlId()
        {
            return Data("ControlId");
        }

        public bool Bool(string key)
        {
            return Data(key).ToBool();
        }

        public int Int(string key)
        {
            return Data(key).ToInt();
        }

        public long Long(string key)
        {
            return Data(key).ToLong();
        }

        public decimal Decimal(Context context, string key)
        {
            return Data(key).ToDecimal(context.CultureInfo());
        }

        public DateTime DateTime(string key)
        {
            return Data(key).ToDateTime();
        }

        public List<int> IntList(string name)
        {
            return Data(name)
                .Deserialize<List<int>>()?
                .ToList() ?? new List<int>();
        }

        public List<long> LongList(string name)
        {
            return Data(name)
                .Deserialize<List<long>>()?
                .ToList() ?? new List<long>();
        }

        public List<string> List(string name)
        {
            return Data(name)
                .Deserialize<List<string>>()?
                .Where(o => o != string.Empty)
                .ToList() ?? new List<string>();
        }
    }
}