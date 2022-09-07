using Implem.PleasanterTest.Models;
using System.Collections.Generic;
using System.Linq;

namespace Implem.PleasanterTest.Utilities
{
    internal class BaseData
    {
        public static List<BaseTest> Tests(params BaseTest[] tests)
        {
            return tests.ToList();
        }
    }
}
