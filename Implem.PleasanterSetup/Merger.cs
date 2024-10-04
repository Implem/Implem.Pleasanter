using Newtonsoft.Json.Linq;

namespace Implem.PleasanterSetup
{
    internal static class Merger
    {
        internal static void MergeParametersJson(string installDir, string prevPath, string currentVersion, string newVersion)
        {
            try
            {
                PatchJson.ApplyToPatch(installDir,prevPath, newVersion, currentVersion);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
