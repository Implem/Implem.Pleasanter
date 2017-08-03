using System;
namespace Implem.Pleasanter.Libraries.DataTypes
{
    public class Attachment
    {
        public long Id;
        public string Name;
        public long Size;
        public string Extention;
        [NonSerialized]
        public string Guid;
    }
}