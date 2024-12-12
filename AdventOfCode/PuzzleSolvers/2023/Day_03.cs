namespace AdventOfCode.PuzzleSolvers._2023
{
    using System.Text.RegularExpressions;
    using Logic.Extensions;
    using Logic.Modules;
    
    using static Logic.Modules.Grid;

    public class Day_03 : DayBase2023
    {
        public override int Day => 3;

        private Grid<EngineNode> grid;

        [SetUp]
        public async Task SetUp()
        {
            var input = await this.SplitInput();
            this.grid = ParseInputToGrid(input);
        }

        [Test]
        public void PartOne()
        {
            var numberGroups = new List<List<EngineNode>>();

            for (var i = 0; i < grid.Nodes.Values.Count; i++)
            {
                var node = grid.Nodes[i];

                if (!node.IsNumber || !node.Connections.Any(x => grid.Nodes[x].IsPart) || (numberGroups.LastOrDefault()?.Contains(node) ?? false))
                {
                    continue;
                }

                numberGroups.Add(AllAdjacentNumbers(i, i));
            }

            var numbers = numberGroups.Select(x => x.OrderBy(y => y.Id).Join().ToInt()).ToList();
            numbers.Sum().Pass();
        }

        [Test]
        public void PartTwo()
        {
            var totalRatio = 0;

            foreach (var gearNode in grid.Nodes.Values.Where(x => x.IsGear))
            {
                var numberGroups = new List<List<EngineNode>>();
                foreach (var nodeId in gearNode.Connections)
                {
                    var node = grid.Nodes[nodeId];
                    if (!node.IsNumber || numberGroups.Any(x => x.Contains(node)))
                    {
                        continue;
                    }

                    numberGroups.Add(AllAdjacentNumbers(node.Id, node.Id));
                }

                if (numberGroups.Count == 2)
                {
                    totalRatio += (int)numberGroups.Select(x => (long)x.OrderBy(y => y.Id).Join().ToInt()).Product();
                }
            }

            totalRatio.Pass();
        }

        private static Grid<EngineNode> ParseInputToGrid(List<string> input)
        {
            var numberRegex = new Regex(@"\d+");
            var valueMap = input.Select(x => x.ToCharArray()).ToArray();

            return CreateGrid<EngineNode>(input[0].Length, input.Count, (x, y, node) =>
            {
                var value = valueMap[y][x];

                node.IsNumber = numberRegex.IsMatch(value.ToString());
                node.IsPart = !node.IsNumber && value != '.';
                node.Value = value;
                node.IsGear = value == '*';
            }).AddAllConnections(x => x.IncludeDiagonal = true, (_, to) => to.Value != '.');
        }

        private List<EngineNode> AllAdjacentNumbers(int from, int x)
        {
            var values = new List<EngineNode> { grid.Nodes[x] };
            foreach (var nodeIndex in grid.Nodes[x].Connections.Where(y => grid.Nodes[y].IsNumber && y != from))
            {
                values.AddRange(AllAdjacentNumbers(x, nodeIndex));
            }

            return values;
        }

        private class EngineNode : Node
        {
            internal char Value { get; set; }

            internal bool IsNumber { get; set; }
            internal bool IsPart { get; set; }
            internal bool IsGear { get; set; }

            public override string ToString()
            {
                return this.Value.ToString();
            }
        }
    }
}
