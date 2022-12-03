namespace AdventOfCode._2021
{
	using System;
	using System.Drawing;
	using NUnit.Framework;

	[TestFixture]
    public class Day_17
    {
	    private const int ScanArea = 1000;
	    private int minX, maxX, minY, maxY;

		[SetUp]
	    public void SetUp()
	    {
		    var split = Input.Replace("target area: x=", string.Empty).Replace(" y=", string.Empty).Split(",");

		    minX = int.Parse(split[0].Split("..")[0]);
		    maxX = int.Parse(split[0].Split("..")[1]);
		    minY = int.Parse(split[1].Split("..")[0]);
		    maxY = int.Parse(split[1].Split("..")[1]);
	    }

		[Test]
	    public void PartOne()
	    {
		    var maxY = int.MinValue;
		    var foundRoutes = 0;

		    for (var x = 0; x < ScanArea; x++)
		    {
			    for (var y = -ScanArea; y < ScanArea; y++)
			    {
				    if (CheckDirectory(x, y, out var routeMax))
				    {
					    maxY = Math.Max(maxY, routeMax);
					    foundRoutes++;
				    }
			    }
		    }

			Assert.Pass($"Routes: {foundRoutes} - highest: {maxY}");
	    }

	    private bool CheckDirectory(int x, int y, out int routeMax)
	    {
		    routeMax = int.MinValue;
		    var currLoc = new Point(0, 0);

		    while (true)
		    {
			    currLoc.X += x;
			    currLoc.Y += y;

			    routeMax = Math.Max(routeMax, currLoc.Y);

			    if (currLoc.X >= minX && currLoc.X <= maxX && currLoc.Y >= minY && currLoc.Y <= maxY)
			    {
				    return true;
			    }
				else if (currLoc.X > maxX || currLoc.Y < minY)
			    {
				    return false;
			    }

			    x = Math.Max(0, x - 1);
			    y--;
		    }
	    }

		private const string TestInput = @"target area: x=20..30, y=-10..-5";
		private const string Input = @"target area: x=269..292, y=-68..-44";
    }
}
