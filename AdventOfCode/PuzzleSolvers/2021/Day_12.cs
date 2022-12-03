namespace AdventOfCode.PuzzleSolvers._2021
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using NUnit.Framework;

	public class Day_12 : DayBase2021
	{
		public override int Day => 12;

        private Dictionary<string, Node> NodeDict;

		[SetUp]
		public async Task SetUp()
		{
			var input = await this.GetInput();

			var nodes = input.Split("\n").SelectMany(row => row.Split("-")).Distinct().Select(id => new Node
			{
				Id = id
			}).ToList();

			NodeDict = nodes.ToDictionary(x => x.Id);

			input.Split("\n").ToList().ForEach(row =>
			{
				var endOne = row.Split("-")[0];
				var endTwo = row.Split("-")[1];

				NodeDict[endOne].Paths.Add(endTwo);
				NodeDict[endTwo].Paths.Add(endOne);
			});
		}

		[Test]
		public override void PartOne()
		{
			var startingNode = NodeDict["start"];
			var possiblePaths = CalculatePaths(new List<Node> { startingNode }, SolvingMode.PartOne);

			Assert.Pass(possiblePaths.Count.ToString());
		}

		[Test]
		public override void PartTwo()
		{
			var startingNode = NodeDict["start"];
			var possiblePaths = CalculatePaths(new List<Node> { startingNode }, SolvingMode.PartTwo);

			Assert.Pass(possiblePaths.Count.ToString());
		}

		private List<List<Node>> CalculatePaths(List<Node> currentPath, SolvingMode mode)
		{
			var leaf = currentPath.Last();

			if (leaf.IsEnd)
			{
				return new List<List<Node>> { currentPath };
			}

			var validNextNodes = mode switch
			{
				SolvingMode.PartOne => GetValidNextNodesPartOne(currentPath, leaf),
				SolvingMode.PartTwo => GetValidNextNodesPartTwo(currentPath, leaf),
				_ => throw new ArgumentException()
			};

			if (!validNextNodes.Any())
			{
				return new List<List<Node>>();
			}

			return validNextNodes
				.Select(validNextNode => new List<List<Node>> {currentPath, new List<Node> {validNextNode}}.SelectMany(x => x).ToList())
				.SelectMany(x => CalculatePaths(x, mode)).Where(x => x.Any()).ToList();
		}

		private List<Node> GetValidNextNodesPartOne(ICollection<Node> currentPath, Node leaf)
		{
			return leaf.Paths
				.Select(id => NodeDict[id])
				.Where(node => node.Big || !currentPath.Contains(node)).ToList();
		}

		private List<Node> GetValidNextNodesPartTwo(ICollection<Node> currentPath, Node leaf)
		{
			static bool IsSmall(Node n) => !(n.IsStart || n.IsEnd || n.Big);

			return leaf.Paths
				.Select(id => NodeDict[id])
				.Where(node => !node.IsStart)
				.Where(node => node.Big || !currentPath.Contains(node) || 
				               currentPath.Where(IsSmall).Count() == currentPath.Where(IsSmall).Distinct().Count())
				.ToList();
		}

	    private class Node
	    {
			internal string Id { get; set; }
			internal bool Big => this.Id.ToUpper() == this.Id;
			internal bool IsStart => this.Id.Equals("start");
			internal bool IsEnd => this.Id.Equals("end");

			internal List<string> Paths = new List<string>();
	    }

	    private enum SolvingMode
	    {
			PartOne,
			PartTwo
	    }
	}
}
