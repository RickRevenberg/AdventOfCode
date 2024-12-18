﻿namespace AdventOfCode.Logic.Extensions
{
    using Newtonsoft.Json;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;

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

        internal static bool ContainsDuplicate<T>(this List<T> input)
        {
            return input.Count != input.Distinct().Count();
        }

        internal static long LCM(this List<long> input)
        {
            var lcm = input.First();
            for (var i = 1; i < input.Count; i++)
            {
                var (gcfA, gcfB) = (lcm, input[i]);

                while (gcfB != 0)
                {
                    var temp = gcfB;
                    gcfB = gcfA % gcfB;
                    gcfA = temp;
                }

                lcm = (lcm / gcfA) * input[i];
            }

            return lcm;
        }

        internal static bool In<T>(this T value, params T[] options)
        {
            return options.Contains(value);
        }

        internal static void Pass(this object input)
        {
            Assert.Pass(input.ToString());
        }

        internal static string Join<T>(this IEnumerable<T> input, string seperator = null)
        {
            return string.Join(seperator ?? "", input);
        }

        internal static long Product(this IEnumerable<int> input) => input.Select(x => (long)x).Product();
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
