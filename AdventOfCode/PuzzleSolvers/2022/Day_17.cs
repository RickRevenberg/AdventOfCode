namespace AdventOfCode.PuzzleSolvers._2022
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Runtime.CompilerServices;
	using System.Text.RegularExpressions;
	using System.Threading.Tasks;
	using Logic;
	using NUnit.Framework;

	public class Day_17 : DayBase2022
	{
		public override int Day => 17;

		//                         X               Y
		private static Dictionary<int, Dictionary<long, bool>> blockedPositions;

		private List<char> movementData;
		private List<char> shapes = new List<char> { '-', '+', 'l', '|', 'o' };

		private static List<Point> pixels;

		[SetUp]
		public async Task SetUp()
		{
			this.movementData =  (await this.GetInput()).ToCharArray().ToList();
			pixels = new List<Point> { new(), new(), new(), new(), new() };
		}

		[Test]
		public void PartOne()
		{
			this.Solve(2022).Pass();
		}

		[Test]
		public void PartTwo()
		{
			const long oneTrillion = 1000000000000;
			this.Solve(oneTrillion).Pass();
		}

		private long Solve(long rocks)
		{
			var cycleSize = movementData.Count * shapes.Count;
			RunIterations(cycleSize * 2, true, out var states);

			int startRepetitionIndex = -1, endRepetitionIndex = -1;

			for (var i = 0; i < states.Count; i++)
			{
				if (startRepetitionIndex != -1)
				{
					break;
				}

				for (var j = i + 1; j < states.Count; j++)
				{
					if (states[i] == states[j])
					{
						startRepetitionIndex = i;
						endRepetitionIndex = j;
						break;
					}
				}
			}

			var repetitionSize = endRepetitionIndex - startRepetitionIndex;

			var setup = RunIterations(startRepetitionIndex);
			var cycleStackSize = RunIterations(endRepetitionIndex) - setup;
			
			rocks -= startRepetitionIndex;

			var cyclesNeeded = rocks / repetitionSize!;
			var iterationsRemaining = rocks % repetitionSize;

			var remaining = RunIterations(endRepetitionIndex + (int)iterationsRemaining) - setup - cycleStackSize;

			return setup + (cyclesNeeded * cycleStackSize) + remaining;
		}

		private long RunIterations(int iterations)
		{
			return RunIterations(iterations, false, out _);
		}

		private long RunIterations(int iterations, bool recordStateData, out List<string> stateData)
		{
			var shapeTracker = 0;
			var movementTracker = 0;
			var highestMaxPoint = 0L;
			var movementCount = this.movementData.Count;

			stateData = new List<string>();

			blockedPositions = new Dictionary<int, Dictionary<long, bool>>
			{
				{ 0, new Dictionary<long, bool> { { 0, true } } },
				{ 1, new Dictionary<long, bool> { { 0, true } } },
				{ 2, new Dictionary<long, bool> { { 0, true } } },
				{ 3, new Dictionary<long, bool> { { 0, true } } },
				{ 4, new Dictionary<long, bool> { { 0, true } } },
				{ 5, new Dictionary<long, bool> { { 0, true } } },
				{ 6, new Dictionary<long, bool> { { 0, true } } },
			};

			for (long i = 0; i < iterations; i++)
			{
				SetNewShape(this.shapes[shapeTracker], highestMaxPoint);
				shapeTracker = (shapeTracker + 1) % 5;

				while (true)
				{
					var moveRight = movementData[movementTracker] == '>';
					if (moveRight && !pixels.Any(p => p.BlockedRight))
					{
						pixels.ForEach(x => x.PosX += 1);
					}
					else if (!moveRight && !pixels.Any(p => p.BlockedLeft))
					{
						pixels.ForEach(x => x.PosX -= 1);
					}

					movementTracker = (movementTracker + 1) % movementCount;

					if (pixels.Any(p => p.BlockedBelow))
					{
						pixels.Where(x => !x.Skipped).ToList().ForEach(p =>
						{
							blockedPositions[p.PosX].Add(p.PosY, true);
							highestMaxPoint = Math.Max(highestMaxPoint, p.PosY);
						});

						if (recordStateData)
						{
							var lowest = blockedPositions.Values.Select(x => x.Keys.Max()).Min();
							var state = $"{movementTracker}-{shapeTracker}-{blockedPositions.Values.Select(x => x.Keys.Max() - lowest).Join("-")}";
							stateData.Add(state);
						}

						break;
					}

					pixels.ForEach(p => p.PosY -= 1);
				}

				if (i % 100 == 0)
				{
					var lowestPoint = blockedPositions.Values.Select(x => x.Keys.Max()).Min() - 10;
					blockedPositions.Keys.ToList()
						.ForEach(key => blockedPositions[key] = blockedPositions[key]
							.Where(x => x.Key > lowestPoint)
							.ToDictionary(x => x.Key, x => x.Value));
				}
			}

			return highestMaxPoint;
		}

		private static void SetNewShape(char shape, long highestPoint)
		{
			switch (shape)
			{
				case '-':
					pixels[0].Set(2, highestPoint + 4);
					pixels[1].Set(3, highestPoint + 4);
					pixels[2].Set(4, highestPoint + 4);
					pixels[3].Set(5, highestPoint + 4);
					pixels[4].Skip();
					break;
				case '+':
					pixels[0].Set(3, highestPoint + 4);
					pixels[1].Set(2, highestPoint + 5);
					pixels[2].Skip();
					pixels[3].Set(4, highestPoint + 5);
					pixels[4].Set(3, highestPoint + 6);
					break;
				case 'l':
					pixels[0].Set(2, highestPoint + 4);
					pixels[1].Set(3, highestPoint + 4);
					pixels[2].Set(4, highestPoint + 4);
					pixels[3].Set(4, highestPoint + 5);
					pixels[4].Set(4, highestPoint + 6);
					break;
				case '|':
					pixels[0].Set(2, highestPoint + 4);
					pixels[1].Set(2, highestPoint + 5);
					pixels[2].Set(2, highestPoint + 6);
					pixels[3].Set(2, highestPoint + 7);
					pixels[4].Skip();
					break;
				case 'o':
					pixels[0].Set(2, highestPoint + 4);
					pixels[1].Set(3, highestPoint + 4);
					pixels[2].Set(2, highestPoint + 5);
					pixels[3].Set(3, highestPoint + 5);
					pixels[4].Skip();
					break;

			}
		}

		private class Point
		{
			internal int PosX { get; set; }
			internal long PosY { get; set; }

			internal bool Skipped { get; private set; }

			internal void Set(int x, long y)
			{
				this.PosX = x;
				this.PosY = y;
				this.Skipped = false;
			}

			internal void Skip()
			{
				this.Skipped = true;
			}

			internal bool BlockedBelow => !this.Skipped && (this.PosY == 1 ||
										  blockedPositions[this.PosX].ContainsKey(this.PosY - 1));

			internal bool BlockedLeft => !this.Skipped && (this.PosX == 0 ||
										 blockedPositions[this.PosX - 1].ContainsKey(this.PosY));

			internal bool BlockedRight => !this.Skipped && (this.PosX == 6 ||
										  blockedPositions[this.PosX + 1].ContainsKey(this.PosY));
		}
	}
}
