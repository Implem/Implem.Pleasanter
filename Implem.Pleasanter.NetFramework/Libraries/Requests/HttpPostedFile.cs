using Implem.Pleasanter.Libraries.Requests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Implem.Pleasanter.NetFramework.Libraries.Requests
{
    class HttpPostedFile : IHttpPostedFile
    {
        HttpPostedFileBase _file;

        public HttpPostedFile(HttpPostedFileBase file)
        {
            _file = file;
        }

        public string FileName { get { return _file.FileName; } }

        public long ContentLength { get { return _file.ContentLength; } }

        public string ContentType { get { return _file.ContentType; } }

        public Stream InputStream() { return _file.InputStream; }

        public void SaveAs(string filename) { _file.SaveAs(filename); }

        public static IHttpPostedFile[] Create(HttpPostedFileBase[] files)
        {
            return files.Select(mfile => new HttpPostedFile(mfile)).ToArray();
        }
    }
}