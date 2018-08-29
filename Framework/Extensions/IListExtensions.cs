using System;
using System.Collections.Generic;
using System.Text;

namespace Entoarox.Framework.Extensions
{
    public static class IListExtensions
    {
        public static int FindIndex<T>(this IList<T> items, Func<T, bool> predicate)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            int retVal = 0;
            foreach (var item in items)
            {
                if (predicate(item)) return retVal;
                retVal++;
            }
            return -1;
        }
    }
}
