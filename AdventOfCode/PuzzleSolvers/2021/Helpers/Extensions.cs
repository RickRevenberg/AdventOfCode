namespace AdventOfCode.PuzzleSolvers._2021.Helpers
{
	using System.Collections.Generic;

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
    }
}
