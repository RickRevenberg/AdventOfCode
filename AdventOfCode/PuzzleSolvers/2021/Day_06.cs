namespace AdventOfCode.PuzzleSolvers._2021
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using NUnit.Framework;

	[TestFixture]
    public class Day_06 : DayBase2021
    {
	    public override int Day => 6;

        private Dictionary<int, double> fishTwo = new Dictionary<int, double>();

		[SetUp]
	    public async Task SetUp()
	    {
		    var fish = (await this.GetInput()).Split(',').Select(x => Convert.ToInt32(x)).ToList();
		    fishTwo = new Dictionary<int, double>();
		    for (var i = 0; i <= 8; i++)
		    {
				fishTwo.Add(i, fish.Count(x => x == i));
		    }
	    }

	    [Test]
        public void PartOne()
	    {
		    Solution(80);
	    }

		[Test]
	    public void PartTwo()
	    {
		    Solution(256);
	    }

	    private void Solution(int amountOfDays)
	    {
		    for (var i = 0; i < amountOfDays; i++)
		    {
			    var newFish = fishTwo[0];

			    for (var j = 0; j < 8; j++)
			    {
				    fishTwo[j] = fishTwo[j + 1];
			    }

			    fishTwo[6] += newFish;
			    fishTwo[8] = newFish;
		    }

		    var total = fishTwo.Keys.Sum(key => fishTwo[key]);

			Assert.Pass(total.ToString());
	    }
    }
}
