namespace AdventOfCode.PuzzleSolvers._2023
{
    using Logic.Extensions;
    using Logic.Modules;

    public class Day_10 : DayBase2023
    {
        public override int Day => 10;

        private List<string> input;

        [SetUp]
        public async Task SetUp()
        {
            this.input = await this.SplitInput();
        }

        [Test]
        public void PartOne()
        {
            var grid = ParseInputToGrid();
            var mainLoop = DetermineMainLoop(grid);

            var furthestNode = (mainLoop.Count / 2);
            furthestNode.Pass();
        }

        [Test]
        public void PartTwo()
        {
            var grid = ParseInputToGrid();
            var mainLoop = DetermineMainLoop(grid);

            var mainLoopDict = mainLoop.ToDictionary(x => x, _ => true);
            grid.Nodes.Where(x => !mainLoopDict.ContainsKey(x.Key)).Select(x => x.Key).ToList().ForEach(key =>
            {
                grid.Nodes[key].Connections.Clear();
                grid.Nodes[key].IsGround = true;
            });

            grid.AddAllConnections((from, to) => !mainLoopDict.ContainsKey(from.Id) && !mainLoopDict.ContainsKey(to.Id));

            for (var i = 0; i < mainLoop.Count; i++)
            {
                var prev = i - 1;
                var curr = i;

                prev = prev < 0 ? mainLoop.Count - 1 : prev;

                var (from, to) = (grid.Nodes[mainLoop[prev]], grid.Nodes[mainLoop[curr]]);
                var (xDiff, yDiff) = (to.PosX - from.PosX, to.PosY - from.PosY);
                var (actX, actY) = (from.PosX + (xDiff / 2), from.PosY + (yDiff / 2));
                var id = (actY * grid.Width) + actX;

                var skippedNode = grid.Nodes[id];
                skippedNode.Connections.ForEach(id => grid.Nodes[id].Connections.Remove(skippedNode.Id));
                skippedNode.Connections.Clear();
            }

            var edgeGroup = GetConnectedGroup(grid, 0);
            var enclosedNodes = grid.Nodes.Values.Where(x => x.IsGround && x.PosX % 2 == 0 && x.PosY % 2 == 0 && !edgeGroup.Contains(x.Id)).ToList();

            enclosedNodes.Count.Pass();
        }

        private static List<int> GetConnectedGroup(PathFinder.Grid<PipeNode> grid, int startId)
        {
            var dict = new Dictionary<int, int> { { startId, startId } };
            var consideredEdges = new List<int> { startId };

            while (consideredEdges.Any())
            {
                var newEdge = new List<int>();
                foreach (var edge in consideredEdges)
                {
                    grid.Nodes[edge].Connections.Where(x => !dict.ContainsKey(x)).ToList().ForEach(key =>
                    {
                        dict.Add(key, key);
                        newEdge.Add(key);
                    });
                }

                consideredEdges = newEdge.Distinct().ToList();
            }

            return dict.Values.ToList();
        }

        private static List<int> DetermineMainLoop(PathFinder.Grid<PipeNode> grid)
        {
            var (key, node) = grid.Nodes.Single(x => x.Value.IsStart);
            var previousKey = key;

            var nodes = new List<int> { key };

            while (true)
            {
                var nextKey = node.Connections.First(x => x != previousKey);

                previousKey = key;
                key = nextKey;
                node = grid.Nodes[key];

                if (node.IsStart)
                {
                    break;
                }

                nodes.Add(key);
            }

            return nodes;
        }

        private PathFinder.Grid<PipeNode> ParseInputToGrid()
        {
            var nodeData = input.Select(x => x.ToCharArray().ToList()).ToList();
            var grid = PathFinder.CreateGrid<PipeNode>(this.input[0].Length * 2, this.input.Count * 2, (x, y, node) => node.Value = (x % 2 != 0 || y % 2 != 0 ? '.' : nodeData[y / 2][x / 2]));

            grid.AddAllConnections(x => x.StepSize = 2, (from, to) =>
                from.Value == '|' && from.PosX == to.PosX &&
                    ((from.PosY > to.PosY && to.Value.In(Symbols('S')) || (from.PosY < to.PosY && to.Value.In(Symbols('N'))))) ||
                from.Value == '-' && from.PosY == to.PosY &&
                    ((from.PosX > to.PosX && to.Value.In(Symbols('E')) || (from.PosX < to.PosX && to.Value.In(Symbols('W'))))) ||
                from.Value == 'L' &&
                    ((from.PosX == to.PosX && from.PosY > to.PosY && to.Value.In(Symbols('S'))) || (from.PosX < to.PosX && from.PosY == to.PosY && to.Value.In(Symbols('W')))) ||
                from.Value == 'J' &&
                    ((from.PosX > to.PosX && from.PosY == to.PosY && to.Value.In(Symbols('E'))) || (from.PosX == to.PosX && from.PosY > to.PosY && to.Value.In(Symbols('S')))) ||
                from.Value == '7' &&
                    ((from.PosX > to.PosX && from.PosY == to.PosY && to.Value.In(Symbols('E'))) || (from.PosX == to.PosX && from.PosY < to.PosY && to.Value.In(Symbols('N')))) ||
                from.Value == 'F' &&
                    ((from.PosX < to.PosX && from.PosY == to.PosY && to.Value.In(Symbols('W'))) || (from.PosX == to.PosX && from.PosY < to.PosY && to.Value.In(Symbols('N')))));

            var startNode = grid.Nodes.Values.Single(x => x.Value == 'S');
            startNode.IsStart = true;

            var (north, east, south, west) = (grid.Nodes[startNode.Id - (grid.Width * 2)], grid.Nodes[startNode.Id + 2], grid.Nodes[startNode.Id + (grid.Width * 2)], grid.Nodes[startNode.Id - 2]);

            if (north.Value.In(Symbols('S')))
            {
                startNode.Connections.Add(north.Id);
                north.Connections.Add(startNode.Id);
            }

            if (east.Value.In(Symbols('W')))
            {
                startNode.Connections.Add(east.Id);
                east.Connections.Add(startNode.Id);
            }

            if (south.Value.In(Symbols('N')))
            {
                startNode.Connections.Add(south.Id);
                south.Connections.Add(startNode.Id);
            }

            if (west.Value.In(Symbols('E')))
            {
                startNode.Connections.Add(west.Id);
                west.Connections.Add(startNode.Id);
            }

            return grid;

            static char[] Symbols(char i)
            {
                return i switch
                {
                    'N' => new[] { '|', 'L', 'J' },
                    'E' => new[] { '-', 'L', 'F' },
                    'S' => new[] { '|', 'F', '7' },
                    'W' => new[] { '-', 'J', '7' },
                    _ => null
                };
            }
        }

        private class PipeNode : PathFinder.Node
        {
            internal bool IsStart { get; set; }

            internal bool IsGround { get; set; }

            internal char Value { get; set; }
        }
    }
}
