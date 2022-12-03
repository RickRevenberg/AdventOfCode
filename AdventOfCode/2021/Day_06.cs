namespace AdventOfCode._2021
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using NUnit.Framework;

	[TestFixture]
    public class Day_06
    {
	    private Dictionary<int, double> fishTwo = new Dictionary<int, double>();

		[SetUp]
	    public void SetUp()
	    {
		    var fish = input.Split(',').Select(x => Convert.ToInt32(x)).ToList();
		    fishTwo = new Dictionary<int, double>();
		    for (var i = 0; i <= 8; i++)
		    {
				fishTwo.Add(i, fish.Count(x => x == i));
		    }
	    }

	    [TestCase(80, TestName = "PartOne")]
	    [TestCase(256, TestName = "PartTwo")]
	    public void Solution(int amountOfDays)
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

	    private string input =
		    @"1,1,3,1,3,2,1,3,1,1,3,1,1,2,1,3,1,1,3,5,1,1,1,3,1,2,1,1,1,1,4,4,1,2,1,2,1,1,1,5,3,2,1,5,2,5,3,3,2,2,5,4,1,1,4,4,1,1,1,1,1,1,5,1,2,4,3,2,2,2,2,1,4,1,1,5,1,3,4,4,1,1,3,3,5,5,3,1,3,3,3,1,4,2,2,1,3,4,1,4,3,3,2,3,1,1,1,5,3,1,4,2,2,3,1,3,1,2,3,3,1,4,2,2,4,1,3,1,1,1,1,1,2,1,3,3,1,2,1,1,3,4,1,1,1,1,5,1,1,5,1,1,1,4,1,5,3,1,1,3,2,1,1,3,1,1,1,5,4,3,3,5,1,3,4,3,3,1,4,4,1,2,1,1,2,1,1,1,2,1,1,1,1,1,5,1,1,2,1,5,2,1,1,2,3,2,3,1,3,1,1,1,5,1,1,2,1,1,1,1,3,4,5,3,1,4,1,1,4,1,4,1,1,1,4,5,1,1,1,4,1,3,2,2,1,1,2,3,1,4,3,5,1,5,1,1,4,5,5,1,1,3,3,1,1,1,1,5,5,3,3,2,4,1,1,1,1,1,5,1,1,2,5,5,4,2,4,4,1,1,3,3,1,5,1,1,1,1,1,1";
    }
}
