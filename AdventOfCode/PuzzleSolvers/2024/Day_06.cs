namespace AdventOfCode.PuzzleSolvers._2024
{
    using Logic.Extensions;
    using Logic.Modules;

    [TestFixture]
    public class Day_06 : DayBase2024
    {
        public override int Day => 6;

        private RoomGrid grid;

        [SetUp]
        public async Task SetUp()
        {
            var inputLines = await this.SplitInput();

            this.grid = PathFinder.CreateGrid<RoomGrid, RoomNode>(inputLines[0].Length, inputLines.Count, (x, y, node) =>
            {
                node.HasGuard = inputLines[y][x] == '^';
                node.IsBlocked = inputLines[y][x] == '#';

                if (node.HasGuard)
                {
                    node.VisitationHistory.Add(Direction.Up);
                }
            });

            this.grid.AddAllConnections();
        }

        [Test]
        public void PartOne()
        {
            var (_, visitedNodes, _) = SolveRoute();
            visitedNodes.Pass();
        }

        [Test]
        public void PartTwo()
        {
            var (route, _, _) = SolveRoute();
            var loopNodes = new List<int>();

            var distinctRoute = route.Distinct().ToList();

            for (var i = 1; i < distinctRoute.Count; i++)
            {
                distinctRoute[i].IsBlocked = true;
                var (_, _, containsLoop) = SolveRoute();
                
                if (containsLoop)
                {
                    loopNodes.Add(distinctRoute[i].Id);
                }

                distinctRoute[i].IsBlocked = false;
            }

            loopNodes.Count().Pass();
        }

        private (List<RoomNode> path, int uniqueNodeCount, bool containsLoop) SolveRoute()
        {
            this.grid.ResetGrid();

            var direction = Direction.Up;
            var currentNode = this.grid.Nodes.Values.Single(x => x.HasGuard);

            var path = new List<RoomNode> { currentNode };

            while (true)
            {
                var nextNode = NextNode(currentNode, direction);
                if (nextNode == null)
                {
                    break;
                }

                if (nextNode.IsBlocked)
                {
                    direction = RotateDirection(direction);

                    continue;
                }

                if (nextNode.IsVisited && nextNode.VisitationHistory.Contains(direction))
                {
                    return (path, this.grid.Nodes.Values.Count(x => x.IsVisited), true);
                }

                path.Add(nextNode);

                currentNode = nextNode;
                currentNode.VisitationHistory.Add(direction);
            }

            return (path, this.grid.Nodes.Values.Count(x => x.IsVisited), false);
        }

        private RoomNode NextNode(RoomNode currentNode, Direction dir)
        {
            var idDelta = GetIdDelta(dir);

            var nextNode = currentNode.Connections.SingleOrDefault(x => x == currentNode.Id + idDelta, -1);
            return nextNode == -1 ? null : this.grid.Nodes[nextNode];
        }

        private int GetIdDelta(Direction dir)
        {
            return dir switch
            {
                Direction.Up => -this.grid.Width,
                Direction.Right => 1,
                Direction.Down => this.grid.Width,
                Direction.Left => -1,
                _ => 0
            };
        }

        private static Direction RotateDirection(Direction dir)
        {
            return dir switch
            {
                Direction.Up => Direction.Right,
                Direction.Right => Direction.Down,
                Direction.Down => Direction.Left,
                Direction.Left => Direction.Up
            };
        }

        private class RoomGrid : PathFinder.Grid<RoomNode>
        {
            internal void ResetGrid()
            {
                base.Nodes.Values.ToList().ForEach(node => node.VisitationHistory.Clear());
                this.Nodes.Values.Single(x => x.HasGuard).VisitationHistory.Add(Direction.Up);
            }
        }

        private class RoomNode : PathFinder.Node
        {
            internal bool HasGuard { get; set; }
            internal bool IsBlocked { get; set; }
            internal bool IsVisited => this.VisitationHistory.Any();

            internal List<Direction> VisitationHistory { get; set; } = [];
        }
    }
}
