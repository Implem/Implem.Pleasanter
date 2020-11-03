using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
namespace Implem.Pleasanter.Models
{
    public class ServerScriptValidators
    {
        public static ErrorData OnCreating(Context context, ServerScript serverScript)
        {
            try
            {
                ServerScriptUtilities.Execute(
                    context: context,
                    ss: new SiteSettings
                    {
                        EditorColumnHash = new Dictionary<string, List<string>>(),
                        ColumnHash = new Dictionary<string, Column>()
                    },
                    itemModel: new BaseItemModel(),
                    scripts: new ServerScript[]
                    {
                        serverScript
                    });
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                return new ErrorData(type: Error.Types.IncorrectServerScript, data: ex.Message);
            }
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