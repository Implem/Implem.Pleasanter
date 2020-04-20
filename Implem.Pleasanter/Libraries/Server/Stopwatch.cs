using System.Collections.Generic;
using System.Diagnostics;
namespace Implem.Pleasanter.Libraries.Server
{
    public class Stopwatch
    {
        public int Index = 1;
        public System.Diagnostics.Stopwatch Sw = new System.Diagnostics.Stopwatch();
        public Dictionary<string, double> Results = new Dictionary<string, double>();

        public Stopwatch()
        {
            Start();
        }

        [Conditional("DEBUG")]
        public void Start()
        {
            Sw.Start();
        }

        [Conditional("DEBUG")]
        public void Record(string name)
        {
            Sw.Stop();
            Results.Add($"{Index}_{name}", Sw.Elapsed.TotalMilliseconds);
            Index++;
            Sw.Restart();
        }
    }
}