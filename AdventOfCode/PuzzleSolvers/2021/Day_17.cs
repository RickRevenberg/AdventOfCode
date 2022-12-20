namespace AdventOfCode.PuzzleSolvers._2021
{
	using System;
	using System.Drawing;
	using System.Threading.Tasks;
	using NUnit.Framework;

	[TestFixture]
    public class Day_17 : DayBase2021
    {
	    public override int Day => 17;

        private const int ScanArea = 1000;
	    private int minX, maxX, minY, maxY;

		[SetUp]
	    public async Task SetUp()
	    {
		    var split = (await this.GetInput()).Replace("target area: x=", string.Empty).Replace(" y=", string.Empty).Split(",");

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

	    public void PartTwo()
	    {
		    throw new NotImplementedException();
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
    }
}
