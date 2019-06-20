using Implem.CodeDefiner.Utilities;
using Implem.Libraries.Classes;
using System.Collections.Generic;
namespace Implem.CodeDefiner.Functions.AspNetMvc.CSharp
{
    internal class DataContainer
    {
        internal Dictionary<string, XlsIo> XlsIoCollection;
        internal string Type = string.Empty;
        internal string DefinitionName = string.Empty;
        internal string ModelName = string.Empty;
        internal string TableName = string.Empty;
        internal string FormName = string.Empty;
        internal string ColumnName = string.Empty;

        internal DataContainer(string type)
        {
            XlsIoCollection = DefinitionFiles.Collection();
            Type = type;
        }
    }
}
