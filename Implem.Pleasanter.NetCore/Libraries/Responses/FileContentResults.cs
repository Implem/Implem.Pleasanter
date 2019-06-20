using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Implem.Pleasanter.NetCore.Libraries.Responses
{
    public static class FileContentResults
    {
        public static Microsoft.AspNetCore.Mvc.FileContentResult ToFileContentResult(this System.Web.Mvc.FileContentResult content)
        {
            return new Microsoft.AspNetCore.Mvc.FileContentResult(content.FileContents, content.ContentType) { FileDownloadName = content.FileDownloadName };
        }
    }
}
