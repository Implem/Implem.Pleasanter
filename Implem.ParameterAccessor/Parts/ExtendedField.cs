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
        public string ControlType;
        public int CheckFilterControlType;
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