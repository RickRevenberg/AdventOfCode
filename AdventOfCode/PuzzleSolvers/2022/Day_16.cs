
namespace AdventOfCode.PuzzleSolvers._2022
{
    using AdventOfCode.Logic.Extensions;
    using Logic.Modules;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    public class Day_16 : DayBase2022
	{
		public override int Day => 16;

		private bool testing = false;

		private Dictionary<int, Valve> valves;
        private static Dictionary<(int start, int end), List<int>> pathCache;

        private const string TestInput = @"Valve AA has flow rate=0; tunnels lead To valves DD, II, BB
Valve BB has flow rate=13; tunnels lead To valves CC, AA
Valve CC has flow rate=2; tunnels lead To valves DD, BB
Valve DD has flow rate=20; tunnels lead To valves CC, AA, EE
Valve EE has flow rate=3; tunnels lead To valves FF, DD
Valve FF has flow rate=0; tunnels lead To valves EE, GG
Valve GG has flow rate=0; tunnels lead To valves FF, HH
Valve HH has flow rate=22; tunnel leads To valve GG
Valve II has flow rate=0; tunnels lead To valves AA, JJ
Valve JJ has flow rate=21; tunnel leads To valve II";

		[SetUp]
		public async Task SetUp()
		{
			var input = testing
				? TestInput.Split("\r\n").ToList()
				: await this.SplitInput();

			valves = new Dictionary<int, Valve>();
            pathCache = new ();

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

		[Test]
		public void PartOne()
		{
            var paths = this.Solve(30);

            var bestPath = paths.MaxBy(x => x.PressureReleased);
            var answer = bestPath.PressureReleased + " ";
            answer += string.Join(" - ", bestPath.OpenValves.Select(x => this.valves[x].Name));

            answer.Pass();
        }

		[Test]
		public void PartTwo()
        {
            var paths = this.Solve(26);

			paths.ForEach(x =>
            {
                x.OpenValves.Remove(this.valves.Values.Single(x => x.Name == "AA").Id);
                x.OpenValves = x.OpenValves.OrderBy(y => y).ToList();
            });

            var possibilities = new SafeDictionary<int, List<(int, int)>>(defaultValue: _ => []);

            for (var i = 0; i < paths.Count; i++)
            {
                for (var j = i + 1; j < paths.Count; j++)
                {
					possibilities[paths[i].PressureReleased + paths[j].PressureReleased].Add((i, j));
                }
            }

            var answer = 0;
            foreach (var key in possibilities.Keys.OrderByDescending(x => x))
            {
                if (possibilities[key].Any(x =>
                        !new List<List<int>> { paths[x.Item1].OpenValves, paths[x.Item2].OpenValves }.SelectMany(x => x)
                            .ToList().ContainsDuplicate()))
                {
                    answer = key;
                    break;
                }
            }

			answer.Pass();
        }

        private List<PathPossibility> Solve(int timeRemaining)
        {
            var startId = this.valves.Values.Single(x => x.Name == "AA").Id;

            var paths = new List<PathPossibility>
            {
                new () { TimeRemaining = timeRemaining, OpenValves = [startId] }
            };

            foreach (var keyOne in this.valves.Keys)
            {
                foreach (var keyTwo in this.valves.Keys)
                {
                    if (keyOne == keyTwo)
                    {
                        continue;
                    }

                    CalculatePath(keyOne, keyTwo);
                }
            }

            var openPaths = paths.Where(x => !x.Completed).ToList();
            while (openPaths.Any())
            {
                foreach (var path in openPaths)
                {
                    var possibilities = this.valves.Values
                        .Where(x => x.FlowRate > 0)
                        .Where(x => !path.OpenValves.Contains(x.Id))
                        .Select(x => (x, pathCache[(path.OpenValves.Last(), x.Id)]))
                        .Where(x => x.Item2.Count < path.TimeRemaining)
                        .ToList();

                    if (!possibilities.Any())
                    {
                        path.Completed = true;
                        path.PressureReleased += path.TimeRemaining * path.OpenValves.Select(x => this.valves[x].FlowRate).Sum();

                        continue;
                    }

                    foreach (var possibility in possibilities)
                    {
                        var newPath = path.Clone();
                        var timeSpent = possibility.Item2.Count;
						
                        newPath.PressureReleased += timeSpent * newPath.OpenValves.Select(x => this.valves[x].FlowRate).Sum();
                        newPath.TimeRemaining -= timeSpent;
						newPath.OpenValves.Add(possibility.x.Id);

						paths.Add(newPath);
                    }

                    paths.Remove(path);
                }

                openPaths = paths.Where(x => !x.Completed).ToList();
            }

            return paths;
        }

		private List<int> CalculatePath(int start, int end, List<List<int>> currentPaths = null)
		{
            if (pathCache.TryGetValue((start, end), out var cachedPath))
            {
                return cachedPath;
            }

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
					pathCache.Add((start, end), completedPath);
					return completedPath;
				}

				leafPaths = leafPaths.Where(x => !visitedNodes.Contains(x.Last()));
				newPaths.AddRange(leafPaths);
			}

			return CalculatePath(start, end, newPaths);
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

        private class PathPossibility
        {
            public int TimeRemaining { get; set; }
            public List<int> OpenValves { get; set; } = [];
            public int PressureReleased { get; set; } = 0;
            public bool Completed { get; set; } = false;
        }
	}
}
