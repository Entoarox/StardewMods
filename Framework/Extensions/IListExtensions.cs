using System;
using System.Collections.Generic;

namespace Entoarox.Framework.Extensions
{
    public static class IListExtensions
    {
        /*********
        ** Public methods
        *********/
        public static int FindIndex<T>(this IList<T> items, Func<T, bool> predicate)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            int retVal = 0;
            foreach (T item in items)
            {
                if (predicate(item))
                    return retVal;
                retVal++;
            }

            return -1;
        }
    }
}
