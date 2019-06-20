using Implem.Pleasanter.Libraries.Requests;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Implem.Pleasanter.NetCore.Libraries.Requests
{
    class HttpPostedFile : IHttpPostedFile
    {
        IFormFile _file;

        public HttpPostedFile(IFormFile file)
        {
            _file = file;
        }

        public string FileName { get { return _file.FileName; } }

        public long ContentLength { get { return _file.Length; } }

        public string ContentType { get { return _file.ContentType; } }

        public Stream InputStream() { return _file.OpenReadStream(); }

        public void SaveAs(string filename)
        {
            using (var stream = new FileStream(filename, FileMode.Create))
                _file.CopyTo(stream);
        }

        public static IHttpPostedFile[] Create(ICollection<IFormFile> files)
        {
            return files.Select(mfile => new HttpPostedFile(mfile)).ToArray();
        }
    }
}
