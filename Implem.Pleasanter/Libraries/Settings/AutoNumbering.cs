using Implem.Libraries.Utilities;
using System;
using System.Runtime.Serialization;
using static Implem.Pleasanter.Libraries.Settings.Column;

namespace Implem.Pleasanter.Libraries.Settings
{
    [Serializable]
    public class AutoNumbering
    {
        public string ColumnName { get; set; }
        public string Format { get; set; }
        public AutoNumberingResetTypes? ResetType { get; set; }
        public int? Default { get; set; }
        public int? Step { get; set; }

        public AutoNumbering()
        {
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext streamingContext)
        {
            Default = Default ?? 1;
            Step = Step ?? 1;
            ResetType = ResetType ?? AutoNumberingResetTypes.None;
        }

        public AutoNumbering GetRecordingData()
        {
            if (!ColumnName.IsNullOrEmpty())
            {
                var autoNumbering = new AutoNumbering();
                autoNumbering.ColumnName = ColumnName;
                autoNumbering.Format = Format;
                if (ResetType != AutoNumberingResetTypes.None)
                {
                    autoNumbering.ResetType = ResetType;
                }
                if (Default != 1)
                {
                    autoNumbering.Default = Default;
                }
                if (Step != 1)
                {
                    autoNumbering.Step = Step;
                }
                return autoNumbering;
            }
            return null;
        }
    }
}
