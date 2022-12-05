namespace AdventOfCode.Logic
{
	using System;
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
    }
}
