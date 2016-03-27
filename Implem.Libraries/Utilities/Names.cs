namespace Implem.Libraries.Utilities
{
    public static class Names
    {
        public enum FirstAndLastNameOrders : int
        {
            None = 0,
            FirstNameIsFirst = 1,
            LastNameIsFirst = 2
        }

        public static string FullName(
            FirstAndLastNameOrders firstAndLastNameOrder, string fullName1, string fullName2)
        {
            switch (firstAndLastNameOrder)
            {
                case FirstAndLastNameOrders.LastNameIsFirst:
                    return fullName2;
                case FirstAndLastNameOrders.FirstNameIsFirst:
                default:
                    return fullName1;
            }
        }
    }
}
