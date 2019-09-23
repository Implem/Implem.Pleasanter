using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Implem.Pleasanter.Libraries.Server
{
    public class IpRange
    {
        public IpAddress Start { get; set; }
        public IpAddress End { get; set; }
        public override string ToString()
        {
            return Start.IpString + "-" + End.IpString;
        }

        public IpRange()
        {
            Start = new IpAddress(0);
            End = new IpAddress(0);
        }

        public IpRange(IpAddress start, IpAddress end)
        {
            Start = start;
            End = end;
        }
        public bool InRange(uint ipNum)
        {
            return Start.Value <= ipNum && ipNum <= End.Value;
        }

        public bool InRange(string ipString)
        {
            return InRange(new IpAddress(ipString).Value);
        }

        public static IpRange FromCidr(string cidrIpString)
        {
            var parts = cidrIpString.Split('/');
            if (parts.Length < 1 || parts.Length > 2)
            {
                return new IpRange();
            }
            var ip = new IpAddress(parts[0]);
            if (parts.Length == 1)
            {
                return new IpRange(ip, ip);
            }
            if (!int.TryParse(parts[1], out int maskbits))
            {
                return new IpRange();
            }
            if (maskbits < 1 || maskbits > 32)
            {
                maskbits = 32;
            }
            uint mask = 0xffffffff << (32 - maskbits);

            return new IpRange()
            {
                Start = new IpAddress(ip.Value & mask),
                End = new IpAddress(ip.Value | ~mask)
            };
        }

    }

}