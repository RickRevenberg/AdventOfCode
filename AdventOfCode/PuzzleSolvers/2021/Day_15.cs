namespace AdventOfCode.PuzzleSolvers._2021
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using NUnit.Framework;

	[TestFixture]
    public class Day_15 : DayBase2021
    {
	    public override int Day => 15;

	    private string input;

        private int gridWidth;
	    private Dictionary<int, Node> NodeDict;

		[SetUp]
	    public async Task SetUp()
	    {
		    this.input = await this.GetInput();
	    }

	    [Test]
	    public override void PartOne()
	    {
		    Solution(1);
	    }

		[Test]
	    public override void PartTwo()
	    {
		    Solution(5);
	    }

	    private void Solution(int dimensionMultiplier)
	    {
		    ParseInput(dimensionMultiplier);

		    var answer = CalculateBestRouteScore();
		    Assert.Pass(answer.ToString());
	    }

        private void ParseInput(int dimensionMultiplier)
	    {
		    var rows = this.input.Split("\n").ToList();
		    gridWidth = rows.First().Length * dimensionMultiplier;

		    for (var i = 0; i < rows.Count; i++)
		    {
			    var newRow = rows[i];
			    var previousSection = rows[i];

			    for (var j = 2; j <= dimensionMultiplier; j++)
			    {
				    var newSection = string.Join("", previousSection.ToCharArray().Select(c => int.Parse(c.ToString())).Select(x => Math.Max(1, (x + 1) % 10)));

				    newRow += newSection;
				    previousSection = newSection;
			    }

			    rows[i] = newRow;
		    }

		    var originalRowCount = rows.Count;
		    for (var i = rows.Count; i < originalRowCount * dimensionMultiplier; i++)
		    {
			    var previousRow = rows[i - originalRowCount];
			    var newRow = string.Join("", previousRow.ToCharArray().Select(c => int.Parse(c.ToString())).Select(x => Math.Max(1, (x + 1) % 10)));

			    rows.Add(newRow);
		    }

		    var nodes = rows
			    .SelectMany((row, y) => row
				    .ToCharArray()
				    .Select(c => int.Parse(c.ToString()))
				    .Select((n, x) => new Node
				    {
					    Id = (y * gridWidth) + x,
					    RiskLevel = n
				    })).ToList();

		    nodes.First().RouteCost = 0;
		    nodes.First().RiskLevel = 0;

		    nodes.ForEach(node =>
		    {
			    var left = node.Id % gridWidth != 0 ? node.Id - 1 : -1;
			    var top = node.Id > gridWidth ? node.Id - gridWidth : -1;
			    var right = (node.Id + 1) % gridWidth != 0 ? node.Id + 1 : -1;
			    var bottom = (node.Id + gridWidth) < nodes.Count ? node.Id + gridWidth : -1;

			    node.Connections = new List<int> { left, top, right, bottom }.Where(x => x != -1).ToList();
		    });

		    NodeDict = nodes.ToDictionary(x => x.Id);
	    }

		// O(n) aww yeah
	    private int CalculateBestRouteScore()
	    {
			var endId = NodeDict.Keys.Count - 1;
			var currentLeafs = new List<int> { 0 };

			while (currentLeafs.Any())
			{
				var newLeafs = new List<int>();
				foreach (var leafId in currentLeafs)
			    {
				    var leaf = NodeDict[leafId];
				    foreach (var newLeafId in leaf.Connections)
				    {
					    var newLeaf = NodeDict[newLeafId];
					    var cost = leaf.RouteCost + newLeaf.RiskLevel;

					    if (cost < newLeaf.RouteCost)
					    {
						    newLeaf.RouteCost = cost;

						    if (newLeafId != endId)
						    {
							    newLeafs.Add(newLeafId);
						    }
					    }
				    }
			    }

			    currentLeafs = newLeafs;
			}

		    return NodeDict[endId].RouteCost;
	    }

	    private class Node
	    {
		    internal int Id { get; set; }
		    internal int RiskLevel { get; set; }

		    internal int RouteCost { get; set; } = int.MaxValue;

		    internal List<int> Connections;

		    public override string ToString()
			{
				return $"{this.Id}: {this.RiskLevel} - {this.RouteCost}";
			}
	    }
    }
}
