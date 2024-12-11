namespace AdventOfCode.PuzzleSolvers._2024
{
    using Logic.Extensions;
    using Logic.Modules;

    public class Day_10 : DayBase2024
    {
        public override int Day => 10;

        private PathFinder.Grid<TrailNode> grid;

        [SetUp]
        public async Task SetUp()
        {
            var input = await this.SplitInput();

            this.grid = PathFinder.CreateGrid<TrailNode>(input[0].Length, input.Count, (x, y, node) => node.Height = int.Parse(input[y][x].ToString()));
            this.grid.AddAllConnections((nodeFrom, nodeTo) => nodeTo.Height == nodeFrom.Height + 1);
        }

        [Test]
        public void PartOne()
        {
            var trailHeads = grid.Nodes.Values.Where(x => x.IsTrailHead).ToList();
            var trailEnds = grid.Nodes.Values.Where(x => x.IsTrailEnd).ToList();

            var totalScore = 0;

            foreach (var trailHead in trailHeads)
            {
                foreach (var trailEnd in trailEnds)
                {
                    var path = this.grid.CalculateShortestPath(trailHead.Id, trailEnd.Id);
                    if (path.route != null)
                    {
                        totalScore++;
                    }
                }
            }

            totalScore.Pass();
        }

        [Test]
        public void PartTwo()
        {
            var trailHeads = grid.Nodes.Values.Where(x => x.IsTrailHead).ToList();
            var trailEnds = grid.Nodes.Values.Where(x => x.IsTrailEnd).ToList();

            var totalScore = 0;

            foreach (var trailHead in trailHeads)
            {
                foreach (var trailEnd in trailEnds)
                {
                    var paths = this.grid.CalculateAllPossiblePaths(trailHead.Id, trailEnd.Id);
                    totalScore += paths.Count;
                }
            }

            totalScore.Pass();
        }

        private class TrailNode : PathFinder.Node
        {
            internal int Height { get; set; }

            internal bool IsTrailHead => this.Height == 0;
            internal bool IsTrailEnd => this.Height == 9;
        }
    }
}
