namespace AdventOfCode.PuzzleSolvers._2022
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text.RegularExpressions;
	using System.Threading.Tasks;
	using Logic;
	using NUnit.Framework;

	public class Day_11 : DayBase2022
	{
		public override int Day => 11;

		private Dictionary<int, Monkey> Monkeys;

		private int WorryReductionIndex = 1;

		[SetUp]
		public async Task SetUp()
		{
			var input = await this.GetInput();
			Monkeys = new Dictionary<int, Monkey>();

			var divisionIndex = 1;
			foreach (var monkeyData in input.Split("\n\n"))
			{
				var itemRegex = new Regex("(items: )([0-9]+, )*([0-9]+)");
				var items = itemRegex.Match(monkeyData).Value.Split("items: ")[1].Split(", ")
					.Select(x => new Item(x.ToInt())).ToList();

				var operationRegex = new Regex("(\\*|\\+) ([0-9]+|(old))");
				var operation = operationRegex.Match(monkeyData).Value;

				var operationValue = operation.Split(" ")[1] == "old" ? (int?)null : operation.Split(" ")[1].ToInt();
				Func<Item, Item> function = operation.Split(" ")[0] switch
				{
					"*" => x => (x * (operationValue ?? x.WorryLevel)) / WorryReductionIndex,
					"+" => x => (x + (operationValue ?? x.WorryLevel)) / WorryReductionIndex,
					_ => throw new Exception()
				};

				var testRegex = new Regex("(by )[0-9]+");
				var testValue = testRegex.Match(monkeyData).Value.Split(" ")[1].ToInt();

				divisionIndex *= testValue;

				var monkeyTargetRegex = new Regex("(monkey )[0-9]+");
				var targets = monkeyTargetRegex.Matches(monkeyData).Select(x => x.Value.Split(" ")[1].ToInt()).ToList();

				Monkeys.Add(Monkeys.Count, new Monkey
				{
					Items = items,
					Operation = function,
					Test = (x) => x.WorryLevel % testValue == 0,
					ThrowTarget = (x) => x ? targets[0] : targets[1]
				});
			}

			foreach (var monkey in Monkeys)
			{
				foreach (var item in monkey.Value.Items)
				{
					item.DivisionCutOff = divisionIndex;
				}
			}
		}

		[Test]
		public override void PartOne()
		{
			this.WorryReductionIndex = 3;
			this.Solve(20).Pass();
		}

		[Test]
		public override void PartTwo()
		{
			this.WorryReductionIndex = 1;
			this.Solve(10000).Pass();
		}

		private long Solve(int turnCount)
		{
			for (var i = 0; i < turnCount; i++)
			{
				foreach (var key in Monkeys.Keys)
				{
					var monkey = Monkeys[key];

					monkey.InspectionCount += monkey.Items.Count;
					monkey.Items = monkey.Items.Select(monkey.Operation).ToList();

					for (var j = 0; j < monkey.Items.Count; j++)
					{
						var target = monkey.ThrowTarget(monkey.Test(monkey.Items[j]));
						Monkeys[target].Items.Add(monkey.Items[j]);

						monkey.Items[j] = null;
					}

					monkey.Items = monkey.Items.Where(x => x != null).ToList();
				}
			}

			var mostActive = this.Monkeys.Keys.Select(key => this.Monkeys[key])
				.OrderByDescending(x => x.InspectionCount).Take(2).ToList();

			return ((long)mostActive[0].InspectionCount * mostActive[1].InspectionCount);
		}

		private class Monkey
		{
			internal List<Item> Items { get; set; }

			internal Func<Item, Item> Operation { get; init; }
			internal Func<Item, bool> Test { get; init; }
			internal Func<bool, int> ThrowTarget { get; init; }

			internal int InspectionCount { get; set; } = 0;
		}

		private class Item
		{
			internal int DivisionCutOff { get; set; }

			internal int WorryLevel { get; private set; }

			public Item(int worryLevel)
			{
				this.WorryLevel = worryLevel;
			}

			public static Item operator +(Item one, int value)
			{
				var temp = (long)one.WorryLevel + value;
				one.WorryLevel = (int)(temp % one.DivisionCutOff);

				return one;
			}

			public static Item operator *(Item one, int value)
			{
				var temp = (long)one.WorryLevel * value;
				one.WorryLevel = (int)(temp % one.DivisionCutOff);

				return one;
			}

			public static Item operator /(Item one, int value)
			{
				one.WorryLevel /= value;
				return one;
			}
		}
	}
}
