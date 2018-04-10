using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlVerUps
    {
        public static HtmlBuilder VerUpCheckBox(this HtmlBuilder hb, BaseModel baseModel)
        {
            var mustVerUp = Versions.MustVerUp(baseModel);
            return
                baseModel.VerType == Versions.VerTypes.Latest &&
                baseModel.MethodType != BaseModel.MethodTypes.New
                    ? hb.FieldCheckBox(
                        controlId: "VerUp",
                        labelText: Displays.VerUp(),
                        _checked: mustVerUp,
                        disabled: mustVerUp,
                        fieldCss: " w400 both",
                        controlCss: " always-send",
                        labelPositionIsRight: true)
                    : hb;
        }
    }
}