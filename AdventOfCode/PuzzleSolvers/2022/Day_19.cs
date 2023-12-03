namespace AdventOfCode.PuzzleSolvers._2022
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using AdventOfCode.Logic.Extensions;
    using NUnit.Framework;

    public class Day_19 : DayBase2022
	{
		public override int Day => 19;

		private List<BluePrint> bluePrints;

		private bool testing = false;

		private const string TestInput =
			@"Blueprint 1:  Each ore robot costs 4 ore.  Each clay robot costs 2 ore.  Each obsidian robot costs 3 ore and 14 clay.  Each geode robot costs 2 ore and 7 obsidian.
Blueprint 2:  Each ore robot costs 2 ore.  Each clay robot costs 3 ore.  Each obsidian robot costs 3 ore and 8 clay.  Each geode robot costs 3 ore and 12 obsidian.";

		[SetUp]
		public async Task SetUp()
		{
			var input = testing
				? TestInput.Split("\r\n").ToList()
				: await this.SplitInput();

			var regex = new Regex("[0-9]+");
			this.bluePrints = input.Select(line =>
			{
				var numbers = regex.Matches(line).Select(x => x.Value.ToInt()).ToList();
				return new BluePrint
				{
					Id = numbers[0],
					OreBotCost = new BuildCost { Ore = numbers[1] },
					ClayBotCost = new BuildCost { Ore = numbers[2] },
					ObsidianBotCost = new BuildCost { Ore = numbers[3], Clay = numbers[4] },
					GeodeBotCost = new BuildCost { Ore = numbers[5], Obsidian = numbers[6] }
				};
			}).ToList();
		}

		[Test]
		public async Task PartOne()
		{
			var result = await Solve(this.bluePrints, 24);
			result.Keys.Select(key => key * result[key]).Sum().Pass();
		}

		[Test]
		public async Task PartTwo()
		{
			var result = await Solve(this.bluePrints.Take(3).ToList(), 32);
			result.Values.Select(x => (long)x).Product().Pass();
		}

		private static async Task<Dictionary<int, int>> Solve(List<BluePrint> availableBluePrints, int cycles)
		{
			const int ChunkSize = 100;

			var bestOutComes = new Dictionary<int, int>();

			foreach (var bluePrint in availableBluePrints)
			{
				var maxOreBotRequired = new List<int> { bluePrint.OreBotCost.Ore, bluePrint.ClayBotCost.Ore, bluePrint.ObsidianBotCost.Ore, bluePrint.GeodeBotCost.Ore }.Max();
				var maxClayBotRequired = new List<int> { bluePrint.OreBotCost.Clay, bluePrint.ClayBotCost.Clay, bluePrint.ObsidianBotCost.Clay, bluePrint.GeodeBotCost.Clay }.Max();
				var maxObsBotRequired = new List<int> { bluePrint.OreBotCost.Obsidian, bluePrint.ClayBotCost.Obsidian, bluePrint.ObsidianBotCost.Obsidian, bluePrint.GeodeBotCost.Obsidian }.Max();

				var possibleStates = new List<ResourceState> { new ResourceState { OreBotCount = 1 } };
				for (var minutesRemaining = cycles - 1; minutesRemaining >= 0; minutesRemaining--)
				{
					var newStates = new ConcurrentBag<List<ResourceState>>();
					foreach (var chunk in possibleStates.Chunk(ChunkSize))
					{
						var tasks = chunk.Select(state =>
							Task.Run(() =>
							{
								var states = DeterminePossibleNewStates(bluePrint, state);
								states = states.Where(state =>
									state.OreBotCount <= maxOreBotRequired &&
									state.ClayBotCount <= maxClayBotRequired &&
									state.ObsidianBotCount <= maxObsBotRequired).ToList();

								newStates.Add(states);
							}));

						await Task.WhenAll(tasks);
					}

					var resolvedStates = newStates.SelectMany(x => x).ToList();

					resolvedStates.ForEach(state =>
					{
						state.OreCount = Math.Min(state.OreCount, maxOreBotRequired * minutesRemaining);
						state.ClayCount = Math.Min(state.ClayCount, maxClayBotRequired * minutesRemaining);
						state.ObsidianCount = Math.Min(state.ObsidianCount, maxObsBotRequired * minutesRemaining);
					});

					var hashes = resolvedStates.Select(x => x.BotHash).Distinct().ToList();

					var filteredStates = new List<ResourceState>();
					foreach (var hash in hashes)
					{
						var matchingStates = resolvedStates.Where(x => x.BotHash == hash).ToList();
						for (var i = 0; i < matchingStates.Count; i++)
						{
							for (var j = i + 1; j < matchingStates.Count; j++)
							{
								var one = matchingStates[i];
								var two = matchingStates[j];

								if (one.OreCount <= two.OreCount && one.ClayCount <= two.ClayCount &&
									one.ObsidianCount <= two.ObsidianCount && one.GeodeCount <= two.GeodeCount)
								{
									matchingStates[i] = null;
									break;
								}
							}
						}

						filteredStates.AddRange(matchingStates.Where(x => x != null));
					}

					possibleStates = filteredStates;
				}

				var maxGeodes = possibleStates.MaxBy(x => x.GeodeCount).GeodeCount;
				bestOutComes.Add(bluePrint.Id, maxGeodes);
			}

			return bestOutComes;
		}

		private static List<ResourceState> DeterminePossibleNewStates(BluePrint bluePrint, ResourceState currentState)
		{
			var newStates = new List<ResourceState> { currentState };
			if (currentState.ValidCost(bluePrint.OreBotCost))
			{
				var newState = currentState - bluePrint.OreBotCost;

				newState.OreCount--;
				newState.OreBotCount += 1;
				newStates.Add(newState);
			}

			if (currentState.ValidCost(bluePrint.ClayBotCost))
			{
				var newState = currentState - bluePrint.ClayBotCost;

				newState.ClayCount--;
				newState.ClayBotCount += 1;
				newStates.Add(newState);
			}

			if (currentState.ValidCost(bluePrint.ObsidianBotCost))
			{
				var newState = currentState - bluePrint.ObsidianBotCost;

				newState.ObsidianCount--;
				newState.ObsidianBotCount += 1;
				newStates.Add(newState);
			}

			if (currentState.ValidCost(bluePrint.GeodeBotCost))
			{
				var newState = currentState - bluePrint.GeodeBotCost;

				newState.GeodeCount--;
				newState.GeodeBotCount += 1;
				newStates.Add(newState);
			}

			newStates.ForEach(state =>
			{
				state.OreCount += state.OreBotCount;
				state.ClayCount += state.ClayBotCount;
				state.ObsidianCount += state.ObsidianBotCount;
				state.GeodeCount += state.GeodeBotCount;
			});

			return newStates;
		}

		private class ResourceState
		{
			public int OreCount { get; set; }
			public int ClayCount { get; set; }
			public int ObsidianCount { get; set; }
			public int GeodeCount { get; set; }
			public int OreBotCount { get; set; }
			public int ClayBotCount { get; set; }
			public int ObsidianBotCount { get; set; }
			public int GeodeBotCount { get; set; }

			internal string BotHash => $"{OreBotCount}-{ClayBotCount}-{ObsidianBotCount}-{GeodeBotCount}";

			internal bool ValidCost(BuildCost cost)
			{
				return OreCount >= cost.Ore && ClayCount >= cost.Clay && ObsidianCount >= cost.Obsidian;
			}

			public static ResourceState operator -(ResourceState state, BuildCost cost)
			{
				var newState = state.Clone();
				
				newState.OreCount = state.OreCount - cost.Ore;
				newState.ClayCount = state.ClayCount - cost.Clay;
				newState.ObsidianCount = state.ObsidianCount - cost.Obsidian;

				return newState;
			}
		}

		private class BluePrint
		{
			internal int Id { get; init; }

			internal BuildCost OreBotCost { get; init; }
			internal BuildCost ClayBotCost { get; init; }
			internal BuildCost ObsidianBotCost { get; init; }
			internal BuildCost GeodeBotCost { get; init; }
		}

		private class BuildCost
		{
			internal int Ore { get; init; }
			internal int Clay { get; init; }
			internal int Obsidian { get; init; }
		}
	}
}
