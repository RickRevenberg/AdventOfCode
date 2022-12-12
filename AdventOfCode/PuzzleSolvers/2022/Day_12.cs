namespace AdventOfCode.PuzzleSolvers._2022
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Logic;
	using NUnit.Framework;

	public class Day_12 : DayBase2022
	{
		public override int Day => 12;

		private Dictionary<int, Node> nodes;

		private const string HeightMap = "abcdefghijklmnopqrstuvwxyz";

		[SetUp]
		public async Task SetUp()
		{
			var input = await this.SplitInput();
			this.nodes = new Dictionary<int, Node>();

			for (var y = 0; y < input.Count; y++)
			{
				var row = input[y].ToCharArray();
				for (var x = 0; x < row.Length; x++)
				{
					var id = y * row.Length + x;
					
					var start = row[x] == 'S';
					var finish = row[x] == 'E';
					
					nodes.Add(id, new Node
					{
						Id = id,
						PosX = x,
						PosY = y,
						IsStart = start,
						IsFinish = finish,
						Height = start ? 0 : finish ? 26 : HeightMap.IndexOf(row[x])
					});
				}
			}

			var width = input[0].Length;
			var height = input.Count;

			foreach (var key in nodes.Keys)
			{
				var node = nodes[key];
				
				node.Connections.Add(node.Id % width != 0 ? node.Id - 1 : -1);
				node.Connections.Add(node.Id >= width ? node.Id - width : -1);
				node.Connections.Add((node.Id + 1) % width != 0 ? node.Id + 1 : -1);
				node.Connections.Add((node.Id + width) < width * height ? node.Id + width : -1);

				node.Connections = node.Connections.Where(x => x != -1 && nodes[x].Height <= node.Height + 1).ToList();
			}
		}

		[Test]
		public override void PartOne()
		{
			var result = CalculateBestPath(this.nodes.Keys.Single(k => this.nodes[k].IsStart));

			// Stepcount equals node in path minus one.
			(result.Count - 1).Pass();
		}

		[Test]
		public override void PartTwo()
		{
			var startingOptions = this.nodes.Keys.Where(k => this.nodes[k].Height == 0).ToList();
			var routeLengths = startingOptions.Select(CalculateBestPath);

			// Stepcount equals node in path minus one.
			(routeLengths.Where(x => x != null).OrderBy(x => x.Count).First().Count - 1).Pass();
		}

		private List<int> CalculateBestPath(int startingIndex)
		{
			this.nodes.Keys.ToList().ForEach(key => this.nodes[key].Visited = false);

			var checkPaths = new List<List<int>> { new List<int> { startingIndex } };
			this.nodes[startingIndex].Visited = true;

			while (true)
			{
				var newPaths = new List<List<int>>();

				foreach (var path in checkPaths.OrderBy(x => x.Count))
				{
					var leaf = path.Last();
					var possibilities = nodes[leaf]
						.Connections.Where(x => !path.Contains(x))
						.Where(x => !nodes[x].Visited)
						.ToList();

					var leafPaths = possibilities.Select(x => new List<List<int>> { path, new() { x } }.SelectMany(y => y).ToList()).ToList();
					if (possibilities.Select(x => nodes[x]).SingleOrDefault(x => x.IsFinish) != null)
					{
						return leafPaths.Single(x => nodes[x.Last()].IsFinish);
					}

					possibilities.ForEach(x => nodes[x].Visited = true);
					newPaths.AddRange(leafPaths);
				}

				checkPaths = newPaths;

				if (!checkPaths.Any())
				{
					return null;
				}
			}
		}

		private class Node
		{
			internal int Id { get; init; }
			internal int PosX { get; init; }
			internal int PosY { get; init; }
			internal int Height { get; init; }

			internal bool IsStart { get; init; }
			internal bool IsFinish { get; init; }

			internal bool Visited { get; set; } = false;

			internal List<int> Connections { get; set; } = new List<int>();
		}
	}
}
