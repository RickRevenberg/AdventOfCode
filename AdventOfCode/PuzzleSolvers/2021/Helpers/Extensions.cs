namespace AdventOfCode.PuzzleSolvers._2021.Helpers
{
	using System.Collections.Generic;
	using Newtonsoft.Json;

	internal static class Extensions
    {
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
    }
}
