using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Implem.Pleasanter.Libraries.Requests
{
    public interface IHttpPostedFile
    {
        string FileName { get; }
        Stream InputStream();
        void SaveAs(string filename);
        long ContentLength { get; }
        string ContentType { get; }
    }
}
