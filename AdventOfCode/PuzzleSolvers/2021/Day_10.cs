namespace AdventOfCode.PuzzleSolvers._2021
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using NUnit.Framework;

	[TestFixture]
    public class Day_10 : DayBase2021
    {
	    public override int Day => 10;

        private readonly List<int> stack = new List<int>();

	    private readonly Dictionary<string, int> checksumChars = new Dictionary<string, int>
	    {
		    {"(", -3},
		    {"[", -57},
		    {"{", -1197},
		    {"<", -25137},
		    {")", 3},
		    {"]", 57},
		    {"}", 1197},
		    {">", 25137}
	    };

		private List<string> Lines;

		[SetUp]
	    public async Task SetUp()
	    {
		    Lines = (await this.GetInput()).Split("\n").ToList();
	    }

	    [Test]
	    public void PartOne()
	    {
		    var totalSyntaxError = 0;

		    foreach (var line in Lines)
		    {
				stack.Clear();

			    var characters = line.ToCharArray().Select(c => c.ToString()).ToList();
			    foreach (var character in characters)
			    {
				    var opening = checksumChars[character] < 0;
				    if (opening)
				    {
						stack.Add(checksumChars[character]);
				    }
				    else
				    {
					    if (checksumChars[character] != -stack.Last())
					    {
						    totalSyntaxError += checksumChars[character];
						    break;
					    }

					    stack.RemoveAt(stack.Count - 1);
				    }
			    }
		    }

		    Assert.Pass(totalSyntaxError.ToString());
	    }

		[Test]
	    public void PartTwo()
	    {
		    var lineScores = new List<long>();

			foreach (var line in Lines)
			{
				stack.Clear();

				var lineCorrupted = false;
				var characters = line.ToCharArray().Select(c => c.ToString());

				foreach (var character in characters)
				{
					if (checksumChars[character] < 0)
					{
						stack.Add(checksumChars[character]);
					}
					else if (checksumChars[character] != -stack.Last())
					{
						lineCorrupted = true;
						break;
					}
					else
					{
						stack.RemoveAt(stack.Count - 1);
					}
				}

				static long ClosingScore(int input)
				{
					return input switch
					{
						-3 => 1,
						-57 => 2,
						-1197 => 3,
						-25137 => 4,
						_ => 0
					};
				}

				if (!lineCorrupted && stack.Any())
				{
					var lineScore = 0L;
					stack.Reverse();
					stack.ForEach(x =>
					{
						lineScore *= 5;
						lineScore += ClosingScore(x);
					});

					lineScores.Add(lineScore);
				}
			}

			var finalScore = lineScores.OrderBy(x => x).Skip(lineScores.Count / 2).First();

			Assert.Pass(finalScore.ToString());
	    }
    }
}
