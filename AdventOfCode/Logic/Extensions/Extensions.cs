namespace AdventOfCode.Logic.Extensions
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using NUnit.Framework;

    internal static class Extensions
    {
        internal static int ToInt(this string input)
        {
            return Convert.ToInt32(input);
        }

        internal static long ToLong(this string input)
        {
            return Convert.ToInt64(input);
        }

        internal static List<List<T>> Split<T>(this List<T> input, Func<T, bool> predicate)
        {
            var totalCount = 0;
            var collections = new List<List<T>>();

            while (totalCount < input.Count)
            {
                var subCollection = input.Skip(totalCount).TakeWhile((x, i) => i == 0 || !predicate(x)).ToList();

                collections.Add(subCollection);
                totalCount += subCollection.Count;
            }

            return collections;
        }

        internal static void Pass(this object input)
        {
            Assert.Pass(input.ToString());
        }

        internal static string Join<T>(this IEnumerable<T> input, string seperator = null)
        {
            return string.Join(seperator ?? "", input);
        }

        internal static long Product(this IEnumerable<long> input)
        {
	        long? total = null;
	        foreach (var number in input)
	        {
		        total = total == null ? number : total!.Value * number;
	        }

	        return total.GetValueOrDefault();
        }

        internal static T Clone<T>(this T input) where T : class
        {
	        return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(input));
        }

        internal static bool HasIndex<T>(this IList<T> data, int index)
        {
	        return index >= 0 && data.Count > index;
        }
    }
}
