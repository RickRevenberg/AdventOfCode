namespace AdventOfCode.PuzzleSolvers._2022
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Logic;
	using NUnit.Framework;

	public class Day_14 : DayBase2022
	{
		public override int Day => 14;

		private List<List<Node>> grid;
		private Dictionary<int, Node> nodes;

		private int gridXStart;
		private static int width, height, leftStack, rightStack;

		[SetUp]
		public async Task SetUp()
		{
			grid = new();
			nodes = new();

			var input = await this.SplitInput();

			var formattedLines = input.Select(line => (List<(int posX, int posY)>)line.Split(" -> ").Select(x => (x.Split(",")[0].ToInt(), x.Split(",")[1].ToInt())).ToList()).ToList();

			var minX = formattedLines.SelectMany(x => x.Select(y => y.posX)).Min();
			var maxX = formattedLines.SelectMany(x => x.Select(y => y.posX)).Max();
			
			var maxY = formattedLines.SelectMany(x => x.Select(y => y.posY)).Max();

			gridXStart = minX;
			width = maxX - minX + 1;
			height = maxY + 1;

			for (var y = 0; y < height; y++)
			{
				grid.Add(new List<Node>());

				for (var x = 0; x < width; x++)
				{
					var node = new Node
					{
						Id = (y * width) + x
					};

					grid[y].Add(node);
					nodes.Add(node.Id, node);
				}
			}

			foreach (var line in formattedLines)
			{
				for (var i = 1; i < line.Count; i++)
				{
					var (from, to) = (line[i - 1], line[i]);

					if (from.posX != to.posX)
					{
						var (smallest, largest) = (Math.Min(from.posX, to.posX), Math.Max(from.posX, to.posX));
						for (var j = 0; j <= largest - smallest; j++)
						{
							grid[from.posY][smallest - gridXStart + j].IsRock = true;
						}
					}
					else
					{
						var (smallest, largest) = (Math.Min(from.posY, to.posY), Math.Max(from.posY, to.posY));
						for (var j = 0; j <= largest - smallest; j++)
						{
							grid[smallest + j][from.posX - gridXStart].IsRock = true;
						}
					}
				}
			}

			grid[0][500 - gridXStart].IsStart = true;
		}

		[Test]
		public override void PartOne()
		{
			var startingNodeId = this.grid.SelectMany(x => x).Single(x => x.IsStart).Id;
			Node trackingNode = null;

			while (true)
			{
				trackingNode ??= this.nodes[startingNodeId];

				var (target, final, _) = NodeFallTarget(trackingNode, false);
				if (target == null)
				{
					break;
				}

				if (final)
				{
					trackingNode.IsSand = true;
					trackingNode = null;
				}
				else
				{
					trackingNode = target;
				}
			}

			DrawGrid();
		}

		[Test]
		public override void PartTwo()
		{
			for (var i = 0; i < grid.Count; i++)
			{
				grid[i] = new List<List<Node>> { new() { new Node() }, grid[i], new() { new Node() } }.SelectMany(x => x).ToList();
			}

			width += 2;
			grid.Add(new List<Node>());
			grid.Add(new List<Node>());

			for (var i = 0; i < width; i++)
			{
				grid[height].Add(new Node());
				grid[height + 1].Add(new Node { IsRock = true });
			}

			height += 2;

			for (var y = 0; y < height; y++)
			{
				for (var x = 0; x < width; x++)
				{
					grid[y][x].Id = (y * width) + x;
				}
			}

			this.nodes.Clear();
			this.grid.SelectMany(x => x).ToList().ForEach(x => this.nodes.Add(x.Id, x));

			var startIndex = this.grid.SelectMany(x => x).Single(x => x.IsStart).Id;
			Node trackingNode = null;

			while (true)
			{
				trackingNode ??= nodes[startIndex];
				var (target, final, direction) = NodeFallTarget(trackingNode, true);

				if (target == null)
				{
					var stackSize = height - trackingNode.PosY - 1;
					var hill = direction == StackDirection.Left ? leftStack : rightStack;
					if (hill < HillSize(stackSize - 1))
					{
						leftStack += direction == StackDirection.Left ? 1 : 0;
						rightStack += direction == StackDirection.Left ? 0 : 1;

						continue;
					}

					trackingNode.IsSand = true;
					trackingNode = null;

					continue;
				}

				if (final)
				{
					trackingNode.IsSand = true;
					if (trackingNode.IsStart)
					{
						break;
					}

					trackingNode = null;
					continue;
				}

				trackingNode = target;
			}

			DrawGrid();
		}

		private (Node target, bool finalNode, StackDirection? direction) NodeFallTarget(Node current, bool useSideStack)
		{
			if (nodes.TryGetValue(current.Id + width, out var nodeBelow))
			{
				if (nodeBelow.IsFree)
				{
					return (nodeBelow, false, null);
				}
			}
			else
			{
				return (null, false, null);
			}

			if (nodes.TryGetValue(current.Id + width - 1, out var nodeBelowLeft) &&
			    nodeBelowLeft.PosY == current.PosY + 1)
			{
				if (nodeBelowLeft.IsFree)
				{
					return (nodeBelowLeft, false, null);
				}
			}
			else if (useSideStack)
			{
				var stackLeftMax = HillSize(height - current.PosY - 2);
				if (stackLeftMax > leftStack)
				{
					return (null, false, StackDirection.Left);
				}
			}
			else
			{
				return (null, false, StackDirection.Left);
			}

			if (nodes.TryGetValue(current.Id + width + 1, out var nodeBelowRight) &&
			    nodeBelowRight.PosY == current.PosY + 1)
			{
				if (nodeBelowRight.IsFree)
				{
					return (nodeBelowRight, false, null);
				}
			}
			else
			{
				return (null, false, StackDirection.Right);
			}

			return (current, true, null);
		}

		private void DrawGrid()
		{
			var drawnGrid = grid.Select(x => x.Select(x => x.IsStart ? "S" : x.IsRock ? "#" : x.IsSand ? "0" : ".").Join()).Join("\n");
			$"{grid.SelectMany(x => x).Count(x => x.IsSand) + leftStack + rightStack} \n\n{drawnGrid}".Pass();
		}

		private static int HillSize(int stackSize)
		{
			return stackSize == 0 ? 0 : stackSize + HillSize(stackSize - 1);
		}

		private class Node
		{
			internal int Id { get; set; }
			internal bool IsRock { get; set; }
			internal bool IsStart { get; set; }
			internal bool IsSand { get; set; }

			internal bool IsFree => !IsRock && !IsSand;
			internal int PosX => this.Id % width;
			internal int PosY => this.Id / width;
		}

		private enum StackDirection
		{
			Left = 0,
			Right = 1
		}
	}
}