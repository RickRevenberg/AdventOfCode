namespace AdventOfCode.PuzzleSolvers._2022
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AdventOfCode.Logic.Extensions;
    using Logic.Modules;
    using NUnit.Framework;

    public class Day_23 : DayBase2022
	{
		public override int Day => 23;

		private List<Proposition> propositions;

		private List<Elf> elves;
		private static SafeDictionary<int, SafeDictionary<int, Elf>> elfDict;

		private Proposition allClearProposition;

		[SetUp]
		public async Task SetUp()
		{
			var input = await this.SplitInput();

			elves = new List<Elf>();
			elfDict = new SafeDictionary<int, SafeDictionary<int, Elf>>((_) => new SafeDictionary<int, Elf>());

			for (var y = 0; y < input.Count; y++)
			{
				var row = input[y].ToCharArray();

				for (var x = 0; x < row.Length; x++)
				{
					var character = row[x];
					if (character == '#')
					{
						elfDict[x][y] = new Elf
						{
							PosX = x,
							PosY = y
						};

						this.elves.Add(elfDict[x][y]);
					}
				}
			}

			this.allClearProposition = new Proposition((0, 0), (-1, -1), (0, -1), (1, -1), (-1, 0), (1, 0), (-1, 1), (0, 1), (1, 1));

			this.propositions = new List<Proposition>
			{
				new Proposition((0, -1), (-1, -1), (0, -1), (1, -1)),
				new Proposition((0, 1), (-1, 1), (0, 1), (1, 1)),
				new Proposition((-1, 0), (-1, 1), (-1, 0), (-1, -1)),
				new Proposition((1, 0), (1, -1), (1, 0), (1, 1))
			};
		}

		[Test]
		public void PartOne()
		{
			for (var i = 0; i < 10; i++)
			{
				var positionsChanged = MovePositions();
				if (!positionsChanged)
				{
					break;
				}
			}

			var leftMost = this.elves.Min(x => x.PosX);
			var rightMost = this.elves.Max(x => x.PosX);
			var topMost = this.elves.Min(x => x.PosY);
			var bottomMost = this.elves.Max(x => x.PosY);

			var width = rightMost - leftMost + 1;
			var height = bottomMost - topMost + 1;

			(height * width - this.elves.Count).Pass();
		}

		[Test]
		public void PartTwo()
		{
			var tracker = 0;
			var positionsChanged = true;

			while (positionsChanged)
			{
				positionsChanged = MovePositions();
				tracker++;
			}

			tracker.Pass();
		}

		private bool MovePositions()
		{
			var data = new List<(int posX, int posY, int moveX, int moveY)>();

			foreach (var elf in this.elves.Where(elf => !allClearProposition.MoveValid(elf.PosX, elf.PosY)))
			{
				var proposedMove = this.propositions.FirstOrDefault(p => p.MoveValid(elf.PosX, elf.PosY))?.NewPosition(elf.PosX, elf.PosY);

				if (proposedMove != null)
				{
					data.Add((elf.PosX, elf.PosY, proposedMove.Value.newX, proposedMove.Value.newY));
				}
			}

			if (!data.Any())
			{
				return false;
			}

			foreach (var move in data)
			{
				if (data.Count(x => x.moveX == move.moveX && x.moveY == move.moveY) > 1)
				{
					continue;
				}

				elfDict[move.moveX][move.moveY] = elfDict[move.posX][move.posY];
				elfDict[move.posX][move.posY] = null;
				elfDict[move.moveX][move.moveY]!.UpdatePosition(move.moveX, move.moveY);
			}

			propositions.Add(propositions[0]);
			propositions = propositions.Skip(1).ToList();

			return true;
		}

		private class Elf
		{
			internal int PosX { get; set; }
			internal int PosY { get; set; }

			internal void UpdatePosition(int x, int y)
			{
				this.PosX = x;
				this.PosY = y;
			}
		}

		private class Proposition
		{
			private readonly (int modX, int modY) moveModifier;
			private readonly List<(int modX, int modY)> modifiers;

			internal Proposition((int modX, int modY) moveModifier, params (int modX, int modY)[] challenges)
			{
				this.moveModifier = moveModifier;
				this.modifiers = challenges.ToList();
			}

			internal bool MoveValid(int currX, int currY)
			{
				foreach (var modifier in modifiers)
				{
					var (newX, newY) = (currX + modifier.modX, currY + modifier.modY);
					if (elfDict[newX][newY] != null)
					{
						return false;
					}
				}

				return true;
			}

			internal (int newX, int newY) NewPosition(int currX, int currY)
			{
				return (currX + moveModifier.modX, currY + moveModifier.modY);
			}
		}
	}
}
