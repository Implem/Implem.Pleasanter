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
            bool isNewMethod = baseModel.MethodType == BaseModel.MethodTypes.New;
            bool cannotUpdate = !context.CanUpdate(ss: ss);
            bool isOldVersion = baseModel.VerType != Versions.VerTypes.Latest;
            if (isNewMethod || cannotUpdate || isOldVersion) return hb;

            var mustVerUp = Versions.MustVerUp(
                context: context,
                ss: ss,
                baseModel: baseModel,
                isSite: ss.SiteId == 0 || ss.IsSite(context: context));
            return hb.FieldCheckBox(
                        controlId: "VerUp",
                        labelText: Displays.VerUp(context: context),
                        _checked: mustVerUp,
                        disabled: mustVerUp,
                        fieldCss: " w400 both",
                        controlCss: " always-send",
                        labelPositionIsRight: true);
        }
    }
}