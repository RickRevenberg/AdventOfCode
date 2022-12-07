namespace AdventOfCode.Logic
{
	using System;
	using System.Collections.Generic;
	using NUnit.Framework;

	internal static class Extensions
    {
	    internal static int ToInt(this string input)
	    {
		    return Convert.ToInt32(input);
	    }

	    internal static void Pass(this object input)
	    {
			Assert.Pass(input.ToString());
	    }

	    internal static string Join(this IEnumerable<object> input, string seperator = null)
	    {
		    return string.Join(seperator ?? "", input);
	    }
    }
}
