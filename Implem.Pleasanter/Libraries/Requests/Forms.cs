using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
namespace Implem.Pleasanter.Libraries.Requests
{
    public static class Forms
    {
        public static Dictionary<string, string> All()
        {
            var hash = new Dictionary<string, string>();
            HttpContext.Current.Request.Form.AllKeys.ToList().ForEach(key =>
                hash.Add(key, HttpContext.Current.Request.Form[key]));
            return hash;
        }

        public static bool Bool(string key)
        {
            return Data(key).ToBool();
        }

        public static int Int(string key)
        {
            return Data(key).ToInt();
        }

        public static long Long(string key)
        {
            return Data(key).ToLong();
        }

        public static decimal Decimal(string key)
        {
            return Data(key).ToDecimal(Sessions.CultureInfo());
        }

        public static DateTime DateTime(string key)
        {
            return Data(key).ToDateTime();
        }

        public static IEnumerable<long> LongList(string name)
        {
            return Forms.Data(name)
                .Split(',')
                .Select(o => o.ToLong())
                .Distinct();
        }

        public static IEnumerable<string> Keys()
        {
            foreach (var key in HttpContext.Current.Request.Form.Keys)
            {
                yield return key.ToString();
            }
        }

        public static IEnumerable<string> FileKeys()
        {
            foreach (var key in HttpContext.Current.Request.Files.Keys)
            {
                yield return key.ToString();
            }
        }

        public static bool Exists(string key)
        {
            return HttpContext.Current.Request.Form[key] != null;
        }

        public static string Data(string key)
        {
            if (HttpContext.Current.Request.Form[key] != null)
            {
                return HttpContext.Current.Request.Form[key];
            }
            else
            {
                return string.Empty;
            }
        }

        public static byte[] File(string key)
        {
            var file = HttpContext.Current.Request.Files[key];
            if (file != null)
            {
                var bin = new byte[file.ContentLength];
                file.InputStream.Read(bin, 0, file.ContentLength);
                return bin;
            }
            else
            {
                return null;
            }
        }

        public static string TextFile(string key, Encoding encoding = null)
        {
            var file = File(key);
            return file != null
                ? (encoding ?? Encoding.UTF8).GetString(file)
                : null;
        }

        public static bool HasData(string key)
        {
            return HttpContext.Current.Request.Form[key] != null;
        }
    }
}