using Implem.Libraries.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Implem.Pleasanter.Libraries.Utilities
{
    public static class Versions
    {
        public enum VerTypes
        {
            Latest,
            Previous,
            First,
            Unknown
        }

        public enum DirectioTypes
        {
            Latest,
            None,
            Next,
            Previous
        }

        public static VerTypes VerType(long id, DirectioTypes directionType = DirectioTypes.None)
        {
            switch (directionType)
            {
                case DirectioTypes.Latest:
                    return VerTypes.Latest;
                case DirectioTypes.None:
                    return id == 1 ? VerTypes.First : VerTypes.Previous;
                case DirectioTypes.Next:
                    return VerTypes.Previous;
                case DirectioTypes.Previous:
                    return id == 1 ? VerTypes.First : VerTypes.Previous;
                default:
                    return VerTypes.Unknown;
            }
        }
    }
}