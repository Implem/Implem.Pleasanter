using NLog;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Implem.Pleasanter.Libraries.Server
{
    public class IpRange
    {
        private static readonly Logger Logger = LogManager.GetLogger("syslogs");

        private static readonly ConcurrentDictionary<string, byte> WarnedValues = new();

        private readonly IPNetwork? network;

        private IpRange(IPNetwork? network)
        {
            this.network = network;
        }

        public override string ToString()
        {
            return network?.ToString() ?? string.Empty;
        }

        public bool InRange(string ipString)
        {
            if (network == null
                || !IPAddress.TryParse(ipString, out var ip))
            {
                return false;
            }
            if (ip.IsIPv4MappedToIPv6)
            {
                return network.Value.BaseAddress.AddressFamily == AddressFamily.InterNetwork
                    ? network.Value.Contains(ip.MapToIPv4())
                    : ContainsAllBits(
                        network: network.Value,
                        ip: ip);
            }
            return network.Value.Contains(ip);
        }

        private static bool ContainsAllBits(IPNetwork network, IPAddress ip)
        {
            var networkBytes = network.BaseAddress.GetAddressBytes();
            var ipBytes = ip.GetAddressBytes();
            if (networkBytes.Length != ipBytes.Length)
            {
                return false;
            }
            var prefixLength = network.PrefixLength;
            for (var i = 0; i < networkBytes.Length && prefixLength > 0; i++, prefixLength -= 8)
            {
                var mask = prefixLength >= 8
                    ? (byte)0xFF
                    : (byte)(0xFF << (8 - prefixLength));
                if ((networkBytes[i] & mask) != (ipBytes[i] & mask))
                {
                    return false;
                }
            }
            return true;
        }

        public static IpRange FromCidr(string cidrIpString)
        {
            if (TryParse(
                cidrIpString: cidrIpString,
                network: out var network))
            {
                return new IpRange(network);
            }
            if (!string.IsNullOrWhiteSpace(cidrIpString)
                && WarnedValues.TryAdd(cidrIpString, 0))
            {
                Logger.Warn($"IpRange.FromCidr: 不正なIPアドレス/CIDR指定のため無視します。value=\"{cidrIpString}\"");
            }
            return new IpRange(null);
        }

        public static bool TryParse(
            string cidrIpString,
            out IPNetwork network,
            bool strictPrefixLength = false)
        {
            network = default;
            cidrIpString = cidrIpString?.Trim();
            if (string.IsNullOrEmpty(cidrIpString)
                || cidrIpString.Contains('%'))
            {
                return false;
            }
            var parts = cidrIpString.Split('/');
            if (parts.Length > 2)
            {
                return false;
            }
            var address = parts[0];
            var decimalIpv4 = NormalizeDecimalIpv4(address);
            if (decimalIpv4 != null)
            {
                cidrIpString = decimalIpv4 + cidrIpString.Substring(address.Length);
                address = decimalIpv4;
            }
            if (!IPAddress.TryParse(address, out var ip))
            {
                return false;
            }
            if (ip.AddressFamily == AddressFamily.InterNetwork
                && decimalIpv4 == null)
            {
                return false;
            }
            var maxPrefixLength = ip.AddressFamily == AddressFamily.InterNetwork
                ? 32
                : 128;
            var prefixLength = maxPrefixLength;
            if (parts.Length == 2)
            {
                if (!int.TryParse(parts[1], out prefixLength))
                {
                    return false;
                }
                if (prefixLength < 1 || prefixLength > maxPrefixLength)
                {
                    if (strictPrefixLength)
                    {
                        return false;
                    }
                    prefixLength = maxPrefixLength;
                }
            }
            return IPNetwork.TryParse($"{address}/{prefixLength}", out network);
        }

        private static string NormalizeDecimalIpv4(string address)
        {
            var octets = address.Split('.');
            return octets.Length == 4
                && octets.All(o => o.Length > 0
                    && o.Length <= 3
                    && o.All(char.IsAsciiDigit)
                    && int.Parse(o) <= 255)
                        ? string.Join(".", octets.Select(o => int.Parse(o)))
                        : null;
        }
    }
}
