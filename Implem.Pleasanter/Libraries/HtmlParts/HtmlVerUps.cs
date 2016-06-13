using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Models;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlVerUps
    {
        public static HtmlBuilder VerUpCheckBox(this HtmlBuilder hb, BaseModel baseModel)
        {
            var mustVerUp = MustVerUp(baseModel);
            return 
                baseModel.VerType == Versions.VerTypes.Latest &&
                baseModel.MethodType != BaseModel.MethodTypes.New
                    ? hb.FieldCheckBox(
                        controlId: "VerUp",
                        labelText: Displays.VerUp(),
                        _checked: mustVerUp,
                        disabled: mustVerUp,
                        fieldCss: " w400 both",
                        controlCss: " must-transport",
                        labelPositionIsRight: true)
                    : hb;
        }

        private static bool MustVerUp(BaseModel baseModel)
        {
            return
                baseModel.Updator.Id != Sessions.UserId() ||
                baseModel.UpdatedTime.DifferentDate();
        }
    }
}