namespace AdventOfCode.PuzzleSolvers._2022
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using NUnit.Framework;

	[TestFixture]
	public class Day_01 : DayBase2022
	{
		private List<int> formattedInput;

		public override int Day => 1;

		[SetUp]
		public async Task SetUp()
		{
			formattedInput = (await this.GetInput()).Split("\n\n")
				.Select(x => x.Split("\n").Select(y => Convert.ToInt32(y)).Sum())
				.ToList();
		}

		[Test]
		public void PartOne()
		{
			var answer = formattedInput.Max();
			Assert.Pass(answer.ToString());
		}

		[Test]
		public void PartTwo()
		{
			var answer = formattedInput.OrderByDescending(x => x).Take(3).Sum();
			Assert.Pass(answer.ToString());
		}
	}
}
