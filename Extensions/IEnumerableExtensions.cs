using System;
using System.Collections.Generic;

namespace Visual_SICXE.Extensions
{
    internal static class IEnumerableExtensions
    {
        /// <summary>
        /// Determines whether any item in the collection lies on the specified closed interval when mapped to an int.
        /// </summary>
        /// <typeparam name="T">The type of the item.</typeparam>
        /// <param name="o">The collection of items that can be mapped to an int.</param>
        /// <param name="transform">A functionn that maps an item in the collection to an int.</param>
        /// <param name="intervalStart">The beginning of the interval.</param>
        /// <param name="intervalSize">The end of the interval.</param>
        /// <returns>A Boolean value indicating whether any item belonged to the specified interval under transformation.</returns>
        public static bool NearlyContains<T>(this IEnumerable<T> o, Func<T, int> transform, int intervalStart, int intervalSize)
        {
            foreach (var item in o)
            {
                int diff = transform(item) - intervalStart;
                if (diff >= 0 && diff <= intervalSize)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the first item the collection that lies on the specified interval when mapped to an int.
        /// </summary>
        /// <typeparam name="T">The type of the item.</typeparam>
        /// <param name="o">The collection of items that can be mapped to an int.</param>
        /// <param name="transform">A function that maps an item in the collection to an int.</param>
        /// <param name="intervalStart">The inclusive beginning of the interval.</param>
        /// <param name="intervalSize">The distance from the interval's lower bound to the exclusive upper bound.</param>
        /// <returns>The first item found that belongs to the specified interval under transformation, if any, or else null.</returns>
        public static T FirstInRange<T>(this IEnumerable<T> o, Func<T, int> transform, int intervalStart, int intervalSize)
        {
            foreach (var item in o)
            {
                int diff = transform(item) - intervalStart;
                if (diff >= 0 && diff < intervalSize)
                {
                    //Debug.WriteLine($"{transform(item).ToString("X")} is inside [{intervalStart.ToString("X")}, {(intervalStart + intervalSize).ToString("X")}).");
                    return item;
                }
                    
            }
            return default(T);
        }

    }
}
