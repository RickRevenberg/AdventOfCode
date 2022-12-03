namespace AdventOfCode.PuzzleSolvers._2021
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using NUnit.Framework;

	[TestFixture]
    public class Day_03 : DayBase2021
    {
	    private List<string> formattedData = new List<string>();

	    public override int Day => 3;

	    [SetUp]
	    public async Task SetUp()
	    {
		    formattedData = (await this.GetInput()).Replace("\n", ",").Split(',').ToList();
	    }

		[Test]
	    public override void PartOne()
	    {
		    var deltaString = "";

		    for (var i = 0; i < 12; i++)
		    {
			    deltaString += formattedData.Select(x => x[i].ToString()).Count(x => x.Equals("1")) > (formattedData.Count / 2) ? 1 : 0;
		    }

		    var deltaValue = Convert.ToInt32(deltaString, 2);
		    var epsilonValue = 4095 - deltaValue;

		    var multiplied = deltaValue * epsilonValue;

			Assert.Pass(multiplied.ToString());
	    }

		[Test]
	    public override void PartTwo()
	    {
		    var remainingData = formattedData.Select(x => x).ToList();
		    var tracker = 0;
		    while (remainingData.Count > 1)
		    {
				var oneCount = remainingData.Count(x => x[tracker] == '1');
				var zeroCount = remainingData.Count - oneCount;

				var remainingdigit = oneCount >= zeroCount ? "1" : "0";

				remainingData = remainingData.Where(x => x[tracker].ToString().Equals(remainingdigit)).ToList();

				tracker++;
			}

		    var oxygen = Convert.ToInt32(remainingData.Single(), 2);

		    remainingData = formattedData.Select(x => x).ToList();
		    tracker = 0;

			while (remainingData.Count > 1)
			{
				var oneCount = remainingData.Count(x => x[tracker] == '1');
				var zeroCount = remainingData.Count - oneCount;

				var remainingdigit = zeroCount <= oneCount ? "0" : "1";

				remainingData = remainingData.Where(x => x[tracker].ToString().Equals(remainingdigit)).ToList();

				tracker++;
			}

			var scrubber = Convert.ToInt32(remainingData.Single(), 2);

		    var multiplied = oxygen * scrubber;

		    Assert.Pass(multiplied.ToString());
		}
    }
}
