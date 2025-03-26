using System;

namespace Implem.ParameterAccessor.Parts
{
    [Serializable()]
    public class TrialLicense
    {
        public bool Check() => Deadline >= DateTime.Now.Date;
        public int? Users { get; set; }
        public DateTime Deadline { get; set; }
        public string Licensee { get; set; }
        public DateTime CreatedTime { get; set; }
        public string CheckSum { get; set; }
    }
}
