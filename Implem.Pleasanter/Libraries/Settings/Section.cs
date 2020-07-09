using Implem.Pleasanter.Libraries.Requests;

namespace Implem.Pleasanter.Libraries.Settings
{
    public class Section
    {
        public int Id;
        public string LabelText;
        public bool? AllowExpand;
        public bool? Expand;

        public Section GetRecordingData(SiteSettings ss)
        {
            var section = new Section();
            section.Id = Id;
            section.LabelText = LabelText;
            section.AllowExpand = AllowExpand;
            section.Expand = Expand ?? true;
            return section;
        }

        public void SetByForm(Context context, SiteSettings ss)
        {
            foreach (string controlId in context.Forms.Keys)
            {
                switch (controlId)
                {
                    case "LabelText":
                        LabelText = String(
                            context: context,
                            controlId: controlId);
                        break;
                    case "AllowExpand":
                        AllowExpand = Bool(
                            context: context,
                            controlId: controlId);
                        break;
                    case "Expand":
                        Expand = Bool(
                            context: context,
                            controlId: controlId);
                        break;
                    default:
                        break;
                }
            }
        }

        private string String(Context context, string controlId)
        {
            var data = context.Forms.Data(controlId);
            return data != string.Empty ? data : null;
        }

        private bool Bool(Context context, string controlId)
        {
            var data = context.Forms.Bool(controlId);
            return data;
        }
    }
}