namespace AdventOfCode.PuzzleSolvers._2022
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AdventOfCode.Logic.Extensions;
    using Logic.Modules;
    using NUnit.Framework;

    public class Day_12 : DayBase2022
	{
		public override int Day => 12;

		private PathFinder.Grid<Node> Grid;

		private Dictionary<int, Node> nodes => Grid.Nodes;

		private const string HeightMap = "abcdefghijklmnopqrstuvwxyz";

		[SetUp]
		public async Task SetUp()
		{
			var input = await this.SplitInput();
			var rows = input.Select((x, i) => (i, x.ToCharArray())).ToDictionary(x => x.i, x => x.Item2);


			this.Grid = PathFinder.CreateGrid<Node>(input[0].ToCharArray().Length, input.Count, (x, y, node) =>
			{
				node.IsStart = rows[y][x] == 'S';
				node.IsFinish = rows[y][x] == 'E';
				node.Height = node.IsStart ? 0 : node.IsFinish ? 26 : HeightMap.IndexOf(rows[y][x]);
			}).AddAllConnections(false, (node, connection) => connection.Height <= node.Height + 1);
		}

		[Test]
		public void PartOne()
		{
			var endIndex = this.Grid.Nodes.Values.Single(x => x.IsFinish).Id;
			var result = this.Grid.CalculateShortestPath(this.Grid.Nodes.Values.Single(x => x.IsStart).Id, endIndex);

			result.length.Pass();
		}

		[Test]
		public void PartTwo()
		{
			var endIndex = this.Grid.Nodes.Values.Single(x => x.IsFinish).Id;
            var startingOptions = this.nodes.Keys.Where(k => this.nodes[k].Height == 0).ToList();

			var routeLengths = startingOptions.Select(start => this.Grid.CalculateShortestPath(start, endIndex));

			var bestRoute = routeLengths.Where(x => x.route != null).MinBy(x => x.length);
			bestRoute.length.Pass();
		}

		private class Node : PathFinder.Node 
		{
			internal int Height { get; set; }

			internal bool IsStart { get; set; }
			internal bool IsFinish { get; set; }
		}
	}
}
