using Microsoft.AspNetCore.Http;
using System.IO;
namespace Implem.Pleasanter.Libraries.Requests
{
    public class HttpPostedFile : IHttpPostedFile
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
    }
}
