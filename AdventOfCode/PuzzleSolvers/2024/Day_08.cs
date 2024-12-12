namespace AdventOfCode.PuzzleSolvers._2024
{
    using Logic.Extensions;
    using Logic.Modules;

    public class Day_08 : DayBase2024
    {
        public override int Day => 8;

        private Grid<CityNode> grid;

        [SetUp]
        public async Task SetUp()
        {
            var input = await this.SplitInput();

            this.grid = Grid.CreateGrid<CityNode>(input[0].Length, input.Count, (x, y, node) =>
            {
                node.HasAntenna = input[y][x] != '.';
                node.Identifier = input[y][x];
            });
        }

        [Test]
        public void PartOne()
        {
            var antiNodeLocations = new List<string>();
            var antennaGroups = this.grid.Nodes.Values.Where(x => x.HasAntenna).GroupBy(x => x.Identifier).ToList();

            foreach (var group in antennaGroups)
            {
                foreach (var sourceAntenna in group)
                {
                    foreach (var targetAntenna in group)
                    {
                        if (sourceAntenna == targetAntenna)
                        {
                            continue;
                        }

                        var positionDelta = (targetAntenna.PosX - sourceAntenna.PosX, targetAntenna.PosY - sourceAntenna.PosY);
                        (int x, int y) antiNodeLocation = (targetAntenna.PosX + positionDelta.Item1, targetAntenna.PosY + positionDelta.Item2);

                        if (antiNodeLocation.x >= 0 && antiNodeLocation.y >= 0 &&
                            antiNodeLocation.x < this.grid.Width && antiNodeLocation.y < this.grid.Height)
                        {
                            antiNodeLocations.Add($"{antiNodeLocation.x}-{antiNodeLocation.y}");
                        }
                    }
                }
            }

            antiNodeLocations.Distinct().Count().Pass();
        }

        [Test]
        public void PartTwo()
        {
            var antiNodeLocations = new List<string>();
            var antennaGroups = this.grid.Nodes.Values.Where(x => x.HasAntenna).GroupBy(x => x.Identifier).ToList();

            foreach (var group in antennaGroups)
            {
                foreach (var sourceAntenna in group)
                {
                    foreach (var targetAntenna in group)
                    {
                        if (sourceAntenna == targetAntenna)
                        {
                            continue;
                        }

                        var tracker = 1;
                        (int x, int y) positionDelta = (targetAntenna.PosX - sourceAntenna.PosX, targetAntenna.PosY - sourceAntenna.PosY);

                        while (true)
                        {
                            (int x, int y) position = (sourceAntenna.PosX + positionDelta.x * tracker,
                                sourceAntenna.PosY + positionDelta.y * tracker);

                            if (position.x >= 0 && position.y >= 0 &&
                                position.x < this.grid.Width && position.y < this.grid.Height)
                            {
                                antiNodeLocations.Add($"{position.x}-{position.y}");
                                tracker++;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }

            antiNodeLocations.Distinct().Count().Pass();
        }

        private class CityNode : Grid.Node
        {
            internal bool HasAntenna { get; set; }
            internal char Identifier { get; set; }
        }
    }
}
