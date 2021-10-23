using System;
namespace Implem.Pleasanter.Libraries.Settings
{
    [Serializable]
    public class BulkUpdateColumnDetail
    {
        public bool? ValidateRequired;
        public bool? EditorReadOnly;
        public string DefaultInput;

        public BulkUpdateColumnDetail()
        {
        }
    }
}