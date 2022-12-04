namespace AdventOfCode.PuzzleSolvers._2022
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Logic;
	using NUnit.Framework;

	public class Day_04 : DayBase2022
	{
		public override int Day => 4;

		private List<List<(int start, int end)>> formattedInput;

		[SetUp]
		public async Task SetUp()
		{
			formattedInput = 
				(await this.GetInput()).Split("\n")
				.Select(x => x.Split(',')
					.Select(y => (y.Split('-')[0].ToInt(), y.Split('-')[1].ToInt())).ToList())
				.ToList();
		}
		
		[Test]
		public override void PartOne()
		{
			var inclusivePairs = formattedInput.Count(input =>
				(input[0].start <= input[1].start && input[0].end >= input[1].end) ||
				(input[1].start <= input[0].start && input[1].end >= input[0].end));

			Assert.Pass(inclusivePairs.ToString());
		}

		[Test]
		public override void PartTwo()
		{
			var overlappingPairs = formattedInput.Count(input => !(input[0].end < input[1].start || input[0].start > input[1].end));

			Assert.Pass(overlappingPairs.ToString());
		}
	}
}
