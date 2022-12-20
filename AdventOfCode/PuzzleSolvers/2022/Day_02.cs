namespace AdventOfCode.PuzzleSolvers._2022
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using NUnit.Framework;

	[TestFixture]
	public class Day_02 : DayBase2022
	{
		private List<(string opponent, string me)> formattedInput;

		public override int Day => 2;

		[SetUp]
		public async Task SetUp()
		{
			formattedInput = (await this.GetInput()).Split("\n").Select(x => (x.Split(' ')[0], x.Split(' ')[1])).ToList();
		}

		[Test]
		public void PartOne()
		{
			var score = 0;

			foreach (var input in formattedInput)
			{
				if (Draw(input))
				{
					score += (MoveScore(input.me) + 3);
				}
				else if (Won(input))
				{
					score += (MoveScore(input.me) + 6);
				}
				else
				{
					score += MoveScore(input.me);
				}
			}

			Assert.Pass(score.ToString());
		}

		[Test]
		public void PartTwo()
		{
			formattedInput = formattedInput.Select(mapCondition).ToList();
			PartOne();
		}

		private static int MoveScore(string input)
		{
			return input switch
			{
				"X" => 1,
				"Y" => 2,
				"Z" => 3,
				_ => 0
			};
		}

		private static (string, string) mapCondition((string opponent, string me) input)
		{
			var combined = $"{input.opponent}{input.me}";

			return combined switch
			{
				"AX" => ("A", "Z"),
				"AY" => ("A", "X"),
				"AZ" => ("A", "Y"),
				"BX" => ("B", "X"),
				"BY" => ("B", "Y"),
				"BZ" => ("B", "Z"),
				"CX" => ("C", "Y"),
				"CY" => ("C", "Z"),
				"CZ" => ("C", "X"),
				_ => (null, null)
			};
		}

		private static bool Draw((string opponent, string me) input)
		{
			var combined = $"{input.opponent}{input.me}";

			return combined switch
			{
				"AX" => true,
				"BY" => true,
				"CZ" => true,
				_ => false
			};
		}

		private static bool Won((string opponent, string me) input)
		{
			var combined = $"{input.opponent}{input.me}";

			return combined switch
			{
				"AY" => true,
				"AZ" => false,
				"BX" => false,
				"BZ" => true,
				"CX" => true,
				"CY" => false,
				_ => false
			};
		}
	}
}
