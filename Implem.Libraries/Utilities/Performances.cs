using Implem.Libraries.Classes;
namespace Implem.Libraries.Utilities
{
    public static class Performances
    {
        public static PerformanceCollection PerformanceCollection = new PerformanceCollection();

        public static void Record(string name)
        {
            PerformanceCollection.Record(name);
        }
    }
}
