using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Scripts;
using Implem.Pleasanter.Models;
namespace Implem.Pleasanter.Libraries.Views
{
    public static class HtmlScripts
    {
        public static HtmlBuilder Scripts(
            this HtmlBuilder hb,
            BaseModel.MethodTypes methodType,
            string script,
            string modelName,
            bool allowAccess)
        {
            return hb
                .Script(src: Navigations.Get("Scripts/Plugins/jquery-2.1.4.min.js"))
                .Script(src: Navigations.Get("Scripts/Plugins/jquery-ui.min.js"))
                .Script(src: Navigations.Get("Scripts/Plugins/jquery.validate.min.js"))
                .Script(src: Navigations.Get("Scripts/Plugins/d3.min.js"))
                .Script(script: script, _using: script != string.Empty)
                .Validator(methodType: methodType, modelName: modelName, allowAccess: allowAccess);
        }

        private static HtmlBuilder Validator(
            this HtmlBuilder hb,
            BaseModel.MethodTypes methodType,
            string modelName,
            bool allowAccess)
        {
            return Editor(methodType) && allowAccess
                ? hb.Script(src: JavaScripts.Validator(modelName))
                : hb;
        }

        private static bool Editor(BaseModel.MethodTypes methodType)
        {
            return 
                methodType == BaseModel.MethodTypes.Edit ||
                methodType == BaseModel.MethodTypes.New;
        }
    }
}