namespace AdventOfCode.PuzzleSolvers._2021
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using NUnit.Framework;

	[TestFixture]
    public class Day_02 : DayBase2021
    {
	    public override int Day => 2;

        private List<(string command, int amount)> formattedInput = new ();

	    [SetUp]
	    public async Task SetUp()
	    {
		    this.formattedInput = (await this.GetInput())
			    .Split("\n")
			    .Select(command => (command.Split(' ')[0], int.Parse(command.Split(' ')[1]))).ToList();
	    }

		[Test]
	    public void PartOne()
	    {
		    var forwardTotal = formattedInput.Where(i => i.command.Equals("forward")).Sum(i => i.amount);
		    var upTotal = formattedInput.Where(i => i.command.Equals("up")).Sum(i => i.amount);
		    var downTotal = formattedInput.Where(i => i.command.Equals("down")).Sum(i => i.amount);

		    var depth = downTotal - upTotal;
		    var multiplied = forwardTotal * depth;

		    Assert.Pass(multiplied.ToString());
		}

		[Test]
	    public void PartTwo()
	    {
		    var aim = 0;
		    var horizontal = 0;
		    var depth = 0;

		    foreach (var point in formattedInput)
		    {
				var depthChange = point.command.Equals("forward") ? 0 : point.amount * (point.command.Equals("down") ? 1 : -1);
				aim += depthChange;

				if (point.command.Equals("forward"))
				{
					horizontal += point.amount;
					depth += point.amount * aim;
				}
		    }

		    var multiplied = horizontal * depth;

		    Assert.Pass(multiplied.ToString());
		}
    }
}