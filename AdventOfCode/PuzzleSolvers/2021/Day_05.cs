namespace AdventOfCode.PuzzleSolvers._2021
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Linq;
	using System.Threading.Tasks;
	using NUnit.Framework;

	[TestFixture]
	public class Day_05 : DayBase2021
	{
		public override int Day => 5;

        private List<Line> lines = new ();

		private readonly int[][] grid = new int[1000][];

		[SetUp]
	    public async Task SetUp()
	    {
		    var pairs = (await this.GetInput()).Split("\n").ToList();
		    lines = pairs.Select(pair => new Line
		    {
				StartPoint = new Point(Convert.ToInt32(pair.Split(" -> ")[0].Split(',')[0]), Convert.ToInt32(pair.Split(" -> ")[0].Split(',')[1])),
				EndPoint = new Point(Convert.ToInt32(pair.Split(" -> ")[1].Split(',')[0]), Convert.ToInt32(pair.Split(" -> ")[1].Split(',')[1]))
			}).ToList();

		    for (var i = 0; i < 1000; i++)
		    {
			    grid[i] = new int[1000];
		    }
		}

	    [Test]
	    public override void PartOne()
	    {
			lines = lines.Where(line => line.StartPoint.X == line.EndPoint.X || line.StartPoint.Y == line.EndPoint.Y).ToList();
			lines.ForEach(AddStraightLineToGrid);

			var moreThanOne = grid.ToList().SelectMany(row => row.Where(x => x > 1)).Count();

			Assert.Pass(moreThanOne.ToString());
	    }

	    [Test]
	    public override void PartTwo()
	    {
		    var straightLines = lines.Where(line => line.StartPoint.X == line.EndPoint.X || line.StartPoint.Y == line.EndPoint.Y).ToList();
			var diagonalLines = lines.Where(line => line.StartPoint.X != line.EndPoint.X && line.StartPoint.Y != line.EndPoint.Y).ToList();

			straightLines.ForEach(AddStraightLineToGrid);
			diagonalLines.ForEach(AddDiagonalLineToGrid);

		    var moreThanOne = grid.ToList().SelectMany(row => row.Where(x => x > 1)).Count();

		    Assert.Pass(moreThanOne.ToString());
		}

		private void AddStraightLineToGrid(Line line)
	    {
		    var horizontal = line.StartPoint.X != line.EndPoint.X;

		    var smallest = horizontal
			    ? Math.Min(line.StartPoint.X, line.EndPoint.X)
			    : Math.Min(line.StartPoint.Y, line.EndPoint.Y);

			var biggest = horizontal
				? Math.Max(line.StartPoint.X, line.EndPoint.X)
				: Math.Max(line.StartPoint.Y, line.EndPoint.Y);

			var constant = horizontal ? line.StartPoint.Y : line.StartPoint.X;

			for (var i = smallest; i <= biggest; i++)
			{
				var x = horizontal ? i : constant;
				var y = horizontal ? constant : i;

				grid[y][x]++;
			}
	    }

		private void AddDiagonalLineToGrid(Line line)
		{
			var horMod = line.EndPoint.X > line.StartPoint.X ? 1 : -1;
			var verMod = line.EndPoint.Y > line.StartPoint.Y ? 1 : -1;

			var length = Math.Abs(line.EndPoint.X - line.StartPoint.X);

			for (var i = 0; i <= length; i++)
			{
				grid[line.StartPoint.Y + (i * verMod)][line.StartPoint.X + (i * horMod)]++;
			}
		}

	    private class Line
	    {
			internal Point StartPoint { get; init; }
			internal Point EndPoint { get; init; }
	    }
	}
}
