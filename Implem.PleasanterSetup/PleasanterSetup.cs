using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implem.PleasanterSetup
{
    public class PleasanterSetup : ConsoleAppBase
    {
        [RootCommand]
        public void Setup()
        {
            Console.WriteLine("This is Setup Command.");
        }

        [Command("merge")]
        public void Merge()
        {
            Console.WriteLine("This is Merge Command.");
        }

        [Command("rds")]
        public void Rds()
        {
            Console.WriteLine("This is Rds Command.");
        }
    }
}
