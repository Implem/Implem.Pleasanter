using Implem.Libraries.Classes;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.Security
{
    public static class Defenses
    {
        public static Dictionary<string, TwoData<string, int>> RequestVolume =
            new Dictionary<string, TwoData<string, int>>();
    }
}