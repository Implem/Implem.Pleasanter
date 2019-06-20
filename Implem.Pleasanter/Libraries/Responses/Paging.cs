namespace Implem.Pleasanter.Libraries.Responses
{
    public static class Paging
    {
        public static int NextOffset(int offset, int totalCount, int pageSize)
        {
            return offset + pageSize < totalCount
                ? offset + pageSize
                : -1;
        }
    }
}