using Implem.PleasanterTest.Models;
using System.Collections.Generic;

namespace Implem.PleasanterTest.Utilities
{
    public static class HtmlData
    {
        public static HtmlTest ExistsOne(string selector)
        {
            return new HtmlTest()
            {
                Type = HtmlTest.Types.ExistsOne,
                Selector = selector
            };
        }
    }
}
