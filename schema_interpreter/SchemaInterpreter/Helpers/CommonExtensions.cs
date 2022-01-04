using System.Collections.Generic;
using System.Linq;

namespace SchemaInterpreter.Helpers
{
    public static class CommonExtensions
    {
        /// <summary>
        /// Returns a single object as an enumerable.
        /// </summary>
        /// <typeparam name="T">The type of object to create the enumerable from.</typeparam>
        /// <param name="obj">The object to return as enumerable.</param>
        /// <returns></returns>
        public static IEnumerable<T> AsEnumerable<T>(this T obj)
            => Enumerable.Repeat(obj, 1);
    }
}