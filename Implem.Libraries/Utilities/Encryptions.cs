using System.Linq;
using System.Security.Cryptography;
using System.Text;
namespace Implem.Libraries.Utilities
{
    public static class Encryptions
    {
        public static string Sha512Cng(this string self)
        {
            using (var sha512cng = new SHA512Cng())
            {
                return sha512cng.ComputeHash(Encoding.UTF8.GetBytes(self))
                    .Select(hash => hash.ToString("x2"))
                    .Join(string.Empty);
            }
        }
    }
}
