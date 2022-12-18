namespace AdventOfCode.PuzzleSolvers._2022
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text.RegularExpressions;
	using System.Threading.Tasks;
	using AdventOfCode.PuzzleSolvers._2021.Helpers;
	using Logic;
	using NUnit.Framework;

	public class Day_18 : DayBase2022
	{
		public override int Day => 18;

		private List<(int x, int y, int z)> coordinates;

		[SetUp]
		public async Task SetUp()
		{
			var regex = new Regex("([0-9]+,?){3}");
			coordinates = regex.Matches(await this.GetInput()).Select(x => x.Value.Split(',').Select(x => x.ToInt()).ToList()).Select(x => (x[0], x[1], x[2])).ToList();
		}

		[Test]
		public override void PartOne()
		{
			var (highX, highY, highZ) = (coordinates.Select(x => x.x).Max(), coordinates.Select(x => x.y).Max(), coordinates.Select(x => x.z).Max());
			var grid = new bool[highX + 2, highY + 2, highZ + 2];

			foreach (var coordinate in coordinates)
			{
				var (x, y, z) = coordinate;
				grid[x, y, z] = true;
			}

			Solve(grid);
		}

		[Test]
		public override void PartTwo()
		{
			const int Addition = 2;

			coordinates = coordinates.Select(x => (x.x + Addition, x.y + Addition, x.z + Addition)).ToList();

			var (highX, highY, highZ) = (coordinates.Select(x => x.x).Max(), coordinates.Select(x => x.y).Max(), coordinates.Select(x => x.z).Max());
			var grid = new bool[highX + 1 + Addition, highY + 1 + Addition, highZ + 1 + Addition];

			foreach (var coordinate in coordinates)
			{
				var (x, y, z) = coordinate;
				grid[x, y, z] = true;
			}

			var rowSize = highX + Addition;
			var sliceSize = rowSize * (highY + Addition);
			var cubeSize = sliceSize * (highZ + Addition);

			var nodes = new List<Node>();
			for (var z = 0; z < highZ + Addition; z++)
			{
				for (var y = 0; y < highY + Addition; y++)
				{
					for (var x = 0; x < highX + Addition; x++)
					{
						var node = new Node
						{
							Id = nodes.Count,
							PosX = x,
							PosY = y,
							PosZ = z
						};

						node.neighbours.Add(x > 0 ? node.Id - 1 : -1);
						node.neighbours.Add((x + 1) < rowSize ? node.Id + 1 : -1);

						node.neighbours.Add(y > 0 ? node.Id - rowSize : -1);
						node.neighbours.Add((y + 1) < sliceSize - rowSize ? node.Id + rowSize : -1);

						node.neighbours.Add(z > 0 ? node.Id - sliceSize : -1);
						node.neighbours.Add(z < cubeSize - sliceSize - 1 ? node.Id + sliceSize : -1);

						nodes.Add(node);
					}
				}
			}

			var nodeDict = nodes.Where(node => !grid[node.PosX, node.PosY, node.PosZ]).ToDictionary(n => n.Id, n => n);
			nodes.ForEach(node =>
			{
				node.Visited = false;
				node.neighbours = node.neighbours.Where(x => x != -1 && nodeDict.ContainsKey(x)).ToList();
			});
			
			var outSideNodes = CalculatePaths(nodeDict, 0).SelectMany(x => x).Distinct().ToList();
			var insideNodes = nodes.Where(x => !outSideNodes.Contains(x.Id) && !grid[x.PosX, x.PosY, x.PosZ]).ToList();

			insideNodes.ForEach(node => grid[node.PosX, node.PosY, node.PosZ] = true);

			Solve(grid);
		}

		private static List<List<int>> CalculatePaths(Dictionary<int, Node> nodeDict, int startingNode)
		{
			var completedPaths = new List<List<int>>();
			var currentPaths = new List<List<int>> { new List<int> { startingNode } };

			nodeDict[startingNode].Visited = true;

			while (true)
			{
				var newPaths = new List<List<int>>();
				
				foreach (var path in currentPaths.OrderBy(x => x.Count))
				{
					var nextOptions = nodeDict[path.Last()].neighbours.Where(x => !nodeDict[x].Visited).ToList();
					if (!nextOptions.Any())
					{
						completedPaths.Add(path);
						continue;
					}

					var addedPaths = nextOptions.Select(x => new List<List<int>> { path, new() { x } }.SelectMany(x => x).ToList()).ToList();

					newPaths.AddRange(addedPaths);
					nextOptions.ForEach(x => nodeDict[x].Visited = true);
				}

				if (newPaths.Any())
				{
					currentPaths = newPaths;
					continue;
				}

				break;
			}

			return new List<List<List<int>>>{ completedPaths, currentPaths }.SelectMany(x => x).ToList();
		}

		private void Solve(bool[,,] grid)
		{
			var totalOpenSides = 0;
			foreach (var coordinate in coordinates)
			{
				var openSides = 6;
				var (x, y, z) = coordinate;

				for (var i = 0; i < 6; i++)
				{
					var modifier = i < 3 ? 1 : -1;
					var (xMod, yMod, zMod) = ((i % 3 == 0 ? 1 : 0) * modifier, ((i + 1) % 3 == 0 ? 1 : 0) * modifier, ((i + 2) % 3 == 0 ? 1 : 0) * modifier);

					var (xNew, yNew, zNew) = (x + xMod, y + yMod, z + zMod);

					if (xNew == -1 || yNew == -1 || zNew == -1 || xNew >= grid.GetLength(0) || yNew >= grid.GetLength(1) || zNew >= grid.GetLength(2))
					{
						continue;
					}

					if (grid[xNew, yNew, zNew])
					{
						openSides--;
					}
				}

				totalOpenSides += openSides;
			}

			totalOpenSides.Pass();
		}

		private class Node
		{
			internal int Id { get; set; }

			internal int PosX { get; set; }
			internal int PosY { get; set; }
			internal int PosZ { get; set; }

			internal bool Visited { get; set; }

			internal List<int> neighbours { get; set; } = new ();
		}
	}
}
