using System;
namespace Implem.Pleasanter.Libraries.Requests
{
    [Serializable]
    public class ImportApi : Api
    {
        public bool ReplaceAllGroupMembers { get; set; }
        public bool UpdatableImport { get; set; }
        public bool RejectNullImport { get; set; }
        public bool MigrationMode { get; set; }
        public string Encoding { get; set; }
        public string Key {get; set;}
    }
}