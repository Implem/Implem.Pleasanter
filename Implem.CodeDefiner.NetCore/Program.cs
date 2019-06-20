using System.Text;

namespace Implem.CodeDefiner.NetCore
{
    class Program
    {
        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Starter.Main(args);
        }
    }
}
