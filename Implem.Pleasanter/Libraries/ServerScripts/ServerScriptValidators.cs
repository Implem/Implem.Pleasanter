using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptValidators
    {
        public static ErrorData OnCreating(Context context, ServerScript serverScript)
        {
            return new ErrorData(type: Error.Types.None);
        }

        public static ErrorData OnUpdating(Context context, ServerScript serverScript)
        {
            return OnCreating(
                context: context,
                serverScript: serverScript);
        }
    }
}