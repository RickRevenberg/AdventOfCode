namespace AdventOfCode.PuzzleSolvers._2022
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using NUnit.Framework;

	public class Day_04 : DayBase2022
	{
		public override int Day => 4;

		private List<List<(int start, int end)>> formattedInput;

		[SetUp]
		public async Task SetUp()
		{
			formattedInput = (await this.GetInput()).Split("\n").Select(x =>
					x.Split(',').Select(y => (Convert.ToInt32(y.Split('-')[0]), Convert.ToInt32(y.Split('-')[1])))
						.ToList())
				.ToList();
		}
		
		[Test]
		public override void PartOne()
		{
			var inclusivePairs = 0;

			foreach (var input in formattedInput)
			{
				inclusivePairs +=
					(input[0].start <= input[1].start && input[0].end >= input[1].end) ||
					(input[1].start <= input[0].start && input[1].end >= input[0].end)
						? 1 : 0;

			}

			Assert.Pass(inclusivePairs.ToString());
		}

		[Test]
		public override void PartTwo()
		{
			var overlappingPairs = 0;
			foreach (var input in formattedInput)
			{
				overlappingPairs += (input[0].end < input[1].start || input[0].start > input[1].end) ? 0 : 1;
			}

			Assert.Pass(overlappingPairs.ToString());
		}
	}
}
