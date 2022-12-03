namespace AdventOfCode.PuzzleSolvers._2021
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using NUnit.Framework;

	[TestFixture]
    public class Day_07 : DayBase2021
    {
	    public override int Day => 7;

        private List<int> crabs = new ();

		[SetUp]
	    public async Task SetUp()
	    {
		    this.crabs = (await this.GetInput()).Split(',').Select(x => Convert.ToInt32(x)).OrderBy(x => x).ToList();
	    }

	    [Test]
	    public override void PartOne()
	    {
		    var mean = -1;
		    var difference = int.MaxValue;

		    for (var i = 0; i < crabs.Count; i++)
		    {
			    var lower = crabs.Count(x => x <= i);
			    var higher = crabs.Count(x => x >= i);

			    var currDiff = Math.Abs(higher - lower);
			    if (currDiff <= difference)
			    {
				    difference = currDiff;
				    mean = i;
			    }
			    else
			    {
				    break;
			    }
		    }

			var fuelSpent = crabs.Select(x => Math.Abs(x - mean)).Sum();

			Assert.Pass(fuelSpent.ToString());
	    }

		[Test]
	    public override void PartTwo()
	    {
		    var lastBest = int.MaxValue;

		    for (var i = crabs[0]; i <= crabs.Last(); i++)
		    {
			    var fuelSpent = crabs.Select(x => FactorialIsh(Math.Abs(x - i))).Sum();
			    if (fuelSpent <= lastBest)
			    {
				    lastBest = fuelSpent;
			    }
			    else
			    {
				    break;
			    }
			}

			Assert.Pass(lastBest.ToString());
	    }

	    private static int FactorialIsh(int input)
	    {
		    var total = 0;

		    for (var i = 1; i <= input; i++)
		    {
			    total += i;
		    }

		    return total;
	    }
    }
}
