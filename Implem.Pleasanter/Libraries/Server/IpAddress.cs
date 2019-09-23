using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Implem.Pleasanter.Libraries.Server
{

    public class IpAddress
    {
        public string IpString { get; }
        public uint Value { get; }

        public IpAddress(string ipString)
        {
            IpString = ipString;
            Value = IpStringToInteger(ipString);
        }

        public IpAddress(uint value)
        {
            Value = value;
            IpString = $"{(Value & 0xff000000) >> 24}.{(Value & 0x00ff0000) >> 16}.{(Value & 0x0000ff00) >> 8}.{Value & 0x000000ff}";
        }

        public uint IpStringToInteger(string ipString)
        {
            var octet = ipString.Split('.');
            if (octet.Length != 4)
            {
                return 0;
            }
            if (!uint.TryParse(octet[0], out uint octet_0)
                || !uint.TryParse(octet[1], out uint octet_1)
                || !uint.TryParse(octet[2], out uint octet_2)
                || !uint.TryParse(octet[3], out uint octet_3))
            {
                return 0;
            }
            return (octet_0 << 24) | (octet_1 << 16) | (octet_2 << 8) | octet_3;
        }
    }
}