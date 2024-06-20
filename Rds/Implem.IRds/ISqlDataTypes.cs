﻿using System.Data;

namespace Implem.IRds
{
    public interface ISqlDataType
    {
        string Convert(string name);
        string ConvertBack(string name);
        string DefaultDefinition(DataRow dataRow);
    }
}
