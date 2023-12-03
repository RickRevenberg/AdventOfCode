namespace AdventOfCode.PuzzleSolvers._2022
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using AdventOfCode.Logic.Extensions;
    using Logic.Modules;
    using NUnit.Framework;

    public class Day_22 : DayBase2022
	{
		public override int Day => 22;

		private Dictionary<int, Node> nodeDict;

		private int boardWidth;
		private int cubeEdgeSize;

		private List<(int distance, string turnDirection)> instructions;
		private Dictionary<int, CubeFace> faceDict;

		[SetUp]
		public async Task SetUp()
		{
			var input = await this.SplitInput();

			var instructionRegex = new Regex("[0-9]+_(R|L|F)?");
			this.instructions = instructionRegex
				.Matches($"{input.Last().Replace("R", "_R").Replace("L", "_L")}_F")
				.Select(x => (x.Value.Split('_')[0].ToInt(), x.Value.Split('_')[1])).ToList();

			input = input.Take(input.Count - 2).ToList();

			this.boardWidth = input.OrderBy(x => x.Length).Last().Length;

			nodeDict = new Dictionary<int, Node>();

			for (var y = 0; y < input.Count; y++)
			{
				var row = input[y].ToCharArray().ToList();

				var skipCount = row.Count - input[y].TrimStart(' ').Length;
				for (var x = skipCount; x < row.Count; x++)
				{
					nodeDict.Add(y * boardWidth + x, new Node
					{
						Id = y * boardWidth + x,
						IsWall = row[x] == '#',
						PosX = x,
						PosY = y
					});
				}
			}

			this.cubeEdgeSize = (int)Math.Sqrt(nodeDict.Count / 6);

			this.faceDict = new Dictionary<int, CubeFace>();
			var faceCorners = new Dictionary<int, ((int start, int end) x, (int start, int end) y)>();

			for (var y = 0; y < input.Count; y += cubeEdgeSize)
			{
				for (var x = 0; x < this.boardWidth; x += cubeEdgeSize)
				{
					var id = y * boardWidth + x;
					if (nodeDict.ContainsKey(id))
					{
						faceDict.Add(faceCorners.Keys.Count, new CubeFace{ Id = faceCorners.Keys.Count });
						faceCorners.Add(faceCorners.Keys.Count, DetermineCorners(id));
					}
				}
			}

			foreach (var node in nodeDict.Values)
			{
				node.FaceId = faceCorners.Keys.Single(k =>
					faceCorners[k].x.start <= node.PosX && faceCorners[k].x.end >= node.PosX && 
					faceCorners[k].y.start <= node.PosY && faceCorners[k].y.end >= node.PosY);
			}

			for (var i = 0; i < 6; i++)
			{
				var topLeft = nodeDict.Values.First(x => x.FaceId == i);
				if (nodeDict.TryGetValue(topLeft.Id - 1, out var leftNode) && topLeft.Id % boardWidth != 0)
				{
					faceDict[i].Borders[Direction.Left] = faceDict[leftNode.FaceId];
				}

				if (nodeDict.TryGetValue(topLeft.Id - boardWidth, out var topNode))
				{
					faceDict[i].Borders[Direction.Up] = faceDict[topNode.FaceId];
				}

				var bottomRight = nodeDict.Values.Last(x => x.FaceId == i);
				if (nodeDict.TryGetValue(bottomRight.Id + 1, out var rightNode) && (bottomRight.Id + 1) % boardWidth != 0)
				{
					faceDict[i].Borders[Direction.Right] = faceDict[rightNode.FaceId];
				}

				if (nodeDict.TryGetValue(bottomRight.Id + boardWidth, out var bottomNode))
				{
					faceDict[i].Borders[Direction.Down] = faceDict[bottomNode.FaceId];
				}
			}

			foreach (var topSide in faceDict.Values)
			{
				if (faceDict.Values.All(x => x.Borders.Count == 4 && x.Borders.All(y => y.Value != null)))
				{
					break;
				}

				topSide.YIndex = 0;
				topSide.Borders.Values.ToList().ForEach(face => face.YIndex = 1);

				foreach (var cubeFace in faceDict.Values.Where(x => x.YIndex == 1))
				{
					var topDirection = cubeFace.Borders.Single(x => x.Value == topSide).Key;
					var fromDirection = topSide.Borders.Single(x => x.Value == cubeFace).Key;

					var neighbourOne = topSide.Borders[(Direction)(((int)fromDirection + 1) % 4)];
					if (neighbourOne != null)
					{
						var neighBourDirection = neighbourOne.Borders.Single(x => x.Value == topSide).Key;
						neighbourOne.Borders[(Direction)(((int)neighBourDirection + 1) % 4)] = cubeFace;
						cubeFace.Borders[(Direction)(((int)topDirection + 3) % 4)] = neighbourOne;
					}

					var neighBourTwo = topSide.Borders[(Direction)(((int)fromDirection + 3) % 4)];
					if (neighBourTwo != null)
					{
						var neighBourDirection = neighBourTwo.Borders.Single(x => x.Value == topSide).Key;
						neighBourTwo.Borders[(Direction)(((int)neighBourDirection + 3) % 4)] = cubeFace;
						cubeFace.Borders[(Direction)(((int)topDirection + 1) % 4)] = neighBourTwo;
					}
				}

				faceDict.Values.ToList().ForEach(x => x.YIndex = -1);
			}
			while (faceDict.Values.Any(x => x.Borders.Count < 4 || x.Borders.Any(y => y.Value == null)))
			{
				foreach (var cubeFace in faceDict.Values.Where(x => x.Borders.Count < 4 || x.Borders.Any(y => y.Value == null)))
				{
					for (var i = 0; i < 4; i++)
					{
						var connection = cubeFace.Borders[(Direction)i];
						if (connection != null)
						{
							continue;
						}

						var sibling = cubeFace.Borders[(Direction)((i + 3) % 4)];
						if (sibling == null)
						{
							continue;
						}

						var siblingToFace = sibling.Borders.Single(x => x.Value == cubeFace).Key;
						var target = sibling.Borders[(Direction)(((int)siblingToFace + 3) % 4)];

						var targetToSibling = target.Borders.Single(x => x.Value == sibling).Key;

						cubeFace.Borders[(Direction)i] = target;
						target.Borders[(Direction)(((int)targetToSibling + 3) % 4)] = cubeFace;
					}
				}
			}
		}

		[Test]
		public void PartOne()
		{
			nodeDict.Values.ToList().ForEach(node => node.FaceId = 0);
			SetStandardNeighBours(true);

			this.Solve();
		}

		[Test]
		public void PartTwo()
		{
			SetStandardNeighBours(false);
			foreach (var key in this.faceDict.Keys)
			{
				var face = this.faceDict[key];
				var faceNodes = nodeDict.Values.Where(x => x.FaceId == key).ToList();

				foreach (var direction in face.Borders.Keys)
				{
					var faceEdgeNodes = (direction switch
					{
						Direction.Right => faceNodes.Where(x => x.PosX == faceNodes.Last().PosX),
						Direction.Down => faceNodes.Where(x => x.PosY == faceNodes.Last().PosY),
						Direction.Left => faceNodes.Where(x => x.PosX == faceNodes.First().PosX),
						Direction.Up => faceNodes.Where(x => x.PosY == faceNodes.First().PosY),
						_ => throw new ArgumentOutOfRangeException()
					}).ToList();

					var target = face.Borders[direction];
					var targetNodes = nodeDict.Values.Where(x => x.FaceId == target.Id).ToList();
					var targetToFace = target.Borders.Single(x => x.Value == face).Key;

					var targetEdgeNodes = (targetToFace switch
					{
						Direction.Right => targetNodes.Where(x => x.PosX == targetNodes.Last().PosX),
						Direction.Down => targetNodes.Where(x => x.PosY == targetNodes.Last().PosY),
						Direction.Left => targetNodes.Where(x => x.PosX == targetNodes.First().PosX),
						Direction.Up => targetNodes.Where(x => x.PosY == targetNodes.First().PosY),
						_ => throw new ArgumentOutOfRangeException()
					}).ToList();

					if (direction == targetToFace)
					{
						targetEdgeNodes.Reverse();
					}

					for (var i = 0; i < this.cubeEdgeSize; i++)
					{
						if (!faceEdgeNodes[i].NeighBours.ContainsKey(direction))
						{
							faceEdgeNodes[i].NeighBours.Add(direction, targetEdgeNodes[i].Id);
						}
					}
				}
			}

			this.Solve();
		}

		private void SetStandardNeighBours(bool wrapAround)
		{
			foreach (var node in nodeDict.Values)
			{
				node.NeighBours.Add(Direction.Left, node.Id % boardWidth != 0
					? nodeDict.ContainsKey(node.Id - 1)
						? node.Id - 1
						: wrapAround ? nodeDict.Values.Where(x => x.PosY == node.PosY).Max(x => x.Id) : -1
					: wrapAround ? nodeDict.Values.Where(x => x.PosY == node.PosY).Max(x => x.Id) : -1);

				node.NeighBours.Add(Direction.Right, (node.Id + 1) % boardWidth != 0
					? nodeDict.ContainsKey(node.Id + 1)
						? node.Id + 1
						: wrapAround ? nodeDict.Values.Where(x => x.PosY == node.PosY).Min(x => x.Id) : -1
					: wrapAround ? nodeDict.Values.Where(x => x.PosY == node.PosY).Min(x => x.Id) : -1);

				node.NeighBours.Add(Direction.Up, nodeDict.ContainsKey(node.Id - boardWidth)
					? node.Id - boardWidth
					: wrapAround ? nodeDict.Values.Where(x => x.PosX == node.PosX).Max(x => x.Id) : -1);

				node.NeighBours.Add(Direction.Down, nodeDict.ContainsKey(node.Id + boardWidth)
					? node.Id + boardWidth
					: wrapAround ? nodeDict.Values.Where(x => x.PosX == node.PosX).Min(x => x.Id) : -1);

				node.NeighBours = node.NeighBours.Where(x => x.Value != -1).ToDictionary(x => x.Key, x => x.Value);
			}
		}

		private void Solve()
		{
			var currentNode = nodeDict[nodeDict.Keys.Min()];
			var currentDirection = Direction.Right;

			foreach (var instruction in instructions)
			{
				for (var i = 0; i < instruction.distance; i++)
				{
					if (nodeDict[currentNode.NeighBours[currentDirection]].IsWall)
					{
						break;
					}

					var nextNode = nodeDict[currentNode.NeighBours[currentDirection]];

					if (nextNode.FaceId != currentNode.FaceId)
					{
						var targetToCurrent = faceDict[nextNode.FaceId].Borders
							.Single(x => x.Value == faceDict[currentNode.FaceId]).Key;

						currentDirection = (Direction)(((int)targetToCurrent + 2) % 4);
					}

					currentNode = nextNode;
				}

				currentDirection = MapDirection(currentDirection, instruction.turnDirection);
			}

			var row = currentNode.PosY + 1;
			var column = currentNode.PosX + 1;
			var answer = (1000 * row) + (4 * column) + ((int)currentDirection);
			answer.Pass();
		}

		private static Direction MapDirection(Direction currentDirection, string turnDirection)
		{
			return currentDirection switch
			{
				Direction.Left => turnDirection switch
				{
					"L" => Direction.Down,
					"R" => Direction.Up,
					_ => currentDirection
				},
				Direction.Up => turnDirection switch
				{
					"L" => Direction.Left,
					"R" => Direction.Right,
					_ => currentDirection
				},
				Direction.Right => turnDirection switch
				{
					"L" => Direction.Up,
					"R" => Direction.Down,
					_ => currentDirection
				},
				Direction.Down => turnDirection switch
				{
					"L" => Direction.Right,
					"R" => Direction.Left,
					_ => currentDirection
				},
				_ => throw new Exception()
			};
		}

		private ((int, int), (int, int)) DetermineCorners(int topLeftId)
		{
			return (
				(nodeDict[topLeftId].PosX, nodeDict[topLeftId].PosX + this.cubeEdgeSize - 1),
				(nodeDict[topLeftId].PosY, nodeDict[topLeftId].PosY + this.cubeEdgeSize - 1));
		}

		private class CubeFace
		{
			internal int Id { get; init; }

			internal int YIndex { get; set; } = -1;

			internal readonly SafeDictionary<Direction, CubeFace> Borders = new ();
		}

		private class Node
		{
			internal int Id { get; init; }

			internal int FaceId { get; set; }

			internal bool IsWall { get; init; }

			internal int PosX { get; init; }
			internal int PosY { get; init; }

			internal Dictionary<Direction, int> NeighBours { get; set; } = new ();
		}

		private enum Direction
		{
			Right = 0,
			Down = 1,
			Left = 2,
			Up = 3
		}
	}
}
