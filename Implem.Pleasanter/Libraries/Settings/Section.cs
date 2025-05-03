using Implem.Pleasanter.Libraries.Requests;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.Settings
{
    public class Section
    {
        public int Id;
        public string LabelText;
        public bool? AllowExpand;
        public bool? Expand;
        public bool? Hide;

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
                        LabelText = context.Forms.Data(controlId);
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

        private bool Bool(Context context, string controlId)
        {
            var data = context.Forms.Bool(controlId);
            return data;
        }

        public void Update(
            int id,
            string labelText,
            bool? allowExpand,
            bool? expand,
            bool? hide)
        {
            Id = id;
            if (labelText != null) LabelText = labelText;
            if (allowExpand != null) AllowExpand = allowExpand;
            if (expand != null) Expand = expand;
            if (hide != null) Hide = hide;
        }
    }
}