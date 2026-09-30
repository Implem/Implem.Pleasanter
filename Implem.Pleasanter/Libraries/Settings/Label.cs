using Implem.Pleasanter.Libraries.Requests;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Settings
{
    public class Label
    {
        public int Id;
        public string Body;
        public string LabelType;
        public bool? Hide;

        public Label GetRecordingData(SiteSettings ss)
        {
            var label = new Label();
            label.Id = Id;
            label.Body = Body;
            label.LabelType = LabelTypes.Normalize(LabelType);
            return label;
        }

        public void SetByForm(Context context, SiteSettings ss)
        {
            foreach (string controlId in context.Forms.Keys)
            {
                switch (controlId)
                {
                    case "LabelBody":
                        Body = context.Forms.Data(controlId);
                        break;
                    case "LabelType":
                        LabelType = LabelTypes.Normalize(context.Forms.Data(controlId));
                        break;
                    default:
                        break;
                }
            }
        }

        public void Update(
            int id,
            string body,
            string labelType,
            bool? hide)
        {
            Id = id;
            if (body != null) Body = body;
            if (labelType != null) LabelType = LabelTypes.Normalize(labelType);
            if (hide != null) Hide = hide;
        }
    }

    public static class LabelTypes
    {
        public const string Plain = "Plain";
        public const string Info = "Info";
        public const string Warning = "Warning";
        public const string Alert = "Alert";

        public static List<string> All = new List<string>
        {
            Plain,
            Info,
            Warning,
            Alert
        };

        public static string Normalize(string labelType)
        {
            return All.Any(o => o == labelType)
                ? labelType
                : Plain;
        }
    }
}
