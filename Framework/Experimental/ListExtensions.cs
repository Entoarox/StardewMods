using System;
using System.Collections.Generic;

namespace Entoarox.Framework.Experimental
{
    /// <summary>Be warned that this is a experimental feature, thus it might cause crashes or other issues, and might potentially be removed from the framework. Should at any time this feature stop being experimental, it will be moved to a more correct namespace.</summary>
    [Obsolete("This feature is still experimental, it may not work properly and could be removed at any time!")]
    public static class ListExtensions
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Removes the last value from the list and returns it.</summary>
        /// <typeparam name="T">The type of the list.</typeparam>
        /// <param name="list">The list being manipulated.</param>
        /// <returns>The removed value.</returns>
        public static T Pop<T>(this IList<T> list)
        {
            T index = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            return index;
        }

        /// <summary>Adds the given value onto the end of the given list.</summary>
        /// <typeparam name="T">The type of the list.</typeparam>
        /// <param name="list">The list being manipulated.</param>
        /// <param name="value">The value that is being added.</param>
        public static void Push<T>(this IList<T> list, T value)
        {
            list.Insert(list.Count, value);
        }

        /// <summary>Removes the first value from the list and returns it.</summary>
        /// <typeparam name="T">The type of the list.</typeparam>
        /// <param name="list">The list being manipulated.</param>
        /// <returns>The removed value.</returns>
        public static T Shift<T>(this IList<T> list)
        {
            T index = list[0];
            list.RemoveAt(0);
            return index;
        }

        /// <summary>Adds the given value onto the start of the given list.</summary>
        /// <typeparam name="T">The type of the list.</typeparam>
        /// <param name="list">The list being manipulated.</param>
        /// <param name="value">The value that is being added.</param>
        public static void Unshift<T>(this IList<T> list, T value)
        {
            list.Insert(0, value);
        }
    }
}
