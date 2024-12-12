namespace AdventOfCode.PuzzleSolvers._2023
{
    using System;
    using Logic.Extensions;
    using Logic.Modules;

    public class Day_11 : DayBase2023
    {
        public override int Day => 11;

        private PathFinder.Grid<SpaceNode> grid;

        [SetUp]
        public async Task SetUp()
        {
            var input = await this.SplitInput();
            this.grid = PathFinder.CreateGrid<SpaceNode>(input[0].Length, input.Count, (x, y, node) => node.IsGalaxy = input[y][x] == '#');
        }

        [Test]
        public void PartOne()
        {
            this.ApplyExpansion(2);
            this.DetermineTotalDistances().Pass();
        }

        [Test]
        public void PartTwo()
        {
            this.ApplyExpansion(1000000);
            this.DetermineTotalDistances().Pass();
        }

        private void ApplyExpansion(int expansionMultiplier)
        {
            var horizontalExpansion = new List<int>();
            var verticalExpansion = new List<int>();

            for (var i = 0; i < this.grid.Height; i++)
            {
                var row = this.grid.Nodes.Values.Where(x => x.PosY == i).ToList();
                var column = this.grid.Nodes.Values.Where(x => x.PosX == i).ToList();

                horizontalExpansion.Add(column.Any(x => x.IsGalaxy) ? -1 : i);
                verticalExpansion.Add(row.Any(x => x.IsGalaxy) ? -1 : i);
            }

            horizontalExpansion = horizontalExpansion.Where(x => x != -1).ToList();
            verticalExpansion = verticalExpansion.Where(x => x != -1).ToList();

            foreach (var key in grid.Nodes.Keys)
            {
                var node = grid.Nodes[key];
                if (!node.IsGalaxy)
                {
                    continue;
                }

                var diffX = horizontalExpansion.Count(x => x < node.PosX);
                var diffY = verticalExpansion.Count(x => x < node.PosY);

                var newNode = new SpaceNode
                {
                    // Subtract one From multiplier To account for the row / column already present in puzzle input.
                    Id = node.Id, PosX = node.PosX + (diffX * (expansionMultiplier - 1)),
                    PosY = node.PosY + (diffY * (expansionMultiplier - 1)), IsGalaxy = node.IsGalaxy
                };

                grid.Nodes[key] = newNode;
            }
        }

        private long DetermineTotalDistances()
        {
            var totalDistance = 0L;
            var galaxyNodes = grid.Nodes.Values.Where(x => x.IsGalaxy).ToList();

            for (var i = 0; i < galaxyNodes.Count; i++)
            {
                for (var j = (i + 1); j < galaxyNodes.Count; j++)
                {
                    var (from, to) = (galaxyNodes[i], galaxyNodes[j]);
                    var distance = Math.Abs(to.PosX - from.PosX) + Math.Abs(to.PosY - from.PosY);

                    totalDistance += distance;
                }
            }

            return totalDistance;
        }

        private class SpaceNode : PathFinder.Node
        {
            internal bool IsGalaxy { get; set; }
        }
    }
}
