using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.ServerScripts;
using System;
using System.Linq;

namespace Implem.Pleasanter.Libraries.Settings
{
    public class BackgroundServerScriptValidators
    {
        public static ErrorData OnCreating(Context context, BackgroundServerScript script)
        {
            var invalid = ServerScriptValidators.OnCreating(context: context, serverScript: script);
            if (invalid.Type != Error.Types.None) return invalid;
            return Validator(context: context, script: script);
        }


        public static ErrorData OnUpdating(Context context, BackgroundServerScript script)
        {
            var invalid = ServerScriptValidators.OnUpdating(context: context, serverScript: script);
            if (invalid.Type != Error.Types.None) return invalid;
            return Validator(context: context, script: script);
        }

        private static ErrorData Validator(Context context, BackgroundServerScript script)
        {
            if (script.Background != true) return new ErrorData(context: context, type: Error.Types.ParameterSyntaxError);
            if (script.Disabled != true && script.Shared == false && !CanExecuteUser(context: context, script: script))
            {
                return new ErrorData(
                    context: context,
                    type: Error.Types.IncorrectUser,
                    data: new string[] { Displays.ExecutionUser(context: context) });
            }
            return new ErrorData(type: Error.Types.None);
        }

        public static bool CanExecuteUser(Context context, BackgroundServerScript script)
        {
            // ユーザ権限を確認する。
            return SiteInfo.TenantCaches.Get(context.TenantId)
                ?.UserHash
                .Count(o => o.Key == script.UserId && !o.Value.Disabled && o.Value.TenantId == context.TenantId) == 1;
        }
    }
}