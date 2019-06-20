namespace Implem.Libraries.Utilities
{
    public static class Databases
    {
        public enum AccessStatuses
        {
            Initialized,
            Selected,
            Deleted,
            Failed,
            NotFound,
            Overlap
        }
    }
}
