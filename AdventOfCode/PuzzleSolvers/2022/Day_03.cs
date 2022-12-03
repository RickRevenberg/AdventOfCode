namespace AdventOfCode.PuzzleSolvers._2022
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using NUnit.Framework;

	[TestFixture]
	public class Day_03 : DayBase2022
	{
		private List<(List<string> compartmentOne, List<string> compartmentTwo, List<string> combined)> formattedInput;

		public override int Day => 3;

		[SetUp]
		public async Task SetUp()
		{
			this.formattedInput = (await this.GetInput())
				.Split("\n")
				.Select(x => x.ToCharArray().Select(x => x.ToString()).ToList())
				.Select(x => (
					x.Take(x.Count / 2).Distinct().ToList(),
					x.Skip(x.Count / 2).Take(x.Count).Distinct().ToList(),
					x.Distinct().ToList()))
				.ToList();
		}

		[Test]
		public override void PartOne()
		{
			var priorityScore = formattedInput.Sum(input => GetPriority(input.compartmentOne.Single(input.compartmentTwo.Contains)));
			Assert.Pass(priorityScore.ToString());
		}

		[Test]
		public override void PartTwo()
		{
			var priorityScore = formattedInput.Chunk(3).Sum(group =>
				GetPriority(group[0].combined
					.Single(x => group[1].combined.Contains(x) && group[2].combined.Contains(x))));
			
			Assert.Pass(priorityScore.ToString());
		}

		private static int GetPriority(string input)
		{
			var score = 0;

			if (input.ToUpper() == input)
			{
				input = input.ToLower();
				score += 26;
			}

			score += ((int)Encoding.ASCII.GetBytes(input)[0]) - 96;

			return score;
		}
	}
}
