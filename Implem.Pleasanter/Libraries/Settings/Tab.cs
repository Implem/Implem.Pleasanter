using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Requests;
using System.Runtime.Serialization;
namespace Implem.Pleasanter.Libraries.Settings
{
    public class Tab : ISettingListItem
    {
        public int Id { get; set; }
        public string LabelText;

        public Tab()
        {
        }

        public Tab(Context context, SiteSettings ss)
        {
            SetByForm(context: context, ss: ss);
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext streamingContext)
        {
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext streamingContext)
        {
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

        public Tab GetRecordingData(SiteSettings ss)
        {
            return new Tab
            {
                Id = Id,
                LabelText = LabelText
            };
        }
    }
}