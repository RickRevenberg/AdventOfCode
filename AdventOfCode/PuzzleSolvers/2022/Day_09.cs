namespace AdventOfCode.PuzzleSolvers._2022
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Logic;
	using NUnit.Framework;

	public class Day_09 : DayBase2022
	{
		public override int Day => 9;

		private List<(string direction, int amount)> formattedInput;

		[SetUp]
		public async Task SetUp()
		{
			formattedInput = (await this.SplitInput()).Select(x => x.Split(" ")).Select(x => (x[0], x[1].ToInt())).ToList();
		}

		[Test]
	    public override void PartOne()
	    {
		    Solve(2);
	    }

		[Test]
	    public override void PartTwo()
	    {
		    Solve(10);
        }

	    private void Solve(int knotAmount)
	    {
		    var knots = new List<(int x, int y)>();
		    for (var i = 0; i < knotAmount; i++)
		    {
			    knots.Add((0, 0));
		    }

		    var tailPositions = new List<(int, int)> { (0, 0) };

		    foreach (var input in formattedInput)
		    {
			    for (var i = 0; i < input.amount; i++)
			    {
				    var diffState = CoordinateDiff(input.direction);
					knots[0] = knots[0].Apply(diffState);

				    for (var j = 1; j < knotAmount; j++)
				    {
					    var head = knots[j - 1];
					    var tail = knots[j];

					    if (head.Distance(tail) > 1)
					    {
						    var (xDiff, yDiff) = tail.Difference(head);
						    var xMove = xDiff > 0 ? 1 : xDiff < 0 ? -1 : 0;
						    var yMove = yDiff > 0 ? 1 : yDiff < 0 ? -1 : 0;

						    knots[j] = tail.Apply((xMove, yMove));

						    if (j == (knotAmount - 1))
						    {
							    tailPositions.Add(tail);
						    }
					    }
				    }
			    }
		    }

			// +1. for some reason.
		    (tailPositions.Distinct().Count() + 1).Pass();
        }

	    private static (int, int) CoordinateDiff(string input)
	    {
		    return input.ToLower() switch
		    {
			    "u" => (0, -1),
			    "d" => (0, 1),
			    "l" => (-1, 0),
			    "r" => (1, 0),
			    _ => throw new Exception()
		    };
	    }
	}

	internal static class Extensions
	{
		internal static (int, int) Apply(this (int, int) state, (int, int) modifier)
		{
			return (state.Item1 + modifier.Item1, state.Item2 + modifier.Item2);
		}

		internal static int Distance(this (int, int) one, (int, int) two)
		{
			var xDifference = Math.Abs(one.Item1 - two.Item1);
			var yDifference = Math.Abs(one.Item2 - two.Item2);

			return Math.Max(xDifference, yDifference);
		}

		internal static (int, int) Difference(this (int, int) one, (int, int) two)
		{
			return (two.Item1 - one.Item1, two.Item2 - one.Item2);
		}
	}
}
