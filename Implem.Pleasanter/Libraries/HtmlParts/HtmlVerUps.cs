using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlVerUps
    {
        public static HtmlBuilder VerUpCheckBox(
            this HtmlBuilder hb, Context context, SiteSettings ss, BaseModel baseModel)
        {
            var mustVerUp = Versions.MustVerUp(context: context, baseModel: baseModel);
            return baseModel.VerType == Versions.VerTypes.Latest
                && baseModel.MethodType != BaseModel.MethodTypes.New
                && context.CanUpdate(ss: ss)
                    ? hb.FieldCheckBox(
                        controlId: "VerUp",
                        labelText: Displays.VerUp(context: context),
                        _checked: mustVerUp,
                        disabled: mustVerUp,
                        fieldCss: " w400 both",
                        controlCss: " always-send",
                        labelPositionIsRight: true)
                    : hb;
        }
    }
}