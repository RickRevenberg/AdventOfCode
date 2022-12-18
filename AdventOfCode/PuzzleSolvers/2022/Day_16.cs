
namespace AdventOfCode.PuzzleSolvers._2022
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Runtime.CompilerServices;
	using System.Text.RegularExpressions;
	using System.Threading.Tasks;
	using Logic;
	using NUnit.Framework;

	public class Day_16 : DayBase2022
	{
		public override int Day => 16;

		private bool testing = false;

		private Dictionary<int, Valve> valves;

		private const string TestInput = @"Valve AA has flow rate=0; tunnels lead to valves DD, II, BB
Valve BB has flow rate=13; tunnels lead to valves CC, AA
Valve CC has flow rate=2; tunnels lead to valves DD, BB
Valve DD has flow rate=20; tunnels lead to valves CC, AA, EE
Valve EE has flow rate=3; tunnels lead to valves FF, DD
Valve FF has flow rate=0; tunnels lead to valves EE, GG
Valve GG has flow rate=0; tunnels lead to valves FF, HH
Valve HH has flow rate=22; tunnel leads to valve GG
Valve II has flow rate=0; tunnels lead to valves AA, JJ
Valve JJ has flow rate=21; tunnel leads to valve II";

		[SetUp]
		public async Task SetUp()
		{
			var input = testing
				? TestInput.Split("\r\n").ToList()
				: await this.SplitInput();

			valves = new Dictionary<int, Valve>();

			var nameRegex = new Regex("[A-Z][A-Z]");
			var flowRegex = new Regex("[0-9]+");

			foreach (var data in input)
			{
				var valve = new Valve
				{
					Id = valves.Keys.Count,
					Name = nameRegex.Matches(data)[0].Value,
					FlowRate = flowRegex.Match(data).Value.ToInt()
				};

				valves.Add(valves.Keys.Count, valve);
			}

			for (var i = 0; i < input.Count; i++)
			{
				var connections = nameRegex.Matches(input[i]).Skip(1).Select(x => x.Value).ToList();
				valves[i].Connections = valves.Values.Where(x => connections.Contains(x.Name)).Select(x => x.Id).ToList();
			}
		}

		public override void PartOne(){}

		[TestCase(5, 100)]
		public void PartOne(int chunkSize, int cacheSize)
		{
			var startingPoint = this.valves.Values.Single(x => x.Name == "AA").Id;

			var routeWeight = new Dictionary<int, Dictionary<int, int>>();
			var usefulNodes = this.valves.Values.Where(x => x.Id == startingPoint || x.FlowRate > 0).ToList();

			foreach (var startId in usefulNodes.Select(x => x.Id))
			{
				routeWeight.Add(startId, new Dictionary<int, int>());

				foreach (var endId in usefulNodes.Select(x => x.Id))
				{
					if (startId == endId) { continue; }
					routeWeight[startId].Add(endId, CalculatePath(startId, endId).Count - 1);
				}
			}

			usefulNodes = usefulNodes.Where(x => x.Id != startingPoint).ToList();

			var bestSegments = new List<(int score, int timeConsumed, List<int> route)>
			{
				(0, 0, new List<int> { startingPoint })
			};

			while (true)
			{
				var changed = false;

				var worstOption = 0;
				var newData = new List<(int score, int timeConsumed, List<int> route)>();

				foreach (var segment in bestSegments)
				{
					var options = DetermineOrders(usefulNodes.Select(x => x.Id).Except(segment.route).ToList(), chunkSize);

					foreach (var option in options)
					{
						var routeScore = 0;
						var timeRemaining = 30 - segment.timeConsumed;
						var currentPoint = segment.route.Last();

						var route = option;

						foreach (var node in option)
						{
							timeRemaining -= routeWeight[currentPoint][node] + 1;

							if (timeRemaining <= 0)
							{
								route = option.Take(option.FindIndex(x => x == node)).ToList();
								break;
							}

							routeScore += this.valves[node].FlowRate * timeRemaining;
							currentPoint = node;
						}

						if (segment.score + routeScore == 2694)
						{
							var what = 0;
						}

						if (routeScore > 0 && (routeScore > worstOption || newData.Count < cacheSize))
						{
							changed = true;

							newData.Add((segment.score + routeScore, 30 - timeRemaining, new List<List<int>> { segment.route, route }.SelectMany(x => x).ToList()));
							
							newData = newData.OrderByDescending(x => x.score).Take(cacheSize).ToList();
							worstOption = newData.MinBy(x => x.score).score - segment.score;
						}
					}
				}

				bestSegments.AddRange(newData);
				bestSegments = bestSegments.OrderByDescending(x => x.score).Take(cacheSize).ToList();

				if (!changed)
				{
					break;
				}
			}

			$"{bestSegments[0].score}: {(bestSegments[0].route.Select(x => this.valves[x].Name).Join(" - "))}".Pass();;
		}

		public override void PartTwo()
		{
			throw new System.NotImplementedException();
		}

		private List<int> CalculatePath(int start, int end, List<List<int>> currentPaths = null)
		{
			currentPaths ??= new List<List<int>>
			{
				new List<int>
				{
					start
				}
			};

			var newPaths = new List<List<int>>();
			var visitedNodes = currentPaths.SelectMany(x => x).Distinct().ToList();

			foreach (var path in currentPaths.OrderBy(x => x.Count))
			{
				var leafPaths = this.valves[path.Last()].Connections
					.Select(x => new List<List<int>> { path, new List<int> { x } }.SelectMany(x => x).ToList());

				var completedPath = leafPaths.FirstOrDefault(x => x.Last() == end);
				if (completedPath != null)
				{
					return completedPath;
				}

				leafPaths = leafPaths.Where(x => !visitedNodes.Contains(x.Last()));
				newPaths.AddRange(leafPaths);
			}

			return CalculatePath(start, end, newPaths);
		}

		private static List<List<int>> DetermineOrders(List<int> input, int maxDepth, int currentDepth = 1)
		{
			if (currentDepth == maxDepth || input.Count == 1)
			{
				return input.Select(x => new List<int> { x }).ToList();
			}

			var options = new List<List<int>>();
			foreach (var value in input)
			{
				var newList = new List<int> { value };
				foreach (var data in DetermineOrders(input.Where(x => x != value).ToList(), maxDepth, currentDepth + 1) ?? new List<List<int>>())
				{
					options.Add(new List<List<int>> { newList, data }.SelectMany(x => x).ToList());
				}
			}

			return options;
		}

		private class Valve
		{
			internal int Id { get; set; }
			internal string Name { get; set; }
			internal int FlowRate { get; set; }
			internal List<int> Connections { get; set; }

			public override string ToString()
			{
				return $"{Id} - {this.FlowRate}";
			}
		}
	}
}
