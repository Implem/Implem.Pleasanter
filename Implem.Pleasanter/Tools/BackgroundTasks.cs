using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
using System;
using System.Threading;
namespace Implem.Pleasanter.Tools
{
    public static class BackgroundTasks
    {
        public static string Do()
        {
            var now = DateTime.Now;
            while ((DateTime.Now - now).Seconds <= Parameters.General.AdminTasksDoSpan)
            {
                SysLogsUtility.Maintenance();
                ItemsUtility.Maintenance();
                Thread.Sleep(100);
            }
            return new ResponseCollection().ToJson();
        }
    }
}
