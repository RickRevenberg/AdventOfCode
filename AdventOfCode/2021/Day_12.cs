namespace AdventOfCode._2021
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using NUnit.Framework;

	public class Day_12
	{
		private Dictionary<string, Node> NodeDict;

		[SetUp]
		public void SetUp()
		{
			var nodes = Input.Split("\r\n").SelectMany(row => row.Split("-")).Distinct().Select(id => new Node
			{
				Id = id
			}).ToList();

			NodeDict = nodes.ToDictionary(x => x.Id);

			Input.Split("\r\n").ToList().ForEach(row =>
			{
				var endOne = row.Split("-")[0];
				var endTwo = row.Split("-")[1];

				NodeDict[endOne].Paths.Add(endTwo);
				NodeDict[endTwo].Paths.Add(endOne);
			});
		}

		[Test]
		public void PartOne()
		{
			var startingNode = NodeDict["start"];
			var possiblePaths = CalculatePaths(new List<Node> { startingNode }, SolvingMode.PartOne);

			Assert.Pass(possiblePaths.Count.ToString());
		}

		[Test]
		public void PartTwo()
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

	    private readonly string TestInput = @"start-A
start-b
A-c
A-b
b-d
A-end
b-end";

	    private readonly string TestInput2 = @"dc-end
HN-start
start-kj
dc-start
dc-HN
LN-dc
HN-end
kj-sa
kj-HN
kj-dc";

	    private readonly string TestInput3 = @"fs-end
he-DX
fs-he
start-DX
pj-DX
end-zg
zg-sl
zg-pj
pj-he
RW-he
fs-DX
pj-RW
zg-RW
start-pj
he-WI
zg-he
pj-fs
start-RW";


		private readonly string Input = @"GC-zi
end-zv
lk-ca
lk-zi
GC-ky
zi-ca
end-FU
iv-FU
lk-iv
lk-FU
GC-end
ca-zv
lk-GC
GC-zv
start-iv
zv-QQ
ca-GC
ca-FU
iv-ca
start-lk
zv-FU
start-zi";
    }
}
