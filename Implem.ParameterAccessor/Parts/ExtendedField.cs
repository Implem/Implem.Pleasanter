using System;
using System.Runtime.Serialization;

namespace Implem.ParameterAccessor.Parts
{
    [Serializable]
    public class ExtendedField : ExtendedBase
    {
        public string FieldType;
        public string TypeName;
        public string LabelText;
        public string ChoicesText;
        public string DefaultInput;
        public string EditorFormat;
        public string ControlType;
        public bool? ValidateRequired;
        public bool? ValidateNumber;
        public bool? ValidateDate;
        public bool? ValidateEmail;
        public decimal? MaxLength;
        public string ValidateEqualTo;
        public int? ValidateMaxLength;
        public int? DecimalPlaces;
        public bool? Nullable;
        public string Unit;
        public decimal? Min;
        public decimal? Max;
        public decimal? Step;
        public bool? AutoPostBack;
        public string FieldCss;
        public int CheckFilterControlType;
        public int? DateTimeStep;
        public string ControlCss;
        public string After;
        public bool SqlParam;
    
        [OnDeserialized]
        private void OnDeserialized(StreamingContext streamingContext)
        {
            TypeName = string.IsNullOrWhiteSpace(TypeName)
                ? "nvarchar"
                : TypeName;
        }
    }
}