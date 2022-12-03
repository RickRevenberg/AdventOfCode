namespace AdventOfCode.PuzzleSolvers._2021
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using NUnit.Framework;

	[TestFixture]
    public class Day_01 : DayBase2021
    {
	    private List<int> formattedInput;

	    public override int Day => 1;

        [SetUp]
	    public async Task SetUp()
	    {
		    this.formattedInput = (await this.GetInput()).Split("\n").Select(int.Parse).ToList();
	    }

	    [Test]
	    public override void PartOne()
	    {
		    var answer = formattedInput.Where((x, index) => index != 0 && x > formattedInput[index - 1]).Count();
		    Assert.Pass(answer.ToString());
        }

		[Test]
		public override void PartTwo()
        {
		    var answer = formattedInput.Where((x, index) => index > 2 && x > formattedInput[index - 3]).Count();
		    Assert.Pass(answer.ToString());
	    }
    }
}
