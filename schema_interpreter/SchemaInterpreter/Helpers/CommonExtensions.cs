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

        /// <summary>
        /// Checks if a enumerable of numbers is consecutive.
        /// </summary>
        public static bool IsConsecutive(this IEnumerable<int> enumerable)
            => !enumerable.Select((i, j) => i - j).Distinct().Skip(1).Any();

        public static bool IsNullOrWhitespace(this string input)
            => string.IsNullOrWhiteSpace(input);

        /// <summary>
        /// Returns the index of the first <paramref name="characters"/> found in the string.
        /// Returns -1 if none of the <paramref name="characters"/> is found.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="characters">The array of characters to search.</param>
        public static int IndexOf(this string input, char[] characters)
        {
            for(int i = 0; i < input.Length; i++)
            {
                char currentChar = input[i];
                if (characters.Contains(currentChar))
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Tries to find a list of <paramref name="characters"/> in a string and returns
        /// the first to find. Returns null if no character is present in the string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="characters">The array of characters to search.</param>
        public static char? FindFirst(this string input, char[] characters)
        {
            for (int i = 0; i < input.Length; i++)
            {
                char currentChar = input[i];
                if (characters.Contains(currentChar))
                    return currentChar;
            }

            return null;
        }
    }
}