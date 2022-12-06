namespace AdventOfCode.PuzzleSolvers._2022
{
	using System.Linq;
	using System.Threading.Tasks;
	using Logic;
	using NUnit.Framework;

	public class Day_06 : DayBase2022
    {
	    public override int Day => 6;

	    private string inputStream;

		[SetUp]
	    public async Task SetUp()
	    {
		    this.inputStream = await this.GetInput();
	    }

		[Test]
	    public override void PartOne()
	    {
		    this.Solve(4);
        }

		[Test]
	    public override void PartTwo()
	    {
		   this.Solve(14);
        }

	    private void Solve(int uniqueCharacters)
	    {
		    var data = inputStream.ToCharArray();
		    var tracker = 0;
		    while (true)
		    {
			    var packet = data.Skip(tracker).Take(uniqueCharacters).ToList();
			    if (packet.Distinct().Count() == uniqueCharacters)
			    {
				    (tracker + uniqueCharacters).Pass();
			    }

			    tracker++;
		    }
        }
    }
}
