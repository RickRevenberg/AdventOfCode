namespace AdventOfCode.PuzzleSolvers._2022
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text.RegularExpressions;
	using System.Threading.Tasks;
	using Logic;
	using NUnit.Framework;

	public class Day_21 : DayBase2022
	{
		public override int Day => 21;

		private static Dictionary<string, Monkey> monkeyDict;

		[SetUp]
		public async Task SetUp()
		{
			var input = await this.GetInput();
			monkeyDict = new Dictionary<string, Monkey>();

			var numericMonkeyRegex = new Regex("[a-z]+: [0-9]+");
			var operationMonkeyRegex = new Regex("[a-z]+: [a-z]+ (\\+|-|\\*|\\/) [a-z]+");

			foreach (var monkey in numericMonkeyRegex.Matches(input).Select(x => x.Value))
			{
				monkeyDict.Add(monkey.Split(": ")[0], new Monkey
				{
					Number = monkey.Split(": ")[1].ToInt()
				});
			}

			var monkeyIdRegex = new Regex("[a-z]+");
			foreach (var monkey in operationMonkeyRegex.Matches(input).Select(x => x.Value))
			{
				var data = monkey.Split(": ");

				var operationType = data[1].Contains("+")
					? OperationType.Add
					: data[1].Contains("-")
						? OperationType.Subtract
						: data[1].Contains("*")
							? OperationType.Multiply
							: OperationType.Divide;

				Func<decimal, decimal, decimal> operation = operationType switch
				{
					OperationType.Add => (one, two) => one + two,
					OperationType.Subtract => (one, two) => one - two,
					OperationType.Multiply => (one, two) => one * two,
					OperationType.Divide => (one, two) => one / two,
					_ => throw new ArgumentOutOfRangeException()
				};

				var operationIds = monkeyIdRegex.Matches(data[1]).Select(x => x.Value).ToList();
				monkeyDict.Add(data[0], new Monkey
				{
					Left = operationIds[0],
					Right = operationIds[1],
					OperationType = operationType,
					Operation = operation
				});
			}
		}

		[Test]
		public void PartOne()
		{
			monkeyDict["root"].GetNumber().Pass();
		}

		[Test]
		public void PartTwo()
		{
			var currMonkey = monkeyDict["root"];

			var (leftValue, rightValue, rightChild) = DetermineTurn(currMonkey);
			var targetValue = rightChild ? leftValue : rightValue;

			while (true)
			{
				currMonkey = rightChild ? monkeyDict[currMonkey.Right] : monkeyDict[currMonkey.Left];

				if (currMonkey == monkeyDict["humn"])
				{
					break;
				}

				(leftValue, rightValue, rightChild) = DetermineTurn(currMonkey);

				targetValue = DeterminePartValue(
					targetValue, currMonkey.OperationType,
					rightChild ? leftValue : null,
					rightChild ? null : rightValue);
			}

			targetValue.Pass();
		}
		private static (decimal left, decimal right, bool turnRight) DetermineTurn(Monkey currMonkey)
		{
			monkeyDict["humn"].Number = 1;

			var leftValue = monkeyDict[currMonkey.Left].GetNumber();
			var rightValue = monkeyDict[currMonkey.Right].GetNumber();

			monkeyDict["humn"].Number = 100;

			var newLeftValue = monkeyDict[currMonkey.Left].GetNumber();

			return (leftValue, rightValue, leftValue == newLeftValue);
		}


		private static decimal DeterminePartValue(decimal target, OperationType type, decimal? one, decimal? two)
		{
			return type switch
			{
				OperationType.Add => target - one.GetValueOrDefault() - two.GetValueOrDefault(),
				OperationType.Multiply => target / (one ?? two!.Value),
				OperationType.Subtract => one.HasValue ? one.Value - target : target + two!.Value,
				OperationType.Divide => one.HasValue ? one.Value / target : two!.Value * target,
				_ => throw new Exception()
			};
		}

		private class Monkey
		{
			internal int? Number { get; set; }
			internal Func<decimal, decimal, decimal> Operation { get; init; }

			internal string Left { get; init; }
			internal string Right { get; init; }

			internal OperationType OperationType { get; init; }

			internal decimal GetNumber()
			{
				return Number ?? Operation(monkeyDict[this.Left].GetNumber(), monkeyDict[this.Right].GetNumber());
			}
		}

		private enum OperationType
		{
			Add = 0,
			Subtract = 1,
			Multiply = 2,
			Divide = 3
		}
	}
}
