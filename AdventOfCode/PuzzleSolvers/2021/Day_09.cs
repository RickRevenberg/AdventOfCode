namespace AdventOfCode.PuzzleSolvers._2021
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using NUnit.Framework;

	[TestFixture]
    public class Day_09 : DayBase2021
    {
	    public override int Day => 9;

        private List<Node> nodes;
	    private Dictionary<int, Node> NodeDict = new Dictionary<int, Node>();

		[SetUp]
	    public async Task SetUp()
	    {
		    var rows = (await this.GetInput()).Split("\n").ToList();
		    var gridWidth = rows[0].Length;

		    this.nodes = rows.SelectMany((row, yIndex) => row.ToCharArray().Select((c, xIndex) => new Node
		    {
				Id = yIndex * gridWidth + xIndex,
			    PosX = xIndex,
			    PosY = yIndex,
			    Height = Convert.ToInt32(c.ToString())
		    })).ToList();

		    foreach (var node in this.nodes)
			{
				var left = node.Id % gridWidth != 0 ? node.Id - 1 : -1;
				var right = (node.Id + 1) % gridWidth != 0 ? node.Id + 1 : -1;
				var top = node.Id >= gridWidth ? node.Id - gridWidth : -1;
				var bottom = node.Id + gridWidth <= this.nodes.Count() ? node.Id + gridWidth : -1;

				node.NeighBourIds = new List<int> {top, right, bottom, left}.Where(x => x != -1).ToList();
			}

		    NodeDict = this.nodes.ToDictionary(x => x.Id);
		}

	    [Test]
	    public void PartOne()
	    {
		    var lowPoints = nodes.Where(node => node.NeighBourIds.All(id => NodeDict[id].Height > node.Height));
		    var score = lowPoints.Select(x => x.Height + 1).Sum();

			Assert.Pass(score.ToString());
	    }

		[Test]
	    public void PartTwo()
	    {
		    var lowPoints = nodes.Where(node => node.NeighBourIds.All(id => NodeDict[id].Height > node.Height)).ToList();

		    var basins = new List<List<Node>>();
		    foreach (var lowPoint in lowPoints)
		    {
			    var basin = new List<Node> { lowPoint };

			    while (true)
			    {
				    var newNodes = basin
					    .SelectMany(x => x.NeighBourIds.Select(id => NodeDict[id]).Where(x => !basin.Contains(x) && x.Height != 9))
					    .Distinct().ToList();

					basin.AddRange(newNodes);
					if (!newNodes.Any())
					{
						break;
					}
			    }

				basins.Add(basin);
		    }

		    var largestThree = basins.OrderByDescending(x => x.Count).Take(3).Select(x => x.Count).ToList();
		    var score = largestThree[0] * largestThree[1] * largestThree[2];

			Assert.Pass(score.ToString());
	    }

	    private class Node
	    {
			internal int Id { get; set; }
			internal int PosX { get; set; }
			internal int PosY { get; set; }
			internal int Height { get; set; }

			internal List<int> NeighBourIds { get; set; } = new List<int>();

			public override string ToString()
			{
				return $"{Id} - {Height}";
			}
	    }
    }
}
