namespace AdventOfCode.PuzzleSolvers._2021
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using NUnit.Framework;

	[TestFixture]
	public class Day_11 : DayBase2021
	{
		public override int Day => 11;

        private int gridWidth = 0;
		private List<Octo> Octos;
		private Dictionary<int, Octo> OctoDict;

		[SetUp]
	    public async Task SetUp()
	    {
		    var input = await this.GetInput();

		    gridWidth = input.Split("\n").First().Length;
		    Octos = input.Split("\n").SelectMany((row, yPos) => row.ToCharArray().Select((c, xPos) => new Octo
		    {
				Id = (yPos * gridWidth) + xPos,
				PosX = xPos,
			    PosY = yPos,
			    Energy = int.Parse(c.ToString())
		    })).ToList();

		    Octos.ForEach(octo =>
		    {
			    var hasLeft = octo.PosX % gridWidth != 0;
			    var hasTop = octo.PosY > 0;
			    var hasRight = (octo.PosX + 1) % gridWidth != 0;
			    var hasBottom = octo.Id < Octos.Count - gridWidth;

			    octo.Neighbours.Add(hasLeft ? octo.Id - 1 : -1);
				octo.Neighbours.Add(hasLeft && hasTop ? octo.Id - 1 - gridWidth : -1);
				octo.Neighbours.Add(hasTop ? octo.Id - gridWidth : -1);
				octo.Neighbours.Add(hasTop && hasRight ? octo.Id + 1 - gridWidth : -1);
				octo.Neighbours.Add(hasRight ? octo.Id + 1 : -1);
				octo.Neighbours.Add(hasRight && hasBottom ? octo.Id + 1 + gridWidth : -1);
				octo.Neighbours.Add(hasBottom ? octo.Id + gridWidth : -1);
				octo.Neighbours.Add(hasBottom && hasLeft ? octo.Id - 1 + gridWidth : -1);

				octo.Neighbours = octo.Neighbours.Where(x => x >= 0).ToList();
		    });

		    OctoDict = Octos.ToDictionary(x => x.Id);
	    }

	    [Test]
	    public void PartOne()
	    {
		    var charges = 0;

		    for (var i = 0; i < 100; i++)
		    {
				Octos.ForEach(o =>
				{
					o.Energy++;
					o.flashed = false;
				});
				var chargedOctos = Octos.Where(o => o.Energy >= 10).ToList();

				while (chargedOctos.Any())
				{
					chargedOctos.ForEach(o =>
					{
						charges++;
						o.Energy = 0;
						o.flashed = true;
						o.Neighbours.ForEach(id => OctoDict[id].Energy += OctoDict[id].flashed ? 0 : 1);
					});

					chargedOctos = Octos.Where(o => o.Energy >= 10).ToList();
				}
		    }

			Assert.Pass(charges.ToString());
	    }

		[Test]
	    public void PartTwo()
	    {
		    var allFlashed = int.MaxValue;

		    for (var i = 0; i < allFlashed; i++)
		    {
			    Octos.ForEach(o =>
			    {
				    o.Energy++;
				    o.flashed = false;
			    });

			    var chargedOctos = Octos.Where(o => o.Energy >= 10).ToList();
			    while (chargedOctos.Any())
			    {
				    chargedOctos.ForEach(o =>
				    {
					    o.Energy = 0;
					    o.flashed = true;
						o.Neighbours.ForEach(id => OctoDict[id].Energy += OctoDict[id].flashed ? 0 : 1);
					});

				    chargedOctos = Octos.Where(o => o.Energy >= 10).ToList();
			    }

			    if (Octos.All(o => o.flashed))
			    {
				    allFlashed = i;
				    break;
			    }
		    }

		    var answer = allFlashed + 1;

			Assert.Pass(answer.ToString());
	    }

	    private class Octo
	    {
			internal int Id { get; set; }
			internal int PosX { get; set; }
			internal int PosY { get; set; }
			internal int Energy { get; set; }
			internal bool flashed { get; set; }
			internal List<int> Neighbours { get; set; } = new List<int>();
	    }
	}
}
