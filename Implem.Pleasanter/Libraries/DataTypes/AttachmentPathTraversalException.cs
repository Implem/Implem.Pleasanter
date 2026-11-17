using System;
namespace Implem.Pleasanter.Libraries.DataTypes
{
    public class AttachmentPathTraversalException : Exception
    {
        public AttachmentPathTraversalException(string guid, string name)
            : base($"Invalid attachment temp path. Guid='{guid}' Name='{name}'")
        {
        }
    }
}
