using System;
using System.Collections.Generic;

namespace Implem.PleasanterTest.Utilities
{
    public static class ItemData
    {
        public enum ReferenceTypes
        {
            Sites,
            Issues,
            Results,
            Wikis
        };

        public static IEnumerable<ReferenceTypes> GetReferenceTypes()
        {
            foreach (ReferenceTypes referenceType in Enum.GetValues(typeof(ReferenceTypes)))
            {
                yield return referenceType;
            }
        }
    }
}
