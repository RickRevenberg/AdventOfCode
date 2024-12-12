namespace AdventOfCode.PuzzleSolvers._2024
{
    using Logic.Extensions;
    using Logic.Modules;

    public class Day_12 : DayBase2024
    {
        public override int Day => 12;

        private Grid<GardenNode> grid;
        private List<List<GardenNode>> nodeGroups;

        [SetUp]
        public async Task SetUp()
        {
            var input = await this.SplitInput();

            this.grid = Grid.CreateGrid<GardenNode>(input[0].Length, input.Count, (x, y, node) => node.PlantType = input[y][x].ToString());
            this.grid.AddAllConnections((fromNode, toNode) => fromNode.PlantType == toNode.PlantType);

            this.nodeGroups = new List<List<GardenNode>>();

            var nodeCache = new Dictionary<int, GardenNode>();
            foreach (var node in this.grid.Nodes.Values.Where(x => !nodeCache.ContainsKey(x.Id)))
            {
                var nodeGroup = this.grid.GetConnectedNodeGroup(node);
                nodeGroup.ForEach(node => nodeCache.Add(node.Id, node));

                this.nodeGroups.Add(nodeGroup);
            }
        }

        [Test]
        public void PartOne()
        {
            var total = 0;
            foreach (var group in this.nodeGroups)
            {
                var perimeterCount = group.Sum(x => 4 - x.Connections.Count);
                total += perimeterCount * group.Count;
            }

            total.Pass();
        }

        [Test]
        public void PartTwo()
        {
            var total = 0;
            foreach (var group in nodeGroups)
            {
                var vectors = new List<Vector2D>();

                foreach (var node in group)
                {
                    var connectionNodes = node.Connections.Select(x => this.grid.Nodes[x]).ToList();
                    if (connectionNodes.All(x => x.PosX >= node.PosX))
                    {
                        vectors.Add(new (node.PosX, node.PosY + 1, node.PosX, node.PosY));
                    }

                    if (connectionNodes.All(x => x.PosY >= node.PosY))
                    {
                        vectors.Add(new (node.PosX, node.PosY, node.PosX + 1, node.PosY));
                    }

                    if (connectionNodes.All(x => x.PosX <= node.PosX))
                    {
                        vectors.Add(new (node.PosX + 1, node.PosY, node.PosX + 1, node.PosY + 1));
                    }

                    if (connectionNodes.All(x => x.PosY <= node.PosY))
                    {
                        vectors.Add(new (node.PosX + 1, node.PosY + 1, node.PosX, node.PosY + 1));
                    }
                }

                for (var i = 0; i < vectors.Count; i++)
                {
                    var startVector = vectors[i];
                    var matchingVector = vectors.SingleOrDefault(x => x.From == startVector.To && x.Direction() == startVector.Direction());

                    if (matchingVector == null)
                    {
                        continue;
                    }

                    vectors[i] = new Vector2D(startVector.From, matchingVector.To);
                    vectors.Remove(matchingVector);
                        
                    i--;
                }

                total += (vectors.Count * group.Count);
            }

            total.Pass();
        }

        private class GardenNode : Grid.Node
        {
            internal string PlantType { get; set; }
        }
    }
}
